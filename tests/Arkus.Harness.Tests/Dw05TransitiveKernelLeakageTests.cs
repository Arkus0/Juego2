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

            Assert.NotEmpty(vocabulary.City.FactTypes);
            Assert.NotEmpty(vocabulary.City.Fields);
            Assert.NotEmpty(vocabulary.City.Relations);
            Assert.NotEmpty(vocabulary.City.Identities);
            Assert.NotEmpty(vocabulary.City.IdPrefixes);
            Assert.NotEmpty(vocabulary.City.BehaviorValues);

            Assert.Equal(
                PaProjectionManifest.AdoptedEntityTypes.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                vocabulary.Pa.FactTypes.OrderBy(value => value, StringComparer.Ordinal).ToArray());
            Assert.Equal(
                PaProjectionManifest.AdoptedFields.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                vocabulary.Pa.Fields.OrderBy(value => value, StringComparer.Ordinal).ToArray());
            Assert.Equal(
                PaProjectionManifest.AdoptedRelations.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                vocabulary.Pa.Relations.OrderBy(value => value, StringComparer.Ordinal).ToArray());
            Assert.NotEmpty(vocabulary.Pa.Identities);
            Assert.NotEmpty(vocabulary.Pa.IdPrefixes);
            Assert.NotEmpty(vocabulary.Pa.BehaviorValues);
        }

        [Fact]
        public void PrefixFreeCityFieldAndPaRelationOutsideEarlierReviewExamplesTurnAuditRed()
        {
            var vocabulary = BuildAdoptedDomainVocabulary(FindRepositoryRoot());
            const string cityProbe = "importance";
            const string paProbe = "declared-in";

            Assert.Contains(cityProbe, vocabulary.City.Fields);
            Assert.Contains(paProbe, vocabulary.Pa.Relations);
            Assert.DoesNotContain("city", cityProbe, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("pa", paProbe, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("loc.", cityProbe, StringComparison.Ordinal);
            Assert.DoesNotContain("fam.", paProbe, StringComparison.Ordinal);
            Assert.DoesNotContain("finding", cityProbe + paProbe, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("disposition", cityProbe + paProbe, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("evidence", cityProbe + paProbe, StringComparison.OrdinalIgnoreCase);

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

            Assert.Contains(violations, item => item.Contains("field:" + cityProbe, StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("relation:" + paProbe, StringComparison.Ordinal));
            Assert.DoesNotContain(violations, item => item.Contains("identifier-or-path", StringComparison.Ordinal));
        }

        [Fact]
        public void StructurallyDerivedIdPrefixesTurnLeakageAuditRedWithoutHandwrittenPrefixInventory()
        {
            var vocabulary = BuildAdoptedDomainVocabulary(FindRepositoryRoot());
            var cityPrefix = vocabulary.City.IdPrefixes
                .Where(value => !value.StartsWith("loc.", StringComparison.Ordinal))
                .OrderBy(value => value.Length)
                .ThenBy(value => value, StringComparer.Ordinal)
                .First();
            var paPrefix = vocabulary.Pa.IdPrefixes
                .OrderBy(value => value.Length)
                .ThenBy(value => value, StringComparer.Ordinal)
                .First();

            var injected = new[]
            {
                new Surface(
                    "src/Arkus.Game.World/WorldRule.cs",
                    "if (factId.StartsWith(\"" + cityPrefix + "\", StringComparison.Ordinal)) return true;"),
                new Surface(
                    "src/Arkus.Game.Core/CoreRule.cs",
                    "if (factId.StartsWith(\"" + paPrefix + "\", StringComparison.Ordinal)) return true;")
            };

            var violations = FindLeakage(injected, vocabulary);

            Assert.Contains(violations, item => item.Contains("identity-prefix:" + cityPrefix, StringComparison.Ordinal));
            Assert.Contains(violations, item => item.Contains("identity-prefix:" + paPrefix, StringComparison.Ordinal));
        }

        [Fact]
        public void RuntimeSelectedAdoptedFieldAbsentFromTestExamplesStillTurnsAuditRed()
        {
            var root = FindRepositoryRoot();
            var vocabulary = BuildAdoptedDomainVocabulary(root);
            var testSource = File.ReadAllText(Path.Combine(
                root, "tests", "Arkus.Harness.Tests", "Dw05TransitiveKernelLeakageTests.cs"));
            var unseenField = vocabulary.City.Fields.Concat(vocabulary.Pa.Fields)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .First(value => !testSource.Contains("\"" + value + "\"", StringComparison.Ordinal));

            var injected = new[]
            {
                new Surface(
                    "src/Arkus.Game.World/RuntimeSelectedRule.cs",
                    "if (fact.Fields.ContainsKey(\"" + unseenField + "\")) return true;")
            };

            var violations = FindLeakage(injected, vocabulary);

            Assert.Contains(violations, item => item.Contains("field:" + unseenField, StringComparison.Ordinal));
        }

        [Fact]
        public void AdoptedEnglishLiteralOutsideSemanticOrBehavioralContextDoesNotFalsePositive()
        {
            var vocabulary = BuildAdoptedDomainVocabulary(FindRepositoryRoot());
            var englishLikeField = vocabulary.City.Fields.Concat(vocabulary.Pa.Fields)
                .Distinct(StringComparer.Ordinal)
                .Where(value => value.All(char.IsLetter))
                .OrderBy(value => value, StringComparer.Ordinal)
                .First();
            var injected = new[]
            {
                new Surface(
                    "src/Arkus.Game.Core/DiagnosticText.cs",
                    "var diagnosticLabel = \"" + englishLikeField + "\";\nConsole.WriteLine(diagnosticLabel);")
            };

            Assert.Empty(FindLeakage(injected, vocabulary));
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

            return new DomainVocabulary(Inventory(cityFacts), Inventory(paFacts));
        }

        private static SemanticInventory Inventory(IEnumerable<DesignFact> facts)
        {
            var materialized = facts.ToArray();
            var factTypes = new SortedSet<string>(materialized.Select(fact => fact.FactType), StringComparer.Ordinal);
            var fields = new SortedSet<string>(materialized.SelectMany(fact => fact.Fields.Keys), StringComparer.Ordinal);
            var relations = new SortedSet<string>(
                materialized.SelectMany(fact => fact.Relations.Select(relation => relation.RelationType)),
                StringComparer.Ordinal);
            var identities = new SortedSet<string>(
                materialized.Select(fact => fact.FactId)
                    .Concat(materialized.SelectMany(fact => fact.Relations.Select(relation => relation.TargetFactId))),
                StringComparer.Ordinal);
            var idPrefixes = DeriveRepeatedIdPrefixes(identities);
            var behaviorValues = DeriveBoundedAtomicBehaviorValues(materialized);
            return new SemanticInventory(
                factTypes.ToArray(),
                fields.ToArray(),
                relations.ToArray(),
                identities.ToArray(),
                idPrefixes.ToArray(),
                behaviorValues.ToArray());
        }

        private static SortedSet<string> DeriveRepeatedIdPrefixes(IEnumerable<string> identities)
        {
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var identity in identities.Distinct(StringComparer.Ordinal))
            {
                for (var index = identity.IndexOf('.'); index >= 0; index = identity.IndexOf('.', index + 1))
                {
                    var prefix = identity.Substring(0, index + 1);
                    counts[prefix] = counts.TryGetValue(prefix, out var count) ? count + 1 : 1;
                }
            }

            return new SortedSet<string>(
                counts.Where(pair => pair.Value >= 2).Select(pair => pair.Key),
                StringComparer.Ordinal);
        }

        private static SortedSet<string> DeriveBoundedAtomicBehaviorValues(IReadOnlyCollection<DesignFact> facts)
        {
            var result = new SortedSet<string>(StringComparer.Ordinal);
            var valuesByField = facts
                .SelectMany(fact => fact.Fields.Select(pair => new FieldValue(pair.Key, pair.Value)))
                .Where(item => item.Value.Kind == DesignValueKind.String)
                .GroupBy(item => item.Field, StringComparer.Ordinal);

            foreach (var field in valuesByField)
            {
                var values = field.Select(item => item.Value.CanonicalValue)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(value => value, StringComparer.Ordinal)
                    .ToArray();
                if (values.Length > 16)
                {
                    continue;
                }

                foreach (var value in values)
                {
                    if (value.Length > 0 && value.Length <= 64 &&
                        Regex.IsMatch(value, @"^[A-Za-z0-9][A-Za-z0-9._-]*$", RegexOptions.CultureInvariant))
                    {
                        result.Add(value);
                    }
                }
            }

            return result;
        }

        private static string[] FindLeakage(IEnumerable<Surface> surfaces, DomainVocabulary vocabulary)
        {
            var identifierPattern = new Regex(
                @"\bcity\b|\bpa\b|\bCity[A-Z][A-Za-z0-9_]*\b|\bPa[A-Z][A-Za-z0-9_]*\b",
                RegexOptions.CultureInvariant);
            var violations = new List<string>();
            var semanticTokens = vocabulary.Tokens().ToArray();

            foreach (var surface in surfaces)
            {
                var fileName = Path.GetFileName(surface.Path);
                if (identifierPattern.IsMatch(surface.Content) || identifierPattern.IsMatch(fileName))
                {
                    violations.Add(surface.Path + "::identifier-or-path");
                }

                foreach (var token in semanticTokens)
                {
                    if (HasBehavioralUse(surface.Content, token))
                    {
                        violations.Add(surface.Path + "::domain-semantic:" + KindName(token.Kind) + ":" + token.Value);
                    }
                }
            }

            return violations.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        }

        private static bool HasBehavioralUse(string content, SemanticToken token)
        {
            var literalPattern = new Regex(
                "\\\"" + Regex.Escape(token.Value) + "\\\"",
                RegexOptions.CultureInvariant);
            foreach (Match literal in literalPattern.Matches(content))
            {
                var line = LineContext(content, literal.Index);
                if (IsSemanticContext(line, token.Kind) || IsBehavioralContext(line))
                {
                    return true;
                }
            }

            var aliasPattern = new Regex(
                "(?:const\\s+)?(?:string|var)\\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\\s*=\\s*\\\"" +
                Regex.Escape(token.Value) + "\\\"",
                RegexOptions.CultureInvariant);
            foreach (Match alias in aliasPattern.Matches(content))
            {
                var name = alias.Groups["name"].Value;
                var usePattern = new Regex(@"\b" + Regex.Escape(name) + @"\b", RegexOptions.CultureInvariant);
                foreach (Match use in usePattern.Matches(content))
                {
                    if (use.Index == alias.Groups["name"].Index)
                    {
                        continue;
                    }

                    var line = LineContext(content, use.Index);
                    if (IsSemanticContext(line, token.Kind) || IsBehavioralContext(line))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string LineContext(string content, int index)
        {
            var start = content.LastIndexOf('\n', Math.Max(0, index - 1));
            start = start < 0 ? 0 : start + 1;
            var end = content.IndexOf('\n', index);
            if (end < 0)
            {
                end = content.Length;
            }
            return content.Substring(start, end - start);
        }

        private static bool IsSemanticContext(string line, SemanticKind kind)
        {
            switch (kind)
            {
                case SemanticKind.FactType:
                    return line.Contains("FactType", StringComparison.Ordinal);
                case SemanticKind.Field:
                    return line.Contains("Fields", StringComparison.Ordinal);
                case SemanticKind.Relation:
                    return line.Contains("RelationType", StringComparison.Ordinal) ||
                           line.Contains("Relations", StringComparison.Ordinal);
                case SemanticKind.Identity:
                case SemanticKind.IdentityPrefix:
                    return line.IndexOf("factId", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           line.Contains("TargetFactId", StringComparison.Ordinal);
                case SemanticKind.BehaviorValue:
                    return line.Contains("CanonicalValue", StringComparison.Ordinal);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        private static bool IsBehavioralContext(string line) =>
            line.Contains("if (", StringComparison.Ordinal) ||
            line.Contains("if(", StringComparison.Ordinal) ||
            line.Contains("switch (", StringComparison.Ordinal) ||
            line.Contains("switch(", StringComparison.Ordinal) ||
            line.Contains("case ", StringComparison.Ordinal) ||
            line.Contains("==", StringComparison.Ordinal) ||
            line.Contains("!=", StringComparison.Ordinal) ||
            line.Contains(".Equals(", StringComparison.Ordinal) ||
            line.Contains(".StartsWith(", StringComparison.Ordinal) ||
            line.Contains(".Contains(", StringComparison.Ordinal) ||
            line.Contains(".ContainsKey(", StringComparison.Ordinal) ||
            line.Contains(".TryGetValue(", StringComparison.Ordinal);

        private static string KindName(SemanticKind kind)
        {
            switch (kind)
            {
                case SemanticKind.FactType: return "fact-type";
                case SemanticKind.Field: return "field";
                case SemanticKind.Relation: return "relation";
                case SemanticKind.Identity: return "identity";
                case SemanticKind.IdentityPrefix: return "identity-prefix";
                case SemanticKind.BehaviorValue: return "behavior-value";
                default: throw new ArgumentOutOfRangeException(nameof(kind));
            }
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
        private sealed record FieldValue(string Field, DesignValue Value);
        private sealed record SemanticToken(string Value, SemanticKind Kind);
        private sealed record SemanticInventory(
            IReadOnlyList<string> FactTypes,
            IReadOnlyList<string> Fields,
            IReadOnlyList<string> Relations,
            IReadOnlyList<string> Identities,
            IReadOnlyList<string> IdPrefixes,
            IReadOnlyList<string> BehaviorValues);

        private sealed class DomainVocabulary
        {
            public DomainVocabulary(SemanticInventory city, SemanticInventory pa)
            {
                City = city;
                Pa = pa;
            }

            public SemanticInventory City { get; }
            public SemanticInventory Pa { get; }

            public IEnumerable<SemanticToken> Tokens()
            {
                foreach (var inventory in new[] { City, Pa })
                {
                    foreach (var value in inventory.FactTypes) yield return new SemanticToken(value, SemanticKind.FactType);
                    foreach (var value in inventory.Fields) yield return new SemanticToken(value, SemanticKind.Field);
                    foreach (var value in inventory.Relations) yield return new SemanticToken(value, SemanticKind.Relation);
                    foreach (var value in inventory.Identities) yield return new SemanticToken(value, SemanticKind.Identity);
                    foreach (var value in inventory.IdPrefixes) yield return new SemanticToken(value, SemanticKind.IdentityPrefix);
                    foreach (var value in inventory.BehaviorValues) yield return new SemanticToken(value, SemanticKind.BehaviorValue);
                }
            }
        }

        private enum SemanticKind
        {
            FactType,
            Field,
            Relation,
            Identity,
            IdentityPrefix,
            BehaviorValue
        }
    }
}
