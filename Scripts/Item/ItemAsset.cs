using System;
using SnowIsland.Scripts.Inventory;
using UnityEngine;

namespace SnowIsland.Scripts.Item
{
    //enum只可以往后续写,不然会出问题!!
    public enum ItemType
    {
      Air=0,
      Log=1,Stone=2,//材料1~10
      Pickaxe=10,//工具10~15
      RawMeat=20,CookedMeat=21,Beer=22,//食物20~30
      M4A4=31,//枪械30~40
      //道具40~60
      //放置物品
      Bonfire=71,Mine=72,

    }
    [CreateAssetMenu(fileName = "asset", menuName = "SnowIsland/Item/Material", order = 0)]
    public class ItemAsset : ScriptableObject
    {
        [Header("Base Stats")]
        public int maxStack = 1; 
        public int maxDurability = 5; 
        [TextArea(1, 30)] public string toolTip;
        public Sprite image;

        [Header("3D Representation")]
        public ItemDropped drop; 
        public GameObject modelPrefab;  
    }
    [Serializable]
    public struct ScriptableItemAndAmount
    {
        public ItemType item;
        public int amount;
    }
}