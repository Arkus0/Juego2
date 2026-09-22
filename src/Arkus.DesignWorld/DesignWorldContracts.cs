using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Arkus.DesignWorld
{
    public enum DesignValueKind
    {
        String = 1,
        Integer = 2,
        Boolean = 3,
        Decimal = 4
    }

    public readonly struct DesignValue : IEquatable<DesignValue>
    {
        private DesignValue(DesignValueKind kind, string canonicalValue)
        {
            Kind = kind;
            CanonicalValue = canonicalValue ?? throw new ArgumentNullException(nameof(canonicalValue));
        }

        public DesignValueKind Kind { get; }

        public string CanonicalValue { get; }

        public static DesignValue String(string value) => new DesignValue(DesignValueKind.String, value ?? throw new ArgumentNullException(nameof(value)));

        public static DesignValue Integer(long value) => new DesignValue(DesignValueKind.Integer, value.ToString(CultureInfo.InvariantCulture));

        public static DesignValue Boolean(bool value) => new DesignValue(DesignValueKind.Boolean, value ? "true" : "false");

        public static DesignValue Decimal(decimal value) => new DesignValue(DesignValueKind.Decimal, value.ToString("G29", CultureInfo.InvariantCulture));

        public bool Equals(DesignValue other) => Kind == other.Kind && StringComparer.Ordinal.Equals(CanonicalValue, other.CanonicalValue);

        public override bool Equals(object? obj) => obj is DesignValue other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Kind * 397) ^ StringComparer.Ordinal.GetHashCode(CanonicalValue ?? string.Empty);
            }
        }

        public override string ToString() => Kind.ToString() + ":" + CanonicalValue;

        public static bool operator ==(DesignValue left, DesignValue right) => left.Equals(right);

        public static bool operator !=(DesignValue left, DesignValue right) => !left.Equals(right);
    }

    public sealed class DesignRelation
    {
        public DesignRelation(string relationType, string targetFactId)
        {
            RelationType = DesignToken.Validate(relationType, nameof(relationType), 100);
            TargetFactId = DesignToken.Validate(targetFactId, nameof(targetFactId), 128);
        }

        public string RelationType { get; }

        public string TargetFactId { get; }
    }

    public sealed class DesignAuthorityAnchor
    {
        public DesignAuthorityAnchor(
            string authorityId,
            string sourcePath,
            string anchor,
            string sourceDigest,
            string anchorDigest)
        {
            AuthorityId = DesignToken.Validate(authorityId, nameof(authorityId), 128);
            SourcePath = RequireText(sourcePath, nameof(sourcePath));
            Anchor = RequireText(anchor, nameof(anchor));
            SourceDigest = DigestToken.Validate(sourceDigest, nameof(sourceDigest));
            AnchorDigest = DigestToken.Validate(anchorDigest, nameof(anchorDigest));
        }

        public string AuthorityId { get; }

        public string SourcePath { get; }

        public string Anchor { get; }

        public string SourceDigest { get; }

        public string AnchorDigest { get; }

        private static string RequireText(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Authority provenance text cannot be empty.", parameterName);
            }

            return value;
        }
    }

    public sealed class DesignFact
    {
        private readonly IReadOnlyDictionary<string, DesignValue> _fields;
        private readonly IReadOnlyList<DesignRelation> _relations;

        public DesignFact(
            string factId,
            string factType,
            IReadOnlyDictionary<string, DesignValue> fields,
            IEnumerable<DesignRelation>? relations,
            DesignAuthorityAnchor provenance)
        {
            FactId = DesignToken.Validate(factId, nameof(factId), 128);
            FactType = DesignToken.Validate(factType, nameof(factType), 128);
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var copiedFields = new SortedDictionary<string, DesignValue>(StringComparer.Ordinal);
            foreach (var pair in fields)
            {
                copiedFields.Add(DesignToken.Validate(pair.Key, nameof(fields), 128), pair.Value);
            }

            var copiedRelations = new List<DesignRelation>(relations ?? Array.Empty<DesignRelation>());
            copiedRelations.Sort((left, right) =>
            {
                var comparison = StringComparer.Ordinal.Compare(left.RelationType, right.RelationType);
                return comparison != 0 ? comparison : StringComparer.Ordinal.Compare(left.TargetFactId, right.TargetFactId);
            });

            _fields = new ReadOnlyDictionary<string, DesignValue>(copiedFields);
            _relations = copiedRelations.AsReadOnly();
            Provenance = provenance ?? throw new ArgumentNullException(nameof(provenance));
        }

        public string FactId { get; }

        public string FactType { get; }

        public IReadOnlyDictionary<string, DesignValue> Fields => _fields;

        public IReadOnlyList<DesignRelation> Relations => _relations;

        public DesignAuthorityAnchor Provenance { get; }
    }

    public interface IDesignAuthorityUniverse
    {
        IReadOnlyList<string> RequiredFactIds { get; }
    }

    public sealed class StaticDesignAuthorityUniverse : IDesignAuthorityUniverse
    {
        private readonly IReadOnlyList<string> _requiredFactIds;

        public StaticDesignAuthorityUniverse(IEnumerable<string> requiredFactIds)
        {
            if (requiredFactIds == null)
            {
                throw new ArgumentNullException(nameof(requiredFactIds));
            }

            var unique = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var id in requiredFactIds)
            {
                if (!unique.Add(DesignToken.Validate(id, nameof(requiredFactIds), 128)))
                {
                    throw new ArgumentException("The authority universe cannot contain duplicate fact ids.", nameof(requiredFactIds));
                }
            }

            if (unique.Count == 0)
            {
                throw new ArgumentException("The authority universe must contain at least one fact.", nameof(requiredFactIds));
            }

            _requiredFactIds = new List<string>(unique).AsReadOnly();
        }

        public IReadOnlyList<string> RequiredFactIds => _requiredFactIds;
    }

    public enum DesignAuthorityResolutionStatus
    {
        Found = 1,
        Missing = 2,
        Ambiguous = 3,
        Stale = 4
    }

    public sealed class DesignAuthorityReadResult
    {
        private DesignAuthorityReadResult(DesignAuthorityResolutionStatus status, DesignFact? fact, string detail)
        {
            Status = status;
            Fact = fact;
            Detail = detail ?? string.Empty;
        }

        public DesignAuthorityResolutionStatus Status { get; }

        public DesignFact? Fact { get; }

        public string Detail { get; }

        public static DesignAuthorityReadResult Found(DesignFact fact) => new DesignAuthorityReadResult(
            DesignAuthorityResolutionStatus.Found,
            fact ?? throw new ArgumentNullException(nameof(fact)),
            string.Empty);

        public static DesignAuthorityReadResult Failure(DesignAuthorityResolutionStatus status, string detail)
        {
            if (status == DesignAuthorityResolutionStatus.Found)
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }

            return new DesignAuthorityReadResult(status, null, detail);
        }
    }

    public interface IDesignAuthorityReader
    {
        DesignAuthorityReadResult Read(string factId);

        DesignAuthorityResolutionStatus Resolve(DesignAuthorityAnchor anchor);
    }

    public sealed class DesignProjectionVersion : IEquatable<DesignProjectionVersion>
    {
        public DesignProjectionVersion(int schemaVersion, string ruleVersion)
        {
            if (schemaVersion <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(schemaVersion));
            }

            SchemaVersion = schemaVersion;
            RuleVersion = DesignToken.Validate(ruleVersion, nameof(ruleVersion), 128);
        }

        public int SchemaVersion { get; }

        public string RuleVersion { get; }

        public bool Equals(DesignProjectionVersion? other) =>
            other != null && SchemaVersion == other.SchemaVersion && StringComparer.Ordinal.Equals(RuleVersion, other.RuleVersion);

        public override bool Equals(object? obj) => Equals(obj as DesignProjectionVersion);

        public override int GetHashCode()
        {
            unchecked
            {
                return (SchemaVersion * 397) ^ StringComparer.Ordinal.GetHashCode(RuleVersion);
            }
        }

        public override string ToString() => SchemaVersion.ToString(CultureInfo.InvariantCulture) + ":" + RuleVersion;
    }

    public sealed class AnchoredFactDefinition
    {
        private readonly IReadOnlyDictionary<string, DesignValue> _fields;
        private readonly IReadOnlyList<DesignRelation> _relations;

        public AnchoredFactDefinition(
            string factId,
            string factType,
            string sourceAnchorText,
            IReadOnlyDictionary<string, DesignValue> fields,
            IEnumerable<DesignRelation>? relations = null)
        {
            FactId = DesignToken.Validate(factId, nameof(factId), 128);
            FactType = DesignToken.Validate(factType, nameof(factType), 128);
            if (string.IsNullOrEmpty(sourceAnchorText))
            {
                throw new ArgumentException("Source anchor text cannot be empty.", nameof(sourceAnchorText));
            }

            SourceAnchorText = sourceAnchorText;
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            _fields = new ReadOnlyDictionary<string, DesignValue>(new Dictionary<string, DesignValue>(fields, StringComparer.Ordinal));
            _relations = new List<DesignRelation>(relations ?? Array.Empty<DesignRelation>()).AsReadOnly();
        }

        public string FactId { get; }

        public string FactType { get; }

        public string SourceAnchorText { get; }

        public IReadOnlyDictionary<string, DesignValue> Fields => _fields;

        public IReadOnlyList<DesignRelation> Relations => _relations;
    }

    internal static class DesignToken
    {
        public static string Validate(string value, string parameterName, int maximumLength)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.Length == 0 || value.Length > maximumLength)
            {
                throw new ArgumentException("Stable design tokens must be non-empty and within the declared length bound.", parameterName);
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
                    throw new ArgumentException("Stable design tokens use lowercase ASCII letters, digits, '.', '_' and '-' only.", parameterName);
                }
            }

            return value;
        }
    }

    internal static class DigestToken
    {
        public static string Validate(string value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.Length != 64)
            {
                throw new ArgumentException("Digests must be lowercase SHA-256 hex.", parameterName);
            }

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!(character >= '0' && character <= '9') && !(character >= 'a' && character <= 'f'))
                {
                    throw new ArgumentException("Digests must be lowercase SHA-256 hex.", parameterName);
                }
            }

            return value;
        }
    }
}
