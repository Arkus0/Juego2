using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Arkus.Kernel.Proof;

namespace Arkus.Kernel.Proof.Tool
{
    /// <summary>
    /// Command line entry point for the kernel boundary proof.
    /// </summary>
    /// <remarks>
    /// Exit codes are part of the contract: <c>0</c> green, <c>1</c> findings,
    /// <c>2</c> the proof could not be completed. CI treats anything non-zero as a
    /// failure, so an unusable tool can never be mistaken for a passing claim.
    /// </remarks>
    public static class Program
    {
        /// <summary>Runs the proof.</summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Process exit code.</returns>
        public static int Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            try
            {
                return Run(args);
            }
            catch (ProofToolException ex)
            {
                Console.Error.WriteLine("PROOF INCONCLUSIVE: " + ex.Message);
                if (ex.InnerException is not null)
                {
                    Console.Error.WriteLine("  caused by: " + ex.InnerException.Message);
                }

                return 2;
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("PROOF INCONCLUSIVE (io): " + ex.Message);
                return 2;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine("PROOF INCONCLUSIVE (access): " + ex.Message);
                return 2;
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine("PROOF INCONCLUSIVE (json): " + ex.Message);
                return 2;
            }
        }

        private static int Run(IReadOnlyList<string> args)
        {
            var phase = "all";
            var repositoryRoot = Directory.GetCurrentDirectory();
            var configuration = "Debug";
            var reportPath = string.Empty;
            var inventoryPath = string.Empty;
            var expected = new List<string>();
            var expectedAbsent = new List<string>();

            for (var i = 0; i < args.Count; i++)
            {
                var argument = args[i];
                switch (argument)
                {
                    case "--phase":
                        phase = Next(args, ref i, "--phase");
                        break;
                    case "--repository-root":
                        repositoryRoot = Next(args, ref i, "--repository-root");
                        break;
                    case "--configuration":
                        configuration = Next(args, ref i, "--configuration");
                        break;
                    case "--report":
                        reportPath = Next(args, ref i, "--report");
                        break;
                    case "--inventory":
                        inventoryPath = Next(args, ref i, "--inventory");
                        break;
                    case "--expect":
                        expected.Add(Next(args, ref i, "--expect"));
                        break;
                    case "--expect-absent":
                        expectedAbsent.Add(Next(args, ref i, "--expect-absent"));
                        break;
                    case "--help":
                        PrintUsage();
                        return 0;
                    default:
                        throw new ProofToolException($"Unknown argument '{argument}'.");
                }
            }

            var runStatic = phase is "static" or "effective" or "compiler" or "all";
            var runEffective = phase is "effective" or "all";
            var runCompiler = phase is "compiler" or "all";

            if (phase is not ("preflight" or "static" or "effective" or "compiler" or "all"))
            {
                throw new ProofToolException($"Unknown phase '{phase}'.");
            }

            if (inventoryPath.Length > 0 && phase != "all")
            {
                throw new ProofToolException("Inventories may only be generated from a complete run (--phase all).");
            }

            var runner = new KernelProofRunner(repositoryRoot, configuration);
            var report = runner.Run(runStatic, runEffective, runCompiler);

            foreach (var finding in report.Findings)
            {
                Console.Error.WriteLine(
                    FormattableString.Invariant($"{finding.CheckId} [{finding.Phase}] {finding.Subject}: {finding.Message}"));
            }

            if (reportPath.Length > 0)
            {
                report.WriteJson(Path.GetFullPath(reportPath));
            }

            if (inventoryPath.Length > 0 && report.IsGreen)
            {
                runner.BuildInventory().Write(Path.GetFullPath(inventoryPath));
            }

            var phaseSummary = string.Join("+", report.Phases);
            Console.Out.WriteLine(
                FormattableString.Invariant(
                    $"phases={phaseSummary} configuration={configuration} findings={report.Findings.Count}"));

            if (expected.Count > 0 || expectedAbsent.Count > 0)
            {
                return EvaluateExpectations(report, expected, expectedAbsent);
            }

            if (!report.IsGreen)
            {
                Console.Out.WriteLine("PROOF RESULT: RED");
                return 1;
            }

            Console.Out.WriteLine("PROOF RESULT: GREEN");
            return 0;
        }

        /// <summary>
        /// Judges a negative control: a self-attack passes only when the intended
        /// check fires, and when checks that must stay quiet stayed quiet.
        /// </summary>
        private static int EvaluateExpectations(
            ProofReport report,
            IReadOnlyList<string> expected,
            IReadOnlyList<string> expectedAbsent)
        {
            var fired = new HashSet<string>(StringComparer.Ordinal);
            foreach (var finding in report.Findings)
            {
                fired.Add(finding.CheckId);
            }

            var satisfied = true;

            foreach (var checkId in expected)
            {
                if (fired.Contains(checkId))
                {
                    Console.Out.WriteLine("EXPECTED CHECK FIRED: " + checkId);
                }
                else
                {
                    Console.Out.WriteLine("EXPECTED CHECK DID NOT FIRE: " + checkId);
                    satisfied = false;
                }
            }

            foreach (var checkId in expectedAbsent)
            {
                if (fired.Contains(checkId))
                {
                    Console.Out.WriteLine("CHECK FIRED BUT WAS REQUIRED TO STAY QUIET: " + checkId);
                    satisfied = false;
                }
                else
                {
                    Console.Out.WriteLine("EXPECTED QUIET CHECK STAYED QUIET: " + checkId);
                }
            }

            if (!satisfied)
            {
                Console.Out.WriteLine("EXPECTATION RESULT: NOT MET");
                return 1;
            }

            Console.Out.WriteLine("EXPECTATION RESULT: MET");
            return 0;
        }

        private static string Next(IReadOnlyList<string> args, ref int index, string name)
        {
            index++;
            if (index >= args.Count)
            {
                throw new ProofToolException($"Missing value for {name}.");
            }

            return args[index];
        }

        private static void PrintUsage()
        {
            Console.Out.WriteLine("Arkus.Kernel.Proof.Tool");
            Console.Out.WriteLine("  --phase <preflight|static|effective|compiler|all>");
            Console.Out.WriteLine("                                             which checks to run (default: all)");
            Console.Out.WriteLine("  --repository-root <path>                   repository root (default: cwd)");
            Console.Out.WriteLine("  --configuration <name>                     build configuration (default: Debug)");
            Console.Out.WriteLine("  --report <file>                            write the JSON report");
            Console.Out.WriteLine("  --inventory <directory>                    write generated inventories");
            Console.Out.WriteLine("  --expect <check-id>                        require this check to fire (repeatable)");
            Console.Out.WriteLine("  --expect-absent <check-id>                 require this check to stay quiet (repeatable)");
        }
    }
}
