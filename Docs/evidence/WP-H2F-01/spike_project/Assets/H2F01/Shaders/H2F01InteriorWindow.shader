// H2F-01 S05 spike: project-owned "interior mapping" for NON-ENTERABLE windows only. A world-space room box is
// ray-cast behind the glass plane (no textures, no plugin). Never used for an enterable/playable opening.
Shader "H2F01/InteriorWindow"
{
    Properties
    {
        _RoomOrigin ("Glass centre (world)", Vector) = (0, 0, 0, 0)
        _RoomRight ("Facade right axis (world)", Vector) = (0, 0, 1, 0)
        _RoomIn ("Inward axis (world)", Vector) = (1, 0, 0, 0)
        _RoomSize ("Room width, height below sill-centre, height above, depth", Vector) = (3.6, 1.35, 1.45, 3.2)
        _Wall ("Wall", Color) = (0.55, 0.50, 0.42, 1)
        _Floor ("Floor", Color) = (0.30, 0.22, 0.16, 1)
        _Ceiling ("Ceiling", Color) = (0.48, 0.44, 0.38, 1)
        _Lamp ("Interior light", Color) = (1.0, 0.78, 0.52, 1)
        _Curtain ("Curtain", Color) = (0.42, 0.20, 0.16, 1)
        _Glass ("Glass tint/reflect", Color) = (0.55, 0.62, 0.66, 0.28)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+10" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _RoomOrigin, _RoomRight, _RoomIn, _RoomSize;
                half4 _Wall, _Floor, _Ceiling, _Lamp, _Curtain, _Glass;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 positionWS : TEXCOORD0; float fog : TEXCOORD1; };

            Varyings vert(Attributes input)
            {
                Varyings o;
                VertexPositionInputs p = GetVertexPositionInputs(input.positionOS.xyz);
                o.positionCS = p.positionCS; o.positionWS = p.positionWS; o.fog = ComputeFogFactor(p.positionCS.z);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float3 R = normalize(_RoomRight.xyz), U = float3(0, 1, 0), N = normalize(_RoomIn.xyz);
                float3 rel = i.positionWS - _RoomOrigin.xyz;
                float3 o = float3(dot(rel, R), dot(rel, U), dot(rel, N));
                float3 viewDir = normalize(i.positionWS - _WorldSpaceCameraPos);
                float3 d = float3(dot(viewDir, R), dot(viewDir, U), dot(viewDir, N));
                float halfW = _RoomSize.x * 0.5;
                float tx = ((d.x > 0 ? halfW : -halfW) - o.x) / d.x;
                float ty = ((d.y > 0 ? _RoomSize.z : -_RoomSize.y) - o.y) / d.y;
                float tz = (_RoomSize.w - o.z) / max(d.z, 1e-4);
                float t = min(tx, min(ty, tz));
                float3 hitP = o + d * t;
                half3 col;
                if (t == ty) col = d.y > 0 ? _Ceiling.rgb : _Floor.rgb * (0.8 + 0.2 * step(0.5, frac(hitP.x * 2.5)));
                else col = _Wall.rgb;
                if (t == tz)
                {
                    // back wall: darker doorway and a small framed picture
                    float door = step(abs(hitP.x - halfW * 0.45), 0.45) * step(hitP.y, 0.8);
                    float pic = step(abs(hitP.x + halfW * 0.35), 0.35) * step(abs(hitP.y - 0.45), 0.25);
                    col = lerp(col, col * 0.25, door);
                    col = lerp(col, half3(0.30, 0.34, 0.30), pic);
                }
                float depthFade = saturate(t * dot(d, float3(0, 0, 1)) / _RoomSize.w);
                float lampGlow = exp(-dot(hitP.xy - float2(0, _RoomSize.z * 0.8), hitP.xy - float2(0, _RoomSize.z * 0.8)) * 0.35);
                col *= lerp(0.95, 0.45, depthFade);
                col += _Lamp.rgb * lampGlow * 0.35;
                // curtain band on the glass plane itself (local o.x/o.y on the pane)
                float curtain = step(0.52, o.y) + step(0.55, abs(o.x)) * step(-0.1, o.y) * 0.8;
                col = lerp(col, _Curtain.rgb * 0.8, saturate(curtain));
                // glass: fresnel sky reflection over the fake room
                float3 n = -N;
                float fres = pow(1 - saturate(dot(n, -viewDir)), 3);
                float3 refl = GlossyEnvironmentReflection(reflect(viewDir, n), 0.1, 1);
                col = lerp(col, refl * _Glass.rgb * 1.5, saturate(_Glass.a + fres * 0.6));
                return half4(MixFog(col, i.fog), 1);
            }
            ENDHLSL
        }
    }
}
