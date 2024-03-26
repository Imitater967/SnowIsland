using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Chest;
using SnowIsland.Scripts.Inventory;
using SnowIsland.Scripts.Resources;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStatus))]
    public class PlayerDeath: NetworkBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        [SerializeField] private Material _material;
        [SerializeField, Tooltip("玩家死亡后的箱子")] private GameObject chestPrefab;
        [SerializeField] private List<GameObject> objectToDestroy;
        [SerializeField]
        private ParticleSystem snowTrail;
        private PlayerStatus _playerStatus;
        private PlayerProfile _profile;
        private PlayerInteract _playerInteract;
        private PlayerHotbar _playerHotbar;
        private void Awake()
        {
            _playerHotbar = GetComponent<PlayerHotbar>();
            _profile = GetComponent<PlayerProfile>();
            _playerInteract = GetComponent<PlayerInteract>();
            _playerStatus = GetComponent<PlayerStatus>();
            _playerStatus.OnPlayerDeathServer += OnPlayerDeath;
        }

        private void ChangeLayerAndDestroyObject()
        {
            _meshRenderer.material = _material;
            gameObject.layer = LayerMask.NameToLayer("Ghost"); 
            _meshRenderer.gameObject.layer=LayerMask.NameToLayer("Ghost"); 
            Destroy(GetComponent<PlayerIndicator>());
            snowTrail.Stop(true);
            foreach (var o in objectToDestroy)
            {
                Destroy(o);
            }    
        }
        public void OnPlayerDeath()
        {
            ChangeLayerAndDestroyObject();
            ChangeLayerAndDestroyObjectsClientRpc();
            ChangeCameraLayerClientRpc(new ClientRpcParams(){Send = new ClientRpcSendParams()
            {
                TargetClientIds = new []{OwnerClientId}
            }});
           SpawnCorpse();
           // _playerInteract.StopView();
        }

        private void SpawnCorpse()
        {
            GameObject corpse= Instantiate(chestPrefab,transform.position,Quaternion.identity);
            corpse.GetComponent<NetworkObject>().Spawn();
            corpse.GetComponent<PlayerCorpse>().name = _profile.playerName.Value.SomeText;
            var chestStorage = corpse.GetComponent<ChestStorage>();
            chestStorage.name.Value = _profile.playerName.Value;
            for (var i = 0; i < _playerHotbar.slots.Count; i++)
            {
                chestStorage.slots[i] = new ItemSlot() { item = _playerHotbar.slots[i].item,amount = _playerHotbar.slots[i].amount};
                _playerHotbar.slots[i] = new ItemSlot();
            }
        }

        [ClientRpc]
        private void ChangeCameraLayerClientRpc(ClientRpcParams @params)
        {
            Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("Ghost"));
        }
        [ClientRpc]
        private void ChangeLayerAndDestroyObjectsClientRpc()
        {
            if(IsHost)
                return;
            ChangeLayerAndDestroyObject();
        }
    }
}