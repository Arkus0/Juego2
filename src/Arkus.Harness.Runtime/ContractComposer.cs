using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
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
            if (!CanonicalIdentityRules.IsCanonicalIdentifier(descriptor.ProviderId) ||
                !CanonicalIdentityRules.IsCanonicalIdentifier(descriptor.Scope))
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
                if (!CanonicalIdentityRules.IsCanonicalIdentifier(currentNamespace))
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

                if (!route.MatchesHandlerBinding())
                {
                    issues.Add(new CompositionIssue(
                        "composition.binding_metadata_mismatch",
                        route.Key.ToString(),
                        "Registered route identity must match the independently declared handler binding identity."));
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
                    CanonicalIdentityRules.IsWithinNamespace(definition.Key.Name, ownedNamespace))
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
}
