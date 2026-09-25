using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.H1.Projection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor
{
    public static partial class H1SceneProjection
    {
        public static string ExecuteReconciliation(string payload)
        {
            ReconciliationRequest request;
            try
            {
                request = JsonUtility.FromJson<ReconciliationRequest>(payload);
            }
            catch (Exception)
            {
                request = null;
            }
            if (request == null || request.schemaId != "arkus.h1-projection-reconciliation-worker-request@1" || request.sceneLogicalId != SceneId)
                return ReconciliationFailure("projection.invalid-reconciliation-request");

            try
            {
                ProjectionManifest manifest;
                try
                {
                    manifest = ReadManifest();
                }
                catch (InvalidDataException exception)
                {
                    return JsonUtility.ToJson(new ReconciliationReply
                    {
                        schemaId = "arkus.h1-projection-reconciliation-worker-result@1",
                        sceneLogicalId = SceneId,
                        errorCode = "",
                        observation = EmptyReconciliationObservation(new ReconciliationDiagnostic
                        {
                            code = CanonicalProjectionCode(exception, "projection.manifest-invalid"),
                            subject = "$scene"
                        })
                    });
                }

                var observation = manifest == null
                    ? EmptyReconciliationObservation()
                    : ObserveReconciliation(manifest);
                return JsonUtility.ToJson(new ReconciliationReply
                {
                    schemaId = "arkus.h1-projection-reconciliation-worker-result@1",
                    sceneLogicalId = SceneId,
                    errorCode = "",
                    observation = observation
                });
            }
            catch (Exception exception)
            {
                Debug.LogError("ARKUS_H1_RECONCILIATION_FAILURE:" + exception.GetType().Name + ":" + exception.Message);
                return ReconciliationFailure(CanonicalProjectionCode(exception, "projection.reconciliation-editor-failure"));
            }
        }

        private static ReconciliationObservation ObserveReconciliation(ProjectionManifest manifest)
        {
            var diagnostics = new List<ReconciliationDiagnostic>();
            var unmanaged = new SortedSet<string>(StringComparer.Ordinal);
            var nodes = new List<ProjectionObservedNode>();
            var fullPath = Path.Combine(H1Bootstrap.ProjectRoot(), manifest.scenePath);
            if (!File.Exists(fullPath))
            {
                diagnostics.Add(Diagnostic("projection.active-scene-missing", "$scene"));
                return Observation(manifest, false, nodes, unmanaged, diagnostics);
            }

            var scene = EditorSceneManager.OpenScene(manifest.scenePath, OpenSceneMode.Single);
            var roots = scene.GetRootGameObjects();
            if (roots.Length != 1)
                diagnostics.Add(Diagnostic("projection.root-count", "$scene"));

            GameObject rootObject = null;
            foreach (var candidate in roots)
            {
                var marker = candidate.GetComponent<H1ManagedMarker>();
                if (marker == null || marker.role != "root" || marker.sceneLogicalId != SceneId) continue;
                if (rootObject != null)
                {
                    diagnostics.Add(Diagnostic("projection.root-marker-invalid", "$scene"));
                    rootObject = null;
                    break;
                }
                rootObject = candidate;
            }
            if (rootObject == null)
            {
                diagnostics.Add(Diagnostic("projection.root-marker-invalid", "$scene"));
                return Observation(manifest, true, nodes, unmanaged, diagnostics);
            }

            var root = rootObject.GetComponent<H1ManagedMarker>();
            if (root == null || root.schemaId != H1ManagedMarker.SchemaId || root.role != "root" || root.sceneLogicalId != SceneId ||
                root.generationId != manifest.generationId || root.canonicalObjectId != "" || root.sourceLogicalId != "")
                diagnostics.Add(Diagnostic("projection.root-marker-invalid", "$scene"));

            CollectUnmanagedReconciliationPaths(rootObject, manifest.generationId, unmanaged, diagnostics);

            var markers = rootObject.GetComponentsInChildren<H1ManagedMarker>(true);
            foreach (var marker in markers)
            {
                if (marker == root) continue;
                var objectId = marker == null || string.IsNullOrEmpty(marker.canonicalObjectId) ? "$unknown" : marker.canonicalObjectId;
                if (marker == null || marker.schemaId != H1ManagedMarker.SchemaId || marker.role != "object" || marker.sceneLogicalId != SceneId ||
                    marker.generationId != manifest.generationId || string.IsNullOrEmpty(marker.canonicalObjectId))
                {
                    diagnostics.Add(Diagnostic("projection.duplicate-or-invalid-marker", objectId));
                    continue;
                }

                var parentMarker = marker.transform.parent == null ? null : marker.transform.parent.GetComponent<H1ManagedMarker>();
                var parentObjectId = "";
                if (parentMarker == null)
                {
                    diagnostics.Add(Diagnostic("projection.unmanaged-parent", objectId));
                }
                else if (parentMarker != root)
                {
                    if (parentMarker.role != "object" || parentMarker.sceneLogicalId != SceneId || parentMarker.generationId != manifest.generationId ||
                        string.IsNullOrEmpty(parentMarker.canonicalObjectId))
                        diagnostics.Add(Diagnostic("projection.unmanaged-parent", objectId));
                    else
                        parentObjectId = parentMarker.canonicalObjectId;
                }

                var position = Quantize(marker.transform.localPosition, 1000);
                var rotation = Quantize(marker.transform.localEulerAngles, 1000);
                var scale = Quantize(marker.transform.localScale, 1000000);
                var row = new ProjectionObservedNode
                {
                    objectId = marker.canonicalObjectId,
                    parentObjectId = parentObjectId,
                    sourceLogicalId = marker.sourceLogicalId,
                    sourceKind = "",
                    sourcePath = "",
                    sourceGuid = "",
                    sourceLocalFileId = "",
                    sourceContentSha256 = "",
                    realizationKind = "",
                    realizedPath = "",
                    realizedGuid = "",
                    realizedLocalFileId = "",
                    prefabGenerationId = "",
                    relationshipDigest = "",
                    relationships = new PrefabRelationship[0],
                    componentDigest = "",
                    componentRows = new string[0],
                    positionMm = position,
                    rotationMilliDegrees = rotation,
                    scalePpm = scale
                };

                try
                {
                    var realization = ObserveRealization(marker.gameObject, marker.sourceLogicalId);
                    row.sourceKind = realization.sourceKind;
                    row.sourcePath = realization.sourcePath;
                    row.sourceGuid = realization.sourceGuid;
                    row.sourceLocalFileId = realization.sourceLocalFileId;
                    row.sourceContentSha256 = realization.sourceContentSha256;
                    row.realizationKind = realization.realizationKind;
                    row.realizedPath = realization.realizedPath;
                    row.realizedGuid = realization.realizedGuid;
                    row.realizedLocalFileId = realization.realizedLocalFileId;
                    row.prefabGenerationId = realization.prefabGenerationId;
                    row.relationshipDigest = realization.relationshipDigest;
                    row.relationships = realization.relationships ?? new PrefabRelationship[0];
                }
                catch (Exception exception)
                {
                    diagnostics.Add(Diagnostic(CanonicalProjectionCode(exception, "projection.reconciliation-node-unreadable"), objectId));
                    diagnostics.Add(Diagnostic("projection.reconciliation-node-unreadable", objectId));
                }

                try
                {
                    row.componentRows = ObserveComponentRows(marker.gameObject, position, rotation, scale);
                    row.componentDigest = ComponentDigest(row.componentRows);
                }
                catch (Exception exception)
                {
                    diagnostics.Add(Diagnostic(CanonicalProjectionCode(exception, "projection.reconciliation-node-unreadable"), objectId));
                    diagnostics.Add(Diagnostic("projection.reconciliation-node-unreadable", objectId));
                }
                nodes.Add(row);
            }

            nodes.Sort((left, right) =>
            {
                var byId = StringComparer.Ordinal.Compare(left.objectId, right.objectId);
                if (byId != 0) return byId;
                return StringComparer.Ordinal.Compare(left.parentObjectId, right.parentObjectId);
            });
            return Observation(manifest, true, nodes, unmanaged, diagnostics);
        }

        private static void CollectUnmanagedReconciliationPaths(
            GameObject root,
            string generationId,
            ISet<string> unmanaged,
            ICollection<ReconciliationDiagnostic> diagnostics)
        {
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                var candidate = transform.gameObject;
                if (candidate == root) continue;
                var marker = candidate.GetComponent<H1ManagedMarker>();
                if (marker != null) continue;

                var admittedPrefabChild = false;
                if (PrefabUtility.IsPartOfPrefabInstance(candidate) && !PrefabUtility.IsAddedGameObjectOverride(candidate))
                {
                    var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(candidate);
                    var owner = prefabRoot == null ? null : prefabRoot.GetComponent<H1ManagedMarker>();
                    admittedPrefabChild = owner != null && owner.schemaId == H1ManagedMarker.SchemaId && owner.role == "object" &&
                        owner.sceneLogicalId == SceneId && owner.generationId == generationId && !string.IsNullOrEmpty(owner.canonicalObjectId);
                }
                if (admittedPrefabChild) continue;

                var path = RelativePath(root.transform, transform);
                if (string.IsNullOrEmpty(path)) path = candidate.name;
                unmanaged.Add(path);
                diagnostics.Add(Diagnostic("projection.unmanaged-scene-member", path));
            }
        }

        private static ReconciliationObservation Observation(
            ProjectionManifest manifest,
            bool active,
            IList<ProjectionObservedNode> nodes,
            ISet<string> unmanaged,
            IList<ReconciliationDiagnostic> diagnostics)
        {
            string graphDigest = "";
            string realizationDigest = "";
            try { graphDigest = GraphDigest(nodes); }
            catch (Exception) { diagnostics.Add(Diagnostic("projection.reconciliation-graph-unreadable", "$scene")); }
            try { realizationDigest = RealizationDigest(nodes); }
            catch (Exception) { diagnostics.Add(Diagnostic("projection.reconciliation-realization-unreadable", "$scene")); }
            return new ReconciliationObservation
            {
                schemaId = "arkus.h1-projection-reconciliation-observation@1",
                sceneLogicalId = SceneId,
                active = active,
                generationId = manifest.generationId,
                inputDigest = manifest.inputDigest,
                canonicalHash = manifest.canonicalHash,
                catalogueFingerprint = manifest.catalogueFingerprint,
                manifestGraphDigest = manifest.graphDigest,
                manifestRealizationDigest = manifest.realizationDigest,
                graphDigest = graphDigest,
                realizationDigest = realizationDigest,
                managedDigest = "",
                nodes = nodes.ToArray(),
                unmanagedPaths = unmanaged.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                diagnostics = diagnostics
                    .GroupBy(value => value.code + "\u001f" + value.subject, StringComparer.Ordinal)
                    .Select(group => group.First())
                    .OrderBy(value => value.code, StringComparer.Ordinal)
                    .ThenBy(value => value.subject, StringComparer.Ordinal)
                    .ToArray()
            };
        }

        private static ReconciliationObservation EmptyReconciliationObservation(params ReconciliationDiagnostic[] diagnostics)
        {
            return new ReconciliationObservation
            {
                schemaId = "arkus.h1-projection-reconciliation-observation@1",
                sceneLogicalId = SceneId,
                active = false,
                generationId = "",
                inputDigest = "",
                canonicalHash = "",
                catalogueFingerprint = "",
                manifestGraphDigest = "",
                manifestRealizationDigest = "",
                graphDigest = "",
                realizationDigest = "",
                managedDigest = "",
                nodes = new ProjectionObservedNode[0],
                unmanagedPaths = new string[0],
                diagnostics = diagnostics ?? new ReconciliationDiagnostic[0]
            };
        }

        private static string ReconciliationFailure(string code)
        {
            return JsonUtility.ToJson(new ReconciliationReply
            {
                schemaId = "arkus.h1-projection-reconciliation-worker-result@1",
                sceneLogicalId = SceneId,
                errorCode = code,
                observation = EmptyReconciliationObservation()
            });
        }

        private static ReconciliationDiagnostic Diagnostic(string code, string subject) => new ReconciliationDiagnostic
        {
            code = code ?? "projection.reconciliation-node-unreadable",
            subject = subject ?? "$scene"
        };

        private static string CanonicalProjectionCode(Exception exception, string fallback)
        {
            var invalid = exception as InvalidDataException;
            return invalid != null && invalid.Message != null && invalid.Message.StartsWith("projection.", StringComparison.Ordinal)
                ? invalid.Message
                : fallback;
        }

        [Serializable]
        private sealed class ReconciliationRequest
        {
            public string schemaId;
            public string sceneLogicalId;
        }

        [Serializable]
        private sealed class ReconciliationReply
        {
            public string schemaId;
            public string sceneLogicalId;
            public string errorCode;
            public ReconciliationObservation observation;
        }

        [Serializable]
        private sealed class ReconciliationObservation
        {
            public string schemaId;
            public string sceneLogicalId;
            public bool active;
            public string generationId;
            public string inputDigest;
            public string canonicalHash;
            public string catalogueFingerprint;
            public string manifestGraphDigest;
            public string manifestRealizationDigest;
            public string graphDigest;
            public string realizationDigest;
            public string managedDigest;
            public ProjectionObservedNode[] nodes;
            public string[] unmanagedPaths;
            public ReconciliationDiagnostic[] diagnostics;
        }

        [Serializable]
        private sealed class ReconciliationDiagnostic
        {
            public string code;
            public string subject;
        }
    }
}
