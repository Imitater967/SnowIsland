using System;
using SnowIsland.Scripts.Game;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    public class PlayerCharacter: NetworkBehaviour
    {
        public AudioSource AudioSource { get; private set; }
        public PlayerCraft PlayerCraft { get; private set; }
        public PlayerStatus PlayerStatus { get; private set; }
        public PlayerMotion PlayerMotion { get; private set; }
        public PlayerIdentity PlayerIdentity { get; private set; }
        public PlayerControl PlayerControl { get; private set; }
        public PlayerHotbar PlayerHotbar { get; private set; } 
        public PlayerProfile PlayerProfile { get; private set; }
        public PlayerAnimation PlayerAnimation { get; private set; }
        public PlayerGunUsage PlayerGunUsage { get; private set; }
        public CharacterController cc { get; private set; }
        public PlayerInteract PlayerInteract { get; set; }
        public PlayerItemUsage PlayerItemUsage { get; set; }
        public PlayerPlacement PlayerPlacement { get; set; }
        public bool dead => PlayerStatus.dead.Value;
        public bool crawling => PlayerMotion.isCrawl.Value;
        private void Awake()
        {
            cc = GetComponent<CharacterController>();
            PlayerControl = GetComponent<PlayerControl>();
            PlayerMotion = GetComponent<PlayerMotion>();
            PlayerIdentity = GetComponent<PlayerIdentity>();
            PlayerStatus = GetComponent<PlayerStatus>(); 
            PlayerProfile = GetComponent<PlayerProfile>();
            PlayerHotbar = GetComponent<PlayerHotbar>();
            PlayerInteract = GetComponent<PlayerInteract>();
            PlayerAnimation = GetComponent<PlayerAnimation>();
            PlayerItemUsage = GetComponent<PlayerItemUsage>();
            PlayerCraft = GetComponent<PlayerCraft>();
            PlayerPlacement = GetComponent<PlayerPlacement>();
            PlayerGunUsage = GetComponent<PlayerGunUsage>();
            AudioSource = GetComponent<AudioSource>();
            AbstractRoomManager.Instance.OnGameStartClient += () =>
            {
                enabled = true;
                if (IsLocalPlayer)
                {
                    if (gameObject.TryGetComponent(out AudioListener listener))
                    {
                        Debug.Log("Already audio listener on player skipping");
                    }
                    else
                    {
                        gameObject.AddComponent<AudioListener>(); ;
                        Debug.Log("added audio player for local player");
                    }
                }

            };
            enabled = false;
        }

        public bool IsRunOnServer => IsServer;
        public bool IsRunOnClient => IsClient;
        public void Teleport(Vector3 pos)
        {
            DisableMovement();
            transform.position = pos;
            EnableMovement();
        }
        public void DisableMovement()
        {
            cc.enabled = false;
            PlayerMotion.enabled = false;
        } 
        
        public void EnableMovement()
        { 
            cc.enabled = true;
            PlayerMotion.enabled = true;
        }
     
        public static PlayerCharacter Local { get; private set; }

        public override void OnNetworkSpawn()
        {
            DontDestroyOnLoad(gameObject);
            if(IsServer){
                Teleport( new Vector3(0, -1000, 0));
                DisableMovement();
            }
            if (IsLocalPlayer)
                Local = this;
        }
 
    }
}