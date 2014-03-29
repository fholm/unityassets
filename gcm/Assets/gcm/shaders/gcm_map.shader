Shader "Terrain/gcm (map)" {
  Properties {
	_MainTex ("Main", 2D) = "white" {}
  }
  Subshader {
	Pass {        
		Cull Back 
		Lighting Off 
		ZWrite Off
		ZTest Always
		Fog { Mode Off }
		
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		 
		uniform float2 _Pos;
		uniform float4 _Input;
		uniform sampler2D _MainTex; 
		
		#define POS _Pos
		#define LEVEL_SCALE _Input.x
		#define SRC_SIZE _Input.y
		#define DST_SIZE _Input.z 
		#define DST_HALF_SIZE _Input.w
		
		struct IN {
		    float4 vertex : POSITION;
		    float2 texcoord : TEXCOORD0;
		};
		
		struct OUT {
		    float4 pos : SV_POSITION; 
		    float2 uv : TEXCOORD0;
		};
		
		OUT vert (IN i) {
		    OUT o; 
		    
		    // TODO, simplify - this uses way to many operations and is super redundant
		 	
		 	float2 uv = float2(0, 0);
			uv.x = (POS.x - (DST_HALF_SIZE * LEVEL_SCALE)) / SRC_SIZE;
			uv.y = (POS.y - (DST_HALF_SIZE * LEVEL_SCALE)) / SRC_SIZE;
			uv.x = uv.x + (i.texcoord.x * (DST_SIZE * LEVEL_SCALE / SRC_SIZE)); 
			uv.y = uv.y + (i.texcoord.y * (DST_SIZE * LEVEL_SCALE / SRC_SIZE)); 
			 
		    #if SHADER_API_D3D9 
		    uv -= (1.0 / DST_SIZE) * 0.5;
		    #endif
		    
		    o.pos = mul(UNITY_MATRIX_MVP, i.vertex); 
			o.uv = uv;  
			
		    return o;
		}
		
		float4 frag (OUT i) : COLOR {  
		    return tex2D(_MainTex, i.uv);
		}
		
		ENDCG
  	}
  }
}