using SnowIsland.Scripts.UI.Game;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Facilities
{
    public class CraftingTable : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        public void OpenServerRpc(NetworkBehaviourReference playerInteractRef)
        {
            OpenUIClientRpc(new ClientRpcParams(){Send = new ClientRpcSendParams(){TargetClientIds = new []{((NetworkBehaviour)playerInteractRef).OwnerClientId}}});
        }
        [ClientRpc]
        private void OpenUIClientRpc(ClientRpcParams @params)
        {
            UIPanelManager.Instance.Open(UIType.Crafting);
        }
    }
}