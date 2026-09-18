using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// Toolchain drift detection rules.
    /// </summary>
    public sealed class SdkPinRulesTests
    {
        [Theory]
        [InlineData("disable", true)]
        [InlineData("patch", true)]
        [InlineData("latestPatch", true)]
        [InlineData("feature", false)]
        [InlineData("latestFeature", false)]
        [InlineData("latestMinor", false)]
        [InlineData("latestMajor", false)]
        [InlineData("", false)]
        public void OnlyFeatureBandPinningPoliciesAreNarrowEnough(string rollForward, bool expected)
        {
            Assert.Equal(expected, SdkPinRules.IsNarrowEnough(rollForward));
        }

        [Fact]
        public void PatchInsideTheFeatureBandSatisfiesThePin()
        {
            var pin = new GlobalJsonPin("8.0.100", "latestPatch", false);

            Assert.True(SdkPinRules.IsSatisfiedBy(pin, SdkVersion.Parse("8.0.131"), out _));
        }

        [Fact]
        public void HigherFeatureBandDoesNotSatisfyAPatchPin()
        {
            var pin = new GlobalJsonPin("8.0.100", "latestPatch", false);

            Assert.False(SdkPinRules.IsSatisfiedBy(pin, SdkVersion.Parse("8.0.203"), out var reason));
            Assert.Contains("8.0.203", reason, System.StringComparison.Ordinal);
        }

        [Fact]
        public void DifferentMajorVersionDoesNotSatisfyAPatchPin()
        {
            var pin = new GlobalJsonPin("8.0.100", "latestPatch", false);

            Assert.False(SdkPinRules.IsSatisfiedBy(pin, SdkVersion.Parse("9.0.100"), out _));
        }

        [Fact]
        public void DisableRequiresAnExactMatch()
        {
            var pin = new GlobalJsonPin("8.0.131", "disable", false);

            Assert.True(SdkPinRules.IsSatisfiedBy(pin, SdkVersion.Parse("8.0.131"), out _));
            Assert.False(SdkPinRules.IsSatisfiedBy(pin, SdkVersion.Parse("8.0.132"), out _));
        }

        [Fact]
        public void PrereleaseSdkIsRejectedUnlessAllowed()
        {
            var pin = new GlobalJsonPin("8.0.100", "latestPatch", false);

            Assert.False(SdkPinRules.IsSatisfiedBy(pin, SdkVersion.Parse("8.0.131-preview.1"), out var reason));
            Assert.Contains("prerelease", reason, System.StringComparison.Ordinal);
        }

        [Fact]
        public void UnknownRollForwardPolicyNeverSatisfiesThePin()
        {
            var pin = new GlobalJsonPin("8.0.100", "latestMajor", false);

            Assert.False(SdkPinRules.IsSatisfiedBy(pin, SdkVersion.Parse("8.0.131"), out _));
        }

        [Fact]
        public void VersionComponentsAreParsedIntoFeatureBandAndPatch()
        {
            var version = SdkVersion.Parse("8.0.131");

            Assert.Equal(8, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.FeatureBand);
            Assert.Equal(31, version.Patch);
        }

        [Fact]
        public void MalformedVersionsAreRejectedRatherThanGuessed()
        {
            Assert.Throws<ProofToolException>(() => SdkVersion.Parse("8.0"));
            Assert.Throws<ProofToolException>(() => SdkVersion.Parse("eight.zero.one"));
        }
    }
}
