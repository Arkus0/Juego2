using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk06BNegativeConformanceTests
    {
        [Fact]
        public void IndependentOracleCoversExtensionExistenceAndTurnsRedForOmissionOrFalseExtra()
        {
            var baseline = Hk02TestFixtures.MicroWorld();
            var added = AddExtension(baseline);
            var removed = RemoveExtension(baseline, "future.alpha@2@global");

            AssertExactSemanticResources(baseline, added, "world.extension:future.gamma@3@global");
            AssertExactSemanticResources(baseline, removed, "world.extension:future.alpha@2@global");

            var expectedAdded = IndependentChangedResources(baseline, added);
            Assert.Equal(new[] { "world.extension:future.gamma@3@global" }, expectedAdded);

            var omissionMutant = Array.Empty<string>();
            Assert.Contains(
                "missing:world.extension:future.gamma@3@global",
                SemanticCoverageIssues(expectedAdded, omissionMutant));

            var falseExtraMutant = new[]
            {
                "world.extension:future.gamma@3@global",
                "world.object:invented"
            };
            Assert.Contains(
                "extra:world.object:invented",
                SemanticCoverageIssues(expectedAdded, falseExtraMutant));
        }

        private static void AssertExactSemanticResources(
            WorldState before,
            WorldState after,
            string expectedResource)
        {
            var session = new PortableWorldAuthoringSession(before);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var result = contract.Dispatch(
                WorldPortabilityContract.CompareName,
                Hk04TransactionalMutationTests.ExactVersion(),
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["base"] = Export(before),
                    ["target"] = Export(after)
                });

            Assert.True(result.Success, result.Error?.Message);
            var effective = Resources(result.Data!);
            var independent = IndependentChangedResources(before, after);
            Assert.Equal(new[] { expectedResource }, independent);
            Assert.Equal(independent, effective);
            Assert.Empty(SemanticCoverageIssues(independent, effective));
        }

        private static IReadOnlyDictionary<string, object?> Export(WorldState state)
        {
            var result = new PortableWorldAuthoringSession(state)
                .ExportSnapshot(Hk01TestFixtures.EmptyRequest());
            Assert.True(result.Success, result.Error?.Message);
            return result.Data!;
        }

        private static WorldState AddExtension(WorldState source)
        {
            var extensions = new List<WorldExtensionData>(source.Extensions)
            {
                new WorldExtensionData("future.gamma", 3, new byte[] { 0x21, 0x22 })
            };
            return new WorldState(source.Id, source.Revision, source.Objects, extensions, source.SchemaVersion);
        }

        private static WorldState RemoveExtension(WorldState source, string resourceKey)
        {
            var extensions = new List<WorldExtensionData>();
            foreach (var extension in source.Extensions)
            {
                if (!string.Equals(extension.Identity.ResourceKey, resourceKey, StringComparison.Ordinal))
                    extensions.Add(extension);
            }
            return new WorldState(source.Id, source.Revision, source.Objects, extensions, source.SchemaVersion);
        }

        private static IReadOnlyList<string> IndependentChangedResources(WorldState before, WorldState after)
        {
            var result = new SortedSet<string>(StringComparer.Ordinal);
            var beforeObjects = ObjectProjection(before);
            var afterObjects = ObjectProjection(after);
            var keys = new SortedSet<string>(beforeObjects.Keys, StringComparer.Ordinal);
            keys.UnionWith(afterObjects.Keys);
            foreach (var key in keys)
            {
                beforeObjects.TryGetValue(key, out var left);
                afterObjects.TryGetValue(key, out var right);
                if (!string.Equals(left, right, StringComparison.Ordinal))
                    result.Add("world.object:" + key);
            }

            var beforeExtensions = ExtensionProjection(before);
            var afterExtensions = ExtensionProjection(after);
            keys = new SortedSet<string>(beforeExtensions.Keys, StringComparer.Ordinal);
            keys.UnionWith(afterExtensions.Keys);
            foreach (var key in keys)
            {
                beforeExtensions.TryGetValue(key, out var left);
                afterExtensions.TryGetValue(key, out var right);
                if (!string.Equals(left, right, StringComparison.Ordinal))
                    result.Add("world.extension:" + key);
            }

            return new List<string>(result).AsReadOnly();
        }

        private static IReadOnlyList<string> SemanticCoverageIssues(
            IReadOnlyList<string> expected,
            IReadOnlyList<string> observed)
        {
            var expectedSet = new HashSet<string>(expected, StringComparer.Ordinal);
            var observedSet = new HashSet<string>(observed, StringComparer.Ordinal);
            var issues = new List<string>();
            foreach (var resource in expectedSet)
                if (!observedSet.Contains(resource)) issues.Add("missing:" + resource);
            foreach (var resource in observedSet)
                if (!expectedSet.Contains(resource)) issues.Add("extra:" + resource);
            issues.Sort(StringComparer.Ordinal);
            return issues.AsReadOnly();
        }

        private static IReadOnlyList<string> Resources(IReadOnlyDictionary<string, object?> diff)
        {
            var result = new List<string>();
            foreach (var value in Hk04TransactionalMutationTests.List(diff, "changes"))
            {
                var change = (IReadOnlyDictionary<string, object?>)value!;
                result.Add((string)change["resource"]!);
            }
            return result.AsReadOnly();
        }

        private static Dictionary<string, string> ObjectProjection(WorldState state)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var value in state.Objects)
            {
                var references = new List<string>();
                foreach (var reference in value.References)
                    references.Add(reference.Kind.Value + "->" + reference.TargetId.Value);
                references.Sort(StringComparer.Ordinal);
                result.Add(
                    value.Id.Value,
                    value.TypeId.Value + "|" +
                    (value.ContainerId.HasValue ? value.ContainerId.Value.Value : "-") + "|" +
                    string.Join(",", references));
            }
            return result;
        }

        private static Dictionary<string, string> ExtensionProjection(WorldState state)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var value in state.Extensions)
            {
                var dependencies = new List<string>();
                foreach (var dependency in value.Dependencies)
                    dependencies.Add(dependency.Kind.Value + "->" + dependency.TargetId.Value);
                dependencies.Sort(StringComparer.Ordinal);
                result.Add(
                    value.Identity.ResourceKey,
                    Convert.ToBase64String(value.GetPayloadCopy()) + "|" + string.Join(",", dependencies));
            }
            return result;
        }
    }
}
