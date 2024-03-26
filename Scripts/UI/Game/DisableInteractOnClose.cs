using System;
using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Inventory;
using UnityEngine;

namespace SnowIsland.Scripts.UI.Game
{
    [RequireComponent(typeof(UIPanel))]
    public class DisableInteractOnClose: MonoBehaviour
    {
        private UIPanel _uiPanel;
        private void Awake()
        {
            _uiPanel = GetComponent<UIPanel>();
            _uiPanel.OnClose += CheckIfChest;
        }
        //既然一个UI对应一个Interact,而且一次只会出现一个,那么在关闭UI的时候,就停止交互
         private void CheckIfChest()
         {
             var localPlayerInteract = PlayerCharacter.Local.PlayerInteract;  
             localPlayerInteract.StopInteractServerRpc();
         }
    }
}