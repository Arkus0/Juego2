using Xunit;

namespace Arkus.Harness.Tests
{
    [CollectionDefinition(Name, DisableParallelization = true)]
    public sealed class H1UnityEditorLifecycleProjectLeaseCollection
    {
        public const string Name = "H1 Unity project lease";
    }
}
