using System;
using System.Collections.Generic;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Canonical discoverable contract for HK06B semantic diff and snapshot portability.</summary>
    public static class WorldPortabilityContract
    {
        public const string CompareName = "authoring.diff.compare";
        public const string ExportName = "authoring.snapshot.export";
        public const string ImportName = "authoring.snapshot.import";
        public const string ContractVersionText = "1.0";

        public const string SnapshotSchemaId = "arkus.authoring.snapshot@1";
        public const string DiffSchemaId = "arkus.authoring.semantic-diff@1";
        public const string ImportResultSchemaId = "arkus.authoring.snapshot-import-result@1";
        public const string StateFormat = "arkus.world-state.canonical";
        public const int SnapshotVersion = 1;

        private static readonly ContractVersion Version = ContractVersion.Parse(ContractVersionText);

        public static IReadOnlyList<CapabilityDefinition> CreateDefinitions()
        {
            return new List<CapabilityDefinition>
            {
                DefineCompare(),
                DefineExport(),
                DefineImport()
            }.AsReadOnly();
        }

        public static JsonSchemaDocument SnapshotSchema()
        {
            return new JsonSchemaDocument(SnapshotNode());
        }

        public static JsonSchemaDocument DiffResultSchema()
        {
            var change = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["resource"] = SchemaNode.String(),
                    ["resourceKind"] = SchemaNode.String(new[] { "object", "extension" }),
                    ["action"] = SchemaNode.String(new[] { "create", "update", "remove" }),
                    ["fields"] = SchemaNode.Array(SchemaNode.String()),
                    ["relationsAdded"] = SchemaNode.Array(SchemaNode.String()),
                    ["relationsRemoved"] = SchemaNode.Array(SchemaNode.String())
                },
                new[]
                {
                    "resource", "resourceKind", "action", "fields", "relationsAdded", "relationsRemoved"
                });

            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { DiffSchemaId }),
                    ["sameAuthorableState"] = SchemaNode.Boolean(),
                    ["base"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["target"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["changes"] = SchemaNode.Array(change)
                },
                new[] { "schemaId", "sameAuthorableState", "base", "target", "changes" }));
        }

        public static JsonSchemaDocument ImportResultSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { ImportResultSchemaId }),
                    ["previous"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["current"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["lineageDisposition"] = SchemaNode.String(new[] { "new-local-lineage" }),
                    ["retainedJournalEntries"] = SchemaNode.Integer(),
                    ["importedJournalEntries"] = SchemaNode.Integer(),
                    ["replayed"] = SchemaNode.Boolean()
                },
                new[]
                {
                    "schemaId", "previous", "current", "lineageDisposition",
                    "retainedJournalEntries", "importedJournalEntries", "replayed"
                }));
        }

        internal static SchemaNode SnapshotNode()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { SnapshotSchemaId }),
                    ["snapshotVersion"] = SchemaNode.Integer(),
                    ["stateFormat"] = SchemaNode.String(new[] { StateFormat }),
                    ["stateFormatVersion"] = SchemaNode.Integer(),
                    ["anchor"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["authoredStateBase64"] = SchemaNode.String(),
                    ["provenanceIncluded"] = SchemaNode.Boolean(),
                    ["runtimeObservationsIncluded"] = SchemaNode.Boolean()
                },
                new[]
                {
                    "schemaId", "snapshotVersion", "stateFormat", "stateFormatVersion", "anchor",
                    "authoredStateBase64", "provenanceIncluded", "runtimeObservationsIncluded"
                });
        }

        private static CapabilityDefinition DefineCompare()
        {
            var request = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["base"] = SnapshotNode(),
                    ["target"] = SnapshotNode()
                },
                new[] { "base", "target" }));

            return new CapabilityDefinition(
                new CapabilityKey(CompareName, Version),
                Provider(),
                request,
                DiffResultSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                new[] { "snapshots-valid-and-version-supported", "world-identity-and-state-schema-match" },
                new[] { "semantic-resource-diff-deterministic", "canonical-authored-state-unchanged" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Minimal),
                new CostSemantics(4, "semantic authored-resource comparison independent of serializer text"));
        }

        private static CapabilityDefinition DefineExport()
        {
            return new CapabilityDefinition(
                new CapabilityKey(ExportName, Version),
                Provider(),
                CanonicalContractSchemas.EmptyObject(),
                SnapshotSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                new[]
                {
                    "snapshot-reconstructs-canonical-hash",
                    "provenance-not-embedded",
                    "runtime-observations-not-embedded",
                    "canonical-authored-state-unchanged"
                },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Minimal),
                new CostSemantics(3, "canonical authored-state snapshot export"));
        }

        private static CapabilityDefinition DefineImport()
        {
            var request = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = SchemaNode.String(),
                    ["expectedRevision"] = SchemaNode.Integer(),
                    ["expectedHash"] = SchemaNode.String(),
                    ["snapshot"] = SnapshotNode()
                },
                new[] { "idempotencyKey", "expectedRevision", "expectedHash", "snapshot" }));

            return new CapabilityDefinition(
                new CapabilityKey(ImportName, Version),
                Provider(),
                request,
                ImportResultSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.CanonicalMutation,
                DeterminismClass.Deterministic,
                new[]
                {
                    "expected-revision-and-hash-match",
                    "snapshot-valid-and-version-supported",
                    "snapshot-anchor-matches-embedded-state",
                    "idempotency-key-not-conflicting"
                },
                new[]
                {
                    "imported-state-hash-preserved",
                    "new-local-lineage-rooted-at-imported-state",
                    "local-mutation-history-empty",
                    "replacement-atomic-or-state-unchanged"
                },
                new ConcurrencySemantics(ConcurrencyClass.OptimisticVersioned, "expectedRevision+expectedHash"),
                new IdempotencySemantics(IdempotencyClass.IdempotentWithKey, "idempotencyKey"),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.CanonicalTransaction,
                    ProvenanceRequirement.Required),
                new CostSemantics(6, "validated authored-session rebase from canonical snapshot"));
        }

        private static ProviderMetadata Provider()
        {
            return new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "authoring");
        }
    }
}
