using System.Linq;
using UnityEditor;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Layer 0-2: Unity Terrain (relief + rasantes) with exact street/platform stamps, river holes and splat.
    public class TerrainBuild
    {
        public const int Res = 2049;
        public const int Alpha = 2048;
        const float Step = Heights.Size / (Res - 1);
        public float[,] Hm;          // metres, [x=i(u), y=j(v)]
        public bool[,] Hole;         // [x, y] on (Res-1)^2 cells
        public Terrain Terrain;
        readonly Heights heights;
        readonly Layout lay;

        public TerrainBuild(Heights h, Layout l) { heights = h; lay = l; }

        static Vector2 SampleP(int i, int j) => P(Heights.U0 + i * Step, Heights.V0 + j * Step);

        public float Ground(Vector2 p)
        {
            float x = Mathf.Clamp((p.x - Heights.U0) / Step, 0, Res - 1.001f), y = Mathf.Clamp((p.y - Heights.V0) / Step, 0, Res - 1.001f);
            int i = (int)x, j = (int)y;
            float fx = x - i, fy = y - j;
            // match Unity's terrain triangulation closely enough for placement
            return Mathf.Lerp(Mathf.Lerp(Hm[i, j], Hm[i + 1, j], fx), Mathf.Lerp(Hm[i, j + 1], Hm[i + 1, j + 1], fx), fy);
        }

        void ForSamples(Rect b, System.Action<int, int, Vector2> f)
        {
            int i0 = Mathf.Max(0, Mathf.FloorToInt((b.xMin - Heights.U0) / Step)), i1 = Mathf.Min(Res - 1, Mathf.CeilToInt((b.xMax - Heights.U0) / Step));
            int j0 = Mathf.Max(0, Mathf.FloorToInt((b.yMin - Heights.V0) / Step)), j1 = Mathf.Min(Res - 1, Mathf.CeilToInt((b.yMax - Heights.V0) / Step));
            for (int i = i0; i <= i1; i++) for (int j = j0; j <= j1; j++) f(i, j, SampleP(i, j));
        }

        public void ComputeHeights()
        {
            Hm = new float[Res, Res];
            for (int i = 0; i < Res; i++)
                for (int j = 0; j < Res; j++)
                {
                    var p = SampleP(i, j);
                    Hm[i, j] = heights.Sample(p.x, p.y);
                }
            // exact rasantes (routes), then platforms on top
            foreach (var r in Seed.Routes)
                ForSamples(Bounds(r.Pts, r.Width * 0.5f + 1f), (i, j, p) =>
                {
                    float d = r.Query(p, out float rh, out _, out _);
                    if (d <= r.Width * 0.5f + 0.5f) Hm[i, j] = rh;
                });
            foreach (var pl in Seed.Platforms)
                ForSamples(Bounds(pl.Poly, 0.6f), (i, j, p) => { if (InPoly(p, pl.Poly) || DistPolyEdge(p, pl.Poly) < 0.35f) Hm[i, j] = pl.H; });
            foreach (var g in lay.Gardens)
                ForSamples(Bounds(g.Poly, 0.2f), (i, j, p) => { if (InPoly(p, g.Poly)) Hm[i, j] = g.H; });
            // house plots sit slightly below their floor (hidden, keeps plinths honest)
            foreach (var h in lay.Houses)
                ForSamples(Bounds(h.Foot, 0.2f), (i, j, p) => { if (InPoly(p, h.Foot)) Hm[i, j] = Mathf.Min(Hm[i, j], h.FL - 0.25f); });

            // encounter stamps: ground lowered under built surfaces (bridge, ford steps, slipway) so nothing is coplanar
            ForSamples(Bounds(new[] { Bridge.At(0, -Bridge.HalfW), Bridge.At(0, Bridge.HalfW), Bridge.At(Bridge.SEnd, -Bridge.HalfW), Bridge.At(Bridge.SEnd, Bridge.HalfW) }, 0.5f), (i, j, p) =>
            {
                if (Bridge.InFootprint(p, 0f, out float s) && s > 0.05f) Hm[i, j] = Mathf.Min(Hm[i, j], Bridge.DeckSmooth(s) - 0.7f);
            });
            foreach (var st in Stamps.All)
                ForSamples(Bounds(st.poly, 0.3f), (i, j, p) => { if (InPoly(p, st.poly)) Hm[i, j] = Mathf.Min(Hm[i, j], st.h(p)); });

            // river masks inside the hard seed: holes + land height carried up to the exact edge
            Hole = new bool[Res - 1, Res - 1];
            ForSamples(Bounds(Seed.Rio.Concat(Seed.Arroyo).ToArray(), 1f), (i, j, p) =>
            {
                if (!Seed.InHard(p) || !Seed.InMask(p)) return;
                float dr = InPoly(p, Seed.Rio) ? DistPolyEdge(p, Seed.Rio) : 99, da = InPoly(p, Seed.Arroyo) ? DistPolyEdge(p, Seed.Arroyo) : 99;
                float d = Mathf.Min(dr, da);
                bool corridor = DistSeg(p, Stamps.FordA, Stamps.FordB, out _) < 2.4f
                    || DistSeg(p, Stamps.SlipTop, Stamps.SlipTop + Stamps.SlipDir * (Stamps.SlipLen + 1), out _) < 3.4f;
                if (d < 1.0f && !corridor)
                {
                    // project outward to land and copy that height
                    var poly = dr < da ? Seed.Rio : Seed.Arroyo;
                    Vector2 e = ClosestOnEdge(p, poly);
                    var q = e + (e - p).normalized * 1.2f;
                    Hm[i, j] = heights.Exact(q, out _);
                }
                else Hm[i, j] = Heights.Bed;
            });
            for (int i = 0; i < Res - 1; i++)
                for (int j = 0; j < Res - 1; j++)
                {
                    var c = SampleP(i, j) + Vector2.one * (Step * 0.5f);
                    if (c.x < -25 || c.x > 215 || c.y < -65 || c.y > 145) continue;
                    if (!Seed.InHard(c) || !Seed.InMask(c)) continue;
                    float d = Mathf.Min(InPoly(c, Seed.Rio) ? DistPolyEdge(c, Seed.Rio) : 99, InPoly(c, Seed.Arroyo) ? DistPolyEdge(c, Seed.Arroyo) : 99);
                    if (d > 0.08f) Hole[i, j] = true;
                }
        }

        public static Vector2 ClosestOnEdge(Vector2 p, Vector2[] poly)
        {
            float best = float.MaxValue; Vector2 bp = p;
            for (int k = 0; k < poly.Length; k++)
            {
                Vector2 a = poly[k], b = poly[(k + 1) % poly.Length];
                float d = DistSeg(p, a, b, out float t);
                if (d < best) { best = d; bp = Vector2.Lerp(a, b, t); }
            }
            return bp;
        }

        public Terrain BuildTerrain(Transform parent)
        {
            MatLib.EnsureDirs();
            var td = new TerrainData { heightmapResolution = Res };
            td.size = new Vector3(Heights.Size, Heights.Range, Heights.Size);
            AssetDatabase.CreateAsset(td, MatLib.GenDir + "/Terrain.asset");
            var arr = new float[Res, Res];
            for (int i = 0; i < Res; i++)
                for (int j = 0; j < Res; j++)
                    arr[j, i] = Mathf.Clamp01((Hm[i, j] - Heights.Base) / Heights.Range);
            td.SetHeights(0, 0, arr);

            var holes = new bool[Res - 1, Res - 1];
            for (int i = 0; i < Res - 1; i++) for (int j = 0; j < Res - 1; j++) holes[j, i] = !Hole[i, j];
            td.SetHoles(0, 0, holes);

            // layers
            var grass = MatLib.Layer("Grass", MatLib.GenTex("grass", 512), null, 5f, 0.12f, Color.white);
            var dirt = MatLib.Layer("Dirt", MatLib.GenTex("dirt", 512), null, 4f, 0.2f, Color.white);
            var cobble = MatLib.Layer("Cobble", MatLib.Tex("Assets/ThirdParty/Quaternius/MedievalVillage/Materials/T_RoundRocks_BaseColor.png"),
                MatLib.NormalTex("Assets/ThirdParty/Quaternius/MedievalVillage/Materials/T_RoundRocks_Normal.png"), 2.2f, 0.42f, new Color(0.82f, 0.82f, 0.82f));
            var flag = MatLib.Layer("Flag", MatLib.Tex("Assets/ThirdParty/Quaternius/MedievalVillage/Materials/T_UnevenBrick_BaseColor.png"),
                MatLib.NormalTex("Assets/ThirdParty/Quaternius/MedievalVillage/Materials/T_UnevenBrick_Normal.png"), 3.0f, 0.36f, new Color(0.85f, 0.84f, 0.82f));
            var rock = MatLib.Layer("Rock", MatLib.GenTex("rock", 512), null, 12f, 0.18f, Color.white);
            var gravel = MatLib.Layer("Gravel", MatLib.GenTex("gravel", 512), null, 3f, 0.3f, Color.white);
            var meadow = MatLib.Layer("Meadow", MatLib.GenTex("grass", 512), null, 13f, 0.1f, new Color(0.86f, 0.9f, 0.8f));
            td.terrainLayers = new[] { grass, dirt, cobble, flag, rock, gravel, meadow };
            td.alphamapResolution = Alpha;
            td.baseMapResolution = 1024;
            td.SetAlphamaps(0, 0, Splat(td));
            EditorUtility.SetDirty(td);
            AssetDatabase.SaveAssets();

            var go = Terrain.CreateTerrainGameObject(td);
            go.name = "Terrain";
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(Heights.U0, Heights.Base, Heights.V0);
            Terrain = go.GetComponent<Terrain>();
            var tm = new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit")) { name = "TerrainLit" };
            tm.EnableKeyword("_TERRAIN_INSTANCED_PERPIXEL_NORMAL");
            AssetDatabase.CreateAsset(tm, MatLib.GenDir + "/TerrainLit.mat");
            Terrain.materialTemplate = tm;
            Terrain.heightmapPixelError = 2;
            Terrain.basemapDistance = 220;
            Terrain.drawInstanced = true;
            Terrain.treeDistance = 900;
            Terrain.detailObjectDistance = 90;
            Terrain.detailObjectDensity = 1f;
            go.isStatic = true;
            return Terrain;
        }

        float[,,] Splat(TerrainData td)
        {
            const int L = 7; // grass, dirt, cobble, flag, rock, gravel, meadow
            var a = new float[Alpha, Alpha, L];
            float step = Heights.Size / Alpha;
            var w = new float[L];
            for (int y = 0; y < Alpha; y++)
                for (int x = 0; x < Alpha; x++)
                {
                    var p = P(Heights.U0 + (x + 0.5f) * step, Heights.V0 + (y + 0.5f) * step);
                    System.Array.Clear(w, 0, L);
                    float h = Ground(p);
                    float hx = Ground(p + P(1, 0)) - Ground(p - P(1, 0)), hy = Ground(p + P(0, 1)) - Ground(p - P(0, 1));
                    float slope = Mathf.Sqrt(hx * hx + hy * hy) * 0.5f;       // rise per metre
                    float n = Fbm(p.x * 0.07f, p.y * 0.07f, 3), n2 = Noise(p.x * 0.4f, p.y * 0.4f);
                    // base: grass near town, meadow mix farther out
                    float far = Mathf.Clamp01(Smooth(90, 300, (p - P(100, 40)).magnitude) * 0.6f + (Fbm(p.x * 0.012f + 50, p.y * 0.012f, 3) - 0.35f) * 1.4f);
                    w[0] = 1 - far; w[6] = far;
                    if (n > 0.62f) { w[1] += (n - 0.62f) * 3f; }
                    // mountains / steep ground
                    float rk = Smooth(0.7f, 1.2f, slope + (n2 - 0.5f) * 0.3f) + Smooth(28, 60, h + n * 18) * 0.8f;
                    if (rk > 0) { w[4] += rk * 3; }
                    // natural banks outside the seed
                    float wd = Seed.WaterDistance(p);
                    if (wd < 2.5f && !Seed.InHard(p)) w[5] += 3 * (1 - wd / 2.5f);
                    if (h < 0.4f) w[5] += 2;
                    a[y, x, 0] = 0;
                    // authored surfaces override
                    int surf = SurfaceAt(p, out float edge);
                    if (surf >= 0)
                    {
                        System.Array.Clear(w, 0, L);
                        w[surf] = 1;
                        if (edge > 0) { w[1] += edge * 0.35f; }
                    }
                    else if (InGarden(p, out float rows))
                    {
                        System.Array.Clear(w, 0, L);
                        w[1] = 0.55f + 0.45f * rows; w[0] = 0.45f * (1 - rows);
                    }
                    else if (Seed.InHard(p) && (NearHouse(p, 7f) || lay.RouteClear(p) < 4f))
                    {
                        // inside the town: trodden earth, gravel and moss rather than lawn
                        bool near = NearHouse(p, 0.8f);
                        float m = Fbm(p.x * 0.18f + 7, p.y * 0.18f, 3);
                        System.Array.Clear(w, 0, L);
                        // worn old paving with earth and moss in the joints
                        w[2] = 0.55f + 0.3f * m; w[1] = near ? 0.45f : 0.3f; w[0] = Mathf.Clamp01(m - 0.45f) * 1.4f; w[5] = 0.1f * n2;
                        if (slope > 0.9f) w[4] += 0.8f;
                    }
                    float sum = 0;
                    for (int k = 0; k < L; k++) sum += w[k];
                    if (sum < 1e-4f) { w[0] = 1; sum = 1; }
                    for (int k = 0; k < L; k++) a[y, x, k] = w[k] / sum;
                }
            return a;
        }

        /// 2 = cobble, 3 = flag, 1 = dirt; edge in 0..1 for grime at lane edges; -1 if not authored.
        int SurfaceAt(Vector2 p, out float edge)
        {
            edge = 0;
            foreach (var pl in Seed.Platforms)
                if (InPoly(p, pl.Poly)) return pl.Surface == Surf.Dirt ? 1 : pl.Surface == Surf.Cobble ? 2 : 3;
            foreach (var r in Seed.Routes)
            {
                if (!Bounds(r.Pts, r.Width).Contains(p)) continue;
                float d = r.Query(p, out _, out _, out _);
                float hw = r.Width * 0.5f;
                if (d > hw) continue;
                if (r.Surface == Surf.Dirt) return 1;
                edge = Smooth(hw - 0.45f, hw, d);
                if (r.Width >= 2.8f && d < 0.24f) return 3;      // central gutter of flat stones
                return 2;
            }
            if (InPoly(p, Seed.X1) || InPoly(p, Seed.StubX1O) || InPoly(p, Seed.StubX1W)) return 2;
            return -1;
        }

        bool InGarden(Vector2 p, out float rows)
        {
            rows = 0;
            foreach (var g in lay.Gardens)
            {
                if (!InPoly(p, g.Poly)) continue;
                float along = Vector2.Dot(p - g.Poly[0], Perp(g.Dir));
                rows = Mathf.Abs(Mathf.Sin(along * Mathf.PI / 0.9f)) > 0.55f ? 1 : 0;
                if (DistPolyEdge(p, g.Poly) < 1.0f) rows = 0.3f;
                return true;
            }
            return false;
        }

        bool NearHouse(Vector2 p, float dist = 0.8f)
        {
            float r2 = (dist + 9f) * (dist + 9f);
            foreach (var h in lay.Houses)
            {
                if ((h.Center - p).sqrMagnitude > r2) continue;
                if (InPoly(p, h.Foot) || DistPolyEdge(p, h.Foot) < dist) return true;
            }
            return false;
        }
    }
}
