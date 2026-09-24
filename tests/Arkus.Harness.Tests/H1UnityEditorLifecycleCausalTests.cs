using Arkus.H1.UnityHost;
using Xunit;

namespace Arkus.Harness.Tests
{
    [Collection(H1UnityEditorLifecycleProjectLeaseCollection.Name)]
    public sealed class H1UnityEditorLifecycleCausalTests
    {
        [Fact]
        public void File_project_lease_serializes_independent_same_project_hosts()
        {
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var first = new FileH1UnityProjectLease(profile);
            var second = new FileH1UnityProjectLease(profile);

            var held = first.TryAcquire();
            Assert.NotNull(held);
            try
            {
                Assert.Null(second.TryAcquire());
            }
            finally
            {
                held!.Dispose();
            }

            using var reacquired = second.TryAcquire();
            Assert.NotNull(reacquired);
        }
    }
}
