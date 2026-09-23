using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw05NeutralFixtureAuthorityTests
    {
        private const string RelativePath = "Docs/evidence/WP-DW-05/fixtures/neutral-observatory.txt";
        private const string AuthorityId = "neutral-observatory-v1";
        private const string FrozenSource =
            "OBSERVATORY|id=observatory.north|elevation_m=2410|remote=true\n" +
            "DETECTOR|id=detector.spectro|wavelength_min_nm=380.5|wavelength_max_nm=740.25\n" +
            "CALIBRATION|id=calibration.lamp-a|mode=argon\n" +
            "CONDITION|sky=clear\n";

        private static readonly DesignProjectionVersion Version = new DesignProjectionVersion(1, "dw05-neutral-v1");

        [Fact]
        public void NeutralAuthorityAndProjectionMatchIndependentFrozenExpectedGraph()
        {
            var source = ReadFrozenSource();
            var expected = FrozenExpectedGraph();
            var parsedAuthority = ParseAuthoritySemantics(source);

            Assert.Empty(CompareAuthorityToFrozenExpected(parsedAuthority, expected));

            var context = BuildProjection(source, CanonicalDefinitions());
            var generic = new DesignWorldProjectionValidator().Validate(
                context.Projection,
                context.Universe,
                context.Reader,
                Version);

            Assert.True(generic.IsValid, string.Join("; ", generic.Issues.Select(issue => issue.MachineCode + ":" + issue.FactId)));
            Assert.Empty(CompareProjectionToFrozenExpected(context.Projection, expected, parsedAuthority));

            Assert.All(context.Projection.Facts, fact =>
            {
                Assert.Equal(AuthorityId, fact.Provenance.AuthorityId);
                Assert.Equal(RelativePath, fact.Provenance.SourcePath);
                Assert.Equal(DesignAuthorityResolutionStatus.Found, context.Reader.Resolve(fact.Provenance));
            });
        }

        [Fact]
        public void SourceDefinitionSemanticDisagreementAndRelationLossTurnIndependentOracleRed()
        {
            var source = ReadFrozenSource();
            var expected = FrozenExpectedGraph();
            var parsedAuthority = ParseAuthoritySemantics(source);
            Assert.Empty(CompareAuthorityToFrozenExpected(parsedAuthority, expected));

            var corruptDefinitions = new[]
            {
                new AnchoredFactDefinition(
                    "observatory.north",
                    "observatory",
                    "OBSERVATORY|id=observatory.north|elevation_m=2410|remote=true",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["elevation-m"] = DesignValue.Integer(9999),
                        ["remote"] = DesignValue.Boolean(false)
                    }),
                new AnchoredFactDefinition(
                    "detector.spectro",
                    "detector",
                    "DETECTOR|id=detector.spectro|wavelength_min_nm=380.5|wavelength_max_nm=740.25",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["wavelength-min-nm"] = DesignValue.Decimal(1m),
                        ["wavelength-max-nm"] = DesignValue.Decimal(999m)
                    }),
                new AnchoredFactDefinition(
                    "calibration.lamp-a",
                    "calibration-source",
                    "CALIBRATION|id=calibration.lamp-a|mode=argon",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["mode"] = DesignValue.String("xenon")
                    })
            };

            var context = BuildProjection(source, corruptDefinitions);
            var generic = new DesignWorldProjectionValidator().Validate(
                context.Projection,
                context.Universe,
                context.Reader,
                Version);

            Assert.True(generic.IsValid, "The negative must stay structurally/provenance GREEN so only the independent semantic oracle catches the disagreement.");

            var issues = CompareProjectionToFrozenExpected(context.Projection, expected, parsedAuthority);
            Assert.Contains("field-value:observatory.north:elevation-m", issues);
            Assert.Contains("field-value:observatory.north:remote", issues);
            Assert.Contains("field-value:detector.spectro:wavelength-min-nm", issues);
            Assert.Contains("field-value:detector.spectro:wavelength-max-nm", issues);
            Assert.Contains("field-value:calibration.lamp-a:mode", issues);
            Assert.Contains("relation-set:observatory.north", issues);
            Assert.Contains("relation-set:detector.spectro", issues);
        }

        private static ProjectionContext BuildProjection(string source, IEnumerable<AnchoredFactDefinition> definitions)
        {
            var expected = FrozenExpectedGraph();
            var universe = new StaticDesignAuthorityUniverse(expected.Select(fact => fact.FactId));
            var reader = new AnchoredTextAuthorityReader(AuthorityId, RelativePath, source, definitions);
            var projection = new DesignWorldProjector().Build(universe, reader, Version);
            return new ProjectionContext(projection, universe, reader);
        }

        private static AnchoredFactDefinition[] CanonicalDefinitions() =>
            new[]
            {
                new AnchoredFactDefinition(
                    "observatory.north",
                    "observatory",
                    "OBSERVATORY|id=observatory.north|elevation_m=2410|remote=true",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["elevation-m"] = DesignValue.Integer(2410),
                        ["remote"] = DesignValue.Boolean(true)
                    },
                    new[] { new DesignRelation("houses", "detector.spectro") }),
                new AnchoredFactDefinition(
                    "detector.spectro",
                    "detector",
                    "DETECTOR|id=detector.spectro|wavelength_min_nm=380.5|wavelength_max_nm=740.25",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["wavelength-min-nm"] = DesignValue.Decimal(380.5m),
                        ["wavelength-max-nm"] = DesignValue.Decimal(740.25m)
                    },
                    new[] { new DesignRelation("calibrated-by", "calibration.lamp-a") }),
                new AnchoredFactDefinition(
                    "calibration.lamp-a",
                    "calibration-source",
                    "CALIBRATION|id=calibration.lamp-a|mode=argon",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["mode"] = DesignValue.String("argon")
                    })
            };

        private static ExpectedFact[] FrozenExpectedGraph() =>
            new[]
            {
                new ExpectedFact(
                    "observatory.north",
                    "observatory",
                    "OBSERVATORY|id=observatory.north|elevation_m=2410|remote=true",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["elevation-m"] = DesignValue.Integer(2410),
                        ["remote"] = DesignValue.Boolean(true)
                    },
                    new[] { "houses->detector.spectro" }),
                new ExpectedFact(
                    "detector.spectro",
                    "detector",
                    "DETECTOR|id=detector.spectro|wavelength_min_nm=380.5|wavelength_max_nm=740.25",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["wavelength-min-nm"] = DesignValue.Decimal(380.5m),
                        ["wavelength-max-nm"] = DesignValue.Decimal(740.25m)
                    },
                    new[] { "calibrated-by->calibration.lamp-a" }),
                new ExpectedFact(
                    "calibration.lamp-a",
                    "calibration-source",
                    "CALIBRATION|id=calibration.lamp-a|mode=argon",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["mode"] = DesignValue.String("argon")
                    },
                    Array.Empty<string>())
            };

        private static IReadOnlyDictionary<string, ParsedAuthorityFact> ParseAuthoritySemantics(string source)
        {
            var result = new Dictionary<string, ParsedAuthorityFact>(StringComparer.Ordinal);
            foreach (var line in source.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var cells = line.Split('|');
                if (cells.Length == 0 || StringComparer.Ordinal.Equals(cells[0], "CONDITION"))
                {
                    continue;
                }

                var values = cells.Skip(1)
                    .Select(cell => cell.Split('=', 2))
                    .ToDictionary(parts => parts[0], parts => parts[1], StringComparer.Ordinal);
                var id = values["id"];
                var fields = new Dictionary<string, DesignValue>(StringComparer.Ordinal);
                string factType;

                switch (cells[0])
                {
                    case "OBSERVATORY":
                        factType = "observatory";
                        fields["elevation-m"] = DesignValue.Integer(long.Parse(values["elevation_m"], CultureInfo.InvariantCulture));
                        fields["remote"] = DesignValue.Boolean(bool.Parse(values["remote"]));
                        break;
                    case "DETECTOR":
                        factType = "detector";
                        fields["wavelength-min-nm"] = DesignValue.Decimal(decimal.Parse(values["wavelength_min_nm"], CultureInfo.InvariantCulture));
                        fields["wavelength-max-nm"] = DesignValue.Decimal(decimal.Parse(values["wavelength_max_nm"], CultureInfo.InvariantCulture));
                        break;
                    case "CALIBRATION":
                        factType = "calibration-source";
                        fields["mode"] = DesignValue.String(values["mode"]);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected neutral authority record kind: " + cells[0]);
                }

                result.Add(id, new ParsedAuthorityFact(id, factType, line, fields));
            }

            return result;
        }

        private static string[] CompareAuthorityToFrozenExpected(
            IReadOnlyDictionary<string, ParsedAuthorityFact> parsed,
            IReadOnlyList<ExpectedFact> expected)
        {
            var issues = new List<string>();
            var expectedIds = expected.Select(fact => fact.FactId).OrderBy(id => id, StringComparer.Ordinal).ToArray();
            var parsedIds = parsed.Keys.OrderBy(id => id, StringComparer.Ordinal).ToArray();
            if (!expectedIds.SequenceEqual(parsedIds, StringComparer.Ordinal))
            {
                issues.Add("authority-id-set");
            }

            foreach (var fact in expected)
            {
                if (!parsed.TryGetValue(fact.FactId, out var sourceFact))
                {
                    issues.Add("authority-missing:" + fact.FactId);
                    continue;
                }

                if (!StringComparer.Ordinal.Equals(fact.FactType, sourceFact.FactType))
                {
                    issues.Add("authority-type:" + fact.FactId);
                }

                if (!StringComparer.Ordinal.Equals(fact.Anchor, sourceFact.Anchor))
                {
                    issues.Add("authority-anchor:" + fact.FactId);
                }

                CompareFields(fact.FactId, sourceFact.Fields, fact.Fields, "authority", issues);
            }

            return issues.OrderBy(issue => issue, StringComparer.Ordinal).ToArray();
        }

        private static string[] CompareProjectionToFrozenExpected(
            DesignWorldProjection projection,
            IReadOnlyList<ExpectedFact> expected,
            IReadOnlyDictionary<string, ParsedAuthorityFact> parsedAuthority)
        {
            var issues = new List<string>();
            var expectedIds = expected.Select(fact => fact.FactId).OrderBy(id => id, StringComparer.Ordinal).ToArray();
            if (!expectedIds.SequenceEqual(projection.FactIds, StringComparer.Ordinal))
            {
                issues.Add("projection-id-set");
            }

            var actualById = projection.Facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);
            foreach (var expectedFact in expected)
            {
                if (!actualById.TryGetValue(expectedFact.FactId, out var actual))
                {
                    issues.Add("projection-missing:" + expectedFact.FactId);
                    continue;
                }

                if (!StringComparer.Ordinal.Equals(expectedFact.FactType, actual.FactType))
                {
                    issues.Add("fact-type:" + expectedFact.FactId);
                }

                CompareFields(expectedFact.FactId, actual.Fields, expectedFact.Fields, "field", issues);

                var actualRelations = actual.Relations
                    .Select(relation => relation.RelationType + "->" + relation.TargetFactId)
                    .OrderBy(value => value, StringComparer.Ordinal)
                    .ToArray();
                var expectedRelations = expectedFact.Relations.OrderBy(value => value, StringComparer.Ordinal).ToArray();
                if (!expectedRelations.SequenceEqual(actualRelations, StringComparer.Ordinal))
                {
                    issues.Add("relation-set:" + expectedFact.FactId);
                }

                if (!StringComparer.Ordinal.Equals(actual.Provenance.AuthorityId, AuthorityId) ||
                    !StringComparer.Ordinal.Equals(actual.Provenance.SourcePath, RelativePath) ||
                    !StringComparer.Ordinal.Equals(actual.Provenance.Anchor, expectedFact.Anchor))
                {
                    issues.Add("provenance:" + expectedFact.FactId);
                }

                if (!parsedAuthority.TryGetValue(expectedFact.FactId, out var sourceFact))
                {
                    issues.Add("source-semantics-missing:" + expectedFact.FactId);
                }
                else
                {
                    CompareFields(expectedFact.FactId, actual.Fields, sourceFact.Fields, "source-field", issues);
                }
            }

            return issues.Distinct(StringComparer.Ordinal).OrderBy(issue => issue, StringComparer.Ordinal).ToArray();
        }

        private static void CompareFields(
            string factId,
            IReadOnlyDictionary<string, DesignValue> actual,
            IReadOnlyDictionary<string, DesignValue> expected,
            string prefix,
            ICollection<string> issues)
        {
            if (actual.Count != expected.Count)
            {
                issues.Add(prefix + "-field-count:" + factId);
            }

            foreach (var pair in expected)
            {
                if (!actual.TryGetValue(pair.Key, out var value))
                {
                    issues.Add(prefix + "-field-missing:" + factId + ":" + pair.Key);
                }
                else if (value != pair.Value)
                {
                    issues.Add(prefix + "-value:" + factId + ":" + pair.Key);
                    if (StringComparer.Ordinal.Equals(prefix, "field"))
                    {
                        issues.Add("field-value:" + factId + ":" + pair.Key);
                    }
                }
            }
        }

        private static string ReadFrozenSource()
        {
            var root = FindRepositoryRoot();
            var absolutePath = Path.Combine(root, RelativePath.Replace('/', Path.DirectorySeparatorChar));
            var source = File.ReadAllText(absolutePath).Replace("\r\n", "\n", StringComparison.Ordinal);
            Assert.Equal(FrozenSource, source);
            return source;
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

        private sealed record ProjectionContext(
            DesignWorldProjection Projection,
            StaticDesignAuthorityUniverse Universe,
            AnchoredTextAuthorityReader Reader);

        private sealed record ParsedAuthorityFact(
            string FactId,
            string FactType,
            string Anchor,
            IReadOnlyDictionary<string, DesignValue> Fields);

        private sealed record ExpectedFact(
            string FactId,
            string FactType,
            string Anchor,
            IReadOnlyDictionary<string, DesignValue> Fields,
            IReadOnlyList<string> Relations);
    }
}
