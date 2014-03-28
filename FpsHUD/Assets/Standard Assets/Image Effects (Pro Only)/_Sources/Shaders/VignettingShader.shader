Shader "Hidden/VignettingShader" {
	Properties {
		_MainTex ("Base", 2D) = "" {}
		_VignetteTex ("Vignette", 2D) = "" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	sampler2D _VignetteTex;
	
	float4 _MainTex_TexelSize;
	float _Intensity;
	float _Blur;
		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	half4 frag(v2f i) : COLOR {
		half2 coords = i.uv;
		half2 uv = i.uv;
		
		coords = (coords - 0.5) * 2.0;		
		half coordDot = dot (coords,coords);
		half4 color = tex2D (_MainTex, uv);	 
		 		 
		float mask = 1.0 - coordDot * _Intensity * 0.1; 
		
		half4 colorBlur = tex2D (_VignetteTex, uv);
		color = lerp (color, colorBlur, saturate (_Blur * coordDot));
		
		return color * mask;
	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}

Fallback off	
} 