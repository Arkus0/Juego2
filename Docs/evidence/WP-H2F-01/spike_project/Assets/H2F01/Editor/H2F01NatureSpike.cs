using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Juego2.ART;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>
    /// S04 nature spike. (A) Project-owned deterministic seeded scatter of ART's admitted Quaternius Nature pieces on
    /// the pinned ART-01 valley banks, with no-go masks for traversable surfaces (road/threshold/floor), structures
    /// (houses/walls) and the river; determinism and mask compliance are measured, and a negative run with the road
    /// mask disabled proves the audit detects violations. (B) The same rule through native Terrain trees/details.
    /// </summary>
    public static class H2F01NatureSpike
    {
        const int Seed = 20260927;
        // scale bands copied from ART-01's reviewed benchmark placements (ART is visual authority); density bounded
        static readonly (string kind, string model, float spacing, float weight, float clearance, float scaleMin, float scaleMax)[] Kinds =
        {
            ("tree", "CommonTree_1", 7.0f, 0.05f, 2.2f, 0.62f, 0.76f),
            ("bush", "Bush_Common", 2.6f, 0.12f, 1.0f, 0.45f, 0.55f),
            ("bush_flowers", "Bush_Common_Flowers", 3.0f, 0.06f, 1.0f, 0.50f, 0.58f),
            ("fern", "Fern_1", 1.6f, 0.18f, 0.6f, 0.12f, 0.16f),
            ("grass", "Grass_Common_Short", 1.1f, 0.30f, 0.35f, 0.45f, 0.6f),
            ("rock", "Rock_Medium_1", 4.0f, 0.04f, 0.8f, 0.45f, 0.6f),
        };

        [Serializable]
        class Result
        {
            public int placed, candidates; public string digestRun1, digestRun2; public bool deterministic;
            public int violationsRoad, violationsStructure, violationsRiver;
            public int negativeRoadMaskOffViolations;
            public string countsByKind; public float waterY;
            public int terrainTrees, terrainDetailCells; public string terrainGrassShader; public bool terrainAuditGreen;
            public List<string> notes = new List<string>();
        }

        static float waterY;
        static Bounds water;

        public static void Run()
        {
            var r = new Result();
            OpenArtScene();
            H2F01RenderSpike.BaselineLook();
            var waterR = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).Where(x => x.sharedMaterials.Any(m => m != null && m.name == "Water")).ToList();
            water = waterR.Select(x => x.bounds).Aggregate((a, c) => { a.Encapsulate(c); return a; });
            waterY = water.max.y; r.waterY = waterY;
            // temporary placement-only colliders on the non-collidable ART scenic ground (removed before capture)
            var scenic = UnityEngine.Object.FindObjectsByType<Art01Piece>(FindObjectsSortMode.None).Where(p => p.assemblyRole == "SCENIC").ToList();
            var temp = scenic.SelectMany(p => p.GetComponentsInChildren<MeshFilter>()).Select(mf => mf.gameObject.AddComponent<MeshCollider>()).ToList();
            foreach (var t in temp) t.gameObject.layer = 31;
            Physics.SyncTransforms();
            var area = scenic.SelectMany(p => p.GetComponentsInChildren<Renderer>()).Select(x => x.bounds).Aggregate((a, c) => { a.Encapsulate(c); return a; });
            r.notes.Add($"scenic area {area.min:F1}..{area.max:F1}; water top y {waterY:F2}");

            var run1 = Scatter(area, true, out var placed1, out var cand, "run1");
            r.digestRun1 = run1; r.placed = placed1.Count; r.candidates = cand;
            (r.violationsRoad, r.violationsStructure, r.violationsRiver) = Audit(placed1);
            r.countsByKind = string.Join(",", placed1.GroupBy(p => p.kind).Select(g => g.Key + "=" + g.Count()));
            foreach (var p in placed1) UnityEngine.Object.DestroyImmediate(p.go);
            var run2 = Scatter(area, true, out var placed2, out _, "run2");
            r.digestRun2 = run2; r.deterministic = run1 == run2;
            // negative control: road/threshold mask off -> audit must find road violations
            var neg = Scatter(area, false, out var placedNeg, out _, "negative");
            r.negativeRoadMaskOffViolations = Audit(placedNeg).road;
            foreach (var p in placedNeg) UnityEngine.Object.DestroyImmediate(p.go);
            foreach (var t in temp) UnityEngine.Object.DestroyImmediate(t);
            // captures of run2 on the real bank/route
            H2F01RenderSpike.PoseHumans(H2F01RenderSpike.UalClip("Idle_Loop"));
            float y0 = RoadY(0, -30);
            Shot("s04/A_scatter_bridge_view.png", new Vector3(0, y0 + 1.8f, -31), new Vector3(-14, waterY + 1, -22));
            Shot("s04/A_scatter_bank_low.png", new Vector3(-10, waterY + 2.2f, -36), new Vector3(-3, waterY + 0.8f, -22));
            Shot("s04/A_scatter_w12_edge.png", new Vector3(0, RoadY(0, -6) + 1.65f, -12), new Vector3(-9, RoadY(0, 2), 4));
            Shot("s04/A_scatter_overview.png", new Vector3(-24, 22, -46), new Vector3(0, 0, -12));
            AnimationMode.StopAnimationMode();
            TerrainRoute(r);
            WriteJson("s04/result.json", JsonUtility.ToJson(r, true));
            Log($"H2F01_S04_DONE placed={r.placed} deterministic={r.deterministic} violations road/struct/river={r.violationsRoad}/{r.violationsStructure}/{r.violationsRiver} negativeRoad={r.negativeRoadMaskOffViolations} kinds={r.countsByKind} terrainTrees={r.terrainTrees} terrainAudit={r.terrainAuditGreen}");
        }

        class Placed { public string kind; public GameObject go; public Vector3 pos; }

        static string Scatter(Bounds area, bool roadMask, out List<Placed> placed, out int candidates, string label)
        {
            var rng = new System.Random(Seed);
            placed = new List<Placed>();
            candidates = 0;
            var root = new GameObject("S04_SCATTER_" + label + "_NOT_KEEPER");
            const float cell = 1.3f;
            var lines = new List<string>();
            for (float x = area.min.x; x <= area.max.x; x += cell)
                for (float z = area.min.z; z <= area.max.z; z += cell)
                {
                    float jx = (float)rng.NextDouble() * cell, jz = (float)rng.NextDouble() * cell, pick = (float)rng.NextDouble();
                    float yaw = (float)rng.NextDouble() * 360f, scaleT = (float)rng.NextDouble();
                    candidates++;
                    var p = new Vector3(x + jx, 0, z + jz);
                    if (!Physics.Raycast(p + Vector3.up * 60, Vector3.down, out var hit, 120)) continue;
                    if (hit.collider.gameObject.layer != 31) continue; // only land on scenic ground, never on road/roof/wall
                    if (Vector3.Angle(hit.normal, Vector3.up) > 32f) continue;
                    var kindSpec = PickKind(pick);
                    float scale = Mathf.Lerp(kindSpec.scaleMin, kindSpec.scaleMax, scaleT);
                    if (pick > 0.75f) continue; // bounded coverage: a quarter of the cells stay bare soil
                    if (!Allowed(hit.point, kindSpec.clearance, roadMask)) continue;
                    if (placed.Any(q => q.kind == kindSpec.kind && (q.pos - hit.point).sqrMagnitude < kindSpec.spacing * kindSpec.spacing)) continue;
                    if (kindSpec.kind == "tree" && placed.Any(q => q.kind == "tree" && (q.pos - hit.point).sqrMagnitude < 36)) continue;
                    var go = Spawn(kindSpec.model, root.transform, hit.point, yaw, scale);
                    placed.Add(new Placed { kind = kindSpec.kind, go = go, pos = hit.point });
                    lines.Add($"{kindSpec.kind}|{Mathf.RoundToInt(hit.point.x * 1000)}|{Mathf.RoundToInt(hit.point.y * 1000)}|{Mathf.RoundToInt(hit.point.z * 1000)}|{Mathf.RoundToInt(yaw * 10)}|{Mathf.RoundToInt(scale * 1000)}");
                }
            return Sha(Encoding.UTF8.GetBytes(string.Join("\n", lines)));
        }

        static (string kind, string model, float spacing, float weight, float clearance, float scaleMin, float scaleMax) PickKind(float pick)
        {
            float acc = 0;
            foreach (var k in Kinds) { acc += k.weight; if (pick <= acc) return k; }
            return Kinds.Last();
        }

        /// <summary>No-go masks: traversable surfaces (road/threshold/floor), structures, and the river edge.</summary>
        static bool Allowed(Vector3 p, float clearance, bool roadMask)
        {
            if (p.y < waterY + 0.12f && Flat(p, water)) return false;                        // river + wet edge
            foreach (var c in Physics.OverlapSphere(p + Vector3.up * 0.5f, clearance + 0.5f, ~(1 << 31), QueryTriggerInteraction.Ignore))
            {
                var piece = c.GetComponentInParent<Art01Piece>();
                bool traversable = piece != null && (piece.collisionRole ?? "").ToUpperInvariant().Contains("TRAVERS");
                if (traversable && !roadMask) continue;
                return false;                                                              // road/threshold, house, wall, prop
            }
            return true;
        }

        static bool Flat(Vector3 p, Bounds b) => p.x > b.min.x - 0.8f && p.x < b.max.x + 0.8f && p.z > b.min.z - 0.8f && p.z < b.max.z + 0.8f;

        static (int road, int structure, int river) Audit(List<Placed> placed)
        {
            int road = 0, structure = 0, river = 0;
            foreach (var q in placed)
            {
                foreach (var c in Physics.OverlapSphere(q.pos + Vector3.up * 0.3f, 0.35f, ~(1 << 31), QueryTriggerInteraction.Ignore))
                {
                    if (c.transform.IsChildOf(q.go.transform)) continue;
                    var piece = c.GetComponentInParent<Art01Piece>();
                    if (piece != null && (piece.collisionRole ?? "").ToUpperInvariant().Contains("TRAVERS")) road++; else structure++;
                }
                if (q.pos.y < waterY + 0.05f && Flat(q.pos, water)) river++;
            }
            return (road, structure, river);
        }

        static GameObject Spawn(string model, Transform parent, Vector3 at, float yaw, float scale)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Arkus/ART/External/Nature/Models/{model}.fbx");
            var holder = new GameObject(model);
            holder.transform.SetParent(parent, false);
            holder.transform.SetPositionAndRotation(at, Quaternion.Euler(0, yaw, 0));
            holder.transform.localScale = Vector3.one * scale;
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, holder.transform);
            inst.transform.localRotation = prefab.transform.localRotation;
            inst.transform.localScale = prefab.transform.localScale;
            RemapNature(inst);
            return holder;
        }

        static void RemapNature(GameObject go)
        {
            string Id(string n) => n.Contains("Bark") ? "Bark" : n.Contains("Leaves_NormalTree") ? "Leaves"
                : n.Contains("Leaves") ? "LeavesGeneric" : n.Contains("Flowers") ? "Flowers" : n.Contains("Rock") ? "Rock"
                : n == "Grass" || n.Contains("Grass") ? "GrassGround" : "LeavesGeneric";
            foreach (var r in go.GetComponentsInChildren<Renderer>(true))
                r.sharedMaterials = r.sharedMaterials.Select(m => AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{Id(m.name)}.mat")
                    ?? AssetDatabase.LoadAssetAtPath<Material>("Assets/Arkus/ART/Materials/Leaves.mat")).ToArray();
        }

        /// <summary>(B) native Terrain trees + detail meshes with the same kind of exclusion strip.</summary>
        static void TerrainRoute(Result r)
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            H2F01RenderSpike.BaselineLook();
            var data = new TerrainData { heightmapResolution = 129, size = new Vector3(40, 6, 40) };
            var h = new float[129, 129];
            for (int z = 0; z < 129; z++) for (int x = 0; x < 129; x++) h[z, x] = Mathf.PerlinNoise(x * 0.04f, z * 0.04f) * 0.35f;
            data.SetHeights(0, 0, h);
            data.terrainLayers = new[] { new TerrainLayer { diffuseTexture = Texture2D.grayTexture, tileSize = new Vector2(4, 4), diffuseRemapMax = new Vector4(0.36f, 0.43f, 0.37f, 0.05f) } };
            // Terrain prototypes render the prefab's own materials (no per-instance remap): the FBX's embedded,
            // pre-URP Standard materials render magenta (recorded negative capture). Use URP prefab variants.
            var tree = UrpPrefab("CommonTree_1", 0.7f);
            var grass = UrpPrefab("Grass_Common_Short", 0.5f);
            data.treePrototypes = new[] { new TreePrototype { prefab = tree } };
            data.detailPrototypes = new[] { new DetailPrototype { prototype = grass, usePrototypeMesh = true, renderMode = DetailRenderMode.VertexLit, useInstancing = true, minWidth = 0.9f, maxWidth = 1.2f, minHeight = 0.9f, maxHeight = 1.2f } };
            data.SetDetailResolution(128, 16);
            var rng = new System.Random(Seed);
            var trees = new List<TreeInstance>();
            var detail = new int[128, 128];
            bool Road(float nx) => Mathf.Abs(nx - 0.5f) < 0.06f; // 2.4 m strip down the middle = mask
            for (int i = 0; i < 90; i++)
            {
                float nx = (float)rng.NextDouble(), nz = (float)rng.NextDouble();
                if (Road(nx) || Mathf.Abs(nx - 0.5f) < 0.11f) continue;
                trees.Add(new TreeInstance { position = new Vector3(nx, 0, nz), widthScale = 1, heightScale = 1, color = Color.white, lightmapColor = Color.white, prototypeIndex = 0, rotation = (float)rng.NextDouble() * 6.28f });
            }
            for (int z = 0; z < 128; z++) for (int x = 0; x < 128; x++) if (!Road(x / 127f) && rng.NextDouble() < 0.35) detail[z, x] = 2;
            data.SetTreeInstances(trees.ToArray(), true);
            data.SetDetailLayer(0, 0, 0, detail);
            r.terrainTrees = trees.Count;
            r.terrainDetailCells = detail.Cast<int>().Count(v => v > 0);
            var go = UnityEngine.Terrain.CreateTerrainGameObject(data);
            go.transform.position = new Vector3(-20, 0, -20);
            var terrain = go.GetComponent<UnityEngine.Terrain>();
            terrain.materialTemplate = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/TerrainLit.mat");
            // tree/detail prototype materials: converted ART palette
            Shot("s04/B_terrain_trees_details.png", new Vector3(0, 3.5f, -19), new Vector3(0, 1.2f, 0));
            // renderer-based audit cannot see terrain trees/details: audit the prototypes explicitly
            var protoMats = data.treePrototypes.SelectMany(t => t.prefab.GetComponentsInChildren<Renderer>(true)).Concat(
                data.detailPrototypes.Where(d => d.prototype != null).SelectMany(d => d.prototype.GetComponentsInChildren<Renderer>(true)))
                .SelectMany(x => x.sharedMaterials).Where(m => m != null).Distinct().ToList();
            var bad = protoMats.Where(m => !m.shader.isSupported || m.shader.name == "Standard" || m.shader.name.Contains("Error")).Select(m => m.name + ":" + m.shader.name).ToList();
            r.terrainAuditGreen = bad.Count == 0;
            r.terrainGrassShader = string.Join(",", protoMats.Select(m => m.name + ":" + m.shader.name));
            if (bad.Count > 0) r.notes.Add("terrain prototype materials failing: " + string.Join(",", bad));
            r.notes.Add("terrain trees/details are serialized inside TerrainData (asset), prefab scatter as scene objects");
        }

        /// <summary>
        /// Terrain tree/detail prototypes need the mesh on the prefab root and ignore the FBX child's -90 X import
        /// rotation, so the Nature model is baked into a root-level derivative mesh (URP materials, submeshes kept).
        /// </summary>
        static GameObject UrpPrefab(string model, float scale)
        {
            if (!AssetDatabase.IsValidFolder("Assets/H2F01/Nature")) AssetDatabase.CreateFolder("Assets/H2F01", "Nature");
            var source = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Arkus/ART/External/Nature/Models/{model}.fbx");
            var temp = new GameObject("bake_" + model);
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(source, temp.transform);
            inst.transform.localRotation = source.transform.localRotation;
            inst.transform.localScale = source.transform.localScale * scale;
            RemapNature(inst);
            var parts = inst.GetComponentsInChildren<MeshFilter>();
            var combine = new List<CombineInstance>();
            var mats = new List<Material>();
            foreach (var mf in parts)
            {
                var mr = mf.GetComponent<MeshRenderer>();
                for (int sub = 0; sub < mf.sharedMesh.subMeshCount; sub++)
                {
                    combine.Add(new CombineInstance { mesh = mf.sharedMesh, subMeshIndex = sub, transform = temp.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix });
                    mats.Add(mr.sharedMaterials[Mathf.Min(sub, mr.sharedMaterials.Length - 1)]);
                }
            }
            var baked = new Mesh { name = model + "_rootbaked" };
            baked.CombineMeshes(combine.ToArray(), false, true);
            var meshPath = $"Assets/H2F01/Nature/{model}_rootbaked.asset";
            AssetDatabase.DeleteAsset(meshPath);
            AssetDatabase.CreateAsset(baked, meshPath);
            var root = new GameObject(model + "_URP");
            root.AddComponent<MeshFilter>().sharedMesh = baked;
            root.AddComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
            var saved = PrefabUtility.SaveAsPrefabAsset(root, $"Assets/H2F01/Nature/{model}_URP.prefab");
            UnityEngine.Object.DestroyImmediate(root);
            UnityEngine.Object.DestroyImmediate(temp);
            return saved;
        }

        static float RoadY(float x, float z) => Physics.Raycast(new Vector3(x, 40, z), Vector3.down, out var h, 80) ? h.point.y : 0;

        static void Shot(string path, Vector3 pos, Vector3 target)
        {
            var cam = CameraAt("s04_cam", pos, target, 55);
            cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            Image(cam, path);
            UnityEngine.Object.DestroyImmediate(cam.gameObject);
        }
    }
}
