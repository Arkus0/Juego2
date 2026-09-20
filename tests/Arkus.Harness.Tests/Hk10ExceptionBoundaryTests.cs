using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
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

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("contract.handler_failure", result.Error!.MachineCode);
            Assert.Equal("$", result.Error.Path);
            Assert.False(result.Error.Retryable);
            Assert.Equal("world.summary@1.0", result.Error.Context["capability"]);
            Assert.Equal("System.InvalidOperationException", result.Error.Context["exceptionType"]);
            Assert.False((bool)result.Error.Context["publicationCommitted"]!);
            Assert.DoesNotContain("HK10_INTERNAL_SENTINEL", result.Error.Message, StringComparison.Ordinal);
            Assert.DoesNotContain("HK10_INTERNAL_SENTINEL", result.Error.RepairHint, StringComparison.Ordinal);
        }

        private sealed class ThrowingWorldStateSource : IWorldStateSource
        {
            public WorldState Current => throw new InvalidOperationException("HK10_INTERNAL_SENTINEL");
        }
    }
}
