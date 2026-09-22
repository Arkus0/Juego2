using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Arkus.DesignWorld
{
    public sealed class CityProductionQueryException : InvalidOperationException
    {
        public CityProductionQueryException(
            string machineCode,
            string subjectId,
            string rule,
            string detail,
            params string[] sourcePaths)
            : base(detail)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            SubjectId = subjectId ?? string.Empty;
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            Detail = detail ?? throw new ArgumentNullException(nameof(detail));
            SourcePaths = new List<string>(sourcePaths ?? throw new ArgumentNullException(nameof(sourcePaths))).AsReadOnly();
        }

        public string MachineCode { get; }
        public string SubjectId { get; }
        public string Rule { get; }
        public string Detail { get; }
        public IReadOnlyList<string> SourcePaths { get; }
    }

    public sealed class CityProjectionManifestField
    {
        public CityProjectionManifestField(string heading, string column, string purpose)
        {
            Heading = RequireText(heading, nameof(heading));
            Column = RequireText(column, nameof(column));
            Purpose = RequireText(purpose, nameof(purpose));
        }

        public string SourcePath => CityProductionQueryProvider.ProgrammeSourcePath;
        public string Heading { get; }
        public string Column { get; }
        public string Purpose { get; }

        private static string RequireText(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Manifest text cannot be empty.", parameterName);
            }

            return value;
        }
    }

    public static class CityProductionProjectionManifest
    {
        public const string ManifestId = "dw02-city-production-manifest-v1";
        public const string ProgrammeHeading = "## 4. District × location programme";

        private static readonly IReadOnlyList<CityProjectionManifestField> ManifestFields =
            new List<CityProjectionManifestField>
            {
                new CityProjectionManifestField(ProgrammeHeading, "Programme ID", "stable projected identity and loc/fam query family"),
                new CityProjectionManifestField(ProgrammeHeading, "District/family", "district grouping and district content-shape distribution"),
                new CityProjectionManifestField(ProgrammeHeading, "Place / family", "human-readable projected result label"),
                new CityProjectionManifestField(ProgrammeHeading, "A–D", "systemic-importance distribution and RD-5 cross-tab"),
                new CityProjectionManifestField(ProgrammeHeading, "S", "spatial-production-depth distribution and RD-5 cross-tab"),
                new CityProjectionManifestField(ProgrammeHeading, "Interior", "I0-I3 distribution and I2+ production query"),
                new CityProjectionManifestField(ProgrammeHeading, "Default access posture", "source-owned required access-role query and demand counts")
            }.AsReadOnly();

        public static IReadOnlyList<CityProjectionManifestField> Fields => ManifestFields;
    }

    public sealed class CityProductionQueryResult
    {
        internal CityProductionQueryResult(DesignFact fact)
        {
            if (fact == null) throw new ArgumentNullException(nameof(fact));
            FactId = fact.FactId;
            District = RequiredField(fact, "district");
            Label = RequiredField(fact, "place-label");
            Importance = RequiredField(fact, "importance");
            SpatialDepth = RequiredField(fact, "spatial-depth");
            InteriorDepth = RequiredField(fact, "interior-depth");
            ProgrammeKind = RequiredField(fact, "programme-kind");
            Provenance = fact.Provenance;
        }

        public string FactId { get; }
        public string District { get; }
        public string Label { get; }
        public string Importance { get; }
        public string SpatialDepth { get; }
        public string InteriorDepth { get; }
        public string ProgrammeKind { get; }
        public DesignAuthorityAnchor Provenance { get; }

        private static string RequiredField(DesignFact fact, string name)
        {
            if (!fact.Fields.TryGetValue(name, out var value) || value.Kind != DesignValueKind.String)
            {
                throw new CityProductionQueryException(
                    "city.query_field_missing",
                    fact.FactId,
                    "every projected CITY-02 production subject must expose the reviewed query fields",
                    "Projected subject '" + fact.FactId + "' is missing string field '" + name + "'.",
                    fact.Provenance.SourcePath);
            }

            return value.CanonicalValue;
        }
    }

    public sealed class CityDistrictPoiBucket
    {
        internal CityDistrictPoiBucket(string district, IEnumerable<CityProductionQueryResult> results)
        {
            District = district ?? throw new ArgumentNullException(nameof(district));
            Results = new List<CityProductionQueryResult>(results ?? throw new ArgumentNullException(nameof(results))).AsReadOnly();
        }

        public string District { get; }
        public IReadOnlyList<CityProductionQueryResult> Results { get; }
    }

    public sealed class CityContentShapeReport
    {
        internal CityContentShapeReport(
            int totalSubjects,
            int declaredPois,
            int programmeFamilies,
            SortedDictionary<string, int> districts,
            SortedDictionary<string, int> importance,
            SortedDictionary<string, int> spatialDepth,
            SortedDictionary<string, int> interiorDepth,
            SortedDictionary<string, int> importanceBySpatialDepth,
            SortedDictionary<string, int> accessRoles)
        {
            TotalSubjects = totalSubjects;
            DeclaredPoiCount = declaredPois;
            ProgrammeFamilyCount = programmeFamilies;
            DistrictCounts = ReadOnly(districts);
            ImportanceCounts = ReadOnly(importance);
            SpatialDepthCounts = ReadOnly(spatialDepth);
            InteriorDepthCounts = ReadOnly(interiorDepth);
            ImportanceBySpatialDepthCounts = ReadOnly(importanceBySpatialDepth);
            AccessRoleCounts = ReadOnly(accessRoles);
        }

        public string SchemaId => "dw02-city-content-shape-v1";
        public string CostModelStatus => "UNMODELED";
        public int TotalSubjects { get; }
        public int DeclaredPoiCount { get; }
        public int ProgrammeFamilyCount { get; }
        public IReadOnlyDictionary<string, int> DistrictCounts { get; }
        public IReadOnlyDictionary<string, int> ImportanceCounts { get; }
        public IReadOnlyDictionary<string, int> SpatialDepthCounts { get; }
        public IReadOnlyDictionary<string, int> InteriorDepthCounts { get; }
        public IReadOnlyDictionary<string, int> ImportanceBySpatialDepthCounts { get; }
        public IReadOnlyDictionary<string, int> AccessRoleCounts { get; }

        public string ToNormalizedText()
        {
            var builder = new StringBuilder();
            builder.Append("schema=").Append(SchemaId).Append('\n');
            builder.Append("cost-model=").Append(CostModelStatus).Append('\n');
            builder.Append("total-subjects=").Append(TotalSubjects).Append('\n');
            builder.Append("declared-pois=").Append(DeclaredPoiCount).Append('\n');
            builder.Append("programme-families=").Append(ProgrammeFamilyCount).Append('\n');
            AppendMap(builder, "district", DistrictCounts);
            AppendMap(builder, "importance", ImportanceCounts);
            AppendMap(builder, "spatial-depth", SpatialDepthCounts);
            AppendMap(builder, "interior-depth", InteriorDepthCounts);
            AppendMap(builder, "importance-spatial", ImportanceBySpatialDepthCounts);
            AppendMap(builder, "access-role", AccessRoleCounts);
            return builder.ToString();
        }

        private static IReadOnlyDictionary<string, int> ReadOnly(SortedDictionary<string, int> source) =>
            new ReadOnlyDictionary<string, int>(new SortedDictionary<string, int>(source, StringComparer.Ordinal));

        private static void AppendMap(StringBuilder builder, string prefix, IReadOnlyDictionary<string, int> map)
        {
            foreach (var pair in map.OrderBy(item => item.Key, StringComparer.Ordinal))
            {
                builder.Append(prefix).Append('[').Append(pair.Key).Append("]=").Append(pair.Value).Append('\n');
            }
        }
    }

    public sealed class CityProductionQueryDataset
    {
        internal CityProductionQueryDataset(DesignWorldProjection projection, CityProductionQueryService queries)
        {
            Projection = projection ?? throw new ArgumentNullException(nameof(projection));
            Queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        public DesignWorldProjection Projection { get; }
        public CityProductionQueryService Queries { get; }
        public string ManifestId => CityProductionProjectionManifest.ManifestId;
    }

    public sealed class CityProductionQueryService
    {
        private static readonly string[] Roles = { "private", "public", "semi-private", "service" };
        private static readonly string[] ImportanceValues = { "A", "B", "C", "D" };
        private static readonly string[] SpatialDepthValues = { "S0", "S1", "S2", "S3", "S4" };
        private static readonly string[] InteriorDepthValues = { "I0", "I1", "I2", "I3" };
        private readonly IReadOnlyList<DesignFact> _facts;
        private readonly IReadOnlyList<CityProductionQueryResult> _results;

        internal CityProductionQueryService(IEnumerable<DesignFact> facts, IEnumerable<string> requiredSubjectIds)
        {
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            if (requiredSubjectIds == null) throw new ArgumentNullException(nameof(requiredSubjectIds));

            var expected = new SortedSet<string>(requiredSubjectIds, StringComparer.Ordinal);
            if (expected.Count == 0)
            {
                throw new CityProductionQueryException(
                    "city.query_universe_empty", string.Empty,
                    "the complete accepted CITY-02 programme table defines the production-query universe",
                    "The required production-query subject universe is empty.",
                    CityProductionQueryProvider.ProgrammeSourcePath);
            }

            var ordered = facts.OrderBy(fact => fact.FactId, StringComparer.Ordinal).ToList();
            var actual = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var fact in ordered)
            {
                if (!actual.Add(fact.FactId))
                {
                    throw Error("city.query_subject_duplicate", fact.FactId,
                        "each required production-query subject must occur exactly once",
                        "Projected subject occurs more than once.");
                }
            }

            foreach (var id in expected)
            {
                if (!actual.Contains(id))
                {
                    throw Error("city.query_subject_missing", id,
                        "the production-query projection must contain every independently required CITY-02 programme subject",
                        "Required projected subject is absent while the source-owned query universe remains complete.");
                }
            }

            foreach (var id in actual)
            {
                if (!expected.Contains(id))
                {
                    throw Error("city.query_subject_unexpected", id,
                        "the production-query projection must not grow beyond the independently required CITY-02 programme universe",
                        "Projected subject is outside the source-owned query universe.");
                }
            }

            foreach (var fact in ordered)
            {
                ValidateFact(fact);
            }

            _facts = ordered.AsReadOnly();
            _results = ordered.Select(fact => new CityProductionQueryResult(fact)).ToList().AsReadOnly();
        }

        public IReadOnlyList<CityProductionQueryResult> AllSubjects => _results;

        public IReadOnlyList<CityDistrictPoiBucket> DeclaredPoisByDistrict()
        {
            return _results
                .Where(result => StringComparer.Ordinal.Equals(result.ProgrammeKind, "poi"))
                .GroupBy(result => result.District, StringComparer.Ordinal)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .Select(group => new CityDistrictPoiBucket(
                    group.Key,
                    group.OrderBy(result => result.FactId, StringComparer.Ordinal)))
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<CityProductionQueryResult> SubjectsRequiringAccessRole(string role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (!Roles.Contains(role, StringComparer.Ordinal))
            {
                throw new ArgumentOutOfRangeException(nameof(role), "Unsupported CITY access role.");
            }

            return _facts
                .Where(fact => HasRole(fact, role))
                .OrderBy(fact => fact.FactId, StringComparer.Ordinal)
                .Select(fact => new CityProductionQueryResult(fact))
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<CityProductionQueryResult> HigherDepthInteriors()
        {
            return _results
                .Where(result => result.InteriorDepth == "I2" || result.InteriorDepth == "I3")
                .OrderBy(result => result.InteriorDepth == "I3" ? 0 : 1)
                .ThenBy(result => result.FactId, StringComparer.Ordinal)
                .ToList()
                .AsReadOnly();
        }

        public CityContentShapeReport BuildContentShapeReport()
        {
            var districts = new SortedDictionary<string, int>(StringComparer.Ordinal);
            var importance = Seed(ImportanceValues);
            var spatial = Seed(SpatialDepthValues);
            var interior = Seed(InteriorDepthValues);
            var crossTab = new SortedDictionary<string, int>(StringComparer.Ordinal);
            foreach (var importanceValue in ImportanceValues)
            {
                foreach (var spatialValue in SpatialDepthValues)
                {
                    crossTab.Add(importanceValue + "|" + spatialValue, 0);
                }
            }
            var roles = Seed(Roles);

            var poiCount = 0;
            var familyCount = 0;
            foreach (var result in _results)
            {
                Increment(districts, result.District);
                Increment(importance, result.Importance);
                Increment(spatial, result.SpatialDepth);
                Increment(interior, result.InteriorDepth);
                Increment(crossTab, result.Importance + "|" + result.SpatialDepth);
                if (result.ProgrammeKind == "poi") poiCount++;
                if (result.ProgrammeKind == "family") familyCount++;
            }

            foreach (var role in Roles)
            {
                roles[role] = _facts.Count(fact => HasRole(fact, role));
            }

            return new CityContentShapeReport(
                _results.Count,
                poiCount,
                familyCount,
                districts,
                importance,
                spatial,
                interior,
                crossTab,
                roles);
        }

        public string BuildNormalizedSnapshot()
        {
            var builder = new StringBuilder();
            builder.Append("schema=dw02-city-query-snapshot-v1\n");
            builder.Append("manifest=").Append(CityProductionProjectionManifest.ManifestId).Append('\n');

            foreach (var bucket in DeclaredPoisByDistrict())
            {
                foreach (var result in bucket.Results)
                {
                    AppendResult(builder, "q1", bucket.District, result);
                }
            }

            foreach (var role in Roles.OrderBy(value => value, StringComparer.Ordinal))
            {
                foreach (var result in SubjectsRequiringAccessRole(role))
                {
                    AppendResult(builder, "q2:" + role, string.Empty, result);
                }
            }

            foreach (var result in HigherDepthInteriors())
            {
                AppendResult(builder, "q3:" + result.InteriorDepth, string.Empty, result);
            }

            builder.Append(BuildContentShapeReport().ToNormalizedText());
            return builder.ToString();
        }

        private static SortedDictionary<string, int> Seed(IEnumerable<string> values)
        {
            var result = new SortedDictionary<string, int>(StringComparer.Ordinal);
            foreach (var value in values)
            {
                result.Add(value, 0);
            }
            return result;
        }

        private static void Increment(IDictionary<string, int> map, string key)
        {
            if (!map.TryGetValue(key, out var count))
            {
                map.Add(key, 1);
            }
            else
            {
                map[key] = count + 1;
            }
        }

        private static bool HasRole(DesignFact fact, string role)
        {
            return fact.Fields.TryGetValue("required-role-" + role, out var value) &&
                value.Kind == DesignValueKind.Boolean &&
                StringComparer.Ordinal.Equals(value.CanonicalValue, "true");
        }

        private static void ValidateFact(DesignFact fact)
        {
            if (!StringComparer.Ordinal.Equals(fact.FactType, "city-production-place"))
            {
                throw Error("city.query_fact_type_invalid", fact.FactId,
                    "production queries consume only the reviewed CITY-02 programme fact type",
                    "Projected fact type is '" + fact.FactType + "'.");
            }

            if (!StringComparer.Ordinal.Equals(fact.Provenance.SourcePath, CityProductionQueryProvider.ProgrammeSourcePath))
            {
                throw Error("city.query_provenance_invalid", fact.FactId,
                    "every production-query result must retain exact accepted CITY-02 source provenance",
                    "Projected subject points to source path '" + fact.Provenance.SourcePath + "'.");
            }

            if (string.IsNullOrWhiteSpace(fact.Provenance.Anchor) ||
                string.IsNullOrWhiteSpace(fact.Provenance.SourceDigest) ||
                string.IsNullOrWhiteSpace(fact.Provenance.AnchorDigest))
            {
                throw Error("city.query_provenance_invalid", fact.FactId,
                    "every production-query result must retain exact accepted CITY-02 source provenance",
                    "Projected subject has incomplete source provenance.");
            }

            var result = new CityProductionQueryResult(fact);
            if (!ImportanceValues.Contains(result.Importance, StringComparer.Ordinal))
            {
                throw Error("city.query_importance_invalid", fact.FactId,
                    "A-D classification must remain source-owned and queryable",
                    "Unsupported importance bucket '" + result.Importance + "'.");
            }
            if (!SpatialDepthValues.Contains(result.SpatialDepth, StringComparer.Ordinal))
            {
                throw Error("city.query_spatial_depth_invalid", fact.FactId,
                    "S0-S4 spatial-production depth must remain source-owned and queryable",
                    "Unsupported spatial-depth bucket '" + result.SpatialDepth + "'.");
            }
            if (!InteriorDepthValues.Contains(result.InteriorDepth, StringComparer.Ordinal))
            {
                throw Error("city.query_interior_depth_invalid", fact.FactId,
                    "I0-I3 interior priority must remain source-owned and queryable",
                    "Unsupported interior-depth bucket '" + result.InteriorDepth + "'.");
            }

            var expectedKind = fact.FactId.StartsWith("loc.", StringComparison.Ordinal) ? "poi" :
                fact.FactId.StartsWith("fam.", StringComparison.Ordinal) ? "family" : string.Empty;
            if (expectedKind.Length == 0 || !StringComparer.Ordinal.Equals(expectedKind, result.ProgrammeKind))
            {
                throw Error("city.query_programme_kind_invalid", fact.FactId,
                    "declared CITY-02 loc/fam identity family must remain explicit in production queries",
                    "Projected programme kind does not match the accepted stable identity family.");
            }
        }

        private static void AppendResult(
            StringBuilder builder,
            string query,
            string grouping,
            CityProductionQueryResult result)
        {
            builder.Append(query).Append('\t')
                .Append(Escape(grouping)).Append('\t')
                .Append(Escape(result.FactId)).Append('\t')
                .Append(Escape(result.District)).Append('\t')
                .Append(Escape(result.Importance)).Append('\t')
                .Append(Escape(result.SpatialDepth)).Append('\t')
                .Append(Escape(result.InteriorDepth)).Append('\t')
                .Append(Escape(result.Provenance.SourcePath)).Append('\t')
                .Append(Escape(result.Provenance.Anchor)).Append('\t')
                .Append(result.Provenance.SourceDigest).Append('\t')
                .Append(result.Provenance.AnchorDigest).Append('\n');
        }

        private static string Escape(string value) =>
            value.Replace("\\", "\\\\")
                .Replace("\t", "\\t")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");

        private static CityProductionQueryException Error(
            string code,
            string subjectId,
            string rule,
            string detail) =>
            new CityProductionQueryException(
                code,
                subjectId,
                rule,
                detail,
                CityProductionQueryProvider.ProgrammeSourcePath);
    }

    public sealed class CityProductionQueryProvider
    {
        public const string ProgrammeSourcePath = "Docs/production/CITY_LOCATION_PROGRAMME.md";
        private static readonly DesignProjectionVersion Version = new DesignProjectionVersion(2, "dw02-city-production-v1");
        private static readonly string[] AllowedRoles = { "private", "public", "semi-private", "service" };
        private static readonly string[] AllowedImportance = { "A", "B", "C", "D" };
        private static readonly string[] AllowedSpatialDepth = { "S0", "S1", "S2", "S3", "S4" };
        private readonly bool _reverseEnumeration;

        public CityProductionQueryProvider(bool reverseEnumeration = false)
        {
            _reverseEnumeration = reverseEnumeration;
        }

        public CityProductionQueryDataset BuildAndValidate(string programmeSource)
        {
            if (programmeSource == null) throw new ArgumentNullException(nameof(programmeSource));
            var rows = ParseProgramme(programmeSource);
            var requiredIds = rows.Keys.ToList();
            var definitions = rows.Values.Select(ToDefinition).ToList();
            if (_reverseEnumeration)
            {
                requiredIds.Reverse();
                definitions.Reverse();
            }

            var universe = new StaticDesignAuthorityUniverse(requiredIds);
            var reader = new AnchoredTextAuthorityReader(
                "city02-location-programme-dw02-v1",
                ProgrammeSourcePath,
                programmeSource,
                definitions);
            var projection = new DesignWorldProjector().Build(universe, reader, Version);
            var validation = new DesignWorldProjectionValidator().Validate(projection, universe, reader, Version);
            if (!validation.IsValid)
            {
                var first = validation.Issues
                    .OrderBy(issue => issue.FactId, StringComparer.Ordinal)
                    .ThenBy(issue => issue.MachineCode, StringComparer.Ordinal)
                    .First();
                throw new CityProductionQueryException(
                    "city.query_projection_invalid",
                    first.FactId,
                    "fresh CITY-02 production projection must validate against current accepted authority",
                    first.MachineCode + ": " + first.Detail,
                    ProgrammeSourcePath);
            }

            var queries = new CityProductionQueryService(projection.Facts, rows.Keys);
            return new CityProductionQueryDataset(projection, queries);
        }

        public IReadOnlyList<DesignWorldValidationIssue> ValidateProvenanceAgainstSource(
            CityProductionQueryDataset dataset,
            string programmeSource)
        {
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));
            if (programmeSource == null) throw new ArgumentNullException(nameof(programmeSource));
            var rows = ParseProgramme(programmeSource);
            var definitions = rows.Values.Select(ToDefinition).ToList();
            var universe = new StaticDesignAuthorityUniverse(rows.Keys);
            var reader = new AnchoredTextAuthorityReader(
                "city02-location-programme-dw02-v1",
                ProgrammeSourcePath,
                programmeSource,
                definitions);
            return new DesignWorldProjectionValidator().Validate(dataset.Projection, universe, reader, Version).Issues;
        }

        private static AnchoredFactDefinition ToDefinition(ProgrammeRow row)
        {
            var fields = new Dictionary<string, DesignValue>(StringComparer.Ordinal)
            {
                ["district"] = DesignValue.String(row.District),
                ["place-label"] = DesignValue.String(row.Label),
                ["importance"] = DesignValue.String(row.Importance),
                ["spatial-depth"] = DesignValue.String(row.SpatialDepth),
                ["interior-depth"] = DesignValue.String(row.InteriorDepth),
                ["programme-kind"] = DesignValue.String(row.Kind)
            };
            foreach (var role in row.Roles)
            {
                fields.Add("required-role-" + role, DesignValue.Boolean(true));
            }
            return new AnchoredFactDefinition(row.Id, "city-production-place", row.Anchor, fields);
        }

        private static SortedDictionary<string, ProgrammeRow> ParseProgramme(string source)
        {
            var table = ParseTable(
                source,
                CityProductionProjectionManifest.ProgrammeHeading,
                "Programme ID",
                10);
            var rows = new SortedDictionary<string, ProgrammeRow>(StringComparer.Ordinal);
            foreach (var row in table)
            {
                var id = Clean(row.Cells[0]);
                var district = Clean(row.Cells[1]);
                var label = Clean(row.Cells[2]);
                var importance = Clean(row.Cells[5]);
                var spatialDepth = Clean(row.Cells[6]);
                var interiorDepth = ParseInteriorDepth(row.Cells[7], id);
                var roles = ParseProgrammeRoles(row.Cells[8]);
                var kind = id.StartsWith("loc.", StringComparison.Ordinal) ? "poi" :
                    id.StartsWith("fam.", StringComparison.Ordinal) ? "family" : string.Empty;

                if (kind.Length == 0)
                {
                    throw Shape(id, "Programme identity must use the accepted loc.* or fam.* family.");
                }
                if (district.Length == 0 || label.Length == 0)
                {
                    throw Shape(id, "District/family and Place/family values must be non-empty.");
                }
                if (!AllowedImportance.Contains(importance, StringComparer.Ordinal))
                {
                    throw Shape(id, "Unsupported A-D importance value '" + importance + "'.");
                }
                if (!AllowedSpatialDepth.Contains(spatialDepth, StringComparer.Ordinal))
                {
                    throw Shape(id, "Unsupported S0-S4 spatial depth value '" + spatialDepth + "'.");
                }
                if (rows.ContainsKey(id))
                {
                    throw new CityProductionQueryException(
                        "city.query_source_subject_duplicate",
                        id,
                        "every accepted CITY-02 programme row must have a unique stable identity",
                        "Programme identity occurs more than once in the accepted source table.",
                        ProgrammeSourcePath);
                }

                rows.Add(id, new ProgrammeRow(
                    id,
                    district,
                    label,
                    importance,
                    spatialDepth,
                    interiorDepth,
                    kind,
                    roles,
                    row.Anchor));
            }

            if (rows.Count == 0)
            {
                throw Shape(string.Empty, "The accepted CITY-02 programme table is empty.");
            }

            return rows;
        }

        private static IReadOnlyList<TableRow> ParseTable(
            string source,
            string heading,
            string firstHeader,
            int columns)
        {
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var headingIndex = Array.FindIndex(lines, line => line.Trim() == heading);
            if (headingIndex < 0)
            {
                throw Shape(string.Empty, "Required heading '" + heading + "' is absent.");
            }

            var headerIndex = -1;
            for (var index = headingIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (line.StartsWith("## ", StringComparison.Ordinal)) break;
                if (!line.StartsWith("|", StringComparison.Ordinal)) continue;
                var cells = SplitRow(line);
                if (cells.Count > 0 && Clean(cells[0]) == firstHeader)
                {
                    headerIndex = index;
                    break;
                }
            }

            if (headerIndex < 0 || headerIndex + 1 >= lines.Length)
            {
                throw Shape(string.Empty, "Required programme table is absent.");
            }
            if (SplitRow(lines[headerIndex].Trim()).Count != columns ||
                lines[headerIndex + 1].IndexOf("---", StringComparison.Ordinal) < 0)
            {
                throw Shape(string.Empty, "Required programme table changed shape.");
            }

            var rows = new List<TableRow>();
            for (var index = headerIndex + 2; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (!line.StartsWith("|", StringComparison.Ordinal)) break;
                var cells = SplitRow(line);
                if (cells.Count != columns)
                {
                    throw Shape(string.Empty, "A programme row changed column count.");
                }
                rows.Add(new TableRow(cells, line));
            }

            if (rows.Count == 0)
            {
                throw Shape(string.Empty, "Required programme table has no data rows.");
            }
            return rows.AsReadOnly();
        }

        private static IReadOnlyList<string> SplitRow(string row)
        {
            if (row.Length < 2 || row[0] != '|' || row[row.Length - 1] != '|')
            {
                throw Shape(string.Empty, "CITY programme rows must start and end with '|'.");
            }
            return row.Substring(1, row.Length - 2).Split('|')
                .Select(cell => cell.Trim())
                .ToList()
                .AsReadOnly();
        }

        private static SortedSet<string> ParseProgrammeRoles(string raw)
        {
            var text = Clean(raw).ToLowerInvariant().Replace("conditional public", string.Empty);
            var roles = new SortedSet<string>(StringComparer.Ordinal);
            if (text.Contains("semi-private"))
            {
                roles.Add("semi-private");
                text = text.Replace("semi-private", string.Empty);
            }
            if (text.Contains("public")) roles.Add("public");
            if (text.Contains("service")) roles.Add("service");
            if (text.Contains("private")) roles.Add("private");
            return roles;
        }

        private static string ParseInteriorDepth(string raw, string id)
        {
            var value = Clean(raw);
            foreach (var depth in new[] { "I0", "I1", "I2", "I3" })
            {
                if (value == depth || value.StartsWith(depth + " ", StringComparison.Ordinal))
                {
                    return depth;
                }
            }
            throw Shape(id, "Unsupported I0-I3 interior priority '" + value + "'.");
        }

        private static string Clean(string value) =>
            value.Replace("**", string.Empty).Replace("`", string.Empty).Trim();

        private static CityProductionQueryException Shape(string subjectId, string detail) =>
            new CityProductionQueryException(
                "city.query_source_shape_invalid",
                subjectId,
                "the reviewed CITY-02 production-query manifest must remain mechanically parseable",
                detail,
                ProgrammeSourcePath);

        private sealed class TableRow
        {
            public TableRow(IReadOnlyList<string> cells, string anchor)
            {
                Cells = cells;
                Anchor = anchor;
            }
            public IReadOnlyList<string> Cells { get; }
            public string Anchor { get; }
        }

        private sealed class ProgrammeRow
        {
            public ProgrammeRow(
                string id,
                string district,
                string label,
                string importance,
                string spatialDepth,
                string interiorDepth,
                string kind,
                SortedSet<string> roles,
                string anchor)
            {
                Id = id;
                District = district;
                Label = label;
                Importance = importance;
                SpatialDepth = spatialDepth;
                InteriorDepth = interiorDepth;
                Kind = kind;
                Roles = roles;
                Anchor = anchor;
            }

            public string Id { get; }
            public string District { get; }
            public string Label { get; }
            public string Importance { get; }
            public string SpatialDepth { get; }
            public string InteriorDepth { get; }
            public string Kind { get; }
            public SortedSet<string> Roles { get; }
            public string Anchor { get; }
        }
    }
}
