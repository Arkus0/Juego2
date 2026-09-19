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
                EmptyRequest());

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.ContainsKey("capabilities"));
            Assert.Empty(CanonicalProjectionConformance.Compare(contract.Definitions, contract.Projection));
        }

        [Fact]
        public void SyntheticScopedProviderComposesIntoSingleCanonicalInventory()
        {
            var scoped = FixtureProvider("fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0));
            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

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
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { FixtureProvider("fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0)) });
            Assert.True(composition.Success);
            var contract = composition.Contract!;

            var unknown = contract.Dispatch(
                "missing.capability",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                EmptyRequest());
            Assert.False(unknown.Success);
            Assert.Equal("contract.unknown_capability", unknown.Error!.MachineCode);

            var unsupported = contract.Dispatch(
                "engine.observe",
                ContractVersionRange.Exact(new ContractVersion(2, 0)),
                EmptyRequest());
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
            var v10 = FixtureDefinition("fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0), false);
            var v11 = FixtureDefinition("fixture.engine", "engine", "engine.observe", new ContractVersion(1, 1), true);
            var handler = new FixtureEngineHandler();
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { v10, v11 },
                new[]
                {
                    new CapabilityRoute("fixture.engine", v10.Key, handler),
                    new CapabilityRoute("fixture.engine", v11.Key, handler)
                });

            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });
            Assert.True(composition.Success);

            var result = composition.Contract!.Dispatch(
                "engine.observe",
                new ContractVersionRange(1, 0, 1),
                EmptyRequest());

            Assert.True(result.Success);
            Assert.Equal("1.1", result.Data!["value"]);
        }

        [Fact]
        public void CompatibilityRulesDistinguishAdditiveFromBreakingChange()
        {
            var v10 = FixtureDefinition("fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0), false);
            var v11 = FixtureDefinition("fixture.engine", "engine", "engine.observe", new ContractVersion(1, 1), true);
            var additive = ContractCompatibility.Compare(v10, v11);

            Assert.Equal(CompatibilityKind.Additive, additive.Kind);

            var breaking = new CapabilityDefinition(
                new CapabilityKey("engine.observe", new ContractVersion(1, 1)),
                v10.Provider,
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["target"] = LogicalReference()
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
            var definition = FixtureDefinition("fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0), true);
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

        private static IReadOnlyDictionary<string, object?> EmptyRequest()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }

        internal static CanonicalProviderContribution FixtureProvider(
            string providerId,
            string capabilityNamespace,
            string capabilityName,
            ContractVersion version)
        {
            var definition = FixtureDefinition(providerId, capabilityNamespace, capabilityName, version, true);
            return new CanonicalProviderContribution(
                new ProviderDescriptor(providerId, ProviderKind.Scoped, "engine", new[] { capabilityNamespace }),
                new[] { definition },
                new[] { new CapabilityRoute(providerId, definition.Key, new FixtureEngineHandler()) });
        }

        internal static CapabilityDefinition FixtureDefinition(
            string providerId,
            string capabilityNamespace,
            string capabilityName,
            ContractVersion version,
            bool includeOptionalTarget)
        {
            var requestProperties = new Dictionary<string, SchemaNode>(StringComparer.Ordinal);
            if (includeOptionalTarget)
            {
                requestProperties["target"] = LogicalReference();
            }

            return new CapabilityDefinition(
                new CapabilityKey(capabilityName, version),
                new ProviderMetadata(providerId, ProviderKind.Scoped, "engine", capabilityNamespace),
                new JsonSchemaDocument(SchemaNode.Object(requestProperties)),
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["value"] = SchemaNode.String()
                    },
                    new[] { "value" })),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                new[] { "provider-available" },
                new[] { "observation-returned" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.PublicRead,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Required),
                new CostSemantics(1));
        }

        internal static SchemaNode LogicalReference()
        {
            return SchemaNode.String(
                format: "arkus-logical-reference",
                logicalReferenceNamespace: "fixture.engine.entity");
        }
    }

    public sealed class Hk01SelfAttackTests
    {
        [Fact]
        public void RegisteredDispatchableButUndiscoveredTurnsProjectionConformanceRed()
        {
            var contract = ComposeWithFixture();
            var projection = new CanonicalContractProjection(new[] { contract.Definitions[0] });

            var issues = CanonicalProjectionConformance.Compare(contract.Definitions, projection);

            Assert.Contains(issues, issue => issue.Code == "projection.omitted_capability");
        }

        [Fact]
        public void DiscoveredCapabilityMissingSuccessOrErrorSchemaFailsClosed()
        {
            var valid = Hk01CanonicalContractTests.FixtureDefinition(
                "fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0), true);
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
                new[] { new CapabilityRoute("fixture.engine", invalid.Key, new FixtureEngineHandler()) });
            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "contract.missing_schema");
        }

        [Fact]
        public void SchemaDispatcherMismatchFailsComposition()
        {
            var definition = Hk01CanonicalContractTests.FixtureDefinition(
                "fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0), true);
            var wrongKey = new CapabilityKey("engine.other", new ContractVersion(1, 0));
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { definition },
                new[] { new CapabilityRoute("fixture.engine", wrongKey, new FixtureEngineHandler()) });

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.definition_without_route");
            Assert.Contains(result.Issues, issue => issue.Code == "composition.route_without_definition");
        }

        [Fact]
        public void UnsupportedContractVersionFailsBeforeHandlerInvocation()
        {
            FixtureEngineHandler.InvocationCount = 0;
            var contract = ComposeWithFixture();

            var result = contract.Dispatch(
                "engine.observe",
                ContractVersionRange.Exact(new ContractVersion(9, 0)),
                EmptyRequest());

            Assert.False(result.Success);
            Assert.Equal("contract.unsupported_version", result.Error!.MachineCode);
            Assert.Equal(0, FixtureEngineHandler.InvocationCount);
        }

        [Fact]
        public void UnknownExtraRouteIsFoundByIndependentUniverse()
        {
            var contract = BaseContract.Compose();
            var universe = RouteUniverse.Enumerate(
                typeof(OrphanHandler).Assembly,
                new[] { "fixture.orphan" });

            var report = CanonicalContractConformance.Evaluate(contract, universe);

            Assert.False(report.IsConformant);
            Assert.Contains(report.Issues, issue => issue.Code == "conformance.extra-public-route");
        }

        [Fact]
        public void ImplementationTypeOrTransportSpecificSchemaMetadataIsRejected()
        {
            var valid = Hk01CanonicalContractTests.FixtureDefinition(
                "fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0), true);
            var implementationTypeSchema = new JsonSchemaDocument(SchemaNode.String(format: "dotnet-type"));
            var invalid = CopyWithRequest(valid, implementationTypeSchema);

            var issues = CanonicalContractValidator.Validate(invalid);

            Assert.Contains(issues, issue => issue.Code == "contract.schema.non_portable_format");

            var transportSchema = new JsonSchemaDocument(SchemaNode.String(format: "mcp-tool"));
            var transportIssues = CanonicalContractValidator.Validate(CopyWithRequest(valid, transportSchema));
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
            var contract = ComposeWithFixture();
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
        public void DeletingRegistryMetadataCannotShrinkIndependentRouteProofUniverse()
        {
            var emptyBase = new CanonicalProviderContribution(
                new ProviderDescriptor("arkus.base", ProviderKind.Base, "base", new[] { "system" }),
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());
            var composition = ContractComposer.Compose(emptyBase);
            Assert.True(composition.Success);

            var universe = RouteUniverse.Enumerate(
                typeof(SystemDescribeHandler).Assembly,
                new[] { "arkus.base" });
            var report = CanonicalContractConformance.Evaluate(composition.Contract!, universe);

            Assert.False(report.IsConformant);
            Assert.Contains(report.Issues, issue => issue.Code == "conformance.extra-public-route");
        }

        [Fact]
        public void SyntheticScopedPublicRouteWithoutCanonicalCompositionIsRejectedByConformance()
        {
            var contract = BaseContract.Compose();
            var universe = RouteUniverse.Enumerate(
                typeof(FixtureEngineHandler).Assembly,
                new[] { "fixture.engine" });

            var report = CanonicalContractConformance.Evaluate(contract, universe);

            Assert.False(report.IsConformant);
            Assert.Contains(report.Issues, issue => issue.Code == "conformance.extra-public-route");
        }

        [Fact]
        public void DuplicateOrOverlappingScopedNamespaceFailsClosed()
        {
            var first = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.a", ProviderKind.Scoped, "engine-a", new[] { "engine" }),
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());
            var second = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.b", ProviderKind.Scoped, "engine-b", new[] { "engine.sub" }),
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { first, second });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.namespace_conflict");
        }

        [Fact]
        public void DuplicateScopedCapabilityIdentityFailsClosed()
        {
            var firstDefinition = Hk01CanonicalContractTests.FixtureDefinition(
                "fixture.a", "engine", "engine.observe", new ContractVersion(1, 0), true);
            var secondDefinition = Hk01CanonicalContractTests.FixtureDefinition(
                "fixture.b", "engine", "engine.observe", new ContractVersion(1, 0), true);
            var first = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.a", ProviderKind.Scoped, "engine-a", new[] { "engine" }),
                new[] { firstDefinition },
                new[] { new CapabilityRoute("fixture.a", firstDefinition.Key, new FixtureEngineHandler()) });
            var second = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.b", ProviderKind.Scoped, "engine-b", new[] { "engine" }),
                new[] { secondDefinition },
                new[] { new CapabilityRoute("fixture.b", secondDefinition.Key, new FixtureEngineHandler()) });

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { first, second });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.duplicate_capability");
        }

        [Fact]
        public void ScopedSchemaCannotRequireEngineRuntimeImplementationType()
        {
            var valid = Hk01CanonicalContractTests.FixtureDefinition(
                "fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0), true);
            var runtimeTypeRequest = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["target"] = SchemaNode.String(format: "dotnet-type")
                },
                new[] { "target" }));
            var invalid = CopyWithRequest(valid, runtimeTypeRequest);
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { invalid },
                new[] { new CapabilityRoute("fixture.engine", invalid.Key, new FixtureEngineHandler()) });

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "contract.schema.non_portable_format");
        }

        private static CapabilityDefinition CopyWithRequest(CapabilityDefinition source, JsonSchemaDocument requestSchema)
        {
            return new CapabilityDefinition(
                source.Key,
                source.Provider,
                requestSchema,
                source.SuccessSchema,
                source.ErrorSchema,
                source.SideEffect,
                source.Determinism,
                source.Preconditions,
                source.Postconditions,
                source.Concurrency,
                source.Idempotency,
                source.Batching,
                source.Repair,
                source.Policy,
                source.Cost);
        }

        private static ComposedContract ComposeWithFixture()
        {
            var result = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[]
                {
                    Hk01CanonicalContractTests.FixtureProvider(
                        "fixture.engine", "engine", "engine.observe", new ContractVersion(1, 0))
                });
            Assert.True(result.Success);
            return result.Contract!;
        }

        private static IReadOnlyDictionary<string, object?> EmptyRequest()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }
    }

    [PublicCapabilityRoute("fixture.engine", "engine.observe", "1.0")]
    public sealed class FixtureEngineHandler : ICanonicalCapabilityHandler
    {
        public static int InvocationCount { get; set; }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            InvocationCount++;
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = context.Definition.Key.Version.ToString()
            });
        }
    }

    [PublicCapabilityRoute("fixture.orphan", "orphan.route", "1.0")]
    public sealed class OrphanHandler : ICanonicalCapabilityHandler
    {
        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal));
        }
    }
}
