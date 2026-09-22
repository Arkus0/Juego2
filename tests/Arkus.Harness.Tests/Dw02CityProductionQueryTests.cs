using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw02CityProductionQueryTests
    {
        private static readonly string[] Roles = { "private", "public", "semi-private", "service" };
        private static readonly string[] ImportanceValues = { "A", "B", "C", "D" };
        private static readonly string[] SpatialDepthValues = { "S0", "S1", "S2", "S3", "S4" };
        private static readonly string[] InteriorDepthValues = { "I0", "I1", "I2", "I3" };

        [Fact]
        public void AcceptedCity02LedgerAnswersFrozenQuerySuiteAgainstIndependentSourceOracle()
        {
            var source = ReadProgramme();
            var expected = ParseSourceIndependently(source);
            var dataset = new CityProductionQueryProvider().BuildAndValidate(source);

            Assert.Equal(expected.Count, dataset.Projection.Facts.Count);
            Assert.Equal(expected.Keys, dataset.Queries.AllSubjects.Select(result => result.FactId));
            Assert.All(dataset.Queries.AllSubjects, result =>
            {
                Assert.Equal(CityProductionQueryProvider.ProgrammeSourcePath, result.Provenance.SourcePath);
                Assert.False(string.IsNullOrWhiteSpace(result.Provenance.Anchor));
                Assert.False(string.IsNullOrWhiteSpace(result.Provenance.SourceDigest));
                Assert.False(string.IsNullOrWhiteSpace(result.Provenance.AnchorDigest));
            });

            var expectedPois = expected.Values
                .Where(row => row.Kind == "poi")
                .OrderBy(row => row.District, StringComparer.Ordinal)
                .ThenBy(row => row.Id, StringComparer.Ordinal)
                .Select(row => row.District + "|" + row.Id)
                .ToList();
            var actualPois = dataset.Queries.DeclaredPoisByDistrict()
                .SelectMany(bucket => bucket.Results.Select(result => bucket.District + "|" + result.FactId))
                .ToList();
            Assert.Equal(expectedPois, actualPois);

            foreach (var role in Roles)
            {
                var expectedIds = expected.Values
                    .Where(row => row.Roles.Contains(role))
                    .Select(row => row.Id)
                    .OrderBy(id => id, StringComparer.Ordinal)
                    .ToList();
                Assert.Equal(
                    expectedIds,
                    dataset.Queries.SubjectsRequiringAccessRole(role).Select(result => result.FactId).ToList());
            }

            var expectedHigherDepth = expected.Values
                .Where(row => row.InteriorDepth == "I2" || row.InteriorDepth == "I3")
                .OrderBy(row => row.InteriorDepth == "I3" ? 0 : 1)
                .ThenBy(row => row.Id, StringComparer.Ordinal)
                .Select(row => row.InteriorDepth + "|" + row.Id)
                .ToList();
            var actualHigherDepth = dataset.Queries.HigherDepthInteriors()
                .Select(result => result.InteriorDepth + "|" + result.FactId)
                .ToList();
            Assert.Equal(expectedHigherDepth, actualHigherDepth);

            AssertReportMatchesIndependentSource(expected, dataset.Queries.BuildContentShapeReport());
        }

        [Fact]
        public void ManifestDeclaresExactReviewedSourceColumnsForFrozenSuite()
        {
            Assert.Equal("dw02-city-production-manifest-v1", CityProductionProjectionManifest.ManifestId);
            Assert.All(CityProductionProjectionManifest.Fields, field =>
                Assert.Equal(CityProductionQueryProvider.ProgrammeSourcePath, field.SourcePath));
            Assert.Equal(
                new[]
                {
                    "Programme ID",
                    "District/family",
                    "Place / family",
                    "A–D",
                    "S",
                    "Interior",
                    "Default access posture"
                },
                CityProductionProjectionManifest.Fields.Select(field => field.Column).ToArray());
        }

        [Fact]
        public void OmittingProjectedSubjectCannotShrinkCompleteSourceOwnedQueryUniverse()
        {
            var source = ReadProgramme();
            var expected = ParseSourceIndependently(source);
            var dataset = new CityProductionQueryProvider().BuildAndValidate(source);
            var omitted = expected.Keys.First(id => id.StartsWith("loc.", StringComparison.Ordinal));
            var reducedFacts = dataset.Projection.Facts.Where(fact => fact.FactId != omitted).ToList();

            var error = Assert.Throws<CityProductionQueryException>(() =>
                new CityProductionQueryService(reducedFacts, expected.Keys));

            Assert.Equal("city.query_subject_missing", error.MachineCode);
            Assert.Equal(omitted, error.SubjectId);
            Assert.Contains(CityProductionQueryProvider.ProgrammeSourcePath, error.SourcePaths);
        }

        [Fact]
        public void DroppingEntireProgrammeFamilyClassCannotSilentlyUndercountReport()
        {
            var source = ReadProgramme();
            var expected = ParseSourceIndependently(source);
            var dataset = new CityProductionQueryProvider().BuildAndValidate(source);
            var reducedFacts = dataset.Projection.Facts
                .Where(fact => !fact.FactId.StartsWith("fam.", StringComparison.Ordinal))
                .ToList();

            var error = Assert.Throws<CityProductionQueryException>(() =>
                new CityProductionQueryService(reducedFacts, expected.Keys));

            Assert.Equal("city.query_subject_missing", error.MachineCode);
            Assert.StartsWith("fam.", error.SubjectId, StringComparison.Ordinal);
        }

        [Fact]
        public void ChangedAcceptedSpatialClassificationMovesSubjectBetweenReportBucketsOnRebuild()
        {
            var source = ReadProgramme();
            var baseline = new CityProductionQueryProvider().BuildAndValidate(source);
            var changed = MutateProgrammeRow(
                source,
                "`loc.casco.bar`",
                line => line.Replace("| **S4** |", "| **S3** |"));
            Assert.NotEqual(source, changed);

            var rebuilt = new CityProductionQueryProvider().BuildAndValidate(changed);
            var before = baseline.Queries.BuildContentShapeReport();
            var after = rebuilt.Queries.BuildContentShapeReport();
            var changedSubject = Assert.Single(rebuilt.Queries.AllSubjects.Where(result => result.FactId == "loc.casco.bar"));

            Assert.Equal("S3", changedSubject.SpatialDepth);
            Assert.Equal(before.SpatialDepthCounts["S4"] - 1, after.SpatialDepthCounts["S4"]);
            Assert.Equal(before.SpatialDepthCounts["S3"] + 1, after.SpatialDepthCounts["S3"]);
            Assert.Equal(before.ImportanceBySpatialDepthCounts["A|S4"] - 1, after.ImportanceBySpatialDepthCounts["A|S4"]);
            Assert.Equal(before.ImportanceBySpatialDepthCounts["A|S3"] + 1, after.ImportanceBySpatialDepthCounts["A|S3"]);
            Assert.NotEqual(baseline.Queries.BuildNormalizedSnapshot(), rebuilt.Queries.BuildNormalizedSnapshot());
        }

        [Fact]
        public void CorrectValuesWithWrongSourceProvenanceAreRejected()
        {
            var source = ReadProgramme();
            var expected = ParseSourceIndependently(source);
            var dataset = new CityProductionQueryProvider().BuildAndValidate(source);
            var original = dataset.Projection.Facts[0];
            var wrongProvenance = new DesignAuthorityAnchor(
                original.Provenance.AuthorityId,
                "Docs/production/NOT_CITY_02.md",
                original.Provenance.Anchor,
                original.Provenance.SourceDigest,
                original.Provenance.AnchorDigest);
            var replacement = new DesignFact(
                original.FactId,
                original.FactType,
                original.Fields,
                original.Relations,
                wrongProvenance);
            var mutatedFacts = dataset.Projection.Facts
                .Select(fact => fact.FactId == original.FactId ? replacement : fact)
                .ToList();

            var error = Assert.Throws<CityProductionQueryException>(() =>
                new CityProductionQueryService(mutatedFacts, expected.Keys));

            Assert.Equal("city.query_provenance_invalid", error.MachineCode);
            Assert.Equal(original.FactId, error.SubjectId);
        }

        [Fact]
        public void EqualSourceAndSchemaRebuildToEqualProjectionAndNormalizedQueryOutput()
        {
            var source = ReadProgramme();
            var first = new CityProductionQueryProvider().BuildAndValidate(source);
            var rebuilt = new CityProductionQueryProvider().BuildAndValidate(source);
            var reversed = new CityProductionQueryProvider(reverseEnumeration: true).BuildAndValidate(source);

            Assert.Equal(first.Projection.Digest, rebuilt.Projection.Digest);
            Assert.Equal(first.Projection.Digest, reversed.Projection.Digest);
            Assert.Equal(first.Projection.NormalizedRepresentation, rebuilt.Projection.NormalizedRepresentation);
            Assert.Equal(first.Projection.NormalizedRepresentation, reversed.Projection.NormalizedRepresentation);
            Assert.Equal(first.Queries.BuildNormalizedSnapshot(), rebuilt.Queries.BuildNormalizedSnapshot());
            Assert.Equal(first.Queries.BuildNormalizedSnapshot(), reversed.Queries.BuildNormalizedSnapshot());
        }

        [Fact]
        public void ChangedAuthorityBytesInvalidateCachedProductionProjectionProvenance()
        {
            var source = ReadProgramme();
            var provider = new CityProductionQueryProvider();
            var dataset = provider.BuildAndValidate(source);
            var changed = source + "\n<!-- DW-02 provenance mutation -->\n";

            var issues = provider.ValidateProvenanceAgainstSource(dataset, changed);

            Assert.Contains(issues, issue => issue.MachineCode == "dw.provenance_stale");
        }

        [Fact]
        public void ContentShapeReportKeepsCostExplicitlyUnmodeledRatherThanFabricatingZero()
        {
            var dataset = new CityProductionQueryProvider().BuildAndValidate(ReadProgramme());
            var report = dataset.Queries.BuildContentShapeReport();
            var normalized = report.ToNormalizedText();

            Assert.Equal("UNMODELED", report.CostModelStatus);
            Assert.Contains("cost-model=UNMODELED", normalized, StringComparison.Ordinal);
            Assert.DoesNotContain("cost-model=0", normalized, StringComparison.Ordinal);
            Assert.DoesNotContain("hours=", normalized, StringComparison.Ordinal);
            Assert.DoesNotContain("euros=", normalized, StringComparison.Ordinal);
            Assert.DoesNotContain("asset-effort=", normalized, StringComparison.Ordinal);
        }

        [Fact]
        public void EveryProjectedFactRemainsGenericH0WorldStateWithExactRowProvenance()
        {
            var source = ReadProgramme();
            var expected = ParseSourceIndependently(source);
            var dataset = new CityProductionQueryProvider().BuildAndValidate(source);
            var facts = dataset.Projection.Facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);

            Assert.All(dataset.Projection.WorldState.Objects, item =>
                Assert.Equal(DesignWorldProjector.GenericWorldType, item.TypeId.Value));
            foreach (var row in expected.Values)
            {
                Assert.Equal(row.Anchor, facts[row.Id].Provenance.Anchor);
            }
        }

        private static void AssertReportMatchesIndependentSource(
            SortedDictionary<string, ExpectedRow> expected,
            CityContentShapeReport report)
        {
            Assert.Equal(expected.Count, report.TotalSubjects);
            Assert.Equal(expected.Values.Count(row => row.Kind == "poi"), report.DeclaredPoiCount);
            Assert.Equal(expected.Values.Count(row => row.Kind == "family"), report.ProgrammeFamilyCount);

            AssertMapEqual(CountBy(expected.Values.Select(row => row.District)), report.DistrictCounts);
            AssertMapEqual(CountBy(expected.Values.Select(row => row.Importance), ImportanceValues), report.ImportanceCounts);
            AssertMapEqual(CountBy(expected.Values.Select(row => row.SpatialDepth), SpatialDepthValues), report.SpatialDepthCounts);
            AssertMapEqual(CountBy(expected.Values.Select(row => row.InteriorDepth), InteriorDepthValues), report.InteriorDepthCounts);

            var crossKeys = ImportanceValues.SelectMany(importance =>
                SpatialDepthValues.Select(spatial => importance + "|" + spatial)).ToArray();
            AssertMapEqual(
                CountBy(expected.Values.Select(row => row.Importance + "|" + row.SpatialDepth), crossKeys),
                report.ImportanceBySpatialDepthCounts);

            var expectedRoles = new SortedDictionary<string, int>(StringComparer.Ordinal);
            foreach (var role in Roles)
            {
                expectedRoles.Add(role, expected.Values.Count(row => row.Roles.Contains(role)));
            }
            AssertMapEqual(expectedRoles, report.AccessRoleCounts);
        }

        private static void AssertMapEqual(
            IReadOnlyDictionary<string, int> expected,
            IReadOnlyDictionary<string, int> actual)
        {
            Assert.Equal(expected.Keys.OrderBy(key => key, StringComparer.Ordinal), actual.Keys.OrderBy(key => key, StringComparer.Ordinal));
            foreach (var pair in expected)
            {
                Assert.True(actual.ContainsKey(pair.Key), "Missing report bucket: " + pair.Key);
                Assert.Equal(pair.Value, actual[pair.Key]);
            }
        }

        private static SortedDictionary<string, int> CountBy(IEnumerable<string> values, IEnumerable<string>? seededKeys = null)
        {
            var result = new SortedDictionary<string, int>(StringComparer.Ordinal);
            foreach (var key in seededKeys ?? Array.Empty<string>())
            {
                result[key] = 0;
            }
            foreach (var value in values)
            {
                result[value] = result.TryGetValue(value, out var count) ? count + 1 : 1;
            }
            return result;
        }

        private static SortedDictionary<string, ExpectedRow> ParseSourceIndependently(string source)
        {
            const string heading = "## 4. District × location programme";
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var headingIndex = Array.FindIndex(lines, line => line.Trim() == heading);
            Assert.True(headingIndex >= 0, "Independent oracle could not find CITY-02 programme heading.");

            var headerIndex = -1;
            for (var index = headingIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (!line.StartsWith("|", StringComparison.Ordinal)) continue;
                var cells = SplitIndependent(line);
                if (cells.Count == 10 && CleanIndependent(cells[0]) == "Programme ID")
                {
                    headerIndex = index;
                    break;
                }
            }
            Assert.True(headerIndex >= 0, "Independent oracle could not find CITY-02 programme table.");

            var result = new SortedDictionary<string, ExpectedRow>(StringComparer.Ordinal);
            for (var index = headerIndex + 2; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (!line.StartsWith("|", StringComparison.Ordinal)) break;
                var cells = SplitIndependent(line);
                Assert.Equal(10, cells.Count);
                var id = CleanIndependent(cells[0]);
                var kind = id.StartsWith("loc.", StringComparison.Ordinal) ? "poi" :
                    id.StartsWith("fam.", StringComparison.Ordinal) ? "family" : "unknown";
                var row = new ExpectedRow(
                    id,
                    CleanIndependent(cells[1]),
                    CleanIndependent(cells[5]),
                    CleanIndependent(cells[6]),
                    IndependentInterior(CleanIndependent(cells[7])),
                    kind,
                    IndependentRoles(cells[8]),
                    line);
                Assert.True(result.TryAdd(id, row), "Independent oracle found duplicate CITY-02 id: " + id);
            }

            Assert.NotEmpty(result);
            return result;
        }

        private static IReadOnlyList<string> SplitIndependent(string line)
        {
            Assert.True(line.Length >= 2 && line[0] == '|' && line[line.Length - 1] == '|');
            return line.Substring(1, line.Length - 2).Split('|').Select(cell => cell.Trim()).ToList();
        }

        private static string CleanIndependent(string value) =>
            value.Replace("**", string.Empty).Replace("`", string.Empty).Trim();

        private static string IndependentInterior(string value)
        {
            foreach (var depth in InteriorDepthValues)
            {
                if (value == depth || value.StartsWith(depth + " ", StringComparison.Ordinal)) return depth;
            }
            throw new InvalidOperationException("Independent oracle encountered unknown interior depth: " + value);
        }

        private static SortedSet<string> IndependentRoles(string raw)
        {
            var text = CleanIndependent(raw).ToLowerInvariant().Replace("conditional public", string.Empty);
            var roles = new SortedSet<string>(StringComparer.Ordinal);
            if (text.Contains("semi-private"))
            {
                roles.Add("semi-private");
                text = text.Replace("semi-private", string.Empty);
            }
            if (text.Contains("public")) roles.Add("public");
            if (text.Contains("service")) roles.Add("service");
            if (text.Contains("private")) roles.Add("private");
            return roles;
        }

        private static string MutateProgrammeRow(string source, string idToken, Func<string, string> mutate)
        {
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var changed = false;
            for (var index = 0; index < lines.Length; index++)
            {
                if (!lines[index].Contains(idToken, StringComparison.Ordinal)) continue;
                var replacement = mutate(lines[index]);
                if (!StringComparer.Ordinal.Equals(lines[index], replacement))
                {
                    lines[index] = replacement;
                    changed = true;
                    break;
                }
            }
            Assert.True(changed, "Expected independent source mutation did not change a row.");
            return string.Join("\n", lines);
        }

        private static string ReadProgramme()
        {
            var root = FindRepositoryRoot();
            return File.ReadAllText(Path.Combine(
                root,
                CityProductionQueryProvider.ProgrammeSourcePath.Replace('/', Path.DirectorySeparatorChar)));
        }

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

        private sealed class ExpectedRow
        {
            public ExpectedRow(
                string id,
                string district,
                string importance,
                string spatialDepth,
                string interiorDepth,
                string kind,
                SortedSet<string> roles,
                string anchor)
            {
                Id = id;
                District = district;
                Importance = importance;
                SpatialDepth = spatialDepth;
                InteriorDepth = interiorDepth;
                Kind = kind;
                Roles = roles;
                Anchor = anchor;
            }

            public string Id { get; }
            public string District { get; }
            public string Importance { get; }
            public string SpatialDepth { get; }
            public string InteriorDepth { get; }
            public string Kind { get; }
            public SortedSet<string> Roles { get; }
            public string Anchor { get; }
        }
    }
}
