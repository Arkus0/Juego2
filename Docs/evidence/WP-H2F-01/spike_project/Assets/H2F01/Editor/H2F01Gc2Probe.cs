using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>
    /// Owner-supplied candidate probe (Game Creator 2 Core, isolated workspace only): compiles?, which assemblies
    /// and runtime systems arrive, what project-level state it creates, and which of its types would carry world
    /// state (variables/save) versus presentation (character locomotion/camera). Reflection only: no GC2 API
    /// dependency in this recipe, so the recipe still compiles where GC2 is absent.
    /// </summary>
    public static class H2F01Gc2Probe
    {
        public static void Run()
        {
            var lines = new System.Collections.Generic.List<string>();
            var gcAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("GameCreator")).OrderBy(a => a.GetName().Name).ToList();
            lines.Add("assemblies=" + string.Join(",", gcAssemblies.Select(a => a.GetName().Name)));
            string[] probes =
            {
                "GameCreator.Runtime.Characters.Character", "GameCreator.Runtime.Characters.UnitPlayerDirectional",
                "GameCreator.Runtime.Characters.UnitDriverController", "GameCreator.Runtime.Characters.UnitDriverNavmesh",
                "GameCreator.Runtime.Cameras.MainCamera", "GameCreator.Runtime.Cameras.ShotCamera",
                "GameCreator.Runtime.Cameras.ShotTypeThirdPerson", "GameCreator.Runtime.Variables.GlobalNameVariables",
                "GameCreator.Runtime.Variables.LocalNameVariables", "GameCreator.Runtime.Common.SaveLoadManager",
                "GameCreator.Runtime.VisualScripting.Trigger", "GameCreator.Runtime.VisualScripting.Actions",
            };
            foreach (var name in probes)
            {
                var type = gcAssemblies.Select(a => a.GetType(name)).FirstOrDefault(t => t != null);
                lines.Add($"type {name}: {(type != null ? "present" : "absent")}");
            }
            // every public MonoBehaviour GC2 adds, grouped by namespace: what an author could drop into a scene
            var behaviours = gcAssemblies.SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
                .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic).ToList();
            foreach (var g in behaviours.GroupBy(t => t.Namespace).OrderBy(g => g.Key))
                lines.Add($"components {g.Key}: {g.Count()} ({string.Join(", ", g.Select(t => t.Name).OrderBy(n => n).Take(14))}{(g.Count() > 14 ? ", ..." : "")})");
            var settings = Directory.GetFiles(Path.Combine(Application.dataPath, "Plugins/GameCreator"), "*.asset", SearchOption.AllDirectories)
                .Where(p => !p.Contains("Packages")).Select(p => p.Replace(Application.dataPath, "Assets")).ToArray();
            lines.Add("generated_settings_assets=" + string.Join(",", settings));
            lines.Add("scripting_defines=" + PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone));
            lines.Add("activeInputHandler=" + new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]).FindProperty("activeInputHandler")?.intValue);
            File.WriteAllLines(Out("gc2/probe.txt"), lines);
            foreach (var l in lines) Log("H2F01_GC2 " + l);
        }
    }
}
