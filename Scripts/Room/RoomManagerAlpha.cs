using System;
using System.Collections.Generic;
using System.Text;
using SnowIsland.Scripts.Network;
using SnowIsland.Scripts.Room;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SnowIsland.Scripts.Room 
{
    public class RoomManagerAlpha : AbstractRoomManager
    {
        public TMP_Text playerListTextField;
 

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += RefreshPlayerList;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += RefreshPlayerList; 
        }

        private void RefreshPlayerList (ulong id)
        {  
            List<StringContainer> players = new List<StringContainer>();
            foreach (var clientDataValue in clientData.Values)
            {   var container = new StringContainer();
                container.SomeText = clientDataValue.PlayerName;
                players.Add(container);
            }
            RefreshPlayerListClientRpc(players.ToArray());
        }

        [ClientRpc]
        private void RefreshPlayerListClientRpc(StringContainer[] playerNames)
        { 
            StringBuilder text =new StringBuilder();
            for (var i = 0; i < playerNames.Length; i++)
            {
                text.Append(playerNames[i].SomeText + "\n");
            }

            playerListTextField.text = text.ToString(); 
        }

        private void OnApproval(NetworkManager.ConnectionApprovalRequest arg1, NetworkManager.ConnectionApprovalResponse arg2)
        {
            var payloadBytes = arg1.Payload;
            var payloadJson = Encoding.ASCII.GetString(payloadBytes);
            var payload = JsonUtility.FromJson<ConnectionPayload>(payloadJson); 
            clientData[arg1.ClientNetworkId] = new PlayerData(payload.playerName);
            arg2.Approved = true;
            arg2.CreatePlayerObject = true;
        }

        private void OnDisconnect(ulong obj)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                clientData.Remove(obj);
            }
        }

        //Bnt
        public void StartHost()
        { 
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerName = playerNameField.text
            });
            byte[] payloadBytes = Encoding.ASCII.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            
            NetworkManager.Singleton.ConnectionApprovalCallback += OnApproval;  
            NetworkManager.Singleton.StartHost(); 
        }

        //Bnt
        public void StartClient()
        {
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerName = playerNameField.text
            });
            byte[] payloadBytes = Encoding.ASCII.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            NetworkManager.Singleton.StartClient();
        }
        //Bnt
        public void StartGame()
        {
            LoadScene();
        }
        private void LoadScene()
        {
            NetworkManager.Singleton.SceneManager.LoadScene(roomProperty.mapName, LoadSceneMode.Single);
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoad;
        }

        private void OnSceneLoad(ulong clientid, string scenename, LoadSceneMode loadscenemode)
        {
            switch (scenename)
            {
                case "Game":
                    FireGameStartEvent();
                    break;
            }
        }


    }
}