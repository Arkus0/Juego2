using System;
using System.Collections.Generic;
using System.IO;
using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Pins the WP-HK-00 module direction as an executable contract.
    /// </summary>
    /// <remarks>
    /// The workpack states the required direction in prose. Encoding it here means
    /// a future edit to the manifest that quietly changes the boundary fails a
    /// test, instead of only changing a document nobody re-reads.
    /// </remarks>
    public sealed class KernelManifestTests
    {
        private static readonly Dictionary<string, string[]> RequiredDirection =
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["Arkus.Harness.Protocol"] = Array.Empty<string>(),
                ["Arkus.Game.Core"] = Array.Empty<string>(),
                ["Arkus.Game.World"] = new[] { "Arkus.Game.Core" },
                ["Arkus.Game.Authoring"] = new[] { "Arkus.Game.Core", "Arkus.Game.World", "Arkus.Harness.Protocol" },
                ["Arkus.Game.Validation"] = new[] { "Arkus.Game.Core", "Arkus.Game.World", "Arkus.Harness.Protocol" },
                ["Arkus.Harness.Runtime"] = new[]
                {
                    "Arkus.Game.Authoring",
                    "Arkus.Game.Validation",
                    "Arkus.Harness.Protocol",
                },
                ["Arkus.Harness.Cli"] = new[] { "Arkus.Harness.Runtime" },
            };

        [Fact]
        public void ManifestDeclaresExactlyTheRequiredModuleDirection()
        {
            var manifest = KernelManifest.Load(RepositoryLocator.ManifestPath());

            foreach (var required in RequiredDirection)
            {
                var project = manifest.FindProject(required.Key);
                Assert.NotNull(project);

                var declared = new List<string>(project!.DependsOn);
                declared.Sort(StringComparer.Ordinal);

                var expected = new List<string>(required.Value);
                expected.Sort(StringComparer.Ordinal);

                Assert.Equal(expected, declared);
            }
        }

        [Fact]
        public void PortableKernelProjectsTargetAUnityCompatibleContract()
        {
            var manifest = KernelManifest.Load(RepositoryLocator.ManifestPath());
            var policy = manifest.ProjectClasses["portable-kernel"];

            Assert.Equal("netstandard2.1", policy.RequiredProperties["TargetFramework"]);
            Assert.Equal("9.0", policy.RequiredProperties["LangVersion"]);
            Assert.Equal("true", policy.RequiredProperties["TreatWarningsAsErrors"]);
            Assert.False(policy.AllowPackageReferences);
            Assert.False(policy.AllowExternalCompiledSources);
            Assert.True(policy.EngineFree);
            Assert.True(policy.RequireDeclaredEdgesExercised);
        }

        [Fact]
        public void EveryProjectHasAKnownClassAndResolvableDependencies()
        {
            var manifest = KernelManifest.Load(RepositoryLocator.ManifestPath());

            foreach (var project in manifest.Projects)
            {
                Assert.True(
                    manifest.ProjectClasses.ContainsKey(project.ClassName),
                    project.Name + " has unknown class " + project.ClassName);

                foreach (var dependency in project.DependsOn)
                {
                    Assert.NotNull(manifest.FindProject(dependency));
                }
            }
        }

        [Fact]
        public void EveryProjectPathExistsOnDisk()
        {
            var manifest = KernelManifest.Load(RepositoryLocator.ManifestPath());

            foreach (var project in manifest.Projects)
            {
                Assert.True(
                    File.Exists(Path.Combine(RepositoryLocator.Root(), project.Path)),
                    project.Path + " is declared but missing");
            }
        }

        [Fact]
        public void NoProductionClassMayTakeNuGetOrExternalSources()
        {
            var manifest = KernelManifest.Load(RepositoryLocator.ManifestPath());

            foreach (var entry in manifest.ProjectClasses)
            {
                if (!entry.Value.Production)
                {
                    continue;
                }

                Assert.False(entry.Value.AllowPackageReferences, entry.Key + " must not allow packages");
                Assert.False(entry.Value.AllowExternalCompiledSources, entry.Key + " must not allow external sources");
                Assert.True(entry.Value.EngineFree, entry.Key + " must be engine free");
            }
        }

        [Fact]
        public void MalformedManifestsFailClosed()
        {
            var directory = Path.Combine(Path.GetTempPath(), "arkus-manifest-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            try
            {
                var missing = Path.Combine(directory, "absent.json");
                Assert.Throws<ProofToolException>(() => KernelManifest.Load(missing));

                var malformed = Path.Combine(directory, "malformed.json");
                File.WriteAllText(malformed, "{ not json");
                Assert.Throws<ProofToolException>(() => KernelManifest.Load(malformed));

                var wrongSchema = Path.Combine(directory, "schema.json");
                File.WriteAllText(wrongSchema, "{ \"schemaVersion\": 99 }");
                Assert.Throws<ProofToolException>(() => KernelManifest.Load(wrongSchema));

                var empty = Path.Combine(directory, "empty.json");
                File.WriteAllText(empty, "{ \"schemaVersion\": 1, \"projects\": [] }");
                Assert.Throws<ProofToolException>(() => KernelManifest.Load(empty));
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }
    }
}
