using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arkus.HK00.Proof
{
    internal static class NonProductEffectiveChecks
    {
        public static int Run(string root, string configuration)
        {
            var findings = 0;
            var probe = new MsBuildProbe(root, configuration);
            var trackedSources = ReadTrackedSources(root);

            foreach (var spec in FixedContract.Projects)
            {
                if (spec.IsProduct)
                {
                    continue;
                }

                ProjectFacts facts;
                CompilerCommandLine compiler;
                try
                {
                    facts = probe.Evaluate(spec);
                    compiler = CompilerCommandLine.Parse(
                        probe.CompilerArguments(spec),
                        Path.Combine(root, spec.Directory));
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-NONPRODUCT-EFFECTIVE-ORACLE", "effective", spec.Name, ex.Message);
                    continue;
                }

                findings += CheckOptions(spec, compiler);
                findings += CheckSources(root, spec, compiler, trackedSources);
                findings += CheckAssembly(root, spec, facts, trackedSources);

                if (spec.RequiresTrustedCompilerExtensions)
                {
                    findings += CheckTrustedAnalyzers(root, spec, compiler);
                }
            }

            return findings;
        }

        private static int CheckOptions(ProjectSpec spec, CompilerCommandLine compiler)
        {
            var findings = 0;
            if (!string.Equals(compiler.Value("langversion"), "9.0", StringComparison.OrdinalIgnoreCase))
            {
                findings += Report("HK00-COMPILER-OPTION", "effective", spec.Name + ":langversion", "Non-product canonical project did not compile as C# 9.");
            }
            if (!string.Equals(compiler.Value("nullable"), "enable", StringComparison.OrdinalIgnoreCase))
            {
                findings += Report("HK00-COMPILER-OPTION", "effective", spec.Name + ":nullable", "Nullable context differs from the fixed contract.");
            }
            if (!compiler.HasSwitch("warnaserror+") || compiler.HasSwitch("warnaserror-"))
            {
                findings += Report("HK00-COMPILER-OPTION", "effective", spec.Name + ":warnaserror", "Warnings-as-errors is not effective.");
            }
            if (!compiler.HasSwitch("deterministic+"))
            {
                findings += Report("HK00-COMPILER-OPTION", "effective", spec.Name + ":deterministic", "Deterministic compiler mode is not effective.");
            }
            if (!string.Equals(compiler.Value("debug"), "portable", StringComparison.OrdinalIgnoreCase))
            {
                findings += Report("HK00-COMPILER-OPTION", "effective", spec.Name + ":debug", "Portable PDB generation is not effective.");
            }
            return findings;
        }

        private static int CheckSources(
            string root,
            ProjectSpec spec,
            CompilerCommandLine compiler,
            SortedSet<string> trackedSources)
        {
            var findings = 0;
            var prefix = spec.Directory.TrimEnd('/') + "/";
            var expected = new SortedSet<string>(
                trackedSources.Where(path => path.StartsWith(prefix, StringComparison.Ordinal)),
                StringComparer.Ordinal);
            var actual = new SortedSet<string>(StringComparer.Ordinal);

            foreach (var source in compiler.Sources)
            {
                var full = Path.GetFullPath(source);
                if (!ProcessExec.IsInside(root, full))
                {
                    findings += Report(
                        "HK00-COMPILER-SOURCE-FOREIGN",
                        "effective",
                        spec.Name + ":" + full,
                        "Canonical project compiler consumed source outside the repository.");
                    continue;
                }

                var relative = ProcessExec.Relative(root, full);
                actual.Add(relative);
                if (!expected.Contains(relative))
                {
                    findings += Report(
                        "HK00-COMPILER-SOURCE-UNTRACKED",
                        "effective",
                        spec.Name + ":" + relative,
                        "Canonical project compiler consumed source outside its tracked ownership boundary.");
                }
            }

            foreach (var source in expected)
            {
                if (!actual.Contains(source))
                {
                    findings += Report(
                        "HK00-SOURCE-NOT-COMPILED-EFFECTIVE",
                        "effective",
                        spec.Name + ":" + source,
                        "Tracked owned source did not reach the actual compiler command line.");
                }
            }

            return findings;
        }

        private static int CheckAssembly(
            string root,
            ProjectSpec spec,
            ProjectFacts facts,
            SortedSet<string> trackedSources)
        {
            var targetPath = facts.Property("TargetPath");
            if (string.IsNullOrWhiteSpace(targetPath) || !File.Exists(targetPath))
            {
                return Report("HK00-ASSEMBLY-MISSING", "effective", spec.Name, "Expected output is absent: " + targetPath);
            }

            AssemblyFacts assembly;
            try
            {
                assembly = AssemblyFacts.Read(targetPath);
            }
            catch (Exception ex)
            {
                return Report("HK00-ASSEMBLY-ORACLE", "effective", spec.Name, ex.Message);
            }

            var findings = 0;
            if (!string.Equals(assembly.AssemblyName, spec.Name, StringComparison.Ordinal))
            {
                findings += Report("HK00-ASSEMBLY-NAME", "effective", spec.Name, "Built assembly identity differs from the fixed project identity.");
            }

            var expectedDependencies = new SortedSet<string>(spec.Dependencies, StringComparer.Ordinal);
            var actualReferences = new SortedSet<string>(assembly.AssemblyReferences, StringComparer.Ordinal);
            foreach (var dependency in expectedDependencies)
            {
                if (!actualReferences.Contains(dependency))
                {
                    findings += Report(
                        "HK00-ASSEMBLY-REF-MISSING",
                        "effective",
                        spec.Name + " -> " + dependency,
                        "Required canonical dependency is absent from emitted IL.");
                }
            }
            foreach (var reference in actualReferences)
            {
                if (FixedContract.ByName(reference) is not null && !expectedDependencies.Contains(reference))
                {
                    findings += Report(
                        "HK00-ASSEMBLY-REF-UNDECLARED",
                        "effective",
                        spec.Name + " -> " + reference,
                        "Built assembly contains an undeclared canonical dependency.");
                }
            }

            var prefix = spec.Directory.TrimEnd('/') + "/";
            var expectedSources = new SortedSet<string>(
                trackedSources.Where(path => path.StartsWith(prefix, StringComparison.Ordinal)),
                StringComparer.Ordinal);
            var documents = new Dictionary<string, CompiledDocument>(StringComparer.Ordinal);
            foreach (var document in assembly.CompiledDocuments)
            {
                var full = Path.IsPathRooted(document.Path)
                    ? Path.GetFullPath(document.Path)
                    : Path.GetFullPath(Path.Combine(Path.Combine(root, spec.Directory), document.Path));
                if (!ProcessExec.IsInside(root, full))
                {
                    findings += Report("HK00-PDB-SOURCE-FOREIGN", "effective", spec.Name + ":" + document.Path, "PDB records source outside the repository.");
                    continue;
                }

                var relative = ProcessExec.Relative(root, full);
                if (!expectedSources.Contains(relative))
                {
                    findings += Report("HK00-PDB-SOURCE-UNTRACKED", "effective", spec.Name + ":" + relative, "PDB records source outside the project's tracked ownership boundary.");
                    continue;
                }
                documents[relative] = document;
            }

            foreach (var source in expectedSources)
            {
                if (!documents.TryGetValue(source, out var document))
                {
                    findings += Report("HK00-PDB-SOURCE-MISSING", "effective", spec.Name + ":" + source, "Owned source is absent from the portable PDB.");
                    continue;
                }
                if (document.HashAlgorithm != AssemblyFacts.Sha256Algorithm
                    || !AssemblyFacts.HashEquals(document.Hash, AssemblyFacts.Sha256OfFile(Path.Combine(root, source))))
                {
                    findings += Report("HK00-PDB-SOURCE-HASH", "effective", spec.Name + ":" + source, "PDB checksum does not match candidate bytes.");
                }
            }

            return findings;
        }

        private static int CheckTrustedAnalyzers(string root, ProjectSpec spec, CompilerCommandLine compiler)
        {
            var findings = 0;
            var sdkDirectory = RunningSdkDirectory(root);
            var dotnetRoot = Directory.GetParent(Directory.GetParent(sdkDirectory)!.FullName)!.FullName;
            var packsDirectory = Path.Combine(dotnetRoot, "packs");

            foreach (var value in compiler.Values("analyzer"))
            {
                foreach (var rawPath in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var token = rawPath.Trim().Trim('"');
                    if (token.Length == 0)
                    {
                        continue;
                    }

                    var projectDirectory = Path.Combine(root, spec.Directory);
                    var absolute = Path.IsPathRooted(token)
                        ? Path.GetFullPath(token)
                        : Path.GetFullPath(Path.Combine(projectDirectory, token));
                    if (!ProcessExec.IsInside(sdkDirectory, absolute)
                        && !(Directory.Exists(packsDirectory) && ProcessExec.IsInside(packsDirectory, absolute)))
                    {
                        findings += Report(
                            "HK00-COMPILER-ANALYZER-UNTRUSTED",
                            "effective",
                            spec.Name + ":" + absolute,
                            "Proof authority may load compiler extensions only from the selected SDK/reference packs.");
                    }
                }
            }

            return findings;
        }

        private static SortedSet<string> ReadTrackedSources(string root)
        {
            var result = ProcessExec.Run("git", new[] { "ls-files", "-z" }, root);
            var sources = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var raw in ProcessExec.SplitNull(result.Stdout))
            {
                var path = raw.Replace('\\', '/');
                if (path.EndsWith(".cs", StringComparison.Ordinal))
                {
                    sources.Add(path);
                }
            }
            return sources;
        }

        private static string RunningSdkDirectory(string root)
        {
            var version = ProcessExec.Run("dotnet", new[] { "--version" }, root).Stdout.Trim();
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

        private static int Report(string id, string phase, string subject, string message)
        {
            Console.Error.WriteLine(id + " [" + phase + "] " + subject + ": " + message);
            return 1;
        }
    }
}
