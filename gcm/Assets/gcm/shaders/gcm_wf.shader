Shader "Terrain/gcm_wireframe" {
	Properties {
	}
	SubShader {
	Pass {
		Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		Lighting Off 
		ZWrite Off
		ZTest Always
		
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		
		float4 _Color;
		 
		struct IN {
		    float4 vertex : POSITION;
		};
		
		struct OUT {
		    float4 pos : SV_POSITION; 
		};
		
		OUT vert (IN i) {
			OUT o;
			o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
			return o;
		}
		
		float4 frag (OUT i) : COLOR {  
			return _Color;
		}
		
		ENDCG
	} 
	}
}
