using System;
using UnityEngine;

namespace SnowIsland.Scripts
{
    public class RotateYAxis : MonoBehaviour
    {
        public int speed = 30;
        private void Update()
        {
            transform.Rotate(Vector3.up,speed*Time.deltaTime);
        }
    }
}