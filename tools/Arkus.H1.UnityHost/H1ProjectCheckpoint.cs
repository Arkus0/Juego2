using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.H1.UnityHost
{
    public sealed class H1ProjectCheckpointException : Exception
    {
        public H1ProjectCheckpointException(string code, string message) : base(message) { Code = code; }
        public string Code { get; }
    }

    public sealed class H1ProjectEnvironmentFingerprint
    {
        public string ProjectIdentity { get; set; } = "";
        public string BridgeFingerprint { get; set; } = "";
        public string ProfileFingerprint { get; set; } = "";
        public string CatalogueFingerprint { get; set; } = "";
        public string SourceFingerprint { get; set; } = "";
        public string PackageFingerprint { get; set; } = "";
        public string EditorFingerprint { get; set; } = "";
        public string Digest { get; set; } = "";

        public IEnumerable<KeyValuePair<string, string>> Components()
        {
            yield return new KeyValuePair<string, string>("project", ProjectIdentity);
            yield return new KeyValuePair<string, string>("bridge", BridgeFingerprint);
            yield return new KeyValuePair<string, string>("profile", ProfileFingerprint);
            yield return new KeyValuePair<string, string>("catalogue", CatalogueFingerprint);
            yield return new KeyValuePair<string, string>("source", SourceFingerprint);
            yield return new KeyValuePair<string, string>("package", PackageFingerprint);
            yield return new KeyValuePair<string, string>("editor", EditorFingerprint);
        }
    }

    public sealed class H1CheckpointAnchor
    {
        public string WorldId { get; set; } = "";
        public long Revision { get; set; }
        public string Hash { get; set; } = "";
    }

    public sealed class H1CheckpointArtifact
    {
        public string SchemaId { get; set; } = "";
        public string RelativePath { get; set; } = "";
        public string Sha256 { get; set; } = "";
    }

    public sealed class H1CheckpointProjection
    {
        public string SceneLogicalId { get; set; } = H1ManagedScenePlan.SceneId;
        public string PlanSchemaId { get; set; } = H1ManagedScenePlan.Schema;
        public string InputDigest { get; set; } = "";
        public string CanonicalHash { get; set; } = "";
        public long WorldRevision { get; set; }
        public string CatalogueFingerprint { get; set; } = "";
        public string ObservationGraphDigest { get; set; } = "";
        public string ObservationRealizationDigest { get; set; } = "";
    }

    public sealed class H1ProjectCheckpointManifest
    {
        public const string Schema = "arkus.h1-project-checkpoint@1";
        public string SchemaId { get; set; } = Schema;
        public string CheckpointId { get; set; } = "";
        public H1CheckpointAnchor Canonical { get; set; } = new H1CheckpointAnchor();
        public H1CheckpointArtifact Snapshot { get; set; } = new H1CheckpointArtifact();
        public H1CheckpointArtifact Journal { get; set; } = new H1CheckpointArtifact();
        public H1ProjectEnvironmentFingerprint Environment { get; set; } = new H1ProjectEnvironmentFingerprint();
        public H1CheckpointProjection Projection { get; set; } = new H1CheckpointProjection();
    }

    public sealed class H1ProjectCheckpointRead
    {
        public string State { get; set; } = "";
        public string CheckpointId { get; set; } = "";
        public string ManifestSha256 { get; set; } = "";
        public H1ProjectCheckpointManifest Manifest { get; set; } = new H1ProjectCheckpointManifest();
        public string SnapshotJson { get; set; } = "";
        public string JournalJson { get; set; } = "";
        public string[] Blockers { get; set; } = Array.Empty<string>();
    }

    public static class H1ProjectEnvironmentProbe
    {
        private static readonly string[] BridgeFiles =
        {
            "tools/Arkus.H1.UnityHost/H1ProjectCheckpoint.cs",
            "tools/Arkus.H1.UnityHost/H1ManagedScenePlan.cs",
            "tools/Arkus.H1.UnityHost/H1ManagedSceneCapability.cs",
            "tools/Arkus.H1.UnityHost/H1ProjectionReconciliation.cs",
            "tools/Arkus.H1.UnityHost/H1ProjectionReconciliationCapability.cs",
            "Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1EditorWorker.cs",
            "Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.cs",
            "Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Validation.cs",
            "Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Reconciliation.cs",
            "Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Checkpoint.cs"
        };

        private static readonly string[] PackageFiles =
        {
            "Unity/ArkusUnity/Packages/manifest.json",
            "Unity/ArkusUnity/Packages/packages-lock.json"
        };

        public static H1ProjectEnvironmentFingerprint Capture(H1UnityLaunchProfile profile, H1CatalogueSnapshot catalogue)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (catalogue == null) throw new ArgumentNullException(nameof(catalogue));

            var bridge = FingerprintFiles(profile.RepositoryRoot, BridgeFiles, "checkpoint.bridge-missing");
            var package = FingerprintFiles(profile.RepositoryRoot, PackageFiles, "checkpoint.package-missing");
            var sourceRows = new List<string>();
            foreach (var entry in catalogue.Entries
                         .Where(value => value.Path.StartsWith("Assets/Arkus/H1/SourceSlice/", StringComparison.Ordinal))
                         .GroupBy(value => value.Path, StringComparer.Ordinal)
                         .Select(group => group.First())
                         .OrderBy(value => value.Path, StringComparer.Ordinal))
            {
                var path = Path.Combine(profile.ProjectRoot, entry.Path.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(path)) throw Error("checkpoint.source-missing", "An accepted H1 SourceSlice file required by the catalogue is missing.");
                var actual = ShaBytes(File.ReadAllBytes(path));
                if (!string.Equals(actual, entry.ContentSha256, StringComparison.Ordinal))
                    throw Error("checkpoint.source-rebound", "An accepted H1 SourceSlice file no longer matches its catalogue fingerprint.");
                sourceRows.Add(entry.Path + "=" + actual);
            }
            if (sourceRows.Count == 0) throw Error("checkpoint.source-missing", "The effective H1 catalogue exposes no approved SourceSlice content.");
            var source = ShaText(string.Join("\n", sourceRows));

            var profileJson = JsonSerializer.Serialize(profile.ToPublicData());
            var profileFingerprint = ShaText(profileJson);
            var editor = ShaText(profile.EffectiveEditorVersion + "|" + profile.EffectiveEditorRevision);
            var result = new H1ProjectEnvironmentFingerprint
            {
                ProjectIdentity = profile.ProjectIdentity,
                BridgeFingerprint = bridge,
                ProfileFingerprint = profileFingerprint,
                CatalogueFingerprint = catalogue.Fingerprint,
                SourceFingerprint = source,
                PackageFingerprint = package,
                EditorFingerprint = editor
            };
            result.Digest = ShaText(string.Join("\n", result.Components().Select(value => value.Key + "=" + value.Value)));
            return result;
        }

        private static string FingerprintFiles(string root, IEnumerable<string> relativePaths, string missingCode)
        {
            var rows = new List<string>();
            foreach (var relative in relativePaths.OrderBy(value => value, StringComparer.Ordinal))
            {
                var path = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(path)) throw Error(missingCode, "A file required by the H1 checkpoint environment fingerprint is missing: " + relative);
                rows.Add(relative + "=" + ShaBytes(File.ReadAllBytes(path)));
            }
            return ShaText(string.Join("\n", rows));
        }

        internal static string ShaText(string value) => ShaBytes(Encoding.UTF8.GetBytes(value));
        internal static string ShaBytes(byte[] value) => Convert.ToHexString(SHA256.HashData(value)).ToLowerInvariant();
        private static H1ProjectCheckpointException Error(string code, string message) => new H1ProjectCheckpointException(code, message);
    }

    public sealed class H1ProjectCheckpointStore
    {
        public const string RelativeRoot = "ProjectSettings/Arkus/H1Checkpoint";
        private const string PointerSchema = "arkus.h1-project-checkpoint-current@1";
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false,
            WriteIndented = false
        };
        private readonly string _root;

        public H1ProjectCheckpointStore(H1UnityLaunchProfile profile)
            : this(Path.Combine(profile?.ProjectRoot ?? throw new ArgumentNullException(nameof(profile)), RelativeRoot.Replace('/', Path.DirectorySeparatorChar))) { }

        public H1ProjectCheckpointStore(string root)
        {
            _root = Path.GetFullPath(root ?? throw new ArgumentNullException(nameof(root)));
        }

        public string Root => _root;

        public H1ProjectCheckpointRead Capture(
            IReadOnlyDictionary<string, object?> snapshot,
            IReadOnlyDictionary<string, object?> journal,
            H1ProjectEnvironmentFingerprint environment,
            H1ManagedScenePlan plan,
            string observationGraphDigest,
            string observationRealizationDigest)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (journal == null) throw new ArgumentNullException(nameof(journal));
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (plan == null) throw new ArgumentNullException(nameof(plan));

            var canonical = SnapshotAnchor(snapshot);
            var journalCurrent = Anchor(RequireDictionary(journal, "current"));
            if (!Same(canonical, journalCurrent))
                throw Error("checkpoint.canonical-journal-mismatch", "The accepted snapshot and journal do not describe the same current canonical anchor.");
            if (canonical.WorldId != plan.WorldId || canonical.Revision != plan.WorldRevision || canonical.Hash != plan.CanonicalHash)
                throw Error("checkpoint.plan-anchor-mismatch", "The normalized Unity plan is not derived from the checkpoint canonical anchor.");
            if (environment.CatalogueFingerprint != plan.CatalogueFingerprint)
                throw Error("checkpoint.catalogue-plan-mismatch", "The checkpoint catalogue fingerprint disagrees with the normalized Unity plan.");

            var snapshotJson = JsonSerializer.Serialize(snapshot, Json);
            var journalJson = JsonSerializer.Serialize(journal, Json);
            var snapshotArtifact = new H1CheckpointArtifact
            {
                SchemaId = (string)snapshot["schemaId"]!, RelativePath = "snapshot.json", Sha256 = H1ProjectEnvironmentProbe.ShaText(snapshotJson)
            };
            var journalArtifact = new H1CheckpointArtifact
            {
                SchemaId = (string)journal["schemaId"]!, RelativePath = "journal.json", Sha256 = H1ProjectEnvironmentProbe.ShaText(journalJson)
            };
            var projection = new H1CheckpointProjection
            {
                InputDigest = plan.InputDigest,
                CanonicalHash = plan.CanonicalHash,
                WorldRevision = plan.WorldRevision,
                CatalogueFingerprint = plan.CatalogueFingerprint,
                ObservationGraphDigest = observationGraphDigest ?? "",
                ObservationRealizationDigest = observationRealizationDigest ?? ""
            };
            var checkpointId = H1ProjectEnvironmentProbe.ShaText(string.Join("\n", new[]
            {
                H1ProjectCheckpointManifest.Schema,
                canonical.WorldId,
                canonical.Revision.ToString(CultureInfo.InvariantCulture),
                canonical.Hash,
                snapshotArtifact.Sha256,
                journalArtifact.Sha256,
                environment.Digest,
                projection.InputDigest,
                projection.ObservationGraphDigest,
                projection.ObservationRealizationDigest
            }));
            var manifest = new H1ProjectCheckpointManifest
            {
                CheckpointId = checkpointId,
                Canonical = canonical,
                Snapshot = snapshotArtifact,
                Journal = journalArtifact,
                Environment = environment,
                Projection = projection
            };
            var manifestJson = JsonSerializer.Serialize(manifest, Json);
            var manifestSha = H1ProjectEnvironmentProbe.ShaText(manifestJson);

            Directory.CreateDirectory(_root);
            var checkpoints = Path.Combine(_root, "checkpoints");
            var staging = Path.Combine(_root, "staging");
            Directory.CreateDirectory(checkpoints);
            Directory.CreateDirectory(staging);
            var stage = Path.Combine(staging, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(stage);
            try
            {
                File.WriteAllText(Path.Combine(stage, snapshotArtifact.RelativePath), snapshotJson, new UTF8Encoding(false));
                File.WriteAllText(Path.Combine(stage, journalArtifact.RelativePath), journalJson, new UTF8Encoding(false));
                File.WriteAllText(Path.Combine(stage, "manifest.json"), manifestJson, new UTF8Encoding(false));
                ValidateGeneration(stage, manifest, manifestSha);

                var final = Path.Combine(checkpoints, checkpointId);
                if (Directory.Exists(final))
                {
                    var existing = File.ReadAllText(Path.Combine(final, "manifest.json"));
                    if (!string.Equals(existing, manifestJson, StringComparison.Ordinal))
                        throw Error("checkpoint.identity-collision", "A checkpoint ID resolved to different immutable manifest bytes.");
                    Directory.Delete(stage, true);
                }
                else
                {
                    Directory.Move(stage, final);
                }

                if (File.Exists(Path.Combine(_root, "fail-before-publish")))
                    throw Error("checkpoint.forced-prepublication-failure", "Checkpoint publication was interrupted before the current pointer changed.");

                var pointerJson = JsonSerializer.Serialize(new H1CheckpointPointer
                {
                    SchemaId = PointerSchema, CheckpointId = checkpointId, ManifestSha256 = manifestSha
                }, Json);
                FileH1UnityInvocationLedger.AtomicWrite(Path.Combine(_root, "current.json"), new[] { pointerJson });
                return new H1ProjectCheckpointRead
                {
                    State = "ready", CheckpointId = checkpointId, ManifestSha256 = manifestSha, Manifest = manifest,
                    SnapshotJson = snapshotJson, JournalJson = journalJson
                };
            }
            finally
            {
                if (Directory.Exists(stage)) Directory.Delete(stage, true);
            }
        }

        public H1ProjectCheckpointRead ReadCurrent(H1ProjectEnvironmentFingerprint? expectedEnvironment, params string[] preexistingBlockers)
        {
            var pointerPath = Path.Combine(_root, "current.json");
            if (!File.Exists(pointerPath)) throw Error("checkpoint.missing-current", "No current H1 project checkpoint has been published.");
            H1CheckpointPointer pointer;
            try
            {
                pointer = JsonSerializer.Deserialize<H1CheckpointPointer>(File.ReadAllText(pointerPath), Json) ?? throw new JsonException();
            }
            catch (JsonException)
            {
                throw Error("checkpoint.corrupt-current", "The H1 checkpoint current pointer is malformed.");
            }
            if (pointer.SchemaId != PointerSchema || !IsHash(pointer.CheckpointId) || !IsHash(pointer.ManifestSha256))
                throw Error("checkpoint.corrupt-current", "The H1 checkpoint current pointer identity is invalid.");

            var generation = Path.Combine(_root, "checkpoints", pointer.CheckpointId);
            var manifestPath = Path.Combine(generation, "manifest.json");
            if (!File.Exists(manifestPath)) throw Error("checkpoint.corrupt-current", "The published H1 checkpoint generation is missing its manifest.");
            var manifestJson = File.ReadAllText(manifestPath);
            if (H1ProjectEnvironmentProbe.ShaText(manifestJson) != pointer.ManifestSha256)
                throw Error("checkpoint.corrupt-current", "The current checkpoint manifest checksum does not match its publication pointer.");
            H1ProjectCheckpointManifest manifest;
            try
            {
                manifest = JsonSerializer.Deserialize<H1ProjectCheckpointManifest>(manifestJson, Json) ?? throw new JsonException();
            }
            catch (JsonException)
            {
                throw Error("checkpoint.corrupt-current", "The current checkpoint manifest is malformed.");
            }
            if (manifest.SchemaId != H1ProjectCheckpointManifest.Schema || manifest.CheckpointId != pointer.CheckpointId ||
                manifest.Environment == null || manifest.Projection == null || manifest.Canonical == null || manifest.Snapshot == null || manifest.Journal == null)
                throw Error("checkpoint.corrupt-current", "The current checkpoint manifest identity is incomplete.");

            ValidateGeneration(generation, manifest, pointer.ManifestSha256);
            var blockers = new SortedSet<string>(preexistingBlockers ?? Array.Empty<string>(), StringComparer.Ordinal);
            if (expectedEnvironment == null)
            {
                blockers.Add("checkpoint.environment-unavailable");
            }
            else
            {
                var expected = expectedEnvironment.Components().ToDictionary(value => value.Key, value => value.Value, StringComparer.Ordinal);
                foreach (var captured in manifest.Environment.Components())
                {
                    if (!expected.TryGetValue(captured.Key, out var value) || !string.Equals(value, captured.Value, StringComparison.Ordinal))
                        blockers.Add("checkpoint.fingerprint-mismatch:" + captured.Key);
                }
                if (!string.Equals(expectedEnvironment.Digest, manifest.Environment.Digest, StringComparison.Ordinal))
                    blockers.Add("checkpoint.fingerprint-mismatch:environment");
            }

            var snapshotJson = File.ReadAllText(Path.Combine(generation, manifest.Snapshot.RelativePath));
            var journalJson = File.ReadAllText(Path.Combine(generation, manifest.Journal.RelativePath));
            ValidateArtifactAnchors(snapshotJson, journalJson, manifest);
            return new H1ProjectCheckpointRead
            {
                State = blockers.Count == 0 ? "ready" : "blocked",
                CheckpointId = pointer.CheckpointId,
                ManifestSha256 = pointer.ManifestSha256,
                Manifest = manifest,
                SnapshotJson = blockers.Count == 0 ? snapshotJson : "",
                JournalJson = blockers.Count == 0 ? journalJson : "",
                Blockers = blockers.ToArray()
            };
        }

        public static IReadOnlyDictionary<string, object?> ParsePortableObject(string json)
        {
            using var document = JsonDocument.Parse(json);
            var value = Portable(document.RootElement);
            return value as IReadOnlyDictionary<string, object?> ?? throw Error("checkpoint.corrupt-artifact", "Checkpoint artifact root is not an object.");
        }

        private static void ValidateGeneration(string generation, H1ProjectCheckpointManifest manifest, string manifestSha)
        {
            if (!IsHash(manifestSha) || !IsHash(manifest.CheckpointId) || !IsHash(manifest.Snapshot.Sha256) || !IsHash(manifest.Journal.Sha256) ||
                manifest.Snapshot.RelativePath != "snapshot.json" || manifest.Journal.RelativePath != "journal.json")
                throw Error("checkpoint.invalid-generation", "Checkpoint generation identities or bounded artifact names are invalid.");
            foreach (var artifact in new[] { manifest.Snapshot, manifest.Journal })
            {
                var path = Path.Combine(generation, artifact.RelativePath);
                if (!File.Exists(path) || H1ProjectEnvironmentProbe.ShaBytes(File.ReadAllBytes(path)) != artifact.Sha256)
                    throw Error("checkpoint.invalid-generation", "A checkpoint artifact is missing or failed checksum validation.");
            }
        }

        private static void ValidateArtifactAnchors(string snapshotJson, string journalJson, H1ProjectCheckpointManifest manifest)
        {
            var snapshot = ParsePortableObject(snapshotJson);
            var journal = ParsePortableObject(journalJson);
            if (!snapshot.TryGetValue("schemaId", out var snapshotSchema) || !string.Equals(snapshotSchema as string, manifest.Snapshot.SchemaId, StringComparison.Ordinal) ||
                !journal.TryGetValue("schemaId", out var journalSchema) || !string.Equals(journalSchema as string, manifest.Journal.SchemaId, StringComparison.Ordinal))
                throw Error("checkpoint.corrupt-artifact", "Checkpoint artifact schema identity disagrees with the manifest.");
            var snapshotAnchor = SnapshotAnchor(snapshot);
            var journalCurrent = Anchor(RequireDictionary(journal, "current"));
            if (!Same(snapshotAnchor, manifest.Canonical) || !Same(journalCurrent, manifest.Canonical))
                throw Error("checkpoint.corrupt-artifact", "Checkpoint artifact canonical anchors disagree with the manifest.");
        }

        private static H1CheckpointAnchor SnapshotAnchor(IReadOnlyDictionary<string, object?> snapshot) => Anchor(RequireDictionary(snapshot, "anchor"));

        private static IReadOnlyDictionary<string, object?> RequireDictionary(IReadOnlyDictionary<string, object?> source, string key)
        {
            if (!source.TryGetValue(key, out var value) || value is not IReadOnlyDictionary<string, object?> dictionary)
                throw Error("checkpoint.corrupt-artifact", "Checkpoint artifact is missing required object: " + key);
            return dictionary;
        }

        private static H1CheckpointAnchor Anchor(IReadOnlyDictionary<string, object?> source)
        {
            if (!source.TryGetValue("worldId", out var world) || world is not string worldId ||
                !source.TryGetValue("revision", out var revisionRaw) ||
                !source.TryGetValue("hash", out var hashRaw) || hashRaw is not string hash || !IsHash(hash))
                throw Error("checkpoint.corrupt-artifact", "Checkpoint artifact contains an invalid canonical anchor.");
            return new H1CheckpointAnchor { WorldId = worldId, Revision = Convert.ToInt64(revisionRaw, CultureInfo.InvariantCulture), Hash = hash };
        }

        private static bool Same(H1CheckpointAnchor left, H1CheckpointAnchor right) =>
            left.WorldId == right.WorldId && left.Revision == right.Revision && left.Hash == right.Hash;

        private static object? Portable(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    var map = new Dictionary<string, object?>(StringComparer.Ordinal);
                    foreach (var property in value.EnumerateObject()) map[property.Name] = Portable(property.Value);
                    return new ReadOnlyDictionary<string, object?>(map);
                case JsonValueKind.Array:
                    return value.EnumerateArray().Select(Portable).ToArray();
                case JsonValueKind.String: return value.GetString();
                case JsonValueKind.Number:
                    if (value.TryGetInt64(out var integer)) return integer;
                    return value.GetDouble();
                case JsonValueKind.True: return true;
                case JsonValueKind.False: return false;
                case JsonValueKind.Null: return null;
                default: throw Error("checkpoint.corrupt-artifact", "Checkpoint JSON contains an unsupported value kind.");
            }
        }

        private static bool IsHash(string value) => value != null && value.Length == 64 && value.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'));
        private static H1ProjectCheckpointException Error(string code, string message) => new H1ProjectCheckpointException(code, message);

        private sealed class H1CheckpointPointer
        {
            public string SchemaId { get; set; } = "";
            public string CheckpointId { get; set; } = "";
            public string ManifestSha256 { get; set; } = "";
        }
    }

    public sealed class H1ProjectionCleanRebuildExecutor : IH1UnityCapabilityExecutor
    {
        public static readonly CapabilityKey Key = new CapabilityKey("unity.host.projection.clean-rebuild", new ContractVersion(1, 0));
        public const string WorkerExecutorId = "arkus.h1.worker.projection.clean-rebuild@1";
        private readonly H1ManagedSceneExecutor _inner;

        public H1ProjectionCleanRebuildExecutor(IWorldStateSource world, H1UnityLaunchProfile profile)
        {
            _inner = new H1ManagedSceneExecutor(H1ManagedSceneExecutor.MaterializeKey, H1ManagedSceneExecutor.MaterializeExecutorId, world, profile);
        }

        public CapabilityKey Capability => Key;
        public string ExecutorId => WorkerExecutorId;

        public string EncodeRequest(IReadOnlyDictionary<string, object?> request)
        {
            var payload = JsonNode.Parse(_inner.EncodeRequest(request))?.AsObject() ?? throw new JsonException("Missing managed-scene worker request.");
            payload["mode"] = "clean-rebuild";
            return payload.ToJsonString();
        }

        public CapabilityInvocationResult DecodeResult(H1UnityResultEnvelope result) => _inner.DecodeResult(result);
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.checkpoint.capture", "1.0")]
    public sealed class H1ProjectCheckpointCaptureHandler : ICanonicalCapabilityHandler
    {
        private readonly IWorldStateSource _world;
        private readonly H1UnityLaunchProfile _profile;
        private readonly H1ProjectCheckpointStore _store;
        public H1ProjectCheckpointCaptureHandler(IWorldStateSource world, H1UnityLaunchProfile profile, H1ProjectCheckpointStore store)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
        {
            try
            {
                H1ProjectionContract.RequireScene(request);
                var snapshot = RequireSuccess(context.Contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()), "checkpoint.snapshot-export-failed");
                var journal = RequireSuccess(context.Contract.Dispatch(WorldProvenanceContract.ReadName, Exact(), Empty()), "checkpoint.journal-read-failed");
                var catalogue = H1ProjectionReconciliationExecutor.LoadCatalogue(_profile);
                var environment = H1ProjectEnvironmentProbe.Capture(_profile, catalogue);
                var plan = H1ManagedScenePlan.Build(_world.Current, catalogue);

                var graphDigest = "";
                var realizationDigest = "";
                var observed = context.Contract.Dispatch(H1ManagedSceneExecutor.ObserveKey.Name, Exact(), SceneRequest());
                if (observed.Success && observed.Data != null && observed.Data.TryGetValue("current", out var currentRaw) && currentRaw is bool current && current)
                {
                    graphDigest = observed.Data.TryGetValue("graphDigest", out var graph) ? graph as string ?? "" : "";
                    realizationDigest = observed.Data.TryGetValue("realizationDigest", out var realization) ? realization as string ?? "" : "";
                }

                var captured = _store.Capture(snapshot, journal, environment, plan, graphDigest, realizationDigest);
                return CapabilityInvocationResult.Succeeded(CaptureData(captured));
            }
            catch (H1ProjectCheckpointException exception) { return Failure(exception.Code, exception.Message); }
            catch (H1CatalogueException exception) { return Failure("checkpoint.catalogue-invalid", exception.Code + ": " + exception.Message); }
            catch (H1ProjectionException exception) { return Failure(exception.Code, exception.Message); }
            catch (IOException exception) { return Failure("checkpoint.io-failure", exception.GetType().Name + ": checkpoint storage or environment input is unavailable."); }
        }
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.checkpoint.current", "1.0")]
    public sealed class H1ProjectCheckpointCurrentHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityLaunchProfile _profile;
        private readonly H1ProjectCheckpointStore _store;
        public H1ProjectCheckpointCurrentHandler(H1UnityLaunchProfile profile, H1ProjectCheckpointStore store)
        {
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request)
        {
            try
            {
                H1ProjectionContract.RequireScene(request);
                H1ProjectCheckpointRead read;
                try
                {
                    var catalogue = H1ProjectionReconciliationExecutor.LoadCatalogue(_profile);
                    read = _store.ReadCurrent(H1ProjectEnvironmentProbe.Capture(_profile, catalogue));
                }
                catch (H1ProjectCheckpointException exception) when (exception.Code.StartsWith("checkpoint.source-", StringComparison.Ordinal) ||
                                                                       exception.Code.StartsWith("checkpoint.package-", StringComparison.Ordinal) ||
                                                                       exception.Code.StartsWith("checkpoint.bridge-", StringComparison.Ordinal))
                {
                    read = _store.ReadCurrent(null, exception.Code);
                }
                catch (H1CatalogueException exception)
                {
                    read = _store.ReadCurrent(null, "checkpoint.catalogue-invalid:" + exception.Code);
                }
                catch (IOException)
                {
                    read = _store.ReadCurrent(null, "checkpoint.environment-io-failure");
                }
                return CapabilityInvocationResult.Succeeded(CurrentData(read));
            }
            catch (H1ProjectCheckpointException exception) { return Failure(exception.Code, exception.Message); }
        }
    }

    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.projection.clean-rebuild", "1.0")]
    public sealed class H1ProjectionCleanRebuildHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityEditorExecutionCoordinator _coordinator;
        public H1ProjectionCleanRebuildHandler(H1UnityEditorExecutionCoordinator coordinator) { _coordinator = coordinator; }
        public CapabilityInvocationResult Invoke(CapabilityInvocationContext context, IReadOnlyDictionary<string, object?> request) => _coordinator.Invoke(context, request);
    }

    public static class H1ProjectCheckpointContract
    {
        public const string ReferenceNamespace = "ref.arkus.h1-project-checkpoint";
        public static readonly CapabilityKey CaptureKey = new CapabilityKey("unity.host.checkpoint.capture", new ContractVersion(1, 0));
        public static readonly CapabilityKey CurrentKey = new CapabilityKey("unity.host.checkpoint.current", new ContractVersion(1, 0));

        public static ContractContribution CreateProjectCheckpointContribution(IWorldStateSource world, H1UnityLaunchProfile profile, H1ProjectCheckpointStore store)
        {
            return new ContractContribution(
                new[] { Definition(CaptureKey, SideEffectClass.ExternalReversible, CaptureSuccess(), "atomic project checkpoint publication"), Definition(CurrentKey, SideEffectClass.ReadOnly, CurrentSuccess(), "checksummed current checkpoint read and fingerprint verification") },
                new[] { CapabilityRoute.FromHandler(new H1ProjectCheckpointCaptureHandler(world, profile, store)), CapabilityRoute.FromHandler(new H1ProjectCheckpointCurrentHandler(profile, store)) });
        }

        public static ContractContribution CreateCleanRebuildContribution(H1UnityEditorExecutionCoordinator coordinator)
        {
            return new ContractContribution(
                new[] { Definition(H1ProjectionCleanRebuildExecutor.Key, SideEffectClass.ExternalReversible, H1ManagedSceneSuccessSchema(), "clean generated-output rebuild through accepted materialization") },
                new[] { CapabilityRoute.FromHandler(new H1ProjectionCleanRebuildHandler(coordinator)) });
        }

        public static IReadOnlyList<UnityHostCapabilityGrant> ProjectCheckpointGrants() => new[]
        {
            new UnityHostCapabilityGrant(CaptureKey, UnityProjectWorkspaceAuthority.ProjectSettingsRootId, ReferenceNamespace, UnityHostResourceClass.ProjectMetadata, UnityHostTimeClass.BoundedEditorEffect),
            new UnityHostCapabilityGrant(CurrentKey, UnityProjectWorkspaceAuthority.ProjectSettingsRootId, ReferenceNamespace, UnityHostResourceClass.ProjectMetadata, UnityHostTimeClass.BoundedRead)
        };

        public static IReadOnlyList<UnityHostCapabilityGrant> CleanRebuildGrants() => new[]
        {
            new UnityHostCapabilityGrant(H1ProjectionCleanRebuildExecutor.Key, UnityProjectWorkspaceAuthority.ManagedAssetsRootId, H1ProjectionContract.ReferenceNamespace, UnityHostResourceClass.ManagedAsset, UnityHostTimeClass.BoundedEditorEffect)
        };

        private static CapabilityDefinition Definition(CapabilityKey key, SideEffectClass effect, JsonSchemaDocument success, string cost) => new CapabilityDefinition(
            key,
            new ProviderMetadata(H1UnityHostCapabilityPolicy.HostProviderId, ProviderKind.Scoped, H1UnityHostCapabilityPolicy.HostScope, H1UnityHostCapabilityPolicy.HostNamespace),
            SceneRequestSchema(), success, CanonicalContractSchemas.StructuredError(), effect, DeterminismClass.EnvironmentDependent,
            new[] { "fixed-project-checkpoint-root", "fixed-managed-scene", "canonical-snapshot-and-journal-remain-h0-owned" },
            new[] { "canonical-authority-remains-h0", "fingerprint-or-clean-rebuild-outcome-is-structured" },
            new ConcurrencySemantics(ConcurrencyClass.Serialized), new IdempotencySemantics(IdempotencyClass.Idempotent),
            new BatchingSemantics(BatchingClass.Unsupported), new RepairSemantics(true, true),
            new PolicySemantics(PrivilegeClass.Authoring, TransactionRequirement.None, ProvenanceRequirement.Required),
            new CostSemantics(1, cost));

        private static JsonSchemaDocument SceneRequestSchema() => new JsonSchemaDocument(SchemaNode.Object(
            new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
            {
                ["sceneLogicalId"] = SchemaNode.String(format: "arkus-logical-reference", logicalReferenceNamespace: H1ProjectionContract.ReferenceNamespace)
            }, new[] { "sceneLogicalId" }));

        private static JsonSchemaDocument CaptureSuccess() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-project-checkpoint-capture@1" }),
            ["state"] = SchemaNode.String(new[] { "ready" }), ["checkpointId"] = SchemaNode.String(), ["manifestSha256"] = SchemaNode.String(),
            ["canonicalHash"] = SchemaNode.String(), ["revision"] = SchemaNode.Integer(), ["planInputDigest"] = SchemaNode.String(), ["environmentDigest"] = SchemaNode.String(),
            ["observationGraphDigest"] = SchemaNode.String(), ["observationRealizationDigest"] = SchemaNode.String()
        }, new[] { "schemaId", "state", "checkpointId", "manifestSha256", "canonicalHash", "revision", "planInputDigest", "environmentDigest", "observationGraphDigest", "observationRealizationDigest" }));

        private static JsonSchemaDocument CurrentSuccess() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-project-checkpoint-current@1" }),
            ["state"] = SchemaNode.String(new[] { "ready", "blocked" }), ["checkpointId"] = SchemaNode.String(), ["manifestSha256"] = SchemaNode.String(),
            ["canonicalHash"] = SchemaNode.String(), ["revision"] = SchemaNode.Integer(), ["planInputDigest"] = SchemaNode.String(), ["environmentDigest"] = SchemaNode.String(),
            ["observationGraphDigest"] = SchemaNode.String(), ["observationRealizationDigest"] = SchemaNode.String(), ["blockers"] = SchemaNode.Array(SchemaNode.String()),
            ["snapshot"] = WorldPortabilityContract.SnapshotSchema().Root,
            ["journal"] = WorldProvenanceContract.JournalResultSchema().Root
        }, new[] { "schemaId", "state", "checkpointId", "manifestSha256", "canonicalHash", "revision", "planInputDigest", "environmentDigest", "observationGraphDigest", "observationRealizationDigest", "blockers" }));

        private static JsonSchemaDocument H1ManagedSceneSuccessSchema() => new JsonSchemaDocument(SchemaNode.Object(new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
        {
            ["schemaId"] = SchemaNode.String(new[] { "arkus.h1-managed-scene-observation@1" }), ["sceneLogicalId"] = SchemaNode.String(),
            ["active"] = SchemaNode.Boolean(), ["current"] = SchemaNode.Boolean(), ["generationId"] = SchemaNode.String(), ["inputDigest"] = SchemaNode.String(),
            ["canonicalHash"] = SchemaNode.String(), ["catalogueFingerprint"] = SchemaNode.String(), ["graphDigest"] = SchemaNode.String(), ["realizationDigest"] = SchemaNode.String(),
            ["nodes"] = SchemaNode.Array(SchemaNode.Object(additionalPropertiesAllowed: true))
        }, new[] { "schemaId", "sceneLogicalId", "active", "current", "generationId", "inputDigest", "canonicalHash", "catalogueFingerprint", "graphDigest", "realizationDigest", "nodes" }));
    }

    internal static class H1ProjectCheckpointData
    {
        public static IReadOnlyDictionary<string, object?> CaptureData(H1ProjectCheckpointRead read) => ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["schemaId"] = "arkus.h1-project-checkpoint-capture@1", ["state"] = "ready", ["checkpointId"] = read.CheckpointId,
            ["manifestSha256"] = read.ManifestSha256, ["canonicalHash"] = read.Manifest.Canonical.Hash, ["revision"] = read.Manifest.Canonical.Revision,
            ["planInputDigest"] = read.Manifest.Projection.InputDigest, ["environmentDigest"] = read.Manifest.Environment.Digest,
            ["observationGraphDigest"] = read.Manifest.Projection.ObservationGraphDigest, ["observationRealizationDigest"] = read.Manifest.Projection.ObservationRealizationDigest
        });

        public static IReadOnlyDictionary<string, object?> CurrentData(H1ProjectCheckpointRead read)
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.h1-project-checkpoint-current@1", ["state"] = read.State, ["checkpointId"] = read.CheckpointId,
                ["manifestSha256"] = read.ManifestSha256, ["canonicalHash"] = read.Manifest.Canonical.Hash, ["revision"] = read.Manifest.Canonical.Revision,
                ["planInputDigest"] = read.Manifest.Projection.InputDigest, ["environmentDigest"] = read.Manifest.Environment.Digest,
                ["observationGraphDigest"] = read.Manifest.Projection.ObservationGraphDigest, ["observationRealizationDigest"] = read.Manifest.Projection.ObservationRealizationDigest,
                ["blockers"] = read.Blockers.Cast<object?>().ToArray()
            };
            if (read.State == "ready")
            {
                data["snapshot"] = H1ProjectCheckpointStore.ParsePortableObject(read.SnapshotJson);
                data["journal"] = H1ProjectCheckpointStore.ParsePortableObject(read.JournalJson);
            }
            return ReadOnly(data);
        }

        public static CapabilityInvocationResult Failure(string code, string message) => CapabilityInvocationResult.Failed(new StructuredError(
            code, message, "$", ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)), false,
            "Repair the checkpoint artifact or required project fingerprint before retrying."));

        public static IReadOnlyDictionary<string, object?> RequireSuccess(CapabilityInvocationResult result, string code)
        {
            if (!result.Success || result.Data == null) throw new H1ProjectCheckpointException(code, result.Error?.MachineCode ?? "Nested accepted H0 capability failed.");
            return result.Data;
        }

        public static IReadOnlyDictionary<string, object?> Empty() => ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal));
        public static IReadOnlyDictionary<string, object?> SceneRequest() => ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal) { ["sceneLogicalId"] = H1ManagedScenePlan.SceneId });
        public static ContractVersionRange Exact() => ContractVersionRange.Exact(new ContractVersion(1, 0));
        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> value) => new ReadOnlyDictionary<string, object?>(value);
    }

    internal static class H1ProjectCheckpointAliases
    {
        public static IReadOnlyDictionary<string, object?> CaptureData(H1ProjectCheckpointRead read) => H1ProjectCheckpointData.CaptureData(read);
        public static IReadOnlyDictionary<string, object?> CurrentData(H1ProjectCheckpointRead read) => H1ProjectCheckpointData.CurrentData(read);
        public static CapabilityInvocationResult Failure(string code, string message) => H1ProjectCheckpointData.Failure(code, message);
        public static IReadOnlyDictionary<string, object?> RequireSuccess(CapabilityInvocationResult result, string code) => H1ProjectCheckpointData.RequireSuccess(result, code);
        public static IReadOnlyDictionary<string, object?> Empty() => H1ProjectCheckpointData.Empty();
        public static IReadOnlyDictionary<string, object?> SceneRequest() => H1ProjectCheckpointData.SceneRequest();
        public static ContractVersionRange Exact() => H1ProjectCheckpointData.Exact();
    }

    // Keep handler code terse without widening any public authority.
    file static class H1ProjectCheckpointHandlerImports
    {
        public static IReadOnlyDictionary<string, object?> CaptureData(H1ProjectCheckpointRead read) => H1ProjectCheckpointData.CaptureData(read);
        public static IReadOnlyDictionary<string, object?> CurrentData(H1ProjectCheckpointRead read) => H1ProjectCheckpointData.CurrentData(read);
        public static CapabilityInvocationResult Failure(string code, string message) => H1ProjectCheckpointData.Failure(code, message);
        public static IReadOnlyDictionary<string, object?> RequireSuccess(CapabilityInvocationResult result, string code) => H1ProjectCheckpointData.RequireSuccess(result, code);
        public static IReadOnlyDictionary<string, object?> Empty() => H1ProjectCheckpointData.Empty();
        public static IReadOnlyDictionary<string, object?> SceneRequest() => H1ProjectCheckpointData.SceneRequest();
        public static ContractVersionRange Exact() => H1ProjectCheckpointData.Exact();
    }
}