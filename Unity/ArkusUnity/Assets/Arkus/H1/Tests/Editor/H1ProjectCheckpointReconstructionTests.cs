using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Arkus.H1.Editor.Tests
{
    /// <summary>
    /// Effective Unity stages of the process-staged H1-10 proof. Each stage runs in its own Editor process.
    /// The canonical plan is passed to the accepted worker entry points byte-for-byte (never re-serialized by
    /// test-local types), and the raw worker replies are handed back to the .NET product decoder and parity
    /// verifier, which own the reconstruction-parity decision.
    /// </summary>
    public sealed class H1ProjectCheckpointReconstructionTests
    {
        private const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        private const string ManagedScenes = "Assets/Arkus/H1/ManagedScenes";
        private const string ManagedPrefabs = "Assets/Arkus/H1/ManagedPrefabs";
        private const string ManifestPath = ManagedScenes + "/current.json";
        private const string SceneId = "arkus.h1-05.scene.potes";

        [Test]
        public void StageA_MaterializeCheckpointBaselineInFreshEditorProcess()
        {
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
            DeleteGenerated();
            var rawPlan = ReadRawPlan("plan.json", out var plan);
            var materialized = Execute("materialize", rawPlan, out _);
            Assert.That(materialized.errorCode, Is.Empty);
            Assert.That(materialized.observation, Is.Not.Null);
            Assert.That(materialized.observation.active, Is.True);
            Assert.That(materialized.observation.inputDigest, Is.EqualTo(plan.inputDigest));
            Assert.That(materialized.observation.canonicalHash, Is.EqualTo(plan.canonicalHash));
            Assert.That(materialized.observation.catalogueFingerprint, Is.EqualTo(plan.catalogueFingerprint));

            var observed = Reconcile();
            RequireParityObservation(observed, plan);
            var baseline = new BaselineEvidence
            {
                schemaId = "arkus.h1-10-baseline@1",
                inputDigest = observed.observation.inputDigest,
                canonicalHash = observed.observation.canonicalHash,
                catalogueFingerprint = observed.observation.catalogueFingerprint,
                graphDigest = observed.observation.graphDigest,
                realizationDigest = observed.observation.realizationDigest
            };
            File.WriteAllText(Path.Combine(ProofDirectory(), "baseline.json"), JsonUtility.ToJson(baseline));
            WriteObservation(rawPlan, "baseline-observation.json");
        }

        [Test]
        public void StageB_CleanGeneratedOutputRebuildMatchesCheckpointInSecondEditorProcess()
        {
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
            AssetDatabase.Refresh();
            Assert.That(AssetDatabase.IsValidFolder(ManagedScenes), Is.False, "workflow must remove all generated scenes before fresh-process rebuild");
            Assert.That(AssetDatabase.IsValidFolder(ManagedPrefabs), Is.False, "workflow must remove all generated prefabs before fresh-process rebuild");

            var rawPlan = ReadRawPlan("restored-plan.json", out var plan);
            var baseline = ReadBaseline();
            Assert.That(plan.inputDigest, Is.EqualTo(baseline.inputDigest));
            Assert.That(plan.canonicalHash, Is.EqualTo(baseline.canonicalHash));
            Assert.That(plan.catalogueFingerprint, Is.EqualTo(baseline.catalogueFingerprint));

            // The public clean-rebuild worker entry point (H1EditorWorker executor clean-rebuild@1).
            var rebuiltRaw = H1SceneProjection.ExecuteCleanRebuild(Request("clean-rebuild", rawPlan));
            var rebuilt = JsonUtility.FromJson<ProjectionReply>(rebuiltRaw);
            Assert.That(rebuilt, Is.Not.Null, rebuiltRaw);
            Assert.That(rebuilt.errorCode, Is.Empty, rebuiltRaw);
            Assert.That(rebuilt.observation, Is.Not.Null, rebuiltRaw);
            Assert.That(rebuilt.observation.active, Is.True, rebuiltRaw);
            Assert.That(rebuilt.observation.inputDigest, Is.EqualTo(baseline.inputDigest));
            Assert.That(rebuilt.observation.canonicalHash, Is.EqualTo(baseline.canonicalHash));
            Assert.That(rebuilt.observation.catalogueFingerprint, Is.EqualTo(baseline.catalogueFingerprint));

            var observed = Reconcile();
            RequireParityObservation(observed, plan);
            Assert.That(observed.observation.graphDigest, Is.EqualTo(baseline.graphDigest));
            // The accepted realization digest embeds generation-local derivative-prefab locators, which a clean
            // rebuild must regenerate. Normalized reconstruction parity is decided by the .NET product verifier
            // (H1ReconstructionParity) over the raw observation written below.
            WriteObservation(rawPlan, "rebuilt-observation.json");
            File.WriteAllText(Path.Combine(ProofDirectory(), "final.json"), JsonUtility.ToJson(new BaselineEvidence
            {
                schemaId = "arkus.h1-10-final@1",
                inputDigest = observed.observation.inputDigest,
                canonicalHash = observed.observation.canonicalHash,
                catalogueFingerprint = observed.observation.catalogueFingerprint,
                graphDigest = observed.observation.graphDigest,
                realizationDigest = observed.observation.realizationDigest
            }));
        }

        [Test]
        public void Negative_PrepublicationFailureDoesNotPublishGeneratedCurrent()
        {
            Assert.That(AssetDatabase.IsValidFolder(SourceRoot), Is.True, "accepted H1 SourceSlice must be mounted");
            DeleteGenerated();
            var rawPlan = ReadRawPlan("plan.json", out _);
            var fault = Path.Combine(ProjectRoot(), "Library", "Arkus", "H1Projection", "fail-before-publish");
            Directory.CreateDirectory(Path.GetDirectoryName(fault));
            File.WriteAllText(fault, "fail");
            try
            {
                var failed = Execute("materialize", rawPlan, out _);
                Assert.That(failed.errorCode, Is.EqualTo("projection.forced-prepublication-failure"));
                AssetDatabase.Refresh();
                Assert.That(File.Exists(ProjectPath(ManifestPath)), Is.False, "failed staged generation must never become current");
            }
            finally
            {
                if (File.Exists(fault)) File.Delete(fault);
                DeleteGenerated();
            }
        }

        private static string Request(string mode, string rawPlan) =>
            "{\"schemaId\":\"arkus.h1-projection-worker-request@1\",\"mode\":\"" + mode + "\",\"sceneLogicalId\":\"" + SceneId + "\",\"plan\":" + rawPlan + "}";

        private static ProjectionReply Execute(string mode, string rawPlan, out string raw)
        {
            raw = H1SceneProjection.Execute(Request(mode, rawPlan));
            var reply = JsonUtility.FromJson<ProjectionReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            return reply;
        }

        private static void WriteObservation(string rawPlan, string name)
        {
            // Exactly the worker reply the public unity.host.projection.observe executor decodes.
            var observed = Execute("observe", rawPlan, out var raw);
            Assert.That(observed.errorCode, Is.Empty, raw);
            Assert.That(observed.observation, Is.Not.Null, raw);
            Assert.That(observed.observation.active, Is.True, raw);
            File.WriteAllText(Path.Combine(ProofDirectory(), name), raw);
        }

        private static ReconciliationReply Reconcile()
        {
            var raw = H1SceneProjection.ExecuteReconciliation(JsonUtility.ToJson(new ReconciliationRequest
            {
                schemaId = "arkus.h1-projection-reconciliation-worker-request@1",
                sceneLogicalId = SceneId
            }));
            var reply = JsonUtility.FromJson<ReconciliationReply>(raw);
            Assert.That(reply, Is.Not.Null, raw);
            Assert.That(reply.errorCode, Is.Empty, raw);
            Assert.That(reply.observation, Is.Not.Null, raw);
            return reply;
        }

        private static void RequireParityObservation(ReconciliationReply reply, ProjectionPlan plan)
        {
            var observation = reply.observation;
            Assert.That(observation.active, Is.True);
            Assert.That(observation.inputDigest, Is.EqualTo(plan.inputDigest));
            Assert.That(observation.canonicalHash, Is.EqualTo(plan.canonicalHash));
            Assert.That(observation.catalogueFingerprint, Is.EqualTo(plan.catalogueFingerprint));
            Assert.That(observation.unmanagedPaths, Is.Empty);
            Assert.That(observation.diagnostics, Is.Empty);
            Assert.That(observation.graphDigest, Is.EqualTo(observation.manifestGraphDigest));
            Assert.That(observation.realizationDigest, Is.EqualTo(observation.manifestRealizationDigest));
            Assert.That(observation.nodes.Select(value => value.objectId).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                Is.EqualTo(plan.nodes.Select(value => value.objectId).OrderBy(value => value, StringComparer.Ordinal).ToArray()));
        }

        private static string ReadRawPlan(string name, out ProjectionPlan plan)
        {
            var path = Path.Combine(ProofDirectory(), name);
            Assert.That(File.Exists(path), Is.True, "H1-10 proof plan is missing: " + path);
            var raw = File.ReadAllText(path);
            plan = JsonUtility.FromJson<ProjectionPlan>(raw);
            Assert.That(plan, Is.Not.Null);
            Assert.That(plan.schemaId, Is.EqualTo("arkus.h1-managed-scene-plan@1"));
            Assert.That(plan.sceneLogicalId, Is.EqualTo(SceneId));
            Assert.That(plan.nodes, Is.Not.Null.And.Not.Empty);
            return raw;
        }

        private static BaselineEvidence ReadBaseline()
        {
            var path = Path.Combine(ProofDirectory(), "baseline.json");
            Assert.That(File.Exists(path), Is.True, "H1-10 baseline evidence is missing");
            var baseline = JsonUtility.FromJson<BaselineEvidence>(File.ReadAllText(path));
            Assert.That(baseline, Is.Not.Null);
            Assert.That(baseline.schemaId, Is.EqualTo("arkus.h1-10-baseline@1"));
            return baseline;
        }

        private static string ProofDirectory() => Path.Combine(ProjectRoot(), "H1-10-Reconstruction");
        private static string ProjectRoot() => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private static string ProjectPath(string assetPath) => Path.Combine(ProjectRoot(), assetPath.Replace('/', Path.DirectorySeparatorChar));

        private static void DeleteGenerated()
        {
            if (AssetDatabase.IsValidFolder(ManagedScenes)) AssetDatabase.DeleteAsset(ManagedScenes);
            if (AssetDatabase.IsValidFolder(ManagedPrefabs)) AssetDatabase.DeleteAsset(ManagedPrefabs);
            AssetDatabase.Refresh();
        }

        [Serializable] private sealed class ProjectionPlan
        {
            public string schemaId; public string sceneLogicalId; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
            public ProjectionNodeId[] nodes;
        }
        [Serializable] private sealed class ProjectionNodeId { public string objectId; }
        [Serializable] private sealed class ProjectionReply { public string errorCode; public ProjectionObservation observation; }
        [Serializable] private sealed class ProjectionObservation
        {
            public bool active; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
        }
        [Serializable] private sealed class ReconciliationRequest { public string schemaId; public string sceneLogicalId; }
        [Serializable] private sealed class ReconciliationReply { public string errorCode; public ReconciliationObservation observation; }
        [Serializable] private sealed class ReconciliationObservation
        {
            public bool active; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
            public string manifestGraphDigest; public string manifestRealizationDigest; public string graphDigest; public string realizationDigest;
            public ReconciliationNode[] nodes; public string[] unmanagedPaths; public ReconciliationDiagnostic[] diagnostics;
        }
        [Serializable] private sealed class ReconciliationNode { public string objectId; }
        [Serializable] private sealed class ReconciliationDiagnostic { public string code; public string subject; }
        [Serializable] private sealed class BaselineEvidence
        {
            public string schemaId; public string inputDigest; public string canonicalHash; public string catalogueFingerprint;
            public string graphDigest; public string realizationDigest;
        }
    }
}
