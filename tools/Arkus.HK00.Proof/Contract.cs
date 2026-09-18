using System;
using System.Collections.Generic;

namespace Arkus.HK00.Proof
{
    internal enum ProjectKind
    {
        Portable,
        Host,
        Tests,
        Proof,
    }

    internal sealed class ProjectSpec
    {
        public ProjectSpec(
            string name,
            string path,
            ProjectKind kind,
            string[] dependencies,
            string[] allowedPackages)
        {
            Name = name;
            Path = path;
            Kind = kind;
            Dependencies = dependencies;
            AllowedPackages = allowedPackages;
        }

        public string Name { get; }
        public string Path { get; }
        public ProjectKind Kind { get; }
        public string[] Dependencies { get; }
        public string[] AllowedPackages { get; }
        public string Directory => System.IO.Path.GetDirectoryName(Path)!.Replace('\\', '/');
        public bool IsProduct => Kind == ProjectKind.Portable || Kind == ProjectKind.Host;
        public string TargetFramework => Kind == ProjectKind.Portable ? "netstandard2.1" : "net8.0";
    }

    internal static class FixedContract
    {
        public const string SdkVersion = "8.0.425";
        public const string SdkRollForward = "disable";

        public static readonly IReadOnlyList<ProjectSpec> Projects = new[]
        {
            new ProjectSpec("Arkus.Harness.Protocol", "src/Arkus.Harness.Protocol/Arkus.Harness.Protocol.csproj", ProjectKind.Portable, Array.Empty<string>(), Array.Empty<string>()),
            new ProjectSpec("Arkus.Game.Core", "src/Arkus.Game.Core/Arkus.Game.Core.csproj", ProjectKind.Portable, Array.Empty<string>(), Array.Empty<string>()),
            new ProjectSpec("Arkus.Game.World", "src/Arkus.Game.World/Arkus.Game.World.csproj", ProjectKind.Portable, new[] { "Arkus.Game.Core" }, Array.Empty<string>()),
            new ProjectSpec("Arkus.Game.Authoring", "src/Arkus.Game.Authoring/Arkus.Game.Authoring.csproj", ProjectKind.Portable, new[] { "Arkus.Game.Core", "Arkus.Game.World", "Arkus.Harness.Protocol" }, Array.Empty<string>()),
            new ProjectSpec("Arkus.Game.Validation", "src/Arkus.Game.Validation/Arkus.Game.Validation.csproj", ProjectKind.Portable, new[] { "Arkus.Game.Core", "Arkus.Game.World", "Arkus.Harness.Protocol" }, Array.Empty<string>()),
            new ProjectSpec("Arkus.Harness.Runtime", "src/Arkus.Harness.Runtime/Arkus.Harness.Runtime.csproj", ProjectKind.Portable, new[] { "Arkus.Game.Authoring", "Arkus.Game.Validation", "Arkus.Harness.Protocol" }, Array.Empty<string>()),
            new ProjectSpec("Arkus.Harness.Cli", "src/Arkus.Harness.Cli/Arkus.Harness.Cli.csproj", ProjectKind.Host, new[] { "Arkus.Harness.Runtime" }, Array.Empty<string>()),
            new ProjectSpec("Arkus.Harness.Tests", "tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj", ProjectKind.Tests, new[] { "Arkus.Game.Authoring", "Arkus.Game.Core", "Arkus.Game.Validation", "Arkus.Game.World", "Arkus.Harness.Protocol", "Arkus.Harness.Runtime" }, new[] { "Microsoft.NET.Test.Sdk", "xunit", "xunit.runner.visualstudio" }),
            new ProjectSpec("Arkus.HK00.Proof", "tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj", ProjectKind.Proof, Array.Empty<string>(), Array.Empty<string>()),
        };

        public static readonly IReadOnlyList<string> RequiredFiles = new[]
        {
            "global.json",
            "NuGet.config",
            "Directory.Build.props",
            "Directory.Packages.props",
            "Juego2.sln",
            ".github/workflows/hk00-ci.yml",
            "scripts/proof.sh",
            "scripts/self-attacks/run-self-attacks.sh",
            "scripts/self-attacks/run-closure-attacks.sh",
            "scripts/self-attacks/run-terminal-inventory-attack.sh",
            "tools/Arkus.HK00.Proof/Arkus.HK00.Proof.csproj",
        };

        public static ProjectSpec? ByName(string name)
        {
            foreach (var project in Projects)
            {
                if (string.Equals(project.Name, name, StringComparison.Ordinal))
                {
                    return project;
                }
            }

            return null;
        }

        public static ProjectSpec? ByPath(string path)
        {
            var normalized = path.Replace('\\', '/');
            foreach (var project in Projects)
            {
                if (string.Equals(project.Path, normalized, StringComparison.Ordinal))
                {
                    return project;
                }
            }

            return null;
        }
    }
}
