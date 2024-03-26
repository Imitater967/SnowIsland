using SnowIsland.Scripts.Character;
using SnowIsland.Scripts.Inventory;
using UnityEngine;

namespace SnowIsland.Scripts.Item
{
    /*
     * 可使用的物品
     * 1. 手持食物,当使用的时候
     * - 动画机: 播放吃动画
     * 2. 当吃完(使用完成)的时候执行相关参数
     */
    
    /*
     * 
     * 能否使用,当放置物品的时候,需要检测
     * 放置物品的话我们需要
     * TrapPlacer.canPlace 能否放置=>CanUse
     * TrapPlacer.Location 放置的位置=>OnUsed
     */
    [CreateAssetMenu(fileName = "asset", menuName = "SnowIsland/Item/Usable", order = 0)]
    public class UsableItemAsset: ItemAsset,IItemAnimation
    { 
        [field: SerializeField,Header("可使用物品"),Tooltip("动画ID")]
        public int animationOnUse { get; private set; } 
        public virtual bool CanUse()
        {
            return true;
        }
        //进入播放使用动画
        public virtual void OnPrepareEnter(PlayerCharacter playerCharacter)
        {
            Debug.Log(playerCharacter.name+" Preparing Use "+name);
        } 
        //在使用动画播放完毕,仍然按着左键而进入的状态机
        public virtual void OnUseEnter(PlayerCharacter playerCharacter)
        {
            Debug.Log(playerCharacter.name+" Used "+name); 
        }

        public virtual void OnCancelEnter(PlayerCharacter playerCharacter)
        {
            Debug.Log(playerCharacter.name+" Cancelled Use "+name);
        }

        public int GetItemAnimation()
        {
            return animationOnUse;
        }
    }
}