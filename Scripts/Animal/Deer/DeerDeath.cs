using System;
using System.Collections;
using BehaviorDesigner.Runtime;
using DG.Tweening;
using SnowIsland.Scripts.Chest;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace SnowIsland.Scripts.Animal.Deer
{
    [RequireComponent(typeof(BehaviorTree))]
    [RequireComponent(typeof(DeerAnimation))]
    [RequireComponent(typeof(Deer))]
    [RequireComponent(typeof(AudioSource))]
    public class DeerDeath :NetworkBehaviour
    {
        private Deer _deer;
        private DeerAnimation _deerAnimation;
        private BehaviorTree _behaviorTree;
        private BoxCollider _boxCollider;
        private AudioSource _audioSource;
        private Sequence deathAnim;
        public GameObject deerCorpsePrefab;
        public AudioClip deerDeathAudio;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _deer = GetComponent<Deer>();
            _deerAnimation = GetComponent<DeerAnimation>();
            _behaviorTree = GetComponent<BehaviorTree>();
            _deer.OnDeerDeadOnServer += OnDeerDead;
            _audioSource = GetComponent<AudioSource>();
            deathAnim=DOTween.Sequence();
            deathAnim.Pause();
            deathAnim.Insert(0, transform.DOMoveY(transform.position.y-0.5f,3));
            deathAnim.InsertCallback(3, () =>
            { 
                deathAnim.Kill();
                GetComponent<NetworkObject>().Despawn(true);
            });
            deathAnim.InsertCallback(1.5f, () =>
            {
                //5. 生成尸体
                if (deerCorpsePrefab != null)
                {
                    GameObject deerCorpseObj = Instantiate(deerCorpsePrefab, transform.position, Quaternion.identity);
                    deerCorpseObj.GetComponent<NetworkObject>().Spawn();
                    deerCorpseObj.GetComponent<ChestStorage>().name.Value.SomeText = "鹿";
                }
            });
        }
        
        private void OnDeerDead()
        {
            //1.关闭AI
            _behaviorTree.enabled = false;
            //2.死亡动画
            _deerAnimation.MarkDead(); 
            //3. 销毁碰撞体
            Destroy(_boxCollider); 
            Debug.Log(name+" 已死亡,将在3秒后播放死亡动画");
            //4. 开始下降
            StartCoroutine(StartDeathAnim());
            DeerDeadClientRpc();
        }

        [ClientRpc]
        private void DeerDeadClientRpc()
        {
            _audioSource.PlayOneShot(deerDeathAudio);
        }

        IEnumerator StartDeathAnim()
        {
            yield return new WaitForSeconds(3);
            deathAnim.Play();
        }
    }
}