using System;
using System.Collections;
using System.Collections.Generic;
using Arkus.EngineBridge;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1PortableProjectionTests
    {
        [Fact]
        public void PortableContractIsEngineNeutralMachineReadableAndReceiptCarriesDiagnostics()
        {
            var plan = Plan();
            var materializer = new ReferenceMaterializer();
            var failed = materializer.Materialize(
                plan,
                new[] { "asset.market-stall", "asset.workshop-kit" },
                ReferenceFailurePoint.BeforePublication);

            var data = ProjectionPortableData.Materialization(failed);
            AssertPortable(data);

            var receipt = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(data["receipt"]);
            var observation = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(data["observation"]);
            var diagnostics = Assert.IsAssignableFrom<IReadOnlyList<object?>>(receipt["diagnostics"]);

            Assert.Equal("not-published", receipt["publicationStatus"]);
            Assert.Equal("failed", observation["state"]);
            Assert.Contains("reference.failure.before-publication", diagnostics);
            Assert.Equal(failed.Observation.ObservationDigest, receipt["observationDigest"]);
            Assert.Equal(plan.Input.CatalogueFingerprint, receipt["catalogueFingerprint"]);
            Assert.Equal(plan.Input.Profile.Fingerprint, receipt["bridgeProfileFingerprint"]);
        }

        [Fact]
        public void PortableReceiptFailsClosedWhenPairedWithDifferentObservation()
        {
            var plan = Plan();
            var first = new ReferenceMaterializer().Materialize(
                plan,
                new[] { "asset.market-stall", "asset.workshop-kit" },
                ReferenceFailurePoint.BeforePublication);
            var second = new ReferenceMaterializer().Materialize(
                plan,
                new[] { "asset.market-stall", "asset.workshop-kit" },
                ReferenceFailurePoint.BeforeValidation);

            Assert.NotEqual(first.Observation.ObservationDigest, second.Observation.ObservationDigest);
            Assert.Throws<ArgumentException>(() => ProjectionPortableData.Receipt(first.Receipt, second.Observation));
        }

        private static ProjectionPlan Plan()
        {
            var input = new ProjectionInput(
                new CanonicalProjectionAnchor("world.reference", 7, "world-hash-7"),
                "binding@1",
                "catalogue-a",
                new BridgeToolchainProfile("reference", "1.0", "toolchain-a", "portable"),
                new byte[] { 1, 2, 3, 4 });

            return ProjectionPlanner.Create(
                input,
                new[]
                {
                    new ProjectionResource("world/plaza", null, "root", "plaza-v1"),
                    new ProjectionResource("world/plaza/market", "world/plaza", "fixture", "market-v1", new[] { "asset.market-stall" }),
                    new ProjectionResource("world/plaza/bar", "world/plaza", "fixture", "bar-v1"),
                    new ProjectionResource("world/plaza/workshop", "world/plaza", "fixture", "workshop-v1", new[] { "asset.workshop-kit" })
                });
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
