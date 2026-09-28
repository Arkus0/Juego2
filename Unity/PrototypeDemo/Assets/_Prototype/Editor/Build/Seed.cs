using System.Collections.Generic;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    public enum Surf { Cobble, Road, Dirt, Flag }

    /// A street centreline with a height profile (hand-authored cotas per vertex; linear by arc length).
    public class Route
    {
        public string Id, Name;
        public Vector2[] Pts;
        public float[] H;
        public float Width;
        public Surf Surface;
        public bool Landings;     // E2 "cuesta con rellanos": ramps + flat landings
        public bool Soft;         // outside the hard seed (visual only)
        public float[] Cum;
        public float Length => Cum[Cum.Length - 1];

        public Route(string id, string name, float w, Surf s, float[] h, params float[] xy)
        {
            Id = id; Name = name; Width = w; Surface = s; H = h; Pts = Pts(xy);
            Cum = Geo.Cum(Pts);
            if (H.Length != Pts.Length) Debug.LogError("[Proto] route " + id + " height count mismatch");
        }

        public float HeightAt(float s)
        {
            float L = Length;
            s = Mathf.Clamp(s, 0, L);
            if (Landings)
            {
                const float period = 12f, ramp = 9f;
                float Remap(float x) => Mathf.Floor(x / period) * period + Mathf.Min(x % period, ramp) * period / ramp;
                s = Remap(s) * L / Remap(L);
                s = Mathf.Clamp(s, 0, L);
            }
            int i = 1;
            while (i < Pts.Length - 1 && Cum[i] < s) i++;
            float seg = Cum[i] - Cum[i - 1];
            float t = seg > 1e-6f ? (s - Cum[i - 1]) / seg : 0;
            return Mathf.Lerp(H[i - 1], H[i], t);
        }

        /// Distance from p to the centreline and the route height at the closest point.
        public float Query(Vector2 p, out float h, out float s, out Vector2 dir)
        {
            float d = DistPolyline(p, Pts, out int seg, out float t);
            s = Cum[seg] + t * (Cum[seg + 1] - Cum[seg]);
            h = HeightAt(s);
            dir = (Pts[seg + 1] - Pts[seg]).normalized;
            return d;
        }
    }

    public class Platform
    {
        public string Id;
        public Vector2[] Poly;
        public float H;
        public Surf Surface;
        public Platform(string id, float h, Surf s, Vector2[] poly) { Id = id; H = h; Surface = s; Poly = poly; }
    }

    /// Accepted CITY-03 seed geometry + CITY-07 concept (PR #228) routes/places, in seed U/V metres.
    public static class Seed
    {
        public static readonly Vector2[] Hard = Pts(-20, -60, 70, -80, 205, -48, 245, 22, 220, 95, 150, 145, 70, 132, -20, 55);
        public static readonly Vector2[] Rio = Pts(-20, -8, 0, -10, 20, -24, 40, -42, 60, -46, 95, -45, 140, -42, 185, -40, 205, -38,
            205, -48, 185, -52, 140, -57, 95, -61, 60, -62, 40, -58, 20, -43, 0, -30, -20, -24);
        public static readonly Vector2[] Arroyo = Pts(-20, -8, 0, 10, 20, 28, 40, 48, 60, 68, 78, 90, 88, 111, 108, 132, 120, 140.125f,
            100, 136.875f, 99, 130, 90, 118, 82, 106, 70, 86, 50, 60, 30, 40, 8, 20, -20, 4);
        public static readonly Vector2[] X1 = Pts(53, -33, 45, -65, 39, -63, 47, -31);
        public static readonly Vector2[] X5 = Pts(90.5f, 107.8f, 88.5f, 121.8f, 91.5f, 122.2f, 93.5f, 108.2f);
        public static readonly Vector2[] StubX1W = Pts(42, -40, 58, -40, 58, -22, 42, -22);
        public static readonly Vector2[] StubX1O = Pts(34, -69, 50, -72, 52, -62, 37, -60);
        public static readonly Vector2[] StubX5W = Pts(84, 99, 99, 99, 96, 108, 87, 108);
        public static readonly Vector2[] StubX5E = Pts(86, 119, 93, 124, 94, 130, 84, 128);

        public static readonly Vector2 WLanding = P(0, 0), WX1 = P(50, -32), OX1 = P(42, -64), WCasco = P(80, 35),
            WX5 = P(92, 108), EX5 = P(90, 122), WPlaza = P(150, 58), WShop = P(195, 70);

        // Cotas (hypotheses from the brief, adjusted where noted in PROTOTYPE_REPORT.md)
        public const float HWater = 0f, HLanding = 1f, HFord = 0.5f, HCasco = 8f, HX1 = 7.5f, HCrest = 8.3f, HOX1 = 4.4f,
            HPlaza = 12f, HShop = 12.5f, HX5 = 1.8f;

        public static readonly Dictionary<string, Vector2[]> Slots = new Dictionary<string, Vector2[]>
        {
            { "F01", Rect(88, 96, 45, 60) }, { "F02", Rect(148, 166, 66, 88) }, { "F03", Rect(188, 198, 76, 94) },
            { "F04", Rect(62, 69, 40, 54) }, { "F05", Rect(98, 105, 24, 36) }, { "F06", Rect(126, 138, 62, 78) },
            { "F07", Rect(170, 178, 74, 90) }, { "F08", Rect(50, 57, 3, 16) },
        };
        public static readonly Vector2[] S01 = Pts(132, 35, 150, 40, 148, 52, 130, 47);
        public static readonly Vector2[] S02 = StubX1W;
        public static readonly Vector2[] S03 = Rect(70, 83, 55, 68);

        public static readonly Vector2[] PlazaPoly = Pts(136, 50, 150, 53, 168, 57, 168, 65, 148, 65, 136, 61);
        public static readonly Vector2[] CascoSq = Pts(73, 30, 84, 28, 88, 36, 83, 41, 75, 39);
        public static readonly Vector2[] LandingSq = Pts(-8, -3, 6, -5, 8, 3, -2, 4);

        public static readonly List<Route> Routes = new List<Route>
        {
            new Route("W04", "Calle comercial", 5f, Surf.Road, new[] { 12.0f, 12.1f, 12.25f, 12.4f, 12.5f }, 150, 58, 162, 61, 175, 66, 186, 68, 195, 70),
            new Route("W05", "Del Casco a la Plaza", 3.6f, Surf.Cobble, new[] { 8.5f, 9.2f, 10.3f, 11.3f, 12.0f }, 96, 40, 104, 41, 118, 47, 132, 54, 150, 58),
            new Route("W06", "La Cuesta", 3.4f, Surf.Cobble, new[] { 8.0f, 7.3f, 6.2f, 5.0f, 3.6f, 2.2f, 1.0f }, 80, 35, 70, 30, 60, 25, 46, 22, 32, 15, 16, 7, 0, 0) { Landings = true },
            new Route("W12", "Subida del Puente", 3.8f, Surf.Cobble, new[] { 8.0f, 7.95f, 7.85f, 7.75f, 7.6f, 7.5f }, 80, 35, 74, 22, 68, 8, 62, -6, 56, -18, 50, -32),
            new Route("W13", "Callejón bajo", 3.0f, Surf.Cobble, new[] { 8.0f, 7.7f, 6.8f, 5.6f, 4.4f, 3.1f, HX5 }, 80, 35, 84, 44, 86, 58, 87, 72, 89, 86, 90, 97, 92, 108),
            new Route("microA", "casco.micro.A", 2.8f, Surf.Cobble, new[] { 8.0f, 8.05f, 8.15f, 8.3f, 8.5f }, 80, 35, 84, 28, 90, 27, 95, 31, 96, 40),
            new Route("microB", "casco.micro.B", 2.8f, Surf.Cobble, new[] { 8.0f, 8.2f, 8.4f, 8.5f }, 80, 35, 83, 41, 90, 43, 96, 40),
            new Route("P8a", "Calleja de los Tintes", 3.0f, Surf.Cobble, new[] { 9.2f, 9.0f, 7.6f, 7.7f, 8.0f, 9.4f, 10.9f, 12.0f }, 104, 41, 108, 38, 110, 22, 128, 6, 155, 8, 172, 28, 172, 45, 168, 57),
            new Route("P8b", "Travesía del Horno", 2.8f, Surf.Cobble, new[] { 7.85f, 7.7f, 7.6f }, 68, 8, 88, 12, 110, 22),
            new Route("P8c", "Calleja Alta", 2.6f, Surf.Cobble, new[] { 12.0f, 11.0f, 9.8f, 9.2f, 6.8f, 6.1f, 5.4f, 5.0f }, 168, 65, 168, 93, 160, 110, 150, 114, 128, 106, 110, 96, 96, 84, 88, 79),
            new Route("P8d", "Pasadizo del Arco", 2.6f, Surf.Cobble, new[] { 5.0f, 6.8f, 8.6f, 10.35f }, 88, 79, 104, 72, 116, 62, 121, 48.5f),
            new Route("P8f", "Senda de las Huertas", 1.8f, Surf.Dirt, new[] { 7.6f, 7.75f, 7.9f, 8.0f }, 113, 20, 125, 28, 140, 24, 155, 8),
            new Route("P8g", "Senda del Arroyo", 1.8f, Surf.Dirt, new[] { 6.8f, 4.6f, 2.4f, HX5 }, 128, 106, 114, 116, 100, 108, 92, 108),
            new Route("P8e", "Camino del Ensanche bajo", 3.4f, Surf.Road, new[] { HX5, 2.0f, 2.4f, 2.8f, 3.2f }, 90, 122, 84, 132, 60, 112, 35, 88, 12, 64),
        };

        public static readonly List<Platform> Platforms = new List<Platform>
        {
            new Platform("Plaza", HPlaza, Surf.Flag, PlazaPoly),
            new Platform("S01", 11.6f, Surf.Flag, S01),
            new Platform("CascoSq", HCasco, Surf.Flag, CascoSq),
            new Platform("Landing", HLanding, Surf.Flag, LandingSq),
            new Platform("S02", HX1, Surf.Cobble, Pts(42, -38, 58, -38, 58, -22, 42, -22)),
            new Platform("S03", 6.8f, Surf.Flag, S03),
            new Platform("StubX1O", HOX1, Surf.Cobble, Pts(35, -68.5f, 50, -71.5f, 51.5f, -63, 38, -61)),
            new Platform("StubX5W", HX5, Surf.Flag, Pts(86, 101, 98, 101, 95.5f, 106, 88, 106)),
            new Platform("StubX5E", HX5, Surf.Flag, Pts(87, 124, 93, 125, 93.5f, 129, 85, 127.5f)),
            new Platform("Horno", 7.6f, Surf.Flag, Circle(P(114, 19), 6.2f, 14)),
            new Platform("Tinte", 9.3f, Surf.Cobble, Circle(P(171, 27), 5.5f, 14)),
            new Platform("Era", 9.2f, Surf.Dirt, Circle(P(150, 113), 8.5f, 16)),
            new Platform("Mirador", 4.6f, Surf.Flag, Circle(P(115, 118), 3.8f, 12)),
            new Platform("BarPatio", 8.5f, Surf.Flag, Rect(88.3f, 95.7f, 55.95f, 59.45f)),
        };

        // P9 frontage regions (CITY_P9_PLACES_AMENDMENT.md), with the route each one faces.
        public struct P9Place { public string Id, Name, Sign, Route; public Vector2[] Poly; }
        public static readonly P9Place[] P9 =
        {
            new P9Place { Id = "F09", Name = "Taberna del Puente", Sign = "taberna", Route = "W12", Poly = Pts(68, -22.5f, 71.5f, -15.5f, 62.5f, -11, 59, -18) },
            new P9Place { Id = "F10", Name = "Barbería", Sign = "barberia", Route = "W12", Poly = Pts(67, 12, 69.5f, 17.5f, 61, 21, 58.5f, 15.5f) },
            new P9Place { Id = "F11", Name = "Orujería del alambique", Sign = "orujos", Route = "W06", Poly = Pts(49.5f, 25, 57.5f, 27, 55.5f, 36.5f, 47.5f, 35) },
            new P9Place { Id = "F12", Name = "Horno de pan", Sign = "horno", Route = "P8b", Poly = Pts(102, 16, 96, 13.5f, 99.5f, 5, 106, 8) },
            new P9Place { Id = "F13", Name = "Quesería", Sign = "quesos", Route = "W05", Poly = Pts(110.5f, 56.5f, 104.5f, 53.5f, 108, 45.5f, 114.5f, 48) },
            new P9Place { Id = "F14", Name = "Ferretería", Sign = "ferreteria", Route = "W05", Poly = Pts(125, 63.5f, 118.5f, 60, 123, 52, 129, 55.5f) },
            new P9Place { Id = "F15", Name = "Café-billar", Sign = "cafe_billar", Route = "P8d", Poly = Pts(89, 70, 88.5f, 62, 98.5f, 61, 99, 69) },
            new P9Place { Id = "F16", Name = "Fonda de los Tintes", Sign = "fonda", Route = "P8a", Poly = Pts(158, 42.5f, 158, 33.5f, 170, 33.5f, 170, 42.5f) },
            new P9Place { Id = "F17", Name = "Taller del herrero", Sign = "herreria", Route = "P8a", Poly = Pts(144.5f, 5, 135.5f, 4.5f, 136.5f, -6.5f, 145.5f, -6) },
            new P9Place { Id = "F18", Name = "Estanco-quiosco", Sign = "estanco", Route = "P8c", Poly = Pts(153.5f, 96.5f, 156, 91, 165, 95, 162, 100.5f) },
            new P9Place { Id = "F19", Name = "Ultramarinos del Ensanche", Sign = "ultramarinos", Route = "P8e", Poly = Pts(54.5f, 81.5f, 61, 87.5f, 50, 99, 43.5f, 93) },
        };

        public struct Secret { public string Id, Name, Text; public Vector2 At; }
        public static readonly Secret[] Secrets =
        {
            new Secret { Id = "sec.riada", At = P(48, -37), Name = "Marcas de riada", Text = "Muescas talladas en el estribo con las alturas de las crecidas; por eso el vado solo se pasa con aguas bajas." },
            new Secret { Id = "disc.bar.secondary_layer", At = P(86.5f, 57.5f), Name = "El patio de atrás del bar", Text = "Por la rendija del callejón se ven el patio trasero del bar y una escalera: la sala no es todo el edificio." },
            new Secret { Id = "disc.ayuntamiento.records_boundary", At = P(157, 64), Name = "El tablón del Ayuntamiento", Text = "Bandos y avisos clavados junto a una puerta con reja: detrás hay un archivo que no es público." },
            new Secret { Id = "sec.court_stair", At = P(85.5f, 64), Name = "La escalera del patio", Text = "Una escalera exterior sube por el patio compartido; se nota que es de los vecinos, no un paso." },
            new Secret { Id = "sec.puerta_cegada", At = P(70.5f, 12.5f), Name = "La puerta cegada", Text = "Un arco tapiado con sillares de otro color: aquí hubo otra puerta, otro uso." },
            new Secret { Id = "sec.hornacina", At = P(85.5f, 30), Name = "La hornacina", Text = "Un nicho con una vela encendida en la esquina de la plazuela. Buena referencia para no perderse." },
            new Secret { Id = "sec.rendija", At = P(60.5f, -10), Name = "La rendija al río", Text = "Entre dos casas, un hueco estrecho enmarca el río y el Puente Viejo." },
            new Secret { Id = "sec.barca", At = P(-4, -2), Name = "La barca varada", Text = "Una barca vieja, varada en el desembarcadero. Alguna vez cruzó el río desde aquí." },
            new Secret { Id = "sec.pozo", At = P(140, 21), Name = "El pozo de la huerta", Text = "Tras la reja de una huerta: un pozo viejo y una higuera. La huerta es privada." },
            new Secret { Id = "sec.mirador_arroyo", At = P(115, 118), Name = "Mirador del Arroyo", Text = "Un banco sobre el Arroyo: desde aquí se ven las pasaderas y el Ensanche." },
            new Secret { Id = "sec.era", At = P(150, 113), Name = "La Era alta", Text = "Explanada de tierra con un castaño y unos bolos apoyados. Aquí se juega sin prisa." },
        };

        // Soft-envelope river centrelines (concept map) — visual only.
        public static readonly Vector2[] RioC = Pts(-20, -16, 0, -20, 20, -33, 40, -50, 60, -54, 95, -53, 140, -49.5f, 185, -46, 205, -43, 250, -45, 300, -52, 360, -62, 470, -75, 600, -85);
        public static readonly Vector2[] JoinC = Pts(-20, -10, -50, -14, -90, -24, -140, -40, -240, -58, -400, -80);
        public static readonly Vector2[] ArrC = Pts(-20, -2, 4, 15, 25, 34, 45, 54, 65, 77, 80, 98, 94, 120, 110, 138.5f, 140, 152, 180, 162, 230, 170, 290, 180, 360, 190, 470, 200, 600, 210);

        public static bool InHard(Vector2 p) => InPoly(p, Hard);

        static float Interp(Vector2[] c, float u)
        {
            if (u <= c[0].x) return c[0].y + (c[1].y - c[0].y) * (u - c[0].x) / (c[1].x - c[0].x);
            for (int i = 1; i < c.Length; i++)
                if (u <= c[i].x) return c[i - 1].y + (c[i].y - c[i - 1].y) * (u - c[i - 1].x) / (c[i].x - c[i - 1].x);
            int n = c.Length;
            return c[n - 2].y + (c[n - 1].y - c[n - 2].y) * (u - c[n - 2].x) / (c[n - 1].x - c[n - 2].x);
        }

        public static float RioV(float u) => Interp(RioC, u);
        public static float ArrV(float u) => Interp(ArrC, u);

        /// Soft-envelope ribbons (only used outside the exact masks).
        static float RibbonDist(Vector2 p, out float halfW)
        {
            float best = float.MaxValue; halfW = 0;
            float d;
            d = DistPolyline(p, RioC, out int s1, out _);
            if (s1 >= 8 && d < best) { best = d; halfW = 5.5f + 0.02f * Mathf.Max(0, p.x - 205); }
            d = DistPolyline(p, JoinC);
            if (d < best) { best = d; halfW = 14f + 0.03f * Mathf.Max(0, -20 - p.x); }
            d = DistPolyline(p, ArrC, out int s3, out _);
            if (s3 >= 7 && d < best) { best = d; halfW = 4.2f; }
            return best;
        }

        public static bool InMask(Vector2 p) => InPoly(p, Rio) || InPoly(p, Arroyo);

        /// Water test for the whole playable+visual world.
        public static bool IsWater(Vector2 p)
        {
            if (InMask(p)) return true;
            if (InHard(p) && p.x < 205) return false;
            return RibbonDist(p, out float hw) < hw;
        }

        /// Signed distance-ish to the nearest water (negative inside water), coarse; used for bank shaping.
        public static float WaterDistance(Vector2 p)
        {
            float dr = RibbonDist(p, out float hw) - hw;
            float dm = InMask(p) ? -Mathf.Min(DistPolyEdge(p, Rio), DistPolyEdge(p, Arroyo)) : Mathf.Min(DistPolyEdge(p, Rio), DistPolyEdge(p, Arroyo));
            return Mathf.Min(dr, dm);
        }
    }
}
