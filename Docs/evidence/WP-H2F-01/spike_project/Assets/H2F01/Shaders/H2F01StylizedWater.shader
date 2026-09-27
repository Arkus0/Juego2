// H2F-01 S03 spike: project-owned, dependency-free URP water (depth tint, broken shoreline foam,
// flow-aligned procedural ripples, fresnel sky reflection). Non-keeper candidate, not an adopted asset.
Shader "H2F01/StylizedWater"
{
    Properties
    {
        _ShallowColor ("Shallow", Color) = (0.37, 0.47, 0.45, 0.55)
        _DeepColor ("Deep", Color) = (0.09, 0.15, 0.16, 0.95)
        _DepthRange ("Depth Range (m)", Float) = 1.4
        _FoamColor ("Foam", Color) = (0.76, 0.80, 0.78, 1)
        _FoamWidth ("Foam Width (m)", Float) = 0.22
        _FlowDir ("Flow Direction (xz)", Vector) = (1, 0, 0, 0)
        _FlowSpeed ("Flow Speed (m/s)", Float) = 0.35
        _RippleScale ("Ripple Scale", Float) = 1.4
        _RippleStrength ("Ripple Strength", Float) = 0.45
        _Smoothness ("Smoothness", Range(0, 1)) = 0.85
        _TimeOverride ("Time Override (<0 = live)", Float) = -1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _ShallowColor, _DeepColor, _FoamColor;
                float4 _FlowDir;
                float _DepthRange, _FoamWidth, _FlowSpeed, _RippleScale, _RippleStrength, _Smoothness, _TimeOverride;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float fog : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings o;
                VertexPositionInputs p = GetVertexPositionInputs(input.positionOS.xyz);
                o.positionCS = p.positionCS;
                o.positionWS = p.positionWS;
                o.screenPos = ComputeScreenPos(p.positionCS);
                o.fog = ComputeFogFactor(p.positionCS.z);
                return o;
            }

            float Hash(float2 p) { return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453); }
            float Noise(float2 p)
            {
                float2 i = floor(p), f = frac(p);
                float2 u = f * f * (3 - 2 * f);
                return lerp(lerp(Hash(i), Hash(i + float2(1, 0)), u.x), lerp(Hash(i + float2(0, 1)), Hash(i + float2(1, 1)), u.x), u.y);
            }
            float Ripple(float2 p, float t)
            {
                float2 flow = normalize(_FlowDir.xz + 1e-4) * _FlowSpeed * t;
                return Noise((p - flow) * _RippleScale) * 0.6 + Noise((p - flow * 1.3) * _RippleScale * 2.3 + float2(3.1, 1.7)) * 0.4;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float t = _TimeOverride >= 0 ? _TimeOverride : _Time.y;
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float sceneEye = LinearEyeDepth(SampleSceneDepth(uv), _ZBufferParams);
                float thickness = max(0, sceneEye - i.screenPos.w);
                float2 p = i.positionWS.xz;
                const float e = 0.05;
                float h = Ripple(p, t), hx = Ripple(p + float2(e, 0), t), hz = Ripple(p + float2(0, e), t);
                float3 n = normalize(float3((h - hx) / e * _RippleStrength * 0.12, 1, (h - hz) / e * _RippleStrength * 0.12));
                half4 body = lerp(_ShallowColor, _DeepColor, saturate(thickness / _DepthRange));
                Light mainLight = GetMainLight();
                float3 v = normalize(GetWorldSpaceViewDir(i.positionWS));
                float fres = pow(1 - saturate(dot(n, v)), 4);
                float3 refl = GlossyEnvironmentReflection(reflect(-v, n), 1 - _Smoothness, 1);
                float3 hdir = normalize(mainLight.direction + v);
                float spec = pow(saturate(dot(n, hdir)), 160) * 0.45;
                float3 rgb = body.rgb * (mainLight.color * (0.35 + 0.5 * saturate(dot(n, mainLight.direction))) + SampleSH(n) * 0.5);
                rgb = lerp(rgb, refl, fres * 0.55) + spec * mainLight.color;
                float foam = (1 - saturate(thickness / _FoamWidth)) * step(0.38, Noise(p * 5 + t * 0.25));
                rgb = lerp(rgb, _FoamColor.rgb, foam * 0.8);
                float alpha = saturate(body.a + fres * 0.25 + foam * 0.6);
                return half4(MixFog(rgb, i.fog), alpha);
            }
            ENDHLSL
        }
    }
}
