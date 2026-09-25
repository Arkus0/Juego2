using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1ProjectionReconciliationTests
    {
        [Fact]
        public void DriftOracleClassifiesManagedAndUnmanagedFactsWithoutDigestContamination()
        {
            var plan = Plan("a", 7, Node("facade"), Node("workshop"));
            var facade = Observed(plan.Nodes[0]);
            facade.PositionMm.X += 250;
            facade.ComponentRows = Rows(plan.Nodes[0], positionX: facade.PositionMm.X);
            facade.ComponentDigest = Hash("facade-components-changed");
            var extra = Observed(Node("extra"));
            var observation = Observation(plan, facade, extra);
            observation.UnmanagedPaths = new[] { "Arkus Managed Root/LooseLamp" };
            observation.ManagedDigest = H1ProjectionReconciliation.ManagedDigest(observation.Nodes);

            var report = H1ProjectionReconciliation.Compare(plan, observation);

            Assert.False(report.Parity);
            Assert.Equal("engine-drift", report.State);
            Assert.Equal(new[]
            {
                "missing|workshop|managed",
                "extra|Arkus Managed Root/LooseLamp|unmanaged",
                "extra|extra|managed",
                "changed|facade|managed"
            }, report.Items.Select(Token).OrderBy(value => value, StringComparer.Ordinal).ToArray());
            Assert.Contains(report.Items.Single(value => value.ObjectId == "facade").Fields, value => value == "transform");

            var withDifferentUnmanaged = Observation(plan, facade, extra);
            withDifferentUnmanaged.UnmanagedPaths = new[] { "Arkus Managed Root/OtherLooseLamp" };
            withDifferentUnmanaged.ManagedDigest = H1ProjectionReconciliation.ManagedDigest(withDifferentUnmanaged.Nodes);
            var second = H1ProjectionReconciliation.Compare(plan, withDifferentUnmanaged);
            Assert.Equal(report.ManagedDigest, second.ManagedDigest);
            Assert.NotEqual(report.Items.Single(value => !value.Managed).ObjectId, second.Items.Single(value => !value.Managed).ObjectId);
        }

        [Fact]
        public void AnchorCatalogueAndDuplicateIdentityNeverCollapseToParity()
        {
            var plan = Plan("b", 11, Node("facade"));
            var first = Observed(plan.Nodes[0]);
            var duplicate = Observed(plan.Nodes[0]);
            var observation = Observation(plan, first, duplicate);
            observation.CanonicalHash = Hash("older-canonical");
            observation.InputDigest = Hash("older-input");
            observation.CatalogueFingerprint = Hash("older-catalogue");
            observation.ManagedDigest = H1ProjectionReconciliation.ManagedDigest(observation.Nodes);

            var report = H1ProjectionReconciliation.Compare(plan, observation);

            Assert.False(report.Parity);
            Assert.Equal("ambiguous", report.State);
            Assert.Contains(report.Items, value => value.Classification == H1ProjectionDriftClass.Ambiguous && value.ObjectId == "facade");
            Assert.Contains(report.Items, value => value.Classification == H1ProjectionDriftClass.CatalogueDrift);
            Assert.Contains(report.Items, value => value.Classification == H1ProjectionDriftClass.CanonicalAhead);
        }

        [Fact]
        public void SupportedFacadeTransformAndMaterialEditCompilesOnlyItsH0OperationWithDerivedDependency()
        {
            var catalogue = Catalogue();
            var materials = catalogue.Entries.Where(value => value.Kind == "material").Take(2).ToArray();
            Assert.Equal(2, materials.Length);
            var source = catalogue.Entries.First(value => value.Kind == "prefab");

            var canonical = CanonicalState(7);
            var canonicalHash = CanonicalWorldStateCodec.ComputeContentHash(canonical);
            var facade = Node("facade", source, materials[0], canonicalLinkTarget: "workshop");
            var workshop = Node("workshop", source, materials[0]);
            var plan = Plan(canonicalHash, 7, new[] { facade, workshop }, catalogue.Fingerprint, true);

            var editedFacade = Observed(facade);
            editedFacade.PositionMm.X += 400;
            editedFacade.ComponentRows = Rows(facade, positionX: editedFacade.PositionMm.X, renderer: materials[1]);
            editedFacade.ComponentDigest = Hash(string.Join("\n", editedFacade.ComponentRows));
            var driftedWorkshop = Observed(workshop);
            driftedWorkshop.SourceLogicalId = "source.changed-but-not-importable";
            var observation = Observation(plan, editedFacade, driftedWorkshop);
            observation.ManagedDigest = H1ProjectionReconciliation.ManagedDigest(observation.Nodes);
            var report = H1ProjectionReconciliation.Compare(plan, observation);

            var proposal = H1ProjectionReconciliation.CompileProposal(plan, observation, report, catalogue);

            Assert.True(proposal.Available);
            Assert.Equal(new[] { "facade" }, proposal.ObjectIds);
            Assert.Contains(proposal.Diagnostics, value => value.StartsWith("projection.import-unsupported-fields:workshop:", StringComparison.Ordinal));
            Assert.NotNull(proposal.MutationRequest);
            Assert.Equal(7L, proposal.MutationRequest!["expectedRevision"]);
            Assert.Equal(canonicalHash, proposal.MutationRequest["expectedHash"]);
            var operations = (IReadOnlyList<object?>)proposal.MutationRequest["operations"]!;
            Assert.Single(operations);
            var operation = (IReadOnlyDictionary<string, object?>)operations[0]!;
            Assert.Equal("put-extension", operation["kind"]);
            Assert.Equal("facade", operation["subjectId"]);
            var dependencies = (IReadOnlyList<object?>)operation["dependencies"]!;
            var dependency = (IReadOnlyDictionary<string, object?>)Assert.Single(dependencies)!;
            Assert.Equal("attached-to", dependency["kind"]);
            Assert.Equal("workshop", dependency["targetId"]);

            Assert.Empty(canonical.Extensions);
        }

        [Fact]
        public void ProposalUsesAcceptedH0CasSoStaleBaseFailsAndFreshBaseCanPlanDryRunApply()
        {
            var catalogue = Catalogue();
            var materials = catalogue.Entries.Where(value => value.Kind == "material").Take(2).ToArray();
            var source = catalogue.Entries.First(value => value.Kind == "prefab");
            var original = CanonicalState(7);
            var originalHash = CanonicalWorldStateCodec.ComputeContentHash(original);
            var facade = Node("facade", source, materials[0], canonicalLinkTarget: "workshop");
            var workshop = Node("workshop", source, materials[0]);
            var expected = Plan(originalHash, original.Revision, new[] { facade, workshop }, catalogue.Fingerprint, true);
            var edited = Observed(facade);
            edited.PositionMm.Z += 100;
            edited.ComponentRows = Rows(facade, positionZ: edited.PositionMm.Z, renderer: materials[1]);
            edited.ComponentDigest = Hash(string.Join("\n", edited.ComponentRows));
            var observation = Observation(expected, edited, Observed(workshop));
            observation.ManagedDigest = H1ProjectionReconciliation.ManagedDigest(observation.Nodes);
            var report = H1ProjectionReconciliation.Compare(expected, observation);
            var proposal = H1ProjectionReconciliation.CompileProposal(expected, observation, report, catalogue);
            Assert.True(proposal.Available);

            var newer = CanonicalState(8);
            var staleSession = new PortableWorldAuthoringSession(newer);
            var staleContract = CanonicalWorldContract.Compose(new WorldInspectionService(staleSession), staleSession);
            var stalePlan = staleContract.Dispatch(WorldMutationContract.PlanName, Exact(), proposal.MutationRequest!);
            Assert.False(stalePlan.Success);
            Assert.NotNull(stalePlan.Error);
            Assert.Contains(stalePlan.Error!.MachineCode, new[] { "mutation.stale_revision", "mutation.stale_hash" });
            Assert.Empty(staleSession.Current.Extensions);

            var freshSession = new PortableWorldAuthoringSession(original);
            var freshContract = CanonicalWorldContract.Compose(new WorldInspectionService(freshSession), freshSession);
            var planResult = freshContract.Dispatch(WorldMutationContract.PlanName, Exact(), proposal.MutationRequest!);
            var dryRun = freshContract.Dispatch(WorldMutationContract.DryRunName, Exact(), proposal.MutationRequest!);
            var apply = freshContract.Dispatch(WorldMutationContract.ApplyName, Exact(), proposal.MutationRequest!);
            Assert.True(planResult.Success, planResult.Error?.MachineCode);
            Assert.True(dryRun.Success, dryRun.Error?.MachineCode);
            Assert.True(apply.Success, apply.Error?.MachineCode);
            Assert.Single(freshSession.Current.Extensions);
        }

        private static WorldState CanonicalState(long revision) => new WorldState(
            new WorldId("world.h1-09"), revision,
            new[]
            {
                new WorldObject(new WorldObjectId("facade"), new WorldTypeId("fixture.facade")),
                new WorldObject(new WorldObjectId("workshop"), new WorldTypeId("fixture.workshop"))
            });

        private static H1ManagedScenePlan Plan(string seedOrHash, long revision, params H1ManagedSceneNode[] nodes) =>
            Plan(seedOrHash, revision, nodes, Hash("catalogue-default"), false);

        private static H1ManagedScenePlan Plan(string seedOrHash, long revision, H1ManagedSceneNode[] nodes, string catalogueFingerprint, bool canonicalHashIsLiteral)
        {
            var canonical = canonicalHashIsLiteral ? seedOrHash : Hash("canonical-" + seedOrHash);
            var input = Hash("input-" + canonical + "-" + catalogueFingerprint + "-" + string.Join("|", nodes.Select(value => value.ObjectId)));
            return new H1ManagedScenePlan
            {
                WorldId = "world.h1-09",
                WorldRevision = revision,
                CanonicalHash = canonical,
                CatalogueFingerprint = catalogueFingerprint,
                InputDigest = input,
                Nodes = nodes.OrderBy(value => value.ObjectId, StringComparer.Ordinal).ToArray()
            };
        }

        private static H1ManagedSceneNode Node(string id) => new H1ManagedSceneNode
        {
            ObjectId = id,
            SourceKind = "asset",
            SourceLogicalId = "asset." + id,
            SourcePath = "Assets/Arkus/H1/SourceSlice/" + id + ".fbx",
            SourceGuid = new string('1', 32),
            SourceLocalFileId = "1",
            SourceContentSha256 = Hash("source-" + id),
            PositionMm = V(100, 0, 0), RotationMilliDegrees = V(0, 0, 0), ScalePpm = V(1000000, 1000000, 1000000),
            Components = new[] { new H1ComponentPlan { SchemaId = H1ComponentSchemas.Transform, Kind = "transform" } }
        };

        private static H1ManagedSceneNode Node(string id, H1CatalogueEntry source, H1CatalogueEntry material, string canonicalLinkTarget = "")
        {
            var components = new List<H1ComponentPlan>
            {
                new H1ComponentPlan { SchemaId = H1ComponentSchemas.Transform, Kind = "transform" },
                new H1ComponentPlan
                {
                    SchemaId = H1ComponentSchemas.MeshRenderer, Kind = "renderer", ReferenceKind = "material", ReferenceLogicalId = material.LogicalId,
                    ReferencePath = material.Path, ReferenceGuid = material.NativeGuid, ReferenceLocalFileId = material.LocalFileId, ReferenceContentSha256 = material.ContentSha256
                }
            };
            if (canonicalLinkTarget.Length != 0)
                components.Add(new H1ComponentPlan { SchemaId = H1ComponentSchemas.CanonicalLink, Kind = "canonical-link", Relation = "attached-to", TargetObjectId = canonicalLinkTarget });
            return new H1ManagedSceneNode
            {
                ObjectId = id, SourceKind = source.Kind, SourceLogicalId = source.LogicalId, SourcePath = source.Path, SourceGuid = source.NativeGuid,
                SourceLocalFileId = source.LocalFileId, SourceContentSha256 = source.ContentSha256,
                PositionMm = V(100, 0, 0), RotationMilliDegrees = V(0, 0, 0), ScalePpm = V(1000000, 1000000, 1000000),
                Components = components.OrderBy(value => value.SortKey, StringComparer.Ordinal).ToArray()
            };
        }

        private static H1ObservedSceneNode Observed(H1ManagedSceneNode node) => new H1ObservedSceneNode
        {
            ObjectId = node.ObjectId, ParentObjectId = node.ParentObjectId, SourceLogicalId = node.SourceLogicalId, SourceKind = node.SourceKind,
            SourcePath = node.SourcePath, SourceGuid = node.SourceGuid, SourceLocalFileId = node.SourceLocalFileId, SourceContentSha256 = node.SourceContentSha256,
            RealizationKind = node.SourceKind == "prefab" ? "managed-prefab-variant" : "source-asset",
            RealizedPath = node.SourcePath, RealizedGuid = node.SourceGuid, RealizedLocalFileId = node.SourceLocalFileId,
            PrefabGenerationId = node.SourceKind == "prefab" ? new string('2', 32) : "",
            RelationshipDigest = Hash("relationships-" + node.ObjectId), Relationships = Array.Empty<H1ObservedPrefabRelationship>(),
            PositionMm = V(node.PositionMm.X, node.PositionMm.Y, node.PositionMm.Z),
            RotationMilliDegrees = V(node.RotationMilliDegrees.X, node.RotationMilliDegrees.Y, node.RotationMilliDegrees.Z),
            ScalePpm = V(node.ScalePpm.X, node.ScalePpm.Y, node.ScalePpm.Z),
            ComponentRows = Rows(node), ComponentDigest = Hash(string.Join("\n", Rows(node)))
        };

        private static H1ProjectionReconciliationObservation Observation(H1ManagedScenePlan plan, params H1ObservedSceneNode[] nodes)
        {
            return new H1ProjectionReconciliationObservation
            {
                Active = true, GenerationId = new string('3', 32), InputDigest = plan.InputDigest, CanonicalHash = plan.CanonicalHash,
                CatalogueFingerprint = plan.CatalogueFingerprint, ManifestGraphDigest = Hash("graph-baseline"), ManifestRealizationDigest = Hash("realization-baseline"),
                GraphDigest = Hash("graph-baseline"), RealizationDigest = Hash("realization-baseline"), Nodes = nodes,
                ManagedDigest = H1ProjectionReconciliation.ManagedDigest(nodes)
            };
        }

        private static string[] Rows(H1ManagedSceneNode node, long? positionX = null, long? positionZ = null, H1CatalogueEntry? renderer = null)
        {
            var position = V(positionX ?? node.PositionMm.X, node.PositionMm.Y, positionZ ?? node.PositionMm.Z);
            var rows = new List<string>
            {
                H1ComponentSchemas.Transform + "|positionMm=" + Vec(position) + "|rotationMilliDegrees=" + VecNormalized(node.RotationMilliDegrees) + "|scalePpm=" + Vec(node.ScalePpm)
            };
            foreach (var component in node.Components)
            {
                if (component.SchemaId == H1ComponentSchemas.MeshRenderer)
                {
                    var identity = renderer == null
                        ? component.ReferencePath + "|" + component.ReferenceGuid + "|" + component.ReferenceLocalFileId
                        : renderer.Path + "|" + renderer.NativeGuid + "|" + renderer.LocalFileId;
                    rows.Add(H1ComponentSchemas.MeshRenderer + "|enabled=true|material=" + identity);
                }
                else if (component.SchemaId == H1ComponentSchemas.Animator)
                    rows.Add(H1ComponentSchemas.Animator + "|applyRootMotion=false|updateMode=Normal|cullingMode=AlwaysAnimate|clip=" + component.ReferencePath + "|" + component.ReferenceGuid + "|" + component.ReferenceLocalFileId);
                else if (component.SchemaId == H1ComponentSchemas.CanonicalLink)
                    rows.Add(H1ComponentSchemas.CanonicalLink + "|relation=" + component.Relation + "|target=" + component.TargetObjectId);
            }
            rows.Sort(StringComparer.Ordinal);
            return rows.ToArray();
        }

        private static H1CatalogueSnapshot Catalogue()
        {
            var root = RepositoryRoot();
            return H1CatalogueSnapshot.Build(
                File.ReadAllText(Path.Combine(root, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath)),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath)));
        }

        private static string RepositoryRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null && !File.Exists(Path.Combine(current.FullName, "Juego2.sln"))) current = current.Parent;
            Assert.NotNull(current);
            return current!.FullName;
        }

        private static ContractVersionRange Exact() => ContractVersionRange.Exact(new ContractVersion(1, 0));
        private static H1ProjectionVector V(long x, long y, long z) => new H1ProjectionVector { X = x, Y = y, Z = z };
        private static string Vec(H1ProjectionVector value) => value.X + "," + value.Y + "," + value.Z;
        private static string VecNormalized(H1ProjectionVector value) => Normalize(value.X) + "," + Normalize(value.Y) + "," + Normalize(value.Z);
        private static long Normalize(long value) { var result = value % 360000; return result < 0 ? result + 360000 : result; }
        private static string Hash(string value) => H1ManagedScenePlan.Sha(value);
        private static string Token(H1ProjectionDriftItem item) => item.Classification + "|" + item.ObjectId + "|" + (item.Managed ? "managed" : "unmanaged");
    }
}
