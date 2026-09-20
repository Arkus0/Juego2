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
            // reads receive the accepted read-only attenuation facade, a bounded deterministic paging
            // view and a deterministic integrity envelope over each continuation cursor. If one bounded
            // page covers the whole journal, the final compatibility view restores the exact HK06A
            // complete artifact so replay/audit meaning stays unchanged.
            var complete = (IWorldProvenanceService)new WorldMutationPlannerView(service);
            var bounded = (IWorldProvenanceService)new BoundedWorldProvenanceService(complete);
            var integrityBounded = (IWorldProvenanceService)new IntegrityBoundWorldProvenanceService(bounded);
            var publicRead = (IWorldProvenanceService)new CompleteJournalPreservingProvenanceService(integrityBounded);
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldProvenanceReadHandler(publicRead))
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
