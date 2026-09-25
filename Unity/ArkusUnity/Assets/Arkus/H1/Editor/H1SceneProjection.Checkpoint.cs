using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor
{
    public static partial class H1SceneProjection
    {
        // H1-10 owns only the clean-output orchestration. Validation, realization, postflight and
        // atomic generation publication remain the accepted H1 materialization implementation.
        public static string ExecuteCleanRebuild(string payload)
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
            if (request.mode != "clean-rebuild") return InvalidRequestReply("projection.invalid-worker-mode", request.plan);

            try
            {
                ValidatedProjectionPlan validatedPlan;
                var preflight = RunPreflight(request.plan, payload, out validatedPlan);
                if (!preflight.valid)
                    return ValidationReply(request.plan, FirstValidationCode(preflight), SafeObserveActive(), preflight);

                DeleteGeneratedProjection();
                H1ValidationResult postflight;
                var observation = Materialize(validatedPlan, out postflight);
                return ValidationReply(request.plan, "", observation, postflight);
            }
            catch (H1ValidationFailureException validationFailure)
            {
                return ValidationReply(request.plan, FirstValidationCode(validationFailure.Result), SafeObserveActive(), validationFailure.Result);
            }
            catch (Exception exception)
            {
                Debug.LogError("ARKUS_H1_CLEAN_REBUILD_FAILURE:" + exception.GetType().Name + ":" + exception.Message);
                return ValidationReply(request.plan,
                    exception is InvalidDataException && exception.Message.StartsWith("projection.", StringComparison.Ordinal)
                        ? exception.Message : "projection.editor-failure",
                    SafeObserveActive(), null);
            }
        }

        private static void DeleteGeneratedProjection()
        {
            // Never delete or rewrite SourceSlice/catalogue inputs. Only the two H1 generated-output
            // roots are cleared, then the accepted materializer stages and publishes a new generation.
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (AssetDatabase.IsValidFolder(Root) && !AssetDatabase.DeleteAsset(Root))
                throw new InvalidDataException("projection.clean-scene-delete-failed");
            if (AssetDatabase.IsValidFolder(PrefabRoot) && !AssetDatabase.DeleteAsset(PrefabRoot))
                throw new InvalidDataException("projection.clean-prefab-delete-failed");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (File.Exists(Path.Combine(H1Bootstrap.ProjectRoot(), ManifestPath)))
                throw new InvalidDataException("projection.clean-manifest-retained");
        }
    }
}
