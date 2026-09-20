using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.Game.Authoring;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk10InspectionOrderingTests
    {
        [Fact]
        public void ObjectQueryOrderMatchesIndependentOrdinalIdOracle()
        {
            var state = Hk02TestFixtures.MicroWorld();
            var contract = Hk03InspectionTests.Compose(state);
            var request = Hk03InspectionTests.Anchor(state);
            request["limit"] = 100;

            var result = Hk03InspectionTests.Dispatch(
                contract,
                WorldInspectionContract.ObjectQueryName,
                request);
            var actual = Hk03InspectionTests.List(result.Data!, "items")
                .Select(item => (string)((IReadOnlyDictionary<string, object?>)item!)["id"]!)
                .ToArray();
            var expected = state.Objects
                .Select(item => item.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray();

            Assert.Equal(expected, actual);
        }
    }
}
