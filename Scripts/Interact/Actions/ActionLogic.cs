using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Interact;
using UnityEngine;

namespace SnowIsland.Scripts.UI.Game.Interact.Actions
{
    public class ActionLogic : MonoBehaviour
    {
        private bool canPerform;
        public virtual void Perform(Interactable interactable)
        { 
            PlayerCharacter.Local.PlayerInteract.RequestInteractServerRpc();
        }

        public virtual bool CanPerform(Interactable interactable)
        {
            return true;
        }
    }
}