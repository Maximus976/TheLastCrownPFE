Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size ("Blur Size", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _MainTex_ST;
            float _Size;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = float2(_Size / _ScreenParams.x, _Size / _ScreenParams.y);
                fixed4 col = tex2D(_MainTex, i.uv) * 0.36;
                col += tex2D(_MainTex, i.uv + offset) * 0.16;
                col += tex2D(_MainTex, i.uv - offset) * 0.16;
                col += tex2D(_MainTex, i.uv + float2(offset.x, -offset.y)) * 0.16;
                col += tex2D(_MainTex, i.uv - float2(offset.x, -offset.y)) * 0.16;
                return col;
            }
            ENDCG
        }
    }
}
