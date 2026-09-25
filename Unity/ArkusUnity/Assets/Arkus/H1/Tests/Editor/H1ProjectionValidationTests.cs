using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Arkus.H1.Editor.Tests
{
    public sealed class H1ProjectionValidationTests
    {
        private const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        private const string ManagedScenes = "Assets/Arkus/H1/ManagedScenes";
        private const string ManagedPrefabs = "Assets/Arkus/H1/ManagedPrefabs";
        private const string SceneId = "arkus.h1-05.scene.potes";
        private const string TransformSchema = "arkus.h1.component.transform@1";
        private string _faultPath;

        [SetUp]
        public void SetUp()
        {
            DeleteGenerated();
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
            _faultPath = Path.Combine(Path.GetFullPath(Path.Combine(Application.dataPath, "..")), "Library", "Arkus", "H1Projection", "inject-non-finite-transform");
            DeleteFault();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteFault();
            DeleteGenerated();
        }

        [Test]
        public void Inventory_IsFiniteUniqueVersionedAndDeterministicallyOrdered()
        {
            var inventory = H1ProjectionValidation.Inventory();
            Assert.That(inventory, Has.Length.EqualTo(12));
            Assert.That(inventory.Select(value => value.invariantId).Distinct(StringComparer.Ordinal).Count(), Is.EqualTo(inventory.Length));
            Assert.That(inventory.Select(value => value.order), Is.Ordered);
            Assert.That(inventory.All(value => value.phase == H1ProjectionValidation.Preflight || value.phase == H1ProjectionValidation.PostMaterialization), Is.True);
            Assert.That(inventory.Select(value => value.invariantId), Does.Contain("unity.scene.finite-transform"));
            Assert.That(H1ProjectionValidation.InventoryJson(), Does.Contain(H1ProjectionValidation.InventorySchema));
        }

        [Test]
        public void ProposedValidation_AggregatesIndependentDefectsDeterministically_WhenInputOrderReverses()
        {
            var firstPlan = BuildValidPlan("aggregate-a", 2);
            foreach (var node in firstPlan.nodes)
            {
                node.sourcePath = SourceRoot + "/missing-h1-08-source.fbx";
                node.sourceGuid = new string('0', 32);
            }
            firstPlan.nodes.Single(node => node.objectId == "node.1").parentObjectId = "missing.parent";
            firstPlan.inputDigest = HashText("aggregate-shared-input");

            var reversedPlan = Clone(firstPlan);
            reversedPlan.nodes = reversedPlan.nodes.Reverse().ToArray();

            var first = Execute("validate-proposed", firstPlan);
            var second = Execute("validate-proposed", reversedPlan);

            Assert.That(first.validation, Is.Not.Null);
            Assert.That(first.validation.valid, Is.False);
            Assert.That(first.validation.phase, Is.EqualTo(H1ProjectionValidation.Preflight));
            Assert.That(first.validation.diagnostics.Select(value => value.code), Does.Contain("projection.source-missing"));
            Assert.That(first.validation.diagnostics.Select(value => value.code), Does.Contain("projection.unbound-parent"));
            Assert.That(StableRows(first.validation), Is.EqualTo(StableRows(second.validation)),
                "reversing ambiguous/defective input enumeration must not change the complete diagnostic set/order");
        }

        [Test]
        public void InvalidNodeShape_DoesNotSuppressIndependentHierarchyDiagnostic()
        {
            var plan = BuildValidPlan("local-suppression", 2);
            plan.nodes[0].components = new ProjectionComponent[0];
            plan.nodes[1].parentObjectId = "missing.parent";
            plan.inputDigest = HashText("local-suppression-input");

            var reply = Execute("validate-proposed", plan);
            Assert.That(reply.validation, Is.Not.Null);
            Assert.That(reply.validation.valid, Is.False);
            Assert.That(reply.validation.diagnostics.Select(value => value.code), Does.Contain("projection.invalid-node"));
            Assert.That(reply.validation.diagnostics.Select(value => value.code), Does.Contain("projection.unbound-parent"),
                "one malformed node may suppress only checks that depend on that node, never an independent hierarchy defect");
        }

        [Test]
        public void ProposedValidation_ComposesCanonicalSourceIdentityCheck_ForIncompleteLocatorParts()
        {
            foreach (var missingPart in new[] { "path", "guid", "local-file-id" })
            {
                var plan = BuildValidPlan("incomplete-" + missingPart, 1);
                if (missingPart == "path") plan.nodes[0].sourcePath = "";
                else if (missingPart == "guid") plan.nodes[0].sourceGuid = "";
                else plan.nodes[0].sourceLocalFileId = "";

                var reply = Execute("validate-proposed", plan);
                Assert.That(reply.validation, Is.Not.Null, missingPart);
                Assert.That(reply.validation.valid, Is.False, missingPart);
                Assert.That(reply.validation.diagnostics.Any(value =>
                        value.invariantId == "unity.plan.source-binding" &&
                        (value.code == "projection.source-locator-mismatch" || value.code == "projection.source-rebound")),
                    Is.True, missingPart + " must reach the accepted H1 source resolver rather than being skipped");
            }
        }

        [Test]
        public void InvalidRequestEnvelope_ReturnsStructuredDiagnostic_NotRawException()
        {
            var raw = H1SceneProjection.Execute("{");
            var reply = JsonUtility.FromJson<ProjectionReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.errorCode, Is.EqualTo("projection.invalid-worker-plan"));
            Assert.That(reply.validation, Is.Not.Null);
            Assert.That(reply.validation.valid, Is.False);
            Assert.That(reply.validation.phase, Is.EqualTo(H1ProjectionValidation.Preflight));
            Assert.That(reply.validation.diagnostics.Single().code, Is.EqualTo("projection.invalid-worker-plan"));
            Assert.That(reply.validationInventory, Is.Not.Empty);
        }

        [Test]
        public void UnsupportedComponentSchema_ReturnsStructuredDiagnostic_NotRawUnityException()
        {
            var plan = BuildValidPlan("unsupported", 1);
            plan.nodes[0].components = plan.nodes[0].components.Concat(new[]
            {
                new ProjectionComponent { schemaId = "arkus.h1.component.unsupported@1", kind = "unsupported" }
            }).ToArray();
            plan.inputDigest = HashText("unsupported-component-schema");

            var reply = Execute("validate-proposed", plan);
            Assert.That(reply.errorCode, Is.EqualTo("projection.component-schema-unsupported"));
            Assert.That(reply.validation, Is.Not.Null);
            Assert.That(reply.validation.valid, Is.False);
            Assert.That(reply.validation.diagnostics.Single(value => value.code == "projection.component-schema-unsupported").invariantId,
                Is.EqualTo("unity.plan.component"));
        }

        [Test]
        public void NonFinitePostflightSample_FailsBeforePublication_AndKeepsPreviousGenerationActive()
        {
            var baselinePlan = BuildValidPlan("baseline", 1);
            var baseline = Execute("materialize", baselinePlan);
            Assert.That(baseline.errorCode, Is.Empty);
            Assert.That(baseline.observation.active, Is.True);
            var activeGeneration = baseline.observation.generationId;

            Directory.CreateDirectory(Path.GetDirectoryName(_faultPath));
            File.WriteAllText(_faultPath, "H1-08 bounded finite-check sample fault");

            var attemptedPlan = BuildValidPlan("non-finite", 1);
            attemptedPlan.nodes[0].positionMm.x = 2500;
            attemptedPlan.inputDigest = HashText("non-finite-attempt");
            var rejected = Execute("materialize", attemptedPlan);

            Assert.That(rejected.errorCode, Is.EqualTo("projection.non-finite-transform"));
            Assert.That(rejected.validation, Is.Not.Null);
            Assert.That(rejected.validation.phase, Is.EqualTo(H1ProjectionValidation.PostMaterialization));
            Assert.That(rejected.validation.diagnostics.Single(value => value.code == "projection.non-finite-transform").invariantId,
                Is.EqualTo("unity.scene.finite-transform"));
            Assert.That(rejected.observation.active, Is.True);
            Assert.That(rejected.observation.generationId, Is.EqualTo(activeGeneration),
                "failed effective postflight must not receive the active-generation receipt");
            Assert.That(rejected.observation.inputDigest, Is.EqualTo(baselinePlan.inputDigest));
        }

        [Test]
        public void CurrentValidation_UsesActiveManifestIdentity_NotCallerProposedIdentity()
        {
            var activePlan = BuildValidPlan("current-active", 1);
            var materialized = Execute("materialize", activePlan);
            Assert.That(materialized.errorCode, Is.Empty);

            var callerPlan = BuildValidPlan("current-caller-b", 1);
            var current = Execute("validate-current", callerPlan);

            Assert.That(current.errorCode, Is.Empty);
            Assert.That(current.validation, Is.Not.Null);
            Assert.That(current.validation.valid, Is.True);
            Assert.That(current.validation.inputDigest, Is.EqualTo(activePlan.inputDigest));
            Assert.That(current.observation.inputDigest, Is.EqualTo(activePlan.inputDigest));
            Assert.That(current.observation.canonicalHash, Is.EqualTo(activePlan.canonicalHash));
            Assert.That(current.observation.catalogueFingerprint, Is.EqualTo(activePlan.catalogueFingerprint));
            Assert.That(current.observation.inputDigest, Is.Not.EqualTo(callerPlan.inputDigest));
        }

        [TestCase("graphDigest", "projection.active-scene-drift")]
        [TestCase("realizationDigest", "projection.active-prefab-drift")]
        public void CurrentValidation_RejectsActiveManifestDigestDrift(string field, string expectedCode)
        {
            var plan = BuildValidPlan("manifest-drift-" + field, 1);
            var materialized = Execute("materialize", plan);
            Assert.That(materialized.errorCode, Is.Empty);

            var manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ManagedScenes, "current.json"));
            var manifest = File.ReadAllText(manifestPath);
            var original = field == "graphDigest" ? materialized.observation.graphDigest : materialized.observation.realizationDigest;
            var replacement = HashText("tampered-" + field);
            Assert.That(manifest, Does.Contain("\"" + field + "\":\"" + original + "\""));
            File.WriteAllText(manifestPath,
                manifest.Replace("\"" + field + "\":\"" + original + "\"", "\"" + field + "\":\"" + replacement + "\""));

            var current = Execute("validate-current", BuildValidPlan("unrelated-caller-" + field, 1));
            Assert.That(current.errorCode, Is.EqualTo(expectedCode));
            Assert.That(current.validation, Is.Not.Null);
            Assert.That(current.validation.valid, Is.False);
            Assert.That(current.validation.diagnostics.Any(value =>
                value.invariantId == "unity.scene.effective-observation" && value.code == expectedCode), Is.True);
        }

        [Test]
        public void CurrentValidation_IsPublicVersionedAndPostMaterializationScoped()
        {
            var plan = BuildValidPlan("current", 1);
            var materialized = Execute("materialize", plan);
            Assert.That(materialized.errorCode, Is.Empty);

            var current = Execute("validate-current", plan);
            Assert.That(current.errorCode, Is.Empty);
            Assert.That(current.validation, Is.Not.Null);
            Assert.That(current.validation.valid, Is.True);
            Assert.That(current.validation.phase, Is.EqualTo(H1ProjectionValidation.PostMaterialization));
            Assert.That(current.validationInventory.Select(value => value.invariantId), Does.Contain("unity.scene.managed-marker"));
            Assert.That(current.validationInventory.Select(value => value.invariantId), Does.Contain("unity.scene.component"));
        }

        private static ProjectionPlan BuildValidPlan(string salt, int nodeCount)
        {
            var wallPath = SourceRoot + "/Wall_Plaster_Window_Wide_Flat.fbx";
            var mesh = AssetDatabase.LoadAllAssetsAtPath(wallPath).OfType<Mesh>().FirstOrDefault();
            Assert.That(mesh, Is.Not.Null, "accepted facade FBX must expose a Mesh");
            Assert.That(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mesh, out string guid, out long fileId), Is.True, wallPath);
            var full = Path.GetFullPath(Path.Combine(Application.dataPath, "..", wallPath));
            var sha = HashBytes(File.ReadAllBytes(full));

            var nodes = new List<ProjectionNode>();
            for (var index = 0; index < nodeCount; index++)
            {
                nodes.Add(new ProjectionNode
                {
                    objectId = "node." + index,
                    parentObjectId = "",
                    sourceKind = "asset",
                    sourceLogicalId = "quaternius.medieval.asset.wall-plaster-window-wide-flat",
                    sourcePath = wallPath,
                    sourceGuid = guid,
                    sourceLocalFileId = fileId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    sourceContentSha256 = sha,
                    positionMm = Vec(index * 1000, 0, 0),
                    rotationMilliDegrees = Vec(0, 0, 0),
                    scalePpm = Vec(1000000, 1000000, 1000000),
                    components = new[] { new ProjectionComponent { schemaId = TransformSchema, kind = "transform" } }
                });
            }

            return new ProjectionPlan
            {
                schemaId = "arkus.h1-managed-scene-plan@1",
                sceneLogicalId = SceneId,
                worldId = "world.potes.h1-08-probe",
                worldRevision = 1,
                canonicalHash = HashText("canonical-" + salt),
                catalogueFingerprint = HashText("catalogue-" + salt),
                inputDigest = HashText("input-" + salt),
                nodes = nodes.ToArray()
            };
        }

        private static ProjectionPlan Clone(ProjectionPlan plan) => JsonUtility.FromJson<ProjectionPlan>(JsonUtility.ToJson(plan));

        private static ProjectionReply Execute(string mode, ProjectionPlan plan)
        {
            var payload = JsonUtility.ToJson(new ProjectionRequest
            {
                schemaId = "arkus.h1-projection-worker-request@1",
                mode = mode,
                sceneLogicalId = SceneId,
                plan = plan
            });
            var raw = H1SceneProjection.Execute(payload);
            var reply = JsonUtility.FromJson<ProjectionReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.schemaId, Is.EqualTo("arkus.h1-projection-worker-result@1"), raw);
            return reply;
        }

        private static string[] StableRows(H1ValidationResult result) => result.diagnostics.Select(value =>
            value.invariantId + "|" + value.code + "|" + value.canonicalResource + "|" + value.logicalAsset + "|" + value.managedPath).ToArray();

        private void DeleteFault()
        {
            if (!string.IsNullOrEmpty(_faultPath) && File.Exists(_faultPath)) File.Delete(_faultPath);
        }

        private static void DeleteGenerated()
        {
            if (AssetDatabase.IsValidFolder(ManagedScenes)) AssetDatabase.DeleteAsset(ManagedScenes);
            if (AssetDatabase.IsValidFolder(ManagedPrefabs)) AssetDatabase.DeleteAsset(ManagedPrefabs);
            AssetDatabase.Refresh();
        }

        private static ProjectionVector Vec(long x, long y, long z) => new ProjectionVector { x = x, y = y, z = z };
        private static string HashText(string text) => HashBytes(System.Text.Encoding.UTF8.GetBytes(text));
        private static string HashBytes(byte[] bytes)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        [Serializable] private sealed class ProjectionRequest { public string schemaId; public string mode; public string sceneLogicalId; public ProjectionPlan plan; }
        [Serializable] private sealed class ProjectionPlan
        {
            public string schemaId; public string sceneLogicalId; public string worldId; public long worldRevision;
            public string canonicalHash; public string catalogueFingerprint; public string inputDigest; public ProjectionNode[] nodes;
        }
        [Serializable] private sealed class ProjectionNode
        {
            public string objectId; public string parentObjectId; public string sourceKind; public string sourceLogicalId;
            public string sourcePath; public string sourceGuid; public string sourceLocalFileId; public string sourceContentSha256;
            public ProjectionVector positionMm; public ProjectionVector rotationMilliDegrees; public ProjectionVector scalePpm;
            public ProjectionComponent[] components;
        }
        [Serializable] private sealed class ProjectionComponent
        {
            public string schemaId; public string kind; public string relation; public string targetObjectId; public string referenceKind;
            public string referenceLogicalId; public string referencePath; public string referenceGuid; public string referenceLocalFileId; public string referenceContentSha256;
        }
        [Serializable] private sealed class ProjectionVector { public long x; public long y; public long z; }
        [Serializable] private sealed class ProjectionReply
        {
            public string schemaId; public string sceneLogicalId; public string expectedInputDigest; public string errorCode;
            public ProjectionObservation observation; public H1ValidationResult validation; public H1ValidationInvariantDescriptor[] validationInventory;
        }
        [Serializable] private sealed class ProjectionObservation
        {
            public string schemaId; public string sceneLogicalId; public bool active; public string generationId; public string inputDigest;
            public string canonicalHash; public string catalogueFingerprint; public string graphDigest; public string realizationDigest;
        }
    }
}
