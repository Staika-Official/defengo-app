Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Speed ("Speed", Range(-1, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard 

        sampler2D _MainTex;
		float _Speed;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_Tex_Noise;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
 	        fixed4 c = tex2D(_MainTex, IN.uv_MainTex + (_Time.y* _Speed));
 	        o.Emission = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
