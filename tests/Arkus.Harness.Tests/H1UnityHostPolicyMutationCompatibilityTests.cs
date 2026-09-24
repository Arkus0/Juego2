using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arkus.Harness.H1HostPolicyFixture;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityHostPolicyMutationCompatibilityTests
    {
        private const string ResourceNamespace = "ref.arkus.unity-host.asset";

        [Fact]
        public async Task H1ProfilePreservesOrdinaryH0CanonicalMutationAlongsideUnityRead()
        {
            H0MutationFixtureHandler.InvocationCount = 0;
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { H0MutationContribution(), UnityReadContribution() });
            Assert.True(composition.Success, string.Join("; ", composition.Issues.Select(issue => issue.Code + ":" + issue.Message)));

            using var projection = H1UnityHostCapabilityPolicy.CreateProjection(
                composition.Contract!,
                UnityProjectWorkspaceAuthority.ForArkusUnityProject(),
                new[]
                {
                    new UnityHostCapabilityGrant(
                        new CapabilityKey("unity.host.inspect", new ContractVersion(1, 0)),
                        UnityProjectWorkspaceAuthority.ManagedAssetsRootId,
                        ResourceNamespace,
                        UnityHostResourceClass.ManagedAsset,
                        UnityHostTimeClass.BoundedRead)
                });

            var mutation = await projection.InvokeAsync(new NeutralProjectionRequest(
                "h0-mutation",
                "h0.fixture.mutate",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal)));
            var unityRead = await projection.InvokeAsync(new NeutralProjectionRequest(
                "unity-read",
                "unity.host.inspect",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["resource"] = "asset.potes.market-stall"
                }));

            Assert.True(mutation.Success);
            Assert.True(unityRead.Success);
            Assert.Equal(1, H0MutationFixtureHandler.InvocationCount);
        }

        private static CanonicalProviderContribution H0MutationContribution()
        {
            var definition = new CapabilityDefinition(
                new CapabilityKey("h0.fixture.mutate", new ContractVersion(1, 0)),
                new ProviderMetadata("fixture.h0", ProviderKind.Scoped, "h0-fixture", "h0.fixture"),
                new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal))),
                SuccessSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.CanonicalMutation,
                DeterminismClass.Deterministic,
                new[] { "canonical-state-available" },
                new[] { "canonical-mutation-dispatched" },
                new ConcurrencySemantics(ConcurrencyClass.Serialized),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.CanonicalTransaction,
                    ProvenanceRequirement.Required),
                new CostSemantics(1));

            return new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.h0", ProviderKind.Scoped, "h0-fixture", new[] { "h0.fixture" }),
                new[] { definition },
                new[] { CapabilityRoute.FromHandler(new H0MutationFixtureHandler()) });
        }

        private static CanonicalProviderContribution UnityReadContribution()
        {
            var definition = new CapabilityDefinition(
                new CapabilityKey("unity.host.inspect", new ContractVersion(1, 0)),
                new ProviderMetadata(
                    H1UnityHostCapabilityPolicy.HostProviderId,
                    ProviderKind.Scoped,
                    H1UnityHostCapabilityPolicy.HostScope,
                    H1UnityHostCapabilityPolicy.HostNamespace),
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["resource"] = SchemaNode.String(
                            format: "arkus-logical-reference",
                            logicalReferenceNamespace: ResourceNamespace)
                    },
                    new[] { "resource" })),
                SuccessSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.EnvironmentDependent,
                new[] { "unity-project-bootstrapped" },
                new[] { "observation-returned" },
                new ConcurrencySemantics(ConcurrencyClass.Serialized),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.None,
                    ProvenanceRequirement.Required),
                new CostSemantics(1));

            return new CanonicalProviderContribution(
                new ProviderDescriptor(
                    H1UnityHostCapabilityPolicy.HostProviderId,
                    ProviderKind.Scoped,
                    H1UnityHostCapabilityPolicy.HostScope,
                    new[] { H1UnityHostCapabilityPolicy.HostNamespace }),
                new[] { definition },
                new[] { CapabilityRoute.FromHandler(new UnityHostFixtureHandler()) });
        }

        private static JsonSchemaDocument SuccessSchema() => new JsonSchemaDocument(SchemaNode.Object(
            new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["value"] = SchemaNode.String()
            },
            new[] { "value" }));
    }
}
