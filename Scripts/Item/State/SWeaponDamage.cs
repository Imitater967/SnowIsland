using SnowIsland.Scripts.Character;
using UnityEngine;

namespace SnowIsland.Scripts.Item.State
{
    public class SWeaponDamage : StateMachineBehaviour
    {
        //使用broadcastmessage太浪费性能了
        //获取handItem
        private Weapon _weapon;
        private PlayerCharacter _player;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            var playerCharacter = animator.GetComponent<PlayerCharacter>();
            _player = playerCharacter; 
            var handObject = playerCharacter.PlayerHotbar.currentHandObject;
            if(handObject==null)
            {return;}

            _weapon = handObject.GetComponent<Weapon>();
            if(_weapon==null)
                return; 
                _weapon.StartDamageDetection();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {  
                _weapon.EndDamageDetection();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }
    }
}