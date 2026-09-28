using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Retaining walls, river walls, parapets and garden walls (procedural stone).
    public class WallBuild
    {
        readonly TerrainBuild tb;
        readonly Layout lay;
        readonly Transform root;
        public WallBuild(TerrainBuild t, Layout l, Transform parent) { tb = t; lay = l; root = parent; }

        /// Generic wall along a plan polyline. "outward" = side the face looks at (left normal * side).
        /// top(i) / bottom(i) are per-sample heights. Parapet sits on the outward edge.
        public void Wall(MeshKit mk, Vector2[] line, float side, float thickness, System.Func<Vector2, float> top, System.Func<Vector2, float> bottom,
            float parapet, float batter = 0f, float wetLine = -99f, bool coping = true)
        {
            var pts = Resample(line, 0.8f);
            int n = pts.Length;
            var nrm = new Vector2[n];
            for (int i = 0; i < n; i++)
            {
                Vector2 t = (i < n - 1 ? pts[i + 1] - pts[i] : pts[i] - pts[i - 1]).normalized;
                if (i > 0 && i < n - 1) t = (pts[i + 1] - pts[i - 1]).normalized;
                nrm[i] = Perp(t) * side;
            }
            var stone = MatLib.Get("Stone"); var wet = MatLib.Get("StoneWet"); var cap = MatLib.Get("Coping");
            for (int i = 0; i < n - 1; i++)
            {
                Vector2 a = pts[i], b = pts[i + 1];
                Vector2 na = nrm[i], nb = nrm[i + 1];
                Vector3 no = ToW(na + nb).normalized, ni = -no, up = Vector3.up;
                float ta = top(a), tb2 = top(b), ba = bottom(a), bb = bottom(b);
                Vector3 oa0 = W(a, ba), ob0 = W(b, bb);
                Vector3 oa1 = W(a - na * batter, ta), ob1 = W(b - nb * batter, tb2);
                Vector3 ia0 = W(a - na * thickness, ba), ib0 = W(b - nb * thickness, bb);
                Vector3 ia1 = W(a - na * thickness, ta), ib1 = W(b - nb * thickness, tb2);
                if (wetLine > -90 && Mathf.Min(ba, bb) < wetLine)
                {
                    float wa = Mathf.Clamp(wetLine, ba, ta), wb = Mathf.Clamp(wetLine, bb, tb2);
                    Vector3 oaw = Vector3.Lerp(oa0, oa1, Mathf.InverseLerp(ba, ta, wa)), obw = Vector3.Lerp(ob0, ob1, Mathf.InverseLerp(bb, tb2, wb));
                    O(mk, wet, oa0, ob0, obw, oaw, no);
                    if (ta > wetLine || tb2 > wetLine) O(mk, stone, oaw, obw, ob1, oa1, no);
                }
                else O(mk, stone, oa0, ob0, ob1, oa1, no);
                O(mk, stone, ia0, ib0, ib1, ia1, ni);
                Vector3 wa3 = ToW(na) * 0.08f, wb3 = ToW(nb) * 0.08f, h = up * 0.12f;
                if (parapet <= 0)
                {
                    if (coping)
                    {
                        O(mk, cap, oa1 + h + wa3, ob1 + h + wb3, ib1 + h, ia1 + h, up);
                        O(mk, cap, oa1 + wa3, ob1 + wb3, ob1 + h + wb3, oa1 + h + wa3, no);
                        O(mk, cap, ia1, ib1, ib1 + h, ia1 + h, ni);
                    }
                    else O(mk, stone, oa1, ob1, ib1, ia1, up);
                }
                else
                {
                    float pw = 0.42f;
                    O(mk, stone, oa1, ob1, ib1, ia1, up);
                    Vector3 pa1 = oa1 + up * parapet, pb1 = ob1 + up * parapet;
                    Vector3 qa0 = oa1 - ToW(na) * pw, qb0 = ob1 - ToW(nb) * pw;
                    Vector3 qa1 = qa0 + up * parapet, qb1 = qb0 + up * parapet;
                    O(mk, stone, oa1, ob1, pb1, pa1, no);
                    O(mk, stone, qa0, qb0, qb1, qa1, ni);
                    Vector3 ca = ToW(na) * 0.06f, cb = ToW(nb) * 0.06f, hh = up * 0.1f;
                    O(mk, cap, pa1 + hh + ca, pb1 + hh + cb, qb1 + hh - cb, qa1 + hh - ca, up);
                    O(mk, cap, pa1 + ca, pb1 + cb, pb1 + hh + cb, pa1 + hh + ca, no);
                    O(mk, cap, qa1 - ca, qb1 - cb, qb1 + hh - cb, qa1 + hh - ca, ni);
                }
            }
            // end caps
            EndCap(mk, stone, pts[0], nrm[0], top(pts[0]) + Mathf.Max(0, parapet), bottom(pts[0]), thickness, true, (pts[1] - pts[0]).normalized);
            EndCap(mk, stone, pts[n - 1], nrm[n - 1], top(pts[n - 1]) + Mathf.Max(0, parapet), bottom(pts[n - 1]), thickness, false, (pts[n - 1] - pts[n - 2]).normalized);
        }

        static Vector3 ToW(Vector2 d) => new Vector3(d.x, 0, d.y);

        /// Quad (a,b,c,d as a loop) oriented so its face normal agrees with "want".
        public static void O(MeshKit mk, Material m, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 want)
        {
            var test = Vector3.Cross(b - a, d - a);
            if (test.sqrMagnitude < 1e-10f) test = Vector3.Cross(c - b, a - b);
            if (Vector3.Dot(test, want) >= 0) mk.Quad(m, a, b, c, d);
            else mk.Quad(m, a, d, c, b);
        }

        static void EndCap(MeshKit mk, Material m, Vector2 p, Vector2 nn, float top, float bottom, float th, bool start, Vector2 along)
        {
            Vector3 o0 = W(p, bottom), o1 = W(p, top), i0 = W(p - nn * th, bottom), i1 = W(p - nn * th, top);
            O(mk, m, o0, i0, i1, o1, ToW(start ? -along : along));
        }

        // ------------------------------------------------------------------ banks

        public void BankWalls()
        {
            var mk = new MeshKit { Tile = 2.2f };
            // Río: north bank R.N0..R.N8 (indices 0..8), south bank R.S8..R.S0 (9..17)
            BankRun(mk, Seed.Rio, 0, 8, -1);
            BankRun(mk, Seed.Rio, 9, 17, -1);
            // Arroyo: west bank A.W0..A.W8 (0..8), east bank A.E8..A.E0 (9..17)
            BankRun(mk, Seed.Arroyo, 0, 8, -1);
            BankRun(mk, Seed.Arroyo, 9, 17, -1);
            mk.Build("BankWalls", root, true);
        }

        void BankRun(MeshKit mk, Vector2[] poly, int from, int to, float _)
        {
            var edge = new List<Vector2>();
            for (int k = from; k <= to; k++) edge.Add(poly[k]);
            var pts = Resample(edge.ToArray(), 0.8f);
            // water side: polygon interior. Determine side sign from centroid of the local segment.
            // split into runs, skipping gaps (ford, landing quay steps handled separately)
            var run = new List<Vector2>();
            void Flush()
            {
                if (run.Count >= 2)
                {
                    var line = run.ToArray();
                    Vector2 mid = line[line.Length / 2], t = (line[Mathf.Min(line.Length - 1, line.Length / 2 + 1)] - line[Mathf.Max(0, line.Length / 2 - 1)]).normalized;
                    float side = InPoly(mid + Perp(t) * 0.6f, poly) ? 1 : -1;     // face towards the water
                    // shift the face 0.4 m into the water so the terrain edge stays behind it
                    var shifted = line.Select((q, i) => q).ToArray();
                    var off = Offset(shifted, 0.4f * side);
                    Wall(mk, off, side, 1.2f,
                        q => LandTop(q - PerpAt(off, q) * side * 1.6f, poly),
                        q => -1.6f,
                        ParapetAt(off), 0.12f, 0.9f);
                }
                run.Clear();
            }
            foreach (var p in pts)
            {
                bool gap = !Seed.InHard(p) && DistPolyEdge(p, Seed.Hard) > 0.5f;
                gap |= poly == Seed.Arroyo && DistSeg(p, Stamps.FordA, Stamps.FordB, out _) < 1.6f;      // X5 ford steps
                gap |= Bridge.InFootprint(p, 0.4f, out _);                               // the bridge abutment replaces the bank wall
                gap |= poly == Seed.Rio && DistSeg(p, Stamps.SlipTop, Stamps.SlipTop + Stamps.SlipDir * (Stamps.SlipLen + 1), out _) < 2.9f;   // landing slipway
                if (gap) Flush(); else run.Add(p);
            }
            Flush();
        }

        static Vector2 PerpAt(Vector2[] line, Vector2 q)
        {
            DistPolyline(q, line, out int s, out _);
            return Perp((line[s + 1] - line[s]).normalized);
        }

        float LandTop(Vector2 q, Vector2[] poly)
        {
            float h = tb.Ground(q);
            return Mathf.Max(h, 0.7f);
        }

        float ParapetAt(Vector2[] line)
        {
            // parapet where the bank top is high enough to be a fall risk
            var mid = line[line.Length / 2];
            float h = tb.Ground(mid);
            return h > 1.6f ? 0.85f : 0f;
        }

        // ------------------------------------------------------------------ garden / street walls

        public void FrontWalls()
        {
            var mk = new MeshKit { Tile = 2f };
            foreach (var (a, b, h) in lay.FrontWalls)
            {
                var d = (b - a).normalized;
                var line = new[] { a - d * 0.05f, b + d * 0.05f };
                float hh = h + 2.2f;
                Wall(mk, line, 1, 0.45f, q => Mathf.Max(hh, tb.Ground(q) + 1.6f), q => Mathf.Min(h, tb.Ground(q)) - 0.6f, 0, 0, -99, true);
            }
            mk.Build("StreetTapias", root, true);
        }

        public void GardenWalls()
        {
            var mk = new MeshKit { Tile = 2f };
            foreach (var g in lay.Gardens)
            {
                for (int k = 0; k < 4; k++)
                {
                    Vector2 a = g.Poly[k], b = g.Poly[(k + 1) % 4];
                    var line = new[] { a, b };
                    Wall(mk, line, InPoly((a + b) * 0.5f + Perp((b - a).normalized) * 0.5f, g.Poly) ? -1 : 1, 0.4f,
                        q => Mathf.Max(g.H, tb.Ground(q + Perp((b - a).normalized) * 0.6f), tb.Ground(q - Perp((b - a).normalized) * 0.6f)) + 1.7f,
                        q => Mathf.Min(g.H, Mathf.Min(tb.Ground(q + Perp((b - a).normalized) * 0.8f), tb.Ground(q - Perp((b - a).normalized) * 0.8f))) - 0.5f,
                        0, 0, -99, true);
                }
            }
            mk.Build("GardenWalls", root, true);
        }

        /// Platform edges: retaining wall + parapet where the ground falls away and nothing else continues the surface.
        public void PlatformEdges()
        {
            var mk = new MeshKit { Tile = 2.2f };
            foreach (var pl in Seed.Platforms)
            {
                if (pl.Id == "BarPatio") continue;      // enclosed by its own tapias
                var c = Centroid(pl.Poly);
                for (int k = 0; k < pl.Poly.Length; k++)
                {
                    Vector2 a = pl.Poly[k], b = pl.Poly[(k + 1) % pl.Poly.Length];
                    var d = (b - a).normalized; var nOut = Perp(d);
                    if (Vector2.Dot(nOut, (a + b) * 0.5f - c) < 0) nOut = -nOut;
                    var run = new List<Vector2>();
                    foreach (var p in Resample(new[] { a, b }, 0.8f))
                    {
                        var o = p + nOut * 1.6f;
                        bool open = !OnSurface(o) && tb.Ground(o) < pl.H - 0.9f && !Seed.InMask(o + nOut * 0.2f)
                            && !Bridge.InFootprint(o, 0.6f, out _) && !Bridge.InFootprint(p, 0.6f, out _);   // the bridge continues the surface
                        if (open) run.Add(p);
                        else { Emit(); }
                    }
                    Emit();
                    void Emit()
                    {
                        if (run.Count >= 2)
                        {
                            float side = Vector2.Dot(Perp(d), nOut) > 0 ? 1 : -1;
                            var line = run.ToArray();
                            float hh = pl.H;
                            Wall(mk, line, side, 0.9f, q => hh, q => tb.Ground(q + nOut * 1.2f) - 0.4f, 0.9f, 0.06f, -99);
                        }
                        run.Clear();
                    }
                }
            }
            mk.Build("PlatformEdges", root, true);
        }

        /// Route edges where the ground drops or rises sharply beside the lane.
        public void RouteEdges()
        {
            var mk = new MeshKit { Tile = 2.2f };
            foreach (var r in Seed.Routes)
            {
                foreach (int side in new[] { 1, -1 })
                {
                    var edge = Offset(r.Pts, side * (r.Width * 0.5f + 0.25f));
                    var pts = Resample(edge, 0.8f);
                    var runLow = new List<Vector2>(); var runHigh = new List<Vector2>();
                    foreach (var p in pts)
                    {
                        r.Query(p, out float rh, out _, out var dir);
                        var nOut = Perp(dir) * side;
                        var o = p + nOut * 1.8f;
                        bool free = !OnSurface(o) && !OnSurface(p + nOut * 0.6f) && !Seed.InMask(o) && !Bridge.InFootprint(o, 0.6f, out _);
                        float g = tb.Ground(o);
                        if (free && g < rh - 1.3f) runLow.Add(p); else Flush(runLow, side, r, true);
                        if (free && g > rh + 1.3f) runHigh.Add(p); else Flush(runHigh, side, r, false);
                    }
                    Flush(runLow, side, r, true); Flush(runHigh, side, r, false);
                }
            }
            mk.Build("RouteEdges", root, true);

            void Flush(List<Vector2> run, int side, Route r, bool low)
            {
                if (run.Count >= 3)
                {
                    var line = run.ToArray();
                    if (low)
                        Wall(mk, line, side, 0.8f, q => { r.Query(q, out float h, out _, out _); return h; }, q => tb.Ground(q + PerpAt(line, q) * side * 1.2f) - 0.4f, 0.9f, 0.05f);
                    else
                        Wall(mk, line, -side, 0.8f, q => tb.Ground(q + PerpAt(line, q) * side * 1.4f) + 0.2f, q => { r.Query(q, out float h, out _, out _); return h - 0.5f; }, 0, 0.05f);
                }
                run.Clear();
            }
        }

        bool OnSurface(Vector2 p)
        {
            foreach (var pl in Seed.Platforms) if (InPoly(p, pl.Poly)) return true;
            if (lay.RouteClear(p) < 0.2f) return true;
            foreach (var h in lay.Houses) if ((h.Center - p).sqrMagnitude < 150 && InPoly(p, h.Foot)) return true;
            foreach (var g in lay.Gardens) if (InPoly(p, g.Poly)) return true;
            return false;
        }

        /// Invisible fence exactly along the water masks: nobody walks into the river. Gaps: X1 deck, X5 ford.
        public void WaterFence()
        {
            var verts = new List<Vector3>(); var tris = new List<int>();
            foreach (var poly in new[] { Seed.Rio, Seed.Arroyo })
            {
                var loop = poly.Concat(new[] { poly[0] }).ToArray();
                var pts = Resample(loop, 0.8f);
                for (int i = 0; i < pts.Length - 1; i++)
                {
                    Vector2 a = pts[i], b = pts[i + 1], m = (a + b) * 0.5f;
                    if (!Seed.InHard(m) && DistPolyEdge(m, Seed.Hard) > 0.3f) continue;
                    if (InPoly(m, Seed.X1) || DistPolyEdge(m, Seed.X1) < 0.6f) continue;
                    if (poly == Seed.Arroyo && (InPoly(m, Seed.X5) || DistPolyEdge(m, Seed.X5) < 0.3f)) continue;
                    float top = Mathf.Max(tb.Ground(m + (m - Centroid(poly)).normalized * 1.5f), 1f) + 1.6f;
                    int k = verts.Count;
                    verts.Add(W(a, -2)); verts.Add(W(b, -2)); verts.Add(W(b, top)); verts.Add(W(a, top));
                    tris.AddRange(new[] { k, k + 1, k + 2, k, k + 2, k + 3, k, k + 2, k + 1, k, k + 3, k + 2 });
                }
            }
            // ford corridor sides inside the Arroyo (X5 polygon long edges, wet part only)
            foreach (int t in new[] { -1, 1 })
            {
                Vector2 a = Stamps.Ford(Stamps.FordWetIn - 0.4f, t * 1.5f), b = Stamps.Ford(Stamps.FordWetOut + 0.4f, t * 1.5f);
                int k = verts.Count;
                verts.Add(W(a, -2)); verts.Add(W(b, -2)); verts.Add(W(b, 4)); verts.Add(W(a, 4));
                tris.AddRange(new[] { k, k + 1, k + 2, k, k + 2, k + 3, k, k + 2, k + 1, k, k + 3, k + 2 });
            }
            var mesh = new Mesh { name = "WaterFence", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.RecalculateBounds();
            MeshKit.Meshes.Add(mesh);
            var go = new GameObject("WaterFence (invisible)");
            go.transform.SetParent(root, false);
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
            go.isStatic = true;
        }

        /// Invisible boundary colliders along the hard seed polygon (the playable limit).
        public void HardBoundary()
        {
            var go = new GameObject("HardBoundary");
            go.transform.SetParent(root, false);
            for (int k = 0; k < Seed.Hard.Length; k++)
            {
                Vector2 a = Seed.Hard[k], b = Seed.Hard[(k + 1) % Seed.Hard.Length];
                var seg = new GameObject($"edge{k}");
                seg.transform.SetParent(go.transform, false);
                var mid = (a + b) * 0.5f;
                float h = tb.Ground(mid);
                seg.transform.position = W(mid, h + 5);
                seg.transform.rotation = Quaternion.LookRotation(W(b - a, 0), Vector3.up);
                var bc = seg.AddComponent<BoxCollider>();
                bc.size = new Vector3(0.5f, 60, (b - a).magnitude + 1);
                seg.isStatic = true;
            }
        }
    }
}
