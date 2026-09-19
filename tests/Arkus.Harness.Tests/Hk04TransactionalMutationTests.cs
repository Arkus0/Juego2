using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk04TransactionalMutationTests
    {
        [Fact]
        public void PlanDryRunAndApplyShareOneDeterministicCandidateWhileDryRunDoesNotPersist()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            var request = Request(initial, "request.shared-path", PutObject("node.peer", "fixture.updated"));
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

            var plan = Success(contract, WorldMutationContract.PlanName, request);
            var dryRun = Success(contract, WorldMutationContract.DryRunName, request);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.False((bool)plan.Data!["persisted"]!);
            Assert.False((bool)dryRun.Data!["persisted"]!);

            var planned = Map(plan.Data, "plan");
            var dryPlanned = Map(dryRun.Data, "plan");
            Assert.Equal(planned["planId"], dryPlanned["planId"]);
            Assert.Equal(Map(planned, "result")["hash"], Map(dryPlanned, "result")["hash"]);

            var applied = Success(contract, WorldMutationContract.ApplyName, request);
            var appliedPlan = Map(applied.Data!, "plan");
            var predicted = (string)Map(planned, "result")["hash"]!;
            Assert.Equal(planned["planId"], appliedPlan["planId"]);
            Assert.Equal(predicted, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(initial.Revision + 1, session.Current.Revision);
            Assert.True((bool)applied.Data!["persisted"]!);
            Assert.False((bool)applied.Data["replayed"]!);

            var summary = Success(contract, WorldInspectionContract.SummaryName, Empty());
            var world = Map(summary.Data!, "world");
            Assert.Equal(session.Current.Revision, world["revision"]);
            Assert.Equal(predicted, world["hash"]);
        }

        [Fact]
        public void FailedLaterOperationLeavesWholeStateUnchanged()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            var request = Request(
                initial,
                "request.atomic-failure",
                PutObject("node.extra", "fixture.item"),
                RemoveObject("node.root"));
            var before = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

            var result = contract.Dispatch(WorldMutationContract.ApplyName, ExactVersion(), request);

            Assert.False(result.Success);
            Assert.Equal("world.change.invalid_candidate", result.Error!.MachineCode);
            Assert.Equal(before, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(initial.Revision, session.Current.Revision);
        }

        [Fact]
        public void StaleWriterCannotOverwriteAcceptedConcurrentState()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            var first = Request(initial, "request.writer-a", PutObject("node.peer", "fixture.writer-a"));
            var stale = Request(initial, "request.writer-b", PutObject("node.peer", "fixture.writer-b"));

            Success(contract, WorldMutationContract.ApplyName, first);
            var acceptedHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var staleResult = contract.Dispatch(WorldMutationContract.ApplyName, ExactVersion(), stale);

            Assert.False(staleResult.Success);
            Assert.Equal("world.change.stale_revision", staleResult.Error!.MachineCode);
            Assert.Equal(acceptedHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
        }

        [Fact]
        public void ExactRetryReplaysReceiptAndKeyReuseForDifferentRequestFailsClosed()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            var request = Request(initial, "request.retry", PutObject("node.peer", "fixture.retry"));

            var first = Success(contract, WorldMutationContract.ApplyName, request);
            var acceptedHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var acceptedRevision = session.Current.Revision;
            var retry = Success(contract, WorldMutationContract.ApplyName, request);

            Assert.False((bool)first.Data!["replayed"]!);
            Assert.True((bool)retry.Data!["replayed"]!);
            Assert.Equal(acceptedRevision, session.Current.Revision);
            Assert.Equal(acceptedHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Equal(Map(first.Data, "plan")["planId"], Map(retry.Data, "plan")["planId"]);

            var conflicting = Request(initial, "request.retry", PutObject("node.peer", "fixture.other"));
            var conflict = contract.Dispatch(WorldMutationContract.ApplyName, ExactVersion(), conflicting);
            Assert.False(conflict.Success);
            Assert.Equal("world.change.idempotency_conflict", conflict.Error!.MachineCode);
            Assert.Equal(acceptedRevision, session.Current.Revision);
        }

        [Fact]
        public void PlanNamesAffectedResourceFieldsAndReferenceEdges()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            var replacement = PutObject(
                "node.child",
                "fixture.item",
                "node.root",
                Reference("fixture.root", "node.root"));
            var planResult = Success(
                contract,
                WorldMutationContract.PlanName,
                Request(initial, "request.change-set", replacement));

            var plan = Map(planResult.Data!, "plan");
            var changes = List(plan, "changes");
            Assert.Single(changes);
            var change = (IReadOnlyDictionary<string, object?>)changes[0]!;
            Assert.Equal("world.object:node.child", change["resource"]);
            Assert.Equal("update", change["action"]);
            Assert.Contains("references", Strings(change, "fields"));
            Assert.Contains("fixture.peer->node.peer", Strings(change, "referencesRemoved"));
            Assert.Empty(Strings(change, "referencesAdded"));
            Assert.All(List(plan, "preconditions"), value => Assert.True((bool)((IReadOnlyDictionary<string, object?>)value!)["satisfied"]!));
            Assert.All(List(plan, "postconditions"), value => Assert.True((bool)((IReadOnlyDictionary<string, object?>)value!)["satisfied"]!));
        }

        [Fact]
        public void EmptyMicroWorldCanBeCreatedAndThenInspectedThroughSameCanonicalState()
        {
            var initial = new WorldState(new WorldId("world.empty"), 0, Array.Empty<WorldObject>());
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Compose(session);
            var request = Request(
                initial,
                "request.create-micro-world",
                PutObject("node.root", "fixture.container"),
                PutObject(
                    "node.child",
                    "fixture.item",
                    "node.root",
                    Reference("fixture.root", "node.root")));

            Success(contract, WorldMutationContract.ApplyName, request);

            Assert.Equal(2, session.Current.Objects.Count);
            var summary = Success(contract, WorldInspectionContract.SummaryName, Empty());
            Assert.Equal(2, summary.Data!["objectCount"]);
            Assert.Equal(1, summary.Data["referenceCount"]);
            Assert.Equal(session.Current.Revision, Map(summary.Data, "world")["revision"]);
        }

        [Fact]
        public void DiscoveryDispatcherAndTransactionalMutationSurfacesAreConformant()
        {
            var session = new TransactionalWorldAuthoringSession(Hk02TestFixtures.MicroWorld());
            var contract = Compose(session);
            var routeUniverse = RouteUniverse.Enumerate(typeof(SystemDescribeHandler).Assembly);
            var canonical = CanonicalContractConformance.Evaluate(contract, routeUniverse);
            Assert.True(canonical.IsConformant, Format(canonical.Issues));

            var mutation = MutationSurfaceConformance.Evaluate(
                contract,
                new[] { typeof(SystemDescribeHandler).Assembly });
            Assert.True(mutation.IsConformant, FormatMutation(mutation.Issues));

            var mutationNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (var definition in contract.Definitions)
            {
                if (definition.Key.Name.StartsWith("world.change.", StringComparison.Ordinal))
                {
                    mutationNames.Add(definition.Key.Name);
                    Assert.NotNull(definition.RequestSchema);
                    Assert.NotNull(definition.SuccessSchema);
                    Assert.NotNull(definition.ErrorSchema);
                }
            }

            Assert.True(new HashSet<string>(StringComparer.Ordinal)
            {
                WorldMutationContract.PlanName,
                WorldMutationContract.DryRunName,
                WorldMutationContract.ApplyName
            }.SetEquals(mutationNames));
        }

        internal static ComposedContract Compose(TransactionalWorldAuthoringSession session)
        {
            return CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
        }

        internal static Dictionary<string, object?> Request(
            WorldState state,
            string idempotencyKey,
            params IReadOnlyDictionary<string, object?>[] operations)
        {
            var values = new List<object?>();
            foreach (var operation in operations) values.Add(operation);
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = idempotencyKey,
                ["expectedRevision"] = state.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(state),
                ["operations"] = values.AsReadOnly()
            };
        }

        internal static IReadOnlyDictionary<string, object?> PutObject(
            string id,
            string typeId,
            string? containerId = null,
            params IReadOnlyDictionary<string, object?>[] references)
        {
            var values = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-object",
                ["id"] = id,
                ["typeId"] = typeId
            };
            if (containerId != null) values["containerId"] = containerId;
            if (references.Length != 0)
            {
                var list = new List<object?>();
                foreach (var reference in references) list.Add(reference);
                values["references"] = list.AsReadOnly();
            }

            return Hk03InspectionTests.ReadOnlyMap(values);
        }

        internal static IReadOnlyDictionary<string, object?> RemoveObject(string id)
        {
            return Hk03InspectionTests.ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "remove-object",
                ["id"] = id
            });
        }

        internal static IReadOnlyDictionary<string, object?> Reference(string kind, string targetId)
        {
            return Hk03InspectionTests.ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = kind,
                ["targetId"] = targetId
            });
        }

        internal static CapabilityInvocationResult Success(
            ComposedContract contract,
            string capability,
            IReadOnlyDictionary<string, object?> request)
        {
            var result = contract.Dispatch(capability, ExactVersion(), request);
            Assert.True(result.Success, result.Error == null ? "unknown failure" : result.Error.MachineCode + ":" + result.Error.Message);
            Assert.NotNull(result.Data);
            return result;
        }

        internal static IReadOnlyDictionary<string, object?> Map(IReadOnlyDictionary<string, object?> data, string key)
        {
            return (IReadOnlyDictionary<string, object?>)data[key]!;
        }

        internal static IReadOnlyList<object?> List(IReadOnlyDictionary<string, object?> data, string key)
        {
            return (IReadOnlyList<object?>)data[key]!;
        }

        internal static IReadOnlyList<string> Strings(IReadOnlyDictionary<string, object?> data, string key)
        {
            var values = new List<string>();
            foreach (var item in List(data, key)) values.Add((string)item!);
            return values.AsReadOnly();
        }

        internal static ContractVersionRange ExactVersion()
        {
            return ContractVersionRange.Exact(new ContractVersion(1, 0));
        }

        private static Dictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }

        private static string Format(IReadOnlyList<ConformanceIssue> issues)
        {
            var value = string.Empty;
            foreach (var issue in issues) value += issue.Code + ":" + issue.Subject + ";";
            return value;
        }

        private static string FormatMutation(IReadOnlyList<MutationSurfaceIssue> issues)
        {
            var value = string.Empty;
            foreach (var issue in issues) value += issue.Code + ":" + issue.Subject + ";";
            return value;
        }
    }
}
