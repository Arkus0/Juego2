using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Arkus.DesignWorld
{
    public sealed class PaCorpusSemanticIssue
    {
        public PaCorpusSemanticIssue(string machineCode, string factId, string rule, string detail, IEnumerable<string> sourcePaths)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            FactId = factId ?? string.Empty;
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            Detail = detail ?? throw new ArgumentNullException(nameof(detail));
            SourcePaths = new List<string>(sourcePaths ?? Array.Empty<string>()).AsReadOnly();
        }

        public string MachineCode { get; }
        public string FactId { get; }
        public string Rule { get; }
        public string Detail { get; }
        public IReadOnlyList<string> SourcePaths { get; }
    }

    public sealed class PaCorpusSemanticReport
    {
        internal PaCorpusSemanticReport(IEnumerable<PaCorpusSemanticIssue> issues)
        {
            Issues = new List<PaCorpusSemanticIssue>(issues ?? throw new ArgumentNullException(nameof(issues)))
                .OrderBy(item => item.FactId, StringComparer.Ordinal)
                .ThenBy(item => item.MachineCode, StringComparer.Ordinal)
                .ThenBy(item => item.Detail, StringComparer.Ordinal)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<PaCorpusSemanticIssue> Issues { get; }
        public bool IsValid => Issues.Count == 0;
    }

    /// <summary>
    /// Independent PA authority-side losslessness oracle. It intentionally does not consume
    /// PaProjectionManifest, PaProductionCorpusParser, PaRecordDefinition, query output or
    /// projected counts when reconstructing the expected corpus.
    /// </summary>
    public sealed class PaSourceCorpusOracle : IPaCorpusSemanticOracle
    {
        public PaCorpusSemanticReport Validate(DesignWorldProjection projection, PaAcceptedCorpusSources sources)
        {
            if (projection == null) throw new ArgumentNullException(nameof(projection));
            if (sources == null) throw new ArgumentNullException(nameof(sources));

            var issues = new List<PaCorpusSemanticIssue>();
            IReadOnlyDictionary<string, OracleFact> expected;
            try
            {
                expected = new OracleBuilder().Build(sources);
            }
            catch (OracleSourceException exception)
            {
                issues.Add(new PaCorpusSemanticIssue(
                    exception.MachineCode, exception.SubjectId,
                    "the PA semantic oracle must independently reconstruct the reviewed accepted authority universe",
                    exception.Message, new[] { exception.SourcePath }));
                return new PaCorpusSemanticReport(issues);
            }

            var actual = new SortedDictionary<string, DesignFact>(StringComparer.Ordinal);
            foreach (var fact in projection.Facts)
            {
                if (actual.ContainsKey(fact.FactId))
                {
                    issues.Add(Issue("pa.semantic_identity_duplicate", fact.FactId,
                        "each PA semantic record must occur exactly once",
                        "Projected corpus contains a duplicate identity.", fact.Provenance.SourcePath));
                    continue;
                }
                actual.Add(fact.FactId, fact);
            }

            foreach (var pair in expected)
            {
                if (!actual.TryGetValue(pair.Key, out var fact))
                {
                    issues.Add(Issue("pa.semantic_fact_missing", pair.Key,
                        "every independently reconstructed accepted PA record must survive projection",
                        "Expected source-side record is absent from the projected corpus.", pair.Value.SourcePath));
                    continue;
                }
                CompareFact(pair.Value, fact, issues);
            }

            foreach (var pair in actual)
            {
                if (!expected.ContainsKey(pair.Key))
                {
                    issues.Add(Issue("pa.semantic_fact_unexpected", pair.Key,
                        "the PA projection may contain only records reconstructed from the reviewed accepted authority surfaces",
                        "Projected record is outside the independent source-side universe.", pair.Value.Provenance.SourcePath));
                }
            }

            return new PaCorpusSemanticReport(issues);
        }

        private static void CompareFact(OracleFact expected, DesignFact actual, ICollection<PaCorpusSemanticIssue> issues)
        {
            if (!StringComparer.Ordinal.Equals(expected.FactType, actual.FactType))
            {
                issues.Add(Issue("pa.semantic_type_mismatch", expected.FactId,
                    "typed PA identity must preserve its independently reconstructed entity class",
                    "Expected type '" + expected.FactType + "' but projected '" + actual.FactType + "'.", expected.SourcePath));
            }

            var actualFields = actual.Fields.ToDictionary(
                pair => pair.Key,
                pair => new OracleValue(pair.Value.Kind.ToString(), pair.Value.CanonicalValue),
                StringComparer.Ordinal);
            foreach (var field in expected.Fields)
            {
                if (!actualFields.TryGetValue(field.Key, out var value))
                {
                    issues.Add(Issue("pa.semantic_field_missing", expected.FactId,
                        "material PA content must preserve the complete reviewed field set",
                        "Missing field '" + field.Key + "'.", expected.SourcePath));
                }
                else if (!field.Value.Equals(value))
                {
                    issues.Add(Issue("pa.semantic_content_mismatch", expected.FactId,
                        "material PA content must match the accepted source-side reconstruction exactly",
                        "Field '" + field.Key + "' expected " + field.Value + " but projected " + value + ".", expected.SourcePath));
                }
            }
            foreach (var field in actualFields.Keys.Where(key => !expected.Fields.ContainsKey(key)))
            {
                issues.Add(Issue("pa.semantic_field_unexpected", expected.FactId,
                    "reviewed PA schema is fail-closed to undeclared material fields",
                    "Unexpected projected field '" + field + "'.", expected.SourcePath));
            }

            var expectedRelations = expected.Relations.Select(RelationKey).OrderBy(value => value, StringComparer.Ordinal).ToList();
            var actualRelations = actual.Relations.Select(item => item.RelationType + "\u001f" + item.TargetFactId)
                .OrderBy(value => value, StringComparer.Ordinal).ToList();
            if (!expectedRelations.SequenceEqual(actualRelations, StringComparer.Ordinal))
            {
                var missing = MultisetDifference(expectedRelations, actualRelations);
                var extra = MultisetDifference(actualRelations, expectedRelations);
                var code = missing.Count > 0 && extra.Count == 0 ? "pa.semantic_relation_missing" :
                    missing.Count == 0 && extra.Count > 0 ? "pa.semantic_relation_extra" : "pa.semantic_relation_mismatch";
                issues.Add(Issue(code, expected.FactId,
                    "relation names, cardinality and targets are part of PA corpus meaning/navigation and must match source-side expectations",
                    "missing=[" + string.Join(",", missing) + "] extra=[" + string.Join(",", extra) + "]", expected.SourcePath));
            }

            var provenance = actual.Provenance;
            var expectedSourceDigest = DesignWorldEncoding.Sha256Hex(expected.SourceText);
            var expectedAnchorDigest = DesignWorldEncoding.Sha256Hex(expected.Anchor);
            if (!StringComparer.Ordinal.Equals(provenance.AuthorityId, expected.AuthorityId) ||
                !StringComparer.Ordinal.Equals(provenance.SourcePath, expected.SourcePath) ||
                !StringComparer.Ordinal.Equals(provenance.Anchor, expected.Anchor) ||
                !StringComparer.Ordinal.Equals(provenance.SourceDigest, expectedSourceDigest) ||
                !StringComparer.Ordinal.Equals(provenance.AnchorDigest, expectedAnchorDigest))
            {
                issues.Add(Issue("pa.semantic_provenance_mismatch", expected.FactId,
                    "equal PA values are insufficient without exact accepted source-open provenance",
                    "Projected provenance does not identify the independently reconstructed accepted source anchor.",
                    expected.SourcePath, provenance.SourcePath));
            }
        }

        private static List<string> MultisetDifference(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var value in right)
            {
                counts[value] = counts.TryGetValue(value, out var count) ? count + 1 : 1;
            }
            var result = new List<string>();
            foreach (var value in left)
            {
                if (counts.TryGetValue(value, out var count) && count > 0) counts[value] = count - 1;
                else result.Add(value.Replace("\u001f", "->"));
            }
            return result;
        }

        private static string RelationKey(OracleRelation relation) => relation.Type + "\u001f" + relation.Target;

        private static PaCorpusSemanticIssue Issue(string code, string factId, string rule, string detail, params string[] paths) =>
            new PaCorpusSemanticIssue(code, factId, rule, detail, paths.Where(path => !string.IsNullOrWhiteSpace(path)).Distinct(StringComparer.Ordinal));

        private sealed class OracleBuilder
        {
            private const string P1 = "Docs/research/living-world/results/PA-01.md";
            private const string P2 = "Docs/research/living-world/results/PA-02.md";
            private const string P3 = "Docs/research/living-world/results/PA-03.md";
            private const string P4 = "Docs/research/living-world/results/PA-04.md";
            private const string P5 = "Docs/research/living-world/results/PA-05.md";
            private const string P5F = "Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md";

            private readonly Dictionary<string, OracleFact> _records = new Dictionary<string, OracleFact>(StringComparer.Ordinal);
            private readonly Dictionary<string, string> _texts = new Dictionary<string, string>(StringComparer.Ordinal);

            public IReadOnlyDictionary<string, OracleFact> Build(PaAcceptedCorpusSources sources)
            {
                AddSource(P1, "40e854e4bc2e49630c1f49a6001eb5e60896f3ae", sources.Read(P1));
                AddSource(P2, "ce05c64f8bb0cf1f2a0090d35fa1f32757fdd33d", sources.Read(P2));
                AddSource(P3, "f2c553e01fb00a4ef2da88e9bc76f644436f0dfe", sources.Read(P3));
                AddSource(P4, "d065241d5dd4ed663dd686fa288e6d8a43e1dc46", sources.Read(P4));
                AddSource(P5, "d38e8b6261876b28d297893f96150f9f527d7270", sources.Read(P5));
                AddSource(P5F, "3cfb1b431acd9ffb8e1e1098e195367abd96d553", sources.Read(P5F));

                Root("pa01", P1); Root("pa02", P2); Root("pa03", P3); Root("pa04", P4); Root("pa05", P5);

                EvidenceTable("pa01", P1, "## 1. Exact provenance audited", new[] { "Input", "Exact provenance", "Juego2 use" });
                FindingTable("pa01", P1, "## 3. Donor → Juego2 disposition", new[] { "ID", "Finding/mechanism", "Disposition", "Juego2 meaning" }, 2,
                    new[] { "DL-01","DL-02","DL-03","DL-04","DL-05","DL-06","DL-07","DL-08","DL-09","DL-10","DL-11","DL-12","DL-13","DL-14" }, -1);
                Numbered("pa01", "pa-invariant", P1, "## 5. Required semantic invariants", 10);
                H3Fixtures("pa01", P1, "## 6. Product scenarios / negative controls", new[] { "P1","P2","P3","P4","NC-01","NC-02","NC-03","NC-04" });

                EvidenceTable("pa02", P2, "## 1. Exact provenance audited", new[] { "Input", "Exact provenance", "Juego2 use" });
                FindingTable("pa02", P2, "## 3. Donor → Juego2 mechanism disposition", new[] { "ID", "Finding / mechanism", "Disposition", "Juego2 meaning" }, 2,
                    Enumerable.Range(1,15).Select(index => "AG-" + index.ToString("00", CultureInfo.InvariantCulture)).ToArray(), -1);
                Numbered("pa02", "pa-invariant", P2, "## 4. Bounded discovery / anti-global-scan requirements", 6);
                H3Fixtures("pa02", P2, "## 9. Product scenarios and negative controls", new[] { "P1","NC-01","NC-02","NC-03","NC-04","NC-05","NC-06","NC-07" });

                EvidenceTable("pa03", P3, "## 1. Exact provenance audited", new[] { "Input", "Exact provenance", "Juego2 use" });
                FindingTable("pa03", P3, "## 4. Minimal relationship vocabulary recommendation", new[] { "Relationship/mechanism", "Status", "Juego2 recommendation" }, 1, null, 20);
                H3Fixtures("pa03", P3, "## 8. Same-world / different-edge counterfactual fixtures", new[] { "CF-01","CF-02","CF-03","NC-01","NC-02","NC-03","NC-04","NC-05" });
                FailureTable("pa03", P3, "## 13. Failure modes this result forbids", new[] { "Failure mode", "Why it fails PA-03" });

                EvidenceTable("pa04", P4, "## 1. Exact provenance audited", new[] { "Input", "Exact provenance", "Juego2 use" });
                FindingTable("pa04", P4, "## 11. Juego2 disposition table", new[] { "Mechanism", "Status", "Juego2 recommendation" }, 1, null, 18);
                H3Fixtures("pa04", P4, "## 12. Required causal fixtures", new[] { "CF-01","CF-02","CF-03" });
                H3Fixtures("pa04", P4, "## 13. Negative controls", new[] { "NC-01","NC-02","NC-03","NC-04","NC-05","NC-06" });
                FailureTable("pa04", P4, "## 17. Failure-mode audit", new[] { "Failure mode", "PA-04 guardrail" });

                Pa05Evidence(); Pa05Findings();
                Numbered("pa05", "pa-failure-mode", P5, "## 7. Failure modes / hard negative gates", 14);
                Pa05Fixtures();

                Wire();
                return new ReadOnlyDictionary<string, OracleFact>(new SortedDictionary<string, OracleFact>(_records, StringComparer.Ordinal));
            }

            private void AddSource(string path, string expectedBlob, string text)
            {
                var normalized = Normalize(text);
                var actual = GitBlob(normalized);
                if (!StringComparer.Ordinal.Equals(expectedBlob, actual))
                    throw new OracleSourceException("pa.oracle_source_revision", path, path, "Independent oracle source blob mismatch: expected " + expectedBlob + " actual " + actual + ".");
                _texts.Add(path, normalized);
            }

            private void Root(string pa, string path)
            {
                var title = Split(_texts[path]).FirstOrDefault(line => line.StartsWith("# ", StringComparison.Ordinal));
                if (title == null) throw Source(pa, path, "Missing result title.");
                Add(pa + ".corpus", "pa-corpus-root", pa, path, title, MakeFields(pa, "root", pa, title));
            }

            private void EvidenceTable(string pa, string path, string heading, string[] headers)
            {
                foreach (var row in Table(path, heading, headers))
                    AddMaterial(pa, "pa-evidence", path, row.Anchor, Clean(row.Cells[0]), row.Anchor);
            }

            private void FindingTable(string pa, string path, string heading, string[] headers, int dispositionIndex, string[] expectedIds, int expectedCount)
            {
                var rows = Table(path, heading, headers).ToList();
                if (expectedCount >= 0 && rows.Count != expectedCount) throw Source(pa, path, "Independent finding row count mismatch.");
                if (expectedIds != null) ExactSet(expectedIds, rows.Select(row => Clean(row.Cells[0])), path, "finding");
                foreach (var row in rows) Finding(pa, path, Clean(row.Cells[0]), row.Anchor, Clean(row.Cells[dispositionIndex]));
            }

            private void FailureTable(string pa, string path, string heading, string[] headers)
            {
                foreach (var row in Table(path, heading, headers))
                    AddMaterial(pa, "pa-failure-mode", path, row.Anchor, Clean(row.Cells[0]), row.Anchor);
            }

            private void Numbered(string pa, string type, string path, string heading, int expectedCount)
            {
                var section = H2(path, heading);
                var items = Split(section).Where(IsNumbered).ToList();
                if (items.Count != expectedCount) throw Source(pa, path, "Independent numbered item count mismatch.");
                foreach (var item in items) AddMaterial(pa, type, path, item, item.Trim().Substring(0, item.Trim().IndexOf('.', StringComparison.Ordinal)), item);
            }

            private void H3Fixtures(string pa, string path, string heading, string[] expected)
            {
                var parts = Subsections(H2(path, heading), "### ")
                    .Select(part => new { Part = part, Key = BeforeDash(part.Heading) })
                    .Where(item => FixtureKey(item.Key)).ToList();
                ExactSet(expected, parts.Select(item => item.Key), path, "fixture");
                foreach (var item in parts) AddMaterial(pa, "pa-fixture", path, item.Part.Body, item.Key, item.Part.Body);
            }

            private void Pa05Evidence()
            {
                var section = H2(P5, "## 1. Exact donor provenance and review lineage");
                var bullets = Split(section).Where(line => line.StartsWith("- ", StringComparison.Ordinal)).ToList();
                if (bullets.Count != 9) throw Source("pa05", P5, "Independent donor provenance bullet count mismatch.");
                foreach (var bullet in bullets) AddMaterial("pa05", "pa-evidence", P5, bullet, bullet.Substring(2).Trim(), bullet);
            }

            private void Pa05Findings()
            {
                var parts = Subsections(H2(P5, "## 4. Findings"), "### ")
                    .Where(part => part.Heading.StartsWith("PA-05-H", StringComparison.Ordinal)).ToList();
                var expected = Enumerable.Range(1,10).Select(index => "PA-05-H" + index.ToString("00", CultureInfo.InvariantCulture)).ToArray();
                ExactSet(expected, parts.Select(part => BeforeDash(part.Heading)), P5, "PA-05 finding");
                foreach (var part in parts)
                {
                    var decisionLine = Split(part.Body).SingleOrDefault(line => line.StartsWith("**Decision:**", StringComparison.Ordinal));
                    if (decisionLine == null) throw Source(part.Heading, P5, "Independent oracle could not locate PA-05 Decision.");
                    Finding("pa05", P5, BeforeDash(part.Heading), part.Body, Clean(decisionLine.Substring("**Decision:**".Length)));
                }
            }

            private void Pa05Fixtures()
            {
                var parts = Subsections(_texts[P5F], "## ")
                    .Select(part => new { Part = part, Key = H2FixtureKey(part.Heading) })
                    .Where(item => FixtureKey(item.Key)).ToList();
                var expected = new[] { "CF-01","NC-01","CF-02","NC-02","CF-03","CF-04","CF-05" };
                ExactSet(expected, parts.Select(item => item.Key), P5F, "PA-05 fixture");
                foreach (var item in parts) AddMaterial("pa05", "pa-fixture", P5F, item.Part.Body, item.Key, item.Part.Body);
            }

            private void Finding(string pa, string path, string key, string anchor, string disposition)
            {
                var id = Id(pa, "finding", key);
                Add(id, "pa-finding", pa, path, anchor, MakeFields(pa, "finding", key, anchor));
                var fields = MakeFields(pa, "disposition", key, disposition);
                fields["disposition-text"] = S(disposition);
                fields["contains-adopt"] = B(Has(disposition, "ADOPT"));
                fields["contains-adapt"] = B(Has(disposition, "ADAPT"));
                fields["contains-later"] = B(Has(disposition, "LATER"));
                fields["contains-reject"] = B(Has(disposition, "REJECT"));
                fields["contains-baseline"] = B(Has(disposition, "baseline"));
                Add(id + ".disposition", "pa-disposition", pa, path, anchor, fields);
            }

            private void AddMaterial(string pa, string type, string path, string anchor, string key, string material)
            {
                var kind = type.Substring(3);
                Add(Id(pa, kind, key), type, pa, path, anchor, MakeFields(pa, kind, key, material));
            }

            private void Add(string id, string type, string pa, string path, string anchor, Dictionary<string, OracleValue> fields)
            {
                if (_records.ContainsKey(id)) throw Source(id, path, "Independent identity collision.");
                var authority = path == P5F ? "pa05-fixtures-v1" : pa + "-result-v1";
                _records.Add(id, new OracleFact(id, type, pa, path, authority, _texts[path], anchor, fields));
            }

            private void Wire()
            {
                foreach (var group in _records.Values.GroupBy(record => record.PaId, StringComparer.Ordinal))
                {
                    var root = group.Single(record => record.FactType == "pa-corpus-root");
                    var evidence = group.Where(record => record.FactType == "pa-evidence").OrderBy(record => record.FactId, StringComparer.Ordinal).ToList();
                    var fixtures = group.Where(record => record.FactType == "pa-fixture").OrderBy(record => record.FactId, StringComparer.Ordinal).ToList();
                    foreach (var child in group.Where(record => record != root).OrderBy(record => record.FactId, StringComparer.Ordinal))
                    {
                        child.Relations.Add(new OracleRelation("declared-in", root.FactId));
                        root.Relations.Add(new OracleRelation(Container(child.FactType), child.FactId));
                    }
                    foreach (var finding in group.Where(record => record.FactType == "pa-finding"))
                    {
                        finding.Relations.Add(new OracleRelation("has-disposition", finding.FactId + ".disposition"));
                        foreach (var item in evidence) finding.Relations.Add(new OracleRelation("same-authority-evidence", item.FactId));
                        foreach (var item in fixtures) finding.Relations.Add(new OracleRelation("same-authority-fixture", item.FactId));
                    }
                }
            }

            private static string Container(string type)
            {
                switch (type)
                {
                    case "pa-finding": return "contains-finding";
                    case "pa-disposition": return "contains-disposition";
                    case "pa-evidence": return "contains-evidence";
                    case "pa-fixture": return "contains-fixture";
                    case "pa-invariant": return "contains-invariant";
                    case "pa-failure-mode": return "contains-failure-mode";
                    default: throw new InvalidOperationException("Unknown oracle PA type: " + type);
                }
            }

            private Dictionary<string, OracleValue> MakeFields(string pa, string kind, string key, string material)
            {
                return new Dictionary<string, OracleValue>(StringComparer.Ordinal)
                {
                    ["pa-id"] = S(pa), ["record-kind"] = S(kind), ["domain"] = S("living-world"),
                    ["failure-family"] = S(Family(pa)), ["source-key"] = S(key), ["material-text"] = S(material)
                };
            }

            private static string Family(string pa)
            {
                switch (pa)
                {
                    case "pa01": return "daily-life";
                    case "pa02": return "npc-agency";
                    case "pa03": return "social-graph";
                    case "pa04": return "knowledge-belief";
                    case "pa05": return "rumour-flow";
                    default: throw new ArgumentOutOfRangeException(nameof(pa));
                }
            }

            private IEnumerable<OracleRow> Table(string path, string heading, string[] expectedHeaders)
            {
                var section = H2(path, heading);
                var lines = Split(section);
                var headerIndex = Array.FindIndex(lines, line => line.StartsWith("|", StringComparison.Ordinal) && Cells(line).SequenceEqual(expectedHeaders, StringComparer.Ordinal));
                if (headerIndex < 0 || headerIndex + 1 >= lines.Length) throw Source(heading, path, "Independent exact table header missing.");
                var separator = Cells(lines[headerIndex + 1]);
                if (separator.Count != expectedHeaders.Length || separator.Any(cell => cell.Trim(':').Length < 3 || cell.Trim(':').Any(ch => ch != '-')))
                    throw Source(heading, path, "Independent table separator/schema mismatch.");
                var rows = new List<OracleRow>();
                for (var index = headerIndex + 2; index < lines.Length; index++)
                {
                    if (!lines[index].StartsWith("|", StringComparison.Ordinal)) break;
                    var cells = Cells(lines[index]);
                    if (cells.Count != expectedHeaders.Length) throw Source(heading, path, "Independent table row width mismatch.");
                    rows.Add(new OracleRow(lines[index], cells));
                }
                if (rows.Count == 0) throw Source(heading, path, "Independent table is empty.");
                return rows;
            }

            private string H2(string path, string heading)
            {
                var lines = Split(_texts[path]);
                var indexes = Enumerable.Range(0, lines.Length).Where(index => StringComparer.Ordinal.Equals(lines[index], heading)).ToList();
                if (indexes.Count != 1) throw Source(heading, path, "Independent exact H2 heading count=" + indexes.Count + ".");
                var start = indexes[0];
                var end = lines.Length;
                for (var index = start + 1; index < lines.Length; index++) if (lines[index].StartsWith("## ", StringComparison.Ordinal)) { end = index; break; }
                return string.Join("\n", lines.Skip(start).Take(end - start));
            }

            private static List<OracleSection> Subsections(string source, string prefix)
            {
                var lines = Split(source);
                var starts = Enumerable.Range(0, lines.Length).Where(index => lines[index].StartsWith(prefix, StringComparison.Ordinal)).ToList();
                var result = new List<OracleSection>();
                for (var i = 0; i < starts.Count; i++)
                {
                    var start = starts[i]; var end = lines.Length;
                    for (var index = start + 1; index < lines.Length; index++)
                    {
                        if (lines[index].StartsWith(prefix, StringComparison.Ordinal) || (prefix == "### " && lines[index].StartsWith("## ", StringComparison.Ordinal))) { end = index; break; }
                    }
                    result.Add(new OracleSection(lines[start].Substring(prefix.Length).Trim(), string.Join("\n", lines.Skip(start).Take(end - start))));
                }
                return result;
            }

            private static IReadOnlyList<string> Cells(string line)
            {
                if (line.Length < 2 || line[0] != '|' || line[line.Length - 1] != '|') return Array.Empty<string>();
                return line.Substring(1, line.Length - 2).Split('|').Select(cell => cell.Trim()).ToList();
            }

            private static string[] Split(string value) => value.Split('\n');
            private static string Normalize(string value) => value.Replace("\r\n", "\n").Replace('\r', '\n');
            private static string Clean(string value) => value.Replace("**", string.Empty).Replace("`", string.Empty).Trim();
            private static string BeforeDash(string value) { var index = value.IndexOf(" — ", StringComparison.Ordinal); return (index >= 0 ? value.Substring(0, index) : value).Trim(); }
            private static string H2FixtureKey(string heading) { var dot = heading.IndexOf(". ", StringComparison.Ordinal); return BeforeDash(dot >= 0 ? heading.Substring(dot + 2) : heading); }
            private static bool FixtureKey(string value) => value == "P1" || value == "P2" || value == "P3" || value == "P4" || value.StartsWith("CF-", StringComparison.Ordinal) || value.StartsWith("NC-", StringComparison.Ordinal);
            private static bool IsNumbered(string line) { var text = line.Trim(); var dot = text.IndexOf('.', StringComparison.Ordinal); return dot > 0 && text.Substring(0,dot).All(char.IsDigit) && dot + 1 < text.Length && text[dot+1] == ' '; }
            private static bool Has(string value, string token) => value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
            private static OracleValue S(string value) => new OracleValue(DesignValueKind.String.ToString(), value);
            private static OracleValue B(bool value) => new OracleValue(DesignValueKind.Boolean.ToString(), value ? "true" : "false");

            private static string Id(string pa, string kind, string key)
            {
                var token = Token(key);
                return !string.IsNullOrEmpty(token) && token.Length <= 32 ? pa + "." + kind + "." + token : pa + "." + kind + ".h" + Sha256(key);
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
                using (var sha = SHA256.Create()) return Hex(sha.ComputeHash(Encoding.UTF8.GetBytes(value)));
            }

            private static string GitBlob(string value)
            {
                var bytes = Encoding.UTF8.GetBytes(value);
                var prefix = Encoding.UTF8.GetBytes("blob " + bytes.Length.ToString(CultureInfo.InvariantCulture) + "\0");
                var all = new byte[prefix.Length + bytes.Length];
                Buffer.BlockCopy(prefix,0,all,0,prefix.Length); Buffer.BlockCopy(bytes,0,all,prefix.Length,bytes.Length);
                using (var sha = SHA1.Create()) return Hex(sha.ComputeHash(all));
            }

            private static string Hex(byte[] bytes)
            {
                var builder = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) builder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                return builder.ToString();
            }

            private static void ExactSet(IEnumerable<string> expected, IEnumerable<string> actual, string path, string label)
            {
                var e = new SortedSet<string>(expected, StringComparer.Ordinal); var a = new SortedSet<string>(actual, StringComparer.Ordinal);
                if (!e.SetEquals(a)) throw new OracleSourceException("pa.oracle_source_shape_invalid", label, path, "Independent " + label + " set mismatch. expected=[" + string.Join(",",e) + "] actual=[" + string.Join(",",a) + "]");
            }

            private static OracleSourceException Source(string subject, string path, string message) =>
                new OracleSourceException("pa.oracle_source_shape_invalid", subject, path, message);
        }

        private sealed class OracleFact
        {
            public OracleFact(string id, string type, string paId, string sourcePath, string authorityId, string sourceText, string anchor, Dictionary<string, OracleValue> fields)
            {
                FactId=id; FactType=type; PaId=paId; SourcePath=sourcePath; AuthorityId=authorityId; SourceText=sourceText; Anchor=anchor;
                Fields = new ReadOnlyDictionary<string, OracleValue>(new SortedDictionary<string, OracleValue>(fields, StringComparer.Ordinal));
                Relations = new List<OracleRelation>();
            }
            public string FactId { get; }
            public string FactType { get; }
            public string PaId { get; }
            public string SourcePath { get; }
            public string AuthorityId { get; }
            public string SourceText { get; }
            public string Anchor { get; }
            public IReadOnlyDictionary<string, OracleValue> Fields { get; }
            public List<OracleRelation> Relations { get; }
        }

        private sealed class OracleValue : IEquatable<OracleValue>
        {
            public OracleValue(string kind, string value) { Kind=kind; Value=value; }
            public string Kind { get; }
            public string Value { get; }
            public bool Equals(OracleValue other) => other != null && StringComparer.Ordinal.Equals(Kind,other.Kind) && StringComparer.Ordinal.Equals(Value,other.Value);
            public override bool Equals(object obj) => Equals(obj as OracleValue);
            public override int GetHashCode() => (Kind ?? string.Empty).GetHashCode() ^ (Value ?? string.Empty).GetHashCode();
            public override string ToString() => Kind + ":" + Value;
        }

        private sealed class OracleRelation
        {
            public OracleRelation(string type, string target) { Type=type; Target=target; }
            public string Type { get; }
            public string Target { get; }
        }

        private sealed class OracleRow
        {
            public OracleRow(string anchor, IReadOnlyList<string> cells) { Anchor=anchor; Cells=cells; }
            public string Anchor { get; }
            public IReadOnlyList<string> Cells { get; }
        }

        private sealed class OracleSection
        {
            public OracleSection(string heading, string body) { Heading=heading; Body=body; }
            public string Heading { get; }
            public string Body { get; }
        }

        private sealed class OracleSourceException : Exception
        {
            public OracleSourceException(string machineCode, string subjectId, string sourcePath, string message) : base(message)
            { MachineCode=machineCode; SubjectId=subjectId; SourcePath=sourcePath; }
            public string MachineCode { get; }
            public string SubjectId { get; }
            public string SourcePath { get; }
        }
    }
}