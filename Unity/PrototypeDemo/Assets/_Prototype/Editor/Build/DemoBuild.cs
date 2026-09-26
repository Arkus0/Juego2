using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Proto.EditorTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Orchestrates the layered construction (0 terrain … 6 dressing), scene save and fixed-camera captures.
    public static class DemoBuild
    {
        public const string ScenePath = "Assets/_Prototype/Scenes/PuenteBar.unity";
        public static Layout Lay;
        public static TerrainBuild Tb;
        public static Heights Hts;

        public struct Cam { public string Name; public Vector2 From; public float Eye; public Vector3 To; }

        /// The six fixed review cameras from the brief (+ an aerial). Eye height is 1.65 m over the ground.
        public static readonly Cam[] Cams =
        {
            new Cam { Name = "1_orilla_sur_al_casco", From = P(38.5f, -69f), Eye = 1.7f, To = new Vector3(76, 15, 18) },
            new Cam { Name = "2_lomo_del_puente", From = P(46.3f, -49.5f), Eye = 1.7f, To = new Vector3(66, 11, 6) },
            new Cam { Name = "3_subida_w12", From = P(58.5f, -17f), Eye = 1.7f, To = new Vector3(71, 10.5f, 14) },
            new Cam { Name = "4_plazuela_casco", From = P(74.5f, 31.5f), Eye = 1.7f, To = new Vector3(92, 10, 45) },
            new Cam { Name = "5_fachada_bar", From = P(89.6f, 41.9f), Eye = 1.7f, To = new Vector3(92.5f, 10.6f, 46) },
            new Cam { Name = "6_plaza_torre", From = P(141f, 49f), Eye = 1.7f, To = new Vector3(151, 26, 69) },
        };

        static string Arg(string name, string def)
        {
            var args = Environment.GetCommandLineArgs();
            int i = Array.IndexOf(args, name);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : def;
        }

        [MenuItem("Prototype/Rebuild Scene (all layers)")]
        public static void RebuildAll() => Rebuild(6);

        /// -executeMethod Proto.Build.DemoBuild.BatchLayer -protoLayer N
        public static void BatchLayer()
        {
            int layer = int.Parse(Arg("-protoLayer", "6"));
            Rebuild(layer);
            CaptureAll(Path.GetFullPath($"Captures/Layers/L{layer}"));
        }

        public static void Rebuild(int maxLayer)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var t0 = DateTime.Now;
            MeshKit.Meshes.Clear();
            MatLib.EnsureDirs();
            MatLib.RemapImportedMaterials();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var world = new GameObject("World").transform;
            var L = new Transform[7];
            string[] names = { "L0_Terreno", "L1_Rasantes", "L2_Superficies", "L3_Plataformas_Cimientos", "L4_Casas", "L5_Encuentros", "L6_Vestido" };
            for (int i = 0; i < 7; i++) { L[i] = new GameObject(names[i]).transform; L[i].SetParent(world, false); }
            Atmosphere.Build(world);

            Lay = new Layout();
            Lay.Plan();
            Hts = new Heights();
            Hts.Solve(Lay, false);
            Hts.AssignGardenHeights(Lay);
            Hts.Solve(Lay, true);
            Tb = new TerrainBuild(Hts, Lay);
            Tb.ComputeHeights();
            Tb.BuildTerrain(L[0]);
            Log(t0, "terrain");

            Water(L[0]);
            var walls = new WallBuild(Tb, Lay, L[0]);
            walls.BankWalls();
            walls.HardBoundary();
            walls.WaterFence();

            if (maxLayer >= 2) { new WallBuild(Tb, Lay, L[2]).RouteEdges(); }
            if (maxLayer >= 3)
            {
                var w3 = new WallBuild(Tb, Lay, L[3]);
                w3.PlatformEdges(); w3.GardenWalls(); w3.FrontWalls();
            }
            Log(t0, "walls");
            BuildLate(maxLayer, L);
            Log(t0, "late layers");
            CheckPresence(world);

            Atmosphere.Probe(world, new Vector3(90, 14, 10), new Vector3(420, 120, 420), "ReflectionProbe_Town");
            SaveScene(scene);
            Log(t0, "saved");
        }

        /// What is missing never shows in a capture: flag placed renderers that ended up microscopic
        /// (e.g. a prop whose 100x root scale was overwritten) instead of trusting the review to notice.
        static void CheckPresence(Transform world)
        {
            var tiny = world.GetComponentsInChildren<Renderer>(true)
                .Where(r => r.enabled && !(r is ParticleSystemRenderer) && r.bounds.size.magnitude < 0.03f)
                .Select(r => r.gameObject.name).ToList();
            Debug.Log(tiny.Count == 0 ? "[Proto] presence check: OK (no microscopic renderers)"
                : $"[Proto] presence check: {tiny.Count} microscopic renderers, e.g. " + string.Join(", ", tiny.Distinct().Take(8)));
        }

        /// Hook filled by the later-layer builders (houses, encounters, dressing, gameplay).
        static void BuildLate(int maxLayer, Transform[] L)
        {
            var late = typeof(DemoBuild).Assembly.GetType("Proto.Build.LateLayers");
            late?.GetMethod("Build")?.Invoke(null, new object[] { maxLayer, L });
        }

        static void Log(DateTime t0, string what) => Debug.Log($"[Proto] {what} done at {(DateTime.Now - t0).TotalSeconds:F1}s");

        static void Water(Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = "Water_Rio_y_Arroyo";
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(Heights.U0 + Heights.Size * 0.5f, Seed.HWater, Heights.V0 + Heights.Size * 0.5f);
            go.transform.localScale = new Vector3(Heights.Size / 10f * 1.4f, 1, Heights.Size / 10f * 1.4f);
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            var mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = MatLib.Get("Water");
            mr.shadowCastingMode = ShadowCastingMode.Off;
            go.AddComponent<Proto.Runtime.WaterScroll>();
        }

        static void SaveScene(UnityEngine.SceneManagement.Scene scene)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            // persist generated meshes so the saved scene and the player build keep them
            string meshPath = MatLib.GenDir + "/Meshes.asset";
            var holder = ScriptableObject.CreateInstance<MeshHolder>();
            AssetDatabase.CreateAsset(holder, meshPath);
            foreach (var m in MeshKit.Meshes.Distinct()) if (m != null && !AssetDatabase.Contains(m)) AssetDatabase.AddObjectToAsset(m, holder);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            // reflections
            foreach (var rp in UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None))
                Lightmapping.BakeReflectionProbe(rp, $"{MatLib.GenDir}/{rp.name}.exr");
            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        /// -executeMethod Proto.Build.DemoBuild.BatchBuildPlayer : full rebuild + Windows player in <repo>/Builds/PrototypeDemo
        public static void BatchBuildPlayer()
        {
            if (Arg("-skipRebuild", "0") != "1") { Rebuild(6); CaptureAll(Path.GetFullPath("Captures/Layers/L6")); }
            var dir = Path.GetFullPath("../../Builds/PrototypeDemo");
            Directory.CreateDirectory(dir);
            var opts = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = Path.Combine(dir, "PuenteBar.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            };
            var report = BuildPipeline.BuildPlayer(opts);
            Debug.Log($"[Proto] player build: {report.summary.result} {report.summary.totalSize / (1024 * 1024)} MB -> {opts.locationPathName} ({report.summary.totalErrors} errors)");
        }

        public static void CaptureAll(string dir)
        {
            Directory.CreateDirectory(dir);
            float fog = RenderSettings.fogDensity;
            foreach (var c in Cams)
            {
                float g = GameplayBuild.WalkHeight(Tb, c.From);
                var cam = Capture.MakeCamera(W(c.From, g + c.Eye), c.To, 62f);
                Capture.Shot(cam, Path.Combine(dir, c.Name + ".png"));
                UnityEngine.Object.DestroyImmediate(cam.gameObject);
            }
            RenderSettings.fogDensity = fog * 0.45f;
            var air = Capture.MakeCamera(new Vector3(-30, 95, -135), new Vector3(105, 5, 40), 55f);
            Capture.Shot(air, Path.Combine(dir, "0_aerea.png"));
            UnityEngine.Object.DestroyImmediate(air.gameObject);
            var top = Capture.MakeCamera(new Vector3(110, 330, 30), new Vector3(110, 0, 30.01f), 45f);
            top.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            Capture.Shot(top, Path.Combine(dir, "0_cenital.png"), 1400, 1400);
            UnityEngine.Object.DestroyImmediate(top.gameObject);
            RenderSettings.fogDensity = fog;
            Debug.Log("[Proto] captures -> " + dir);
        }
    }

    public class MeshHolder : ScriptableObject { }
}
