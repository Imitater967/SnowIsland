using System;
using Cinemachine;
using SnowIsland.Scripts.Item;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotion: NetworkBehaviour
    {
        [SerializeField, ReadOnly] 
        public Vector3 velocity=new Vector3();

        [SerializeField] public Vector2 direction=new Vector2();
        [field: SerializeField]
        public bool isGrounded { get; private set; } = true;

        [field:SerializeField] 
        public NetworkVariable<bool> isRun { get; private set; } = new NetworkVariable<bool>();
        [field:SerializeField] 
        public NetworkVariable<bool> isCrawl { get; private set; } = new NetworkVariable<bool>();
        [SerializeField] 
        private float moveSpeed=2f;
        [SerializeField] 
        private float crawlSpeed=1f;
        [SerializeField] 
        private float runSpeed=3.5f;
        [SerializeField]
        private float rotateSpeedRun=10f;
        [SerializeField]
        private float rotateSpeedWalk=5f;
        [SerializeField,ReadOnly] 
        private float rotateInternal=5f;

        private float foodConsumePerSecondRunning=2;
        private CharacterController cc;
        private PlayerInteract _playerInteract;
        private PlayerStatus _playerStatus; 
        private PlayerHotbar _playerHotbar;
        private PlayerItemUsage _playerItemUsage;
        private PlayerGunUsage _playerGunUsage;
        private void Awake()
        {
            _playerGunUsage = GetComponent<PlayerGunUsage>();
            _playerItemUsage = GetComponent<PlayerItemUsage>();
            _playerHotbar = GetComponent<PlayerHotbar>(); 
            _playerStatus = GetComponent<PlayerStatus>();
            cc = GetComponent<CharacterController>();
            _playerInteract = GetComponent<PlayerInteract>();
        }
 
        private void Update()
        {
            if(!IsServer)
                return;
            if (isRun.Value&&_playerStatus.foodCurrent.Value>0)
            {
                _playerStatus.foodCurrent.Value -= foodConsumePerSecondRunning * Time.deltaTime;
                rotateInternal = rotateSpeedRun;
            }
            else
            {
                rotateInternal = rotateSpeedWalk;
            }
            
            cc.Move(velocity * Time.deltaTime);
            isGrounded = cc.isGrounded;
            ProcessY();
            //玩家交互的时候不允许移动
            ProcessXZ();
            //交互不允许移动
            if (!CheckCanMove())
            {
                velocity.x = 0;
                velocity.z = 0;
            }
            if(!_playerGunUsage.aiming.Value)
                RotateCharacter(velocity);
        }

        private bool CheckCanMove()
        {
            if (_playerInteract.serverInteracting || _playerInteract.serverConfirming)
            {
                return false;
            }

            if (_playerHotbar.itemInHand.asset is PlacementAsset )
                if (_playerItemUsage.isItemUsing.Value)
                    return false;
            return true;
        }
        private void ProcessXZ()
        {
            Vector2 direction = this.direction;
            if (isCrawl.Value)
            {
                direction *= crawlSpeed;
            }
            else
            {
                if (isRun.Value)
                {
                    direction *= runSpeed; 
                }
                else
                { 
                    direction *= moveSpeed;
                }   
            }

            velocity.x = Mathf.MoveTowards(velocity.x, direction.x, rotateInternal);
            velocity.z = Mathf.MoveTowards(velocity.z, direction.y, rotateInternal);
        }

        private void ProcessY()
        {
            if(!isGrounded){
                velocity.y +=-9.8f * Time.deltaTime;
            }
            else
            {
                //设置为-1,防止反复横跳
                velocity.y = -1f;
            }
        }
        private void RotateCharacter(Vector3 _direction)
        {
            _direction.y = 0;
            Vector3 targetForward = Vector3.RotateTowards(transform.forward, _direction.normalized, rotateInternal * Time.deltaTime,.1f);
            //往目标方向旋转 
            //使用normalized返回一个方向向量。
            Quaternion _newRotation = Quaternion.LookRotation(targetForward);//生成新四元数
            transform.rotation = _newRotation;//设置新四元数
        }
        /*
         * 如果参数没有改变的话, 在服务器端只执行了一次,
         */
        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        public void MoveServerRpc(Vector2 moveDir)
        { 
            direction = moveDir.normalized ; 
        }
        /*
         * 这里有一个坑,当moveDir没有改变的时候,velocity没有清零
         * 也就是只执行一次, 这个时候isRun改变,也不会执行下面的代码
         * 只有当moveDir改变后才会执行代码
         */
        // [ServerRpc]
        // public void MoveServerRpc(Vector2 moveDir)
        // { 
        //     Vector2 direction = moveDir.normalized ;
        //     if (isRun.Value)
        //     {
        //         direction *= runSpeed;
        //     }
        //     else
        //     {
        //         direction *= moveSpeed;
        //     }
        //     Debug.Log("Run");
        //     velocity.x = direction.x;
        //     velocity.z = direction.y;
        // }
    }
}