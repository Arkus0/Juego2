using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw05ResidualInventoryCompletenessTests
    {
        [Fact]
        public void AcceptedDw00ThroughDw03ResidualSectionsExactlyMatchIndependentInventoryAnchors()
        {
            var root = FindRepositoryRoot();
            var inventory = ReadInventory(root);

            AssertSourceResiduals(
                root,
                inventory,
                "Docs/evidence/WP-DW-00/RESIDUAL_RISK.md",
                ExtractTableFirstCells(ReadSource(root, "Docs/evidence/WP-DW-00/RESIDUAL_RISK.md"), "## Bounded residuals / non-claims"));

            AssertSourceResiduals(
                root,
                inventory,
                "Docs/evidence/WP-DW-01/RESIDUAL_RISK.md",
                ExtractTableFirstCells(ReadSource(root, "Docs/evidence/WP-DW-01/RESIDUAL_RISK.md"), "## Non-blocking residuals"));

            AssertSourceResiduals(
                root,
                inventory,
                "Docs/evidence/WP-DW-02/RESIDUAL_RISK.md",
                ExtractBullets(ReadSource(root, "Docs/evidence/WP-DW-02/RESIDUAL_RISK.md"), "## Non-blocking residuals"));

            AssertSourceResiduals(
                root,
                inventory,
                "Docs/evidence/WP-DW-03/RESIDUAL_RISK.md",
                ExtractNumberedItems(ReadSource(root, "Docs/evidence/WP-DW-03/RESIDUAL_RISK.md"), "## Residual risks outside the DW-03 claim"));
        }

        [Fact]
        public void Dw04HasNoResidualRiskFileAndAcceptedDiagnosticTokenCostLimitIsInventoried()
        {
            var root = FindRepositoryRoot();
            var inventory = ReadInventory(root);
            const string residualPath = "Docs/evidence/WP-DW-04/RESIDUAL_RISK.md";
            const string docSyncPath = "Docs/evidence/WP-DW-04/DOCSYNC.md";
            const string anchor = "Provider token/cost observations remain diagnostic only; no universal token- or cost-saving claim is accepted.";

            Assert.False(File.Exists(ToAbsolute(root, residualPath)));
            Assert.Contains(anchor, ReadSource(root, docSyncPath), StringComparison.Ordinal);
            Assert.Contains(inventory.Clauses, clause => clause.Source == docSyncPath && clause.Anchor == anchor);
        }

        private static void AssertSourceResiduals(
            string root,
            ResidualInventory inventory,
            string sourcePath,
            IReadOnlyCollection<string> extracted)
        {
            Assert.NotEmpty(ReadSource(root, sourcePath));
            var inventoried = inventory.Clauses
                .Where(clause => clause.Source == sourcePath)
                .Select(clause => clause.Anchor)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(anchor => anchor, StringComparer.Ordinal)
                .ToArray();
            var expected = extracted
                .Distinct(StringComparer.Ordinal)
                .OrderBy(anchor => anchor, StringComparer.Ordinal)
                .ToArray();

            Assert.NotEmpty(expected);
            Assert.Equal(expected, inventoried);
        }

        private static string[] ExtractTableFirstCells(string source, string heading)
        {
            return ExtractSection(source, heading)
                .Split('\n')
                .Where(line => line.StartsWith("|", StringComparison.Ordinal))
                .Select(line => line.Split('|'))
                .Where(parts => parts.Length >= 3)
                .Select(parts => parts[1].Trim())
                .Where(cell => cell.Length > 0 && cell != "Residual" && !cell.All(ch => ch == '-' || ch == ':' || char.IsWhiteSpace(ch)))
                .ToArray();
        }

        private static string[] ExtractBullets(string source, string heading)
        {
            return ExtractSection(source, heading)
                .Split('\n')
                .Where(line => line.StartsWith("- ", StringComparison.Ordinal))
                .Select(line => line.Substring(2).Trim())
                .ToArray();
        }

        private static string[] ExtractNumberedItems(string source, string heading)
        {
            var pattern = new Regex(@"^[0-9]+\.\s+(.*)$", RegexOptions.CultureInvariant);
            return ExtractSection(source, heading)
                .Split('\n')
                .Select(line => pattern.Match(line))
                .Where(match => match.Success)
                .Select(match => match.Groups[1].Value.Trim())
                .ToArray();
        }

        private static string ExtractSection(string source, string heading)
        {
            var normalized = source.Replace("\r\n", "\n", StringComparison.Ordinal);
            var start = normalized.IndexOf(heading, StringComparison.Ordinal);
            if (start < 0)
            {
                throw new InvalidOperationException("Missing residual heading: " + heading);
            }

            start += heading.Length;
            var next = normalized.IndexOf("\n## ", start, StringComparison.Ordinal);
            return next < 0 ? normalized.Substring(start) : normalized.Substring(start, next - start);
        }

        private static ResidualInventory ReadInventory(string root)
        {
            var json = ReadSource(root, "Docs/evidence/WP-DW-05/PREDECESSOR_RESIDUAL_INVENTORY.json");
            return JsonSerializer.Deserialize<ResidualInventory>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Could not parse predecessor residual inventory.");
        }

        private static string ReadSource(string root, string relativePath) =>
            File.ReadAllText(ToAbsolute(root, relativePath)).Replace("\r\n", "\n", StringComparison.Ordinal);

        private static string ToAbsolute(string root, string relativePath) =>
            Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));

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

        private sealed class ResidualInventory
        {
            [JsonPropertyName("clauses")]
            public List<ResidualClause> Clauses { get; set; } = new List<ResidualClause>();
        }

        private sealed class ResidualClause
        {
            [JsonPropertyName("source")]
            public string Source { get; set; } = string.Empty;

            [JsonPropertyName("anchor")]
            public string Anchor { get; set; } = string.Empty;
        }
    }
}
