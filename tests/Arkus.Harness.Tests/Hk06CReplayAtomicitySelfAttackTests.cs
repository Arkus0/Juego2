using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Causal RED->GREEN controls for HK06C aggregate atomicity and canonical-authority reuse.
    /// The forged journals remain internally valid according to the accepted HK06A identity
    /// binding, so generic replay parsing alone cannot make these tests green.
    /// </summary>
    public sealed class Hk06CReplayAtomicitySelfAttackTests
    {
        [Fact]
        public void IntegrityValidLateResultDivergenceCannotPublishStagedStateOrJournal()
        {
            var fixture = BuildTwoStepSource();
            var forgedJournal = ForgeLateResultDivergence(fixture.Journal);

            var target = ImportCleanBase(fixture.BaseSnapshot);
            var targetContract = Compose(target);
            var beforeRevision = target.Current.Revision;
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(target.Current);

            var replay = DispatchReplay(targetContract, target, forgedJournal);

            Assert.False(replay.Success);
            Assert.Equal("world.replay.result_divergence", replay.Error!.MachineCode);
            AssertUnchanged(targetContract, target, beforeRevision, beforeHash);
        }

        [Fact]
        public void IntegrityValidSemanticallyInvalidLaterStepMustBeRejectedByCanonicalValidationWithoutPartialPublish()
        {
            var fixture = BuildTwoStepSource();
            var forgedJournal = ForgeCanonicalValidationFailure(fixture.Journal);

            var target = ImportCleanBase(fixture.BaseSnapshot);
            var targetContract = Compose(target);
            var beforeRevision = target.Current.Revision;
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(target.Current);

            var replay = DispatchReplay(targetContract, target, forgedJournal);

            Assert.False(replay.Success);
            Assert.Equal("world.replay.step_failed", replay.Error!.MachineCode);
            Assert.Equal(2L, Convert.ToInt64(replay.Error.Context["sequence"], CultureInfo.InvariantCulture));
            AssertUnchanged(targetContract, target, beforeRevision, beforeHash);
        }

        private static SourceFixture BuildTwoStepSource()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var source = new PortableWorldAuthoringSession(initial);
            var sourceContract = Compose(source);
            var baseSnapshot = Snapshot(sourceContract);

            Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    source.Current,
                    "request.hk06c.atomicity.first",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.atomicity-first")));
            Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    source.Current,
                    "request.hk06c.atomicity.second",
                    Hk04TransactionalMutationTests.PutObject("node.child", "fixture.atomicity-second")));

            return new SourceFixture(
                baseSnapshot,
                Hk06AProvenanceJournalTests.ReadJournal(sourceContract).Data!);
        }

        private static CapabilityInvocationResult DispatchReplay(
            ComposedContract contract,
            PortableWorldAuthoringSession target,
            IReadOnlyDictionary<string, object?> journal)
        {
            return contract.Dispatch(
                WorldReplayContract.ReplayName,
                Hk04TransactionalMutationTests.ExactVersion(),
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["expectedRevision"] = target.Current.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target.Current),
                    ["journal"] = journal
                });
        }

        private static void AssertUnchanged(
            ComposedContract targetContract,
            PortableWorldAuthoringSession target,
            long beforeRevision,
            string beforeHash)
        {
            Assert.Equal(beforeRevision, target.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(
                0,
                Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(targetContract).Data!["entryCount"], CultureInfo.InvariantCulture));
        }

        private static IReadOnlyDictionary<string, object?> ForgeLateResultDivergence(
            IReadOnlyDictionary<string, object?> source)
        {
            var journal = (Dictionary<string, object?>)Clone(source)!;
            var entries = (List<object?>)journal["entries"]!;
            Assert.Equal(2, entries.Count);
            var second = (Dictionary<string, object?>)entries[1]!;
            var result = (Dictionary<string, object?>)second["result"]!;

            // Use a canonical but deliberately false result hash and keep the top-level current
            // anchor consistent with it. Recompute entryId independently so the HK06A integrity
            // parser accepts the evidence and the defect is exposed only after actual execution.
            result["hash"] = new string('f', 64);
            ((Dictionary<string, object?>)journal["current"]!)["hash"] = result["hash"];
            second["entryId"] = ComputeEntryId(second);
            return journal;
        }

        private static IReadOnlyDictionary<string, object?> ForgeCanonicalValidationFailure(
            IReadOnlyDictionary<string, object?> source)
        {
            var journal = (Dictionary<string, object?>)Clone(source)!;
            var entries = (List<object?>)journal["entries"]!;
            Assert.Equal(2, entries.Count);
            var second = (Dictionary<string, object?>)entries[1]!;
            var request = (Dictionary<string, object?>)second["request"]!;

            // Syntactically valid HK04 mutation request, but removing node.root leaves the accepted
            // micro-world containment graph invalid because node.child still names it as container.
            // A private replay mutator that bypasses HK05 could publish this; the canonical committer
            // must instead fail the staged second step.
            request["operations"] = new List<object?>
            {
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "remove-object",
                    ["id"] = "node.root"
                }
            };

            var identity = (Dictionary<string, object?>)second["requestIdentity"]!;
            identity["fingerprint"] = ComputeRemoveObjectRequestFingerprint(request, "node.root");
            second["entryId"] = ComputeEntryId(second);
            return journal;
        }

        private static string ComputeRemoveObjectRequestFingerprint(
            IReadOnlyDictionary<string, object?> request,
            string objectId)
        {
            var builder = new StringBuilder();
            builder.Append("arkus-world-mutation-request-v2").Append('\n');
            builder.Append(Convert.ToInt64(request["expectedRevision"], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)).Append('\n');
            builder.Append((string)request["expectedHash"]!).Append('\n');
            builder.Append(1).Append('\t').Append(objectId).Append('\n');
            return Sha256(builder.ToString());
        }

        private static string ComputeEntryId(IReadOnlyDictionary<string, object?> entry)
        {
            var builder = new StringBuilder();
            var capability = (IReadOnlyDictionary<string, object?>)entry["capability"]!;
            var identity = (IReadOnlyDictionary<string, object?>)entry["requestIdentity"]!;
            var baseAnchor = (IReadOnlyDictionary<string, object?>)entry["base"]!;
            var resultAnchor = (IReadOnlyDictionary<string, object?>)entry["result"]!;

            Append(builder, (string)entry["schemaId"]!);
            Append(builder, Convert.ToInt64(entry["sequence"], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));
            Append(builder, (string)capability["name"]!);
            Append(builder, (string)capability["version"]!);
            Append(builder, (string)identity["idempotencyKey"]!);
            Append(builder, (string)identity["fingerprint"]!);
            Append(builder, (string)baseAnchor["worldId"]!);
            Append(builder, Convert.ToInt64(baseAnchor["stateSchemaVersion"], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));
            Append(builder, Convert.ToInt64(baseAnchor["revision"], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));
            Append(builder, (string)baseAnchor["hash"]!);
            Append(builder, Convert.ToInt64(resultAnchor["revision"], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));
            Append(builder, (string)resultAnchor["hash"]!);

            foreach (var raw in (IReadOnlyList<object?>)entry["affectedResources"]!)
                Append(builder, (string)raw!);

            return Sha256(builder.ToString());
        }

        private static void Append(StringBuilder builder, string value)
        {
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture))
                .Append(':')
                .Append(value)
                .Append('\n');
        }

        private static string Sha256(string value)
        {
            using var sha = SHA256.Create();
            var digest = sha.ComputeHash(new UTF8Encoding(false, true).GetBytes(value));
            var text = new StringBuilder(digest.Length * 2);
            for (var index = 0; index < digest.Length; index++)
                text.Append(digest[index].ToString("x2", CultureInfo.InvariantCulture));
            return text.ToString();
        }

        private static PortableWorldAuthoringSession ImportCleanBase(IReadOnlyDictionary<string, object?> snapshot)
        {
            var bootstrap = new WorldState(new WorldId("world.hk06c.atomicity-bootstrap"), 0, Array.Empty<WorldObject>());
            var target = new PortableWorldAuthoringSession(bootstrap);
            var contract = Compose(target);
            Hk04TransactionalMutationTests.Success(
                contract,
                WorldPortabilityContract.ImportName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "request.hk06c.atomicity-import",
                    ["expectedRevision"] = target.Current.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target.Current),
                    ["snapshot"] = snapshot
                });
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

        private static object? Clone(object? value)
        {
            if (value is IReadOnlyDictionary<string, object?> map)
            {
                var copy = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var pair in map) copy.Add(pair.Key, Clone(pair.Value));
                return copy;
            }

            if (value is IReadOnlyList<object?> list)
            {
                var copy = new List<object?>();
                for (var index = 0; index < list.Count; index++) copy.Add(Clone(list[index]));
                return copy;
            }

            return value;
        }

        private sealed class SourceFixture
        {
            public SourceFixture(
                IReadOnlyDictionary<string, object?> baseSnapshot,
                IReadOnlyDictionary<string, object?> journal)
            {
                BaseSnapshot = baseSnapshot;
                Journal = journal;
            }

            public IReadOnlyDictionary<string, object?> BaseSnapshot { get; }
            public IReadOnlyDictionary<string, object?> Journal { get; }
        }
    }
}
