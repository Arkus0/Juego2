using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// Runs every mechanical check that defends the WP-HK-00 boundary claim.
    /// </summary>
    /// <remarks>
    /// The run is split into three phases. <c>preflight</c> proves the proof
    /// machinery itself is present, so a deleted project or tool can never look
    /// like a pass. <c>static</c> checks evaluated MSBuild facts. <c>effective</c>
    /// checks the produced binaries and portable PDBs, which is what the compiler
    /// actually did; the static phase is defence in depth around it.
    /// </remarks>
    public sealed class KernelProofRunner
    {
        private const string PhasePreflight = "preflight";
        private const string PhaseStatic = "static";
        private const string PhaseEffective = "effective";
        private const string PhaseCompiler = "compiler";

        private readonly string repositoryRoot;
        private readonly string configuration;
        private readonly List<Finding> findings = new List<Finding>();
        private readonly Dictionary<string, ProjectFacts> facts =
            new Dictionary<string, ProjectFacts>(StringComparer.Ordinal);
        private readonly Dictionary<string, AssemblyFacts> assemblies =
            new Dictionary<string, AssemblyFacts>(StringComparer.Ordinal);
        private readonly Dictionary<string, List<string>> ownedSources =
            new Dictionary<string, List<string>>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> projectDirectories =
            new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly SortedSet<string> unclassifiedSources = new SortedSet<string>(StringComparer.Ordinal);
        private readonly SortedSet<string> compiledStatically = new SortedSet<string>(StringComparer.Ordinal);
        private readonly SortedSet<string> compiledEffectively = new SortedSet<string>(StringComparer.Ordinal);
        private readonly SortedDictionary<string, SortedSet<string>> actualEdges =
            new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
        private readonly SortedDictionary<string, SortedSet<string>> effectiveKernelReferences =
            new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);

        private KernelManifest manifest = new KernelManifest();
        private NamePatternMatcher forbiddenNames = new NamePatternMatcher(Array.Empty<string>());
        private NamePatternMatcher generatedSourceNames = new NamePatternMatcher(Array.Empty<string>());
        private MsBuildEvaluator evaluator = new MsBuildEvaluator("Debug", Array.Empty<string>());
        private string runningSdkVersion = string.Empty;

        /// <summary>Creates a runner.</summary>
        /// <param name="repositoryRoot">Absolute repository root.</param>
        /// <param name="configuration">Build configuration to evaluate and inspect.</param>
        public KernelProofRunner(string repositoryRoot, string configuration)
        {
            this.repositoryRoot = Path.GetFullPath(
                repositoryRoot ?? throw new ArgumentNullException(nameof(repositoryRoot)));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>Loaded manifest, available after <see cref="Run"/>.</summary>
        public KernelManifest Manifest => manifest;

        /// <summary>Runs the requested phases.</summary>
        /// <param name="runStatic">Whether to run the static phase.</param>
        /// <param name="runEffective">Whether to run the effective phase.</param>
        /// <param name="runCompiler">Whether to run the compiler command line phase.</param>
        /// <returns>The report.</returns>
        public ProofReport Run(bool runStatic, bool runEffective, bool runCompiler)
        {
            var phases = new List<string> { PhasePreflight };

            manifest = KernelManifest.Load(Path.Combine(repositoryRoot, "kernel-manifest.json"));
            forbiddenNames = new NamePatternMatcher(manifest.ForbiddenAssemblyNamePatterns);
            generatedSourceNames = new NamePatternMatcher(manifest.GeneratedSourceNamePatterns);

            RunPreflight();

            if (runStatic || runEffective || runCompiler)
            {
                phases.Add(PhaseStatic);
                runningSdkVersion = MsBuildEvaluator.RunningSdkVersion(repositoryRoot);
                CheckSdkPin();
                DiscoverProjects();
                EvaluateProjects();
                CheckToolchainProperties();
                CheckPackages();
                CheckStaticEngineDependencies();
                CheckDependencyGraph();
                CheckSourceOwnership();
            }

            if (runEffective)
            {
                phases.Add(PhaseEffective);
                CheckEffectiveAssemblies();
            }

            if (runCompiler)
            {
                phases.Add(PhaseCompiler);
                CheckCompilerCommandLines();
            }

            var environmentInfo = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["repositoryRoot"] = repositoryRoot,
                ["configuration"] = configuration,
                ["runningSdkVersion"] = runningSdkVersion,
                ["manifestWorkpack"] = manifest.Workpack,
            };

            return new ProofReport(phases, findings, environmentInfo);
        }

        /// <summary>Builds the deterministic inventory model for a completed full run.</summary>
        /// <returns>The inventory model.</returns>
        public InventoryModel BuildInventory()
        {
            if (facts.Count == 0)
            {
                throw new ProofToolException("Inventory requested before the static phase ran.");
            }

            return InventoryModel.Create(
                repositoryRoot,
                manifest,
                facts,
                ownedSources,
                unclassifiedSources,
                compiledStatically,
                compiledEffectively,
                actualEdges,
                effectiveKernelReferences);
        }

        private void Report(string checkId, string phase, string subject, string message)
        {
            findings.Add(new Finding(checkId, phase, subject, message));
        }

        private string Relative(string absolutePath)
        {
            return RepositoryPaths.ToRelative(repositoryRoot, absolutePath);
        }

        private void RunPreflight()
        {
            foreach (var required in manifest.RequiredFiles)
            {
                if (!File.Exists(Path.Combine(repositoryRoot, required)))
                {
                    Report(
                        CheckIds.PreflightFileMissing,
                        PhasePreflight,
                        required,
                        "A file required by the kernel manifest is missing; the proof cannot be trusted.");
                }
            }

            foreach (var project in manifest.Projects)
            {
                if (!File.Exists(Path.Combine(repositoryRoot, project.Path)))
                {
                    Report(
                        CheckIds.PreflightProjectMissing,
                        PhasePreflight,
                        project.Path,
                        $"Manifest declares project '{project.Name}' but the project file does not exist.");
                }
            }

            foreach (var testProject in manifest.RequiredTestProjects)
            {
                var declared = manifest.FindProject(testProject);
                if (declared is null || !File.Exists(Path.Combine(repositoryRoot, declared.Path)))
                {
                    Report(
                        CheckIds.PreflightTestProjectMissing,
                        PhasePreflight,
                        testProject,
                        "A required test project is missing or unclassified; CI must fail closed.");
                }
            }

            var proofTool = manifest.FindProject(manifest.RequiredProofTool);
            if (proofTool is null || !File.Exists(Path.Combine(repositoryRoot, proofTool.Path)))
            {
                Report(
                    CheckIds.PreflightProofToolMissing,
                    PhasePreflight,
                    manifest.RequiredProofTool,
                    "The proof tool project is missing or unclassified; CI must fail closed.");
            }

            CheckSolutionMembership();
        }

        private void CheckSolutionMembership()
        {
            var solutionPath = Path.Combine(repositoryRoot, "Juego2.sln");
            if (!File.Exists(solutionPath))
            {
                return;
            }

            var solutionProjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in File.ReadAllLines(solutionPath))
            {
                if (!line.StartsWith("Project(", StringComparison.Ordinal))
                {
                    continue;
                }

                var parts = line.Split('"');
                foreach (var part in parts)
                {
                    if (part.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                    {
                        solutionProjects.Add(part.Replace('\\', '/'));
                    }
                }
            }

            foreach (var project in manifest.Projects)
            {
                if (!solutionProjects.Contains(project.Path))
                {
                    Report(
                        CheckIds.PreflightSolutionProjectMissing,
                        PhasePreflight,
                        project.Path,
                        $"Project '{project.Name}' is not part of Juego2.sln, so a solution build would silently skip it.");
                }
            }
        }

        private void CheckSdkPin()
        {
            var pinPath = Path.Combine(repositoryRoot, manifest.Toolchain.GlobalJsonPath);
            var pin = GlobalJsonPin.Load(pinPath);
            var declared = manifest.Toolchain.Sdk;

            if (!string.Equals(pin.Version, declared.Version, StringComparison.Ordinal)
                || !string.Equals(pin.RollForward, declared.RollForward, StringComparison.Ordinal)
                || pin.AllowPrerelease != declared.AllowPrerelease)
            {
                Report(
                    CheckIds.ToolchainSdkPin,
                    PhaseStatic,
                    manifest.Toolchain.GlobalJsonPath,
                    $"SDK pin '{pin.Version}/{pin.RollForward}/allowPrerelease={pin.AllowPrerelease}' does not match the manifest pin "
                    + $"'{declared.Version}/{declared.RollForward}/allowPrerelease={declared.AllowPrerelease}'.");
            }

            if (!SdkPinRules.IsNarrowEnough(pin.RollForward))
            {
                Report(
                    CheckIds.ToolchainSdkPin,
                    PhaseStatic,
                    manifest.Toolchain.GlobalJsonPath,
                    $"Roll-forward policy '{pin.RollForward}' does not keep the SDK feature band pinned.");
            }

            if (!SdkPinRules.IsSatisfiedBy(pin, SdkVersion.Parse(runningSdkVersion), out var reason))
            {
                Report(
                    CheckIds.ToolchainSdkPin,
                    PhaseStatic,
                    manifest.Toolchain.GlobalJsonPath,
                    $"Toolchain drift: {reason}.");
            }
        }

        private void DiscoverProjects()
        {
            var declaredPaths = new HashSet<string>(StringComparer.Ordinal);
            foreach (var project in manifest.Projects)
            {
                declaredPaths.Add(project.Path);
            }

            foreach (var root in manifest.Repository.SourceScanRoots)
            {
                var absoluteRoot = Path.Combine(repositoryRoot, root);
                foreach (var found in RepositoryPaths.EnumerateFiles(
                             absoluteRoot,
                             "*.csproj",
                             manifest.Repository.ExcludedDirectoryNames))
                {
                    var relative = Relative(found);
                    if (!declaredPaths.Contains(relative))
                    {
                        Report(
                            CheckIds.ManifestProjectUndeclared,
                            PhaseStatic,
                            relative,
                            "A project exists on disk but is not classified in kernel-manifest.json.");
                    }
                }
            }
        }

        private void EvaluateProjects()
        {
            var requiredPropertyNames = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var policy in manifest.ProjectClasses.Values)
            {
                foreach (var propertyName in policy.RequiredProperties.Keys)
                {
                    requiredPropertyNames.Add(propertyName);
                }
            }

            evaluator = new MsBuildEvaluator(configuration, requiredPropertyNames);

            foreach (var project in manifest.Projects)
            {
                if (!manifest.ProjectClasses.ContainsKey(project.ClassName))
                {
                    Report(
                        CheckIds.ManifestClassUnknown,
                        PhaseStatic,
                        project.Name,
                        $"Project class '{project.ClassName}' is not defined by the manifest.");
                    continue;
                }

                var absolute = Path.Combine(repositoryRoot, project.Path);
                if (!File.Exists(absolute))
                {
                    continue;
                }

                projectDirectories[project.Name] = Path.GetDirectoryName(absolute)!;
                facts[project.Name] = evaluator.Evaluate(project.Name, absolute);
            }
        }

        private void CheckToolchainProperties()
        {
            foreach (var project in manifest.Projects)
            {
                if (!facts.TryGetValue(project.Name, out var projectFacts))
                {
                    continue;
                }

                var policy = manifest.ClassOf(project);

                foreach (var required in policy.RequiredProperties)
                {
                    var actual = projectFacts.Property(required.Key);
                    if (!string.Equals(actual, required.Value, StringComparison.Ordinal))
                    {
                        Report(
                            CheckIds.ToolchainProperty,
                            PhaseStatic,
                            $"{project.Name}:{required.Key}",
                            $"Evaluated '{required.Key}' is '{actual}' but class '{project.ClassName}' requires '{required.Value}'.");
                    }
                }

                if (!string.Equals(projectFacts.Property("AssemblyName"), project.Name, StringComparison.Ordinal))
                {
                    Report(
                        CheckIds.ToolchainProperty,
                        PhaseStatic,
                        $"{project.Name}:AssemblyName",
                        $"Evaluated AssemblyName is '{projectFacts.Property("AssemblyName")}'; ownership claims assume it equals the project name.");
                }

                foreach (var token in SplitList(projectFacts.Property("NoWarn")))
                {
                    if (!manifest.Toolchain.AllowedNoWarn.Contains(token))
                    {
                        Report(
                            CheckIds.ToolchainSuppression,
                            PhaseStatic,
                            $"{project.Name}:NoWarn",
                            $"Warning '{token}' is suppressed but is not in the manifest allow-list.");
                    }
                }

                foreach (var token in SplitList(projectFacts.Property("WarningsNotAsErrors")))
                {
                    Report(
                        CheckIds.ToolchainSuppression,
                        PhaseStatic,
                        $"{project.Name}:WarningsNotAsErrors",
                        $"Warning '{token}' is demoted from error; warnings-as-errors must hold for every kernel project.");
                }

                if (manifest.Toolchain.RequireCentralPackageManagement
                    && !string.Equals(
                        projectFacts.Property("ManagePackageVersionsCentrally"),
                        "true",
                        StringComparison.Ordinal))
                {
                    Report(
                        CheckIds.ToolchainProperty,
                        PhaseStatic,
                        $"{project.Name}:ManagePackageVersionsCentrally",
                        "Central package version management is required so no project can float a dependency version.");
                }
            }
        }

        private void CheckPackages()
        {
            foreach (var project in manifest.Projects)
            {
                if (!facts.TryGetValue(project.Name, out var projectFacts))
                {
                    continue;
                }

                var policy = manifest.ClassOf(project);

                foreach (var package in projectFacts.PackageReferences)
                {
                    if (!policy.AllowPackageReferences)
                    {
                        Report(
                            CheckIds.PackageForbidden,
                            PhaseStatic,
                            $"{project.Name}:{package.Id}",
                            $"Class '{project.ClassName}' must have no NuGet dependencies, but references '{package.Id}'.");
                    }

                    if (package.Version.Length > 0)
                    {
                        Report(
                            CheckIds.PackageUnpinned,
                            PhaseStatic,
                            $"{project.Name}:{package.Id}",
                            $"Package '{package.Id}' declares an inline version '{package.Version}' instead of a central pin.");
                    }

                    if (forbiddenNames.IsMatch(package.Id))
                    {
                        Report(
                            CheckIds.EngineDependencyStatic,
                            PhaseStatic,
                            $"{project.Name}:{package.Id}",
                            $"Package '{package.Id}' matches a forbidden engine/game-content pattern.");
                    }
                }
            }
        }

        private void CheckStaticEngineDependencies()
        {
            foreach (var project in manifest.Projects)
            {
                if (!facts.TryGetValue(project.Name, out var projectFacts))
                {
                    continue;
                }

                var policy = manifest.ClassOf(project);

                foreach (var reference in projectFacts.Item("Reference"))
                {
                    var simpleName = reference.Split(',')[0].Trim();

                    if (forbiddenNames.IsMatch(simpleName))
                    {
                        Report(
                            CheckIds.EngineDependencyStatic,
                            PhaseStatic,
                            $"{project.Name}:{simpleName}",
                            $"Assembly reference '{simpleName}' matches a forbidden engine/game-content pattern.");
                        continue;
                    }

                    if (policy.Production)
                    {
                        Report(
                            CheckIds.ReferenceRawAssembly,
                            PhaseStatic,
                            $"{project.Name}:{simpleName}",
                            "Production projects must not use raw assembly references; dependencies come from classified projects only.");
                    }
                }

                foreach (var reference in projectFacts.Item("ProjectReference"))
                {
                    var name = Path.GetFileNameWithoutExtension(reference);
                    if (forbiddenNames.IsMatch(name))
                    {
                        Report(
                            CheckIds.EngineDependencyStatic,
                            PhaseStatic,
                            $"{project.Name}:{name}",
                            $"Project reference '{name}' matches a forbidden engine/game-content pattern.");
                    }
                }
            }
        }

        private void CheckDependencyGraph()
        {
            var names = new List<string>();
            foreach (var project in manifest.Projects)
            {
                names.Add(project.Name);
            }

            var declaredGraph = new DependencyGraph(names);
            var actualGraph = new DependencyGraph(names);

            foreach (var project in manifest.Projects)
            {
                foreach (var dependency in project.DependsOn)
                {
                    if (manifest.FindProject(dependency) is null)
                    {
                        Report(
                            CheckIds.ManifestDependencyUnknown,
                            PhaseStatic,
                            $"{project.Name} -> {dependency}",
                            "Declared dependency names a project the manifest does not define.");
                        continue;
                    }

                    declaredGraph.AddEdge(project.Name, dependency);
                }
            }

            foreach (var project in manifest.Projects)
            {
                var actual = new SortedSet<string>(StringComparer.Ordinal);
                actualEdges[project.Name] = actual;

                if (!facts.TryGetValue(project.Name, out var projectFacts))
                {
                    continue;
                }

                foreach (var reference in projectFacts.Item("ProjectReference"))
                {
                    var relative = Relative(reference);
                    var target = FindProjectByPath(relative);

                    if (target is null)
                    {
                        Report(
                            CheckIds.GraphUnknownTarget,
                            PhaseStatic,
                            $"{project.Name} -> {relative}",
                            "Project reference points at a project that is not classified in the manifest.");
                        continue;
                    }

                    actual.Add(target.Name);
                    actualGraph.AddEdge(project.Name, target.Name);
                }

                var declared = new SortedSet<string>(project.DependsOn, StringComparer.Ordinal);

                foreach (var edge in actual)
                {
                    if (!declared.Contains(edge))
                    {
                        Report(
                            CheckIds.GraphUndeclaredEdge,
                            PhaseStatic,
                            $"{project.Name} -> {edge}",
                            "Project file declares a dependency edge that the manifest does not allow.");
                    }
                }

                foreach (var edge in declared)
                {
                    if (!actual.Contains(edge))
                    {
                        Report(
                            CheckIds.GraphMissingEdge,
                            PhaseStatic,
                            $"{project.Name} -> {edge}",
                            "Manifest declares a dependency edge that the project file does not contain.");
                    }
                }
            }

            ReportCycles(declaredGraph, "manifest");
            ReportCycles(actualGraph, "project files");
        }

        private void ReportCycles(DependencyGraph graph, string origin)
        {
            foreach (var cycle in graph.FindCycles())
            {
                Report(
                    CheckIds.GraphCycle,
                    PhaseStatic,
                    string.Join(" -> ", cycle),
                    $"The dependency graph derived from {origin} contains a cycle or back-edge.");
            }
        }

        private ManifestProject? FindProjectByPath(string relativePath)
        {
            foreach (var project in manifest.Projects)
            {
                if (string.Equals(project.Path, relativePath, StringComparison.Ordinal))
                {
                    return project;
                }
            }

            return null;
        }

        private void CheckSourceOwnership()
        {
            var ownerByDirectory = new List<KeyValuePair<string, string>>();
            foreach (var pair in projectDirectories)
            {
                ownerByDirectory.Add(new KeyValuePair<string, string>(pair.Value, pair.Key));
            }

            ownerByDirectory.Sort((a, b) => b.Key.Length.CompareTo(a.Key.Length));

            foreach (var project in manifest.Projects)
            {
                ownedSources[project.Name] = new List<string>();
            }

            foreach (var root in manifest.Repository.SourceScanRoots)
            {
                foreach (var source in RepositoryPaths.EnumerateFiles(
                             Path.Combine(repositoryRoot, root),
                             "*.cs",
                             manifest.Repository.ExcludedDirectoryNames))
                {
                    var owner = FindOwner(ownerByDirectory, source);
                    if (owner is null)
                    {
                        unclassifiedSources.Add(source);
                        Report(
                            CheckIds.SourceUnclassified,
                            PhaseStatic,
                            Relative(source),
                            "Source file is inside a scanned root but owned by no classified project.");
                        continue;
                    }

                    ownedSources[owner].Add(source);
                }
            }

            var compileItemOwners = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);

            foreach (var project in manifest.Projects)
            {
                if (!facts.TryGetValue(project.Name, out var projectFacts))
                {
                    continue;
                }

                var projectDirectory = projectDirectories[project.Name];
                var compiled = new SortedSet<string>(StringComparer.Ordinal);

                foreach (var item in projectFacts.Item("Compile"))
                {
                    if (item.Length == 0)
                    {
                        continue;
                    }

                    var full = Path.GetFullPath(item);
                    compiled.Add(full);
                    compiledStatically.Add(full);

                    if (!compileItemOwners.TryGetValue(full, out var owners))
                    {
                        owners = new SortedSet<string>(StringComparer.Ordinal);
                        compileItemOwners[full] = owners;
                    }

                    owners.Add(project.Name);

                    if (!RepositoryPaths.IsInside(projectDirectory, full))
                    {
                        Report(
                            CheckIds.SourceForeignCompileItem,
                            PhaseStatic,
                            $"{project.Name}:{Relative(full)}",
                            "Compiler input lies outside the project directory, so the source has more than one owner.");
                    }
                }

                foreach (var owned in ownedSources[project.Name])
                {
                    if (!compiled.Contains(owned))
                    {
                        Report(
                            CheckIds.SourceNotCompiledStatic,
                            PhaseStatic,
                            $"{project.Name}:{Relative(owned)}",
                            "Owned source file is not among the evaluated compiler inputs.");
                    }
                }
            }

            foreach (var pair in compileItemOwners)
            {
                if (pair.Value.Count > 1)
                {
                    Report(
                        CheckIds.SourceDuplicateOwnership,
                        PhaseStatic,
                        Relative(pair.Key),
                        $"Source file is compiled by more than one project: {string.Join(", ", pair.Value)}.");
                }
            }
        }

        private static string? FindOwner(List<KeyValuePair<string, string>> ownerByDirectory, string source)
        {
            foreach (var pair in ownerByDirectory)
            {
                if (RepositoryPaths.IsInside(pair.Key, source))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        private void CheckEffectiveAssemblies()
        {
            var artifactsRoot = Path.Combine(repositoryRoot, "artifacts");
            var effectiveDocumentOwners = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);

            foreach (var project in manifest.Projects)
            {
                if (!facts.TryGetValue(project.Name, out var projectFacts))
                {
                    continue;
                }

                var policy = manifest.ClassOf(project);
                var targetPath = projectFacts.Property("TargetPath");

                if (targetPath.Length == 0 || !File.Exists(targetPath))
                {
                    Report(
                        CheckIds.OutputMissing,
                        PhaseEffective,
                        project.Name,
                        $"Build output '{targetPath}' is missing; the effective oracles cannot run, so the claim is unproven.");
                    continue;
                }

                var assemblyFacts = AssemblyFacts.Read(targetPath);
                assemblies[project.Name] = assemblyFacts;

                CheckEffectiveTargetFramework(project, policy, assemblyFacts);
                CheckEffectiveReferences(project, policy, assemblyFacts);
                CheckEffectiveDocuments(project, policy, assemblyFacts, artifactsRoot, effectiveDocumentOwners);
            }

            foreach (var pair in effectiveDocumentOwners)
            {
                if (pair.Value.Count > 1)
                {
                    Report(
                        CheckIds.SourceDuplicateOwnership,
                        PhaseEffective,
                        Relative(pair.Key),
                        $"The compiler consumed this file into more than one assembly: {string.Join(", ", pair.Value)}.");
                }
            }
        }

        private void CheckEffectiveTargetFramework(
            ManifestProject project,
            ProjectClassPolicy policy,
            AssemblyFacts assemblyFacts)
        {
            if (!policy.RequiredProperties.TryGetValue("TargetFramework", out var expectedShort))
            {
                return;
            }

            var expected = FrameworkMoniker(expectedShort);
            if (!string.Equals(assemblyFacts.TargetFramework, expected, StringComparison.Ordinal))
            {
                Report(
                    CheckIds.ToolchainEffectiveTfm,
                    PhaseEffective,
                    project.Name,
                    $"Built assembly targets '{assemblyFacts.TargetFramework}' but class '{project.ClassName}' requires '{expected}'.");
            }

            if (!string.Equals(assemblyFacts.AssemblyName, project.Name, StringComparison.Ordinal))
            {
                Report(
                    CheckIds.ToolchainProperty,
                    PhaseEffective,
                    project.Name,
                    $"Built assembly is named '{assemblyFacts.AssemblyName}' instead of '{project.Name}'.");
            }
        }

        private void CheckEffectiveReferences(
            ManifestProject project,
            ProjectClassPolicy policy,
            AssemblyFacts assemblyFacts)
        {
            var declared = new SortedSet<string>(project.DependsOn, StringComparer.Ordinal);
            var used = new SortedSet<string>(StringComparer.Ordinal);

            foreach (var reference in assemblyFacts.AssemblyReferences)
            {
                if (policy.EngineFree && forbiddenNames.IsMatch(reference))
                {
                    Report(
                        CheckIds.EngineDependencyEffective,
                        PhaseEffective,
                        $"{project.Name}:{reference}",
                        $"The built assembly actually references forbidden engine/game-content assembly '{reference}'.");
                    continue;
                }

                if (manifest.FindProject(reference) is null)
                {
                    continue;
                }

                used.Add(reference);

                if (!declared.Contains(reference))
                {
                    Report(
                        CheckIds.ReferenceUndeclaredEffective,
                        PhaseEffective,
                        $"{project.Name} -> {reference}",
                        "The built assembly references a kernel assembly that is not a declared dependency.");
                }
            }

            effectiveKernelReferences[project.Name] = used;

            if (!policy.RequireDeclaredEdgesExercised)
            {
                return;
            }

            foreach (var dependency in declared)
            {
                if (!used.Contains(dependency))
                {
                    Report(
                        CheckIds.ReferenceUnexercised,
                        PhaseEffective,
                        $"{project.Name} -> {dependency}",
                        "A declared dependency is never actually used, so the declared direction is not evidence of anything.");
                }
            }
        }

        private void CheckEffectiveDocuments(
            ManifestProject project,
            ProjectClassPolicy policy,
            AssemblyFacts assemblyFacts,
            string artifactsRoot,
            SortedDictionary<string, SortedSet<string>> effectiveDocumentOwners)
        {
            var projectDirectory = projectDirectories[project.Name];
            var consumed = new SortedSet<string>(StringComparer.Ordinal);

            foreach (var document in assemblyFacts.CompiledDocuments)
            {
                var full = Path.GetFullPath(document.Path);

                if (RepositoryPaths.IsInside(artifactsRoot, full))
                {
                    CheckGeneratedSource(project.Name, full, PhaseEffective);
                    continue;
                }

                if (!RepositoryPaths.IsInside(repositoryRoot, full))
                {
                    // Source injected from outside the repository, such as the test
                    // host entry point that Microsoft.NET.Test.Sdk contributes. It is
                    // never acceptable in a project class that owns product code, and
                    // it is deliberately left out of the inventory because its path is
                    // machine-specific.
                    if (!policy.AllowExternalCompiledSources)
                    {
                        Report(
                            CheckIds.SourceForeignCompiledEffective,
                            PhaseEffective,
                            $"{project.Name}:{full}",
                            "The compiler consumed a source file from outside the repository into a project class that owns product code.");
                    }

                    continue;
                }

                consumed.Add(full);
                compiledEffectively.Add(full);

                if (!effectiveDocumentOwners.TryGetValue(full, out var owners))
                {
                    owners = new SortedSet<string>(StringComparer.Ordinal);
                    effectiveDocumentOwners[full] = owners;
                }

                owners.Add(project.Name);

                if (!RepositoryPaths.IsInside(projectDirectory, full))
                {
                    Report(
                        CheckIds.SourceForeignCompiledEffective,
                        PhaseEffective,
                        $"{project.Name}:{Relative(full)}",
                        "The compiler consumed a source file that this project does not own.");
                    continue;
                }

                if (!File.Exists(full))
                {
                    Report(
                        CheckIds.SourceContentMismatch,
                        PhaseEffective,
                        $"{project.Name}:{Relative(full)}",
                        "The compiler consumed a source file that no longer exists on disk.");
                    continue;
                }

                if (document.HashAlgorithm != AssemblyFacts.Sha256Algorithm)
                {
                    Report(
                        CheckIds.SourceContentMismatch,
                        PhaseEffective,
                        $"{project.Name}:{Relative(full)}",
                        FormattableString.Invariant($"Source checksum algorithm {document.HashAlgorithm} is not SHA-256, so compiled content cannot be verified."));
                    continue;
                }

                if (!AssemblyFacts.HashEquals(document.Hash, AssemblyFacts.Sha256OfFile(full)))
                {
                    Report(
                        CheckIds.SourceContentMismatch,
                        PhaseEffective,
                        $"{project.Name}:{Relative(full)}",
                        "The compiled content of this file differs from the owned file on disk.");
                }
            }

            foreach (var owned in ownedSources[project.Name])
            {
                if (!consumed.Contains(owned))
                {
                    Report(
                        CheckIds.SourceNotCompiledEffective,
                        PhaseEffective,
                        $"{project.Name}:{Relative(owned)}",
                        "Owned source file was never consumed by the compiler that produced this assembly.");
                }
            }
        }

        private void CheckGeneratedSource(string projectName, string fullPath, string phase)
        {
            if (!generatedSourceNames.IsMatch(Path.GetFileName(fullPath)))
            {
                Report(
                    CheckIds.SourceUnexpectedGenerated,
                    phase,
                    $"{projectName}:{Relative(fullPath)}",
                    "A build-generated source file matches no expected generated-source pattern, so product code could be hiding in it.");
            }
        }

        private void CheckCompilerCommandLines()
        {
            var artifactsRoot = Path.Combine(repositoryRoot, "artifacts");

            foreach (var project in manifest.Projects)
            {
                if (!facts.TryGetValue(project.Name, out _))
                {
                    continue;
                }

                var policy = manifest.ClassOf(project);
                var projectDirectory = projectDirectories[project.Name];
                var commandLine = CompilerCommandLine.Parse(
                    evaluator.CompilerArguments(Path.Combine(repositoryRoot, project.Path)),
                    projectDirectory);

                CheckCompilerOptions(project, policy, commandLine);
                CheckCompilerReferences(project, policy, commandLine);
                CheckCompilerSources(project, policy, commandLine, projectDirectory, artifactsRoot);
            }
        }

        private void CheckCompilerOptions(
            ManifestProject project,
            ProjectClassPolicy policy,
            CompilerCommandLine commandLine)
        {
            void RequireOption(string description, bool satisfied, string actual)
            {
                if (!satisfied)
                {
                    Report(
                        CheckIds.CompilerOption,
                        PhaseCompiler,
                        $"{project.Name}:{description}",
                        $"The compiler was actually invoked with '{actual}', which breaks the '{project.ClassName}' contract.");
                }
            }

            if (policy.RequiredProperties.TryGetValue("LangVersion", out var langVersion))
            {
                RequireOption(
                    "langversion",
                    string.Equals(commandLine.Value("langversion"), langVersion, StringComparison.OrdinalIgnoreCase),
                    "langversion=" + commandLine.Value("langversion"));
            }

            if (policy.RequiredProperties.TryGetValue("TreatWarningsAsErrors", out var warningsAsErrors)
                && string.Equals(warningsAsErrors, "true", StringComparison.Ordinal))
            {
                RequireOption(
                    "warnaserror",
                    commandLine.HasSwitch("warnaserror+") && !commandLine.HasSwitch("warnaserror-"),
                    commandLine.HasSwitch("warnaserror-") ? "warnaserror-" : "no warnaserror+");
            }

            if (policy.RequiredProperties.TryGetValue("Nullable", out var nullable))
            {
                RequireOption(
                    "nullable",
                    string.Equals(commandLine.Value("nullable"), nullable, StringComparison.OrdinalIgnoreCase),
                    "nullable=" + commandLine.Value("nullable"));
            }

            if (policy.RequiredProperties.TryGetValue("Deterministic", out var deterministic)
                && string.Equals(deterministic, "true", StringComparison.Ordinal))
            {
                RequireOption("deterministic", commandLine.HasSwitch("deterministic+"), "no deterministic+");
            }

            if (policy.RequiredProperties.TryGetValue("DebugType", out var debugType))
            {
                RequireOption(
                    "debug",
                    string.Equals(commandLine.Value("debug"), debugType, StringComparison.OrdinalIgnoreCase),
                    "debug=" + commandLine.Value("debug"));
            }

            if (policy.RequiredProperties.TryGetValue("EmbedAllSources", out var embedAllSources)
                && string.Equals(embedAllSources, "true", StringComparison.Ordinal))
            {
                RequireOption("embed", commandLine.HasSwitch("embed"), "no embed");
            }

            foreach (var suppressed in commandLine.SuppressedWarnings())
            {
                if (!manifest.Toolchain.AllowedNoWarn.Contains(suppressed))
                {
                    Report(
                        CheckIds.CompilerSuppression,
                        PhaseCompiler,
                        $"{project.Name}:{suppressed}",
                        "The compiler was actually invoked with a warning suppression outside the manifest allow-list.");
                }
            }
        }

        private void CheckCompilerReferences(
            ManifestProject project,
            ProjectClassPolicy policy,
            CompilerCommandLine commandLine)
        {
            var declared = new SortedSet<string>(project.DependsOn, StringComparer.Ordinal);

            foreach (var reference in commandLine.Values("reference"))
            {
                var name = Path.GetFileNameWithoutExtension(reference);

                if (policy.EngineFree && forbiddenNames.IsMatch(name))
                {
                    Report(
                        CheckIds.EngineDependencyCompiler,
                        PhaseCompiler,
                        $"{project.Name}:{name}",
                        "The compiler was actually given a forbidden engine/game-content reference.");
                    continue;
                }

                if (manifest.FindProject(name) is not null && !declared.Contains(name))
                {
                    Report(
                        CheckIds.CompilerReferenceUndeclared,
                        PhaseCompiler,
                        $"{project.Name} -> {name}",
                        "The compiler was actually given a kernel reference that the manifest does not declare.");
                }
            }
        }

        private void CheckCompilerSources(
            ManifestProject project,
            ProjectClassPolicy policy,
            CompilerCommandLine commandLine,
            string projectDirectory,
            string artifactsRoot)
        {
            var owned = new SortedSet<string>(ownedSources[project.Name], StringComparer.Ordinal);
            var seen = new SortedSet<string>(StringComparer.Ordinal);

            foreach (var source in commandLine.Sources)
            {
                if (RepositoryPaths.IsInside(artifactsRoot, source))
                {
                    CheckGeneratedSource(project.Name, source, PhaseCompiler);
                    continue;
                }

                if (!RepositoryPaths.IsInside(repositoryRoot, source))
                {
                    if (!policy.AllowExternalCompiledSources)
                    {
                        Report(
                            CheckIds.CompilerSourceForeign,
                            PhaseCompiler,
                            $"{project.Name}:{source}",
                            "The compiler was actually given source from outside the repository.");
                    }

                    continue;
                }

                if (!RepositoryPaths.IsInside(projectDirectory, source))
                {
                    Report(
                        CheckIds.CompilerSourceForeign,
                        PhaseCompiler,
                        $"{project.Name}:{Relative(source)}",
                        "The compiler was actually given source that this project does not own.");
                    continue;
                }

                seen.Add(source);
            }

            foreach (var source in owned)
            {
                if (!seen.Contains(source))
                {
                    Report(
                        CheckIds.CompilerSourceMismatch,
                        PhaseCompiler,
                        $"{project.Name}:{Relative(source)}",
                        "An owned source file was never passed to the compiler.");
                }
            }
        }

        /// <summary>Maps a short target framework moniker to the emitted framework name.</summary>
        /// <param name="shortMoniker">Short moniker such as <c>netstandard2.1</c>.</param>
        /// <returns>The framework name recorded in the assembly.</returns>
        public static string FrameworkMoniker(string shortMoniker)
        {
            return shortMoniker switch
            {
                "netstandard2.1" => ".NETStandard,Version=v2.1",
                "netstandard2.0" => ".NETStandard,Version=v2.0",
                "net8.0" => ".NETCoreApp,Version=v8.0",
                _ => throw new ProofToolException(
                    $"No effective framework mapping for target framework '{shortMoniker}'."),
            };
        }

        private static IReadOnlyList<string> SplitList(string value)
        {
            var results = new List<string>();
            if (string.IsNullOrWhiteSpace(value))
            {
                return results;
            }

            foreach (var token in value.Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                results.Add(token.Trim());
            }

            return results;
        }
    }
}
