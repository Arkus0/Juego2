using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk05ContentShapeProbeTests
    {
        [Fact]
        public void PotesHeroSliceFitsValidationAndBrokenAuthoredReferenceFailsClosed()
        {
            var plazaId = new WorldObjectId("place.plaza");
            var barId = new WorldObjectId("building.bar");
            var shopId = new WorldObjectId("building.shop");
            var anaId = new WorldObjectId("npc.ana");
            var worksAt = new WorldReferenceKind("works.at");

            var world = new WorldState(
                new WorldId("world.potes"),
                7,
                new WorldObject[]
                {
                    new WorldObject(plazaId, new WorldTypeId("fixture.place")),
                    new WorldObject(barId, new WorldTypeId("fixture.building"), plazaId),
                    new WorldObject(shopId, new WorldTypeId("fixture.building"), plazaId),
                    new WorldObject(
                        anaId,
                        new WorldTypeId("fixture.npc"),
                        plazaId,
                        new[] { new WorldReference(worksAt, barId) })
                },
                new[]
                {
                    new WorldExtensionData(
                        "arkus.npc-profile",
                        1,
                        new byte[] { 0x50, 0x4f, 0x54, 0x45, 0x53 },
                        anaId,
                        new[] { new WorldReference(worksAt, barId) })
                });

            var session = new TransactionalWorldAuthoringSession(world);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);

            var current = Hk04TransactionalMutationTests.Success(
                contract,
                WorldValidationContract.CurrentName,
                new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.True((bool)current.Data!["valid"]!);
            Assert.Equal(0, current.Data["diagnosticCount"]);

            var summary = Hk04TransactionalMutationTests.Success(
                contract,
                WorldInspectionContract.SummaryName,
                new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.Equal(4, summary.Data!["objectCount"]);
            Assert.Equal(1, summary.Data["extensionCount"]);

            var removeBar = Hk04TransactionalMutationTests.Request(
                world,
                "request.potes-remove-bar",
                Hk04TransactionalMutationTests.RemoveObject("building.bar"));

            var proposed = Hk04TransactionalMutationTests.Success(
                contract,
                WorldValidationContract.ProposedName,
                removeBar);
            Assert.False((bool)proposed.Data!["valid"]!);
            Assert.True((int)proposed.Data["diagnosticCount"]! >= 2);

            var diagnosticResources = DiagnosticResources(proposed.Data);
            Assert.Contains("world.object:npc.ana", diagnosticResources);
            Assert.Contains("world.extension:arkus.npc-profile@1@object:npc.ana", diagnosticResources);

            var apply = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                removeBar);
            Assert.False(apply.Success);
            Assert.Equal("world.change.invalid_candidate", apply.Error!.MachineCode);
            Assert.Equal(7, session.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
        }

        private static IReadOnlyList<string> DiagnosticResources(IReadOnlyDictionary<string, object?> validation)
        {
            var resources = new List<string>();
            var diagnostics = (IReadOnlyList<object?>)validation["diagnostics"]!;
            foreach (var raw in diagnostics)
            {
                var diagnostic = (IReadOnlyDictionary<string, object?>)raw!;
                resources.Add((string)diagnostic["resource"]!);
            }

            return resources.AsReadOnly();
        }
    }
}
