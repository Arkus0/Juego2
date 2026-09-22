using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Arkus.Game.World;

namespace Arkus.DesignWorld
{
    public sealed class DesignWorldProjectionException : InvalidOperationException
    {
        public DesignWorldProjectionException(string machineCode, string message)
            : base(message)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
        }

        public string MachineCode { get; }
    }

    public sealed class AnchoredTextAuthorityReader : IDesignAuthorityReader
    {
        private readonly string _authorityId;
        private readonly string _sourcePath;
        private readonly string _sourceText;
        private readonly string _sourceDigest;
        private readonly IReadOnlyDictionary<string, AnchoredFactDefinition> _definitions;

        public AnchoredTextAuthorityReader(
            string authorityId,
            string sourcePath,
            string sourceText,
            IEnumerable<AnchoredFactDefinition> definitions)
        {
            _authorityId = DesignToken.Validate(authorityId, nameof(authorityId), 128);
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new ArgumentException("Source path cannot be empty.", nameof(sourcePath));
            }

            _sourcePath = sourcePath;
            _sourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
            _sourceDigest = DesignWorldEncoding.Sha256Hex(_sourceText);
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            var byId = new SortedDictionary<string, AnchoredFactDefinition>(StringComparer.Ordinal);
            foreach (var definition in definitions)
            {
                if (definition == null)
                {
                    throw new ArgumentException("Authority definitions cannot contain null entries.", nameof(definitions));
                }

                if (byId.ContainsKey(definition.FactId))
                {
                    throw new ArgumentException("Authority definitions cannot contain duplicate fact ids.", nameof(definitions));
                }

                byId.Add(definition.FactId, definition);
            }

            _definitions = new ReadOnlyDictionary<string, AnchoredFactDefinition>(byId);
        }

        public string SourceDigest => _sourceDigest;

        public DesignAuthorityReadResult Read(string factId)
        {
            factId = DesignToken.Validate(factId, nameof(factId), 128);
            if (!_definitions.TryGetValue(factId, out var definition))
            {
                return DesignAuthorityReadResult.Failure(
                    DesignAuthorityResolutionStatus.Missing,
                    "No authority definition exists for the required fact id.");
            }

            var occurrences = CountOccurrences(_sourceText, definition.SourceAnchorText);
            if (occurrences == 0)
            {
                return DesignAuthorityReadResult.Failure(
                    DesignAuthorityResolutionStatus.Missing,
                    "The declared authority anchor is absent from the source bytes.");
            }

            if (occurrences > 1)
            {
                return DesignAuthorityReadResult.Failure(
                    DesignAuthorityResolutionStatus.Ambiguous,
                    "The declared authority anchor occurs more than once in the source bytes.");
            }

            var provenance = new DesignAuthorityAnchor(
                _authorityId,
                _sourcePath,
                definition.SourceAnchorText,
                _sourceDigest,
                DesignWorldEncoding.Sha256Hex(definition.SourceAnchorText));
            return DesignAuthorityReadResult.Found(new DesignFact(
                definition.FactId,
                definition.FactType,
                definition.Fields,
                definition.Relations,
                provenance));
        }

        public DesignAuthorityResolutionStatus Resolve(DesignAuthorityAnchor anchor)
        {
            if (anchor == null)
            {
                throw new ArgumentNullException(nameof(anchor));
            }

            if (!StringComparer.Ordinal.Equals(anchor.AuthorityId, _authorityId) ||
                !StringComparer.Ordinal.Equals(anchor.SourcePath, _sourcePath))
            {
                return DesignAuthorityResolutionStatus.Missing;
            }

            var occurrences = CountOccurrences(_sourceText, anchor.Anchor);
            if (occurrences == 0)
            {
                return DesignAuthorityResolutionStatus.Missing;
            }

            if (occurrences > 1)
            {
                return DesignAuthorityResolutionStatus.Ambiguous;
            }

            if (!StringComparer.Ordinal.Equals(anchor.SourceDigest, _sourceDigest) ||
                !StringComparer.Ordinal.Equals(anchor.AnchorDigest, DesignWorldEncoding.Sha256Hex(anchor.Anchor)))
            {
                return DesignAuthorityResolutionStatus.Stale;
            }

            return DesignAuthorityResolutionStatus.Found;
        }

        private static int CountOccurrences(string source, string value)
        {
            var count = 0;
            var start = 0;
            while (start <= source.Length - value.Length)
            {
                var index = source.IndexOf(value, start, StringComparison.Ordinal);
                if (index < 0)
                {
                    break;
                }

                count++;
                start = index + value.Length;
            }

            return count;
        }
    }

    public sealed class DesignWorldProjection
    {
        private readonly IReadOnlyList<DesignFact> _facts;
        private readonly IReadOnlyList<string> _factIds;
        private readonly IReadOnlyDictionary<string, string> _factFingerprints;

        internal DesignWorldProjection(
            DesignProjectionVersion version,
            IEnumerable<DesignFact> facts,
            WorldState worldState,
            string normalizedRepresentation,
            string digest,
            IReadOnlyDictionary<string, string> factFingerprints)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
            if (facts == null)
            {
                throw new ArgumentNullException(nameof(facts));
            }

            var ordered = new List<DesignFact>(facts);
            ordered.Sort((left, right) => StringComparer.Ordinal.Compare(left.FactId, right.FactId));
            _facts = ordered.AsReadOnly();
            var ids = new List<string>(ordered.Count);
            for (var index = 0; index < ordered.Count; index++)
            {
                ids.Add(ordered[index].FactId);
            }

            _factIds = ids.AsReadOnly();
            WorldState = worldState ?? throw new ArgumentNullException(nameof(worldState));
            NormalizedRepresentation = normalizedRepresentation ?? throw new ArgumentNullException(nameof(normalizedRepresentation));
            Digest = DigestToken.Validate(digest, nameof(digest));
            _factFingerprints = factFingerprints ?? throw new ArgumentNullException(nameof(factFingerprints));
        }

        public DesignProjectionVersion Version { get; }

        public IReadOnlyList<DesignFact> Facts => _facts;

        public IReadOnlyList<string> FactIds => _factIds;

        public WorldState WorldState { get; }

        public string NormalizedRepresentation { get; }

        public string Digest { get; }

        internal IReadOnlyDictionary<string, string> FactFingerprints => _factFingerprints;
    }

    public sealed class DesignWorldProjector
    {
        public const string ExtensionOwner = "arkus.designworld";
        public const int ExtensionSchemaVersion = 1;
        public const string GenericWorldType = "dw.fact";
        public const string WorldIdentity = "design-world";

        public DesignWorldProjection Build(
            IDesignAuthorityUniverse universe,
            IDesignAuthorityReader reader,
            DesignProjectionVersion version)
        {
            if (universe == null)
            {
                throw new ArgumentNullException(nameof(universe));
            }

            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            var expectedIds = ValidateUniverse(universe.RequiredFactIds);
            var expectedSet = new HashSet<string>(expectedIds, StringComparer.Ordinal);
            var facts = new List<DesignFact>(expectedIds.Count);
            for (var index = 0; index < expectedIds.Count; index++)
            {
                var factId = expectedIds[index];
                var result = reader.Read(factId);
                if (result.Status != DesignAuthorityResolutionStatus.Found || result.Fact == null)
                {
                    throw new DesignWorldProjectionException(
                        MachineCode(result.Status),
                        "Authority fact '" + factId + "' could not be projected: " + result.Detail);
                }

                if (!StringComparer.Ordinal.Equals(result.Fact.FactId, factId))
                {
                    throw new DesignWorldProjectionException(
                        "dw.authority_identity_mismatch",
                        "The authority reader returned a different fact identity than the independently required universe id.");
                }

                for (var relationIndex = 0; relationIndex < result.Fact.Relations.Count; relationIndex++)
                {
                    var relation = result.Fact.Relations[relationIndex];
                    if (!expectedSet.Contains(relation.TargetFactId))
                    {
                        throw new DesignWorldProjectionException(
                            "dw.relation_target_outside_universe",
                            "A projected relation targets a fact outside the independently declared authority universe.");
                    }
                }

                facts.Add(result.Fact);
            }

            var normalized = DesignWorldEncoding.Normalize(version, facts);
            var digest = DesignWorldEncoding.Sha256Hex(normalized);
            var fingerprints = DesignWorldEncoding.FingerprintFacts(facts);
            var state = BuildWorldState(version, facts);
            return new DesignWorldProjection(version, facts, state, normalized, digest, fingerprints);
        }

        private static IReadOnlyList<string> ValidateUniverse(IReadOnlyList<string> ids)
        {
            if (ids == null)
            {
                throw new DesignWorldProjectionException("dw.universe_missing", "The projection requires an independently declared fact universe.");
            }

            var unique = new SortedSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < ids.Count; index++)
            {
                var id = DesignToken.Validate(ids[index], nameof(ids), 128);
                if (!unique.Add(id))
                {
                    throw new DesignWorldProjectionException("dw.universe_duplicate", "The independently declared authority universe contains a duplicate fact id.");
                }
            }

            if (unique.Count == 0)
            {
                throw new DesignWorldProjectionException("dw.universe_empty", "The projection cannot validate completeness against an empty authority universe.");
            }

            return new List<string>(unique).AsReadOnly();
        }

        private static WorldState BuildWorldState(DesignProjectionVersion version, IReadOnlyList<DesignFact> facts)
        {
            var objects = new List<WorldObject>(facts.Count);
            var extensions = new List<WorldExtensionData>(facts.Count);
            for (var index = 0; index < facts.Count; index++)
            {
                var fact = facts[index];
                var references = new List<WorldReference>(fact.Relations.Count);
                for (var relationIndex = 0; relationIndex < fact.Relations.Count; relationIndex++)
                {
                    var relation = fact.Relations[relationIndex];
                    references.Add(new WorldReference(
                        new WorldReferenceKind(relation.RelationType),
                        new WorldObjectId(relation.TargetFactId)));
                }

                var id = new WorldObjectId(fact.FactId);
                objects.Add(new WorldObject(id, new WorldTypeId(GenericWorldType), references: references));
                extensions.Add(new WorldExtensionData(
                    ExtensionOwner,
                    ExtensionSchemaVersion,
                    Encoding.UTF8.GetBytes(DesignWorldEncoding.NormalizeFact(version, fact)),
                    id,
                    references));
            }

            return new WorldState(new WorldId(WorldIdentity), 0L, objects, extensions);
        }

        private static string MachineCode(DesignAuthorityResolutionStatus status)
        {
            switch (status)
            {
                case DesignAuthorityResolutionStatus.Missing:
                    return "dw.provenance_missing";
                case DesignAuthorityResolutionStatus.Ambiguous:
                    return "dw.provenance_ambiguous";
                case DesignAuthorityResolutionStatus.Stale:
                    return "dw.provenance_stale";
                default:
                    return "dw.authority_unresolved";
            }
        }
    }

    public sealed class DesignWorldValidationIssue
    {
        public DesignWorldValidationIssue(string machineCode, string factId, string detail)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            FactId = factId ?? string.Empty;
            Detail = detail ?? string.Empty;
        }

        public string MachineCode { get; }

        public string FactId { get; }

        public string Detail { get; }
    }

    public sealed class DesignWorldValidationReport
    {
        internal DesignWorldValidationReport(IEnumerable<DesignWorldValidationIssue> issues)
        {
            Issues = new List<DesignWorldValidationIssue>(issues ?? throw new ArgumentNullException(nameof(issues))).AsReadOnly();
        }

        public IReadOnlyList<DesignWorldValidationIssue> Issues { get; }

        public bool IsValid => Issues.Count == 0;
    }

    public sealed class DesignWorldProjectionValidator
    {
        public DesignWorldValidationReport Validate(
            DesignWorldProjection projection,
            IDesignAuthorityUniverse universe,
            IDesignAuthorityReader reader,
            DesignProjectionVersion expectedVersion)
        {
            if (projection == null)
            {
                throw new ArgumentNullException(nameof(projection));
            }

            if (universe == null)
            {
                throw new ArgumentNullException(nameof(universe));
            }

            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (expectedVersion == null)
            {
                throw new ArgumentNullException(nameof(expectedVersion));
            }

            var issues = new List<DesignWorldValidationIssue>();
            var versionIsCurrent = projection.Version.Equals(expectedVersion);
            if (!versionIsCurrent)
            {
                issues.Add(new DesignWorldValidationIssue(
                    "dw.projection_version_stale",
                    string.Empty,
                    "Cached derived state was built with a different schema/rule version."));
            }

            var expected = new SortedSet<string>(universe.RequiredFactIds, StringComparer.Ordinal);
            var actual = new SortedSet<string>(projection.FactIds, StringComparer.Ordinal);
            foreach (var id in expected)
            {
                if (!actual.Contains(id))
                {
                    issues.Add(new DesignWorldValidationIssue(
                        "dw.projection_fact_missing",
                        id,
                        "A fact from the independently declared authority universe is absent from the derived projection."));
                }
            }

            foreach (var id in actual)
            {
                if (!expected.Contains(id))
                {
                    issues.Add(new DesignWorldValidationIssue(
                        "dw.projection_fact_unexpected",
                        id,
                        "Derived state contains a fact outside the independently declared authority universe."));
                }
            }

            var provenanceIsCurrent = true;
            for (var index = 0; index < projection.Facts.Count; index++)
            {
                var fact = projection.Facts[index];
                var status = reader.Resolve(fact.Provenance);
                if (status != DesignAuthorityResolutionStatus.Found)
                {
                    provenanceIsCurrent = false;
                    issues.Add(new DesignWorldValidationIssue(
                        MachineCode(status),
                        fact.FactId,
                        "Projected provenance no longer resolves uniquely to the accepted authority bytes."));
                }
            }

            var normalized = DesignWorldEncoding.Normalize(projection.Version, projection.Facts);
            var digest = DesignWorldEncoding.Sha256Hex(normalized);
            if (!StringComparer.Ordinal.Equals(normalized, projection.NormalizedRepresentation) ||
                !StringComparer.Ordinal.Equals(digest, projection.Digest))
            {
                issues.Add(new DesignWorldValidationIssue(
                    "dw.normalization_mismatch",
                    string.Empty,
                    "The stored normalized projection/digest does not match the projected facts."));
            }

            if (versionIsCurrent && provenanceIsCurrent)
            {
                try
                {
                    var rebuilt = new DesignWorldProjector().Build(universe, reader, expectedVersion);
                    if (!StringComparer.Ordinal.Equals(rebuilt.NormalizedRepresentation, projection.NormalizedRepresentation) ||
                        !StringComparer.Ordinal.Equals(rebuilt.Digest, projection.Digest))
                    {
                        issues.Add(new DesignWorldValidationIssue(
                            "dw.projection_rules_stale",
                            string.Empty,
                            "Cached derived state does not match a fresh rebuild using the current effective projection rules."));
                    }
                }
                catch (DesignWorldProjectionException error)
                {
                    issues.Add(new DesignWorldValidationIssue(
                        error.MachineCode,
                        string.Empty,
                        "A fresh rebuild using the current effective projection rules failed: " + error.Message));
                }
            }

            ValidateWorldSurface(projection, issues);
            return new DesignWorldValidationReport(issues);
        }

        private static void ValidateWorldSurface(DesignWorldProjection projection, ICollection<DesignWorldValidationIssue> issues)
        {
            var objectIds = new SortedSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < projection.WorldState.Objects.Count; index++)
            {
                var current = projection.WorldState.Objects[index];
                objectIds.Add(current.Id.Value);
                if (!StringComparer.Ordinal.Equals(current.TypeId.Value, DesignWorldProjector.GenericWorldType))
                {
                    issues.Add(new DesignWorldValidationIssue(
                        "dw.h0_surface_type_changed",
                        current.Id.Value,
                        "The Design World projection must remain on the generic H0 world-object surface."));
                }
            }

            if (!objectIds.SetEquals(projection.FactIds))
            {
                issues.Add(new DesignWorldValidationIssue(
                    "dw.h0_surface_incomplete",
                    string.Empty,
                    "The H0 world-object projection does not exactly match the projected fact identities."));
            }

            var extensionSubjects = new SortedSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < projection.WorldState.Extensions.Count; index++)
            {
                var extension = projection.WorldState.Extensions[index];
                if (!StringComparer.Ordinal.Equals(extension.Owner, DesignWorldProjector.ExtensionOwner) ||
                    extension.SchemaVersion != DesignWorldProjector.ExtensionSchemaVersion ||
                    !extension.SubjectId.HasValue)
                {
                    issues.Add(new DesignWorldValidationIssue(
                        "dw.h0_extension_contract_mismatch",
                        extension.SubjectId.HasValue ? extension.SubjectId.Value.Value : string.Empty,
                        "A Design World extension does not use the owned generic extension envelope."));
                    continue;
                }

                extensionSubjects.Add(extension.SubjectId.Value.Value);
            }

            if (!extensionSubjects.SetEquals(projection.FactIds))
            {
                issues.Add(new DesignWorldValidationIssue(
                    "dw.h0_extension_incomplete",
                    string.Empty,
                    "The H0 extension projection does not exactly match the projected fact identities."));
            }
        }

        private static string MachineCode(DesignAuthorityResolutionStatus status)
        {
            switch (status)
            {
                case DesignAuthorityResolutionStatus.Missing:
                    return "dw.provenance_missing";
                case DesignAuthorityResolutionStatus.Ambiguous:
                    return "dw.provenance_ambiguous";
                case DesignAuthorityResolutionStatus.Stale:
                    return "dw.provenance_stale";
                default:
                    return "dw.provenance_invalid";
            }
        }
    }

    public sealed class DesignWorldProjectionDiff
    {
        private DesignWorldProjectionDiff(
            IEnumerable<string> added,
            IEnumerable<string> removed,
            IEnumerable<string> changed)
        {
            AddedFactIds = new List<string>(added).AsReadOnly();
            RemovedFactIds = new List<string>(removed).AsReadOnly();
            ChangedFactIds = new List<string>(changed).AsReadOnly();
        }

        public IReadOnlyList<string> AddedFactIds { get; }

        public IReadOnlyList<string> RemovedFactIds { get; }

        public IReadOnlyList<string> ChangedFactIds { get; }

        public bool IsEmpty => AddedFactIds.Count == 0 && RemovedFactIds.Count == 0 && ChangedFactIds.Count == 0;

        public static DesignWorldProjectionDiff Compare(DesignWorldProjection left, DesignWorldProjection right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var added = new List<string>();
            var removed = new List<string>();
            var changed = new List<string>();
            foreach (var pair in left.FactFingerprints)
            {
                if (!right.FactFingerprints.TryGetValue(pair.Key, out var rightFingerprint))
                {
                    removed.Add(pair.Key);
                }
                else if (!StringComparer.Ordinal.Equals(pair.Value, rightFingerprint))
                {
                    changed.Add(pair.Key);
                }
            }

            foreach (var pair in right.FactFingerprints)
            {
                if (!left.FactFingerprints.ContainsKey(pair.Key))
                {
                    added.Add(pair.Key);
                }
            }

            added.Sort(StringComparer.Ordinal);
            removed.Sort(StringComparer.Ordinal);
            changed.Sort(StringComparer.Ordinal);
            return new DesignWorldProjectionDiff(added, removed, changed);
        }
    }

    internal static class DesignWorldEncoding
    {
        public static string Normalize(DesignProjectionVersion version, IReadOnlyList<DesignFact> facts)
        {
            var ordered = new List<DesignFact>(facts);
            ordered.Sort((left, right) => StringComparer.Ordinal.Compare(left.FactId, right.FactId));
            var builder = new StringBuilder();
            Append(builder, "format", "arkus-design-world-v1");
            Append(builder, "schema", version.SchemaVersion.ToString(CultureInfo.InvariantCulture));
            Append(builder, "rule", version.RuleVersion);
            for (var index = 0; index < ordered.Count; index++)
            {
                Append(builder, "fact", NormalizeFact(version, ordered[index]));
            }

            return builder.ToString();
        }

        public static string NormalizeFact(DesignProjectionVersion version, DesignFact fact)
        {
            var builder = new StringBuilder();
            Append(builder, "schema", version.SchemaVersion.ToString(CultureInfo.InvariantCulture));
            Append(builder, "rule", version.RuleVersion);
            Append(builder, "id", fact.FactId);
            Append(builder, "type", fact.FactType);
            foreach (var pair in fact.Fields)
            {
                Append(builder, "field-name", pair.Key);
                Append(builder, "field-kind", ((int)pair.Value.Kind).ToString(CultureInfo.InvariantCulture));
                Append(builder, "field-value", pair.Value.CanonicalValue);
            }

            for (var index = 0; index < fact.Relations.Count; index++)
            {
                Append(builder, "relation-type", fact.Relations[index].RelationType);
                Append(builder, "relation-target", fact.Relations[index].TargetFactId);
            }

            Append(builder, "authority", fact.Provenance.AuthorityId);
            Append(builder, "source-path", fact.Provenance.SourcePath);
            Append(builder, "anchor", fact.Provenance.Anchor);
            Append(builder, "source-digest", fact.Provenance.SourceDigest);
            Append(builder, "anchor-digest", fact.Provenance.AnchorDigest);
            return builder.ToString();
        }

        public static IReadOnlyDictionary<string, string> FingerprintFacts(IReadOnlyList<DesignFact> facts)
        {
            var result = new SortedDictionary<string, string>(StringComparer.Ordinal);
            var neutralVersion = new DesignProjectionVersion(1, "fact-fingerprint");
            for (var index = 0; index < facts.Count; index++)
            {
                result.Add(facts[index].FactId, Sha256Hex(NormalizeFact(neutralVersion, facts[index])));
            }

            return new ReadOnlyDictionary<string, string>(result);
        }

        public static string Sha256Hex(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value ?? throw new ArgumentNullException(nameof(value)));
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);
                for (var index = 0; index < hash.Length; index++)
                {
                    builder.Append(hash[index].ToString("x2", CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }

        private static void Append(StringBuilder builder, string label, string value)
        {
            builder.Append(label);
            builder.Append('=');
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(value);
            builder.Append('\n');
        }
    }
}
