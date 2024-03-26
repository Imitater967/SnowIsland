using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnowIsland.Scripts.UI.Game
{
    public enum UIType
    {
        Chest,Crafting
    }

    [Serializable]
    public struct UIPair
    {
        public UIType UIType;
        public UIPanel Panel;
    }
    public class UIPanelManager : MonoBehaviour
    {
        [SerializeField]
        private List<UIPair> UIPairs;

        [SerializeField,ReadOnly] private UIPanel lastPanel;
        public static UIPanelManager Instance;
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There Is More Than 2 UIPanelManager On The Scene");
                return;
            }
            Instance = this;
        }

        public void Open(UIType type)
        {
            Close();
            lastPanel = GetUIObject(type);
            lastPanel.gameObject.SetActive(true);
        } 
        public void Close()
        {
            if (lastPanel!=null)
                lastPanel.gameObject.SetActive(false);
            Debug.Log("Ingame UI Panel Manager: Closed UI");
        }

        private UIPanel GetUIObject(UIType uiType)
        {
            for (var i = 0; i < UIPairs.Count; i++)
            {
                var current = UIPairs[i];
                if (current.UIType == uiType)
                { 
                    Debug.Log("Ingame UI Panel Manager: Opening UI "+uiType);
                    return current.Panel;
                }
            }
            Debug.Log("Ingame UI Panel Manager: UI not found with type "+uiType);
            return null;
        }
    }
}