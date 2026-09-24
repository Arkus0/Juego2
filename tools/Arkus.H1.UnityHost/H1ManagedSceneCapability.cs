using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Arkus.Game.Authoring;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    public sealed class H1ManagedSceneExecutor : IH1UnityCapabilityExecutor
    {
        public static readonly CapabilityKey MaterializeKey = new CapabilityKey("unity.host.projection.materialize", new ContractVersion(1, 0));
        public static readonly CapabilityKey ObserveKey = new CapabilityKey("unity.host.projection.observe", new ContractVersion(1, 0));
        public const string MaterializeExecutorId = "arkus.h1.worker.projection.materialize@1";
        public const string ObserveExecutorId = "arkus.h1.worker.projection.observe@1";
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };
        private readonly IWorldStateSource _world;
        private readonly H1UnityLaunchProfile _profile;
        private H1ManagedScenePlan? _pendingPlan;

        public H1ManagedSceneExecutor(CapabilityKey capability, string executorId, IWorldStateSource world, H1UnityLaunchProfile profile)
        {
            Capability = capability;
            ExecutorId = executorId;
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
        }
        public CapabilityKey Capability { get; }
        public string ExecutorId { get; }

        public string EncodeRequest(IReadOnlyDictionary<string, object?> request)
        {
            H1ProjectionContract.RequireScene(request);
            var plan = H1ProjectionContract.Plan(_world, _profile);
            _pendingPlan = plan;
            return JsonSerializer.Serialize(new
            {
                schemaId = "arkus.h1-projection-worker-request@1",
                mode = Capability.Equals(MaterializeKey) ? "materialize" : "observe",
                sceneLogicalId = H1ManagedScenePlan.SceneId,
                plan
            }, Json);
        }

        public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result)
        {
            try
            {
                var expected = _pendingPlan ?? throw new JsonException("No pending projection plan.");
                var reply = JsonSerializer.Deserialize<H1ManagedSceneWorkerReply>(result.Payload, Json) ?? throw new JsonException("Missing projection reply.");
                if (reply.SchemaId != "arkus.h1-projection-worker-result@1" || reply.SceneLogicalId != H1ManagedScenePlan.SceneId ||
                    reply.ExpectedInputDigest != expected.InputDigest || reply.Observation == null)
                    throw new JsonException("Projection reply identity is wrong.");
                if (reply.ErrorCode.Length != 0)
                    return Failure(reply.ErrorCode, "Unity rejected the staged managed-scene projection.");
                var observation = reply.Observation;
                if (observation.SchemaId != "arkus.h1-managed-scene-observation@1" ||
                    observation.SceneLogicalId != H1ManagedScenePlan.SceneId ||
                    observation.Nodes == null || observation.Nodes.Length > H1ManagedScenePlan.MaximumObjects)
                    throw new JsonException("Projection observation is malformed.");
                var currentHash = Arkus.Game.World.CanonicalWorldStateCodec.ComputeContentHash(_world.Current);
                var current = observation.Active && observation.InputDigest == expected.InputDigest &&
                    observation.CanonicalHash == expected.CanonicalHash &&
                    observation.CatalogueFingerprint == expected.CatalogueFingerprint &&
                    currentHash == expected.CanonicalHash;
                if (Capability.Equals(MaterializeKey))
                {
                    if (!current || observation.Nodes.Length != expected.Nodes.Length || !SameGraph(expected.Nodes, observation.Nodes))
                        return Failure("projection.observation-mismatch", "Effective scene observation does not equal the canonical projection plan.");
                }
                var data = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["schemaId"] = observation.SchemaId,
                    ["sceneLogicalId"] = observation.SceneLogicalId,
                    ["active"] = observation.Active,
                    ["current"] = current,
                    ["generationId"] = observation.GenerationId,
                    ["inputDigest"] = observation.InputDigest,
                    ["canonicalHash"] = observation.CanonicalHash,
                    ["catalogueFingerprint"] = observation.CatalogueFingerprint,
                    ["graphDigest"] = observation.GraphDigest,
                    ["nodes"] = observation.Nodes.Select(node => (object?)new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["objectId"] = node.ObjectId, ["parentObjectId"] = node.ParentObjectId,
                        ["sourceLogicalId"] = node.SourceLogicalId,
                        ["positionMm"] = VectorData(node.PositionMm),
                        ["rotationMilliDegrees"] = VectorData(node.RotationMilliDegrees),
                        ["scalePpm"] = VectorData(node.ScalePpm)
                    })).ToArray()
                };
                return CapabilityInvocationResult.Succeeded(new ReadOnlyDictionary<string, object?>(data));
            }
            catch (JsonException)
            {
                return Failure("projection.corrupt-observation", "Unity returned a malformed projection observation.");
            }
            finally { _pendingPlan = null; }
        }

        private static bool SameGraph(IReadOnlyList<H1ManagedSceneNode> expected, IReadOnlyList<H1ObservedSceneNode> actual)
        {
            var ordered = actual.OrderBy(node => node.ObjectId, StringComparer.Ordinal).ToArray();
            for (var i = 0; i < expected.Count; i++)
            {
                if (expected[i].ObjectId != ordered[i].ObjectId ||
                    expected[i].ParentObjectId != ordered[i].ParentObjectId ||
                    expected[i].SourceLogicalId != ordered[i].SourceLogicalId ||
                    !Equal(expected[i].PositionMm, ordered[i].PositionMm) ||
                    !Equal(expected[i].ScalePpm, ordered[i].ScalePpm)) return false;
            }
            return true;
        }

        private static bool Equal(H1ProjectionVector left, H1ProjectionVector right) =>
            left.X == right.X && left.Y == right.Y && left.Z == right.Z;

        private static IReadOnlyDictionary<string, object?> VectorData(H1ProjectionVector value) =>
            new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            { ["x"] = value.X, ["y"] = value.Y, ["z"] = value.Z });

        private static CapabilityInvocationResult Failure(string code, string message) =>
            CapabilityInvocationResult.Failed(new StructuredError(code, message, "$",
                new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)),
                false, "Inspect the active generation and retry after repairing the canonical binding or effective Unity input."));
    }

    public sealed class H1ManagedSceneWorkerReply
    {
        public string SchemaId { get; set; } = "";
        public string SceneLogicalId { get; set; } = "";
        public string ExpectedInputDigest { get; set; } = "";
        public string ErrorCode { get; set; } = "";
        public H1ManagedSceneObservation? Observation { get; set; }
    }

    public sealed class H1ManagedSceneObservation
    {
        public string SchemaId { get; set; } = "";
        public string SceneLogicalId { get; set; } = "";
        public bool Active { get; set; }
        public string GenerationId { get; set; } = "";
        public string InputDigest { get; set; } = "";
        public string CanonicalHash { get; set; } = "";
        public string CatalogueFingerprint { get; set; } = "";
        public string GraphDigest { get; set; } = "";
        public H1ObservedSceneNode[] Nodes { get; set; } = Array.Empty<H1ObservedSceneNode>();
    }

    public sealed class H1ObservedSceneNode
    {
        public string ObjectId { get; set; } = "";
        public string ParentObjectId { get; set; } = "";
        public string SourceLogicalId { get; set; } = "";
        public H1ProjectionVector PositionMm { get; set; } = new H1ProjectionVector();
        public H1ProjectionVector RotationMilliDegrees { get; set; } = new H1ProjectionVector();
        public H1ProjectionVector ScalePpm { get; set; } = new H1ProjectionVector();
    }

    [PublicCapabilityRoute("arkus.unity-projection", "unity.projection.plan", "1.0")]
    public sealed class H1ManagedScenePlanHandler : ICanonicalCapabilityHandler
    {
        private readonly IWorldStateSource _world;
        private readonly H1UnityLaunchProfile _profile;
        public H1ManagedScenePlanHandler(IWorldStateSource world, H1UnityLaunchProfile profile) { _world = world; _profile = profile; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
        {
            try
            {
                H1ProjectionContract.RequireScene(request);
                var plan = H1ProjectionContract.Plan(_world, _profile);
                return CapabilityInvocationResult.Succeeded(new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["schemaId"] = H1ManagedScenePlan.Schema,
                    ["sceneLogicalId"] = plan.SceneLogicalId,
                    ["worldId"] = plan.WorldId,
                    ["worldRevision"] = plan.WorldRevision,
                    ["canonicalHash"] = plan.CanonicalHash,
                    ["catalogueFingerprint"] = plan.CatalogueFingerprint,
                    ["inputDigest"] = plan.InputDigest,
                    ["objectIds"] = plan.Nodes.Select(node => (object?)node.ObjectId).ToArray()
                }));
            }
            catch (H1ProjectionException exception)
            {
                return CapabilityInvocationResult.Failed(new StructuredError(exception.Code, exception.Message, "$",
                    new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)), false,
                    "Repair the canonical binding or catalogue input before materialization."));
            }
        }
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.projection.materialize", "1.0")]
    public sealed class H1ManagedSceneMaterializeHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1ManagedSceneMaterializeHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.projection.observe", "1.0")]
    public sealed class H1ManagedSceneObserveHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1ManagedSceneObserveHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    public static class H1ProjectionContract
    {
        public const string ReferenceNamespace = "ref.arkus.unity-host.projection";
        private static readonly CapabilityKey PlanKey = new CapabilityKey("unity.projection.plan", new ContractVersion(1, 0));

        public static H1ManagedScenePlan Plan(IWorldStateSource world, H1UnityLaunchProfile profile)
        {
            try
            {
                var root = profile.RepositoryRoot;
                var snapshot = H1CatalogueSnapshot.Build(
                    File.ReadAllText(Path.Combine(root, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
                    File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath)),
                    File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath)));
                return H1ManagedScenePlan.Build(world.Current, snapshot);
            }
            catch (IOException exception) { throw new H1ProjectionException("projection.catalogue-unavailable", exception.Message); }
            catch (H1CatalogueException exception) { throw new H1ProjectionException("projection.catalogue-unavailable", exception.Code + ": " + exception.Message); }
        }

        public static void RequireScene(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null || !request.TryGetValue("sceneLogicalId", out var raw) ||
                raw is not string scene || scene != H1ManagedScenePlan.SceneId)
                throw new H1ProjectionException("projection.scene-out-of-scope", "Only the fixed reviewed managed scene is admitted.");
        }

        public static CanonicalProviderContribution PlanContribution(IWorldStateSource world, H1UnityLaunchProfile profile)
        {
            var definition = new CapabilityDefinition(PlanKey,
                new ProviderMetadata("arkus.unity-projection", ProviderKind.Scoped, "unity-projection", "unity.projection"),
                PlanRequest(), PlanSuccess(), CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly, DeterminismClass.EnvironmentDependent,
                new[] { "canonical-snapshot-and-accepted-catalogue-bound" },
                new[] { "normalized-managed-scene-plan-returned", "canonical-state-unchanged" },
                new ConcurrencySemantics(ConcurrencyClass.Serialized), new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported), new RepairSemantics(false, true),
                new PolicySemantics(PrivilegeClass.Authoring, TransactionRequirement.ReadOnlyEnvelope, ProvenanceRequirement.Required),
                new CostSemantics(1, "portable canonical binding and accepted catalogue planning"));
            return new CanonicalProviderContribution(
                new ProviderDescriptor("arkus.unity-projection", ProviderKind.Scoped, "unity-projection", new[] { "unity.projection" }),
                new[] { definition }, new[] { CapabilityRoute.FromHandler(new H1ManagedScenePlanHandler(world, profile)) });
        }

        public static IReadOnlyList<CapabilityDefinition> EditorDefinitions() => new[]
        {
            EditorDefinition(H1ManagedSceneExecutor.MaterializeKey, SideEffectClass.ExternalReversible, ObservationSuccess(), "staged managed-scene publication"),
            EditorDefinition(H1ManagedSceneExecutor.ObserveKey, SideEffectClass.ReadOnly, ObservationSuccess(), "normalized effective managed-scene observation")
        };

        public static IReadOnlyList<CapabilityRoute> EditorRoutes(H1UnityEditorExecutionCoordinator coordinator) => new[]
        {
            CapabilityRoute.FromHandler(new H1ManagedSceneMaterializeHandler(coordinator)),
            CapabilityRoute.FromHandler(new H1ManagedSceneObserveHandler(coordinator))
        };

        public static IReadOnlyList<UnityHostCapabilityGrant> Grants() => new[]
        {
            new UnityHostCapabilityGrant(H1ManagedSceneExecutor.MaterializeKey, UnityProjectWorkspaceAuthority.ManagedAssetsRootId,
                ReferenceNamespace, UnityHostResourceClass.ManagedAsset, UnityHostTimeClass.BoundedEditorEffect),
            new UnityHostCapabilityGrant(H1ManagedSceneExecutor.ObserveKey, UnityProjectWorkspaceAuthority.ManagedAssetsRootId,
                ReferenceNamespace, UnityHostResourceClass.ManagedAsset, UnityHostTimeClass.BoundedRead)
        };

        private static CapabilityDefinition EditorDefinition(CapabilityKey key, SideEffectClass effect, JsonSchemaDocument success, string cost) =>
            new CapabilityDefinition(key,
                new ProviderMetadata(H1UnityHostCapabilityPolicy.HostProviderId, ProviderKind.Scoped,
                    H1UnityHostCapabilityPolicy.HostScope, H1UnityHostCapabilityPolicy.HostNamespace),
                Request(), success, CanonicalContractSchemas.StructuredError(),
                effect, DeterminismClass.EnvironmentDependent,
                new[] { "fixed-managed-scene", "canonical-and-catalogue-inputs-bound" },
                new[] { "effective-scene-observed", "canonical-state-unchanged" },
                new ConcurrencySemantics(ConcurrencyClass.Serialized), new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported), new RepairSemantics(true, true),
                new PolicySemantics(PrivilegeClass.Authoring, TransactionRequirement.None, ProvenanceRequirement.Required),
                new CostSemantics(1, "one short-lived Unity Editor worker: " + cost));

        private static JsonSchemaDocument Request() => new JsonSchemaDocument(SchemaNode.Object(
            new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["sceneLogicalId"] = SchemaNode.String(format: "arkus-logical-reference", logicalReferenceNamespace: ReferenceNamespace)
            }, new[] { "sceneLogicalId" }));

        private static JsonSchemaDocument PlanRequest() => new JsonSchemaDocument(SchemaNode.Object(
            new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            { ["sceneLogicalId"] = SchemaNode.String(new[] { H1ManagedScenePlan.SceneId }) },
            new[] { "sceneLogicalId" }));

        private static JsonSchemaDocument PlanSuccess() => new JsonSchemaDocument(SchemaNode.Object(
            new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["schemaId"] = SchemaNode.String(new[] { H1ManagedScenePlan.Schema }),
                ["sceneLogicalId"] = SchemaNode.String(), ["worldId"] = SchemaNode.String(),
                ["worldRevision"] = SchemaNode.Integer(), ["canonicalHash"] = SchemaNode.String(),
                ["catalogueFingerprint"] = SchemaNode.String(), ["inputDigest"] = SchemaNode.String(),
                ["objectIds"] = SchemaNode.Array(SchemaNode.String())
            }, new[] { "schemaId", "sceneLogicalId", "worldId", "worldRevision", "canonicalHash", "catalogueFingerprint", "inputDigest", "objectIds" }));

        private static SchemaNode Vector() => SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        { ["x"] = SchemaNode.Integer(), ["y"] = SchemaNode.Integer(), ["z"] = SchemaNode.Integer() }, new[] { "x", "y", "z" });

        private static JsonSchemaDocument ObservationSuccess() => new JsonSchemaDocument(SchemaNode.Object(
            new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-managed-scene-observation@1" }),
                ["sceneLogicalId"] = SchemaNode.String(), ["active"] = SchemaNode.Boolean(), ["current"] = SchemaNode.Boolean(),
                ["generationId"] = SchemaNode.String(), ["inputDigest"] = SchemaNode.String(),
                ["canonicalHash"] = SchemaNode.String(), ["catalogueFingerprint"] = SchemaNode.String(),
                ["graphDigest"] = SchemaNode.String(),
                ["nodes"] = SchemaNode.Array(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["objectId"] = SchemaNode.String(), ["parentObjectId"] = SchemaNode.String(),
                    ["sourceLogicalId"] = SchemaNode.String(), ["positionMm"] = Vector(),
                    ["rotationMilliDegrees"] = Vector(), ["scalePpm"] = Vector()
                }, new[] { "objectId", "parentObjectId", "sourceLogicalId", "positionMm", "rotationMilliDegrees", "scalePpm" }))
            }, new[] { "schemaId", "sceneLogicalId", "active", "current", "generationId", "inputDigest", "canonicalHash", "catalogueFingerprint", "graphDigest", "nodes" }));
    }
}
