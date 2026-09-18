using System.IO;
using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Ownership decisions rest on containment and repository enumeration, so
    /// neither may create accidental proof escape hatches.
    /// </summary>
    public sealed class RepositoryPathsTests
    {
        [Fact]
        public void SiblingDirectoriesWithSharedPrefixAreNotContained()
        {
            Assert.False(RepositoryPaths.IsInside("/repo/src/Core", "/repo/src/CoreExtras/File.cs"));
            Assert.True(RepositoryPaths.IsInside("/repo/src/Core", "/repo/src/Core/File.cs"));
        }

        [Fact]
        public void NestedDirectoriesAreContained()
        {
            Assert.True(RepositoryPaths.IsInside("/repo", "/repo/src/Core/deep/File.cs"));
        }

        [Fact]
        public void DirectoryIsNotInsideItself()
        {
            Assert.False(RepositoryPaths.IsInside("/repo/src", "/repo/src"));
        }

        [Fact]
        public void RelativePathsUseForwardSlashes()
        {
            Assert.Equal("src/Core/File.cs", RepositoryPaths.ToRelative("/repo", "/repo/src/Core/File.cs"));
        }

        [Fact]
        public void PathsOutsideTheRepositoryStayAbsolute()
        {
            Assert.Equal("/elsewhere/File.cs", RepositoryPaths.ToRelative("/repo", "/elsewhere/File.cs"));
        }

        [Fact]
        public void OnlyDirectChildrenOfTheProofRootAreExcluded()
        {
            var root = Path.Combine(Path.GetTempPath(), "arkus-scan-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "artifacts"));
            Directory.CreateDirectory(Path.Combine(root, "keep", "artifacts"));
            Directory.CreateDirectory(Path.Combine(root, "keep", "obj"));

            try
            {
                File.WriteAllText(Path.Combine(root, "artifacts", "Generated.cs"), "// generated workspace");
                File.WriteAllText(Path.Combine(root, "keep", "artifacts", "Nested.cs"), "// must be seen");
                File.WriteAllText(Path.Combine(root, "keep", "obj", "Hidden.cs"), "// must be seen");

                var found = RepositoryPaths.EnumerateFiles(root, "*.cs", new[] { "artifacts" });

                Assert.Equal(2, found.Count);
                Assert.Contains(found, path => path.EndsWith("Nested.cs", System.StringComparison.Ordinal));
                Assert.Contains(found, path => path.EndsWith("Hidden.cs", System.StringComparison.Ordinal));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        [Fact]
        public void ArbitraryDirectoriesUnderRepositoryRootAreEnumerated()
        {
            var root = Path.Combine(Path.GetTempPath(), "arkus-full-scan-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "outside-old-roots", "deep"));

            try
            {
                var rogue = Path.Combine(root, "outside-old-roots", "deep", "Rogue.csproj");
                File.WriteAllText(rogue, "<Project />");

                var found = RepositoryPaths.EnumerateFiles(root, "*.csproj", System.Array.Empty<string>());

                Assert.Single(found);
                Assert.Equal(Path.GetFullPath(rogue), found[0]);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}
