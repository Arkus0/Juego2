using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// Authoring-assembly adapter used by the runtime route binding. It intentionally does not
    /// implement ordinary canonical mutation authority: the public replay handler therefore carries
    /// only the dedicated replay surface while replay itself still delegates every staged step to
    /// the accepted HK04/HK06A canonical committer inside PortableWorldAuthoringSession.
    /// </summary>
    internal sealed class WorldReplayBindingAuthority : ICanonicalWorldReplayExecutor, IWorldReplayService
    {
        private readonly ICanonicalWorldReplayExecutor _executor;
        private readonly IWorldReplayService _compatibility;

        public WorldReplayBindingAuthority(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            _executor = CanonicalWorldReplayAuthority.Bind(service);
            _compatibility = CanonicalWorldReplayAuthority.BindCompatibility(service);
        }

        public CapabilityInvocationResult Replay(
            IReadOnlyDictionary<string, object?> request,
            InvocationResourceBudget resourceBudget)
        {
            return _executor.Replay(
                request ?? throw new ArgumentNullException(nameof(request)),
                resourceBudget ?? throw new ArgumentNullException(nameof(resourceBudget)));
        }

        public CapabilityInvocationResult CheckReplayCompatibility(IReadOnlyDictionary<string, object?> request)
        {
            return _compatibility.CheckReplayCompatibility(request ?? throw new ArgumentNullException(nameof(request)));
        }
    }
}
