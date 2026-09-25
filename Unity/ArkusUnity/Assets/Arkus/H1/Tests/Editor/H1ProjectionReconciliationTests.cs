using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor.Tests
{
    public sealed class H1ProjectionReconciliationTests
    {
        private const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        private const string ManagedScenes = "Assets/Arkus/H1/ManagedScenes";
        private const string ManagedPrefabs = "Assets/Arkus/H1/ManagedPrefabs";
        private const string SceneId = "arkus.h1-05.scene.potes";
        private const string TransformSchema = "arkus.h1.component.transform@1";
        private const string RendererSchema = "arkus.h1.component.mesh-renderer@1";
        private const string AcceptedMaterialAGuid = "963743a1121475e92497a201e301bf48";
        private const string AcceptedMaterialBGuid = "75fb52ef5e0f0ad40a28c36d06ecd99c";
        private const long AcceptedMaterialLocalFileId = 2100000;

        [SetUp]
        public void SetUp()
        {
            DeleteGenerated();
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
        }

        [TearDown]
        public void TearDown() => DeleteGenerated();

        [Test]
        public void EffectiveScene_MixedManagedAndUnmanagedDrift_RemainsObservableWithoutStrictFalseGreen()
        {
            var materialA = AcceptedMaterial(AcceptedMaterialAGuid, AcceptedMaterialLocalFileId);
            var materialB = AcceptedMaterial(AcceptedMaterialBGuid, AcceptedMaterialLocalFileId);
            Assert.That(materialA.path, Is.Not.EqualTo(materialB.path), "the causal probe needs two independently accepted material identities");

            var plan = BuildPlan(materialA);
            var materialized = ExecuteStrict("materialize", plan);
            Assert.That(materialized.errorCode, Is.Empty);
            Assert.That(materialized.observation.active, Is.True);

            var root = GameObject.Find("Arkus Managed Root");
            var facade = GameObject.Find("facade.potes");
            var workshop = GameObject.Find("workshop.potes");
            Assert.That(root, Is.Not.Null);
            Assert.That(facade, Is.Not.Null);
            Assert.That(workshop, Is.Not.Null);

            facade.transform.localPosition += new Vector3(0.5f, 0f, 0f);
            var renderer = facade.GetComponent<MeshRenderer>();
            Assert.That(renderer, Is.Not.Null);
            renderer.sharedMaterial = materialB.asset;

            var extra = UnityEngine.Object.Instantiate(facade, root.transform, false);
            extra.name = "extra.facade";
            RewriteManagedObjectId(extra, "extra.facade");

            UnityEngine.Object.DestroyImmediate(workshop);
            var unmanaged = new GameObject("Loose Unmanaged");
            unmanaged.transform.SetParent(root.transform, false);

            Assert.That(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()), Is.True);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var reconciled = ExecuteReconciliation();
            Assert.That(reconciled.errorCode, Is.Empty);
            Assert.That(reconciled.observation, Is.Not.Null);
            Assert.That(reconciled.observation.active, Is.True);
            Assert.That(reconciled.observation.inputDigest, Is.EqualTo(plan.inputDigest));
            Assert.That(reconciled.observation.canonicalHash, Is.EqualTo(plan.canonicalHash));
            Assert.That(reconciled.observation.catalogueFingerprint, Is.EqualTo(plan.catalogueFingerprint));

            var ids = reconciled.observation.nodes.Select(value => value.objectId).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            Assert.That(ids, Is.EqualTo(new[] { "extra.facade", "facade.potes" }),
                "the effective observer must expose the added managed object and the missing workshop as an actual absence");

            var facadeObserved = reconciled.observation.nodes.Single(value => value.objectId == "facade.potes");
            Assert.That(facadeObserved.positionMm.x, Is.EqualTo(plan.nodes.Single(value => value.objectId == "facade.potes").positionMm.x + 500));
            Assert.That(facadeObserved.componentRows, Does.Contain(
                RendererSchema + "|enabled=true|material=" + materialB.path + "|" + materialB.guid + "|" + materialB.fileId));
            Assert.That(reconciled.observation.unmanagedPaths, Does.Contain("Loose Unmanaged"));
            Assert.That(reconciled.observation.diagnostics.Select(value => value.code), Does.Contain("projection.unmanaged-scene-member"));
            Assert.That(reconciled.observation.manifestGraphDigest, Is.Not.EqualTo(reconciled.observation.graphDigest));
        }

        private static ProjectionPlan BuildPlan(AssetIdentity material)
        {
            var wallPath = SourceRoot + "/Wall_Plaster_Window_Wide_Flat.fbx";
            var mesh = AssetDatabase.LoadAllAssetsAtPath(wallPath).OfType<Mesh>().FirstOrDefault();
            Assert.That(mesh, Is.Not.Null, "accepted facade FBX must expose a Mesh");
            var source = Identity(mesh, wallPath);
            var transform = new ProjectionComponent { schemaId = TransformSchema, kind = "transform" };
            var renderer = new ProjectionComponent
            {
                schemaId = RendererSchema,
                kind = "renderer",
                referenceKind = "material",
                referenceLogicalId = "quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster",
                referencePath = material.path,
                referenceGuid = material.guid,
                referenceLocalFileId = material.fileId,
                referenceContentSha256 = material.sha256
            };
            var facade = Node("facade.potes", source, Vec(1250, 0, -500), transform, renderer);
            var workshop = Node("workshop.potes", source, Vec(4000, 0, 1000), transform);
            return new ProjectionPlan
            {
                schemaId = "arkus.h1-managed-scene-plan@1",
                sceneLogicalId = SceneId,
                worldId = "world.potes.h1-09-effective-probe",
                worldRevision = 7,
                canonicalHash = HashText("h1-09-effective-canonical"),
                catalogueFingerprint = HashText("h1-09-effective-catalogue"),
                inputDigest = HashText("h1-09-effective-input"),
                nodes = new[] { facade, workshop }.OrderBy(value => value.objectId, StringComparer.Ordinal).ToArray()
            };
        }

        private static ProjectionNode Node(string id, AssetIdentity source, ProjectionVector position, params ProjectionComponent[] components) => new ProjectionNode
        {
            objectId = id,
            parentObjectId = "",
            sourceKind = "asset",
            sourceLogicalId = "quaternius.medieval.asset.wall-plaster-window-wide-flat",
            sourcePath = source.path,
            sourceGuid = source.guid,
            sourceLocalFileId = source.fileId,
            sourceContentSha256 = source.sha256,
            positionMm = position,
            rotationMilliDegrees = Vec(0, 0, 0),
            scalePpm = Vec(1000000, 1000000, 1000000),
            components = components.OrderBy(value => value.schemaId, StringComparer.Ordinal).ToArray()
        };

        private static ProjectionReply ExecuteStrict(string mode, ProjectionPlan plan)
        {
            var raw = H1SceneProjection.Execute(JsonUtility.ToJson(new ProjectionRequest
            {
                schemaId = "arkus.h1-projection-worker-request@1",
                mode = mode,
                sceneLogicalId = SceneId,
                plan = plan
            }));
            var reply = JsonUtility.FromJson<ProjectionReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            return reply;
        }

        private static ReconciliationReply ExecuteReconciliation()
        {
            var raw = H1SceneProjection.ExecuteReconciliation(JsonUtility.ToJson(new ReconciliationRequest
            {
                schemaId = "arkus.h1-projection-reconciliation-worker-request@1",
                sceneLogicalId = SceneId
            }));
            var reply = JsonUtility.FromJson<ReconciliationReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.schemaId, Is.EqualTo("arkus.h1-projection-reconciliation-worker-result@1"), raw);
            return reply;
        }

        private static void RewriteManagedObjectId(GameObject owner, string objectId)
        {
            var marker = owner.GetComponents<Component>().SingleOrDefault(component =>
                component != null && component.GetType().GetField("canonicalObjectId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null);
            Assert.That(marker, Is.Not.Null, "materialized object must carry the accepted H1 managed marker");
            var field = marker.GetType().GetField("canonicalObjectId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(marker, objectId);
        }

        private static AssetIdentity AcceptedMaterial(string guid, long localFileId)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Assert.That(path, Is.Not.Empty, "accepted material GUID must resolve in the mounted SourceSlice");
            var asset = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Material>().SingleOrDefault(value =>
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(value, out string candidateGuid, out long candidateFileId) &&
                string.Equals(candidateGuid, guid, StringComparison.Ordinal) && candidateFileId == localFileId);
            Assert.That(asset, Is.Not.Null, "accepted material GUID/local-file-id must resolve uniquely");
            return Identity(asset, path);
        }

        private static AssetIdentity Identity(UnityEngine.Object asset, string path)
        {
            Assert.That(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long fileId), Is.True, path);
            var full = Path.GetFullPath(Path.Combine(Application.dataPath, "..", path));
            return new AssetIdentity
            {
                asset = asset as Material,
                path = path,
                guid = guid,
                fileId = fileId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                sha256 = HashBytes(File.ReadAllBytes(full))
            };
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

        private sealed class AssetIdentity
        {
            public Material asset;
            public string path;
            public string guid;
            public string fileId;
            public string sha256;
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
        [Serializable] private sealed class ProjectionReply { public string errorCode; public ProjectionObservation observation; }
        [Serializable] private sealed class ProjectionObservation
        {
            public bool active; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
        }
        [Serializable] private sealed class ReconciliationRequest { public string schemaId; public string sceneLogicalId; }
        [Serializable] private sealed class ReconciliationReply { public string schemaId; public string sceneLogicalId; public string errorCode; public ReconciliationObservation observation; }
        [Serializable] private sealed class ReconciliationObservation
        {
            public bool active; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
            public string manifestGraphDigest; public string graphDigest; public ReconciliationNode[] nodes; public string[] unmanagedPaths; public ReconciliationDiagnostic[] diagnostics;
        }
        [Serializable] private sealed class ReconciliationNode { public string objectId; public ProjectionVector positionMm; public string[] componentRows; }
        [Serializable] private sealed class ReconciliationDiagnostic { public string code; public string subject; }
    }
}
