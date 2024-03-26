Shader "Custom/Character Placement"
{
    Properties
    {
        _Color("Color",color) = (1,1,1,1)  
        _ZTestAddValue("ZTest Add Value",Range(0,1))=0
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
        ZTest Always
        ZWrite On 
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
 
 
            struct Attributes
            {
                float4 positionOS : POSITION; 
                float4 normalOS : NORMAL;
            };
 
            struct Varyings
            {
                float4 positionCS : SV_POSITION;  
            };
 
            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            float _ZTestAddValue;
            CBUFFER_END 
            
 
            Varyings vert(Attributes IN)
            {
                 Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz); 
                float4 clipPos= positionInputs.positionCS;
                // #if UNITY_REVERSED_Z
                // clipPos.z+=_ZTestAddValue;
                // clipPos=min(clipPos.z,clipPos.w*UNITY_NEAR_CLIP_VALUE);
                // #else
                // clipPos.-+=_ZTestAddValue;
                // clipPos=max(clipPos.z,clipPos.w*UNITY_NEAR_CLIP_VALUE);
                // #endif
                OUT.positionCS=clipPos;
                return OUT;
            }
 
            half4 frag(Varyings i) : SV_Target
            {  
                return _Color;
            }
            ENDHLSL
        }
    }
}