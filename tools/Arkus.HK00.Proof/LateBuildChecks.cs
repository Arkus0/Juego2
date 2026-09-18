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
            ExternalAuthority authority;
            try
            {
                authority = ExternalAuthority.Create(root);
            }
            catch (Exception ex)
            {
                return Report("HK00-EXTERNAL-AUTHORITY", "effective", "external-authority", ex.Message);
            }
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
                        if (!authority.IsSdkOrPack(absolute))
                        {
                            findings += Report(
                                "HK00-COMPILER-ANALYZER-UNTRUSTED",
                                "effective",
                                spec.Name + ":" + absolute,
                                "Actual C# compiler command line contains an analyzer/source-generator outside the single selected SDK/reference-pack authority.");
                        }
                    }
                }
            }

            return findings;
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
