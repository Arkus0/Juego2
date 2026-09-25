using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Arkus.H1.Editor.Tests
{
    public sealed class H1ProjectionContentShapeTests
    {
        private const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        private const string SceneId = "arkus.h1-05.scene.potes";
        private const string TransformSchema = "arkus.h1.component.transform@1";
        private const string RendererSchema = "arkus.h1.component.mesh-renderer@1";

        [Test]
        public void StreetCorner_BadMaterialReferenceAndIndependentHierarchyDefect_AggregateActionably()
        {
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True);
            var wallPath = SourceRoot + "/Wall_Plaster_Window_Wide_Flat.fbx";
            var mesh = AssetDatabase.LoadAllAssetsAtPath(wallPath).OfType<Mesh>().First();
            Assert.That(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mesh, out string sourceGuid, out long sourceFileId), Is.True);
            var sourceHash = Hash(File.ReadAllBytes(Path.GetFullPath(Path.Combine(Application.dataPath, "..", wallPath))));

            var materialPath = SourceRoot + "/FacadeImportedMaterial.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            Assert.That(material, Is.Not.Null);
            Assert.That(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(material, out string materialGuid, out long materialFileId), Is.True);
            var materialHash = Hash(File.ReadAllBytes(Path.GetFullPath(Path.Combine(Application.dataPath, "..", materialPath))));

            var transform = new ProjectionComponent { schemaId = TransformSchema, kind = "transform" };
            var reboundMaterial = new ProjectionComponent
            {
                schemaId = RendererSchema,
                kind = "renderer",
                referenceKind = "material",
                referenceLogicalId = "street-corner.facade.material",
                referencePath = materialPath,
                referenceGuid = new string('0', materialGuid.Length),
                referenceLocalFileId = materialFileId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                referenceContentSha256 = materialHash
            };
            var plan = new ProjectionPlan
            {
                schemaId = "arkus.h1-managed-scene-plan@1",
                sceneLogicalId = SceneId,
                worldId = "world.potes.street-corner.h1-08",
                worldRevision = 1,
                canonicalHash = HashText("street-corner-canonical"),
                catalogueFingerprint = HashText("street-corner-catalogue"),
                inputDigest = HashText("street-corner-invalid-probe"),
                nodes = new[]
                {
                    Node("street-corner.facade", "", sourceGuid, sourceFileId, sourceHash, transform, reboundMaterial),
                    Node("street-corner.prop", "street-corner.missing-parent", sourceGuid, sourceFileId, sourceHash, transform)
                }
            };

            var reply = Execute(plan);
            Assert.That(reply.validation, Is.Not.Null);
            Assert.That(reply.validation.valid, Is.False);
            Assert.That(reply.validation.diagnostics.Select(value => value.code), Does.Contain("projection.component-reference-rebound"));
            Assert.That(reply.validation.diagnostics.Select(value => value.code), Does.Contain("projection.unbound-parent"));
            Assert.That(reply.validation.diagnostics.Single(value => value.code == "projection.component-reference-rebound").invariantId,
                Is.EqualTo("unity.plan.component"));
            Assert.That(reply.validation.diagnostics.Single(value => value.code == "projection.unbound-parent").invariantId,
                Is.EqualTo("unity.plan.hierarchy"));
        }

        private static ProjectionNode Node(string id, string parent, string guid, long fileId, string sha, params ProjectionComponent[] components) =>
            new ProjectionNode
            {
                objectId = id,
                parentObjectId = parent,
                sourceKind = "asset",
                sourceLogicalId = "quaternius.medieval.asset.wall-plaster-window-wide-flat",
                sourcePath = SourceRoot + "/Wall_Plaster_Window_Wide_Flat.fbx",
                sourceGuid = guid,
                sourceLocalFileId = fileId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                sourceContentSha256 = sha,
                positionMm = new ProjectionVector { x = 0, y = 0, z = 0 },
                rotationMilliDegrees = new ProjectionVector { x = 0, y = 0, z = 0 },
                scalePpm = new ProjectionVector { x = 1000000, y = 1000000, z = 1000000 },
                components = components
            };

        private static ProjectionReply Execute(ProjectionPlan plan)
        {
            var raw = H1SceneProjection.Execute(JsonUtility.ToJson(new ProjectionRequest
            {
                schemaId = "arkus.h1-projection-worker-request@1",
                mode = "validate-proposed",
                sceneLogicalId = SceneId,
                plan = plan
            }));
            return JsonUtility.FromJson<ProjectionReply>(raw);
        }

        private static string HashText(string value) => Hash(System.Text.Encoding.UTF8.GetBytes(value));
        private static string Hash(byte[] bytes)
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
        [Serializable] private sealed class ProjectionReply { public string errorCode; public H1ValidationResult validation; }
    }
}
