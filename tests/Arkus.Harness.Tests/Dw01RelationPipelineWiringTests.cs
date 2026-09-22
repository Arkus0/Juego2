using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw01RelationPipelineWiringTests
    {
        [Fact]
        public void BuildAndValidateExecutesRelationOracleAfterGenericProjectionValidation()
        {
            var root = FindRepositoryRoot();
            var programme = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.ProgrammeSourcePath));
            var bindings = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.BindingSourcePath));
            var interiors = File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.InteriorsSourcePath));
            var targetSubject = ParseFirstCity02InteriorId(programme);
            var injected = false;

            var provider = new CityDesignWorldProvider(
                reverseEnumeration: false,
                interiorRelationTransform: (subjectId, canonicalRelations) =>
                {
                    if (!StringComparer.Ordinal.Equals(subjectId, targetSubject))
                    {
                        return canonicalRelations;
                    }

                    injected = true;
                    var relation = Assert.Single(canonicalRelations);
                    Assert.Equal("allocates-depth", relation.RelationType);
                    Assert.Equal("depth." + targetSubject, relation.TargetFactId);
                    return Array.Empty<DesignRelation>();
                });

            var error = Assert.Throws<CityInvariantException>(() =>
                provider.BuildAndValidate(programme, bindings, interiors));

            Assert.True(injected, "The fault injection must remove only the canonical relation for a real CITY-02 I1-I3 subject.");
            Assert.Equal("city.interior_allocation_relation_missing", error.MachineCode);
            Assert.Equal(targetSubject, error.SubjectId);
            Assert.Contains("allocates-depth", error.Rule, StringComparison.Ordinal);
            Assert.Contains("allocation." + targetSubject, error.Detail, StringComparison.Ordinal);
            Assert.Contains("observed relations: <none>", error.Detail, StringComparison.Ordinal);
            Assert.Contains(CityDesignWorldProvider.ProgrammeSourcePath, error.SourcePaths);
            Assert.Contains(CityDesignWorldProvider.InteriorsSourcePath, error.SourcePaths);
        }

        private static string ParseFirstCity02InteriorId(string programmeSource)
        {
            const string heading = "## 4. District × location programme";
            var lines = programmeSource.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var headingIndex = Array.FindIndex(lines, line => line.Trim() == heading);
            Assert.True(headingIndex >= 0, "CITY-02 programme heading not found.");

            var sawHeader = false;
            for (var index = headingIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (line.StartsWith("## ", StringComparison.Ordinal)) break;
                if (!line.StartsWith("|", StringComparison.Ordinal)) continue;

                var cells = line.Substring(1, line.Length - 2)
                    .Split('|')
                    .Select(Clean)
                    .ToArray();
                if (cells.Length != 10) continue;
                if (cells[0] == "Programme ID")
                {
                    sawHeader = true;
                    continue;
                }
                if (!sawHeader || cells[0].StartsWith("---", StringComparison.Ordinal)) continue;

                if (cells[7].StartsWith("I1", StringComparison.Ordinal) ||
                    cells[7].StartsWith("I2", StringComparison.Ordinal) ||
                    cells[7].StartsWith("I3", StringComparison.Ordinal))
                {
                    return cells[0];
                }
            }

            throw new InvalidOperationException("CITY-02 contains no I1-I3 subject for the relation wiring proof.");
        }

        private static string Clean(string value) =>
            value.Replace("**", string.Empty).Replace("`", string.Empty).Trim();

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
