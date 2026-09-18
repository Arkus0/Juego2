using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Arkus.HK00.Proof
{
    internal sealed class Finding
    {
        public Finding(string id, string phase, string subject, string message)
        {
            Id = id;
            Phase = phase;
            Subject = subject;
            Message = message;
        }

        public string Id { get; }
        public string Phase { get; }
        public string Subject { get; }
        public string Message { get; }
    }

    internal sealed class ProofRunner
    {
        private readonly string root;
        private readonly string configuration;
        private readonly List<Finding> findings = new List<Finding>();
        private readonly SortedSet<string> tracked = new SortedSet<string>(StringComparer.Ordinal);
        private readonly SortedSet<string> trackedSources = new SortedSet<string>(StringComparer.Ordinal);
        private readonly SortedSet<string> trackedProjects = new SortedSet<string>(StringComparer.Ordinal);
        private readonly Dictionary<string, ProjectSpec> sourceOwners = new Dictionary<string, ProjectSpec>(StringComparer.Ordinal);
        private readonly Dictionary<string, ProjectFacts> projectFacts = new Dictionary<string, ProjectFacts>(StringComparer.Ordinal);
        private readonly SortedDictionary<string, SortedSet<string>> actualEdges = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
        private readonly SortedDictionary<string, SortedSet<string>> effectiveSources = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
        private readonly MsBuildProbe probe;

        public ProofRunner(string root, string configuration)
        {
            this.root = Path.GetFullPath(root);
            this.configuration = configuration;
            probe = new MsBuildProbe(this.root, configuration);
        }

        public IReadOnlyList<Finding> Findings => findings;

        public void RunRepository()
        {
            CheckRequiredFiles();
            ReadGitUniverse();
            CheckProjectUniverse();
            CheckSourceUniverse();
            CheckToolchainPin();
        }

        public void RunStatic()
        {
            EnsureRepositoryUniverse();
            foreach (var spec in FixedContract.Projects)
            {
                var full = Path.Combine(root, spec.Path);
                if (!File.Exists(full))
                {
                    continue;
                }

                try
                {
                    projectFacts[spec.Name] = probe.Evaluate(spec);
                }
                catch (Exception ex)
                {
                    Report("HK00-MSBUILD-EVALUATION", "static", spec.Path, ex.Message);
                }
            }

            foreach (var spec in FixedContract.Projects)
            {
                if (!projectFacts.TryGetValue(spec.Name, out var facts))
                {
                    continue;
                }

                CheckProperties(spec, facts);
                CheckPackages(spec, facts);
                CheckReferences(spec, facts);
                CheckStaticCompileOwnership(spec, facts);
            }

            CheckCycles();
        }

        public void RunEffective()
        {
            EnsureRepositoryUniverse();
            if (projectFacts.Count == 0)
            {
                RunStatic();
            }

            foreach (var spec in FixedContract.Projects)
            {
                if (!spec.IsProduct || !projectFacts.TryGetValue(spec.Name, out var facts))
                {
                    continue;
                }

                try
                {
                    var projectDirectory = Path.Combine(root, spec.Directory);
                    var compiler = CompilerCommandLine.Parse(probe.CompilerArguments(spec), projectDirectory);
                    CheckCompilerOptions(spec, compiler);
                    CheckCompilerSources(spec, compiler);
                    CheckCompilerReferences(spec, compiler);
                    CheckAssemblyAndPdb(spec, facts);
                }
                catch (Exception ex)
                {
                    Report("HK00-EFFECTIVE-ORACLE", "effective", spec.Path, ex.Message);
                }
            }
        }

        public void WriteEvidence(string inventoryDirectory, string reportPath)
        {
            Directory.CreateDirectory(inventoryDirectory);
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

            var projectRows = new List<object>();
            foreach (var spec in FixedContract.Projects.OrderBy(p => p.Path, StringComparer.Ordinal))
            {
                projectRows.Add(new
                {
                    spec.Name,
                    spec.Path,
                    Kind = spec.Kind.ToString(),
                    spec.TargetFramework,
                });
            }

            File.WriteAllText(
                Path.Combine(inventoryDirectory, "projects.json"),
                JsonSerializer.Serialize(projectRows, jsonOptions) + Environment.NewLine);

            var sourceRows = new List<object>();
            foreach (var source in trackedSources)
            {
                sourceOwners.TryGetValue(source, out var owner);
                sourceRows.Add(new { Path = source, Owner = owner?.Name ?? string.Empty });
            }

            File.WriteAllText(
                Path.Combine(inventoryDirectory, "sources.json"),
                JsonSerializer.Serialize(sourceRows, jsonOptions) + Environment.NewLine);

            var graphRows = new List<object>();
            foreach (var spec in FixedContract.Projects.OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                var edges = actualEdges.TryGetValue(spec.Name, out var actual)
                    ? actual.ToArray()
                    : Array.Empty<string>();
                graphRows.Add(new { Project = spec.Name, Dependencies = edges });
            }

            File.WriteAllText(
                Path.Combine(inventoryDirectory, "graph.json"),
                JsonSerializer.Serialize(graphRows, jsonOptions) + Environment.NewLine);

            Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
            var report = new
            {
                CandidateSha = ProcessExec.Run("git", new[] { "rev-parse", "HEAD" }, root).Stdout.Trim(),
                Configuration = configuration,
                RunningSdk = ProcessExec.Run("dotnet", new[] { "--version" }, root).Stdout.Trim(),
                FindingCount = findings.Count,
                Findings = findings.Select(f => new { f.Id, f.Phase, f.Subject, f.Message }).ToArray(),
            };
            File.WriteAllText(reportPath, JsonSerializer.Serialize(report, jsonOptions) + Environment.NewLine);
        }

        private void CheckRequiredFiles()
        {
            foreach (var required in FixedContract.RequiredFiles)
            {
                if (!File.Exists(Path.Combine(root, required)))
                {
                    Report("HK00-REQUIRED-FILE-MISSING", "repository", required, "A fixed proof/test/CI input is absent.");
                }
            }
        }

        private void ReadGitUniverse()
        {
            tracked.Clear();
            trackedSources.Clear();
            trackedProjects.Clear();
            sourceOwners.Clear();

            var rootResult = ProcessExec.Run("git", new[] { "rev-parse", "--show-toplevel" }, root);
            if (!string.Equals(Path.GetFullPath(rootResult.Stdout.Trim()), root, StringComparison.Ordinal))
            {
                Report("HK00-GIT-ROOT", "repository", rootResult.Stdout.Trim(), "Proof root is not the Git worktree root.");
            }

            var staged = ProcessExec.Run("git", new[] { "ls-files", "-s", "-z" }, root);
            foreach (var entry in ProcessExec.SplitNull(staged.Stdout))
            {
                var tab = entry.IndexOf('\t');
                if (tab < 0)
                {
                    continue;
                }

                var metadata = entry.Substring(0, tab).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var path = entry.Substring(tab + 1).Replace('\\', '/');
                tracked.Add(path);
                var mode = metadata.Length > 0 ? metadata[0] : string.Empty;
                if (string.Equals(mode, "120000", StringComparison.Ordinal))
                {
                    Report("HK00-REPO-SYMLINK", "repository", path, "Tracked symlinks are outside the HK00 ownership model.");
                }
                else if (string.Equals(mode, "160000", StringComparison.Ordinal))
                {
                    Report("HK00-REPO-GITLINK", "repository", path, "Tracked gitlinks/submodules are outside the HK00 ownership model.");
                }

                if (IsCSharp(path))
                {
                    CheckExtensionCase(path);
                    if (!File.Exists(Path.Combine(root, path)))
                    {
                        Report("HK00-TRACKED-MISSING", "repository", path, "Tracked C# input is absent from the physical checkout.");
                    }
                }

                if (path.EndsWith(".cs", StringComparison.Ordinal))
                {
                    trackedSources.Add(path);
                }
                else if (path.EndsWith(".csproj", StringComparison.Ordinal))
                {
                    trackedProjects.Add(path);
                }
            }

            var untracked = ProcessExec.Run("git", new[] { "ls-files", "--others", "--exclude-standard", "-z" }, root);
            foreach (var pathRaw in ProcessExec.SplitNull(untracked.Stdout))
            {
                var path = pathRaw.Replace('\\', '/');
                if (!IsCSharp(path) || IsGeneratedWorkspacePath(path))
                {
                    continue;
                }

                Report("HK00-REPO-UNTRACKED-CSHARP", "repository", path, "Physical C# input is not part of the candidate Git tree.");
            }
        }

        private void CheckProjectUniverse()
        {
            var expected = new SortedSet<string>(FixedContract.Projects.Select(p => p.Path), StringComparer.Ordinal);
            foreach (var project in trackedProjects)
            {
                if (!expected.Contains(project))
                {
                    Report("HK00-PROJECT-UNEXPECTED", "repository", project, "Tracked C# project is not part of the fixed HK00 contract.");
                }
            }

            foreach (var project in expected)
            {
                if (!trackedProjects.Contains(project))
                {
                    Report("HK00-PROJECT-MISSING", "repository", project, "Fixed HK00 project is absent from the tracked project universe.");
                }
            }
        }

        private void CheckSourceUniverse()
        {
            foreach (var source in trackedSources)
            {
                var owners = new List<ProjectSpec>();
                foreach (var spec in FixedContract.Projects)
                {
                    var prefix = spec.Directory.TrimEnd('/') + "/";
                    if (source.StartsWith(prefix, StringComparison.Ordinal))
                    {
                        owners.Add(spec);
                    }
                }

                if (owners.Count != 1)
                {
                    Report(
                        owners.Count == 0 ? "HK00-SOURCE-UNOWNED" : "HK00-SOURCE-MULTI-OWNER",
                        "repository",
                        source,
                        owners.Count == 0
                            ? "Tracked C# source belongs to no fixed project boundary."
                            : "Tracked C# source falls under more than one fixed project boundary.");
                    continue;
                }

                sourceOwners[source] = owners[0];
            }
        }

        private void CheckToolchainPin()
        {
            var path = Path.Combine(root, "global.json");
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                using var document = JsonDocument.Parse(File.ReadAllText(path));
                var sdk = document.RootElement.GetProperty("sdk");
                var version = sdk.GetProperty("version").GetString() ?? string.Empty;
                var rollForward = sdk.GetProperty("rollForward").GetString() ?? string.Empty;
                var allowPrerelease = sdk.TryGetProperty("allowPrerelease", out var pre) && pre.GetBoolean();
                if (!string.Equals(version, FixedContract.SdkVersion, StringComparison.Ordinal)
                    || !string.Equals(rollForward, FixedContract.SdkRollForward, StringComparison.Ordinal)
                    || allowPrerelease)
                {
                    Report("HK00-TOOLCHAIN-PIN", "repository", "global.json", "SDK pin differs from the fixed HK00 contract.");
                }
            }
            catch (Exception ex)
            {
                Report("HK00-TOOLCHAIN-PIN", "repository", "global.json", "SDK pin is unreadable: " + ex.Message);
            }
        }

        private void CheckProperties(ProjectSpec spec, ProjectFacts facts)
        {
            RequireProperty(spec, facts, "AssemblyName", spec.Name);
            RequireProperty(spec, facts, "TargetFramework", spec.TargetFramework);
            RequireProperty(spec, facts, "LangVersion", "9.0");
            RequireProperty(spec, facts, "Nullable", "enable");
            RequireProperty(spec, facts, "TreatWarningsAsErrors", "true");
            RequireProperty(spec, facts, "DisableTransitiveProjectReferences", "true");
            RequireProperty(spec, facts, "Deterministic", "true");
            RequireProperty(spec, facts, "DebugType", "portable");
            RequireProperty(spec, facts, "EmbedAllSources", "true");
            RequireProperty(spec, facts, "GenerateAssemblyInfo", "false");
            RequireProperty(spec, facts, "GenerateTargetFrameworkAttribute", "false");
            RequireProperty(spec, facts, "ImplicitUsings", "disable");
            RequireProperty(spec, facts, "ManagePackageVersionsCentrally", "true");

            var warningsNotAsErrors = facts.Property("WarningsNotAsErrors");
            if (!string.IsNullOrWhiteSpace(warningsNotAsErrors))
            {
                Report("HK00-WARNING-DEMOTION", "static", spec.Name, "WarningsNotAsErrors is not empty: " + warningsNotAsErrors);
            }

            foreach (var token in SplitWarnings(facts.Property("NoWarn")))
            {
                if (!string.Equals(token, "1701", StringComparison.Ordinal)
                    && !string.Equals(token, "1702", StringComparison.Ordinal))
                {
                    Report("HK00-WARNING-SUPPRESSION", "static", spec.Name, "Unexpected NoWarn token: " + token);
                }
            }
        }

        private void RequireProperty(ProjectSpec spec, ProjectFacts facts, string name, string expected)
        {
            var actual = facts.Property(name);
            if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
            {
                Report("HK00-PROJECT-PROPERTY", "static", spec.Name + ":" + name, $"Expected '{expected}', observed '{actual}'.");
            }
        }

        private void CheckPackages(ProjectSpec spec, ProjectFacts facts)
        {
            var allowed = new SortedSet<string>(spec.AllowedPackages, StringComparer.Ordinal);
            var actual = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var package in facts.Packages)
            {
                actual.Add(package.Id);
                if (!allowed.Contains(package.Id))
                {
                    Report("HK00-PACKAGE-FORBIDDEN", "static", spec.Name + ":" + package.Id, "Package is not allowed by the fixed HK00 contract.");
                }
                if (!string.IsNullOrEmpty(package.Version))
                {
                    Report("HK00-PACKAGE-INLINE-VERSION", "static", spec.Name + ":" + package.Id, "Package version bypasses central version policy.");
                }
            }

            foreach (var required in allowed)
            {
                if (!actual.Contains(required))
                {
                    Report("HK00-PACKAGE-MISSING", "static", spec.Name + ":" + required, "Required test package is absent.");
                }
            }
        }

        private void CheckReferences(ProjectSpec spec, ProjectFacts facts)
        {
            foreach (var reference in facts.Item("Reference"))
            {
                if (spec.IsProduct)
                {
                    Report("HK00-RAW-REFERENCE", "static", spec.Name + ":" + reference, "Product project uses a raw assembly Reference item.");
                }
                if (IsForbiddenEngineName(reference))
                {
                    Report("HK00-ENGINE-REFERENCE", "static", spec.Name + ":" + reference, "Engine/DFU reference appears in the portable boundary.");
                }
            }

            var actual = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var projectReference in facts.Item("ProjectReference"))
            {
                var relative = ProcessExec.Relative(root, projectReference);
                var target = FixedContract.ByPath(relative);
                if (target is null)
                {
                    Report("HK00-GRAPH-UNKNOWN-TARGET", "static", spec.Name + " -> " + relative, "ProjectReference points outside the fixed project universe.");
                    continue;
                }
                actual.Add(target.Name);
            }

            actualEdges[spec.Name] = actual;
            var expected = new SortedSet<string>(spec.Dependencies, StringComparer.Ordinal);
            foreach (var edge in actual)
            {
                if (!expected.Contains(edge))
                {
                    Report("HK00-GRAPH-UNDECLARED-EDGE", "static", spec.Name + " -> " + edge, "Observed project edge is not allowed by the fixed HK00 contract.");
                }
            }
            foreach (var edge in expected)
            {
                if (!actual.Contains(edge))
                {
                    Report("HK00-GRAPH-MISSING-EDGE", "static", spec.Name + " -> " + edge, "Required project edge is absent.");
                }
            }
        }

        private void CheckStaticCompileOwnership(ProjectSpec spec, ProjectFacts facts)
        {
            var seen = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var compile in facts.Item("Compile"))
            {
                if (string.IsNullOrEmpty(compile))
                {
                    continue;
                }
                var full = Path.GetFullPath(compile);
                if (!ProcessExec.IsInside(root, full))
                {
                    if (spec.IsProduct)
                    {
                        Report("HK00-SOURCE-FOREIGN-COMPILE", "static", spec.Name + ":" + full, "Evaluated Compile item is outside the repository.");
                    }
                    continue;
                }

                var relative = ProcessExec.Relative(root, full);
                seen.Add(relative);
                if (!trackedSources.Contains(relative))
                {
                    if (spec.IsProduct)
                    {
                        Report("HK00-SOURCE-UNTRACKED-COMPILE", "static", spec.Name + ":" + relative, "Evaluated product Compile item is not tracked source.");
                    }
                    continue;
                }
                if (!sourceOwners.TryGetValue(relative, out var owner) || !ReferenceEquals(owner, spec))
                {
                    Report("HK00-SOURCE-FOREIGN-COMPILE", "static", spec.Name + ":" + relative, "Evaluated Compile item is owned by a different project boundary.");
                }
            }

            foreach (var pair in sourceOwners)
            {
                if (ReferenceEquals(pair.Value, spec) && !seen.Contains(pair.Key))
                {
                    Report("HK00-SOURCE-NOT-COMPILED-STATIC", "static", spec.Name + ":" + pair.Key, "Owned source is absent from evaluated Compile items.");
                }
            }
        }

        private void CheckCycles()
        {
            var visiting = new HashSet<string>(StringComparer.Ordinal);
            var visited = new HashSet<string>(StringComparer.Ordinal);
            foreach (var spec in FixedContract.Projects)
            {
                DetectCycle(spec.Name, visiting, visited, new List<string>());
            }
        }

        private void DetectCycle(string node, HashSet<string> visiting, HashSet<string> visited, List<string> path)
        {
            if (visited.Contains(node)) return;
            if (!visiting.Add(node))
            {
                path.Add(node);
                Report("HK00-GRAPH-CYCLE", "static", string.Join(" -> ", path), "Evaluated project graph contains a cycle.");
                path.RemoveAt(path.Count - 1);
                return;
            }

            path.Add(node);
            if (actualEdges.TryGetValue(node, out var edges))
            {
                foreach (var edge in edges)
                {
                    DetectCycle(edge, visiting, visited, path);
                }
            }
            path.RemoveAt(path.Count - 1);
            visiting.Remove(node);
            visited.Add(node);
        }

        private void CheckCompilerOptions(ProjectSpec spec, CompilerCommandLine compiler)
        {
            RequireCompilerOption(spec, "langversion", string.Equals(compiler.Value("langversion"), "9.0", StringComparison.OrdinalIgnoreCase), compiler.Value("langversion"));
            RequireCompilerOption(spec, "nullable", string.Equals(compiler.Value("nullable"), "enable", StringComparison.OrdinalIgnoreCase), compiler.Value("nullable"));
            RequireCompilerOption(spec, "warnaserror", compiler.HasSwitch("warnaserror+") && !compiler.HasSwitch("warnaserror-"), compiler.HasSwitch("warnaserror-") ? "warnaserror-" : "missing warnaserror+");
            RequireCompilerOption(spec, "deterministic", compiler.HasSwitch("deterministic+"), "missing deterministic+");
            RequireCompilerOption(spec, "debug", string.Equals(compiler.Value("debug"), "portable", StringComparison.OrdinalIgnoreCase), compiler.Value("debug"));
            RequireCompilerOption(spec, "embed", compiler.HasSwitch("embed"), "missing embed");

            foreach (var warning in compiler.SuppressedWarnings())
            {
                if (!string.Equals(warning, "1701", StringComparison.Ordinal)
                    && !string.Equals(warning, "1702", StringComparison.Ordinal))
                {
                    Report("HK00-COMPILER-SUPPRESSION", "effective", spec.Name + ":" + warning, "Compiler received an unapproved warning suppression.");
                }
            }
        }

        private void RequireCompilerOption(ProjectSpec spec, string option, bool condition, string observed)
        {
            if (!condition)
            {
                Report("HK00-COMPILER-OPTION", "effective", spec.Name + ":" + option, "Compiler option violates fixed contract; observed " + observed + ".");
            }
        }

        private void CheckCompilerSources(ProjectSpec spec, CompilerCommandLine compiler)
        {
            var seen = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var source in compiler.Sources)
            {
                var full = Path.GetFullPath(source);
                if (!ProcessExec.IsInside(root, full))
                {
                    Report("HK00-COMPILER-SOURCE-FOREIGN", "effective", spec.Name + ":" + full, "Product compiler consumed source outside the repository.");
                    continue;
                }
                var relative = ProcessExec.Relative(root, full);
                if (!trackedSources.Contains(relative))
                {
                    Report("HK00-COMPILER-SOURCE-UNTRACKED", "effective", spec.Name + ":" + relative, "Product compiler consumed generated/untracked C# source; HK00 forbids this structurally.");
                    continue;
                }
                if (!sourceOwners.TryGetValue(relative, out var owner) || !ReferenceEquals(owner, spec))
                {
                    Report("HK00-COMPILER-SOURCE-FOREIGN", "effective", spec.Name + ":" + relative, "Compiler consumed source owned by another fixed project boundary.");
                    continue;
                }
                seen.Add(relative);
            }

            effectiveSources[spec.Name] = seen;
            foreach (var pair in sourceOwners)
            {
                if (ReferenceEquals(pair.Value, spec) && !seen.Contains(pair.Key))
                {
                    Report("HK00-SOURCE-NOT-COMPILED-EFFECTIVE", "effective", spec.Name + ":" + pair.Key, "Owned source never reached the actual C# compiler command line.");
                }
            }
        }

        private void CheckCompilerReferences(ProjectSpec spec, CompilerCommandLine compiler)
        {
            var expected = new SortedSet<string>(spec.Dependencies, StringComparer.Ordinal);
            foreach (var reference in compiler.Values("reference"))
            {
                var name = Path.GetFileNameWithoutExtension(reference);
                if (IsForbiddenEngineName(name))
                {
                    Report("HK00-ENGINE-COMPILER-REF", "effective", spec.Name + ":" + name, "Compiler received a forbidden engine/DFU reference.");
                }
                if (FixedContract.ByName(name) is not null && !expected.Contains(name))
                {
                    Report("HK00-COMPILER-REF-UNDECLARED", "effective", spec.Name + " -> " + name, "Compiler received an undeclared Arkus project reference.");
                }
            }
        }

        private void CheckAssemblyAndPdb(ProjectSpec spec, ProjectFacts facts)
        {
            var targetPath = facts.Property("TargetPath");
            if (string.IsNullOrWhiteSpace(targetPath) || !File.Exists(targetPath))
            {
                Report("HK00-ASSEMBLY-MISSING", "effective", spec.Name, "Expected build output is absent: " + targetPath);
                return;
            }

            AssemblyFacts assembly;
            try
            {
                assembly = AssemblyFacts.Read(targetPath);
            }
            catch (FileNotFoundException ex)
            {
                Report("HK00-PDB-MISSING", "effective", spec.Name, ex.Message);
                return;
            }

            if (!string.Equals(assembly.AssemblyName, spec.Name, StringComparison.Ordinal))
            {
                Report("HK00-ASSEMBLY-NAME", "effective", spec.Name, "Built assembly name differs from fixed project identity.");
            }

            var expected = new SortedSet<string>(spec.Dependencies, StringComparer.Ordinal);
            foreach (var reference in assembly.AssemblyReferences)
            {
                if (IsForbiddenEngineName(reference))
                {
                    Report("HK00-ENGINE-ASSEMBLY-REF", "effective", spec.Name + ":" + reference, "Built assembly references forbidden engine/DFU assembly.");
                }
                if (FixedContract.ByName(reference) is not null && !expected.Contains(reference))
                {
                    Report("HK00-ASSEMBLY-REF-UNDECLARED", "effective", spec.Name + " -> " + reference, "Built assembly contains undeclared Arkus dependency.");
                }
            }

            var documents = new Dictionary<string, CompiledDocument>(StringComparer.Ordinal);
            foreach (var document in assembly.CompiledDocuments)
            {
                var full = Path.IsPathRooted(document.Path)
                    ? Path.GetFullPath(document.Path)
                    : Path.GetFullPath(Path.Combine(Path.Combine(root, spec.Directory), document.Path));
                if (!ProcessExec.IsInside(root, full))
                {
                    Report("HK00-PDB-SOURCE-FOREIGN", "effective", spec.Name + ":" + document.Path, "PDB records source outside repository.");
                    continue;
                }
                var relative = ProcessExec.Relative(root, full);
                if (!trackedSources.Contains(relative))
                {
                    Report("HK00-PDB-SOURCE-UNTRACKED", "effective", spec.Name + ":" + relative, "PDB records generated/untracked source in product assembly.");
                    continue;
                }
                documents[relative] = document;
            }

            foreach (var pair in sourceOwners)
            {
                if (!ReferenceEquals(pair.Value, spec))
                {
                    continue;
                }
                if (!documents.TryGetValue(pair.Key, out var document))
                {
                    Report("HK00-PDB-SOURCE-MISSING", "effective", spec.Name + ":" + pair.Key, "Owned source is absent from portable PDB document table.");
                    continue;
                }
                if (document.HashAlgorithm != AssemblyFacts.Sha256Algorithm
                    || !AssemblyFacts.HashEquals(document.Hash, AssemblyFacts.Sha256OfFile(Path.Combine(root, pair.Key))))
                {
                    Report("HK00-PDB-SOURCE-HASH", "effective", spec.Name + ":" + pair.Key, "PDB source checksum does not match candidate bytes.");
                }
            }
        }

        private void EnsureRepositoryUniverse()
        {
            if (tracked.Count == 0)
            {
                RunRepository();
            }
        }

        private static bool IsCSharp(string path)
        {
            return path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);
        }

        private void CheckExtensionCase(string path)
        {
            if (path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".cs", StringComparison.Ordinal))
            {
                Report("HK00-EXTENSION-CASE", "repository", path, "C# source extension casing is non-canonical.");
            }
            if (path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".csproj", StringComparison.Ordinal))
            {
                Report("HK00-EXTENSION-CASE", "repository", path, "C# project extension casing is non-canonical.");
            }
        }

        private static bool IsGeneratedWorkspacePath(string path)
        {
            foreach (var segment in path.Split('/'))
            {
                if (string.Equals(segment, "bin", StringComparison.Ordinal)
                    || string.Equals(segment, "obj", StringComparison.Ordinal)
                    || string.Equals(segment, "artifacts", StringComparison.Ordinal)
                    || string.Equals(segment, "TestResults", StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsForbiddenEngineName(string name)
        {
            return name.IndexOf("UnityEngine", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("UnityEditor", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("Daggerfall", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("DaggerfallUnity", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("DFU", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static IEnumerable<string> SplitWarnings(string value)
        {
            return value.Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim());
        }

        private void Report(string id, string phase, string subject, string message)
        {
            findings.Add(new Finding(id, phase, subject, message));
            Console.Error.WriteLine($"{id} [{phase}] {subject}: {message}");
        }
    }
}
