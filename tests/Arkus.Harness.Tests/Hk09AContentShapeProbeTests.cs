using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk09AContentShapeProbeTests
    {
        [Fact]
        public void PotesMarketSliceStillSupportsInspectAuthorSnapshotAndReplayUnderH0Policy()
        {
            // Approved product-shape source: Docs/art/VISUAL_BIBLE.md — plaza/market grain,
            // bar terrace and workshops in the fictional Potes/Liebana valley-town slice.
            var plazaId = new WorldObjectId("place.plaza");
            var initial = new WorldState(
                new WorldId("world.hk09a.potes-market"),
                0,
                new[]
                {
                    new WorldObject(plazaId, new WorldTypeId("fixture.hk09a.plaza")),
                    new WorldObject(
                        new WorldObjectId("market.stalls"),
                        new WorldTypeId("fixture.hk09a.market.stalls"),
                        plazaId),
                    new WorldObject(
                        new WorldObjectId("building.bar.terrace"),
                        new WorldTypeId("fixture.hk09a.bar.terrace"),
                        plazaId),
                    new WorldObject(
                        new WorldObjectId("building.workshop.front"),
                        new WorldTypeId("fixture.hk09a.workshop.front"),
                        plazaId)
                });

            var source = new PortableWorldAuthoringSession(initial);
            var sourceContract = CanonicalWorldContract.Compose(new WorldInspectionService(source), source);
            Assert.Empty(H0HostCapabilityPolicy.Validate(sourceContract.Definitions));

            var summary = Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldInspectionContract.SummaryName,
                Hk01TestFixtures.EmptyRequest());
            Assert.Equal(4, Convert.ToInt32(summary.Data!["objectCount"]));

            var baseSnapshot = Snapshot(sourceContract);
            Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    source.Current,
                    "request.hk09a.market-rain-layout",
                    Hk04TransactionalMutationTests.PutObject(
                        "market.stalls",
                        "fixture.hk09a.market.stalls.rain-layout",
                        "place.plaza"),
                    Hk04TransactionalMutationTests.PutObject(
                        "building.bar.terrace",
                        "fixture.hk09a.bar.terrace.wet-weather",
                        "place.plaza")));

            var finalSnapshot = Snapshot(sourceContract);
            Assert.NotEqual(baseSnapshot["authoredStateBase64"], finalSnapshot["authoredStateBase64"]);
            var journal = Hk06AProvenanceJournalTests.ReadJournal(sourceContract).Data!;
            Assert.Equal(1, Convert.ToInt32(journal["entryCount"]));

            var target = new PortableWorldAuthoringSession(
                new WorldState(new WorldId("world.hk09a.replay-target"), 0, Array.Empty<WorldObject>()));
            var targetContract = CanonicalWorldContract.Compose(new WorldInspectionService(target), target);
            Assert.Empty(H0HostCapabilityPolicy.Validate(targetContract.Definitions));

            Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldPortabilityContract.ImportName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "request.hk09a.base-import",
                    ["expectedRevision"] = target.Current.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target.Current),
                    ["snapshot"] = baseSnapshot
                });

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
            Assert.Equal(
                finalSnapshot["authoredStateBase64"],
                Snapshot(targetContract)["authoredStateBase64"]);
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
