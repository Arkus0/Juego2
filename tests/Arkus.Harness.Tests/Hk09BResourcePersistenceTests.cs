using System;
using System.Collections.Generic;
using System.Globalization;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk09BResourcePersistenceTests
    {
        private static readonly ContractVersionRange ExactV1 =
            ContractVersionRange.Exact(new ContractVersion(1, 0));

        [Fact]
        public void EnforcedH0EnvelopeIsMachineReadableAndMatchesCanonicalMetadata()
        {
            var contract = Compose(new PortableWorldAuthoringSession(EmptyWorld("world.hk09b.envelope")));
            var result = Success(contract, "system.resource-envelope.describe", Empty()).Data!;

            Assert.Equal(H0ResourceEnvelope.SchemaId, result["schemaId"]);
            Assert.Equal(
                H0ResourceEnvelope.MaximumCanonicalRequestBytes,
                Convert.ToInt32(Map(result, "request")["maximumCanonicalArgumentBytes"], CultureInfo.InvariantCulture));
            Assert.Equal(
                H0ResourceEnvelope.MaximumBatchOperations,
                Convert.ToInt32(Map(result, "mutation")["maximumOperations"], CultureInfo.InvariantCulture));
            Assert.Equal(
                H0ResourceEnvelope.MaximumPageSize,
                Convert.ToInt32(Map(result, "query")["maximumPageSize"], CultureInfo.InvariantCulture));
            Assert.Equal("process-local-checkpoint", Map(result, "persistence")["durabilityLevel"]);
            Assert.False((bool)Map(result, "persistence")["powerLossDurabilityClaimed"]!);

            var definition = FindDefinition(contract, "system.resource-envelope.describe");
            Assert.Empty(definition.SuccessSchema!.ValidateValue(result));
            var apply = FindDefinition(contract, WorldMutationContract.ApplyName);
            Assert.Equal(H0ResourceEnvelope.MaximumBatchOperations, apply.Batching!.MaximumItems);
        }

        [Fact]
        public void OversizeDeepBatchPayloadAndPageRequestsFailBeforeCanonicalEffect()
        {
            var initial = EmptyWorld("world.hk09b.resource-negatives");
            var session = new PortableWorldAuthoringSession(initial);
            using var projection = new NeutralProjectionService(Compose(session));

            AssertResource(
                Invoke(projection, "oversize", "system.describe", new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["padding"] = new string('x', H0ResourceEnvelope.MaximumCanonicalRequestBytes + 1)
                }),
                "resource.request_bytes_exceeded",
                "canonicalArgumentBytes");

            AssertResource(
                Invoke(projection, "deep", "system.describe", DeepArguments(H0ResourceEnvelope.MaximumPortableDepth + 2)),
                "resource.nesting_depth_exceeded",
                "portableDepth");

            var tooMany = new List<object?>();
            for (var index = 0; index <= H0ResourceEnvelope.MaximumBatchOperations; index++)
                tooMany.Add(PutObject("too-many." + index.ToString("D3", CultureInfo.InvariantCulture)));
            AssertResource(
                Invoke(projection, "batch-count", WorldMutationContract.ApplyName,
                    MutationRequest(initial, "request.hk09b.too-many", tooMany)),
                "resource.batch_operations_exceeded",
                "batchOperations");

            var payloadOperations = new List<object?>();
            for (var index = 0; index < 3; index++)
                payloadOperations.Add(PutExtension("fixture.hk09b.payload-" + index, new byte[200 * 1024]));
            AssertResource(
                Invoke(projection, "batch-bytes", WorldMutationContract.ApplyName,
                    MutationRequest(initial, "request.hk09b.too-many-bytes", payloadOperations)),
                "resource.batch_payload_exceeded",
                "decodedBatchPayloadBytes");

            AssertResource(
                Invoke(projection, "page", WorldInspectionContract.ObjectQueryName,
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["limit"] = H0ResourceEnvelope.MaximumPageSize + 1
                    }),
                "resource.page_size_exceeded",
                "pageItems");

            AssertAnchorAndJournal(session, Compose(session), initial.Revision,
                CanonicalWorldStateCodec.ComputeContentHash(initial), 0);
        }

        [Fact]
        public void RepresentativeNinetySixOperationIntentFitsMeasuredEnvelopeAndCommitsOnce()
        {
            var initial = EmptyWorld("world.hk09b.representative");
            var session = new PortableWorldAuthoringSession(initial);
            using var projection = new NeutralProjectionService(Compose(session));
            var operations = RepresentativeOperations();
            var request = MutationRequest(initial, "request.hk09b.representative", operations);

            Assert.Equal(96, operations.Count);
            Assert.True(PortableData.TryMeasure(
                request,
                H0ResourceEnvelope.MaximumPortableDepth,
                H0ResourceEnvelope.MaximumCanonicalRequestBytes,
                out var metrics,
                out var issue),
                issue?.Code + ":" + issue?.Path);
            Assert.True(metrics.Utf8JsonBytes < H0ResourceEnvelope.MaximumCanonicalRequestBytes);
            Assert.True(metrics.MaximumDepth <= H0ResourceEnvelope.MaximumPortableDepth);

            var outcome = Invoke(projection, "representative", WorldMutationContract.ApplyName, request);
            Assert.True(outcome.Success, outcome.Error?.MachineCode);
            Assert.Equal(1, session.Current.Revision);
            Assert.Equal(96, session.Current.Objects.Count + session.Current.Extensions.Count);
            Assert.Equal(1, JournalCount(Compose(session)));
        }

        [Fact]
        public void ExpiredAndInterruptedMutationBudgetsPublishNeitherStateReceiptNorJournal()
        {
            var initial = EmptyWorld("world.hk09b.mutation-interrupt");
            var session = new PortableWorldAuthoringSession(initial);
            var contract = Compose(session);
            var request = MutationRequest(
                initial,
                "request.hk09b.mutation-interrupt",
                new object?[] { PutObject("checkpoint.item") });

            var expired = contract.Dispatch(
                WorldMutationContract.ApplyName,
                ExactV1,
                request,
                InvocationResourceBudget.ExpiredForTesting());
            Assert.False(expired.Success);
            Assert.Equal("resource.execution_budget_exceeded", expired.Error!.MachineCode);
            AssertAnchorAndJournal(session, contract, 0, CanonicalWorldStateCodec.ComputeContentHash(initial), 0);

            var interrupted = contract.Dispatch(
                WorldMutationContract.ApplyName,
                ExactV1,
                request,
                InvocationResourceBudget.InterruptedAtPublication("canonical-mutation"));
            Assert.False(interrupted.Success);
            Assert.Equal("resource.persistence_interrupted", interrupted.Error!.MachineCode);
            AssertAnchorAndJournal(session, contract, 0, CanonicalWorldStateCodec.ComputeContentHash(initial), 0);

            var accepted = Success(contract, WorldMutationContract.ApplyName, request);
            Assert.False((bool)accepted.Data!["replayed"]!);
            var retry = Success(contract, WorldMutationContract.ApplyName, request);
            Assert.True((bool)retry.Data!["replayed"]!);
            AssertAnchorAndJournal(
                session,
                contract,
                1,
                CanonicalWorldStateCodec.ComputeContentHash(session.Current),
                1);
        }

        [Fact]
        public void InvalidOversizedAndInterruptedImportCannotReplaceStateHistoryOrEvidence()
        {
            var source = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var sourceContract = Compose(source);
            var snapshot = Success(sourceContract, WorldPortabilityContract.ExportName, Empty()).Data!;

            var targetInitial = EmptyWorld("world.hk09b.import-target");
            var target = new PortableWorldAuthoringSession(targetInitial);
            var targetContract = Compose(target);
            Success(
                targetContract,
                WorldMutationContract.ApplyName,
                MutationRequest(target.Current, "request.hk09b.local-history", new object?[] { PutObject("local.item") }));
            var beforeRevision = target.Current.Revision;
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(target.Current);
            var beforeJournal = JournalCount(targetContract);

            var unsupported = CloneMap(snapshot);
            unsupported["snapshotVersion"] = 2;
            AssertImportRejected(
                target,
                targetContract,
                ImportRequest(target.Current, unsupported, "request.hk09b.unsupported"),
                "world.snapshot.unsupported_version",
                beforeRevision,
                beforeHash,
                beforeJournal);

            var oversized = CloneMap(snapshot);
            oversized["authoredStateBase64"] = new string(
                'A',
                ((H0ResourceEnvelope.MaximumCanonicalWorldBytes + 2) / 3 * 4) + 4);
            AssertImportRejected(
                target,
                targetContract,
                ImportRequest(target.Current, oversized, "request.hk09b.oversized"),
                "resource.snapshot_bytes_exceeded",
                beforeRevision,
                beforeHash,
                beforeJournal);

            var request = ImportRequest(target.Current, snapshot, "request.hk09b.interrupted-import");
            var interrupted = targetContract.Dispatch(
                WorldPortabilityContract.ImportName,
                ExactV1,
                request,
                InvocationResourceBudget.InterruptedAtPublication("snapshot-import"));
            Assert.False(interrupted.Success);
            Assert.Equal("resource.persistence_interrupted", interrupted.Error!.MachineCode);
            AssertAnchorAndJournal(target, targetContract, beforeRevision, beforeHash, beforeJournal);

            var accepted = Success(targetContract, WorldPortabilityContract.ImportName, request);
            Assert.False((bool)accepted.Data!["replayed"]!);
            Assert.Equal(0, JournalCount(targetContract));
            var retry = Success(targetContract, WorldPortabilityContract.ImportName, request);
            Assert.True((bool)retry.Data!["replayed"]!);
            Assert.Equal(accepted.Data["rebaseEvidence"], retry.Data!["rebaseEvidence"]);
        }

        [Fact]
        public void InterruptedReplayCannotPublishStagedStateOrProvenance()
        {
            var initial = EmptyWorld("world.hk09b.replay");
            var source = new PortableWorldAuthoringSession(initial);
            var sourceContract = Compose(source);
            Success(
                sourceContract,
                WorldMutationContract.ApplyName,
                MutationRequest(source.Current, "request.hk09b.replay-source", new object?[] { PutObject("replayed.item") }));
            var journal = Success(sourceContract, WorldProvenanceContract.ReadName, Empty()).Data!;

            var target = new PortableWorldAuthoringSession(initial);
            var targetContract = Compose(target);
            var replayRequest = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["expectedRevision"] = target.Current.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target.Current),
                ["journal"] = journal
            };
            var interrupted = targetContract.Dispatch(
                WorldReplayContract.ReplayName,
                ExactV1,
                replayRequest,
                InvocationResourceBudget.InterruptedAtPublication("journal-replay"));

            Assert.False(interrupted.Success);
            Assert.Equal("resource.persistence_interrupted", interrupted.Error!.MachineCode);
            AssertAnchorAndJournal(
                target,
                targetContract,
                initial.Revision,
                CanonicalWorldStateCodec.ComputeContentHash(initial),
                0);

            var accepted = Success(targetContract, WorldReplayContract.ReplayName, replayRequest);
            Assert.Equal(source.Current.Revision, target.Current.Revision);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(source.Current),
                CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(1, Convert.ToInt32(accepted.Data!["replayedEntries"], CultureInfo.InvariantCulture));
            Assert.Equal(1, JournalCount(targetContract));
        }

        private static void AssertImportRejected(
            PortableWorldAuthoringSession target,
            ComposedContract contract,
            IReadOnlyDictionary<string, object?> request,
            string code,
            long revision,
            string hash,
            int journalCount)
        {
            var result = contract.Dispatch(WorldPortabilityContract.ImportName, ExactV1, request);
            Assert.False(result.Success);
            Assert.Equal(code, result.Error!.MachineCode);
            AssertAnchorAndJournal(target, contract, revision, hash, journalCount);
        }

        private static void AssertAnchorAndJournal(
            PortableWorldAuthoringSession session,
            ComposedContract contract,
            long revision,
            string hash,
            int journalCount)
        {
            Assert.Equal(revision, session.Current.Revision);
            Assert.Equal(hash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(journalCount, JournalCount(contract));
        }

        private static void AssertResource(
            NeutralProjectionOutcome outcome,
            string code,
            string dimension)
        {
            Assert.False(outcome.Success);
            Assert.Equal(NeutralProjectionFailureKind.Resource, outcome.FailureKind);
            Assert.Equal(code, outcome.Error!.MachineCode);
            Assert.Equal(H0ResourceEnvelope.SchemaId, outcome.Error.Context["resourceEnvelope"]);
            Assert.Equal(dimension, outcome.Error.Context["dimension"]);
        }

        private static NeutralProjectionOutcome Invoke(
            NeutralProjectionService projection,
            string requestId,
            string capability,
            IReadOnlyDictionary<string, object?> arguments)
        {
            return projection.InvokeAsync(new NeutralProjectionRequest(
                requestId,
                capability,
                ExactV1,
                arguments)).GetAwaiter().GetResult();
        }

        private static IReadOnlyDictionary<string, object?> DeepArguments(int levels)
        {
            IReadOnlyDictionary<string, object?> current = Empty();
            for (var index = 0; index < levels; index++)
            {
                current = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["nested"] = current
                };
            }
            return current;
        }

        private static IReadOnlyList<object?> RepresentativeOperations()
        {
            var operations = new List<object?>();
            for (var index = 0; index < 72; index++)
                operations.Add(PutObject("potes.plaza.item-" + index.ToString("D2", CultureInfo.InvariantCulture)));
            for (var index = 0; index < 24; index++)
                operations.Add(PutExtension("fixture.hk09b.potes-visual-" + index, new[] { (byte)index }));
            return operations.AsReadOnly();
        }

        private static IReadOnlyDictionary<string, object?> PutObject(string id)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-object",
                ["id"] = id,
                ["typeId"] = "fixture.hk09b.potes-market-item",
                ["references"] = Array.Empty<object?>()
            };
        }

        private static IReadOnlyDictionary<string, object?> PutExtension(string owner, byte[] payload)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-extension",
                ["owner"] = owner,
                ["schemaVersion"] = 1,
                ["payloadBase64"] = Convert.ToBase64String(payload),
                ["dependencies"] = Array.Empty<object?>()
            };
        }

        private static IReadOnlyDictionary<string, object?> MutationRequest(
            WorldState state,
            string idempotencyKey,
            IReadOnlyList<object?> operations)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = idempotencyKey,
                ["expectedRevision"] = state.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(state),
                ["operations"] = operations
            };
        }

        private static IReadOnlyDictionary<string, object?> ImportRequest(
            WorldState state,
            IReadOnlyDictionary<string, object?> snapshot,
            string idempotencyKey)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = idempotencyKey,
                ["expectedRevision"] = state.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(state),
                ["snapshot"] = snapshot
            };
        }

        private static Dictionary<string, object?> CloneMap(IReadOnlyDictionary<string, object?> source)
        {
            return new Dictionary<string, object?>(source, StringComparer.Ordinal);
        }

        private static WorldState EmptyWorld(string id)
        {
            return new WorldState(new WorldId(id), 0, Array.Empty<WorldObject>());
        }

        private static ComposedContract Compose(PortableWorldAuthoringSession session)
        {
            return CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
        }

        private static CapabilityInvocationResult Success(
            ComposedContract contract,
            string capability,
            IReadOnlyDictionary<string, object?> request)
        {
            var result = contract.Dispatch(capability, ExactV1, request);
            Assert.True(result.Success, result.Error?.MachineCode + ":" + result.Error?.Message);
            return result;
        }

        private static int JournalCount(ComposedContract contract)
        {
            return Convert.ToInt32(
                Success(contract, WorldProvenanceContract.ReadName, Empty()).Data!["entryCount"],
                CultureInfo.InvariantCulture);
        }

        private static CapabilityDefinition FindDefinition(ComposedContract contract, string name)
        {
            foreach (var definition in contract.Definitions)
                if (string.Equals(name, definition.Key.Name, StringComparison.Ordinal)) return definition;
            throw new InvalidOperationException("Missing capability " + name);
        }

        private static IReadOnlyDictionary<string, object?> Map(
            IReadOnlyDictionary<string, object?> source,
            string key)
        {
            return (IReadOnlyDictionary<string, object?>)source[key]!;
        }

        private static IReadOnlyDictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }
    }
}
