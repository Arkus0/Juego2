using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk01SelfAttackTests
    {
        [Fact]
        public void RegisteredDispatchableButUndiscoveredTurnsProjectionConformanceRed()
        {
            var contract = Hk01TestFixtures.ComposeWithFixture();
            var projection = new CanonicalContractProjection(new[] { contract.Definitions[0] });

            var issues = CanonicalProjectionConformance.Compare(contract.Definitions, projection);

            Assert.Contains(issues, issue => issue.Code == "projection.omitted_capability");
        }

        [Fact]
        public void DiscoveredCapabilityMissingSuccessOrErrorSchemaFailsClosed()
        {
            var valid = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var invalid = new CapabilityDefinition(
                valid.Key,
                valid.Provider,
                valid.RequestSchema,
                null,
                null,
                valid.SideEffect,
                valid.Determinism,
                valid.Preconditions,
                valid.Postconditions,
                valid.Concurrency,
                valid.Idempotency,
                valid.Batching,
                valid.Repair,
                valid.Policy,
                valid.Cost);

            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { invalid },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });
            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "contract.missing_schema");
        }

        [Fact]
        public void SchemaDispatcherOrBindingMismatchFailsComposition()
        {
            var definition = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var wrongKey = new CapabilityKey("engine.other", new ContractVersion(1, 0));
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { definition },
                new[] { new CapabilityRoute("fixture.engine", wrongKey, new FixtureEngineHandler()) });

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.binding_metadata_mismatch");
            Assert.Contains(result.Issues, issue => issue.Code == "composition.definition_without_route");
            Assert.Contains(result.Issues, issue => issue.Code == "composition.route_without_definition");
        }

        [Fact]
        public void UnsupportedContractVersionFailsBeforeHandlerInvocation()
        {
            FixtureEngineHandler.InvocationCount = 0;
            var contract = Hk01TestFixtures.ComposeWithFixture();

            var result = contract.Dispatch(
                "engine.observe",
                ContractVersionRange.Exact(new ContractVersion(9, 0)),
                Hk01TestFixtures.EmptyRequest());

            Assert.False(result.Success);
            Assert.Equal("contract.unsupported_version", result.Error!.MachineCode);
            Assert.Equal(0, FixtureEngineHandler.InvocationCount);
        }

        [Fact]
        public void UnknownExtraRouteIsFoundByIndependentUniverse()
        {
            var contract = BaseContract.Compose();
            var universe = RouteUniverse.Enumerate(typeof(OrphanHandler).Assembly);

            Assert.Contains(universe.Routes, route => route.ProviderId == "fixture.orphan");
            var report = CanonicalContractConformance.Evaluate(contract, universe);

            Assert.False(report.IsConformant);
            Assert.Contains(report.Issues, issue => issue.Code == "conformance.extra-public-route");
        }

        [Fact]
        public void ImplementationTypeOrTransportSpecificSchemaMetadataIsRejected()
        {
            var valid = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var implementationTypeSchema = new JsonSchemaDocument(SchemaNode.String(format: "dotnet-type"));
            var implementationIssues = CanonicalContractValidator.Validate(
                Hk01TestFixtures.CopyWithRequest(valid, implementationTypeSchema));
            Assert.Contains(implementationIssues, issue => issue.Code == "contract.schema.non_portable_format");

            var transportSchema = new JsonSchemaDocument(SchemaNode.String(format: "mcp-tool"));
            var transportIssues = CanonicalContractValidator.Validate(
                Hk01TestFixtures.CopyWithRequest(valid, transportSchema));
            Assert.Contains(transportIssues, issue => issue.Code == "contract.schema.non_portable_format");
        }

        [Fact]
        public void TransportProjectionCannotBecomeCanonicalProviderRegistry()
        {
            var descriptor = new ProviderDescriptor(
                "transport.mcp",
                ProviderKind.TransportProjection,
                "transport",
                new[] { "mcp" });
            var contribution = new CanonicalProviderContribution(
                descriptor,
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { contribution });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.transport_registry_forbidden");
        }

        [Fact]
        public void ProjectionCannotSilentlyAlterCanonicalSchemaMeaning()
        {
            var contract = Hk01TestFixtures.ComposeWithFixture();
            CapabilityDefinition? original = null;
            foreach (var definition in contract.Definitions)
            {
                if (definition.Key.Name == "engine.observe")
                {
                    original = definition;
                    break;
                }
            }

            Assert.NotNull(original);
            var altered = new CapabilityDefinition(
                original!.Key,
                original.Provider,
                original.RequestSchema,
                new JsonSchemaDocument(SchemaNode.Object()),
                original.ErrorSchema,
                original.SideEffect,
                original.Determinism,
                original.Preconditions,
                original.Postconditions,
                original.Concurrency,
                original.Idempotency,
                original.Batching,
                original.Repair,
                original.Policy,
                original.Cost);

            var projected = new List<CapabilityDefinition>();
            foreach (var definition in contract.Definitions)
            {
                projected.Add(definition.Key.Equals(original.Key) ? altered : definition);
            }

            var issues = CanonicalProjectionConformance.Compare(
                contract.Definitions,
                new CanonicalContractProjection(projected));

            Assert.Contains(issues, issue => issue.Code == "projection.semantic_mismatch");
        }

        [Fact]
        public void EmittedDiscoveryArtifactCannotAlterSemanticMetadata()
        {
            var contract = Hk01TestFixtures.ComposeWithFixture();
            var discovery = contract.Dispatch(
                "system.describe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                Hk01TestFixtures.EmptyRequest());
            Assert.True(discovery.Success);

            var alteredRoot = new Dictionary<string, object?>(discovery.Data!, StringComparer.Ordinal);
            var emittedCapabilities = Assert.IsAssignableFrom<IReadOnlyList<object?>>(alteredRoot["capabilities"]);
            var alteredCapabilities = new List<object?>(emittedCapabilities);
            for (var index = 0; index < alteredCapabilities.Count; index++)
            {
                var emittedCapability = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(alteredCapabilities[index]);
                if (string.Equals(emittedCapability["name"] as string, "engine.observe", StringComparison.Ordinal))
                {
                    var alteredCapability = new Dictionary<string, object?>(emittedCapability, StringComparer.Ordinal)
                    {
                        ["preconditions"] = new List<object?> { "artifact-semantic-drift" }.AsReadOnly()
                    };
                    alteredCapabilities[index] = alteredCapability;
                    break;
                }
            }

            alteredRoot["capabilities"] = alteredCapabilities.AsReadOnly();
            var issues = CanonicalProjectionConformance.Compare(contract.Definitions, alteredRoot);

            Assert.Contains(issues, issue => issue.Code == "projection.semantic_mismatch");
        }

        [Fact]
        public void ProjectionOracleDetectsOptionalSemanticFieldOmittedFromArtifact()
        {
            var contract = Hk01TestFixtures.ComposeWithFixture();
            var emitted = contract.Projection.ToData();
            var alteredRoot = new Dictionary<string, object?>(emitted, StringComparer.Ordinal);
            var emittedCapabilities = Assert.IsAssignableFrom<IReadOnlyList<object?>>(emitted["capabilities"]);
            var alteredCapabilities = new List<object?>(emittedCapabilities);
            for (var index = 0; index < alteredCapabilities.Count; index++)
            {
                var emittedCapability = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(alteredCapabilities[index]);
                if (string.Equals(emittedCapability["name"] as string, "engine.observe", StringComparison.Ordinal))
                {
                    var alteredCapability = new Dictionary<string, object?>(emittedCapability, StringComparer.Ordinal);
                    Assert.True(alteredCapability.Remove("cost"));
                    alteredCapabilities[index] = alteredCapability;
                    break;
                }
            }

            alteredRoot["capabilities"] = alteredCapabilities.AsReadOnly();
            var issues = CanonicalProjectionConformance.Compare(contract.Definitions, alteredRoot);

            Assert.Contains(issues, issue => issue.Code == "projection.semantic_mismatch");
        }

        [Fact]
        public void DeletingRegistryMetadataCannotShrinkIndependentRouteProofUniverse()
        {
            var emptyBase = new CanonicalProviderContribution(
                new ProviderDescriptor("arkus.base", ProviderKind.Base, "base", new[] { "system" }),
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());
            var composition = ContractComposer.Compose(emptyBase);
            Assert.True(composition.Success);

            var universe = RouteUniverse.Enumerate(typeof(SystemDescribeHandler).Assembly);
            var report = CanonicalContractConformance.Evaluate(composition.Contract!, universe);

            Assert.False(report.IsConformant);
            Assert.Contains(report.Issues, issue => issue.Code == "conformance.extra-public-route");
        }

        [Fact]
        public void SyntheticScopedPublicRouteWithoutCanonicalCompositionIsRejectedByConformance()
        {
            var contract = BaseContract.Compose();
            var universe = RouteUniverse.Enumerate(typeof(FixtureEngineHandler).Assembly);

            Assert.Contains(universe.Routes, route => route.ProviderId == "fixture.engine");
            var report = CanonicalContractConformance.Evaluate(contract, universe);

            Assert.False(report.IsConformant);
            Assert.Contains(report.Issues, issue => issue.Code == "conformance.extra-public-route");
        }

        [Fact]
        public void DuplicateOrOverlappingScopedNamespaceFailsClosed()
        {
            var first = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.a", ProviderKind.Scoped, "engine", new[] { "engine" }),
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());
            var second = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.b", ProviderKind.Scoped, "engine", new[] { "engine.sub" }),
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { first, second });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.namespace_conflict");
        }

        [Fact]
        public void DuplicateScopedCapabilityIdentityFailsClosed()
        {
            var firstDefinition = Hk01TestFixtures.DefinitionForProvider(
                "fixture.a", "engine", "engine.observe", new ContractVersion(1, 0), true);
            var secondDefinition = Hk01TestFixtures.DefinitionForProvider(
                "fixture.b", "engine", "engine.observe", new ContractVersion(1, 0), true);
            var first = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.a", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { firstDefinition },
                System.Array.Empty<CapabilityRoute>());
            var second = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.b", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { secondDefinition },
                System.Array.Empty<CapabilityRoute>());

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { first, second });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.duplicate_capability");
        }

        [Fact]
        public void ScopedSchemaCannotRequireEngineRuntimeImplementationType()
        {
            var valid = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var runtimeTypeRequest = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["target"] = SchemaNode.String(format: "dotnet-type")
                },
                new[] { "target" }));
            var invalid = Hk01TestFixtures.CopyWithRequest(valid, runtimeTypeRequest);
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { invalid },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "contract.schema.non_portable_format");
        }

        [Fact]
        public void ImplementationBindingCannotLieAboutContractVersion()
        {
            var v11 = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 1), true);
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { v11 },
                new[] { new CapabilityRoute("fixture.engine", v11.Key, new FixtureEngineHandler()) });

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.binding_metadata_mismatch");
        }
    }
}
