using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tactical;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace SnowIsland.Scripts.Actions
{
    public class TargetSpotted: Conditional
    {
        public SharedGameObject returnedObjects;
        public float CheckRange=50;
        public override TaskStatus OnUpdate()
        {  
            if (returnedObjects.Value!=null)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}