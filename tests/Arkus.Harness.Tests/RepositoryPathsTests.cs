using System.IO;
using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Ownership decisions rest on containment, so containment must not be
    /// prefix-matching by accident.
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
        public void ExcludedDirectoriesAreSkippedAtAnyDepth()
        {
            var root = Path.Combine(Path.GetTempPath(), "arkus-scan-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "keep", "deep"));
            Directory.CreateDirectory(Path.Combine(root, "keep", "obj"));

            try
            {
                File.WriteAllText(Path.Combine(root, "keep", "deep", "Kept.cs"), "// kept");
                File.WriteAllText(Path.Combine(root, "keep", "obj", "Generated.cs"), "// generated");

                var found = RepositoryPaths.EnumerateFiles(root, "*.cs", new[] { "obj" });

                Assert.Single(found);
                Assert.EndsWith("Kept.cs", found[0], System.StringComparison.Ordinal);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}
