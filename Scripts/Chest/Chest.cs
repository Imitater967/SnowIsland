using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Interact;
using SnowIsland.Scripts.UI.Game;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Chest
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Interactable))]
    public class Chest: NetworkBehaviour
    {
        [SerializeField]
        private AudioClip openClip;
        [SerializeField]
        private AudioClip closeClip;
        private AudioSource _audioSource;
        private Animator _animator;
        private Interactable _interactable;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _interactable = GetComponent<Interactable>();
            _animator = GetComponent<Animator>();
            _interactable.OnStopInteract += ServerClose ;
        }

        private void ServerClose()
        { 
            _animator.SetBool("Open",false);
            CloseAudioClientRpc();
        }
        public void ServerOpen(NetworkBehaviourReference networkBehaviourReference)
        {
            _animator.SetBool("Open",true);    
            PlayOpenAudioClientRpc();
            OpenUIClientRpc(new ClientRpcParams(){Send = new ClientRpcSendParams(){TargetClientIds = new []{((NetworkBehaviour)networkBehaviourReference).OwnerClientId}}});
        }
        [ClientRpc]
        private void OpenUIClientRpc(ClientRpcParams @params)
        {
            UIPanelManager.Instance.Open(UIType.Chest);
        }
        [ClientRpc]
        private void CloseAudioClientRpc()
        {
            _audioSource.PlayOneShot(closeClip);
        }
        [ClientRpc]
        private void PlayOpenAudioClientRpc()
        {
            _audioSource.PlayOneShot(openClip);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void OpenServerRpc(NetworkBehaviourReference playerInteractRef)
        {
            ServerOpen(playerInteractRef);
        }
   
    }
}