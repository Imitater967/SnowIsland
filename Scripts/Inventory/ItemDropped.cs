using SnowIsland.Scripts.Item;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Inventory
{
    public class ItemDropped : NetworkBehaviour
    {
        [Header("Item")]
        // default itemData, can be assigned in Inspector
#pragma warning disable CS0649 // Field is never assigned to
        [SerializeField] ItemType defaultItemToSpawn; // not public, so that people use .item & .amount
#pragma warning restore CS0649 // Field is never assigned to

        // drops need a real Item + amount so that we can set dynamic stats like ammo
        // note: we don't use 'ItemSlot' so that 'amount' can be assigned in Inspector for default spawns
         public NetworkVariable<int> amount = new NetworkVariable<int>(1); // sometimes set on server, needs to sync
        
         public NetworkVariable<Item> item=new NetworkVariable<Item>();
 
        void Awake()
        { 
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                if (defaultItemToSpawn != ItemType.Air&&item.Value.itemType==ItemType.Air)
                {
                    item.Value = new Item(defaultItemToSpawn);
                }

                if (amount.Value == 0)
                {
                    amount.Value = 1;
                }
            }
        }
    }
}