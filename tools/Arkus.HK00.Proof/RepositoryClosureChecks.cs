using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

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

                    if (authority.IsSdkOrPack(absolute)
                        || (spec.Kind == ProjectKind.Tests && authority.IsLockedTestPackagePath(absolute)))
                    {
                        continue;
                    }

                    if (!ProcessExec.IsInside(root, absolute))
                    {
                        findings += Report(
                            "HK00-MSBUILD-IMPORT-EXTERNAL-UNTRUSTED",
                            "static",
                            spec.Name + ":" + absolute,
                            "Evaluated MSBuild import comes from outside Arkus ownership and the single external-authority model.");
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
