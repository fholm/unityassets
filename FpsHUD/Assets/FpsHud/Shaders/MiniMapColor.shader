Shader "Fps Hud/Mini Map/Backdrop" {
	Properties {
		_Color ("Color", Color) = (0, 0, 0, 1)
		_MaskTex ("Mask", 2D) = "" { TexGen ObjectLinear }
		_MapSize ("Map Size", float) = 1
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
			
			float _MapSize;
			float4 _Color;
			sampler2D _MaskTex;

			struct IN {
				float4 vertex : POSITION;
			};

			struct OUT { 
				float4 vertex : SV_POSITION;
				float2 mask : TEXCOORD1;
			};

			OUT vert (IN i)
			{
				OUT o;
				 
				float4 world = mul(_Object2World, i.vertex);
				float halfSize = _MapSize / 2;

				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.mask.x = (world.x + halfSize) / _MapSize;
				o.mask.y = (world.y + halfSize) / _MapSize;

				return o;
			}

			half4 frag (OUT i) : COLOR
			{
				half mask = tex2D(_MaskTex, i.mask).r;
				return half4(_Color.rgb, mask);
			}

			ENDCG
		}
	}
}