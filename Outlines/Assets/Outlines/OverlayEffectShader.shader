Shader "FFG/Overlay/Image Effect" {
  Properties{
    _MainTex("Render Input", 2D) = "white" {}
    _OverlayTex("Overlay Mask", 2D) = "black" {}
  }
    SubShader{
      ZTest Always Cull Off ZWrite Off Fog { Mode Off }
      Pass {
        CGPROGRAM
          #pragma vertex vert_img
          #pragma fragment frag

          #include "UnityCG.cginc"

          float _ScreenWidth;
          float _ScreenHeight;

          sampler2D _MainTex;
          sampler2D _OverlayTex;

          float4 frag(v2f_img IN) : COLOR {
            return tex2D(_MainTex, IN.uv);
          }
        ENDCG
      }
      Pass {
        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
          #pragma vertex vert_img
          #pragma fragment frag

          #include "UnityCG.cginc"

          float _ScreenWidth;
          float _ScreenHeight;
          float _MinCompare;

          sampler2D _MainTex;
          sampler2D _OverlayTex;

          float4 frag(v2f_img IN) : COLOR {
            float tx = 1.0 / _ScreenWidth;
            float ty = 1.0 / _ScreenHeight;

            // up
            float3 u = tex2D(_OverlayTex, IN.uv + float2(0, ty));

            // down
            float3 d = tex2D(_OverlayTex, IN.uv + float2(0, -ty));

            // left
            float3 l = tex2D(_OverlayTex, IN.uv + float2(-tx, 0));

            // right
            float3 r = tex2D(_OverlayTex, IN.uv + float2(+tx, 0));

            // sum
            float s = any(u) + any(d) + any(l) + any(r);

            // calculate
            return float4(max(max(max(u, d), l), r), (s > _MinCompare) && (s < 4));
          }
        ENDCG
      }
  }
}