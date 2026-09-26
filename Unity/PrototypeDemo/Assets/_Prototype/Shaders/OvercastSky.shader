Shader "Proto/OvercastSky"
{
    Properties
    {
        _Top ("Zenith", Color) = (0.55, 0.61, 0.66, 1)
        _Horizon ("Horizon", Color) = (0.70, 0.75, 0.77, 1)
        _Bottom ("Below horizon", Color) = (0.52, 0.56, 0.57, 1)
        _CloudDark ("Cloud dark", Color) = (0.50, 0.54, 0.58, 1)
        _CloudLight ("Cloud light", Color) = (0.80, 0.83, 0.85, 1)
        _Scale ("Cloud scale", Float) = 1.1
        _Cover ("Cloud cover", Range(0, 1)) = 0.62
        _Speed ("Drift", Float) = 0.004
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _Top, _Horizon, _Bottom, _CloudDark, _CloudLight;
            float _Scale, _Cover, _Speed;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 dir : TEXCOORD0; };

            Varyings vert (Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.dir = i.positionOS.xyz;
                return o;
            }

            float hash21 (float2 p) { p = frac(p * float2(123.34, 456.21)); p += dot(p, p + 45.32); return frac(p.x * p.y); }
            float vnoise (float2 p)
            {
                float2 i = floor(p), f = frac(p);
                float2 u = f * f * (3 - 2 * f);
                return lerp(lerp(hash21(i), hash21(i + float2(1, 0)), u.x), lerp(hash21(i + float2(0, 1)), hash21(i + float2(1, 1)), u.x), u.y);
            }
            float fbm (float2 p)
            {
                float s = 0, a = 0.5;
                for (int k = 0; k < 5; k++) { s += a * vnoise(p); p = p * 2.03 + 17.1; a *= 0.5; }
                return s;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 d = normalize(i.dir);
                float y = d.y;
                float3 col = y > 0 ? lerp(_Horizon.rgb, _Top.rgb, pow(saturate(y), 0.55)) : lerp(_Horizon.rgb, _Bottom.rgb, saturate(-y * 5));
                if (y > -0.02)
                {
                    float2 uv = d.xz / (max(y, 0) + 0.10) * _Scale + _Time.y * _Speed;
                    float n = fbm(uv);
                    float c = smoothstep(1 - _Cover - 0.15, 1 - _Cover + 0.25, n);
                    float shade = fbm(uv * 2.1 + 3.7);
                    float3 cl = lerp(_CloudDark.rgb, _CloudLight.rgb, saturate(shade * 1.2 - 0.1));
                    float fade = saturate(y * 7 + 0.1);
                    col = lerp(col, cl, c * fade * 0.9);
                    col = lerp(col, _Horizon.rgb, (1 - saturate(y * 4)) * 0.55);
                }
                return half4(col, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
