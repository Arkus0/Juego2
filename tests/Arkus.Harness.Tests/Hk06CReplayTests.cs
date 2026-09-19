using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06CReplayTests
    {
        [Fact]
        public void AcceptedJournalReplaysFromCleanSnapshotBaseToIdenticalHashDiffAndEntryIdentities()
        {
            var source = BuildSource();
            var target = TargetFromSnapshot(source.BaseSnapshot);
            var targetContract = Compose(target);

            var replay = Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldReplayContract.ReplayName,
                ReplayRequest(target.Current, source.Journal));

            Assert.Equal(WorldReplayContract.ReplayResultSchemaId, replay.Data!["schemaId"]);
            Assert.Equal(2, replay.Data["replayedEntries"]);
            Assert.Equal("replayed-local-mutation-history", replay.Data["journalDisposition"]);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(source.Source.Current),
                CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(source.Source.Current.Revision, target.Current.Revision);

            var sourceEntries = Hk06AProvenanceJournalTests.Entries(source.Journal);
            var replayedJournal = Hk06AProvenanceJournalTests.ReadJournal(targetContract).Data!;
            var replayedEntries = Hk06AProvenanceJournalTests.Entries(replayedJournal);
            Assert.Equal(sourceEntries.Count, replayedEntries.Count);
            for (var index = 0; index < sourceEntries.Count; index++)
            {
                Assert.Equal(sourceEntries[index]["entryId"], replayedEntries[index]["entryId"]);
                Assert.Equal(
                    Hk04TransactionalMutationTests.Map(sourceEntries[index], "result")["hash"],
                    Hk04TransactionalMutationTests.Map(replayedEntries[index], "result")["hash"]);
            }

            var targetFinal = Snapshot(targetContract);
            var diff = Compare(targetContract, source.FinalSnapshot, targetFinal);
            Assert.True((bool)diff["sameAuthorableState"]!);
            Assert.Empty(Hk04TransactionalMutationTests.List(diff, "changes"));
            Assert.Equal(
                Hk04TransactionalMutationTests.Map(source.FinalSnapshot, "anchor")["hash"],
                Hk04TransactionalMutationTests.Map(targetFinal, "anchor")["hash"]);

            var definition = FindDefinition(targetContract, WorldReplayContract.ReplayName);
            Assert.Equal(SideEffectClass.CanonicalReplay, definition.SideEffect);
            Assert.Equal(TransactionRequirement.CanonicalReplay, definition.Policy!.TransactionRequirement);
            Assert.Equal(ProvenanceRequirement.Required, definition.Policy.ProvenanceRequirement);
            Assert.Empty(definition.SuccessSchema!.ValidateValue(replay.Data));
        }

        [Fact]
        public void CompatibilityPolicyIsMachineReadableAndFailsClosedForUnknownJournalOrSnapshotVersions()
        {
            var session = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var contract = Compose(session);
            var supported = Hk04TransactionalMutationTests.Success(
                contract,
                WorldReplayContract.CompatibilityName,
                CompatibilityRequest(
                    WorldProvenanceContract.JournalSchemaId,
                    WorldProvenanceContract.EntrySchemaId,
                    WorldPortabilityContract.SnapshotSchemaId,
                    WorldPortabilityContract.SnapshotVersion));
            Assert.Equal("supported", supported.Data!["disposition"]);
            Assert.Empty(FindDefinition(contract, WorldReplayContract.CompatibilityName).SuccessSchema!.ValidateValue(supported.Data));

            var unsupportedJournal = Hk04TransactionalMutationTests.Success(
                contract,
                WorldReplayContract.CompatibilityName,
                CompatibilityRequest(
                    "arkus.authoring.journal@2",
                    WorldProvenanceContract.EntrySchemaId,
                    WorldPortabilityContract.SnapshotSchemaId,
                    WorldPortabilityContract.SnapshotVersion));
            Assert.Equal("unsupported", unsupportedJournal.Data!["disposition"]);

            var unsupportedSnapshot = Hk04TransactionalMutationTests.Success(
                contract,
                WorldReplayContract.CompatibilityName,
                CompatibilityRequest(
                    WorldProvenanceContract.JournalSchemaId,
                    WorldProvenanceContract.EntrySchemaId,
                    WorldPortabilityContract.SnapshotSchemaId,
                    2));
            Assert.Equal("unsupported", unsupportedSnapshot.Data!["disposition"]);

            var discovery = Hk04TransactionalMutationTests.Success(
                contract,
                "system.describe",
                Hk01TestFixtures.EmptyRequest());
            Assert.NotNull(FindDiscovered(discovery.Data!, WorldReplayContract.CompatibilityName)["successSchema"]);
            Assert.NotNull(FindDiscovered(discovery.Data!, WorldReplayContract.ReplayName)["successSchema"]);
        }

        [Fact]
        public void ReorderedMissingAlteredAndUnsupportedJournalEvidenceFailsBeforePublishingAnything()
        {
            var source = BuildSource();

            AssertRejectedWithoutTargetChange(
                source,
                MutateJournal(source.Journal, journal =>
                {
                    var entries = (List<object?>)journal["entries"]!;
                    var first = entries[0];
                    entries[0] = entries[1];
                    entries[1] = first;
                }),
                "world.replay.sequence_invalid");

            AssertRejectedWithoutTargetChange(
                source,
                MutateJournal(source.Journal, journal =>
                {
                    ((List<object?>)journal["entries"]!).RemoveAt(1);
                    journal["entryCount"] = 1;
                }),
                "world.replay.sequence_invalid");

            AssertRejectedWithoutTargetChange(
                source,
                MutateJournal(source.Journal, journal =>
                {
                    var entry = Entry(journal, 0);
                    var request = (Dictionary<string, object?>)entry["request"]!;
                    var operation = (Dictionary<string, object?>)((List<object?>)request["operations"]!)[0]!;
                    operation["typeId"] = "fixture.tampered";
                }),
                "world.replay.entry_invalid");

            AssertRejectedWithoutTargetChange(
                source,
                MutateJournal(source.Journal, journal => journal["schemaId"] = "arkus.authoring.journal@2"),
                "world.replay.unsupported_version");

            AssertRejectedWithoutTargetChange(
                source,
                MutateJournal(source.Journal, journal =>
                {
                    ((Dictionary<string, object?>)journal["current"]!)["hash"] = new string('0', 64);
                }),
                "world.replay.sequence_invalid");
        }

        [Fact]
        public void WrongBasePreconditionAndExistingLocalHistoryCannotBeSplicedIntoReplay()
        {
            var source = BuildSource();
            var target = TargetFromSnapshot(source.BaseSnapshot);
            var contract = Compose(target);
            var before = CanonicalWorldStateCodec.ComputeContentHash(target.Current);
            var wrong = ReplayRequest(target.Current, source.Journal);
            wrong["expectedHash"] = new string('0', 64);

            var stale = contract.Dispatch(
                WorldReplayContract.ReplayName,
                Hk04TransactionalMutationTests.ExactVersion(),
                wrong);
            Assert.False(stale.Success);
            Assert.Equal("world.replay.concurrent_update", stale.Error!.MachineCode);
            Assert.Equal(before, CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(0, JournalCount(contract));

            // Build a different source whose journal base equals a state that already has local history.
            var local = TargetFromSnapshot(source.BaseSnapshot);
            var localContract = Compose(local);
            Hk04TransactionalMutationTests.Success(
                localContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    local.Current,
                    "request.hk06c.local-history",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.local-base")));
            Assert.Equal(1, JournalCount(localContract));
            var evolvedBase = local.Current;

            var replaySource = new PortableWorldAuthoringSession(evolvedBase);
            var replaySourceContract = Compose(replaySource);
            Hk04TransactionalMutationTests.Success(
                replaySourceContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    replaySource.Current,
                    "request.hk06c.after-evolved-base",
                    Hk04TransactionalMutationTests.PutObject("node.child", "fixture.replayed")));
            var replayJournal = Hk06AProvenanceJournalTests.ReadJournal(replaySourceContract).Data!;
            var retainedHash = CanonicalWorldStateCodec.ComputeContentHash(local.Current);

            var splice = localContract.Dispatch(
                WorldReplayContract.ReplayName,
                Hk04TransactionalMutationTests.ExactVersion(),
                ReplayRequest(local.Current, replayJournal));
            Assert.False(splice.Success);
            Assert.Equal("world.replay.target_history_not_empty", splice.Error!.MachineCode);
            Assert.Equal(retainedHash, CanonicalWorldStateCodec.ComputeContentHash(local.Current));
            Assert.Equal(1, JournalCount(localContract));
        }

        [Fact]
        public void ReplayClassificationCannotAliasOrdinaryMutationOrRebaseSurface()
        {
            var session = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var contribution = CanonicalWorldContract.CreateContribution(new WorldInspectionService(session), session);
            Assert.Empty(ReplaySurfaceConformance.Validate(contribution.Definitions, contribution.Routes));

            var replayDefinition = FindDefinition(Compose(session), WorldReplayContract.ReplayName);
            var withoutReplayRoute = new List<CapabilityRoute>();
            foreach (var route in contribution.Routes)
            {
                if (!string.Equals(route.Key.Name, WorldReplayContract.ReplayName, StringComparison.Ordinal))
                    withoutReplayRoute.Add(route);
            }
            Assert.NotEmpty(ReplaySurfaceConformance.Validate(new[] { replayDefinition }, withoutReplayRoute));
        }

        private static SourceFixture BuildSource()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var source = new PortableWorldAuthoringSession(initial);
            var contract = Compose(source);
            var baseSnapshot = Snapshot(contract);

            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    source.Current,
                    "request.hk06c.first",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.replay-first"),
                    Hk06AProvenanceJournalTests.PutExtension("future.replay", 1, new byte[] { 0x11, 0x22 })));
            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    source.Current,
                    "request.hk06c.second",
                    Hk04TransactionalMutationTests.PutObject(
                        "node.extra",
                        "fixture.item",
                        "node.root",
                        Hk04TransactionalMutationTests.Reference("fixture.root", "node.root"))));

            return new SourceFixture(
                source,
                baseSnapshot,
                Snapshot(contract),
                Hk06AProvenanceJournalTests.ReadJournal(contract).Data!);
        }

        private static PortableWorldAuthoringSession TargetFromSnapshot(IReadOnlyDictionary<string, object?> snapshot)
        {
            var bootstrap = new WorldState(new WorldId("world.hk06c.bootstrap"), 0, Array.Empty<WorldObject>());
            var target = new PortableWorldAuthoringSession(bootstrap);
            var contract = Compose(target);
            Hk04TransactionalMutationTests.Success(
                contract,
                WorldPortabilityContract.ImportName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "request.hk06c.base-import",
                    ["expectedRevision"] = target.Current.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target.Current),
                    ["snapshot"] = snapshot
                });
            Assert.Equal(0, JournalCount(contract));
            return target;
        }

        private static ComposedContract Compose(PortableWorldAuthoringSession session)
        {
            return CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
        }

        private static IReadOnlyDictionary<string, object?> Snapshot(ComposedContract contract)
        {
            return Hk04TransactionalMutationTests.Success(
                contract,
                WorldPortabilityContract.ExportName,
                Hk01TestFixtures.EmptyRequest()).Data!;
        }

        private static IReadOnlyDictionary<string, object?> Compare(
            ComposedContract contract,
            IReadOnlyDictionary<string, object?> before,
            IReadOnlyDictionary<string, object?> after)
        {
            return Hk04TransactionalMutationTests.Success(
                contract,
                WorldPortabilityContract.CompareName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["base"] = before,
                    ["target"] = after
                }).Data!;
        }

        private static Dictionary<string, object?> ReplayRequest(
            WorldState current,
            IReadOnlyDictionary<string, object?> journal)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["expectedRevision"] = current.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(current),
                ["journal"] = journal
            };
        }

        private static Dictionary<string, object?> CompatibilityRequest(
            string journalSchema,
            string entrySchema,
            string snapshotSchema,
            int snapshotVersion)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["journalSchemaId"] = journalSchema,
                ["entrySchemaId"] = entrySchema,
                ["snapshotSchemaId"] = snapshotSchema,
                ["snapshotVersion"] = snapshotVersion
            };
        }

        private static void AssertRejectedWithoutTargetChange(
            SourceFixture source,
            IReadOnlyDictionary<string, object?> journal,
            string expectedCode)
        {
            var target = TargetFromSnapshot(source.BaseSnapshot);
            var contract = Compose(target);
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(target.Current);
            var beforeRevision = target.Current.Revision;
            var result = contract.Dispatch(
                WorldReplayContract.ReplayName,
                Hk04TransactionalMutationTests.ExactVersion(),
                ReplayRequest(target.Current, journal));
            Assert.False(result.Success);
            Assert.Equal(expectedCode, result.Error!.MachineCode);
            Assert.Equal(beforeRevision, target.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(0, JournalCount(contract));
        }

        private static IReadOnlyDictionary<string, object?> MutateJournal(
            IReadOnlyDictionary<string, object?> source,
            Action<Dictionary<string, object?>> mutation)
        {
            var clone = (Dictionary<string, object?>)CloneValue(source)!;
            mutation(clone);
            return clone;
        }

        private static Dictionary<string, object?> Entry(Dictionary<string, object?> journal, int index)
        {
            return (Dictionary<string, object?>)((List<object?>)journal["entries"]!)[index]!;
        }

        private static object? CloneValue(object? value)
        {
            if (value is IReadOnlyDictionary<string, object?> map)
            {
                var clone = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var pair in map) clone.Add(pair.Key, CloneValue(pair.Value));
                return clone;
            }
            if (value is IReadOnlyList<object?> list)
            {
                var clone = new List<object?>();
                for (var index = 0; index < list.Count; index++) clone.Add(CloneValue(list[index]));
                return clone;
            }
            return value;
        }

        private static int JournalCount(ComposedContract contract)
        {
            return Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"]);
        }

        private static CapabilityDefinition FindDefinition(ComposedContract contract, string name)
        {
            foreach (var definition in contract.Definitions)
                if (string.Equals(definition.Key.Name, name, StringComparison.Ordinal)) return definition;
            throw new InvalidOperationException("Missing definition " + name);
        }

        private static IReadOnlyDictionary<string, object?> FindDiscovered(
            IReadOnlyDictionary<string, object?> discovery,
            string name)
        {
            foreach (var value in Hk04TransactionalMutationTests.List(discovery, "capabilities"))
            {
                var capability = (IReadOnlyDictionary<string, object?>)value!;
                if (string.Equals((string)capability["name"]!, name, StringComparison.Ordinal)) return capability;
            }
            throw new InvalidOperationException("Missing discovered capability " + name);
        }

        private sealed class SourceFixture
        {
            public SourceFixture(
                PortableWorldAuthoringSession source,
                IReadOnlyDictionary<string, object?> baseSnapshot,
                IReadOnlyDictionary<string, object?> finalSnapshot,
                IReadOnlyDictionary<string, object?> journal)
            {
                Source = source;
                BaseSnapshot = baseSnapshot;
                FinalSnapshot = finalSnapshot;
                Journal = journal;
            }

            public PortableWorldAuthoringSession Source { get; }
            public IReadOnlyDictionary<string, object?> BaseSnapshot { get; }
            public IReadOnlyDictionary<string, object?> FinalSnapshot { get; }
            public IReadOnlyDictionary<string, object?> Journal { get; }
        }
    }
}
