using System;
using System.Collections.Generic;
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
        internal CapabilityInvocationContext(
            ComposedContract contract,
            CapabilityDefinition definition,
            InvocationResourceBudget resourceBudget)
        {
            Contract = contract ?? throw new ArgumentNullException(nameof(contract));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            ResourceBudget = resourceBudget ?? throw new ArgumentNullException(nameof(resourceBudget));
        }

        public ComposedContract Contract { get; }
        public CapabilityDefinition Definition { get; }
        public InvocationResourceBudget ResourceBudget { get; }
    }

    public sealed class CapabilityRoute
    {
        public CapabilityRoute(string providerId, CapabilityKey key, ICanonicalCapabilityHandler handler)
        {
            ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));

            if (MutationAuthorityInspector.CarriesCanonicalWriteAuthority(handler) &&
                !(handler is ITransactionalMutationHandler))
            {
                throw new ArgumentException(
                    "A public handler carrying canonical world write authority must implement the transactional mutation boundary.",
                    nameof(handler));
            }
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

        internal bool MatchesHandlerBinding()
        {
            try
            {
                var effective = FromHandler(Handler);
                return string.Equals(ProviderId, effective.ProviderId, StringComparison.Ordinal) && Key.Equals(effective.Key);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
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
}
