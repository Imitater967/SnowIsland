using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PlayerStatus))]
    public class PlayerAudio : NetworkBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip[] hurtSounds;

        private PlayerStatus _playerStatus;
        private void Awake()
        {
            _playerStatus = GetComponent<PlayerStatus>();
            _audioSource = GetComponent<AudioSource>();
            _playerStatus.OnDamageReceiveServer += () =>
            {
                PlayHurtAudioClientRpc();
            };
        }
        
        [ClientRpc]
        void PlayHurtAudioClientRpc()
        { 
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, hurtSounds.Length);
            _audioSource.clip = hurtSounds[n];
            _audioSource.PlayOneShot(_audioSource.clip);

            // move picked sound to index 0 so it's not picked next time
            hurtSounds[n] = hurtSounds[0];
            hurtSounds[0] = _audioSource.clip;
        }
    }
}