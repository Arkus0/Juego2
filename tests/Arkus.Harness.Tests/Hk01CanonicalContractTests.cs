using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk01CanonicalContractTests
    {
        [Fact]
        public void BaseContractComposesAndSystemDescribeDiscoversWholeSurface()
        {
            var contract = BaseContract.Compose();

            Assert.Single(contract.Definitions);
            Assert.Equal("system.describe", contract.Definitions[0].Key.Name);

            var result = contract.Dispatch(
                "system.describe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                Hk01TestFixtures.EmptyRequest());

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.ContainsKey("capabilities"));
            Assert.Empty(CanonicalProjectionConformance.Compare(contract.Definitions, result.Data));
        }

        [Fact]
        public void SyntheticScopedProviderComposesIntoSingleCanonicalInventory()
        {
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { Hk01TestFixtures.FixtureProvider() });

            Assert.True(composition.Success);
            Assert.NotNull(composition.Contract);
            Assert.Equal(2, composition.Contract!.Definitions.Count);

            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var definition in composition.Contract.Definitions)
            {
                names.Add(definition.Key.Name);
            }

            Assert.Contains("system.describe", names);
            Assert.Contains("engine.observe", names);
            Assert.Equal(2, composition.Contract.Projection.Capabilities.Count);
        }

        [Fact]
        public void AcceptedSyntheticScopedSurfaceIsIndependentlyEnumerableAndConformant()
        {
            var composition = ContractComposer.Compose(
                CanonicalWorldContract.CreateContribution(new UnavailableWorldInspectionService()),
                Hk01TestFixtures.CompleteFixtureProviders());
            Assert.True(composition.Success);
            Assert.NotNull(composition.Contract);

            var routeUniverse = RouteUniverse.Enumerate(new[]
            {
                typeof(SystemDescribeHandler).Assembly,
                typeof(FixtureEngineHandler).Assembly
            });
            var report = CanonicalContractConformance.Evaluate(composition.Contract!, routeUniverse);

            Assert.True(report.IsConformant, FormatIssues(report.Issues));
            Assert.Empty(routeUniverse.Issues);
            Assert.Equal(composition.Contract.Definitions.Count, routeUniverse.Routes.Count);
            Assert.Equal(composition.Contract.Definitions.Count, composition.Contract.Projection.Capabilities.Count);
            Assert.Contains(composition.Contract.Definitions, definition =>
                definition.Key.Name == WorldProvenanceContract.ReadName &&
                definition.Key.Version.Equals(new ContractVersion(1, 0)));
            Assert.Contains(composition.Contract.Definitions, definition =>
                definition.Key.Name == WorldProvenanceContract.ReadName &&
                definition.Key.Version.Equals(new ContractVersion(2, 0)));
        }

        [Fact]
        public void RuntimeFailsClosedForUnknownVersionAndSchemaInvalidPayload()
        {
            var contract = Hk01TestFixtures.ComposeWithFixture();

            var unknown = contract.Dispatch(
                "missing.capability",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                Hk01TestFixtures.EmptyRequest());
            Assert.False(unknown.Success);
            Assert.Equal("contract.unknown_capability", unknown.Error!.MachineCode);

            var unsupported = contract.Dispatch(
                "engine.observe",
                ContractVersionRange.Exact(new ContractVersion(2, 0)),
                Hk01TestFixtures.EmptyRequest());
            Assert.False(unsupported.Success);
            Assert.Equal("contract.unsupported_version", unsupported.Error!.MachineCode);

            var invalid = contract.Dispatch(
                "engine.observe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["target"] = 42
                });
            Assert.False(invalid.Success);
            Assert.Equal("contract.invalid_request", invalid.Error!.MachineCode);
            Assert.Equal("$/target", invalid.Error.Path);
        }

        [Fact]
        public void VersionNegotiationSelectsHighestAcceptedCompatibleVersion()
        {
            var v10 = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var v11 = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 1), true);
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { v10, v11 },
                new[]
                {
                    CapabilityRoute.FromHandler(new FixtureEngineHandler()),
                    CapabilityRoute.FromHandler(new FixtureEngineV11Handler())
                });

            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });
            Assert.True(composition.Success);

            var result = composition.Contract!.Dispatch(
                "engine.observe",
                new ContractVersionRange(1, 0, 1),
                Hk01TestFixtures.EmptyRequest());

            Assert.True(result.Success);
            Assert.Equal("1.1", result.Data!["value"]);
        }

        [Fact]
        public void CompatibilityRulesDistinguishAdditiveFromBreakingChange()
        {
            var v10 = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var v11 = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 1), true);
            var additive = ContractCompatibility.Compare(v10, v11);

            Assert.Equal(CompatibilityKind.Additive, additive.Kind);

            var breaking = new CapabilityDefinition(
                new CapabilityKey("engine.observe", new ContractVersion(1, 1)),
                v10.Provider,
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["target"] = Hk01TestFixtures.LogicalReference()
                    },
                    new[] { "target" })),
                v10.SuccessSchema,
                v10.ErrorSchema,
                v10.SideEffect,
                v10.Determinism,
                v10.Preconditions,
                v10.Postconditions,
                v10.Concurrency,
                v10.Idempotency,
                v10.Batching,
                v10.Repair,
                v10.Policy,
                v10.Cost);

            Assert.Equal(CompatibilityKind.Breaking, ContractCompatibility.Compare(v10, breaking).Kind);
        }

        [Fact]
        public void IndependentRouteUniverseMatchesDispatcherDiscoveryAndSchemas()
        {
            var baseContract = CanonicalWorldContract.Compose(new UnavailableWorldInspectionService());
            var productionAssemblies = LoadProductionAssemblies();
            var routeUniverse = RouteUniverse.Enumerate(productionAssemblies);

            var report = CanonicalContractConformance.Evaluate(baseContract, routeUniverse);

            Assert.True(report.IsConformant, FormatIssues(report.Issues));
            Assert.Empty(routeUniverse.Issues);
            Assert.Equal(baseContract.Definitions.Count, routeUniverse.Routes.Count);
            Assert.Contains(routeUniverse.Routes, route =>
                route.Key.Name == WorldProvenanceContract.ReadName &&
                route.Key.Version.Equals(new ContractVersion(1, 0)));
            Assert.Contains(routeUniverse.Routes, route =>
                route.Key.Name == WorldProvenanceContract.ReadName &&
                route.Key.Version.Equals(new ContractVersion(2, 0)));
            foreach (var route in routeUniverse.Routes)
            {
                Assert.Equal("arkus.base", route.ProviderId);
            }
        }

        [Fact]
        public void ScopedSchemaUsesPortableLogicalReferencesWithoutImplementationTypes()
        {
            var definition = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var issues = CanonicalContractValidator.Validate(definition);

            Assert.Empty(issues);
            Assert.DoesNotContain(typeof(FixtureEngineHandler).FullName!, definition.SemanticFingerprint());
            Assert.Equal("arkus-logical-reference", definition.RequestSchema!.Root.Properties["target"].Format);
            Assert.Equal("ref.fixture.engine.entity", definition.RequestSchema.Root.Properties["target"].LogicalReferenceNamespace);
        }

        private static IReadOnlyList<Assembly> LoadProductionAssemblies()
        {
            var repositoryRoot = FindRepositoryRoot();
            var sourceRoot = Path.Combine(repositoryRoot, "src");
            var projectDirectories = Directory.GetDirectories(sourceRoot);
            System.Array.Sort(projectDirectories, StringComparer.Ordinal);

            var loadedByName = new Dictionary<string, Assembly>(StringComparer.Ordinal);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.GetName().Name;
                if (name != null)
                {
                    loadedByName[name] = assembly;
                }
            }

            var assemblies = new List<Assembly>();
            foreach (var projectDirectory in projectDirectories)
            {
                var projectName = Path.GetFileName(projectDirectory);
                if (!File.Exists(Path.Combine(projectDirectory, projectName + ".csproj")))
                {
                    continue;
                }

                var output = FindReleaseAssembly(projectDirectory, projectName);
                if (loadedByName.TryGetValue(projectName, out var loaded))
                {
                    assemblies.Add(loaded);
                }
                else
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(output);
                    loadedByName.Add(projectName, assembly);
                    assemblies.Add(assembly);
                }
            }

            if (assemblies.Count == 0)
            {
                throw new InvalidOperationException("No production Arkus assemblies were independently discovered under src/.");
            }

            return assemblies.AsReadOnly();
        }

        private static string FindReleaseAssembly(string projectDirectory, string projectName)
        {
            var candidates = Directory.GetFiles(projectDirectory, projectName + ".dll", SearchOption.AllDirectories);
            string? selected = null;
            var releaseSegment = Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Release" + Path.DirectorySeparatorChar;
            var referenceSegment = Path.DirectorySeparatorChar + "ref" + Path.DirectorySeparatorChar;
            var referenceIntermediateSegment = Path.DirectorySeparatorChar + "refint" + Path.DirectorySeparatorChar;

            foreach (var candidate in candidates)
            {
                var normalized = candidate.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                if (!normalized.Contains(releaseSegment, StringComparison.Ordinal) ||
                    normalized.Contains(referenceSegment, StringComparison.Ordinal) ||
                    normalized.Contains(referenceIntermediateSegment, StringComparison.Ordinal))
                {
                    continue;
                }

                if (selected != null && !string.Equals(selected, candidate, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("Multiple effective Release assemblies found for production project '" + projectName + "'.");
                }

                selected = candidate;
            }

            if (selected == null)
            {
                throw new InvalidOperationException("No effective Release assembly found for production project '" + projectName + "'.");
            }

            return Path.GetFullPath(selected);
        }

        private static string FindRepositoryRoot()
        {
            DirectoryInfo? current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "Juego2.sln")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new InvalidOperationException("Unable to locate Juego2 repository root from the test execution directory.");
        }

        private static string FormatIssues(IReadOnlyList<ConformanceIssue> issues)
        {
            var text = string.Empty;
            foreach (var issue in issues)
            {
                text += issue.Code + ":" + issue.Subject + ";";
            }

            return text;
        }
    }
}
