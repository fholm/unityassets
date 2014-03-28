Shader "Fog Of War/Soft (Indie)" {
	Properties {
		_FadeTime ("Fade Time", float) = 1
		_WorldTime ("World Time", float) = 0
		_Color ("Fog Color", Color) = (1, 1, 1, 0.5)
	}

	SubShader {
		Tags { "Queue"="Overlay-1" "RenderType"="Transparent" }
		Cull Back 
		Lighting Off 
		Fog { Mode Off }

		Pass {
			ZWrite On
			ZTest Always
			ColorMask 0
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 vert (appdata_base v) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, v.vertex);
			}

			half4 frag (float4 i) : COLOR
			{
				return 0;
			}

			ENDCG
		}

		Pass {
			Blend DstColor Zero
			ZWrite Off
			ZTest Equal

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float4 _Color;
			float _FadeTime;
			float _WorldTime;

			struct output {
				float4 pos : SV_POSITION;
				half4 col : COLOR;
			};

			output vert (appdata_base v)
			{
				float t;
				output o;
				
				t = clamp(1 - ((_WorldTime - v.texcoord.x) / _FadeTime), v.texcoord.y, 1);

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.col = t + (t * _Color * abs(1 - t));
				
				return o;
			}

			half4 frag (output i) : COLOR
			{
				return i.col;
			}

			ENDCG
		}
	}
}