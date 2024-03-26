using System;
using SnowIsland.Scripts.UI.Game.Interact.Actions;
using UnityEngine;

namespace SnowIsland.Scripts.UI.Game.Interact
{
    [RequireComponent(typeof(ActionLogic))]
    public class UIInteractCheckCanPerform : MonoBehaviour
    {
        private ActionLogic action;
        private UIInteractPreview parent;
        [SerializeField]
        private GameObject cantDoMask;

        private void Awake()
        {
            action = GetComponent<ActionLogic>();
            parent = GetComponentInParent<UIInteractPreview>();
        }

        private void Update()
        {
            cantDoMask.gameObject.SetActive(!action.CanPerform(parent.interactableClient));
        }
    }
}