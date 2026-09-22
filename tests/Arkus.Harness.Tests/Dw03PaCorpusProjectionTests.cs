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
            var sources = ReadSources();
            var dataset = new PaDesignWorldProvider().BuildAndValidate(sources);

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
                new[] { "P1","NC-01","NC-02","NC-03","NC-04","NC-05","NC-06","NC-07" },
                dataset.Queries.ByPa("pa02", "fixture").Select(record => record.SourceKey).OrderBy(value => FixtureOrder(value)).ThenBy(value => value, StringComparer.Ordinal).ToArray());

            var pa04Negative = dataset.Queries.Fixture("pa04", "NC-02");
            Assert.Contains("Privileged/debug metadata is causally non-authoritative", pa04Negative.MaterialText);

            var pa05Negative = dataset.Queries.Fixture("pa05", "NC-02");
            Assert.Contains("HIDDEN_LINEAGE_EPISTEMIC_INVARIANCE", pa05Negative.MaterialText);
            Assert.Equal(
                Enumerable.Range(1,10).Select(index => "PA-05-H" + index.ToString("00")).ToArray(),
                dataset.Queries.FindingsLinkedToFixture("pa05", "NC-02").Select(record => record.SourceKey).ToArray());

            var reject = dataset.Queries.FindingsByDispositionFlag("reject");
            var later = dataset.Queries.FindingsByDispositionFlag("later");
            Assert.Contains(reject, record => record.PaId == "pa01" && record.SourceKey == "DL-11");
            Assert.Contains(reject, record => record.PaId == "pa04" && record.DispositionText.Contains("REJECT", StringComparison.Ordinal));
            Assert.Contains(later, record => record.PaId == "pa02" && record.SourceKey == "AG-09");
            Assert.All(reject, record => Assert.Contains("REJECT", record.DispositionText, StringComparison.OrdinalIgnoreCase));

            foreach (var family in new[] { "daily-life", "npc-agency", "social-graph", "knowledge-belief", "rumour-flow" })
            {
                Assert.NotEmpty(dataset.Queries.ByFailureFamily(family));
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
        public void MutatingAcceptedAuthorityBytesCannotShrinkOrRedefineUniverse()
        {
            var original = ReadSources();
            var pa01 = Read(PaProjectionManifest.Pa01Path);
            var mutated = pa01.Replace("| DL-01 |", "| DL-X1 |", StringComparison.Ordinal);
            Assert.NotEqual(pa01, mutated);
            var sources = new PaAcceptedCorpusSources(
                mutated,
                Read(PaProjectionManifest.Pa02Path),
                Read(PaProjectionManifest.Pa03Path),
                Read(PaProjectionManifest.Pa04Path),
                Read(PaProjectionManifest.Pa05Path),
                Read(PaProjectionManifest.Pa05FixturesPath));

            var error = Assert.Throws<PaCorpusProjectionException>(() => new PaDesignWorldProvider().BuildAndValidate(sources));
            Assert.Equal("pa.accepted_source_blob_mismatch", error.MachineCode);
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
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-finding" && Field(fact, "pa-id") == "pa01");
            var mutated = Mutate(fixture.Dataset, facts =>
                facts.Where(fact => fact.FactId != target.FactId)
                    .Select(fact => RemoveRelationsTo(fact, target.FactId)).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_fact_missing");
        }

        [Fact]
        public void OmittedEvidenceItemIsRedEvenWhenGenericProjectionSelfConfirms()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-evidence" && Field(fact, "pa-id") == "pa02");
            var mutated = Mutate(fixture.Dataset, facts =>
                facts.Where(fact => fact.FactId != target.FactId)
                    .Select(fact => RemoveRelationsTo(fact, target.FactId)).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_fact_missing");
        }

        [Fact]
        public void MaterialDispositionWeakeningIsRedWithFindingIdentityPreserved()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-disposition" && Bool(target: fact, "contains-reject"));
            var mutated = Mutate(fixture.Dataset, facts => facts.Select(fact =>
            {
                if (fact.FactId != target.FactId) return fact;
                var fields = CopyFields(fact);
                fields["disposition-text"] = DesignValue.String("ADOPT");
                fields["contains-adopt"] = DesignValue.Boolean(true);
                fields["contains-reject"] = DesignValue.Boolean(false);
                return CopyFact(fact, fields: fields);
            }).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_content_mismatch");
        }

        [Fact]
        public void LostNegativeFailureModeIsRedWhileOtherCorpusRemainsConsistent()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-failure-mode" && Field(fact, "pa-id") == "pa05");
            var mutated = Mutate(fixture.Dataset, facts =>
                facts.Where(fact => fact.FactId != target.FactId)
                    .Select(fact => RemoveRelationsTo(fact, target.FactId)).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_fact_missing");
        }

        [Fact]
        public void OmittedFixtureIsRedWhileFindingAndOtherFixturesRemain()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.Single(fact => fact.FactType == "pa-fixture" && Field(fact, "pa-id") == "pa05" && Field(fact, "source-key") == "NC-02");
            var mutated = Mutate(fixture.Dataset, facts =>
                facts.Where(fact => fact.FactId != target.FactId)
                    .Select(fact => RemoveRelationsTo(fact, target.FactId)).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_fact_missing");
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
                relation.RelationType == "has-disposition" ? new DesignRelation("has-disposition-renamed", relation.TargetFactId) : relation).ToList());
            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "pa.semantic_relation_mismatch");
        }

        [Fact]
        public void WrongRelationTargetWithSameTypeIsSemanticRed()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-finding" && Field(fact, "pa-id") == "pa05");
            var wrongDisposition = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "pa-disposition" && !fact.FactId.StartsWith(target.FactId, StringComparison.Ordinal));
            var mutated = MutateOneRelation(fixture.Dataset, target.FactId, relations => relations.Select(relation =>
                relation.RelationType == "has-disposition" ? new DesignRelation("has-disposition", wrongDisposition.FactId) : relation).ToList());
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

        private static MutatedProjection MutateOneRelation(PaCorpusDataset dataset, string factId, Func<IReadOnlyList<DesignRelation>, IReadOnlyList<DesignRelation>> mutate)
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

        private static SortedDictionary<string, DesignValue> CopyFields(DesignFact fact) =>
            new SortedDictionary<string, DesignValue>(fact.Fields, StringComparer.Ordinal);

        private static DesignFact CopyFact(
            DesignFact fact,
            IDictionary<string, DesignValue> fields = null,
            IEnumerable<DesignRelation> relations = null,
            DesignAuthorityAnchor provenance = null)
        {
            return new DesignFact(
                fact.FactId,
                fact.FactType,
                fields ?? new SortedDictionary<string, DesignValue>(fact.Fields, StringComparer.Ordinal),
                relations ?? fact.Relations,
                provenance ?? fact.Provenance);
        }

        private static string Field(DesignFact fact, string name) => fact.Fields[name].CanonicalValue;
        private static bool Bool(DesignFact target, string name) => target.Fields[name].CanonicalValue == "true";

        private static int FixtureOrder(string key)
        {
            if (key == "P1") return 0;
            if (key.StartsWith("NC-", StringComparison.Ordinal)) return 10 + int.Parse(key.Substring(3));
            return 100;
        }

        private static PaAcceptedCorpusSources ReadSources() => new PaAcceptedCorpusSources(
            Read(PaProjectionManifest.Pa01Path),
            Read(PaProjectionManifest.Pa02Path),
            Read(PaProjectionManifest.Pa03Path),
            Read(PaProjectionManifest.Pa04Path),
            Read(PaProjectionManifest.Pa05Path),
            Read(PaProjectionManifest.Pa05FixturesPath));

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
                    new PaCorpusSemanticIssue("pa.semantic_sentinel", "sentinel", "wiring", "semantic oracle sentinel", Array.Empty<string>())
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
            public CorpusFixture(PaAcceptedCorpusSources sources, PaCorpusDataset dataset) { Sources = sources; Dataset = dataset; }
            public PaAcceptedCorpusSources Sources { get; }
            public PaCorpusDataset Dataset { get; }
        }

        private sealed class MutatedProjection
        {
            public MutatedProjection(DesignWorldProjection projection, IDesignAuthorityUniverse universe, IDesignAuthorityReader reader)
            { Projection=projection; Universe=universe; Reader=reader; }
            public DesignWorldProjection Projection { get; }
            public IDesignAuthorityUniverse Universe { get; }
            public IDesignAuthorityReader Reader { get; }
        }
    }
}