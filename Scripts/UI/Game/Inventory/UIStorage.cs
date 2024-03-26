using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Chest;
using SnowIsland.Scripts.Interact;
using SnowIsland.Scripts.Inventory;
using SnowIsland.Scripts.Resources;
using TMPro;
using UnityEngine;

namespace SnowIsland.Scripts.UI.Game
{
    public class UIStorage : MonoBehaviour
    {
        public TMP_Text name;
        public Transform content;
        [ReadOnly]
        public List<UIStorageSlot> contents;

        private PlayerInteract interactable;
        private ChestStorage chest;
 
        private void Start()
        {
            for (int i = 0; i < content.childCount; i++)
            {
                contents.Add(content.GetChild(i).GetComponent<UIStorageSlot>());
            }
        }

        private void Update()
        { 
            interactable=PlayerCharacter.Local.PlayerInteract;
            if (interactable.clientInteractable == null)
            { 
                gameObject.SetActive(false);
                return;
            }   
            chest=interactable.clientInteractable.GetComponent<ChestStorage>();
            name.text = chest.name.Value.SomeText;
            for (var i = 0; i < contents.Count; i++)
            { 
                var slot = contents[i];
                slot.dragAndDropable.name = i.ToString(); // drag and drop index
                ItemSlot itemSlot = chest.slots[i];
                slot.image.color = Color.white; // reset for non-durability items
                slot.image.sprite = itemSlot.item.image;
                slot.amountOverlay.SetActive(itemSlot.item.maxStack > 1);
                slot.amountText.text = itemSlot.amount.ToString();
            }
        }
    }
}