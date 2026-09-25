using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.H1.Projection;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor
{
    public static partial class H1SceneProjection
    {
        public static string Execute(string payload)
        {
            ProjectionRequest request;
            try { request = JsonUtility.FromJson<ProjectionRequest>(payload); }
            catch (Exception) { return InvalidRequestReply("projection.invalid-worker-plan"); }

            if (request == null || request.schemaId != "arkus.h1-projection-worker-request@1" ||
                request.sceneLogicalId != SceneId || request.plan == null ||
                request.plan.schemaId != "arkus.h1-managed-scene-plan@1" ||
                request.plan.sceneLogicalId != SceneId || request.plan.nodes == null || request.plan.nodes.Length > 128 ||
                !IsHash(request.plan.inputDigest) || !IsHash(request.plan.canonicalHash) || !IsHash(request.plan.catalogueFingerprint))
                return InvalidRequestReply("projection.invalid-worker-plan");
            if (request.mode != "materialize" && request.mode != "observe" &&
                request.mode != "validate-proposed" && request.mode != "validate-current")
                return InvalidRequestReply("projection.invalid-worker-mode", request.plan);

            try
            {
                if (request.mode == "materialize" || request.mode == "validate-proposed")
                {
                    var preflight = RunPreflight(request.plan);
                    if (!preflight.valid)
                        return ValidationReply(request.plan, FirstValidationCode(preflight), SafeObserveActive(), preflight);
                    if (request.mode == "validate-proposed")
                        return ValidationReply(request.plan, "", SafeObserveActive(), preflight);

                    H1ValidationResult postflight;
                    var observation = Materialize(request.plan, out postflight);
                    return ValidationReply(request.plan, "", observation, postflight);
                }

                ProjectionObservation current;
                var currentValidation = RunCurrentValidation(out current);
                return ValidationReply(request.plan, currentValidation.valid ? "" : FirstValidationCode(currentValidation), current, currentValidation);
            }
            catch (H1ValidationFailureException validationFailure)
            {
                return ValidationReply(request.plan, FirstValidationCode(validationFailure.Result), SafeObserveActive(), validationFailure.Result);
            }
            catch (Exception exception)
            {
                Debug.LogError("ARKUS_H1_PROJECTION_FAILURE:" + exception.GetType().Name + ":" + exception.Message);
                return ValidationReply(request.plan,
                    exception is InvalidDataException && exception.Message.StartsWith("projection.", StringComparison.Ordinal)
                        ? exception.Message : "projection.editor-failure",
                    SafeObserveActive(), null);
            }
        }

        private static ProjectionObservation Materialize(ProjectionPlan plan)
        {
            H1ValidationResult ignored;
            return Materialize(plan, out ignored);
        }

        private static string InvalidRequestReply(string code, ProjectionPlan plan = null)
        {
            var validation = new H1ValidationResult
            {
                schemaId = H1ProjectionValidation.ResultSchema,
                inventorySchemaId = H1ProjectionValidation.InventorySchema,
                phase = H1ProjectionValidation.Preflight,
                scope = "request",
                inputDigest = plan == null ? "" : plan.inputDigest ?? "",
                valid = false,
                executedInvariantIds = new[] { "unity.plan.node-shape" },
                diagnostics = new[]
                {
                    new H1ValidationDiagnostic
                    {
                        code = code,
                        severity = "error",
                        invariantId = "unity.plan.node-shape",
                        phase = H1ProjectionValidation.Preflight,
                        canonicalResource = SceneId,
                        logicalAsset = "",
                        managedPath = Root,
                        context = "use the versioned H1 projection request schema and a supported validation/materialization mode"
                    }
                }
            };
            return ValidationReply(plan, code, SafeObserveActive(), validation);
        }

        private static string ValidationReply(ProjectionPlan plan, string errorCode, ProjectionObservation observation, H1ValidationResult validation)
        {
            return JsonUtility.ToJson(new ValidationProjectionReply
            {
                schemaId = "arkus.h1-projection-worker-result@1",
                sceneLogicalId = SceneId,
                expectedInputDigest = plan == null ? "" : plan.inputDigest ?? "",
                errorCode = errorCode ?? "",
                observation = observation ?? EmptyObservation(),
                validation = validation,
                validationInventory = H1ProjectionValidation.Inventory()
            });
        }

        private static string FirstValidationCode(H1ValidationResult validation)
        {
            return validation != null && validation.diagnostics != null && validation.diagnostics.Length != 0
                ? validation.diagnostics[0].code : "projection.validation-failed";
        }

        private static ProjectionObservation SafeObserveActive()
        {
            try { return ObserveActive(); }
            catch (Exception) { return EmptyObservation(); }
        }

        private static H1ValidationResult RunPreflight(ProjectionPlan plan)
        {
            var checks = new List<H1ValidationCheck>
            {
                H1ProjectionValidation.Check("unity.catalogue.snapshot", SceneId, "", "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json",
                    "refresh accepted catalogue snapshot or repair source binding", ValidateCatalogueSnapshot),
                H1ProjectionValidation.Check("unity.component.adapter-inventory", SceneId, "", "Assets/Arkus/H1",
                    "repair the declared/effective H1 component adapter inventory", () => H1ComponentProjection.CaptureInventory()),
                H1ProjectionValidation.Check("unity.plan.node-shape", SceneId, "", Root,
                    "repair canonical node identity/shape", () => ValidatePlanShapeForH108(plan))
            };

            foreach (var node in plan.nodes.Where(value => value != null).OrderBy(value => value.objectId ?? "", StringComparer.Ordinal))
            {
                var captured = node;
                var resource = string.IsNullOrEmpty(captured.objectId) ? SceneId : captured.objectId;
                var logical = captured.sourceLogicalId ?? "";
                var managed = string.IsNullOrEmpty(captured.sourcePath) ? Root + "/" + resource : captured.sourcePath;
                checks.Add(H1ProjectionValidation.Check("unity.plan.source-binding", resource, logical, managed,
                    "repair logical source identity/path/type", () => ResolveSource(captured)));
                checks.Add(H1ProjectionValidation.Check("unity.plan.component", resource, logical, Root + "/" + resource,
                    "repair component schema/field/reference", () =>
                    {
                        if (!ComponentCheckEligible(captured)) return;
                        ValidateComponents(captured);
                        ValidateComponentReferencesForH108(captured);
                    }));
            }

            checks.Add(H1ProjectionValidation.Check("unity.plan.hierarchy", SceneId, "", Root,
                "repair parent or canonical component target", () => ValidatePlanHierarchyForH108(plan)));
            return H1ProjectionValidation.Run(H1ProjectionValidation.Preflight, "proposed", plan.inputDigest, checks);
        }

        private static bool ComponentCheckEligible(ProjectionNode node)
        {
            return node != null && node.components != null && node.components.Length != 0 && node.components.Length <= MaximumComponents;
        }

        private static H1ValidationResult RunCurrentValidation(out ProjectionObservation observation)
        {
            var manifest = ReadManifest();
            if (manifest == null)
            {
                observation = EmptyObservation();
                return H1ProjectionValidation.Run(H1ProjectionValidation.PostMaterialization, "current", "", new[]
                {
                    H1ProjectionValidation.Check("unity.scene.finite-transform", SceneId, "", Root,
                        "materialize a managed generation before validating current state", () => { throw new InvalidDataException("projection.active-scene-missing"); })
                });
            }

            var current = default(ProjectionObservation);
            var transformsFinite = true;
            var checks = new List<H1ValidationCheck>
            {
                H1ProjectionValidation.Check("unity.scene.finite-transform", SceneId, "", manifest.scenePath,
                    "repair non-finite local transform values before publication", () =>
                    {
                        try { H1ProjectionValidation.ValidateFiniteTransforms(manifest.scenePath); }
                        catch (InvalidDataException) { transformsFinite = false; throw; }
                    }),
                H1ProjectionValidation.Check("unity.scene.managed-marker", SceneId, "", manifest.scenePath,
                    "repair managed root/object marker identity and hierarchy", () => ValidateManagedMarkerClass(manifest.scenePath, manifest.generationId)),
                H1ProjectionValidation.Check("unity.scene.prefab-link", SceneId, "", manifest.scenePath,
                    "repair prefab derivative/source lineage without mutating source", () => ValidatePrefabClass(manifest.scenePath)),
                H1ProjectionValidation.Check("unity.scene.component", SceneId, "", manifest.scenePath,
                    "repair effective component field/reference realization", () =>
                    {
                        if (!transformsFinite) return;
                        ValidateEffectiveComponentClass(manifest.scenePath);
                    }),
                H1ProjectionValidation.Check("unity.scene.effective-observation", SceneId, "", manifest.scenePath,
                    "repair active manifest identity or effective managed scene drift", () =>
                    {
                        if (!transformsFinite) return;
                        try { current = ObserveManifest(manifest); }
                        catch (InvalidDataException error) when (OwnedBySpecializedPostflight(error.Message) &&
                            error.Message != "projection.active-prefab-drift") { }
                    })
            };

            var result = H1ProjectionValidation.Run(H1ProjectionValidation.PostMaterialization,
                "current", manifest.inputDigest, checks);
            observation = current ?? EmptyObservation();
            return result;
        }

        private static H1ValidationResult RunPostflight(ProjectionPlan plan, string scenePath, string generationId,
            ProjectionObservation seed, bool comparePlan, out ProjectionObservation observation)
        {
            var current = seed;
            var transformsFinite = true;
            var checks = new List<H1ValidationCheck>
            {
                H1ProjectionValidation.Check("unity.scene.finite-transform", SceneId, "", scenePath,
                    "repair non-finite local transform values before publication", () =>
                    {
                        try { H1ProjectionValidation.ValidateFiniteTransforms(scenePath); }
                        catch (InvalidDataException) { transformsFinite = false; throw; }
                    }),
                H1ProjectionValidation.Check("unity.scene.managed-marker", SceneId, "", scenePath,
                    "repair managed root/object marker identity and hierarchy", () => ValidateManagedMarkerClass(scenePath, generationId)),
                H1ProjectionValidation.Check("unity.scene.prefab-link", SceneId, "", scenePath,
                    "repair prefab derivative/source lineage without mutating source", () => ValidatePrefabClass(scenePath)),
                H1ProjectionValidation.Check("unity.scene.component", SceneId, "", scenePath,
                    "repair effective component field/reference realization", () =>
                    {
                        if (!transformsFinite) return;
                        ValidateEffectiveComponentClass(scenePath);
                    }),
                H1ProjectionValidation.Check("unity.scene.effective-observation", SceneId, "", scenePath,
                    "repair effective managed scene observation", () =>
                    {
                        if (!transformsFinite) return;
                        try
                        {
                            ValidateSourceBytes(plan);
                            current = ObserveScene(scenePath, generationId, plan.inputDigest, plan.canonicalHash, plan.catalogueFingerprint);
                        }
                        catch (InvalidDataException error) when (OwnedBySpecializedPostflight(error.Message)) { }
                    })
            };
            if (comparePlan)
            {
                checks.Add(H1ProjectionValidation.Check("unity.scene.plan-observation", SceneId, "", scenePath,
                    "repair effective scene so graph and realization match the proposed plan", () =>
                    {
                        if (!transformsFinite || current == null) return;
                        if (!SameGraph(plan, current) || !SameRealization(plan, current))
                            throw new InvalidDataException("projection.stage-observation-mismatch");
                    }));
            }
            var result = H1ProjectionValidation.Run(H1ProjectionValidation.PostMaterialization,
                comparePlan ? "proposed-effective" : "current", plan.inputDigest, checks);
            observation = current ?? EmptyObservation();
            return result;
        }

        private static void ValidateManagedMarkerClass(string scenePath, string generationId)
        {
            if (!File.Exists(Path.Combine(H1Bootstrap.ProjectRoot(), scenePath)))
                throw new InvalidDataException("projection.active-scene-missing");
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
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
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var marker in roots[0].GetComponentsInChildren<H1ManagedMarker>(true))
            {
                if (marker == root) continue;
                if (marker.schemaId != H1ManagedMarker.SchemaId || marker.role != "object" || marker.sceneLogicalId != SceneId ||
                    marker.generationId != generationId || string.IsNullOrEmpty(marker.canonicalObjectId) || !seen.Add(marker.canonicalObjectId))
                    throw new InvalidDataException("projection.duplicate-or-invalid-marker");
                var parent = marker.transform.parent == null ? null : marker.transform.parent.GetComponent<H1ManagedMarker>();
                if (parent == null || (parent != root && parent.role != "object"))
                    throw new InvalidDataException("projection.unmanaged-parent");
            }
        }

        private static void ValidatePrefabClass(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            foreach (var marker in root.GetComponentsInChildren<H1ManagedMarker>(true))
            {
                if (marker == null || marker.role != "object" || string.IsNullOrEmpty(marker.sourceLogicalId)) continue;
                ObserveRealization(marker.gameObject, marker.sourceLogicalId);
            }
        }

        private static void ValidateEffectiveComponentClass(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            foreach (var marker in root.GetComponentsInChildren<H1ManagedMarker>(true))
            {
                if (marker == null || marker.role != "object") continue;
                var position = Quantize(marker.transform.localPosition, 1000);
                var rotation = Quantize(marker.transform.localEulerAngles, 1000);
                var scale = Quantize(marker.transform.localScale, 1000000);
                ObserveComponentRows(marker.gameObject, position, rotation, scale);
            }
        }

        private static bool OwnedBySpecializedPostflight(string code) =>
            OwnsPostflightCode("unity.scene.managed-marker", code) ||
            OwnsPostflightCode("unity.scene.prefab-link", code) ||
            OwnsPostflightCode("unity.scene.component", code);

        private static bool OwnsPostflightCode(string invariantId, string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            if (invariantId == "unity.scene.managed-marker")
                return code == "projection.root-count" || code.IndexOf("marker", StringComparison.Ordinal) >= 0 ||
                    code == "projection.unmanaged-parent" || code == "projection.unmanaged-scene-member";
            if (invariantId == "unity.scene.prefab-link")
                return code.IndexOf("prefab", StringComparison.Ordinal) >= 0 || code.IndexOf("lineage", StringComparison.Ordinal) >= 0;
            if (invariantId == "unity.scene.component")
                return code.IndexOf("component", StringComparison.Ordinal) >= 0;
            return false;
        }

        private static void ValidatePlanShapeForH108(ProjectionPlan plan)
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
            }
        }

        private static void ValidateComponentReferencesForH108(ProjectionNode node)
        {
            foreach (var component in node.components)
            {
                if (component == null ||
                    (component.schemaId != H1ComponentProjection.MeshRendererSchema && component.schemaId != H1ComponentProjection.AnimatorSchema))
                    continue;
                var probe = new GameObject("H1-08 preflight component probe");
                try
                {
                    if (component.schemaId == H1ComponentProjection.MeshRendererSchema)
                    {
                        probe.AddComponent<MeshRenderer>();
                        H1ComponentProjection.ApplyRenderer(probe, component.referencePath, component.referenceGuid,
                            component.referenceLocalFileId, component.referenceContentSha256);
                    }
                    else
                    {
                        H1ComponentProjection.ApplyAnimator(probe, "h1-08-preflight", component.referencePath, component.referenceGuid,
                            component.referenceLocalFileId, component.referenceContentSha256);
                    }
                }
                finally { UnityEngine.Object.DestroyImmediate(probe); }
            }
        }

        private static void ValidatePlanHierarchyForH108(ProjectionPlan plan)
        {
            var seen = new HashSet<string>(plan.nodes.Where(node => node != null && !string.IsNullOrEmpty(node.objectId))
                .Select(node => node.objectId), StringComparer.Ordinal);
            foreach (var node in plan.nodes)
            {
                if (node == null) continue;
                if (!string.IsNullOrEmpty(node.parentObjectId) && !seen.Contains(node.parentObjectId))
                    throw new InvalidDataException("projection.unbound-parent");
                if (node.components == null) continue;
                foreach (var component in node.components)
                    if (component != null && component.schemaId == H1ComponentProjection.CanonicalLinkSchema && !seen.Contains(component.targetObjectId))
                        throw new InvalidDataException("projection.component-reference-unresolved");
            }
        }

        private static void InjectNonFiniteTransformForH108(IReadOnlyDictionary<string, GameObject> created)
        {
            // H1-08 fault injection is consumed by ValidateFiniteTransforms as a synthetic sample.
            // Do not write NaN into Unity Transform state: Unity sanitizes/rejects that assignment.
        }

        [Serializable]
        private sealed class ValidationProjectionReply
        {
            public string schemaId;
            public string sceneLogicalId;
            public string expectedInputDigest;
            public string errorCode;
            public ProjectionObservation observation;
            public H1ValidationResult validation;
            public H1ValidationInvariantDescriptor[] validationInventory;
        }
    }
}
