Shader "Fps Hud/Image Effect" {

	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_HudTex ("Hud", 2D) = "white" {}
	}

	SubShader {
		Pass {
			ZTest Always 
			Cull Off 
			ZWrite Off
			Blend One Zero
			//Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode off }
				
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _HudTex;

			float4 frag (v2f_img i) : COLOR
			{	
				float4 main = tex2D(_MainTex, i.uv);

				#if SHADER_API_D3D9
				i.uv.y = 1f - i.uv.y;
				#endif

				float4 hud = tex2D(_HudTex, i.uv);

				return main + hud;
			}
			ENDCG
		}
	}

	Fallback off

}