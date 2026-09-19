using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk02AObjectScopedExtensionTests
    {
        [Fact]
        public void CompositeIdentityAllowsSameOwnerVersionOnDifferentSubjects()
        {
            var state = ScopedWorld();

            Assert.Equal(2, state.Extensions.Count);
            Assert.NotEqual(state.Extensions[0].Identity, state.Extensions[1].Identity);
            Assert.NotEqual(state.Extensions[0].Identity.ResourceKey, state.Extensions[1].Identity.ResourceKey);
            WorldStateValidator.ValidateOrThrow(state);
        }

        [Fact]
        public void CanonicalCodecCoversSubjectAndDependenciesButIgnoresInputOrdering()
        {
            var first = ScopedWorld();
            var reordered = ScopedWorld(true);

            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(first),
                CanonicalWorldStateCodec.ComputeContentHash(reordered));

            var roundTripped = CanonicalWorldStateCodec.Deserialize(CanonicalWorldStateCodec.Serialize(first));
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(first),
                CanonicalWorldStateCodec.ComputeContentHash(roundTripped));
            Assert.Equal(new WorldObjectId("npc.ana"), roundTripped.Extensions[0].SubjectId);
            Assert.Equal(2, roundTripped.Extensions[0].Dependencies.Count);

            var subjectMutant = SingleExtensionWorld(
                new WorldObjectId("npc.bob"),
                new[] { Dependency("works_at", "building.shop") });
            var dependencyMutant = SingleExtensionWorld(
                new WorldObjectId("npc.ana"),
                new[] { Dependency("works_at", "plaza.main") });
            var baseline = SingleExtensionWorld(
                new WorldObjectId("npc.ana"),
                new[] { Dependency("works_at", "building.shop") });

            Assert.NotEqual(
                CanonicalWorldStateCodec.ComputeContentHash(baseline),
                CanonicalWorldStateCodec.ComputeContentHash(subjectMutant));
            Assert.NotEqual(
                CanonicalWorldStateCodec.ComputeContentHash(baseline),
                CanonicalWorldStateCodec.ComputeContentHash(dependencyMutant));
        }

        [Fact]
        public void InvalidSubjectDependencyAndDuplicateEdgesFailClosed()
        {
            var objects = TownObjects();

            Assert.Equal("world.dangling_extension_subject", Assert.Throws<WorldStateException>(() =>
                NewWorld(objects, new[]
                {
                    Extension("npc.missing", Dependency("works_at", "building.shop"))
                })).MachineCode);

            Assert.Equal("world.dangling_extension_dependency", Assert.Throws<WorldStateException>(() =>
                NewWorld(objects, new[]
                {
                    Extension("npc.ana", Dependency("works_at", "building.missing"))
                })).MachineCode);

            var duplicate = Dependency("works_at", "building.shop");
            Assert.Equal("world.duplicate_extension_dependency", Assert.Throws<WorldStateException>(() =>
                NewWorld(objects, new[]
                {
                    Extension("npc.ana", duplicate, duplicate)
                })).MachineCode);

            Assert.Equal("world.duplicate_extension", Assert.Throws<WorldStateException>(() =>
                NewWorld(objects, new[]
                {
                    Extension("npc.ana"),
                    Extension("npc.ana")
                })).MachineCode);
        }

        [Fact]
        public void InspectionExposesCompositeAddressAndReconstructsExactHash()
        {
            var source = ScopedWorld();
            var session = new TransactionalWorldAuthoringSession(source);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var queryRequest = Hk03InspectionTests.Anchor(source);
            queryRequest["filter"] = Hk03InspectionTests.ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["subjectIds"] = new List<object?> { "npc.ana" }.AsReadOnly()
            });
            var filtered = Hk04TransactionalMutationTests.Success(
                contract,
                WorldInspectionContract.ExtensionQueryName,
                queryRequest);
            var filteredRows = Hk04TransactionalMutationTests.List(filtered.Data!, "items");
            Assert.Single(filteredRows);
            var anaDescriptor = (IReadOnlyDictionary<string, object?>)filteredRows[0]!;
            Assert.Equal("object", anaDescriptor["scope"]);
            Assert.Equal("npc.ana", anaDescriptor["subjectId"]);
            Assert.Equal(2, anaDescriptor["dependencyCount"]);

            var reconstructed = ReadAllExtensions(contract, source);
            var copy = new WorldState(source.Id, source.Revision, source.Objects, reconstructed, source.SchemaVersion);
            Assert.Equal(
                CanonicalWorldStateCodec.ComputeContentHash(source),
                CanonicalWorldStateCodec.ComputeContentHash(copy));
        }

        [Fact]
        public void MutationAddressesSubjectAndReportsDependencyEffects()
        {
            var initial = Hk02TestFixtures.MicroWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk02a-put",
                PutExtension("future.profile", "node.child", "works_at", "node.root"),
                PutExtension("future.profile", "node.peer", "visits", "node.root"));

            var planResult = Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.PlanName, request);
            var changes = Hk04TransactionalMutationTests.List(
                Hk04TransactionalMutationTests.Map(planResult.Data!, "plan"),
                "changes");
            Assert.Equal(2, changes.Count);
            foreach (var rawChange in changes)
            {
                var change = (IReadOnlyDictionary<string, object?>)rawChange!;
                Assert.Contains("dependencies", Hk04TransactionalMutationTests.Strings(change, "fields"));
                Assert.Single(Hk04TransactionalMutationTests.Strings(change, "referencesAdded"));
            }

            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, request);
            Assert.Equal(4, session.Current.Extensions.Count);

            var remove = Hk04TransactionalMutationTests.Request(
                session.Current,
                "request.hk02a-remove",
                RemoveExtension("future.profile", "node.child"));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, remove);

            Assert.Contains(session.Current.Extensions, value =>
                value.Owner == "future.profile" && value.SubjectId == new WorldObjectId("node.peer"));
            Assert.DoesNotContain(session.Current.Extensions, value =>
                value.Owner == "future.profile" && value.SubjectId == new WorldObjectId("node.child"));
        }

        [Theory]
        [InlineData("building.shop", "world.dangling_extension_dependency")]
        [InlineData("npc.ana", "world.dangling_extension_subject")]
        public void RemovingObjectReferencedByExtensionCannotReachApply(string objectId, string sourceCode)
        {
            var initial = ScopedWorld();
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var beforeHash = CanonicalWorldStateCodec.ComputeContentHash(initial);
            var request = Hk04TransactionalMutationTests.Request(
                initial,
                "request.hk02a-dangling",
                Hk04TransactionalMutationTests.RemoveObject(objectId));

            var plan = contract.Dispatch(
                WorldMutationContract.PlanName,
                Hk04TransactionalMutationTests.ExactVersion(),
                request);
            var apply = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(),
                request);

            Assert.False(plan.Success);
            Assert.False(apply.Success);
            Assert.Equal("world.change.invalid_candidate", plan.Error!.MachineCode);
            Assert.Equal("world.change.invalid_candidate", apply.Error!.MachineCode);
            var validation = (IReadOnlyDictionary<string, object?>)plan.Error.Context["validation"]!;
            var diagnostics = (IReadOnlyList<object?>)validation["diagnostics"]!;
            Assert.Contains(diagnostics, value =>
                ((IReadOnlyDictionary<string, object?>)value!)["machineCode"] as string == sourceCode);
            Assert.Equal(initial.Revision, session.Current.Revision);
            Assert.Equal(beforeHash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GlobalAndDashSubjectHaveDistinctIdempotencyFingerprints(bool remove)
        {
            var objects = new List<WorldObject>(TownObjects())
            {
                new WorldObject(new WorldObjectId("-"), new WorldTypeId("fixture.npc"))
            };
            var initial = NewWorld(objects, remove
                ? new[]
                {
                    new WorldExtensionData("fixture.dash", 1, new byte[] { 0x41 }),
                    new WorldExtensionData("fixture.dash", 1, new byte[] { 0x41 }, new WorldObjectId("-"))
                }
                : Array.Empty<WorldExtensionData>());
            var session = new TransactionalWorldAuthoringSession(initial);
            var contract = Hk04TransactionalMutationTests.Compose(session);
            var global = Hk04TransactionalMutationTests.Request(
                initial, "request.hk02a-dash", DashOperation(remove, null));
            var scoped = Hk04TransactionalMutationTests.Request(
                initial, "request.hk02a-dash", DashOperation(remove, "-"));

            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, global);
            var hash = CanonicalWorldStateCodec.ComputeContentHash(session.Current);
            var revision = session.Current.Revision;
            var conflict = contract.Dispatch(WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.ExactVersion(), scoped);
            Assert.False(conflict.Success);
            Assert.Equal("world.change.idempotency_conflict", conflict.Error!.MachineCode);
            Assert.Equal(revision, session.Current.Revision);
            Assert.Equal(hash, CanonicalWorldStateCodec.ComputeContentHash(session.Current));
            Assert.Single(session.Current.Extensions);
            Assert.Equal(remove ? new WorldObjectId("-") : (WorldObjectId?)null,
                session.Current.Extensions[0].SubjectId);

            var distinctKey = Hk04TransactionalMutationTests.Request(
                session.Current, "request.hk02a-dash-distinct", DashOperation(remove, "-"));
            Hk04TransactionalMutationTests.Success(contract, WorldMutationContract.ApplyName, distinctKey);
            Assert.Equal(remove ? 0 : 2, session.Current.Extensions.Count);
        }

        [Fact]
        public void SecondDependencyPageIsRequiredForExactCanonicalReconstruction()
        {
            var objects = new List<WorldObject>(TownObjects());
            var dependencies = new List<WorldReference>();
            for (var index = 0; index <= WorldInspectionService.MaximumExtensionDependencyPageSize; index++)
            {
                var id = "target." + index.ToString("D3", System.Globalization.CultureInfo.InvariantCulture);
                objects.Add(new WorldObject(new WorldObjectId(id), new WorldTypeId("fixture.target")));
                dependencies.Add(Dependency("links_to", id));
            }
            var source = NewWorld(objects, new[]
            {
                new WorldExtensionData("fixture.many-dependencies", 1, new byte[] { 0x41 }, null, dependencies)
            });
            var contract = Hk04TransactionalMutationTests.Compose(new TransactionalWorldAuthoringSession(source));
            var request = Hk03InspectionTests.Anchor(source);
            request["owner"] = "fixture.many-dependencies";
            request["schemaVersion"] = 1;
            request["offset"] = 0;
            request["limit"] = WorldInspectionService.MaximumExtensionChunkBytes;
            request["dependencyOffset"] = 0;
            request["dependencyLimit"] = WorldInspectionService.MaximumExtensionDependencyPageSize;
            var first = Hk04TransactionalMutationTests.Success(
                contract, WorldInspectionContract.ExtensionReadName, request);
            Assert.Equal(WorldInspectionService.MaximumExtensionDependencyPageSize,
                Hk04TransactionalMutationTests.List(first.Data!, "dependencies").Count);
            Assert.Equal(WorldInspectionService.MaximumExtensionDependencyPageSize,
                Convert.ToInt32(first.Data!["nextDependencyOffset"], System.Globalization.CultureInfo.InvariantCulture));
            request["dependencyOffset"] = first.Data!["nextDependencyOffset"];
            var second = Hk04TransactionalMutationTests.Success(
                contract, WorldInspectionContract.ExtensionReadName, request);
            Assert.Single(Hk04TransactionalMutationTests.List(second.Data!, "dependencies"));
            Assert.False(second.Data!.ContainsKey("nextDependencyOffset"));

            var complete = ReadAllExtensions(contract, source);
            Assert.Single(complete);
            Assert.Equal(dependencies.Count, complete[0].Dependencies.Count);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(source),
                CanonicalWorldStateCodec.ComputeContentHash(
                    new WorldState(source.Id, source.Revision, source.Objects, complete, source.SchemaVersion)));
            var firstPageOnly = new[]
            {
                new WorldExtensionData("fixture.many-dependencies", 1, new byte[] { 0x41 }, null,
                    dependencies.GetRange(0, WorldInspectionService.MaximumExtensionDependencyPageSize))
            };
            Assert.NotEqual(CanonicalWorldStateCodec.ComputeContentHash(source),
                CanonicalWorldStateCodec.ComputeContentHash(
                    new WorldState(source.Id, source.Revision, source.Objects, firstPageOnly, source.SchemaVersion)));
        }

        private static IReadOnlyDictionary<string, object?> DashOperation(bool remove, string? subjectId)
        {
            var operation = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = remove ? "remove-extension" : "put-extension",
                ["owner"] = "fixture.dash",
                ["schemaVersion"] = 1
            };
            if (subjectId != null) operation["subjectId"] = subjectId;
            if (!remove) operation["payloadBase64"] = Convert.ToBase64String(new byte[] { 0x41 });
            return Hk03InspectionTests.ReadOnlyMap(operation);
        }

        [Fact]
        public void IndependentCoverageOracleDetectsOmittedDependencyEffects()
        {
            var before = SingleExtensionWorld(
                new WorldObjectId("npc.ana"),
                new[] { Dependency("works_at", "building.shop") });
            var after = new WorldState(
                before.Id,
                before.Revision + 1,
                before.Objects,
                new[]
                {
                    new WorldExtensionData(
                        "arkus.npc-profile",
                        1,
                        new byte[] { 0x41 },
                        new WorldObjectId("npc.ana"),
                        new[] { Dependency("works_at", "plaza.main") })
                });

            var missing = WorldMutationCoverage.FindMismatches(before, after, Array.Empty<WorldMutationChange>());
            Assert.Contains(missing, value => value.Contains("|field|dependencies", StringComparison.Ordinal));
            Assert.Contains(missing, value => value.Contains("|reference+|works_at->plaza.main", StringComparison.Ordinal));
            Assert.Contains(missing, value => value.Contains("|reference-|works_at->building.shop", StringComparison.Ordinal));
        }

        private static WorldState ScopedWorld(bool reverseInput = false)
        {
            var anaDependencies = new List<WorldReference>
            {
                Dependency("works_at", "building.shop"),
                Dependency("visits", "plaza.main")
            };
            var objects = new List<WorldObject>(TownObjects());
            if (reverseInput)
            {
                objects.Reverse();
                anaDependencies.Reverse();
            }

            var extensions = new List<WorldExtensionData>
            {
                new WorldExtensionData(
                    "arkus.npc-profile",
                    1,
                    new byte[] { 0x41, 0x4e, 0x41 },
                    new WorldObjectId("npc.ana"),
                    anaDependencies),
                new WorldExtensionData(
                    "arkus.npc-profile",
                    1,
                    new byte[] { 0x42, 0x4f, 0x42 },
                    new WorldObjectId("npc.bob"),
                    new[] { Dependency("visits", "plaza.main") })
            };
            if (reverseInput)
            {
                extensions.Reverse();
            }

            return NewWorld(objects, extensions);
        }

        private static WorldState SingleExtensionWorld(
            WorldObjectId subjectId,
            IReadOnlyList<WorldReference> dependencies)
        {
            return NewWorld(TownObjects(), new[]
            {
                new WorldExtensionData("arkus.npc-profile", 1, new byte[] { 0x41 }, subjectId, dependencies)
            });
        }

        private static WorldState NewWorld(
            IEnumerable<WorldObject> objects,
            IEnumerable<WorldExtensionData> extensions)
        {
            return new WorldState(new WorldId("world.potes"), 7, objects, extensions);
        }

        private static IReadOnlyList<WorldObject> TownObjects()
        {
            return new[]
            {
                new WorldObject(new WorldObjectId("npc.ana"), new WorldTypeId("fixture.npc")),
                new WorldObject(new WorldObjectId("npc.bob"), new WorldTypeId("fixture.npc")),
                new WorldObject(new WorldObjectId("building.shop"), new WorldTypeId("fixture.building")),
                new WorldObject(new WorldObjectId("plaza.main"), new WorldTypeId("fixture.place"))
            };
        }

        private static WorldExtensionData Extension(string subjectId, params WorldReference[] dependencies)
        {
            return new WorldExtensionData(
                "arkus.npc-profile",
                1,
                new byte[] { 0x41 },
                new WorldObjectId(subjectId),
                dependencies);
        }

        private static WorldReference Dependency(string kind, string targetId)
        {
            return new WorldReference(new WorldReferenceKind(kind), new WorldObjectId(targetId));
        }

        private static IReadOnlyDictionary<string, object?> PutExtension(
            string owner,
            string subjectId,
            string dependencyKind,
            string dependencyTarget)
        {
            return Hk03InspectionTests.ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "put-extension",
                ["owner"] = owner,
                ["schemaVersion"] = 1,
                ["subjectId"] = subjectId,
                ["dependencies"] = new List<object?>
                {
                    Hk04TransactionalMutationTests.Reference(dependencyKind, dependencyTarget)
                }.AsReadOnly(),
                ["payloadBase64"] = Convert.ToBase64String(new byte[] { 0x01 })
            });
        }

        private static IReadOnlyDictionary<string, object?> RemoveExtension(string owner, string subjectId)
        {
            return Hk03InspectionTests.ReadOnlyMap(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["kind"] = "remove-extension",
                ["owner"] = owner,
                ["schemaVersion"] = 1,
                ["subjectId"] = subjectId
            });
        }

        private static IReadOnlyList<WorldExtensionData> ReadAllExtensions(
            ComposedContract contract,
            WorldState source)
        {
            var query = Hk03InspectionTests.Anchor(source);
            query["limit"] = WorldInspectionService.MaximumPageSize;
            var result = Hk04TransactionalMutationTests.Success(
                contract,
                WorldInspectionContract.ExtensionQueryName,
                query);
            var values = new List<WorldExtensionData>();
            foreach (var rawDescriptor in Hk04TransactionalMutationTests.List(result.Data!, "items"))
            {
                var descriptor = (IReadOnlyDictionary<string, object?>)rawDescriptor!;
                var request = Hk03InspectionTests.Anchor(source);
                request["owner"] = descriptor["owner"];
                request["schemaVersion"] = descriptor["schemaVersion"];
                if (descriptor.TryGetValue("subjectId", out var subject)) request["subjectId"] = subject;
                request["offset"] = 0;
                request["limit"] = WorldInspectionService.MaximumExtensionChunkBytes;
                request["dependencyOffset"] = 0;
                request["dependencyLimit"] = WorldInspectionService.MaximumExtensionDependencyPageSize;
                var dependencies = new List<WorldReference>();
                CapabilityInvocationResult read;
                do
                {
                    read = Hk04TransactionalMutationTests.Success(
                        contract, WorldInspectionContract.ExtensionReadName, request);
                    foreach (var rawDependency in Hk04TransactionalMutationTests.List(read.Data!, "dependencies"))
                    {
                        var dependency = (IReadOnlyDictionary<string, object?>)rawDependency!;
                        dependencies.Add(Dependency((string)dependency["kind"]!, (string)dependency["targetId"]!));
                    }
                    if (!read.Data!.TryGetValue("nextDependencyOffset", out var nextOffset)) break;
                    request["dependencyOffset"] = nextOffset;
                }
                while (true);

                var subjectId = read.Data!.TryGetValue("subjectId", out var rawSubject)
                    ? new WorldObjectId((string)rawSubject!)
                    : (WorldObjectId?)null;
                values.Add(new WorldExtensionData(
                    (string)read.Data!["owner"]!,
                    Convert.ToInt32(read.Data!["schemaVersion"], System.Globalization.CultureInfo.InvariantCulture),
                    Convert.FromBase64String((string)read.Data!["payloadBase64"]!),
                    subjectId,
                    dependencies));
            }

            return values.AsReadOnly();
        }
    }
}
