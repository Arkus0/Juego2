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
        private static readonly IReadOnlyList<string> ExpectedOwnedInvariantIds = new[]
        {
            "arkus.world.identity.initialized/v1",
            "arkus.world.schema.supported/v1",
            "arkus.world.revision.nonnegative/v1",
            "arkus.world.object.entry-present/v1",
            "arkus.world.object.identity-unique/v1",
            "arkus.world.object.container-resolves/v1",
            "arkus.world.object.container-not-self/v1",
            "arkus.world.object.reference-entry-present/v1",
            "arkus.world.object.reference-unique/v1",
            "arkus.world.object.reference-target-resolves/v1",
            "arkus.world.object.containment-acyclic/v1",
            "arkus.world.extension.entry-present/v1",
            "arkus.world.extension.identity-unique/v1",
            "arkus.world.extension.subject-resolves/v1",
            "arkus.world.extension.dependency-entry-present/v1",
            "arkus.world.extension.dependency-unique/v1",
            "arkus.world.extension.dependency-target-resolves/v1"
        };

        [Fact]
        public void ValidatorInventoryExactlyClassifiesTheIndependentInvariantUniverse()
        {
            Assert.Empty(WorldValidationEngine.FindInventoryIssues());
            Assert.Equal(WorldInvariantCatalog.All.Count, WorldValidationEngine.ValidatorInventory.Count);

            var expected = new HashSet<string>(ExpectedOwnedInvariantIds, StringComparer.Ordinal);
            var catalog = new HashSet<string>(StringComparer.Ordinal);
            foreach (var invariant in WorldInvariantCatalog.All) Assert.True(catalog.Add(invariant.InvariantId));
            Assert.True(expected.SetEquals(catalog));

            var registered = new HashSet<string>(StringComparer.Ordinal);
            foreach (var validator in WorldValidationEngine.ValidatorInventory)
            {
                Assert.True(registered.Add(validator.InvariantId));
                Assert.False(string.IsNullOrWhiteSpace(validator.MachineCode));
                Assert.Contains(validator.Category, new[] { "structural", "identity", "reference" });
            }

            Assert.True(expected.SetEquals(registered));

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

            var expected = new HashSet<string>(ExpectedOwnedInvariantIds, StringComparer.Ordinal);
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
            Assert.Empty(DiagnosticShapeIssues(first.Diagnostics));

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

            var reversed = new List<string>(Signatures(first.Diagnostics));
            reversed.Reverse();
            Assert.False(SequencesEqual(Signatures(first.Diagnostics), reversed));

            var ambiguous = new WorldValidationDiagnostic(
                "world.injected",
                WorldDiagnosticSeverity.Error,
                string.Empty,
                string.Empty,
                "arkus.world.injected/v1",
                "Injected ambiguous diagnostic.",
                new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.NotEmpty(DiagnosticShapeIssues(new[] { ambiguous }));
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
            Assert.Equal(
                currentDefinition.SuccessSchema!.SemanticFingerprint(),
                proposedDefinition.SuccessSchema!.SemanticFingerprint());
            Assert.Contains("schemaId", currentDefinition.SuccessSchema.Root.RequiredProperties);
            Assert.Contains("diagnostics", currentDefinition.SuccessSchema.Root.RequiredProperties);

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
        public void EveryPublicCanonicalMutationRouteEnforcesExplicitValidation()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new PortableWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var request = InvalidMutation(initial);
            var before = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var explicitValidation = contract.Dispatch(
                WorldValidationContract.ProposedName,
                Hk04TransactionalMutationTests.ExactVersion(),
                request);

            Assert.True(explicitValidation.Success);
            Assert.False((bool)explicitValidation.Data!["valid"]!);

            var mutationRoutes = new List<CapabilityDefinition>();
            foreach (var definition in contract.Definitions)
            {
                if (definition.SideEffect == SideEffectClass.CanonicalMutation) mutationRoutes.Add(definition);
            }

            Assert.NotEmpty(mutationRoutes);
            var covered = new HashSet<string>(StringComparer.Ordinal);
            foreach (var definition in mutationRoutes)
            {
                if (string.Equals(definition.Key.Name, WorldMutationContract.ApplyName, StringComparison.Ordinal))
                {
                    var result = contract.Dispatch(
                        definition.Key.Name,
                        ContractVersionRange.Exact(definition.Key.Version),
                        request);
                    Assert.Empty(MutationValidationIssues(explicitValidation, result));
                    Assert.True(covered.Add(definition.Key.Name));
                    continue;
                }

                if (string.Equals(definition.Key.Name, WorldPortabilityContract.ImportName, StringComparison.Ordinal))
                {
                    var snapshot = session.ExportSnapshot(Hk01TestFixtures.EmptyRequest()).Data!;
                    var invalidSnapshot = new Dictionary<string, object?>(snapshot, StringComparer.Ordinal)
                    {
                        ["authoredStateBase64"] = Convert.ToBase64String(new byte[] { 0x00 })
                    };
                    var invalidImport = new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["idempotencyKey"] = "request.hk05-invalid-snapshot",
                        ["expectedRevision"] = session.Current.Revision,
                        ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(session.Current),
                        ["snapshot"] = invalidSnapshot
                    };
                    var result = contract.Dispatch(
                        definition.Key.Name,
                        ContractVersionRange.Exact(definition.Key.Version),
                        invalidImport);
                    Assert.Empty(SnapshotImportValidationIssues(result));
                    Assert.True(covered.Add(definition.Key.Name));
                    continue;
                }

                Assert.True(false, "Canonical mutation route lacks an explicit route-specific validation control: " + definition.Key);
            }

            Assert.Equal(mutationRoutes.Count, covered.Count);

            var acceptedInvalidMutant = CapabilityInvocationResult.Succeeded(
                new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.Contains(
                "mutation-accepted-invalid-state",
                MutationValidationIssues(explicitValidation, acceptedInvalidMutant));
            Assert.Contains(
                "snapshot-import-accepted-invalid-state",
                SnapshotImportValidationIssues(acceptedInvalidMutant));

            var skippedValidationMutant = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.Contains(
                "mutation-validation-skipped",
                MutationValidationIssues(explicitValidation, skippedValidationMutant));

            Assert.Equal(before, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(initial.Revision, session.Current.Revision);
        }

        [Fact]
        public void ExpectedValidationErrorsDoNotEscapeAsRuntimeExceptions()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var malformed = new Dictionary<string, object?>(StringComparer.Ordinal);

            Assert.False(EscapesException(() => contract.Dispatch(
                WorldValidationContract.ProposedName,
                Hk04TransactionalMutationTests.ExactVersion(),
                malformed)));

            var result = contract.Dispatch(
                WorldValidationContract.ProposedName,
                Hk04TransactionalMutationTests.ExactVersion(),
                malformed);
            Assert.False(result.Success);
            Assert.Contains(result.Error!.MachineCode, new[]
            {
                "contract.invalid_request",
                "world.change.invalid_request"
            });

            Assert.True(EscapesException(() => throw new InvalidOperationException("injected escaped exception")));
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
            Assert.Empty(MutationValidationIssues(proposed!, applied!));

            var divergentValidation = CapabilityInvocationResult.Succeeded(
                WorldValidationEngine.ValidateCandidate(InvalidTownCandidate(false)).ToData());
            Assert.Contains(
                "validate-apply-disagreement",
                MutationValidationIssues(divergentValidation, applied!));

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

        private static IReadOnlyList<string> MutationValidationIssues(
            CapabilityInvocationResult explicitValidation,
            CapabilityInvocationResult mutation)
        {
            var issues = new List<string>();
            if (!explicitValidation.Success || explicitValidation.Data == null ||
                !explicitValidation.Data.TryGetValue("valid", out var rawValid) || !(rawValid is bool valid) || valid)
            {
                issues.Add("explicit-validation-not-invalid");
                return issues.AsReadOnly();
            }

            if (mutation.Success)
            {
                issues.Add("mutation-accepted-invalid-state");
                return issues.AsReadOnly();
            }

            if (mutation.Error == null ||
                !string.Equals(mutation.Error.MachineCode, "world.change.invalid_candidate", StringComparison.Ordinal) ||
                !mutation.Error.Context.TryGetValue("validation", out var rawValidation) ||
                !(rawValidation is IReadOnlyDictionary<string, object?> applyValidation))
            {
                issues.Add("mutation-validation-skipped");
                return issues.AsReadOnly();
            }

            if (!SequencesEqual(DataSignatures(explicitValidation.Data), DataSignatures(applyValidation)))
                issues.Add("validate-apply-disagreement");
            return issues.AsReadOnly();
        }

        private static IReadOnlyList<string> SnapshotImportValidationIssues(CapabilityInvocationResult mutation)
        {
            var issues = new List<string>();
            if (mutation.Success)
            {
                issues.Add("snapshot-import-accepted-invalid-state");
                return issues.AsReadOnly();
            }

            if (mutation.Error == null ||
                !string.Equals(mutation.Error.MachineCode, "world.snapshot.invalid_state", StringComparison.Ordinal))
            {
                issues.Add("snapshot-import-validation-skipped");
            }

            return issues.AsReadOnly();
        }

        private static IReadOnlyList<string> DiagnosticShapeIssues(IEnumerable<WorldValidationDiagnostic> diagnostics)
        {
            var issues = new List<string>();
            foreach (var diagnostic in diagnostics)
            {
                if (string.IsNullOrWhiteSpace(diagnostic.Resource)) issues.Add("diagnostic-resource-ambiguous");
                if (string.IsNullOrWhiteSpace(diagnostic.Path) || !diagnostic.Path.StartsWith("$/", StringComparison.Ordinal))
                    issues.Add("diagnostic-path-ambiguous");
                if (diagnostic.RemediationContext.Count == 0) issues.Add("diagnostic-context-empty");
            }
            return issues.AsReadOnly();
        }

        private static bool EscapesException(Func<CapabilityInvocationResult> call)
        {
            try
            {
                call();
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private static bool SequencesEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left.Count != right.Count) return false;
            for (var index = 0; index < left.Count; index++)
            {
                if (!string.Equals(left[index], right[index], StringComparison.Ordinal)) return false;
            }
            return true;
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
