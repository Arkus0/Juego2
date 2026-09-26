using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Proto.Build
{
    /// Materials and generated textures. Kit textures are used as-is; only tints/params change.
    public static class MatLib
    {
        public const string GenDir = "Assets/_Derived/Generated";
        public const string MatDir = "Assets/_Derived/Materials";
        const string Kit = "Assets/ThirdParty/Quaternius/MedievalVillage/Materials/";
        const string Nat = "Assets/ThirdParty/Quaternius/Nature/Textures/";
        const string Prp = "Assets/ThirdParty/Quaternius/Props/Textures/";

        static readonly Dictionary<string, Material> cache = new Dictionary<string, Material>();

        public static Shader Lit => Shader.Find("Universal Render Pipeline/Lit");

        public static Texture2D Tex(string path)
        {
            var t = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (t == null) Debug.LogWarning("[Proto] missing texture " + path);
            return t;
        }

        public static void EnsureDirs()
        {
            Directory.CreateDirectory(GenDir + "/Textures");
            Directory.CreateDirectory(MatDir);
        }

        public static Material Get(string name) => cache.TryGetValue(name, out var m) ? m : Build(name);

        static Material Save(string name, Material m)
        {
            string path = $"{MatDir}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) { existing.shader = m.shader; existing.CopyPropertiesFromMaterial(m); existing.shaderKeywords = m.shaderKeywords; EditorUtility.SetDirty(existing); m = existing; }
            else AssetDatabase.CreateAsset(m, path);
            cache[name] = m;
            return m;
        }

        static Material LitMat(string name, Color c, Texture2D baseMap = null, Texture2D normal = null, float smooth = 0.2f, float tiling = 1f, bool clip = false, bool twoSided = false, float normalScale = 1f)
        {
            var m = new Material(Lit) { name = name };
            m.SetColor("_BaseColor", c);
            if (baseMap) m.SetTexture("_BaseMap", baseMap);
            if (normal) { m.SetTexture("_BumpMap", normal); m.SetFloat("_BumpScale", normalScale); m.EnableKeyword("_NORMALMAP"); }
            m.SetFloat("_Smoothness", smooth);
            m.SetFloat("_Metallic", 0);
            m.SetTextureScale("_BaseMap", Vector2.one * tiling);
            if (clip) { m.SetFloat("_AlphaClip", 1); m.SetFloat("_Cutoff", 0.4f); m.EnableKeyword("_ALPHATEST_ON"); m.renderQueue = (int)RenderQueue.AlphaTest; }
            if (twoSided) m.SetFloat("_Cull", 0);
            m.enableInstancing = true;
            return m;
        }

        static Material Build(string name)
        {
            switch (name)
            {
                // procedural architecture (bank walls, bridge, plinths, tapias)
                case "Stone": return Save(name, LitMat(name, new Color(0.86f, 0.85f, 0.83f), Tex(Kit + "T_UnevenBrick_BaseColor.png"), NormalTex(Kit + "T_UnevenBrick_Normal.png"), 0.12f));
                case "StoneDark": return Save(name, LitMat(name, new Color(0.62f, 0.6f, 0.57f), Tex(Kit + "T_UnevenBrick_BaseColor.png"), NormalTex(Kit + "T_UnevenBrick_Normal.png"), 0.18f));
                case "StoneWet": return Save(name, LitMat(name, new Color(0.5f, 0.52f, 0.5f), Tex(Kit + "T_UnevenBrick_BaseColor.png"), NormalTex(Kit + "T_UnevenBrick_Normal.png"), 0.42f));
                case "Coping": return Save(name, LitMat(name, new Color(0.78f, 0.77f, 0.74f), Tex(Kit + "T_Brick_BaseColor.png"), NormalTex(Kit + "T_Brick_Normal.png"), 0.2f));
                case "Cobble": return Save(name, LitMat(name, new Color(0.8f, 0.8f, 0.8f), Tex(Kit + "T_RoundRocks_BaseColor.png"), NormalTex(Kit + "T_RoundRocks_Normal.png"), 0.38f));
                case "Plaster": return Save(name, LitMat(name, new Color(0.93f, 0.91f, 0.87f), Tex(Kit + "T_Plaster_BaseColor.png"), NormalTex(Kit + "T_Plaster_Normal.png"), 0.1f));
                case "Wood": return Save(name, LitMat(name, new Color(0.62f, 0.52f, 0.45f), Tex(Kit + "T_WoodTrim_BaseColor.png"), NormalTex(Kit + "T_WoodTrim_Normal.png"), 0.15f));
                case "WoodDark": return Save(name, LitMat(name, new Color(0.36f, 0.28f, 0.23f), Tex(Kit + "T_WoodTrim_BaseColor.png"), NormalTex(Kit + "T_WoodTrim_Normal.png"), 0.2f));
                case "Iron": return Save(name, LitMat(name, new Color(0.16f, 0.16f, 0.17f), null, null, 0.35f));
                case "Water": return Water(name);
                case "Rock": return Save(name, LitMat(name, new Color(0.62f, 0.61f, 0.58f), GenTex("rock", 512), null, 0.15f));
                case "Paper": return Save(name, LitMat(name, new Color(0.9f, 0.88f, 0.8f), null, null, 0.05f));
                case "SignBoard": return Save(name, LitMat(name, new Color(0.22f, 0.3f, 0.3f), null, null, 0.25f));
                case "Awning": return Save(name, LitMat(name, new Color(0.62f, 0.25f, 0.18f), Tex(Prp + "T_Trim_Cloth_BaseColor.png"), null, 0.05f, 1, false, true));
                case "Cloth": return Save(name, LitMat(name, Color.white, null, null, 0.05f, 1, false, true));
                case "Candle": return Emissive(name, new Color(1f, 0.72f, 0.38f), 6f);
                case "WarmGlass": return Emissive(name, new Color(0.85f, 0.55f, 0.28f), 0.55f);
                // renders over the stone wall modules (lime wash, ochre, faded rose) and village materials
                case "Lime": return Save(name, LitMat(name, new Color(0.9f, 0.9f, 0.88f), null, NormalTex(Kit + "T_Plaster_Normal.png"), 0.06f));
                case "Ochre": return Save(name, LitMat(name, new Color(0.9f, 0.74f, 0.5f), Tex(Kit + "T_Plaster_BaseColor.png"), NormalTex(Kit + "T_Plaster_Normal.png"), 0.08f));
                case "Rose": return Save(name, LitMat(name, new Color(0.9f, 0.72f, 0.66f), Tex(Kit + "T_Plaster_BaseColor.png"), NormalTex(Kit + "T_Plaster_Normal.png"), 0.08f));
                case "Straw": return Save(name, LitMat(name, new Color(0.86f, 0.72f, 0.42f), GenTex("gravel", 512), null, 0.03f, 3f));
                case "Soil": return Save(name, LitMat(name, new Color(0.5f, 0.4f, 0.32f), GenTex("dirt", 512), null, 0.06f, 2f));
                case "Boards": return Save(name, LitMat(name, new Color(0.55f, 0.46f, 0.38f), Tex(Kit + "T_WoodTrim_BaseColor.png"), NormalTex(Kit + "T_WoodTrim_Normal.png"), 0.12f));
                case "Soot": return Save(name, LitMat(name, new Color(0.05f, 0.045f, 0.04f), null, null, 0.05f));
                case "Fire": return Emissive(name, new Color(1f, 0.55f, 0.2f), 5f);
                case "Bread": return Save(name, LitMat(name, new Color(0.66f, 0.43f, 0.22f), Tex(Kit + "T_Plaster_BaseColor.png"), null, 0.22f));
                case "Cheese": return Save(name, LitMat(name, new Color(0.96f, 0.88f, 0.6f), null, null, 0.3f));
                case "Rind": return Save(name, LitMat(name, new Color(0.78f, 0.6f, 0.32f), Tex(Kit + "T_Plaster_BaseColor.png"), null, 0.2f));
                case "Ham": return Save(name, LitMat(name, new Color(0.44f, 0.21f, 0.15f), Tex(Kit + "T_Plaster_BaseColor.png"), null, 0.38f));
                case "Mannequin": return Save(name, LitMat(name, new Color(0.3f, 0.34f, 0.4f), null, null, 0.3f));
                // dark wet tile: the kit tile material with a darker tint (VISUAL_BIBLE teja #4A3730)
                // dark wet tile / dark timber / green-blue shutters: kit materials with luminance-remapped base textures
                case "TileDark": return KitRemap(name, "MI_RoundTiles", "T_RoundTiles_BaseColor", new Color(0.09f, 0.075f, 0.07f), new Color(0.36f, 0.31f, 0.29f));
                case "TileDarkFlat": return KitRemap(name, "MI_FlatTiles", "T_FlatTiles_BaseColor", new Color(0.13f, 0.10f, 0.09f), new Color(0.46f, 0.38f, 0.34f));
                case "TrimDark": return KitRemap(name, "MI_WoodTrim", "T_WoodTrim_BaseColor", new Color(0.12f, 0.08f, 0.06f), new Color(0.45f, 0.34f, 0.25f));
                case "ShutterGreen": return KitRemap(name, "MI_WoodTrim_Wear", "T_WoodTrim_BaseColor", new Color(0.10f, 0.16f, 0.16f), new Color(0.36f, 0.52f, 0.5f));
                case "ShutterRed": return KitRemap(name, "MI_WoodTrim_Wear", "T_WoodTrim_BaseColor", new Color(0.18f, 0.06f, 0.05f), new Color(0.56f, 0.24f, 0.2f));
                case "ShutterBlue": return KitRemap(name, "MI_WoodTrim_Wear", "T_WoodTrim_BaseColor", new Color(0.09f, 0.12f, 0.17f), new Color(0.38f, 0.46f, 0.56f));
                case "ShutterWood": return KitRemap(name, "MI_WoodTrim_Wear", "T_WoodTrim_BaseColor", new Color(0.16f, 0.1f, 0.06f), new Color(0.58f, 0.44f, 0.3f));
                // nature / props remaps
                case "Bark": return Save(name, LitMat(name, new Color(0.8f, 0.78f, 0.75f), Tex(Nat + "Bark_NormalTree.png"), NormalTex(Nat + "Bark_NormalTree_Normal.png"), 0.1f));
                case "Leaves": return Save(name, LitMat(name, new Color(0.34f, 0.44f, 0.30f), Tex(Nat + "Leaves_NormalTree.png"), null, 0.1f, 1, true, true));
                case "LeavesPine": return Save(name, LitMat(name, new Color(0.26f, 0.36f, 0.28f), Tex(Nat + "Leaf_Pine.png"), null, 0.1f, 1, true, true));
                case "LeavesGeneric": return Save(name, LitMat(name, new Color(0.38f, 0.48f, 0.33f), Tex(Nat + "Leaves.png"), null, 0.1f, 1, true, true));
                case "Flowers": return Save(name, LitMat(name, new Color(0.62f, 0.66f, 1.0f), Tex(Nat + "Flowers.png"), null, 0.1f, 1, true, true));
                case "Grass": return Save(name, LitMat(name, new Color(0.42f, 0.52f, 0.36f), Tex(Nat + "Grass.png"), null, 0.05f, 1, true, true));
                case "NatRocks": return Save(name, LitMat(name, new Color(0.8f, 0.8f, 0.78f), Tex(Nat + "Rocks_Diffuse.png"), null, 0.15f));
                case "PathRocks": return Save(name, LitMat(name, new Color(0.8f, 0.8f, 0.78f), Tex(Nat + "PathRocks_Diffuse.png"), null, 0.3f));
                case "Mushrooms": return Save(name, LitMat(name, Color.white, Tex(Nat + "Mushrooms.png"), null, 0.1f));
                case "TrimFurniture": return Save(name, LitMat(name, Color.white, Tex(Prp + "T_Trim_Furniture_BaseColor.png"), NormalTex(Prp + "T_Trim_Furniture_Normal.png"), 0.2f));
                case "TrimMetal": return Save(name, LitMat(name, Color.white, Tex(Prp + "T_Trim_Metal_BaseColor.png"), NormalTex(Prp + "T_Trim_Metal_Normal.png"), 0.45f));
                case "TrimProps": return Save(name, LitMat(name, Color.white, Tex(Prp + "T_Trim_Props_BaseColor.png"), NormalTex(Prp + "T_Trim_Props_Normal.png"), 0.25f));
                case "TrimCloth": return Save(name, LitMat(name, Color.white, Tex(Prp + "T_Trim_Cloth_BaseColor.png"), NormalTex(Prp + "T_Trim_Cloth_Normal.png"), 0.05f, 1, false, true));
            }
            Debug.LogError("[Proto] unknown material " + name);
            return Save(name, LitMat(name, Color.magenta));
        }

        /// Copy of a kit material whose base texture is re-mapped by luminance to a dark..light palette ramp.
        static Material KitRemap(string name, string kitMat, string tex, Color dark, Color light)
        {
            var src = AssetDatabase.LoadAssetAtPath<Material>(Kit + kitMat + ".mat");
            var m = new Material(src) { name = name };
            m.SetTexture("_Base_Color_Texture", Remapped(Kit + tex + ".png", name, dark, light));
            m.SetColor("_Color", Color.white);
            return Save(name, m);
        }

        static Texture2D Remapped(string srcPath, string name, Color dark, Color light)
        {
            string path = $"{GenDir}/Textures/remap_{name}.png";
            if (!File.Exists(path))
            {
                var imp = (TextureImporter)AssetImporter.GetAtPath(srcPath);
                bool was = imp.isReadable;
                if (!was) { imp.isReadable = true; imp.SaveAndReimport(); }
                var src = AssetDatabase.LoadAssetAtPath<Texture2D>(srcPath);
                var px = src.GetPixels();
                float lo = 1, hi = 0;
                foreach (var c in px) { float l = c.grayscale; lo = Mathf.Min(lo, l); hi = Mathf.Max(hi, l); }
                for (int i = 0; i < px.Length; i++)
                {
                    float l = Mathf.InverseLerp(lo, hi, px[i].grayscale);
                    px[i] = Color.Lerp(dark, light, Mathf.Pow(l, 0.9f));
                }
                var t = new Texture2D(src.width, src.height, TextureFormat.RGB24, true);
                t.SetPixels(px); t.Apply();
                File.WriteAllBytes(path, t.EncodeToPNG());
                Object.DestroyImmediate(t);
                if (!was) { imp.isReadable = false; imp.SaveAndReimport(); }
            }
            return Import(path, false);
        }

        static Material KitVariant(string name, string kitMat, Color tint)
        {
            var src = AssetDatabase.LoadAssetAtPath<Material>(Kit + kitMat + ".mat");
            var m = new Material(src) { name = name };
            m.SetColor("_Color", tint);
            return Save(name, m);
        }

        static Material Emissive(string name, Color c, float intensity)
        {
            var m = LitMat(name, c, null, null, 0.5f);
            m.SetColor("_EmissionColor", c * intensity);
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            return Save(name, m);
        }

        static Material Water(string name)
        {
            var m = LitMat(name, new Color(0.2f, 0.29f, 0.3f), null, GenNormal("water_n", 512), 0.93f, 1f, false, false, 0.35f);
            m.SetTextureScale("_BaseMap", new Vector2(0.06f, 0.06f));
            m.SetFloat("_SpecularHighlights", 1);
            m.SetFloat("_EnvironmentReflections", 1);
            return Save(name, m);
        }

        public static Texture2D NormalTex(string path)
        {
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp != null && imp.textureType != TextureImporterType.NormalMap)
            {
                imp.textureType = TextureImporterType.NormalMap;
                imp.SaveAndReimport();
            }
            return Tex(path);
        }

        // ---------- generated, tileable textures ----------

        static float PNoise(float x, float y, int period, int seed)
        {
            int xi = Mathf.FloorToInt(x), yi = Mathf.FloorToInt(y);
            float fx = x - xi, fy = y - yi;
            float H(int a, int b) => Geo.Hash(((a % period) + period) % period + seed * 131, ((b % period) + period) % period + seed * 71);
            float sx = fx * fx * (3 - 2 * fx), sy = fy * fy * (3 - 2 * fy);
            return Mathf.Lerp(Mathf.Lerp(H(xi, yi), H(xi + 1, yi), sx), Mathf.Lerp(H(xi, yi + 1), H(xi + 1, yi + 1), sx), sy);
        }

        static float PFbm(float x, float y, int basePeriod, int oct, int seed)
        {
            float s = 0, a = 0.5f, f = 1, norm = 0;
            for (int i = 0; i < oct; i++) { s += a * PNoise(x * f, y * f, basePeriod * (int)f, seed + i); norm += a; f *= 2; a *= 0.5f; }
            return s / norm;
        }

        public static Texture2D GenTex(string kind, int size)
        {
            string path = $"{GenDir}/Textures/{kind}.png";
            if (File.Exists(path)) return Import(path, false);
            var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
            var px = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float u = (float)x / size, v = (float)y / size;
                    Color c;
                    switch (kind)
                    {
                        case "grass":
                        {
                            float big = PFbm(u * 4, v * 4, 4, 4, 1), fine = PFbm(u * 64, v * 64, 64, 2, 2), blade = Geo.Hash(x, y);
                            var baseC = Color.Lerp(new Color(0.22f, 0.29f, 0.19f), new Color(0.31f, 0.38f, 0.25f), big);
                            c = Color.Lerp(baseC, new Color(0.40f, 0.42f, 0.29f), Mathf.Clamp01((fine - 0.55f) * 3f) * 0.45f);
                            c *= 0.9f + 0.2f * blade;
                            break;
                        }
                        case "dirt":
                        {
                            float big = PFbm(u * 4, v * 4, 4, 4, 5), fine = PFbm(u * 48, v * 48, 48, 3, 6), peb = Geo.Hash(x * 3, y * 7);
                            c = Color.Lerp(new Color(0.3f, 0.26f, 0.21f), new Color(0.42f, 0.37f, 0.3f), big * 0.6f + fine * 0.4f);
                            if (peb > 0.985f) c = new Color(0.55f, 0.53f, 0.5f);
                            break;
                        }
                        case "rock":
                        {
                            float big = PFbm(u * 3, v * 3, 3, 5, 9), cr = Mathf.Abs(PFbm(u * 8, v * 8, 8, 3, 10) - 0.5f);
                            c = Color.Lerp(new Color(0.42f, 0.41f, 0.39f), new Color(0.66f, 0.65f, 0.61f), big);
                            if (cr < 0.02f) c *= 0.7f;
                            c = Color.Lerp(c, new Color(0.36f, 0.42f, 0.3f), Mathf.Clamp01(PFbm(u * 6, v * 6, 6, 3, 11) - 0.62f) * 2.5f);
                            break;
                        }
                        case "gravel":
                        {
                            float h = Geo.Hash(x / 3, y / 3), big = PFbm(u * 5, v * 5, 5, 3, 13);
                            c = Color.Lerp(new Color(0.36f, 0.35f, 0.32f), new Color(0.6f, 0.58f, 0.54f), h * 0.6f + big * 0.4f);
                            break;
                        }
                        default: c = Color.gray; break;
                    }
                    px[y * size + x] = c;
                }
            tex.SetPixels(px);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            return Import(path, false);
        }

        public static Texture2D GenNormal(string kind, int size)
        {
            string path = $"{GenDir}/Textures/{kind}.png";
            if (!File.Exists(path))
            {
                var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
                var hgt = new float[size, size];
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                        hgt[x, y] = PFbm((float)x / size * 8, (float)y / size * 8, 8, 4, 21);
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        float dx = hgt[(x + 1) % size, y] - hgt[(x + size - 1) % size, y];
                        float dy = hgt[x, (y + 1) % size] - hgt[x, (y + size - 1) % size];
                        var n = new Vector3(-dx * 6f, -dy * 6f, 1).normalized;
                        tex.SetPixel(x, y, new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z * 0.5f + 0.5f));
                    }
                tex.Apply();
                File.WriteAllBytes(path, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
            }
            return Import(path, true);
        }

        static Texture2D Import(string path, bool normal)
        {
            AssetDatabase.ImportAsset(path);
            var imp = (TextureImporter)AssetImporter.GetAtPath(path);
            imp.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            imp.wrapMode = TextureWrapMode.Repeat;
            imp.mipmapEnabled = true;
            imp.anisoLevel = 4;
            imp.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        public static TerrainLayer Layer(string name, Texture2D diffuse, Texture2D normal, float tile, float smooth, Color tint)
        {
            string path = $"{GenDir}/TL_{name}.terrainlayer";
            var l = AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);
            bool create = l == null;
            if (create) l = new TerrainLayer();
            l.diffuseTexture = diffuse;
            l.normalMapTexture = normal;
            l.tileSize = new Vector2(tile, tile);
            l.smoothness = smooth;
            l.metallic = 0;
            l.diffuseRemapMax = new Vector4(tint.r, tint.g, tint.b, 1);
            l.normalScale = 1f;
            if (create) AssetDatabase.CreateAsset(l, path); else EditorUtility.SetDirty(l);
            return l;
        }

        // ---------- remap imported FBX materials of Nature/Props to our URP materials ----------
        static readonly Dictionary<string, string> Remap = new Dictionary<string, string>
        {
            { "Bark_NormalTree", "Bark" }, { "Bark_DeadTree", "Bark" }, { "Bark_TwistedTree", "Bark" },
            { "Leaves_NormalTree", "Leaves" }, { "Leaves_Pine", "LeavesPine" }, { "Leaves_TwistedTree", "Leaves" }, { "Leaves_GiantPine", "LeavesPine" },
            { "Leaves", "LeavesGeneric" }, { "Flowers", "Flowers" }, { "Grass", "Grass" }, { "Rocks", "NatRocks" }, { "PathRocks", "PathRocks" },
            { "Mushrooms", "Mushrooms" }, { "Rocks_Desert", "NatRocks" },
            { "MI_Trim_Furniture", "TrimFurniture" }, { "MI_Trim_Metal", "TrimMetal" }, { "MI_Trim_Props", "TrimProps" }, { "MI_Trim_Cloth", "TrimCloth" },
            { "UAL1-M_Main", "Mannequin" }, { "UAL2-M_Main", "Mannequin" },
            { "T_Trim_Furniture", "TrimFurniture" }, { "T_Trim_Metal", "TrimMetal" }, { "T_Trim_Props", "TrimProps" }, { "T_Trim_Cloth", "TrimCloth" },
        };

        /// URP replacement for a legacy/imported material by name (null if unknown).
        public static Material ReplacementFor(Material m)
        {
            if (m == null) return null;
            if (Remap.TryGetValue(m.name, out var t)) return Get(t);
            foreach (var kv in Remap.OrderByDescending(k => k.Key.Length)) if (m.name.StartsWith(kv.Key)) return Get(kv.Value);
            return null;
        }

        public static void RemapImportedMaterials()
        {
            // one batched pass: centimetre scale, embedded materials and every known name -> URP remap
            var models = new System.Collections.Generic.List<ModelImporter>();
            foreach (var dir in new[] { "Assets/ThirdParty/Quaternius/Nature/Models", "Assets/ThirdParty/Quaternius/Props/Models" })
                foreach (var guid in AssetDatabase.FindAssets("t:Model", new[] { dir }))
                    models.Add((ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)));
            var targets = Remap.ToDictionary(kv => kv.Key, kv => Get(kv.Value));
            var dirty = new System.Collections.Generic.List<ModelImporter>();
            foreach (var imp in models)
            {
                bool changed = false;
                if (Mathf.Abs(imp.globalScale - 0.01f) > 1e-5f || imp.materialLocation != ModelImporterMaterialLocation.InPrefab)
                {
                    imp.globalScale = 0.01f; imp.useFileScale = false;
                    imp.materialLocation = ModelImporterMaterialLocation.InPrefab;
                    changed = true;
                }
                var existing = imp.GetExternalObjectMap();
                foreach (var kv in targets)
                {
                    var id = new AssetImporter.SourceAssetIdentifier(typeof(Material), kv.Key);
                    if (existing.TryGetValue(id, out var cur) && cur == kv.Value) continue;
                    imp.AddRemap(id, kv.Value);
                    changed = true;
                }
                if (changed) dirty.Add(imp);
            }
            if (dirty.Count == 0) return;
            AssetDatabase.StartAssetEditing();
            try { foreach (var imp in dirty) { EditorUtility.SetDirty(imp); imp.SaveAndReimport(); } }
            finally { AssetDatabase.StopAssetEditing(); }
            Debug.Log("[Proto] remapped/reimported models: " + dirty.Count);
        }
    }
}
