using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    internal static class WorldProvenanceBindings
    {
        public static IReadOnlyList<CapabilityRoute> CreateRoutes(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            // The authoritative session also owns internal commit authority. Journal reads receive
            // the accepted HK04 attenuation facade so observation cannot acquire that authority.
            var provenance = (IWorldProvenanceService)new WorldMutationPlannerView(service);
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldProvenanceReadHandler(provenance))
            }.AsReadOnly();
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldProvenanceContract.ReadName, "1.0")]
    internal sealed class WorldProvenanceReadHandler : ICanonicalCapabilityHandler
    {
        private readonly IWorldProvenanceService _service;

        public WorldProvenanceReadHandler(IWorldProvenanceService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _service.ReadJournal(request);
        }
    }
}
