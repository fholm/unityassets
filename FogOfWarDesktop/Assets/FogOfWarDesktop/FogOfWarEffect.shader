Shader "Fog Of War/Blurred (Effect)" {

	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_FogTex ("Fog", 2D) = "white" {}
	}

	SubShader {
		Pass {
			ZTest Always 
			Cull Off 
			ZWrite Off
			Blend One Zero
			Fog { Mode off }
				
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _FogTex;

			fixed4 frag (v2f_img i) : COLOR
			{	
				return tex2D(_MainTex, i.uv) * tex2D(_FogTex, i.uv);
			}
			ENDCG
		}
	}

	Fallback off

}