using System;
using SnowIsland.Scripts.Item;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerMotion))]
    public class PlayerAnimation : NetworkBehaviour
    {
        public float fallThreshold = -3.5f;
        public float runThreshold = 2.5f;
        [SerializeField,ReadOnly]
        private float runSqr;
        private PlayerMotion _playerMotion;
        private Animator _animator;
        private PlayerInteract _playerInteract;
        private NetworkAnimator _networkAnimator;
        private PlayerItemUsage _playerItemUsage;
        private PlayerGunUsage _playerGun;
        private static readonly int _fall = Animator.StringToHash("Fall");
        private static readonly int _speed = Animator.StringToHash("Speed");
        private static readonly int _crawl = Animator.StringToHash("Crawl");
        private static readonly int _interacting = Animator.StringToHash("Interacting");
        private static readonly int _interaction = Animator.StringToHash("Interaction");
        private static readonly int _confirming = Animator.StringToHash("Confirming");
        private static readonly int _using = Animator.StringToHash("Using");
        private static readonly int _item = Animator.StringToHash("Item");
        private static readonly int _injury = Animator.StringToHash("Injury");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerInteract = GetComponent<PlayerInteract>();
            _playerMotion = GetComponent<PlayerMotion>();
            _playerItemUsage = GetComponent<PlayerItemUsage>();
            _networkAnimator = GetComponent<NetworkAnimator>();
            _playerGun = GetComponent<PlayerGunUsage>();
        }

        private void OnEnable()
        {
            runSqr = (float)Math.Pow(runThreshold, 2);
        }

        private void Update()
        {
            if(!IsServer)
                return;
            var velocity = _playerMotion.velocity;
            var speed2d = new Vector2(velocity.x, velocity.z).magnitude; 
            _animator.SetFloat(_speed,speed2d);
            if (_playerGun.aiming.Value)
            {
                //向(0,0,-1)先走,如果速度为(0,0,-1),那么结果为(0,0,1)
                //TransformDirection会将 (0,0,1)转化为(0,0,-1)
                //所以我们需要用InverseTransform
                Vector3 pos = new Vector3(_playerMotion.direction.x,0,_playerMotion.direction.y);
                pos = transform.InverseTransformDirection(pos).normalized;
                _animator.SetFloat("X",pos.x);
                _animator.SetFloat("Z",pos.z); 
            }
            else
            {
                //在非瞄准状态下,走的方向就是玩家前进的方向, 
                _animator.SetFloat("X", 0);
                _animator.SetFloat("Z",1);
            }
            
            _animator.SetBool(_crawl,_playerMotion.isCrawl.Value);
            _animator.SetBool(_fall,velocity.y<=fallThreshold); 
            {
                _animator.SetBool(_confirming,_playerInteract.serverConfirming);
                _animator.SetBool(_interacting,_playerInteract.serverInteracting);
                _animator.SetInteger(_interaction,_playerInteract.interactAnimParam);
            }
            _animator.SetBool(_using,_playerItemUsage.useInputOnServer);
            if (_playerItemUsage.current is IItemAnimation)
            {
                var usable = _playerItemUsage.current as IItemAnimation;
                _animator.SetInteger(_item,usable.GetItemAnimation());
            }
            else
            {
                StopUseItem();
            }
        }

        public void TriggerInjury()
        {
           _networkAnimator.SetTrigger(_injury); 
        }
        public void StopUseItem()
        {
          
            _animator.SetInteger(_item,-1);
        }
    }
}