Shader "Fps Hud/Grayscale Effect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Amount ("Amount (0, 1)", float) = 0 
	}

	SubShader {
		Pass {
			ZTest Always 
			Cull Off 
			ZWrite Off
			Fog { Mode off }
				
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"
			
			float _Amount;
			uniform sampler2D _MainTex;

			fixed4 frag (v2f_img i) : COLOR
			{
				fixed4 color = tex2D(_MainTex, i.uv);
				fixed grayscale = dot(color.rgb, fixed3(0.299, 0.587, 0.114)); // fixed3(0.22, 0.707, 0.071)

				color.r = ((1 - _Amount) * color.r) + (_Amount * grayscale) + (_Amount * 0.1);
				color.g = ((1 - _Amount) * color.g) + (_Amount * grayscale) + (_Amount * 0.1);
				color.b = ((1 - _Amount) * color.b) + (_Amount * grayscale) + (_Amount * 0.1);

				return color;
			}
			ENDCG

		}
	}

	Fallback off
}