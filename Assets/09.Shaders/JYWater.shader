Shader "Custom/JYWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2("WaveTexture",2D) = "white" {}
        _WaveX("WaveX",Range(0,1)) = 0.5
        _WaveY("WaveY",Range(0,1)) = 0.5
        _Brightness("Brightness",Range(0,1)) =0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
             }
        Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MainTex2;
            float4 _MainTex2_ST;
            float _WaveX;
            float _Brightness;
            float _WaveY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex2);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //물이 가로로 왔다갔다 하는 정도
                i.uv.x += sin(_Time.y * _WaveX)*0.2;
                //파도의 밝기 조절
                float b = saturate(sin(_Time.y * 2)) * 0.05 + _Brightness;
                //서브텍스쳐 정의 y 방향으로 흐른다
                fixed4 c = tex2D(_MainTex2, float2(i.uv.x, i.uv.y+_Time.y*_WaveY*0.5));
                //메인 텍스쳐 정의 서브텍스쳐의 r값으로 uv 움직이고 b로 밝기 조절
                fixed4 col = tex2D(_MainTex, float2(i.uv.x+c.r*0.3,i.uv.y+c.r*0.7+_Time.y*_WaveY))*b;
                return col;
            }
            ENDCG
        }
    }
}
