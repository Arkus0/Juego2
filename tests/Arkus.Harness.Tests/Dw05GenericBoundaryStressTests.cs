using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw05GenericBoundaryStressTests
    {
        private const string NeutralSource =
            "OBSERVATORY|id=observatory.north|elevation_m=2410|remote=true\n" +
            "DETECTOR|id=detector.spectro|wavelength_min_nm=380.5|wavelength_max_nm=740.25\n" +
            "CALIBRATION|id=calibration.lamp-a|mode=argon\n" +
            "CONDITION|sky=clear\n";

        private static readonly DesignProjectionVersion NeutralVersion = new DesignProjectionVersion(1, "dw05-neutral-v1");

        [Fact]
        public void NeutralThirdShapeProjectsQueriesValidatesAndRebuildsThroughAcceptedGenericSeams()
        {
            var sourceBytesBefore = Encoding.UTF8.GetBytes(NeutralSource);
            var universe = NeutralUniverse();
            var reader = NeutralReader(NeutralSource, NeutralDefinitions(includeCalibrationRelation: true, relationTarget: "calibration.lamp-a"));
            var projector = new DesignWorldProjector();

            var projection = projector.Build(universe, reader, NeutralVersion);
            var report = new DesignWorldProjectionValidator().Validate(projection, universe, reader, NeutralVersion);

            Assert.True(report.IsValid, string.Join("; ", report.Issues.Select(issue => issue.MachineCode + ":" + issue.FactId)));
            Assert.Equal(new[] { "calibration.lamp-a", "detector.spectro", "observatory.north" }, projection.FactIds);

            var remoteObservatories = projection.Facts
                .Where(fact => fact.FactType == "observatory")
                .Where(fact => fact.Fields.TryGetValue("remote", out var value) && value == DesignValue.Boolean(true))
                .Select(fact => fact.FactId)
                .ToArray();
            Assert.Equal(new[] { "observatory.north" }, remoteObservatories);

            var calibrationTargets = projection.Facts
                .Single(fact => fact.FactId == "detector.spectro")
                .Relations
                .Where(relation => relation.RelationType == "calibrated-by")
                .Select(relation => relation.TargetFactId)
                .ToArray();
            Assert.Equal(new[] { "calibration.lamp-a" }, calibrationTargets);

            foreach (var fact in projection.Facts)
            {
                Assert.Equal(DesignAuthorityResolutionStatus.Found, reader.Resolve(fact.Provenance));
                Assert.Equal("neutral-observatory-v1", fact.Provenance.AuthorityId);
                Assert.Equal("Docs/evidence/WP-DW-05/fixtures/neutral-observatory.txt", fact.Provenance.SourcePath);
            }

            var rebuiltReader = NeutralReader(NeutralSource, NeutralDefinitions(includeCalibrationRelation: true, relationTarget: "calibration.lamp-a"));
            var rebuilt = projector.Build(universe, rebuiltReader, NeutralVersion);
            Assert.Equal(projection.Digest, rebuilt.Digest);
            Assert.Equal(projection.NormalizedRepresentation, rebuilt.NormalizedRepresentation);
            Assert.True(DesignWorldProjectionDiff.Compare(projection, rebuilt).IsEmpty);
            Assert.Equal(sourceBytesBefore, Encoding.UTF8.GetBytes(NeutralSource));
        }

        [Fact]
        public void MissingNeutralAuthorityEdgeFailsClosedThroughGenericProvenancePath()
        {
            var sourceWithoutCalibration = NeutralSource.Replace("CALIBRATION|id=calibration.lamp-a|mode=argon\n", string.Empty, StringComparison.Ordinal);
            var reader = NeutralReader(sourceWithoutCalibration, NeutralDefinitions(includeCalibrationRelation: true, relationTarget: "calibration.lamp-a"));

            var error = Assert.Throws<DesignWorldProjectionException>(() =>
                new DesignWorldProjector().Build(NeutralUniverse(), reader, NeutralVersion));

            Assert.Equal("dw.provenance_missing", error.MachineCode);
        }

        [Fact]
        public void StaleNeutralAuthorityBytesTurnExistingProjectionRed()
        {
            var universe = NeutralUniverse();
            var originalReader = NeutralReader(NeutralSource, NeutralDefinitions(includeCalibrationRelation: true, relationTarget: "calibration.lamp-a"));
            var projection = new DesignWorldProjector().Build(universe, originalReader, NeutralVersion);
            var changedSource = NeutralSource.Replace("CONDITION|sky=clear", "CONDITION|sky=cloudy", StringComparison.Ordinal);
            var changedReader = NeutralReader(changedSource, NeutralDefinitions(includeCalibrationRelation: true, relationTarget: "calibration.lamp-a"));

            var report = new DesignWorldProjectionValidator().Validate(projection, universe, changedReader, NeutralVersion);

            Assert.False(report.IsValid);
            Assert.Contains(report.Issues, issue => issue.MachineCode == "dw.provenance_stale");
        }

        [Fact]
        public void RelationTargetOutsideIndependentNeutralUniverseFailsGenericProjection()
        {
            var reader = NeutralReader(NeutralSource, NeutralDefinitions(includeCalibrationRelation: true, relationTarget: "calibration.ghost"));

            var error = Assert.Throws<DesignWorldProjectionException>(() =>
                new DesignWorldProjector().Build(NeutralUniverse(), reader, NeutralVersion));

            Assert.Equal("dw.relation_target_outside_universe", error.MachineCode);
        }

        [Fact]
        public void RequiredNeutralRelationOmissionIsDetectedByProbeOracleWithoutPretendingH0OwnsDomainSemantics()
        {
            var universe = NeutralUniverse();
            var reader = NeutralReader(NeutralSource, NeutralDefinitions(includeCalibrationRelation: false, relationTarget: "calibration.lamp-a"));
            var projection = new DesignWorldProjector().Build(universe, reader, NeutralVersion);
            var genericReport = new DesignWorldProjectionValidator().Validate(projection, universe, reader, NeutralVersion);

            Assert.True(genericReport.IsValid);
            var probeIssues = ValidateNeutralSemantics(projection);
            Assert.Equal(new[] { "neutral.required_relation_missing:detector.spectro:calibrated-by:calibration.lamp-a" }, probeIssues);
        }

        [Fact]
        public void GenericDwAndH0DependencyClosureContainsNoCityOrPaLeakageAndInjectedLeakageReds()
        {
            var repositoryRoot = FindRepositoryRoot();
            var genericFiles = GenericBoundaryFiles(repositoryRoot).ToArray();
            var violations = FindLeakage(genericFiles.Select(path => new SourceSurface(path, File.ReadAllText(path))));

            Assert.Empty(violations);

            var injected = new[]
            {
                new SourceSurface("src/Arkus.Game.World/CityKernelRule.cs", "namespace Arkus.Game.World { public sealed class CityKernelRule { } }")
            };
            Assert.Contains(FindLeakage(injected), item => item.Contains("CityKernelRule", StringComparison.Ordinal));
        }

        [Fact]
        public void AcceptedGenericDwAndH0SeamBytesRemainAtBaselineBlobIdentities()
        {
            var repositoryRoot = FindRepositoryRoot();
            var expected = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["src/Arkus.DesignWorld/Arkus.DesignWorld.csproj"] = "3a7bc98c19e0f356e6500cfb3f0e5057fd190a40",
                ["src/Arkus.DesignWorld/DesignWorldContracts.cs"] = "947f896ebf556d677a8d548df55b54d16d2a6af1",
                ["src/Arkus.DesignWorld/DesignWorldProjection.cs"] = "23231f5622420f0e3280e60baa5faccafe44fab4",
                ["src/Arkus.Game.World/Arkus.Game.World.csproj"] = "c5a3a18202feb50b0e7df4cdecf6b9677a95fe3c",
                ["src/Arkus.Game.World/CanonicalWorldStateCodec.cs"] = "84915f9f66a802b34319c3fb1e39fe6c8b9b86c3",
                ["src/Arkus.Game.World/WorldIdentifiers.cs"] = "140b0fea0b3ce636159c3fc63a7fb9b03947eca3",
                ["src/Arkus.Game.World/WorldModule.cs"] = "de9a2620ee8f29fb85b9e1f9159ed260145cb19e",
                ["src/Arkus.Game.World/WorldState.cs"] = "20f70fe70183754352fecb41c9bb1189b4450f1f",
                ["src/Arkus.Game.World/WorldStateValidator.cs"] = "94d39ff279be30eb21cc720331b530f523a62cad"
            };

            foreach (var pair in expected)
            {
                var path = Path.Combine(repositoryRoot, pair.Key.Replace('/', Path.DirectorySeparatorChar));
                Assert.Equal(pair.Value, ComputeGitBlobSha1(path));
            }
        }

        [Fact]
        public void PredecessorResidualManifestReconcilesIndependentEvidenceInventory()
        {
            var repositoryRoot = FindRepositoryRoot();
            var inventory = ReadJson<ResidualInventory>(Path.Combine(repositoryRoot, "Docs", "evidence", "WP-DW-05", "PREDECESSOR_RESIDUAL_INVENTORY.json"));
            var manifest = ReadJson<LimitationManifest>(Path.Combine(repositoryRoot, "Docs", "evidence", "WP-DW-05", "LIMITATIONS_MANIFEST.json"));

            foreach (var clause in inventory.Clauses)
            {
                var source = File.ReadAllText(Path.Combine(repositoryRoot, clause.Source.Replace('/', Path.DirectorySeparatorChar)));
                Assert.Contains(clause.Anchor, source, StringComparison.Ordinal);
            }

            Assert.Empty(ValidateResidualReconciliation(inventory, manifest));
        }

        [Fact]
        public void ResidualReconciliationRedsForOmissionAndUnsupportedKernelClassification()
        {
            var repositoryRoot = FindRepositoryRoot();
            var inventory = ReadJson<ResidualInventory>(Path.Combine(repositoryRoot, "Docs", "evidence", "WP-DW-05", "PREDECESSOR_RESIDUAL_INVENTORY.json"));
            var manifest = ReadJson<LimitationManifest>(Path.Combine(repositoryRoot, "Docs", "evidence", "WP-DW-05", "LIMITATIONS_MANIFEST.json"));

            var omission = CloneManifest(manifest);
            omission.Limitations.RemoveAll(item => item.Id == inventory.Clauses[0].LimitationId);
            Assert.Contains(ValidateResidualReconciliation(inventory, omission), issue => issue.StartsWith("residual-missing:", StringComparison.Ordinal));

            var misclassified = CloneManifest(manifest);
            var domainEntry = misclassified.Limitations.First(item => item.Classification == "domain_need");
            domainEntry.Classification = "generic_arkus_contradiction_deficiency";
            domainEntry.PublicSeamFailureEvidence = string.Empty;
            Assert.Contains(ValidateResidualReconciliation(inventory, misclassified), issue => issue == "kernel-classification-without-public-seam-evidence:" + domainEntry.Id);
        }

        [Fact]
        public void H2InputKeepsUntestedDownstreamOpportunitiesOutOfAcceptedCapabilities()
        {
            var repositoryRoot = FindRepositoryRoot();
            var report = ReadJson<H2BoundaryInput>(Path.Combine(repositoryRoot, "Docs", "evidence", "WP-DW-05", "H2_BOUNDARY_INPUT_V1.json"));
            var requiredCandidates = new[]
            {
                "design-unity-drift",
                "generated-art-briefs",
                "content-catalogue-coverage",
                "replay-qa",
                "narrative-knowledge-checks"
            };

            Assert.False(report.Claims.ArbitraryDomainUniversality);
            Assert.False(report.Claims.ExternalRepositoryConsumability);
            foreach (var candidate in requiredCandidates)
            {
                Assert.Contains(candidate, report.OptionalOpportunities);
                Assert.DoesNotContain(candidate, report.AcceptedCapabilities);
            }
        }

        private static StaticDesignAuthorityUniverse NeutralUniverse() =>
            new StaticDesignAuthorityUniverse(new[] { "observatory.north", "detector.spectro", "calibration.lamp-a" });

        private static AnchoredTextAuthorityReader NeutralReader(string source, IEnumerable<AnchoredFactDefinition> definitions) =>
            new AnchoredTextAuthorityReader(
                "neutral-observatory-v1",
                "Docs/evidence/WP-DW-05/fixtures/neutral-observatory.txt",
                source,
                definitions);

        private static AnchoredFactDefinition[] NeutralDefinitions(bool includeCalibrationRelation, string relationTarget)
        {
            var detectorRelations = includeCalibrationRelation
                ? new[] { new DesignRelation("calibrated-by", relationTarget) }
                : Array.Empty<DesignRelation>();

            return new[]
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
                    detectorRelations),
                new AnchoredFactDefinition(
                    "calibration.lamp-a",
                    "calibration-source",
                    "CALIBRATION|id=calibration.lamp-a|mode=argon",
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["mode"] = DesignValue.String("argon")
                    })
            };
        }

        private static string[] ValidateNeutralSemantics(DesignWorldProjection projection)
        {
            var issues = new List<string>();
            var detector = projection.Facts.Single(fact => fact.FactId == "detector.spectro");
            if (!detector.Relations.Any(relation =>
                    relation.RelationType == "calibrated-by" && relation.TargetFactId == "calibration.lamp-a"))
            {
                issues.Add("neutral.required_relation_missing:detector.spectro:calibrated-by:calibration.lamp-a");
            }

            return issues.ToArray();
        }

        private static IEnumerable<string> GenericBoundaryFiles(string repositoryRoot)
        {
            yield return Path.Combine(repositoryRoot, "src", "Arkus.DesignWorld", "Arkus.DesignWorld.csproj");
            yield return Path.Combine(repositoryRoot, "src", "Arkus.DesignWorld", "DesignWorldContracts.cs");
            yield return Path.Combine(repositoryRoot, "src", "Arkus.DesignWorld", "DesignWorldProjection.cs");
            var h0Directory = Path.Combine(repositoryRoot, "src", "Arkus.Game.World");
            foreach (var file in Directory.GetFiles(h0Directory, "*", SearchOption.TopDirectoryOnly)
                         .Where(path => path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".csproj", StringComparison.Ordinal))
                         .OrderBy(path => path, StringComparer.Ordinal))
            {
                yield return file;
            }
        }

        private static string[] FindLeakage(IEnumerable<SourceSurface> surfaces)
        {
            var pattern = new Regex(@"\bcity\b|\bpa\b|\bCity[A-Z][A-Za-z0-9_]*\b|\bPa[A-Z][A-Za-z0-9_]*\b", RegexOptions.CultureInvariant);
            return surfaces
                .Where(surface => pattern.IsMatch(surface.Content) || pattern.IsMatch(Path.GetFileName(surface.Path)))
                .Select(surface => surface.Path)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
        }

        private static string[] ValidateResidualReconciliation(ResidualInventory inventory, LimitationManifest manifest)
        {
            var issues = new List<string>();
            var allowedClassifications = new HashSet<string>(new[]
            {
                "domain_need",
                "dw_tooling_need",
                "generic_arkus_contradiction_deficiency",
                "downstream_residual"
            }, StringComparer.Ordinal);
            var byId = manifest.Limitations.GroupBy(item => item.Id, StringComparer.Ordinal).ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);

            foreach (var clause in inventory.Clauses)
            {
                if (!byId.TryGetValue(clause.LimitationId, out var entries) || entries.Count != 1)
                {
                    issues.Add("residual-missing:" + clause.LimitationId);
                }
            }

            foreach (var entry in manifest.Limitations)
            {
                if (!allowedClassifications.Contains(entry.Classification))
                {
                    issues.Add("classification-invalid:" + entry.Id);
                }

                if (string.IsNullOrWhiteSpace(entry.CausalOwner))
                {
                    issues.Add("owner-missing:" + entry.Id);
                }

                if (entry.Classification == "generic_arkus_contradiction_deficiency" && string.IsNullOrWhiteSpace(entry.PublicSeamFailureEvidence))
                {
                    issues.Add("kernel-classification-without-public-seam-evidence:" + entry.Id);
                }
            }

            return issues.Distinct(StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal).ToArray();
        }

        private static LimitationManifest CloneManifest(LimitationManifest source)
        {
            var json = JsonSerializer.Serialize(source);
            return JsonSerializer.Deserialize<LimitationManifest>(json) ?? throw new InvalidOperationException("Could not clone limitation manifest.");
        }

        private static T ReadJson<T>(string path)
        {
            var result = JsonSerializer.Deserialize<T>(File.ReadAllText(path), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result ?? throw new InvalidOperationException("Could not parse " + path);
        }

        private static string ComputeGitBlobSha1(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var header = Encoding.UTF8.GetBytes("blob " + bytes.Length + "\0");
            var payload = new byte[header.Length + bytes.Length];
            Buffer.BlockCopy(header, 0, payload, 0, header.Length);
            Buffer.BlockCopy(bytes, 0, payload, header.Length, bytes.Length);
#pragma warning disable SYSLIB0021
            using var sha1 = SHA1.Create();
#pragma warning restore SYSLIB0021
            return Convert.ToHexString(sha1.ComputeHash(payload)).ToLowerInvariant();
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

        private sealed record SourceSurface(string Path, string Content);

        private sealed class ResidualInventory
        {
            [JsonPropertyName("clauses")]
            public List<ResidualClause> Clauses { get; set; } = new List<ResidualClause>();
        }

        private sealed class ResidualClause
        {
            [JsonPropertyName("source")]
            public string Source { get; set; } = string.Empty;

            [JsonPropertyName("anchor")]
            public string Anchor { get; set; } = string.Empty;

            [JsonPropertyName("limitation_id")]
            public string LimitationId { get; set; } = string.Empty;
        }

        private sealed class LimitationManifest
        {
            [JsonPropertyName("limitations")]
            public List<LimitationEntry> Limitations { get; set; } = new List<LimitationEntry>();
        }

        private sealed class LimitationEntry
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("classification")]
            public string Classification { get; set; } = string.Empty;

            [JsonPropertyName("causal_owner")]
            public string CausalOwner { get; set; } = string.Empty;

            [JsonPropertyName("public_seam_failure_evidence")]
            public string PublicSeamFailureEvidence { get; set; } = string.Empty;
        }

        private sealed class H2BoundaryInput
        {
            [JsonPropertyName("accepted_capabilities")]
            public List<string> AcceptedCapabilities { get; set; } = new List<string>();

            [JsonPropertyName("optional_opportunities")]
            public List<string> OptionalOpportunities { get; set; } = new List<string>();

            [JsonPropertyName("claims")]
            public H2Claims Claims { get; set; } = new H2Claims();
        }

        private sealed class H2Claims
        {
            [JsonPropertyName("arbitrary_domain_universality")]
            public bool ArbitraryDomainUniversality { get; set; }

            [JsonPropertyName("external_repository_consumability")]
            public bool ExternalRepositoryConsumability { get; set; }
        }
    }
}
