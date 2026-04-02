Shader "UI/DiscoBall"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Vector) = (0.5, 0, 0, 0)
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Sphere)]
        _CircleRadius ("Circle Radius", Range(0, 0.5)) = 0.45
        _SphereStrength ("Sphere Strength", Range(0.01, 1.5)) = 0.8

        [Header(Emission Outline)]
        [HDR] _EmissionColor ("Emission Color", Color) = (1, 0.8, 0.2, 1)
        _EmissionWidth ("Emission Width", Range(0, 0.15)) = 0.05
        _CircleSoftness ("Edge Softness", Range(0, 0.1)) = 0.01

        [Header(Stencil)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 rawUV : TEXCOORD1;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _ScrollSpeed;
                half4 _Color;
                float _CircleRadius;
                float _CircleSoftness;
                half4 _EmissionColor;
                float _EmissionWidth;
                float _SphereStrength;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex) + _ScrollSpeed.xy * _Time.y;
                output.rawUV = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 center = input.rawUV - 0.5;
                float dist = length(center);

                // Spherical UV distortion - stronger at edges
                float2 sphereUV = input.uv;
                float r = saturate(dist / _CircleRadius);
                if (r < 1.0)
                {
                    float theta = asin(r);
                    float mapped = tan(theta * _SphereStrength) / tan(_SphereStrength);
                    float2 dir = center / max(dist, 0.001);
                    sphereUV = input.uv + dir * (mapped - r) * _CircleRadius;
                }

float halfSoft = _CircleSoftness * 0.5;
float innerEdge = _CircleRadius - _EmissionWidth + halfSoft;
float outerEdge = _CircleRadius - halfSoft;

float textureMask = 1.0 - smoothstep(innerEdge - _CircleSoftness, innerEdge, dist);
float circleMask = 1.0 - smoothstep(outerEdge, outerEdge + _CircleSoftness, dist);
float emissionMask = smoothstep(innerEdge - _CircleSoftness, innerEdge, dist)
                   * (1.0 - smoothstep(outerEdge, outerEdge + _CircleSoftness, dist));

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sphereUV) * input.color;
                tex.rgb = tex.rgb * textureMask + _EmissionColor.rgb * emissionMask;
                tex.a *= circleMask;

                return tex;
            }
            ENDHLSL
        }
    }
}