using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.Harness.Projection;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    public static class H1ProjectionReconciliationComposition
    {
        public static CanonicalProviderContribution CreateEditorHostContribution(H1UnityEditorExecutionCoordinator coordinator)
        {
            if (coordinator == null) throw new ArgumentNullException(nameof(coordinator));
            var existing = H1UnityLifecycleContract.CreateEditorHostContribution(coordinator, includeCatalogue: true, includeProjection: true);
            return new CanonicalProviderContribution(
                existing.Descriptor,
                existing.Definitions.Concat(H1ProjectionReconciliationContract.Definitions()).ToArray(),
                existing.Routes.Concat(H1ProjectionReconciliationContract.Routes(coordinator)).ToArray());
        }

        public static IReadOnlyList<UnityHostCapabilityGrant> CreateGrants()
        {
            return H1UnityLifecycleContract.CreateGrants(includeCatalogue: true, includeProjection: true)
                .Concat(H1ProjectionReconciliationContract.Grants())
                .ToArray();
        }
    }
}
