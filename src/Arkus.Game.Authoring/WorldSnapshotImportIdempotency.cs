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
    /// The accepted binder name is retained, but HK09B moves keyed receipts into the same immutable
    /// PortableWorldAuthoringSession root as imported state and rebase evidence. No wrapper may
    /// publish a receipt after the state swap.
    /// </summary>
    internal static class IdempotentWorldSnapshotAuthority
    {
        public static ICanonicalWorldSnapshotImporter Bind(IWorldMutationService service)
        {
            return new SnapshotImportBindingAuthority(
                CanonicalWorldSnapshotAuthority.Bind(
                    service ?? throw new ArgumentNullException(nameof(service))));
        }

        /// <summary>
        /// Keeps the public Runtime handler from directly carrying the multi-surface session object.
        /// The actual import remains one Authoring-owned atomic authority behind this narrow adapter.
        /// </summary>
        private sealed class SnapshotImportBindingAuthority : ICanonicalWorldSnapshotImporter
        {
            private readonly ICanonicalWorldSnapshotImporter _importer;

            public SnapshotImportBindingAuthority(ICanonicalWorldSnapshotImporter importer)
            {
                _importer = importer ?? throw new ArgumentNullException(nameof(importer));
            }

            public CapabilityInvocationResult ImportSnapshot(
                IReadOnlyDictionary<string, object?> request,
                InvocationResourceBudget resourceBudget)
            {
                return _importer.ImportSnapshot(
                    request ?? throw new ArgumentNullException(nameof(request)),
                    resourceBudget ?? throw new ArgumentNullException(nameof(resourceBudget)));
            }
        }
    }

    internal static class WorldSnapshotImportIdentity
    {
        private const string FingerprintVersion = "arkus-world-snapshot-import-v1";

        public static bool IsStableKey(string value)
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

        public static string Fingerprint(IReadOnlyDictionary<string, object?> request)
        {
            var builder = new StringBuilder(FingerprintVersion);
            AppendValue(builder, request);
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));
                var text = new StringBuilder(hash.Length * 2);
                for (var index = 0; index < hash.Length; index++)
                    text.Append(hash[index].ToString("x2", CultureInfo.InvariantCulture));
                return text.ToString();
            }
        }

        public static IReadOnlyDictionary<string, object?> WithReplayAndEvidence(
            IReadOnlyDictionary<string, object?> source,
            bool replayed,
            string idempotencyKey,
            string requestFingerprint,
            IReadOnlyDictionary<string, object?> request)
        {
            var snapshot = (IReadOnlyDictionary<string, object?>)request["snapshot"]!;
            var evidence = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = WorldPortabilityContract.RebaseEvidenceSchemaId,
                ["idempotencyKey"] = idempotencyKey,
                ["requestFingerprint"] = requestFingerprint,
                ["snapshotSchemaId"] = snapshot["schemaId"],
                ["snapshotVersion"] = snapshot["snapshotVersion"],
                ["snapshotAnchor"] = snapshot["anchor"],
                ["previous"] = source["previous"],
                ["current"] = source["current"],
                ["lineageDisposition"] = source["lineageDisposition"],
                ["mutationJournalDisposition"] = "new-local-lineage-empty"
            });

            var copy = new Dictionary<string, object?>(source, StringComparer.Ordinal)
            {
                ["rebaseEvidence"] = evidence,
                ["replayed"] = replayed
            };
            return ReadOnly(copy);
        }

        public static IReadOnlyDictionary<string, object?> WithReplay(
            IReadOnlyDictionary<string, object?> source,
            bool replayed)
        {
            var copy = new Dictionary<string, object?>(source, StringComparer.Ordinal)
            {
                ["replayed"] = replayed
            };
            return ReadOnly(copy);
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

        private static IReadOnlyDictionary<string, object?> ReadOnly(
            IDictionary<string, object?> source)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(source, StringComparer.Ordinal));
        }
    }
}
