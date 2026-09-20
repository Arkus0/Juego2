using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk10ExceptionBoundaryTests
    {
        [Fact]
        public void ThrownHandlerFailureBecomesDefinedCanonicalErrorWithoutInternalMessageLeak()
        {
            var provider = new ProviderMetadata(
                "fixture.hk10",
                ProviderKind.Scoped,
                "hk10",
                "fixture.hk10");
            var definition = new CapabilityDefinition(
                new CapabilityKey("fixture.hk10.throw", new ContractVersion(1, 0)),
                provider,
                CanonicalContractSchemas.EmptyObject(),
                CanonicalContractSchemas.EmptyObject(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                Array.Empty<string>(),
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.PublicRead,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Required),
                new CostSemantics(1, "HK10 controlled thrown-handler boundary fixture"));
            var contribution = new CanonicalProviderContribution(
                new ProviderDescriptor(
                    "fixture.hk10",
                    ProviderKind.Scoped,
                    "hk10",
                    new[] { "fixture.hk10" }),
                new[] { definition },
                new[] { CapabilityRoute.FromHandler(new ThrowingHandler()) });

            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { contribution });
            Assert.True(composition.Success, string.Join(";", composition.Issues));
            Assert.NotNull(composition.Contract);

            var result = composition.Contract!.Dispatch(
                "fixture.hk10.throw",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal));

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("contract.handler_failure", result.Error!.MachineCode);
            Assert.Equal("$", result.Error.Path);
            Assert.False(result.Error.Retryable);
            Assert.Equal("fixture.hk10.throw@1.0", result.Error.Context["capability"]);
            Assert.Equal("System.InvalidOperationException", result.Error.Context["exceptionType"]);
            Assert.False((bool)result.Error.Context["publicationCommitted"]!);
            Assert.DoesNotContain("HK10_INTERNAL_SENTINEL", result.Error.Message, StringComparison.Ordinal);
            Assert.DoesNotContain("HK10_INTERNAL_SENTINEL", result.Error.RepairHint, StringComparison.Ordinal);
            Assert.Empty(definition.ErrorSchema!.ValidateValue(result.Error.ToData()));
        }

        [PublicCapabilityRoute("fixture.hk10", "fixture.hk10.throw", "1.0")]
        private sealed class ThrowingHandler : ICanonicalCapabilityHandler
        {
            public CapabilityInvocationResult Invoke(
                CapabilityInvocationContext context,
                IReadOnlyDictionary<string, object?> request)
            {
                throw new InvalidOperationException("HK10_INTERNAL_SENTINEL");
            }
        }
    }
}
