using System;
using System.IO;
using System.Text;
using Arkus.H1.UnityHost;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class H1UnityEditorLifecycleCodecTests
    {
        [Fact]
        public void Product_result_codec_rejects_corrupt_main_thread_field()
        {
            var path = Path.Combine(Path.GetTempPath(), "h1-03a-corrupt-" + Guid.NewGuid().ToString("N") + ".env");
            try
            {
                File.WriteAllLines(path, new[]
                {
                    "schema=" + H1UnityLaunchProfile.ResultSchemaId,
                    "invocation=" + Encode("h1u-codec-corrupt"),
                    "capability=" + Encode("unity.host.project-profile.inspect@1.0"),
                    "executor=" + Encode(ProjectProfileInspectExecutor.WorkerExecutorId),
                    "profile=" + Encode(H1UnityLaunchProfile.ProfileId),
                    "project=" + Encode("arkus.unity-project@1:ArkusUnity"),
                    "editorVersion=" + Encode(H1UnityLaunchProfile.EditorVersion),
                    "editorRevision=" + Encode(H1UnityLaunchProfile.EditorRevision),
                    "mainThread=not-a-bool",
                    "payload=" + Encode("inspect")
                }, new UTF8Encoding(false));

                Assert.False(H1UnityEnvelopeCodec.TryReadResult(path, out var result));
                Assert.Null(result);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        private static string Encode(string value) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }
}
