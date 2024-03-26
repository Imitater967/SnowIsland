using UnityEngine;

namespace SnowIsland.Scripts.Combat
{
    public interface IDamageReceiver
    {
        public bool IsAlive();
        public IDamageTrigger GetKiller();

        public abstract void ReceiveDamage(DamageInfo damage); 
    }

    public enum DamageType
    {
        Melee,Magic,Explosion
    }
    public struct DamageInfo
    {
        public DamageType DamageType;
        public IDamageTrigger DamageTrigger;
        public float Damage;
        public Vector3 DamageNormal; 
        public Vector3 DamagePos;
    }
    public interface IDamageTrigger 
    {
        public string GetName();
    }

    public interface IAimTarget
    {
        public void OnFocus();
        public void OnLoseFocus();
        public Vector3 GetAimOffset();
    }
}