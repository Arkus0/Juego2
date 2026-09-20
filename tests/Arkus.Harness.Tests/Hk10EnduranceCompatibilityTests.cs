using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk10EnduranceCompatibilityTests
    {
        private const int BoundedSessionTransactions = 512;

        [Fact]
        public void ProtocolV1CompatibilityCorpusMatchesAcceptedRuntimeAndResourceEnvelope()
        {
            var root = Hk07AProcessHarness.FindRepositoryRoot();
            var path = Path.Combine(root, "tests", "Arkus.Harness.Tests", "Compatibility", "protocol-v1.json");
            Assert.True(File.Exists(path), "HK10 compatibility corpus is required and must fail closed when missing.");

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            var corpus = document.RootElement;
            Assert.Equal("arkus.hk10.protocol-v1-compatibility-corpus@1", corpus.GetProperty("schemaId").GetString());
            Assert.Equal(1, corpus.GetProperty("protocolMajor").GetInt32());

            var transport = corpus.GetProperty("referenceTransport");
            Assert.Equal("arkus.reference.jsonl@1", transport.GetProperty("contract").GetString());
            Assert.Equal(H0ResourceEnvelope.MaximumTransportFrameBytes, transport.GetProperty("maximumFrameBytes").GetInt32());
            Assert.Equal("transport.frame_too_large", transport.GetProperty("oversizeMachineCode").GetString());

            var envelope = corpus.GetProperty("resourceEnvelope");
            Assert.Equal(H0ResourceEnvelope.SchemaId, envelope.GetProperty("schemaId").GetString());
            Assert.Equal(H0ResourceEnvelope.MaximumCanonicalRequestBytes, envelope.GetProperty("maximumCanonicalArgumentBytes").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumPortableDepth, envelope.GetProperty("maximumPortableDepth").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumBatchOperations, envelope.GetProperty("maximumOperations").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumBatchPayloadBytes, envelope.GetProperty("maximumDecodedPayloadBytes").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumPageSize, envelope.GetProperty("maximumPageSize").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumCanonicalWorldBytes, envelope.GetProperty("maximumCanonicalWorldBytes").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumWorldResources, envelope.GetProperty("maximumWorldResources").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumSessionTransactions, envelope.GetProperty("maximumSessionTransactions").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumSnapshotImportReceipts, envelope.GetProperty("maximumImportReceipts").GetInt32());
            Assert.Equal(H0ResourceEnvelope.MaximumExecutionMilliseconds, envelope.GetProperty("maximumExecutionMilliseconds").GetInt32());
            Assert.Equal("process-local-checkpoint", envelope.GetProperty("durabilityLevel").GetString());
            Assert.False(envelope.GetProperty("powerLossDurabilityClaimed").GetBoolean());

            var session = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var contract = Compose(session);
            foreach (var capability in corpus.GetProperty("requiredCapabilities").EnumerateArray())
            {
                var name = capability.GetProperty("name").GetString();
                var version = ContractVersion.Parse(capability.GetProperty("version").GetString()!);
                var definition = Assert.Single(contract.Definitions.Where(candidate =>
                    string.Equals(candidate.Key.Name, name, StringComparison.Ordinal) &&
                    candidate.Key.Version.Equals(version)));
                Assert.NotNull(definition.RequestSchema);
                Assert.NotNull(definition.SuccessSchema);
                Assert.NotNull(definition.ErrorSchema);
            }
        }

        [Fact]
        public void BoundedLongAuthoringSessionExercisesInspectValidateMutateJournalSnapshotAndRestartRecovery()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new PortableWorldAuthoringSession(initial);
            var contract = Compose(session);
            var process = Process.GetCurrentProcess();
            var workingSetBefore = process.WorkingSet64;
            var managedBefore = GC.GetTotalMemory(true);
            long maximumWorkingSet = workingSetBefore;
            long maximumManaged = managedBefore;
            long lastJournalBytes = 0;
            IReadOnlyDictionary<string, object?>? lastSnapshot = null;

            for (var index = 0; index < BoundedSessionTransactions; index++)
            {
                var request = Hk04TransactionalMutationTests.Request(
                    session.Current,
                    "request.hk10.session." + index.ToString("D4", CultureInfo.InvariantCulture),
                    Hk04TransactionalMutationTests.PutObject(
                        "node.peer",
                        "fixture.hk10.session" + index.ToString("D4", CultureInfo.InvariantCulture)));
                Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, request);

                if ((index + 1) % 64 != 0) continue;

                var summary = Hk04TransactionalMutationTests.Success(
                    contract,
                    WorldInspectionContract.SummaryName,
                    Empty());
                var world = Hk04TransactionalMutationTests.Map(summary.Data!, "world");
                Assert.Equal(session.Current.Revision, Convert.ToInt64(world["revision"], CultureInfo.InvariantCulture));
                Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(session.Current), world["hash"]);

                var validation = Hk04TransactionalMutationTests.Success(
                    contract,
                    WorldValidationContract.CurrentName,
                    Empty());
                Assert.True((bool)validation.Data!["valid"]!);

                var journal = Hk06AProvenanceJournalTests.ReadJournal(contract).Data!;
                Assert.Equal(index + 1, Convert.ToInt32(journal["entryCount"], CultureInfo.InvariantCulture));
                lastJournalBytes = JsonSerializer.SerializeToUtf8Bytes(journal).LongLength;

                lastSnapshot = Hk04TransactionalMutationTests.Success(
                    contract,
                    WorldPortabilityContract.ExportName,
                    Empty()).Data!;
                var snapshotBytes = Convert.FromBase64String((string)lastSnapshot["authoredStateBase64"]!);
                Assert.InRange(snapshotBytes.Length, 1, H0ResourceEnvelope.MaximumCanonicalWorldBytes);

                process.Refresh();
                maximumWorkingSet = Math.Max(maximumWorkingSet, process.WorkingSet64);
                maximumManaged = Math.Max(maximumManaged, GC.GetTotalMemory(false));
            }

            Assert.Equal(initial.Revision + BoundedSessionTransactions, session.Current.Revision);
            Assert.True(BoundedSessionTransactions < H0ResourceEnvelope.MaximumSessionTransactions);
            Assert.NotNull(lastSnapshot);
            Assert.InRange(CanonicalWorldStateCodec.Serialize(session.Current).Length, 1, H0ResourceEnvelope.MaximumCanonicalWorldBytes);

            var finalJournal = Hk06AProvenanceJournalTests.ReadJournal(contract).Data!;
            Assert.Equal(BoundedSessionTransactions, Convert.ToInt32(finalJournal["entryCount"], CultureInfo.InvariantCulture));
            Assert.True(lastJournalBytes > 0);

            var restarted = new PortableWorldAuthoringSession(initial);
            var restartedContract = Compose(restarted);
            var import = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.hk10.restart-recovery",
                ["expectedRevision"] = restarted.Current.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(restarted.Current),
                ["snapshot"] = lastSnapshot
            };
            Hk04TransactionalMutationTests.Success(restartedContract, WorldPortabilityContract.ImportName, import);
            Assert.Equal(session.Current.Revision, restarted.Current.Revision);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(session.Current),
                CanonicalWorldStateCodec.ComputeContentHash(restarted.Current));

            process.Refresh();
            var workingSetAfter = process.WorkingSet64;
            var managedAfter = GC.GetTotalMemory(true);
            Console.WriteLine(
                "HK10_ENDURANCE transactions={0} journalBytes={1} workingSetBefore={2} workingSetAfter={3} maximumWorkingSet={4} managedBefore={5} managedAfter={6} maximumManaged={7}",
                BoundedSessionTransactions,
                lastJournalBytes,
                workingSetBefore,
                workingSetAfter,
                maximumWorkingSet,
                managedBefore,
                managedAfter,
                maximumManaged);
        }

        [Fact]
        public void SeededConflictingWritersRecoverByExplicitReplanWithoutAutomaticMergeClaim()
        {
            var seeds = new[] { 17, 31, 73, 127, 251, 509, 1021, 2039, 4093, 8191, 16381, 32749 };
            foreach (var seed in seeds)
            {
                var initial = Hk02TestFixtures.MicroWorld();
                var session = new TransactionalWorldAuthoringSession(initial);
                var contract = Hk04TransactionalMutationTests.Compose(session);
                var initialHash = CanonicalWorldStateCodec.ComputeContentHash(initial);

                var firstKey = seed % 2 == 0 ? "writer-a" : "writer-b";
                var staleKey = seed % 2 == 0 ? "writer-b" : "writer-a";
                Hk04TransactionalMutationTests.Success(
                    contract,
                    WorldMutationContract.ApplyName,
                    Hk04TransactionalMutationTests.Request(
                        initial,
                        "request.hk10." + firstKey + "." + seed,
                        Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.hk10." + firstKey + seed)));

                var acceptedHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
                var stale = contract.Dispatch(
                    WorldMutationContract.ApplyName,
                    Hk04TransactionalMutationTests.ExactVersion(),
                    Hk04TransactionalMutationTests.Request(
                        initial,
                        "request.hk10." + staleKey + "." + seed,
                        Hk04TransactionalMutationTests.PutObject("node.child", "fixture.hk10." + staleKey + seed, "node.root")));
                Assert.False(stale.Success);
                Assert.Equal("world.change.stale_revision", stale.Error!.MachineCode);
                var recovery = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(stale.Error.Context["recovery"]);
                var expected = Hk04TransactionalMutationTests.Map(recovery, "expected");
                var current = Hk04TransactionalMutationTests.Map(recovery, "current");
                Assert.Equal(initial.Revision, Convert.ToInt64(expected["revision"], CultureInfo.InvariantCulture));
                Assert.Equal(initialHash, expected["hash"]);
                Assert.Equal(session.Current.Revision, Convert.ToInt64(current["revision"], CultureInfo.InvariantCulture));
                Assert.Equal(acceptedHash, current["hash"]);

                var retry = Hk04TransactionalMutationTests.Request(
                    session.Current,
                    "request.hk10." + staleKey + "." + seed,
                    Hk04TransactionalMutationTests.PutObject("node.child", "fixture.hk10." + staleKey + seed, "node.root"));
                Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.PlanName, retry);
                Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.DryRunName, retry);
                Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, retry);

                Assert.Equal(initial.Revision + 2, session.Current.Revision);
                Assert.Equal(2, Convert.ToInt32(
                    Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"],
                    CultureInfo.InvariantCulture));
            }
        }

        private static ComposedContract Compose(PortableWorldAuthoringSession session)
        {
            return CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
        }

        private static IReadOnlyDictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }
    }
}
