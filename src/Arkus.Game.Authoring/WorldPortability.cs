using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Read-only HK06B semantic diff and snapshot-export surface.</summary>
    public interface IWorldPortabilityService
    {
        CapabilityInvocationResult CompareSnapshots(IReadOnlyDictionary<string, object?> request);
        CapabilityInvocationResult ExportSnapshot(IReadOnlyDictionary<string, object?> request);
    }

    /// <summary>
    /// Narrow Authoring-owned authority for replacing the complete session aggregate from a
    /// validated HK06B snapshot. This is a session rebase, not an HK04 mutation replay.
    /// </summary>
    internal interface ICanonicalWorldSnapshotImporter
    {
        CapabilityInvocationResult ImportSnapshot(IReadOnlyDictionary<string, object?> request);
    }

    internal static class CanonicalWorldSnapshotAuthority
    {
        public static ICanonicalWorldSnapshotImporter Bind(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            return service as ICanonicalWorldSnapshotImporter ?? new UnavailableWorldSnapshotImporter();
        }
    }

    public sealed class UnavailableWorldPortabilityService : IWorldPortabilityService
    {
        public CapabilityInvocationResult CompareSnapshots(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return Unavailable();
        }

        public CapabilityInvocationResult ExportSnapshot(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return Unavailable();
        }

        internal static CapabilityInvocationResult Unavailable()
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.state_unavailable",
                "No portable authored-world session is bound to this runtime instance.",
                "$",
                EmptyContext(),
                true,
                "Bind a PortableWorldAuthoringSession before invoking HK06B portability capabilities."));
        }

        internal static IReadOnlyDictionary<string, object?> EmptyContext()
        {
            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal));
        }
    }

    internal sealed class UnavailableWorldSnapshotImporter : ICanonicalWorldSnapshotImporter
    {
        public CapabilityInvocationResult ImportSnapshot(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return UnavailableWorldPortabilityService.Unavailable();
        }
    }

    /// <summary>
    /// H0 authored-state holder that composes the accepted transactional session with HK06B
    /// snapshot rebase semantics. Ordinary mutations remain delegated to the accepted HK04/HK06A
    /// session; successful import atomically swaps the whole inner lineage only after full snapshot
    /// validation and optimistic current-state reconciliation.
    /// </summary>
    public sealed partial class PortableWorldAuthoringSession :
        IWorldStateSource,
        IWorldMutationService,
        IWorldValidationService,
        IWorldProvenanceService,
        IWorldPortabilityService,
        ICanonicalWorldMutationCommitter,
        ICanonicalWorldSnapshotImporter
    {
        private readonly object _gate = new object();
        private TransactionalWorldAuthoringSession _inner;

        public PortableWorldAuthoringSession(WorldState initialState)
        {
            _inner = new TransactionalWorldAuthoringSession(
                initialState ?? throw new ArgumentNullException(nameof(initialState)));
        }

        public WorldState Current
        {
            get
            {
                lock (_gate)
                {
                    return _inner.Current;
                }
            }
        }

        public CapabilityInvocationResult Plan(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            lock (_gate) return _inner.Plan(request);
        }

        public CapabilityInvocationResult DryRun(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            lock (_gate) return _inner.DryRun(request);
        }

        public CapabilityInvocationResult ValidateCurrent(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            lock (_gate) return _inner.ValidateCurrent(request);
        }

        public CapabilityInvocationResult ValidateProposed(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            lock (_gate) return _inner.ValidateProposed(request);
        }

        public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            lock (_gate) return _inner.ReadJournal(request);
        }

        CapabilityInvocationResult ICanonicalWorldMutationCommitter.Apply(
            IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            lock (_gate)
            {
                return CanonicalWorldMutationAuthority.Bind(_inner).Apply(request);
            }
        }

        public CapabilityInvocationResult CompareSnapshots(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return WorldPortabilityEngine.CompareSnapshots(request);
        }

        public CapabilityInvocationResult ExportSnapshot(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Count != 0)
            {
                return WorldPortabilityEngine.Failure(
                    "world.snapshot.invalid_request",
                    "Snapshot export takes an empty request.",
                    "$",
                    false,
                    "Use the empty request declared by system.describe.");
            }

            lock (_gate)
            {
                return CapabilityInvocationResult.Succeeded(WorldSnapshotArtifact.CreateData(_inner.Current));
            }
        }

        CapabilityInvocationResult ICanonicalWorldSnapshotImporter.ImportSnapshot(
            IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!WorldPortabilityEngine.OnlyFields(request, "expectedRevision", "expectedHash", "snapshot") ||
                !WorldPortabilityEngine.TryInteger(request, "expectedRevision", out var expectedRevision) ||
                expectedRevision < 0)
            {
                return WorldPortabilityEngine.Failure(
                    "world.snapshot.invalid_request",
                    "Snapshot import requires only a non-negative expectedRevision, expectedHash and snapshot.",
                    "$",
                    false,
                    "Refresh world.summary and use the import request declared by system.describe.");
            }

            if (!WorldPortabilityEngine.TryString(request, "expectedHash", out var expectedHash) ||
                !WorldPortabilityEngine.IsCanonicalHash(expectedHash))
            {
                return WorldPortabilityEngine.Failure(
                    "world.snapshot.invalid_request",
                    "expectedHash must be a lowercase 64-character SHA-256 hex string.",
                    "$.expectedHash",
                    false,
                    "Refresh world.summary and use its canonical hash.");
            }

            if (!request.TryGetValue("snapshot", out var rawSnapshot) ||
                !(rawSnapshot is IReadOnlyDictionary<string, object?> snapshotData))
            {
                return WorldPortabilityEngine.Failure(
                    "world.snapshot.invalid_request",
                    "snapshot must be an HK06B snapshot object.",
                    "$.snapshot",
                    false,
                    "Pass an artifact produced by authoring.snapshot.export@1.0.");
            }

            var parseError = WorldSnapshotArtifact.TryRead(snapshotData, "$.snapshot", out var imported);
            if (parseError != null) return CapabilityInvocationResult.Failed(parseError);

            lock (_gate)
            {
                var previousState = _inner.Current;
                var previousAnchor = AuthoredWorldAnchor.FromState(previousState);
                if (previousAnchor.Revision != expectedRevision ||
                    !string.Equals(previousAnchor.Hash, expectedHash, StringComparison.Ordinal))
                {
                    return WorldPortabilityEngine.Failure(
                        "world.snapshot.concurrent_update",
                        "Canonical state advanced after the import precondition was captured; no state was replaced.",
                        "$.expectedRevision",
                        true,
                        "Refresh world.summary and retry against the current revision/hash.");
                }

                // One reference swap publishes the imported canonical state together with a fresh
                // HK06A lineage base, empty receipts and an empty local mutation journal.
                _inner = new TransactionalWorldAuthoringSession(imported!);
                var currentAnchor = AuthoredWorldAnchor.FromState(imported!);
                return CapabilityInvocationResult.Succeeded(WorldPortabilityEngine.ReadOnly(
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["schemaId"] = WorldPortabilityContract.ImportResultSchemaId,
                        ["previous"] = previousAnchor.ToData(),
                        ["current"] = currentAnchor.ToData(),
                        ["lineageDisposition"] = "new-local-lineage",
                        ["retainedJournalEntries"] = 0,
                        ["importedJournalEntries"] = 0
                    }));
            }
        }
    }

    internal sealed class WorldSemanticChange
    {
        public WorldSemanticChange(
            string resource,
            string resourceKind,
            string action,
            IEnumerable<string> fields,
            IEnumerable<string> relationsAdded,
            IEnumerable<string> relationsRemoved)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            ResourceKind = resourceKind ?? throw new ArgumentNullException(nameof(resourceKind));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Fields = Copy(fields, nameof(fields));
            RelationsAdded = Copy(relationsAdded, nameof(relationsAdded));
            RelationsRemoved = Copy(relationsRemoved, nameof(relationsRemoved));
        }

        public string Resource { get; }
        public string ResourceKind { get; }
        public string Action { get; }
        public IReadOnlyList<string> Fields { get; }
        public IReadOnlyList<string> RelationsAdded { get; }
        public IReadOnlyList<string> RelationsRemoved { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            return WorldPortabilityEngine.ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["resource"] = Resource,
                ["resourceKind"] = ResourceKind,
                ["action"] = Action,
                ["fields"] = WorldPortabilityEngine.ToObjectList(Fields),
                ["relationsAdded"] = WorldPortabilityEngine.ToObjectList(RelationsAdded),
                ["relationsRemoved"] = WorldPortabilityEngine.ToObjectList(RelationsRemoved)
            });
        }

        private static IReadOnlyList<string> Copy(IEnumerable<string> source, string parameterName)
        {
            if (source == null) throw new ArgumentNullException(parameterName);
            var values = new List<string>();
            foreach (var value in source)
                values.Add(value ?? throw new ArgumentException("Semantic change values cannot contain null.", parameterName));
            return values.AsReadOnly();
        }
    }

    internal static class WorldSemanticDiffEngine
    {
        public static IReadOnlyList<WorldSemanticChange> Compare(WorldState before, WorldState after)
        {
            if (before == null) throw new ArgumentNullException(nameof(before));
            if (after == null) throw new ArgumentNullException(nameof(after));
            WorldStateValidator.ValidateOrThrow(before);
            WorldStateValidator.ValidateOrThrow(after);

            var changes = new List<WorldSemanticChange>();
            var beforeObjects = ObjectsById(before);
            var afterObjects = ObjectsById(after);
            var objectIds = new SortedSet<string>(beforeObjects.Keys, StringComparer.Ordinal);
            objectIds.UnionWith(afterObjects.Keys);
            foreach (var id in objectIds)
            {
                beforeObjects.TryGetValue(id, out var oldValue);
                afterObjects.TryGetValue(id, out var newValue);
                var fields = new List<string>();
                if (oldValue == null || newValue == null) fields.Add("existence");
                if (oldValue == null || newValue == null || oldValue.TypeId != newValue.TypeId) fields.Add("typeId");
                if (oldValue == null || newValue == null || oldValue.ContainerId != newValue.ContainerId) fields.Add("containerId");

                var oldRelations = ObjectRelations(oldValue);
                var newRelations = ObjectRelations(newValue);
                var added = Difference(newRelations, oldRelations);
                var removed = Difference(oldRelations, newRelations);
                if (added.Count != 0 || removed.Count != 0) fields.Add("references");

                if (fields.Count != 0)
                {
                    changes.Add(new WorldSemanticChange(
                        "world.object:" + id,
                        "object",
                        oldValue == null ? "create" : newValue == null ? "remove" : "update",
                        fields,
                        added,
                        removed));
                }
            }

            var beforeExtensions = ExtensionsByIdentity(before);
            var afterExtensions = ExtensionsByIdentity(after);
            var extensionIds = new SortedSet<string>(beforeExtensions.Keys, StringComparer.Ordinal);
            extensionIds.UnionWith(afterExtensions.Keys);
            foreach (var id in extensionIds)
            {
                beforeExtensions.TryGetValue(id, out var oldValue);
                afterExtensions.TryGetValue(id, out var newValue);
                var fields = new List<string>();
                if (oldValue == null || newValue == null) fields.Add("existence");
                if (oldValue == null || newValue == null || !PayloadEquals(oldValue, newValue)) fields.Add("payload");

                var oldRelations = ExtensionRelations(oldValue);
                var newRelations = ExtensionRelations(newValue);
                var added = Difference(newRelations, oldRelations);
                var removed = Difference(oldRelations, newRelations);
                if (added.Count != 0 || removed.Count != 0) fields.Add("dependencies");

                if (fields.Count != 0)
                {
                    changes.Add(new WorldSemanticChange(
                        "world.extension:" + id,
                        "extension",
                        oldValue == null ? "create" : newValue == null ? "remove" : "update",
                        fields,
                        added,
                        removed));
                }
            }

            changes.Sort((left, right) => StringComparer.Ordinal.Compare(left.Resource, right.Resource));
            return changes.AsReadOnly();
        }

        private static Dictionary<string, WorldObject> ObjectsById(WorldState state)
        {
            var result = new Dictionary<string, WorldObject>(StringComparer.Ordinal);
            for (var index = 0; index < state.Objects.Count; index++)
                result.Add(state.Objects[index].Id.Value, state.Objects[index]);
            return result;
        }

        private static Dictionary<string, WorldExtensionData> ExtensionsByIdentity(WorldState state)
        {
            var result = new Dictionary<string, WorldExtensionData>(StringComparer.Ordinal);
            for (var index = 0; index < state.Extensions.Count; index++)
                result.Add(state.Extensions[index].Identity.ResourceKey, state.Extensions[index]);
            return result;
        }

        private static SortedSet<string> ObjectRelations(WorldObject? value)
        {
            var result = new SortedSet<string>(StringComparer.Ordinal);
            if (value == null) return result;
            for (var index = 0; index < value.References.Count; index++)
                result.Add(value.References[index].Kind.Value + "->" + value.References[index].TargetId.Value);
            return result;
        }

        private static SortedSet<string> ExtensionRelations(WorldExtensionData? value)
        {
            var result = new SortedSet<string>(StringComparer.Ordinal);
            if (value == null) return result;
            for (var index = 0; index < value.Dependencies.Count; index++)
                result.Add(value.Dependencies[index].Kind.Value + "->" + value.Dependencies[index].TargetId.Value);
            return result;
        }

        private static IReadOnlyList<string> Difference(
            SortedSet<string> left,
            SortedSet<string> right)
        {
            var result = new List<string>();
            foreach (var value in left)
                if (!right.Contains(value)) result.Add(value);
            return result.AsReadOnly();
        }

        private static bool PayloadEquals(WorldExtensionData left, WorldExtensionData right)
        {
            var first = left.GetPayloadCopy();
            var second = right.GetPayloadCopy();
            if (first.Length != second.Length) return false;
            for (var index = 0; index < first.Length; index++)
                if (first[index] != second[index]) return false;
            return true;
        }
    }

    internal static class WorldSnapshotArtifact
    {
        public static IReadOnlyDictionary<string, object?> CreateData(WorldState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            WorldStateValidator.ValidateOrThrow(state);
            return WorldPortabilityEngine.ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = WorldPortabilityContract.SnapshotSchemaId,
                ["snapshotVersion"] = WorldPortabilityContract.SnapshotVersion,
                ["stateFormat"] = WorldPortabilityContract.StateFormat,
                ["stateFormatVersion"] = state.SchemaVersion,
                ["anchor"] = AuthoredWorldAnchor.FromState(state).ToData(),
                ["authoredStateBase64"] = Convert.ToBase64String(CanonicalWorldStateCodec.Serialize(state)),
                ["provenanceIncluded"] = false,
                ["runtimeObservationsIncluded"] = false
            });
        }

        public static StructuredError? TryRead(
            IReadOnlyDictionary<string, object?> data,
            string path,
            out WorldState? state)
        {
            state = null;
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (!WorldPortabilityEngine.OnlyFields(
                    data,
                    "schemaId", "snapshotVersion", "stateFormat", "stateFormatVersion", "anchor",
                    "authoredStateBase64", "provenanceIncluded", "runtimeObservationsIncluded"))
            {
                return Error(
                    "world.snapshot.invalid_request",
                    "Snapshot contains fields outside the HK06B artifact grammar.",
                    path,
                    "Use an artifact produced by authoring.snapshot.export@1.0.");
            }

            if (!WorldPortabilityEngine.TryString(data, "schemaId", out var schemaId) ||
                !WorldPortabilityEngine.TryInteger(data, "snapshotVersion", out var snapshotVersion))
            {
                return Error(
                    "world.snapshot.invalid_request",
                    "Snapshot schemaId and snapshotVersion are required.",
                    path,
                    "Use the versioned HK06B snapshot grammar declared by system.describe.");
            }

            if (!string.Equals(schemaId, WorldPortabilityContract.SnapshotSchemaId, StringComparison.Ordinal) ||
                snapshotVersion != WorldPortabilityContract.SnapshotVersion)
            {
                return Error(
                    "world.snapshot.unsupported_version",
                    "Snapshot schema/version is not supported by this HK06B contract.",
                    path + ".snapshotVersion",
                    "Provide an arkus.authoring.snapshot@1 artifact.");
            }

            if (!WorldPortabilityEngine.TryString(data, "stateFormat", out var stateFormat) ||
                !string.Equals(stateFormat, WorldPortabilityContract.StateFormat, StringComparison.Ordinal) ||
                !WorldPortabilityEngine.TryInteger(data, "stateFormatVersion", out var stateFormatVersion) ||
                stateFormatVersion != WorldState.CurrentSchemaVersion)
            {
                return Error(
                    "world.snapshot.unsupported_state_format",
                    "Snapshot canonical state format/version is not supported.",
                    path + ".stateFormatVersion",
                    "Export the snapshot from a runtime using the current canonical world-state format.");
            }

            if (!data.TryGetValue("provenanceIncluded", out var rawProvenance) ||
                !(rawProvenance is bool provenanceIncluded) || provenanceIncluded ||
                !data.TryGetValue("runtimeObservationsIncluded", out var rawRuntime) ||
                !(rawRuntime is bool runtimeIncluded) || runtimeIncluded)
            {
                return Error(
                    "world.snapshot.boundary_violation",
                    "HK06B snapshots contain authored state only; provenance and runtime observations must be excluded.",
                    path,
                    "Remove external history/runtime payloads and export canonical authored state only.");
            }

            if (!WorldPortabilityEngine.TryString(data, "authoredStateBase64", out var base64) ||
                !TryCanonicalBase64(base64, out var bytes))
            {
                return Error(
                    "world.snapshot.invalid_request",
                    "authoredStateBase64 must be canonical Base64.",
                    path + ".authoredStateBase64",
                    "Use the exact Base64 payload emitted by snapshot export.");
            }

            try
            {
                state = CanonicalWorldStateCodec.Deserialize(bytes);
            }
            catch (WorldStateException exception)
            {
                return Error(
                    "world.snapshot.invalid_state",
                    "Embedded canonical authored state is invalid: " + exception.Message,
                    path + ".authoredStateBase64",
                    "Repair or re-export the canonical authored state before importing it.");
            }
            catch (ArgumentException exception)
            {
                return Error(
                    "world.snapshot.invalid_state",
                    "Embedded canonical authored state is invalid: " + exception.Message,
                    path + ".authoredStateBase64",
                    "Repair or re-export the canonical authored state before importing it.");
            }

            if (!data.TryGetValue("anchor", out var rawAnchor) ||
                !(rawAnchor is IReadOnlyDictionary<string, object?> anchor) ||
                !AnchorMatches(anchor, state))
            {
                state = null;
                return Error(
                    "world.snapshot.anchor_mismatch",
                    "Snapshot anchor does not truthfully identify the embedded canonical authored state.",
                    path + ".anchor",
                    "Do not edit snapshot metadata independently of the canonical authored-state payload.");
            }

            return null;
        }

        private static bool AnchorMatches(IReadOnlyDictionary<string, object?> anchor, WorldState state)
        {
            if (!WorldPortabilityEngine.OnlyFields(anchor, "worldId", "stateSchemaVersion", "revision", "hash") ||
                !WorldPortabilityEngine.TryString(anchor, "worldId", out var worldId) ||
                !WorldPortabilityEngine.TryInteger(anchor, "stateSchemaVersion", out var schemaVersion) ||
                !WorldPortabilityEngine.TryInteger(anchor, "revision", out var revision) ||
                !WorldPortabilityEngine.TryString(anchor, "hash", out var hash))
            {
                return false;
            }

            return string.Equals(worldId, state.Id.Value, StringComparison.Ordinal) &&
                schemaVersion == state.SchemaVersion &&
                revision == state.Revision &&
                string.Equals(hash, CanonicalWorldStateCodec.ComputeContentHash(state), StringComparison.Ordinal);
        }

        private static bool TryCanonicalBase64(string text, out byte[] bytes)
        {
            bytes = Array.Empty<byte>();
            try
            {
                bytes = Convert.FromBase64String(text);
                return string.Equals(Convert.ToBase64String(bytes), text, StringComparison.Ordinal);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static StructuredError Error(string code, string message, string path, string repairHint)
        {
            return new StructuredError(
                code,
                message,
                path,
                UnavailableWorldPortabilityService.EmptyContext(),
                false,
                repairHint);
        }
    }

    internal static class WorldPortabilityEngine
    {
        public static CapabilityInvocationResult CompareSnapshots(IReadOnlyDictionary<string, object?> request)
        {
            if (!OnlyFields(request, "base", "target"))
            {
                return Failure(
                    "world.diff.invalid_request",
                    "Semantic diff requires only base and target HK06B snapshots.",
                    "$",
                    false,
                    "Use the diff request declared by system.describe.");
            }

            if (!request.TryGetValue("base", out var rawBase) ||
                !(rawBase is IReadOnlyDictionary<string, object?> baseData) ||
                !request.TryGetValue("target", out var rawTarget) ||
                !(rawTarget is IReadOnlyDictionary<string, object?> targetData))
            {
                return Failure(
                    "world.diff.invalid_request",
                    "base and target must each be HK06B snapshot objects.",
                    "$",
                    false,
                    "Pass artifacts produced by authoring.snapshot.export@1.0.");
            }

            var baseError = WorldSnapshotArtifact.TryRead(baseData, "$.base", out var before);
            if (baseError != null) return CapabilityInvocationResult.Failed(baseError);
            var targetError = WorldSnapshotArtifact.TryRead(targetData, "$.target", out var after);
            if (targetError != null) return CapabilityInvocationResult.Failed(targetError);

            if (!string.Equals(before!.Id.Value, after!.Id.Value, StringComparison.Ordinal) ||
                before.SchemaVersion != after.SchemaVersion)
            {
                return Failure(
                    "world.diff.incompatible_world",
                    "Semantic diff requires snapshots from the same canonical world identity and state schema.",
                    "$",
                    false,
                    "Compare snapshots from the same world lineage/schema or treat them as separate worlds.");
            }

            var changes = WorldSemanticDiffEngine.Compare(before, after);
            var changeData = new List<object?>();
            for (var index = 0; index < changes.Count; index++) changeData.Add(changes[index].ToData());
            return CapabilityInvocationResult.Succeeded(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = WorldPortabilityContract.DiffSchemaId,
                ["sameAuthorableState"] = changes.Count == 0,
                ["base"] = AuthoredWorldAnchor.FromState(before).ToData(),
                ["target"] = AuthoredWorldAnchor.FromState(after).ToData(),
                ["changes"] = changeData.AsReadOnly()
            }));
        }

        internal static CapabilityInvocationResult Failure(
            string code,
            string message,
            string path,
            bool retryable,
            string repairHint)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                code,
                message,
                path,
                UnavailableWorldPortabilityService.EmptyContext(),
                retryable,
                repairHint));
        }

        internal static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> values)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(values, StringComparer.Ordinal));
        }

        internal static IReadOnlyList<object?> ToObjectList(IReadOnlyList<string> values)
        {
            var result = new List<object?>();
            for (var index = 0; index < values.Count; index++) result.Add(values[index]);
            return result.AsReadOnly();
        }

        internal static bool TryString(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out string value)
        {
            value = string.Empty;
            return data.TryGetValue(key, out var raw) && raw is string text && (value = text) != null;
        }

        internal static bool TryInteger(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out long value)
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

        internal static bool OnlyFields(IReadOnlyDictionary<string, object?> data, params string[] allowed)
        {
            var set = new HashSet<string>(allowed, StringComparer.Ordinal);
            foreach (var key in data.Keys)
                if (!set.Contains(key)) return false;
            return true;
        }

        internal static bool IsCanonicalHash(string value)
        {
            if (value == null || value.Length != 64) return false;
            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f')))
                    return false;
            }
            return true;
        }
    }
}
