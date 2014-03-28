
Shader "Hidden/EdgeDetectGeometry" { 
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	// Shader code pasted into all further CGPROGRAM blocks	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv[5] : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	uniform half4 _MainTex_TexelSize;
	sampler2D _CameraDepthNormalsTexture;
	
	uniform half4 sensitivity; 
	uniform half4 _BgColor;
	uniform half _BgFade;
	
	inline half CheckSame (half2 centerNormal, float centerDepth, half4 sample)
	{
		// difference in normals
		// do not bother decoding normals - there's no need here
		half2 diff = abs(centerNormal - sample.xy) * sensitivity.y;
		half isSameNormal = (diff.x + diff.y) * sensitivity.y < 0.1;
		// difference in depth
		float sampleDepth = DecodeFloatRG (sample.zw);
		float zdiff = abs(centerDepth-sampleDepth);
		// scale the required threshold by the distance
		half isSameDepth = zdiff * sensitivity.x < 0.09 * centerDepth;
	
		// return:
		// 1 - if normals and depth are similar enough
		// 0 - otherwise
		
		return isSameNormal * isSameDepth;
	}	
		
	v2f vertRobert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = uv;
		
		// On D3D when AA is used, the main texture & scene depth texture
		// will come out in different vertical orientations.
		// So flip sampling of depth texture when that is the case (main texture
		// texel size will have negative Y)
		
		#if SHADER_API_D3D9
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif
				
		// calc coord for the X pattern
		// maybe nicer TODO for the future: rotated triangles
		
		o.uv[0] = uv;
		o.uv[1] = uv + _MainTex_TexelSize.xy * half2(1,1);
		o.uv[2] = uv + _MainTex_TexelSize.xy * half2(-1,-1);
		o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1,1);
		o.uv[4] = uv + _MainTex_TexelSize.xy * half2(1,-1);
				 
		return o;
	} 
	
	v2f vertThin( appdata_img v )
	{
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = uv;
		
		// On D3D when AA is used, the main texture & scene depth texture
		// will come out in different vertical orientations.
		// So flip sampling of depth texture when that is the case (main texture
		// texel size will have negative Y)
		
		#if SHADER_API_D3D9
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif
		
		o.uv[1] = uv;
		o.uv[4] = uv;
				
		// offsets for two additional samples
		o.uv[2] = uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y);
		o.uv[3] = uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y);
		
		return o;
	}	  
	 
	half4 fragRobert(v2f i) : COLOR {		
		
		//half4 sample0 = tex2D(_CameraDepthNormalsTexture, i.uv[0].xy);
		half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1].xy);
		half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2].xy);
		half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3].xy);
		half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4].xy);

		half edge = 1.0;
		
		edge *= CheckSame(sample1.xy, DecodeFloatRG(sample1.zw), sample2);
		edge *= CheckSame(sample3.xy, DecodeFloatRG(sample3.zw), sample4);

		return edge * lerp(tex2D(_MainTex, i.uv[0].xy), _BgColor, _BgFade);
	}
	
	half4 fragThin (v2f i) : COLOR
	{
		half4 original = tex2D(_MainTex, i.uv[0]);
		
		half4 center = tex2D (_CameraDepthNormalsTexture, i.uv[1]);
		half4 sample1 = tex2D (_CameraDepthNormalsTexture, i.uv[2]);
		half4 sample2 = tex2D (_CameraDepthNormalsTexture, i.uv[3]);
		
		// encoded normal
		half2 centerNormal = center.xy;
		// decoded depth
		float centerDepth = DecodeFloatRG (center.zw);
		
		half edge = 1.0;
		
		edge *= CheckSame(centerNormal, centerDepth, sample1);
		edge *= CheckSame(centerNormal, centerDepth, sample2);
			
		return edge * lerp(original, _BgColor, _BgFade);
	}
	
	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vertThin
      #pragma fragment fragThin
      ENDCG
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vertRobert
      #pragma fragment fragRobert
      ENDCG
  }

}

Fallback off
	
} // shader