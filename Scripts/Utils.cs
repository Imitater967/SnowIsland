using UnityEngine;
using UnityEngine.AI;

namespace SnowIsland.Scripts
{
    public static class Utils
    {
        public static Vector3 RandomUnitCircleOnNavMesh(Vector3 position, float radiusMultiplier)
        {
            // random circle point
            Vector2 r = UnityEngine.Random.insideUnitCircle * radiusMultiplier;

            // convert to 3d
            Vector3 randomPosition = new Vector3(position.x + r.x, position.y, position.z + r.y);

            // raycast to find valid point on NavMesh. otherwise return original one
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, radiusMultiplier * 2, NavMesh.AllAreas))
                return hit.position;
            return position;
        }
        public static Vector3 ReachableRandomUnitCircleOnNavMesh(Vector3 position, float radiusMultiplier, int solverAttempts)
        {
            for (int i = 0; i < solverAttempts; ++i)
            {
                // get random point on navmesh around position
                Vector3 candidate = RandomUnitCircleOnNavMesh(position, radiusMultiplier);

                // check if anything obstructs the way (walls etc.)
                if (!NavMesh.Raycast(position, candidate, out NavMeshHit hit, NavMesh.AllAreas))
                    return candidate;
            }

            // otherwise return original position if we can't find any good point.
            // in that case it's best to just drop it where the entity stands.
            return position;
        }
    }
}