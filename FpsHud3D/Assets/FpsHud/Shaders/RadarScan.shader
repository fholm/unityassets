Shader "Fps Hud/Radar Scan" {
	Properties {
		_MainTex ("Texture", 2D) = "" { TexGen ObjectLinear }
		_MaskTex ("Mask", 2D) = "" { TexGen ObjectLinear }
		_Color ("Tint Color", Color) = (1, 1, 1, 1)
		_Speed ("Speed", float) = 2.0
		_Start ("Start", float) = 0.0
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
			
			float _Speed;
			float _Start;
			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _MaskTex;

			struct IN {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct OUT {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordMask : TEXCOORD1;
			};

			OUT vert (IN v)
			{
				OUT o;

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoordMask = v.texcoord;
				o.texcoord = v.texcoord;
				o.texcoord.y = -((_Time.y - _Start) / _Speed) + o.texcoord.y + 0.38;

				return o;
			}

			half4 frag (OUT i) : COLOR
			{
				return tex2D(_MainTex, i.texcoord) * _Color * tex2D(_MaskTex, i.texcoordMask).r;
			}

			ENDCG
		}
	}
}