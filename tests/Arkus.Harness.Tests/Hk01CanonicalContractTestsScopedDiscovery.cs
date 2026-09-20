using System;
using System.Collections.Generic;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk01CanonicalContractTestsScopedDiscovery
    {
        [Fact]
        public void SystemDescribeReturnsEveryAcceptedBaseAndScopedCapabilityVersion()
        {
            var composition = ContractComposer.Compose(
                BaseContract.CreateContribution(),
                Hk01TestFixtures.CompleteFixtureProviders());
            Assert.True(composition.Success);

            var result = composition.Contract!.Dispatch(
                "system.describe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                Hk01TestFixtures.EmptyRequest());

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            var capabilities = Assert.IsAssignableFrom<IReadOnlyList<object?>>(result.Data!["capabilities"]);
            Assert.Equal(5, capabilities.Count);

            var discovered = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in capabilities)
            {
                var capability = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(value);
                discovered.Add((string)capability["name"]! + "@" + (string)capability["version"]!);
            }

            Assert.Contains("system.describe@1.0", discovered);
            Assert.Contains("system.resource-envelope.describe@1.0", discovered);
            Assert.Contains("engine.observe@1.0", discovered);
            Assert.Contains("engine.observe@1.1", discovered);
            Assert.Contains("orphan.route@1.0", discovered);
        }
    }
}
