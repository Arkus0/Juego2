using System;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk09AMetadataTruthfulnessTests
    {
        [Fact]
        public void CanonicalStateChangeCannotMasqueradeAsPublicReadPrivilege()
        {
            var definition = Definition(PrivilegeClass.PublicRead, ProvenanceRequirement.Required);
            var issues = H0HostCapabilityPolicy.Validate(new[] { definition });
            Assert.Contains(issues, issue => issue.Code == H0HostCapabilityPolicy.MetadataMismatchCode);
        }

        [Fact]
        public void CanonicalStateChangeCannotDropRequiredProvenanceMetadata()
        {
            var definition = Definition(PrivilegeClass.Authoring, ProvenanceRequirement.Minimal);
            var issues = H0HostCapabilityPolicy.Validate(new[] { definition });
            Assert.Contains(issues, issue => issue.Code == H0HostCapabilityPolicy.MetadataMismatchCode);
        }

        private static CapabilityDefinition Definition(
            PrivilegeClass privilege,
            ProvenanceRequirement provenance)
        {
            return new CapabilityDefinition(
                new CapabilityKey("test.hk09a.metadata", new ContractVersion(1, 0)),
                new ProviderMetadata("test.hk09a", ProviderKind.Scoped, "hk09a", "test"),
                null,
                null,
                null,
                SideEffectClass.CanonicalMutation,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                Array.Empty<string>(),
                null,
                null,
                null,
                null,
                new PolicySemantics(
                    privilege,
                    TransactionRequirement.CanonicalTransaction,
                    provenance));
        }
    }
}
