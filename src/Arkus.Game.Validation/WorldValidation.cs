using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Validation
{
    public enum WorldDiagnosticSeverity
    {
        Error = 1,
        Warning = 2
    }

    public sealed class WorldValidationDiagnostic
    {
        public WorldValidationDiagnostic(
            string machineCode,
            WorldDiagnosticSeverity severity,
            string resource,
            string path,
            string invariantId,
            string message,
            IReadOnlyDictionary<string, object?> remediationContext)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            Severity = severity;
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            InvariantId = invariantId ?? throw new ArgumentNullException(nameof(invariantId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            RemediationContext = new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(remediationContext ??
                    throw new ArgumentNullException(nameof(remediationContext)), StringComparer.Ordinal));
        }

        public string MachineCode { get; }
        public WorldDiagnosticSeverity Severity { get; }
        public string Resource { get; }
        public string Path { get; }
        public string InvariantId { get; }
        public string Message { get; }
        public IReadOnlyDictionary<string, object?> RemediationContext { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["machineCode"] = MachineCode,
                ["severity"] = Severity == WorldDiagnosticSeverity.Error ? "error" : "warning",
                ["resource"] = Resource,
                ["path"] = Path,
                ["invariantId"] = InvariantId,
                ["message"] = Message,
                ["remediationContext"] = RemediationContext
            });
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> values)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(values, StringComparer.Ordinal));
        }
    }

    public sealed class WorldValidationResult
    {
        public const string SchemaId = "arkus.world-validation-result/v1";

        public WorldValidationResult(string target, IEnumerable<WorldValidationDiagnostic> diagnostics, int evaluatedInvariantCount)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            if (diagnostics == null) throw new ArgumentNullException(nameof(diagnostics));

            var values = new List<WorldValidationDiagnostic>();
            foreach (var diagnostic in diagnostics)
            {
                values.Add(diagnostic ?? throw new ArgumentException(
                    "Validation diagnostics cannot contain null entries.", nameof(diagnostics)));
            }

            Diagnostics = values.AsReadOnly();
            EvaluatedInvariantCount = evaluatedInvariantCount;
        }

        public string Target { get; }
        public bool Valid => Diagnostics.Count == 0;
        public IReadOnlyList<WorldValidationDiagnostic> Diagnostics { get; }
        public int EvaluatedInvariantCount { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var diagnostics = new List<object?>();
            for (var index = 0; index < Diagnostics.Count; index++) diagnostics.Add(Diagnostics[index].ToData());

            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = SchemaId,
                ["invariantCatalogVersion"] = WorldInvariantCatalog.CatalogVersion,
                ["target"] = Target,
                ["valid"] = Valid,
                ["evaluatedInvariantCount"] = EvaluatedInvariantCount,
                ["diagnosticCount"] = Diagnostics.Count,
                ["diagnostics"] = diagnostics.AsReadOnly()
            });
        }
    }

    public sealed class WorldValidatorDescriptor
    {
        public WorldValidatorDescriptor(string invariantId, string machineCode, string category)
        {
            InvariantId = invariantId ?? throw new ArgumentNullException(nameof(invariantId));
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            Category = category ?? throw new ArgumentNullException(nameof(category));
        }

        public string InvariantId { get; }
        public string MachineCode { get; }
        public string Category { get; }
    }

    public static class WorldValidationEngine
    {
        private static readonly IReadOnlyList<WorldValidatorDescriptor> Descriptors = BuildInventory();

        public static IReadOnlyList<WorldValidatorDescriptor> ValidatorInventory => Descriptors;

        public static WorldValidationResult ValidateCurrent(WorldState state)
        {
            if (state is null) throw new ArgumentNullException(nameof(state));
            return ValidateCandidate(WorldStateCandidate.FromState(state), "current");
        }

        public static WorldValidationResult ValidateCandidate(WorldStateCandidate candidate, string target = "proposed")
        {
            if (candidate is null) throw new ArgumentNullException(nameof(candidate));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var diagnostics = new List<WorldValidationDiagnostic>();
            var violations = WorldStateValidator.ValidateCandidate(candidate);
            for (var index = 0; index < violations.Count; index++)
            {
                diagnostics.Add(ToDiagnostic(violations[index]));
            }

            diagnostics.Sort(CompareDiagnostics);
            return new WorldValidationResult(target, diagnostics, Descriptors.Count);
        }

        /// <summary>
        /// Reconciles the validator registrations with the independently declared invariant
        /// universe. Empty means no invariant/rejection identity is unclassified or multiply owned.
        /// </summary>
        public static IReadOnlyList<string> FindInventoryIssues()
        {
            return FindInventoryIssues(Descriptors);
        }

        public static IReadOnlyList<string> FindInventoryIssues(IEnumerable<WorldValidatorDescriptor> inventory)
        {
            if (inventory == null) throw new ArgumentNullException(nameof(inventory));
            var issues = new List<string>();
            var expected = new Dictionary<string, WorldInvariantDefinition>(StringComparer.Ordinal);
            for (var index = 0; index < WorldInvariantCatalog.All.Count; index++)
            {
                var invariant = WorldInvariantCatalog.All[index];
                if (!expected.TryAdd(invariant.InvariantId, invariant))
                {
                    issues.Add("duplicate-catalog:" + invariant.InvariantId);
                }
            }

            var actual = new Dictionary<string, WorldValidatorDescriptor>(StringComparer.Ordinal);
            foreach (var descriptor in inventory)
            {
                if (descriptor == null)
                {
                    issues.Add("null-validator");
                    continue;
                }
                if (!actual.TryAdd(descriptor.InvariantId, descriptor))
                {
                    issues.Add("duplicate-validator:" + descriptor.InvariantId);
                }
            }

            foreach (var pair in expected)
            {
                if (!actual.TryGetValue(pair.Key, out var descriptor))
                {
                    issues.Add("missing-validator:" + pair.Key);
                    continue;
                }

                if (!string.Equals(pair.Value.MachineCode, descriptor.MachineCode, StringComparison.Ordinal) ||
                    !string.Equals(pair.Value.Category, descriptor.Category, StringComparison.Ordinal))
                {
                    issues.Add("classification-mismatch:" + pair.Key);
                }
            }

            foreach (var pair in actual)
            {
                if (!expected.ContainsKey(pair.Key)) issues.Add("unclassified-validator:" + pair.Key);
            }

            issues.Sort(StringComparer.Ordinal);
            return issues.AsReadOnly();
        }

        private static IReadOnlyList<WorldValidatorDescriptor> BuildInventory()
        {
            var values = new List<WorldValidatorDescriptor>();
            var registered = WorldStateValidator.RegisteredInvariants;
            for (var index = 0; index < registered.Count; index++)
            {
                var invariant = registered[index];
                values.Add(new WorldValidatorDescriptor(
                    invariant.InvariantId,
                    invariant.MachineCode,
                    invariant.Category));
            }

            values.Sort((left, right) => StringComparer.Ordinal.Compare(left.InvariantId, right.InvariantId));
            return values.AsReadOnly();
        }

        private static WorldValidationDiagnostic ToDiagnostic(WorldStateViolation violation)
        {
            var context = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var pair in violation.RemediationContext) context.Add(pair.Key, pair.Value);

            return new WorldValidationDiagnostic(
                violation.Invariant.MachineCode,
                WorldDiagnosticSeverity.Error,
                violation.Resource,
                violation.Path,
                violation.Invariant.InvariantId,
                violation.Message,
                new ReadOnlyDictionary<string, object?>(context));
        }

        private static int CompareDiagnostics(WorldValidationDiagnostic left, WorldValidationDiagnostic right)
        {
            var comparison = left.Severity.CompareTo(right.Severity);
            if (comparison != 0) return comparison;
            comparison = StringComparer.Ordinal.Compare(left.InvariantId, right.InvariantId);
            if (comparison != 0) return comparison;
            comparison = StringComparer.Ordinal.Compare(left.Resource, right.Resource);
            if (comparison != 0) return comparison;
            comparison = StringComparer.Ordinal.Compare(left.Path, right.Path);
            if (comparison != 0) return comparison;
            comparison = StringComparer.Ordinal.Compare(left.MachineCode, right.MachineCode);
            return comparison != 0 ? comparison : StringComparer.Ordinal.Compare(left.Message, right.Message);
        }
    }

    public interface IWorldValidationService
    {
        CapabilityInvocationResult ValidateCurrent(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult ValidateProposed(IReadOnlyDictionary<string, object?> request);
    }

    public sealed class UnavailableWorldValidationService : IWorldValidationService
    {
        public CapabilityInvocationResult ValidateCurrent(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult ValidateProposed(IReadOnlyDictionary<string, object?> request) => Unavailable();

        private static CapabilityInvocationResult Unavailable()
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.state_unavailable",
                "No canonical world state is bound to this runtime instance.",
                "$",
                new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)),
                true,
                "Bind a canonical world authoring session before invoking validation capabilities."));
        }
    }
}
