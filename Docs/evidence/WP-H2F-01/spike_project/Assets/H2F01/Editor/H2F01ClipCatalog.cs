using System.Linq;
using UnityEditor;
using UnityEngine;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>Lists every clip (name, length, loop, root motion, average speed) in UAL1/UAL1_RM/UAL2/UAL2_RM.</summary>
    public static class H2F01ClipCatalog
    {
        public static void Run()
        {
            var sources = new[] {
                "Assets/Arkus/ART/External/UAL/UAL1.fbx",
                "Assets/H2F01Inputs/Animation/UAL1_RM.fbx",
                "Assets/H2F01Inputs/Animation/UAL2.fbx",
                "Assets/H2F01Inputs/Animation/UAL2_RM.fbx" };
            var lines = new System.Collections.Generic.List<string> { "source\tclip\tlength\thumanMotion\thasRootCurves\tavgSpeed\tavgVelocity" };
            foreach (var src in sources)
            {
                var importer = AssetImporter.GetAtPath(src) as ModelImporter;
                var avatar = AssetDatabase.LoadAllAssetsAtPath(src).OfType<Avatar>().FirstOrDefault();
                Log($"H2F01_CATALOG_SOURCE {src} animationType={importer?.animationType} avatar={(avatar ? avatar.name : "none")} human={(avatar && avatar.isHuman)} valid={(avatar && avatar.isValid)}");
                foreach (var clip in AssetDatabase.LoadAllAssetsAtPath(src).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview")))
                    lines.Add($"{System.IO.Path.GetFileNameWithoutExtension(src)}\t{clip.name}\t{clip.length:F2}\t{clip.humanMotion}\t{clip.hasRootCurves}\t{clip.averageSpeed.magnitude:F3}\t{clip.averageSpeed}");
            }
            System.IO.File.WriteAllLines(Out("s07/clip_catalog.tsv"), lines);
            Log("H2F01_CATALOG_DONE clips=" + (lines.Count - 1));
        }
    }
}
