using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Arkus.Game.Validation;
using Arkus.Harness.Protocol;

[assembly: InternalsVisibleTo("Arkus.Harness.Runtime")]
[assembly: InternalsVisibleTo("Arkus.Harness.Tests")]

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// Public mutation-planning authority. This surface can propose and validate canonical
    /// changes but cannot commit authorable state.
    /// </summary>
    public interface IWorldMutationService
    {
        CapabilityInvocationResult Plan(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult DryRun(IReadOnlyDictionary<string, object?> request);
    }

    /// <summary>
    /// Canonical write capability. It is intentionally internal to the Authoring/Runtime
    /// boundary so ordinary public/scoped handlers cannot obtain commit authority from public API.
    /// </summary>
    internal interface ICanonicalWorldMutationCommitter
    {
        CapabilityInvocationResult Apply(
            IReadOnlyDictionary<string, object?> request,
            InvocationResourceBudget resourceBudget);
    }

    /// <summary>
    /// Authoring-owned authority binder. Runtime receives the narrow internal commit capability;
    /// public callers receive only IWorldMutationService planning/dry-run semantics.
    /// </summary>
    internal static class CanonicalWorldMutationAuthority
    {
        public static ICanonicalWorldMutationCommitter Bind(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (service is ICanonicalWorldMutationCommitter committer) return committer;

            throw new ArgumentException(
                "Canonical mutation bindings require an Authoring-owned commit authority.",
                nameof(service));
        }
    }

    public sealed class UnavailableWorldMutationService :
        IWorldMutationService,
        IWorldValidationService,
        IWorldProvenanceService,
        ICanonicalWorldMutationCommitter
    {
        private readonly UnavailableWorldValidationService _validation = new UnavailableWorldValidationService();

        public CapabilityInvocationResult Plan(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult DryRun(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult ValidateCurrent(IReadOnlyDictionary<string, object?> request) =>
            _validation.ValidateCurrent(request);
        public CapabilityInvocationResult ValidateProposed(IReadOnlyDictionary<string, object?> request) =>
            _validation.ValidateProposed(request);
        public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request) => Unavailable();
        CapabilityInvocationResult ICanonicalWorldMutationCommitter.Apply(
            IReadOnlyDictionary<string, object?> request,
            InvocationResourceBudget resourceBudget) => Unavailable();

        private static CapabilityInvocationResult Unavailable()
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.state_unavailable",
                "No transactional canonical world state is bound to this runtime instance.",
                "$",
                new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)),
                true,
                "Bind a transactional world authoring session before invoking mutation capabilities."));
        }
    }

    public sealed class WorldMutationChange
    {
        public WorldMutationChange(
            string resource,
            string action,
            IEnumerable<string> fields,
            IEnumerable<string> referencesAdded,
            IEnumerable<string> referencesRemoved)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Fields = Copy(fields, nameof(fields));
            ReferencesAdded = Copy(referencesAdded, nameof(referencesAdded));
            ReferencesRemoved = Copy(referencesRemoved, nameof(referencesRemoved));
        }

        public string Resource { get; }
        public string Action { get; }
        public IReadOnlyList<string> Fields { get; }
        public IReadOnlyList<string> ReferencesAdded { get; }
        public IReadOnlyList<string> ReferencesRemoved { get; }

        private static IReadOnlyList<string> Copy(IEnumerable<string> values, string parameterName)
        {
            if (values == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            var copy = new List<string>();
            foreach (var value in values)
            {
                copy.Add(value ?? throw new ArgumentException("Mutation change collections cannot contain null values.", parameterName));
            }

            return copy.AsReadOnly();
        }
    }
}
