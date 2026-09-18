using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// Extracts evaluated MSBuild properties and items by invoking the pinned SDK.
    /// </summary>
    /// <remarks>
    /// Reading project XML would only prove what the file says. Asking MSBuild for
    /// the evaluated values proves what the build actually resolves, including
    /// everything inherited or injected.
    /// </remarks>
    public sealed class MsBuildEvaluator
    {
        private static readonly string[] RequestedProperties =
        {
            "AssemblyName",
            "Configuration",
            "DebugType",
            "Deterministic",
            "DisableTransitiveProjectReferences",
            "EmbedAllSources",
            "EnableNETAnalyzers",
            "ImplicitUsings",
            "LangVersion",
            "ManagePackageVersionsCentrally",
            "NoWarn",
            "Nullable",
            "OutputType",
            "TargetFramework",
            "TargetPath",
            "TreatWarningsAsErrors",
            "WarningsNotAsErrors",
        };

        private static readonly string[] RequestedItems =
        {
            "Compile",
            "PackageReference",
            "ProjectReference",
            "Reference",
        };

        private readonly string configuration;
        private readonly List<string> properties = new List<string>();

        /// <summary>Creates an evaluator.</summary>
        /// <param name="configuration">Build configuration to evaluate.</param>
        /// <param name="additionalProperties">
        /// Property names the manifest requires. They are requested as well, so a
        /// property named in a class contract can never be silently unreadable and
        /// then reported as an empty mismatch.
        /// </param>
        public MsBuildEvaluator(string configuration, IEnumerable<string> additionalProperties)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var unique = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var property in RequestedProperties)
            {
                unique.Add(property);
            }

            if (additionalProperties is not null)
            {
                foreach (var property in additionalProperties)
                {
                    unique.Add(property);
                }
            }

            properties.AddRange(unique);
        }

        /// <summary>Evaluates one project.</summary>
        /// <param name="projectName">Project name used in findings.</param>
        /// <param name="projectPath">Absolute project file path.</param>
        /// <returns>The evaluated facts.</returns>
        public ProjectFacts Evaluate(string projectName, string projectPath)
        {
            if (projectPath is null)
            {
                throw new ArgumentNullException(nameof(projectPath));
            }

            if (!File.Exists(projectPath))
            {
                throw new ProofToolException($"Project file not found: {projectPath}");
            }

            var arguments = new List<string>
            {
                "msbuild",
                projectPath,
                "-nologo",
                "-noAutoResponse",
                $"-p:Configuration={configuration}",
            };

            foreach (var property in properties)
            {
                arguments.Add($"-getProperty:{property}");
            }

            foreach (var item in RequestedItems)
            {
                arguments.Add($"-getItem:{item}");
            }

            var output = RunDotnet(arguments, Path.GetDirectoryName(projectPath)!);
            return Parse(projectName, projectPath, output);
        }

        /// <summary>
        /// Rebuilds a project and returns the arguments the C# compiler actually
        /// received.
        /// </summary>
        /// <param name="projectPath">Absolute project file path.</param>
        /// <returns>Compiler arguments in command-line order.</returns>
        /// <remarks>
        /// The rebuild is required: an up-to-date project skips <c>CoreCompile</c>
        /// and would report no arguments at all, which must never be mistaken for
        /// a project compiled with an empty, harmless command line.
        /// </remarks>
        public IReadOnlyList<string> CompilerArguments(string projectPath)
        {
            if (projectPath is null)
            {
                throw new ArgumentNullException(nameof(projectPath));
            }

            var arguments = new List<string>
            {
                "build",
                projectPath,
                "-nologo",
                "-noAutoResponse",
                $"-p:Configuration={configuration}",
                "-t:Rebuild",
                "-p:ProvideCommandLineArgs=true",
                "-getItem:CscCommandLineArgs",
            };

            var output = RunDotnet(arguments, Path.GetDirectoryName(projectPath)!);

            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(output);
            }
            catch (JsonException ex)
            {
                throw new ProofToolException(
                    $"Could not read the compiler command line for '{projectPath}'.",
                    ex);
            }

            using (document)
            {
                var results = new List<string>();
                if (document.RootElement.TryGetProperty("Items", out var items)
                    && items.TryGetProperty("CscCommandLineArgs", out var args))
                {
                    foreach (var entry in args.EnumerateArray())
                    {
                        if (entry.TryGetProperty("Identity", out var identity))
                        {
                            results.Add(identity.GetString() ?? string.Empty);
                        }
                    }
                }

                if (results.Count == 0)
                {
                    throw new ProofToolException(
                        $"No compiler command line was reported for '{projectPath}'; the compilation claim cannot be checked.");
                }

                return results;
            }
        }

        /// <summary>Reads the running SDK version.</summary>
        /// <param name="workingDirectory">Directory whose SDK pin applies.</param>
        /// <returns>The SDK version text.</returns>
        public static string RunningSdkVersion(string workingDirectory)
        {
            return RunDotnet(new List<string> { "--version" }, workingDirectory).Trim();
        }

        private static string RunDotnet(IReadOnlyList<string> arguments, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

            startInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
            startInfo.Environment["DOTNET_NOLOGO"] = "1";
            startInfo.Environment["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1";

            using var process = new Process { StartInfo = startInfo };
            var standardOutput = new StringBuilder();
            var standardError = new StringBuilder();

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    standardOutput.AppendLine(e.Data);
                }
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    standardError.AppendLine(e.Data);
                }
            };

            if (!process.Start())
            {
                throw new ProofToolException("Failed to start 'dotnet'.");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var joined = string.Join(" ", arguments);
                throw new ProofToolException(
                    FormattableString.Invariant($"'dotnet {joined}' failed with exit code {process.ExitCode}.{Environment.NewLine}{standardOutput}{standardError}"));
            }

            return standardOutput.ToString();
        }

        private static ProjectFacts Parse(string projectName, string projectPath, string json)
        {
            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(json);
            }
            catch (JsonException ex)
            {
                throw new ProofToolException($"MSBuild evaluation of '{projectPath}' produced non-JSON output.", ex);
            }

            using (document)
            {
                var properties = new Dictionary<string, string>(StringComparer.Ordinal);
                if (document.RootElement.TryGetProperty("Properties", out var propertiesElement))
                {
                    foreach (var property in propertiesElement.EnumerateObject())
                    {
                        properties[property.Name] = property.Value.GetString() ?? string.Empty;
                    }
                }

                var items = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
                var packageReferences = new List<PackageReferenceFact>();

                if (document.RootElement.TryGetProperty("Items", out var itemsElement))
                {
                    foreach (var itemType in itemsElement.EnumerateObject())
                    {
                        var values = new List<string>();
                        foreach (var entry in itemType.Value.EnumerateArray())
                        {
                            var fullPath = entry.TryGetProperty("FullPath", out var fp)
                                ? fp.GetString() ?? string.Empty
                                : string.Empty;
                            var identity = entry.TryGetProperty("Identity", out var id)
                                ? id.GetString() ?? string.Empty
                                : string.Empty;

                            if (string.Equals(itemType.Name, "PackageReference", StringComparison.Ordinal))
                            {
                                var version = entry.TryGetProperty("Version", out var ver)
                                    ? ver.GetString() ?? string.Empty
                                    : string.Empty;
                                packageReferences.Add(new PackageReferenceFact(identity, version));
                                values.Add(identity);
                                continue;
                            }

                            values.Add(
                                string.Equals(itemType.Name, "Compile", StringComparison.Ordinal)
                                || string.Equals(itemType.Name, "ProjectReference", StringComparison.Ordinal)
                                    ? fullPath
                                    : identity);
                        }

                        values.Sort(StringComparer.Ordinal);
                        items[itemType.Name] = values;
                    }
                }

                return new ProjectFacts(projectName, projectPath, properties, items, packageReferences);
            }
        }
    }
}
