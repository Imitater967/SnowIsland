using System;
using SnowIsland.Scripts.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game.Hud
{
    public class HudPlayerIdentity : MonoBehaviour
    {
        [SerializeField] private Image idIcon;
        [SerializeField] private TMP_Text idName;
        [SerializeField] private TMP_Text idIntroduction;
        private PlayerCharacter _playerCharacter;
        private PlayerIdentity _playerIdentity;
        private void Awake()
        {
            _playerCharacter=PlayerCharacter.Local;
            if (_playerCharacter == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _playerIdentity = _playerCharacter.GetComponent<PlayerIdentity>();
            _playerIdentity.OnIdentityChangeClient += RefreshHud;
        }

        private void Start()
        {
            // RefreshHud(_playerIdentity.identityAsset);
        }

        private void RefreshHud(IdentityAsset obj)
        {
            idIcon.sprite = obj.Icon;
            idName.text = obj.Name;
            idIntroduction.text = obj.Introduction;
        }
    }
}