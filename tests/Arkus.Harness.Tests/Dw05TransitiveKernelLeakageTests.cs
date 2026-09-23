using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw05TransitiveKernelLeakageTests
    {
        [Fact]
        public void DesignWorldH0DependencyClosureIncludesGameCoreAndContainsNoDomainSemanticLeakage()
        {
            var root = FindRepositoryRoot();
            var vocabulary = BuildAdoptedDomainVocabulary(root);
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

            var violations = FindLeakage(
                closureFiles.Select(path => new Surface(path, File.ReadAllText(path))),
                vocabulary);
            Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations));

            var coreProject = File.ReadAllText(Path.Combine(root, "src", "Arkus.Game.Core", "Arkus.Game.Core.csproj"));
            Assert.DoesNotContain("ProjectReference", coreProject, StringComparison.Ordinal);
        }

        [Fact]
        public void AdoptedVocabularyInventoryIsReconciledAgainstActualCityAndPaSemanticSurfaces()
        {
            var vocabulary = BuildAdoptedDomainVocabulary(FindRepositoryRoot());

            Assert.Contains("required-role-private", vocabulary.CitySemanticTokens);
            Assert.Contains("allocated", vocabulary.CitySemanticTokens);
            Assert.Contains("importance", vocabulary.CitySemanticTokens);
            Assert.Contains("loc.", vocabulary.CityIdPrefixes);

            Assert.Empty(PaProjectionManifest.AdoptedEntityTypes.Except(vocabulary.PaSemanticTokens, StringComparer.Ordinal));
            Assert.Empty(PaProjectionManifest.AdoptedFields.Except(vocabulary.PaSemanticTokens, StringComparer.Ordinal));
            Assert.Empty(PaProjectionManifest.AdoptedRelations.Except(vocabulary.PaSemanticTokens, StringComparer.Ordinal));
            Assert.Contains("failure-family", vocabulary.PaSemanticTokens);
            Assert.Contains("contains-reject", vocabulary.PaSemanticTokens);
            Assert.Contains("declared-in", vocabulary.PaSemanticTokens);
        }

        [Fact]
        public void PrefixFreeCityAndPaBehaviorOutsideEarlierExamplesTurnsLeakageAuditRed()
        {
            var vocabulary = BuildAdoptedDomainVocabulary(FindRepositoryRoot());
            const string cityProbe = "importance";
            const string paProbe = "declared-in";

            Assert.Contains(cityProbe, vocabulary.CitySemanticTokens);
            Assert.Contains(paProbe, vocabulary.PaSemanticTokens);
            Assert.DoesNotContain("city", cityProbe, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("pa", paProbe, StringComparison.OrdinalIgnoreCase);

            var injected = new[]
            {
                new Surface(
                    "src/Arkus.Game.World/WorldRule.cs",
                    "if (fact.Fields.ContainsKey(\"" + cityProbe + "\")) return true;"),
                new Surface(
                    "src/Arkus.Game.Core/CoreRule.cs",
                    "if (fact.Relations.Any(r => r.RelationType == \"" + paProbe + "\")) return true;")
            };

            var violations = FindLeakage(injected, vocabulary);

            Assert.Contains(violations, item => item.Contains("domain-token:" + cityProbe, StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("domain-token:" + paProbe, StringComparison.Ordinal));
            Assert.DoesNotContain(violations, item => item.Contains("identifier-or-path", StringComparison.Ordinal));
        }

        [Fact]
        public void StructurallyDerivedIdPrefixesTurnLeakageAuditRed()
        {
            var vocabulary = BuildAdoptedDomainVocabulary(FindRepositoryRoot());
            Assert.Contains("loc.", vocabulary.CityIdPrefixes);

            var paPrefix = vocabulary.PaIdPrefixes
                .OrderBy(value => value, StringComparer.Ordinal)
                .FirstOrDefault(value => value.Length >= 3);
            Assert.False(string.IsNullOrEmpty(paPrefix), "PA projection must expose at least one structurally derivable id prefix.");

            var injected = new[]
            {
                new Surface(
                    "src/Arkus.Game.World/WorldRule.cs",
                    "if (factId.StartsWith(\"loc.\", StringComparison.Ordinal)) return true;"),
                new Surface(
                    "src/Arkus.Game.Core/CoreRule.cs",
                    "if (factId.StartsWith(\"" + paPrefix + "\", StringComparison.Ordinal)) return true;")
            };

            var violations = FindLeakage(injected, vocabulary);

            Assert.Contains(violations, item => item.Contains("domain-id-prefix:loc.", StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("domain-id-prefix:" + paPrefix, StringComparison.Ordinal));
        }

        [Fact]
        public void ExplicitCityOrPaIdentifiersStillTurnLeakageAuditRed()
        {
            var vocabulary = BuildAdoptedDomainVocabulary(FindRepositoryRoot());
            var injected = new[]
            {
                new Surface("src/Arkus.Game.World/CityKernelRule.cs", "public sealed class CityKernelRule { }"),
                new Surface("src/Arkus.Game.Core/PaKernelDependency.cs", "public sealed class PaKernelDependency { }")
            };

            var violations = FindLeakage(injected, vocabulary);
            Assert.Contains(violations, item => item.Contains("identifier-or-path", StringComparison.Ordinal) && item.Contains("CityKernelRule", StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("identifier-or-path", StringComparison.Ordinal) && item.Contains("PaKernelDependency", StringComparison.Ordinal));
        }

        private static IEnumerable<string> ProjectSurfaceFiles(string directory) =>
            Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .Where(path => path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".csproj", StringComparison.Ordinal))
                .OrderBy(path => path, StringComparer.Ordinal);

        private static DomainVocabulary BuildAdoptedDomainVocabulary(string root)
        {
            var city = new CityDesignWorldProvider().BuildAndValidate(
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.ProgrammeSourcePath)),
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.BindingSourcePath)),
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.InteriorsSourcePath)));
            var cityFacts = city.ProgrammeProjection.Facts
                .Concat(city.BindingProjection.Facts)
                .Concat(city.InteriorProjection.Facts)
                .ToArray();

            var paSources = new PaAcceptedCorpusSources(
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa01Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa02Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa03Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa04Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa05Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa05FixturesPath)));
            var paFacts = new PaDesignWorldProvider().BuildAndValidate(paSources).Projection.Facts;

            var cityInventory = Inventory(cityFacts);
            var paInventory = Inventory(paFacts);
            return new DomainVocabulary(
                cityInventory.SemanticTokens,
                cityInventory.IdPrefixes,
                paInventory.SemanticTokens,
                paInventory.IdPrefixes);
        }

        private static SemanticInventory Inventory(IEnumerable<DesignFact> facts)
        {
            var tokens = new SortedSet<string>(StringComparer.Ordinal);
            var idPrefixes = new SortedSet<string>(StringComparer.Ordinal);

            foreach (var fact in facts)
            {
                tokens.Add(fact.FactId);
                tokens.Add(fact.FactType);
                tokens.Add(fact.Provenance.AuthorityId);
                tokens.Add(fact.Provenance.SourcePath);
                AddIdPrefixes(fact.FactId, idPrefixes);

                foreach (var field in fact.Fields)
                {
                    tokens.Add(field.Key);
                    if (field.Value.Kind == DesignValueKind.String && IsAtomicSemanticValue(field.Value.CanonicalValue))
                    {
                        tokens.Add(field.Value.CanonicalValue);
                    }
                }

                foreach (var relation in fact.Relations)
                {
                    tokens.Add(relation.RelationType);
                    tokens.Add(relation.TargetFactId);
                    AddIdPrefixes(relation.TargetFactId, idPrefixes);
                }
            }

            return new SemanticInventory(tokens.ToArray(), idPrefixes.ToArray());
        }

        private static bool IsAtomicSemanticValue(string value) =>
            value.Length > 0 &&
            value.Length <= 80 &&
            Regex.IsMatch(value, @"^[A-Za-z0-9][A-Za-z0-9._:/-]*$", RegexOptions.CultureInvariant);

        private static void AddIdPrefixes(string id, ISet<string> prefixes)
        {
            for (var index = 0; index < id.Length; index++)
            {
                if (id[index] == '.' || id[index] == ':' || id[index] == '/')
                {
                    prefixes.Add(id.Substring(0, index + 1));
                }
            }
        }

        private static string[] FindLeakage(IEnumerable<Surface> surfaces, DomainVocabulary vocabulary)
        {
            var identifierPattern = new Regex(
                @"\bcity\b|\bpa\b|\bCity[A-Z][A-Za-z0-9_]*\b|\bPa[A-Z][A-Za-z0-9_]*\b",
                RegexOptions.CultureInvariant);
            var violations = new List<string>();
            var semanticTokens = vocabulary.CitySemanticTokens
                .Concat(vocabulary.PaSemanticTokens)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            var idPrefixes = vocabulary.CityIdPrefixes
                .Concat(vocabulary.PaIdPrefixes)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();

            foreach (var surface in surfaces)
            {
                var fileName = Path.GetFileName(surface.Path);
                if (identifierPattern.IsMatch(surface.Content) || identifierPattern.IsMatch(fileName))
                {
                    violations.Add(surface.Path + "::identifier-or-path");
                }

                foreach (var prefix in idPrefixes)
                {
                    if (surface.Content.Contains("\"" + prefix + "\"", StringComparison.Ordinal))
                    {
                        violations.Add(surface.Path + "::domain-id-prefix:" + prefix);
                    }
                }

                foreach (var token in semanticTokens)
                {
                    if (surface.Content.Contains("\"" + token + "\"", StringComparison.Ordinal))
                    {
                        violations.Add(surface.Path + "::domain-token:" + token);
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
        private sealed record SemanticInventory(IReadOnlyList<string> SemanticTokens, IReadOnlyList<string> IdPrefixes);
        private sealed record DomainVocabulary(
            IReadOnlyList<string> CitySemanticTokens,
            IReadOnlyList<string> CityIdPrefixes,
            IReadOnlyList<string> PaSemanticTokens,
            IReadOnlyList<string> PaIdPrefixes);
    }
}
