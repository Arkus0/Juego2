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
    /// <summary>Read-only access to the authored mutation journal.</summary>
    public interface IWorldProvenanceService
    {
        CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request);
    }

    public sealed class UnavailableWorldProvenanceService : IWorldProvenanceService
    {
        public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return CapabilityInvocationResult.Failed(new StructuredError(
                "world.state_unavailable",
                "No authored-world provenance journal is bound to this runtime instance.",
                "$",
                new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)),
                true,
                "Bind a transactional world authoring session before reading provenance."));
        }
    }

    /// <summary>
    /// Immutable identity of one canonical authored state. Runtime observations may refer to this
    /// anchor, but the anchor does not grant mutation or journal authority.
    /// </summary>
    public sealed class AuthoredWorldAnchor
    {
        public AuthoredWorldAnchor(string worldId, int stateSchemaVersion, long revision, string hash)
        {
            var validatedWorldId = new WorldId(worldId);
            if (stateSchemaVersion <= 0) throw new ArgumentOutOfRangeException(nameof(stateSchemaVersion));
            if (revision < 0) throw new ArgumentOutOfRangeException(nameof(revision));
            if (!IsCanonicalHash(hash)) throw new ArgumentException("Hash must be lowercase SHA-256 hex.", nameof(hash));

            WorldId = validatedWorldId.Value;
            StateSchemaVersion = stateSchemaVersion;
            Revision = revision;
            Hash = hash;
        }

        public string WorldId { get; }
        public int StateSchemaVersion { get; }
        public long Revision { get; }
        public string Hash { get; }

        public static AuthoredWorldAnchor FromState(WorldState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            return new AuthoredWorldAnchor(
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

        private static bool IsCanonicalHash(string? value)
        {
            if (value == null || value.Length != 64) return false;
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

        internal static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> values)
        {
            return new ReadOnlyDictionary<string, object?>(
                new Dictionary<string, object?>(values, StringComparer.Ordinal));
        }
    }

    /// <summary>
    /// Separately named contract stamp for future live/runtime observations. It identifies the
    /// authored base observed without making transient observation fields canonical authored state.
    /// </summary>
    public sealed class RuntimeObservationStamp
    {
        public const string SchemaId = "arkus.runtime-observation-stamp@1";

        public RuntimeObservationStamp(AuthoredWorldAnchor authoredBase)
        {
            AuthoredBase = authoredBase ?? throw new ArgumentNullException(nameof(authoredBase));
        }

        public AuthoredWorldAnchor AuthoredBase { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            return AuthoredWorldAnchor.ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = SchemaId,
                ["authoredBase"] = AuthoredBase.ToData()
            });
        }
    }

    /// <summary>
    /// Independent invariant guard between the parsed mutation identity and the normalized request
    /// body persisted for future replay. This intentionally consumes the machine-readable envelope
    /// instead of the private parsed operation objects so a serializer drift cannot remain green.
    /// </summary>
    internal static class WorldProvenanceIntegrity
    {
        private const string FingerprintVersion = "arkus-world-mutation-request-v2";
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        public static bool RequestBindingMatches(
            IReadOnlyDictionary<string, object?> normalizedRequest,
            string idempotencyKey,
            string requestFingerprint,
            AuthoredWorldAnchor baseAnchor)
        {
            if (normalizedRequest == null || idempotencyKey == null || requestFingerprint == null || baseAnchor == null)
                return false;
            if (!OnlyFields(normalizedRequest, "idempotencyKey", "expectedRevision", "expectedHash", "operations"))
                return false;
            if (!TryString(normalizedRequest, "idempotencyKey", out var journalKey) ||
                !string.Equals(journalKey, idempotencyKey, StringComparison.Ordinal))
                return false;
            if (!TryLong(normalizedRequest, "expectedRevision", out var expectedRevision) ||
                expectedRevision != baseAnchor.Revision)
                return false;
            if (!TryString(normalizedRequest, "expectedHash", out var expectedHash) ||
                !string.Equals(expectedHash, baseAnchor.Hash, StringComparison.Ordinal))
                return false;
            return TryComputeRequestFingerprint(normalizedRequest, out var computed) &&
                string.Equals(computed, requestFingerprint, StringComparison.Ordinal);
        }

        public static bool EntryIdentityMatches(
            string entryId,
            long sequence,
            string capabilityName,
            string capabilityVersion,
            string idempotencyKey,
            string requestFingerprint,
            IReadOnlyDictionary<string, object?> normalizedRequest,
            AuthoredWorldAnchor baseAnchor,
            AuthoredWorldAnchor resultAnchor,
            IReadOnlyList<string> affectedResources)
        {
            if (!RequestBindingMatches(normalizedRequest, idempotencyKey, requestFingerprint, baseAnchor)) return false;
            if (!string.Equals(baseAnchor.WorldId, resultAnchor.WorldId, StringComparison.Ordinal) ||
                baseAnchor.StateSchemaVersion != resultAnchor.StateSchemaVersion ||
                resultAnchor.Revision != baseAnchor.Revision + 1)
                return false;

            var builder = new StringBuilder();
            AppendIdentityField(builder, WorldProvenanceContract.EntrySchemaId);
            AppendIdentityField(builder, sequence.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(builder, capabilityName);
            AppendIdentityField(builder, capabilityVersion);
            AppendIdentityField(builder, idempotencyKey);
            AppendIdentityField(builder, requestFingerprint);
            AppendIdentityField(builder, baseAnchor.WorldId);
            AppendIdentityField(builder, baseAnchor.StateSchemaVersion.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(builder, baseAnchor.Revision.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(builder, baseAnchor.Hash);
            AppendIdentityField(builder, resultAnchor.Revision.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(builder, resultAnchor.Hash);
            for (var index = 0; index < affectedResources.Count; index++)
            {
                AppendIdentityField(builder, affectedResources[index]);
            }

            return string.Equals(entryId, Sha256(builder.ToString()), StringComparison.Ordinal);
        }

        private static bool TryComputeRequestFingerprint(
            IReadOnlyDictionary<string, object?> normalizedRequest,
            out string fingerprint)
        {
            fingerprint = string.Empty;
            if (!TryLong(normalizedRequest, "expectedRevision", out var expectedRevision) ||
                !TryString(normalizedRequest, "expectedHash", out var expectedHash) ||
                !normalizedRequest.TryGetValue("operations", out var rawOperations) ||
                !(rawOperations is IReadOnlyList<object?> operations))
                return false;

            var builder = new StringBuilder();
            builder.Append(FingerprintVersion).Append('\n');
            builder.Append(expectedRevision.ToString(CultureInfo.InvariantCulture)).Append('\n');
            builder.Append(expectedHash).Append('\n');
            for (var index = 0; index < operations.Count; index++)
            {
                if (!(operations[index] is IReadOnlyDictionary<string, object?> operation) ||
                    !TryAppendOperationFingerprint(builder, operation))
                    return false;
            }

            fingerprint = Sha256(builder.ToString());
            return true;
        }

        private static bool TryAppendOperationFingerprint(
            StringBuilder builder,
            IReadOnlyDictionary<string, object?> operation)
        {
            if (!TryString(operation, "kind", out var kind)) return false;
            switch (kind)
            {
                case "put-object":
                    if (!OnlyFields(operation, "kind", "id", "typeId", "containerId", "references") ||
                        !TryString(operation, "id", out var id) ||
                        !TryString(operation, "typeId", out var typeId) ||
                        !TryOptionalString(operation, "containerId", out var containerId) ||
                        !TryReferences(operation, "references", out var references))
                        return false;
                    builder.Append(0).Append('\t').Append(id).Append('\t').Append(typeId).Append('\t')
                        .Append(containerId ?? "-").Append('\t');
                    AppendReferences(builder, references);
                    break;

                case "remove-object":
                    if (!OnlyFields(operation, "kind", "id") || !TryString(operation, "id", out var removeId))
                        return false;
                    builder.Append(1).Append('\t').Append(removeId);
                    break;

                case "put-extension":
                    if (!OnlyFields(operation, "kind", "owner", "schemaVersion", "subjectId", "dependencies", "payloadBase64") ||
                        !TryString(operation, "owner", out var owner) ||
                        !TryInt(operation, "schemaVersion", out var schemaVersion) || schemaVersion <= 0 ||
                        !TryOptionalString(operation, "subjectId", out var subjectId) ||
                        !TryReferences(operation, "dependencies", out var dependencies) ||
                        !TryString(operation, "payloadBase64", out var payloadBase64) ||
                        !IsCanonicalBase64(payloadBase64))
                        return false;
                    builder.Append(2).Append('\t').Append(owner).Append('\t')
                        .Append(schemaVersion.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(subjectId == null
                            ? "global"
                            : "object:" + subjectId.Length.ToString(CultureInfo.InvariantCulture) + ":" + subjectId)
                        .Append('\t');
                    AppendReferences(builder, dependencies);
                    builder.Append('\t').Append(payloadBase64);
                    break;

                case "remove-extension":
                    if (!OnlyFields(operation, "kind", "owner", "schemaVersion", "subjectId") ||
                        !TryString(operation, "owner", out var removeOwner) ||
                        !TryInt(operation, "schemaVersion", out var removeVersion) || removeVersion <= 0 ||
                        !TryOptionalString(operation, "subjectId", out var removeSubjectId))
                        return false;
                    builder.Append(3).Append('\t').Append(removeOwner).Append('\t')
                        .Append(removeVersion.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(removeSubjectId == null
                            ? "global"
                            : "object:" + removeSubjectId.Length.ToString(CultureInfo.InvariantCulture) + ":" + removeSubjectId);
                    break;

                default:
                    return false;
            }

            builder.Append('\n');
            return true;
        }

        private static bool TryReferences(
            IReadOnlyDictionary<string, object?> operation,
            string field,
            out IReadOnlyList<KeyValuePair<string, string>> references)
        {
            references = Array.Empty<KeyValuePair<string, string>>();
            if (!operation.TryGetValue(field, out var raw) || !(raw is IReadOnlyList<object?> values)) return false;
            var parsed = new List<KeyValuePair<string, string>>();
            for (var index = 0; index < values.Count; index++)
            {
                if (!(values[index] is IReadOnlyDictionary<string, object?> reference) ||
                    !OnlyFields(reference, "kind", "targetId") ||
                    !TryString(reference, "kind", out var kind) ||
                    !TryString(reference, "targetId", out var targetId))
                    return false;
                parsed.Add(new KeyValuePair<string, string>(kind, targetId));
            }

            references = parsed.AsReadOnly();
            return true;
        }

        private static void AppendReferences(
            StringBuilder builder,
            IReadOnlyList<KeyValuePair<string, string>> references)
        {
            for (var index = 0; index < references.Count; index++)
            {
                builder.Append(references[index].Key).Append("->").Append(references[index].Value).Append(';');
            }
        }

        private static bool TryOptionalString(
            IReadOnlyDictionary<string, object?> data,
            string key,
            out string? value)
        {
            value = null;
            if (!data.TryGetValue(key, out var raw)) return true;
            if (!(raw is string text)) return false;
            value = text;
            return true;
        }

        private static bool TryString(IReadOnlyDictionary<string, object?> data, string key, out string value)
        {
            value = string.Empty;
            if (!data.TryGetValue(key, out var raw) || !(raw is string text)) return false;
            value = text;
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

        private static bool TryInt(IReadOnlyDictionary<string, object?> data, string key, out int value)
        {
            value = 0;
            if (!TryLong(data, key, out var parsed) || parsed < int.MinValue || parsed > int.MaxValue) return false;
            value = (int)parsed;
            return true;
        }

        private static bool OnlyFields(IReadOnlyDictionary<string, object?> data, params string[] allowed)
        {
            var set = new HashSet<string>(allowed, StringComparer.Ordinal);
            foreach (var key in data.Keys)
            {
                if (!set.Contains(key)) return false;
            }

            return true;
        }

        private static bool IsCanonicalBase64(string value)
        {
            try
            {
                var bytes = Convert.FromBase64String(value);
                return string.Equals(Convert.ToBase64String(bytes), value, StringComparison.Ordinal);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static void AppendIdentityField(StringBuilder builder, string value)
        {
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture))
                .Append(':')
                .Append(value)
                .Append('\n');
        }

        private static string Sha256(string value)
        {
            byte[] digest;
            using (var sha = SHA256.Create())
            {
                digest = sha.ComputeHash(StrictUtf8.GetBytes(value));
            }

            const string alphabet = "0123456789abcdef";
            var characters = new char[digest.Length * 2];
            for (var index = 0; index < digest.Length; index++)
            {
                characters[index * 2] = alphabet[digest[index] >> 4];
                characters[index * 2 + 1] = alphabet[digest[index] & 0x0f];
            }

            return new string(characters);
        }
    }

    internal sealed class WorldMutationProvenanceEntry
    {
        public WorldMutationProvenanceEntry(
            string entryId,
            long sequence,
            string capabilityName,
            string capabilityVersion,
            string idempotencyKey,
            string requestFingerprint,
            IReadOnlyDictionary<string, object?> normalizedRequest,
            AuthoredWorldAnchor baseAnchor,
            AuthoredWorldAnchor resultAnchor,
            IReadOnlyList<string> affectedResources)
        {
            if (entryId == null) throw new ArgumentNullException(nameof(entryId));
            if (sequence <= 0) throw new ArgumentOutOfRangeException(nameof(sequence));
            if (capabilityName == null) throw new ArgumentNullException(nameof(capabilityName));
            if (capabilityVersion == null) throw new ArgumentNullException(nameof(capabilityVersion));
            if (idempotencyKey == null) throw new ArgumentNullException(nameof(idempotencyKey));
            if (requestFingerprint == null) throw new ArgumentNullException(nameof(requestFingerprint));
            if (normalizedRequest == null) throw new ArgumentNullException(nameof(normalizedRequest));
            if (baseAnchor == null) throw new ArgumentNullException(nameof(baseAnchor));
            if (resultAnchor == null) throw new ArgumentNullException(nameof(resultAnchor));
            var copiedAffectedResources = Copy(affectedResources);

            if (!WorldProvenanceIntegrity.RequestBindingMatches(
                    normalizedRequest,
                    idempotencyKey,
                    requestFingerprint,
                    baseAnchor))
            {
                throw new InvalidOperationException(
                    "The normalized accepted request is not bound to the parsed mutation fingerprint/base anchor.");
            }

            if (!WorldProvenanceIntegrity.EntryIdentityMatches(
                    entryId,
                    sequence,
                    capabilityName,
                    capabilityVersion,
                    idempotencyKey,
                    requestFingerprint,
                    normalizedRequest,
                    baseAnchor,
                    resultAnchor,
                    copiedAffectedResources))
            {
                throw new InvalidOperationException(
                    "The provenance entry identity is not bound to the normalized accepted request and transition.");
            }

            EntryId = entryId;
            Sequence = sequence;
            CapabilityName = capabilityName;
            CapabilityVersion = capabilityVersion;
            IdempotencyKey = idempotencyKey;
            RequestFingerprint = requestFingerprint;
            NormalizedRequest = normalizedRequest;
            Base = baseAnchor;
            Result = resultAnchor;
            AffectedResources = copiedAffectedResources;
        }

        public string EntryId { get; }
        public long Sequence { get; }
        public string CapabilityName { get; }
        public string CapabilityVersion { get; }
        public string IdempotencyKey { get; }
        public string RequestFingerprint { get; }
        public IReadOnlyDictionary<string, object?> NormalizedRequest { get; }
        public AuthoredWorldAnchor Base { get; }
        public AuthoredWorldAnchor Result { get; }
        public IReadOnlyList<string> AffectedResources { get; }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var affected = new List<object?>();
            for (var index = 0; index < AffectedResources.Count; index++)
            {
                affected.Add(AffectedResources[index]);
            }

            return AuthoredWorldAnchor.ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = WorldProvenanceContract.EntrySchemaId,
                ["entryId"] = EntryId,
                ["sequence"] = Sequence,
                ["capability"] = AuthoredWorldAnchor.ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["name"] = CapabilityName,
                    ["version"] = CapabilityVersion
                }),
                ["requestIdentity"] = AuthoredWorldAnchor.ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["idempotencyKey"] = IdempotencyKey,
                    ["fingerprint"] = RequestFingerprint
                }),
                ["request"] = NormalizedRequest,
                ["base"] = Base.ToData(),
                ["result"] = Result.ToData(),
                ["affectedResources"] = affected.AsReadOnly()
            });
        }

        private static IReadOnlyList<string> Copy(IReadOnlyList<string> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var copy = new List<string>();
            for (var index = 0; index < values.Count; index++)
            {
                copy.Add(values[index] ?? throw new ArgumentException("Affected resources cannot contain null.", nameof(values)));
            }

            return copy.AsReadOnly();
        }
    }
}
