using SnowIsland.Scripts.Character;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Item
{

    [CreateAssetMenu(fileName = "asset", menuName = "SnowIsland/Item/Placement", order = 0)]
    public class PlacementAsset : UsableItemAsset
    {
        [Header("放置物品")]
        [Tooltip("生成的物品")]
        public GameObject objectToSpawn;
        public override void OnUseEnter(PlayerCharacter playerCharacter)
        {
            base.OnUseEnter(playerCharacter);
            var placement = playerCharacter.PlayerPlacement;
            placement.PlaceClientRpc();
        }
    }
}