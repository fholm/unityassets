Shader "Action Bar/Pickup" {
	Properties {
		_Button ("Button", 2D) = "" { TexGen ObjectLinear }
		_Atlas ("Atlas", 2D) = "" { TexGen ObjectLinear }
		_IconScale ("IconScale", float) = 0.25
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off 
		ColorMask RGB
		Lighting Off 
		ZWrite Off
		Fog { Color (0,0,0,1) }

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct text_input {
				float4 vertex : POSITION;
				float4 color : COLOR0;
				float4 texcoord : TEXCOORD0;
			};
			
			uniform sampler2D _Atlas;
			uniform sampler2D _Button;
			float _IconScale;

			struct output {
				float4  pos : SV_POSITION;
				float4	cl : COLOR0;
				float2  uv : TEXCOORD0;
			};

			output vert (text_input v)
			{
				output o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				o.cl = v.color;

				return o;
			}

			half4 frag (output i) : COLOR
			{
				float2 uv = i.uv;

				// Mask uv (bottom left = half)
				float2 half_uv = uv * 0.5f;

				// Atlas
				float2 atlas_uv = i.uv * _IconScale;
				atlas_uv.x = i.cl.g + atlas_uv.x;
				atlas_uv.y = i.cl.a + atlas_uv.y;
				
				// mask
				float4 mask = tex2D(_Button, half_uv);

				// Return color
				return tex2D(_Atlas, atlas_uv) * mask.r;
			}

			ENDCG
		}
	}
}