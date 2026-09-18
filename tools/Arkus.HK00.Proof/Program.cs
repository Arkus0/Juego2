using System;
using System.IO;

namespace Arkus.HK00.Proof
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var root = Directory.GetCurrentDirectory();
                var configuration = "Release";
                var phase = "all";
                string? inventory = null;
                string? report = null;

                for (var i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--root":
                            root = args[++i];
                            break;
                        case "--configuration":
                            configuration = args[++i];
                            break;
                        case "--phase":
                            phase = args[++i];
                            break;
                        case "--inventory":
                            inventory = args[++i];
                            break;
                        case "--report":
                            report = args[++i];
                            break;
                        default:
                            throw new ArgumentException("Unknown argument: " + args[i]);
                    }
                }

                var runner = new ProofRunner(root, configuration);
                if (phase == "repository" || phase == "all")
                {
                    runner.RunRepository();
                }
                if (phase == "static" || phase == "all")
                {
                    runner.RunStatic();
                }
                if (phase == "effective" || phase == "all")
                {
                    runner.RunEffective();
                }
                if (phase != "repository" && phase != "static" && phase != "effective" && phase != "all")
                {
                    throw new ArgumentException("Unsupported phase: " + phase);
                }

                if (inventory is not null || report is not null)
                {
                    var inventoryDirectory = inventory ?? Path.Combine(root, "artifacts", "proof", "inventory");
                    var reportPath = report ?? Path.Combine(root, "artifacts", "proof", "report.json");
                    runner.WriteEvidence(inventoryDirectory, reportPath);
                }

                if (runner.Findings.Count != 0)
                {
                    Console.Error.WriteLine($"HK00 proof FAILED with {runner.Findings.Count} finding(s).");
                    return 1;
                }

                Console.WriteLine("HK00 proof GREEN");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("HK00-PROOF-UNAVAILABLE: " + ex);
                return 2;
            }
        }
    }
}
