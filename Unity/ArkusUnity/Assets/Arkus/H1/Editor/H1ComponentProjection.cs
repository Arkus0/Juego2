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
    // names one schema and one Unity type and performs typed reads/writes only. TypeCache is used
    // only to enumerate the closed adapter implementation universe for completeness proof.
    internal static class H1ComponentProjection
    {
        internal const string TransformSchema = "arkus.h1.component.transform@1";
        internal const string MeshRendererSchema = "arkus.h1.component.mesh-renderer@1";
        internal const string AnimatorSchema = "arkus.h1.component.animator@1";
        internal const string CanonicalLinkSchema = H1CanonicalLinkMarker.SchemaId;
        internal const string InventorySchema = "arkus.h1-component-adapter-inventory@1";
        private const string ControllersRoot = "Assets/Arkus/H1/ManagedScenes/controllers";

        // Declared portable schemas are independent from effective adapter enumeration.
        private static readonly ComponentSchemaDescriptor[] DeclaredDescriptors =
        {
            Descriptor(AnimatorSchema, typeof(Animator),
                "applyRootMotion:bool=false", "clip:catalogue-ref:animation-clip",
                "cullingMode:enum=AlwaysAnimate", "updateMode:enum=Normal"),
            Descriptor(CanonicalLinkSchema, typeof(H1CanonicalLinkMarker),
                "relation:string", "targetObjectId:canonical-ref"),
            Descriptor(MeshRendererSchema, typeof(MeshRenderer),
                "enabled:bool=true", "material:catalogue-ref:material"),
            Descriptor(TransformSchema, typeof(Transform),
                "positionMm:vector3-int64", "rotationMilliDegrees:vector3-int64", "scalePpm:vector3-int64")
        };

        internal static readonly string[] DeclaredSchemas = DeclaredDescriptors
            .Select(value => value.schemaId).OrderBy(value => value, StringComparer.Ordinal).ToArray();

        internal static ComponentInventory CaptureInventory()
        {
            var adapters = BuildEffectiveAdapters();
            var effective = adapters.Select(adapter => new ComponentAdapterRow
            {
                schemaId = adapter.SchemaId,
                unityType = adapter.UnityType.FullName,
                adapterType = adapter.GetType().FullName,
                fields = adapter.Fields.OrderBy(value => value, StringComparer.Ordinal).ToArray()
            }).OrderBy(row => row.schemaId, StringComparer.Ordinal).ToArray();

            if (effective.Select(row => row.schemaId).Distinct(StringComparer.Ordinal).Count() != effective.Length)
                throw new InvalidDataException("projection.component-adapter-duplicate");
            if (!DeclaredSchemas.SequenceEqual(effective.Select(row => row.schemaId), StringComparer.Ordinal))
                throw new InvalidDataException("projection.component-adapter-inventory-mismatch");

            foreach (var declared in DeclaredDescriptors.OrderBy(value => value.schemaId, StringComparer.Ordinal))
            {
                var adapter = effective.Single(value => value.schemaId == declared.schemaId);
                if (adapter.unityType != declared.unityType || !adapter.fields.SequenceEqual(declared.fields, StringComparer.Ordinal))
                    throw new InvalidDataException("projection.component-adapter-schema-mismatch");
            }

            var digest = DescriptorDigest(DeclaredDescriptors);
            return new ComponentInventory
            {
                schemaId = InventorySchema,
                digest = digest,
                declaredSchemas = DeclaredDescriptors.OrderBy(value => value.schemaId, StringComparer.Ordinal).ToArray(),
                effectiveAdapters = effective
            };
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

        internal static string ApplyRenderer(GameObject owner, string path, string guid, string localFileId, string contentSha256)
        {
            return Get<MeshRendererAdapter>(MeshRendererSchema).Apply(owner,
                ResolveAsset<Material>(path, guid, localFileId, contentSha256, "material"));
        }

        internal static string ApplyAnimator(GameObject owner, string objectId, string path, string guid, string localFileId, string contentSha256)
        {
            return Get<AnimatorAdapter>(AnimatorSchema).Apply(owner, objectId,
                ResolveAsset<AnimationClip>(path, guid, localFileId, contentSha256, "animation-clip"));
        }

        internal static void ApplyCanonicalLink(GameObject owner, string relation, string targetObjectId, GameObject target)
        {
            Get<CanonicalLinkAdapter>(CanonicalLinkSchema).Apply(owner, relation, targetObjectId, target);
        }

        internal static string ObserveReference(GameObject owner, string schemaId)
        {
            ValidateSchema(schemaId);
            if (schemaId == MeshRendererSchema) return Get<MeshRendererAdapter>(schemaId).Observe(owner);
            if (schemaId == AnimatorSchema) return Get<AnimatorAdapter>(schemaId).Observe(owner);
            if (schemaId == CanonicalLinkSchema) return Get<CanonicalLinkAdapter>(schemaId).Observe(owner);
            return Get<TransformAdapter>(schemaId).Observe(owner);
        }

        private static IH1ComponentAdapter[] BuildEffectiveAdapters()
        {
            var types = TypeCache.GetTypesDerivedFrom<IH1ComponentAdapter>()
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .OrderBy(type => type.FullName, StringComparer.Ordinal).ToArray();
            var adapters = new List<IH1ComponentAdapter>();
            foreach (var type in types)
            {
                var created = Activator.CreateInstance(type, true) as IH1ComponentAdapter;
                if (created == null) throw new InvalidDataException("projection.component-adapter-construction-failed");
                adapters.Add(created);
            }
            return adapters.ToArray();
        }

        private static T Get<T>(string schemaId) where T : class, IH1ComponentAdapter
        {
            ValidateSchemaWithoutRecursion(schemaId);
            var adapter = BuildEffectiveAdapters().SingleOrDefault(value => value.SchemaId == schemaId) as T;
            if (adapter == null) throw new InvalidDataException("projection.component-adapter-missing");
            return adapter;
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

        private static T ResolveSingleOwnedComponent<T>(GameObject owner) where T : Component
        {
            var matches = owner.GetComponentsInChildren<T>(true)
                .Where(component => !BelongsToNestedManagedObject(owner.transform, component.transform)).ToArray();
            if (matches.Length == 0) throw new InvalidDataException("projection.component-target-missing");
            if (matches.Length != 1) throw new InvalidDataException("projection.component-target-cardinality");
            return matches[0];
        }

        private static bool BelongsToNestedManagedObject(Transform owner, Transform candidate)
        {
            for (var current = candidate; current != null && current != owner; current = current.parent)
                if (current.GetComponent<H1ManagedMarker>() != null) return true;
            return false;
        }

        private static string RelativePath(Transform owner, Transform target)
        {
            if (owner == target) return "";
            var names = new List<string>();
            var current = target;
            while (current != null && current != owner)
            {
                names.Add(current.name);
                current = current.parent;
            }
            if (current != owner) throw new InvalidDataException("projection.component-target-outside-owner");
            names.Reverse();
            return string.Join("/", names.ToArray());
        }

        private static ComponentSchemaDescriptor Descriptor(string schemaId, Type unityType, params string[] fields)
        {
            return new ComponentSchemaDescriptor
            {
                schemaId = schemaId,
                unityType = unityType.FullName,
                fields = fields.OrderBy(value => value, StringComparer.Ordinal).ToArray()
            };
        }

        private static string DescriptorDigest(IEnumerable<ComponentSchemaDescriptor> descriptors)
        {
            var rows = descriptors.OrderBy(value => value.schemaId, StringComparer.Ordinal)
                .Select(value => value.schemaId + "|" + value.unityType + "|" + string.Join(",", value.fields));
            return Sha(string.Join("\n", rows));
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
            string[] Fields { get; }
            string Observe(GameObject owner);
        }

        private sealed class TransformAdapter : IH1ComponentAdapter
        {
            public string SchemaId => TransformSchema;
            public Type UnityType => typeof(Transform);
            public string[] Fields => new[] { "positionMm:vector3-int64", "rotationMilliDegrees:vector3-int64", "scalePpm:vector3-int64" };
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
            public string[] Fields => new[] { "enabled:bool=true", "material:catalogue-ref:material" };
            public string Apply(GameObject owner, Material material)
            {
                var renderer = ResolveSingleOwnedComponent<MeshRenderer>(owner);
                renderer.sharedMaterial = material;
                renderer.enabled = true;
                return RelativePath(owner.transform, renderer.transform);
            }
            public string Observe(GameObject owner)
            {
                var renderer = ResolveSingleOwnedComponent<MeshRenderer>(owner);
                if (renderer.sharedMaterial == null) throw new InvalidDataException("projection.component-target-missing");
                return SchemaId + "|enabled=" + renderer.enabled.ToString().ToLowerInvariant() + "|material=" + StableAssetIdentity(renderer.sharedMaterial);
            }
        }

        private sealed class AnimatorAdapter : IH1ComponentAdapter
        {
            public string SchemaId => AnimatorSchema;
            public Type UnityType => typeof(Animator);
            public string[] Fields => new[] { "applyRootMotion:bool=false", "clip:catalogue-ref:animation-clip", "cullingMode:enum=AlwaysAnimate", "updateMode:enum=Normal" };
            public string Apply(GameObject owner, string objectId, AnimationClip clip)
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
                    var motions = controller.animationClips.Distinct().ToArray();
                    if (motions.Length != 1 || motions[0] != clip) throw new InvalidDataException("projection.component-controller-drift");
                }
                animator.runtimeAnimatorController = controller;
                return "";
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
            public string[] Fields => new[] { "relation:string", "targetObjectId:canonical-ref" };
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
            public string digest;
            public ComponentSchemaDescriptor[] declaredSchemas;
            public ComponentAdapterRow[] effectiveAdapters;
        }
        [Serializable] internal sealed class ComponentSchemaDescriptor
        {
            public string schemaId;
            public string unityType;
            public string[] fields;
        }
        [Serializable] internal sealed class ComponentAdapterRow
        {
            public string schemaId;
            public string unityType;
            public string adapterType;
            public string[] fields;
        }
    }
}
