using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Arkus.Kernel.Proof
{
    /// <summary>One inventoried project.</summary>
    public sealed class ProjectInventoryEntry
    {
        /// <summary>Project name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Repository-relative project path.</summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>Manifest class.</summary>
        public string Class { get; set; } = string.Empty;

        /// <summary>Whether the class owns production source.</summary>
        public bool Production { get; set; }

        /// <summary>Evaluated target framework.</summary>
        public string TargetFramework { get; set; } = string.Empty;

        /// <summary>Evaluated language version.</summary>
        public string LangVersion { get; set; } = string.Empty;

        /// <summary>Declared direct dependencies.</summary>
        public List<string> DeclaredDependencies { get; set; } = new List<string>();

        /// <summary>Number of source files owned by this project.</summary>
        public int OwnedSourceCount { get; set; }
    }

    /// <summary>One inventoried source file.</summary>
    public sealed class SourceInventoryEntry
    {
        /// <summary>Repository-relative source path.</summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>Owning project, or an empty string when unclassified.</summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>Owning project class.</summary>
        public string Class { get; set; } = string.Empty;

        /// <summary>Whether this is production source.</summary>
        public bool Production { get; set; }

        /// <summary>Lowercase hexadecimal SHA-256 of the file bytes.</summary>
        public string Sha256 { get; set; } = string.Empty;

        /// <summary>Whether the file is an evaluated compiler input.</summary>
        public bool CompiledStatically { get; set; }

        /// <summary>Whether the compiler actually consumed the file.</summary>
        public bool CompiledEffectively { get; set; }
    }

    /// <summary>One dependency edge.</summary>
    public sealed class EdgeInventoryEntry
    {
        /// <summary>Dependent project.</summary>
        public string From { get; set; } = string.Empty;

        /// <summary>Dependency.</summary>
        public string To { get; set; } = string.Empty;
    }

    /// <summary>
    /// Mechanically generated, environment-independent inventories.
    /// </summary>
    /// <remarks>
    /// These files are committed and regenerated in CI. Any drift between the
    /// committed inventory and the tree is a CI failure, so the inventory cannot
    /// silently go stale while the claim it supports stays published.
    /// </remarks>
    public sealed class InventoryModel
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private InventoryModel(
            IReadOnlyList<ProjectInventoryEntry> projects,
            IReadOnlyList<SourceInventoryEntry> sources,
            IReadOnlyList<string> unclassifiedSources,
            IReadOnlyList<EdgeInventoryEntry> declaredEdges,
            IReadOnlyList<EdgeInventoryEntry> projectReferenceEdges,
            IReadOnlyList<EdgeInventoryEntry> effectiveAssemblyReferenceEdges)
        {
            Projects = projects;
            Sources = sources;
            UnclassifiedSources = unclassifiedSources;
            DeclaredEdges = declaredEdges;
            ProjectReferenceEdges = projectReferenceEdges;
            EffectiveAssemblyReferenceEdges = effectiveAssemblyReferenceEdges;
        }

        /// <summary>Inventoried projects.</summary>
        public IReadOnlyList<ProjectInventoryEntry> Projects { get; }

        /// <summary>Inventoried sources.</summary>
        public IReadOnlyList<SourceInventoryEntry> Sources { get; }

        /// <summary>Sources owned by no project.</summary>
        public IReadOnlyList<string> UnclassifiedSources { get; }

        /// <summary>Edges declared by the manifest.</summary>
        public IReadOnlyList<EdgeInventoryEntry> DeclaredEdges { get; }

        /// <summary>Edges present in project files.</summary>
        public IReadOnlyList<EdgeInventoryEntry> ProjectReferenceEdges { get; }

        /// <summary>Edges present in built assembly reference tables.</summary>
        public IReadOnlyList<EdgeInventoryEntry> EffectiveAssemblyReferenceEdges { get; }

        /// <summary>Builds the inventory from a completed run.</summary>
        /// <param name="repositoryRoot">Absolute repository root.</param>
        /// <param name="manifest">Loaded manifest.</param>
        /// <param name="facts">Evaluated project facts.</param>
        /// <param name="ownedSources">Owned source paths by project.</param>
        /// <param name="unclassifiedSources">Absolute paths of unowned sources.</param>
        /// <param name="compiledStatically">Absolute paths present in evaluated compiler inputs.</param>
        /// <param name="compiledEffectively">Absolute paths the compiler actually consumed.</param>
        /// <param name="projectReferenceEdges">Edges found in project files.</param>
        /// <param name="effectiveReferences">Kernel assembly references found in built assemblies.</param>
        /// <returns>The inventory model.</returns>
        public static InventoryModel Create(
            string repositoryRoot,
            KernelManifest manifest,
            IReadOnlyDictionary<string, ProjectFacts> facts,
            IReadOnlyDictionary<string, List<string>> ownedSources,
            IReadOnlyCollection<string> unclassifiedSources,
            IReadOnlyCollection<string> compiledStatically,
            IReadOnlyCollection<string> compiledEffectively,
            IReadOnlyDictionary<string, SortedSet<string>> projectReferenceEdges,
            IReadOnlyDictionary<string, SortedSet<string>> effectiveReferences)
        {
            if (manifest is null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            if (facts is null)
            {
                throw new ArgumentNullException(nameof(facts));
            }

            if (ownedSources is null)
            {
                throw new ArgumentNullException(nameof(ownedSources));
            }

            var staticSet = new HashSet<string>(compiledStatically, StringComparer.Ordinal);
            var effectiveSet = new HashSet<string>(compiledEffectively, StringComparer.Ordinal);

            var projectEntries = new List<ProjectInventoryEntry>();
            var sourceEntries = new List<SourceInventoryEntry>();
            var declaredEdgeEntries = new List<EdgeInventoryEntry>();
            var actualEdgeEntries = new List<EdgeInventoryEntry>();
            var effectiveEdgeEntries = new List<EdgeInventoryEntry>();

            var orderedProjects = new List<ManifestProject>(manifest.Projects);
            orderedProjects.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));

            foreach (var project in orderedProjects)
            {
                var policy = manifest.ClassOf(project);
                facts.TryGetValue(project.Name, out var projectFacts);

                var declared = new List<string>(project.DependsOn);
                declared.Sort(StringComparer.Ordinal);

                ownedSources.TryGetValue(project.Name, out var owned);
                owned ??= new List<string>();

                projectEntries.Add(new ProjectInventoryEntry
                {
                    Name = project.Name,
                    Path = project.Path,
                    Class = project.ClassName,
                    Production = policy.Production,
                    TargetFramework = projectFacts?.Property("TargetFramework") ?? string.Empty,
                    LangVersion = projectFacts?.Property("LangVersion") ?? string.Empty,
                    DeclaredDependencies = declared,
                    OwnedSourceCount = owned.Count,
                });

                foreach (var dependency in declared)
                {
                    declaredEdgeEntries.Add(new EdgeInventoryEntry { From = project.Name, To = dependency });
                }

                if (projectReferenceEdges.TryGetValue(project.Name, out var actual))
                {
                    foreach (var edge in actual)
                    {
                        actualEdgeEntries.Add(new EdgeInventoryEntry { From = project.Name, To = edge });
                    }
                }

                if (effectiveReferences.TryGetValue(project.Name, out var effective))
                {
                    foreach (var edge in effective)
                    {
                        effectiveEdgeEntries.Add(new EdgeInventoryEntry { From = project.Name, To = edge });
                    }
                }

                var ownedSorted = new List<string>(owned);
                ownedSorted.Sort(StringComparer.Ordinal);

                foreach (var source in ownedSorted)
                {
                    sourceEntries.Add(new SourceInventoryEntry
                    {
                        Path = RepositoryPaths.ToRelative(repositoryRoot, source),
                        Owner = project.Name,
                        Class = project.ClassName,
                        Production = policy.Production,
                        Sha256 = ToHex(AssemblyFacts.Sha256OfFile(source)),
                        CompiledStatically = staticSet.Contains(source),
                        CompiledEffectively = effectiveSet.Contains(source),
                    });
                }
            }

            sourceEntries.Sort((a, b) => string.CompareOrdinal(a.Path, b.Path));

            var unclassified = new List<string>();
            foreach (var source in unclassifiedSources)
            {
                unclassified.Add(RepositoryPaths.ToRelative(repositoryRoot, source));
            }

            unclassified.Sort(StringComparer.Ordinal);

            return new InventoryModel(
                projectEntries,
                sourceEntries,
                unclassified,
                declaredEdgeEntries,
                actualEdgeEntries,
                effectiveEdgeEntries);
        }

        /// <summary>Writes the inventory files.</summary>
        /// <param name="directory">Absolute output directory.</param>
        public void Write(string directory)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            Directory.CreateDirectory(directory);

            var productionSources = 0;
            foreach (var source in Sources)
            {
                if (source.Production)
                {
                    productionSources++;
                }
            }

            WriteJson(
                Path.Combine(directory, "projects.json"),
                new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    ["schemaVersion"] = 1,
                    ["projectCount"] = Projects.Count,
                    ["projects"] = Projects,
                });

            WriteJson(
                Path.Combine(directory, "sources.json"),
                new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    ["schemaVersion"] = 1,
                    ["sourceCount"] = Sources.Count,
                    ["productionSourceCount"] = productionSources,
                    ["unclassifiedProductionSourceCount"] = UnclassifiedSources.Count,
                    ["unclassifiedSources"] = UnclassifiedSources,
                    ["sources"] = Sources,
                });

            WriteJson(
                Path.Combine(directory, "graph.json"),
                new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    ["schemaVersion"] = 1,
                    ["declaredEdges"] = DeclaredEdges,
                    ["projectReferenceEdges"] = ProjectReferenceEdges,
                    ["effectiveAssemblyReferenceEdges"] = EffectiveAssemblyReferenceEdges,
                });
        }

        private static void WriteJson(string path, object payload)
        {
            var json = JsonSerializer.Serialize(payload, Options);
            File.WriteAllText(path, json + "\n", new UTF8Encoding(false));
        }

        private static string ToHex(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (var value in bytes)
            {
                builder.Append(value.ToString("x2", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }
    }
}
