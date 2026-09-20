using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Canonical discoverable contracts for authored mutation provenance.</summary>
    public static class WorldProvenanceContract
    {
        public const string ReadName = "authoring.journal.read";
        public const string ContractVersionText = "1.0";
        public const string JournalSchemaId = "arkus.authoring.journal@1";
        public const string JournalPageSchemaId = "arkus.authoring.journal-page@1";
        public const string EntrySchemaId = "arkus.authoring.journal-entry@1";
        public const int MaximumPageSize = 100;
        public const int DefaultPageSize = 50;

        private static readonly ContractVersion Version = ContractVersion.Parse(ContractVersionText);

        public static IReadOnlyList<CapabilityDefinition> CreateDefinitions()
        {
            return new List<CapabilityDefinition> { DefineRead() }.AsReadOnly();
        }

        /// <summary>
        /// Complete persisted journal artifact used by accepted HK06 replay/audit semantics.
        /// HK08A leaves this artifact unchanged. When one bounded read covers the complete journal,
        /// the public capability returns this exact shape so accepted replay clients remain truthful.
        /// Multi-page reads use the separate page marker below and cannot be mistaken for a complete
        /// replay artifact.
        /// </summary>
        public static JsonSchemaDocument JournalResultSchema()
        {
            var anchor = AuthoredAnchorSchema();
            var entry = JournalEntrySchema(anchor);
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

        public static JsonSchemaDocument JournalReadRequestSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["revision"] = SchemaNode.Integer(),
                    ["hash"] = SchemaNode.String(),
                    ["limit"] = SchemaNode.Integer(),
                    ["cursor"] = SchemaNode.String()
                }));
        }

        /// <summary>
        /// Public read result accepts either the unchanged complete HK06A artifact (when the complete
        /// journal fits the requested page) or an explicitly marked HK08A bounded page. Conditional
        /// schema constraints are intentionally enforced by the implementation/conformance tests;
        /// partial pages always carry pageOffset/journalSchemaId and the page schema marker.
        /// </summary>
        public static JsonSchemaDocument JournalPageResultSchema()
        {
            var anchor = AuthoredAnchorSchema();
            var entry = JournalEntrySchema(anchor);
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { JournalSchemaId, JournalPageSchemaId }),
                    ["journalSchemaId"] = SchemaNode.String(new[] { JournalSchemaId }),
                    ["entrySchemaId"] = SchemaNode.String(new[] { EntrySchemaId }),
                    ["base"] = anchor,
                    ["current"] = anchor,
                    ["entryCount"] = SchemaNode.Integer(),
                    ["pageOffset"] = SchemaNode.Integer(),
                    ["entries"] = SchemaNode.Array(entry),
                    ["nextCursor"] = SchemaNode.String()
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

        private static SchemaNode JournalEntrySchema(SchemaNode anchor)
        {
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
            return SchemaNode.Object(
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
        }

        private static CapabilityDefinition DefineRead()
        {
            return new CapabilityDefinition(
                new CapabilityKey(ReadName, Version),
                new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "authoring"),
                JournalReadRequestSchema(),
                JournalPageResultSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                new[]
                {
                    "journal-page-preserves-persisted-sequence-order",
                    "page-is-bound-to-one-authored-revision-and-hash",
                    "complete-single-page-read-preserves-hk06a-journal-artifact",
                    "canonical-authored-state-unchanged"
                },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Minimal),
                new CostSemantics(2, "bounded deterministic page over session-local authored mutation provenance"));
        }
    }
}
