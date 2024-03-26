using UnityEngine;

namespace SnowIsland.Scripts.Item
{
    [CreateAssetMenu(fileName = "asset", menuName = "SnowIsland/Craft/Recipe", order = 0)]
    public class RecipeAsset : ScriptableObject
    {
        [field: SerializeField]
        public ScriptableItemAndAmount ingredientA { get; private set; }
        [field: SerializeField]
        public ScriptableItemAndAmount ingredientB{ get; private set; }
        [field: SerializeField]
        public ScriptableItemAndAmount result{ get; private set; }
    }
}