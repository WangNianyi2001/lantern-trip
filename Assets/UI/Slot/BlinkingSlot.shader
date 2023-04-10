Shader "BlinkingSlot" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		mainColor ("Color", color) = (1, 1, 1, 0)
		t ("Time", float) = 0
		attenuation ("Attenuation", float) = 2
		blink ("Blink", float) = .8
	}
	SubShader {
		Pass {
			Tags{ "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float3 mainColor;
			float t;
			float attenuation;
			float blink;

			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);

				float s = exp(-(floor(t + 1) - t) * attenuation);
				float3 blinkColor = lerp(mainColor, float3(1, 1, 1), blink);
				col.rgb *= lerp(mainColor, blinkColor, s);
				return col;
			}
			ENDCG
		}
	}
}
