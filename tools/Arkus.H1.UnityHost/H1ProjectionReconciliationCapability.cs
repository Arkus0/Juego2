using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    public sealed class H1ProjectionRematerializeExecutor : IH1UnityCapabilityExecutor
    {
        public static readonly CapabilityKey Key = new CapabilityKey("unity.host.projection.rematerialize", new ContractVersion(1, 0));
        private readonly H1ManagedSceneExecutor _inner;
        public H1ProjectionRematerializeExecutor(IWorldStateSource world, H1UnityLaunchProfile profile)
        {
            _inner = new H1ManagedSceneExecutor(H1ManagedSceneExecutor.MaterializeKey, H1ManagedSceneExecutor.MaterializeExecutorId, world, profile);
        }
        public CapabilityKey Capability => Key;
        public string ExecutorId => H1ManagedSceneExecutor.MaterializeExecutorId;
        public string EncodeRequest(IReadOnlyDictionary<string, object?> request) => _inner.EncodeRequest(request);
        public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result) => _inner.DecodeResult(result);
    }

    public sealed class H1ProjectionReconciliationExecutor : IH1UnityCapabilityExecutor
    {
        public static readonly CapabilityKey DriftKey = new CapabilityKey("unity.host.projection.drift", new ContractVersion(1, 0));
        public static readonly CapabilityKey ImportProposalKey = new CapabilityKey("unity.host.projection.import-proposal", new ContractVersion(1, 0));
        public const string WorkerExecutorId = "arkus.h1.worker.projection.reconcile@1";
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };
        private readonly IWorldStateSource _world;
        private readonly H1UnityLaunchProfile _profile;
        private H1ManagedScenePlan? _pendingPlan;
        private H1CatalogueSnapshot? _pendingCatalogue;
        private string _pendingPlanningError = "";

        public H1ProjectionReconciliationExecutor(CapabilityKey capability, IWorldStateSource world, H1UnityLaunchProfile profile)
        {
            if (!capability.Equals(DriftKey) && !capability.Equals(ImportProposalKey)) throw new ArgumentException("Unsupported H1-09 capability.", nameof(capability));
            Capability = capability;
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
        }

        public CapabilityKey Capability { get; }
        public string ExecutorId => WorkerExecutorId;

        public string EncodeRequest(IReadOnlyDictionary<string, object?> request)
        {
            H1ProjectionContract.RequireScene(request);
            _pendingPlan = null;
            _pendingCatalogue = null;
            _pendingPlanningError = "";
            try
            {
                _pendingCatalogue = LoadCatalogue(_profile);
                _pendingPlan = H1ManagedScenePlan.Build(_world.Current, _pendingCatalogue);
            }
            catch (IOException exception)
            {
                _pendingPlanningError = "projection.catalogue-unavailable:" + exception.GetType().Name;
            }
            catch (H1CatalogueException exception)
            {
                _pendingPlanningError = H1ProjectionContract.MapCatalogueFailure(exception.Code) + ":" + exception.Code;
            }
            catch (H1ProjectionException exception)
            {
                _pendingPlanningError = exception.Code;
            }
            return JsonSerializer.Serialize(new
            {
                schemaId = "arkus.h1-projection-reconciliation-worker-request@1",
                sceneLogicalId = H1ManagedScenePlan.SceneId
            }, Json);
        }

        public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result)
        {
            try
            {
                var reply = JsonSerializer.Deserialize<H1ProjectionReconciliationWorkerReply>(result.Payload, Json) ?? throw new JsonException("Missing reconciliation reply.");
                if (reply.SchemaId != "arkus.h1-projection-reconciliation-worker-result@1" || reply.SceneLogicalId != H1ManagedScenePlan.SceneId || reply.Observation == null)
                    throw new JsonException("Reconciliation reply identity is wrong.");
                if (reply.ErrorCode.Length != 0) return Failure(reply.ErrorCode, "Unity could not observe the managed reconciliation scope.");
                ValidateObservation(reply.Observation);

                var report = _pendingPlan == null
                    ? CatalogueDriftReport(_world.Current, reply.Observation, _pendingPlanningError)
                    : H1ProjectionReconciliation.Compare(_pendingPlan, reply.Observation);
                if (Capability.Equals(DriftKey))
                    return CapabilityInvocationResult.Succeeded(ReportData(report));

                var proposal = _pendingPlan == null || _pendingCatalogue == null
                    ? new H1ProjectionImportProposal { Available = false, Diagnostics = new[] { "projection.import-blocked-catalogue-drift:$scene" } }
                    : H1ProjectionReconciliation.CompileProposal(_pendingPlan, reply.Observation, report, _pendingCatalogue);
                return CapabilityInvocationResult.Succeeded(ProposalData(report, proposal));
            }
            catch (JsonException)
            {
                return Failure("projection.corrupt-reconciliation-observation", "Unity returned a malformed reconciliation observation.");
            }
            finally
            {
                _pendingPlan = null;
                _pendingCatalogue = null;
                _pendingPlanningError = "";
            }
        }

        internal static H1CatalogueSnapshot LoadCatalogue(H1UnityLaunchProfile profile)
        {
            var root = profile.RepositoryRoot;
            return H1CatalogueSnapshot.Build(
                File.ReadAllText(Path.Combine(root, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath)),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath)));
        }

        private static void ValidateObservation(H1ProjectionReconciliationObservation observation)
        {
            if (observation.SchemaId != H1ProjectionReconciliationObservation.Schema || observation.SceneLogicalId != H1ManagedScenePlan.SceneId ||
                observation.Nodes == null || observation.Nodes.Length > H1ManagedScenePlan.MaximumObjects * 2 || observation.UnmanagedPaths == null ||
                observation.UnmanagedPaths.Length > H1ManagedScenePlan.MaximumObjects * 4 || observation.Diagnostics == null || observation.Diagnostics.Length > 256 ||
                observation.Nodes.Any(node => node == null || node.ComponentRows == null || node.ComponentRows.Length > 16 || node.Relationships == null || node.Relationships.Length > 512))
                throw new JsonException("Reconciliation observation exceeds its bounded shape.");
        }

        private static H1ProjectionDriftReport CatalogueDriftReport(WorldState state, H1ProjectionReconciliationObservation observation, string code)
        {
            var canonicalHash = CanonicalWorldStateCodec.ComputeContentHash(state);
            return new H1ProjectionDriftReport
            {
                State = "missing-dependency",
                Parity = false,
                ExpectedRevision = state.Revision,
                ExpectedCanonicalHash = canonicalHash,
                EffectiveCanonicalHash = observation.CanonicalHash,
                EffectiveInputDigest = observation.InputDigest,
                EffectiveCatalogueFingerprint = observation.CatalogueFingerprint,
                ManagedDigest = H1ProjectionReconciliation.ManagedDigest(observation.Nodes),
                Items = new[] { new H1ProjectionDriftItem { Classification = H1ProjectionDriftClass.CatalogueDrift, ObjectId = "$scene", Managed = true, Fields = new[] { "catalogue", code.Length == 0 ? "projection.catalogue-unavailable" : code } } },
                Diagnostics = observation.Diagnostics.OrderBy(value => value.Code, StringComparer.Ordinal).ThenBy(value => value.Subject, StringComparer.Ordinal).ToArray()
            };
        }

        internal static IReadOnlyDictionary<string, object?> ReportData(H1ProjectionDriftReport report)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = report.SchemaId,
                ["sceneLogicalId"] = report.SceneLogicalId,
                ["state"] = report.State,
                ["parity"] = report.Parity,
                ["expectedRevision"] = report.ExpectedRevision,
                ["expectedCanonicalHash"] = report.ExpectedCanonicalHash,
                ["expectedInputDigest"] = report.ExpectedInputDigest,
                ["expectedCatalogueFingerprint"] = report.ExpectedCatalogueFingerprint,
                ["effectiveCanonicalHash"] = report.EffectiveCanonicalHash,
                ["effectiveInputDigest"] = report.EffectiveInputDigest,
                ["effectiveCatalogueFingerprint"] = report.EffectiveCatalogueFingerprint,
                ["managedDigest"] = report.ManagedDigest,
                ["items"] = report.Items.Select(item => (object?)ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["classification"] = item.Classification, ["objectId"] = item.ObjectId, ["managed"] = item.Managed,
                    ["fields"] = item.Fields.Cast<object?>().ToArray()
                })).ToArray(),
                ["diagnostics"] = report.Diagnostics.Select(value => (object?)ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                { ["code"] = value.Code, ["subject"] = value.Subject })).ToArray()
            });
        }

        private static IReadOnlyDictionary<string, object?> ProposalData(H1ProjectionDriftReport report, H1ProjectionImportProposal proposal)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = proposal.SchemaId,
                ["sceneLogicalId"] = proposal.SceneLogicalId,
                ["available"] = proposal.Available,
                ["objectIds"] = proposal.ObjectIds.Cast<object?>().ToArray(),
                ["diagnostics"] = proposal.Diagnostics.Cast<object?>().ToArray(),
                ["mutationRequest"] = proposal.MutationRequest,
                ["drift"] = ReportData(report)
            });
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> value) => new ReadOnlyDictionary<string, object?>(value);
        private static CapabilityInvocationResult Failure(string code, string message) => CapabilityInvocationResult.Failed(new StructuredError(
            code, message, "$", new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)), false,
            "Inspect the H1-09 drift report and repair or rematerialize before retrying."));
    }

    public sealed class H1ProjectionReconciliationWorkerReply
    {
        public string SchemaId { get; set; } = "";
        public string SceneLogicalId { get; set; } = "";
        public string ErrorCode { get; set; } = "";
        public H1ProjectionReconciliationObservation? Observation { get; set; }
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.projection.drift", "1.0")]
    public sealed class H1ProjectionDriftHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1ProjectionDriftHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.projection.rematerialize", "1.0")]
    public sealed class H1ProjectionRematerializeHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1ProjectionRematerializeHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.projection.import-proposal", "1.0")]
    public sealed class H1ProjectionImportProposalHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1ProjectionImportProposalHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    public static class H1ProjectionReconciliationContract
    {
        public static IReadOnlyList<CapabilityDefinition> Definitions() => new[]
        {
            Definition(H1ProjectionReconciliationExecutor.DriftKey, SideEffectClass.ReadOnly, DriftSchema(), "effective managed drift observation and deterministic classification"),
            Definition(H1ProjectionRematerializeExecutor.Key, SideEffectClass.ExternalReversible, RematerializeSchema(), "canonical-authoritative managed-scene rematerialization"),
            Definition(H1ProjectionReconciliationExecutor.ImportProposalKey, SideEffectClass.ReadOnly, ProposalSchema(), "allowlisted effective Unity edit compilation into an H0 mutation proposal")
        };

        public static IReadOnlyList<CapabilityRoute> Routes(H1UnityEditorExecutionCoordinator coordinator) => new[]
        {
            CapabilityRoute.FromHandler(new H1ProjectionDriftHandler(coordinator)),
            CapabilityRoute.FromHandler(new H1ProjectionRematerializeHandler(coordinator)),
            CapabilityRoute.FromHandler(new H1ProjectionImportProposalHandler(coordinator))
        };

        public static IReadOnlyList<UnityHostCapabilityGrant> Grants() => new[]
        {
            new UnityHostCapabilityGrant(H1ProjectionReconciliationExecutor.DriftKey, UnityProjectWorkspaceAuthority.ManagedAssetsRootId, H1ProjectionContract.ReferenceNamespace, UnityHostResourceClass.ManagedAsset, UnityHostTimeClass.BoundedRead),
            new UnityHostCapabilityGrant(H1ProjectionRematerializeExecutor.Key, UnityProjectWorkspaceAuthority.ManagedAssetsRootId, H1ProjectionContract.ReferenceNamespace, UnityHostResourceClass.ManagedAsset, UnityHostTimeClass.BoundedEditorEffect),
            new UnityHostCapabilityGrant(H1ProjectionReconciliationExecutor.ImportProposalKey, UnityProjectWorkspaceAuthority.ManagedAssetsRootId, H1ProjectionContract.ReferenceNamespace, UnityHostResourceClass.ManagedAsset, UnityHostTimeClass.BoundedRead)
        };

        private static CapabilityDefinition Definition(CapabilityKey key, SideEffectClass effect, JsonSchemaDocument success, string cost) => new CapabilityDefinition(
            key,
            new ProviderMetadata(H1UnityHostCapabilityPolicy.HostProviderId, ProviderKind.Scoped, H1UnityHostCapabilityPolicy.HostScope, H1UnityHostCapabilityPolicy.HostNamespace),
            Request(), success, CanonicalContractSchemas.StructuredError(), effect, DeterminismClass.EnvironmentDependent,
            new[] { "fixed-managed-scene", "effective-unity-observation-through-h1-03a", "canonical-authority-remains-h0" },
            new[] { "canonical-state-unchanged-unless-separately-accepted-by-h0", "normalized-reconciliation-outcome-returned" },
            new ConcurrencySemantics(ConcurrencyClass.Serialized), new IdempotencySemantics(IdempotencyClass.Idempotent),
            new BatchingSemantics(BatchingClass.Unsupported), new RepairSemantics(true, true),
            new PolicySemantics(PrivilegeClass.Authoring, TransactionRequirement.None, ProvenanceRequirement.Required),
            new CostSemantics(1, "one short-lived Unity Editor worker: " + cost));

        private static JsonSchemaDocument Request() => new JsonSchemaDocument(SchemaNode.Object(
            new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            { ["sceneLogicalId"] = SchemaNode.String(format: "arkus-logical-reference", logicalReferenceNamespace: H1ProjectionContract.ReferenceNamespace) },
            new[] { "sceneLogicalId" }));

        private static SchemaNode DriftNode() => SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { H1ProjectionDriftReport.Schema }), ["sceneLogicalId"] = SchemaNode.String(),
            ["state"] = SchemaNode.String(new[] { "in-sync", "canonical-ahead", "engine-drift", "missing-dependency", "ambiguous" }), ["parity"] = SchemaNode.Boolean(),
            ["expectedRevision"] = SchemaNode.Integer(), ["expectedCanonicalHash"] = SchemaNode.String(), ["expectedInputDigest"] = SchemaNode.String(),
            ["expectedCatalogueFingerprint"] = SchemaNode.String(), ["effectiveCanonicalHash"] = SchemaNode.String(), ["effectiveInputDigest"] = SchemaNode.String(),
            ["effectiveCatalogueFingerprint"] = SchemaNode.String(), ["managedDigest"] = SchemaNode.String(),
            ["items"] = SchemaNode.Array(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["classification"] = SchemaNode.String(new[] { "missing", "extra", "changed", "ambiguous", "catalogue-drift", "canonical-ahead" }),
                ["objectId"] = SchemaNode.String(), ["managed"] = SchemaNode.Boolean(), ["fields"] = SchemaNode.Array(SchemaNode.String())
            }, new[] { "classification", "objectId", "managed", "fields" })),
            ["diagnostics"] = SchemaNode.Array(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            { ["code"] = SchemaNode.String(), ["subject"] = SchemaNode.String() }, new[] { "code", "subject" }))
        }, new[] { "schemaId", "sceneLogicalId", "state", "parity", "expectedRevision", "expectedCanonicalHash", "expectedInputDigest", "expectedCatalogueFingerprint",
            "effectiveCanonicalHash", "effectiveInputDigest", "effectiveCatalogueFingerprint", "managedDigest", "items", "diagnostics" });

        private static JsonSchemaDocument DriftSchema() => new JsonSchemaDocument(DriftNode());
        private static JsonSchemaDocument RematerializeSchema() => new JsonSchemaDocument(SchemaNode.Any());
        private static JsonSchemaDocument ProposalSchema() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { H1ProjectionImportProposal.Schema }), ["sceneLogicalId"] = SchemaNode.String(), ["available"] = SchemaNode.Boolean(),
            ["objectIds"] = SchemaNode.Array(SchemaNode.String()), ["diagnostics"] = SchemaNode.Array(SchemaNode.String()), ["mutationRequest"] = SchemaNode.Any(), ["drift"] = DriftNode()
        }, new[] { "schemaId", "sceneLogicalId", "available", "objectIds", "diagnostics", "mutationRequest", "drift" }));
    }
}
