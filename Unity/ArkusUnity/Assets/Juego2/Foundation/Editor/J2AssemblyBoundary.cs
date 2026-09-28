using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Juego2.Foundation.Editor
{
    /// <summary>
    /// GC2 lint L1 (H2F02_CORE_HANDOFF.md §4), checked without GC2 so it also runs where Core is absent:
    /// only the Juego2 GC2 adapter assemblies may reference <c>GameCreator.*</c>, those assemblies compile only when
    /// Core is provisioned, and no Arkus/CITY/story or Juego2 foundation source names GameCreator.
    /// </summary>
    public static class J2AssemblyBoundary
    {
        public static readonly string[] Gc2AdapterRoots = { "Assets/Juego2/Gc2Adapter" };
        public const string Gc2Define = "JUEGO2_GC2_CORE";
        static readonly Regex GameCreatorToken = new Regex(@"\bGameCreator\b");
        static readonly Regex StringsAndComments = new Regex(@"@""(?:""""|[^""])*""|""(?:\\.|[^""\\])*""|//[^\n]*|/\*.*?\*/", RegexOptions.Singleline);

        /// <summary>True when code (not a string literal or comment) names the GameCreator namespace.</summary>
        public static bool CodeNamesGameCreator(string source) => GameCreatorToken.IsMatch(StringsAndComments.Replace(source, " "));

        public static List<string> Verify() => Verify(J2PackageBaseline.ProjectRoot);

        public static List<string> Verify(string projectRoot)
        {
            var findings = new List<string>();
            var assets = Path.Combine(projectRoot, "Assets");
            foreach (var asmdef in Directory.EnumerateFiles(assets, "*.asmdef", SearchOption.AllDirectories))
            {
                var rel = Rel(projectRoot, asmdef);
                if (rel.StartsWith("Assets/Plugins/GameCreator/", StringComparison.Ordinal)) continue;
                var text = File.ReadAllText(asmdef);
                bool adapter = Gc2AdapterRoots.Any(r => rel.StartsWith(r + "/", StringComparison.Ordinal));
                // Project asmdefs reference by name, so the rule cannot be dodged through an opaque GUID reference.
                if (text.Contains("\"GUID:")) findings.Add("J2_ASMDEF_GUID_REFERENCE:" + rel);
                bool referencesGc2 = Regex.IsMatch(text, "\"GameCreator\\.[^\"]+\"");
                if (referencesGc2 && !adapter) findings.Add("J2_GC2_L1_REFERENCE_OUTSIDE_ADAPTER:" + rel);
                if (adapter && !text.Contains("\"" + Gc2Define + "\"")) findings.Add("J2_GC2_ADAPTER_NOT_GATED:" + rel);
            }
            foreach (var root in new[] { "Assets/Arkus", "Assets/Juego2/Arkus", "Assets/Juego2/Foundation" })
            {
                var dir = Path.Combine(projectRoot, root);
                if (!Directory.Exists(dir)) continue;
                foreach (var cs in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
                    if (CodeNamesGameCreator(File.ReadAllText(cs))) findings.Add("J2_GC2_L1_SOURCE_NAMES_GAMECREATOR:" + Rel(projectRoot, cs));
            }
            return findings;
        }

        static string Rel(string root, string path) => path.Substring(root.Length).TrimStart('\\', '/').Replace('\\', '/');
    }
}
