using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PublicCapabilityRouteAttribute : Attribute
    {
        public PublicCapabilityRouteAttribute(string providerId, string capabilityName, string contractVersion)
        {
            ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
            CapabilityName = capabilityName ?? throw new ArgumentNullException(nameof(capabilityName));
            ContractVersion = contractVersion ?? throw new ArgumentNullException(nameof(contractVersion));
        }

        public string ProviderId { get; }
        public string CapabilityName { get; }
        public string ContractVersion { get; }
    }

    public interface ICanonicalCapabilityHandler
    {
        CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request);
    }

    public sealed class CapabilityInvocationContext
    {
        internal CapabilityInvocationContext(ComposedContract contract, CapabilityDefinition definition)
        {
            Contract = contract ?? throw new ArgumentNullException(nameof(contract));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public ComposedContract Contract { get; }
        public CapabilityDefinition Definition { get; }
    }

    public sealed class CapabilityRoute
    {
        public CapabilityRoute(string providerId, CapabilityKey key, ICanonicalCapabilityHandler handler)
        {
            ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public string ProviderId { get; }
        public CapabilityKey Key { get; }
        public ICanonicalCapabilityHandler Handler { get; }

        public static CapabilityRoute FromHandler(ICanonicalCapabilityHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var attributes = handler.GetType().GetCustomAttributes(typeof(PublicCapabilityRouteAttribute), false);
            if (attributes.Length != 1)
            {
                throw new ArgumentException(
                    "Canonical public handlers must declare exactly one PublicCapabilityRouteAttribute.",
                    nameof(handler));
            }

            var attribute = (PublicCapabilityRouteAttribute)attributes[0];
            return new CapabilityRoute(
                attribute.ProviderId,
                new CapabilityKey(attribute.CapabilityName, ContractVersion.Parse(attribute.ContractVersion)),
                handler);
        }
    }

    public sealed class ProviderDescriptor
    {
        public ProviderDescriptor(
            string providerId,
            ProviderKind kind,
            string scope,
            IEnumerable<string> ownedNamespaces)
        {
            ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
            Kind = kind;
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            if (ownedNamespaces == null)
            {
                throw new ArgumentNullException(nameof(ownedNamespaces));
            }

            var namespaces = new List<string>();
            foreach (var value in ownedNamespaces)
            {
                namespaces.Add(value ?? throw new ArgumentException("Owned namespaces may not contain null entries.", nameof(ownedNamespaces)));
            }

            OwnedNamespaces = namespaces.AsReadOnly();
        }

        public string ProviderId { get; }
        public ProviderKind Kind { get; }
        public string Scope { get; }
        public IReadOnlyList<string> OwnedNamespaces { get; }
    }

    public sealed class CanonicalProviderContribution
    {
        public CanonicalProviderContribution(
            ProviderDescriptor descriptor,
            IEnumerable<CapabilityDefinition> definitions,
            IEnumerable<CapabilityRoute> routes)
        {
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            Definitions = CopyDefinitions(definitions);
            Routes = CopyRoutes(routes);
        }

        public ProviderDescriptor Descriptor { get; }
        public IReadOnlyList<CapabilityDefinition> Definitions { get; }
        public IReadOnlyList<CapabilityRoute> Routes { get; }

        private static IReadOnlyList<CapabilityDefinition> CopyDefinitions(IEnumerable<CapabilityDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            var copy = new List<CapabilityDefinition>();
            foreach (var definition in definitions)
            {
                copy.Add(definition ?? throw new ArgumentException("Definitions may not contain null entries.", nameof(definitions)));
            }

            return copy.AsReadOnly();
        }

        private static IReadOnlyList<CapabilityRoute> CopyRoutes(IEnumerable<CapabilityRoute> routes)
        {
            if (routes == null)
            {
                throw new ArgumentNullException(nameof(routes));
            }

            var copy = new List<CapabilityRoute>();
            foreach (var route in routes)
            {
                copy.Add(route ?? throw new ArgumentException("Routes may not contain null entries.", nameof(routes)));
            }

            return copy.AsReadOnly();
        }
    }

    public sealed class CompositionIssue
    {
        public CompositionIssue(string code, string path, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Code { get; }
        public string Path { get; }
        public string Message { get; }
    }

    public sealed class ContractCompositionResult
    {
        internal ContractCompositionResult(ComposedContract? contract, IReadOnlyList<CompositionIssue> issues)
        {
            Contract = contract;
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public bool Success => Contract != null && Issues.Count == 0;
        public ComposedContract? Contract { get; }
        public IReadOnlyList<CompositionIssue> Issues { get; }
    }

    public static class ContractComposer
    {
        public static ContractCompositionResult Compose(
            CanonicalProviderContribution baseContribution,
            IEnumerable<CanonicalProviderContribution>? scopedContributions = null)
        {
            if (baseContribution == null)
            {
                throw new ArgumentNullException(nameof(baseContribution));
            }

            var contributions = new List<CanonicalProviderContribution> { baseContribution };
            if (scopedContributions != null)
            {
                foreach (var contribution in scopedContributions)
                {
                    contributions.Add(contribution ?? throw new ArgumentException("Scoped contributions may not contain null entries.", nameof(scopedContributions)));
                }
            }

            var issues = new List<CompositionIssue>();
            var providerIds = new HashSet<string>(StringComparer.Ordinal);
            var namespaces = new List<NamespaceOwner>();
            var definitions = new Dictionary<CapabilityKey, CapabilityDefinition>();
            var routes = new Dictionary<CapabilityKey, CapabilityRoute>();

            for (var index = 0; index < contributions.Count; index++)
            {
                var contribution = contributions[index];
                var descriptor = contribution.Descriptor;
                var path = "$.providers[" + index + "]";

                ValidateDescriptor(descriptor, index == 0, path, providerIds, namespaces, issues);
                ValidateContribution(contribution, path, definitions, routes, issues);
            }

            if (issues.Count != 0)
            {
                return new ContractCompositionResult(null, issues.AsReadOnly());
            }

            foreach (var definition in definitions)
            {
                if (!routes.ContainsKey(definition.Key))
                {
                    issues.Add(new CompositionIssue(
                        "composition.definition_without_route",
                        definition.Key.ToString(),
                        "Every public canonical definition must have an implementation binding."));
                }
            }

            foreach (var route in routes)
            {
                if (!definitions.ContainsKey(route.Key))
                {
                    issues.Add(new CompositionIssue(
                        "composition.route_without_definition",
                        route.Key.ToString(),
                        "A public route cannot exist without an accepted canonical definition."));
                }
            }

            if (issues.Count != 0)
            {
                return new ContractCompositionResult(null, issues.AsReadOnly());
            }

            return new ContractCompositionResult(
                new ComposedContract(definitions, routes),
                issues.AsReadOnly());
        }

        private static void ValidateDescriptor(
            ProviderDescriptor descriptor,
            bool isBase,
            string path,
            ISet<string> providerIds,
            IList<NamespaceOwner> namespaces,
            IList<CompositionIssue> issues)
        {
            if (!CanonicalContractValidator.IsCanonicalIdentifier(descriptor.ProviderId) ||
                !CanonicalContractValidator.IsCanonicalIdentifier(descriptor.Scope))
            {
                issues.Add(new CompositionIssue(
                    "composition.invalid_provider_identity",
                    path,
                    "Provider identity and scope must use portable canonical identifiers."));
            }

            if ((isBase && descriptor.Kind != ProviderKind.Base) ||
                (!isBase && descriptor.Kind != ProviderKind.Scoped))
            {
                issues.Add(new CompositionIssue(
                    descriptor.Kind == ProviderKind.TransportProjection
                        ? "composition.transport_registry_forbidden"
                        : "composition.invalid_provider_kind",
                    path + ".kind",
                    "Only the base provider and reviewed scoped semantic providers may contribute to the canonical inventory."));
            }

            if (!providerIds.Add(descriptor.ProviderId))
            {
                issues.Add(new CompositionIssue(
                    "composition.duplicate_provider",
                    path + ".id",
                    "Provider identities must be unique within one composed contract."));
            }

            if (descriptor.OwnedNamespaces.Count == 0)
            {
                issues.Add(new CompositionIssue(
                    "composition.missing_namespace",
                    path + ".namespaces",
                    "Every provider must own at least one explicit canonical capability namespace."));
            }

            foreach (var currentNamespace in descriptor.OwnedNamespaces)
            {
                if (!CanonicalContractValidator.IsCanonicalIdentifier(currentNamespace))
                {
                    issues.Add(new CompositionIssue(
                        "composition.invalid_namespace",
                        path + ".namespaces",
                        "Provider namespaces must be portable canonical identifiers."));
                    continue;
                }

                foreach (var existing in namespaces)
                {
                    if (NamespacesOverlap(currentNamespace, existing.Value))
                    {
                        issues.Add(new CompositionIssue(
                            "composition.namespace_conflict",
                            path + ".namespaces",
                            "Provider namespace '" + currentNamespace + "' conflicts with namespace '" + existing.Value + "' owned by '" + existing.ProviderId + "'."));
                    }
                }

                namespaces.Add(new NamespaceOwner(currentNamespace, descriptor.ProviderId));
            }
        }

        private static void ValidateContribution(
            CanonicalProviderContribution contribution,
            string path,
            IDictionary<CapabilityKey, CapabilityDefinition> definitions,
            IDictionary<CapabilityKey, CapabilityRoute> routes,
            IList<CompositionIssue> issues)
        {
            foreach (var definition in contribution.Definitions)
            {
                if (!DefinitionMatchesDescriptor(definition, contribution.Descriptor))
                {
                    issues.Add(new CompositionIssue(
                        "composition.definition_provider_mismatch",
                        path + ".definitions",
                        "Definition provider/scope/namespace metadata does not match the contributing provider descriptor."));
                }

                foreach (var validationIssue in CanonicalContractValidator.Validate(definition))
                {
                    issues.Add(new CompositionIssue(validationIssue.Code, path + validationIssue.Path.Substring(1), validationIssue.Message));
                }

                if (definitions.ContainsKey(definition.Key))
                {
                    issues.Add(new CompositionIssue(
                        "composition.duplicate_capability",
                        definition.Key.ToString(),
                        "Duplicate/conflicting canonical capability identity is forbidden."));
                }
                else
                {
                    definitions.Add(definition.Key, definition);
                }
            }

            foreach (var route in contribution.Routes)
            {
                if (!string.Equals(route.ProviderId, contribution.Descriptor.ProviderId, StringComparison.Ordinal))
                {
                    issues.Add(new CompositionIssue(
                        "composition.route_provider_mismatch",
                        route.Key.ToString(),
                        "Implementation binding provider does not match its canonical contribution."));
                }

                if (routes.ContainsKey(route.Key))
                {
                    issues.Add(new CompositionIssue(
                        "composition.duplicate_route",
                        route.Key.ToString(),
                        "Duplicate/conflicting public implementation binding is forbidden."));
                }
                else
                {
                    routes.Add(route.Key, route);
                }
            }
        }

        private static bool DefinitionMatchesDescriptor(CapabilityDefinition definition, ProviderDescriptor descriptor)
        {
            if (!string.Equals(definition.Provider.ProviderId, descriptor.ProviderId, StringComparison.Ordinal) ||
                definition.Provider.Kind != descriptor.Kind ||
                !string.Equals(definition.Provider.Scope, descriptor.Scope, StringComparison.Ordinal))
            {
                return false;
            }

            foreach (var ownedNamespace in descriptor.OwnedNamespaces)
            {
                if (string.Equals(definition.Provider.CapabilityNamespace, ownedNamespace, StringComparison.Ordinal) &&
                    CanonicalContractValidator.IsWithinNamespace(definition.Key.Name, ownedNamespace))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool NamespacesOverlap(string left, string right)
        {
            return string.Equals(left, right, StringComparison.Ordinal) ||
                left.StartsWith(right + ".", StringComparison.Ordinal) ||
                right.StartsWith(left + ".", StringComparison.Ordinal);
        }

        private sealed class NamespaceOwner
        {
            public NamespaceOwner(string value, string providerId)
            {
                Value = value;
                ProviderId = providerId;
            }

            public string Value { get; }
            public string ProviderId { get; }
        }
    }

    public sealed class ComposedContract
    {
        private readonly IReadOnlyDictionary<CapabilityKey, CapabilityDefinition> _definitionsByKey;
        private readonly IReadOnlyDictionary<CapabilityKey, CapabilityRoute> _routesByKey;

        internal ComposedContract(
            IDictionary<CapabilityKey, CapabilityDefinition> definitions,
            IDictionary<CapabilityKey, CapabilityRoute> routes)
        {
            var definitionCopy = new Dictionary<CapabilityKey, CapabilityDefinition>(definitions);
            var routeCopy = new Dictionary<CapabilityKey, CapabilityRoute>(routes);
            _definitionsByKey = new ReadOnlyDictionary<CapabilityKey, CapabilityDefinition>(definitionCopy);
            _routesByKey = new ReadOnlyDictionary<CapabilityKey, CapabilityRoute>(routeCopy);

            var definitionList = new List<CapabilityDefinition>(definitionCopy.Values);
            definitionList.Sort(CompareDefinitions);
            Definitions = definitionList.AsReadOnly();
            Projection = new CanonicalContractProjection(Definitions);
        }

        public IReadOnlyList<CapabilityDefinition> Definitions { get; }
        public CanonicalContractProjection Projection { get; }

        internal IReadOnlyCollection<CapabilityKey> RouteKeys => _routesByKey.Keys;

        public CapabilityInvocationResult Dispatch(
            string capabilityName,
            ContractVersionRange acceptedVersions,
            IReadOnlyDictionary<string, object?> request)
        {
            if (capabilityName == null)
            {
                throw new ArgumentNullException(nameof(capabilityName));
            }

            if (acceptedVersions == null)
            {
                throw new ArgumentNullException(nameof(acceptedVersions));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var available = new List<CapabilityDefinition>();
            foreach (var definition in Definitions)
            {
                if (string.Equals(definition.Key.Name, capabilityName, StringComparison.Ordinal))
                {
                    available.Add(definition);
                }
            }

            if (available.Count == 0)
            {
                return Failure(
                    "contract.unknown_capability",
                    "Unknown canonical capability.",
                    "$.capability",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["capability"] = capabilityName
                    },
                    false,
                    "Call system.describe and select a discovered capability identity.");
            }

            CapabilityDefinition? selected = null;
            foreach (var definition in available)
            {
                if (acceptedVersions.Accepts(definition.Key.Version) &&
                    (selected == null || definition.Key.Version.CompareTo(selected.Key.Version) > 0))
                {
                    selected = definition;
                }
            }

            if (selected == null)
            {
                var versions = new List<object?>();
                foreach (var definition in available)
                {
                    versions.Add(definition.Key.Version.ToString());
                }

                return Failure(
                    "contract.unsupported_version",
                    "No discovered contract version satisfies the requested version range.",
                    "$.version",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["requested"] = acceptedVersions.ToString(),
                        ["available"] = versions.AsReadOnly()
                    },
                    false,
                    "Negotiate one of the versions returned by system.describe.");
            }

            if (selected.RequestSchema == null)
            {
                return InternalFailure("contract.missing_request_schema", "Accepted canonical definition has no request schema.");
            }

            var requestIssues = selected.RequestSchema.ValidateValue(request);
            if (requestIssues.Count != 0)
            {
                var first = requestIssues[0];
                return Failure(
                    "contract.invalid_request",
                    first.Message,
                    first.Path,
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["schemaCode"] = first.Code,
                        ["capability"] = selected.Key.ToString()
                    },
                    false,
                    "Repair the request so it conforms to the discovered requestSchema.");
            }

            if (!_routesByKey.TryGetValue(selected.Key, out var route))
            {
                return InternalFailure("contract.route_missing", "Accepted canonical capability has no implementation binding.");
            }

            var result = route.Handler.Invoke(new CapabilityInvocationContext(this, selected), request);
            if (result.Success)
            {
                if (result.Data == null || selected.SuccessSchema == null)
                {
                    return InternalFailure("contract.invalid_result", "Capability returned success without a schema-valid result payload.");
                }

                var resultIssues = selected.SuccessSchema.ValidateValue(result.Data);
                if (resultIssues.Count != 0)
                {
                    return InternalFailure("contract.invalid_result", "Capability success payload violated its canonical success schema.");
                }

                return result;
            }

            if (result.Error == null || selected.ErrorSchema == null)
            {
                return InternalFailure("contract.invalid_error", "Capability returned failure without a canonical structured error.");
            }

            var errorIssues = selected.ErrorSchema.ValidateValue(result.Error.ToData());
            if (errorIssues.Count != 0)
            {
                return InternalFailure("contract.invalid_error", "Capability error payload violated the canonical structured error schema.");
            }

            return result;
        }

        private static CapabilityInvocationResult InternalFailure(string code, string message)
        {
            return Failure(
                code,
                message,
                "$",
                new Dictionary<string, object?>(StringComparer.Ordinal),
                false,
                "This is a canonical runtime invariant failure; do not retry unchanged.");
        }

        private static CapabilityInvocationResult Failure(
            string code,
            string message,
            string path,
            IReadOnlyDictionary<string, object?> context,
            bool retryable,
            string repairHint)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                code,
                message,
                path,
                context,
                retryable,
                repairHint));
        }

        private static int CompareDefinitions(CapabilityDefinition left, CapabilityDefinition right)
        {
            var name = string.Compare(left.Key.Name, right.Key.Name, StringComparison.Ordinal);
            return name != 0 ? name : left.Key.Version.CompareTo(right.Key.Version);
        }
    }

    [PublicCapabilityRoute("arkus.base", "system.describe", "1.0")]
    public sealed class SystemDescribeHandler : ICanonicalCapabilityHandler
    {
        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return CapabilityInvocationResult.Succeeded(context.Contract.Projection.ToData());
        }
    }

    public static class BaseContract
    {
        public static CanonicalProviderContribution CreateContribution()
        {
            var provider = new ProviderMetadata("arkus.base", ProviderKind.Base, "base", "system");
            var definition = new CapabilityDefinition(
                new CapabilityKey("system.describe", new ContractVersion(1, 0)),
                provider,
                CanonicalContractSchemas.EmptyObject(),
                CanonicalContractSchemas.SystemDescribeSuccess(),
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ReadOnly,
                DeterminismClass.Deterministic,
                System.Array.Empty<string>(),
                new[] { "returns-the-complete-composed-canonical-inventory" },
                new ConcurrencySemantics(ConcurrencyClass.ParallelSafe),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(false, true),
                new PolicySemantics(
                    PrivilegeClass.PublicRead,
                    TransactionRequirement.ReadOnlyEnvelope,
                    ProvenanceRequirement.Required),
                new CostSemantics(1, "single canonical inventory projection"));

            return new CanonicalProviderContribution(
                new ProviderDescriptor("arkus.base", ProviderKind.Base, "base", new[] { "system" }),
                new[] { definition },
                new[] { CapabilityRoute.FromHandler(new SystemDescribeHandler()) });
        }

        public static ComposedContract Compose()
        {
            var result = ContractComposer.Compose(CreateContribution());
            if (!result.Success || result.Contract == null)
            {
                throw new InvalidOperationException("The built-in base canonical contract must compose successfully.");
            }

            return result.Contract;
        }
    }

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

    public static class RouteUniverse
    {
        public static RouteUniverseResult Enumerate(Assembly assembly, IEnumerable<string> providerIds)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (providerIds == null)
            {
                throw new ArgumentNullException(nameof(providerIds));
            }

            var selectedProviders = new HashSet<string>(providerIds, StringComparer.Ordinal);
            var routes = new List<PublicRouteFact>();
            var issues = new List<RouteUniverseIssue>();

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
                if (!selectedProviders.Contains(attribute.ProviderId))
                {
                    continue;
                }

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

            foreach (var projectionIssue in CanonicalProjectionConformance.Compare(contract.Definitions, contract.Projection))
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
