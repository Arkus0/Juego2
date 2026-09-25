using System;
using System.IO;
using Arkus.CITY;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Arkus.CITY.Editor
{
    // Rebuildable Unity projection of the frozen CITY-03 planning geometry.
    // Manual measurements and spatial verdicts remain CITY-04 evidence, not editor state.
    public static class City04GreyboxBuilder
    {
        private const string Base = "Assets/Arkus/CITY";
        private const string ScenePath = Base + "/City04Greybox.unity";
        private const string MaterialsPath = Base + "/Materials";
        private static Material land, water, arroyo, road, historic, bridge, ford;
        private static Material closed, bar, civic, shop, site, threshold, scenic, marker;

        [MenuItem("Arkus/CITY-04/Rebuild greybox from accepted projection")]
        public static void Build()
        {
            if (Application.unityVersion != "6000.3.24f1")
                throw new InvalidOperationException("CITY-04 requires pinned Unity 6000.3.24f1");
            if (City04Layout.SourceBlob != "3186b80d173cd20f961f33d5a28bf447b2c6971d")
                throw new InvalidOperationException("CITY-03 source identity changed");

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            if (!AssetDatabase.IsValidFolder(MaterialsPath))
                AssetDatabase.CreateFolder(Base, "Materials");
            land = Mat("Land", new Color(0.46f, 0.52f, 0.42f));
            water = Mat("Rio water", new Color(0.18f, 0.42f, 0.56f));
            arroyo = Mat("Arroyo water", new Color(0.24f, 0.48f, 0.60f));
            road = Mat("Ordinary road", new Color(0.65f, 0.59f, 0.49f));
            historic = Mat("Historic lane", new Color(0.65f, 0.56f, 0.42f));
            bridge = Mat("X1 old bridge", new Color(0.57f, 0.55f, 0.50f));
            ford = Mat("X5 conditional foot crossing", new Color(0.79f, 0.69f, 0.48f));
            closed = Mat("Closed ordinary fabric", new Color(0.56f, 0.55f, 0.51f));
            bar = Mat("Bar public", new Color(0.67f, 0.38f, 0.27f));
            civic = Mat("Civic public", new Color(0.53f, 0.45f, 0.70f));
            shop = Mat("Everyday shop", new Color(0.61f, 0.62f, 0.38f));
            site = Mat("Open site", new Color(0.67f, 0.67f, 0.57f));
            threshold = Mat("Restricted threshold", new Color(0.80f, 0.35f, 0.29f));
            scenic = Mat("Scenic only", new Color(0.45f, 0.48f, 0.49f));
            marker = Mat("Measurement marker", new Color(0.88f, 0.77f, 0.37f));

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.34f, 0.35f, 0.36f);
            var sun = new GameObject("Greybox daylight").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 0.7f;
            sun.transform.rotation = Quaternion.Euler(55, -35, 0);

            MeshObject("Exact dry land visual — three components", "land_visual", land, false);
            MeshObject("Authorized bank-aware walkable collision", "walkable_collision", land, true, false);
            MeshObject("mask.rio — no traversal", "rio_water", water, false);
            MeshObject("mask.arroyo — no traversal", "arroyo_water", arroyo, false);
            MeshObject("X1 permanent Río crossing", "x1_crossing", bridge, true);
            GameObject x5 = MeshObject("X5 low-water-only crossing (G)", "x5_crossing", ford, true);
            BuildOldBridgeEdges();

            foreach (var route in City04Layout.ROUTES)
            {
                if (route.Id == "X1" || route.Id == "X5") continue;
                Material surface = route.Id == "W04" ? road : historic;
                for (int i = 1; i < route.Points.Length; i++)
                    Ribbon(route.Id + " trace " + i, route.Points[i - 1], route.Points[i], route.Width, surface);
            }
            foreach (var stub in City04Layout.STUBS)
                Outline(stub.Id + " receiving footprint", stub.Points, marker);
            foreach (var siteRegion in City04Layout.SITES)
            {
                Outline(siteRegion.Id + " exact placement region", siteRegion.Points, marker);
                if (siteRegion.Id.StartsWith("F")) BuildFrontage(siteRegion);
                else if (siteRegion.Id == "S03") BuildCourt(siteRegion);
                Label(siteRegion.Id + " " + siteRegion.Role, Centroid(siteRegion.Points), 2.2f, Color.black);
            }
            foreach (var anchor in City04Layout.ANCHORS)
            {
                Vector2 p = anchor.Points[0];
                Label(anchor.Id, p + new Vector2(2, 2), 2.0f, Color.black);
                Box(anchor.Id + " survey peg", At(p, 0.7f), new Vector3(0.45f, 1.4f, 0.45f), marker, false);
            }

            var busyMarkers = new GameObject("S01 temporary busy apron markers (Q)");
            Box("Market stall proxy", At(new Vector2(137, 42), 0.6f), new Vector3(2.0f, 1.2f, 1.5f), site, true).transform.SetParent(busyMarkers.transform, true);
            Box("Market crate proxy", At(new Vector2(143, 45), 0.4f), new Vector3(1.3f, 0.8f, 1.3f), site, true).transform.SetParent(busyMarkers.transform, true);
            busyMarkers.SetActive(false);

            var barrier = Box("micro.A temporary closure (B)", At(new Vector2(83, 37), 0.8f),
                new Vector3(0.3f, 1.6f, 3.3f), threshold, true);
            barrier.SetActive(false);

            // Non-colliding D/S0 silhouettes hint at later city without adding playable edges.
            Scenic("Calle Mayor continuation", new Vector2(250, 64), new Vector3(12, 9, 22));
            Scenic("Upper Barrio continuation", new Vector2(154, 157), new Vector3(26, 8, 11));
            Scenic("Ensanche across Arroyo", new Vector2(105, 148), new Vector3(20, 7, 12));
            Scenic("Orilla-sur continuation", new Vector2(38, -81), new Vector3(20, 7, 8));
            Scenic("Puerto across water", new Vector2(-28, -42), new Vector3(16, 7, 9));
            // Soft context is deliberately non-colliding. It suggests later
            // streets across the cuts without adding a CITY-01 playable edge.
            SoftGround("commercial continuation", new Vector2(257, 65), new Vector3(34, 0.2f, 57));
            Ribbon("NON-PLAYABLE W04 commercial continuation", new Vector2(195, 70), new Vector2(255, 68), 5, road);
            SoftGround("upper future", new Vector2(152, 167), new Vector3(32, 0.2f, 28));
            Ribbon("NON-PLAYABLE upper visual continuation", new Vector2(150, 145), new Vector2(156, 169), 2.8f, historic);
            SoftGround("Ensanche beyond X5", new Vector2(99, 146), new Vector3(30, 0.2f, 23));
            Ribbon("NON-PLAYABLE Ensanche visual continuation", new Vector2(90, 122), new Vector2(100, 149), 2.5f, historic);
            SoftGround("Orilla-sur beyond X1", new Vector2(42, -91), new Vector3(29, 0.2f, 25));
            Ribbon("NON-PLAYABLE Orilla visual continuation", new Vector2(42, -64), new Vector2(43, -93), 2.8f, historic);
            Box("NON-PLAYABLE water beyond confluence", new Vector3(-39, -1.7f, -20),
                new Vector3(38, 0.05f, 36), water, false);

            GameObject player = new GameObject("CITY-04 human traversal probe (Play mode)");
            Vector2 start = new Vector2(80, 35);
            player.transform.position = At(start, 0.15f);
            var character = player.AddComponent<CharacterController>();
            character.height = 2f;
            character.radius = 0.32f;
            character.center = Vector3.up;
            character.stepOffset = 0.4f;
            var cameraObject = new GameObject("Human eye");
            cameraObject.transform.SetParent(player.transform, false);
            cameraObject.transform.localPosition = new Vector3(0, 1.65f, 0);
            var humanCamera = cameraObject.AddComponent<Camera>();
            humanCamera.tag = "MainCamera";
            humanCamera.farClipPlane = 500;
            player.transform.rotation = Quaternion.Euler(0, 30, 0);
            var probe = player.AddComponent<City04TraversalProbe>();
            probe.View = humanCamera;
            probe.X5Crossing = x5;
            probe.RouteABarrier = barrier;
            probe.BusyMarkers = busyMarkers;
            var proxy = Box("Follow/search spatial proxy (Z/X)", At(new Vector2(73, 30), 0.85f),
                new Vector3(0.6f, 1.7f, 0.6f), marker, false);
            proxy.SetActive(false);
            probe.ProxyActor = proxy;

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new InvalidOperationException("CITY-04 scene save failed");
            AssetDatabase.SaveAssets();
            Debug.Log("CITY04_BUILD_GREEN scene=" + ScenePath + " source_blob=" + City04Layout.SourceBlob);
        }

        [MenuItem("Arkus/CITY-04/Capture bounded greybox views")]
        public static void Capture()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid()) throw new InvalidOperationException("CITY-04 scene missing");
            var evidence = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../Docs/evidence/WP-CITY-04"));
            Directory.CreateDirectory(evidence);
            CaptureView(Path.Combine(evidence, "overhead.png"),
                new Vector3(110, 270, 30), new Vector3(110, 0, 30), true);
            CaptureView(Path.Combine(evidence, "x1_to_casco.png"),
                At(new Vector2(42, -64), 1.7f), At(new Vector2(80, 35), 2.5f), false);
            CaptureView(Path.Combine(evidence, "plaza_to_casco.png"),
                At(new Vector2(150, 58), 1.7f), At(new Vector2(80, 35), 2.5f), false);
            CaptureView(Path.Combine(evidence, "landing_to_port.png"),
                At(new Vector2(0, 0), 1.7f), At(new Vector2(-28, -42), 4), false);
            Debug.Log("CITY04_CAPTURE_GREEN folder=" + evidence);
        }

        [MenuItem("Arkus/CITY-04/Inspect imported geometry")]
        public static void InspectProjection()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            foreach (string wanted in new[] { "Exact dry land visual", "Authorized bank-aware", "mask.rio", "mask.arroyo", "X1 permanent", "X5 low-water" })
            {
                foreach (var go in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
                {
                    if (!go.name.StartsWith(wanted, StringComparison.Ordinal)) continue;
                    foreach (var renderer in go.GetComponentsInChildren<Renderer>())
                        Debug.Log("CITY04_GEOMETRY " + wanted + " world_bounds=" + renderer.bounds +
                                  " material=" + (renderer.sharedMaterial == null ? "NONE" : renderer.sharedMaterial.name) +
                                  " enabled=" + renderer.enabled);
                    foreach (var filter in go.GetComponentsInChildren<MeshFilter>())
                        Debug.Log("CITY04_MESH " + wanted + " local_bounds=" + filter.sharedMesh.bounds +
                                  " transform=" + filter.transform.localToWorldMatrix);
                }
            }
        }

        [MenuItem("Arkus/CITY-04/Check physical collider cuts")]
        public static void CheckPhysicalCuts()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Physics.SyncTransforms();
            RequireSurface("Wedge", new Vector2(120, 20), "Authorized bank-aware");
            RequireSurface("Orilla-sur", new Vector2(55, -68), "Authorized bank-aware");
            RequireSurface("Ensanche", new Vector2(80, 125), "Authorized bank-aware");
            RequireSurface("X1", new Vector2(46, -48), "X1 permanent");
            RequireSurface("X5 available", new Vector2(91, 115), "X5 low-water");
            RequireSurface("Río void", new Vector2(110, -52), null);
            RequireSurface("Arroyo void", new Vector2(55, 64), null);
            GameObject x5 = GameObject.Find("X5 low-water-only crossing (G)");
            if (x5 == null) throw new InvalidOperationException("X5 scene object missing");
            foreach (var collider in x5.GetComponentsInChildren<Collider>()) collider.enabled = false;
            Physics.SyncTransforms();
            RequireSurface("X5 closed", new Vector2(91, 115), null);
            Debug.Log("CITY04_PHYSICAL_CUTS_GREEN: three dry components, X1 and active X5 collision, X5 closure, water voids");
        }

        private static void RequireSurface(string label, Vector2 point, string expectedRoot)
        {
            RaycastHit[] hits = Physics.RaycastAll(new Vector3(point.x, 50, point.y), Vector3.down, 100);
            bool matched = false;
            foreach (var hit in hits)
            {
                string root = hit.collider.transform.root.name;
                if (expectedRoot != null && root.StartsWith(expectedRoot, StringComparison.Ordinal)) matched = true;
                if (expectedRoot == null && !root.StartsWith("CITY-04 human traversal", StringComparison.Ordinal))
                    throw new InvalidOperationException(label + " unexpectedly collides with " + root);
            }
            if (expectedRoot != null && !matched)
                throw new InvalidOperationException(label + " missing expected collision: " + expectedRoot);
            Debug.Log("CITY04_CUT " + label + " expected=" + (expectedRoot ?? "VOID") + " hits=" + hits.Length);
        }

        private static void CaptureView(string path, Vector3 from, Vector3 target, bool overhead)
        {
            var go = new GameObject("Temporary capture camera");
            var camera = go.AddComponent<Camera>();
            camera.backgroundColor = new Color(0.55f, 0.69f, 0.81f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.transform.position = from;
            camera.transform.rotation = overhead
                ? Quaternion.LookRotation(Vector3.down, Vector3.forward)
                : Quaternion.LookRotation(target - from, Vector3.up);
            camera.orthographic = overhead;
            if (overhead) camera.orthographicSize = 140;
            camera.farClipPlane = 600;
            RenderTexture rt = RenderTexture.GetTemporary(1600, 1300, 24);
            camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(1600, 1300, TextureFormat.RGB24, false);
            png.ReadPixels(new Rect(0, 0, 1600, 1300), 0, 0);
            png.Apply();
            File.WriteAllBytes(path, png.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            UnityEngine.Object.DestroyImmediate(png);
            UnityEngine.Object.DestroyImmediate(go);
        }

        private static Material Mat(string name, Color color)
        {
            string path = MaterialsPath + "/" + name.Replace(' ', '_') + ".mat";
            var result = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (result == null)
            {
                result = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(result, path);
            }
            result.color = color;
            EditorUtility.SetDirty(result);
            return result;
        }

        private static GameObject MeshObject(string name, string meshName, Material material, bool collision, bool visible = true)
        {
            string path = Base + "/Meshes/" + meshName + ".obj";
            GameObject imported = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (imported == null) throw new InvalidOperationException("CITY-04 mesh missing: " + path);
            GameObject result = UnityEngine.Object.Instantiate(imported);
            result.name = name;
            foreach (var filter in result.GetComponentsInChildren<MeshFilter>())
            {
                var renderer = filter.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = material;
                    renderer.enabled = visible;
                }
                if (collision)
                {
                    var collider = filter.gameObject.AddComponent<MeshCollider>();
                    collider.sharedMesh = filter.sharedMesh;
                }
            }
            return result;
        }

        private static Vector3 At(Vector2 p, float above = 0)
        {
            return new Vector3(p.x, City04TraversalProbe.GroundHeight(p) + above, p.y);
        }

        private static GameObject Box(string name, Vector3 position, Vector3 size, Material material, bool collision)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = material;
            if (!collision) UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        private static void Ribbon(string name, Vector2 start, Vector2 end, float width, Material material)
        {
            Vector3 a = At(start, 0.08f), b = At(end, 0.08f);
            var roadPiece = Box(name, (a + b) / 2, new Vector3(width, 0.045f, (b - a).magnitude), material, false);
            roadPiece.transform.rotation = Quaternion.FromToRotation(Vector3.forward, b - a);
        }

        private static void BuildOldBridgeEdges()
        {
            Vector2 wedge = new Vector2(50, -32), orilla = new Vector2(42, -64);
            Vector2 cross = new Vector2(32, -8).normalized;
            foreach (float side in new[] { -2.7f, 2.7f })
            {
                Vector3 a = At(wedge + cross * side, 0.75f);
                Vector3 b = At(orilla + cross * side, 0.75f);
                var rail = Box("X1 temporary stone edge", (a + b) / 2,
                    new Vector3(0.22f, 1.25f, (b - a).magnitude), bridge, true);
                rail.transform.rotation = Quaternion.FromToRotation(Vector3.forward, b - a);
            }
            foreach (Vector2 head in new[] { wedge, orilla })
            {
                foreach (float side in new[] { -1.15f, 1.15f })
                    Box("X1 pedestrian bollard", At(head + cross * side, 0.7f),
                        new Vector3(0.45f, 1.4f, 0.45f), bridge, true);
            }
        }

        private static void Outline(string name, Vector2[] points, Material material)
        {
            for (int i = 0; i < points.Length; i++)
                Ribbon(name + " " + i, points[i], points[(i + 1) % points.Length], 0.18f, material);
        }

        private static Vector2 Centroid(Vector2[] points)
        {
            Vector2 sum = Vector2.zero;
            foreach (var p in points) sum += p;
            return sum / points.Length;
        }

        private static void Label(string value, Vector2 position, float size, Color color)
        {
            var go = new GameObject("LABEL " + value);
            go.transform.position = At(position, 0.22f);
            go.transform.rotation = Quaternion.Euler(-90, 0, 0);
            var mesh = go.AddComponent<TextMesh>();
            mesh.text = value;
            mesh.fontSize = 32;
            mesh.characterSize = size / 32;
            mesh.color = color;
            mesh.anchor = TextAnchor.MiddleCenter;
        }

        private static void BuildFrontage(City04Layout.Region region)
        {
            float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
            foreach (var p in region.Points)
            {
                minX = Mathf.Min(minX, p.x); maxX = Mathf.Max(maxX, p.x);
                minZ = Mathf.Min(minZ, p.y); maxZ = Mathf.Max(maxZ, p.y);
            }
            Vector2 mid = new Vector2((minX + maxX) / 2, (minZ + maxZ) / 2);
            float floorY = City04TraversalProbe.GroundHeight(mid) + 0.05f;
            bool playable = region.Id == "F01" || region.Id == "F02" || region.Id == "F03";
            Material surface = region.Id == "F01" ? bar : region.Id == "F02" ? civic : region.Id == "F03" ? shop : closed;
            if (!playable)
            {
                Box(region.Id + " closed I0 shell", new Vector3(mid.x, floorY + 1.65f, mid.y),
                    new Vector3(maxX - minX - 0.5f, 3.3f, maxZ - minZ - 0.5f), surface, true);
                return;
            }
            Box(region.Id + " public floor", new Vector3(mid.x, floorY, mid.y),
                new Vector3(maxX - minX - 0.25f, 0.18f, maxZ - minZ - 0.25f), surface, true);
            float wallY = floorY + 1.45f;
            Box(region.Id + " west wall", new Vector3(minX + 0.12f, wallY, mid.y), new Vector3(0.24f, 2.9f, maxZ - minZ), closed, true);
            Box(region.Id + " east wall", new Vector3(maxX - 0.12f, wallY, mid.y), new Vector3(0.24f, 2.9f, maxZ - minZ), closed, true);
            float doorHalf = 1.15f;
            float leftWidth = mid.x - doorHalf - minX;
            Box(region.Id + " public front left", new Vector3(minX + leftWidth / 2, wallY, minZ + 0.12f),
                new Vector3(leftWidth, 2.9f, 0.24f), closed, true);
            Box(region.Id + " public front right", new Vector3(maxX - leftWidth / 2, wallY, minZ + 0.12f),
                new Vector3(leftWidth, 2.9f, 0.24f), closed, true);
            Box(region.Id + " public threshold", new Vector3(mid.x, floorY + 0.13f, minZ),
                new Vector3(2.1f, 0.07f, 0.4f), marker, false);
            // A distinct back service threshold does not become a public through-route.
            Box(region.Id + " service back left", new Vector3(minX + leftWidth / 2, wallY, maxZ - 0.12f),
                new Vector3(leftWidth, 2.9f, 0.24f), closed, true);
            Box(region.Id + " service back right", new Vector3(maxX - leftWidth / 2, wallY, maxZ - 0.12f),
                new Vector3(leftWidth, 2.9f, 0.24f), closed, true);
            Box(region.Id + " service threshold", new Vector3(mid.x, floorY + 0.13f, maxZ),
                new Vector3(2.1f, 0.07f, 0.4f), threshold, false);
            float split = minZ + (maxZ - minZ) * 0.58f;
            Box(region.Id + " inner role partition", new Vector3(mid.x, wallY, split),
                new Vector3(maxX - minX - 1.0f, 2.9f, 0.18f), threshold, true);
            if (region.Id != "F03")
            {
                Box(region.Id + " secondary role pocket", new Vector3(maxX - 1.4f, floorY + 0.6f, maxZ - 2.1f),
                    new Vector3(1.6f, 1.2f, 2.0f), region.Id == "F01" ? bar : civic, true);
                Label(region.Id == "F01" ? "semi-private" : "private records",
                    new Vector2(maxX - 1.4f, maxZ - 2.1f), 1.1f, Color.black);
            }
        }

        private static void BuildCourt(City04Layout.Region region)
        {
            Vector2 mid = Centroid(region.Points);
            Box("S03 semi-private court marker", At(mid, 0.4f), new Vector3(4, 0.8f, 4), threshold, true);
            Label("PUBLIC EDGE / SEMI-PRIVATE COURT", mid + new Vector2(0, 4), 1.3f, Color.black);
        }

        private static void Scenic(string name, Vector2 point, Vector3 size)
        {
            Box("NON-PLAYABLE SCENIC " + name, At(point, size.y / 2), size, scenic, false);
            Label("future: " + name, point, 1.5f, Color.black);
        }

        private static void SoftGround(string name, Vector2 point, Vector3 size)
        {
            Box("NON-PLAYABLE soft ground " + name,
                new Vector3(point.x, City04TraversalProbe.GroundHeight(point) - 0.2f, point.y),
                size, scenic, false);
        }
    }
}
