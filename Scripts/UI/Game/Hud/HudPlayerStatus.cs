using System;
using SnowIsland.Scripts.Character;
using UnityEngine;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game.Hud
{
    public class HudPlayerStatus: MonoBehaviour
    {
        [SerializeField] private Slider health;
        [SerializeField] private Slider healthTemp;
        [SerializeField] private Slider warmth;
        [SerializeField] private Slider food;
        private PlayerCharacter _playerCharacter;
        private PlayerStatus _playerStatus;
        private void Awake()
        {
            _playerCharacter=PlayerCharacter.Local;
            if (_playerCharacter == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _playerStatus = _playerCharacter.GetComponent<PlayerStatus>();
        }

        private void Update()
        {
            health.maxValue = _playerStatus.HealthMaxPB;
            health.value = _playerStatus.healthCurrent.Value;
            healthTemp.maxValue = _playerStatus.HealthMaxPB;
            healthTemp.value = Mathf.Abs(_playerStatus.tempHealthMax.Value);
            warmth.value = _playerStatus.warmthCurrent.Value;
            warmth.maxValue = _playerStatus.WarmthMax;
            food.value = _playerStatus.foodCurrent.Value;
            food.maxValue = _playerStatus.FoodMax;

        }
    }
}