using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            EntryId = entryId ?? throw new ArgumentNullException(nameof(entryId));
            if (sequence <= 0) throw new ArgumentOutOfRangeException(nameof(sequence));
            Sequence = sequence;
            CapabilityName = capabilityName ?? throw new ArgumentNullException(nameof(capabilityName));
            CapabilityVersion = capabilityVersion ?? throw new ArgumentNullException(nameof(capabilityVersion));
            IdempotencyKey = idempotencyKey ?? throw new ArgumentNullException(nameof(idempotencyKey));
            RequestFingerprint = requestFingerprint ?? throw new ArgumentNullException(nameof(requestFingerprint));
            NormalizedRequest = normalizedRequest ?? throw new ArgumentNullException(nameof(normalizedRequest));
            Base = baseAnchor ?? throw new ArgumentNullException(nameof(baseAnchor));
            Result = resultAnchor ?? throw new ArgumentNullException(nameof(resultAnchor));
            AffectedResources = Copy(affectedResources);
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
