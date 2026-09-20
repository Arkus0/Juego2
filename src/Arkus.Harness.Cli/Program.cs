using System;

namespace Arkus.Harness.Cli
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
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
