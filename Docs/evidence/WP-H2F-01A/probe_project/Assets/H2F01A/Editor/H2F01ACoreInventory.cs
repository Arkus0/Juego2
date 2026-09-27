using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Juego2.H2F01A.Editor
{
    /// <summary>
    /// C01 — exact installed GC2 Core surface and project-state inventory. Reflection and file hashing are used for
    /// discovery only; no probe or adapter depends on anything this inventory finds through non-public members.
    /// </summary>
    public static class H2F01ACoreInventory
    {
        const string Gc2Root = "Assets/Plugins/GameCreator";

        // Extension families: base type full name -> family label. A base counts as a supported extension point only
        // if it is public, and every abstract member can be overridden from another assembly.
        static readonly (string type, string family)[] Bases =
        {
            ("GameCreator.Runtime.VisualScripting.Instruction", "visual-scripting.instruction"),
            ("GameCreator.Runtime.VisualScripting.Condition", "visual-scripting.condition"),
            ("GameCreator.Runtime.VisualScripting.Event", "visual-scripting.event"),
            ("GameCreator.Runtime.VisualScripting.Spot", "visual-scripting.hotspot-spot"),
            ("GameCreator.Runtime.Common.TPropertyTypeGet`1", "property.get"),
            ("GameCreator.Runtime.Common.TPropertyTypeSet`1", "property.set"),
            ("GameCreator.Runtime.Common.SaveSystem.TDataStorage", "save.storage"),
            ("GameCreator.Runtime.Common.SaveSystem.TDataEncryption", "save.encryption"),
            ("GameCreator.Runtime.Common.IGameSave", "save.game-save-host"),
            ("GameCreator.Runtime.Common.Memory", "save.remember-memory"),
            ("GameCreator.Runtime.Characters.TUnitPlayer", "character.unit.player-input"),
            ("GameCreator.Runtime.Characters.TUnitMotion", "character.unit.motion"),
            ("GameCreator.Runtime.Characters.TUnitDriver", "character.unit.driver"),
            ("GameCreator.Runtime.Characters.TUnitFacing", "character.unit.facing"),
            ("GameCreator.Runtime.Characters.TUnitAnimim", "character.unit.animim"),
            ("GameCreator.Runtime.Characters.IK.TRig", "character.ik-rig"),
            ("GameCreator.Runtime.Characters.TRagdollSystem", "character.ragdoll"),
            ("GameCreator.Runtime.Characters.FootstepDetectorBase", "character.footstep-detector"),
            ("GameCreator.Runtime.Characters.TInteractionMode", "character.interaction-mode"),
            ("GameCreator.Runtime.Characters.IInteractive", "character.interactive"),
            ("GameCreator.Runtime.Characters.State", "character.animation-state-asset"),
            ("GameCreator.Runtime.Cameras.TShotType", "camera.shot-type"),
            ("GameCreator.Runtime.Cameras.TCamera", "camera.component"),
            ("GameCreator.Runtime.Common.TInputButton", "input.button"),
            ("GameCreator.Runtime.Common.TInputValue`1", "input.value"),
            ("GameCreator.Runtime.Common.TMarkerType", "navigation.marker-type"),
            ("GameCreator.Runtime.Variables.TVariable", "variables.value-type"),
            ("GameCreator.Runtime.Common.Singleton`1", "runtime.manager-singleton"),
        };

        [Serializable] class Row { public string family, baseType, type, category, assembly; public bool isPublic; }
        [Serializable] class Family { public string family, baseType; public bool found, isPublic, overridableFromOutside; public int concreteTypes, publicConcreteTypes; public string[] nonOverridableAbstracts; }
        [Serializable] class Asm { public string name; public int types, publicTypes; public string[] references; }
        [Serializable] class AsmDef { public string path, name; public string[] references, defineConstraints, versionDefines; public bool autoReferenced; }
        [Serializable] class FileHash { public string path, sha256; public long bytes; }
        [Serializable] class Hook { public string kind, member; }
        [Serializable]
        class Report
        {
            public string mode, version, unityVersion, scriptingDefines, activeInputHandler;
            public int runtimeSourceFiles, editorSourceFiles, testSourceFiles, exampleFiles;
            public List<Asm> assemblies = new List<Asm>();
            public List<AsmDef> asmdefs = new List<AsmDef>();
            public List<Family> families = new List<Family>();
            public List<FileHash> generatedSettings = new List<FileHash>();
            public List<Hook> autoHooks = new List<Hook>();
            public List<string> namespaces = new List<string>();
            public List<string> publicMonoBehaviours = new List<string>();
            public List<string> scriptableObjectTypes = new List<string>();
            public List<string> projectStateChanged = new List<string>();
            public List<string> projectStateAdded = new List<string>();
            public List<string> layers = new List<string>();
            public List<string> tags = new List<string>();
            public int menuItems;
        }

        /// <summary>Batch mode: records the state GC2 has produced without an interactive editor session.</summary>
        public static void Run()
        {
            try { Inventory("batch"); }
            catch (Exception e) { Debug.LogError("H2F01A_C01_FAILED " + e); EditorApplication.Exit(1); }
        }

        /// <summary>
        /// Windowed editor: GC2 creates its settings assets from deferred editor callbacks, which never run inside a
        /// single batch -executeMethod. Wait for the editor to settle (bounded), then inventory and exit.
        /// </summary>
        public static void RunWindowed()
        {
            var start = EditorApplication.timeSinceStartup;
            int ticks = 0;
            EditorApplication.CallbackFunction wait = null;
            wait = () =>
            {
                ticks++;
                bool settled = Directory.Exists($"{Gc2Root}/Data") && ticks > 600;
                if (!settled && EditorApplication.timeSinceStartup - start < 120) return;
                EditorApplication.update -= wait;
                try { Inventory("windowed"); EditorApplication.Exit(0); }
                catch (Exception e) { Debug.LogError("H2F01A_C01_FAILED " + e); EditorApplication.Exit(1); }
            };
            EditorApplication.update += wait;
        }

        static void Inventory(string mode)
        {
            var r = new Report
            {
                mode = mode,
                version = File.ReadAllText($"{Gc2Root}/Packages/Core/Editor/Version.txt").Trim(),
                unityVersion = Application.unityVersion,
                scriptingDefines = PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone),
                activeInputHandler = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]).FindProperty("activeInputHandler")?.intValue.ToString(),
            };
            int Count(string dir, string ext) => Directory.Exists(dir) ? Directory.GetFiles(dir, "*" + ext, SearchOption.AllDirectories).Length : 0;
            r.runtimeSourceFiles = Count($"{Gc2Root}/Packages/Core/Runtime", ".cs");
            r.editorSourceFiles = Count($"{Gc2Root}/Packages/Core/Editor", ".cs");
            r.testSourceFiles = Count($"{Gc2Root}/Packages/Core/Tests", ".cs");
            r.exampleFiles = Directory.Exists($"{Gc2Root}/Packages/Core/Examples")
                ? Directory.GetFiles($"{Gc2Root}/Packages/Core/Examples", "*", SearchOption.AllDirectories).Count(p => !p.EndsWith(".meta")) : 0;

            var gc = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("GameCreator")).OrderBy(a => a.GetName().Name).ToList();
            var types = new List<Type>();
            foreach (var a in gc)
            {
                Type[] all;
                try { all = a.GetTypes(); } catch (ReflectionTypeLoadException e) { all = e.Types.Where(t => t != null).ToArray(); }
                types.AddRange(all);
                r.assemblies.Add(new Asm
                {
                    name = a.GetName().Name, types = all.Length, publicTypes = all.Count(t => t.IsPublic || t.IsNestedPublic),
                    references = a.GetReferencedAssemblies().Select(n => n.Name).OrderBy(n => n).ToArray(),
                });
            }
            foreach (var path in Directory.GetFiles(Gc2Root, "*.asmdef", SearchOption.AllDirectories).OrderBy(p => p))
            {
                var json = JsonUtility.FromJson<AsmDefJson>(File.ReadAllText(path));
                r.asmdefs.Add(new AsmDef
                {
                    path = path.Replace('\\', '/'), name = json.name, references = json.references ?? new string[0],
                    defineConstraints = json.defineConstraints ?? new string[0], autoReferenced = json.autoReferenced,
                    versionDefines = (json.versionDefines ?? new VersionDefine[0]).Select(v => $"{v.name}:{v.expression}->{v.define}").ToArray(),
                });
            }
            r.namespaces = types.Where(t => t.IsPublic).GroupBy(t => t.Namespace ?? "<global>")
                .OrderBy(g => g.Key).Select(g => $"{g.Key} {g.Count()}").ToList();
            r.publicMonoBehaviours = types.Where(t => t.IsPublic && !t.IsAbstract && typeof(MonoBehaviour).IsAssignableFrom(t))
                .Select(t => t.FullName).OrderBy(n => n).ToList();
            r.scriptableObjectTypes = types.Where(t => t.IsPublic && !t.IsAbstract && typeof(ScriptableObject).IsAssignableFrom(t)
                && !typeof(EditorWindow).IsAssignableFrom(t) && !typeof(UnityEditor.Editor).IsAssignableFrom(t))
                .Select(t => t.FullName).OrderBy(n => n).ToList();

            var rows = new List<Row>();
            foreach (var (baseName, family) in Bases)
            {
                var b = types.FirstOrDefault(t => t.FullName == baseName);
                var f = new Family { family = family, baseType = baseName, found = b != null };
                if (b != null)
                {
                    f.isPublic = b.IsPublic;
                    // An abstract member declared `internal abstract` (IsAssembly, not FamilyOrAssembly) cannot be implemented outside GC2.
                    var sealedAbstracts = b.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                        .OfType<MethodBase>().Where(m => m.IsAbstract && m.IsAssembly).Select(m => m.Name).ToArray();
                    f.nonOverridableAbstracts = sealedAbstracts;
                    f.overridableFromOutside = b.IsPublic && sealedAbstracts.Length == 0;
                    foreach (var t in types.Where(t => !t.IsAbstract && !t.IsInterface && Derives(t, b)))
                    {
                        f.concreteTypes++;
                        if (t.IsPublic || t.IsNestedPublic) f.publicConcreteTypes++;
                        rows.Add(new Row
                        {
                            family = family, baseType = baseName, type = t.FullName, assembly = t.Assembly.GetName().Name,
                            isPublic = t.IsPublic || t.IsNestedPublic, category = CategoryOf(t),
                        });
                    }
                }
                r.families.Add(f);
            }

            foreach (var t in types)
            {
                const BindingFlags all = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
                if (t.GetCustomAttributes(typeof(InitializeOnLoadAttribute), false).Length > 0)
                    r.autoHooks.Add(new Hook { kind = "InitializeOnLoad", member = t.FullName });
                foreach (var m in t.GetMethods(all))
                {
                    if (m.GetCustomAttributes(typeof(InitializeOnLoadMethodAttribute), false).Length > 0)
                        r.autoHooks.Add(new Hook { kind = "InitializeOnLoadMethod", member = t.FullName + "." + m.Name });
                    if (m.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false).Length > 0)
                        r.autoHooks.Add(new Hook { kind = "RuntimeInitializeOnLoadMethod", member = t.FullName + "." + m.Name });
                    if (m.GetCustomAttributes(typeof(MenuItem), false).Length > 0) r.menuItems++;
                }
            }
            r.autoHooks = r.autoHooks.OrderBy(h => h.kind).ThenBy(h => h.member).ToList();

            foreach (var path in (Directory.Exists($"{Gc2Root}/Data") ? Directory.GetFiles($"{Gc2Root}/Data", "*", SearchOption.AllDirectories) : new string[0]).Where(p => !p.EndsWith(".meta")).OrderBy(p => p))
            {
                var bytes = File.ReadAllBytes(path);
                r.generatedSettings.Add(new FileHash { path = path.Replace('\\', '/'), sha256 = Sha(bytes), bytes = bytes.Length });
            }

            // project-global state compared with the snapshot bootstrap_01a.py took before the GC2 import
            var projectDir = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var after = new SortedDictionary<string, string>();
            foreach (var p in Directory.GetFiles(Path.Combine(projectDir, "ProjectSettings")).Append(Path.Combine(projectDir, "Packages/manifest.json")))
                after[Path.GetRelativePath(projectDir, p).Replace('\\', '/')] = Sha(File.ReadAllBytes(p));
            var beforePath = Out("c01/project_state_before_gc2.json");
            if (File.Exists(beforePath))
            {
                var before = ParseFlatJson(File.ReadAllText(beforePath));
                foreach (var kv in after)
                {
                    if (!before.TryGetValue(kv.Key, out var old)) r.projectStateAdded.Add(kv.Key);
                    else if (old != kv.Value) r.projectStateChanged.Add(kv.Key);
                }
            }
            else r.projectStateChanged.Add("<no pre-import snapshot>");
            File.WriteAllText(Out($"c01/project_state_after_gc2_{mode}.json"), "{\n" + string.Join(",\n", after.Select(kv => $"  \"{kv.Key}\": \"{kv.Value}\"")) + "\n}\n");
            r.layers = Enumerable.Range(0, 32).Select(i => $"{i}:{LayerMask.LayerToName(i)}").Where(s => !s.EndsWith(":")).ToList();
            r.tags = UnityEditorInternal.InternalEditorUtility.tags.ToList();

            File.WriteAllText(Out($"c01/core_surface_{mode}.json"), JsonUtility.ToJson(r, true) + "\n", new UTF8Encoding(false));
            var tsv = new StringBuilder("family\tbase\ttype\tpublic\tcategory\tassembly\n");
            foreach (var row in rows.OrderBy(x => x.family).ThenBy(x => x.type))
                tsv.Append($"{row.family}\t{row.baseType}\t{row.type}\t{row.isPublic}\t{row.category}\t{row.assembly}\n");
            File.WriteAllText(Out("c01/core_extension_types.tsv"), tsv.ToString(), new UTF8Encoding(false));
            foreach (var f in r.families)
                Debug.Log($"H2F01A_C01_FAMILY {f.family} found={f.found} public={f.isPublic} extensible={f.overridableFromOutside} concrete={f.concreteTypes}");
            Debug.Log($"H2F01A_C01_DONE mode={mode} version={r.version} assemblies={r.assemblies.Count} families={r.families.Count(f => f.found)}/{r.families.Count} " +
                      $"settings={r.generatedSettings.Count} hooks={r.autoHooks.Count} changed={string.Join(",", r.projectStateChanged)} added={string.Join(",", r.projectStateAdded)}");
        }

        [Serializable] class AsmDefJson { public string name; public string[] references, defineConstraints; public bool autoReferenced = true; public VersionDefine[] versionDefines; }
        [Serializable] class VersionDefine { public string name, expression, define; }

        static bool Derives(Type t, Type b)
        {
            if (b.IsInterface) return b.IsAssignableFrom(t);
            for (var c = t.BaseType; c != null; c = c.BaseType)
                if (c == b || (b.IsGenericTypeDefinition && c.IsGenericType && c.GetGenericTypeDefinition() == b)) return true;
            if (b.IsGenericTypeDefinition && b.IsInterface) return t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == b);
            return false;
        }

        static string CategoryOf(Type t)
        {
            var a = t.GetCustomAttributes(false).FirstOrDefault(x => x.GetType().Name == "CategoryAttribute");
            return a == null ? "" : a.ToString();
        }

        static Dictionary<string, string> ParseFlatJson(string text)
        {
            var d = new Dictionary<string, string>();
            foreach (var line in text.Split('\n'))
            {
                var parts = line.Trim().TrimEnd(',').Split(new[] { "\": \"" }, StringSplitOptions.None);
                if (parts.Length == 2) d[parts[0].Trim('"')] = parts[1].Trim('"');
            }
            return d;
        }

        internal static string Sha(byte[] bytes)
        {
            using (var sha = SHA256.Create()) return string.Concat(sha.ComputeHash(bytes).Select(b => b.ToString("x2")));
        }

        internal static string Out(string relative) => Juego2.H2F01.Editor.H2F01Common.Out(relative);
    }
}
