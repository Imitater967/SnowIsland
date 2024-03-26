using SnowIsland.Scripts.Character;
using UnityEngine;

namespace SnowIsland.Scripts.Item
{
    [CreateAssetMenu(fileName = "asset", menuName = "SnowIsland/Item/Food", order = 0)]
    public class FoodAsset : UsableItemAsset
    {
        public int Health;
        public int Warmth;
        public int Hunger;
        [Range(0,1)]
        public float HealthPercent;
        [Range(0,1)]
        public float WarmthPercent;
        [Range(0,1)]
        public float HungerPercent; 
        public override void OnUseEnter(PlayerCharacter playerCharacter)
        {
            base.OnUseEnter(playerCharacter);
            var playerCharacterPlayerStatus = playerCharacter.PlayerStatus;
            playerCharacterPlayerStatus.foodCurrent.Value += Hunger;
            playerCharacterPlayerStatus.healthCurrent.Value += Health;
            playerCharacterPlayerStatus.warmthCurrent.Value += Warmth;
            playerCharacterPlayerStatus.foodCurrent.Value += playerCharacterPlayerStatus.FoodMax*HungerPercent;
            playerCharacterPlayerStatus.healthCurrent.Value += playerCharacterPlayerStatus.HealthMax*HealthPercent;
            playerCharacterPlayerStatus.warmthCurrent.Value += playerCharacterPlayerStatus.WarmthMax*WarmthPercent;
        }
    }
}