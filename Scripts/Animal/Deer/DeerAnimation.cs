using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace SnowIsland.Scripts.Animal.Deer
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class DeerAnimation : NetworkBehaviour
    {
        private NavMeshAgent _agent;
        private Animator _animator;
        [SerializeField,ReadOnly]
        private float speed;

        private static readonly int _attack = Animator.StringToHash("Attack");
        private static readonly int _dead = Animator.StringToHash("Dead");
        private static readonly int _speed = Animator.StringToHash("Speed");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (IsServer)
            {
                speed = _agent.velocity.magnitude;
                _animator.SetFloat(_speed,speed); 
            }
        }

        public void BeginAttack()
        {
            _animator.SetTrigger(_attack);
        }
        public void MarkDead(){
            _animator.SetBool(_dead,true);
        }
    }
}