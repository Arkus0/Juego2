using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk05ValidationDiagnosticsTests
    {
        [Fact]
        public void ValidatorInventoryExactlyClassifiesTheIndependentInvariantUniverse()
        {
            Assert.Empty(WorldValidationEngine.FindInventoryIssues());
            Assert.Equal(WorldInvariantCatalog.All.Count, WorldValidationEngine.ValidatorInventory.Count);

            var catalog = new HashSet<string>(StringComparer.Ordinal);
            foreach (var invariant in WorldInvariantCatalog.All) Assert.True(catalog.Add(invariant.InvariantId));

            var registered = new HashSet<string>(StringComparer.Ordinal);
            foreach (var validator in WorldValidationEngine.ValidatorInventory)
            {
                Assert.True(registered.Add(validator.InvariantId));
                Assert.False(string.IsNullOrWhiteSpace(validator.MachineCode));
                Assert.Contains(validator.Category, new[] { "structural", "identity", "reference" });
            }

            Assert.True(catalog.SetEquals(registered));

            var omissionMutant = new List<WorldValidatorDescriptor>();
            for (var index = 1; index < WorldValidationEngine.ValidatorInventory.Count; index++)
                omissionMutant.Add(WorldValidationEngine.ValidatorInventory[index]);
            var issues = WorldValidationEngine.FindInventoryIssues(omissionMutant);
            Assert.Contains("missing-validator:" + WorldValidationEngine.ValidatorInventory[0].InvariantId, issues);
        }

        [Fact]
        public void EveryOwnedInvariantHasAnEffectiveDiagnosticFixture()
        {
            var observed = new HashSet<string>(StringComparer.Ordinal);
            foreach (var candidate in InvariantFixtures())
            {
                var result = WorldValidationEngine.ValidateCandidate(candidate);
                Assert.False(result.Valid);
                foreach (var diagnostic in result.Diagnostics) observed.Add(diagnostic.InvariantId);
            }

            var expected = new HashSet<string>(StringComparer.Ordinal);
            foreach (var invariant in WorldInvariantCatalog.All) expected.Add(invariant.InvariantId);
            Assert.True(expected.SetEquals(observed));
        }

        [Fact]
        public void MultipleViolationsAreDeterministicAndCarryUnambiguousRepairContext()
        {
            var first = WorldValidationEngine.ValidateCandidate(InvalidTownCandidate(false));
            var reordered = WorldValidationEngine.ValidateCandidate(InvalidTownCandidate(true));

            Assert.False(first.Valid);
            Assert.True(first.Diagnostics.Count >= 6);
            Assert.Equal(Signatures(first.Diagnostics), Signatures(reordered.Diagnostics));
            Assert.Equal(first.Diagnostics.Count, reordered.Diagnostics.Count);

            foreach (var diagnostic in first.Diagnostics)
            {
                Assert.Equal(WorldDiagnosticSeverity.Error, diagnostic.Severity);
                Assert.StartsWith("world.", diagnostic.MachineCode);
                Assert.StartsWith("arkus.world.", diagnostic.InvariantId);
                Assert.StartsWith("world", diagnostic.Resource);
                Assert.StartsWith("$/", diagnostic.Path);
                Assert.False(string.IsNullOrWhiteSpace(diagnostic.Message));
                Assert.NotEmpty(diagnostic.RemediationContext);
            }
        }

        [Fact]
        public void CurrentAndProposedValidationAreVersionedDiscoverableAndReadOnly()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var before = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

            var currentDefinition = Definition(contract, WorldValidationContract.CurrentName);
            var proposedDefinition = Definition(contract, WorldValidationContract.ProposedName);
            Assert.NotNull(currentDefinition.SuccessSchema);
            Assert.NotNull(proposedDefinition.SuccessSchema);
            Assert.True(CanonicalSemanticEquality.SchemaDocumentsEqual(
                currentDefinition.SuccessSchema!, proposedDefinition.SuccessSchema!));
            Assert.Contains("schemaId", currentDefinition.SuccessSchema!.Root.Required);
            Assert.Contains("diagnostics", currentDefinition.SuccessSchema.Root.Required);

            var current = Hk04TransactionalMutationTests.Success(
                contract,
                WorldValidationContract.CurrentName,
                new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.Equal(WorldValidationResult.SchemaId, current.Data!["schemaId"]);
            Assert.Equal(WorldInvariantCatalog.CatalogVersion, current.Data["invariantCatalogVersion"]);
            Assert.Equal("current", current.Data["target"]);
            Assert.True((bool)current.Data["valid"]!);
            Assert.Equal(0, current.Data["diagnosticCount"]);

            var proposed = Hk04TransactionalMutationTests.Success(
                contract,
                WorldValidationContract.ProposedName,
                InvalidMutation(initial));
            Assert.Equal("proposed", proposed.Data!["target"]);
            Assert.False((bool)proposed.Data["valid"]!);
            Assert.True((int)proposed.Data["diagnosticCount"]! >= 3);
            Assert.Equal(before, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(initial.Revision, session.Current.Revision);
        }

        [Fact]
        public void ExplicitValidationAndApplyUseTheSameDiagnosticsAndInvalidStateCannotCommit()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var request = InvalidMutation(initial);
            var before = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

            CapabilityInvocationResult? proposed = null;
            CapabilityInvocationResult? applied = null;
            var exception = Record.Exception(() =>
            {
                proposed = contract.Dispatch(
                    WorldValidationContract.ProposedName,
                    Hk04TransactionalMutationTests.ExactVersion(),
                    request);
                applied = contract.Dispatch(
                    WorldMutationContract.ApplyName,
                    Hk04TransactionalMutationTests.ExactVersion(),
                    request);
            });

            Assert.Null(exception);
            Assert.NotNull(proposed);
            Assert.NotNull(applied);
            Assert.True(proposed!.Success);
            Assert.False((bool)proposed.Data!["valid"]!);
            Assert.False(applied!.Success);
            Assert.Equal("world.change.invalid_candidate", applied.Error!.MachineCode);

            var applyValidation = (IReadOnlyDictionary<string, object?>)applied.Error.Context["validation"]!;
            Assert.Equal(DataSignatures(proposed.Data), DataSignatures(applyValidation));
            Assert.Equal(before, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(initial.Revision, session.Current.Revision);
        }

        private static CapabilityDefinition Definition(ComposedContract contract, string name)
        {
            foreach (var definition in contract.Definitions)
            {
                if (string.Equals(definition.Key.Name, name, StringComparison.Ordinal)) return definition;
            }

            throw new InvalidOperationException("Missing capability definition: " + name);
        }

        private static Dictionary<string, object?> InvalidMutation(WorldState initial)
        {
            return Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk05-invalid",
                Hk04TransactionalMutationTests.RemoveObject("node.root"),
                Hk04TransactionalMutationTests.RemoveObject("node.peer"));
        }

        private static IReadOnlyList<string> Signatures(IReadOnlyList<WorldValidationDiagnostic> diagnostics)
        {
            var values = new List<string>();
            foreach (var diagnostic in diagnostics)
            {
                values.Add(diagnostic.InvariantId + "|" + diagnostic.MachineCode + "|" +
                    diagnostic.Resource + "|" + diagnostic.Path);
            }

            return values.AsReadOnly();
        }

        private static IReadOnlyList<string> DataSignatures(IReadOnlyDictionary<string, object?> validation)
        {
            var values = new List<string>();
            var diagnostics = (IReadOnlyList<object?>)validation["diagnostics"]!;
            foreach (var raw in diagnostics)
            {
                var diagnostic = (IReadOnlyDictionary<string, object?>)raw!;
                values.Add((string)diagnostic["invariantId"]! + "|" +
                    (string)diagnostic["machineCode"]! + "|" +
                    (string)diagnostic["resource"]! + "|" +
                    (string)diagnostic["path"]!);
            }

            return values.AsReadOnly();
        }

        private static WorldStateCandidate InvalidTownCandidate(bool reverseObjects)
        {
            var ana = new WorldObject(
                new WorldObjectId("npc.ana"),
                new WorldTypeId("fixture.npc"),
                new WorldObjectId("building.shop"),
                new[]
                {
                    new WorldReference(new WorldReferenceKind("visits"), new WorldObjectId("plaza.missing")),
                    new WorldReference(new WorldReferenceKind("visits"), new WorldObjectId("plaza.missing"))
                });
            var shop = new WorldObject(
                new WorldObjectId("building.shop"),
                new WorldTypeId("fixture.building"),
                new WorldObjectId("npc.ana"));
            var objects = new List<WorldObject?> { ana, shop };
            if (reverseObjects) objects.Reverse();

            var missingDependency = new WorldReference(
                new WorldReferenceKind("works.at"),
                new WorldObjectId("market.missing"));
            var extension = new WorldExtensionData(
                "arkus.npc-profile",
                1,
                new byte[] { 0x01 },
                new WorldObjectId("npc.missing"),
                new[] { missingDependency, missingDependency });

            return new WorldStateCandidate(
                new WorldId("world.potes"),
                1,
                objects,
                new WorldExtensionData?[] { extension });
        }

        private static IReadOnlyList<WorldStateCandidate> InvariantFixtures()
        {
            var worldId = new WorldId("world.fixture");
            var rootId = new WorldObjectId("node.root");
            var peerId = new WorldObjectId("node.peer");
            var missingId = new WorldObjectId("node.missing");
            var typeId = new WorldTypeId("fixture.item");
            var kind = new WorldReferenceKind("fixture.link");
            var root = new WorldObject(rootId, typeId);
            var peer = new WorldObject(peerId, typeId);
            var validObjects = new WorldObject?[] { root, peer };
            var duplicateReference = new WorldReference(kind, peerId);
            var duplicateDependency = new WorldReference(kind, peerId);

            return new List<WorldStateCandidate>
            {
                new WorldStateCandidate(default, 0, Array.Empty<WorldObject?>()),
                new WorldStateCandidate(worldId, 0, Array.Empty<WorldObject?>(), schemaVersion: 999),
                new WorldStateCandidate(worldId, -1, Array.Empty<WorldObject?>()),
                new WorldStateCandidate(worldId, 0, new WorldObject?[] { null }),
                new WorldStateCandidate(worldId, 0, new WorldObject?[] { root, root }),
                new WorldStateCandidate(worldId, 0, new WorldObject?[]
                {
                    new WorldObject(rootId, typeId, missingId)
                }),
                new WorldStateCandidate(worldId, 0, new WorldObject?[]
                {
                    new WorldObject(rootId, typeId, rootId)
                }),
                new WorldStateCandidate(worldId, 0, new WorldObject?[]
                {
                    new WorldObject(rootId, typeId, references: new WorldReference[] { null! })
                }),
                new WorldStateCandidate(worldId, 0, new WorldObject?[]
                {
                    new WorldObject(rootId, typeId, references: new[] { duplicateReference, duplicateReference }), peer
                }),
                new WorldStateCandidate(worldId, 0, new WorldObject?[]
                {
                    new WorldObject(rootId, typeId, references: new[] { new WorldReference(kind, missingId) })
                }),
                new WorldStateCandidate(worldId, 0, new WorldObject?[]
                {
                    new WorldObject(rootId, typeId, peerId), new WorldObject(peerId, typeId, rootId)
                }),
                new WorldStateCandidate(worldId, 0, validObjects, new WorldExtensionData?[] { null }),
                new WorldStateCandidate(worldId, 0, validObjects, new WorldExtensionData?[]
                {
                    new WorldExtensionData("fixture.data", 1, new byte[] { 1 }),
                    new WorldExtensionData("fixture.data", 1, new byte[] { 2 })
                }),
                new WorldStateCandidate(worldId, 0, validObjects, new WorldExtensionData?[]
                {
                    new WorldExtensionData("fixture.data", 1, new byte[] { 1 }, missingId)
                }),
                new WorldStateCandidate(worldId, 0, validObjects, new WorldExtensionData?[]
                {
                    new WorldExtensionData("fixture.data", 1, new byte[] { 1 }, dependencies: new WorldReference[] { null! })
                }),
                new WorldStateCandidate(worldId, 0, validObjects, new WorldExtensionData?[]
                {
                    new WorldExtensionData("fixture.data", 1, new byte[] { 1 },
                        dependencies: new[] { duplicateDependency, duplicateDependency })
                }),
                new WorldStateCandidate(worldId, 0, validObjects, new WorldExtensionData?[]
                {
                    new WorldExtensionData("fixture.data", 1, new byte[] { 1 },
                        dependencies: new[] { new WorldReference(kind, missingId) })
                })
            }.AsReadOnly();
        }
    }
}
