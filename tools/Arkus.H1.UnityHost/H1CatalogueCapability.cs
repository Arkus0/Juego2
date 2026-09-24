using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    public sealed class H1CatalogueExecutor : IH1UnityCapabilityExecutor
    {
        public static readonly CapabilityKey QueryKey = new CapabilityKey("unity.host.catalogue.query", new ContractVersion(1, 0));
        public static readonly CapabilityKey GetKey = new CapabilityKey("unity.host.catalogue.get", new ContractVersion(1, 0));
        public static readonly CapabilityKey ResolveKey = new CapabilityKey("unity.host.catalogue.resolve", new ContractVersion(1, 0));
        public const string QueryExecutorId = "arkus.h1.worker.catalogue.query@1";
        public const string GetExecutorId = "arkus.h1.worker.catalogue.get@1";
        public const string ResolveExecutorId = "arkus.h1.worker.catalogue.resolve@1";

        private readonly string _mappingPath;
        private readonly string _adoptionPath;
        public H1CatalogueExecutor(CapabilityKey capability, string executorId, string mappingPath, string adoptionPath)
        {
            Capability = capability ?? throw new ArgumentNullException(nameof(capability));
            ExecutorId = executorId ?? throw new ArgumentNullException(nameof(executorId));
            _mappingPath = mappingPath ?? throw new ArgumentNullException(nameof(mappingPath));
            _adoptionPath = adoptionPath ?? throw new ArgumentNullException(nameof(adoptionPath));
        }
        public CapabilityKey Capability { get; }
        public string ExecutorId { get; }

        public string EncodeRequest(IReadOnlyDictionary<string, object?> request) =>
            JsonSerializer.Serialize(request ?? throw new ArgumentNullException(nameof(request)));

        public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            try
            {
                using var response = JsonDocument.Parse(result.Payload);
                var root = response.RootElement;
                if (root.GetProperty("schemaId").GetString() != "arkus.h1-catalogue-worker-observation@1")
                    throw new JsonException("Wrong catalogue worker response schema.");
                var requestJson = root.GetProperty("requestJson").GetString() ?? throw new JsonException("Missing echoed request.");
                var inventoryJson = root.GetProperty("inventory").GetRawText();
                if (!File.Exists(_mappingPath)) throw new H1CatalogueException("catalogue.mapping-missing", "The reviewed catalogue mapping is missing.");
                if (!File.Exists(_adoptionPath)) throw new H1CatalogueException("catalogue.adoption-missing", "The approved Quaternius Source adoption record is missing.");
                var snapshot = H1CatalogueSnapshot.Build(inventoryJson, File.ReadAllText(_mappingPath), File.ReadAllText(_adoptionPath));
                using var request = JsonDocument.Parse(requestJson);
                IReadOnlyDictionary<string, object?> data;
                if (Capability.Equals(QueryKey))
                {
                    var body = request.RootElement;
                    var kind = body.TryGetProperty("kind", out var kindValue) ? kindValue.GetString() ?? "" : "";
                    var offset = body.TryGetProperty("offset", out var offsetValue) ? offsetValue.GetInt32() : 0;
                    long? expectedToken = body.TryGetProperty("expectedSnapshotToken", out var tokenValue) ? tokenValue.GetInt64() : null;
                    data = snapshot.Query(kind, body.GetProperty("pageSize").GetInt32(), offset, expectedToken);
                }
                else if (Capability.Equals(GetKey) || Capability.Equals(ResolveKey))
                {
                    var body = request.RootElement;
                    data = snapshot.Get(body.GetProperty("logicalId").GetString() ?? "", body.GetProperty("kind").GetString() ?? "", Capability.Equals(ResolveKey));
                }
                else throw new JsonException("Unpaired catalogue executor.");
                return CapabilityInvocationResult.Succeeded(data);
            }
            catch (H1CatalogueException exception)
            {
                return CapabilityInvocationResult.Failed(new StructuredError(exception.Code, exception.Message, "$",
                    new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)),
                    false, "Repair the reviewed mapping, source input or catalogue reference before materialization."));
            }
            catch (IOException)
            {
                return CapabilityInvocationResult.Failed(new StructuredError("catalogue.mapping-unreadable", "The reviewed catalogue mapping cannot be read.", "$",
                    new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)), false,
                    "Restore the reviewed mapping file before querying the Unity catalogue."));
            }
        }
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.catalogue.query", "1.0")]
    public sealed class H1CatalogueQueryHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1CatalogueQueryHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.catalogue.get", "1.0")]
    public sealed class H1CatalogueGetHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1CatalogueGetHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.catalogue.resolve", "1.0")]
    public sealed class H1CatalogueResolveHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1CatalogueResolveHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    public static class H1CatalogueContract
    {
        public const string PublicReferenceNamespace = "ref.arkus.unity-host.catalogue";
        private static readonly string[] Kinds = { "scene", "asset", "prefab", "material", "animation-clip", "component-schema" };

        public static IReadOnlyList<CapabilityDefinition> Definitions() => new[]
        {
            Definition(H1CatalogueExecutor.QueryKey, QueryRequest(), QuerySuccess(), "bounded complete scoped catalogue observation and page"),
            Definition(H1CatalogueExecutor.GetKey, ReferenceRequest(), GetSuccess(), "one logical catalogue entry"),
            Definition(H1CatalogueExecutor.ResolveKey, ReferenceRequest(), GetSuccess(), "one compatible logical catalogue reference")
        };

        public static IReadOnlyList<CapabilityRoute> Routes(H1UnityEditorExecutionCoordinator coordinator) => new[]
        {
            CapabilityRoute.FromHandler(new H1CatalogueQueryHandler(coordinator)),
            CapabilityRoute.FromHandler(new H1CatalogueGetHandler(coordinator)),
            CapabilityRoute.FromHandler(new H1CatalogueResolveHandler(coordinator))
        };

        public static IReadOnlyList<UnityHostCapabilityGrant> Grants() => new[]
        {
            // Query selects the fixed reviewed project inventory, not a caller-selected asset.
            new UnityHostCapabilityGrant(H1CatalogueExecutor.QueryKey, UnityProjectWorkspaceAuthority.ManagedAssetsRootId,
                PublicReferenceNamespace, UnityHostResourceClass.ProjectMetadata, UnityHostTimeClass.BoundedRead),
            Grant(H1CatalogueExecutor.GetKey), Grant(H1CatalogueExecutor.ResolveKey)
        };

        private static UnityHostCapabilityGrant Grant(CapabilityKey key) => new UnityHostCapabilityGrant(key,
            UnityProjectWorkspaceAuthority.ManagedAssetsRootId, PublicReferenceNamespace,
            UnityHostResourceClass.ManagedAsset, UnityHostTimeClass.BoundedRead);

        private static CapabilityDefinition Definition(CapabilityKey key, JsonSchemaDocument request, JsonSchemaDocument success, string cost) =>
            new CapabilityDefinition(key,
                new ProviderMetadata(H1UnityHostCapabilityPolicy.HostProviderId, ProviderKind.Scoped,
                    H1UnityHostCapabilityPolicy.HostScope, H1UnityHostCapabilityPolicy.HostNamespace),
                request, success, CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly, DeterminismClass.EnvironmentDependent,
                new[] { "h1-unity-profile-bound", "reviewed-catalogue-source-present" },
                new[] { "effective-inventory-reconciled-with-mapping" },
                new ConcurrencySemantics(ConcurrencyClass.Serialized),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported), new RepairSemantics(true, true),
                new PolicySemantics(PrivilegeClass.Authoring, TransactionRequirement.None, ProvenanceRequirement.Required),
                new CostSemantics(1, "one short-lived Unity Editor worker: " + cost));

        private static JsonSchemaDocument QueryRequest() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["kind"] = SchemaNode.String(Kinds), ["pageSize"] = SchemaNode.Integer(),
            ["offset"] = SchemaNode.Integer(), ["expectedSnapshotToken"] = SchemaNode.Integer()
        }, new[] { "pageSize" }));

        private static JsonSchemaDocument ReferenceRequest() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["logicalId"] = SchemaNode.String(format: "arkus-logical-reference", logicalReferenceNamespace: PublicReferenceNamespace),
            ["kind"] = SchemaNode.String(Kinds)
        }, new[] { "logicalId", "kind" }));

        private static SchemaNode EntrySchema() => SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-catalogue-entry@1" }),
            ["logicalId"] = SchemaNode.String(), ["kind"] = SchemaNode.String(Kinds),
            ["name"] = SchemaNode.String(), ["typeName"] = SchemaNode.String(),
            ["dimensions"] = SchemaNode.String(), ["sourceId"] = SchemaNode.String(),
            ["adoptionStatus"] = SchemaNode.String(), ["compatible"] = SchemaNode.Boolean(),
            ["path"] = SchemaNode.String(), ["nativeGuid"] = SchemaNode.String(),
            ["localFileId"] = SchemaNode.String(), ["contentSha256"] = SchemaNode.String(),
            ["dependencies"] = SchemaNode.Array(SchemaNode.String()),
            ["schemaFields"] = SchemaNode.Array(SchemaNode.String())
        }, new[] { "schemaId", "logicalId", "kind", "name", "typeName", "dimensions", "sourceId", "adoptionStatus", "compatible", "path", "nativeGuid", "localFileId", "contentSha256", "dependencies", "schemaFields" });

        private static JsonSchemaDocument QuerySuccess() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { H1CatalogueSnapshot.SnapshotSchema }),
            ["fingerprint"] = SchemaNode.String(), ["snapshotToken"] = SchemaNode.Integer(),
            ["total"] = SchemaNode.Integer(), ["entries"] = SchemaNode.Array(EntrySchema()),
            ["nextOffset"] = SchemaNode.Integer()
        }, new[] { "schemaId", "fingerprint", "snapshotToken", "total", "entries", "nextOffset" }));

        private static JsonSchemaDocument GetSuccess() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-catalogue-get@1" }),
            ["fingerprint"] = SchemaNode.String(), ["entry"] = EntrySchema()
        }, new[] { "schemaId", "fingerprint", "entry" }));
    }
}
