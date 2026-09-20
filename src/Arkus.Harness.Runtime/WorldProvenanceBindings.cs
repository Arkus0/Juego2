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

            // The authoritative session still owns the complete HK06A journal artifact. Public HK08A
            // reads receive the accepted read-only attenuation facade plus a bounded deterministic
            // paging view; neither layer can acquire canonical commit authority.
            var complete = (IWorldProvenanceService)new WorldMutationPlannerView(service);
            var bounded = (IWorldProvenanceService)new BoundedWorldProvenanceService(complete);
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldProvenanceReadHandler(bounded))
            }.AsReadOnly();
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldProvenanceContract.ReadName, WorldProvenanceContract.ContractVersionText)]
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
