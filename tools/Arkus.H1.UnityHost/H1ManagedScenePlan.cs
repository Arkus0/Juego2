using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.World;

namespace Arkus.H1.UnityHost
{
    public sealed class H1ProjectionException : Exception
    {
        public H1ProjectionException(string code, string message) : base(message) { Code = code; }
        public string Code { get; }
    }

    // These DTOs are the versioned host-to-Editor plan. Native locators are replaceable
    // resolution hints; canonical identity and the content hash come only from WorldState.
    public sealed class H1ManagedScenePlan
    {
        public const string Schema = "arkus.h1-managed-scene-plan@1";
        public const string SceneId = "arkus.h1-05.scene.potes";
        public const int MaximumObjects = 128;

        public string SchemaId { get; set; } = Schema;
        public string SceneLogicalId { get; set; } = SceneId;
        public string WorldId { get; set; } = "";
        public long WorldRevision { get; set; }
        public string CanonicalHash { get; set; } = "";
        public string CatalogueFingerprint { get; set; } = "";
        public string InputDigest { get; set; } = "";
        public H1ManagedSceneNode[] Nodes { get; set; } = Array.Empty<H1ManagedSceneNode>();

        public static H1ManagedScenePlan Build(WorldState state, H1CatalogueSnapshot catalogue)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (catalogue == null) throw new ArgumentNullException(nameof(catalogue));
            var objects = state.Objects.ToDictionary(item => item.Id.Value, StringComparer.Ordinal);
            var nodes = new List<H1ManagedSceneNode>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var extension in state.Extensions)
            {
                if (extension.Owner != UnityBindingProducer.ExtensionOwner ||
                    extension.SchemaVersion != UnityBindingProducer.ExtensionSchemaVersion) continue;
                if (!extension.SubjectId.HasValue || !objects.TryGetValue(extension.SubjectId.Value.Value, out var subject))
                    throw Error("projection.subject-missing", "A Unity binding must belong to an existing canonical object.");
                if (!seen.Add(subject.Id.Value)) throw Error("projection.duplicate-binding", "One canonical object has duplicate Unity bindings.");
                var dependencies = extension.Dependencies.Select(reference => (object?)new Dictionary<string, object?>
                {
                    ["kind"] = reference.Kind.Value, ["targetId"] = reference.TargetId.Value
                }).ToArray();
                IReadOnlyDictionary<string, object?> inspected;
                try
                {
                    inspected = UnityBindingProducer.Inspect(new Dictionary<string, object?>
                    {
                        ["subjectId"] = subject.Id.Value,
                        ["payloadBase64"] = Convert.ToBase64String(extension.GetPayloadCopy()),
                        ["dependencies"] = dependencies
                    });
                }
                catch (UnityBindingException exception)
                {
                    throw Error("projection.binding-invalid", exception.MachineCode + " at " + exception.Path);
                }
                var binding = (IReadOnlyDictionary<string, object?>)inspected["binding"]!;
                if ((string)binding["targetSceneId"]! != SceneId)
                    throw Error("projection.scene-out-of-scope", "The binding targets a scene outside the fixed managed scene.");
                var source = (IReadOnlyDictionary<string, object?>)binding["source"]!;
                var sourceKind = (string)source["kind"]!;
                var sourceId = (string)source["logicalId"]!;
                H1CatalogueEntry resolved;
                try
                {
                    catalogue.Get(sourceId, sourceKind, true);
                    resolved = catalogue.Entries.Single(entry => entry.LogicalId == sourceId);
                }
                catch (H1CatalogueException exception)
                {
                    throw Error("projection.source-unavailable", exception.Code + ": " + exception.Message);
                }
                if (sourceKind != "prefab" && sourceKind != "asset")
                    throw Error("projection.source-kind", "Only admitted prefab or mesh asset sources may be materialized.");
                var transform = (IReadOnlyDictionary<string, object?>)binding["transform"]!;
                nodes.Add(new H1ManagedSceneNode
                {
                    ObjectId = subject.Id.Value,
                    ParentObjectId = subject.ContainerId?.Value ?? "",
                    SourceKind = sourceKind,
                    SourceLogicalId = sourceId,
                    SourcePath = resolved.Path,
                    SourceGuid = resolved.NativeGuid,
                    SourceLocalFileId = resolved.LocalFileId,
                    SourceContentSha256 = resolved.ContentSha256,
                    PositionMm = Vector((IReadOnlyDictionary<string, object?>)transform["positionMm"]!),
                    RotationMilliDegrees = Vector((IReadOnlyDictionary<string, object?>)transform["rotationMilliDegrees"]!),
                    ScalePpm = Vector((IReadOnlyDictionary<string, object?>)transform["scalePpm"]!)
                });
            }
            if (nodes.Count > MaximumObjects) throw Error("projection.object-limit", "The managed scene exceeds its reviewed object ceiling.");
            var bound = new HashSet<string>(nodes.Select(node => node.ObjectId), StringComparer.Ordinal);
            foreach (var node in nodes)
                if (node.ParentObjectId.Length != 0 && !bound.Contains(node.ParentObjectId))
                    throw Error("projection.parent-unbound", "Every managed child needs a managed canonical container in the same scene.");
            nodes.Sort((left, right) => StringComparer.Ordinal.Compare(left.ObjectId, right.ObjectId));
            var plan = new H1ManagedScenePlan
            {
                WorldId = state.Id.Value,
                WorldRevision = state.Revision,
                CanonicalHash = CanonicalWorldStateCodec.ComputeContentHash(state),
                CatalogueFingerprint = catalogue.Fingerprint,
                Nodes = nodes.ToArray()
            };
            plan.InputDigest = Sha(JsonSerializer.Serialize(new
            {
                plan.SchemaId, plan.SceneLogicalId, plan.WorldId, plan.WorldRevision,
                plan.CanonicalHash, plan.CatalogueFingerprint, plan.Nodes
            }));
            return plan;
        }

        public static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

        private static H1ProjectionVector Vector(IReadOnlyDictionary<string, object?> source) => new H1ProjectionVector
        {
            X = (long)source["x"]!, Y = (long)source["y"]!, Z = (long)source["z"]!
        };

        private static H1ProjectionException Error(string code, string message) => new H1ProjectionException(code, message);
    }

    public sealed class H1ProjectionVector
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
    }

    public sealed class H1ManagedSceneNode
    {
        public string ObjectId { get; set; } = "";
        public string ParentObjectId { get; set; } = "";
        public string SourceKind { get; set; } = "";
        public string SourceLogicalId { get; set; } = "";
        public string SourcePath { get; set; } = "";
        public string SourceGuid { get; set; } = "";
        public string SourceLocalFileId { get; set; } = "";
        public string SourceContentSha256 { get; set; } = "";
        public H1ProjectionVector PositionMm { get; set; } = new H1ProjectionVector();
        public H1ProjectionVector RotationMilliDegrees { get; set; } = new H1ProjectionVector();
        public H1ProjectionVector ScalePpm { get; set; } = new H1ProjectionVector();
    }
}
