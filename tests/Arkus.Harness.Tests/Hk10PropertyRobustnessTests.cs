using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk10PropertyRobustnessTests
    {
        private static readonly int[] Seeds =
        {
            101, 509, 1237, 4099, 8191, 16381, 32749, 65521,
            104729, 130363, 196613, 262147, 524287, 786433, 999983, 15485863
        };

        [Fact]
        public void SeededCanonicalSerializationHashAndQueriesAreDeterministicAndReproducible()
        {
            foreach (var seed in Seeds)
            {
                var state = SeededWorld(seed);
                var canonical = CanonicalWorldStateCodec.Serialize(state);
                var decoded = CanonicalWorldStateCodec.Deserialize(canonical);
                var reserialized = CanonicalWorldStateCodec.Serialize(decoded);

                Assert.True(canonical.SequenceEqual(reserialized), "seed=" + seed + " canonical round-trip drifted");
                Assert.Equal(
                    CanonicalWorldStateCodec.ComputeContentHash(state),
                    CanonicalWorldStateCodec.ComputeContentHash(decoded));

                var reversed = new WorldState(
                    state.Id,
                    state.Revision,
                    state.Objects.Reverse(),
                    state.Extensions.Reverse(),
                    state.SchemaVersion);
                Assert.Equal(
                    Convert.ToBase64String(canonical),
                    Convert.ToBase64String(CanonicalWorldStateCodec.Serialize(reversed)));
                Assert.Equal(
                    CanonicalWorldStateCodec.ComputeContentHash(state),
                    CanonicalWorldStateCodec.ComputeContentHash(reversed));

                var contract = Hk03InspectionTests.Compose(state);
                var query = Hk03InspectionTests.Anchor(state);
                query["limit"] = 100;
                var first = Hk03InspectionTests.Dispatch(contract, WorldInspectionContract.ObjectQueryName, query);
                var second = Hk03InspectionTests.Dispatch(contract, WorldInspectionContract.ObjectQueryName, query);
                Assert.Equal(
                    JsonSerializer.Serialize(first.Data),
                    JsonSerializer.Serialize(second.Data));
            }
        }

        [Fact]
        public void SeededAtomicityIdempotencyAndStaleRecoveryPreserveIndependentStateAnchors()
        {
            foreach (var seed in Seeds)
            {
                var initial = Hk02TestFixtures.MicroWorld();
                var session = new TransactionalWorldAuthoringSession(initial);
                var contract = Hk04TransactionalMutationTests.Compose(session);
                var initialHash = CanonicalWorldStateCodec.ComputeContentHash(initial);

                var invalid = Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk10.invalid." + seed,
                    Hk04TransactionalMutationTests.PutObject("node.hk10-extra", "fixture.hk10.extra"),
                    Hk04TransactionalMutationTests.RemoveObject("node.root"));
                var rejected = contract.Dispatch(
                    WorldMutationContract.ApplyName,
                    Hk04TransactionalMutationTests.ExactVersion(),
                    invalid);
                Assert.False(rejected.Success);
                Assert.Equal(initial.Revision, session.Current.Revision);
                Assert.Equal(initialHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));

                var acceptedRequest = Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk10.accepted." + seed,
                    Hk04TransactionalMutationTests.PutObject(
                        "node.peer",
                        "fixture.hk10.type" + seed));
                var first = Hk04TransactionalMutationTests.Success(
                    contract,
                    WorldMutationContract.ApplyName,
                    acceptedRequest);
                Assert.False((bool)first.Data!["replayed"]!);
                var acceptedRevision = session.Current.Revision;
                var acceptedHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

                var retry = Hk04TransactionalMutationTests.Success(
                    contract,
                    WorldMutationContract.ApplyName,
                    acceptedRequest);
                Assert.True((bool)retry.Data!["replayed"]!);
                Assert.Equal(acceptedRevision, session.Current.Revision);
                Assert.Equal(acceptedHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));

                var stale = contract.Dispatch(
                    WorldMutationContract.PlanName,
                    Hk04TransactionalMutationTests.ExactVersion(),
                    Hk04TransactionalMutationTests.Request(
                        initial,
                        "request.hk10.stale." + seed,
                        Hk04TransactionalMutationTests.PutObject("node.child", "fixture.hk10.stale" + seed, "node.root")));
                Assert.False(stale.Success);
                Assert.Equal("world.change.stale_revision", stale.Error!.MachineCode);
                Assert.True(stale.Error.Context.TryGetValue("recovery", out var recoveryValue));
                var recovery = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(recoveryValue);
                var expected = Hk04TransactionalMutationTests.Map(recovery, "expected");
                var current = Hk04TransactionalMutationTests.Map(recovery, "current");
                Assert.Equal(initial.Revision, Convert.ToInt64(expected["revision"]));
                Assert.Equal(initialHash, expected["hash"]);
                Assert.Equal(acceptedRevision, Convert.ToInt64(current["revision"]));
                Assert.Equal(acceptedHash, current["hash"]);
                Assert.Equal(acceptedHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            }
        }

        [Fact]
        public void SeededReplayProducesEquivalentFinalStateAndProvenanceIdentity()
        {
            foreach (var seed in Seeds.Take(8))
            {
                var initial = Hk02TestFixtures.MicroWorld();
                var source = new PortableWorldAuthoringSession(initial);
                var sourceContract = ComposePortable(source);

                Hk04TransactionalMutationTests.Success(
                    sourceContract,
                    WorldMutationContract.ApplyName,
                    Hk04TransactionalMutationTests.Request(
                        source.Current,
                        "request.hk10.replay.a." + seed,
                        Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.hk10.replaya" + seed)));
                Hk04TransactionalMutationTests.Success(
                    sourceContract,
                    WorldMutationContract.ApplyName,
                    Hk04TransactionalMutationTests.Request(
                        source.Current,
                        "request.hk10.replay.b." + seed,
                        Hk04TransactionalMutationTests.PutObject("node.child", "fixture.hk10.replayb" + seed, "node.root")));

                var journal = Hk06AProvenanceJournalTests.ReadJournal(sourceContract).Data!;
                var target = new PortableWorldAuthoringSession(initial);
                var targetContract = ComposePortable(target);
                var replayRequest = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["expectedRevision"] = target.Current.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target.Current),
                    ["journal"] = journal
                };

                var replay = Hk04TransactionalMutationTests.Success(
                    targetContract,
                    WorldReplayContract.ReplayName,
                    replayRequest);
                Assert.Equal(2, Convert.ToInt32(replay.Data!["replayedEntries"]));
                Assert.Equal(source.Current.Revision, target.Current.Revision);
                Assert.Equal(
                    CanonicalWorldStateCodec.ComputeContentHash(source.Current),
                    CanonicalWorldStateCodec.ComputeContentHash(target.Current));

                var sourceEntries = Hk06AProvenanceJournalTests.Entries(journal);
                var targetEntries = Hk06AProvenanceJournalTests.Entries(
                    Hk06AProvenanceJournalTests.ReadJournal(targetContract).Data!);
                Assert.Equal(sourceEntries.Count, targetEntries.Count);
                for (var index = 0; index < sourceEntries.Count; index++)
                {
                    Assert.Equal(sourceEntries[index]["entryId"], targetEntries[index]["entryId"]);
                }
            }
        }

        [Fact]
        public void MalformedTruncatedUnknownVersionUnknownCommandAndSchemaInvalidInputsFailDefined()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);

            var unknown = contract.Dispatch(
                "fixture.hk10.unknown-command",
                Hk04TransactionalMutationTests.ExactVersion(),
                Empty());
            Assert.False(unknown.Success);
            Assert.Equal("contract.unknown_capability", unknown.Error!.MachineCode);

            var unsupported = contract.Dispatch(
                "system.describe",
                ContractVersionRange.Exact(new ContractVersion(99, 0)),
                Empty());
            Assert.False(unsupported.Success);
            Assert.Equal("contract.unsupported_version", unsupported.Error!.MachineCode);

            var schemaInvalid = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                Empty());
            Assert.False(schemaInvalid.Success);
            Assert.Equal("contract.invalid_request", schemaInvalid.Error!.MachineCode);

            var canonical = CanonicalWorldStateCodec.Serialize(initial);
            var truncated = canonical.Take(canonical.Length - 1).ToArray();
            var truncatedFailure = Assert.Throws<WorldStateException>(() => CanonicalWorldStateCodec.Deserialize(truncated));
            Assert.False(string.IsNullOrWhiteSpace(truncatedFailure.MachineCode));

            var text = Encoding.UTF8.GetString(canonical);
            var objectIndex = text.IndexOf("object\t", StringComparison.Ordinal);
            Assert.True(objectIndex >= 0);
            var unknownRecordText = text.Substring(0, objectIndex) + "bogusx\t" +
                text.Substring(objectIndex + "object\t".Length);
            var unknownRecordFailure = Assert.Throws<WorldStateException>(() =>
                CanonicalWorldStateCodec.Deserialize(Encoding.UTF8.GetBytes(unknownRecordText)));
            Assert.Equal("world.unknown_record", unknownRecordFailure.MachineCode);

            var invalidUtf8Failure = Assert.Throws<WorldStateException>(() =>
                CanonicalWorldStateCodec.Deserialize(new byte[] { 0xff, 0xfe, 0xfd }));
            Assert.Equal("world.invalid_utf8", invalidUtf8Failure.MachineCode);

            Assert.Equal(initial.Revision, session.Current.Revision);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(initial),
                CanonicalWorldStateCodec.ComputeContentHash(session.Current));
        }

        [Fact]
        public void CancelledMutationFailsBeforePublicationAndKeepsStateAndJournalStable()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new PortableWorldAuthoringSession(initial);
            var contract = ComposePortable(session);
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(initial);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            var result = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk10.cancelled",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.hk10.cancelled")),
                InvocationResourceBudget.Start(cancellation.Token));

            Assert.False(result.Success);
            Assert.Equal("resource.execution_cancelled", result.Error!.MachineCode);
            Assert.Equal(initial.Revision, session.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(0, Convert.ToInt32(
                Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"]));
        }

        private static WorldState SeededWorld(int seed)
        {
            var random = new Random(seed);
            var objectCount = random.Next(4, 14);
            var root = new WorldObject(new WorldObjectId("node.root"), new WorldTypeId("fixture.hk10.root"));
            var objects = new List<WorldObject> { root };
            for (var index = 1; index < objectCount; index++)
            {
                var id = "node.item" + index.ToString("D2");
                var references = index % 2 == 0
                    ? new[] { new WorldReference(new WorldReferenceKind("fixture.root"), new WorldObjectId("node.root")) }
                    : Array.Empty<WorldReference>();
                objects.Add(new WorldObject(
                    new WorldObjectId(id),
                    new WorldTypeId("fixture.hk10.type" + random.Next(0, 5)),
                    new WorldObjectId("node.root"),
                    references));
            }

            Shuffle(objects, random);

            var extensions = new List<WorldExtensionData>();
            var extensionCount = random.Next(1, 5);
            for (var index = 0; index < extensionCount; index++)
            {
                var payload = new byte[random.Next(0, 65)];
                random.NextBytes(payload);
                extensions.Add(new WorldExtensionData(
                    "future.hk10.owner" + index,
                    1,
                    payload,
                    new WorldObjectId("node.item" + (index + 1).ToString("D2")),
                    new[] { new WorldReference(new WorldReferenceKind("fixture.root"), new WorldObjectId("node.root")) }));
            }
            Shuffle(extensions, random);

            return new WorldState(
                new WorldId("world.hk10.seed" + seed),
                random.Next(0, 20),
                objects,
                extensions);
        }

        private static void Shuffle<T>(IList<T> values, Random random)
        {
            for (var index = values.Count - 1; index > 0; index--)
            {
                var target = random.Next(index + 1);
                (values[index], values[target]) = (values[target], values[index]);
            }
        }

        private static ComposedContract ComposePortable(PortableWorldAuthoringSession session)
        {
            return CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
        }

        private static IReadOnlyDictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }
    }
}
