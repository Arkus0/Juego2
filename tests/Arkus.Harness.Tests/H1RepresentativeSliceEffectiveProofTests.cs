using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Process-staged WP-H1-11 proof over the representative real-source slice. CI launches every stage as its own
    /// `dotnet test` process and interleaves separate Unity Editor processes; the durable hand-off is the accepted H1-10
    /// project checkpoint plus bounded proof files under <c>Unity/ArkusUnity/H1-11-RealAssetSlice</c>. Every stage only
    /// composes accepted public capabilities (H0 contract, planner, catalogue, checkpoint store, observe decoder, parity).
    /// </summary>
    public sealed class H1RepresentativeSliceEffectiveProofTests
    {
        private const string EnabledVariable = "ARKUS_H1_11_EFFECTIVE_PROOF";
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        [Fact]
        public void StageA_SeedStreetCornerAndPlanTheCompleteRepresentativeUniverse()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var catalogue = Catalogue(profile);
            var manifest = H1RepresentativeSliceScenario.ReadManifest(profile.RepositoryRoot);
            var directory = ProofDirectory(profile);
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
            var checkpointRoot = Path.Combine(profile.ProjectRoot, H1ProjectCheckpointStore.RelativeRoot.Replace('/', Path.DirectorySeparatorChar));
            if (Directory.Exists(checkpointRoot)) Directory.Delete(checkpointRoot, true);

            var session = H1RepresentativeSliceScenario.AuthoredSession();
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var journal = Success(contract.Dispatch(WorldProvenanceContract.ReadName, Exact(), Empty()), "journal read");
            Assert.Equal(1L, Integer(journal, "entryCount"));

            var plan = H1ManagedScenePlan.Build(session.Current, catalogue);
            Assert.Empty(H1RepresentativeSliceScenario.UncoveredObligations(manifest, plan));
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(session.Current), plan.CanonicalHash);
            File.WriteAllText(Path.Combine(directory, "plan.json"), JsonSerializer.Serialize(plan, Json));
            File.WriteAllText(Path.Combine(directory, "seed.json"), JsonSerializer.Serialize(new SeedEvidence
            {
                SchemaId = "arkus.h1-11-seed@1",
                WorldId = session.Current.Id.Value,
                Revision = session.Current.Revision,
                CanonicalHash = plan.CanonicalHash,
                InputDigest = plan.InputDigest,
                CatalogueFingerprint = plan.CatalogueFingerprint,
                Nodes = plan.Nodes.Length,
                SelectedItems = manifest.Items.Count,
                Clips = manifest.Clips.Count
            }, Json));
        }

        [Fact]
        public void StageB_CaptureCheckpointFromTheEffectiveRepresentativeBaseline()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var directory = ProofDirectory(profile);
            var baseline = Read<BaselineEvidence>(Path.Combine(directory, "baseline.json"));
            Assert.Equal("arkus.h1-11-baseline@1", baseline.SchemaId);

            var catalogue = Catalogue(profile);
            var session = H1RepresentativeSliceScenario.AuthoredSession();
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var snapshot = Success(contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()), "snapshot export");
            var journal = Success(contract.Dispatch(WorldProvenanceContract.ReadName, Exact(), Empty()), "journal read");
            var plan = H1ManagedScenePlan.Build(session.Current, catalogue);
            Assert.Equal(plan.InputDigest, baseline.InputDigest);
            Assert.Equal(plan.CanonicalHash, baseline.CanonicalHash);
            Assert.Equal(plan.CatalogueFingerprint, baseline.CatalogueFingerprint);

            var observed = DecodeObservation(new ReadOnlyWorldStateView(session), profile, File.ReadAllText(Path.Combine(directory, "baseline-observation.json")));
            Assert.True((bool)observed["current"]!);
            Assert.Equal(baseline.GraphDigest, Text(observed, "graphDigest"));
            Assert.Equal(plan.Nodes.Length, ((IEnumerable<object?>)observed["nodes"]!).Count());

            // The normalized observation that parity relies on covers the humanoid rig: every civilian's
            // source-derived relationships carry its skinned mesh and both skinned material references.
            foreach (var civilian in new[] { "civilian.idle", "civilian.walk", "civilian.sit" })
            {
                var node = ((IEnumerable<object?>)observed["nodes"]!).Cast<IReadOnlyDictionary<string, object?>>().Single(value => Text(value, "objectId") == civilian);
                var relationships = ((IEnumerable<object?>)node["relationships"]!).Cast<IReadOnlyDictionary<string, object?>>().ToArray();
                Assert.Contains(relationships, row => Text(row, "kind") == "mesh-reference" && Text(row, "relativeObjectPath") == "Mannequin" &&
                    Text(row, "assetPath") == "Assets/Arkus/H1/SourceSlice/UAL1.fbx" && Text(row, "typeName") == "UnityEngine.Mesh");
                Assert.Equal(2, relationships.Count(row => Text(row, "kind") == "material-reference" && Text(row, "relativeObjectPath") == "Mannequin"));
            }

            var environment = H1ProjectEnvironmentProbe.Capture(profile, catalogue);
            var captured = new H1ProjectCheckpointStore(profile).CaptureFromObservation(snapshot, journal, environment, plan, observed);
            Assert.Equal("ready", captured.State);
            File.WriteAllText(Path.Combine(directory, "captured.json"), JsonSerializer.Serialize(new CaptureEvidence
            {
                SchemaId = "arkus.h1-11-capture-evidence@1",
                CheckpointId = captured.CheckpointId,
                ManifestSha256 = captured.ManifestSha256,
                EnvironmentDigest = captured.Manifest.Environment.Digest,
                SourceFingerprint = captured.Manifest.Environment.SourceFingerprint,
                CatalogueFingerprint = captured.Manifest.Environment.CatalogueFingerprint,
                ReconstructionDigest = captured.Manifest.Projection.ObservationReconstructionDigest
            }, Json));
        }

        [Fact]
        public void StageC_FreshProcessRestoresRepresentativeSliceOnlyThroughAcceptedH0Import()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var directory = ProofDirectory(profile);
            var catalogue = Catalogue(profile);
            var checkpoint = new H1ProjectCheckpointStore(profile).ReadCurrent(H1ProjectEnvironmentProbe.Capture(profile, catalogue));
            Assert.Equal("ready", checkpoint.State);

            var fresh = FreshImport(checkpoint, "h1-11-effective:");
            var restoredPlan = H1ManagedScenePlan.Build(fresh.Current, catalogue);
            Assert.Equal(checkpoint.Manifest.Canonical.Hash, restoredPlan.CanonicalHash);
            Assert.Equal(checkpoint.Manifest.Projection.InputDigest, restoredPlan.InputDigest);
            File.WriteAllText(Path.Combine(directory, "restored-plan.json"), JsonSerializer.Serialize(restoredPlan, Json));
        }

        [Fact]
        public void StageD_RebuiltRepresentativeObservationProvesNormalizedParity()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var directory = ProofDirectory(profile);
            var catalogue = Catalogue(profile);
            var checkpoint = new H1ProjectCheckpointStore(profile).ReadCurrent(H1ProjectEnvironmentProbe.Capture(profile, catalogue));
            Assert.Equal("ready", checkpoint.State);

            var fresh = FreshImport(checkpoint, "h1-11-parity:");
            var rebuilt = DecodeObservation(new ReadOnlyWorldStateView(fresh), profile, File.ReadAllText(Path.Combine(directory, "rebuilt-observation.json")));
            var reconstruction = H1ReconstructionParity.Require(checkpoint.Manifest, rebuilt);
            Assert.Equal(checkpoint.Manifest.Projection.ObservationReconstructionDigest, reconstruction);

            // Rendered-looking success must not hide a normalized failure: one changed fact on a real node breaks parity.
            foreach (var node in new[] { "civilian.walk", "roof.house" })
            {
                var drifted = CopyWithNodeField(rebuilt, node, "componentDigest", new string('0', 64));
                Assert.Equal("checkpoint.restore-observation-drift",
                    Assert.Throws<H1ProjectCheckpointException>(() => H1ReconstructionParity.Require(checkpoint.Manifest, drifted)).Code);
            }
            File.WriteAllText(Path.Combine(directory, "parity.json"), JsonSerializer.Serialize(new ParityEvidence
            {
                SchemaId = "arkus.h1-11-parity-evidence@1",
                CheckpointId = checkpoint.CheckpointId,
                ReconstructionDigest = reconstruction,
                BaselineRealizationDigest = checkpoint.Manifest.Projection.ObservationRealizationDigest,
                RebuiltRealizationDigest = Text(rebuilt, "realizationDigest"),
                GraphDigest = Text(rebuilt, "graphDigest"),
                Nodes = ((IEnumerable<object?>)rebuilt["nodes"]!).Count()
            }, Json));
        }

        [Fact]
        public void StageE_RemovedOrReplacedSelectedAssetIsNamedByTheLiveCatalogue()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var directory = ProofDirectory(profile);
            var mapping = File.ReadAllText(Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath));
            var adoption = File.ReadAllText(Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath));

            // The inventories were captured by the effective Unity catalogue after the Editor removed / rebound one real item.
            var removed = Assert.Throws<H1CatalogueException>(() => H1CatalogueSnapshot.Build(
                File.ReadAllText(Path.Combine(directory, "negative-removed-inventory.json")), mapping, adoption));
            Assert.Equal("catalogue.stale-mapping", removed.Code);
            Assert.Contains(".prop-crate", removed.Message, StringComparison.Ordinal);

            var replaced = Assert.Throws<H1CatalogueException>(() => H1CatalogueSnapshot.Build(
                File.ReadAllText(Path.Combine(directory, "negative-replaced-inventory.json")), mapping, adoption));
            Assert.Contains(replaced.Code, new[] { "catalogue.stale-mapping", "catalogue.incompatible-mapping" });
            Assert.Contains(".prop-wagon", replaced.Message, StringComparison.Ordinal);

            // The unchanged inventory still reconciles exactly (the negative is causal, not environmental).
            H1CatalogueSnapshot.Build(File.ReadAllText(Path.Combine(directory, "effective-inventory.json")), mapping, adoption);
        }

        [Fact]
        public async System.Threading.Tasks.Task StageE_RemovedOrReplacedSelectedAssetBlocksPublicRestore()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var catalogue = Catalogue(profile);
            var crate = catalogue.Entries.First(value => value.LogicalId == H1RepresentativeSliceScenario.CrateMesh);
            var path = Path.Combine(profile.ProjectRoot, crate.Path.Replace('/', Path.DirectorySeparatorChar));
            var accepted = File.ReadAllBytes(path);
            var wagon = catalogue.Entries.First(value => value.LogicalId == "quaternius.medieval.asset.prop-wagon");
            using var host = ProductionH1ProjectCheckpointHost.Create();
            try
            {
                File.WriteAllBytes(path, File.ReadAllBytes(Path.Combine(profile.ProjectRoot, wagon.Path.Replace('/', Path.DirectorySeparatorChar))));
                AssertBlocked(await Restore(host), "checkpoint.source-rebound");
                File.Delete(path);
                AssertBlocked(await Restore(host), "checkpoint.source-missing");
            }
            finally
            {
                File.WriteAllBytes(path, accepted);
            }

            var current = await host.InvokeAsync(new NeutralProjectionRequest("h1-11.current", "unity.host.checkpoint.current", Exact(), SceneRequest()));
            Assert.True(current.Success, current.Error?.MachineCode);
            Assert.Equal("ready", Text(current.Result!, "state"));
        }

        private static PortableWorldAuthoringSession FreshImport(H1ProjectCheckpointRead checkpoint, string keyPrefix)
        {
            // A distinct test process starts from an empty session and uses only the accepted H0 snapshot import.
            var fresh = new PortableWorldAuthoringSession(new WorldState(new WorldId("world.h1-11.fresh-process"), 0, Array.Empty<WorldObject>()));
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(fresh), fresh);
            var anchor = Map(Success(contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()), "fresh export"), "anchor");
            var portable = H1ProjectCheckpointStore.ParsePortableObject(checkpoint.SnapshotJson);
            var import = Success(contract.Dispatch(WorldPortabilityContract.ImportName, Exact(), ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = keyPrefix + checkpoint.CheckpointId,
                ["expectedRevision"] = Integer(anchor, "revision"),
                ["expectedHash"] = Text(anchor, "hash"),
                ["snapshot"] = portable
            })), "accepted H0 snapshot import");
            Assert.Equal("new-local-lineage", Text(import, "lineageDisposition"));
            var after = Success(contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()), "post-import export");
            var diff = Success(contract.Dispatch(WorldPortabilityContract.CompareName, Exact(), ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["base"] = portable,
                ["target"] = after
            })), "semantic diff");
            Assert.True((bool)diff["sameAuthorableState"]!);
            return fresh;
        }

        private static IReadOnlyDictionary<string, object?> DecodeObservation(IWorldStateSource world, H1UnityLaunchProfile profile, string rawUnityReply)
        {
            var executor = new H1ManagedSceneExecutor(H1ManagedSceneExecutor.ObserveKey, H1ManagedSceneExecutor.ObserveExecutorId, world, profile);
            executor.EncodeRequest(SceneRequest());
            var envelope = new H1UnityResultEnvelope(
                "h1-11-effective-proof", H1ManagedSceneExecutor.ObserveKey.Name, H1ManagedSceneExecutor.ObserveExecutorId, profile.Id,
                profile.ProjectIdentity, profile.EffectiveEditorVersion, profile.EffectiveEditorRevision, true, rawUnityReply);
            return Success(executor.DecodeResult(envelope), "decode effective Unity observation through the public observe executor");
        }

        private static IReadOnlyDictionary<string, object?> CopyWithNodeField(IReadOnlyDictionary<string, object?> observation, string objectId, string field, string value)
        {
            var found = false;
            var nodes = ((IEnumerable<object?>)observation["nodes"]!)
                .Select(node =>
                {
                    var copy = new Dictionary<string, object?>((IReadOnlyDictionary<string, object?>)node!, StringComparer.Ordinal);
                    if ((string)copy["objectId"]! == objectId) { copy[field] = value; found = true; }
                    return (object?)copy;
                })
                .ToArray();
            Assert.True(found, "observation has no node " + objectId);
            var result = observation.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
            result["nodes"] = nodes;
            return result;
        }

        private static System.Threading.Tasks.Task<NeutralProjectionOutcome> Restore(NeutralProjectionService host) =>
            host.InvokeAsync(new NeutralProjectionRequest("h1-11.restore." + Guid.NewGuid().ToString("N"), "unity.host.checkpoint.restore", Exact(), SceneRequest()));

        private static void AssertBlocked(NeutralProjectionOutcome outcome, string blocker)
        {
            Assert.True(outcome.Success, outcome.Error?.MachineCode);
            var result = outcome.Result!;
            Assert.Equal("blocked", Text(result, "state"));
            Assert.Equal("not-restored", Text(result, "lineageDisposition"));
            Assert.Contains(blocker, ((IEnumerable<object?>)result["blockers"]!).Cast<string>());
        }

        private static H1CatalogueSnapshot Catalogue(H1UnityLaunchProfile profile) => H1RepresentativeSliceScenario.CommittedCatalogue(profile.RepositoryRoot);
        private static string ProofDirectory(H1UnityLaunchProfile profile) => Path.Combine(profile.ProjectRoot, "H1-11-RealAssetSlice");
        private static bool Enabled() => string.Equals(Environment.GetEnvironmentVariable(EnabledVariable), "1", StringComparison.Ordinal);
        private static T Read<T>(string path) where T : class => JsonSerializer.Deserialize<T>(File.ReadAllText(path), Json) ?? throw new InvalidDataException("Invalid H1-11 proof artifact: " + path);
        private static IReadOnlyDictionary<string, object?> Success(CapabilityInvocationResult result, string phase)
        {
            Assert.True(result.Success, phase + ": " + result.Error?.MachineCode);
            return Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(result.Data);
        }
        private static IReadOnlyDictionary<string, object?> Map(IReadOnlyDictionary<string, object?> source, string key) => Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(source[key]);
        private static string Text(IReadOnlyDictionary<string, object?> source, string key) => Assert.IsType<string>(source[key]);
        private static long Integer(IReadOnlyDictionary<string, object?> source, string key) => Convert.ToInt64(source[key], CultureInfo.InvariantCulture);
        private static IReadOnlyDictionary<string, object?> Empty() => ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal));
        private static IReadOnlyDictionary<string, object?> SceneRequest() => ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal) { ["sceneLogicalId"] = H1ManagedScenePlan.SceneId });
        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> value) => new ReadOnlyDictionary<string, object?>(value);
        private static ContractVersionRange Exact() => ContractVersionRange.Exact(new ContractVersion(1, 0));

        public sealed class BaselineEvidence
        {
            public string SchemaId { get; set; } = "";
            public string InputDigest { get; set; } = "";
            public string CanonicalHash { get; set; } = "";
            public string CatalogueFingerprint { get; set; } = "";
            public string GraphDigest { get; set; } = "";
            public string RealizationDigest { get; set; } = "";
        }
        private sealed class SeedEvidence
        {
            public string SchemaId { get; set; } = ""; public string WorldId { get; set; } = ""; public long Revision { get; set; }
            public string CanonicalHash { get; set; } = ""; public string InputDigest { get; set; } = ""; public string CatalogueFingerprint { get; set; } = "";
            public int Nodes { get; set; } public int SelectedItems { get; set; } public int Clips { get; set; }
        }
        private sealed class CaptureEvidence
        {
            public string SchemaId { get; set; } = ""; public string CheckpointId { get; set; } = ""; public string ManifestSha256 { get; set; } = "";
            public string EnvironmentDigest { get; set; } = ""; public string SourceFingerprint { get; set; } = ""; public string CatalogueFingerprint { get; set; } = "";
            public string ReconstructionDigest { get; set; } = "";
        }
        private sealed class ParityEvidence
        {
            public string SchemaId { get; set; } = ""; public string CheckpointId { get; set; } = ""; public string ReconstructionDigest { get; set; } = "";
            public string BaselineRealizationDigest { get; set; } = ""; public string RebuiltRealizationDigest { get; set; } = "";
            public string GraphDigest { get; set; } = ""; public int Nodes { get; set; }
        }
    }
}
