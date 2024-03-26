using SnowIsland.Scripts.Character;
using UnityEngine;

namespace SnowIsland.Scripts.Item.State
{
    public class SWeaponCharge : StateMachineBehaviour
    {
        private Weapon _weapon;
        private PlayerCharacter PlayerCharacter;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            var playerCharacter = animator.GetComponent<PlayerCharacter>();
            PlayerCharacter = playerCharacter;
            var handObject = playerCharacter.PlayerHotbar.currentHandObject;
            if(handObject==null)
            {return;}

            _weapon = handObject.GetComponent<Weapon>();
            
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (_weapon != null&&PlayerCharacter.IsRunOnServer)
            {
                _weapon.charge = stateInfo.normalizedTime;
            }
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