using System;
using System.Collections.Generic;
using System.Reflection;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    public sealed class MutationSurfaceIssue
    {
        public MutationSurfaceIssue(string code, string subject, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Subject { get; }
        public string Message { get; }
    }

    public sealed class MutationSurfaceReport
    {
        internal MutationSurfaceReport(IReadOnlyList<MutationSurfaceIssue> issues)
        {
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public bool IsConformant => Issues.Count == 0;
        public IReadOnlyList<MutationSurfaceIssue> Issues { get; }
    }

    /// <summary>
    /// Mechanically closes the HK04 mutation surface. Contract metadata is reconciled with two
    /// independently derived executable surfaces: transactional-marker handlers and handlers that
    /// structurally carry actual canonical write authority. The latter does not consult SideEffect
    /// or transaction policy metadata and therefore cannot disappear merely by relabelling a route.
    /// </summary>
    public static class MutationSurfaceConformance
    {
        public static MutationSurfaceReport Evaluate(
            ComposedContract contract,
            IEnumerable<Assembly> effectiveAssemblies)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));
            if (effectiveAssemblies == null) throw new ArgumentNullException(nameof(effectiveAssemblies));

            var assemblies = CopyAssemblies(effectiveAssemblies);
            var issues = new List<MutationSurfaceIssue>();
            var discoveredMutation = new HashSet<CapabilityKey>();
            var declaredTransactional = new HashSet<CapabilityKey>();
            foreach (var definition in contract.Definitions)
            {
                if (definition.SideEffect == SideEffectClass.CanonicalMutation)
                {
                    discoveredMutation.Add(definition.Key);
                    if (definition.Concurrency?.Class != ConcurrencyClass.OptimisticVersioned ||
                        definition.Idempotency?.Class != IdempotencyClass.IdempotentWithKey)
                    {
                        issues.Add(new MutationSurfaceIssue(
                            "mutation-surface.unsafe-semantics",
                            definition.Key.ToString(),
                            "Canonical mutations must declare optimistic versioning and idempotency with a key."));
                    }
                }

                if (definition.Policy?.TransactionRequirement == TransactionRequirement.CanonicalTransaction)
                {
                    declaredTransactional.Add(definition.Key);
                }
            }

            var effectiveTransactional = EnumerateEffectiveTransactionalHandlers(assemblies, issues);
            var effectiveWriteAuthority = EnumerateEffectiveWriteAuthorityHandlers(assemblies, issues);
            var dispatcher = new HashSet<CapabilityKey>(contract.RouteKeys);

            CompareSets(discoveredMutation, declaredTransactional, "transaction-policy", issues);
            CompareSets(discoveredMutation, effectiveTransactional, "effective-transaction-handler", issues);
            CompareSets(discoveredMutation, effectiveWriteAuthority, "effective-write-authority", issues);

            foreach (var key in discoveredMutation)
            {
                if (!dispatcher.Contains(key))
                {
                    issues.Add(new MutationSurfaceIssue(
                        "mutation-surface.dispatcher-omission",
                        key.ToString(),
                        "A discovered canonical mutation is missing from the effective dispatcher."));
                }
            }

            foreach (var key in effectiveTransactional)
            {
                if (!dispatcher.Contains(key))
                {
                    issues.Add(new MutationSurfaceIssue(
                        "mutation-surface.handler-not-dispatched",
                        key.ToString(),
                        "An effective transactional mutation handler is not bound into the canonical dispatcher."));
                }
            }

            foreach (var key in effectiveWriteAuthority)
            {
                if (!dispatcher.Contains(key))
                {
                    issues.Add(new MutationSurfaceIssue(
                        "mutation-surface.write-authority-not-dispatched",
                        key.ToString(),
                        "A public handler carrying canonical write authority exists outside the canonical dispatcher."));
                }
            }

            return new MutationSurfaceReport(issues.AsReadOnly());
        }

        private static IReadOnlyList<Assembly> CopyAssemblies(IEnumerable<Assembly> assemblies)
        {
            var result = new List<Assembly>();
            var visited = new HashSet<string>(StringComparer.Ordinal);
            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                {
                    throw new ArgumentException("Effective assembly collection cannot contain null entries.", nameof(assemblies));
                }

                var identity = assembly.FullName ?? assembly.GetName().Name ?? assembly.ToString();
                if (visited.Add(identity)) result.Add(assembly);
            }

            return result.AsReadOnly();
        }

        private static HashSet<CapabilityKey> EnumerateEffectiveTransactionalHandlers(
            IEnumerable<Assembly> assemblies,
            IList<MutationSurfaceIssue> issues)
        {
            var result = new HashSet<CapabilityKey>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract || !typeof(ITransactionalMutationHandler).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    AddHandlerKey(type, result, "transactional", issues);
                }
            }

            return result;
        }

        private static HashSet<CapabilityKey> EnumerateEffectiveWriteAuthorityHandlers(
            IEnumerable<Assembly> assemblies,
            IList<MutationSurfaceIssue> issues)
        {
            var result = new HashSet<CapabilityKey>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass ||
                        type.IsAbstract ||
                        !typeof(ICanonicalCapabilityHandler).IsAssignableFrom(type) ||
                        !MutationAuthorityInspector.TypeCarriesCanonicalWriteAuthority(type))
                    {
                        continue;
                    }

                    AddHandlerKey(type, result, "write-authority", issues);
                }
            }

            return result;
        }

        private static void AddHandlerKey(
            Type type,
            ISet<CapabilityKey> result,
            string surface,
            IList<MutationSurfaceIssue> issues)
        {
            var attributes = type.GetCustomAttributes(typeof(PublicCapabilityRouteAttribute), false);
            if (attributes.Length != 1)
            {
                issues.Add(new MutationSurfaceIssue(
                    "mutation-surface.binding-metadata-count",
                    type.FullName ?? type.Name,
                    "Every effective " + surface + " handler must declare exactly one public route identity."));
                return;
            }

            var attribute = (PublicCapabilityRouteAttribute)attributes[0];
            try
            {
                var key = new CapabilityKey(
                    attribute.CapabilityName,
                    ContractVersion.Parse(attribute.ContractVersion));
                if (!result.Add(key))
                {
                    issues.Add(new MutationSurfaceIssue(
                        "mutation-surface.duplicate-handler",
                        key.ToString(),
                        "More than one effective " + surface + " handler claims this mutation route."));
                }
            }
            catch (FormatException)
            {
                issues.Add(new MutationSurfaceIssue(
                    "mutation-surface.invalid-version",
                    type.FullName ?? type.Name,
                    surface + " route binding has an invalid canonical version."));
            }
        }

        private static void CompareSets(
            ISet<CapabilityKey> canonical,
            ISet<CapabilityKey> other,
            string surface,
            IList<MutationSurfaceIssue> issues)
        {
            foreach (var key in canonical)
            {
                if (!other.Contains(key))
                {
                    issues.Add(new MutationSurfaceIssue(
                        "mutation-surface.omission",
                        key.ToString(),
                        surface + " omits a canonical mutation route."));
                }
            }

            foreach (var key in other)
            {
                if (!canonical.Contains(key))
                {
                    issues.Add(new MutationSurfaceIssue(
                        "mutation-surface.extra",
                        key.ToString(),
                        surface + " contains a route not declared as a canonical mutation."));
                }
            }
        }
    }
}
