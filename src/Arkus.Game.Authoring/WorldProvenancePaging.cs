using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// HK08A public interaction view over the complete HK06A journal artifact. The wrapped service
    /// remains the provenance authority; this class only validates an authored anchor and returns a
    /// deterministic bounded slice in persisted sequence order.
    /// </summary>
    public sealed class BoundedWorldProvenanceService : IWorldProvenanceService
    {
        private const string CursorVersion = "arkus-journal-cursor-v1";
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        private static readonly IReadOnlyDictionary<string, object?> EmptyRequest =
            ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal));

        private readonly IWorldProvenanceService _inner;

        public BoundedWorldProvenanceService(IWorldProvenanceService inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var requestError = ParseRequest(request, out var parsedRequest);
            if (requestError != null) return requestError;

            var complete = _inner.ReadJournal(EmptyRequest);
            if (!complete.Success) return complete;
            if (complete.Data == null)
            {
                throw new InvalidOperationException("Successful provenance read returned no data.");
            }

            var journalError = ParseJournal(complete.Data, out var journal);
            if (journalError != null) return journalError;

            if (parsedRequest.HasAnchor &&
                (parsedRequest.Revision != journal.Current.Revision ||
                 !string.Equals(parsedRequest.Hash, journal.Current.Hash, StringComparison.Ordinal)))
            {
                return StaleAnchor(parsedRequest, journal.Current);
            }

            var offset = 0;
            if (parsedRequest.Cursor != null)
            {
                if (!TryParseCursor(parsedRequest.Cursor, out var cursor))
                {
                    return InvalidCursor("Cursor does not match the declared canonical journal cursor grammar.");
                }

                if (cursor.Limit != parsedRequest.Limit)
                {
                    return InvalidCursor("Cursor belongs to a different journal page-size context.");
                }

                if (!CursorMatchesJournal(cursor, journal))
                {
                    return StaleCursor(cursor, journal);
                }

                if (cursor.Offset > journal.Entries.Count)
                {
                    return InvalidCursor("Cursor offset is outside the persisted journal sequence.");
                }

                offset = cursor.Offset;
            }

            var end = Math.Min(journal.Entries.Count, offset + parsedRequest.Limit);
            var entries = new List<object?>();
            for (var index = offset; index < end; index++)
            {
                entries.Add(journal.Entries[index]);
            }

            var payload = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = WorldProvenanceContract.JournalPageSchemaId,
                ["journalSchemaId"] = WorldProvenanceContract.JournalSchemaId,
                ["entrySchemaId"] = WorldProvenanceContract.EntrySchemaId,
                ["base"] = journal.Base.Data,
                ["current"] = journal.Current.Data,
                ["entryCount"] = journal.Entries.Count,
                ["pageOffset"] = offset,
                ["entries"] = entries.AsReadOnly()
            };
            if (end < journal.Entries.Count)
            {
                payload["nextCursor"] = CreateCursor(journal, parsedRequest.Limit, end);
            }

            return CapabilityInvocationResult.Succeeded(ReadOnly(payload));
        }

        private static CapabilityInvocationResult? ParseRequest(
            IReadOnlyDictionary<string, object?> request,
            out ParsedRequest parsed)
        {
            parsed = new ParsedRequest(false, 0, string.Empty, WorldProvenanceContract.DefaultPageSize, null);

            foreach (var key in request.Keys)
            {
                if (!string.Equals(key, "revision", StringComparison.Ordinal) &&
                    !string.Equals(key, "hash", StringComparison.Ordinal) &&
                    !string.Equals(key, "limit", StringComparison.Ordinal) &&
                    !string.Equals(key, "cursor", StringComparison.Ordinal))
                {
                    return InvalidRequest("$.", "Journal read contains an undeclared request field: " + key + ".");
                }
            }

            var hasRevision = request.ContainsKey("revision");
            var hasHash = request.ContainsKey("hash");
            if (hasRevision != hasHash)
            {
                return InvalidRequest("$", "revision and hash must be supplied together when anchoring a journal read.");
            }

            long revision = 0;
            var hash = string.Empty;
            if (hasRevision)
            {
                if (!TryGetInteger(request, "revision", out revision) || revision < 0)
                {
                    return InvalidRequest("$.revision", "revision must be a non-negative integer.");
                }

                if (!TryGetString(request, "hash", out hash) || !IsCanonicalHash(hash))
                {
                    return InvalidRequest("$.hash", "hash must be a lowercase 64-character SHA-256 hex string.");
                }
            }

            var limit = WorldProvenanceContract.DefaultPageSize;
            if (request.ContainsKey("limit"))
            {
                if (!TryGetInteger(request, "limit", out var rawLimit) ||
                    rawLimit <= 0 || rawLimit > WorldProvenanceContract.MaximumPageSize)
                {
                    return InvalidRequest(
                        "$.limit",
                        "limit must be between 1 and " +
                        WorldProvenanceContract.MaximumPageSize.ToString(CultureInfo.InvariantCulture) + ".");
                }

                limit = (int)rawLimit;
            }

            string? cursor = null;
            if (request.ContainsKey("cursor"))
            {
                if (!TryGetString(request, "cursor", out cursor) || cursor.Length == 0 || cursor.Length > 4096)
                {
                    return InvalidRequest("$.cursor", "cursor must be a non-empty bounded string.");
                }
            }

            parsed = new ParsedRequest(hasRevision, revision, hash, limit, cursor);
            return null;
        }

        private static CapabilityInvocationResult? ParseJournal(
            IReadOnlyDictionary<string, object?> data,
            out ParsedJournal journal)
        {
            journal = ParsedJournal.Empty;
            if (!TryGetString(data, "schemaId", out var schemaId) ||
                !string.Equals(schemaId, WorldProvenanceContract.JournalSchemaId, StringComparison.Ordinal) ||
                !TryGetString(data, "entrySchemaId", out var entrySchemaId) ||
                !string.Equals(entrySchemaId, WorldProvenanceContract.EntrySchemaId, StringComparison.Ordinal) ||
                !TryGetMap(data, "base", out var baseData) ||
                !TryGetMap(data, "current", out var currentData) ||
                !TryParseAnchor(baseData, out var baseAnchor) ||
                !TryParseAnchor(currentData, out var currentAnchor) ||
                !TryGetInteger(data, "entryCount", out var entryCount) ||
                entryCount < 0 || entryCount > int.MaxValue ||
                !data.TryGetValue("entries", out var rawEntries) ||
                !(rawEntries is IReadOnlyList<object?> entries) ||
                entries.Count != (int)entryCount)
            {
                return CapabilityInvocationResult.Failed(new StructuredError(
                    "world.provenance.invalid_internal_journal",
                    "The accepted provenance authority returned an internally inconsistent complete journal artifact.",
                    "$",
                    EmptyContext(),
                    false,
                    "Treat this as an internal provenance invariant failure; do not return a partial page."));
            }

            journal = new ParsedJournal(baseAnchor, currentAnchor, entries);
            return null;
        }

        private static bool TryParseAnchor(
            IReadOnlyDictionary<string, object?> data,
            out ParsedAnchor anchor)
        {
            anchor = ParsedAnchor.Empty;
            if (!TryGetString(data, "worldId", out var worldId) || worldId.Length == 0 || worldId.IndexOf('\n') >= 0 ||
                !TryGetInteger(data, "stateSchemaVersion", out var schemaVersion) || schemaVersion <= 0 || schemaVersion > int.MaxValue ||
                !TryGetInteger(data, "revision", out var revision) || revision < 0 ||
                !TryGetString(data, "hash", out var hash) || !IsCanonicalHash(hash))
            {
                return false;
            }

            anchor = new ParsedAnchor(worldId, (int)schemaVersion, revision, hash, data);
            return true;
        }

        private static string CreateCursor(ParsedJournal journal, int limit, int offset)
        {
            var text = string.Join("\n", new[]
            {
                CursorVersion,
                journal.Current.WorldId,
                journal.Current.StateSchemaVersion.ToString(CultureInfo.InvariantCulture),
                journal.Current.Revision.ToString(CultureInfo.InvariantCulture),
                journal.Current.Hash,
                journal.Base.Revision.ToString(CultureInfo.InvariantCulture),
                journal.Base.Hash,
                journal.Entries.Count.ToString(CultureInfo.InvariantCulture),
                limit.ToString(CultureInfo.InvariantCulture),
                offset.ToString(CultureInfo.InvariantCulture)
            });
            return Convert.ToBase64String(StrictUtf8.GetBytes(text));
        }

        private static bool TryParseCursor(string value, out ParsedCursor cursor)
        {
            cursor = ParsedCursor.Empty;
            if (value.Length == 0 || value.Length > 4096) return false;

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(value);
            }
            catch (FormatException)
            {
                return false;
            }

            if (!string.Equals(Convert.ToBase64String(bytes), value, StringComparison.Ordinal)) return false;

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
            if (parts.Length != 10 ||
                !string.Equals(parts[0], CursorVersion, StringComparison.Ordinal) ||
                parts[1].Length == 0 || parts[1].IndexOf('\n') >= 0 ||
                !int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var stateSchemaVersion) || stateSchemaVersion <= 0 ||
                !long.TryParse(parts[3], NumberStyles.None, CultureInfo.InvariantCulture, out var revision) || revision < 0 ||
                !IsCanonicalHash(parts[4]) ||
                !long.TryParse(parts[5], NumberStyles.None, CultureInfo.InvariantCulture, out var baseRevision) || baseRevision < 0 ||
                !IsCanonicalHash(parts[6]) ||
                !int.TryParse(parts[7], NumberStyles.None, CultureInfo.InvariantCulture, out var entryCount) || entryCount < 0 ||
                !int.TryParse(parts[8], NumberStyles.None, CultureInfo.InvariantCulture, out var limit) ||
                limit <= 0 || limit > WorldProvenanceContract.MaximumPageSize ||
                !int.TryParse(parts[9], NumberStyles.None, CultureInfo.InvariantCulture, out var offset) || offset < 0)
            {
                return false;
            }

            cursor = new ParsedCursor(
                parts[1],
                stateSchemaVersion,
                revision,
                parts[4],
                baseRevision,
                parts[6],
                entryCount,
                limit,
                offset);
            return true;
        }

        private static bool CursorMatchesJournal(ParsedCursor cursor, ParsedJournal journal)
        {
            return string.Equals(cursor.WorldId, journal.Current.WorldId, StringComparison.Ordinal) &&
                cursor.StateSchemaVersion == journal.Current.StateSchemaVersion &&
                cursor.Revision == journal.Current.Revision &&
                string.Equals(cursor.Hash, journal.Current.Hash, StringComparison.Ordinal) &&
                cursor.BaseRevision == journal.Base.Revision &&
                string.Equals(cursor.BaseHash, journal.Base.Hash, StringComparison.Ordinal) &&
                cursor.EntryCount == journal.Entries.Count;
        }

        private static CapabilityInvocationResult StaleAnchor(ParsedRequest request, ParsedAnchor current)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.provenance.stale_anchor",
                "The requested journal anchor does not match the current authored revision/hash.",
                "$.revision",
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["expectedRevision"] = request.Revision,
                    ["expectedHash"] = request.Hash,
                    ["actualRevision"] = current.Revision,
                    ["actualHash"] = current.Hash
                }),
                true,
                "Restart the bounded journal read against the current revision/hash."));
        }

        private static CapabilityInvocationResult StaleCursor(ParsedCursor cursor, ParsedJournal journal)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.provenance.stale_cursor",
                "Journal cursor is bound to a different authored journal anchor.",
                "$.cursor",
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["cursorRevision"] = cursor.Revision,
                    ["cursorHash"] = cursor.Hash,
                    ["actualRevision"] = journal.Current.Revision,
                    ["actualHash"] = journal.Current.Hash
                }),
                true,
                "Discard the cursor and restart the bounded journal read against the current authored anchor."));
        }

        private static CapabilityInvocationResult InvalidCursor(string message)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.provenance.invalid_cursor",
                message,
                "$.cursor",
                EmptyContext(),
                false,
                "Discard the cursor and restart the journal read using the current page shape."));
        }

        private static CapabilityInvocationResult InvalidRequest(string path, string message)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.provenance.invalid_request",
                message,
                path,
                EmptyContext(),
                false,
                "Use only the bounded journal-read fields declared by system.describe."));
        }

        private static bool TryGetMap(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out IReadOnlyDictionary<string, object?> value)
        {
            value = EmptyContext();
            if (!data.TryGetValue(key, out var raw) || !(raw is IReadOnlyDictionary<string, object?> map)) return false;
            value = map;
            return true;
        }

        private static bool TryGetString(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out string value)
        {
            value = string.Empty;
            if (!data.TryGetValue(key, out var raw) || !(raw is string text)) return false;
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
            switch (raw)
            {
                case byte typed: value = typed; return true;
                case sbyte typed: value = typed; return true;
                case short typed: value = typed; return true;
                case ushort typed: value = typed; return true;
                case int typed: value = typed; return true;
                case uint typed: value = typed; return true;
                case long typed: value = typed; return true;
                default: value = 0; return false;
            }
        }

        private static bool IsCanonicalHash(string? value)
        {
            if (value == null || value.Length != 64) return false;
            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f'))) return false;
            }

            return true;
        }

        private static IReadOnlyDictionary<string, object?> EmptyContext()
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal));
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> data)
        {
            return new ReadOnlyDictionary<string, object?>(data);
        }

        private sealed class ParsedRequest
        {
            public ParsedRequest(bool hasAnchor, long revision, string hash, int limit, string? cursor)
            {
                HasAnchor = hasAnchor;
                Revision = revision;
                Hash = hash;
                Limit = limit;
                Cursor = cursor;
            }

            public bool HasAnchor { get; }
            public long Revision { get; }
            public string Hash { get; }
            public int Limit { get; }
            public string? Cursor { get; }
        }

        private sealed class ParsedAnchor
        {
            public static readonly ParsedAnchor Empty = new ParsedAnchor(
                string.Empty,
                1,
                0,
                new string('0', 64),
                EmptyContext());

            public ParsedAnchor(
                string worldId,
                int stateSchemaVersion,
                long revision,
                string hash,
                IReadOnlyDictionary<string, object?> data)
            {
                WorldId = worldId;
                StateSchemaVersion = stateSchemaVersion;
                Revision = revision;
                Hash = hash;
                Data = data;
            }

            public string WorldId { get; }
            public int StateSchemaVersion { get; }
            public long Revision { get; }
            public string Hash { get; }
            public IReadOnlyDictionary<string, object?> Data { get; }
        }

        private sealed class ParsedJournal
        {
            public static readonly ParsedJournal Empty = new ParsedJournal(
                ParsedAnchor.Empty,
                ParsedAnchor.Empty,
                Array.Empty<object?>());

            public ParsedJournal(
                ParsedAnchor baseAnchor,
                ParsedAnchor currentAnchor,
                IReadOnlyList<object?> entries)
            {
                Base = baseAnchor;
                Current = currentAnchor;
                Entries = entries;
            }

            public ParsedAnchor Base { get; }
            public ParsedAnchor Current { get; }
            public IReadOnlyList<object?> Entries { get; }
        }

        private sealed class ParsedCursor
        {
            public static readonly ParsedCursor Empty = new ParsedCursor(
                string.Empty,
                1,
                0,
                new string('0', 64),
                0,
                new string('0', 64),
                0,
                1,
                0);

            public ParsedCursor(
                string worldId,
                int stateSchemaVersion,
                long revision,
                string hash,
                long baseRevision,
                string baseHash,
                int entryCount,
                int limit,
                int offset)
            {
                WorldId = worldId;
                StateSchemaVersion = stateSchemaVersion;
                Revision = revision;
                Hash = hash;
                BaseRevision = baseRevision;
                BaseHash = baseHash;
                EntryCount = entryCount;
                Limit = limit;
                Offset = offset;
            }

            public string WorldId { get; }
            public int StateSchemaVersion { get; }
            public long Revision { get; }
            public string Hash { get; }
            public long BaseRevision { get; }
            public string BaseHash { get; }
            public int EntryCount { get; }
            public int Limit { get; }
            public int Offset { get; }
        }
    }
}
