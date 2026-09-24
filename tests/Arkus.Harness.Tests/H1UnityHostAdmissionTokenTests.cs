using System;
using System.Collections.Generic;
using System.Reflection;
using Arkus.Harness.H1HostPolicyFixture;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityHostAdmissionTokenTests
    {
        private const string ResourceNamespace = "ref.arkus.unity-host.asset";

        [Fact]
        public void AdmittedProjectionRetainsExactBootstrappedWorkspaceAndReviewedGrant()
        {
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { HostContribution() });
            Assert.True(composition.Success);

            var workspace = UnityProjectWorkspaceAuthority.ForArkusUnityProject();
            var grant = new UnityHostCapabilityGrant(
                new CapabilityKey("unity.host.inspect", new ContractVersion(1, 0)),
                UnityProjectWorkspaceAuthority.ManagedAssetsRootId,
                ResourceNamespace,
                UnityHostResourceClass.ManagedAsset,
                UnityHostTimeClass.BoundedRead);

            using var projection = H1UnityHostCapabilityPolicy.CreateProjection(
                composition.Contract!, workspace, new[] { grant });

            var field = typeof(NeutralProjectionService).GetField(
                "_h1Admission", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            var admission = field!.GetValue(projection);
            Assert.NotNull(admission);

            var workspaceProperty = admission!.GetType().GetProperty(
                "Workspace", BindingFlags.Instance | BindingFlags.NonPublic);
            var grantsProperty = admission.GetType().GetProperty(
                "Grants", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(workspaceProperty);
            Assert.NotNull(grantsProperty);

            Assert.Same(workspace, workspaceProperty!.GetValue(admission));
            var retained = Assert.IsAssignableFrom<IReadOnlyDictionary<CapabilityKey, UnityHostCapabilityGrant>>(
                grantsProperty!.GetValue(admission));
            Assert.Single(retained);
            Assert.True(retained.TryGetValue(grant.Capability, out var retainedGrant));
            Assert.Same(grant, retainedGrant);
        }

        private static CanonicalProviderContribution HostContribution()
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
                new JsonSchemaDocument(SchemaNode.Object(
                    new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                    {
                        ["value"] = SchemaNode.String(new[] { "ok" })
                    },
                    new[] { "value" })),
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
    }
}
