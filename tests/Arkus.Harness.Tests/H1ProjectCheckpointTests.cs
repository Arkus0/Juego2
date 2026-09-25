using System;
using System.Collections.Generic;
using System.IO;
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
            var first = store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, "graph-a", "realization-a");

            File.WriteAllText(Path.Combine(temp.Path, "fail-before-publish"), "fail");
            var exception = Assert.Throws<H1ProjectCheckpointException>(() =>
                store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, "graph-b", "realization-b"));
            Assert.Equal("checkpoint.forced-prepublication-failure", exception.Code);
            File.Delete(Path.Combine(temp.Path, "fail-before-publish"));

            var current = store.ReadCurrent(fixture.Environment);
            Assert.Equal("ready", current.State);
            Assert.Equal(first.CheckpointId, current.CheckpointId);
            Assert.Equal("graph-a", current.Manifest.Projection.ObservationGraphDigest);
        }

        [Fact]
        public void ReadCurrent_FingerprintMismatch_IsBlockedAndDoesNotReleaseRestoreArtifacts()
        {
            using var temp = new TempDirectory();
            var store = new H1ProjectCheckpointStore(temp.Path);
            var fixture = Fixture();
            store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, "graph", "realization");

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
            var captured = store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, "graph", "realization");
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

            var first = store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, "graph", "realization");
            var second = store.Capture(fixture.Snapshot, fixture.Journal, fixture.Environment, fixture.Plan, "graph", "realization");

            Assert.Equal(first.CheckpointId, second.CheckpointId);
            Assert.Equal(first.ManifestSha256, second.ManifestSha256);
            Assert.Equal(first.Manifest.Projection.InputDigest, second.Manifest.Projection.InputDigest);
        }

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
            environment.Digest = H1ProjectEnvironmentProbe.ShaText(string.Join("\n", new[]
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
