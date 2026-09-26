using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Arkus.DesignWorld
{
    public sealed class H1CatalogueProjectionException : InvalidOperationException
    {
        public H1CatalogueProjectionException(string machineCode, string subjectId, string rule, string detail, params string[] sourcePaths)
            : base(detail)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            SubjectId = subjectId ?? string.Empty;
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            Detail = detail ?? throw new ArgumentNullException(nameof(detail));
            SourcePaths = new List<string>(sourcePaths ?? Array.Empty<string>()).AsReadOnly();
        }

        public string MachineCode { get; }
        public string SubjectId { get; }
        public string Rule { get; }
        public string Detail { get; }
        public IReadOnlyList<string> SourcePaths { get; }
    }

    public sealed class H1ProjectionSourceDescriptor
    {
        public H1ProjectionSourceDescriptor(string authorityId, string sourcePath, string acceptedBlobSha, string schemaId)
        {
            AuthorityId = Require(authorityId, nameof(authorityId));
            SourcePath = Require(sourcePath, nameof(sourcePath));
            AcceptedBlobSha = Require(acceptedBlobSha, nameof(acceptedBlobSha));
            SchemaId = Require(schemaId, nameof(schemaId));
        }

        public string AuthorityId { get; }
        public string SourcePath { get; }
        public string AcceptedBlobSha { get; }
        public string SchemaId { get; }

        private static string Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Manifest value cannot be empty.", name);
            return value;
        }
    }

    public static class H1ProjectionManifest
    {
        public const string AdapterId = "ctx-dw-h1-01-adapter-v1";
        public const string LifecycleId = "ctx-dw-h1-01-lifecycle-v1";
        public const string AcceptedH104CandidateSha = "8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5";
        public const string CataloguePath = "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json";
        public const string SourceAdoptionPath = "Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json";
        public const string CatalogueSchemaId = "arkus.h1-catalogue-mapping@1";
        public const string SourceAdoptionSchemaId = "arkus.h1-04-source-adoption@1";
        public const string ProjectIdentity = "arkus.unity-project@1:ArkusUnity";
        public const string DistributionMode = "external-readonly-local-source";
        public const string CatalogueBlobSha = "31d5ccc9dac335a3b49f0ec3cbb5007848146626";
        public const string SourceAdoptionBlobSha = "23e2a423c26903702e4035b600007ad35a926c58";

        private static readonly IReadOnlyList<H1ProjectionSourceDescriptor> ManifestSources =
            new List<H1ProjectionSourceDescriptor>
            {
                new H1ProjectionSourceDescriptor("h1-04-catalogue-mapping", CataloguePath, CatalogueBlobSha, CatalogueSchemaId),
                new H1ProjectionSourceDescriptor("h1-04-source-adoption", SourceAdoptionPath, SourceAdoptionBlobSha, SourceAdoptionSchemaId)
            }.AsReadOnly();

        private static readonly IReadOnlyList<string> Types = new List<string>
        {
            "h1-catalogue", "h1-catalogue-entry", "h1-source-adoption", "h1-source", "h1-source-slice"
        }.AsReadOnly();

        private static readonly IReadOnlyList<string> Relations = new List<string>
        {
            "declared-in", "adopted-from-source"
        }.AsReadOnly();

        public static IReadOnlyList<H1ProjectionSourceDescriptor> Sources => ManifestSources;
        public static IReadOnlyList<string> AdoptedEntityTypes => Types;
        public static IReadOnlyList<string> AdoptedRelations => Relations;
        public static DesignProjectionVersion CurrentProjectionVersion =>
            new DesignProjectionVersion(1, "ctx-dw-h1-01-v1");
    }

    public sealed class H1AcceptedAuthoritySources
    {
        private readonly IReadOnlyDictionary<string, string> _sources;

        public H1AcceptedAuthoritySources(string catalogueMappingJson, string sourceAdoptionJson)
        {
            _sources = new ReadOnlyDictionary<string, string>(
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    [H1ProjectionManifest.CataloguePath] = Require(catalogueMappingJson, nameof(catalogueMappingJson)),
                    [H1ProjectionManifest.SourceAdoptionPath] = Require(sourceAdoptionJson, nameof(sourceAdoptionJson))
                });
        }

        internal string Read(string path)
        {
            if (!_sources.TryGetValue(path, out var source))
            {
                throw new H1CatalogueProjectionException(
                    "h1.source_missing", path,
                    "every frozen H1-04 authority input must be supplied before projection",
                    "Required accepted H1-04 authority source is missing.", path);
            }
            return source;
        }

        private static string Require(string value, string name)
        {
            if (value == null) throw new ArgumentNullException(name);
            if (value.Length == 0) throw new ArgumentException("Accepted authority source cannot be empty.", name);
            return value;
        }
    }

    public sealed class H1CatalogueDataset
    {
        internal H1CatalogueDataset(DesignWorldProjection projection)
        {
            Projection = projection ?? throw new ArgumentNullException(nameof(projection));
        }

        public DesignWorldProjection Projection { get; }
        public string AdapterId => H1ProjectionManifest.AdapterId;
        public string LifecycleId => H1ProjectionManifest.LifecycleId;
        public string AcceptedH104CandidateSha => H1ProjectionManifest.AcceptedH104CandidateSha;
        public string ProjectionIdentity => H1ProjectionManifest.AdapterId + ":" + Projection.Digest;
    }

    internal interface IH1CatalogueSemanticOracle
    {
        H1CatalogueSemanticReport Validate(DesignWorldProjection projection, H1AcceptedAuthoritySources sources);
    }

    public sealed class H1DesignWorldProvider
    {
        private readonly bool _reverseEnumeration;
        private readonly IH1CatalogueSemanticOracle _oracle;

        public H1DesignWorldProvider(bool reverseEnumeration = false)
            : this(reverseEnumeration, new H1SourceAuthorityOracle())
        {
        }

        internal H1DesignWorldProvider(bool reverseEnumeration, IH1CatalogueSemanticOracle oracle)
        {
            _reverseEnumeration = reverseEnumeration;
            _oracle = oracle ?? throw new ArgumentNullException(nameof(oracle));
        }

        public H1CatalogueDataset BuildAndValidate(H1AcceptedAuthoritySources sources)
        {
            if (sources == null) throw new ArgumentNullException(nameof(sources));

            H1AcceptedAuthorityParser.ValidateAcceptedSourceBlobs(sources);
            var definitions = new H1AcceptedAuthorityParser().Parse(sources);
            if (_reverseEnumeration) definitions.Reverse();

            var universe = new StaticDesignAuthorityUniverse(definitions.Select(item => item.FactId));
            var reader = new H1MultiSourceAuthorityReader(definitions, sources);
            var version = H1ProjectionManifest.CurrentProjectionVersion;
            var projection = new DesignWorldProjector().Build(universe, reader, version);
            var generic = new DesignWorldProjectionValidator().Validate(projection, universe, reader, version);
            if (!generic.IsValid)
            {
                var first = generic.Issues
                    .OrderBy(issue => issue.FactId, StringComparer.Ordinal)
                    .ThenBy(issue => issue.MachineCode, StringComparer.Ordinal)
                    .First();
                throw new H1CatalogueProjectionException(
                    "h1.generic_projection_invalid", first.FactId,
                    "fresh H1 projection must satisfy accepted generic Design World guarantees",
                    first.MachineCode + ": " + first.Detail,
                    projection.Facts.Select(fact => fact.Provenance.SourcePath).Distinct(StringComparer.Ordinal).ToArray());
            }

            var semantic = _oracle.Validate(projection, sources);
            if (!semantic.IsValid)
            {
                var first = semantic.Issues.First();
                throw new H1CatalogueProjectionException(
                    first.MachineCode, first.FactId, first.Rule, first.Detail, first.SourcePaths.ToArray());
            }

            return new H1CatalogueDataset(projection);
        }
    }

    internal sealed class H1RecordDefinition
    {
        public H1RecordDefinition(
            string factId,
            string factType,
            string sourcePath,
            string sourceAnchor,
            IDictionary<string, DesignValue> fields,
            IEnumerable<DesignRelation>? relations = null)
        {
            FactId = factId;
            FactType = factType;
            SourcePath = sourcePath;
            SourceAnchor = sourceAnchor;
            Fields = new SortedDictionary<string, DesignValue>(fields, StringComparer.Ordinal);
            Relations = new List<DesignRelation>(relations ?? Array.Empty<DesignRelation>());
        }

        public string FactId { get; }
        public string FactType { get; }
        public string SourcePath { get; }
        public string SourceAnchor { get; }
        public SortedDictionary<string, DesignValue> Fields { get; }
        public List<DesignRelation> Relations { get; }
    }

    internal sealed class H1AcceptedAuthorityParser
    {
        private static readonly Regex CatalogueRootPattern = new Regex(
            @"""schemaId""\s*:\s*""(?<schema>[^""]+)""\s*,\s*""projectIdentity""\s*:\s*""(?<project>[^""]+)""",
            RegexOptions.CultureInvariant);

        private static readonly Regex CatalogueEntryPattern = new Regex(
            @"\{\s*""logicalId""\s*:\s*""(?<logicalId>[^""]+)""\s*,\s*""kind""\s*:\s*""(?<kind>[^""]+)""\s*,\s*""nativeGuid""\s*:\s*""(?<nativeGuid>[^""]*)""\s*,\s*""localFileId""\s*:\s*""(?<localFileId>[^""]+)""\s*,\s*""typeName""\s*:\s*""(?<typeName>[^""]+)""\s*,\s*""sourceId""\s*:\s*""(?<sourceId>[^""]+)""\s*,\s*""adoptionStatus""\s*:\s*""(?<adoptionStatus>[^""]+)""\s*,\s*""contentSha256""\s*:\s*""(?<contentSha256>(?:[0-9a-f]{64})?)""\s*\}",
            RegexOptions.CultureInvariant | RegexOptions.Singleline);

        private static readonly Regex AdoptionRootPattern = new Regex(
            @"""schemaId""\s*:\s*""(?<schema>[^""]+)""\s*,\s*""distributionMode""\s*:\s*""(?<mode>[^""]+)""",
            RegexOptions.CultureInvariant);

        private static readonly Regex SourcePattern = new Regex(
            @"\{\s*""sourceId""\s*:\s*""(?<sourceId>[^""]+)""\s*,\s*""originUrl""\s*:\s*""(?<originUrl>[^""]+)""\s*,\s*""distributionSha256""\s*:\s*""(?<distributionSha256>[0-9a-f]{64})""\s*,\s*""licenseId""\s*:\s*""(?<licenseId>[^""]+)""\s*,\s*""licenseSha256""\s*:\s*""(?<licenseSha256>[0-9a-f]{64})""\s*,\s*""commercialUse""\s*:\s*(?<commercialUse>true|false)\s*,\s*""noticeRequirement""\s*:\s*""(?<noticeRequirement>[^""]+)""\s*\}",
            RegexOptions.CultureInvariant | RegexOptions.Singleline);

        private static readonly Regex SlicePattern = new Regex(
            @"\{\s*""assetPath""\s*:\s*""(?<assetPath>[^""]+)""\s*,\s*""sourceId""\s*:\s*""(?<sourceId>[^""]+)""\s*,\s*""adoptionStatus""\s*:\s*""(?<adoptionStatus>[^""]+)""\s*,\s*""contentSha256""\s*:\s*""(?<contentSha256>[0-9a-f]{64})""\s*\}",
            RegexOptions.CultureInvariant | RegexOptions.Singleline);

        public static void ValidateAcceptedSourceBlobs(H1AcceptedAuthoritySources sources)
        {
            foreach (var descriptor in H1ProjectionManifest.Sources)
            {
                var actual = H1ProjectionText.GitBlobSha1(sources.Read(descriptor.SourcePath));
                if (!StringComparer.Ordinal.Equals(actual, descriptor.AcceptedBlobSha))
                {
                    throw new H1CatalogueProjectionException(
                        "h1.accepted_source_blob_mismatch", descriptor.SourcePath,
                        "the derived H1 projection is pinned to exact independently accepted H1-04 authority bytes",
                        "Accepted source blob changed from " + descriptor.AcceptedBlobSha + " to " + actual +
                        "; rebuild requires explicit review of the new H1 authority.",
                        descriptor.SourcePath);
                }
            }
        }

        public List<H1RecordDefinition> Parse(H1AcceptedAuthoritySources sources)
        {
            var catalogue = sources.Read(H1ProjectionManifest.CataloguePath);
            var adoption = sources.Read(H1ProjectionManifest.SourceAdoptionPath);
            var records = new List<H1RecordDefinition>();

            var catalogueRoot = RequireSingle(
                CatalogueRootPattern, catalogue, "h1.catalogue_root_shape", H1ProjectionManifest.CataloguePath);
            RequireEqual(
                H1ProjectionManifest.CatalogueSchemaId,
                catalogueRoot.Groups["schema"].Value,
                "h1.catalogue_schema_stale",
                H1ProjectionManifest.CataloguePath);
            RequireEqual(
                H1ProjectionManifest.ProjectIdentity,
                catalogueRoot.Groups["project"].Value,
                "h1.project_identity_stale",
                H1ProjectionManifest.CataloguePath);
            records.Add(new H1RecordDefinition(
                "h1.catalogue",
                "h1-catalogue",
                H1ProjectionManifest.CataloguePath,
                catalogueRoot.Value,
                Fields(
                    "schema-id", catalogueRoot.Groups["schema"].Value,
                    "project-identity", catalogueRoot.Groups["project"].Value,
                    "accepted-candidate-sha", H1ProjectionManifest.AcceptedH104CandidateSha,
                    "accepted-blob-sha", H1ProjectionManifest.CatalogueBlobSha,
                    "adapter-id", H1ProjectionManifest.AdapterId)));

            var adoptionRoot = RequireSingle(
                AdoptionRootPattern, adoption, "h1.source_adoption_root_shape", H1ProjectionManifest.SourceAdoptionPath);
            RequireEqual(
                H1ProjectionManifest.SourceAdoptionSchemaId,
                adoptionRoot.Groups["schema"].Value,
                "h1.source_adoption_schema_stale",
                H1ProjectionManifest.SourceAdoptionPath);
            RequireEqual(
                H1ProjectionManifest.DistributionMode,
                adoptionRoot.Groups["mode"].Value,
                "h1.distribution_mode_stale",
                H1ProjectionManifest.SourceAdoptionPath);
            records.Add(new H1RecordDefinition(
                "h1.source-adoption",
                "h1-source-adoption",
                H1ProjectionManifest.SourceAdoptionPath,
                adoptionRoot.Value,
                Fields(
                    "schema-id", adoptionRoot.Groups["schema"].Value,
                    "distribution-mode", adoptionRoot.Groups["mode"].Value,
                    "accepted-candidate-sha", H1ProjectionManifest.AcceptedH104CandidateSha,
                    "accepted-blob-sha", H1ProjectionManifest.SourceAdoptionBlobSha,
                    "adapter-id", H1ProjectionManifest.AdapterId)));

            var sourceIds = ParseSources(adoption, records);
            ParseCatalogue(catalogue, sourceIds, records);
            ParseSlices(adoption, sourceIds, records);
            return records;
        }

        private static HashSet<string> ParseSources(string adoption, ICollection<H1RecordDefinition> records)
        {
            var matches = SourcePattern.Matches(adoption).Cast<Match>().ToList();
            var declared = Regex.Matches(adoption, @"""originUrl""\s*:").Count;
            if (matches.Count == 0 || matches.Count != declared)
            {
                throw Shape(
                    "h1.source_universe_incomplete",
                    H1ProjectionManifest.SourceAdoptionPath,
                    "Adapter parsed " + matches.Count.ToString(CultureInfo.InvariantCulture) +
                    " of " + declared.ToString(CultureInfo.InvariantCulture) + " declared source records.");
            }

            var sourceIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var match in matches)
            {
                var sourceId = match.Groups["sourceId"].Value;
                if (!sourceIds.Add(sourceId))
                {
                    throw Shape(
                        "h1.source_identity_duplicate",
                        H1ProjectionManifest.SourceAdoptionPath,
                        "Duplicate accepted sourceId '" + sourceId + "'.");
                }

                records.Add(new H1RecordDefinition(
                    SourceFactId(sourceId),
                    "h1-source",
                    H1ProjectionManifest.SourceAdoptionPath,
                    match.Value,
                    Fields(
                        "source-id", sourceId,
                        "origin-url", match.Groups["originUrl"].Value,
                        "distribution-sha256", match.Groups["distributionSha256"].Value,
                        "license-id", match.Groups["licenseId"].Value,
                        "license-sha256", match.Groups["licenseSha256"].Value,
                        "commercial-use", match.Groups["commercialUse"].Value,
                        "notice-requirement", match.Groups["noticeRequirement"].Value),
                    new[] { new DesignRelation("declared-in", "h1.source-adoption") }));
            }
            return sourceIds;
        }

        private static void ParseCatalogue(
            string catalogue,
            ISet<string> sourceIds,
            ICollection<H1RecordDefinition> records)
        {
            var matches = CatalogueEntryPattern.Matches(catalogue).Cast<Match>().ToList();
            var declared = Regex.Matches(catalogue, @"""logicalId""\s*:").Count;
            if (matches.Count == 0 || matches.Count != declared)
            {
                throw Shape(
                    "h1.catalogue_universe_incomplete",
                    H1ProjectionManifest.CataloguePath,
                    "Adapter parsed " + matches.Count.ToString(CultureInfo.InvariantCulture) +
                    " of " + declared.ToString(CultureInfo.InvariantCulture) + " declared catalogue entries.");
            }

            var logicalIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var match in matches)
            {
                var logicalId = match.Groups["logicalId"].Value;
                if (!logicalIds.Add(logicalId))
                {
                    throw Shape(
                        "h1.catalogue_identity_duplicate",
                        H1ProjectionManifest.CataloguePath,
                        "Duplicate logicalId '" + logicalId + "'.");
                }

                var kind = match.Groups["kind"].Value;
                var nativeGuid = match.Groups["nativeGuid"].Value;
                var localFileId = match.Groups["localFileId"].Value;
                var contentSha = match.Groups["contentSha256"].Value;
                ValidateLocator(logicalId, kind, nativeGuid, localFileId, contentSha);

                var sourceId = match.Groups["sourceId"].Value;
                var relations = new List<DesignRelation>
                {
                    new DesignRelation("declared-in", "h1.catalogue")
                };
                if (sourceIds.Contains(sourceId))
                {
                    relations.Add(new DesignRelation("adopted-from-source", SourceFactId(sourceId)));
                }

                records.Add(new H1RecordDefinition(
                    logicalId,
                    "h1-catalogue-entry",
                    H1ProjectionManifest.CataloguePath,
                    match.Value,
                    Fields(
                        "logical-id", logicalId,
                        "kind", kind,
                        "native-guid", nativeGuid,
                        "local-file-id", localFileId,
                        "type-name", match.Groups["typeName"].Value,
                        "source-id", sourceId,
                        "adoption-status", match.Groups["adoptionStatus"].Value,
                        "content-sha256", contentSha),
                    relations));
            }
        }

        private static void ParseSlices(
            string adoption,
            ISet<string> sourceIds,
            ICollection<H1RecordDefinition> records)
        {
            var matches = SlicePattern.Matches(adoption).Cast<Match>().ToList();
            var declared = Regex.Matches(adoption, @"""assetPath""\s*:").Count;
            if (matches.Count == 0 || matches.Count != declared)
            {
                throw Shape(
                    "h1.source_slice_universe_incomplete",
                    H1ProjectionManifest.SourceAdoptionPath,
                    "Adapter parsed " + matches.Count.ToString(CultureInfo.InvariantCulture) +
                    " of " + declared.ToString(CultureInfo.InvariantCulture) + " declared source slices.");
            }

            var sliceIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var match in matches)
            {
                var assetPath = match.Groups["assetPath"].Value;
                var factId = SliceFactId(assetPath);
                if (!sliceIds.Add(factId))
                {
                    throw Shape(
                        "h1.source_slice_identity_duplicate",
                        H1ProjectionManifest.SourceAdoptionPath,
                        "Duplicate accepted source slice identity for '" + assetPath + "'.");
                }

                var sourceId = match.Groups["sourceId"].Value;
                if (!sourceIds.Contains(sourceId))
                {
                    throw Shape(
                        "h1.source_slice_orphan",
                        H1ProjectionManifest.SourceAdoptionPath,
                        "Source slice references undeclared sourceId '" + sourceId + "'.");
                }

                records.Add(new H1RecordDefinition(
                    factId,
                    "h1-source-slice",
                    H1ProjectionManifest.SourceAdoptionPath,
                    match.Value,
                    Fields(
                        "asset-path", assetPath,
                        "source-id", sourceId,
                        "adoption-status", match.Groups["adoptionStatus"].Value,
                        "content-sha256", match.Groups["contentSha256"].Value),
                    new[]
                    {
                        new DesignRelation("declared-in", "h1.source-adoption"),
                        new DesignRelation("adopted-from-source", SourceFactId(sourceId))
                    }));
            }
        }

        private static void ValidateLocator(
            string logicalId,
            string kind,
            string nativeGuid,
            string localFileId,
            string contentSha)
        {
            if (StringComparer.Ordinal.Equals(kind, "component-schema"))
            {
                if (nativeGuid.Length != 0 ||
                    !StringComparer.Ordinal.Equals(localFileId, "0") ||
                    contentSha.Length != 0)
                {
                    throw Shape(
                        "h1.component_schema_locator_stale",
                        H1ProjectionManifest.CataloguePath,
                        "Component-schema entry '" + logicalId +
                        "' must retain empty GUID/content and localFileId 0.");
                }
                return;
            }

            if (nativeGuid.Length != 32 || contentSha.Length != 64)
            {
                throw Shape(
                    "h1.native_locator_stale",
                    H1ProjectionManifest.CataloguePath,
                    "Asset-like entry '" + logicalId + "' lost its reviewed native locator or content fingerprint.");
            }
        }

        internal static string SourceFactId(string sourceId) => "h1.source." + sourceId;

        internal static string SliceFactId(string assetPath) =>
            "h1.slice." + DesignWorldEncoding.Sha256Hex(assetPath).Substring(0, 24);

        private static SortedDictionary<string, DesignValue> Fields(params string[] values)
        {
            if (values.Length % 2 != 0)
            {
                throw new ArgumentException("Fields require name/value pairs.", nameof(values));
            }

            var result = new SortedDictionary<string, DesignValue>(StringComparer.Ordinal);
            for (var index = 0; index < values.Length; index += 2)
            {
                result.Add(values[index], DesignValue.String(values[index + 1]));
            }
            return result;
        }

        private static Match RequireSingle(Regex pattern, string source, string code, string path)
        {
            var matches = pattern.Matches(source).Cast<Match>().ToList();
            if (matches.Count != 1)
            {
                throw Shape(
                    code,
                    path,
                    "Expected exactly one reviewed root shape but found " +
                    matches.Count.ToString(CultureInfo.InvariantCulture) + ".");
            }
            return matches[0];
        }

        private static void RequireEqual(string expected, string actual, string code, string path)
        {
            if (!StringComparer.Ordinal.Equals(expected, actual))
            {
                throw Shape(code, path, "Expected '" + expected + "' but found '" + actual + "'.");
            }
        }

        private static H1CatalogueProjectionException Shape(string code, string path, string detail) =>
            new H1CatalogueProjectionException(
                code,
                path,
                "accepted H1-04 authority shape must remain exact until explicitly re-adopted",
                detail,
                path);
    }

    internal sealed class H1MultiSourceAuthorityReader : IDesignAuthorityReader
    {
        private readonly IReadOnlyDictionary<string, DesignFact> _facts;
        private readonly IReadOnlyDictionary<string, SourceState> _sources;

        public H1MultiSourceAuthorityReader(
            IEnumerable<H1RecordDefinition> definitions,
            H1AcceptedAuthoritySources sources)
        {
            var states = new Dictionary<string, SourceState>(StringComparer.Ordinal);
            foreach (var descriptor in H1ProjectionManifest.Sources)
            {
                states.Add(
                    descriptor.SourcePath,
                    new SourceState(descriptor.AuthorityId, sources.Read(descriptor.SourcePath)));
            }
            _sources = new ReadOnlyDictionary<string, SourceState>(states);

            var facts = new SortedDictionary<string, DesignFact>(StringComparer.Ordinal);
            foreach (var definition in definitions)
            {
                if (facts.ContainsKey(definition.FactId))
                {
                    throw new H1CatalogueProjectionException(
                        "h1.identity_duplicate",
                        definition.FactId,
                        "every projected H1 record identity must be unique",
                        "Adapter produced a duplicate projected identity.",
                        definition.SourcePath);
                }

                if (!states.TryGetValue(definition.SourcePath, out var state))
                {
                    throw new H1CatalogueProjectionException(
                        "h1.source_unreviewed",
                        definition.FactId,
                        "H1 adapter may project only frozen accepted H1-04 authority sources",
                        "Record points at an undeclared authority path.",
                        definition.SourcePath);
                }

                var occurrences = H1ProjectionText.CountOccurrences(state.Text, definition.SourceAnchor);
                if (occurrences != 1)
                {
                    throw new H1CatalogueProjectionException(
                        occurrences == 0
                            ? "h1.provenance_anchor_missing"
                            : "h1.provenance_anchor_ambiguous",
                        definition.FactId,
                        "every projected H1 record must source-open to one unique accepted authority anchor",
                        "Source anchor occurrence count is " +
                        occurrences.ToString(CultureInfo.InvariantCulture) + ".",
                        definition.SourcePath);
                }

                var anchor = new DesignAuthorityAnchor(
                    state.AuthorityId,
                    definition.SourcePath,
                    definition.SourceAnchor,
                    state.SourceDigest,
                    DesignWorldEncoding.Sha256Hex(definition.SourceAnchor));
                facts.Add(
                    definition.FactId,
                    new DesignFact(
                        definition.FactId,
                        definition.FactType,
                        definition.Fields,
                        definition.Relations,
                        anchor));
            }
            _facts = new ReadOnlyDictionary<string, DesignFact>(facts);
        }

        public DesignAuthorityReadResult Read(string factId)
        {
            if (!_facts.TryGetValue(factId, out var fact))
            {
                return DesignAuthorityReadResult.Failure(
                    DesignAuthorityResolutionStatus.Missing,
                    "No H1 authority record exists for the independently required identity.");
            }
            return DesignAuthorityReadResult.Found(fact);
        }

        public DesignAuthorityResolutionStatus Resolve(DesignAuthorityAnchor anchor)
        {
            if (anchor == null) throw new ArgumentNullException(nameof(anchor));
            if (!_sources.TryGetValue(anchor.SourcePath, out var state))
            {
                return DesignAuthorityResolutionStatus.Missing;
            }

            if (!StringComparer.Ordinal.Equals(anchor.AuthorityId, state.AuthorityId))
            {
                return DesignAuthorityResolutionStatus.Missing;
            }

            var count = H1ProjectionText.CountOccurrences(state.Text, anchor.Anchor);
            if (count == 0) return DesignAuthorityResolutionStatus.Missing;
            if (count > 1) return DesignAuthorityResolutionStatus.Ambiguous;
            if (!StringComparer.Ordinal.Equals(anchor.SourceDigest, state.SourceDigest) ||
                !StringComparer.Ordinal.Equals(
                    anchor.AnchorDigest,
                    DesignWorldEncoding.Sha256Hex(anchor.Anchor)))
            {
                return DesignAuthorityResolutionStatus.Stale;
            }
            return DesignAuthorityResolutionStatus.Found;
        }

        private sealed class SourceState
        {
            public SourceState(string authorityId, string text)
            {
                AuthorityId = authorityId;
                Text = text;
                SourceDigest = DesignWorldEncoding.Sha256Hex(text);
            }

            public string AuthorityId { get; }
            public string Text { get; }
            public string SourceDigest { get; }
        }
    }

    internal static class H1ProjectionText
    {
        public static int CountOccurrences(string source, string value)
        {
            var count = 0;
            var start = 0;
            while (start <= source.Length - value.Length)
            {
                var index = source.IndexOf(value, start, StringComparison.Ordinal);
                if (index < 0) break;
                count++;
                start = index + value.Length;
            }
            return count;
        }

        public static string GitBlobSha1(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var content = Encoding.UTF8.GetBytes(text);
            var header = Encoding.UTF8.GetBytes(
                "blob " + content.Length.ToString(CultureInfo.InvariantCulture) + "\0");
            var bytes = new byte[header.Length + content.Length];
            Buffer.BlockCopy(header, 0, bytes, 0, header.Length);
            Buffer.BlockCopy(content, 0, bytes, header.Length, content.Length);
            using (var sha = SHA1.Create())
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
    }
}
