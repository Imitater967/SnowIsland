using System;
using UnityEngine;

namespace SnowIsland.Scripts.Ground{
public class SnowGround : MonoBehaviour
{
    public Camera renderCamera;
    public Material material;
    public MeshRenderer lowPoly;
    public MeshRenderer highPoly;
    [SerializeField, ReadOnly] 
    private MeshRenderer current;
    [SerializeField, ReadOnly] 
    private RenderTexture renderTexture;

    private static readonly int _trackTexture = Shader.PropertyToID("_TrackTexture");
    private int rtResolution = 256; 
    private void ChoseModel()
    {
        MeshRenderer other;
#if UNITY_EDITOR || UNITY_STANDALONE
        current = highPoly;
        other = lowPoly;
        rtResolution = 512;
#elif UNITY_ANDROID
        current = lowPoly;
        other=highPoly;
        rtResolution = 256;
#endif
        current.gameObject.SetActive(true);
        other.gameObject.SetActive(false);
    }

    private void GenerateTextureAndMaterial()
    {
        material = Instantiate(material);
        renderTexture = new RenderTexture(rtResolution, rtResolution, 8, RenderTextureFormat.Default);
        renderCamera.targetTexture = renderTexture;
        material.SetTexture(_trackTexture,renderTexture);
        current.material = material;
    }
    private void Awake()
    {
        ChoseModel();
        GenerateTextureAndMaterial();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}