using System;
using System.Text;
using SnowIsland.Scripts.Item;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Inventory
{
    [Serializable]
    public struct Item: INetworkSerializable,IEquatable<Item>
    {
        public ItemType itemType;
        public int durability;
        public int ammo;
        public Scripts.Item.ItemAsset asset => AbstractItemManager.Instance.GetItem(itemType);
        public string name => asset.name;
        public int maxStack => asset.maxStack;
        public int maxDurability => asset.maxDurability;
        public Sprite image => asset.image;
        public bool CheckDurability() =>
            maxDurability == 0 || durability > 0;
        public float DurabilityPercent()
        {
            return (durability != 0 && maxDurability != 0) ? (float)durability / (float)maxDurability : 0;
        } 
        public Item(ItemType itemType)  
        {
            this.itemType=itemType;
            ammo = 0;
            var itemAsset = AbstractItemManager.Instance.GetItem(itemType);
            durability = itemAsset.maxDurability;
        }
        
        public string ToolTip()
        {
            // we use a StringBuilder so that addons can modify tooltips later too
            // ('string' itself can't be passed as a mutable object)
            StringBuilder tip = new StringBuilder(asset.toolTip);
            tip.Replace("{AMMO}", ammo.ToString());
            // show durability only if item has durability
            if (maxDurability > 0)
                tip.Replace("{DURABILITY}", (DurabilityPercent() * 100).ToString("F0"));
            return tip.ToString();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
        // public ItemType itemType;
        // public int durability;
        // public int ammo;
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(itemType);
            serializer.GetFastBufferWriter().WriteValueSafe(durability);
            serializer.GetFastBufferWriter().WriteValueSafe(ammo);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out itemType); 
            serializer.GetFastBufferReader().ReadValueSafe(out durability);
            serializer.GetFastBufferReader().ReadValueSafe(out ammo);
        }
        }

        public bool Equals(Item other)
        {
            return itemType == other.itemType && durability == other.durability && ammo == other.ammo;
        }

        public override bool Equals(object obj)
        {
            return obj is Item other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)itemType, durability, ammo);
        }
    }
}