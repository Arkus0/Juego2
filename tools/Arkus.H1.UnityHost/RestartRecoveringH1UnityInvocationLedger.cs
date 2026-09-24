using System;

namespace Arkus.H1.UnityHost
{
    /// <summary>
    /// Reclassifies a persisted Running record only when this host can acquire the reviewed
    /// project-operation lease. The launch lease is inherited by the Unity child process, so a
    /// host crash cannot release exclusion while that previously launched worker remains alive.
    /// Once the worker has exited, a restarted host can acquire the lease and conservatively turn
    /// a still-Running record into Indeterminate. Final states win because the ledger is re-read
    /// under the lease before any recovery write.
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
