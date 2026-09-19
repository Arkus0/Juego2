using System;

namespace Arkus.Game.World
{
    internal static class StableToken
    {
        public static string Validate(string value, string parameterName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.Length == 0 || value.Length > 128)
            {
                throw new ArgumentException("Stable identifiers must contain between 1 and 128 characters.", parameterName);
            }

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                var valid =
                    character >= 'a' && character <= 'z' ||
                    character >= '0' && character <= '9' ||
                    character == '.' ||
                    character == '_' ||
                    character == '-';

                if (!valid)
                {
                    throw new ArgumentException(
                        "Stable identifiers use lowercase ASCII letters, digits, '.', '_' and '-' only.",
                        parameterName);
                }
            }

            return value;
        }
    }

    public readonly struct WorldId : IEquatable<WorldId>, IComparable<WorldId>
    {
        private readonly string? _value;

        public WorldId(string value)
        {
            _value = StableToken.Validate(value, nameof(value));
        }

        public string Value => _value ?? string.Empty;

        public int CompareTo(WorldId other) => StringComparer.Ordinal.Compare(Value, other.Value);

        public bool Equals(WorldId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object? obj) => obj is WorldId other && Equals(other);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() => Value;

        public static bool operator ==(WorldId left, WorldId right) => left.Equals(right);

        public static bool operator !=(WorldId left, WorldId right) => !left.Equals(right);
    }

    public readonly struct WorldObjectId : IEquatable<WorldObjectId>, IComparable<WorldObjectId>
    {
        private readonly string? _value;

        public WorldObjectId(string value)
        {
            _value = StableToken.Validate(value, nameof(value));
        }

        public string Value => _value ?? string.Empty;

        public int CompareTo(WorldObjectId other) => StringComparer.Ordinal.Compare(Value, other.Value);

        public bool Equals(WorldObjectId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object? obj) => obj is WorldObjectId other && Equals(other);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() => Value;

        public static bool operator ==(WorldObjectId left, WorldObjectId right) => left.Equals(right);

        public static bool operator !=(WorldObjectId left, WorldObjectId right) => !left.Equals(right);
    }

    public readonly struct WorldTypeId : IEquatable<WorldTypeId>, IComparable<WorldTypeId>
    {
        private readonly string? _value;

        public WorldTypeId(string value)
        {
            _value = StableToken.Validate(value, nameof(value));
        }

        public string Value => _value ?? string.Empty;

        public int CompareTo(WorldTypeId other) => StringComparer.Ordinal.Compare(Value, other.Value);

        public bool Equals(WorldTypeId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object? obj) => obj is WorldTypeId other && Equals(other);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() => Value;

        public static bool operator ==(WorldTypeId left, WorldTypeId right) => left.Equals(right);

        public static bool operator !=(WorldTypeId left, WorldTypeId right) => !left.Equals(right);
    }

    public readonly struct WorldReferenceKind : IEquatable<WorldReferenceKind>, IComparable<WorldReferenceKind>
    {
        private readonly string? _value;

        public WorldReferenceKind(string value)
        {
            _value = StableToken.Validate(value, nameof(value));
        }

        public string Value => _value ?? string.Empty;

        public int CompareTo(WorldReferenceKind other) => StringComparer.Ordinal.Compare(Value, other.Value);

        public bool Equals(WorldReferenceKind other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object? obj) => obj is WorldReferenceKind other && Equals(other);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

        public override string ToString() => Value;

        public static bool operator ==(WorldReferenceKind left, WorldReferenceKind right) => left.Equals(right);

        public static bool operator !=(WorldReferenceKind left, WorldReferenceKind right) => !left.Equals(right);
    }
