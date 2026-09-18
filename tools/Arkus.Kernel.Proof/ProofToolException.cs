using System;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// Raised when the proof tool cannot complete a check at all.
    /// </summary>
    /// <remarks>
    /// The harness must fail closed: an inconclusive proof run is a failure, never
    /// a pass. Callers map this to a distinct non-zero exit code so an unusable
    /// tool can never be mistaken for a green result.
    /// </remarks>
    public sealed class ProofToolException : Exception
    {
        /// <summary>Creates the exception.</summary>
        public ProofToolException()
        {
        }

        /// <summary>Creates the exception with a message.</summary>
        /// <param name="message">Explanation.</param>
        public ProofToolException(string message)
            : base(message)
        {
        }

        /// <summary>Creates the exception with a message and inner cause.</summary>
        /// <param name="message">Explanation.</param>
        /// <param name="innerException">Cause.</param>
        public ProofToolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
