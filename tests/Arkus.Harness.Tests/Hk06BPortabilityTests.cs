using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06BPortabilityTests
    {
        [Fact]
        public void SemanticDiffIsFieldLevelDeterministicAndIgnoresRepresentationOrder()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new PortableWorldAuthoringSession(initial);
            var contract = Compose(session);
            var before = Snapshot(contract);

            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk06b.semantic-diff",
                Hk04TransactionalMutationTests.PutObject(
                    "node.child",
                    "fixture.changed",
                    "node.root",
                    Hk04TransactionalMutationTests.Reference("fixture.root", "node.root")),
                Hk06AProvenanceJournalTests.PutExtension(
                    "future.alpha",
                    2,
                    new byte[] { 0x44, 0x55, 0x66 }));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, request);
            var after = Snapshot(contract);

            var first = Compare(contract, before, after);
            var second = Compare(contract, before, after);
            Assert.False((bool)first["sameAuthorableState"]!);
            Assert.Equal(Resources(first), Resources(second));
            Assert.Equal(new[]
            {
                "world.extension:future.alpha@2@global",
                "world.object:node.child"
            }, Resources(first));

            var objectChange = Change(first, "world.object:node.child");
            Assert.Equal("update", objectChange["action"]);
            Assert.Contains("typeId", Hk04TransactionalMutationTests.Strings(objectChange, "fields"));
            Assert.Contains("references", Hk04TransactionalMutationTests.Strings(objectChange, "fields"));
            Assert.Contains("fixture.peer->node.peer", Hk04TransactionalMutationTests.Strings(objectChange, "relationsRemoved"));

            var extensionChange = Change(first, "world.extension:future.alpha@2@global");
            Assert.Contains("payload", Hk04TransactionalMutationTests.Strings(extensionChange, "fields"));

            var reordered = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld(reverseInputOrder: true));
            var reorderedContract = Compose(reordered);
            var reorderedSnapshot = Snapshot(reorderedContract);
            var representationOnly = Compare(contract, before, reorderedSnapshot);
            Assert.True((bool)representationOnly["sameAuthorableState"]!);
            Assert.Empty(Hk04TransactionalMutationTests.List(representationOnly, "changes"));
            Assert.Equal(
                Hk04TransactionalMutationTests.Map(before, "anchor")["hash"],
                Hk04TransactionalMutationTests.Map(reorderedSnapshot, "anchor")["hash"]);
        }

        [Fact]
        public void SnapshotRoundTripPreservesCanonicalHashAndStartsTruthfulNewLocalLineage()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var source = new PortableWorldAuthoringSession(initial);
            var sourceContract = Compose(source);
            Hk04TransactionalMutationTests.Success(
                sourceContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    initial,
                    "request.hk06b.source-history",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.portable")));
            Assert.Equal(1, JournalCount(sourceContract));

            var snapshot = Snapshot(sourceContract);
            Assert.Equal(WorldPortabilityContract.SnapshotSchemaId, snapshot["schemaId"]);
            Assert.False((bool)snapshot["provenanceIncluded"]!);
            Assert.False((bool)snapshot["runtimeObservationsIncluded"]!);

            var targetInitial = new WorldState(new WorldId("world.target"), 3, Array.Empty<WorldObject>());
            var target = new PortableWorldAuthoringSession(targetInitial);
            var targetContract = Compose(target);
            var importRequest = ImportRequest(target.Current, snapshot, "request.hk06b.roundtrip");
            var import = Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldPortabilityContract.ImportName,
                importRequest);

            Assert.Equal("new-local-lineage", import.Data!["lineageDisposition"]);
            Assert.False((bool)import.Data["replayed"]!);
            Assert.Equal(0, import.Data["retainedJournalEntries"]);
            Assert.Equal(0, import.Data["importedJournalEntries"]);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(source.Current),
                CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(source.Current.Revision, target.Current.Revision);
            Assert.Equal(0, JournalCount(targetContract));

            var replay = Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldPortabilityContract.ImportName,
                importRequest);
            Assert.True((bool)replay.Data!["replayed"]!);
            Assert.Equal(import.Data["current"], replay.Data["current"]);
            Assert.Equal(0, JournalCount(targetContract));

            var conflictRequest = new Dictionary<string, object?>(importRequest, StringComparer.Ordinal)
            {
                ["expectedHash"] = new string('0', 64)
            };
            var conflict = targetContract.Dispatch(
                WorldPortabilityContract.ImportName,
                Hk04TransactionalMutationTests.ExactVersion(),
                conflictRequest);
            Assert.False(conflict.Success);
            Assert.Equal("world.snapshot.idempotency_conflict", conflict.Error!.MachineCode);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(source.Current),
                CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(0, JournalCount(targetContract));

            var journal = Hk06AProvenanceJournalTests.ReadJournal(targetContract).Data!;
            Assert.Equal(
                Hk04TransactionalMutationTests.Map(snapshot, "anchor")["hash"],
                Hk04TransactionalMutationTests.Map(journal, "base")["hash"]);
            Assert.Equal(
                Hk04TransactionalMutationTests.Map(snapshot, "anchor")["hash"],
                Hk04TransactionalMutationTests.Map(journal, "current")["hash"]);

            var imported = target.Current;
            Hk04TransactionalMutationTests.Success(
                targetContract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    imported,
                    "request.hk06b.after-import",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.after-import")));
            Assert.Equal(1, JournalCount(targetContract));
        }

        [Fact]
        public void InvalidUnsupportedOrBoundaryViolatingSnapshotCannotPartiallyReplaceState()
        {
            var source = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var sourceContract = Compose(source);
            var snapshot = Snapshot(sourceContract);

            AssertRejectedWithoutReplacement(
                MutateSnapshot(snapshot, data => data["snapshotVersion"] = 2L),
                "world.snapshot.unsupported_version");

            AssertRejectedWithoutReplacement(
                MutateSnapshot(snapshot, data => data["runtimeObservationsIncluded"] = true),
                "world.snapshot.boundary_violation");

            AssertRejectedWithoutReplacement(
                MutateSnapshot(snapshot, data =>
                {
                    var anchor = new Dictionary<string, object?>(
                        Hk04TransactionalMutationTests.Map(data, "anchor"),
                        StringComparer.Ordinal);
                    anchor["hash"] = new string('0', 64);
                    data["anchor"] = anchor;
                }),
                "world.snapshot.anchor_mismatch");

            AssertRejectedWithoutReplacement(
                MutateSnapshot(snapshot, data =>
                {
                    var payload = Convert.FromBase64String((string)data["authoredStateBase64"]!);
                    payload[payload.Length - 1] ^= 0x01;
                    data["authoredStateBase64"] = Convert.ToBase64String(payload);
                }),
                "world.snapshot.invalid_state");
        }

        [Fact]
        public void IndependentSemanticOracleCoversEveryCurrentAuthorableResourceFieldClass()
        {
            var baseline = Hk02TestFixtures.MicroWorld();
            var cases = new[]
            {
                new SemanticCase(ChangeObject(baseline, "node.peer", value =>
                    new WorldObject(value.Id, new WorldTypeId("fixture.changed"), value.ContainerId, value.References)),
                    "world.object:node.peer", "typeId"),
                new SemanticCase(ChangeObject(baseline, "node.child", value =>
                    new WorldObject(value.Id, value.TypeId, null, value.References)),
                    "world.object:node.child", "containerId"),
                new SemanticCase(ChangeObject(baseline, "node.child", value =>
                    new WorldObject(
                        value.Id,
                        value.TypeId,
                        value.ContainerId,
                        new[] { new WorldReference(new WorldReferenceKind("fixture.root"), new WorldObjectId("node.root")) })),
                    "world.object:node.child", "references"),
                new SemanticCase(ChangeExtension(baseline, "future.alpha@2@global", value =>
                    new WorldExtensionData(value.Owner, value.SchemaVersion, new byte[] { 0x7a }, value.SubjectId, value.Dependencies)),
                    "world.extension:future.alpha@2@global", "payload"),
                new SemanticCase(ChangeExtension(baseline, "future.alpha@2@global", value =>
                    new WorldExtensionData(
                        value.Owner,
                        value.SchemaVersion,
                        value.GetPayloadCopy(),
                        value.SubjectId,
                        new[] { new WorldReference(new WorldReferenceKind("fixture.dep"), new WorldObjectId("node.peer")) })),
                    "world.extension:future.alpha@2@global", "dependencies"),
                new SemanticCase(AddObject(baseline), "world.object:node.added", "existence"),
                new SemanticCase(RemoveChild(baseline), "world.object:node.child", "existence")
            };

            for (var index = 0; index < cases.Length; index++)
            {
                var left = ExportDirect(baseline);
                var right = ExportDirect(cases[index].State);
                var contract = Compose(new PortableWorldAuthoringSession(baseline));
                var diff = Compare(contract, left, right);
                var change = Change(diff, cases[index].Resource);
                Assert.Contains(cases[index].Field, Hk04TransactionalMutationTests.Strings(change, "fields"));
                Assert.Contains(cases[index].Resource, IndependentChangedResources(baseline, cases[index].State));
            }
        }

        [Fact]
        public void PublicDiscoveryPublishesVersionedDiffSnapshotAndImportSchemas()
        {
            var session = new PortableWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var contract = Compose(session);
            foreach (var name in new[]
            {
                WorldPortabilityContract.CompareName,
                WorldPortabilityContract.ExportName,
                WorldPortabilityContract.ImportName
            })
            {
                var definition = FindDefinition(contract, name);
                Assert.Equal("1.0", definition.Key.Version.ToString());
                Assert.NotNull(definition.RequestSchema);
                Assert.NotNull(definition.SuccessSchema);
                Assert.NotNull(definition.ErrorSchema);
            }

            var snapshot = Snapshot(contract);
            Assert.Empty(FindDefinition(contract, WorldPortabilityContract.ExportName).SuccessSchema!.ValidateValue(snapshot));
            var diff = Compare(contract, snapshot, snapshot);
            Assert.Empty(FindDefinition(contract, WorldPortabilityContract.CompareName).SuccessSchema!.ValidateValue(diff));
        }

        [Fact]
        public void PotesContentShapeRoundTripsAuthoredStateWhileRuntimeSurrogateRemainsTransient()
        {
            var potes = PotesWorld();
            var session = new PortableWorldAuthoringSession(potes);
            var contract = Compose(session);
            var before = Snapshot(contract);
            var runtimeStep = 0;
            runtimeStep++;
            runtimeStep++;
            Assert.Equal(2, runtimeStep);
            var afterRuntimeOnly = Snapshot(contract);
            Assert.Equal(before["authoredStateBase64"], afterRuntimeOnly["authoredStateBase64"]);

            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    potes,
                    "request.hk06b.potes-shop",
                    Hk04TransactionalMutationTests.PutObject(
                        "building.shop",
                        "fixture.shop.refreshed",
                        "place.plaza")));
            var after = Snapshot(contract);
            var diff = Compare(contract, before, after);
            Assert.Equal(new[] { "world.object:building.shop" }, Resources(diff));

            var clean = new PortableWorldAuthoringSession(
                new WorldState(new WorldId("world.clean"), 0, Array.Empty<WorldObject>()));
            var cleanContract = Compose(clean);
            Hk04TransactionalMutationTests.Success(
                cleanContract,
                WorldPortabilityContract.ImportName,
                ImportRequest(clean.Current, after, "request.hk06b.potes-import"));
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(session.Current),
                CanonicalWorldStateCodec.ComputeContentHash(clean.Current));
            Assert.Equal(0, JournalCount(cleanContract));
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

        private static IReadOnlyDictionary<string, object?> ExportDirect(WorldState state)
        {
            return new PortableWorldAuthoringSession(state).ExportSnapshot(Hk01TestFixtures.EmptyRequest()).Data!;
        }

        private static IReadOnlyDictionary<string, object?> Compare(
            ComposedContract contract,
            IReadOnlyDictionary<string, object?> before,
            IReadOnlyDictionary<string, object?> after)
        {
            return Hk04TransactionalMutationTests.Success(
                contract,
                WorldPortabilityContract.CompareName,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["base"] = before,
                    ["target"] = after
                }).Data!;
        }

        private static IReadOnlyDictionary<string, object?> ImportRequest(
            WorldState current,
            IReadOnlyDictionary<string, object?> snapshot,
            string idempotencyKey = "request.hk06b.snapshot-import")
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = idempotencyKey,
                ["expectedRevision"] = current.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(current),
                ["snapshot"] = snapshot
            };
        }

        private static int JournalCount(ComposedContract contract)
        {
            return Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"]);
        }

        private static IReadOnlyDictionary<string, object?> MutateSnapshot(
            IReadOnlyDictionary<string, object?> source,
            Action<Dictionary<string, object?>> mutation)
        {
            var clone = new Dictionary<string, object?>(source, StringComparer.Ordinal);
            mutation(clone);
            return clone;
        }

        private static void AssertRejectedWithoutReplacement(
            IReadOnlyDictionary<string, object?> snapshot,
            string expectedCode)
        {
            var targetInitial = Hk02TestFixtures.MicroWorld();
            var target = new PortableWorldAuthoringSession(targetInitial);
            var contract = Compose(target);
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(target.Current);
            var beforeRevision = target.Current.Revision;
            var result = contract.Dispatch(
                WorldPortabilityContract.ImportName,
                Hk04TransactionalMutationTests.ExactVersion(),
                ImportRequest(target.Current, snapshot));
            Assert.False(result.Success);
            Assert.Equal(expectedCode, result.Error!.MachineCode);
            Assert.Equal(beforeRevision, target.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(target.Current));
            Assert.Equal(0, JournalCount(contract));
        }

        private static IReadOnlyList<string> Resources(IReadOnlyDictionary<string, object?> diff)
        {
            var result = new List<string>();
            foreach (var value in Hk04TransactionalMutationTests.List(diff, "changes"))
                result.Add((string)((IReadOnlyDictionary<string, object?>)value!)["resource"]!);
            return result.AsReadOnly();
        }

        private static IReadOnlyDictionary<string, object?> Change(
            IReadOnlyDictionary<string, object?> diff,
            string resource)
        {
            foreach (var value in Hk04TransactionalMutationTests.List(diff, "changes"))
            {
                var change = (IReadOnlyDictionary<string, object?>)value!;
                if (string.Equals((string)change["resource"]!, resource, StringComparison.Ordinal)) return change;
            }
            throw new InvalidOperationException("Missing semantic diff resource: " + resource);
        }

        private static CapabilityDefinition FindDefinition(ComposedContract contract, string name)
        {
            foreach (var definition in contract.Definitions)
                if (string.Equals(definition.Key.Name, name, StringComparison.Ordinal)) return definition;
            throw new InvalidOperationException("Missing capability definition: " + name);
        }

        private static WorldState ChangeObject(
            WorldState source,
            string id,
            Func<WorldObject, WorldObject> mutation)
        {
            var objects = new List<WorldObject>();
            foreach (var value in source.Objects)
                objects.Add(value.Id.Value == id ? mutation(value) : value);
            return new WorldState(source.Id, source.Revision, objects, source.Extensions, source.SchemaVersion);
        }

        private static WorldState ChangeExtension(
            WorldState source,
            string key,
            Func<WorldExtensionData, WorldExtensionData> mutation)
        {
            var extensions = new List<WorldExtensionData>();
            foreach (var value in source.Extensions)
                extensions.Add(value.Identity.ResourceKey == key ? mutation(value) : value);
            return new WorldState(source.Id, source.Revision, source.Objects, extensions, source.SchemaVersion);
        }

        private static WorldState AddObject(WorldState source)
        {
            var objects = new List<WorldObject>(source.Objects)
            {
                new WorldObject(new WorldObjectId("node.added"), new WorldTypeId("fixture.item"))
            };
            return new WorldState(source.Id, source.Revision, objects, source.Extensions, source.SchemaVersion);
        }

        private static WorldState RemoveChild(WorldState source)
        {
            var objects = new List<WorldObject>();
            foreach (var value in source.Objects)
                if (value.Id.Value != "node.child") objects.Add(value);
            return new WorldState(source.Id, source.Revision, objects, source.Extensions, source.SchemaVersion);
        }

        private static IReadOnlyList<string> IndependentChangedResources(WorldState before, WorldState after)
        {
            var result = new SortedSet<string>(StringComparer.Ordinal);
            var beforeObjects = ObjectProjection(before);
            var afterObjects = ObjectProjection(after);
            var keys = new SortedSet<string>(beforeObjects.Keys, StringComparer.Ordinal);
            keys.UnionWith(afterObjects.Keys);
            foreach (var key in keys)
            {
                beforeObjects.TryGetValue(key, out var left);
                afterObjects.TryGetValue(key, out var right);
                if (!string.Equals(left, right, StringComparison.Ordinal)) result.Add("world.object:" + key);
            }

            var beforeExtensions = ExtensionProjection(before);
            var afterExtensions = ExtensionProjection(after);
            keys = new SortedSet<string>(beforeExtensions.Keys, StringComparer.Ordinal);
            keys.UnionWith(afterExtensions.Keys);
            foreach (var key in keys)
            {
                beforeExtensions.TryGetValue(key, out var left);
                afterExtensions.TryGetValue(key, out var right);
                if (!string.Equals(left, right, StringComparison.Ordinal)) result.Add("world.extension:" + key);
            }
            return new List<string>(result).AsReadOnly();
        }

        private static Dictionary<string, string> ObjectProjection(WorldState state)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var value in state.Objects)
            {
                var references = new List<string>();
                foreach (var reference in value.References)
                    references.Add(reference.Kind.Value + "->" + reference.TargetId.Value);
                references.Sort(StringComparer.Ordinal);
                result.Add(
                    value.Id.Value,
                    value.TypeId.Value + "|" + (value.ContainerId.HasValue ? value.ContainerId.Value.Value : "-") + "|" +
                    string.Join(",", references));
            }
            return result;
        }

        private static Dictionary<string, string> ExtensionProjection(WorldState state)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var value in state.Extensions)
            {
                var dependencies = new List<string>();
                foreach (var dependency in value.Dependencies)
                    dependencies.Add(dependency.Kind.Value + "->" + dependency.TargetId.Value);
                dependencies.Sort(StringComparer.Ordinal);
                result.Add(
                    value.Identity.ResourceKey,
                    Convert.ToBase64String(value.GetPayloadCopy()) + "|" + string.Join(",", dependencies));
            }
            return result;
        }

        private static WorldState PotesWorld()
        {
            var plaza = new WorldObject(new WorldObjectId("place.plaza"), new WorldTypeId("fixture.place"));
            var bar = new WorldObject(
                new WorldObjectId("building.bar"),
                new WorldTypeId("fixture.bar"),
                new WorldObjectId("place.plaza"));
            var shop = new WorldObject(
                new WorldObjectId("building.shop"),
                new WorldTypeId("fixture.shop"),
                new WorldObjectId("place.plaza"));
            var npc = new WorldObject(
                new WorldObjectId("npc.ana"),
                new WorldTypeId("fixture.npc"),
                new WorldObjectId("place.plaza"),
                new[]
                {
                    new WorldReference(new WorldReferenceKind("works.at"), new WorldObjectId("building.bar"))
                });
            return new WorldState(
                new WorldId("world.potes"),
                7,
                new[] { shop, npc, plaza, bar },
                new[] { new WorldExtensionData("future.social", 1, new byte[] { 0x10, 0x20 }) });
        }

        private sealed class SemanticCase
        {
            public SemanticCase(WorldState state, string resource, string field)
            {
                State = state;
                Resource = resource;
                Field = field;
            }

            public WorldState State { get; }
            public string Resource { get; }
            public string Field { get; }
        }
    }
}
