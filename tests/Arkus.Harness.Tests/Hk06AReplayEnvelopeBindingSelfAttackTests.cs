using System;
using System.Collections.Generic;
using System.Reflection;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06AReplayEnvelopeBindingSelfAttackTests
    {
        [Fact]
        public void SchemaValidReplayEnvelopeCorruptionBreaksProvenanceBindingAcrossOperationGrammar()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);

            var create = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.binding-create",
                Hk04TransactionalMutationTests.PutObject(
                    "node.extra",
                    "fixture.item",
                    "node.root",
                    Hk04TransactionalMutationTests.Reference("fixture.root", "node.root")),
                Hk06AProvenanceJournalTests.PutExtension(
                    "future.alpha",
                    2,
                    new byte[] { 0x44, 0x55, 0x66 },
                    "node.extra",
                    Hk04TransactionalMutationTests.Reference("fixture.root", "node.root")));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, create);

            var afterCreate = session.Current;
            var remove = Hk04TransactionalMutationTests.Request(
                afterCreate,
                "request.hk06a.binding-remove",
                RemoveExtension("future.alpha", 2, "node.extra"),
                Hk04TransactionalMutationTests.RemoveObject("node.extra"));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, remove);

            var journal = Hk06AProvenanceJournalTests.ReadJournal(contract).Data!;
            var entries = Hk06AProvenanceJournalTests.Entries(journal);
            Assert.Equal(2, entries.Count);

            var createEntry = entries[0];
            AssertCorruptionRejected(createEntry, request => request["idempotencyKey"] = "request.hk06a.binding-other");
            AssertCorruptionRejected(createEntry, request => request["expectedRevision"] = initial.Revision + 1L);
            AssertCorruptionRejected(createEntry, request => request["expectedHash"] = new string('a', 64));
            AssertCorruptionRejected(createEntry, request => Operation(request, 0)["id"] = "node.other");
            AssertCorruptionRejected(createEntry, request => Operation(request, 0)["typeId"] = "fixture.other");
            AssertCorruptionRejected(createEntry, request => Operation(request, 0)["containerId"] = "node.peer");
            AssertCorruptionRejected(createEntry, request => Reference(Operation(request, 0), "references", 0)["kind"] = "fixture.other");
            AssertCorruptionRejected(createEntry, request => Reference(Operation(request, 0), "references", 0)["targetId"] = "node.peer");
            AssertCorruptionRejected(createEntry, request => Operation(request, 1)["owner"] = "future.beta");
            AssertCorruptionRejected(createEntry, request => Operation(request, 1)["schemaVersion"] = 3);
            AssertCorruptionRejected(createEntry, request => Operation(request, 1)["subjectId"] = "node.root");
            AssertCorruptionRejected(createEntry, request => Reference(Operation(request, 1), "dependencies", 0)["kind"] = "fixture.other");
            AssertCorruptionRejected(createEntry, request => Reference(Operation(request, 1), "dependencies", 0)["targetId"] = "node.peer");
            AssertCorruptionRejected(createEntry, request => Operation(request, 1)["payloadBase64"] = Convert.ToBase64String(new byte[] { 0x44, 0x55, 0x67 }));

            var removeEntry = entries[1];
            AssertCorruptionRejected(removeEntry, request => Operation(request, 0)["owner"] = "future.beta");
            AssertCorruptionRejected(removeEntry, request => Operation(request, 0)["schemaVersion"] = 3);
            AssertCorruptionRejected(removeEntry, request => Operation(request, 0)["subjectId"] = "node.root");
            AssertCorruptionRejected(removeEntry, request => Operation(request, 1)["id"] = "node.peer");
        }

        [Fact]
        public void CanonicalLookingButSemanticallyWrongEntryIdentityIsRejected()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.identity-self-attack",
                Hk04TransactionalMutationTests.PutObject("node.extra", "fixture.item"));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, request);

            var entry = Assert.Single(Hk06AProvenanceJournalTests.Entries(
                Hk06AProvenanceJournalTests.ReadJournal(contract).Data!));
            var wrongCanonicalId = new string('0', 64);
            Assert.NotEqual(entry["entryId"], wrongCanonicalId);

            var error = InvokeEntryConstructor(entry, DeepCloneMap(Hk04TransactionalMutationTests.Map(entry, "request")), wrongCanonicalId);
            Assert.IsType<InvalidOperationException>(error);
            Assert.Contains("identity", error.Message, StringComparison.OrdinalIgnoreCase);
        }

        private static IReadOnlyDictionary<string, object?> RemoveExtension(string owner, int schemaVersion, string subjectId)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "remove-extension",
                ["owner"] = owner,
                ["schemaVersion"] = schemaVersion,
                ["subjectId"] = subjectId
            };
        }

        private static void AssertCorruptionRejected(
            IReadOnlyDictionary<string, object?> entry,
            Action<Dictionary<string, object?>> corrupt)
        {
            var request = DeepCloneMap(Hk04TransactionalMutationTests.Map(entry, "request"));
            corrupt(request);
            var error = InvokeEntryConstructor(entry, request, (string)entry["entryId"]!);
            Assert.IsType<InvalidOperationException>(error);
            Assert.Contains("normalized accepted request", error.Message, StringComparison.OrdinalIgnoreCase);
        }

        private static Exception InvokeEntryConstructor(
            IReadOnlyDictionary<string, object?> entry,
            IReadOnlyDictionary<string, object?> normalizedRequest,
            string entryId)
        {
            var entryType = typeof(AuthoredWorldAnchor).Assembly.GetType(
                "Arkus.Game.Authoring.WorldMutationProvenanceEntry",
                throwOnError: true)!;
            ConstructorInfo? constructor = null;
            foreach (var candidate in entryType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (candidate.GetParameters().Length == 10)
                {
                    constructor = candidate;
                    break;
                }
            }

            Assert.NotNull(constructor);
            var capability = Hk04TransactionalMutationTests.Map(entry, "capability");
            var identity = Hk04TransactionalMutationTests.Map(entry, "requestIdentity");
            var baseAnchor = Anchor(Hk04TransactionalMutationTests.Map(entry, "base"));
            var resultAnchor = Anchor(Hk04TransactionalMutationTests.Map(entry, "result"));
            var affected = Hk04TransactionalMutationTests.Strings(entry, "affectedResources");

            var invocation = Assert.Throws<TargetInvocationException>(() => constructor!.Invoke(new object?[]
            {
                entryId,
                (long)entry["sequence"]!,
                (string)capability["name"]!,
                (string)capability["version"]!,
                (string)identity["idempotencyKey"]!,
                (string)identity["fingerprint"]!,
                normalizedRequest,
                baseAnchor,
                resultAnchor,
                affected
            }));
            Assert.NotNull(invocation.InnerException);
            return invocation.InnerException!;
        }

        private static AuthoredWorldAnchor Anchor(IReadOnlyDictionary<string, object?> data)
        {
            return new AuthoredWorldAnchor(
                (string)data["worldId"]!,
                Convert.ToInt32(data["stateSchemaVersion"]),
                Convert.ToInt64(data["revision"]),
                (string)data["hash"]!);
        }

        private static Dictionary<string, object?> Operation(Dictionary<string, object?> request, int index)
        {
            return (Dictionary<string, object?>)((List<object?>)request["operations"]!)[index]!;
        }

        private static Dictionary<string, object?> Reference(
            Dictionary<string, object?> operation,
            string field,
            int index)
        {
            return (Dictionary<string, object?>)((List<object?>)operation[field]!)[index]!;
        }

        private static Dictionary<string, object?> DeepCloneMap(IReadOnlyDictionary<string, object?> source)
        {
            var result = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var pair in source) result[pair.Key] = DeepClone(pair.Value);
            return result;
        }

        private static object? DeepClone(object? value)
        {
            if (value is IReadOnlyDictionary<string, object?> map) return DeepCloneMap(map);
            if (value is IReadOnlyList<object?> list)
            {
                var copy = new List<object?>();
                foreach (var item in list) copy.Add(DeepClone(item));
                return copy;
            }

            return value;
        }
    }
}
