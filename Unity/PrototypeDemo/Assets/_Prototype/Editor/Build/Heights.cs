using System.Linq;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Layer 0/1: natural valley relief + street/platform rasantes, solved as a screened relaxation.
    public class Heights
    {
        // Terrain extents in seed metres (Unity X/Z)
        public const float U0 = -200f, V0 = -250f, Size = 640f;
        public const float Base = -6f, Range = 160f;      // world Y = Base + h01 * Range
        const int N = 641;                                // 1 m coarse grid
        public readonly float[,] H = new float[N, N];
        readonly bool[,] fixedCell = new bool[N, N];
        readonly bool[,] hole = new bool[N, N];
        readonly float[,] target = new float[N, N];
        public const float Bed = -2.2f;

        public static float Spine(float u) => Interp(u, new[] { -20f, 0, 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 175, 200, 230, 260, 320 },
            new[] { 1.0f, 1.0f, 1.8f, 3.3f, 5.2f, 7.0f, 7.9f, 8.4f, 8.6f, 9.8f, 11.2f, 12.0f, 12.3f, 12.6f, 13.2f, 14f, 16f });

        static float Interp(float x, float[] xs, float[] ys)
        {
            if (x <= xs[0]) return ys[0];
            for (int i = 1; i < xs.Length; i++)
                if (x <= xs[i]) return Mathf.Lerp(ys[i - 1], ys[i], (x - xs[i - 1]) / (xs[i] - xs[i - 1]));
            return ys[ys.Length - 1];
        }

        /// Natural relief (before streets): wedge spine between the rivers, banks, valley sides rising to the Picos.
        public static float Natural(Vector2 p)
        {
            float u = p.x, v = p.y;
            float vr = Seed.RioV(u), va = Seed.ArrV(u);
            float n = Fbm(u * 0.012f, v * 0.012f);
            float hills;
            if (u < -18f)
            {
                // below the confluence: joined river runs west; Puerto side south, Ensanche side north
                float vj = Mathf.Lerp(-10, -60, Mathf.InverseLerp(-20, -240, u));
                float d = Mathf.Abs(v - vj) - 16f;
                hills = 1.2f + 0.05f * Mathf.Max(0, d) + 70f * Smooth(50, 240, d) * (0.65f + 0.5f * n);
                return hills;
            }
            if (v < vr)
            {
                float d = vr - v - 7f;                     // south of the Río (Orilla sur)
                return 4.0f + 0.05f * Mathf.Max(0, d) + 85f * Smooth(22, 200, d) * (0.6f + 0.55f * n) + 4f * Smooth(8, 40, d) * n;
            }
            if (v > va)
            {
                float d = v - va - 5f;                     // north of the Arroyo (Ensanche / Barrio Alto)
                return 1.8f + 0.04f * Mathf.Max(0, d) + 90f * Smooth(55, 250, d) * (0.6f + 0.55f * n) + 3f * Smooth(10, 60, d) * n;
            }
            // wedge
            float spine = Spine(u);
            float k = Interp(u, new[] { 95f, 115, 140, 170, 210, 300 }, new[] { 0f, 1.5f, 4.0f, 5.0f, 5.5f, 5f });
            float south = k * Smooth(46, 2, v);
            float dA = va - v - 5f;
            float lowA = 1.5f + 0.006f * u;
            float fallA = Mathf.Max(0, spine - lowA) * (1 - Smooth(0, 26, dA));
            float dR = v - vr - 7f;
            float lowR = u < 105 ? spine : 4.2f + 0.01f * (u - 105);
            float fallR = u < 105 ? 0 : Mathf.Max(0, spine - south - lowR) * (1 - Smooth(0, 22, dR));
            float h = spine - south - fallA - fallR + (n - 0.5f) * 0.6f;
            if (u > 245) h += 20f * Smooth(245, 420, u) * (0.6f + 0.5f * n);
            return Mathf.Max(1.0f, h);
        }

        static void Cell(float u, float v, out int i, out int j)
        {
            i = Mathf.RoundToInt(u - U0); j = Mathf.RoundToInt(v - V0);
        }

        public void Solve(Layout lay, bool withGardens)
        {
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    var p = P(U0 + i, V0 + j);
                    fixedCell[i, j] = false; hole[i, j] = false;
                    target[i, j] = Natural(p);
                    if (Seed.IsWater(p))
                    {
                        if (Seed.InHard(p) && Seed.InMask(p)) hole[i, j] = true;
                        else { fixedCell[i, j] = true; target[i, j] = Bed; }
                    }
                    H[i, j] = target[i, j];
                }

            // gardens (lowest priority), houses, routes, platforms (highest)
            if (withGardens)
                foreach (var g in lay.Gardens) Stamp(g.Poly, 0, _ => g.H);
            foreach (var h in lay.Houses) if (!float.IsNaN(h.FL)) Stamp(h.Foot, 0.4f, _ => h.FL - 0.2f);
            foreach (var r in Seed.Routes)
            {
                var b = Bounds(r.Pts, r.Width * 0.5f + 1.5f);
                ForCells(b, (i, j, p) =>
                {
                    float d = r.Query(p, out float rh, out _, out _);
                    if (d <= r.Width * 0.5f + 0.9f) Fix(i, j, rh);
                });
            }
            foreach (var pl in Seed.Platforms) Stamp(pl.Poly, 0.9f, _ => pl.H);

            // screened relaxation (red-black Gauss-Seidel), Neumann at holes
            const float k = 0.22f;
            for (int it = 0; it < 220; it++)
                for (int color = 0; color < 2; color++)
                    for (int i = 1; i < N - 1; i++)
                        for (int j = 1 + ((i + color) & 1); j < N - 1; j += 2)
                        {
                            if (fixedCell[i, j] || hole[i, j]) continue;
                            float s = 0; int c = 0;
                            if (!hole[i - 1, j]) { s += H[i - 1, j]; c++; }
                            if (!hole[i + 1, j]) { s += H[i + 1, j]; c++; }
                            if (!hole[i, j - 1]) { s += H[i, j - 1]; c++; }
                            if (!hole[i, j + 1]) { s += H[i, j + 1]; c++; }
                            H[i, j] = (s + k * target[i, j]) / (c + k);
                        }
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (hole[i, j]) H[i, j] = Bed;
        }

        void Fix(int i, int j, float h)
        {
            if (i < 0 || j < 0 || i >= N || j >= N || hole[i, j]) return;
            fixedCell[i, j] = true; target[i, j] = h; H[i, j] = h;
        }

        void ForCells(Rect b, System.Action<int, int, Vector2> f)
        {
            Cell(b.xMin, b.yMin, out int i0, out int j0);
            Cell(b.xMax, b.yMax, out int i1, out int j1);
            for (int i = Mathf.Max(0, i0); i <= Mathf.Min(N - 1, i1); i++)
                for (int j = Mathf.Max(0, j0); j <= Mathf.Min(N - 1, j1); j++)
                    f(i, j, P(U0 + i, V0 + j));
        }

        void Stamp(Vector2[] poly, float pad, System.Func<Vector2, float> h)
        {
            ForCells(Bounds(poly, pad + 1), (i, j, p) =>
            {
                if (InPoly(p, poly) || (pad > 0 && DistPolyEdge(p, poly) <= pad)) Fix(i, j, h(p));
            });
        }

        /// Bilinear sample of the solved coarse field (metres).
        public float Sample(float u, float v)
        {
            float x = Mathf.Clamp(u - U0, 0, N - 1.001f), y = Mathf.Clamp(v - V0, 0, N - 1.001f);
            int i = (int)x, j = (int)y;
            float fx = x - i, fy = y - j;
            return Mathf.Lerp(Mathf.Lerp(H[i, j], H[i + 1, j], fx), Mathf.Lerp(H[i, j + 1], H[i + 1, j + 1], fx), fy);
        }

        /// Exact authored height if p is on a route band or platform, else the solved field.
        public float Exact(Vector2 p, out bool authored)
        {
            authored = true;
            foreach (var pl in Seed.Platforms)
                if (InPoly(p, pl.Poly)) return pl.H;
            float best = float.MaxValue, bh = 0;
            foreach (var r in Seed.Routes)
            {
                float d = r.Query(p, out float rh, out _, out _);
                if (d <= r.Width * 0.5f + 0.4f && d < best) { best = d; bh = rh; }
            }
            if (best < float.MaxValue) return bh;
            authored = false;
            return Sample(p.x, p.y);
        }

        public void AssignGardenHeights(Layout lay)
        {
            // back buildings sit on the relaxed ground of the first solve; too steep -> dropped
            foreach (var h in lay.Houses.Where(x => float.IsNaN(x.FL)).ToList())
            {
                var hs = h.Foot.Select(q => Sample(q.x, q.y)).ToArray();
                if (hs.Max() - hs.Min() > 2.4f) { lay.Houses.Remove(h); continue; }
                var hc = h.Center;
                float a = hs.Average() * 0.5f + Sample(hc.x, hc.y) * 0.5f;
                h.FL = Mathf.Round(a * 4f) / 4f + 0.12f;
                h.StreetH = h.FL - 0.17f;
            }
            foreach (var g in lay.Gardens)
            {
                var c = Centroid(g.Poly);
                float avg = g.Poly.Select(q => Sample(q.x, q.y)).Average() * 0.5f + Sample(c.x, c.y) * 0.5f;
                g.H = Mathf.Round(avg * 4f) / 4f;
            }
        }
    }
}
