using System;
using SnowIsland.Scripts.Inventory;
using SnowIsland.Scripts.Item;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    public class PlayerHotbar: Inventory.Inventory
    {
        private PlayerCharacter PlayerCharacter;
        [field:SerializeField]
        public NetworkVariable<int> selection { get; private set; }= new NetworkVariable<int>(0); 
        public Transform handTransform;

        public GameObject currentHandObject
        {
            get
            {
                if (handTransform.childCount == 0)
                    return null;
                return handTransform.GetChild(0).gameObject;
            }
        }
        public Inventory.Item itemInHand=>slots[selection.Value].item;

        [field:SerializeField]
        public NetworkVariable<bool> backpacked { get; private set; } = new NetworkVariable<bool>(false);
        protected override void Awake()
        {
            base.Awake();
            PlayerCharacter = GetComponent<PlayerCharacter>();
            PlayerCharacter.PlayerStatus.OnPlayerDeathServer += () =>
            {
                DestroyHandItemClientRpc();
            };

        }
        [ClientRpc]
        private void DestroyHandItemClientRpc()
        {
            for (int i = 0; i < handTransform.childCount; i++)
            {
                Destroy(handTransform.GetChild(i).gameObject);
            }
        }

        //如果有背包,就解锁第五格,如果没有,就-1
        public override bool CanAdd(Inventory.Item item, int amount)
        { 
            int backpack=backpacked.Value?0:-1;
            // go through each slot
            for (int i = 0; i < slots.Count+backpack; ++i)
            {
                // empty? then subtract maxstack
                if (slots[i].amount == 0)
                    amount -= item.maxStack;
                // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                else if (slots[i].item.Equals(item))
                    amount -= (slots[i].item.maxStack - slots[i].amount);

                // were we able to fit the whole amount already?
                if (amount <= 0) return true;
            }

            // if we got here than amount was never <= 0
            return false; 
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            selection.OnValueChanged += (a, b) =>
            {
                if (IsServer)
                { 
                    PlayerCharacter.PlayerItemUsage.useInputOnServer = false;
                }
                RefreshHandModel(); 
            };
             
                slots.OnListChanged += OnHotBarChanged;
                RefreshHandModel(); 

        }

        private void OnHotBarChanged(NetworkListEvent<ItemSlot> changeevent)
        {
    
                RefreshHandModel(); 
        }

        void RefreshHandModel()
        {
            if(PlayerCharacter.PlayerStatus.dead.Value)
                return;
            ItemSlot slot = slots[selection.Value];
            var item = slot.item.asset;
            if (handTransform.childCount> 0)
            {
                var oldModelTransform = handTransform.GetChild(0);
                GameObject oldModel = oldModelTransform.gameObject;
                oldModel.transform.parent = null;
                Destroy(oldModel);
            } 
            if (item.modelPrefab != null)
            {
                var newModel=Instantiate(item.modelPrefab, handTransform, false);
                newModel.SetActive(true);
            }
        }
 
        #region Inv Basic

        [ServerRpc]
        public void SelectServerRpc(int index)
        {
            //禁止在攻击的时候切换物品
            if (currentHandObject != null)
            {
                if (currentHandObject.TryGetComponent(out Weapon weapon))
                {
                    if(weapon.enabled)
                        return;
                }
            }
            //如果没有开启背包,就不启动第五格
            if(index==4&&!backpacked.Value)
                return;
            //使用物品时候无法切换物品
            if(PlayerCharacter.PlayerItemUsage.useInputOnServer)
                return;
            //根据选择,切换物品
            if (0 <= index && index < slots.Count )
                selection.Value = index;
        }
        [ServerRpc]
        public void SwapHotbarHotbarServerRpc(int fromIndex, int toIndex)
        {
            // note: should never send a command with complex types!
            // validate: make sure that the slots actually exist in the inventory
            // and that they are not equal
            if (!PlayerCharacter.dead &&
                0 <= fromIndex && fromIndex < slots.Count &&
                0 <= toIndex && toIndex < slots.Count &&
                fromIndex != toIndex)
            {
                // swap them
                ItemSlot temp = slots[fromIndex];
                slots[fromIndex] = slots[toIndex];
                slots[toIndex] = temp;
            }
        }
        [ServerRpc]
        public void HotbarSplitServerRpc(int fromIndex, int toIndex)
        {
            // note: should never send a command with complex types!
            // validate: make sure that the slots actually exist in the inventory
            // and that they are not equal
            if (!PlayerCharacter.dead  &&
                0 <= fromIndex && fromIndex < slots.Count &&
                0 <= toIndex && toIndex < slots.Count &&
                fromIndex != toIndex)
            {
                // slotFrom needs at least two to split, slotTo has to be empty
                ItemSlot slotFrom = slots[fromIndex];
                ItemSlot slotTo = slots[toIndex];
                if (slotFrom.amount >= 2 && slotTo.amount == 0)
                {
                    // split them serversided (has to work for even and odd)
                    slotTo = slotFrom; // copy the value

                    slotTo.amount = slotFrom.amount / 2;
                    slotFrom.amount -= slotTo.amount; // works for odd too

                    // put back into the list
                    slots[fromIndex] = slotFrom;
                    slots[toIndex] = slotTo;
                }
            }
        }
        [ServerRpc]
        public void HotbarMergeServerRpc(int fromIndex, int toIndex)
        {
            if (!PlayerCharacter.dead &&
                0 <= fromIndex && fromIndex < slots.Count &&
                0 <= toIndex && toIndex < slots.Count &&
                fromIndex != toIndex)
            {
                // both items have to be valid
                ItemSlot slotFrom = slots[fromIndex];
                ItemSlot slotTo = slots[toIndex];
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
                        slots[fromIndex] = slotFrom;
                        slots[toIndex] = slotTo;
                    }
                }
            }
        }
        #endregion
        //使用背包添加slot
        public void AddSlot()
        {
            slots.Add(new ItemSlot());
        }
        [ServerRpc]
        public void PickUpServerRpc(NetworkBehaviourReference itemRef)
        {
            var item = (ItemDropped)itemRef;
            if(item==null)
                return;
            if (CanAdd(item.item.Value, item.amount.Value))
            {
                Add(item.item.Value, item.amount.Value);
                item.GetComponent<NetworkObject>().Despawn(true);
            }
        }

        [ServerRpc]
        public void DropItemServerRpc()
        {
            var itemSlot = slots[selection.Value];
            if(!CheckCanDrop(itemSlot))
                return;
            if(itemSlot.amount<=0)
                return;
            //先掉落物品,然后减少数量
            SpawnDroppedItem(itemSlot.item,1); 
            itemSlot.DecreaseAmount(1);
            slots[selection.Value] = itemSlot;  
        }       
        [ServerRpc]
        public void DropSlotServerRpc(int slot)
        {
            
            ItemSlot itemSlot = slots[slot];
            //使用期间禁止掉东西
            if(!CheckCanDrop(itemSlot))
                return;
           DropItemAndClearSlot(slot);
        }
        public void SpawnDroppedItem(Inventory.Item item, int amount)
        {
            if (item.asset.drop==null)
            {
                return;
            }
            Transform modelTransform = handTransform.transform;
            if (handTransform.childCount >= 1)
            {
                modelTransform = modelTransform.GetChild(0);
            }
             
            // drop
            GameObject go = Instantiate(item.asset.drop.gameObject,modelTransform.position,modelTransform.rotation);
            ItemDropped drop = go.GetComponent<ItemDropped>();
            go.GetComponent<NetworkObject>().Spawn();
            drop.item.Value = item;
            drop.amount.Value = amount;
        }
        private void DropItemAndClearSlot(int index)
        {
            
            // drop and remove from inventory
            ItemSlot slot = slots[index];
            SpawnDroppedItem(slot.item, slot.amount);
            slot.amount = 0;
            slot.item.itemType = ItemType.Air;
            slots[index] = slot;
         
        }

        private bool CheckCanDrop(ItemSlot itemSlot)
        {
            return !PlayerCharacter.PlayerItemUsage.useInputOnServer&&
                   !PlayerCharacter.PlayerItemUsage.isItemUsing.Value&&itemSlot.item.asset.drop!=null&&!PlayerCharacter.PlayerGunUsage.aiming.Value;
        }

        #region Inv Calls
        void OnDragAndClear_HotbarSlot(int slotIndex)
        {
            DropSlotServerRpc(slotIndex);
        }
        void OnDragAndDrop_HotbarSlot_HotbarSlot(int[] slotIndices)
        {
            // slotIndices[0] = slotFrom; slotIndices[1] = slotTo

            // merge? (just check equality, rest is done server sided)
            if (slots[slotIndices[0]].amount > 0 && slots[slotIndices[1]].amount > 0 &&
                slots[slotIndices[0]].item.Equals(slots[slotIndices[1]].item))
            {
                HotbarMergeServerRpc(slotIndices[0], slotIndices[1]);
            }
            // split?
            else if (PlayerCharacter.PlayerControl.split)
            {
                HotbarSplitServerRpc(slotIndices[0], slotIndices[1]);
            }
            // swap?
            else
            { 
                SwapHotbarHotbarServerRpc(slotIndices[0], slotIndices[1]);
            }
        }

        #endregion
    }
}