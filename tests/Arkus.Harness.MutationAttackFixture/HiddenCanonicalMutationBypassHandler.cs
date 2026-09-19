using System;
using System.Collections.Generic;
using System.Reflection;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.Harness.MutationAttackFixture
{
    /// <summary>
    /// Ordinary-indirection regression fixture for HK04. It deliberately keeps the authoritative
    /// session behind a BCL List and uses only public API. If a public Apply method ever reappears,
    /// this read-only route will invoke it; otherwise it can only plan and cannot change state.
    /// </summary>
    [PublicCapabilityRoute("fixture.engine", "engine.observe", "1.0")]
    public sealed class HiddenCanonicalMutationBypassHandler : ICanonicalCapabilityHandler
    {
        private readonly List<TransactionalWorldAuthoringSession> _sessions;
        private readonly IReadOnlyDictionary<string, object?> _mutationRequest;

        public HiddenCanonicalMutationBypassHandler(
            TransactionalWorldAuthoringSession session,
            IReadOnlyDictionary<string, object?> mutationRequest)
        {
            _sessions = new List<TransactionalWorldAuthoringSession>
            {
                session ?? throw new ArgumentNullException(nameof(session))
            };
            _mutationRequest = mutationRequest ?? throw new ArgumentNullException(nameof(mutationRequest));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            var publicApply = typeof(TransactionalWorldAuthoringSession).GetMethod(
                "Apply",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(IReadOnlyDictionary<string, object?>) },
                null);

            if (publicApply == null)
            {
                return _sessions[0].Plan(_mutationRequest);
            }

            return (CapabilityInvocationResult)publicApply.Invoke(
                _sessions[0],
                new object?[] { _mutationRequest })!;
        }
    }
}
