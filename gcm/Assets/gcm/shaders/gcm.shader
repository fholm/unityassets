Shader "Terrain/gcm" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_slab_origin ("Slab Origin", Vector) = (0, 0, 0, 0)
		_slab_scale  ("Slab Scale", Vector)  = (1, 1, 0, 0)
		_slab_tex_origin  ("Slab Tex Origin", Vector)  = (0, 0, 0, 0)
	}
	SubShader {
	Pass {
		Tags { "RenderType"="Opaque" }
		Cull Back 
		Lighting Off 
		
		CGPROGRAM
		
		#pragma only_renderers d3d9 d3d11
		#pragma exclude_renderers gles
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
		 
		uniform sampler2D _MainTex; 
		uniform float2 _slab_origin;
		uniform float2 _slab_scale;
		uniform float2 _slab_tex_origin;
		uniform float  _tex_width;
		
		struct IN {
		    float3 vertex : POSITION;
		    half2 texcoord : TEXCOORD0;
		};
		
		struct OUT {
		    float4 pos : SV_POSITION; 
		    half2 uv : TEXCOORD0;
		};
		
		OUT vert (IN i) {
		    OUT o;
		    
		    float2 pos = i.vertex.xz;
		    float2 tex_inv = 1.0 / _tex_width;
		   	float2 world_pos = pos * _slab_scale + _slab_origin;
		   	
		    o.uv = tex_inv * pos + _slab_tex_origin;
		    
		    #if SHADER_API_D3D9 
		    o.uv += tex_inv * 0.5;
		    #endif
		    
		    float y = tex2Dlod(_MainTex, float4(o.uv, 0, 0));
		    
		    o.pos = mul(UNITY_MATRIX_VP, float4(world_pos.x, y * 64, world_pos.y, 1));
		
		    return o;
		}
		
		float4 frag (OUT i) : COLOR {  
			return tex2D(_MainTex, i.uv);
		}
		
		ENDCG
	} 
	}
}
