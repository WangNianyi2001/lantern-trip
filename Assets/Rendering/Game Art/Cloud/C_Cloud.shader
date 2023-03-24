Shader "Custom/C_Cloud"
{
    Properties
    {
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,1,1,1)
        _MainTex ("BaseTex", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags
        {
            "RenderPipiline"="UniversalRenderPipeline"
            "RenderType"="Opaque"
        }

        Pass{
        
        HLSLPROGRAM
        
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

        float4 _Color1;
        float4 _Color2;
        CBUFFER_START(UnityPerMaterial)
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        CBUFFER_END

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float4 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
        };
        struct Varyings
        {
            float4 positionCS : POSITION;
            float2 uv : TEXCOORD0;
            float3 positionWS : TEXCOORD1;
            float3 normalWS : TEXCOORD2;
            float4 screenPos : TEXCOORD3;
            float3 tangentWS : TEXCOORD4;
            float3 bitangentWS : TEXCOORD5;
        };

        float computeCloudSize(float4 lerpTex, float4 cloudTex,half _LerpCtrl)
        {   
            half cloudStep = 1- _LerpCtrl;
            half cloudLerp = smoothstep(0.95,1,_LerpCtrl);
            half a1 = smoothstep(saturate(cloudStep-0.1),cloudStep,lerpTex.r);  
        
            return lerp(a1,cloudTex.a ,cloudLerp);
        }

        Varyings vert(Attributes v)
        {
            Varyings o;
            o.uv = v.uv;
            o.positionWS = TransformObjectToWorld(v.positionOS);
            o.positionCS = TransformWorldToHClip(o.positionWS);
            o.normalWS = TransformObjectToWorldNormal(v.normalOS);
            o.screenPos = ComputeScreenPos(o.positionCS);

            o.tangentWS = normalize(mul(unity_ObjectToWorld, float4(v.tangentOS.xyz, 0.0)).xyz);
            o.bitangentWS = normalize(cross(o.normalWS, o.tangentWS) * v.tangentOS.w);

            return o;
        }

        float4 frag(Varyings i) : SV_TARGET
        {
            //R:melt
            //G:edge mask
            //B:color
            //A:mask
            float4 MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            
            
            float4 color = lerp(_Color1, _Color2, MainTex.z);
            return float4(1, 1, 1, MainTex.w);
        }
        
        
        ENDHLSL
        }
    }
    FallBack "Diffuse"
}
