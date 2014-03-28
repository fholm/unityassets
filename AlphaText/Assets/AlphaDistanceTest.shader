Shader "Action Bar/Text" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" { TexGen ObjectLinear }
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off
		ColorMask RGB
		Lighting Off 
		ZWrite Off
		AlphaTest GEqual 0.3
		Fog { Mode Off }

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;

			struct IN {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct OUT {
				float4  vertex : SV_POSITION;
				float2  texcoord : TEXCOORD0;
			};

			OUT vert (IN v)
			{
				OUT o;

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;

				return o;
			}

			float4 frag (OUT i) : COLOR0
			{
				float4 outlineColor = float4(0, 0, 0, 1);
				float4 baseColor = float4(1, 0.75, 0, 1);
				float alphaMask = tex2D(_MainTex, i.texcoord).r;
				float outlineMinValue0 = 0.4;
				float outlineMinValue1 = 0.6;
				float outlineFactor = 1;
				
				if(alphaMask < outlineMinValue1)
				{
					outlineFactor = smoothstep(outlineMinValue0, outlineMinValue1, alphaMask);
				}
				else
				{
					outlineFactor = smoothstep(outlineMinValue1, outlineMinValue0, alphaMask);
				}
				
				baseColor = lerp(baseColor, outlineColor, outlineFactor);

				//baseColor.a = 1 * smoothstep(0.5, 0.6, alphaMask);
				
				/*
				if(
				float outlineFactor = 1.0;
				float4 glowTexel = tex2D(_MainTex, i.uv.xy + (0.1, 0.1))
				float4 glowColor = (1, 0, 0, 1) * smoothstep(0.3, 0.7, glowTexel.a);
				return lerp(glowColor, color, mskUsed); // <- Where the fuck does mskUsed come from?
				*/

				return baseColor;
			}

			ENDCG
		}
	}
}