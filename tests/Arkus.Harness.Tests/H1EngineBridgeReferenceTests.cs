using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Arkus.EngineBridge;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1EngineBridgeReferenceTests
    {
        private static readonly string[] Catalogue = { "asset.market-stall", "asset.workshop-kit" };

        [Fact]
        public void SameFullInputNormalizesPlanAndRetryHasNoSecondSemanticDelta()
        {
            var snapshot = new byte[] { 1, 2, 3, 4, 5 };
            var firstInput = Input(snapshot);
            var secondInput = Input(snapshot);
            var first = ProjectionPlanner.Create(firstInput, Resources().Reverse());
            var second = ProjectionPlanner.Create(secondInput, Resources());

            Assert.Equal(firstInput.InputDigest, secondInput.InputDigest);
            Assert.Equal(first.PlanDigest, second.PlanDigest);
            Assert.Equal(first.GenerationId, second.GenerationId);
            Assert.Equal(first.Resources.Select(value => value.ResourceId), second.Resources.Select(value => value.ResourceId));

            var materializer = new ReferenceMaterializer();
            var applied = materializer.Materialize(first, Catalogue);
            var retried = materializer.Materialize(second, Catalogue);

            Assert.True(applied.SemanticChange);
            Assert.False(retried.SemanticChange);
            Assert.Equal(applied.Receipt.ReceiptDigest, retried.Receipt.ReceiptDigest);
            Assert.Equal(ProjectionDriftState.InSync, retried.Observation.State);
        }

        [Fact]
        public void EveryFullInputAxisChangesIdentityAndReceiptAnchorsTheTuple()
        {
            var snapshot = new byte[] { 1, 2, 3 };
            var baseline = ProjectionPlanner.Create(Input(snapshot), Resources());
            var canonical = ProjectionPlanner.Create(Input(snapshot, revision: 8, stateHash: "world-hash-8"), Resources());
            var snapshotBytes = ProjectionPlanner.Create(Input(new byte[] { 1, 2, 4 }), Resources());
            var binding = ProjectionPlanner.Create(Input(snapshot, binding: "binding@2"), Resources());
            var catalogue = ProjectionPlanner.Create(Input(snapshot, catalogue: "catalogue-b"), Resources());
            var toolchain = ProjectionPlanner.Create(Input(snapshot, toolchain: "toolchain-b"), Resources());

            foreach (var changed in new[] { canonical, snapshotBytes, binding, catalogue, toolchain })
            {
                Assert.NotEqual(baseline.Input.InputDigest, changed.Input.InputDigest);
                Assert.NotEqual(baseline.PlanDigest, changed.PlanDigest);
                Assert.NotEqual(baseline.GenerationId, changed.GenerationId);
            }

            Assert.Equal(baseline.Input.CanonicalSnapshotDigest, canonical.Input.CanonicalSnapshotDigest);
            Assert.NotEqual(baseline.Input.CanonicalAnchor.NormalizedForTest(), canonical.Input.CanonicalAnchor.NormalizedForTest());

            var materializer = new ReferenceMaterializer();
            var result = materializer.Materialize(baseline, Catalogue);
            Assert.Equal(baseline.Input.CanonicalAnchor.NormalizedForTest(), result.Receipt.CanonicalAnchor);
            Assert.Equal(baseline.Input.CatalogueFingerprint, result.Receipt.CatalogueFingerprint);
            Assert.Equal(baseline.Input.BindingVersion, result.Receipt.BindingVersion);
            Assert.Equal(baseline.Input.Profile.Fingerprint, result.Receipt.BridgeProfileFingerprint);
            Assert.Equal(baseline.Input.CanonicalSnapshotDigest, result.Receipt.CanonicalSnapshotDigest);
        }

        [Fact]
        public void RebuildFromSameInputsReconstructsSameGenerationAndObservation()
        {
            var plan = ProjectionPlanner.Create(Input(new byte[] { 9, 8, 7 }), Resources());
            var firstMaterializer = new ReferenceMaterializer();
            var first = firstMaterializer.Materialize(plan, Catalogue);

            var rebuiltMaterializer = new ReferenceMaterializer();
            Assert.Equal(ProjectionDriftState.Absent, rebuiltMaterializer.Observe(plan, null, Catalogue).State);
            var rebuilt = rebuiltMaterializer.Materialize(plan, Catalogue);

            Assert.True(first.Receipt.Published);
            Assert.True(rebuilt.Receipt.Published);
            Assert.Equal(first.Receipt.GenerationId, rebuilt.Receipt.GenerationId);
            Assert.Equal(first.Receipt.PlanDigest, rebuilt.Receipt.PlanDigest);
            Assert.Equal(first.Observation.ObservationDigest, rebuilt.Observation.ObservationDigest);
            Assert.Equal(first.Receipt.ReceiptDigest, rebuilt.Receipt.ReceiptDigest);
        }

        [Fact]
        public void FailedStagingNeverMovesTheActiveGeneration()
        {
            var materializer = new ReferenceMaterializer();
            var first = ProjectionPlanner.Create(Input(new byte[] { 1 }), Resources());
            var next = ProjectionPlanner.Create(Input(new byte[] { 2 }, revision: 2, stateHash: "world-hash-2"), Resources());
            var published = materializer.Materialize(first, Catalogue);
            var active = materializer.ActiveGenerationId;

            var failed = materializer.Materialize(next, Catalogue, ReferenceFailurePoint.BeforePublication);

            Assert.True(published.Receipt.Published);
            Assert.False(failed.Receipt.Published);
            Assert.Equal(ProjectionDriftState.Failed, failed.Observation.State);
            Assert.Equal(active, materializer.ActiveGenerationId);
            Assert.Equal(published.Receipt.ReceiptDigest, materializer.ActiveReceipt!.ReceiptDigest);
        }

        [Fact]
        public void ObservationDistinguishesAllRequiredStatesFromEffectiveResources()
        {
            var expected = ProjectionPlanner.Create(Input(new byte[] { 1 }), Resources());
            var materializer = new ReferenceMaterializer();

            Assert.Equal(ProjectionDriftState.Absent, materializer.Observe(expected, null, Catalogue).State);
            Assert.True(materializer.Materialize(expected, Catalogue).Receipt.Published);
            Assert.Equal(ProjectionDriftState.InSync, materializer.Observe(expected, null, Catalogue).State);

            var ahead = ProjectionPlanner.Create(Input(new byte[] { 2 }, revision: 2, stateHash: "world-hash-2"), Resources());
            Assert.Equal(ProjectionDriftState.CanonicalAhead, materializer.Observe(ahead, null, Catalogue).State);

            var changed = Resources().ToArray();
            changed[2] = new ProjectionResource(changed[2].ResourceId, changed[2].ParentResourceId, changed[2].ResourceKind, "changed-content", changed[2].LogicalDependencies);
            Assert.Equal(ProjectionDriftState.EngineDrift, materializer.Observe(expected, changed, Catalogue).State);

            Assert.Equal(ProjectionDriftState.MissingDependency, materializer.Observe(expected, null, new[] { "asset.market-stall" }).State);

            var duplicate = Resources().Concat(new[] { Resources().First() }).ToArray();
            Assert.Equal(ProjectionDriftState.Ambiguous, materializer.Observe(expected, duplicate, Catalogue).State);

            var failedMaterializer = new ReferenceMaterializer();
            failedMaterializer.Materialize(expected, Catalogue, ReferenceFailurePoint.BeforeValidation);
            Assert.Equal(ProjectionDriftState.Failed, failedMaterializer.Observe(expected, null, Catalogue).State);
        }

        [Fact]
        public void AmbiguousObservationDigestIsIndependentOfDuplicateEnumerationOrder()
        {
            var expected = ProjectionPlanner.Create(Input(new byte[] { 1 }), Resources());
            var materializer = new ReferenceMaterializer();
            var baselineRoot = Resources().First();
            var conflictingRoot = new ProjectionResource(
                baselineRoot.ResourceId,
                baselineRoot.ParentResourceId,
                baselineRoot.ResourceKind,
                "plaza-conflicting-content",
                baselineRoot.LogicalDependencies);
            var remaining = Resources().Where(value => value.ResourceId != baselineRoot.ResourceId).ToArray();

            var forward = new[] { baselineRoot, conflictingRoot }.Concat(remaining).ToArray();
            var reversed = new[] { conflictingRoot, baselineRoot }.Concat(remaining).ToArray();

            var first = materializer.Observe(expected, forward, Catalogue);
            var second = materializer.Observe(expected, reversed, Catalogue);

            Assert.Equal(ProjectionDriftState.Ambiguous, first.State);
            Assert.Equal(ProjectionDriftState.Ambiguous, second.State);
            Assert.Equal(first.Diagnostics, second.Diagnostics);
            Assert.Equal(first.EffectiveDigest, second.EffectiveDigest);
            Assert.Equal(first.ObservationDigest, second.ObservationDigest);
        }

        [Fact]
        public void DriftOracleDetectsExtraMissingAndChangedManagedResources()
        {
            var plan = ProjectionPlanner.Create(Input(new byte[] { 1 }), Resources());
            var materializer = new ReferenceMaterializer();
            materializer.Materialize(plan, Catalogue);

            var original = Resources().ToArray();
            var missing = original.Where(value => value.ResourceId != "world/plaza/bar").ToArray();
            var extra = original.Concat(new[] { new ProjectionResource("world/plaza/unmanaged", "world/plaza", "fixture", "extra") }).ToArray();
            var changed = original.Select(value => value.ResourceId == "world/plaza/market"
                ? new ProjectionResource(value.ResourceId, value.ParentResourceId, value.ResourceKind, "different", value.LogicalDependencies)
                : value).ToArray();

            Assert.Equal(ProjectionDriftState.EngineDrift, materializer.Observe(plan, missing, Catalogue).State);
            Assert.Equal(ProjectionDriftState.EngineDrift, materializer.Observe(plan, extra, Catalogue).State);
            Assert.Equal(ProjectionDriftState.EngineDrift, materializer.Observe(plan, changed, Catalogue).State);
        }

        [Fact]
        public void BridgePlanningAndMaterializationCannotMutateCanonicalSnapshotBytes()
        {
            var callerBytes = new byte[] { 7, 8, 9, 10 };
            var before = Hash(callerBytes);
            var input = Input(callerBytes);
            var plan = ProjectionPlanner.Create(input, Resources());
            var materializer = new ReferenceMaterializer();
            materializer.Materialize(plan, Catalogue);
            materializer.Observe(plan, null, Catalogue);

            Assert.Equal(before, Hash(callerBytes));
            Assert.Equal(before, input.CanonicalSnapshotDigest);
            var copy = input.GetCanonicalSnapshotCopy();
            copy[0] = 99;
            Assert.Equal(before, input.CanonicalSnapshotDigest);
            Assert.NotEqual(copy[0], input.GetCanonicalSnapshotCopy()[0]);
        }

        [Fact]
        public void NeutralBridgeAssemblyHasNoRuntimeUnityOrTransportDependency()
        {
            var references = typeof(ProjectionInput).Assembly.GetReferencedAssemblies().Select(value => value.Name).ToArray();
            Assert.DoesNotContain("Arkus.Harness.Runtime", references);
            Assert.DoesNotContain("Arkus.Harness.Projection", references);
            Assert.DoesNotContain("Arkus.Harness.Mcp", references);
            Assert.DoesNotContain(references, value => value != null && value.IndexOf("Unity", StringComparison.OrdinalIgnoreCase) >= 0);

            var publicTypeNames = typeof(ProjectionInput).Assembly.GetExportedTypes().Select(value => value.FullName ?? value.Name).ToArray();
            Assert.DoesNotContain(publicTypeNames, value => value.IndexOf("UnityEngine", StringComparison.Ordinal) >= 0 || value.IndexOf("UnityEditor", StringComparison.Ordinal) >= 0);
        }

        [Fact]
        public void ContentShapeProbeRepresentsPlazaMarketBarWorkshopWithTwoLogicalAssets()
        {
            var plan = ProjectionPlanner.Create(Input(new byte[] { 4, 3, 2, 1 }), Resources());
            Assert.Equal(
                new[] { "world/plaza", "world/plaza/bar", "world/plaza/market", "world/plaza/workshop" },
                plan.Resources.Select(value => value.ResourceId).ToArray());
            Assert.Equal(
                Catalogue,
                plan.Resources.SelectMany(value => value.LogicalDependencies).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray());
            Assert.All(plan.Resources.Where(value => value.ParentResourceId != null), value => Assert.Equal("world/plaza", value.ParentResourceId));
        }

        private static ProjectionInput Input(
            byte[] snapshot,
            long revision = 7,
            string stateHash = "world-hash-7",
            string binding = "binding@1",
            string catalogue = "catalogue-a",
            string toolchain = "toolchain-a")
        {
            return new ProjectionInput(
                new CanonicalProjectionAnchor("world.reference", revision, stateHash),
                binding,
                catalogue,
                new BridgeToolchainProfile("reference", "1.0", toolchain, "portable"),
                snapshot);
        }

        private static IEnumerable<ProjectionResource> Resources()
        {
            yield return new ProjectionResource("world/plaza", null, "root", "plaza-v1");
            yield return new ProjectionResource("world/plaza/market", "world/plaza", "fixture", "market-v1", new[] { "asset.market-stall" });
            yield return new ProjectionResource("world/plaza/bar", "world/plaza", "fixture", "bar-v1");
            yield return new ProjectionResource("world/plaza/workshop", "world/plaza", "fixture", "workshop-v1", new[] { "asset.workshop-kit" });
        }

        private static string Hash(byte[] value)
        {
            using var sha = SHA256.Create();
            return string.Concat(sha.ComputeHash(value).Select(item => item.ToString("x2", System.Globalization.CultureInfo.InvariantCulture)));
        }
    }

    internal static class H1AnchorTestExtensions
    {
        internal static string NormalizedForTest(this CanonicalProjectionAnchor anchor)
        {
            static string Sequence(params string[] values) => string.Concat(values.Select(value => value.Length.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + value + ";"));
            return Sequence(anchor.WorldId, anchor.Revision.ToString(System.Globalization.CultureInfo.InvariantCulture), anchor.StateHash);
        }
    }
}