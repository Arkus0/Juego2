using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Arkus.H1.UnityHost;

namespace Arkus.Harness.H1LeaseCrashFixture
{
    public static class FixtureMarker { }

    internal static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0) return 2;
            return string.Equals(args[0], "host", StringComparison.Ordinal)
                ? RunHost(args)
                : string.Equals(args[0], "worker", StringComparison.Ordinal)
                    ? RunWorker(args)
                    : 2;
        }

        private static int RunHost(string[] args)
        {
            if (args.Length != 4) return 3;
            var invocationId = args[1];
            var readyPath = Path.GetFullPath(args[2]);
            var releasePath = Path.GetFullPath(args[3]);
            var profile = H1UnityLaunchProfile.ForCurrentHost();
            var lease = new FileH1UnityProjectLease(profile).TryAcquire();
            if (lease == null) return 4;

            new FileH1UnityInvocationLedger(profile).Record(new H1UnityInvocationRecord(
                invocationId,
                "unity.host.project-profile.inspect@1.0",
                H1UnityInvocationStatus.Running,
                "running"));

            var info = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = profile.RepositoryRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            info.ArgumentList.Add(typeof(FixtureMarker).Assembly.Location);
            info.ArgumentList.Add("worker");
            info.ArgumentList.Add(readyPath);
            info.ArgumentList.Add(releasePath);
            var worker = Process.Start(info);
            if (worker == null) return 5;

            var deadline = DateTime.UtcNow.AddSeconds(15);
            while (!File.Exists(readyPath))
            {
                if (worker.HasExited) return 6;
                if (DateTime.UtcNow >= deadline) return 7;
                Thread.Sleep(20);
            }

            // Do not dispose the lease: this intentionally models an abrupt host death. The worker
            // has already been launched with the production inheritable lease handle. Keep the host
            // copy rooted until the child has signalled that it is alive.
            GC.KeepAlive(lease);
            Environment.FailFast("intentional host crash after child launch");
            return 8;
        }

        private static int RunWorker(string[] args)
        {
            if (args.Length != 3) return 9;
            var readyPath = Path.GetFullPath(args[1]);
            var releasePath = Path.GetFullPath(args[2]);
            var readyDirectory = Path.GetDirectoryName(readyPath);
            if (!string.IsNullOrEmpty(readyDirectory)) Directory.CreateDirectory(readyDirectory);
            File.WriteAllText(readyPath, Process.GetCurrentProcess().Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
            while (!File.Exists(releasePath)) Thread.Sleep(20);
            return 0;
        }
    }
}
