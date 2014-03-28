Shader "Transparent Effects/CheapForcefield2" 

{

 

Properties

 {

     _Color("_Color", Color) = (0,1,0,1)

     _Rim("_Rim", Range(0,4) ) = 1.2

     _Texture("_Texture", 2D) = "white" {}

     _Speed("_Speed", Range(0.5,5) ) = 0.5

     _Tile("_Tile", Range(1,10) ) = 5.0

     _Strength("_Strength", Range(0,5) ) = 1.5

     _Properties("Rim, Strength, Speed, Tile ", Vector) = (1.2, 1.5, 0.5, 5.0)

 }

    

 SubShader

 {

    Tags

    {

     "Queue"="Transparent"

     "RenderType"="Transparent"

    }

 

    Cull Back

    ZWrite On

    ZTest LEqual

    

    CGPROGRAM

    #pragma surface surf BlinnPhong alpha

 

        fixed4 _Color;

        sampler2D _Texture;

        fixed4 _Properties;

        

        struct Input 

        {

            half4 screenPos;

            half3 viewDir;

            half2 uv_Texture;

        };

 

        inline fixed Fresnel(half3 viewDir)

        {

 

            return fixed(1.0) - dot( normalize(viewDir), fixed3(0.0,0.0,1.0));

        }

 

        void surf (Input IN, inout SurfaceOutput o) 

        {

            // fresnel related effects

            fixed stepfresnel = step( Fresnel( IN.viewDir ) , fixed(1.0));

            fixed rimContrib = pow( Fresnel( IN.viewDir ) , _Properties.x );

            

            // calculate texture coords

            half2 texCoords = half2( IN.uv_Texture.x , IN.uv_Texture.y + half(_Time.x) * _Properties.z ) * _Properties.ww;

            

            // and get the texture contribution

            fixed  texContrib = tex2D (_Texture, texCoords).x * _Properties.y;

            

            // put the contributions together into the alpha

            o.Alpha = texContrib * rimContrib  * _Color.a;

            

            // set the colours

            o.Albedo = fixed3(0.0,0.0,0.0);

            o.Emission = _Color.rgb;

            o.Normal = fixed3(0.0,0.0,1.0);

        }

    ENDCG

    } 

    Fallback "Diffuse"

}