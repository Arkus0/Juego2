using System;

namespace Arkus.Harness.Protocol
{
    /// <summary>
    /// Public portable identity rules shared by canonical contract producers and the Arkus composer.
    /// This surface is deliberately semantic and contains no Runtime, transport, or engine dependency.
    /// </summary>
    public static class CanonicalIdentityRules
    {
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
    }
}
