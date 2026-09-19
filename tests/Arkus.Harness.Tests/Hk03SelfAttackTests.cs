using System;
using System.Collections.Generic;
using System.Reflection;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk03SelfAttackTests
    {
        [Fact]
        public void HiddenAuthorableStateMutantBreaksIndependentReadReconstructionHash()
        {
            var original = Hk02TestFixtures.MicroWorld(reverseInputOrder: true);
            var reconstructed = ReconstructThroughPublicReads(original);

            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(original),
                CanonicalWorldStateCodec.ComputeContentHash(reconstructed));

            var hiddenExtensionMutant = new WorldState(
                reconstructed.Id,
                reconstructed.Revision,
                reconstructed.Objects,
                new[]
                {
                    new WorldExtensionData("future.alpha", 2, Array.Empty<byte>()),
                    new WorldExtensionData("future.beta", 1, new byte[] { 0x04, 0x05 })
                },
                reconstructed.SchemaVersion);

            Assert.NotEqual(
                CanonicalWorldStateCodec.ComputeContentHash(original),
                CanonicalWorldStateCodec.ComputeContentHash(hiddenExtensionMutant));

            AssertCurrentSemanticSurfaceIsExplicitlyMapped();
        }

        [Fact]
        public void NondeterministicQueryOrderMutantCannotLeakInputOrdering()
        {
            var first = Hk02TestFixtures.MicroWorld(false);
            var reordered = Hk02TestFixtures.MicroWorld(true);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(first),
                CanonicalWorldStateCodec.ComputeContentHash(reordered));

            var firstPage = Page(Compose(first), first, 1);
            var reorderedPage = Page(Compose(reordered), reordered, 1);

            Assert.Equal(firstPage.Id, reorderedPage.Id);
            Assert.Equal(firstPage.Cursor, reorderedPage.Cursor);
            Assert.Equal("node.child", firstPage.Id);
        }

        [Fact]
        public void UnboundedNestedRelationshipMutantIsSplitIntoBoundedReferencePages()
        {
            var objects = new List<WorldObject>();
            var references = new List<WorldReference>();
            for (var index = 0; index < 130; index++)
            {
                var suffix = index.ToString("D3", System.Globalization.CultureInfo.InvariantCulture);
                var targetId = new WorldObjectId("node.target." + suffix);
                objects.Add(new WorldObject(targetId, new WorldTypeId("fixture.target")));
                references.Add(new WorldReference(new WorldReferenceKind("fixture.link"), targetId));
            }

            objects.Add(new WorldObject(
                new WorldObjectId("node.root"),
                new WorldTypeId("fixture.root"),
                null,
                references));
            var state = new WorldState(new WorldId("world.many-links"), 4, objects);
            var contract = Compose(state);

            var objectRequest = Anchor(state);
            objectRequest["id"] = "node.root";
            var objectResult = Success(contract, WorldInspectionContract.ObjectGetName, objectRequest);
            var projected = Hk03InspectionTests.Map(objectResult.Data!, "object");
            Assert.False(projected.ContainsKey("references"));

            var firstRequest = Anchor(state);
            firstRequest["limit"] = WorldInspectionService.MaximumPageSize;
            firstRequest["filter"] = Hk03InspectionTests.ReadOnlyMap(
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["sourceIds"] = Hk03InspectionTests.ReadOnlyList("node.root")
                });
            var first = Success(contract, WorldInspectionContract.ReferenceQueryName, firstRequest);
            Assert.Equal(WorldInspectionService.MaximumPageSize, Hk03InspectionTests.List(first.Data!, "items").Count);
            Assert.True(first.Data!.TryGetValue("nextCursor", out var cursor));

            var secondRequest = Anchor(state);
            secondRequest["limit"] = WorldInspectionService.MaximumPageSize;
            secondRequest["filter"] = firstRequest["filter"];
            secondRequest["cursor"] = cursor;
            var second = Success(contract, WorldInspectionContract.ReferenceQueryName, secondRequest);
            Assert.Equal(30, Hk03InspectionTests.List(second.Data!, "items").Count);
            Assert.False(second.Data!.ContainsKey("nextCursor"));
        }

        [Fact]
        public void UnboundedDeclaredLimitsFailClosed()
        {
            var state = Hk02TestFixtures.MicroWorld();
            var contract = Compose(state);

            var query = Anchor(state);
            query["limit"] = WorldInspectionService.MaximumPageSize + 1;
            var queryResult = contract.Dispatch(
                WorldInspectionContract.ObjectQueryName,
                ExactVersion(),
                query);
            Assert.False(queryResult.Success);
            Assert.Equal("world.invalid_selector", queryResult.Error!.MachineCode);
            Assert.Equal("$.limit", queryResult.Error.Path);

            var extension = Anchor(state);
            extension["owner"] = "future.alpha";
            extension["schemaVersion"] = 2;
            extension["limit"] = WorldInspectionService.MaximumExtensionChunkBytes + 1;
            var extensionResult = contract.Dispatch(
                WorldInspectionContract.ExtensionReadName,
                ExactVersion(),
                extension);
            Assert.False(extensionResult.Success);
            Assert.Equal("world.invalid_selector", extensionResult.Error!.MachineCode);
            Assert.Equal("$.limit", extensionResult.Error.Path);
        }

        [Fact]
        public void StaleRevisionAndCursorAreRejectedAfterSourceAdvances()
        {
            var original = Hk02TestFixtures.MicroWorld();
            var source = new MutableWorldSource(original);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(source));

            var firstRequest = Anchor(original);
            firstRequest["limit"] = 1;
            var first = Success(contract, WorldInspectionContract.ObjectQueryName, firstRequest);
            var cursor = (string)first.Data!["nextCursor"]!;

            var advanced = new WorldState(
                original.Id,
                original.Revision + 1,
                original.Objects,
                original.Extensions,
                original.SchemaVersion);
            source.Current = advanced;

            var staleRevision = contract.Dispatch(
                WorldInspectionContract.ObjectQueryName,
                ExactVersion(),
                firstRequest);
            Assert.False(staleRevision.Success);
            Assert.Equal("world.stale_revision", staleRevision.Error!.MachineCode);

            var staleCursorRequest = Anchor(advanced);
            staleCursorRequest["limit"] = 1;
            staleCursorRequest["cursor"] = cursor;
            var staleCursor = contract.Dispatch(
                WorldInspectionContract.ObjectQueryName,
                ExactVersion(),
                staleCursorRequest);
            Assert.False(staleCursor.Success);
            Assert.Equal("world.stale_cursor", staleCursor.Error!.MachineCode);
        }

        [Fact]
        public void SelectorCannotEscapeDeclaredStableTokenGrammar()
        {
            var state = Hk02TestFixtures.MicroWorld();
            var contract = Compose(state);
            var request = Anchor(state);
            request["filter"] = Hk03InspectionTests.ReadOnlyMap(
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["ids"] = Hk03InspectionTests.ReadOnlyList("node.child || true")
                });

            var result = contract.Dispatch(
                WorldInspectionContract.ObjectQueryName,
                ExactVersion(),
                request);

            Assert.False(result.Success);
            Assert.Equal("world.invalid_selector", result.Error!.MachineCode);
            Assert.Equal("$.filter.ids", result.Error.Path);
        }

        [Fact]
        public void EveryEffectiveWorldReadIsDiscoveredAndMissingSchemaMutantTurnsRed()
        {
            var contract = CanonicalWorldContract.Compose(new UnavailableWorldInspectionService());
            var routeUniverse = RouteUniverse.Enumerate(typeof(SystemDescribeHandler).Assembly);
            var conformance = CanonicalContractConformance.Evaluate(contract, routeUniverse);
            Assert.True(conformance.IsConformant, FormatIssues(conformance.Issues));

            var expected = Hk03InspectionTests.ExpectedWorldCommands();
            var actual = new HashSet<string>(StringComparer.Ordinal);
            CapabilityDefinition? representative = null;
            foreach (var definition in contract.Definitions)
            {
                if (!definition.Key.Name.StartsWith("world.", StringComparison.Ordinal))
                {
                    continue;
                }

                actual.Add(definition.Key.Name);
                representative ??= definition;
                Assert.NotNull(definition.RequestSchema);
                Assert.NotNull(definition.SuccessSchema);
                Assert.NotNull(definition.ErrorSchema);
            }

            Assert.True(expected.SetEquals(actual));
            Assert.NotNull(representative);

            var missingRequestSchema = new CapabilityDefinition(
                representative!.Key,
                representative.Provider,
                null,
                representative.SuccessSchema,
                representative.ErrorSchema,
                representative.SideEffect,
                representative.Determinism,
                representative.Preconditions,
                representative.Postconditions,
                representative.Concurrency,
                representative.Idempotency,
                representative.Batching,
                representative.Repair,
                representative.Policy,
                representative.Cost);
            var issues = CanonicalContractValidator.Validate(missingRequestSchema);
            Assert.Contains(issues, issue => issue.Code == "contract.missing_schema");
        }

        private static WorldState ReconstructThroughPublicReads(WorldState source)
        {
            var contract = Compose(source);
            var summary = Success(
                contract,
                WorldInspectionContract.SummaryName,
                new Dictionary<string, object?>(StringComparer.Ordinal));
            var metadata = Hk03InspectionTests.Map(summary.Data!, "world");

            var referencesBySource = ReadAllReferences(contract, source);
            var objects = new List<WorldObject>();
            string? objectCursor = null;
            do
            {
                var objectRequest = Anchor(source);
                objectRequest["limit"] = WorldInspectionService.MaximumPageSize;
                if (objectCursor != null)
                {
                    objectRequest["cursor"] = objectCursor;
                }

                var objectResult = Success(contract, WorldInspectionContract.ObjectQueryName, objectRequest);
                foreach (var rawObject in Hk03InspectionTests.List(objectResult.Data!, "items"))
                {
                    var data = (IReadOnlyDictionary<string, object?>)rawObject!;
                    WorldObjectId? container = null;
                    if (data.TryGetValue("containerId", out var rawContainer))
                    {
                        container = new WorldObjectId((string)rawContainer!);
                    }

                    var id = (string)data["id"]!;
                    referencesBySource.TryGetValue(id, out var references);
                    objects.Add(new WorldObject(
                        new WorldObjectId(id),
                        new WorldTypeId((string)data["typeId"]!),
                        container,
                        references ?? new List<WorldReference>()));
                }

                objectCursor = objectResult.Data!.TryGetValue("nextCursor", out var next)
                    ? (string)next!
                    : null;
            }
            while (objectCursor != null);

            var extensions = ReadAllExtensions(contract, source);
            return new WorldState(
                new WorldId((string)metadata["worldId"]!),
                Convert.ToInt64(metadata["revision"], System.Globalization.CultureInfo.InvariantCulture),
                objects,
                extensions,
                Convert.ToInt32(metadata["schemaVersion"], System.Globalization.CultureInfo.InvariantCulture));
        }

        private static Dictionary<string, List<WorldReference>> ReadAllReferences(
            ComposedContract contract,
            WorldState source)
        {
            var bySource = new Dictionary<string, List<WorldReference>>(StringComparer.Ordinal);
            string? cursor = null;
            do
            {
                var request = Anchor(source);
                request["limit"] = WorldInspectionService.MaximumPageSize;
                if (cursor != null)
                {
                    request["cursor"] = cursor;
                }

                var result = Success(contract, WorldInspectionContract.ReferenceQueryName, request);
                foreach (var rawReference in Hk03InspectionTests.List(result.Data!, "items"))
                {
                    var row = (IReadOnlyDictionary<string, object?>)rawReference!;
                    var sourceId = (string)row["sourceId"]!;
                    if (!bySource.TryGetValue(sourceId, out var references))
                    {
                        references = new List<WorldReference>();
                        bySource.Add(sourceId, references);
                    }

                    references.Add(new WorldReference(
                        new WorldReferenceKind((string)row["kind"]!),
                        new WorldObjectId((string)row["targetId"]!)));
                }

                cursor = result.Data!.TryGetValue("nextCursor", out var next)
                    ? (string)next!
                    : null;
            }
            while (cursor != null);

            return bySource;
        }

        private static List<WorldExtensionData> ReadAllExtensions(
            ComposedContract contract,
            WorldState source)
        {
            var extensions = new List<WorldExtensionData>();
            string? cursor = null;
            do
            {
                var query = Anchor(source);
                query["limit"] = WorldInspectionService.MaximumPageSize;
                if (cursor != null)
                {
                    query["cursor"] = cursor;
                }

                var extensionResult = Success(contract, WorldInspectionContract.ExtensionQueryName, query);
                foreach (var rawExtension in Hk03InspectionTests.List(extensionResult.Data!, "items"))
                {
                    var descriptor = (IReadOnlyDictionary<string, object?>)rawExtension!;
                    var owner = (string)descriptor["owner"]!;
                    var schemaVersion = Convert.ToInt32(descriptor["schemaVersion"], System.Globalization.CultureInfo.InvariantCulture);
                    var payloadLength = Convert.ToInt32(descriptor["payloadLength"], System.Globalization.CultureInfo.InvariantCulture);
                    WorldObjectId? subjectId = descriptor.TryGetValue("subjectId", out var rawSubject)
                        ? new WorldObjectId((string)rawSubject!)
                        : (WorldObjectId?)null;
                    var bytes = new List<byte>();
                    var offset = 0;
                    do
                    {
                        var readRequest = Anchor(source);
                        readRequest["owner"] = owner;
                        readRequest["schemaVersion"] = schemaVersion;
                        if (subjectId.HasValue) readRequest["subjectId"] = subjectId.Value.Value;
                        readRequest["offset"] = offset;
                        readRequest["limit"] = WorldInspectionService.MaximumExtensionChunkBytes;
                        var read = Success(contract, WorldInspectionContract.ExtensionReadName, readRequest);
                        bytes.AddRange(Convert.FromBase64String((string)read.Data!["payloadBase64"]!));
                        if (!read.Data.TryGetValue("nextOffset", out var nextOffset))
                        {
                            break;
                        }

                        offset = Convert.ToInt32(nextOffset, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    while (offset < payloadLength);

                    var dependencies = new List<WorldReference>();
                    var dependencyOffset = 0;
                    do
                    {
                        var dependencyRequest = Anchor(source);
                        dependencyRequest["owner"] = owner;
                        dependencyRequest["schemaVersion"] = schemaVersion;
                        if (subjectId.HasValue) dependencyRequest["subjectId"] = subjectId.Value.Value;
                        dependencyRequest["offset"] = payloadLength;
                        dependencyRequest["limit"] = WorldInspectionService.MaximumExtensionChunkBytes;
                        dependencyRequest["dependencyOffset"] = dependencyOffset;
                        dependencyRequest["dependencyLimit"] = WorldInspectionService.MaximumExtensionDependencyPageSize;
                        var dependencyRead = Success(contract, WorldInspectionContract.ExtensionReadName, dependencyRequest);
                        foreach (var rawDependency in Hk03InspectionTests.List(dependencyRead.Data!, "dependencies"))
                        {
                            var dependency = (IReadOnlyDictionary<string, object?>)rawDependency!;
                            dependencies.Add(new WorldReference(
                                new WorldReferenceKind((string)dependency["kind"]!),
                                new WorldObjectId((string)dependency["targetId"]!)));
                        }

                        if (!dependencyRead.Data.TryGetValue("nextDependencyOffset", out var nextDependencyOffset))
                        {
                            break;
                        }

                        dependencyOffset = Convert.ToInt32(
                            nextDependencyOffset,
                            System.Globalization.CultureInfo.InvariantCulture);
                    }
                    while (true);

                    extensions.Add(new WorldExtensionData(owner, schemaVersion, bytes.ToArray(), subjectId, dependencies));
                }

                cursor = extensionResult.Data!.TryGetValue("nextCursor", out var next)
                    ? (string)next!
                    : null;
            }
            while (cursor != null);

            return extensions;
        }

        private static void AssertCurrentSemanticSurfaceIsExplicitlyMapped()
        {
            var expected = new HashSet<string>(StringComparer.Ordinal)
            {
                "WorldState.SchemaVersion",
                "WorldState.Revision",
                "WorldState.Id",
                "WorldState.Objects",
                "WorldState.Extensions",
                "WorldObject.Id",
                "WorldObject.TypeId",
                "WorldObject.ContainerId",
                "WorldObject.References",
                "WorldReference.Kind",
                "WorldReference.TargetId",
                "WorldExtensionData.Identity",
                "WorldExtensionData.Owner",
                "WorldExtensionData.SchemaVersion",
                "WorldExtensionData.SubjectId",
                "WorldExtensionData.Dependencies",
                "WorldExtensionData.PayloadLength"
            };
            var actual = new HashSet<string>(StringComparer.Ordinal);
            AddPublicProperties(typeof(WorldState), actual);
            AddPublicProperties(typeof(WorldObject), actual);
            AddPublicProperties(typeof(WorldReference), actual);
            AddPublicProperties(typeof(WorldExtensionData), actual);

            Assert.True(expected.SetEquals(actual), "HK02 semantic surface changed; update the HK03 completeness map before freeze.");
        }

        private static void AddPublicProperties(Type type, ISet<string> target)
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                target.Add(type.Name + "." + property.Name);
            }
        }

        private static (string Id, string Cursor) Page(ComposedContract contract, WorldState state, int limit)
        {
            var request = Anchor(state);
            request["limit"] = limit;
            var result = Success(contract, WorldInspectionContract.ObjectQueryName, request);
            var first = (IReadOnlyDictionary<string, object?>)Hk03InspectionTests.List(result.Data!, "items")[0]!;
            return ((string)first["id"]!, (string)result.Data!["nextCursor"]!);
        }

        private static Dictionary<string, object?> Anchor(WorldState state) => Hk03InspectionTests.Anchor(state);

        private static ComposedContract Compose(WorldState state) => Hk03InspectionTests.Compose(state);

        private static ContractVersionRange ExactVersion() => ContractVersionRange.Exact(new ContractVersion(1, 0));

        private static CapabilityInvocationResult Success(
            ComposedContract contract,
            string capability,
            IReadOnlyDictionary<string, object?> request)
        {
            return Hk03InspectionTests.Dispatch(contract, capability, request);
        }

        private static string FormatIssues(IReadOnlyList<ConformanceIssue> issues)
        {
            var text = string.Empty;
            foreach (var issue in issues)
            {
                text += issue.Code + ":" + issue.Subject + ";";
            }

            return text;
        }

        private sealed class MutableWorldSource : IWorldStateSource
        {
            public MutableWorldSource(WorldState current)
            {
                Current = current;
            }

            public WorldState Current { get; set; }
        }
    }
}
