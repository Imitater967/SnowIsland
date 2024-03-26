using System;
using System.Collections.Generic;
using System.Linq;
using SnowIsland.Scripts.Character;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace SnowIsland.Scripts.Room
{
    public class IdentityManagerAlpha: AbstractIdentityManager
    {
        [ReadOnly]
        public List<IdentityAsset> availiableCrewIdentity;
        [ReadOnly]
        public List<IdentityAsset> availiableImposterIdentity;
        protected override void Awake()
        {
            base.Awake();
            foreach (var identityPair in IdentityReg)
            {
                if (identityPair.IdentityAsset.DefaultImposter)
                {
                    
                    availiableImposterIdentity.Add(identityPair.IdentityAsset);
                }
                else
                {
                    availiableCrewIdentity.Add(identityPair.IdentityAsset);
                }
            }
        }

        protected override void AssignIdentity()
        { 
            
            Debug.Log("----Start Assign Identity By Alpha Identity Manager---");
            /*
          * 1. 验证数据,狼人数最多为玩家数-1
          */
            int playerAmount = AbstractRoomManager.clientData.Count;
            int imposterAmount = Mathf.Min(playerAmount - 1, AbstractRoomManager.Instance.RoomProperty.wereWolves);
            Debug.Log("|There will be "+playerAmount+" of player and "+imposterAmount+"of imposter");
            //2. 抽取imposterAmount个玩家为狼,剩下的为普通人
            var randoms=AbstractRoomManager.clientData.Keys.OrderBy(u => new Guid());
            List<ulong> imposter = new List<ulong>();
            List<ulong> crew = new List<ulong>();
            foreach (var random in randoms)
            {
                if (imposter.Count >= imposterAmount)
                {
                    Debug.Log("|-Mark "+random+" As Crew");
                    crew.Add(random);
                }
                else
                {
                    imposter.Add(random);
                    Debug.Log("|-Mark "+random+" As Imposter");
                }
            }
            Debug.Log("|Start Assign Imposter Identity");
            //3. 赋值狼人 (后期根据模式会有不同的分配方法,就重写这个函数再注册就可以了
            foreach (var keyValuePair in imposter)
            {
                var playerObj= NetworkManager.ConnectedClients[keyValuePair].PlayerObject;
                var playerIdentity = playerObj.GetComponent<PlayerIdentity>();
                playerIdentity.camp.Value=Camp.Imposter;
                //random.Range是左闭右开区间
                var randomIdentity = availiableImposterIdentity[Random.Range(0, availiableImposterIdentity.Count)];
                playerIdentity.ChangeIdentityOnServer(randomIdentity.Identity);
                Debug.Log("|-Assigned "+keyValuePair+" as "+randomIdentity.Identity);
            }
            
            Debug.Log("|Start Assign Crew Identity");
            foreach (var keyValuePair in crew)
            {
                var playerObj= NetworkManager.ConnectedClients[keyValuePair].PlayerObject;
                var playerIdentity = playerObj.GetComponent<PlayerIdentity>();
                playerIdentity.camp.Value=Camp.Crew;
                //random.Range是左闭右开区间
                var randomIdentity = availiableCrewIdentity[Random.Range(0, availiableCrewIdentity.Count)];
                playerIdentity.ChangeIdentityOnServer(randomIdentity.Identity); 
                Debug.Log("|-Assigned "+keyValuePair+" as "+randomIdentity.Identity);
            }
            
            Debug.Log("---- Identity Assignment Complete ----");
        }
    }
}