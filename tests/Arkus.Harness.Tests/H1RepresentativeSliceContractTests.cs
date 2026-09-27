using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.H1.UnityHost;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// WP-H1-11 repository-level contract over the committed representative slice. These checks need no private
    /// source bytes or Unity: they bind the H1-11 manifest to the H1-04 admission record and catalogue, and prove the
    /// street-corner scenario covers the complete independently enumerated proof universe through the accepted planner.
    /// </summary>
    public sealed class H1RepresentativeSliceContractTests
    {
        private static readonly string[] H104Slices =
        {
            "Assets/Arkus/H1/SourceSlice/Wall_Plaster_Window_Wide_Flat.fbx",
            "Assets/Arkus/H1/SourceSlice/MI_Plaster.mat",
            "Assets/Arkus/H1/SourceSlice/FacadeImportedMaterial.mat",
            "Assets/Arkus/H1/SourceSlice/UAL1.fbx"
        };

        [Fact]
        public void Every_selected_item_is_admitted_catalogued_and_nothing_is_admitted_silently()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            var manifest = H1RepresentativeSliceScenario.ReadManifest(root);
            var catalogue = H1RepresentativeSliceScenario.CommittedCatalogue(root);
            Assert.InRange(manifest.Items.Count, 10, 15);

            using var adoption = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath)));
            var slices = adoption.RootElement.GetProperty("slices").EnumerateArray().ToDictionary(
                slice => slice.GetProperty("assetPath").GetString()!, slice => slice, StringComparer.Ordinal);

            foreach (var item in manifest.Items)
            {
                Assert.True(slices.TryGetValue(item.AssetPath, out var admitted), "unadmitted item " + item.AssetPath);
                Assert.Equal(item.SourceId, admitted.GetProperty("sourceId").GetString());
                Assert.Equal("approved-source", admitted.GetProperty("adoptionStatus").GetString());
                Assert.Equal(item.ContentSha256, admitted.GetProperty("contentSha256").GetString());
                Assert.Equal(H104Slices.Contains(item.AssetPath, StringComparer.Ordinal), item.Provenance == "h1-04-source-slice");
                Assert.NotEmpty(item.Catalogue);
                foreach (var pair in item.Catalogue)
                {
                    Assert.DoesNotContain("/", pair.Value, StringComparison.Ordinal);
                    var entry = catalogue.Entries.Single(value => value.LogicalId == pair.Value);
                    Assert.Equal(pair.Key, entry.Kind);
                    Assert.Equal(item.SourceId, entry.SourceId);
                    Assert.Equal(item.ContentSha256, entry.ContentSha256);
                    Assert.Equal(item.AssetPath, entry.Path);
                    Assert.True(entry.Compatible, pair.Value + " is not compatible with the effective project");
                }
            }

            // Reverse direction: the admission record may grow only through this manifest.
            var extension = slices.Keys.Where(path => !H104Slices.Contains(path, StringComparer.Ordinal)).OrderBy(path => path, StringComparer.Ordinal);
            var selected = manifest.Items.Where(item => item.Provenance == "distribution-entry").Select(item => item.AssetPath).OrderBy(path => path, StringComparer.Ordinal);
            Assert.Equal(extension, selected);

            // Every catalogue row rooted in an admitted representative file is mapped to that file's logical identities.
            foreach (var entry in catalogue.Entries.Where(value => selected.Contains(value.Path, StringComparer.Ordinal)))
                Assert.Contains(entry.LogicalId, manifest.Items.Single(item => item.AssetPath == entry.Path).Catalogue.Values);

            Assert.Equal(new[] { "idle", "sit", "walk" }, manifest.Clips.Select(clip => clip.Role).OrderBy(role => role, StringComparer.Ordinal));
            foreach (var clip in manifest.Clips)
            {
                var entry = catalogue.Entries.Single(value => value.LogicalId == clip.LogicalId);
                Assert.Equal("animation-clip", entry.Kind);
                Assert.Equal("quaternius-ual1-source", entry.SourceId);
            }
        }

        [Fact]
        public void Street_corner_plan_covers_the_complete_representative_universe_through_the_accepted_planner()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            var manifest = H1RepresentativeSliceScenario.ReadManifest(root);
            var catalogue = H1RepresentativeSliceScenario.CommittedCatalogue(root);
            var session = H1RepresentativeSliceScenario.AuthoredSession();
            var plan = H1ManagedScenePlan.Build(session.Current, catalogue);

            Assert.Equal(H1RepresentativeSliceScenario.Nodes.Count, plan.Nodes.Length);
            Assert.Empty(H1RepresentativeSliceScenario.UncoveredObligations(manifest, plan));
            Assert.True(H1RepresentativeSliceScenario.MaximumDepth(plan) >= 3);
            Assert.Equal(3, plan.Nodes.Count(node => node.SourceLogicalId == "quaternius.ual1.prefab.ual1" &&
                node.Components.Any(component => component.Kind == "animator")));
            Assert.All(plan.Nodes, node => Assert.StartsWith("Assets/Arkus/H1/SourceSlice/", node.SourcePath, StringComparison.Ordinal));
            Assert.Equal(plan.InputDigest, H1ManagedScenePlan.Build(session.Current, catalogue).InputDigest);
        }

        [Fact]
        public void Omitting_any_selected_item_or_clip_from_the_scenario_is_detected()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            var manifest = H1RepresentativeSliceScenario.ReadManifest(root);
            var catalogue = H1RepresentativeSliceScenario.CommittedCatalogue(root);

            // Substitute every use of one selected item by another selected mesh: the omission must be named.
            foreach (var item in manifest.Items)
            {
                var substitute = item.Catalogue.Values.Contains(H1RepresentativeSliceScenario.CrateMesh)
                    ? "quaternius.medieval.asset.prop-woodenfence-single"
                    : H1RepresentativeSliceScenario.CrateMesh;
                var nodes = H1RepresentativeSliceScenario.Nodes.Select(node => item.Catalogue.Values.Contains(node.SourceId)
                    ? node.WithSource("asset", substitute)
                    : node);
                var plan = H1ManagedScenePlan.Build(H1RepresentativeSliceScenario.InitialState(nodes), catalogue);
                Assert.Contains("item:" + item.AssetPath, H1RepresentativeSliceScenario.UncoveredObligations(manifest, plan));
            }

            // Dropping one clip binding must be named as well.
            foreach (var clip in manifest.Clips)
            {
                var nodes = H1RepresentativeSliceScenario.Nodes.Select(node => node.ClipId == clip.LogicalId
                    ? new H1RepresentativeSliceScenario.NodeSpec(node.ObjectId, node.Parent, node.SourceKind, node.SourceId,
                        node.PositionMm, node.Yaw, node.ScalePpm, node.MaterialId, null, node.Link)
                    : node);
                var plan = H1ManagedScenePlan.Build(H1RepresentativeSliceScenario.InitialState(nodes), catalogue);
                Assert.Contains("clip:" + clip.Role, H1RepresentativeSliceScenario.UncoveredObligations(manifest, plan));
            }
        }

        [Fact]
        public void Logical_identity_replaced_by_an_asset_path_or_unknown_item_never_resolves()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            var catalogue = H1RepresentativeSliceScenario.CommittedCatalogue(root);
            var crate = H1RepresentativeSliceScenario.Nodes.Single(node => node.ObjectId == "crate.stack");

            var pathAsIdentity = Assert.Throws<UnityBindingException>(() =>
                H1RepresentativeSliceScenario.Binding(crate.WithSource("asset", "Assets/Arkus/H1/SourceSlice/Prop_Crate.fbx")));
            Assert.StartsWith("unity.binding.", pathAsIdentity.MachineCode, StringComparison.Ordinal);

            var unknown = H1RepresentativeSliceScenario.Nodes.Select(node => node.ObjectId == "crate.stack"
                ? node.WithSource("asset", "quaternius.medieval.asset.prop-lamp")
                : node);
            var missing = Assert.Throws<H1ProjectionException>(() =>
                H1ManagedScenePlan.Build(H1RepresentativeSliceScenario.InitialState(unknown), catalogue));
            Assert.Equal("projection.source-missing", missing.Code);

            var wrongKind = H1RepresentativeSliceScenario.Nodes.Select(node => node.ObjectId == "crate.stack"
                ? node.WithSource("prefab", H1RepresentativeSliceScenario.CrateMesh)
                : node);
            Assert.Equal("projection.source-wrong-type", Assert.Throws<H1ProjectionException>(() =>
                H1ManagedScenePlan.Build(H1RepresentativeSliceScenario.InitialState(wrongKind), catalogue)).Code);
        }
    }
}
