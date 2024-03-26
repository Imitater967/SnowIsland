using SnowIsland.Scripts.UI.Game;
using Unity.Netcode;
using UnityEngine;

namespace SnowIsland.Scripts.Chest
{
    public class PlayerCorpse : NetworkBehaviour,ICorpse
    {
        public GameObject nameDisplay;
        public string name;
        public Vector3 offset = new Vector3(0, 2, 0);
        [SerializeField] private UIPlayerCorpse nameDisplayPrefab;
        [SerializeField, Scripts.ReadOnly] private UIPlayerCorpse nameDisplayInstance;

        private void OnEnable()
        {
            nameDisplayInstance = Instantiate(nameDisplayPrefab.gameObject,GameObject.Find("Canvas/NameDisplays").transform).GetComponent<UIPlayerCorpse>();
            nameDisplayInstance.playerName.text = name;

        }

        private void OnDestroy()
        {
            if(nameDisplayInstance!=null)
                Destroy(nameDisplayInstance.gameObject);
        }

        private void Update()
        {
            nameDisplayInstance.gameObject.transform.position =
                RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + offset);
        }
        [ServerRpc(RequireOwnership = false)]
        public void DestroyServerRpc( )
        { 
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}