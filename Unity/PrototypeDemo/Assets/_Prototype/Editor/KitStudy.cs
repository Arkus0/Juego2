using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Proto.EditorTools
{
    /// Throwaway investigation: how the Quaternius kit is meant to be assembled and how it renders in URP.
    public static class KitStudy
    {
        const string Kit = "Assets/ThirdParty/Quaternius/MedievalVillage/";
        static string Out => Path.GetFullPath("Captures/KitStudy");

        [MenuItem("Prototype/Study/Run Kit Study")]
        public static void Run()
        {
            Directory.CreateDirectory(Out);
            DumpSampleScene();
            RenderSampleScene();
            RenderPieces();
            DumpUAL();
            Debug.Log("[Proto] Kit study written to " + Out);
        }

        static void DumpSampleScene()
        {
            var scene = EditorSceneManager.OpenScene(Kit + "Levels/L_SampleScene_1.unity", OpenSceneMode.Single);
            var sb = new StringBuilder();
            foreach (var root in scene.GetRootGameObjects())
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    var src = PrefabUtility.GetCorrespondingObjectFromOriginalSource(t.gameObject);
                    if (src == null || !PrefabUtility.IsOutermostPrefabInstanceRoot(t.gameObject)) continue;
                    var p = t.position; var e = t.rotation.eulerAngles; var s = t.lossyScale;
                    sb.AppendLine($"{src.name}\t{p.x:F3}\t{p.y:F3}\t{p.z:F3}\t{e.x:F1}\t{e.y:F1}\t{e.z:F1}\t{s.x:F3}\t{s.y:F3}\t{s.z:F3}");
                }
            File.WriteAllText(Path.Combine(Out, "sample_scene_instances.tsv"), sb.ToString());
        }

        static void Lighting()
        {
            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.1f;
            sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(50, -30, 0);
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.72f, 0.77f, 0.79f);
            RenderSettings.ambientEquatorColor = new Color(0.58f, 0.64f, 0.67f);
            RenderSettings.ambientGroundColor = new Color(0.35f, 0.34f, 0.3f);
        }

        static void RenderSampleScene()
        {
            Lighting();
            var bounds = new Bounds();
            bool first = true;
            foreach (var r in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            {
                if (first) { bounds = r.bounds; first = false; } else bounds.Encapsulate(r.bounds);
            }
            File.WriteAllText(Path.Combine(Out, "sample_bounds.txt"), bounds.ToString());
            var c = bounds.center;
            Shot(new Vector3(c.x + bounds.extents.x * 1.1f, c.y + bounds.extents.y * 2.5f + 15, c.z - bounds.extents.z * 1.2f), c, "sample_overview.png");
            Shot(new Vector3(c.x, 2.0f, c.z - 6), c + new Vector3(0, 3, 10), "sample_street.png");
            Shot(new Vector3(c.x - 8, 1.7f, c.z), c + new Vector3(10, 4, 0), "sample_street2.png");
        }

        static void Shot(Vector3 from, Vector3 to, string file)
        {
            var cam = Capture.MakeCamera(from, to);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.72f, 0.77f, 0.79f);
            Capture.Shot(cam, Path.Combine(Out, file));
            Object.DestroyImmediate(cam.gameObject);
        }

        static readonly string[] Pieces =
        {
            "Wall_UnevenBrick_Straight", "Wall_UnevenBrick_Window_Wide_Flat", "Wall_UnevenBrick_Window_Thin_Round", "Wall_UnevenBrick_Door_Flat",
            "Wall_Plaster_Straight", "Wall_Plaster_Window_Wide_Flat", "Wall_Plaster_Door_Round", "Wall_Arch",
            "Corner_Exterior_Brick", "Corner_ExteriorWide_Brick", "DoorFrame_Flat_Brick", "Door_1_Flat",
            "Window_Wide_Flat1", "Window_Thin_Round1", "WindowShutters_Wide_Flat_Open", "Balcony_Simple_Straight",
            "Roof_RoundTiles_6x8", "Roof_Modular_RoundTiles_6_Mid", "Roof_Front_Brick6", "Roof_Tower_RoundTiles",
            "Overhang_UnevenBrick_Long", "Stairs_Exterior_Straight", "Prop_Chimney", "Floor_RoundRocks",
        };

        static void RenderPieces()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Lighting();
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.localScale = new Vector3(10, 1, 10);
            var sb = new StringBuilder();
            var all = AssetDatabase.FindAssets("t:Prefab", new[] { Kit + "Modules/Prefabs" })
                .Select(AssetDatabase.GUIDToAssetPath).ToDictionary(Path.GetFileNameWithoutExtension, p => p);
            for (int i = 0; i < Pieces.Length; i++)
            {
                if (!all.TryGetValue(Pieces[i], out var path)) { sb.AppendLine("MISSING " + Pieces[i]); continue; }
                var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                go.transform.position = new Vector3((i % 8) * 10f, 0, (i / 8) * 12f);
                var b = new Bounds(go.transform.position, Vector3.zero);
                foreach (var r in go.GetComponentsInChildren<Renderer>()) b.Encapsulate(r.bounds);
                var mats = string.Join(",", go.GetComponentsInChildren<Renderer>().SelectMany(r => r.sharedMaterials).Where(m => m).Select(m => m.name + ":" + m.shader.name).Distinct());
                var cols = string.Join(",", go.GetComponentsInChildren<Collider>().Select(c => c.GetType().Name));
                sb.AppendLine($"{Pieces[i]}\tmin={b.min - go.transform.position}\tmax={b.max - go.transform.position}\tmats={mats}\tcolliders={cols}");
            }
            File.WriteAllText(Path.Combine(Out, "pieces.txt"), sb.ToString());
            Shot(new Vector3(35, 14, -22), new Vector3(35, 2, 14), "pieces_front.png");
            Shot(new Vector3(35, 14, 50), new Vector3(35, 2, 14), "pieces_back.png");
            Shot(new Vector3(-6, 3, -4), new Vector3(10, 2, 2), "pieces_close.png");
        }

        static void DumpUAL()
        {
            var sb = new StringBuilder();
            foreach (var guid in AssetDatabase.FindAssets("t:Model", new[] { "Assets/ThirdParty/Quaternius/UAL" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                sb.AppendLine("# " + path);
                foreach (var o in AssetDatabase.LoadAllAssetsAtPath(path))
                    if (o is AnimationClip clip && !clip.name.StartsWith("__preview"))
                        sb.AppendLine($"{clip.name}\t{clip.length:F2}s\tloop={clip.isLooping}");
                    else if (o is Avatar av) sb.AppendLine($"avatar {av.name} human={av.isHuman} valid={av.isValid}");
                    else if (o is SkinnedMeshRenderer smr) sb.AppendLine($"smr {smr.name} mats={string.Join(",", smr.sharedMaterials.Select(m => m ? m.name : "null"))}");
            }
            File.WriteAllText(Path.Combine(Out, "ual.txt"), sb.ToString());
        }
    }
}
