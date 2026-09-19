using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arkus.HK00.Proof
{
    internal static class RepositoryClosureChecks
    {
        public static int RunRepository(string root)
        {
            var findings = 0;
            ProcessResult result;
            try
            {
                result = ProcessExec.Run("dotnet", new[] { "sln", FixedContract.CanonicalSolution, "list" }, root);
            }
            catch (Exception ex)
            {
                return Report("HK00-SOLUTION-ORACLE", "repository", FixedContract.CanonicalSolution, ex.Message);
            }

            var actual = new SortedSet<string>(StringComparer.Ordinal);
            var inProjectList = false;
            foreach (var rawLine in result.Stdout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var line = rawLine.Trim();
                if (!inProjectList)
                {
                    if (line.Length >= 3 && line.All(ch => ch == '-'))
                    {
                        inProjectList = true;
                    }
                    continue;
                }

                if (line.Length == 0)
                {
                    continue;
                }

                var relative = Path.IsPathRooted(line)
                    ? ProcessExec.Relative(root, line)
                    : line.Replace('\\', '/');
                actual.Add(relative);
            }

            if (actual.Count == 0)
            {
                return Report(
                    "HK00-SOLUTION-ORACLE",
                    "repository",
                    FixedContract.CanonicalSolution,
                    "dotnet sln produced no inspectable project membership.");
            }

            var expected = new SortedSet<string>(FixedContract.Projects.Select(p => p.Path), StringComparer.Ordinal);
            foreach (var project in actual)
            {
                if (!expected.Contains(project))
                {
                    findings += Report(
                        "HK00-SOLUTION-UNEXPECTED-PROJECT",
                        "repository",
                        project,
                        "Solution contains a build participant outside the fixed HK00 project contract.");
                }
            }

            foreach (var project in expected)
            {
                if (!actual.Contains(project))
                {
                    findings += Report(
                        "HK00-SOLUTION-MISSING-PROJECT",
                        "repository",
                        project,
                        "Fixed HK00 project is absent from the canonical solution.");
                }
            }

            return findings;
        }

        public static int RunStatic(string root, string configuration)
        {
            var findings = 0;
            var tracked = ReadTrackedFiles(root);
            ExternalAuthority authority;
            try
            {
                authority = ExternalAuthority.Create(root);
            }
            catch (Exception ex)
            {
                return Report("HK00-EXTERNAL-AUTHORITY", "static", "external-authority", ex.Message);
            }

            var observationDirectory = Path.Combine(root, "artifacts", "proof", "import-closure");
            Directory.CreateDirectory(observationDirectory);

            foreach (var spec in FixedContract.Projects)
            {
                var preprocessedPath = Path.Combine(observationDirectory, spec.Name + ".xml");
                var args = new List<string>
                {
                    "msbuild",
                    Path.Combine(root, spec.Path),
                    "-nologo",
                    "-noAutoResponse",
                    "-p:Configuration=" + configuration,
                    "-preprocess:" + preprocessedPath,
                };

                try
                {
                    ProcessExec.Run("dotnet", args, root);
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-MSBUILD-IMPORT-ORACLE", "static", spec.Name, ex.Message);
                    continue;
                }

                IReadOnlyList<string> imports;
                try
                {
                    imports = ReadPreprocessedImportBoundaries(preprocessedPath);
                }
                catch (Exception ex)
                {
                    findings += Report(
                        "HK00-MSBUILD-IMPORT-ORACLE",
                        "static",
                        spec.Name,
                        "Preprocessed import closure is unreadable: " + ex.Message);
                    continue;
                }

                if (imports.Count == 0)
                {
                    findings += Report(
                        "HK00-MSBUILD-IMPORT-ORACLE",
                        "static",
                        spec.Name,
                        "MSBuild -preprocess exposed no import boundaries; evaluated import closure is not inspectable.");
                    continue;
                }

                foreach (var absolute in imports)
                {
                    findings += CheckImportAuthority(root, spec, absolute, tracked, authority);
                }
            }

            return findings;
        }

        private static IReadOnlyList<string> ReadPreprocessedImportBoundaries(string path)
        {
            if (!File.Exists(path))
            {
                throw new InvalidOperationException("MSBuild did not produce the requested preprocessed project: " + path);
            }

            var lines = File.ReadAllLines(path);
            var imports = new SortedSet<string>(StringComparer.Ordinal);
            for (var i = 0; i < lines.Length; i++)
            {
                if (!IsBoundarySeparator(lines[i]))
                {
                    continue;
                }

                for (var j = i + 1; j < lines.Length && !IsBoundarySeparator(lines[j]); j++)
                {
                    var candidate = lines[j].Trim();
                    if (candidate.Length == 0 || !Path.IsPathRooted(candidate))
                    {
                        continue;
                    }

                    var full = Path.GetFullPath(candidate);
                    if (File.Exists(full))
                    {
                        imports.Add(full);
                    }
                }
            }

            return imports.ToArray();
        }

        private static bool IsBoundarySeparator(string line)
        {
            var trimmed = line.Trim();
            return trimmed.Length >= 32 && trimmed.All(ch => ch == '=');
        }

        private static int CheckImportAuthority(
            string root,
            ProjectSpec spec,
            string absolute,
            SortedSet<string> tracked,
            ExternalAuthority authority)
        {
            if (authority.IsSdkOrPack(absolute)
                || (spec.Kind == ProjectKind.Tests && authority.IsLockedTestPackagePath(absolute)))
            {
                return 0;
            }

            if (!ProcessExec.IsInside(root, absolute))
            {
                return Report(
                    "HK00-MSBUILD-IMPORT-EXTERNAL-UNTRUSTED",
                    "static",
                    spec.Name + ":" + absolute,
                    "Evaluated MSBuild import comes from outside Arkus ownership and the single external-authority model.");
            }

            var relative = ProcessExec.Relative(root, absolute);
            if (IsAllowedRepositoryImport(spec, relative))
            {
                return 0;
            }

            if (IsGeneratedIntermediate(relative) && !tracked.Contains(relative))
            {
                return 0;
            }

            return Report(
                "HK00-MSBUILD-IMPORT-UNTRUSTED",
                "static",
                spec.Name + ":" + relative,
                "Evaluated MSBuild import closure contains repository-owned build logic outside the fixed HK00 surface.");
        }

        private static SortedSet<string> ReadTrackedFiles(string root)
        {
            var tracked = new SortedSet<string>(StringComparer.Ordinal);
            var result = ProcessExec.Run("git", new[] { "ls-files", "-z" }, root);
            foreach (var raw in ProcessExec.SplitNull(result.Stdout))
            {
                tracked.Add(raw.Replace('\\', '/'));
            }
            return tracked;
        }

        private static bool IsAllowedRepositoryImport(ProjectSpec spec, string relative)
        {
            return string.Equals(relative, spec.Path, StringComparison.Ordinal)
                || string.Equals(relative, "Directory.Build.props", StringComparison.Ordinal)
                || string.Equals(relative, "Directory.Packages.props", StringComparison.Ordinal);
        }

        private static bool IsGeneratedIntermediate(string relative)
        {
            return relative.Split('/').Any(segment => string.Equals(segment, "obj", StringComparison.Ordinal));
        }

        private static int Report(string id, string phase, string subject, string message)
        {
            Console.Error.WriteLine(id + " [" + phase + "] " + subject + ": " + message);
            return 1;
        }
    }
}
