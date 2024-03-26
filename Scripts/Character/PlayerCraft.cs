using System;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    public class PlayerCraft : NetworkBehaviour
    {
        private PlayerCharacter PlayerCharacter;
        private PlayerHotbar PlayerHotbar; 
        public int ClientRecipeIndex
        {
            get { return clientRecipeIndex; }
            set
            {
                clientRecipeIndex = value;
                Debug.Log(PlayerCharacter.Local.name+" has now changed craft index to "+value);
            }
        }
        private int clientRecipeIndex;

        private void Awake()
        {
            PlayerCharacter = GetComponent<PlayerCharacter>();
            PlayerHotbar = GetComponent<PlayerHotbar>();
        }
 
        [ServerRpc]
        public void CraftServerRpc(int id)
        {
            var recipe = AbstractRecipeManager.Instance.GetRecipe(id);
            if (!EnoughItem(id))
            {
                Debug.LogError(name+": Trying to craft while not enough item");
                return;
            }

            PlayerHotbar.Remove(new Inventory.Item() { itemType = recipe.ingredientA.item }, recipe.ingredientA.amount);
            PlayerHotbar.Remove(new Inventory.Item() { itemType = recipe.ingredientB.item }, recipe.ingredientB.amount);
            if (PlayerHotbar.CanAdd(new Inventory.Item() { itemType = recipe.result.item }, recipe.result.amount))
            {
                PlayerHotbar.Add(new Inventory.Item() { itemType = recipe.result.item }, recipe.result.amount);
            }
            else
            {
                PlayerHotbar.SpawnDroppedItem(new Inventory.Item() { itemType = recipe.result.item }, recipe.result.amount);
            }
            Debug.Log(name+": Crafted with recipe "+recipe.name); 
        }

        public bool EnoughItem(int id)
        {
            var recipe = AbstractRecipeManager.Instance.GetRecipe(id);
           // Debug.Log("Check Enough-----------------");
            bool enoughA=PlayerHotbar.Count(new Inventory.Item(){itemType = recipe.ingredientA.item}) >= recipe.ingredientA.amount;
           // Debug.Log(PlayerHotbar.Count(new Inventory.Item(){itemType = recipe.ingredientA.item}) +"|"+ recipe.ingredientA.amount);
            bool enoughB=PlayerHotbar.Count(new Inventory.Item(){itemType = recipe.ingredientB.item}) >= recipe.ingredientB.amount;
           // Debug.Log(PlayerHotbar.Count(new Inventory.Item(){itemType = recipe.ingredientB.item}) +"|"+ recipe.ingredientB.amount);
            
            //Debug.Log("Check Complete-----------------");
            return enoughA && enoughB;
        }
    }
}