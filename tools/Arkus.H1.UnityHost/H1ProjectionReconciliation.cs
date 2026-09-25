using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Arkus.EngineBridge.UnityAuthoring;

namespace Arkus.H1.UnityHost
{
    public sealed class H1ProjectionObservationDiagnostic
    {
        public string Code { get; set; } = "";
        public string Subject { get; set; } = "";
    }

    // Deliberately distinct from the strict H1-05/H1-08 observation. This shape is read-only
    // and may describe drift that the strict observer correctly rejects.
    public sealed class H1ProjectionReconciliationObservation
    {
        public const string Schema = "arkus.h1-projection-reconciliation-observation@1";
        public string SchemaId { get; set; } = Schema;
        public string SceneLogicalId { get; set; } = H1ManagedScenePlan.SceneId;
        public bool Active { get; set; }
        public string GenerationId { get; set; } = "";
        public string InputDigest { get; set; } = "";
        public string CanonicalHash { get; set; } = "";
        public string CatalogueFingerprint { get; set; } = "";
        public string ManifestGraphDigest { get; set; } = "";
        public string ManifestRealizationDigest { get; set; } = "";
        public string GraphDigest { get; set; } = "";
        public string RealizationDigest { get; set; } = "";
        public string ManagedDigest { get; set; } = "";
        public H1ObservedSceneNode[] Nodes { get; set; } = Array.Empty<H1ObservedSceneNode>();
        public string[] UnmanagedPaths { get; set; } = Array.Empty<string>();
        public H1ProjectionObservationDiagnostic[] Diagnostics { get; set; } = Array.Empty<H1ProjectionObservationDiagnostic>();
    }

    public static class H1ProjectionDriftClass
    {
        public const string Missing = "missing";
        public const string Extra = "extra";
        public const string Changed = "changed";
        public const string Ambiguous = "ambiguous";
        public const string CatalogueDrift = "catalogue-drift";
        public const string CanonicalAhead = "canonical-ahead";
    }

    public sealed class H1ProjectionDriftItem
    {
        public string Classification { get; set; } = "";
        public string ObjectId { get; set; } = "";
        public bool Managed { get; set; }
        public string[] Fields { get; set; } = Array.Empty<string>();
    }

    public sealed class H1ProjectionDriftReport
    {
        public const string Schema = "arkus.h1-projection-drift-report@1";
        public string SchemaId { get; set; } = Schema;
        public string SceneLogicalId { get; set; } = H1ManagedScenePlan.SceneId;
        public string State { get; set; } = "";
        public bool Parity { get; set; }
        public long ExpectedRevision { get; set; }
        public string ExpectedCanonicalHash { get; set; } = "";
        public string ExpectedInputDigest { get; set; } = "";
        public string ExpectedCatalogueFingerprint { get; set; } = "";
        public string EffectiveCanonicalHash { get; set; } = "";
        public string EffectiveInputDigest { get; set; } = "";
        public string EffectiveCatalogueFingerprint { get; set; } = "";
        public string ManagedDigest { get; set; } = "";
        public H1ProjectionDriftItem[] Items { get; set; } = Array.Empty<H1ProjectionDriftItem>();
        public H1ProjectionObservationDiagnostic[] Diagnostics { get; set; } = Array.Empty<H1ProjectionObservationDiagnostic>();
    }

    public sealed class H1ProjectionImportProposal
    {
        public const string Schema = "arkus.h1-projection-import-proposal@1";
        public string SchemaId { get; set; } = Schema;
        public string SceneLogicalId { get; set; } = H1ManagedScenePlan.SceneId;
        public bool Available { get; set; }
        public string[] ObjectIds { get; set; } = Array.Empty<string>();
        public string[] Diagnostics { get; set; } = Array.Empty<string>();
        public IReadOnlyDictionary<string, object?>? MutationRequest { get; set; }
    }

    public static class H1ProjectionReconciliation
    {
        private static readonly string[] GloballyBlockingClasses =
        {
            H1ProjectionDriftClass.Ambiguous,
            H1ProjectionDriftClass.CatalogueDrift,
            H1ProjectionDriftClass.CanonicalAhead
        };

        public static H1ProjectionDriftReport Compare(H1ManagedScenePlan expected, H1ProjectionReconciliationObservation effective)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (effective == null) throw new ArgumentNullException(nameof(effective));
            if (expected.SceneLogicalId != H1ManagedScenePlan.SceneId || effective.SceneLogicalId != H1ManagedScenePlan.SceneId ||
                effective.SchemaId != H1ProjectionReconciliationObservation.Schema)
                throw new H1ProjectionException("projection.reconciliation-identity", "Reconciliation input is outside the reviewed managed scene/schema.");

            var items = new List<H1ProjectionDriftItem>();
            var diagnostics = NormalizeDiagnostics(effective.Diagnostics);
            var computedManagedDigest = ManagedDigest(effective.Nodes ?? Array.Empty<H1ObservedSceneNode>());
            if (effective.ManagedDigest.Length != 0 && effective.ManagedDigest != computedManagedDigest)
            {
                diagnostics.Add(new H1ProjectionObservationDiagnostic { Code = "projection.managed-digest-mismatch", Subject = "$scene" });
            }

            if (!effective.Active)
            {
                foreach (var node in expected.Nodes)
                    items.Add(Item(H1ProjectionDriftClass.Missing, node.ObjectId, true, "object"));
            }
            else
            {
                if (effective.CatalogueFingerprint != expected.CatalogueFingerprint)
                    items.Add(Item(H1ProjectionDriftClass.CatalogueDrift, "$scene", true, "catalogueFingerprint"));
                if (effective.CanonicalHash != expected.CanonicalHash || effective.InputDigest != expected.InputDigest)
                {
                    var fields = new List<string>();
                    if (effective.CanonicalHash != expected.CanonicalHash) fields.Add("canonicalHash");
                    if (effective.InputDigest != expected.InputDigest) fields.Add("inputDigest");
                    items.Add(Item(H1ProjectionDriftClass.CanonicalAhead, "$scene", true, fields.ToArray()));
                }

                var expectedById = expected.Nodes.ToDictionary(node => node.ObjectId, StringComparer.Ordinal);
                var actualGroups = (effective.Nodes ?? Array.Empty<H1ObservedSceneNode>())
                    .GroupBy(node => node?.ObjectId ?? "", StringComparer.Ordinal)
                    .ToDictionary(group => group.Key, group => group.Where(node => node != null).ToArray(), StringComparer.Ordinal);

                foreach (var group in actualGroups.Where(pair => pair.Key.Length == 0 || pair.Value.Length != 1))
                    items.Add(Item(H1ProjectionDriftClass.Ambiguous, group.Key.Length == 0 ? "$unknown" : group.Key, true, "objectId"));

                foreach (var node in expected.Nodes)
                {
                    if (!actualGroups.TryGetValue(node.ObjectId, out var rows) || rows.Length == 0)
                    {
                        items.Add(Item(H1ProjectionDriftClass.Missing, node.ObjectId, true, "object"));
                        continue;
                    }
                    if (rows.Length != 1) continue;
                    var changed = ChangedFields(node, rows[0]);
                    if (changed.Length != 0) items.Add(Item(H1ProjectionDriftClass.Changed, node.ObjectId, true, changed));
                }

                foreach (var pair in actualGroups.Where(pair => pair.Key.Length != 0 && !expectedById.ContainsKey(pair.Key)))
                {
                    if (pair.Value.Length == 1) items.Add(Item(H1ProjectionDriftClass.Extra, pair.Key, true, "object"));
                }

                // Manifest digests are accepted H1-05 baseline facts. If effective realization changed in a
                // way not represented by the allowlisted node facts, keep the drift visible rather than
                // falsely declaring parity.
                if (effective.ManifestGraphDigest.Length != 0 && effective.GraphDigest.Length != 0 &&
                    effective.ManifestGraphDigest != effective.GraphDigest &&
                    !items.Any(item => item.Managed && (item.Classification == H1ProjectionDriftClass.Missing ||
                        item.Classification == H1ProjectionDriftClass.Extra || item.Classification == H1ProjectionDriftClass.Changed ||
                        item.Classification == H1ProjectionDriftClass.Ambiguous)))
                    items.Add(Item(H1ProjectionDriftClass.Changed, "$scene", true, "graphDigest"));
                if (effective.ManifestRealizationDigest.Length != 0 && effective.RealizationDigest.Length != 0 &&
                    effective.ManifestRealizationDigest != effective.RealizationDigest &&
                    !items.Any(item => item.Managed && item.Classification == H1ProjectionDriftClass.Changed && item.Fields.Contains("realization", StringComparer.Ordinal)))
                    items.Add(Item(H1ProjectionDriftClass.Changed, "$scene", true, "realizationDigest"));
            }

            foreach (var path in (effective.UnmanagedPaths ?? Array.Empty<string>()).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal))
                items.Add(Item(H1ProjectionDriftClass.Extra, path, false, "unmanaged"));
            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Code == "projection.duplicate-or-invalid-marker" || diagnostic.Code == "projection.managed-digest-mismatch" ||
                    diagnostic.Code == "projection.reconciliation-node-unreadable" || diagnostic.Code == "projection.root-marker-invalid")
                    items.Add(Item(H1ProjectionDriftClass.Ambiguous, diagnostic.Subject.Length == 0 ? "$scene" : diagnostic.Subject, true, "observation"));
            }

            var ordered = items
                .GroupBy(item => item.Classification + "\u001f" + item.ObjectId + "\u001f" + item.Managed + "\u001f" + string.Join("\u001e", item.Fields), StringComparer.Ordinal)
                .Select(group => group.First())
                .OrderBy(item => Rank(item.Classification))
                .ThenBy(item => item.ObjectId, StringComparer.Ordinal)
                .ThenBy(item => item.Managed ? 0 : 1)
                .ToArray();
            var state = State(ordered);
            return new H1ProjectionDriftReport
            {
                State = state,
                Parity = ordered.Length == 0,
                ExpectedRevision = expected.WorldRevision,
                ExpectedCanonicalHash = expected.CanonicalHash,
                ExpectedInputDigest = expected.InputDigest,
                ExpectedCatalogueFingerprint = expected.CatalogueFingerprint,
                EffectiveCanonicalHash = effective.CanonicalHash,
                EffectiveInputDigest = effective.InputDigest,
                EffectiveCatalogueFingerprint = effective.CatalogueFingerprint,
                ManagedDigest = computedManagedDigest,
                Items = ordered,
                Diagnostics = diagnostics.OrderBy(value => value.Code, StringComparer.Ordinal).ThenBy(value => value.Subject, StringComparer.Ordinal).ToArray()
            };
        }

        public static H1ProjectionImportProposal CompileProposal(
            H1ManagedScenePlan expected,
            H1ProjectionReconciliationObservation effective,
            H1ProjectionDriftReport report,
            H1CatalogueSnapshot catalogue)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (effective == null) throw new ArgumentNullException(nameof(effective));
            if (report == null) throw new ArgumentNullException(nameof(report));
            if (catalogue == null) throw new ArgumentNullException(nameof(catalogue));

            var diagnostics = new SortedSet<string>(StringComparer.Ordinal);
            if (report.Items.Any(item => GloballyBlockingClasses.Contains(item.Classification, StringComparer.Ordinal)))
            {
                foreach (var item in report.Items.Where(item => GloballyBlockingClasses.Contains(item.Classification, StringComparer.Ordinal)))
                    diagnostics.Add("projection.import-blocked-" + item.Classification + ":" + item.ObjectId);
                return Unavailable(diagnostics);
            }

            var actualById = effective.Nodes.GroupBy(node => node.ObjectId, StringComparer.Ordinal)
                .Where(group => group.Count() == 1).ToDictionary(group => group.Key, group => group.Single(), StringComparer.Ordinal);
            var expectedById = expected.Nodes.ToDictionary(node => node.ObjectId, StringComparer.Ordinal);
            var operations = new List<object?>();
            var objectIds = new List<string>();

            foreach (var item in report.Items.OrderBy(value => value.ObjectId, StringComparer.Ordinal))
            {
                if (!item.Managed)
                {
                    diagnostics.Add("projection.import-unmanaged:" + item.ObjectId);
                    continue;
                }
                if (item.Classification != H1ProjectionDriftClass.Changed || item.ObjectId == "$scene")
                {
                    diagnostics.Add("projection.import-unsupported-" + item.Classification + ":" + item.ObjectId);
                    continue;
                }
                if (item.Fields.Any(field => field != "transform" && field != "components"))
                {
                    diagnostics.Add("projection.import-unsupported-fields:" + item.ObjectId + ":" + string.Join(",", item.Fields));
                    continue;
                }
                if (!expectedById.TryGetValue(item.ObjectId, out var planned) || !actualById.TryGetValue(item.ObjectId, out var actual))
                {
                    diagnostics.Add("projection.import-ambiguous-object:" + item.ObjectId);
                    continue;
                }

                if (!TryCompileBinding(expected, planned, actual, catalogue, out var compiled, out var failure))
                {
                    diagnostics.Add(failure + ":" + item.ObjectId);
                    continue;
                }
                operations.Add(compiled!["extensionMutation"]);
                objectIds.Add(item.ObjectId);
            }

            if (operations.Count == 0) return Unavailable(diagnostics);
            objectIds.Sort(StringComparer.Ordinal);
            var idempotencySeed = expected.CanonicalHash + "|" + report.ManagedDigest + "|" + string.Join("|", objectIds);
            var request = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = "h1-09-import-" + H1ManagedScenePlan.Sha(idempotencySeed).Substring(0, 32),
                ["expectedRevision"] = expected.WorldRevision,
                ["expectedHash"] = expected.CanonicalHash,
                ["operations"] = operations.AsReadOnly()
            });
            return new H1ProjectionImportProposal
            {
                Available = true,
                ObjectIds = objectIds.ToArray(),
                Diagnostics = diagnostics.ToArray(),
                MutationRequest = request
            };
        }

        public static string ManagedDigest(IEnumerable<H1ObservedSceneNode> nodes)
        {
            var builder = new StringBuilder();
            foreach (var node in (nodes ?? Array.Empty<H1ObservedSceneNode>()).OrderBy(value => value.ObjectId, StringComparer.Ordinal)
                         .ThenBy(value => value.ParentObjectId, StringComparer.Ordinal).ThenBy(value => value.SourceLogicalId, StringComparer.Ordinal))
            {
                builder.Append(node.ObjectId).Append('|').Append(node.ParentObjectId).Append('|').Append(node.SourceLogicalId).Append('|')
                    .Append(node.SourceKind).Append('|').Append(node.SourcePath).Append('|').Append(node.SourceGuid).Append('|')
                    .Append(node.SourceLocalFileId).Append('|').Append(node.SourceContentSha256).Append('|')
                    .Append(node.RealizationKind).Append('|').Append(node.RealizedPath).Append('|').Append(node.RealizedGuid).Append('|')
                    .Append(node.RealizedLocalFileId).Append('|').Append(node.PrefabGenerationId).Append('|').Append(node.RelationshipDigest).Append('|')
                    .Append(Vec(node.PositionMm)).Append('|').Append(VecNormalized(node.RotationMilliDegrees)).Append('|').Append(Vec(node.ScalePpm)).Append('|');
                foreach (var row in (node.ComponentRows ?? Array.Empty<string>()).OrderBy(value => value, StringComparer.Ordinal)) builder.Append(row).Append('\u001e');
                builder.Append('\n');
            }
            return H1ManagedScenePlan.Sha(builder.ToString());
        }

        internal static string[] ExpectedComponentRows(H1ManagedSceneNode node)
        {
            var rows = new List<string>();
            foreach (var component in node.Components)
            {
                if (component.SchemaId == H1ComponentSchemas.Transform)
                    rows.Add(component.SchemaId + "|positionMm=" + Vec(node.PositionMm) + "|rotationMilliDegrees=" + VecNormalized(node.RotationMilliDegrees) + "|scalePpm=" + Vec(node.ScalePpm));
                else if (component.SchemaId == H1ComponentSchemas.MeshRenderer)
                    rows.Add(component.SchemaId + "|enabled=true|material=" + AssetIdentity(component));
                else if (component.SchemaId == H1ComponentSchemas.Animator)
                    rows.Add(component.SchemaId + "|applyRootMotion=false|updateMode=Normal|cullingMode=AlwaysAnimate|clip=" + AssetIdentity(component));
                else if (component.SchemaId == H1ComponentSchemas.CanonicalLink)
                    rows.Add(component.SchemaId + "|relation=" + component.Relation + "|target=" + component.TargetObjectId);
            }
            rows.Sort(StringComparer.Ordinal);
            return rows.ToArray();
        }

        private static string[] ChangedFields(H1ManagedSceneNode expected, H1ObservedSceneNode actual)
        {
            var fields = new SortedSet<string>(StringComparer.Ordinal);
            if (expected.ParentObjectId != actual.ParentObjectId) fields.Add("hierarchy");
            if (expected.SourceLogicalId != actual.SourceLogicalId || expected.SourceKind != actual.SourceKind ||
                expected.SourcePath != actual.SourcePath || expected.SourceGuid != actual.SourceGuid ||
                expected.SourceLocalFileId != actual.SourceLocalFileId || expected.SourceContentSha256 != actual.SourceContentSha256)
                fields.Add("source");
            if (!Equal(expected.PositionMm, actual.PositionMm) || !EqualNormalized(expected.RotationMilliDegrees, actual.RotationMilliDegrees) ||
                !Equal(expected.ScalePpm, actual.ScalePpm)) fields.Add("transform");
            if (!ExpectedComponentRows(expected).SequenceEqual((actual.ComponentRows ?? Array.Empty<string>()).OrderBy(value => value, StringComparer.Ordinal), StringComparer.Ordinal))
                fields.Add("components");
            if (actual.Relationships == null || actual.Relationships.Length > 512 || actual.RelationshipDigest.Length != 64 ||
                actual.ComponentDigest.Length != 64) fields.Add("realization");
            return fields.ToArray();
        }

        private static bool TryCompileBinding(H1ManagedScenePlan scene, H1ManagedSceneNode expected, H1ObservedSceneNode actual,
            H1CatalogueSnapshot catalogue, out IReadOnlyDictionary<string, object?>? compiled, out string failure)
        {
            compiled = null;
            failure = "projection.import-unsupported-component";
            var expectedSchemas = expected.Components.Select(value => value.SchemaId).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            var rows = (actual.ComponentRows ?? Array.Empty<string>()).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            var rowSchemas = rows.Select(RowSchema).Where(value => value.Length != 0).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            if (!expectedSchemas.SequenceEqual(rowSchemas, StringComparer.Ordinal)) return false;

            var components = new List<object?>();
            foreach (var row in rows)
            {
                var schema = RowSchema(row);
                if (schema == H1ComponentSchemas.Transform) continue;
                if (schema == H1ComponentSchemas.MeshRenderer)
                {
                    const string prefix = H1ComponentSchemas.MeshRenderer + "|enabled=true|material=";
                    if (!row.StartsWith(prefix, StringComparison.Ordinal) || !TryResolveAsset(catalogue, "material", row.Substring(prefix.Length), out var id))
                    { failure = "projection.import-reference-unresolved"; return false; }
                    components.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "renderer", ["materialId"] = id }));
                }
                else if (schema == H1ComponentSchemas.Animator)
                {
                    const string prefix = H1ComponentSchemas.Animator + "|applyRootMotion=false|updateMode=Normal|cullingMode=AlwaysAnimate|clip=";
                    if (!row.StartsWith(prefix, StringComparison.Ordinal) || !TryResolveAsset(catalogue, "animation-clip", row.Substring(prefix.Length), out var id))
                    { failure = "projection.import-reference-unresolved"; return false; }
                    components.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal) { ["kind"] = "animator", ["clipId"] = id }));
                }
                else if (schema == H1ComponentSchemas.CanonicalLink)
                {
                    var prefix = H1ComponentSchemas.CanonicalLink + "|relation=";
                    var marker = "|target=";
                    if (!row.StartsWith(prefix, StringComparison.Ordinal) || row.IndexOf(marker, prefix.Length, StringComparison.Ordinal) is var split && split < 0)
                    { failure = "projection.import-component-row-invalid"; return false; }
                    var at = row.IndexOf(marker, prefix.Length, StringComparison.Ordinal);
                    var relation = row.Substring(prefix.Length, at - prefix.Length);
                    var target = row.Substring(at + marker.Length);
                    if (relation.Length == 0 || target.Length == 0) { failure = "projection.import-component-row-invalid"; return false; }
                    components.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                    { ["kind"] = "canonical-link", ["relation"] = relation, ["targetObjectId"] = target }));
                }
                else { failure = "projection.import-component-schema-unsupported"; return false; }
            }

            var binding = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                ["targetSceneId"] = scene.SceneLogicalId,
                ["source"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                { ["kind"] = expected.SourceKind, ["logicalId"] = expected.SourceLogicalId }),
                ["transform"] = ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                    ["positionMm"] = Vector(actual.PositionMm), ["rotationMilliDegrees"] = Vector(actual.RotationMilliDegrees), ["scalePpm"] = Vector(actual.ScalePpm)
                }),
                ["components"] = components.AsReadOnly()
            });
            try
            {
                compiled = UnityBindingProducer.Compile(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                { ["subjectId"] = expected.ObjectId, ["binding"] = binding }));
                return true;
            }
            catch (UnityBindingException exception)
            {
                failure = exception.MachineCode;
                return false;
            }
        }

        private static bool TryResolveAsset(H1CatalogueSnapshot catalogue, string kind, string identity, out string logicalId)
        {
            logicalId = "";
            var parts = identity.Split('|');
            if (parts.Length != 3) return false;
            var matches = catalogue.Entries.Where(entry => entry.Kind == kind && entry.Path == parts[0] && entry.NativeGuid == parts[1] && entry.LocalFileId == parts[2]).ToArray();
            if (matches.Length != 1) return false;
            logicalId = matches[0].LogicalId;
            return true;
        }

        private static List<H1ProjectionObservationDiagnostic> NormalizeDiagnostics(IEnumerable<H1ProjectionObservationDiagnostic>? source) =>
            (source ?? Array.Empty<H1ProjectionObservationDiagnostic>()).Where(value => value != null)
                .Select(value => new H1ProjectionObservationDiagnostic { Code = value.Code ?? "", Subject = value.Subject ?? "" })
                .GroupBy(value => value.Code + "\u001f" + value.Subject, StringComparer.Ordinal).Select(group => group.First()).ToList();

        private static H1ProjectionImportProposal Unavailable(IEnumerable<string> diagnostics) => new H1ProjectionImportProposal
        { Available = false, Diagnostics = diagnostics.OrderBy(value => value, StringComparer.Ordinal).ToArray(), MutationRequest = null };

        private static H1ProjectionDriftItem Item(string classification, string objectId, bool managed, params string[] fields) => new H1ProjectionDriftItem
        { Classification = classification, ObjectId = objectId, Managed = managed, Fields = fields.OrderBy(value => value, StringComparer.Ordinal).ToArray() };

        private static string State(IReadOnlyList<H1ProjectionDriftItem> items)
        {
            if (items.Any(value => value.Classification == H1ProjectionDriftClass.Ambiguous)) return "ambiguous";
            if (items.Any(value => value.Classification == H1ProjectionDriftClass.CatalogueDrift)) return "missing-dependency";
            if (items.Any(value => value.Classification == H1ProjectionDriftClass.Missing || value.Classification == H1ProjectionDriftClass.Extra || value.Classification == H1ProjectionDriftClass.Changed)) return "engine-drift";
            if (items.Any(value => value.Classification == H1ProjectionDriftClass.CanonicalAhead)) return "canonical-ahead";
            return "in-sync";
        }

        private static int Rank(string classification) => classification switch
        {
            H1ProjectionDriftClass.Missing => 0,
            H1ProjectionDriftClass.Extra => 1,
            H1ProjectionDriftClass.Changed => 2,
            H1ProjectionDriftClass.Ambiguous => 3,
            H1ProjectionDriftClass.CatalogueDrift => 4,
            H1ProjectionDriftClass.CanonicalAhead => 5,
            _ => 99
        };

        private static string RowSchema(string row)
        {
            var index = row.IndexOf('|');
            return index <= 0 ? "" : row.Substring(0, index);
        }
        private static IReadOnlyDictionary<string, object?> Vector(H1ProjectionVector value) => ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
        { ["x"] = value.X, ["y"] = value.Y, ["z"] = value.Z });
        private static IReadOnlyDictionary<string, object?> ReadOnly(Dictionary<string, object?> value) => new ReadOnlyDictionary<string, object?>(value);
        private static string AssetIdentity(H1ComponentPlan component) => component.ReferencePath + "|" + component.ReferenceGuid + "|" + component.ReferenceLocalFileId;
        private static string Vec(H1ProjectionVector value) => value.X + "," + value.Y + "," + value.Z;
        private static string VecNormalized(H1ProjectionVector value) => Normalize(value.X) + "," + Normalize(value.Y) + "," + Normalize(value.Z);
        private static long Normalize(long value) { var result = value % 360000; return result < 0 ? result + 360000 : result; }
        private static bool Equal(H1ProjectionVector left, H1ProjectionVector right) => left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        private static bool EqualNormalized(H1ProjectionVector left, H1ProjectionVector right) => Normalize(left.X) == Normalize(right.X) && Normalize(left.Y) == Normalize(right.Y) && Normalize(left.Z) == Normalize(right.Z);
    }
}
