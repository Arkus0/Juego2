using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Non-playable backdrop: faceted mountain ring (Picos-like to the north/west), forests, distant roofs.
    public class Backdrop
    {
        readonly TerrainBuild tb; readonly Transform root;
        public Backdrop(TerrainBuild t, Transform r) { tb = t; root = r; }
        static readonly Vector2 C = P(Heights.U0 + Heights.Size * 0.5f, Heights.V0 + Heights.Size * 0.5f);

        float EdgeHeight(Vector2 p)
        {
            var q = new Vector2(Mathf.Clamp(p.x, Heights.U0 + 1, Heights.U0 + Heights.Size - 1), Mathf.Clamp(p.y, Heights.V0 + 1, Heights.V0 + Heights.Size - 1));
            return tb.Ground(q);
        }

        float Mountain(Vector2 p)
        {
            var d = p - P(110, 40);
            float ang = Mathf.Atan2(d.y, d.x);                   // 0 = +U (valley east), pi/2 = +V (north)
            float north = Mathf.Clamp01(Mathf.Sin(ang) * 0.8f + 0.35f);
            float west = Mathf.Clamp01(-Mathf.Cos(ang) * 0.6f);
            float valley = Mathf.Pow(Mathf.Abs(Mathf.Cos(ang)), 6);   // lower along the river axis
            float ridge = 1 - Mathf.Abs(Fbm(p.x * 0.0012f + 3, p.y * 0.0012f, 5) * 2 - 1);
            float amp = Mathf.Lerp(260, 1350, Mathf.Max(north, west * 0.8f)) * (1 - 0.55f * valley);
            return 40 + amp * Mathf.Pow(ridge, 1.6f) * (0.55f + 0.6f * Fbm(p.x * 0.004f, p.y * 0.004f, 3));
        }

        public void BuildMountains()
        {
            var mats = new[] { MatLib.Get("Rock"), Forest(), Meadow() };
            var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>();
            var tris = new[] { new List<int>(), new List<int>(), new List<int>() };
            int rings = 70, segs = 200;
            float r0 = 300, r1 = 4200;
            var grid = new Vector3[rings + 1, segs];
            for (int i = 0; i <= rings; i++)
            {
                float t = i / (float)rings;
                float r = Mathf.Lerp(r0, r1, t * t);
                for (int j = 0; j < segs; j++)
                {
                    float a = j * Mathf.PI * 2 / segs;
                    var p = C + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
                    bool inside = p.x > Heights.U0 + 2 && p.x < Heights.U0 + Heights.Size - 2 && p.y > Heights.V0 + 2 && p.y < Heights.V0 + Heights.Size - 2;
                    float h;
                    if (inside) h = tb.Ground(p) - 6f;
                    else
                    {
                        var q = new Vector2(Mathf.Clamp(p.x, Heights.U0, Heights.U0 + Heights.Size), Mathf.Clamp(p.y, Heights.V0, Heights.V0 + Heights.Size));
                        float out_ = (p - q).magnitude;
                        h = Mathf.Lerp(EdgeHeight(p) - 1.5f, Mountain(p), Smooth(0, 700, out_));
                    }
                    grid[i, j] = W(p, h);
                }
            }
            for (int i = 0; i < rings; i++)
                for (int j = 0; j < segs; j++)
                {
                    int j1 = (j + 1) % segs;
                    Face(grid[i, j], grid[i + 1, j], grid[i + 1, j1]);
                    Face(grid[i, j], grid[i + 1, j1], grid[i, j1]);
                }
            void Face(Vector3 a, Vector3 b, Vector3 c)
            {
                var n = Vector3.Cross(b - a, c - a).normalized;
                if (n.y < 0) { var tmp = b; b = c; c = tmp; n = -n; }
                float h = (a.y + b.y + c.y) / 3;
                float slope = 1 - n.y;
                float nz = Fbm(a.x * 0.003f, a.z * 0.003f, 2);
                int m = (h > 520 + nz * 250 || slope > 0.42f) ? 0 : (h > 120 + nz * 120 || slope > 0.2f) ? 1 : 2;
                int k = verts.Count;
                verts.Add(a); verts.Add(b); verts.Add(c);
                norms.Add(n); norms.Add(n); norms.Add(n);
                uvs.Add(new Vector2(a.x, a.z) / 60f); uvs.Add(new Vector2(b.x, b.z) / 60f); uvs.Add(new Vector2(c.x, c.z) / 60f);
                tris[m].Add(k); tris[m].Add(k + 1); tris[m].Add(k + 2);
            }
            var mesh = new Mesh { name = "Backdrop_Mountains", indexFormat = IndexFormat.UInt32 };
            mesh.SetVertices(verts); mesh.SetNormals(norms); mesh.SetUVs(0, uvs);
            mesh.subMeshCount = 3;
            for (int s = 0; s < 3; s++) mesh.SetTriangles(tris[s], s);
            mesh.RecalculateBounds();
            MeshKit.Meshes.Add(mesh);
            var go = new GameObject("Telon_Montanas");
            go.transform.SetParent(root, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterials = mats;
            mr.shadowCastingMode = ShadowCastingMode.Off;
            go.isStatic = true;
        }

        static Material Forest()
        {
            var m = new Material(MatLib.Lit) { name = "FarForest" };
            m.SetColor("_BaseColor", new Color(0.2f, 0.27f, 0.19f)); m.SetFloat("_Smoothness", 0.05f);
            m.SetTexture("_BaseMap", MatLib.GenTex("grass", 512));
            AssetDatabase.CreateAsset(m, MatLib.GenDir + "/FarForest.mat");
            return m;
        }

        static Material Meadow()
        {
            var m = new Material(MatLib.Lit) { name = "FarMeadow" };
            m.SetColor("_BaseColor", new Color(0.62f, 0.7f, 0.56f)); m.SetFloat("_Smoothness", 0.05f);
            m.SetTexture("_BaseMap", MatLib.GenTex("grass", 512));
            AssetDatabase.CreateAsset(m, MatLib.GenDir + "/FarMeadow.mat");
            return m;
        }

        /// Forest patches and hedgerows as terrain tree instances (outside the seed).
        public void Forests(Terrain terrain)
        {
            var names = new[] { "CommonTree_1", "CommonTree_2", "CommonTree_3", "CommonTree_4", "CommonTree_5", "Pine_1", "Pine_2", "Pine_3", "Pine_4", "Pine_5" };
            var protos = names.Select(n => new TreePrototype { prefab = TreePrefab(n) }).ToArray();
            var td = terrain.terrainData;
            td.treePrototypes = protos;
            var list = new List<TreeInstance>();
            var r = new Rng(4242);
            for (float u = Heights.U0 + 4; u < Heights.U0 + Heights.Size - 4; u += 4.5f)
                for (float v = Heights.V0 + 4; v < Heights.V0 + Heights.Size - 4; v += 4.5f)
                {
                    var p = P(u + r.Range(-2f, 2f), v + r.Range(-2f, 2f));
                    float dTown = Seed.InHard(p) ? -1 : DistPolyEdge(p, Seed.Hard);
                    if (dTown < 18) continue;
                    if (Seed.WaterDistance(p) < 4) continue;
                    float g = tb.Ground(p);
                    float forest = Fbm(p.x * 0.011f + 11, p.y * 0.011f, 4);
                    float hedge = Mathf.Abs(Mathf.Sin(p.x * 0.045f + Mathf.Sin(p.y * 0.02f) * 2)) < 0.035f ? 1 : 0;
                    float slopeBias = Smooth(8, 40, g);
                    float chance = Smooth(0.5f, 0.62f, forest + slopeBias * 0.12f) * 0.85f + hedge * 0.5f;
                    if (!r.Chance(chance)) continue;
                    int proto = g > 38 || (forest > 0.66f && r.Chance(0.5f)) ? r.Range(5, 10) : r.Range(0, 5);
                    float s = r.Range(0.85f, 1.45f);
                    list.Add(new TreeInstance
                    {
                        prototypeIndex = proto,
                        position = new Vector3((p.x - Heights.U0) / Heights.Size, 0, (p.y - Heights.V0) / Heights.Size),
                        heightScale = s, widthScale = s * r.Range(0.9f, 1.1f),
                        rotation = r.Range(0, Mathf.PI * 2),
                        color = Color.Lerp(new Color(0.85f, 0.9f, 0.85f), Color.white, r.Next()), lightmapColor = Color.white,
                    });
                }
            td.SetTreeInstances(list.ToArray(), true);
            terrain.treeBillboardDistance = 5000;
            terrain.treeDistance = 700;
            terrain.treeMaximumFullLODCount = 5000;
            Debug.Log("[Proto] forest trees: " + list.Count);
        }

        /// Prefab variant with a LODGroup so the terrain never builds (broken) billboards; culled when tiny on screen.
        static GameObject TreePrefab(string n)
        {
            string path = $"{MatLib.GenDir}/Tree_{n}.prefab";
            var model = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/ThirdParty/Quaternius/Nature/Models/{n}.fbx");
            var go = new GameObject("Tree_" + n);
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(model, go.transform);
            Kit.Swap(inst);
            var lod = go.AddComponent<LODGroup>();
            lod.SetLODs(new[] { new LOD(0.012f, inst.GetComponentsInChildren<Renderer>()) });
            lod.RecalculateBounds();
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        /// Cheap distant houses along the soft-envelope roads: stone box + dark tile prism. Not enterable, no colliders.
        public void DistantRoofs()
        {
            var mk = new MeshKit { Tile = 2.5f };
            var wall = MatLib.Get("Stone"); var roof = MatLib.Get("TileDark"); var white = MatLib.Get("Plaster");
            var r = new Rng(77);
            var roads = new List<Vector2[]>
            {
                Pts(206, 72, 240, 55, 270, 48, 305, 42, 350, 38, 410, 36),         // Calle Mayor -> Vega
                Pts(240, 55, 236, 90, 233, 130, 231, 168, 230, 200),               // Barrio Alto
                Pts(270, 48, 282, 70, 292, 95, 300, 120, 308, 150),
                Pts(84, 134, 80, 142, 70, 160, 60, 185),                           // Ensanche
                Pts(-10, 150, 40, 158, 100, 165, 160, 170, 220, 175),
                Pts(20, 120, 20, 200), Pts(60, 150, 60, 230), Pts(-40, 130, -40, 210),
                Pts(-35, -70, -60, -66, -90, -72, -120, -80),                      // Puerto across the confluence
                Pts(30, -75, 10, -86, -15, -96, -45, -102),                        // camino sur
                Pts(215, -30, 262, 0, 300, -28, 340, -36),                         // Ribera
            };
            foreach (var road in roads)
            {
                var pts = Resample(road, 1f);
                var cum = Cum(pts);
                float L = cum[cum.Length - 1];
                foreach (int side in new[] { 1, -1 })
                    for (float s = 2; s < L - 4; s += r.Range(8f, 13f))
                    {
                        var p = At(pts, cum, s, out var dir);
                        var n = Perp(dir) * side;
                        var c = p + n * r.Range(5f, 7f);
                        if (Seed.InHard(c) || DistPolyEdge(c, Seed.Hard) < 14 || Seed.WaterDistance(c) < 6 || (c - Seed.OX1).magnitude < 40) continue;
                        float w = r.Range(6f, 10f), d = r.Range(6f, 8f), h = r.Range(5.5f, 8.8f);
                        float g = tb.Ground(c);
                        var fr = W(dir, 0); var sd = W(n, 0);
                        var cc = W(c, g - 1f);
                        mk.Box(r.Chance(0.25f) ? white : wall, cc + Vector3.up * (h + 1f) * 0.5f, fr * w * 0.5f, Vector3.up * (h + 1f) * 0.5f, sd * d * 0.5f);
                        // gable roof prism with eaves
                        Vector3 top = cc + Vector3.up * (h + 1f);
                        float rh = d * 0.38f, ov = 0.5f;
                        Vector3 e0 = top - sd * (d * 0.5f + ov) + Vector3.down * 0.25f, e1 = top + sd * (d * 0.5f + ov) + Vector3.down * 0.25f, ridge = top + Vector3.up * rh;
                        Vector3 A = fr * (w * 0.5f + 0.3f);
                        WallBuild.O(mk, roof, e0 - A, e0 + A, ridge + A, ridge - A, (Vector3.up - sd).normalized);
                        WallBuild.O(mk, roof, e1 - A, ridge - A, ridge + A, e1 + A, (Vector3.up + sd).normalized);
                        foreach (int g2 in new[] { -1, 1 })
                        {
                            var o = fr * (w * 0.5f) * g2;
                            var t0 = top - sd * d * 0.5f + o; var t1 = top + sd * d * 0.5f + o; var t2 = top + Vector3.up * (rh - 0.2f) + o;
                            if (Vector3.Dot(Vector3.Cross(t1 - t0, t2 - t0), fr * g2) >= 0) mk.Tri(wall, t0, t1, t2); else mk.Tri(wall, t0, t2, t1);
                        }
                    }
            }
            var go = mk.Build("Tejados_Lejanos", root, false, true);
        }
    }
}
