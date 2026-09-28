using System.Linq;
using Proto.Runtime;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Lived-in interiors: extra life for the bar and four walk-in shops (tavern, bakery, cheese shop, grocer).
    /// Every shop uses the same frame: local x along the front (0..W), z inward (0..D); the counter stands across the room.
    public class Interiors
    {
        readonly TerrainBuild tb; readonly Layout lay; readonly Transform root; readonly MeshKit mk; readonly SignKit sk;
        Transform props;
        public Interiors(TerrainBuild t, Layout l, Transform r, MeshKit m, SignKit s) { tb = t; lay = l; root = r; mk = m; sk = s; }
        Material M(string n) => MatLib.Get(n);

        public static readonly (string id, string kind)[] Shops = { ("F03", "grocer"), ("F09", "tavern"), ("F12", "bakery"), ("F13", "cheese") };

        static Vector2 L(HousePlan h, float x, float z) => h.A + h.Dir * x + h.In * z;
        static float CounterZ(HousePlan h) => Mathf.Max(3.0f, h.D * 0.58f);

        /// Where the shopkeeper stands (behind the counter, looking at the door) — used by GameplayBuild.
        public static (Vector2 at, Vector2 face) Keeper(HousePlan h) => (L(h, h.W * 0.45f, CounterZ(h) + 0.75f), -h.In);

        public void BuildAll()
        {
            props = new GameObject("Interior_Props").transform; props.SetParent(root, false);
            BarLife();
            foreach (var (id, kind) in Shops)
            {
                var h = lay.Houses.FirstOrDefault(x => x.Id == id);
                if (h != null) Shop(h, kind);
            }
        }

        // ------------------------------------------------------------------ bar F01
        void BarLife()
        {
            var h = lay.Houses.First(x => x.Id == "F01");
            float fl = h.FL;
            // hearth on the partition wall (public side): jambs, mantel, hood, soot, logs and a living fire
            float fu0 = 89.25f, fu1 = 91.05f, fv = 52.8f;
            var stone = M("Stone"); var dark = M("WoodDark");
            mk.Box(stone, new Vector3(fu0 + 0.18f, fl + 0.6f, fv - 0.25f), Vector3.right * 0.18f, Vector3.up * 0.6f, Vector3.forward * 0.25f);
            mk.Box(stone, new Vector3(fu1 - 0.18f, fl + 0.6f, fv - 0.25f), Vector3.right * 0.18f, Vector3.up * 0.6f, Vector3.forward * 0.25f);
            mk.Box(stone, new Vector3((fu0 + fu1) / 2, fl + 1.32f, fv - 0.27f), Vector3.right * ((fu1 - fu0) / 2 + 0.05f), Vector3.up * 0.12f, Vector3.forward * 0.27f);
            mk.Box(dark, new Vector3((fu0 + fu1) / 2, fl + 1.49f, fv - 0.33f), Vector3.right * ((fu1 - fu0) / 2 + 0.12f), Vector3.up * 0.05f, Vector3.forward * 0.2f);
            mk.Box(stone, new Vector3((fu0 + fu1) / 2, fl + 2.24f, fv - 0.2f), Vector3.right * ((fu1 - fu0) / 2 - 0.1f), Vector3.up * 0.72f, Vector3.forward * 0.2f);
            mk.Box(M("Soot"), new Vector3((fu0 + fu1) / 2, fl + 0.62f, fv - 0.04f), Vector3.right * ((fu1 - fu0) / 2 - 0.36f), Vector3.up * 0.6f, Vector3.forward * 0.03f);
            mk.Box(stone, new Vector3((fu0 + fu1) / 2, fl + 0.05f, fv - 0.45f), Vector3.right * ((fu1 - fu0) / 2 + 0.1f), Vector3.up * 0.05f, Vector3.forward * 0.45f);
            var fc = new Vector3((fu0 + fu1) / 2, fl + 0.1f, fv - 0.3f);
            foreach (var (a, dx) in new[] { (20f, -0.1f), (-25f, 0.1f), (80f, 0f) })
            {
                var q = Quaternion.Euler(0, a, 0);
                mk.Box(M("Wood"), fc + new Vector3(dx, 0.07f, 0), q * Vector3.right * 0.32f, Vector3.up * 0.055f, q * Vector3.forward * 0.055f);
            }
            foreach (float a in new[] { 0f, 60f, 120f })
            {
                var q = Quaternion.Euler(0, a, 0);
                Vector3 r = q * Vector3.right * 0.2f;
                var fire = M("Fire");
                WallBuild.O(mk, fire, fc - r + Vector3.up * 0.1f, fc + r + Vector3.up * 0.1f, fc + r * 0.3f + Vector3.up * 0.52f, fc - r * 0.3f + Vector3.up * 0.52f, q * Vector3.forward);
                WallBuild.O(mk, fire, fc - r + Vector3.up * 0.1f, fc - r * 0.3f + Vector3.up * 0.52f, fc + r * 0.3f + Vector3.up * 0.52f, fc + r + Vector3.up * 0.1f, q * Vector3.back);
            }
            var fl0 = new GameObject("Hogar").AddComponent<Light>();
            fl0.transform.SetParent(root, false); fl0.transform.position = fc + Vector3.up * 0.5f + Vector3.back * 0.35f;
            fl0.type = LightType.Point; fl0.range = 5.5f; fl0.intensity = 2.4f; fl0.color = new Color(1f, 0.55f, 0.25f);
            fl0.gameObject.AddComponent<Flicker>();
            Kit.Put("Pot_1", props, new Vector3(fu0 + 0.4f, fl + 1.54f, fv - 0.3f), Quaternion.identity, Vector3.one * 0.8f, true, false);
            Kit.Put("CandleStick", props, new Vector3(fu1 - 0.35f, fl + 1.54f, fv - 0.3f), Quaternion.identity, null, true, false);

            // iron chandeliers instead of lamp boxes, bottles that read from the room, hams over the counter
            foreach (var (u, v) in new[] { (91.5f, 48.2f), (91.5f, 51.4f) })
                Kit.Put("Chandelier", props, W(P(u, v), fl + 2.9f), Quaternion.identity, Vector3.one * 0.5f, true, false);
            var rng = new Rng(19);
            for (int i = 0; i < 3; i++)
                for (float v = 47.3f; v < 51.0f; v += 0.21f)
                    if (rng.Chance(0.8f)) { bool big = rng.Chance(0.6f); Kit.Put(big ? "Bottle_1" : "SmallBottle", props, W(P(95.45f, v), fl + 1.33f + i * 0.45f), Quaternion.Euler(0, rng.Range(0, 360f), 0), Vector3.one * (big ? 0.95f : 1.7f), true, false); }
            for (int k = 0; k < 4; k++) Ham(new Vector3(94.55f, fl + 2.72f, 47.6f + k * 0.8f), 0.9f + 0.1f * (k % 2));
            mk.Box(dark, new Vector3(94.55f, fl + 2.78f, 49.0f), Vector3.right * 0.04f, Vector3.up * 0.04f, Vector3.forward * 1.9f);
            // posters and the chalk board
            sk.Poster("poster_fiestas", new Vector3(88.34f, fl + 1.75f, 49.6f), Vector3.right);
            sk.Poster("poster_bolos", new Vector3(88.34f, fl + 1.75f, 46.9f), Vector3.right, 0.55f);
            sk.FacadeBoard("pizarra_bar", new Vector3(91.72f, fl + 1.55f, 52.8f), Vector3.back, 0.52f, 0.74f);
            Kit.Put("Peg_Rack", props, new Vector3(88.36f, fl + 1.6f, 45.95f + 0.9f), Quaternion.LookRotation(Vector3.right), null, true, false);
            Kit.Put("Barrel_Holder", props, W(P(95.1f, 50.95f), fl), Quaternion.Euler(0, 90, 0), null, true, false);
        }

        /// Cured ham hanging by its hock: a flattened lathe.
        void Ham(Vector3 top, float s)
        {
            var ham = M("Ham");
            mk.Box(M("Iron"), top + Vector3.down * 0.05f * s, Vector3.right * 0.005f, Vector3.up * 0.05f * s, Vector3.forward * 0.005f);
            var prof = new[] { (0.018f, -0.1f), (0.03f, -0.2f), (0.055f, -0.28f), (0.085f, -0.38f), (0.1f, -0.48f), (0.092f, -0.58f), (0.06f, -0.66f), (0.0f, -0.7f) };
            const int N = 10;
            for (int k = 0; k < prof.Length - 1; k++)
                for (int i = 0; i < N; i++)
                {
                    float a0 = i * Mathf.PI * 2 / N, a1 = (i + 1) * Mathf.PI * 2 / N;
                    Vector3 R(float r, float y, float a) => top + new Vector3(Mathf.Cos(a) * r * 0.7f, y, Mathf.Sin(a) * r) * s;
                    WallBuild.O(mk, ham, R(prof[k].Item1, prof[k].Item2, a0), R(prof[k].Item1, prof[k].Item2, a1), R(prof[k + 1].Item1, prof[k + 1].Item2, a1), R(prof[k + 1].Item1, prof[k + 1].Item2, a0),
                        new Vector3(Mathf.Cos((a0 + a1) / 2), 0, Mathf.Sin((a0 + a1) / 2)));
                }
        }

        // ------------------------------------------------------------------ walk-in shops
        void Shop(HousePlan h, string kind)
        {
            float fl = h.FL;
            Vector3 Wl(float x, float z, float y) => W(L(h, x, z), fl + y);
            Vector3 X = W(h.Dir, 0), Z = W(h.In, 0);
            var floorMat = kind == "bakery" || kind == "cheese" ? M("Coping") : M("WoodDark");
            // floor and ceiling inside the 0.31 m kit wall body
            float x0 = 0.33f, x1 = h.W - 0.33f, z0 = 0.33f, z1 = h.D - 0.33f;
            mk.Box(floorMat, Wl((x0 + x1) / 2, (z0 + z1) / 2, -0.04f), X * ((x1 - x0) / 2), Vector3.up * 0.05f, Z * ((z1 - z0) / 2));
            mk.Box(M("Plaster"), Wl((x0 + x1) / 2, (z0 + z1) / 2, 3.0f), X * ((x1 - x0) / 2), Vector3.up * 0.05f, Z * ((z1 - z0) / 2));
            for (float z = z0 + 0.8f; z < z1; z += 1.4f)
                mk.Box(M("WoodDark"), Wl((x0 + x1) / 2, z, 2.86f), X * ((x1 - x0) / 2), Vector3.up * 0.1f, Z * 0.09f);
            // counter across the room with a gap at the far end for the keeper
            float cz = CounterZ(h), cx0 = 0.7f, cx1 = h.W - 1.5f;
            var cTop = kind == "tavern" ? M("WoodDark") : M("Coping");
            mk.Box(M("WoodDark"), Wl((cx0 + cx1) / 2, cz, 0.5f), X * ((cx1 - cx0) / 2), Vector3.up * 0.5f, Z * 0.3f);
            mk.Box(cTop, Wl((cx0 + cx1) / 2, cz - 0.04f, 1.03f), X * ((cx1 - cx0) / 2 + 0.05f), Vector3.up * 0.03f, Z * 0.36f);
            // back wall shelves
            float sz = z1 - 0.2f;
            for (int i = 0; i < 3; i++)
                mk.Box(M("WoodDark"), Wl((x0 + x1) / 2, sz, 1.1f + i * 0.5f), X * ((x1 - x0) / 2 - 0.2f), Vector3.up * 0.025f, Z * 0.2f);
            // warm light
            var l = new GameObject("Luz_" + h.Id).AddComponent<Light>();
            l.transform.SetParent(root, false); l.transform.position = Wl(h.W * 0.5f, h.D * 0.45f, 2.45f);
            l.type = LightType.Point; l.range = 7f; l.intensity = 2.6f; l.color = new Color(1f, 0.78f, 0.52f);
            Kit.Put("Chandelier", props, Wl(h.W * 0.5f, h.D * 0.45f, 2.93f), Quaternion.identity, Vector3.one * 0.42f, true, false);

            var rng = new Rng((uint)(h.Seed + 5));
            var faceDoor = Quaternion.LookRotation(-Z);
            switch (kind)
            {
                case "tavern":
                    for (int i = 0; i < 3; i++)
                        for (float x = x0 + 0.4f; x < x1 - 0.4f; x += 0.24f)
                            if (rng.Chance(0.8f)) { bool big = rng.Chance(0.6f); Kit.Put(big ? "Bottle_1" : "SmallBottle", props, Wl(x, sz, 1.13f + i * 0.5f), Quaternion.Euler(0, rng.Range(0, 360f), 0), Vector3.one * (big ? 0.95f : 1.7f), true, false); }
                    for (float x = cx0 + 0.3f; x < cx1 - 0.2f; x += 0.6f) Kit.Put("Mug", props, Wl(x, cz - 0.1f, 1.06f), Quaternion.Euler(0, rng.Range(0, 360f), 0), null, true, false);
                    Kit.Put("Barrel_Holder", props, Wl(x1 - 0.55f, cz + 0.9f, 0), Quaternion.LookRotation(-X), null, true, false);
                    Kit.Put("Barrel", props, Wl(x0 + 0.5f, cz + 1.0f, 0), Quaternion.identity);
                    for (int k = 0; k < 3; k++) Ham(Wl(1.2f + k * 0.7f, cz + 0.5f, 2.8f), 0.9f);
                    foreach (float x in new[] { 1.4f, h.W - 2.0f })
                    {
                        var tc = L(h, x, 1.6f);
                        TableAt(tc, fl, X, Z);
                        Kit.Put("Stool", props, W(tc - h.Dir * 0.7f, fl), Quaternion.identity);
                        Kit.Put("Stool", props, W(tc + h.Dir * 0.7f, fl), Quaternion.identity);
                        Kit.Put("Mug", props, W(tc, fl + 0.77f), Quaternion.identity, null, true, false);
                    }
                    sk.Poster("poster_feria", Wl(0.34f, 2.2f, 1.7f), X, 0.55f);
                    break;
                case "bakery":
                    // bread oven mouth in the back wall with a glow, loaves everywhere, flour sacks
                    var ov = L(h, h.W * 0.5f, z1 - 0.05f);
                    mk.Box(M("Stone"), W(ov, fl + 0.9f), X * 0.9f, Vector3.up * 0.9f, Z * 0.35f);
                    mk.Box(M("Soot"), W(ov - h.In * 0.36f, fl + 0.95f), X * 0.38f, Vector3.up * 0.26f, Z * 0.02f);
                    mk.Box(M("Fire"), W(ov - h.In * 0.34f, fl + 0.8f), X * 0.3f, Vector3.up * 0.05f, Z * 0.02f);
                    var glow = new GameObject("Horno_Fuego").AddComponent<Light>();
                    glow.transform.SetParent(root, false); glow.transform.position = W(ov - h.In * 0.8f, fl + 0.95f);
                    glow.type = LightType.Point; glow.range = 3.5f; glow.intensity = 1.8f; glow.color = new Color(1f, 0.5f, 0.2f);
                    glow.gameObject.AddComponent<Flicker>();
                    for (int i = 0; i < 3; i++)
                        for (float x = x0 + 0.3f; x < x1 - 0.3f; x += 0.3f)
                        {
                            if (Mathf.Abs(x - h.W * 0.5f) < 1.05f && i < 2) continue;
                            if (rng.Chance(0.8f)) Loaf(W(L(h, x, sz), fl + 1.17f + i * 0.5f), X, rng.Range(0.8f, 1.2f));
                        }
                    for (float x = cx0 + 0.3f; x < cx1 - 0.2f; x += 0.35f) Loaf(Wl(x, cz - 0.1f, 1.1f), rng.Chance(0.5f) ? X : Z, rng.Range(0.9f, 1.3f));
                    Kit.Put("Bag", props, Wl(x0 + 0.4f, cz + 0.9f, 0), Quaternion.identity, Vector3.one * 0.9f, true, false);
                    Kit.Put("Bag", props, Wl(x0 + 0.5f, cz + 1.5f, 0), Quaternion.Euler(0, 40, 0), Vector3.one * 0.85f, true, false);
                    Kit.Put("FarmCrate_Empty", props, Wl(x1 - 0.6f, 1.2f, 0), faceDoor, null, true, false);
                    break;
                case "cheese":
                    for (int i = 0; i < 3; i++)
                        for (float x = x0 + 0.35f; x < x1 - 0.35f; x += 0.46f)
                            if (rng.Chance(0.85f)) Cheese(W(L(h, x, sz), fl + 1.13f + i * 0.5f), rng.Range(0.14f, 0.2f), rng.Range(0.08f, 0.14f), rng.Chance(0.4f));
                    for (float x = cx0 + 0.3f; x < cx1 - 0.3f; x += 0.55f) Cheese(Wl(x, cz - 0.05f, 1.06f), rng.Range(0.12f, 0.18f), 0.1f, rng.Chance(0.5f));
                    Kit.Put("Table_Knife", props, Wl(cx0 + 0.5f, cz - 0.25f, 1.07f), Quaternion.LookRotation(X), null, true, false);
                    Kit.Put("Barrel", props, Wl(x1 - 0.55f, 1.0f, 0), Quaternion.identity);
                    TableAt(L(h, 1.5f, 1.6f), fl, X, Z);
                    for (int k = 0; k < 3; k++) Cheese(W(L(h, 1.25f + k * 0.25f, 1.6f), fl + 0.8f), 0.13f, 0.1f, k == 1);
                    break;
                default: // grocer
                    string[] goods = { "Bottle_1", "Pot_1", "SmallBottles_1", "Vase_2", "Bag" };
                    for (int i = 0; i < 3; i++)
                        for (float x = x0 + 0.4f; x < x1 - 0.4f; x += 0.42f)
                            if (rng.Chance(0.8f))
                            {
                                string n = goods[rng.Range(0, goods.Length)];
                                Kit.Put(n, props, Wl(x, sz, 1.13f + i * 0.5f), Quaternion.Euler(0, rng.Range(0, 360f), 0), Vector3.one * (n == "Bottle_1" ? 1.0f : n == "Bag" ? 0.5f : 0.75f), true, false);
                            }
                    foreach (var (x, n) in new[] { (1.0f, "FarmCrate_Apple"), (1.9f, "FarmCrate_Carrot"), (2.8f, "FarmCrate_Apple") })
                        Kit.Put(n, props, Wl(x, 1.0f, 0), faceDoor, null, true, false);
                    Kit.Put("Barrel_Apples", props, Wl(h.W - 1.0f, 1.2f, 0), Quaternion.identity, null, true, false);
                    Kit.Put("Bag", props, Wl(h.W - 1.8f, 1.0f, 0), Quaternion.Euler(0, 30, 0), Vector3.one * 0.85f, true, false);
                    Kit.Put("Coin_Pile", props, Wl(cx1 - 0.4f, cz - 0.1f, 1.06f), Quaternion.identity, null, true, false);
                    break;
            }
        }

        void TableAt(Vector2 c, float fl, Vector3 X, Vector3 Z)
        {
            var wood = M("WoodDark");
            mk.Box(wood, W(c, fl + 0.74f), X * 0.4f, Vector3.up * 0.03f, Z * 0.35f);
            foreach (var (a, b) in new[] { (-1, -1), (1, -1), (-1, 1), (1, 1) })
                mk.Box(wood, W(c, fl + 0.36f) + X * (a * 0.34f) + Z * (b * 0.29f), X * 0.035f, Vector3.up * 0.36f, Z * 0.035f);
        }

        /// Round country loaf (hogaza): a low dome.
        void Loaf(Vector3 at, Vector3 along, float s)
        {
            var prof = new[] { (0.1f, 0f), (0.112f, 0.03f), (0.1f, 0.065f), (0.065f, 0.09f), (0.0f, 0.1f) };
            const int N = 10;
            var bread = M("Bread");
            for (int k = 0; k < prof.Length - 1; k++)
                for (int i = 0; i < N; i++)
                {
                    float a0 = i * Mathf.PI * 2 / N, a1 = (i + 1) * Mathf.PI * 2 / N;
                    Vector3 R(float r, float y, float a) => at + new Vector3(Mathf.Cos(a) * r, y, Mathf.Sin(a) * r * 0.85f) * s;
                    WallBuild.O(mk, bread, R(prof[k].Item1, prof[k].Item2, a0), R(prof[k].Item1, prof[k].Item2, a1), R(prof[k + 1].Item1, prof[k + 1].Item2, a1), R(prof[k + 1].Item1, prof[k + 1].Item2, a0),
                        new Vector3(Mathf.Cos((a0 + a1) / 2), 0.6f, Mathf.Sin((a0 + a1) / 2)));
                }
        }

        void Cheese(Vector3 at, float r, float hgt, bool cut)
        {
            const int N = 12;
            var rind = M("Rind"); var paste = M("Cheese");
            float span = cut ? Mathf.PI * 1.6f : Mathf.PI * 2f;
            for (int i = 0; i < N; i++)
            {
                float a0 = span * i / N, a1 = span * (i + 1) / N;
                Vector3 p0 = at + new Vector3(Mathf.Cos(a0), 0, Mathf.Sin(a0)) * r, p1 = at + new Vector3(Mathf.Cos(a1), 0, Mathf.Sin(a1)) * r;
                var outN = new Vector3(Mathf.Cos((a0 + a1) / 2), 0, Mathf.Sin((a0 + a1) / 2));
                WallBuild.O(mk, rind, p0, p1, p1 + Vector3.up * hgt, p0 + Vector3.up * hgt, outN);
                TriUp(rind, at + Vector3.up * hgt, p0 + Vector3.up * hgt, p1 + Vector3.up * hgt);
            }
            if (cut)
            {
                foreach (float a in new[] { 0f, span })
                {
                    var p = at + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * r;
                    var n = Vector3.Cross(Vector3.up, p - at).normalized * (a == 0 ? -1 : 1);
                    WallBuild.O(mk, paste, at, p, p + Vector3.up * hgt, at + Vector3.up * hgt, n);
                }
            }
        }

        void TriUp(Material m, Vector3 a, Vector3 b, Vector3 c)
        {
            if (Vector3.Dot(Vector3.Cross(b - a, c - a), Vector3.up) >= 0) mk.Tri(m, a, b, c); else mk.Tri(m, a, c, b);
        }
    }
}
