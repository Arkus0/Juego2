using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Arkus.H1.Projection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arkus.H1.Editor
{
    // Harness-only fixture for the single H1-06 relationship rule:
    // expected source-derived multiset == effective realized source-derived multiset.
    // One duplicate-sibling fixture falsifies both under-count and over-count directions.
    public static class H1PrefabNestedConformance
    {
        private const string Root = "Assets/Arkus/H1/ManagedPrefabs/H1NestedProof";
        private const string Generation = "ffffffffffffffffffffffffffffffff";
        private const string GeneratedRoot = "Assets/Arkus/H1/ManagedPrefabs/generations/" + Generation;
        private const string Logical = "fixture.nested-parent";

        // Mutate an ignored, bridge-managed derivative between public materialize calls.
        // The public proof then exercises the real reuse -> stage -> publish recovery path.
        public static void InjectExtraMaterial()
        {
            H1Bootstrap.RequirePinnedEditor();
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-arkus-h1-input");
            if (index < 0 || index + 1 >= args.Length)
                throw new InvalidDataException("nested-proof.drift-input-missing");
            var requestPath = Path.GetFullPath(args[index + 1]);
            var request = JsonUtility.FromJson<DriftRequest>(File.ReadAllText(requestPath));
            if (request == null || string.IsNullOrEmpty(request.derivativePath) ||
                !request.derivativePath.StartsWith("Assets/Arkus/H1/ManagedPrefabs/generations/", StringComparison.Ordinal) ||
                request.derivativePath.Contains("..") ||
                PrefabUtility.GetPrefabAssetType(AssetDatabase.LoadAssetAtPath<GameObject>(request.derivativePath)) != PrefabAssetType.Variant)
                throw new InvalidDataException("nested-proof.drift-derivative-invalid");

            var root = PrefabUtility.LoadPrefabContents(request.derivativePath);
            try
            {
                var renderer = root.GetComponentsInChildren<MeshRenderer>(true)
                    .FirstOrDefault(candidate => candidate.sharedMaterials.Any(material => material != null));
                if (renderer == null) throw new InvalidDataException("nested-proof.drift-material-missing");
                var materials = renderer.sharedMaterials;
                var materialToRepeat = materials.First(material => material != null);
                renderer.sharedMaterials = materials.Concat(new[] { materialToRepeat }).ToArray();
                bool saved;
                PrefabUtility.SaveAsPrefabAsset(root, request.derivativePath, out saved);
                if (!saved) throw new InvalidDataException("nested-proof.drift-save-failed");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("H106_STALE_EXTRA_RELATION_INJECTED:" + request.derivativePath);
        }

        public static void Run()
        {
            H1Bootstrap.RequirePinnedEditor();
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-arkus-h1-output");
            if (index < 0 || index + 1 >= args.Length)
                throw new InvalidDataException("nested-proof.output-missing");
            var output = Path.GetFullPath(args[index + 1]);
            if (!AssetDatabase.IsValidFolder("Assets/Arkus/H1/ManagedPrefabs"))
                AssetDatabase.CreateFolder("Assets/Arkus/H1", "ManagedPrefabs");
            if (AssetDatabase.IsValidFolder(Root)) AssetDatabase.DeleteAsset(Root);
            AssetDatabase.CreateFolder("Assets/Arkus/H1/ManagedPrefabs", "H1NestedProof");
            if (!AssetDatabase.IsValidFolder("Assets/Arkus/H1/ManagedPrefabs/generations"))
                AssetDatabase.CreateFolder("Assets/Arkus/H1/ManagedPrefabs", "generations");
            if (AssetDatabase.IsValidFolder(GeneratedRoot)) AssetDatabase.DeleteAsset(GeneratedRoot);
            AssetDatabase.CreateFolder("Assets/Arkus/H1/ManagedPrefabs/generations", Generation);

            try
            {
                var nestedPath = Root + "/nested.prefab";
                var parentPath = Root + "/parent.prefab";
                var variantPath = GeneratedRoot + "/variant.prefab";
                var flattenedPath = Root + "/flattened.prefab";
                var scenePath = Root + "/proof.unity";

                var nestedRoot = new GameObject("NestedProp");
                Save(nestedRoot, nestedPath);
                UnityEngine.Object.DestroyImmediate(nestedRoot);
                var nestedAsset = AssetDatabase.LoadAssetAtPath<GameObject>(nestedPath);

                var parentRoot = new GameObject("ParentWall");
                var nestedInstance = PrefabUtility.InstantiatePrefab(nestedAsset) as GameObject;
                var nestedTwin = PrefabUtility.InstantiatePrefab(nestedAsset) as GameObject;
                if (nestedInstance == null || nestedTwin == null)
                    throw new InvalidDataException("nested-proof.source-instance-missing");
                nestedInstance.name = "SameNested";
                nestedTwin.name = "SameNested";
                nestedInstance.transform.SetParent(parentRoot.transform, false);
                nestedTwin.transform.SetParent(parentRoot.transform, false);
                Save(parentRoot, parentPath);
                UnityEngine.Object.DestroyImmediate(parentRoot);
                var parentAsset = AssetDatabase.LoadAssetAtPath<GameObject>(parentPath);
                if (parentAsset == null || !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(parentAsset, out string parentGuid, out long parentFileId))
                    throw new InvalidDataException("nested-proof.parent-identity-missing");

                var projectionType = typeof(H1SceneProjection);
                var observe = projectionType.GetMethod("ObserveRealization", BindingFlags.NonPublic | BindingFlags.Static);
                var collect = projectionType.GetMethod("CollectRelationships", BindingFlags.NonPublic | BindingFlags.Static);
                var digest = projectionType.GetMethod("RelationshipDigest", BindingFlags.NonPublic | BindingFlags.Static);
                if (observe == null || collect == null || digest == null)
                    throw new InvalidDataException("nested-proof.product-observer-missing");

                var variant = BuildVariant(parentAsset, parentPath, parentGuid, parentFileId, variantPath, nestedAsset, false);
                var observed = ObserveVariant(variant, scenePath);
                if (observed.transform.childCount != 2)
                    throw new InvalidDataException("nested-proof.duplicate-sibling-count-lost-after-reload");
                var nestedObserved = observed.transform.Cast<Transform>().ToArray();
                if (nestedObserved.Any(child => child.name != "SameNested" ||
                    AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromOriginalSource(child.gameObject)) != nestedPath))
                    throw new InvalidDataException("nested-proof.nested-link-missing-after-reload");
                H1SceneProjection.ValidateSourceRelationships(observed, parentAsset, parentPath);

                var productObservation = observe.Invoke(null, new object[] { observed, Logical });
                var relationshipField = productObservation.GetType().GetField("relationships");
                var digestField = productObservation.GetType().GetField("relationshipDigest");
                var productRows = relationshipField == null ? null : relationshipField.GetValue(productObservation) as Array;
                var positiveDigest = digestField == null ? null : digestField.GetValue(productObservation) as string;
                if (productRows == null || productRows.Cast<object>().Count(row =>
                    (string)row.GetType().GetField("kind").GetValue(row) == "nested-prefab") != 2)
                    throw new InvalidDataException("nested-proof.product-observation-lost-duplicate-multiplicity");
                if (string.IsNullOrEmpty(positiveDigest))
                    throw new InvalidDataException("nested-proof.product-relationship-digest-missing");

                // Under-count: remove exactly one of two same-name nested siblings from the effective realization.
                UnityEngine.Object.DestroyImmediate(observed.transform.GetChild(1).gameObject);
                if (!EditorSceneManager.SaveScene(observed.scene, scenePath))
                    throw new InvalidDataException("nested-proof.loss-scene-save-failed");
                observed = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single).GetRootGameObjects().Single();
                if (observed.transform.childCount != 1 || observed.transform.GetChild(0).name != "SameNested")
                    throw new InvalidDataException("nested-proof.loss-control-not-preserved-after-reload");
                var brokenRows = collect.Invoke(null, new object[] { observed, parentAsset, parentPath }) as Array;
                var brokenDigest = brokenRows == null ? null : digest.Invoke(null, new object[] { brokenRows }) as string;
                if (string.IsNullOrEmpty(brokenDigest) || brokenDigest == positiveDigest)
                    throw new InvalidDataException("nested-proof.duplicate-sibling-loss-digest-false-green");
                ExpectProductFailure(observe, observed, "projection.prefab-nested-lineage-missing",
                    "nested-proof.duplicate-sibling-loss-false-green");

                // Over-count: preserve every source row and add one more occurrence of the same observed
                // source-derived relationship. This is deliberately not a relation-type-specific guard.
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                variant = BuildVariant(parentAsset, parentPath, parentGuid, parentFileId, variantPath, nestedAsset, true);
                observed = ObserveVariant(variant, scenePath);
                if (observed.transform.childCount != 3 || observed.transform.Cast<Transform>().Any(child => child.name != "SameNested"))
                    throw new InvalidDataException("nested-proof.extra-control-not-preserved-after-reload");
                var extraRows = collect.Invoke(null, new object[] { observed, parentAsset, parentPath }) as Array;
                var extraDigest = extraRows == null ? null : digest.Invoke(null, new object[] { extraRows }) as string;
                if (string.IsNullOrEmpty(extraDigest) || extraDigest == positiveDigest)
                    throw new InvalidDataException("nested-proof.extra-source-derived-digest-false-green");
                ExpectRelationshipFailure(observed, parentAsset, parentPath, "projection.prefab-source-reference-unexpected",
                    "nested-proof.extra-source-derived-false-green");
                ExpectProductFailure(observe, observed, "projection.prefab-source-reference-unexpected",
                    "nested-proof.extra-product-observation-false-green");

                // Rebuild the same deterministic managed path from the unchanged source. The exact relationship
                // multiset and digest must converge back to the original positive observation.
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                variant = BuildVariant(parentAsset, parentPath, parentGuid, parentFileId, variantPath, nestedAsset, false);
                observed = ObserveVariant(variant, scenePath);
                H1SceneProjection.ValidateSourceRelationships(observed, parentAsset, parentPath);
                var recoveredObservation = observe.Invoke(null, new object[] { observed, Logical });
                var recoveredDigest = digestField == null ? null : digestField.GetValue(recoveredObservation) as string;
                if (recoveredDigest != positiveDigest)
                    throw new InvalidDataException("nested-proof.rebuild-relationship-digest-did-not-converge");

                var brokenRoot = new GameObject("ParentWall");
                var brokenChild = new GameObject("nested");
                brokenChild.transform.SetParent(brokenRoot.transform, false);
                Save(brokenRoot, flattenedPath);
                UnityEngine.Object.DestroyImmediate(brokenRoot);
                var flattened = AssetDatabase.LoadAssetAtPath<GameObject>(flattenedPath);
                var flattenedObserved = ObserveVariant(flattened, scenePath);
                ExpectRelationshipFailure(flattenedObserved, parentAsset, parentPath, "projection.prefab-nested-lineage-missing",
                    "nested-proof.flattened-false-green");

                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllText(output, JsonUtility.ToJson(new Result
                {
                    schemaId = "arkus.h1-06-nested-prefab-conformance@1",
                    result = "GREEN",
                    positive = "two-same-name-nested-prefab-links-preserved-after-save-reload",
                    productPath = "ObserveRealization:exact-source-derived-multiset-and-digest",
                    negative = "one-of-two-same-name-siblings-removed:projection.prefab-nested-lineage-missing",
                    extraNegative = "third-identical-source-derived-relation:projection.prefab-source-reference-unexpected",
                    recovery = "stale-extra-derivative-rebuilt:relationship-digest-converged",
                    fixtureRoot = Root
                }, true), new UTF8Encoding(false));
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                AssetDatabase.DeleteAsset(Root);
                AssetDatabase.DeleteAsset(GeneratedRoot);
                AssetDatabase.SaveAssets();
            }
        }

        private static GameObject BuildVariant(GameObject parentAsset, string parentPath, string parentGuid, long parentFileId,
            string variantPath, GameObject nestedAsset, bool addExtra)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(variantPath) != null && !AssetDatabase.DeleteAsset(variantPath))
                throw new InvalidDataException("nested-proof.variant-replace-failed");
            var variantRoot = PrefabUtility.InstantiatePrefab(parentAsset) as GameObject;
            if (variantRoot == null) throw new InvalidDataException("nested-proof.parent-instance-missing");
            try
            {
                var lineage = variantRoot.AddComponent<H1ManagedPrefabLineage>();
                lineage.prefabGenerationId = Generation;
                lineage.sourceLogicalId = Logical;
                lineage.sourcePath = parentPath;
                lineage.sourceGuid = parentGuid;
                lineage.sourceLocalFileId = parentFileId.ToString(System.Globalization.CultureInfo.InvariantCulture);
                using (var sha = SHA256.Create())
                    lineage.sourceContentSha256 = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(
                        Path.Combine(H1Bootstrap.ProjectRoot(), parentPath)))).Replace("-", "").ToLowerInvariant();
                if (addExtra)
                {
                    var extra = PrefabUtility.InstantiatePrefab(nestedAsset) as GameObject;
                    if (extra == null) throw new InvalidDataException("nested-proof.extra-instance-missing");
                    extra.name = "SameNested";
                    extra.transform.SetParent(variantRoot.transform, false);
                }
                Save(variantRoot, variantPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(variantRoot);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var variant = AssetDatabase.LoadAssetAtPath<GameObject>(variantPath);
            if (variant == null || PrefabUtility.GetPrefabAssetType(variant) != PrefabAssetType.Variant)
                throw new InvalidDataException("nested-proof.variant-flattened");
            return variant;
        }

        private static GameObject ObserveVariant(GameObject prefab, string scenePath)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (PrefabUtility.InstantiatePrefab(prefab, scene) == null || !EditorSceneManager.SaveScene(scene, scenePath))
                throw new InvalidDataException("nested-proof.scene-save-failed");
            return EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single).GetRootGameObjects().Single();
        }

        private static void ExpectRelationshipFailure(GameObject realized, GameObject source, string sourcePath,
            string expected, string falseGreen)
        {
            try
            {
                H1SceneProjection.ValidateSourceRelationships(realized, source, sourcePath);
                throw new InvalidDataException(falseGreen);
            }
            catch (InvalidDataException error)
            {
                if (error.Message != expected) throw;
            }
        }

        private static void ExpectProductFailure(MethodInfo observe, GameObject realized, string expected, string falseGreen)
        {
            try
            {
                observe.Invoke(null, new object[] { realized, Logical });
                throw new InvalidDataException(falseGreen);
            }
            catch (TargetInvocationException error)
            {
                var inner = error.InnerException as InvalidDataException;
                if (inner == null || inner.Message != expected) throw;
            }
        }

        private static void Save(GameObject source, string path)
        {
            bool saved;
            if (PrefabUtility.SaveAsPrefabAsset(source, path, out saved) == null || !saved)
                throw new InvalidDataException("nested-proof.prefab-save-failed:" + path);
        }

        [Serializable] private sealed class Result
        {
            public string schemaId;
            public string result;
            public string positive;
            public string productPath;
            public string negative;
            public string extraNegative;
            public string recovery;
            public string fixtureRoot;
        }

        [Serializable] private sealed class DriftRequest
        {
            public string derivativePath;
        }
    }
}
