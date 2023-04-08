Shader "Custom/Cloud Parallax" 
{
	Properties {
		_Color("Color",Color) = (1,1,1,1)
		_MainTex("MainTex",2D)="white"{}
		_Alpha("Alpha", Range(0,1)) = 0.5
		_Height("Displacement Amount",range(0,1)) = 0.15
		_HeightAmount("Turbulence Amount",range(0,2)) = 1
		_HeightTileSpeed("Turbulence Tile&Speed",Vector) = (1.0,1.0,0.05,0.0)
		_LightIntensity ("Ambient Intensity", Range(0,3)) = 1.0
		[Toggle] _UseFixedLight("Use Fixed Light", Int) = 1
		_FixedLightDir("Fixed Light Direction", Vector) = (0.981, 0.122, -0.148, 0.0)

	}

	SubShader 
	{
		LOD 300		
        Tags 
		{
            "IgnoreProjector"="True"
            "Queue"="Transparent-50"
            "RenderType"="Transparent"
			"RenderPipeline" = "UniversalPipeline"
        }

		Pass
		{
		    Name "FORWARD"
            Tags 
			{
                "LightMode"="UniversalForward"
            }
			Blend SrcAlpha OneMinusSrcAlpha
			//ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

			
			 // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _Height;
			float4 _HeightTileSpeed;
			half _HeightAmount;
			half4 _Color;
			half _Alpha;
			half _LightIntensity;

			half4 _LightingColor;
			half4 _FixedLightDir;
			half _UseFixedLight;

			struct Attributes
			{
			    float4 vertex : POSITION;
			    float4 tangent : TANGENT;
			    float3 normal : NORMAL;
			    float4 texcoord : TEXCOORD0;//第一纹理坐标
			    half4 color : COLOR;//顶点颜色
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct Varyings
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float4 posWorld : TEXCOORD3;
				float2 uv2 : TEXCOORD4;
				float4 color : TEXCOORD5;
				float fogCoord : TEXCOORD6;
			};

			Varyings vert (Attributes v) 
			{
				Varyings o;
				o.pos = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex) + frac(_Time.y*_HeightTileSpeed.zw);
				o.uv2 = v.texcoord * _HeightTileSpeed.xy;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalDir = TransformObjectToWorldNormal(v.normal);
				
				//TANGENT_SPACE_ROTATION;
				float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
				float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
				
				//o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
				o.viewDir=mul(rotation,TransformWorldToObject(GetCameraPositionWS()) - v.vertex);
				
				o.color = v.color;
				o.fogCoord = ComputeFogFactor(o.pos.z);
				return o;
			}

			float4 frag(Varyings i) : COLOR
			{
				float3 viewRay=normalize(i.viewDir*-1);
				viewRay.z=abs(viewRay.z)+0.2;
				viewRay.xy *= _Height;

				float3 shadeP = float3(i.uv,0);
				float3 shadeP2 = float3(i.uv2,0);


				float linearStep = 16;

				float4 T = tex2D(_MainTex, shadeP2.xy);
				float h2 = T.a * _HeightAmount;

				float3 lioffset = viewRay / (viewRay.z * linearStep);
				float d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy,0,0)).a * h2;
				float3 prev_d = d;
				float3 prev_shadeP = shadeP;
				while(d > shadeP.z)
				{
					prev_shadeP = shadeP;
					shadeP += lioffset;
					prev_d = d;
					d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy,0,0)).a * h2;
				}
				float d1 = d - shadeP.z;
				float d2 = prev_d - prev_shadeP.z;
				float w = d1 / (d1 - d2);
				shadeP = lerp(shadeP, prev_shadeP, w);

				half4 c = tex2D(_MainTex,shadeP.xy) * T * _Color;
				half Alpha = lerp(c.a, 1.0, _Alpha) * i.color.r;

				float3 normal = normalize(i.normalDir);
				half3 lightDir1 = normalize(_FixedLightDir.xyz);
				half3 lightDir2=_MainLightPosition.xyz - i.posWorld;
				//half3 lightDir2 = UnityWorldSpaceLightDir(i.posWorld);
				half3 lightDir = lerp(lightDir2, lightDir1, _UseFixedLight);
				float NdotL = max(0,dot(normal,lightDir));
				half3 lightColor = _MainLightColor.rgb;
                half3 finalColor = c.rgb*(NdotL*lightColor + 1.0);
                return half4(finalColor.rgb,Alpha);
			}
		ENDHLSL
		}
	}
	
	SubShader 
	{
		LOD 200		
        Tags 
		{
            "IgnoreProjector"="True"
            "Queue"="Transparent-50"
            "RenderType"="Transparent"
        }

		Pass
		{
		    Name "FORWARD"
            Tags 
			{
                "LightMode"="UniversalForward"
            }
			Blend SrcAlpha OneMinusSrcAlpha
			//ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

			
			 // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _Height;
			float4 _HeightTileSpeed;
			half _HeightAmount;
			half4 _Color;
			half _LightIntensity;
			half _Alpha;

			half _DirectLightAmount;
			half4 _LightingColor;
			half4 _FixedLightDir;
			half _UseFixedLight;
			struct Attributes
			{
			    float4 vertex : POSITION;
			    float4 tangent : TANGENT;
			    float3 normal : NORMAL;
			    float4 texcoord : TEXCOORD0;//第一纹理坐标
			    half4 color : COLOR;//顶点颜色
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct Varyings
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float4 posWorld : TEXCOORD3;
				float2 uv2 : TEXCOORD4;
				float4 color : TEXCOORD5;
				float fogCoord : TEXCOORD6;
			};
			Varyings vert (Attributes v) 
			{
				Varyings o;
				o.pos = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex) + frac(_Time.y*_HeightTileSpeed.zw);
				o.uv2 = v.texcoord * _HeightTileSpeed.xy;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalDir = TransformObjectToWorldNormal(v.normal);
				
				//TANGENT_SPACE_ROTATION;
				float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
				float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
				
				//o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
				o.viewDir=mul(rotation,TransformWorldToObject(GetCameraPositionWS()) - v.vertex);
				
				o.color = v.color;
				o.fogCoord = ComputeFogFactor(o.pos.z);
				return o;
			}

			float4 frag(Varyings i) : COLOR
			{
				float3 viewRay=normalize(i.viewDir*-1);
				viewRay.z=abs(viewRay.z)+0.42;
				viewRay.xy *= _Height;

				float3 shadeP = float3(i.uv,0);
				float3 shadeP2 = float3(i.uv2,0);

				float4 T = tex2D(_MainTex,shadeP2.xy);
				float h2 = T.a * _HeightAmount;

				float3 sioffset = viewRay / viewRay.z;
				float d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy,0,0)).a * h2;
				shadeP += sioffset * d;

				half4 c = tex2D(_MainTex,shadeP.xy) * T * _Color;
				half Alpha = lerp(c.a, 1.0, _Alpha) * i.color.r;

				float3 normal = normalize(i.normalDir);
				half3 lightDir1 = normalize(_FixedLightDir.xyz);
				half3 lightDir2 = _MainLightPosition.xyz-i.posWorld;
				half3 lightDir = lerp(lightDir2, lightDir1, _UseFixedLight);
				float NdotL = max(0,dot(normal,lightDir));

				half initFactor = step(0.1, _LightingColor.a);
				_DirectLightAmount = lerp(1.0, _DirectLightAmount, initFactor);
				half3 lightColor = _MainLightColor.rgb * _DirectLightAmount;

                half3 finalColor = c.rgb*(NdotL*lightColor + unity_AmbientEquator.rgb);
				//UNITY_APPLY_FOG(i.fogCoord, finalColor);
				finalColor = MixFog(finalColor, i.fogCoord);
				return half4(finalColor.rgb,Alpha);
			}
		ENDHLSL
		}
	}
	FallBack "Diffuse"
}
