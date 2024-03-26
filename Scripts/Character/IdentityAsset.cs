using UnityEngine;

namespace SnowIsland.Scripts.Character
{
    
    public enum Identity{
        Scout,//侦察兵
        Doctor,//医生
        Imposter_Detector,
        Imposter_TeamLeader,
    }
    [CreateAssetMenu(fileName = "asset", menuName = "SnowIsland/Identity", order = 0)]
    public class IdentityAsset : ScriptableObject
    {
        [field:SerializeField]
        public Sprite Icon { get; private set; }
        [field: SerializeField] 
        public bool DefaultImposter { get; private set; } = false;
        [field:SerializeField]
        public Identity Identity { get; private set; }
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public string Introduction { get; private set; }
        [field: SerializeField,Multiline(3)]
        public string Description { get; private set; }
        [field: SerializeField]
        public float SkillCooldown { get; private set; }
    }
}