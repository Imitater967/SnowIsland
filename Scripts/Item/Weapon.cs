using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Combat;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Item
{
    [RequireComponent(typeof(BoxCollider))]
    public class Weapon: MonoBehaviour,IDamageTrigger
    {
        [SerializeField]
        private float damage;
        private BoxCollider _collider;
        public float charge;
        private PlayerCharacter _playerCharacter;
        private List<GameObject> attackCache;
        private WeaponEffects _weaponEffects;
        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _playerCharacter = GetComponentInParent<PlayerCharacter>();
            _weaponEffects = GetComponent<WeaponEffects>();
            
            _collider.enabled = false;
            _weaponEffects.enabled = false;
            enabled = false;
        }
 
        private bool CheckCache(GameObject gameObject)
        {
            if (attackCache.Contains(gameObject))
                return false;
            attackCache.Add(gameObject);
            return true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_playerCharacter.IsRunOnServer)

                return;
            var resultObject = collision.gameObject;
            if (_playerCharacter.gameObject == resultObject.gameObject)
                return;
            if (resultObject.gameObject.TryGetComponent(out IDamageReceiver damageReceiver) &&
                CheckCache(resultObject.gameObject))
            {
                var damageInfo = new DamageInfo()
                {
                    DamageType = DamageType.Melee,
                    DamageTrigger = this,
                    Damage = damage * Mathf.Clamp01(charge),
                    //其实可以选择所有的normal之和的,不过在近战这里不需要
                    DamageNormal = collision.contacts[0].normal,
                    DamagePos = collision.contacts[0].point,
                };
                Debug.Log(_playerCharacter.name + " damaged " + resultObject.gameObject.name + " by " +
                          _playerCharacter.name + " with " + damageInfo.Damage + " damage");
                damageReceiver.ReceiveDamage(damageInfo);
            }
        }
 

        // private void Update()
        // {
        //     if(!_playerCharacter.IsRunOnServer)
        //         return;
        //     
        //     Collider[] results = new Collider[10];
        //     var size = Physics.OverlapBoxNonAlloc(_collider.center+transform.position, _collider.size / 2, results, transform.rotation);
        //     for (int i = 0; i < size; i++)
        //     {
        //         //ignore self
        //         var resultObject = results[i];
        //         if(_playerCharacter.gameObject==resultObject.gameObject)
        //             continue;
        //         if (resultObject.gameObject.TryGetComponent(out IDamageReceiver damageReceiver)&&
        //             CheckCache(resultObject.gameObject))
        //         {
        //             var damageInfo=new DamageInfo(){
        //                 DamageType = DamageType.Melee,
        //                 DamageTrigger = this,
        //                 Damage = damage*Mathf.Clamp01(charge),
        //                 //其实可以选择所有的normal之和的,不过在近战这里不需要
        //                 damageNormal = ((_collider.center+transform.position)-resultObject.transform.position).normalized
        //             };
        //             Debug.Log(_playerCharacter.name+" damaged "+resultObject.gameObject.name+" by "+_playerCharacter.name+" with "+damageInfo.Damage+" damage");
        //             damageReceiver.ReceiveDamage(damageInfo);
        //         }
        //     }
        // }
        //
 

        private void OnEnable()
        {
            _collider.enabled = true;
            _weaponEffects.enabled = true;
            attackCache = new List<GameObject>();
        }

        private void OnDisable()
        {
            _collider.enabled = false;
            _weaponEffects.enabled = false;
            // foreach (var o in attackCache)
            // {
            //     if(o!=null)
            //         o.GetComponent<IDamageReceiver>().OnDamageEnd(this);
            // }
        }

        public string GetName()
        {
            return _playerCharacter.PlayerProfile.playerName.Value.SomeText;
        }

        public void EndDamageDetection()
        {
            enabled = false;
        }

        public void StartDamageDetection()
        {
            enabled = true;
        }
    }
}