using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Arkus.H1.Editor
{
    /// <summary>
    /// Effective-Unity fixture for WP-H1-03. The caller supplies only a reviewed logical resource
    /// identity. The project root and managed asset root are bootstrapped from the running Editor and
    /// cannot be selected by protocol payload.
    /// </summary>
    public static class H1HostPolicyProbe
    {
        public const string ProjectIdentity = "arkus.unity-project@1:ArkusUnity";
        public const string ManagedRoot = "Assets/Arkus";
        public const string PotesMarketResource = "asset.potes.market-stall";

        private static readonly IReadOnlyDictionary<string, string> ReviewedResources =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [PotesMarketResource] = "Assets/Arkus/H1/PolicyProbe/potes-market-stall.txt"
            };

        public static string ProjectRootAbsolute()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }

        public static string ResolveReviewedResource(string logicalResource)
        {
            if (logicalResource == null) throw new ArgumentNullException(nameof(logicalResource));
            if (!ReviewedResources.TryGetValue(logicalResource, out var projectRelative))
                throw new InvalidOperationException("Logical resource is not admitted by the H1 workspace fixture.");

            var projectRoot = ProjectRootAbsolute();
            var managedRoot = Path.GetFullPath(Path.Combine(projectRoot, ManagedRoot));
            var candidate = Path.GetFullPath(Path.Combine(projectRoot, projectRelative));
            var prefix = managedRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!candidate.StartsWith(prefix, StringComparison.Ordinal))
                throw new InvalidOperationException("Reviewed logical resource escaped its managed root.");
            return candidate;
        }

        public static void ExternalReversibleWriteProbe(string logicalResource, string payload)
        {
            var path = ResolveReviewedResource(logicalResource);
            var directory = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Probe path has no directory.");
            Directory.CreateDirectory(directory);
            var existed = File.Exists(path);
            var original = existed ? File.ReadAllText(path) : null;
            try
            {
                File.WriteAllText(path, payload ?? string.Empty);
                if (!File.Exists(path) || !string.Equals(File.ReadAllText(path), payload ?? string.Empty, StringComparison.Ordinal))
                    throw new InvalidOperationException("Managed reversible write did not round-trip inside the Editor workspace.");
            }
            finally
            {
                if (existed)
                    File.WriteAllText(path, original ?? string.Empty);
                else if (File.Exists(path))
                    File.Delete(path);

                if (!existed && Directory.Exists(directory) && Directory.GetFileSystemEntries(directory).Length == 0)
                    Directory.Delete(directory);
            }
        }
    }
}
