using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arkus.EngineBridge;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1PortableProjectionTests
    {
        private static readonly string[] Catalogue = { "asset.market-stall", "asset.workshop-kit" };

        [Fact]
        public void PortableContractIsVersionedMachineReadableAndReceiptCarriesStructuredAnchorDiagnostics()
        {
            var plan = Plan();
            var materializer = new ReferenceMaterializer();
            var failed = materializer.Materialize(
                plan,
                Catalogue,
                ReferenceFailurePoint.BeforePublication);

            var data = ProjectionPortableData.Materialization(plan, failed);
            AssertPortable(data);

            var receipt = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(data["receipt"]);
            var observation = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(data["observation"]);
            var anchor = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(receipt["canonicalAnchor"]);
            var profile = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(receipt["bridgeProfile"]);
            var diagnostics = Assert.IsAssignableFrom<IReadOnlyList<object?>>(receipt["diagnostics"]);

            Assert.Equal(ProjectionInput.ContractVersion, data["contractVersion"]);
            Assert.Equal(ProjectionInput.ContractVersion, receipt["contractVersion"]);
            Assert.Equal(ProjectionInput.ContractVersion, observation["contractVersion"]);
            Assert.Equal("world.reference", anchor["worldId"]);
            Assert.Equal(7L, anchor["revision"]);
            Assert.Equal("world-hash-7", anchor["stateHash"]);
            Assert.Equal(plan.Input.Profile.Fingerprint, profile["profileFingerprint"]);
            Assert.Equal("not-published", receipt["publicationStatus"]);
            Assert.Equal("failed", observation["state"]);
            Assert.Contains("reference.failure.before-publication", diagnostics);
            Assert.Equal(failed.Observation.ObservationDigest, receipt["observationDigest"]);
            Assert.Equal(plan.Input.CatalogueFingerprint, receipt["catalogueFingerprint"]);
        }

        [Fact]
        public void PortableReceiptFailsClosedWhenPairedWithDifferentObservation()
        {
            var plan = Plan();
            var first = new ReferenceMaterializer().Materialize(
                plan,
                Catalogue,
                ReferenceFailurePoint.BeforePublication);
            var second = new ReferenceMaterializer().Materialize(
                plan,
                Catalogue,
                ReferenceFailurePoint.BeforeValidation);

            Assert.NotEqual(first.Observation.ObservationDigest, second.Observation.ObservationDigest);
            Assert.Throws<ArgumentException>(() =>
                ProjectionPortableData.Receipt(plan, first.Receipt, second.Observation));
        }

        [Fact]
        public void PortableReceiptFailsClosedWhenPairedWithDifferentPlan()
        {
            var plan = Plan();
            var result = new ReferenceMaterializer().Materialize(plan, Catalogue);
            var differentPlan = Plan("catalogue-b");

            Assert.NotEqual(plan.Input.InputDigest, differentPlan.Input.InputDigest);
            Assert.Throws<ArgumentException>(() =>
                ProjectionPortableData.Receipt(differentPlan, result.Receipt, result.Observation));
        }

        [Fact]
        public void DriftThenDeletionAndRebuildFromSameInputsRestoresSameNormalizedObservation()
        {
            var plan = Plan();
            var firstMaterializer = new ReferenceMaterializer();
            var published = firstMaterializer.Materialize(plan, Catalogue);
            var drifted = EffectiveResources().ToArray();
            drifted[1] = new ProjectionResource(
                drifted[1].ResourceId,
                drifted[1].ParentResourceId,
                drifted[1].ResourceKind,
                "bar-manually-edited",
                drifted[1].LogicalDependencies);

            var drift = firstMaterializer.Observe(plan, drifted, Catalogue);
            Assert.Equal(ProjectionDriftState.EngineDrift, drift.State);

            var rebuiltMaterializer = new ReferenceMaterializer();
            Assert.Equal(ProjectionDriftState.Absent, rebuiltMaterializer.Observe(plan, null, Catalogue).State);
            var rebuilt = rebuiltMaterializer.Materialize(plan, Catalogue);

            Assert.Equal(ProjectionDriftState.InSync, rebuilt.Observation.State);
            Assert.Equal(published.Receipt.GenerationId, rebuilt.Receipt.GenerationId);
            Assert.Equal(published.Observation.ObservationDigest, rebuilt.Observation.ObservationDigest);
        }

        [Fact]
        public void ProtocolAndRuntimeDoNotAcquireUpwardEngineBridgeDependency()
        {
            var protocolReferences = typeof(CapabilityKey).Assembly.GetReferencedAssemblies()
                .Select(value => value.Name).ToArray();
            var runtimeReferences = typeof(ComposedContract).Assembly.GetReferencedAssemblies()
                .Select(value => value.Name).ToArray();

            Assert.DoesNotContain("Arkus.EngineBridge", protocolReferences);
            Assert.DoesNotContain("Arkus.EngineBridge", runtimeReferences);
        }

        private static ProjectionPlan Plan(string catalogue = "catalogue-a")
        {
            var input = new ProjectionInput(
                new CanonicalProjectionAnchor("world.reference", 7, "world-hash-7"),
                "binding@1",
                catalogue,
                new BridgeToolchainProfile("reference", "1.0", "toolchain-a", "portable"),
                new byte[] { 1, 2, 3, 4 });

            return ProjectionPlanner.Create(input, EffectiveResources());
        }

        private static IEnumerable<ProjectionResource> EffectiveResources()
        {
            yield return new ProjectionResource("world/plaza", null, "root", "plaza-v1");
            yield return new ProjectionResource("world/plaza/bar", "world/plaza", "fixture", "bar-v1");
            yield return new ProjectionResource("world/plaza/market", "world/plaza", "fixture", "market-v1", new[] { "asset.market-stall" });
            yield return new ProjectionResource("world/plaza/workshop", "world/plaza", "fixture", "workshop-v1", new[] { "asset.workshop-kit" });
        }

        private static void AssertPortable(object? value)
        {
            if (value == null || value is string || value is bool || value is int || value is long)
                return;

            if (value is IReadOnlyDictionary<string, object?> dictionary)
            {
                foreach (var pair in dictionary)
                {
                    Assert.False(string.IsNullOrWhiteSpace(pair.Key));
                    AssertPortable(pair.Value);
                }
                return;
            }

            if (value is IEnumerable sequence)
            {
                foreach (var item in sequence)
                    AssertPortable(item);
                return;
            }

            throw new Xunit.Sdk.XunitException("Non-portable projection value: " + value.GetType().FullName);
        }
    }
}
