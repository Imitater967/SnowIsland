using Unity.Netcode;

namespace SnowIsland.Scripts.Chest
{
    public interface ICorpse
    {
        [ServerRpc]
        void DestroyServerRpc();
    }
}