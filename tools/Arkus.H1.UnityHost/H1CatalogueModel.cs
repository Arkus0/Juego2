using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Arkus.Harness.Protocol;
using Arkus.Harness.Projection;

namespace Arkus.H1.UnityHost
{
    public sealed class H1CatalogueException : Exception
    {
        public H1CatalogueException(string code, string message) : base(message) { Code = code; }
        public string Code { get; }
    }

    public sealed class H1CatalogueEffectiveInventory
    {
        public string SchemaId { get; set; } = "";
        public string ProjectIdentity { get; set; } = "";
        public string EditorVersion { get; set; } = "";
        public List<H1CatalogueEffectiveRow> Rows { get; set; } = new List<H1CatalogueEffectiveRow>();
    }

    public sealed class H1CatalogueEffectiveRow
    {
        public string Kind { get; set; } = "";
        public string Path { get; set; } = "";
        public string NativeGuid { get; set; } = "";
        public string LocalFileId { get; set; } = "";
        public string Name { get; set; } = "";
        public string TypeName { get; set; } = "";
        public string Dimensions { get; set; } = "";
        public string ContentSha256 { get; set; } = "";
        public string[] Dependencies { get; set; } = Array.Empty<string>();
        public bool Compatible { get; set; }
    }

    public sealed class H1CatalogueMapping
    {
        public string SchemaId { get; set; } = "";
        public string ProjectIdentity { get; set; } = "";
        public List<H1CatalogueMappingRow> Entries { get; set; } = new List<H1CatalogueMappingRow>();
    }

    public sealed class H1CatalogueMappingRow
    {
        public string LogicalId { get; set; } = "";
        public string Kind { get; set; } = "";
        public string NativeGuid { get; set; } = "";
        public string LocalFileId { get; set; } = "";
        public string TypeName { get; set; } = "";
        public string SourceId { get; set; } = "";
        public string AdoptionStatus { get; set; } = "";
        public string ContentSha256 { get; set; } = "";
    }

    public sealed class H1CatalogueEntry
    {
        public string LogicalId { get; init; } = "";
        public string Kind { get; init; } = "";
        public string Name { get; init; } = "";
        public string TypeName { get; init; } = "";
        public string Dimensions { get; init; } = "";
        public string SourceId { get; init; } = "";
        public string AdoptionStatus { get; init; } = "";
        public bool Compatible { get; init; }
        public string Path { get; init; } = "";
        public string NativeGuid { get; init; } = "";
        public string LocalFileId { get; init; } = "";
        public string ContentSha256 { get; init; } = "";
        public IReadOnlyList<string> Dependencies { get; init; } = Array.Empty<string>();

        public IReadOnlyDictionary<string, object?> ToData() => new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["schemaId"] = "arkus.h1-catalogue-entry@1",
            ["logicalId"] = LogicalId, ["kind"] = Kind, ["name"] = Name,
            ["typeName"] = TypeName, ["dimensions"] = Dimensions,
            ["sourceId"] = SourceId, ["adoptionStatus"] = AdoptionStatus,
            ["compatible"] = Compatible, ["path"] = Path,
            ["nativeGuid"] = NativeGuid, ["localFileId"] = LocalFileId,
            ["contentSha256"] = ContentSha256,
            ["dependencies"] = Dependencies.Cast<object?>().ToArray()
        });
    }

    public sealed class H1CatalogueSnapshot
    {
        public const string MappingSchema = "arkus.h1-catalogue-mapping@1";
        public const string InventorySchema = "arkus.h1-catalogue-effective-inventory@1";
        public const string SnapshotSchema = "arkus.h1-catalogue-snapshot@1";
        public const int MaximumEntries = 512;
        public const int MaximumPageSize = 64;
        public const string MappingRelativePath = "Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false
        };
        private static readonly HashSet<string> Kinds = new HashSet<string>(new[]
        {
            "scene", "asset", "prefab", "material", "animation-clip", "component-schema"
        }, StringComparer.Ordinal);
        private static readonly HashSet<string> ComponentTypes = new HashSet<string>(new[]
        {
            "UnityEngine.Animator", "UnityEngine.MeshRenderer", "UnityEngine.Transform"
        }, StringComparer.Ordinal);
        private readonly IReadOnlyDictionary<string, H1CatalogueEntry> _byId;

        private H1CatalogueSnapshot(IReadOnlyList<H1CatalogueEntry> entries, string fingerprint)
        {
            Entries = entries;
            Fingerprint = fingerprint;
            _byId = new ReadOnlyDictionary<string, H1CatalogueEntry>(entries.ToDictionary(entry => entry.LogicalId, StringComparer.Ordinal));
        }

        public IReadOnlyList<H1CatalogueEntry> Entries { get; }
        public string Fingerprint { get; }
        public long SnapshotToken => long.Parse(Fingerprint.Substring(0, 15), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        public static H1CatalogueSnapshot Build(string effectiveJson, string mappingJson)
        {
            H1CatalogueEffectiveInventory effective;
            H1CatalogueMapping mapping;
            try
            {
                effective = JsonSerializer.Deserialize<H1CatalogueEffectiveInventory>(effectiveJson, JsonOptions) ?? throw new JsonException();
                mapping = JsonSerializer.Deserialize<H1CatalogueMapping>(mappingJson, JsonOptions) ?? throw new JsonException();
            }
            catch (JsonException) { throw Error("catalogue.invalid-document", "Effective inventory or mapping is not valid versioned JSON."); }
            if (effective.SchemaId != InventorySchema || mapping.SchemaId != MappingSchema ||
                effective.ProjectIdentity != UnityProjectWorkspaceAuthority.ProjectIdentity ||
                mapping.ProjectIdentity != effective.ProjectIdentity ||
                effective.EditorVersion != H1UnityLaunchProfile.EditorVersion)
                throw Error("catalogue.identity-mismatch", "The inventory, mapping, project or editor identity disagrees.");
            if (effective.Rows == null || mapping.Entries == null || effective.Rows.Count > MaximumEntries || mapping.Entries.Count > MaximumEntries)
                throw Error("catalogue.bound-exceeded", "The declared catalogue slice exceeded its fixed entry ceiling.");

            var byNative = new Dictionary<string, H1CatalogueEffectiveRow>(StringComparer.Ordinal);
            var observedTypes = new HashSet<string>(StringComparer.Ordinal);
            foreach (var row in effective.Rows)
            {
                ValidateEffective(row);
                if (row.Kind == "component-schema") observedTypes.Add(row.TypeName);
                if (!byNative.TryAdd(NativeKey(row.Kind, row.NativeGuid, row.LocalFileId, row.TypeName), row))
                    throw Error("catalogue.duplicate-native-identity", "Effective inventory contains two admitted entries with one native identity.");
            }
            if (!observedTypes.SetEquals(ComponentTypes))
                throw Error("catalogue.component-universe-mismatch", "The reviewed component type universe is incomplete.");

            var mappedNative = new HashSet<string>(StringComparer.Ordinal);
            var logicalIds = new HashSet<string>(StringComparer.Ordinal);
            var entries = new List<H1CatalogueEntry>();
            foreach (var row in mapping.Entries)
            {
                if (row == null || !CanonicalIdentityRules.IsCanonicalIdentifier(row.LogicalId) || !Kinds.Contains(row.Kind))
                    throw Error("catalogue.invalid-mapping", "Mapping contains an invalid logical identity or kind.");
                if (!logicalIds.Add(row.LogicalId)) throw Error("catalogue.duplicate-logical-id", "Two mapping rows claim one Arkus logical ID.");
                var nativeKey = NativeKey(row.Kind, row.NativeGuid, row.LocalFileId, row.TypeName);
                if (!mappedNative.Add(nativeKey)) throw Error("catalogue.duplicate-mapping", "Two logical IDs claim one effective native identity.");
                if (!byNative.TryGetValue(nativeKey, out var found))
                    throw Error("catalogue.stale-mapping", "A mapped native identity is missing from effective Unity inventory: " + row.LogicalId);
                if (row.TypeName != found.TypeName || row.ContentSha256 != found.ContentSha256)
                    throw Error("catalogue.incompatible-mapping", "A mapped source type or content fingerprint changed: " + row.LogicalId);
                ValidateAdoption(row, found);
                entries.Add(new H1CatalogueEntry
                {
                    LogicalId = row.LogicalId, Kind = row.Kind, Name = found.Name,
                    TypeName = found.TypeName, Dimensions = found.Dimensions,
                    SourceId = row.SourceId, AdoptionStatus = row.AdoptionStatus,
                    Compatible = found.Compatible, Path = found.Path,
                    NativeGuid = found.NativeGuid, LocalFileId = found.LocalFileId,
                    ContentSha256 = found.ContentSha256,
                    Dependencies = Array.AsReadOnly(found.Dependencies.OrderBy(value => value, StringComparer.Ordinal).ToArray())
                });
            }
            if (mappedNative.Count != byNative.Count)
                throw Error("catalogue.unmapped-effective-entry", "AssetDatabase or reviewed TypeCache exposes an entry without an Arkus mapping.");

            entries.Sort((left, right) => StringComparer.Ordinal.Compare(left.LogicalId, right.LogicalId));
            return new H1CatalogueSnapshot(entries.AsReadOnly(), FingerprintOf(entries));
        }

        public IReadOnlyDictionary<string, object?> Query(string kind, int pageSize, int offset, long? expectedSnapshotToken = null)
        {
            if (pageSize < 1 || pageSize > MaximumPageSize) throw Error("catalogue.page-bound", "Page size is outside the fixed 1..64 bound.");
            if (!string.IsNullOrEmpty(kind) && !Kinds.Contains(kind)) throw Error("catalogue.unknown-kind", "Requested catalogue kind is not admitted.");
            var source = Entries.Where(entry => string.IsNullOrEmpty(kind) || entry.Kind == kind).ToArray();
            if (offset < 0 || offset > source.Length) throw Error("catalogue.page-bound", "Offset is outside the effective scoped inventory.");
            if (expectedSnapshotToken.HasValue && expectedSnapshotToken.Value != SnapshotToken)
                throw Error("catalogue.stale-snapshot", "A previous page belongs to a different effective catalogue snapshot.");
            var page = source.Skip(offset).Take(pageSize).Select(entry => (object?)entry.ToData()).ToArray();
            var next = offset + page.Length;
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = SnapshotSchema, ["fingerprint"] = Fingerprint,
                ["snapshotToken"] = SnapshotToken, ["total"] = source.Length,
                ["entries"] = page, ["nextOffset"] = next
            });
        }

        public IReadOnlyDictionary<string, object?> Get(string logicalId, string kind, bool requireCompatible)
        {
            if (!CanonicalIdentityRules.IsCanonicalIdentifier(logicalId) || !Kinds.Contains(kind))
                throw Error("catalogue.invalid-reference", "Catalogue reference has invalid logical identity or kind.");
            if (!_byId.TryGetValue(logicalId, out var entry))
                throw Error("catalogue.missing-reference", "The referenced Arkus logical ID is not in the effective catalogue.");
            if (entry.Kind != kind) throw Error("catalogue.incompatible-reference", "The referenced kind disagrees with the effective entry.");
            if (requireCompatible && !entry.Compatible)
                throw Error("catalogue.incompatible-reference", "The referenced Unity entry is not compatible with the effective project.");
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = "arkus.h1-catalogue-get@1", ["fingerprint"] = Fingerprint,
                ["entry"] = entry.ToData()
            });
        }

        private static void ValidateEffective(H1CatalogueEffectiveRow row)
        {
            if (row == null || !Kinds.Contains(row.Kind) || row.Dependencies == null ||
                row.Name == null || row.Dimensions == null || row.TypeName == null)
                throw Error("catalogue.invalid-effective-entry", "Effective Unity emitted a malformed admitted entry.");
            if (row.Kind == "component-schema")
            {
                if (!ComponentTypes.Contains(row.TypeName) || row.Path != "type:" + row.TypeName || row.NativeGuid != "" || row.LocalFileId != "0" || row.ContentSha256 != "")
                    throw Error("catalogue.invalid-component-schema", "The component schema locator is outside the reviewed TypeCache slice.");
                return;
            }
            var slash = row.Path.LastIndexOf('/');
            var parent = slash < 0 ? "" : row.Path.Substring(0, slash);
            if (parent != "Assets/Arkus/H1/SourceSlice" && parent != "Assets/Arkus/H1/CatalogueProof")
                throw Error("catalogue.scope-escape", "Effective entry is outside the reviewed immediate-file roots.");
            if (row.NativeGuid.Length != 32 || !long.TryParse(row.LocalFileId, NumberStyles.Integer, CultureInfo.InvariantCulture, out _) ||
                row.ContentSha256.Length != 64)
                throw Error("catalogue.native-identity-missing", "Effective asset has no complete Unity locator or content fingerprint.");
        }

        private static void ValidateAdoption(H1CatalogueMappingRow mapped, H1CatalogueEffectiveRow observed)
        {
            var source = observed.Path.StartsWith("Assets/Arkus/H1/SourceSlice/", StringComparison.Ordinal);
            if (mapped.Kind == "component-schema")
            {
                if (mapped.SourceId != "unity-builtin" || mapped.AdoptionStatus != "builtin")
                    throw Error("catalogue.adoption-missing", "A component schema must state its reviewed built-in origin.");
            }
            else if (!source)
            {
                if (mapped.SourceId != "arkus-harness-proof" || mapped.AdoptionStatus != "harness-proof")
                    throw Error("catalogue.substitute-source", "Repository proof objects may not masquerade as approved game art.");
            }
            else
            {
                var expectedSource = observed.Path.EndsWith("UAL1.fbx", StringComparison.Ordinal)
                    ? "quaternius-ual1-source" : "quaternius-medieval-source";
                var expectedStatus = observed.Path.EndsWith("FacadeImportedMaterial.mat", StringComparison.Ordinal)
                    ? "source-derived" : "approved-source";
                if (mapped.SourceId != expectedSource || mapped.AdoptionStatus != expectedStatus)
                    throw Error("catalogue.adoption-missing", "Game-representative source identity or approval status is missing or ambiguous.");
            }
        }

        private static string NativeKey(string kind, string guid, string fileId, string typeName) =>
            kind + "|" + (kind == "component-schema" ? typeName : guid + "|" + fileId);

        private static string FingerprintOf(IEnumerable<H1CatalogueEntry> entries)
        {
            using var sha = SHA256.Create();
            var builder = new StringBuilder(SnapshotSchema);
            foreach (var entry in entries)
            {
                foreach (var value in new[] { entry.LogicalId, entry.Kind, entry.Name, entry.TypeName, entry.Dimensions,
                    entry.SourceId, entry.AdoptionStatus, entry.Compatible ? "true" : "false", entry.Path,
                    entry.NativeGuid, entry.LocalFileId, entry.ContentSha256 }) Append(builder, value);
                foreach (var dependency in entry.Dependencies) Append(builder, dependency);
                Append(builder, "<end>");
            }
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()))).ToLowerInvariant();
        }

        private static void Append(StringBuilder builder, string value) => builder.Append(value.Length.ToString(CultureInfo.InvariantCulture)).Append(':').Append(value);
        private static H1CatalogueException Error(string code, string message) => new H1CatalogueException(code, message);
        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> data) =>
            new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(data, StringComparer.Ordinal));
    }
}
