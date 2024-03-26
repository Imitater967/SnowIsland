using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Interact;
using UnityEngine;
using UnityEngine.UI;

namespace SnowIsland.Scripts.UI.Game.Interact
{
    public class UIInteractAction: MonoBehaviour
    {
        public GameObject selectOutline;
        public float requireTime;
        public Image progress;
        public int interactAnimationId; 
    }
}