using System;
using UnityEngine;

namespace SnowIsland.Scripts
{
    [RequireComponent(typeof(ParticleSystem))]
    public class DestroyAfterEffect : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>(); 
        }

        private void Update()
        {
            if (_particleSystem.isStopped)
            {
                Destroy(gameObject);
            }
        }
    }
}