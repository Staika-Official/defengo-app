Shader "Unlit/DisabledIconShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha",Range(0,1)) = 1
        _Color ("MulColor",Color) = (0,0,0,1)
        _Multiplier("Multiplier",Range(0,1)) = 1

    }
    SubShader
    {
         Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }    //! 쉐이더 타입을 Transparent로 변경, Render Queue도 같이
        Blend SrcAlpha OneMinusSrcAlpha
 
        LOD 100

        Pass
        {
            

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  

            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 color : COLOR;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Alpha;
            fixed4 _Color;
            float _Multiplier;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col*_Color;
            }
            ENDCG
        }
    }
}
