using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Arkus.Harness.Protocol
{
    public sealed class CanonicalContractProjection
    {
        public const string ContractModelVersion = "1.0";

        public CanonicalContractProjection(IEnumerable<CapabilityDefinition> capabilities)
        {
            if (capabilities == null)
            {
                throw new ArgumentNullException(nameof(capabilities));
            }

            var copy = new List<CapabilityDefinition>();
            var identities = new HashSet<CapabilityKey>();
            foreach (var capability in capabilities)
            {
                if (capability == null)
                {
                    throw new ArgumentException("Projection capabilities may not contain null entries.", nameof(capabilities));
                }

                if (!identities.Add(capability.Key))
                {
                    throw new ArgumentException("Projection contains a duplicate capability identity.", nameof(capabilities));
                }

                copy.Add(capability);
            }

            copy.Sort(CompareDefinitions);
            Capabilities = copy.AsReadOnly();
        }

        public IReadOnlyList<CapabilityDefinition> Capabilities { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var capabilities = new List<object?>();
            foreach (var capability in Capabilities)
            {
                capabilities.Add(capability.ToData());
            }

            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["contractModelVersion"] = ContractModelVersion,
                ["capabilities"] = capabilities.AsReadOnly()
            });
        }

        private static int CompareDefinitions(CapabilityDefinition left, CapabilityDefinition right)
        {
            var name = string.Compare(left.Key.Name, right.Key.Name, StringComparison.Ordinal);
            return name != 0 ? name : left.Key.Version.CompareTo(right.Key.Version);
        }
    }

    public sealed class ProjectionConformanceIssue
    {
        public ProjectionConformanceIssue(string code, string identity, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Identity { get; }
        public string Message { get; }
    }

    public static class CanonicalProjectionConformance
    {
        public static IReadOnlyList<ProjectionConformanceIssue> Compare(
            IEnumerable<CapabilityDefinition> canonicalDefinitions,
            CanonicalContractProjection projection)
        {
            if (canonicalDefinitions == null)
            {
                throw new ArgumentNullException(nameof(canonicalDefinitions));
            }

            if (projection == null)
            {
                throw new ArgumentNullException(nameof(projection));
            }

            return Compare(canonicalDefinitions, projection.ToData());
        }

        public static IReadOnlyList<ProjectionConformanceIssue> Compare(
            IEnumerable<CapabilityDefinition> canonicalDefinitions,
            IReadOnlyDictionary<string, object?> projectionArtifact)
        {
            if (canonicalDefinitions == null)
            {
                throw new ArgumentNullException(nameof(canonicalDefinitions));
            }

            if (projectionArtifact == null)
            {
                throw new ArgumentNullException(nameof(projectionArtifact));
            }

            var canonical = ToMap(canonicalDefinitions);
            var issues = new List<ProjectionConformanceIssue>();
            foreach (var portableIssue in PortableData.Validate(projectionArtifact))
            {
                issues.Add(new ProjectionConformanceIssue(
                    "projection.invalid_artifact",
                    portableIssue.Path,
                    portableIssue.Message));
            }

            if (issues.Count != 0)
            {
                return issues.AsReadOnly();
            }

            if (projectionArtifact.Count != 2 ||
                !projectionArtifact.ContainsKey("contractModelVersion") ||
                !projectionArtifact.ContainsKey("capabilities"))
            {
                issues.Add(new ProjectionConformanceIssue(
                    "projection.semantic_mismatch",
                    "$",
                    "Projection root fields differ from the canonical discovery artifact."));
            }

            if (!projectionArtifact.TryGetValue("contractModelVersion", out var modelVersionValue) ||
                !(modelVersionValue is string modelVersion) ||
                !string.Equals(modelVersion, CanonicalContractProjection.ContractModelVersion, StringComparison.Ordinal))
            {
                issues.Add(new ProjectionConformanceIssue(
                    "projection.contract_model_version_mismatch",
                    "$.contractModelVersion",
                    "Projection contract-model version differs from the canonical projection model."));
            }

            if (!projectionArtifact.TryGetValue("capabilities", out var capabilitiesValue) ||
                !(capabilitiesValue is IReadOnlyList<object?> capabilityArtifacts))
            {
                issues.Add(new ProjectionConformanceIssue(
                    "projection.invalid_artifact",
                    "$.capabilities",
                    "Projection capabilities must be a portable array of capability objects."));
                return issues.AsReadOnly();
            }

            var projected = ToArtifactMap(capabilityArtifacts, issues);

            foreach (var pair in canonical)
            {
                if (!projected.TryGetValue(pair.Key, out var projectedDefinitionData))
                {
                    issues.Add(new ProjectionConformanceIssue(
                        "projection.omitted_capability",
                        pair.Key.ToString(),
                        "Projection omitted a capability from the canonical composed inventory."));
                    continue;
                }

                if (!CanonicalDataEquality.AreEqual(
                    CanonicalProjectionOracleData.FromDefinition(pair.Value),
                    projectedDefinitionData))
                {
                    issues.Add(new ProjectionConformanceIssue(
                        "projection.semantic_mismatch",
                        pair.Key.ToString(),
                        "Projection changed canonical schema or semantic metadata."));
                }
            }

            foreach (var pair in projected)
            {
                if (!canonical.ContainsKey(pair.Key))
                {
                    issues.Add(new ProjectionConformanceIssue(
                        "projection.extra_capability",
                        pair.Key.ToString(),
                        "Projection invented a capability outside the canonical composed inventory."));
                }
            }

            return issues.AsReadOnly();
        }

        private static Dictionary<CapabilityKey, IReadOnlyDictionary<string, object?>> ToArtifactMap(
            IReadOnlyList<object?> capabilityArtifacts,
            IList<ProjectionConformanceIssue> issues)
        {
            var result = new Dictionary<CapabilityKey, IReadOnlyDictionary<string, object?>>();
            for (var index = 0; index < capabilityArtifacts.Count; index++)
            {
                var path = "$.capabilities[" + index + "]";
                if (!(capabilityArtifacts[index] is IReadOnlyDictionary<string, object?> capabilityData))
                {
                    issues.Add(new ProjectionConformanceIssue(
                        "projection.invalid_artifact",
                        path,
                        "Projected capability entry must be a portable object."));
                    continue;
                }

                if (!capabilityData.TryGetValue("name", out var nameValue) || !(nameValue is string name) ||
                    !capabilityData.TryGetValue("version", out var versionValue) || !(versionValue is string versionText))
                {
                    issues.Add(new ProjectionConformanceIssue(
                        "projection.invalid_artifact",
                        path,
                        "Projected capability entry must contain string name and version fields."));
                    continue;
                }

                ContractVersion version;
                try
                {
                    version = ContractVersion.Parse(versionText);
                }
                catch (FormatException)
                {
                    issues.Add(new ProjectionConformanceIssue(
                        "projection.invalid_artifact",
                        path + ".version",
                        "Projected capability version is not a canonical contract version."));
                    continue;
                }

                var key = new CapabilityKey(name, version);
                if (result.ContainsKey(key))
                {
                    issues.Add(new ProjectionConformanceIssue(
                        "projection.duplicate_capability",
                        key.ToString(),
                        "Projection emitted a duplicate capability identity."));
                    continue;
                }

                result.Add(key, capabilityData);
            }

            return result;
        }

        private static Dictionary<CapabilityKey, CapabilityDefinition> ToMap(IEnumerable<CapabilityDefinition> definitions)
        {
            var result = new Dictionary<CapabilityKey, CapabilityDefinition>();
            foreach (var definition in definitions)
            {
                if (definition == null)
                {
                    throw new ArgumentException("Canonical definitions may not contain null entries.", nameof(definitions));
                }

                if (result.ContainsKey(definition.Key))
                {
                    throw new ArgumentException("Canonical definition set contains duplicate identities.", nameof(definitions));
                }

                result.Add(definition.Key, definition);
            }

            return result;
        }
    }

    public enum CompatibilityKind
    {
        Compatible = 0,
        Additive = 1,
        Breaking = 2
    }

    public sealed class CompatibilityDecision
    {
        public CompatibilityDecision(CompatibilityKind kind, string reason)
        {
            Kind = kind;
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        }

        public CompatibilityKind Kind { get; }
        public string Reason { get; }
    }

    public static class ContractCompatibility
    {
        public static CompatibilityDecision Compare(CapabilityDefinition previous, CapabilityDefinition next)
        {
            if (previous == null)
            {
                throw new ArgumentNullException(nameof(previous));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (!string.Equals(previous.Key.Name, next.Key.Name, StringComparison.Ordinal))
            {
                return Breaking("Capability identity changed.");
            }

            var versionComparison = next.Key.Version.CompareTo(previous.Key.Version);
            if (versionComparison < 0)
            {
                return Breaking("Contract version moved backwards.");
            }

            if (versionComparison == 0)
            {
                return CanonicalSemanticEquality.DefinitionsEqual(previous, next)
                    ? new CompatibilityDecision(CompatibilityKind.Compatible, "Canonical semantics and version are unchanged.")
                    : Breaking("Canonical semantics changed without incrementing the contract version.");
            }

            if (next.Key.Version.Major != previous.Key.Version.Major)
            {
                return Breaking("A major-version change is a breaking compatibility boundary.");
            }

            if (!NonRequestSemanticsEqual(previous, next))
            {
                return Breaking("Result/error schema or canonical semantic metadata changed.");
            }

            if (previous.RequestSchema == null || next.RequestSchema == null)
            {
                return Breaking("Compatibility cannot be evaluated with a missing request schema.");
            }

            if (CanonicalSemanticEquality.SchemaDocumentsEqual(previous.RequestSchema, next.RequestSchema))
            {
                return new CompatibilityDecision(CompatibilityKind.Compatible, "Canonical semantics are unchanged across the version increment.");
            }

            if (IsAdditiveRequestChange(previous.RequestSchema.Root, next.RequestSchema.Root))
            {
                return new CompatibilityDecision(
                    CompatibilityKind.Additive,
                    "Request schema changed only by adding optional properties or relaxing requiredness within the same major version.");
            }

            return Breaking("Request schema changed incompatibly.");
        }

        private static CompatibilityDecision Breaking(string reason)
        {
            return new CompatibilityDecision(CompatibilityKind.Breaking, reason);
        }

        private static bool NonRequestSemanticsEqual(CapabilityDefinition left, CapabilityDefinition right)
        {
            if (!ProviderEquals(left.Provider, right.Provider) ||
                left.SideEffect != right.SideEffect ||
                left.Determinism != right.Determinism ||
                !SequenceEquals(left.Preconditions, right.Preconditions) ||
                !SequenceEquals(left.Postconditions, right.Postconditions) ||
                !ConcurrencyEquals(left.Concurrency, right.Concurrency) ||
                !IdempotencyEquals(left.Idempotency, right.Idempotency) ||
                !BatchingEquals(left.Batching, right.Batching) ||
                !RepairEquals(left.Repair, right.Repair) ||
                !PolicyEquals(left.Policy, right.Policy) ||
                !CostEquals(left.Cost, right.Cost))
            {
                return false;
            }

            return SchemaEquals(left.SuccessSchema, right.SuccessSchema) && SchemaEquals(left.ErrorSchema, right.ErrorSchema);
        }

        private static bool IsAdditiveRequestChange(SchemaNode previous, SchemaNode next)
        {
            if (previous.ValueType != SchemaValueType.Object || next.ValueType != SchemaValueType.Object ||
                previous.AdditionalPropertiesAllowed != next.AdditionalPropertiesAllowed)
            {
                return false;
            }

            foreach (var pair in previous.Properties)
            {
                if (!next.Properties.TryGetValue(pair.Key, out var nextSchema) || !SchemaNodeEquals(pair.Value, nextSchema))
                {
                    return false;
                }
            }

            var previousRequired = new HashSet<string>(previous.RequiredProperties, StringComparer.Ordinal);
            var nextRequired = new HashSet<string>(next.RequiredProperties, StringComparer.Ordinal);
            foreach (var required in nextRequired)
            {
                if (!previousRequired.Contains(required))
                {
                    return false;
                }
            }

            foreach (var pair in next.Properties)
            {
                if (!previous.Properties.ContainsKey(pair.Key) && nextRequired.Contains(pair.Key))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SchemaNodeEquals(SchemaNode left, SchemaNode right)
        {
            return CanonicalSemanticEquality.SchemaNodesEqual(left, right);
        }

        private static bool SchemaEquals(JsonSchemaDocument? left, JsonSchemaDocument? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return CanonicalSemanticEquality.SchemaDocumentsEqual(left, right);
        }

        private static bool ProviderEquals(ProviderMetadata left, ProviderMetadata right)
        {
            return string.Equals(left.ProviderId, right.ProviderId, StringComparison.Ordinal) &&
                left.Kind == right.Kind &&
                string.Equals(left.Scope, right.Scope, StringComparison.Ordinal) &&
                string.Equals(left.CapabilityNamespace, right.CapabilityNamespace, StringComparison.Ordinal);
        }

        private static bool SequenceEquals(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            for (var index = 0; index < left.Count; index++)
            {
                if (!string.Equals(left[index], right[index], StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ConcurrencyEquals(ConcurrencySemantics? left, ConcurrencySemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Class == right.Class && string.Equals(left.VersionTokenName, right.VersionTokenName, StringComparison.Ordinal);
        }

        private static bool IdempotencyEquals(IdempotencySemantics? left, IdempotencySemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Class == right.Class && string.Equals(left.KeyField, right.KeyField, StringComparison.Ordinal);
        }

        private static bool BatchingEquals(BatchingSemantics? left, BatchingSemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Class == right.Class && left.MaximumItems == right.MaximumItems;
        }

        private static bool RepairEquals(RepairSemantics? left, RepairSemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Retryable == right.Retryable && left.ExposesRepairHint == right.ExposesRepairHint;
        }

        private static bool PolicyEquals(PolicySemantics? left, PolicySemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.Privilege == right.Privilege &&
                left.TransactionRequirement == right.TransactionRequirement &&
                left.ProvenanceRequirement == right.ProvenanceRequirement;
        }

        private static bool CostEquals(CostSemantics? left, CostSemantics? right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            return left.RelativeWeight == right.RelativeWeight && string.Equals(left.Note, right.Note, StringComparison.Ordinal);
        }
    }
}
