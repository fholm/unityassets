Shader "Fps Hud/Temporal Repeater" {
	Properties {
		_Line ("Line", 2D) = "" {}
		_Gradient ("Gradient", 2D) = "" {}
		_Mod ("Mod", float) = 0 
		_TintColor ("Tint Color", Color) = (1, 1, 1, 1)
	}

	SubShader {
		Pass {
			ZTest Always 
			Cull Off 
			ZWrite Off
			Fog { Mode off }
			Blend SrcAlpha OneMinusSrcAlpha
				
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"
			
			float _Mod;
			fixed4 _TintColor;
			sampler2D _Line;
			sampler2D _Gradient;

			fixed4 frag (v2f_img i) : COLOR
			{
				//clamp(((1 - _Mod)), 0, 0.5f)
				float2 uv = i.uv;

				//i.uv.x = fmod(_Time.y, 1f) + i.uv.x; // + fmod(i.uv.x, _Mod);
				i.uv.x = fmod(i.uv.x + (fmod(_Time.y, 2f) / 2f), _Mod);

				fixed4 color = tex2D(_Line, i.uv);

				/*
				fixed4 gradient = tex2D(_Gradient, uv);
				float t = fmod(_Time.y, 2f) / 2f;
				float coef = 1 - clamp((1 - floor((t / (1 - gradient.r)))), 0, 1);
				*/

				return fixed4(color.rbg * _TintColor.rgb, color.a);
			}
			ENDCG

		}
	}

	Fallback off
}
