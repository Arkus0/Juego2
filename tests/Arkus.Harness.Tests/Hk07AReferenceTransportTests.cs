using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Arkus.Harness.Protocol;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk07AReferenceTransportTests
    {
        [Fact]
        public void CleanOneShotProcessDiscoversCanonicalInventoryAndKeepsDiagnosticsOffStdout()
        {
            var result = Hk07AProcessHarness.Run(
                new[] { "--once", "--diagnostics" },
                Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame("describe", "system.describe", Empty())));

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("arkus-host: ready mode=once", result.StandardError, StringComparison.Ordinal);
            Assert.DoesNotContain("arkus-host", result.StandardOutput, StringComparison.Ordinal);
            var lines = result.StandardOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.Single(lines);

            using var document = JsonDocument.Parse(lines[0]);
            var response = document.RootElement.GetProperty("response");
            Assert.Equal("success", response.GetProperty("status").GetString());
            var capabilities = response.GetProperty("result").GetProperty("capabilities");
            Assert.True(capabilities.GetArrayLength() >= 16);
            Assert.Contains(capabilities.EnumerateArray(), value =>
                value.GetProperty("name").GetString() == "authoring.journal.replay");
        }

        [Fact]
        public void MalformedTruncatedInvalidUtf8AndOversizedFramesFailWithStableTransportErrors()
        {
            AssertErrorCode(
                Hk07AProcessHarness.Run(new[] { "--once" }, Encoding.UTF8.GetBytes("{\"value\":}")),
                "transport.malformed_json");
            AssertErrorCode(
                Hk07AProcessHarness.Run(new[] { "--once" }, Array.Empty<byte>()),
                "transport.truncated_frame");
            AssertErrorCode(
                Hk07AProcessHarness.Run(new[] { "--once" }, Encoding.UTF8.GetBytes("{")),
                "transport.truncated_frame");
            AssertErrorCode(
                Hk07AProcessHarness.Run(new[] { "--once" }, new byte[] { 0xc3, 0x28, (byte)'\n' }),
                "transport.invalid_utf8");

            var oversized = Enumerable.Repeat(
                (byte)'x',
                H0ResourceEnvelope.MaximumTransportFrameBytes + 1).ToArray();
            Array.Resize(ref oversized, oversized.Length + 1);
            oversized[oversized.Length - 1] = (byte)'\n';
            AssertErrorCode(
                Hk07AProcessHarness.Run(new[] { "--once" }, oversized),
                "resource.request_bytes_exceeded");
        }

        [Fact]
        public void TimeoutAndCanonicalFailuresRemainDistinctMachineReadableOutcomes()
        {
            var expired = Hk07AProcessHarness.Run(
                new[] { "--once" },
                Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame(
                    "expired",
                    "world.summary",
                    Empty(),
                    0)));
            using var expiredDocument = JsonDocument.Parse(expired.StandardOutput);
            var expiredResponse = expiredDocument.RootElement.GetProperty("response");
            Assert.Equal("timed-out", expiredResponse.GetProperty("failureKind").GetString());
            Assert.Equal("projection.timeout", expiredResponse.GetProperty("error").GetProperty("machineCode").GetString());

            var unknown = Hk07AProcessHarness.Run(
                new[] { "--once" },
                Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame("unknown", "missing.capability", Empty())));
            using var unknownDocument = JsonDocument.Parse(unknown.StandardOutput);
            var unknownResponse = unknownDocument.RootElement.GetProperty("response");
            Assert.Equal("canonical", unknownResponse.GetProperty("failureKind").GetString());
            Assert.Equal("contract.unknown_capability", unknownResponse.GetProperty("error").GetProperty("machineCode").GetString());
        }

        [Fact]
        public void UnrelatedEnvironmentCannotChangeCanonicalProcessSemantics()
        {
            var input = Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame("summary", "world.summary", Empty()));
            var baseline = Hk07AProcessHarness.Run(new[] { "--once" }, input);
            var altered = Hk07AProcessHarness.Run(
                new[] { "--once" },
                input,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["ARKUS_WORLD_ID"] = "world.environment.must-not-win",
                    ["ARKUS_TRANSPORT_REGISTRY"] = "base-only",
                    ["ARKUS_DEFAULT_CAPABILITY"] = "missing.capability"
                });

            Assert.Equal(0, baseline.ExitCode);
            Assert.Equal(0, altered.ExitCode);
            Assert.Equal(baseline.StandardOutput, altered.StandardOutput);
            Assert.Equal(string.Empty, baseline.StandardError);
            Assert.Equal(string.Empty, altered.StandardError);
        }

        [Fact]
        public void FileLaunchModeIsNotExposedByTheH0ProductionHost()
        {
            var path = Path.Combine(Path.GetTempPath(), "arkus-hk07a-" + Guid.NewGuid().ToString("N") + ".jsonl");
            try
            {
                File.WriteAllText(
                    path,
                    Hk07AProcessHarness.Frame("file.summary", "world.summary", Empty()),
                    new UTF8Encoding(false));
                var result = Hk07AProcessHarness.Run(new[] { "--file", path }, Array.Empty<byte>());

                Assert.Equal(64, result.ExitCode);
                Assert.Equal(string.Empty, result.StandardOutput);
                Assert.Contains("--file is not exposed", result.StandardError, StringComparison.Ordinal);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void UnknownLaunchOptionFailsBeforeProtocolOutput()
        {
            var result = Hk07AProcessHarness.Run(new[] { "--unknown" }, Array.Empty<byte>());

            Assert.Equal(64, result.ExitCode);
            Assert.Equal(string.Empty, result.StandardOutput);
            Assert.Contains("unknown option", result.StandardError, StringComparison.Ordinal);
        }

        [Fact]
        public void MultipleOneShotFramesAndUnknownOrDuplicateEnvelopeFieldsFailClosed()
        {
            var frame = Hk07AProcessHarness.Frame("first", "world.summary", Empty());
            AssertErrorCode(
                Hk07AProcessHarness.Run(
                    new[] { "--once" },
                    Encoding.UTF8.GetBytes(frame + "\n" + frame)),
                "transport.multiple_frames");

            const string unknownField =
                "{\"protocol\":\"arkus.reference.jsonl@1\",\"request\":{},\"extra\":true}";
            AssertErrorCode(
                Hk07AProcessHarness.Run(new[] { "--once" }, Encoding.UTF8.GetBytes(unknownField)),
                "transport.invalid_frame");

            const string duplicateField =
                "{\"protocol\":\"arkus.reference.jsonl@1\",\"protocol\":\"arkus.reference.jsonl@1\",\"request\":{}}";
            AssertErrorCode(
                Hk07AProcessHarness.Run(new[] { "--once" }, Encoding.UTF8.GetBytes(duplicateField)),
                "transport.invalid_frame");
        }

        private static void AssertErrorCode(Hk07AProcessResult result, string expectedCode)
        {
            Assert.Equal(2, result.ExitCode);
            using var document = JsonDocument.Parse(result.StandardOutput);
            var response = document.RootElement.GetProperty("response");
            Assert.Equal("transport", response.GetProperty("failureKind").GetString());
            Assert.Equal(expectedCode, response.GetProperty("error").GetProperty("machineCode").GetString());
        }

        private static IReadOnlyDictionary<string, object?> Empty() =>
            new Dictionary<string, object?>(StringComparer.Ordinal);
    }

    internal sealed class Hk07AProcessResult
    {
        public Hk07AProcessResult(int exitCode, string standardOutput, string standardError)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
        }

        public int ExitCode { get; }
        public string StandardOutput { get; }
        public string StandardError { get; }
    }

    internal static class Hk07AProcessHarness
    {
        internal static Hk07AProcessResult Run(
            IReadOnlyList<string> arguments,
            byte[] standardInput,
            IReadOnlyDictionary<string, string>? environment = null)
        {
            var start = StartInfo(arguments);
            if (environment != null)
            {
                foreach (var pair in environment) start.Environment[pair.Key] = pair.Value;
            }

            using var process = Process.Start(start) ?? throw new InvalidOperationException("Failed to start Arkus CLI.");
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            process.StandardInput.BaseStream.Write(standardInput, 0, standardInput.Length);
            process.StandardInput.Close();
            Assert.True(process.WaitForExit(30000), "Arkus CLI did not exit within 30 seconds.");
            return new Hk07AProcessResult(process.ExitCode, outputTask.Result, errorTask.Result);
        }

        internal static Process Start(IReadOnlyList<string>? arguments = null)
        {
            return Process.Start(StartInfo(arguments ?? Array.Empty<string>())) ??
                throw new InvalidOperationException("Failed to start Arkus CLI.");
        }

        internal static string Frame(
            string requestId,
            string capability,
            object arguments,
            int? timeoutMilliseconds = null)
        {
            var request = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["projectionVersion"] = "arkus.neutral-projection@1",
                ["requestId"] = requestId,
                ["capability"] = capability,
                ["acceptedVersions"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["major"] = 1,
                    ["minimumMinor"] = 0,
                    ["maximumMinor"] = 0
                },
                ["arguments"] = arguments
            };
            if (timeoutMilliseconds.HasValue) request["timeoutMilliseconds"] = timeoutMilliseconds.Value;

            return JsonSerializer.Serialize(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["protocol"] = "arkus.reference.jsonl@1",
                ["request"] = request
            });
        }

        internal static string FindRepositoryRoot()
        {
            DirectoryInfo? current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "Juego2.sln"))) return current.FullName;
                current = current.Parent;
            }
            throw new InvalidOperationException("Unable to locate Juego2 repository root.");
        }

        private static ProcessStartInfo StartInfo(IReadOnlyList<string> arguments)
        {
            var root = FindRepositoryRoot();
            var cli = Path.Combine(
                root,
                "src",
                "Arkus.Harness.Cli",
                "bin",
                "Release",
                "net8.0",
                "Arkus.Harness.Cli.dll");
            if (!File.Exists(cli)) throw new InvalidOperationException("Release Arkus CLI assembly is missing: " + cli);

            var host = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
            if (string.IsNullOrWhiteSpace(host)) host = "dotnet";
            var start = new ProcessStartInfo(host)
            {
                WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardInputEncoding = new UTF8Encoding(false),
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false),
                CreateNoWindow = true
            };
            start.ArgumentList.Add(cli);
            foreach (var argument in arguments) start.ArgumentList.Add(argument);
            return start;
        }
    }
}
