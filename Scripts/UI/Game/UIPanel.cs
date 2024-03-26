using System;
using UnityEngine;

namespace SnowIsland.Scripts.UI.Game
{
    public class UIPanel: MonoBehaviour
    {
        public Action OnClose;
        public Action OnOpen;

        protected virtual void OnEnable()
        {
            OnOpen?.Invoke();
            Debug.Log("Panel "+name+" Enabled");
        }

        protected virtual void OnDisable()
        {
            OnClose?.Invoke();
            Debug.Log("Panel "+name+" Disabled");
        }
    }
}