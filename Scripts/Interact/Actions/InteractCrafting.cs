using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Facilities;
using SnowIsland.Scripts.UI.Game;
using SnowIsland.Scripts.UI.Game.Interact.Actions;
using UnityEngine;

namespace SnowIsland.Scripts.Interact.Actions
{
    public class InteractCrafting : ActionLogic
    {
        public override void Perform(Interactable interactable)
        {
            base.Perform(interactable);   
            interactable.GetComponent<CraftingTable>().OpenServerRpc(PlayerCharacter.Local.PlayerInteract); 

        }
    }
}