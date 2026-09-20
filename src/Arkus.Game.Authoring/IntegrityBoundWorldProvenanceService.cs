using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// HK08A integrity envelope for public journal cursors. The underlying bounded provenance
    /// service owns pagination semantics; this wrapper only makes the cursor's complete context
    /// self-verifying so altering offset/limit/anchor fields cannot silently create gaps or repeats.
    /// The checksum is deterministic framing integrity, not an authorization or secrecy mechanism.
    /// </summary>
    public sealed class IntegrityBoundWorldProvenanceService : IWorldProvenanceService
    {
        private const string CursorEnvelopeVersion = "arkus-journal-cursor-envelope-v1";
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        private readonly IWorldProvenanceService _inner;

        public IntegrityBoundWorldProvenanceService(IWorldProvenanceService inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            IReadOnlyDictionary<string, object?> forwarded = request;
            if (request.TryGetValue("cursor", out var rawCursor) && rawCursor is string encodedCursor)
            {
                if (!TryUnwrapCursor(encodedCursor, out var innerCursor))
                {
                    return InvalidCursor(
                        "Cursor integrity check failed; its paging context may have been altered or corrupted.");
                }

                var copy = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var pair in request) copy.Add(pair.Key, pair.Value);
                copy["cursor"] = innerCursor;
                forwarded = ReadOnly(copy);
            }

            var result = _inner.ReadJournal(forwarded);
            if (!result.Success || result.Data == null ||
                !result.Data.TryGetValue("nextCursor", out var rawNextCursor) ||
                !(rawNextCursor is string nextCursor))
            {
                return result;
            }

            var payload = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var pair in result.Data) payload.Add(pair.Key, pair.Value);
            payload["nextCursor"] = WrapCursor(nextCursor);
            return CapabilityInvocationResult.Succeeded(ReadOnly(payload));
        }

        private static string WrapCursor(string innerCursor)
        {
            var payload = CursorEnvelopeVersion + "\n" + innerCursor;
            var envelope = payload + "\n" + ComputeChecksum(payload);
            return Convert.ToBase64String(StrictUtf8.GetBytes(envelope));
        }

        private static bool TryUnwrapCursor(string value, out string innerCursor)
        {
            innerCursor = string.Empty;
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
            if (parts.Length != 3 ||
                !string.Equals(parts[0], CursorEnvelopeVersion, StringComparison.Ordinal) ||
                parts[1].Length == 0 || parts[1].Length > 4096 ||
                !IsCanonicalHash(parts[2]))
            {
                return false;
            }

            var payload = parts[0] + "\n" + parts[1];
            if (!string.Equals(ComputeChecksum(payload), parts[2], StringComparison.Ordinal)) return false;

            innerCursor = parts[1];
            return true;
        }

        private static string ComputeChecksum(string payload)
        {
            using (var sha256 = SHA256.Create())
            {
                var digest = sha256.ComputeHash(StrictUtf8.GetBytes(payload));
                var builder = new StringBuilder(digest.Length * 2);
                for (var index = 0; index < digest.Length; index++)
                {
                    builder.Append(digest[index].ToString("x2", CultureInfo.InvariantCulture));
                }
                return builder.ToString();
            }
        }

        private static bool IsCanonicalHash(string value)
        {
            if (value.Length != 64) return false;
            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f'))) return false;
            }
            return true;
        }

        private static CapabilityInvocationResult InvalidCursor(string message)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.provenance.invalid_cursor",
                message,
                "$.cursor",
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)),
                false,
                "Discard the cursor and restart the bounded journal read against the current authored anchor."));
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> data)
        {
            return new ReadOnlyDictionary<string, object?>(data);
        }
    }
}
