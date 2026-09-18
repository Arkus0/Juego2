using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Arkus.HK00.Proof
{
    internal static class RepositoryClosureChecks
    {
        private const string TestLockPath = "tests/Arkus.Harness.Tests/packages.lock.json";

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
            var sdkDirectory = RunningSdkDirectory(root);
            var dotnetRoot = Directory.GetParent(Directory.GetParent(sdkDirectory)!.FullName)!.FullName;
            var packsDirectory = Path.Combine(dotnetRoot, "packs");
            var packageRoot = PackageRoot();
            Dictionary<string, string> lockedPackages;
            try
            {
                lockedPackages = ReadLockedPackages(root);
            }
            catch (Exception ex)
            {
                return Report("HK00-DEPENDENCY-LOCK", "static", TestLockPath, ex.Message);
            }

            foreach (var spec in FixedContract.Projects)
            {
                var args = new List<string>
                {
                    "msbuild",
                    Path.Combine(root, spec.Path),
                    "-nologo",
                    "-noAutoResponse",
                    "-p:Configuration=" + configuration,
                    "-getProperty:MSBuildAllProjects",
                };

                ProcessResult result;
                try
                {
                    result = ProcessExec.Run("dotnet", args, root);
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-MSBUILD-IMPORT-ORACLE", "static", spec.Name, ex.Message);
                    continue;
                }

                string imports;
                try
                {
                    var stdout = result.Stdout.Trim();
                    if (stdout.StartsWith("{", StringComparison.Ordinal))
                    {
                        using var document = JsonDocument.Parse(stdout);
                        imports = document.RootElement.TryGetProperty("Properties", out var properties)
                            && properties.TryGetProperty("MSBuildAllProjects", out var allProjects)
                                ? allProjects.GetString() ?? string.Empty
                                : string.Empty;
                    }
                    else
                    {
                        imports = stdout;
                    }
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-MSBUILD-IMPORT-ORACLE", "static", spec.Name, "MSBuildAllProjects output is unreadable: " + ex.Message);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(imports))
                {
                    findings += Report(
                        "HK00-MSBUILD-IMPORT-ORACLE",
                        "static",
                        spec.Name,
                        "MSBuildAllProjects was empty; evaluated import closure is not inspectable.");
                    continue;
                }

                foreach (var raw in imports.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var token = raw.Trim();
                    if (token.Length == 0)
                    {
                        continue;
                    }

                    var absolute = Path.IsPathRooted(token)
                        ? Path.GetFullPath(token)
                        : Path.GetFullPath(Path.Combine(root, token));

                    if (!ProcessExec.IsInside(root, absolute))
                    {
                        if (IsAllowedExternalImport(spec, absolute, sdkDirectory, packsDirectory, packageRoot, lockedPackages))
                        {
                            continue;
                        }

                        findings += Report(
                            "HK00-MSBUILD-IMPORT-EXTERNAL-UNTRUSTED",
                            "static",
                            spec.Name + ":" + absolute,
                            "Evaluated MSBuild import comes from outside the repository, selected SDK/packs, and exact locked test package closure.");
                        continue;
                    }

                    var relative = ProcessExec.Relative(root, absolute);
                    if (IsAllowedRepositoryImport(spec, relative))
                    {
                        continue;
                    }

                    if (IsGeneratedIntermediate(relative) && !tracked.Contains(relative))
                    {
                        continue;
                    }

                    findings += Report(
                        "HK00-MSBUILD-IMPORT-UNTRUSTED",
                        "static",
                        spec.Name + ":" + relative,
                        "Evaluated MSBuild import closure contains repository-owned build logic outside the fixed HK00 surface.");
                }
            }

            return findings;
        }

        private static bool IsAllowedExternalImport(
            ProjectSpec spec,
            string absolute,
            string sdkDirectory,
            string packsDirectory,
            string packageRoot,
            IReadOnlyDictionary<string, string> lockedPackages)
        {
            if (ProcessExec.IsInside(sdkDirectory, absolute)
                || (Directory.Exists(packsDirectory) && ProcessExec.IsInside(packsDirectory, absolute)))
            {
                return true;
            }

            if (spec.Kind != ProjectKind.Tests || !ProcessExec.IsInside(packageRoot, absolute))
            {
                return false;
            }

            var relative = ProcessExec.Relative(packageRoot, absolute);
            var segments = relative.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 3)
            {
                return false;
            }

            var packageId = segments[0].ToLowerInvariant();
            var packageVersion = segments[1];
            return lockedPackages.TryGetValue(packageId, out var lockedVersion)
                && string.Equals(packageVersion, lockedVersion, StringComparison.OrdinalIgnoreCase);
        }

        private static Dictionary<string, string> ReadLockedPackages(string root)
        {
            var path = Path.Combine(root, TestLockPath);
            if (!File.Exists(path))
            {
                throw new InvalidOperationException("Committed test package lock is missing.");
            }

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            if (!document.RootElement.TryGetProperty("dependencies", out var dependencies)
                || !dependencies.TryGetProperty("net8.0", out var target))
            {
                throw new InvalidOperationException("Committed test package lock has no net8.0 dependency graph.");
            }

            var packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var dependency in target.EnumerateObject())
            {
                if (!dependency.Value.TryGetProperty("resolved", out var resolved))
                {
                    continue;
                }
                var version = resolved.GetString();
                if (string.IsNullOrWhiteSpace(version))
                {
                    continue;
                }
                packages[dependency.Name.ToLowerInvariant()] = version;
            }

            if (packages.Count == 0)
            {
                throw new InvalidOperationException("Committed test package lock contains no resolved packages.");
            }
            return packages;
        }

        private static string PackageRoot()
        {
            var configured = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return Path.GetFullPath(configured);
            }

            return Path.GetFullPath(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nuget",
                "packages"));
        }

        private static string RunningSdkDirectory(string root)
        {
            var version = ProcessExec.Run("dotnet", new[] { "--version" }, root).Stdout.Trim();
            if (!string.Equals(version, FixedContract.SdkVersion, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unexpected SDK while classifying MSBuild imports: " + version);
            }

            var list = ProcessExec.Run("dotnet", new[] { "--list-sdks" }, root).Stdout;
            foreach (var rawLine in list.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var line = rawLine.Trim();
                if (!line.StartsWith(version + " ", StringComparison.Ordinal))
                {
                    continue;
                }
                var open = line.LastIndexOf('[');
                var close = line.LastIndexOf(']');
                if (open >= 0 && close > open)
                {
                    var baseDirectory = line.Substring(open + 1, close - open - 1);
                    return Path.GetFullPath(Path.Combine(baseDirectory, version));
                }
            }
            throw new InvalidOperationException("Could not derive selected SDK directory for " + version + ".");
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
