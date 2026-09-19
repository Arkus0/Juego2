using System;
using System.Collections.Generic;
using Arkus.Game.World;

namespace Arkus.Harness.Tests
{
    internal static class Hk02TestFixtures
    {
        public static WorldState MicroWorld(bool reverseInputOrder = false)
        {
            var root = new WorldObject(
                new WorldObjectId("node.root"),
                new WorldTypeId("fixture.container"));

            var childReferences = new List<WorldReference>
            {
                new WorldReference(new WorldReferenceKind("fixture.peer"), new WorldObjectId("node.peer")),
                new WorldReference(new WorldReferenceKind("fixture.root"), new WorldObjectId("node.root"))
            };
            if (reverseInputOrder)
            {
                childReferences.Reverse();
            }

            var child = new WorldObject(
                new WorldObjectId("node.child"),
                new WorldTypeId("fixture.item"),
                new WorldObjectId("node.root"),
                childReferences);

            var peer = new WorldObject(
                new WorldObjectId("node.peer"),
                new WorldTypeId("fixture.item"));

            var objects = new List<WorldObject> { root, child, peer };
            var extensions = new List<WorldExtensionData>
            {
                new WorldExtensionData("future.alpha", 2, new byte[] { 0x01, 0x02, 0x03 }),
                new WorldExtensionData("future.beta", 1, new byte[] { 0x04, 0x05 })
            };

            if (reverseInputOrder)
            {
                objects.Reverse();
                extensions.Reverse();
            }

            return new WorldState(new WorldId("world.fixture"), 7, objects, extensions);
        }

        public static WorldState SemanticMutationState(
            string worldId = "world.fixture",
            long revision = 7,
            string peerId = "node.peer",
            string childType = "fixture.item",
            bool childContained = true,
            string referenceKind = "fixture.peer",
            bool referenceToRoot = false,
            string extensionOwner = "future.alpha",
            int extensionVersion = 2,
            byte extensionPayloadMarker = 1)
        {
            var rootId = new WorldObjectId("node.root");
            var peerObjectId = new WorldObjectId(peerId);
            var target = referenceToRoot ? rootId : peerObjectId;

            var objects = new[]
            {
                new WorldObject(rootId, new WorldTypeId("fixture.container")),
                new WorldObject(
                    new WorldObjectId("node.child"),
                    new WorldTypeId(childType),
                    childContained ? rootId : (WorldObjectId?)null,
                    new[]
                    {
                        new WorldReference(new WorldReferenceKind(referenceKind), target)
                    }),
                new WorldObject(peerObjectId, new WorldTypeId("fixture.item"))
            };

            return new WorldState(
                new WorldId(worldId),
                revision,
                objects,
                new[]
                {
                    new WorldExtensionData(extensionOwner, extensionVersion, new[] { extensionPayloadMarker, (byte)0x7f })
                });
        }
    }
}
