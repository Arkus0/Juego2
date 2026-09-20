using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08ANegativeConformanceTests
    {
        private static readonly ContractVersionRange ExactV1 =
            ContractVersionRange.Exact(new ContractVersion(1, 0));
        private static readonly ContractVersionRange ExactV2 =
            ContractVersionRange.Exact(new ContractVersion(2, 0));

        [Fact]
        public void BatchIntegrityOracleTurnsRedIfSuccessfulBatchOmitsProvenance()
        {
            var initial = EmptyWorld("world.hk08a.provenance-negative");
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var request = MutationRequest(
                initial,
                "request.hk08a.provenance-negative",
                new object?[] { PutObject("item.a"), PutObject("item.b") });

            var applied = contract.Dispatch(WorldMutationContract.ApplyName, ExactV1, request);
            Assert.True(applied.Success);
            Assert.True((bool)applied.Data!["persisted"]!);
            var journal = contract.Dispatch(WorldProvenanceContract.ReadName, ExactV1, Empty());
            Assert.True(journal.Success);
            var journalCount = Convert.ToInt32(journal.Data!["entryCount"]);

            Assert.Empty(BatchIntegrityIssues(
                reportedPersisted: true,
                revisionDelta: session.Current.Revision - initial.Revision,
                journalDelta: journalCount));

            // Controlled defect injection: keep the successful/persisted mutation observation but
            // remove its required provenance effect. The same independent observation oracle is RED.
            Assert.Contains(
                "missing-provenance",
                BatchIntegrityIssues(
                    reportedPersisted: true,
                    revisionDelta: session.Current.Revision - initial.Revision,
                    journalDelta: 0));
        }

        [Fact]
        public void JournalReconstructionOracleTurnsRedForDuplicateAndMissingSequence()
        {
            var initial = EmptyWorld("world.hk08a.pagination-negative");
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            for (var index = 0; index < 3; index++)
            {
                var current = session.Current;
                var applied = contract.Dispatch(
                    WorldMutationContract.ApplyName,
                    ExactV1,
                    MutationRequest(
                        current,
                        "request.hk08a.pagination-negative." + index,
                        new object?[] { PutObject("page.item." + index) }));
                Assert.True(applied.Success);
            }

            var anchor = session.Current;
            var first = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV2,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["revision"] = anchor.Revision,
                    ["hash"] = CanonicalWorldStateCodec.ComputeContentHash(anchor),
                    ["limit"] = 2
                });
            Assert.True(first.Success);
            var firstEntries = Entries(first.Data!);
            var cursor = Assert.IsType<string>(first.Data!["nextCursor"]);

            var second = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV2,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = 2,
                    ["cursor"] = cursor
                });
            Assert.True(second.Success);
            var reconstructed = firstEntries.Concat(Entries(second.Data!)).ToArray();
            Assert.Empty(JournalSequenceIssues(reconstructed));

            var duplicate = new[] { reconstructed[0], reconstructed[1], reconstructed[1] };
            Assert.Contains("duplicate-entry", JournalSequenceIssues(duplicate));

            var missing = new[] { reconstructed[0], reconstructed[2] };
            Assert.Contains("sequence-gap", JournalSequenceIssues(missing));
        }

        private static IReadOnlyList<string> BatchIntegrityIssues(
            bool reportedPersisted,
            long revisionDelta,
            int journalDelta)
        {
            var issues = new List<string>();
            if (reportedPersisted && revisionDelta != 1) issues.Add("non-atomic-revision-publication");
            if (reportedPersisted && journalDelta != 1) issues.Add("missing-provenance");
            return issues.AsReadOnly();
        }

        private static IReadOnlyList<string> JournalSequenceIssues(
            IReadOnlyList<IReadOnlyDictionary<string, object?>> entries)
        {
            var issues = new List<string>();
            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < entries.Count; index++)
            {
                var entry = entries[index];
                var id = (string)entry["entryId"]!;
                if (!ids.Add(id)) issues.Add("duplicate-entry");
                var sequence = Convert.ToInt64(entry["sequence"]);
                if (sequence != index + 1L) issues.Add("sequence-gap");
            }
            return issues.Distinct(StringComparer.Ordinal).ToArray();
        }

        private static IReadOnlyList<IReadOnlyDictionary<string, object?>> Entries(
            IReadOnlyDictionary<string, object?> journal)
        {
            return ((IReadOnlyList<object?>)journal["entries"]!)
                .Cast<IReadOnlyDictionary<string, object?>>()
                .ToArray();
        }

        private static IReadOnlyDictionary<string, object?> MutationRequest(
            WorldState state,
            string idempotencyKey,
            IReadOnlyList<object?> operations)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = idempotencyKey,
                ["expectedRevision"] = state.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(state),
                ["operations"] = operations
            };
        }

        private static IReadOnlyDictionary<string, object?> PutObject(string id)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-object",
                ["id"] = id,
                ["typeId"] = "fixture.hk08a.negative-item",
                ["references"] = Array.Empty<object?>()
            };
        }

        private static WorldState EmptyWorld(string id)
        {
            return new WorldState(new WorldId(id), 0, Array.Empty<WorldObject>());
        }

        private static IReadOnlyDictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }
    }
}
