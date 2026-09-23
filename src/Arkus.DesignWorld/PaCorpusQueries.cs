using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Arkus.DesignWorld
{
    public sealed class PaCorpusQueryRecord
    {
        internal PaCorpusQueryRecord(DesignFact fact, string dispositionText)
        {
            if (fact == null) throw new ArgumentNullException(nameof(fact));
            FactId = fact.FactId;
            FactType = fact.FactType;
            PaId = RequiredString(fact, "pa-id");
            RecordKind = RequiredString(fact, "record-kind");
            Domain = RequiredString(fact, "domain");
            FailureFamily = RequiredString(fact, "failure-family");
            SourceKey = RequiredString(fact, "source-key");
            MaterialText = RequiredString(fact, "material-text");
            DispositionText = dispositionText ?? string.Empty;
            Provenance = fact.Provenance;
            Relations = fact.Relations;
        }

        public string FactId { get; }
        public string FactType { get; }
        public string PaId { get; }
        public string RecordKind { get; }
        public string Domain { get; }
        public string FailureFamily { get; }
        public string SourceKey { get; }
        public string MaterialText { get; }
        public string DispositionText { get; }
        public DesignAuthorityAnchor Provenance { get; }
        public IReadOnlyList<DesignRelation> Relations { get; }

        private static string RequiredString(DesignFact fact, string field)
        {
            if (!fact.Fields.TryGetValue(field, out var value) || value.Kind != DesignValueKind.String)
            {
                throw new PaCorpusProjectionException(
                    "pa.query_field_missing", fact.FactId,
                    "every compact PA query record must preserve the reviewed typed material fields",
                    "Missing required string field '" + field + "'.", fact.Provenance.SourcePath);
            }
            return value.CanonicalValue;
        }
    }

    public sealed class PaCorpusQueryService
    {
        private readonly IReadOnlyDictionary<string, DesignFact> _facts;
        private readonly IReadOnlyList<PaCorpusQueryRecord> _all;

        internal PaCorpusQueryService(IEnumerable<DesignFact> facts)
        {
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            var map = new SortedDictionary<string, DesignFact>(StringComparer.Ordinal);
            foreach (var fact in facts)
            {
                if (map.ContainsKey(fact.FactId))
                {
                    throw new PaCorpusProjectionException(
                        "pa.query_identity_duplicate", fact.FactId,
                        "compact PA queries require one record per semantic identity",
                        "Duplicate projected record identity.", fact.Provenance.SourcePath);
                }
                map.Add(fact.FactId, fact);
            }
            _facts = new ReadOnlyDictionary<string, DesignFact>(map);
            _all = map.Values.Select(ToRecord).ToList().AsReadOnly();
        }

        public IReadOnlyList<PaCorpusQueryRecord> AllRecords => _all;

        public PaCorpusQueryRecord ById(string factId)
        {
            if (factId == null) throw new ArgumentNullException(nameof(factId));
            if (!_facts.TryGetValue(factId, out var fact))
            {
                throw new PaCorpusProjectionException(
                    "pa.query_record_missing", factId,
                    "source-derived compact identity must resolve exactly when queried",
                    "No projected PA record has the requested identity.");
            }
            return ToRecord(fact);
        }

        public IReadOnlyList<PaCorpusQueryRecord> ByPa(string paId, string? recordKind = null)
        {
            if (paId == null) throw new ArgumentNullException(nameof(paId));
            return _facts.Values
                .Where(fact => StringField(fact, "pa-id") == paId &&
                    (recordKind == null || StringField(fact, "record-kind") == recordKind))
                .OrderBy(fact => fact.FactId, StringComparer.Ordinal)
                .Select(ToRecord).ToList().AsReadOnly();
        }

        public IReadOnlyList<PaCorpusQueryRecord> ByDomain(string domain)
        {
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            return _facts.Values
                .Where(fact => StringField(fact, "domain") == domain)
                .OrderBy(fact => fact.FactId, StringComparer.Ordinal)
                .Select(ToRecord).ToList().AsReadOnly();
        }

        public IReadOnlyList<PaCorpusQueryRecord> ByFailureFamily(string failureFamily)
        {
            if (failureFamily == null) throw new ArgumentNullException(nameof(failureFamily));
            return _facts.Values
                .Where(fact => StringField(fact, "failure-family") == failureFamily)
                .OrderBy(fact => fact.FactId, StringComparer.Ordinal)
                .Select(ToRecord).ToList().AsReadOnly();
        }

        public IReadOnlyList<PaCorpusQueryRecord> FindingsByDispositionFlag(string flag)
        {
            var field = flag switch
            {
                "adopt" => "contains-adopt",
                "adapt" => "contains-adapt",
                "later" => "contains-later",
                "reject" => "contains-reject",
                "baseline" => "contains-baseline",
                _ => throw new ArgumentOutOfRangeException(nameof(flag), "Supported disposition flags: adopt, adapt, later, reject, baseline.")
            };

            var dispositionIds = new SortedSet<string>(
                _facts.Values.Where(fact => fact.FactType == "pa-disposition" && BoolField(fact, field))
                    .Select(fact => fact.FactId), StringComparer.Ordinal);
            return _facts.Values.Where(fact => fact.FactType == "pa-finding" &&
                    fact.Relations.Any(relation => relation.RelationType == "has-disposition" && dispositionIds.Contains(relation.TargetFactId)))
                .OrderBy(fact => fact.FactId, StringComparer.Ordinal)
                .Select(ToRecord).ToList().AsReadOnly();
        }

        public PaCorpusQueryRecord Fixture(string paId, string fixtureKey)
        {
            if (paId == null) throw new ArgumentNullException(nameof(paId));
            if (fixtureKey == null) throw new ArgumentNullException(nameof(fixtureKey));
            var matches = _facts.Values.Where(fact => fact.FactType == "pa-fixture" &&
                    StringField(fact, "pa-id") == paId && StringField(fact, "source-key") == fixtureKey).ToList();
            if (matches.Count != 1)
            {
                throw new PaCorpusProjectionException(
                    "pa.query_fixture_resolution", paId + ":" + fixtureKey,
                    "accepted fixture key must resolve exactly once inside its PA authority",
                    "Fixture match count is " + matches.Count + ".");
            }
            return ToRecord(matches[0]);
        }

        public IReadOnlyList<PaCorpusQueryRecord> FindingsLinkedToFixture(string paId, string fixtureKey)
        {
            var fixture = Fixture(paId, fixtureKey);
            return _facts.Values.Where(fact => fact.FactType == "pa-finding" &&
                    StringField(fact, "pa-id") == paId &&
                    fact.Relations.Any(relation => relation.RelationType == "same-authority-fixture" && relation.TargetFactId == fixture.FactId))
                .OrderBy(fact => fact.FactId, StringComparer.Ordinal)
                .Select(ToRecord).ToList().AsReadOnly();
        }

        public IReadOnlyList<PaCorpusQueryRecord> FindingsLinkedToEvidenceKey(string paId, string evidenceKey)
        {
            if (paId == null) throw new ArgumentNullException(nameof(paId));
            if (evidenceKey == null) throw new ArgumentNullException(nameof(evidenceKey));
            var evidence = _facts.Values.Where(fact => fact.FactType == "pa-evidence" &&
                    StringField(fact, "pa-id") == paId && StringField(fact, "source-key") == evidenceKey).ToList();
            if (evidence.Count != 1)
            {
                throw new PaCorpusProjectionException(
                    "pa.query_evidence_resolution", paId + ":" + evidenceKey,
                    "accepted evidence key must resolve exactly once inside its PA authority",
                    "Evidence match count is " + evidence.Count + ".");
            }
            var id = evidence[0].FactId;
            return _facts.Values.Where(fact => fact.FactType == "pa-finding" && StringField(fact, "pa-id") == paId &&
                    fact.Relations.Any(relation => relation.RelationType == "same-authority-evidence" && relation.TargetFactId == id))
                .OrderBy(fact => fact.FactId, StringComparer.Ordinal).Select(ToRecord).ToList().AsReadOnly();
        }

        public string BuildCompactIndex()
        {
            var builder = new StringBuilder();
            builder.Append("schema=dw03-pa-compact-index-v1\n");
            builder.Append("manifest=").Append(PaProjectionManifest.ManifestId).Append('\n');
            foreach (var fact in _facts.Values.OrderBy(item => item.FactId, StringComparer.Ordinal))
            {
                builder.Append("record=").Append(fact.FactId)
                    .Append("|type=").Append(fact.FactType)
                    .Append("|pa=").Append(StringField(fact, "pa-id"))
                    .Append("|family=").Append(StringField(fact, "failure-family"))
                    .Append("|material=").Append(DesignWorldEncoding.Sha256Hex(StringField(fact, "material-text")))
                    .Append("|source=").Append(fact.Provenance.SourcePath)
                    .Append("|anchor=").Append(fact.Provenance.AnchorDigest)
                    .Append('\n');
                foreach (var relation in fact.Relations.OrderBy(item => item.RelationType, StringComparer.Ordinal).ThenBy(item => item.TargetFactId, StringComparer.Ordinal))
                {
                    builder.Append("relation=").Append(fact.FactId).Append('|')
                        .Append(relation.RelationType).Append('|').Append(relation.TargetFactId).Append('\n');
                }
            }
            return builder.ToString();
        }

        private PaCorpusQueryRecord ToRecord(DesignFact fact)
        {
            var disposition = string.Empty;
            if (fact.FactType == "pa-finding")
            {
                var relation = fact.Relations.SingleOrDefault(item => item.RelationType == "has-disposition");
                if (relation == null || !_facts.TryGetValue(relation.TargetFactId, out var dispositionFact))
                {
                    throw new PaCorpusProjectionException(
                        "pa.query_disposition_relation_invalid", fact.FactId,
                        "every finding must resolve exactly one typed disposition before compact delivery",
                        "Finding has no resolvable has-disposition relation.", fact.Provenance.SourcePath);
                }
                disposition = StringField(dispositionFact, "disposition-text");
            }
            return new PaCorpusQueryRecord(fact, disposition);
        }

        private static string StringField(DesignFact fact, string name)
        {
            if (!fact.Fields.TryGetValue(name, out var value) || value.Kind != DesignValueKind.String)
            {
                throw new PaCorpusProjectionException(
                    "pa.query_field_missing", fact.FactId,
                    "query fields must preserve reviewed PA typed schema",
                    "Missing string field '" + name + "'.", fact.Provenance.SourcePath);
            }
            return value.CanonicalValue;
        }

        private static bool BoolField(DesignFact fact, string name)
        {
            if (!fact.Fields.TryGetValue(name, out var value) || value.Kind != DesignValueKind.Boolean)
            {
                throw new PaCorpusProjectionException(
                    "pa.query_field_missing", fact.FactId,
                    "disposition query flags must preserve reviewed PA typed schema",
                    "Missing boolean field '" + name + "'.", fact.Provenance.SourcePath);
            }
            return value.CanonicalValue == "true";
        }
    }
}
