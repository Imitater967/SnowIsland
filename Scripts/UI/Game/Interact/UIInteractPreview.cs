using System;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Interact;
using SnowIsland.Scripts.UI.Game.Interact.Actions;
using UnityEngine;
using UnityEngine.Serialization;

namespace SnowIsland.Scripts.UI.Game.Interact
{
    public class UIInteractPreview : MonoBehaviour
    {
        [FormerlySerializedAs("InteractZone")] public Interactable interactableClient;
        public List<UIInteractAction> InteractActions;
        public int selection = 0;
        public bool isPressing;
        [SerializeField,ReadOnly]
        private bool tickPress;

        public UIInteractAction currentAction => InteractActions[selection];
        
        [SerializeField, ReadOnly] private float pressStartTime;

        private void Start()
        {
            if (InteractActions.Count == 0)
                enabled = false;
            UpdatePos();
        }

        private void Update()
        {
            UpdatePos();
            for (var i = 0; i < InteractActions.Count; i++)
            {
                InteractActions[i].selectOutline.SetActive(i==selection);
            }
            //操作无法执行
            var selectedAction = InteractActions[selection];
            var actionLogic = selectedAction.GetComponent<ActionLogic>();
            if (!actionLogic.CanPerform(interactableClient))
            {
                return;
            } 
            //玩家正在按E
            if (isPressing)
            {
                //第一次按E
                if(!tickPress){
                    tickPress = true;
                    pressStartTime = Time.time;
                    PlayerCharacter.Local.PlayerInteract.RequestConfirmServerRpc(true,currentAction.interactAnimationId);
                }
                //按E进行中
                if (tickPress)
                {
                    //刷新进度条
                    selectedAction.progress.fillAmount =
                        (Time.time - pressStartTime) / selectedAction.requireTime;
                    //时间达到,执行逻辑Perform
                    if (Time.time >= pressStartTime + selectedAction.requireTime)
                    {
                        actionLogic.Perform(interactableClient);
                        Debug.Log("本地玩家尝试执行"+actionLogic.GetType().Name);
                        isPressing = false;
                        tickPress = false; 
                    }
                }
            }
            else
            {
                //松开的第一帧
                if (tickPress)
                {
                    PlayerCharacter.Local.PlayerInteract.RequestConfirmServerRpc(false,0);
                }
                tickPress = false; 
                selectedAction = InteractActions[selection];
                selectedAction.progress.fillAmount = 0;
            }
        }

        private void UpdatePos()
        {
            transform.position =
                RectTransformUtility.WorldToScreenPoint(Camera.main, interactableClient.transform.position + interactableClient.uiOffset);
        }
        public void Next()
        {
            selection += 1;
            if (selection >= InteractActions.Count)
            {
                selection = 0;
            }

            isPressing = false;
            tickPress = false;
            PlayerCharacter.Local.PlayerInteract.RequestConfirmServerRpc(false,0);
        }

        private void OnDisable()
        {
            isPressing = false;  
            PlayerCharacter.Local.PlayerInteract.RequestConfirmServerRpc(false,0); 
        }
    }
}