using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Arkus.Harness.Protocol;

[assembly: InternalsVisibleTo("Arkus.Harness.Runtime")]

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
    /// Canonical write capability. It is intentionally internal to the authoring/runtime
    /// boundary so ordinary public/scoped handlers cannot receive it through the public API.
    /// </summary>
    internal interface ICanonicalWorldMutationCommitter
    {
        CapabilityInvocationResult Apply(IReadOnlyDictionary<string, object?> request);
    }

    /// <summary>
    /// Authoring-owned authority binder. Runtime gets a narrow commit capability rather than
    /// the public planning service. TransactionalWorldAuthoringSession remains the implementation
    /// of HK04 semantics, but its public Apply method is treated as write authority by the
    /// independent runtime conformance oracle until the API can be narrowed without a source break.
    /// </summary>
    internal static class CanonicalWorldMutationAuthority
    {
        public static ICanonicalWorldMutationCommitter Bind(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (service is ICanonicalWorldMutationCommitter committer) return committer;
            if (service is TransactionalWorldAuthoringSession session) return new SessionCommitter(session);

            throw new ArgumentException(
                "Canonical mutation bindings require an Authoring-owned commit authority.",
                nameof(service));
        }

        private sealed class SessionCommitter : ICanonicalWorldMutationCommitter
        {
            private readonly TransactionalWorldAuthoringSession _session;

            public SessionCommitter(TransactionalWorldAuthoringSession session)
            {
                _session = session ?? throw new ArgumentNullException(nameof(session));
            }

            public CapabilityInvocationResult Apply(IReadOnlyDictionary<string, object?> request)
            {
                return _session.Apply(request);
            }
        }
    }

    public sealed class UnavailableWorldMutationService : IWorldMutationService, ICanonicalWorldMutationCommitter
    {
        public CapabilityInvocationResult Plan(IReadOnlyDictionary<string, object?> request) => Unavailable();
        public CapabilityInvocationResult DryRun(IReadOnlyDictionary<string, object?> request) => Unavailable();
        CapabilityInvocationResult ICanonicalWorldMutationCommitter.Apply(IReadOnlyDictionary<string, object?> request) => Unavailable();

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
