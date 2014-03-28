Shader "Fps Hud/Mini Map/Border" {
	Properties {
		_Color ("Color", Color) = (0, 0, 0, 1)
		_MainTex ("Mask", 2D) = "" { TexGen ObjectLinear }
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off 
		ColorMask RGB
		Lighting Off 
		ZWrite Off
		Offset -1, -1
		Fog { Color (0,0,0,1) }

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float4 _Color;
			sampler2D _MainTex;

			struct IN {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct OUT { 
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			OUT vert (IN i)
			{
				OUT o;
				
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.texcoord = i.texcoord;

				return o;
			}

			half4 frag (OUT i) : COLOR
			{
				half4 main = tex2D(_MainTex, i.texcoord);
				return main * _Color;
			}

			ENDCG
		}
	}
}