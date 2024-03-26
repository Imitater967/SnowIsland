using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Inventory;
using SnowIsland.Scripts.UI.Game.Interact.Actions;
using Unity.Services.Lobbies.Models;

namespace SnowIsland.Scripts.Interact.Actions
{
    public class InteractPickupItem: ActionLogic
    {
        public override void Perform(Interactable interactable)
        {
            base.Perform(interactable);
           var item= interactable.GetComponent<ItemDropped>();
           PlayerCharacter.Local.PlayerInteract.StopInteractServerRpc();
            //先销毁再拾取,不然networkbehaviour被摧毁就不好了
           if(item!=null)
               PlayerCharacter.Local.PlayerHotbar.PickUpServerRpc(item);
        }

        public override bool CanPerform(Interactable interactable)
        {
            var dropped=interactable.GetComponent<ItemDropped>();
            return PlayerCharacter.Local.PlayerHotbar.CanAdd(dropped.item.Value, dropped.amount.Value);
        }
    }
}