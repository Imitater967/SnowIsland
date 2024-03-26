using UnityEngine;

namespace SnowIsland.Scripts.Item
{
    
    [CreateAssetMenu(fileName = "asset", menuName = "SnowIsland/Item/Gun", order = 0)]
    public  class GunAsset: ItemAsset,IItemAnimation
    {
        
        [field: SerializeField,Header("物品动画Id"),Tooltip("动画ID")]
        public int animationOnUse { get; private set; } 
        public int GetItemAnimation()
        {
            return animationOnUse;
        }

        public virtual void OnAim(){}
        public virtual void OnFire(){}
        public virtual void OnReload(){}
    }
}