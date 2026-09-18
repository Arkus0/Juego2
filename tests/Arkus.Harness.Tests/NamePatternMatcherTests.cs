using System;
using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Forbidden-name matching must catch engine assemblies without catching
    /// unrelated names that merely contain the same words.
    /// </summary>
    public sealed class NamePatternMatcherTests
    {
        private static readonly string[] Patterns =
        {
            "UnityEngine",
            "UnityEngine.*",
            "UnityEditor",
            "Unity.*",
            "DaggerfallWorkshop*",
        };

        [Theory]
        [InlineData("UnityEngine", true)]
        [InlineData("unityengine", true)]
        [InlineData("UnityEngine.CoreModule", true)]
        [InlineData("UnityEditor", true)]
        [InlineData("Unity.Collections", true)]
        [InlineData("DaggerfallWorkshop", true)]
        [InlineData("DaggerfallWorkshop.Game", true)]
        [InlineData("Arkus.Game.Core", false)]
        [InlineData("CommunityToolkit", false)]
        [InlineData("MyUnityEngineHelper", false)]
        [InlineData("", false)]
        public void PatternsMatchEngineAssembliesOnly(string name, bool expected)
        {
            var matcher = new NamePatternMatcher(Patterns);

            Assert.Equal(expected, matcher.IsMatch(name));
        }

        [Fact]
        public void EmptyPatternIsRejectedInsteadOfMatchingEverything()
        {
            Assert.Throws<ProofToolException>(() => new NamePatternMatcher(new[] { string.Empty }));
        }

        [Fact]
        public void DotsAreLiteralNotRegexWildcards()
        {
            var matcher = new NamePatternMatcher(new[] { "Unity.Collections" });

            Assert.False(matcher.IsMatch("UnityXCollections"));
            Assert.True(matcher.IsMatch("Unity.Collections"));
        }

        [Fact]
        public void ManifestPatternsCoverTheDocumentedForbiddenSet()
        {
            var manifest = KernelManifest.Load(RepositoryLocator.ManifestPath());
            var matcher = new NamePatternMatcher(manifest.ForbiddenAssemblyNamePatterns);

            foreach (var forbidden in new[]
                     {
                         "UnityEngine",
                         "UnityEngine.CoreModule",
                         "UnityEditor",
                         "UnityEditor.Graphs",
                         "Unity.TextMeshPro",
                         "Assembly-CSharp",
                         "DaggerfallWorkshop",
                         "DaggerfallConnect",
                     })
            {
                Assert.True(matcher.IsMatch(forbidden), forbidden + " must be forbidden");
            }

            foreach (var allowed in new[] { "Arkus.Game.Core", "netstandard", "System.Runtime", "xunit.core" })
            {
                Assert.False(matcher.IsMatch(allowed), allowed + " must not be forbidden");
            }
        }

        [Fact]
        public void NullPatternListIsRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new NamePatternMatcher(null!));
        }
    }
}
