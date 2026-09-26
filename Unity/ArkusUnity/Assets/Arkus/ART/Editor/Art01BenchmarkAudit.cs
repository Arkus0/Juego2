using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Juego2.ART.Editor
{
    /// <summary>
    /// Small effective-scene audit for ART's composition claim. Positions are read from the
    /// tagged pieces, never hard-coded. The material check here is renderer-dependent and is
    /// re-run under the final URP pipeline; the renderer-independent geometry lives in
    /// <see cref="Art01StructuralAudit"/>.
    /// </summary>
    public static class Art01BenchmarkAudit
    {
        [Serializable] sealed class Support
        {
            public string point;
            public string hit;
            public float topY;
        }
        [Serializable] sealed class Report
        {
            public string schema = "juego2.art01.benchmark-audit@2";
            public string unityVersion;
            public string scene = "Assets/Arkus/ART/Art01Benchmark.unity";
            public string renderPipeline;
            public bool passed;
            public int taggedPieces;
            public int streetMeshColliders;
            public int dressedHumans;
            public Support[] supports;
            public string[] violations;
        }

        static Bounds RendererBounds(GameObject go)
        {
            var rs = go.GetComponentsInChildren<Renderer>();
            var b = rs.Length > 0 ? rs[0].bounds : new Bounds(go.transform.position, Vector3.zero);
            foreach (var r in rs) b.Encapsulate(r.bounds);
            return b;
        }

        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Arkus/ART/Art01Benchmark.unity", OpenSceneMode.Single);
            Physics.SyncTransforms();
            var issues = new List<string>();
            var pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            var report = new Report
            {
                unityVersion = Application.unityVersion,
                renderPipeline = pipeline == null ? "built-in" : pipeline.GetType().Name,
            };
            var tags = UnityEngine.Object.FindObjectsByType<Art01Piece>(FindObjectsSortMode.None);
            report.taggedPieces = tags.Length;
            foreach (var piece in tags)
                if (piece.presentationState != "KEEPER_READY")
                    issues.Add(piece.logicalId + " state=" + piece.presentationState);
            var byId = new Dictionary<string, Art01Piece>();
            foreach (var t in tags) byId[t.logicalId] = t;
            GameObject Find(string id) => byId.TryGetValue(id, out var p) ? p.gameObject : null;

            var road = Find("art01.street.s02_w12_casco_microB");
            report.streetMeshColliders = road == null ? 0 : road.GetComponents<MeshCollider>().Length;
            if (report.streetMeshColliders != 1) issues.Add("road lacks single MeshCollider owner");
            var plinthFaces = tags.Where(t => t.assemblyRole == "PLINTH" && t.logicalId.Contains("F01") && t.logicalId.Contains(".front"))
                .Select(t => RendererBounds(t.gameObject).min.z).ToList();
            if (road != null && plinthFaces.Count > 0 &&
                Mathf.Abs(road.GetComponent<MeshCollider>().bounds.max.z - plinthFaces.Min()) > 0.02f)
                issues.Add("micro-route B does not terminate on the F01 plinth face");
            foreach (var renderer in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            {
                if (renderer.GetComponent<TextMesh>() != null) continue;
                foreach (var material in renderer.sharedMaterials)
                    if (material == null || material.shader == null ||
                        material.shader.name.Contains("InternalErrorShader"))
                        issues.Add("missing/error material at " + renderer.gameObject.name);
            }
            foreach (var piece in tags)
                if (piece.assemblyRole == "SCALE_REFERENCE")
                {
                    report.dressedHumans++;
                    var animator = piece.GetComponentInChildren<Animator>(true);
                    if (animator == null || animator.avatar == null || !animator.avatar.isHuman ||
                        !animator.avatar.isValid || !animator.gameObject.name.Contains("Townsfolk_Forastero"))
                        issues.Add("unclothed/invalid human " + piece.logicalId);
                }
            if (report.dressedHumans < 1) issues.Add("no dressed human-scale inspection model");
            if (UnityEngine.Object.FindFirstObjectByType<Art01WalkInspector>() == null)
                issues.Add("no third-person walk inspector");

            var checks = new List<(string, Vector3, string)> {
                ("W12 road", new Vector3(0, 3, 0), "art01.street.s02_w12_casco_microB") };
            foreach (var (label, id) in new[] { ("F01 first step", "art01.f01.threshold.step"),
                ("F01 landing", "art01.f01.threshold.landing"), ("F01 interior", "art01.f01.interior.floor") })
            {
                var go = Find(id);
                if (go == null) { issues.Add("missing " + id); continue; }
                var b = RendererBounds(go);
                var probe = id.EndsWith("floor")
                    ? new Vector3(b.center.x, 3, b.min.z + 1.0f)
                    : new Vector3(b.center.x - 0.3f, 3, b.center.z);
                checks.Add((label, probe, id));
            }
            var supports = new List<Support>();
            foreach (var check in checks)
            {
                var hits = Physics.RaycastAll(check.Item2, Vector3.down, 4f);
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                string hit = hits.Length == 0 ? "NONE" : hits[0].collider.gameObject.name;
                supports.Add(new Support { point = check.Item1, hit = hit, topY = hits.Length == 0 ? -999 : hits[0].point.y });
                if (hit != check.Item3) issues.Add(check.Item1 + " support=" + hit + " expected=" + check.Item3);
            }
            report.supports = supports.ToArray();
            report.violations = issues.ToArray();
            report.passed = issues.Count == 0;
            var path = Path.GetFullPath(Path.Combine(Application.dataPath,
                "../../../Docs/evidence/WP-ART-01/UNITY_BENCHMARK_AUDIT.json"));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonUtility.ToJson(report, true) + "\n");
            Debug.Log("ART01_BENCHMARK_AUDIT " + (report.passed ? "GREEN" : "RED") +
                " pieces=" + report.taggedPieces + " issues=" + issues.Count + " pipeline=" + report.renderPipeline);
            if (!report.passed) throw new Exception("ART01_BENCHMARK_AUDIT_RED " + string.Join("; ", issues));
        }
    }
}
