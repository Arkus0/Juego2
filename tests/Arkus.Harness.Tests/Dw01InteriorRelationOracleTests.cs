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
            Assert.Empty(ValidateRelationShape(slice.InteriorProjection, expectedInteriorIds));
        }

        [Fact]
        public void MissingAllocatesDepthRelationIsRedEvenWhenGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                Array.Empty<DesignRelation>());

            var errors = ValidateRelationShape(projection, new[] { "loc.alpha", "loc.beta" });

            Assert.Contains("relation-missing:loc.alpha", errors);
        }

        [Fact]
        public void RenamedAllocatesDepthRelationIsRedEvenWhenGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[] { new DesignRelation("allocates-interior", "depth.loc.alpha") });

            var errors = ValidateRelationShape(projection, new[] { "loc.alpha", "loc.beta" });

            Assert.Contains("relation-missing:loc.alpha", errors);
        }

        [Fact]
        public void WrongAllocatesDepthTargetIsRedEvenWhenGenericValidatorSelfConfirms()
        {
            var projection = BuildSyntheticProjection(
                "loc.alpha",
                new[] { new DesignRelation("allocates-depth", "depth.loc.beta") });

            var errors = ValidateRelationShape(projection, new[] { "loc.alpha", "loc.beta" });

            Assert.Contains("relation-target:loc.alpha", errors);
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

        private static IReadOnlyList<string> ValidateRelationShape(
            DesignWorldProjection interiors,
            IEnumerable<string> expectedInteriorIds)
        {
            var errors = new List<string>();
            var facts = interiors.Facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);

            foreach (var id in expectedInteriorIds.OrderBy(value => value, StringComparer.Ordinal))
            {
                var allocationId = "allocation." + id;
                var depthId = "depth." + id;
                if (!facts.TryGetValue(allocationId, out var allocation))
                {
                    errors.Add("allocation-missing:" + id);
                    continue;
                }

                if (!facts.ContainsKey(depthId))
                {
                    errors.Add("depth-missing:" + id);
                    continue;
                }

                var canonicalRelations = allocation.Relations
                    .Where(relation => StringComparer.Ordinal.Equals(relation.RelationType, "allocates-depth"))
                    .ToList();
                if (canonicalRelations.Count == 0)
                {
                    errors.Add("relation-missing:" + id);
                    continue;
                }

                if (canonicalRelations.Count != 1)
                {
                    errors.Add("relation-cardinality:" + id);
                    continue;
                }

                if (!StringComparer.Ordinal.Equals(canonicalRelations[0].TargetFactId, depthId))
                {
                    errors.Add("relation-target:" + id);
                }
            }

            return errors.AsReadOnly();
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
