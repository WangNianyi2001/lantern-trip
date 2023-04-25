Shader "Interactive Grass/GPU Grass" {
	Properties {
		[Header(Shading)]
		_MainTex             ("Base", 2D) = "white" {}
		_TopColor            ("Top Color", Color) = (1, 1, 1, 1)
		_BottomColor         ("Bottom Color", Color) = (1, 1, 1, 1)
		_TranslucentGain     ("Translucent Gain", Range(0, 1)) = 0.5
		[Space]
		[Header(Tessellation)]
		_Tessellation        ("Tessellation Strength", Range(1, 64)) = 16
		_TessellationMinDist ("Tessellation Min Distance", Float) = 1
		_TessellationMaxDist ("Tessellation Max Distance", Float) = 200
		[Header(Blades)]
		_BladeWidth          ("Blade Width", Float) = 0.05
		_BladeWidthRandom    ("Blade Width Random", Float) = 0.02
		_BladeHeight         ("Blade Height", Float) = 0.5
		_BladeHeightRandom   ("Blade Height Random", Float) = 0.3
		_BendRotationRandom  ("Bend Rotation Random", Range(0, 1)) = 0.2
		_BladeForward        ("Blade Forward Amount", Float) = 0.38
		[Header(Wind)]
		_WindDistortionMap   ("Wind Distortion Map", 2D) = "white" {}
		_WindStrength        ("Wind Strength", Float) = 1
		_WindFrequency       ("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
		[Header(Force)]
		_ForceCenter1        ("Force Center 1", Vector) = (0, 0, 0, 0)
		_ForceCenter2        ("Force Center 2", Vector) = (0, 0, 0, 0)
		_ForceCenter3        ("Force Center 3", Vector) = (0, 0, 0, 0)
		_ForceRange          ("Force Range", Float) = 2
		_ForceIntensity      ("Force Intensity", Float) = 2
		[Header(Trail)]
		[NoScaleOffset]_TrailTex ("Trail", 2D) = "bump" {}
		_TrailPress              ("TrailPress", Range(0, 4)) = 3
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Autolight.cginc"
	#include "Lighting.cginc"

	float _Tessellation, _TessellationMinDist, _TessellationMaxDist;
	float4 _TopColor, _BottomColor;
	float _TranslucentGain;
	float _BladeHeight, _BladeHeightRandom, _BladeWidthRandom, _BladeWidth, _BladeForward, _BendRotationRandom;
	sampler2D _WindDistortionMap, _MainTex, _TrailTex;
	float4 _WindDistortionMap_ST;
	float _WindStrength;
	float2 _WindFrequency;
	float4 _ForceCenter1, _ForceCenter2, _ForceCenter3;
	float _ForceRange, _ForceIntensity, _TrailPress;
	
	struct vertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float2 texcoord : TEXCOORD0;
	};
	struct vertexOutput
	{
		float4 vertex : SV_POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float2 texcoord : TEXCOORD0;
	};
	struct geometryOutput
	{
		float4 pos : SV_POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
		unityShadowCoord4 _ShadowCoord : TEXCOORD1;
		float2 uvOrig : TEXCOORD2;
		UNITY_FOG_COORDS(3)
	};

	// tessellation part of shader ///////////////////////////////////////////////////////////////////////////////////////////////////////////
	struct TessellationFactors 
	{
		float edge[3] : SV_TessFactor;
		float inside : SV_InsideTessFactor;
	};
	float TessellationEdgeFactor (vertexInput cp0, vertexInput cp1)
	{
		float3 p0 = mul(unity_ObjectToWorld, float4(cp0.vertex.xyz, 1.0)).xyz;
		float3 p1 = mul(unity_ObjectToWorld, float4(cp1.vertex.xyz, 1.0)).xyz;

		float3 center = (p0 + p1) * 0.5;
		float dist = distance(center, _WorldSpaceCameraPos);
		return clamp(1.0 - (dist - _TessellationMinDist) / (_TessellationMaxDist - _TessellationMinDist), 0.0, 1.0) * _Tessellation;
	}
	TessellationFactors patchConstantFunction (InputPatch<vertexInput, 3> patch)
	{
		TessellationFactors f;
#if ENABLE_DIST_LOD
		float f12 = TessellationEdgeFactor(patch[1], patch[2]);
		float f20 = TessellationEdgeFactor(patch[2], patch[0]);
		float f01 = TessellationEdgeFactor(patch[0], patch[1]);
		f.edge[0] = f12;
		f.edge[1] = f20;
		f.edge[2] = f01;
		f.inside = (f12 + f20 + f01) * (1.0 / 3.0);
#else
		f.edge[0] = _Tessellation;
		f.edge[1] = _Tessellation;
		f.edge[2] = _Tessellation;
		f.inside = _Tessellation;
#endif
		return f;
	}
	[UNITY_domain("tri")]
	[UNITY_outputcontrolpoints(3)]
	[UNITY_outputtopology("triangle_cw")]
	[UNITY_partitioning("integer")]
	[UNITY_patchconstantfunc("patchConstantFunction")]
	vertexInput hull (InputPatch<vertexInput, 3> patch, uint id : SV_OutputControlPointID)
	{
		return patch[id];
	}
	[UNITY_domain("tri")]
	vertexOutput domain (TessellationFactors factors, OutputPatch<vertexInput, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
	{
		vertexInput v;

		#define GRASS_DOMAIN_INTERPOLATE(fieldName) v.fieldName = \
			patch[0].fieldName * barycentricCoordinates.x + \
			patch[1].fieldName * barycentricCoordinates.y + \
			patch[2].fieldName * barycentricCoordinates.z;

		GRASS_DOMAIN_INTERPOLATE(vertex)
		GRASS_DOMAIN_INTERPOLATE(normal)
		GRASS_DOMAIN_INTERPOLATE(tangent)
		GRASS_DOMAIN_INTERPOLATE(texcoord)

		vertexOutput o;
		o.vertex = v.vertex;
		o.normal = v.normal;
		o.tangent = v.tangent;
		o.texcoord = v.texcoord;
		return o;
	}
	// tessellation part of shader ///////////////////////////////////////////////////////////////////////////////////////////////////////////
	float rand (float3 v) { return frac(sin(dot(v.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453); }
	float3x3 AngleAxis3x3 (float angle, float3 axis)
	{
		float c, s;
		sincos(angle, s, c);
		float t = 1 - c;
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;
		return float3x3(
			t * x * x + c, t * x * y - s * z, t * x * z + s * y,
			t * x * y + s * z, t * y * y + c, t * y * z - s * x,
			t * x * z - s * y, t * y * z + s * x, t * z * z + c);
	}
	float2 InteractiveForce(float3 forceCenter, float3 vertPos)
	{
		float3 center = forceCenter;
		float3 wldpos = mul(unity_ObjectToWorld, float4(vertPos, 1.0)).xyz;
		float3 dist = distance(center, wldpos);
		float3 circle = 1.0 - saturate(dist / _ForceRange);
		float3 disp = (wldpos - center) * circle;
		return disp.xz * _ForceIntensity;
	}
	vertexInput vert (vertexInput v) { return v; }
	[maxvertexcount(3)]
	void geo (point vertexOutput IN[1], inout TriangleStream<geometryOutput> triStream)
	{
		float3 pos = IN[0].vertex.xyz;

		// build tangent to local matrix
		float3 nor = IN[0].normal;
		float4 tgt = IN[0].tangent;
		float3 bin = cross(nor, tgt) * tgt.w;
		float3x3 t2l = float3x3(
			tgt.x, bin.x, nor.x,
			tgt.y, bin.y, nor.y,
			tgt.z, bin.z, nor.z);

		// config size
		float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
		float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
		float forward = rand(pos.yyz) * _BladeForward;

		// wind animation
		float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
		float2 s = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
		float3 wind = normalize(float3(s.x, s.y, 0));
		float3x3 windMat = AngleAxis3x3(UNITY_PI * s, wind);

		// transform
		float3x3 m1 = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
		float3x3 m2 = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
		float3x3 mCompAll = mul(mul(mul(t2l, windMat), m1), m2);
		float3x3 mFacing = mul(t2l, m1);

		// position after above calculation
		float3 pos1 = pos + mul(mFacing, float3(width, 0, 0));
		float3 pos2 = pos + mul(mFacing, float3(-width, 0, 0));
		float3 pos3 = pos + mul(mCompAll, float3(0, 0, height));

#if ENABLE_TRAIL
		half4 trail = tex2Dlod(_TrailTex, float4(IN[0].texcoord, 0, 0));
		trail.xy = (trail.xy * 2.0) - 1.0;
		pos3.xz += (trail.xy * 2.0);
		pos3.y -= length(trail.xy * _TrailPress);
#else
		pos3.xz += InteractiveForce(_ForceCenter1.xyz, pos3);
		pos3.xz += InteractiveForce(_ForceCenter2.xyz, pos3);
		pos3.xz += InteractiveForce(_ForceCenter3.xyz, pos3);
#endif

		geometryOutput o;
		o.pos = UnityObjectToClipPos(pos1);
		o.uv = float2(0, 0);
		o.normal = UnityObjectToWorldNormal(mul(mFacing, normalize(float3(0, -1, forward))));
		o._ShadowCoord = ComputeScreenPos(o.pos);
		o.uvOrig = IN[0].texcoord;
		UNITY_TRANSFER_FOG(o, o.pos);
		triStream.Append(o);

		o.pos = UnityObjectToClipPos(pos2);
		o.uv = float2(1, 0);
		o.normal = UnityObjectToWorldNormal(mul(mFacing, normalize(float3(0, -1, forward))));
		o._ShadowCoord = ComputeScreenPos(o.pos);
		o.uvOrig = IN[0].texcoord;
		UNITY_TRANSFER_FOG(o, o.pos);
		triStream.Append(o);

		o.pos = UnityObjectToClipPos(pos3);
		o.uv = float2(0, 1);
		o.normal = UnityObjectToWorldNormal(mul(mCompAll, normalize(float3(0, -1, forward))));
		o._ShadowCoord = ComputeScreenPos(o.pos);
		o.uvOrig = IN[0].texcoord;
		UNITY_TRANSFER_FOG(o, o.pos);
		triStream.Append(o);
	}
	ENDCG
	SubShader {
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
		Cull Off
		Pass {
			Tags{ "LightMode" = "UniversalForward" }
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile _ ENABLE_TRAIL
			#pragma multi_compile _ ENABLE_DIST_LOD
			float4 frag (geometryOutput input,  fixed facing : VFACE) : SV_Target
			{
				float3 nor = facing > 0 ? input.normal : -input.normal;
				float shadow = SHADOW_ATTENUATION(input);
				float ndl = saturate(saturate(dot(nor, _WorldSpaceLightPos0)) + _TranslucentGain) * shadow;
				float4 lit = ndl * _LightColor0 + float4(ShadeSH9(float4(nor, 1.0)), 1.0);
				float4 albedo = tex2D(_MainTex, input.uv);
				half4 c = lerp(albedo * _BottomColor, albedo * _TopColor * lit, input.uv.y);
				UNITY_APPLY_FOG(input.fogCoord, c);
				return c;
			}
			ENDCG
		}
		Pass {
			Tags { "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile _ ENABLE_DIST_LOD
			float4 frag (geometryOutput input) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(input)
			}
			ENDCG
		}
	}
	Fallback Off
}
