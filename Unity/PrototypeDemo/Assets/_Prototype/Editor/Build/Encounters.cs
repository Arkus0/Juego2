using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Layer 5: meetings between systems — bridge/bank, ford, slipway, door/street, bar inside/outside, civic thresholds.
    public class Encounters
    {
        readonly TerrainBuild tb; readonly Layout lay; readonly Transform root;
        readonly MeshKit mk = new MeshKit { Tile = 1.6f };
        readonly SignKit sk;
        public Encounters(TerrainBuild t, Layout l, Transform r) { tb = t; lay = l; root = r; sk = new SignKit(mk); }

        Material M(string n) => MatLib.Get(n);

        public void BuildAll()
        {
            Bridge.Build(root, tb.Ground);
            FloodStela();
            Ford();
            Slipway();
            BarInterior();
            BarPatio();
            new Interiors(tb, lay, root, mk, sk).BuildAll();
            Civic();
            Signs();
            Hornacina();
            PuertaCegada();
            RendijaRail();
            CourtStair();
            mk.Build("Encuentros_Piedra_Madera", root, true);
        }

        // ------------------------------------------------------------------ riada
        void FloodStela()
        {
            var p = Bridge.At(0.4f, Bridge.HalfW + 0.9f);
            float g = Seed.HX1;
            var d = Bridge.Dir; var n = Perp(d);
            var c = W(p, g + 1.25f);
            var r = new Vector3(n.x, 0, n.y) * 0.5f; var f = new Vector3(d.x, 0, d.y) * 0.2f;
            mk.Box(M("Coping"), c, r, Vector3.up * 1.25f, f);
            // incised flood lines (dark, slightly proud) on the face towards the bridgehead (-d)
            var face = W(p - d * 0.21f, 0);
            float[] hs = { 0.55f, 0.95f, 1.45f, 1.9f };
            foreach (var hh in hs)
            {
                var y = g + hh;
                mk.Box(M("Iron"), new Vector3(face.x, y, face.z), r * 0.8f, Vector3.up * 0.018f, f * 0.06f);
                mk.Box(M("Iron"), new Vector3(face.x, y + 0.06f, face.z) + r * 0.62f, r * 0.08f, Vector3.up * 0.06f, f * 0.06f);
            }
            sk.Face("plaque_riadas", W(p - d * 0.215f, g + 2.26f), W(-d, 0), 0.78f, 0.225f);
        }

        // ------------------------------------------------------------------ X5 ford
        void Ford()
        {
            var fs = Perp(Stamps.FordDir);
            var wet = M("StoneWet"); var stone = M("StoneDark");
            var rng = new Rng(55);
            // stepping stones across the water
            for (float s = Stamps.FordWetIn + 0.35f; s < Stamps.FordWetOut - 0.2f; s += 0.82f)
            {
                var p = Stamps.Ford(s, rng.Range(-0.18f, 0.18f));
                var rot = Quaternion.AngleAxis(rng.Range(-12f, 12f), Vector3.up);
                var r = rot * W(fs, 0) * rng.Range(0.34f, 0.42f); var fw = rot * W(Stamps.FordDir, 0) * 0.3f;
                mk.Box(wet, W(p, (Stamps.StoneTop - 1.8f) * 0.5f), r, Vector3.up * (Stamps.StoneTop + 1.8f) * 0.5f, fw);
            }
            // approach steps on both banks
            for (int side = 0; side < 2; side++)
            {
                float s0 = side == 0 ? Stamps.FordWetIn - Stamps.FordStepLen : Stamps.FordWetOut;
                float s1 = side == 0 ? Stamps.FordWetIn : Stamps.FordWetOut + Stamps.FordStepLen;
                for (float s = s0; s < s1 - 0.01f; s += 0.3f)
                {
                    float h = Stamps.FordApproach(s + 0.15f, Seed.HX5);
                    mk.Slab(stone, Stamps.Ford(s, -1.1f), Stamps.Ford(s, 1.1f), -0.3f, h - 1.2f, h);
                }
                // low cheek walls
                foreach (int t in new[] { -1, 1 })
                    mk.Slab(M("Stone"), Stamps.Ford(s0, t * 1.1f), Stamps.Ford(s1, t * 1.1f), t * 0.4f, -1f, Seed.HX5 + 0.35f);
            }
            // flood layer + high-water blocker (runtime component toggles them)
            var flood = FloodMesh();
            var blocker = new GameObject("X5_HighWaterBlocker");
            blocker.transform.SetParent(root, false);
            var mid = Stamps.Ford((Stamps.FordWetIn + Stamps.FordWetOut) * 0.5f);
            blocker.transform.position = W(mid, 1.5f);
            blocker.transform.rotation = Quaternion.LookRotation(W(Stamps.FordDir, 0));
            var bc = blocker.AddComponent<BoxCollider>();
            bc.size = new Vector3(3.2f, 4f, Stamps.FordWetOut - Stamps.FordWetIn + 0.6f);
            var ford = new GameObject("X5_Vado (F: aguas bajas/altas)").AddComponent<Proto.Runtime.Ford>();
            ford.transform.SetParent(root, false);
            ford.transform.position = W(mid, 2f);
            ford.flood = flood.transform;
            ford.blocker = bc;
            blocker.SetActive(false);
            flood.SetActive(false);
        }

        GameObject FloodMesh()
        {
            // Arroyo polygon at "high water" (heights taper to 0 at the confluence, animated by the Ford component)
            var poly = Seed.Arroyo;
            var dense = Resample(poly.Concat(new[] { poly[0] }).ToArray(), 2f).Take(9999).ToArray();
            dense = dense.Take(dense.Length - 1).ToArray();
            var tris = Triangulate(dense);
            var verts = dense.Select(q => W(q, Smooth(-5, 40, q.x))).ToArray();
            var mesh = new Mesh { name = "ArroyoFlood" };
            mesh.vertices = verts;
            mesh.triangles = tris.ToArray().Reverse().ToArray();
            mesh.RecalculateNormals();
            if (mesh.normals.Length > 0 && mesh.normals[0].y < 0) { mesh.triangles = tris.ToArray(); mesh.RecalculateNormals(); }
            mesh.RecalculateBounds();
            MeshKit.Meshes.Add(mesh);
            var go = new GameObject("Arroyo_AguasAltas");
            go.transform.SetParent(root, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = M("Water");
            mr.shadowCastingMode = ShadowCastingMode.Off;
            return go;
        }

        // ------------------------------------------------------------------ landing slipway + barca
        void Slipway()
        {
            var d = Stamps.SlipDir; var n = Perp(d);
            int steps = 6;
            for (int i = 0; i < steps; i++)
            {
                float s0 = Stamps.SlipLen * i / steps, s1 = Stamps.SlipLen * (i + 1) / steps;
                float h = Mathf.Lerp(Stamps.SlipTopH, Stamps.SlipBottomH, (i + 1f) / steps);
                mk.Slab(M("StoneDark"), Stamps.SlipTop + d * s0 - n * 2.6f, Stamps.SlipTop + d * s0 + n * 2.6f, -(s1 - s0) - 0.02f, -1.2f, h);
            }
            foreach (int t in new[] { -1, 1 })
                mk.Slab(M("Stone"), Stamps.SlipTop + n * t * 2.6f, Stamps.SlipTop + d * (Stamps.SlipLen + 0.3f) + n * t * 2.6f, t * 0.5f, -1.5f, Stamps.SlipTopH + 0.25f);
            // bollards
            foreach (int t in new[] { -1, 1 })
                mk.Box(M("Coping"), W(Stamps.SlipTop + n * t * 3.4f, Stamps.SlipTopH + 0.35f), Vector3.right * 0.18f, Vector3.up * 0.35f, Vector3.forward * 0.18f);
            Boat(Stamps.SlipTop + d * (Stamps.SlipLen * 0.55f) + n * 0.8f, Quaternion.LookRotation(W(d, 0)) * Quaternion.Euler(-9, 12, 4));
        }

        void Boat(Vector2 at, Quaternion rot)
        {
            var bk = new MeshKit { Tile = 1f };
            var wood = M("WoodDark");
            float L = 4.4f; int n = 10;
            Vector3 Pt(float x, float w, float y) => new Vector3(w, y, x);
            for (int i = 0; i < n; i++)
            {
                float x0 = -L / 2 + L * i / n, x1 = -L / 2 + L * (i + 1) / n;
                float w0 = 0.7f * (1 - Mathf.Pow(Mathf.Abs(2 * x0 / L), 3)), w1 = 0.7f * (1 - Mathf.Pow(Mathf.Abs(2 * x1 / L), 3));
                float b0 = 0.05f + 0.25f * Mathf.Pow(Mathf.Abs(2 * x0 / L), 4), b1 = 0.05f + 0.25f * Mathf.Pow(Mathf.Abs(2 * x1 / L), 4);
                // hull sides (double-sided) + bottom
                foreach (int sg in new[] { -1, 1 })
                {
                    var a = Pt(x0, sg * w0, 0.62f); var b = Pt(x1, sg * w1, 0.62f); var c = Pt(x1, sg * w1 * 0.55f, b1); var dd = Pt(x0, sg * w0 * 0.55f, b0);
                    WallBuild.O(bk, wood, a, b, c, dd, new Vector3(sg, -0.3f, 0));
                    WallBuild.O(bk, wood, a, dd, c, b, new Vector3(-sg, 0.3f, 0));
                }
                WallBuild.O(bk, wood, Pt(x0, -w0 * 0.55f, b0), Pt(x1, -w1 * 0.55f, b1), Pt(x1, w1 * 0.55f, b1), Pt(x0, w0 * 0.55f, b0), Vector3.down);
                WallBuild.O(bk, wood, Pt(x0, -w0 * 0.55f, b0 + 0.02f), Pt(x1, -w1 * 0.55f, b1 + 0.02f), Pt(x1, w1 * 0.55f, b1 + 0.02f), Pt(x0, w0 * 0.55f, b0 + 0.02f), Vector3.up);
            }
            bk.Box(wood, new Vector3(0, 0.42f, 0.4f), Vector3.right * 0.62f, Vector3.up * 0.03f, Vector3.forward * 0.14f);
            bk.Box(wood, new Vector3(0, 0.42f, -0.9f), Vector3.right * 0.55f, Vector3.up * 0.03f, Vector3.forward * 0.14f);
            var go = bk.Build("Barca_Varada", root, true);
            go.transform.position = W(at, Stamps.SlipTopH - 0.55f);
            go.transform.rotation = rot;
            var oar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            oar.name = "Remo"; oar.transform.SetParent(go.transform, false);
            oar.transform.localPosition = new Vector3(0.2f, 0.55f, -0.2f);
            oar.transform.localRotation = Quaternion.Euler(84, 20, 0);
            oar.transform.localScale = new Vector3(0.06f, 1.4f, 0.06f);
            oar.GetComponent<Renderer>().sharedMaterial = M("Wood");
            Object.DestroyImmediate(oar.GetComponent<Collider>());
        }

        // ------------------------------------------------------------------ bar F01
        HousePlan H(string id) => lay.Houses.First(h => h.Id == id);

        void BarInterior()
        {
            var h = H("F01");
            float fl = h.FL;
            float u0 = 88.32f, u1 = 95.68f, v0 = 45.92f, v1 = 55.28f, part = 52.9f;
            var wood = M("WoodDark"); var plaster = M("Plaster"); var stone = M("Stone");
            // floor (wood) and ceiling with beams
            mk.Box(wood, new Vector3((u0 + u1) / 2, fl - 0.04f, (v0 + v1) / 2), Vector3.right * (u1 - u0) / 2, Vector3.up * 0.05f, Vector3.forward * (v1 - v0) / 2);
            mk.Box(wood, new Vector3((u0 + u1) / 2, fl + 3.0f, (v0 + v1) / 2), Vector3.right * (u1 - u0) / 2, Vector3.up * 0.06f, Vector3.forward * (v1 - v0) / 2);
            for (float v = v0 + 0.9f; v < v1; v += 1.5f)
                mk.Box(wood, new Vector3((u0 + u1) / 2, fl + 2.84f, v), Vector3.right * (u1 - u0) / 2, Vector3.up * 0.11f, Vector3.forward * 0.1f);
            // partition with the closed service door (public | service)
            float du0 = 92.35f, du1 = 93.45f;
            mk.Box(plaster, new Vector3((u0 + du0) / 2, fl + 1.5f, part), Vector3.right * (du0 - u0) / 2, Vector3.up * 1.5f, Vector3.forward * 0.1f);
            mk.Box(plaster, new Vector3((du1 + u1) / 2, fl + 1.5f, part), Vector3.right * (u1 - du1) / 2, Vector3.up * 1.5f, Vector3.forward * 0.1f);
            mk.Box(plaster, new Vector3((du0 + du1) / 2, fl + 2.6f, part), Vector3.right * (du1 - du0) / 2, Vector3.up * 0.4f, Vector3.forward * 0.1f);
            Kit.Put("Door_2_Flat", root, W(P(du0 + 0.02f, part - 0.02f), fl), Quaternion.LookRotation(Vector3.back));
            sk.FacadeBoard("private", W(P((du0 + du1) / 2, part - 0.1f), fl + 2.36f), Vector3.back, 0.5f, 0.17f);
            // counter (barra) along the east side
            float cu0 = 93.3f, cu1 = 94.0f, cv0 = 46.9f, cv1 = 51.4f;
            mk.Box(wood, new Vector3((cu0 + cu1) / 2, fl + 0.52f, (cv0 + cv1) / 2), Vector3.right * (cu1 - cu0) / 2, Vector3.up * 0.52f, Vector3.forward * (cv1 - cv0) / 2);
            mk.Box(M("Coping"), new Vector3((cu0 + cu1) / 2 - 0.06f, fl + 1.07f, (cv0 + cv1) / 2), Vector3.right * ((cu1 - cu0) / 2 + 0.1f), Vector3.up * 0.04f, Vector3.forward * ((cv1 - cv0) / 2 + 0.05f));
            mk.Box(wood, new Vector3(cu0 + 0.35f, fl + 0.52f, cv0 - 0.35f), Vector3.right * 0.35f, Vector3.up * 0.52f, Vector3.forward * 0.35f);
            // back shelves
            for (int i = 0; i < 3; i++)
                mk.Box(wood, new Vector3(u1 - 0.15f, fl + 1.3f + i * 0.45f, (cv0 + cv1) / 2), Vector3.right * 0.15f, Vector3.up * 0.025f, Vector3.forward * ((cv1 - cv0) / 2 - 0.3f));
            var props = new GameObject("Bar_Props").transform; props.SetParent(root, false);
            var rng = new Rng(9);
            foreach (var v in new[] { 47.6f, 48.9f, 50.2f })
                Kit.Put("Stool", props, W(P(cu0 - 0.55f, v), fl), Quaternion.identity);
            for (float v = cv0 + 0.3f; v < cv1; v += 0.7f) Kit.Put("Mug", props, W(P(cu0 + 0.2f, v), fl + 1.11f), Quaternion.Euler(0, rng.Range(0, 360f), 0), null, false, false);
            Kit.Put("Barrel", props, W(P(u1 - 0.5f, 52.1f), fl), Quaternion.identity);
            Kit.Put("Barrel", props, W(P(u1 - 0.5f, 46.4f), fl), Quaternion.identity);
            // tables
            foreach (var (u, v) in new[] { (89.9f, 48.0f), (89.9f, 51.2f) })
            {
                Table(P(u, v), fl, 0.7f, 1.3f);
                Kit.Put("Chair_1", props, W(P(u - 1.05f, v), fl), Quaternion.Euler(0, 90, 0));
                Kit.Put("Chair_1", props, W(P(u + 1.05f, v), fl), Quaternion.Euler(0, -90, 0));
                Kit.Put("Mug", props, W(P(u + 0.15f, v - 0.2f), fl + 0.77f), Quaternion.identity, null, false, false);
                Kit.Put("CandleStick", props, W(P(u - 0.1f, v + 0.25f), fl + 0.77f), Quaternion.identity, null, false, false);
            }
            // warm light
            foreach (var (u, v, i) in new[] { (91.5f, 48.2f, 3.2f), (91.5f, 51.4f, 2.6f) })
            {
                var l = new GameObject("BarLight").AddComponent<Light>();
                l.transform.SetParent(root, false);
                l.transform.position = W(P(u, v), fl + 2.5f);
                l.type = LightType.Point; l.range = 7f; l.intensity = i; l.color = new Color(1f, 0.74f, 0.45f);
                l.shadows = LightShadows.Soft;
            }
            var light = new GameObject("BarDoorGlow").AddComponent<Light>();
            light.transform.SetParent(root, false); light.transform.position = W(P(91f, 44.8f), fl + 2.2f);
            light.type = LightType.Point; light.range = 4.5f; light.intensity = 1.2f; light.color = new Color(1f, 0.74f, 0.45f);
            // terrace in the Casco square in front of the bar
            foreach (var (u, v) in new[] { (84.2f, 38.2f), (86.6f, 36.4f) })
            {
                float g = tb.Ground(P(u, v));
                Table(P(u, v), g, 0.6f, 1.1f);
                Kit.Put("Chair_1", props, W(P(u - 0.9f, v - 0.3f), tb.Ground(P(u - 0.9f, v - 0.3f))), Quaternion.Euler(0, 110, 0));
                Kit.Put("Chair_1", props, W(P(u + 0.9f, v + 0.3f), tb.Ground(P(u + 0.9f, v + 0.3f))), Quaternion.Euler(0, -70, 0));
            }
        }

        /// Simple tavern table (top + four legs) — the kit's Table_Large is a long feast table.
        void Table(Vector2 c, float g, float hw, float hl)
        {
            var wood = M("WoodDark");
            mk.Box(wood, W(c, g + 0.74f), Vector3.right * hw * 0.5f, Vector3.up * 0.03f, Vector3.forward * hl * 0.5f);
            foreach (var (x, z) in new[] { (-1, -1), (1, -1), (-1, 1), (1, 1) })
                mk.Box(wood, W(c + P(x * (hw * 0.5f - 0.06f), z * (hl * 0.5f - 0.06f)), g + 0.36f), Vector3.right * 0.035f, Vector3.up * 0.36f, Vector3.forward * 0.035f);
        }

        void BarPatio()
        {
            var h = H("F01");
            float g = 8.5f, top = g + 2.3f;
            var stone = M("Stone");
            // tapias: west (with the iron-barred rendija at eye level from the W13 alley, which runs 1.7 m lower)
            float alley = tb.Ground(P(86.2f, 57.6f));
            float r0 = alley + 1.05f, r1 = Mathf.Min(top - 0.3f, alley + 2.35f);
            mk.Slab(stone, P(88.0f, 55.7f), P(88.0f, 57.1f), 0.4f, alley - 0.6f, top);
            mk.Slab(stone, P(88.0f, 58.2f), P(88.0f, 59.8f), 0.4f, alley - 0.6f, top);
            mk.Slab(stone, P(88.0f, 57.1f), P(88.0f, 58.2f), 0.4f, r1, top);
            mk.Slab(stone, P(88.0f, 57.1f), P(88.0f, 58.2f), 0.4f, alley - 0.6f, r0);
            for (float v = 57.2f; v < 58.15f; v += 0.13f)
                mk.Box(M("Iron"), W(P(88.2f, v), (r0 + r1) / 2), Vector3.right * 0.02f, Vector3.up * (r1 - r0) / 2, Vector3.forward * 0.02f);
            mk.Slab(stone, P(96.0f, 59.8f), P(88.0f, 59.8f), 0.4f, g - 1.5f, top);
            mk.Slab(stone, P(96.0f, 55.7f), P(96.0f, 59.8f), -0.4f, g - 1.5f, top);
            // exterior stair to the first floor along the bar's back wall
            float y0 = g, y1 = h.FL + 3f, v0 = 55.75f, v1 = 56.85f;
            int n = Mathf.CeilToInt((y1 - y0) / 0.19f); float rise = (y1 - y0) / n, run = 0.27f;
            float uStart = 95.6f; run = 0.25f;
            for (int i = 0; i < n; i++)
            {
                float u = uStart - i * run;
                mk.Box(stone, new Vector3(u - run / 2, (y0 + rise * (i + 1) + y0 - 0.2f) / 2, (v0 + v1) / 2), Vector3.right * run / 2, Vector3.up * (rise * (i + 1) + 0.2f) / 2, Vector3.forward * (v1 - v0) / 2);
            }
            float uLand = uStart - n * run;
            mk.Box(stone, new Vector3(uLand - 0.8f, (y1 + y0 - 0.2f) / 2, (v0 + v1) / 2), Vector3.right * 0.8f, Vector3.up * (y1 - y0 + 0.2f) / 2, Vector3.forward * (v1 - v0) / 2);
            for (float u = uLand - 1.6f; u < uStart; u += 0.5f)
            {
                float yy = u < uLand ? y1 : y0 + rise * Mathf.Clamp((uStart - u) / run, 0, n);
                mk.Box(M("Iron"), new Vector3(u, yy + 0.5f, v1 - 0.03f), Vector3.right * 0.02f, Vector3.up * 0.5f, Vector3.forward * 0.02f);
            }
            // patio life: plants, barrels, a washtub
            var props = new GameObject("Patio_Props").transform; props.SetParent(root, false);
            Kit.Put("Barrel", props, W(P(94.8f, 59.0f), g), Quaternion.identity);
            Kit.Put("Bucket_Wooden_1", props, W(P(93.9f, 59.2f), g), Quaternion.identity, null, true, false);
            Kit.Put("Bush_Common_Flowers", props, W(P(89.0f, 59.0f), g), Quaternion.identity, Vector3.one * 0.8f, true, false);
            Kit.Put("Crate_Wooden", props, W(P(90.2f, 59.1f), g), Quaternion.Euler(0, 15, 0));
        }

        // ------------------------------------------------------------------ Ayuntamiento thresholds
        void Civic()
        {
            var h = H("F02");
            float fl = h.FL;
            // tablón: notice board between the first two modules of the front
            var face = h.A + h.Dir * 3.0f - h.In * 0.12f;
            var wood = M("WoodDark");
            mk.Box(wood, W(face, fl + 1.55f), Vector3.right * 0.75f, Vector3.up * 0.55f, Vector3.forward * 0.04f);
            var rng = new Rng(4);
            for (int i = 0; i < 6; i++)
            {
                var pp = face + h.Dir * rng.Range(-0.55f, 0.55f) - h.In * 0.05f;
                mk.Box(M("Paper"), W(pp, fl + 1.55f + rng.Range(-0.32f, 0.32f)), Vector3.right * rng.Range(0.1f, 0.18f), Vector3.up * rng.Range(0.13f, 0.2f), Vector3.forward * 0.006f);
            }
            sk.FacadeBoard("board_bandos", W(face - h.In * 0.04f, fl + 2.24f), W(-h.In, 0), 1.2f, 0.24f);
            // records door with an iron grille (module 4)
            var gp = h.A + h.Dir * 9f - h.In * 0.28f;
            for (float x = -0.55f; x <= 0.56f; x += 0.14f)
                mk.Box(M("Iron"), W(gp + h.Dir * x, fl + 1.15f), Vector3.right * 0.018f, Vector3.up * 1.12f, Vector3.forward * 0.018f);
            foreach (var y in new[] { 0.3f, 1.2f, 2.1f })
                mk.Box(M("Iron"), W(gp, fl + y), Vector3.right * 0.6f, Vector3.up * 0.02f, Vector3.forward * 0.02f);
            sk.Face("plaque_archivo", W(h.A + h.Dir * 9f - h.In * 0.12f, fl + 2.62f), W(-h.In, 0), 1.0f, 0.25f);
            sk.FacadeBoard("plaque_ayuntamiento", W(h.A + h.Dir * 5f - h.In * 0.12f, fl + 2.74f), W(-h.In, 0), 2.2f, 0.5f);
        }

        // ------------------------------------------------------------------ signs, street plaques, posters
        void Signs()
        {
            foreach (var h in lay.Houses.Where(x => x.Sign != null))
            {
                var e = HouseBuilder.Edges(h)[0];
                var d = (e.b - e.a).normalized;
                int k = h.DoorModule >= 0 ? h.DoorModule : 0;
                var c = e.a + d * (1 + 2 * k);
                var outward = W(e.outward, 0);
                // hanging sign beside the door, readable walking either way along the street
                sk.Hanging("hang_" + h.Sign, W(c + d * 1.15f + e.outward * 0.1f, h.FL + 2.55f), outward);
                if (h.Facade != null)
                {
                    float w = h.Facade == "facade_bar" ? 2.2f : 2.0f;
                    sk.FacadeBoard(h.Facade, W(c + e.outward * 0.1f, h.FL + 2.72f), outward, w, w * (h.Facade == "facade_bar" ? 300f / 1400f : 280f / 1300f));
                }
            }
            StreetPlaques();
            // posters on a few street walls near the plaza and the Casco
            int n = 0;
            string[] posters = { "poster_fiestas", "poster_feria", "poster_bolos" };
            foreach (var h in lay.Houses.Where(x => x.Kind == HouseKind.Filler).OrderBy(x => Mathf.Min((x.Center - P(150, 58)).sqrMagnitude, (x.Center - P(80, 35)).sqrMagnitude)))
            {
                if (n >= 6) break;
                var e = HouseBuilder.Edges(h)[1];
                var at = (e.a + e.b) * 0.5f + e.outward * 0.1f;
                if (lay.RouteClear(at + e.outward * 0.8f) > 1.2f) continue;     // only walls you actually walk past
                sk.Poster(posters[n % posters.Length], W(at, h.FL + 1.55f), W(e.outward, 0));
                n++;
            }
            sk.Easel("pizarra_bar", P(83.0f, 40.6f), tb.Ground(P(83.0f, 40.6f)), P(-0.6f, -1f));
        }

        void StreetPlaques()
        {
            var names = new Dictionary<string, string> { { "W04", "calle_W04" }, { "W05", "calle_W05" }, { "W06", "calle_W06" }, { "W12", "calle_W12" },
                { "W13", "calle_W13" }, { "P8a", "calle_P8a" }, { "P8b", "calle_P8b" }, { "P8c", "calle_P8c" }, { "P8d", "calle_P8d" }, { "P8e", "calle_P8e" } };
            foreach (var r in Seed.Routes)
            {
                if (!names.TryGetValue(r.Id, out var id)) continue;
                foreach (var end in new[] { 0, 1 })
                {
                    float s0 = end == 0 ? 6f : r.Length - 6f;
                    var p = At(r.Pts, r.Cum, s0, out _);
                    PlaqueNear(id, p, 7f);
                }
            }
            PlaqueNear("calle_plaza", P(150, 62), 9f);
            PlaqueNear("calle_casco", P(80, 34), 9f);
            sk.Plaque("calle_puente", W(Bridge.At(-0.05f, Bridge.HalfW - 0.22f), Seed.HX1 + 0.62f), W(-Bridge.Dir, 0), 0.54f, 0.18f);
        }

        void PlaqueNear(string id, Vector2 p, float maxDist)
        {
            HousePlan best = null; float bd = maxDist; Vector2 at = p; Vector2 outward = Vector2.zero;
            foreach (var h in lay.Houses)
                foreach (var e in HouseBuilder.Edges(h))
                {
                    float d = DistSeg(p, e.a, e.b, out float t);
                    var q = Vector2.Lerp(e.a, e.b, Mathf.Clamp(t, 0.08f, 0.92f));
                    if (d < bd && Vector2.Dot(p - q, e.outward) > 0.3f) { bd = d; best = h; at = q; outward = e.outward; }
                }
            if (best == null) return;
            sk.Plaque(id, W(at + outward * 0.1f, best.FL + 2.75f), W(outward, 0));
        }

        static Font font;
        static Material textMat;
        public void Label(Vector3 pos, Quaternion rot, string text, float size, Color col)
        {
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var go = new GameObject("Rotulo_" + text);
            go.transform.SetParent(root, false);
            go.transform.SetPositionAndRotation(pos, rot);
            var tm = go.AddComponent<TextMesh>();
            tm.text = text; tm.font = font; tm.fontSize = 64; tm.characterSize = size * 0.25f;
            tm.anchor = TextAnchor.MiddleCenter; tm.alignment = TextAlignment.Center; tm.color = col;
            if (textMat == null)
            {
                textMat = new Material(Shader.Find("Proto/Text3D")) { name = "SignText" };
                textMat.SetTexture("_MainTex", font.material.mainTexture);
                UnityEditor.AssetDatabase.CreateAsset(textMat, MatLib.GenDir + "/SignText.mat");
            }
            go.GetComponent<MeshRenderer>().sharedMaterial = textMat;
            // rot = the direction the viewer looks into the sign; TextMesh is readable looking along its +Z
        }

        // ------------------------------------------------------------------ small secrets
        void Hornacina()
        {
            var p = P(86.4f, 30.4f); var face = (P(80, 34) - p).normalized; var side = Perp(face);
            float g = tb.Ground(p);
            var stone = M("Stone");
            // machón (stone pier) with a recessed niche
            var back = p - face * 0.5f;
            mk.Slab(stone, back - side * 0.9f, back + side * 0.9f, 1.0f, g - 0.5f, g + 1.35f);
            mk.Slab(stone, back - side * 0.9f, back + side * 0.9f, 1.0f, g + 2.2f, g + 3.4f);
            mk.Slab(stone, back - side * 0.9f, back - side * 0.3f, 1.0f, g + 1.35f, g + 2.2f);
            mk.Slab(stone, back + side * 0.3f, back + side * 0.9f, 1.0f, g + 1.35f, g + 2.2f);
            mk.Slab(M("StoneDark"), back - side * 0.3f, back + side * 0.3f, 0.35f, g + 1.35f, g + 2.2f);
            mk.Slab(M("Coping"), back - side * 1.0f + face * 1.05f, back + side * 1.0f + face * 1.05f, -1.15f, g + 3.4f, g + 3.52f);
            var c = back + face * 0.62f;
            Kit.Put("Candle_1", root, W(c, g + 1.36f), Quaternion.identity, null, false, false);
            mk.Box(M("Candle"), W(c, g + 1.6f), Vector3.right * 0.015f, Vector3.up * 0.03f, Vector3.forward * 0.015f);
            var l = new GameObject("Hornacina_Vela").AddComponent<Light>();
            l.transform.SetParent(root, false); l.transform.position = W(c + face * 0.1f, g + 1.7f);
            l.type = LightType.Point; l.range = 2.2f; l.intensity = 1.4f; l.color = new Color(1f, 0.7f, 0.4f);
        }

        void PuertaCegada()
        {
            // blind arch on the façade that lines W12 near sec.puerta_cegada
            var target = P(71.5f, 12.2f);
            HousePlan best = null; float bd = 4f; Vector2 at = target;
            foreach (var h in lay.Houses)
            {
                var e = HouseBuilder.Edges(h)[0];
                float d = DistSeg(target, e.a, e.b, out float t);
                if (d < bd) { bd = d; best = h; at = Vector2.Lerp(e.a, e.b, t); }
            }
            Vector2 face, dir;
            if (best != null) { face = -best.In; dir = best.Dir; at += face * 0.12f; }
            else { dir = (P(74, 22) - P(68, 8)).normalized; face = -Perp(dir); at = target + face * 0.2f; }
            float g = tb.Ground(at + face * 0.5f);
            if (best != null) g = best.FL;
            var infill = M("StoneDark"); var arch = M("Coping");
            mk.Box(infill, W(at, g + 1.1f), W(dir, 0) * 0.62f, Vector3.up * 1.1f, W(face, 0) * 0.05f);
            // voussoirs of the old arch + jambs
            for (int i = 0; i <= 8; i++)
            {
                float a = Mathf.PI * i / 8f;
                var p = at + dir * (Mathf.Cos(a) * 0.72f);
                mk.Box(arch, W(p, g + 2.2f + Mathf.Sin(a) * 0.72f), W(dir, 0) * 0.12f, Vector3.up * 0.12f, W(face, 0) * 0.07f);
            }
            foreach (int s in new[] { -1, 1 })
                mk.Box(arch, W(at + dir * s * 0.74f, g + 1.1f), W(dir, 0) * 0.1f, Vector3.up * 1.1f, W(face, 0) * 0.07f);
        }

        void RendijaRail()
        {
            // at the river end of the reserved view gap: low wall already exists (bank wall); add a timber rail + stone step
            var a = P(52.5f, -16.3f); var b = P(51.4f, -15.0f);
            float g = tb.Ground((a + b) * 0.5f);
            mk.Slab(M("WoodDark"), a, b, 0.08f, g + 0.95f, g + 1.05f);
        }

        void CourtStair()
        {
            // S03 shared court: exterior stair to a gallery door (semi-private), seen from W13
            var stone = M("Stone");
            float g = 6.8f;
            // back wall of the court (a house side) with a real door opening at the top of the stair
            float y1 = g + 3f; int n = 16; float rise = (y1 - g) / n, run = 0.28f;
            mk.Slab(stone, P(71.0f, 67.6f), P(71.55f, 67.6f), -0.5f, g - 1f, g + 6.2f);
            mk.Slab(stone, P(72.75f, 67.6f), P(78.0f, 67.6f), -0.5f, g - 1f, g + 6.2f);
            mk.Slab(stone, P(71.55f, 67.6f), P(72.75f, 67.6f), -0.5f, g - 1f, y1);
            mk.Slab(stone, P(71.55f, 67.6f), P(72.75f, 67.6f), -0.5f, y1 + 2.2f, g + 6.2f);
            mk.Slab(M("StoneDark"), P(71.55f, 68.4f), P(72.75f, 68.4f), -0.2f, y1, y1 + 2.2f);   // dark vestibule behind the door
            for (int i = 0; i < n; i++)
            {
                float u = 77.4f - i * run;
                mk.Box(stone, new Vector3(u - run / 2, (g + rise * (i + 1) + g - 0.2f) / 2, 66.6f), Vector3.right * run / 2, Vector3.up * (rise * (i + 1) + 0.2f) / 2, Vector3.forward * 0.5f);
            }
            mk.Box(stone, new Vector3(72.2f, (y1 + g - 0.2f) / 2, 66.6f), Vector3.right * 0.7f, Vector3.up * (y1 - g + 0.2f) / 2, Vector3.forward * 0.5f);
            Kit.Put("Door_5_Flat", root, W(P(71.57f, 67.35f), y1), Quaternion.LookRotation(Vector3.back));
            mk.Slab(M("WoodDark"), P(71.2f, 66.05f), P(73.0f, 66.05f), 0.06f, y1 + 0.9f, y1 + 1.0f);
            // low court wall toward W13 with an open gateway
            mk.Slab(stone, P(83.0f, 55.4f), P(83.0f, 60.0f), -0.4f, g - 0.8f, g + 1.3f);
            mk.Slab(stone, P(83.0f, 62.2f), P(83.0f, 67.6f), -0.4f, g - 0.8f, g + 1.3f);
            var props = new GameObject("Court_Props").transform; props.SetParent(root, false);
            Kit.Put("Bush_Common_Flowers", props, W(P(74.5f, 64.5f), g), Quaternion.identity, Vector3.one * 0.7f, true, false);
            Kit.Put("Bucket_Wooden_1", props, W(P(78.8f, 66.2f), g), Quaternion.identity, null, true, false);
        }
    }
}
