using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace SnowIsland.Scripts.Actions
{
    public class AttackTargetNonStop: Action
    {
        public SharedFloat attackIntervalInSecond = 3;
        public bool debugInfo = false;
        private float nextAttackTime;

        public override TaskStatus OnUpdate()
        {
            if (Time.time < nextAttackTime)
            {
                return TaskStatus.Success;
            }
            nextAttackTime = Time.time + attackIntervalInSecond.Value;
            gameObject.SendMessage("BeginAttack");
            return TaskStatus.Success;
        }
    }
}