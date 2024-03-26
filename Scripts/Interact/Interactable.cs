using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.UI.Game.Interact;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Interact
{
    /*
     * ~~交互区域,当玩家走进交互区域的时候,就会成为预览者~~这样设计更适合手机端,不适合电脑端
     * 1. 客户端走入区域,根据interactZone是否有interactor值展示Gui
     * 
     * 
     */
    //正确的做法应该是,在interact的时候授予权限, 停止interact的时候终止权限,但是没有这个必要,毕竟Demo而已,
    public class Interactable : NetworkBehaviour
    {
        //预览者,只存在于服务器
        [SerializeField,ReadOnly]
        private List<PlayerInteract> viewers;
        //交互者,只存在于服务器 
        public Action OnStopInteract;
        [SerializeField]
        private List<PlayerInteract> interactors; 
        public string name;
        public Vector3 uiOffset;
        public bool @private = true;
        public GameObject claimedUIPrefab;
        public GameObject interactUIPrefab;
        public GameObject uiInstanceOnClient;
        public void StartView(PlayerInteract playerInteract)
        {
            //开始预览,如果当前交互者为空,就发送交互面板,反之,发送已有人的面板
            if (interactors.Count>=1&&@private)
            {
                ShowClaimedUIClientRpc(new ClientRpcParams()
                {
                    Send = new ClientRpcSendParams(){TargetClientIds = new []{playerInteract.OwnerClientId}}
                });  
                return;
            }  
            ShowInteractUIClientRpc(new ClientRpcParams()
            {
                Send = new ClientRpcSendParams(){TargetClientIds = new []{playerInteract.OwnerClientId}}
            });  
            viewers.Add(playerInteract);
                
            Debug.Log("交互:"+name+"开始"+playerInteract.name+"的预览");
        }

        public void StopInteract(PlayerInteract playerInteract)
        { 
            Debug.Log( playerInteract.name+" Is trying to stop interacting with  "+name); 
            interactors.Remove(playerInteract);
            OnStopInteract?.Invoke();
        }
        public void EndView(PlayerInteract playerInteract)
        {  
            Debug.Log("交互:"+name+"结束被"+playerInteract.name+"的预览");
            //结束预览
            viewers.Remove(playerInteract);
                HideUIClientRpc(new ClientRpcParams()
                {
                    Send = new ClientRpcSendParams(){TargetClientIds = new []{playerInteract.OwnerClientId}}
                });  
        }

        
        //单独发给客户端
        [ClientRpc]
        private void ShowInteractUIClientRpc(ClientRpcParams @params)
        {
            if (uiInstanceOnClient != null)
            {
                Destroy(uiInstanceOnClient);
            }
            uiInstanceOnClient = Instantiate(interactUIPrefab, GameObject.Find("Canvas/InteractUI").transform).gameObject;
            var interactPreview = uiInstanceOnClient.GetComponent<UIInteractPreview>();
            PlayerCharacter.Local.PlayerInteract .clientUIView = interactPreview;
            interactPreview.interactableClient = this;
            uiInstanceOnClient.SetActive(true);
        } 
        [ClientRpc]
        private void ShowClaimedUIClientRpc(ClientRpcParams @params)
        {
            
            if (uiInstanceOnClient != null)
            {
                Destroy(uiInstanceOnClient);
            }
            uiInstanceOnClient = Instantiate(claimedUIPrefab, GameObject.Find("Canvas/InteractUI").transform).gameObject;
            uiInstanceOnClient.GetComponent<UIInteractPreview>().interactableClient = this;
            uiInstanceOnClient.SetActive(true);
        }
        [ClientRpc]
        private void HideUIClientRpc(ClientRpcParams @params)
        {
            Destroy(uiInstanceOnClient);
        }

        public override void OnDestroy()
        {
            Destroy(uiInstanceOnClient.gameObject);
        }

        public void AddInteractor(PlayerInteract interactor)
        {
            //在有新的玩家进行交互的时候,对已存在的观察者进行更新
            //比如说3个玩家在观察,交互体只可以被一个玩家交互,此时有一个玩家来交互了,就直接刷新其他三个人的面板
            if (@private&&interactors.Count>0)
            {
                foreach (var playerInteract in viewers)
                {
                    ShowClaimedUIClientRpc(new ClientRpcParams()
                    {
                        Send = new ClientRpcSendParams(){TargetClientIds = new []{playerInteract.OwnerClientId}}
                    });  
                }   
            }
            else
            {
                foreach (var playerInteract in viewers)
                {
                    ShowInteractUIClientRpc(new ClientRpcParams()
                    {
                        Send = new ClientRpcSendParams(){TargetClientIds = new []{playerInteract.OwnerClientId}}
                    });  
                } 
            }
            interactors.Add(interactor);
        }     
    
    }
}