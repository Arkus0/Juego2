using System;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk09ATransportPolicyBoundaryTests
    {
        [Fact]
        public void TransportPathCannotProjectCompositionThatSkippedHostCapabilityAdmission()
        {
            var accepted = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var externalDefinition = new CapabilityDefinition(
                accepted.Key,
                accepted.Provider,
                accepted.RequestSchema,
                accepted.SuccessSchema,
                accepted.ErrorSchema,
                SideEffectClass.ExternalReversible,
                accepted.Determinism,
                accepted.Preconditions,
                accepted.Postconditions,
                accepted.Concurrency,
                accepted.Idempotency,
                accepted.Batching,
                accepted.Repair,
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.None,
                    ProvenanceRequirement.Minimal),
                accepted.Cost);
            var externalProvider = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { externalDefinition },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });

            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { externalProvider });

            Assert.True(composition.Success);
            Assert.NotNull(composition.Contract);
            FixtureEngineHandler.InvocationCount = 0;

            var rejection = Assert.Throws<InvalidOperationException>(
                () => new NeutralProjectionService(composition.Contract!));

            Assert.Contains(H0HostCapabilityPolicy.ExternalEffectCode, rejection.Message, StringComparison.Ordinal);
            Assert.Equal(0, FixtureEngineHandler.InvocationCount);
        }
    }
}
