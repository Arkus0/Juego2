using System;
using System.Collections.Generic;
using System.Globalization;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk05AmbiguousIdentityRegressionTests
    {
        [Fact]
        public void DuplicateObjectIdentityDefersIdentityDependentDiagnosticsDeterministically()
        {
            var first = WorldValidationEngine.ValidateCandidate(DuplicateObjectCandidate(false));
            var reordered = WorldValidationEngine.ValidateCandidate(DuplicateObjectCandidate(true));

            Assert.False(first.Valid);
            Assert.False(reordered.Valid);
            Assert.Equal(first.Diagnostics.Count, reordered.Diagnostics.Count);
            Assert.Equal(CompleteSignatures(first.Diagnostics), CompleteSignatures(reordered.Diagnostics));
            Assert.Empty(ActionabilityIssues(first.Diagnostics));
            Assert.Empty(ActionabilityIssues(reordered.Diagnostics));

            Assert.Equal(1, first.Diagnostics.Count);
            var duplicate = first.Diagnostics[0];
            Assert.Equal(WorldInvariantCatalog.ObjectIdentityUnique.InvariantId, duplicate.InvariantId);
            Assert.Equal("world.object@index:1", duplicate.Resource);
            Assert.Equal("$/objects/1/id", duplicate.Path);
            Assert.Equal("0", duplicate.RemediationContext["firstIndex"]);
            Assert.Equal("1", duplicate.RemediationContext["duplicateIndex"]);
            Assert.Equal("node.a", duplicate.RemediationContext["duplicateId"]);

            Assert.False(ContainsInvariant(first.Diagnostics, WorldInvariantCatalog.ContainerResolves.InvariantId));
            Assert.False(ContainsInvariant(first.Diagnostics, WorldInvariantCatalog.ContainmentAcyclic.InvariantId));
        }

        [Fact]
        public void DuplicateExtensionIdentityDefersIdentityDependentDiagnosticsDeterministically()
        {
            var first = WorldValidationEngine.ValidateCandidate(DuplicateExtensionCandidate(false));
            var reordered = WorldValidationEngine.ValidateCandidate(DuplicateExtensionCandidate(true));

            Assert.False(first.Valid);
            Assert.False(reordered.Valid);
            Assert.Equal(first.Diagnostics.Count, reordered.Diagnostics.Count);
            Assert.Equal(CompleteSignatures(first.Diagnostics), CompleteSignatures(reordered.Diagnostics));
            Assert.Empty(ActionabilityIssues(first.Diagnostics));
            Assert.Empty(ActionabilityIssues(reordered.Diagnostics));

            Assert.Equal(1, first.Diagnostics.Count);
            var duplicate = first.Diagnostics[0];
            Assert.Equal(WorldInvariantCatalog.ExtensionIdentityUnique.InvariantId, duplicate.InvariantId);
            Assert.Equal("world.extension@index:1", duplicate.Resource);
            Assert.Equal("$/extensions/1", duplicate.Path);
            Assert.Equal("0", duplicate.RemediationContext["firstIndex"]);
            Assert.Equal("1", duplicate.RemediationContext["duplicateIndex"]);
            Assert.False(string.IsNullOrWhiteSpace((string)duplicate.RemediationContext["duplicateIdentity"]!));

            Assert.False(ContainsInvariant(
                first.Diagnostics,
                WorldInvariantCatalog.ExtensionDependencyTargetResolves.InvariantId));
        }

        [Fact]
        public void AmbiguityOracleRequiresAnIndexAddressableRepairLocation()
        {
            var merelyNonEmpty = new WorldValidationDiagnostic(
                WorldInvariantCatalog.ObjectIdentityUnique.MachineCode,
                WorldDiagnosticSeverity.Error,
                "world.object:node.a",
                "$/objects/node.a",
                WorldInvariantCatalog.ObjectIdentityUnique.InvariantId,
                "Duplicate identity with a non-empty but ambiguous location.",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["action"] = "rename-one-entry"
                });

            var issues = ActionabilityIssues(new[] { merelyNonEmpty });
            Assert.Contains("duplicate-context-missing-source-indices", issues);
            Assert.Contains("duplicate-location-not-index-addressable", issues);
        }

        private static WorldStateCandidate DuplicateObjectCandidate(bool reverseDuplicates)
        {
            var type = new WorldTypeId("fixture.item");
            var nodeA = new WorldObjectId("node.a");
            var nodeB = new WorldObjectId("node.b");
            var missing = new WorldObjectId("node.missing");

            var aToB = new WorldObject(nodeA, type, nodeB);
            var aToMissing = new WorldObject(nodeA, type, missing);
            var bToA = new WorldObject(nodeB, type, nodeA);

            var objects = reverseDuplicates
                ? new WorldObject?[] { aToMissing, aToB, bToA }
                : new WorldObject?[] { aToB, aToMissing, bToA };

            return new WorldStateCandidate(new WorldId("world.ambiguous-object"), 1, objects);
        }

        private static WorldStateCandidate DuplicateExtensionCandidate(bool reverseDuplicates)
        {
            var rootId = new WorldObjectId("node.root");
            var root = new WorldObject(rootId, new WorldTypeId("fixture.item"));
            var kind = new WorldReferenceKind("fixture.link");

            var first = new WorldExtensionData(
                "fixture.data",
                1,
                new byte[] { 0x01 },
                rootId,
                new[]
                {
                    new WorldReference(kind, new WorldObjectId("node.missing-one"))
                });
            var second = new WorldExtensionData(
                "fixture.data",
                1,
                new byte[] { 0x02 },
                rootId,
                new[]
                {
                    new WorldReference(kind, new WorldObjectId("node.missing-two"))
                });

            var extensions = reverseDuplicates
                ? new WorldExtensionData?[] { second, first }
                : new WorldExtensionData?[] { first, second };

            return new WorldStateCandidate(
                new WorldId("world.ambiguous-extension"),
                1,
                new WorldObject?[] { root },
                extensions);
        }

        private static IReadOnlyList<string> ActionabilityIssues(
            IEnumerable<WorldValidationDiagnostic> diagnostics)
        {
            var issues = new List<string>();
            foreach (var diagnostic in diagnostics)
            {
                if (string.IsNullOrWhiteSpace(diagnostic.Resource))
                    issues.Add("diagnostic-resource-ambiguous");
                if (string.IsNullOrWhiteSpace(diagnostic.Path) ||
                    !diagnostic.Path.StartsWith("$/", StringComparison.Ordinal))
                    issues.Add("diagnostic-path-ambiguous");
                if (diagnostic.RemediationContext.Count == 0)
                    issues.Add("diagnostic-context-empty");

                if (string.Equals(
                    diagnostic.InvariantId,
                    WorldInvariantCatalog.ObjectIdentityUnique.InvariantId,
                    StringComparison.Ordinal))
                {
                    CheckDuplicateLocation(diagnostic, "world.object@index:", "$/objects/", "/id", issues);
                    if (!HasNonEmptyContext(diagnostic, "duplicateId"))
                        issues.Add("duplicate-context-missing-identity");
                }
                else if (string.Equals(
                    diagnostic.InvariantId,
                    WorldInvariantCatalog.ExtensionIdentityUnique.InvariantId,
                    StringComparison.Ordinal))
                {
                    CheckDuplicateLocation(diagnostic, "world.extension@index:", "$/extensions/", string.Empty, issues);
                    if (!HasNonEmptyContext(diagnostic, "duplicateIdentity"))
                        issues.Add("duplicate-context-missing-identity");
                }
            }

            return issues.AsReadOnly();
        }

        private static void CheckDuplicateLocation(
            WorldValidationDiagnostic diagnostic,
            string resourcePrefix,
            string pathPrefix,
            string pathSuffix,
            IList<string> issues)
        {
            if (!TryGetContextString(diagnostic, "firstIndex", out var firstIndex) ||
                !TryGetContextString(diagnostic, "duplicateIndex", out var duplicateIndex) ||
                string.Equals(firstIndex, duplicateIndex, StringComparison.Ordinal))
            {
                issues.Add("duplicate-context-missing-source-indices");
                issues.Add("duplicate-location-not-index-addressable");
                return;
            }

            if (!string.Equals(
                    diagnostic.Resource,
                    resourcePrefix + duplicateIndex,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    diagnostic.Path,
                    pathPrefix + duplicateIndex + pathSuffix,
                    StringComparison.Ordinal))
            {
                issues.Add("duplicate-location-not-index-addressable");
            }
        }

        private static bool HasNonEmptyContext(WorldValidationDiagnostic diagnostic, string key)
        {
            return TryGetContextString(diagnostic, key, out var value) &&
                !string.IsNullOrWhiteSpace(value);
        }

        private static bool TryGetContextString(
            WorldValidationDiagnostic diagnostic,
            string key,
            out string value)
        {
            value = string.Empty;
            if (!diagnostic.RemediationContext.TryGetValue(key, out var raw) || raw == null) return false;
            value = raw as string ?? Convert.ToString(raw, CultureInfo.InvariantCulture) ?? string.Empty;
            return value.Length != 0;
        }

        private static bool ContainsInvariant(
            IReadOnlyList<WorldValidationDiagnostic> diagnostics,
            string invariantId)
        {
            for (var index = 0; index < diagnostics.Count; index++)
            {
                if (string.Equals(diagnostics[index].InvariantId, invariantId, StringComparison.Ordinal)) return true;
            }

            return false;
        }

        private static IReadOnlyList<string> CompleteSignatures(
            IReadOnlyList<WorldValidationDiagnostic> diagnostics)
        {
            var values = new List<string>();
            for (var index = 0; index < diagnostics.Count; index++)
            {
                var diagnostic = diagnostics[index];
                values.Add(
                    diagnostic.Severity + "|" +
                    diagnostic.InvariantId + "|" +
                    diagnostic.MachineCode + "|" +
                    diagnostic.Resource + "|" +
                    diagnostic.Path + "|" +
                    diagnostic.Message + "|" +
                    ContextSignature(diagnostic.RemediationContext));
            }

            return values.AsReadOnly();
        }

        private static string ContextSignature(IReadOnlyDictionary<string, object?> context)
        {
            var keys = new List<string>(context.Keys);
            keys.Sort(StringComparer.Ordinal);
            var parts = new List<string>();
            for (var index = 0; index < keys.Count; index++)
            {
                var key = keys[index];
                parts.Add(key + "=" + Convert.ToString(context[key], CultureInfo.InvariantCulture));
            }

            return string.Join(";", parts);
        }
    }
}
