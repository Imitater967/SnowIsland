using System;
using SnowIsland.Scripts.Game;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    public enum Camp
    {
        Crew,Imposter,ThirdParty
    }
    [DisallowMultipleComponent]
    public class PlayerIdentity : NetworkBehaviour
    {
        public NetworkVariable<Camp> camp = new NetworkVariable<Camp>();
        public Action<IdentityAsset> OnIdentityChangeClient;
        [field: SerializeField]
        public IdentityAsset identityAsset { get; private set; } 
        public float skillUseTime = 0;
        public Identity Identity => identityAsset.Identity;
        public bool CanUseSkill()
        {
            return Time.time >= identityAsset.SkillCooldown + skillUseTime;
        }

        public void ChangeIdentityOnServer(Identity newId)
        {
            if (!IsServer)
            {
                Debug.Log(name+" Trying to switch identity on client which is not allowed");
                return;
            }
            ChangeIdentityClientRpc(newId);
        }
        //广播所有客户端,这个人的身份改变了
        [ClientRpc]
        private void ChangeIdentityClientRpc(Identity identity)
        {
            var idAsset=AbstractIdentityManager.Instance.GetIdentity(identity);
            if (idAsset == null)
            {
                Debug.LogError(name+" Trying switch identity to "+identity+" but it was not found in the reg-table");
            }

            identityAsset = idAsset;
            OnIdentityChangeClient?.Invoke(idAsset);
            Debug.Log("Telling others that "+name+" is now "+identity);
            EnableIndicatorsIfDetector(identity);
        }
        public bool MatchIdentity(Identity identity)
        {
            return identity == Identity;
        }

        public void EnableIndicatorsIfDetector(Identity identity)
        {
            if ((identity != Identity.Imposter_Detector))
            {
                return;   
            }
            if(!IsLocalPlayer)
                return;
            foreach (var keyValuePair in RoomManagerAlpha.clientData)
            {
                if(keyValuePair.Key.Equals(this.OwnerClientId))
                    continue;
                var id=NetworkManager.Singleton.ConnectedClients[keyValuePair.Key].PlayerObject.GetComponent<PlayerIndicator>();
                id.enabled = true;
            }
        }
    }
}