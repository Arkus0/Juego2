using System;
using System.Collections.Generic;
using System.IO;
using Arkus.Game.World;
using Arkus.Harness.Runtime;
using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Reads the shipped binaries rather than the project files, so the boundary
    /// claim is checked against what the compiler actually produced.
    /// </summary>
    public sealed class EffectiveAssemblyTests
    {
        [Fact]
        public void PortableKernelAssemblyTargetsNetStandard21()
        {
            var facts = AssemblyFacts.Read(LocationOf(typeof(WorldModule)));

            Assert.Equal(KernelProofRunner.FrameworkMoniker("netstandard2.1"), facts.TargetFramework);
        }

        [Fact]
        public void WorldActuallyReferencesCoreAndNothingElseFromTheKernel()
        {
            var facts = AssemblyFacts.Read(LocationOf(typeof(WorldModule)));
            var kernelReferences = KernelReferences(facts);

            Assert.Equal(new List<string> { "Arkus.Game.Core" }, kernelReferences);
        }

        [Fact]
        public void RuntimeDoesNotReachPastAuthoringAndValidation()
        {
            var facts = AssemblyFacts.Read(LocationOf(typeof(RuntimeModule)));
            var kernelReferences = KernelReferences(facts);

            Assert.Equal(
                new List<string>
                {
                    "Arkus.Game.Authoring",
                    "Arkus.Game.Validation",
                    "Arkus.Harness.Protocol",
                },
                kernelReferences);
            Assert.DoesNotContain("Arkus.Game.Core", kernelReferences);
            Assert.DoesNotContain("Arkus.Game.World", kernelReferences);
        }

        [Fact]
        public void NoKernelAssemblyReferencesAForbiddenEngineAssembly()
        {
            var manifest = KernelManifest.Load(RepositoryLocator.ManifestPath());
            var matcher = new NamePatternMatcher(manifest.ForbiddenAssemblyNamePatterns);

            foreach (var type in new[] { typeof(WorldModule), typeof(RuntimeModule) })
            {
                foreach (var reference in AssemblyFacts.Read(LocationOf(type)).AssemblyReferences)
                {
                    Assert.False(matcher.IsMatch(reference), reference + " is forbidden");
                }
            }
        }

        [Fact]
        public void CompiledSourceMatchesTheFilesOnDisk()
        {
            var facts = AssemblyFacts.Read(LocationOf(typeof(WorldModule)));
            var repositoryRoot = RepositoryLocator.Root();
            var checkedFiles = 0;

            foreach (var document in facts.CompiledDocuments)
            {
                if (!RepositoryPaths.IsInside(Path.Combine(repositoryRoot, "src"), document.Path))
                {
                    continue;
                }

                Assert.Equal(AssemblyFacts.Sha256Algorithm, document.HashAlgorithm);
                Assert.True(File.Exists(document.Path), document.Path + " must exist");
                Assert.True(
                    AssemblyFacts.HashEquals(document.Hash, AssemblyFacts.Sha256OfFile(document.Path)),
                    document.Path + " content differs from what was compiled");
                checkedFiles++;
            }

            Assert.True(checkedFiles > 0, "the effective-compilation oracle inspected no owned source");
        }

        [Fact]
        public void MissingPortablePdbFailsClosed()
        {
            var directory = Path.Combine(Path.GetTempPath(), "arkus-asm-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            try
            {
                var copy = Path.Combine(directory, "Arkus.Game.World.dll");
                File.Copy(LocationOf(typeof(WorldModule)), copy);

                Assert.Throws<ProofToolException>(() => AssemblyFacts.Read(copy));
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        [Fact]
        public void UnknownTargetFrameworkFailsClosedInsteadOfPassing()
        {
            Assert.Throws<ProofToolException>(() => KernelProofRunner.FrameworkMoniker("net48"));
        }

        private static string LocationOf(Type type)
        {
            var location = type.Assembly.Location;
            Assert.False(string.IsNullOrEmpty(location), "assembly location must be a real file");
            return location;
        }

        private static List<string> KernelReferences(AssemblyFacts facts)
        {
            var references = new List<string>();
            foreach (var reference in facts.AssemblyReferences)
            {
                if (reference.StartsWith("Arkus.", StringComparison.Ordinal))
                {
                    references.Add(reference);
                }
            }

            references.Sort(StringComparer.Ordinal);
            return references;
        }
    }
}
