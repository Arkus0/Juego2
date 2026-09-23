using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Arkus.DesignWorld;
using Arkus.Game.World;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw05SemanticAlphaRenamingTests
    {
        [Fact]
        public void AcceptedCityAndPaGraphsRemainBehaviorallyInvariantUnderSystematicSemanticAlphaRenaming()
        {
            var root = FindRepositoryRoot();
            var cases = BuildCityCases(root)
                .Concat(new[] { new DomainCase("pa", BuildPaFacts(root)) })
                .ToArray();

            foreach (var domainCase in cases)
            {
                var renamedFacts = AlphaRename(domainCase.Facts);
                var originalInventory = Inventory(domainCase.Facts);
                var renamedInventory = Inventory(renamedFacts);

                AssertSemanticAxesRenamedWithoutSharedAtoms(domainCase.Name, originalInventory, renamedInventory);
                Assert.Equal(TopologySignature(domainCase.Facts), TopologySignature(renamedFacts));

                var original = BuildScenario(domainCase.Name, domainCase.Facts);
                var renamed = BuildScenario(domainCase.Name, renamedFacts);
                var differences = CompareObservations(
                    ObserveSharedBoundary(original),
                    ObserveSharedBoundary(renamed));

                Assert.True(
                    differences.Length == 0,
                    domainCase.Name + " alpha-renaming changed generic DW/H0 behavior:" +
                    Environment.NewLine + string.Join(Environment.NewLine, differences));
            }
        }

        [Fact]
        public void RuntimeDerivedFieldFamilyDependencyThroughIndirectHelperBreaksDifferentialOracle()
        {
            var cityCases = BuildCityCases(FindRepositoryRoot());
            var selected = cityCases
                .Select(domainCase => new FamilyCase(domainCase, TryDeriveRepeatedFieldFamily(
                    domainCase.Facts.SelectMany(fact => fact.Fields.Keys))))
                .First(item => item.Family != null);
            var family = selected.Family!;
            var cityFacts = selected.Domain.Facts;
            var renamedFacts = AlphaRename(cityFacts);

            Assert.True(
                cityFacts.SelectMany(fact => fact.Fields.Keys)
                    .Distinct(StringComparer.Ordinal)
                    .Count(field => field.StartsWith(family, StringComparison.Ordinal)) >= 3,
                "The causal control must use a real repeated CITY field family derived from an accepted projection.");
            Assert.DoesNotContain(
                renamedFacts.SelectMany(fact => fact.Fields.Keys),
                field => field.StartsWith(family, StringComparison.Ordinal));

            var original = BuildScenario(selected.Domain.Name + "-family-control", cityFacts);
            var renamed = BuildScenario(selected.Domain.Name + "-family-control", renamedFacts);
            var dependency = new InjectedFamilyDependency(family);

            var differences = CompareObservations(
                ObserveSharedBoundary(original, projection => dependency.RequiredSemanticFamilyIsPresent(projection)),
                ObserveSharedBoundary(renamed, projection => dependency.RequiredSemanticFamilyIsPresent(projection)));

            Assert.Contains(differences, difference =>
                difference.StartsWith("behavior.injected-domain-gate:", StringComparison.Ordinal));
        }

        [Fact]
        public void DomainVocabularyUsedOnlyAsOpaquePayloadDoesNotCreateBehavioralFalsePositive()
        {
            var root = FindRepositoryRoot();
            var adoptedDomainText = BuildCityCases(root)
                .SelectMany(domainCase => domainCase.Facts)
                .SelectMany(fact => fact.Fields.Keys)
                .OrderBy(value => value, StringComparer.Ordinal)
                .First();
            var renamedPaFacts = AlphaRename(BuildPaFacts(root));
            var opaquePayloadFacts = ReplaceFirstStringValue(renamedPaFacts, adoptedDomainText);

            var renamed = BuildScenario("pa-opaque-control", renamedPaFacts);
            var opaque = BuildScenario("pa-opaque-control", opaquePayloadFacts);
            var differences = CompareObservations(
                ObserveSharedBoundary(renamed),
                ObserveSharedBoundary(opaque));

            Assert.True(
                differences.Length == 0,
                "Opaque data containing adopted domain text must not be treated as semantic dependence:" +
                Environment.NewLine + string.Join(Environment.NewLine, differences));
        }

        private static Scenario BuildScenario(string name, IReadOnlyList<DesignFact> facts)
        {
            var version = new DesignProjectionVersion(1, "dw05-alpha-" + name + "-v1");
            var universe = new StaticDesignAuthorityUniverse(facts.Select(fact => fact.FactId));
            var reader = new StaticFactReader(facts);
            var projection = new DesignWorldProjector().Build(universe, reader, version);
            return new Scenario(projection, universe, reader, version);
        }

        private static IReadOnlyDictionary<string, string> ObserveSharedBoundary(
            Scenario scenario,
            Func<DesignWorldProjection, bool>? injectedDomainGate = null)
        {
            var result = new SortedDictionary<string, string>(StringComparer.Ordinal);
            var dwReport = new DesignWorldProjectionValidator().Validate(
                scenario.Projection,
                scenario.Universe,
                scenario.Reader,
                scenario.Version);
            result["dw.validation-issues"] = JoinOrdered(dwReport.Issues.Select(issue => issue.MachineCode));

            var h0Issues = WorldStateValidator.Validate(scenario.Projection.WorldState);
            result["h0.validation-issues"] = JoinOrdered(h0Issues.Select(issue => issue.Invariant.MachineCode));

            var canonical = CanonicalWorldStateCodec.Serialize(scenario.Projection.WorldState);
            var roundTripped = CanonicalWorldStateCodec.Deserialize(canonical);
            var roundTripIssues = WorldStateValidator.Validate(roundTripped);
            result["h0.roundtrip-validation-issues"] = JoinOrdered(roundTripIssues.Select(issue => issue.Invariant.MachineCode));
            result["h0.roundtrip-canonical-stable"] =
                canonical.SequenceEqual(CanonicalWorldStateCodec.Serialize(roundTripped)) ? "true" : "false";

            result["shape.fact-count"] = scenario.Projection.Facts.Count.ToString(CultureInfo.InvariantCulture);
            result["shape.fact-topology"] = TopologySignature(scenario.Projection.Facts);
            result["shape.world-object-count"] = scenario.Projection.WorldState.Objects.Count.ToString(CultureInfo.InvariantCulture);
            result["shape.world-reference-count"] = scenario.Projection.WorldState.Objects
                .Sum(item => item.References.Count).ToString(CultureInfo.InvariantCulture);
            result["shape.extension-count"] = scenario.Projection.WorldState.Extensions.Count.ToString(CultureInfo.InvariantCulture);
            result["shape.extension-dependency-count"] = scenario.Projection.WorldState.Extensions
                .Sum(item => item.Dependencies.Count).ToString(CultureInfo.InvariantCulture);
            result["shape.world-type-count"] = scenario.Projection.WorldState.Objects
                .Select(item => item.TypeId.Value)
                .Distinct(StringComparer.Ordinal)
                .Count().ToString(CultureInfo.InvariantCulture);
            result["shape.extension-owner-schema-count"] = scenario.Projection.WorldState.Extensions
                .Select(item => item.Owner + ":" + item.SchemaVersion.ToString(CultureInfo.InvariantCulture))
                .Distinct(StringComparer.Ordinal)
                .Count().ToString(CultureInfo.InvariantCulture);

            if (injectedDomainGate != null)
            {
                result["behavior.injected-domain-gate"] = injectedDomainGate(scenario.Projection) ? "true" : "false";
            }

            return result;
        }

        private static string[] CompareObservations(
            IReadOnlyDictionary<string, string> left,
            IReadOnlyDictionary<string, string> right)
        {
            var keys = new SortedSet<string>(left.Keys, StringComparer.Ordinal);
            keys.UnionWith(right.Keys);
            var differences = new List<string>();
            foreach (var key in keys)
            {
                var hasLeft = left.TryGetValue(key, out var leftValue);
                var hasRight = right.TryGetValue(key, out var rightValue);
                if (!hasLeft || !hasRight || !StringComparer.Ordinal.Equals(leftValue, rightValue))
                {
                    differences.Add(
                        key + ":" + (hasLeft ? leftValue : "<missing>") + " -> " +
                        (hasRight ? rightValue : "<missing>"));
                }
            }

            return differences.ToArray();
        }

        private static IReadOnlyList<DesignFact> AlphaRename(IReadOnlyList<DesignFact> facts)
        {
            var inventory = Inventory(facts);
            var idMap = MakeMap(inventory.Identities, "dw05-alpha.fact.");
            var typeMap = MakeMap(inventory.FactTypes, "dw05-alpha-type-");
            var fieldMap = MakeMap(inventory.Fields, "dw05-alpha-field-");
            var relationMap = MakeMap(inventory.Relations, "dw05-alpha-relation-");
            var valueMap = MakeMap(inventory.CategoricalStringValues, "dw05-alpha-value-");
            var renamed = new List<DesignFact>(facts.Count);

            foreach (var fact in facts)
            {
                var fields = new Dictionary<string, DesignValue>(StringComparer.Ordinal);
                foreach (var pair in fact.Fields)
                {
                    var value = pair.Value;
                    if (value.Kind == DesignValueKind.String &&
                        valueMap.TryGetValue(value.CanonicalValue, out var renamedValue))
                    {
                        value = DesignValue.String(renamedValue);
                    }

                    fields.Add(fieldMap[pair.Key], value);
                }

                var relations = fact.Relations.Select(relation =>
                    new DesignRelation(relationMap[relation.RelationType], idMap[relation.TargetFactId]));
                renamed.Add(new DesignFact(
                    idMap[fact.FactId],
                    typeMap[fact.FactType],
                    fields,
                    relations,
                    fact.Provenance));
            }

            return renamed.AsReadOnly();
        }

        private static IReadOnlyList<DesignFact> ReplaceFirstStringValue(
            IReadOnlyList<DesignFact> facts,
            string opaqueText)
        {
            var replaced = false;
            var result = new List<DesignFact>(facts.Count);
            foreach (var fact in facts)
            {
                var fields = new Dictionary<string, DesignValue>(StringComparer.Ordinal);
                foreach (var pair in fact.Fields)
                {
                    var value = pair.Value;
                    if (!replaced && value.Kind == DesignValueKind.String)
                    {
                        value = DesignValue.String(opaqueText);
                        replaced = true;
                    }

                    fields.Add(pair.Key, value);
                }

                result.Add(new DesignFact(
                    fact.FactId,
                    fact.FactType,
                    fields,
                    fact.Relations,
                    fact.Provenance));
            }

            if (!replaced)
            {
                throw new InvalidOperationException("The PA alpha-renamed graph did not contain a string payload for the false-positive control.");
            }

            return result.AsReadOnly();
        }

        private static SemanticInventory Inventory(IEnumerable<DesignFact> facts)
        {
            var materialized = facts.ToArray();
            var factTypes = materialized.Select(fact => fact.FactType)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            var fields = materialized.SelectMany(fact => fact.Fields.Keys)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            var relations = materialized.SelectMany(fact => fact.Relations.Select(relation => relation.RelationType))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            var identities = materialized.Select(fact => fact.FactId)
                .Concat(materialized.SelectMany(fact => fact.Relations.Select(relation => relation.TargetFactId)))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            var categoricalValues = DeriveBoundedCategoricalStringValues(materialized).ToArray();
            return new SemanticInventory(factTypes, fields, relations, identities, categoricalValues);
        }

        private static SortedSet<string> DeriveBoundedCategoricalStringValues(IReadOnlyCollection<DesignFact> facts)
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

        private static Dictionary<string, string> MakeMap(IEnumerable<string> values, string prefix)
        {
            var map = new Dictionary<string, string>(StringComparer.Ordinal);
            var index = 0;
            foreach (var value in values.Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal))
            {
                map.Add(value, prefix + index.ToString("D4", CultureInfo.InvariantCulture));
                index++;
            }

            return map;
        }

        private static void AssertSemanticAxesRenamedWithoutSharedAtoms(
            string domain,
            SemanticInventory original,
            SemanticInventory renamed)
        {
            AssertAxisRenamed(domain, "fact-types", original.FactTypes, renamed.FactTypes);
            AssertAxisRenamed(domain, "fields", original.Fields, renamed.Fields);
            AssertAxisRenamed(domain, "relations", original.Relations, renamed.Relations);
            AssertAxisRenamed(domain, "identities", original.Identities, renamed.Identities);
            AssertAxisRenamed(
                domain,
                "categorical-string-values",
                original.CategoricalStringValues,
                renamed.CategoricalStringValues);
        }

        private static void AssertAxisRenamed(
            string domain,
            string axis,
            IReadOnlyCollection<string> original,
            IReadOnlyCollection<string> renamed)
        {
            Assert.Equal(original.Count, renamed.Count);
            var overlap = original.Intersect(renamed, StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            Assert.True(
                overlap.Length == 0,
                domain + " alpha-renaming retained semantic atoms on " + axis + ": " + string.Join(",", overlap));
        }

        private static string TopologySignature(IReadOnlyList<DesignFact> facts)
        {
            var incoming = facts.ToDictionary(fact => fact.FactId, _ => 0, StringComparer.Ordinal);
            foreach (var relation in facts.SelectMany(fact => fact.Relations))
            {
                incoming[relation.TargetFactId]++;
            }

            var factShapes = facts.Select(fact =>
                "fields=" + FieldKindSignature(fact) +
                ";out=" + fact.Relations.Count.ToString(CultureInfo.InvariantCulture) +
                ";in=" + incoming[fact.FactId].ToString(CultureInfo.InvariantCulture))
                .OrderBy(value => value, StringComparer.Ordinal);
            var relationMultiplicity = facts.SelectMany(fact => fact.Relations)
                .GroupBy(relation => relation.RelationType, StringComparer.Ordinal)
                .Select(group => group.Count())
                .OrderBy(value => value)
                .Select(value => value.ToString(CultureInfo.InvariantCulture));

            return string.Join("|", factShapes) + "||relation-type-multiplicity=" + string.Join(",", relationMultiplicity);
        }

        private static string FieldKindSignature(DesignFact fact) =>
            string.Join(",", fact.Fields.Values
                .GroupBy(value => value.Kind)
                .OrderBy(group => group.Key)
                .Select(group => group.Key + ":" + group.Count().ToString(CultureInfo.InvariantCulture)));

        private static string? TryDeriveRepeatedFieldFamily(IEnumerable<string> fields)
        {
            var distinct = fields.Distinct(StringComparer.Ordinal).ToArray();
            var candidates = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var field in distinct)
            {
                for (var index = field.IndexOf('-'); index >= 0; index = field.IndexOf('-', index + 1))
                {
                    var prefix = field.Substring(0, index + 1);
                    if (prefix.Length < 4 || prefix.Length == field.Length)
                    {
                        continue;
                    }

                    candidates[prefix] = distinct.Count(value => value.StartsWith(prefix, StringComparison.Ordinal));
                }
            }

            return candidates
                .Where(pair => pair.Value >= 3)
                .OrderByDescending(pair => pair.Key.Length)
                .ThenByDescending(pair => pair.Value)
                .ThenBy(pair => pair.Key, StringComparer.Ordinal)
                .Select(pair => pair.Key)
                .FirstOrDefault();
        }

        private static IReadOnlyList<DomainCase> BuildCityCases(string root)
        {
            var city = new CityDesignWorldProvider().BuildAndValidate(
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.ProgrammeSourcePath)),
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.BindingSourcePath)),
                File.ReadAllText(Path.Combine(root, CityDesignWorldProvider.InteriorsSourcePath)));
            return new[]
            {
                new DomainCase("city-programme", city.ProgrammeProjection.Facts),
                new DomainCase("city-binding", city.BindingProjection.Facts),
                new DomainCase("city-interiors", city.InteriorProjection.Facts)
            };
        }

        private static IReadOnlyList<DesignFact> BuildPaFacts(string root)
        {
            var sources = new PaAcceptedCorpusSources(
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa01Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa02Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa03Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa04Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa05Path)),
                File.ReadAllText(Path.Combine(root, PaProjectionManifest.Pa05FixturesPath)));
            return new PaDesignWorldProvider().BuildAndValidate(sources).Projection.Facts;
        }

        private static string JoinOrdered(IEnumerable<string> values) =>
            string.Join(",", values.OrderBy(value => value, StringComparer.Ordinal));

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

        private sealed class InjectedFamilyDependency
        {
            private readonly SemanticFamilyProvider _familyProvider;

            public InjectedFamilyDependency(string family)
            {
                _familyProvider = new SemanticFamilyProvider(family);
            }

            public bool RequiredSemanticFamilyIsPresent(DesignWorldProjection projection) =>
                projection.Facts.Any(fact => fact.Fields.Keys.Any(key =>
                    key.StartsWith(_familyProvider.FieldPrefix, StringComparison.Ordinal)));
        }

        private sealed class SemanticFamilyProvider
        {
            public SemanticFamilyProvider(string fieldPrefix)
            {
                FieldPrefix = fieldPrefix;
            }

            public string FieldPrefix { get; }
        }

        private sealed class StaticFactReader : IDesignAuthorityReader
        {
            private readonly IReadOnlyDictionary<string, DesignFact> _facts;
            private readonly HashSet<string> _anchors;

            public StaticFactReader(IEnumerable<DesignFact> facts)
            {
                _facts = facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);
                _anchors = new HashSet<string>(
                    _facts.Values.Select(fact => AnchorKey(fact.Provenance)),
                    StringComparer.Ordinal);
            }

            public DesignAuthorityReadResult Read(string factId) =>
                _facts.TryGetValue(factId, out var fact)
                    ? DesignAuthorityReadResult.Found(fact)
                    : DesignAuthorityReadResult.Failure(
                        DesignAuthorityResolutionStatus.Missing,
                        "Fact absent from the bounded alpha-renaming fixture.");

            public DesignAuthorityResolutionStatus Resolve(DesignAuthorityAnchor anchor) =>
                anchor != null && _anchors.Contains(AnchorKey(anchor))
                    ? DesignAuthorityResolutionStatus.Found
                    : DesignAuthorityResolutionStatus.Missing;

            private static string AnchorKey(DesignAuthorityAnchor anchor) =>
                anchor.AuthorityId + "\n" + anchor.SourcePath + "\n" + anchor.Anchor + "\n" +
                anchor.SourceDigest + "\n" + anchor.AnchorDigest;
        }

        private sealed record DomainCase(string Name, IReadOnlyList<DesignFact> Facts);
        private sealed record FamilyCase(DomainCase Domain, string? Family);
        private sealed record Scenario(
            DesignWorldProjection Projection,
            IDesignAuthorityUniverse Universe,
            IDesignAuthorityReader Reader,
            DesignProjectionVersion Version);
        private sealed record FieldValue(string Field, DesignValue Value);
        private sealed record SemanticInventory(
            IReadOnlyList<string> FactTypes,
            IReadOnlyList<string> Fields,
            IReadOnlyList<string> Relations,
            IReadOnlyList<string> Identities,
            IReadOnlyList<string> CategoricalStringValues);
    }
}
