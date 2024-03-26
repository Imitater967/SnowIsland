using System;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Game
{
    public abstract class AbstractGameManager: NetworkBehaviour
    { 
        public static AbstractGameManager Instance;
        protected virtual void Awake()
        {
            if(Instance!=null)
                Debug.LogError("More Than 2 Game Manager");
            Instance = this;
        }

        public virtual void Start()
        {
            if (IsServer)
            {
                Debug.Log(name+ "Game Prepared On Server"); 
                AbstractRoomManager.Instance.OnGameStartServer?.Invoke();
            }

            if (IsClient)
            {
                Debug.Log(name+ "Game Prepared On Client");
                AbstractRoomManager.Instance.OnGameStartClient?.Invoke();
            }
        }
    }
}