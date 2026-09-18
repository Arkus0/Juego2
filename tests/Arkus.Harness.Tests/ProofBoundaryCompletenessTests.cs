using System;
using System.IO;
using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Pins the proof-completeness boundary independently from the manifest it
    /// evaluates. This is the regression suite for the WP-HK-00 review failure.
    /// </summary>
    public sealed class ProofBoundaryCompletenessTests
    {
        [Fact]
        public void RepositoryBoundaryIsProofOwnedAndCoversTheWholeCheckout()
        {
            var manifest = KernelManifest.Load(RepositoryLocator.ManifestPath());

            Assert.Equal(new[] { "." }, manifest.Repository.SourceScanRoots);
            Assert.Equal(new[] { ".git", "artifacts" }, manifest.Repository.ExcludedDirectoryNames);
        }

        [Fact]
        public void LegacyManifestOwnedScanBoundaryFailsClosed()
        {
            var directory = Path.Combine(
                Path.GetTempPath(),
                "arkus-proof-boundary-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            try
            {
                var path = Path.Combine(directory, "manifest.json");
                File.WriteAllText(
                    path,
                    "{\"schemaVersion\":1,\"repository\":{\"sourceScanRoots\":[\"src\"]}}");

                Assert.Throws<ProofToolException>(() => KernelManifest.Load(path));
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }
    }
}
