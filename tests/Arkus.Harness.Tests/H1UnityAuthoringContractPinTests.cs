using System;
using System.Linq;
using Arkus.EngineBridge.UnityAuthoring;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityAuthoringContractPinTests
    {
        [Fact]
        public void VersionOnePublicIdentityCannotDriftWithoutExplicitVersionChange()
        {
            Assert.Equal("arkus.unity-binding@1", UnityBindingProducer.BindingSchemaId);
            Assert.Equal("arkus.unity-binding-result@1", UnityBindingProducer.ResultSchemaId);
            Assert.Equal("arkus.unity-binding", UnityBindingProducer.ExtensionOwner);
            Assert.Equal(1, UnityBindingProducer.ExtensionSchemaVersion);
            Assert.Equal("arkus.unity-authoring", UnityAuthoringProvider.ProviderId);
            Assert.Equal("unity.binding", UnityAuthoringProvider.CapabilityNamespace);
            Assert.Equal("1.0", UnityAuthoringProvider.ContractVersionText);
        }

        [Fact]
        public void PortableProviderPublicSurfaceContainsNoUnityRuntimeOrEditorTypes()
        {
            var assembly = typeof(UnityAuthoringProvider).Assembly;
            Assert.DoesNotContain(
                assembly.GetReferencedAssemblies(),
                reference =>
                    (reference.Name ?? string.Empty).StartsWith("UnityEngine", StringComparison.Ordinal) ||
                    (reference.Name ?? string.Empty).StartsWith("UnityEditor", StringComparison.Ordinal));
            Assert.DoesNotContain(
                assembly.GetExportedTypes(),
                type =>
                    (type.FullName ?? string.Empty).Contains("UnityEngine", StringComparison.Ordinal) ||
                    (type.FullName ?? string.Empty).Contains("UnityEditor", StringComparison.Ordinal));
        }
    }
}
