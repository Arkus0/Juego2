using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk01SelfAttackTestsAdditional
    {
        [Fact]
        public void DuplicateScopedScopeWithDisjointNamespacesFailsClosed()
        {
            var first = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.scope-a", ProviderKind.Scoped, "shared-scope", new[] { "alpha" }),
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());
            var second = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.scope-b", ProviderKind.Scoped, "shared-scope", new[] { "beta" }),
                System.Array.Empty<CapabilityDefinition>(),
                System.Array.Empty<CapabilityRoute>());

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { first, second });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "composition.scope_conflict");
            Assert.DoesNotContain(result.Issues, issue => issue.Code == "composition.namespace_conflict");
        }

        [Fact]
        public void AnySchemaCannotAdmitClrOrEngineObjectAtCanonicalBoundary()
        {
            var valid = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var permissiveRequest = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["target"] = SchemaNode.Any()
                },
                new[] { "target" }));
            var definition = Hk01TestFixtures.CopyWithRequest(valid, permissiveRequest);
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { definition },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });
            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });
            Assert.True(composition.Success);

            FixtureEngineHandler.InvocationCount = 0;
            var result = composition.Contract!.Dispatch(
                "engine.observe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["target"] = new RuntimeOnlyReference()
                });

            Assert.False(result.Success);
            Assert.Equal("contract.invalid_request", result.Error!.MachineCode);
            Assert.Equal("portable.non_json_value", result.Error.Context["schemaCode"]);
            Assert.Equal(0, FixtureEngineHandler.InvocationCount);
        }

        [Fact]
        public void MissingMandatoryPolicyMetadataFailsComposition()
        {
            var valid = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var invalid = new CapabilityDefinition(
                valid.Key,
                valid.Provider,
                valid.RequestSchema,
                valid.SuccessSchema,
                valid.ErrorSchema,
                valid.SideEffect,
                valid.Determinism,
                valid.Preconditions,
                valid.Postconditions,
                valid.Concurrency,
                valid.Idempotency,
                valid.Batching,
                valid.Repair,
                null,
                valid.Cost);
            var scoped = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { invalid },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });

            var result = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { scoped });

            Assert.False(result.Success);
            Assert.Contains(result.Issues, issue => issue.Code == "contract.missing_policy");
        }

        [Fact]
        public void SemanticChangeWithoutVersionIncrementIsBreaking()
        {
            var previous = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), false);
            var sameVersionWithNewOptionalField = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);

            var result = ContractCompatibility.Compare(previous, sameVersionWithNewOptionalField);

            Assert.Equal(CompatibilityKind.Breaking, result.Kind);
        }

        private sealed class RuntimeOnlyReference
        {
        }
    }
}
