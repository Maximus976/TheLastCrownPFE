Shader "Unlit/Fog"

{
    Properties
    {
        _MainTex ("Fog Texture", 2D) = "white" {}
        _MaskPos ("Mask Position", Vector) = (0,0,0,1)
        _MaskRadius ("Mask Radius", Range(0.1, 10)) = 3
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Gère la transparence
            ZWrite Off // Évite les problèmes d'affichage
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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _MaskPos;
            float _MaskRadius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Coordonnées mondiales
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calcule la distance entre chaque pixel et le joueur
                float dist = distance(i.worldPos, _MaskPos);

                // Crée une transition douce pour la dispersion de la brume
                float mask = smoothstep(_MaskRadius, _MaskRadius * 0.5, dist); 

                // Applique la texture et le masque de transparence
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= mask; // Rend la brume transparente autour du joueur

                return col;
            }
            ENDCG
        }
    }
}