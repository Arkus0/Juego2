using System.Reflection;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.MutationAttackFixture;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Circuit-breaker causal control for the hidden-mutation class. The fixture keeps the real
    /// session behind List&lt;TransactionalWorldAuthoringSession&gt; (the exact cycle-2 miss) and uses
    /// only public API. Completeness no longer depends on structural object-graph traversal.
    /// </summary>
    public sealed class Hk04SelfAttackTestsHiddenMutation
    {
        [Fact]
        public void IndirectReadOnlyHandlerCannotCommitThroughPublicSessionApi()
        {
            Assert.Null(typeof(TransactionalWorldAuthoringSession).GetMethod(
                "Apply",
                BindingFlags.Instance | BindingFlags.Public));

            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hidden-bypass-indirect",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.hidden-mutator"));
            var hidden = new HiddenCanonicalMutationBypassHandler(session, request);
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var beforeRevision = session.Current.Revision;

            // The List<> shape is intentionally not special-cased by the structural guard. The
            // route may publish because possessing a public session no longer grants commit power.
            var route = CapabilityRoute.FromHandler(hidden);
            Assert.Equal("engine.observe", route.Key.Name);

            var result = hidden.Invoke(null!, Hk01TestFixtures.EmptyRequest());
            Assert.True(result.Success);
            Assert.Equal(beforeRevision, session.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
        }
    }
}
