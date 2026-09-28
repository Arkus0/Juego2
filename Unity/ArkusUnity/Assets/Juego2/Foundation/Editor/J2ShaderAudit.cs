using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// Fail-closed URP material audit: no hidden built-in, missing or unsupported shader. Covers material assets under
    /// given roots and everything a scene renders, including Terrain material/tree/detail prototypes (which
    /// renderer-only audits miss; H2F-01 finding). Declared H1 proof fixtures are reported, never hidden.
    /// </summary>
    public static class J2ShaderAudit
    {
        static readonly string[] UrpLightModes = { "UniversalForward", "UniversalForwardOnly", "UniversalGBuffer", "SRPDefaultUnlit", "Universal2D" };
        static readonly string[] PipelineAgnosticPrefixes = { "Skybox/", "UI/", "Sprites/", "TextMeshPro/", "Hidden/Universal", "Hidden/TerrainEngine" };

        [Serializable]
        public sealed class Row
        {
            public string material, shader, source, status;
        }

        [Serializable]
        public sealed class Report
        {
            public string pipeline;
            public int materials;
            public bool green;
            public List<Row> rows = new List<Row>();
            public List<string> failures = new List<string>();
            public List<string> declared = new List<string>();
        }

        public static string Classify(Shader shader)
        {
            if (shader == null) return "MISSING_SHADER";
            if (shader.name == "Hidden/InternalErrorShader") return "MISSING_SHADER";
            if (!shader.isSupported) return "UNSUPPORTED";
            if (PipelineAgnosticPrefixes.Any(p => shader.name.StartsWith(p, StringComparison.Ordinal))) return "URP_OK";
            // The subshader RenderPipeline tag is reliable in batch mode; the active-subshader pass list is not.
            for (int i = 0; i < shader.subshaderCount; i++)
                if (shader.FindSubshaderTagValue(i, new ShaderTagId("RenderPipeline")).name == "UniversalPipeline") return "URP_OK";
            bool allUntagged = shader.passCount > 0;
            for (int i = 0; i < shader.passCount; i++)
            {
                var mode = shader.FindPassTagValue(i, new ShaderTagId("LightMode")).name;
                if (UrpLightModes.Contains(mode)) return "URP_OK";
                allUntagged &= string.IsNullOrEmpty(mode);
            }
            // URP draws passes without a LightMode tag as SRPDefaultUnlit (e.g. legacy TextMesh "GUI/Text Shader").
            return allUntagged ? "URP_OK" : "BUILTIN_ONLY";
        }

        public static Report Audit(IEnumerable<(Material material, string source)> materials, IEnumerable<string> declaredFixturePrefixes = null)
        {
            var declared = (declaredFixturePrefixes ?? Enumerable.Empty<string>()).ToArray();
            var report = new Report { pipeline = GraphicsSettings.currentRenderPipeline == null ? "builtin" : GraphicsSettings.currentRenderPipeline.GetType().Name };
            foreach (var (material, source) in materials.GroupBy(m => (m.material, m.source)).Select(g => g.Key))
            {
                var path = material == null ? "<null material>" : AssetDatabase.GetAssetPath(material);
                var status = material == null ? "NULL_MATERIAL" : Classify(material.shader);
                var row = new Row { material = material == null ? "<null>" : path + "#" + material.name, shader = material?.shader?.name ?? "<none>", source = source, status = status };
                report.rows.Add(row);
                if (status == "URP_OK") continue;
                if (declared.Any(d => path.StartsWith(d, StringComparison.Ordinal))) report.declared.Add(row.material + " " + status);
                else report.failures.Add($"{status} {row.material} via {source} ({row.shader})");
            }
            report.materials = report.rows.Count;
            report.green = report.failures.Count == 0 && report.pipeline != "builtin";
            return report;
        }

        public static IEnumerable<(Material, string)> MaterialAssets(params string[] roots) =>
            AssetDatabase.FindAssets("t:Material", roots).Select(AssetDatabase.GUIDToAssetPath)
                .SelectMany(p => AssetDatabase.LoadAllAssetsAtPath(p).OfType<Material>().Select(m => (m, "asset:" + p)));

        public static IEnumerable<(Material, string)> SceneMaterials(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var r in root.GetComponentsInChildren<Renderer>(true))
                    foreach (var m in r.sharedMaterials)
                        yield return (m, "renderer:" + r.name);
                foreach (var t in root.GetComponentsInChildren<Terrain>(true))
                {
                    yield return (t.materialTemplate, "terrain-material:" + t.name);
                    var data = t.terrainData;
                    if (data == null) continue;
                    foreach (var tree in data.treePrototypes)
                        if (tree.prefab != null)
                            foreach (var r in tree.prefab.GetComponentsInChildren<Renderer>(true))
                                foreach (var m in r.sharedMaterials)
                                    yield return (m, "terrain-tree:" + tree.prefab.name);
                    foreach (var detail in data.detailPrototypes)
                        if (detail.usePrototypeMesh && detail.prototype != null)
                            foreach (var r in detail.prototype.GetComponentsInChildren<Renderer>(true))
                                foreach (var m in r.sharedMaterials)
                                    yield return (m, "terrain-detail:" + detail.prototype.name);
                }
            }
            if (scene == SceneManager.GetActiveScene() && RenderSettings.skybox != null) yield return (RenderSettings.skybox, "skybox");
        }
    }
}
