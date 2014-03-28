Shader "Hidden/BlendModesOverlay" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_Overlay ("Color", 2D) = "white" {}
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
	};
			
	sampler2D _Overlay;
	sampler2D _MainTex;
	
	half _Intensity;
	half4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v ) { 
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv[0] =  v.texcoord.xy;
		
		#if SHADER_API_D3D9 || SHADER_API_D3D11 || SHADER_API_XBOX360
		if(_MainTex_TexelSize.y<0.0)
			o.uv[0].y = 1.0-o.uv[0].y;
		#endif
		
		o.uv[1] =  v.texcoord.xy;	
		return o;
	}
	
	half4 fragAddSub (v2f i) : COLOR {
		half4 toAdd = tex2D(_Overlay, i.uv[0]) * _Intensity;
		return tex2D(_MainTex, i.uv[1]) + toAdd;
	}

	half4 fragMultiply (v2f i) : COLOR {
		half4 toBlend = tex2D(_Overlay, i.uv[0]) * _Intensity;
		return tex2D(_MainTex, i.uv[1]) * toBlend;
	}	
			
	half4 fragScreen (v2f i) : COLOR {
		half4 toBlend =  (tex2D(_Overlay, i.uv[0]) * _Intensity);
		return 1-saturate(1-toBlend)*(1-(tex2D(_MainTex, i.uv[1])));
	}

	half4 fragOverlay (v2f i) : COLOR {
		half4 m = (tex2D(_Overlay, i.uv[0])) * 255.0;
		half4 color = (tex2D(_MainTex, i.uv[1]))* 255.0;

		// overlay blend mode
		color.rgb = (color.rgb/255.0) * (color.rgb + ((2*m.rgb)/( 255.0 )) * (255.0-color.rgb));
		color.rgb /= 255.0; 
		return color;
	}
	
	half4 fragAlphaBlend (v2f i) : COLOR {
		half4 toAdd = tex2D(_Overlay, i.uv[0]) ;
		return lerp(tex2D(_MainTex, i.uv[1]), toAdd, toAdd.a);
	}	


	ENDCG 
	
Subshader {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }  
      ColorMask RGB	  
  		  	
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAddSub
      ENDCG
  }

 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragScreen
      ENDCG
  }

 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragMultiply
      ENDCG
  }  

 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragOverlay
      ENDCG
  }  
  
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAlphaBlend
      ENDCG
  }   
}

Fallback off
	
} // shader