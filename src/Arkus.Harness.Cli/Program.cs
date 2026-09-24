using System;

namespace Arkus.Harness.Cli
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                // HK09A deliberately exposes no caller-controlled filesystem authority in H0.
                // The legacy HK07A --file framing convenience remains an internal parser detail,
                // but the production executable rejects it before ReferenceTransportHost can open
                // any path. Accepted H0 workflows remain available over stdin/stdout.
                for (var index = 0; index < args.Length; index++)
                {
                    if (string.Equals(args[index], "--file", StringComparison.Ordinal))
                    {
                        Console.Error.WriteLine(
                            "arkus-host usage: --file is not exposed by the H0 production host; send the request over stdin");
                        return ReferenceTransportHost.UsageFailureExitCode;
                    }
                }

                if (args.Length == 1 && string.Equals(args[0], "--h1-unity", StringComparison.Ordinal))
                {
                    return H1ReferenceTransportHost.Run(
                        Console.OpenStandardInput(),
                        Console.OpenStandardOutput(),
                        Console.Error);
                }

                return ReferenceTransportHost.Run(
                    args,
                    Console.OpenStandardInput(),
                    Console.OpenStandardOutput(),
                    Console.Error);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("arkus-host fatal: " + exception.GetType().Name);
                return ReferenceTransportHost.SoftwareFailureExitCode;
            }
        }
    }
}
