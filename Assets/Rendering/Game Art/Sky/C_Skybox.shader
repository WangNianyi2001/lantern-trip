Shader "Custom/Skybox"
{
    Properties
    {
		[Header(Base Settings)]
		_SunSetDuration ("Sun Set Duration", Range(0, 1)) = 0.25
		_SkyboxBias ("Skybox Bias", Range(-1, 1)) = -0.25
		_Brightness ("Brightness", Range(0.1, 100)) = 1
		_SkyColorSaturate ("Sky Color Saturate", Range(0, 1)) = 1
		_SunAndMoonColorSaturate ("Sun And Moon Color Saturate", Range(0, 1)) = 1
		[ToggleOff] _EnableSolidGroundLineColor ("Enable Solid Ground Line Color", int) = 1
		_GroundColor ("Ground Color", Color) = (0, 0, 0)
		_GroundLineScatter ("Ground Line Scatter", Range(0, 1)) = 1
		_ReduceSunIntensity ("Reduce Sun Intensity", Range(0, 1)) = 0
		_ReduceMoonIntensity ("Reduce Moon Intensity", Range(0, 1)) = 0

		[Header(Midnight)]
        [HDR] _HigherSkyColor1 ("Higher Sky Color", Color) = (0, 0.012, 0.024)
		[HDR] _LowerSkyColor1 ("Lower Sky Color", Color) = (0, 0.016, 0.027)
		[HDR] _HigherHorizonColor1 ("Higher Horizon Color", Color) = (0, 0.020, 0.047)
		[HDR] _LowerHorizonColor1 ("Lower Horizon Color", Color) = (0, 0.047, 0.098)
		_SkyColorBoundary1 ("Sky Color Boundary", Range(-1, 1)) = 0.25
		_SkyColorRamp1 ("Sky Color Ramp", Range(0, 1)) = 1
		_HorizonColorBoundary1 ("Horizon Color Boundary", Range(-1, 1)) = -1
		_HorizonColorRamp1 ("Horizon Color Ramp", Range(0, 1)) = 1
		_SkyHorizonColorBoundary1 ("Sky Horizon Color Boundary", Range(-1, 1)) = -0.5
		_SkyHorizonColorRamp1 ("Sky Horizon Color Ramp", Range(0, 1)) = 0.5
		[HDR] _SunColor1 ("Sun Color", Color) = (0, 0, 0)
		_SunRadius1 ("Sun Radius", Range(0, 1)) = 0.2
		_SunScatter1 ("Sun Scatter", Range(0, 1)) = 0
		[HDR] _MoonColor1 ("Moon Color", Color) = (3.154, 12.549, 12.549)
		_MoonRadius1 ("Moon Radius", Range(0, 1)) = 0.15
		_MoonScatter1 ("Moon Scatter", Range(0, 1)) = 1

		[Header(Early Night And Late Night)]
        [HDR] _HigherSkyColor2 ("Higher Sky Color", Color) = (0.008, 0.020, 0.031)
		[HDR] _LowerSkyColor2 ("Lower Sky Color", Color) = (0.016, 0.024, 0.071)
		[HDR] _HigherHorizonColor2 ("Higher Horizon Color", Color) = (0.2, 0.059, 0.077)
		[HDR] _LowerHorizonColor2 ("Lower Horizon Color", Color) = (0.236, 0.108, 0.046)
		_SkyColorBoundary2 ("Sky Color Boundary", Range(-1, 1)) = 0.25
		_SkyColorRamp2 ("Sky Color Ramp", Range(0, 1)) = 1
		_HorizonColorBoundary2 ("Horizon Color Boundary", Range(-1, 1)) = -1
		_HorizonColorRamp2 ("Horizon Color Ramp", Range(0, 1)) = 1
		_SkyHorizonColorBoundary2 ("Sky Horizon Color Boundary", Range(-1, 1)) = -0.9
		_SkyHorizonColorRamp2 ("Sky Horizon Color Ramp", Range(0, 1)) = 0.5
		[HDR] _SunColor2 ("Sun Color", Color) = (8, 1.851, 0)
		_SunRadius2 ("Sun Radius", Range(0, 1)) = 0.2
		_SunScatter2 ("Sun Scatter", Range(0, 1)) = 0.5
		[HDR] _MoonColor2 ("Moon Color", Color) = (4, 4, 4)
		_MoonRadius2 ("Moon Radius", Range(0, 1)) = 0.15
		_MoonScatter2 ("Moon Scatter", Range(0, 1)) = 0.5

		[Header(Dawn And Sunset)]
        [HDR] _HigherSkyColor3 ("Higher Sky Color", Color) = (0.088, 0.214, 0.453)
		[HDR] _LowerSkyColor3 ("Lower Sky Color", Color) = (0, 0.406, 0.249)
		[HDR] _HigherHorizonColor3 ("Higher Horizon Color", Color) = (1, 0.556, 0.052)
		[HDR] _LowerHorizonColor3 ("Lower Horizon Color", Color) = (1, 0.249, 0.051)
		_SkyColorBoundary3 ("Sky Color Boundary", Range(-1, 1)) = 0.25
		_SkyColorRamp3 ("Sky Color Ramp", Range(0, 1)) = 1
		_HorizonColorBoundary3 ("Horizon Color Boundary", Range(-1, 1)) = -1
		_HorizonColorRamp3 ("Horizon Color Ramp", Range(0, 1)) = 0.4
		_SkyHorizonColorBoundary3 ("Sky Horizon Color Boundary", Range(-1, 1)) = -1
		_SkyHorizonColorRamp3 ("Sky Horizon Color Ramp", Range(0, 1)) = 0.5
		[HDR] _SunColor3 ("Sun Color", Color) = (181.019, 61.760, 0)
		_SunRadius3 ("Sun Radius", Range(0, 1)) = 0.2
		_SunScatter3 ("Sun Scatter", Range(0, 1)) = 1
		[HDR] _MoonColor3 ("Moon Color", Color) = (1, 1, 1)
		_MoonRadius3 ("Moon Radius", Range(0, 1)) = 0.15
		_MoonScatter3 ("Moon Scatter", Range(0, 1)) = 0

		[Header(Noon)]
        [HDR] _HigherSkyColor4 ("Higher Sky Color", Color) = (0.098, 0.361, 1)
		[HDR] _LowerSkyColor4 ("Lower Sky Color", Color) = (0, 0.537, 1)
		[HDR] _HigherHorizonColor4 ("Higher Horizon Color", Color) = (0.392, 0.957, 1)
		[HDR] _LowerHorizonColor4 ("Lower Horizon Color", Color) = (0.816, 0.961, 1)
		_SkyColorBoundary4 ("Sky Color Boundary", Range(-1, 1)) = 0.25
		_SkyColorRamp4 ("Sky Color Ramp", Range(0, 1)) = 1
		_HorizonColorBoundary4 ("Horizon Color Boundary", Range(-1, 1)) = -1
		_HorizonColorRamp4 ("Horizon Color Ramp", Range(0, 1)) = 0.4
		_SkyHorizonColorBoundary4 ("Sky Horizon Color Boundary", Range(-1, 1)) = -1
		_SkyHorizonColorRamp4 ("Sky Horizon Color Ramp", Range(0, 1)) = 0.75
		[HDR] _SunColor4 ("Sun Color", Color) = (85.445, 75.156, 0)
		_SunRadius4 ("Sun Radius", Range(0, 1)) = 0.2
		_SunScatter4 ("Sun Scatter", Range(0, 1)) = 0.5
		[HDR] _MoonColor4 ("Moon Color", Color) = (0, 0, 0)
		_MoonRadius4 ("Moon Radius", Range(0, 1)) = 0.15
		_MoonScatter4 ("Moon Scatter", Range(0, 1)) = 0

		[Header(Stars Settings)]
		_NoiseTexture ("Noise Texture", 2D) = "black"{}
        _StarsBrightness ("Stars Brightness", Range(0, 2)) = 1
        _StarsHeight ("Stars Height", Range(0, 1)) = 0
        _StarsScale ("Stars Scale", Range(0.5, 2)) = 0.5

    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType" = "Opaque"
        }
        Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _SunSetDuration;
			float _SkyboxBias;
			float _Brightness;
			float _SkyColorSaturate;
			float _SunAndMoonColorSaturate;
			int _EnableSolidGroundLineColor;
			float3 _GroundColor;
			float _GroundLineScatter;
			float _ReduceSunIntensity;
			float _ReduceMoonIntensity;

			float3 _HigherSkyColor1;
			float3 _LowerSkyColor1;
			float3 _HigherHorizonColor1;
			float3 _LowerHorizonColor1;
			float _SkyColorBoundary1;
			float _SkyColorRamp1;
			float _HorizonColorBoundary1;
			float _HorizonColorRamp1;
			float _SkyHorizonColorBoundary1;
			float _SkyHorizonColorRamp1;
			float3 _SunColor1;
			float _SunRadius1;
			float _SunScatter1;
			float3 _MoonColor1;
			float _MoonRadius1;
			float _MoonScatter1;

			float3 _HigherSkyColor2;
			float3 _LowerSkyColor2;
			float3 _HigherHorizonColor2;
			float3 _LowerHorizonColor2;
			float _SkyColorBoundary2;
			float _SkyColorRamp2;
			float _HorizonColorBoundary2;
			float _HorizonColorRamp2;
			float _SkyHorizonColorBoundary2;
			float _SkyHorizonColorRamp2;
			float3 _SunColor2;
			float _SunRadius2;
			float _SunScatter2;
			float3 _MoonColor2;
			float _MoonRadius2;
			float _MoonScatter2;

			float3 _HigherSkyColor3;
			float3 _LowerSkyColor3;
			float3 _HigherHorizonColor3;
			float3 _LowerHorizonColor3;
			float _SkyColorBoundary3;
			float _SkyColorRamp3;
			float _HorizonColorBoundary3;
			float _HorizonColorRamp3;
			float _SkyHorizonColorBoundary3;
			float _SkyHorizonColorRamp3;
			float3 _SunColor3;
			float _SunRadius3;
			float _SunScatter3;
			float3 _MoonColor3;
			float _MoonRadius3;
			float _MoonScatter3;

			float3 _HigherSkyColor4;
			float3 _LowerSkyColor4;
			float3 _HigherHorizonColor4;
			float3 _LowerHorizonColor4;
			float _SkyColorBoundary4;
			float _SkyColorRamp4;
			float _HorizonColorBoundary4;
			float _HorizonColorRamp4;
			float _SkyHorizonColorBoundary4;
			float _SkyHorizonColorRamp4;
			float3 _SunColor4;
			float _SunRadius4;
			float _SunScatter4;
			float3 _MoonColor4;
			float _MoonRadius4;
			float _MoonScatter4;

			sampler2D _NoiseTexture;
			float _StarsBrightness;
            float _StarsHeight;
            float _StarsScale;

			//samplerCUBE _CloudCubeMap;

			
			///
            /// helper
            /// 
			float3 float3Lerp(float3 a, float3 b, float c)
            {
                return a * (1 - c) + b * c;
            }

            float floatLerp(float a, float b, float c)
            {
                return a * (1 - c) + b * c;
            }

			int IsGreaterOrEqual(float number1, float number2)
			{
				return 1 - saturate(ceil(number2 - number1));
			}

			int IsGreater(float number1, float number2)
			{
				return saturate(ceil(number1 - number2));
			}

			float LimitInRange11(float minValue, float maxValue, float value)
			{
				return value * IsGreaterOrEqual(value, minValue) * IsGreaterOrEqual(maxValue, value);
			}

			float LimitInRange01(float minValue, float maxValue, float value)
			{
				return value * IsGreater(value, minValue) * IsGreaterOrEqual(maxValue, value);
			}

			float LimitInRange10(float minValue, float maxValue, float value)
			{
				return value * IsGreaterOrEqual(value, minValue) * IsGreater(maxValue, value);
			}

			int IsInRange11(float minValue, float maxValue, float value)
			{
				return IsGreaterOrEqual(value, minValue) * IsGreaterOrEqual(maxValue, value);
			}

			int IsInRange01(float minValue, float maxValue, float value)
			{
				return IsGreater(value, minValue) * IsGreaterOrEqual(maxValue, value);
			}

			int IsInRange10(float minValue, float maxValue, float value)
			{
				return IsGreaterOrEqual(value, minValue) * IsGreater(maxValue, value);
			}

			float SmoothstepAfterLimitInRange11(float minValue, float maxValue, float value)
			{
				return smoothstep(minValue, maxValue, LimitInRange11(minValue, maxValue, value));
			}

			float2 ConvertSkyboxUVToSphereUV(float3 uv)
			{
				float pi = acos(-1);
				return float2(acos(dot(normalize(float3(uv.x, 0, uv.z)), float3(0, 0, 1))) / 180, acos(dot(uv, normalize(float3(uv.x, 0, uv.z)))) / 180);
			}

			float3 CalculateSkyboxBiasUV(float3 uv)
			{
				float biasY = min(max(uv.y - _SkyboxBias, -1), 1);
				
				return float3(uv.x, biasY, uv.z);
			}

			struct SkyDataInput
			{
				float3 higherSkyColor;
				float3 lowerSkyColor;
				float3 higherHorizonColor;
				float3 lowerHorizonColor;
				float skyColorBoundary;
				float skyColorRamp;
				float horizonColorBoundary;
				float horizonColorRamp;
				float skyHorizonColorBoundary;
				float skyHorizonColorRamp;
				float3 sunColor;
				float sunRadius;
				float sunScatter;
				float3 moonColor;
				float moonRadius;
				float moonScatter;
			};

			///
			/// Core
			///
			float3 CalculateBackgroundColor(float3 uv, SkyDataInput i)
			{
				float3 uv21 = CalculateSkyboxBiasUV(max(uv, 0));
				float3 uv22 = max(CalculateSkyboxBiasUV(uv), 0);
				float3 uv2 = float3Lerp(uv21, uv22, 0);
				float3 cameraDirection = normalize(UNITY_MATRIX_V[2].xyz);
                float3 forwardDirectionWS = normalize(float3(cameraDirection.x, 0, cameraDirection.z));
				float scatterFactor = pow(smoothstep(0.85, 1, dot(cameraDirection, forwardDirectionWS)), 2.5);
				// Sky-Horizon
				float skyHorizonCenterBoundary = 0.5 * (1 + i.skyHorizonColorBoundary);
				float skyHorizonLowerBoundary = skyHorizonCenterBoundary - skyHorizonCenterBoundary * i.skyHorizonColorRamp;
				float skyHorizonHigherBoundary = skyHorizonCenterBoundary + (1 - skyHorizonCenterBoundary) * i.skyHorizonColorRamp;
				float linear01SkyFactor = smoothstep(skyHorizonLowerBoundary, skyHorizonHigherBoundary, uv2.y);
				float linear01HorizonFactor = 1 - linear01SkyFactor;
				

				// SkyFactor
				float linear01LocalSkyFactor = SmoothstepAfterLimitInRange11(skyHorizonLowerBoundary, 1, uv2.y);
				float localSkyCenterBoundary = 0.5 * (1 + i.skyColorBoundary);
				float localSkyLowerBoundary = localSkyCenterBoundary - localSkyCenterBoundary * i.skyColorRamp;
				float localSkyHigherBoundary = localSkyCenterBoundary + (1 - localSkyCenterBoundary) * i.skyColorRamp;
				float linear01HigherSkyFactor = smoothstep(localSkyLowerBoundary, localSkyHigherBoundary, linear01LocalSkyFactor) * linear01SkyFactor;
				float linear01LowerSkyFactor = (1 - smoothstep(localSkyLowerBoundary, localSkyHigherBoundary, linear01LocalSkyFactor)) * linear01SkyFactor;
				

				// HorizonFactor
				float linear01LocalHorizonFactor = SmoothstepAfterLimitInRange11(0, skyHorizonHigherBoundary, uv2.y);
				float localHorizonCenterBoundary = 0.5 * (1 + i.horizonColorBoundary);
				float localHorizonLowerBoundary = localHorizonCenterBoundary - localHorizonCenterBoundary * i.horizonColorRamp;
				float localHorizonHigherBoundary = localHorizonCenterBoundary + (1 - localHorizonCenterBoundary) * i.horizonColorRamp;
				float linear01HigherHorizonFactor = smoothstep(localHorizonLowerBoundary, localHorizonHigherBoundary, linear01LocalHorizonFactor) * linear01HorizonFactor;
				float linear01LowerHorizonFactor = (1 - smoothstep(localHorizonLowerBoundary, localHorizonHigherBoundary, linear01LocalHorizonFactor)) * linear01HorizonFactor;
				

				float3 finalBackgroundColor = i.higherSkyColor * linear01HigherSkyFactor + i.lowerSkyColor * linear01LowerSkyFactor;
				finalBackgroundColor += i.higherHorizonColor * linear01HigherHorizonFactor + i.lowerHorizonColor * linear01LowerHorizonFactor;

				// Ground
				float linear01GroundFactor = ceil(-uv.y) * _EnableSolidGroundLineColor;
				float3 finalGroundLineScatterColor = ceil(-uv.y) * finalBackgroundColor * _GroundLineScatter * 0.5 * (0.025 * pow(1 - min(max(0, -uv.y), 1), 3) + pow(1 - min(max(0, -uv.y) * floatLerp(3, 20, scatterFactor), 1), 5) + pow(1 - min(max(0, -uv.y) * floatLerp(3, 50, scatterFactor), 1), 10));
				float3 finalGroundColor = _GroundColor * linear01GroundFactor + finalGroundLineScatterColor;
				

				float finalBackgroundColorGrayscale = (finalBackgroundColor.r + finalBackgroundColor.g + finalBackgroundColor.b) / 3;
				finalBackgroundColor = float3Lerp(float3(finalBackgroundColorGrayscale, finalBackgroundColorGrayscale, finalBackgroundColorGrayscale), finalBackgroundColor, _SkyColorSaturate);

				finalBackgroundColor = float3Lerp(finalBackgroundColor, finalGroundColor, linear01GroundFactor);

				return finalBackgroundColor;
			}

			float3 CalculateSunAndMoonColor(float3 uv, float3 lightDirection, SkyDataInput i)
			{
				float3 uv2 = ceil(uv);
				float3 cameraDirection = normalize(UNITY_MATRIX_V[2].xyz);
                float3 forwardDirectionWS = normalize(float3(cameraDirection.x, 0, cameraDirection.z));
				float scatterFactor = pow(smoothstep(0.85, 1, dot(cameraDirection, forwardDirectionWS)), 2.5);
				float linear01SunFactor = saturate(1 - distance(uv, lightDirection) / (0.1 * i.sunRadius));
				float linear01SunScatterFactor = saturate(1 - distance(uv, lightDirection) / (i.sunScatter * (1 + pow(1 - min(abs(uv.y) * 3, 1), 5))));
				linear01SunScatterFactor = 0.02 * pow(linear01SunScatterFactor, 5);
				float3 finalSunColor = i.sunColor * linear01SunFactor;
				float3 finalSunScatterColor = i.sunColor * linear01SunScatterFactor;

				float linear01MoonFactor = saturate(1 - distance(uv, -lightDirection) / (0.1 * i.moonRadius));
				float linear01MoonScatterFactor = saturate(1 - distance(uv, -lightDirection) / (i.moonScatter * (1 + pow(1 - min(abs(uv.y) * 3, 1), 4))));
				linear01MoonScatterFactor = 0.0025 * pow(linear01MoonScatterFactor, 3);
				float3 finalMoonColor = i.moonColor * linear01MoonFactor;
				float3 finalMoonScatterColor = i.moonColor * linear01MoonScatterFactor;

				float3 finalSunAndMoonColor = float3Lerp(finalSunColor, float3Lerp(0, 0, 0), _ReduceSunIntensity) + float3Lerp(finalMoonColor, float3Lerp(0, 0, 0), _ReduceMoonIntensity);
				float3 finalSunAndMoonScatterColor = finalSunScatterColor + finalMoonScatterColor;
				finalSunAndMoonColor += finalSunAndMoonScatterColor;
				finalSunAndMoonColor *= uv2.y;

				float linear01GroundFactor = ceil(-uv.y) * _EnableSolidGroundLineColor;
				float3 finalGroundLineScatterColor = ceil(-uv.y) * finalSunAndMoonScatterColor * _GroundLineScatter * 0.5 * (0.25 * pow(1 - min(max(0, -uv.y), 1), 5) + pow(1 - min(max(0, -uv.y) * floatLerp(2, 5, scatterFactor), 1), 3) + pow(1 - min(max(0, -uv.y) * floatLerp(2, 5, scatterFactor), 1), 3));
				float3 finalGroundColor = _GroundColor * linear01GroundFactor;
				finalSunAndMoonColor = float3Lerp(finalSunAndMoonColor, finalGroundColor, linear01GroundFactor);
				finalSunAndMoonColor += finalGroundLineScatterColor;

				float finalSunAndMoonColorGrayscale = (finalSunAndMoonColor.r + finalSunAndMoonColor.g + finalSunAndMoonColor.b) / 3;
				finalSunAndMoonColor = float3Lerp(float3(finalSunAndMoonColorGrayscale, finalSunAndMoonColorGrayscale, finalSunAndMoonColorGrayscale), finalSunAndMoonColor, _SunAndMoonColorSaturate);

				return finalSunAndMoonColor;
			}

			float3 CalculateShiftColor(float3 color1, float3 color2, float minValue, float maxValue, float value)
			{
				return float3Lerp(color1, color2, (value - minValue) / (maxValue - minValue));
			}

			float3 CalculateStarColor(float3 uv, float timeMapping)
			{
				float4 noise = 0.5 * tex2D(_NoiseTexture, uv.xz / _StarsScale) + 0.5 * tex2D(_NoiseTexture, (uv.xz * 5 - float2(_Time.y * 0.002, 0)) / _StarsScale) + 0.5 * tex2D(_NoiseTexture, (uv.xz * 4 - float2(0, _Time.y * 0.002)) / _StarsScale);
				noise = float4(saturate(noise.r), saturate(noise.g), saturate(noise.b), 1);
				float yFactor = min(uv.y - _SkyboxBias * 0.5, 1);
                float3 starColor = pow(noise.rgb, 50) * _StarsBrightness * 1000 * pow(saturate((yFactor - _StarsHeight)), 2) * pow(saturate(abs(timeMapping)), 7) * saturate(-timeMapping);
				float starColorGrayscale = max((starColor.r + starColor.g + starColor.b) / 3, 0.001);
				starColor = starColor / starColorGrayscale * pow(_StarsBrightness, 2);
				starColorGrayscale = (starColor.r + starColor.g + starColor.b) / 3;
				starColor = float3Lerp(float3(starColorGrayscale, starColorGrayscale, starColorGrayscale), starColor, _SkyColorSaturate);

				return starColor;
			}

			SkyDataInput skyData1;
			SkyDataInput skyData2;
			SkyDataInput skyData3;
			SkyDataInput skyData4;

			void InitializeSkyData()
			{
				skyData1.higherSkyColor = _HigherSkyColor1;
				skyData1.lowerSkyColor = _LowerSkyColor1;
				skyData1.higherHorizonColor = _HigherHorizonColor1;
				skyData1.lowerHorizonColor = _LowerHorizonColor1;
				skyData1.skyColorBoundary = _SkyColorBoundary1;
				skyData1.skyColorRamp = _SkyColorRamp1;
				skyData1.horizonColorBoundary = _HorizonColorBoundary1;
				skyData1.horizonColorRamp = _HorizonColorRamp1;
				skyData1.skyHorizonColorBoundary = _SkyHorizonColorBoundary1;
				skyData1.skyHorizonColorRamp = _SkyHorizonColorRamp1;
				skyData1.sunColor = _SunColor1;
				skyData1.sunRadius = _SunRadius1;
				skyData1.sunScatter = _SunScatter1;
				skyData1.moonColor = _MoonColor1;
				skyData1.moonRadius = _MoonRadius1;
				skyData1.moonScatter = _MoonScatter1;

				skyData2.higherSkyColor = _HigherSkyColor2;
				skyData2.lowerSkyColor = _LowerSkyColor2;
				skyData2.higherHorizonColor = _HigherHorizonColor2;
				skyData2.lowerHorizonColor = _LowerHorizonColor2;
				skyData2.skyColorBoundary = _SkyColorBoundary2;
				skyData2.skyColorRamp = _SkyColorRamp2;
				skyData2.horizonColorBoundary = _HorizonColorBoundary2;
				skyData2.horizonColorRamp = _HorizonColorRamp2;
				skyData2.skyHorizonColorBoundary = _SkyHorizonColorBoundary2;
				skyData2.skyHorizonColorRamp = _SkyHorizonColorRamp2;
				skyData2.sunColor = _SunColor2;
				skyData2.sunRadius = _SunRadius2;
				skyData2.sunScatter = _SunScatter2;
				skyData2.moonColor = _MoonColor2;
				skyData2.moonRadius = _MoonRadius2;
				skyData2.moonScatter = _MoonScatter2;

				skyData3.higherSkyColor = _HigherSkyColor3;
				skyData3.lowerSkyColor = _LowerSkyColor3;
				skyData3.higherHorizonColor = _HigherHorizonColor3;
				skyData3.lowerHorizonColor = _LowerHorizonColor3;
				skyData3.skyColorBoundary = _SkyColorBoundary3;
				skyData3.skyColorRamp = _SkyColorRamp3;
				skyData3.horizonColorBoundary = _HorizonColorBoundary3;
				skyData3.horizonColorRamp = _HorizonColorRamp3;
				skyData3.skyHorizonColorBoundary = _SkyHorizonColorBoundary3;
				skyData3.skyHorizonColorRamp = _SkyHorizonColorRamp3;
				skyData3.sunColor = _SunColor3;
				skyData3.sunRadius = _SunRadius3;
				skyData3.sunScatter = _SunScatter3;
				skyData3.moonColor = _MoonColor3;
				skyData3.moonRadius = _MoonRadius3;
				skyData3.moonScatter = _MoonScatter3;

				skyData4.higherSkyColor = _HigherSkyColor4;
				skyData4.lowerSkyColor = _LowerSkyColor4;
				skyData4.higherHorizonColor = _HigherHorizonColor4;
				skyData4.lowerHorizonColor = _LowerHorizonColor4;
				skyData4.skyColorBoundary = _SkyColorBoundary4;
				skyData4.skyColorRamp = _SkyColorRamp4;
				skyData4.horizonColorBoundary = _HorizonColorBoundary4;
				skyData4.horizonColorRamp = _HorizonColorRamp4;
				skyData4.skyHorizonColorBoundary = _SkyHorizonColorBoundary4;
				skyData4.skyHorizonColorRamp = _SkyHorizonColorRamp4;
				skyData4.sunColor = _SunColor4;
				skyData4.sunRadius = _SunRadius4;
				skyData4.sunScatter = _SunScatter4;
				skyData4.moonColor = _MoonColor4;
				skyData4.moonRadius = _MoonRadius4;
				skyData4.moonScatter = _MoonScatter4;
			}

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 uv : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 positionOS : TEXCOORD1;
                float3 uv : TEXCOORD2;
			};

			Varyings vert(Attributes v)
			{
				Varyings o;
				o.positionCS = UnityObjectToClipPos(v.positionOS);
				o.positionWS = mul(unity_ObjectToWorld, v.positionOS);
				o.positionOS = v.positionOS.xyz;
                o.uv = v.uv;
				return o;
			}

			float4 frag(Varyings i): SV_Target
			{
				InitializeSkyData();
                float3 lightDirection = normalize(_WorldSpaceLightPos0);
                float timeMapping = dot(lightDirection, float3(0, 1, 0));

				float3 finalSkyboxColor1 = CalculateBackgroundColor(i.uv, skyData1) + CalculateSunAndMoonColor(i.uv, lightDirection, skyData1);
				float3 finalSkyboxColor2 = CalculateBackgroundColor(i.uv, skyData2) + CalculateSunAndMoonColor(i.uv, lightDirection, skyData2);
				float3 finalSkyboxColor3 = CalculateBackgroundColor(i.uv, skyData3) + CalculateSunAndMoonColor(i.uv, lightDirection, skyData3);
				float3 finalSkyboxColor4 = CalculateBackgroundColor(i.uv, skyData4) + CalculateSunAndMoonColor(i.uv, lightDirection, skyData4);

				float3 skyboxShiftColor1 = CalculateShiftColor(finalSkyboxColor1, finalSkyboxColor2, -1, -(_SunSetDuration * 0.4 + 0.1), timeMapping) * IsInRange10(-1, -(_SunSetDuration * 0.4 + 0.1), timeMapping);
				float3 skyboxShiftColor2 = CalculateShiftColor(finalSkyboxColor2, finalSkyboxColor3, -(_SunSetDuration * 0.4 + 0.1), (_SunSetDuration * 0.4 + 0.1), timeMapping) * IsInRange11(-(_SunSetDuration * 0.4 + 0.1), (_SunSetDuration * 0.4 + 0.1), timeMapping);
				float3 skyboxShiftColor3 = CalculateShiftColor(finalSkyboxColor3, finalSkyboxColor4, (_SunSetDuration * 0.4 + 0.1), 1, timeMapping) * IsInRange01((_SunSetDuration * 0.4 + 0.1), 1, timeMapping);

				float3 starColor = CalculateStarColor(i.uv, timeMapping);

				float3 finalSkyboxColor = skyboxShiftColor1 + skyboxShiftColor2 + skyboxShiftColor3 + starColor;
				finalSkyboxColor *= _Brightness;

                return float4(finalSkyboxColor, 1);
		    }

		    ENDCG
	    }
    }
}