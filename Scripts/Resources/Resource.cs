using System;
using SnowIsland.Scripts.Combat;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Resources
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(DropRandomItemOnDeath))]
    public class Resource : NetworkBehaviour,IDamageReceiver
    {
        public float health;
        public AudioClip treeFallClip;
        public GameObject hitEffect;
        private AudioSource _audioSource;
        private DropRandomItemOnDeath _dropRandomItemOnDeath;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _dropRandomItemOnDeath = GetComponent<DropRandomItemOnDeath>();
        }

        public bool IsAlive()
        {
            return health > 0;
        }

        public IDamageTrigger GetKiller()
        {
            return null;
        }

        public void ReceiveDamage(DamageInfo damage)
        {
            if (IsServer)
            {
                health -= damage.Damage;
                PlayHitEffectClientRpc(damage.DamagePos,damage.DamageNormal);
                if (health <= 0)
                {
                    PlayDeathEffectClientRpc();
                    _dropRandomItemOnDeath.OnDeath();
                    GetComponent<NetworkObject>().Despawn(true);
                }
 
            }
        }

        public void OnDamageEnd(IDamageTrigger damageTrigger)
        {
             
        }

        [ClientRpc]
        public void PlayDeathEffectClientRpc()
        {
            AudioSource.PlayClipAtPoint(treeFallClip,transform.position,1f);
        }
        //树木破坏特效和音效
        [ClientRpc]
        public void PlayHitEffectClientRpc(Vector3 pos,Vector3 normal)
        {
            Instantiate(hitEffect ,pos,Quaternion.LookRotation(normal));
            _audioSource.Play();
        }
    }
}