using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk03InspectionTests
    {
        [Fact]
        public void CanonicalDiscoveryPublishesAllWorldReadsWithSchemas()
        {
            var contract = Compose(Hk02TestFixtures.MicroWorld());
            var expected = ExpectedWorldCommands();
            var discovered = new HashSet<string>(StringComparer.Ordinal);

            foreach (var definition in contract.Definitions)
            {
                if (!definition.Key.Name.StartsWith("world.", StringComparison.Ordinal))
                {
                    continue;
                }

                discovered.Add(definition.Key.Name);
                Assert.NotNull(definition.RequestSchema);
                Assert.NotNull(definition.SuccessSchema);
                Assert.NotNull(definition.ErrorSchema);
                Assert.Equal(SideEffectClass.ReadOnly, definition.SideEffect);
                Assert.Equal(DeterminismClass.Deterministic, definition.Determinism);
                Assert.Equal(PrivilegeClass.PublicRead, definition.Policy!.Privilege);
            }

            Assert.Equal(expected, discovered);

            var describe = contract.Dispatch(
                "system.describe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                Empty());
            Assert.True(describe.Success);
            Assert.Empty(CanonicalProjectionConformance.Compare(contract.Definitions, describe.Data!));
        }

        [Fact]
        public void SummaryAndObjectLookupBindEveryResponseToRevisionAndHash()
        {
            var state = Hk02TestFixtures.MicroWorld();
            var contract = Compose(state);

            var summary = Dispatch(contract, WorldInspectionContract.SummaryName, Empty());
            var summaryWorld = Map(summary.Data!, "world");
            Assert.Equal(state.Id.Value, summaryWorld["worldId"]);
            Assert.Equal(state.SchemaVersion, summaryWorld["schemaVersion"]);
            Assert.Equal(state.Revision, summaryWorld["revision"]);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(state), summaryWorld["hash"]);
            Assert.Equal(3, summary.Data!["objectCount"]);
            Assert.Equal(2, summary.Data["referenceCount"]);
            Assert.Equal(2, summary.Data["extensionCount"]);

            var request = Anchor(state);
            request["id"] = "node.child";
            var result = Dispatch(contract, WorldInspectionContract.ObjectGetName, request);
            var world = Map(result.Data!, "world");
            Assert.Equal(state.Revision, world["revision"]);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(state), world["hash"]);

            var value = Map(result.Data, "object");
            Assert.Equal("node.child", value["id"]);
            Assert.Equal("fixture.item", value["typeId"]);
            Assert.Equal("node.root", value["containerId"]);
            Assert.Equal(2, List(value, "references").Count);
        }

        [Fact]
        public void ObjectQueryIsFilteredProjectedPaginatedAndCanonicalOrder()
        {
            var state = Hk02TestFixtures.MicroWorld(reverseInputOrder: true);
            var contract = Compose(state);
            var request = Anchor(state);
            request["limit"] = 1;
            request["filter"] = ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["typeIds"] = ReadOnlyList("fixture.item")
            });
            request["fields"] = ReadOnlyList("typeId");

            var first = Dispatch(contract, WorldInspectionContract.ObjectQueryName, request);
            var firstItems = List(first.Data!, "items");
            Assert.Single(firstItems);
            var firstObject = (IReadOnlyDictionary<string, object?>)firstItems[0]!;
            Assert.Equal("node.child", firstObject["id"]);
            Assert.Equal("fixture.item", firstObject["typeId"]);
            Assert.False(firstObject.ContainsKey("containerId"));
            Assert.False(firstObject.ContainsKey("references"));
            Assert.True(first.Data!.TryGetValue("nextCursor", out var cursor));

            var secondRequest = Anchor(state);
            secondRequest["limit"] = 1;
            secondRequest["filter"] = request["filter"];
            secondRequest["fields"] = request["fields"];
            secondRequest["cursor"] = cursor;
            var second = Dispatch(contract, WorldInspectionContract.ObjectQueryName, secondRequest);
            var secondObject = (IReadOnlyDictionary<string, object?>)List(second.Data!, "items")[0]!;
            Assert.Equal("node.peer", secondObject["id"]);
            Assert.False(second.Data!.ContainsKey("nextCursor"));
        }

        [Fact]
        public void ReferenceQueryProvidesDeterministicRelationshipSurface()
        {
            var state = Hk02TestFixtures.MicroWorld();
            var contract = Compose(state);
            var request = Anchor(state);
            request["filter"] = ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["sourceIds"] = ReadOnlyList("node.child")
            });

            var result = Dispatch(contract, WorldInspectionContract.ReferenceQueryName, request);
            var rows = List(result.Data!, "items");
            Assert.Equal(2, rows.Count);
            var first = (IReadOnlyDictionary<string, object?>)rows[0]!;
            var second = (IReadOnlyDictionary<string, object?>)rows[1]!;
            Assert.Equal("fixture.peer", first["kind"]);
            Assert.Equal("node.peer", first["targetId"]);
            Assert.Equal("fixture.root", second["kind"]);
            Assert.Equal("node.root", second["targetId"]);
        }

        [Fact]
        public void ExtensionDescriptorsAndBoundedChunksExposeOpaqueStateCompletely()
        {
            var payload = new byte[WorldInspectionService.MaximumExtensionChunkBytes * 2 + 17];
            for (var index = 0; index < payload.Length; index++)
            {
                payload[index] = (byte)(index % 251);
            }

            var state = new WorldState(
                new WorldId("world.large-extension"),
                11,
                new[] { new WorldObject(new WorldObjectId("node.root"), new WorldTypeId("fixture.root")) },
                new[] { new WorldExtensionData("future.large", 3, payload) });
            var contract = Compose(state);

            var descriptor = Dispatch(contract, WorldInspectionContract.ExtensionQueryName, Anchor(state));
            var descriptorRow = (IReadOnlyDictionary<string, object?>)List(descriptor.Data!, "items")[0]!;
            Assert.Equal("future.large", descriptorRow["owner"]);
            Assert.Equal(3, descriptorRow["schemaVersion"]);
            Assert.Equal(payload.Length, descriptorRow["payloadLength"]);

            var reconstructed = new List<byte>();
            var offset = 0;
            while (offset < payload.Length)
            {
                var request = Anchor(state);
                request["owner"] = "future.large";
                request["schemaVersion"] = 3;
                request["offset"] = offset;
                request["limit"] = WorldInspectionService.MaximumExtensionChunkBytes;
                var chunk = Dispatch(contract, WorldInspectionContract.ExtensionReadName, request);
                var bytes = Convert.FromBase64String((string)chunk.Data!["payloadBase64"]!);
                reconstructed.AddRange(bytes);
                if (!chunk.Data.TryGetValue("nextOffset", out var next))
                {
                    break;
                }

                offset = Convert.ToInt32(next, System.Globalization.CultureInfo.InvariantCulture);
            }

            Assert.Equal(payload, reconstructed.ToArray());
        }

        [Fact]
        public void MissingResourceAndStaleAnchorReturnStableStructuredErrors()
        {
            var state = Hk02TestFixtures.MicroWorld();
            var contract = Compose(state);

            var missing = Anchor(state);
            missing["id"] = "node.missing";
            var missingResult = contract.Dispatch(
                WorldInspectionContract.ObjectGetName,
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                missing);
            Assert.False(missingResult.Success);
            Assert.Equal("world.object_not_found", missingResult.Error!.MachineCode);

            var stale = Anchor(state);
            stale["revision"] = state.Revision - 1;
            var staleResult = contract.Dispatch(
                WorldInspectionContract.ObjectQueryName,
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                stale);
            Assert.False(staleResult.Success);
            Assert.Equal("world.stale_revision", staleResult.Error!.MachineCode);
            Assert.True(staleResult.Error.Retryable);
        }

        [Fact]
        public void ReadsRemainSideEffectFreeAndRepeatableAgainstSameState()
        {
            var state = Hk02TestFixtures.MicroWorld();
            var hashBefore = CanonicalWorldStateCodec.ComputeContentHash(state);
            var contract = Compose(state);
            var request = Anchor(state);
            request["limit"] = 2;

            var first = Dispatch(contract, WorldInspectionContract.ObjectQueryName, request);
            var second = Dispatch(contract, WorldInspectionContract.ObjectQueryName, request);

            Assert.Equal(ItemIds(first), ItemIds(second));
            Assert.Equal(first.Data!["nextCursor"], second.Data!["nextCursor"]);
            Assert.Equal(hashBefore, CanonicalWorldStateCodec.ComputeContentHash(state));
            Assert.Equal(state.Revision, Map(first.Data, "world")["revision"]);
            Assert.Equal(hashBefore, Map(first.Data, "world")["hash"]);
        }

        internal static ComposedContract Compose(WorldState state)
        {
            return CanonicalWorldContract.Compose(
                new WorldInspectionService(new FixedWorldStateSource(state)));
        }

        internal static Dictionary<string, object?> Anchor(WorldState state)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = state.Revision,
                ["hash"] = CanonicalWorldStateCodec.ComputeContentHash(state)
            };
        }

        internal static CapabilityInvocationResult Dispatch(
            ComposedContract contract,
            string capability,
            IReadOnlyDictionary<string, object?> request)
        {
            var result = contract.Dispatch(
                capability,
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                request);
            Assert.True(result.Success, result.Error == null ? "unknown failure" : result.Error.MachineCode + ":" + result.Error.Message);
            Assert.NotNull(result.Data);
            return result;
        }

        internal static IReadOnlyDictionary<string, object?> Map(
            IReadOnlyDictionary<string, object?> data,
            string key)
        {
            return (IReadOnlyDictionary<string, object?>)data[key]!;
        }

        internal static IReadOnlyList<object?> List(
            IReadOnlyDictionary<string, object?> data,
            string key)
        {
            return (IReadOnlyList<object?>)data[key]!;
        }

        internal static IReadOnlyDictionary<string, object?> ReadOnlyMap(IDictionary<string, object?> values)
        {
            return new System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(values, StringComparer.Ordinal));
        }

        internal static IReadOnlyList<object?> ReadOnlyList(params object?[] values)
        {
            return new List<object?>(values).AsReadOnly();
        }

        internal static HashSet<string> ExpectedWorldCommands()
        {
            return new HashSet<string>(StringComparer.Ordinal)
            {
                WorldInspectionContract.SummaryName,
                WorldInspectionContract.ObjectGetName,
                WorldInspectionContract.ObjectQueryName,
                WorldInspectionContract.ReferenceQueryName,
                WorldInspectionContract.ExtensionQueryName,
                WorldInspectionContract.ExtensionReadName
            };
        }

        private static Dictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }

        private static string ItemIds(CapabilityInvocationResult result)
        {
            var ids = new List<string>();
            foreach (var item in List(result.Data!, "items"))
            {
                ids.Add((string)((IReadOnlyDictionary<string, object?>)item!)["id"]!);
            }

            return string.Join("|", ids);
        }
    }
}
