using System;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw02MachineOutputTests
    {
        [Fact]
        public void ContentShapeArtifactCarriesProvenanceForEveryCountedSubject()
        {
            var source = ReadProgramme();
            var dataset = new CityProductionQueryProvider().BuildAndValidate(source);
            var artifact = CityProductionMachineOutput.BuildContentShapeArtifact(dataset);

            Assert.Equal(artifact.Report.TotalSubjects, artifact.Subjects.Count);
            Assert.Equal(
                dataset.Projection.Facts.Select(fact => fact.FactId),
                artifact.Subjects.Select(subject => subject.FactId));
            Assert.All(artifact.Subjects, subject =>
            {
                Assert.Equal(CityProductionQueryProvider.ProgrammeSourcePath, subject.SourcePath);
                Assert.False(string.IsNullOrWhiteSpace(subject.Anchor));
                Assert.False(string.IsNullOrWhiteSpace(subject.SourceDigest));
                Assert.False(string.IsNullOrWhiteSpace(subject.AnchorDigest));
            });

            var normalized = artifact.ToNormalizedText();
            Assert.Contains("artifact-schema=dw02-city-content-shape-artifact-v1", normalized, StringComparison.Ordinal);
            Assert.Contains("cost-model=UNMODELED", normalized, StringComparison.Ordinal);
            foreach (var fact in dataset.Projection.Facts)
            {
                Assert.Contains("subject\t" + fact.FactId + "\t", normalized, StringComparison.Ordinal);
                Assert.Contains(fact.Provenance.SourceDigest, normalized, StringComparison.Ordinal);
                Assert.Contains(fact.Provenance.AnchorDigest, normalized, StringComparison.Ordinal);
            }
        }

        [Fact]
        public void CanonicalMachineOutputIsStableAcrossCleanAndReverseEnumerationRebuilds()
        {
            var source = ReadProgramme();
            var first = new CityProductionQueryProvider().BuildAndValidate(source);
            var rebuilt = new CityProductionQueryProvider().BuildAndValidate(source);
            var reversed = new CityProductionQueryProvider(reverseEnumeration: true).BuildAndValidate(source);

            var expected = CityProductionMachineOutput.BuildNormalizedSnapshot(first);
            Assert.Equal(expected, CityProductionMachineOutput.BuildNormalizedSnapshot(rebuilt));
            Assert.Equal(expected, CityProductionMachineOutput.BuildNormalizedSnapshot(reversed));
        }

        private static string ReadProgramme()
        {
            var root = FindRepositoryRoot();
            return File.ReadAllText(Path.Combine(
                root,
                CityProductionQueryProvider.ProgrammeSourcePath.Replace('/', Path.DirectorySeparatorChar)));
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Juego2.sln"))) return directory.FullName;
                directory = directory.Parent;
            }
            throw new InvalidOperationException("Could not locate Juego2.sln from the test output directory.");
        }
    }
}
