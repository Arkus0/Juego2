using System;
using System.IO;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw02ReviewedHeaderSchemaTests
    {
        private const string AcceptedHeader =
            "| Programme ID | District/family | Place / family | Mobility anchor | Domain | A–D | S | Interior | Default access posture | Non-visual revisit reason |";

        [Fact]
        public void RenamingConsumedReviewedHeaderFailsClosedBeforeProjection()
        {
            var source = ReadProgramme();
            var changed = ReplaceAcceptedHeader(
                source,
                AcceptedHeader.Replace("District/family", "District family"));

            var error = Assert.Throws<CityProductionQueryException>(() =>
                new CityProductionQueryProvider().BuildAndValidate(changed));

            Assert.Equal("city.query_source_shape_invalid", error.MachineCode);
            Assert.Contains("header schema changed", error.Detail, StringComparison.Ordinal);
            Assert.Contains(CityProductionQueryProvider.ProgrammeSourcePath, error.SourcePaths);
        }

        [Fact]
        public void ReorderingConsumedReviewedHeadersFailsClosedBeforeProjection()
        {
            var source = ReadProgramme();
            var changedHeader = AcceptedHeader
                .Replace(
                    "District/family | Place / family",
                    "Place / family | District/family");
            var changed = ReplaceAcceptedHeader(source, changedHeader);

            var error = Assert.Throws<CityProductionQueryException>(() =>
                new CityProductionQueryProvider().BuildAndValidate(changed));

            Assert.Equal("city.query_source_shape_invalid", error.MachineCode);
            Assert.Contains("header schema changed", error.Detail, StringComparison.Ordinal);
        }

        [Fact]
        public void ManifestPinsCompleteAcceptedProgrammeHeaderSequence()
        {
            Assert.Equal(
                new[]
                {
                    "Programme ID",
                    "District/family",
                    "Place / family",
                    "Mobility anchor",
                    "Domain",
                    "A–D",
                    "S",
                    "Interior",
                    "Default access posture",
                    "Non-visual revisit reason"
                },
                CityProductionProjectionManifest.ProgrammeHeaders);
        }

        private static string ReplaceAcceptedHeader(string source, string replacement)
        {
            Assert.Contains(AcceptedHeader, source, StringComparison.Ordinal);
            var changed = source.Replace(AcceptedHeader, replacement, StringComparison.Ordinal);
            Assert.NotEqual(source, changed);
            return changed;
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
