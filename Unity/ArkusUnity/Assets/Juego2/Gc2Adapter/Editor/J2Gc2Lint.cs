using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using Juego2.Arkus;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Juego2.Gc2Adapter.Editor
{
    /// <summary>
    /// The fail-closed GC2 Core rules handed over by WP-H2F-01A (H2F02_CORE_HANDOFF.md §4), checked over Unity's
    /// serialized graph (the Inspector's surface), so the rules do not depend on private GC2 members:
    /// L2 no Remember / GC2 scene load-unload; L3 no GC2 Variable named as a Juego2 fact; L4 an adapter entry sits on
    /// a Juego2-bound object; L5 no stock mutation of an Arkus-bound object except after an Arkus request that stops
    /// on refusal; L6 GC2 input reads only the Juego2 action asset (NPCs read none); L7 no Juego2 save through GC2.
    /// L1 (no GameCreator reference from Arkus/CITY/story assemblies) is the GC2-free check in J2AssemblyBoundary.
    /// </summary>
    public static class J2Gc2Lint
    {
        public const string InputAsset = "Assets/Juego2/Foundation/Input/J2_Input.inputactions";
        const string AdapterNamespace = "Juego2.Gc2Adapter.";
        const string RequestType = "Juego2.Gc2Adapter.InstructionArkusRequestTransition";
        static readonly string[] SceneInstructions = { "InstructionCommonSceneLoad", "InstructionCommonSceneUnload" };
        static readonly string[] MutatingPrefixes =
        {
            "InstructionTransform", "InstructionGameObject", "InstructionPhysics", "InstructionVariables",
            "InstructionCommonSave", "InstructionCommonLoad", "InstructionCommonDelete", "InstructionCommonReset",
        };
        // GC2 normalizes Variable names (TextUtils.ProcessID: 'j2.door.a:state' -> 'j2-door-a-state'), so a Juego2 key
        // can only appear in normalized form. Variables may not use the Juego2 key namespace at all.
        static readonly Regex Juego2KeyNamespace = new Regex(@"^j2[-_.:]", RegexOptions.IgnoreCase);
        static readonly Regex InstructionElement = new Regex(@"^(?<list>.*\.m_Instructions\.Array)\.data\[(?<index>\d+)\]$");

        public readonly struct Finding
        {
            public readonly string Code, Where;
            public Finding(string code, string where) { Code = code; Where = where; }
            public override string ToString() => Code + " @ " + Where;
        }

        public static List<Finding> CheckScene(Scene scene) =>
            Check(scene.GetRootGameObjects(), AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAsset));

        /// <summary>Every scene and prefab under the given Juego2 content roots (vendor folders excluded).</summary>
        public static List<Finding> CheckProjectContent(params string[] roots)
        {
            var findings = new List<Finding>();
            var input = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAsset);
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", roots))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var root = PrefabUtility.LoadPrefabContents(path);
                try { findings.AddRange(Check(new[] { root }, input).Select(f => new Finding(f.Code, path + ":" + f.Where))); }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
            foreach (var guid in AssetDatabase.FindAssets("t:Scene", roots))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                try { findings.AddRange(Check(scene.GetRootGameObjects(), input).Select(f => new Finding(f.Code, path + ":" + f.Where))); }
                finally { EditorSceneManager.CloseScene(scene, true); }
            }
            foreach (var guid in AssetDatabase.FindAssets("t:TGlobalVariables", roots))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset != null) CheckVariableNames(asset, path, findings);
            }
            findings.AddRange(CheckSaveImplementations());
            return findings;
        }

        public static List<Finding> Check(IEnumerable<GameObject> roots, InputActionAsset juego2Input)
        {
            var findings = new List<Finding>();
            foreach (var root in roots)
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                    CheckObject(t.gameObject, juego2Input, findings);
            return findings;
        }

        static void CheckObject(GameObject go, InputActionAsset juego2Input, List<Finding> findings)
        {
            string where = HierarchyPath(go);
            bool bound = ArkusEntityBinding.KeyOf(go) != null;
            foreach (var component in go.GetComponents<Component>())
            {
                if (component == null) continue;
                if (component is Remember) findings.Add(new Finding("J2_GC2_L2_REMEMBER", where));
                var refs = ManagedReferences(component);
                foreach (var (path, type) in refs)
                {
                    var shortName = type.Substring(type.LastIndexOf('.') + 1);
                    if (SceneInstructions.Contains(shortName)) findings.Add(new Finding("J2_GC2_L2_SCENE_INSTRUCTION", where + "/" + path));
                    if (type.StartsWith(AdapterNamespace, StringComparison.Ordinal) && !bound)
                        findings.Add(new Finding("J2_GC2_L4_ADAPTER_ON_UNBOUND_OBJECT", where + "/" + path));
                    if (shortName.StartsWith("InputValue", StringComparison.Ordinal) || shortName.StartsWith("InputButton", StringComparison.Ordinal))
                        CheckInput(component, path, shortName, juego2Input, where, findings);
                }
                if (bound) CheckMutationOrder(component, refs, where, findings);
                if (component is TLocalVariables) CheckVariableNames(component, where, findings);
                if (component is Character character) CheckCharacterInput(character, where, findings);
            }
        }

        /// <summary>L6: an input value/button reads the Juego2 action asset through InputAction, or reads nothing.</summary>
        static void CheckInput(Component component, string path, string shortName, InputActionAsset juego2Input, string where, List<Finding> findings)
        {
            if (shortName.EndsWith("None", StringComparison.Ordinal)) return;
            if (shortName.Contains("InputAction"))
            {
                var asset = new SerializedObject(component).FindProperty(path + ".m_Input.m_InputAsset");
                if (asset != null && juego2Input != null && asset.objectReferenceValue == juego2Input) return;
                findings.Add(new Finding("J2_GC2_L6_INPUT_ACTION_NOT_JUEGO2", where + "/" + path));
                return;
            }
            findings.Add(new Finding("J2_GC2_L6_DEVICE_INPUT:" + shortName, where + "/" + path));
        }

        /// <summary>L6: an NPC's dormant player unit reads no input at all (01A preset rule).</summary>
        static void CheckCharacterInput(Character character, string where, List<Finding> findings)
        {
            var so = new SerializedObject(character);
            var isPlayer = so.FindProperty("m_IsPlayer");
            var input = so.FindProperty("m_Kernel.m_Player.m_InputMove.m_Input");
            if (isPlayer == null || input == null) { findings.Add(new Finding("J2_GC2_AUTHORING_PATH_MISSING", where + "/m_Kernel")); return; }
            if (!isPlayer.boolValue && !input.managedReferenceFullTypename.EndsWith(nameof(InputValueVector2None), StringComparison.Ordinal))
                findings.Add(new Finding("J2_GC2_L6_NPC_INPUT_ENABLED", where));
        }

        /// <summary>L5: on an Arkus-bound object, a stock mutating instruction must follow an Arkus request that stops on refusal.</summary>
        static void CheckMutationOrder(Component component, List<(string path, string type)> refs, string where, List<Finding> findings)
        {
            var so = new SerializedObject(component);
            foreach (var list in refs.Select(r => (r, m: InstructionElement.Match(r.path))).Where(x => x.m.Success)
                         .GroupBy(x => x.m.Groups["list"].Value))
            {
                bool gated = false;
                foreach (var (entry, match) in list.OrderBy(x => int.Parse(x.m.Groups["index"].Value)).Select(x => (x.r, x.m)))
                {
                    if (entry.type == RequestType)
                    {
                        gated |= so.FindProperty(entry.path + ".m_StopIfRejected")?.boolValue == true;
                        continue;
                    }
                    var shortName = entry.type.Substring(entry.type.LastIndexOf('.') + 1);
                    if (!gated && MutatingPrefixes.Any(p => shortName.StartsWith(p, StringComparison.Ordinal)))
                        findings.Add(new Finding("J2_GC2_L5_UNGATED_MUTATION:" + shortName, where + "/" + entry.path));
                }
            }
        }

        /// <summary>L3: a GC2 Variable is local/presentation state; a name in the Juego2 key namespace would impersonate a fact.</summary>
        static void CheckVariableNames(UnityEngine.Object component, string where, List<Finding> findings)
        {
            var it = new SerializedObject(component).GetIterator();
            while (it.Next(true))
                if (it.propertyType == SerializedPropertyType.String && it.propertyPath.EndsWith("m_Name.m_String", StringComparison.Ordinal)
                    && Juego2KeyNamespace.IsMatch(it.stringValue ?? ""))
                    findings.Add(new Finding("J2_GC2_L3_VARIABLE_NAMED_AS_FACT:" + it.stringValue, where));
        }

        /// <summary>L7: until H6 adopts a save host, no Juego2 assembly persists through GC2 (IGameSave / data storage).</summary>
        public static List<Finding> CheckSaveImplementations() =>
            CheckSaveImplementations(name => name.StartsWith("Juego2.", StringComparison.Ordinal) && !name.Contains(".Tests"));

        public static List<Finding> CheckSaveImplementations(Func<string, bool> assemblyFilter)
        {
            var findings = new List<Finding>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => assemblyFilter(a.GetName().Name)))
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch (System.Reflection.ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }
                foreach (var type in types)
                    if (!type.IsAbstract && (typeof(IGameSave).IsAssignableFrom(type) || typeof(GameCreator.Runtime.Common.SaveSystem.TDataStorage).IsAssignableFrom(type)))
                        findings.Add(new Finding("J2_GC2_L7_JUEGO2_SAVE_THROUGH_GC2", type.FullName));
            }
            return findings;
        }

        static List<(string path, string type)> ManagedReferences(Component component)
        {
            var result = new List<(string, string)>();
            var it = new SerializedObject(component).GetIterator();
            while (it.Next(true))
                if (it.propertyType == SerializedPropertyType.ManagedReference && !string.IsNullOrEmpty(it.managedReferenceFullTypename))
                {
                    var full = it.managedReferenceFullTypename; // "<assembly> <namespace.Type>"
                    result.Add((it.propertyPath, full.Substring(full.IndexOf(' ') + 1)));
                }
            return result;
        }

        static string HierarchyPath(GameObject go)
        {
            var names = new List<string>();
            for (var t = go.transform; t != null; t = t.parent) names.Add(t.name);
            names.Reverse();
            return string.Join("/", names);
        }
    }
}
