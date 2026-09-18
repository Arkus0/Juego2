using System;
using System.Collections.Generic;
using System.IO;

namespace Arkus.HK00.Proof
{
    internal static class CompilerInputClosureChecks
    {
        private static readonly string[] ForbiddenInputSwitches =
        {
            "addmodule",
            "link",
            "resource",
            "linkresource",
            "win32res",
            "win32icon",
            "recurse",
            "additionalfile",
        };

        public static int Run(string root, string configuration)
        {
            var findings = 0;
            var probe = new MsBuildProbe(root, configuration);
            var sdkDirectory = RunningSdkDirectory(root);
            var sdkBase = Directory.GetParent(sdkDirectory)
                ?? throw new InvalidOperationException("Selected SDK directory has no parent.");
            var dotnetRoot = Directory.GetParent(sdkBase.FullName)
                ?? throw new InvalidOperationException("Selected SDK base has no DOTNET_ROOT parent.");
            var packsDirectory = Path.Combine(dotnetRoot.FullName, "packs");

            foreach (var spec in FixedContract.Projects)
            {
                CompilerCommandLine compiler;
                try
                {
                    compiler = CompilerCommandLine.Parse(
                        probe.CompilerArguments(spec),
                        Path.Combine(root, spec.Directory));
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-COMPILER-INPUT-ORACLE", "effective", spec.Name, ex.Message);
                    continue;
                }

                foreach (var name in ForbiddenInputSwitches)
                {
                    foreach (var value in compiler.Values(name))
                    {
                        findings += Report(
                            "HK00-COMPILER-INPUT-UNCLASSIFIED",
                            "effective",
                            spec.Name + ":/" + name,
                            "Compiler received an input-bearing switch outside the closed HK00 input model: " + value);
                    }
                }

                foreach (var value in compiler.Values("analyzerconfig"))
                {
                    var token = value.Trim().Trim('"');
                    if (token.Length == 0)
                    {
                        continue;
                    }

                    var full = Path.IsPathRooted(token)
                        ? Path.GetFullPath(token)
                        : Path.GetFullPath(Path.Combine(Path.Combine(root, spec.Directory), token));
                    var allowedObj = Path.Combine(root, spec.Directory, "obj");
                    var trusted = ProcessExec.IsInside(allowedObj, full)
                        || ProcessExec.IsInside(sdkDirectory, full)
                        || (Directory.Exists(packsDirectory) && ProcessExec.IsInside(packsDirectory, full));
                    if (!trusted)
                    {
                        findings += Report(
                            "HK00-COMPILER-ANALYZERCONFIG-UNTRUSTED",
                            "effective",
                            spec.Name + ":" + full,
                            "Analyzer config input must come from this project's generated obj tree or the pinned SDK/reference-pack roots.");
                    }
                }
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

        private static int Report(string id, string phase, string subject, string message)
        {
            Console.Error.WriteLine(id + " [" + phase + "] " + subject + ": " + message);
            return 1;
        }
    }
}
