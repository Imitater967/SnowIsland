using System;
using SnowIsland.Scripts.Inventory;
using UnityEngine;

namespace SnowIsland.Scripts.Resources
{
    [Serializable]
    public class ItemDropChance
    {
        public ItemDropped drop;
        [Range(0,1)] public float probability;
    }

}