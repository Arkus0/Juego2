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
    /// </summary>
    public static class Art01BenchmarkBuilder
    {
        const string Art = "Assets/Arkus/ART";
        const float Storey = 3.122689f; // effective selected Quaternius wall, Unity audit
        const float Floor = 0.30f;
        static Transform root;
        static readonly List<GameObject> Dressing = new List<GameObject>();

        static void Tag(GameObject go, string id, string role, string connection, string collision = "none")
        {
            var tag = go.GetComponent<Art01Piece>() ?? go.AddComponent<Art01Piece>();
            tag.logicalId = id;
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

        static GameObject Source(string id, string category, string name, Transform parent,
            Vector3 localPosition, Quaternion localRotation, Vector3 localScale, string role,
            string connection, string materialOverride = null)
        {
            var path = Art + "/External/" + category + "/Models/" + name + ".fbx";
            if (category == "Characters") path = Art + "/External/Characters/" + name + ".fbx";
            if (category == "DerivedHuman") path = Art + "/Derived/Characters/" + name + ".fbx";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null) throw new Exception("ART01_SOURCE_MISSING " + path);
            var wrapper = Group(id, parent);
            wrapper.transform.localPosition = localPosition;
            wrapper.transform.localRotation = localRotation;
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(asset, wrapper.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = asset.transform.localRotation; // Props root is approx -90° X
            instance.transform.localScale = Vector3.Scale(asset.transform.localScale, localScale); // preserve x100 Props root
            Art01Materials.Remap(instance, materialOverride);
            Tag(wrapper, id, role, connection);
            if (role == "DRESSING" || role == "NATURE") Dressing.Add(wrapper);
            return wrapper;
        }

        static GameObject Solid(string id, Transform parent, Vector3 centre, Vector3 size,
            string material, string role, string connection, bool collide = false)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = id;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = centre;
            go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = Art01Materials.Get(material);
            if (!collide) UnityEngine.Object.DestroyImmediate(go.GetComponent<BoxCollider>());
            Tag(go, id, role, connection, collide ? "sole_local_support" : "none");
            if (role == "DRESSING" || role == "NATURE") Dressing.Add(go);
            return go;
        }

        static Mesh SaveMesh(Mesh mesh, string name)
        {
            var dir = Art + "/Derived";
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder(Art, "Derived");
            if (!AssetDatabase.IsValidFolder(dir + "/Meshes")) AssetDatabase.CreateFolder(dir, "Meshes");
            var path = dir + "/Meshes/" + name + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing == null) { AssetDatabase.CreateAsset(mesh, path); return mesh; }
            EditorUtility.CopySerialized(mesh, existing);
            EditorUtility.SetDirty(existing);
            UnityEngine.Object.DestroyImmediate(mesh);
            return existing;
        }

        static GameObject MeshObject(string id, Mesh mesh, string material, Transform parent,
            string role, string connection, bool collider = false)
        {
            var go = Group(id, parent);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = Art01Materials.Get(material);
            if (collider) go.AddComponent<MeshCollider>().sharedMesh = mesh;
            Tag(go, id, role, connection, collider ? "sole_traversable_owner" : "none");
            return go;
        }

        static Mesh RoadMesh()
        {
            // One continuous crowned surface: no stacked road/ground slabs or duplicate path collider.
            var stations = new[] {
                new Vector2(-36, 5.5f), new Vector2(-26, 5.5f),
                new Vector2(-23, 2.8f), new Vector2(7, 2.8f),
                new Vector2(13, 6.0f), new Vector2(18, 6.0f),
                new Vector2(22, 2.4f), new Vector2(23.84f, 2.4f),
            };
            var verts = new List<Vector3>();
            var uv = new List<Vector2>();
            var triangles = new List<int>();
            foreach (var station in stations)
            {
                float z = station.x, half = station.y * 0.5f;
                verts.Add(new Vector3(-half, 0.01f, z));
                verts.Add(new Vector3(0, 0.055f, z));
                verts.Add(new Vector3(half, 0.01f, z));
                uv.Add(new Vector2(-half * 0.5f, z * 0.5f));
                uv.Add(new Vector2(0, z * 0.5f));
                uv.Add(new Vector2(half * 0.5f, z * 0.5f));
            }
            for (int i = 0; i < stations.Length - 1; i++)
            {
                int a = i * 3, b = (i + 1) * 3;
                triangles.AddRange(new[] { a, b, a + 1, a + 1, b, b + 1,
                                           a + 1, b + 1, a + 2, a + 2, b + 1, b + 2 });
            }
            var mesh = new Mesh { name = "ART01_Road_W12_S02" };
            mesh.SetVertices(verts); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, "art01.street.crowned.s02_w12_casco.v1");
        }

        static Mesh ScenicBankMesh(int side)
        {
            // Only scenic terrain response around a retained road edge. No alternate
            // traversable ownership and no claim about CITY elevation geometry.
            var xs = new[] { 0.04f, 3.2f, 5f, 8f, 13f, 21f, 34f, 48f };
            var zs = new[] { -47f, -38f, -29f, -19f, -8f, 3f, 14f, 24f, 35f, 48f };
            var verts = new List<Vector3>(); var uv = new List<Vector2>();
            var tris = new List<int>();
            for (int j = 0; j < zs.Length; j++)
                for (int i = 0; i < xs.Length; i++)
                {
                    float x = xs[i];
                    float rise = Mathf.Max(0, x - 8f) * 0.062f;
                    float irregular = i < 2 ? 0 : 0.16f * Mathf.Sin(j * 1.63f + i * 0.82f);
                    verts.Add(new Vector3(side * x, -0.08f + rise + irregular, zs[j]));
                    uv.Add(new Vector2(x * 0.34f, zs[j] * 0.34f));
                }
            for (int j = 0; j < zs.Length - 1; j++)
                for (int i = 0; i < xs.Length - 1; i++)
                {
                    int a = j * xs.Length + i, b = a + xs.Length;
                    if (side > 0) tris.AddRange(new[] { a,b,a+1, a+1,b,b+1 });
                    else tris.AddRange(new[] { a,a+1,b, a+1,b+1,b });
                }
            var mesh = new Mesh { name = "art01.site.scenic_bank." + side + ".v1" };
            mesh.SetVertices(verts); mesh.SetUVs(0, uv); mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, mesh.name);
        }

        static void RoadEdges(Transform parent)
        {
            var stations = new[] {
                new Vector2(-36, 5.5f), new Vector2(-26, 5.5f),
                new Vector2(-23, 2.8f), new Vector2(7, 2.8f),
                new Vector2(13, 6f), new Vector2(18, 6f),
                new Vector2(22, 2.4f), new Vector2(23.84f, 2.4f),
            };
            for (int side = -1; side <= 1; side += 2)
                for (int i = 0; i < stations.Length - 1; i++)
                {
                    var a = new Vector3(side * stations[i].y * 0.5f, 0, stations[i].x);
                    var b = new Vector3(side * stations[i + 1].y * 0.5f, 0, stations[i + 1].x);
                    var dir = b - a;
                    var curb = Solid($"art01.kerb.{side}.{i}", parent, (a + b) * 0.5f + Vector3.up * 0.065f,
                        new Vector3(0.18f, 0.13f, dir.magnitude), "StoneTrim", "EDGE",
                        "MEETS road edge; outside sole traversable road collider");
                    curb.transform.localRotation = Quaternion.LookRotation(dir);
                    if (side > 0)
                    {
                        var drain = Solid($"art01.drain.{i}", parent,
                            (a + b) * 0.5f - Vector3.right * 0.17f - Vector3.up * 0.005f,
                            new Vector3(0.12f, 0.012f, dir.magnitude), "StoneShadow", "DRAIN",
                            "DRAINS_TO edge; visual groove below road crown");
                        drain.transform.localRotation = Quaternion.LookRotation(dir);
                    }
                }
        }

        static void StoneSupport(Transform parent)
        {
            // Bridgehead interface specimen and bank contact; no CITY-07 bridge or terrain claim.
            Solid("art01.bridgehead.left.parapet", parent, new Vector3(-3.05f, 0.54f, -31),
                new Vector3(0.55f, 1.08f, 10), "StoneShadow", "RETAINING",
                "SUPPORTED_BY bridge edge; MEETS S02", true);
            Solid("art01.bridgehead.right.parapet", parent, new Vector3(3.05f, 0.54f, -31),
                new Vector3(0.55f, 1.08f, 10), "StoneShadow", "RETAINING",
                "SUPPORTED_BY bridge edge; MEETS S02", true);
            Solid("art01.bridgehead.left.coping", parent, new Vector3(-3.05f, 1.09f, -31),
                new Vector3(0.68f, 0.12f, 10.1f), "StoneTrim", "CAP",
                "CAPS parapet");
            Solid("art01.bridgehead.right.coping", parent, new Vector3(3.05f, 1.09f, -31),
                new Vector3(0.68f, 0.12f, 10.1f), "StoneTrim", "CAP",
                "CAPS parapet");
            Solid("art01.bridgehead.left.bank", parent, new Vector3(-5.7f, -0.75f, -29),
                new Vector3(4.7f, 1.5f, 15), "Stone", "RETAINING",
                "MEETS bank and bridgehead", true);
            Solid("art01.bridgehead.right.bank", parent, new Vector3(5.7f, -0.75f, -29),
                new Vector3(4.7f, 1.5f, 15), "Stone", "RETAINING",
                "MEETS bank and bridgehead", true);
            Solid("art01.water.visual", parent, new Vector3(0, -2.2f, -31),
                new Vector3(70, 0.02f, 20), "Water", "SCENIC",
                "visual water below bridge; no traversable collider");
        }

        static void W12CascoEdge(Transform parent)
        {
            // The right W12 house ends at z=2. Its plinth meets this retained
            // 0.56 m capped bank wall, which ends before the Casco reveal at z=7.
            // The inner wall face is x=1.51, outside the 2.8 m road and kerb;
            // the wall foot reaches the scenic bank datum (-0.08), not a prop mask.
            const float x = 1.72f;
            const float z0 = 1.95f, z1 = 6.75f;
            float length = z1 - z0, centre = (z0 + z1) * 0.5f;
            Solid("art01.w12_casco.right.retaining", parent,
                new Vector3(x, 0.20f, centre), new Vector3(0.42f, 0.56f, length),
                "Stone", "RETAINING",
                "SUPPORTED_BY scenic bank; MEETS W12 right plinth and road shoulder; CLEAR_OF 2.8 m route", true);
            Solid("art01.w12_casco.right.coping", parent,
                new Vector3(x, 0.525f, centre), new Vector3(0.53f, 0.09f, length + 0.05f),
                "StoneTrim", "CAP", "CAPS W12/Casco retaining wall; resolves exposed end");
        }

        static Mesh GableMesh(float width, float rise, float depth, string name)
        {
            float w = width * 0.5f, d = depth * 0.5f;
            var verts = new[] {
                new Vector3(-w, 0, -d), new Vector3(w, 0, -d), new Vector3(0, rise, -d),
                new Vector3(-w, 0, d), new Vector3(w, 0, d), new Vector3(0, rise, d),
            };
            var tris = new[] { 0, 2, 1, 3, 4, 5, 0, 1, 4, 0, 4, 3,
                               1, 2, 5, 1, 5, 4, 2, 0, 3, 2, 3, 5 };
            var mesh = new Mesh { name = name, vertices = verts, triangles = tris,
                uv = new[] { new Vector2(0,0), new Vector2(width,0), new Vector2(width/2,rise),
                             new Vector2(0,0), new Vector2(width,0), new Vector2(width/2,rise) } };
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, name);
        }

        static Mesh RenderHostMesh(string kind)
        {
            // Juego2 derivative: a single 2 m structural render host with a real 0.44 m
            // deep aperture. It uses audited Quaternius insert dimensions, but never
            // paints a flat facade over a solid source wall.
            float left = -1f, right = 1f, bottom = 0f, top = Storey;
            float apertureLeft = -0.805f, apertureRight = 0.805f;
            float apertureBottom = kind == "Door_Flat" ? 0f : 0.94f;
            float apertureTop = kind == "Door_Flat" ? 2.39f : 2.53f;
            var vertices = new List<Vector3>(); var triangles = new List<int>();
            var uv = new List<Vector2>();
            void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            {
                int start = vertices.Count;
                vertices.AddRange(new[] { a, b, c, d });
                uv.AddRange(new[] { new Vector2(0, 0), new Vector2(1, 0),
                    new Vector2(1, 1), new Vector2(0, 1) });
                triangles.AddRange(new[] { start, start + 1, start + 2,
                    start, start + 2, start + 3 });
            }
            void Prism(float x0, float x1, float y0, float y1)
            {
                if (x1 <= x0 || y1 <= y0) return;
                float f = 0.22f, b = -0.22f;
                Quad(new Vector3(x0,y0,f), new Vector3(x1,y0,f), new Vector3(x1,y1,f), new Vector3(x0,y1,f));
                Quad(new Vector3(x1,y0,b), new Vector3(x0,y0,b), new Vector3(x0,y1,b), new Vector3(x1,y1,b));
                Quad(new Vector3(x0,y0,b), new Vector3(x0,y0,f), new Vector3(x0,y1,f), new Vector3(x0,y1,b));
                Quad(new Vector3(x1,y0,f), new Vector3(x1,y0,b), new Vector3(x1,y1,b), new Vector3(x1,y1,f));
                Quad(new Vector3(x0,y1,f), new Vector3(x1,y1,f), new Vector3(x1,y1,b), new Vector3(x0,y1,b));
                Quad(new Vector3(x0,y0,b), new Vector3(x1,y0,b), new Vector3(x1,y0,f), new Vector3(x0,y0,f));
            }
            if (kind == "Straight") Prism(left, right, bottom, top);
            else
            {
                Prism(left, apertureLeft, bottom, top);
                Prism(apertureRight, right, bottom, top);
                Prism(apertureLeft, apertureRight, apertureTop, top);
                Prism(apertureLeft, apertureRight, bottom, apertureBottom);
            }
            var mesh = new Mesh { name = "ART01_RenderHost_" + kind };
            mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, "art01.facade.render_host." + kind.ToLowerInvariant() + ".v1");
        }

        static Mesh LeanToMesh()
        {
            // Retained 3.6 x 1.6 m threshold canopy: continuous tile plane with
            // pitched wall meeting and visible fascia, rather than a horizontal box.
            var v = new[] {
                new Vector3(-1.8f,3.10f,0), new Vector3(1.8f,3.10f,0),
                new Vector3(-1.8f,2.82f,-1.65f), new Vector3(1.8f,2.82f,-1.65f),
                new Vector3(-1.8f,3.03f,0), new Vector3(1.8f,3.03f,0),
                new Vector3(-1.8f,2.75f,-1.65f), new Vector3(1.8f,2.75f,-1.65f),
            };
            var t = new[] { 0,2,1, 1,2,3, 4,5,6, 5,7,6,
                2,6,3, 3,6,7, 0,4,2, 2,4,6, 1,3,5, 3,7,5 };
            var uv = new[] { new Vector2(0,0),new Vector2(3.6f,0),
                new Vector2(0,1.65f),new Vector2(3.6f,1.65f),
                new Vector2(0,0),new Vector2(3.6f,0),
                new Vector2(0,1.65f),new Vector2(3.6f,1.65f) };
            var mesh = new Mesh { name = "art01.roof.threshold.lean_to.v1", vertices = v, triangles = t, uv = uv };
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return SaveMesh(mesh, mesh.name);
        }

        static void Wall(Transform house, int index, float x, float z, float y, Quaternion face,
            string kind, bool publicDoor, bool plaster = false)
        {
            var pos = new Vector3(x, y, z);
            var id = $"art01.wall.{house.name}.{index}.{y:F2}";
            var source = plaster
                ? MeshObject(id, RenderHostMesh(kind), "Plaster", house,
                    "FACADE_HOST", "SUPPORTED_BY plinth; HOSTS real " + kind + " aperture; MEETS adjacent module")
                : Source(id, "Medieval", "Wall_UnevenBrick_" + kind, house, pos, face,
                    Vector3.one, "FACADE_HOST", "SUPPORTED_BY plinth; HOSTS " + kind + "; MEETS adjacent module");
            if (plaster) { source.transform.localPosition = pos; source.transform.localRotation = face; }
            if (kind == "Door_Flat")
            {
                Source(id + ".frame", "Medieval", "DoorFrame_Flat_Brick", house,
                    pos, face, Vector3.one, "HOSTED_INSERT", "FILLS host opening; jamb/lintel return");
                if (!publicDoor)
                    Source(id + ".leaf", "Medieval", "Door_3_Flat", house,
                        pos + face * (Vector3.right * 0.5f), face,
                        Vector3.one, "HOSTED_INSERT", "FILLS private/closed opening");
                else
                {
                    var open = Source(id + ".open_leaf", "Medieval", "Door_3_Flat", house,
                        pos + face * (Vector3.right * 0.5f), face * Quaternion.Euler(0, 85, 0),
                        Vector3.one, "HOSTED_INSERT", "open leaf keeps public clear width");
                    // The source mesh has no traversal collider; jambs below own the physical cut.
                }
                var coll = source.AddComponent<BoxCollider>();
                coll.size = new Vector3(0.42f, Storey, 0.41f);
                coll.center = new Vector3(-0.79f, Storey * 0.5f, -0.11f);
                var coll2 = source.AddComponent<BoxCollider>();
                coll2.size = new Vector3(0.42f, Storey, 0.41f);
                coll2.center = new Vector3(0.79f, Storey * 0.5f, -0.11f);
                var lintel = source.AddComponent<BoxCollider>();
                lintel.size = new Vector3(1.16f, 0.95f, 0.41f);
                lintel.center = new Vector3(0, 2.65f, -0.11f);
            }
            else if (kind == "Window_Wide_Flat")
            {
                foreach (float side in new[] { -0.9f, 0.9f })
                {
                    var jamb = source.AddComponent<BoxCollider>();
                    jamb.size = new Vector3(0.2f, Storey, 0.44f);
                    jamb.center = new Vector3(side, Storey * 0.5f, 0);
                }
                var sill = source.AddComponent<BoxCollider>();
                sill.size = new Vector3(1.61f, 0.94f, 0.44f);
                sill.center = new Vector3(0, 0.47f, 0);
                var windowLintel = source.AddComponent<BoxCollider>();
                windowLintel.size = new Vector3(1.61f, Storey - 2.53f, 0.44f);
                windowLintel.center = new Vector3(0, (Storey + 2.53f) * 0.5f, 0);
            }
            else
            {
                var coll = source.AddComponent<BoxCollider>();
                coll.size = new Vector3(2, Storey, 0.41f);
                coll.center = new Vector3(0, Storey * 0.5f, -0.11f);
            }
            if (kind == "Window_Wide_Flat")
            {
                Source(id + ".window", "Medieval", "Window_Wide_Flat1", house, pos, face,
                    Vector3.one, "HOSTED_INSERT", "FILLS real hosted opening");
                Source(id + ".shutters", "Medieval", "WindowShutters_Wide_Flat_Open", house,
                    pos, face, Vector3.one, "HOSTED_INSERT", "ATTACHED_TO opening frame", "ShutterGreen");
            }
        }

        static void House(string id, Vector3 centre, float yaw, int width, int depth, bool bar,
            bool renderFinish = false)
        {
            var house = Group(id, root);
            house.transform.localPosition = centre;
            house.transform.localRotation = Quaternion.Euler(0, yaw, 0);
            Tag(house, "art01.assembly." + id, "BUILDING_ASSEMBLY",
                "site->plinth->facade hosts->openings->roof->threshold->street");
            int nx = width / 2, nz = depth / 2;
            float front = -depth * 0.5f, back = depth * 0.5f;
            int door = bar ? 1 : Mathf.Clamp(nx / 2, 0, nx - 1);
            for (int floor = 0; floor < 2; floor++)
            {
                float y = Floor + floor * Storey;
                for (int i = 0; i < nx; i++)
                {
                    float x = -width * 0.5f + 1 + 2 * i;
                    string frontKind = floor == 0 && i == door ? "Door_Flat" :
                        (floor == 1
                            ? (i % 2 == 0 || bar && i == nx - 1 ? "Window_Wide_Flat" : "Straight")
                            : (i != nx - 1 ? "Window_Wide_Flat" : "Straight"));
                    Wall(house.transform, i, x, front, y, Quaternion.Euler(0, 180, 0),
                        frontKind, bar && floor == 0 && i == door, renderFinish || floor == 1);
                    Wall(house.transform, i + 100, x, back, y, Quaternion.identity,
                        i % 2 == 0 && floor == 1 ? "Window_Wide_Flat" : "Straight", false,
                        renderFinish || floor == 1);
                }
                for (int i = 0; i < nz; i++)
                {
                    float z = -depth * 0.5f + 1 + 2 * i;
                    Wall(house.transform, i + 200, -width * 0.5f, z, y,
                        Quaternion.Euler(0, 270, 0),
                        i == 1 && floor == 1 ? "Window_Wide_Flat" : "Straight", false,
                        renderFinish || floor == 1);
                    Wall(house.transform, i + 300, width * 0.5f, z, y,
                        Quaternion.Euler(0, 90, 0),
                        i == nz - 2 && floor == 1 ? "Window_Wide_Flat" : "Straight", false,
                        renderFinish || floor == 1);
                }
                if (floor == 0)
                    for (int sx = -1; sx <= 1; sx += 2)
                        for (int sz = -1; sz <= 1; sz += 2)
                            Source($"art01.corner.{id}.{floor}.{sx}.{sz}", "Medieval",
                                "Corner_Exterior_Brick", house.transform,
                                new Vector3(sx * width * 0.5f, y, sz * depth * 0.5f),
                                Quaternion.Euler(0, sx == sz ? 0 : 90, 0), Vector3.one,
                                "CORNER", "MEETS two stone facade hosts; SUPPORTED_BY plinth");
            }
            // Plinth follows the 0.25 m floor raise. Its public opening is deliberately cut.
            for (int i = 0; i < nx; i++) if (i != door)
            {
                float x = -width * 0.5f + 1 + 2 * i;
                Solid($"art01.plinth.{id}.front.{i}", house.transform,
                    new Vector3(x, 0.15f, front - 0.08f), new Vector3(2.02f, 0.3f, 0.52f),
                    "StoneShadow", "PLINTH", "SUPPORTED_BY site; MEETS street edge");
            }
            Solid($"art01.plinth.{id}.left", house.transform,
                new Vector3(-width * 0.5f, 0.15f, 0), new Vector3(0.52f, 0.3f, depth),
                "StoneShadow", "PLINTH", "SUPPORTED_BY site; MEETS wall end");
            Solid($"art01.plinth.{id}.right", house.transform,
                new Vector3(width * 0.5f, 0.15f, 0), new Vector3(0.52f, 0.3f, depth),
                "StoneShadow", "PLINTH", "SUPPORTED_BY site; MEETS wall end");
            Solid($"art01.plinth.{id}.back", house.transform,
                new Vector3(0, 0.15f, back), new Vector3(width, 0.3f, 0.52f),
                "StoneShadow", "PLINTH", "SUPPORTED_BY site; MEETS wall end");

            float eave = Floor + 2 * Storey;
            string roofName = width == 8 ? "Roof_RoundTiles_8x14" : "Roof_RoundTiles_6x10";
            Vector3 roofScale = width == 8 ? new Vector3(0.91f, 0.45f, 0.96f) :
                new Vector3(0.86f, 0.45f, 0.94f);
            // Normalized source roof has minY < 0. Its own scaled underside rests at wall top.
            float minSourceY = width == 8 ? -0.780f : -0.65f;
            float roofPivot = eave - minSourceY * roofScale.y;
            var roof = Source($"art01.roof.lowpitch.{width}x{depth}.{id}", "Medieval", roofName,
                house.transform, new Vector3(0, roofPivot, 0), Quaternion.identity, roofScale,
                "ROOF_DERIVED", "CAPS facade perimeter; MEETS gables/eaves; 0.45 pitch scale named derivative");
            // Closed gables extend into the supported roof by 0.25 m; no independent Y guess.
            float roofRise = (width == 8 ? 6.7813f : 5.6722f) * roofScale.y - 0.25f;
            var gableMesh = GableMesh(width, roofRise, 0.40f, $"art01.gable.{width}.v1");
            var frontGable = MeshObject($"art01.gable.{id}.front", gableMesh,
                renderFinish ? "Plaster" : "Stone", house.transform,
                "GABLE", "MEETS facade top; CAPS under roof");
            frontGable.transform.localPosition = new Vector3(0, eave, front + 0.15f);
            var backGable = MeshObject($"art01.gable.{id}.back", gableMesh,
                "Stone", house.transform, "GABLE", "MEETS facade top; CAPS under roof");
            backGable.transform.localPosition = new Vector3(0, eave, back - 0.15f);
            for (int sx = -1; sx <= 1; sx += 2)
                Solid($"art01.eave.{id}.{sx}", house.transform,
                    new Vector3(sx * (width * 0.5f + 0.25f), eave - 0.06f, 0),
                    new Vector3(0.14f, 0.12f, depth + 0.9f), "WoodDark", "EAVE",
                    "SUPPORTED_BY wall top; MEETS roof underside");

            if (bar)
            {
                float dx = -width * 0.5f + 1 + 2 * door;
                var canopy = MeshObject("art01.f01.roof.threshold.lean_to", LeanToMesh(), "TileWet",
                    house.transform, "PORCH_ROOF", "MEETS facade under first floor; SUPPORTED_BY two timber posts");
                canopy.transform.localPosition = new Vector3(dx, 0, front - 0.08f);
                for (int side = -1; side <= 1; side += 2)
                {
                    Solid($"art01.f01.porch.post.{side}", house.transform,
                        new Vector3(dx + side * 1.65f, 1.43f, front - 1.48f),
                        new Vector3(0.17f, 2.65f, 0.17f), "WoodDark", "PORCH_POST",
                        "SUPPORTED_BY threshold edge; SUPPORTS lean-to roof", true);
                    Solid($"art01.f01.porch.post_foot.{side}", house.transform,
                        new Vector3(dx + side * 1.65f, 0.16f, front - 1.48f),
                        new Vector3(0.32f, 0.32f, 0.32f), "StoneTrim", "PLINTH",
                        "MEETS post and ground", true);
                }
                Solid("art01.f01.porch.fascia", house.transform,
                    new Vector3(dx, 2.79f, front - 1.68f),
                    new Vector3(3.65f, 0.16f, 0.11f), "WoodDark", "EAVE",
                    "CAPS threshold roof edge");
                // ART-owned legible two-sided insert, not a flat building facade.
                Solid("art01.f01.sign.board", house.transform,
                    new Vector3(2.55f, 2.59f, front - 0.37f),
                    new Vector3(1.55f, 0.48f, 0.10f), "WoodDark", "SIGN",
                    "ATTACHED_TO facade host; clear of public opening");
                foreach (int signSide in new[] { -1, 1 })
                {
                    var lettering = Group("art01.f01.sign.lettering." + signSide, house.transform);
                    lettering.transform.localPosition = new Vector3(2.55f, 2.59f,
                        front - 0.37f + signSide * 0.064f);
                    lettering.transform.localRotation = Quaternion.Euler(0, signSide < 0 ? 180 : 0, 0);
                    var text = lettering.AddComponent<TextMesh>();
                    text.text = "BAR"; text.fontSize = 60; text.characterSize = 0.016f;
                    text.anchor = TextAnchor.MiddleCenter; text.alignment = TextAlignment.Center;
                    text.color = new Color(0.83f, 0.77f, 0.58f);
                    Tag(lettering, "art01.f01.sign.lettering." + signSide, "SIGN_INSERT",
                        "FACES both sides; ATTACHED_TO sign board");
                }
                Solid("art01.f01.interior.floor", house.transform,
                    new Vector3(0, Floor - 0.065f, 0),
                    new Vector3(width, 0.13f, depth), "WoodDark", "INTERIOR_FLOOR",
                    "TRANSITIONS_TO public threshold", true);
                Solid("art01.f01.interior.ceiling", house.transform,
                    new Vector3(0, Floor + Storey - 0.1f, 0),
                    new Vector3(width, 0.2f, depth), "WoodDark", "INTERIOR_CEILING",
                    "SUPPORTED_BY facades; CAPS public room");
                Solid("art01.f01.threshold.landing", house.transform,
                    new Vector3(dx, Floor - 0.055f, front - 0.48f),
                    new Vector3(1.2f, 0.11f, 0.96f), "StoneTrim", "THRESHOLD",
                    "TRANSITIONS_TO street and interior floor", true);
                Solid("art01.f01.threshold.step", house.transform,
                    new Vector3(dx, 0.075f, front - 1.01f),
                    new Vector3(1.45f, 0.15f, 0.3f), "StoneTrim", "THRESHOLD",
                    "TRANSITIONS_TO landing; 0.15 m rise", true);
                Source("art01.f01.table", "Props", "Table_Large", house.transform,
                    new Vector3(1.4f, Floor, front + 3.8f), Quaternion.identity,
                    Vector3.one * 0.75f, "DRESSING", "SUPPORTED_BY interior floor");
                Source("art01.f01.stool", "Props", "Stool", house.transform,
                    new Vector3(1.0f, Floor, front + 2.7f), Quaternion.identity,
                    Vector3.one, "DRESSING", "SUPPORTED_BY interior floor");
                Source("art01.f01.chair", "Props", "Chair_1", house.transform,
                    new Vector3(2.2f, Floor, front + 4.8f), Quaternion.Euler(0, 180, 0),
                    Vector3.one, "DRESSING", "SUPPORTED_BY interior floor");
                Solid("art01.f01.counter.top", house.transform,
                    new Vector3(2.2f, Floor + 0.96f, front + 5.1f),
                    new Vector3(0.92f, 0.12f, 3.8f), "WoodDark", "INTERIOR_FURNITURE",
                    "SUPPORTED_BY counter carcass; public serving edge");
                Solid("art01.f01.counter.front", house.transform,
                    new Vector3(1.82f, Floor + 0.48f, front + 5.1f),
                    new Vector3(0.12f, 0.86f, 3.6f), "PropWood", "INTERIOR_FURNITURE",
                    "SUPPORTED_BY floor; SUPPORTS counter top");
                for (int slat = 0; slat < 5; slat++)
                    Solid($"art01.f01.counter.stile.{slat}", house.transform,
                        new Vector3(1.735f, Floor + 0.48f, front + 3.45f + slat * 0.8f),
                        new Vector3(0.045f, 0.84f, 0.06f), "WoodDark", "INTERIOR_FURNITURE",
                        "ATTACHED_TO counter carcass");
                for (int seat = 0; seat < 2; seat++)
                    Source($"art01.f01.counter.stool.{seat}", "Props", "Stool", house.transform,
                        new Vector3(0.82f, Floor, front + 4.1f + seat * 1.5f),
                        Quaternion.Euler(0, 90, 0), Vector3.one,
                        "DRESSING", "SUPPORTED_BY public room floor; clear of threshold");
                Source("art01.f01.counter.mug", "Props", "Mug", house.transform,
                    new Vector3(2.05f, Floor + 1.02f, front + 4.2f), Quaternion.identity,
                    Vector3.one, "DRESSING", "SUPPORTED_BY counter top");
                var point = Group("art01.f01.warm.interiormood", house.transform).AddComponent<Light>();
                point.type = LightType.Point; point.color = new Color(1f, 0.69f, 0.38f);
                point.intensity = 1.25f; point.range = 7f;
                point.transform.localPosition = new Vector3(0, 2.6f, front + 3.3f);
            }
        }

        static void NatureAndProps()
        {
            var humanF01 = Source("art01.human.forastero.f01.scale", "DerivedHuman", "Townsfolk_Forastero", root,
                new Vector3(1.75f, 0.06f, 20.4f), Quaternion.identity, Vector3.one,
                "SCALE_REFERENCE", "standing on accepted 2.4 m pedestrian strip; no NPC behavior");
            var humanW12 = Source("art01.human.forastero.w12.scale", "DerivedHuman", "Townsfolk_Forastero", root,
                new Vector3(-0.42f, 0.06f, 7.5f), Quaternion.Euler(0, 180, 0), Vector3.one,
                "SCALE_REFERENCE", "standing on road; visual scale only, no AI or schedule");
            ConfigureScaleHuman(humanF01);
            ConfigureScaleHuman(humanW12);
            Source("art01.nature.tree.s02", "Nature", "CommonTree_1", root,
                new Vector3(8, -0.1f, -23), Quaternion.Euler(0, 15, 0), Vector3.one * 0.62f,
                "NATURE", "ROOTED_IN bank soil; CLEAR_OF X1/S02 path");
            foreach (var site in new[] {
                new Vector3(-15f, 0.3f, -18f), new Vector3(15f, 0.3f, -12f),
                new Vector3(-18f, 0.4f, 18f), new Vector3(17f, 0.4f, 35f) })
                Source($"art01.nature.tree.bank.{site.x}.{site.z}", "Nature", "CommonTree_1", root,
                    site, Quaternion.Euler(0, site.z * 11f, 0), Vector3.one * 0.76f,
                    "NATURE", "ROOTED_IN scenic bank; no traversal obstruction");
            Source("art01.nature.bush.casco", "Nature", "Bush_Common_Flowers", root,
                new Vector3(-5.3f, 0, 12.5f), Quaternion.identity, Vector3.one * 0.55f,
                "NATURE", "ROOTED_IN edge; CLEAR_OF public route");
            Source("art01.nature.fern.w12", "Nature", "Fern_1", root,
                new Vector3(-4.2f, -0.08f, -2), Quaternion.identity, Vector3.one * 0.12f,
                "NATURE", "ROOTED_IN wall edge; CLEAR_OF route");
            Source("art01.nature.rock.s02", "Nature", "Rock_Medium_1", root,
                new Vector3(-8.5f, -0.65f, -25), Quaternion.Euler(0, 35, 0), Vector3.one * 0.6f,
                "NATURE", "SUPPORTED_BY bank; CLEAR_OF path");
            Source("art01.prop.bench.s02", "Props", "Bench", root,
                new Vector3(-4.9f, 0.0f, -20), Quaternion.Euler(0, 90, 0), Vector3.one * 0.73f,
                "DRESSING", "SUPPORTED_BY bridgehead pocket; CLEAR_OF W12");
            Source("art01.prop.barrel.f01", "Props", "Barrel", root,
                new Vector3(3.2f, 0.0f, 23.2f), Quaternion.identity, Vector3.one * 0.84f,
                "DRESSING", "SUPPORTED_BY street edge; CLEAR_OF threshold");
            Source("art01.prop.crate.f01", "Props", "Crate_Wooden", root,
                new Vector3(-3.2f, 0.0f, 23.5f), Quaternion.Euler(0, 15, 0), Vector3.one * 0.7f,
                "DRESSING", "SUPPORTED_BY street edge; CLEAR_OF threshold");
            Source("art01.prop.lamp.f01", "Props", "Lantern_Wall", root,
                new Vector3(-3.2f, 2.25f, 25.05f), Quaternion.Euler(0, 180, 0),
                Vector3.one * 0.45f, "DRESSING", "ATTACHED_TO facade host; faces public approach");
            for (int side = -1; side <= 1; side += 2)
            {
                Solid($"art01.f01.garden.retaining.{side}", root,
                    new Vector3(side * 4.15f, 0.28f, 20.4f),
                    new Vector3(0.42f, 0.56f, 8.2f), "Stone", "RETAINING",
                    "SUPPORTED_BY ground; MEETS property shoulder", true);
                Solid($"art01.f01.garden.coping.{side}", root,
                    new Vector3(side * 4.15f, 0.6f, 20.4f),
                    new Vector3(0.53f, 0.09f, 8.3f), "StoneTrim", "CAP",
                    "CAPS retaining garden wall");
                Source($"art01.f01.garden.bush.{side}", "Nature", "Bush_Common", root,
                    new Vector3(side * 5.2f, 0, 19.5f), Quaternion.Euler(0, side * 25, 0),
                    Vector3.one * 0.5f, "NATURE", "ROOTED_IN retained garden edge");
                Source($"art01.f01.garden.fern.{side}", "Nature", "Fern_1", root,
                    new Vector3(side * 5.1f, 0, 23.2f), Quaternion.identity,
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
            rig.transform.position = new Vector3(0, 0.08f, -33);
            Tag(rig, "art01.inspector.rig", "INSPECTION_ONLY",
                "uses sole authored road/floor collider; no game or CITY-07 identity");
            var body = rig.AddComponent<CharacterController>();
            body.radius = 0.27f; body.height = 1.8f; body.center = new Vector3(0, 0.9f, 0);
            body.stepOffset = 0.25f; body.slopeLimit = 40f;
            var visual = Source("art01.inspector.clothed_scale", "DerivedHuman", "Townsfolk_Forastero",
                rig.transform, Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one,
                "SCALE_REFERENCE", "1.8 m clothed human; visual inspection only");
            ConfigureScaleHuman(visual);
            var camera = CameraAt("ART01_THIRD_PERSON_CAMERA", new Vector3(0, 1.88f, -36.1f),
                new Vector3(0, 1.43f, -33));
            camera.gameObject.tag = "MainCamera";
            camera.gameObject.AddComponent<AudioListener>();
            rig.AddComponent<Art01WalkInspector>().view = camera;
        }

        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            root = Group("ART01_BENCHMARK_ONLY_NOT_CITY07_KEEPER", null).transform;
            Tag(root.gameObject, "art01.benchmark.v1", "BENCHMARK",
                "local specimen order X1/S02 -> W12 -> Casco/micro.B -> F01; not CITY coordinates");
            var road = MeshObject("art01.street.s02_w12_casco_microB", RoadMesh(), "Cobble", root,
                "STREET", "SUPPORTED_BY site; TRANSITIONS_TO F01 threshold", true);
            RoadEdges(root);
            StoneSupport(root);
            // Scenic ground is deliberately non-collidable. Only road/floor/threshold owns travel.
            MeshObject("art01.site.left.scenic", ScenicBankMesh(-1), "GrassGround", root,
                "SCENIC", "terrain visual below route; no traversable collision");
            MeshObject("art01.site.right.scenic", ScenicBankMesh(1), "GrassGround", root,
                "SCENIC", "terrain visual below route; no traversable collision");
            House("S02_stone_house", new Vector3(-6.4f, 0, -19), -90, 6, 10, false);
            House("W12_left_house", new Vector3(-6.4f, 0, -5), -90, 6, 10, false);
            House("W12_right_house", new Vector3(6.4f, 0, -1), 90, 6, 10, false);
            W12CascoEdge(root);
            House("F01_bar_exterior_threshold", new Vector3(0, 0, 32), 0, 8, 14, true);
            NatureAndProps();
            Lighting();
            WalkInspector();
            var scenePath = Art + "/Art01Benchmark.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("ART01_BENCHMARK_BUILD_GREEN scene=" + scenePath +
                " roadCollision=" + road.GetComponents<MeshCollider>().Length + " dressing=" + Dressing.Count);
        }

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

        public static void CaptureNeutral() => Capture(true);
        public static void CaptureDressed() => Capture(false);
        public static void CaptureThirdPerson()
        {
            EditorSceneManager.OpenScene(Art + "/Art01Benchmark.unity", OpenSceneMode.Single);
            var inspector = UnityEngine.Object.FindFirstObjectByType<Art01WalkInspector>();
            if (inspector == null || inspector.view == null)
                throw new Exception("ART01_THIRD_PERSON_INSPECTOR_MISSING");
            var views = new[] { ("puente_s02", -33f), ("w12_casco", -8f),
                ("w12_casco_edge", 3f), ("f01_threshold", 19f) };
            var outDir = Path.GetFullPath(Path.Combine(Application.dataPath,
                "../../../Docs/evidence/WP-ART-01/captures"));
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
                inspector.transform.position = new Vector3(0, 0.08f, entry.Item2);
                inspector.transform.rotation = Quaternion.identity;
                var animator = inspector.GetComponentInChildren<Animator>(true);
                AnimationMode.SampleAnimationClip(animator.gameObject, IdleClip(), 0.25f);
                var camera = inspector.view;
                camera.transform.position = inspector.transform.TransformPoint(new Vector3(0, 1.8f, -3.1f));
                camera.transform.LookAt(inspector.transform.position + Vector3.up * 1.35f);
                Image(camera, Path.Combine(outDir, "third_person_" + entry.Item1 + ".png"));
            }
            AnimationMode.StopAnimationMode();
            Debug.Log("ART01_THIRD_PERSON_CAPTURE_GREEN 4 viewpoints clothed scale reference");
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
                    map[renderer] = renderer.sharedMaterials;
                    var temp = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < temp.Length; i++) temp[i] = plain;
                    renderer.sharedMaterials = temp;
                }
            }
            var captures = new[] {
                ("puente_s02", new Vector3(0, 1.65f, -35), new Vector3(0, 2.1f, -16)),
                ("w12_casco", new Vector3(0, 1.65f, -10), new Vector3(0, 2.2f, 13)),
                ("w12_casco_joint", new Vector3(0.15f, 1.65f, 5.1f), new Vector3(1.65f, 0.65f, 1.75f)),
                ("w12_casco_edge", new Vector3(0, 1.65f, 2.5f), new Vector3(1.75f, 0.75f, 6.5f)),
                ("f01_exterior", new Vector3(0, 1.65f, 12), new Vector3(-1, 3.0f, 28)),
                ("f01_threshold", new Vector3(-1, 1.65f, 23), new Vector3(-1, 1.5f, 29)),
                ("f01_interior_scale", new Vector3(-1, 1.65f, 25.7f), new Vector3(1.5f, 1.5f, 31)),
            };
            var outDir = Path.GetFullPath(Path.Combine(Application.dataPath,
                "../../../Docs/evidence/WP-ART-01/captures"));
            foreach (var shot in captures)
            {
                var camera = CameraAt(shot.Item1, shot.Item2, shot.Item3);
                Image(camera, Path.Combine(outDir,
                    (neutral ? "neutral_" : "dressed_") + shot.Item1 + ".png"));
                UnityEngine.Object.DestroyImmediate(camera.gameObject);
            }
            foreach (var entry in map) entry.Key.sharedMaterials = entry.Value;
            foreach (var go in hidden) go.SetActive(true);
            if (plain != null) UnityEngine.Object.DestroyImmediate(plain);
            AnimationMode.StopAnimationMode();
            Debug.Log(neutral ? "ART01_NEUTRAL_CAPTURE_GREEN" : "ART01_DRESSED_CAPTURE_GREEN");
        }
    }
}
