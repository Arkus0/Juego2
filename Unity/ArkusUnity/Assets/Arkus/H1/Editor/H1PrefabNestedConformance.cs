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
    // Harness-only fixture: prove evaluated nested links survive save/reload and a
    // flattened child cannot be hidden by transitive AssetDatabase dependencies.
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
                if (nestedInstance == null) throw new InvalidDataException("nested-proof.source-instance-missing");
                nestedInstance.transform.SetParent(parentRoot.transform, false);
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
                var nestedObserved = observed.transform.childCount == 1 ? observed.transform.GetChild(0) : null;
                if (nestedObserved == null ||
                    AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromOriginalSource(nestedObserved.gameObject)) != nestedPath)
                    throw new InvalidDataException("nested-proof.nested-link-missing-after-reload");
                H1SceneProjection.ValidateSourceRelationships(observed, parentAsset, parentPath);
                var observe = typeof(H1SceneProjection).GetMethod("ObserveRealization", BindingFlags.NonPublic | BindingFlags.Static);
                if (observe == null) throw new InvalidDataException("nested-proof.product-observer-missing");
                var productObservation = observe.Invoke(null, new object[] { observed, Logical });
                var relationshipField = productObservation.GetType().GetField("relationships");
                var productRows = relationshipField == null ? null : relationshipField.GetValue(productObservation) as Array;
                if (productRows == null || !productRows.Cast<object>().Any(row =>
                    (string)row.GetType().GetField("kind").GetValue(row) == "nested-prefab"))
                    throw new InvalidDataException("nested-proof.product-observation-omitted-nested-link");

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
                    positive = "nested-prefab-link-preserved-after-save-reload",
                    productPath = "ObserveRealization:nested-prefab-row",
                    negative = "projection.prefab-nested-lineage-missing",
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
