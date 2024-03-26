using SnowIsland.Scripts.Character;
using UnityEngine;

namespace SnowIsland.Scripts.Item.State
{
    public class SFoodAudio : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            var player=animator.GetComponent<PlayerCharacter>();
            if (player.IsRunOnClient)
            {
                player.PlayerHotbar.currentHandObject.GetComponent<AudioSource>().Play();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            var player=animator.GetComponent<PlayerCharacter>();
            if (player.IsRunOnClient)
            {
                player.PlayerHotbar.currentHandObject.GetComponent<AudioSource>().Stop();
            }
            // Debug.Log("Run On Client");
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