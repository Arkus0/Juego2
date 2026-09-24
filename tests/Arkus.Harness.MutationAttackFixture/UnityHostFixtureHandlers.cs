using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;

namespace Arkus.Harness.H1HostPolicyFixture
{
    [PublicCapabilityRoute("arkus.unity-host", "unity.host.inspect", "1.0")]
    public sealed class UnityHostFixtureHandler : ICanonicalCapabilityHandler
    {
        public static int InvocationCount { get; set; }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            InvocationCount++;
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = "ok"
            });
        }
    }

    [PublicCapabilityRoute("arkus.unity-host", "unity.host.touch", "1.0")]
    public sealed class UnityHostTouchFixtureHandler : ICanonicalCapabilityHandler
    {
        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            UnityHostFixtureHandler.InvocationCount++;
            return CapabilityInvocationResult.Succeeded(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["value"] = "ok"
            });
        }
    }
}
