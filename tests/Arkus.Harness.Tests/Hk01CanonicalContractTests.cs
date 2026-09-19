using System;
using System.Collections.Generic;
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
            Assert.Empty(CanonicalProjectionConformance.Compare(contract.Definitions, contract.Projection));
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
            var baseContract = BaseContract.Compose();
            var routeUniverse = RouteUniverse.Enumerate(
                typeof(SystemDescribeHandler).Assembly,
                new[] { "arkus.base" });

            var report = CanonicalContractConformance.Evaluate(baseContract, routeUniverse);

            Assert.True(report.IsConformant, FormatIssues(report.Issues));
            Assert.Empty(routeUniverse.Issues);
        }

        [Fact]
        public void ScopedSchemaUsesPortableLogicalReferencesWithoutImplementationTypes()
        {
            var definition = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var issues = CanonicalContractValidator.Validate(definition);

            Assert.Empty(issues);
            Assert.DoesNotContain(typeof(FixtureEngineHandler).FullName!, definition.SemanticFingerprint());
            Assert.Equal("arkus-logical-reference", definition.RequestSchema!.Root.Properties["target"].Format);
            Assert.Equal("fixture.engine.entity", definition.RequestSchema.Root.Properties["target"].LogicalReferenceNamespace);
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
