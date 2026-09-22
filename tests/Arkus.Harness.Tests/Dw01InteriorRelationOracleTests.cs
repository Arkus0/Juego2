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

        [Fact]
        public void AcceptedCityProjectionHasExactlyOneCanonicalAllocationRelationPerCity02InteriorSubject()
        {
            var root = FindRepositoryRoot();
            var programme = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.ProgrammeSourcePath));
            var bindings = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.BindingSourcePath));
            var interiors = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.InteriorsSourcePath));

            var expectedInteriorIds = ParseCity02InteriorIds(programme);
            var slice = new CityDesignWorldProvider().BuildAndValidate(programme, bindings, interiors);

            Assert.Equal(14, expectedInteriorIds.Count);
            new CityInteriorRelationOracle().Validate(slice.InteriorProjection, expectedInteriorIds);
        }

        [Fact]
        public void MissingAllocatesDepthRelationMakesCityOracleRedWhileGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                Array.Empty<DesignRelation>());

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityInteriorRelationOracle().Validate(
                    projection,
                    new[] { "loc.alpha", "loc.beta" }));

            Assert.Equal("city.interior_allocation_relation_missing", error.MachineCode);
            Assert.Equal("loc.alpha", error.SubjectId);
            Assert.Contains("allocation.loc.alpha", error.Detail, StringComparison.Ordinal);
            Assert.Contains("depth.loc.alpha", error.Detail, StringComparison.Ordinal);
            Assert.Contains(CityDesignWorldProvider.InteriorsSourcePath, error.SourcePaths);
        }

        [Fact]
        public void RenamedAllocatesDepthRelationMakesCityOracleRedWhileGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[] { new DesignRelation("allocates-interior", "depth.loc.alpha") });

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityInteriorRelationOracle().Validate(
                    projection,
                    new[] { "loc.alpha", "loc.beta" }));

            Assert.Equal("city.interior_allocation_relation_missing", error.MachineCode);
            Assert.Equal("loc.alpha", error.SubjectId);
        }

        [Fact]
        public void WrongAllocatesDepthTargetMakesCityOracleRedWhileGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[] { new DesignRelation("allocates-depth", "depth.loc.beta") });

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityInteriorRelationOracle().Validate(
                    projection,
                    new[] { "loc.alpha", "loc.beta" }));

            Assert.Equal("city.interior_allocation_relation_target_invalid", error.MachineCode);
            Assert.Equal("loc.alpha", error.SubjectId);
            Assert.Contains("depth.loc.beta", error.Detail, StringComparison.Ordinal);
            Assert.Contains("depth.loc.alpha", error.Detail, StringComparison.Ordinal);
        }

        private static DesignWorldProjection BuildSyntheticProjection(
            string mutatedSubject,
            IEnumerable<DesignRelation> mutatedRelations)
        {
            const string source =
                "depth-alpha-anchor\n" +
                "allocation-alpha-anchor\n" +
                "depth-beta-anchor\n" +
                "allocation-beta-anchor\n";

            var definitions = new List<AnchoredFactDefinition>();
            foreach (var id in new[] { "loc.alpha", "loc.beta" })
            {
                definitions.Add(new AnchoredFactDefinition(
                    "depth." + id,
                    "city-interior-depth",
                    "depth-" + id.Substring("loc.".Length) + "-anchor",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["interior-depth"] = DesignValue.String("I1")
                    }));

                var relations = StringComparer.Ordinal.Equals(id, mutatedSubject)
                    ? mutatedRelations
                    : new[] { new DesignRelation("allocates-depth", "depth." + id) };

                definitions.Add(new AnchoredFactDefinition(
                    "allocation." + id,
                    "city-interior-allocation",
                    "allocation-" + id.Substring("loc.".Length) + "-anchor",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["allocated"] = DesignValue.Boolean(true)
                    },
                    relations));
            }

            var universe = new StaticDesignAuthorityUniverse(new[]
            {
                "depth.loc.alpha",
                "allocation.loc.alpha",
                "depth.loc.beta",
                "allocation.loc.beta"
            });
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
                "The causal control must preserve the generic self-confirming path so only the independent CITY oracle detects the defect.");

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
