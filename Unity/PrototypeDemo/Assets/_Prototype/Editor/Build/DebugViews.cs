using System.IO;
using Proto.EditorTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Proto.Build
{
    public static class DebugViews
    {
        public static void PropsLineup()
        {
            string dir = Path.GetFullPath("Captures/Debug/Props");
            Directory.CreateDirectory(dir);
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane); ground.transform.localScale = Vector3.one * 3;
            string[] names = { "Chair_1", "Stool", "Table_Large", "Barrel", "Mug", "Bench", "CandleStick", "Lantern_Wall", "Pot_1", "Crate_Wooden", "FarmCrate_Apple", "Bucket_Wooden_1", "Bottle_1", "Candle_1", "Barrel_Holder" };
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < names.Length; i++)
            {
                var go = Kit.Put(names[i], null, new Vector3((i % 5) * 1.6f - 3.2f, 0, (i / 5) * 1.8f), Quaternion.identity);
                var b = new Bounds(go.transform.position, Vector3.zero);
                foreach (var r in go.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);
                sb.AppendLine($"{names[i]} rootRot={go.transform.rotation.eulerAngles} child0Rot={(go.transform.childCount > 0 ? go.transform.GetChild(0).localRotation.eulerAngles.ToString() : "-")} size={b.size} min={b.min - go.transform.position}");
            }
            File.WriteAllText(Path.Combine(dir, "props.txt"), sb.ToString());
            var cam = Capture.MakeCamera(new Vector3(0, 3.2f, -5.5f), new Vector3(0, 0.3f, 1.6f), 55f);
            Capture.Shot(cam, Path.Combine(dir, "props.png"), 1200, 700);
            // ford close-up in the demo scene
            EditorSceneManager.OpenScene(DemoBuild.ScenePath);
            var c2 = Capture.MakeCamera(new Vector3(96, 7, 104), new Vector3(90.5f, 0.3f, 116.5f), 55f);
            Capture.Shot(c2, Path.Combine(dir, "ford.png"), 1200, 700);
            c2.transform.position = new Vector3(91, 14, 116); c2.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            Capture.Shot(c2, Path.Combine(dir, "ford_top.png"), 900, 900);
        }

        public static void ModelInfo()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var n in new[] { "Nature/Models/CommonTree_1", "Nature/Models/Bush_Common_Flowers", "Nature/Models/Grass_Common_Short", "Props/Models/Barrel", "Props/Models/Pot_1", "Props/Models/Bench" })
            {
                var path = "Assets/ThirdParty/Quaternius/" + n + ".fbx";
                var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                var b = new Bounds(go.transform.position, Vector3.zero);
                foreach (var r in go.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);
                sb.AppendLine($"{n} size={b.size} mats={string.Join(",", System.Linq.Enumerable.Select(go.GetComponentsInChildren<Renderer>()[0].sharedMaterials, m => m ? m.name + "/" + m.shader.name + "@" + AssetDatabase.GetAssetPath(m) : "null"))}");
                var imp = (ModelImporter)AssetImporter.GetAtPath(path);
                sb.AppendLine("   remaps=" + string.Join(",", System.Linq.Enumerable.Select(imp.GetExternalObjectMap(), kv => kv.Key.name + "->" + (kv.Value ? kv.Value.name : "null"))) + " scale=" + imp.globalScale + " useFileScale=" + imp.useFileScale);
                Object.DestroyImmediate(go);
            }
            File.WriteAllText(Path.GetFullPath("Captures/Debug/models.txt"), sb.ToString());
        }

        public static void EmptyTest()
        {
            string dir = Path.GetFullPath("Captures/Debug/Empty");
            Directory.CreateDirectory(dir);
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            GameObject.CreatePrimitive(PrimitiveType.Cube);
            var cam = Capture.MakeCamera(new Vector3(3, 2, -4), Vector3.zero, 55f);
            Capture.Shot(cam, Path.Combine(dir, "a_default.png"), 320, 180);
            cam.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            var data = cam.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            data.renderPostProcessing = false;
            var rt = new RenderTexture(320, 180, 24);
            cam.targetTexture = rt; cam.Render(); cam.targetTexture = null;
            RenderTexture.active = rt; var t = new Texture2D(320, 180, TextureFormat.RGB24, false); t.ReadPixels(new Rect(0, 0, 320, 180), 0, 0); t.Apply();
            File.WriteAllBytes(Path.Combine(dir, "b_camrender_nopost.png"), t.EncodeToPNG());
            Debug.Log("[Proto] pipeline=" + UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline + " quality=" + QualitySettings.GetQualityLevel() + " rp=" + QualitySettings.renderPipeline);
        }

        public static void Bisect2()
        {
            EditorSceneManager.OpenScene(DemoBuild.ScenePath);
            string dir = Path.GetFullPath("Captures/Debug/Bisect2");
            Directory.CreateDirectory(dir);
            var from = new Vector3(-30, 95, -135); var to = new Vector3(105, 5, 40);
            void Shot(string name) { var cam = Capture.MakeCamera(from, to, 55f); Capture.Shot(cam, Path.Combine(dir, name + ".png"), 480, 270); Object.DestroyImmediate(cam.gameObject); }
            var world = GameObject.Find("World").transform;
            var kids = new System.Collections.Generic.List<Transform>();
            foreach (Transform t in world) kids.Add(t);
            foreach (var k in kids) k.gameObject.SetActive(false);
            Shot("none");
            foreach (var k in kids) { k.gameObject.SetActive(true); Shot("only_" + k.name.Replace(" ", "_")); k.gameObject.SetActive(false); }
            // inside L5 / L6: one child at a time
            foreach (var layer in new[] { "L5_Encuentros", "L6_Vestido" })
            {
                var L = world.Find(layer); L.gameObject.SetActive(true);
                var sub = new System.Collections.Generic.List<Transform>(); foreach (Transform t in L) sub.Add(t);
                foreach (var c in sub) c.gameObject.SetActive(false);
                foreach (var c in sub) { c.gameObject.SetActive(true); Shot(layer + "_" + c.name.Replace(" ", "_").Replace("/", "_").Replace(":", "_")); c.gameObject.SetActive(false); }
                L.gameObject.SetActive(false);
            }
        }

        public static void Bisect()
        {
            EditorSceneManager.OpenScene(DemoBuild.ScenePath);
            string dir = Path.GetFullPath("Captures/Debug/Bisect");
            Directory.CreateDirectory(dir);
            var from = new Vector3(-30, 95, -135); var to = new Vector3(105, 5, 40);
            void Shot(string name)
            {
                var cam = Capture.MakeCamera(from, to, 55f);
                Capture.Shot(cam, Path.Combine(dir, name + ".png"), 480, 270);
                Object.DestroyImmediate(cam.gameObject);
            }
            Shot("0_all");
            var terrain = Object.FindAnyObjectByType<Terrain>();
            terrain.drawTreesAndFoliage = false; Shot("1_notrees"); terrain.drawTreesAndFoliage = true;
            var sky = RenderSettings.skybox; RenderSettings.skybox = null; Shot("x_nosky"); RenderSettings.skybox = sky;
            RenderSettings.fog = false; Shot("x_nofog"); RenderSettings.fog = true;
            terrain.gameObject.SetActive(false); Shot("x_noterrain"); terrain.gameObject.SetActive(true);
            foreach (var n in new[] { "Sun (overcast key)", "Water_Rio_y_Arroyo", "L0_Terreno", "L1_Rasantes", "L2_Superficies", "L3_Plataformas_Cimientos", "World", "Main Camera", "Telon_Montanas", "Tejados_Lejanos", "Props", "Vecinos", "Vestido_Procedural", "L6_Vestido", "L5_Encuentros", "L4_Casas", "PostProcess", "ReflectionProbe_Town" })
            {
                var go = GameObject.Find(n);
                if (go == null) { Debug.Log("[Proto] bisect: missing " + n); continue; }
                go.SetActive(false); Shot("off_" + n.Replace(" ", "_")); go.SetActive(true);
            }
        }

        public static void PlanOnly()
        {
            var lay = new Layout();
            lay.Plan();
            var sb = new System.Text.StringBuilder();
            foreach (var h in lay.Houses) sb.AppendLine(string.Join("	", h.Id, h.Kind, string.Join(" ", System.Array.ConvertAll(h.Foot, q => $"{q.x:F2},{q.y:F2}")), h.Floors, h.FL.ToString("F2")));
            foreach (var g in lay.Gardens) sb.AppendLine(string.Join("	", "G", "Garden", string.Join(" ", System.Array.ConvertAll(g.Poly, q => $"{q.x:F2},{q.y:F2}")), 0, g.H.ToString("F2")));
            File.WriteAllText(Path.GetFullPath("Captures/Debug/houses.tsv"), sb.ToString());
        }

        /// Opens the saved scene and renders fog-free top views + splat channel dumps.
        public static void Run()
        {
            EditorSceneManager.OpenScene(DemoBuild.ScenePath);
            RenderSettings.fog = false;
            string dir = Path.GetFullPath("Captures/Debug");
            Directory.CreateDirectory(dir);
            var cam = Capture.MakeCamera(new Vector3(80, 120, 20), new Vector3(80, 0, 20.01f), 50f);
            cam.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            Capture.Shot(cam, Path.Combine(dir, "top_casco.png"), 1400, 1400);
            cam.transform.position = new Vector3(110, 330, 30);
            Capture.Shot(cam, Path.Combine(dir, "top_all.png"), 1400, 1400);
            cam.transform.position = new Vector3(58, 6, -20);
            cam.transform.rotation = Quaternion.LookRotation(new Vector3(0.3f, -0.8f, 1f), Vector3.up);
            Capture.Shot(cam, Path.Combine(dir, "w12_down.png"));
            Object.DestroyImmediate(cam.gameObject);
            var t = Object.FindAnyObjectByType<Terrain>();
            var td = t.terrainData;
            var a = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
            var tex = new Texture2D(td.alphamapWidth, td.alphamapHeight, TextureFormat.RGB24, false);
            for (int y = 0; y < td.alphamapHeight; y++)
                for (int x = 0; x < td.alphamapWidth; x++)
                    tex.SetPixel(x, y, new Color(a[y, x, 2], a[y, x, 3], a[y, x, 1]));
            tex.Apply();
            File.WriteAllBytes(Path.Combine(dir, "splat_cobble_flag_dirt.png"), tex.EncodeToPNG());
            Debug.Log("[Proto] layers: " + string.Join(",", System.Array.ConvertAll(td.terrainLayers, l => l.name + ":" + (l.diffuseTexture ? l.diffuseTexture.name : "null"))));
        }
    }
}
