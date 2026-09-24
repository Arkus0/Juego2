using System;
using System.IO;
using NUnit.Framework;
using Arkus.H1.Editor;

namespace Arkus.H1.Editor.Tests
{
    public sealed class H1HostPolicyTests
    {
        [Test]
        public void BootstrappedAuthorityResolvesPotesResourceInsideReviewedManagedRoot()
        {
            var projectRoot = H1HostPolicyProbe.ProjectRootAbsolute();
            var managedRoot = Path.GetFullPath(Path.Combine(projectRoot, H1HostPolicyProbe.ManagedRoot));
            var resolved = H1HostPolicyProbe.ResolveReviewedResource(H1HostPolicyProbe.PotesMarketResource);

            Assert.That(H1HostPolicyProbe.ProjectIdentity, Is.EqualTo("arkus.unity-project@1:ArkusUnity"));
            Assert.That(resolved, Does.StartWith(managedRoot + Path.DirectorySeparatorChar));
            Assert.That(resolved, Does.StartWith(projectRoot + Path.DirectorySeparatorChar));
        }

        [TestCase("../outside.txt")]
        [TestCase("file:///tmp/outside.txt")]
        [TestCase("https://example.invalid/outside")]
        [TestCase("System.Diagnostics.Process")]
        public void AmbientAuthorityShapedSelectorsAreNotLogicalResources(string selector)
        {
            Assert.Throws<InvalidOperationException>(() => H1HostPolicyProbe.ResolveReviewedResource(selector));
        }

        [Test]
        public void ExternalReversibleEffectStaysInsideManagedRootAndRestoresWorkspace()
        {
            var path = H1HostPolicyProbe.ResolveReviewedResource(H1HostPolicyProbe.PotesMarketResource);
            var parent = Path.GetDirectoryName(path);
            Assert.That(parent, Is.Not.Null);
            Assert.That(File.Exists(path), Is.False, "Probe target must not pre-exist in the tracked fixture.");

            H1HostPolicyProbe.ExternalReversibleWriteProbe(
                H1HostPolicyProbe.PotesMarketResource,
                "WP-H1-03 effective Unity admission probe");

            Assert.That(File.Exists(path), Is.False, "ExternalReversible probe must restore the workspace after itself.");
            Assert.That(Directory.Exists(parent!), Is.False, "Probe-created directory must be removed after the reversible effect.");
        }
    }
}
