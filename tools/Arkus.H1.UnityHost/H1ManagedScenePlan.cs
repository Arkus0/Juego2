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
                if (extension.Owner != UnityBindingProducer.ExtensionOwner) continue;
                if (extension.SchemaVersion != UnityBindingProducer.ExtensionSchemaVersion)
                    throw Error("projection.binding-version", "A Unity binding has an unsupported authored schema version.");
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
                var resolved = Resolve(catalogue, sourceId, sourceKind, MapSourceFailure);
                if (sourceKind != "prefab" && sourceKind != "asset")
                    throw Error("projection.source-kind", "Only admitted prefab or mesh asset sources may be materialized.");

                foreach (var raw in (IReadOnlyList<object?>)inspected["catalogueDependencies"]!)
                {
                    var reference = (IReadOnlyDictionary<string, object?>)raw!;
                    var kind = (string)reference["kind"]!;
                    if (kind == "scene") continue;
                    Resolve(catalogue, (string)reference["logicalId"]!, kind,
                        code => code == "catalogue.missing-reference" ? "projection.reference-missing" :
                            code == "catalogue.incompatible-reference" ? "projection.reference-wrong-type" : "projection.reference-rebound");
                }

                var transform = (IReadOnlyDictionary<string, object?>)binding["transform"]!;
                var components = new List<H1ComponentPlan>
                {
                    new H1ComponentPlan { SchemaId = H1ComponentSchemas.Transform, Kind = "transform" }
                };
                foreach (var raw in (IReadOnlyList<object?>)binding["components"]!)
                {
                    var component = (IReadOnlyDictionary<string, object?>)raw!;
                    var kind = (string)component["kind"]!;
                    if (kind == "renderer")
                    {
                        var materialId = (string)component["materialId"]!;
                        var material = Resolve(catalogue, materialId, "material", MapReferenceFailure);
                        components.Add(new H1ComponentPlan
                        {
                            SchemaId = H1ComponentSchemas.MeshRenderer,
                            Kind = kind,
                            ReferenceKind = "material",
                            ReferenceLogicalId = materialId,
                            ReferencePath = material.Path,
                            ReferenceGuid = material.NativeGuid,
                            ReferenceLocalFileId = material.LocalFileId,
                            ReferenceContentSha256 = material.ContentSha256
                        });
                    }
                    else if (kind == "animator")
                    {
                        var clipId = (string)component["clipId"]!;
                        var clip = Resolve(catalogue, clipId, "animation-clip", MapReferenceFailure);
                        components.Add(new H1ComponentPlan
                        {
                            SchemaId = H1ComponentSchemas.Animator,
                            Kind = kind,
                            ReferenceKind = "animation-clip",
                            ReferenceLogicalId = clipId,
                            ReferencePath = clip.Path,
                            ReferenceGuid = clip.NativeGuid,
                            ReferenceLocalFileId = clip.LocalFileId,
                            ReferenceContentSha256 = clip.ContentSha256
                        });
                    }
                    else if (kind == "canonical-link")
                    {
                        var targetObjectId = (string)component["targetObjectId"]!;
                        if (!objects.ContainsKey(targetObjectId))
                            throw Error("projection.canonical-target-missing", "A canonical component reference targets an object absent from the canonical state.");
                        components.Add(new H1ComponentPlan
                        {
                            SchemaId = H1ComponentSchemas.CanonicalLink,
                            Kind = kind,
                            Relation = (string)component["relation"]!,
                            TargetObjectId = targetObjectId
                        });
                    }
                    else
                    {
                        throw Error("projection.component-not-allowlisted", "The binding contains a component with no H1-07 adapter schema.");
                    }
                }

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
                    ScalePpm = Vector((IReadOnlyDictionary<string, object?>)transform["scalePpm"]!),
                    Components = components.OrderBy(value => value.SortKey, StringComparer.Ordinal).ToArray()
                });
            }
            if (nodes.Count > MaximumObjects) throw Error("projection.object-limit", "The managed scene exceeds its reviewed object ceiling.");
            var bound = new HashSet<string>(nodes.Select(node => node.ObjectId), StringComparer.Ordinal);
            foreach (var node in nodes)
            {
                if (node.ParentObjectId.Length != 0 && !bound.Contains(node.ParentObjectId))
                    throw Error("projection.parent-unbound", "Every managed child needs a managed canonical container in the same scene.");
                foreach (var component in node.Components)
                    if (component.Kind == "canonical-link" && !bound.Contains(component.TargetObjectId))
                        throw Error("projection.canonical-target-unbound", "Every realized canonical component reference must target a managed object in the same scene.");
            }
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

        private static H1CatalogueEntry Resolve(H1CatalogueSnapshot catalogue, string logicalId, string kind, Func<string, string> mapFailure)
        {
            try
            {
                catalogue.Get(logicalId, kind, true);
                return catalogue.Entries.Single(entry => entry.LogicalId == logicalId);
            }
            catch (H1CatalogueException exception)
            {
                throw Error(mapFailure(exception.Code), exception.Code + ": " + exception.Message);
            }
        }

        private static string MapSourceFailure(string code)
        {
            if (code == "catalogue.missing-reference") return "projection.source-missing";
            if (code == "catalogue.incompatible-reference") return "projection.source-wrong-type";
            if (code == "catalogue.stale-mapping" || code == "catalogue.incompatible-mapping") return "projection.source-rebound";
            return "projection.source-unavailable";
        }

        private static string MapReferenceFailure(string code)
        {
            if (code == "catalogue.missing-reference") return "projection.component-reference-missing";
            if (code == "catalogue.incompatible-reference") return "projection.component-reference-wrong-type";
            if (code == "catalogue.stale-mapping" || code == "catalogue.incompatible-mapping") return "projection.component-reference-rebound";
            return "projection.component-reference-unavailable";
        }

        private static H1ProjectionVector Vector(IReadOnlyDictionary<string, object?> source) => new H1ProjectionVector
        {
            X = (long)source["x"]!, Y = (long)source["y"]!, Z = (long)source["z"]!
        };

        private static H1ProjectionException Error(string code, string message) => new H1ProjectionException(code, message);
    }

    public static class H1ComponentSchemas
    {
        public const string Transform = "arkus.h1.component.transform@1";
        public const string MeshRenderer = "arkus.h1.component.mesh-renderer@1";
        public const string Animator = "arkus.h1.component.animator@1";
        public const string CanonicalLink = "arkus.h1.component.canonical-link@1";
        public static readonly string[] Required = { Animator, CanonicalLink, MeshRenderer, Transform };
    }

    public sealed class H1ProjectionVector
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
    }

    public sealed class H1ComponentPlan
    {
        public string SchemaId { get; set; } = "";
        public string Kind { get; set; } = "";
        public string Relation { get; set; } = "";
        public string TargetObjectId { get; set; } = "";
        public string ReferenceKind { get; set; } = "";
        public string ReferenceLogicalId { get; set; } = "";
        public string ReferencePath { get; set; } = "";
        public string ReferenceGuid { get; set; } = "";
        public string ReferenceLocalFileId { get; set; } = "";
        public string ReferenceContentSha256 { get; set; } = "";
        public string SortKey => SchemaId + "\u001f" + Relation + "\u001f" + TargetObjectId + "\u001f" + ReferenceLogicalId;
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
        public H1ComponentPlan[] Components { get; set; } = Array.Empty<H1ComponentPlan>();
    }
}