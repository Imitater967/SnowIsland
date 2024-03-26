using System;
using SnowIsland.Scripts.Chest;
using SnowIsland.Scripts.Inventory;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    public class PlayerStorageUsage: NetworkBehaviour
    {
        private PlayerHotbar inventory;
        private PlayerInteract _playerInteract;
        private PlayerControl _playerControl;

        private void Awake()
        {
            _playerControl = GetComponent<PlayerControl>();
            _playerInteract = GetComponent<PlayerInteract>();
            inventory = GetComponent<PlayerHotbar>();
        }

        #region Rpcs

        [ServerRpc]
        public void SwapStorageStorageServerRpc(NetworkObjectReference storageGameObjectRef, int fromIndex, int toIndex)
        {
            GameObject storageGameObject = storageGameObjectRef;
            // validate: make sure that the slots actually exist in the inventory
            // and that they are not equal
            if (storageGameObject != null)
            {
                ChestStorage storage = storageGameObject.GetComponent<ChestStorage>();
                if (storage != null && 
                    0 <= fromIndex && fromIndex < storage.slots.Count &&
                    0 <= toIndex && toIndex < storage.slots.Count &&
                    fromIndex != toIndex)
                {
                    // swap them
                    ItemSlot temp = storage.slots[fromIndex];
                    storage.slots[fromIndex] = storage.slots[toIndex];
                    storage.slots[toIndex] = temp;
                }
            }
        }    
        [ServerRpc]
        public void StorageSplitServerRpc(NetworkObjectReference storageGameObjectRef, int fromIndex, int toIndex)
        {
            
            GameObject storageGameObject = storageGameObjectRef;
            // validate: make sure that the slots actually exist in the inventory
            // and that they are not equal
            if (storageGameObject != null)
            {
                ChestStorage storage = storageGameObject.GetComponent<ChestStorage>();
                if (storage != null && 
                    0 <= fromIndex && fromIndex < storage.slots.Count &&
                    0 <= toIndex && toIndex < storage.slots.Count &&
                    fromIndex != toIndex)
                {
                    // slotFrom needs at least two to split, slotTo has to be empty
                    ItemSlot slotFrom = storage.slots[fromIndex];
                    ItemSlot slotTo = storage.slots[toIndex];
                    if (slotFrom.amount >= 2 && slotTo.amount == 0) {
                        // split them serversided (has to work for even and odd)
                        slotTo = slotFrom; // copy the value

                        slotTo.amount = slotFrom.amount / 2;
                        slotFrom.amount -= slotTo.amount; // works for odd too

                        // put back into the list
                        storage.slots[fromIndex] = slotFrom;
                        storage.slots[toIndex] = slotTo;
                    }
                }
            }
        }
        [ServerRpc]
        public void StorageMergeServerRpc(NetworkObjectReference storageGameObjectRef, int fromIndex, int toIndex)
        {
            
            GameObject storageGameObject = storageGameObjectRef;
            if (storageGameObject != null)
            {
                ChestStorage storage = storageGameObject.GetComponent<ChestStorage>();
                if (storage != null && 
                    0 <= fromIndex && fromIndex < storage.slots.Count &&
                    0 <= toIndex && toIndex < storage.slots.Count &&
                    fromIndex != toIndex)
                {
                    // both items have to be valid
                    ItemSlot slotFrom = storage.slots[fromIndex];
                    ItemSlot slotTo = storage.slots[toIndex];
                    if (slotFrom.amount > 0 && slotTo.amount > 0)
                    {
                        // make sure that items are the same type
                        // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                        if (slotFrom.item.Equals(slotTo.item))
                        {
                            // merge from -> to
                            // put as many as possible into 'To' slot
                            int put = slotTo.IncreaseAmount(slotFrom.amount);
                            slotFrom.DecreaseAmount(put);

                            // put back into the list
                            storage.slots[fromIndex] = slotFrom;
                            storage.slots[toIndex] = slotTo;
                        }
                    }
                }
            }
        }
        [ServerRpc]
        public void SwapInventoryStorageServerRpc(NetworkObjectReference storageGameObjectRef, int inventoryIndex, int storageIndex)
        {
            
            GameObject storageGameObject = storageGameObjectRef;
            if (storageGameObject != null)
            {
                ChestStorage storage = storageGameObject.GetComponent<ChestStorage>();
                if (storage != null &&  0 <= inventoryIndex && inventoryIndex < inventory.slots.Count &&
                    0 <= storageIndex && storageIndex < storage.slots.Count)
                {
                    // swap them
                    ItemSlot temp = storage.slots[storageIndex];
                    storage.slots[storageIndex] = inventory.slots[inventoryIndex];
                    inventory.slots[inventoryIndex] = temp;
                }
            }
        }
        [ServerRpc]
        public void MergeInventoryStorageServerRpc(NetworkObjectReference storageGameObjectRef, int inventoryIndex, int storageIndex)
        {
            GameObject storageGameObject = storageGameObjectRef;
            // validate: make sure that the slots actually exist in the inventory
            // and in the storage
            if (storageGameObject != null)
            { 
                ChestStorage storage = storageGameObject.GetComponent<ChestStorage>();
                if (storage != null && 
                    0 <= inventoryIndex && inventoryIndex < inventory.slots.Count &&
                    0 <= storageIndex && storageIndex < storage.slots.Count)
                {
                    // both items have to be valid
                    ItemSlot slotFrom = inventory.slots[inventoryIndex];
                    ItemSlot slotTo = storage.slots[storageIndex];
                    if (slotFrom.amount > 0 && slotTo.amount > 0)
                    {
                        // make sure that items are the same type
                        // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                        if (slotFrom.item.Equals(slotTo.item))
                        {
                            // merge from -> to
                            // put as many as possible into 'To' slot
                            int put = slotTo.IncreaseAmount(slotFrom.amount);
                            slotFrom.DecreaseAmount(put);

                            // put back into the lists
                            inventory.slots[inventoryIndex] = slotFrom;
                            storage.slots[storageIndex] = slotTo;
                        }
                    }
                }
            }
        }
        [ServerRpc]
        public void MergeStorageInventoryServerRpc(NetworkObjectReference storageGameObjectRef, int storageIndex, int inventoryIndex)
        {
            GameObject storageGameObject = storageGameObjectRef;
            // validate: make sure that the slots actually exist in the inventory
            // and in the storage
            if (storageGameObject != null)
            {
                ChestStorage storage = storageGameObject.GetComponent<ChestStorage>();
                if (storage != null && 
                    0 <= inventoryIndex && inventoryIndex < inventory.slots.Count &&
                    0 <= storageIndex && storageIndex < storage.slots.Count)
                {
                    // both items have to be valid
                    ItemSlot slotFrom = storage.slots[storageIndex];
                    ItemSlot slotTo = inventory.slots[inventoryIndex];
                    if (slotFrom.amount > 0 && slotTo.amount > 0)
                    {
                        // make sure that items are the same type
                        // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                        if (slotFrom.item.Equals(slotTo.item))
                        {
                            // merge from -> to
                            // put as many as possible into 'To' slot
                            int put = slotTo.IncreaseAmount(slotFrom.amount);
                            slotFrom.DecreaseAmount(put);

                            // put back into the lists
                            storage.slots[storageIndex] = slotFrom;
                            inventory.slots[inventoryIndex] = slotTo;
                        }
                    }
                }
            }
        }

        #endregion

        #region DropAndDrag
// drag & drop /////////////////////////////////////////////////////////////
    void OnDragAndDrop_StorageSlot_StorageSlot(int[] slotIndices)
    {
        // slotIndices[0] = slotFrom; slotIndices[1] = slotTo
        if (_playerInteract.clientInteractable != null)
        {
            ChestStorage storage = _playerInteract.clientInteractable.GetComponent<ChestStorage>();
             if (storage != null)
            {
                // merge? (just check equality, rest is done server sided)
                if (storage.slots[slotIndices[0]].amount > 0 && storage.slots[slotIndices[1]].amount > 0 &&
                    storage.slots[slotIndices[0]].item.Equals(storage.slots[slotIndices[1]].item))
                {
                     StorageMergeServerRpc(storage.gameObject, slotIndices[0], slotIndices[1]);
                }
                // split?
                else if (_playerControl.split)
                {
                    StorageSplitServerRpc(storage.gameObject, slotIndices[0], slotIndices[1]);
                }
                // swap?
                else
                {
                    SwapStorageStorageServerRpc(storage.gameObject, slotIndices[0], slotIndices[1]);
                }
            }
        }
    }

    void OnDragAndDrop_HotbarSlot_StorageSlot(int[] slotIndices)
    {
        // slotIndices[0] = slotFrom; slotIndices[1] = slotTo
        if (_playerInteract.clientInteractable != null)
        {
            ChestStorage storage = _playerInteract.clientInteractable.GetComponent<ChestStorage>();
            if (storage != null)
            {
                // merge? (just check equality, rest is done server sided)
                if (inventory.slots[slotIndices[0]].amount > 0 && storage.slots[slotIndices[1]].amount > 0 &&
                    inventory.slots[slotIndices[0]].item.Equals(storage.slots[slotIndices[1]].item))
                {
                    MergeInventoryStorageServerRpc(storage.gameObject, slotIndices[0], slotIndices[1]);
                }
                // swap?
                else
                {
                    SwapInventoryStorageServerRpc(storage.gameObject, slotIndices[0], slotIndices[1]);
                }
            }
        }
    }

    void OnDragAndDrop_StorageSlot_HotbarSlot(int[] slotIndices)
    {
        // slotIndices[0] = slotFrom; slotIndices[1] = slotTo
        if (_playerInteract.clientInteractable != null)
        {
            ChestStorage storage = _playerInteract.clientInteractable.GetComponent<ChestStorage>();
            if (storage != null)
            {
                // merge? (just check equality, rest is done server sided)
                if (storage.slots[slotIndices[0]].amount > 0 && inventory.slots[slotIndices[1]].amount > 0 &&
                    storage.slots[slotIndices[0]].item.Equals(inventory.slots[slotIndices[1]].item))
                {
                    MergeStorageInventoryServerRpc(storage.gameObject, slotIndices[0], slotIndices[1]);
                }
                // swap?
                else
                {
                     SwapInventoryStorageServerRpc(storage.gameObject, slotIndices[1], slotIndices[0]);
                }
            }
        }
    }
        

        #endregion
    }
}