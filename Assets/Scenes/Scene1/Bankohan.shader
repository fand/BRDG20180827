Shader "Raymarching/Bankohan"
{

Properties
{
    [Header(PBS)]
    _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    _Metallic("Metallic", Range(0.0, 1.0)) = 0.5
    _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5

    [Header(Raymarching Settings)]
    _Loop("Loop", Range(1, 100)) = 30
    _MinDistance("Minimum Distance", Range(0.001, 0.1)) = 0.01
    _ShadowLoop("Shadow Loop", Range(1, 100)) = 10
    _ShadowMinDistance("Shadow Minimum Distance", Range(0.001, 0.1)) = 0.01
    _ShadowExtraBias("Shadow Extra Bias", Range(0.0, 1.0)) = 0.01

// @block Properties
// _Color2("Color2", Color) = (1.0, 1.0, 1.0, 1.0)
_AccTime("Accumulated Time", Float) = 0.0
_Size1("Size1", Float) = 1.0
_Size2("Size2", Float) = 1.0
_YoColor("YoColor", Color) = (0, 1, 1, 1)
// @endblock
}

SubShader
{

Tags
{
    "RenderType" = "Opaque"
    "DisableBatching" = "True"
}

Cull Off

CGINCLUDE

#define SPHERICAL_HARMONICS_PER_PIXEL
#define CAMERA_INSIDE_OBJECT

#define DISTANCE_FUNCTION DistanceFunction
#define POST_EFFECT PostEffect
#define PostEffectOutput SurfaceOutputStandard

#include "Assets/uRaymarching/Shaders/Include/Common.cginc"

// @block DistanceFunction
float _AccTime;
float _Size1;
float _Size2;
float _Box;
float4 _YoColor;

inline float2 rotate(float2 st, float a) {
    float c = cos(a), s = sin(a);
    return mul(float2x2(c, -s, s, c), st);
}

inline float smoothMin(float d1, float d2, float k){
    float h = exp(-k * d1) + exp(-k * d2);
    return -log(h) / k;
}

inline float sdBox(float3 p, float b){
    b *= 1.4;
    p += sin(_AccTime * 92) * .2;
    p.xy = rotate(p.xy, _AccTime + b) * (sin(_AccTime * 43.) *.1+ 1.1);
    p.xz = rotate(p.xz, _AccTime * 2. + b) * (sin(_AccTime * 42.) * .04 + 1.1);
    return length(max(abs(p)-b,0.0))-.02;
}

inline float DistanceFunction(float3 p)
{
    float t = _AccTime* .3;

    float3 c1 = float3(0, 0, 0);
    float d1 = _Box == 1 ? 
        sdBox(p, 2.4) :
        length(p - c1) - 2.4 * _Size1;

    float d = 9999.;

    for (int i = 0; i <2; i++) {
        float fi = float(i + t*.3);
        float ti = (1. - pow(1. - frac((t + fi * .2)), 1.5)) * 8.;

        float3 u = float3(rotate(float2(1, 0), (fi + 1.) * .7 * PI), 0);
        u.xy = rotate(u.xy, (fi + 1.) * .3);
        u.xz = rotate(u.xz, (fi * 2. + 1.) * 1.9);
        float3 c2 = u * ti;
        float d2 = _Box == 1 ? 
            sdBox(p - c2, .7) :
            length(p - c2) - .7 * _Size2;

        float a1 = max(dot(normalize(p - c1), normalize(c2 - c1)), 0.);
        float a2 = max(dot(normalize(p - c2), normalize(c1 - c2)), 0.);

        d = smoothMin(
            d,
            smoothMin(
                d1 * (1.1 - pow(a1, 15.)),
                d2 * (1.1 - pow(a2, 15.)),
                2.2
            ),
            2.5
        );

        for (int j = 0; j < 2; j++) {
            float fj = float(j + t + i);
            float tj = (1. - pow(1. - max(ti - 5. + fj * .8, 0.) / 3., 3.)) * 4.;

            float3 uj = float3(rotate(float2(1, 0), (fj + 1.) * .4 * PI + 1.5 * PI), 0);
            uj.xy = rotate(uj.xy, (fi + 1.) * 1.3);
            uj.xz = rotate(uj.xz, (fi * 3. + 1.) * .9);

            float3 cj = c2 + uj * tj;
            float dj = length(p - cj) - .3 * (_Size1 * _Size2);

            float aj1 = max(dot(normalize(p - c2), normalize(cj - c2)), 0.);
            float aj2 = max(dot(normalize(p - cj), normalize(c2 - cj)), 0.);

            d = smoothMin(
                d,
                smoothMin(
                    d2 * (1.1 - pow(aj1, 10.)),
                    dj * (1.1 - pow(aj2, 10.)),
                    4.3
                ),
                3.6
            );
        }
    }

    return d;
}
// @endblock

// @block PostEffect
inline void PostEffect(RaymarchInfo ray, inout PostEffectOutput o)
{
    o.Emission = _YoColor;
}
// @endblock

#include "Assets/uRaymarching/Shaders/Include/Raymarching.cginc"

ENDCG

Pass
{
    Tags { "LightMode" = "Deferred" }

    Stencil
    {
        Comp Always
        Pass Replace
        Ref 128
    }

    CGPROGRAM
    #include "Assets/uRaymarching/Shaders/Include/VertFragStandardObject.cginc"
    #pragma target 3.0
    #pragma vertex Vert
    #pragma fragment Frag
    #pragma exclude_renderers nomrt
    #pragma multi_compile_prepassfinal
    #pragma multi_compile ___ UNITY_HDR_ON
    #pragma multi_compile OBJECT_SHAPE_CUBE OBJECT_SHAPE_SPHERE ___
    ENDCG
}

Pass
{
    Tags { "LightMode" = "ShadowCaster" }

    CGPROGRAM
    #include "Assets/uRaymarching/Shaders/Include/VertFragShadowObject.cginc"
    #pragma target 3.0
    #pragma vertex Vert
    #pragma fragment Frag
    #pragma fragmentoption ARB_precision_hint_fastest
    #pragma multi_compile_shadowcaster
    #pragma multi_compile OBJECT_SHAPE_CUBE OBJECT_SHAPE_SPHERE ___
    ENDCG
}

}

Fallback "Raymarching/Fallbacks/StandardSurfaceShader"

CustomEditor "uShaderTemplate.MaterialEditor"

}