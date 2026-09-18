using System;
using System.Collections.Generic;
using System.IO;
using Arkus.Game.Authoring;
using Arkus.Game.Core;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Proves the declared module direction is real code, not only project metadata.
    /// </summary>
    public sealed class KernelCompositionTests
    {
        [Fact]
        public void ProtocolComposesOnlyItself()
        {
            Assert.Equal(new[] { "Arkus.Harness.Protocol" }, ProtocolModule.ComposedModules);
        }

        [Fact]
        public void CoreComposesOnlyItself()
        {
            Assert.Equal(new[] { "Arkus.Game.Core" }, CoreModule.ComposedModules);
        }

        [Fact]
        public void WorldComposesCore()
        {
            Assert.Equal(new[] { "Arkus.Game.Core", "Arkus.Game.World" }, WorldModule.ComposedModules);
        }

        [Fact]
        public void AuthoringComposesCoreWorldAndProtocol()
        {
            Assert.Equal(
                new[]
                {
                    "Arkus.Game.Authoring",
                    "Arkus.Game.Core",
                    "Arkus.Game.World",
                    "Arkus.Harness.Protocol",
                },
                AuthoringModule.ComposedModules);
        }

        [Fact]
        public void ValidationComposesCoreWorldAndProtocol()
        {
            Assert.Equal(
                new[]
                {
                    "Arkus.Game.Core",
                    "Arkus.Game.Validation",
                    "Arkus.Game.World",
                    "Arkus.Harness.Protocol",
                },
                ValidationModule.ComposedModules);
        }

        [Fact]
        public void RuntimeComposesTheWholeKernel()
        {
            Assert.Equal(
                new[]
                {
                    "Arkus.Game.Authoring",
                    "Arkus.Game.Core",
                    "Arkus.Game.Validation",
                    "Arkus.Game.World",
                    "Arkus.Harness.Protocol",
                    "Arkus.Harness.Runtime",
                },
                RuntimeModule.ComposedModules);
        }

        [Fact]
        public void HeadlessHostRunsAndReportsTheComposedKernel()
        {
            var originalOut = Console.Out;
            using var writer = new StringWriter();
            int exitCode;

            try
            {
                Console.SetOut(writer);
                exitCode = Cli.Program.Main();
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            var printed = new List<string>(
                writer.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

            Assert.Equal(0, exitCode);
            Assert.Equal(new List<string>(RuntimeModule.ComposedModules), printed);
        }
    }
}
