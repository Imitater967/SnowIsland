using SnowIsland.Scripts.Character;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game
{
    /*
     * 背包中,可以拖曳的东西,结构为
     * UIPlayerHotBar
     * - UIPlayerHotBarSlot
     *  - UIDragAndDropable
     * 当拖曳的时候,根据拖到地方的dragee所属的背包,以及位置,执行drop等操作故需要两个属性,一个是位置,一个是所属背包
     * - 位置: 由UIPlayerHotBar在实例化的时候,修改UIDragAndDropable的名字为index进行
     * - 所属: 用tag,hotbarslot和croposeslot酱, sendMessage OnDragAndDrop_hotbarslot_cropseLot
     *  - 毕竟交互的背包除了hotbar自己交互,就是和交互系统中当前交互的东西进行背包的操作
     */
    public class UIDragAndDropable : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        
        public GameObject drageePrefab;
        public static GameObject currentlyDragged;
        public bool dragable = true;
        public bool dropable = true;
        [HideInInspector] public bool draggedToSlot = false;
        public void OnBeginDrag(PointerEventData eventData)
        { 
            if (dragable  )
            {
                // load current
                currentlyDragged = Instantiate(drageePrefab, transform.position, Quaternion.identity);
                currentlyDragged.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
                currentlyDragged.GetComponent<Image>().color = GetComponent<Image>().color; // for durability etc.
                currentlyDragged.transform.SetParent(transform.root, true); // canvas
                currentlyDragged.transform.SetAsLastSibling(); // move to foreground

                // disable button while dragging so onClick isn't fired if we drop a
                // slot on itself
                GetComponent<Button>().interactable = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        { 
            if (dragable  )
                // move current
                currentlyDragged.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
         // delete current in any case
              Destroy(currentlyDragged);
      
              // one mouse button is enough for drag and drop
              if (dragable  )
              {
                  // try destroy if not dragged to a slot (flag will be set by slot)
                  // message is sent to drag and drop handler for game specifics
                  // -> only if dropping it into nirvana. do nothing if we just drop
                  //    it on a panel. otherwise item slots are cleared if we
                  //    accidentally drop it on the panel between two slots 
                  if (!draggedToSlot && eventData.pointerEnter == null)
                  {
                      // send a drag and clear message like
                      // OnDragAndClear_Skillbar({index})
                      PlayerCharacter.Local.SendMessage("OnDragAndClear_" + tag,
                                                     name.ToInt(),
                                                     SendMessageOptions.DontRequireReceiver);
                      Debug.Log("Inventory: Executing "+"OnDragAndClear_" + tag);
                  }
      
                  // reset flag
                  draggedToSlot = false;
      
                  // enable button again
                  GetComponent<Button>().interactable = true;
              }
              }

        public void OnDrop(PointerEventData eventData)
        {
            // one mouse button is enough for drag and drop
            if (dropable )
            {
                
                // was the dropped GameObject a UIDragAndDropable and was it dragable?
                // (Unity calls OnDrop even if .dragable was false)
                UIDragAndDropable dropDragable = eventData.pointerDrag.GetComponent<UIDragAndDropable>();
                if (dropDragable != null && dropDragable.dragable)
                {
                    // let the dragable know that it was dropped onto a slot
                    dropDragable.draggedToSlot = true;

                    // only do something if we didn't drop it on itself. this way we
                    // don't have to ignore raycasts etc.
                    // message is sent to drag and drop handler for game specifics
                    if (dropDragable != this)
                    {
                        // send a drag and drop message like
                        // OnDragAndDrop_Skillbar_Inventory({from, to})
                        int from = dropDragable.name.ToInt();
                        int to = name.ToInt();
                        PlayerCharacter.Local.SendMessage("OnDragAndDrop_" + dropDragable.tag + "_" + tag,
                            new int[]{from, to},
                            SendMessageOptions.DontRequireReceiver);
                        
                        Debug.Log("Inventory: Executing "+"OnDragAndDrop_" + dropDragable.tag + "_" + tag);
                    }
                }
            }
        }
    }
}