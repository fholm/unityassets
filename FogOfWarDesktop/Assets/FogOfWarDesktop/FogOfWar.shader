Shader "Fog Of War/Blurred (Pre-Render)" {
	Properties {
		_FadeTime ("Fade Time", float) = 1
		_WorldTime ("World Time", float) = 0
		_Color ("Fog Color", Color) = (1, 1, 1, 0.5)
	}

	SubShader {
		Tags { "Queue"="Overlay-1" "RenderType"="Transparent" }
		Cull Back 
		Lighting Off 
		ZWrite Off
		ZTest Always
		Fog { Mode Off }

		Pass {
			Blend One Zero

			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float4 _Color;
			float _FadeTime;
			float _WorldTime;

			struct output {
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			output vert (appdata_base v)
			{
				output o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;

				return o;
			}

			half4 frag (output i) : COLOR
			{
				float t;

				t = 1 - ((_WorldTime - i.uv.x) / _FadeTime);
				t = clamp(t, i.uv.y, 1);
				
				return half4(t + (t * _Color * abs(1 - t)));
			}

			ENDCG
		}
	}
}