using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor
{
    public static partial class H1SceneProjection
    {
        public static string Execute(string payload)
        {
            var request = JsonUtility.FromJson<ProjectionRequest>(payload);
            if (request == null || request.schemaId != "arkus.h1-projection-worker-request@1" ||
                request.sceneLogicalId != SceneId || request.plan == null ||
                request.plan.schemaId != "arkus.h1-managed-scene-plan@1" ||
                request.plan.sceneLogicalId != SceneId || request.plan.nodes == null || request.plan.nodes.Length > 128 ||
                !IsHash(request.plan.inputDigest) || !IsHash(request.plan.canonicalHash) || !IsHash(request.plan.catalogueFingerprint))
                throw new InvalidDataException("projection.invalid-worker-plan");
            if (request.mode != "materialize" && request.mode != "observe" &&
                request.mode != "validate-proposed" && request.mode != "validate-current")
                throw new InvalidDataException("projection.invalid-worker-mode");

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
                var currentValidation = RunCurrentValidation(request.plan, out current);
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

        private static string ValidationReply(ProjectionPlan plan, string errorCode, ProjectionObservation observation, H1ValidationResult validation)
        {
            return JsonUtility.ToJson(new ValidationProjectionReply
            {
                schemaId = "arkus.h1-projection-worker-result@1",
                sceneLogicalId = SceneId,
                expectedInputDigest = plan.inputDigest,
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
            var shapeValid = true;
            var checks = new List<H1ValidationCheck>
            {
                H1ProjectionValidation.Check("unity.catalogue.snapshot", SceneId, "", "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json",
                    "refresh accepted catalogue snapshot or repair source binding", ValidateCatalogueSnapshot),
                H1ProjectionValidation.Check("unity.component.adapter-inventory", SceneId, "", "Assets/Arkus/H1",
                    "repair the declared/effective H1 component adapter inventory", () => H1ComponentProjection.CaptureInventory()),
                H1ProjectionValidation.Check("unity.plan.node-shape", SceneId, "", Root,
                    "repair canonical node identity/shape", () =>
                    {
                        try { ValidatePlanShapeForH108(plan); }
                        catch (InvalidDataException) { shapeValid = false; throw; }
                    })
            };

            foreach (var node in plan.nodes.Where(value => value != null).OrderBy(value => value.objectId ?? "", StringComparer.Ordinal))
            {
                var captured = node;
                var resource = string.IsNullOrEmpty(captured.objectId) ? SceneId : captured.objectId;
                var logical = captured.sourceLogicalId ?? "";
                var managed = string.IsNullOrEmpty(captured.sourcePath) ? Root + "/" + resource : captured.sourcePath;
                checks.Add(H1ProjectionValidation.Check("unity.plan.source-binding", resource, logical, managed,
                    "repair logical source identity/path/type", () =>
                    {
                        if (!shapeValid) return;
                        ResolveSource(captured);
                    }));
                checks.Add(H1ProjectionValidation.Check("unity.plan.component", resource, logical, Root + "/" + resource,
                    "repair component schema/field/reference", () =>
                    {
                        if (!shapeValid) return;
                        ValidateComponents(captured);
                        ValidateComponentReferencesForH108(captured);
                    }));
            }

            checks.Add(H1ProjectionValidation.Check("unity.plan.hierarchy", SceneId, "", Root,
                "repair parent or canonical component target", () =>
                {
                    if (!shapeValid) return;
                    ValidatePlanHierarchyForH108(plan);
                }));
            return H1ProjectionValidation.Run(H1ProjectionValidation.Preflight, "proposed", plan.inputDigest, checks);
        }

        private static H1ValidationResult RunCurrentValidation(ProjectionPlan plan, out ProjectionObservation observation)
        {
            var manifest = ReadManifest();
            if (manifest == null)
            {
                observation = EmptyObservation();
                return H1ProjectionValidation.Run(H1ProjectionValidation.PostMaterialization, "current", plan.inputDigest, new[]
                {
                    H1ProjectionValidation.Check("unity.scene.finite-transform", SceneId, "", Root,
                        "materialize a managed generation before validating current state", () => { throw new InvalidDataException("projection.active-scene-missing"); })
                });
            }
            return RunPostflight(plan, manifest.scenePath, manifest.generationId, null, false, out observation);
        }

        private static H1ValidationResult RunPostflight(ProjectionPlan plan, string scenePath, string generationId,
            ProjectionObservation seed, bool comparePlan, out ProjectionObservation observation)
        {
            var current = seed;
            var finiteValid = true;
            var checks = new List<H1ValidationCheck>
            {
                H1ProjectionValidation.Check("unity.scene.finite-transform", SceneId, "", scenePath,
                    "repair non-finite local transform values before publication", () =>
                    {
                        try { H1ProjectionValidation.ValidateFiniteTransforms(scenePath); }
                        catch (InvalidDataException) { finiteValid = false; throw; }
                    }),
                H1ProjectionValidation.Check("unity.scene.managed-marker", SceneId, "", scenePath,
                    "repair managed root/object marker identity and hierarchy", () =>
                    {
                        if (!finiteValid) return;
                        ValidateManagedMarkerClass(plan, scenePath, generationId);
                    }),
                H1ProjectionValidation.Check("unity.scene.prefab-link", SceneId, "", scenePath,
                    "repair prefab derivative/source lineage without mutating source", () =>
                    {
                        if (!finiteValid) return;
                        ValidatePrefabClass(scenePath);
                    }),
                H1ProjectionValidation.Check("unity.scene.component", SceneId, "", scenePath,
                    "repair effective component field/reference realization", () =>
                    {
                        if (!finiteValid) return;
                        ValidateEffectiveComponentClass(scenePath);
                    }),
                H1ProjectionValidation.Check("unity.scene.effective-observation", SceneId, "", scenePath,
                    "repair effective managed scene observation", () =>
                    {
                        if (!finiteValid) return;
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
                        if (current == null) return;
                        if (!SameGraph(plan, current) || !SameRealization(plan, current))
                            throw new InvalidDataException("projection.stage-observation-mismatch");
                    }));
            }
            var result = H1ProjectionValidation.Run(H1ProjectionValidation.PostMaterialization,
                comparePlan ? "proposed-effective" : "current", plan.inputDigest, checks);
            observation = current ?? EmptyObservation();
            return result;
        }

        private static void ValidateManagedMarkerClass(ProjectionPlan plan, string scenePath, string generationId)
        {
            try
            {
                ObserveScene(scenePath, generationId, plan.inputDigest, plan.canonicalHash, plan.catalogueFingerprint);
            }
            catch (InvalidDataException error) when (!OwnsPostflightCode("unity.scene.managed-marker", error.Message)) { }
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
                if (component.schemaId != H1ComponentProjection.MeshRendererSchema && component.schemaId != H1ComponentProjection.AnimatorSchema)
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
            var seen = new HashSet<string>(plan.nodes.Select(node => node.objectId), StringComparer.Ordinal);
            foreach (var node in plan.nodes)
            {
                if (!string.IsNullOrEmpty(node.parentObjectId) && !seen.Contains(node.parentObjectId))
                    throw new InvalidDataException("projection.unbound-parent");
                foreach (var component in node.components)
                    if (component.schemaId == H1ComponentProjection.CanonicalLinkSchema && !seen.Contains(component.targetObjectId))
                        throw new InvalidDataException("projection.component-reference-unresolved");
            }
        }

        private static void InjectNonFiniteTransformForH108(IReadOnlyDictionary<string, GameObject> created)
        {
            var fault = Path.Combine(H1Bootstrap.ProjectRoot(), "Library", "Arkus", "H1Projection", "inject-non-finite-transform");
            if (!File.Exists(fault) || created == null || created.Count == 0) return;
            created.OrderBy(value => value.Key, StringComparer.Ordinal).First().Value.transform.localPosition = new Vector3(float.NaN, 0f, 0f);
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
