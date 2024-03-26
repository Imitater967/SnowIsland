using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Inventory;
using SnowIsland.Scripts.Item;
using UnityEngine;
using UnityEngine.Serialization;

namespace SnowIsland.Scripts.Room
{    [Serializable]
    public struct ItemReg
    {
        public ItemType ItemType;
        [FormerlySerializedAs("item")] public Item.ItemAsset itemAsset;
    }
    public class AbstractItemManager: MonoBehaviour
    {
        //更好的加载方式是Resource.Load,然后itemType存到asset里建立表格
        public static AbstractItemManager Instance;
        [field: SerializeField]
        public List<ItemReg> ItemRegTable { get; private set; }
        
        protected virtual void Awake()
        {
            if(Instance!=null)
                Debug.LogError("More Than 2 Item Manager");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public Item.ItemAsset GetItem(ItemType identity)
        {
            foreach (var identityPair in ItemRegTable)
            {
                if (identityPair.ItemType == identity)
                    return identityPair.itemAsset;
            }

            return null;
        }
    }
}