using System;
using SnowIsland.Scripts.UI.Game;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    
    public class PlayerDyingDisplay: NetworkBehaviour
    {
        private PlayerStatus _playerStatus;
        public Vector3 offset = new Vector3(0, 2, 0);
        [SerializeField] private UIPlayerDyingDisplay nameDisplayPrefab;
        [SerializeField, Scripts.ReadOnly] private UIPlayerDyingDisplay nameDisplayInstance;

        private void Awake()
        {
            _playerStatus = GetComponent<PlayerStatus>();
            enabled = false;
            _playerStatus.OnPlayerDyingServer += EnableClientRpc;
            _playerStatus.OnPlayerRecoverServer += DisableClientRpc;
            _playerStatus.OnPlayerDeathServer += DisableClientRpc;
        }

        [ClientRpc]
        private void EnableClientRpc()
        { 
            enabled = true; 
        }  [ClientRpc]
        private void DisableClientRpc()
        { 
            enabled = false; 
        }
        private void OnEnable()
        { 
            nameDisplayInstance = Instantiate(nameDisplayPrefab.gameObject,GameObject.Find("Canvas/Dying").transform).GetComponent<UIPlayerDyingDisplay>();
            nameDisplayInstance.Init(GetComponent<PlayerCharacter>());  
             
        }

 
        private void Update()
        {
            nameDisplayInstance.gameObject.transform.position =
                RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + offset);
        }

        private void OnDisable()
        {
            if(nameDisplayInstance!=null)
            Destroy(nameDisplayInstance.gameObject); 
        }

     
    }
}