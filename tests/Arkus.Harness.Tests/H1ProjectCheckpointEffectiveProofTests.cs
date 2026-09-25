using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Process-staged H1-10 proof. CI invokes each method with a separate `dotnet test` process and
    /// interleaves two independent Unity Editor processes. The only durable hand-off is the fixed
    /// project checkpoint plus bounded proof observations; no test helper restores canonical state.
    /// </summary>
    public sealed class H1ProjectCheckpointEffectiveProofTests
    {
        private const string EnabledVariable = "ARKUS_H1_10_EFFECTIVE_PROOF";
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        [Fact]
        public void StageA_SeedAcceptedCanonicalArtifactsAndNormalizedPlan()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var catalogue = Catalogue(profile);
            var directory = ProofDirectory(profile);
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
            var checkpointRoot = Path.Combine(profile.ProjectRoot, H1ProjectCheckpointStore.RelativeRoot.Replace('/', Path.DirectorySeparatorChar));
            if (Directory.Exists(checkpointRoot)) Directory.Delete(checkpointRoot, true);

            var session = AuthoredSession(catalogue);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var snapshot = Success(contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()), "snapshot export");
            var journal = Success(contract.Dispatch(WorldProvenanceContract.ReadName, Exact(), Empty()), "journal read");
            Assert.Equal(1L, Integer(journal, "entryCount"));

            var plan = H1ManagedScenePlan.Build(session.Current, catalogue);
            Assert.Equal(2, plan.Nodes.Length);
            Assert.Equal(session.Current.Revision, plan.WorldRevision);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(session.Current), plan.CanonicalHash);
            File.WriteAllText(Path.Combine(directory, "plan.json"), JsonSerializer.Serialize(plan, Json));
            File.WriteAllText(Path.Combine(directory, "seed.json"), JsonSerializer.Serialize(new ProofAnchor
            {
                SchemaId = "arkus.h1-10-seed@1",
                WorldId = session.Current.Id.Value,
                Revision = session.Current.Revision,
                CanonicalHash = plan.CanonicalHash,
                InputDigest = plan.InputDigest,
                CatalogueFingerprint = plan.CatalogueFingerprint,
                CapturedJournalEntries = Integer(journal, "entryCount")
            }, Json));
            Assert.NotEmpty(snapshot);
        }

        [Fact]
        public void StageB_CapturePublishedCheckpointFromEffectiveUnityBaseline()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var directory = ProofDirectory(profile);
            var baseline = Read<BaselineEvidence>(Path.Combine(directory, "baseline.json"));
            Assert.Equal("arkus.h1-10-baseline@1", baseline.SchemaId);
            Assert.False(string.IsNullOrWhiteSpace(baseline.GraphDigest));
            Assert.False(string.IsNullOrWhiteSpace(baseline.RealizationDigest));

            var catalogue = Catalogue(profile);
            var session = AuthoredSession(catalogue);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var snapshot = Success(contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()), "snapshot export");
            var journal = Success(contract.Dispatch(WorldProvenanceContract.ReadName, Exact(), Empty()), "journal read");
            Assert.Equal(1L, Integer(journal, "entryCount"));
            var plan = H1ManagedScenePlan.Build(session.Current, catalogue);
            Assert.Equal(plan.InputDigest, baseline.InputDigest);
            Assert.Equal(plan.CanonicalHash, baseline.CanonicalHash);
            Assert.Equal(plan.CatalogueFingerprint, baseline.CatalogueFingerprint);

            var environment = H1ProjectEnvironmentProbe.Capture(profile, catalogue);
            var captured = new H1ProjectCheckpointStore(profile).Capture(
                snapshot,
                journal,
                environment,
                plan,
                baseline.GraphDigest,
                baseline.RealizationDigest);
            Assert.Equal("ready", captured.State);
            File.WriteAllText(Path.Combine(directory, "captured.json"), JsonSerializer.Serialize(new CaptureEvidence
            {
                SchemaId = "arkus.h1-10-capture-evidence@1",
                CheckpointId = captured.CheckpointId,
                ManifestSha256 = captured.ManifestSha256,
                EnvironmentDigest = captured.Manifest.Environment.Digest,
                CanonicalHash = captured.Manifest.Canonical.Hash,
                Revision = captured.Manifest.Canonical.Revision,
                InputDigest = captured.Manifest.Projection.InputDigest,
                GraphDigest = captured.Manifest.Projection.ObservationGraphDigest,
                RealizationDigest = captured.Manifest.Projection.ObservationRealizationDigest
            }, Json));
        }

        [Fact]
        public void StageC_FreshProcessRestoresOnlyThroughAcceptedH0ImportAndWritesRebuildPlan()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var directory = ProofDirectory(profile);
            var catalogue = Catalogue(profile);
            var environment = H1ProjectEnvironmentProbe.Capture(profile, catalogue);
            var checkpoint = new H1ProjectCheckpointStore(profile).ReadCurrent(environment);
            Assert.Equal("ready", checkpoint.State);
            Assert.Equal(1L, Integer(H1ProjectCheckpointStore.ParsePortableObject(checkpoint.JournalJson), "entryCount"));

            // This method is launched by a distinct testhost process. Start from a genuinely empty
            // authored session and use only the accepted H0 snapshot-import capability to recover.
            var empty = new WorldState(new WorldId("world.h1-10.fresh-process"), 0, Array.Empty<WorldObject>());
            var fresh = new PortableWorldAuthoringSession(empty);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(fresh), fresh);
            var before = Success(contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()), "fresh snapshot export");
            var beforeAnchor = Map(before, "anchor");
            var portableSnapshot = H1ProjectCheckpointStore.ParsePortableObject(checkpoint.SnapshotJson);
            var import = Success(contract.Dispatch(
                WorldPortabilityContract.ImportName,
                Exact(),
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "h1-10-effective:" + checkpoint.CheckpointId,
                    ["expectedRevision"] = Integer(beforeAnchor, "revision"),
                    ["expectedHash"] = Text(beforeAnchor, "hash"),
                    ["snapshot"] = portableSnapshot
                })), "accepted H0 snapshot import");

            Assert.Equal("new-local-lineage", Text(import, "lineageDisposition"));
            Assert.Equal(0L, Integer(import, "retainedJournalEntries"));
            Assert.Equal(0L, Integer(import, "importedJournalEntries"));
            var importedCurrent = Map(import, "current");
            Assert.Equal(checkpoint.Manifest.Canonical.WorldId, Text(importedCurrent, "worldId"));
            Assert.Equal(checkpoint.Manifest.Canonical.Revision, Integer(importedCurrent, "revision"));
            Assert.Equal(checkpoint.Manifest.Canonical.Hash, Text(importedCurrent, "hash"));

            var freshJournal = Success(contract.Dispatch(WorldProvenanceContract.ReadName, Exact(), Empty()), "fresh journal read");
            Assert.Equal(0L, Integer(freshJournal, "entryCount"));
            Assert.Equal(checkpoint.Manifest.Canonical.Hash, Text(Map(freshJournal, "current"), "hash"));

            var after = Success(contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()), "post-import snapshot export");
            var diff = Success(contract.Dispatch(
                WorldPortabilityContract.CompareName,
                Exact(),
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["base"] = portableSnapshot,
                    ["target"] = after
                })), "semantic diff");
            Assert.True((bool)diff["sameAuthorableState"]!);

            var restoredPlan = H1ManagedScenePlan.Build(fresh.Current, catalogue);
            Assert.Equal(checkpoint.Manifest.Canonical.Hash, restoredPlan.CanonicalHash);
            Assert.Equal(checkpoint.Manifest.Canonical.Revision, restoredPlan.WorldRevision);
            Assert.Equal(checkpoint.Manifest.Projection.InputDigest, restoredPlan.InputDigest);
            Assert.Equal(checkpoint.Manifest.Environment.CatalogueFingerprint, restoredPlan.CatalogueFingerprint);
            File.WriteAllText(Path.Combine(directory, "restored-plan.json"), JsonSerializer.Serialize(restoredPlan, Json));
            File.WriteAllText(Path.Combine(directory, "restore.json"), JsonSerializer.Serialize(new RestoreEvidence
            {
                SchemaId = "arkus.h1-10-restore-evidence@1",
                CheckpointId = checkpoint.CheckpointId,
                CanonicalHash = restoredPlan.CanonicalHash,
                Revision = restoredPlan.WorldRevision,
                InputDigest = restoredPlan.InputDigest,
                LineageDisposition = Text(import, "lineageDisposition"),
                CapturedJournalEntries = 1,
                FreshJournalEntries = Integer(freshJournal, "entryCount"),
                SameAuthorableState = (bool)diff["sameAuthorableState"]!
            }, Json));
        }

        [Fact]
        public void Negative_EffectiveFingerprintMismatchDoesNotReleaseCanonicalArtifacts()
        {
            if (!Enabled()) return;
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var catalogue = Catalogue(profile);
            var expected = H1ProjectEnvironmentProbe.Capture(profile, catalogue);
            expected.BridgeFingerprint = new string(expected.BridgeFingerprint[0] == '0' ? '1' : '0', 64);
            expected.Digest = new string('f', 64);

            var checkpoint = new H1ProjectCheckpointStore(profile).ReadCurrent(expected);
            Assert.Equal("blocked", checkpoint.State);
            Assert.Contains(checkpoint.Blockers, value => value == "checkpoint.fingerprint-mismatch:bridge");
            Assert.Equal(string.Empty, checkpoint.SnapshotJson);
            Assert.Equal(string.Empty, checkpoint.JournalJson);
        }

        private static PortableWorldAuthoringSession AuthoredSession(H1CatalogueSnapshot catalogue)
        {
            var source = catalogue.Entries.First(value => value.Kind == "prefab");
            var initial = new WorldState(
                new WorldId("world.h1-10.composed"),
                7,
                new[]
                {
                    new WorldObject(new WorldObjectId("facade"), new WorldTypeId("fixture.facade")),
                    new WorldObject(new WorldObjectId("workshop"), new WorldTypeId("fixture.workshop"))
                },
                new[]
                {
                    Binding("facade", source.LogicalId, 100, 0, 0),
                    Binding("workshop", source.LogicalId, 900, 0, 0)
                });
            var session = new PortableWorldAuthoringSession(initial);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            var applied = contract.Dispatch(
                WorldMutationContract.ApplyName,
                Exact(),
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "h1-10.checkpoint.observer",
                    ["expectedRevision"] = initial.Revision,
                    ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(initial),
                    ["operations"] = new object?[]
                    {
                        new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["kind"] = "put-object",
                            ["id"] = "observer",
                            ["typeId"] = "fixture.concurrent"
                        }
                    }
                }));
            Assert.True(applied.Success, applied.Error?.MachineCode);
            return session;
        }

        private static WorldExtensionData Binding(string subject, string sourceLogicalId, long x, long y, long z)
        {
            var binding = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                ["targetSceneId"] = H1ManagedScenePlan.SceneId,
                ["source"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "prefab",
                    ["logicalId"] = sourceLogicalId
                },
                ["transform"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                    ["positionMm"] = Vector(x, y, z),
                    ["rotationMilliDegrees"] = Vector(0, 0, 0),
                    ["scalePpm"] = Vector(1000000, 1000000, 1000000)
                },
                ["components"] = Array.Empty<object?>()
            };
            var compiled = UnityBindingProducer.Compile(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["subjectId"] = subject,
                ["binding"] = binding
            });
            return new WorldExtensionData(
                UnityBindingProducer.ExtensionOwner,
                UnityBindingProducer.ExtensionSchemaVersion,
                Convert.FromBase64String((string)compiled["payloadBase64"]!),
                new WorldObjectId(subject),
                Array.Empty<WorldReference>());
        }

        private static H1CatalogueSnapshot Catalogue(H1UnityLaunchProfile profile) => H1CatalogueSnapshot.Build(
            File.ReadAllText(Path.Combine(profile.RepositoryRoot, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
            File.ReadAllText(Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.MappingRelativePath)),
            File.ReadAllText(Path.Combine(profile.RepositoryRoot, H1CatalogueSnapshot.AdoptionRelativePath)));

        private static Dictionary<string, object?> Vector(long x, long y, long z) => new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["x"] = x, ["y"] = y, ["z"] = z
        };

        private static string ProofDirectory(H1UnityLaunchProfile profile) => Path.Combine(profile.ProjectRoot, "H1-10-Reconstruction");
        private static bool Enabled() => string.Equals(Environment.GetEnvironmentVariable(EnabledVariable), "1", StringComparison.Ordinal);
        private static T Read<T>(string path) where T : class => JsonSerializer.Deserialize<T>(File.ReadAllText(path), Json) ?? throw new InvalidDataException("Invalid H1-10 proof artifact: " + path);
        private static IReadOnlyDictionary<string, object?> Success(CapabilityInvocationResult result, string phase)
        {
            Assert.True(result.Success, phase + ": " + result.Error?.MachineCode);
            return Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(result.Data);
        }
        private static IReadOnlyDictionary<string, object?> Map(IReadOnlyDictionary<string, object?> source, string key) => Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(source[key]);
        private static string Text(IReadOnlyDictionary<string, object?> source, string key) => Assert.IsType<string>(source[key]);
        private static long Integer(IReadOnlyDictionary<string, object?> source, string key) => Convert.ToInt64(source[key], CultureInfo.InvariantCulture);
        private static IReadOnlyDictionary<string, object?> Empty() => ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal));
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
        private sealed class ProofAnchor
        {
            public string SchemaId { get; set; } = ""; public string WorldId { get; set; } = ""; public long Revision { get; set; }
            public string CanonicalHash { get; set; } = ""; public string InputDigest { get; set; } = ""; public string CatalogueFingerprint { get; set; } = "";
            public long CapturedJournalEntries { get; set; }
        }
        private sealed class CaptureEvidence
        {
            public string SchemaId { get; set; } = ""; public string CheckpointId { get; set; } = ""; public string ManifestSha256 { get; set; } = "";
            public string EnvironmentDigest { get; set; } = ""; public string CanonicalHash { get; set; } = ""; public long Revision { get; set; }
            public string InputDigest { get; set; } = ""; public string GraphDigest { get; set; } = ""; public string RealizationDigest { get; set; } = "";
        }
        private sealed class RestoreEvidence
        {
            public string SchemaId { get; set; } = ""; public string CheckpointId { get; set; } = ""; public string CanonicalHash { get; set; } = "";
            public long Revision { get; set; } public string InputDigest { get; set; } = ""; public string LineageDisposition { get; set; } = "";
            public long CapturedJournalEntries { get; set; } public long FreshJournalEntries { get; set; } public bool SameAuthorableState { get; set; }
        }
    }
}
