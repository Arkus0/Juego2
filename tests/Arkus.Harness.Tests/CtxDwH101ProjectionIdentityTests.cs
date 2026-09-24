using System;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class CtxDwH101ProjectionIdentityTests
    {
        [Fact]
        public void ExactAcceptedAuthorityRebuildMatchesPublishedIdentity()
        {
            var root = FindRepositoryRoot();
            var sources = ReadSources(root);
            var published = PublishedIdentityBinding.Read(root);
            var forward = new H1DesignWorldProvider(false).BuildAndValidate(sources);
            var reverse = new H1DesignWorldProvider(true).BuildAndValidate(sources);

            Assert.True(published.Matches(forward));
            Assert.True(published.Matches(reverse));
            Assert.Equal(forward.Projection.Digest, reverse.Projection.Digest);
            Assert.Equal(forward.Projection.NormalizedRepresentation, reverse.Projection.NormalizedRepresentation);
            Assert.Equal(forward.ProjectionIdentity, reverse.ProjectionIdentity);
            Assert.Equal(H1ProjectionManifest.AdapterId + ":" + forward.Projection.Digest, forward.ProjectionIdentity);

            WriteProbe(forward, reverse);
        }

        [Fact]
        public void UnilateralPublishedIdentityChangeIsRed()
        {
            var root = FindRepositoryRoot();
            var dataset = new H1DesignWorldProvider().BuildAndValidate(ReadSources(root));
            var published = PublishedIdentityBinding.Read(root);

            Assert.True(published.Matches(dataset));
            Assert.False(new PublishedIdentityBinding(
                published.Digest,
                "tampered:" + published.Identity).Matches(dataset));
            Assert.False(new PublishedIdentityBinding(
                new string('0', 64),
                published.Identity).Matches(dataset));
        }

        private static void WriteProbe(H1CatalogueDataset forward, H1CatalogueDataset reverse)
        {
            var outputPath = Environment.GetEnvironmentVariable("CTX_DW_H1_01_IDENTITY_OUTPUT");
            if (string.IsNullOrWhiteSpace(outputPath)) return;

            var fullPath = Path.GetFullPath(outputPath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(
                fullPath,
                "Projection.Digest=" + forward.Projection.Digest + Environment.NewLine +
                "ProjectionIdentity=" + forward.ProjectionIdentity + Environment.NewLine +
                "ReverseProjectionIdentity=" + reverse.ProjectionIdentity + Environment.NewLine);
        }

        private static H1AcceptedAuthoritySources ReadSources(string root) => new H1AcceptedAuthoritySources(
            Read(root, H1ProjectionManifest.CataloguePath),
            Read(root, H1ProjectionManifest.SourceAdoptionPath));

        private static string Read(string root, string relativePath) =>
            File.ReadAllText(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar)));

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Juego2.sln"))) return directory.FullName;
                directory = directory.Parent;
            }

            throw new InvalidOperationException("Could not locate Juego2.sln from test output directory.");
        }

        private sealed class PublishedIdentityBinding
        {
            public PublishedIdentityBinding(string digest, string identity)
            {
                Digest = digest;
                Identity = identity;
            }

            public string Digest { get; }
            public string Identity { get; }

            public bool Matches(H1CatalogueDataset dataset) =>
                StringComparer.Ordinal.Equals(Digest, dataset.Projection.Digest) &&
                StringComparer.Ordinal.Equals(Identity, dataset.ProjectionIdentity) &&
                StringComparer.Ordinal.Equals(Identity, H1ProjectionManifest.AdapterId + ":" + Digest);

            public static PublishedIdentityBinding Read(string root)
            {
                var lifecycle = File.ReadAllLines(Path.Combine(
                    root,
                    "Docs",
                    "evidence",
                    "WP-CTX-DW-H1-01",
                    "LIFECYCLE.md"));
                var digest = ExtractSingle(lifecycle, "- Projection.Digest: `");
                var identity = ExtractSingle(lifecycle, "- ProjectionIdentity: `");
                Assert.Equal(64, digest.Length);
                Assert.Equal(H1ProjectionManifest.AdapterId + ":" + digest, identity);
                return new PublishedIdentityBinding(digest, identity);
            }

            private static string ExtractSingle(string[] lines, string prefix)
            {
                var matches = lines
                    .Where(line => line.StartsWith(prefix, StringComparison.Ordinal) && line.EndsWith("`", StringComparison.Ordinal))
                    .Select(line => line.Substring(prefix.Length, line.Length - prefix.Length - 1))
                    .ToArray();
                Assert.Single(matches);
                return matches[0];
            }
        }
    }
}
