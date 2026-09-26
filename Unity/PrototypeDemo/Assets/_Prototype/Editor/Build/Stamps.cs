using System;
using System.Collections.Generic;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Authored step/ramp geometry that the terrain must make room for (planned before the terrain is computed).
    public static class Stamps
    {
        public static readonly List<(Vector2[] poly, Func<Vector2, float> h)> All = new List<(Vector2[], Func<Vector2, float>)>();

        // ---- X5 ford: W.X5 -> E.X5
        public static readonly Vector2 FordA = Seed.WX5, FordB = Seed.EX5;
        public static Vector2 FordDir => (FordB - FordA).normalized;
        public static float FordLen => (FordB - FordA).magnitude;
        public static float FordWetIn, FordWetOut;          // s where the axis enters / leaves mask.arroyo
        public const float StoneTop = Seed.HFord, FordStepLen = 3.0f;

        // ---- landing slipway (Río side of W.LANDING)
        public static readonly Vector2 SlipTop = P(-3.5f, -5.4f);
        public static readonly Vector2 SlipDir = new Vector2(-0.12f, -1f).normalized;
        public static float SlipLen;                          // down to the water edge
        public const float SlipTopH = Seed.HLanding, SlipBottomH = 0.12f;

        public static Vector2 Ford(float s, float t = 0) => FordA + FordDir * s + Perp(FordDir) * t;

        /// Step top heights on the ford approaches (from bank level down to the stones).
        public static float FordApproach(float s, float bankH)
        {
            if (s < FordWetIn - FordStepLen) return bankH;
            if (s < FordWetIn) { float t = (s - (FordWetIn - FordStepLen)) / FordStepLen; return Quant(Mathf.Lerp(bankH, StoneTop, t), bankH); }
            if (s > FordWetOut + FordStepLen) return bankH;
            if (s > FordWetOut) { float t = (FordWetOut + FordStepLen - s) / FordStepLen; return Quant(Mathf.Lerp(bankH, StoneTop, t), bankH); }
            return StoneTop;
        }

        static float Quant(float h, float bankH) => StoneTop + Mathf.Round((h - StoneTop) / 0.18f) * 0.18f;

        static Stamps()
        {
            bool inside = false;
            for (float s = 0; s <= FordLen; s += 0.05f)
            {
                bool w = InPoly(Ford(s), Seed.Arroyo);
                if (w && !inside) FordWetIn = s;
                if (!w && inside) FordWetOut = s;
                inside = w;
            }
            var fs = Perp(FordDir);
            All.Add((new[] { Ford(FordWetIn - FordStepLen - 0.3f, -1.3f), Ford(FordWetIn - FordStepLen - 0.3f, 1.3f), Ford(FordWetIn + 0.2f, 1.3f), Ford(FordWetIn + 0.2f, -1.3f) },
                p => FordApproach(Vector2.Dot(p - FordA, FordDir), Seed.HX5) - 0.35f));
            All.Add((new[] { Ford(FordWetOut - 0.2f, -1.3f), Ford(FordWetOut - 0.2f, 1.3f), Ford(FordWetOut + FordStepLen + 0.3f, 1.3f), Ford(FordWetOut + FordStepLen + 0.3f, -1.3f) },
                p => FordApproach(Vector2.Dot(p - FordA, FordDir), Seed.HX5) - 0.35f));

            SlipLen = 0;
            for (float s = 0; s < 12; s += 0.05f) if (InPoly(SlipTop + SlipDir * s, Seed.Rio)) { SlipLen = s + 0.4f; break; }
            var ss = Perp(SlipDir);
            All.Add((new[] { SlipTop - ss * 3.0f, SlipTop + ss * 3.0f, SlipTop + SlipDir * SlipLen + ss * 3.0f, SlipTop + SlipDir * SlipLen - ss * 3.0f },
                p => Mathf.Lerp(SlipTopH, SlipBottomH, Mathf.Clamp01(Vector2.Dot(p - SlipTop, SlipDir) / SlipLen)) - 0.3f));
        }
    }
}
