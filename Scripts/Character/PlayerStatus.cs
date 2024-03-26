using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Combat;
using SnowIsland.Scripts.Interact;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    public class PlayerStatus: NetworkBehaviour,IDamageReceiver,IDamageTrigger{
    
        [field:SerializeField] 
        //玩家是否死亡
        public NetworkVariable<bool> dead { get; private set; } = new NetworkVariable<bool>(){Value = false};
        //玩家濒死
        public NetworkVariable<bool> dying { get; private set; } = new NetworkVariable<bool>(){Value = false};
        [field: SerializeField]
        public NetworkVariable<float> liftUpRemian { get; private set; } = new NetworkVariable<float>();
        [field: SerializeField]
        public NetworkVariable<float> extraHealthMax { get; private set; }=new NetworkVariable<float>();
        [field: SerializeField]
        public NetworkVariable<float> tempHealthMax { get; private set; }=new NetworkVariable<float>();
        [field: SerializeField]
        public NetworkVariable<float> extraFoodMax { get; private set; }=new NetworkVariable<float>();
        [field: SerializeField]
        public NetworkVariable<float> extraWarmthMax { get; private set; }=new NetworkVariable<float>();
        [field: SerializeField]
        public NetworkVariable<float> healthCurrent { get; private set; }=new NetworkVariable<float>();
        [field: SerializeField]
        public NetworkVariable<float> foodCurrent { get; private set; }=new NetworkVariable<float>();
        [field: SerializeField]
        public NetworkVariable<float> warmthCurrent { get; private set; }=new NetworkVariable<float>();
        public NetworkVariable<bool> safeZone { get; private set; }=new NetworkVariable<bool>();
        public readonly float baseHealthMax = 1000;
        public readonly float baseFoodMax = 500;
        public readonly float baseWarmthMax = 500;
        public readonly float liftUpMax = 60;
        public float warmLossPerSecond = 2.5f;        
        public float foodLossPerSecond = 1.5f;
        public float healthLossPerSecond = 10f;
        public float maxHealthLossPerSecond = 10f;
        public float maxHealthMaxLoss = 700;
        public float HealthMax => baseHealthMax + extraHealthMax.Value+tempHealthMax.Value;
        //进度条用的最大生命值,是不需要减去最大生命值衰减的
        public float HealthMaxPB => baseHealthMax + extraHealthMax.Value;
        public float FoodMax => baseFoodMax + extraFoodMax.Value;
        public float WarmthMax => baseWarmthMax + extraWarmthMax.Value;

        public IDamageTrigger killer;
        public Interactable recoverInteraction;
        public Collider recoverInteractionCollider;
        public event Action OnPlayerDeathServer;
        public event Action OnPlayerRecoverServer;
        public event Action OnPlayerDyingServer;
        public event Action OnDamageReceiveServer;
        private PlayerCharacter PlayerCharacter;
        private void Awake()
        {
            enabled = false;
            InitValue();
            PlayerCharacter = GetComponent<PlayerCharacter>(); 
            recoverInteraction.enabled = false;
            recoverInteractionCollider.enabled = false;
            //这里需要注意,后期用碰撞检测,是否到达SafeZone,可能会和这个冲突,游戏开始的时候
            RoomManagerAlpha.Instance.OnGameScenePreparedServer += () =>
            {
                enabled = true;
            };
            OnPlayerRecoverServer += () =>
            {
                recoverInteraction.enabled = false;
                recoverInteractionCollider.enabled = false;
                PlayerCharacter.PlayerMotion.isCrawl.Value = false;
            };
            OnPlayerDyingServer += () => {
                recoverInteraction.enabled = true;
                recoverInteractionCollider.enabled = true;
                PlayerCharacter.PlayerMotion.isCrawl.Value = true;
                //
                // PlayerCharacter.PlayerInteract.StopInteractOnServer();
                // PlayerCharacter.PlayerInteract.StopView();
                PlayerCharacter.PlayerGunUsage.aiming.Value = false;
            };
            OnPlayerDeathServer += () =>
            {
                recoverInteraction.enabled = false;
                recoverInteractionCollider.enabled = false;
                PlayerCharacter.PlayerMotion.isCrawl.Value = false;
            };
       
            OnDamageReceiveServer += () =>
            {
                PlayerCharacter.PlayerInteract.StopInteractOnServer(); 
            };
        }

        public override void OnNetworkSpawn()
        {
            safeZone.Value = false;
        }

        private void Update()
        {
            //在服务器,而且不在安全区,而且没有处于濒死状态
            if (IsServer)
            {
                if(dead.Value)
                    return;
                if (dying.Value)
                {
                    var remainRecover = liftUpRemian.Value -= Time.deltaTime;
                    liftUpRemian.Value = Mathf.Max(remainRecover, 0);
                    if (liftUpRemian.Value <= 0)
                    {
                        dead.Value = true;
                        OnPlayerDeathServer?.Invoke();
                    }
                }
                else
                {
                    UpdateStatusLoss();
                    UpdateStatusDownToMax();
                    if (healthCurrent.Value <= 0)
                    {
                        OnPlayerDyingServer?.Invoke();
                        dying.Value = true;
                    }
                    var remainRecover = liftUpRemian.Value + Time.deltaTime;
                    liftUpRemian.Value = Mathf.Min(remainRecover, liftUpMax);
                    //UpdateFreezeTime();
                }
            }
        }

        // [SerializeField] private float freezeTime = 1;
        // [SerializeField] private float freezeEndTime;
        // private void UpdateFreezeTime()
        // {
        //         PlayerCharacter.PlayerMotion.enabled = Time.time > freezeEndTime;
        // }

        //如果生命值当前大于最大生命值,则选择最小的作为生命值
        private void UpdateStatusDownToMax()
        {
            foodCurrent.Value = Mathf.Min(foodCurrent.Value, FoodMax);
            warmthCurrent.Value = Mathf.Min(warmthCurrent.Value, WarmthMax);
            healthCurrent.Value = Mathf.Min(healthCurrent.Value, HealthMax);
        }
        //计算各数值随着时间流失
        private void UpdateStatusLoss()
        {
            //计算基本数值损失
           var foodCurrentValue = foodCurrent.Value- foodLossPerSecond* Time.deltaTime;
           var warmCurrentValue = warmthCurrent.Value - warmLossPerSecond * Time.deltaTime;
           foodCurrent.Value = Mathf.Max(0, foodCurrentValue);
           warmthCurrent.Value = Mathf.Max(0, warmCurrentValue);
           //计算生命值损失
           if (warmthCurrent.Value <= 0 || foodCurrent.Value <= 0)
           {
               var healthMaxValue = tempHealthMax.Value - maxHealthLossPerSecond * Time.deltaTime;
               tempHealthMax.Value = Mathf.Max(-maxHealthMaxLoss, healthMaxValue);
           }
            //双值都为0, 则开始掉血
           if (warmthCurrent.Value <= 0 && foodCurrent.Value <= 0)
           {
               var healthLost = healthCurrent.Value -= healthLossPerSecond * Time.deltaTime;
               healthCurrent.Value = Mathf.Max(0, healthLost);
           }
        }

        public void InitValue()
        {
            healthCurrent.Value = HealthMax;
            foodCurrent.Value = FoodMax;
            warmthCurrent.Value = WarmthMax;
        }
        //根据不同等级进行恢复
        public void Recover(int tier)
        {
        if(!IsServer||dying.Value!=false)
            return;
        if (tier >= 5)
            tier = 4;
        if (tier <= 0)
            tier = 0;
        healthCurrent.Value = HealthMax/(5-tier);
        foodCurrent.Value = FoodMax/(10-tier);
        warmthCurrent.Value = WarmthMax/(10-tier);
        dying.Value = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RecoverServerRpc()
        {
            Recover(0);
            OnPlayerRecoverServer?.Invoke();

        }
    
        #region Combat
 
        public bool IsAlive()
        {
            return dead.Value;
        }

        public IDamageTrigger GetKiller()
        {
            return killer;
        }

        public void ReceiveDamage(DamageInfo damage)
        {
            if(!IsServer)
                return;
            OnDamageReceiveServer?.Invoke();
            if (dying.Value)
            {
                liftUpRemian.Value -= damage.Damage / 10;
            }
            else
            {
                healthCurrent.Value -= damage.Damage;
            }

            killer = damage.DamageTrigger;
            if (damage.DamageType == DamageType.Melee)
            {
                // freezeEndTime = Time.time + freezeTime;
               // PlayerCharacter.PlayerAnimation.TriggerInjury(); 
            }
        } 

        

        public string GetName()
        {
            return PlayerCharacter.Local.PlayerProfile.playerName.Value.SomeText;
        }

        #endregion
      
    }
}