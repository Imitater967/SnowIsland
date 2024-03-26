using System;
using SnowIsland.Scripts.Character;
using UnityEngine;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game
{
    public class UIPlayerDyingDisplay : MonoBehaviour
    {
        public Image fill;
        
        [ReadOnly,SerializeField]
        private PlayerCharacter playerCharacter;

        public void Init(PlayerCharacter playerCharacter)
        {
            this.playerCharacter = playerCharacter;
        }

        private void Update()
        {
            fill.fillAmount=  playerCharacter.PlayerStatus.liftUpRemian.Value / playerCharacter.PlayerStatus.liftUpMax;
        }
    }
}