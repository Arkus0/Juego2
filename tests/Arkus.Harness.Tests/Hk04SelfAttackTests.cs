using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk04SelfAttackTests
    {
        [Fact]
        public void PartialApplyAfterLaterFailureMutantCannotChangeCommittedHash()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var before = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "attack.partial-apply",
                Hk04TransactionalMutationTests.PutObject("node.should-not-persist", "fixture.item"),
                Hk04TransactionalMutationTests.RemoveObject("node.root"));

            var result = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                request);

            Assert.False(result.Success);
            Assert.Equal("world.change.invalid_candidate", result.Error!.MachineCode);
            Assert.Equal(before, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.DoesNotContain(session.Current.Objects, value => value.Id.Value == "node.should-not-persist");
        }

        [Fact]
        public void StaleRevisionOverwriteMutantFailsBeforeReplacingAcceptedState()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var accepted = Hk04TransactionalMutationTests.Request(
                initial,
                "attack.stale.accepted",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.accepted"));
            var stale = Hk04TransactionalMutationTests.Request(
                initial,
                "attack.stale.writer",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.stale"));

            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, accepted);
            var acceptedHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var result = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                stale);

            Assert.False(result.Success);
            Assert.Equal("world.change.stale_revision", result.Error!.MachineCode);
            Assert.Equal(acceptedHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
        }

        [Fact]
        public void DuplicateRetryMutantCannotAdvanceRevisionTwice()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "attack.duplicate-retry",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.retry"));

            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, request);
            var once = session.Current.Revision;
            var retry = Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, request);

            Assert.True((bool)retry.Data!["replayed"]!);
            Assert.Equal(once, session.Current.Revision);
            Assert.Equal(initial.Revision + 1, once);
        }

        [Fact]
        public void HiddenMutationBypassMutantBreaksMutationSurfaceEquality()
        {
            var source = Hk01TestFixtures.FixtureDefinition(new ContractVersion(1, 0), true);
            var mutantDefinition = new CapabilityDefinition(
                source.Key,
                source.Provider,
                source.RequestSchema,
                source.SuccessSchema,
                source.ErrorSchema,
                SideEffectClass.CanonicalMutation,
                source.Determinism,
                new[] { "expected-version" },
                new[] { "mutation-committed" },
                new ConcurrencySemantics(ConcurrencyClass.OptimisticVersioned, "expectedRevision"),
                new IdempotencySemantics(IdempotencyClass.IdempotentWithKey, "idempotencyKey"),
                source.Batching,
                source.Repair,
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.CanonicalTransaction,
                    ProvenanceRequirement.Required),
                source.Cost);
            var mutantProvider = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.engine", ProviderKind.Scoped, "engine", new[] { "engine" }),
                new[] { mutantDefinition },
                new[] { CapabilityRoute.FromHandler(new FixtureEngineHandler()) });
            var composition = ContractComposer.Compose(BaseContract.CreateContribution(), new[] { mutantProvider });
            Assert.True(composition.Success);

            var report = MutationSurfaceConformance.Evaluate(
                composition.Contract!,
                new[] { typeof(FixtureEngineHandler).Assembly });

            Assert.False(report.IsConformant);
            Assert.Contains(report.Issues, issue =>
                issue.Code == "mutation-surface.omission" &&
                issue.Subject == "engine.observe@1.0");
        }

        [Fact]
        public void DryRunDivergenceMutantIsDetectedByPredictedCanonicalHash()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "attack.dry-run-divergence",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.planned"));
            var dry = Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.DryRunName, request);
            var predicted = (string)Hk04TransactionalMutationTests.Map(
                Hk04TransactionalMutationTests.Map(dry.Data!, "plan"),
                "result")["hash"]!;

            var divergent = ReplacePeer(initial, "fixture.mutant", initial.Revision + 1);
            Assert.NotEqual(predicted, CanonicalWorldStateCodec.ComputeContentHash(divergent));

            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, request);
            Assert.Equal(predicted, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
        }

        [Fact]
        public void OmittedChangeEffectMutantTurnsIndependentCoverageOracleRed()
        {
            var before = Hk02TestFixtures.MicroWorld();
            var after = ReplacePeer(before, "fixture.changed", before.Revision + 1);

            var omitted = WorldMutationCoverage.FindMismatches(
                before,
                after,
                Array.Empty<WorldMutationChange>());

            Assert.NotEmpty(omitted);
            Assert.Contains(omitted, value => value.Contains("world.object:node.peer|field|typeId", StringComparison.Ordinal));

            var declared = new[]
            {
                new WorldMutationChange(
                    "world.object:node.peer",
                    "update",
                    new[] { "typeId" },
                    Array.Empty<string>(),
                    Array.Empty<string>())
            };
            Assert.Empty(WorldMutationCoverage.FindMismatches(before, after, declared));
        }

        private static WorldState ReplacePeer(WorldState source, string typeId, long revision)
        {
            var objects = new List<WorldObject>();
            for (var index = 0; index < source.Objects.Count; index++)
            {
                var current = source.Objects[index];
                objects.Add(current.Id.Value == "node.peer"
                    ? new WorldObject(current.Id, new WorldTypeId(typeId), current.ContainerId, current.References)
                    : current);
            }

            return new WorldState(
                source.Id,
                revision,
                objects,
                source.Extensions,
                source.SchemaVersion);
        }
    }
}
