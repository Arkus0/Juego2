using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Juego2.Foundation.Editor
{
    /// <summary>Exact package-set check over the committed manifest and lock (no floating versions, no unreviewed package).</summary>
    public static class J2PackageBaseline
    {
        static readonly Regex ManifestRow = new Regex("\"(?<name>[^\"]+)\"\\s*:\\s*\"(?<version>[^\"]+)\"");
        static readonly Regex LockRow = new Regex("\"(?<name>com\\.[^\"]+)\"\\s*:\\s*\\{\\s*\"version\"\\s*:\\s*\"(?<version>[^\"]+)\"\\s*,\\s*\"depth\"\\s*:\\s*(?<depth>\\d+)");

        public static string ProjectRoot => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        public static Dictionary<string, string> ReadManifest(string text)
        {
            var body = text.Substring(text.IndexOf("\"dependencies\"", System.StringComparison.Ordinal));
            body = body.Substring(0, body.IndexOf('}') + 1);
            return ManifestRow.Matches(body).Cast<Match>().Where(m => m.Groups["name"].Value != "dependencies")
                .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["version"].Value);
        }

        public static Dictionary<string, (string version, int depth)> ReadLock(string text) =>
            LockRow.Matches(text).Cast<Match>().ToDictionary(m => m.Groups["name"].Value,
                m => (m.Groups["version"].Value, int.Parse(m.Groups["depth"].Value)));

        public static List<string> Verify() => Verify(
            File.ReadAllText(Path.Combine(ProjectRoot, "Packages/manifest.json")),
            File.ReadAllText(Path.Combine(ProjectRoot, "Packages/packages-lock.json")));

        public static List<string> Verify(string manifestText, string lockText)
        {
            var findings = new List<string>();
            var manifest = ReadManifest(manifestText);
            var locked = ReadLock(lockText);
            foreach (var row in manifest)
            {
                if (!J2FoundationBaseline.DirectPackages.TryGetValue(row.Key, out var expected)) findings.Add("J2_PACKAGE_UNREVIEWED_DIRECT:" + row.Key);
                else if (row.Value != expected) findings.Add($"J2_PACKAGE_VERSION:{row.Key}={row.Value}");
                if (!Regex.IsMatch(row.Value, @"^\d+\.\d+\.\d+$")) findings.Add($"J2_PACKAGE_NOT_EXACT:{row.Key}={row.Value}");
            }
            foreach (var expected in J2FoundationBaseline.DirectPackages)
            {
                if (!manifest.ContainsKey(expected.Key)) findings.Add("J2_PACKAGE_MISSING_DIRECT:" + expected.Key);
                if (!locked.TryGetValue(expected.Key, out var l)) findings.Add("J2_LOCK_MISSING:" + expected.Key);
                else if (l.version != expected.Value || l.depth != 0) findings.Add($"J2_LOCK_MISMATCH:{expected.Key}={l.version}/{l.depth}");
            }
            foreach (var forbidden in J2FoundationBaseline.ForbiddenPackages)
                if (locked.ContainsKey(forbidden)) findings.Add("J2_PACKAGE_NOT_ADMITTED:" + forbidden);
            return findings;
        }
    }
}
