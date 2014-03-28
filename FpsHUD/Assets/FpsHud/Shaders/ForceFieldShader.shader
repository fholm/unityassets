Shader "Force Field" {
	Properties {
		_Atlas ("Atlas", 2D) = "" { TexGen ObjectLinear }
		_TintColor ("_TintColor", Color) = (1, 1, 1, 0.25)
		_Extrude ("Extrude", float) = 0.01
		_Speed ("Speed", float) = 0.25
		_MinAlpha ("Min Alpha", float) = 0.1
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
			
			float _Speed;
			float _Extrude;
			float _MinAlpha;
			float4 _TintColor;
			float4 _Atlas_ST;
			sampler2D _Atlas;
			sampler2D _CameraDepthTexture;

			struct text_input {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 normal : NORMAL0;
			};

			struct output {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float4 proj : TEXCOORD1;
				float2 angles : TEXCOORD2;
			};

			output vert (text_input v)
			{
				output o;

				float4 world = mul(_Object2World, v.vertex);
				float4 worldNormal = mul(_Object2World, v.normal);
				float4 position = mul(UNITY_MATRIX_MVP, v.vertex);
				float4 normal = mul(UNITY_MATRIX_MVP, v.normal);
				float3 camDir = normalize(_WorldSpaceCameraPos - world.xyz);
				
				o.vertex = position + (normal * _Extrude);
				o.texcoord = fmod(_Time.y * _Speed, 1f) + TRANSFORM_TEX(v.texcoord, _Atlas);
				o.angles = 0;
				o.angles.x = (acos(dot(camDir, worldNormal)) / 2.2f);

				// Compute projection
				o.proj = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.proj.z);

				return o;
			}

			half4 frag (output i) : COLOR
			{
				float4 color = float4(1, 1, 0, 1);
				half4 tex = tex2D(_Atlas, i.texcoord);
				float depth = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.proj))));
				float alpha = tex.a * _TintColor.a * clamp((depth  - i.proj.z) / 1, _MinAlpha, 1.0);

				return half4(_TintColor.rgb, alpha);
			}

			ENDCG
		}
	}
}