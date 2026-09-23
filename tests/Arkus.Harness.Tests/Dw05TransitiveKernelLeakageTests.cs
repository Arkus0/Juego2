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
        [Fact]
        public void DesignWorldH0DependencyClosureIncludesGameCoreAndContainsNoCityOrPaLeakage()
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
        public void InjectedPaAssumptionInTransitiveKernelDependencyTurnsLeakageAuditRed()
        {
            var injected = new[]
            {
                new Surface(
                    "src/Arkus.Game.Core/PaKernelDependency.cs",
                    "namespace Arkus.Game.Core { public sealed class PaKernelDependency { } }")
            };

            Assert.Contains(
                FindLeakage(injected),
                item => item.Contains("PaKernelDependency", StringComparison.Ordinal));
        }

        private static IEnumerable<string> ProjectSurfaceFiles(string directory) =>
            Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .Where(path => path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".csproj", StringComparison.Ordinal))
                .OrderBy(path => path, StringComparer.Ordinal);

        private static string[] FindLeakage(IEnumerable<Surface> surfaces)
        {
            var pattern = new Regex(
                @"\bcity\b|\bpa\b|\bCity[A-Z][A-Za-z0-9_]*\b|\bPa[A-Z][A-Za-z0-9_]*\b",
                RegexOptions.CultureInvariant);
            return surfaces
                .Where(surface => pattern.IsMatch(surface.Content) || pattern.IsMatch(Path.GetFileName(surface.Path)))
                .Select(surface => surface.Path)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
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
