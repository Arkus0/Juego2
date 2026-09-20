using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk07BMcpCancellationConformanceTests
    {
        [Fact]
        public void PreCancelledMcpAdmissionReturnsNeutralCancellationWithoutCanonicalDispatch()
        {
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { Hk01TestFixtures.FixtureProvider() });
            Assert.True(composition.Success);
            using var projection = new NeutralProjectionService(composition.Contract!);

            var (adapterType, adapter) = CreateAdapter(projection);
            var describe = adapterType.GetMethod("DescribeCapabilities", BindingFlags.Public | BindingFlags.Instance)!;
            var projected = ((IEnumerable)describe.Invoke(adapter, null)!).Cast<object>().Single(value =>
            {
                var definition = (CapabilityDefinition)value.GetType().GetProperty("Definition")!.GetValue(value)!;
                return definition.Key.ToString() == "engine.observe@1.0";
            });
            var toolName = (string)projected.GetType().GetProperty("ToolName")!.GetValue(projected)!;

            var invoke = adapterType.GetMethod("InvokeProjectedAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            FixtureEngineHandler.InvocationCount = 0;
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            var task = (Task<NeutralProjectionOutcome>)invoke.Invoke(
                adapter,
                new object?[]
                {
                    toolName,
                    new Dictionary<string, object?>(StringComparer.Ordinal),
                    "mcp:causal-cancel",
                    null,
                    cancellation.Token
                })!;
            var outcome = task.GetAwaiter().GetResult();

            Assert.False(outcome.Success);
            Assert.Equal(NeutralProjectionFailureKind.Cancelled, outcome.FailureKind);
            Assert.Equal("projection.cancelled", outcome.Error!.MachineCode);
            Assert.Equal(0, FixtureEngineHandler.InvocationCount);
        }

        [Fact]
        public void McpToolNameConstraintStaysTransportFramingWithoutCanonicalIdentityAmendment()
        {
            using var projection = ProductionHarnessHost.Create();
            var (adapterType, adapter) = CreateAdapter(projection);
            var describe = adapterType.GetMethod("DescribeCapabilities", BindingFlags.Public | BindingFlags.Instance)!;
            var projected = ((IEnumerable)describe.Invoke(adapter, null)!).Cast<object>().Single(value =>
            {
                var definition = (CapabilityDefinition)value.GetType().GetProperty("Definition")!.GetValue(value)!;
                return definition.Key.ToString() == "world.summary@1.0";
            });

            var definition = (CapabilityDefinition)projected.GetType().GetProperty("Definition")!.GetValue(projected)!;
            var toolName = (string)projected.GetType().GetProperty("ToolName")!.GetValue(projected)!;

            Assert.Equal("world.summary@1.0", definition.Key.ToString());
            Assert.Equal("world.summary_401.0", toolName);
            Assert.NotEqual(definition.Key.ToString(), toolName);

            var neutralRequest = new NeutralProjectionRequest(
                "neutral.identity",
                definition.Key.Name,
                ContractVersionRange.Exact(definition.Key.Version),
                new Dictionary<string, object?>(StringComparer.Ordinal));
            var neutralFields = neutralRequest.ToData().Keys.ToArray();
            Assert.Empty(McpFramingLeakIssues(neutralFields));

            var deliberatelyAmendedNeutralShape = neutralFields.Concat(new[] { "toolName" }).ToArray();
            Assert.Contains("mcp-framing-field:toolName", McpFramingLeakIssues(deliberatelyAmendedNeutralShape));
        }

        [Fact]
        public void SdkReplacementBoundaryOracleRejectsCanonicalLayerDependencyLeak()
        {
            var root = Hk07AProcessHarness.FindRepositoryRoot();
            var canonicalProjects = new[]
            {
                "src/Arkus.Harness.Protocol/Arkus.Harness.Protocol.csproj",
                "src/Arkus.Harness.Runtime/Arkus.Harness.Runtime.csproj",
                "src/Arkus.Harness.Projection/Arkus.Harness.Projection.csproj"
            };
            var actual = canonicalProjects.Select(relative => new KeyValuePair<string, string>(
                relative,
                File.ReadAllText(Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar))))).ToArray();

            Assert.Empty(SdkDependencyLeakIssues(actual));

            var injected = actual.Select(pair => pair.Key.EndsWith("Arkus.Harness.Projection.csproj", StringComparison.Ordinal)
                ? new KeyValuePair<string, string>(pair.Key, pair.Value + "\n<PackageReference Include=\"ModelContextProtocol.Core\" />")
                : pair).ToArray();
            Assert.Contains(
                "mcp-sdk-dependency:src/Arkus.Harness.Projection/Arkus.Harness.Projection.csproj",
                SdkDependencyLeakIssues(injected));
        }

        private static IReadOnlyList<string> McpFramingLeakIssues(IEnumerable<string> neutralFields)
        {
            var transportOnly = new HashSet<string>(StringComparer.Ordinal)
            {
                "toolName", "jsonrpc", "_meta"
            };
            return neutralFields
                .Where(transportOnly.Contains)
                .Select(value => "mcp-framing-field:" + value)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
        }

        private static IReadOnlyList<string> SdkDependencyLeakIssues(
            IEnumerable<KeyValuePair<string, string>> canonicalProjectTexts)
        {
            return canonicalProjectTexts
                .Where(pair => pair.Value.IndexOf("ModelContextProtocol", StringComparison.Ordinal) >= 0)
                .Select(pair => "mcp-sdk-dependency:" + pair.Key)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
        }

        private static (Type AdapterType, object Adapter) CreateAdapter(NeutralProjectionService projection)
        {
            var root = Hk07AProcessHarness.FindRepositoryRoot();
            var assemblyPath = Path.Combine(
                root,
                "src",
                "Arkus.Harness.Mcp",
                "bin",
                "Release",
                "net8.0",
                "Arkus.Harness.Mcp.dll");
            Assert.True(File.Exists(assemblyPath), "Release MCP assembly is missing: " + assemblyPath);
            var assembly = Assembly.LoadFrom(assemblyPath);
            var adapterType = assembly.GetType("Arkus.Harness.Mcp.McpProjectionAdapter", throwOnError: true)!;
            var adapter = Activator.CreateInstance(adapterType, projection)!;
            return (adapterType, adapter);
        }
    }
}
