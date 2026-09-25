using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Arkus.H1.Projection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Arkus.H1.Editor
{
    // H1-07 deliberately has no SerializedProperty/reflection mutation fallback. Each adapter
    // names one schema and one Unity type and performs typed reads/writes only.
    internal static class H1ComponentProjection
    {
        internal const string TransformSchema = "arkus.h1.component.transform@1";
        internal const string MeshRendererSchema = "arkus.h1.component.mesh-renderer@1";
        internal const string AnimatorSchema = "arkus.h1.component.animator@1";
        internal const string CanonicalLinkSchema = H1CanonicalLinkMarker.SchemaId;
        internal const string InventorySchema = "arkus.h1-component-adapter-inventory@1";
        private const string ControllersRoot = "Assets/Arkus/H1/ManagedScenes/controllers";

        // This is the declared schema side of the proof. Effective adapters are enumerated below
        // from concrete adapter instances rather than derived from this list.
        internal static readonly string[] DeclaredSchemas =
        {
            AnimatorSchema, CanonicalLinkSchema, MeshRendererSchema, TransformSchema
        };

        private static readonly IH1ComponentAdapter[] EffectiveAdapters =
        {
            new AnimatorAdapter(), new CanonicalLinkAdapter(), new MeshRendererAdapter(), new TransformAdapter()
        };

        internal static ComponentInventory CaptureInventory()
        {
            var effective = EffectiveAdapters.Select(adapter => new ComponentAdapterRow
            {
                schemaId = adapter.SchemaId,
                unityType = adapter.UnityType.FullName,
                adapterType = adapter.GetType().FullName
            }).OrderBy(row => row.schemaId, StringComparer.Ordinal).ToArray();
            if (effective.Select(row => row.schemaId).Distinct(StringComparer.Ordinal).Count() != effective.Length)
                throw new InvalidDataException("projection.component-adapter-duplicate");
            if (!DeclaredSchemas.SequenceEqual(effective.Select(row => row.schemaId), StringComparer.Ordinal))
                throw new InvalidDataException("projection.component-adapter-inventory-mismatch");
            return new ComponentInventory { schemaId = InventorySchema, declaredSchemas = DeclaredSchemas.ToArray(), effectiveAdapters = effective };
        }

        internal static void ValidateSchema(string schemaId)
        {
            CaptureInventory();
            if (!DeclaredSchemas.Contains(schemaId, StringComparer.Ordinal))
                throw new InvalidDataException("projection.component-schema-unsupported");
        }

        internal static void ApplyTransform(GameObject owner, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
        {
            Get<TransformAdapter>(TransformSchema).Apply(owner, localPosition, localEulerAngles, localScale);
        }

        internal static void ApplyRenderer(GameObject owner, string path, string guid, string localFileId, string contentSha256)
        {
            Get<MeshRendererAdapter>(MeshRendererSchema).Apply(owner, ResolveAsset<Material>(path, guid, localFileId, contentSha256, "material"));
        }

        internal static void ApplyAnimator(GameObject owner, string objectId, string path, string guid, string localFileId, string contentSha256)
        {
            Get<AnimatorAdapter>(AnimatorSchema).Apply(owner, objectId, ResolveAsset<AnimationClip>(path, guid, localFileId, contentSha256, "animation-clip"));
        }

        internal static void ApplyCanonicalLink(GameObject owner, string relation, string targetObjectId, GameObject target)
        {
            Get<CanonicalLinkAdapter>(CanonicalLinkSchema).Apply(owner, relation, targetObjectId, target);
        }

        internal static string ObserveDigest(GameObject owner)
        {
            CaptureInventory();
            var rows = new List<string>();
            rows.Add(Get<TransformAdapter>(TransformSchema).Observe(owner));
            var renderer = owner.GetComponent<MeshRenderer>();
            if (renderer != null) rows.Add(Get<MeshRendererAdapter>(MeshRendererSchema).Observe(owner));
            var animator = owner.GetComponent<Animator>();
            if (animator != null) rows.Add(Get<AnimatorAdapter>(AnimatorSchema).Observe(owner));
            var link = owner.GetComponent<H1CanonicalLinkMarker>();
            if (link != null) rows.Add(Get<CanonicalLinkAdapter>(CanonicalLinkSchema).Observe(owner));
            rows.Sort(StringComparer.Ordinal);
            return Sha(string.Join("\n", rows));
        }

        internal static string ObserveReference(GameObject owner, string schemaId)
        {
            ValidateSchema(schemaId);
            if (schemaId == MeshRendererSchema) return Get<MeshRendererAdapter>(schemaId).Observe(owner);
            if (schemaId == AnimatorSchema) return Get<AnimatorAdapter>(schemaId).Observe(owner);
            if (schemaId == CanonicalLinkSchema) return Get<CanonicalLinkAdapter>(schemaId).Observe(owner);
            return Get<TransformAdapter>(schemaId).Observe(owner);
        }

        private static T Get<T>(string schemaId) where T : class, IH1ComponentAdapter
        {
            ValidateSchemaWithoutRecursion(schemaId);
            var adapter = EffectiveAdapters.SingleOrDefault(value => value.SchemaId == schemaId) as T;
            if (adapter == null) throw new InvalidDataException("projection.component-adapter-missing");
            return adapter;
        }

        private static IH1ComponentAdapter Get(string schemaId)
        {
            ValidateSchemaWithoutRecursion(schemaId);
            var matches = EffectiveAdapters.Where(value => value.SchemaId == schemaId).ToArray();
            if (matches.Length != 1) throw new InvalidDataException("projection.component-adapter-cardinality");
            return matches[0];
        }

        private static void ValidateSchemaWithoutRecursion(string schemaId)
        {
            CaptureInventory();
            if (!DeclaredSchemas.Contains(schemaId, StringComparer.Ordinal))
                throw new InvalidDataException("projection.component-schema-unsupported");
        }

        private static T ResolveAsset<T>(string path, string guid, string localFileId, string contentSha256, string kind) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(localFileId) || string.IsNullOrEmpty(contentSha256))
                throw new InvalidDataException("projection.component-reference-incomplete");
            if (AssetDatabase.AssetPathToGUID(path) != guid)
                throw new InvalidDataException("projection.component-reference-rebound");
            var full = Path.Combine(H1Bootstrap.ProjectRoot(), path);
            if (!File.Exists(full) || ShaBytes(File.ReadAllBytes(full)) != contentSha256)
                throw new InvalidDataException("projection.component-reference-rebound");
            var identityFound = false;
            foreach (var candidate in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (candidate == null || !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(candidate, out string candidateGuid, out long fileId) ||
                    candidateGuid != guid || fileId.ToString(CultureInfo.InvariantCulture) != localFileId) continue;
                identityFound = true;
                if (candidate is T typed) return typed;
            }
            if (identityFound) throw new InvalidDataException("projection.component-reference-wrong-type");
            throw new InvalidDataException("projection.component-reference-rebound");
        }

        private static string StableAssetIdentity(UnityEngine.Object asset)
        {
            if (asset == null || !AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out string guid, out long fileId))
                throw new InvalidDataException("projection.component-reference-unresolved");
            return AssetDatabase.GetAssetPath(asset) + "|" + guid + "|" + fileId.ToString(CultureInfo.InvariantCulture);
        }

        private static string Sha(string value) => ShaBytes(System.Text.Encoding.UTF8.GetBytes(value));
        private static string ShaBytes(byte[] bytes)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        private interface IH1ComponentAdapter
        {
            string SchemaId { get; }
            Type UnityType { get; }
            string Observe(GameObject owner);
        }

        private sealed class TransformAdapter : IH1ComponentAdapter
        {
            public string SchemaId => TransformSchema;
            public Type UnityType => typeof(Transform);
            public void Apply(GameObject owner, Vector3 position, Vector3 euler, Vector3 scale)
            {
                owner.transform.localPosition = position;
                owner.transform.localEulerAngles = euler;
                owner.transform.localScale = scale;
            }
            public string Observe(GameObject owner) => SchemaId + "|position=" + Vec(owner.transform.localPosition) +
                "|rotation=" + Vec(owner.transform.localEulerAngles) + "|scale=" + Vec(owner.transform.localScale);
        }

        private sealed class MeshRendererAdapter : IH1ComponentAdapter
        {
            public string SchemaId => MeshRendererSchema;
            public Type UnityType => typeof(MeshRenderer);
            public void Apply(GameObject owner, Material material)
            {
                var renderer = owner.GetComponent<MeshRenderer>();
                if (renderer == null) throw new InvalidDataException("projection.component-target-missing");
                renderer.sharedMaterial = material;
            }
            public string Observe(GameObject owner)
            {
                var renderer = owner.GetComponent<MeshRenderer>();
                if (renderer == null || renderer.sharedMaterial == null) throw new InvalidDataException("projection.component-target-missing");
                return SchemaId + "|enabled=" + renderer.enabled.ToString().ToLowerInvariant() + "|material=" + StableAssetIdentity(renderer.sharedMaterial);
            }
        }

        private sealed class AnimatorAdapter : IH1ComponentAdapter
        {
            public string SchemaId => AnimatorSchema;
            public Type UnityType => typeof(Animator);
            public void Apply(GameObject owner, string objectId, AnimationClip clip)
            {
                var animator = owner.GetComponent<Animator>();
                if (animator == null) animator = owner.AddComponent<Animator>();
                animator.applyRootMotion = false;
                animator.updateMode = AnimatorUpdateMode.Normal;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                if (!AssetDatabase.IsValidFolder(ControllersRoot))
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Arkus/H1/ManagedScenes")) AssetDatabase.CreateFolder("Assets/Arkus/H1", "ManagedScenes");
                    AssetDatabase.CreateFolder("Assets/Arkus/H1/ManagedScenes", "controllers");
                }
                var identity = Sha(objectId + "|" + StableAssetIdentity(clip)).Substring(0, 24);
                var path = ControllersRoot + "/" + identity + ".controller";
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
                if (controller == null)
                {
                    controller = AnimatorController.CreateAnimatorControllerAtPath(path);
                    var state = controller.layers[0].stateMachine.AddState("h1-clip");
                    state.motion = clip;
                    controller.layers[0].stateMachine.defaultState = state;
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    var motions = controller.animationClips;
                    if (motions.Length != 1 || motions[0] != clip) throw new InvalidDataException("projection.component-controller-drift");
                }
                animator.runtimeAnimatorController = controller;
            }
            public string Observe(GameObject owner)
            {
                var animator = owner.GetComponent<Animator>();
                if (animator == null || animator.runtimeAnimatorController == null) throw new InvalidDataException("projection.component-target-missing");
                var clips = animator.runtimeAnimatorController.animationClips.Distinct().ToArray();
                if (clips.Length != 1) throw new InvalidDataException("projection.component-animation-cardinality");
                return SchemaId + "|applyRootMotion=" + animator.applyRootMotion.ToString().ToLowerInvariant() +
                    "|updateMode=" + animator.updateMode + "|cullingMode=" + animator.cullingMode + "|clip=" + StableAssetIdentity(clips[0]);
            }
        }

        private sealed class CanonicalLinkAdapter : IH1ComponentAdapter
        {
            public string SchemaId => CanonicalLinkSchema;
            public Type UnityType => typeof(H1CanonicalLinkMarker);
            public void Apply(GameObject owner, string relation, string targetObjectId, GameObject target)
            {
                if (string.IsNullOrEmpty(relation) || string.IsNullOrEmpty(targetObjectId) || target == null)
                    throw new InvalidDataException("projection.component-reference-invalid");
                var marker = owner.GetComponent<H1CanonicalLinkMarker>();
                if (marker == null) marker = owner.AddComponent<H1CanonicalLinkMarker>();
                marker.schemaId = CanonicalLinkSchema;
                marker.relation = relation;
                marker.targetObjectId = targetObjectId;
                marker.target = target;
            }
            public string Observe(GameObject owner)
            {
                var marker = owner.GetComponent<H1CanonicalLinkMarker>();
                if (marker == null || marker.schemaId != SchemaId || marker.target == null)
                    throw new InvalidDataException("projection.component-reference-unresolved");
                var targetMarker = marker.target.GetComponent<H1ManagedMarker>();
                if (targetMarker == null || targetMarker.canonicalObjectId != marker.targetObjectId)
                    throw new InvalidDataException("projection.component-reference-target-mismatch");
                return SchemaId + "|relation=" + marker.relation + "|target=" + marker.targetObjectId;
            }
        }

        private static string Vec(Vector3 value) => value.x.ToString("R", CultureInfo.InvariantCulture) + "," +
            value.y.ToString("R", CultureInfo.InvariantCulture) + "," + value.z.ToString("R", CultureInfo.InvariantCulture);

        [Serializable] internal sealed class ComponentInventory
        {
            public string schemaId;
            public string[] declaredSchemas;
            public ComponentAdapterRow[] effectiveAdapters;
        }
        [Serializable] internal sealed class ComponentAdapterRow
        {
            public string schemaId;
            public string unityType;
            public string adapterType;
        }
    }
}
