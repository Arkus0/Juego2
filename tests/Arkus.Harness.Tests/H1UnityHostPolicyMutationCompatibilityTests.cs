using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arkus.Game.Authoring;
using Arkus.Game.World;
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
        public async Task H1ProfilePreservesRealH0CanonicalMutationAlongsideUnityRead()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

            var composition = ContractComposer.Compose(
                CanonicalWorldContract.CreateContribution(new WorldInspectionService(session), session),
                new[] { UnityReadContribution() });
            Assert.True(
                composition.Success,
                string.Join("; ", composition.Issues.Select(issue => issue.Code + ":" + issue.Message)));

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

            var mutationRequest = Hk04TransactionalMutationTests.Request(
                initial,
                "h1-host-policy.real-h0-mutation",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.h1-compatible"));
            var mutation = await projection.InvokeAsync(new NeutralProjectionRequest(
                "h0-mutation",
                WorldMutationContract.ApplyName,
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                mutationRequest));
            var unityRead = await projection.InvokeAsync(new NeutralProjectionRequest(
                "unity-read",
                "unity.host.inspect",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["resource"] = "asset.potes.market-stall"
                }));

            Assert.True(mutation.Success, mutation.Error == null ? "unknown mutation failure" : mutation.Error.MachineCode);
            Assert.True(unityRead.Success);
            Assert.Equal(initial.Revision + 1, session.Current.Revision);
            Assert.NotEqual(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Contains(session.Current.Objects, item =>
                item.Id.Value == "node.peer" && item.TypeId.Value == "fixture.h1-compatible");
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
