using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08BContentShapeProbeTests
    {
        [Fact]
        public void RepresentativeMarketMicroBlockStaleEditRecoversByInspectingOnlyChangedResources()
        {
            var rootId = new WorldObjectId("market.root");
            var initial = new WorldState(
                new WorldId("world.hk08b.market-probe"),
                0,
                new[]
                {
                    new WorldObject(rootId, new WorldTypeId("fixture.hk08b.market.root")),
                    new WorldObject(
                        new WorldObjectId("plaza.stalls"),
                        new WorldTypeId("fixture.hk08b.market.cluster"),
                        rootId),
                    new WorldObject(
                        new WorldObjectId("street.bar.front"),
                        new WorldTypeId("fixture.hk08b.facade.bar"),
                        rootId),
                    new WorldObject(
                        new WorldObjectId("street.workshop.front"),
                        new WorldTypeId("fixture.hk08b.facade.workshop"),
                        rootId)
                });
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);

            // Two accepted edits advance the local lineage after the client formed its coherent intent.
            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk08b.market-writer-a",
                    Hk04TransactionalMutationTests.PutObject(
                        "plaza.stalls",
                        "fixture.hk08b.market.cluster.vendor-layout",
                        "market.root")));
            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    session.Current,
                    "request.hk08b.market-writer-b",
                    Hk04TransactionalMutationTests.PutObject(
                        "street.workshop.front",
                        "fixture.hk08b.facade.workshop.open",
                        "market.root")));

            // The stale client intent is still one coherent transaction spanning two authored resources.
            var staleIntent = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk08b.market-client",
                Hk04TransactionalMutationTests.PutObject(
                    "plaza.stalls",
                    "fixture.hk08b.market.cluster.rain-layout",
                    "market.root"),
                Hk04TransactionalMutationTests.PutObject(
                    "street.bar.front",
                    "fixture.hk08b.facade.bar.terrace-layout",
                    "market.root"));
            var stale = contract.Dispatch(
                WorldMutationContract.PlanName,
                Hk04TransactionalMutationTests.ExactVersion(),
                staleIntent);

            Assert.False(stale.Success);
            Assert.Equal("world.change.stale_revision", stale.Error!.MachineCode);
            var recovery = Recovery(stale);
            Assert.Equal(WorldConflictRecoveryContract.SameLineageReplan, recovery["disposition"]);
            Assert.Equal(
                new[]
                {
                    "world.object:plaza.stalls",
                    "world.object:street.workshop.front"
                },
                Hk04TransactionalMutationTests.Strings(recovery, "changedResources"));

            var proof = Hk04TransactionalMutationTests.Map(recovery, "historyProof");
            Assert.Equal(WorldConflictRecoveryContract.HistoryComplete, proof["reason"]);
            Assert.Equal(2, Convert.ToInt32(proof["transitionCount"]));

            var descriptors = Hk04TransactionalMutationTests.List(recovery, "currentResources");
            Assert.Equal(2, descriptors.Count);
            foreach (var raw in descriptors)
            {
                var descriptor = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(raw);
                Assert.Equal("object", descriptor["kind"]);
                Assert.Equal("present", descriptor["presence"]);
                var inspection = Hk04TransactionalMutationTests.Map(descriptor, "inspection");
                Assert.Equal(WorldInspectionContract.ObjectGetName, inspection["name"]);
                var inspected = contract.Dispatch(
                    (string)inspection["name"]!,
                    Hk04TransactionalMutationTests.ExactVersion(),
                    Hk04TransactionalMutationTests.Map(inspection, "request"));
                Assert.True(inspected.Success, inspected.Error?.MachineCode);
            }

            // Re-plan from the authoritative current anchor, preserving the original two-resource intent.
            var recoveredIntent = Hk04TransactionalMutationTests.Request(
                session.Current,
                "request.hk08b.market-client",
                Hk04TransactionalMutationTests.PutObject(
                    "plaza.stalls",
                    "fixture.hk08b.market.cluster.rain-layout",
                    "market.root"),
                Hk04TransactionalMutationTests.PutObject(
                    "street.bar.front",
                    "fixture.hk08b.facade.bar.terrace-layout",
                    "market.root"));
            var plan = Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.PlanName,
                recoveredIntent);
            Assert.Equal(
                2,
                Hk04TransactionalMutationTests.List(
                    Hk04TransactionalMutationTests.Map(plan.Data!, "plan"),
                    "changes").Count);

            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.DryRunName,
                recoveredIntent);
            var applied = Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                recoveredIntent);

            Assert.False((bool)applied.Data!["replayed"]!);
            Assert.Equal(initial.Revision + 3, session.Current.Revision);
            Assert.Equal(
                3,
                Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"]));
        }

        private static IReadOnlyDictionary<string, object?> Recovery(CapabilityInvocationResult result)
        {
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.True(result.Error!.Context.TryGetValue("recovery", out var value));
            return Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(value);
        }
    }
}
