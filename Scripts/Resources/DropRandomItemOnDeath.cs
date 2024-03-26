using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Resources
{
    public class DropRandomItemOnDeath : MonoBehaviour
    {
        public ItemDropChance[] dropChances;
        void DropItemAtRandomPosition(GameObject dropPrefab)
        {
            // drop
            Vector3 position = Utils.ReachableRandomUnitCircleOnNavMesh(transform.position, 1, 3);

            GameObject drop = Instantiate(dropPrefab,  position, Quaternion.identity);
            drop.GetComponent<NetworkObject>().Spawn(true);
        }

        public void OnDeath()
        {
            foreach (ItemDropChance itemChance in dropChances)
                if (Random.value <= itemChance.probability)
                    DropItemAtRandomPosition(itemChance.drop.gameObject);
        }
    }
}