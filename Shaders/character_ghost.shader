Shader "Custom/Character Ghost"
{
    Properties
    {
        _RimColor("Rim Color",color) = (1,1,1,1) 
        _RimPower("Rim Power",Range(0,1))=1
        }
 
    SubShader
    {
        Tags { "Queue"="Transparent"
               "RenderType"="Opaque"
               "IgnoreProjector" = "True" 
               "RenderPipeline" = "UniversalPipeline"
               "ForceNoShadowCasting" = "true" 
            }
        
        LOD 100
          //开启混合
        Blend SrcAlpha One 
   
        Pass
        {   
            Name "Unlit"
            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
 
 
            struct Attributes
            {
                float4 positionOS : POSITION; 
                float4 normalOS : NORMAL;
            };
 
            struct Varyings
            {
                float4 positionCS : SV_POSITION; 
                float3 positionWS: TEXCOORD0;
                float3 viewDirWS: TEXCOORD1;
                float3 normalWS: TEXCOORD3;
            };
 
            CBUFFER_START(UnityPerMaterial)
            half4 _RimColor;
            half _RimPower;
            CBUFFER_END 
            
 
            Varyings vert(Attributes IN)
            {
                 Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                //单位化向量,使其不受距离控制
                OUT.viewDirWS = normalize(GetCameraPositionWS() - positionInputs.positionWS);
                OUT.normalWS = normalize(normalInputs.normalWS);
                return OUT;
            }
 
            half4 frag(Varyings i) : SV_Target
            { 
                float rim=1-saturate(dot(i.normalWS,i.viewDirWS));
                float4 rimColor=_RimColor*pow(rim,1/_RimPower); 
                return rimColor;
            }
            ENDHLSL
        }
    }
}