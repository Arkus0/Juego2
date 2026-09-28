using System.Collections.Generic;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// X1 — Puente Viejo: one segmental stone arch over the Río, cobbled deck, parapets,
    /// gentle rise from the bridgehead (S02) and a stepped descent to Orilla sur.
    public static class Bridge
    {
        public static readonly Vector2 A = Seed.WX1, B = Seed.OX1;
        public static Vector2 Dir => (B - A).normalized;
        public static Vector2 Side => Perp(Dir);
        public const float S0 = 0f, SCrest0 = 17.5f, SCrest1 = 19.5f, SEnd = 38f;
        public const float HalfWalk = 2.0f, Parapet = 0.45f, HalfW = HalfWalk + Parapet;
        public const float HStart = Seed.HX1, HCrest = 8.3f, HEnd = Seed.HOX1;
        public const float StepRise = 0.15f;

        public static Vector2 At(float s, float t = 0) => A + Dir * s + Side * t;

        /// Walking surface height (steps on the south descent).
        public static float Deck(float s)
        {
            if (s <= S0) return HStart;
            if (s <= SCrest0) return Mathf.Lerp(HStart, HCrest, Smooth(0, SCrest0, s) * 0.5f + (s / SCrest0) * 0.5f);
            if (s <= SCrest1) return HCrest;
            if (s >= SEnd) return HEnd;
            int n = Mathf.RoundToInt((HCrest - HEnd) / StepRise);
            float tread = (SEnd - SCrest1) / n;
            int k = Mathf.Min(n - 1, Mathf.FloorToInt((s - SCrest1) / tread));
            return HCrest - (k + 1) * (HCrest - HEnd) / n;
        }

        /// Smooth line along the step nosings (parapet top, terrain stamp).
        public static float DeckSmooth(float s)
        {
            if (s <= SCrest1) return Deck(s);
            return Mathf.Lerp(HCrest, HEnd, Mathf.Clamp01((s - SCrest1) / (SEnd - SCrest1)));
        }

        public static bool InFootprint(Vector2 p, float pad, out float s)
        {
            var d = p - A;
            s = Vector2.Dot(d, Dir);
            float t = Vector2.Dot(d, Side);
            return s >= S0 - pad && s <= SEnd + pad && Mathf.Abs(t) <= HalfW + pad;
        }

        static float RiverEdge(bool north)
        {
            // walk along the axis and find where it enters / leaves mask.rio
            float prev = 0; bool inside = false;
            for (float s = 0; s < SEnd; s += 0.05f)
            {
                bool w = InPoly(At(s), Seed.Rio);
                if (w && !inside && north) return s;
                if (!w && inside && !north) return s;
                inside = w; prev = s;
            }
            return prev;
        }

        public static GameObject Build(Transform parent, System.Func<Vector2, float> ground)
        {
            var mk = new MeshKit { Tile = 2f };
            var stone = MatLib.Get("Stone"); var dark = MatLib.Get("StoneDark"); var cob = MatLib.Get("Cobble"); var ring = MatLib.Get("Coping");
            float sN = RiverEdge(true) - 0.6f, sS = RiverEdge(false) + 0.6f;
            float springY = 0.6f, crownY = HCrest - 1.25f;
            float span = sS - sN, rise = crownY - springY, mid = (sN + sS) * 0.5f;
            float R = (span * span * 0.25f + rise * rise) / (2 * rise), cy = crownY - R;

            // side profile polygon in (s, y): top (deck with steps) then bottom (foundation + arch)
            var prof = new List<Vector2>();
            prof.Add(new Vector2(S0, -2.5f));
            float sStart = S0;
            prof.Add(new Vector2(sStart, Deck(sStart)));
            for (float s = sStart + 0.5f; s < SCrest1; s += 0.5f) prof.Add(new Vector2(s, Deck(s)));
            prof.Add(new Vector2(SCrest1, HCrest));
            int n = Mathf.RoundToInt((HCrest - HEnd) / StepRise);
            float tread = (SEnd - SCrest1) / n;
            for (int k = 0; k < n; k++)
            {
                float s0 = SCrest1 + k * tread, h1 = HCrest - (k + 1) * (HCrest - HEnd) / n;
                prof.Add(new Vector2(s0, h1));
                prof.Add(new Vector2(s0 + tread, h1));
            }
            int topEnd = prof.Count - 1;
            prof.Add(new Vector2(SEnd, -2.5f));
            // bottom: foundation to south springing, arch intrados back to north springing
            prof.Add(new Vector2(sS, -2.5f));
            prof.Add(new Vector2(sS, springY));
            for (int i = 1; i < 24; i++)
            {
                float a = Mathf.Lerp(Mathf.Asin((sS - mid) / R), Mathf.Asin((sN - mid) / R), i / 24f);
                prof.Add(new Vector2(mid + R * Mathf.Sin(a), cy + R * Mathf.Cos(a)));
            }
            prof.Add(new Vector2(sN, springY));
            prof.Add(new Vector2(sN, -2.5f));

            Vector3 P(Vector2 sy, float t) => W(At(sy.x, t), sy.y);
            var side3 = new Vector3(Side.x, 0, Side.y);
            // side faces (triangulated profile)
            var tri = Triangulate(prof.ToArray());
            for (int i = 0; i < tri.Count; i += 3)
            {
                Vector2 a = prof[tri[i]], b = prof[tri[i + 1]], c = prof[tri[i + 2]];
                Tri(mk, stone, P(a, HalfW), P(b, HalfW), P(c, HalfW), side3);
                Tri(mk, stone, P(a, -HalfW), P(b, -HalfW), P(c, -HalfW), -side3);
            }
            // band faces between the sides
            for (int i = 0; i < prof.Count; i++)
            {
                Vector2 a = prof[i], b = prof[(i + 1) % prof.Count];
                var e = b - a;
                var nrm2 = new Vector2(e.y, -e.x).normalized;               // outward for a CCW profile
                if (SignedArea(prof.ToArray()) < 0) nrm2 = -nrm2;
                var want = W(Dir * nrm2.x, 0) + Vector3.up * nrm2.y;
                bool deck = i >= 1 && i < topEnd && Mathf.Abs(e.x) > 0.01f;
                bool arch = a.y > springY - 0.01f && b.y > springY - 0.01f && a.x > sN - 0.01f && a.x < sS + 0.01f && b.x > sN - 0.01f && b.x < sS + 0.01f && a.y < HCrest - 0.5f;
                var m = deck ? cob : arch ? dark : stone;
                WallBuild.O(mk, m, P(a, HalfW), P(b, HalfW), P(b, -HalfW), P(a, -HalfW), want);
            }
            // voussoir ring, proud of both faces
            for (int sgn = -1; sgn <= 1; sgn += 2)
            {
                float t = sgn * (HalfW + 0.07f);
                for (int i = 0; i < 24; i++)
                {
                    float a0 = Mathf.Lerp(Mathf.Asin((sS - mid) / R), Mathf.Asin((sN - mid) / R), i / 24f);
                    float a1 = Mathf.Lerp(Mathf.Asin((sS - mid) / R), Mathf.Asin((sN - mid) / R), (i + 1) / 24f);
                    Vector2 i0 = new Vector2(mid + R * Mathf.Sin(a0), cy + R * Mathf.Cos(a0)), i1 = new Vector2(mid + R * Mathf.Sin(a1), cy + R * Mathf.Cos(a1));
                    Vector2 o0 = new Vector2(mid + (R + 0.75f) * Mathf.Sin(a0), cy + (R + 0.75f) * Mathf.Cos(a0)), o1 = new Vector2(mid + (R + 0.75f) * Mathf.Sin(a1), cy + (R + 0.75f) * Mathf.Cos(a1));
                    var ws = side3 * sgn;
                    WallBuild.O(mk, ring, P(i0, t), P(i1, t), P(o1, t), P(o0, t), ws);
                    WallBuild.O(mk, ring, P(i0, t), P(i1, t), P(i1, sgn * HalfW), P(i0, sgn * HalfW), (Vector3.up * (cy - i0.y) + W(Dir * (mid - i0.x), 0)).normalized);
                }
            }
            // parapets following the smooth nosing line
            for (int sgn = -1; sgn <= 1; sgn += 2)
            {
                float tIn = sgn * HalfWalk, tOut = sgn * HalfW;
                for (float s = 0.3f; s < SEnd - 0.6f; s += 0.5f)
                {
                    float s1 = Mathf.Min(s + 0.5f, SEnd - 0.6f);
                    float b0 = Deck(s) - 0.05f, b1 = Deck(s1) - 0.05f;
                    float top0 = DeckSmooth(s) + 0.95f, top1 = DeckSmooth(s1) + 0.95f;
                    Vector3 a0 = W(At(s, tIn), Mathf.Min(b0, b1)), a1 = W(At(s1, tIn), Mathf.Min(b0, b1));
                    WallBuild.O(mk, stone, a0, a1, W(At(s1, tIn), top1), W(At(s, tIn), top0), -side3 * sgn);
                    float ob = Mathf.Max(Deck(s), Deck(s1));
                    WallBuild.O(mk, stone, W(At(s, tOut), top0), W(At(s1, tOut), top1), W(At(s1, tOut), ob), W(At(s, tOut), ob), side3 * sgn);
                    if (s <= 0.3f || s1 >= SEnd - 0.6f)
                    {
                        float se = s <= 0.3f ? s : s1, te = s <= 0.3f ? top0 : top1;
                        WallBuild.O(mk, stone, W(At(se, tIn), Deck(se) - 0.05f), W(At(se, tOut), Deck(se) - 0.05f), W(At(se, tOut), te + 0.1f), W(At(se, tIn), te + 0.1f), W(Dir * (s <= 0.3f ? -1 : 1), 0));
                    }
                    WallBuild.O(mk, ring, W(At(s, tIn - sgn * 0.05f), top0 + 0.1f), W(At(s1, tIn - sgn * 0.05f), top1 + 0.1f), W(At(s1, tOut + sgn * 0.05f), top1 + 0.1f), W(At(s, tOut + sgn * 0.05f), top0 + 0.1f), Vector3.up);
                    WallBuild.O(mk, ring, W(At(s, tIn - sgn * 0.05f), top0), W(At(s1, tIn - sgn * 0.05f), top1), W(At(s1, tIn - sgn * 0.05f), top1 + 0.1f), W(At(s, tIn - sgn * 0.05f), top0 + 0.1f), -side3 * sgn);
                    WallBuild.O(mk, ring, W(At(s, tOut + sgn * 0.05f), top0), W(At(s1, tOut + sgn * 0.05f), top1), W(At(s1, tOut + sgn * 0.05f), top1 + 0.1f), W(At(s, tOut + sgn * 0.05f), top0 + 0.1f), side3 * sgn);
                }
            }
            var go = mk.Build("X1_PuenteViejo", parent, true);
            return go;
        }

        static void Tri(MeshKit mk, Material m, Vector3 a, Vector3 b, Vector3 c, Vector3 want)
        {
            if (Vector3.Dot(Vector3.Cross(b - a, c - a), want) >= 0) mk.Tri(m, a, b, c); else mk.Tri(m, a, c, b);
        }
    }
}
