using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw00DesignWorldProjectionTests
    {
        private static readonly DesignProjectionVersion VersionOne = new DesignProjectionVersion(1, "dw00-v1");

        [Fact]
        public void NeutralFixtureRebuildsDeterministicallyThroughPublicH0InspectionSurface()
        {
            var source = NeutralSource();
            var universe = NeutralUniverse();
            var reader = NeutralReader(source);
            var sourceDigestBefore = reader.SourceDigest;
            var projector = new DesignWorldProjector();

            var first = projector.Build(universe, reader, VersionOne);
            var rebuiltReader = NeutralReader(source);
            var second = projector.Build(universe, rebuiltReader, VersionOne);
            var sourceDigestAfter = rebuiltReader.SourceDigest;
            var validation = new DesignWorldProjectionValidator().Validate(first, universe, reader, VersionOne);

            Assert.True(validation.IsValid, string.Join("; ", validation.Issues.Select(issue => issue.MachineCode)));
            Assert.Equal(sourceDigestBefore, sourceDigestAfter);
            Assert.Equal(first.Digest, second.Digest);
            Assert.Equal(first.NormalizedRepresentation, second.NormalizedRepresentation);
            Assert.Equal(
                CanonicalWorldStateCodec.Serialize(first.WorldState),
                CanonicalWorldStateCodec.Serialize(second.WorldState));
            Assert.True(DesignWorldProjectionDiff.Compare(first, second).IsEmpty);

            var inspection = new WorldInspectionService(new FixedWorldStateSource(first.WorldState));
            var summary = inspection.Summary(new Dictionary<string, object?>(StringComparer.Ordinal));
            Assert.True(summary.Success);
            Assert.NotNull(summary.Data);
            Assert.Equal(3, Assert.IsType<int>(summary.Data!["objectCount"]));
            Assert.Equal(2, Assert.IsType<int>(summary.Data["referenceCount"]));
            Assert.Equal(3, Assert.IsType<int>(summary.Data["extensionCount"]));

            var query = inspection.QueryObjects(AnchoredRequest(first.WorldState));
            Assert.True(query.Success);
            Assert.NotNull(query.Data);
            var items = Assert.IsAssignableFrom<IReadOnlyList<object?>>(query.Data!["items"]);
            Assert.Equal(3, items.Count);

            var references = inspection.QueryReferences(AnchoredRequest(first.WorldState));
            Assert.True(references.Success);
            Assert.NotNull(references.Data);
            var referenceItems = Assert.IsAssignableFrom<IReadOnlyList<object?>>(references.Data!["items"]);
            Assert.Equal(2, referenceItems.Count);
            AssertReference(referenceItems[0], "root", "contains", "item-a");
            AssertReference(referenceItems[1], "root", "contains", "item-b");
        }

        [Fact]
        public void IndependentUniverseMakesMaterialProjectionOmissionCausallyRed()
        {
            var source = NeutralSource();
            var completeUniverse = NeutralUniverse();
            var incompleteReader = new AnchoredTextAuthorityReader(
                "neutral-authority",
                "fixture/neutral.txt",
                source,
                NeutralDefinitions().Where(definition => definition.FactId != "item-b"));

            var error = Assert.Throws<DesignWorldProjectionException>(() =>
                new DesignWorldProjector().Build(completeUniverse, incompleteReader, VersionOne));

            Assert.Equal("dw.provenance_missing", error.MachineCode);
        }

        [Fact]
        public void StaleAuthorityBytesInvalidatePreviouslyProjectedProvenance()
        {
            var source = NeutralSource();
            var universe = NeutralUniverse();
            var originalReader = NeutralReader(source);
            var projection = new DesignWorldProjector().Build(universe, originalReader, VersionOne);
            var changedSource = source.Replace("beta=20", "beta=21", StringComparison.Ordinal);
            var changedReader = NeutralReader(changedSource);

            var validation = new DesignWorldProjectionValidator().Validate(projection, universe, changedReader, VersionOne);

            Assert.False(validation.IsValid);
            Assert.Contains(validation.Issues, issue => issue.MachineCode == "dw.provenance_stale");
        }

        [Fact]
        public void AmbiguousAuthorityAnchorCannotValidateAsTrustworthy()
        {
            var source = NeutralSource();
            var universe = NeutralUniverse();
            var reader = NeutralReader(source);
            var projection = new DesignWorldProjector().Build(universe, reader, VersionOne);
            var ambiguousReader = NeutralReader(source + "item-a alpha=10\n");

            var validation = new DesignWorldProjectionValidator().Validate(projection, universe, ambiguousReader, VersionOne);

            Assert.False(validation.IsValid);
            Assert.Contains(validation.Issues, issue => issue.FactId == "item-a" && issue.MachineCode == "dw.provenance_ambiguous");
        }

        [Fact]
        public void ReusingDerivedStateAcrossProjectionRuleVersionIsCausallyRed()
        {
            var source = NeutralSource();
            var universe = NeutralUniverse();
            var reader = NeutralReader(source);
            var projection = new DesignWorldProjector().Build(universe, reader, VersionOne);
            var versionTwo = new DesignProjectionVersion(1, "dw00-v2");

            var validation = new DesignWorldProjectionValidator().Validate(projection, universe, reader, versionTwo);

            Assert.False(validation.IsValid);
            Assert.Contains(validation.Issues, issue => issue.MachineCode == "dw.projection_version_stale");
        }

        [Fact]
        public void MaterialAuthorityChangeChangesNormalizedProjectionAndDiff()
        {
            var source = NeutralSource();
            var universe = NeutralUniverse();
            var first = new DesignWorldProjector().Build(universe, NeutralReader(source), VersionOne);
            var changedDefinitions = NeutralDefinitions().Select(definition =>
            {
                if (definition.FactId != "item-b")
                {
                    return definition;
                }

                return new AnchoredFactDefinition(
                    "item-b",
                    "neutral-item",
                    "item-b beta=21",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["value"] = DesignValue.Integer(21)
                    });
            }).ToArray();
            var changedSource = source.Replace("item-b beta=20", "item-b beta=21", StringComparison.Ordinal);
            var changedReader = new AnchoredTextAuthorityReader(
                "neutral-authority",
                "fixture/neutral.txt",
                changedSource,
                changedDefinitions);
            var second = new DesignWorldProjector().Build(universe, changedReader, VersionOne);

            var diff = DesignWorldProjectionDiff.Compare(first, second);

            Assert.NotEqual(first.Digest, second.Digest);
            Assert.Contains("item-b", diff.ChangedFactIds);
            Assert.Empty(diff.AddedFactIds);
            Assert.Empty(diff.RemovedFactIds);
        }

        [Fact]
        public void DeletingAllDerivedStateStillRebuildsFromAuthorityOnly()
        {
            var source = NeutralSource();
            var universe = NeutralUniverse();
            var reader = NeutralReader(source);
            var projector = new DesignWorldProjector();
            var firstBytes = CanonicalWorldStateCodec.Serialize(projector.Build(universe, reader, VersionOne).WorldState);

            var rebuilt = projector.Build(universe, NeutralReader(source), VersionOne);
            var rebuiltBytes = CanonicalWorldStateCodec.Serialize(rebuilt.WorldState);

            Assert.Equal(firstBytes, rebuiltBytes);
        }

        [Fact]
        public void DesignWorldIsAConsumerAndCannotBecomeAH0Dependency()
        {
            var h0WorldAssembly = typeof(WorldState).Assembly;
            Assert.DoesNotContain(
                h0WorldAssembly.GetReferencedAssemblies(),
                reference => StringComparer.Ordinal.Equals(reference.Name, "Arkus.DesignWorld"));
            Assert.Contains(
                typeof(DesignWorldProjector).Assembly.GetReferencedAssemblies(),
                reference => StringComparer.Ordinal.Equals(reference.Name, "Arkus.Game.World"));
        }

        [Fact]
        public void RepresentativeJuego2CityShapeFitsGenericSurfaceWithoutChangingAuthorityBytes()
        {
            var repositoryRoot = FindRepositoryRoot();
            var sourcePath = Path.Combine(repositoryRoot, "Docs", "production", "CITY_LOCATION_PROGRAMME.md");
            var beforeBytes = File.ReadAllBytes(sourcePath);
            var sourceText = File.ReadAllText(sourcePath);
            var barAnchor = "| `loc.casco.bar` | Casco Viejo | bar / social house | `W.CASCO` | food, drink, social, information | **A** | **S4** | **I3** | public + service + semi-private layers | service/social use, meeting/witness potential, repeat interior use |";
            var definitions = new[]
            {
                new AnchoredFactDefinition(
                    "loc.casco.bar",
                    "programme-place",
                    barAnchor,
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["district"] = DesignValue.String("Casco Viejo"),
                        ["importance"] = DesignValue.String("A"),
                        ["spatial-depth"] = DesignValue.String("S4"),
                        ["interior-priority"] = DesignValue.String("I3")
                    },
                    new[] { new DesignRelation("mobility-anchor", "mobility.w.casco") }),
                new AnchoredFactDefinition(
                    "mobility.w.casco",
                    "programme-mobility-anchor",
                    barAnchor,
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["planning-id"] = DesignValue.String("W.CASCO")
                    })
            };
            var universe = new StaticDesignAuthorityUniverse(new[] { "loc.casco.bar", "mobility.w.casco" });
            var reader = new AnchoredTextAuthorityReader(
                "city-location-programme-v1",
                "Docs/production/CITY_LOCATION_PROGRAMME.md",
                sourceText,
                definitions);

            var projection = new DesignWorldProjector().Build(universe, reader, VersionOne);
            var validation = new DesignWorldProjectionValidator().Validate(projection, universe, reader, VersionOne);
            var inspection = new WorldInspectionService(new FixedWorldStateSource(projection.WorldState));
            var references = inspection.QueryReferences(AnchoredRequest(projection.WorldState));
            var afterBytes = File.ReadAllBytes(sourcePath);

            Assert.True(validation.IsValid, string.Join("; ", validation.Issues.Select(issue => issue.MachineCode)));
            Assert.True(references.Success);
            Assert.Equal(beforeBytes, afterBytes);
            Assert.Equal(2, projection.Facts.Count);
            Assert.All(projection.WorldState.Objects, item => Assert.Equal(DesignWorldProjector.GenericWorldType, item.TypeId.Value));
            Assert.All(projection.WorldState.Extensions, extension => Assert.Equal(DesignWorldProjector.ExtensionOwner, extension.Owner));
        }

        [Fact]
        public void DuplicateRepresentativeAnchorFailsClosedRatherThanTrustingAnIndexRow()
        {
            const string row = "| `loc.casco.bar` | Casco Viejo | bar / social house | `W.CASCO` |";
            var reader = new AnchoredTextAuthorityReader(
                "probe-authority",
                "probe.md",
                row + "\n" + row + "\n",
                new[]
                {
                    new AnchoredFactDefinition(
                        "loc.casco.bar",
                        "programme-place",
                        row,
                        new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                        {
                            ["importance"] = DesignValue.String("A")
                        })
                });
            var universe = new StaticDesignAuthorityUniverse(new[] { "loc.casco.bar" });

            var error = Assert.Throws<DesignWorldProjectionException>(() =>
                new DesignWorldProjector().Build(universe, reader, VersionOne));

            Assert.Equal("dw.provenance_ambiguous", error.MachineCode);
        }

        private static void AssertReference(object? raw, string sourceId, string kind, string targetId)
        {
            var row = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(raw);
            Assert.Equal(sourceId, Assert.IsType<string>(row["sourceId"]));
            Assert.Equal(kind, Assert.IsType<string>(row["kind"]));
            Assert.Equal(targetId, Assert.IsType<string>(row["targetId"]));
        }

        private static IReadOnlyDictionary<string, object?> AnchoredRequest(WorldState state)
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = state.Revision,
                ["hash"] = CanonicalWorldStateCodec.ComputeContentHash(state)
            };
        }

        private static StaticDesignAuthorityUniverse NeutralUniverse()
        {
            return new StaticDesignAuthorityUniverse(new[] { "root", "item-a", "item-b" });
        }

        private static AnchoredTextAuthorityReader NeutralReader(string source)
        {
            return new AnchoredTextAuthorityReader(
                "neutral-authority",
                "fixture/neutral.txt",
                source,
                NeutralDefinitions());
        }

        private static AnchoredFactDefinition[] NeutralDefinitions()
        {
            return new[]
            {
                new AnchoredFactDefinition(
                    "root",
                    "neutral-root",
                    "root catalogue",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["complete"] = DesignValue.Boolean(true)
                    },
                    new[]
                    {
                        new DesignRelation("contains", "item-a"),
                        new DesignRelation("contains", "item-b")
                    }),
                new AnchoredFactDefinition(
                    "item-a",
                    "neutral-item",
                    "item-a alpha=10",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["value"] = DesignValue.Integer(10)
                    }),
                new AnchoredFactDefinition(
                    "item-b",
                    "neutral-item",
                    "item-b beta=20",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["value"] = DesignValue.Integer(20)
                    })
            };
        }

        private static string NeutralSource()
        {
            return "root catalogue\nitem-a alpha=10\nitem-b beta=20\n";
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Juego2.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new InvalidOperationException("Could not locate Juego2.sln from the test output directory.");
        }
    }
}
