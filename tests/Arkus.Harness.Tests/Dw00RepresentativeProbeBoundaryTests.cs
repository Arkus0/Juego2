using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Arkus.Game.Authoring;
using Arkus.Game.World;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw00RepresentativeProbeBoundaryTests
    {
        [Fact]
        public void ApprovedCityShapeRebuildsAndDiffsThroughTheOwnedGenericBoundaries()
        {
            var repositoryRoot = FindRepositoryRoot();
            var sourcePath = Path.Combine(repositoryRoot, "Docs", "production", "CITY_LOCATION_PROGRAMME.md");
            var sourceText = File.ReadAllText(sourcePath);
            var sourceBytesBefore = File.ReadAllBytes(sourcePath);
            var row = "| `loc.casco.bar` | Casco Viejo | bar / social house | `W.CASCO` | food, drink, social, information | **A** | **S4** | **I3** | public + service + semi-private layers | service/social use, meeting/witness potential, repeat interior use |";
            var definitions = Definitions(row);
            var universe = new StaticDesignAuthorityUniverse(new[] { "loc.casco.bar", "mobility.w.casco" });
            var version = new DesignProjectionVersion(1, "dw00-v1");
            var firstReader = new AnchoredTextAuthorityReader(
                "city-location-programme-v1",
                "Docs/production/CITY_LOCATION_PROGRAMME.md",
                sourceText,
                definitions);
            var projector = new DesignWorldProjector();

            var first = projector.Build(universe, firstReader, version);
            var rebuiltReader = new AnchoredTextAuthorityReader(
                "city-location-programme-v1",
                "Docs/production/CITY_LOCATION_PROGRAMME.md",
                sourceText,
                definitions);
            var rebuilt = projector.Build(universe, rebuiltReader, version);
            var validation = new DesignWorldProjectionValidator().Validate(rebuilt, universe, rebuiltReader, version);
            var inspection = new WorldInspectionService(new FixedWorldStateSource(rebuilt.WorldState));
            var references = inspection.QueryReferences(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["revision"] = rebuilt.WorldState.Revision,
                ["hash"] = CanonicalWorldStateCodec.ComputeContentHash(rebuilt.WorldState)
            });
            var sourceBytesAfter = File.ReadAllBytes(sourcePath);

            Assert.True(validation.IsValid, string.Join("; ", validation.Issues.Select(issue => issue.MachineCode)));
            Assert.True(references.Success);
            Assert.Equal(first.Digest, rebuilt.Digest);
            Assert.True(DesignWorldProjectionDiff.Compare(first, rebuilt).IsEmpty);
            Assert.Equal(sourceBytesBefore, sourceBytesAfter);
        }

        private static AnchoredFactDefinition[] Definitions(string row)
        {
            return new[]
            {
                new AnchoredFactDefinition(
                    "loc.casco.bar",
                    "programme-place",
                    row,
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["district"] = DesignValue.String("Casco Viejo"),
                        ["importance"] = DesignValue.String("A"),
                        ["spatial-depth"] = DesignValue.String("S4"),
                        ["interior-priority"] = DesignValue.String("I3")
                    },
                    new[] { new DesignRelation("mobility-anchor", "mobility.w.casco") }),
                new AnchoredFactDefinition(
                    "mobility.w.casco",
                    "programme-mobility-anchor",
                    row,
                    new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                    {
                        ["planning-id"] = DesignValue.String("W.CASCO")
                    })
            };
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
    }
}
