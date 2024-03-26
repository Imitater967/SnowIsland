using SnowIsland.Scripts.Character;
using UnityEngine;

namespace SnowIsland.Scripts.Item.State
{
    public class SItemUse : StateMachineBehaviour
    { 
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            var playerCharacter = animator.GetComponent<PlayerCharacter>();
            if(playerCharacter.IsRunOnServer)
            {
                var playerItemUsage = animator.GetComponent<PlayerItemUsage>();
                playerItemUsage.isItemUsing.Value = false;
                var usable=playerItemUsage.current as UsableItemAsset;
                usable.OnUseEnter(playerCharacter);
            }
        }
 
    }
}