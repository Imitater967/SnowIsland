using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.UI.Game.Interact.Actions;

namespace SnowIsland.Scripts.Interact.Actions
{
    public class InteractPlayerRecover: ActionLogic
    {
        public override void Perform(Interactable interactable)
        {
            base.Perform(interactable);
            interactable.GetComponentInParent<PlayerStatus>().RecoverServerRpc();
            PlayerCharacter.Local.PlayerInteract.StopInteractServerRpc();
        }
    }
}