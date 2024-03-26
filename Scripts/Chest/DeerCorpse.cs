using Unity.Netcode;

namespace SnowIsland.Scripts.Chest
{
    public class DeerCorpse:NetworkBehaviour,ICorpse
    {
        [ServerRpc(RequireOwnership = false)]
        public void DestroyServerRpc( )
        { 
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}