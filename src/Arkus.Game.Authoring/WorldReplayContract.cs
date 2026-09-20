using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Canonical discoverable contract for HK06C deterministic authored-journal replay.</summary>
    public static class WorldReplayContract
    {
        public const string CompatibilityName = "authoring.replay.compatibility";
        public const string ReplayName = "authoring.journal.replay";
        public const string ContractVersionText = "1.0";
        public const string ReplayResultSchemaId = "arkus.authoring.replay-result@1";
        public const string CompatibilityResultSchemaId = "arkus.authoring.replay-compatibility@1";
        public const string AuditEntrySchemaId = "arkus.authoring.replay-audit-entry@1";

        private static readonly ContractVersion Version = ContractVersion.Parse(ContractVersionText);

        public static IReadOnlyList<CapabilityDefinition> CreateDefinitions()
        {
            return new List<CapabilityDefinition>
            {
                DefineCompatibility(),
                DefineReplay()
            }.AsReadOnly();
        }

        public static JsonSchemaDocument CompatibilityRequestSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["journalSchemaId"] = SchemaNode.String(),
                    ["entrySchemaId"] = SchemaNode.String(),
                    ["snapshotSchemaId"] = SchemaNode.String(),
                    ["snapshotVersion"] = SchemaNode.Integer()
                },
                new[] { "journalSchemaId", "entrySchemaId", "snapshotSchemaId", "snapshotVersion" }));
        }

        public static JsonSchemaDocument CompatibilityResultSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { CompatibilityResultSchemaId }),
                    ["disposition"] = SchemaNode.String(new[] { "supported", "unsupported", "migration-required" }),
                    ["journalSchemaId"] = SchemaNode.String(),
                    ["entrySchemaId"] = SchemaNode.String(),
                    ["snapshotSchemaId"] = SchemaNode.String(),
                    ["snapshotVersion"] = SchemaNode.Integer(),
                    ["supportedJournalSchemaId"] = SchemaNode.String(new[] { WorldProvenanceContract.JournalSchemaId }),
                    ["supportedEntrySchemaId"] = SchemaNode.String(new[] { WorldProvenanceContract.EntrySchemaId }),
                    ["supportedSnapshotSchemaId"] = SchemaNode.String(new[] { WorldPortabilityContract.SnapshotSchemaId }),
                    ["supportedSnapshotVersion"] = SchemaNode.Integer(),
                    ["migrationCapability"] = SchemaNode.String()
                },
                new[]
                {
                    "schemaId", "disposition", "journalSchemaId", "entrySchemaId", "snapshotSchemaId",
                    "snapshotVersion", "supportedJournalSchemaId", "supportedEntrySchemaId",
                    "supportedSnapshotSchemaId", "supportedSnapshotVersion"
                }));
        }

        public static JsonSchemaDocument ReplayRequestSchema()
        {
            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["expectedRevision"] = SchemaNode.Integer(),
                    ["expectedHash"] = SchemaNode.String(),
                    ["journal"] = VersionedJournalEnvelopeSchema()
                },
                new[] { "expectedRevision", "expectedHash", "journal" }));
        }

        /// <summary>
        /// Compatibility envelope only. HK06A remains the sole owner of the accepted journal schema;
        /// replay deliberately accepts version identifiers as strings so unsupported/future artifacts
        /// reach the HK06C compatibility policy and fail with a stable replay diagnostic instead of
        /// being rejected by generic dispatcher validation first.
        /// </summary>
        private static SchemaNode VersionedJournalEnvelopeSchema()
        {
            return SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(),
                    ["entrySchemaId"] = SchemaNode.String(),
                    ["base"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                    ["current"] = SchemaNode.Object(additionalPropertiesAllowed: true),
                    ["entryCount"] = SchemaNode.Integer(),
                    ["entries"] = SchemaNode.Array(SchemaNode.Any())
                },
                new[] { "schemaId", "entrySchemaId", "base", "current", "entryCount", "entries" });
        }

        public static JsonSchemaDocument ReplayResultSchema()
        {
            var audit = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { AuditEntrySchemaId }),
                    ["sequence"] = SchemaNode.Integer(),
                    ["sourceEntryId"] = SchemaNode.String(),
                    ["replayedEntryId"] = SchemaNode.String(),
                    ["sourceResult"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["replayedResult"] = WorldProvenanceContract.AuthoredAnchorSchema()
                },
                new[]
                {
                    "schemaId", "sequence", "sourceEntryId", "replayedEntryId", "sourceResult", "replayedResult"
                });

            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { ReplayResultSchemaId }),
                    ["journalSchemaId"] = SchemaNode.String(new[] { WorldProvenanceContract.JournalSchemaId }),
                    ["entrySchemaId"] = SchemaNode.String(new[] { WorldProvenanceContract.EntrySchemaId }),
                    ["sourceBase"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["sourceCurrent"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["targetPrevious"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["targetCurrent"] = WorldProvenanceContract.AuthoredAnchorSchema(),
                    ["replayedEntries"] = SchemaNode.Integer(),
                    ["journalDisposition"] = SchemaNode.String(new[] { "replayed-local-mutation-history" }),
                    ["audit"] = SchemaNode.Array(audit)
                },
                new[]
                {
                    "schemaId", "journalSchemaId", "entrySchemaId", "sourceBase", "sourceCurrent",
                    "targetPrevious", "targetCurrent", "replayedEntries", "journalDisposition", "audit"
                }));
        }

        private static CapabilityDefinition DefineCompatibility()
        {
            return new CapabilityDefinition(
                new CapabilityKey(CompatibilityName, Version),
                Provider(),
                CompatibilityRequestSchema(),
                CompatibilityResultSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                new[] { "replay-compatibility-policy-reported-without-state-change" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Minimal),
                new CostSemantics(1, "machine-readable accepted journal/snapshot replay compatibility"));
        }

        private static CapabilityDefinition DefineReplay()
        {
            return new CapabilityDefinition(
                new CapabilityKey(ReplayName, Version),
                Provider(),
                ReplayRequestSchema(),
                ReplayResultSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.CanonicalReplay,
                DeterminismClass.Deterministic,
                new[]
                {
                    "expected-revision-and-hash-match",
                    "target-is-clean-replay-base",
                    "journal-version-supported",
                    "journal-integrity-and-chain-valid"
                },
                new[]
                {
                    "all-source-entries-replayed-through-canonical-mutation-authority-or-target-unchanged",
                    "final-anchor-exactly-matches-journal-current",
                    "replayed-local-journal-binds-source-entry-identities"
                },
                new ConcurrencySemantics(ConcurrencyClass.OptimisticVersioned, "expectedRevision+expectedHash"),
                new IdempotencySemantics(IdempotencyClass.NonIdempotent),
                new BatchingSemantics(BatchingClass.Required),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.CanonicalReplay,
                    ProvenanceRequirement.Required),
                new CostSemantics(10, "atomic deterministic replay of the supplied accepted mutation journal"));
        }

        private static ProviderMetadata Provider()
        {
            return new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "authoring");
        }
    }
}
