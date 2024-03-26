using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SnowIsland.Scripts.Character;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnowIsland.Scripts.Room
{
    [Serializable]
    public struct IdentityPair
    {
        public Identity Identity;
        public IdentityAsset IdentityAsset;
    }
    public abstract class AbstractIdentityManager: NetworkBehaviour
    {
        public static AbstractIdentityManager Instance;
        [field: SerializeField]
        public List<IdentityPair> IdentityReg { get; private set; }
        
        protected virtual void Awake()
        {
            if(Instance!=null)
                Debug.LogError("More Than 2 Identity Manager");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Start()
        {
            AbstractRoomManager.Instance.OnGameScenePreparedServer += AssignIdentity;
        }

        protected virtual void AssignIdentity()
        {
         
        }

  
        public IdentityAsset GetIdentity(Identity identity)
        {
            foreach (var identityPair in IdentityReg)
            {
                if (identityPair.Identity == identity)
                    return identityPair.IdentityAsset;
            }

            return null;
        }

    }
}