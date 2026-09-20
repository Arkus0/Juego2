# WP-HK-10 bounded-session endurance evidence

The HK10 endurance case is deliberately bounded by the accepted HK09B H0 envelope. It is evidence of behaviour inside that envelope, not a production latency, memory or throughput SLO.

## Executed shape

`Hk10EnduranceCompatibilityTests.BoundedLongAuthoringSessionExercisesInspectValidateMutateJournalSnapshotAndRestartRecovery` performs 512 canonical authoring transactions. Every 64 transactions it exercises inspection, current-state validation, provenance journal read and snapshot export. The final snapshot is imported into a fresh process-local session and the resulting revision/hash must match the source session.

Accepted envelope comparisons:

- transactions exercised: `512` < HK09B session ceiling `10,000`;
- snapshot imports exercised for restart/recovery: `1` < receipt ceiling `1,024`;
- exported canonical state is asserted at runtime to remain below the HK09B `655,360` byte world/snapshot ceiling;
- every mutation uses the accepted canonical dispatch/transaction/provenance path.

## Exact observed run

Draft exact-SHA observation `35524945085` on candidate `04b77e0c792d21e2372255011c6a7c15daec7b11` completed GREEN and emitted:

`HK10_ENDURANCE transactions=512 journalBytes=491028 workingSetBefore=91475968 workingSetAfter=178704384 maximumWorkingSet=178667520 managedBefore=2240376 managedAfter=7114160 maximumManaged=75820368`

Interpretation is intentionally narrow. The process/GC figures are observations from one GitHub-hosted run and are recorded so growth is visible; they are not hard limits and do not establish a leak/no-leak theorem. HK09B already states that hard scheduler/memory preemption and shipping SLOs are outside H0. The material HK10 claim is that the bounded session remains functionally correct and within the accepted state/session resource envelope, with journal/session growth explicitly observed instead of assumed away.

The final frozen exact-SHA verifier reruns this executable case; telemetry can vary by runner while the bounded semantic/resource assertions remain fixed.

ENDURANCE_VERDICT: PASS
BOUNDED_SESSION_TRANSACTIONS: 512
SESSION_TRANSACTION_LIMIT: 10000
SNAPSHOT_IMPORTS_EXERCISED: 1
SNAPSHOT_IMPORT_RECEIPT_LIMIT: 1024
