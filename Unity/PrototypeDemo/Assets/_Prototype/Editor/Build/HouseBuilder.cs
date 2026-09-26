using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Layer 3-4: platform/plinth + kit wall modules with real openings + inserts + roof, per the assembly grammar:
    /// site datum -> plinth -> storeys -> hosted wall modules -> openings/inserts -> roof + gables -> thresholds.
    public class HouseBuilder
    {
        readonly TerrainBuild tb;
        readonly Layout lay;
        public readonly MeshKit Plinths = new MeshKit { Tile = 1.6f };
        public const float RoofFlatten = 0.55f;
        static readonly int[] RoofSpans = { 4, 6, 8 };

        public HouseBuilder(TerrainBuild t, Layout l) { tb = t; lay = l; }

        public struct Edge { public Vector2 a, b, outward; public int idx; }

        public static Edge[] Edges(HousePlan h) => new[]
        {
            new Edge { a = h.A, b = h.B, outward = -h.In, idx = 0 },
            new Edge { a = h.B, b = h.C, outward = h.Dir, idx = 1 },
            new Edge { a = h.C, b = h.Dd, outward = h.In, idx = 2 },
            new Edge { a = h.Dd, b = h.A, outward = -h.Dir, idx = 3 },
        };

        static Quaternion Face(Vector2 outward) => Quaternion.LookRotation(new Vector3(outward.x, 0, outward.y), Vector3.up);

        public static Vector3 ModulePos(Edge e, int k, float y)
        {
            var d = (e.b - e.a).normalized;
            return W(e.a + d * (1 + 2 * k), y);
        }

        bool Covered(HousePlan self, Vector2 p, float top)
        {
            foreach (var o in lay.Houses)
                if (o != self && o.Top >= top - 0.05f && (o.Center - p).sqrMagnitude < 150 && InPoly(p, o.Foot)) return true;
            return false;
        }

        public GameObject Build(HousePlan h, Transform parent, bool layer4)
        {
            var rng = new Rng((uint)(h.Seed * 2654435761u + 1));
            var root = new GameObject($"{h.Id}_{h.Kind}{(h.Name != null ? "_" + h.Name : "")}").transform;
            root.SetParent(parent, false);
            root.gameObject.isStatic = true;

            // ---- layer 3: plinth (zócalo) down to the lowest ground + door steps
            BuildPlinth(h);
            if (!layer4) return root.gameObject;

            bool plaster = false;   // kit plaster walls carry timber framing (vetoed): stone only
            bool wide = rng.Chance(0.7f);
            int doorN = rng.Range(1, 9);
            bool roundDoor = rng.Chance(0.25f) || h.Kind == HouseKind.Civic;
            bool solana = h.Balcony && h.W >= 8 && rng.Chance(0.45f);
            var shutterMat = MatLib.Get("ShutterGreen");
            var edges = Edges(h);
            int F = h.Floors;
            bool tower = h.Kind == HouseKind.Tower, arcade = h.Kind == HouseKind.Arcade;

            for (int f = 0; f < F; f++)
            {
                float y = h.FL + 3f * f;
                foreach (var e0 in edges)
                {
                    var e = e0;
                    if (arcade && f == 0)
                    {
                        // arcade: ground floor wall set back 2 m behind stone pillars
                        if (e.idx == 0) { e.a = h.A + h.In * 2; e.b = h.B + h.In * 2; }
                        if (e.idx == 1) e.b = h.C; if (e.idx == 1) e.a = h.B + h.In * 2;
                        if (e.idx == 3) { e.b = h.A + h.In * 2; }
                    }
                    float L = (e.b - e.a).magnitude;
                    int count = Mathf.RoundToInt(L / 2f);
                    var rot = Face(e.outward);
                    for (int k = 0; k < count; k++)
                    {
                        var pos = ModulePos(e, k, y);
                        var probe = UV(pos) + e.outward * 0.45f;
                        if (Covered(h, probe, y + 3f)) continue;
                        Module(h, root, e, k, count, f, F, pos, rot, rng, plaster, wide, doorN, roundDoor, solana, shutterMat, tower);
                    }
                }
                // quoins
                var corners = new[] { (h.A, h.Dir, h.In), (h.B, -h.Dir, h.In), (h.C, -h.Dir, -h.In), (h.Dd, h.Dir, -h.In) };
                foreach (var (p, a, b) in corners)
                {
                    if (arcade && f == 0 && (p == h.A || p == h.B)) continue;
                    if (Covered(h, p + (-a - b).normalized * 0.4f, y + 3f)) continue;
                    var up = Vector3.up;
                    var fa = new Vector3(a.x, 0, a.y); var fb = new Vector3(b.x, 0, b.y);
                    Quaternion r = Vector3.Dot(Vector3.Cross(up, fb), fa) > 0.5f ? Quaternion.LookRotation(fb, up) : Quaternion.LookRotation(fa, up);
                    Kit.Put("Corner_Exterior_Brick", root, W(p, y), r);
                }
            }
            if (arcade) Arcade(h, root);
            Roof(h, root, rng, tower);
            return root.gameObject;
        }

        void Module(HousePlan h, Transform root, Edge e, int k, int count, int f, int F, Vector3 pos, Quaternion rot, Rng rng,
            bool plaster, bool wide, int doorN, bool roundDoor, bool solana, Material shutterMat, bool tower)
        {
            string mat = (f > 0 && plaster) ? "Plaster" : "UnevenBrick";
            bool front = e.idx == 0;
            bool top = f == F - 1;
            bool shop = h.Kind == HouseKind.P9 || h.Kind == HouseKind.Shop || h.Kind == HouseKind.Bar;
            string wall = $"Wall_{mat}_Straight";
            string insert = null, shutters = null, door = null, frame = null, balcony = null;
            bool openDoor = false;

            if (tower)
            {
                bool mid = k == count / 2;
                if (f == 0 && front && mid) { wall = "Wall_UnevenBrick_Door_Round"; frame = "DoorFrame_Round_Brick"; door = "Door_6_Round"; }
                else if (top && mid) wall = "Wall_UnevenBrick_Window_Wide_Round";            // open belfry
                else if (mid && f % 2 == 1) { wall = "Wall_UnevenBrick_Window_Thin_Round"; insert = "Window_Thin_Round2"; }
            }
            else if (f == 0 && front && k == h.DoorModule)
            {
                wall = roundDoor ? "Wall_UnevenBrick_Door_Round" : "Wall_UnevenBrick_Door_Flat";
                frame = roundDoor ? "DoorFrame_Round_Brick" : "DoorFrame_Flat_Brick";
                door = roundDoor ? $"Door_{doorN}_Round" : $"Door_{doorN}_Flat";
                openDoor = h.Enterable;
            }
            else if (f == 0 && front)
            {
                if (shop || rng.Chance(0.55f)) { wall = "Wall_UnevenBrick_Window_Wide_Flat"; insert = "Window_Wide_Flat1"; if (!shop && rng.Chance(0.6f)) shutters = "WindowShutters_Wide_Flat_" + (rng.Chance(0.5f) ? "Closed" : "Open"); }
            }
            else if (h.Kind == HouseKind.Bar && e.idx == 2)
            {
                // back of the bar: plain ground floor behind the patio stair, a door onto the stair landing upstairs
                if (f == 1 && k == 2) { wall = "Wall_UnevenBrick_Door_Flat"; frame = "DoorFrame_Flat_Brick"; door = "Door_4_Flat"; }
            }
            else if (f == 0)
            {
                // service door of the bar towards W13
                if (h.Kind == HouseKind.Bar && e.idx == 3 && k == 0) { wall = "Wall_UnevenBrick_Door_Flat"; frame = "DoorFrame_Flat_Brick"; door = "Door_3_Flat"; }
                else if (rng.Chance(0.18f)) { wall = "Wall_UnevenBrick_Window_Thin_Round"; insert = "Window_Thin_Round1"; }
            }
            else if (front)
            {
                bool balc = h.Balcony && top && F >= 2 && (solana || (k == count / 2) || (count >= 4 && k == count / 2 - 1));
                if (balc)
                {
                    wall = $"Wall_{mat}_Door_Flat"; door = "Door_1_Flat"; balcony = "Balcony_Simple_Straight";
                }
                else if (count >= 4 && rng.Chance(0.12f)) { }
                else if (wide || mat == "Plaster")
                {
                    wall = $"Wall_{mat}_Window_Wide_Flat"; insert = "Window_Wide_Flat" + (1 + rng.Range(0, 2));
                    if (rng.Chance(0.8f)) shutters = "WindowShutters_Wide_Flat_" + (rng.Chance(0.55f) ? "Open" : "Closed");
                }
                else
                {
                    wall = $"Wall_{mat}_Window_Thin_Round"; insert = "Window_Thin_Round" + (1 + rng.Range(0, 2));
                    if (rng.Chance(0.8f)) shutters = "WindowShutters_Thin_Round_" + (rng.Chance(0.55f) ? "Open" : "Closed");
                }
            }
            else
            {
                float p = e.idx == 2 ? 0.5f : 0.3f;
                if (k > 0 && k < count - 1 && rng.Chance(p))
                {
                    wall = $"Wall_{mat}_Window_Wide_Flat"; insert = "Window_Wide_Flat1";
                    if (rng.Chance(0.6f)) shutters = "WindowShutters_Wide_Flat_Closed";
                }
            }

            Kit.Put(wall, root, pos, rot);
            if (insert != null) Kit.Put(insert, root, pos, rot);
            if (frame != null) Kit.Put(frame, root, pos, rot);
            if (shutters != null) { var s = Kit.Put(shutters, root, pos, rot); if (s) Kit.Tint(s, "MI_WoodTrim_Wear", shutterMat); }
            if (door != null)
            {
                var right = rot * Vector3.right;
                var d = Kit.Put(door, root, pos + right * 0.5f, rot);
                if (d != null && openDoor)
                {
                    // swing inward around its hinge so the doorway is walkable
                    d.transform.RotateAround(pos + right * 0.5f, Vector3.up, 95f);
                    foreach (var c in d.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(c);
                }
            }
            if (balcony != null) Kit.Put(balcony, root, pos, rot);
        }

        void BuildPlinth(HousePlan h)
        {
            var stone = MatLib.Get("StoneDark"); var cap = MatLib.Get("Coping");
            float top = h.FL + 0.5f;
            foreach (var e in Edges(h))
            {
                bool arcadeFront = h.Kind == HouseKind.Arcade && e.idx == 0;
                if (arcadeFront) continue;
                var d = (e.b - e.a).normalized;
                float L = (e.b - e.a).magnitude;
                // spans (skip the door opening on the front)
                var spans = new List<(float s0, float s1)>();
                if (e.idx == 0 && h.DoorModule >= 0 && h.Kind != HouseKind.Tower)
                {
                    float dc = 1 + 2 * h.DoorModule;
                    spans.Add((-0.16f, dc - 0.8f)); spans.Add((dc + 0.8f, L + 0.16f));
                }
                else if (h.Kind == HouseKind.Bar && e.idx == 3) { spans.Add((-0.16f, 0.2f)); spans.Add((1.8f, L + 0.16f)); }
                else spans.Add((-0.16f, L + 0.16f));
                foreach (var (s0, s1) in spans)
                {
                    if (s1 - s0 < 0.1f) continue;
                    Vector2 a = e.a + d * s0, b = e.a + d * s1;
                    float g = Mathf.Min(tb.Ground(a + e.outward * 0.4f), tb.Ground(b + e.outward * 0.4f), tb.Ground((a + b) * 0.5f + e.outward * 0.4f));
                    float bottom = Mathf.Min(g, h.FL) - 0.5f;
                    var o = e.outward * 0.15f;
                    // box: from face (outward 0.15) to 0.4 inside
                    Plinths.Slab(stone, a + o, b + o, 0.55f, bottom, top);
                    Plinths.Slab(cap, a + o + e.outward * 0.04f, b + o + e.outward * 0.04f, 0.22f, top, top + 0.06f);
                }
            }
            // door step(s)
            if (h.DoorModule >= 0)
            {
                var e = Edges(h)[0];
                var d = (e.b - e.a).normalized;
                var c = e.a + d * (1 + 2 * h.DoorModule);
                float g = tb.Ground(c + e.outward * 0.9f);
                float rise = h.FL - g;
                int steps = Mathf.Clamp(Mathf.CeilToInt(rise / 0.18f), 1, 5);
                for (int i = 0; i < steps; i++)
                {
                    float t0 = h.FL - i * rise / steps;
                    var p0 = c + e.outward * (0.2f + (i + 1) * 0.32f);
                    Plinths.Slab(stone, p0 - d * 0.9f, p0 + d * 0.9f, 0.32f + (i == 0 ? 0.05f : 0), g - 0.4f, t0 - 0.01f);
                }
            }
        }

        static Bounds LocalBounds(GameObject go)
        {
            var b = new Bounds(Vector3.zero, Vector3.zero); bool first = true;
            foreach (var mf in go.GetComponentsInChildren<MeshFilter>())
            {
                var m = mf.sharedMesh; if (m == null) continue;
                foreach (var v in m.vertices)
                {
                    var lp = go.transform.InverseTransformPoint(mf.transform.TransformPoint(v));
                    lp = Vector3.Scale(lp, go.transform.localScale);
                    if (first) { b = new Bounds(lp, Vector3.zero); first = false; } else b.Encapsulate(lp);
                }
            }
            return b;
        }

        /// Pentagon prism under the roof: wall line at gp, facing "outward", spanning spanDir.
        void Gable(Vector2 gp, Vector2 outward, Vector2 spanDir, int span, float top, Bounds roofLocal)
        {
            var stone = MatLib.Get("Stone");
            float half = span * 0.5f;
            float halfExt = Mathf.Max(roofLocal.extents.x, half + 0.1f);
            float ridge = top + roofLocal.max.y, eave = top + roofLocal.min.y;
            float Y(float x) => ridge - (ridge - eave) * Mathf.Abs(x) / halfExt - 0.32f;
            float yCorner = Mathf.Max(top + 0.05f, Y(half));
            var face = gp + outward * 0.07f;
            var back = face - outward * 0.35f;
            Vector3 P(Vector2 at, float x, float y) => W(at + spanDir * x, y);
            var fo = new Vector3(outward.x, 0, outward.y);
            var pts = new[] { (-half, top), (half, top), (half, yCorner), (0f, Y(0)), (-half, yCorner) };
            // front and back faces (fan from the centre bottom)
            for (int i = 0; i < pts.Length; i++)
            {
                var a = pts[i]; var b = pts[(i + 1) % pts.Length];
                // side band (quad) between front and back
                WallBuild.O(Plinths, stone, P(face, a.Item1, a.Item2), P(face, b.Item1, b.Item2), P(back, b.Item1, b.Item2), P(back, a.Item1, a.Item2),
                    (P(face, (a.Item1 + b.Item1) * 0.5f, (a.Item2 + b.Item2) * 0.5f) - P(face, 0, top + (Y(0) - top) * 0.4f)).normalized);
            }
            var cF = P(face, 0, top + (Y(0) - top) * 0.4f); var cB = P(back, 0, top + (Y(0) - top) * 0.4f);
            for (int i = 0; i < pts.Length; i++)
            {
                var a = pts[i]; var b = pts[(i + 1) % pts.Length];
                TriO(Plinths, stone, cF, P(face, a.Item1, a.Item2), P(face, b.Item1, b.Item2), fo);
                TriO(Plinths, stone, cB, P(back, a.Item1, a.Item2), P(back, b.Item1, b.Item2), -fo);
            }
        }

        static void TriO(MeshKit mk, Material m, Vector3 a, Vector3 b, Vector3 c, Vector3 want)
        {
            if (Vector3.Dot(Vector3.Cross(b - a, c - a), want) >= 0) mk.Tri(m, a, b, c); else mk.Tri(m, a, c, b);
        }

        void Arcade(HousePlan h, Transform root)
        {
            var stone = MatLib.Get("Stone"); var cap = MatLib.Get("Coping");
            for (int k = 0; k <= h.W / 2; k++)
            {
                var p = h.A + h.Dir * (2 * k);
                p += h.In * 0.3f + h.Dir * (k == 0 ? 0.3f : k == h.W / 2 ? -0.3f : 0);
                Plinths.Box(stone, W(p, h.FL + 1.35f), Vector3.right * 0.3f, Vector3.up * 1.6f, Vector3.forward * 0.3f);
                Plinths.Box(cap, W(p, h.FL + 0.05f), Vector3.right * 0.38f, Vector3.up * 0.1f, Vector3.forward * 0.38f);
            }
            // lintel + arcade ceiling + walkable floor
            var la = h.A; var lb = h.B;
            Plinths.Slab(stone, la, lb, 0.6f, h.FL + 2.65f, h.FL + 3.02f);
            Plinths.Slab(MatLib.Get("WoodDark"), la + h.In * 0.6f, lb + h.In * 0.6f, 1.4f, h.FL + 2.86f, h.FL + 2.96f);
            Plinths.Slab(MatLib.Get("StoneDark"), la, lb, 2.05f, h.FL - 0.6f, h.FL - 0.02f);
        }

        void Roof(HousePlan h, Transform root, Rng rng, bool tower)
        {
            var c = h.Center;
            if (tower)
            {
                Kit.Put("Roof_Tower_RoundTiles", root, W(c, h.Top), Face(-h.In), new Vector3(1.28f, 0.72f, 1.28f));
                return;
            }
            // option A: ridge along the front (eaves to street); option B: gable to street
            var opts = new List<(int span, int len, Vector2 ridge, Vector2 spanDir)>
            {
                (h.D, h.W, h.Dir, h.In),
                (h.W, h.D, h.In, h.Dir),
            };
            if (h.GableToStreet) opts.Reverse();
            (int span, int len, Vector2 ridge, Vector2 spanDir) pick = opts[0];
            string name = null;
            foreach (var o in opts)
                if (Kit.Has($"Roof_RoundTiles_{o.span}x{o.len}")) { pick = o; name = $"Roof_RoundTiles_{o.span}x{o.len}"; break; }
            float sx = 1, sz = 1;
            if (name == null)
            {
                // nearest available, scaled to fit exactly
                var o = opts[0];
                int bs = RoofSpans.OrderBy(s => Mathf.Abs(s - o.span)).First();
                var lens = Kit.Names($"Roof_RoundTiles_{bs}x").Select(n => int.Parse(n.Split('x')[1])).ToList();
                int bl = lens.OrderBy(l => Mathf.Abs(l - o.len)).First();
                name = $"Roof_RoundTiles_{bs}x{bl}";
                sx = (float)o.span / bs; sz = (float)o.len / bl; pick = o;
            }
            var rot = Quaternion.LookRotation(new Vector3(pick.ridge.x, 0, pick.ridge.y), Vector3.up);
            var roof = Kit.Put(name, root, W(c, h.Top), rot, new Vector3(sx, RoofFlatten, sz));
            // stone gable ends following the measured roof slope (kit gables are timber-framed plaster)
            var lb = LocalBounds(roof);
            foreach (int sgn in new[] { 1, -1 })
            {
                var gp = c + pick.ridge * (pick.len * 0.5f) * sgn;
                if (Covered(h, gp + pick.ridge * sgn * 0.5f, h.Top + 1)) continue;
                Gable(gp, pick.ridge * sgn, pick.spanDir, pick.span, h.Top, lb);
            }
            // chimney
            if (rng.Chance(0.75f))
            {
                var cp = c + pick.ridge * (pick.len * 0.22f * (rng.Chance(0.5f) ? 1 : -1)) + pick.spanDir * 0.7f;
                Kit.Put(rng.Chance(0.5f) ? "Prop_Chimney" : "Prop_Chimney2", root, W(cp, h.Top + 0.3f), rot);
            }
        }
    }
}
