using System;
using System.Collections.Generic;
using System.Reflection;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    public sealed class RouteUniverseIssue
    {
        public RouteUniverseIssue(string code, string subject, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Subject { get; }
        public string Message { get; }
    }

    public sealed class PublicRouteFact
    {
        public PublicRouteFact(string providerId, CapabilityKey key, string implementationType)
        {
            ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
        }

        public string ProviderId { get; }
        public CapabilityKey Key { get; }
        public string ImplementationType { get; }
    }

    public sealed class RouteUniverseResult
    {
        internal RouteUniverseResult(IReadOnlyList<PublicRouteFact> routes, IReadOnlyList<RouteUniverseIssue> issues)
        {
            Routes = routes ?? throw new ArgumentNullException(nameof(routes));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public IReadOnlyList<PublicRouteFact> Routes { get; }
        public IReadOnlyList<RouteUniverseIssue> Issues { get; }
    }

    /// <summary>
    /// Independently enumerates concrete public capability handlers from effective assemblies.
    /// Provider/registry metadata is deliberately not an input: an unknown or unregistered
    /// provider must remain observable rather than being filtered out of its own proof universe.
    /// </summary>
    public static class RouteUniverse
    {
        public static RouteUniverseResult Enumerate(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return Enumerate(new[] { assembly });
        }

        public static RouteUniverseResult Enumerate(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var routes = new List<PublicRouteFact>();
            var issues = new List<RouteUniverseIssue>();
            var visitedAssemblies = new HashSet<string>(StringComparer.Ordinal);

            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                {
                    throw new ArgumentException("Route universe assemblies may not contain null entries.", nameof(assemblies));
                }

                var assemblyIdentity = assembly.FullName ?? assembly.GetName().Name ?? assembly.ToString();
                if (!visitedAssemblies.Add(assemblyIdentity))
                {
                    continue;
                }

                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract || !typeof(ICanonicalCapabilityHandler).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    var attributes = type.GetCustomAttributes(typeof(PublicCapabilityRouteAttribute), false);
                    if (attributes.Length != 1)
                    {
                        issues.Add(new RouteUniverseIssue(
                            "route-universe.binding-metadata-count",
                            type.FullName ?? type.Name,
                            "Every concrete public capability handler in an enumerated assembly must declare exactly one route identity."));
                        continue;
                    }

                    var attribute = (PublicCapabilityRouteAttribute)attributes[0];
                    try
                    {
                        routes.Add(new PublicRouteFact(
                            attribute.ProviderId,
                            new CapabilityKey(attribute.CapabilityName, ContractVersion.Parse(attribute.ContractVersion)),
                            type.FullName ?? type.Name));
                    }
                    catch (FormatException)
                    {
                        issues.Add(new RouteUniverseIssue(
                            "route-universe.invalid-version",
                            type.FullName ?? type.Name,
                            "Route binding metadata contains an invalid canonical contract version."));
                    }
                }
            }

            return new RouteUniverseResult(routes.AsReadOnly(), issues.AsReadOnly());
        }
    }

    public sealed class ConformanceIssue
    {
        public ConformanceIssue(string code, string subject, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Subject { get; }
        public string Message { get; }
    }

    public sealed class ContractConformanceReport
    {
        internal ContractConformanceReport(IReadOnlyList<ConformanceIssue> issues)
        {
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public bool IsConformant => Issues.Count == 0;
        public IReadOnlyList<ConformanceIssue> Issues { get; }
    }

    public static class CanonicalContractConformance
    {
        public static ContractConformanceReport Evaluate(ComposedContract contract, RouteUniverseResult routeUniverse)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (routeUniverse == null)
            {
                throw new ArgumentNullException(nameof(routeUniverse));
            }

            var issues = new List<ConformanceIssue>();
            foreach (var universeIssue in routeUniverse.Issues)
            {
                issues.Add(new ConformanceIssue(universeIssue.Code, universeIssue.Subject, universeIssue.Message));
            }

            var definitionKeys = new HashSet<CapabilityKey>();
            foreach (var definition in contract.Definitions)
            {
                definitionKeys.Add(definition.Key);
                foreach (var validationIssue in CanonicalContractValidator.Validate(definition))
                {
                    issues.Add(new ConformanceIssue(
                        "conformance.invalid-definition",
                        definition.Key.ToString(),
                        validationIssue.Code + ": " + validationIssue.Message));
                }
            }

            var routeKeys = new HashSet<CapabilityKey>(contract.RouteKeys);
            var independentKeys = new HashSet<CapabilityKey>();
            foreach (var route in routeUniverse.Routes)
            {
                if (!independentKeys.Add(route.Key))
                {
                    issues.Add(new ConformanceIssue(
                        "conformance.duplicate-effective-route",
                        route.Key.ToString(),
                        "Independent route enumeration found a duplicate public route identity."));
                }

                CapabilityDefinition? matching = null;
                foreach (var definition in contract.Definitions)
                {
                    if (definition.Key.Equals(route.Key))
                    {
                        matching = definition;
                        break;
                    }
                }

                if (matching != null && !string.Equals(matching.Provider.ProviderId, route.ProviderId, StringComparison.Ordinal))
                {
                    issues.Add(new ConformanceIssue(
                        "conformance.provider-binding-mismatch",
                        route.Key.ToString(),
                        "Effective route provider identity differs from the accepted canonical provider identity."));
                }
            }

            CompareSets(definitionKeys, routeKeys, "dispatcher", issues);
            CompareSets(definitionKeys, independentKeys, "independent-route-universe", issues);

            var discovery = contract.Dispatch(
                "system.describe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                new Dictionary<string, object?>(StringComparer.Ordinal));
            if (!discovery.Success || discovery.Data == null)
            {
                issues.Add(new ConformanceIssue(
                    "projection.discovery_failed",
                    "system.describe@1.0",
                    "Canonical discovery could not emit a schema-valid projection artifact."));
                return new ContractConformanceReport(issues.AsReadOnly());
            }

            foreach (var projectionIssue in CanonicalProjectionConformance.Compare(contract.Definitions, discovery.Data))
            {
                issues.Add(new ConformanceIssue(projectionIssue.Code, projectionIssue.Identity, projectionIssue.Message));
            }

            return new ContractConformanceReport(issues.AsReadOnly());
        }

        private static void CompareSets(
            ISet<CapabilityKey> canonical,
            ISet<CapabilityKey> other,
            string surface,
            IList<ConformanceIssue> issues)
        {
            foreach (var key in canonical)
            {
                if (!other.Contains(key))
                {
                    issues.Add(new ConformanceIssue(
                        "conformance.surface-omission",
                        key.ToString(),
                        surface + " omitted a canonical public capability."));
                }
            }

            foreach (var key in other)
            {
                if (!canonical.Contains(key))
                {
                    issues.Add(new ConformanceIssue(
                        "conformance.extra-public-route",
                        key.ToString(),
                        surface + " exposes a public route with no accepted canonical definition."));
                }
            }
        }
    }
}
