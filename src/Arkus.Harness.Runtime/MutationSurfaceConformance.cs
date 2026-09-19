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
    /// Mechanically closes the HK04 mutation surface: canonical mutation definitions,
    /// canonical-transaction definitions, effective transactional handlers and dispatcher
    /// bindings must identify the same capability keys.
    /// </summary>
    public static class MutationSurfaceConformance
    {
        public static MutationSurfaceReport Evaluate(
            ComposedContract contract,
            IEnumerable<Assembly> effectiveAssemblies)
        {
            if (contract == null) throw new ArgumentNullException(nameof(contract));
            if (effectiveAssemblies == null) throw new ArgumentNullException(nameof(effectiveAssemblies));

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

            var effectiveTransactional = EnumerateEffectiveTransactionalHandlers(effectiveAssemblies, issues);
            var dispatcher = new HashSet<CapabilityKey>(contract.RouteKeys);

            CompareSets(discoveredMutation, declaredTransactional, "transaction-policy", issues);
            CompareSets(discoveredMutation, effectiveTransactional, "effective-transaction-handler", issues);

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

            return new MutationSurfaceReport(issues.AsReadOnly());
        }

        private static HashSet<CapabilityKey> EnumerateEffectiveTransactionalHandlers(
            IEnumerable<Assembly> assemblies,
            IList<MutationSurfaceIssue> issues)
        {
            var result = new HashSet<CapabilityKey>();
            var visited = new HashSet<string>(StringComparer.Ordinal);
            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                {
                    throw new ArgumentException("Effective assembly collection cannot contain null entries.", nameof(assemblies));
                }

                var identity = assembly.FullName ?? assembly.GetName().Name ?? assembly.ToString();
                if (!visited.Add(identity)) continue;

                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract || !typeof(ITransactionalMutationHandler).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    var attributes = type.GetCustomAttributes(typeof(PublicCapabilityRouteAttribute), false);
                    if (attributes.Length != 1)
                    {
                        issues.Add(new MutationSurfaceIssue(
                            "mutation-surface.binding-metadata-count",
                            type.FullName ?? type.Name,
                            "Every effective transactional handler must declare exactly one public route identity."));
                        continue;
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
                                "More than one effective transactional handler claims this mutation route."));
                        }
                    }
                    catch (FormatException)
                    {
                        issues.Add(new MutationSurfaceIssue(
                            "mutation-surface.invalid-version",
                            type.FullName ?? type.Name,
                            "Transactional route binding has an invalid canonical version."));
                    }
                }
            }

            return result;
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
