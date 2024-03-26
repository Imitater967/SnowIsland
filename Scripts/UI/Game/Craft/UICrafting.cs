using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Inventory;
using SnowIsland.Scripts.Room;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game.Craft
{
    public class UICrafting: MonoBehaviour
    {
        public UIRecipe current;
        
        [SerializeField,ReadOnly]
        public List<UIRecipe> recipes;

        [SerializeField] private AudioClip craftSuccess;
        [SerializeField]
        private Button craftButton;
        [SerializeField]
        private GameObject context;
        [SerializeField]
        private UIRecipe recipePrefab;
        private GameObject craftPreview;
        private void Awake()
        {
            InitRecipes();
            craftButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            RefreshCanCraft();
            if(PlayerCharacter.Local.IsRunOnClient)
                PlayerCharacter.Local.PlayerHotbar.slots.OnListChanged += Refresh;
        }
        private void OnDisable()
        {
            if(PlayerCharacter.Local.IsRunOnClient)
                PlayerCharacter.Local.PlayerHotbar.slots.OnListChanged -= Refresh;
        }

        private void Refresh(NetworkListEvent<ItemSlot> changeevent)
        {
            RefreshCanCraft();
        }

        //bUTTON
        public void Craft()
        {
            PlayerCharacter.Local.PlayerCraft.CraftServerRpc(current.recipeIndex);
            AudioSource.PlayClipAtPoint(craftSuccess,PlayerCharacter.Local.transform.position);
        }

        private void RefreshCanCraft()
        { 
            for (var i = 0; i < recipes.Count; i++)
            {
                bool enough=PlayerCharacter.Local.PlayerCraft.EnoughItem(i); 
                var uiRecipe = recipes[i];
                uiRecipe.MarkCanCraft(enough);
                if (enough)
                {
                    uiRecipe.transform.SetAsFirstSibling();
                }
            }
        }

        private void Update()
        {
            var interactable=PlayerCharacter.Local.PlayerInteract;
            if (interactable.clientInteractable == null)
            { 
                gameObject.SetActive(false);
                return;
            }
        }

        public void ChangeRecipe(int recipeIndex)
        {
            if (current != null)
                current.DeSelect();
            if (this.craftPreview != null)
            {
                Destroy(this.craftPreview);
                Debug.Log("Crafting: destroy preview item");
            }
            craftButton.gameObject.SetActive(true); 
            current = recipes[recipeIndex];
            current.Select();
            PlayerCharacter.Local.PlayerCraft.ClientRecipeIndex = recipeIndex;
            var recipe= AbstractRecipeManager.Instance.GetRecipe(recipeIndex);
            var item=AbstractItemManager.Instance.GetItem(recipe.result.item);
            if (item.modelPrefab != null)
            {
                craftPreview=Instantiate(item.modelPrefab, Vector3.zero, Quaternion.identity,GameObject.Find("CraftPreview").transform);
                craftPreview.transform.localPosition=Vector3.zero;
                craftPreview.AddComponent<RotateYAxis>();
                craftPreview.layer = LayerMask.NameToLayer("CraftPreview");
                Debug.Log("Crafting: begin preview for "+item.name);
            }
        }
        private void InitRecipes()
        {
            for (var i = 0; i < AbstractRecipeManager.Instance.recipes.Count; i++)
            {
               var recipeObject= Instantiate(recipePrefab, context.transform);
               recipeObject.GetComponent<UIRecipe>().recipeIndex = i; 
               recipes.Add(recipeObject);
               recipeObject.Init(this);
            }
        }
    }
}