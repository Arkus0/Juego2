using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>
    /// S03 river spike on the pinned ART-01 river/bank/bridge: (A) ART's flat water converted to URP Lit versus
    /// (B) a project-owned dependency-free URP water shader (depth tint, shoreline foam, flow ripples, fresnel).
    /// Paid Stylized Water 3 is not bought or imported.
    /// </summary>
    public static class H2F01WaterSpike
    {
        [Serializable]
        class Result
        {
            public string waterObjects; public Vector3 boundsCenter, boundsSize; public string riverAxis;
            public string shader; public bool shaderSupported; public int passes; public bool depthTexture;
            public float flowPixelDelta; public bool auditGreen; public List<string> notes = new List<string>();
        }

        public static void Run()
        {
            var asset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
            if (asset == null) throw new Exception("H2F01_URP_REQUIRED");
            asset.supportsCameraDepthTexture = true; // required by any depth-faded water/shoreline route
            EditorUtility.SetDirty(asset);
            var r = new Result { depthTexture = asset.supportsCameraDepthTexture };
            OpenArtScene();
            H2F01RenderSpike.BaselineLook();
            H2F01RenderSpike.PoseHumans(H2F01RenderSpike.UalClip("Idle_Loop"));
            var water = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None)
                .Where(x => x.sharedMaterials.Any(m => m != null && m.name == "Water")).ToList();
            if (water.Count == 0) throw new Exception("H2F01_ART_WATER_NOT_FOUND");
            var b = water.Select(x => x.bounds).Aggregate((a, c) => { a.Encapsulate(c); return a; });
            r.waterObjects = string.Join(",", water.Select(x => x.name));
            r.boundsCenter = b.center; r.boundsSize = b.size;
            bool alongX = b.size.x >= b.size.z;
            r.riverAxis = alongX ? "X" : "Z";
            var flow = alongX ? Vector3.right : Vector3.forward;
            var across = alongX ? Vector3.forward : Vector3.right;
            float wy = b.max.y;
            var views = new (string id, Vector3 pos, Vector3 target)[]
            {
                ("bridge_third_person", new Vector3(0, RoadY(0, b.center.z) + 1.9f, b.center.z) - across * 1.2f + flow * 0.5f, new Vector3(b.center.x, wy, b.center.z) + flow * 14f),
                ("bank_low", new Vector3(b.center.x, wy + 0.7f, b.center.z) + flow * 9f + across * (alongX ? b.extents.z : b.extents.x) * 0.95f, new Vector3(b.center.x, wy, b.center.z) + flow * 2f),
                ("arch_embankment", new Vector3(-13.5f, 0.2f, -35.5f), new Vector3(-2.0f, -1.1f, -28.2f)),
            };
            r.notes.Add($"views derived from ART water bounds; flow along {r.riverAxis}");
            Shoot("s03/A_flat", views);
            var mat = new Material(AssetDatabase.LoadAssetAtPath<Shader>("Assets/H2F01/Shaders/H2F01StylizedWater.shader"));
            r.shader = mat.shader.name; r.shaderSupported = mat.shader.isSupported; r.passes = mat.passCount;
            mat.SetVector("_FlowDir", new Vector4(flow.x, 0, flow.z, 0));
            mat.SetFloat("_TimeOverride", 0f);
            foreach (var w in water) w.sharedMaterials = w.sharedMaterials.Select(m => m != null && m.name == "Water" ? mat : m).ToArray();
            Shoot("s03/B_stylized", views);
            var audit = AuditShaders();
            r.auditGreen = audit.green;
            // flow check: same view, shader time 0 vs 1.5 s -> water pixels must change
            mat.SetFloat("_TimeOverride", 1.5f);
            Shoot("s03/B_stylized_t1p5", views.Take(1).ToArray());
            r.flowPixelDelta = MeanDelta(Out("s03/B_stylized_bridge_third_person.png"), Out("s03/B_stylized_t1p5_bridge_third_person.png"));
            AnimationMode.StopAnimationMode();
            WriteJson("s03/result.json", JsonUtility.ToJson(r, true));
            WriteJson("s03/audit_B.json", JsonUtility.ToJson(audit, true));
            Log($"H2F01_S03_DONE water={r.waterObjects} axis={r.riverAxis} shaderSupported={r.shaderSupported} audit={r.auditGreen} flowDelta={r.flowPixelDelta:F4}");
        }

        static float RoadY(float x, float z) => Physics.Raycast(new Vector3(x, 40, z), Vector3.down, out var h, 80) ? h.point.y : 0;

        static void Shoot(string prefix, (string id, Vector3 pos, Vector3 target)[] views)
        {
            foreach (var v in views)
            {
                var cam = CameraAt("s03_cam", v.pos, v.target, 55);
                cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
                Image(cam, $"{prefix}_{v.id}.png");
                UnityEngine.Object.DestroyImmediate(cam.gameObject);
            }
        }

        static float MeanDelta(string a, string b)
        {
            var ta = new Texture2D(2, 2); ta.LoadImage(System.IO.File.ReadAllBytes(a));
            var tb = new Texture2D(2, 2); tb.LoadImage(System.IO.File.ReadAllBytes(b));
            var pa = ta.GetPixels(); var pb = tb.GetPixels();
            double sum = 0;
            for (int i = 0; i < pa.Length; i++) sum += Mathf.Abs(pa[i].grayscale - pb[i].grayscale);
            return (float)(sum / pa.Length);
        }
    }
}
