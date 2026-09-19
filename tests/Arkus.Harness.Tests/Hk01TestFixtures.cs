using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    internal static class Hk01TestFixtures
    {
        internal static IReadOnlyDictionary<string, object?> EmptyRequest()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }

        internal static CanonicalProviderContribution FixtureProvider()
        {
            var definition = FixtureDefinition(new ContractVersion(1, 0), true);
            return new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { definition },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });
        }

        internal static IReadOnlyList<CanonicalProviderContribution> CompleteFixtureProviders()
        {
            var engineV10 = FixtureDefinition(new ContractVersion(1, 0), true);
            var engineV11 = FixtureDefinition(new ContractVersion(1, 1), true);
            var engine = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { engineV10, engineV11 },
                new[]
                {
                    CapabilityRoute.FromHandler(new FixtureEngineHandler()),
                    CapabilityRoute.FromHandler(new FixtureEngineV11Handler())
                });

            var orphanDefinition = DefinitionForProvider(
                "fixture.orphan",
                "orphan",
                "orphan.route",
                new ContractVersion(1, 0),
                false);
            var orphan = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.orphan", ProviderKind.Scoped, "engine", new[] { "orphan" }),
                new[] { orphanDefinition },
                new[] { CapabilityRoute.FromHandler(new OrphanHandler()) });

            return new[] { engine, orphan };
        }

        internal static CapabilityDefinition FixtureDefinition(ContractVersion version, bool includeOptionalTarget)
        {
            return DefinitionForProvider(
                "fixture.engine",
                "engine",
                "engine.observe",
                version,
                includeOptionalTarget);
        }

        internal static CapabilityDefinition DefinitionForProvider(
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

        internal static CapabilityDefinition CopyWithRequest(CapabilityDefinition source, JsonSchemaDocument requestSchema)
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

        internal static ComposedContract ComposeWithFixture()
        {
            var result = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { FixtureProvider() });
            Assert.True(result.Success);
            return result.Contract!;
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

    [PublicCapabilityRoute("fixture.engine", "engine.observe", "1.1")]
    public sealed class FixtureEngineV11Handler : ICanonicalCapabilityHandler
    {
        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
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
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = "orphan"
            });
        }
    }
}
