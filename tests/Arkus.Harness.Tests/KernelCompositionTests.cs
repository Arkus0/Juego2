using Arkus.Game.Authoring;
using Arkus.Game.Core;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class KernelCompositionTests
    {
        [Fact]
        public void RequiredBoundariesAreReachableThroughDeclaredEdges()
        {
            Assert.Equal("Arkus.Game.Core", CoreModule.Name);
            Assert.Equal("Arkus.Game.Core", WorldModule.CoreBoundary);
            Assert.Equal("Arkus.Game.Core:Arkus.Game.World:Arkus.Harness.Protocol", AuthoringModule.Composition);
            Assert.Equal("Arkus.Game.Core:Arkus.Game.World:Arkus.Harness.Protocol", ValidationModule.Composition);
            Assert.Equal("Arkus.Harness.Protocol:Arkus.Game.Authoring:Arkus.Game.Validation", RuntimeModule.Composition);
            Assert.Equal("Arkus.Harness.Protocol", ProtocolModule.Name);
        }
    }
}
