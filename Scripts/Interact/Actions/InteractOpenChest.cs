using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Resources;
using SnowIsland.Scripts.UI.Game;
using SnowIsland.Scripts.UI.Game.Interact.Actions;
using UnityEngine;

namespace SnowIsland.Scripts.Interact.Actions
{
    public class InteractOpenChest : ActionLogic
    {
        public override void Perform(Interactable interactable)
        {
            base.Perform(interactable);
            interactable.GetComponent<Chest.Chest>().OpenServerRpc(PlayerCharacter.Local.PlayerInteract); 
        }
    }
}