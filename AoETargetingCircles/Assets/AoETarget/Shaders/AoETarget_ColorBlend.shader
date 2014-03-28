Shader "AoE Target/Color Blend" {
  Properties {
  	  _Color ("Main Color", Color) = (1,1,1,1)   	
      _Tex ("Texture", 2D) = "" { TexGen ObjectLinear }
  }
  Subshader {
     Pass {
        ZWrite off
        Fog { Color (0, 0, 0) }
        Color [_Color]
        ColorMask RGB
        Blend One OneMinusSrcAlpha
		Offset -1, -1
        SetTexture [_Tex] {
		   combine texture * primary, ONE - texture
           Matrix [_Projector]
        }
     }
  }
}