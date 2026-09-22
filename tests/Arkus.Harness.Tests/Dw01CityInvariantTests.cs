using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw01CityInvariantTests
    {
        [Fact]
        public void AcceptedCitySourcesProjectCompleteAccessAndInteriorUniverses()
        {
            var sources = ReadSources();
            var slice = new CityDesignWorldProvider().BuildAndValidate(
                sources.Programme, sources.Bindings, sources.Interiors);

            Assert.Equal(23, slice.AccessPlaceCount);
            Assert.Equal(14, slice.InteriorPlaceCount);
            Assert.Equal(23, slice.ProgrammeProjection.Facts.Count);
            Assert.Equal(23, slice.BindingProjection.Facts.Count);
            Assert.Equal(28, slice.InteriorProjection.Facts.Count);
            Assert.All(slice.ProgrammeProjection.Facts, fact =>
                Assert.Equal(CityDesignWorldProvider.ProgrammeSourcePath, fact.Provenance.SourcePath));
            Assert.All(slice.BindingProjection.Facts, fact =>
                Assert.Equal(CityDesignWorldProvider.BindingSourcePath, fact.Provenance.SourcePath));
            Assert.All(slice.InteriorProjection.Facts, fact =>
                Assert.Equal(CityDesignWorldProvider.InteriorsSourcePath, fact.Provenance.SourcePath));
            Assert.All(slice.ProgrammeProjection.WorldState.Objects, item =>
                Assert.Equal(DesignWorldProjector.GenericWorldType, item.TypeId.Value));
            Assert.All(slice.BindingProjection.WorldState.Objects, item =>
                Assert.Equal(DesignWorldProjector.GenericWorldType, item.TypeId.Value));
            Assert.All(slice.InteriorProjection.WorldState.Objects, item =>
                Assert.Equal(DesignWorldProjector.GenericWorldType, item.TypeId.Value));
        }

        [Fact]
        public void RemovingOneInheritedRequiredAccessRoleMakesPipelineRed()
        {
            var sources = ReadSources();
            var mutated = MutateLineAfterHeading(
                sources.Bindings,
                "## 9. A/B place → exterior composition mapping",
                "`loc.casco.bar`",
                line => line.Replace("{public, service, semi-private}", "{public, semi-private}"));

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityDesignWorldProvider().BuildAndValidate(sources.Programme, mutated, sources.Interiors));

            Assert.Equal("city.access_role_missing", error.MachineCode);
            Assert.Equal("loc.casco.bar", error.SubjectId);
            Assert.Contains("service", error.Detail, StringComparison.Ordinal);
            Assert.Contains(CityDesignWorldProvider.ProgrammeSourcePath, error.SourcePaths);
            Assert.Contains(CityDesignWorldProvider.BindingSourcePath, error.SourcePaths);
        }

        [Fact]
        public void RemovingEntireBoundRoleSurfaceCannotSelfShrinkTheOracle()
        {
            var sources = ReadSources();
            var mutated = MutateLineAfterHeading(
                sources.Bindings,
                "## 9. A/B place → exterior composition mapping",
                "`loc.casco.bar`",
                line => line.Replace("{public, service, semi-private}", "{}"));

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityDesignWorldProvider().BuildAndValidate(sources.Programme, mutated, sources.Interiors));

            Assert.Equal("city.access_role_missing", error.MachineCode);
            Assert.Equal("loc.casco.bar", error.SubjectId);
        }

        [Fact]
        public void OmittingOneBindingRowKeepsProgrammeUniverseIndependentAndRed()
        {
            var sources = ReadSources();
            var mutated = DeleteLineAfterHeading(
                sources.Bindings,
                "## 9. A/B place → exterior composition mapping",
                "`loc.casco.bar`");

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityDesignWorldProvider().BuildAndValidate(sources.Programme, mutated, sources.Interiors));

            Assert.Equal("city.access_binding_subject_missing", error.MachineCode);
            Assert.Equal("loc.casco.bar", error.SubjectId);
        }

        [Fact]
        public void OmittingOneFullInteriorAllocationKeepsCity02InteriorUniverseIndependentAndRed()
        {
            var sources = ReadSources();
            var mutated = DeleteLineAfterHeading(
                sources.Interiors,
                "## 6. Full I1–I3 allocation",
                "`loc.casco.bar`");

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityDesignWorldProvider().BuildAndValidate(sources.Programme, sources.Bindings, mutated));

            Assert.Equal("city.interior_allocation_missing", error.MachineCode);
            Assert.Equal("loc.casco.bar", error.SubjectId);
        }

        [Fact]
        public void WeakeningInheritedInteriorDepthMakesSecondSemanticOracleRed()
        {
            var sources = ReadSources();
            var mutated = MutateLineAfterHeading(
                sources.Interiors,
                "## 5. Inherited access roles inside interiors",
                "`loc.casco.bar`",
                line => line.Replace("| I3 |", "| I2 |"));

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityDesignWorldProvider().BuildAndValidate(sources.Programme, sources.Bindings, mutated));

            Assert.Equal("city.interior_depth_mismatch", error.MachineCode);
            Assert.Equal("loc.casco.bar", error.SubjectId);
            Assert.Contains("I3", error.Detail, StringComparison.Ordinal);
            Assert.Contains("I2", error.Detail, StringComparison.Ordinal);
        }

        [Fact]
        public void AllocatingAnI0PlaceIsRejectedAsAnExtraDownstreamSubject()
        {
            var sources = ReadSources();
            const string extra = "| `loc.plaza.market` | `if.invalid` | public exterior only | — | — |";
            var mutated = InsertDataRowAfterHeader(
                sources.Interiors,
                "## 6. Full I1–I3 allocation",
                extra);

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityDesignWorldProvider().BuildAndValidate(sources.Programme, sources.Bindings, mutated));

            Assert.Equal("city.interior_allocation_unexpected", error.MachineCode);
            Assert.Equal("loc.plaza.market", error.SubjectId);
        }

        [Fact]
        public void DuplicateInteriorAllocationFailsClosedBeforeProjection()
        {
            var sources = ReadSources();
            var duplicated = DuplicateLineAfterHeading(
                sources.Interiors,
                "## 6. Full I1–I3 allocation",
                "`loc.casco.bar`");

            var error = Assert.Throws<CityInvariantException>(() =>
                new CityDesignWorldProvider().BuildAndValidate(sources.Programme, sources.Bindings, duplicated));

            Assert.Equal("city.interior_allocation_duplicate", error.MachineCode);
            Assert.Equal("loc.casco.bar", error.SubjectId);
        }

        [Fact]
        public void EqualLookingValuesWithChangedAuthorityBytesFailInheritedProvenance()
        {
            var sources = ReadSources();
            var provider = new CityDesignWorldProvider();
            var slice = provider.BuildAndValidate(sources.Programme, sources.Bindings, sources.Interiors);
            var changedBindings = sources.Bindings + "\n<!-- same semantic table, changed accepted bytes -->\n";

            var issues = provider.ValidateProvenanceAgainstSources(
                slice, sources.Programme, changedBindings, sources.Interiors);

            Assert.Contains(issues, issue => issue.MachineCode == "dw.provenance_stale");
        }

        [Fact]
        public void ReverseDefinitionEnumerationAndFreshRebuildPreserveCanonicalDigests()
        {
            var sources = ReadSources();
            var first = new CityDesignWorldProvider().BuildAndValidate(
                sources.Programme, sources.Bindings, sources.Interiors);
            var rebuilt = new CityDesignWorldProvider().BuildAndValidate(
                sources.Programme, sources.Bindings, sources.Interiors);
            var reversed = new CityDesignWorldProvider(reverseEnumeration: true).BuildAndValidate(
                sources.Programme, sources.Bindings, sources.Interiors);

            Assert.Equal(first.ProgrammeProjection.Digest, rebuilt.ProgrammeProjection.Digest);
            Assert.Equal(first.BindingProjection.Digest, rebuilt.BindingProjection.Digest);
            Assert.Equal(first.InteriorProjection.Digest, rebuilt.InteriorProjection.Digest);
            Assert.Equal(first.ProgrammeProjection.Digest, reversed.ProgrammeProjection.Digest);
            Assert.Equal(first.BindingProjection.Digest, reversed.BindingProjection.Digest);
            Assert.Equal(first.InteriorProjection.Digest, reversed.InteriorProjection.Digest);
            Assert.Equal(first.ProgrammeProjection.NormalizedRepresentation, reversed.ProgrammeProjection.NormalizedRepresentation);
            Assert.Equal(first.BindingProjection.NormalizedRepresentation, reversed.BindingProjection.NormalizedRepresentation);
            Assert.Equal(first.InteriorProjection.NormalizedRepresentation, reversed.InteriorProjection.NormalizedRepresentation);
        }

        private static CitySources ReadSources()
        {
            var root = FindRepositoryRoot();
            return new CitySources(
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.ProgrammeSourcePath)),
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.BindingSourcePath)),
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.InteriorsSourcePath)));
        }

        private static string MutateLineAfterHeading(string source, string heading, string marker, Func<string, string> mutation)
        {
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList();
            var headingIndex = lines.FindIndex(line => line.Trim() == heading);
            Assert.True(headingIndex >= 0, "heading not found: " + heading);
            for (var index = headingIndex + 1; index < lines.Count; index++)
            {
                if (lines[index].StartsWith("## ", StringComparison.Ordinal)) break;
                if (lines[index].Contains(marker, StringComparison.Ordinal))
                {
                    var changed = mutation(lines[index]);
                    Assert.NotEqual(lines[index], changed);
                    lines[index] = changed;
                    return string.Join("\n", lines);
                }
            }
            throw new InvalidOperationException("row not found: " + marker);
        }

        private static string DeleteLineAfterHeading(string source, string heading, string marker)
        {
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList();
            var headingIndex = lines.FindIndex(line => line.Trim() == heading);
            Assert.True(headingIndex >= 0, "heading not found: " + heading);
            for (var index = headingIndex + 1; index < lines.Count; index++)
            {
                if (lines[index].StartsWith("## ", StringComparison.Ordinal)) break;
                if (lines[index].Contains(marker, StringComparison.Ordinal))
                {
                    lines.RemoveAt(index);
                    return string.Join("\n", lines);
                }
            }
            throw new InvalidOperationException("row not found: " + marker);
        }

        private static string DuplicateLineAfterHeading(string source, string heading, string marker)
        {
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList();
            var headingIndex = lines.FindIndex(line => line.Trim() == heading);
            Assert.True(headingIndex >= 0, "heading not found: " + heading);
            for (var index = headingIndex + 1; index < lines.Count; index++)
            {
                if (lines[index].StartsWith("## ", StringComparison.Ordinal)) break;
                if (lines[index].Contains(marker, StringComparison.Ordinal))
                {
                    lines.Insert(index + 1, lines[index]);
                    return string.Join("\n", lines);
                }
            }
            throw new InvalidOperationException("row not found: " + marker);
        }

        private static string InsertDataRowAfterHeader(string source, string heading, string row)
        {
            var lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList();
            var headingIndex = lines.FindIndex(line => line.Trim() == heading);
            Assert.True(headingIndex >= 0, "heading not found: " + heading);
            for (var index = headingIndex + 1; index < lines.Count - 1; index++)
            {
                if (lines[index].TrimStart().StartsWith("| Place |", StringComparison.Ordinal))
                {
                    lines.Insert(index + 2, row);
                    return string.Join("\n", lines);
                }
            }
            throw new InvalidOperationException("allocation table header not found");
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

        private sealed class CitySources
        {
            public CitySources(string programme, string bindings, string interiors)
            {
                Programme = programme;
                Bindings = bindings;
                Interiors = interiors;
            }
            public string Programme { get; }
            public string Bindings { get; }
            public string Interiors { get; }
        }
    }
}
