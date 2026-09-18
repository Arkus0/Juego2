using System;
using System.IO;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Finds the repository root from the test output directory.
    /// </summary>
    public static class RepositoryLocator
    {
        /// <summary>Absolute repository root.</summary>
        /// <returns>The directory containing the kernel manifest.</returns>
        public static string Root()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current is not null)
            {
                if (File.Exists(Path.Combine(current.FullName, "kernel-manifest.json")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new InvalidOperationException("Could not locate the repository root from the test output directory.");
        }

        /// <summary>Absolute path of the kernel manifest.</summary>
        /// <returns>The manifest path.</returns>
        public static string ManifestPath()
        {
            return Path.Combine(Root(), "kernel-manifest.json");
        }
    }
}
