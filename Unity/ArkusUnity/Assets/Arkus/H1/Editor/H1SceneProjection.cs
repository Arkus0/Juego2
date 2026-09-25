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
    public static partial class H1SceneProjection
    {
        private const string Root = "Assets/Arkus/H1/ManagedScenes";
        private const string Generations = Root + "/generations";
        private const string ManifestPath = Root + "/current.json";
        private const string PrefabRoot = "Assets/Arkus/H1/ManagedPrefabs";
        private const string PrefabGenerations = PrefabRoot + "/generations";
        private const string SceneId = "arkus.h1-05.scene.potes";
        private const string ObservationSchema = "arkus.h1-managed-scene-observation@1";
        private const string ManifestSchema = "arkus.h1-managed-scene-manifest@1";
        private const int MaximumRelationships = 512;
        private const int MaximumComponents = 16;

        private static string ExecuteLegacy(string payload)
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
                H1ComponentProjection.CaptureInventory();
                ValidatePlan(request.plan);
                var observation = request.mode == "materialize" ? Materialize(request.plan) : ObserveActive();
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

        private static ProjectionObservation Materialize(ProjectionPlan plan, out H1ValidationResult postflight)
        {
            postflight = null;
            var previous = ReadManifest();
            if (previous != null && previous.inputDigest == plan.inputDigest)
            {
                try
                {
                    var existing = ObserveManifest(previous);
                    if (SameGraph(plan, existing) && SameRealization(plan, existing))
                    {
                        ProjectionObservation verified;
                        postflight = RunPostflight(plan, previous.scenePath, previous.generationId, existing, true, out verified);
                        if (!postflight.valid) throw new H1ValidationFailureException(postflight);
                        return verified;
                    }
                }
                catch (InvalidDataException) { }
            }

            EnsureFolders();
            var generationId = Guid.NewGuid().ToString("N");
            var prefabGenerationId = PrefabGenerationId(plan);
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
                    var derivative = RealizeDerivative(plan, node, (GameObject)source, prefabGenerationId);
                    instance = PrefabUtility.InstantiatePrefab(derivative, scene) as GameObject;
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
                H1ComponentProjection.ApplyTransform(instance,
                    new Vector3(Unit(node.positionMm.x, 1000), Unit(node.positionMm.y, 1000), Unit(node.positionMm.z, 1000)),
                    new Vector3(Unit(Normalize(node.rotationMilliDegrees.x), 1000), Unit(Normalize(node.rotationMilliDegrees.y), 1000), Unit(Normalize(node.rotationMilliDegrees.z), 1000)),
                    new Vector3(Unit(node.scalePpm.x, 1000000), Unit(node.scalePpm.y, 1000000), Unit(node.scalePpm.z, 1000000)));
            }
            foreach (var node in plan.nodes)
                ApplyComponents(node, created[node.objectId], created);

            InjectNonFiniteTransformForH108(created);
            if (!EditorSceneManager.SaveScene(scene, scenePath, false)) throw new IOException("projection.scene-save-failed");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ProjectionObservation staged;
            postflight = RunPostflight(plan, scenePath, generationId, null, true, out staged);
            if (!postflight.valid) throw new H1ValidationFailureException(postflight);

            var fault = Path.Combine(H1Bootstrap.ProjectRoot(), "Library", "Arkus", "H1Projection", "fail-before-publish");
            if (File.Exists(fault)) throw new InvalidDataException("projection.forced-prepublication-failure");

            var manifest = new ProjectionManifest
            {
                schemaId = ManifestSchema, sceneLogicalId = SceneId, generationId = generationId,
                scenePath = scenePath, inputDigest = plan.inputDigest,
                canonicalHash = plan.canonicalHash, catalogueFingerprint = plan.catalogueFingerprint,
                graphDigest = staged.graphDigest, realizationDigest = staged.realizationDigest
            };
            Publish(manifest);
            if (previous != null && previous.scenePath != scenePath)
            {
                try { AssetDatabase.DeleteAsset(previous.scenePath); }
                catch (Exception cleanupError) { Debug.LogWarning("ARKUS_H1_PROJECTION_CLEANUP:" + cleanupError.GetType().Name); }
            }
            CleanupPrefabGenerations(prefabGenerationId, plan.nodes.Any(node => node.sourceKind == "prefab"));
            AssetDatabase.SaveAssets();
            ValidateSourceBytes(plan);
            return ObserveManifest(manifest);
        }

        private static void ApplyComponents(ProjectionNode node, GameObject instance, IReadOnlyDictionary<string, GameObject> created)
        {
            foreach (var component in node.components)
            {
                H1ComponentProjection.ValidateSchema(component.schemaId);
                if (component.schemaId == H1ComponentProjection.TransformSchema) continue;
                if (component.schemaId == H1ComponentProjection.MeshRendererSchema)
                {
                    var ownership = Ownership(instance);
                    ownership.rendererRelativePath = H1ComponentProjection.ApplyRenderer(instance, component.referencePath, component.referenceGuid,
                        component.referenceLocalFileId, component.referenceContentSha256);
                    ownership.rendererMaterial = true;
                }
                else if (component.schemaId == H1ComponentProjection.AnimatorSchema)
                {
                    var ownership = Ownership(instance);
                    ownership.animatorRelativePath = H1ComponentProjection.ApplyAnimator(instance, node.objectId, component.referencePath, component.referenceGuid,
                        component.referenceLocalFileId, component.referenceContentSha256);
                    ownership.animatorClip = true;
                }
                else if (component.schemaId == H1ComponentProjection.CanonicalLinkSchema)
                {
                    if (!created.TryGetValue(component.targetObjectId, out var target))
                        throw new InvalidDataException("projection.component-reference-unresolved");
                    H1ComponentProjection.ApplyCanonicalLink(instance, component.relation, component.targetObjectId, target);
                }
                else throw new InvalidDataException("projection.component-schema-unsupported");
            }
        }

        private static H1ComponentOwnershipMarker Ownership(GameObject owner)
        {
            var marker = owner.GetComponent<H1ComponentOwnershipMarker>();
            if (marker == null) marker = owner.AddComponent<H1ComponentOwnershipMarker>();
            marker.schemaId = H1ComponentOwnershipMarker.SchemaId;
            return marker;
        }

        private static GameObject RealizeDerivative(ProjectionPlan plan, ProjectionNode node, GameObject source, string prefabGenerationId)
        {
            EnsurePrefabGenerationFolder(prefabGenerationId);
            var derivativePath = DerivativePath(plan, node);
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(derivativePath);
            if (existing != null)
            {
                try { ValidateDerivativeAsset(existing, derivativePath, node, source, prefabGenerationId); return existing; }
                catch (InvalidDataException)
                {
                    if (!AssetDatabase.DeleteAsset(derivativePath)) throw new InvalidDataException("projection.prefab-derivative-replace-failed");
                }
            }
            var before = SourceHash(node);
            var instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
            if (instance == null) throw new InvalidDataException("projection.prefab-instantiation-failed");
            try
            {
                var lineage = instance.AddComponent<H1ManagedPrefabLineage>();
                lineage.prefabGenerationId = prefabGenerationId;
                lineage.sourceLogicalId = node.sourceLogicalId;
                lineage.sourcePath = node.sourcePath;
                lineage.sourceGuid = node.sourceGuid;
                lineage.sourceLocalFileId = node.sourceLocalFileId;
                lineage.sourceContentSha256 = node.sourceContentSha256;
                bool saved;
                var derivative = PrefabUtility.SaveAsPrefabAsset(instance, derivativePath, out saved);
                if (!saved || derivative == null) throw new InvalidDataException("projection.prefab-derivative-save-failed");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (before != SourceHash(node)) throw new InvalidDataException("projection.source-mutated");
            var created = AssetDatabase.LoadAssetAtPath<GameObject>(derivativePath);
            if (created == null) throw new InvalidDataException("projection.prefab-derivative-missing");
            ValidateDerivativeAsset(created, derivativePath, node, source, prefabGenerationId);
            return created;
        }

        private static void ValidateDerivativeAsset(GameObject derivative, string derivativePath, ProjectionNode node, GameObject source, string prefabGenerationId)
        {
            var expectedPrefix = PrefabGenerations + "/" + prefabGenerationId + "/";
            if (!derivativePath.StartsWith(expectedPrefix, StringComparison.Ordinal) || derivativePath.Substring(expectedPrefix.Length).Contains("/"))
                throw new InvalidDataException("projection.prefab-derivative-scope-escape");
            if (PrefabUtility.GetPrefabAssetType(derivative) != PrefabAssetType.Variant)
                throw new InvalidDataException("projection.prefab-derivative-not-variant");
            var lineage = derivative.GetComponent<H1ManagedPrefabLineage>();
            if (lineage == null || lineage.schemaId != H1ManagedPrefabLineage.SchemaId || lineage.prefabGenerationId != prefabGenerationId ||
                lineage.sourceLogicalId != node.sourceLogicalId || lineage.sourcePath != node.sourcePath || lineage.sourceGuid != node.sourceGuid ||
                lineage.sourceLocalFileId != node.sourceLocalFileId || lineage.sourceContentSha256 != node.sourceContentSha256)
                throw new InvalidDataException("projection.prefab-lineage-invalid");
            if (!AssetDatabase.GetDependencies(derivativePath, true).Contains(node.sourcePath, StringComparer.Ordinal))
                throw new InvalidDataException("projection.prefab-source-lineage-missing");
            if (SourceHash(node) != node.sourceContentSha256) throw new InvalidDataException("projection.source-rebound");
            ValidateSourceRelationships(derivative, source, node.sourcePath);
        }

        private static void ValidatePlan(ProjectionPlan plan)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var node in plan.nodes)
            {
                if (node == null || string.IsNullOrEmpty(node.objectId) || !seen.Add(node.objectId) ||
                    string.IsNullOrEmpty(node.sourceLogicalId) || !IsHash(node.sourceContentSha256) ||
                    node.positionMm == null || node.rotationMilliDegrees == null || node.scalePpm == null ||
                    node.components == null || node.components.Length == 0 || node.components.Length > MaximumComponents ||
                    (node.sourceKind != "prefab" && node.sourceKind != "asset"))
                    throw new InvalidDataException("projection.invalid-node");
                ResolveSource(node);
                ValidateComponents(node);
            }
            foreach (var node in plan.nodes)
            {
                if (node.parentObjectId.Length != 0 && !seen.Contains(node.parentObjectId))
                    throw new InvalidDataException("projection.unbound-parent");
                foreach (var component in node.components)
                    if (component.schemaId == H1ComponentProjection.CanonicalLinkSchema && !seen.Contains(component.targetObjectId))
                        throw new InvalidDataException("projection.component-reference-unresolved");
            }
        }

        private static void ValidateComponents(ProjectionNode node)
        {
            var schemas = new HashSet<string>(StringComparer.Ordinal);
            foreach (var component in node.components)
            {
                if (component == null || string.IsNullOrEmpty(component.schemaId) || !schemas.Add(component.schemaId))
                    throw new InvalidDataException("projection.component-cardinality");
                H1ComponentProjection.ValidateSchema(component.schemaId);
                if (component.schemaId == H1ComponentProjection.TransformSchema)
                {
                    if (component.kind != "transform") throw new InvalidDataException("projection.component-schema-kind-mismatch");
                }
                else if (component.schemaId == H1ComponentProjection.MeshRendererSchema)
                    ValidateAssetComponent(component, "renderer", "material");
                else if (component.schemaId == H1ComponentProjection.AnimatorSchema)
                    ValidateAssetComponent(component, "animator", "animation-clip");
                else if (component.schemaId == H1ComponentProjection.CanonicalLinkSchema)
                {
                    if (component.kind != "canonical-link" || string.IsNullOrEmpty(component.relation) || string.IsNullOrEmpty(component.targetObjectId))
                        throw new InvalidDataException("projection.component-reference-invalid");
                }
            }
            if (!schemas.Contains(H1ComponentProjection.TransformSchema)) throw new InvalidDataException("projection.component-transform-missing");
        }

        private static void ValidateAssetComponent(ProjectionComponent component, string kind, string referenceKind)
        {
            if (component.kind != kind || component.referenceKind != referenceKind || string.IsNullOrEmpty(component.referenceLogicalId) ||
                string.IsNullOrEmpty(component.referencePath) || string.IsNullOrEmpty(component.referenceGuid) ||
                string.IsNullOrEmpty(component.referenceLocalFileId) || !IsHash(component.referenceContentSha256))
                throw new InvalidDataException("projection.component-reference-invalid");
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
            if (string.IsNullOrEmpty(node.sourcePath) || !node.sourcePath.StartsWith(H1CatalogueInventory.SourceRoot + "/", StringComparison.Ordinal) ||
                node.sourcePath.Substring(H1CatalogueInventory.SourceRoot.Length + 1).Contains("/"))
                throw new InvalidDataException("projection.source-locator-mismatch");
            var fullPath = Path.Combine(H1Bootstrap.ProjectRoot(), node.sourcePath);
            if (!File.Exists(fullPath)) throw new InvalidDataException("projection.source-missing");
            if (AssetDatabase.AssetPathToGUID(node.sourcePath) != node.sourceGuid || SourceHash(node) != node.sourceContentSha256)
                throw new InvalidDataException("projection.source-rebound");
            var matchingIdentity = false;
            foreach (var candidate in AssetDatabase.LoadAllAssetsAtPath(node.sourcePath))
            {
                if (candidate == null || !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(candidate, out string guid, out long fileId) ||
                    guid != node.sourceGuid || fileId.ToString(CultureInfo.InvariantCulture) != node.sourceLocalFileId) continue;
                matchingIdentity = true;
                if (node.sourceKind == "prefab" && candidate is GameObject) return candidate;
                if (node.sourceKind == "asset" && candidate is Mesh) return candidate;
            }
            if (matchingIdentity) throw new InvalidDataException("projection.source-wrong-type");
            throw new InvalidDataException("projection.source-rebound");
        }

        private static string SourceHash(ProjectionNode node)
        {
            var fullPath = Path.Combine(H1Bootstrap.ProjectRoot(), node.sourcePath);
            return File.Exists(fullPath) ? Sha(File.ReadAllBytes(fullPath)) : "";
        }

        private static void ValidateSourceBytes(ProjectionPlan plan)
        {
            foreach (var node in plan.nodes) if (SourceHash(node) != node.sourceContentSha256) throw new InvalidDataException("projection.source-mutated");
        }

        private static ProjectionObservation ObserveActive()
        {
            var manifest = ReadManifest();
            return manifest == null ? EmptyObservation() : ObserveManifest(manifest);
        }

        private static ProjectionObservation EmptyObservation() => new ProjectionObservation
        {
            schemaId = ObservationSchema, sceneLogicalId = SceneId, active = false, generationId = "", inputDigest = "",
            canonicalHash = "", catalogueFingerprint = "", graphDigest = "", realizationDigest = "", nodes = new ProjectionObservedNode[0]
        };

        private static ProjectionObservation ObserveManifest(ProjectionManifest manifest)
        {
            var observed = ObserveScene(manifest.scenePath, manifest.generationId, manifest.inputDigest, manifest.canonicalHash, manifest.catalogueFingerprint);
            if (observed.graphDigest != manifest.graphDigest) throw new InvalidDataException("projection.active-scene-drift");
            if (!string.IsNullOrEmpty(manifest.realizationDigest) && observed.realizationDigest != manifest.realizationDigest)
                throw new InvalidDataException("projection.active-prefab-drift");
            return observed;
        }

        private static ProjectionObservation ObserveScene(string path, string generationId, string inputDigest, string canonicalHash, string catalogueFingerprint)
        {
            if (!File.Exists(Path.Combine(H1Bootstrap.ProjectRoot(), path))) throw new InvalidDataException("projection.active-scene-missing");
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            var roots = scene.GetRootGameObjects();
            if (roots.Length != 1) throw new InvalidDataException("projection.root-count");
            var root = roots[0].GetComponent<H1ManagedMarker>();
            if (root == null || root.schemaId != H1ManagedMarker.SchemaId || root.role != "root" || root.sceneLogicalId != SceneId ||
                root.generationId != generationId || root.canonicalObjectId != "" || root.sourceLogicalId != "")
                throw new InvalidDataException("projection.root-marker-invalid");
            if (!Equal(Quantize(root.transform.localPosition, 1000), new ProjectionVector()) ||
                !Equal(Quantize(root.transform.localScale, 1000000), new ProjectionVector { x = 1000000, y = 1000000, z = 1000000 }) ||
                Quaternion.Angle(root.transform.localRotation, Quaternion.identity) > 0.02f)
                throw new InvalidDataException("projection.root-transform-drift");
            ValidateEffectiveMembership(roots[0], generationId);
            var markers = roots[0].GetComponentsInChildren<H1ManagedMarker>(true);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var nodes = new List<ProjectionObservedNode>();
            foreach (var marker in markers)
            {
                if (marker == root) continue;
                if (marker.schemaId != H1ManagedMarker.SchemaId || marker.role != "object" || marker.sceneLogicalId != SceneId ||
                    marker.generationId != generationId || string.IsNullOrEmpty(marker.canonicalObjectId) || !seen.Add(marker.canonicalObjectId))
                    throw new InvalidDataException("projection.duplicate-or-invalid-marker");
                var parent = marker.transform.parent == null ? null : marker.transform.parent.GetComponent<H1ManagedMarker>();
                if (parent == null || (parent != root && parent.role != "object")) throw new InvalidDataException("projection.unmanaged-parent");
                var realization = ObserveRealization(marker.gameObject, marker.sourceLogicalId);
                var position = Quantize(marker.transform.localPosition, 1000);
                var rotation = Quantize(marker.transform.localEulerAngles, 1000);
                var scale = Quantize(marker.transform.localScale, 1000000);
                var componentRows = ObserveComponentRows(marker.gameObject, position, rotation, scale);
                nodes.Add(new ProjectionObservedNode
                {
                    objectId = marker.canonicalObjectId, parentObjectId = parent == root ? "" : parent.canonicalObjectId,
                    sourceLogicalId = marker.sourceLogicalId, sourceKind = realization.sourceKind, sourcePath = realization.sourcePath,
                    sourceGuid = realization.sourceGuid, sourceLocalFileId = realization.sourceLocalFileId, sourceContentSha256 = realization.sourceContentSha256,
                    realizationKind = realization.realizationKind, realizedPath = realization.realizedPath, realizedGuid = realization.realizedGuid,
                    realizedLocalFileId = realization.realizedLocalFileId, prefabGenerationId = realization.prefabGenerationId,
                    relationshipDigest = realization.relationshipDigest, relationships = realization.relationships,
                    componentDigest = ComponentDigest(componentRows), componentRows = componentRows,
                    positionMm = position, rotationMilliDegrees = rotation, scalePpm = scale
                });
            }
            nodes.Sort((left, right) => StringComparer.Ordinal.Compare(left.objectId, right.objectId));
            return new ProjectionObservation
            {
                schemaId = ObservationSchema, sceneLogicalId = SceneId, active = true, generationId = generationId,
                inputDigest = inputDigest, canonicalHash = canonicalHash, catalogueFingerprint = catalogueFingerprint,
                graphDigest = GraphDigest(nodes), realizationDigest = RealizationDigest(nodes), nodes = nodes.ToArray()
            };
        }

        private static string[] ObserveComponentRows(GameObject owner, ProjectionVector position, ProjectionVector rotation, ProjectionVector scale)
        {
            var rows = new List<string>
            {
                TransformRow(position, rotation, scale)
            };
            var ownership = owner.GetComponent<H1ComponentOwnershipMarker>();
            if (ownership != null)
            {
                if (ownership.schemaId != H1ComponentOwnershipMarker.SchemaId) throw new InvalidDataException("projection.component-ownership-invalid");
                if (ownership.rendererMaterial) rows.Add(H1ComponentProjection.ObserveReference(owner, H1ComponentProjection.MeshRendererSchema));
                if (ownership.animatorClip) rows.Add(H1ComponentProjection.ObserveReference(owner, H1ComponentProjection.AnimatorSchema));
            }
            if (owner.GetComponent<H1CanonicalLinkMarker>() != null)
                rows.Add(H1ComponentProjection.ObserveReference(owner, H1ComponentProjection.CanonicalLinkSchema));
            rows.Sort(StringComparer.Ordinal);
            return rows.ToArray();
        }

        private static RealizationObservation ObserveRealization(GameObject owner, string sourceLogicalId)
        {
            var lineage = owner.GetComponent<H1ManagedPrefabLineage>();
            if (lineage != null)
            {
                if (lineage.schemaId != H1ManagedPrefabLineage.SchemaId || lineage.sourceLogicalId != sourceLogicalId ||
                    !IsGenerationId(lineage.prefabGenerationId) || !IsHash(lineage.sourceContentSha256))
                    throw new InvalidDataException("projection.prefab-lineage-invalid");
                var derivativePath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(owner);
                var expectedPrefix = PrefabGenerations + "/" + lineage.prefabGenerationId + "/";
                if (string.IsNullOrEmpty(derivativePath) || !derivativePath.StartsWith(expectedPrefix, StringComparison.Ordinal) ||
                    derivativePath.Substring(expectedPrefix.Length).Contains("/")) throw new InvalidDataException("projection.prefab-derivative-scope-escape");
                var derivative = AssetDatabase.LoadAssetAtPath<GameObject>(derivativePath);
                if (derivative == null || PrefabUtility.GetPrefabAssetType(derivative) != PrefabAssetType.Variant)
                    throw new InvalidDataException("projection.prefab-derivative-not-variant");
                var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(owner);
                if (source == null || !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(source, out string sourceGuid, out long sourceFileId) ||
                    AssetDatabase.GetAssetPath(source) != lineage.sourcePath || sourceGuid != lineage.sourceGuid ||
                    sourceFileId.ToString(CultureInfo.InvariantCulture) != lineage.sourceLocalFileId || AssetDatabase.AssetPathToGUID(lineage.sourcePath) != lineage.sourceGuid)
                    throw new InvalidDataException("projection.prefab-source-lineage-missing");
                var fullSource = Path.Combine(H1Bootstrap.ProjectRoot(), lineage.sourcePath);
                if (!File.Exists(fullSource)) throw new InvalidDataException("projection.source-missing");
                if (Sha(File.ReadAllBytes(fullSource)) != lineage.sourceContentSha256) throw new InvalidDataException("projection.source-rebound");
                var sourceDependencies = AssetDatabase.GetDependencies(lineage.sourcePath, true);
                var derivativeDependencies = new HashSet<string>(AssetDatabase.GetDependencies(derivativePath, true), StringComparer.Ordinal);
                foreach (var dependency in sourceDependencies) if (!derivativeDependencies.Contains(dependency)) throw new InvalidDataException("projection.prefab-nested-lineage-missing");
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(derivative, out string derivativeGuid, out long derivativeFileId))
                    throw new InvalidDataException("projection.prefab-derivative-identity-missing");
                var relationships = CollectRelationships(owner, source, lineage.sourcePath);
                ValidateSourceRelationships(owner, source, lineage.sourcePath, relationships);
                return new RealizationObservation
                {
                    sourceKind = "prefab", sourcePath = lineage.sourcePath, sourceGuid = lineage.sourceGuid,
                    sourceLocalFileId = lineage.sourceLocalFileId, sourceContentSha256 = lineage.sourceContentSha256,
                    realizationKind = "managed-prefab-variant", realizedPath = derivativePath, realizedGuid = derivativeGuid,
                    realizedLocalFileId = derivativeFileId.ToString(CultureInfo.InvariantCulture), prefabGenerationId = lineage.prefabGenerationId,
                    relationshipDigest = RelationshipDigest(relationships), relationships = relationships
                };
            }
            var filter = owner.GetComponent<MeshFilter>();
            var mesh = filter == null ? null : filter.sharedMesh;
            if (mesh == null || !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mesh, out string guid, out long fileId))
                throw new InvalidDataException("projection.source-wrong-type");
            var path = AssetDatabase.GetAssetPath(mesh);
            var fullPath = Path.Combine(H1Bootstrap.ProjectRoot(), path);
            if (!File.Exists(fullPath)) throw new InvalidDataException("projection.source-missing");
            return new RealizationObservation
            {
                sourceKind = "asset", sourcePath = path, sourceGuid = guid, sourceLocalFileId = fileId.ToString(CultureInfo.InvariantCulture),
                sourceContentSha256 = Sha(File.ReadAllBytes(fullPath)), realizationKind = "source-asset", realizedPath = path, realizedGuid = guid,
                realizedLocalFileId = fileId.ToString(CultureInfo.InvariantCulture), prefabGenerationId = "",
                relationshipDigest = Sha(Encoding.UTF8.GetBytes("")), relationships = new PrefabRelationship[0]
            };
        }

        private static PrefabRelationship[] CollectRelationships(GameObject owner, GameObject originalSource, string originalSourcePath)
        {
            var rows = new List<PrefabRelationship>();
            AddRelationship(rows, "variant-base", "", originalSource);
            foreach (var transform in owner.GetComponentsInChildren<Transform>(true))
            {
                if (BelongsToNestedManagedObject(owner.transform, transform)) continue;
                var candidate = transform.gameObject;
                var relative = RelativePath(owner.transform, transform);
                if (candidate != owner && PrefabUtility.IsAnyPrefabInstanceRoot(candidate))
                {
                    var nested = PrefabUtility.GetCorrespondingObjectFromOriginalSource(candidate);
                    if (nested != null && AssetDatabase.GetAssetPath(nested) != originalSourcePath) AddRelationship(rows, "nested-prefab", relative, nested);
                }
                var filter = candidate.GetComponent<MeshFilter>();
                if (filter != null && filter.sharedMesh != null) AddRelationship(rows, "mesh-reference", relative, filter.sharedMesh);
                var renderer = candidate.GetComponent<MeshRenderer>();
                if (renderer != null) foreach (var material in renderer.sharedMaterials) if (material != null) AddRelationship(rows, "material-reference", relative, material);
                foreach (var clip in AnimationUtility.GetAnimationClips(candidate)) if (clip != null) AddRelationship(rows, "animation-clip-reference", relative, clip);
            }
            if (rows.Count > MaximumRelationships) throw new InvalidDataException("projection.prefab-relationship-limit");
            return NormalizeRelationships(rows);
        }

        private static bool BelongsToNestedManagedObject(Transform owner, Transform candidate)
        {
            for (var current = candidate; current != null && current != owner; current = current.parent)
                if (current.GetComponent<H1ManagedMarker>() != null) return true;
            return false;
        }

        internal static void ValidateSourceRelationships(GameObject realized, GameObject source, string sourcePath)
        {
            ValidateSourceRelationships(realized, source, sourcePath, CollectRelationships(realized, source, sourcePath));
        }

        private static void ValidateSourceRelationships(GameObject realized, GameObject source, string sourcePath, PrefabRelationship[] realizedRows)
        {
            var remaining = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var row in SourceDerivedRelationships(realizedRows).Where(row => !H1OwnedRelationship(realized, row)))
            {
                var key = RelationshipKey(row);
                remaining[key] = remaining.TryGetValue(key, out var count) ? count + 1 : 1;
            }
            foreach (var row in SourceDerivedRelationships(CollectRelationships(source, source, sourcePath)).Where(row => !H1OwnedRelationship(realized, row)))
            {
                var key = RelationshipKey(row);
                if (!remaining.TryGetValue(key, out var count) || count <= 0)
                    throw new InvalidDataException(row.kind == "nested-prefab" ? "projection.prefab-nested-lineage-missing" : "projection.prefab-source-reference-missing");
                if (count == 1) remaining.Remove(key); else remaining[key] = count - 1;
            }
            if (remaining.Count != 0) throw new InvalidDataException("projection.prefab-source-reference-unexpected");
        }

        private static bool H1OwnedRelationship(GameObject realized, PrefabRelationship row)
        {
            var ownership = realized.GetComponent<H1ComponentOwnershipMarker>();
            if (ownership == null || ownership.schemaId != H1ComponentOwnershipMarker.SchemaId) return false;
            if (ownership.rendererMaterial && row.kind == "material-reference" && row.relativeObjectPath == ownership.rendererRelativePath) return true;
            if (ownership.animatorClip && row.kind == "animation-clip-reference" && row.relativeObjectPath == ownership.animatorRelativePath) return true;
            return false;
        }

        private static PrefabRelationship[] SourceDerivedRelationships(IEnumerable<PrefabRelationship> rows) => NormalizeRelationships(rows.Where(row => row.kind != "variant-base"));
        private static PrefabRelationship[] NormalizeRelationships(IEnumerable<PrefabRelationship> rows) => rows.OrderBy(RelationshipKey, StringComparer.Ordinal).ToArray();

        private static void AddRelationship(ICollection<PrefabRelationship> rows, string kind, string relativePath, UnityEngine.Object asset)
        {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long fileId)) throw new InvalidDataException("projection.prefab-reference-unresolved");
            var path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path)) throw new InvalidDataException("projection.prefab-reference-unresolved");
            rows.Add(new PrefabRelationship { kind = kind, relativeObjectPath = relativePath, assetPath = path, assetGuid = guid,
                localFileId = fileId.ToString(CultureInfo.InvariantCulture), typeName = asset.GetType().FullName ?? asset.GetType().Name });
        }

        private static string RelativePath(Transform root, Transform target)
        {
            if (target == root) return "";
            var names = new List<string>();
            var current = target;
            while (current != null && current != root) { names.Add(current.name); current = current.parent; }
            names.Reverse();
            return string.Join("/", names.ToArray());
        }

        private static string RelationshipKey(PrefabRelationship row) => row.kind + "|" + row.relativeObjectPath + "|" + row.assetPath + "|" + row.assetGuid + "|" + row.localFileId + "|" + row.typeName;
        private static string RelationshipDigest(IEnumerable<PrefabRelationship> rows)
        {
            var builder = new StringBuilder();
            foreach (var row in NormalizeRelationships(rows)) builder.Append(RelationshipKey(row)).Append('\n');
            return Sha(Encoding.UTF8.GetBytes(builder.ToString()));
        }

        private static void ValidateEffectiveMembership(GameObject root, string generationId)
        {
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                var candidate = transform.gameObject;
                if (candidate == root) continue;
                var marker = candidate.GetComponent<H1ManagedMarker>();
                if (marker != null) continue;
                if (!PrefabUtility.IsPartOfPrefabInstance(candidate) || PrefabUtility.IsAddedGameObjectOverride(candidate)) throw new InvalidDataException("projection.unmanaged-scene-member");
                var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(candidate);
                var owner = prefabRoot == null ? null : prefabRoot.GetComponent<H1ManagedMarker>();
                if (owner == null || owner.schemaId != H1ManagedMarker.SchemaId || owner.role != "object" || owner.sceneLogicalId != SceneId ||
                    owner.generationId != generationId || string.IsNullOrEmpty(owner.canonicalObjectId)) throw new InvalidDataException("projection.unmanaged-scene-member");
            }
        }

        private static bool SameGraph(ProjectionPlan plan, ProjectionObservation observation)
        {
            if (!observation.active || observation.nodes.Length != plan.nodes.Length) return false;
            var expected = plan.nodes.OrderBy(node => node.objectId, StringComparer.Ordinal).ToArray();
            for (var index = 0; index < expected.Length; index++)
            {
                var left = expected[index]; var right = observation.nodes[index];
                if (left.objectId != right.objectId || left.parentObjectId != right.parentObjectId || left.sourceLogicalId != right.sourceLogicalId ||
                    !Equal(left.positionMm, right.positionMm) || !Equal(left.scalePpm, right.scalePpm)) return false;
                var effective = Quaternion.Euler(Unit(right.rotationMilliDegrees.x, 1000), Unit(right.rotationMilliDegrees.y, 1000), Unit(right.rotationMilliDegrees.z, 1000));
                var authored = Quaternion.Euler(Unit(Normalize(left.rotationMilliDegrees.x), 1000), Unit(Normalize(left.rotationMilliDegrees.y), 1000), Unit(Normalize(left.rotationMilliDegrees.z), 1000));
                if (Quaternion.Angle(authored, effective) > 0.02f) return false;
            }
            return true;
        }

        private static bool SameRealization(ProjectionPlan plan, ProjectionObservation observation)
        {
            if (!observation.active || observation.nodes.Length != plan.nodes.Length || !IsHash(observation.realizationDigest)) return false;
            var expected = plan.nodes.OrderBy(node => node.objectId, StringComparer.Ordinal).ToArray();
            var actual = observation.nodes.OrderBy(node => node.objectId, StringComparer.Ordinal).ToArray();
            var prefabGenerationId = PrefabGenerationId(plan);
            for (var index = 0; index < expected.Length; index++)
            {
                var left = expected[index]; var right = actual[index];
                if (right.sourceKind != left.sourceKind || right.sourcePath != left.sourcePath || right.sourceGuid != left.sourceGuid ||
                    right.sourceLocalFileId != left.sourceLocalFileId || right.sourceContentSha256 != left.sourceContentSha256 || !IsHash(right.relationshipDigest) ||
                    right.relationships == null || right.relationships.Length > MaximumRelationships || !IsHash(right.componentDigest) || right.componentRows == null ||
                    !ExpectedComponentRows(left).SequenceEqual(right.componentRows, StringComparer.Ordinal)) return false;
                if (left.sourceKind == "prefab")
                {
                    if (right.realizationKind != "managed-prefab-variant" || right.prefabGenerationId != prefabGenerationId || right.realizedPath != DerivativePath(plan, left) ||
                        string.IsNullOrEmpty(right.realizedGuid) || string.IsNullOrEmpty(right.realizedLocalFileId) ||
                        !right.relationships.Any(row => row.kind == "variant-base" && row.assetPath == left.sourcePath && row.assetGuid == left.sourceGuid && row.localFileId == left.sourceLocalFileId)) return false;
                }
                else if (right.realizationKind != "source-asset" || right.realizedPath != left.sourcePath || right.realizedGuid != left.sourceGuid ||
                         right.realizedLocalFileId != left.sourceLocalFileId || right.prefabGenerationId != "") return false;
            }
            return true;
        }

        private static string[] ExpectedComponentRows(ProjectionNode node)
        {
            var rows = new List<string>();
            foreach (var component in node.components)
            {
                if (component.schemaId == H1ComponentProjection.TransformSchema) rows.Add(TransformRow(node.positionMm, node.rotationMilliDegrees, node.scalePpm));
                else if (component.schemaId == H1ComponentProjection.MeshRendererSchema)
                    rows.Add(component.schemaId + "|enabled=true|material=" + AssetIdentity(component));
                else if (component.schemaId == H1ComponentProjection.AnimatorSchema)
                    rows.Add(component.schemaId + "|applyRootMotion=false|updateMode=Normal|cullingMode=AlwaysAnimate|clip=" + AssetIdentity(component));
                else if (component.schemaId == H1ComponentProjection.CanonicalLinkSchema)
                    rows.Add(component.schemaId + "|relation=" + component.relation + "|target=" + component.targetObjectId);
            }
            rows.Sort(StringComparer.Ordinal);
            return rows.ToArray();
        }

        private static string AssetIdentity(ProjectionComponent component) => component.referencePath + "|" + component.referenceGuid + "|" + component.referenceLocalFileId;
        private static string TransformRow(ProjectionVector position, ProjectionVector rotation, ProjectionVector scale) =>
            H1ComponentProjection.TransformSchema + "|positionMm=" + Vec(position) + "|rotationMilliDegrees=" + VecNormalized(rotation) + "|scalePpm=" + Vec(scale);
        private static string Vec(ProjectionVector value) => value.x + "," + value.y + "," + value.z;
        private static string VecNormalized(ProjectionVector value) => Normalize(value.x) + "," + Normalize(value.y) + "," + Normalize(value.z);
        private static string ComponentDigest(IEnumerable<string> rows) => Sha(Encoding.UTF8.GetBytes(string.Join("\n", rows)));
        private static bool Equal(ProjectionVector left, ProjectionVector right) => left.x == right.x && left.y == right.y && left.z == right.z;
        private static long Normalize(long value) { var result = value % 360000; return result < 0 ? result + 360000 : result; }
        private static ProjectionVector Quantize(Vector3 value, long scale) => new ProjectionVector
        {
            x = (long)Math.Round(value.x * scale, MidpointRounding.AwayFromZero), y = (long)Math.Round(value.y * scale, MidpointRounding.AwayFromZero),
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

        private static string RealizationDigest(IEnumerable<ProjectionObservedNode> nodes)
        {
            var builder = new StringBuilder();
            foreach (var node in nodes.OrderBy(value => value.objectId, StringComparer.Ordinal))
                builder.Append(node.objectId).Append('|').Append(node.sourceKind).Append('|').Append(node.sourceLogicalId).Append('|')
                    .Append(node.sourcePath).Append('|').Append(node.sourceGuid).Append('|').Append(node.sourceLocalFileId).Append('|')
                    .Append(node.sourceContentSha256).Append('|').Append(node.realizationKind).Append('|').Append(node.realizedPath).Append('|')
                    .Append(node.realizedGuid).Append('|').Append(node.realizedLocalFileId).Append('|').Append(node.prefabGenerationId).Append('|')
                    .Append(node.relationshipDigest).Append('|').Append(node.componentDigest).Append('\n');
            return Sha(Encoding.UTF8.GetBytes(builder.ToString()));
        }

        private static string PrefabGenerationId(ProjectionPlan plan) => Sha(Encoding.UTF8.GetBytes("h1-06|" + plan.inputDigest + "|" + plan.catalogueFingerprint)).Substring(0, 32);
        private static string DerivativePath(ProjectionPlan plan, ProjectionNode node)
        {
            var identity = Sha(Encoding.UTF8.GetBytes(node.sourceLogicalId + "|" + node.sourceGuid + "|" + node.sourceLocalFileId + "|" + node.sourceContentSha256)).Substring(0, 24);
            return PrefabGenerations + "/" + PrefabGenerationId(plan) + "/" + identity + ".prefab";
        }

        private static void Mark(GameObject target, string role, string objectId, string sourceId, string generationId)
        {
            var marker = target.AddComponent<H1ManagedMarker>();
            marker.role = role; marker.sceneLogicalId = SceneId; marker.generationId = generationId; marker.canonicalObjectId = objectId; marker.sourceLogicalId = sourceId;
        }

        private static ProjectionManifest ReadManifest()
        {
            var path = Path.Combine(H1Bootstrap.ProjectRoot(), ManifestPath);
            if (!File.Exists(path)) return null;
            var manifest = JsonUtility.FromJson<ProjectionManifest>(File.ReadAllText(path));
            if (manifest == null || manifest.schemaId != ManifestSchema || manifest.sceneLogicalId != SceneId || !IsGenerationId(manifest.generationId) ||
                manifest.scenePath != Generations + "/" + manifest.generationId + ".unity" || !IsHash(manifest.inputDigest) || !IsHash(manifest.canonicalHash) ||
                !IsHash(manifest.catalogueFingerprint) || !IsHash(manifest.graphDigest) || (!string.IsNullOrEmpty(manifest.realizationDigest) && !IsHash(manifest.realizationDigest)))
                throw new InvalidDataException("projection.manifest-invalid");
            return manifest;
        }

        private static void Publish(ProjectionManifest manifest)
        {
            var target = Path.Combine(H1Bootstrap.ProjectRoot(), ManifestPath);
            var temp = target + ".tmp-" + Guid.NewGuid().ToString("N");
            File.WriteAllText(temp, JsonUtility.ToJson(manifest), new UTF8Encoding(false));
            if (File.Exists(target)) File.Replace(temp, target, null); else File.Move(temp, target);
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder(Root)) AssetDatabase.CreateFolder("Assets/Arkus/H1", "ManagedScenes");
            if (!AssetDatabase.IsValidFolder(Generations)) AssetDatabase.CreateFolder(Root, "generations");
            if (!AssetDatabase.IsValidFolder(PrefabRoot)) AssetDatabase.CreateFolder("Assets/Arkus/H1", "ManagedPrefabs");
            if (!AssetDatabase.IsValidFolder(PrefabGenerations)) AssetDatabase.CreateFolder(PrefabRoot, "generations");
        }

        private static void EnsurePrefabGenerationFolder(string generationId)
        {
            EnsureFolders(); var path = PrefabGenerations + "/" + generationId;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(PrefabGenerations, generationId);
        }

        private static void CleanupPrefabGenerations(string current, bool hasPrefabs)
        {
            if (!hasPrefabs || !AssetDatabase.IsValidFolder(PrefabGenerations)) return;
            var full = Path.Combine(H1Bootstrap.ProjectRoot(), PrefabGenerations);
            if (!Directory.Exists(full)) return;
            foreach (var directory in Directory.GetDirectories(full))
            {
                var name = Path.GetFileName(directory);
                if (name == current || !IsGenerationId(name)) continue;
                try { AssetDatabase.DeleteAsset(PrefabGenerations + "/" + name); }
                catch (Exception error) { Debug.LogWarning("ARKUS_H1_PREFAB_CLEANUP:" + error.GetType().Name); }
            }
        }

        private static bool IsHash(string value) => value != null && value.Length == 64 && value.All(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f');
        private static bool IsGenerationId(string value) => value != null && value.Length == 32 && value.All(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f');
        private static float Unit(long value, long denominator) => (float)value / denominator;
        private static string Sha(byte[] bytes) { using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant(); }

        [Serializable] private sealed class ProjectionRequest { public string schemaId; public string mode; public string sceneLogicalId; public ProjectionPlan plan; }
        [Serializable] private sealed class ProjectionReply { public string schemaId; public string sceneLogicalId; public string expectedInputDigest; public string errorCode; public ProjectionObservation observation; }
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
        [Serializable] private sealed class ProjectionManifest
        {
            public string schemaId; public string sceneLogicalId; public string generationId; public string scenePath; public string inputDigest;
            public string canonicalHash; public string catalogueFingerprint; public string graphDigest; public string realizationDigest;
        }
        [Serializable] private sealed class ProjectionObservation
        {
            public string schemaId; public string sceneLogicalId; public bool active; public string generationId; public string inputDigest;
            public string canonicalHash; public string catalogueFingerprint; public string graphDigest; public string realizationDigest; public ProjectionObservedNode[] nodes;
        }
        [Serializable] private sealed class ProjectionObservedNode
        {
            public string objectId; public string parentObjectId; public string sourceLogicalId; public string sourceKind; public string sourcePath;
            public string sourceGuid; public string sourceLocalFileId; public string sourceContentSha256; public string realizationKind; public string realizedPath;
            public string realizedGuid; public string realizedLocalFileId; public string prefabGenerationId; public string relationshipDigest; public PrefabRelationship[] relationships;
            public string componentDigest; public string[] componentRows;
            public ProjectionVector positionMm; public ProjectionVector rotationMilliDegrees; public ProjectionVector scalePpm;
        }
        [Serializable] private sealed class RealizationObservation
        {
            public string sourceKind; public string sourcePath; public string sourceGuid; public string sourceLocalFileId; public string sourceContentSha256;
            public string realizationKind; public string realizedPath; public string realizedGuid; public string realizedLocalFileId; public string prefabGenerationId;
            public string relationshipDigest; public PrefabRelationship[] relationships;
        }
        [Serializable] private sealed class PrefabRelationship
        {
            public string kind; public string relativeObjectPath; public string assetPath; public string assetGuid; public string localFileId; public string typeName;
        }
    }
}
