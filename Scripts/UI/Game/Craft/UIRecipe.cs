using System;
using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game.Craft
{
    public class UIRecipe: MonoBehaviour
    {
        public int recipeIndex;
        public Image imageA;
        public TMP_Text amountA;
        public Image imageB;
        public TMP_Text amountB;
        public Image imageResult;
        public TMP_Text amountResult;
        public Image selection;
        public Image canCraft;
        public Button button;
        private UICrafting _uiCrafting;
        private void Awake()
        {
            selection.gameObject.SetActive(false);
        }
        //如果不能合成,就打开蒙版,反之则关闭
        public void MarkCanCraft(bool can)
        {
            if (can)
            {
                button.enabled = true;
                canCraft.gameObject.SetActive(false);
                return;
            }
            button.enabled = false;
            canCraft.gameObject.SetActive(true);
        }
        //button
        public void OnSelect()
        {
            _uiCrafting.ChangeRecipe(recipeIndex);
        }

        public void Init(UICrafting ui)
        {
            _uiCrafting = ui;
            var recipe = AbstractRecipeManager.Instance.recipes[this.recipeIndex];
            var itemA = AbstractItemManager.Instance.GetItem(recipe.ingredientA.item);
            var itemB = AbstractItemManager.Instance.GetItem(recipe.ingredientB.item);
            var result = AbstractItemManager.Instance.GetItem(recipe.result.item);
            amountA.text=""+ recipe.ingredientA.amount;
            imageA.sprite = itemA.image;
            amountB.text=""+ recipe.ingredientB.amount;
            imageB.sprite = itemB.image; 
            amountResult.text=""+ recipe.result.amount; 
            imageResult.sprite = result.image;
        }

        public void Select()
        {
            selection.gameObject.SetActive(true);
        }

        public void DeSelect()
        {
            selection.gameObject.SetActive(false);
        }
    }
}