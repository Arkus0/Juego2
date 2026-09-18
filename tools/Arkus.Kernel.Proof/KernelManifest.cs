using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// Fixed repository scan policy owned by the proof implementation, not by the
    /// manifest being proved.
    /// </summary>
    public sealed class RepositoryScanPolicy
    {
        private static readonly string[] FixedRoots = { "." };
        private static readonly string[] FixedExcludedDirectories = { ".git", "artifacts" };

        /// <summary>The complete repository checkout is the scan root.</summary>
        public IReadOnlyList<string> SourceScanRoots => FixedRoots;

        /// <summary>
        /// Direct children of the scan root that are proof-internal/generated and
        /// therefore not repository product input.
        /// </summary>
        public IReadOnlyList<string> ExcludedDirectoryNames => FixedExcludedDirectories;
    }

    /// <summary>Declared SDK pin.</summary>
    public sealed class SdkPinPolicy
    {
        /// <summary>Pinned SDK version.</summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>Pinned roll-forward policy.</summary>
        public string RollForward { get; set; } = string.Empty;

        /// <summary>Whether prerelease SDKs are allowed.</summary>
        public bool AllowPrerelease { get; set; }
    }

    /// <summary>Toolchain policy.</summary>
    public sealed class ToolchainPolicy
    {
        /// <summary>Path of the SDK pin file, relative to the repository root.</summary>
        public string GlobalJsonPath { get; set; } = string.Empty;

        /// <summary>Declared SDK pin that <c>global.json</c> must match exactly.</summary>
        public SdkPinPolicy Sdk { get; set; } = new SdkPinPolicy();

        /// <summary>Warning identifiers that may appear in an evaluated <c>NoWarn</c>.</summary>
        public List<string> AllowedNoWarn { get; } = new List<string>();

        /// <summary>Whether every project must use central package version management.</summary>
        public bool RequireCentralPackageManagement { get; set; }
    }

    /// <summary>Build contract for one class of projects.</summary>
    public sealed class ProjectClassPolicy
    {
        /// <summary>Why this class exists.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Whether sources owned by this class are production sources.</summary>
        public bool Production { get; set; }

        /// <summary>Whether engine/game-content dependencies are forbidden.</summary>
        public bool EngineFree { get; set; }

        /// <summary>Whether NuGet dependencies are allowed.</summary>
        public bool AllowPackageReferences { get; set; }

        /// <summary>Whether every declared dependency must be effectively referenced by the built assembly.</summary>
        public bool RequireDeclaredEdgesExercised { get; set; }

        /// <summary>Whether the compiler may consume source injected from outside the repository.</summary>
        public bool AllowExternalCompiledSources { get; set; }

        /// <summary>Evaluated MSBuild properties every project of this class must have.</summary>
        public Dictionary<string, string> RequiredProperties { get; } =
            new Dictionary<string, string>(StringComparer.Ordinal);
    }

    /// <summary>One classified project.</summary>
    public sealed class ManifestProject
    {
        /// <summary>Project name (also the expected assembly name).</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Project file path relative to the repository root.</summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>Project class key.</summary>
        [JsonPropertyName("class")]
        public string ClassName { get; set; } = string.Empty;

        /// <summary>Declared direct dependencies, by project name.</summary>
        public List<string> DependsOn { get; } = new List<string>();
    }

    /// <summary>
    /// Declarative source of truth for canonical kernel policy and classification.
    /// </summary>
    /// <remarks>
    /// Repository completeness is intentionally not configurable here. The proof
    /// owns its repository boundary independently so the object being proved
    /// cannot shrink the universe that the proof inspects.
    /// </remarks>
    public sealed class KernelManifest
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,

            // Collection properties are intentionally get-only so no caller can
            // swap the backing instance; populating them keeps the manifest model
            // immutable in shape while still fully deserializable.
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
        };

        /// <summary>Manifest schema version.</summary>
        public int SchemaVersion { get; set; }

        /// <summary>Owning workpack.</summary>
        public string Workpack { get; set; } = string.Empty;

        /// <summary>Free-text intent.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Proof-owned repository boundary. It is deliberately ignored by JSON
        /// and therefore cannot be controlled by kernel-manifest.json.
        /// </summary>
        [JsonIgnore]
        public RepositoryScanPolicy Repository { get; } = new RepositoryScanPolicy();

        /// <summary>
        /// Rejects the legacy manifest-owned repository scan policy if anybody
        /// attempts to reintroduce it.
        /// </summary>
        [JsonPropertyName("repository")]
        public JsonElement RepositoryPolicyMustNotBeDeclared
        {
            get => default;
            set => throw new JsonException(
                "'repository' scan policy is not configurable; WP-HK-00 proof completeness owns the repository boundary independently.");
        }

        /// <summary>Toolchain policy.</summary>
        public ToolchainPolicy Toolchain { get; set; } = new ToolchainPolicy();

        /// <summary>Project class contracts.</summary>
        public Dictionary<string, ProjectClassPolicy> ProjectClasses { get; } =
            new Dictionary<string, ProjectClassPolicy>(StringComparer.Ordinal);

        /// <summary>Classified projects.</summary>
        public List<ManifestProject> Projects { get; } = new List<ManifestProject>();

        /// <summary>Assembly name patterns that may never appear in the kernel.</summary>
        public List<string> ForbiddenAssemblyNamePatterns { get; } = new List<string>();

        /// <summary>File-name patterns that build-generated sources are allowed to match.</summary>
        public List<string> GeneratedSourceNamePatterns { get; } = new List<string>();

        /// <summary>Files that must exist for the proof to be considered runnable.</summary>
        public List<string> RequiredFiles { get; } = new List<string>();

        /// <summary>Test projects that must exist.</summary>
        public List<string> RequiredTestProjects { get; } = new List<string>();

        /// <summary>Project name of the proof tool itself.</summary>
        public string RequiredProofTool { get; set; } = string.Empty;

        /// <summary>Loads and structurally validates the manifest.</summary>
        /// <param name="path">Absolute manifest path.</param>
        /// <returns>The loaded manifest.</returns>
        public static KernelManifest Load(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new ProofToolException($"Kernel manifest not found: {path}");
            }

            KernelManifest? manifest;
            try
            {
                manifest = JsonSerializer.Deserialize<KernelManifest>(File.ReadAllText(path), Options);
            }
            catch (JsonException ex)
            {
                throw new ProofToolException($"Kernel manifest is not valid for the closed schema: {path}", ex);
            }

            if (manifest is null)
            {
                throw new ProofToolException($"Kernel manifest deserialized to null: {path}");
            }

            manifest.Validate(path);
            return manifest;
        }

        /// <summary>Finds a project by name.</summary>
        /// <param name="name">Project name.</param>
        /// <returns>The project, or <c>null</c>.</returns>
        public ManifestProject? FindProject(string name)
        {
            foreach (var project in Projects)
            {
                if (string.Equals(project.Name, name, StringComparison.Ordinal))
                {
                    return project;
                }
            }

            return null;
        }

        /// <summary>Resolves the class policy of a project.</summary>
        /// <param name="project">Classified project.</param>
        /// <returns>The class policy.</returns>
        public ProjectClassPolicy ClassOf(ManifestProject project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (!ProjectClasses.TryGetValue(project.ClassName, out var policy))
            {
                throw new ProofToolException(
                    $"Project '{project.Name}' declares unknown class '{project.ClassName}'.");
            }

            return policy;
        }

        private void Validate(string path)
        {
            if (SchemaVersion != 1)
            {
                throw new ProofToolException($"Unsupported manifest schemaVersion {SchemaVersion} in {path}.");
            }

            if (Projects.Count == 0)
            {
                throw new ProofToolException($"Manifest declares no projects: {path}");
            }

            if (ProjectClasses.Count == 0)
            {
                throw new ProofToolException($"Manifest declares no project classes: {path}");
            }

            if (ForbiddenAssemblyNamePatterns.Count == 0)
            {
                throw new ProofToolException($"Manifest declares no forbidden assembly patterns: {path}");
            }

            if (GeneratedSourceNamePatterns.Count == 0)
            {
                throw new ProofToolException($"Manifest declares no generated-source patterns: {path}");
            }

            if (string.IsNullOrEmpty(RequiredProofTool))
            {
                throw new ProofToolException($"Manifest declares no proof tool project: {path}");
            }

            if (RequiredTestProjects.Count == 0)
            {
                throw new ProofToolException($"Manifest declares no required test projects: {path}");
            }

            if (string.IsNullOrEmpty(Toolchain.GlobalJsonPath) || string.IsNullOrEmpty(Toolchain.Sdk.Version))
            {
                throw new ProofToolException($"Manifest declares no SDK pin: {path}");
            }

            var seenNames = new HashSet<string>(StringComparer.Ordinal);
            var seenPaths = new HashSet<string>(StringComparer.Ordinal);
            foreach (var project in Projects)
            {
                if (string.IsNullOrEmpty(project.Name) || string.IsNullOrEmpty(project.Path))
                {
                    throw new ProofToolException($"Manifest contains a project without name or path: {path}");
                }

                if (!seenNames.Add(project.Name))
                {
                    throw new ProofToolException($"Manifest declares project '{project.Name}' twice: {path}");
                }

                if (!seenPaths.Add(project.Path))
                {
                    throw new ProofToolException($"Manifest classifies project path '{project.Path}' twice: {path}");
                }
            }
        }
    }
}
