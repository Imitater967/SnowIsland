using System;
using System.Text;
using SnowIsland.Scripts.Item;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Inventory
{
    [Serializable]
    public struct ItemSlot: INetworkSerializable, IEquatable<ItemSlot>
    {
    
        public Item item;
        public int amount;

        // constructors
        public ItemSlot(Item item, int amount=1)
        {
            this.item = item;
            this.amount = amount;
        }

        // helper functions to increase/decrease amount more easily
        // -> returns the amount that we were able to increase/decrease by
        public int DecreaseAmount(int reduceBy)
        {
            // as many as possible
            int limit = Mathf.Clamp(reduceBy, 0, amount); 
            amount -= limit;
            if (amount<=0)
            {
                item.itemType = ItemType.Air;
            }
            return limit;
        }

        public int IncreaseAmount(int increaseBy)
        {
            // as many as possible
            int limit = Mathf.Clamp(increaseBy, 0, item.maxStack - amount);
            amount += limit;
            return limit;
        }

        // tooltip
        public string ToolTip()
        {
            if (amount == 0) return "";

            // we use a StringBuilder so that addons can modify tooltips later too
            // ('string' itself can't be passed as a mutable object)
            StringBuilder tip = new StringBuilder(item.ToolTip());
            tip.Replace("{AMOUNT}", amount.ToString());
            return tip.ToString();
        } 
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        { 
            if (serializer.IsWriter)
            {
                serializer.GetFastBufferWriter().WriteValueSafe(item);
                serializer.GetFastBufferWriter().WriteValueSafe(amount);
            }
            else
            {
                serializer.GetFastBufferReader().ReadValueSafe(out item); 
                serializer.GetFastBufferReader().ReadValueSafe(out amount);
            }
        }

        public bool Equals(ItemSlot other)
        {
            return item.Equals(other.item) && amount == other.amount;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemSlot other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(item, amount);
        }
    }
}