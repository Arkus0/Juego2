using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>
    /// S02 topography + linear + navigation spike. Next to the pinned ART-01 authored route (untouched), it
    /// realizes a disposable copy of the W12 climb from a Unity Splines centreline with project-owned profile
    /// extrusion, an X1 (5.5 m) T-junction with kerb cut + corner fillets, a retaining wall, a Source fence via
    /// SplineInstantiate, Terrain banks, a ProBuilder threshold step, and one AI Navigation bake over both.
    /// </summary>
    public static class H2F01WorldSpike
    {
        const float X1W = 5.5f, W12W = 2.8f, KerbW = 0.2f, KerbH = 0.12f, CornerR = 1.2f;
        const float OffX = 18f, JunctionZ = -20f, EndZ = 12f;
        static Transform root;
        static readonly Result R = new Result();

        [Serializable]
        class Result
        {
            public string splinesVersion = "2.9.1", navVersion = "2.0.15", probuilderVersion = "6.1.2";
            public float w12Length, x1Length;
            public float w12WidthMin = 99, w12WidthMax, x1WidthMin = 99, x1WidthMax;
            public int junctionSamples, junctionHoles, junctionSteps;
            public float leftSeamMaxGap, wallFootMaxGap;
            public int fenceInstances; public bool fenceInstancesSerialized;
            public string artPath, splinePath, bridgeToBar; public float artPathLength, splinePathLength;
            public float agentRadius, agentStep, agentSlope; public int navTriangles; public bool navOnScenic;
            public string probuilderBefore, probuilderAfter; public int probuilderVertices;
            public List<string> notes = new List<string>();
        }

        public static void Run()
        {
            ConfigureAgent();
            // Phase A: the pinned ART-01 authored route (read-only): centreline heights + one nav bake/path.
            OpenArtScene();
            var heights = new List<float>();
            for (float z = JunctionZ; z <= EndZ + 0.01f; z += 2f) heights.Add(RoadY(0, z));
            var artHost = new GameObject("H2F01_S02_ART_NAV_DIAGNOSTIC").transform;
            var artTri = Bake(artHost);
            R.navOnScenic = NavMesh.SamplePosition(new Vector3(-8, RoadY(-8, 0), 0), out _, 0.3f, NavMesh.AllAreas);
            R.artPath = PathBetween(new Vector3(0, RoadY(0, -30), -30), new Vector3(-1f, RoadY(0, 22f) + 0.4f, 29f), out R.artPathLength);
            H2F01RenderSpike.BaselineLook();
            root = artHost;
            OverlayNav(artTri);
            H2F01RenderSpike.PoseHumans(H2F01RenderSpike.UalClip("Idle_Loop"));
            var overlay = root.Find("navmesh_overlay_diagnostic").gameObject;
            overlay.SetActive(true);
            Shot("s02/navmesh_art_topdown.png", new Vector3(0, 75, -2), new Vector3(0, 0, -1.99f), 55);
            Shot("s02/navmesh_art_f01_threshold.png", new Vector3(2.2f, RoadY(0, 20) + 3.2f, 19.5f), new Vector3(0, RoadY(0, 24), 26f));
            AnimationMode.StopAnimationMode();

            // Phase B: clean disposable scene. W12 copy from the same heights with a bend, X1 T-junction, edges.
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var sun = UnityEngine.Object.FindFirstObjectByType<Light>();
            sun.color = new Color(0.83f, 0.89f, 0.93f); sun.intensity = 0.9f; sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(43, -28, 0);
            H2F01RenderSpike.BaselineLook();
            root = new GameObject("H2F01_S02_SPIKE_NOT_KEEPER").transform;
            var cobble = ArtMat("Cobble"); var trim = ArtMat("StoneTrim"); var stone = ArtMat("Stone");
            var pts = new List<Vector3>();
            for (int i = 0; i < heights.Count; i++)
                pts.Add(new Vector3(OffX + (i == 0 ? 0 : 1.4f * Mathf.Sin(i * 0.33f)), heights[i], JunctionZ + 2f * i));
            float junctionY = heights[0];
            var w12 = MakeSpline("W12_centreline_spline", pts);
            var x1 = MakeSpline("X1_centreline_spline", new List<Vector3> {
                new Vector3(OffX - 14, junctionY, JunctionZ), new Vector3(OffX, junctionY, JunctionZ), new Vector3(OffX + 14, junctionY, JunctionZ) });
            R.w12Length = w12.Spline.GetLength(); R.x1Length = x1.Spline.GetLength();
            float open = W12W / 2 + KerbW + CornerR;
            Extrude(x1, X1W, 0, R.x1Length, cobble, trim, leftOpen: new List<Vector2> { new Vector2(14 - open, 14 + open) }, rightOpen: null, name: "X1");
            float start = X1W / 2 + KerbW;
            float apronEnd = X1W / 2 + KerbW + CornerR;
            Extrude(w12, W12W, apronEnd, R.w12Length, cobble, trim, leftOpen: null, rightOpen: null, name: "W12");
            JunctionFillets(junctionY, trim);
            JunctionApron(w12, junctionY, apronEnd);
            RetainingWall(w12, start, stone);
            Fence(w12, start);
            Terrain(w12, x1);
            ProBuilderStep(w12);
            Measure(w12, x1);
            var tri = Bake(root);
            R.navTriangles = tri.indices.Length / 3;
            R.splinePath = PathBetween(new Vector3(OffX - 10, junctionY, JunctionZ), w12.transform.TransformPoint((Vector3)w12.Spline.EvaluatePosition(0.97f)), out R.splinePathLength);
            OverlayNav(tri);
            Capture();
            if (!AssetDatabase.IsValidFolder("Assets/H2F01/Scenes")) AssetDatabase.CreateFolder("Assets/H2F01", "Scenes");
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), "Assets/H2F01/Scenes/S02_SplineRoute.unity");
            WriteJson("s02/result.json", JsonUtility.ToJson(R, true));
            Log($"H2F01_S02_DONE w12Width=[{R.w12WidthMin:F3},{R.w12WidthMax:F3}] x1Width=[{R.x1WidthMin:F3},{R.x1WidthMax:F3}] junctionHoles={R.junctionHoles}/{R.junctionSamples} steps={R.junctionSteps} seam={R.leftSeamMaxGap:F3} art={R.artPath}:{R.artPathLength:F1} spline={R.splinePath}:{R.splinePathLength:F1} fence={R.fenceInstances} pb={R.probuilderAfter}");
        }

        /// <summary>Project NavMesh agent sized to the accepted door/lane classes (default Humanoid 0.5 m cannot pass 1.0 m doors).</summary>
        internal static void ConfigureAgent()
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/NavMeshAreas.asset")[0];
            var so = new SerializedObject(asset);
            var s0 = so.FindProperty("m_Settings").GetArrayElementAtIndex(0);
            s0.FindPropertyRelative("agentRadius").floatValue = 0.28f;
            s0.FindPropertyRelative("agentHeight").floatValue = 1.8f;
            s0.FindPropertyRelative("agentClimb").floatValue = 0.3f;
            s0.FindPropertyRelative("agentSlope").floatValue = 40f;
            so.ApplyModifiedPropertiesWithoutUndo();
            var st = NavMesh.GetSettingsByID(0);
            R.agentRadius = st.agentRadius; R.agentStep = st.agentClimb; R.agentSlope = st.agentSlope;
        }

        static NavMeshTriangulation Bake(Transform host)
        {
            var surface = host.gameObject.AddComponent<NavMeshSurface>();
            surface.agentTypeID = 0;
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.overrideVoxelSize = true; surface.voxelSize = 0.07f;
            NavMesh.RemoveAllNavMeshData();
            surface.BuildNavMesh();
            return NavMesh.CalculateTriangulation();
        }

        // ------------------------------------------------------------------ realization

        static Material ArtMat(string id) => AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{id}.mat");

        static float RoadY(float x, float z) =>
            Physics.Raycast(new Vector3(x, 40, z), Vector3.down, out var hit, 80) ? hit.point.y : 0;

        static SplineContainer MakeSpline(string name, List<Vector3> points)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root, false);
            var c = go.AddComponent<SplineContainer>();
            c.Spline.Clear();
            foreach (var p in points) c.Spline.Add(new BezierKnot((float3)p), TangentMode.AutoSmooth);
            return c;
        }

        static void Frame(SplineContainer c, float d, out Vector3 pos, out Vector3 fwd, out Vector3 right)
        {
            var s = c.Spline;
            float t = Mathf.Clamp01(d / s.GetLength());
            SplineUtility.Evaluate(s, t, out float3 p, out float3 tan, out float3 up);
            pos = c.transform.TransformPoint((Vector3)p);
            fwd = ((Vector3)tan).normalized;
            right = Vector3.Cross(Vector3.up, fwd).normalized;
        }

        static bool Inside(List<Vector2> openings, float d) => openings != null && openings.Any(o => d >= o.x && d <= o.y);

        /// <summary>Project-owned profile extrusion: crowned traversable surface (sole collider) + kerbs outside it.</summary>
        static void Extrude(SplineContainer c, float width, float d0, float d1, Material road, Material kerb,
            List<Vector2> leftOpen, List<Vector2> rightOpen, string name)
        {
            var roadV = new List<Vector3>(); var roadUV = new List<Vector2>(); var roadT = new List<int>();
            var kerbV = new List<Vector3>(); var kerbT = new List<int>();
            const float step = 0.5f;
            int rows = Mathf.CeilToInt((d1 - d0) / step);
            float[] lateral = { -width / 2, -width / 4, 0, width / 4, width / 2 };
            float[] crown = { 0, 0.02f, 0.03f, 0.02f, 0 };
            for (int r = 0; r <= rows; r++)
            {
                float d = Mathf.Min(d0 + r * step, d1);
                Frame(c, d, out var p, out var fwd, out var right);
                for (int k = 0; k < lateral.Length; k++)
                {
                    roadV.Add(p + right * lateral[k] + Vector3.up * crown[k]);
                    roadUV.Add(new Vector2(lateral[k] / 1.5f, d / 1.5f));
                }
                if (r > 0)
                    for (int k = 0; k < lateral.Length - 1; k++)
                    {
                        int a = (r - 1) * lateral.Length + k, b = a + 1, e = a + lateral.Length, f = e + 1;
                        roadT.AddRange(new[] { a, e, b, b, e, f });
                    }
                // kerb quads (outer face, top, inner face) per side, skipped inside openings
                if (r > 0)
                {
                    float dPrev = d0 + (r - 1) * step;
                    Frame(c, dPrev, out var p0, out _, out var right0);
                    foreach (int side in new[] { -1, 1 })
                    {
                        var openList = side < 0 ? leftOpen : rightOpen;
                        if (Inside(openList, (dPrev + d) / 2)) continue;
                        KerbSegment(kerbV, kerbT, p0, right0, p, right, side, width);
                    }
                }
            }
            var go = MeshGo(name + "_road_traversable", roadV, roadUV, roadT, road, collider: true);
            MeshGo(name + "_kerbs_edge", kerbV, WorldUV(kerbV), kerbT, kerb, collider: false);
        }

        static void KerbSegment(List<Vector3> v, List<int> t, Vector3 p0, Vector3 r0, Vector3 p1, Vector3 r1, int side, float width)
        {
            // profile (lateral from edge, height): inner foot, inner top, outer top, outer foot (embedded)
            (float, float)[] prof = { (0, 0), (0, KerbH), (KerbW, KerbH), (KerbW, -0.15f) };
            int b = v.Count;
            foreach (var (p, r) in new[] { (p0, r0), (p1, r1) })
                foreach (var (l, h) in prof)
                    v.Add(p + r * side * (width / 2 + l) + Vector3.up * h);
            for (int k = 0; k < prof.Length - 1; k++)
            {
                int a = b + k, c = a + 1, e = b + prof.Length + k, f = e + 1;
                if (side > 0) t.AddRange(new[] { a, c, e, c, f, e }); else t.AddRange(new[] { a, e, c, c, e, f });
            }
        }

        /// <summary>Simple world-projected UVs (x+z along, y up) so tiled Source textures read at metric scale.</summary>
        static List<Vector2> WorldUV(List<Vector3> v) => v.Select(p => new Vector2((p.x + p.z) / 1.5f, p.y / 1.5f)).ToList();

        static GameObject MeshGo(string name, List<Vector3> v, List<Vector2> uv, List<int> t, Material m, bool collider)
        {
            var mesh = new Mesh { name = name, indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            mesh.SetVertices(v);
            if (uv != null) mesh.SetUVs(0, uv);
            mesh.SetTriangles(t, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            var go = new GameObject(name);
            go.transform.SetParent(root, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = m;
            if (collider) go.AddComponent<MeshCollider>().sharedMesh = mesh;
            return go;
        }

        /// <summary>Quarter-circle kerb returns joining W12's kerbs to X1's north kerb line.</summary>
        static void JunctionFillets(float y, Material trim)
        {
            var v = new List<Vector3>(); var t = new List<int>();
            float edgeZ = JunctionZ + X1W / 2; // X1 north traversable edge
            foreach (int side in new[] { -1, 1 })
            {
                // circle centre sits outside both kerbs; arc from X1 kerb line to W12 kerb line
                var centre = new Vector3(OffX + side * (W12W / 2 + KerbW + CornerR), y, edgeZ + KerbW + CornerR);
                const int n = 8;
                for (int k = 0; k < n; k++)
                {
                    float a0 = Mathf.PI / 2 * k / n, a1 = Mathf.PI / 2 * (k + 1) / n;
                    Vector3 Dir(float a) => new Vector3(-side * Mathf.Cos(a), 0, -Mathf.Sin(a));
                    var p0 = centre + Dir(a0) * (CornerR + KerbW); var p1 = centre + Dir(a1) * (CornerR + KerbW);
                    // kerb inner edge at radius CornerR+KerbW (facing road), outer at CornerR
                    KerbSegmentRadial(v, t, centre, p0, p1);
                }
            }
            MeshGo("junction_kerb_fillets", v, WorldUV(v), t, trim, false);
        }

        /// <summary>Apron from X1's north edge (fillet mouth) to W12's real first cross-section.</summary>
        static void JunctionApron(SplineContainer w12, float y, float dW12)
        {
            float edgeZ = JunctionZ + X1W / 2;
            float half = W12W / 2 + KerbW + CornerR;
            Frame(w12, dW12, out var p, out _, out var right);
            var av = new List<Vector3> {
                new Vector3(OffX - half, y, edgeZ), new Vector3(OffX + half, y, edgeZ),
                p - right * (W12W / 2), p + right * (W12W / 2) };
            var at = new List<int> { 0, 2, 1, 1, 2, 3 };
            MeshGo("junction_apron_traversable", av, null, at, ArtMat("Cobble"), true);
        }

        static void KerbSegmentRadial(List<Vector3> v, List<int> t, Vector3 centre, Vector3 innerA, Vector3 innerB)
        {
            Vector3 Out(Vector3 p) => centre + (p - centre).normalized * CornerR;
            (Vector3, float)[] ring(Vector3 p) => new[] { (p, 0f), (p, KerbH), (Out(p), KerbH), (Out(p), -0.15f) };
            int b = v.Count;
            foreach (var p in new[] { innerA, innerB }) foreach (var (q, h) in ring(p)) v.Add(q + Vector3.up * h);
            for (int k = 0; k < 3; k++) { int a = b + k, c = a + 1, e = b + 4 + k, f = e + 1; t.AddRange(new[] { a, c, e, c, f, e, a, e, c, c, e, f }); }
        }

        static void RetainingWall(SplineContainer c, float d0, Material stone)
        {
            var v = new List<Vector3>(); var t = new List<int>();
            const float step = 0.5f, thick = 0.4f, parapet = 0.65f, drop = 2.2f;
            float d1 = c.Spline.GetLength();
            int rows = Mathf.CeilToInt((d1 - d0) / step);
            for (int r = 0; r <= rows; r++)
            {
                Frame(c, Mathf.Min(d0 + r * step, d1), out var p, out _, out var right);
                var inner = p + right * (W12W / 2 + KerbW);
                var outer = inner + right * thick;
                v.Add(inner + Vector3.up * parapet); v.Add(outer + Vector3.up * parapet); v.Add(outer + Vector3.down * drop);
                if (r > 0) { int a = (r - 1) * 3; for (int k = 0; k < 2; k++) t.AddRange(new[] { a + k, a + 3 + k, a + k + 1, a + k + 1, a + 3 + k, a + 4 + k }); }
            }
            var go = MeshGo("W12_retaining_wall_edge", v, WorldUV(v), t, stone, true);
        }

        static void Fence(SplineContainer w12, float d0)
        {
            // Derived offset line on the fence side; SplineInstantiate places the Source fence module along it.
            var pts = new List<Vector3>();
            float len = w12.Spline.GetLength();
            for (float d = d0 + 0.5f; d <= len; d += 2f)
            {
                Frame(w12, d, out var p, out _, out var right);
                pts.Add(p - right * (W12W / 2 + KerbW + 0.45f));
            }
            var line = MakeSpline("W12_fence_line_spline", pts);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/H2F01Inputs/QuaterniusURP/Models/Prop_WoodenFence_Extension1.fbx");
            if (prefab == null) { R.notes.Add("fence prefab missing"); return; }
            var bounds = prefab.GetComponentsInChildren<Renderer>().Select(r => r.bounds).Aggregate((a, b) => { a.Encapsulate(b); return a; });
            float spacing = Mathf.Max(bounds.size.x, bounds.size.z);
            var inst = line.gameObject.AddComponent<SplineInstantiate>();
            inst.Container = line;
            inst.itemsToInstantiate = new[] { new SplineInstantiate.InstantiableItem { Prefab = prefab, Probability = 100 } };
            inst.InstantiateMethod = SplineInstantiate.Method.SpacingDistance;
            inst.MinSpacing = spacing; inst.MaxSpacing = spacing;
            inst.ForwardAxis = bounds.size.x >= bounds.size.z ? SplineComponent.AlignAxis.XAxis : SplineComponent.AlignAxis.ZAxis;
            inst.UpdateInstances();
            var instancesRoot = (GameObject)typeof(SplineInstantiate).GetProperty("InstancesRoot", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(inst);
            R.fenceInstances = instancesRoot != null ? instancesRoot.transform.childCount : -1;
            R.fenceInstancesSerialized = instancesRoot != null && (instancesRoot.hideFlags & HideFlags.DontSave) == 0;
            foreach (var r in line.GetComponentsInChildren<Renderer>(true)) r.sharedMaterial = ArtMat("WoodDark");
            R.notes.Add($"fence module {prefab.name} length {spacing:F2} m (from imported bounds)");
        }

        static void Terrain(SplineContainer w12, SplineContainer x1)
        {
            // Left bank: flush datum within 1.5 m of the kerb outer edge, blending to valley relief.
            // Right bank: below the retaining wall (2.2 m drop), then relief.
            float baseY = w12.transform.TransformPoint((Vector3)w12.Spline.EvaluatePosition(0f)).y - 8f;
            var plan = new List<Vector3>();
            for (float d = 0; d <= w12.Spline.GetLength(); d += 0.25f) { Frame(w12, d, out var pp, out _, out _); plan.Add(pp); }
            foreach (int side in new[] { -1, 1 })
            {
                var data = new TerrainData { heightmapResolution = 129 };
                var size = new Vector3(26, 14, 42);
                data.size = size;
                var origin = new Vector3(side < 0 ? OffX - 2.0f - size.x : OffX + 2.0f, baseY, JunctionZ + X1W / 2 + KerbW + 0.3f);
                int res = data.heightmapResolution;
                var h = new float[res, res];
                for (int zi = 0; zi < res; zi++)
                    for (int xi = 0; xi < res; xi++)
                    {
                        var world = origin + new Vector3(xi / (float)(res - 1) * size.x, 0, zi / (float)(res - 1) * size.z);
                        NearestPlan(plan, world.x, world.z, out float planDist, out float edgeY);
                        float lateral = planDist - (W12W / 2 + KerbW);
                        float y;
                        if (side < 0)
                            y = lateral < 1.5f ? edgeY - 0.05f : Mathf.Lerp(edgeY - 0.05f, edgeY - 1.8f + Mathf.PerlinNoise(world.x * 0.15f, world.z * 0.15f) * 1.2f, Mathf.SmoothStep(0, 1, (lateral - 1.5f) / 8f));
                        else
                            y = lateral < 0.9f ? edgeY - 2.2f + 0.05f : Mathf.Lerp(edgeY - 2.15f, edgeY - 3.5f + Mathf.PerlinNoise(world.x * 0.12f, world.z * 0.12f) * 1.5f, Mathf.SmoothStep(0, 1, (lateral - 0.9f) / 10f));
                        h[zi, xi] = Mathf.Clamp01((y - baseY) / size.y);
                    }
                data.SetHeights(0, 0, h);
                var layer = new TerrainLayer { diffuseTexture = GrassTexture(), tileSize = new Vector2(4, 4), smoothness = 0.05f, metallic = 0f };
                data.terrainLayers = new[] { layer };
                var go = UnityEngine.Terrain.CreateTerrainGameObject(data);
                go.name = side < 0 ? "terrain_bank_left_flush" : "terrain_bank_right_below_wall";
                go.transform.SetParent(root, false);
                go.transform.position = origin;
                var terrain = go.GetComponent<UnityEngine.Terrain>();
                terrain.materialTemplate = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/TerrainLit.mat");
                var mod = go.AddComponent<NavMeshModifier>();
                mod.overrideArea = true; mod.area = 1; // Not Walkable
                R.notes.Add(go.name + ": NavMeshModifier area=Not Walkable (scenic, not street network)");
            }
        }

        static void NearestPlan(List<Vector3> plan, float x, float z, out float dist, out float y)
        {
            dist = float.MaxValue; y = 0;
            foreach (var p in plan)
            {
                float dx = p.x - x, dz = p.z - z, d2 = dx * dx + dz * dz;
                if (d2 < dist) { dist = d2; y = p.y; }
            }
            dist = Mathf.Sqrt(dist);
        }

        static Texture2D GrassTexture()
        {
            var tex = new Texture2D(64, 64);
            for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
                {
                    float n = Mathf.PerlinNoise(x * 0.2f, y * 0.2f) * 0.12f;
                    tex.SetPixel(x, y, new Color(0.33f + n, 0.40f + n, 0.35f + n));
                }
            tex.Apply();
            return tex;
        }

        static void ProBuilderStep(SplineContainer w12)
        {
            // A bounded transition piece (two-riser step) as ProBuilder would author it, then stripped to plain mesh.
            var pb = ShapeGenerator.GenerateStair(PivotLocation.FirstVertex, new Vector3(1.2f, 0.25f, 0.6f), 2, true);
            pb.name = "probuilder_threshold_step";
            pb.transform.SetParent(root, false);
            Frame(w12, w12.Spline.GetLength(), out var p, out var fwd, out _);
            pb.transform.position = p + fwd * 0.6f;
            pb.transform.rotation = Quaternion.LookRotation(fwd);
            R.probuilderBefore = string.Join(",", pb.GetComponents<Component>().Select(c => c.GetType().Name));
            // The editor "Strip ProBuilder Scripts" action is internal in 6.1.2; invoke it by reflection and record that.
            var strip = typeof(UnityEditor.ProBuilder.EditorUtility).Assembly.GetType("UnityEditor.ProBuilder.Actions.StripProBuilderScripts")
                ?.GetMethod("DoStrip", BindingFlags.Public | BindingFlags.Static);
            if (strip != null) strip.Invoke(null, new object[] { pb, false });
            R.notes.Add("ProBuilder strip API: " + (strip != null ? "internal StripProBuilderScripts.DoStrip via reflection (no public API)" : "not found"));
            var go = root.Find("probuilder_threshold_step").gameObject;
            R.probuilderAfter = string.Join(",", go.GetComponents<Component>().Select(c => c.GetType().Name));
            R.probuilderVertices = go.GetComponent<MeshFilter>()?.sharedMesh?.vertexCount ?? 0;
            go.GetComponent<MeshRenderer>().sharedMaterial = ArtMat("StoneTrim");
        }

        // ------------------------------------------------------------------ measurement

        static void Measure(SplineContainer w12, SplineContainer x1)
        {
            float start = X1W / 2 + KerbW;
            for (float d = start + KerbW + CornerR + 0.5f; d < w12.Spline.GetLength() - 0.5f; d += 1f)
            {
                var w = TraversableWidth(w12, d, "W12_road_traversable");
                if (w < W12W - 0.05f && R.notes.Count < 14)
                {
                    Frame(w12, d, out var pc, out _, out _);
                    var hits = Physics.RaycastAll(pc + Vector3.up * 3, Vector3.down, 6).OrderBy(h => h.distance).Select(h => $"{h.collider.name}@{h.point.y:F2}");
                    R.notes.Add($"w12 width {w:F3} at d={d:F2} centreY={pc.y:F2} hits={string.Join("|", hits)}");
                }
                R.w12WidthMin = Mathf.Min(R.w12WidthMin, w); R.w12WidthMax = Mathf.Max(R.w12WidthMax, w);
                // left seam: terrain height just outside the kerb vs road edge
                Frame(w12, d, out var p, out _, out var right);
                var probe = p - right * (W12W / 2 + KerbW + 0.3f);
                var terrain = root.GetComponentsInChildren<UnityEngine.Terrain>().FirstOrDefault(t => t.name.Contains("left"));
                if (terrain != null)
                {
                    float ty = terrain.SampleHeight(probe) + terrain.transform.position.y;
                    R.leftSeamMaxGap = Mathf.Max(R.leftSeamMaxGap, Mathf.Abs(ty - (p.y - 0.05f)));
                }
            }
            for (float d = 1f; d < x1.Spline.GetLength() - 1f; d += 1f)
            {
                if (Mathf.Abs(d - 14) < W12W / 2 + KerbW + CornerR + 0.3f) continue; // junction mouth is measured by the grid below
                var w = TraversableWidth(x1, d, "X1_road_traversable");
                R.x1WidthMin = Mathf.Min(R.x1WidthMin, w); R.x1WidthMax = Mathf.Max(R.x1WidthMax, w);
            }
            // junction continuity: dense grid over X1 north half + apron + first 2 m of W12
            float y0 = RoadY(OffX, JunctionZ);
            float? prev = null;
            for (float z = JunctionZ; z <= JunctionZ + X1W / 2 + KerbW + CornerR + 2f; z += 0.1f)
            {
                prev = null;
                float cx = OffX;
                if (z > JunctionZ + X1W / 2)
                {
                    SplineUtility.GetNearestPoint(w12.Spline, new float3(OffX, y0, z), out float3 nearC, out _);
                    cx = Mathf.Lerp(OffX, nearC.x, Mathf.Clamp01((z - (JunctionZ + X1W / 2)) / (KerbW + CornerR)));
                }
                for (float x = cx - W12W / 2 + 0.05f; x <= cx + W12W / 2 - 0.05f; x += 0.1f)
                {
                    R.junctionSamples++;
                    if (!Physics.Raycast(new Vector3(x, y0 + 5, z), Vector3.down, out var hit, 10) || !hit.collider.name.Contains("traversable"))
                    { R.junctionHoles++; prev = null; if (R.junctionHoles <= 5) R.notes.Add($"junction hole x={x:F2} z={z:F2} hit={(hit.collider ? hit.collider.name : "none")}"); continue; }
                    if (prev.HasValue && Mathf.Abs(hit.point.y - prev.Value) > 0.05f) R.junctionSteps++;
                    prev = hit.point.y;
                }
            }
        }

        /// <summary>Measured, not assumed: lateral extent of the traversable collider at distance d.</summary>
        static float TraversableWidth(SplineContainer c, float d, string collider)
        {
            Frame(c, d, out var p, out _, out var right);
            float left = 0, rightW = 0;
            for (float s = 0; s < 6; s += 0.005f)
            {
                if (Physics.Raycast(p + right * s + Vector3.up * 3, Vector3.down, out var h, 6) && h.collider.name == collider) rightW = s; else break;
            }
            for (float s = 0; s < 6; s += 0.005f)
            {
                if (Physics.Raycast(p - right * s + Vector3.up * 3, Vector3.down, out var h, 6) && h.collider.name == collider) left = s; else break;
            }
            return left + rightW;
        }

        static string PathBetween(Vector3 a, Vector3 b, out float length)
        {
            length = 0;
            var filter = new NavMeshQueryFilter { agentTypeID = 0, areaMask = NavMesh.AllAreas };
            bool okA = NavMesh.SamplePosition(a, out var ha, 1.5f, filter), okB = NavMesh.SamplePosition(b, out var hb, 1.5f, filter);
            if (!okA || !okB) { R.notes.Add($"endpoint off navmesh a={a} ok={okA} b={b} ok={okB}"); return "ENDPOINT_OFF_NAVMESH"; }
            var path = new NavMeshPath();
            NavMesh.CalculatePath(ha.position, hb.position, filter, path);
            for (int i = 1; i < path.corners.Length; i++) length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            return path.status.ToString();
        }

        static void OverlayNav(NavMeshTriangulation tri)
        {
            var mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            mesh.SetVertices(tri.vertices.Select(v => v + Vector3.up * 0.04f).ToList());
            mesh.SetTriangles(tri.indices, 0);
            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.SetColor("_BaseColor", new Color(0.1f, 0.45f, 1f, 1f));
            var go = new GameObject("navmesh_overlay_diagnostic");
            go.transform.SetParent(root, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = mat;
            go.SetActive(false);
        }

        static void Capture()
        {
            Shot("s02/spline_overview.png", new Vector3(OffX + 16, 16, JunctionZ - 10), new Vector3(OffX, 0, JunctionZ + 12));
            Shot("s02/spline_junction_street.png", new Vector3(OffX - 6, RoadY(OffX - 6, JunctionZ) + 1.65f, JunctionZ - 1.5f), new Vector3(OffX, RoadY(OffX, JunctionZ + 6) + 1.2f, JunctionZ + 6));
            Shot("s02/spline_w12_street.png", new Vector3(OffX, RoadY(OffX, JunctionZ + 5) + 1.65f, JunctionZ + 5), new Vector3(OffX + 1.5f, RoadY(OffX, 4) + 2f, 4));
            Shot("s02/spline_wall_fence.png", new Vector3(OffX + 7.5f, RoadY(OffX, -6) + 2.2f, -10), new Vector3(OffX + 1.2f, RoadY(OffX, -4) - 0.6f, -3));
            var overlay = root.Find("navmesh_overlay_diagnostic").gameObject;
            overlay.SetActive(true);
            Shot("s02/navmesh_spline_topdown.png", new Vector3(OffX, 60, -4), new Vector3(OffX, 0, -3.99f), 50);
            overlay.SetActive(false);
        }

        static void Shot(string path, Vector3 pos, Vector3 target, float fov = 60)
        {
            var cam = CameraAt("s02_cam", pos, target, fov);
            cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            Image(cam, path);
            UnityEngine.Object.DestroyImmediate(cam.gameObject);
        }
    }
}
