using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// The outcome of a proof run.
    /// </summary>
    public sealed class ProofReport
    {
        /// <summary>Creates a report.</summary>
        /// <param name="phases">Phases that were executed.</param>
        /// <param name="findings">Findings, in discovery order.</param>
        /// <param name="environmentInfo">Environment facts that must never be drift-checked.</param>
        public ProofReport(
            IReadOnlyList<string> phases,
            IReadOnlyList<Finding> findings,
            IReadOnlyDictionary<string, string> environmentInfo)
        {
            Phases = phases ?? throw new ArgumentNullException(nameof(phases));
            Findings = findings ?? throw new ArgumentNullException(nameof(findings));
            EnvironmentInfo = environmentInfo ?? throw new ArgumentNullException(nameof(environmentInfo));
        }

        /// <summary>Phases that were executed.</summary>
        public IReadOnlyList<string> Phases { get; }

        /// <summary>Findings, in discovery order.</summary>
        public IReadOnlyList<Finding> Findings { get; }

        /// <summary>Environment facts (SDK version, configuration, repository root).</summary>
        public IReadOnlyDictionary<string, string> EnvironmentInfo { get; }

        /// <summary>Whether the run produced no findings.</summary>
        [JsonIgnore]
        public bool IsGreen => Findings.Count == 0;

        /// <summary>Writes the report as JSON.</summary>
        /// <param name="path">Absolute output path.</param>
        public void WriteJson(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            };

            File.WriteAllText(path, JsonSerializer.Serialize(this, options) + Environment.NewLine);
        }
    }
}
