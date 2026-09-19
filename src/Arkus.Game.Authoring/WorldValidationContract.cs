using System;
using System.Collections.Generic;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Canonical discoverable contract for current/proposed world validation.</summary>
    public static class WorldValidationContract
    {
        public const string CurrentName = "world.validation.current";
        public const string ProposedName = "authoring.change.validate";

        private static readonly ContractVersion Version = new ContractVersion(1, 0);

        public static IReadOnlyList<CapabilityDefinition> CreateDefinitions()
        {
            return new List<CapabilityDefinition>
            {
                DefineCurrent(),
                DefineProposed()
            }.AsReadOnly();
        }

        public static JsonSchemaDocument ValidationResultSchema()
        {
            var diagnostic = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["machineCode"] = SchemaNode.String(),
                    ["severity"] = SchemaNode.String(new[] { "error", "warning" }),
                    ["resource"] = SchemaNode.String(),
                    ["path"] = SchemaNode.String(),
                    ["invariantId"] = SchemaNode.String(),
                    ["message"] = SchemaNode.String(),
                    ["remediationContext"] = SchemaNode.Object(additionalPropertiesAllowed: true)
                },
                new[]
                {
                    "machineCode", "severity", "resource", "path", "invariantId", "message",
                    "remediationContext"
                });

            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { WorldValidationResult.SchemaId }),
                    ["invariantCatalogVersion"] = SchemaNode.String(new[] { WorldInvariantCatalog.CatalogVersion }),
                    ["target"] = SchemaNode.String(new[] { "current", "proposed" }),
                    ["valid"] = SchemaNode.Boolean(),
                    ["evaluatedInvariantCount"] = SchemaNode.Integer(),
                    ["diagnosticCount"] = SchemaNode.Integer(),
                    ["diagnostics"] = SchemaNode.Array(diagnostic)
                },
                new[]
                {
                    "schemaId", "invariantCatalogVersion", "target", "valid",
                    "evaluatedInvariantCount", "diagnosticCount", "diagnostics"
                }));
        }

        private static CapabilityDefinition DefineCurrent()
        {
            return new CapabilityDefinition(
                new CapabilityKey(CurrentName, Version),
                Provider("world"),
                CanonicalContractSchemas.EmptyObject(),
                ValidationResultSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                new[] { "all-owned-world-invariants-evaluated", "canonical-state-unchanged" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.PublicRead,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Required),
                new CostSemantics(2, "aggregate current-state validation"));
        }

        private static CapabilityDefinition DefineProposed()
        {
            return new CapabilityDefinition(
                new CapabilityKey(ProposedName, Version),
                Provider("authoring"),
                WorldMutationContract.MutationRequestSchema(),
                ValidationResultSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                new[] { "expected-revision-and-hash-match", "mutation-envelope-valid" },
                new[] { "all-owned-world-invariants-evaluated", "canonical-state-unchanged" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Supported, WorldMutationContract.MaximumOperations),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Required),
                new CostSemantics(4, "aggregate proposed-state validation without persistence"));
        }

        private static ProviderMetadata Provider(string capabilityNamespace)
        {
            return new ProviderMetadata("arkus.base", ProviderKind.Base, "base", capabilityNamespace);
        }
    }
}
