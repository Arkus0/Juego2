using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Proto.Build
{
    /// Prefab lookup (Quaternius kit, Nature, Props) + material overrides applied on placement.
    public static class Kit
    {
        static Dictionary<string, GameObject> map;
        static HashSet<string> zUp;          // Props pack exports are Z-up (not the "FBX (Unity)" variant)
        static Dictionary<Material, Material> swaps;

        static void Load()
        {
            if (map != null) return;
            map = new Dictionary<string, GameObject>();
            zUp = new HashSet<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/ThirdParty/Quaternius/MedievalVillage/Modules/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                map[Path.GetFileNameWithoutExtension(path)] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            foreach (var dir in new[] { "Assets/ThirdParty/Quaternius/Nature/Models", "Assets/ThirdParty/Quaternius/Props/Models", "Assets/ThirdParty/Quaternius/UAL/Models" })
                foreach (var guid in AssetDatabase.FindAssets("t:Model", new[] { dir }))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    map[Path.GetFileNameWithoutExtension(path)] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (dir.EndsWith("Props/Models")) zUp.Add(Path.GetFileNameWithoutExtension(path));
                }
            const string K = "Assets/ThirdParty/Quaternius/MedievalVillage/Materials/";
            swaps = new Dictionary<Material, Material>
            {
                { AssetDatabase.LoadAssetAtPath<Material>(K + "MI_RoundTiles.mat"), MatLib.Get("TileDark") },
                { AssetDatabase.LoadAssetAtPath<Material>(K + "MI_FlatTiles.mat"), MatLib.Get("TileDarkFlat") },
                { AssetDatabase.LoadAssetAtPath<Material>(K + "MI_WoodTrim.mat"), MatLib.Get("TrimDark") },
            };
        }

        public static void Reset() { map = null; swaps = null; }

        public static bool Has(string name) { Load(); return map.ContainsKey(name); }

        public static GameObject Put(string name, Transform parent, Vector3 pos, Quaternion rot, Vector3? scale = null, bool swap = true, bool colliders = true)
        {
            Load();
            if (!map.TryGetValue(name, out var prefab)) { Debug.LogWarning("[Proto] missing kit piece " + name); return null; }
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            if (zUp.Contains(name)) rot = rot * Quaternion.Euler(-90f, 0f, 0f);
            go.transform.SetPositionAndRotation(pos, rot);
            if (scale.HasValue) go.transform.localScale = scale.Value;
            if (swap) Swap(go);
            if (!colliders) foreach (var c in go.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(c);
            go.isStatic = true;
            foreach (Transform t in go.GetComponentsInChildren<Transform>()) t.gameObject.isStatic = true;
            return go;
        }

        public static void Swap(GameObject go, Dictionary<Material, Material> extra = null)
        {
            Load();
            foreach (var r in go.GetComponentsInChildren<Renderer>())
            {
                var mats = r.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] == null) continue;
                    if (extra != null && extra.TryGetValue(mats[i], out var e)) { mats[i] = e; changed = true; }
                    else if (swaps.TryGetValue(mats[i], out var m)) { mats[i] = m; changed = true; }
                    else if (mats[i].shader != null && (mats[i].shader.name == "Standard" || mats[i].shader.name.StartsWith("Legacy") || mats[i].shader.name.Contains("InternalErrorShader")))
                    {
                        var rep = MatLib.ReplacementFor(mats[i]);
                        if (rep != null) { mats[i] = rep; changed = true; }
                        else Debug.LogWarning("[Proto] no URP replacement for " + mats[i].name);
                    }
                }
                if (changed) r.sharedMaterials = mats;
            }
        }

        public static void Tint(GameObject go, string kitMat, Material to)
        {
            foreach (var r in go.GetComponentsInChildren<Renderer>())
            {
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++) if (mats[i] != null && mats[i].name.StartsWith(kitMat)) mats[i] = to;
                r.sharedMaterials = mats;
            }
        }

        public static IEnumerable<string> Names(string prefix) { Load(); return map.Keys.Where(k => k.StartsWith(prefix)); }
    }
}
