#ifndef MODEL_GRASS_SHADOW_CASTER
#define MODEL_GRASS_SHADOW_CASTER

struct ShadowPassAttributes
{
	float4 positionOS  : POSITION;
	float2 uv          : TEXCOORD0;
	half4  color       : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct ShadowPassVaryings
{
	float2 uv          : TEXCOORD0;
	float4 positionCS  : SV_POSITION;
};
struct CustomSurfaceData
{
	half3 diffuse;
};

void SurfaceFunction (ShadowPassVaryings IN, out CustomSurfaceData surfaceData);

ShadowPassVaryings ShadowPassVertex (ShadowPassAttributes IN)
{
	float4 positionOS = VertexAnim(IN.positionOS, IN.color.r);
	VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS.xyz);

	ShadowPassVaryings OUT;
	OUT.uv = IN.uv;
	OUT.positionCS = vertexInput.positionCS;
	return OUT;
}

half4 ShadowPassFragment (ShadowPassVaryings IN) : SV_Target
{
	CustomSurfaceData surfaceData;
	SurfaceFunction(IN, surfaceData);
	return half4(surfaceData.diffuse, 1.0);
}

#endif