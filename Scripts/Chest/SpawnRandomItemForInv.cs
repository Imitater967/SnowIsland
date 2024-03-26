using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnowIsland.Scripts.Chest
{
    [RequireComponent(typeof(Inventory.Inventory))]
    public class SpawnRandomItemForInv : NetworkBehaviour
    {
        public LootTable[] loots;
        private Inventory.Inventory _inventory;
        public override void OnNetworkSpawn()
        {
            if(!IsServer)
                return;
            _inventory = GetComponent<Inventory.Inventory>();
            
            foreach (var lootTable in loots)
            {
                if (Random.value <= lootTable.Probability)
                {
                    int amount = Random.Range(lootTable.AmountMin, lootTable.AmountMax+1);
                    _inventory.Add(new Inventory.Item(lootTable.Item), amount);
                }
            }
        }
    }
}