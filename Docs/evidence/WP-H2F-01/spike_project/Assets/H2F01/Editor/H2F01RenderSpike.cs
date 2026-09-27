using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Juego2.ART;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>
    /// S01 render spike. Reproduces the pinned ART-01 structural benchmark, captures it in Built-in,
    /// then compares URP routes on the same geometry: (A) the official Built-in->URP converter on ART's
    /// palette materials, (B) Quaternius Source URP-native Shader Graph materials, (C) A plus Source
    /// normal maps. Non-keeper, disposable workspace only.
    /// </summary>
    public static class H2F01RenderSpike
    {
        const string ExpectedDigest = "a7f8534f33a46a189c5afb726125b01b6ab0eeba5907cbc3ba4aaafb8e30601e";
        const string Settings = "Assets/H2F01/Settings";
        const string SourceMats = "Assets/H2F01Inputs/QuaterniusURP/Materials";

        static readonly (string id, Vector3 pos, Vector3 target)[] Views =
        {
            ("puente_s02", new Vector3(0, 1.65f, -35), new Vector3(0, 2.1f, -16)),
            ("w12_casco", new Vector3(0, 1.65f, -10), new Vector3(0, 2.2f, 13)),
            ("roof_eave_corner", new Vector3(3.6f, 2.2f, -9.6f), new Vector3(1.8f, 6.6f, -4.0f)),
            ("facade_stone_close", new Vector3(1.2f, 1.6f, -8.5f), new Vector3(-2.4f, 1.8f, -6.5f)),
            ("f01_exterior", new Vector3(0, 1.65f, 12), new Vector3(0, 3.0f, 28)),
            ("f01_threshold", new Vector3(0, 1.65f, 22.4f), new Vector3(0, 1.3f, 29)),
        };

        // ---------------------------------------------------------------- Built-in reproduction

        public static void Reproduce()
        {
            UsePipeline(null);
            Juego2.ART.Editor.Art01BenchmarkBuilder.Build();
            Juego2.ART.Editor.Art01StructuralAudit.Run();
            var digestPath = Path.Combine(Root, "Docs/evidence/WP-ART-01/STRUCTURAL_SCENE_DIGEST.json");
            var auditPath = Path.Combine(Root, "Docs/evidence/WP-ART-01/UNITY_STRUCTURAL_AUDIT.json");
            var digest = JsonUtility.FromJson<Digest>(File.ReadAllText(digestPath));
            var audit = File.ReadAllText(auditPath);
            bool match = digest.sha256 == ExpectedDigest;
            WriteJson("s01/reproduction.json", JsonUtility.ToJson(new Reproduction
            {
                unityVersion = Application.unityVersion,
                expectedDigest = ExpectedDigest,
                actualDigest = digest.sha256,
                rows = digest.rowCount,
                digestMatch = match,
                auditSummary = audit.Length > 400 ? audit.Substring(0, 400) : audit,
            }, true));
            Log((match ? "H2F01_S01_REPRODUCTION_GREEN" : "H2F01_S01_REPRODUCTION_RED") + " digest=" + digest.sha256 + " rows=" + digest.rowCount);
            if (!match) throw new Exception("H2F01_ART_INPUT_NOT_REPRODUCED");
        }

        [Serializable] class Digest { public string sha256; public int rowCount; }
        [Serializable] class Reproduction { public string unityVersion, expectedDigest, actualDigest, auditSummary; public int rows; public bool digestMatch; }

        public static void CaptureBuiltin()
        {
            UsePipeline(null);
            OpenArtScene();
            Shoot("builtin");
            WriteJson("s01/audit_builtin.json", JsonUtility.ToJson(AuditShaders(), true));
            Log("H2F01_S01_BUILTIN_CAPTURED");
        }

        // ---------------------------------------------------------------- URP setup and routes

        public static void SetupUrp()
        {
            if (!AssetDatabase.IsValidFolder(Settings)) AssetDatabase.CreateFolder("Assets/H2F01", "Settings");
            var rendererPath = Settings + "/H2F01_Renderer.asset";
            var assetPath = Settings + "/H2F01_URP.asset";
            AssetDatabase.DeleteAsset(rendererPath);
            AssetDatabase.DeleteAsset(assetPath);
            var renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
            AssetDatabase.CreateAsset(renderer, rendererPath);
            var ssao = ScriptableObject.CreateInstance<ScreenSpaceAmbientOcclusion>();
            ssao.name = "SSAO";
            AssetDatabase.AddObjectToAsset(ssao, renderer);
            renderer.rendererFeatures.Add(ssao);
            var decal = ScriptableObject.CreateInstance<DecalRendererFeature>();
            decal.name = "Decals";
            AssetDatabase.AddObjectToAsset(decal, renderer);
            renderer.rendererFeatures.Add(decal);
            typeof(ScriptableRendererData).GetMethod("ValidateRendererFeatures", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(renderer, null);
            EditorUtility.SetDirty(renderer);
            var asset = UniversalRenderPipelineAsset.Create(renderer);
            asset.supportsHDR = true;
            asset.msaaSampleCount = 4;
            asset.shadowDistance = 70;
            asset.shadowCascadeCount = 2;
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            UsePipeline(asset);
            Log("H2F01_S01_URP_READY asset=" + assetPath + " features=" + renderer.rendererFeatures.Count);
        }

        /// <summary>
        /// Route A: Unity's own Built-in->URP material upgrade. The documented batch entry point
        /// (Converters.RunInBatchMode) is tried first and its outcome recorded; URP 17.3.0 on 6000.3.24f1
        /// throws MissingMethodException (it instantiates abstract Base2DMaterialUpgrader). The same URP
        /// upgrader set is then applied through the public core MaterialUpgrader API.
        /// </summary>
        public static void ConvertRouteA()
        {
            RequireUrp();
            string batchResult;
            try
            {
                Converters.RunInBatchMode(ConverterContainerId.BuiltInToURP,
                    new List<ConverterId> { ConverterId.Material, ConverterId.RenderSettings }, ConverterFilter.Inclusive);
                batchResult = "OK";
            }
            catch (Exception e)
            {
                batchResult = e.GetType().Name + ": " + e.Message;
            }
            var upgraders = UnityEditor.Rendering.MaterialUpgrader.FetchAllUpgradersForPipeline(typeof(UniversalRenderPipelineAsset));
            int upgraded = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/Arkus/ART/Materials" }))
            {
                var material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if (material.shader.name != "Standard") continue;
                string message = null;
                if (UnityEditor.Rendering.MaterialUpgrader.Upgrade(material, upgraders, UnityEditor.Rendering.MaterialUpgrader.UpgradeFlags.None, ref message))
                    upgraded++;
                EditorUtility.SetDirty(material);
            }
            AssetDatabase.SaveAssets();
            WriteJson("s01/converter.json", JsonUtility.ToJson(new ConverterResult { batchApi = batchResult, publicUpgraderCount = upgraders.Count, materialsUpgraded = upgraded }, true));
            Log("H2F01_S01_BATCH_CONVERTER " + batchResult);
            var standard = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Arkus/ART/Materials" })
                .Select(g => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g)))
                .Where(m => m.shader.name == "Standard").Select(m => m.name).ToArray();
            Log("H2F01_S01_CONVERTER_DONE remaining_standard=" + standard.Length + " " + string.Join(",", standard));
        }

        public static void CaptureRoutes()
        {
            RequireUrp();
            OpenArtScene();
            var look = BaselineLook();
            // A: converted ART palette materials
            Shoot("urpA");
            WriteJson("s01/audit_urpA.json", JsonUtility.ToJson(AuditShaders(), true));
            // B: Source URP-native Shader Graph materials on the Source-derived pieces
            var original = Snapshot();
            ApplyMap(SourceNative());
            Shoot("urpB");
            WriteJson("s01/audit_urpB.json", JsonUtility.ToJson(AuditShaders(), true));
            Restore(original);
            // C: ART palette (converted) + Source normal maps
            ApplyMap(PaletteWithNormals());
            Shoot("urpC");
            WriteJson("s01/audit_urpC.json", JsonUtility.ToJson(AuditShaders(), true));
            Restore(original);
            // Evening variant on the same preset: low warm key, darker ambient, Bar threshold light kept.
            var key = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None).First(l => l.type == LightType.Directional);
            var keyRot = key.transform.rotation; var keyCol = key.color; var keyI = key.intensity;
            var amb = (RenderSettings.ambientSkyColor, RenderSettings.ambientEquatorColor, RenderSettings.ambientGroundColor, RenderSettings.fogColor);
            key.transform.rotation = Quaternion.Euler(9, -55, 0);
            key.color = new Color(1f, 0.74f, 0.52f);
            key.intensity = 0.55f;
            RenderSettings.ambientSkyColor = new Color(0.3f, 0.33f, 0.4f);
            RenderSettings.ambientEquatorColor = new Color(0.26f, 0.25f, 0.26f);
            RenderSettings.ambientGroundColor = new Color(0.12f, 0.12f, 0.12f);
            RenderSettings.fogColor = new Color(0.36f, 0.36f, 0.4f);
            Shoot("urpA_evening", new[] { "w12_casco", "f01_threshold", "f01_exterior" });
            key.transform.rotation = keyRot; key.color = keyCol; key.intensity = keyI;
            (RenderSettings.ambientSkyColor, RenderSettings.ambientEquatorColor, RenderSettings.ambientGroundColor, RenderSettings.fogColor) = amb;
            // Same A geometry without the post/SSAO/decal baseline, to isolate the look layer.
            look.SetActive(false);
            SetSsao(false);
            Shoot("urpA_nopost", new[] { "w12_casco", "f01_threshold" });
            SetSsao(true);
            Log("H2F01_S01_URP_CAPTURED");
        }

        public static void CaptureH1Slice()
        {
            // H2F-00 flagged the H1 slice MI_Plaster as incompatible without Shader Graph. Check the same
            // Source material (original GUID links) on the H1 facade FBX module under URP 17.3.
            RequireUrp();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var plaster = AssetDatabase.LoadAssetAtPath<Material>(SourceMats + "/MI_Plaster.mat");
            var result = new H1Slice { material = plaster != null ? plaster.name : "<missing>", shader = plaster != null ? plaster.shader.name : "<none>", supported = plaster != null && plaster.shader.isSupported };
            var props = new List<string>();
            if (plaster != null)
                for (int i = 0; i < plaster.shader.GetPropertyCount(); i++)
                {
                    var n = plaster.shader.GetPropertyName(i);
                    if (plaster.shader.GetPropertyType(i) == ShaderPropertyType.Texture)
                        props.Add(n + "=" + (plaster.GetTexture(n) != null ? plaster.GetTexture(n).name : "none"));
                }
            result.textures = props.ToArray();
            WriteJson("s01/h1_slice_mi_plaster.json", JsonUtility.ToJson(result, true));
            Log("H2F01_S01_H1_SLICE shader=" + result.shader + " supported=" + result.supported);
        }

        [Serializable] class ConverterResult { public string batchApi; public int publicUpgraderCount, materialsUpgraded; }

        [Serializable] class H1Slice { public string material, shader; public bool supported; public string[] textures; }

        // ---------------------------------------------------------------- helpers

        static void UsePipeline(RenderPipelineAsset asset)
        {
            GraphicsSettings.defaultRenderPipeline = asset;
            int current = QualitySettings.GetQualityLevel();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = asset;
            }
            QualitySettings.SetQualityLevel(current, false);
            AssetDatabase.SaveAssets();
        }

        static void RequireUrp()
        {
            var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(Settings + "/H2F01_URP.asset");
            if (asset == null) throw new Exception("H2F01_URP_ASSET_MISSING run SetupUrp first");
            UsePipeline(asset);
        }

        static void SetSsao(bool on)
        {
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(Settings + "/H2F01_Renderer.asset");
            foreach (var f in renderer.rendererFeatures)
                if (f is ScreenSpaceAmbientOcclusion) f.SetActive(on);
        }

        /// <summary>Fixed overcast baseline: ART's key light/fog kept; adds Volume tone/colour, reflection probe and one damp decal.</summary>
        public static GameObject BaselineLook()
        {
            var root = new GameObject("H2F01_S01_BASELINE_LOOK_NOT_KEEPER");
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            var tone = profile.Add<Tonemapping>(true);
            tone.mode.Override(TonemappingMode.Neutral);
            var colour = profile.Add<ColorAdjustments>(true);
            colour.postExposure.Override(0.25f);
            colour.contrast.Override(8f);
            colour.saturation.Override(-12f);
            var wb = profile.Add<WhiteBalance>(true);
            wb.temperature.Override(-6f);
            var volume = root.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.sharedProfile = profile;
            var probe = new GameObject("reflection_probe").AddComponent<ReflectionProbe>();
            probe.transform.SetParent(root.transform);
            probe.transform.position = new Vector3(0, 3, 0);
            probe.size = new Vector3(40, 20, 90);
            probe.mode = ReflectionProbeMode.Realtime;
            probe.refreshMode = ReflectionProbeRefreshMode.OnAwake;
            probe.RenderProbe();
            // Damp stain decal on the W12 road near the retaining edge.
            var decalMat = new Material(AssetDatabase.LoadAssetAtPath<Material>(
                "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Decal.mat"));
            decalMat.SetTexture("Base_Map", DampTexture());
            var decal = new GameObject("damp_decal").AddComponent<DecalProjector>();
            decal.transform.SetParent(root.transform);
            decal.material = decalMat;
            decal.size = new Vector3(2.2f, 3.5f, 1.2f);
            decal.fadeFactor = 0.85f;
            var roadY = RoadHeightAt(0.4f, -3f);
            decal.transform.position = new Vector3(0.4f, roadY + 0.3f, -3f);
            decal.transform.rotation = Quaternion.Euler(90, 0, 0);
            // Fixed high-overcast sky/ambient/fog preset (native Skybox/Procedural + RenderSettings; no weather suite).
            var sky = new Material(Shader.Find("Skybox/Panoramic"));
            sky.SetTexture("_MainTex", OvercastSky());
            sky.SetFloat("_Exposure", 1.0f);
            RenderSettings.skybox = sky;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.62f, 0.67f, 0.7f);
            RenderSettings.ambientEquatorColor = new Color(0.5f, 0.54f, 0.54f);
            RenderSettings.ambientGroundColor = new Color(0.3f, 0.32f, 0.3f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.012f;
            RenderSettings.fogColor = new Color(0.66f, 0.7f, 0.72f);
            foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            return root;
        }

        static float RoadHeightAt(float x, float z)
        {
            if (Physics.Raycast(new Vector3(x, 30, z), Vector3.down, out var hit, 60)) return hit.point.y;
            return 0;
        }

        /// <summary>Project-owned generated high-overcast equirect gradient with soft cloud banding (no external HDRI).</summary>
        static Texture2D OvercastSky()
        {
            const int w = 512, h = 256;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false) { wrapModeU = TextureWrapMode.Repeat, wrapModeV = TextureWrapMode.Clamp };
            var zenith = new Color(0.60f, 0.64f, 0.67f);
            var horizon = new Color(0.78f, 0.80f, 0.80f);
            var ground = new Color(0.42f, 0.45f, 0.44f);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float v = y / (float)(h - 1); // 0 bottom .. 1 top
                    Color c;
                    if (v < 0.5f) c = Color.Lerp(ground, horizon, Mathf.Pow(v / 0.5f, 3f));
                    else
                    {
                        float t = (v - 0.5f) / 0.5f;
                        c = Color.Lerp(horizon, zenith, Mathf.Sqrt(t));
                        float cloud = Mathf.PerlinNoise(x * 0.012f, y * 0.05f) * 0.6f + Mathf.PerlinNoise(x * 0.04f, y * 0.12f) * 0.4f;
                        c = Color.Lerp(c, new Color(0.84f, 0.85f, 0.85f), Mathf.Clamp01(cloud - 0.45f) * 0.9f * (1 - t * 0.5f));
                    }
                    tex.SetPixel(x, y, c);
                }
            tex.Apply();
            return tex;
        }

        static Texture2D DampTexture()
        {
            var tex = new Texture2D(128, 128, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var rng = new System.Random(12);
            for (int y = 0; y < 128; y++)
                for (int x = 0; x < 128; x++)
                {
                    float dx = (x - 64) / 64f, dy = (y - 64) / 64f;
                    float r = Mathf.Sqrt(dx * dx * 1.6f + dy * dy);
                    float n = Mathf.PerlinNoise(x * 0.07f, y * 0.07f);
                    float a = Mathf.Clamp01((1f - r) * 1.4f) * (0.55f + 0.45f * n) * 0.55f;
                    tex.SetPixel(x, y, new Color(0.08f, 0.09f, 0.09f, a));
                }
            tex.Apply();
            return tex;
        }

        static void Shoot(string prefix, string[] only = null)
        {
            var inspector = UnityEngine.Object.FindFirstObjectByType<Art01WalkInspector>();
            PoseHumans(UalClip("Idle_Loop"));
            foreach (var v in Views)
            {
                if (only != null && !only.Contains(v.id)) continue;
                var camera = CameraAt("h2f01_" + v.id, v.pos, v.target);
                if (GraphicsSettings.currentRenderPipeline != null)
                    camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
                Image(camera, $"s01/{prefix}_{v.id}.png");
                UnityEngine.Object.DestroyImmediate(camera.gameObject);
            }
            if (only == null && inspector != null && inspector.view != null)
            {
                // Clothed avatar at W12 from a third-person camera; UAL1 Idle_Loop evaluated through a
                // PlayableGraph (edit-mode humanoid posing), then the rig is put back.
                var home = inspector.transform.position;
                inspector.transform.position = new Vector3(0, RoadHeightAt(0, -8) + 0.03f, -8);
                PoseHumans(UalClip("Idle_Loop"));
                var camera = CameraAt("h2f01_avatar", inspector.transform.TransformPoint(new Vector3(0.8f, 1.6f, 2.4f)),
                    inspector.transform.position + Vector3.up * 1.1f, 45);
                if (GraphicsSettings.currentRenderPipeline != null)
                    camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
                Image(camera, $"s01/{prefix}_avatar_w12.png");
                UnityEngine.Object.DestroyImmediate(camera.gameObject);
                inspector.transform.position = home;
            }
            AnimationMode.StopAnimationMode();
        }

        public static AnimationClip UalClip(string name, string fbx = "Assets/Arkus/ART/External/UAL/UAL1.fbx")
        {
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(fbx))
                if (asset is AnimationClip clip && (clip.name == name || clip.name == "Armature|" + name)) return clip;
            throw new Exception("H2F01_CLIP_MISSING " + name + " in " + fbx);
        }

        /// <summary>
        /// Poses every humanoid with a clip so captures never show a bind/T-pose. Edit-mode PlayableGraph
        /// evaluation does not move humanoid bones here (probe 06); AnimationMode sampling does. Caller must
        /// AnimationMode.StopAnimationMode() after rendering.
        /// </summary>
        public static void PoseHumans(AnimationClip clip, float time = 0.4f)
        {
            if (!AnimationMode.InAnimationMode()) AnimationMode.StartAnimationMode();
            foreach (var animator in UnityEngine.Object.FindObjectsByType<Animator>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                if (animator.avatar != null && animator.avatar.isHuman)
                    AnimationMode.SampleAnimationClip(animator.gameObject, clip, time);
        }

        static Dictionary<Renderer, Material[]> Snapshot() =>
            UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToDictionary(r => r, r => r.sharedMaterials);

        static void Restore(Dictionary<Renderer, Material[]> map)
        {
            foreach (var kv in map) kv.Key.sharedMaterials = kv.Value;
        }

        static void ApplyMap(Dictionary<string, Material> map)
        {
            foreach (var renderer in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var mats = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                    if (mats[i] != null && map.TryGetValue(mats[i].name, out var replacement)) { mats[i] = replacement; changed = true; }
                if (changed) renderer.sharedMaterials = mats;
            }
        }

        static readonly Dictionary<string, string> ArtToSource = new Dictionary<string, string>
        {
            { "Stone", "MI_UnevenBrick" }, { "StoneShadow", "MI_UnevenBrick" }, { "StoneTrim", "MI_Brick" },
            { "Plaster", "MI_Plaster" }, { "PlasterOchre", "MI_Plaster" }, { "WoodDark", "MI_WoodTrim" },
            { "ShutterGreen", "MI_WoodTrim" }, { "TileWet", "MI_RoundTiles" }, { "Cobble", "MI_RoundRocks" },
            { "Glass", "MI_WindowGlass" }, { "Iron", "MI_MetalOrnaments" },
        };

        static readonly Dictionary<string, string> ArtToNormal = new Dictionary<string, string>
        {
            { "Stone", "T_UnevenBrick_Normal" }, { "StoneShadow", "T_UnevenBrick_Normal" }, { "StoneTrim", "T_Brick_Normal" },
            { "Plaster", "T_Plaster_Normal" }, { "PlasterOchre", "T_Plaster_Normal" }, { "WoodDark", "T_WoodTrim_Normal" },
            { "ShutterGreen", "T_WoodTrim_Normal" }, { "TileWet", "T_RoundTiles_Normal" }, { "Cobble", "T_RoundRocks_Normal" },
        };

        static Dictionary<string, Material> SourceNative() =>
            ArtToSource.ToDictionary(kv => kv.Key, kv =>
                AssetDatabase.LoadAssetAtPath<Material>($"{SourceMats}/{kv.Value}.mat") ?? throw new Exception("H2F01_SOURCE_MATERIAL_MISSING " + kv.Value));

        static Dictionary<string, Material> PaletteWithNormals()
        {
            var result = new Dictionary<string, Material>();
            foreach (var kv in ArtToNormal)
            {
                var basis = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{kv.Key}.mat");
                var normal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{SourceMats}/{kv.Value}.png");
                if (basis == null) continue; // ART only materializes palette entries its builder uses
                if (normal == null) throw new Exception("H2F01_ROUTE_C_NORMAL_MISSING " + kv.Value);
                var m = new Material(basis) { name = kv.Key + "_C" };
                m.SetTexture("_BumpMap", normal);
                m.SetFloat("_BumpScale", 1f);
                m.EnableKeyword("_NORMALMAP");
                result[kv.Key] = m;
            }
            return result;
        }
    }
}
