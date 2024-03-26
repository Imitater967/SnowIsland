using System;
using SnowIsland.Scripts.Item;
using UnityEngine;

namespace SnowIsland.Scripts.Chest
{
    [Serializable]
    public struct LootTable
    {
        public ItemType Item;
        public int AmountMax;
        public int AmountMin;
     [Range(0,1)] public float Probability;
    }
}