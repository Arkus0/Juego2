using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Juego2.ART.Editor
{
    /// <summary>
    /// Bounded ART assembly specimen for the accepted X1/S02/W12/Casco/F01 chain.
    /// This is deliberately local to ART: it does not transcribe CITY coordinates or build a town.
    /// Every height/offset below is either a measured source constant, a dimensional-profile
    /// value, or derived from a host/support relation; there are no free world-space Y guesses.
    /// </summary>
    public static class Art01BenchmarkBuilder
    {
        const string Art = "Assets/Arkus/ART";

        // ---- measured source constants (UNITY_SOURCE_AUDIT_NORMALIZED.json) ----
        public const float Storey = 3.122689f;  // selected Quaternius wall module height
        public const float WallOut = 0.0924f;   // source wall outer face beyond its module line
        public const float WallIn = 0.3141f;    // source wall inner face behind its module line
        const float OpenShutterReach = 1.2403f; // open leaves reach +/-1.24 m from a 2 m module centre

        // ---- ART dimensional-profile decisions (ART_01_DIMENSIONAL_PROFILE.md) ----
        public const float Datum = -0.05f;          // flat local site datum beside retained features
        public const float PlinthProjection = 0.10f;
        public const float PlinthBottom = -0.15f;   // plinth/kerb/wall foundations embed below datum
        public const float BarFloor = 0.30f;        // public room: two 0.12-0.18 m risers from street
        public const float HouseFloor = 0.18f;      // private: plinth top is the single door sill
        public const float KerbWidth = 0.20f, KerbTop = 0.13f;
        public const float LandingWidth = 1.70f, LandingDepth = 0.96f, StepTread = 0.30f, StepTop = 0.1775f;
        const float PorchProjection = 1.65f, PorchPostSetback = 1.40f, PorchHalfSpan = 1.65f;
        const float CanopyWallUnderside = 3.03f, CanopyEdgeUnderside = 2.75f, CanopyThickness = 0.07f;
        const float WaterLevel = -2.2f, RiverEdgeZ = -26.5f;
        const int AuditLayer = 31;

        sealed class HouseSpec
        {
            public string id, kit;
            public Vector3 centre;
            public float yaw, floor;
            public int width, depth;
            public bool bar;
        }

        // Houses sit so their plinth outer face meets the kerb outer edge: the accepted route
        // width is never narrowed by facade, plinth or kerb geometry.
        static readonly HouseSpec[] Houses =
        {
            new HouseSpec { id = "S02_stone_house", kit = "art01.assembly.house.two_storey_6x10.v1",
                centre = new Vector3(-6.8f, 0, -19), yaw = -90, width = 6, depth = 10, floor = HouseFloor },
            new HouseSpec { id = "W12_left_house", kit = "art01.assembly.house.two_storey_6x10.v1",
                centre = new Vector3(-6.8f, 0, -5), yaw = -90, width = 6, depth = 10, floor = HouseFloor },
            new HouseSpec { id = "W12_right_house", kit = "art01.assembly.house.two_storey_6x10.v1",
                centre = new Vector3(6.8f, 0, -1), yaw = 90, width = 6, depth = 10, floor = HouseFloor },
            // F01 is shifted so its public door module is centred on the 2.4 m micro-route B.
            new HouseSpec { id = "F01_bar_exterior_threshold", kit = "art01.assembly.house.bar_8x14.v1",
                centre = new Vector3(1, 0, 32), yaw = 0, width = 8, depth = 14, bar = true, floor = BarFloor },
        };

        static float F01Front => 32f - 7f;
        static float RoadEnd => F01Front - WallOut - PlinthProjection; // lane meets F01 plinth face

        // Local specimen stations (z, accepted width class); not CITY coordinates.
        static Vector2[] Stations => new[] {
            new Vector2(-36, 5.5f), new Vector2(-26, 5.5f),
            new Vector2(-23, 2.8f), new Vector2(7, 2.8f),
            new Vector2(13, 6.0f), new Vector2(18, 6.0f),
            new Vector2(22, 2.4f), new Vector2(RoadEnd, 2.4f),
        };
        const int FirstKerbStation = 1; // X1 bridge deck edge is the parapet, not a kerb

        static Transform root;
        static readonly List<GameObject> Dressing = new List<GameObject>();

        // ------------------------------------------------------------------ tagging

        static void Tag(GameObject go, string id, string kit, string role, string connection,
            string collision = "none")
        {
            var tag = go.GetComponent<Art01Piece>() ?? go.AddComponent<Art01Piece>();
            tag.logicalId = id;
            tag.kitId = kit;
            tag.assemblyRole = role;
            tag.connection = connection;
            tag.collisionRole = collision;
        }

        static GameObject Group(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }

        /// <summary>Culture-invariant number text: logical IDs must not depend on the machine locale.</summary>
        static string Inv(float v, string format = "F2") => v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);

        static string SourceKit(string category, string name) =>
            "art01.quaternius." + category.ToLowerInvariant() + "." + name.ToLowerInvariant();

        static GameObject Source(string id, string category, string name, Transform parent,
            Vector3 localPosition, Quaternion localRotation, Vector3 localScale, string role,
            string connection, string materialOverride = null, string kit = null)
        {
            var path = Art + "/External/" + category + "/Models/" + name + ".fbx";
            if (category == "DerivedHuman") path = Art + "/Derived/Characters/" + name + ".fbx";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null) throw new Exception("ART01_SOURCE_MISSING " + path);
            var wrapper = Group(id, parent);
            wrapper.transform.localPosition = localPosition;
            wrapper.transform.localRotation = localRotation;
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(asset, wrapper.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = asset.transform.localRotation; // Props root is approx -90 deg X
            instance.transform.localScale = Vector3.Scale(asset.transform.localScale, localScale); // keep x100 Props root
            Art01Materials.Remap(instance, materialOverride);
            Tag(wrapper, id, kit ?? SourceKit(category, name), role, connection);
            if (role == "DRESSING" || role == "NATURE") Dressing.Add(wrapper);
            return wrapper;
        }

        static GameObject Box(string id, string kit, Transform parent, Vector3 centre, Vector3 size,
            string material, string role, string connection, bool collide = false)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = id;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = centre;
            go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = Art01Materials.Get(material);
            if (!collide) UnityEngine.Object.DestroyImmediate(go.GetComponent<BoxCollider>());
            Tag(go, id, kit, role, connection, collide ? "sole_local_support" : "none");
            if (role == "DRESSING" || role == "NATURE") Dressing.Add(go);
            return go;
        }

        static GameObject BoxSpan(string id, string kit, Transform parent, Vector3 min, Vector3 max,
            string material, string role, string connection, bool collide = false) =>
            Box(id, kit, parent, (min + max) * 0.5f, max - min, material, role, connection, collide);

        static Mesh SaveMesh(Mesh mesh, string name)
        {
            var dir = Art + "/Derived";
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder(Art, "Derived");
            if (!AssetDatabase.IsValidFolder(dir + "/Meshes")) AssetDatabase.CreateFolder(dir, "Meshes");
            var path = dir + "/Meshes/" + name + ".asset";
            mesh.name = name;
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing == null) { AssetDatabase.CreateAsset(mesh, path); return mesh; }
            EditorUtility.CopySerialized(mesh, existing);
            EditorUtility.SetDirty(existing);
            UnityEngine.Object.DestroyImmediate(mesh);
            return existing;
        }

        static GameObject MeshObject(string id, string kit, Mesh mesh, Material[] materials, Transform parent,
            string role, string connection, bool collider = false)
        {
            var go = Group(id, parent);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterials = materials;
            if (collider) go.AddComponent<MeshCollider>().sharedMesh = mesh;
            Tag(go, id, kit, role, connection, collider ? "sole_traversable_owner" : "none");
            return go;
        }

        static GameObject MeshObject(string id, string kit, Mesh mesh, string material, Transform parent,
            string role, string connection, bool collider = false) =>
            MeshObject(id, kit, mesh, new[] { Art01Materials.Get(material) }, parent, role, connection, collider);

        static void Quad(List<Vector3> v, List<int> t, List<Vector2> uv, Vector3 a, Vector3 b, Vector3 c, Vector3 d,
            float uScale = 1, float vScale = 1)
        {
            int s = v.Count;
            v.AddRange(new[] { a, b, c, d });
            float w = Vector3.Distance(a, b) * uScale, h = Vector3.Distance(a, d) * vScale;
            uv.AddRange(new[] { new Vector2(0, 0), new Vector2(w, 0), new Vector2(w, h), new Vector2(0, h) });
            t.AddRange(new[] { s, s + 1, s + 2, s, s + 2, s + 3 });
        }

        // ------------------------------------------------------------------ street / site

        static float HalfWidth(float z)
        {
            var st = Stations;
            if (z <= st[0].x) return st[0].y * 0.5f;
            for (int i = 0; i < st.Length - 1; i++)
                if (z <= st[i + 1].x)
                    return Mathf.Lerp(st[i].y, st[i + 1].y, (z - st[i].x) / (st[i + 1].x - st[i].x)) * 0.5f;
            return st[st.Length - 1].y * 0.5f;
        }

        /// <summary>Crowned street section with an integrated right-side drainage channel.</summary>
        public static float RoadY(float x, float z)
        {
            float hw = HalfWidth(z);
            float y = 0.055f - 0.045f * Mathf.Clamp01(Mathf.Abs(x) / hw);
            float c = hw - 0.20f;
            if (x > c - 0.10f && x < c + 0.10f) y -= 0.03f * (1f - Mathf.Abs(x - c) / 0.10f);
            return y;
        }

        static Mesh RoadMesh()
        {
            // One continuous crowned surface: no stacked road/ground slabs or duplicate path collider.
            // Section: left edge, crown, channel lip, channel invert, channel lip, right edge.
            var st = Stations;
            var verts = new List<Vector3>(); var uv = new List<Vector2>();
            var cobble = new List<int>(); var channel = new List<int>();
            foreach (var station in st)
            {
                float z = station.x, hw = station.y * 0.5f;
                foreach (var s in new[] { -hw, 0f, hw - 0.30f, hw - 0.20f, hw - 0.10f, hw })
                {
                    verts.Add(new Vector3(s, RoadY(s, z), z));
                    uv.Add(new Vector2(s * 0.5f, z * 0.5f));
                }
            }
            const int n = 6;
            for (int i = 0; i < st.Length - 1; i++)
                for (int k = 0; k < n - 1; k++)
                {
                    int a = i * n + k, b = (i + 1) * n + k;
                    var list = k == 2 || k == 3 ? channel : cobble;
                    list.AddRange(new[] { a, b, a + 1, a + 1, b, b + 1 });
                }
            var mesh = new Mesh { subMeshCount = 2 };
            mesh.SetVertices(verts); mesh.SetUVs(0, uv);
            mesh.SetTriangles(cobble, 0); mesh.SetTriangles(channel, 1);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, "art01.street.crowned.s02_w12_casco.v1");
        }

        static Mesh KerbMesh(int side)
        {
            // Continuous mitred kerb outside the traversable edge; embedded foot, no joint gaps.
            var st = Stations;
            var pts = new List<Vector2>();
            for (int i = FirstKerbStation; i < st.Length; i++) pts.Add(new Vector2(side * st[i].y * 0.5f, st[i].x));
            var outer = new List<Vector2>();
            for (int i = 0; i < pts.Count; i++)
            {
                Vector2 Normal(int a, int b) { var d = (pts[b] - pts[a]).normalized; return side * new Vector2(d.y, -d.x); }
                Vector2 n0 = i > 0 ? Normal(i - 1, i) : Normal(i, i + 1);
                Vector2 n1 = i < pts.Count - 1 ? Normal(i, i + 1) : n0;
                var m = (n0 + n1).normalized;
                outer.Add(pts[i] + m * (KerbWidth / Vector2.Dot(m, n1)));
            }
            var v = new List<Vector3>(); var t = new List<int>(); var uv = new List<Vector2>();
            Vector3 P(Vector2 p, float y) => new Vector3(p.x, y, p.y);
            for (int i = 0; i < pts.Count - 1; i++)
            {
                var a0 = pts[i]; var a1 = pts[i + 1]; var b0 = outer[i]; var b1 = outer[i + 1];
                bool flip = side < 0;
                void Q(Vector3 a, Vector3 b, Vector3 c, Vector3 d) { if (flip) Quad(v, t, uv, b, a, d, c); else Quad(v, t, uv, a, b, c, d); }
                Q(P(a0, KerbTop), P(a1, KerbTop), P(b1, KerbTop), P(b0, KerbTop));           // top
                Q(P(b0, PlinthBottom), P(b0, KerbTop), P(b1, KerbTop), P(b1, PlinthBottom)); // outer face
                Q(P(a1, PlinthBottom), P(a1, KerbTop), P(a0, KerbTop), P(a0, PlinthBottom)); // road face
            }
            void Cap(Vector2 a, Vector2 b, bool start)
            {
                bool flip = (side < 0) ^ start;
                if (flip) Quad(v, t, uv, P(b, PlinthBottom), P(a, PlinthBottom), P(a, KerbTop), P(b, KerbTop));
                else Quad(v, t, uv, P(a, PlinthBottom), P(b, PlinthBottom), P(b, KerbTop), P(a, KerbTop));
            }
            Cap(pts[0], outer[0], true);
            Cap(pts[pts.Count - 1], outer[outer.Count - 1], false);
            var mesh = new Mesh();
            mesh.SetVertices(v); mesh.SetUVs(0, uv); mesh.SetTriangles(t, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, "art01.street.kerb." + side + ".v1");
        }

        static readonly List<Rect> FlatSites = new List<Rect>();

        static float DistanceToRect(Rect r, float x, float z)
        {
            float dx = Mathf.Max(r.xMin - x, 0, x - r.xMax), dz = Mathf.Max(r.yMin - z, 0, z - r.yMax);
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>Scenic ground: flat datum beside retained route/building/edge features, valley relief beyond.</summary>
        public static float GroundY(float x, float z)
        {
            float d = z < RiverEdgeZ ? 0 : z - RiverEdgeZ;
            if (z >= Stations[0].x && z <= RoadEnd) d = Mathf.Min(d, Mathf.Max(0, Mathf.Abs(x) - HalfWidth(z) - KerbWidth));
            foreach (var r in FlatSites) d = Mathf.Min(d, DistanceToRect(r, x, z));
            float blend = Mathf.SmoothStep(0, 1, Mathf.Clamp01((d - 1.5f) / 5f));
            float ax = Mathf.Abs(x);
            float relief = Mathf.Max(0, ax - 10f) * 0.06f + 0.14f +
                0.16f * Mathf.Sin(z * 0.23f + ax * 0.41f) + 0.08f * Mathf.Sin(z * 0.57f - ax * 0.19f);
            return Datum + blend * relief;
        }

        static Mesh GroundMesh(int side)
        {
            var xs = new List<float>();
            for (float x = 0; x < 16f - 0.001f; x += 0.5f) xs.Add(x);
            for (float x = 16f; x <= 48f + 0.001f; x += 2f) xs.Add(x);
            var zs = new List<float>();
            for (float z = RiverEdgeZ; z <= 48f + 0.001f; z += 0.5f) zs.Add(z);
            var verts = new List<Vector3>(); var uv = new List<Vector2>(); var tris = new List<int>();
            foreach (var z in zs)
                foreach (var x in xs)
                {
                    verts.Add(new Vector3(side * x, GroundY(side * x, z), z));
                    uv.Add(new Vector2(x * 0.34f, z * 0.34f));
                }
            int w = xs.Count;
            for (int j = 0; j < zs.Count - 1; j++)
                for (int i = 0; i < w - 1; i++)
                {
                    int a = j * w + i, b = a + w;
                    if (side > 0) tris.AddRange(new[] { a, b, a + 1, a + 1, b, b + 1 });
                    else tris.AddRange(new[] { a, a + 1, b, a + 1, b + 1, b });
                }
            var mesh = new Mesh { indexFormat = IndexFormat.UInt32 };
            mesh.SetVertices(verts); mesh.SetUVs(0, uv); mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, "art01.site.scenic_bank." + side + ".v1");
        }

        static Mesh BridgeSpanMesh(float halfWidth, float z0, float z1, float springA, float springB, float rise)
        {
            // Stone deck body with one segmental arch; top is under the road/parapets, sides are the spandrels.
            float span = springB - springA, zc = (springA + springB) * 0.5f;
            float radius = (span * span * 0.25f + rise * rise) / (2 * rise);
            float cy = WaterLevel + rise - radius;
            float Bottom(float z) => z > springA && z < springB
                ? cy + Mathf.Sqrt(Mathf.Max(0, radius * radius - (z - zc) * (z - zc)))
                : WaterLevel - 0.2f;
            var zs = new List<float> { z0 };
            for (float z = springA; z < springB - 0.001f; z += 0.25f) zs.Add(z);
            zs.Add(springB);
            zs.Add(z1);
            var v = new List<Vector3>(); var t = new List<int>(); var uv = new List<Vector2>();
            for (int i = 0; i < zs.Count - 1; i++)
            {
                float a = zs[i], b = zs[i + 1];
                if (b - a < 0.0001f) continue;
                float ba = Bottom(a + 0.0001f), bb = Bottom(b - 0.0001f);
                if (a == springA) ba = WaterLevel; if (b == springB) bb = WaterLevel;
                Quad(v, t, uv, new Vector3(halfWidth, ba, a), new Vector3(halfWidth, 0, a),
                    new Vector3(halfWidth, 0, b), new Vector3(halfWidth, bb, b), 0.5f, 0.5f);   // +x spandrel
                Quad(v, t, uv, new Vector3(-halfWidth, ba, a), new Vector3(-halfWidth, bb, b),
                    new Vector3(-halfWidth, 0, b), new Vector3(-halfWidth, 0, a), 0.5f, 0.5f);  // -x spandrel
                Quad(v, t, uv, new Vector3(-halfWidth, ba, a), new Vector3(halfWidth, ba, a),
                    new Vector3(halfWidth, bb, b), new Vector3(-halfWidth, bb, b), 0.5f, 0.5f); // intrados / pier foot
            }
            Quad(v, t, uv, new Vector3(-halfWidth, 0, z0), new Vector3(-halfWidth, 0, z1),
                new Vector3(halfWidth, 0, z1), new Vector3(halfWidth, 0, z0), 0.5f, 0.5f);           // closed deck top
            foreach (var (z, s) in new[] { (z0, -1f), (z1, 1f) })
            {
                float b = Bottom(z);
                if (s < 0) Quad(v, t, uv, new Vector3(halfWidth, b, z), new Vector3(-halfWidth, b, z),
                    new Vector3(-halfWidth, 0, z), new Vector3(halfWidth, 0, z), 0.5f, 0.5f);
                else Quad(v, t, uv, new Vector3(-halfWidth, b, z), new Vector3(halfWidth, b, z),
                    new Vector3(halfWidth, 0, z), new Vector3(-halfWidth, 0, z), 0.5f, 0.5f);
            }
            var mesh = new Mesh();
            mesh.SetVertices(v); mesh.SetUVs(0, uv); mesh.SetTriangles(t, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, "art01.bridge.arch_span.x1.v1");
        }

        static void Bridgehead(Transform parent)
        {
            // Isolated Puente Viejo -> S02 bridgehead sample: arch span, spandrel faces, parapets,
            // river embankment and visible water. Not CITY-07's final bridge geometry.
            const string Span = "art01.derived.bridge.arch_span.v1";
            const string Parapet = "art01.derived.bridge.parapet_capped.v1";
            const string Bank = "art01.derived.retaining.embankment.v1";
            float edge = Stations[0].y * 0.5f, outerFace = edge + 0.55f;
            float z0 = Stations[0].x, z1 = RiverEdgeZ;
            MeshObject("art01.bridge.x1.arch_span", Span,
                BridgeSpanMesh(outerFace, z0, z1, -35f, RiverEdgeZ - 0.6f, 1.5f), "Stone", parent,
                "BRIDGE_SPAN", "SUPPORTS X1 deck road and parapets; SPRINGS_FROM embankment; street owns traversal");
            float pz0 = z0, pz1 = Stations[1].x;
            foreach (int side in new[] { -1, 1 })
            {
                string s = side < 0 ? "left" : "right";
                BoxSpan($"art01.bridge.x1.{s}.parapet", Parapet, parent,
                    new Vector3(side < 0 ? -outerFace : edge, -0.10f, pz0),
                    new Vector3(side < 0 ? -edge : outerFace, 1.08f, pz1), "StoneShadow", "PARAPET",
                    "SUPPORTED_BY arch span/bridgehead ground; MEETS road edge and S02 kerb", true);
                BoxSpan($"art01.bridge.x1.{s}.coping", Parapet, parent,
                    new Vector3(side < 0 ? -outerFace - 0.065f : edge - 0.065f, 1.08f, pz0),
                    new Vector3(side < 0 ? -edge + 0.065f : outerFace + 0.065f, 1.20f, pz1 + 0.05f),
                    "StoneTrim", "CAP", "CAPS parapet");
                BoxSpan($"art01.river.{s}.embankment", Bank, parent,
                    new Vector3(side < 0 ? -40f : outerFace, WaterLevel - 0.2f, RiverEdgeZ - 0.6f),
                    new Vector3(side < 0 ? -outerFace : 40f, Datum, RiverEdgeZ), "Stone", "RETAINING",
                    "RETAINS S02 bank above river; MEETS arch span abutment", true);
                BoxSpan($"art01.river.{s}.embankment_coping", Bank, parent,
                    new Vector3(side < 0 ? -40f : outerFace, Datum, RiverEdgeZ - 0.65f),
                    new Vector3(side < 0 ? -outerFace : 40f, Datum + 0.10f, RiverEdgeZ + 0.05f),
                    "StoneTrim", "CAP", "CAPS embankment; MEETS bank datum");
            }
            BoxSpan("art01.river.water", "art01.derived.site.river_water.v1", parent,
                new Vector3(-40f, WaterLevel - 0.02f, -48f), new Vector3(40f, WaterLevel, RiverEdgeZ),
                "Water", "SCENIC_WATER", "visible river below X1 arch; no traversable collider");
        }

        static void W12CascoEdge(Transform parent, HouseSpec rightHouse)
        {
            // Capped retaining edge between the W12 right house and the Casco reveal. Its inner
            // face continues the kerb outer edge; it starts at the house's north wall face.
            float inner = 1.4f + KerbWidth;
            float z0 = rightHouse.centre.z + rightHouse.width * 0.5f + WallOut - 0.02f, z1 = 6.75f;
            BoxSpan("art01.w12_casco.right.retaining", "art01.derived.retaining.capped.v1", parent,
                new Vector3(inner, PlinthBottom, z0), new Vector3(inner + 0.42f, 0.48f, z1),
                "Stone", "RETAINING",
                "SUPPORTED_BY site datum; MEETS W12 right house wall and kerb; CLEAR_OF 2.8 m route", true);
            BoxSpan("art01.w12_casco.right.coping", "art01.derived.retaining.capped.v1", parent,
                new Vector3(inner - 0.055f, 0.48f, z0), new Vector3(inner + 0.475f, 0.57f, z1 + 0.05f),
                "StoneTrim", "CAP", "CAPS W12/Casco retaining wall; resolves exposed end");
        }

        // ------------------------------------------------------------------ building kit

        static Mesh GableMesh(float halfWidth, float rise, float outer, float inner, string name)
        {
            var verts = new[] {
                new Vector3(-halfWidth, 0, outer), new Vector3(halfWidth, 0, outer), new Vector3(0, rise, outer),
                new Vector3(-halfWidth, 0, -inner), new Vector3(halfWidth, 0, -inner), new Vector3(0, rise, -inner),
            };
            var tris = new[] { 0, 1, 2, 3, 5, 4, 0, 4, 1, 0, 3, 4, 1, 5, 2, 1, 4, 5, 2, 3, 0, 2, 5, 3 };
            var mesh = new Mesh { vertices = verts, triangles = tris,
                uv = new[] { new Vector2(0,0), new Vector2(2*halfWidth,0), new Vector2(halfWidth,rise),
                             new Vector2(0,0), new Vector2(2*halfWidth,0), new Vector2(halfWidth,rise) } };
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, name);
        }

        static Mesh RenderHostMesh(string kind)
        {
            // Juego2 derivative: one 2 m render host with the SAME faces as the stone source wall
            // (outer +0.0924, inner -0.3141), so storeys are flush and corners close identically.
            float left = -1f, right = 1f, bottom = 0f, top = Storey;
            float apertureLeft = -0.805f, apertureRight = 0.805f;
            float apertureBottom = kind == "Door_Flat" ? 0f : 0.94f;
            float apertureTop = kind == "Door_Flat" ? 2.39f : 2.53f;
            var v = new List<Vector3>(); var t = new List<int>(); var uv = new List<Vector2>();
            void Prism(float x0, float x1, float y0, float y1)
            {
                if (x1 <= x0 || y1 <= y0) return;
                float f = WallOut, b = -WallIn;
                Quad(v, t, uv, new Vector3(x0,y0,f), new Vector3(x1,y0,f), new Vector3(x1,y1,f), new Vector3(x0,y1,f));
                Quad(v, t, uv, new Vector3(x1,y0,b), new Vector3(x0,y0,b), new Vector3(x0,y1,b), new Vector3(x1,y1,b));
                Quad(v, t, uv, new Vector3(x0,y0,b), new Vector3(x0,y0,f), new Vector3(x0,y1,f), new Vector3(x0,y1,b));
                Quad(v, t, uv, new Vector3(x1,y0,f), new Vector3(x1,y0,b), new Vector3(x1,y1,b), new Vector3(x1,y1,f));
                Quad(v, t, uv, new Vector3(x0,y1,f), new Vector3(x1,y1,f), new Vector3(x1,y1,b), new Vector3(x0,y1,b));
                Quad(v, t, uv, new Vector3(x0,y0,b), new Vector3(x1,y0,b), new Vector3(x1,y0,f), new Vector3(x0,y0,f));
            }
            if (kind == "Straight") Prism(left, right, bottom, top);
            else
            {
                Prism(left, apertureLeft, bottom, top);
                Prism(apertureRight, right, bottom, top);
                Prism(apertureLeft, apertureRight, apertureTop, top);
                Prism(apertureLeft, apertureRight, bottom, apertureBottom);
            }
            var mesh = new Mesh();
            mesh.SetVertices(v); mesh.SetUVs(0, uv); mesh.SetTriangles(t, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, "art01.facade.render_host." + kind.ToLowerInvariant() + ".v1");
        }

        static Mesh LeanToMesh()
        {
            // 3.6 m wide threshold canopy; back edge embedded 0.02 m in the facade, pitched to a fascia.
            float w = PorchHalfSpan + 0.15f, back = 0.02f, front = -PorchProjection;
            float yb = CanopyWallUnderside + 0.0034f, yf = CanopyEdgeUnderside, th = CanopyThickness;
            var v = new[] {
                new Vector3(-w, yb + th, back), new Vector3(w, yb + th, back),
                new Vector3(-w, yf + th, front), new Vector3(w, yf + th, front),
                new Vector3(-w, yb, back), new Vector3(w, yb, back),
                new Vector3(-w, yf, front), new Vector3(w, yf, front),
            };
            var t = new[] { 0,1,2, 1,3,2, 4,6,5, 5,6,7, 2,3,6, 3,7,6, 0,2,4, 2,6,4, 1,5,3, 3,5,7 };
            var uv = new[] { new Vector2(0,0), new Vector2(2*w,0), new Vector2(0,1.67f), new Vector2(2*w,1.67f),
                             new Vector2(0,0), new Vector2(2*w,0), new Vector2(0,1.67f), new Vector2(2*w,1.67f) };
            var mesh = new Mesh { vertices = v, triangles = t, uv = uv };
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, "art01.roof.threshold.lean_to.v1");
        }

        /// <summary>Deepest facade surface (house-local z, inward positive for the front) over a rectangle.</summary>
        static float FacadeSurfaceDepth(Transform H, GameObject host, float x0, float x1, float y0, float y1, float outside)
        {
            var temp = TempColliders(host);
            float deepest = float.NaN;
            bool old = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = true;
            try
            {
                for (float x = x0; x <= x1 + 1e-4f; x += 0.05f)
                    for (float y = y0; y <= y1 + 1e-4f; y += 0.05f)
                        if (Physics.Raycast(H.TransformPoint(new Vector3(x, y, outside)), H.forward, out var hit, 2f, AuditMask,
                                QueryTriggerInteraction.Ignore))
                            deepest = float.IsNaN(deepest) ? H.InverseTransformPoint(hit.point).z
                                : Mathf.Max(deepest, H.InverseTransformPoint(hit.point).z);
            }
            finally { Physics.queriesHitBackfaces = old; }
            foreach (var go in temp) UnityEngine.Object.DestroyImmediate(go);
            Physics.SyncTransforms();
            if (float.IsNaN(deepest)) throw new Exception("ART01_FACADE_SURFACE_UNMEASURED " + host.name);
            return deepest;
        }

        static float CanopyUndersideAt(float setback) =>
            CanopyWallUnderside - (CanopyWallUnderside - CanopyEdgeUnderside) * (setback / PorchProjection);

        static string WallKit(string kind, bool plaster) => plaster
            ? "art01.derived.render_host." + kind.ToLowerInvariant() + ".v1"
            : "art01.quaternius.medieval.wall_unevenbrick_" + kind.ToLowerInvariant();

        static void Wall(Transform house, int index, float x, float z, float y, Quaternion face,
            string kind, string doorMode, bool plaster, string shutters)
        {
            var pos = new Vector3(x, y, z);
            var id = $"art01.wall.{house.name}.{index}.{Inv(y)}";
            var connection = plaster
                ? "SUPPORTED_BY plinth/lower storey; HOSTS real " + kind + " aperture; MEETS adjacent module"
                : "SUPPORTED_BY plinth; HOSTS " + kind + "; MEETS adjacent module";
            GameObject host;
            if (plaster)
            {
                host = MeshObject(id, WallKit(kind, true), RenderHostMesh(kind), "Plaster", house, "FACADE_HOST", connection);
                host.transform.localPosition = pos; host.transform.localRotation = face;
            }
            else host = Source(id, "Medieval", "Wall_UnevenBrick_" + kind, house, pos, face, Vector3.one,
                "FACADE_HOST", connection, null, WallKit(kind, false));
            float cz = (WallOut - WallIn) * 0.5f, depth = WallOut + WallIn;
            if (kind == "Door_Flat")
            {
                Source(id + ".frame", "Medieval", "DoorFrame_Flat_Brick", house,
                    pos, face, Vector3.one, "HOSTED_INSERT", "FILLS host opening; jamb/lintel return");
                // Leaf recessed 0.10 m behind the frame face: a readable reveal, not a flush sticker.
                var leafOffset = face * new Vector3(0.5f, 0, -0.10f);
                if (doorMode == "private")
                    Source(id + ".leaf", "Medieval", "Door_3_Flat", house, pos + leafOffset, face,
                        Vector3.one, "HOSTED_INSERT", "FILLS private/closed opening; recessed reveal");
                else
                    Source(id + ".open_leaf", "Medieval", "Door_3_Flat", house, pos + leafOffset,
                        face * Quaternion.Euler(0, -85, 0), Vector3.one, "HOSTED_INSERT",
                        "open leaf swings inward against the jamb; keeps public clear width");
                foreach (float side in new[] { -0.9025f, 0.9025f })
                {
                    var jamb = host.AddComponent<BoxCollider>();
                    jamb.size = new Vector3(0.195f, Storey, depth); jamb.center = new Vector3(side, Storey * 0.5f, cz);
                }
                var lintel = host.AddComponent<BoxCollider>();
                lintel.size = new Vector3(1.61f, Storey - 2.39f, depth);
                lintel.center = new Vector3(0, (Storey + 2.39f) * 0.5f, cz);
            }
            else if (kind == "Window_Wide_Flat")
            {
                foreach (float side in new[] { -0.9025f, 0.9025f })
                {
                    var jamb = host.AddComponent<BoxCollider>();
                    jamb.size = new Vector3(0.195f, Storey, depth); jamb.center = new Vector3(side, Storey * 0.5f, cz);
                }
                var sill = host.AddComponent<BoxCollider>();
                sill.size = new Vector3(1.61f, 0.94f, depth); sill.center = new Vector3(0, 0.47f, cz);
                var windowLintel = host.AddComponent<BoxCollider>();
                windowLintel.size = new Vector3(1.61f, Storey - 2.53f, depth);
                windowLintel.center = new Vector3(0, (Storey + 2.53f) * 0.5f, cz);
                Source(id + ".window", "Medieval", "Window_Wide_Flat1", house, pos, face,
                    Vector3.one, "HOSTED_INSERT", "FILLS real hosted opening");
                if (shutters == "open")
                    Source(id + ".shutters", "Medieval", "WindowShutters_Wide_Flat_Open", house,
                        pos, face, Vector3.one, "HOSTED_INSERT",
                        "ATTACHED_TO opening frame; leaves CLEAR_OF neighbouring openings and facade end", "ShutterGreen");
                else if (shutters == "closed")
                    Source(id + ".shutters", "Medieval", "WindowShutters_Wide_Flat_Closed", house,
                        pos, face, Vector3.one, "HOSTED_INSERT",
                        "FILLS opening in front of glazing where open leaves would clash", "ShutterGreen");
            }
            else
            {
                var coll = host.AddComponent<BoxCollider>();
                coll.size = new Vector3(2, Storey, depth); coll.center = new Vector3(0, Storey * 0.5f, cz);
            }
        }

        /// <summary>Open leaves need a plain neighbour module on both sides; otherwise they would clash or overhang.</summary>
        static string ShutterMode(string[] row, int i, bool barGround)
        {
            if (row[i] != "Window_Wide_Flat") return "none";
            bool free = i > 0 && i < row.Length - 1 && row[i - 1] == "Straight" && row[i + 1] == "Straight";
            if (free) return "open";
            return barGround ? "none" : "closed";
        }

        static readonly int AuditMask = 1 << AuditLayer;

        static List<GameObject> TempColliders(GameObject target)
        {
            var list = new List<GameObject>();
            foreach (var mf in target.GetComponentsInChildren<MeshFilter>(true))
            {
                var go = new GameObject("__art01_measure") { layer = AuditLayer };
                go.transform.SetParent(mf.transform, false);
                go.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
                list.Add(go);
            }
            Physics.SyncTransforms();
            return list;
        }

        static float Probe(Vector3 origin, Vector3 dir, float length)
        {
            bool old = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = true;
            try
            {
                return Physics.Raycast(origin, dir, out var hit, length, AuditMask, QueryTriggerInteraction.Ignore)
                    ? hit.point.y : float.NaN;
            }
            finally { Physics.queriesHitBackfaces = old; }
        }

        static float MaxMeasured(float a, float b) => float.IsNaN(a) ? b : float.IsNaN(b) ? a : Mathf.Max(a, b);
        static float MinMeasured(float a, float b) => float.IsNaN(a) ? b : float.IsNaN(b) ? a : Mathf.Min(a, b);

        static void House(HouseSpec spec)
        {
            var house = Group(spec.id, root);
            house.transform.localPosition = spec.centre;
            house.transform.localRotation = Quaternion.Euler(0, spec.yaw, 0);
            var H = house.transform;
            Tag(house, "art01.assembly." + spec.id, spec.kit, "BUILDING_ASSEMBLY",
                "site->plinth->facade hosts->openings->roof->threshold->street");
            int width = spec.width, depth = spec.depth, nx = width / 2, nz = depth / 2;
            float floorY = spec.floor, front = -depth * 0.5f, back = depth * 0.5f;
            float ax = width * 0.5f + WallOut, az = depth * 0.5f + WallOut; // outer wall faces
            int door = spec.bar ? 1 : nx / 2;
            float doorX = -width * 0.5f + 1 + 2 * door;

            for (int floor = 0; floor < 2; floor++)
            {
                float y = floorY + floor * Storey;
                bool plasterFloor = floor == 1;
                var frontRow = new string[nx]; var backRow = new string[nx];
                var leftRow = new string[nz]; var rightRow = new string[nz];
                for (int i = 0; i < nx; i++)
                {
                    frontRow[i] = floor == 0 && i == door ? "Door_Flat" :
                        floor == 1 ? (i % 2 == 0 || spec.bar && i == nx - 1 ? "Window_Wide_Flat" : "Straight")
                                   : (i != nx - 1 ? "Window_Wide_Flat" : "Straight");
                    backRow[i] = i % 2 == 0 && floor == 1 ? "Window_Wide_Flat" : "Straight";
                }
                if (spec.bar && floor == 1) { frontRow[0] = "Straight"; frontRow[1] = "Window_Wide_Flat"; frontRow[2] = "Straight"; frontRow[3] = "Window_Wide_Flat"; }
                for (int i = 0; i < nz; i++)
                {
                    leftRow[i] = i == 1 && floor == 1 ? "Window_Wide_Flat" : "Straight";
                    rightRow[i] = i == nz - 2 && floor == 1 ? "Window_Wide_Flat" : "Straight";
                }
                for (int i = 0; i < nx; i++)
                {
                    float x = -width * 0.5f + 1 + 2 * i;
                    Wall(H, i, x, front, y, Quaternion.Euler(0, 180, 0), frontRow[i],
                        spec.bar && floor == 0 && i == door ? "public" : "private", plasterFloor,
                        ShutterMode(frontRow, i, spec.bar && floor == 0));
                    Wall(H, i + 100, x, back, y, Quaternion.identity, backRow[i], "private", plasterFloor,
                        ShutterMode(backRow, i, false));
                }
                for (int i = 0; i < nz; i++)
                {
                    float z = -depth * 0.5f + 1 + 2 * i;
                    Wall(H, i + 200, -width * 0.5f, z, y, Quaternion.Euler(0, 270, 0), leftRow[i], "private",
                        plasterFloor, ShutterMode(leftRow, i, false));
                    Wall(H, i + 300, width * 0.5f, z, y, Quaternion.Euler(0, 90, 0), rightRow[i], "private",
                        plasterFloor, ShutterMode(rightRow, i, false));
                }
                for (int sx = -1; sx <= 1; sx += 2)
                    for (int sz = -1; sz <= 1; sz += 2)
                    {
                        // Closes the WallOut x WallOut notch where two module lines meet.
                        BoxSpan($"art01.corner_closure.{spec.id}.{floor}.{sx}.{sz}",
                            "art01.derived.facade.corner_closure.v1", H,
                            new Vector3(sx < 0 ? -ax : width * 0.5f, y, sz < 0 ? -az : depth * 0.5f),
                            new Vector3(sx < 0 ? -width * 0.5f : ax, y + Storey, sz < 0 ? -depth * 0.5f : az),
                            plasterFloor ? "Plaster" : "Stone", "CORNER_CLOSURE",
                            "MEETS two facade module ends; fills measured outer-face notch");
                        if (floor == 0)
                            Source($"art01.corner.{spec.id}.{floor}.{sx}.{sz}", "Medieval",
                                "Corner_Exterior_Brick", H,
                                new Vector3(sx * width * 0.5f, y, sz * depth * 0.5f),
                                Quaternion.Euler(0, sx == sz ? 0 : 90, 0), Vector3.one,
                                "CORNER", "MEETS two stone facade hosts; SUPPORTED_BY plinth");
                    }
            }

            // Continuous plinth ring from the wall inner face to 0.10 m beyond the outer face.
            // Only the public threshold interrupts it; a private door sill is the plinth top.
            const string Plinth = "art01.derived.plinth.segment.v1";
            float px = ax + PlinthProjection, pz = az + PlinthProjection;
            float ix = width * 0.5f - WallIn, iz = depth * 0.5f - WallIn;
            string plinthConn = "SUPPORTED_BY site datum (embedded); SUPPORTS facade hosts; MEETS street edge";
            if (spec.bar)
            {
                BoxSpan($"art01.plinth.{spec.id}.front.0", Plinth, H, new Vector3(-px, PlinthBottom, -pz),
                    new Vector3(doorX - LandingWidth * 0.5f, floorY, -iz), "StoneShadow", "PLINTH", plinthConn);
                BoxSpan($"art01.plinth.{spec.id}.front.1", Plinth, H, new Vector3(doorX + LandingWidth * 0.5f, PlinthBottom, -pz),
                    new Vector3(px, floorY, -iz), "StoneShadow", "PLINTH", plinthConn);
            }
            else BoxSpan($"art01.plinth.{spec.id}.front", Plinth, H, new Vector3(-px, PlinthBottom, -pz),
                new Vector3(px, floorY, -iz), "StoneShadow", "PLINTH", plinthConn + "; private door sill");
            BoxSpan($"art01.plinth.{spec.id}.back", Plinth, H, new Vector3(-px, PlinthBottom, iz),
                new Vector3(px, floorY, pz), "StoneShadow", "PLINTH", plinthConn);
            BoxSpan($"art01.plinth.{spec.id}.left", Plinth, H, new Vector3(-px, PlinthBottom, -iz),
                new Vector3(-ix, floorY, iz), "StoneShadow", "PLINTH", plinthConn);
            BoxSpan($"art01.plinth.{spec.id}.right", Plinth, H, new Vector3(ix, PlinthBottom, -iz),
                new Vector3(px, floorY, iz), "StoneShadow", "PLINTH", plinthConn);

            // Roof: named low-pitch derivative, placed so its measured underside meets the outer
            // wall-top line; gables are then fitted to the measured underside.
            float eave = floorY + 2 * Storey;
            string roofName = width == 8 ? "Roof_RoundTiles_8x14" : "Roof_RoundTiles_6x10";
            string roofKit = width == 8 ? "art01.derived.roof.lowpitch.8x14.v1" : "art01.derived.roof.lowpitch.6x10.v1";
            Vector3 roofScale = width == 8 ? new Vector3(0.91f, 0.45f, 0.96f) : new Vector3(0.86f, 0.45f, 0.94f);
            var roof = Source($"art01.roof.lowpitch.{width}x{depth}.{spec.id}", "Medieval", roofName,
                H, new Vector3(0, eave, 0), Quaternion.identity, roofScale,
                "ROOF_DERIVED", "CAPS facade perimeter; underside MEETS outer wall top; 0.45 pitch-scale named derivative",
                null, roofKit);
            var temp = TempColliders(roof);
            float Underside(float x, float z) => Probe(H.TransformPoint(new Vector3(x, eave - 4f, z)), Vector3.up, 12f);
            float Topside(float x, float z) => Probe(H.TransformPoint(new Vector3(x, eave + 12f, z)), Vector3.down, 16f);
            float atFace = MaxMeasured(Underside(ax - 0.01f, 0), Underside(-ax + 0.01f, 0));
            if (float.IsNaN(atFace)) throw new Exception("ART01_ROOF_UNDERSIDE_UNMEASURED " + spec.id);
            roof.transform.localPosition += Vector3.up * (eave - atFace);
            Physics.SyncTransforms();
            float rise = 0;
            var gablePlanes = new[] { front - WallOut + 0.01f, front + WallIn - 0.01f, back - WallIn + 0.01f, back + WallOut - 0.01f };
            for (int k = 1; k < 20; k++)
            {
                float x = -ax + 2 * ax * k / 20f, slope = 1 - Mathf.Abs(x) / ax;
                foreach (var z in gablePlanes)
                {
                    float u = Underside(x, z);
                    if (!float.IsNaN(u)) rise = Mathf.Max(rise, (u - eave) / slope);
                }
            }
            rise += 0.03f;
            for (int k = 1; k < 20; k++)
            {
                float x = -ax + 2 * ax * k / 20f, top = eave + rise * (1 - Mathf.Abs(x) / ax);
                foreach (var z in gablePlanes)
                {
                    float tp = Topside(x, z);
                    if (float.IsNaN(tp) || tp < top + 0.01f)
                        throw new Exception($"ART01_GABLE_PIERCES_ROOF {spec.id} x={x:F2} top={top:F3} roof={tp:F3}");
                }
            }
            float plateOuter = ax + 0.14f;
            float plateTop = MinMeasured(Underside(plateOuter, 0), Underside(-plateOuter, 0));
            if (float.IsNaN(plateTop)) throw new Exception("ART01_EAVE_UNDERSIDE_UNMEASURED " + spec.id);
            foreach (var go in temp) UnityEngine.Object.DestroyImmediate(go);
            Physics.SyncTransforms();

            var gableMesh = GableMesh(ax, rise, WallOut, WallIn, $"art01.gable.{width}.v1");
            foreach (var (name, z, yaw) in new[] { ("front", front, 180f), ("back", back, 0f) })
            {
                var gable = MeshObject($"art01.gable.{spec.id}.{name}", "art01.derived.gable.stone.v1", gableMesh,
                    "Stone", H, "GABLE", "MEETS top-storey facade face flush; fitted to measured roof underside");
                gable.transform.localPosition = new Vector3(0, eave, z);
                gable.transform.localRotation = Quaternion.Euler(0, yaw, 0);
            }
            for (int sx = -1; sx <= 1; sx += 2)
                BoxSpan($"art01.eave.{spec.id}.{sx}", "art01.derived.eave.timber_plate.v1", H,
                    new Vector3(sx < 0 ? -plateOuter : ax, plateTop - 0.12f, -az - 0.45f),
                    new Vector3(sx < 0 ? -ax : plateOuter, plateTop, az + 0.45f), "WoodDark", "EAVE",
                    "ATTACHED_TO outer wall face; top MEETS measured roof underside");

            if (spec.bar) PublicThreshold(spec, H, doorX, front, floorY, width, depth);
        }

        static void PublicThreshold(HouseSpec spec, Transform H, float doorX, float front, float floorY,
            int width, int depth)
        {
            const string Threshold = "art01.derived.threshold.public.v1";
            const string Porch = "art01.derived.threshold.porch_frame.v1";
            float face = front - WallOut, riser = face - LandingDepth;
            BoxSpan("art01.f01.threshold.landing", Threshold, H,
                new Vector3(doorX - LandingWidth * 0.5f, Datum, riser),
                new Vector3(doorX + LandingWidth * 0.5f, floorY, front), "StoneTrim", "THRESHOLD",
                "SUPPORTED_BY street (embedded); TRANSITIONS_TO public floor at door line", true);
            BoxSpan("art01.f01.threshold.step", Threshold, H,
                new Vector3(doorX - LandingWidth * 0.5f, Datum, riser - StepTread),
                new Vector3(doorX + LandingWidth * 0.5f, StepTop, riser), "StoneTrim", "THRESHOLD",
                "SUPPORTED_BY street (embedded); TRANSITIONS_TO landing", true);

            var canopy = MeshObject("art01.f01.roof.threshold.lean_to", "art01.derived.threshold.lean_to.v1",
                LeanToMesh(), "TileWet", H, "PORCH_ROOF",
                "back edge embedded in facade under first floor; SUPPORTED_BY two timber posts");
            canopy.transform.localPosition = new Vector3(doorX, 0, face);
            float postZ = face - PorchPostSetback, postTop = CanopyUndersideAt(PorchPostSetback);
            for (int side = -1; side <= 1; side += 2)
            {
                float px = doorX + side * PorchHalfSpan;
                BoxSpan($"art01.f01.porch.post_foot.{side}", Porch, H,
                    new Vector3(px - 0.16f, PlinthBottom, postZ - 0.16f), new Vector3(px + 0.16f, 0.25f, postZ + 0.16f),
                    "StoneTrim", "PORCH_FOOT", "SUPPORTED_BY site datum (embedded); SUPPORTS post", true);
                BoxSpan($"art01.f01.porch.post.{side}", Porch, H,
                    new Vector3(px - 0.085f, 0.25f, postZ - 0.085f), new Vector3(px + 0.085f, postTop, postZ + 0.085f),
                    "WoodDark", "PORCH_POST", "SUPPORTED_BY stone foot; SUPPORTS lean-to underside", true);
            }
            BoxSpan("art01.f01.porch.fascia", Porch, H,
                new Vector3(doorX - PorchHalfSpan - 0.175f, CanopyEdgeUnderside - 0.04f, face - PorchProjection - 0.11f),
                new Vector3(doorX + PorchHalfSpan + 0.175f, CanopyEdgeUnderside + CanopyThickness + 0.05f, face - PorchProjection),
                "WoodDark", "EAVE", "CAPS threshold roof edge");

            // Flush wall sign on the plain ground-storey module; one readable face toward the street.
            float signX = -width * 0.5f + 7f;
            // Back embedded 0.01 m behind the deepest measured stone surface: no gap behind the board.
            var signHost = H.Find($"art01.wall.{spec.id}.3.{Inv(floorY)}").gameObject;
            float signBack = FacadeSurfaceDepth(H, signHost, signX - 0.70f, signX + 0.70f, floorY + 2.05f, floorY + 2.47f, face - 0.5f) + 0.01f;
            BoxSpan("art01.f01.sign.board", "art01.derived.sign.bar.v1", H,
                new Vector3(signX - 0.70f, floorY + 2.05f, face - 0.08f),
                new Vector3(signX + 0.70f, floorY + 2.47f, signBack), "WoodDark", "SIGN",
                "ATTACHED_TO plain facade host; back embedded behind measured stone surface");
            var lettering = Group("art01.f01.sign.lettering", H);
            lettering.transform.localPosition = new Vector3(signX, floorY + 2.26f, face - 0.086f);
            lettering.transform.localRotation = Quaternion.identity; // readable from the street (-z)
            var text = lettering.AddComponent<TextMesh>();
            text.text = "BAR"; text.fontSize = 60; text.characterSize = 0.045f;
            text.anchor = TextAnchor.MiddleCenter; text.alignment = TextAlignment.Center;
            text.color = new Color(0.83f, 0.77f, 0.58f);
            Tag(lettering, "art01.f01.sign.lettering", "art01.derived.sign.bar.v1", "SIGN_INSERT",
                "ATTACHED_TO sign board face; readable from public approach");
            Source("art01.prop.lamp.f01", "Props", "Lantern_Wall", H,
                new Vector3(doorX - 1.0f, floorY + 1.85f,
                    FacadeSurfaceDepth(H, H.Find($"art01.wall.{spec.id}.0.{Inv(floorY)}").gameObject,
                        doorX - 1.07f, doorX - 0.93f, floorY + 1.90f, floorY + 2.45f, face - 0.5f) + 0.03f),
                Quaternion.Euler(0, 180, 0),
                Vector3.one * 0.45f, "DRESSING", "ATTACHED_TO facade host beside public door; under canopy");

            const string Room = "art01.derived.interior.public_room_shell.v1";
            BoxSpan("art01.f01.interior.floor", Room, H,
                new Vector3(-width * 0.5f, floorY - 0.13f, front), new Vector3(width * 0.5f, floorY, -front),
                "WoodDark", "INTERIOR_FLOOR", "TRANSITIONS_TO public landing at door line", true);
            BoxSpan("art01.f01.interior.ceiling", Room, H,
                new Vector3(-width * 0.5f, floorY + Storey - 0.2f, front), new Vector3(width * 0.5f, floorY + Storey, -front),
                "WoodDark", "INTERIOR_CEILING", "SUPPORTED_BY facades; CAPS public room");
            Source("art01.f01.table", "Props", "Table_Large", H,
                new Vector3(-2.0f, floorY, front + 4.5f), Quaternion.Euler(0, 90, 0),
                Vector3.one * 0.90f, "DRESSING", "SUPPORTED_BY interior floor; CLEAR_OF counter and door");
            Source("art01.f01.stool", "Props", "Stool", H,
                new Vector3(1.0f, floorY, front + 2.7f), Quaternion.identity,
                Vector3.one, "DRESSING", "SUPPORTED_BY interior floor");
            Source("art01.f01.chair", "Props", "Chair_1", H,
                new Vector3(-2.0f, floorY, front + 6.3f), Quaternion.Euler(0, 180, 0),
                Vector3.one, "DRESSING", "SUPPORTED_BY interior floor; faces table end");
            const string Counter = "art01.derived.interior.counter.v1";
            BoxSpan("art01.f01.counter.top", Counter, H, new Vector3(1.74f, floorY + 0.90f, front + 3.2f),
                new Vector3(2.66f, floorY + 1.02f, front + 7.0f), "WoodDark", "INTERIOR_FURNITURE",
                "SUPPORTED_BY counter carcass; public serving edge");
            BoxSpan("art01.f01.counter.front", Counter, H, new Vector3(1.76f, floorY, front + 3.3f),
                new Vector3(1.88f, floorY + 0.90f, front + 6.9f), "PropWood", "INTERIOR_FURNITURE",
                "SUPPORTED_BY floor; SUPPORTS counter top");
            for (int slat = 0; slat < 5; slat++)
                BoxSpan($"art01.f01.counter.stile.{slat}", Counter, H,
                    new Vector3(1.715f, floorY, front + 3.42f + slat * 0.8f),
                    new Vector3(1.76f, floorY + 0.90f, front + 3.48f + slat * 0.8f), "WoodDark", "INTERIOR_FURNITURE",
                    "ATTACHED_TO counter carcass");
            for (int seat = 0; seat < 2; seat++)
                Source($"art01.f01.counter.stool.{seat}", "Props", "Stool", H,
                    new Vector3(1.25f, floorY, front + 4.1f + seat * 1.5f),
                    Quaternion.Euler(0, 90, 0), Vector3.one,
                    "DRESSING", "SUPPORTED_BY public room floor; clear of threshold");
            Source("art01.f01.counter.mug", "Props", "Mug", H,
                new Vector3(2.05f, floorY + 1.02f, front + 4.2f), Quaternion.identity,
                Vector3.one, "DRESSING", "SUPPORTED_BY counter top");
            var point = Group("art01.f01.warm.interiormood", H).AddComponent<Light>();
            point.type = LightType.Point; point.color = new Color(1f, 0.69f, 0.38f);
            point.intensity = 1.25f; point.range = 7f;
            point.transform.localPosition = new Vector3(0, floorY + 2.3f, front + 3.3f);
        }

        // ------------------------------------------------------------------ dressing / nature

        static Vector3 OnGround(float x, float z, float embed = 0.02f) => new Vector3(x, GroundY(x, z) - embed, z);
        static Vector3 OnRoad(float x, float z) => new Vector3(x, RoadY(x, z), z);

        static void NatureAndProps()
        {
            const string Human = "art01.derived.clothed_scale.forastero.v1";
            var humanF01 = Source("art01.human.forastero.f01.scale", "DerivedHuman", "Townsfolk_Forastero", root,
                OnRoad(1.2f, 20.4f), Quaternion.identity, Vector3.one,
                "SCALE_REFERENCE", "standing on accepted 2.4 m pedestrian strip; no NPC behavior", null, Human);
            var humanW12 = Source("art01.human.forastero.w12.scale", "DerivedHuman", "Townsfolk_Forastero", root,
                OnRoad(-0.42f, 7.5f), Quaternion.Euler(0, 180, 0), Vector3.one,
                "SCALE_REFERENCE", "standing on road; visual scale only, no AI or schedule", null, Human);
            ConfigureScaleHuman(humanF01);
            ConfigureScaleHuman(humanW12);
            Source("art01.nature.tree.s02", "Nature", "CommonTree_1", root,
                OnGround(8, -22), Quaternion.Euler(0, 15, 0), Vector3.one * 0.62f,
                "NATURE", "ROOTED_IN bank soil; CLEAR_OF X1/S02 path");
            foreach (var site in new[] { new Vector2(-15f, -18f), new Vector2(15f, -12f),
                                         new Vector2(-18f, 18f), new Vector2(17f, 35f) })
                Source($"art01.nature.tree.bank.{Inv(site.x, "0")}.{Inv(site.y, "0")}", "Nature", "CommonTree_1", root,
                    OnGround(site.x, site.y), Quaternion.Euler(0, site.y * 11f, 0), Vector3.one * 0.76f,
                    "NATURE", "ROOTED_IN scenic bank; no traversal obstruction");
            Source("art01.nature.bush.casco", "Nature", "Bush_Common_Flowers", root,
                OnGround(-5.3f, 12.5f), Quaternion.identity, Vector3.one * 0.55f,
                "NATURE", "ROOTED_IN edge; CLEAR_OF public route");
            Source("art01.nature.fern.w12", "Nature", "Fern_1", root,
                OnGround(-4.2f, -1.15f), Quaternion.identity, Vector3.one * 0.12f,
                "NATURE", "ROOTED_IN W12 plinth edge; CLEAR_OF route");
            Source("art01.nature.rock.s02", "Nature", "Rock_Medium_1", root,
                OnGround(-8.5f, -24.6f, 0.10f), Quaternion.Euler(0, 35, 0), Vector3.one * 0.6f,
                "NATURE", "SUPPORTED_BY bank above embankment; CLEAR_OF path");
            Source("art01.prop.bench.s02", "Props", "Bench", root,
                OnGround(-3.0f, -24.3f), Quaternion.Euler(0, 90, 0), Vector3.one * 0.73f,
                "DRESSING", "SUPPORTED_BY S02 bridgehead pause; CLEAR_OF route and S02 house");
            Source("art01.prop.barrel.f01", "Props", "Barrel", root,
                OnGround(3.2f, 23.2f), Quaternion.identity, Vector3.one * 0.84f,
                "DRESSING", "SUPPORTED_BY ground beside lane end; CLEAR_OF threshold");
            Source("art01.prop.crate.f01", "Props", "Crate_Wooden", root,
                OnGround(-3.2f, 23.5f), Quaternion.Euler(0, 15, 0), Vector3.one * 0.7f,
                "DRESSING", "SUPPORTED_BY ground beside lane end; CLEAR_OF threshold");
            for (int side = -1; side <= 1; side += 2)
            {
                float x = side * 4.15f;
                BoxSpan($"art01.f01.garden.retaining.{side}", "art01.derived.retaining.capped.v1", root,
                    new Vector3(x - 0.21f, PlinthBottom, 16.3f), new Vector3(x + 0.21f, 0.56f, RoadEnd),
                    "Stone", "RETAINING", "SUPPORTED_BY site datum; MEETS F01 frontage plinth line", true);
                BoxSpan($"art01.f01.garden.coping.{side}", "art01.derived.retaining.capped.v1", root,
                    new Vector3(x - 0.265f, 0.56f, 16.25f), new Vector3(x + 0.265f, 0.65f, RoadEnd),
                    "StoneTrim", "CAP", "CAPS retaining garden wall");
                Source($"art01.f01.garden.bush.{side}", "Nature", "Bush_Common", root,
                    OnGround(side * 5.4f, 19.5f), Quaternion.Euler(0, side * 25, 0),
                    Vector3.one * 0.5f, "NATURE", "ROOTED_IN retained garden edge");
                Source($"art01.f01.garden.fern.{side}", "Nature", "Fern_1", root,
                    OnGround(side * 5.1f, 23.2f), Quaternion.identity,
                    Vector3.one * 0.16f, "NATURE", "ROOTED_IN damp wall edge");
            }
        }

        static AnimationClip IdleClip()
        {
            var path = Art + "/External/UAL/UAL1.fbx";
            var found = new List<string>();
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
                if (asset is AnimationClip clip)
                {
                    found.Add(clip.name);
                    if (clip.name == "Armature|Idle_Loop" || clip.name == "Idle_Loop") return clip;
                }
            throw new Exception("ART01_IDLE_CLIP_MISSING " + path + " clips=" + string.Join(",", found));
        }

        static void ConfigureScaleHuman(GameObject wrapper)
        {
            var path = Art + "/Derived/Characters/Art01ScaleIdle.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller != null && controller.layers.Length == 0)
            {
                AssetDatabase.DeleteAsset(path); // discard an interrupted first build
                controller = null;
            }
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(path);
                var state = controller.layers[0].stateMachine.AddState("Idle");
                state.motion = IdleClip();
                controller.layers[0].stateMachine.defaultState = state;
                EditorUtility.SetDirty(controller);
            }
            else
            {
                var state = controller.layers[0].stateMachine.defaultState;
                if (state == null)
                {
                    state = controller.layers[0].stateMachine.AddState("Idle");
                    controller.layers[0].stateMachine.defaultState = state;
                }
                state.motion = IdleClip();
                EditorUtility.SetDirty(controller);
            }
            var animator = wrapper.GetComponentInChildren<Animator>(true);
            if (animator == null) throw new Exception("ART01_DERIVED_HUMANOID_ANIMATOR_MISSING");
            if (animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman)
                throw new Exception("ART01_DERIVED_HUMANOID_AVATAR_INVALID");
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
        }

        // Provisional (renderer-dependent): replaced by the URP lighting/atmosphere baseline after H1-GATE.
        static void Lighting()
        {
            var light = Group("art01.overcast.key", root).AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.83f, 0.89f, 0.93f);
            light.intensity = 0.9f;
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(43, -28, 0);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.62f, 0.65f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.68f, 0.73f, 0.75f);
            RenderSettings.fogStartDistance = 40;
            RenderSettings.fogEndDistance = 95;
        }

        static void WalkInspector()
        {
            var rig = Group("ART01_LOCAL_THIRD_PERSON_INSPECTOR", root);
            rig.transform.position = OnRoad(0, -33) + Vector3.up * 0.03f;
            Tag(rig, "art01.inspector.rig", "art01.inspection.walk_rig.v1", "INSPECTION_ONLY",
                "uses sole authored road/floor collider; no game or CITY-07 identity");
            var body = rig.AddComponent<CharacterController>();
            body.radius = 0.27f; body.height = 1.8f; body.center = new Vector3(0, 0.9f, 0);
            body.stepOffset = 0.25f; body.slopeLimit = 40f;
            var visual = Source("art01.inspector.clothed_scale", "DerivedHuman", "Townsfolk_Forastero",
                rig.transform, Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one,
                "SCALE_REFERENCE", "1.8 m clothed human; visual inspection only", null,
                "art01.derived.clothed_scale.forastero.v1");
            ConfigureScaleHuman(visual);
            var camera = CameraAt("ART01_THIRD_PERSON_CAMERA", new Vector3(0, 1.88f, -36.1f),
                new Vector3(0, 1.43f, -33));
            camera.gameObject.tag = "MainCamera";
            camera.gameObject.AddComponent<AudioListener>();
            rig.AddComponent<Art01WalkInspector>().view = camera;
        }

        public static void Build()
        {
            Dressing.Clear();
            FlatSites.Clear();
            foreach (var h in Houses)
            {
                // World-space footprint (incl. plinth) plus 1 m site apron.
                bool across = Mathf.Abs(Mathf.DeltaAngle(h.yaw, 90)) < 1 || Mathf.Abs(Mathf.DeltaAngle(h.yaw, -90)) < 1;
                float hx = (across ? h.depth : h.width) * 0.5f + WallOut + PlinthProjection + 1f;
                float hz = (across ? h.width : h.depth) * 0.5f + WallOut + PlinthProjection + 1f;
                FlatSites.Add(Rect.MinMaxRect(h.centre.x - hx, h.centre.z - hz, h.centre.x + hx, h.centre.z + hz));
            }
            FlatSites.Add(Rect.MinMaxRect(-5f, 15.5f, 5f, RoadEnd + 0.5f)); // F01 garden/frontage
            FlatSites.Add(Rect.MinMaxRect(1.4f, 1.5f, 3.2f, 7.5f));        // W12/Casco retaining edge

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            root = Group("ART01_BENCHMARK_ONLY_NOT_CITY07_KEEPER", null).transform;
            Tag(root.gameObject, "art01.benchmark.v1", "art01.assembly.benchmark.retained_chain.v1", "BENCHMARK",
                "local specimen order X1/S02 -> W12 -> Casco/micro.B -> F01; not CITY coordinates");
            var road = MeshObject("art01.street.s02_w12_casco_microB", "art01.derived.street.crowned.v1", RoadMesh(),
                new[] { Art01Materials.Get("Cobble"), Art01Materials.Get("StoneShadow") }, root,
                "STREET", "SUPPORTED_BY site/arch span; integrated right drainage channel; MEETS kerbs, F01 plinth", true);
            foreach (int side in new[] { -1, 1 })
                MeshObject("art01.kerb." + side, "art01.derived.street.edge_drain.v1", KerbMesh(side), "StoneTrim", root,
                    "EDGE", "MEETS road edge outside sole traversable collider; embedded foot; MEETS plinths/parapet ends");
            Bridgehead(root);
            // Scenic ground is deliberately non-collidable. Only road/floor/threshold owns travel.
            MeshObject("art01.site.left.scenic", "art01.derived.site.scenic_bank.v1", GroundMesh(-1), "GrassGround", root,
                "SCENIC", "terrain visual below route; flat datum at retained features; no traversable collision");
            MeshObject("art01.site.right.scenic", "art01.derived.site.scenic_bank.v1", GroundMesh(1), "GrassGround", root,
                "SCENIC", "terrain visual below route; flat datum at retained features; no traversable collision");
            foreach (var h in Houses) House(h);
            W12CascoEdge(root, Houses[2]);
            NatureAndProps();
            Lighting();
            WalkInspector();
            var scenePath = Art + "/Art01Benchmark.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("ART01_BENCHMARK_BUILD_GREEN scene=" + scenePath +
                " roadCollision=" + road.GetComponents<MeshCollider>().Length + " dressing=" + Dressing.Count);
        }

        // ------------------------------------------------------------------ captures

        static Camera CameraAt(string name, Vector3 position, Vector3 target)
        {
            var go = new GameObject(name);
            var camera = go.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.73f, 0.78f, 0.8f);
            camera.fieldOfView = 60;
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 125;
            go.transform.position = position;
            go.transform.LookAt(target);
            return camera;
        }

        static void Image(Camera camera, string path)
        {
            var rt = new RenderTexture(1280, 720, 24);
            camera.targetTexture = rt;
            var old = RenderTexture.active;
            RenderTexture.active = rt;
            camera.Render();
            var frame = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            frame.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            frame.Apply();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, frame.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = old;
            UnityEngine.Object.DestroyImmediate(frame);
            UnityEngine.Object.DestroyImmediate(rt);
        }

        static string EvidenceDir(string sub) => Path.GetFullPath(Path.Combine(Application.dataPath,
            "../../../Docs/evidence/WP-ART-01/" + sub));

        // STRUCTURAL checkpoint: one neutral grey material, dressing/nature hidden.
        static readonly (string, Vector3, Vector3)[] StructuralShots = {
            ("puente_s02", new Vector3(0, 1.65f, -35), new Vector3(0, 2.1f, -16)),
            ("x1_arch_embankment", new Vector3(-13.5f, 0.2f, -35.5f), new Vector3(-2.0f, -1.1f, -28.2f)),
            ("w12_casco", new Vector3(0, 1.65f, -10), new Vector3(0, 2.2f, 13)),
            ("w12_private_door", new Vector3(0.9f, 1.6f, -8.2f), new Vector3(-2.0f, 1.0f, -5f)),
            ("w12_casco_joint", new Vector3(0.15f, 1.65f, 5.1f), new Vector3(1.75f, 0.55f, 2.1f)),
            ("w12_casco_edge", new Vector3(0, 1.65f, 2.5f), new Vector3(1.8f, 0.6f, 6.5f)),
            ("roof_eave_corner", new Vector3(3.6f, 2.2f, -9.6f), new Vector3(1.8f, 6.6f, -4.0f)),
            ("f01_exterior", new Vector3(0, 1.65f, 12), new Vector3(0, 3.0f, 28)),
            ("f01_threshold", new Vector3(0, 1.65f, 22.4f), new Vector3(0, 1.3f, 29)),
            ("f01_threshold_side", new Vector3(2.6f, 1.1f, 22.6f), new Vector3(0, 0.25f, 24.6f)),
            ("f01_interior_scale", new Vector3(0, 1.65f, 25.7f), new Vector3(-2.5f, 1.4f, 31)),
        };

        public static void CaptureNeutral() => Capture(true);
        public static void CaptureDressed() => Capture(false);

        public static void CaptureThirdPerson()
        {
            EditorSceneManager.OpenScene(Art + "/Art01Benchmark.unity", OpenSceneMode.Single);
            var inspector = UnityEngine.Object.FindFirstObjectByType<Art01WalkInspector>();
            if (inspector == null || inspector.view == null)
                throw new Exception("ART01_THIRD_PERSON_INSPECTOR_MISSING");
            var views = new[] { ("puente_s02", -33f), ("w12_casco", -8f),
                ("w12_casco_edge", 3f), ("f01_threshold", 20.5f) };
            var outDir = EvidenceDir("provisional_builtin");
            AnimationMode.StartAnimationMode();
            foreach (var tag in UnityEngine.Object.FindObjectsByType<Art01Piece>(FindObjectsSortMode.None))
                if (tag.assemblyRole == "SCALE_REFERENCE")
                {
                    if (!tag.transform.IsChildOf(inspector.transform))
                    {
                        tag.gameObject.SetActive(false);
                        continue;
                    }
                    var dressed = tag.GetComponentInChildren<Animator>(true);
                    if (dressed != null) AnimationMode.SampleAnimationClip(dressed.gameObject, IdleClip(), 0.25f);
                }
            foreach (var entry in views)
            {
                inspector.transform.position = OnRoad(0, entry.Item2) + Vector3.up * 0.03f;
                inspector.transform.rotation = Quaternion.identity;
                var animator = inspector.GetComponentInChildren<Animator>(true);
                AnimationMode.SampleAnimationClip(animator.gameObject, IdleClip(), 0.25f);
                var camera = inspector.view;
                camera.transform.position = inspector.transform.TransformPoint(new Vector3(0, 1.8f, -3.1f));
                camera.transform.LookAt(inspector.transform.position + Vector3.up * 1.35f);
                Image(camera, Path.Combine(outDir, "third_person_" + entry.Item1 + ".png"));
            }
            AnimationMode.StopAnimationMode();
            Debug.Log("ART01_THIRD_PERSON_CAPTURE_GREEN 4 viewpoints clothed scale reference (PROVISIONAL built-in)");
        }

        static void Capture(bool neutral)
        {
            var scenePath = Art + "/Art01Benchmark.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var inspector = UnityEngine.Object.FindFirstObjectByType<Art01WalkInspector>();
            if (inspector != null) inspector.gameObject.SetActive(false);
            var map = new Dictionary<Renderer, Material[]>();
            var hidden = new List<GameObject>();
            Material plain = null;
            AnimationMode.StartAnimationMode();
            foreach (var tag in UnityEngine.Object.FindObjectsByType<Art01Piece>(FindObjectsSortMode.None))
                if (tag.assemblyRole == "SCALE_REFERENCE")
                {
                    var animator = tag.GetComponentInChildren<Animator>(true);
                    if (animator != null) AnimationMode.SampleAnimationClip(animator.gameObject, IdleClip(), 0.25f);
                }
            if (neutral)
            {
                plain = new Material(Shader.Find("Standard"));
                plain.color = new Color(0.72f, 0.73f, 0.73f);
                foreach (var tag in UnityEngine.Object.FindObjectsByType<Art01Piece>(FindObjectsSortMode.None))
                    if (tag.assemblyRole == "DRESSING" || tag.assemblyRole == "NATURE")
                    { tag.gameObject.SetActive(false); hidden.Add(tag.gameObject); }
                foreach (var renderer in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
                {
                    if (renderer.GetComponent<TextMesh>() != null) continue;
                    map[renderer] = renderer.sharedMaterials;
                    var temp = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < temp.Length; i++) temp[i] = plain;
                    renderer.sharedMaterials = temp;
                }
            }
            var outDir = EvidenceDir(neutral ? "structural_checkpoint" : "provisional_builtin");
            foreach (var shot in StructuralShots)
            {
                var camera = CameraAt(shot.Item1, shot.Item2, shot.Item3);
                Image(camera, Path.Combine(outDir, (neutral ? "neutral_" : "dressed_") + shot.Item1 + ".png"));
                UnityEngine.Object.DestroyImmediate(camera.gameObject);
            }
            foreach (var entry in map) entry.Key.sharedMaterials = entry.Value;
            foreach (var go in hidden) go.SetActive(true);
            if (plain != null) UnityEngine.Object.DestroyImmediate(plain);
            AnimationMode.StopAnimationMode();
            Debug.Log(neutral ? "ART01_NEUTRAL_CAPTURE_GREEN" : "ART01_DRESSED_CAPTURE_GREEN (PROVISIONAL built-in)");
        }
    }
}
