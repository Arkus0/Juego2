using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Layer 6: props, vegetation, street furniture, seams. Only after surfaces and hosts are resolved.
    public class Dressing
    {
        readonly TerrainBuild tb; readonly Layout lay; readonly Transform root;
        readonly MeshKit mk = new MeshKit { Tile = 1.2f };
        readonly Rng rng = new Rng(2026);
        Transform props;
        public Dressing(TerrainBuild t, Layout l, Transform r) { tb = t; lay = l; root = r; }
        Material M(string n) => MatLib.Get(n);
        float G(Vector2 p) => GameplayBuild.WalkHeight(tb, p);

        GameObject Put(string name, Vector2 at, float yaw, float scale = 1, bool col = true, float dy = 0)
            => Kit.Put(name, props, W(at, G(at) + dy), Quaternion.Euler(0, yaw, 0), Vector3.one * scale, true, col);

        public void BuildAll()
        {
            props = new GameObject("Props").transform; props.SetParent(root, false);
            Market();
            Fountains();
            Benches();
            Lamps();
            HouseLife();
            Laundry();
            Gardens();
            Farmyards();
            Meadows();
            Era();
            RiverSide();
            Seams();
            RiverWalk();
            mk.Build("Vestido_Procedural", root, true);
        }

        // ------------------------------------------------------------------ market S01
        void Market()
        {
            var stalls = new[] { (P(136.5f, 42.4f), 17f), (P(141.5f, 44.2f), 17f), (P(146.2f, 45.6f), 17f) };
            int i = 0;
            foreach (var (c, yaw) in stalls)
            {
                float g = G(c);
                var q = Quaternion.Euler(0, yaw, 0);
                Vector3 R = q * Vector3.right, F = q * Vector3.forward;
                foreach (var (x, z) in new[] { (-1.3f, -0.8f), (1.3f, -0.8f), (-1.3f, 0.8f), (1.3f, 0.8f) })
                    mk.Box(M("WoodDark"), W(c, g) + R * x + F * z + Vector3.up * 1.15f, R * 0.05f, Vector3.up * 1.15f, F * 0.05f);
                mk.Box(M("Wood"), W(c, g) + Vector3.up * 0.85f - F * 0.2f, R * 1.35f, Vector3.up * 0.04f, F * 0.55f);
                var aw = i == 1 ? M("Awning") : M("Cloth");
                // sloping awning (accent red on one stall only: ≤5 % rule)
                Vector3 a0 = W(c, g) + R * -1.45f + F * -1.05f + Vector3.up * 2.05f, a1 = W(c, g) + R * 1.45f + F * -1.05f + Vector3.up * 2.05f;
                Vector3 b0 = W(c, g) + R * -1.45f + F * 1.0f + Vector3.up * 2.4f, b1 = W(c, g) + R * 1.45f + F * 1.0f + Vector3.up * 2.4f;
                WallBuild.O(mk, aw, a0, a1, b1, b0, Vector3.up);
                WallBuild.O(mk, aw, a0, b0, b1, a1, Vector3.down);
                foreach (var (x, name) in new[] { (-0.8f, "FarmCrate_Apple"), (0.1f, "FarmCrate_Carrot"), (0.9f, "FarmCrate_Apple") })
                    Kit.Put(name, props, W(c, g) + R * x - F * 0.25f + Vector3.up * 0.89f, q, Vector3.one * 0.8f, true, false);
                Put("FarmCrate_Empty", c + (Vector2)(new Vector2(F.x, F.z) * 1.4f), yaw + 10);
                i++;
            }
            Put("Barrel_Apples", P(133.8f, 41.0f), 0);
            Put("Crate_Wooden", P(149.0f, 47.4f), 25);
            Put("Prop_Wagon", P(131.8f, 37.8f), 80);
        }

        // ------------------------------------------------------------------ fountains
        void Fountains()
        {
            // plaza: octagonal basin with a column and four spouts
            var c = P(153.5f, 60.8f); float g = G(c);
            var stone = M("Stone"); var cap = M("Coping");
            for (int k = 0; k < 8; k++)
            {
                float a0 = k * Mathf.PI / 4, a1 = (k + 1) * Mathf.PI / 4;
                Vector2 p0 = c + new Vector2(Mathf.Cos(a0), Mathf.Sin(a0)) * 2.1f, p1 = c + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * 2.1f;
                mk.Slab(stone, p1, p0, 0.32f, g - 0.3f, g + 0.62f);
                mk.Slab(cap, p1 + (p1 - c).normalized * 0.05f, p0 + (p0 - c).normalized * 0.05f, 0.42f, g + 0.62f, g + 0.7f);
            }
            var water = new List<Vector2>(); for (int k = 0; k < 8; k++) water.Add(c + new Vector2(Mathf.Cos(k * Mathf.PI / 4), Mathf.Sin(k * Mathf.PI / 4)) * 1.85f);
            var wt = Triangulate(water.ToArray());
            for (int t = 0; t < wt.Count; t += 3) WallBuild.O(mk, M("Water"), W(water[wt[t]], g + 0.45f), W(water[wt[t + 1]], g + 0.45f), W(water[wt[t + 2]], g + 0.45f), W(water[wt[t + 2]], g + 0.45f), Vector3.up);
            mk.Box(stone, W(c, g + 1.1f), Vector3.right * 0.28f, Vector3.up * 1.1f, Vector3.forward * 0.28f);
            mk.Box(cap, W(c, g + 2.25f), Vector3.right * 0.42f, Vector3.up * 0.08f, Vector3.forward * 0.42f);
            mk.Box(stone, W(c, g + 2.55f), Vector3.right * 0.16f, Vector3.up * 0.25f, Vector3.forward * 0.16f);
            foreach (var d in new[] { Vector3.right, Vector3.left, Vector3.forward, Vector3.back })
                mk.Box(M("Iron"), W(c, g + 1.55f) + d * 0.42f, d * 0.16f, Vector3.up * 0.03f, Vector3.Cross(d, Vector3.up) * 0.03f);
            // Casco: wall fountain (caño) with trough on a stone back
            var f = P(75.2f, 37.4f); var face = (P(80, 34) - f).normalized; float gf = G(f);
            mk.Slab(stone, f - Perp(face) * 1.1f, f + Perp(face) * 1.1f, 0.5f, gf - 0.3f, gf + 2.3f);
            mk.Slab(cap, f - Perp(face) * 1.2f - face * 0.05f, f + Perp(face) * 1.2f - face * 0.05f, 0.6f, gf + 2.3f, gf + 2.42f);
            var tr = f + face * 0.5f;
            mk.Slab(stone, tr - Perp(face) * 0.9f + face * 0.05f, tr + Perp(face) * 0.9f + face * 0.05f, -0.2f, gf - 0.2f, gf + 0.55f);
            mk.Slab(stone, tr - Perp(face) * 0.9f + face * 0.62f, tr + Perp(face) * 0.9f + face * 0.62f, 0.18f, gf - 0.2f, gf + 0.55f);
            foreach (int s in new[] { -1, 1 }) mk.Slab(stone, tr + Perp(face) * s * 0.9f, tr + Perp(face) * s * 0.9f + face * 0.62f, s * 0.18f, gf - 0.2f, gf + 0.55f);
            mk.Box(M("Water"), W(tr + face * 0.33f, gf + 0.42f), W(Perp(face), 0) * 0.75f, Vector3.up * 0.01f, W(face, 0) * 0.24f);
            mk.Box(M("Iron"), W(f + face * 0.36f, gf + 1.2f), W(face, 0) * 0.22f, Vector3.up * 0.025f, W(Perp(face), 0) * 0.025f);
        }

        void Benches()
        {
            foreach (var (p, yaw) in new[] { (P(160f, 63.5f), 180f), (P(146.5f, 63.7f), 180f), (P(77.5f, 35.0f), 120f), (P(3.5f, 1.2f), 200f),
                         (P(115.8f, 120.3f), 200f), (P(35.8f, -66.2f), 20f), (P(156f, 110f), 250f), (P(170.5f, 30f), 270f) })
                Put("Bench", p, yaw);
        }

        void Lamp(Vector2 p, float yaw)
        {
            // never inside a lane: push to the nearest route edge
            foreach (var r in Seed.Routes)
            {
                float d = r.Query(p, out _, out _, out var dir);
                if (d < r.Width * 0.5f + 0.35f)
                {
                    DistPolyline(p, r.Pts, out int seg, out float t);
                    var c = Vector2.Lerp(r.Pts[seg], r.Pts[seg + 1], t);
                    var n = (p - c).sqrMagnitude > 1e-4f ? (p - c).normalized : Perp(dir);
                    p = c + n * (r.Width * 0.5f + 0.4f);
                }
            }
            float g = G(p);
            mk.Box(M("Iron"), W(p, g + 1.6f), Vector3.right * 0.05f, Vector3.up * 1.6f, Vector3.forward * 0.05f);
            mk.Box(M("StoneDark"), W(p, g + 0.15f), Vector3.right * 0.16f, Vector3.up * 0.15f, Vector3.forward * 0.16f);
            var q = Quaternion.Euler(0, yaw, 0);
            mk.Box(M("Iron"), W(p, g + 3.15f) + q * Vector3.forward * 0.25f, q * Vector3.right * 0.03f, Vector3.up * 0.03f, q * Vector3.forward * 0.28f);
            Kit.Put("Lantern_Wall", props, W(p, g + 2.35f) + q * Vector3.forward * 0.02f, q, Vector3.one * 0.62f, true, false);
        }

        void Lamps()
        {
            foreach (var (p, yaw) in new[] { (P(51.5f, -29.5f), 200f), (P(58.2f, -23.8f), 180f), (P(64.8f, 1.8f), 110f), (P(72.9f, 20.8f), 110f),
                         (P(82.5f, 30.5f), 40f), (P(138.3f, 52.8f), 0f), (P(166.5f, 58.4f), 0f), (P(166.5f, 63.9f), 180f), (P(137.5f, 60.0f), 90f),
                         (P(43.0f, -66.5f), 30f), (P(25.5f, 9.9f), 300f), (P(47.5f, 19.4f), 300f), (P(100.0f, 42.8f), 180f), (P(189.5f, 71.8f), 180f) })
                Lamp(p, yaw);
            // wall lanterns beside the doors of the places people go to
            foreach (var h in lay.Houses.Where(x => x.Sign != null || x.Kind == HouseKind.Civic))
            {
                var e = HouseBuilder.Edges(h)[0];
                var d = (e.b - e.a).normalized;
                var at = e.a + d * (1 + 2 * Mathf.Max(0, h.DoorModule) - 1.15f) + e.outward * 0.16f;
                Kit.Put("Lantern_Wall", props, W(at, h.FL + 2.3f), Quaternion.LookRotation(W(e.outward, 0)), null, true, false);
            }
        }

        // ------------------------------------------------------------------ houses: pots, hydrangeas, firewood, vines, barrels
        void HouseLife()
        {
            foreach (var h in lay.Houses)
            {
                var r = new Rng((uint)(h.Seed * 13 + 7));
                var e = HouseBuilder.Edges(h)[0];
                var d = (e.b - e.a).normalized;
                if (h.DoorModule >= 0 && r.Chance(0.55f))
                {
                    // hydrangea pots flanking the door (on the plinth line, out of the lane band)
                    foreach (int sgn in new[] { -1, 1 })
                    {
                        if (!r.Chance(0.7f)) continue;
                        var at = e.a + d * (1 + 2 * h.DoorModule + sgn * 1.05f) + e.outward * 0.42f;
                        if (lay.RouteClear(at) < -0.15f) continue;
                        float g = G(at);
                        Kit.Put("Pot_1", props, W(at, g), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * 0.95f, true, false);
                        Kit.Put("Bush_Common_Flowers", props, W(at, g + 0.17f), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.32f, 0.42f), true, false);
                    }
                }
                if (h.Kind == HouseKind.Filler && r.Chance(0.3f))
                {
                    // firewood stacked against a side wall
                    var se = HouseBuilder.Edges(h)[r.Chance(0.5f) ? 1 : 3];
                    var sd = (se.b - se.a).normalized;
                    var at = se.a + sd * r.Range(1.2f, (se.b - se.a).magnitude - 2.4f) + se.outward * 0.55f;
                    if (lay.RouteClear(at) > 0.4f) Woodpile(at, sd, G(at));
                }
                if (r.Chance(0.18f))
                {
                    var ve = HouseBuilder.Edges(h)[r.Range(0, 4)];
                    var vd = (ve.b - ve.a).normalized;
                    var at = ve.a + vd * r.Range(1f, (ve.b - ve.a).magnitude - 1f) + ve.outward * 0.1f;
                    Kit.Put("Prop_Vine" + r.Range(1, 10), props, W(at, h.FL + r.Range(0f, 1.5f)), Quaternion.LookRotation(W(ve.outward, 0)), null, true, false);
                }
                if ((h.Kind == HouseKind.P9 || h.Kind == HouseKind.Shop) && h.DoorModule >= 0)
                {
                    var at = e.a + d * (1 + 2 * h.DoorModule + 1.3f) + e.outward * 0.6f;
                    if (lay.RouteClear(at) > -0.3f) Put(r.Chance(0.5f) ? "Barrel" : "Crate_Wooden", at, r.Range(0, 360f), 0.95f);
                }
                // balcony pots
                if (h.Balcony && h.Floors >= 2 && r.Chance(0.6f))
                {
                    var at = e.a + d * (h.W * 0.5f + r.Range(-0.6f, 0.6f)) + e.outward * 0.85f;
                    float y = h.FL + 3f * (h.Floors - 1) + 0.02f;
                    Kit.Put("Pot_1", props, W(at, y), Quaternion.identity, Vector3.one * 0.85f, true, false);
                    Kit.Put("Bush_Common_Flowers", props, W(at, y + 0.15f), Quaternion.identity, Vector3.one * 0.3f, true, false);
                }
            }
            // the bar's barrels by the door and a crate stack by the service door
            Put("Barrel", P(95.2f, 44.7f), 0); Put("Barrel_Holder", P(87.2f, 53.2f), 90); Put("Crate_Wooden", P(87.3f, 50.9f), 10);
        }

        void Woodpile(Vector2 at, Vector2 along, float g)
        {
            var q = Quaternion.LookRotation(W(Perp(along), 0));
            for (int row = 0; row < 4; row++)
                for (int k = 0; k < 7 - row; k++)
                {
                    var p = W(at + along * ((k - (6 - row) * 0.5f) * 0.2f), g + 0.1f + row * 0.18f);
                    mk.Box(M("Wood"), p, q * Vector3.right * 0.09f, Vector3.up * 0.09f, q * Vector3.forward * 0.4f);
                }
        }

        // ------------------------------------------------------------------ laundry across lanes
        void Laundry()
        {
            var colors = new[] { new Color(0.85f, 0.84f, 0.8f), new Color(0.55f, 0.6f, 0.68f), new Color(0.72f, 0.62f, 0.5f), new Color(0.43f, 0.47f, 0.72f), new Color(0.8f, 0.8f, 0.76f) };
            var clothMats = colors.Select((c, i) => { var m = new Material(M("Cloth")) { name = "Laundry" + i }; m.SetColor("_BaseColor", c); UnityEditor.AssetDatabase.CreateAsset(m, $"{MatLib.GenDir}/Laundry{i}.mat"); return m; }).ToArray();
            int lines = 0;
            var fronts = lay.Houses.Where(h => h.Kind == HouseKind.Filler || h.Kind == HouseKind.P9).ToList();
            foreach (var a in fronts)
            {
                if (lines > 14) break;
                var ea = HouseBuilder.Edges(a)[0];
                var ma = (ea.a + ea.b) * 0.5f;
                foreach (var b in fronts)
                {
                    if (b == a) continue;
                    var eb = HouseBuilder.Edges(b)[0];
                    var mb = (eb.a + eb.b) * 0.5f;
                    float dist = (ma - mb).magnitude;
                    if (dist < 3.2f || dist > 6.5f || Vector2.Dot(ea.outward, eb.outward) > -0.7f) continue;
                    if (new Rng((uint)(a.Seed + b.Seed)).Next() > 0.35f) continue;
                    float y = Mathf.Min(a.FL, b.FL) + 5.2f;
                    var p0 = W(ma + ea.outward * 0.1f, y); var p1 = W(mb + eb.outward * 0.1f, y);
                    var dir = (p1 - p0); float L = dir.magnitude; dir /= L;
                    var side = Vector3.Cross(dir, Vector3.up).normalized;
                    mk.Box(M("Iron"), (p0 + p1) * 0.5f + Vector3.down * 0.12f, side * 0.006f, Vector3.up * 0.006f, dir * (L * 0.5f));
                    for (float s = 0.6f; s < L - 0.6f; s += rng.Range(0.55f, 0.95f))
                    {
                        float sag = 0.25f * 4 * (s / L) * (1 - s / L);
                        var top = p0 + dir * s + Vector3.down * (0.12f + sag);
                        float w = rng.Range(0.25f, 0.45f), hgt = rng.Range(0.4f, 0.75f);
                        var m = clothMats[rng.Range(0, clothMats.Length)];
                        WallBuild.O(mk, m, top - dir * w, top + dir * w, top + dir * w + Vector3.down * hgt, top - dir * w + Vector3.down * hgt, side);
                        WallBuild.O(mk, m, top - dir * w, top - dir * w + Vector3.down * hgt, top + dir * w + Vector3.down * hgt, top + dir * w, -side);
                    }
                    lines++;
                    break;
                }
            }
        }

        // ------------------------------------------------------------------ huertas, era, river edge
        void Gardens()
        {
            // three kinds of plot so no walled garden reads as an empty lot: vegetable rows, orchard, hay yard
            string[][] crops = { new[] { "Plant_1", "Plant_7" }, new[] { "Bush_Common" }, new[] { "Grass_Common_Tall", "Grass_Wispy_Tall" }, new[] { "Fern_1" }, new[] { "Plant_1_Big" } };
            foreach (var g in lay.Gardens)
            {
                var r = new Rng((uint)g.Seed + 99);
                var c = Centroid(g.Poly);
                var along = g.Dir; var across = Perp(g.Dir);
                float w = (g.Poly[1] - g.Poly[0]).magnitude, dp = (g.Poly[3] - g.Poly[0]).magnitude;
                float kind = g.WithWell ? 0.1f : r.Next();
                if (kind < 0.5f)
                {
                    // huerta: soil ridges with dense rows, a different crop every row or two, bean poles on one row
                    int row = 0;
                    for (float b = -dp / 2 + 1.3f; b < dp / 2 - 1.1f; b += 1.05f, row++)
                    {
                        var set = crops[(row / 2 + r.Range(0, crops.Length)) % crops.Length];
                        var rc = c + across * b;
                        mk.Box(M("Soil"), W(rc, g.H + 0.03f), W(along, 0) * (w / 2 - 1.0f), Vector3.up * 0.06f, W(across, 0) * 0.34f);
                        bool poles = row == 1 && r.Chance(0.6f);
                        for (float a = -w / 2 + 1.25f; a < w / 2 - 1.2f; a += poles ? 0.7f : 0.42f)
                        {
                            var q = rc + along * (a + r.Range(-0.06f, 0.06f));
                            if (poles)
                            {
                                foreach (int sg in new[] { -1, 1 })
                                    mk.Box(M("Wood"), W(q + across * sg * 0.12f, g.H + 1.0f), W(along, 0) * 0.015f, Vector3.up * 0.95f, W(across, 0) * 0.015f);
                                Kit.Put("Plant_7", props, W(q, g.H + 0.08f), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.9f, 1.2f), true, false);
                                continue;
                            }
                            string n = set[r.Range(0, set.Length)];
                            float sc = n == "Bush_Common" ? r.Range(0.26f, 0.34f) : n == "Plant_1_Big" ? r.Range(0.45f, 0.6f) : r.Range(0.75f, 1.05f);
                            Kit.Put(n, props, W(q, g.H + 0.08f), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * sc, true, false);
                        }
                    }
                }
                else if (kind < 0.8f)
                {
                    // frutal: a few low fruit trees over grass and flowers
                    int nx = Mathf.Max(1, Mathf.FloorToInt((w - 2f) / 4f)), ny = Mathf.Max(1, Mathf.FloorToInt((dp - 2f) / 4f));
                    for (int i = 0; i < nx; i++)
                        for (int j = 0; j < ny; j++)
                        {
                            var q = c + along * ((i - (nx - 1) * 0.5f) * 4f + r.Range(-0.5f, 0.5f)) + across * ((j - (ny - 1) * 0.5f) * 4f + r.Range(-0.5f, 0.5f));
                            Kit.Put("CommonTree_" + r.Range(1, 6), props, W(q, g.H), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.38f, 0.52f), true, false);
                        }
                    for (int k = 0; k < 14; k++)
                    {
                        var q = c + along * r.Range(-w / 2 + 1, w / 2 - 1) + across * r.Range(-dp / 2 + 1, dp / 2 - 1);
                        Kit.Put(r.Chance(0.3f) ? "Flower_3_Group" : r.Chance(0.5f) ? "Grass_Common_Tall" : "Clover_1", props, W(q, g.H), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.7f, 1.1f), true, false);
                    }
                }
                else
                {
                    // hay yard: a meda, a cart or crates, firewood
                    Meda(c + along * r.Range(-w / 5, w / 5) + across * r.Range(-dp / 6, dp / 6), g.H, r.Range(0.85f, 1.1f));
                    if (w > 10f) Meda(c + along * (w / 2 - 2.2f) - across * (dp / 2 - 2.2f), g.H, r.Range(0.7f, 0.9f));
                    var corner = c - along * (w / 2 - 1.6f) + across * (dp / 2 - 1.3f);
                    if (r.Chance(0.5f)) Kit.Put("Prop_Wagon", props, W(corner, g.H), Quaternion.LookRotation(W(along, 0)), Vector3.one * 0.9f, true, false);
                    else { Kit.Put("FarmCrate_Empty", props, W(corner, g.H), Quaternion.identity, null, true, false); Kit.Put("Barrel", props, W(corner + along * 0.9f, g.H), Quaternion.identity, null, true, false); }
                    Woodpile(c + across * (dp / 2 - 0.9f) + along * (w / 4), along, g.H);
                }
                if (g.WithWell)
                {
                    var wp = c - across * 1.2f;
                    for (int k = 0; k < 10; k++)
                    {
                        float a0 = k * Mathf.PI / 5, a1 = (k + 1) * Mathf.PI / 5;
                        mk.Slab(M("Stone"), wp + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * 0.75f, wp + new Vector2(Mathf.Cos(a0), Mathf.Sin(a0)) * 0.75f, 0.22f, g.H - 0.2f, g.H + 0.8f);
                    }
                    mk.Box(M("Iron"), W(wp, g.H + 1.9f), Vector3.right * 0.8f, Vector3.up * 0.03f, Vector3.forward * 0.03f);
                    foreach (int s in new[] { -1, 1 }) mk.Box(M("WoodDark"), W(wp, g.H + 1.3f) + Vector3.right * s * 0.8f, Vector3.right * 0.05f, Vector3.up * 0.6f, Vector3.forward * 0.05f);
                    Kit.Put("Bucket_Wooden_1", props, W(wp + P(0.9f, 0.4f), g.H), Quaternion.identity, null, true, false);
                    // the garden grille onto the path (sec.pozo): iron bars in the wall toward P8f
                    var gate = TerrainBuild.ClosestOnEdge(P(140, 21), g.Poly);
                    var gd = Perp((gate - c).normalized);
                    for (float x = -0.55f; x <= 0.56f; x += 0.12f) mk.Box(M("Iron"), W(gate + gd * x + (gate - c).normalized * 0.25f, g.H + 1.3f), Vector3.right * 0.015f, Vector3.up * 1.0f, Vector3.forward * 0.015f);
                }
            }
        }

        /// Cantabrian meda: straw stacked round a pole.
        void Meda(Vector2 at, float g, float scale)
        {
            var straw = M("Straw");
            var prof = new[] { (1.15f, 0f), (1.3f, 0.55f), (1.2f, 1.35f), (0.85f, 2.05f), (0.35f, 2.55f), (0.05f, 2.72f) };
            const int N = 14;
            for (int k = 0; k < prof.Length - 1; k++)
                for (int i = 0; i < N; i++)
                {
                    float a0 = i * Mathf.PI * 2 / N, a1 = (i + 1) * Mathf.PI * 2 / N;
                    Vector3 R(float r, float y, float a) => W(at + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r * scale, g + y * scale);
                    WallBuild.O(mk, straw, R(prof[k].Item1, prof[k].Item2, a0), R(prof[k].Item1, prof[k].Item2, a1), R(prof[k + 1].Item1, prof[k + 1].Item2, a1), R(prof[k + 1].Item1, prof[k + 1].Item2, a0),
                        W(new Vector2(Mathf.Cos((a0 + a1) / 2), Mathf.Sin((a0 + a1) / 2)), 0.4f));
                }
            mk.Box(M("WoodDark"), W(at, g + 1.75f * scale), Vector3.right * 0.04f, Vector3.up * 1.75f * scale, Vector3.forward * 0.04f);
        }

        void Meadows()
        {
            var r = new Rng(606);
            var b = Bounds(Seed.Hard);
            int n = 0;
            for (float u = b.xMin + 2; u < b.xMax - 2; u += 7f)
                for (float v = b.yMin + 2; v < b.yMax - 2; v += 7f)
                {
                    var p = P(u, v) + new Vector2(r.Range(-2.5f, 2.5f), r.Range(-2.5f, 2.5f));
                    if (!Seed.InHard(p) || Seed.InMask(p) || Seed.WaterDistance(p) < 4.5f || lay.RouteClear(p) < 3f) continue;
                    if (DistPolyEdge(p, Seed.Hard) < 2f) continue;
                    if (Seed.Platforms.Any(pl => InPoly(p, pl.Poly) || DistPolyEdge(p, pl.Poly) < 2f)) continue;
                    if (lay.Houses.Any(h => (h.Center - p).sqrMagnitude < 200 && (InPoly(p, h.Foot) || DistPolyEdge(p, h.Foot) < 3f))) continue;
                    if (lay.Gardens.Any(g => InPoly(p, g.Poly) || DistPolyEdge(p, g.Poly) < 2.5f)) continue;
                    float x = r.Next(); float g0 = G(p);
                    if (x < 0.45f) Kit.Put("CommonTree_" + r.Range(1, 6), props, W(p, g0 - 0.05f), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.45f, 0.62f), true, false);
                    else if (x < 0.6f) Meda(p, g0 - 0.05f, r.Range(0.8f, 1.05f));
                    else if (x < 0.8f) Kit.Put("Bush_Common", props, W(p, g0 - 0.05f), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.5f, 0.8f), true, false);
                    else continue;
                    n++;
                }
            Debug.Log("[Proto] meadow items: " + n);
        }

        void Farmyards()
        {
            foreach (var h in lay.Houses.Where(x => x.Kind == HouseKind.Back))
            {
                var r = new Rng((uint)(h.Seed * 17 + 5));
                var side = HouseBuilder.Edges(h)[r.Chance(0.5f) ? 1 : 3];
                var sd = (side.b - side.a).normalized;
                var at = side.a + sd * ((side.b - side.a).magnitude * 0.5f) + side.outward * 2.2f;
                bool clear = lay.RouteClear(at) > 1.6f && !lay.Houses.Any(o => o != h && (InPoly(at, o.Foot) || DistPolyEdge(at, o.Foot) < 1.6f))
                    && !lay.Gardens.Any(gg => InPoly(at, gg.Poly) || DistPolyEdge(at, gg.Poly) < 1.4f) && Seed.WaterDistance(at) > 3f;
                if (!clear) continue;
                if (h.Style == "barn" && r.Chance(0.75f)) Meda(at, G(at) - 0.05f, r.Range(0.8f, 1.05f));
                else if (h.Style == "shed") { Woodpile(side.a + sd * 1.2f + side.outward * 0.55f, sd, G(side.a + side.outward * 0.55f)); }
                else if (r.Chance(0.5f)) { Put("Barrel", at, r.Range(0, 360f)); Put("Bucket_Wooden_1", at + sd * 0.8f, 0); }
            }
        }

        void Era()
        {
            var c = P(150, 113);
            Kit.Put("CommonTree_3", props, W(c + P(3.5f, 2.5f), G(c + P(3.5f, 2.5f))), Quaternion.Euler(0, 40, 0), Vector3.one * 1.15f, true, true);
            // bolos: nine pins in a square + a ball, and two leaning against the tree
            for (int x = 0; x < 3; x++)
                for (int z = 0; z < 3; z++)
                {
                    var p = c + P(-2.5f + x * 0.9f, -1.5f + z * 0.9f);
                    mk.Box(M("Wood"), W(p, G(p) + 0.22f), Vector3.right * 0.05f, Vector3.up * 0.22f, Vector3.forward * 0.05f);
                }
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.transform.SetParent(props, false); ball.transform.position = W(c + P(3f, -3.5f), G(c + P(3f, -3.5f)) + 0.1f); ball.transform.localScale = Vector3.one * 0.2f;
            ball.GetComponent<Renderer>().sharedMaterial = M("WoodDark");
            Put("Bench", c + P(-5.2f, 3.5f), 150);
        }

        void RiverSide()
        {
            // trees and rocks on the natural banks outside the seed, reeds of grass along Orilla sur
            var r = new Rng(88);
            for (int i = 0; i < 420; i++)
            {
                var p = P(r.Range(-160f, 380f), r.Range(-180f, 300f));
                if (Seed.InHard(p) && DistPolyEdge(p, Seed.Hard) > 3f) continue;
                float wd = Seed.WaterDistance(p);
                if (wd < 2.5f || wd > 26f) continue;
                if (Bridge.InFootprint(p, 3f, out _)) continue;
                float g = tb.Ground(p);
                if (r.Chance(0.5f)) Kit.Put("CommonTree_" + r.Range(1, 6), props, W(p, g - 0.1f), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.7f, 1.15f), true, false);
                else if (r.Chance(0.5f)) Kit.Put("Bush_Common", props, W(p, g - 0.05f), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.8f, 1.4f), true, false);
                else Kit.Put("Rock_Medium_" + r.Range(1, 4), props, W(p, g - 0.3f), Quaternion.Euler(r.Range(-10f, 10f), r.Range(0, 360f), 0), Vector3.one * r.Range(0.6f, 1.6f), true, false);
            }
            // tufts on Orilla sur near the start
            for (int i = 0; i < 90; i++)
            {
                var p = P(r.Range(20f, 90f), r.Range(-78f, -62f));
                if (!Seed.InHard(p) || Seed.InMask(p) || Seed.WaterDistance(p) < 1.5f || Bridge.InFootprint(p, 1f, out _) || InPoly(p, Seed.StubX1O)) continue;
                Kit.Put(r.Chance(0.6f) ? "Grass_Common_Tall" : "Grass_Wispy_Tall", props, W(p, tb.Ground(p)), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.6f, 0.9f), true, false);
            }
            // rocks at the confluence tip under the Casco (the "rock" the town sits on)
            foreach (var p in new[] { P(-12, -6), P(-16, -3), P(-9, 7), P(-15, 2) })
                Kit.Put("Rock_Medium_" + r.Range(1, 4), props, W(p, tb.Ground(p) - 0.4f), Quaternion.Euler(r.Range(-8f, 8f), r.Range(0, 360f), 0), Vector3.one * r.Range(0.7f, 1.1f), true, false);
        }

        /// Riverside walk inside the seed: a row of trees and a few benches on the free bank tops of the Casco.
        void RiverWalk()
        {
            var r = new Rng(606);
            var line = Resample(new[] { P(22, -26), P(40, -38.5f), P(62, -41.5f), P(95, -40.5f), P(140, -37.6f), P(185, -35.6f), P(203, -34) }, 1f);
            float next = 0;
            var cum = Cum(line);
            for (int i = 0; i < line.Length; i++)
            {
                if (cum[i] < next) continue;
                var p = line[i];
                if (Bridge.InFootprint(p, 4f, out _) || lay.RouteClear(p) < 1.2f || Seed.InMask(p) || Seed.WaterDistance(p) < 1.3f) continue;
                bool blocked = lay.Houses.Any(h => (h.Center - p).sqrMagnitude < 150 && DistPolyEdge(p, h.Foot) < 2.5f) || lay.Gardens.Any(g => InPoly(p, g.Poly) || DistPolyEdge(p, g.Poly) < 1.5f);
                if (blocked) continue;
                float g = G(p);
                Kit.Put("CommonTree_" + r.Range(1, 6), props, W(p, g - 0.05f), Quaternion.Euler(0, r.Range(0, 360f), 0), Vector3.one * r.Range(0.62f, 0.8f), true, true);
                if (r.Chance(0.3f)) Put("Bench", p + P(0, 1.8f), 180 + r.Range(-8f, 8f));
                next = cum[i] + r.Range(8f, 11f);
            }
        }

        void Seams()
        {
            // Calle Mayor seam (W04 east end): parked cart + crates where the playable street stops
            Put("Prop_Wagon", P(206.5f, 71.5f), 100);
            Put("Crate_Wooden", P(206.0f, 68.4f), 20); Put("Crate_Wooden", P(206.3f, 69.4f), 70, 0.9f, true, 0.0f);
            // Ensanche lane end and the camino sur: timber fences
            foreach (var (p, yaw) in new[] { (P(14.5f, 66.2f), 45f), (P(13.2f, 64.9f), 45f), (P(30.5f, -73.8f), 100f), (P(28.5f, -74.2f), 100f) })
                Put("Prop_WoodenFence_Single", p, yaw);
        }
    }
}
