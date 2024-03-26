using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Room
{

    [Serializable]
    public struct RoomProperty
    {
        public byte wereWolves;
        public string mapName;
    }
    public abstract class AbstractRoomManager: NetworkBehaviour
    {
        
        [SerializeField]
        protected TMP_InputField playerNameField;
        public static Dictionary<ulong, PlayerData> clientData { get; protected set; }=new Dictionary<ulong, PlayerData>(); 
        public static AbstractRoomManager Instance;
        protected RoomProperty roomProperty =new RoomProperty(){wereWolves = 1,mapName = "Game"};
        public RoomProperty RoomProperty => roomProperty;
        public  event Action OnGameScenePreparedServer;
        public  event Action OnGameScenePreparedClient;
        public Action OnGameStartServer;
        public Action OnGameStartClient;

        protected virtual void Awake()
        {
            if(Instance!=null)
                Debug.LogError("More Than 2 Room Manager");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static PlayerData? GetPlayerData(ulong clientId)
        {
            if (clientData.TryGetValue(clientId, out PlayerData result))
            {
                return result;
            }

            return null;
        } 
        protected void FireGameStartEvent()
        {
            OnGameScenePreparedServer?.Invoke(); 
            SendEventClientRpc();
            Debug.Log("OnGameScene Loaded");
        }

        [ClientRpc]
        private void SendEventClientRpc()
        {
            OnGameScenePreparedClient?.Invoke();
        }
    }
}