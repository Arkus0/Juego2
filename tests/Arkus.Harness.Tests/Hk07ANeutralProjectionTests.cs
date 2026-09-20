using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk07ANeutralProjectionTests
    {
        [Fact]
        public void ProductionHostProjectsTheCompleteCanonicalWorldComposition()
        {
            using var production = ProductionHarnessHost.Create();

            var initial = new WorldState(
                new WorldId(ProductionHarnessHost.InitialWorldId),
                0,
                Array.Empty<WorldObject>());
            var session = new PortableWorldAuthoringSession(initial);
            var independentlyComposed = CanonicalWorldContract.Compose(
                new WorldInspectionService(session),
                session);

            var issues = NeutralProjectionCompleteness.Compare(
                independentlyComposed.Definitions,
                NeutralProjectionCompleteness.KeysFrom(production));

            Assert.Empty(issues);
            Assert.Equal(
                independentlyComposed.Projection.ToData(),
                Invoke(production, "production.describe", "system.describe", Empty()).Result);
        }

        [Fact]
        public void SyntheticScopedCapabilityIsProjectedAndInvokedWithoutAdapterRegistry()
        {
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { Hk01TestFixtures.FixtureProvider() });
            Assert.True(composition.Success);
            using var projection = new NeutralProjectionService(composition.Contract!);

            var describe = Invoke(projection, "scoped.describe", "system.describe", Empty());
            Assert.True(describe.Success);
            var discovered = Hk04TransactionalMutationTests.List(describe.Result!, "capabilities");
            Assert.Contains(discovered, value =>
                value is IReadOnlyDictionary<string, object?> item &&
                Equals(item["name"], "engine.observe") &&
                Equals(item["version"], "1.0"));

            FixtureEngineHandler.InvocationCount = 0;
            var observed = Invoke(projection, "scoped.invoke", "engine.observe", Empty());
            Assert.True(observed.Success);
            Assert.Equal("1.0", observed.Result!["value"]);
            Assert.Equal(1, FixtureEngineHandler.InvocationCount);
        }

        [Fact]
        public void CompletenessOracleRejectsTransportOwnedBaseOnlyList()
        {
            var baseContract = BaseContract.Compose();
            var composed = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { Hk01TestFixtures.FixtureProvider() });
            Assert.True(composed.Success);

            var fixedBaseOnlyList = baseContract.Definitions.Select(value => value.Key).ToArray();
            var issues = NeutralProjectionCompleteness.Compare(
                composed.Contract!.Definitions,
                fixedBaseOnlyList);

            Assert.Contains(issues, issue =>
                issue.Code == "projection.omitted_capability" &&
                issue.Identity == "engine.observe@1.0");
        }

        [Fact]
        public void CancellationAndExpiredAdmissionNeverInvokeCanonicalHandler()
        {
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { Hk01TestFixtures.FixtureProvider() });
            Assert.True(composition.Success);
            using var projection = new NeutralProjectionService(composition.Contract!);
            FixtureEngineHandler.InvocationCount = 0;

            using var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            var cancelledOutcome = projection.InvokeAsync(
                Request("cancelled", "engine.observe", Empty()),
                cancelled.Token).GetAwaiter().GetResult();
            var timedOutOutcome = projection.InvokeAsync(
                Request("timed-out", "engine.observe", Empty(), 0)).GetAwaiter().GetResult();

            Assert.False(cancelledOutcome.Success);
            Assert.Equal(NeutralProjectionFailureKind.Cancelled, cancelledOutcome.FailureKind);
            Assert.Equal("projection.cancelled", cancelledOutcome.Error!.MachineCode);
            Assert.False(timedOutOutcome.Success);
            Assert.Equal(NeutralProjectionFailureKind.TimedOut, timedOutOutcome.FailureKind);
            Assert.Equal("projection.timeout", timedOutOutcome.Error!.MachineCode);
            Assert.Equal(0, FixtureEngineHandler.InvocationCount);
        }

        [Fact]
        public void CanonicalFailureMeaningIsPreservedByNeutralOutcome()
        {
            using var projection = new NeutralProjectionService(BaseContract.Compose());
            var outcome = Invoke(projection, "unknown", "missing.capability", Empty());

            Assert.False(outcome.Success);
            Assert.Equal(NeutralProjectionFailureKind.Canonical, outcome.FailureKind);
            Assert.Equal("contract.unknown_capability", outcome.Error!.MachineCode);
            Assert.Equal("$.capability", outcome.Error.Path);
            Assert.False(outcome.Error.Retryable);
        }

        [Fact]
        public void NeutralContractContainsNoReferenceTransportFramingFields()
        {
            var request = Request("neutral.shape", "system.describe", Empty(), 250);
            var data = request.ToData();

            Assert.Equal(
                new[]
                {
                    "acceptedVersions", "arguments", "capability", "projectionVersion",
                    "requestId", "timeoutMilliseconds"
                },
                data.Keys.OrderBy(value => value, StringComparer.Ordinal).ToArray());
            Assert.DoesNotContain(data.Keys, value =>
                value.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0 ||
                value.IndexOf("frame", StringComparison.OrdinalIgnoreCase) >= 0 ||
                value.IndexOf("stdin", StringComparison.OrdinalIgnoreCase) >= 0 ||
                value.IndexOf("stdout", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        [Fact]
        public void KernelAndNeutralProjectionDoNotReferenceCliOrJsonFramingAssemblies()
        {
            var runtimeReferences = typeof(ComposedContract).Assembly.GetReferencedAssemblies()
                .Select(value => value.Name).ToArray();
            var projectionReferences = typeof(NeutralProjectionService).Assembly.GetReferencedAssemblies()
                .Select(value => value.Name).ToArray();

            Assert.DoesNotContain("Arkus.Harness.Projection", runtimeReferences);
            Assert.DoesNotContain("Arkus.Harness.Cli", runtimeReferences);
            Assert.DoesNotContain("Arkus.Harness.Cli", projectionReferences);
            Assert.DoesNotContain("System.Text.Json", projectionReferences);
        }

        private static NeutralProjectionOutcome Invoke(
            NeutralProjectionService projection,
            string requestId,
            string capability,
            IReadOnlyDictionary<string, object?> arguments)
        {
            return projection.InvokeAsync(Request(requestId, capability, arguments))
                .GetAwaiter().GetResult();
        }

        private static NeutralProjectionRequest Request(
            string requestId,
            string capability,
            IReadOnlyDictionary<string, object?> arguments,
            int? timeoutMilliseconds = null)
        {
            return new NeutralProjectionRequest(
                requestId,
                capability,
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                arguments,
                timeoutMilliseconds);
        }

        private static IReadOnlyDictionary<string, object?> Empty() =>
            new Dictionary<string, object?>(StringComparer.Ordinal);
    }
}
