using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// HK06B keyed-idempotency authority for snapshot rebase. Receipts are deliberately separate
    /// from the HK06A mutation journal: they prove retry identity for the import command and do not
    /// claim that an imported or pre-import mutation history belongs to the new local lineage.
    /// The store is keyed by the authoritative session object so recomposing the runtime contract
    /// does not silently forget accepted import keys while that session remains alive.
    /// </summary>
    internal static class IdempotentWorldSnapshotAuthority
    {
        private static readonly ConditionalWeakTable<IWorldMutationService, ReceiptStore> Stores =
            new ConditionalWeakTable<IWorldMutationService, ReceiptStore>();

        public static ICanonicalWorldSnapshotImporter Bind(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            var raw = CanonicalWorldSnapshotAuthority.Bind(service);
            return new Importer(raw, Stores.GetValue(service, _ => new ReceiptStore()));
        }

        private sealed class Importer : ICanonicalWorldSnapshotImporter
        {
            private const string FingerprintVersion = "arkus-world-snapshot-import-v1";
            private readonly ICanonicalWorldSnapshotImporter _raw;
            private readonly ReceiptStore _store;

            public Importer(ICanonicalWorldSnapshotImporter raw, ReceiptStore store)
            {
                _raw = raw ?? throw new ArgumentNullException(nameof(raw));
                _store = store ?? throw new ArgumentNullException(nameof(store));
            }

            public CapabilityInvocationResult ImportSnapshot(IReadOnlyDictionary<string, object?> request)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (!WorldPortabilityEngine.OnlyFields(
                        request,
                        "idempotencyKey", "expectedRevision", "expectedHash", "snapshot") ||
                    !WorldPortabilityEngine.TryString(request, "idempotencyKey", out var idempotencyKey) ||
                    !IsStableKey(idempotencyKey))
                {
                    return WorldPortabilityEngine.Failure(
                        "world.snapshot.invalid_request",
                        "Snapshot import requires a stable idempotencyKey plus expectedRevision, expectedHash and snapshot.",
                        "$.idempotencyKey",
                        false,
                        "Use a non-empty stable idempotency key of at most 128 ASCII token characters.");
                }

                var forwarded = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var pair in request)
                {
                    if (!string.Equals(pair.Key, "idempotencyKey", StringComparison.Ordinal))
                        forwarded.Add(pair.Key, pair.Value);
                }

                var fingerprint = Fingerprint(forwarded);
                lock (_store.Gate)
                {
                    if (_store.Receipts.TryGetValue(idempotencyKey, out var existing))
                    {
                        if (!string.Equals(existing.Fingerprint, fingerprint, StringComparison.Ordinal))
                        {
                            return WorldPortabilityEngine.Failure(
                                "world.snapshot.idempotency_conflict",
                                "This idempotency key was already accepted for different snapshot-import semantics.",
                                "$.idempotencyKey",
                                false,
                                "Retry the original import for this key or allocate a new key for different semantics.");
                        }

                        return CapabilityInvocationResult.Succeeded(WithReplay(existing.Result, true));
                    }

                    var result = _raw.ImportSnapshot(new ReadOnlyDictionary<string, object?>(forwarded));
                    if (!result.Success || result.Data == null) return result;

                    var first = WithReplay(result.Data, false);
                    _store.Receipts.Add(idempotencyKey, new Receipt(fingerprint, first));
                    return CapabilityInvocationResult.Succeeded(first);
                }
            }

            private static bool IsStableKey(string value)
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 128) return false;
                for (var index = 0; index < value.Length; index++)
                {
                    var character = value[index];
                    var accepted =
                        (character >= 'a' && character <= 'z') ||
                        (character >= 'A' && character <= 'Z') ||
                        (character >= '0' && character <= '9') ||
                        character == '.' || character == '_' || character == ':' || character == '-';
                    if (!accepted) return false;
                }
                return true;
            }

            private static string Fingerprint(IReadOnlyDictionary<string, object?> request)
            {
                var builder = new StringBuilder(FingerprintVersion);
                AppendValue(builder, request);
                using (var sha = SHA256.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(builder.ToString());
                    var hash = sha.ComputeHash(bytes);
                    var text = new StringBuilder(hash.Length * 2);
                    for (var index = 0; index < hash.Length; index++)
                        text.Append(hash[index].ToString("x2", CultureInfo.InvariantCulture));
                    return text.ToString();
                }
            }

            private static void AppendValue(StringBuilder builder, object? value)
            {
                if (value == null)
                {
                    builder.Append("N;");
                    return;
                }

                if (value is string text)
                {
                    builder.Append('S').Append(text.Length.ToString(CultureInfo.InvariantCulture))
                        .Append(':').Append(text).Append(';');
                    return;
                }

                if (value is bool flag)
                {
                    builder.Append(flag ? "B1;" : "B0;");
                    return;
                }

                if (TryInteger(value, out var integer))
                {
                    builder.Append('I').Append(integer.ToString(CultureInfo.InvariantCulture)).Append(';');
                    return;
                }

                if (value is IReadOnlyDictionary<string, object?> map)
                {
                    builder.Append("M{");
                    var keys = new List<string>(map.Keys);
                    keys.Sort(StringComparer.Ordinal);
                    for (var index = 0; index < keys.Count; index++)
                    {
                        AppendValue(builder, keys[index]);
                        AppendValue(builder, map[keys[index]]);
                    }
                    builder.Append("};");
                    return;
                }

                if (value is IReadOnlyList<object?> list)
                {
                    builder.Append("L[");
                    for (var index = 0; index < list.Count; index++) AppendValue(builder, list[index]);
                    builder.Append("];");
                    return;
                }

                throw new ArgumentException("Snapshot import contains a value outside the canonical request grammar.");
            }

            private static bool TryInteger(object value, out long integer)
            {
                integer = 0;
                switch (value)
                {
                    case sbyte current: integer = current; return true;
                    case byte current: integer = current; return true;
                    case short current: integer = current; return true;
                    case ushort current: integer = current; return true;
                    case int current: integer = current; return true;
                    case uint current: integer = current; return true;
                    case long current: integer = current; return true;
                    case ulong current when current <= long.MaxValue: integer = (long)current; return true;
                    default: return false;
                }
            }

            private static IReadOnlyDictionary<string, object?> WithReplay(
                IReadOnlyDictionary<string, object?> source,
                bool replayed)
            {
                var copy = new Dictionary<string, object?>(source, StringComparer.Ordinal)
                {
                    ["replayed"] = replayed
                };
                return new ReadOnlyDictionary<string, object?>(copy);
            }
        }

        private sealed class ReceiptStore
        {
            public object Gate { get; } = new object();
            public Dictionary<string, Receipt> Receipts { get; } =
                new Dictionary<string, Receipt>(StringComparer.Ordinal);
        }

        private sealed class Receipt
        {
            public Receipt(string fingerprint, IReadOnlyDictionary<string, object?> result)
            {
                Fingerprint = fingerprint;
                Result = result;
            }

            public string Fingerprint { get; }
            public IReadOnlyDictionary<string, object?> Result { get; }
        }
    }
}
