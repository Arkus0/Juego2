using System;

namespace Arkus.H1.UnityHost
{
    /// <summary>
    /// Reclassifies a persisted Running record only when this host can acquire the reviewed
    /// project-operation lease. A live operation keeps that lease, so status queries cannot turn
    /// an actually running invocation into an indeterminate one. If the previous host died, the
    /// lease is released by the OS and a lingering Running record has no trustworthy accepted
    /// result, so Indeterminate is the conservative restart truth.
    /// </summary>
    public sealed class RestartRecoveringH1UnityInvocationLedger : IH1UnityInvocationLedger
    {
        private readonly IH1UnityInvocationLedger _inner;
        private readonly IH1UnityProjectLease _projectLease;

        public RestartRecoveringH1UnityInvocationLedger(
            IH1UnityInvocationLedger inner,
            IH1UnityProjectLease projectLease)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _projectLease = projectLease ?? throw new ArgumentNullException(nameof(projectLease));
        }

        public void Record(H1UnityInvocationRecord record) => _inner.Record(record);

        public bool TryRead(string invocationId, out H1UnityInvocationRecord? record)
        {
            if (!_inner.TryRead(invocationId, out record) || record == null) return false;
            if (record.Status != H1UnityInvocationStatus.Running) return true;

            var recoveryLease = _projectLease.TryAcquire();
            if (recoveryLease == null) return true;

            using (recoveryLease)
            {
                // Re-read under the lease. The original owner may have completed between the first
                // read and our acquisition, in which case its final lifecycle state wins.
                if (!_inner.TryRead(invocationId, out var latest) || latest == null)
                {
                    record = null;
                    return false;
                }

                if (latest.Status == H1UnityInvocationStatus.Running)
                {
                    latest = new H1UnityInvocationRecord(
                        latest.InvocationId,
                        latest.Capability,
                        H1UnityInvocationStatus.Indeterminate,
                        H1UnityEditorExecutionCoordinator.IndeterminateCode);
                    _inner.Record(latest);
                }

                record = latest;
                return true;
            }
        }
    }
}
