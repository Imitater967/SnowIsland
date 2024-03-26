using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using UnityEngine;

namespace SnowIsland.Scripts.UI.Game
{
    public class UIHotbar : MonoBehaviour
    {
        private PlayerHotbar _playerHotbar;
        private PlayerCharacter PlayerCharacter;
        [field: SerializeField,Tooltip("需要按顺序安放")]
        public List<UIHotbarSlot> slots { get; private set; }
        private void Awake()
        {
            PlayerCharacter=PlayerCharacter.Local;
            if (PlayerCharacter == null||PlayerCharacter.PlayerHotbar==null)
            {
                gameObject.SetActive(false);
                return;
            }

            _playerHotbar = PlayerCharacter.PlayerHotbar;
        }

        private void Update()
        {
            for (var i = 0; i < slots.Count; i++)
            {
                var slot = slots[i]; 
                slot.selectionOutline.SetActive(i == _playerHotbar.selection.Value);
                var itemSlot = _playerHotbar.slots[i];
                if (itemSlot.amount > 0)
                { 
                    slot.tooltip.enabled = true;
                    slot.dragAndDropable.dragable = true;
                    
                    slot.image.color = Color.white;
                    slot.image.sprite = itemSlot.item.image;
                    slot.amountOverlay.SetActive(itemSlot.item.maxStack > 1);
                    if (itemSlot.amount >= 1) slot.amountText.text = itemSlot.amount.ToString();
                }else
                {
                    // refresh invalid item
                    slot.tooltip.enabled = false;
                    slot.dragAndDropable.dragable = false;
                    slot.image.color = Color.clear;
                    slot.image.sprite = null; 
                    slot.amountOverlay.SetActive(false);
                }

                if (i == 4)
                {
                    slot.gameObject.SetActive(_playerHotbar.backpacked.Value);
                }
            }
        }
    }
}