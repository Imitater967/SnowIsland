using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnowIsland.Scripts
{
    public class WaypointList : MonoBehaviour
    {
        [SerializeField,ReadOnly]
        private List<GameObject> points;

        private void Awake()
        {
            points = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                points.Add(transform.GetChild(i).gameObject);
            }
        }

        public object GetGameObjectList()
        {
            return points;
        }
    }
}