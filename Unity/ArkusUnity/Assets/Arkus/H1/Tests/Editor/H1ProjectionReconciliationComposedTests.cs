using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor.Tests
{
    public sealed class H1ProjectionReconciliationComposedTests
    {
        private const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        private const string ManagedScenes = "Assets/Arkus/H1/ManagedScenes";
        private const string ManagedPrefabs = "Assets/Arkus/H1/ManagedPrefabs";
        private const string SceneId = "arkus.h1-05.scene.potes";

        [SetUp]
        public void SetUp()
        {
            DeleteGenerated();
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
        }

        [TearDown]
        public void TearDown() => DeleteGenerated();

        [Test]
        public void Stage1_MaterializeCanonicalPlan_EditEffectiveUnity_EmitPublicDriftReply()
        {
            var plan = ReadPlan("initial-plan.json");
            var materialized = ExecuteStrict("materialize", plan);
            Assert.That(materialized.errorCode, Is.Empty);
            Assert.That(materialized.observation.active, Is.True);

            var facade = GameObject.Find("facade");
            Assert.That(facade, Is.Not.Null, "seeded canonical plan must materialize facade");
            var before = plan.nodes.Single(value => value.objectId == "facade").positionMm.x;
            facade.transform.localPosition += new Vector3(0.375f, 0f, 0f);
            Assert.That(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()), Is.True);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var raw = ExecuteReconciliationRaw();
            var reply = JsonUtility.FromJson<ReconciliationReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.errorCode, Is.Empty, raw);
            Assert.That(reply.observation, Is.Not.Null, raw);
            Assert.That(reply.observation.active, Is.True);
            Assert.That(reply.observation.diagnostics, Is.Empty, "supported transform edit must remain importable, not ambiguous");
            var observedFacade = reply.observation.nodes.Single(value => value.objectId == "facade");
            Assert.That(observedFacade.positionMm.x, Is.EqualTo(before + 375));

            File.WriteAllText(Path.Combine(CompositionDirectory(), "drift-worker-reply.json"), raw);
        }

        [Test]
        public void Stage2_RematerializeAppliedH0Plan_EmitFinalEffectiveReply()
        {
            var plan = ReadPlan("applied-plan.json");
            var materialized = ExecuteStrict("materialize", plan);
            Assert.That(materialized.errorCode, Is.Empty);
            Assert.That(materialized.observation.active, Is.True);
            Assert.That(materialized.observation.inputDigest, Is.EqualTo(plan.inputDigest));
            Assert.That(materialized.observation.canonicalHash, Is.EqualTo(plan.canonicalHash));

            var raw = ExecuteReconciliationRaw();
            var reply = JsonUtility.FromJson<ReconciliationReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.errorCode, Is.Empty, raw);
            Assert.That(reply.observation, Is.Not.Null, raw);
            Assert.That(reply.observation.active, Is.True);
            Assert.That(reply.observation.inputDigest, Is.EqualTo(plan.inputDigest));
            Assert.That(reply.observation.canonicalHash, Is.EqualTo(plan.canonicalHash));
            Assert.That(reply.observation.catalogueFingerprint, Is.EqualTo(plan.catalogueFingerprint));
            Assert.That(reply.observation.unmanagedPaths, Is.Empty);
            Assert.That(reply.observation.diagnostics, Is.Empty);
            Assert.That(reply.observation.nodes.Select(value => value.objectId).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                Is.EqualTo(plan.nodes.Select(value => value.objectId).OrderBy(value => value, StringComparer.Ordinal).ToArray()));
            Assert.That(reply.observation.graphDigest, Is.EqualTo(reply.observation.manifestGraphDigest));
            Assert.That(reply.observation.realizationDigest, Is.EqualTo(reply.observation.manifestRealizationDigest));

            File.WriteAllText(Path.Combine(CompositionDirectory(), "final-worker-reply.json"), raw);
        }

        private static ProjectionPlan ReadPlan(string fileName)
        {
            var path = Path.Combine(CompositionDirectory(), fileName);
            Assert.That(File.Exists(path), Is.True, "composed proof input is missing: " + path);
            var raw = File.ReadAllText(path);
            var plan = JsonUtility.FromJson<ProjectionPlan>(raw);
            Assert.That(plan, Is.Not.Null, raw);
            Assert.That(plan.schemaId, Is.EqualTo("arkus.h1-managed-scene-plan@1"));
            Assert.That(plan.sceneLogicalId, Is.EqualTo(SceneId));
            Assert.That(plan.nodes, Is.Not.Null);
            return plan;
        }

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

        private static string ExecuteReconciliationRaw()
        {
            return H1SceneProjection.ExecuteReconciliation(JsonUtility.ToJson(new ReconciliationRequest
            {
                schemaId = "arkus.h1-projection-reconciliation-worker-request@1",
                sceneLogicalId = SceneId
            }));
        }

        private static string CompositionDirectory()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "H1-09-Composition"));
        }

        private static void DeleteGenerated()
        {
            if (AssetDatabase.IsValidFolder(ManagedScenes)) AssetDatabase.DeleteAsset(ManagedScenes);
            if (AssetDatabase.IsValidFolder(ManagedPrefabs)) AssetDatabase.DeleteAsset(ManagedPrefabs);
            AssetDatabase.Refresh();
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
        [Serializable] private sealed class ProjectionObservation { public bool active; public string inputDigest; public string canonicalHash; public string catalogueFingerprint; }
        [Serializable] private sealed class ReconciliationRequest { public string schemaId; public string sceneLogicalId; }
        [Serializable] private sealed class ReconciliationReply { public string schemaId; public string sceneLogicalId; public string errorCode; public ReconciliationObservation observation; }
        [Serializable] private sealed class ReconciliationObservation
        {
            public bool active; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
            public string manifestGraphDigest; public string manifestRealizationDigest; public string graphDigest; public string realizationDigest;
            public ReconciliationNode[] nodes; public string[] unmanagedPaths; public ReconciliationDiagnostic[] diagnostics;
        }
        [Serializable] private sealed class ReconciliationNode { public string objectId; public ProjectionVector positionMm; public string[] componentRows; }
        [Serializable] private sealed class ReconciliationDiagnostic { public string code; public string subject; }
    }
}
