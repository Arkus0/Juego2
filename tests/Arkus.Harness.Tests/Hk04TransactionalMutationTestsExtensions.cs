using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk04TransactionalMutationTestsExtensions
    {
        [Fact]
        public void ExtensionPutAndRemoveCommitAsOneCanonicalCandidate()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var replacementPayload = new byte[] { 0x11, 0x22, 0x33 };
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "request.extension-roundtrip",
                PutExtension("future.alpha", 2, replacementPayload),
                RemoveExtension("future.beta", 1));

            var plan = Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.PlanName,
                request);
            var changes = Hk04TransactionalMutationTests.List(
                Hk04TransactionalMutationTests.Map(plan.Data!, "plan"),
                "changes");
            Assert.Equal(2, changes.Count);

            var apply = Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                request);
            Assert.True((bool)apply.Data!["persisted"]!);
            Assert.Single(session.Current.Extensions);
            Assert.Equal("future.alpha", session.Current.Extensions[0].Owner);
            Assert.Equal(2, session.Current.Extensions[0].SchemaVersion);
            Assert.Equal(replacementPayload, session.Current.Extensions[0].GetPayloadCopy());
        }

        [Fact]
        public async Task ConcurrentWritersFromSameAnchorCannotBothCommit()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var requestA = Hk04TransactionalMutationTests.Request(
                initial,
                "request.concurrent-a",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.concurrent-a"));
            var requestB = Hk04TransactionalMutationTests.Request(
                initial,
                "request.concurrent-b",
                Hk04TransactionalMutationTests.PutObject("node.peer", "fixture.concurrent-b"));
            using var start = new Barrier(2);

            Task<CapabilityInvocationResult> Run(IReadOnlyDictionary<string, object?> request)
            {
                return Task.Run(() =>
                {
                    start.SignalAndWait();
                    return contract.Dispatch(
                        WorldMutationContract.ApplyName,
                        Hk04TransactionalMutationTests.ExactVersion(),
                        request);
                });
            }

            var results = await Task.WhenAll(Run(requestA), Run(requestB));
            var successes = 0;
            CapabilityInvocationResult? failure = null;
            foreach (var result in results)
            {
                if (result.Success)
                {
                    successes++;
                }
                else
                {
                    failure = result;
                }
            }

            Assert.Equal(1, successes);
            Assert.NotNull(failure);
            Assert.Contains(
                failure!.Error!.MachineCode,
                new[] { "world.change.stale_revision", "world.change.concurrent_update" });
            Assert.Equal(initial.Revision + 1, session.Current.Revision);
        }

        private static IReadOnlyDictionary<string, object?> PutExtension(
            string owner,
            int schemaVersion,
            byte[] payload)
        {
            return Hk03InspectionTests.ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-extension",
                ["owner"] = owner,
                ["schemaVersion"] = schemaVersion,
                ["payloadBase64"] = Convert.ToBase64String(payload)
            });
        }

        private static IReadOnlyDictionary<string, object?> RemoveExtension(string owner, int schemaVersion)
        {
            return Hk03InspectionTests.ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "remove-extension",
                ["owner"] = owner,
                ["schemaVersion"] = schemaVersion
            });
        }
    }
}
