using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Juego2.ART.Editor
{
    /// <summary>Small effective-scene audit for ART's specific composition claim.</summary>
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
            public string schema = "juego2.art01.benchmark-audit@1";
            public string unityVersion;
            public string scene = "Assets/Arkus/ART/Art01Benchmark.unity";
            public bool passed;
            public int taggedPieces;
            public int streetMeshColliders;
            public int continuousRoadSamples;
            public bool publicDoorBodyClear;
            public int dressedHumans;
            public Support[] supports;
            public string[] violations;
        }

        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Arkus/ART/Art01Benchmark.unity", OpenSceneMode.Single);
            Physics.SyncTransforms();
            var issues = new List<string>();
            var report = new Report { unityVersion = Application.unityVersion };
            var tags = UnityEngine.Object.FindObjectsByType<Art01Piece>(FindObjectsSortMode.None);
            report.taggedPieces = tags.Length;
            foreach (var piece in tags)
                if (piece.presentationState != "KEEPER_READY")
                    issues.Add(piece.logicalId + " state=" + piece.presentationState);
            var road = GameObject.Find("art01.street.s02_w12_casco_microB");
            report.streetMeshColliders = road == null ? 0 : road.GetComponents<MeshCollider>().Length;
            if (report.streetMeshColliders != 1) issues.Add("road lacks single MeshCollider owner");
            if (road != null && road.GetComponent<MeshCollider>().bounds.max.z > 23.85f)
                issues.Add("road collider overlaps F01 threshold support");
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

            var checks = new[] {
                ("W12 road", new Vector3(0,3,0), "art01.street.s02_w12_casco_microB"),
                ("F01 first step", new Vector3(-1,3,23.94f), "art01.f01.threshold.step"),
                ("F01 landing", new Vector3(-1,3,24.5f), "art01.f01.threshold.landing"),
                ("F01 interior", new Vector3(-1,3,25.4f), "art01.f01.interior.floor"),
            };
            var supports = new List<Support>();
            foreach (var check in checks)
            {
                var hits = Physics.RaycastAll(check.Item2, Vector3.down, 4f);
                Array.Sort(hits, (a,b) => a.distance.CompareTo(b.distance));
                string hit = hits.Length == 0 ? "NONE" : hits[0].collider.gameObject.name;
                supports.Add(new Support { point = check.Item1, hit = hit,
                    topY = hits.Length == 0 ? -999 : hits[0].point.y });
                if (hit != check.Item3) issues.Add(check.Item1 + " support=" + hit + " expected=" + check.Item3);
            }
            report.supports = supports.ToArray();
            for (float z = -35f; z <= 23.5f; z += 0.5f)
            {
                var hits = Physics.RaycastAll(new Vector3(0, 2.5f, z), Vector3.down, 3f);
                bool roadSupports = false;
                foreach (var hit in hits)
                    if (hit.collider.gameObject.name == "art01.street.s02_w12_casco_microB")
                        roadSupports = true;
                report.continuousRoadSamples++;
                if (!roadSupports) issues.Add("road support missing at local z=" + z);
            }
            report.publicDoorBodyClear = Physics.OverlapCapsule(
                new Vector3(-1, 0.57f, 25f), new Vector3(-1, 1.56f, 25f), 0.24f).Length == 0;
            if (!report.publicDoorBodyClear) issues.Add("F01 public doorway lacks 0.48 m body clearance");
            report.violations = issues.ToArray();
            report.passed = issues.Count == 0;
            var path = Path.GetFullPath(Path.Combine(Application.dataPath,
                "../../../Docs/evidence/WP-ART-01/UNITY_BENCHMARK_AUDIT.json"));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonUtility.ToJson(report, true) + "\n");
            Debug.Log("ART01_BENCHMARK_AUDIT " + (report.passed ? "GREEN" : "RED") +
                " pieces=" + report.taggedPieces + " issues=" + issues.Count);
            if (!report.passed) throw new Exception("ART01_BENCHMARK_AUDIT_RED " + string.Join("; ", issues));
        }
    }
}
