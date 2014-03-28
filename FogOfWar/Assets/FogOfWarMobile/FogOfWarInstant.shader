Shader "Mobile/Fog of War (Instant - Faded)" {
  Properties {

  }
  Subshader {
	Tags { "Queue"="Overlay-50" "RenderType"="Transparent" }

	Pass {	
		Cull Back 
		Lighting Off 
		ZWrite Off
		ZTest Always
		Blend Zero OneMinusSrcAlpha
		Fog { Mode Off }
		
		CGPROGRAM
		
		#pragma vertex vert_img
		#pragma fragment frag

		struct appdata_img {
			float4 vertex : POSITION;
			half2 texcoord : TEXCOORD0;
		};

		struct v2f_img {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f_img vert_img( appdata_img v )
		{
			v2f_img o;

			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord;

			return o;
		}

		float4 frag (v2f_img i) : COLOR
		{	
			return float4(1, 1, 1, i.uv.x);
		}

		ENDCG
	}
  }
}