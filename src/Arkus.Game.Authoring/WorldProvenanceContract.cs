using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Canonical discoverable contract for the HK06A authored mutation journal.</summary>
    public static class WorldProvenanceContract
    {
        public const string ReadName = "authoring.journal.read";
        public const string JournalSchemaId = "arkus.authoring.journal@1";
        public const string EntrySchemaId = "arkus.authoring.journal-entry@1";

        private static readonly ContractVersion Version = new ContractVersion(1, 0);

        public static IReadOnlyList<CapabilityDefinition> CreateDefinitions()
        {
            return new List<CapabilityDefinition> { DefineRead() }.AsReadOnly();
        }

        public static JsonSchemaDocument JournalResultSchema()
        {
            var anchor = AuthoredAnchorSchema();
            var capability = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["name"] = SchemaNode.String(new[] { WorldMutationContract.ApplyName }),
                    ["version"] = SchemaNode.String(new[] { WorldMutationContract.ContractVersionText })
                },
                new[] { "name", "version" });
            var requestIdentity = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = SchemaNode.String(),
                    ["fingerprint"] = SchemaNode.String()
                },
                new[] { "idempotencyKey", "fingerprint" });
            var entry = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { EntrySchemaId }),
                    ["entryId"] = SchemaNode.String(),
                    ["sequence"] = SchemaNode.Integer(),
                    ["capability"] = capability,
                    ["requestIdentity"] = requestIdentity,
                    ["request"] = WorldMutationContract.MutationRequestSchema().Root,
                    ["base"] = anchor,
                    ["result"] = anchor,
                    ["affectedResources"] = SchemaNode.Array(SchemaNode.String())
                },
                new[]
                {
                    "schemaId", "entryId", "sequence", "capability", "requestIdentity",
                    "request", "base", "result", "affectedResources"
                });

            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { JournalSchemaId }),
                    ["entrySchemaId"] = SchemaNode.String(new[] { EntrySchemaId }),
                    ["base"] = anchor,
                    ["current"] = anchor,
                    ["entryCount"] = SchemaNode.Integer(),
                    ["entries"] = SchemaNode.Array(entry)
                },
                new[] { "schemaId", "entrySchemaId", "base", "current", "entryCount", "entries" }));
        }

        public static JsonSchemaDocument RuntimeObservationStampSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { RuntimeObservationStamp.SchemaId }),
                    ["authoredBase"] = AuthoredAnchorSchema()
                },
                new[] { "schemaId", "authoredBase" }));
        }

        internal static SchemaNode AuthoredAnchorSchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["worldId"] = SchemaNode.String(),
                    ["stateSchemaVersion"] = SchemaNode.Integer(),
                    ["revision"] = SchemaNode.Integer(),
                    ["hash"] = SchemaNode.String()
                },
                new[] { "worldId", "stateSchemaVersion", "revision", "hash" });
        }

        private static CapabilityDefinition DefineRead()
        {
            return new CapabilityDefinition(
                new CapabilityKey(ReadName, Version),
                new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "authoring"),
                CanonicalContractSchemas.EmptyObject(),
                JournalResultSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                new[] { "journal-reflects-only-persisted-authored-mutations", "canonical-authored-state-unchanged" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Minimal),
                new CostSemantics(2, "read deterministic session-local authored mutation provenance"));
        }
    }
}
