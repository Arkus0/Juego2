using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08BHistoryTruthTests
    {
        [Fact]
        public void GappedRequiredHistoryCannotBeMisclassifiedAsRecoverable()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var service = new GappedHistoryMutationService(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(service), service);

            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk08b.history-writer",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.history-writer")));

            var stale = contract.Dispatch(
                WorldMutationContract.PlanName,
                Hk04TransactionalMutationTests.ExactVersion(),
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk08b.history-stale",
                    Hk04TransactionalMutationTests.PutObject("node.child", "fixture.history-client", "node.root")));

            Assert.False(stale.Success);
            Assert.Equal("world.change.stale_revision", stale.Error!.MachineCode);
            var recovery = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(
                stale.Error.Context["recovery"]);
            Assert.Equal(WorldConflictRecoveryContract.BoundedReinspectionRequired, recovery["disposition"]);
            var proof = Hk04TransactionalMutationTests.Map(recovery, "historyProof");
            Assert.Equal(WorldConflictRecoveryContract.RequiredHistoryUnavailable, proof["reason"]);
            Assert.Empty(Hk04TransactionalMutationTests.List(recovery, "changedResources"));
            Assert.Empty(Hk04TransactionalMutationTests.List(recovery, "currentResources"));
        }

        /// <summary>
        /// Causal self-attack: the wrapped authoring session is internally consistent, but the
        /// provenance reader presented to Runtime corrupts the first sequence number. A recovery
        /// implementation that merely finds the expected base, or that ignores history gaps, would
        /// incorrectly return a convincing changed-resource delta.
        /// </summary>
        private sealed class GappedHistoryMutationService :
            IWorldMutationService,
            IWorldProvenanceService,
            IWorldStateSource,
            ICanonicalWorldMutationCommitter
        {
            private readonly TransactionalWorldAuthoringSession _inner;

            public GappedHistoryMutationService(WorldState initial)
            {
                _inner = new TransactionalWorldAuthoringSession(initial);
            }

            public WorldState Current => _inner.Current;

            public CapabilityInvocationResult Plan(IReadOnlyDictionary<string, object?> request) =>
                _inner.Plan(request);

            public CapabilityInvocationResult DryRun(IReadOnlyDictionary<string, object?> request) =>
                _inner.DryRun(request);

            CapabilityInvocationResult ICanonicalWorldMutationCommitter.Apply(
                IReadOnlyDictionary<string, object?> request,
                InvocationResourceBudget resourceBudget) =>
                ((ICanonicalWorldMutationCommitter)_inner).Apply(request, resourceBudget);

            public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request)
            {
                var original = ((IWorldProvenanceService)_inner).ReadJournal(request);
                if (!original.Success || original.Data == null) return original;

                var data = new Dictionary<string, object?>(original.Data, StringComparer.Ordinal);
                if (!data.TryGetValue("entries", out var rawEntries) ||
                    !(rawEntries is IReadOnlyList<object?> entries) ||
                    entries.Count == 0)
                {
                    return original;
                }

                var attackedEntries = new List<object?>(entries.Count);
                for (var index = 0; index < entries.Count; index++)
                {
                    var entry = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(entries[index]);
                    var attacked = new Dictionary<string, object?>(entry, StringComparer.Ordinal);
                    if (index == 0) attacked["sequence"] = 2L;
                    attackedEntries.Add(new ReadOnlyDictionary<string, object?>(attacked));
                }

                data["entries"] = attackedEntries.AsReadOnly();
                return CapabilityInvocationResult.Succeeded(
                    new ReadOnlyDictionary<string, object?>(data));
            }
        }
    }
}
