using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Juego2.Foundation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// The canonical Juego2 URP/toolchain project baseline adopted by WP-H2F-02. <see cref="Apply"/> is idempotent
    /// and updates existing assets in place (stable GUIDs). <see cref="Verify"/> fails closed with stable codes and is
    /// what later Workers run to prove they still stand on this baseline.
    /// </summary>
    public static class J2FoundationBaseline
    {
        public const string UnityVersion = "6000.3.24f1";
        public const string Root = "Assets/Juego2/Foundation";
        public const string Rendering = Root + "/Rendering";
        public const string PipelineHigh = Rendering + "/J2_URP_High.asset";
        public const string PipelineLow = Rendering + "/J2_URP_Low.asset";
        public const string RendererHigh = Rendering + "/J2_Renderer_High.asset";
        public const string RendererLow = Rendering + "/J2_Renderer_Low.asset";
        public const string GlobalSettings = Rendering + "/J2_URPGlobalSettings.asset";
        public const string DefaultVolume = Rendering + "/J2_DefaultVolumeProfile.asset";
        public const string LookVolume = Rendering + "/J2_LookVolumeProfile.asset";
        public const string SkyTexture = Rendering + "/J2_OvercastSky.png";
        public const string SkyMaterial = Rendering + "/J2_Sky_Overcast.mat";
        public const string LookPreset = Rendering + "/J2_LookPreset.asset";
        public const string InputAsset = Root + "/Input/J2_Input.inputactions";
        public const string Presets = Root + "/Presets";
        public const string PlayerPreset = Presets + "/J2_Player.asset";
        public const string NpcPreset = Presets + "/J2_Npc_Civilian.asset";
        public const string CameraPreset = Presets + "/J2_PlayerCamera.asset";

        public const float ShadowDistanceHigh = 70f, ShadowDistanceLow = 40f;
        public const float AgentRadius = 0.28f, AgentHeight = 1.8f, AgentClimb = 0.3f, AgentSlope = 40f;
        public const int InputHandlerBoth = 2;

        /// <summary>Exact direct package set (H2F-01 selection + 01A Core requirements; Cinemachine not admitted).</summary>
        public static readonly IReadOnlyDictionary<string, string> DirectPackages = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["com.unity.ai.navigation"] = "2.0.15",
            ["com.unity.animation.rigging"] = "1.4.1",
            ["com.unity.collections"] = "2.6.8",
            ["com.unity.inputsystem"] = "1.20.0",
            ["com.unity.mathematics"] = "1.3.3",
            ["com.unity.render-pipelines.universal"] = "17.3.0",
            ["com.unity.shadergraph"] = "17.3.0",
            ["com.unity.splines"] = "2.9.1",
            ["com.unity.test-framework"] = "1.6.0",
            ["com.unity.ugui"] = "2.0.0",
            ["com.unity.modules.ai"] = "1.0.0",
            ["com.unity.modules.animation"] = "1.0.0",
            ["com.unity.modules.audio"] = "1.0.0",
            ["com.unity.modules.imageconversion"] = "1.0.0",
            ["com.unity.modules.imgui"] = "1.0.0",
            ["com.unity.modules.jsonserialize"] = "1.0.0",
            ["com.unity.modules.particlesystem"] = "1.0.0",
            ["com.unity.modules.physics"] = "1.0.0",
            ["com.unity.modules.physics2d"] = "1.0.0",
            ["com.unity.modules.screencapture"] = "1.0.0",
            ["com.unity.modules.terrain"] = "1.0.0",
            ["com.unity.modules.terrainphysics"] = "1.0.0",
            ["com.unity.modules.ui"] = "1.0.0",
            ["com.unity.modules.uielements"] = "1.0.0",
        };

        /// <summary>Evaluated, not selected (H2F-01 matrix / S06 amendment). Their presence means an unreviewed adoption.</summary>
        public static readonly string[] ForbiddenPackages =
        {
            "com.unity.cinemachine", "com.unity.probuilder", "com.unity.terrain-tools", "com.unity.render-pipelines.high-definition",
            "com.unity.timeline", "com.unity.addressables", "com.unity.recorder", "com.unity.ai.assistant", "com.unity.ai.navigation.components",
        };

        public static readonly string[] QualityLevels = { "Low", "High" };

        // ------------------------------------------------------------------ apply

        public static void Apply()
        {
            RequireEditor();
            EnsureFolder(Rendering);
            AdoptUrpGeneratedAssets();

            var rendererHigh = EnsureRenderer(RendererHigh, ssao: true);
            var rendererLow = EnsureRenderer(RendererLow, ssao: false);
            var high = EnsurePipeline(PipelineHigh, rendererHigh, ShadowDistanceHigh, cascades: 2, MsaaQuality._4x);
            var low = EnsurePipeline(PipelineLow, rendererLow, ShadowDistanceLow, cascades: 1, MsaaQuality.Disabled);

            GraphicsSettings.defaultRenderPipeline = high;
            ApplyQuality(low, high);
            ApplyRenderingLayers();
            ApplyPlayerSettings();
            ApplyNavMeshAgent();
            EnsureLookAssets();
            EnsurePresets();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("J2_FOUNDATION_BASELINE_APPLIED pipeline=" + PipelineHigh);
        }

        static void RequireEditor()
        {
            if (Application.unityVersion != UnityVersion)
                throw new InvalidOperationException("J2_EDITOR_VERSION_MISMATCH " + Application.unityVersion);
        }

        public static void EnsureFolder(string path)
        {
            var parts = path.Split('/');
            var current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        /// <summary>URP creates its global settings and default Volume profile at the Assets root; keep them under the foundation (GUID-preserving move).</summary>
        static void AdoptUrpGeneratedAssets()
        {
            Move("Assets/UniversalRenderPipelineGlobalSettings.asset", GlobalSettings);
            Move("Assets/DefaultVolumeProfile.asset", DefaultVolume);
        }

        static void Move(string from, string to)
        {
            if (AssetDatabase.LoadMainAssetAtPath(from) == null || AssetDatabase.LoadMainAssetAtPath(to) != null) return;
            var error = AssetDatabase.MoveAsset(from, to);
            if (!string.IsNullOrEmpty(error)) throw new InvalidOperationException("J2_URP_ASSET_MOVE_FAILED " + from + ": " + error);
        }

        static UniversalRendererData EnsureRenderer(string path, bool ssao)
        {
            var data = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(data, path);
            }
            if (ssao) EnsureFeature<ScreenSpaceAmbientOcclusion>(data, "SSAO");
            var decal = EnsureFeature<DecalRendererFeature>(data, "Decals");
            var so = new SerializedObject(decal);
            so.FindProperty("m_Settings.decalLayers").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();
            SyncFeatureMap(data);
            EditorUtility.SetDirty(data);
            return data;
        }

        static T EnsureFeature<T>(UniversalRendererData data, string name) where T : ScriptableRendererFeature
        {
            var existing = data.rendererFeatures.OfType<T>().FirstOrDefault();
            if (existing != null) return existing;
            var feature = ScriptableObject.CreateInstance<T>();
            feature.name = name;
            AssetDatabase.AddObjectToAsset(feature, data);
            data.rendererFeatures.Add(feature);
            return feature;
        }

        /// <summary>The renderer's feature map holds each feature's local file ID; derive it through the public AssetDatabase API.</summary>
        static void SyncFeatureMap(UniversalRendererData data)
        {
            var so = new SerializedObject(data);
            var features = so.FindProperty("m_RendererFeatures");
            var map = so.FindProperty("m_RendererFeatureMap");
            map.arraySize = features.arraySize;
            for (int i = 0; i < features.arraySize; i++)
            {
                var feature = features.GetArrayElementAtIndex(i).objectReferenceValue;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feature, out _, out long localId);
                map.GetArrayElementAtIndex(i).longValue = localId;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static UniversalRenderPipelineAsset EnsurePipeline(string path, UniversalRendererData renderer, float shadowDistance, int cascades, MsaaQuality msaa)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            if (asset == null)
            {
                asset = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(asset, path);
            }
            var so = new SerializedObject(asset);
            var list = so.FindProperty("m_RendererDataList");
            list.arraySize = 1;
            list.GetArrayElementAtIndex(0).objectReferenceValue = renderer;
            so.FindProperty("m_DefaultRendererIndex").intValue = 0;
            so.FindProperty("m_RequireDepthTexture").boolValue = true;      // water depth fade (S03)
            so.FindProperty("m_RequireOpaqueTexture").boolValue = false;
            so.FindProperty("m_SupportsHDR").boolValue = true;
            so.FindProperty("m_MSAA").intValue = (int)msaa;
            so.FindProperty("m_MainLightShadowsSupported").boolValue = true;
            so.FindProperty("m_ShadowDistance").floatValue = shadowDistance;
            so.FindProperty("m_ShadowCascadeCount").intValue = cascades;
            so.FindProperty("m_SoftShadowsSupported").boolValue = true;
            so.FindProperty("m_LightProbeSystem").intValue = (int)LightProbeSystem.LegacyLightProbes; // APV off (GI policy)
            so.FindProperty("m_SupportsLightLayers").boolValue = true;      // rendering layers for decal reception
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        static void ApplyQuality(RenderPipelineAsset low, RenderPipelineAsset high)
        {
            var quality = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0];
            var so = new SerializedObject(quality);
            var levels = so.FindProperty("m_QualitySettings");
            // Keep the built-in "Low" and "High" rows as templates; drop the others.
            for (int i = levels.arraySize - 1; i >= 0; i--)
            {
                var name = levels.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
                if (!QualityLevels.Contains(name)) levels.DeleteArrayElementAtIndex(i);
            }
            if (levels.arraySize != QualityLevels.Length)
                throw new InvalidOperationException("J2_QUALITY_TEMPLATE_MISSING levels=" + levels.arraySize);
            for (int i = 0; i < levels.arraySize; i++)
            {
                var row = levels.GetArrayElementAtIndex(i);
                var isHigh = row.FindPropertyRelative("name").stringValue == "High";
                row.FindPropertyRelative("customRenderPipeline").objectReferenceValue = isHigh ? high : low;
            }
            so.FindProperty("m_CurrentQuality").intValue = Array.IndexOf(QualityLevels, "High");
            var perPlatform = so.FindProperty("m_PerPlatformDefaultQuality");
            for (int i = 0; i < perPlatform.arraySize; i++)
                perPlatform.GetArrayElementAtIndex(i).FindPropertyRelative("second").intValue = Array.IndexOf(QualityLevels, "High");
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void ApplyRenderingLayers()
        {
            var tags = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            var so = new SerializedObject(tags);
            var layers = so.FindProperty("m_RenderingLayers");
            if (layers.arraySize < 2) layers.arraySize = 2;
            layers.GetArrayElementAtIndex(1).stringValue = J2RenderingLayers.DecalReceiverName;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void ApplyPlayerSettings()
        {
            PlayerSettings.colorSpace = ColorSpace.Linear;
            var player = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0];
            var so = new SerializedObject(player);
            so.FindProperty("activeInputHandler").intValue = InputHandlerBoth;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>Project NavMesh agent sized to the accepted door/lane classes (H2F-01 S02: default Humanoid 0.5 m cannot pass 1.0 m doors).</summary>
        static void ApplyNavMeshAgent()
        {
            var areas = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/NavMeshAreas.asset")[0];
            var so = new SerializedObject(areas);
            var agent = so.FindProperty("m_Settings").GetArrayElementAtIndex(0);
            agent.FindPropertyRelative("agentRadius").floatValue = AgentRadius;
            agent.FindPropertyRelative("agentHeight").floatValue = AgentHeight;
            agent.FindPropertyRelative("agentClimb").floatValue = AgentClimb;
            agent.FindPropertyRelative("agentSlope").floatValue = AgentSlope;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void EnsureLookAssets()
        {
            if (!File.Exists(SkyTexture))
            {
                File.WriteAllBytes(SkyTexture, J2FoundationLook.GenerateOvercastSky().EncodeToPNG());
                AssetDatabase.ImportAsset(SkyTexture, ImportAssetOptions.ForceSynchronousImport);
            }
            var importer = (TextureImporter)AssetImporter.GetAtPath(SkyTexture);
            importer.wrapModeU = TextureWrapMode.Repeat;
            importer.wrapModeV = TextureWrapMode.Clamp;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = true;
            importer.SaveAndReimport();

            var sky = AssetDatabase.LoadAssetAtPath<Material>(SkyMaterial);
            if (sky == null)
            {
                sky = new Material(Shader.Find("Skybox/Panoramic"));
                AssetDatabase.CreateAsset(sky, SkyMaterial);
            }
            sky.shader = Shader.Find("Skybox/Panoramic");
            sky.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(SkyTexture));
            sky.SetFloat("_Exposure", 1f);
            EditorUtility.SetDirty(sky);

            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(LookVolume);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, LookVolume);
            }
            var tone = Component<Tonemapping>(profile);
            tone.mode.Override(TonemappingMode.Neutral);
            var colour = Component<ColorAdjustments>(profile);
            colour.postExposure.Override(0.25f);
            colour.contrast.Override(8f);
            colour.saturation.Override(-12f);
            var wb = Component<WhiteBalance>(profile);
            wb.temperature.Override(-6f);
            EditorUtility.SetDirty(profile);

            var look = AssetDatabase.LoadAssetAtPath<J2LookPreset>(LookPreset);
            if (look == null)
            {
                look = ScriptableObject.CreateInstance<J2LookPreset>();
                AssetDatabase.CreateAsset(look, LookPreset);
            }
            look.skybox = sky;
            look.volumeProfile = profile;
            EditorUtility.SetDirty(look);
        }

        /// <summary>S06 amendment presets (PUBLIC_AUTHORING_SURFACE.md): data only; the GC2 materializer builds from them.</summary>
        static void EnsurePresets()
        {
            EnsureFolder(Presets);
            var input = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(InputAsset)
                        ?? throw new InvalidOperationException("J2_INPUT_ASSET_MISSING " + InputAsset);
            var player = Preset<J2CharacterPreset>(PlayerPreset);
            player.isPlayer = true; player.height = AgentHeight; player.radius = AgentRadius; player.linearSpeed = 1.45f;
            player.inputActions = input; player.moveActionMap = "Player"; player.moveAction = "Move";
            player.navMeshDriver = false; player.footstepDetector = J2CharacterPreset.FootstepDetector.Fulcrum;
            EditorUtility.SetDirty(player);
            var npc = Preset<J2CharacterPreset>(NpcPreset);
            npc.isPlayer = false; npc.height = AgentHeight; npc.radius = AgentRadius; npc.linearSpeed = 1.2f;
            npc.inputActions = null; npc.navMeshDriver = true; npc.footstepDetector = J2CharacterPreset.FootstepDetector.Fulcrum;
            EditorUtility.SetDirty(npc);
            var camera = Preset<J2CameraPreset>(CameraPreset);
            camera.radius = 3f; camera.autoAlign = true; camera.alignDelay = 0.5f; camera.alignSmoothTime = 1f;
            camera.nearClip = 0.05f; camera.fieldOfView = 55f; camera.inputActions = input;
            camera.actionMap = "Player"; camera.lookAction = "Look"; camera.zoomAction = "Zoom";
            EditorUtility.SetDirty(camera);
        }

        static T Preset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        static T Component<T>(VolumeProfile profile) where T : VolumeComponent
        {
            if (profile.TryGet<T>(out var existing)) return existing;
            var component = profile.Add<T>(true);
            component.name = typeof(T).Name;
            component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(component, profile);
            return component;
        }

        // ------------------------------------------------------------------ verify

        /// <summary>Every deviation from the adopted baseline, as stable codes. Empty means GREEN.</summary>
        public static List<string> Verify()
        {
            var findings = new List<string>();
            void Require(bool ok, string code) { if (!ok) findings.Add(code); }

            Require(Application.unityVersion == UnityVersion, "J2_EDITOR_VERSION_MISMATCH:" + Application.unityVersion);
            Require(EditorSettings.serializationMode == SerializationMode.ForceText, "J2_SERIALIZATION_NOT_FORCE_TEXT");

            var high = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineHigh);
            var low = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineLow);
            Require(high != null && low != null, "J2_PIPELINE_ASSET_MISSING");
            Require(GraphicsSettings.defaultRenderPipeline == high, "J2_DEFAULT_PIPELINE_NOT_J2_URP_HIGH");
            Require(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset, "J2_ACTIVE_PIPELINE_NOT_URP");
            Require(QualitySettings.names.SequenceEqual(QualityLevels), "J2_QUALITY_LEVELS:" + string.Join(",", QualitySettings.names));
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                var expected = QualitySettings.names[i] == "High" ? high : low;
                Require(QualitySettings.GetRenderPipelineAssetAt(i) == expected, "J2_QUALITY_PIPELINE:" + QualitySettings.names[i]);
            }
            if (high != null && low != null)
            {
                foreach (var (asset, distance, cascades, ssao) in new[] { (high, ShadowDistanceHigh, 2, true), (low, ShadowDistanceLow, 1, false) })
                {
                    var so = new SerializedObject(asset);
                    var tag = asset.name;
                    Require(so.FindProperty("m_RequireDepthTexture").boolValue, "J2_DEPTH_TEXTURE_OFF:" + tag);
                    Require(Mathf.Approximately(so.FindProperty("m_ShadowDistance").floatValue, distance), "J2_SHADOW_DISTANCE:" + tag);
                    Require(so.FindProperty("m_ShadowCascadeCount").intValue == cascades, "J2_SHADOW_CASCADES:" + tag);
                    Require(so.FindProperty("m_SoftShadowsSupported").boolValue, "J2_SOFT_SHADOWS_OFF:" + tag);
                    Require(so.FindProperty("m_LightProbeSystem").intValue == (int)LightProbeSystem.LegacyLightProbes, "J2_APV_ENABLED:" + tag);
                    Require(so.FindProperty("m_SupportsLightLayers").boolValue, "J2_RENDERING_LAYERS_OFF:" + tag);
                    var renderer = so.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0).objectReferenceValue as UniversalRendererData;
                    Require(renderer != null, "J2_RENDERER_MISSING:" + tag);
                    if (renderer == null) continue;
                    Require(renderer.rendererFeatures.OfType<ScreenSpaceAmbientOcclusion>().Any() == ssao, "J2_SSAO_POLICY:" + tag);
                    var decal = renderer.rendererFeatures.OfType<DecalRendererFeature>().FirstOrDefault();
                    Require(decal != null && new SerializedObject(decal).FindProperty("m_Settings.decalLayers").boolValue, "J2_DECAL_LAYERS_OFF:" + tag);
                    Require(renderer.rendererFeatures.All(f => f != null), "J2_RENDERER_FEATURE_NULL:" + tag);
                }
            }
            Require(PlayerSettings.colorSpace == ColorSpace.Linear, "J2_COLOR_SPACE_NOT_LINEAR");
            var player = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            Require(player.FindProperty("activeInputHandler").intValue == InputHandlerBoth, "J2_INPUT_HANDLER");
            var agent = NavMesh.GetSettingsByID(0);
            Require(Mathf.Approximately(agent.agentRadius, AgentRadius) && Mathf.Approximately(agent.agentHeight, AgentHeight)
                    && Mathf.Approximately(agent.agentClimb, AgentClimb) && Mathf.Approximately(agent.agentSlope, AgentSlope), "J2_NAVMESH_AGENT");
            var tags = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]).FindProperty("m_RenderingLayers");
            Require(tags.arraySize > 1 && tags.GetArrayElementAtIndex(1).stringValue == J2RenderingLayers.DecalReceiverName, "J2_RENDERING_LAYER_NAME");
            Require(AssetDatabase.LoadAssetAtPath<J2LookPreset>(LookPreset) is J2LookPreset look && look.skybox != null && look.volumeProfile != null, "J2_LOOK_PRESET");
            Require(AssetDatabase.LoadMainAssetAtPath("Assets/UniversalRenderPipelineGlobalSettings.asset") == null, "J2_URP_GLOBAL_SETTINGS_AT_ROOT");
            var playerPreset = AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(PlayerPreset);
            var input = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(InputAsset);
            Require(input != null && input.FindActionMap("Player", false) is var map && map != null
                    && new[] { "Move", "Look", "Zoom", "Interact" }.All(a => map.FindAction(a, false) != null), "J2_INPUT_ASSET");
            Require(playerPreset != null && playerPreset.isPlayer && playerPreset.inputActions == input, "J2_PLAYER_PRESET");
            Require(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(NpcPreset) is J2CharacterPreset npc && !npc.isPlayer && npc.inputActions == null, "J2_NPC_PRESET");
            Require(AssetDatabase.LoadAssetAtPath<J2CameraPreset>(CameraPreset) is J2CameraPreset cam && cam.inputActions == input, "J2_CAMERA_PRESET");
            findings.AddRange(J2PackageBaseline.Verify());
            findings.AddRange(J2AssemblyBoundary.Verify());
            return findings;
        }
    }
}
