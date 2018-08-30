Shader "BRDG/FxShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Fx[16];
            float _Hit[16];
            float _Volume;
            float _Knob[8];

            // Utils
            float2 rot(float2 st, float t)
            {
                float c = cos(t), s = sin(t);
                return mul(float2x2(c, -s, s, c), st);
            }
            float random(float2 st)
            {
                return frac(sin(dot(st, float2(329., 4938.))) * 23043.);
            }
            float noise(float2 st)
            {
                float2 i = floor(st);
                float2 f = frac(st);

                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));

                float2 u = f*f*(3.0-2.0*f);
                return lerp(a, b, u.x) +
                        (c - a)* u.y * (1.0 - u.x) +
                        (d - b) * u.x * u.y;
            }
            float3 rgb2hsv(float3 c) {
              float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
              float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

              float d = q.x - min(q.w, q.y);
              float e = 1.0e-10;
              return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c) {
              c = float3(c.x, clamp(c.yz, 0.0, 1.0));
              float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            // Effects
            inline void postRGBShift(inout float2 uv, inout float4 color, in float a)
            {
                float d = 0.1 * sin(_Time.y * 30000.) * _Volume * 3. * a;
                color.r = tex2D(_MainTex, uv + float2(-d, 0)).r;
                color.g = tex2D(_MainTex, uv + float2(0, 0)).g;
                color.b = tex2D(_MainTex, uv + float2(d, 0)).b;
            }

            inline void postRed(inout float2 uv, inout float4 color, in float a)
            {
                color.r = lerp(color.r, 1. - color.g, a * 1.5 * random(_Time.y));
            }

            inline void postRandom(inout float2 uv, inout float4 color, in float a)
            {
                float4 c = color;
                c.g = 1. - color.b;
                c.b = 1. - color.r;
                c.r = 1. - color.b;
                color = lerp(color, c, a * 1.3 * random(_Time.y + 3.));
            }

            inline void postHSV(inout float2 uv, inout float4 color, in float a)
            {
                float3 hsv = rgb2hsv(color.rgb);
                hsv.r += frac(noise(float2(_Time.y, _Time.y * 2.4)) * 10);
                hsv.r += a * noise(float2((uv.x + uv.y) * 3, _Time.y));
                float4 c = float4(hsv2rgb(hsv), color.a);
                color = lerp(color, c, a * 2. * random(_Time.y + 2.));
            }

            inline void postBloom(inout float2 uv, inout float4 color, in float a)
            {
                float4 c = float4(0, 0, 0, 0);
                float d = 0.01;

                for (int i = 0; i < 30; i++)
                {
                    float fi = (float)i;
                    c.r += tex2D(_MainTex, (uv - .5) * (1 - fi * d * 1) + .5).r;
                    c.g += tex2D(_MainTex, (uv - .5) * (1 - fi * d * 2) + .5).g;
                    c.b += tex2D(_MainTex, (uv - .5) * (1 - fi * d * 3) + .5).b;
                }

                c.rgb /= 30.0;
                color += c * _Volume * a * 10.;
            }

            inline void postEdge(inout float2 uv, inout float4 color)
            {
                float4 c = float4(0, 0, 0, 0);

                float gray = length(color.rgb);
                float edge = ddx(gray) + ddy(gray);
                float thre = sin(_Time.y * 328.) * 0.2 + 0.5;
                float k = smoothstep(thre, thre + 0.04, edge * 10.);
                color = float4(k,k,k,1);
            }

            inline void f5(inout float2 uv, inout float4 color)
            {
                float2 uv2 = uv * 2. - 1.;

                uv2 *= uv2;
                uv2 = rot(uv2, _Time.y * 4.);
                uv2 = abs(uv2);

                color.r += step(.8, sin((uv2.x - uv.y) * 20. + _Time.y * 20.));
                color.b -= color.r * .3;
            }

            inline void f6(inout float2 uv, inout float4 color)
            {
                float2 uv2 = uv * 2. - 1.;
                float4 c = float4(0, 0, 0, 0);

                uv2.x += random(_Time.y * .007) * .2;
                uv2.y += random(_Time.y * .002) * .1;

                if (_Time.y % 1.2 < .3)
                {
                    uv2 = rot(uv2, _Time.y * 2.+ sin(_Time.y * 3.) * .2);
                }
                else if (_Time.y % 1.8 < .7)
                {
                    uv2 = rot(uv2, 9);
                    uv2.y -= _Time.y *2.;
                }
                else
                {
                    uv2.x *= .3 * sin(_Time.y - 2.);
                }

                c = step(.0, sin((uv2.x - uv2.y) * 30. + _Time.y * 2.));

                uv2 = rot(uv2, _Time.y * .1);
                uv2.x *= .03;
                c += step(.8, sin((uv2.x - uv2.y) * 20. + _Time.y * 2.3));

                color.r += c;
            }

            inline void preCellGlitch(inout float2 uv, inout float4 color, in float a)
            {
                float cell = (7. + a * 4.) + random(_Time.y * .271) * 5.;
                float2 uv2 = floor(uv * cell) + float2(
                    random(_Time.y * .21) - .5,
                    random(_Time.y * .13) - .5
                );

                float n = noise(uv2 + _Time.y);
                // if (n < .6) { return; }

                float t = n * 5.;
                t = max(1. - t, 0);
                float g = (noise(uv2) * 2. - 1.) * 2.4 * t;
                uv.x += g;

                color = tex2D(_MainTex, frac(uv));
                color.r = (1. - color.b) * g * 4.;
            }

            inline void preRandomFocusGlitch(inout float2 uv, inout float4 color, in float a)
            {
                float2 uv0 = uv;

                uv -= .5;
                uv += float2(
                    random(_Time.y * .3) - .5,
                    random(_Time.y * .28) - .5
                ) * 2.;

                float zoom = random(_Time.y * .03) * 13;
                uv /= float2(
                    1 + zoom,
                    1 + zoom + random(_Time.y * .028) * .02
                );
                uv += .5;

                uv = lerp(uv0, uv, a);
            }

            inline void preSlash(inout float2 uv, inout float4 color, in float a)
            {
                uv.x += (uv.y * (1.8 + sin(_Time.y * 349.) * a)) * noise(_Time.y * 10.) * 5. + noise(_Time.y * 3) * a * 30.;
                uv = frac(uv);
                color = tex2D(_MainTex, uv);
            }

            inline void preKaleido(inout float2 uv, inout float4 color, in float a)
            {
                uv -= .5;
                uv = abs(uv);

                if (a % .1 < .03) {
                    uv *= uv;
                }

                float r = _Time.y + noise(float2(_Time.y, 1)) * 2;
                uv = rot(uv - .5 + sin(_Time.y) * .2, r) + .5;
                color = lerp(color, tex2D(_MainTex, uv), a + random(_Time.y * 32.));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,1);
                col = tex2D(_MainTex, i.uv);

                float2 uv = i.uv;

                // Pre FX
                if (_Fx[13] > 0) { preKaleido(uv, col, _Knob[3]); }
                if (_Fx[11] > 0) { preCellGlitch(uv, col, _Knob[3]); }
                if (_Fx[12] > 0) { preRandomFocusGlitch(uv, col, _Knob[4]); }
                if (_Fx[6] > 0) { preSlash(uv, col, _Knob[4]); }

                // Post FX
                if (_Fx[0] > 0) { postRGBShift(uv, col, _Knob[0] * 10); }
                if (_Fx[4] > 0) { postEdge(uv, col); }
                if (_Fx[1] > 0) { postRed(uv, col, _Knob[2]); }
                if (_Fx[2] > 0) { postRandom(uv, col, _Knob[2]); }
                if (_Fx[5] > 0) { postHSV(uv, col, _Knob[2]); }
                if (_Fx[3] > 0) { postBloom(uv, col, _Knob[1] * 10); }

                if (_Fx[9] > 0) { f5(uv, col); }
                if (_Fx[10] > 0) { f6(uv, col); }

                return col;
            }
            ENDCG
        }
    }
}
