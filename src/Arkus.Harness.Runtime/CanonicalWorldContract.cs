using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// Canonical H0 world composition. Accepted read, validation and mutation capabilities enter one
    /// HK01 composer and one discovery/dispatcher inventory; unavailable services keep the inventory
    /// complete even when an embedding runtime has not bound authorable state.
    /// </summary>
    public static class CanonicalWorldContract
    {
        public static CanonicalProviderContribution CreateContribution(IWorldInspectionService inspection)
        {
            return CreateContribution(inspection, new UnavailableWorldMutationService());
        }

        public static CanonicalProviderContribution CreateContribution(
            IWorldInspectionService inspection,
            IWorldMutationService mutation)
        {
            if (inspection == null) throw new ArgumentNullException(nameof(inspection));
            if (mutation == null) throw new ArgumentNullException(nameof(mutation));

            var acceptedBase = BaseContract.CreateContribution();
            var definitions = new List<CapabilityDefinition>(acceptedBase.Definitions);
            definitions.AddRange(WorldInspectionContract.CreateDefinitions());
            definitions.AddRange(WorldValidationContract.CreateDefinitions());
            definitions.AddRange(WorldMutationContract.CreateDefinitions());
            definitions.AddRange(WorldProvenanceContract.CreateDefinitions());
            definitions.AddRange(WorldPortabilityContract.CreateDefinitions());
            definitions.AddRange(WorldReplayContract.CreateDefinitions());

            var routes = new List<CapabilityRoute>(acceptedBase.Routes);
            routes.AddRange(WorldInspectionBindings.CreateRoutes(inspection));
            routes.AddRange(WorldValidationBindings.CreateRoutes(mutation));
            routes.AddRange(WorldMutationBindings.CreateRoutes(mutation));
            routes.AddRange(WorldProvenanceBindings.CreateRoutes(mutation));
            routes.AddRange(WorldPortabilityBindings.CreateRoutes(mutation));
            routes.AddRange(WorldReplayBindings.CreateRoutes(mutation));

            var replayIssues = ReplaySurfaceConformance.Validate(definitions, routes);
            if (replayIssues.Count != 0)
                throw new InvalidOperationException(replayIssues[0]);

            return new CanonicalProviderContribution(
                new ProviderDescriptor(
                    "arkus.base",
                    ProviderKind.Base,
                    "base",
                    new[] { "system", "world", "authoring" }),
                definitions,
                routes);
        }

        public static ComposedContract Compose(IWorldInspectionService inspection)
        {
            return Compose(inspection, new UnavailableWorldMutationService());
        }

        public static ComposedContract Compose(
            IWorldInspectionService inspection,
            IWorldMutationService mutation)
        {
            var result = ContractComposer.Compose(CreateContribution(inspection, mutation));
            if (!result.Success || result.Contract == null)
            {
                throw new InvalidOperationException("The canonical world contract must compose successfully.");
            }

            return result.Contract;
        }

        /// <summary>
        /// Compose the complete canonical contract over one empty portable session. Keeping this
        /// pairing inside Runtime prevents hosts from accidentally binding reads and writes to
        /// different aggregates while preserving the existing interface-based embedding overloads.
        /// </summary>
        public static ComposedContract ComposeEmptyPortableSession(string worldId)
        {
            if (worldId == null) throw new ArgumentNullException(nameof(worldId));
            var initial = new WorldState(new WorldId(worldId), 0, Array.Empty<WorldObject>());
            var session = new PortableWorldAuthoringSession(initial);
            return Compose(new WorldInspectionService(session), session);
        }
    }
}
