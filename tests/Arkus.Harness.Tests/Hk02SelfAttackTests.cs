using System;
using System.Collections.Generic;
using System.Text;
using Arkus.Game.World;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk02SelfAttackTests
    {
        [Fact]
        public void UnstableOrderingMutantTurnsRedWhileCanonicalCodecStaysStable()
        {
            var first = Hk02TestFixtures.MicroWorld(false);
            var reordered = Hk02TestFixtures.MicroWorld(true);

            Assert.NotEqual(NaiveOrderFingerprint(first), NaiveOrderFingerprint(reordered));
            Assert.Equal(
                CanonicalWorldStateCodec.Serialize(first),
                CanonicalWorldStateCodec.Serialize(reordered));
        }

        [Fact]
        public void DuplicateIdentityFailsClosed()
        {
            var duplicate = new WorldObjectId("node.duplicate");
            var exception = Assert.Throws<WorldStateException>(() => new WorldState(
                new WorldId("world.fixture"),
                0,
                new[]
                {
                    new WorldObject(duplicate, new WorldTypeId("fixture.item")),
                    new WorldObject(duplicate, new WorldTypeId("fixture.container"))
                }));

            Assert.Equal("world.duplicate_id", exception.MachineCode);
        }

        [Fact]
        public void DanglingReferenceFailsClosed()
        {
            var exception = Assert.Throws<WorldStateException>(() => new WorldState(
                new WorldId("world.fixture"),
                0,
                new[]
                {
                    new WorldObject(
                        new WorldObjectId("node.source"),
                        new WorldTypeId("fixture.item"),
                        null,
                        new[]
                        {
                            new WorldReference(
                                new WorldReferenceKind("fixture.link"),
                                new WorldObjectId("node.missing"))
                        })
                }));

            Assert.Equal("world.dangling_reference", exception.MachineCode);
        }

        [Fact]
        public void SerializationOrderNoiseCannotChangeContentHash()
        {
            var first = Hk02TestFixtures.MicroWorld(false);
            var reordered = Hk02TestFixtures.MicroWorld(true);

            Assert.NotEqual(NaiveOrderFingerprint(first), NaiveOrderFingerprint(reordered));
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(first),
                CanonicalWorldStateCodec.ComputeContentHash(reordered));
        }

        [Fact]
        public void EveryCurrentSemanticFieldClassChangesCanonicalHashWhenMeaningChanges()
        {
            var baseline = CanonicalWorldStateCodec.ComputeContentHash(Hk02TestFixtures.SemanticMutationState());
            var variants = new List<WorldState>
            {
                Hk02TestFixtures.SemanticMutationState(worldId: "world.other"),
                Hk02TestFixtures.SemanticMutationState(revision: 8),
                Hk02TestFixtures.SemanticMutationState(peerId: "node.other"),
                Hk02TestFixtures.SemanticMutationState(childType: "fixture.other"),
                Hk02TestFixtures.SemanticMutationState(childContained: false),
                Hk02TestFixtures.SemanticMutationState(referenceKind: "fixture.other"),
                Hk02TestFixtures.SemanticMutationState(referenceToRoot: true),
                Hk02TestFixtures.SemanticMutationState(extensionOwner: "future.other"),
                Hk02TestFixtures.SemanticMutationState(extensionVersion: 3),
                Hk02TestFixtures.SemanticMutationState(extensionPayloadMarker: 2)
            };

            for (var index = 0; index < variants.Count; index++)
            {
                Assert.NotEqual(baseline, CanonicalWorldStateCodec.ComputeContentHash(variants[index]));
            }
        }

        [Fact]
        public void UnsupportedSchemaPayloadFailsClosed()
        {
            var canonical = Encoding.UTF8.GetString(CanonicalWorldStateCodec.Serialize(Hk02TestFixtures.MicroWorld()));
            var unsupported = canonical.Replace("schema\t1\n", "schema\t2\n", StringComparison.Ordinal);

            var exception = Assert.Throws<WorldStateException>(() =>
                CanonicalWorldStateCodec.Deserialize(Encoding.UTF8.GetBytes(unsupported)));

            Assert.Equal("world.unsupported_schema", exception.MachineCode);
        }

        [Fact]
        public void UnknownStructuralRecordFailsClosedInsteadOfBeingSilentlyDropped()
        {
            var canonical = Encoding.UTF8.GetString(CanonicalWorldStateCodec.Serialize(Hk02TestFixtures.MicroWorld()));
            var unknown = canonical.Replace("end\n", "future-record\tAA==\nend\n", StringComparison.Ordinal);

            var exception = Assert.Throws<WorldStateException>(() =>
                CanonicalWorldStateCodec.Deserialize(Encoding.UTF8.GetBytes(unknown)));

            Assert.Equal("world.unknown_record", exception.MachineCode);
        }

        [Fact]
        public void DanglingContainmentFailsClosed()
        {
            var exception = Assert.Throws<WorldStateException>(() => new WorldState(
                new WorldId("world.fixture"),
                0,
                new[]
                {
                    new WorldObject(
                        new WorldObjectId("node.child"),
                        new WorldTypeId("fixture.item"),
                        new WorldObjectId("node.missing"))
                }));

            Assert.Equal("world.dangling_container", exception.MachineCode);
        }

        [Fact]
        public void ContainmentCycleFailsClosed()
        {
            var first = new WorldObjectId("node.first");
            var second = new WorldObjectId("node.second");

            var exception = Assert.Throws<WorldStateException>(() => new WorldState(
                new WorldId("world.fixture"),
                0,
                new[]
                {
                    new WorldObject(first, new WorldTypeId("fixture.item"), second),
                    new WorldObject(second, new WorldTypeId("fixture.item"), first)
                }));

            Assert.Equal("world.containment_cycle", exception.MachineCode);
        }

        [Fact]
        public void NonCanonicalPayloadOrderingFailsClosedEvenWhenSemanticsCouldBeRecovered()
        {
            var canonical = Encoding.UTF8.GetString(CanonicalWorldStateCodec.Serialize(Hk02TestFixtures.MicroWorld()));
            var firstObjectStart = canonical.IndexOf("object\t", StringComparison.Ordinal);
            var firstReferenceStart = canonical.IndexOf("reference\t", StringComparison.Ordinal);
            Assert.True(firstObjectStart > 0);
            Assert.True(firstReferenceStart > firstObjectStart);

            var objectBlock = canonical.Substring(firstObjectStart, firstReferenceStart - firstObjectStart);
            var lines = objectBlock.Split('\n');
            Assert.True(lines.Length >= 4);
            var swappedBlock = lines[1] + "\n" + lines[0] + "\n" + lines[2] + "\n";
            var mutated = canonical.Substring(0, firstObjectStart) + swappedBlock + canonical.Substring(firstReferenceStart);

            var exception = Assert.Throws<WorldStateException>(() =>
                CanonicalWorldStateCodec.Deserialize(Encoding.UTF8.GetBytes(mutated)));

            Assert.Equal("world.noncanonical_payload", exception.MachineCode);
        }

        private static string NaiveOrderFingerprint(WorldState state)
        {
            var builder = new StringBuilder();
            for (var objectIndex = 0; objectIndex < state.Objects.Count; objectIndex++)
            {
                var current = state.Objects[objectIndex];
                builder.Append(current.Id.Value).Append('|');
                for (var referenceIndex = 0; referenceIndex < current.References.Count; referenceIndex++)
                {
                    builder.Append(current.References[referenceIndex].Kind.Value)
                        .Append('>')
                        .Append(current.References[referenceIndex].TargetId.Value)
                        .Append('|');
                }
            }

            for (var extensionIndex = 0; extensionIndex < state.Extensions.Count; extensionIndex++)
            {
                builder.Append(state.Extensions[extensionIndex].Owner).Append('|');
            }

            return builder.ToString();
        }
    }
}
