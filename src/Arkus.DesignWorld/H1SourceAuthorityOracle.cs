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
    public sealed class H1CatalogueSemanticIssue
    {
        public H1CatalogueSemanticIssue(string machineCode, string factId, string rule, string detail, IEnumerable<string> sourcePaths)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            FactId = factId ?? string.Empty;
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            Detail = detail ?? throw new ArgumentNullException(nameof(detail));
            SourcePaths = new List<string>(sourcePaths ?? Array.Empty<string>()).AsReadOnly();
        }

        public string MachineCode { get; }
        public string FactId { get; }
        public string Rule { get; }
        public string Detail { get; }
        public IReadOnlyList<string> SourcePaths { get; }
    }

    public sealed class H1CatalogueSemanticReport
    {
        internal H1CatalogueSemanticReport(IEnumerable<H1CatalogueSemanticIssue> issues)
        {
            Issues = new List<H1CatalogueSemanticIssue>(issues ?? throw new ArgumentNullException(nameof(issues)))
                .OrderBy(issue => issue.FactId, StringComparer.Ordinal)
                .ThenBy(issue => issue.MachineCode, StringComparer.Ordinal)
                .ThenBy(issue => issue.Detail, StringComparer.Ordinal)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<H1CatalogueSemanticIssue> Issues { get; }
        public bool IsValid => Issues.Count == 0;
    }

    /// <summary>
    /// Independent H1-04 authority-side completeness oracle. It reconstructs the expected
    /// universe directly from frozen authority bytes and intentionally does not consume the
    /// production H1 parser, production record definitions or projection-derived counts.
    /// </summary>
    public sealed class H1SourceAuthorityOracle : IH1CatalogueSemanticOracle
    {
        public H1CatalogueSemanticReport Validate(DesignWorldProjection projection, H1AcceptedAuthoritySources sources)
        {
            if (projection == null) throw new ArgumentNullException(nameof(projection));
            if (sources == null) throw new ArgumentNullException(nameof(sources));

            IReadOnlyDictionary<string, OracleFact> expected;
            try
            {
                expected = new OracleBuilder().Build(sources);
            }
            catch (OracleSourceException error)
            {
                return new H1CatalogueSemanticReport(new[]
                {
                    new H1CatalogueSemanticIssue(
                        error.MachineCode,
                        error.SubjectId,
                        "the H1 lifecycle oracle must independently reconstruct the frozen accepted H1-04 authority universe",
                        error.Message,
                        new[] { error.SourcePath })
                });
            }

            var issues = new List<H1CatalogueSemanticIssue>();
            var actual = new SortedDictionary<string, DesignFact>(StringComparer.Ordinal);
            foreach (var fact in projection.Facts)
            {
                if (actual.ContainsKey(fact.FactId))
                {
                    issues.Add(Issue(
                        "h1.semantic_identity_duplicate",
                        fact.FactId,
                        "each H1 authority record must occur exactly once",
                        "Projected H1 surface contains a duplicate identity.",
                        fact.Provenance.SourcePath));
                    continue;
                }

                actual.Add(fact.FactId, fact);
            }

            foreach (var pair in expected)
            {
                if (!actual.TryGetValue(pair.Key, out var fact))
                {
                    issues.Add(Issue(
                        "h1.semantic_fact_missing",
                        pair.Key,
                        "every independently enumerated accepted H1-04 record must survive projection",
                        "Expected authority-side H1 record is absent from the projection.",
                        pair.Value.SourcePath));
                    continue;
                }

                Compare(pair.Value, fact, issues);
            }

            foreach (var pair in actual)
            {
                if (!expected.ContainsKey(pair.Key))
                {
                    issues.Add(Issue(
                        "h1.semantic_fact_unexpected",
                        pair.Key,
                        "the H1 projection may contain only records independently reconstructed from frozen H1-04 authority",
                        "Projected H1 record is outside the independent source-side universe.",
                        pair.Value.Provenance.SourcePath));
                }
            }

            return new H1CatalogueSemanticReport(issues);
        }

        private static void Compare(OracleFact expected, DesignFact actual, ICollection<H1CatalogueSemanticIssue> issues)
        {
            if (!StringComparer.Ordinal.Equals(expected.FactType, actual.FactType))
            {
                issues.Add(Issue(
                    "h1.semantic_type_mismatch",
                    expected.FactId,
                    "H1 adapter types are part of the versioned projection schema",
                    "Expected type '" + expected.FactType + "' but projected '" + actual.FactType + "'.",
                    expected.SourcePath));
            }

            var actualFields = actual.Fields.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
            foreach (var field in expected.Fields)
            {
                if (!actualFields.TryGetValue(field.Key, out var value))
                {
                    issues.Add(Issue(
                        "h1.semantic_field_missing",
                        expected.FactId,
                        "declared H1 projection fields must be complete against accepted authority",
                        "Missing field '" + field.Key + "'.",
                        expected.SourcePath));
                }
                else if (value.Kind != DesignValueKind.String ||
                         !StringComparer.Ordinal.Equals(value.CanonicalValue, field.Value))
                {
                    issues.Add(Issue(
                        "h1.semantic_content_mismatch",
                        expected.FactId,
                        "declared H1 projection fields must exactly preserve accepted authority values",
                        "Field '" + field.Key + "' expected '" + field.Value + "' but projected '" + value + "'.",
                        expected.SourcePath));
                }
            }

            foreach (var field in actualFields.Keys.Where(key => !expected.Fields.ContainsKey(key)))
            {
                issues.Add(Issue(
                    "h1.semantic_field_unexpected",
                    expected.FactId,
                    "the versioned H1 adapter is fail-closed to undeclared projection fields",
                    "Unexpected projected field '" + field + "'.",
                    expected.SourcePath));
            }

            var expectedRelations = expected.Relations
                .Select(RelationKey)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToList();
            var actualRelations = actual.Relations
                .Select(relation => relation.RelationType + "\u001f" + relation.TargetFactId)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToList();

            if (!expectedRelations.SequenceEqual(actualRelations, StringComparer.Ordinal))
            {
                var missing = MultisetDifference(expectedRelations, actualRelations);
                var extra = MultisetDifference(actualRelations, expectedRelations);
                var code = missing.Count > 0 && extra.Count == 0
                    ? "h1.semantic_relation_missing"
                    : missing.Count == 0 && extra.Count > 0
                        ? "h1.semantic_relation_extra"
                        : "h1.semantic_relation_mismatch";

                issues.Add(Issue(
                    code,
                    expected.FactId,
                    "H1 relation names, cardinality and targets must match independently reconstructed authority meaning",
                    "missing=[" + string.Join(",", missing) + "] extra=[" + string.Join(",", extra) + "]",
                    expected.SourcePath));
            }

            var provenance = actual.Provenance;
            var expectedSourceDigest = Sha256(expected.SourceText);
            var expectedAnchorDigest = Sha256(expected.Anchor);
            if (!StringComparer.Ordinal.Equals(provenance.AuthorityId, expected.AuthorityId) ||
                !StringComparer.Ordinal.Equals(provenance.SourcePath, expected.SourcePath) ||
                !StringComparer.Ordinal.Equals(provenance.Anchor, expected.Anchor) ||
                !StringComparer.Ordinal.Equals(provenance.SourceDigest, expectedSourceDigest) ||
                !StringComparer.Ordinal.Equals(provenance.AnchorDigest, expectedAnchorDigest))
            {
                issues.Add(Issue(
                    "h1.semantic_provenance_mismatch",
                    expected.FactId,
                    "equal H1 values are insufficient without exact source-open accepted H1-04 provenance",
                    "Projected provenance does not resolve to the independently reconstructed accepted source anchor.",
                    expected.SourcePath,
                    provenance.SourcePath));
            }
        }

        private static List<string> MultisetDifference(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var value in right)
            {
                counts[value] = counts.TryGetValue(value, out var count) ? count + 1 : 1;
            }

            var result = new List<string>();
            foreach (var value in left)
            {
                if (counts.TryGetValue(value, out var count) && count > 0)
                {
                    counts[value] = count - 1;
                }
                else
                {
                    result.Add(value.Replace("\u001f", "->"));
                }
            }

            return result;
        }

        private static string RelationKey(OracleRelation relation) =>
            relation.Type + "\u001f" + relation.Target;

        private static H1CatalogueSemanticIssue Issue(
            string code,
            string factId,
            string rule,
            string detail,
            params string[] paths) =>
            new H1CatalogueSemanticIssue(
                code,
                factId,
                rule,
                detail,
                paths.Where(path => !string.IsNullOrWhiteSpace(path)).Distinct(StringComparer.Ordinal));

        private static string Sha256(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);
                foreach (var item in hash)
                {
                    builder.Append(item.ToString("x2", CultureInfo.InvariantCulture));
                }
                return builder.ToString();
            }
        }

        private sealed class OracleBuilder
        {
            private const string CataloguePath = "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json";
            private const string AdoptionPath = "Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json";
            private const string CatalogueBlob = "922dbdffbe2f0a622acc2e153ca8b181427f6ce6";
            private const string AdoptionBlob = "664e83e25269f345a248ce43410a28ed0a670750";
            private const string CandidateSha = "8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5";
            private const string AdapterId = "ctx-dw-h1-01-adapter-v1";

            private readonly SortedDictionary<string, OracleFact> _facts =
                new SortedDictionary<string, OracleFact>(StringComparer.Ordinal);

            public IReadOnlyDictionary<string, OracleFact> Build(H1AcceptedAuthoritySources sources)
            {
                var catalogue = sources.Read(CataloguePath);
                var adoption = sources.Read(AdoptionPath);
                RequireBlob(CataloguePath, CatalogueBlob, catalogue);
                RequireBlob(AdoptionPath, AdoptionBlob, adoption);

                var catalogueSchema = JsonSurface.TopLevelString(catalogue, "schemaId", CataloguePath);
                var projectIdentity = JsonSurface.TopLevelString(catalogue, "projectIdentity", CataloguePath);
                if (!StringComparer.Ordinal.Equals(catalogueSchema, "arkus.h1-catalogue-mapping@1"))
                {
                    throw Source(
                        "h1.oracle_catalogue_schema_stale",
                        CataloguePath,
                        CataloguePath,
                        "Unexpected catalogue schema '" + catalogueSchema + "'.");
                }

                if (!StringComparer.Ordinal.Equals(projectIdentity, "arkus.unity-project@1:ArkusUnity"))
                {
                    throw Source(
                        "h1.oracle_project_identity_stale",
                        CataloguePath,
                        CataloguePath,
                        "Unexpected project identity '" + projectIdentity + "'.");
                }

                Add(new OracleFact(
                    "h1.catalogue",
                    "h1-catalogue",
                    "h1-04-catalogue-mapping",
                    CataloguePath,
                    catalogue,
                    JsonSurface.PairAnchor(
                        catalogue,
                        "schemaId",
                        catalogueSchema,
                        "projectIdentity",
                        projectIdentity,
                        CataloguePath),
                    Fields(
                        "schema-id", catalogueSchema,
                        "project-identity", projectIdentity,
                        "accepted-candidate-sha", CandidateSha,
                        "accepted-blob-sha", CatalogueBlob,
                        "adapter-id", AdapterId),
                    Array.Empty<OracleRelation>()));

                var adoptionSchema = JsonSurface.TopLevelString(adoption, "schemaId", AdoptionPath);
                var distributionMode = JsonSurface.TopLevelString(adoption, "distributionMode", AdoptionPath);
                if (!StringComparer.Ordinal.Equals(adoptionSchema, "arkus.h1-04-source-adoption@1"))
                {
                    throw Source(
                        "h1.oracle_adoption_schema_stale",
                        AdoptionPath,
                        AdoptionPath,
                        "Unexpected source-adoption schema '" + adoptionSchema + "'.");
                }

                if (!StringComparer.Ordinal.Equals(distributionMode, "external-readonly-local-source"))
                {
                    throw Source(
                        "h1.oracle_distribution_mode_stale",
                        AdoptionPath,
                        AdoptionPath,
                        "Unexpected distribution mode '" + distributionMode + "'.");
                }

                Add(new OracleFact(
                    "h1.source-adoption",
                    "h1-source-adoption",
                    "h1-04-source-adoption",
                    AdoptionPath,
                    adoption,
                    JsonSurface.PairAnchor(
                        adoption,
                        "schemaId",
                        adoptionSchema,
                        "distributionMode",
                        distributionMode,
                        AdoptionPath),
                    Fields(
                        "schema-id", adoptionSchema,
                        "distribution-mode", distributionMode,
                        "accepted-candidate-sha", CandidateSha,
                        "accepted-blob-sha", AdoptionBlob,
                        "adapter-id", AdapterId),
                    Array.Empty<OracleRelation>()));

                var sourceIds = new HashSet<string>(StringComparer.Ordinal);
                var sourceObjects = JsonSurface.ObjectsInArray(adoption, "sources", AdoptionPath);
                if (sourceObjects.Count == 0)
                {
                    throw Source(
                        "h1.oracle_source_universe_empty",
                        AdoptionPath,
                        AdoptionPath,
                        "No authority-side source records found.");
                }

                foreach (var raw in sourceObjects)
                {
                    var values = JsonSurface.ObjectFields(raw, AdoptionPath);
                    var sourceId = RequireNonEmpty(values, "sourceId", AdoptionPath);
                    if (!sourceIds.Add(sourceId))
                    {
                        throw Source(
                            "h1.oracle_source_identity_duplicate",
                            sourceId,
                            AdoptionPath,
                            "Duplicate sourceId.");
                    }

                    Add(new OracleFact(
                        "h1.source." + sourceId,
                        "h1-source",
                        "h1-04-source-adoption",
                        AdoptionPath,
                        adoption,
                        raw,
                        Fields(
                            "source-id", sourceId,
                            "origin-url", RequireNonEmpty(values, "originUrl", AdoptionPath),
                            "distribution-sha256", RequireNonEmpty(values, "distributionSha256", AdoptionPath),
                            "license-id", RequireNonEmpty(values, "licenseId", AdoptionPath),
                            "license-sha256", RequireNonEmpty(values, "licenseSha256", AdoptionPath),
                            "commercial-use", RequireNonEmpty(values, "commercialUse", AdoptionPath),
                            "notice-requirement", RequireNonEmpty(values, "noticeRequirement", AdoptionPath)),
                        new[] { new OracleRelation("declared-in", "h1.source-adoption") }));
                }

                var catalogueObjects = JsonSurface.ObjectsInArray(catalogue, "entries", CataloguePath);
                if (catalogueObjects.Count == 0)
                {
                    throw Source(
                        "h1.oracle_catalogue_universe_empty",
                        CataloguePath,
                        CataloguePath,
                        "No authority-side catalogue records found.");
                }

                var logicalIds = new HashSet<string>(StringComparer.Ordinal);
                foreach (var raw in catalogueObjects)
                {
                    var values = JsonSurface.ObjectFields(raw, CataloguePath);
                    var logicalId = RequireNonEmpty(values, "logicalId", CataloguePath);
                    if (!logicalIds.Add(logicalId))
                    {
                        throw Source(
                            "h1.oracle_catalogue_identity_duplicate",
                            logicalId,
                            CataloguePath,
                            "Duplicate logicalId.");
                    }

                    var kind = RequireNonEmpty(values, "kind", CataloguePath);
                    var nativeGuid = RequirePresent(values, "nativeGuid", CataloguePath);
                    var localFileId = RequireNonEmpty(values, "localFileId", CataloguePath);
                    var contentSha256 = RequirePresent(values, "contentSha256", CataloguePath);
                    ValidateLocator(kind, nativeGuid, localFileId, contentSha256, logicalId);

                    var sourceId = RequireNonEmpty(values, "sourceId", CataloguePath);
                    var relations = new List<OracleRelation>
                    {
                        new OracleRelation("declared-in", "h1.catalogue")
                    };
                    if (sourceIds.Contains(sourceId))
                    {
                        relations.Add(new OracleRelation("adopted-from-source", "h1.source." + sourceId));
                    }

                    Add(new OracleFact(
                        logicalId,
                        "h1-catalogue-entry",
                        "h1-04-catalogue-mapping",
                        CataloguePath,
                        catalogue,
                        raw,
                        Fields(
                            "logical-id", logicalId,
                            "kind", kind,
                            "native-guid", nativeGuid,
                            "local-file-id", localFileId,
                            "type-name", RequireNonEmpty(values, "typeName", CataloguePath),
                            "source-id", sourceId,
                            "adoption-status", RequireNonEmpty(values, "adoptionStatus", CataloguePath),
                            "content-sha256", contentSha256),
                        relations));
                }

                var slices = JsonSurface.ObjectsInArray(adoption, "slices", AdoptionPath);
                if (slices.Count == 0)
                {
                    throw Source(
                        "h1.oracle_slice_universe_empty",
                        AdoptionPath,
                        AdoptionPath,
                        "No authority-side source slices found.");
                }

                var sliceIds = new HashSet<string>(StringComparer.Ordinal);
                foreach (var raw in slices)
                {
                    var values = JsonSurface.ObjectFields(raw, AdoptionPath);
                    var assetPath = RequireNonEmpty(values, "assetPath", AdoptionPath);
                    var sourceId = RequireNonEmpty(values, "sourceId", AdoptionPath);
                    if (!sourceIds.Contains(sourceId))
                    {
                        throw Source(
                            "h1.oracle_slice_orphan",
                            assetPath,
                            AdoptionPath,
                            "Source slice points to undeclared sourceId '" + sourceId + "'.");
                    }

                    var factId = "h1.slice." + Sha256(assetPath).Substring(0, 24);
                    if (!sliceIds.Add(factId))
                    {
                        throw Source(
                            "h1.oracle_slice_identity_duplicate",
                            assetPath,
                            AdoptionPath,
                            "Duplicate source-slice identity.");
                    }

                    Add(new OracleFact(
                        factId,
                        "h1-source-slice",
                        "h1-04-source-adoption",
                        AdoptionPath,
                        adoption,
                        raw,
                        Fields(
                            "asset-path", assetPath,
                            "source-id", sourceId,
                            "adoption-status", RequireNonEmpty(values, "adoptionStatus", AdoptionPath),
                            "content-sha256", RequireNonEmpty(values, "contentSha256", AdoptionPath)),
                        new[]
                        {
                            new OracleRelation("declared-in", "h1.source-adoption"),
                            new OracleRelation("adopted-from-source", "h1.source." + sourceId)
                        }));
                }

                return new ReadOnlyDictionary<string, OracleFact>(_facts);
            }

            private static void ValidateLocator(
                string kind,
                string nativeGuid,
                string localFileId,
                string contentSha256,
                string logicalId)
            {
                if (StringComparer.Ordinal.Equals(kind, "component-schema"))
                {
                    if (nativeGuid.Length != 0 ||
                        !StringComparer.Ordinal.Equals(localFileId, "0") ||
                        contentSha256.Length != 0)
                    {
                        throw Source(
                            "h1.oracle_component_locator_stale",
                            logicalId,
                            CataloguePath,
                            "Component-schema locator must remain empty GUID/content with localFileId 0.");
                    }
                    return;
                }

                if (nativeGuid.Length != 32 || contentSha256.Length != 64)
                {
                    throw Source(
                        "h1.oracle_native_locator_stale",
                        logicalId,
                        CataloguePath,
                        "Asset-like catalogue entry lost its reviewed native GUID or content fingerprint.");
                }
            }

            private void Add(OracleFact fact)
            {
                if (_facts.ContainsKey(fact.FactId))
                {
                    throw Source(
                        "h1.oracle_identity_duplicate",
                        fact.FactId,
                        fact.SourcePath,
                        "Independent oracle produced duplicate identity.");
                }
                _facts.Add(fact.FactId, fact);
            }

            private static SortedDictionary<string, string> Fields(params string[] values)
            {
                if (values.Length % 2 != 0)
                {
                    throw new ArgumentException("Fields require name/value pairs.", nameof(values));
                }

                var result = new SortedDictionary<string, string>(StringComparer.Ordinal);
                for (var index = 0; index < values.Length; index += 2)
                {
                    result.Add(values[index], values[index + 1]);
                }
                return result;
            }

            private static string RequireNonEmpty(
                IReadOnlyDictionary<string, string> values,
                string name,
                string path)
            {
                if (!values.TryGetValue(name, out var value) || string.IsNullOrEmpty(value))
                {
                    throw Source(
                        "h1.oracle_field_missing",
                        name,
                        path,
                        "Required authority field '" + name + "' is missing or empty.");
                }
                return value;
            }

            private static string RequirePresent(
                IReadOnlyDictionary<string, string> values,
                string name,
                string path)
            {
                if (!values.TryGetValue(name, out var value))
                {
                    throw Source(
                        "h1.oracle_field_missing",
                        name,
                        path,
                        "Required authority field '" + name + "' is missing.");
                }
                return value;
            }

            private static void RequireBlob(string path, string expected, string text)
            {
                var actual = GitBlobSha1(text);
                if (!StringComparer.Ordinal.Equals(expected, actual))
                {
                    throw Source(
                        "h1.oracle_source_revision",
                        path,
                        path,
                        "Independent oracle source blob mismatch: expected " + expected + " actual " + actual + ".");
                }
            }

            private static string GitBlobSha1(string text)
            {
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
                    foreach (var item in hash)
                    {
                        builder.Append(item.ToString("x2", CultureInfo.InvariantCulture));
                    }
                    return builder.ToString();
                }
            }
        }

        private static class JsonSurface
        {
            private static readonly Regex ScalarLine = new Regex(
                @"^\s*""(?<key>[^""]+)""\s*:\s*(?<value>""(?:\\.|[^""])*""|true|false|-?[0-9]+)\s*,?\s*$",
                RegexOptions.CultureInvariant);

            public static string TopLevelString(string source, string property, string path)
            {
                var marker = "\"" + property + "\"";
                var index = source.IndexOf(marker, StringComparison.Ordinal);
                if (index < 0)
                {
                    throw Source(
                        "h1.oracle_property_missing",
                        property,
                        path,
                        "Missing top-level property '" + property + "'.");
                }

                var colon = source.IndexOf(':', index + marker.Length);
                if (colon < 0)
                {
                    throw Source(
                        "h1.oracle_property_shape",
                        property,
                        path,
                        "Malformed top-level property '" + property + "'.");
                }

                var quote = source.IndexOf('"', colon + 1);
                if (quote < 0)
                {
                    throw Source(
                        "h1.oracle_property_shape",
                        property,
                        path,
                        "Top-level property '" + property + "' is not a string.");
                }

                var end = FindStringEnd(source, quote + 1);
                if (end < 0)
                {
                    throw Source(
                        "h1.oracle_property_shape",
                        property,
                        path,
                        "Top-level property '" + property + "' is unterminated.");
                }

                return Unescape(source.Substring(quote + 1, end - quote - 1));
            }

            public static string PairAnchor(
                string source,
                string firstName,
                string firstValue,
                string secondName,
                string secondValue,
                string path)
            {
                var first = "\"" + firstName + "\"";
                var second = "\"" + secondName + "\"";
                var start = source.IndexOf(first, StringComparison.Ordinal);
                var secondStart = start < 0
                    ? -1
                    : source.IndexOf(second, start + first.Length, StringComparison.Ordinal);
                if (start < 0 || secondStart < 0)
                {
                    throw Source(
                        "h1.oracle_root_anchor_missing",
                        firstName,
                        path,
                        "Could not locate root provenance pair.");
                }

                var colon = source.IndexOf(':', secondStart + second.Length);
                var quote = colon < 0 ? -1 : source.IndexOf('"', colon + 1);
                var endQuote = quote < 0 ? -1 : FindStringEnd(source, quote + 1);
                if (endQuote < 0)
                {
                    throw Source(
                        "h1.oracle_root_anchor_shape",
                        secondName,
                        path,
                        "Could not delimit root provenance pair.");
                }

                var anchor = source.Substring(start, endQuote - start + 1);
                if (!anchor.Contains(firstValue, StringComparison.Ordinal) ||
                    !anchor.Contains(secondValue, StringComparison.Ordinal))
                {
                    throw Source(
                        "h1.oracle_root_anchor_value",
                        firstName,
                        path,
                        "Root provenance pair does not contain independently parsed values.");
                }

                return anchor;
            }

            public static IReadOnlyList<string> ObjectsInArray(
                string source,
                string property,
                string path)
            {
                var marker = "\"" + property + "\"";
                var propertyIndex = source.IndexOf(marker, StringComparison.Ordinal);
                if (propertyIndex < 0)
                {
                    throw Source(
                        "h1.oracle_array_missing",
                        property,
                        path,
                        "Missing authority array '" + property + "'.");
                }

                var arrayStart = source.IndexOf('[', propertyIndex + marker.Length);
                if (arrayStart < 0)
                {
                    throw Source(
                        "h1.oracle_array_shape",
                        property,
                        path,
                        "Authority array '" + property + "' has no opening bracket.");
                }

                var objects = new List<string>();
                var arrayDepth = 1;
                var objectDepth = 0;
                var objectStart = -1;
                var inString = false;
                var escaped = false;

                for (var index = arrayStart + 1; index < source.Length; index++)
                {
                    var character = source[index];
                    if (inString)
                    {
                        if (escaped) escaped = false;
                        else if (character == '\\') escaped = true;
                        else if (character == '"') inString = false;
                        continue;
                    }

                    if (character == '"')
                    {
                        inString = true;
                        continue;
                    }

                    if (character == '[')
                    {
                        arrayDepth++;
                        continue;
                    }

                    if (character == ']')
                    {
                        arrayDepth--;
                        if (arrayDepth == 0)
                        {
                            if (objectDepth != 0)
                            {
                                throw Source(
                                    "h1.oracle_object_shape",
                                    property,
                                    path,
                                    "Unclosed object in array '" + property + "'.");
                            }
                            return objects.AsReadOnly();
                        }
                        continue;
                    }

                    if (arrayDepth != 1) continue;

                    if (character == '{')
                    {
                        if (objectDepth == 0) objectStart = index;
                        objectDepth++;
                    }
                    else if (character == '}')
                    {
                        objectDepth--;
                        if (objectDepth < 0)
                        {
                            throw Source(
                                "h1.oracle_object_shape",
                                property,
                                path,
                                "Unexpected object terminator in array '" + property + "'.");
                        }

                        if (objectDepth == 0 && objectStart >= 0)
                        {
                            objects.Add(source.Substring(objectStart, index - objectStart + 1));
                            objectStart = -1;
                        }
                    }
                }

                throw Source(
                    "h1.oracle_array_shape",
                    property,
                    path,
                    "Authority array '" + property + "' has no closing bracket.");
            }

            public static IReadOnlyDictionary<string, string> ObjectFields(
                string rawObject,
                string path)
            {
                var result = new Dictionary<string, string>(StringComparer.Ordinal);
                var lines = rawObject.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
                foreach (var line in lines)
                {
                    var match = ScalarLine.Match(line);
                    if (!match.Success) continue;

                    var key = match.Groups["key"].Value;
                    var token = match.Groups["value"].Value;
                    var value = token.Length >= 2 && token[0] == '"'
                        ? Unescape(token.Substring(1, token.Length - 2))
                        : token;
                    if (!result.TryAdd(key, value))
                    {
                        throw Source(
                            "h1.oracle_field_duplicate",
                            key,
                            path,
                            "Duplicate field '" + key + "' in authority object.");
                    }
                }

                return new ReadOnlyDictionary<string, string>(result);
            }

            private static int FindStringEnd(string source, int start)
            {
                var escaped = false;
                for (var index = start; index < source.Length; index++)
                {
                    var character = source[index];
                    if (escaped) escaped = false;
                    else if (character == '\\') escaped = true;
                    else if (character == '"') return index;
                }
                return -1;
            }

            private static string Unescape(string value) =>
                value
                    .Replace("\\\"", "\"")
                    .Replace("\\\\", "\\")
                    .Replace("\\/", "/")
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\r")
                    .Replace("\\t", "\t");
        }

        private sealed class OracleFact
        {
            public OracleFact(
                string factId,
                string factType,
                string authorityId,
                string sourcePath,
                string sourceText,
                string anchor,
                SortedDictionary<string, string> fields,
                IEnumerable<OracleRelation> relations)
            {
                FactId = factId;
                FactType = factType;
                AuthorityId = authorityId;
                SourcePath = sourcePath;
                SourceText = sourceText;
                Anchor = anchor;
                Fields = fields;
                Relations = new List<OracleRelation>(relations).AsReadOnly();
            }

            public string FactId { get; }
            public string FactType { get; }
            public string AuthorityId { get; }
            public string SourcePath { get; }
            public string SourceText { get; }
            public string Anchor { get; }
            public SortedDictionary<string, string> Fields { get; }
            public IReadOnlyList<OracleRelation> Relations { get; }
        }

        private sealed class OracleRelation
        {
            public OracleRelation(string type, string target)
            {
                Type = type;
                Target = target;
            }

            public string Type { get; }
            public string Target { get; }
        }

        private sealed class OracleSourceException : InvalidOperationException
        {
            public OracleSourceException(
                string machineCode,
                string subjectId,
                string sourcePath,
                string message)
                : base(message)
            {
                MachineCode = machineCode;
                SubjectId = subjectId;
                SourcePath = sourcePath;
            }

            public string MachineCode { get; }
            public string SubjectId { get; }
            public string SourcePath { get; }
        }

        private static OracleSourceException Source(
            string code,
            string subjectId,
            string path,
            string detail) =>
            new OracleSourceException(code, subjectId, path, detail);
    }
}
