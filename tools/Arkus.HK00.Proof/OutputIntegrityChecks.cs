using System;
using System.IO;

namespace Arkus.HK00.Proof
{
    internal static class OutputIntegrityChecks
    {
        public static int Run(string root, string configuration)
        {
            var findings = 0;
            var probe = new MsBuildProbe(root, configuration);

            foreach (var spec in FixedContract.Projects)
            {
                ProjectFacts facts;
                try
                {
                    facts = probe.Evaluate(spec);
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-OUTPUT-ORACLE", "output", spec.Name, ex.Message);
                    continue;
                }

                var targetPath = facts.Property("TargetPath");
                if (string.IsNullOrWhiteSpace(targetPath) || !File.Exists(targetPath))
                {
                    findings += Report("HK00-ASSEMBLY-MISSING", "output", spec.Name, "Expected output is absent: " + targetPath);
                    continue;
                }

                AssemblyFacts assembly;
                try
                {
                    assembly = AssemblyFacts.Read(targetPath);
                }
                catch (Exception ex)
                {
                    findings += Report("HK00-OUTPUT-ORACLE", "output", spec.Name, ex.Message);
                    continue;
                }

                if (!assembly.PdbIdentityMatchesPe)
                {
                    findings += Report(
                        "HK00-PDB-PE-MISMATCH",
                        "output",
                        spec.Name,
                        "Portable PDB identity does not match the CodeView identity embedded in the produced PE.");
                }

                if (assembly.AssemblyFileCount != 0)
                {
                    findings += Report(
                        "HK00-ASSEMBLY-SECONDARY-FILE",
                        "output",
                        spec.Name,
                        "Multi-file assembly/netmodule inputs are outside the HK00 compiler-input model.");
                }

                if (assembly.ManifestResourceCount != 0 || assembly.NativeResourceSize != 0)
                {
                    findings += Report(
                        "HK00-ASSEMBLY-RESOURCE",
                        "output",
                        spec.Name,
                        "Embedded managed/native resource inputs are outside the HK00 portable-kernel build surface.");
                }
            }

            return findings;
        }

        private static int Report(string id, string phase, string subject, string message)
        {
            Console.Error.WriteLine(id + " [" + phase + "] " + subject + ": " + message);
            return 1;
        }
    }
}
