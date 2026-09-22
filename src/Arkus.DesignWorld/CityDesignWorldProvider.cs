using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkus.DesignWorld
{
    public sealed class CityInvariantException : InvalidOperationException
    {
        public CityInvariantException(string code, string subjectId, string rule, string detail, params string[] sources)
            : base(detail)
        {
            MachineCode = code ?? throw new ArgumentNullException(nameof(code));
            SubjectId = subjectId ?? string.Empty;
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            Detail = detail ?? throw new ArgumentNullException(nameof(detail));
            SourcePaths = new List<string>(sources ?? throw new ArgumentNullException(nameof(sources))).AsReadOnly();
        }

        public string MachineCode { get; }
        public string SubjectId { get; }
        public string Rule { get; }
        public string Detail { get; }
        public IReadOnlyList<string> SourcePaths { get; }
    }

    public sealed class CityDesignWorldSlice
    {
        internal CityDesignWorldSlice(
            DesignWorldProjection programme,
            DesignWorldProjection bindings,
            DesignWorldProjection interiors,
            int accessCount,
            int interiorCount)
        {
            ProgrammeProjection = programme;
            BindingProjection = bindings;
            InteriorProjection = interiors;
            AccessPlaceCount = accessCount;
            InteriorPlaceCount = interiorCount;
        }

        public DesignWorldProjection ProgrammeProjection { get; }
        public DesignWorldProjection BindingProjection { get; }
        public DesignWorldProjection InteriorProjection { get; }
        public int AccessPlaceCount { get; }
        public int InteriorPlaceCount { get; }
    }

    public sealed class CityDesignWorldProvider
    {
        public const string ProgrammeSourcePath = "Docs/production/CITY_LOCATION_PROGRAMME.md";
        public const string BindingSourcePath = "Docs/production/CITY_ENVIRONMENT_GRAMMAR.md";
        public const string InteriorsSourcePath = "Docs/production/CITY_INTERIORS_DISCOVERY.md";

        private static readonly DesignProjectionVersion Version = new DesignProjectionVersion(1, "dw01-city-v1");
        private static readonly string[] AllowedRoles = { "private", "public", "semi-private", "service" };
        private readonly bool _reverseEnumeration;
        private readonly Func<string, IReadOnlyList<DesignRelation>, IEnumerable<DesignRelation>> _interiorRelationTransform;

        public CityDesignWorldProvider(bool reverseEnumeration = false)
            : this(reverseEnumeration, PreserveInteriorRelations)
        {
        }

        internal CityDesignWorldProvider(
            bool reverseEnumeration,
            Func<string, IReadOnlyList<DesignRelation>, IEnumerable<DesignRelation>> interiorRelationTransform)
        {
            _reverseEnumeration = reverseEnumeration;
            _interiorRelationTransform = interiorRelationTransform ??
                throw new ArgumentNullException(nameof(interiorRelationTransform));
        }

        public CityDesignWorldSlice BuildAndValidate(string programmeSource, string bindingSource, string interiorsSource)
        {
            var model = Parse(programmeSource, bindingSource, interiorsSource);
            RequireSameSet(
                model.AccessIds, model.Bindings.Keys,
                "city.access_binding_subject_missing", "city.access_binding_subject_unexpected",
                "CITY-02 A/B universe must equal CITY-05 binding universe",
                ProgrammeSourcePath, BindingSourcePath);
            RequireSameSet(
                model.InteriorIds, model.InteriorDepths.Keys,
                "city.interior_depth_subject_missing", "city.interior_depth_subject_unexpected",
                "CITY-02 I1-I3 universe must equal CITY-06 inherited-depth universe",
                ProgrammeSourcePath, InteriorsSourcePath);
            RequireSameSet(
                model.InteriorIds, model.Allocations.Keys,
                "city.interior_allocation_missing", "city.interior_allocation_unexpected",
                "CITY-02 I1-I3 universe must equal CITY-06 Full I1-I3 allocation universe",
                ProgrammeSourcePath, InteriorsSourcePath);

            var programme = BuildProgrammeProjection(model);
            var bindings = BuildBindingProjection(model);
            var interiors = BuildInteriorProjection(model);
            ValidateFresh(programme);
            ValidateFresh(bindings);
            ValidateFresh(interiors);
            new CityInteriorRelationOracle().Validate(interiors.Projection, model.InteriorIds);
            ValidateAccess(programme.Projection, bindings.Projection, model.AccessIds);
            ValidateDepth(programme.Projection, interiors.Projection, model.InteriorIds);
            return new CityDesignWorldSlice(
                programme.Projection, bindings.Projection, interiors.Projection,
                model.AccessIds.Count, model.InteriorIds.Count);
        }

        public IReadOnlyList<DesignWorldValidationIssue> ValidateProvenanceAgainstSources(
            CityDesignWorldSlice slice,
            string programmeSource,
            string bindingSource,
            string interiorsSource)
        {
            if (slice == null)
            {
                throw new ArgumentNullException(nameof(slice));
            }

            var model = Parse(programmeSource, bindingSource, interiorsSource);
            var programme = BuildProgrammeProjection(model);
            var bindings = BuildBindingProjection(model);
            var interiors = BuildInteriorProjection(model);
            var issues = new List<DesignWorldValidationIssue>();
            issues.AddRange(new DesignWorldProjectionValidator().Validate(
                slice.ProgrammeProjection, programme.Universe, programme.Reader, Version).Issues);
            issues.AddRange(new DesignWorldProjectionValidator().Validate(
                slice.BindingProjection, bindings.Universe, bindings.Reader, Version).Issues);
            issues.AddRange(new DesignWorldProjectionValidator().Validate(
                slice.InteriorProjection, interiors.Universe, interiors.Reader, Version).Issues);
            return issues.AsReadOnly();
        }

        private ProjectionContext BuildProgrammeProjection(Model model)
        {
            var programmeIds = new SortedSet<string>(model.AccessIds, StringComparer.Ordinal);
            programmeIds.UnionWith(model.InteriorIds);
            var definitions = programmeIds.Select(id =>
            {
                var row = model.Programme[id];
                var fields = new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                {
                    ["importance"] = DesignValue.String(row.Importance),
                    ["interior-depth"] = DesignValue.String(row.InteriorDepth)
                };
                AddRoleFields(fields, row.Roles);
                return new AnchoredFactDefinition(id, "city-programme-place", row.Anchor, fields);
            }).ToList();
            return BuildOneSource(
                "city02-location-programme-v1", ProgrammeSourcePath, model.ProgrammeSource,
                programmeIds, definitions);
        }

        private ProjectionContext BuildBindingProjection(Model model)
        {
            var definitions = model.Bindings.Values.Select(row =>
            {
                var fields = new Dictionary<string, DesignValue>(StringComparer.Ordinal);
                AddRoleFields(fields, row.Roles);
                return new AnchoredFactDefinition(row.Id, "city-access-binding", row.Anchor, fields);
            }).ToList();
            return BuildOneSource(
                "city05-environment-grammar-v1", BindingSourcePath, model.BindingSource,
                model.AccessIds, definitions);
        }

        private ProjectionContext BuildInteriorProjection(Model model)
        {
            var definitions = new List<AnchoredFactDefinition>();
            foreach (var row in model.InteriorDepths.Values)
            {
                definitions.Add(new AnchoredFactDefinition(
                    DepthId(row.Id), "city-interior-depth", row.Anchor,
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["interior-depth"] = DesignValue.String(row.InteriorDepth)
                    }));
            }

            foreach (var row in model.Allocations.Values)
            {
                var canonicalRelations = new[]
                {
                    new DesignRelation("allocates-depth", DepthId(row.Id))
                };
                var relations = _interiorRelationTransform(row.Id, canonicalRelations);
                if (relations == null)
                {
                    throw new InvalidOperationException("Interior relation transform returned null.");
                }

                definitions.Add(new AnchoredFactDefinition(
                    AllocationId(row.Id), "city-interior-allocation", row.Anchor,
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["allocated"] = DesignValue.Boolean(true)
                    },
                    relations));
            }

            var ids = new List<string>();
            foreach (var id in model.InteriorIds)
            {
                ids.Add(DepthId(id));
                ids.Add(AllocationId(id));
            }

            return BuildOneSource(
                "city06-interiors-discovery-v1", InteriorsSourcePath, model.InteriorsSource,
                ids, definitions);
        }

        private ProjectionContext BuildOneSource(
            string authorityId,
            string sourcePath,
            string source,
            IEnumerable<string> requiredIds,
            List<AnchoredFactDefinition> definitions)
        {
            var ids = requiredIds.ToList();
            if (_reverseEnumeration)
            {
                ids.Reverse();
                definitions.Reverse();
            }

            var universe = new StaticDesignAuthorityUniverse(ids);
            var reader = new AnchoredTextAuthorityReader(authorityId, sourcePath, source, definitions);
            var projection = new DesignWorldProjector().Build(universe, reader, Version);
            return new ProjectionContext(projection, universe, reader);
        }

        private static void ValidateFresh(ProjectionContext context)
        {
            var report = new DesignWorldProjectionValidator().Validate(
                context.Projection, context.Universe, context.Reader, Version);
            if (!report.IsValid)
            {
                var first = report.Issues
                    .OrderBy(issue => issue.FactId, StringComparer.Ordinal)
                    .ThenBy(issue => issue.MachineCode, StringComparer.Ordinal)
                    .First();
                throw new CityInvariantException(
                    "city.dw_projection_invalid", first.FactId,
                    "fresh CITY projection must validate against current authority",
                    first.MachineCode + ": " + first.Detail,
                    context.Projection.Facts.Select(fact => fact.Provenance.SourcePath).Distinct().ToArray());
            }
        }

        private static void ValidateAccess(
            DesignWorldProjection programme,
            DesignWorldProjection bindings,
            IReadOnlyCollection<string> ids)
        {
            var p = programme.Facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);
            var b = bindings.Facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);
            foreach (var id in ids.OrderBy(value => value, StringComparer.Ordinal))
            {
                foreach (var role in AllowedRoles)
                {
                    if (HasRole(p[id], role) && !HasRole(b[id], role))
                    {
                        throw new CityInvariantException(
                            "city.access_role_missing", id,
                            "programme_required_access_roles(place) must be a subset of bound_required_access_roles(place)",
                            "Projected subject '" + id + "' is missing required access role '" + role + "'.",
                            p[id].Provenance.SourcePath, b[id].Provenance.SourcePath);
                    }
                }
            }
        }

        private static void ValidateDepth(
            DesignWorldProjection programme,
            DesignWorldProjection interiors,
            IReadOnlyCollection<string> ids)
        {
            var p = programme.Facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);
            var i = interiors.Facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);
            foreach (var id in ids.OrderBy(value => value, StringComparer.Ordinal))
            {
                if (!p.ContainsKey(id))
                {
                    throw new CityInvariantException(
                        "city.interior_programme_projection_missing", id,
                        "every CITY-02 I1-I3 subject must remain in the projected programme universe",
                        "Required interior subject is absent from the projected programme slice.",
                        ProgrammeSourcePath);
                }

                var expected = p[id].Fields["interior-depth"].CanonicalValue;
                var actual = i[DepthId(id)].Fields["interior-depth"].CanonicalValue;
                if (!StringComparer.Ordinal.Equals(expected, actual))
                {
                    throw new CityInvariantException(
                        "city.interior_depth_mismatch", id,
                        "CITY-02 I-depth must equal CITY-06 inherited I-depth for every Full I1-I3 allocation",
                        "Projected subject '" + id + "' requires " + expected + " but CITY-06 declares " + actual + ".",
                        p[id].Provenance.SourcePath, i[DepthId(id)].Provenance.SourcePath);
                }
            }
        }

        private static bool HasRole(DesignFact fact, string role)
        {
            return fact.Fields.TryGetValue("required-role-" + role, out var value) &&
                value.Kind == DesignValueKind.Boolean &&
                StringComparer.Ordinal.Equals(value.CanonicalValue, "true");
        }

        private static void AddRoleFields(IDictionary<string, DesignValue> fields, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                fields.Add("required-role-" + role, DesignValue.Boolean(true));
            }
        }

        private static void RequireSameSet(
            IReadOnlyCollection<string> expected,
            IEnumerable<string> actualValues,
            string missingCode,
            string extraCode,
            string rule,
            string expectedSource,
            string actualSource)
        {
            var actual = new SortedSet<string>(actualValues, StringComparer.Ordinal);
            foreach (var id in expected.OrderBy(value => value, StringComparer.Ordinal))
            {
                if (!actual.Contains(id))
                {
                    throw new CityInvariantException(
                        missingCode, id, rule,
                        "Required subject '" + id + "' is absent from the downstream accepted source.",
                        expectedSource, actualSource);
                }
            }

            var expectedSet = new HashSet<string>(expected, StringComparer.Ordinal);
            foreach (var id in actual)
            {
                if (!expectedSet.Contains(id))
                {
                    throw new CityInvariantException(
                        extraCode, id, rule,
                        "Downstream subject '" + id + "' is outside the independently derived upstream universe.",
                        expectedSource, actualSource);
                }
            }
        }

        private static Model Parse(string programmeSource, string bindingSource, string interiorsSource)
        {
            if (programmeSource == null) throw new ArgumentNullException(nameof(programmeSource));
            if (bindingSource == null) throw new ArgumentNullException(nameof(bindingSource));
            if (interiorsSource == null) throw new ArgumentNullException(nameof(interiorsSource));

            var programme = new SortedDictionary<string, ProgrammeRow>(StringComparer.Ordinal);
            foreach (var row in ParseTable(
                programmeSource, "## 4. District × location programme", "Programme ID", 10, ProgrammeSourcePath))
            {
                var id = Clean(row.Cells[0]);
                AddUnique(programme, id, new ProgrammeRow(
                    id, Clean(row.Cells[5]), InteriorDepth(row.Cells[7], id, ProgrammeSourcePath),
                    ProgrammeRoles(row.Cells[8]), row.Anchor),
                    "city.programme_subject_duplicate", ProgrammeSourcePath);
            }

            var accessIds = new SortedSet<string>(
                programme.Values.Where(row => row.Importance == "A" || row.Importance == "B").Select(row => row.Id),
                StringComparer.Ordinal);
            var interiorIds = new SortedSet<string>(
                programme.Values.Where(row => row.InteriorDepth != "I0").Select(row => row.Id),
                StringComparer.Ordinal);
            if (accessIds.Count == 0 || interiorIds.Count == 0)
            {
                throw Shape("CITY-02 invariant universe is empty.", ProgrammeSourcePath);
            }

            var bindings = new SortedDictionary<string, BindingRow>(StringComparer.Ordinal);
            foreach (var row in ParseTable(
                bindingSource, "## 9. A/B place → exterior composition mapping", "CITY-02 place", 6, BindingSourcePath))
            {
                var id = Clean(row.Cells[0]);
                AddUnique(bindings, id, new BindingRow(id, BindingRoles(row.Cells[4], id), row.Anchor),
                    "city.access_binding_subject_duplicate", BindingSourcePath);
            }

            var depths = new SortedDictionary<string, DepthRow>(StringComparer.Ordinal);
            foreach (var row in ParseTable(
                interiorsSource, "## 5. Inherited access roles inside interiors", "Place", 4, InteriorsSourcePath))
            {
                var id = Clean(row.Cells[0]);
                AddUnique(depths, id, new DepthRow(id, InteriorDepth(row.Cells[1], id, InteriorsSourcePath), row.Anchor),
                    "city.interior_depth_subject_duplicate", InteriorsSourcePath);
            }

            var allocations = new SortedDictionary<string, AllocationRow>(StringComparer.Ordinal);
            foreach (var row in ParseTable(
                interiorsSource, "## 6. Full I1–I3 allocation", "Place", 5, InteriorsSourcePath))
            {
                var id = Clean(row.Cells[0]);
                AddUnique(allocations, id, new AllocationRow(id, row.Anchor),
                    "city.interior_allocation_duplicate", InteriorsSourcePath);
            }

            return new Model(
                programmeSource, bindingSource, interiorsSource,
                programme, bindings, depths, allocations, accessIds, interiorIds);
        }

        private static IReadOnlyList<TableRow> ParseTable(
            string source, string heading, string firstHeader, int columns, string sourcePath)
        {
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var headingIndex = Array.FindIndex(lines, line => line.Trim() == heading);
            if (headingIndex < 0)
            {
                throw Shape("Required heading '" + heading + "' is absent.", sourcePath);
            }

            var headerIndex = -1;
            for (var index = headingIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (line.StartsWith("## ", StringComparison.Ordinal)) break;
                if (!line.StartsWith("|", StringComparison.Ordinal)) continue;
                var cells = SplitRow(line, sourcePath);
                if (cells.Count > 0 && Clean(cells[0]) == firstHeader)
                {
                    headerIndex = index;
                    break;
                }
            }

            if (headerIndex < 0 || headerIndex + 1 >= lines.Length)
            {
                throw Shape("Required table '" + firstHeader + "' is absent.", sourcePath);
            }

            if (SplitRow(lines[headerIndex].Trim(), sourcePath).Count != columns ||
                lines[headerIndex + 1].IndexOf("---", StringComparison.Ordinal) < 0)
            {
                throw Shape("Required table '" + firstHeader + "' changed shape.", sourcePath);
            }

            var rows = new List<TableRow>();
            for (var index = headerIndex + 2; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (!line.StartsWith("|", StringComparison.Ordinal)) break;
                var cells = SplitRow(line, sourcePath);
                if (cells.Count != columns)
                {
                    throw Shape("A row in '" + firstHeader + "' changed column count.", sourcePath);
                }
                rows.Add(new TableRow(cells, line));
            }

            if (rows.Count == 0)
            {
                throw Shape("Required table '" + firstHeader + "' has no data rows.", sourcePath);
            }

            return rows.AsReadOnly();
        }

        private static IReadOnlyList<string> SplitRow(string row, string sourcePath)
        {
            if (row.Length < 2 || row[0] != '|' || row[row.Length - 1] != '|')
            {
                throw Shape("CITY table row must start and end with '|'.", sourcePath);
            }

            return row.Substring(1, row.Length - 2).Split('|')
                .Select(cell => cell.Trim()).ToList().AsReadOnly();
        }

        private static SortedSet<string> ProgrammeRoles(string raw)
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

        private static SortedSet<string> BindingRoles(string raw, string id)
        {
            var text = Clean(raw);
            if (text.Length < 2 || text[0] != '{' || text[text.Length - 1] != '}')
            {
                throw Shape("CITY-05 role set for '" + id + "' is not braced.", BindingSourcePath);
            }

            var roles = new SortedSet<string>(StringComparer.Ordinal);
            var body = text.Substring(1, text.Length - 2).Trim();
            if (body.Length == 0) return roles;
            foreach (var part in body.Split(','))
            {
                var role = part.Trim();
                if (!AllowedRoles.Contains(role, StringComparer.Ordinal) || !roles.Add(role))
                {
                    throw Shape("CITY-05 role set for '" + id + "' contains an invalid/duplicate role.", BindingSourcePath);
                }
            }
            return roles;
        }

        private static string InteriorDepth(string raw, string id, string sourcePath)
        {
            var value = Clean(raw);
            foreach (var depth in new[] { "I0", "I1", "I2", "I3" })
            {
                if (value == depth || value.StartsWith(depth + " ", StringComparison.Ordinal)) return depth;
            }
            throw Shape("Subject '" + id + "' has unsupported interior depth '" + value + "'.", sourcePath);
        }

        private static void AddUnique<T>(
            IDictionary<string, T> target, string id, T value, string code, string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(id)) throw Shape("CITY table contains an empty subject id.", sourcePath);
            if (target.ContainsKey(id))
            {
                throw new CityInvariantException(
                    code, id, "each bounded CITY invariant subject must occur exactly once",
                    "Subject '" + id + "' occurs more than once.", sourcePath);
            }
            target.Add(id, value);
        }

        private static string Clean(string value) =>
            value.Replace("**", string.Empty).Replace("`", string.Empty).Trim();

        private static CityInvariantException Shape(string detail, string path) =>
            new CityInvariantException(
                "city.source_shape_invalid", string.Empty,
                "bounded accepted CITY Markdown shape must remain mechanically parseable",
                detail, path);

        private static IEnumerable<DesignRelation> PreserveInteriorRelations(
            string subjectId,
            IReadOnlyList<DesignRelation> relations) => relations;

        private static string DepthId(string id) => "depth." + id;
        private static string AllocationId(string id) => "allocation." + id;

        private sealed class ProjectionContext
        {
            public ProjectionContext(
                DesignWorldProjection projection,
                IDesignAuthorityUniverse universe,
                IDesignAuthorityReader reader)
            {
                Projection = projection;
                Universe = universe;
                Reader = reader;
            }
            public DesignWorldProjection Projection { get; }
            public IDesignAuthorityUniverse Universe { get; }
            public IDesignAuthorityReader Reader { get; }
        }

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
            public ProgrammeRow(string id, string importance, string depth, SortedSet<string> roles, string anchor)
            {
                Id = id; Importance = importance; InteriorDepth = depth; Roles = roles; Anchor = anchor;
            }
            public string Id { get; }
            public string Importance { get; }
            public string InteriorDepth { get; }
            public SortedSet<string> Roles { get; }
            public string Anchor { get; }
        }

        private sealed class BindingRow
        {
            public BindingRow(string id, SortedSet<string> roles, string anchor)
            {
                Id = id; Roles = roles; Anchor = anchor;
            }
            public string Id { get; }
            public SortedSet<string> Roles { get; }
            public string Anchor { get; }
        }

        private sealed class DepthRow
        {
            public DepthRow(string id, string depth, string anchor)
            {
                Id = id; InteriorDepth = depth; Anchor = anchor;
            }
            public string Id { get; }
            public string InteriorDepth { get; }
            public string Anchor { get; }
        }

        private sealed class AllocationRow
        {
            public AllocationRow(string id, string anchor) { Id = id; Anchor = anchor; }
            public string Id { get; }
            public string Anchor { get; }
        }

        private sealed class Model
        {
            public Model(
                string programmeSource, string bindingSource, string interiorsSource,
                SortedDictionary<string, ProgrammeRow> programme,
                SortedDictionary<string, BindingRow> bindings,
                SortedDictionary<string, DepthRow> depths,
                SortedDictionary<string, AllocationRow> allocations,
                SortedSet<string> accessIds, SortedSet<string> interiorIds)
            {
                ProgrammeSource = programmeSource;
                BindingSource = bindingSource;
                InteriorsSource = interiorsSource;
                Programme = programme;
                Bindings = bindings;
                InteriorDepths = depths;
                Allocations = allocations;
                AccessIds = accessIds;
                InteriorIds = interiorIds;
            }
            public string ProgrammeSource { get; }
            public string BindingSource { get; }
            public string InteriorsSource { get; }
            public SortedDictionary<string, ProgrammeRow> Programme { get; }
            public SortedDictionary<string, BindingRow> Bindings { get; }
            public SortedDictionary<string, DepthRow> InteriorDepths { get; }
            public SortedDictionary<string, AllocationRow> Allocations { get; }
            public SortedSet<string> AccessIds { get; }
            public SortedSet<string> InteriorIds { get; }
        }
    }
}
