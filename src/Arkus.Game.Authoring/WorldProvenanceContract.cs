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
        public const string PagedContractVersionText = "2.0";
        public const string JournalSchemaId = "arkus.authoring.journal@1";
        public const string JournalPageSchemaId = "arkus.authoring.journal-page@1";
        public const string EntrySchemaId = "arkus.authoring.journal-entry@1";
        public const int MaximumPageSize = 100;
        public const int DefaultPageSize = 50;

        private static readonly ContractVersion Version = ContractVersion.Parse(ContractVersionText);
        private static readonly ContractVersion PagedVersion = ContractVersion.Parse(PagedContractVersionText);

        public static IReadOnlyList<CapabilityDefinition> CreateDefinitions()
        {
            return new List<CapabilityDefinition> { DefineRead(), DefinePagedRead() }.AsReadOnly();
        }

        /// <summary>
        /// Complete persisted journal artifact used by accepted HK06 replay/audit semantics.
        /// This exact shape remains the success contract of authoring.journal.read@1.0.
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

        /// <summary>Accepted HK06A request contract: empty request means complete journal.</summary>
        public static JsonSchemaDocument JournalReadRequestSchema()
        {
            return CanonicalContractSchemas.EmptyObject();
        }

        /// <summary>HK08A paged request contract, exposed only by authoring.journal.read@2.0.</summary>
        public static JsonSchemaDocument JournalPageReadRequestSchema()
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
        /// Version 2 may return either the unchanged complete HK06A artifact (when the requested page
        /// covers the whole journal) or an explicitly marked bounded page. Partial pages always carry
        /// pageOffset/journalSchemaId and the page schema marker.
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
            // Keep the accepted HK06A public identity and semantics byte-for-byte equivalent at the
            // canonical model level: empty request -> complete journal artifact.
            return new CapabilityDefinition(
                new CapabilityKey(ReadName, Version),
                new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "authoring"),
                JournalReadRequestSchema(),
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

        private static CapabilityDefinition DefinePagedRead()
        {
            return new CapabilityDefinition(
                new CapabilityKey(ReadName, PagedVersion),
                new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "authoring"),
                JournalPageReadRequestSchema(),
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
