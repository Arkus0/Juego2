using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Arkus.HK00.Proof
{
    internal static class CompilerInputClosureChecks
    {
        private static readonly string[] ForbiddenInputSwitches =
        {
            "addmodule",
            "link",
            "resource",
            "linkresource",
            "win32res",
            "win32icon",
            "recurse",
            "additionalfile",
        };

        public static int Run(string root, string configuration)
        {
            var findings = 0;
            var probe = new MsBuildProbe(root, configuration);
            ExternalAuthority authority;
            try
            {
                authority = ExternalAuthority.Create(root);
            }
            catch (Exception ex)
            {
                return Report("HK00-EXTERNAL-AUTHORITY", "effective", "external-authority", ex.Message);
            }

            foreach (var spec in FixedContract.Projects)
            {
                CompilerCommandLine compiler;
                try
                {
                    compiler = CompilerCommandLine.Parse(
                        probe.CompilerArguments(spec),
                        Path.Combine(root, spec.Directory));
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-COMPILER-INPUT-ORACLE", "effective", spec.Name, ex.Message);
                    continue;
                }

                foreach (var name in ForbiddenInputSwitches)
                {
                    foreach (var value in compiler.Values(name))
                    {
                        findings += Report(
                            "HK00-COMPILER-INPUT-UNCLASSIFIED",
                            "effective",
                            spec.Name + ":/" + name,
                            "Compiler received an input-bearing switch outside the closed HK00 input model: " + value);
                    }
                }

                findings += CheckAnalyzerConfigs(root, spec, compiler, authority);
                findings += CheckReferences(root, configuration, spec, compiler, probe, authority);
            }

            return findings;
        }

        public static void WriteInventory(string root, string configuration, string path)
        {
            var probe = new MsBuildProbe(root, configuration);
            var authority = ExternalAuthority.Create(root);
            var rows = new List<object>();

            foreach (var spec in FixedContract.Projects.OrderBy(project => project.Name, StringComparer.Ordinal))
            {
                var arguments = probe.CompilerArguments(spec);
                var normalized = new List<string>(arguments.Count);
                foreach (var argument in arguments)
                {
                    normalized.Add(NormalizeArgument(root, authority, argument));
                }

                rows.Add(new
                {
                    Project = spec.Name,
                    Arguments = normalized,
                });
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, JsonSerializer.Serialize(rows, options) + Environment.NewLine);
        }

        private static string NormalizeArgument(string root, ExternalAuthority authority, string argument)
        {
            var value = argument.Replace('\\', '/');
            var replacements = new[]
            {
                new KeyValuePair<string, string>(Path.GetFullPath(authority.PackageRoot).Replace('\\', '/'), "$NUGET"),
                new KeyValuePair<string, string>(Path.GetFullPath(authority.SdkDirectory).Replace('\\', '/'), "$SDK"),
                new KeyValuePair<string, string>(Path.GetFullPath(authority.PacksDirectory).Replace('\\', '/'), "$PACKS"),
                new KeyValuePair<string, string>(Path.GetFullPath(root).Replace('\\', '/'), "$ROOT"),
            };

            foreach (var replacement in replacements.OrderByDescending(pair => pair.Key.Length))
            {
                value = value.Replace(replacement.Key, replacement.Value);
            }

            return value;
        }

        private static int CheckAnalyzerConfigs(string root, ProjectSpec spec, CompilerCommandLine compiler, ExternalAuthority authority)
        {
            var findings = 0;
            foreach (var value in compiler.Values("analyzerconfig"))
            {
                var token = value.Trim().Trim('"');
                if (token.Length == 0)
                {
                    continue;
                }

                var full = Path.IsPathRooted(token)
                    ? Path.GetFullPath(token)
                    : Path.GetFullPath(Path.Combine(Path.Combine(root, spec.Directory), token));
                var allowedObj = Path.Combine(root, spec.Directory, "obj");
                if (!ProcessExec.IsInside(allowedObj, full) && !authority.IsSdkOrPack(full))
                {
                    findings += Report(
                        "HK00-COMPILER-ANALYZERCONFIG-UNTRUSTED",
                        "effective",
                        spec.Name + ":" + full,
                        "Analyzer config input must come from the exact selected SDK/packs or this project's generated obj directory.");
                }
            }
            return findings;
        }

        private static int CheckReferences(
            string root,
            string configuration,
            ProjectSpec spec,
            CompilerCommandLine compiler,
            MsBuildProbe probe,
            ExternalAuthority authority)
        {
            var findings = 0;
            var dependencyOutputs = new HashSet<string>(StringComparer.Ordinal);
            foreach (var dependencyName in spec.Dependencies)
            {
                var dependency = FixedContract.ByName(dependencyName);
                if (dependency is null)
                {
                    findings += Report("HK00-COMPILER-REFERENCE-ORACLE", "effective", spec.Name + " -> " + dependencyName, "Fixed dependency has no project contract.");
                    continue;
                }

                try
                {
                    var facts = probe.Evaluate(dependency);
                    AddIfPresent(dependencyOutputs, facts.Property("TargetPath"));
                    AddIfPresent(dependencyOutputs, facts.Property("TargetRefPath"));

                    var dependencyRoot = Path.GetFullPath(Path.Combine(root, dependency.Directory));
                    var assemblyFile = dependency.Name + ".dll";
                    dependencyOutputs.Add(Path.GetFullPath(Path.Combine(dependencyRoot, "obj", configuration, dependency.TargetFramework, "ref", assemblyFile)));
                    dependencyOutputs.Add(Path.GetFullPath(Path.Combine(dependencyRoot, "obj", configuration, dependency.TargetFramework, "refint", assemblyFile)));
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-COMPILER-REFERENCE-ORACLE", "effective", spec.Name + " -> " + dependencyName, ex.Message);
                }
            }

            foreach (var value in compiler.Values("reference"))
            {
                foreach (var raw in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var token = raw.Trim().Trim('"');
                    if (token.Length == 0)
                    {
                        continue;
                    }

                    var equals = token.IndexOf('=');
                    if (equals > 0 && token.Substring(0, equals).IndexOfAny(new[] { '/', '\\' }) < 0)
                    {
                        token = token.Substring(equals + 1).Trim().Trim('"');
                    }

                    var full = Path.IsPathRooted(token)
                        ? Path.GetFullPath(token)
                        : Path.GetFullPath(Path.Combine(Path.Combine(root, spec.Directory), token));

                    if (authority.IsFrameworkReference(full)
                        || dependencyOutputs.Contains(full)
                        || (spec.Kind == ProjectKind.Tests && authority.IsLockedTestPackagePath(full)))
                    {
                        continue;
                    }

                    findings += Report(
                        "HK00-COMPILER-REFERENCE-UNTRUSTED",
                        "effective",
                        spec.Name + ":" + full,
                        "Compiler reference is outside framework packs, declared Arkus dependency outputs/reference assemblies, and the exact locked test package closure.");
                }
            }

            return findings;
        }

        private static void AddIfPresent(HashSet<string> outputs, string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                outputs.Add(Path.GetFullPath(path));
            }
        }

        private static int Report(string id, string phase, string subject, string message)
        {
            Console.Error.WriteLine(id + " [" + phase + "] " + subject + ": " + message);
            return 1;
        }
    }
}
