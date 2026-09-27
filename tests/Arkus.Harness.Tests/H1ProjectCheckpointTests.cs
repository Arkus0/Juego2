using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1ProjectCheckpointTests
    {
        [Fact]
        public void Capture_InterruptedBeforePointerPublication_KeepsPreviousCheckpointCurrent()
        {
            using var temp = new TempDirectory();
            var store = new H1ProjectCheckpointStore(temp.Path);
            var fixture = Fixture();
            var first = store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, Hash('a'), Hash('b'), Hash('c'));

            File.WriteAllText(Path.Combine(temp.Path, "fail-before-publish"), "fail");
            var exception = Assert.Throws<H1ProjectCheckpointException>(() =>
                store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, Hash('d'), Hash('e'), Hash('f')));
            Assert.Equal("checkpoint.forced-prepublication-failure", exception.Code);
            File.Delete(Path.Combine(temp.Path, "fail-before-publish"));

            var current = store.ReadCurrent(fixture.Environment);
            Assert.Equal("ready", current.State);
            Assert.Equal(first.CheckpointId, current.CheckpointId);
            Assert.Equal(Hash('a'), current.Manifest.Projection.ObservationGraphDigest);
            Assert.Equal(Hash('c'), current.Manifest.Projection.ObservationReconstructionDigest);
        }

        [Fact]
        public void ReadCurrent_FingerprintMismatch_IsBlockedAndDoesNotReleaseRestoreArtifacts()
        {
            using var temp = new TempDirectory();
            var store = new H1ProjectCheckpointStore(temp.Path);
            var fixture = Fixture();
            store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, Hash('a'), Hash('b'), Hash('c'));

            var rebound = Environment(fixture.Plan.CatalogueFingerprint, bridge: Hash('9'));
            var current = store.ReadCurrent(rebound);

            Assert.Equal("blocked", current.State);
            Assert.Contains("checkpoint.fingerprint-mismatch:bridge", current.Blockers);
            Assert.Contains("checkpoint.fingerprint-mismatch:environment", current.Blockers);
            Assert.Equal(string.Empty, current.SnapshotJson);
            Assert.Equal(string.Empty, current.JournalJson);
        }

        [Fact]
        public void ReadCurrent_TamperedSnapshot_FailsChecksumBeforeRestoreArtifactIsExposed()
        {
            using var temp = new TempDirectory();
            var store = new H1ProjectCheckpointStore(temp.Path);
            var fixture = Fixture();
            var captured = store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, Hash('a'), Hash('b'), Hash('c'));
            var snapshotPath = Path.Combine(temp.Path, "checkpoints", captured.CheckpointId, "snapshot.json");
            File.AppendAllText(snapshotPath, " ");

            var exception = Assert.Throws<H1ProjectCheckpointException>(() => store.ReadCurrent(fixture.Environment));
            Assert.Equal("checkpoint.invalid-generation", exception.Code);
        }

        [Fact]
        public void Capture_IsDeterministicForSameAcceptedArtifactsAndEnvironment()
        {
            using var temp = new TempDirectory();
            var store = new H1ProjectCheckpointStore(temp.Path);
            var fixture = Fixture();

            var first = store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, Hash('a'), Hash('b'), Hash('c'));
            var second = store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, Hash('a'), Hash('b'), Hash('c'));

            Assert.Equal(first.CheckpointId, second.CheckpointId);
            Assert.Equal(first.ManifestSha256, second.ManifestSha256);
            Assert.Equal(first.Manifest.Projection.InputDigest, second.Manifest.Projection.InputDigest);
        }

        [Fact]
        public void CaptureFromObservation_NotCurrent_PublishesNoCheckpoint()
        {
            using var temp = new TempDirectory();
            var store = new H1ProjectCheckpointStore(temp.Path);
            var fixture = Fixture();
            var observation = Observation(fixture.Plan, current: false, PrefabNode("facade", "g1", "1"));

            var exception = Assert.Throws<H1ProjectCheckpointException>(() =>
                store.CaptureFromObservation(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, observation));
            Assert.Equal("checkpoint.observation-not-current", exception.Code);
            var missing = Assert.Throws<H1ProjectCheckpointException>(() => store.ReadCurrent(fixture.Environment));
            Assert.Equal("checkpoint.missing-current", missing.Code);
        }

        [Fact]
        public void CaptureFromObservation_RecordsNormalizedReconstructionBaseline()
        {
            using var temp = new TempDirectory();
            var store = new H1ProjectCheckpointStore(temp.Path);
            var fixture = Fixture();
            var observation = Observation(fixture.Plan, current: true, PrefabNode("facade", "g1", "1"), SourceNode("workshop", "s1"));

            var captured = store.CaptureFromObservation(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, observation);

            Assert.Equal(H1ReconstructionParity.Digest(observation), captured.Manifest.Projection.ObservationReconstructionDigest);
            Assert.Equal(Hash('7'), captured.Manifest.Projection.ObservationGraphDigest);
            Assert.Equal(captured.Manifest.Projection.ObservationReconstructionDigest, H1ReconstructionParity.Require(captured.Manifest, observation));
        }

        [Fact]
        public void Parity_ExcludesOnlyGenerationLocalDerivativeLocators()
        {
            var plan = Fixture().Plan;
            var first = Observation(plan, true, PrefabNode("facade", "11111111111111111111111111111111", "100"), SourceNode("workshop", "s1"));
            var rebuilt = Observation(plan, true, SourceNode("workshop", "s1"), PrefabNode("facade", "22222222222222222222222222222222", "200"));

            Assert.Equal(H1ReconstructionParity.Digest(first), H1ReconstructionParity.Digest(rebuilt));
        }

        [Theory]
        [InlineData("sourceContentSha256")]
        [InlineData("sourceGuid")]
        [InlineData("sourceLocalFileId")]
        [InlineData("sourcePath")]
        [InlineData("sourceLogicalId")]
        [InlineData("parentObjectId")]
        [InlineData("realizedPath")]
        [InlineData("prefabGenerationId")]
        [InlineData("relationshipDigest")]
        [InlineData("componentDigest")]
        public void Parity_DetectsEveryNonLocatorFact(string field)
        {
            var plan = Fixture().Plan;
            var baseline = Observation(plan, true, PrefabNode("facade", "g1", "1"));
            var changed = PrefabNode("facade", "g2", "2");
            changed[field] = (string)changed[field]! + "-drift";

            Assert.NotEqual(H1ReconstructionParity.Digest(baseline), H1ReconstructionParity.Digest(Observation(plan, true, changed)));
        }

        [Fact]
        public void Parity_IncludesObservedFactsItDoesNotNameExplicitly()
        {
            var plan = Fixture().Plan;
            var baseline = H1ReconstructionParity.Digest(Observation(plan, true, PrefabNode("facade", "g1", "1")));
            var extended = PrefabNode("facade", "g2", "2");
            extended["futureRealizationFact"] = "value";
            Assert.NotEqual(baseline, H1ReconstructionParity.Digest(Observation(plan, true, extended)));
            var relationships = PrefabNode("facade", "g3", "3");
            relationships["relationships"] = new object?[] { new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "variant-base", ["assetGuid"] = "a" } };
            Assert.NotEqual(baseline, H1ReconstructionParity.Digest(Observation(plan, true, relationships)));
        }

        [Fact]
        public void Parity_DetectsSourceLocatorTransformAndMembershipDrift()
        {
            var plan = Fixture().Plan;
            var baseline = H1ReconstructionParity.Digest(Observation(plan, true, PrefabNode("facade", "g", "1"), SourceNode("workshop", "s1")));

            // Source-asset realizations are accepted external input, so their locator is not excluded.
            Assert.NotEqual(baseline, H1ReconstructionParity.Digest(Observation(plan, true, PrefabNode("facade", "g", "1"), SourceNode("workshop", "s2"))));
            var moved = PrefabNode("facade", "g", "1");
            moved["positionMm"] = Vector(101, 0, 0);
            Assert.NotEqual(baseline, H1ReconstructionParity.Digest(Observation(plan, true, moved, SourceNode("workshop", "s1"))));
            Assert.NotEqual(baseline, H1ReconstructionParity.Digest(Observation(plan, true, PrefabNode("facade", "g", "1"))));
            var duplicate = Assert.Throws<H1ProjectCheckpointException>(() =>
                H1ReconstructionParity.Digest(Observation(plan, true, PrefabNode("facade", "g", "1"), PrefabNode("facade", "g", "1"))));
            Assert.Equal("checkpoint.observation-invalid", duplicate.Code);
        }

        [Fact]
        public void Require_ReportsStructuredRestoreOutcomes()
        {
            using var temp = new TempDirectory();
            var store = new H1ProjectCheckpointStore(temp.Path);
            var fixture = Fixture();
            var observation = Observation(fixture.Plan, true, PrefabNode("facade", "g1", "1"));
            var manifest = store.CaptureFromObservation(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, observation).Manifest;

            Assert.Equal(manifest.Projection.ObservationReconstructionDigest,
                H1ReconstructionParity.Require(manifest, Observation(fixture.Plan, true, PrefabNode("facade", "g9", "9"))));

            var drifted = PrefabNode("facade", "g1", "1");
            drifted["componentDigest"] = Hash('9');
            Assert.Equal("checkpoint.restore-observation-drift", Assert.Throws<H1ProjectCheckpointException>(() =>
                H1ReconstructionParity.Require(manifest, Observation(fixture.Plan, true, drifted))).Code);
            Assert.Equal("checkpoint.restore-observation-not-current", Assert.Throws<H1ProjectCheckpointException>(() =>
                H1ReconstructionParity.Require(manifest, Observation(fixture.Plan, false, PrefabNode("facade", "g1", "1")))).Code);
            var otherPlan = Observation(fixture.Plan, true, PrefabNode("facade", "g1", "1"));
            ((Dictionary<string, object?>)otherPlan)["inputDigest"] = Hash('8');
            Assert.Equal("checkpoint.restore-plan-drift", Assert.Throws<H1ProjectCheckpointException>(() =>
                H1ReconstructionParity.Require(manifest, otherPlan)).Code);
            manifest.Projection.ObservationReconstructionDigest = "";
            Assert.Equal("checkpoint.restore-missing-baseline-observation", Assert.Throws<H1ProjectCheckpointException>(() =>
                H1ReconstructionParity.RequireBaseline(manifest)).Code);
        }

        private static IReadOnlyDictionary<string, object?> Observation(H1ManagedScenePlan plan, bool current, params Dictionary<string, object?>[] nodes) =>
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.h1-managed-scene-observation@1",
                ["current"] = current,
                ["inputDigest"] = plan.InputDigest,
                ["canonicalHash"] = plan.CanonicalHash,
                ["catalogueFingerprint"] = plan.CatalogueFingerprint,
                ["graphDigest"] = Hash('7'),
                ["realizationDigest"] = Hash('6'),
                ["nodes"] = nodes.Cast<object?>().ToArray()
            };

        private static Dictionary<string, object?> PrefabNode(string id, string realizedGuid, string realizedFileId) => new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["objectId"] = id, ["parentObjectId"] = "", ["sourceLogicalId"] = "quaternius.medieval.prefab.wall-plaster-window-wide-flat",
            ["sourceKind"] = "prefab", ["sourcePath"] = "Assets/Arkus/H1/SourceSlice/Wall_Plaster_Window_Wide_Flat.fbx",
            ["sourceGuid"] = "0123456789abcdef0123456789abcdef", ["sourceLocalFileId"] = "919132149155446097", ["sourceContentSha256"] = Hash('3'),
            ["realizationKind"] = H1ReconstructionParity.GeneratedRealizationKind, ["realizedPath"] = "Assets/Arkus/H1/ManagedPrefabs/Generations/gen/facade.prefab",
            ["realizedGuid"] = realizedGuid, ["realizedLocalFileId"] = realizedFileId, ["prefabGenerationId"] = "0123456789abcdef0123456789abcdef",
            ["relationshipDigest"] = Hash('4'), ["componentDigest"] = Hash('5'),
            ["positionMm"] = Vector(100, 0, 0), ["rotationMilliDegrees"] = Vector(0, 0, 0), ["scalePpm"] = Vector(1000000, 1000000, 1000000)
        };

        private static Dictionary<string, object?> SourceNode(string id, string guid) => new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["objectId"] = id, ["parentObjectId"] = "", ["sourceLogicalId"] = "quaternius.medieval.mesh.facade", ["sourceKind"] = "asset",
            ["sourcePath"] = "Assets/Arkus/H1/SourceSlice/Wall.fbx", ["sourceGuid"] = guid, ["sourceLocalFileId"] = "4300000", ["sourceContentSha256"] = Hash('3'),
            ["realizationKind"] = "source-asset", ["realizedPath"] = "Assets/Arkus/H1/SourceSlice/Wall.fbx", ["realizedGuid"] = guid, ["realizedLocalFileId"] = "4300000",
            ["prefabGenerationId"] = "", ["relationshipDigest"] = Hash('4'), ["componentDigest"] = Hash('5'),
            ["positionMm"] = Vector(900, 0, 0), ["rotationMilliDegrees"] = Vector(0, 360000, 0), ["scalePpm"] = Vector(1000000, 1000000, 1000000)
        };

        private static IReadOnlyDictionary<string, object?> Vector(long x, long y, long z) =>
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["x"] = x, ["y"] = y, ["z"] = z };

        private static FixtureData Fixture()
        {
            var state = new WorldState(new WorldId("world.h1-10"), 0, Array.Empty<WorldObject>());
            var session = new PortableWorldAuthoringSession(state);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var snapshot = Success(contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()));
            var journal = Success(contract.Dispatch(WorldProvenanceContract.ReadName, Exact(), Empty()));
            var catalogue = Catalogue();
            var plan = H1ManagedScenePlan.Build(state, catalogue);
            return new FixtureData(snapshot, journal, Environment(plan.CatalogueFingerprint), plan);
        }

        private static H1CatalogueSnapshot Catalogue()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            return H1CatalogueSnapshot.Build(
                File.ReadAllText(Path.Combine(root, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath)),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath)));
        }

        private static H1ProjectEnvironmentFingerprint Environment(string catalogueFingerprint, string? bridge = null)
        {
            var environment = new H1ProjectEnvironmentFingerprint
            {
                ProjectIdentity = "arkus.unity-project@1:ArkusUnity",
                BridgeFingerprint = bridge ?? Hash('1'),
                ProfileFingerprint = Hash('2'),
                CatalogueFingerprint = catalogueFingerprint,
                SourceFingerprint = Hash('3'),
                PackageFingerprint = Hash('4'),
                EditorFingerprint = Hash('5')
            };
            environment.Digest = Sha(string.Join("\n", new[]
            {
                "project=" + environment.ProjectIdentity,
                "bridge=" + environment.BridgeFingerprint,
                "profile=" + environment.ProfileFingerprint,
                "catalogue=" + environment.CatalogueFingerprint,
                "source=" + environment.SourceFingerprint,
                "package=" + environment.PackageFingerprint,
                "editor=" + environment.EditorFingerprint
            }));
            return environment;
        }

        private static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
        private static string Hash(char value) => new string(value, 64);
        private static IReadOnlyDictionary<string, object?> Success(CapabilityInvocationResult result)
        {
            Assert.True(result.Success, result.Error?.MachineCode);
            return Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(result.Data);
        }
        private static IReadOnlyDictionary<string, object?> Empty() => new Dictionary<string, object?>(StringComparer.Ordinal);
        private static ContractVersionRange Exact() => ContractVersionRange.Exact(new ContractVersion(1, 0));

        private sealed class FixtureData
        {
            public FixtureData(IReadOnlyDictionary<string, object?> snapshot, IReadOnlyDictionary<string, object?> journal, H1ProjectEnvironmentFingerprint environment, H1ManagedScenePlan plan)
            {
                Snapshot = snapshot; Journal = journal; Environment = environment; Plan = plan;
            }
            public IReadOnlyDictionary<string, object?> Snapshot { get; }
            public IReadOnlyDictionary<string, object?> Journal { get; }
            public H1ProjectEnvironmentFingerprint Environment { get; }
            public H1ManagedScenePlan Plan { get; }
        }

        private sealed class TempDirectory : IDisposable
        {
            public TempDirectory()
            {
                Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "arkus-h1-10-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(Path);
            }
            public string Path { get; }
            public void Dispose()
            {
                try { if (Directory.Exists(Path)) Directory.Delete(Path, true); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }
    }
}
