using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Game;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    public class PlayerSpawner: MonoBehaviour
    {
        [field: SerializeField] public List<Transform> spawnPoints { get; private set; } = new List<Transform>();
        //Execute Before GameManager
        protected void Start()
        {
            
            Debug.Log(name+": Registered Event For Player Spawning");
            AbstractRoomManager.Instance.OnGameStartServer += SpawnPlayer;
        }

        void SpawnPlayer()
        {
            Debug.Log(name+": Prepare for player Spawning");
            int index = 0;
            foreach (var keyValuePair in AbstractRoomManager.clientData)
            {
               var playerObj= NetworkManager.Singleton.ConnectedClients[keyValuePair.Key].PlayerObject;
               playerObj.transform.position = spawnPoints[index++].position;
               playerObj.GetComponent<PlayerCharacter>().EnableMovement();
               Debug.Log(name+": Spawning Object For Player "+playerObj.name +" Id "+keyValuePair.Key);
               if (index >= spawnPoints.Count)
               {
                   index = 0;
               }
            }
        }
    }
}