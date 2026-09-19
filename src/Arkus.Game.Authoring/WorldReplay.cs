using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Read-only HK06C replay compatibility policy.</summary>
    public interface IWorldReplayService
    {
        CapabilityInvocationResult CheckReplayCompatibility(IReadOnlyDictionary<string, object?> request);
    }

    /// <summary>
    /// Narrow Authoring-owned authority for atomically replaying accepted HK06A journal evidence.
    /// Replay is orchestration over canonical mutations, not a second mutation implementation.
    /// </summary>
    internal interface ICanonicalWorldReplayExecutor
    {
        CapabilityInvocationResult Replay(IReadOnlyDictionary<string, object?> request);
    }

    internal static class CanonicalWorldReplayAuthority
    {
        public static ICanonicalWorldReplayExecutor Bind(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (service is ICanonicalWorldReplayExecutor executor) return executor;
            return new UnavailableWorldReplayAuthority();
        }

        public static IWorldReplayService BindCompatibility(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            return service as IWorldReplayService ?? new UnavailableWorldReplayService();
        }
    }

    public sealed class UnavailableWorldReplayService : IWorldReplayService
    {
        public CapabilityInvocationResult CheckReplayCompatibility(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.state_unavailable",
                "No replay-capable portable authored-world session is bound to this runtime instance.",
                "$",
                EmptyContext(),
                true,
                "Bind a PortableWorldAuthoringSession before invoking HK06C replay capabilities."));
        }

        internal static IReadOnlyDictionary<string, object?> EmptyContext()
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(StringComparer.Ordinal));
        }
    }

    internal sealed class UnavailableWorldReplayAuthority : ICanonicalWorldReplayExecutor
    {
        public CapabilityInvocationResult Replay(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return new UnavailableWorldReplayService().CheckReplayCompatibility(request);
        }
    }

    internal sealed class ReplayJournalEntry
    {
        public ReplayJournalEntry(
            string entryId,
            long sequence,
            string idempotencyKey,
            string requestFingerprint,
            IReadOnlyDictionary<string, object?> request,
            AuthoredWorldAnchor baseAnchor,
            AuthoredWorldAnchor resultAnchor,
            IReadOnlyList<string> affectedResources)
        {
            EntryId = entryId;
            Sequence = sequence;
            IdempotencyKey = idempotencyKey;
            RequestFingerprint = requestFingerprint;
            Request = request;
            Base = baseAnchor;
            Result = resultAnchor;
            AffectedResources = affectedResources;
        }

        public string EntryId { get; }
        public long Sequence { get; }
        public string IdempotencyKey { get; }
        public string RequestFingerprint { get; }
        public IReadOnlyDictionary<string, object?> Request { get; }
        public AuthoredWorldAnchor Base { get; }
        public AuthoredWorldAnchor Result { get; }
        public IReadOnlyList<string> AffectedResources { get; }
    }

    internal sealed class ReplayJournalArtifact
    {
        public ReplayJournalArtifact(
            AuthoredWorldAnchor baseAnchor,
            AuthoredWorldAnchor currentAnchor,
            IReadOnlyList<ReplayJournalEntry> entries)
        {
            Base = baseAnchor;
            Current = currentAnchor;
            Entries = entries;
        }

        public AuthoredWorldAnchor Base { get; }
        public AuthoredWorldAnchor Current { get; }
        public IReadOnlyList<ReplayJournalEntry> Entries { get; }
    }

    /// <summary>
    /// Fail-closed parser for the accepted HK06A public journal artifact. It verifies the schema
    /// identifiers, complete sequence/anchor chain and the HK06A request + entry identity binding
    /// before any replay mutation is eligible to execute.
    /// </summary>
    internal static class ReplayJournalReader
    {
        public static StructuredError? TryRead(
            IReadOnlyDictionary<string, object?> journal,
            string path,
            out ReplayJournalArtifact? artifact)
        {
            artifact = null;
            if (!OnlyFields(journal, "schemaId", "entrySchemaId", "base", "current", "entryCount", "entries"))
                return Error("world.replay.journal_invalid", "Journal contains fields outside the accepted HK06A schema.", path);

            if (!TryString(journal, "schemaId", out var schemaId) ||
                !string.Equals(schemaId, WorldProvenanceContract.JournalSchemaId, StringComparison.Ordinal) ||
                !TryString(journal, "entrySchemaId", out var entrySchemaId) ||
                !string.Equals(entrySchemaId, WorldProvenanceContract.EntrySchemaId, StringComparison.Ordinal))
            {
                return Error(
                    "world.replay.unsupported_version",
                    "Replay accepts only the reviewed HK06A journal@1 and journal-entry@1 formats.",
                    path,
                    false,
                    "Query authoring.replay.compatibility@1.0 before replaying another format.");
            }

            if (!TryObject(journal, "base", out var baseData))
                return Error("world.replay.journal_invalid", "Journal base anchor is missing or not an object.", path + ".base");
            if (!TryAnchor(baseData, path + ".base", out var baseAnchor, out var baseError))
                return baseError!;

            if (!TryObject(journal, "current", out var currentData))
                return Error("world.replay.journal_invalid", "Journal current anchor is missing or not an object.", path + ".current");
            if (!TryAnchor(currentData, path + ".current", out var currentAnchor, out var currentError))
                return currentError!;

            if (!TryLong(journal, "entryCount", out var entryCount) || entryCount < 0 ||
                !journal.TryGetValue("entries", out var rawEntries) ||
                !(rawEntries is IReadOnlyList<object?> entries) || entryCount != entries.Count)
            {
                return Error(
                    "world.replay.sequence_invalid",
                    "Journal entryCount must exactly match the supplied entry array.",
                    path + ".entryCount");
            }

            var parsed = new List<ReplayJournalEntry>();
            var expectedBase = baseAnchor!;
            for (var index = 0; index < entries.Count; index++)
            {
                var entryPath = path + ".entries[" + index + "]";
                if (!(entries[index] is IReadOnlyDictionary<string, object?> entry))
                    return Error("world.replay.entry_invalid", "Journal entry must be an object.", entryPath);

                var entryError = TryReadEntry(entry, entryPath, index + 1L, expectedBase, out var parsedEntry);
                if (entryError != null) return entryError;
                parsed.Add(parsedEntry!);
                expectedBase = parsedEntry!.Result;
            }

            if (!SameAnchor(expectedBase, currentAnchor!))
            {
                return Error(
                    "world.replay.sequence_invalid",
                    "Journal current anchor does not equal the final chained entry result.",
                    path + ".current");
            }

            artifact = new ReplayJournalArtifact(baseAnchor!, currentAnchor!, parsed.AsReadOnly());
            return null;
        }

        private static StructuredError? TryReadEntry(
            IReadOnlyDictionary<string, object?> entry,
            string path,
            long expectedSequence,
            AuthoredWorldAnchor expectedBase,
            out ReplayJournalEntry? parsed)
        {
            parsed = null;
            if (!OnlyFields(
                    entry,
                    "schemaId", "entryId", "sequence", "capability", "requestIdentity", "request",
                    "base", "result", "affectedResources"))
                return Error("world.replay.entry_invalid", "Journal entry contains fields outside the accepted HK06A schema.", path);

            if (!TryString(entry, "schemaId", out var schemaId) ||
                !string.Equals(schemaId, WorldProvenanceContract.EntrySchemaId, StringComparison.Ordinal))
                return Error("world.replay.unsupported_version", "Journal entry schema is not journal-entry@1.", path + ".schemaId");

            if (!TryString(entry, "entryId", out var entryId) || !IsCanonicalHash(entryId))
                return Error("world.replay.entry_invalid", "entryId must be a lowercase SHA-256 identifier.", path + ".entryId");
            if (!TryLong(entry, "sequence", out var sequence) || sequence != expectedSequence)
                return Error("world.replay.sequence_invalid", "Journal sequence must be contiguous and preserve original order.", path + ".sequence");

            if (!TryObject(entry, "capability", out var capability) ||
                !OnlyFields(capability, "name", "version") ||
                !TryString(capability, "name", out var capabilityName) ||
                !string.Equals(capabilityName, WorldMutationContract.ApplyName, StringComparison.Ordinal) ||
                !TryString(capability, "version", out var capabilityVersion) ||
                !string.Equals(capabilityVersion, WorldMutationContract.ContractVersionText, StringComparison.Ordinal))
            {
                return Error("world.replay.entry_invalid", "Journal entry capability identity is not the accepted canonical mutation apply capability.", path + ".capability");
            }

            if (!TryObject(entry, "requestIdentity", out var requestIdentity) ||
                !OnlyFields(requestIdentity, "idempotencyKey", "fingerprint") ||
                !TryString(requestIdentity, "idempotencyKey", out var idempotencyKey) ||
                !TryString(requestIdentity, "fingerprint", out var requestFingerprint) ||
                !IsCanonicalHash(requestFingerprint))
            {
                return Error("world.replay.entry_invalid", "Journal request identity is malformed.", path + ".requestIdentity");
            }

            if (!TryObject(entry, "request", out var request))
                return Error("world.replay.entry_invalid", "Journal entry request is not an object.", path + ".request");
            if (!TryObject(entry, "base", out var baseData))
                return Error("world.replay.entry_invalid", "Journal entry base is not an object.", path + ".base");
            if (!TryAnchor(baseData, path + ".base", out var baseAnchor, out var baseError))
                return baseError!;
            if (!SameAnchor(baseAnchor!, expectedBase))
                return Error("world.replay.sequence_invalid", "Journal entry base does not continue the preceding result anchor.", path + ".base");
            if (!TryObject(entry, "result", out var resultData))
                return Error("world.replay.entry_invalid", "Journal entry result is not an object.", path + ".result");
            if (!TryAnchor(resultData, path + ".result", out var resultAnchor, out var resultError))
                return resultError!;

            if (!entry.TryGetValue("affectedResources", out var rawResources) ||
                !(rawResources is IReadOnlyList<object?> resourceValues))
                return Error("world.replay.entry_invalid", "affectedResources must be an array.", path + ".affectedResources");
            var resources = new List<string>();
            for (var index = 0; index < resourceValues.Count; index++)
            {
                if (!(resourceValues[index] is string resource))
                    return Error("world.replay.entry_invalid", "affectedResources entries must be strings.", path + ".affectedResources[" + index + "]");
                resources.Add(resource);
            }

            var clonedRequest = CloneObject(request);
            if (!WorldProvenanceIntegrity.EntryIdentityMatches(
                    entryId,
                    sequence,
                    capabilityName,
                    capabilityVersion,
                    idempotencyKey,
                    requestFingerprint,
                    clonedRequest,
                    baseAnchor!,
                    resultAnchor!,
                    resources))
            {
                return Error(
                    "world.replay.entry_invalid",
                    "Journal entry identity does not bind the supplied normalized request, anchors and affected resources.",
                    path + ".entryId");
            }

            parsed = new ReplayJournalEntry(
                entryId,
                sequence,
                idempotencyKey,
                requestFingerprint,
                clonedRequest,
                baseAnchor!,
                resultAnchor!,
                resources.AsReadOnly());
            return null;
        }

        internal static bool SameAnchor(AuthoredWorldAnchor left, AuthoredWorldAnchor right)
        {
            return string.Equals(left.WorldId, right.WorldId, StringComparison.Ordinal) &&
                left.StateSchemaVersion == right.StateSchemaVersion &&
                left.Revision == right.Revision &&
                string.Equals(left.Hash, right.Hash, StringComparison.Ordinal);
        }

        internal static bool TryAnchor(
            IReadOnlyDictionary<string, object?> data,
            string path,
            out AuthoredWorldAnchor? anchor,
            out StructuredError? error)
        {
            anchor = null;
            error = null;
            if (!OnlyFields(data, "worldId", "stateSchemaVersion", "revision", "hash") ||
                !TryString(data, "worldId", out var worldId) ||
                !TryLong(data, "stateSchemaVersion", out var schemaVersion) ||
                schemaVersion <= 0 || schemaVersion > int.MaxValue ||
                !TryLong(data, "revision", out var revision) || revision < 0 ||
                !TryString(data, "hash", out var hash) || !IsCanonicalHash(hash))
            {
                error = Error("world.replay.entry_invalid", "Authored anchor is not canonical.", path);
                return false;
            }

            try
            {
                anchor = new AuthoredWorldAnchor(worldId, (int)schemaVersion, revision, hash);
                return true;
            }
            catch (ArgumentException)
            {
                error = Error("world.replay.entry_invalid", "Authored anchor contains an invalid world identity.", path);
                return false;
            }
        }

        internal static StructuredError Error(
            string code,
            string message,
            string path,
            bool retryable = false,
            string? repairHint = null,
            IReadOnlyDictionary<string, object?>? context = null)
        {
            return new StructuredError(
                code,
                message,
                path,
                context ?? UnavailableWorldReplayService.EmptyContext(),
                retryable,
                repairHint);
        }

        internal static bool OnlyFields(IReadOnlyDictionary<string, object?> data, params string[] allowed)
        {
            var set = new HashSet<string>(allowed, StringComparer.Ordinal);
            foreach (var key in data.Keys)
                if (!set.Contains(key)) return false;
            return true;
        }

        internal static bool TryString(IReadOnlyDictionary<string, object?> data, string key, out string value)
        {
            value = string.Empty;
            return data.TryGetValue(key, out var raw) && raw is string text && (value = text) != null;
        }

        internal static bool TryLong(IReadOnlyDictionary<string, object?> data, string key, out long value)
        {
            value = 0;
            if (!data.TryGetValue(key, out var raw)) return false;
            switch (raw)
            {
                case sbyte current: value = current; return true;
                case byte current: value = current; return true;
                case short current: value = current; return true;
                case ushort current: value = current; return true;
                case int current: value = current; return true;
                case uint current: value = current; return true;
                case long current: value = current; return true;
                case ulong current when current <= long.MaxValue: value = (long)current; return true;
                default: return false;
            }
        }

        internal static bool TryObject(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out IReadOnlyDictionary<string, object?> value)
        {
            value = null!;
            if (!data.TryGetValue(key, out var raw) || !(raw is IReadOnlyDictionary<string, object?> current)) return false;
            value = current;
            return true;
        }

        internal static bool IsCanonicalHash(string value)
        {
            if (value == null || value.Length != 64) return false;
            for (var index = 0; index < value.Length; index++)
            {
                var c = value[index];
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'))) return false;
            }
            return true;
        }

        private static IReadOnlyDictionary<string, object?> CloneObject(IReadOnlyDictionary<string, object?> source)
        {
            var result = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var pair in source) result.Add(pair.Key, CloneValue(pair.Value));
            return new ReadOnlyDictionary<string, object?>(result);
        }

        private static object? CloneValue(object? value)
        {
            if (value is IReadOnlyDictionary<string, object?> map) return CloneObject(map);
            if (value is IReadOnlyList<object?> list)
            {
                var copy = new List<object?>();
                for (var index = 0; index < list.Count; index++) copy.Add(CloneValue(list[index]));
                return copy.AsReadOnly();
            }
            return value;
        }
    }

    public sealed partial class PortableWorldAuthoringSession :
        IWorldReplayService,
        ICanonicalWorldReplayExecutor
    {
        public CapabilityInvocationResult CheckReplayCompatibility(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!ReplayJournalReader.OnlyFields(
                    request,
                    "journalSchemaId", "entrySchemaId", "snapshotSchemaId", "snapshotVersion") ||
                !ReplayJournalReader.TryString(request, "journalSchemaId", out var journalSchemaId) ||
                !ReplayJournalReader.TryString(request, "entrySchemaId", out var entrySchemaId) ||
                !ReplayJournalReader.TryString(request, "snapshotSchemaId", out var snapshotSchemaId) ||
                !ReplayJournalReader.TryLong(request, "snapshotVersion", out var snapshotVersion) ||
                snapshotVersion < 0 || snapshotVersion > int.MaxValue)
            {
                return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                    "world.replay.invalid_request",
                    "Compatibility query requires exactly journalSchemaId, entrySchemaId, snapshotSchemaId and non-negative snapshotVersion.",
                    "$"));
            }

            var supported =
                string.Equals(journalSchemaId, WorldProvenanceContract.JournalSchemaId, StringComparison.Ordinal) &&
                string.Equals(entrySchemaId, WorldProvenanceContract.EntrySchemaId, StringComparison.Ordinal) &&
                string.Equals(snapshotSchemaId, WorldPortabilityContract.SnapshotSchemaId, StringComparison.Ordinal) &&
                snapshotVersion == WorldPortabilityContract.SnapshotVersion;

            return CapabilityInvocationResult.Succeeded(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = WorldReplayContract.CompatibilityResultSchemaId,
                ["disposition"] = supported ? "supported" : "unsupported",
                ["journalSchemaId"] = journalSchemaId,
                ["entrySchemaId"] = entrySchemaId,
                ["snapshotSchemaId"] = snapshotSchemaId,
                ["snapshotVersion"] = snapshotVersion,
                ["supportedJournalSchemaId"] = WorldProvenanceContract.JournalSchemaId,
                ["supportedEntrySchemaId"] = WorldProvenanceContract.EntrySchemaId,
                ["supportedSnapshotSchemaId"] = WorldPortabilityContract.SnapshotSchemaId,
                ["supportedSnapshotVersion"] = WorldPortabilityContract.SnapshotVersion
            }));
        }

        CapabilityInvocationResult ICanonicalWorldReplayExecutor.Replay(
            IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!ReplayJournalReader.OnlyFields(request, "expectedRevision", "expectedHash", "journal") ||
                !ReplayJournalReader.TryLong(request, "expectedRevision", out var expectedRevision) || expectedRevision < 0 ||
                !ReplayJournalReader.TryString(request, "expectedHash", out var expectedHash) ||
                !ReplayJournalReader.IsCanonicalHash(expectedHash) ||
                !ReplayJournalReader.TryObject(request, "journal", out var journalData))
            {
                return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                    "world.replay.invalid_request",
                    "Replay requires exactly non-negative expectedRevision, canonical expectedHash and an HK06A journal object.",
                    "$"));
            }

            var parseError = ReplayJournalReader.TryRead(journalData, "$.journal", out var sourceJournal);
            if (parseError != null) return CapabilityInvocationResult.Failed(parseError);

            lock (_gate)
            {
                var targetPrevious = AuthoredWorldAnchor.FromState(_inner.Current);
                if (targetPrevious.Revision != expectedRevision ||
                    !string.Equals(targetPrevious.Hash, expectedHash, StringComparison.Ordinal))
                {
                    return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                        "world.replay.concurrent_update",
                        "Canonical state does not match the replay precondition; no replay entry was published.",
                        "$.expectedRevision",
                        true,
                        "Refresh world.summary and retry only if this state is the intended replay base."));
                }

                if (!ReplayJournalReader.SameAnchor(targetPrevious, sourceJournal!.Base))
                {
                    return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                        "world.replay.base_mismatch",
                        "Target canonical anchor is not the journal base; replay is fail-closed.",
                        "$.journal.base",
                        false,
                        "Import or otherwise establish the exact accepted replay base before replaying this journal."));
                }

                var existingJournalResult = _inner.ReadJournal(EmptyRequest());
                if (!existingJournalResult.Success || existingJournalResult.Data == null ||
                    !ReplayJournalReader.TryLong(existingJournalResult.Data, "entryCount", out var existingEntryCount) ||
                    existingEntryCount != 0)
                {
                    return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                        "world.replay.target_history_not_empty",
                        "Replay will not discard or splice an existing local mutation lineage.",
                        "$",
                        false,
                        "Establish the replay base as a fresh local lineage, for example with the accepted snapshot rebase capability."));
                }

                var staged = new TransactionalWorldAuthoringSession(_inner.Current);
                var stagedCommitter = CanonicalWorldMutationAuthority.Bind(staged);
                var audit = new List<object?>();

                for (var index = 0; index < sourceJournal.Entries.Count; index++)
                {
                    var sourceEntry = sourceJournal.Entries[index];
                    var apply = stagedCommitter.Apply(sourceEntry.Request);
                    if (!apply.Success)
                    {
                        var context = new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["sequence"] = sourceEntry.Sequence,
                            ["sourceEntryId"] = sourceEntry.EntryId,
                            ["causeMachineCode"] = apply.Error?.MachineCode ?? "unknown"
                        };
                        return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                            "world.replay.step_failed",
                            "A canonical mutation step failed; the target session remains unchanged.",
                            "$.journal.entries[" + index + "]",
                            false,
                            "Inspect the source entry and canonical validation diagnostic before retrying.",
                            ReadOnly(context)));
                    }

                    var actual = AuthoredWorldAnchor.FromState(staged.Current);
                    if (!ReplayJournalReader.SameAnchor(actual, sourceEntry.Result))
                    {
                        var context = new Dictionary<string, object?>(StringComparer.Ordinal)
                        {
                            ["sequence"] = sourceEntry.Sequence,
                            ["sourceEntryId"] = sourceEntry.EntryId,
                            ["expected"] = sourceEntry.Result.ToData(),
                            ["actual"] = actual.ToData()
                        };
                        return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                            "world.replay.result_divergence",
                            "Canonical replay step persisted a result different from the journal evidence; target remains unchanged.",
                            "$.journal.entries[" + index + "].result",
                            false,
                            "Do not accept this journal/runtime combination as equivalent.",
                            ReadOnly(context)));
                    }
                }

                var localJournalResult = staged.ReadJournal(EmptyRequest());
                if (!localJournalResult.Success || localJournalResult.Data == null)
                    return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                        "world.replay.audit_failed",
                        "Staged canonical provenance could not be read; target remains unchanged.",
                        "$"));
                var localError = ReplayJournalReader.TryRead(localJournalResult.Data, "$.replayedJournal", out var localJournal);
                if (localError != null)
                    return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                        "world.replay.audit_failed",
                        "Staged canonical provenance failed HK06A integrity verification; target remains unchanged.",
                        "$"));
                if (localJournal!.Entries.Count != sourceJournal.Entries.Count)
                    return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                        "world.replay.audit_failed",
                        "Replay did not produce exactly one HK06A provenance entry per source entry; target remains unchanged.",
                        "$"));

                for (var index = 0; index < sourceJournal.Entries.Count; index++)
                {
                    var sourceEntry = sourceJournal.Entries[index];
                    var localEntry = localJournal.Entries[index];
                    if (!string.Equals(sourceEntry.EntryId, localEntry.EntryId, StringComparison.Ordinal) ||
                        !ReplayJournalReader.SameAnchor(sourceEntry.Result, localEntry.Result))
                    {
                        return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                            "world.replay.audit_divergence",
                            "Replayed HK06A provenance differs from the source entry identity; target remains unchanged.",
                            "$.journal.entries[" + index + "]"));
                    }

                    audit.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["schemaId"] = WorldReplayContract.AuditEntrySchemaId,
                        ["sequence"] = sourceEntry.Sequence,
                        ["sourceEntryId"] = sourceEntry.EntryId,
                        ["replayedEntryId"] = localEntry.EntryId,
                        ["sourceResult"] = sourceEntry.Result.ToData(),
                        ["replayedResult"] = localEntry.Result.ToData()
                    }));
                }

                var targetCurrent = AuthoredWorldAnchor.FromState(staged.Current);
                if (!ReplayJournalReader.SameAnchor(targetCurrent, sourceJournal.Current))
                    return CapabilityInvocationResult.Failed(ReplayJournalReader.Error(
                        "world.replay.final_divergence",
                        "Replay completed its steps but final canonical anchor differs from journal current; target remains unchanged.",
                        "$.journal.current"));

                // One reference swap publishes state, HK04 idempotency receipts and HK06A journal as
                // one aggregate only after the complete replay and independent audit have succeeded.
                _inner = staged;
                return CapabilityInvocationResult.Succeeded(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["schemaId"] = WorldReplayContract.ReplayResultSchemaId,
                    ["journalSchemaId"] = WorldProvenanceContract.JournalSchemaId,
                    ["entrySchemaId"] = WorldProvenanceContract.EntrySchemaId,
                    ["sourceBase"] = sourceJournal.Base.ToData(),
                    ["sourceCurrent"] = sourceJournal.Current.ToData(),
                    ["targetPrevious"] = targetPrevious.ToData(),
                    ["targetCurrent"] = targetCurrent.ToData(),
                    ["replayedEntries"] = sourceJournal.Entries.Count,
                    ["journalDisposition"] = "replayed-local-mutation-history",
                    ["audit"] = audit.AsReadOnly()
                }));
            }
        }

        private static IReadOnlyDictionary<string, object?> EmptyRequest()
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(StringComparer.Ordinal));
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> data)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(data, StringComparer.Ordinal));
        }
    }
}
