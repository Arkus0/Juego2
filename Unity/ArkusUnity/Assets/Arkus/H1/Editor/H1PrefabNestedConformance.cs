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
    // Harness-only fixture: prove evaluated nested links survive save/reload, preserve
    // duplicate-sibling multiplicity, and cannot be hidden by transitive dependencies.
    public static class H1PrefabNestedConformance
    {
        private const string Root = "Assets/Arkus/H1/ManagedPrefabs/H1NestedProof";
        private const string Generation = "ffffffffffffffffffffffffffffffff";
        private const string GeneratedRoot = "Assets/Arkus/H1/ManagedPrefabs/generations/" + Generation;
        private const string Logical = "fixture.nested-parent";

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

                var variantRoot = PrefabUtility.InstantiatePrefab(parentAsset) as GameObject;
                if (variantRoot == null) throw new InvalidDataException("nested-proof.parent-instance-missing");
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(parentAsset, out string parentGuid, out long parentFileId))
                    throw new InvalidDataException("nested-proof.parent-identity-missing");
                var lineage = variantRoot.AddComponent<H1ManagedPrefabLineage>();
                lineage.prefabGenerationId = Generation;
                lineage.sourceLogicalId = Logical;
                lineage.sourcePath = parentPath;
                lineage.sourceGuid = parentGuid;
                lineage.sourceLocalFileId = parentFileId.ToString(System.Globalization.CultureInfo.InvariantCulture);
                using (var sha = SHA256.Create())
                    lineage.sourceContentSha256 = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(
                        Path.Combine(H1Bootstrap.ProjectRoot(), parentPath)))).Replace("-", "").ToLowerInvariant();
                Save(variantRoot, variantPath);
                UnityEngine.Object.DestroyImmediate(variantRoot);
                var variant = AssetDatabase.LoadAssetAtPath<GameObject>(variantPath);
                if (PrefabUtility.GetPrefabAssetType(variant) != PrefabAssetType.Variant)
                    throw new InvalidDataException("nested-proof.variant-flattened");

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                if (PrefabUtility.InstantiatePrefab(variant, scene) == null || !EditorSceneManager.SaveScene(scene, scenePath))
                    throw new InvalidDataException("nested-proof.scene-save-failed");
                var observed = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single).GetRootGameObjects().Single();
                if (observed.transform.childCount != 2)
                    throw new InvalidDataException("nested-proof.duplicate-sibling-count-lost-after-reload");
                var nestedObserved = observed.transform.Cast<Transform>().ToArray();
                if (nestedObserved.Any(child => child.name != "SameNested" ||
                    AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromOriginalSource(child.gameObject)) != nestedPath))
                    throw new InvalidDataException("nested-proof.nested-link-missing-after-reload");
                H1SceneProjection.ValidateSourceRelationships(observed, parentAsset, parentPath);

                var projectionType = typeof(H1SceneProjection);
                var observe = projectionType.GetMethod("ObserveRealization", BindingFlags.NonPublic | BindingFlags.Static);
                var collect = projectionType.GetMethod("CollectRelationships", BindingFlags.NonPublic | BindingFlags.Static);
                var digest = projectionType.GetMethod("RelationshipDigest", BindingFlags.NonPublic | BindingFlags.Static);
                if (observe == null || collect == null || digest == null)
                    throw new InvalidDataException("nested-proof.product-observer-missing");
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

                // Remove exactly one of two same-name nested siblings from the managed scene instance.
                // Unity records the deletion as a removed-GameObject Prefab override; save/reload must
                // retain that one-sibling loss while the surviving sibling keeps the same dependency.
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
                try
                {
                    observe.Invoke(null, new object[] { observed, Logical });
                    throw new InvalidDataException("nested-proof.duplicate-sibling-loss-false-green");
                }
                catch (TargetInvocationException error)
                {
                    var inner = error.InnerException as InvalidDataException;
                    if (inner == null || inner.Message != "projection.prefab-nested-lineage-missing") throw;
                }

                var brokenRoot = new GameObject("ParentWall");
                var brokenChild = new GameObject("nested");
                brokenChild.transform.SetParent(brokenRoot.transform, false);
                Save(brokenRoot, flattenedPath);
                UnityEngine.Object.DestroyImmediate(brokenRoot);
                var flattened = AssetDatabase.LoadAssetAtPath<GameObject>(flattenedPath);
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                if (PrefabUtility.InstantiatePrefab(flattened, scene) == null || !EditorSceneManager.SaveScene(scene, scenePath))
                    throw new InvalidDataException("nested-proof.flattened-scene-save-failed");
                observed = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single).GetRootGameObjects().Single();
                try
                {
                    H1SceneProjection.ValidateSourceRelationships(observed, parentAsset, parentPath);
                    throw new InvalidDataException("nested-proof.flattened-false-green");
                }
                catch (InvalidDataException error)
                {
                    if (error.Message != "projection.prefab-nested-lineage-missing") throw;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllText(output, JsonUtility.ToJson(new Result
                {
                    schemaId = "arkus.h1-06-nested-prefab-conformance@1",
                    result = "GREEN",
                    positive = "two-same-name-nested-prefab-links-preserved-after-save-reload",
                    productPath = "ObserveRealization:nested-prefab-multiplicity-and-digest",
                    negative = "one-of-two-same-name-siblings-removed:projection.prefab-nested-lineage-missing",
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
            public string fixtureRoot;
        }
    }
}
