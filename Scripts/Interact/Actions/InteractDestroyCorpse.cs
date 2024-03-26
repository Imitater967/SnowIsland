using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Chest;
using SnowIsland.Scripts.Resources;
using SnowIsland.Scripts.UI.Game.Interact.Actions;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Interact.Actions
{
    public class InteractDestroyCorpse : ActionLogic
    {
        public override void Perform(Interactable interactable)
        {
            base.Perform(interactable);
         
            interactable.GetComponent<ICorpse>().DestroyServerRpc( );
            PlayerCharacter.Local.PlayerInteract.StopInteractServerRpc();
        }
    }
}