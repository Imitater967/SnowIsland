using System;
using SnowIsland.Scripts.UI.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace SnowIsland.Scripts.Character
{
    [DisallowMultipleComponent]
    public class PlayerIndicator : MonoBehaviour
    {
        
        [FormerlySerializedAs("playerIndicatorPrefab")] [SerializeField] private UIPosIndicator posIndicatorPrefab;
        [SerializeField, Scripts.ReadOnly] private UIPosIndicator posIndicatorInstance;

   
        private void OnEnable()
        { 
            posIndicatorInstance = Instantiate(posIndicatorPrefab.gameObject,GameObject.Find("Canvas/Indicators").transform).GetComponent<UIPosIndicator>();
        }
        
        private void Update()
        {
            var indicatorRect=posIndicatorInstance.gameObject.GetComponent<RectTransform>();
            var size = indicatorRect.sizeDelta;
          //  Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 pos =
                RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position ); 
            float xDistance = Screen.width ;
            float yDistance = Screen.height ;   
            float x = Mathf.Clamp(pos.x,  size.x, xDistance - size.x);
            float y = Mathf.Clamp(pos.y,   size.y, yDistance - size.y); ;
            var uiTransform = posIndicatorInstance.gameObject.transform;
            var uiPos = new Vector2(x, y);
            uiTransform.position = uiPos;
            var centerPos = new Vector2(xDistance / 2, yDistance / 2);
            UILookAt(uiTransform,(-(centerPos-uiPos)),Vector3.right);
        }
        public void UILookAt(Transform transform, Vector3 dir, Vector3 lookAxis)
        {
            Quaternion q = Quaternion.identity;
            q.SetFromToRotation(lookAxis, dir);
            transform.rotation = q;
        } 
        private void OnDisable()
        {
            Destroy(posIndicatorInstance.gameObject);
        }
    }
}