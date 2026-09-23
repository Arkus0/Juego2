using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw05TransitiveKernelLeakageTests
    {
        private static readonly string[] ExactDomainSemanticLiterals =
        {
            // CITY adopted fact/field/relation vocabulary.
            "city-programme-place",
            "city-access-binding",
            "city-interior-depth",
            "city-interior-allocation",
            "allocates-depth",
            "interior-depth",
            "required-role-",

            // PA adopted types plus prefix-free semantic forms that would still
            // constitute hidden PA behavior if hard-coded into the generic seam.
            "pa-corpus-root",
            "pa-finding",
            "pa-disposition",
            "pa-evidence",
            "pa-fixture",
            "pa-invariant",
            "pa-failure-mode",
            "finding",
            "disposition",
            "evidence",
            "contains-finding",
            "contains-disposition",
            "contains-evidence",
            "contains-fixture",
            "contains-invariant",
            "contains-failure-mode",
            "has-disposition",
            "same-authority-evidence",
            "same-authority-fixture"
        };

        [Fact]
        public void DesignWorldH0DependencyClosureIncludesGameCoreAndContainsNoDomainSemanticLeakage()
        {
            var root = FindRepositoryRoot();
            var worldProject = Path.Combine(root, "src", "Arkus.Game.World", "Arkus.Game.World.csproj");
            var worldProjectText = File.ReadAllText(worldProject);

            Assert.Contains("../Arkus.Game.Core/Arkus.Game.Core.csproj", worldProjectText, StringComparison.Ordinal);

            var closureFiles = new List<string>
            {
                Path.Combine(root, "src", "Arkus.DesignWorld", "Arkus.DesignWorld.csproj"),
                Path.Combine(root, "src", "Arkus.DesignWorld", "DesignWorldContracts.cs"),
                Path.Combine(root, "src", "Arkus.DesignWorld", "DesignWorldProjection.cs")
            };
            closureFiles.AddRange(ProjectSurfaceFiles(Path.Combine(root, "src", "Arkus.Game.World")));
            closureFiles.AddRange(ProjectSurfaceFiles(Path.Combine(root, "src", "Arkus.Game.Core")));

            Assert.Empty(FindLeakage(closureFiles.Select(path => new Surface(path, File.ReadAllText(path)))));

            var coreProject = File.ReadAllText(Path.Combine(root, "src", "Arkus.Game.Core", "Arkus.Game.Core.csproj"));
            Assert.DoesNotContain("ProjectReference", coreProject, StringComparison.Ordinal);
        }

        [Fact]
        public void PrefixFreeCityAndPaBehaviorTurnsLeakageAuditRed()
        {
            var injected = new[]
            {
                new Surface(
                    "src/Arkus.Game.World/WorldRule.cs",
                    "if (factId.StartsWith(\"loc.\", StringComparison.Ordinal) || factId.StartsWith(\"fam.\", StringComparison.Ordinal)) return true;"),
                new Surface(
                    "src/Arkus.Game.Core/CoreRule.cs",
                    "if (fact.FactType == \"finding\" || fact.FactType == \"disposition\" || fact.FactType == \"evidence\") return true;")
            };

            var violations = FindLeakage(injected);

            Assert.Contains(violations, item => item.Contains("domain-id-prefix:loc.", StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("domain-id-prefix:fam.", StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("domain-literal:finding", StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("domain-literal:disposition", StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("domain-literal:evidence", StringComparison.Ordinal));
            Assert.DoesNotContain(violations, item => item.Contains("City", StringComparison.Ordinal) || item.Contains("Pa", StringComparison.Ordinal));
        }

        [Fact]
        public void ExplicitCityOrPaIdentifiersStillTurnLeakageAuditRed()
        {
            var injected = new[]
            {
                new Surface("src/Arkus.Game.World/CityKernelRule.cs", "public sealed class CityKernelRule { }"),
                new Surface("src/Arkus.Game.Core/PaKernelDependency.cs", "public sealed class PaKernelDependency { }")
            };

            var violations = FindLeakage(injected);
            Assert.Contains(violations, item => item.Contains("identifier-or-path", StringComparison.Ordinal) && item.Contains("CityKernelRule", StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("identifier-or-path", StringComparison.Ordinal) && item.Contains("PaKernelDependency", StringComparison.Ordinal));
        }

        private static IEnumerable<string> ProjectSurfaceFiles(string directory) =>
            Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .Where(path => path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".csproj", StringComparison.Ordinal))
                .OrderBy(path => path, StringComparer.Ordinal);

        private static string[] FindLeakage(IEnumerable<Surface> surfaces)
        {
            var identifierPattern = new Regex(
                @"\bcity\b|\bpa\b|\bCity[A-Z][A-Za-z0-9_]*\b|\bPa[A-Z][A-Za-z0-9_]*\b",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            var domainIdPattern = new Regex(
                "\\\"(?<prefix>loc\\.|fam\\.)(?:[^\\\"]*)\\\"",
                RegexOptions.CultureInvariant);
            var violations = new List<string>();

            foreach (var surface in surfaces)
            {
                var fileName = Path.GetFileName(surface.Path);
                if (identifierPattern.IsMatch(surface.Content) || identifierPattern.IsMatch(fileName))
                {
                    violations.Add(surface.Path + "::identifier-or-path");
                }

                foreach (Match match in domainIdPattern.Matches(surface.Content))
                {
                    violations.Add(surface.Path + "::domain-id-prefix:" + match.Groups["prefix"].Value);
                }

                foreach (var literal in ExactDomainSemanticLiterals)
                {
                    if (surface.Content.Contains("\"" + literal + "\"", StringComparison.Ordinal))
                    {
                        violations.Add(surface.Path + "::domain-literal:" + literal);
                    }
                }
            }

            return violations.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Juego2.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new InvalidOperationException("Could not locate Juego2.sln from the test output directory.");
        }

        private sealed record Surface(string Path, string Content);
    }
}
