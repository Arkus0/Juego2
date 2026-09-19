using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Canonical schema/definition source for HK04 transactional mutation.</summary>
    public static class WorldMutationContract
    {
        public const string PlanName = "authoring.change.plan";
        public const string DryRunName = "authoring.change.dry-run";
        public const string ApplyName = "authoring.change.apply";
        public const int MaximumOperations = 64;

        private static readonly ContractVersion Version = new ContractVersion(1, 0);

        public static IReadOnlyList<CapabilityDefinition> CreateDefinitions()
        {
            return new List<CapabilityDefinition>
            {
                DefineReadPhase(PlanName, "deterministic mutation plan without persistence"),
                DefineReadPhase(DryRunName, "same semantic plan/validation path without persistence"),
                DefineApply()
            }.AsReadOnly();
        }

        private static CapabilityDefinition DefineReadPhase(string name, string note)
        {
            return new CapabilityDefinition(
                new CapabilityKey(name, Version),
                Provider(),
                MutationRequestSchema(),
                MutationSuccessSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                new[] { "expected-revision-and-hash-match", "mutation-envelope-valid" },
                new[] { "candidate-state-valid", "no-authorable-state-persisted" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Supported, MaximumOperations),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Required),
                new CostSemantics(4, note));
        }

        private static CapabilityDefinition DefineApply()
        {
            return new CapabilityDefinition(
                new CapabilityKey(ApplyName, Version),
                Provider(),
                MutationRequestSchema(),
                MutationSuccessSchema(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.CanonicalMutation,
                DeterminismClass.Deterministic,
                new[] { "expected-revision-and-hash-match", "mutation-envelope-valid", "idempotency-key-not-conflicting" },
                new[] { "candidate-state-valid", "whole-candidate-committed-atomically-or-state-unchanged" },
                new ConcurrencySemantics(ConcurrencyClass.OptimisticVersioned, "expectedRevision+expectedHash"),
                new IdempotencySemantics(IdempotencyClass.IdempotentWithKey, "idempotencyKey"),
                new BatchingSemantics(BatchingClass.Supported, MaximumOperations),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.CanonicalTransaction,
                    ProvenanceRequirement.Required),
                new CostSemantics(6, "atomic canonical world-state compare-and-swap"));
        }

        private static ProviderMetadata Provider()
        {
            return new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "authoring");
        }

        private static JsonSchemaDocument MutationRequestSchema()
        {
            var reference = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["kind"] = SchemaNode.String(),
                    ["targetId"] = SchemaNode.String()
                },
                new[] { "kind", "targetId" });

            var operation = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["kind"] = SchemaNode.String(new[]
                    {
                        "put-object", "remove-object", "put-extension", "remove-extension"
                    }),
                    ["id"] = SchemaNode.String(),
                    ["typeId"] = SchemaNode.String(),
                    ["containerId"] = SchemaNode.String(),
                    ["references"] = SchemaNode.Array(reference),
                    ["owner"] = SchemaNode.String(),
                    ["schemaVersion"] = SchemaNode.Integer(),
                    ["payloadBase64"] = SchemaNode.String()
                },
                new[] { "kind" });

            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = SchemaNode.String(),
                    ["expectedRevision"] = SchemaNode.Integer(),
                    ["expectedHash"] = SchemaNode.String(),
                    ["operations"] = SchemaNode.Array(operation)
                },
                new[] { "idempotencyKey", "expectedRevision", "expectedHash", "operations" }));
        }

        private static JsonSchemaDocument MutationSuccessSchema()
        {
            var world = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["worldId"] = SchemaNode.String(),
                    ["schemaVersion"] = SchemaNode.Integer(),
                    ["revision"] = SchemaNode.Integer(),
                    ["hash"] = SchemaNode.String()
                },
                new[] { "worldId", "schemaVersion", "revision", "hash" });

            var change = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["resource"] = SchemaNode.String(),
                    ["action"] = SchemaNode.String(new[] { "create", "update", "remove" }),
                    ["fields"] = SchemaNode.Array(SchemaNode.String()),
                    ["referencesAdded"] = SchemaNode.Array(SchemaNode.String()),
                    ["referencesRemoved"] = SchemaNode.Array(SchemaNode.String())
                },
                new[] { "resource", "action", "fields", "referencesAdded", "referencesRemoved" });

            var condition = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["code"] = SchemaNode.String(),
                    ["path"] = SchemaNode.String(),
                    ["satisfied"] = SchemaNode.Boolean()
                },
                new[] { "code", "path", "satisfied" });

            var plan = SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["planId"] = SchemaNode.String(),
                    ["base"] = world,
                    ["result"] = world,
                    ["changes"] = SchemaNode.Array(change),
                    ["preconditions"] = SchemaNode.Array(condition),
                    ["postconditions"] = SchemaNode.Array(condition)
                },
                new[] { "planId", "base", "result", "changes", "preconditions", "postconditions" });

            return new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["mode"] = SchemaNode.String(new[] { "plan", "dry-run", "apply" }),
                    ["persisted"] = SchemaNode.Boolean(),
                    ["replayed"] = SchemaNode.Boolean(),
                    ["plan"] = plan
                },
                new[] { "mode", "persisted", "replayed", "plan" }));
        }
    }
}
