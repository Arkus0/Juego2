using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Arkus.H1.UnityHost;

namespace Arkus.H1.UnityLeaseFixture
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0) return 64;
            return args[0] switch
            {
                "host" => RunHost(args),
                "worker" => RunWorker(args),
                _ => 64
            };
        }

        private static int RunHost(string[] args)
        {
            if (args.Length != 6) return 64;
            var invocationId = args[1];
            var workerReadyPath = Path.GetFullPath(args[2]);
            var releasePath = Path.GetFullPath(args[3]);
            var workerExitedPath = Path.GetFullPath(args[4]);
            var hostReadyPath = Path.GetFullPath(args[5]);
            var profile = H1UnityLaunchProfile.ForCurrentHost();

            var ledger = new FileH1UnityInvocationLedger(profile);
            ledger.Record(new H1UnityInvocationRecord(
                invocationId,
                ProjectProfileInspectExecutor.Key.ToString(),
                H1UnityInvocationStatus.Running,
                "running"));

            using var lease = new FileH1UnityProjectLease(profile).TryAcquire();
            if (lease == null) return 65;

            using var worker = new Process();
            worker.StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = profile.RepositoryRoot,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            worker.StartInfo.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
            worker.StartInfo.ArgumentList.Add("worker");
            worker.StartInfo.ArgumentList.Add(workerReadyPath);
            worker.StartInfo.ArgumentList.Add(releasePath);
            worker.StartInfo.ArgumentList.Add(workerExitedPath);
            if (!worker.Start()) return 66;

            WaitForWorkerReady(worker, workerReadyPath);
            File.WriteAllText(hostReadyPath, "ready");
            Thread.Sleep(Timeout.Infinite);
            return 0;
        }

        private static int RunWorker(string[] args)
        {
            if (args.Length != 4) return 64;
            var workerReadyPath = Path.GetFullPath(args[1]);
            var releasePath = Path.GetFullPath(args[2]);
            var workerExitedPath = Path.GetFullPath(args[3]);

            File.WriteAllText(workerReadyPath, "ready");
            while (File.Exists(releasePath)) Thread.Sleep(10);
            File.WriteAllText(workerExitedPath, "exited");
            return 0;
        }

        private static void WaitForWorkerReady(Process worker, string workerReadyPath)
        {
            var deadline = Stopwatch.StartNew();
            while (!File.Exists(workerReadyPath))
            {
                if (worker.HasExited) throw new InvalidOperationException("Lease fixture worker exited before becoming ready.");
                if (deadline.ElapsedMilliseconds > 10000) throw new TimeoutException("Lease fixture worker did not become ready.");
                Thread.Sleep(10);
            }
        }
    }
}
