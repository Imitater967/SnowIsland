using SnowIsland.Scripts.Network;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    public class PlayerProfile : NetworkBehaviour
    {
        public NetworkVariable<StringContainer> playerName=new NetworkVariable<StringContainer>();
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                
                var playerData=AbstractRoomManager.clientData[this.OwnerClientId];
                playerName.Value=new StringContainer();
                playerName.Value.SomeText = playerData.PlayerName;
            }

            if (IsClient)
            {
                gameObject.name =  playerName.Value.SomeText;
            }
         
            if (IsLocalPlayer)
            {
                gameObject.name = "{Local Player}" + playerName.Value.SomeText;
            }
            transform.SetSiblingIndex(0);
        }
    }
}