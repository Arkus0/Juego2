using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1CatalogueTests
    {
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        [Fact]
        public void Effective_universe_reconciles_to_stable_logical_ids_and_deterministic_bounded_pages()
        {
            var (inventory, mapping) = Fixture();
            var first = Build(inventory, mapping);
            var second = Build(inventory, mapping);
            Assert.Equal(first.Fingerprint, second.Fingerprint);
            Assert.Equal(6, first.Entries.Count);
            Assert.DoesNotContain("Assets/", first.Entries.Single(e => e.Kind == "prefab").LogicalId);

            var firstPage = first.Query("", 2, 0);
            var secondPage = first.Query("", 2, (int)firstPage["nextOffset"]!, (long)firstPage["snapshotToken"]!);
            Assert.Equal(6, firstPage["total"]);
            Assert.Equal(2, ((object?[])firstPage["entries"]!).Length);
            Assert.Equal(2, ((object?[])secondPage["entries"]!).Length);
            Assert.Throws<H1CatalogueException>(() => first.Query("", 65, 0));
            Assert.Throws<H1CatalogueException>(() => first.Query("", 2, 2, first.SnapshotToken + 1));

            var wall = first.Get("quaternius.medieval.prefab.wall", "prefab", true);
            Assert.NotNull(wall["entry"]);
            Assert.Equal("catalogue.incompatible-reference", Assert.Throws<H1CatalogueException>(() =>
                first.Get("quaternius.medieval.material.plaster", "material", true)).Code);
            Assert.Equal("catalogue.missing-reference", Assert.Throws<H1CatalogueException>(() =>
                first.Get("quaternius.medieval.prefab.absent", "prefab", true)).Code);
        }

        [Fact]
        public void Move_preserves_logical_identity_but_copy_delete_or_mapping_omission_fail_visible()
        {
            var (inventory, mapping) = Fixture();
            var prefab = inventory.Rows.Single(row => row.Kind == "prefab");
            var baseline = Build(inventory, mapping);
            prefab.Path = "Assets/Arkus/H1/SourceSlice/MovedWall.fbx";
            var moved = Build(inventory, mapping);
            Assert.Equal(baseline.Get("quaternius.medieval.prefab.wall", "prefab", true)["schemaId"],
                moved.Get("quaternius.medieval.prefab.wall", "prefab", true)["schemaId"]);
            Assert.NotEqual(baseline.Fingerprint, moved.Fingerprint);

            inventory.Rows.Add(new H1CatalogueEffectiveRow
            {
                Kind = "prefab", Path = "Assets/Arkus/H1/SourceSlice/CopyWall.fbx", NativeGuid = new string('e', 32),
                LocalFileId = prefab.LocalFileId, Name = prefab.Name, TypeName = prefab.TypeName,
                Dimensions = prefab.Dimensions, ContentSha256 = prefab.ContentSha256, Compatible = true
            });
            Assert.Equal("catalogue.unmapped-effective-entry", Assert.Throws<H1CatalogueException>(() => Build(inventory, mapping)).Code);
            inventory.Rows.RemoveAt(inventory.Rows.Count - 1);

            mapping.Entries.RemoveAll(row => row.Kind == "prefab");
            Assert.Equal("catalogue.unmapped-effective-entry", Assert.Throws<H1CatalogueException>(() => Build(inventory, mapping)).Code);
            mapping.Entries.Add(Map(prefab, "quaternius.medieval.prefab.wall", "quaternius-medieval-source", "approved-source"));
            inventory.Rows.Remove(prefab);
            Assert.Equal("catalogue.stale-mapping", Assert.Throws<H1CatalogueException>(() => Build(inventory, mapping)).Code);
        }

        [Fact]
        public void Native_alias_wrong_source_and_duplicate_logical_id_are_rejected()
        {
            var (inventory, mapping) = Fixture();
            var prefab = mapping.Entries.Single(row => row.Kind == "prefab");
            var material = mapping.Entries.Single(row => row.Kind == "material");
            material.LogicalId = prefab.LogicalId;
            Assert.Equal("catalogue.duplicate-logical-id", Assert.Throws<H1CatalogueException>(() => Build(inventory, mapping)).Code);
            material.LogicalId = "quaternius.medieval.material.plaster";
            material.NativeGuid = prefab.NativeGuid;
            material.LocalFileId = prefab.LocalFileId;
            Assert.Equal("catalogue.stale-mapping", Assert.Throws<H1CatalogueException>(() => Build(inventory, mapping)).Code);
            material.NativeGuid = new string('c', 32);
            material.LocalFileId = "3";
            material.SourceId = "arkus-harness-proof";
            Assert.Equal("catalogue.adoption-missing", Assert.Throws<H1CatalogueException>(() => Build(inventory, mapping)).Code);
        }

        [Fact]
        public void Production_composition_admits_catalogue_only_through_the_existing_unity_host()
        {
            using var projection = ProductionH1UnityHost.Create();
            var names = projection.Capabilities.Select(definition => definition.Key.Name).ToArray();
            Assert.Contains("unity.host.catalogue.query", names);
            Assert.Contains("unity.host.catalogue.get", names);
            Assert.Contains("unity.host.catalogue.resolve", names);
            Assert.Equal(3, names.Count(name => name.StartsWith("unity.host.catalogue.", StringComparison.Ordinal)));
        }

        private static H1CatalogueSnapshot Build(H1CatalogueEffectiveInventory inventory, H1CatalogueMapping mapping) =>
            H1CatalogueSnapshot.Build(JsonSerializer.Serialize(inventory, Json), JsonSerializer.Serialize(mapping, Json));

        private static (H1CatalogueEffectiveInventory, H1CatalogueMapping) Fixture()
        {
            var rows = new List<H1CatalogueEffectiveRow>
            {
                Row("prefab", "Assets/Arkus/H1/SourceSlice/Wall.fbx", new string('b', 32), "2", "Wall", "UnityEngine.GameObject", new string('b', 64), true),
                Row("material", "Assets/Arkus/H1/SourceSlice/MI_Plaster.mat", new string('c', 32), "3", "Plaster", "UnityEngine.Material", new string('c', 64), false),
                Row("scene", "Assets/Arkus/H1/CatalogueProof/Proof.unity", new string('d', 32), "4", "Proof", "UnityEditor.SceneAsset", new string('d', 64), true)
            };
            foreach (var type in new[] { "UnityEngine.Animator", "UnityEngine.MeshRenderer", "UnityEngine.Transform" })
                rows.Add(Row("component-schema", "type:" + type, "", "0", type.Substring(type.LastIndexOf('.') + 1), type, "", true));
            var mapping = new H1CatalogueMapping
            {
                SchemaId = H1CatalogueSnapshot.MappingSchema,
                ProjectIdentity = UnityProjectWorkspaceAuthority.ProjectIdentity,
                Entries = new List<H1CatalogueMappingRow>
                {
                    Map(rows[0], "quaternius.medieval.prefab.wall", "quaternius-medieval-source", "approved-source"),
                    Map(rows[1], "quaternius.medieval.material.plaster", "quaternius-medieval-source", "approved-source"),
                    Map(rows[2], "arkus.h1-04.scene.proof", "arkus-harness-proof", "harness-proof")
                }
            };
            mapping.Entries.AddRange(rows.Skip(3).Select(row => Map(row,
                "unity.component-schema." + row.Name.ToLowerInvariant(), "unity-builtin", "builtin")));
            return (new H1CatalogueEffectiveInventory
            {
                SchemaId = H1CatalogueSnapshot.InventorySchema,
                ProjectIdentity = UnityProjectWorkspaceAuthority.ProjectIdentity,
                EditorVersion = H1UnityLaunchProfile.EditorVersion,
                Rows = rows
            }, mapping);
        }

        private static H1CatalogueEffectiveRow Row(string kind, string path, string guid, string fileId, string name, string type, string sha, bool compatible) =>
            new H1CatalogueEffectiveRow
            {
                Kind = kind, Path = path, NativeGuid = guid, LocalFileId = fileId,
                Name = name, TypeName = type, Dimensions = "effective", ContentSha256 = sha,
                Dependencies = Array.Empty<string>(), Compatible = compatible
            };

        private static H1CatalogueMappingRow Map(H1CatalogueEffectiveRow row, string logicalId, string sourceId, string status) =>
            new H1CatalogueMappingRow
            {
                LogicalId = logicalId, Kind = row.Kind, NativeGuid = row.NativeGuid,
                LocalFileId = row.LocalFileId, TypeName = row.TypeName,
                SourceId = sourceId, AdoptionStatus = status, ContentSha256 = row.ContentSha256
            };
    }
}
