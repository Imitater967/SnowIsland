 
using System;
using SnowIsland.Scripts.Room;
using SnowIsland.Scripts.UI.Game;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerProfile))]
    public class PlayerNameDisplay: NetworkBehaviour
    {
        private PlayerProfile _playerProfile;
        public Vector3 offset = new Vector3(0, 2, 0);
        [SerializeField] private UIOverHeadDisplay nameDisplayPrefab;
        [SerializeField, Scripts.ReadOnly] private UIOverHeadDisplay nameDisplayInstance;
    

        private void OnEnable()
        {
            _playerProfile = GetComponent<PlayerProfile>();
            nameDisplayInstance = Instantiate(nameDisplayPrefab.gameObject,GameObject.Find("Canvas/NameDisplays").transform).GetComponent<UIOverHeadDisplay>();
            nameDisplayInstance.Init(GetComponent<PlayerCharacter>());
            nameDisplayInstance.nameField.text = _playerProfile.playerName.Value.SomeText;
            EnableClientRpc();
             
        }

        [ClientRpc]
        private void EnableClientRpc()
        {
            if (IsHost)
            { 
                return;   
            }
            enabled = true; 
        }  [ClientRpc]
        private void DisableClientRpc()
        {
            if (IsHost)
            { 
                return;   
            } 
            enabled = false; 
        }
        private void Update()
        {
            nameDisplayInstance.gameObject.transform.position =
                RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + offset);
        }

        private void OnDisable()
        {
            Destroy(nameDisplayInstance.gameObject);
            DisableClientRpc(); 
        }

      
    }
}