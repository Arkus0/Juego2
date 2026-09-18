using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace Arkus.HK00.Proof
{
    /// <summary>
    /// Additional fixed-contract checks that deliberately live outside mutable inventory data.
    /// They close compiler-extension and effective-edge classes discovered during Worker pre-freeze audit.
    /// </summary>
    internal static class AdditionalChecks
    {
        public static int RunStatic(string root, string configuration)
        {
            var findings = 0;
            var sdkDirectory = RunningSdkDirectory(root);

            findings += CheckOwnedBuildXml(root);

            foreach (var spec in FixedContract.Projects)
            {
                if (!spec.IsProduct)
                {
                    continue;
                }

                findings += CheckProductAnalyzers(root, configuration, sdkDirectory, spec);
            }

            return findings;
        }

        public static int RunEffective(string root, string configuration)
        {
            var findings = 0;
            var probe = new MsBuildProbe(root, configuration);

            foreach (var spec in FixedContract.Projects)
            {
                if (!spec.IsProduct)
                {
                    continue;
                }

                ProjectFacts facts;
                try
                {
                    facts = probe.Evaluate(spec);
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-EFFECTIVE-EDGE-ORACLE", "effective", spec.Name, ex.Message);
                    continue;
                }

                var targetPath = facts.Property("TargetPath");
                if (string.IsNullOrWhiteSpace(targetPath) || !File.Exists(targetPath))
                {
                    findings += Report(
                        "HK00-EFFECTIVE-EDGE-ORACLE",
                        "effective",
                        spec.Name,
                        "Cannot verify exercised dependency edges because the built assembly is absent: " + targetPath);
                    continue;
                }

                AssemblyFacts assembly;
                try
                {
                    assembly = AssemblyFacts.Read(targetPath);
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-EFFECTIVE-EDGE-ORACLE", "effective", spec.Name, ex.Message);
                    continue;
                }

                var actual = new HashSet<string>(assembly.AssemblyReferences, StringComparer.Ordinal);
                foreach (var dependency in spec.Dependencies)
                {
                    if (!actual.Contains(dependency))
                    {
                        findings += Report(
                            "HK00-ASSEMBLY-REF-MISSING",
                            "effective",
                            spec.Name + " -> " + dependency,
                            "Required dependency is declared by project graph but absent from the produced assembly; the edge is decorative rather than causally exercised.");
                    }
                }
            }

            return findings;
        }

        private static int CheckOwnedBuildXml(string root)
        {
            var findings = 0;
            var files = new List<string>
            {
                "Directory.Build.props",
                "Directory.Packages.props",
            };
            files.AddRange(FixedContract.Projects.Select(p => p.Path));

            foreach (var relative in files)
            {
                var full = Path.Combine(root, relative);
                if (!File.Exists(full))
                {
                    continue;
                }

                XDocument document;
                try
                {
                    document = XDocument.Load(full, LoadOptions.PreserveWhitespace);
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-BUILD-XML", "static", relative, "Build XML cannot be inspected: " + ex.Message);
                    continue;
                }

                if (relative.EndsWith(".csproj", StringComparison.Ordinal))
                {
                    var sdk = document.Root?.Attribute("Sdk")?.Value ?? string.Empty;
                    if (!string.Equals(sdk, "Microsoft.NET.Sdk", StringComparison.Ordinal))
                    {
                        findings += Report(
                            "HK00-PROJECT-SDK",
                            "static",
                            relative,
                            "Fixed HK00 projects must use exactly Microsoft.NET.Sdk; observed '" + sdk + "'.");
                    }
                }

                foreach (var element in document.Descendants())
                {
                    if (string.Equals(element.Name.LocalName, "Import", StringComparison.Ordinal))
                    {
                        findings += Report(
                            "HK00-CUSTOM-IMPORT",
                            "static",
                            relative,
                            "Repository-owned HK00 project/build policy contains an explicit Import; custom build extensions must not become an unproved toolchain surface.");
                    }
                }
            }

            return findings;
        }

        private static int CheckProductAnalyzers(
            string root,
            string configuration,
            string sdkDirectory,
            ProjectSpec spec)
        {
            var args = new List<string>
            {
                "msbuild",
                Path.Combine(root, spec.Path),
                "-nologo",
                "-noAutoResponse",
                "-p:Configuration=" + configuration,
                "-getItem:Analyzer",
            };

            ProcessResult result;
            try
            {
                result = ProcessExec.Run("dotnet", args, root);
            }
            catch (Exception ex)
            {
                return Report("HK00-ANALYZER-ORACLE", "static", spec.Name, ex.Message);
            }

            using var document = JsonDocument.Parse(result.Stdout);
            if (!document.RootElement.TryGetProperty("Items", out var items)
                || !items.TryGetProperty("Analyzer", out var analyzers))
            {
                return 0;
            }

            var findings = 0;
            foreach (var analyzer in analyzers.EnumerateArray())
            {
                var path = analyzer.TryGetProperty("FullPath", out var fullPath)
                    ? fullPath.GetString() ?? string.Empty
                    : analyzer.TryGetProperty("Identity", out var identity)
                        ? identity.GetString() ?? string.Empty
                        : string.Empty;

                if (string.IsNullOrWhiteSpace(path))
                {
                    findings += Report(
                        "HK00-ANALYZER-UNTRUSTED",
                        "static",
                        spec.Name,
                        "Analyzer item has no inspectable path.");
                    continue;
                }

                var absolute = Path.GetFullPath(path);
                if (!ProcessExec.IsInside(sdkDirectory, absolute))
                {
                    findings += Report(
                        "HK00-ANALYZER-UNTRUSTED",
                        "static",
                        spec.Name + ":" + absolute,
                        "Portable/product project loads an analyzer or source-generator outside the pinned .NET SDK. Custom compiler extensions are forbidden in HK00.");
                }
            }

            return findings;
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

                var open = line.LastIndexOf('[', StringComparison.Ordinal);
                var close = line.LastIndexOf(']', StringComparison.Ordinal);
                if (open >= 0 && close > open)
                {
                    var baseDirectory = line.Substring(open + 1, close - open - 1);
                    return Path.GetFullPath(Path.Combine(baseDirectory, version));
                }
            }

            throw new InvalidOperationException("Could not derive the selected .NET SDK directory for version " + version + ".");
        }

        private static int Report(string id, string phase, string subject, string message)
        {
            Console.Error.WriteLine(id + " [" + phase + "] " + subject + ": " + message);
            return 1;
        }
    }
}
