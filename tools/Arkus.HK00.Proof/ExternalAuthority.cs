using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Arkus.HK00.Proof
{
    internal sealed class ExternalAuthority
    {
        private const string TestLockPath = "tests/Arkus.Harness.Tests/packages.lock.json";
        private readonly Dictionary<string, string> lockedPackages;

        private ExternalAuthority(string sdkDirectory, string packsDirectory, string packageRoot, Dictionary<string, string> lockedPackages)
        {
            SdkDirectory = sdkDirectory;
            PacksDirectory = packsDirectory;
            PackageRoot = packageRoot;
            this.lockedPackages = lockedPackages;
        }

        public string SdkDirectory { get; }
        public string PacksDirectory { get; }
        public string PackageRoot { get; }

        public static ExternalAuthority Create(string root)
        {
            var version = ProcessExec.Run("dotnet", new[] { "--version" }, root).Stdout.Trim();
            if (!string.Equals(version, FixedContract.SdkVersion, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unexpected SDK while establishing external authority: " + version);
            }

            var list = ProcessExec.Run("dotnet", new[] { "--list-sdks" }, root).Stdout;
            string? sdkDirectory = null;
            foreach (var rawLine in list.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var line = rawLine.Trim();
                if (!line.StartsWith(version + " ", StringComparison.Ordinal))
                {
                    continue;
                }
                var open = line.LastIndexOf('[');
                var close = line.LastIndexOf(']');
                if (open >= 0 && close > open)
                {
                    var baseDirectory = line.Substring(open + 1, close - open - 1);
                    sdkDirectory = Path.GetFullPath(Path.Combine(baseDirectory, version));
                    break;
                }
            }
            if (sdkDirectory is null)
            {
                throw new InvalidOperationException("Could not derive selected SDK directory for " + version + ".");
            }

            var dotnetRoot = Directory.GetParent(Directory.GetParent(sdkDirectory)!.FullName)!.FullName;
            var packsDirectory = Path.Combine(dotnetRoot, "packs");
            var configuredPackages = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
            var packageRoot = !string.IsNullOrWhiteSpace(configuredPackages)
                ? Path.GetFullPath(configuredPackages)
                : Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages"));

            var lockFile = Path.Combine(root, TestLockPath);
            if (!File.Exists(lockFile))
            {
                throw new InvalidOperationException("Committed test package lock is missing: " + TestLockPath);
            }

            using var document = JsonDocument.Parse(File.ReadAllText(lockFile));
            if (!document.RootElement.TryGetProperty("dependencies", out var dependencies)
                || !dependencies.TryGetProperty("net8.0", out var target))
            {
                throw new InvalidOperationException("Committed test package lock has no net8.0 dependency graph.");
            }

            var packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var dependency in target.EnumerateObject())
            {
                if (dependency.Value.TryGetProperty("type", out var type)
                    && string.Equals(type.GetString(), "Project", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!dependency.Value.TryGetProperty("resolved", out var resolved)
                    || !dependency.Value.TryGetProperty("contentHash", out var contentHash))
                {
                    throw new InvalidOperationException("Locked package lacks resolved version or contentHash: " + dependency.Name);
                }
                var resolvedVersion = resolved.GetString();
                var hash = contentHash.GetString();
                if (string.IsNullOrWhiteSpace(resolvedVersion) || string.IsNullOrWhiteSpace(hash))
                {
                    throw new InvalidOperationException("Locked package has empty resolved version or contentHash: " + dependency.Name);
                }
                packages[dependency.Name.ToLowerInvariant()] = resolvedVersion;
            }
            if (packages.Count == 0)
            {
                throw new InvalidOperationException("Committed test package lock contains no resolved packages.");
            }

            return new ExternalAuthority(sdkDirectory, packsDirectory, packageRoot, packages);
        }

        public bool IsSdkOrPack(string path)
        {
            var full = Path.GetFullPath(path);
            return ProcessExec.IsInside(SdkDirectory, full)
                || (Directory.Exists(PacksDirectory) && ProcessExec.IsInside(PacksDirectory, full));
        }

        public bool IsFrameworkReference(string path)
        {
            var full = Path.GetFullPath(path);
            return Directory.Exists(PacksDirectory) && ProcessExec.IsInside(PacksDirectory, full);
        }

        public bool IsLockedTestPackagePath(string path)
        {
            var full = Path.GetFullPath(path);
            if (!ProcessExec.IsInside(PackageRoot, full))
            {
                return false;
            }

            var relative = ProcessExec.Relative(PackageRoot, full);
            var segments = relative.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 3)
            {
                return false;
            }

            var packageId = segments[0].ToLowerInvariant();
            var version = segments[1];
            return lockedPackages.TryGetValue(packageId, out var lockedVersion)
                && string.Equals(version, lockedVersion, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsExpectedTestSdkSource(ProjectSpec spec, string path)
        {
            if (spec.Kind != ProjectKind.Tests)
            {
                return false;
            }

            var expected = Path.GetFullPath(Path.Combine(
                PackageRoot,
                "microsoft.net.test.sdk",
                FixedContract.PackageVersions["Microsoft.NET.Test.Sdk"],
                "build",
                "net8.0",
                "Microsoft.NET.Test.Sdk.Program.cs"));
            return string.Equals(Path.GetFullPath(path), expected, StringComparison.Ordinal)
                && IsLockedTestPackagePath(expected);
        }
    }
}
