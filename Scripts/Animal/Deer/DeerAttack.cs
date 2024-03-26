using System;
using BehaviorDesigner.Runtime.Tactical;
using UnityEngine;

namespace SnowIsland.Scripts.Animal.Deer
{
    public class DeerAttack: MonoBehaviour 
    {
        private DeerAnimation _deerAnimation;

        private void Awake()
        {
            _deerAnimation = GetComponent<DeerAnimation>();
        }

        //Invoke by animation event
        public void OnAttackEvent()
        {
            
        }

      
    }
}