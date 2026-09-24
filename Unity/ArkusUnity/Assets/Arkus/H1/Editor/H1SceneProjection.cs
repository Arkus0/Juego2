using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Arkus.H1.Projection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor
{
    // All paths and the publication pointer are fixed by this bridge, never chosen by a request.
    // A scene is staged, saved, reloaded and observed before the one active pointer changes.
    public static class H1SceneProjection
    {
        private const string Root = "Assets/Arkus/H1/ManagedScenes";
        private const string Generations = Root + "/generations";
        private const string ManifestPath = Root + "/current.json";
        private const string SceneId = "arkus.h1-05.scene.potes";
        private const string ObservationSchema = "arkus.h1-managed-scene-observation@1";
        private const string ManifestSchema = "arkus.h1-managed-scene-manifest@1";

        public static string Execute(string payload)
        {
            var request = JsonUtility.FromJson<ProjectionRequest>(payload);
            if (request == null || request.schemaId != "arkus.h1-projection-worker-request@1" ||
                request.sceneLogicalId != SceneId || request.plan == null ||
                request.plan.schemaId != "arkus.h1-managed-scene-plan@1" ||
                request.plan.sceneLogicalId != SceneId || request.plan.nodes == null || request.plan.nodes.Length > 128 ||
                !IsHash(request.plan.inputDigest) || !IsHash(request.plan.canonicalHash) || !IsHash(request.plan.catalogueFingerprint))
                throw new InvalidDataException("projection.invalid-worker-plan");
            if (request.mode != "materialize" && request.mode != "observe")
                throw new InvalidDataException("projection.invalid-worker-mode");

            try
            {
                ValidateCatalogueSnapshot();
                ValidatePlan(request.plan);
                var observation = request.mode == "materialize"
                    ? Materialize(request.plan) : ObserveActive();
                return JsonUtility.ToJson(new ProjectionReply
                {
                    schemaId = "arkus.h1-projection-worker-result@1",
                    sceneLogicalId = SceneId,
                    expectedInputDigest = request.plan.inputDigest,
                    errorCode = "",
                    observation = observation
                });
            }
            catch (Exception exception)
            {
                Debug.LogError("ARKUS_H1_PROJECTION_FAILURE:" + exception.GetType().Name + ":" + exception.Message);
                // A failed stage never claims success. The active pointer is re-read after failure,
                // so a published effect cannot be falsely described as the old generation.
                ProjectionObservation active;
                try { active = ObserveActive(); }
                catch (Exception) { active = EmptyObservation(); }
                return JsonUtility.ToJson(new ProjectionReply
                {
                    schemaId = "arkus.h1-projection-worker-result@1",
                    sceneLogicalId = SceneId,
                    expectedInputDigest = request.plan.inputDigest,
                    errorCode = exception is InvalidDataException && exception.Message.StartsWith("projection.", StringComparison.Ordinal)
                        ? exception.Message : "projection.editor-failure",
                    observation = active
                });
            }
        }

        private static ProjectionObservation Materialize(ProjectionPlan plan)
        {
            var previous = ReadManifest();
            if (previous != null && previous.inputDigest == plan.inputDigest)
            {
                try
                {
                    var existing = ObserveManifest(previous);
                    if (SameGraph(plan, existing)) return existing; // no semantic or serialized churn
                }
                catch (InvalidDataException) { /* Rebuild a fresh generation from the unchanged canonical input. */ }
            }

            EnsureFolders();
            var generationId = Guid.NewGuid().ToString("N");
            var scenePath = Generations + "/" + generationId + ".unity";
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("Arkus Managed Root");
            SceneManager.MoveGameObjectToScene(root, scene);
            Mark(root, "root", "", "", generationId);

            var created = new Dictionary<string, GameObject>(StringComparer.Ordinal);
            foreach (var node in plan.nodes.OrderBy(value => value.objectId, StringComparer.Ordinal))
            {
                var source = ResolveSource(node);
                GameObject instance;
                if (node.sourceKind == "prefab")
                {
                    instance = PrefabUtility.InstantiatePrefab((GameObject)source, scene) as GameObject;
                    if (instance == null) throw new InvalidDataException("projection.prefab-instantiation-failed");
                }
                else
                {
                    instance = new GameObject(node.objectId);
                    SceneManager.MoveGameObjectToScene(instance, scene);
                    instance.AddComponent<MeshFilter>().sharedMesh = (Mesh)source;
                    instance.AddComponent<MeshRenderer>();
                }
                instance.name = node.objectId;
                Mark(instance, "object", node.objectId, node.sourceLogicalId, generationId);
                created.Add(node.objectId, instance);
            }
            foreach (var node in plan.nodes)
            {
                var instance = created[node.objectId];
                var parent = node.parentObjectId.Length == 0 ? root : created[node.parentObjectId];
                instance.transform.SetParent(parent.transform, false);
                instance.transform.localPosition = new Vector3(Unit(node.positionMm.x, 1000), Unit(node.positionMm.y, 1000), Unit(node.positionMm.z, 1000));
                instance.transform.localEulerAngles = new Vector3(Unit(node.rotationMilliDegrees.x, 1000), Unit(node.rotationMilliDegrees.y, 1000), Unit(node.rotationMilliDegrees.z, 1000));
                instance.transform.localScale = new Vector3(Unit(node.scalePpm.x, 1000000), Unit(node.scalePpm.y, 1000000), Unit(node.scalePpm.z, 1000000));
            }
            if (!EditorSceneManager.SaveScene(scene, scenePath, false))
                throw new IOException("projection.scene-save-failed");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var staged = ObserveScene(scenePath, generationId, plan.inputDigest, plan.canonicalHash, plan.catalogueFingerprint);
            if (!SameGraph(plan, staged)) throw new InvalidDataException("projection.stage-observation-mismatch");

            var fault = Path.Combine(H1Bootstrap.ProjectRoot(), "Library", "Arkus", "H1Projection", "fail-before-publish");
            if (File.Exists(fault)) throw new InvalidDataException("projection.forced-prepublication-failure");

            var manifest = new ProjectionManifest
            {
                schemaId = ManifestSchema, sceneLogicalId = SceneId, generationId = generationId,
                scenePath = scenePath, inputDigest = plan.inputDigest,
                canonicalHash = plan.canonicalHash, catalogueFingerprint = plan.catalogueFingerprint,
                graphDigest = staged.graphDigest
            };
            Publish(manifest);
            if (previous != null && previous.scenePath != scenePath)
            {
                try { AssetDatabase.DeleteAsset(previous.scenePath); }
                catch (Exception cleanupError) { Debug.LogWarning("ARKUS_H1_PROJECTION_CLEANUP:" + cleanupError.GetType().Name); }
            }
            return ObserveManifest(manifest);
        }

        private static void ValidatePlan(ProjectionPlan plan)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var node in plan.nodes)
            {
                if (node == null || string.IsNullOrEmpty(node.objectId) || !seen.Add(node.objectId) ||
                    string.IsNullOrEmpty(node.sourceLogicalId) || !IsHash(node.sourceContentSha256) ||
                    node.positionMm == null || node.rotationMilliDegrees == null || node.scalePpm == null ||
                    (node.sourceKind != "prefab" && node.sourceKind != "asset"))
                    throw new InvalidDataException("projection.invalid-node");
                // Validate all effective source inputs before any staged effect can publish.
                ResolveSource(node);
            }
            foreach (var node in plan.nodes)
                if (node.parentObjectId.Length != 0 && !seen.Contains(node.parentObjectId))
                    throw new InvalidDataException("projection.unbound-parent");
        }

        private static void ValidateCatalogueSnapshot()
        {
            var repositoryRoot = Path.GetFullPath(Path.Combine(H1Bootstrap.ProjectRoot(), "..", ".."));
            var expectedPath = Path.Combine(repositoryRoot, "Docs", "evidence", "WP-H1-04", "EFFECTIVE_INVENTORY.json");
            if (!File.Exists(expectedPath)) throw new InvalidDataException("projection.catalogue-snapshot-missing");
            var accepted = JsonUtility.FromJson<EffectiveInventory>(File.ReadAllText(expectedPath));
            if (accepted == null || accepted.schemaId != H1CatalogueInventory.Schema || accepted.rows == null)
                throw new InvalidDataException("projection.catalogue-snapshot-invalid");
            var effective = H1CatalogueInventory.Capture();
            if (JsonUtility.ToJson(accepted) != JsonUtility.ToJson(effective))
                throw new InvalidDataException("projection.catalogue-snapshot-stale");
        }

        private static UnityEngine.Object ResolveSource(ProjectionNode node)
        {
            if (node.sourcePath == null || !node.sourcePath.StartsWith(H1CatalogueInventory.SourceRoot + "/", StringComparison.Ordinal) ||
                node.sourcePath.Substring(H1CatalogueInventory.SourceRoot.Length + 1).Contains("/") ||
                AssetDatabase.AssetPathToGUID(node.sourcePath) != node.sourceGuid)
                throw new InvalidDataException("projection.source-locator-mismatch");
            var fullPath = Path.Combine(H1Bootstrap.ProjectRoot(), node.sourcePath);
            if (!File.Exists(fullPath) || Sha(File.ReadAllBytes(fullPath)) != node.sourceContentSha256)
                throw new InvalidDataException("projection.source-content-mismatch");
            foreach (var candidate in AssetDatabase.LoadAllAssetsAtPath(node.sourcePath))
            {
                if (candidate == null || !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(candidate, out string guid, out long fileId) ||
                    guid != node.sourceGuid || fileId.ToString(CultureInfo.InvariantCulture) != node.sourceLocalFileId) continue;
                if (node.sourceKind == "prefab" && candidate is GameObject) return candidate;
                if (node.sourceKind == "asset" && candidate is Mesh) return candidate;
            }
            throw new InvalidDataException("projection.source-native-identity-missing");
        }

        private static ProjectionObservation ObserveActive()
        {
            var manifest = ReadManifest();
            return manifest == null ? EmptyObservation() : ObserveManifest(manifest);
        }

        private static ProjectionObservation EmptyObservation() => new ProjectionObservation
        {
            schemaId = ObservationSchema, sceneLogicalId = SceneId, active = false,
            generationId = "", inputDigest = "", canonicalHash = "", catalogueFingerprint = "",
            graphDigest = "", nodes = new ProjectionObservedNode[0]
        };

        private static ProjectionObservation ObserveManifest(ProjectionManifest manifest)
        {
            var observed = ObserveScene(manifest.scenePath, manifest.generationId, manifest.inputDigest,
                manifest.canonicalHash, manifest.catalogueFingerprint);
            if (observed.graphDigest != manifest.graphDigest)
                throw new InvalidDataException("projection.active-scene-drift");
            return observed;
        }

        private static ProjectionObservation ObserveScene(string path, string generationId, string inputDigest,
            string canonicalHash, string catalogueFingerprint)
        {
            if (!File.Exists(Path.Combine(H1Bootstrap.ProjectRoot(), path)))
                throw new InvalidDataException("projection.active-scene-missing");
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            var roots = scene.GetRootGameObjects();
            if (roots.Length != 1) throw new InvalidDataException("projection.root-count");
            var root = roots[0].GetComponent<H1ManagedMarker>();
            if (root == null || root.schemaId != H1ManagedMarker.SchemaId || root.role != "root" || root.sceneLogicalId != SceneId ||
                root.generationId != generationId || root.canonicalObjectId != "")
                throw new InvalidDataException("projection.root-marker-invalid");
            var markers = roots[0].GetComponentsInChildren<H1ManagedMarker>(true);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var nodes = new List<ProjectionObservedNode>();
            foreach (var marker in markers)
            {
                if (marker == root) continue;
                if (marker.schemaId != H1ManagedMarker.SchemaId || marker.role != "object" ||
                    marker.sceneLogicalId != SceneId || marker.generationId != generationId ||
                    string.IsNullOrEmpty(marker.canonicalObjectId) ||
                    !seen.Add(marker.canonicalObjectId))
                    throw new InvalidDataException("projection.duplicate-or-invalid-marker");
                var parent = marker.transform.parent == null ? null : marker.transform.parent.GetComponent<H1ManagedMarker>();
                if (parent == null || (parent != root && parent.role != "object"))
                    throw new InvalidDataException("projection.unmanaged-parent");
                nodes.Add(new ProjectionObservedNode
                {
                    objectId = marker.canonicalObjectId,
                    parentObjectId = parent == root ? "" : parent.canonicalObjectId,
                    sourceLogicalId = marker.sourceLogicalId,
                    positionMm = Quantize(marker.transform.localPosition, 1000),
                    rotationMilliDegrees = Quantize(marker.transform.localEulerAngles, 1000),
                    scalePpm = Quantize(marker.transform.localScale, 1000000)
                });
            }
            nodes.Sort((left, right) => StringComparer.Ordinal.Compare(left.objectId, right.objectId));
            return new ProjectionObservation
            {
                schemaId = ObservationSchema, sceneLogicalId = SceneId, active = true,
                generationId = generationId, inputDigest = inputDigest, canonicalHash = canonicalHash,
                catalogueFingerprint = catalogueFingerprint,
                graphDigest = GraphDigest(nodes), nodes = nodes.ToArray()
            };
        }

        private static bool SameGraph(ProjectionPlan plan, ProjectionObservation observation)
        {
            if (!observation.active || observation.nodes.Length != plan.nodes.Length) return false;
            var expected = plan.nodes.OrderBy(node => node.objectId, StringComparer.Ordinal).ToArray();
            for (var index = 0; index < expected.Length; index++)
            {
                var left = expected[index];
                var right = observation.nodes[index];
                if (left.objectId != right.objectId || left.parentObjectId != right.parentObjectId ||
                    left.sourceLogicalId != right.sourceLogicalId ||
                    !Equal(left.positionMm, right.positionMm) || !Equal(left.scalePpm, right.scalePpm) ||
                    !EqualRotation(left.rotationMilliDegrees, right.rotationMilliDegrees)) return false;
            }
            return true;
        }

        private static bool Equal(ProjectionVector left, ProjectionVector right) =>
            left.x == right.x && left.y == right.y && left.z == right.z;

        private static bool EqualRotation(ProjectionVector left, ProjectionVector right) =>
            Normalize(left.x) == Normalize(right.x) && Normalize(left.y) == Normalize(right.y) && Normalize(left.z) == Normalize(right.z);

        private static long Normalize(long value) { var result = value % 360000; return result < 0 ? result + 360000 : result; }

        private static ProjectionVector Quantize(Vector3 value, long scale) => new ProjectionVector
        {
            x = (long)Math.Round(value.x * scale, MidpointRounding.AwayFromZero),
            y = (long)Math.Round(value.y * scale, MidpointRounding.AwayFromZero),
            z = (long)Math.Round(value.z * scale, MidpointRounding.AwayFromZero)
        };

        private static string GraphDigest(IEnumerable<ProjectionObservedNode> nodes)
        {
            var builder = new StringBuilder();
            foreach (var node in nodes)
                builder.Append(node.objectId).Append('|').Append(node.parentObjectId).Append('|').Append(node.sourceLogicalId).Append('|')
                    .Append(node.positionMm.x).Append(',').Append(node.positionMm.y).Append(',').Append(node.positionMm.z).Append('|')
                    .Append(Normalize(node.rotationMilliDegrees.x)).Append(',').Append(Normalize(node.rotationMilliDegrees.y)).Append(',').Append(Normalize(node.rotationMilliDegrees.z)).Append('|')
                    .Append(node.scalePpm.x).Append(',').Append(node.scalePpm.y).Append(',').Append(node.scalePpm.z).Append('\n');
            return Sha(Encoding.UTF8.GetBytes(builder.ToString()));
        }

        private static void Mark(GameObject target, string role, string objectId, string sourceId, string generationId)
        {
            var marker = target.AddComponent<H1ManagedMarker>();
            marker.role = role; marker.sceneLogicalId = SceneId; marker.generationId = generationId;
            marker.canonicalObjectId = objectId; marker.sourceLogicalId = sourceId;
        }

        private static ProjectionManifest ReadManifest()
        {
            var path = Path.Combine(H1Bootstrap.ProjectRoot(), ManifestPath);
            if (!File.Exists(path)) return null;
            var manifest = JsonUtility.FromJson<ProjectionManifest>(File.ReadAllText(path));
            if (manifest == null || manifest.schemaId != ManifestSchema || manifest.sceneLogicalId != SceneId ||
                !IsGenerationId(manifest.generationId) ||
                manifest.scenePath != Generations + "/" + manifest.generationId + ".unity" ||
                !IsHash(manifest.inputDigest) || !IsHash(manifest.canonicalHash) ||
                !IsHash(manifest.catalogueFingerprint) || !IsHash(manifest.graphDigest))
                throw new InvalidDataException("projection.manifest-invalid");
            return manifest;
        }

        private static void Publish(ProjectionManifest manifest)
        {
            var target = Path.Combine(H1Bootstrap.ProjectRoot(), ManifestPath);
            var temp = target + ".tmp-" + Guid.NewGuid().ToString("N");
            File.WriteAllText(temp, JsonUtility.ToJson(manifest), new UTF8Encoding(false));
            if (File.Exists(target)) File.Replace(temp, target, null);
            else File.Move(temp, target);
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder(Root)) AssetDatabase.CreateFolder("Assets/Arkus/H1", "ManagedScenes");
            if (!AssetDatabase.IsValidFolder(Generations)) AssetDatabase.CreateFolder(Root, "generations");
        }

        private static bool IsHash(string value) => value != null && value.Length == 64 && value.All(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f');
        private static bool IsGenerationId(string value) => value != null && value.Length == 32 && value.All(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f');
        private static float Unit(long value, long denominator) => (float)value / denominator;
        private static string Sha(byte[] bytes)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        [Serializable] private sealed class ProjectionRequest
        {
            public string schemaId;
            public string mode;
            public string sceneLogicalId;
            public ProjectionPlan plan;
        }
        [Serializable] private sealed class ProjectionReply
        {
            public string schemaId;
            public string sceneLogicalId;
            public string expectedInputDigest;
            public string errorCode;
            public ProjectionObservation observation;
        }
        [Serializable] private sealed class ProjectionPlan
        {
            public string schemaId;
            public string sceneLogicalId;
            public string worldId;
            public long worldRevision;
            public string canonicalHash;
            public string catalogueFingerprint;
            public string inputDigest;
            public ProjectionNode[] nodes;
        }
        [Serializable] private sealed class ProjectionNode
        {
            public string objectId;
            public string parentObjectId;
            public string sourceKind;
            public string sourceLogicalId;
            public string sourcePath;
            public string sourceGuid;
            public string sourceLocalFileId;
            public string sourceContentSha256;
            public ProjectionVector positionMm;
            public ProjectionVector rotationMilliDegrees;
            public ProjectionVector scalePpm;
        }
        [Serializable] private sealed class ProjectionVector { public long x; public long y; public long z; }
        [Serializable] private sealed class ProjectionManifest
        {
            public string schemaId;
            public string sceneLogicalId;
            public string generationId;
            public string scenePath;
            public string inputDigest;
            public string canonicalHash;
            public string catalogueFingerprint;
            public string graphDigest;
        }
        [Serializable] private sealed class ProjectionObservation
        {
            public string schemaId;
            public string sceneLogicalId;
            public bool active;
            public string generationId;
            public string inputDigest;
            public string canonicalHash;
            public string catalogueFingerprint;
            public string graphDigest;
            public ProjectionObservedNode[] nodes;
        }
        [Serializable] private sealed class ProjectionObservedNode
        {
            public string objectId;
            public string parentObjectId;
            public string sourceLogicalId;
            public ProjectionVector positionMm;
            public ProjectionVector rotationMilliDegrees;
            public ProjectionVector scalePpm;
        }
    }
}
