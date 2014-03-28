Shader "Battle Text/Animated Text" {
	Properties {
		_FadeTime ("FadeTime", float) = 1
		_Atlas ("Atlas", 2D) = "" { TexGen ObjectLinear }
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
			
			float _FadeTime;
			uniform sampler2D _Atlas;

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
				float4 cl = i.cl;
				float2 uv = i.uv;
					
				uv.y = i.cl.a;
				cl.a = i.uv.y;

				float4 t = tex2D(_Atlas, uv);
				float a = clamp(t.a, 0, 0.75);
				
				return half4(t.xyz * cl.xyz, t.a * (1 - (clamp(_Time.y - cl.a, 0, _FadeTime) / _FadeTime)));
			}

			ENDCG
		}
	}
}