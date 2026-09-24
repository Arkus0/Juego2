using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class CtxDwH101ProjectionLifecycleTests
    {
        [Fact]
        public void ManifestPinsExactAcceptedH104Authority()
        {
            Assert.Equal("8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5", H1ProjectionManifest.AcceptedH104CandidateSha);
            Assert.Equal("ctx-dw-h1-01-adapter-v1", H1ProjectionManifest.AdapterId);
            Assert.Equal("ctx-dw-h1-01-lifecycle-v1", H1ProjectionManifest.LifecycleId);
            Assert.Equal(2, H1ProjectionManifest.Sources.Count);
            Assert.Equal(
                new[]
                {
                    "922dbdffbe2f0a622acc2e153ca8b181427f6ce6",
                    "664e83e25269f345a248ce43410a28ed0a670750"
                },
                H1ProjectionManifest.Sources.Select(source => source.AcceptedBlobSha).ToArray());
            Assert.Equal("arkus.h1-catalogue-mapping@1", H1ProjectionManifest.Sources[0].SchemaId);
            Assert.Equal("arkus.h1-04-source-adoption@1", H1ProjectionManifest.Sources[1].SchemaId);
        }

        [Fact]
        public void AcceptedH104UniverseBuildsCompleteSourceOpenProjection()
        {
            var dataset = new H1DesignWorldProvider().BuildAndValidate(ReadSources());

            Assert.Equal(H1ProjectionManifest.AdapterId, dataset.AdapterId);
            Assert.Equal(H1ProjectionManifest.LifecycleId, dataset.LifecycleId);
            Assert.Equal(H1ProjectionManifest.AcceptedH104CandidateSha, dataset.AcceptedH104CandidateSha);
            Assert.StartsWith(H1ProjectionManifest.AdapterId + ":", dataset.ProjectionIdentity, StringComparison.Ordinal);
            Assert.True(dataset.Projection.Facts.Count > 10);
            Assert.All(dataset.Projection.Facts, fact =>
            {
                Assert.Contains(fact.FactType, H1ProjectionManifest.AdoptedEntityTypes);
                Assert.Contains(fact.Provenance.SourcePath, H1ProjectionManifest.Sources.Select(source => source.SourcePath));
                Assert.False(string.IsNullOrWhiteSpace(fact.Provenance.Anchor));
                Assert.Equal(64, fact.Provenance.SourceDigest.Length);
                Assert.Equal(64, fact.Provenance.AnchorDigest.Length);
            });

            var prefab = dataset.Projection.Facts.Single(fact => fact.FactId == "quaternius.medieval.prefab.wall-plaster-window-wide-flat");
            Assert.Equal("h1-catalogue-entry", prefab.FactType);
            Assert.Equal("prefab", prefab.Fields["kind"].CanonicalValue);
            Assert.Equal("quaternius-medieval-source", prefab.Fields["source-id"].CanonicalValue);
            Assert.Equal("approved-source", prefab.Fields["adoption-status"].CanonicalValue);
            Assert.Contains(prefab.Relations, relation =>
                relation.RelationType == "adopted-from-source" && relation.TargetFactId == "h1.source.quaternius-medieval-source");

            var medieval = dataset.Projection.Facts.Single(fact => fact.FactId == "h1.source.quaternius-medieval-source");
            Assert.Equal("CC0-1.0", medieval.Fields["license-id"].CanonicalValue);
            Assert.Equal("true", medieval.Fields["commercial-use"].CanonicalValue);
            Assert.Contains(dataset.Projection.Facts, fact => fact.FactType == "h1-source-slice");
        }

        [Fact]
        public void IndependentOracleAcceptsProductionProjection()
        {
            var sources = ReadSources();
            var dataset = new H1DesignWorldProvider().BuildAndValidate(sources);
            var report = new H1SourceAuthorityOracle().Validate(dataset.Projection, sources);

            Assert.True(report.IsValid, string.Join("\n", report.Issues.Select(issue => issue.MachineCode + ":" + issue.Detail)));
        }

        [Fact]
        public void ReversingAdapterEnumerationRebuildsByteIdenticalProjection()
        {
            var sources = ReadSources();
            var forward = new H1DesignWorldProvider(false).BuildAndValidate(sources);
            var reverse = new H1DesignWorldProvider(true).BuildAndValidate(sources);

            Assert.Equal(forward.Projection.Digest, reverse.Projection.Digest);
            Assert.Equal(forward.Projection.NormalizedRepresentation, reverse.Projection.NormalizedRepresentation);
            Assert.Equal(forward.ProjectionIdentity, reverse.ProjectionIdentity);
        }

        [Fact]
        public void MaterialAuthorityDriftIsStaleBeforeProjectionCanSelfConfirm()
        {
            var original = Read(H1ProjectionManifest.CataloguePath);
            var mutated = original.Replace(
                "\"adoptionStatus\": \"approved-source\"",
                "\"adoptionStatus\": \"source-derived\"",
                StringComparison.Ordinal);
            Assert.NotEqual(original, mutated);

            var error = Assert.Throws<H1CatalogueProjectionException>(() =>
                new H1DesignWorldProvider().BuildAndValidate(new H1AcceptedAuthoritySources(
                    mutated,
                    Read(H1ProjectionManifest.SourceAdoptionPath))));

            Assert.Equal("h1.accepted_source_blob_mismatch", error.MachineCode);
            Assert.Equal(H1ProjectionManifest.CataloguePath, error.SubjectId);
        }

        [Fact]
        public void OmittedCatalogueFactIsRedEvenWhenGenericProjectionSelfConfirms()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "h1-catalogue-entry");
            var mutated = Mutate(fixture.Dataset, facts => facts.Where(fact => fact.FactId != target.FactId).ToList());

            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "h1.semantic_fact_missing");
        }

        [Fact]
        public void MaterialFieldWeakeningIsRedWithIdentityPreserved()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.Single(fact => fact.FactId == "quaternius.medieval.prefab.wall-plaster-window-wide-flat");
            var mutated = Mutate(fixture.Dataset, facts => facts.Select(fact =>
            {
                if (fact.FactId != target.FactId) return fact;
                var fields = CopyFields(fact);
                fields["adoption-status"] = DesignValue.String("unreviewed");
                return CopyFact(fact, fields: fields);
            }).ToList());

            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "h1.semantic_content_mismatch");
        }

        [Fact]
        public void MissingSourceRelationIsRedWithBothEndpointsPresent()
        {
            var fixture = BuildFixture();
            var targetId = "quaternius.medieval.prefab.wall-plaster-window-wide-flat";
            var mutated = Mutate(fixture.Dataset, facts => facts.Select(fact =>
            {
                if (fact.FactId != targetId) return fact;
                return CopyFact(fact, relations: fact.Relations.Where(relation => relation.RelationType != "adopted-from-source"));
            }).ToList());

            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "h1.semantic_relation_missing");
        }

        [Fact]
        public void EqualValuesWithForgedProvenanceAreRed()
        {
            var fixture = BuildFixture();
            var target = fixture.Dataset.Projection.Facts.First(fact => fact.FactType == "h1-catalogue-entry");
            var forged = new DesignAuthorityAnchor(
                "forged-h1-authority",
                target.Provenance.SourcePath,
                target.Provenance.Anchor,
                target.Provenance.SourceDigest,
                target.Provenance.AnchorDigest);
            var mutated = Mutate(fixture.Dataset, facts => facts.Select(fact =>
                fact.FactId == target.FactId ? CopyFact(fact, provenance: forged) : fact).ToList());

            AssertGenericGreen(mutated);
            AssertOracleRed(fixture.Sources, mutated.Projection, "h1.semantic_provenance_mismatch");
        }

        [Fact]
        public void ProjectionSchemaChangeMarksCachedProjectionStale()
        {
            var dataset = new H1DesignWorldProvider().BuildAndValidate(ReadSources());
            var universe = new StaticDesignAuthorityUniverse(dataset.Projection.FactIds);
            var reader = new SelfConfirmingReader(dataset.Projection.Facts);
            var future = new DesignProjectionVersion(2, "ctx-dw-h1-01-v2");

            var report = new DesignWorldProjectionValidator().Validate(dataset.Projection, universe, reader, future);

            Assert.False(report.IsValid);
            Assert.Contains(report.Issues, issue => issue.MachineCode == "dw.projection_version_stale");
        }

        [Fact]
        public void H1VocabularyDoesNotLeakIntoGenericDesignWorldKernel()
        {
            var contracts = Read("src/Arkus.DesignWorld/DesignWorldContracts.cs");
            var projection = Read("src/Arkus.DesignWorld/DesignWorldProjection.cs");
            var generic = contracts + "\n" + projection;

            Assert.True(generic.IndexOf("quaternius", StringComparison.OrdinalIgnoreCase) < 0);
            Assert.True(generic.IndexOf("h1-catalogue", StringComparison.OrdinalIgnoreCase) < 0);
            Assert.True(generic.IndexOf("h1-source", StringComparison.OrdinalIgnoreCase) < 0);
            Assert.Equal("dw.fact", DesignWorldProjector.GenericWorldType);
        }

        [Fact]
        public void H105RemainsDependentOnAuthoritativeH104NotThisDerivedProjection()
        {
            var h105 = Read("Docs/workpacks/H1/WP-H1-05.md");
            Assert.Contains("Depends on: `WP-H1-04` PASS", h105, StringComparison.Ordinal);
            Assert.DoesNotContain("Depends on: `WP-CTX-DW-H1-01`", h105, StringComparison.Ordinal);
            Assert.Contains("Canonical state remains authoritative", h105, StringComparison.Ordinal);
        }

        private static CorpusFixture BuildFixture()
        {
            var sources = ReadSources();
            return new CorpusFixture(sources, new H1DesignWorldProvider().BuildAndValidate(sources));
        }

        private static MutatedProjection Mutate(H1CatalogueDataset dataset, Func<List<DesignFact>, List<DesignFact>> mutation)
        {
            var facts = mutation(dataset.Projection.Facts.ToList());
            var universe = new StaticDesignAuthorityUniverse(facts.Select(fact => fact.FactId));
            var reader = new SelfConfirmingReader(facts);
            var projection = new DesignWorldProjector().Build(universe, reader, dataset.Projection.Version);
            return new MutatedProjection(projection, universe, reader);
        }

        private static void AssertGenericGreen(MutatedProjection mutated)
        {
            var report = new DesignWorldProjectionValidator().Validate(
                mutated.Projection, mutated.Universe, mutated.Reader, mutated.Projection.Version);
            Assert.True(report.IsValid, string.Join("\n", report.Issues.Select(issue => issue.MachineCode + ":" + issue.Detail)));
        }

        private static void AssertOracleRed(H1AcceptedAuthoritySources sources, DesignWorldProjection projection, string machineCode)
        {
            var report = new H1SourceAuthorityOracle().Validate(projection, sources);
            Assert.False(report.IsValid);
            Assert.Contains(report.Issues, issue => issue.MachineCode == machineCode);
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

        private static H1AcceptedAuthoritySources ReadSources() => new H1AcceptedAuthoritySources(
            Read(H1ProjectionManifest.CataloguePath),
            Read(H1ProjectionManifest.SourceAdoptionPath));

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
            public CorpusFixture(H1AcceptedAuthoritySources sources, H1CatalogueDataset dataset)
            {
                Sources = sources;
                Dataset = dataset;
            }

            public H1AcceptedAuthoritySources Sources { get; }
            public H1CatalogueDataset Dataset { get; }
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