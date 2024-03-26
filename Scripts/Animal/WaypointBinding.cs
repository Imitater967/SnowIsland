using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace SnowIsland.Scripts.Animal
{
    [RequireComponent(typeof(BehaviorTree))]
    public class WaypointBinding : MonoBehaviour
    {
        private BehaviorTree _behaviorTree;
        public WaypointList waypoints;
        private void Awake()
        {
            _behaviorTree = GetComponent<BehaviorTree>();
        }

        private void Start()
        {
            _behaviorTree.SetVariableValue("Waypoints",waypoints.GetGameObjectList());
        }
    }
}