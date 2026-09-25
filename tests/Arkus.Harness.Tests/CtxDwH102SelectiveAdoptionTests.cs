using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class CtxDwH102SelectiveAdoptionTests
    {
        [Fact]
        public void AcceptedProjectionPublishesOnlyItsRealCapabilitiesAndCurrentSourceOpenQueries()
        {
            var root = FindRepositoryRoot();
            var sources = ReadSources(root);
            var dataset = new H1DesignWorldProvider().BuildAndValidate(sources);
            var current = new H1ProjectionLifecycleGuard().ValidateCurrent(
                dataset,
                sources,
                H1ProjectionManifest.AcceptedH104CandidateSha,
                H1ProjectionManifest.CurrentProjectionVersion);
            Assert.True(current.IsCurrent);

            var staleProbe = new H1ProjectionLifecycleGuard().ValidateCurrent(
                dataset,
                sources,
                H1ProjectionManifest.AcceptedH104CandidateSha,
                new DesignProjectionVersion(2, "ctx-dw-h1-02-stale-probe"));
            Assert.False(staleProbe.IsCurrent);
            Assert.Contains(staleProbe.Issues, issue =>
                StringComparer.Ordinal.Equals(issue.MachineCode, "h1.lifecycle_projection_schema_stale"));

            var representative = dataset.Projection.Facts.Single(fact =>
                StringComparer.Ordinal.Equals(
                    fact.FactId,
                    "quaternius.medieval.prefab.wall-plaster-window-wide-flat"));
            Assert.Equal("h1-catalogue-entry", representative.FactType);
            Assert.Equal("prefab", representative.Fields["kind"].CanonicalValue);
            Assert.Equal("quaternius-medieval-source", representative.Fields["source-id"].CanonicalValue);
            Assert.Equal(H1ProjectionManifest.CataloguePath, representative.Provenance.SourcePath);
            Assert.Contains(representative.Relations, relation =>
                StringComparer.Ordinal.Equals(relation.RelationType, "adopted-from-source") &&
                StringComparer.Ordinal.Equals(
                    relation.TargetFactId,
                    "h1.source.quaternius-medieval-source"));

            var componentSchemas = dataset.Projection.Facts
                .Where(fact =>
                    StringComparer.Ordinal.Equals(fact.FactType, "h1-catalogue-entry") &&
                    fact.Fields.TryGetValue("kind", out var kind) &&
                    StringComparer.Ordinal.Equals(kind.CanonicalValue, "component-schema"))
                .OrderBy(fact => fact.FactId, StringComparer.Ordinal)
                .ToArray();
            Assert.NotEmpty(componentSchemas);
            Assert.All(componentSchemas, fact => Assert.Equal(H1ProjectionManifest.CataloguePath, fact.Provenance.SourcePath));

            var capabilities = new SortedSet<string>(StringComparer.Ordinal);
            if (dataset.Projection.Facts.Any(fact => StringComparer.Ordinal.Equals(fact.FactType, "h1-catalogue-entry")))
                capabilities.Add("catalogue-entry");
            if (dataset.Projection.Facts.Any(fact => StringComparer.Ordinal.Equals(fact.FactType, "h1-source")))
                capabilities.Add("source-record");
            if (dataset.Projection.Facts.Any(fact => StringComparer.Ordinal.Equals(fact.FactType, "h1-source-slice")))
                capabilities.Add("source-slice");
            if (dataset.Projection.Facts.SelectMany(fact => fact.Relations).Any(relation =>
                    StringComparer.Ordinal.Equals(relation.RelationType, "declared-in")))
                capabilities.Add("catalogue-declared-in");
            if (dataset.Projection.Facts.SelectMany(fact => fact.Relations).Any(relation =>
                    StringComparer.Ordinal.Equals(relation.RelationType, "adopted-from-source")))
                capabilities.Add("adopted-from-source");
            if (componentSchemas.Length > 0)
                capabilities.Add("component-schema-entry");

            Assert.DoesNotContain("effective-scene-membership", capabilities);
            Assert.DoesNotContain("active-generation-publication", capabilities);
            Assert.DoesNotContain("effective-prefab-relationship-multiset", capabilities);
            Assert.DoesNotContain("effective-component-adapter", capabilities);
            Assert.DoesNotContain("component-field-roundtrip", capabilities);

            WriteObservation(root, dataset, representative, componentSchemas, capabilities, staleProbe);
        }

        private static void WriteObservation(
            string root,
            H1CatalogueDataset dataset,
            DesignFact representative,
            IReadOnlyList<DesignFact> componentSchemas,
            IEnumerable<string> capabilities,
            H1ProjectionLifecycleReport staleProbe)
        {
            var outputPath = Environment.GetEnvironmentVariable("CTX_DW_H1_02_OBSERVATION_OUTPUT");
            if (string.IsNullOrWhiteSpace(outputPath)) return;

            var representativePayload = FactPayload(representative);
            var componentPayload = FactPayload(componentSchemas[0]);
            var representativeJson = JsonSerializer.Serialize(representativePayload);
            var componentJson = JsonSerializer.Serialize(componentPayload);
            var catalogueText = File.ReadAllText(Path.Combine(
                root,
                H1ProjectionManifest.CataloguePath.Replace('/', Path.DirectorySeparatorChar)));

            var observation = new SortedDictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schema"] = "ctx-dw-h1-02-projection-observation-v1",
                ["projection_identity"] = dataset.ProjectionIdentity,
                ["projection_digest"] = dataset.Projection.Digest,
                ["projection_version"] = dataset.Projection.Version.RuleVersion,
                ["lifecycle_current"] = true,
                ["stale_probe_current"] = false,
                ["stale_probe_codes"] = staleProbe.Issues.Select(issue => issue.MachineCode).Distinct(StringComparer.Ordinal).OrderBy(code => code, StringComparer.Ordinal).ToArray(),
                ["capabilities"] = capabilities.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                ["adopted_fact_types"] = dataset.Projection.Facts.Select(fact => fact.FactType).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                ["adopted_relation_types"] = dataset.Projection.Facts.SelectMany(fact => fact.Relations).Select(relation => relation.RelationType).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                ["component_schema_count"] = componentSchemas.Count,
                ["component_schema_examples"] = componentSchemas.Take(3).Select(fact => fact.FactId).ToArray(),
                ["representative_prefab"] = representativePayload,
                ["representative_component_schema"] = componentPayload,
                ["representative_prefab_query_bytes"] = Encoding.UTF8.GetByteCount(representativeJson),
                ["representative_component_schema_query_bytes"] = Encoding.UTF8.GetByteCount(componentJson),
                ["catalogue_authority_bytes"] = Encoding.UTF8.GetByteCount(catalogueText),
                ["normalized_projection_bytes"] = Encoding.UTF8.GetByteCount(dataset.Projection.NormalizedRepresentation)
            };

            var fullPath = Path.GetFullPath(outputPath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(
                fullPath,
                JsonSerializer.Serialize(observation, new JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine);
        }

        private static SortedDictionary<string, object?> FactPayload(DesignFact fact) =>
            new SortedDictionary<string, object?>(StringComparer.Ordinal)
            {
                ["fact_id"] = fact.FactId,
                ["fact_type"] = fact.FactType,
                ["fields"] = fact.Fields.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.CanonicalValue,
                    StringComparer.Ordinal),
                ["relations"] = fact.Relations.Select(relation => new SortedDictionary<string, string>(StringComparer.Ordinal)
                {
                    ["type"] = relation.RelationType,
                    ["target"] = relation.TargetFactId
                }).ToArray(),
                ["provenance_source_path"] = fact.Provenance.SourcePath,
                ["provenance_source_digest"] = fact.Provenance.SourceDigest,
                ["provenance_anchor_digest"] = fact.Provenance.AnchorDigest
            };

        private static H1AcceptedAuthoritySources ReadSources(string root) => new H1AcceptedAuthoritySources(
            Read(root, H1ProjectionManifest.CataloguePath),
            Read(root, H1ProjectionManifest.SourceAdoptionPath));

        private static string Read(string root, string relativePath) =>
            File.ReadAllText(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));

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
    }
}
