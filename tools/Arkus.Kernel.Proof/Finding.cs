using System;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// One mechanical check failure.
    /// </summary>
    public sealed class Finding
    {
        /// <summary>Creates a finding.</summary>
        /// <param name="checkId">Stable check identifier from <see cref="CheckIds"/>.</param>
        /// <param name="phase">Proof phase that produced the finding.</param>
        /// <param name="subject">Repository-relative subject (project, file, edge).</param>
        /// <param name="message">Human-readable explanation.</param>
        public Finding(string checkId, string phase, string subject, string message)
        {
            CheckId = checkId ?? throw new ArgumentNullException(nameof(checkId));
            Phase = phase ?? throw new ArgumentNullException(nameof(phase));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>Stable check identifier.</summary>
        public string CheckId { get; }

        /// <summary>Proof phase that produced the finding.</summary>
        public string Phase { get; }

        /// <summary>Repository-relative subject.</summary>
        public string Subject { get; }

        /// <summary>Human-readable explanation.</summary>
        public string Message { get; }
    }
}
