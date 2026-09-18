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
                result = ProcessExec.Run("dotnet", new[] { "sln", "Juego2.sln", "list" }, root);
            }
            catch (Exception ex)
            {
                return Report("HK00-SOLUTION-ORACLE", "repository", "Juego2.sln", ex.Message);
            }

            var actual = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var rawLine in result.Stdout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var line = rawLine.Trim();
                if (!line.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
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
                findings += Report(
                    "HK00-SOLUTION-ORACLE",
                    "repository",
                    "Juego2.sln",
                    "dotnet sln produced no inspectable C# project membership.");
                return findings;
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
                        "Solution contains a project outside the fixed HK00 project contract.");
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
                        "Fixed HK00 project exists in the repository contract but is absent from Juego2.sln.");
                }
            }

            return findings;
        }

        public static int RunStatic(string root, string configuration)
        {
            var findings = 0;
            var tracked = ReadTrackedFiles(root);

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
                    using var document = JsonDocument.Parse(result.Stdout);
                    imports = document.RootElement.TryGetProperty("Properties", out var properties)
                        && properties.TryGetProperty("MSBuildAllProjects", out var allProjects)
                            ? allProjects.GetString() ?? string.Empty
                            : string.Empty;
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
                        "Evaluated MSBuild import closure contains a repository-owned extension outside the fixed HK00 build-policy surface.");
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
