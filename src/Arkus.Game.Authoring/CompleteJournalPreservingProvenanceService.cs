using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// Compatibility-preserving HK08A view: a bounded read that already covers the whole journal is
    /// returned as the exact accepted HK06A complete artifact. Only genuinely partial reads retain
    /// the explicit journal-page marker/cursor shape, so replay can never silently consume a page as
    /// though it were a complete journal.
    /// </summary>
    public sealed class CompleteJournalPreservingProvenanceService : IWorldProvenanceService
    {
        private readonly IWorldProvenanceService _bounded;

        public CompleteJournalPreservingProvenanceService(IWorldProvenanceService bounded)
        {
            _bounded = bounded ?? throw new ArgumentNullException(nameof(bounded));
        }

        public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var result = _bounded.ReadJournal(request);
            if (!result.Success || result.Data == null) return result;

            var data = result.Data;
            if (!TryGetString(data, "schemaId", out var schemaId) ||
                !string.Equals(schemaId, WorldProvenanceContract.JournalPageSchemaId, StringComparison.Ordinal) ||
                data.ContainsKey("nextCursor") ||
                !TryGetInteger(data, "pageOffset", out var offset) || offset != 0 ||
                !TryGetInteger(data, "entryCount", out var entryCount) || entryCount < 0 || entryCount > int.MaxValue ||
                !data.TryGetValue("entries", out var rawEntries) || !(rawEntries is IReadOnlyList<object?> entries) ||
                entries.Count != (int)entryCount)
            {
                return result;
            }

            if (!data.TryGetValue("entrySchemaId", out var entrySchemaId) ||
                !data.TryGetValue("base", out var baseAnchor) ||
                !data.TryGetValue("current", out var currentAnchor))
            {
                return result;
            }

            var complete = new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["schemaId"] = WorldProvenanceContract.JournalSchemaId,
                    ["entrySchemaId"] = entrySchemaId,
                    ["base"] = baseAnchor,
                    ["current"] = currentAnchor,
                    ["entryCount"] = entryCount,
                    ["entries"] = entries
                });
            return CapabilityInvocationResult.Succeeded(complete);
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
            if (!data.TryGetValue(key, out var raw)) return false;
            switch (raw)
            {
                case byte typed: value = typed; return true;
                case sbyte typed: value = typed; return true;
                case short typed: value = typed; return true;
                case ushort typed: value = typed; return true;
                case int typed: value = typed; return true;
                case uint typed: value = typed; return true;
                case long typed: value = typed; return true;
                default: return false;
            }
        }
    }
}
