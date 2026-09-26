using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Arkus.Game.Authoring;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;

namespace Arkus.H1.UnityHost
{
    /// <summary>
    /// H1-10 orchestration only: verifies the published checkpoint and current bridge environment,
    /// rebases a fresh process through the accepted H0 snapshot-import capability, then asks the
    /// accepted H1 Unity worker to perform a clean generated-output rebuild. It owns no persisted
    /// world, replay, lineage, or materialization algorithm.
    /// </summary>
    [PublicCapabilityRoute(H1UnityHostCapabilityPolicy.HostProviderId, "unity.host.checkpoint.restore", "1.0")]
    public sealed class H1ProjectCheckpointRestoreHandler : ICanonicalCapabilityHandler
    {
        private readonly H1UnityLaunchProfile _profile;
        private readonly H1ProjectCheckpointStore _store;

        public H1ProjectCheckpointRestoreHandler(H1UnityLaunchProfile profile, H1ProjectCheckpointStore store)
        {
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));

            try
            {
                H1ProjectionContract.RequireScene(request);
                var checkpoint = ReadVerifiedCheckpoint();
                if (!string.Equals(checkpoint.State, "ready", StringComparison.Ordinal))
                    return CapabilityInvocationResult.Succeeded(BlockedData(checkpoint));
                // Parity must be provable before anything is imported, deleted or rebuilt.
                H1ReconstructionParity.RequireBaseline(checkpoint.Manifest);

                // Read the fresh process's actual current anchor. This is deliberately obtained from
                // the accepted H0 export surface rather than reconstructed by H1.
                var beforeSnapshot = RequireSuccess(
                    context.Contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()),
                    "checkpoint.restore-current-export-failed");
                var beforeAnchor = RequireMap(beforeSnapshot, "anchor", "checkpoint.restore-current-anchor-invalid");

                var importedSnapshot = H1ProjectCheckpointStore.ParsePortableObject(checkpoint.SnapshotJson);
                var importRequest = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = "h1-checkpoint-restore:" + checkpoint.CheckpointId,
                    ["expectedRevision"] = RequireLong(beforeAnchor, "revision", "checkpoint.restore-current-anchor-invalid"),
                    ["expectedHash"] = RequireString(beforeAnchor, "hash", "checkpoint.restore-current-anchor-invalid"),
                    ["snapshot"] = importedSnapshot
                });

                var imported = RequireSuccess(
                    context.Contract.Dispatch(WorldPortabilityContract.ImportName, Exact(), importRequest),
                    "checkpoint.restore-import-failed");
                VerifyImportedLineage(imported, checkpoint.Manifest);

                // Snapshot import starts a truthful new local lineage. Verify that H1 did not smuggle
                // the old journal back in as mutation history.
                var freshJournal = RequireSuccess(
                    context.Contract.Dispatch(WorldProvenanceContract.ReadName, Exact(), Empty()),
                    "checkpoint.restore-journal-read-failed");
                VerifyFreshJournal(freshJournal, checkpoint.Manifest);

                // Prove canonical authored-state identity through the accepted H0 semantic diff.
                var afterSnapshot = RequireSuccess(
                    context.Contract.Dispatch(WorldPortabilityContract.ExportName, Exact(), Empty()),
                    "checkpoint.restore-post-export-failed");
                var diff = RequireSuccess(
                    context.Contract.Dispatch(
                        WorldPortabilityContract.CompareName,
                        Exact(),
                        ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["base"] = importedSnapshot,
                            ["target"] = afterSnapshot
                        })),
                    "checkpoint.restore-diff-failed");
                if (!RequireBool(diff, "sameAuthorableState", "checkpoint.restore-diff-invalid"))
                    throw Error("checkpoint.restore-canonical-drift", "The accepted H0 semantic diff reports authored-state drift after checkpoint import.");

                // Only after checkpoint/environment/canonical verification may generated Unity output
                // be deleted and rebuilt. The public composed clean-rebuild route owns that effect.
                RequireSuccess(
                    context.Contract.Dispatch(H1ProjectionCleanRebuildExecutor.Key.Name, Exact(), SceneRequest()),
                    "checkpoint.restore-clean-rebuild-failed");

                var observed = RequireSuccess(
                    context.Contract.Dispatch(H1ManagedSceneExecutor.ObserveKey.Name, Exact(), SceneRequest()),
                    "checkpoint.restore-observe-failed");
                var reconstructionDigest = H1ReconstructionParity.Require(checkpoint.Manifest, observed);

                return CapabilityInvocationResult.Succeeded(RestoredData(checkpoint, imported, observed, reconstructionDigest));
            }
            catch (H1ProjectCheckpointException exception)
            {
                return Failure(exception.Code, exception.Message);
            }
            catch (H1ProjectionException exception)
            {
                return Failure(exception.Code, exception.Message);
            }
            catch (H1CatalogueException exception)
            {
                return Failure("checkpoint.catalogue-invalid", exception.Code + ": " + exception.Message);
            }
            catch (System.IO.IOException exception)
            {
                return Failure("checkpoint.environment-io-failure", exception.GetType().Name + ": required checkpoint environment input is unavailable.");
            }
        }

        private H1ProjectCheckpointRead ReadVerifiedCheckpoint()
        {
            try
            {
                var catalogue = H1ProjectionReconciliationExecutor.LoadCatalogue(_profile);
                return _store.ReadCurrent(H1ProjectEnvironmentProbe.Capture(_profile, catalogue));
            }
            catch (H1ProjectCheckpointException exception) when (
                exception.Code.StartsWith("checkpoint.source-", StringComparison.Ordinal) ||
                exception.Code.StartsWith("checkpoint.package-", StringComparison.Ordinal) ||
                exception.Code.StartsWith("checkpoint.bridge-", StringComparison.Ordinal))
            {
                return _store.ReadCurrent(null, exception.Code);
            }
            catch (H1CatalogueException exception)
            {
                return _store.ReadCurrent(null, "checkpoint.catalogue-invalid:" + exception.Code);
            }
            catch (System.IO.IOException)
            {
                return _store.ReadCurrent(null, "checkpoint.environment-io-failure");
            }
        }

        private static void VerifyImportedLineage(
            IReadOnlyDictionary<string, object?> imported,
            H1ProjectCheckpointManifest manifest)
        {
            var current = RequireMap(imported, "current", "checkpoint.restore-import-result-invalid");
            VerifyAnchor(current, manifest.Canonical, "checkpoint.restore-import-anchor-mismatch");
            if (!string.Equals(RequireString(imported, "lineageDisposition", "checkpoint.restore-import-result-invalid"), "new-local-lineage", StringComparison.Ordinal) ||
                RequireLong(imported, "retainedJournalEntries", "checkpoint.restore-import-result-invalid") != 0 ||
                RequireLong(imported, "importedJournalEntries", "checkpoint.restore-import-result-invalid") != 0)
            {
                throw Error("checkpoint.restore-lineage-invalid", "Snapshot restore did not retain the accepted new-local-lineage / empty imported-journal semantics.");
            }
        }

        private static void VerifyFreshJournal(
            IReadOnlyDictionary<string, object?> journal,
            H1ProjectCheckpointManifest manifest)
        {
            if (RequireLong(journal, "entryCount", "checkpoint.restore-journal-invalid") != 0)
                throw Error("checkpoint.restore-lineage-invalid", "The restored process falsely retained pre-checkpoint mutation history as local history.");
            VerifyAnchor(
                RequireMap(journal, "current", "checkpoint.restore-journal-invalid"),
                manifest.Canonical,
                "checkpoint.restore-journal-anchor-mismatch");
        }

        private static IReadOnlyDictionary<string, object?> BlockedData(H1ProjectCheckpointRead checkpoint)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = H1ProjectCheckpointRestoreContract.ResultSchemaId,
                ["state"] = "blocked",
                ["checkpointId"] = checkpoint.CheckpointId,
                ["canonicalHash"] = checkpoint.Manifest.Canonical.Hash,
                ["revision"] = checkpoint.Manifest.Canonical.Revision,
                ["planInputDigest"] = checkpoint.Manifest.Projection.InputDigest,
                ["graphDigest"] = checkpoint.Manifest.Projection.ObservationGraphDigest,
                ["realizationDigest"] = checkpoint.Manifest.Projection.ObservationRealizationDigest,
                ["reconstructionDigest"] = checkpoint.Manifest.Projection.ObservationReconstructionDigest,
                ["lineageDisposition"] = "not-restored",
                ["sameAuthorableState"] = false,
                ["blockers"] = ToObjectArray(checkpoint.Blockers)
            });
        }

        private static IReadOnlyDictionary<string, object?> RestoredData(
            H1ProjectCheckpointRead checkpoint,
            IReadOnlyDictionary<string, object?> imported,
            IReadOnlyDictionary<string, object?> observed,
            string reconstructionDigest)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = H1ProjectCheckpointRestoreContract.ResultSchemaId,
                ["state"] = "restored",
                ["checkpointId"] = checkpoint.CheckpointId,
                ["canonicalHash"] = checkpoint.Manifest.Canonical.Hash,
                ["revision"] = checkpoint.Manifest.Canonical.Revision,
                ["planInputDigest"] = checkpoint.Manifest.Projection.InputDigest,
                ["graphDigest"] = RequireString(observed, "graphDigest", "checkpoint.restore-observation-invalid"),
                ["realizationDigest"] = RequireString(observed, "realizationDigest", "checkpoint.restore-observation-invalid"),
                ["reconstructionDigest"] = reconstructionDigest,
                ["lineageDisposition"] = RequireString(imported, "lineageDisposition", "checkpoint.restore-import-result-invalid"),
                ["sameAuthorableState"] = true,
                ["blockers"] = Array.Empty<object?>()
            });
        }

        private static void VerifyAnchor(
            IReadOnlyDictionary<string, object?> anchor,
            H1CheckpointAnchor expected,
            string code)
        {
            if (!string.Equals(RequireString(anchor, "worldId", code), expected.WorldId, StringComparison.Ordinal) ||
                RequireLong(anchor, "revision", code) != expected.Revision ||
                !string.Equals(RequireString(anchor, "hash", code), expected.Hash, StringComparison.Ordinal))
                throw Error(code, "Canonical anchor differs from the published checkpoint.");
        }

        private static IReadOnlyDictionary<string, object?> RequireMap(
            IReadOnlyDictionary<string, object?> source,
            string key,
            string code)
        {
            if (!source.TryGetValue(key, out var value) || value is not IReadOnlyDictionary<string, object?> map)
                throw Error(code, "Expected object field is missing: " + key + ".");
            return map;
        }

        private static string RequireString(IReadOnlyDictionary<string, object?> source, string key, string code)
        {
            if (!source.TryGetValue(key, out var value) || value is not string text)
                throw Error(code, "Expected string field is missing: " + key + ".");
            return text;
        }

        private static long RequireLong(IReadOnlyDictionary<string, object?> source, string key, string code)
        {
            if (!source.TryGetValue(key, out var value) || value == null)
                throw Error(code, "Expected integer field is missing: " + key + ".");
            try { return Convert.ToInt64(value, CultureInfo.InvariantCulture); }
            catch (Exception exception) when (exception is FormatException || exception is InvalidCastException || exception is OverflowException)
            {
                throw Error(code, "Expected integer field is invalid: " + key + ".");
            }
        }

        private static bool RequireBool(IReadOnlyDictionary<string, object?> source, string key, string code)
        {
            if (!source.TryGetValue(key, out var value) || value is not bool flag)
                throw Error(code, "Expected boolean field is missing: " + key + ".");
            return flag;
        }

        private static object?[] ToObjectArray(IEnumerable<string> values)
        {
            var result = new List<object?>();
            foreach (var value in values) result.Add(value);
            return result.ToArray();
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> values) =>
            new ReadOnlyDictionary<string, object?>(values);

        private static H1ProjectCheckpointException Error(string code, string message) =>
            new H1ProjectCheckpointException(code, message);
    }

    public static class H1ProjectCheckpointRestoreContract
    {
        public const string ResultSchemaId = "arkus.h1-project-checkpoint-restore@1";
        public static readonly CapabilityKey RestoreKey = new CapabilityKey(
            "unity.host.checkpoint.restore",
            new ContractVersion(1, 0));

        public static ContractContribution CreateContribution(
            H1UnityLaunchProfile profile,
            H1ProjectCheckpointStore store)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (store == null) throw new ArgumentNullException(nameof(store));
            return new ContractContribution(
                new[] { Definition() },
                new[] { CapabilityRoute.FromHandler(new H1ProjectCheckpointRestoreHandler(profile, store)) });
        }

        public static IReadOnlyList<UnityHostCapabilityGrant> Grants()
        {
            return new[]
            {
                new UnityHostCapabilityGrant(
                    RestoreKey,
                    UnityProjectWorkspaceAuthority.ProjectSettingsRootId,
                    H1ProjectionContract.ReferenceNamespace,
                    UnityHostResourceClass.ProjectMetadata,
                    UnityHostTimeClass.BoundedEditorEffect)
            };
        }

        private static CapabilityDefinition Definition()
        {
            var request = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["sceneLogicalId"] = SchemaNode.String(
                        format: "arkus-logical-reference",
                        logicalReferenceNamespace: H1ProjectionContract.ReferenceNamespace)
                },
                new[] { "sceneLogicalId" }));
            var success = new JsonSchemaDocument(SchemaNode.Object(
                new Dictionary<string, SchemaNode>(StringComparer.Ordinal)
                {
                    ["schemaId"] = SchemaNode.String(new[] { ResultSchemaId }),
                    ["state"] = SchemaNode.String(new[] { "restored", "blocked" }),
                    ["checkpointId"] = SchemaNode.String(),
                    ["canonicalHash"] = SchemaNode.String(),
                    ["revision"] = SchemaNode.Integer(),
                    ["planInputDigest"] = SchemaNode.String(),
                    ["graphDigest"] = SchemaNode.String(),
                    ["realizationDigest"] = SchemaNode.String(),
                    ["reconstructionDigest"] = SchemaNode.String(),
                    ["lineageDisposition"] = SchemaNode.String(new[] { "new-local-lineage", "not-restored" }),
                    ["sameAuthorableState"] = SchemaNode.Boolean(),
                    ["blockers"] = SchemaNode.Array(SchemaNode.String())
                },
                new[]
                {
                    "schemaId", "state", "checkpointId", "canonicalHash", "revision", "planInputDigest",
                    "graphDigest", "realizationDigest", "reconstructionDigest", "lineageDisposition", "sameAuthorableState", "blockers"
                }));

            return new CapabilityDefinition(
                RestoreKey,
                new ProviderMetadata(
                    H1UnityHostCapabilityPolicy.HostProviderId,
                    ProviderKind.Scoped,
                    H1UnityHostCapabilityPolicy.HostScope,
                    H1UnityHostCapabilityPolicy.HostNamespace),
                request,
                success,
                CanonicalContractSchemas.StructuredError(),
                SideEffectClass.ExternalReversible,
                DeterminismClass.EnvironmentDependent,
                new[]
                {
                    "published-checkpoint-valid",
                    "required-environment-fingerprints-compatible",
                    "canonical-restore-through-authoring.snapshot.import@1.0-only"
                },
                new[]
                {
                    "blocked-environment-does-not-mutate-canonical-or-generated-output",
                    "restored-canonical-state-matches-checkpoint-under-new-local-lineage",
                    "generated-output-rebuilt-through-public-clean-rebuild",
                    "normalized-observation-matches-checkpoint"
                },
                new ConcurrencySemantics(ConcurrencyClass.Serialized),
                new IdempotencySemantics(IdempotencyClass.Idempotent),
                new BatchingSemantics(BatchingClass.Unsupported),
                new RepairSemantics(true, true),
                new PolicySemantics(
                    PrivilegeClass.Authoring,
                    TransactionRequirement.None,
                    ProvenanceRequirement.Required),
                new CostSemantics(8, "verified H0 snapshot rebase plus clean H1 Unity reconstruction"));
        }

        private static IReadOnlyDictionary<string, object?> Empty() => H1ProjectCheckpointData.Empty();
        private static IReadOnlyDictionary<string, object?> SceneRequest() => H1ProjectCheckpointData.SceneRequest();
        private static ContractVersionRange Exact() => H1ProjectCheckpointData.Exact();
        private static IReadOnlyDictionary<string, object?> RequireSuccess(CapabilityInvocationResult result, string code) =>
            H1ProjectCheckpointData.RequireSuccess(result, code);
        private static CapabilityInvocationResult Failure(string code, string message) =>
            H1ProjectCheckpointData.Failure(code, message);
    }
}
