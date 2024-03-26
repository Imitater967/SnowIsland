using System;
using SnowIsland.Scripts.Combat;
using SnowIsland.Scripts.Room;
using SnowIsland.Scripts.UI.Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SnowIsland.Scripts.Character
{ 
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerCharacter))]
    [RequireComponent(typeof(PlayerMotion))]
    public class PlayerControl: NetworkBehaviour,MyPlayerInputAsset.IInGameActions
    {
        public MyPlayerInputAsset myPlayerInputAsset;
        [ReadOnly,SerializeField]
        private PlayerMotion playerMotion;
        [ReadOnly,SerializeField]
        private PlayerHotbar playerHotbar;
        [ReadOnly,SerializeField]
        private PlayerInteract playerInteract;
        [ReadOnly,SerializeField]
        private PlayerGunUsage playerGunUsage;
        [ReadOnly, SerializeField] private PlayerItemUsage _playerItemUsage;
        [ReadOnly,SerializeField]
        private PlayerNameDisplay _playerNameDisplay;
        [Header("Aiming")]
        [SerializeField]
        private LayerMask aimLayerMask;
        [SerializeField] private Vector3 defaultAimOffset = new Vector3(0, 1.5f, 0);
        public bool hasAimTarget = false;
        [Header("Inv")]
        public bool split;
        private void Awake()
        {
            playerMotion = GetComponent<PlayerMotion>();
            _playerNameDisplay = GetComponent<PlayerNameDisplay>();
            playerHotbar = GetComponent<PlayerHotbar>();
            playerInteract = GetComponent<PlayerInteract>();
            _playerItemUsage = GetComponent<PlayerItemUsage>();
            playerGunUsage = GetComponent<PlayerGunUsage>();
            myPlayerInputAsset = new MyPlayerInputAsset();
            myPlayerInputAsset.InGame.SetCallbacks(this);
            enabled = false; 
            AbstractRoomManager.Instance.OnGameScenePreparedClient += () =>
            {
                if (IsLocalPlayer)
                    enabled = true;
            };
        }

      

        private void OnEnable()
        {
            myPlayerInputAsset.Enable();
        }

        private void OnDisable()
        {
            myPlayerInputAsset.Disable();
        }


        public void OnMotion(InputAction.CallbackContext context)
        {
            if(playerMotion==null)
                return;
            Vector2 moveDir=context.ReadValue<Vector2>(); 
            //交互时候不能移动
            //需要手动发送0,0否则会按照之前的状态移动,不明原因
            if (playerInteract.clientUIView!=null&&playerInteract.clientUIView.isPressing)
            { 
                playerMotion.MoveServerRpc(new Vector2(0,0));
                 return;
            } 
            playerMotion.MoveServerRpc(moveDir);
        }

        public void OnHideName(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(_playerNameDisplay==null)
                    return;
                ToggleNameDisplayServerRpc();
            }
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ToggleRunServerRpc(true);   
            }

            if (context.canceled)
            {
                ToggleRunServerRpc(false);
            }
        }

        public void OnCrawlTstOnly(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                playerMotion.MoveServerRpc(new Vector2(0,1));
            }
        }

        public void OnSplit(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                split = true;
            }

            if (context.canceled)
            {
                split = false;
            }
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            if (context.performed)
            {  
                playerHotbar.DropItemServerRpc();
            }
        }

        public void OnHotbarSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
            { 
               int selection =(int)context.ReadValue<float>(); 
               playerHotbar.SelectServerRpc(selection-1);
            }
        }

        public void OnInteractSwitch(InputAction.CallbackContext context)
        {
            if (context.performed)
            {     
                Debug.Log("Performed");
                //交互时候切换
                if (playerInteract.clientUIView!=null&&playerInteract.clientUIView.isPressing)
                {
                    return;
                }  
                playerInteract.clientUIView.Next(); 
            }
        }

        public void OnInteractPerform(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (playerInteract.clientUIView!=null)
                {
                    playerInteract.clientUIView.isPressing = true;
                } 
            }

            if (context.canceled)
            {
                if (playerInteract.clientUIView!=null)
                {
                    playerInteract.clientUIView.isPressing = false;
                } 
            }
        }

        public void OnStopInteract(InputAction.CallbackContext context)
        {
            playerInteract.StopInteractServerRpc();
        }

        public void OnUse(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _playerItemUsage.StartUseServerRpc();
            }
            if (context.canceled)
            {
                _playerItemUsage.CancelUseServerRpc();
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed)
            {

                playerGunUsage.ToggleAimServerRpc(true);
            }

            if (context.canceled)
            {
                playerGunUsage.ToggleAimServerRpc(false);
            }
 
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (playerGunUsage.aiming.Value)
                {
                    playerGunUsage.FireServerRpc();
                }
            }
        }

        private IAimTarget lastAimTarget;
        public void OnAimPos(InputAction.CallbackContext context)
        {
            //这里不应该用屏幕坐标,
            if(playerGunUsage.aiming.Value){
                // Vector2 pos = context.ReadValue<Vector2>();
                // pos = pos - new Vector2(Screen.width/2, Screen.height/2);
                // playerGunUsage.SendAimPosServerRpc(pos.normalized);

                var mousePos = context.ReadValue<Vector2>();
                var ray=Camera.main.ScreenPointToRay(mousePos);
                Physics.Raycast(ray: ray,out RaycastHit hit,100,aimLayerMask);
                bool isLocalPlayer = hit.collider.TryGetComponent(out PlayerCharacter playerCharacter) &&
                                     playerCharacter.IsLocalPlayer;
                if (hit.collider.gameObject.TryGetComponent(out IAimTarget damageReceiver)&&!isLocalPlayer
                  )
                {
                  
                    if (lastAimTarget != damageReceiver)
                    {
                        if (lastAimTarget != null)
                        {
                            lastAimTarget.OnLoseFocus();
                        }
                        damageReceiver.OnFocus();
                        lastAimTarget = damageReceiver;
                    }
                    hasAimTarget = true;
                    Vector3 relativePos= hit.collider.transform.position - transform.position+damageReceiver.GetAimOffset(); 
                    playerGunUsage.SendAimPosServerRpc(relativePos);
                    //在client也同步一份
                    playerGunUsage.aimPos = relativePos;
                }
                else
                {
                    if (lastAimTarget != null)
                    {
                        lastAimTarget.OnLoseFocus();
                        lastAimTarget = null;
                    }
                    hasAimTarget = false;
                    Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
                    var result=(mousePos - playerPos).normalized * 50;
                    Vector3 finalPos = new Vector3(result.x, 0, result.y) + defaultAimOffset;
                    playerGunUsage.SendAimPosServerRpc(finalPos);
                    //在client也同步一份
                    playerGunUsage.aimPos = finalPos;
                }
            }
        }
 
        [ServerRpc]
        private void ToggleCrawlServerRpc()
        {
            playerMotion.isCrawl.Value = !playerMotion.isCrawl.Value;
        }

        [ServerRpc]
        private void ToggleRunServerRpc(bool run)
        {
            playerMotion.isRun.Value = run;
        }
        [ServerRpc]
        private void ToggleNameDisplayServerRpc()
        {
            _playerNameDisplay.enabled = !_playerNameDisplay.enabled;
        }
    }
}