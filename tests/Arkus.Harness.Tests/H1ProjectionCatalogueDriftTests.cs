using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Arkus.H1.UnityHost;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1ProjectionCatalogueDriftTests
    {
        [Fact]
        public void RemovingOneAdmittedCatalogueEntry_IsCatalogueDriftAndNeverCanonicalRewrite()
        {
            var root = RepositoryRoot();
            var effectivePath = Path.Combine(root, "Docs", "evidence", "WP-H1-04", "EFFECTIVE_INVENTORY.json");
            var mappingPath = Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath);
            var adoptionPath = Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath);
            var effectiveText = File.ReadAllText(effectivePath);
            var mappingText = File.ReadAllText(mappingPath);
            var adoptionText = File.ReadAllText(adoptionPath);
            var before = H1CatalogueSnapshot.Build(effectiveText, mappingText, adoptionText);

            var mapping = JsonNode.Parse(mappingText)!.AsObject();
            var mappingEntries = mapping["entries"]!.AsArray();
            var removedMapping = mappingEntries
                .OfType<JsonObject>()
                .First(value => value["kind"]!.GetValue<string>() == "animation-clip");
            var removedGuid = removedMapping["nativeGuid"]!.GetValue<string>();
            var removedLocalFileId = removedMapping["localFileId"]!.GetValue<string>();
            var removedTypeName = removedMapping["typeName"]!.GetValue<string>();
            mappingEntries.Remove(removedMapping);

            var effective = JsonNode.Parse(effectiveText)!.AsObject();
            var effectiveRows = effective["rows"]!.AsArray();
            var removedEffective = effectiveRows
                .OfType<JsonObject>()
                .Single(value =>
                    value["kind"]!.GetValue<string>() == "animation-clip" &&
                    value["nativeGuid"]!.GetValue<string>() == removedGuid &&
                    value["localFileId"]!.GetValue<string>() == removedLocalFileId &&
                    value["typeName"]!.GetValue<string>() == removedTypeName);
            effectiveRows.Remove(removedEffective);

            var after = H1CatalogueSnapshot.Build(effective.ToJsonString(), mapping.ToJsonString(), adoptionText);
            Assert.Equal(before.Entries.Count - 1, after.Entries.Count);
            Assert.NotEqual(before.Fingerprint, after.Fingerprint);

            var plan = new H1ManagedScenePlan
            {
                WorldId = "world.h1-09.catalogue-removal",
                WorldRevision = 7,
                CanonicalHash = H1ManagedScenePlan.Sha("catalogue-removal-canonical"),
                InputDigest = H1ManagedScenePlan.Sha("catalogue-removal-input"),
                CatalogueFingerprint = after.Fingerprint,
                Nodes = Array.Empty<H1ManagedSceneNode>()
            };
            var effectiveObservation = new H1ProjectionReconciliationObservation
            {
                Active = true,
                GenerationId = new string('4', 32),
                CanonicalHash = plan.CanonicalHash,
                InputDigest = plan.InputDigest,
                CatalogueFingerprint = before.Fingerprint,
                ManifestGraphDigest = H1ManagedScenePlan.Sha("empty-graph"),
                GraphDigest = H1ManagedScenePlan.Sha("empty-graph"),
                ManifestRealizationDigest = H1ManagedScenePlan.Sha("empty-realization"),
                RealizationDigest = H1ManagedScenePlan.Sha("empty-realization"),
                Nodes = Array.Empty<H1ObservedSceneNode>(),
                ManagedDigest = H1ProjectionReconciliation.ManagedDigest(Array.Empty<H1ObservedSceneNode>())
            };

            var report = H1ProjectionReconciliation.Compare(plan, effectiveObservation);
            Assert.False(report.Parity);
            Assert.Equal("catalogue-drift", report.State);
            var drift = Assert.Single(report.Items);
            Assert.Equal(H1ProjectionDriftClass.CatalogueDrift, drift.Classification);
            Assert.Equal("$scene", drift.ObjectId);

            var proposal = H1ProjectionReconciliation.CompileProposal(plan, effectiveObservation, report, after);
            Assert.False(proposal.Available);
            Assert.Null(proposal.MutationRequest);
            Assert.Contains("projection.import-blocked-catalogue-drift:$scene", proposal.Diagnostics);
        }

        private static string RepositoryRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null && !File.Exists(Path.Combine(current.FullName, "Juego2.sln"))) current = current.Parent;
            Assert.NotNull(current);
            return current!.FullName;
        }
    }
}
