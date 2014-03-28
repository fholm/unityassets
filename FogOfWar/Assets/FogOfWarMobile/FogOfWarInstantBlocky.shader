Shader "Mobile/Fog of War (Instant - Blocky)" {
  Properties {
	_WorldTime ("WorldTime", float) = 0
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
		
		#pragma vertex fow_vert
		#pragma fragment fow_frag

		float _WorldTime;

		struct fow_appdata_img {
			float4 vertex : POSITION;
			half2 texcoord : TEXCOORD0;
		};

		struct fow_v2f_img {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		fow_v2f_img fow_vert( fow_appdata_img v )
		{
			fow_v2f_img o;

			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv.x = v.texcoord.x;

			return o;
		}

		float4 fow_frag (fow_v2f_img i) : COLOR
		{	
			return float4(1, 1, 1, clamp((_WorldTime - i.uv.x) / 0.1, 0, 0.75));
		}

		ENDCG
	}
  }
}