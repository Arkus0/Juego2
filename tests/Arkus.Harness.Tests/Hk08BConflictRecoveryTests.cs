using System;
using System.Collections.Generic;
using System.Text.Json;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08BConflictRecoveryTests
    {
        [Fact]
        public void SameLineageStaleConflictReturnsCompleteAffectedOnlyRecoveryAndExplicitRetrySucceeds()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);

            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk08b.writer-a",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.writer-a")));

            var afterFirst = session.Current;
            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    afterFirst,
                    "request.hk08b.writer-b",
                    Hk04TransactionalMutationTests.PutObject("node.child", "fixture.writer-b", "node.root")));

            var staleRequest = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk08b.stale-client",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.client-intent"));
            var stale = contract.Dispatch(
                WorldMutationContract.PlanName,
                Hk04TransactionalMutationTests.ExactVersion(),
                staleRequest);

            Assert.False(stale.Success);
            Assert.Equal("world.change.stale_revision", stale.Error!.MachineCode);
            var recovery = Recovery(stale);
            Assert.Equal(WorldConflictRecoveryContract.SchemaId, recovery["schemaId"]);
            Assert.Equal(WorldConflictRecoveryContract.SameLineageReplan, recovery["disposition"]);

            var expected = Hk04TransactionalMutationTests.Map(recovery, "expected");
            Assert.Equal(initial.Revision, Convert.ToInt64(expected["revision"]));
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(initial), expected["hash"]);

            var current = Hk04TransactionalMutationTests.Map(recovery, "current");
            Assert.Equal(session.Current.Id.Value, current["worldId"]);
            Assert.Equal(session.Current.SchemaVersion, Convert.ToInt32(current["stateSchemaVersion"]));
            Assert.Equal(session.Current.Revision, Convert.ToInt64(current["revision"]));
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(session.Current), current["hash"]);

            var proof = Hk04TransactionalMutationTests.Map(recovery, "historyProof");
            Assert.Equal(WorldConflictRecoveryContract.HistoryComplete, proof["reason"]);
            Assert.Equal(0, Convert.ToInt32(proof["transitionStart"]));
            Assert.Equal(2, Convert.ToInt32(proof["transitionCount"]));

            Assert.Equal(
                new[] { "world.object:node.child", "world.object:node.peer" },
                Hk04TransactionalMutationTests.Strings(recovery, "changedResources"));

            var resourceDescriptors = Hk04TransactionalMutationTests.List(recovery, "currentResources");
            Assert.Equal(2, resourceDescriptors.Count);
            foreach (var raw in resourceDescriptors)
            {
                var descriptor = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(raw);
                Assert.Equal("object", descriptor["kind"]);
                Assert.Equal("present", descriptor["presence"]);
                var inspection = Hk04TransactionalMutationTests.Map(descriptor, "inspection");
                Assert.Equal(WorldInspectionContract.ObjectGetName, inspection["name"]);
                Assert.Equal("1.0", inspection["version"]);
                var inspectionRequest = Hk04TransactionalMutationTests.Map(inspection, "request");
                Assert.Equal(session.Current.Revision, Convert.ToInt64(inspectionRequest["revision"]));
                Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(session.Current), inspectionRequest["hash"]);

                // The recovery path is intentionally affected-only. It consumes the bounded descriptor
                // supplied by the error instead of enumerating/reloading the whole world.
                var inspected = contract.Dispatch(
                    (string)inspection["name"]!,
                    Hk04TransactionalMutationTests.ExactVersion(),
                    inspectionRequest);
                Assert.True(inspected.Success, inspected.Error?.MachineCode);
            }

            var recoveredRequest = Hk04TransactionalMutationTests.Request(
                session.Current,
                "request.hk08b.stale-client",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.client-intent"));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.PlanName, recoveredRequest);
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.DryRunName, recoveredRequest);
            var applied = Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                recoveredRequest);

            Assert.False((bool)applied.Data!["replayed"]!);
            Assert.Equal(initial.Revision + 3, session.Current.Revision);
            Assert.Equal(3, Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"]));
        }

        [Fact]
        public void PlanDryRunAndApplyExposeTheSameRecoveryTruthForOneStaleBase()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);

            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk08b.route-writer",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.route-writer")));

            string? canonicalRecovery = null;
            foreach (var capability in new[]
            {
                WorldMutationContract.PlanName,
                WorldMutationContract.DryRunName,
                WorldMutationContract.ApplyName
            })
            {
                var result = contract.Dispatch(
                    capability,
                    Hk04TransactionalMutationTests.ExactVersion(),
                    Hk04TransactionalMutationTests.Request(
                        initial,
                        "request.hk08b.route-stale." + capability,
                        Hk04TransactionalMutationTests.PutObject("node.child", "fixture.route-client", "node.root")));

                Assert.False(result.Success);
                Assert.Equal("world.change.stale_revision", result.Error!.MachineCode);
                var recovery = Recovery(result);
                Assert.Equal(WorldConflictRecoveryContract.SameLineageReplan, recovery["disposition"]);
                Assert.Equal(new[] { "world.object:node.peer" }, Hk04TransactionalMutationTests.Strings(recovery, "changedResources"));

                var signature = JsonSerializer.Serialize(recovery);
                if (canonicalRecovery == null)
                {
                    canonicalRecovery = signature;
                }
                else
                {
                    Assert.Equal(canonicalRecovery, signature);
                }
            }

            Assert.Equal(initial.Revision + 1, session.Current.Revision);
            Assert.Equal(1, Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"]));
        }

        [Fact]
        public void ChangedResourceRemovedSinceExpectedBaseIsReturnedExplicitlyAsAbsent()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);

            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk08b.add-extra",
                    Hk04TransactionalMutationTests.PutObject("node.extra", "fixture.extra")));
            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    session.Current,
                    "request.hk08b.remove-extra",
                    Hk04TransactionalMutationTests.RemoveObject("node.extra")));

            var stale = contract.Dispatch(
                WorldMutationContract.PlanName,
                Hk04TransactionalMutationTests.ExactVersion(),
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk08b.absent-reader",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.intent")));
            var recovery = Recovery(stale);

            Assert.Equal(
                new[] { "world.object:node.extra" },
                Hk04TransactionalMutationTests.Strings(recovery, "changedResources"));
            var descriptor = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(
                Assert.Single(Hk04TransactionalMutationTests.List(recovery, "currentResources")));
            Assert.Equal("world.object:node.extra", descriptor["resource"]);
            Assert.Equal("absent", descriptor["presence"]);
        }

        [Fact]
        public void SnapshotRebaseWithSameRevisionButDifferentHashNeverFabricatesAncestryOrDelta()
        {
            var common = Hk02TestFixtures.MicroWorld();

            var source = new PortableWorldAuthoringSession(common);
            var sourceContract = Compose(source);
            Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    common,
                    "request.hk08b.source-change",
                    Hk04TransactionalMutationTests.PutObject("node.child", "fixture.source", "node.root")));
            var snapshot = Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldPortabilityContract.ExportName,
                Hk01TestFixtures.EmptyRequest()).Data!;

            var target = new PortableWorldAuthoringSession(common);
            var targetContract = Compose(target);
            Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    common,
                    "request.hk08b.target-change",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.target")));
            var preRebase = target.Current;
            var preRebaseHash = CanonicalWorldStateCodec.ComputeContentHash(preRebase);

            var importRequest = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.hk08b.rebase",
                ["expectedRevision"] = preRebase.Revision,
                ["expectedHash"] = preRebaseHash,
                ["snapshot"] = snapshot
            };
            Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldPortabilityContract.ImportName,
                importRequest);

            Assert.Equal(preRebase.Revision, target.Current.Revision);
            Assert.NotEqual(preRebaseHash, CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(0, Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(targetContract).Data!["entryCount"]));

            var stale = targetContract.Dispatch(
                WorldMutationContract.PlanName,
                Hk04TransactionalMutationTests.ExactVersion(),
                Hk04TransactionalMutationTests.Request(
                    preRebase,
                    "request.hk08b.pre-rebase-client",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.old-lineage-intent")));

            Assert.False(stale.Success);
            Assert.Equal("world.change.stale_hash", stale.Error!.MachineCode);
            var recovery = Recovery(stale);
            Assert.Equal(WorldConflictRecoveryContract.BoundedReinspectionRequired, recovery["disposition"]);
            var proof = Hk04TransactionalMutationTests.Map(recovery, "historyProof");
            Assert.Equal(WorldConflictRecoveryContract.ExpectedBaseNotInCurrentLineage, proof["reason"]);
            Assert.Empty(Hk04TransactionalMutationTests.List(recovery, "changedResources"));
            Assert.Empty(Hk04TransactionalMutationTests.List(recovery, "currentResources"));

            var lineageBase = Hk04TransactionalMutationTests.Map(proof, "lineageBase");
            Assert.Equal(target.Current.Revision, Convert.ToInt64(lineageBase["revision"]));
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(target.Current), lineageBase["hash"]);
        }

        private static ComposedContract Compose(PortableWorldAuthoringSession session)
        {
            return CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
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
