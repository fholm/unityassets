Shader "Hidden/NoiseAndGrain" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NoiseTex ("Noise (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _NoiseTex;
		float4 _NoiseTex_TexelSize;
		
		#if SHADER_API_D3D9 || SHADER_API_XBOX360 || SHADER_API_D3D11
			uniform half4 _MainTex_TexelSize;
		#endif

		uniform half3 _NoisePerChannel;
		uniform half3 _NoiseTilingPerChannel;
		uniform half3 _NoiseAmount;
		
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half2 uv2 : TEXCOORD1;
			#if SHADER_API_D3D9 || SHADER_API_XBOX360 || SHADER_API_D3D11
			half4 uv_screen : TEXCOORD2;	
			#else 
			half2 uv_screen : TEXCOORD2;
			#endif		
		};
		
		struct appdata_img2 {
		    float4 vertex : POSITION;
		    half2 texcoord : TEXCOORD0;
		    half2 texcoord1 : TEXCOORD1;
		};		
						
		v2f vert (appdata_img2 v)
		{
			v2f o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv = v.texcoord.xy;
			o.uv2 = v.texcoord1.xy;
			
			#if SHADER_API_D3D9 || SHADER_API_XBOX360 || SHADER_API_D3D11
			o.uv_screen = v.vertex.xyxy;
			if (_MainTex_TexelSize.y < 0)
        		o.uv_screen.y = 1-o.uv_screen.y;
        		#else
        		o.uv_screen = v.vertex.xy;
			#endif
			
			return o; 
		}

		half4 frag ( v2f i ) : COLOR
		{	
			half4 color = tex2D (_MainTex, i.uv_screen.xy);
			
			// curves
			half2 blackWhiteCurve = half2(saturate(Luminance(color.rgb)), saturate(1.0-saturate(Luminance(color.rgb))));
			blackWhiteCurve *= blackWhiteCurve;
			half blackWhiteIntensity = _NoiseAmount.z * (blackWhiteCurve.x) + _NoiseAmount.y * saturate(blackWhiteCurve.y);
			
			// overlay noise mask
			half3 m = half3(0.0, 0.0, 0.0);
			
			m.r = tex2D(_NoiseTex, i.uv.xy + i.uv2.xy*_NoiseTex_TexelSize.xy*_NoiseTilingPerChannel.r).r;
			m.g = tex2D(_NoiseTex, i.uv.xy + i.uv2.xy*_NoiseTex_TexelSize.xy*_NoiseTilingPerChannel.g).g;
			m.b = tex2D(_NoiseTex, i.uv.xy + i.uv2.xy*_NoiseTex_TexelSize.xy*_NoiseTilingPerChannel.b).b;
			
			m = m * 2 - 1;
			m *= _NoisePerChannel.rgb * color.rgb * (_NoiseAmount.x) * blackWhiteIntensity;
			m = m * 0.5 + 0.5;
						
			color.rgb = saturate(color.rgb) * 255.0;
			m = saturate(m) * 255.0;
			
			// overlay blend mode
			color.rgb = (color.rgb/255.0) * (color.rgb + ((2*m)/(255.0)) * (255.0-color.rgb));
			color.rgb /= 255.0; 
			return color;
		} 
	
	ENDCG
	
	SubShader {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }  
	  
		Pass {
	
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}				
	}
	FallBack Off
}
