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

            // The authoritative session still owns the complete HK06A journal artifact. Version 1 is
            // bound directly to that accepted complete read. HK08A version 2 is an explicitly negotiated
            // bounded view with deterministic cursor integrity; when one v2 page covers the whole journal,
            // it may still return the exact HK06A artifact so replay/audit meaning remains recognizable.
            var complete = (IWorldProvenanceService)new WorldMutationPlannerView(service);
            var bounded = (IWorldProvenanceService)new BoundedWorldProvenanceService(complete);
            var integrityBounded = (IWorldProvenanceService)new IntegrityBoundWorldProvenanceService(bounded);
            var pagedRead = (IWorldProvenanceService)new CompleteJournalPreservingProvenanceService(integrityBounded);
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldProvenanceReadHandler(complete)),
                CapabilityRoute.FromHandler(new WorldProvenancePagedReadHandler(pagedRead))
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

    [PublicCapabilityRoute("arkus.base", WorldProvenanceContract.ReadName, WorldProvenanceContract.PagedContractVersionText)]
    internal sealed class WorldProvenancePagedReadHandler : ICanonicalCapabilityHandler
    {
        private readonly IWorldProvenanceService _service;

        public WorldProvenancePagedReadHandler(IWorldProvenanceService service)
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
