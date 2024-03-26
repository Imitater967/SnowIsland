using System;
using SnowIsland.Scripts.Character;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game
{
    public class UIOverHeadDisplay: MonoBehaviour
    {
        public TMP_Text nameField;
        public Scrollbar hpBar;
        public Image identity;
        [ReadOnly,SerializeField]
        private PlayerCharacter playerCharacter;

        private void Awake()
        {
            identity.gameObject.SetActive(false);
            hpBar.gameObject.SetActive(false);
        }

        public void Init(PlayerCharacter belong)
        {
            playerCharacter = belong;
        }

        private void Start()
        {
            if (playerCharacter == null ||playerCharacter.PlayerStatus.dead.Value)
            {
                enabled = false;
                return;
            }
            var playerId = PlayerCharacter.Local.PlayerIdentity.identityAsset;
            bool canSeeHpBar =  playerId.Identity == Identity.Doctor;
            bool canSeeIdentity = playerId.Identity == Identity.Imposter_TeamLeader;
            if (canSeeHpBar)
            {
                hpBar.gameObject.SetActive(true);
            }

            if (canSeeIdentity)
            {
                identity.gameObject.SetActive(true);
            }
            //整理layout
            if (canSeeIdentity && !canSeeHpBar)
            {
                var originPos = hpBar.transform.position;
                originPos.y = 18;
                hpBar.gameObject.transform.position = originPos;
            }
        }
 

        private void Update()
        {
            //这里性能虽然有损失,但是比较方便,就这样吧
            identity.sprite = playerCharacter.PlayerIdentity.identityAsset.Icon;
            hpBar.size = playerCharacter.PlayerStatus.healthCurrent.Value / playerCharacter.PlayerStatus.HealthMax;
        }
    }
}