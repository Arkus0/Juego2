using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>Pure, deterministic and bounded reads over the canonical HK02 state.</summary>
    public sealed class WorldInspectionService : IWorldInspectionService
    {
        public const int MaximumPageSize = 100;
        public const int DefaultPageSize = 50;
        public const int MaximumExtensionChunkBytes = 768;
        public const int MaximumExtensionDependencyPageSize = 100;

        private const string CursorVersion = "arkus-cursor-v1";
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        private readonly IWorldStateSource _source;

        public WorldInspectionService(IWorldStateSource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public CapabilityInvocationResult Summary(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var snapshot = Capture();
            var referenceCount = 0;
            for (var index = 0; index < snapshot.State.Objects.Count; index++)
            {
                referenceCount += snapshot.State.Objects[index].References.Count;
            }

            return Success(snapshot, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["objectCount"] = snapshot.State.Objects.Count,
                ["referenceCount"] = referenceCount,
                ["extensionCount"] = snapshot.State.Extensions.Count
            });
        }

        public CapabilityInvocationResult GetObject(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var snapshot = Capture();
            var anchor = ValidateAnchor(snapshot, request);
            if (anchor != null)
            {
                return anchor;
            }

            if (!TryGetRequiredString(request, "id", out var id) || !IsStableToken(id))
            {
                return SelectorError("$.id", "Object id must use the declared stable identifier grammar.");
            }

            var projectionError = ReadObjectFields(request, out var fields);
            if (projectionError != null)
            {
                return projectionError;
            }

            for (var index = 0; index < snapshot.State.Objects.Count; index++)
            {
                var current = snapshot.State.Objects[index];
                if (string.Equals(current.Id.Value, id, StringComparison.Ordinal))
                {
                    return Success(snapshot, new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["object"] = ProjectObject(current, fields)
                    });
                }
            }

            return Failure(
                "world.object_not_found",
                "The requested canonical object does not exist in this world revision.",
                "$.id",
                new Dictionary<string, object?>(StringComparer.Ordinal) { ["id"] = id },
                false,
                "Refresh world inspection and select an object id present in this revision.");
        }

        public CapabilityInvocationResult QueryObjects(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var snapshot = Capture();
            var anchor = ValidateAnchor(snapshot, request);
            if (anchor != null)
            {
                return anchor;
            }

            var limitError = ReadLimit(request, MaximumPageSize, DefaultPageSize, out var limit);
            if (limitError != null)
            {
                return limitError;
            }

            var projectionError = ReadObjectFields(request, out var fields);
            if (projectionError != null)
            {
                return projectionError;
            }

            var filterError = ReadObjectFilter(request, out var filter);
            if (filterError != null)
            {
                return filterError;
            }

            var fingerprint = HashFingerprint(
                "object\n" + filter.Fingerprint() +
                "\nfields=" + ((int)fields).ToString(CultureInfo.InvariantCulture) +
                "\nlimit=" + limit.ToString(CultureInfo.InvariantCulture));
            var cursorError = ReadCursor(
                snapshot,
                request,
                WorldInspectionContract.ObjectQueryName,
                fingerprint,
                out var offset);
            if (cursorError != null)
            {
                return cursorError;
            }

            var matched = new List<WorldObject>();
            for (var index = 0; index < snapshot.State.Objects.Count; index++)
            {
                var current = snapshot.State.Objects[index];
                if (filter.Matches(current))
                {
                    matched.Add(current);
                }
            }

            matched.Sort((left, right) => left.Id.CompareTo(right.Id));
            if (offset > matched.Count)
            {
                return CursorError("Cursor offset is outside the deterministic result set.");
            }

            var end = Math.Min(matched.Count, offset + limit);
            var items = new List<object?>();
            for (var index = offset; index < end; index++)
            {
                items.Add(ProjectObject(matched[index], fields));
            }

            var payload = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["items"] = items.AsReadOnly()
            };
            if (end < matched.Count)
            {
                payload["nextCursor"] = CreateCursor(
                    WorldInspectionContract.ObjectQueryName,
                    snapshot,
                    fingerprint,
                    end);
            }

            return Success(snapshot, payload);
        }

        public CapabilityInvocationResult QueryReferences(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var snapshot = Capture();
            var anchor = ValidateAnchor(snapshot, request);
            if (anchor != null)
            {
                return anchor;
            }

            var limitError = ReadLimit(request, MaximumPageSize, DefaultPageSize, out var limit);
            if (limitError != null)
            {
                return limitError;
            }

            var filterError = ReadReferenceFilter(request, out var filter);
            if (filterError != null)
            {
                return filterError;
            }

            var fingerprint = HashFingerprint(
                "reference\n" + filter.Fingerprint() +
                "\nlimit=" + limit.ToString(CultureInfo.InvariantCulture));
            var cursorError = ReadCursor(
                snapshot,
                request,
                WorldInspectionContract.ReferenceQueryName,
                fingerprint,
                out var offset);
            if (cursorError != null)
            {
                return cursorError;
            }

            var matched = new List<ReferenceRow>();
            for (var objectIndex = 0; objectIndex < snapshot.State.Objects.Count; objectIndex++)
            {
                var source = snapshot.State.Objects[objectIndex];
                for (var referenceIndex = 0; referenceIndex < source.References.Count; referenceIndex++)
                {
                    var reference = source.References[referenceIndex];
                    if (filter.Matches(source.Id.Value, reference))
                    {
                        matched.Add(new ReferenceRow(source.Id.Value, reference.Kind.Value, reference.TargetId.Value));
                    }
                }
            }

            matched.Sort(ReferenceRow.Compare);
            if (offset > matched.Count)
            {
                return CursorError("Cursor offset is outside the deterministic result set.");
            }

            var end = Math.Min(matched.Count, offset + limit);
            var items = new List<object?>();
            for (var index = offset; index < end; index++)
            {
                items.Add(matched[index].ToData());
            }

            var payload = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["items"] = items.AsReadOnly()
            };
            if (end < matched.Count)
            {
                payload["nextCursor"] = CreateCursor(
                    WorldInspectionContract.ReferenceQueryName,
                    snapshot,
                    fingerprint,
                    end);
            }

            return Success(snapshot, payload);
        }

        public CapabilityInvocationResult QueryExtensions(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var snapshot = Capture();
            var anchor = ValidateAnchor(snapshot, request);
            if (anchor != null)
            {
                return anchor;
            }

            var limitError = ReadLimit(request, MaximumPageSize, DefaultPageSize, out var limit);
            if (limitError != null)
            {
                return limitError;
            }

            var filterError = ReadExtensionFilter(request, out var filter);
            if (filterError != null)
            {
                return filterError;
            }

            var fingerprint = HashFingerprint(
                "extension\n" + filter.Fingerprint() +
                "\nlimit=" + limit.ToString(CultureInfo.InvariantCulture));
            var cursorError = ReadCursor(
                snapshot,
                request,
                WorldInspectionContract.ExtensionQueryName,
                fingerprint,
                out var offset);
            if (cursorError != null)
            {
                return cursorError;
            }

            var matched = new List<WorldExtensionData>();
            for (var index = 0; index < snapshot.State.Extensions.Count; index++)
            {
                var current = snapshot.State.Extensions[index];
                if (filter.Matches(current))
                {
                    matched.Add(current);
                }
            }

            matched.Sort(CompareExtensions);
            if (offset > matched.Count)
            {
                return CursorError("Cursor offset is outside the deterministic result set.");
            }

            var end = Math.Min(matched.Count, offset + limit);
            var items = new List<object?>();
            for (var index = offset; index < end; index++)
            {
                var current = matched[index];
                var descriptor = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["owner"] = current.Owner,
                    ["schemaVersion"] = current.SchemaVersion,
                    ["scope"] = current.SubjectId.HasValue ? "object" : "global",
                    ["payloadLength"] = current.PayloadLength,
                    ["dependencyCount"] = current.Dependencies.Count
                };
                if (current.SubjectId.HasValue)
                {
                    descriptor["subjectId"] = current.SubjectId.Value.Value;
                }

                items.Add(ReadOnly(descriptor));
            }

            var payload = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["items"] = items.AsReadOnly()
            };
            if (end < matched.Count)
            {
                payload["nextCursor"] = CreateCursor(
                    WorldInspectionContract.ExtensionQueryName,
                    snapshot,
                    fingerprint,
                    end);
            }

            return Success(snapshot, payload);
        }

        public CapabilityInvocationResult ReadExtension(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var snapshot = Capture();
            var anchor = ValidateAnchor(snapshot, request);
            if (anchor != null)
            {
                return anchor;
            }

            if (!TryGetRequiredString(request, "owner", out var owner) || !IsStableToken(owner))
            {
                return SelectorError("$.owner", "Extension owner must use the declared stable identifier grammar.");
            }

            if (!TryGetInteger(request, "schemaVersion", out var schemaVersion) ||
                schemaVersion <= 0 || schemaVersion > int.MaxValue)
            {
                return SelectorError("$.schemaVersion", "Extension schemaVersion must be a positive 32-bit integer.");
            }

            WorldObjectId? subjectId = null;
            if (request.ContainsKey("subjectId"))
            {
                if (!TryGetRequiredString(request, "subjectId", out var subjectText) || !IsStableToken(subjectText))
                {
                    return SelectorError("$.subjectId", "Extension subjectId must use the stable identifier grammar.");
                }

                subjectId = new WorldObjectId(subjectText);
            }

            var offset = 0;
            if (request.ContainsKey("offset"))
            {
                if (!TryGetInteger(request, "offset", out var requestedOffset) ||
                    requestedOffset < 0 || requestedOffset > int.MaxValue)
                {
                    return SelectorError("$.offset", "Extension offset must be a non-negative 32-bit integer.");
                }

                offset = (int)requestedOffset;
            }

            var limitError = ReadLimit(
                request,
                MaximumExtensionChunkBytes,
                MaximumExtensionChunkBytes,
                out var limit);
            if (limitError != null)
            {
                return limitError;
            }

            var dependencyOffset = 0;
            if (request.ContainsKey("dependencyOffset"))
            {
                if (!TryGetInteger(request, "dependencyOffset", out var requestedDependencyOffset) ||
                    requestedDependencyOffset < 0 || requestedDependencyOffset > int.MaxValue)
                {
                    return SelectorError("$.dependencyOffset", "dependencyOffset must be a non-negative 32-bit integer.");
                }

                dependencyOffset = (int)requestedDependencyOffset;
            }

            var dependencyLimit = MaximumExtensionDependencyPageSize;
            if (request.ContainsKey("dependencyLimit"))
            {
                if (!TryGetInteger(request, "dependencyLimit", out var requestedDependencyLimit) ||
                    requestedDependencyLimit <= 0 || requestedDependencyLimit > MaximumExtensionDependencyPageSize)
                {
                    return SelectorError(
                        "$.dependencyLimit",
                        "dependencyLimit must be between 1 and " +
                        MaximumExtensionDependencyPageSize.ToString(CultureInfo.InvariantCulture) + ".");
                }

                dependencyLimit = (int)requestedDependencyLimit;
            }

            WorldExtensionData? extension = null;
            for (var index = 0; index < snapshot.State.Extensions.Count; index++)
            {
                var current = snapshot.State.Extensions[index];
                if (string.Equals(current.Owner, owner, StringComparison.Ordinal) &&
                    current.SchemaVersion == (int)schemaVersion &&
                    current.SubjectId == subjectId)
                {
                    extension = current;
                    break;
                }
            }

            if (extension == null)
            {
                return Failure(
                    "world.extension_not_found",
                    "The requested canonical extension does not exist in this world revision.",
                    "$.owner",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["owner"] = owner,
                        ["schemaVersion"] = schemaVersion,
                        ["scope"] = subjectId.HasValue ? "object" : "global",
                        ["subjectId"] = subjectId.HasValue ? (object?)subjectId.Value.Value : null
                    },
                    false,
                    "Refresh world extension descriptors and select an owner/schemaVersion present in this revision.");
            }

            if (offset > extension.PayloadLength)
            {
                return SelectorError("$.offset", "Extension offset is beyond the payload length.");
            }

            if (dependencyOffset > extension.Dependencies.Count)
            {
                return SelectorError("$.dependencyOffset", "Extension dependencyOffset is beyond the dependency count.");
            }

            var bytes = extension.GetPayloadCopy();
            var count = Math.Min(limit, bytes.Length - offset);
            var chunk = new byte[count];
            Array.Copy(bytes, offset, chunk, 0, count);

            var sortedDependencies = new List<WorldReference>(extension.Dependencies);
            sortedDependencies.Sort((left, right) =>
            {
                var comparison = left.Kind.CompareTo(right.Kind);
                return comparison != 0 ? comparison : left.TargetId.CompareTo(right.TargetId);
            });
            var dependencyEnd = Math.Min(sortedDependencies.Count, dependencyOffset + dependencyLimit);
            var dependencies = new List<object?>();
            for (var index = dependencyOffset; index < dependencyEnd; index++)
            {
                dependencies.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = sortedDependencies[index].Kind.Value,
                    ["targetId"] = sortedDependencies[index].TargetId.Value
                }));
            }

            var payload = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["owner"] = extension.Owner,
                ["schemaVersion"] = extension.SchemaVersion,
                ["scope"] = extension.SubjectId.HasValue ? "object" : "global",
                ["payloadLength"] = extension.PayloadLength,
                ["offset"] = offset,
                ["payloadBase64"] = Convert.ToBase64String(chunk),
                ["dependencyCount"] = extension.Dependencies.Count,
                ["dependencyOffset"] = dependencyOffset,
                ["dependencies"] = dependencies.AsReadOnly()
            };
            if (extension.SubjectId.HasValue)
            {
                payload["subjectId"] = extension.SubjectId.Value.Value;
            }
            if (offset + count < bytes.Length)
            {
                payload["nextOffset"] = offset + count;
            }

            if (dependencyEnd < sortedDependencies.Count)
            {
                payload["nextDependencyOffset"] = dependencyEnd;
            }

            return Success(snapshot, payload);
        }

        private WorldSnapshot Capture()
        {
            var state = _source.Current ?? throw new InvalidOperationException("World state sources may not return null.");
            return new WorldSnapshot(state, CanonicalWorldStateCodec.ComputeContentHash(state));
        }

        private static CapabilityInvocationResult? ValidateAnchor(
            WorldSnapshot snapshot,
            IReadOnlyDictionary<string, object?> request)
        {
            if (!TryGetInteger(request, "revision", out var revision) || revision < 0)
            {
                return SelectorError("$.revision", "revision must be a non-negative integer.");
            }

            if (!TryGetRequiredString(request, "hash", out var hash) || !IsCanonicalHash(hash))
            {
                return SelectorError("$.hash", "hash must be a lowercase 64-character SHA-256 hex string.");
            }

            if (revision != snapshot.State.Revision)
            {
                return Failure(
                    "world.stale_revision",
                    "The requested world revision is stale.",
                    "$.revision",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["expected"] = revision,
                        ["actual"] = snapshot.State.Revision,
                        ["actualHash"] = snapshot.Hash
                    },
                    true,
                    "Refresh world.summary and retry against the returned revision/hash.");
            }

            if (!string.Equals(hash, snapshot.Hash, StringComparison.Ordinal))
            {
                return Failure(
                    "world.stale_hash",
                    "The requested world content hash does not match this revision.",
                    "$.hash",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["expected"] = hash,
                        ["actual"] = snapshot.Hash,
                        ["revision"] = snapshot.State.Revision
                    },
                    true,
                    "Refresh world.summary and retry against the returned revision/hash.");
            }

            return null;
        }

        private static CapabilityInvocationResult? ReadLimit(
            IReadOnlyDictionary<string, object?> request,
            int maximum,
            int defaultValue,
            out int limit)
        {
            limit = defaultValue;
            if (!request.ContainsKey("limit"))
            {
                return null;
            }

            if (!TryGetInteger(request, "limit", out var value) || value <= 0 || value > maximum)
            {
                return SelectorError(
                    "$.limit",
                    "limit must be between 1 and " + maximum.ToString(CultureInfo.InvariantCulture) + ".");
            }

            limit = (int)value;
            return null;
        }

        private static CapabilityInvocationResult? ReadObjectFields(
            IReadOnlyDictionary<string, object?> request,
            out ObjectFields fields)
        {
            fields = ObjectFields.All;
            if (!request.TryGetValue("fields", out var raw))
            {
                return null;
            }

            if (!(raw is IReadOnlyList<object?> values))
            {
                return SelectorError("$.fields", "fields must be an array from the declared projection enum.");
            }

            fields = ObjectFields.None;
            for (var index = 0; index < values.Count; index++)
            {
                if (!(values[index] is string value))
                {
                    return SelectorError("$.fields", "fields must contain only declared projection names.");
                }

                switch (value)
                {
                    case "typeId":
                        fields |= ObjectFields.Type;
                        break;
                    case "containerId":
                        fields |= ObjectFields.Container;
                        break;
                    default:
                        return SelectorError("$.fields", "Unknown object projection field.");
                }
            }

            return null;
        }

        private static CapabilityInvocationResult? ReadObjectFilter(
            IReadOnlyDictionary<string, object?> request,
            out ObjectFilter filter)
        {
            filter = new ObjectFilter(null, null, null, null, null);
            if (!request.TryGetValue("filter", out var raw))
            {
                return null;
            }

            if (!(raw is IReadOnlyDictionary<string, object?> data))
            {
                return SelectorError("$.filter", "filter must follow the declared typed object-filter grammar.");
            }

            var error = ReadTokenSet(data, "ids", "$.filter.ids", out var ids);
            if (error != null) return error;
            error = ReadTokenSet(data, "typeIds", "$.filter.typeIds", out var types);
            if (error != null) return error;
            error = ReadTokenSet(data, "containerIds", "$.filter.containerIds", out var containers);
            if (error != null) return error;
            error = ReadTokenSet(data, "referenceKinds", "$.filter.referenceKinds", out var kinds);
            if (error != null) return error;
            error = ReadTokenSet(data, "targetIds", "$.filter.targetIds", out var targets);
            if (error != null) return error;

            filter = new ObjectFilter(ids, types, containers, kinds, targets);
            return null;
        }

        private static CapabilityInvocationResult? ReadReferenceFilter(
            IReadOnlyDictionary<string, object?> request,
            out ReferenceFilter filter)
        {
            filter = new ReferenceFilter(null, null, null);
            if (!request.TryGetValue("filter", out var raw))
            {
                return null;
            }

            if (!(raw is IReadOnlyDictionary<string, object?> data))
            {
                return SelectorError("$.filter", "filter must follow the declared typed reference-filter grammar.");
            }

            var error = ReadTokenSet(data, "sourceIds", "$.filter.sourceIds", out var sources);
            if (error != null) return error;
            error = ReadTokenSet(data, "kinds", "$.filter.kinds", out var kinds);
            if (error != null) return error;
            error = ReadTokenSet(data, "targetIds", "$.filter.targetIds", out var targets);
            if (error != null) return error;

            filter = new ReferenceFilter(sources, kinds, targets);
            return null;
        }

        private static CapabilityInvocationResult? ReadExtensionFilter(
            IReadOnlyDictionary<string, object?> request,
            out ExtensionFilter filter)
        {
            filter = new ExtensionFilter(null, null, null, null);
            if (!request.TryGetValue("filter", out var raw))
            {
                return null;
            }

            if (!(raw is IReadOnlyDictionary<string, object?> data))
            {
                return SelectorError("$.filter", "filter must follow the declared typed extension-filter grammar.");
            }

            var error = ReadTokenSet(data, "owners", "$.filter.owners", out var owners);
            if (error != null)
            {
                return error;
            }

            HashSet<int>? versions = null;
            if (data.TryGetValue("schemaVersions", out var rawVersions))
            {
                if (!(rawVersions is IReadOnlyList<object?> values))
                {
                    return SelectorError("$.filter.schemaVersions", "schemaVersions must be an integer array.");
                }

                versions = new HashSet<int>();
                for (var index = 0; index < values.Count; index++)
                {
                    if (!TryConvertInteger(values[index], out var value) || value <= 0 || value > int.MaxValue)
                    {
                        return SelectorError("$.filter.schemaVersions", "schemaVersions must contain positive 32-bit integers.");
                    }

                    versions.Add((int)value);
                }
            }

            error = ReadTokenSet(data, "scopes", "$.filter.scopes", out var scopes);
            if (error != null)
            {
                return error;
            }

            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    if (!StringComparer.Ordinal.Equals(scope, "global") &&
                        !StringComparer.Ordinal.Equals(scope, "object"))
                    {
                        return SelectorError("$.filter.scopes", "scopes contains a value outside global/object.");
                    }
                }
            }

            error = ReadTokenSet(data, "subjectIds", "$.filter.subjectIds", out var subjectIds);
            if (error != null)
            {
                return error;
            }

            filter = new ExtensionFilter(owners, versions, scopes, subjectIds);
            return null;
        }

        private static CapabilityInvocationResult? ReadTokenSet(
            IReadOnlyDictionary<string, object?> data,
            string key,
            string path,
            out HashSet<string>? values)
        {
            values = null;
            if (!data.TryGetValue(key, out var raw))
            {
                return null;
            }

            if (!(raw is IReadOnlyList<object?> list))
            {
                return SelectorError(path, key + " must be an array of stable identifiers.");
            }

            values = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < list.Count; index++)
            {
                if (!(list[index] is string value) || !IsStableToken(value))
                {
                    return SelectorError(path, key + " contains a value outside the stable identifier grammar.");
                }

                values.Add(value);
            }

            return null;
        }

        private static CapabilityInvocationResult? ReadCursor(
            WorldSnapshot snapshot,
            IReadOnlyDictionary<string, object?> request,
            string command,
            string fingerprint,
            out int offset)
        {
            offset = 0;
            if (!request.TryGetValue("cursor", out var raw))
            {
                return null;
            }

            if (!(raw is string cursor) || !TryParseCursor(cursor, out var parsed))
            {
                return CursorError("Cursor does not match the declared canonical cursor grammar.");
            }

            if (!string.Equals(parsed.Command, command, StringComparison.Ordinal) ||
                !string.Equals(parsed.Fingerprint, fingerprint, StringComparison.Ordinal))
            {
                return CursorError("Cursor belongs to a different command, selector, projection or page-size context.");
            }

            if (parsed.Revision != snapshot.State.Revision ||
                !string.Equals(parsed.Hash, snapshot.Hash, StringComparison.Ordinal))
            {
                return Failure(
                    "world.stale_cursor",
                    "Cursor is bound to a different world revision/hash.",
                    "$.cursor",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["cursorRevision"] = parsed.Revision,
                        ["actualRevision"] = snapshot.State.Revision,
                        ["cursorHash"] = parsed.Hash,
                        ["actualHash"] = snapshot.Hash
                    },
                    true,
                    "Restart the query from world.summary using the current revision/hash.");
            }

            offset = parsed.Offset;
            return null;
        }

        private static string CreateCursor(string command, WorldSnapshot snapshot, string fingerprint, int offset)
        {
            var text = string.Join("\n", new[]
            {
                CursorVersion,
                command,
                snapshot.State.Revision.ToString(CultureInfo.InvariantCulture),
                snapshot.Hash,
                fingerprint,
                offset.ToString(CultureInfo.InvariantCulture)
            });
            return Convert.ToBase64String(StrictUtf8.GetBytes(text));
        }

        private static bool TryParseCursor(string cursor, out ParsedCursor parsed)
        {
            parsed = new ParsedCursor(string.Empty, 0, string.Empty, string.Empty, 0);
            if (cursor.Length == 0 || cursor.Length > 2048)
            {
                return false;
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(cursor);
            }
            catch (FormatException)
            {
                return false;
            }

            if (!string.Equals(Convert.ToBase64String(bytes), cursor, StringComparison.Ordinal))
            {
                return false;
            }

            string text;
            try
            {
                text = StrictUtf8.GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                return false;
            }

            var parts = text.Split('\n');
            if (parts.Length != 6 ||
                !string.Equals(parts[0], CursorVersion, StringComparison.Ordinal) ||
                !long.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var revision) || revision < 0 ||
                !IsCanonicalHash(parts[3]) ||
                !IsCanonicalHash(parts[4]) ||
                !int.TryParse(parts[5], NumberStyles.None, CultureInfo.InvariantCulture, out var offset) || offset < 0)
            {
                return false;
            }

            parsed = new ParsedCursor(parts[1], revision, parts[3], parts[4], offset);
            return true;
        }

        private static IReadOnlyDictionary<string, object?> ProjectObject(WorldObject value, ObjectFields fields)
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["id"] = value.Id.Value
            };
            if ((fields & ObjectFields.Type) != 0)
            {
                data["typeId"] = value.TypeId.Value;
            }

            if ((fields & ObjectFields.Container) != 0 && value.ContainerId.HasValue)
            {
                data["containerId"] = value.ContainerId.Value.Value;
            }

            return ReadOnly(data);
        }

        private static CapabilityInvocationResult Success(WorldSnapshot snapshot, IDictionary<string, object?> payload)
        {
            payload["world"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["worldId"] = snapshot.State.Id.Value,
                ["schemaVersion"] = snapshot.State.SchemaVersion,
                ["revision"] = snapshot.State.Revision,
                ["hash"] = snapshot.Hash
            });
            return CapabilityInvocationResult.Succeeded(ReadOnly(payload));
        }

        private static CapabilityInvocationResult SelectorError(string path, string message)
        {
            return Failure(
                "world.invalid_selector",
                message,
                path,
                new Dictionary<string, object?>(StringComparer.Ordinal),
                false,
                "Use only the typed selector fields and bounds declared by system.describe.");
        }

        private static CapabilityInvocationResult CursorError(string message)
        {
            return Failure(
                "world.invalid_cursor",
                message,
                "$.cursor",
                new Dictionary<string, object?>(StringComparer.Ordinal),
                false,
                "Discard the cursor and restart the query using the current revision/hash and selector.");
        }

        private static CapabilityInvocationResult Failure(
            string code,
            string message,
            string path,
            IReadOnlyDictionary<string, object?> context,
            bool retryable,
            string repairHint)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                code,
                message,
                path,
                context,
                retryable,
                repairHint));
        }

        private static string HashFingerprint(string value)
        {
            byte[] digest;
            using (var sha256 = SHA256.Create())
            {
                digest = sha256.ComputeHash(StrictUtf8.GetBytes(value));
            }

            var characters = new char[digest.Length * 2];
            const string alphabet = "0123456789abcdef";
            for (var index = 0; index < digest.Length; index++)
            {
                characters[index * 2] = alphabet[digest[index] >> 4];
                characters[index * 2 + 1] = alphabet[digest[index] & 0x0f];
            }

            return new string(characters);
        }

        private static string FingerprintSet(HashSet<string>? values)
        {
            if (values == null)
            {
                return "*";
            }

            var sorted = new List<string>(values);
            sorted.Sort(StringComparer.Ordinal);
            return string.Join(",", sorted);
        }

        private static string FingerprintInts(HashSet<int>? values)
        {
            if (values == null)
            {
                return "*";
            }

            var sorted = new List<int>(values);
            sorted.Sort();
            var text = new List<string>();
            for (var index = 0; index < sorted.Count; index++)
            {
                text.Add(sorted[index].ToString(CultureInfo.InvariantCulture));
            }

            return string.Join(",", text);
        }

        private static bool TryGetRequiredString(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out string value)
        {
            value = string.Empty;
            if (!data.TryGetValue(key, out var raw) || !(raw is string text))
            {
                return false;
            }

            value = text;
            return true;
        }

        private static bool TryGetInteger(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out long value)
        {
            value = 0;
            return data.TryGetValue(key, out var raw) && TryConvertInteger(raw, out value);
        }

        private static bool TryConvertInteger(object? raw, out long value)
        {
            value = 0;
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

        private static bool IsCanonicalHash(string value)
        {
            if (value.Length != 64)
            {
                return false;
            }

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f')))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsStableToken(string value)
        {
            if (value.Length == 0 || value.Length > 128)
            {
                return false;
            }

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!(character >= 'a' && character <= 'z') &&
                    !(character >= '0' && character <= '9') &&
                    character != '.' && character != '_' && character != '-')
                {
                    return false;
                }
            }

            return true;
        }

        private static int CompareExtensions(WorldExtensionData left, WorldExtensionData right)
        {
            return left.Identity.CompareTo(right.Identity);
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> data)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(data, StringComparer.Ordinal));
        }

        [Flags]
        private enum ObjectFields
        {
            None = 0,
            Type = 1,
            Container = 2,
            All = Type | Container
        }

        private sealed class WorldSnapshot
        {
            public WorldSnapshot(WorldState state, string hash)
            {
                State = state;
                Hash = hash;
            }

            public WorldState State { get; }
            public string Hash { get; }
        }

        private sealed class ObjectFilter
        {
            public ObjectFilter(
                HashSet<string>? ids,
                HashSet<string>? types,
                HashSet<string>? containers,
                HashSet<string>? referenceKinds,
                HashSet<string>? targets)
            {
                Ids = ids;
                Types = types;
                Containers = containers;
                ReferenceKinds = referenceKinds;
                Targets = targets;
            }

            private HashSet<string>? Ids { get; }
            private HashSet<string>? Types { get; }
            private HashSet<string>? Containers { get; }
            private HashSet<string>? ReferenceKinds { get; }
            private HashSet<string>? Targets { get; }

            public bool Matches(WorldObject value)
            {
                if (Ids != null && !Ids.Contains(value.Id.Value)) return false;
                if (Types != null && !Types.Contains(value.TypeId.Value)) return false;
                if (Containers != null && (!value.ContainerId.HasValue || !Containers.Contains(value.ContainerId.Value.Value))) return false;
                if (ReferenceKinds == null && Targets == null) return true;

                for (var index = 0; index < value.References.Count; index++)
                {
                    var reference = value.References[index];
                    if ((ReferenceKinds == null || ReferenceKinds.Contains(reference.Kind.Value)) &&
                        (Targets == null || Targets.Contains(reference.TargetId.Value)))
                    {
                        return true;
                    }
                }

                return false;
            }

            public string Fingerprint()
            {
                return "ids=" + FingerprintSet(Ids) +
                    "\ntypes=" + FingerprintSet(Types) +
                    "\ncontainers=" + FingerprintSet(Containers) +
                    "\nkinds=" + FingerprintSet(ReferenceKinds) +
                    "\ntargets=" + FingerprintSet(Targets);
            }
        }

        private sealed class ReferenceFilter
        {
            public ReferenceFilter(HashSet<string>? sources, HashSet<string>? kinds, HashSet<string>? targets)
            {
                Sources = sources;
                Kinds = kinds;
                Targets = targets;
            }

            private HashSet<string>? Sources { get; }
            private HashSet<string>? Kinds { get; }
            private HashSet<string>? Targets { get; }

            public bool Matches(string sourceId, WorldReference reference)
            {
                return (Sources == null || Sources.Contains(sourceId)) &&
                    (Kinds == null || Kinds.Contains(reference.Kind.Value)) &&
                    (Targets == null || Targets.Contains(reference.TargetId.Value));
            }

            public string Fingerprint()
            {
                return "sources=" + FingerprintSet(Sources) +
                    "\nkinds=" + FingerprintSet(Kinds) +
                    "\ntargets=" + FingerprintSet(Targets);
            }
        }

        private sealed class ExtensionFilter
        {
            public ExtensionFilter(
                HashSet<string>? owners,
                HashSet<int>? versions,
                HashSet<string>? scopes,
                HashSet<string>? subjectIds)
            {
                Owners = owners;
                Versions = versions;
                Scopes = scopes;
                SubjectIds = subjectIds;
            }

            private HashSet<string>? Owners { get; }
            private HashSet<int>? Versions { get; }
            private HashSet<string>? Scopes { get; }
            private HashSet<string>? SubjectIds { get; }

            public bool Matches(WorldExtensionData value)
            {
                var scope = value.SubjectId.HasValue ? "object" : "global";
                return (Owners == null || Owners.Contains(value.Owner)) &&
                    (Versions == null || Versions.Contains(value.SchemaVersion)) &&
                    (Scopes == null || Scopes.Contains(scope)) &&
                    (SubjectIds == null ||
                        (value.SubjectId.HasValue && SubjectIds.Contains(value.SubjectId.Value.Value)));
            }

            public string Fingerprint()
            {
                return "owners=" + FingerprintSet(Owners) +
                    "\nversions=" + FingerprintInts(Versions) +
                    "\nscopes=" + FingerprintSet(Scopes) +
                    "\nsubjects=" + FingerprintSet(SubjectIds);
            }
        }

        private sealed class ParsedCursor
        {
            public ParsedCursor(string command, long revision, string hash, string fingerprint, int offset)
            {
                Command = command;
                Revision = revision;
                Hash = hash;
                Fingerprint = fingerprint;
                Offset = offset;
            }

            public string Command { get; }
            public long Revision { get; }
            public string Hash { get; }
            public string Fingerprint { get; }
            public int Offset { get; }
        }

        private sealed class ReferenceRow
        {
            public ReferenceRow(string sourceId, string kind, string targetId)
            {
                SourceId = sourceId;
                Kind = kind;
                TargetId = targetId;
            }

            private string SourceId { get; }
            private string Kind { get; }
            private string TargetId { get; }

            public IReadOnlyDictionary<string, object?> ToData()
            {
                return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["sourceId"] = SourceId,
                    ["kind"] = Kind,
                    ["targetId"] = TargetId
                });
            }

            public static int Compare(ReferenceRow left, ReferenceRow right)
            {
                var source = StringComparer.Ordinal.Compare(left.SourceId, right.SourceId);
                if (source != 0) return source;
                var kind = StringComparer.Ordinal.Compare(left.Kind, right.Kind);
                return kind != 0 ? kind : StringComparer.Ordinal.Compare(left.TargetId, right.TargetId);
            }
        }
    }
}
