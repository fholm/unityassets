Shader "Fps Hud/Alpha Blend (Masked)" {
	Properties {
		_MainTex ("Texture", 2D) = "" { TexGen ObjectLinear }
		_MaskTex ("Texture", 2D) = "" { TexGen ObjectLinear }
		_Color ("Tint Color", Color) = (1, 1, 1, 1)
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		ColorMask RGBA
		Lighting Off 
		ZWrite Off
		Fog { Color (0,0,0,1) }

		Pass {

			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _MaskTex;
			uniform float4x4 _MaskMatrix;
			float4 _MainTex_ST;

			struct IN {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct OUT {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float4 mask : TEXCOORD1;
			};

			OUT vert (IN v)
			{
				OUT o;

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.mask = mul(_MaskMatrix, mul(_Object2World, v.vertex));
				o.mask.x += 0.5f;

				//o.mask.y += 0.25f;

				return o;
			}

			half4 frag (OUT i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.texcoord) * _Color;
				return float4(color.rgb, color.a * tex2D(_MaskTex, i.mask.xy));
			}

			ENDCG
		}
	}
}