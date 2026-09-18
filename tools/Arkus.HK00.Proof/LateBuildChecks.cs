using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Arkus.HK00.Proof
{
    internal sealed class TrackedBytesSnapshot
    {
        public TrackedBytesSnapshot(Dictionary<string, string> hashes)
        {
            Hashes = hashes;
        }

        public Dictionary<string, string> Hashes { get; }
    }

    internal static class LateBuildChecks
    {
        public static int CheckCandidateTree(string root)
        {
            var findings = 0;
            findings += ReportGitDelta(
                root,
                new[] { "diff-index", "--cached", "--name-only", "-z", "HEAD", "--" },
                "index differs from exact candidate HEAD");
            findings += ReportGitDelta(
                root,
                new[] { "diff-files", "--name-only", "-z", "--" },
                "worktree bytes differ from the candidate index");
            return findings;
        }

        public static TrackedBytesSnapshot CaptureTrackedBytes(string root)
        {
            var hashes = new Dictionary<string, string>(StringComparer.Ordinal);
            var result = ProcessExec.Run("git", new[] { "ls-files", "-z" }, root);
            foreach (var raw in ProcessExec.SplitNull(result.Stdout))
            {
                var relative = raw.Replace('\\', '/');
                var full = Path.Combine(root, relative);
                if (!File.Exists(full))
                {
                    continue;
                }
                hashes[relative] = Sha256(full);
            }
            return new TrackedBytesSnapshot(hashes);
        }

        public static int CheckTrackedBytesStable(string root, TrackedBytesSnapshot before)
        {
            var findings = 0;
            foreach (var pair in before.Hashes)
            {
                var full = Path.Combine(root, pair.Key);
                if (!File.Exists(full))
                {
                    findings += Report(
                        "HK00-BUILD-MUTATED-TRACKED",
                        "effective",
                        pair.Key,
                        "Tracked candidate file disappeared during effective build/proof execution.");
                    continue;
                }

                var after = Sha256(full);
                if (!string.Equals(pair.Value, after, StringComparison.Ordinal))
                {
                    findings += Report(
                        "HK00-BUILD-MUTATED-TRACKED",
                        "effective",
                        pair.Key,
                        "Tracked candidate bytes changed during effective build/proof execution.");
                }
            }

            return findings;
        }

        public static int CheckEffectiveCompilerExtensions(string root, string configuration)
        {
            var findings = 0;
            var sdkDirectory = RunningSdkDirectory(root);
            var dotnetRoot = Directory.GetParent(Directory.GetParent(sdkDirectory)!.FullName)!.FullName;
            var packsDirectory = Path.Combine(dotnetRoot, "packs");
            var probe = new MsBuildProbe(root, configuration);

            foreach (var spec in FixedContract.Projects)
            {
                if (!spec.IsProduct)
                {
                    continue;
                }

                IReadOnlyList<string> arguments;
                try
                {
                    arguments = probe.CompilerArguments(spec);
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-COMPILER-EXTENSION-ORACLE", "effective", spec.Name, ex.Message);
                    continue;
                }

                var projectDirectory = Path.Combine(root, spec.Directory);
                var compiler = CompilerCommandLine.Parse(arguments, projectDirectory);
                foreach (var value in compiler.Values("analyzer"))
                {
                    foreach (var rawPath in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var token = rawPath.Trim().Trim('"');
                        if (token.Length == 0)
                        {
                            continue;
                        }

                        var absolute = Path.IsPathRooted(token)
                            ? Path.GetFullPath(token)
                            : Path.GetFullPath(Path.Combine(projectDirectory, token));
                        if (!IsTrustedCompilerExtension(sdkDirectory, packsDirectory, absolute))
                        {
                            findings += Report(
                                "HK00-COMPILER-ANALYZER-UNTRUSTED",
                                "effective",
                                spec.Name + ":" + absolute,
                                "Actual C# compiler command line contains an analyzer/source-generator outside the selected SDK or its .NET reference/workload packs.");
                        }
                    }
                }
            }

            return findings;
        }

        private static bool IsTrustedCompilerExtension(string sdkDirectory, string packsDirectory, string path)
        {
            return ProcessExec.IsInside(sdkDirectory, path)
                || (Directory.Exists(packsDirectory) && ProcessExec.IsInside(packsDirectory, path));
        }

        private static int ReportGitDelta(string root, IReadOnlyList<string> args, string reason)
        {
            var result = ProcessExec.Run("git", args, root);
            var findings = 0;
            foreach (var raw in ProcessExec.SplitNull(result.Stdout))
            {
                findings += Report(
                    "HK00-CANDIDATE-TREE-DIVERGENCE",
                    "repository",
                    raw.Replace('\\', '/'),
                    reason + ".");
            }
            return findings;
        }

        private static string RunningSdkDirectory(string root)
        {
            var version = ProcessExec.Run("dotnet", new[] { "--version" }, root).Stdout.Trim();
            var list = ProcessExec.Run("dotnet", new[] { "--list-sdks" }, root).Stdout;
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
                    return Path.GetFullPath(Path.Combine(baseDirectory, version));
                }
            }
            throw new InvalidOperationException("Could not derive selected SDK directory for " + version + ".");
        }

        private static string Sha256(string path)
        {
            using var stream = File.OpenRead(path);
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(stream));
        }

        private static int Report(string id, string phase, string subject, string message)
        {
            Console.Error.WriteLine(id + " [" + phase + "] " + subject + ": " + message);
            return 1;
        }
    }
}
