using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
        public static int FirstInvocationCount { get; set; }
        public static int SecondInvocationCount { get; set; }

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
            var firstHandler = CreateAttributedHandler(Hk07BLongCapabilityNames.First);
            var secondHandler = CreateAttributedHandler(Hk07BLongCapabilityNames.Second);
            var provider = new CanonicalProviderContribution(
                new ProviderDescriptor("fixture.long", ProviderKind.Scoped, "long", new[] { "long" }),
                new[] { firstDefinition, secondDefinition },
                new[]
                {
                    CapabilityRoute.FromHandler(firstHandler),
                    CapabilityRoute.FromHandler(secondHandler)
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

            FirstInvocationCount = 0;
            SecondInvocationCount = 0;
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
            Assert.Equal(1, FirstInvocationCount);
            Assert.Equal(1, SecondInvocationCount);
        }

        public static CapabilityInvocationResult InvokeDynamicHandler(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (string.Equals(context.Definition.Key.Name, Hk07BLongCapabilityNames.First, StringComparison.Ordinal))
            {
                FirstInvocationCount++;
            }
            else if (string.Equals(context.Definition.Key.Name, Hk07BLongCapabilityNames.Second, StringComparison.Ordinal))
            {
                SecondInvocationCount++;
            }
            else
            {
                throw new InvalidOperationException("Unexpected dynamic long-name fixture route: " + context.Definition.Key);
            }

            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = context.Definition.Key.Name
            });
        }

        private static ICanonicalCapabilityHandler CreateAttributedHandler(string capabilityName)
        {
            var assemblyName = new AssemblyName("Arkus.Harness.Tests.Hk07BLongNameFixture." + Guid.NewGuid().ToString("N"));
            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(assemblyName.Name!);
            var type = module.DefineType(
                "LongNameHandler_" + Guid.NewGuid().ToString("N"),
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);
            type.AddInterfaceImplementation(typeof(ICanonicalCapabilityHandler));

            var routeConstructor = typeof(PublicCapabilityRouteAttribute).GetConstructor(new[]
            {
                typeof(string), typeof(string), typeof(string)
            })!;
            type.SetCustomAttribute(new CustomAttributeBuilder(
                routeConstructor,
                new object[] { "fixture.long", capabilityName, "1.0" }));

            var invokeContract = typeof(ICanonicalCapabilityHandler).GetMethod(nameof(ICanonicalCapabilityHandler.Invoke))!;
            var invoke = type.DefineMethod(
                nameof(ICanonicalCapabilityHandler.Invoke),
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                typeof(CapabilityInvocationResult),
                new[]
                {
                    typeof(CapabilityInvocationContext),
                    typeof(IReadOnlyDictionary<string, object>)
                });
            var il = invoke.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, typeof(Hk07BMcpLongNameConformanceTests).GetMethod(
                nameof(InvokeDynamicHandler),
                BindingFlags.Public | BindingFlags.Static)!);
            il.Emit(OpCodes.Ret);
            type.DefineMethodOverride(invoke, invokeContract);

            return (ICanonicalCapabilityHandler)Activator.CreateInstance(type.CreateType()!)!;
        }
    }

    internal static class Hk07BLongCapabilityNames
    {
        internal const string First = "long.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaax";
        internal const string Second = "long.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaay";
    }
}
