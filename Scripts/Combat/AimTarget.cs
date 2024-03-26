using System;
using UnityEngine;

namespace SnowIsland.Scripts.Combat
{
    public class AimTarget : MonoBehaviour,IAimTarget
    {
        public GameObject aimOutline;
        public Vector3 aimOffset;
        private void Awake()
        {
            aimOutline.gameObject.SetActive(false);
        }

        public void OnFocus()
        {
            Debug.Log("Local Player Focus On "+name);
            aimOutline.gameObject.SetActive(true);
        }

        public void OnLoseFocus()
        {
            Debug.Log("Local Player Focus Lose On "+name);
            aimOutline.gameObject.SetActive(false);
        }

        public Vector3 GetAimOffset()
        {
            return aimOffset;
        }
    }
}