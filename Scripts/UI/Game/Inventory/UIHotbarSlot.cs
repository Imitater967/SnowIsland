using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game
{
    public class UIHotbarSlot : MonoBehaviour
    {
        public UIShowToolTip tooltip;
        public UIDragAndDropable dragAndDropable;
        public Image image;
        public Button button; 
        public GameObject amountOverlay;
        public TMP_Text amountText;
        public TMP_Text hotkeyText;
        public GameObject selectionOutline;
    }

}