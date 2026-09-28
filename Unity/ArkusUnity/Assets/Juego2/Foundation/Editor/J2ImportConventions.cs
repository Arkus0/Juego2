using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// Project-wide import rules frozen by WP-H2F-02 (H2F-01 S07 findings). They are explicit apply + fail-closed
    /// verify operations that content owners (ART, CITY, fixtures, Astra) run on their own paths; the foundation does
    /// not install a postprocessor over ART-owned folders.
    /// </summary>
    public static class J2ImportConventions
    {
        /// <summary>Quaternius universal skeleton: auto-mapping picks the ground 'root' as Hips. Explicit shared mapping.</summary>
        public static readonly IReadOnlyDictionary<string, string> BaseCharacterMapping = new Dictionary<string, string>
        {
            ["Hips"] = "pelvis",
            ["Spine"] = "spine_01",
            ["Chest"] = "spine_02",
            ["UpperChest"] = "spine_03",
        };

        public enum SourceFamily
        {
            /// <summary>Quaternius Medieval/Nature/Props Source FBX: measured centimetre vertices.</summary>
            QuaterniusCentimetreModel,
            /// <summary>Base Characters bodies and their Juego2 derivatives: Humanoid with the explicit mapping.</summary>
            HumanoidBaseCharacter,
            /// <summary>UAL1/UAL2 clip libraries: Humanoid, same mapping, `_Loop` clips loop, root motion baked into clips.</summary>
            UalClipLibrary,
        }

        public static void ApplyModelDefaults(ModelImporter importer, SourceFamily family)
        {
            importer.importCameras = false;
            importer.importLights = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            switch (family)
            {
                case SourceFamily.QuaterniusCentimetreModel:
                    importer.globalScale = 0.01f;
                    importer.useFileScale = false;
                    importer.importAnimation = false;
                    importer.animationType = ModelImporterAnimationType.None;
                    break;
                case SourceFamily.HumanoidBaseCharacter:
                    importer.animationType = ModelImporterAnimationType.Human;
                    importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                    importer.importAnimation = false;
                    break;
                case SourceFamily.UalClipLibrary:
                    importer.animationType = ModelImporterAnimationType.Human;
                    importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                    importer.importAnimation = true;
                    break;
            }
        }

        /// <summary>Two-pass Humanoid setup: import once to get Unity's mapping and skeleton, then pin the explicit bones.</summary>
        public static string ApplyHumanoid(string modelPath, SourceFamily family)
        {
            if (family == SourceFamily.QuaterniusCentimetreModel) throw new ArgumentException("J2_NOT_A_HUMANOID_FAMILY");
            var importer = (ModelImporter)AssetImporter.GetAtPath(modelPath) ?? throw new InvalidOperationException("J2_MODEL_MISSING " + modelPath);
            ApplyModelDefaults(importer, family);
            importer.SaveAndReimport();
            var hd = importer.humanDescription;
            var skeleton = new HashSet<string>(hd.skeleton.Select(s => s.name));
            var map = hd.human.ToDictionary(h => h.humanName, h => h);
            foreach (var pin in BaseCharacterMapping)
            {
                if (pin.Key == "UpperChest" && !skeleton.Contains(pin.Value)) { map.Remove(pin.Key); continue; }
                if (!skeleton.Contains(pin.Value)) throw new InvalidOperationException($"J2_AVATAR_BONE_MISSING {modelPath}:{pin.Value}");
                map[pin.Key] = new HumanBone { humanName = pin.Key, boneName = pin.Value, limit = new HumanLimit { useDefaultValues = true } };
            }
            hd.human = map.Values.ToArray();
            importer.humanDescription = hd;
            if (family == SourceFamily.UalClipLibrary)
            {
                var clips = importer.defaultClipAnimations;
                foreach (var clip in clips) clip.loopTime = clip.name.EndsWith("_Loop", StringComparison.Ordinal);
                importer.clipAnimations = clips;
            }
            importer.SaveAndReimport();
            return string.Join(", ", BaseCharacterMapping.Keys.Select(k => k + "=" + (map.TryGetValue(k, out var b) ? b.boneName : "-")));
        }

        /// <summary>Stable codes for every Humanoid/clip import that no longer follows the frozen rules.</summary>
        public static List<string> Verify(IEnumerable<string> humanoidModels, IEnumerable<string> ualLibraries)
        {
            var findings = new List<string>();
            foreach (var path in humanoidModels.Concat(ualLibraries))
            {
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null) { findings.Add("J2_IMPORT_MODEL_MISSING:" + path); continue; }
                if (importer.animationType != ModelImporterAnimationType.Human) findings.Add("J2_IMPORT_NOT_HUMANOID:" + path);
                var bones = importer.humanDescription.human.ToDictionary(h => h.humanName, h => h.boneName);
                if (!bones.TryGetValue("Hips", out var hips) || hips != BaseCharacterMapping["Hips"]) findings.Add("J2_IMPORT_HIPS_NOT_PELVIS:" + path);
                var avatar = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Avatar>().FirstOrDefault();
                if (avatar == null || !avatar.isValid || !avatar.isHuman) findings.Add("J2_IMPORT_AVATAR_INVALID:" + path);
            }
            foreach (var path in ualLibraries)
                if (AssetImporter.GetAtPath(path) is ModelImporter importer)
                    foreach (var clip in importer.clipAnimations.Length > 0 ? importer.clipAnimations : importer.defaultClipAnimations)
                        if (clip.loopTime != clip.name.EndsWith("_Loop", StringComparison.Ordinal))
                            findings.Add($"J2_IMPORT_LOOP_FLAG:{path}#{clip.name}");
            return findings;
        }
    }
}
