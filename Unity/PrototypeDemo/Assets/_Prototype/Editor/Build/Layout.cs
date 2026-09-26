using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    public enum HouseKind { Filler, Bar, Civic, Tower, Shop, P9, Arcade, Slot, Soft }

    public class HousePlan
    {
        public string Id;
        public HouseKind Kind;
        public Vector2 A;        // front-left corner on the exterior face line
        public Vector2 Dir;      // along the front (A -> B)
        public Vector2 In;       // inward normal
        public int W, D;         // metres, even
        public int Floors;
        public float StreetH, FL;
        public int DoorModule = -1;
        public string Sign, Name, Facade;
        public bool Enterable, Hero, PlasterUpper, Balcony, GableToStreet;
        public int Seed;
        public Vector2 B => A + Dir * W;
        public Vector2 C => A + Dir * W + In * D;
        public Vector2 Dd => A + In * D;
        public Vector2[] Foot => new[] { A, B, C, Dd };
        public Vector2 Center => A + Dir * (W * 0.5f) + In * (D * 0.5f);
        public float Top => FL + Floors * 3f;
    }

    public class GardenPlan { public Vector2[] Poly; public float H; public Vector2 Dir; public int Seed; public bool WithWell; }

    /// Planning pass (no geometry). Everything downstream (terrain, walls, houses, dressing) consumes this.
    public class Layout
    {
        public readonly List<HousePlan> Houses = new List<HousePlan>();
        public readonly List<GardenPlan> Gardens = new List<GardenPlan>();
        public readonly List<Vector2[]> Reserved = new List<Vector2[]>();
        /// Street-line garden walls closing the gaps between detached houses: (a, b, street height).
        public readonly List<(Vector2 a, Vector2 b, float h)> FrontWalls = new List<(Vector2, Vector2, float)>();

        // occupancy grid over the hard seed bbox, 0.5 m cells
        const float Cell = 0.5f;
        readonly float u0 = -25, v0 = -85;
        readonly int nu = 560, nv = 470;
        readonly bool[,] occ;
        readonly float[,] routeClear;   // min over routes of (dist - halfWidth)

        public Layout()
        {
            occ = new bool[nu, nv];
            routeClear = new float[nu, nv];
            for (int i = 0; i < nu; i++)
                for (int j = 0; j < nv; j++)
                {
                    var p = CellP(i, j);
                    float best = 99;
                    foreach (var r in Seed.Routes)
                    {
                        float d = DistPolyline(p, r.Pts) - r.Width * 0.5f;
                        if (d < best) best = d;
                    }
                    routeClear[i, j] = best;
                }
        }

        Vector2 CellP(int i, int j) => new Vector2(u0 + (i + 0.5f) * Cell, v0 + (j + 0.5f) * Cell);

        public static Route R(string id) => Seed.Routes.First(r => r.Id == id);

        public float RouteClear(Vector2 p)
        {
            int i = Mathf.FloorToInt((p.x - u0) / Cell), j = Mathf.FloorToInt((p.y - v0) / Cell);
            if (i < 0 || j < 0 || i >= nu || j >= nv) return 99;
            return routeClear[i, j];
        }

        void Mark(Vector2[] poly, float pad)
        {
            var b = Bounds(poly, pad + Cell);
            for (int i = Mathf.Max(0, (int)((b.xMin - u0) / Cell)); i < Mathf.Min(nu, (int)((b.xMax - u0) / Cell) + 1); i++)
                for (int j = Mathf.Max(0, (int)((b.yMin - v0) / Cell)); j < Mathf.Min(nv, (int)((b.yMax - v0) / Cell) + 1); j++)
                {
                    var p = CellP(i, j);
                    if (InPoly(p, poly) || (pad > 0 && DistPolyEdge(p, poly) < pad)) occ[i, j] = true;
                }
        }

        public readonly Dictionary<string, int> Reject = new Dictionary<string, int>();
        bool No(string why) { Reject[why] = Reject.TryGetValue(why, out int c) ? c + 1 : 1; return false; }

        bool Free(Vector2[] poly, float routeMargin)
        {
            var b = Bounds(poly);
            for (int i = Mathf.Max(0, (int)((b.xMin - u0) / Cell)); i < Mathf.Min(nu, (int)((b.xMax - u0) / Cell) + 1); i++)
                for (int j = Mathf.Max(0, (int)((b.yMin - v0) / Cell)); j < Mathf.Min(nv, (int)((b.yMax - v0) / Cell) + 1); j++)
                {
                    var p = CellP(i, j);
                    if (!InPoly(p, poly)) continue;
                    if (occ[i, j]) return No("occupied");
                    if (routeClear[i, j] < routeMargin) return No("route");
                }
            foreach (var p in poly.Concat(new[] { Centroid(poly) }))
            {
                if (!Seed.InHard(p) || Seed.InMask(p)) return No("outside/water");
                if (Seed.WaterDistance(p) < 3.3f) return No("bank");
                if (DistPolyEdge(p, Seed.Hard) < 1.0f) return No("boundary");
            }
            // interior samples for water shoulder
            var c = Centroid(poly);
            for (int k = 0; k < poly.Length; k++)
            {
                var m = (poly[k] + poly[(k + 1) % poly.Length]) * 0.5f;
                if (Seed.WaterDistance(m) < 3.3f || Seed.WaterDistance((m + c) * 0.5f) < 3.3f) return No("bank2");
            }
            return true;
        }

        public void Plan()
        {
            // Reserve every authored/open surface first.
            foreach (var pl in Seed.Platforms) { Mark(pl.Poly, 0.6f); }
            foreach (var s in Seed.Slots.Values) Mark(s, 0.4f);
            Mark(Seed.S01, 0.8f); Mark(Seed.S03, 0.6f);
            Mark(Seed.X1, 1.5f); Mark(Seed.StubX1W, 0.8f); Mark(Seed.StubX1O, 0.8f); Mark(Seed.StubX5W, 0.8f); Mark(Seed.StubX5E, 0.8f);
            foreach (var p in Seed.P9) Mark(p.Poly, 0.4f);
            // rendija al río (sec.rendija): narrow view gap from W12 toward the river
            var rendija = Pts(61.5f, -8.2f, 62.5f, -9.6f, 52.0f, -17.2f, 51.0f, -15.8f);
            Reserved.Add(rendija); Mark(rendija, 0.2f);
            // bar back patio + service alley kept clear
            Mark(Rect(84.5f, 88, 44, 60), 0);

            PlanHeroes();
            PlanP9();
            PlanFiller();
            Debug.Log("[Proto] house rejections: " + string.Join(", ", Reject.Select(kv => kv.Key + "=" + kv.Value)));
            Reject.Clear();
            PlanGardens();
            Debug.Log($"[Proto] layout: {Houses.Count} houses ({Houses.Count(h => h.Kind == HouseKind.Filler)} filler), {Gardens.Count} gardens");
        }

        HousePlan Add(HousePlan h)
        {
            // normalise: interior always on the left of the front (In == Perp(Dir))
            if (Vector2.Dot(h.In, Perp(h.Dir)) < 0)
            {
                h.A = h.A + h.Dir * h.W; h.Dir = -h.Dir;
                if (h.DoorModule >= 0) h.DoorModule = h.W / 2 - 1 - h.DoorModule;
            }
            h.Seed = Houses.Count * 7919 + 17;
            Houses.Add(h);
            Mark(h.Foot, 0.3f);
            return h;
        }

        static float StreetHeight(Route r, Vector2 a, Vector2 b)
        {
            r.Query(a, out float ha, out _, out _);
            r.Query(b, out float hb, out _, out _);
            return Mathf.Max(ha, hb);
        }

        void PlanHeroes()
        {
            // F01 — Bar del Casco (I3): front to casco.micro.B (south), service door to W13 (west), back patio.
            var bar = Add(new HousePlan
            {
                Id = "F01", Kind = HouseKind.Bar, Name = "Bar del Casco", Sign = "bar", Facade = "facade_bar",
                A = P(88, 45.6f), Dir = P(1, 0), In = P(0, 1), W = 8, D = 10, Floors = 3,
                DoorModule = 1, Enterable = true, Hero = true, GableToStreet = true, Balcony = true,
            });
            bar.StreetH = 8.45f; bar.FL = 8.62f;

            // F02 — Ayuntamiento main block + P1 tower at the plaza-facing corner.
            var civ = Add(new HousePlan
            {
                Id = "F02", Kind = HouseKind.Civic, Name = "Ayuntamiento",
                A = P(154f, 66.2f), Dir = P(1, 0), In = P(0, 1), W = 12, D = 8, Floors = 3,
                DoorModule = 2, Hero = true, Balcony = true,
            });
            civ.StreetH = Seed.HPlaza; civ.FL = Seed.HPlaza + 0.17f;
            var tower = Add(new HousePlan
            {
                Id = "P1", Kind = HouseKind.Tower, Name = "Torre del Ayuntamiento",
                A = P(148f, 66.2f), Dir = P(1, 0), In = P(0, 1), W = 6, D = 6, Floors = 8, Hero = true,
            });
            tower.StreetH = Seed.HPlaza; tower.FL = Seed.HPlaza + 0.17f;

            // F03 — tienda de diario on W04.
            var shop = Add(new HousePlan
            {
                Id = "F03", Kind = HouseKind.Shop, Name = "Tienda de diario", Sign = "comestibles", Facade = "facade_comestibles",
                A = P(188, 76.4f), Dir = P(1, 0), In = P(0, 1), W = 10, D = 8, Floors = 2, DoorModule = 2, Hero = true,
            });
            shop.StreetH = 12.5f; shop.FL = 12.67f;

            // F06 — soportales de la Plaza (arcade, I0) facing W05 / plaza.
            var arc = Add(new HousePlan
            {
                Id = "F06", Kind = HouseKind.Arcade, Name = "Soportales",
                A = P(126.5f, 62.4f), Dir = P(1, 0), In = P(0, 1), W = 10, D = 8, Floors = 3, Hero = true, PlasterUpper = true,
            });
            arc.StreetH = 11.6f; arc.FL = 11.75f;

            // Closed slot houses (I0).
            SlotHouse("F04", P(69, 41), P(0, 1), P(-1, 0), 12, 6, 3, R("W06"));
            SlotHouse("F05", P(98.4f, 35), P(0, -1), P(1, 0), 10, 6, 3, R("microA"));
            SlotHouse("F07", P(170, 74.4f), P(1, 0), P(0, 1), 8, 10, 3, R("W04"));
            SlotHouse("F08", P(56.5f, 15.6f), P(-1, 0), P(0, -1), 6, 10, 3, R("W06"));
        }

        void SlotHouse(string id, Vector2 a, Vector2 dir, Vector2 inn, int w, int d, int floors, Route r)
        {
            var h = new HousePlan { Id = id, Kind = HouseKind.Slot, A = a, Dir = dir, In = inn, W = w, D = d, Floors = floors, DoorModule = w / 4, Hero = true, Balcony = true };
            h.StreetH = StreetHeight(r, h.A, h.B);
            h.FL = h.StreetH + 0.17f;
            Add(h);
        }

        void PlanP9()
        {
            foreach (var q in Seed.P9)
            {
                var r = R(q.Route);
                // front edge = edge whose midpoint is closest to the route
                int best = 0; float bd = float.MaxValue;
                for (int k = 0; k < 4; k++)
                {
                    var m = (q.Poly[k] + q.Poly[(k + 1) % 4]) * 0.5f;
                    float d = DistPolyline(m, r.Pts);
                    if (d < bd) { bd = d; best = k; }
                }
                Vector2 e0 = q.Poly[best], e1 = q.Poly[(best + 1) % 4], e2 = q.Poly[(best + 2) % 4];
                float front = (e1 - e0).magnitude, depth = (e2 - e1).magnitude;
                int W = Mathf.Max(6, Mathf.FloorToInt(front / 2f) * 2), D = Mathf.Max(6, Mathf.FloorToInt(depth / 2f) * 2);
                if (W > 8 && D > 8) D = 8;
                Vector2 dir = (e1 - e0).normalized, inn = (e2 - e1).normalized;
                Vector2 a = e0 + dir * ((front - W) * 0.5f);
                var h = new HousePlan
                {
                    Id = q.Id, Kind = HouseKind.P9, Name = q.Name, Sign = q.Sign, A = a, Dir = dir, In = inn, W = W, D = D,
                    Facade = q.Id == "F09" ? "facade_taberna" : q.Id == "F16" ? "facade_fonda" : null,
                    Floors = q.Id == "F19" ? 2 : 3, DoorModule = W / 4, Hero = true, Balcony = q.Id != "F17",
                };
                h.StreetH = StreetHeight(r, h.A, h.B);
                h.FL = h.StreetH + 0.17f;
                Add(h);
            }
        }

        void PlanFiller()
        {
            var rng = new Rng(20260925);
            var widths = new[] { 6, 8, 8, 10 };
            foreach (var r in Seed.Routes)
            {
                if (r.Surface == Surf.Dirt) continue;
                bool p8 = r.Id.StartsWith("P8");
                int floorsMin = p8 ? 2 : 3, floorsMax = 3;
                if (r.Id == "P8e") { floorsMin = 2; floorsMax = 2; }
                for (int k = 1; k < r.Pts.Length; k++)
                {
                    Vector2 a = r.Pts[k - 1], b = r.Pts[k];
                    float L = (b - a).magnitude;
                    Vector2 d = (b - a) / L, n = Perp(d);
                    foreach (int side in new[] { 1, -1 })
                    {
                        float s = 1.2f;
                        HousePlan prev = null;
                        while (s < L - 3f)
                        {
                            int W0 = rng.Pick(widths);
                            HousePlan h = null;
                            foreach (var (tw, td) in new[] { (W0, 8), (W0, 6), (6, 8), (6, 6), (W0, 4), (6, 4) })
                            {
                                int W = tw, D = td;
                                if (s + W > L + 1.5f) continue;
                                float setback = r.Width * 0.5f + (r.Id == "W04" ? 1.6f : 0.35f) + rng.Range(0f, 0.25f);
                                Vector2 inn = n * side;
                                Vector2 A = a + d * s + inn * setback;
                                Vector2 dir = d;
                                if (side < 0) { A = A + d * W; dir = -d; }
                                var cand = new HousePlan { Kind = HouseKind.Filler, A = A, Dir = dir, In = inn, W = W, D = D };
                                if (Free(cand.Foot, 0.15f)) { h = cand; break; }
                            }
                            if (h != null)
                            {
                                int W = h.W, D = h.D;
                                h.Id = $"H{Houses.Count:000}";
                                h.Floors = D <= 4 ? 2 : rng.Range(floorsMin, floorsMax + 1);
                                h.StreetH = StreetHeight(r, h.A, h.B);
                                h.FL = h.StreetH + 0.17f;
                                h.DoorModule = rng.Range(0, W / 2);
                                h.PlasterUpper = rng.Chance(0.3f);
                                h.Balcony = rng.Chance(0.45f);
                                h.GableToStreet = false;
                                Add(h);
                                if (prev != null)
                                {
                                    Vector2 pe = side > 0 ? prev.B : prev.A, cs = side > 0 ? h.A : h.B;
                                    float gap = (cs - pe).magnitude;
                                    if (gap > 0.8f && gap < 7f) FrontWalls.Add((pe, cs, h.StreetH));
                                }
                                prev = h;
                                s += W + (rng.Chance(0.2f) ? rng.Range(4f, 6f) : rng.Range(2.0f, 3.0f));
                            }
                            else { s += 1.0f; prev = null; }
                        }
                    }
                }
            }
            // square fronts: plaza and Casco square, facing inward
            FrontAlongPolygon(Seed.PlazaPoly, rng, 3);
            FrontAlongPolygon(Seed.CascoSq, rng, 3);
        }

        void FrontAlongPolygon(Vector2[] poly, Rng rng, int floors)
        {
            var c = Centroid(poly);
            for (int k = 0; k < poly.Length; k++)
            {
                Vector2 a = poly[k], b = poly[(k + 1) % poly.Length];
                float L = (b - a).magnitude;
                Vector2 d = (b - a) / L, n = Perp(d);
                if (Vector2.Dot(n, c - (a + b) * 0.5f) > 0) n = -n; // outward from square
                float s = 0.5f;
                while (s < L - 5.5f)
                {
                    int W = rng.Chance(0.5f) ? 8 : 6;
                    Vector2 A = a + d * s + n * 1.0f;
                    var h = new HousePlan { Kind = HouseKind.Filler, A = A + d * W, Dir = -d, In = n, W = W, D = 8 };
                    if (Free(h.Foot, 0.15f))
                    {
                        h.Id = $"H{Houses.Count:000}";
                        h.Floors = floors;
                        var pl = Seed.Platforms.First(p => p.Poly == poly);
                        h.StreetH = pl.H; h.FL = pl.H + 0.17f;
                        h.DoorModule = W / 4; h.Balcony = true; h.PlasterUpper = rng.Chance(0.4f);
                        Add(h);
                        s += W + 2.4f;
                    }
                    else s += 1f;
                }
            }
        }

        void PlanGardens()
        {
            var rng = new Rng(777);
            var b = Bounds(Seed.Hard);
            for (float u = b.xMin + 4; u < b.xMax - 4; u += 4f)
                for (float v = b.yMin + 4; v < b.yMax - 4; v += 4f)
                {
                    var p = P(u, v);
                    if (!Seed.InHard(p)) continue;
                    // align to nearest route
                    Vector2 dir = P(1, 0); float best = 99;
                    foreach (var r in Seed.Routes)
                    {
                        float dd = r.Query(p, out _, out _, out var rd);
                        if (dd < best) { best = dd; dir = rd; }
                    }
                    if (best < 4f) continue;
                    if (!Houses.Any(hh => (hh.Center - p).sqrMagnitude < 16f * 16f)) continue;
                    if (Gardens.Count >= 70) break;
                    foreach (var (gw, gd) in new[] { (14f, 10f), (11f, 8f), (8f, 7f) })
                    {
                        var inn = Perp(dir);
                        var A = p - dir * gw * 0.5f - inn * gd * 0.5f;
                        var poly = new[] { A, A + dir * gw, A + dir * gw + inn * gd, A + inn * gd };
                        if (!Free(poly, 0.8f)) continue;
                        Mark(poly, 0.5f);
                        Gardens.Add(new GardenPlan { Poly = poly, Dir = dir, Seed = Gardens.Count * 31 + 5, WithWell = false });
                        break;
                    }
                }
            // the garden nearest sec.pozo gets the well + fig tree
            var pozo = P(140, 16);
            var g = Gardens.OrderBy(x => (Centroid(x.Poly) - pozo).sqrMagnitude).FirstOrDefault();
            if (g != null) g.WithWell = true;
        }
    }
}
