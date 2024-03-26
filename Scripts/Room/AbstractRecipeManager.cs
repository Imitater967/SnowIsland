using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Item;
using UnityEngine;

namespace SnowIsland.Scripts.Room
{
    public class AbstractRecipeManager : MonoBehaviour
    {
        public static AbstractRecipeManager Instance;
        [field: SerializeField]
        public List<RecipeAsset> recipes { get; private set; }
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than 2 recipe Manager in scene");
            }
            Instance = this;
        }

        public RecipeAsset GetRecipe(int id)
        {
            return recipes[id];
        }
    }
}