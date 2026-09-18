using System;
using System.Collections.Generic;
using System.IO;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// Path helpers that keep every emitted path repository-relative and
    /// platform-stable, so generated inventories are byte-identical across
    /// machines.
    /// </summary>
    public static class RepositoryPaths
    {
        /// <summary>Normalizes an absolute path to a repository-relative, forward-slash path.</summary>
        /// <param name="repositoryRoot">Absolute repository root.</param>
        /// <param name="absolutePath">Absolute path inside or outside the repository.</param>
        /// <returns>Repository-relative path, or the absolute path when outside the repository.</returns>
        public static string ToRelative(string repositoryRoot, string absolutePath)
        {
            if (repositoryRoot is null)
            {
                throw new ArgumentNullException(nameof(repositoryRoot));
            }

            if (absolutePath is null)
            {
                throw new ArgumentNullException(nameof(absolutePath));
            }

            var root = Path.GetFullPath(repositoryRoot);
            var full = Path.GetFullPath(absolutePath);

            if (!IsInside(root, full))
            {
                return full.Replace('\\', '/');
            }

            return Path.GetRelativePath(root, full).Replace('\\', '/');
        }

        /// <summary>Whether a path is inside a directory.</summary>
        /// <param name="directory">Absolute directory.</param>
        /// <param name="path">Absolute path.</param>
        /// <returns><c>true</c> when contained.</returns>
        public static bool IsInside(string directory, string path)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var normalizedDirectory = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar)
                                      + Path.DirectorySeparatorChar;
            var normalizedPath = Path.GetFullPath(path);
            return normalizedPath.StartsWith(normalizedDirectory, StringComparison.Ordinal);
        }

        /// <summary>Enumerates files under a root, skipping excluded directory names.</summary>
        /// <param name="root">Absolute root directory.</param>
        /// <param name="searchPattern">File search pattern.</param>
        /// <param name="excludedDirectoryNames">Directory names to skip at any depth.</param>
        /// <returns>Absolute file paths, ordinal-sorted.</returns>
        public static IReadOnlyList<string> EnumerateFiles(
            string root,
            string searchPattern,
            IReadOnlyCollection<string> excludedDirectoryNames)
        {
            if (excludedDirectoryNames is null)
            {
                throw new ArgumentNullException(nameof(excludedDirectoryNames));
            }

            var results = new List<string>();
            if (!Directory.Exists(root))
            {
                return results;
            }

            var pending = new Stack<string>();
            pending.Push(Path.GetFullPath(root));

            while (pending.Count > 0)
            {
                var current = pending.Pop();

                foreach (var file in Directory.EnumerateFiles(current, searchPattern))
                {
                    results.Add(Path.GetFullPath(file));
                }

                foreach (var directory in Directory.EnumerateDirectories(current))
                {
                    var name = Path.GetFileName(directory);
                    var excluded = false;
                    foreach (var candidate in excludedDirectoryNames)
                    {
                        if (string.Equals(name, candidate, StringComparison.Ordinal))
                        {
                            excluded = true;
                            break;
                        }
                    }

                    if (!excluded)
                    {
                        pending.Push(directory);
                    }
                }
            }

            results.Sort(StringComparer.Ordinal);
            return results;
        }
    }
}
