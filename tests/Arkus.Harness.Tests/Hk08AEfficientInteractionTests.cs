using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08AEfficientInteractionTests
    {
        private static readonly ContractVersionRange ExactV1 =
            ContractVersionRange.Exact(new ContractVersion(1, 0));

        [Fact]
        public void RepresentativeNinetySixOperationIntentRemainsOneAtomicTransactionAndOneJournalEntry()
        {
            var initial = EmptyWorld("world.hk08a.representative");
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            var operations = RepresentativeMicroBlockOperations();

            // Causal guard against the provisional pre-HK08A ceiling: this exact product-shaped
            // intent would have been forced across the old 64-operation boundary.
            Assert.Equal(96, operations.Count);
            Assert.True(operations.Count > 64);
            Assert.Equal(operations.Count, WorldMutationContract.MaximumOperations);

            var request = MutationRequest(initial, "request.hk08a.representative", operations);
            var dryRun = Success(contract, WorldMutationContract.DryRunName, request);
            Assert.False((bool)dryRun.Data!["persisted"]!);
            Assert.Equal(initial.Revision, session.Current.Revision);

            var applied = Success(contract, WorldMutationContract.ApplyName, request);
            Assert.True((bool)applied.Data!["persisted"]!);
            Assert.Equal(initial.Revision + 1, session.Current.Revision);
            Assert.Equal(96, session.Current.Objects.Count);

            var journal = Success(contract, WorldProvenanceContract.ReadName, Empty()).Data!;
            Assert.Equal(WorldProvenanceContract.JournalSchemaId, journal["schemaId"]);
            Assert.Equal(1, Convert.ToInt32(journal["entryCount"]));
            var entry = Assert.Single(List(journal, "entries"));
            var normalized = Map((IReadOnlyDictionary<string, object?>)entry!, "request");
            Assert.Equal(96, List(normalized, "operations").Count);

            var applyDefinition = contract.Definitions.Single(value =>
                value.Key.Name == WorldMutationContract.ApplyName);
            Assert.NotNull(applyDefinition.Batching);
            Assert.Equal(BatchingClass.Supported, applyDefinition.Batching!.Class);
            Assert.Equal(96, applyDefinition.Batching.MaximumItems);
        }

        [Fact]
        public void InvalidLaterOperationCannotPublishEarlierBatchEditsOrProvenance()
        {
            var initial = EmptyWorld("world.hk08a.atomic-negative");
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            var operations = new object?[]
            {
                PutObject("block.valid", "fixture.hk08a.item"),
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "remove-object",
                    ["id"] = "block.missing"
                }
            };

            var rejected = contract.Dispatch(
                WorldMutationContract.ApplyName,
                ExactV1,
                MutationRequest(initial, "request.hk08a.atomic-negative", operations));
            Assert.False(rejected.Success);
            Assert.Equal(initial.Revision, session.Current.Revision);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(initial),
                CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Empty(session.Current.Objects);

            var journal = Success(contract, WorldProvenanceContract.ReadName, Empty()).Data!;
            Assert.Equal(0, Convert.ToInt32(journal["entryCount"]));
            Assert.Empty(List(journal, "entries"));
        }

        [Fact]
        public void JournalPagesReconstructOneAnchoredSequenceWithoutDuplicateOrOmissionAndFailClosedWhenStale()
        {
            var initial = EmptyWorld("world.hk08a.journal-pages");
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);

            for (var index = 0; index < 55; index++)
            {
                ApplyOneObject(contract, session, index);
            }

            var anchor = session.Current;
            var anchorHash = CanonicalWorldStateCodec.ComputeContentHash(anchor);
            var first = Success(
                contract,
                WorldProvenanceContract.ReadName,
                JournalPageRequest(anchor, 20)).Data!;
            Assert.Equal(WorldProvenanceContract.JournalPageSchemaId, first["schemaId"]);
            Assert.Equal(WorldProvenanceContract.JournalSchemaId, first["journalSchemaId"]);
            Assert.Equal(55, Convert.ToInt32(first["entryCount"]));
            Assert.Equal(0, Convert.ToInt32(first["pageOffset"]));
            Assert.Equal(20, List(first, "entries").Count);
            var firstCursor = Assert.IsType<string>(first["nextCursor"]);

            var second = Success(
                contract,
                WorldProvenanceContract.ReadName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = 20,
                    ["cursor"] = firstCursor
                }).Data!;
            Assert.Equal(20, Convert.ToInt32(second["pageOffset"]));
            Assert.Equal(20, List(second, "entries").Count);
            var secondCursor = Assert.IsType<string>(second["nextCursor"]);

            var third = Success(
                contract,
                WorldProvenanceContract.ReadName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = 20,
                    ["cursor"] = secondCursor
                }).Data!;
            Assert.Equal(40, Convert.ToInt32(third["pageOffset"]));
            Assert.Equal(15, List(third, "entries").Count);
            Assert.False(third.ContainsKey("nextCursor"));

            var reconstructed = List(first, "entries")
                .Concat(List(second, "entries"))
                .Concat(List(third, "entries"))
                .Cast<IReadOnlyDictionary<string, object?>>()
                .ToArray();
            Assert.Equal(55, reconstructed.Length);
            Assert.Equal(55, reconstructed.Select(entry => (string)entry["entryId"]!).Distinct(StringComparer.Ordinal).Count());
            for (var index = 0; index < reconstructed.Length; index++)
            {
                Assert.Equal(index + 1L, Convert.ToInt64(reconstructed[index]["sequence"]));
            }

            // A sufficiently large bounded page that covers the complete journal preserves the exact
            // HK06A artifact shape, so accepted replay/audit consumers do not need a second meaning.
            var complete = Success(
                contract,
                WorldProvenanceContract.ReadName,
                JournalPageRequest(anchor, WorldProvenanceContract.MaximumPageSize)).Data!;
            Assert.Equal(WorldProvenanceContract.JournalSchemaId, complete["schemaId"]);
            Assert.False(complete.ContainsKey("pageOffset"));
            Assert.False(complete.ContainsKey("nextCursor"));
            Assert.Equal(55, List(complete, "entries").Count);
            Assert.Empty(WorldProvenanceContract.JournalResultSchema().ValidateValue(complete));

            // Change the authored anchor after capturing the cursor. Continuing the old page stream
            // without an explicit anchor must still fail closed from cursor-bound lineage metadata.
            ApplyOneObject(contract, session, 55);
            var staleCursor = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["limit"] = 20,
                    ["cursor"] = firstCursor
                });
            Assert.False(staleCursor.Success);
            Assert.Equal("world.provenance.stale_cursor", staleCursor.Error!.MachineCode);

            var staleAnchor = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["revision"] = anchor.Revision,
                    ["hash"] = anchorHash,
                    ["limit"] = 20
                });
            Assert.False(staleAnchor.Success);
            Assert.Equal("world.provenance.stale_anchor", staleAnchor.Error!.MachineCode);
        }

        [Fact]
        public void DiscoveryAndCompactProjectionExposeEconomicalPublicChoicesWithoutLosingRequiredFields()
        {
            var initial = EmptyWorld("world.hk08a.discovery");
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            Success(
                contract,
                WorldMutationContract.ApplyName,
                MutationRequest(
                    initial,
                    "request.hk08a.discovery.seed",
                    new object?[] { PutObject("block.compact", "fixture.hk08a.item") }));

            var discovery = Success(contract, "system.describe", Empty()).Data!;
            var summary = FindCapability(discovery, WorldInspectionContract.SummaryName);
            var objectQuery = FindCapability(discovery, WorldInspectionContract.ObjectQueryName);
            var apply = FindCapability(discovery, WorldMutationContract.ApplyName);

            Assert.Equal("readonly", summary["sideEffect"]);
            Assert.Equal("readonly", objectQuery["sideEffect"]);
            Assert.Equal("canonicalmutation", apply["sideEffect"]);
            Assert.True(CostWeight(summary) < CostWeight(objectQuery));
            Assert.True(CostWeight(summary) < CostWeight(apply));
            Assert.Equal(96, Convert.ToInt32(Map(apply, "batching")["maximumItems"]));

            var current = session.Current;
            var compact = Success(
                contract,
                WorldInspectionContract.ObjectGetName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["revision"] = current.Revision,
                    ["hash"] = CanonicalWorldStateCodec.ComputeContentHash(current),
                    ["id"] = "block.compact",
                    ["fields"] = Array.Empty<object?>()
                }).Data!;
            var compactObject = Map(compact, "object");
            Assert.Equal("block.compact", compactObject["id"]);
            Assert.False(compactObject.ContainsKey("typeId"));
            Assert.False(compactObject.ContainsKey("containerId"));
            Assert.True(compact.ContainsKey("world"));

            // Causal required-field negative control: compactness may omit requested optional fields,
            // but it may not erase the required world anchor from the canonical success shape.
            var malformed = new Dictionary<string, object?>(compact, StringComparer.Ordinal);
            malformed.Remove("world");
            var definition = contract.Definitions.Single(value =>
                value.Key.Name == WorldInspectionContract.ObjectGetName);
            Assert.NotNull(definition.SuccessSchema);
            Assert.NotEmpty(definition.SuccessSchema!.ValidateValue(malformed));
        }

        private static ComposedContract Compose(TransactionalWorldAuthoringSession session)
        {
            return CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
        }

        private static WorldState EmptyWorld(string id)
        {
            return new WorldState(new WorldId(id), 0, Array.Empty<WorldObject>());
        }

        private static IReadOnlyList<object?> RepresentativeMicroBlockOperations()
        {
            // Product-shape probe only: 72 structural/visual authoring objects plus 24 opaque scoped
            // extension records. Names are fixture vocabulary, not new canonical gameplay semantics.
            var operations = new List<object?>();
            for (var index = 0; index < 72; index++)
            {
                operations.Add(PutObject(
                    "potes.micro-block.item-" + index.ToString("D2", System.Globalization.CultureInfo.InvariantCulture),
                    "fixture.hk08a.micro-block-item"));
            }

            for (var index = 0; index < 24; index++)
            {
                var subjectId = "potes.micro-block.item-" + index.ToString("D2", System.Globalization.CultureInfo.InvariantCulture);
                operations.Add(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "put-extension",
                    ["owner"] = "fixture.hk08a.visual-metadata",
                    ["schemaVersion"] = 1,
                    ["subjectId"] = subjectId,
                    ["payloadBase64"] = Convert.ToBase64String(new byte[] { (byte)index }),
                    ["dependencies"] = Array.Empty<object?>()
                });
            }

            return operations.AsReadOnly();
        }

        private static IReadOnlyDictionary<string, object?> PutObject(string id, string typeId)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-object",
                ["id"] = id,
                ["typeId"] = typeId,
                ["references"] = Array.Empty<object?>()
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

        private static IReadOnlyDictionary<string, object?> JournalPageRequest(WorldState state, int limit)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = state.Revision,
                ["hash"] = CanonicalWorldStateCodec.ComputeContentHash(state),
                ["limit"] = limit
            };
        }

        private static void ApplyOneObject(
            ComposedContract contract,
            TransactionalWorldAuthoringSession session,
            int index)
        {
            var current = session.Current;
            Success(
                contract,
                WorldMutationContract.ApplyName,
                MutationRequest(
                    current,
                    "request.hk08a.journal." + index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    new object?[]
                    {
                        PutObject(
                            "journal.item." + index.ToString("D2", System.Globalization.CultureInfo.InvariantCulture),
                            "fixture.hk08a.journal-item")
                    }));
        }

        private static CapabilityInvocationResult Success(
            ComposedContract contract,
            string capability,
            IReadOnlyDictionary<string, object?> request)
        {
            var result = contract.Dispatch(capability, ExactV1, request);
            Assert.True(result.Success, result.Error == null ? "Expected success." : result.Error.MachineCode + ": " + result.Error.Message);
            Assert.NotNull(result.Data);
            return result;
        }

        private static IReadOnlyDictionary<string, object?> FindCapability(
            IReadOnlyDictionary<string, object?> discovery,
            string name)
        {
            foreach (var value in List(discovery, "capabilities"))
            {
                var capability = (IReadOnlyDictionary<string, object?>)value!;
                if (string.Equals((string)capability["name"]!, name, StringComparison.Ordinal)) return capability;
            }

            throw new Xunit.Sdk.XunitException("Discovery omitted " + name + ".");
        }

        private static int CostWeight(IReadOnlyDictionary<string, object?> capability)
        {
            return Convert.ToInt32(Map(capability, "cost")["relativeWeight"]);
        }

        private static IReadOnlyDictionary<string, object?> Map(
            IReadOnlyDictionary<string, object?> data,
            string key)
        {
            return (IReadOnlyDictionary<string, object?>)data[key]!;
        }

        private static IReadOnlyList<object?> List(
            IReadOnlyDictionary<string, object?> data,
            string key)
        {
            return (IReadOnlyList<object?>)data[key]!;
        }

        private static IReadOnlyDictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }
    }
}
