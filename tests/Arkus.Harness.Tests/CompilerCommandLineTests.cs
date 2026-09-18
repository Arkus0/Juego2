using System.IO;
using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// The compiler command line is the last word on how a project was built, so
    /// its parsing must not quietly mis-read an option as a source file.
    /// </summary>
    public sealed class CompilerCommandLineTests
    {
        private static readonly string[] SampleArguments =
        {
            "/noconfig",
            "/nowarn:1701,1702,1701,1702",
            "/nullable:enable",
            "/reference:/packs/netstandard.dll",
            "/reference:/out/Arkus.Game.Core.dll",
            "/debug:portable",
            "/warnaserror+",
            "/deterministic+",
            "/langversion:9.0",
            "/embed",
            "WorldModule.cs",
            "/warnaserror+:NU1605",
        };

        [Fact]
        public void SwitchesAreRecognised()
        {
            var line = CompilerCommandLine.Parse(SampleArguments, "/project");

            Assert.True(line.HasSwitch("warnaserror+"));
            Assert.True(line.HasSwitch("deterministic+"));
            Assert.True(line.HasSwitch("embed"));
            Assert.False(line.HasSwitch("warnaserror-"));
        }

        [Fact]
        public void ValuedOptionsAreRead()
        {
            var line = CompilerCommandLine.Parse(SampleArguments, "/project");

            Assert.Equal("9.0", line.Value("langversion"));
            Assert.Equal("enable", line.Value("nullable"));
            Assert.Equal("portable", line.Value("debug"));
            Assert.Equal(string.Empty, line.Value("moduleassemblyname"));
        }

        [Fact]
        public void RepeatedOptionsAreCollected()
        {
            var line = CompilerCommandLine.Parse(SampleArguments, "/project");

            Assert.Equal(
                new[] { "/packs/netstandard.dll", "/out/Arkus.Game.Core.dll" },
                line.Values("reference"));
        }

        [Fact]
        public void SuppressedWarningsAreDeduplicatedAndSorted()
        {
            var line = CompilerCommandLine.Parse(SampleArguments, "/project");

            Assert.Equal(new[] { "1701", "1702" }, line.SuppressedWarnings());
        }

        [Fact]
        public void RelativeSourcesResolveAgainstTheProjectDirectory()
        {
            var line = CompilerCommandLine.Parse(SampleArguments, "/project");

            Assert.Equal(new[] { "/project/WorldModule.cs" }, line.Sources);
        }

        [Fact]
        public void AbsoluteSourcePathsAreNotMistakenForOptions()
        {
            var directory = Path.Combine(Path.GetTempPath(), "arkus-cmdline-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            try
            {
                var generated = Path.Combine(directory, "Generated.AssemblyInfo.cs");
                File.WriteAllText(generated, "// generated");

                var line = CompilerCommandLine.Parse(new[] { "/noconfig", generated }, directory);

                Assert.Equal(new[] { generated }, line.Sources);
                Assert.True(line.HasSwitch("noconfig"));
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        [Fact]
        public void EmptyArgumentsAreIgnored()
        {
            var line = CompilerCommandLine.Parse(new[] { string.Empty, "/embed" }, "/project");

            Assert.Empty(line.Sources);
            Assert.True(line.HasSwitch("embed"));
        }
    }
}
