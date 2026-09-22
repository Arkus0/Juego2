using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw03PaCorpusProjectionTests
    {
        [Fact]
        public void AcceptedPaCorpusAnswersFrozenCrossPaQuerySuite()
        {
            var dataset = new PaDesignWorldProvider().BuildAndValidate(ReadSources());

            Assert.Equal("dw03-pa-corpus-manifest-v1", dataset.ManifestId);
            Assert.Equal("dw03-pa-query-suite-v1", dataset.QuerySuiteId);
            Assert.NotEmpty(dataset.Projection.Facts);
            Assert.All(dataset.Queries.AllRecords, record =>
            {
                Assert.False(string.IsNullOrWhiteSpace(record.MaterialText));
                Assert.False(string.IsNullOrWhiteSpace(record.Provenance.SourcePath));
                Assert.False(string.IsNullOrWhiteSpace(record.Provenance.Anchor));
                Assert.False(string.IsNullOrWhiteSpace(record.Provenance.SourceDigest));
                Assert.False(string.IsNullOrWhiteSpace(record.Provenance.AnchorDigest));
            });

            Assert.Equal(
                new[] { "P1", "NC-01", "NC-02", "NC-03", "NC-04", "NC-05", "NC-06", "NC-07" },
                dataset.Queries.ByPa("pa02", "fixture")
                    .Select(record => record.SourceKey)
                    .OrderBy(FixtureOrder)
                    .ThenBy(value => value, StringComparer.Ordinal)
                    .ToArray());

            Assert.Contains(
                "Privileged/debug metadata is causally non-authoritative",
                dataset.Queries.Fixture("pa04", "NC-02").MaterialText);

            var pa05Negative = dataset.Queries.Fixture("pa05", "NC-02");
            Assert.Contains("HIDDEN_LINEAGE_EPISTEMIC_INVARIANCE", pa05Negative.MaterialText);
            Assert.Equal(
                Enumerable.Range(1, 10).Select(index => "PA-05-H" + index.ToString("00")).ToArray(),
                dataset.Queries.FindingsLinkedToFixture("pa05", "NC-02").Select(record => record.SourceKey).ToArray());

            Assert.Equal(
                new[] { "DL-11", "DL-12", "DL-14" },
                dataset.Queries.FindingsByDispositionFlag("reject")
                    .Where(record => record.PaId == "pa01")
                    .Select(record => record.SourceKey)
                    .OrderBy(value => value, StringComparer.Ordinal)
                    .ToArray());
            Assert.Contains(dataset.Queries.FindingsByDispositionFlag("later"), record => record.PaId == "pa01" && record.SourceKey == "DL-13");
            Assert.Contains(dataset.Queries.FindingsByDispositionFlag("later"), record => record.PaId == "pa02" && record.SourceKey == "AG-09");

            foreach (var flag in new[] { "adopt", "adapt", "later", "reject" })
            {
                Assert.Contains(dataset.Queries.FindingsByDispositionFlag(flag), record => record.PaId == "pa03");
            }

            var pa01Evidence = dataset.Queries.ByPa("pa01", "evidence").Single(record => record.SourceKey == "Donor dossier");
            Assert.Contains("PA-01_NPC_DAILY_LIFE.md", pa01Evidence.MaterialText);
            Assert.Equal(
                dataset.Queries.ByPa("pa01", "finding").Select(record => record.FactId).ToArray(),
                dataset.Queries.FindingsLinkedToEvidenceKey("pa01", "Donor dossier").Select(record => record.FactId).ToArray());

            foreach (var family in new[] { "daily-life", "npc-agency", "social-graph", "knowledge-belief", "rumour-flow" })
            {
                var records = dataset.Queries.ByFailureFamily(family);
                Assert.Contains(records, record => record.RecordKind == "finding");
                Assert.Contains(records, record => record.RecordKind == "fixture");
            }

            Assert.Contains("record=pa05.finding.pa-05-h05", dataset.Queries.BuildCompactIndex());
        }

        [Fact]
        public void ManifestPinsAcceptedSourcesTypesFieldsAndRelations()
        {
            Assert.Equal(6, PaProjectionManifest.Sources.Count);
            Assert.Equal(
                new[]
                {
                    "40e854e4bc2e49630c1f49a6001eb5e60896f3ae",
                    "ce05c64f8bb0cf1f2a0090d35fa1f32757fdd33d",
                    "f2c553e01fb00a4ef2da88e9bc76f644436f0dfe",
                    "d065241d5dd4ed663dd686fa288e6d8a43e1dc46",
                    "d38e8b6261876b28d297893f96150f9f527d7270",
                    "3cfb1b431acd9ffb8e1e1098e195367abd96d553"
                },
                PaProjectionManifest.Sources.Select(source => source.AcceptedBlobSha).ToArray());
            Assert.Contains("pa-finding", PaProjectionManifest.AdoptedEntityTypes);
            Assert.Contains("pa-disposition", PaProjectionManifest.AdoptedEntityTypes);
            Assert.Contains("pa-evidence", PaProjectionManifest.AdoptedEntityTypes);
            Assert.Contains("pa-fixture", PaProjectionManifest.AdoptedEntityTypes);
            Assert.Contains("material-text", PaProjectionManifest.AdoptedFields);
            Assert.Contains("has-disposition", PaProjectionManifest.AdoptedRelations);
            Assert.Contains("same-authority-evidence", PaProjectionManifest.AdoptedRelations);
            Assert.Contains("same-authority-fixture", PaProjectionManifest.AdoptedRelations);
        }

        [Fact]
        public void MutatingAcceptedAuthorityCannotShrinkOrRedefineUniverse()
        {
            var pa01 = Read(PaProjectionManifest.Pa01Path);
            var mutated = pa01.Replace("| DL-01 |", "| DL-X1 |", StringComparison.Ordinal);
            Assert.NotEqual(pa01, mutated);

            var error = Assert.Throws<PaCorpusProjectionException>(() =>
                new PaDesignWorldProvider().BuildAndValidate(SourcesWith(pa01: mutated)));
            Assert.Equal("pa.accepted_source_blob_mismatch", error.MachineCode);
        }

        [Theory]
        [InlineData("| Input | Exact provenance | Juego2 use |", "| Input renamed | Exact provenance | Juego2 use |")]
        [InlineData("| Input | Exact provenance | Juego2 use |", "| Exact provenance | Input | Juego2 use |")]
        public void ProductionParserFailsClosedOnReviewedSchemaRenameOrReorder(string before, string after)
        {
            var pa01 = Read(PaProjectionManifest.Pa01Path);
            var mutated = pa01.Replace(before, after, StringComparison.Ordinal);
            Assert.NotEqual(pa01, mutated);

            var error = Assert.Throws<PaCorpusProjectionException>(() =>
                new PaProductionCorpusParser().Parse(SourcesWith(pa01: mutated)));
            Assert.Equal("pa.source_shape_invalid", error.MachineCode);
        }

        [Fact]
        public void ReverseEnumerationPreservesProjectionAndCompactIndexBytes()
        {
            var sources = ReadSources();
            var forward = new PaDesignWorldProvider(false).BuildAndValidate(sources);
            var reverse = new PaDesignWorldProvider(true).BuildAndValidate(sources);

            Assert.Equal(forward.Projection.Digest, reverse.Projection.Digest);
            Assert.Equal(forward.Projection.NormalizedRepresentation, reverse.Projection.NormalizedRepresentation);
            Assert.Equal(forward.Queries.BuildCompactIndex(), reverse.Queries.BuildCompactIndex());
        }

        [Fact]
        public void ProductionBuildRouteActuallyInvokesSemanticOracle()
        {
            var oracle = new SentinelOracle();
            var error = Assert.Throws<PaCorpusProjectionException>(() =>
                new PaDesignWorldProvider(false, oracle).BuildAndValidate(ReadSources()));

            Assert.True(oracle.WasCalled);
            Assert.Equal("pa.semantic_sentinel", error.MachineCode);
        }

        [Fact]
        public void OmittedFindingIsRedEvenWhenGenericProjectionSelfConfirms()
        {
            AssertOmissionRed(
                fact => fact.FactType == "pa-finding" && Field(fact, "pa-id") == "pa01",
                "pa.semantic_fact_missing");
        }

        [Fact]
        public void OmittedEvidenceItemIsRedEvenWhenGenericProjectionSelfConfirms()
        {
            AssertOmissionRed(
                fact => fact.FactType == "pa-evidence" && Field(fact, "pa-id") == "pa02",
                "pa.semantic_fact_missing");
        }

        [Fact]
        public void EntireDispositionSurfaceCannotDisappearAndSelfConfirm()
        {
            var fixture = BuildFixture();
            var dispositionIds = fixture.Dataset.Projection.Facts
                .Where(fact => fact.FactType == "pa-disposition" && Field(fact, "pa-id") == "pa03")
                .Select(fact => fact.FactId)
                .ToHashSet(StringComparer.Ordinal);
            Assert.NotEmpty(dispositionIds);

            var mutated = Mutate(fixture.Dataset, facts => facts
                .Where(fact => !dispositionIds.Contains(fact.FactId))
                .Select(fact => RemoveRelationsToAny(fact, dispositionIds))
                .ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_fact_missing");
        }

        [Fact]
        public void MaterialDispositionWeakeningIsRedWithFindingIdentityPreserved()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-disposition" && Bool(fact, "contains-reject"));
            var mutated = Mutate(fixture.Dataset, facts => facts.Select(fact =>
            {
                if (fact.FactId != target.FactId) return fact;
                var fields = CopyFields(fact);
                fields["material-text"] = DesignValue.String("ADOPT");
                fields["disposition-text"] = DesignValue.String("ADOPT");
                fields["contains-adopt"] = DesignValue.Boolean(true);
                fields["contains-adapt"] = DesignValue.Boolean(false);
                fields["contains-later"] = DesignValue.Boolean(false);
                fields["contains-reject"] = DesignValue.Boolean(false);
                fields["contains-baseline"] = DesignValue.Boolean(false);
                return CopyFact(fact, fields: fields);
            }).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_content_mismatch");
        }

        [Fact]
        public void LostNegativeFailureModeIsRedWhileOtherCorpusRemainsConsistent()
        {
            AssertOmissionRed(
                fact => fact.FactType == "pa-failure-mode" && Field(fact, "pa-id") == "pa05",
                "pa.semantic_fact_missing");
        }

        [Fact]
        public void OmittedFixtureIsRedWhileFindingAndOtherFixturesRemain()
        {
            AssertOmissionRed(
                fact => fact.FactType == "pa-fixture" && Field(fact, "pa-id") == "pa05" && Field(fact, "source-key") == "NC-02",
                "pa.semantic_fact_missing");
        }

        [Fact]
        public void RemovingRelationWithBothEndpointsPresentIsSemanticRed()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-finding");
            var mutated = MutateOneRelation(fixture.Dataset, target.FactId, relations =>
                relations.Where(relation => relation.RelationType != "has-disposition").ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_relation_missing");
        }

        [Fact]
        public void RenamingRelationWithBothEndpointsPresentIsSemanticRed()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-finding");
            var mutated = MutateOneRelation(fixture.Dataset, target.FactId, relations => relations.Select(relation =>
                relation.RelationType == "has-disposition"
                    ? new DesignRelation("has-disposition-renamed", relation.TargetFactId)
                    : relation).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_relation_mismatch");
        }

        [Fact]
        public void WrongRelationTargetWithSameTypeIsSemanticRed()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-finding" && Field(fact, "pa-id") == "pa05");
            var wrongDisposition = fixture.Dataset.Projection.Facts.First(fact =>
                fact.FactType == "pa-disposition" && !fact.FactId.StartsWith(target.FactId, StringComparison.Ordinal));
            var mutated = MutateOneRelation(fixture.Dataset, target.FactId, relations => relations.Select(relation =>
                relation.RelationType == "has-disposition"
                    ? new DesignRelation("has-disposition", wrongDisposition.FactId)
                    : relation).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_relation_mismatch");
        }

        [Fact]
        public void ExtraDuplicateRelationCardinalityIsSemanticRed()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-finding");
            var mutated = MutateOneRelation(fixture.Dataset, target.FactId, relations =>
            {
                var result = new List<DesignRelation>(relations);
                result.Add(relations.First(relation => relation.RelationType == "has-disposition"));
                return result;
            });
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_relation_extra");
        }

        [Fact]
        public void EqualValuesWithForgedProvenanceAreSemanticRed()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-finding");
            var forged = new DesignAuthorityAnchor(
                "forged-authority",
                target.Provenance.SourcePath,
                target.Provenance.Anchor,
                target.Provenance.SourceDigest,
                target.Provenance.AnchorDigest);
            var mutated = Mutate(fixture.Dataset, facts => facts.Select(fact =>
                fact.FactId == target.FactId ? CopyFact(fact, provenance: forged) : fact).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_provenance_mismatch");
        }

        [Fact]
        public void UnreviewedFutureRecordCannotBecomeAuthoritativeByAppearingInProjection()
        {
            var fixture = BuildFixture();
            var template = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-finding");
            var future = new DesignFact(
                "pa06.finding.unreviewed",
                template.FactType,
                CopyFields(template),
                template.Relations,
                template.Provenance);
            var mutated = Mutate(fixture.Dataset, facts =>
            {
                facts.Add(future);
                return facts;
            });
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_fact_unexpected");
        }

        [Fact]
        public void SmallerCompressionThatOmitsMaterialFactCannotRemainGreen()
        {
            var fixture = BuildFixture();
            var originalIndex = fixture.Dataset.Queries.BuildCompactIndex();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-failure-mode");
            var mutated = Mutate(fixture.Dataset, facts =>
                facts.Where(fact => fact.FactId != target.FactId)
                    .Select(fact => RemoveRelationsTo(fact, target.FactId)).ToList());
            AssertGenericGreen(mutated);
            var reducedIndex = new PaCorpusQueryService(mutated.Projection.Facts).BuildCompactIndex();
            Assert.NotEqual(originalIndex, reducedIndex);
            Assert.True(reducedIndex.Length < originalIndex.Length);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_fact_missing");
        }

        private static void AssertOmissionRed(Func<DesignFact, bool> selector, string code)
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(selector);
            var mutated = Mutate(fixture.Dataset, facts => facts
                .Where(fact => fact.FactId != target.FactId)
                .Select(fact => RemoveRelationsTo(fact, target.FactId)).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, code);
        }

        private static CorpusFixture BuildFixture()
        {
            var sources = ReadSources();
            return new CorpusFixture(sources, new PaDesignWorldProvider().BuildAndValidate(sources));
        }

        private static MutatedProjection Mutate(PaCorpusDataset dataset, Func<List<DesignFact>, List<DesignFact>> mutate)
        {
            var facts = mutate(dataset.Projection.Facts.ToList());
            var universe = new StaticDesignAuthorityUniverse(facts.Select(fact => fact.FactId));
            var reader = new SelfConfirmingReader(facts);
            var projection = new DesignWorldProjector().Build(universe, reader, dataset.Projection.Version);
            return new MutatedProjection(projection, universe, reader);
        }

        private static MutatedProjection MutateOneRelation(
            PaCorpusDataset dataset,
            string factId,
            Func<IReadOnlyList<DesignRelation>, IReadOnlyList<DesignRelation>> mutate)
        {
            return Mutate(dataset, facts => facts.Select(fact =>
                fact.FactId == factId ? CopyFact(fact, relations: mutate(fact.Relations)) : fact).ToList());
        }

        private static void AssertGenericGreen(MutatedProjection mutated)
        {
            var report = new DesignWorldProjectionValidator().Validate(
                mutated.Projection, mutated.Universe, mutated.Reader, mutated.Projection.Version);
            Assert.True(report.IsValid, string.Join("\n", report.Issues.Select(issue => issue.MachineCode + ":" + issue.Detail)));
        }

        private static void AssertOracleRed(PaAcceptedCorpusSources sources, DesignWorldProjection projection, string machineCode)
        {
            var report = new PaSourceCorpusOracle().Validate(projection, sources);
            Assert.False(report.IsValid);
            Assert.Contains(report.Issues, issue => issue.MachineCode == machineCode);
        }

        private static DesignFact RemoveRelationsTo(DesignFact fact, string targetId)
        {
            var relations = fact.Relations.Where(relation => relation.TargetFactId != targetId).ToList();
            return relations.Count == fact.Relations.Count ? fact : CopyFact(fact, relations: relations);
        }

        private static DesignFact RemoveRelationsToAny(DesignFact fact, ISet<string> targetIds)
        {
            var relations = fact.Relations.Where(relation => !targetIds.Contains(relation.TargetFactId)).ToList();
            return relations.Count == fact.Relations.Count ? fact : CopyFact(fact, relations: relations);
        }

        private static SortedDictionary<string, DesignValue> CopyFields(DesignFact fact)
        {
            var copy = new SortedDictionary<string, DesignValue>(StringComparer.Ordinal);
            foreach (var pair in fact.Fields) copy.Add(pair.Key, pair.Value);
            return copy;
        }

        private static DesignFact CopyFact(
            DesignFact fact,
            IReadOnlyDictionary<string, DesignValue>? fields = null,
            IEnumerable<DesignRelation>? relations = null,
            DesignAuthorityAnchor? provenance = null)
        {
            return new DesignFact(
                fact.FactId,
                fact.FactType,
                fields ?? CopyFields(fact),
                relations ?? fact.Relations,
                provenance ?? fact.Provenance);
        }

        private static string Field(DesignFact fact, string name) => fact.Fields[name].CanonicalValue;
        private static bool Bool(DesignFact fact, string name) => fact.Fields[name].CanonicalValue == "true";

        private static int FixtureOrder(string key)
        {
            if (key == "P1") return 0;
            if (key.StartsWith("NC-", StringComparison.Ordinal)) return 10 + int.Parse(key.Substring(3));
            return 100;
        }

        private static PaAcceptedCorpusSources SourcesWith(string? pa01 = null)
        {
            return new PaAcceptedCorpusSources(
                pa01 ?? Read(PaProjectionManifest.Pa01Path),
                Read(PaProjectionManifest.Pa02Path),
                Read(PaProjectionManifest.Pa03Path),
                Read(PaProjectionManifest.Pa04Path),
                Read(PaProjectionManifest.Pa05Path),
                Read(PaProjectionManifest.Pa05FixturesPath));
        }

        private static PaAcceptedCorpusSources ReadSources() => SourcesWith();

        private static string Read(string relativePath)
        {
            var root = FindRepositoryRoot();
            return File.ReadAllText(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));
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

        private sealed class SentinelOracle : IPaCorpusSemanticOracle
        {
            public bool WasCalled { get; private set; }

            public PaCorpusSemanticReport Validate(DesignWorldProjection projection, PaAcceptedCorpusSources sources)
            {
                WasCalled = true;
                return new PaCorpusSemanticReport(new[]
                {
                    new PaCorpusSemanticIssue(
                        "pa.semantic_sentinel", "sentinel", "wiring", "semantic oracle sentinel", Array.Empty<string>())
                });
            }
        }

        private sealed class SelfConfirmingReader : IDesignAuthorityReader
        {
            private readonly IReadOnlyDictionary<string, DesignFact> _facts;

            public SelfConfirmingReader(IEnumerable<DesignFact> facts)
            {
                _facts = facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);
            }

            public DesignAuthorityReadResult Read(string factId) =>
                _facts.TryGetValue(factId, out var fact)
                    ? DesignAuthorityReadResult.Found(fact)
                    : DesignAuthorityReadResult.Failure(DesignAuthorityResolutionStatus.Missing, "missing");

            public DesignAuthorityResolutionStatus Resolve(DesignAuthorityAnchor anchor) => DesignAuthorityResolutionStatus.Found;
        }

        private sealed class CorpusFixture
        {
            public CorpusFixture(PaAcceptedCorpusSources sources, PaCorpusDataset dataset)
            {
                Sources = sources;
                Dataset = dataset;
            }

            public PaAcceptedCorpusSources Sources { get; }
            public PaCorpusDataset Dataset { get; }
        }

        private sealed class MutatedProjection
        {
            public MutatedProjection(DesignWorldProjection projection, IDesignAuthorityUniverse universe, IDesignAuthorityReader reader)
            {
                Projection = projection;
                Universe = universe;
                Reader = reader;
            }

            public DesignWorldProjection Projection { get; }
            public IDesignAuthorityUniverse Universe { get; }
            public IDesignAuthorityReader Reader { get; }
        }
    }
}
