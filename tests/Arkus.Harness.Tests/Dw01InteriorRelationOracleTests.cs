using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw01InteriorRelationOracleTests
    {
        private static readonly DesignProjectionVersion SyntheticVersion =
            new DesignProjectionVersion(1, "dw01-relation-oracle-v1");
        private static readonly string[] SyntheticExpectedInteriorIds = { "loc.alpha", "loc.beta" };

        [Fact]
        public void AcceptedCityProjectionHasExactlyOneCanonicalAllocationRelationPerCity02InteriorSubject()
        {
            var root = FindRepositoryRoot();
            var programme = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.ProgrammeSourcePath));
            var bindings = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.BindingSourcePath));
            var interiors = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.InteriorsSourcePath));

            var expectedInteriorIds = ParseCity02InteriorIds(programme);
            var slice = new CityDesignWorldProvider().BuildAndValidate(programme, bindings, interiors);

            Assert.Equal(expectedInteriorIds.Count, slice.InteriorPlaceCount);
            Assert.NotEmpty(expectedInteriorIds);
            new CityInteriorRelationOracle().Validate(slice.InteriorProjection, expectedInteriorIds);
        }

        [Fact]
        public void MissingAllocatesDepthRelationMakesCityOracleRedWhileGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                Array.Empty<DesignRelation>());

            var error = AssertRelationOracleRed(
                projection,
                "city.interior_allocation_relation_missing",
                "loc.alpha");

            Assert.Contains("'allocates-depth' -> 'depth.loc.alpha'", error.Detail, StringComparison.Ordinal);
            Assert.Contains("observed relations: <none>", error.Detail, StringComparison.Ordinal);
        }

        [Fact]
        public void RenamedAllocatesDepthRelationMakesCityOracleRedWhileGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[] { new DesignRelation("allocates-interior", "depth.loc.alpha") });

            var error = AssertRelationOracleRed(
                projection,
                "city.interior_allocation_relation_missing",
                "loc.alpha");

            Assert.Contains("'allocates-depth' -> 'depth.loc.alpha'", error.Detail, StringComparison.Ordinal);
            Assert.Contains("'allocates-interior' -> 'depth.loc.alpha'", error.Detail, StringComparison.Ordinal);
        }

        [Fact]
        public void WrongAllocatesDepthTargetMakesCityOracleRedWhileGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[] { new DesignRelation("allocates-depth", "depth.loc.beta") });

            var error = AssertRelationOracleRed(
                projection,
                "city.interior_allocation_relation_target_invalid",
                "loc.alpha");

            Assert.Contains("Expected 'allocates-depth' -> 'depth.loc.alpha'", error.Detail, StringComparison.Ordinal);
            Assert.Contains("observed 'allocates-depth' -> 'depth.loc.beta'", error.Detail, StringComparison.Ordinal);
        }

        [Fact]
        public void TwoCanonicalRelationsIncludingOneCorrectAndOneWrongAreCardinalityRed()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[]
                {
                    new DesignRelation("allocates-depth", "depth.loc.alpha"),
                    new DesignRelation("allocates-depth", "depth.loc.beta")
                });

            var error = AssertRelationOracleRed(
                projection,
                "city.interior_allocation_relation_cardinality",
                "loc.alpha");

            Assert.Contains("observed 2 canonical relations", error.Detail, StringComparison.Ordinal);
            Assert.Contains("'depth.loc.alpha'", error.Detail, StringComparison.Ordinal);
            Assert.Contains("'depth.loc.beta'", error.Detail, StringComparison.Ordinal);
        }

        [Fact]
        public void MissingAllocationProjectionIsRedAgainstIndependentExpectedSubjectSet()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[] { new DesignRelation("allocates-depth", "depth.loc.alpha") },
                new[] { "allocation.loc.alpha" });

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityInteriorRelationOracle().Validate(projection, SyntheticExpectedInteriorIds));

            Assert.Equal("city.interior_allocation_projection_missing", error.MachineCode);
            Assert.Equal("loc.alpha", error.SubjectId);
            Assert.Contains("allocation.loc.alpha", error.Detail, StringComparison.Ordinal);
            Assert.Contains("allocates-depth", error.Detail, StringComparison.Ordinal);
            Assert.Contains(CityDesignWorldProvider.ProgrammeSourcePath, error.SourcePaths);
            Assert.Contains(CityDesignWorldProvider.InteriorsSourcePath, error.SourcePaths);
        }

        [Fact]
        public void MissingDepthProjectionIsRedAgainstIndependentExpectedSubjectSet()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[] { new DesignRelation("allocates-depth", "depth.loc.beta") },
                new[] { "depth.loc.alpha" });

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityInteriorRelationOracle().Validate(projection, SyntheticExpectedInteriorIds));

            Assert.Equal("city.interior_depth_projection_missing", error.MachineCode);
            Assert.Equal("loc.alpha", error.SubjectId);
            Assert.Contains("depth.loc.alpha", error.Detail, StringComparison.Ordinal);
            Assert.Contains("allocates-depth", error.Detail, StringComparison.Ordinal);
            Assert.Contains(CityDesignWorldProvider.ProgrammeSourcePath, error.SourcePaths);
            Assert.Contains(CityDesignWorldProvider.InteriorsSourcePath, error.SourcePaths);
        }

        [Fact]
        public void DanglingRelationTargetFailsClosedInGenericProjectionBeforeCityOracle()
        {
            var error = Assert.Throws<DesignWorldProjectionException>(() =>
                BuildSyntheticProjection(
                    "loc.alpha",
                    new[] { new DesignRelation("allocates-depth", "depth.loc.missing") }));

            Assert.Equal("dw.relation_target_outside_universe", error.MachineCode);
        }

        private static CityInvariantException AssertRelationOracleRed(
            DesignWorldProjection projection,
            string expectedMachineCode,
            string expectedSubject)
        {
            var error = Assert.Throws<CityInvariantException>(() =>
                new CityInteriorRelationOracle().Validate(projection, SyntheticExpectedInteriorIds));

            Assert.Equal(expectedMachineCode, error.MachineCode);
            Assert.Equal(expectedSubject, error.SubjectId);
            Assert.Contains("allocates-depth", error.Rule, StringComparison.Ordinal);
            Assert.Contains("allocation." + expectedSubject, error.Detail, StringComparison.Ordinal);
            Assert.Contains(CityDesignWorldProvider.ProgrammeSourcePath, error.SourcePaths);
            Assert.Contains(CityDesignWorldProvider.InteriorsSourcePath, error.SourcePaths);
            return error;
        }

        private static DesignWorldProjection BuildSyntheticProjection(
            string mutatedSubject,
            IEnumerable<DesignRelation> mutatedRelations)
        {
            return BuildSyntheticProjection(
                mutatedSubject,
                mutatedRelations,
                Array.Empty<string>());
        }

        private static DesignWorldProjection BuildSyntheticProjection(
            string mutatedSubject,
            IEnumerable<DesignRelation> mutatedRelations,
            IEnumerable<string> omittedFactIds)
        {
            const string source =
                "depth-alpha-anchor\n" +
                "allocation-alpha-anchor\n" +
                "depth-beta-anchor\n" +
                "allocation-beta-anchor\n";

            var omitted = new HashSet<string>(omittedFactIds, StringComparer.Ordinal);
            var definitions = new List<AnchoredFactDefinition>();
            var universeIds = new List<string>();
            foreach (var id in SyntheticExpectedInteriorIds)
            {
                var suffix = id.Substring("loc.".Length);
                var depthFactId = "depth." + id;
                if (!omitted.Contains(depthFactId))
                {
                    definitions.Add(new AnchoredFactDefinition(
                        depthFactId,
                        "city-interior-depth",
                        "depth-" + suffix + "-anchor",
                        new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                        {
                            ["interior-depth"] = DesignValue.String("I1")
                        }));
                    universeIds.Add(depthFactId);
                }

                var allocationFactId = "allocation." + id;
                if (!omitted.Contains(allocationFactId))
                {
                    var relations = StringComparer.Ordinal.Equals(id, mutatedSubject)
                        ? mutatedRelations
                        : new[] { new DesignRelation("allocates-depth", depthFactId) };

                    definitions.Add(new AnchoredFactDefinition(
                        allocationFactId,
                        "city-interior-allocation",
                        "allocation-" + suffix + "-anchor",
                        new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                        {
                            ["allocated"] = DesignValue.Boolean(true)
                        },
                        relations));
                    universeIds.Add(allocationFactId);
                }
            }

            var universe = new StaticDesignAuthorityUniverse(universeIds);
            var reader = new AnchoredTextAuthorityReader(
                "dw01-relation-oracle-synthetic",
                CityDesignWorldProvider.InteriorsSourcePath,
                source,
                definitions);
            var projection = new DesignWorldProjector().Build(universe, reader, SyntheticVersion);

            var genericReport = new DesignWorldProjectionValidator().Validate(
                projection, universe, reader, SyntheticVersion);
            Assert.True(
                genericReport.IsValid,
                "The causal control must preserve the generic self-confirming path so only the independent CITY obligation set detects the defect.");

            return projection;
        }

        private static IReadOnlyList<string> ParseCity02InteriorIds(string programmeSource)
        {
            const string heading = "## 4. District × location programme";
            var lines = programmeSource.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var headingIndex = Array.FindIndex(lines, line => line.Trim() == heading);
            Assert.True(headingIndex >= 0, "CITY-02 programme heading not found.");

            var ids = new SortedSet<string>(StringComparer.Ordinal);
            var sawHeader = false;
            for (var index = headingIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (line.StartsWith("## ", StringComparison.Ordinal)) break;
                if (!line.StartsWith("|", StringComparison.Ordinal)) continue;

                var cells = line.Substring(1, line.Length - 2)
                    .Split('|')
                    .Select(cell => Clean(cell))
                    .ToArray();
                if (cells.Length != 10) continue;
                if (cells[0] == "Programme ID")
                {
                    sawHeader = true;
                    continue;
                }
                if (!sawHeader || cells[0].StartsWith("---", StringComparison.Ordinal)) continue;

                var depth = cells[7];
                if (depth.StartsWith("I1", StringComparison.Ordinal) ||
                    depth.StartsWith("I2", StringComparison.Ordinal) ||
                    depth.StartsWith("I3", StringComparison.Ordinal))
                {
                    Assert.True(ids.Add(cells[0]), "duplicate CITY-02 interior subject: " + cells[0]);
                }
            }

            Assert.NotEmpty(ids);
            return ids.ToList().AsReadOnly();
        }

        private static string Clean(string value) =>
            value.Replace("**", string.Empty).Replace("`", string.Empty).Trim();

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Juego2.sln"))) return directory.FullName;
                directory = directory.Parent;
            }
            throw new InvalidOperationException("Could not locate Juego2.sln from the test output directory.");
        }
    }
}
