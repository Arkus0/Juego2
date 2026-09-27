using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>
    /// S07 rig finding: Unity's automatic Humanoid mapping on the Quaternius universal skeleton assigns Hips to
    /// the ground-level 'root' bone. This applies one explicit, shared mapping (Hips=pelvis, spine chain shifted)
    /// to the body and to every UAL library, then reports the before/after mapping and avatar validity.
    /// </summary>
    public static class H2F01AvatarMap
    {
        static readonly string[] Paths =
        {
            "Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx",
            "Assets/Arkus/ART/External/UAL/UAL1.fbx",
            "Assets/H2F01Inputs/Animation/UAL1_RM.fbx",
            "Assets/H2F01Inputs/Animation/UAL2.fbx",
            "Assets/H2F01Inputs/Animation/UAL2_RM.fbx",
        };

        static readonly string[] Watch = { "Hips", "Spine", "Chest", "UpperChest", "Neck", "Head", "LeftUpperLeg", "LeftFoot", "LeftToes", "LeftHand" };

        public static void Report() => Run(false);
        public static void Apply() => Run(true);

        static void Run(bool apply)
        {
            var lines = new List<string>();
            foreach (var path in Paths)
            {
                var importer = (ModelImporter)AssetImporter.GetAtPath(path);
                var hd = importer.humanDescription;
                var map = hd.human.ToDictionary(h => h.humanName, h => h.boneName);
                lines.Add($"{path}: " + string.Join(", ", Watch.Select(w => w + "=" + (map.TryGetValue(w, out var b) ? b : "-"))));
                if (!apply) continue;
                var skeleton = hd.skeleton.Select(s => s.name).ToHashSet();
                var fixedMap = new Dictionary<string, string>(map)
                {
                    ["Hips"] = "pelvis", ["Spine"] = "spine_01", ["Chest"] = "spine_02",
                };
                if (skeleton.Contains("spine_03")) fixedMap["UpperChest"] = "spine_03"; else fixedMap.Remove("UpperChest");
                var missing = fixedMap.Values.Where(b => !skeleton.Contains(b)).ToArray();
                if (missing.Length > 0) throw new Exception($"H2F01_AVATAR_BONES_MISSING {path}: {string.Join(",", missing)}");
                hd.human = fixedMap.Select(kv =>
                {
                    var old = hd.human.FirstOrDefault(h => h.humanName == kv.Key);
                    return new HumanBone { humanName = kv.Key, boneName = kv.Value, limit = old.limit.useDefaultValues || old.boneName == null ? new HumanLimit { useDefaultValues = true } : old.limit };
                }).ToArray();
                importer.humanDescription = hd;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.SaveAndReimport();
                var avatar = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Avatar>().FirstOrDefault();
                lines.Add($"  -> applied; avatar valid={(avatar && avatar.isValid)} human={(avatar && avatar.isHuman)}");
            }
            System.IO.File.WriteAllLines(Out(apply ? "s07/avatar_map_applied.txt" : "s07/avatar_map_auto.txt"), lines);
            foreach (var l in lines) Log("H2F01_AVATAR " + l);
        }
    }
}
