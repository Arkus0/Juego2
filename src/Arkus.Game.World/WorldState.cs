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

    public sealed class WorldExtensionData
    {
        private readonly byte[] _payload;

        public WorldExtensionData(string owner, int schemaVersion, byte[] payload)
        {
            Owner = StableToken.Validate(owner, nameof(owner));
            if (schemaVersion <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(schemaVersion), "Extension schema versions must be positive.");
            }

            if (payload is null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            SchemaVersion = schemaVersion;
            _payload = (byte[])payload.Clone();
        }

        public string Owner { get; }

        public int SchemaVersion { get; }

        public int PayloadLength => _payload.Length;

        public byte[] GetPayloadCopy() => (byte[])_payload.Clone();

        internal byte[] PayloadBytes => _payload;
    }

    public sealed class WorldState
    {
        public const int CurrentSchemaVersion = 1;

        private readonly IReadOnlyList<WorldObject> _objects;
        private readonly IReadOnlyList<WorldExtensionData> _extensions;

        public WorldState(
            WorldId id,
            long revision,
            IEnumerable<WorldObject> objects,
            IEnumerable<WorldExtensionData>? extensions = null,
            int schemaVersion = CurrentSchemaVersion)
        {
            if (string.IsNullOrEmpty(id.Value))
            {
                throw new ArgumentException("World ID must be initialized.", nameof(id));
            }

            if (objects is null)
            {
                throw new ArgumentNullException(nameof(objects));
            }

            Id = id;
            Revision = revision;
            SchemaVersion = schemaVersion;
            _objects = new List<WorldObject>(objects).AsReadOnly();
            _extensions = new List<WorldExtensionData>(extensions ?? Array.Empty<WorldExtensionData>()).AsReadOnly();

            WorldStateValidator.ValidateOrThrow(this);
        }

        public int SchemaVersion { get; }

        public long Revision { get; }

        public WorldId Id { get; }

        public IReadOnlyList<WorldObject> Objects => _objects;

        public IReadOnlyList<WorldExtensionData> Extensions => _extensions;
    }
}
