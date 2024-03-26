using SnowIsland.Scripts.Combat;
using UnityEngine;

namespace SnowIsland.Scripts.Decoration
{
    public class Decoration : MonoBehaviour,IDamageReceiver
    {
        private IDamageTrigger _damageTrigger=null;
        public bool IsAlive()
        {
            return true;
        }

        public IDamageTrigger GetKiller()
        {
            return _damageTrigger;
        }

        public void ReceiveDamage(DamageInfo damage)
        { 
        }
    }
}