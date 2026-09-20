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
    public sealed class Hk09BContentShapeProbeTests
    {
        private static readonly ContractVersionRange ExactV1 =
            ContractVersionRange.Exact(new ContractVersion(1, 0));

        [Fact]
        public void PotesHeroSliceFitsOneBoundedTransactionAndCheckpointRebase()
        {
            var initial = EmptyWorld("world.hk09b.potes-source");
            var source = new PortableWorldAuthoringSession(initial);
            using var sourceProjection = Projection(source);
            var operations = PotesHeroSliceOperations();
            var mutation = MutationRequest(initial, operations);

            Assert.Equal(96, operations.Count);
            Assert.True(PortableData.TryMeasure(
                mutation,
                H0ResourceEnvelope.MaximumPortableDepth,
                H0ResourceEnvelope.MaximumCanonicalRequestBytes,
                out var mutationMetrics,
                out var mutationIssue),
                mutationIssue?.Code + ":" + mutationIssue?.Path);
            Assert.True(mutationMetrics.Utf8JsonBytes < H0ResourceEnvelope.MaximumCanonicalRequestBytes);

            var applied = Invoke(
                sourceProjection,
                "hk09b.potes.apply",
                WorldMutationContract.ApplyName,
                mutation);
            Assert.True(applied.Success, applied.Error?.MachineCode);
            Assert.Equal(1, source.Current.Revision);
            Assert.Equal(72, source.Current.Objects.Count);
            Assert.Equal(24, source.Current.Extensions.Count);

            var summary = Invoke(
                sourceProjection,
                "hk09b.potes.summary",
                WorldInspectionContract.SummaryName,
                Empty());
            Assert.True(summary.Success, summary.Error?.MachineCode);
            Assert.Equal(72, Convert.ToInt32(summary.Result!["objectCount"], CultureInfo.InvariantCulture));

            var exported = Invoke(
                sourceProjection,
                "hk09b.potes.export",
                WorldPortabilityContract.ExportName,
                Empty());
            Assert.True(exported.Success, exported.Error?.MachineCode);
            var snapshot = exported.Result!;
            var decodedStateBytes = Convert.FromBase64String((string)snapshot["authoredStateBase64"]!).Length;
            Assert.True(decodedStateBytes < H0ResourceEnvelope.MaximumCanonicalWorldBytes);

            var targetInitial = EmptyWorld("world.hk09b.potes-target");
            var target = new PortableWorldAuthoringSession(targetInitial);
            using var targetProjection = Projection(target);
            var import = ImportRequest(targetInitial, snapshot);
            Assert.True(PortableData.TryMeasure(
                import,
                H0ResourceEnvelope.MaximumPortableDepth,
                H0ResourceEnvelope.MaximumCanonicalRequestBytes,
                out var importMetrics,
                out var importIssue),
                importIssue?.Code + ":" + importIssue?.Path);
            Assert.True(importMetrics.Utf8JsonBytes < H0ResourceEnvelope.MaximumCanonicalRequestBytes);

            var imported = Invoke(
                targetProjection,
                "hk09b.potes.import",
                WorldPortabilityContract.ImportName,
                import);
            Assert.True(imported.Success, imported.Error?.MachineCode);
            Assert.Equal(source.Current.Revision, target.Current.Revision);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(source.Current),
                CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(0, JournalCount(target));
            Assert.Equal("new-local-lineage-empty",
                Map(imported.Result!, "rebaseEvidence")["mutationJournalDisposition"]);
        }

        private static IReadOnlyList<object?> PotesHeroSliceOperations()
        {
            var operations = new List<object?>();
            var districts = new[] { "plaza", "market", "bar-terrace", "workshop" };
            for (var district = 0; district < districts.Length; district++)
            {
                for (var index = 0; index < 18; index++)
                {
                    operations.Add(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["kind"] = "put-object",
                        ["id"] = "potes." + districts[district] + ".item-" + index.ToString("D2", CultureInfo.InvariantCulture),
                        ["typeId"] = "fixture.hk09b." + districts[district] + "-mesh",
                        ["references"] = Array.Empty<object?>()
                    });
                }
            }

            for (var index = 0; index < 24; index++)
            {
                operations.Add(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-extension",
                    ["owner"] = "fixture.hk09b.potes-visual-" + index.ToString("D2", CultureInfo.InvariantCulture),
                    ["schemaVersion"] = 1,
                    ["payloadBase64"] = Convert.ToBase64String(new byte[] { 0x09, (byte)index, 0x0b }),
                    ["dependencies"] = Array.Empty<object?>()
                });
            }

            return operations.AsReadOnly();
        }

        private static IReadOnlyDictionary<string, object?> MutationRequest(
            WorldState state,
            IReadOnlyList<object?> operations)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.hk09b.potes-hero-slice",
                ["expectedRevision"] = state.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(state),
                ["operations"] = operations
            };
        }

        private static IReadOnlyDictionary<string, object?> ImportRequest(
            WorldState target,
            IReadOnlyDictionary<string, object?> snapshot)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "request.hk09b.potes-checkpoint",
                ["expectedRevision"] = target.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(target),
                ["snapshot"] = snapshot
            };
        }

        private static NeutralProjectionService Projection(PortableWorldAuthoringSession session)
        {
            return new NeutralProjectionService(
                CanonicalWorldContract.Compose(new WorldInspectionService(session), session));
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

        private static int JournalCount(PortableWorldAuthoringSession session)
        {
            var result = session.ReadJournal(Empty());
            Assert.True(result.Success, result.Error?.MachineCode);
            return Convert.ToInt32(result.Data!["entryCount"], CultureInfo.InvariantCulture);
        }

        private static IReadOnlyDictionary<string, object?> Map(
            IReadOnlyDictionary<string, object?> source,
            string key)
        {
            return (IReadOnlyDictionary<string, object?>)source[key]!;
        }

        private static WorldState EmptyWorld(string id)
        {
            return new WorldState(new WorldId(id), 0, Array.Empty<WorldObject>());
        }

        private static IReadOnlyDictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }
    }
}
