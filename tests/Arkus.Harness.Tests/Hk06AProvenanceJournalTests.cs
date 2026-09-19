using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06AProvenanceJournalTests
    {
        [Fact]
        public void AcceptedSequenceProducesTruthfulDiscoverableAndDeterministicJournal()
        {
            var first = ExecuteRepresentativeSequence();
            var second = ExecuteRepresentativeSequence();

            Assert.Equal(WorldProvenanceContract.JournalSchemaId, first.Journal["schemaId"]);
            Assert.Equal(WorldProvenanceContract.EntrySchemaId, first.Journal["entrySchemaId"]);
            Assert.Equal(2, first.Journal["entryCount"]);
            AssertAnchor(Hk04TransactionalMutationTests.Map(first.Journal, "base"), first.States[0]);
            AssertAnchor(Hk04TransactionalMutationTests.Map(first.Journal, "current"), first.States[2]);

            var entries = Entries(first.Journal);
            Assert.Equal(2, entries.Count);
            AssertEntry(entries[0], 1L, "request.hk06a.first", first.States[0], first.States[1],
                "world.extension:future.alpha@2@global",
                "world.object:node.peer");
            AssertEntry(entries[1], 2L, "request.hk06a.second", first.States[1], first.States[2],
                "world.object:node.extra");

            var normalized = Hk04TransactionalMutationTests.Map(entries[0], "request");
            Assert.Equal(first.States[0].Revision, normalized["expectedRevision"]);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(first.States[0]), normalized["expectedHash"]);
            var normalizedOperations = Hk04TransactionalMutationTests.List(normalized, "operations");
            Assert.Equal(2, normalizedOperations.Count);
            Assert.Equal(
                "put-object",
                ((IReadOnlyDictionary<string, object?>)normalizedOperations[0]!)["kind"]);
            var normalizedExtension = (IReadOnlyDictionary<string, object?>)normalizedOperations[1]!;
            Assert.Equal("put-extension", normalizedExtension["kind"]);
            Assert.Equal(Convert.ToBase64String(new byte[] { 0x44, 0x55, 0x66 }), normalizedExtension["payloadBase64"]);

            var secondEntries = Entries(second.Journal);
            Assert.Equal(entries[0]["entryId"], secondEntries[0]["entryId"]);
            Assert.Equal(entries[1]["entryId"], secondEntries[1]["entryId"]);
            Assert.Equal(
                Hk04TransactionalMutationTests.Map(entries[0], "requestIdentity")["fingerprint"],
                Hk04TransactionalMutationTests.Map(secondEntries[0], "requestIdentity")["fingerprint"]);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(first.States[2]),
                CanonicalWorldStateCodec.ComputeContentHash(second.States[2]));

            var definition = FindDefinition(first.Contract, WorldProvenanceContract.ReadName);
            Assert.NotNull(definition.SuccessSchema);
            Assert.Empty(definition.SuccessSchema!.ValidateValue(first.Journal));

            var discovery = Hk04TransactionalMutationTests.Success(
                first.Contract,
                "system.describe",
                Hk01TestFixtures.EmptyRequest());
            var discovered = FindDiscoveredCapability(discovery.Data!, WorldProvenanceContract.ReadName);
            Assert.Equal("1.0", discovered["version"]);
            Assert.NotNull(discovered["successSchema"]);
        }

        [Fact]
        public void OnlyNewlyPersistedMutationsAppendEntries()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var valid = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.persist-once",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.persisted"));

            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.PlanName, valid);
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.DryRunName, valid);
            Hk04TransactionalMutationTests.Success(contract, WorldValidationContract.CurrentName, Hk01TestFixtures.EmptyRequest());
            Hk04TransactionalMutationTests.Success(contract, WorldValidationContract.ProposedName, valid);
            Hk04TransactionalMutationTests.Success(contract, WorldInspectionContract.SummaryName, Hk01TestFixtures.EmptyRequest());
            AssertJournalCount(contract, 0);

            var rejected = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.rejected",
                Hk04TransactionalMutationTests.RemoveObject("node.root"));
            var rejectedResult = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                rejected);
            Assert.False(rejectedResult.Success);
            Assert.Equal("world.change.invalid_candidate", rejectedResult.Error!.MachineCode);

            var malformed = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.hk06a.empty",
                ["expectedRevision"] = initial.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(initial),
                ["operations"] = Array.Empty<object?>()
            };
            Assert.False(contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                malformed).Success);
            AssertJournalCount(contract, 0);

            var applied = Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, valid);
            Assert.True((bool)applied.Data!["persisted"]!);
            Assert.False((bool)applied.Data["replayed"]!);
            AssertJournalCount(contract, 1);

            var retry = Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, valid);
            Assert.True((bool)retry.Data!["replayed"]!);
            AssertJournalCount(contract, 1);

            var stale = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.stale",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.stale"));
            Assert.False(contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                stale).Success);
            AssertJournalCount(contract, 1);

            // Reading the journal is itself a read-only operation and cannot append to it.
            ReadJournal(contract);
            AssertJournalCount(contract, 1);
        }

        [Fact]
        public void IndependentAuditOracleRejectsMissingFalseOrCorruptedProvenance()
        {
            var run = ExecuteRepresentativeSequence();
            var expected = new[]
            {
                new ExpectedTransition(run.States[0], run.States[1], "request.hk06a.first", new[]
                {
                    "world.extension:future.alpha@2@global",
                    "world.object:node.peer"
                }),
                new ExpectedTransition(run.States[1], run.States[2], "request.hk06a.second", new[]
                {
                    "world.object:node.extra"
                })
            };
            Assert.Empty(Audit(run.Journal, expected));

            var missing = DeepCloneMap(run.Journal);
            missing["entryCount"] = 1;
            ((List<object?>)missing["entries"]!).RemoveAt(1);
            Assert.Contains("entry-count-mismatch", Audit(missing, expected));

            var wrongBase = DeepCloneMap(run.Journal);
            Entry(wrongBase, 0, "base")["hash"] = new string('0', 64);
            Assert.Contains("base-anchor-mismatch:0", Audit(wrongBase, expected));

            var wrongResult = DeepCloneMap(run.Journal);
            Entry(wrongResult, 0, "result")["revision"] = run.States[0].Revision;
            Assert.Contains("result-anchor-mismatch:0", Audit(wrongResult, expected));

            var wrongResources = DeepCloneMap(run.Journal);
            EntryMap(wrongResources, 0)["affectedResources"] = new List<object?> { "world.object:invented" };
            Assert.Contains("affected-resource-mismatch:0", Audit(wrongResources, expected));

            var wrongIdentity = DeepCloneMap(run.Journal);
            EntryMap(wrongIdentity, 0)["entryId"] = "not-a-canonical-entry-id";
            Assert.Contains("entry-id-invalid:0", Audit(wrongIdentity, expected));

            // A synthetic successful entry after a non-persisting operation is an extra transition.
            Assert.Contains("entry-count-mismatch", Audit(run.Journal, Array.Empty<ExpectedTransition>()));
        }

        [Fact]
        public async Task ConcurrentCommitRacePublishesExactlyOneMatchingJournalEntry()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var requestA = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.concurrent-a",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.concurrent-a"));
            var requestB = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.concurrent-b",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.concurrent-b"));
            using var start = new Barrier(2);

            Task<KeyValuePair<string, CapabilityInvocationResult>> Run(
                string idempotencyKey,
                IReadOnlyDictionary<string, object?> request)
            {
                return Task.Run(() =>
                {
                    start.SignalAndWait();
                    return new KeyValuePair<string, CapabilityInvocationResult>(
                        idempotencyKey,
                        contract.Dispatch(
                            WorldMutationContract.ApplyName,
                            Hk04TransactionalMutationTests.ExactVersion(),
                            request));
                });
            }

            var results = await Task.WhenAll(
                Run("request.hk06a.concurrent-a", requestA),
                Run("request.hk06a.concurrent-b", requestB));
            string? acceptedKey = null;
            foreach (var result in results)
            {
                if (result.Value.Success) acceptedKey = result.Key;
            }

            Assert.NotNull(acceptedKey);
            var journal = ReadJournal(contract).Data!;
            var entry = Assert.Single(Entries(journal));
            Assert.Equal(1L, entry["sequence"]);
            Assert.Equal(
                acceptedKey,
                Hk04TransactionalMutationTests.Map(entry, "requestIdentity")["idempotencyKey"]);
            AssertAnchor(Hk04TransactionalMutationTests.Map(entry, "result"), session.Current);
        }

        internal static CapabilityInvocationResult ReadJournal(ComposedContract contract)
        {
            return Hk04TransactionalMutationTests.Success(
                contract,
                WorldProvenanceContract.ReadName,
                Hk01TestFixtures.EmptyRequest());
        }

        internal static IReadOnlyList<IReadOnlyDictionary<string, object?>> Entries(
            IReadOnlyDictionary<string, object?> journal)
        {
            var entries = new List<IReadOnlyDictionary<string, object?>>();
            foreach (var value in Hk04TransactionalMutationTests.List(journal, "entries"))
            {
                entries.Add((IReadOnlyDictionary<string, object?>)value!);
            }

            return entries.AsReadOnly();
        }

        internal static IReadOnlyDictionary<string, object?> PutExtension(
            string owner,
            int schemaVersion,
            byte[] payload,
            string? subjectId = null,
            params IReadOnlyDictionary<string, object?>[] dependencies)
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-extension",
                ["owner"] = owner,
                ["schemaVersion"] = schemaVersion,
                ["payloadBase64"] = Convert.ToBase64String(payload)
            };
            if (subjectId != null) data["subjectId"] = subjectId;
            if (dependencies.Length != 0)
            {
                var values = new List<object?>();
                foreach (var dependency in dependencies) values.Add(dependency);
                data["dependencies"] = values.AsReadOnly();
            }

            return Hk03InspectionTests.ReadOnlyMap(data);
        }

        private static SequenceResult ExecuteRepresentativeSequence()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var first = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06a.first",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.updated"),
                PutExtension("future.alpha", 2, new byte[] { 0x44, 0x55, 0x66 }));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, first);
            var afterFirst = session.Current;

            var second = Hk04TransactionalMutationTests.Request(
                afterFirst,
                "request.hk06a.second",
                Hk04TransactionalMutationTests.PutObject(
                    "node.extra",
                    "fixture.item",
                    "node.root",
                    Hk04TransactionalMutationTests.Reference("fixture.root", "node.root")));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, second);
            var afterSecond = session.Current;
            return new SequenceResult(
                contract,
                new[] { initial, afterFirst, afterSecond },
                ReadJournal(contract).Data!);
        }

        private static void AssertEntry(
            IReadOnlyDictionary<string, object?> entry,
            long sequence,
            string idempotencyKey,
            WorldState before,
            WorldState after,
            params string[] affectedResources)
        {
            Assert.Equal(WorldProvenanceContract.EntrySchemaId, entry["schemaId"]);
            Assert.Equal(sequence, entry["sequence"]);
            AssertCanonicalHash((string)entry["entryId"]!);

            var capability = Hk04TransactionalMutationTests.Map(entry, "capability");
            Assert.Equal(WorldMutationContract.ApplyName, capability["name"]);
            Assert.Equal(WorldMutationContract.ContractVersionText, capability["version"]);

            var identity = Hk04TransactionalMutationTests.Map(entry, "requestIdentity");
            Assert.Equal(idempotencyKey, identity["idempotencyKey"]);
            AssertCanonicalHash((string)identity["fingerprint"]!);
            var request = Hk04TransactionalMutationTests.Map(entry, "request");
            Assert.Equal(idempotencyKey, request["idempotencyKey"]);

            AssertAnchor(Hk04TransactionalMutationTests.Map(entry, "base"), before);
            AssertAnchor(Hk04TransactionalMutationTests.Map(entry, "result"), after);
            Assert.Equal(affectedResources, Hk04TransactionalMutationTests.Strings(entry, "affectedResources"));
        }

        private static void AssertAnchor(IReadOnlyDictionary<string, object?> anchor, WorldState state)
        {
            Assert.Equal(state.Id.Value, anchor["worldId"]);
            Assert.Equal(state.SchemaVersion, anchor["stateSchemaVersion"]);
            Assert.Equal(state.Revision, anchor["revision"]);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(state), anchor["hash"]);
        }

        private static void AssertCanonicalHash(string value)
        {
            Assert.Equal(64, value.Length);
            foreach (var character in value)
            {
                Assert.True((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f'));
            }
        }

        private static void AssertJournalCount(ComposedContract contract, int expected)
        {
            Assert.Equal(expected, ReadJournal(contract).Data!["entryCount"]);
        }

        private static CapabilityDefinition FindDefinition(ComposedContract contract, string name)
        {
            foreach (var definition in contract.Definitions)
            {
                if (string.Equals(definition.Key.Name, name, StringComparison.Ordinal)) return definition;
            }

            throw new InvalidOperationException("Missing definition " + name);
        }

        private static IReadOnlyDictionary<string, object?> FindDiscoveredCapability(
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

        private static IReadOnlyList<string> Audit(
            IReadOnlyDictionary<string, object?> journal,
            IReadOnlyList<ExpectedTransition> expected)
        {
            var issues = new List<string>();
            var entries = Entries(journal);
            if (entries.Count != expected.Count || !Equals(journal["entryCount"], expected.Count))
            {
                issues.Add("entry-count-mismatch");
            }

            var count = entries.Count < expected.Count ? entries.Count : expected.Count;
            for (var index = 0; index < count; index++)
            {
                var entry = entries[index];
                if (!Equals(entry["sequence"], (long)index + 1L)) issues.Add("sequence-mismatch:" + index);
                if (!(entry["entryId"] is string entryId) || !IsCanonicalHash(entryId)) issues.Add("entry-id-invalid:" + index);
                if (!AnchorMatches(Hk04TransactionalMutationTests.Map(entry, "base"), expected[index].Before))
                    issues.Add("base-anchor-mismatch:" + index);
                if (!AnchorMatches(Hk04TransactionalMutationTests.Map(entry, "result"), expected[index].After))
                    issues.Add("result-anchor-mismatch:" + index);
                var identity = Hk04TransactionalMutationTests.Map(entry, "requestIdentity");
                if (!Equals(identity["idempotencyKey"], expected[index].IdempotencyKey))
                    issues.Add("request-identity-mismatch:" + index);

                var actualResources = new HashSet<string>(
                    Hk04TransactionalMutationTests.Strings(entry, "affectedResources"),
                    StringComparer.Ordinal);
                var expectedResources = new HashSet<string>(expected[index].AffectedResources, StringComparer.Ordinal);
                if (!actualResources.SetEquals(expectedResources)) issues.Add("affected-resource-mismatch:" + index);
            }

            return issues.AsReadOnly();
        }

        private static bool AnchorMatches(IReadOnlyDictionary<string, object?> anchor, WorldState state)
        {
            return Equals(anchor["worldId"], state.Id.Value) &&
                Equals(anchor["stateSchemaVersion"], state.SchemaVersion) &&
                Equals(anchor["revision"], state.Revision) &&
                Equals(anchor["hash"], CanonicalWorldStateCodec.ComputeContentHash(state));
        }

        private static bool IsCanonicalHash(string value)
        {
            if (value.Length != 64) return false;
            foreach (var character in value)
            {
                if (!((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f'))) return false;
            }

            return true;
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

        private static Dictionary<string, object?> EntryMap(Dictionary<string, object?> journal, int index)
        {
            return (Dictionary<string, object?>)((List<object?>)journal["entries"]!)[index]!;
        }

        private static Dictionary<string, object?> Entry(
            Dictionary<string, object?> journal,
            int index,
            string anchor)
        {
            return (Dictionary<string, object?>)EntryMap(journal, index)[anchor]!;
        }

        private sealed class SequenceResult
        {
            public SequenceResult(
                ComposedContract contract,
                IReadOnlyList<WorldState> states,
                IReadOnlyDictionary<string, object?> journal)
            {
                Contract = contract;
                States = states;
                Journal = journal;
            }

            public ComposedContract Contract { get; }
            public IReadOnlyList<WorldState> States { get; }
            public IReadOnlyDictionary<string, object?> Journal { get; }
        }

        private sealed class ExpectedTransition
        {
            public ExpectedTransition(
                WorldState before,
                WorldState after,
                string idempotencyKey,
                IReadOnlyList<string> affectedResources)
            {
                Before = before;
                After = after;
                IdempotencyKey = idempotencyKey;
                AffectedResources = affectedResources;
            }

            public WorldState Before { get; }
            public WorldState After { get; }
            public string IdempotencyKey { get; }
            public IReadOnlyList<string> AffectedResources { get; }
        }
    }
}
