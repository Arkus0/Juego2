using System;
using System.Globalization;
using System.Text;
using Arkus.Game.World;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk02CanonicalWorldStateTests
    {
        [Fact]
        public void StableTypedIdentifiersAreValueBasedAndNotRuntimeObjectIdentity()
        {
            var first = new WorldObjectId("node.alpha");
            var second = new WorldObjectId(new string(new[] { 'n', 'o', 'd', 'e', '.', 'a', 'l', 'p', 'h', 'a' }));
            var world = new WorldId("node.alpha");

            Assert.Equal(first, second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
            Assert.False(first.Equals(world));
            Assert.Equal("node.alpha", first.Value);
        }

        [Fact]
        public void CanonicalSerializationAndHashIgnoreCallerOrderingAndCulture()
        {
            var first = Hk02TestFixtures.MicroWorld(false);
            var reordered = Hk02TestFixtures.MicroWorld(true);
            var originalCulture = CultureInfo.CurrentCulture;
            var originalUiCulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("tr-TR");
                var firstBytes = CanonicalWorldStateCodec.Serialize(first);

                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
                var secondBytes = CanonicalWorldStateCodec.Serialize(reordered);

                Assert.Equal(firstBytes, secondBytes);
                Assert.Equal(
                    CanonicalWorldStateCodec.ComputeContentHash(first),
                    CanonicalWorldStateCodec.ComputeContentHash(reordered));
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUiCulture;
            }
        }

        [Fact]
        public void RoundTripPreservesExactCanonicalStateAndOpaqueForwardExtensions()
        {
            var original = Hk02TestFixtures.MicroWorld();
            var bytes = CanonicalWorldStateCodec.Serialize(original);
            var roundTripped = CanonicalWorldStateCodec.Deserialize(bytes);

            Assert.Equal(bytes, CanonicalWorldStateCodec.Serialize(roundTripped));
            Assert.Equal(original.Id, roundTripped.Id);
            Assert.Equal(original.Revision, roundTripped.Revision);
            Assert.Equal(original.SchemaVersion, roundTripped.SchemaVersion);
            Assert.Equal(3, roundTripped.Objects.Count);
            Assert.Equal(2, roundTripped.Extensions.Count);

            WorldExtensionData? alpha = null;
            for (var index = 0; index < roundTripped.Extensions.Count; index++)
            {
                if (StringComparer.Ordinal.Equals(roundTripped.Extensions[index].Owner, "future.alpha"))
                {
                    alpha = roundTripped.Extensions[index];
                    break;
                }
            }

            Assert.NotNull(alpha);
            Assert.Equal(2, alpha!.SchemaVersion);
            Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, alpha.GetPayloadCopy());
        }

        [Fact]
        public void MicroWorldExercisesIdentityContainmentAndReferenceEdgesWithoutGameplayModel()
        {
            var state = Hk02TestFixtures.MicroWorld();
            WorldStateValidator.ValidateOrThrow(state);

            WorldObject? child = null;
            for (var index = 0; index < state.Objects.Count; index++)
            {
                if (state.Objects[index].Id == new WorldObjectId("node.child"))
                {
                    child = state.Objects[index];
                    break;
                }
            }

            Assert.NotNull(child);
            Assert.Equal(new WorldObjectId("node.root"), child!.ContainerId);
            Assert.Equal(2, child.References.Count);
        }

        [Fact]
        public void ContentHashIsStableLowercaseSha256OfCanonicalBytes()
        {
            var state = Hk02TestFixtures.MicroWorld();
            var first = CanonicalWorldStateCodec.ComputeContentHash(state);
            var second = CanonicalWorldStateCodec.ComputeContentHash(state);

            Assert.Equal(first, second);
            Assert.Equal(64, first.Length);
            Assert.Equal(first, first.ToLowerInvariant());
            Assert.DoesNotContain("-", first);
        }

        [Fact]
        public void CanonicalPayloadUsesOnlyLfFramingAndInvariantAsciiStructure()
        {
            var payload = Encoding.UTF8.GetString(CanonicalWorldStateCodec.Serialize(Hk02TestFixtures.MicroWorld()));

            Assert.StartsWith("ARKUS_WORLD_STATE_V1\nschema\t1\n", payload);
            Assert.EndsWith("end\n", payload);
            Assert.DoesNotContain("\r", payload);
        }
    }
}
