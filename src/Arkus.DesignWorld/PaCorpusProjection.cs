using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Arkus.DesignWorld
{
    public sealed class PaCorpusProjectionException : InvalidOperationException
    {
        public PaCorpusProjectionException(string machineCode, string subjectId, string rule, string detail, params string[] sourcePaths)
            : base(detail)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            SubjectId = subjectId ?? string.Empty;
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            Detail = detail ?? throw new ArgumentNullException(nameof(detail));
            SourcePaths = new List<string>(sourcePaths ?? throw new ArgumentNullException(nameof(sourcePaths))).AsReadOnly();
        }

        public string MachineCode { get; }
        public string SubjectId { get; }
        public string Rule { get; }
        public string Detail { get; }
        public IReadOnlyList<string> SourcePaths { get; }
    }

    public sealed class PaSourceDescriptor
    {
        public PaSourceDescriptor(string paId, string authorityId, string sourcePath, string acceptedBlobSha, string semanticRole)
        {
            PaId = Require(paId, nameof(paId));
            AuthorityId = Require(authorityId, nameof(authorityId));
            SourcePath = Require(sourcePath, nameof(sourcePath));
            AcceptedBlobSha = Require(acceptedBlobSha, nameof(acceptedBlobSha));
            SemanticRole = Require(semanticRole, nameof(semanticRole));
        }

        public string PaId { get; }
        public string AuthorityId { get; }
        public string SourcePath { get; }
        public string AcceptedBlobSha { get; }
        public string SemanticRole { get; }

        private static string Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Manifest value cannot be empty.", name);
            return value;
        }
    }

    public static class PaProjectionManifest
    {
        public const string ManifestId = "dw03-pa-corpus-manifest-v1";
        public const string QuerySuiteId = "dw03-pa-query-suite-v1";

        public const string Pa01Path = "Docs/research/living-world/results/PA-01.md";
        public const string Pa02Path = "Docs/research/living-world/results/PA-02.md";
        public const string Pa03Path = "Docs/research/living-world/results/PA-03.md";
        public const string Pa04Path = "Docs/research/living-world/results/PA-04.md";
        public const string Pa05Path = "Docs/research/living-world/results/PA-05.md";
        public const string Pa05FixturesPath = "Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md";

        private static readonly IReadOnlyList<PaSourceDescriptor> ManifestSources = new List<PaSourceDescriptor>
        {
            new PaSourceDescriptor("pa01", "pa01-result-v1", Pa01Path, "40e854e4bc2e49630c1f49a6001eb5e60896f3ae", "accepted PA-01 semantic result"),
            new PaSourceDescriptor("pa02", "pa02-result-v1", Pa02Path, "ce05c64f8bb0cf1f2a0090d35fa1f32757fdd33d", "accepted PA-02 semantic result"),
            new PaSourceDescriptor("pa03", "pa03-result-v1", Pa03Path, "f2c553e01fb00a4ef2da88e9bc76f644436f0dfe", "accepted PA-03 semantic result"),
            new PaSourceDescriptor("pa04", "pa04-result-v1", Pa04Path, "d065241d5dd4ed663dd686fa288e6d8a43e1dc46", "accepted PA-04 semantic result"),
            new PaSourceDescriptor("pa05", "pa05-result-v1", Pa05Path, "d38e8b6261876b28d297893f96150f9f527d7270", "accepted PA-05 semantic result"),
            new PaSourceDescriptor("pa05", "pa05-fixtures-v1", Pa05FixturesPath, "3cfb1b431acd9ffb8e1e1098e195367abd96d553", "accepted PA-05 delegated fixture surface")
        }.AsReadOnly();

        private static readonly IReadOnlyList<string> Types = new List<string>
        {
            "pa-corpus-root", "pa-finding", "pa-disposition", "pa-evidence", "pa-fixture", "pa-invariant", "pa-failure-mode"
        }.AsReadOnly();

        private static readonly IReadOnlyList<string> Fields = new List<string>
        {
            "pa-id", "record-kind", "domain", "failure-family", "source-key", "material-text",
            "disposition-text", "contains-adopt", "contains-adapt", "contains-later", "contains-reject", "contains-baseline"
        }.AsReadOnly();

        private static readonly IReadOnlyList<string> Relations = new List<string>
        {
            "declared-in", "contains-finding", "contains-disposition", "contains-evidence", "contains-fixture",
            "contains-invariant", "contains-failure-mode", "has-disposition", "same-authority-evidence", "same-authority-fixture"
        }.AsReadOnly();

        public static IReadOnlyList<PaSourceDescriptor> Sources => ManifestSources;
        public static IReadOnlyList<string> AdoptedEntityTypes => Types;
        public static IReadOnlyList<string> AdoptedFields => Fields;
        public static IReadOnlyList<string> AdoptedRelations => Relations;

        internal static PaSourceDescriptor GetSource(string path)
        {
            var source = ManifestSources.SingleOrDefault(item => StringComparer.Ordinal.Equals(item.SourcePath, path));
            if (source == null)
            {
                throw new PaCorpusProjectionException(
                    "pa.source_unreviewed", path,
                    "DW-03 accepts only explicitly reviewed PA semantic source families",
                    "Source path is not present in the reviewed DW-03 source manifest.", path);
            }
            return source;
        }
    }

    public sealed class PaAcceptedCorpusSources
    {
        private readonly IReadOnlyDictionary<string, string> _sources;

        public PaAcceptedCorpusSources(
            string pa01,
            string pa02,
            string pa03,
            string pa04,
            string pa05,
            string pa05Fixtures)
        {
            _sources = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [PaProjectionManifest.Pa01Path] = PaText.NormalizeSource(pa01, nameof(pa01)),
                [PaProjectionManifest.Pa02Path] = PaText.NormalizeSource(pa02, nameof(pa02)),
                [PaProjectionManifest.Pa03Path] = PaText.NormalizeSource(pa03, nameof(pa03)),
                [PaProjectionManifest.Pa04Path] = PaText.NormalizeSource(pa04, nameof(pa04)),
                [PaProjectionManifest.Pa05Path] = PaText.NormalizeSource(pa05, nameof(pa05)),
                [PaProjectionManifest.Pa05FixturesPath] = PaText.NormalizeSource(pa05Fixtures, nameof(pa05Fixtures))
            });
        }

        internal string Read(string path)
        {
            if (!_sources.TryGetValue(path, out var value))
            {
                throw new PaCorpusProjectionException(
                    "pa.source_missing", path,
                    "every reviewed semantic source must be supplied before PA projection",
                    "Required accepted PA source is missing.", path);
            }
            return value;
        }

        internal IEnumerable<KeyValuePair<string, string>> All => _sources;
    }

    public sealed class PaCorpusDataset
    {
        internal PaCorpusDataset(DesignWorldProjection projection, PaCorpusQueryService queries)
        {
            Projection = projection ?? throw new ArgumentNullException(nameof(projection));
            Queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        public DesignWorldProjection Projection { get; }
        public PaCorpusQueryService Queries { get; }
        public string ManifestId => PaProjectionManifest.ManifestId;
        public string QuerySuiteId => PaProjectionManifest.QuerySuiteId;
    }

    internal interface IPaCorpusSemanticOracle
    {
        PaCorpusSemanticReport Validate(DesignWorldProjection projection, PaAcceptedCorpusSources sources);
    }

    public sealed class PaDesignWorldProvider
    {
        private static readonly DesignProjectionVersion Version = new DesignProjectionVersion(1, "dw03-pa-corpus-v1");
        private readonly bool _reverseEnumeration;
        private readonly IPaCorpusSemanticOracle _oracle;

        public PaDesignWorldProvider(bool reverseEnumeration = false)
            : this(reverseEnumeration, new PaSourceCorpusOracle())
        {
        }

        internal PaDesignWorldProvider(bool reverseEnumeration, IPaCorpusSemanticOracle oracle)
        {
            _reverseEnumeration = reverseEnumeration;
            _oracle = oracle ?? throw new ArgumentNullException(nameof(oracle));
        }

        public PaCorpusDataset BuildAndValidate(PaAcceptedCorpusSources sources)
        {
            if (sources == null) throw new ArgumentNullException(nameof(sources));

            PaProductionCorpusParser.ValidateAcceptedSourceBlobs(sources);
            var definitions = new PaProductionCorpusParser().Parse(sources);
            var ids = definitions.Select(item => item.FactId).ToList();
            if (_reverseEnumeration)
            {
                ids.Reverse();
                definitions.Reverse();
            }

            var universe = new StaticDesignAuthorityUniverse(ids);
            var reader = new PaMultiSourceAuthorityReader(definitions, sources);
            var projection = new DesignWorldProjector().Build(universe, reader, Version);
            var generic = new DesignWorldProjectionValidator().Validate(projection, universe, reader, Version);
            if (!generic.IsValid)
            {
                var first = generic.Issues.OrderBy(issue => issue.FactId, StringComparer.Ordinal)
                    .ThenBy(issue => issue.MachineCode, StringComparer.Ordinal).First();
                throw new PaCorpusProjectionException(
                    "pa.generic_projection_invalid", first.FactId,
                    "fresh PA projection must satisfy accepted generic Design World guarantees",
                    first.MachineCode + ": " + first.Detail,
                    projection.Facts.Select(fact => fact.Provenance.SourcePath).Distinct().ToArray());
            }

            var semantic = _oracle.Validate(projection, sources);
            if (!semantic.IsValid)
            {
                var first = semantic.Issues.First();
                throw new PaCorpusProjectionException(
                    first.MachineCode, first.FactId,
                    first.Rule, first.Detail, first.SourcePaths.ToArray());
            }

            return new PaCorpusDataset(projection, new PaCorpusQueryService(projection.Facts));
        }
    }

    internal sealed class PaRecordDefinition
    {
        public PaRecordDefinition(
            string factId,
            string factType,
            string paId,
            string sourcePath,
            string sourceAnchor,
            IDictionary<string, DesignValue> fields)
        {
            FactId = factId;
            FactType = factType;
            PaId = paId;
            SourcePath = sourcePath;
            SourceAnchor = sourceAnchor;
            Fields = new SortedDictionary<string, DesignValue>(fields, StringComparer.Ordinal);
            Relations = new List<DesignRelation>();
        }

        public string FactId { get; }
        public string FactType { get; }
        public string PaId { get; }
        public string SourcePath { get; }
        public string SourceAnchor { get; }
        public SortedDictionary<string, DesignValue> Fields { get; }
        public List<DesignRelation> Relations { get; }
    }

    internal sealed class PaMultiSourceAuthorityReader : IDesignAuthorityReader
    {
        private readonly IReadOnlyDictionary<string, DesignFact> _facts;
        private readonly IReadOnlyDictionary<string, SourceState> _sources;

        public PaMultiSourceAuthorityReader(IEnumerable<PaRecordDefinition> definitions, PaAcceptedCorpusSources sources)
        {
            var byPath = new Dictionary<string, SourceState>(StringComparer.Ordinal);
            foreach (var descriptor in PaProjectionManifest.Sources)
            {
                var text = sources.Read(descriptor.SourcePath);
                byPath.Add(descriptor.SourcePath, new SourceState(descriptor.AuthorityId, text));
            }
            _sources = new ReadOnlyDictionary<string, SourceState>(byPath);

            var facts = new SortedDictionary<string, DesignFact>(StringComparer.Ordinal);
            foreach (var definition in definitions)
            {
                if (facts.ContainsKey(definition.FactId))
                {
                    throw new PaCorpusProjectionException(
                        "pa.identity_duplicate", definition.FactId,
                        "every typed PA record identity must be unique",
                        "Production parser produced a duplicate record identity.", definition.SourcePath);
                }

                var state = byPath[definition.SourcePath];
                var occurrences = PaText.CountOccurrences(state.Text, definition.SourceAnchor);
                if (occurrences != 1)
                {
                    throw new PaCorpusProjectionException(
                        occurrences == 0 ? "pa.provenance_anchor_missing" : "pa.provenance_anchor_ambiguous",
                        definition.FactId,
                        "every projected PA record must open one unique accepted source anchor",
                        "Source anchor occurrence count is " + occurrences.ToString(CultureInfo.InvariantCulture) + ".",
                        definition.SourcePath);
                }

                var anchor = new DesignAuthorityAnchor(
                    state.AuthorityId,
                    definition.SourcePath,
                    definition.SourceAnchor,
                    state.SourceDigest,
                    DesignWorldEncoding.Sha256Hex(definition.SourceAnchor));
                facts.Add(definition.FactId, new DesignFact(
                    definition.FactId,
                    definition.FactType,
                    definition.Fields,
                    definition.Relations,
                    anchor));
            }
            _facts = new ReadOnlyDictionary<string, DesignFact>(facts);
        }

        public DesignAuthorityReadResult Read(string factId)
        {
            if (!_facts.TryGetValue(factId, out var fact))
            {
                return DesignAuthorityReadResult.Failure(DesignAuthorityResolutionStatus.Missing, "No PA record exists for the independently required identity.");
            }
            return DesignAuthorityReadResult.Found(fact);
        }

        public DesignAuthorityResolutionStatus Resolve(DesignAuthorityAnchor anchor)
        {
            if (anchor == null) throw new ArgumentNullException(nameof(anchor));
            if (!_sources.TryGetValue(anchor.SourcePath, out var state)) return DesignAuthorityResolutionStatus.Missing;
            if (!StringComparer.Ordinal.Equals(anchor.AuthorityId, state.AuthorityId)) return DesignAuthorityResolutionStatus.Missing;
            var count = PaText.CountOccurrences(state.Text, anchor.Anchor);
            if (count == 0) return DesignAuthorityResolutionStatus.Missing;
            if (count > 1) return DesignAuthorityResolutionStatus.Ambiguous;
            if (!StringComparer.Ordinal.Equals(anchor.SourceDigest, state.SourceDigest) ||
                !StringComparer.Ordinal.Equals(anchor.AnchorDigest, DesignWorldEncoding.Sha256Hex(anchor.Anchor)))
            {
                return DesignAuthorityResolutionStatus.Stale;
            }
            return DesignAuthorityResolutionStatus.Found;
        }

        private sealed class SourceState
        {
            public SourceState(string authorityId, string text)
            {
                AuthorityId = authorityId;
                Text = text;
                SourceDigest = DesignWorldEncoding.Sha256Hex(text);
            }
            public string AuthorityId { get; }
            public string Text { get; }
            public string SourceDigest { get; }
        }
    }

    internal sealed class PaProductionCorpusParser
    {
        private static readonly IReadOnlyDictionary<string, string> FailureFamilies =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["pa01"] = "daily-life",
                ["pa02"] = "npc-agency",
                ["pa03"] = "social-graph",
                ["pa04"] = "knowledge-belief",
                ["pa05"] = "rumour-flow"
            });

        public static void ValidateAcceptedSourceBlobs(PaAcceptedCorpusSources sources)
        {
            foreach (var descriptor in PaProjectionManifest.Sources)
            {
                var actual = PaText.GitBlobSha1(sources.Read(descriptor.SourcePath));
                if (!StringComparer.Ordinal.Equals(actual, descriptor.AcceptedBlobSha))
                {
                    throw new PaCorpusProjectionException(
                        "pa.accepted_source_blob_mismatch", descriptor.PaId,
                        "the declared PA universe is pinned to independently accepted source bytes",
                        "Accepted source blob changed from " + descriptor.AcceptedBlobSha + " to " + actual + "; review the manifest before adopting revised authority.",
                        descriptor.SourcePath);
                }
            }
        }

        public List<PaRecordDefinition> Parse(PaAcceptedCorpusSources sources)
        {
            var records = new List<PaRecordDefinition>();
            AddRoot(records, "pa01", PaProjectionManifest.Pa01Path, sources.Read(PaProjectionManifest.Pa01Path));
            AddRoot(records, "pa02", PaProjectionManifest.Pa02Path, sources.Read(PaProjectionManifest.Pa02Path));
            AddRoot(records, "pa03", PaProjectionManifest.Pa03Path, sources.Read(PaProjectionManifest.Pa03Path));
            AddRoot(records, "pa04", PaProjectionManifest.Pa04Path, sources.Read(PaProjectionManifest.Pa04Path));
            AddRoot(records, "pa05", PaProjectionManifest.Pa05Path, sources.Read(PaProjectionManifest.Pa05Path));

            ParsePa01(records, sources.Read(PaProjectionManifest.Pa01Path));
            ParsePa02(records, sources.Read(PaProjectionManifest.Pa02Path));
            ParsePa03(records, sources.Read(PaProjectionManifest.Pa03Path));
            ParsePa04(records, sources.Read(PaProjectionManifest.Pa04Path));
            ParsePa05(records, sources.Read(PaProjectionManifest.Pa05Path), sources.Read(PaProjectionManifest.Pa05FixturesPath));

            WireRelations(records);
            return records;
        }

        private static void ParsePa01(List<PaRecordDefinition> records, string source)
        {
            AddEvidenceTable(records, "pa01", PaProjectionManifest.Pa01Path, source,
                "## 1. Exact provenance audited", "| Input | Exact provenance | Juego2 use |");
            AddDispositionTable(records, "pa01", PaProjectionManifest.Pa01Path, source,
                "## 3. Donor → Juego2 disposition", "| ID | Finding/mechanism | Disposition | Juego2 meaning |", true,
                new[] { "DL-01","DL-02","DL-03","DL-04","DL-05","DL-06","DL-07","DL-08","DL-09","DL-10","DL-11","DL-12","DL-13","DL-14" });
            AddNumbered(records, "pa01", "pa-invariant", PaProjectionManifest.Pa01Path, source,
                "## 5. Required semantic invariants", 10);
            AddH3Fixtures(records, "pa01", PaProjectionManifest.Pa01Path, source,
                "## 6. Product scenarios / negative controls",
                new[] { "P1","P2","P3","P4","NC-01","NC-02","NC-03","NC-04" });
        }

        private static void ParsePa02(List<PaRecordDefinition> records, string source)
        {
            AddEvidenceTable(records, "pa02", PaProjectionManifest.Pa02Path, source,
                "## 1. Exact provenance audited", "| Input | Exact provenance | Juego2 use |");
            AddDispositionTable(records, "pa02", PaProjectionManifest.Pa02Path, source,
                "## 3. Donor → Juego2 mechanism disposition", "| ID | Finding / mechanism | Disposition | Juego2 meaning |", true,
                Enumerable.Range(1, 15).Select(index => "AG-" + index.ToString("00", CultureInfo.InvariantCulture)).ToArray());
            AddNumbered(records, "pa02", "pa-invariant", PaProjectionManifest.Pa02Path, source,
                "## 4. Bounded discovery / anti-global-scan requirements", 6);
            AddH3Fixtures(records, "pa02", PaProjectionManifest.Pa02Path, source,
                "## 9. Product scenarios and negative controls",
                new[] { "P1","NC-01","NC-02","NC-03","NC-04","NC-05","NC-06","NC-07" });
        }

        private static void ParsePa03(List<PaRecordDefinition> records, string source)
        {
            AddEvidenceTable(records, "pa03", PaProjectionManifest.Pa03Path, source,
                "## 1. Exact provenance audited", "| Input | Exact provenance | Juego2 use |");
            AddDispositionTable(records, "pa03", PaProjectionManifest.Pa03Path, source,
                "## 4. Minimal relationship vocabulary recommendation", "| Relationship/mechanism | Status | Juego2 recommendation |", false, null, 20);
            AddH3Fixtures(records, "pa03", PaProjectionManifest.Pa03Path, source,
                "## 8. Same-world / different-edge counterfactual fixtures",
                new[] { "CF-01","CF-02","CF-03","NC-01","NC-02","NC-03","NC-04","NC-05" });
            AddFailureTable(records, "pa03", PaProjectionManifest.Pa03Path, source,
                "## 13. Failure modes this result forbids", "| Failure mode | Why it fails PA-03 |");
        }

        private static void ParsePa04(List<PaRecordDefinition> records, string source)
        {
            AddEvidenceTable(records, "pa04", PaProjectionManifest.Pa04Path, source,
                "## 1. Exact provenance audited", "| Input | Exact provenance | Juego2 use |");
            AddDispositionTable(records, "pa04", PaProjectionManifest.Pa04Path, source,
                "## 11. Juego2 disposition table", "| Mechanism | Status | Juego2 recommendation |", false, null, 18);
            AddH3Fixtures(records, "pa04", PaProjectionManifest.Pa04Path, source,
                "## 12. Required causal fixtures", new[] { "CF-01","CF-02","CF-03" });
            AddH3Fixtures(records, "pa04", PaProjectionManifest.Pa04Path, source,
                "## 13. Negative controls", new[] { "NC-01","NC-02","NC-03","NC-04","NC-05","NC-06" });
            AddFailureTable(records, "pa04", PaProjectionManifest.Pa04Path, source,
                "## 17. Failure-mode audit", "| Failure mode | PA-04 guardrail |");
        }

        private static void ParsePa05(List<PaRecordDefinition> records, string source, string fixturesSource)
        {
            var evidenceSection = PaText.H2Section(source, "## 1. Exact donor provenance and review lineage", PaProjectionManifest.Pa05Path);
            var evidenceBullets = PaText.Lines(evidenceSection)
                .Where(line => line.StartsWith("- ", StringComparison.Ordinal))
                .TakeWhile(line => !line.StartsWith("- PA-04", StringComparison.Ordinal))
                .ToList();
            if (evidenceBullets.Count != 9)
            {
                throw Shape("pa05", "PA-05 donor provenance bullet count changed.", PaProjectionManifest.Pa05Path);
            }
            foreach (var bullet in evidenceBullets)
            {
                AddMaterialRecord(records, "pa05", "pa-evidence", PaProjectionManifest.Pa05Path, bullet, bullet.Substring(2).Trim(), bullet);
            }

            var findingsSection = PaText.H2Section(source, "## 4. Findings", PaProjectionManifest.Pa05Path);
            var findingSections = PaText.H3Sections(findingsSection);
            var accepted = findingSections.Where(section => section.Heading.StartsWith("PA-05-H", StringComparison.Ordinal)).ToList();
            var expected = Enumerable.Range(1, 10).Select(index => "PA-05-H" + index.ToString("00", CultureInfo.InvariantCulture)).ToArray();
            RequireKeys(expected, accepted.Select(item => PaText.BeforeDash(item.Heading)), "PA-05 finding", PaProjectionManifest.Pa05Path);
            foreach (var item in accepted)
            {
                var fullKey = PaText.BeforeDash(item.Heading);
                var decisionLine = PaText.Lines(item.Body).SingleOrDefault(line => line.StartsWith("**Decision:**", StringComparison.Ordinal));
                if (decisionLine == null) throw Shape(fullKey, "PA-05 finding is missing its exact Decision line.", PaProjectionManifest.Pa05Path);
                var decision = PaText.Clean(decisionLine.Substring("**Decision:**".Length));
                AddFinding(records, "pa05", PaProjectionManifest.Pa05Path, fullKey, item.Body, decision);
            }

            AddNumbered(records, "pa05", "pa-failure-mode", PaProjectionManifest.Pa05Path, source,
                "## 7. Failure modes / hard negative gates", 14);
            AddH2Fixtures(records, "pa05", PaProjectionManifest.Pa05FixturesPath, fixturesSource,
                new[] { "CF-01","NC-01","CF-02","NC-02","CF-03","CF-04","CF-05" });
        }

        private static void AddRoot(List<PaRecordDefinition> records, string paId, string path, string source)
        {
            var title = PaText.Lines(source).FirstOrDefault(line => line.StartsWith("# ", StringComparison.Ordinal));
            if (string.IsNullOrWhiteSpace(title)) throw Shape(paId, "Accepted PA result is missing its title heading.", path);
            records.Add(new PaRecordDefinition(
                paId + ".corpus", "pa-corpus-root", paId, path, title,
                Fields(paId, "root", paId, title)));
        }

        private static void AddEvidenceTable(List<PaRecordDefinition> records, string paId, string path, string source, string heading, string header)
        {
            foreach (var row in PaText.TableRows(PaText.H2Section(source, heading, path), header, path))
            {
                AddMaterialRecord(records, paId, "pa-evidence", path, row.Anchor, PaText.Clean(row.Cells[0]), row.Anchor);
            }
        }

        private static void AddDispositionTable(
            List<PaRecordDefinition> records,
            string paId,
            string path,
            string source,
            string heading,
            string header,
            bool explicitIds,
            IReadOnlyCollection<string> expectedIds,
            int expectedCount = -1)
        {
            var rows = PaText.TableRows(PaText.H2Section(source, heading, path), header, path).ToList();
            if (expectedCount >= 0 && rows.Count != expectedCount)
            {
                throw Shape(paId, "Reviewed disposition table row count changed from " + expectedCount + " to " + rows.Count + ".", path);
            }
            if (explicitIds)
            {
                RequireKeys(expectedIds, rows.Select(row => PaText.Clean(row.Cells[0])), "disposition finding", path);
            }
            foreach (var row in rows)
            {
                var sourceKey = PaText.Clean(row.Cells[0]);
                var dispositionIndex = explicitIds ? 2 : 1;
                AddFinding(records, paId, path, sourceKey, row.Anchor, PaText.Clean(row.Cells[dispositionIndex]));
            }
        }

        private static void AddFailureTable(List<PaRecordDefinition> records, string paId, string path, string source, string heading, string header)
        {
            foreach (var row in PaText.TableRows(PaText.H2Section(source, heading, path), header, path))
            {
                AddMaterialRecord(records, paId, "pa-failure-mode", path, row.Anchor, PaText.Clean(row.Cells[0]), row.Anchor);
            }
        }

        private static void AddNumbered(List<PaRecordDefinition> records, string paId, string factType, string path, string source, string heading, int expectedCount)
        {
            var section = PaText.H2Section(source, heading, path);
            var items = PaText.Lines(section).Where(PaText.IsNumberedLine).ToList();
            if (items.Count != expectedCount)
            {
                throw Shape(paId, "Reviewed numbered semantic surface changed from " + expectedCount + " to " + items.Count + " items.", path);
            }
            foreach (var item in items)
            {
                AddMaterialRecord(records, paId, factType, path, item, PaText.NumberedKey(item), item);
            }
        }

        private static void AddH3Fixtures(List<PaRecordDefinition> records, string paId, string path, string source, string heading, IReadOnlyCollection<string> expectedKeys)
        {
            var fixtures = PaText.H3Sections(PaText.H2Section(source, heading, path))
                .Where(item => PaText.IsFixtureKey(PaText.BeforeDash(item.Heading)))
                .ToList();
            RequireKeys(expectedKeys, fixtures.Select(item => PaText.BeforeDash(item.Heading)), "fixture", path);
            foreach (var fixture in fixtures)
            {
                AddMaterialRecord(records, paId, "pa-fixture", path, fixture.Body, PaText.BeforeDash(fixture.Heading), fixture.Body);
            }
        }

        private static void AddH2Fixtures(List<PaRecordDefinition> records, string paId, string path, string source, IReadOnlyCollection<string> expectedKeys)
        {
            var fixtures = PaText.H2Sections(source)
                .Where(item => PaText.IsFixtureKey(PaText.ExtractH2FixtureKey(item.Heading)))
                .ToList();
            RequireKeys(expectedKeys, fixtures.Select(item => PaText.ExtractH2FixtureKey(item.Heading)), "fixture", path);
            foreach (var fixture in fixtures)
            {
                AddMaterialRecord(records, paId, "pa-fixture", path, fixture.Body, PaText.ExtractH2FixtureKey(fixture.Heading), fixture.Body);
            }
        }

        private static void AddFinding(List<PaRecordDefinition> records, string paId, string path, string sourceKey, string anchor, string disposition)
        {
            var findingId = FactId(paId, "finding", sourceKey);
            records.Add(new PaRecordDefinition(
                findingId, "pa-finding", paId, path, anchor,
                Fields(paId, "finding", sourceKey, anchor)));

            var dispositionId = findingId + ".disposition";
            var dispositionFields = Fields(paId, "disposition", sourceKey, disposition);
            dispositionFields.Add("disposition-text", DesignValue.String(disposition));
            dispositionFields.Add("contains-adopt", DesignValue.Boolean(PaText.ContainsWord(disposition, "ADOPT")));
            dispositionFields.Add("contains-adapt", DesignValue.Boolean(PaText.ContainsWord(disposition, "ADAPT")));
            dispositionFields.Add("contains-later", DesignValue.Boolean(PaText.ContainsWord(disposition, "LATER")));
            dispositionFields.Add("contains-reject", DesignValue.Boolean(PaText.ContainsWord(disposition, "REJECT")));
            dispositionFields.Add("contains-baseline", DesignValue.Boolean(PaText.ContainsWord(disposition, "baseline")));
            records.Add(new PaRecordDefinition(
                dispositionId, "pa-disposition", paId, path, anchor, dispositionFields));
        }

        private static void AddMaterialRecord(List<PaRecordDefinition> records, string paId, string factType, string path, string anchor, string sourceKey, string material)
        {
            var kind = factType.Substring("pa-".Length);
            records.Add(new PaRecordDefinition(
                FactId(paId, kind, sourceKey), factType, paId, path, anchor,
                Fields(paId, kind, sourceKey, material)));
        }

        private static SortedDictionary<string, DesignValue> Fields(string paId, string kind, string sourceKey, string material)
        {
            return new SortedDictionary<string, DesignValue>(StringComparer.Ordinal)
            {
                ["pa-id"] = DesignValue.String(paId),
                ["record-kind"] = DesignValue.String(kind),
                ["domain"] = DesignValue.String("living-world"),
                ["failure-family"] = DesignValue.String(FailureFamilies[paId]),
                ["source-key"] = DesignValue.String(sourceKey),
                ["material-text"] = DesignValue.String(material)
            };
        }

        private static void WireRelations(List<PaRecordDefinition> records)
        {
            var byPa = records.GroupBy(item => item.PaId, StringComparer.Ordinal);
            foreach (var group in byPa)
            {
                var root = group.Single(item => item.FactType == "pa-corpus-root");
                var evidence = group.Where(item => item.FactType == "pa-evidence").OrderBy(item => item.FactId, StringComparer.Ordinal).ToList();
                var fixtures = group.Where(item => item.FactType == "pa-fixture").OrderBy(item => item.FactId, StringComparer.Ordinal).ToList();
                foreach (var child in group.Where(item => item != root).OrderBy(item => item.FactId, StringComparer.Ordinal))
                {
                    child.Relations.Add(new DesignRelation("declared-in", root.FactId));
                    root.Relations.Add(new DesignRelation(ContainerRelation(child.FactType), child.FactId));
                }
                foreach (var finding in group.Where(item => item.FactType == "pa-finding"))
                {
                    var dispositionId = finding.FactId + ".disposition";
                    if (!group.Any(item => StringComparer.Ordinal.Equals(item.FactId, dispositionId)))
                    {
                        throw Shape(finding.FactId, "Finding has no disposition record.", finding.SourcePath);
                    }
                    finding.Relations.Add(new DesignRelation("has-disposition", dispositionId));
                    foreach (var item in evidence) finding.Relations.Add(new DesignRelation("same-authority-evidence", item.FactId));
                    foreach (var item in fixtures) finding.Relations.Add(new DesignRelation("same-authority-fixture", item.FactId));
                }
            }
        }

        private static string ContainerRelation(string type)
        {
            switch (type)
            {
                case "pa-finding": return "contains-finding";
                case "pa-disposition": return "contains-disposition";
                case "pa-evidence": return "contains-evidence";
                case "pa-fixture": return "contains-fixture";
                case "pa-invariant": return "contains-invariant";
                case "pa-failure-mode": return "contains-failure-mode";
                default: throw new InvalidOperationException("Unsupported PA child type: " + type);
            }
        }

        private static string FactId(string paId, string kind, string sourceKey)
        {
            var normalized = PaText.ExplicitToken(sourceKey);
            if (!string.IsNullOrEmpty(normalized) && normalized.Length <= 32)
            {
                return paId + "." + kind + "." + normalized;
            }
            return paId + "." + kind + ".h" + DesignWorldEncoding.Sha256Hex(sourceKey);
        }

        private static void RequireKeys(IEnumerable<string> expected, IEnumerable<string> actual, string label, string path)
        {
            var expectedSet = new SortedSet<string>(expected, StringComparer.Ordinal);
            var actualSet = new SortedSet<string>(actual, StringComparer.Ordinal);
            if (!expectedSet.SetEquals(actualSet))
            {
                throw Shape(label, "Reviewed " + label + " key set changed. expected=[" + string.Join(",", expectedSet) + "] actual=[" + string.Join(",", actualSet) + "]", path);
            }
        }

        private static PaCorpusProjectionException Shape(string subject, string detail, string path)
        {
            return new PaCorpusProjectionException(
                "pa.source_shape_invalid", subject,
                "accepted PA structured surfaces must match the reviewed DW-03 source schema exactly",
                detail, path);
        }
    }

    internal static class PaText
    {
        public static string NormalizeSource(string source, string name)
        {
            if (source == null) throw new ArgumentNullException(name);
            return source.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        public static string[] Lines(string source) => source.Split('\n');

        public static string GitBlobSha1(string source)
        {
            var bytes = Encoding.UTF8.GetBytes(source);
            var prefix = Encoding.UTF8.GetBytes("blob " + bytes.Length.ToString(CultureInfo.InvariantCulture) + "\0");
            var combined = new byte[prefix.Length + bytes.Length];
            Buffer.BlockCopy(prefix, 0, combined, 0, prefix.Length);
            Buffer.BlockCopy(bytes, 0, combined, prefix.Length, bytes.Length);
            using (var sha = SHA1.Create())
            {
                var hash = sha.ComputeHash(combined);
                var builder = new StringBuilder(hash.Length * 2);
                foreach (var value in hash) builder.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                return builder.ToString();
            }
        }

        public static string H2Section(string source, string exactHeading, string path)
        {
            var lines = Lines(source);
            var indexes = Enumerable.Range(0, lines.Length).Where(index => StringComparer.Ordinal.Equals(lines[index].TrimEnd(), exactHeading)).ToList();
            if (indexes.Count != 1)
            {
                throw new PaCorpusProjectionException(
                    "pa.source_shape_invalid", exactHeading,
                    "reviewed PA sections must resolve by exact heading",
                    "Expected exactly one heading but found " + indexes.Count + ".", path);
            }
            var start = indexes[0];
            var end = lines.Length;
            for (var index = start + 1; index < lines.Length; index++)
            {
                if (lines[index].StartsWith("## ", StringComparison.Ordinal)) { end = index; break; }
            }
            return string.Join("\n", lines.Skip(start).Take(end - start));
        }

        public static IReadOnlyList<MarkdownSection> H2Sections(string source)
        {
            return Sections(source, "## ", "## ");
        }

        public static IReadOnlyList<MarkdownSection> H3Sections(string section)
        {
            return Sections(section, "### ", "### ");
        }

        private static IReadOnlyList<MarkdownSection> Sections(string source, string startPrefix, string boundaryPrefix)
        {
            var lines = Lines(source);
            var starts = Enumerable.Range(0, lines.Length).Where(index => lines[index].StartsWith(startPrefix, StringComparison.Ordinal)).ToList();
            var result = new List<MarkdownSection>();
            for (var itemIndex = 0; itemIndex < starts.Count; itemIndex++)
            {
                var start = starts[itemIndex];
                var end = lines.Length;
                for (var index = start + 1; index < lines.Length; index++)
                {
                    if (lines[index].StartsWith(boundaryPrefix, StringComparison.Ordinal)) { end = index; break; }
                    if (startPrefix == "### " && lines[index].StartsWith("## ", StringComparison.Ordinal)) { end = index; break; }
                }
                var heading = lines[start].Substring(startPrefix.Length).Trim();
                result.Add(new MarkdownSection(heading, string.Join("\n", lines.Skip(start).Take(end - start))));
            }
            return result.AsReadOnly();
        }

        public static IReadOnlyList<MarkdownRow> TableRows(string section, string exactHeader, string path)
        {
            var lines = Lines(section);
            var headerIndex = Array.FindIndex(lines, line => StringComparer.Ordinal.Equals(line.TrimEnd(), exactHeader));
            if (headerIndex < 0 || headerIndex + 1 >= lines.Length)
            {
                throw new PaCorpusProjectionException("pa.source_shape_invalid", exactHeader, "reviewed markdown table header must match exactly", "Expected table header is absent.", path);
            }
            var headerCells = SplitRow(exactHeader, path);
            var separatorCells = SplitRow(lines[headerIndex + 1].TrimEnd(), path);
            if (separatorCells.Count != headerCells.Count || separatorCells.Any(cell => !IsSeparator(cell)))
            {
                throw new PaCorpusProjectionException("pa.source_shape_invalid", exactHeader, "reviewed markdown table schema includes exact ordered header width", "Table separator does not match reviewed header width.", path);
            }
            var result = new List<MarkdownRow>();
            for (var index = headerIndex + 2; index < lines.Length; index++)
            {
                var line = lines[index].TrimEnd();
                if (!line.StartsWith("|", StringComparison.Ordinal)) break;
                var cells = SplitRow(line, path);
                if (cells.Count != headerCells.Count)
                {
                    throw new PaCorpusProjectionException("pa.source_shape_invalid", exactHeader, "every accepted table row must match reviewed header width", "Row width changed.", path);
                }
                result.Add(new MarkdownRow(line, cells));
            }
            if (result.Count == 0)
            {
                throw new PaCorpusProjectionException("pa.source_shape_invalid", exactHeader, "reviewed semantic tables cannot project an empty universe", "No table data rows found.", path);
            }
            return result.AsReadOnly();
        }

        private static IReadOnlyList<string> SplitRow(string line, string path)
        {
            if (line.Length < 2 || line[0] != '|' || line[line.Length - 1] != '|')
            {
                throw new PaCorpusProjectionException("pa.source_shape_invalid", line, "accepted table rows use complete pipe-delimited markdown schema", "Malformed markdown table row.", path);
            }
            return line.Substring(1, line.Length - 2).Split('|').Select(cell => cell.Trim()).ToList().AsReadOnly();
        }

        private static bool IsSeparator(string cell)
        {
            var value = cell.Trim().Trim(':');
            return value.Length >= 3 && value.All(character => character == '-');
        }

        public static bool IsNumberedLine(string line)
        {
            var trimmed = line.Trim();
            var dot = trimmed.IndexOf('.', StringComparison.Ordinal);
            return dot > 0 && trimmed.Substring(0, dot).All(char.IsDigit) && dot + 1 < trimmed.Length && trimmed[dot + 1] == ' ';
        }

        public static string NumberedKey(string line)
        {
            var trimmed = line.Trim();
            return trimmed.Substring(0, trimmed.IndexOf('.', StringComparison.Ordinal));
        }

        public static string BeforeDash(string heading)
        {
            var dash = heading.IndexOf(" — ", StringComparison.Ordinal);
            return (dash >= 0 ? heading.Substring(0, dash) : heading).Trim();
        }

        public static string ExtractH2FixtureKey(string heading)
        {
            var dot = heading.IndexOf(". ", StringComparison.Ordinal);
            var rest = dot >= 0 ? heading.Substring(dot + 2) : heading;
            return BeforeDash(rest);
        }

        public static bool IsFixtureKey(string value)
        {
            return value == "P1" || value == "P2" || value == "P3" || value == "P4" ||
                   value.StartsWith("CF-", StringComparison.Ordinal) || value.StartsWith("NC-", StringComparison.Ordinal);
        }

        public static string Clean(string value)
        {
            return value.Replace("**", string.Empty).Replace("`", string.Empty).Trim();
        }

        public static bool ContainsWord(string source, string token)
        {
            return source.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string ExplicitToken(string sourceKey)
        {
            if (string.IsNullOrWhiteSpace(sourceKey)) return string.Empty;
            var builder = new StringBuilder();
            foreach (var character in sourceKey.ToLowerInvariant())
            {
                if ((character >= 'a' && character <= 'z') || (character >= '0' && character <= '9')) builder.Append(character);
                else if (character == '-' || character == '_' || character == '.') builder.Append(character == '_' ? '-' : character);
                else return string.Empty;
            }
            return builder.ToString();
        }

        public static int CountOccurrences(string source, string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            var count = 0;
            var start = 0;
            while (start <= source.Length - value.Length)
            {
                var index = source.IndexOf(value, start, StringComparison.Ordinal);
                if (index < 0) break;
                count++;
                start = index + value.Length;
            }
            return count;
        }
    }

    internal sealed class MarkdownRow
    {
        public MarkdownRow(string anchor, IReadOnlyList<string> cells)
        {
            Anchor = anchor;
            Cells = cells;
        }
        public string Anchor { get; }
        public IReadOnlyList<string> Cells { get; }
    }

    internal sealed class MarkdownSection
    {
        public MarkdownSection(string heading, string body)
        {
            Heading = heading;
            Body = body;
        }
        public string Heading { get; }
        public string Body { get; }
    }
}