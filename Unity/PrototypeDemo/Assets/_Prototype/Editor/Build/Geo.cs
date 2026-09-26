using System;
using System.Collections.Generic;
using UnityEngine;

namespace Proto.Build
{
    /// 2D planning-frame helpers. Seed frame (U,V) maps to Unity as X=U, Z=V, Y=cota
    /// (same mapping as City04GreyboxBuilder.At in Unity/ArkusUnity).
    public static class Geo
    {
        public static Vector3 W(Vector2 uv, float h) => new Vector3(uv.x, h, uv.y);
        public static Vector3 W(float u, float v, float h) => new Vector3(u, h, v);
        public static Vector2 UV(Vector3 w) => new Vector2(w.x, w.z);
        public static Vector2 P(float u, float v) => new Vector2(u, v);

        public static Vector2[] Pts(params float[] xy)
        {
            var r = new Vector2[xy.Length / 2];
            for (int i = 0; i < r.Length; i++) r[i] = new Vector2(xy[2 * i], xy[2 * i + 1]);
            return r;
        }

        public static Vector2[] Rect(float u0, float u1, float v0, float v1) => Pts(u0, v0, u1, v0, u1, v1, u0, v1);

        public static Vector2[] Circle(Vector2 c, float r, int n = 16)
        {
            var pts = new Vector2[n];
            for (int i = 0; i < n; i++)
            {
                float a = i * Mathf.PI * 2 / n;
                pts[i] = c + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
            }
            return pts;
        }

        public static bool InPoly(Vector2 p, Vector2[] poly)
        {
            bool c = false;
            for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
            {
                Vector2 a = poly[i], b = poly[j];
                if ((a.y > p.y) != (b.y > p.y) && p.x < (b.x - a.x) * (p.y - a.y) / (b.y - a.y) + a.x) c = !c;
            }
            return c;
        }

        public static float DistSeg(Vector2 p, Vector2 a, Vector2 b, out float t)
        {
            Vector2 ab = b - a;
            float l = ab.sqrMagnitude;
            t = l > 1e-9f ? Mathf.Clamp01(Vector2.Dot(p - a, ab) / l) : 0f;
            return (a + ab * t - p).magnitude;
        }

        public static float DistPolyline(Vector2 p, Vector2[] pts) => DistPolyline(p, pts, out _, out _);

        public static float DistPolyline(Vector2 p, Vector2[] pts, out int seg, out float t)
        {
            float best = float.MaxValue; seg = 0; t = 0;
            for (int i = 1; i < pts.Length; i++)
            {
                float d = DistSeg(p, pts[i - 1], pts[i], out float tt);
                if (d < best) { best = d; seg = i - 1; t = tt; }
            }
            return best;
        }

        public static float DistPolyEdge(Vector2 p, Vector2[] poly)
        {
            float best = float.MaxValue;
            for (int i = 0; i < poly.Length; i++)
                best = Mathf.Min(best, DistSeg(p, poly[i], poly[(i + 1) % poly.Length], out _));
            return best;
        }

        public static float[] Cum(Vector2[] pts)
        {
            var c = new float[pts.Length];
            for (int i = 1; i < pts.Length; i++) c[i] = c[i - 1] + (pts[i] - pts[i - 1]).magnitude;
            return c;
        }

        public static Vector2 At(Vector2[] pts, float[] cum, float s, out Vector2 dir)
        {
            s = Mathf.Clamp(s, 0, cum[cum.Length - 1]);
            int i = 1;
            while (i < pts.Length - 1 && cum[i] < s) i++;
            float segLen = cum[i] - cum[i - 1];
            float t = segLen > 1e-6f ? (s - cum[i - 1]) / segLen : 0;
            dir = (pts[i] - pts[i - 1]).normalized;
            return Vector2.Lerp(pts[i - 1], pts[i], t);
        }

        public static Vector2 Centroid(Vector2[] poly)
        {
            Vector2 c = Vector2.zero;
            foreach (var p in poly) c += p;
            return c / poly.Length;
        }

        public static Rect Bounds(Vector2[] pts, float pad = 0)
        {
            float x0 = float.MaxValue, y0 = float.MaxValue, x1 = float.MinValue, y1 = float.MinValue;
            foreach (var p in pts) { x0 = Mathf.Min(x0, p.x); y0 = Mathf.Min(y0, p.y); x1 = Mathf.Max(x1, p.x); y1 = Mathf.Max(y1, p.y); }
            return new Rect(x0 - pad, y0 - pad, x1 - x0 + 2 * pad, y1 - y0 + 2 * pad);
        }

        public static float SignedArea(Vector2[] poly)
        {
            float a = 0;
            for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++) a += (poly[j].x * poly[i].y - poly[i].x * poly[j].y);
            return a * 0.5f;
        }

        /// Ear clipping for simple polygons. Returns triangle indices (CCW in UV = upward faces in Unity X/Z).
        public static List<int> Triangulate(Vector2[] poly)
        {
            var idx = new List<int>();
            int n = poly.Length;
            var v = new List<int>();
            bool ccw = SignedArea(poly) > 0;
            for (int i = 0; i < n; i++) v.Add(ccw ? i : n - 1 - i);
            int guard = 0;
            while (v.Count > 3 && guard++ < 10000)
            {
                bool clipped = false;
                for (int i = 0; i < v.Count; i++)
                {
                    int a = v[(i + v.Count - 1) % v.Count], b = v[i], c = v[(i + 1) % v.Count];
                    Vector2 A = poly[a], B = poly[b], C = poly[c];
                    if (Cross(B - A, C - B) <= 1e-9f) continue;
                    bool inside = false;
                    foreach (int k in v)
                    {
                        if (k == a || k == b || k == c) continue;
                        if (InTri(poly[k], A, B, C)) { inside = true; break; }
                    }
                    if (inside) continue;
                    idx.Add(a); idx.Add(b); idx.Add(c);
                    v.RemoveAt(i);
                    clipped = true;
                    break;
                }
                if (!clipped) break;
            }
            if (v.Count == 3) { idx.Add(v[0]); idx.Add(v[1]); idx.Add(v[2]); }
            return idx;
        }

        static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

        static bool InTri(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float d1 = Cross(b - a, p - a), d2 = Cross(c - b, p - b), d3 = Cross(a - c, p - c);
            return d1 >= 0 && d2 >= 0 && d3 >= 0;
        }

        /// Offset a polyline sideways (positive = left of travel direction in UV).
        public static Vector2[] Offset(Vector2[] pts, float d)
        {
            var r = new Vector2[pts.Length];
            for (int i = 0; i < pts.Length; i++)
            {
                Vector2 t0 = i > 0 ? (pts[i] - pts[i - 1]).normalized : (pts[1] - pts[0]).normalized;
                Vector2 t1 = i < pts.Length - 1 ? (pts[i + 1] - pts[i]).normalized : t0;
                Vector2 n0 = new Vector2(-t0.y, t0.x), n1 = new Vector2(-t1.y, t1.x);
                Vector2 n = (n0 + n1).normalized;
                float cos = Mathf.Max(0.35f, Vector2.Dot(n, n0));
                r[i] = pts[i] + n * (d / cos);
            }
            return r;
        }

        public static Vector2[] Resample(Vector2[] pts, float step)
        {
            var cum = Cum(pts);
            float L = cum[cum.Length - 1];
            int n = Mathf.Max(1, Mathf.CeilToInt(L / step));
            var r = new Vector2[n + 1];
            for (int i = 0; i <= n; i++) r[i] = At(pts, cum, L * i / n, out _);
            return r;
        }

        public static Vector2 Perp(Vector2 d) => new Vector2(-d.y, d.x);

        public static float Hash(float x, float y) => Hash(Mathf.FloorToInt(x), Mathf.FloorToInt(y));

        public static float Hash(int x, int y)
        {
            unchecked
            {
                uint h = (uint)x * 374761393u + (uint)y * 668265263u;
                h = (h ^ (h >> 13)) * 1274126177u;
                h ^= h >> 16;
                return (h & 0xffffff) / (float)0x1000000;
            }
        }

        public static float Noise(float x, float y) => Mathf.PerlinNoise(x + 1000.3f, y + 1000.7f);

        public static float Fbm(float x, float y, int oct = 4)
        {
            float s = 0, a = 0.5f, f = 1;
            for (int i = 0; i < oct; i++) { s += a * Noise(x * f, y * f); f *= 2.03f; a *= 0.5f; }
            return s;
        }

        public static float Smooth(float a, float b, float x)
        {
            float t = Mathf.Clamp01((x - a) / (b - a));
            return t * t * (3 - 2 * t);
        }
    }

    /// Deterministic RNG so every rebuild is identical.
    public class Rng
    {
        uint s;
        public Rng(uint seed) { s = seed == 0 ? 1u : seed; }
        public float Next() { s ^= s << 13; s ^= s >> 17; s ^= s << 5; return (s & 0xffffff) / (float)0x1000000; }
        public float Range(float a, float b) => a + (b - a) * Next();
        public int Range(int a, int bExclusive) => a + Mathf.Min(bExclusive - a - 1, (int)(Next() * (bExclusive - a)));
        public T Pick<T>(IList<T> list) => list[Range(0, list.Count)];
        public bool Chance(float p) => Next() < p;
    }
}
