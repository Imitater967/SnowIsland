using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Item;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerHotbar))]
    public class PlayerItemUsage: NetworkBehaviour
    {
        private PlayerHotbar _playerHotbar;
        private PlayerPlacement _placement;
        private PlayerStatus _playerStatus;
        public NetworkVariable<bool> isItemUsing=new NetworkVariable<bool>(false);
        public bool useInputOnServer;
        public Item.ItemAsset current => _playerHotbar.itemInHand.asset;
        private PlayerInteract _playerInteract;
        private List<Func<bool>> checkCanUse=new List<Func<bool>>();
        private void Awake()
        {
            _placement = GetComponent<PlayerPlacement>();
            _playerStatus = GetComponent<PlayerStatus>();
            _playerInteract = GetComponent<PlayerInteract>();
            _playerHotbar = GetComponent<PlayerHotbar>();
            checkCanUse.Add(() => _playerInteract.serverInteractable == null);
            checkCanUse.Add(()=>!_playerStatus.dying.Value&&!_playerStatus.dead.Value);
            checkCanUse.Add(() =>
            {
                if (_playerHotbar.itemInHand.asset is PlacementAsset)
                {
                    return _placement.canPlace.Value;
                } 
                return true;
            });
        } 
        [ServerRpc]
        public void StartUseServerRpc()
        {
            if ( current is not UsableItemAsset)
            {
                return;
            }
            //交互中是不能使用东西的 
            for (var i = 0; i < checkCanUse.Count; i++)
            {
                if(!checkCanUse[i].Invoke())
                    return;
            }
            useInputOnServer = true;
            // if (_playerInteract.serverPreviewing != null)
            // {
            //     _playerInteract.StopView();
            //   // _playerInteract.serverPreviewing.EndView(_playerInteract);
            // }

        }
        [ServerRpc]
        public void CancelUseServerRpc()
        {
            useInputOnServer = false;
        }
    }
}