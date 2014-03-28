Shader "Action Bar/Button" {
	Properties {
		_Button ("Button", 2D) = "" { TexGen ObjectLinear }
		_Atlas ("Atlas", 2D) = "" { TexGen ObjectLinear }
		_IconScale ("IconScale", float) = 0.25
		_OverlayColor ("Overlay Tint", Color) = (1, 0, 0, 0.5)
		_CooldownColor ("Cooldown Darkness", float) = 0.75
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
				float3 normal : NORMAL;
				float4 color : COLOR0;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
			};
			
			uniform sampler2D _Atlas;
			uniform sampler2D _Button;
			float _IconScale;
			float _CooldownColor;
			float4 _OverlayColor;

			struct output {
				float4  pos : SV_POSITION;
				float4	cl : COLOR0;
				float3  nrm : COLOR1;
				float2  uv : TEXCOORD0;
				float2  uv1 : TEXCOORD1;
			};

			output vert (text_input v)
			{
				output o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				o.cl = v.color;
				o.nrm = v.normal;
				o.uv1 = v.texcoord1;

				return o;
			}

			half4 frag (output i) : COLOR
			{
				// Mask uv (bottom left = half)
				float2 half_uv = i.uv * 0.5f;

				// Atlas
				float2 atlas_uv = i.uv * _IconScale;
				atlas_uv.x = i.cl.g + atlas_uv.x;
				atlas_uv.y = i.cl.a + atlas_uv.y;

				// Sample crap
				float4 bg = tex2D(_Button, float2(half_uv.x, 1 - half_uv.y));
				float4 mask = tex2D(_Button, half_uv);
				float4 overlay = tex2D(_Button, float2(half_uv.x + 0.5, 1 - half_uv.y)) * _OverlayColor * mask.r * i.cl.r;
				float4 icon = tex2D(_Atlas, atlas_uv) * mask.r * i.cl.b;
				float4 cd = tex2D(_Button, float2(half_uv.x + 0.5, half_uv.y));

				// Icon color
				icon.rgb = (((icon.r + icon.g + icon.b) / 3) * (i.nrm.x)) + (icon.rgb * (1 - i.nrm.x));

				// Cooldown
				float cd_t = (_Time.y - i.uv1.x) / i.uv1.y;
				float cd_coef = clamp((1 - floor((cd_t / (1 - cd.r)))), 0, 1);
				float4 cd_clr = cd_coef * mask.r * _CooldownColor;

				// Calculate end reuslt
				return  bg + (icon * (1 - cd_clr)) + overlay;
			}

			ENDCG
		}
	}
}