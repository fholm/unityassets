Shader "Fps Hud/Mini Map/Icon" {
	Properties {
		_MainTex ("Texture", 2D) = "" { TexGen ObjectLinear }
		_MaskTex ("Mask", 2D) = "" { TexGen ObjectLinear }
		_MapSize ("Map Size", float) = 256
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
			sampler2D _MainTex;
			sampler2D _MaskTex;

			struct IN {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR0;
			};

			struct OUT { 
				float4 vertex : SV_POSITION;
				float4 color : COLOR0;
				float2 tex : TEXCOORD0;
				float2 mask : TEXCOORD1;
			};

			OUT vert (IN i)
			{
				OUT o;
				
				float4 world = mul(_Object2World, i.vertex);
				float halfSize = _MapSize / 2;

				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.tex = i.texcoord;
				o.mask.x = (world.x + halfSize) / _MapSize;
				o.mask.y = (world.y + halfSize) / _MapSize;
				o.color = i.color;

				return o;
			}

			half4 frag (OUT i) : COLOR
			{
				half4 main = tex2D(_MainTex, i.tex);
				half  mask = tex2D(_MaskTex, i.mask).r;

				return half4(main.rgb * i.color.rgb, i.color.a * main.a * mask);
			}

			ENDCG
		}
	}
}