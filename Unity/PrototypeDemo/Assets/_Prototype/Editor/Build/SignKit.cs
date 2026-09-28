using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Signs with painted textures (Tools/make_signs.py): facade boards, double-faced hanging signs,
    /// enamel street plaques, posters and a chalk-board easel. Every visible face reads correctly (no mirror).
    public class SignKit
    {
        const string Dir = "Assets/_Prototype/Signs/";
        readonly MeshKit mk;
        static readonly Dictionary<string, Material> mats = new Dictionary<string, Material>();
        public SignKit(MeshKit m) { mk = m; }

        public static Material Mat(string id)
        {
            if (mats.TryGetValue(id, out var m) && m != null) return m;
            string path = Dir + id + ".png";
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp != null && (imp.maxTextureSize != 1024 || imp.alphaIsTransparency != true))
            {
                imp.maxTextureSize = 1024; imp.alphaIsTransparency = true; imp.mipmapEnabled = true; imp.anisoLevel = 8;
                imp.wrapMode = TextureWrapMode.Clamp;
                imp.SaveAndReimport();
            }
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null) Debug.LogWarning("[Proto] missing sign texture " + id);
            m = new Material(MatLib.Lit) { name = "Sign_" + id };
            m.SetTexture("_BaseMap", tex);
            m.SetColor("_BaseColor", Color.white);
            bool enamel = id.StartsWith("calle_");
            m.SetFloat("_Smoothness", enamel ? 0.62f : id.StartsWith("plaque") ? 0.1f : 0.28f);
            if (tex != null && imp != null && imp.DoesSourceTextureHaveAlpha())
            {
                m.SetFloat("_AlphaClip", 1); m.SetFloat("_Cutoff", 0.5f); m.EnableKeyword("_ALPHATEST_ON");
            }
            Directory.CreateDirectory(MatLib.GenDir + "/Signs");
            AssetDatabase.CreateAsset(m, $"{MatLib.GenDir}/Signs/{id}.mat");
            mats[id] = m;
            return m;
        }

        public static void Reset() => mats.Clear();

        /// A textured rectangle centred at c, facing n (the viewer looks along -n). Reads left→right for that viewer.
        public void Face(string id, Vector3 c, Vector3 n, float w, float h)
        {
            n.y = 0; n.Normalize();
            var right = Vector3.Cross(Vector3.up, -n).normalized;
            Vector3 bl = c - right * (w * 0.5f) - Vector3.up * (h * 0.5f), tl = bl + Vector3.up * h, tr = tl + right * w, br = bl + right * w;
            mk.QuadUV(Mat(id), bl, tl, tr, br, new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0));
        }

        /// Name board flat on a façade (only its front can ever be seen).
        public void FacadeBoard(string id, Vector3 c, Vector3 outward, float w, float h)
        {
            outward.y = 0; outward.Normalize();
            var right = Vector3.Cross(Vector3.up, -outward).normalized;
            mk.Box(MatLib.Get("WoodDark"), c + outward * 0.02f, right * (w * 0.5f + 0.04f), Vector3.up * (h * 0.5f + 0.04f), outward * 0.025f);
            Face(id, c + outward * 0.048f, outward, w, h);
        }

        /// Hanging sign perpendicular to the wall at anchor (on the façade face), sticking out along 'outward'.
        public void Hanging(string id, Vector3 anchor, Vector3 outward, float w = 0.95f, float h = 0.6f)
        {
            outward.y = 0; outward.Normalize();
            var along = Vector3.Cross(Vector3.up, outward).normalized;   // board thickness axis
            var iron = MatLib.Get("Iron"); var wood = MatLib.Get("WoodDark");
            float reach = w + 0.35f;
            // wall plate, arm with a scroll brace, two short chains
            mk.Box(iron, anchor + Vector3.up * 0.1f, along * 0.06f, Vector3.up * 0.2f, outward * 0.02f);
            mk.Box(iron, anchor + outward * (reach * 0.5f) + Vector3.up * 0.28f, outward * (reach * 0.5f), Vector3.up * 0.022f, along * 0.022f);
            for (int k = 1; k <= 5; k++)
            {
                float t = k / 6f;
                mk.Box(iron, anchor + outward * (0.05f + t * 0.45f) + Vector3.up * (-0.05f + t * 0.33f), outward * 0.02f, Vector3.up * 0.02f, along * 0.018f);
            }
            var c = anchor + outward * (0.3f + w * 0.5f) + Vector3.up * (0.2f - h * 0.5f);
            foreach (float x in new[] { -w * 0.38f, w * 0.38f })
                mk.Box(iron, c + outward * x + Vector3.up * (h * 0.5f + 0.04f), outward * 0.008f, Vector3.up * 0.045f, along * 0.008f);
            mk.Box(wood, c, outward * (w * 0.5f), Vector3.up * (h * 0.5f), along * 0.022f);
            Face(id, c + along * 0.024f, along, w * 0.96f, h * 0.96f);
            Face(id, c - along * 0.024f, -along, w * 0.96f, h * 0.96f);
        }

        /// Enamel street-name plaque.
        public void Plaque(string id, Vector3 c, Vector3 outward, float w = 0.6f, float h = 0.2f)
        {
            outward.y = 0; outward.Normalize();
            var right = Vector3.Cross(Vector3.up, -outward).normalized;
            mk.Box(MatLib.Get("Iron"), c + outward * 0.008f, right * (w * 0.5f), Vector3.up * (h * 0.5f), outward * 0.008f);
            Face(id, c + outward * 0.018f, outward, w, h);
        }

        /// Poster glued on a wall.
        public void Poster(string id, Vector3 c, Vector3 outward, float h = 0.62f, float aspect = 600f / 840f)
        {
            Face(id, c + outward.normalized * 0.012f, outward, h * aspect, h);
        }

        /// A-frame chalk board (both sides readable).
        public void Easel(string id, Vector2 at, float g, Vector2 facing)
        {
            var f = W(facing.normalized, 0); var right = Vector3.Cross(Vector3.up, f);
            var wood = MatLib.Get("WoodDark");
            float w = 0.56f, h = 0.8f;
            foreach (int s in new[] { 1, -1 })
            {
                var q = Quaternion.AngleAxis(s * 12f, right);
                var c = W(at, g + 0.52f) + f * s * 0.12f;
                var up = q * Vector3.up; var nn = q * (f * s);
                mk.Box(wood, c, right * (w * 0.5f + 0.03f), up * (h * 0.5f + 0.03f), nn * 0.015f);
                var r2 = Vector3.Cross(up, -nn).normalized;
                Vector3 cc = c + nn * 0.018f;
                Vector3 bl = cc - r2 * (w * 0.5f) - up * (h * 0.5f), tl = bl + up * h, tr = tl + r2 * w, br = bl + r2 * w;
                mk.QuadUV(Mat(id), bl, tl, tr, br, new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0));
            }
        }
    }
}
