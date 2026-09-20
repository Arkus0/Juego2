using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08AInteractionShapeTests
    {
        private static readonly ContractVersionRange ExactV1 =
            ContractVersionRange.Exact(new ContractVersion(1, 0));

        [Fact]
        public void OneCoherentMutationRequestUpdatesSeveralObjectPropertiesTogether()
        {
            var initial = new WorldState(
                new WorldId("world.hk08a.multi-property"),
                0,
                new[]
                {
                    new WorldObject(new WorldObjectId("item.000"), new WorldTypeId("fixture.hk08a.item")),
                    new WorldObject(new WorldObjectId("item.001"), new WorldTypeId("fixture.hk08a.container")),
                    new WorldObject(new WorldObjectId("item.002"), new WorldTypeId("fixture.hk08a.target"))
                });
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);

            var result = contract.Dispatch(
                WorldMutationContract.ApplyName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "request.hk08a.multi-property",
                    ["expectedRevision"] = initial.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(initial),
                    ["operations"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "put-object",
                            ["id"] = "item.000",
                            ["typeId"] = "fixture.hk08a.item.modified",
                            ["containerId"] = "item.001",
                            ["references"] = new object?[]
                            {
                                new Dictionary<string, object?>(StringComparer.Ordinal)
                                {
                                    ["kind"] = "related.to",
                                    ["targetId"] = "item.002"
                                }
                            }
                        }
                    }
                });

            Assert.True(result.Success);
            Assert.True((bool)result.Data!["persisted"]!);
            Assert.Equal(initial.Revision + 1, session.Current.Revision);

            var plan = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(result.Data!["plan"]);
            var changes = Assert.IsAssignableFrom<IReadOnlyList<object?>>(plan["changes"]);
            var change = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(Assert.Single(changes));
            Assert.Equal("world.object:item.000", change["resource"]);
            Assert.Equal("update", change["action"]);
            var fields = Assert.IsAssignableFrom<IReadOnlyList<object?>>(change["fields"])
                .Cast<string>()
                .ToArray();
            Assert.Contains("typeId", fields);
            Assert.Contains("containerId", fields);
            Assert.Contains("references", fields);

            var journal = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.True(journal.Success);
            Assert.Equal(1, Convert.ToInt32(journal.Data!["entryCount"]));
        }
    }
}
