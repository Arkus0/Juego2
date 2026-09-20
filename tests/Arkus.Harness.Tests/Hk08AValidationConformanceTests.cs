using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk08AValidationConformanceTests
    {
        private static readonly ContractVersionRange ExactV1 =
            ContractVersionRange.Exact(new ContractVersion(1, 0));

        [Fact]
        public void ParsedBatchThatViolatesWorldValidationCannotPublishOrAppendProvenance()
        {
            var initial = new WorldState(
                new WorldId("world.hk08a.validation-negative"),
                0,
                Array.Empty<WorldObject>());
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);

            // Both operations are individually parseable. The second introduces a containment target
            // that does not exist, so only the accepted candidate-level validator can reject the batch.
            var result = contract.Dispatch(
                WorldMutationContract.ApplyName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "request.hk08a.validation-negative",
                    ["expectedRevision"] = initial.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(initial),
                    ["operations"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "put-object",
                            ["id"] = "item.valid",
                            ["typeId"] = "fixture.hk08a.item",
                            ["references"] = Array.Empty<object?>()
                        },
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "put-object",
                            ["id"] = "item.invalid-container",
                            ["typeId"] = "fixture.hk08a.item",
                            ["containerId"] = "container.missing",
                            ["references"] = Array.Empty<object?>()
                        }
                    }
                });

            Assert.False(result.Success);
            Assert.Equal(initial.Revision, session.Current.Revision);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(initial),
                CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Empty(session.Current.Objects);

            var journal = contract.Dispatch(
                WorldProvenanceContract.ReadName,
                ExactV1,
                new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.True(journal.Success);
            Assert.Equal(0, Convert.ToInt32(journal.Data!["entryCount"]));
        }
    }
}
