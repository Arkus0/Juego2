using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06CContentShapeTests
    {
        [Fact]
        public void PotesAuthoredSliceReplaysFromAcceptedSnapshotToIdenticalHashAndEmptySemanticDiff()
        {
            var source = new PortableWorldAuthoringSession(PotesWorld());
            var sourceContract = Compose(source);
            var baseSnapshot = Snapshot(sourceContract);

            Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    source.Current,
                    "request.hk06c.potes.shop",
                    Hk04TransactionalMutationTests.PutObject(
                        "building.shop",
                        "fixture.shop.refreshed",
                        "place.plaza")));
            Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    source.Current,
                    "request.hk06c.potes.social",
                    Hk06AProvenanceJournalTests.PutExtension(
                        "future.social",
                        1,
                        new byte[] { 0x10, 0x20, 0x30 })));

            var sourceFinal = Snapshot(sourceContract);
            var journal = Hk06AProvenanceJournalTests.ReadJournal(sourceContract).Data!;

            var target = new PortableWorldAuthoringSession(
                new WorldState(new WorldId("world.hk06c.potes-bootstrap"), 0, Array.Empty<WorldObject>()));
            var targetContract = Compose(target);
            Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldPortabilityContract.ImportName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "request.hk06c.potes.base-import",
                    ["expectedRevision"] = target.Current.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target.Current),
                    ["snapshot"] = baseSnapshot
                });

            Assert.Equal(0, Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(targetContract).Data!["entryCount"]));
            Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldReplayContract.ReplayName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["expectedRevision"] = target.Current.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target.Current),
                    ["journal"] = journal
                });

            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(source.Current),
                CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(source.Current.Revision, target.Current.Revision);

            var targetFinal = Snapshot(targetContract);
            var diff = Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldPortabilityContract.CompareName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["base"] = sourceFinal,
                    ["target"] = targetFinal
                }).Data!;
            Assert.True((bool)diff["sameAuthorableState"]!);
            Assert.Empty(Hk04TransactionalMutationTests.List(diff, "changes"));
        }

        private static WorldState PotesWorld()
        {
            var plaza = new WorldObject(new WorldObjectId("place.plaza"), new WorldTypeId("fixture.place"));
            var bar = new WorldObject(
                new WorldObjectId("building.bar"),
                new WorldTypeId("fixture.bar"),
                new WorldObjectId("place.plaza"));
            var shop = new WorldObject(
                new WorldObjectId("building.shop"),
                new WorldTypeId("fixture.shop"),
                new WorldObjectId("place.plaza"));
            var npc = new WorldObject(
                new WorldObjectId("npc.ana"),
                new WorldTypeId("fixture.npc"),
                new WorldObjectId("place.plaza"),
                new[]
                {
                    new WorldReference(new WorldReferenceKind("works.at"), new WorldObjectId("building.bar"))
                });
            return new WorldState(
                new WorldId("world.potes"),
                7,
                new[] { shop, npc, plaza, bar },
                new[] { new WorldExtensionData("future.social", 1, new byte[] { 0x10, 0x20 }) });
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
    }
}
