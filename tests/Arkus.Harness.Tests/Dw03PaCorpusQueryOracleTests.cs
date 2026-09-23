using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw03PaCorpusQueryOracleTests
    {
        private const string P1 = "Docs/research/living-world/results/PA-01.md";
        private const string P2 = "Docs/research/living-world/results/PA-02.md";
        private const string P3 = "Docs/research/living-world/results/PA-03.md";
        private const string P4 = "Docs/research/living-world/results/PA-04.md";
        private const string P5 = "Docs/research/living-world/results/PA-05.md";
        private const string P5F = "Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md";

        [Fact]
        public void EveryContractualQueryDimensionMatchesIndependentSourceSideSetsExactly()
        {
            var fixture = BuildFixture();
            AssertProjectionSourceGreen(fixture);

            foreach (var flag in new[] { "adopt", "adapt", "later", "reject", "baseline" })
            {
                AssertExact(
                    fixture.Oracle.ByDisposition(flag),
                    fixture.Dataset.Queries.FindingsByDispositionFlag(flag),
                    fixture.Dataset);
            }

            AssertExact(fixture.Oracle.AllIds, fixture.Dataset.Queries.ByDomain("living-world"), fixture.Dataset);
            Assert.Empty(fixture.Dataset.Queries.ByDomain("not-a-reviewed-domain"));

            foreach (var family in fixture.Oracle.FailureFamilies)
            {
                AssertExact(fixture.Oracle.ByFailureFamily(family), fixture.Dataset.Queries.ByFailureFamily(family), fixture.Dataset);
            }

            foreach (var paId in fixture.Oracle.PaIds)
            {
                AssertExact(fixture.Oracle.ByPa(paId, "fixture"), fixture.Dataset.Queries.ByPa(paId, "fixture"), fixture.Dataset);
                AssertExact(fixture.Oracle.ByPa(paId, "evidence"), fixture.Dataset.Queries.ByPa(paId, "evidence"), fixture.Dataset);
            }

            foreach (var expectedFixture in fixture.Oracle.Fixtures)
            {
                var resolved = fixture.Dataset.Queries.Fixture(expectedFixture.PaId, expectedFixture.SourceKey);
                Assert.Equal(expectedFixture.FactId, resolved.FactId);
                AssertRecordFidelity(resolved, fixture.Dataset);
                AssertExact(
                    fixture.Oracle.Findings(expectedFixture.PaId),
                    fixture.Dataset.Queries.FindingsLinkedToFixture(expectedFixture.PaId, expectedFixture.SourceKey),
                    fixture.Dataset);
            }

            foreach (var expectedEvidence in fixture.Oracle.Evidence)
            {
                var resolved = fixture.Dataset.Queries.ByPa(expectedEvidence.PaId, "evidence")
                    .Single(record => StringComparer.Ordinal.Equals(record.SourceKey, expectedEvidence.SourceKey));
                Assert.Equal(expectedEvidence.FactId, resolved.FactId);
                AssertRecordFidelity(resolved, fixture.Dataset);
                AssertExact(
                    fixture.Oracle.Findings(expectedEvidence.PaId),
                    fixture.Dataset.Queries.FindingsLinkedToEvidenceKey(expectedEvidence.PaId, expectedEvidence.SourceKey),
                    fixture.Dataset);
            }
        }

        [Fact]
        public void QueryOracleRejectsPa04RejectedOmissionWhileProjectionRemainsSourceGreen()
        {
            var fixture = BuildFixture();
            AssertProjectionSourceGreen(fixture);

            var real = fixture.Dataset.Queries.FindingsByDispositionFlag("reject");
            var corrupted = real.Where(record => !StringComparer.Ordinal.Equals(record.PaId, "pa04")).ToList();

            Assert.Contains(corrupted, record => record.PaId == "pa01");
            Assert.Contains(corrupted, record => record.PaId == "pa03");
            Assert.True(corrupted.Count > 1);

            var report = QuerySetReport.Compare(fixture.Oracle.ByDisposition("reject"), corrupted.Select(record => record.FactId));
            Assert.False(report.IsValid);
            Assert.Contains(report.Missing, id => id.StartsWith("pa04.finding.", StringComparison.Ordinal));
        }

        [Fact]
        public void QueryOracleRejectsNonRejectDomainOmissionWhileProjectionRemainsSourceGreen()
        {
            var fixture = BuildFixture();
            AssertProjectionSourceGreen(fixture);

            var real = fixture.Dataset.Queries.ByDomain("living-world");
            var target = real.First(record => record.PaId == "pa05" && record.RecordKind == "fixture");
            var corrupted = real.Where(record => record.FactId != target.FactId).ToList();
            Assert.True(corrupted.Count > 50);

            var report = QuerySetReport.Compare(fixture.Oracle.AllIds, corrupted.Select(record => record.FactId));
            Assert.False(report.IsValid);
            Assert.Contains(target.FactId, report.Missing);
        }

        [Fact]
        public void QueryOracleRejectsUnexpectedExtraMemberAsWellAsOmissions()
        {
            var fixture = BuildFixture();
            AssertProjectionSourceGreen(fixture);

            var dailyLife = fixture.Dataset.Queries.ByFailureFamily("daily-life").ToList();
            var unexpected = fixture.Dataset.Queries.ByFailureFamily("npc-agency").First();
            dailyLife.Add(unexpected);

            var report = QuerySetReport.Compare(fixture.Oracle.ByFailureFamily("daily-life"), dailyLife.Select(record => record.FactId));
            Assert.False(report.IsValid);
            Assert.Contains(unexpected.FactId, report.Unexpected);
        }

        private static CorpusFixture BuildFixture()
        {
            var sources = new PaAcceptedCorpusSources(Read(P1), Read(P2), Read(P3), Read(P4), Read(P5), Read(P5F));
            var dataset = new PaDesignWorldProvider().BuildAndValidate(sources);
            return new CorpusFixture(sources, dataset, IndependentSourceQueryOracle.Build());
        }

        private static void AssertProjectionSourceGreen(CorpusFixture fixture)
        {
            var report = new PaSourceCorpusOracle().Validate(fixture.Dataset.Projection, fixture.Sources);
            Assert.True(report.IsValid, string.Join("\n", report.Issues.Select(issue => issue.MachineCode + ":" + issue.Detail)));
        }

        private static void AssertExact(
            IReadOnlyCollection<string> expectedIds,
            IReadOnlyCollection<PaCorpusQueryRecord> actual,
            PaCorpusDataset dataset)
        {
            var report = QuerySetReport.Compare(expectedIds, actual.Select(record => record.FactId));
            Assert.True(
                report.IsValid,
                "missing=[" + string.Join(",", report.Missing) + "] unexpected=[" + string.Join(",", report.Unexpected) + "]");
            Assert.Equal(expectedIds.Count, actual.Count);
            foreach (var record in actual) AssertRecordFidelity(record, dataset);
        }

        private static void AssertRecordFidelity(PaCorpusQueryRecord record, PaCorpusDataset dataset)
        {
            var fact = dataset.Projection.Facts.Single(item => item.FactId == record.FactId);
            Assert.Equal(fact.FactType, record.FactType);
            Assert.Equal(Field(fact, "pa-id"), record.PaId);
            Assert.Equal(Field(fact, "record-kind"), record.RecordKind);
            Assert.Equal(Field(fact, "domain"), record.Domain);
            Assert.Equal(Field(fact, "failure-family"), record.FailureFamily);
            Assert.Equal(Field(fact, "source-key"), record.SourceKey);
            Assert.Equal(Field(fact, "material-text"), record.MaterialText);

            Assert.Equal(fact.Provenance.AuthorityId, record.Provenance.AuthorityId);
            Assert.Equal(fact.Provenance.SourcePath, record.Provenance.SourcePath);
            Assert.Equal(fact.Provenance.Anchor, record.Provenance.Anchor);
            Assert.Equal(fact.Provenance.SourceDigest, record.Provenance.SourceDigest);
            Assert.Equal(fact.Provenance.AnchorDigest, record.Provenance.AnchorDigest);

            Assert.Equal(
                fact.Relations.Select(RelationKey).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                record.Relations.Select(RelationKey).OrderBy(value => value, StringComparer.Ordinal).ToArray());

            if (fact.FactType == "pa-finding")
            {
                var target = fact.Relations.Single(relation => relation.RelationType == "has-disposition").TargetFactId;
                var disposition = dataset.Projection.Facts.Single(item => item.FactId == target);
                Assert.Equal(Field(disposition, "disposition-text"), record.DispositionText);
            }
            else
            {
                Assert.Equal(string.Empty, record.DispositionText);
            }
        }

        private static string RelationKey(DesignRelation relation) => relation.RelationType + "\u001f" + relation.TargetFactId;
        private static string Field(DesignFact fact, string name) => fact.Fields[name].CanonicalValue;

        private static string Read(string relativePath)
        {
            return File.ReadAllText(Path.Combine(FindRepositoryRoot(), relativePath.Replace('/', Path.DirectorySeparatorChar)))
                .Replace("\r\n", "\n").Replace('\r', '\n');
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Juego2.sln"))) return directory.FullName;
                directory = directory.Parent;
            }
            throw new InvalidOperationException("Could not locate Juego2.sln from test output directory.");
        }

        private sealed class CorpusFixture
        {
            public CorpusFixture(PaAcceptedCorpusSources sources, PaCorpusDataset dataset, IndependentSourceQueryOracle oracle)
            {
                Sources = sources;
                Dataset = dataset;
                Oracle = oracle;
            }

            public PaAcceptedCorpusSources Sources { get; }
            public PaCorpusDataset Dataset { get; }
            public IndependentSourceQueryOracle Oracle { get; }
        }

        private sealed class QuerySetReport
        {
            private QuerySetReport(IReadOnlyList<string> missing, IReadOnlyList<string> unexpected)
            {
                Missing = missing;
                Unexpected = unexpected;
            }

            public IReadOnlyList<string> Missing { get; }
            public IReadOnlyList<string> Unexpected { get; }
            public bool IsValid => Missing.Count == 0 && Unexpected.Count == 0;

            public static QuerySetReport Compare(IEnumerable<string> expected, IEnumerable<string> actual)
            {
                var expectedList = expected.OrderBy(value => value, StringComparer.Ordinal).ToList();
                var actualList = actual.OrderBy(value => value, StringComparer.Ordinal).ToList();
                return new QuerySetReport(
                    MultisetDifference(expectedList, actualList),
                    MultisetDifference(actualList, expectedList));
            }

            private static IReadOnlyList<string> MultisetDifference(IReadOnlyList<string> left, IReadOnlyList<string> right)
            {
                var counts = new Dictionary<string, int>(StringComparer.Ordinal);
                foreach (var value in right) counts[value] = counts.TryGetValue(value, out var count) ? count + 1 : 1;
                var result = new List<string>();
                foreach (var value in left)
                {
                    if (counts.TryGetValue(value, out var count) && count > 0) counts[value] = count - 1;
                    else result.Add(value);
                }
                return result.AsReadOnly();
            }
        }

        private sealed class IndependentSourceQueryOracle
        {
            private static readonly IReadOnlyDictionary<string, string> FamilyByPa =
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["pa01"] = "daily-life",
                    ["pa02"] = "npc-agency",
                    ["pa03"] = "social-graph",
                    ["pa04"] = "knowledge-belief",
                    ["pa05"] = "rumour-flow"
                };

            private readonly List<ExpectedRecord> _records = new List<ExpectedRecord>();

            private IndependentSourceQueryOracle()
            {
            }

            public IReadOnlyCollection<string> AllIds => _records.Select(record => record.FactId).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            public IReadOnlyCollection<string> PaIds => FamilyByPa.Keys.OrderBy(value => value, StringComparer.Ordinal).ToArray();
            public IReadOnlyCollection<string> FailureFamilies => FamilyByPa.Values.OrderBy(value => value, StringComparer.Ordinal).ToArray();
            public IReadOnlyCollection<ExpectedRecord> Fixtures => _records.Where(record => record.Kind == "fixture").OrderBy(record => record.FactId, StringComparer.Ordinal).ToArray();
            public IReadOnlyCollection<ExpectedRecord> Evidence => _records.Where(record => record.Kind == "evidence").OrderBy(record => record.FactId, StringComparer.Ordinal).ToArray();

            public IReadOnlyCollection<string> ByDisposition(string flag)
            {
                var token = flag switch
                {
                    "adopt" => "ADOPT",
                    "adapt" => "ADAPT",
                    "later" => "LATER",
                    "reject" => "REJECT",
                    "baseline" => "baseline",
                    _ => throw new ArgumentOutOfRangeException(nameof(flag))
                };
                return _records.Where(record => record.Kind == "finding" && Contains(record.Disposition, token))
                    .Select(record => record.FactId).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            }

            public IReadOnlyCollection<string> ByFailureFamily(string family)
            {
                var paIds = FamilyByPa.Where(pair => pair.Value == family).Select(pair => pair.Key).ToHashSet(StringComparer.Ordinal);
                return _records.Where(record => paIds.Contains(record.PaId)).Select(record => record.FactId)
                    .OrderBy(value => value, StringComparer.Ordinal).ToArray();
            }

            public IReadOnlyCollection<string> ByPa(string paId, string kind)
            {
                return _records.Where(record => record.PaId == paId && record.Kind == kind).Select(record => record.FactId)
                    .OrderBy(value => value, StringComparer.Ordinal).ToArray();
            }

            public IReadOnlyCollection<string> Findings(string paId)
            {
                return ByPa(paId, "finding");
            }

            public static IndependentSourceQueryOracle Build()
            {
                var oracle = new IndependentSourceQueryOracle();
                oracle.AddRoot("pa01"); oracle.AddRoot("pa02"); oracle.AddRoot("pa03"); oracle.AddRoot("pa04"); oracle.AddRoot("pa05");

                oracle.EvidenceTable("pa01", P1, "## 1. Exact provenance audited", new[] { "Input", "Exact provenance", "Juego2 use" });
                oracle.FindingTable("pa01", P1, "## 3. Donor → Juego2 disposition", new[] { "ID", "Finding/mechanism", "Disposition", "Juego2 meaning" }, 2);
                oracle.Numbered("pa01", "invariant", P1, "## 5. Required semantic invariants");
                oracle.H3Fixtures("pa01", P1, "## 6. Product scenarios / negative controls");

                oracle.EvidenceTable("pa02", P2, "## 1. Exact provenance audited", new[] { "Input", "Exact provenance", "Juego2 use" });
                oracle.FindingTable("pa02", P2, "## 3. Donor → Juego2 mechanism disposition", new[] { "ID", "Finding / mechanism", "Disposition", "Juego2 meaning" }, 2);
                oracle.Numbered("pa02", "invariant", P2, "## 4. Bounded discovery / anti-global-scan requirements");
                oracle.H3Fixtures("pa02", P2, "## 9. Product scenarios and negative controls");

                oracle.EvidenceTable("pa03", P3, "## 1. Exact provenance audited", new[] { "Input", "Exact provenance", "Juego2 use" });
                oracle.FindingTable("pa03", P3, "## 4. Minimal relationship vocabulary recommendation", new[] { "Relationship/mechanism", "Status", "Juego2 recommendation" }, 1);
                oracle.H3Fixtures("pa03", P3, "## 8. Same-world / different-edge counterfactual fixtures");
                oracle.FailureTable("pa03", P3, "## 13. Failure modes this result forbids", new[] { "Failure mode", "Why it fails PA-03" });

                oracle.EvidenceTable("pa04", P4, "## 1. Exact provenance audited", new[] { "Input", "Exact provenance", "Juego2 use" });
                oracle.FindingTable("pa04", P4, "## 11. Juego2 disposition table", new[] { "Mechanism", "Status", "Juego2 recommendation" }, 1);
                oracle.H3Fixtures("pa04", P4, "## 12. Required causal fixtures");
                oracle.H3Fixtures("pa04", P4, "## 13. Negative controls");
                oracle.FailureTable("pa04", P4, "## 17. Failure-mode audit", new[] { "Failure mode", "PA-04 guardrail" });

                oracle.Pa05Evidence();
                oracle.Pa05Findings();
                oracle.Numbered("pa05", "failure-mode", P5, "## 7. Failure modes / hard negative gates");
                oracle.Pa05Fixtures();

                var duplicates = oracle._records.GroupBy(record => record.FactId, StringComparer.Ordinal).Where(group => group.Count() != 1).ToList();
                if (duplicates.Count != 0) throw new InvalidOperationException("Independent query oracle produced duplicate identities.");
                return oracle;
            }

            private void AddRoot(string paId)
            {
                _records.Add(new ExpectedRecord(paId + ".corpus", paId, "root", paId, string.Empty));
            }

            private void EvidenceTable(string paId, string path, string heading, string[] headers)
            {
                foreach (var row in Table(path, heading, headers)) AddMaterial(paId, "evidence", Clean(row.Cells[0]));
            }

            private void FindingTable(string paId, string path, string heading, string[] headers, int dispositionIndex)
            {
                foreach (var row in Table(path, heading, headers))
                    AddFinding(paId, Clean(row.Cells[0]), Clean(row.Cells[dispositionIndex]));
            }

            private void FailureTable(string paId, string path, string heading, string[] headers)
            {
                foreach (var row in Table(path, heading, headers)) AddMaterial(paId, "failure-mode", Clean(row.Cells[0]));
            }

            private void Numbered(string paId, string kind, string path, string heading)
            {
                foreach (var line in Lines(H2(path, heading)).Where(IsNumbered))
                {
                    var text = line.Trim();
                    AddMaterial(paId, kind, text.Substring(0, text.IndexOf('.', StringComparison.Ordinal)));
                }
            }

            private void H3Fixtures(string paId, string path, string heading)
            {
                foreach (var part in Subsections(H2(path, heading), "### "))
                {
                    var key = BeforeDash(part.Heading);
                    if (FixtureKey(key)) AddMaterial(paId, "fixture", key);
                }
            }

            private void Pa05Evidence()
            {
                var bullets = Lines(H2(P5, "## 1. Exact donor provenance and review lineage"))
                    .Where(line => line.StartsWith("- ", StringComparison.Ordinal)).ToList();
                Assert.Equal(9, bullets.Count);
                foreach (var bullet in bullets) AddMaterial("pa05", "evidence", bullet.Substring(2).Trim());
            }

            private void Pa05Findings()
            {
                foreach (var part in Subsections(H2(P5, "## 4. Findings"), "### ")
                    .Where(part => part.Heading.StartsWith("PA-05-H", StringComparison.Ordinal)))
                {
                    var key = BeforeDash(part.Heading);
                    var decision = Lines(part.Body).Single(line => line.StartsWith("**Decision:**", StringComparison.Ordinal));
                    AddFinding("pa05", key, Clean(decision.Substring("**Decision:**".Length)));
                }
            }

            private void Pa05Fixtures()
            {
                foreach (var part in Subsections(Read(P5F), "## "))
                {
                    var key = H2FixtureKey(part.Heading);
                    if (FixtureKey(key)) AddMaterial("pa05", "fixture", key);
                }
            }

            private void AddFinding(string paId, string sourceKey, string disposition)
            {
                var id = FactId(paId, "finding", sourceKey);
                _records.Add(new ExpectedRecord(id, paId, "finding", sourceKey, disposition));
                _records.Add(new ExpectedRecord(id + ".disposition", paId, "disposition", sourceKey, disposition));
            }

            private void AddMaterial(string paId, string kind, string sourceKey)
            {
                _records.Add(new ExpectedRecord(FactId(paId, kind, sourceKey), paId, kind, sourceKey, string.Empty));
            }

            private static IEnumerable<SourceRow> Table(string path, string heading, string[] expectedHeaders)
            {
                var lines = Lines(H2(path, heading));
                var headerIndex = Array.FindIndex(lines, line => line.StartsWith("|", StringComparison.Ordinal) && Cells(line).SequenceEqual(expectedHeaders, StringComparer.Ordinal));
                if (headerIndex < 0) throw new InvalidOperationException("Independent query oracle header not found: " + heading);
                for (var index = headerIndex + 2; index < lines.Length; index++)
                {
                    if (!lines[index].StartsWith("|", StringComparison.Ordinal)) yield break;
                    var cells = Cells(lines[index]);
                    if (cells.Count != expectedHeaders.Length) throw new InvalidOperationException("Independent query oracle row width changed: " + heading);
                    yield return new SourceRow(cells);
                }
            }

            private static string H2(string path, string heading)
            {
                var lines = Lines(Read(path));
                var matches = Enumerable.Range(0, lines.Length).Where(index => lines[index] == heading).ToList();
                if (matches.Count != 1) throw new InvalidOperationException("Independent query oracle H2 mismatch: " + heading);
                var start = matches[0];
                var end = lines.Length;
                for (var index = start + 1; index < lines.Length; index++)
                {
                    if (lines[index].StartsWith("## ", StringComparison.Ordinal)) { end = index; break; }
                }
                return string.Join("\n", lines.Skip(start).Take(end - start));
            }

            private static IReadOnlyList<SourceSection> Subsections(string source, string prefix)
            {
                var lines = Lines(source);
                var starts = Enumerable.Range(0, lines.Length).Where(index => lines[index].StartsWith(prefix, StringComparison.Ordinal)).ToList();
                var result = new List<SourceSection>();
                foreach (var start in starts)
                {
                    var end = lines.Length;
                    for (var index = start + 1; index < lines.Length; index++)
                    {
                        if (lines[index].StartsWith(prefix, StringComparison.Ordinal) ||
                            (prefix == "### " && lines[index].StartsWith("## ", StringComparison.Ordinal)))
                        {
                            end = index;
                            break;
                        }
                    }
                    result.Add(new SourceSection(lines[start].Substring(prefix.Length).Trim(), string.Join("\n", lines.Skip(start).Take(end - start))));
                }
                return result.AsReadOnly();
            }

            private static IReadOnlyList<string> Cells(string line)
            {
                if (line.Length < 2 || line[0] != '|' || line[line.Length - 1] != '|') return Array.Empty<string>();
                return line.Substring(1, line.Length - 2).Split('|').Select(cell => cell.Trim()).ToList().AsReadOnly();
            }

            private static string[] Lines(string value) => value.Split('\n');
            private static string Clean(string value) => value.Replace("**", string.Empty).Replace("`", string.Empty).Trim();
            private static string BeforeDash(string value) { var index = value.IndexOf(" — ", StringComparison.Ordinal); return (index >= 0 ? value.Substring(0, index) : value).Trim(); }
            private static string H2FixtureKey(string heading) { var dot = heading.IndexOf(". ", StringComparison.Ordinal); return BeforeDash(dot >= 0 ? heading.Substring(dot + 2) : heading); }
            private static bool FixtureKey(string value) => value == "P1" || value == "P2" || value == "P3" || value == "P4" || value.StartsWith("CF-", StringComparison.Ordinal) || value.StartsWith("NC-", StringComparison.Ordinal);
            private static bool IsNumbered(string line) { var text = line.Trim(); var dot = text.IndexOf('.', StringComparison.Ordinal); return dot > 0 && text.Substring(0, dot).All(char.IsDigit) && dot + 1 < text.Length && text[dot + 1] == ' '; }
            private static bool Contains(string value, string token) => value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;

            private static string FactId(string paId, string kind, string sourceKey)
            {
                var token = Token(sourceKey);
                return !string.IsNullOrEmpty(token) && token.Length <= 32
                    ? paId + "." + kind + "." + token
                    : paId + "." + kind + ".h" + Sha256(sourceKey);
            }

            private static string Token(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return string.Empty;
                var builder = new StringBuilder();
                foreach (var ch in value.ToLowerInvariant())
                {
                    if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9')) builder.Append(ch);
                    else if (ch == '-' || ch == '_' || ch == '.') builder.Append(ch == '_' ? '-' : ch);
                    else return string.Empty;
                }
                return builder.ToString();
            }

            private static string Sha256(string value)
            {
                using (var sha = SHA256.Create())
                {
                    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                    var builder = new StringBuilder(bytes.Length * 2);
                    foreach (var item in bytes) builder.Append(item.ToString("x2", CultureInfo.InvariantCulture));
                    return builder.ToString();
                }
            }
        }

        private sealed class ExpectedRecord
        {
            public ExpectedRecord(string factId, string paId, string kind, string sourceKey, string disposition)
            {
                FactId = factId;
                PaId = paId;
                Kind = kind;
                SourceKey = sourceKey;
                Disposition = disposition;
            }

            public string FactId { get; }
            public string PaId { get; }
            public string Kind { get; }
            public string SourceKey { get; }
            public string Disposition { get; }
        }

        private sealed class SourceRow
        {
            public SourceRow(IReadOnlyList<string> cells) { Cells = cells; }
            public IReadOnlyList<string> Cells { get; }
        }

        private sealed class SourceSection
        {
            public SourceSection(string heading, string body)
            {
                Heading = heading;
                Body = body;
            }

            public string Heading { get; }
            public string Body { get; }
        }
    }
}
