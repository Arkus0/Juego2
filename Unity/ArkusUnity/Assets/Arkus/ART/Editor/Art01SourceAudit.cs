using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Juego2.ART.Editor
{
    /// <summary>Read-only effective import measurement for ART's selected external models.</summary>
    public static class Art01SourceAudit
    {
        const string ExternalRoot = "Assets/Arkus/ART/External";

        [Serializable] public sealed class Row
        {
            public string path;
            public Vector3 rootScale;
            public Vector3 rootRotationEuler;
            public Vector3 rootPosition;
            public Vector3 boundsCenter;
            public Vector3 boundsSize;
            public float importerGlobalScale;
            public bool importerUseFileScale;
            public string[] materials;
            public string[] shaders;
        }

        [Serializable] public sealed class Report
        {
            public string schema = "juego2.art01.unity-source-audit@1";
            public string unityVersion;
            public List<Row> rows = new List<Row>();
        }

        public static void Measure()
        {
            var report = new Report { unityVersion = Application.unityVersion };
            var guids = AssetDatabase.FindAssets("t:Model", new[] { ExternalRoot });
            Array.Sort(guids, StringComparer.Ordinal);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (asset == null) throw new Exception("ART01_MODEL_MISSING " + path);
                var instance = UnityEngine.Object.Instantiate(asset);
                try
                {
                    var renderers = instance.GetComponentsInChildren<Renderer>(true);
                    if (renderers.Length == 0) throw new Exception("ART01_RENDERER_MISSING " + path);
                    Bounds bounds = renderers[0].bounds;
                    var materialNames = new SortedSet<string>(StringComparer.Ordinal);
                    var shaderNames = new SortedSet<string>(StringComparer.Ordinal);
                    foreach (var renderer in renderers)
                    {
                        bounds.Encapsulate(renderer.bounds);
                        foreach (var material in renderer.sharedMaterials)
                        {
                            if (material == null) continue;
                            materialNames.Add(material.name);
                            shaderNames.Add(material.shader == null ? "NULL" : material.shader.name);
                        }
                    }
                    var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                    report.rows.Add(new Row
                    {
                        path = path,
                        rootScale = asset.transform.localScale,
                        rootRotationEuler = asset.transform.eulerAngles,
                        rootPosition = asset.transform.localPosition,
                        boundsCenter = bounds.center,
                        boundsSize = bounds.size,
                        importerGlobalScale = importer == null ? -1 : importer.globalScale,
                        importerUseFileScale = importer != null && importer.useFileScale,
                        materials = new List<string>(materialNames).ToArray(),
                        shaders = new List<string>(shaderNames).ToArray(),
                    });
                }
                finally { UnityEngine.Object.DestroyImmediate(instance); }
            }
            report.rows.Sort((a, b) => StringComparer.Ordinal.Compare(a.path, b.path));
            var output = Path.GetFullPath(Path.Combine(Application.dataPath,
                "../../../Docs/evidence/WP-ART-01/UNITY_SOURCE_AUDIT_NORMALIZED.json"));
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllText(output, JsonUtility.ToJson(report, true) + "\n");
            Debug.Log("ART01_SOURCE_AUDIT_GREEN rows=" + report.rows.Count + " output=" + output);
        }
    }
}
