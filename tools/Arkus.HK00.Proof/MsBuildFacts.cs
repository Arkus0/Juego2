using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Arkus.HK00.Proof
{
    internal sealed class PackageFact
    {
        public PackageFact(string id, string version)
        {
            Id = id;
            Version = version;
        }

        public string Id { get; }
        public string Version { get; }
    }

    internal sealed class ProjectFacts
    {
        public ProjectFacts(
            ProjectSpec spec,
            Dictionary<string, string> properties,
            Dictionary<string, IReadOnlyList<string>> items,
            IReadOnlyList<PackageFact> packages)
        {
            Spec = spec;
            Properties = properties;
            Items = items;
            Packages = packages;
        }

        public ProjectSpec Spec { get; }
        public Dictionary<string, string> Properties { get; }
        public Dictionary<string, IReadOnlyList<string>> Items { get; }
        public IReadOnlyList<PackageFact> Packages { get; }

        public string Property(string name) => Properties.TryGetValue(name, out var value) ? value : string.Empty;
        public IReadOnlyList<string> Item(string name) => Items.TryGetValue(name, out var values) ? values : Array.Empty<string>();
    }

    internal sealed class MsBuildProbe
    {
        private static readonly string[] Properties =
        {
            "AssemblyName",
            "TargetFramework",
            "LangVersion",
            "Nullable",
            "TreatWarningsAsErrors",
            "DisableTransitiveProjectReferences",
            "Deterministic",
            "DebugType",
            "EmbedAllSources",
            "GenerateAssemblyInfo",
            "GenerateTargetFrameworkAttribute",
            "ImplicitUsings",
            "ManagePackageVersionsCentrally",
            "NoWarn",
            "WarningsNotAsErrors",
            "TargetPath",
            "TargetRefPath",
        };

        private static readonly string[] Items =
        {
            "Compile",
            "ProjectReference",
            "PackageReference",
            "Reference",
        };

        private readonly string root;
        private readonly string configuration;

        public MsBuildProbe(string root, string configuration)
        {
            this.root = root;
            this.configuration = configuration;
        }

        public ProjectFacts Evaluate(ProjectSpec spec)
        {
            var fullProject = Path.Combine(root, spec.Path);
            var args = new List<string>
            {
                "msbuild",
                fullProject,
                "-nologo",
                "-noAutoResponse",
                $"-p:Configuration={configuration}",
            };

            foreach (var property in Properties)
            {
                args.Add($"-getProperty:{property}");
            }

            foreach (var item in Items)
            {
                args.Add($"-getItem:{item}");
            }

            var result = ProcessExec.Run("dotnet", args, root);
            using var document = JsonDocument.Parse(result.Stdout);
            var properties = new Dictionary<string, string>(StringComparer.Ordinal);
            var items = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
            var packages = new List<PackageFact>();

            if (document.RootElement.TryGetProperty("Properties", out var props))
            {
                foreach (var prop in props.EnumerateObject())
                {
                    properties[prop.Name] = prop.Value.GetString() ?? string.Empty;
                }
            }

            if (document.RootElement.TryGetProperty("Items", out var itemRoot))
            {
                foreach (var itemType in itemRoot.EnumerateObject())
                {
                    var values = new List<string>();
                    foreach (var entry in itemType.Value.EnumerateArray())
                    {
                        var identity = entry.TryGetProperty("Identity", out var id) ? id.GetString() ?? string.Empty : string.Empty;
                        var fullPath = entry.TryGetProperty("FullPath", out var fp) ? fp.GetString() ?? string.Empty : string.Empty;
                        if (string.Equals(itemType.Name, "PackageReference", StringComparison.Ordinal))
                        {
                            var version = entry.TryGetProperty("Version", out var ver) ? ver.GetString() ?? string.Empty : string.Empty;
                            packages.Add(new PackageFact(identity, version));
                            values.Add(identity);
                        }
                        else if (string.Equals(itemType.Name, "Compile", StringComparison.Ordinal)
                            || string.Equals(itemType.Name, "ProjectReference", StringComparison.Ordinal))
                        {
                            values.Add(fullPath);
                        }
                        else
                        {
                            values.Add(identity);
                        }
                    }

                    values.Sort(StringComparer.Ordinal);
                    items[itemType.Name] = values;
                }
            }

            return new ProjectFacts(spec, properties, items, packages);
        }

        public IReadOnlyList<string> CompilerArguments(ProjectSpec spec)
        {
            var fullProject = Path.Combine(root, spec.Path);
            var args = new List<string>
            {
                "build",
                fullProject,
                "-nologo",
                "-noAutoResponse",
                $"-p:Configuration={configuration}",
                "-t:Rebuild",
                "-p:ProvideCommandLineArgs=true",
                "-getItem:CscCommandLineArgs",
            };

            var result = ProcessExec.Run("dotnet", args, root);
            using var document = JsonDocument.Parse(result.Stdout);
            var values = new List<string>();
            if (document.RootElement.TryGetProperty("Items", out var itemRoot)
                && itemRoot.TryGetProperty("CscCommandLineArgs", out var compilerArgs))
            {
                foreach (var entry in compilerArgs.EnumerateArray())
                {
                    if (entry.TryGetProperty("Identity", out var identity))
                    {
                        values.Add(identity.GetString() ?? string.Empty);
                    }
                }
            }

            if (values.Count == 0)
            {
                throw new InvalidOperationException($"No CscCommandLineArgs were produced for {spec.Name}.");
            }

            return values;
        }
    }
}
