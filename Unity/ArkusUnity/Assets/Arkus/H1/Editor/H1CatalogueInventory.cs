using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Arkus.H1.Editor
{
    // The roots and admitted types define the reviewed effective universe independently of mapping rows.
    // SourceSlice is deliberately ignored by Git and recreated from the pinned owner Source inputs.
    public static class H1CatalogueInventory
    {
        public const string Schema = "arkus.h1-catalogue-effective-inventory@1";
        public const string SourceRoot = "Assets/Arkus/H1/SourceSlice";
        public const string ProofRoot = "Assets/Arkus/H1/CatalogueProof";
        private static readonly string[] ComponentTypes = { "UnityEngine.Animator", "UnityEngine.MeshRenderer", "UnityEngine.Transform" };

        public static EffectiveInventory Capture()
        {
            H1Bootstrap.RequirePinnedEditor();
            if (!AssetDatabase.IsValidFolder(SourceRoot) || !AssetDatabase.IsValidFolder(ProofRoot))
                throw new InvalidDataException("catalogue.scope-root-missing");

            var rows = new List<EffectiveCatalogueRow>();
            var paths = AssetDatabase.GetAllAssetPaths()
                // The reviewed universe is the immediate files in each root. Importer-generated
                // nested material folders are deliberately outside this H1-04 slice.
                .Where(path => string.Equals(Parent(path), SourceRoot, StringComparison.Ordinal) ||
                               string.Equals(Parent(path), ProofRoot, StringComparison.Ordinal))
                .Where(path => !AssetDatabase.IsValidFolder(path))
                .OrderBy(path => path, StringComparer.Ordinal);

            foreach (var path in paths)
            {
                string contentSha256;
                using (var stream = File.OpenRead(Path.Combine(H1Bootstrap.ProjectRoot(), path)))
                using (var sha = SHA256.Create())
                    contentSha256 = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                var dependencies = AssetDatabase.GetDependencies(path, false)
                    .Where(dependency => !string.Equals(dependency, path, StringComparison.Ordinal))
                    .OrderBy(dependency => dependency, StringComparer.Ordinal).ToArray();
                var assets = path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)
                    ? new UnityEngine.Object[] { AssetDatabase.LoadAssetAtPath<SceneAsset>(path) }
                    : AssetDatabase.LoadAllAssetsAtPath(path);
                var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
                foreach (var asset in assets)
                {
                    if (asset == null) continue;
                    if (asset is GameObject && asset != mainAsset) continue;
                    var kind = Kind(asset);
                    if (kind == null) continue;
                    if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long localFileId) ||
                        string.IsNullOrEmpty(guid) || localFileId == 0)
                        throw new InvalidDataException("catalogue.native-identity-missing:" + path);

                    rows.Add(new EffectiveCatalogueRow
                    {
                        kind = kind,
                        path = path,
                        nativeGuid = guid,
                        localFileId = localFileId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        name = asset.name,
                        typeName = asset.GetType().FullName,
                        dimensions = Dimensions(asset),
                        contentSha256 = contentSha256,
                        schemaFields = new string[0],
                        dependencies = dependencies,
                        compatible = Compatible(asset)
                    });
                }
            }

            var types = TypeCache.GetTypesDerivedFrom<Component>();
            foreach (var name in ComponentTypes)
            {
                var matches = types.Where(type => type.FullName == name && !type.IsAbstract).ToArray();
                if (matches.Length != 1) throw new InvalidDataException("catalogue.component-type-missing:" + name);
                rows.Add(new EffectiveCatalogueRow
                {
                    kind = "component-schema", path = "type:" + name, nativeGuid = "", localFileId = "0",
                    name = matches[0].Name, typeName = name,
                    dimensions = "public-serialized-component", dependencies = new string[0],
                    schemaFields = SerializedFields(matches[0]), compatible = true
                });
            }

            return new EffectiveInventory
            {
                schemaId = Schema,
                projectIdentity = "arkus.unity-project@1:ArkusUnity",
                editorVersion = Application.unityVersion,
                rows = rows.OrderBy(row => row.kind, StringComparer.Ordinal)
                    .ThenBy(row => row.nativeGuid, StringComparer.Ordinal)
                    .ThenBy(row => row.localFileId, StringComparer.Ordinal)
                    .ThenBy(row => row.path, StringComparer.Ordinal).ToArray()
            };
        }

        public static void WriteInventory()
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, "-arkus-h1-output");
            if (index < 0 || index + 1 >= args.Length) throw new InvalidDataException("catalogue.output-missing");
            var output = Path.GetFullPath(args[index + 1]);
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllText(output, JsonUtility.ToJson(Capture(), true), new UTF8Encoding(false));
        }

        public static void CreateProofScene()
        {
            if (!AssetDatabase.IsValidFolder(ProofRoot)) AssetDatabase.CreateFolder("Assets/Arkus/H1", "CatalogueProof");
            var scenePath = ProofRoot + "/CatalogueProof.unity";
            if (!File.Exists(Path.Combine(H1Bootstrap.ProjectRoot(), scenePath)))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var marker = new GameObject("arkus-catalogue-harness-marker");
                marker.transform.position = Vector3.zero;
                if (!EditorSceneManager.SaveScene(scene, scenePath))
                    throw new IOException("catalogue.proof-scene-save-failed");
            }

            // Unity's model importer derives this Standard material from the approved facade FBX.
            // Expose one compatible source-derived material beside the original URP material, whose
            // unresolved Shader Graph is deliberately retained as an incompatible reference case.
            var derived = SourceRoot + "/Materials/Wall_Plaster_Window_Wide_Flat-MI_Plaster.mat";
            var admitted = SourceRoot + "/FacadeImportedMaterial.mat";
            var derivedPath = Path.Combine(H1Bootstrap.ProjectRoot(), derived);
            if (!File.Exists(derivedPath)) throw new InvalidDataException("catalogue.source-derived-material-missing");
            File.Copy(derivedPath, Path.Combine(H1Bootstrap.ProjectRoot(), admitted), true);
            File.WriteAllText(Path.Combine(H1Bootstrap.ProjectRoot(), admitted + ".meta"),
                "fileFormatVersion: 2\nguid: 963743a1121475e92497a201e301bf48\n", new UTF8Encoding(false));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private static string Kind(UnityEngine.Object asset)
        {
            if (asset is SceneAsset) return "scene";
            if (asset is GameObject) return "prefab";
            if (asset is Material) return "material";
            if (asset is AnimationClip) return "animation-clip";
            if (asset is Mesh) return "asset";
            return null;
        }

        private static string Parent(string path)
        {
            var slash = path.LastIndexOf('/');
            return slash < 0 ? "" : path.Substring(0, slash);
        }

        private static string[] SerializedFields(Type componentType)
        {
            GameObject probe = null;
            try
            {
                probe = new GameObject("arkus-component-schema-probe");
                var component = componentType == typeof(Transform) ? probe.transform : probe.AddComponent(componentType);
                var serialized = new SerializedObject(component);
                var iterator = serialized.GetIterator();
                var fields = new List<string>();
                while (iterator.NextVisible(true))
                {
                    fields.Add(iterator.propertyPath + ":" + iterator.propertyType);
                    if (fields.Count > 256) throw new InvalidDataException("catalogue.component-schema-bound-exceeded:" + componentType.FullName);
                }
                return fields.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            }
            finally
            {
                if (probe != null) UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static string Dimensions(UnityEngine.Object asset)
        {
            if (asset is Mesh mesh) return "vertices=" + mesh.vertexCount;
            if (asset is AnimationClip clip) return "seconds=" + clip.length.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + ";fps=" + clip.frameRate.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            if (asset is GameObject go) return "prefabType=" + PrefabUtility.GetPrefabAssetType(go);
            if (asset is Material material) return "shader=" + (material.shader == null ? "missing" : material.shader.name);
            return "unity-scene";
        }

        private static bool Compatible(UnityEngine.Object asset)
        {
            if (asset is Material material) return material.shader != null && !string.Equals(material.shader.name, "Hidden/InternalErrorShader", StringComparison.Ordinal);
            if (asset is GameObject prefab)
            {
                foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (var boundMaterial in renderer.sharedMaterials)
                    {
                        if (boundMaterial == null || boundMaterial.shader == null ||
                            string.Equals(boundMaterial.shader.name, "Hidden/InternalErrorShader", StringComparison.Ordinal)) return false;
                    }
                    if (renderer is SkinnedMeshRenderer skinned && skinned.sharedMesh == null) return false;
                }
                foreach (var meshFilter in prefab.GetComponentsInChildren<MeshFilter>(true))
                    if (meshFilter.sharedMesh == null) return false;
            }
            return true;
        }
    }

    [Serializable]
    public sealed class EffectiveInventory
    {
        public string schemaId;
        public string projectIdentity;
        public string editorVersion;
        public EffectiveCatalogueRow[] rows;
    }

    [Serializable]
    public sealed class EffectiveCatalogueRow
    {
        public string kind;
        public string path;
        public string nativeGuid;
        public string localFileId;
        public string name;
        public string typeName;
        public string dimensions;
        public string contentSha256;
        public string[] schemaFields;
        public string[] dependencies;
        public bool compatible;
    }
}
