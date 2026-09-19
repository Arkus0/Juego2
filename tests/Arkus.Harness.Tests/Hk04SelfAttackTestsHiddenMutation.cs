using System;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Repair-cycle causal control for the independent Reviewer finding on candidate
    /// 28695c9d20d018d96f3afdacb402c0d0ec02288e. Unlike the original metadata-only mutant,
    /// this handler really commits the authoritative WorldState while remaining declared/read-only
    /// and outside ITransactionalMutationHandler.
    /// </summary>
    public sealed class Hk04SelfAttackTestsHiddenMutation
    {
        [Fact]
        public void RealReadOnlyHandlerMutationIsBlockedAndTurnsIndependentAuthorityOracleRed()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hidden-bypass-real",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.hidden-mutator"));
            var hidden = new HiddenCanonicalMutationBypassHandler(session, request);
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

            // Causal premise: this is not a metadata fiction. Invoking the handler directly really
            // changes the authoritative canonical state through a forbidden read-only command path.
            var mutation = hidden.Invoke(null!, Hk01TestFixtures.EmptyRequest());
            Assert.True(mutation.Success);
            Assert.NotEqual(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(initial.Revision + 1, session.Current.Revision);

            // Runtime publication fails closed even though the route metadata itself still says
            // engine.observe/read-only in the fixture contract.
            var freshInitial = Hk02TestFixtures.MicroWorld();
            var freshSession = new TransactionalWorldAuthoringSession(freshInitial);
            var freshHidden = new HiddenCanonicalMutationBypassHandler(
                freshSession,
                Hk04TransactionalMutationTests.Request(
                    freshInitial,
                    "request.hidden-bypass-publication",
                    Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.hidden-mutator")));
            Assert.Throws<ArgumentException>(() => CapabilityRoute.FromHandler(freshHidden));

            // Independent proof universe: keep the canonical definition ReadOnly and independently
            // enumerate executable handler authority from the test assembly. SideEffect is never used
            // to decide that HiddenCanonicalMutationBypassHandler can write.
            var contract = Hk01TestFixtures.ComposeWithFixture();
            var report = MutationSurfaceConformance.Evaluate(
                contract,
                new[] { typeof(HiddenCanonicalMutationBypassHandler).Assembly });

            Assert.False(report.IsConformant);
            Assert.Contains(report.Issues, issue =>
                issue.Code == "mutation-surface.extra" &&
                issue.Subject == "engine.observe@1.0" &&
                issue.Message.Contains("effective-write-authority", StringComparison.Ordinal));
        }
    }

    [PublicCapabilityRoute("fixture.engine", "engine.observe", "1.0")]
    internal sealed class HiddenCanonicalMutationBypassHandler : ICanonicalCapabilityHandler
    {
        private readonly TransactionalWorldAuthoringSession _session;
        private readonly System.Collections.Generic.IReadOnlyDictionary<string, object?> _mutationRequest;

        public HiddenCanonicalMutationBypassHandler(
            TransactionalWorldAuthoringSession session,
            System.Collections.Generic.IReadOnlyDictionary<string, object?> mutationRequest)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _mutationRequest = mutationRequest ?? throw new ArgumentNullException(nameof(mutationRequest));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            System.Collections.Generic.IReadOnlyDictionary<string, object?> request)
        {
            return _session.Apply(_mutationRequest);
        }
    }
}
