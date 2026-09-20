using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk09AHostCapabilityContainmentTests
    {
        [Fact]
        public void ProductionH0InventoryPassesTheHostCapabilityPolicy()
        {
            var contract = CanonicalWorldContract.ComposeEmptyPortableSession("world.hk09a.policy");
            Assert.Empty(H0HostCapabilityPolicy.Validate(contract.Definitions));
        }

        [Fact]
        public void PolicyRejectsExternalEffectsElevatedPrivilegeAndContradictoryMetadata()
        {
            var external = Definition(
                "test.external",
                SideEffectClass.ExternalReversible,
                PrivilegeClass.Authoring,
                TransactionRequirement.ReadOnlyEnvelope);
            var elevated = Definition(
                "test.elevated",
                SideEffectClass.ReadOnly,
                PrivilegeClass.Elevated,
                TransactionRequirement.ReadOnlyEnvelope);
            var contradictory = Definition(
                "test.contradictory",
                SideEffectClass.ReadOnly,
                PrivilegeClass.PublicRead,
                TransactionRequirement.CanonicalTransaction);

            var externalIssues = H0HostCapabilityPolicy.Validate(new[] { external });
            var elevatedIssues = H0HostCapabilityPolicy.Validate(new[] { elevated });
            var contradictoryIssues = H0HostCapabilityPolicy.Validate(new[] { contradictory });

            Assert.Contains(externalIssues, issue => issue.Code == H0HostCapabilityPolicy.ExternalEffectCode);
            Assert.Contains(elevatedIssues, issue => issue.Code == H0HostCapabilityPolicy.ElevatedPrivilegeCode);
            Assert.Contains(contradictoryIssues, issue => issue.Code == H0HostCapabilityPolicy.MetadataMismatchCode);
        }

        [Fact]
        public void ParentTraversalFileAttemptFailsBeforeProtocolOutput()
        {
            var result = Hk07AProcessHarness.Run(
                new[] { "--file", "../outside-h0-boundary.jsonl" },
                Array.Empty<byte>());

            Assert.Equal(64, result.ExitCode);
            Assert.Equal(string.Empty, result.StandardOutput);
            Assert.Contains("--file is not exposed", result.StandardError, StringComparison.Ordinal);
        }

        [Fact]
        public void SymlinkEscapeFileAttemptFailsBeforeProtocolOutput()
        {
            if (OperatingSystem.IsWindows()) return;

            var root = Path.Combine(Path.GetTempPath(), "arkus-hk09a-" + Guid.NewGuid().ToString("N"));
            var outside = Path.Combine(root, "outside");
            var inside = Path.Combine(root, "inside");
            var link = Path.Combine(inside, "escape");
            Directory.CreateDirectory(outside);
            Directory.CreateDirectory(inside);
            File.WriteAllText(Path.Combine(outside, "request.jsonl"), "{}", new UTF8Encoding(false));
            Directory.CreateSymbolicLink(link, outside);

            try
            {
                var result = Hk07AProcessHarness.Run(
                    new[] { "--file", Path.Combine(link, "request.jsonl") },
                    Array.Empty<byte>());

                Assert.Equal(64, result.ExitCode);
                Assert.Equal(string.Empty, result.StandardOutput);
                Assert.Contains("--file is not exposed", result.StandardError, StringComparison.Ordinal);
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        [Fact]
        public void UnsupportedGenericProcessCapabilityCannotBeMintedByProtocolPayload()
        {
            var result = Hk07AProcessHarness.Run(
                new[] { "--once" },
                Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame(
                    "shell-attempt",
                    "host.process.execute",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["command"] = "echo should-not-run"
                    })));

            AssertCanonicalCode(result, "contract.unknown_capability");
        }

        [Fact]
        public void NetworkShapedPayloadCannotAcquireNetworkAuthority()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var endpoint = (IPEndPoint)listener.LocalEndpoint;

            var result = Hk07AProcessHarness.Run(
                new[] { "--once" },
                Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame(
                    "network-attempt",
                    "world.summary",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["url"] = "http://127.0.0.1:" + endpoint.Port + "/should-not-connect"
                    })));

            Assert.Equal(2, result.ExitCode);
            Assert.False(listener.Pending());
            AssertCanonicalFailure(result);
        }

        [Fact]
        public void RuntimeTypeSelectorCannotAcquireRuntimeActivationAuthority()
        {
            var result = Hk07AProcessHarness.Run(
                new[] { "--once" },
                Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame(
                    "type-attempt",
                    "world.summary",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["$type"] = "System.Diagnostics.Process, System.Diagnostics.Process"
                    })));

            Assert.Equal(2, result.ExitCode);
            AssertCanonicalFailure(result);
        }

        [Fact]
        public void AdapterOnlyPrivilegedCapabilityCannotAppearOutsideCanonicalInventory()
        {
            var describe = Hk07AProcessHarness.Run(
                new[] { "--once" },
                Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame(
                    "describe-policy",
                    "system.describe",
                    new Dictionary<string, object?>(StringComparer.Ordinal))));

            Assert.Equal(0, describe.ExitCode);
            using var document = JsonDocument.Parse(describe.StandardOutput);
            var capabilities = document.RootElement.GetProperty("response").GetProperty("result").GetProperty("capabilities");
            foreach (var capability in capabilities.EnumerateArray())
            {
                var sideEffect = capability.GetProperty("sideEffect").GetString();
                var policy = capability.GetProperty("policy");
                var privilege = policy.GetProperty("privilege").GetString();
                Assert.NotEqual("external-reversible", sideEffect);
                Assert.NotEqual("external-irreversible", sideEffect);
                Assert.NotEqual("elevated", privilege);
            }

            var invented = Hk07AProcessHarness.Run(
                new[] { "--once" },
                Encoding.UTF8.GetBytes(Hk07AProcessHarness.Frame(
                    "invented-privilege",
                    "host.filesystem.read",
                    new Dictionary<string, object?>(StringComparer.Ordinal))));
            AssertCanonicalCode(invented, "contract.unknown_capability");
        }

        [Fact]
        public void EffectiveProductionSourceSurfaceHasNoShellNetworkOrRuntimeActivationPrimitive()
        {
            var root = Hk07AProcessHarness.FindRepositoryRoot();
            var sourceRoot = Path.Combine(root, "src");
            var sourceFiles = Directory.GetFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
                .Where(path => !HasGeneratedSegment(path))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            var forbidden = new[]
            {
                "using System.Diagnostics;",
                "Process.Start(",
                "ProcessStartInfo",
                "using System.Net",
                "HttpClient",
                "TcpClient",
                "UdpClient",
                "WebRequest",
                "Type.GetType(",
                "Activator.CreateInstance",
                "Assembly.Load(",
                "NativeLibrary.Load",
                "DllImport("
            };

            foreach (var path in sourceFiles)
            {
                var text = File.ReadAllText(path);
                foreach (var token in forbidden)
                {
                    Assert.DoesNotContain(token, text, StringComparison.Ordinal);
                }
            }

            var filesystemAuthorityFiles = sourceFiles
                .Where(path =>
                {
                    var text = File.ReadAllText(path);
                    return text.Contains("FileStream", StringComparison.Ordinal) ||
                        text.Contains("File.", StringComparison.Ordinal) ||
                        text.Contains("Directory.", StringComparison.Ordinal);
                })
                .Select(path => Path.GetRelativePath(root, path).Replace('\\', '/'))
                .ToArray();

            Assert.Equal(new[] { "src/Arkus.Harness.Cli/ReferenceTransport.cs" }, filesystemAuthorityFiles);

            var cliProgram = File.ReadAllText(Path.Combine(sourceRoot, "Arkus.Harness.Cli", "Program.cs"));
            var rejection = cliProgram.IndexOf("--file is not exposed", StringComparison.Ordinal);
            var delegation = cliProgram.IndexOf("ReferenceTransportHost.Run", StringComparison.Ordinal);
            Assert.True(rejection >= 0 && delegation > rejection,
                "The executable must reject file authority before delegating to the legacy framing host.");

            var mcpProgram = File.ReadAllText(Path.Combine(sourceRoot, "Arkus.Harness.Mcp", "Program.cs"));
            Assert.Contains("ProductionHarnessHost.Create()", mcpProgram, StringComparison.Ordinal);
            Assert.DoesNotContain("System.IO", mcpProgram, StringComparison.Ordinal);
            Assert.DoesNotContain("System.Net", mcpProgram, StringComparison.Ordinal);
        }

        private static bool HasGeneratedSegment(string path)
        {
            var normalized = path.Replace('\\', '/');
            return normalized.Contains("/bin/", StringComparison.Ordinal) ||
                normalized.Contains("/obj/", StringComparison.Ordinal);
        }

        private static CapabilityDefinition Definition(
            string name,
            SideEffectClass sideEffect,
            PrivilegeClass privilege,
            TransactionRequirement transaction)
        {
            return new CapabilityDefinition(
                new CapabilityKey(name, new ContractVersion(1, 0)),
                new ProviderMetadata("test.hk09a", ProviderKind.Scoped, "hk09a", "test"),
                null,
                null,
                null,
                sideEffect,
                DeterminismClass.Deterministic,
                Array.Empty<string>(),
                Array.Empty<string>(),
                null,
                null,
                null,
                null,
                new PolicySemantics(privilege, transaction, ProvenanceRequirement.Minimal));
        }

        private static void AssertCanonicalCode(Hk07AProcessResult result, string code)
        {
            Assert.Equal(2, result.ExitCode);
            using var document = JsonDocument.Parse(result.StandardOutput);
            var response = document.RootElement.GetProperty("response");
            Assert.Equal("canonical", response.GetProperty("failureKind").GetString());
            Assert.Equal(code, response.GetProperty("error").GetProperty("machineCode").GetString());
        }

        private static void AssertCanonicalFailure(Hk07AProcessResult result)
        {
            using var document = JsonDocument.Parse(result.StandardOutput);
            Assert.Equal(
                "canonical",
                document.RootElement.GetProperty("response").GetProperty("failureKind").GetString());
        }
    }
}
