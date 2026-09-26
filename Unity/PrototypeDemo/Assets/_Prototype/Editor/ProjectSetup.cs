using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Proto.EditorTools
{
    /// One-shot, idempotent project configuration (URP asset + renderer with SSAO, linear color, input, layers).
    public static class ProjectSetup
    {
        const string Dir = "Assets/_Prototype/Settings";
        const string RendererPath = Dir + "/Proto_Renderer.asset";
        const string PipelinePath = Dir + "/Proto_URP.asset";

        public static readonly string[] Layers = { "Player", "NPC", "Interact", "CamIgnore" };

        [MenuItem("Prototype/Setup Project")]
        public static void Run()
        {
            Directory.CreateDirectory(Dir);
            var rd = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (rd == null)
            {
                rd = ScriptableObject.CreateInstance<UniversalRendererData>();
                rd.postProcessData = AssetDatabase.LoadAssetAtPath<PostProcessData>(
                    "Packages/com.unity.render-pipelines.universal/Runtime/Data/PostProcessData.asset");
                AssetDatabase.CreateAsset(rd, RendererPath);
            }
            if (!rd.rendererFeatures.Exists(f => f is ScreenSpaceAmbientOcclusion))
            {
                var ssao = ScriptableObject.CreateInstance<ScreenSpaceAmbientOcclusion>();
                ssao.name = "SSAO";
                AssetDatabase.AddObjectToAsset(ssao, rd);
                rd.rendererFeatures.Add(ssao);
                var so = new SerializedObject(rd);
                var map = so.FindProperty("m_RendererFeatureMap");
                map.arraySize = rd.rendererFeatures.Count;
                for (int i = 0; i < rd.rendererFeatures.Count; i++)
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(rd.rendererFeatures[i], out _, out long id);
                    map.GetArrayElementAtIndex(i).longValue = id;
                }
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            var ssaoFeature = rd.rendererFeatures.Find(f => f is ScreenSpaceAmbientOcclusion);
            var sso = new SerializedObject(ssaoFeature);
            sso.FindProperty("m_Settings.Intensity").floatValue = 1.6f;
            sso.FindProperty("m_Settings.Radius").floatValue = 0.45f;
            sso.FindProperty("m_Settings.DirectLightingStrength").floatValue = 0.35f;
            sso.FindProperty("m_Settings.Falloff").floatValue = 60f;
            sso.ApplyModifiedPropertiesWithoutUndo();
            rd.SetDirty();
            EditorUtility.SetDirty(rd);

            var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (asset == null)
            {
                asset = UniversalRenderPipelineAsset.Create(rd);
                AssetDatabase.CreateAsset(asset, PipelinePath);
            }
            asset.supportsHDR = true;
            asset.msaaSampleCount = 4;
            asset.renderScale = 1f;
            asset.supportsCameraDepthTexture = true;
            asset.shadowDistance = 110f;
            asset.shadowCascadeCount = 3;
            asset.mainLightShadowmapResolution = 4096;
            asset.maxAdditionalLightsCount = 8;
            var aso = new SerializedObject(asset);
            SetIfExists(aso, "m_AdditionalLightsRenderingMode", (int)LightRenderingMode.PerPixel);
            SetIfExists(aso, "m_SoftShadowsSupported", true);
            SetIfExists(aso, "m_AdditionalLightShadowsSupported", true);
            SetIfExists(aso, "m_SoftShadowQuality", 3);
            aso.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);

            GraphicsSettings.defaultRenderPipeline = asset;
            var qs = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);
            var levels = qs.FindProperty("m_QualitySettings");
            for (int i = 0; i < levels.arraySize; i++)
            {
                var lvl = levels.GetArrayElementAtIndex(i);
                lvl.FindPropertyRelative("customRenderPipeline").objectReferenceValue = asset;
                var af = lvl.FindPropertyRelative("anisotropicTextures");
                if (af != null) af.intValue = 2;
            }
            qs.ApplyModifiedPropertiesWithoutUndo();

            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.productName = "Del Puente Viejo al Bar (prototipo)";
            PlayerSettings.companyName = "Juego2 Prototype";
            PlayerSettings.defaultScreenWidth = 1600;
            PlayerSettings.defaultScreenHeight = 900;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;

            var ps = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            SetIfExists(ps, "activeInputHandler", 1);
            ps.ApplyModifiedPropertiesWithoutUndo();

            var tm = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layers = tm.FindProperty("layers");
            for (int i = 0; i < Layers.Length; i++)
                layers.GetArrayElementAtIndex(8 + i).stringValue = Layers[i];
            tm.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.SaveAssets();
            Debug.Log("[Proto] Project setup done: " + PipelinePath);
        }

        static void SetIfExists(SerializedObject so, string prop, object value)
        {
            var p = so.FindProperty(prop);
            if (p == null) { Debug.LogWarning("[Proto] missing setting " + prop); return; }
            switch (value)
            {
                case bool b: p.boolValue = b; break;
                case int n: p.intValue = n; break;
                case float f: p.floatValue = f; break;
            }
        }
    }
}
