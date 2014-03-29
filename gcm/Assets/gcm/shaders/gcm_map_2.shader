Shader "Terrain/gcm (map2)" {
  Properties {
	_MainTex ("Main", 2D) = "white" {}
	_Input ("Input", Vector) = (0,0,0,0)
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
		 
		uniform float4 _Input;
		uniform sampler2D _MainTex; 
		
		#define START_X _Input.x
		#define START_Y _Input.y
		#define TEXTURE_WIDTH _Input.z
		#define TEXEL_WIDTH _Input.w
		
		struct IN {
		    float4 vertex : POSITION;
		    float2 texcoord : TEXCOORD0;
		};
		
		struct OUT {
		    float4 pos : SV_POSITION; 
		    float2 texcoord : TEXCOORD0;
		};
		
		OUT vert (IN i) {
		 	float2 uv = float2(0, 0);
			uv.x = START_X + (TEXTURE_WIDTH * i.texcoord.x);
			uv.y = START_Y + (TEXTURE_WIDTH * i.texcoord.y);
			 
		    #if SHADER_API_D3D9 
		    uv -= TEXEL_WIDTH * 0.5f;
		    #endif
		    
		    OUT o; 
		    o.pos = mul(UNITY_MATRIX_MVP, i.vertex); 
			o.texcoord = uv;  
		    return o;
		}
		
		float4 frag (OUT i) : COLOR {  
		    return tex2D(_MainTex, i.texcoord);
		}
		
		ENDCG
  	}
  }
}