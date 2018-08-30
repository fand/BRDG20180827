Shader "Unlit/StripesShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Zebra ("Zebra", Range(1, 10)) = 10
        _Speed ("Speed", Range(0.01, 3)) = 1.0
        _Color ("Color", Color) = (1, 0, 0.3)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Zebra;
            float _Speed;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float ste = .5;// + sin(_Time.y * .3) * .5;
                float k = step(ste, sin((uv.x + uv.y + _Time.y * _Speed) * 10 * _Zebra));
                float4 col = float4(0, 0, 0, k);
                if (k == 1)
                {
                    col = _Color;
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
