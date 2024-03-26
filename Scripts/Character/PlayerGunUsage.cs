using System;
using RootMotion.FinalIK;
using SnowIsland.Scripts.Item;
using SnowIsland.Scripts.Room;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    
    public class PlayerGunUsage: NetworkBehaviour
    {
        public NetworkVariable<bool> aiming;
        public Vector3 aimPos;
        private PlayerInteract _playerInteract;
        private PlayerStatus _playerStatus;
        private PlayerHotbar _playerHotbar;
        private AimIK _aimIK;
        [SerializeField,ReadOnly]
        private GameObject aimIndicatorInstance;
        [SerializeField]
        private GameObject aimIndicatorPrefab;

        private void Awake()
        {
            _playerStatus = GetComponent<PlayerStatus>();
            _playerInteract = GetComponent<PlayerInteract>();
            _playerHotbar = GetComponent<PlayerHotbar>();
            _aimIK = GetComponent<AimIK>();
            aiming.OnValueChanged += (old, @new) =>
            {
                //修改为new
                _aimIK.enabled = @new;
                if (_aimIK.enabled)
                {
                    _aimIK.solver.transform = _playerHotbar.currentHandObject.transform;
                    if(IsServer){
                        _playerInteract.StopInteractOnServer(); 
                    }
                }  
            };
            AbstractRoomManager.Instance.OnGameStartServer += () =>
            {
                var o = Instantiate(aimIndicatorPrefab);
                o.GetComponent<NetworkObject>().Spawn();
                aimIndicatorInstance = o;
                o.name = "AimIndicatorServer";
                _aimIK.solver.target = o.transform;
                UpdateClientAimClientRpc(o);
            };
        }
        [ClientRpc]
        private void UpdateClientAimClientRpc(NetworkObjectReference objectReference)
        {
            GameObject o = objectReference;
            aimIndicatorInstance = o;
            o.name = "AimIndicatorClient";
            _aimIK.solver.target = o.transform; 
        }

        private void Update()
            {
                if(!IsServer){
                return;
                }
                if (aiming.Value)
                {
                    transform.rotation=quaternion.LookRotation(new float3(aimPos.x,0,aimPos.z),Vector3.up);
                }

                if (aimIndicatorInstance != null)
                {
                    aimIndicatorInstance.transform.position = transform.position+aimPos;
                }
            } 
            public bool CheckCanUse()
        {
            if (_playerInteract.serverInteractable != null)
                return false;
            if (_playerStatus.dying.Value || _playerStatus.dead.Value)
                return false;

            return true;
        }
        [ServerRpc]
        public void FireServerRpc()
        {
            if(!aiming.Value)
                return;
        }

        [ServerRpc]
        public void SendAimPosServerRpc(Vector3 pos)
        {
            aimPos = pos;
        }
        [ServerRpc]
        public void ToggleAimServerRpc(bool aim)
        {
            if (aim)
            {
                if (_playerHotbar.itemInHand.asset is GunAsset&&CheckCanUse())
                {
                    aiming.Value = true;
                    return;
                }
            }

            aiming.Value = false;
        }

    
    }
}