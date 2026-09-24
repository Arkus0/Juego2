using System;
using System.IO;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class CtxDwH101ProjectionIdentityTests
    {
        [Fact]
        public void ExactAcceptedAuthorityRebuildProducesStableConcreteIdentity()
        {
            var root = FindRepositoryRoot();
            var sources = new H1AcceptedAuthoritySources(
                Read(root, H1ProjectionManifest.CataloguePath),
                Read(root, H1ProjectionManifest.SourceAdoptionPath));

            var forward = new H1DesignWorldProvider(false).BuildAndValidate(sources);
            var reverse = new H1DesignWorldProvider(true).BuildAndValidate(sources);

            Assert.Equal(forward.Projection.Digest, reverse.Projection.Digest);
            Assert.Equal(forward.ProjectionIdentity, reverse.ProjectionIdentity);
            Assert.Equal(H1ProjectionManifest.AdapterId + ":" + forward.Projection.Digest, forward.ProjectionIdentity);

            var outputPath = Environment.GetEnvironmentVariable("CTX_DW_H1_01_IDENTITY_OUTPUT");
            if (!string.IsNullOrWhiteSpace(outputPath))
            {
                var fullPath = Path.GetFullPath(outputPath);
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                File.WriteAllText(
                    fullPath,
                    "Projection.Digest=" + forward.Projection.Digest + Environment.NewLine +
                    "ProjectionIdentity=" + forward.ProjectionIdentity + Environment.NewLine +
                    "ReverseProjectionIdentity=" + reverse.ProjectionIdentity + Environment.NewLine);
            }
        }

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
    }
}
