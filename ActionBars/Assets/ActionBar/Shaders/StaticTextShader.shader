Shader "Action Bar/Text" {
	Properties {
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
				float4 t = tex2D(_Atlas, i.uv);
				return half4(t.xyz * i.cl.xyz, t.a * i.cl.a);
			}

			ENDCG
		}
	}
}