using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw05NeutralFixtureAuthorityTests
    {
        [Fact]
        public void NeutralAuthorityFixtureIsRealSourceOpenableAndProjectsThroughGenericPath()
        {
            var root = FindRepositoryRoot();
            const string relativePath = "Docs/evidence/WP-DW-05/fixtures/neutral-observatory.txt";
            var absolutePath = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
            var source = File.ReadAllText(absolutePath).Replace("\r\n", "\n", StringComparison.Ordinal);
            var expected =
                "OBSERVATORY|id=observatory.north|elevation_m=2410|remote=true\n" +
                "DETECTOR|id=detector.spectro|wavelength_min_nm=380.5|wavelength_max_nm=740.25\n" +
                "CALIBRATION|id=calibration.lamp-a|mode=argon\n" +
                "CONDITION|sky=clear\n";

            Assert.Equal(expected, source);

            var universe = new StaticDesignAuthorityUniverse(new[]
            {
                "observatory.north",
                "detector.spectro",
                "calibration.lamp-a"
            });
            var reader = new AnchoredTextAuthorityReader(
                "neutral-observatory-v1",
                relativePath,
                source,
                new[]
                {
                    new AnchoredFactDefinition(
                        "observatory.north",
                        "observatory",
                        "OBSERVATORY|id=observatory.north|elevation_m=2410|remote=true",
                        new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                        {
                            ["elevation-m"] = DesignValue.Integer(2410),
                            ["remote"] = DesignValue.Boolean(true)
                        },
                        new[] { new DesignRelation("houses", "detector.spectro") }),
                    new AnchoredFactDefinition(
                        "detector.spectro",
                        "detector",
                        "DETECTOR|id=detector.spectro|wavelength_min_nm=380.5|wavelength_max_nm=740.25",
                        new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                        {
                            ["wavelength-min-nm"] = DesignValue.Decimal(380.5m),
                            ["wavelength-max-nm"] = DesignValue.Decimal(740.25m)
                        },
                        new[] { new DesignRelation("calibrated-by", "calibration.lamp-a") }),
                    new AnchoredFactDefinition(
                        "calibration.lamp-a",
                        "calibration-source",
                        "CALIBRATION|id=calibration.lamp-a|mode=argon",
                        new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                        {
                            ["mode"] = DesignValue.String("argon")
                        })
                });

            var projection = new DesignWorldProjector().Build(
                universe,
                reader,
                new DesignProjectionVersion(1, "dw05-neutral-v1"));
            var report = new DesignWorldProjectionValidator().Validate(
                projection,
                universe,
                reader,
                new DesignProjectionVersion(1, "dw05-neutral-v1"));

            Assert.True(report.IsValid, string.Join("; ", report.Issues.Select(issue => issue.MachineCode)));
            Assert.All(projection.Facts, fact =>
            {
                Assert.Equal(relativePath, fact.Provenance.SourcePath);
                Assert.True(File.Exists(Path.Combine(root, fact.Provenance.SourcePath.Replace('/', Path.DirectorySeparatorChar))));
                Assert.Equal(DesignAuthorityResolutionStatus.Found, reader.Resolve(fact.Provenance));
            });
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
