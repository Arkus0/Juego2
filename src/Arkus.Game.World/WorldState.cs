using System;
using System.Collections.Generic;

namespace Arkus.Game.World
{
    public sealed class WorldStateException : InvalidOperationException
    {
        public WorldStateException(string machineCode, string message, string path)
            : base(message)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string MachineCode { get; }

        public string Path { get; }
    }

    public sealed class WorldReference : IEquatable<WorldReference>
    {
        public WorldReference(WorldReferenceKind kind, WorldObjectId targetId)
        {
            if (string.IsNullOrEmpty(kind.Value))
            {
                throw new ArgumentException("Reference kind must be initialized.", nameof(kind));
            }

            if (string.IsNullOrEmpty(targetId.Value))
            {
                throw new ArgumentException("Reference target must be initialized.", nameof(targetId));
            }

            Kind = kind;
            TargetId = targetId;
        }

        public WorldReferenceKind Kind { get; }

        public WorldObjectId TargetId { get; }

        public bool Equals(WorldReference? other)
        {
            return other is not null && Kind == other.Kind && TargetId == other.TargetId;
        }

        public override bool Equals(object? obj) => Equals(obj as WorldReference);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Kind.GetHashCode() * 397) ^ TargetId.GetHashCode();
            }
        }
    }

    public sealed class WorldObject
    {
        private readonly IReadOnlyList<WorldReference> _references;

        public WorldObject(
            WorldObjectId id,
            WorldTypeId typeId,
            WorldObjectId? containerId = null,
            IEnumerable<WorldReference>? references = null)
        {
            if (string.IsNullOrEmpty(id.Value))
            {
                throw new ArgumentException("Object ID must be initialized.", nameof(id));
            }

            if (string.IsNullOrEmpty(typeId.Value))
            {
                throw new ArgumentException("Object type must be initialized.", nameof(typeId));
            }

            Id = id;
            TypeId = typeId;
            ContainerId = containerId;
            _references = new List<WorldReference>(references ?? Array.Empty<WorldReference>()).AsReadOnly();
        }

        public WorldObjectId Id { get; }

        public WorldTypeId TypeId { get; }

        public WorldObjectId? ContainerId { get; }

        public IReadOnlyList<WorldReference> References => _references;
    }

    public readonly struct WorldExtensionIdentity : IEquatable<WorldExtensionIdentity>, IComparable<WorldExtensionIdentity>
    {
        public WorldExtensionIdentity(string owner, int schemaVersion, WorldObjectId? subjectId = null)
        {
            Owner = StableToken.Validate(owner, nameof(owner));
            if (schemaVersion <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(schemaVersion), "Extension schema versions must be positive.");
            }

            if (subjectId.HasValue && string.IsNullOrEmpty(subjectId.Value.Value))
            {
                throw new ArgumentException("Extension subject IDs must be initialized.", nameof(subjectId));
            }

            SchemaVersion = schemaVersion;
            SubjectId = subjectId;
        }

        public string Owner { get; }

        public int SchemaVersion { get; }

        public WorldObjectId? SubjectId { get; }

        public string ResourceKey =>
            Owner + "@" + SchemaVersion.ToString(System.Globalization.CultureInfo.InvariantCulture) + "@" +
            (SubjectId.HasValue ? "object:" + SubjectId.Value.Value : "global");

        public int CompareTo(WorldExtensionIdentity other)
        {
            var comparison = StringComparer.Ordinal.Compare(Owner, other.Owner);
            if (comparison != 0)
            {
                return comparison;
            }

            comparison = SchemaVersion.CompareTo(other.SchemaVersion);
            if (comparison != 0)
            {
                return comparison;
            }

            if (!SubjectId.HasValue)
            {
                return other.SubjectId.HasValue ? -1 : 0;
            }

            return other.SubjectId.HasValue ? SubjectId.Value.CompareTo(other.SubjectId.Value) : 1;
        }

        public bool Equals(WorldExtensionIdentity other)
        {
            return StringComparer.Ordinal.Equals(Owner, other.Owner) &&
                SchemaVersion == other.SchemaVersion &&
                SubjectId == other.SubjectId;
        }

        public override bool Equals(object? obj) => obj is WorldExtensionIdentity other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.Ordinal.GetHashCode(Owner ?? string.Empty);
                hash = (hash * 397) ^ SchemaVersion;
                hash = (hash * 397) ^ (SubjectId.HasValue ? SubjectId.Value.GetHashCode() : 0);
                return hash;
            }
        }

        public override string ToString() => ResourceKey;

        public static bool operator ==(WorldExtensionIdentity left, WorldExtensionIdentity right) => left.Equals(right);

        public static bool operator !=(WorldExtensionIdentity left, WorldExtensionIdentity right) => !left.Equals(right);
    }

    public sealed class WorldExtensionData
    {
        private readonly IReadOnlyList<WorldReference> _dependencies;
        private readonly byte[] _payload;

        public WorldExtensionData(
            string owner,
            int schemaVersion,
            byte[] payload,
            WorldObjectId? subjectId = null,
            IEnumerable<WorldReference>? dependencies = null)
        {
            if (payload is null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            Identity = new WorldExtensionIdentity(owner, schemaVersion, subjectId);
            _dependencies = new List<WorldReference>(dependencies ?? Array.Empty<WorldReference>()).AsReadOnly();
            _payload = (byte[])payload.Clone();
        }

        public WorldExtensionIdentity Identity { get; }

        public string Owner => Identity.Owner;

        public int SchemaVersion => Identity.SchemaVersion;

        public WorldObjectId? SubjectId => Identity.SubjectId;

        public IReadOnlyList<WorldReference> Dependencies => _dependencies;

        public int PayloadLength => _payload.Length;

        public byte[] GetPayloadCopy() => (byte[])_payload.Clone();

        internal byte[] PayloadBytes => _payload;
    }

    public sealed class WorldState
    {
        public const int CurrentSchemaVersion = 2;

        private readonly IReadOnlyList<WorldObject> _objects;
        private readonly IReadOnlyList<WorldExtensionData> _extensions;

        public WorldState(
            WorldId id,
            long revision,
            IEnumerable<WorldObject> objects,
            IEnumerable<WorldExtensionData>? extensions = null,
            int schemaVersion = CurrentSchemaVersion)
        {
            if (objects is null)
            {
                throw new ArgumentNullException(nameof(objects));
            }

            Id = id;
            Revision = revision;
            SchemaVersion = schemaVersion;
            _objects = new List<WorldObject>(objects).AsReadOnly();
            _extensions = new List<WorldExtensionData>(extensions ?? Array.Empty<WorldExtensionData>()).AsReadOnly();

            WorldStateValidator.ValidateCandidateOrThrow(new WorldStateCandidate(
                Id,
                Revision,
                _objects,
                _extensions,
                SchemaVersion));
        }

        public int SchemaVersion { get; }

        public long Revision { get; }

        public WorldId Id { get; }

        public IReadOnlyList<WorldObject> Objects => _objects;

        public IReadOnlyList<WorldExtensionData> Extensions => _extensions;
    }
}
