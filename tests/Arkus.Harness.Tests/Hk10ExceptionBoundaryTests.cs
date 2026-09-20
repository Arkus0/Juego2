using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.Game.Authoring;
using Arkus.Game.Validation;
using Arkus.Game.World;
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
            var contract = CanonicalWorldContract.Compose(
                new WorldInspectionService(new ThrowingWorldStateSource()));

            var result = contract.Dispatch(
                WorldInspectionContract.SummaryName,
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal));

            AssertDefinedFailure(result, "world.summary@1.0", "HK10_INTERNAL_SENTINEL");
        }

        [Fact]
        public void ThrownValidatorFailureBecomesDefinedCanonicalErrorWithoutInternalMessageLeak()
        {
            var definition = Assert.Single(WorldValidationContract.CreateDefinitions().Where(candidate =>
                string.Equals(candidate.Key.Name, WorldValidationContract.CurrentName, StringComparison.Ordinal)));
            var route = CapabilityRoute.FromHandler(
                new WorldValidationCurrentHandler(new ThrowingValidationService()));
            var contract = new ComposedContract(
                new Dictionary<CapabilityKey, CapabilityDefinition>
                {
                    [definition.Key] = definition
                },
                new Dictionary<CapabilityKey, CapabilityRoute>
                {
                    [definition.Key] = route
                });

            var result = contract.Dispatch(
                WorldValidationContract.CurrentName,
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal));

            AssertDefinedFailure(result, "world.validation.current@1.0", "HK10_VALIDATOR_SENTINEL");
        }

        private static void AssertDefinedFailure(
            CapabilityInvocationResult result,
            string capability,
            string internalSentinel)
        {
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("contract.handler_failure", result.Error!.MachineCode);
            Assert.Equal("$", result.Error.Path);
            Assert.False(result.Error.Retryable);
            Assert.Equal(capability, result.Error.Context["capability"]);
            Assert.Equal("System.InvalidOperationException", result.Error.Context["exceptionType"]);
            Assert.False((bool)result.Error.Context["publicationCommitted"]!);
            Assert.DoesNotContain(internalSentinel, result.Error.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(internalSentinel, result.Error.RepairHint, StringComparison.Ordinal);
        }

        private sealed class ThrowingWorldStateSource : IWorldStateSource
        {
            public WorldState Current => throw new InvalidOperationException("HK10_INTERNAL_SENTINEL");
        }

        private sealed class ThrowingValidationService : IWorldValidationService
        {
            public CapabilityInvocationResult ValidateCurrent(IReadOnlyDictionary<string, object?> request)
            {
                throw new InvalidOperationException("HK10_VALIDATOR_SENTINEL");
            }

            public CapabilityInvocationResult ValidateProposed(IReadOnlyDictionary<string, object?> request)
            {
                throw new InvalidOperationException("HK10_VALIDATOR_SENTINEL");
            }
        }
    }
}
