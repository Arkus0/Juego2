using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>Marker for the separately classified HK06C canonical replay orchestration path.</summary>
    internal interface ICanonicalReplayHandler : ICapabilityHandler
    {
    }

    internal sealed class WorldReplayCompatibilityHandler : ICapabilityHandler
    {
        private readonly IWorldReplayService _service;

        public WorldReplayCompatibilityHandler(IWorldReplayService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public CapabilityInvocationResult Invoke(IReadOnlyDictionary<string, object?> request)
        {
            return _service.CheckReplayCompatibility(request);
        }
    }

    internal sealed class WorldReplayHandler : ICanonicalReplayHandler
    {
        private readonly ICanonicalWorldReplayExecutor _executor;

        public WorldReplayHandler(ICanonicalWorldReplayExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public CapabilityInvocationResult Invoke(IReadOnlyDictionary<string, object?> request)
        {
            return _executor.Replay(request);
        }
    }

    internal static class WorldReplayBindings
    {
        public static IReadOnlyList<CapabilityRoute> CreateRoutes(IWorldMutationService mutation)
        {
            if (mutation == null) throw new ArgumentNullException(nameof(mutation));
            return new List<CapabilityRoute>
            {
                new CapabilityRoute(
                    new CapabilityKey(WorldReplayContract.CompatibilityName, ContractVersion.Parse(WorldReplayContract.ContractVersionText)),
                    new WorldReplayCompatibilityHandler(CanonicalWorldReplayAuthority.BindCompatibility(mutation))),
                new CapabilityRoute(
                    new CapabilityKey(WorldReplayContract.ReplayName, ContractVersion.Parse(WorldReplayContract.ContractVersionText)),
                    new WorldReplayHandler(CanonicalWorldReplayAuthority.Bind(mutation)))
            }.AsReadOnly();
        }
    }

    /// <summary>
    /// HK06C conformance guard: replay is neither an ordinary HK06A canonical mutation nor an HK06B
    /// rebase. Every publicly declared CanonicalReplay route must use the dedicated handler marker
    /// and matching CanonicalReplay transaction requirement, and no other route may carry it.
    /// </summary>
    internal static class ReplaySurfaceConformance
    {
        public static IReadOnlyList<string> Validate(
            IEnumerable<CapabilityDefinition> definitions,
            IEnumerable<CapabilityRoute> routes)
        {
            if (definitions == null) throw new ArgumentNullException(nameof(definitions));
            if (routes == null) throw new ArgumentNullException(nameof(routes));

            var declared = new HashSet<CapabilityKey>();
            foreach (var definition in definitions)
            {
                if (definition.SideEffect != SideEffectClass.CanonicalReplay) continue;
                declared.Add(definition.Key);
                if (definition.Policy == null ||
                    definition.Policy.TransactionRequirement != TransactionRequirement.CanonicalReplay ||
                    definition.Policy.ProvenanceRequirement != ProvenanceRequirement.Required)
                {
                    return new[] { "CanonicalReplay definition has mismatched transaction/provenance policy: " + definition.Key };
                }
            }

            var bound = new HashSet<CapabilityKey>();
            foreach (var route in routes)
            {
                if (!(route.Handler is ICanonicalReplayHandler)) continue;
                bound.Add(route.Key);
                if (route.Handler is ITransactionalMutationHandler || route.Handler is ICanonicalRebaseHandler)
                    return new[] { "CanonicalReplay handler aliases an ordinary mutation or rebase authority: " + route.Key };
            }

            if (declared.SetEquals(bound)) return Array.Empty<string>();
            return new[] { "CanonicalReplay public definitions and dedicated replay handlers differ." };
        }
    }
}
