using System;

namespace Arkus.Harness.Protocol
{
    /// <summary>
    /// Public portable identity rules shared by canonical contract producers and the Arkus composer.
    /// This surface is deliberately semantic and contains no Runtime, transport, or engine dependency.
    /// </summary>
    public static class CanonicalIdentityRules
    {
        private const string LogicalReferenceRoot = "ref.";

        public static bool IsCanonicalIdentifier(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return CanonicalContractValidator.IsCanonicalIdentifier(value);
        }

        public static bool IsWithinNamespace(string capabilityName, string capabilityNamespace)
        {
            if (capabilityName == null)
            {
                throw new ArgumentNullException(nameof(capabilityName));
            }

            if (capabilityNamespace == null)
            {
                throw new ArgumentNullException(nameof(capabilityNamespace));
            }

            return CanonicalContractValidator.IsWithinNamespace(capabilityName, capabilityNamespace);
        }

        /// <summary>
        /// Logical-reference namespaces occupy an Arkus-owned lexical space instead of reusing CLR,
        /// engine, assembly or transport namespaces. The provider-aware overload additionally binds
        /// that space to the canonical provider identity that owns the reference semantics.
        /// </summary>
        public static bool IsPortableLogicalReferenceNamespace(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return IsCanonicalIdentifier(value) &&
                value.StartsWith(LogicalReferenceRoot, StringComparison.Ordinal) &&
                value.Length > LogicalReferenceRoot.Length;
        }

        public static bool IsPortableLogicalReferenceNamespace(string value, string providerId)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (providerId == null)
            {
                throw new ArgumentNullException(nameof(providerId));
            }

            if (!IsPortableLogicalReferenceNamespace(value) || !IsCanonicalIdentifier(providerId))
            {
                return false;
            }

            var providerPrefix = LogicalReferenceRoot + providerId + ".";
            return value.StartsWith(providerPrefix, StringComparison.Ordinal) &&
                value.Length > providerPrefix.Length;
        }
    }
}
