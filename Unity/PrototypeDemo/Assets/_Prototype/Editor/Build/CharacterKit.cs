using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Proto.Build
{
    /// Clothed townsfolk (Tools/blender/make_townsfolk.py → Assets/_Derived/Generated/Characters/Townsfolk_*.fbx).
    /// Materials arrive as names only (Cloth_RRGGBB, Hair_RRGGBB, Skin_<base>_<tone>, MI_Eyes) and are rebuilt here as URP Lit.
    public static class CharacterKit
    {
        const string Dir = "Assets/_Derived/Generated/Characters/";
        const string Tex = "Assets/ThirdParty/Quaternius/BaseCharacters/Textures/";
        static readonly Dictionary<string, Material> mats = new Dictionary<string, Material>();

        public static void Reset() => mats.Clear();

        public static bool Available(string variant) => File.Exists(Dir + "Townsfolk_" + variant + ".fbx");

        /// tint: 0 keeps the authored colours; other values shift clothes slightly so repeated variants differ.
        public static GameObject Spawn(string variant, string name, Transform parent, AnimatorController ac, int tint)
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(Dir + "Townsfolk_" + variant + ".fbx");
            if (model == null) { Debug.LogWarning("[Proto] missing townsfolk " + variant); return null; }
            var go = (GameObject)PrefabUtility.InstantiatePrefab(model, parent);
            go.name = name;
            var anim = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
            anim.runtimeAnimatorController = ac;
            anim.applyRootMotion = false;
            anim.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            foreach (var r in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var m = r.sharedMaterials;
                for (int i = 0; i < m.Length; i++) m[i] = For(m[i] != null ? m[i].name : "", tint);
                r.sharedMaterials = m;
                r.updateWhenOffscreen = false;
                r.skinnedMotionVectors = false;
            }
            return go;
        }

        static Material For(string src, int tint)
        {
            string n = src.Split('.')[0].Trim();
            if (n.StartsWith("Cloth_") && tint != 0) n += "_t" + tint;
            if (mats.TryGetValue(n, out var cached) && cached != null) return cached;
            var m = new Material(MatLib.Lit) { name = "TF_" + n, enableInstancing = true };
            if (n.StartsWith("Cloth_") || n.StartsWith("Hair_"))
            {
                ColorUtility.TryParseHtmlString("#" + n.Substring(n.IndexOf('_') + 1, 6), out var c);
                if (n.StartsWith("Cloth_") && tint != 0)
                {
                    Color.RGBToHSV(c, out float h, out float s, out float v);
                    int hsh = 17; foreach (char ch in n) hsh = hsh * 31 + ch;
                    var rng = new System.Random(tint * 7919 + hsh);
                    h = Mathf.Repeat(h + ((float)rng.NextDouble() - 0.5f) * 0.08f, 1f);
                    s = Mathf.Clamp01(s * (0.8f + 0.4f * (float)rng.NextDouble()));
                    v = Mathf.Clamp01(v * (0.82f + 0.36f * (float)rng.NextDouble()));
                    c = Color.HSVToRGB(h, s, v);
                }
                m.SetColor("_BaseColor", c);
                m.SetFloat("_Smoothness", n.StartsWith("Hair_") ? 0.32f : 0.16f);
            }
            else if (n.StartsWith("Skin_"))
            {
                // Skin_Regular_Male_Light → T_Regular_Male_Light_BaseColor + T_Regular_Male_Normal
                string key = n.Substring(5);
                string shape = key.Substring(0, key.LastIndexOf('_'));
                m.SetTexture("_BaseMap", Load(Tex + "T_" + key + "_BaseColor.png", false));
                var nrm = Load(Tex + "T_" + shape + "_Normal.png", true);
                if (nrm != null) { m.SetTexture("_BumpMap", nrm); m.EnableKeyword("_NORMALMAP"); }
                m.SetColor("_BaseColor", Color.white);
                m.SetFloat("_Smoothness", 0.34f);
            }
            else if (n.StartsWith("MI_Eyes"))
            {
                m.SetTexture("_BaseMap", Load(Tex + "T_Eye_Brown.png", false));
                m.SetColor("_BaseColor", Color.white);
                m.SetFloat("_Smoothness", 0.7f);
            }
            else
            {
                m.SetColor("_BaseColor", new Color(0.3f, 0.26f, 0.22f));
                Debug.Log("[Proto] townsfolk: unmapped material " + src);
            }
            Directory.CreateDirectory(MatLib.GenDir + "/Townsfolk");
            AssetDatabase.CreateAsset(m, $"{MatLib.GenDir}/Townsfolk/{n}.mat");
            mats[n] = m;
            return m;
        }

        static Texture2D Load(string path, bool normal)
        {
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) { Debug.LogWarning("[Proto] missing texture " + path); return null; }
            var want = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            if (imp.textureType != want || imp.maxTextureSize != 1024)
            {
                imp.textureType = want; imp.maxTextureSize = 1024; imp.sRGBTexture = !normal;
                imp.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}
