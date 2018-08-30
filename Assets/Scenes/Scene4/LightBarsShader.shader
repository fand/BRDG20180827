Shader "Unlit/LightBarsShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightWidth ("LightWidth", Range(0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _LightWidth;
            float _LightRed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Show UV
                col = float4(i.uv, 1, 1);

                float t = _Time.y * .9;
                float _Ratio = .94 - _LightWidth * 1.94;
                float bord = smoothstep(_Ratio, _Ratio + .02, sin(i.uv.y * 20 * 3.141592));
                float xwave = frac(i.uv.x * 2 + t + sin(i.uv.y * 8) * 2);
                xwave = smoothstep(.4, .5, xwave) * (1 - smoothstep(.5, .6, xwave));
                float k = bord * xwave;

                col.bg -= _LightRed;
                col.r += _LightRed;

                col.a *= k;

                return col;
            }
            ENDCG
        }
    }
}
