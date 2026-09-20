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
    public sealed class Hk07BMcpLongNameConformanceTests
    {
        [Fact]
        public void OverLimitScopedCapabilitiesRemainDiscoverableUniqueAndInvokable()
        {
            var firstDefinition = Hk01TestFixtures.DefinitionForProvider(
                "fixture.long",
                "long",
                Hk07BLongCapabilityNames.First,
                new ContractVersion(1, 0),
                false,
                "long");
            var secondDefinition = Hk01TestFixtures.DefinitionForProvider(
                "fixture.long",
                "long",
                Hk07BLongCapabilityNames.Second,
                new ContractVersion(1, 0),
                false,
                "long");
            var provider = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.long", ProviderKind.Scoped, "long", new[] { "long" }),
                new[] { firstDefinition, secondDefinition },
                new[]
                {
                    CapabilityRoute.FromHandler(new Hk07BLongCapabilityFirstHandler()),
                    CapabilityRoute.FromHandler(new Hk07BLongCapabilitySecondHandler())
                });

            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                new[] { provider });
            Assert.True(composition.Success);
            Assert.Equal(123, Hk07BLongCapabilityNames.First.Length);
            Assert.Equal(123, Hk07BLongCapabilityNames.Second.Length);

            using var projection = new NeutralProjectionService(composition.Contract!);
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
            var describe = adapterType.GetMethod("DescribeCapabilities", BindingFlags.Public | BindingFlags.Instance)!;
            var values = ((IEnumerable)describe.Invoke(adapter, null)!).Cast<object>().ToArray();
            var projectedByKey = values.ToDictionary(
                value => ((CapabilityDefinition)value.GetType().GetProperty("Definition")!.GetValue(value)!).Key.ToString(),
                value => (string)value.GetType().GetProperty("ToolName")!.GetValue(value)!,
                StringComparer.Ordinal);

            var firstKey = firstDefinition.Key.ToString();
            var secondKey = secondDefinition.Key.ToString();
            Assert.True(projectedByKey.TryGetValue(firstKey, out var firstToolName));
            Assert.True(projectedByKey.TryGetValue(secondKey, out var secondToolName));
            Assert.NotEqual(firstToolName, secondToolName);
            Assert.InRange(firstToolName!.Length, 1, 128);
            Assert.InRange(secondToolName!.Length, 1, 128);
            Assert.True(firstToolName.StartsWith("arkus-long-", StringComparison.Ordinal));
            Assert.True(secondToolName.StartsWith("arkus-long-", StringComparison.Ordinal));

            Hk07BLongCapabilityFirstHandler.InvocationCount = 0;
            Hk07BLongCapabilitySecondHandler.InvocationCount = 0;
            var invoke = adapterType.GetMethod("InvokeProjectedAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var firstTask = (Task<NeutralProjectionOutcome>)invoke.Invoke(
                adapter,
                new object?[]
                {
                    firstToolName,
                    Hk01TestFixtures.EmptyRequest(),
                    "mcp:hk07b-long-first",
                    null,
                    CancellationToken.None
                })!;
            var secondTask = (Task<NeutralProjectionOutcome>)invoke.Invoke(
                adapter,
                new object?[]
                {
                    secondToolName,
                    Hk01TestFixtures.EmptyRequest(),
                    "mcp:hk07b-long-second",
                    null,
                    CancellationToken.None
                })!;

            Assert.True(firstTask.GetAwaiter().GetResult().Success);
            Assert.True(secondTask.GetAwaiter().GetResult().Success);
            Assert.Equal(1, Hk07BLongCapabilityFirstHandler.InvocationCount);
            Assert.Equal(1, Hk07BLongCapabilitySecondHandler.InvocationCount);
        }
    }

    internal static class Hk07BLongCapabilityNames
    {
        internal const string First = "long.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaax";
        internal const string Second = "long.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaay";
    }

    [PublicCapabilityRoute("fixture.long", Hk07BLongCapabilityNames.First, "1.0")]
    public sealed class Hk07BLongCapabilityFirstHandler : ICanonicalCapabilityHandler
    {
        public static int InvocationCount { get; set; }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            InvocationCount++;
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = "first"
            });
        }
    }

    [PublicCapabilityRoute("fixture.long", Hk07BLongCapabilityNames.Second, "1.0")]
    public sealed class Hk07BLongCapabilitySecondHandler : ICanonicalCapabilityHandler
    {
        public static int InvocationCount { get; set; }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            InvocationCount++;
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = "second"
            });
        }
    }
}
