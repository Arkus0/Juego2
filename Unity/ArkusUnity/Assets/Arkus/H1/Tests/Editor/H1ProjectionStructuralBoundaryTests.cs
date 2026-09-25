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
    public sealed class H1ProjectionStructuralBoundaryTests
    {
        private const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        private const string ManagedScenes = "Assets/Arkus/H1/ManagedScenes";
        private const string ManagedPrefabs = "Assets/Arkus/H1/ManagedPrefabs";
        private const string SceneId = "arkus.h1-05.scene.potes";
        private const string TransformSchema = "arkus.h1.component.transform@1";

        [SetUp]
        public void SetUp()
        {
            DeleteGenerated();
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
        }

        [TearDown]
        public void TearDown()
        {
            DeleteGenerated();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void MaterializationStructuralPrecondition_CannotPassPublicPreflight(bool omitParent)
        {
            var plan = BuildValidPlan("null-parent");
            var payload = RequestPayload("validate-proposed", plan);
            const string parentField = "\"parentObjectId\":\"\"";
            Assert.That(payload, Does.Contain(parentField));
            payload = omitParent
                ? payload.Replace(parentField + ",", "")
                : payload.Replace(parentField, "\"parentObjectId\":null");

            var proposed = ExecuteRaw(payload);
            AssertStructuredInvalid(proposed, "projection.invalid-node");
            Assert.That(proposed.validation.diagnostics.Any(value => value.invariantId == "unity.plan.node-shape"), Is.True);

            var effective = ExecuteRaw(payload.Replace("\"mode\":\"validate-proposed\"", "\"mode\":\"materialize\""));
            AssertStructuredInvalid(effective, "projection.invalid-node");
            Assert.That(effective.errorCode, Is.Not.EqualTo("projection.editor-failure"));
            Assert.That(effective.observation.active, Is.False,
                "effective materialization must not be entered without the shared successful preflight token");
        }

        [Test]
        public void CurrentValidation_InvalidActiveManifest_IsStructuredExpectedInvalidity()
        {
            var materialized = Execute("materialize", BuildValidPlan("invalid-manifest"));
            Assert.That(materialized.errorCode, Is.Empty);

            var manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ManagedScenes, "current.json"));
            File.WriteAllText(manifestPath, "{\"schemaId\":\"broken\"}");

            var current = Execute("validate-current", BuildValidPlan("invalid-manifest-caller"));
            AssertStructuredInvalid(current, "projection.manifest-invalid");
            Assert.That(current.validation.diagnostics.Any(value => value.invariantId == "unity.scene.effective-observation"), Is.True);
            Assert.That(current.errorCode, Is.Not.EqualTo("projection.editor-failure"));
        }

        [Test]
        public void Materialize_InvalidActiveManifest_IsStructuredAndDoesNotStageOrPublish()
        {
            var baseline = Execute("materialize", BuildValidPlan("materialize-invalid-manifest-a"));
            Assert.That(baseline.errorCode, Is.Empty);
            Assert.That(baseline.observation.active, Is.True);

            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var manifestPath = Path.Combine(projectRoot, ManagedScenes, "current.json");
            var generationsPath = Path.Combine(projectRoot, ManagedScenes, "generations");
            var beforeGenerationFiles = Directory.GetFiles(generationsPath, "*.unity")
                .Select(Path.GetFileName).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            const string corruptedManifest = "{\"schemaId\":\"broken\"}";
            File.WriteAllText(manifestPath, corruptedManifest);

            var attempted = Execute("materialize", BuildValidPlan("materialize-invalid-manifest-b"));

            AssertStructuredInvalid(attempted, "projection.manifest-invalid");
            Assert.That(attempted.validation.phase, Is.EqualTo(H1ProjectionValidation.PostMaterialization));
            Assert.That(attempted.validation.scope, Is.EqualTo("proposed-effective"));
            Assert.That(attempted.validation.diagnostics.Any(value =>
                value.invariantId == "unity.scene.effective-observation" && value.code == "projection.manifest-invalid"), Is.True);
            Assert.That(attempted.errorCode, Is.Not.EqualTo("projection.editor-failure"));
            Assert.That(File.ReadAllText(manifestPath), Is.EqualTo(corruptedManifest),
                "failed materialization must not publish a replacement current manifest");

            var afterGenerationFiles = Directory.GetFiles(generationsPath, "*.unity")
                .Select(Path.GetFileName).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            CollectionAssert.AreEqual(beforeGenerationFiles, afterGenerationFiles,
                "invalid active lifecycle state must fail before staging a new managed generation");
        }

        [Test]
        public void CurrentValidation_MissingManagedGeneration_IsStructuredExpectedInvalidity()
        {
            var materialized = Execute("materialize", BuildValidPlan("missing-scene"));
            Assert.That(materialized.errorCode, Is.Empty);
            Assert.That(materialized.observation.active, Is.True);

            var scenePath = ManagedScenes + "/generations/" + materialized.observation.generationId + ".unity";
            Assert.That(AssetDatabase.DeleteAsset(scenePath), Is.True, scenePath);
            AssetDatabase.Refresh();

            var current = Execute("validate-current", BuildValidPlan("missing-scene-caller"));
            AssertStructuredInvalid(current, "projection.active-scene-missing");
            Assert.That(current.validation.diagnostics.Any(value => value.invariantId == "unity.scene.effective-observation"), Is.True);
            Assert.That(current.errorCode, Is.Not.EqualTo("projection.editor-failure"));
        }

        private static void AssertStructuredInvalid(ProjectionReply reply, string expectedCode)
        {
            Assert.That(reply.validation, Is.Not.Null);
            Assert.That(reply.validation.valid, Is.False);
            Assert.That(reply.errorCode, Is.EqualTo(expectedCode));
            Assert.That(reply.validation.diagnostics, Is.Not.Empty);
            Assert.That(reply.validation.diagnostics.Any(value => value.code == expectedCode), Is.True);
        }

        private static ProjectionPlan BuildValidPlan(string salt)
        {
            var wallPath = SourceRoot + "/Wall_Plaster_Window_Wide_Flat.fbx";
            var mesh = AssetDatabase.LoadAllAssetsAtPath(wallPath).OfType<Mesh>().FirstOrDefault();
            Assert.That(mesh, Is.Not.Null, "accepted facade FBX must expose a Mesh");
            Assert.That(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mesh, out string guid, out long fileId), Is.True, wallPath);
            var full = Path.GetFullPath(Path.Combine(Application.dataPath, "..", wallPath));

            return new ProjectionPlan
            {
                schemaId = "arkus.h1-managed-scene-plan@1",
                sceneLogicalId = SceneId,
                worldId = "world.potes.h1-08-structural",
                worldRevision = 1,
                canonicalHash = HashText("canonical-" + salt),
                catalogueFingerprint = HashText("catalogue-" + salt),
                inputDigest = HashText("input-" + salt),
                nodes = new[]
                {
                    new ProjectionNode
                    {
                        objectId = "node.0",
                        parentObjectId = "",
                        sourceKind = "asset",
                        sourceLogicalId = "quaternius.medieval.asset.wall-plaster-window-wide-flat",
                        sourcePath = wallPath,
                        sourceGuid = guid,
                        sourceLocalFileId = fileId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        sourceContentSha256 = HashBytes(File.ReadAllBytes(full)),
                        positionMm = Vec(0, 0, 0),
                        rotationMilliDegrees = Vec(0, 0, 0),
                        scalePpm = Vec(1000000, 1000000, 1000000),
                        components = new[] { new ProjectionComponent { schemaId = TransformSchema, kind = "transform" } }
                    }
                }
            };
        }

        private static ProjectionReply Execute(string mode, ProjectionPlan plan)
        {
            return ExecuteRaw(RequestPayload(mode, plan));
        }

        private static string RequestPayload(string mode, ProjectionPlan plan)
        {
            return JsonUtility.ToJson(new ProjectionRequest
            {
                schemaId = "arkus.h1-projection-worker-request@1",
                mode = mode,
                sceneLogicalId = SceneId,
                plan = plan
            });
        }

        private static ProjectionReply ExecuteRaw(string payload)
        {
            var raw = H1SceneProjection.Execute(payload);
            var reply = JsonUtility.FromJson<ProjectionReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.schemaId, Is.EqualTo("arkus.h1-projection-worker-result@1"), raw);
            return reply;
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