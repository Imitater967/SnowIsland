using System;
using UnityEngine;

namespace SnowIsland.Scripts.Item
{
    [RequireComponent(typeof(AudioSource))]
    public class WeaponEffects : MonoBehaviour
    {
        public TrailRenderer TrailRenderer; 
        private AudioSource _source;
        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            TrailRenderer.enabled = true;
            _source.Play();
        }

        private void OnDisable()
        {
            TrailRenderer.enabled = false;
        }
    }
}