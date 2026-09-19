using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06AContentShapeProbeTests
    {
        [Fact]
        public void PotesAuthoredEditIsJournaledWhileScheduledNpcObservationRemainsTransient()
        {
            var world = PotesHeroSlice();
            var session = new TransactionalWorldAuthoringSession(world);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var refreshShop = Hk04TransactionalMutationTests.Request(
                world,
                "request.potes-refresh-shop",
                Hk04TransactionalMutationTests.PutObject(
                    "building.shop",
                    "fixture.building.refreshed",
                    "place.plaza"));

            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                refreshShop);
            var authored = session.Current;
            var journalBefore = Hk06AProvenanceJournalTests.ReadJournal(contract).Data!;
            var entriesBefore = Hk06AProvenanceJournalTests.Entries(journalBefore);
            Assert.Single(entriesBefore);
            Assert.Equal(
                new[] { "world.object:building.shop" },
                Hk04TransactionalMutationTests.Strings(entriesBefore[0], "affectedResources"));

            var stamp = new RuntimeObservationStamp(AuthoredWorldAnchor.FromState(authored));
            Assert.NotEqual(RuntimeObservationStamp.SchemaId, WorldProvenanceContract.EntrySchemaId);
            Assert.Empty(WorldProvenanceContract.RuntimeObservationStampSchema().ValidateValue(stamp.ToData()));
            var observation = new ScheduledNpcObservationSurrogate("npc.ana", stamp);
            var beforeStep = observation.Step;
            var beforePosition = observation.TransientPosition;
            var beforeRevision = session.Current.Revision;
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var beforeEntryIds = JournalEntryIds(contract);

            observation.Advance();
            observation.Advance();

            Assert.Equal(beforeStep + 2, observation.Step);
            Assert.NotEqual(beforePosition, observation.TransientPosition);
            Assert.Equal(beforeRevision, session.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(beforeEntryIds, JournalEntryIds(contract));
            Assert.Equal(authored.Revision, observation.Stamp.AuthoredBase.Revision);
            Assert.Equal(beforeHash, observation.Stamp.AuthoredBase.Hash);

            Assert.Empty(ObservationBoundaryIssues(session, contract, observation.Advance));
        }

        [Fact]
        public void ObservationBoundaryOracleTurnsRedIfSurrogateAcquiresCommitAuthority()
        {
            var world = PotesHeroSlice();
            var session = new TransactionalWorldAuthoringSession(world);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var stamp = new RuntimeObservationStamp(AuthoredWorldAnchor.FromState(world));
            var safe = new ScheduledNpcObservationSurrogate("npc.ana", stamp);

            Assert.Empty(ObservationBoundaryIssues(session, contract, safe.Advance));

            var unsafeRequest = Hk04TransactionalMutationTests.Request(
                session.Current,
                "request.runtime-observation-must-not-commit",
                Hk04TransactionalMutationTests.PutObject(
                    "npc.ana",
                    "fixture.npc.runtime-mutant",
                    "place.plaza",
                    Hk04TransactionalMutationTests.Reference("works.at", "building.bar")));
            var unsafeSurrogate = new UnsafeObservationSurrogate(() =>
                Hk04TransactionalMutationTests.Success(
                    contract,
                    WorldMutationContract.ApplyName,
                    unsafeRequest));

            var issues = ObservationBoundaryIssues(session, contract, unsafeSurrogate.Advance);
            Assert.Contains("authored-revision-changed", issues);
            Assert.Contains("authored-hash-changed", issues);
            Assert.Contains("authored-journal-changed", issues);
        }

        private static WorldState PotesHeroSlice()
        {
            var plazaId = new WorldObjectId("place.plaza");
            var barId = new WorldObjectId("building.bar");
            var shopId = new WorldObjectId("building.shop");
            var anaId = new WorldObjectId("npc.ana");
            var worksAt = new WorldReferenceKind("works.at");
            return new WorldState(
                new WorldId("world.potes"),
                7,
                new WorldObject[]
                {
                    new WorldObject(plazaId, new WorldTypeId("fixture.place")),
                    new WorldObject(barId, new WorldTypeId("fixture.building"), plazaId),
                    new WorldObject(shopId, new WorldTypeId("fixture.building"), plazaId),
                    new WorldObject(
                        anaId,
                        new WorldTypeId("fixture.npc"),
                        plazaId,
                        new[] { new WorldReference(worksAt, barId) })
                },
                new[]
                {
                    new WorldExtensionData(
                        "arkus.npc-profile",
                        1,
                        new byte[] { 0x50, 0x4f, 0x54, 0x45, 0x53 },
                        anaId,
                        new[] { new WorldReference(worksAt, barId) })
                });
        }

        private static IReadOnlyList<string> ObservationBoundaryIssues(
            TransactionalWorldAuthoringSession session,
            ComposedContract contract,
            Action step)
        {
            var beforeRevision = session.Current.Revision;
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var beforeJournal = JournalEntryIds(contract);
            step();

            var issues = new List<string>();
            if (session.Current.Revision != beforeRevision) issues.Add("authored-revision-changed");
            if (!string.Equals(
                CanonicalWorldStateCodec.ComputeContentHash(session.Current),
                beforeHash,
                StringComparison.Ordinal))
            {
                issues.Add("authored-hash-changed");
            }

            var afterJournal = JournalEntryIds(contract);
            if (!SequenceEqual(beforeJournal, afterJournal)) issues.Add("authored-journal-changed");
            return issues.AsReadOnly();
        }

        private static IReadOnlyList<string> JournalEntryIds(ComposedContract contract)
        {
            var result = new List<string>();
            var journal = Hk06AProvenanceJournalTests.ReadJournal(contract).Data!;
            foreach (var entry in Hk06AProvenanceJournalTests.Entries(journal))
            {
                result.Add((string)entry["entryId"]!);
            }

            return result.AsReadOnly();
        }

        private static bool SequenceEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left.Count != right.Count) return false;
            for (var index = 0; index < left.Count; index++)
            {
                if (!string.Equals(left[index], right[index], StringComparison.Ordinal)) return false;
            }

            return true;
        }

        private sealed class ScheduledNpcObservationSurrogate
        {
            public ScheduledNpcObservationSurrogate(string npcId, RuntimeObservationStamp stamp)
            {
                NpcId = npcId;
                Stamp = stamp;
                TransientPosition = "plaza.north";
            }

            public string NpcId { get; }
            public RuntimeObservationStamp Stamp { get; }
            public int Step { get; private set; }
            public string TransientPosition { get; private set; }

            public void Advance()
            {
                Step++;
                TransientPosition = Step % 2 == 0 ? "bar.door" : "plaza.center";
            }
        }

        private sealed class UnsafeObservationSurrogate
        {
            private readonly Action _commit;

            public UnsafeObservationSurrogate(Action commit)
            {
                _commit = commit;
            }

            public void Advance()
            {
                _commit();
            }
        }
    }
}
