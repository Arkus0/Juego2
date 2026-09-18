using System;
using System.IO;
using System.Linq;
using System.Text.Json;

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
                        case "--root": root = args[++i]; break;
                        case "--configuration": configuration = args[++i]; break;
                        case "--phase": phase = args[++i]; break;
                        case "--inventory": inventory = args[++i]; break;
                        case "--report": report = args[++i]; break;
                        default: throw new ArgumentException("Unknown argument: " + args[i]);
                    }
                }

                if (phase != "repository" && phase != "static" && phase != "effective" && phase != "output" && phase != "all")
                {
                    throw new ArgumentException("Unsupported phase: " + phase);
                }

                var runner = new ProofRunner(root, configuration);
                var additionalFindings = LateBuildChecks.CheckCandidateTree(root);
                TrackedBytesSnapshot? trackedBeforeBuild = null;
                if (phase == "effective" || phase == "all")
                {
                    trackedBeforeBuild = LateBuildChecks.CaptureTrackedBytes(root);
                }

                if (phase == "repository" || phase == "all")
                {
                    runner.RunRepository();
                    additionalFindings += BuildSurfaceChecks.RunRepository(root);
                    additionalFindings += RepositoryClosureChecks.RunRepository(root);
                }

                if (phase == "static" || phase == "all")
                {
                    runner.RunStatic();
                    additionalFindings += AdditionalChecks.RunStatic(root, configuration);
                    additionalFindings += RepositoryClosureChecks.RunStatic(root, configuration);
                }

                if (phase == "effective" || phase == "all")
                {
                    runner.RunEffective();
                    additionalFindings += AdditionalChecks.RunEffective(root, configuration);
                    additionalFindings += NonProductEffectiveChecks.Run(root, configuration);
                    additionalFindings += CompilerInputClosureChecks.Run(root, configuration);
                    additionalFindings += LateBuildChecks.CheckEffectiveCompilerExtensions(root, configuration);
                    additionalFindings += OutputIntegrityChecks.Run(root, configuration);
                    additionalFindings += LateBuildChecks.CheckTrackedBytesStable(root, trackedBeforeBuild!);
                    additionalFindings += LateBuildChecks.CheckCandidateTree(root);
                }

                if (phase == "output")
                {
                    additionalFindings += OutputIntegrityChecks.Run(root, configuration);
                }

                var findingCount = runner.Findings.Count + additionalFindings;

                if (inventory is not null || report is not null)
                {
                    var inventoryPath = inventory ?? Path.Combine(root, "artifacts", "proof", "inventory");
                    var reportPath = report ?? Path.Combine(root, "artifacts", "proof", "report.json");
                    runner.WriteEvidence(inventoryPath, reportPath);
                    CompilerInputClosureChecks.WriteInventory(
                        root,
                        configuration,
                        Path.Combine(inventoryPath, "compiler-args.json"));

                    Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
                    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                    var evidenceReport = new
                    {
                        CandidateSha = ProcessExec.Run("git", new[] { "rev-parse", "HEAD" }, root).Stdout.Trim(),
                        Configuration = configuration,
                        RunningSdk = ProcessExec.Run("dotnet", new[] { "--version" }, root).Stdout.Trim(),
                        CoreFindingCount = runner.Findings.Count,
                        AdditionalFindingCount = additionalFindings,
                        FindingCount = findingCount,
                        CoreFindings = runner.Findings.Select(f => new { f.Id, f.Phase, f.Subject, f.Message }).ToArray(),
                    };
                    File.WriteAllText(reportPath, JsonSerializer.Serialize(evidenceReport, jsonOptions) + Environment.NewLine);
                }

                if (findingCount != 0)
                {
                    Console.Error.WriteLine($"HK00 proof FAILED with {findingCount} finding(s).");
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
