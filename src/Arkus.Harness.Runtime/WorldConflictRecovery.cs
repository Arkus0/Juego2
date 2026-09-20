using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// HK08B canonical-route recovery decoration. It never mutates state and never guesses ancestry:
    /// a bounded changed-resource delta is emitted only when the complete current local HK06A journal
    /// positively proves the request base is an ancestor of the current authored anchor.
    /// </summary>
    internal sealed class WorldConflictRecovery
    {
        private static readonly IReadOnlyDictionary<string, object?> EmptyRequest =
            ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal));

        private readonly IWorldStateSource? _source;
        private readonly IWorldProvenanceService? _provenance;

        public WorldConflictRecovery(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            _source = service as IWorldStateSource;
            _provenance = service as IWorldProvenanceService;
        }

        public CapabilityInvocationResult Enrich(
            CapabilityInvocationResult result,
            IReadOnlyDictionary<string, object?> request)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (result.Success || result.Error == null || !IsStale(result.Error.MachineCode)) return result;
            if (!TryLong(request, "expectedRevision", out var expectedRevision) ||
                !TryString(request, "expectedHash", out var expectedHash))
            {
                return result;
            }

            var recovery = Build(expectedRevision, expectedHash);
            var context = new Dictionary<string, object?>(result.Error.Context, StringComparer.Ordinal)
            {
                ["recovery"] = recovery
            };

            return CapabilityInvocationResult.Failed(new StructuredError(
                result.Error.MachineCode,
                result.Error.Message,
                result.Error.Path,
                ReadOnly(context),
                result.Error.Retryable,
                result.Error.RepairHint));
        }

        private IReadOnlyDictionary<string, object?> Build(long expectedRevision, string expectedHash)
        {
            if (_source == null)
            {
                return Fallback(
                    expectedRevision,
                    expectedHash,
                    null,
                    null,
                    WorldConflictRecoveryContract.RequiredHistoryUnavailable);
            }

            var currentState = _source.Current;
            var current = Anchor.FromState(currentState);
            if (_provenance == null)
            {
                return Fallback(
                    expectedRevision,
                    expectedHash,
                    current,
                    null,
                    WorldConflictRecoveryContract.RequiredHistoryUnavailable);
            }

            var journalResult = _provenance.ReadJournal(EmptyRequest);
            if (!journalResult.Success || journalResult.Data == null ||
                !TryAnchor(journalResult.Data, "base", out var journalBase) ||
                !TryAnchor(journalResult.Data, "current", out var journalCurrent) ||
                !AnchorsEqual(current, journalCurrent) ||
                !journalResult.Data.TryGetValue("entries", out var rawEntries) ||
                !(rawEntries is IReadOnlyList<object?> entries))
            {
                return Fallback(
                    expectedRevision,
                    expectedHash,
                    current,
                    journalBase,
                    WorldConflictRecoveryContract.RequiredHistoryUnavailable);
            }

            var parsed = new List<JournalTransition>();
            var previous = journalBase;
            long expectedSequence = 1;
            for (var index = 0; index < entries.Count; index++)
            {
                if (!(entries[index] is IReadOnlyDictionary<string, object?> entry) ||
                    !TryLong(entry, "sequence", out var sequence) || sequence != expectedSequence ||
                    !TryAnchor(entry, "base", out var entryBase) ||
                    !TryAnchor(entry, "result", out var entryResult) ||
                    !AnchorsEqual(previous, entryBase) ||
                    !SameIdentity(journalBase, entryBase) ||
                    !SameIdentity(journalBase, entryResult) ||
                    entryResult.Revision != entryBase.Revision + 1 ||
                    !TryStringList(entry, "affectedResources", out var affectedResources))
                {
                    return Fallback(
                        expectedRevision,
                        expectedHash,
                        current,
                        journalBase,
                        WorldConflictRecoveryContract.RequiredHistoryUnavailable);
                }

                parsed.Add(new JournalTransition(entryBase, entryResult, affectedResources));
                previous = entryResult;
                expectedSequence++;
            }

            if (!AnchorsEqual(previous, journalCurrent))
            {
                return Fallback(
                    expectedRevision,
                    expectedHash,
                    current,
                    journalBase,
                    WorldConflictRecoveryContract.RequiredHistoryUnavailable);
            }

            var expected = new ExpectedAnchor(expectedRevision, expectedHash);
            var startIndex = -1;
            if (Matches(expected, journalBase))
            {
                startIndex = 0;
            }
            else
            {
                for (var index = 0; index < parsed.Count; index++)
                {
                    if (Matches(expected, parsed[index].Result))
                    {
                        startIndex = index + 1;
                        break;
                    }
                }
            }

            if (startIndex < 0)
            {
                return Fallback(
                    expectedRevision,
                    expectedHash,
                    current,
                    journalBase,
                    WorldConflictRecoveryContract.ExpectedBaseNotInCurrentLineage);
            }

            var affected = new SortedSet<string>(StringComparer.Ordinal);
            for (var index = startIndex; index < parsed.Count; index++)
            {
                for (var resourceIndex = 0; resourceIndex < parsed[index].AffectedResources.Count; resourceIndex++)
                {
                    affected.Add(parsed[index].AffectedResources[resourceIndex]);
                }
            }

            var changedResources = new List<object?>();
            var currentResources = new List<object?>();
            foreach (var resource in affected)
            {
                changedResources.Add(resource);
                currentResources.Add(CurrentResourceDescriptor(resource, current));
            }

            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = WorldConflictRecoveryContract.SchemaId,
                ["expected"] = ExpectedData(expectedRevision, expectedHash),
                ["current"] = current.ToData(),
                ["disposition"] = WorldConflictRecoveryContract.SameLineageReplan,
                ["historyProof"] = HistoryProofData(
                    WorldConflictRecoveryContract.HistoryComplete,
                    journalBase,
                    startIndex,
                    parsed.Count - startIndex),
                ["changedResources"] = changedResources.AsReadOnly(),
                ["currentResources"] = currentResources.AsReadOnly()
            });
        }

        private IReadOnlyDictionary<string, object?> CurrentResourceDescriptor(string resource, Anchor current)
        {
            var descriptor = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["resource"] = resource
            };

            const string objectPrefix = "world.object:";
            if (resource.StartsWith(objectPrefix, StringComparison.Ordinal))
            {
                var id = resource.Substring(objectPrefix.Length);
                var present = false;
                if (_source != null)
                {
                    var state = _source.Current;
                    for (var index = 0; index < state.Objects.Count; index++)
                    {
                        if (string.Equals(state.Objects[index].Id.Value, id, StringComparison.Ordinal))
                        {
                            present = true;
                            break;
                        }
                    }
                }

                descriptor["kind"] = "object";
                descriptor["presence"] = present ? "present" : "absent";
                descriptor["inspection"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["name"] = WorldInspectionContract.ObjectGetName,
                    ["version"] = "1.0",
                    ["request"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["revision"] = current.Revision,
                        ["hash"] = current.Hash,
                        ["id"] = id
                    })
                });
                return ReadOnly(descriptor);
            }

            const string extensionPrefix = "world.extension:";
            if (resource.StartsWith(extensionPrefix, StringComparison.Ordinal))
            {
                var identity = resource.Substring(extensionPrefix.Length);
                if (TryParseExtensionIdentity(identity, out var owner, out var schemaVersion, out var subjectId))
                {
                    var present = false;
                    if (_source != null)
                    {
                        var state = _source.Current;
                        for (var index = 0; index < state.Extensions.Count; index++)
                        {
                            if (string.Equals(state.Extensions[index].Identity.ResourceKey, identity, StringComparison.Ordinal))
                            {
                                present = true;
                                break;
                            }
                        }
                    }

                    var request = new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["revision"] = current.Revision,
                        ["hash"] = current.Hash,
                        ["owner"] = owner,
                        ["schemaVersion"] = schemaVersion,
                        ["offset"] = 0,
                        ["limit"] = WorldInspectionService.MaximumExtensionChunkBytes,
                        ["dependencyOffset"] = 0,
                        ["dependencyLimit"] = WorldInspectionService.MaximumExtensionDependencyPageSize
                    };
                    if (subjectId != null) request["subjectId"] = subjectId;

                    descriptor["kind"] = "extension";
                    descriptor["presence"] = present ? "present" : "absent";
                    descriptor["inspection"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["name"] = WorldInspectionContract.ExtensionReadName,
                        ["version"] = "1.0",
                        ["request"] = ReadOnly(request)
                    });
                    return ReadOnly(descriptor);
                }
            }

            descriptor["kind"] = "unknown";
            descriptor["presence"] = "unknown";
            descriptor["inspection"] = null;
            return ReadOnly(descriptor);
        }

        private static IReadOnlyDictionary<string, object?> Fallback(
            long expectedRevision,
            string expectedHash,
            Anchor? current,
            Anchor? journalBase,
            string reason)
        {
            var empty = new List<object?>().AsReadOnly();
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = WorldConflictRecoveryContract.SchemaId,
                ["expected"] = ExpectedData(expectedRevision, expectedHash),
                ["current"] = current == null ? null : current.ToData(),
                ["disposition"] = WorldConflictRecoveryContract.BoundedReinspectionRequired,
                ["historyProof"] = HistoryProofData(reason, journalBase, null, null),
                ["changedResources"] = empty,
                ["currentResources"] = empty
            });
        }

        private static IReadOnlyDictionary<string, object?> ExpectedData(long revision, string hash)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = revision,
                ["hash"] = hash
            });
        }

        private static IReadOnlyDictionary<string, object?> HistoryProofData(
            string reason,
            Anchor? lineageBase,
            int? transitionStart,
            int? transitionCount)
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["reason"] = reason,
                ["lineageBase"] = lineageBase == null ? null : lineageBase.ToData()
            };
            if (transitionStart.HasValue) data["transitionStart"] = transitionStart.Value;
            if (transitionCount.HasValue) data["transitionCount"] = transitionCount.Value;
            return ReadOnly(data);
        }

        private static bool IsStale(string machineCode)
        {
            return string.Equals(machineCode, "world.change.stale_revision", StringComparison.Ordinal) ||
                string.Equals(machineCode, "world.change.stale_hash", StringComparison.Ordinal) ||
                string.Equals(machineCode, "world.change.concurrent_update", StringComparison.Ordinal);
        }

        private static bool TryAnchor(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out Anchor anchor)
        {
            anchor = null!;
            if (!data.TryGetValue(key, out var raw) || !(raw is IReadOnlyDictionary<string, object?> values)) return false;
            if (!TryString(values, "worldId", out var worldId) ||
                !TryInt(values, "stateSchemaVersion", out var stateSchemaVersion) ||
                !TryLong(values, "revision", out var revision) ||
                !TryString(values, "hash", out var hash)) return false;
            anchor = new Anchor(worldId, stateSchemaVersion, revision, hash);
            return true;
        }

        private static bool TryStringList(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out IReadOnlyList<string> values)
        {
            values = Array.Empty<string>();
            if (!data.TryGetValue(key, out var raw) || !(raw is IReadOnlyList<object?> list)) return false;
            var parsed = new List<string>();
            for (var index = 0; index < list.Count; index++)
            {
                if (!(list[index] is string value)) return false;
                parsed.Add(value);
            }
            values = parsed.AsReadOnly();
            return true;
        }

        private static bool TryParseExtensionIdentity(
            string value,
            out string owner,
            out int schemaVersion,
            out string? subjectId)
        {
            owner = string.Empty;
            schemaVersion = 0;
            subjectId = null;
            var parts = value.Split('@');
            if (parts.Length != 3 || string.IsNullOrEmpty(parts[0]) ||
                !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out schemaVersion) ||
                schemaVersion <= 0)
                return false;
            owner = parts[0];
            if (string.Equals(parts[2], "global", StringComparison.Ordinal)) return true;
            const string objectPrefix = "object:";
            if (!parts[2].StartsWith(objectPrefix, StringComparison.Ordinal)) return false;
            subjectId = parts[2].Substring(objectPrefix.Length);
            return subjectId.Length != 0;
        }

        private static bool TryString(IReadOnlyDictionary<string, object?> data, string key, out string value)
        {
            value = string.Empty;
            return data.TryGetValue(key, out var raw) && raw is string text && ((value = text) != null);
        }

        private static bool TryInt(IReadOnlyDictionary<string, object?> data, string key, out int value)
        {
            value = 0;
            if (!TryLong(data, key, out var parsed) || parsed < int.MinValue || parsed > int.MaxValue) return false;
            value = (int)parsed;
            return true;
        }

        private static bool TryLong(IReadOnlyDictionary<string, object?> data, string key, out long value)
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

        private static bool AnchorsEqual(Anchor left, Anchor right)
        {
            return SameIdentity(left, right) &&
                left.Revision == right.Revision &&
                string.Equals(left.Hash, right.Hash, StringComparison.Ordinal);
        }

        private static bool SameIdentity(Anchor left, Anchor right)
        {
            return string.Equals(left.WorldId, right.WorldId, StringComparison.Ordinal) &&
                left.StateSchemaVersion == right.StateSchemaVersion;
        }

        private static bool Matches(ExpectedAnchor expected, Anchor actual)
        {
            return expected.Revision == actual.Revision &&
                string.Equals(expected.Hash, actual.Hash, StringComparison.Ordinal);
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> values)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(values, StringComparer.Ordinal));
        }

        private sealed class JournalTransition
        {
            public JournalTransition(Anchor @base, Anchor result, IReadOnlyList<string> affectedResources)
            {
                Base = @base;
                Result = result;
                AffectedResources = affectedResources;
            }

            public Anchor Base { get; }
            public Anchor Result { get; }
            public IReadOnlyList<string> AffectedResources { get; }
        }

        private sealed class ExpectedAnchor
        {
            public ExpectedAnchor(long revision, string hash)
            {
                Revision = revision;
                Hash = hash;
            }

            public long Revision { get; }
            public string Hash { get; }
        }

        private sealed class Anchor
        {
            public Anchor(string worldId, int stateSchemaVersion, long revision, string hash)
            {
                WorldId = worldId;
                StateSchemaVersion = stateSchemaVersion;
                Revision = revision;
                Hash = hash;
            }

            public string WorldId { get; }
            public int StateSchemaVersion { get; }
            public long Revision { get; }
            public string Hash { get; }

            public static Anchor FromState(WorldState state)
            {
                return new Anchor(
                    state.Id.Value,
                    state.SchemaVersion,
                    state.Revision,
                    CanonicalWorldStateCodec.ComputeContentHash(state));
            }

            public IReadOnlyDictionary<string, object?> ToData()
            {
                return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["worldId"] = WorldId,
                    ["stateSchemaVersion"] = StateSchemaVersion,
                    ["revision"] = Revision,
                    ["hash"] = Hash
                });
            }
        }
    }
}
