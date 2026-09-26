using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Juego2.ART.Editor
{
    /// <summary>Explicit built-in-render-pipeline Standard materials for the selected ART slice.</summary>
    public static class Art01Materials
    {
        const string MatRoot = "Assets/Arkus/ART/Materials";
        const string SrcRoot = "Assets/Arkus/ART/External";
        static readonly Dictionary<string, Material> Cache = new Dictionary<string, Material>();

        static Texture2D Tex(string relative)
        {
            if (relative == null) return null;
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(SrcRoot + "/" + relative);
            if (texture == null) throw new Exception("ART01_TEXTURE_MISSING " + relative);
            return texture;
        }

        static Material Create(string id, string hex, string texturePath = null, bool cutout = false,
            float smoothness = 0.15f)
        {
            if (Cache.TryGetValue(id, out var cached)) return cached;
            var shader = Shader.Find("Standard");
            if (shader == null) throw new Exception("ART01_STANDARD_SHADER_MISSING");
            var path = MatRoot + "/" + id + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                if (!AssetDatabase.IsValidFolder(MatRoot))
                    AssetDatabase.CreateFolder("Assets/Arkus/ART", "Materials");
                material = new Material(shader) { name = id };
                AssetDatabase.CreateAsset(material, path);
            }
            material.shader = shader;
            if (!ColorUtility.TryParseHtmlString(hex, out var color)) throw new Exception("ART01_COLOR_INVALID " + hex);
            material.SetColor("_Color", color);
            material.SetTexture("_MainTex", Tex(texturePath));
            material.SetFloat("_Glossiness", smoothness);
            material.SetFloat("_Metallic", 0);
            if (cutout)
            {
                material.SetFloat("_Mode", 1);
                material.SetFloat("_Cutoff", 0.35f);
                material.EnableKeyword("_ALPHATEST_ON");
                material.renderQueue = 2450;
            }
            else
            {
                material.SetFloat("_Mode", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.renderQueue = -1;
            }
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            Cache[id] = material;
            return material;
        }

        public static Material Get(string id)
        {
            switch (id)
            {
                case "Stone": return Create(id, "#C9C3B6", "Medieval/Textures/T_UnevenBrick_BaseColor.png");
                case "StoneShadow": return Create(id, "#7E7568", "Medieval/Textures/T_UnevenBrick_BaseColor.png");
                case "StoneTrim": return Create(id, "#A79F92", "Medieval/Textures/T_Brick_BaseColor.png");
                case "Plaster": return Create(id, "#C9C3B6", "Medieval/Textures/T_Plaster_BaseColor.png");
                case "PlasterOchre": return Create(id, "#B8A994", "Medieval/Textures/T_Plaster_BaseColor.png");
                case "WoodDark": return Create(id, "#5C4433", "Medieval/Textures/T_WoodTrim_BaseColor.png");
                case "ShutterGreen": return Create(id, "#3E5A57", "Medieval/Textures/T_WoodTrim_BaseColor.png");
                case "TileWet": return Create(id, "#574A45", "Medieval/Textures/T_RoundTiles_BaseColor.png", false, 0.25f);
                case "Cobble": return Create(id, "#8A8379", "Medieval/Textures/T_RoundRocks_BaseColor.png", false, 0.22f);
                case "Iron": return Create(id, "#373B3B", null, false, 0.28f);
                case "Glass": return Create(id, "#40545B", null, false, 0.45f);
                case "Water": return Create(id, "#3F5A5E", null, false, 0.48f);
                case "GrassGround": return Create(id, "#4E7A43", "Nature/Textures/Grass.png");
                case "Bark": return Create(id, "#A9A297", "Nature/Textures/Bark_NormalTree.png");
                case "Leaves": return Create(id, "#77906D", "Nature/Textures/Leaves_NormalTree.png", true);
                case "LeavesGeneric": return Create(id, "#68865D", "Nature/Textures/Leaves.png", true);
                case "Flowers": return Create(id, "#919CC7", "Nature/Textures/Flowers.png", true);
                case "Rock": return Create(id, "#A79F92", "Nature/Textures/Rocks_Diffuse.png");
                case "PropWood": return Create(id, "#A89B88", "Props/Textures/T_Trim_Furniture_BaseColor.png");
                case "PropMetal": return Create(id, "#A3A2A0", "Props/Textures/T_Trim_Metal_BaseColor.png", false, 0.35f);
                case "PropOther": return Create(id, "#B8AC96", "Props/Textures/T_Trim_Props_BaseColor.png");
                case "PropCloth": return Create(id, "#92897D", "Props/Textures/T_Trim_Cloth_BaseColor.png");
                case "Skin": return Create(id, "#F1E5D6", "Characters/T_Regular_Male_Light_BaseColor.png");
                case "Eye": return Create(id, "#FFFFFF", "Characters/T_Eye_Brown.png");
                case "Hair": return Create(id, "#58483F", "Characters/T_Hair_1_BaseColor.png");
                case "Coat": return Create(id, "#344F51");
                case "Trousers": return Create(id, "#3A3530");
                case "Shirt": return Create(id, "#D8D2C4");
                default: throw new Exception("ART01_MATERIAL_UNKNOWN " + id);
            }
        }

        public static Material ForSource(string name)
        {
            if (name.Contains("UnevenBrick")) return Get("Stone");
            if (name.Contains("RockTrim")) return Get("StoneTrim");
            if (name.Contains("Brick")) return Get("StoneTrim");
            if (name.Contains("Plaster")) return Get("Plaster");
            if (name.Contains("WoodTrim")) return Get("WoodDark");
            if (name.Contains("RoundTiles")) return Get("TileWet");
            if (name.Contains("RoundRocks")) return Get("Cobble");
            if (name.Contains("WindowGlass")) return Get("Glass");
            if (name.Contains("MetalOrnaments")) return Get("Iron");
            if (name.Contains("Bark")) return Get("Bark");
            if (name.Contains("Leaves")) return Get("Leaves");
            if (name.Contains("Flowers")) return Get("Flowers");
            if (name == "Grass") return Get("GrassGround");
            if (name.Contains("Rocks")) return Get("Rock");
            if (name.Contains("Trim_Furniture")) return Get("PropWood");
            if (name.Contains("Trim_Metal")) return Get("PropMetal");
            if (name.Contains("Trim_Props")) return Get("PropOther");
            if (name.Contains("Trim_Cloth")) return Get("PropCloth");
            if (name.Contains("Regular_Male")) return Get("Skin");
            if (name.Contains("Eye")) return Get("Eye");
            if (name.Contains("Hair")) return Get("Hair");
            throw new Exception("ART01_SOURCE_MATERIAL_UNMAPPED " + name);
        }

        public static void Remap(GameObject instance, string overrideId = null)
        {
            foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                var old = renderer.sharedMaterials;
                var replacement = new Material[old.Length];
                for (int i = 0; i < old.Length; i++)
                {
                    if (old[i] == null) throw new Exception("ART01_NULL_SOURCE_MATERIAL " + instance.name);
                    replacement[i] = overrideId == null ? ForSource(old[i].name) : Get(overrideId);
                }
                renderer.sharedMaterials = replacement;
            }
        }
    }
}
