using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.EngineBridge.UnityAuthoring;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Arkus.H1.UnityHost;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1ProjectionReconciliationCompositionTests
    {
        [Fact]
        public void SupportedUnityEdit_UsesAcceptedH0SameLineageRecoveryAndCanonicalRebuildConvergesToParity()
        {
            var catalogue = Catalogue();
            var source = catalogue.Entries.First(value => value.Kind == "prefab");
            var original = CanonicalState(7, source.LogicalId);
            var expected = H1ManagedScenePlan.Build(original, catalogue);
            Assert.Equal(2, expected.Nodes.Length);

            var editedFacade = Observed(expected.Nodes.Single(value => value.ObjectId == "facade"));
            editedFacade.PositionMm.Z += 375;
            editedFacade.ComponentRows = Rows(expected.Nodes.Single(value => value.ObjectId == "facade"), editedFacade.PositionMm);
            editedFacade.ComponentDigest = H1ManagedScenePlan.Sha(string.Join("\n", editedFacade.ComponentRows));
            var effective = Observation(expected, editedFacade, Observed(expected.Nodes.Single(value => value.ObjectId == "workshop")));
            var drift = H1ProjectionReconciliation.Compare(expected, effective);
            Assert.False(drift.Parity);
            Assert.Equal("engine-drift", drift.State);

            var proposal = H1ProjectionReconciliation.CompileProposal(expected, effective, drift, catalogue);
            Assert.True(proposal.Available);
            Assert.NotNull(proposal.MutationRequest);
            Assert.Equal(new[] { "facade" }, proposal.ObjectIds);

            var session = new PortableWorldAuthoringSession(original);
            var contract = CanonicalWorldContract.Compose(new WorldInspectionService(session), session);
            Hk04TransactionalMutationTests.Success(
                contract,
                WorldMutationContract.ApplyName,
                Hk04TransactionalMutationTests.Request(
                    original,
                    "request.h1-09.concurrent-writer",
                    Hk04TransactionalMutationTests.PutObject("observer", "fixture.concurrent")));
            Assert.Equal(original.Revision + 1, session.Current.Revision);

            var stale = contract.Dispatch(WorldMutationContract.PlanName, Exact(), proposal.MutationRequest!);
            Assert.False(stale.Success);
            Assert.NotNull(stale.Error);
            Assert.Contains(stale.Error!.MachineCode, new[] { "world.change.stale_revision", "world.change.stale_hash" });
            Assert.True(stale.Error.Context.TryGetValue("recovery", out var rawRecovery));
            var recovery = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(rawRecovery);
            Assert.Equal(WorldConflictRecoveryContract.SameLineageReplan, recovery["disposition"]);
            var current = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(recovery["current"]);
            Assert.Equal(session.Current.Revision, Convert.ToInt64(current["revision"]));
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(session.Current), current["hash"]);

            var recoveredRequest = new Dictionary<string, object?>(proposal.MutationRequest!, StringComparer.Ordinal)
            {
                ["expectedRevision"] = session.Current.Revision,
                ["expectedHash"] = CanonicalWorldStateCodec.ComputeContentHash(session.Current)
            };
            var plan = contract.Dispatch(WorldMutationContract.PlanName, Exact(), recoveredRequest);
            var dryRun = contract.Dispatch(WorldMutationContract.DryRunName, Exact(), recoveredRequest);
            var apply = contract.Dispatch(WorldMutationContract.ApplyName, Exact(), recoveredRequest);
            Assert.True(plan.Success, plan.Error?.MachineCode);
            Assert.True(dryRun.Success, dryRun.Error?.MachineCode);
            Assert.True(apply.Success, apply.Error?.MachineCode);
            Assert.False((bool)apply.Data!["replayed"]!);
            Assert.Equal(original.Revision + 2, session.Current.Revision);
            Assert.Contains(session.Current.Objects, value => value.Id.Value == "observer");
            Assert.Equal(2, Convert.ToInt32(Hk06AProvenanceJournalTests.ReadJournal(contract).Data!["entryCount"]));

            var rebuilt = H1ManagedScenePlan.Build(session.Current, catalogue);
            Assert.Equal(session.Current.Revision, rebuilt.WorldRevision);
            Assert.Equal(CanonicalWorldStateCodec.ComputeContentHash(session.Current), rebuilt.CanonicalHash);
            var rebuiltFacade = rebuilt.Nodes.Single(value => value.ObjectId == "facade");
            Assert.Equal(editedFacade.PositionMm.Z, rebuiltFacade.PositionMm.Z);

            var rematerialized = Observation(rebuilt, rebuilt.Nodes.Select(Observed).ToArray());
            var parity = H1ProjectionReconciliation.Compare(rebuilt, rematerialized);
            Assert.True(parity.Parity);
            Assert.Equal("in-sync", parity.State);
            Assert.Empty(parity.Items);
        }

        private static WorldState CanonicalState(long revision, string sourceLogicalId)
        {
            return new WorldState(
                new WorldId("world.h1-09"),
                revision,
                new[]
                {
                    new WorldObject(new WorldObjectId("facade"), new WorldTypeId("fixture.facade")),
                    new WorldObject(new WorldObjectId("workshop"), new WorldTypeId("fixture.workshop"))
                },
                new[]
                {
                    Binding("facade", sourceLogicalId, 100, 0, 0),
                    Binding("workshop", sourceLogicalId, 900, 0, 0)
                });
        }

        private static WorldExtensionData Binding(string subject, string sourceLogicalId, long x, long y, long z)
        {
            var binding = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["schemaId"] = UnityBindingProducer.BindingSchemaId,
                ["targetSceneId"] = H1ManagedScenePlan.SceneId,
                ["source"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = "prefab",
                    ["logicalId"] = sourceLogicalId
                },
                ["transform"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["coordinateConvention"] = UnityBindingProducer.CoordinateConvention,
                    ["positionMm"] = Vector(x, y, z),
                    ["rotationMilliDegrees"] = Vector(0, 0, 0),
                    ["scalePpm"] = Vector(1000000, 1000000, 1000000)
                },
                ["components"] = Array.Empty<object?>()
            };
            var compiled = UnityBindingProducer.Compile(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["subjectId"] = subject,
                ["binding"] = binding
            });
            return new WorldExtensionData(
                UnityBindingProducer.ExtensionOwner,
                UnityBindingProducer.ExtensionSchemaVersion,
                Convert.FromBase64String((string)compiled["payloadBase64"]!),
                new WorldObjectId(subject),
                Array.Empty<WorldReference>());
        }

        private static Dictionary<string, object?> Vector(long x, long y, long z) => new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["x"] = x,
            ["y"] = y,
            ["z"] = z
        };

        private static H1ObservedSceneNode Observed(H1ManagedSceneNode node)
        {
            var rows = Rows(node, node.PositionMm);
            return new H1ObservedSceneNode
            {
                ObjectId = node.ObjectId,
                ParentObjectId = node.ParentObjectId,
                SourceLogicalId = node.SourceLogicalId,
                SourceKind = node.SourceKind,
                SourcePath = node.SourcePath,
                SourceGuid = node.SourceGuid,
                SourceLocalFileId = node.SourceLocalFileId,
                SourceContentSha256 = node.SourceContentSha256,
                RealizationKind = node.SourceKind == "prefab" ? "managed-prefab-variant" : "source-asset",
                RealizedPath = node.SourcePath,
                RealizedGuid = node.SourceGuid,
                RealizedLocalFileId = node.SourceLocalFileId,
                PrefabGenerationId = node.SourceKind == "prefab" ? new string('2', 32) : "",
                RelationshipDigest = H1ManagedScenePlan.Sha("relationships-" + node.ObjectId),
                Relationships = Array.Empty<H1ObservedPrefabRelationship>(),
                PositionMm = Copy(node.PositionMm),
                RotationMilliDegrees = Copy(node.RotationMilliDegrees),
                ScalePpm = Copy(node.ScalePpm),
                ComponentRows = rows,
                ComponentDigest = H1ManagedScenePlan.Sha(string.Join("\n", rows))
            };
        }

        private static H1ProjectionReconciliationObservation Observation(H1ManagedScenePlan plan, params H1ObservedSceneNode[] nodes)
        {
            return new H1ProjectionReconciliationObservation
            {
                Active = true,
                GenerationId = new string('3', 32),
                InputDigest = plan.InputDigest,
                CanonicalHash = plan.CanonicalHash,
                CatalogueFingerprint = plan.CatalogueFingerprint,
                ManifestGraphDigest = H1ManagedScenePlan.Sha("graph"),
                ManifestRealizationDigest = H1ManagedScenePlan.Sha("realization"),
                GraphDigest = H1ManagedScenePlan.Sha("graph"),
                RealizationDigest = H1ManagedScenePlan.Sha("realization"),
                Nodes = nodes.OrderBy(value => value.ObjectId, StringComparer.Ordinal).ToArray(),
                UnmanagedPaths = Array.Empty<string>(),
                Diagnostics = Array.Empty<H1ProjectionObservationDiagnostic>(),
                ManagedDigest = H1ProjectionReconciliation.ManagedDigest(nodes)
            };
        }

        private static string[] Rows(H1ManagedSceneNode node, H1ProjectionVector position)
        {
            return new[]
            {
                H1ComponentSchemas.Transform + "|positionMm=" + Vec(position) + "|rotationMilliDegrees=" + VecNormalized(node.RotationMilliDegrees) + "|scalePpm=" + Vec(node.ScalePpm)
            };
        }

        private static H1ProjectionVector Copy(H1ProjectionVector value) => new H1ProjectionVector { X = value.X, Y = value.Y, Z = value.Z };
        private static string Vec(H1ProjectionVector value) => value.X + "," + value.Y + "," + value.Z;
        private static string VecNormalized(H1ProjectionVector value) => Normalize(value.X) + "," + Normalize(value.Y) + "," + Normalize(value.Z);
        private static long Normalize(long value) { var result = value % 360000; return result < 0 ? result + 360000 : result; }
        private static ContractVersionRange Exact() => ContractVersionRange.Exact(new ContractVersion(1, 0));

        private static H1CatalogueSnapshot Catalogue()
        {
            var root = H1UnityLaunchProfile.ForCurrentHost().RepositoryRoot;
            return H1CatalogueSnapshot.Build(
                File.ReadAllText(Path.Combine(root, "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json")),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.MappingRelativePath)),
                File.ReadAllText(Path.Combine(root, H1CatalogueSnapshot.AdoptionRelativePath)));
        }
    }
}
