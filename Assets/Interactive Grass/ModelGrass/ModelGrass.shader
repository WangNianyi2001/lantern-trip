Shader "Interactive Grass/Model Grass" {
	Properties {
		[Header(Surface)][Space(5)]
		_BaseColor                            ("Base Color", Color) = (1, 1, 1, 1)
		_BaseMap                              ("Base Map", 2D) = "white" {}
		[Toggle(_NORMALMAP)] _EnableNormalMap ("Enable Normal Map", Float) = 0
		[Normal][NoScaleOffset]_NormalMap     ("Normal Map", 2D) = "bump" {}
		_NormalMapScale                       ("Normal Map Scale", Float) = 1
		[HDR]_Emission                        ("Emission Color", Color) = (0,0,0,1)
		_Cutoff                               ("Cutoff", Range(0, 1)) = 0.5
		[Header(Animation)][Space(5)]
		_Amplitude     ("Amplitude", Float) = 0.2
		_WaveX         ("Waves X", Float) = 1
		_WaveY         ("Waves Y", Float) = 1
		_TimeScale     ("Time Scale", Float) = 1
		_MoveVec       ("Move Vec", Vector) = (0, 0, 0, 0)
		[Header(Burn)][Space(5)]
		_BurnAmount    ("Amount", Range(0.0, 1.0)) = 0.0
		_BurnLineWidth ("Line Width", Range(0.0, 0.2)) = 0.1
		_BurnColor1    ("First Color", Color) = (1, 0, 0, 1)
		_BurnColor2    ("Second Color", Color) = (1, 0, 0, 1)
		_BurnMap       ("Burn", 2D) = "white" {}
		[Header(Shadow)][Space(5)]
		[Enum(UnityEngine.Rendering.CullMode)]  _Cull ("Cull", Float) = 2
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True" }
		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		CBUFFER_START(UnityPerMaterial)
			float4 _BaseMap_ST, _BurnMap_ST;
			half4 _BaseColor, _BurnColor1, _BurnColor2, _Emission;
			half _NormalMapScale, _Cutoff;
			half _BurnAmount, _BurnLineWidth;

			// vertex animation
			float _Amplitude, _WaveX, _WaveY, _TimeScale;
			float3 _MoveVec;
			float4 VertexAnim (float4 pos, float weight)
			{
				float4 p = pos;
				p.x += _Amplitude * sin(p.x * _WaveX + _Time.y * _TimeScale + p.z * _WaveY) * weight + _MoveVec.x * weight;
				p.z += _Amplitude * sin(p.x * _WaveX + _Time.y * _TimeScale + p.z * _WaveY) * weight + _MoveVec.z * weight;
				return p;
			}

			TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
			TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
			TEXTURE2D(_BurnMap); SAMPLER(sampler_BurnMap);
		CBUFFER_END
		ENDHLSL

		Pass {
			Tags { "LightMode" = "UniversalForward" }
			HLSLPROGRAM
			#pragma vertex SurfaceVertex
			#pragma fragment SurfaceFragment
			#include "ModelGrass.hlsl"

			#pragma shader_feature _NORMALMAP
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			void SurfaceFunction (Varyings IN, out CustomSurfaceData surfaceData)
			{
				float2 uv1 = TRANSFORM_TEX(IN.uv, _BaseMap);
				half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv1) * _BaseColor;
				clip(baseColor.a - _Cutoff);

				float2 uv2 = TRANSFORM_TEX(IN.uv, _BurnMap);
				half4 burn = SAMPLE_TEXTURE2D(_BurnMap, sampler_BurnMap, uv2);
				clip(burn.r - _BurnAmount);

				half t = 1.0 - smoothstep(0.0, _BurnLineWidth, burn.r - _BurnAmount);
				half3 burnColor = lerp(_BurnColor1.rgb, _BurnColor2.rgb, t);
				baseColor.rgb = lerp(baseColor.rgb, burnColor, t * step(0.0001, _BurnAmount));

				surfaceData = (CustomSurfaceData)0.0;
				surfaceData.diffuse = baseColor.rgb;
#ifdef _NORMALMAP
				surfaceData.normalWS = GetPerPixelNormalScaled(TEXTURE2D_ARGS(_NormalMap, sampler_NormalMap), uv1, IN.normalWS, IN.tangentWS, _NormalMapScale);
#else
				surfaceData.normalWS = normalize(IN.normalWS);
#endif
				surfaceData.emission = _Emission.rgb;
				surfaceData.alpha = 1.0;
			}
			ENDHLSL
		}
		Pass {
			Tags { "LightMode" = "ShadowCaster" }
			ZWrite On ZTest LEqual Cull [_Cull]

			HLSLPROGRAM
			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment
			#include "ModelGrassShadowCaster.hlsl"

			void SurfaceFunction (ShadowPassVaryings IN, out CustomSurfaceData surfaceData)
			{
				float2 uv1 = TRANSFORM_TEX(IN.uv, _BaseMap);
				half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv1) * _BaseColor;
				clip(baseColor.a - _Cutoff);

				float2 uv2 = TRANSFORM_TEX(IN.uv, _BurnMap);
				half4 burn = SAMPLE_TEXTURE2D(_BurnMap, sampler_BurnMap, uv2);
				clip(burn.r - _BurnAmount);

				surfaceData = (CustomSurfaceData)0.0;
				surfaceData.diffuse = baseColor.rgb;
			}
			ENDHLSL
		}
		UsePass "Universal Render Pipeline/Lit/DepthOnly"
		UsePass "Universal Render Pipeline/Lit/Meta"
	}
	FallBack Off
}
