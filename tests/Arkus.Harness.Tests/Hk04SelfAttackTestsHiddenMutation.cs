using System;
using System.Collections.Generic;
using System.Reflection;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.MutationAttackFixture;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Circuit-breaker causal controls for the hidden-mutation class. The first control preserves
    /// the exact List<> indirection missed by repair cycle 1. The second evaluates every current
    /// public non-mutation route against one live authoritative session and proves revision/hash
    /// remain unchanged, so friend-assembly implementation details cannot silently become writes.
    /// </summary>
    public sealed class Hk04SelfAttackTestsHiddenMutation
    {
        [Fact]
        public void IndirectReadOnlyHandlerCannotCommitThroughPublicSessionApi()
        {
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

            // The fixture invokes a public Apply if one exists. Therefore a regression that makes
            // commit public performs the real state change first and these effective assertions turn RED.
            var result = hidden.Invoke(null!, Hk01TestFixtures.EmptyRequest());
            Assert.True(result.Success);
            Assert.Equal(beforeRevision, session.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Null(typeof(TransactionalWorldAuthoringSession).GetMethod(
                "Apply",
                BindingFlags.Instance | BindingFlags.Public));
        }

        [Fact]
        public void EveryEffectiveNonMutationRouteIsEvaluatedAndCannotChangeCanonicalState()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var requests = ValidNonMutationRequests(initial);
            var nonMutationNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (var definition in contract.Definitions)
            {
                if (definition.SideEffect == SideEffectClass.CanonicalMutation)
                {
                    continue;
                }

                nonMutationNames.Add(definition.Key.Name);
                Assert.True(
                    requests.TryGetValue(definition.Key.Name, out var request),
                    "Missing evaluated request vector for public non-mutation route " + definition.Key);

                var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
                var beforeRevision = session.Current.Revision;
                var result = contract.Dispatch(
                    definition.Key.Name,
                    ContractVersionRange.Exact(definition.Key.Version),
                    request!);

                Assert.True(
                    result.Success,
                    definition.Key + " did not reach a successful effective execution: " +
                    (result.Error == null ? "unknown" : result.Error.MachineCode + ":" + result.Error.Message));
                Assert.Equal(beforeRevision, session.Current.Revision);
                Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            }

            Assert.Equal(nonMutationNames.Count, requests.Count);
            Assert.True(nonMutationNames.SetEquals(requests.Keys));
        }

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, object?>> ValidNonMutationRequests(WorldState state)
        {
            var objectGet = Hk03InspectionTests.Anchor(state);
            objectGet["id"] = "node.child";

            var extensionRead = Hk03InspectionTests.Anchor(state);
            extensionRead["owner"] = "future.alpha";
            extensionRead["schemaVersion"] = 2;

            var plannedMutation = Hk04TransactionalMutationTests.Request(
                state,
                "request.nonmutation-oracle",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.oracle"));

            return new Dictionary<string, IReadOnlyDictionary<string, object?>>(StringComparer.Ordinal)
            {
                ["system.describe"] = Hk01TestFixtures.EmptyRequest(),
                [WorldInspectionContract.SummaryName] = Hk01TestFixtures.EmptyRequest(),
                [WorldInspectionContract.ObjectGetName] = objectGet,
                [WorldInspectionContract.ObjectQueryName] = Hk03InspectionTests.Anchor(state),
                [WorldInspectionContract.ReferenceQueryName] = Hk03InspectionTests.Anchor(state),
                [WorldInspectionContract.ExtensionQueryName] = Hk03InspectionTests.Anchor(state),
                [WorldInspectionContract.ExtensionReadName] = extensionRead,
                [WorldMutationContract.PlanName] = plannedMutation,
                [WorldMutationContract.DryRunName] = plannedMutation
            };
        }
    }
}
