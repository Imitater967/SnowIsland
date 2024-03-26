using System;
using System.Collections.Generic;
using System.Numerics;
using SnowIsland.Scripts.Item;
using Unity.Netcode;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SnowIsland.Scripts.Character
{
    [RequireComponent(typeof(PlayerHotbar))]
    public class PlayerPlacement : NetworkBehaviour
    {
        [SerializeField]
        private Vector3 placementOffset = new Vector3(0, 1, 0.5f);
        [SerializeField] private GameObject previewInstance;
        [SerializeField] private BoxCollider previewCollider;
        [SerializeField] private MeshRenderer[] previewMeshes;
        [Header("Settings")] [SerializeField] private Material material;
        [SerializeField] private float verticalCheckDistance = 1;
        [SerializeField] private LayerMask previewLineCheckMask;
        [SerializeField] private LayerMask placementBoxCheckMask;
        [SerializeField] private Color allowColor=new Color(0,1,0,0.5f);
        [SerializeField] private Color denyColor=new Color(1,0,0,0.5f);
        public Vector3 targetPos;
        public Quaternion targetRot;
        public NetworkVariable<bool> canPlace =new NetworkVariable<bool>() ;
        [SerializeField,ReadOnly,Header("Debug")]
        private bool requireChangeMaterial = true;

        [SerializeField, ReadOnly] private Collider[] colliding;
        private PlayerCharacter _playerCharacter;
        private PlayerHotbar _playerHotbar;
        private static readonly int _color = Shader.PropertyToID("_Color");

        private void Awake()
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerHotbar = GetComponent<PlayerHotbar>();
            _playerHotbar.selection.OnValueChanged += (a, b) =>
            {
                if (_playerHotbar.itemInHand.asset is  PlacementAsset)
                {
                    if(_playerCharacter.IsLocalPlayer){
                        PlacementAsset placementAsset = (PlacementAsset)_playerHotbar.itemInHand.asset;
                        var placement=placementAsset.objectToSpawn;
                        SpawnPreview(placement);
                        requireChangeMaterial = true;
                        RefreshPreviewStatus(canPlace.Value);
                    }
                }
            };
            canPlace.OnValueChanged += (old, @new) =>
            {
                RefreshPreviewStatus(@new);
            };
        }
        private void RefreshPreviewStatus(bool canPlaceValue){if (IsClient)
        {
            if (requireChangeMaterial)
            {
                if (canPlaceValue)
                {
                    ChangePreviewMaterialToGreen();
                }
                else
                {
                    ChangePreviewMaterialToRed();
                }
            }
        }}

        private void ClearPreview()
        {
            Debug.Log("Placement: Clear Placement");
            Destroy(previewInstance);
            previewCollider = null;
            previewInstance = null;
        }

        private void SpawnPreview(GameObject placement)
        {
            ClearPreview(); 
            Debug.Log("Placement: Spawn Placement for "+placement.name);
            previewInstance=Instantiate(placement.gameObject);
            previewCollider = previewInstance.GetComponent<BoxCollider>();
            previewMeshes = previewInstance.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in previewMeshes)
            {
                meshRenderer.material = material;
            }
            foreach (var componentsInChild in previewInstance.GetComponentsInChildren<ParticleSystem>())
            {
                componentsInChild.Stop();
            }
            foreach (var componentsInChild in previewInstance.GetComponentsInChildren<Transform>())
            {
                componentsInChild.gameObject.layer = LayerMask.NameToLayer("ItemPreview");
            }
        }

        private void Update()
        {
            if(!IsLocalPlayer)
                return;
            if (_playerHotbar.itemInHand.asset is not PlacementAsset)
            {
                if (previewInstance != null)
                {
                    ClearPreview(); 
                }
                return;
            }

            if (IsClient)
            {
                PlacementAsset placementAsset = (PlacementAsset)_playerHotbar.itemInHand.asset;
                var placement=placementAsset.objectToSpawn;
                if(placement==null)
                    return;
            
                if (previewInstance == null)
                {
                    SpawnPreview(placement);
                }
                else
                {
                    UpdatePosPreview();
                }
                //canPlace.value的修改有网络延迟,改为监听canPlace
                // if (canPlace.Value && requireChangeMaterial)
                // {
                //     ChangePreviewMaterialToGreen();
                // }
                // if (!canPlace.Value && requireChangeMaterial)
                // {
                //     ChangePreviewMaterialToRed();
                // }
            }
         
           
        }

        private void ChangePreviewMaterialToRed()
        {
            requireChangeMaterial = false;
            foreach (var meshRenderer in previewMeshes)
            {
                meshRenderer.material.SetColor(_color,denyColor);
            }
        }

        private void ChangePreviewMaterialToGreen()
        {   
            requireChangeMaterial = false;
            foreach (var meshRenderer in previewMeshes)
            {
                meshRenderer.material.SetColor(_color,allowColor);
            }

        }

        private void OnDrawGizmos()
        {
            if (previewCollider != null)
            {
                Gizmos.color = Color.red;
                var matrix = new Matrix4x4();
                matrix.SetTRS(targetPos, targetRot, Vector3.one);
                Gizmos.matrix =matrix;
                Gizmos.DrawWireCube(Vector3.zero,previewCollider.size*0.9f );
            }
        }

        //手持可放置物品则进入物品预览状态
        //1. 从targetPos处向下射出1m射线
        private void UpdatePosPreview()
        {
            targetRot=Quaternion.identity;
            targetPos = transform.position + transform.TransformDirection(placementOffset);
            Vector3 endPos = new Vector3(targetPos.x, targetPos.y - verticalCheckDistance, targetPos.z);
            //需要注意的是,遇到超大的正方体, linecast将会失效(检测的线段在正方体体积内部,无法检测到碰撞)
            bool hitSomething=Physics.Linecast(targetPos, endPos,out RaycastHit hitInfo,previewLineCheckMask);
            if (hitSomething)
            {  
                //targetPos = hitInfo.point-previewCollider.center+new Vector3(0,previewCollider.size.y/2,0);
                targetPos = hitInfo.point + hitInfo.normal.normalized * previewCollider.size.y/2;
                targetRot = Quaternion.LookRotation(Vector3.forward,hitInfo.normal);
            } 
            Debug.DrawLine(targetPos,endPos,Color.blue);
            previewInstance.transform.position = targetPos;
            previewInstance.transform.rotation = targetRot; 
            Collider[] colliders = new Collider[5];
            //需要比原来的size小,故这额外*0.9
            //不知道为什么到了斜坡就失效
            Physics.OverlapBoxNonAlloc(targetPos, 0.9f*(previewCollider.size / 2F), colliders,targetRot, placementBoxCheckMask,QueryTriggerInteraction.Ignore);
            bool empty = true;
            colliding = colliders;
            foreach (var collider1 in colliders)
            {
                if (collider1 != null)
                    empty = false;
            }

            if (empty&&hitSomething)
            {
                if (!canPlace.Value)
                {
                    requireChangeMaterial = true;
                }
                MarkCanPlaceServerRpc(true);
            }
            else
            {
                if (canPlace.Value)
                {
                    requireChangeMaterial = true;
                }
                MarkCanPlaceServerRpc(false);
            }
        } 
        [ServerRpc]
        private void MarkCanPlaceServerRpc(bool can)
        {
            canPlace.Value = can;
        }

        [ClientRpc]
        public void PlaceClientRpc()
        { 
            PlaceServerRpc(targetPos,targetRot);
        }

        [ServerRpc]
        public void PlaceServerRpc(Vector3 pos, Quaternion rot)
        {
            var itemAsset = _playerHotbar.itemInHand.asset;
            var placementAsset=itemAsset as PlacementAsset;
            if (placementAsset != null)
            {
                var obj= Instantiate(placementAsset.objectToSpawn.gameObject,pos,rot);
                obj.GetComponent<NetworkObject>().Spawn();
            }
            else
            {
                Debug.Log(name+" trying to place "+itemAsset+" but it's not placmentAsset");
            }
        }
    }
}