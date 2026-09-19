using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// Canonical H0 composition once HK03 world inspection is present. It combines the
    /// accepted base contract and HK03 definitions before invoking the single HK01 composer.
    /// </summary>
    public static class CanonicalWorldContract
    {
        public static CanonicalProviderContribution CreateContribution(IWorldInspectionService inspection)
        {
            if (inspection == null)
            {
                throw new ArgumentNullException(nameof(inspection));
            }

            var acceptedBase = BaseContract.CreateContribution();
            var definitions = new List<CapabilityDefinition>(acceptedBase.Definitions);
            definitions.AddRange(WorldInspectionContract.CreateDefinitions());

            var routes = new List<CapabilityRoute>(acceptedBase.Routes);
            routes.AddRange(WorldInspectionBindings.CreateRoutes(inspection));

            return new CanonicalProviderContribution(
                new ProviderDescriptor(
                    "arkus.base",
                    ProviderKind.Base,
                    "base",
                    new[] { "system", "world" }),
                definitions,
                routes);
        }

        public static ComposedContract Compose(IWorldInspectionService inspection)
        {
            var result = ContractComposer.Compose(CreateContribution(inspection));
            if (!result.Success || result.Contract == null)
            {
                throw new InvalidOperationException("The canonical HK03 world contract must compose successfully.");
            }

            return result.Contract;
        }
    }
}
