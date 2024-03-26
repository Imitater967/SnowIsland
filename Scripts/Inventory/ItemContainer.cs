using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Network;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Inventory
{
    public abstract class ItemContainer : NetworkBehaviour
    {
        public NetworkVariable<StringContainer> name=new NetworkVariable<StringContainer>();
        [field: SerializeField] public int size { get; private set; } = 3;
        [SerializeField]
        public NetworkList<ItemSlot> slots;

        protected virtual void Awake()
        {
            slots= new NetworkList<ItemSlot>();
        }
#if UNITY_EDITOR
        [ReadOnly]
        public List<ItemSlot> itemDebugDisplay;
#endif
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            { 
                for (int i = 0; i < size; i++)
                {
                    slots.Add(new ItemSlot());
                }
            }
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            itemDebugDisplay=new List<ItemSlot>();
            for (var i = 0; i < slots.Count; i++)
            {
                itemDebugDisplay.Add(slots[i]);
            }      
#endif
        }
    }
}