using System;
using SnowIsland.Scripts.Interact;
using SnowIsland.Scripts.UI.Game;
using SnowIsland.Scripts.UI.Game.Interact;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace SnowIsland.Scripts.Character
{
    /*
     * 玩家交互
     */
    [DisallowMultipleComponent]
    public class PlayerInteract: NetworkBehaviour
    {
        //当前正在交互的,仅存在于服务器 
        public Interactable serverInteractable;
        public Interactable clientInteractable; 
        public Interactable serverPreviewing;
        //交互面板,仅存在于客户端 
        public UIInteractPreview clientUIView;
        [SerializeField,ReadOnly]
        public int interactAnimParam;
        [SerializeField,ReadOnly]
        //是否正在交互中,仅存在于服务器
        public bool serverInteracting;
        [SerializeField,ReadOnly]
        //是否正在交互中,仅存在于服务器
        public bool serverConfirming;

        [SerializeField] private float interactDistance = 1; 
        private PlayerItemUsage playerItemUsage;
        private PlayerStatus _playerStatus;
        private PlayerGunUsage playerGunUsage;
        private void Awake()
        {
            _playerStatus = GetComponent<PlayerStatus>();
            playerGunUsage = GetComponent<PlayerGunUsage>();
            playerItemUsage = GetComponent<PlayerItemUsage>();
        }
         
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            enabled = IsServer;
     
        }
 
        /*
         * 一个坑,netcode的networkbehaviour的ontriggerenter只会在服务器端运行
         */
        private void Update()
        {
            if(!IsServer)
                return;
            if (!CheckCanPreview())
            {
                if (serverPreviewing != null)
                { 
                    StopView();
                } 
                return;
            }  
            Collider[] colliders = new Collider[10];
            Physics.OverlapSphereNonAlloc(transform.position, 1, colliders);
            float distanceSqr = float.MaxValue;
            Interactable closest = null;
            //寻找最近的碰撞体
            foreach (var collider1 in colliders)
            {
              
                if (collider1 != null&&collider1.gameObject != gameObject&&collider1.isTrigger&&collider1.TryGetComponent(out Interactable interactable))
                {
                    //获取碰撞点距离,两点距离减去方形碰撞体半径
                    var position = transform.position;
                    float distance =  (collider1.ClosestPoint(position)-position).sqrMagnitude;
                    if ( distance<= distanceSqr && distance<=(Mathf.Sqrt(interactDistance)))
                    {
                        closest = interactable;
                    }
                }
            }
            //如果最近的碰撞体与当前预览的不一样,则停止预览当前的交互面板
            if (closest!=serverPreviewing)
            { 
                StopView();
                //当前交互不为空,则开始预览新的交互面板
                if (closest != null)
                {  
                    serverPreviewing = closest;
                    //显示新的交互区域的预览窗口
                    serverPreviewing.StartView(this);   
                //    Debug.Log(name+" is now Previewing "+serverPreviewing.name);
                }
            }
            
        }

        // private void OnTriggerEnter(Collider other)
        // {
        //     if (serverConfirming || serverInteracting)
        //     {
        //         return;
        //     }
        //     if (other.TryGetComponent(out Interactable interactZone))
        //     { 
        //          StopView();
        //         if (!CheckCanInteract(interactZone))
        //         {  
        //             return;
        //         }
        //         //这里需要走!=,因为unity没有重写?的operation,missing!=null
        //         //currentViewing?.EndView(this);
        //         //使用物品时候,不进行交互操作
        //         serverPreviewing = interactZone;
        //         //显示新的交互区域的预览窗口
        //         serverPreviewing.StartView(this); 
        //     }
        //      
        // }

        public void StopView()
        {
            if (serverPreviewing != null)
            { 
                serverPreviewing.EndView(this);
                serverPreviewing = null;
            }
        }
        private bool CheckCanPreview()
        { 
            //serverConfirming || 
            if (serverPreviewing!=null&&serverPreviewing.transform == transform)
            {
                return false;
            }
            if (serverInteracting)
            {
                return false;
            }
            if (_playerStatus.dead.Value || _playerStatus.dying.Value)
                return false;
            if(playerItemUsage.isItemUsing.Value)
                return false;
            //  由于未知的原因,这个值永远是false,故嫁接一下到aiming变量中去
             if (playerGunUsage.aiming.Value) 
                return false;
            return true;
        }

        // private bool CheckCanInteract(Interactable other)
        // {
        //     if (other.transform == transform)
        //         return false;
        //     if (_playerStatus.dead.Value || _playerStatus.dying.Value)
        //         return false;
        //     if(playerItemUsage.isItemUsing.Value)
        //         return false;
        //     if (playerGunUsage.aiming.Value)
        //         return false;
        //     return true;
        // }

        private void OnTriggerExit(Collider other)
        { 
                if (other.TryGetComponent(out Interactable interactZone))
                {
                    if (interactZone==serverPreviewing)
                    {
                        //关闭当前的预览窗口
                       StopView();
                    } 
                } 
        }    
        /*
         * 发送确认交互请求,确认交互是开启面板后按E就是开始确认
         */
        [ServerRpc]
        public void RequestConfirmServerRpc(bool isPressing, int interaction)
        {
            this.serverConfirming = isPressing;
            this.interactAnimParam = interaction;
        }
        /*
         * 按E一定时间后,则标记为开始交互
         */
        [ServerRpc]
        public void RequestInteractServerRpc()
        {
            serverConfirming = false;
            this.serverInteracting = true;
            serverPreviewing.EndView(this);
            serverInteractable = serverPreviewing;
            serverInteractable.AddInteractor(this);
            RefreshInteractableClientRpc(serverInteractable,new ClientRpcParams()
            {
                Send = new ClientRpcSendParams(){TargetClientIds = new []{OwnerClientId}}
            });
        }

        [ClientRpc]
        private void RefreshInteractableClientRpc(NetworkBehaviourReference serverInteractable,ClientRpcParams @params)
        {
            clientInteractable = (Interactable)serverInteractable;
        }
        //实际上,应该在开启箱子的时候,给予权限
        [ClientRpc]
        private void StopClientInteractClientRpc()
        {
            clientInteractable = null;
        }
 
        public void StopInteractOnServer()
        {
            serverInteracting = false;
            if(serverInteractable!=null)
                serverInteractable.StopInteract(this);
            StopClientInteractClientRpc();
            serverInteractable = null; 
        }
        [ServerRpc]
        public void StopInteractServerRpc()
        { 
            StopInteractOnServer();
        }
    }
}