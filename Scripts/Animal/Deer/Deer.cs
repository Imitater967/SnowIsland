using System;
using BehaviorDesigner.Runtime;
using SnowIsland.Scripts.Combat;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Animal.Deer
{
    [RequireComponent(typeof(BehaviorTree))] 
    public class Deer: NetworkBehaviour,IDamageReceiver
    { 
        private BehaviorTree _behaviorTree;
        public IDamageTrigger killer;
        public bool dead = false;
        public float health=200;
        public Action OnDeerDeadOnServer;
        private void Awake()
        { 
            _behaviorTree = GetComponent<BehaviorTree>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer)
            {
                _behaviorTree.enabled = false;
            }
        }

        public bool IsAlive()
        {
            return health > 0;
        }

        public IDamageTrigger GetKiller()
        {
            return killer;
        }

        public void ReceiveDamage(DamageInfo damage)
        {
            killer = damage.DamageTrigger;
            health -= damage.Damage;
            _behaviorTree.SetVariableValue("Target",((MonoBehaviour)killer).gameObject);
            if (health <= 0&&!dead)
            {
                dead = true;
                OnDeerDeadOnServer?.Invoke();
            }
        }
 
    }
}