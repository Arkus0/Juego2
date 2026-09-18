using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Arkus.HK00.Proof
{
    internal sealed class ProcessResult
    {
        public ProcessResult(int exitCode, string stdout, string stderr)
        {
            ExitCode = exitCode;
            Stdout = stdout;
            Stderr = stderr;
        }

        public int ExitCode { get; }
        public string Stdout { get; }
        public string Stderr { get; }
    }

    internal static class ProcessExec
    {
        public static ProcessResult Run(string fileName, IReadOnlyList<string> arguments, string workingDirectory, bool throwOnFailure = true)
        {
            var startInfo = new ProcessStartInfo(fileName)
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

            startInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
            startInfo.Environment["DOTNET_NOLOGO"] = "1";
            startInfo.Environment["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1";
            startInfo.Environment["MSBUILDDISABLENODEREUSE"] = "1";

            using var process = new Process { StartInfo = startInfo };
            if (!process.Start())
            {
                throw new InvalidOperationException($"Could not start '{fileName}'.");
            }

            // Preserve stdout/stderr exactly. Line-oriented DataReceived handlers append
            // separators that do not exist in the child output and corrupt NUL-delimited
            // Git inventories (in particular their final entry).
            Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
            Task<string> stderrTask = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            Task.WaitAll(stdoutTask, stderrTask);

            var result = new ProcessResult(process.ExitCode, stdoutTask.Result, stderrTask.Result);
            if (throwOnFailure && result.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"{fileName} {string.Join(" ", arguments)} failed with exit code {result.ExitCode}.{Environment.NewLine}{result.Stdout}{result.Stderr}");
            }

            return result;
        }

        public static IReadOnlyList<string> SplitNull(string value)
        {
            var results = new List<string>();
            foreach (var part in value.Split('\0', StringSplitOptions.RemoveEmptyEntries))
            {
                results.Add(part);
            }

            return results;
        }

        public static string Relative(string root, string path)
        {
            return Path.GetRelativePath(Path.GetFullPath(root), Path.GetFullPath(path)).Replace('\\', '/');
        }

        public static bool IsInside(string directory, string path)
        {
            var root = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            var full = Path.GetFullPath(path);
            return full.StartsWith(root, StringComparison.Ordinal);
        }
    }
}
