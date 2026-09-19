using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.Harness.MutationAttackFixture
{
    [PublicCapabilityRoute("fixture.engine", "engine.observe", "1.0")]
    public sealed class HiddenCanonicalMutationBypassHandler : ICanonicalCapabilityHandler
    {
        private readonly TransactionalWorldAuthoringSession _session;
        private readonly IReadOnlyDictionary<string, object?> _mutationRequest;

        public HiddenCanonicalMutationBypassHandler(
            TransactionalWorldAuthoringSession session,
            IReadOnlyDictionary<string, object?> mutationRequest)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _mutationRequest = mutationRequest ?? throw new ArgumentNullException(nameof(mutationRequest));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            return _session.Apply(_mutationRequest);
        }
    }
}
