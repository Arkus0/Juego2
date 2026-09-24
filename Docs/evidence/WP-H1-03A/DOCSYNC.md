# WP-H1-03A — Post-acceptance DocSync

DOCSYNC_STATUS: **DOCSYNC_COMPLETE**  
MODE: **PROCESS_ONLY / DOCUMENTATION_ONLY**  
DATE: 2026-09-24

## Accepted result

- Frozen candidate SHA: `24d24526487af0d32e69799960f4251e44f0b5ad`
- Canonical PR: `#176`
- Final independent PASS review: `#5303016012`
- Implementation merge: `90428b803948820663abfebaa3fe21eb37596247`
- Exact-SHA Candidate Validation: Actions run `35985858123` GREEN
- H1-03A Unity Lifecycle on the frozen SHA: Actions run `35985225199` GREEN
- Arkus Main Safety on the frozen SHA: Actions run `35985225228` GREEN
- Effective Unity evidence: the exact pinned H1-02 editor on the accepted GitHub-hosted substrate exercised real short-lived Editor workers, main-thread execution, normalized reference↔MCP equality, lifecycle failure mapping and same-project lease contention/restart behavior.

## Accepted claim

H1-03A establishes the single accepted public Arkus-host → short-lived Unity batch-worker topology beneath the admitted H1 host policy. Reference and MCP enter the same canonical composed handler; only that handler creates an internal Editor invocation, while Unity remains a typed executor/evidence boundary rather than canonical session or public-registry authority.

The accepted lifecycle distinguishes pre-launch cancellation from post-launch interruption/indeterminate outcomes, records bounded restart-recoverable invocation status, keeps Unity diagnostics out of protocol framing, rejects wrong/corrupt result envelopes and mechanically reconciles admitted Editor-bound handlers with provider-owned worker executors.

The repaired lease invariant is part of the accepted claim: the same-project exclusive lease is preserved for the already-launched worker lifetime even if the .NET host dies. A restarted host therefore returns structured `unity.lifecycle.busy-project` before any second launch while the inherited worker lease remains live, and may only move the abandoned `Running` record to `Indeterminate` once the worker has exited and the real lease is acquirable.

## DocSync actions

1. Marked `WP-H1-03A` COMPLETE / ACCEPTED and recorded the exact candidate, independent PASS review, implementation merge and exact-SHA validation runs.
2. Updated the H1 track so `H1-00`, `H1-01`, `H1-02`, `H1-03` and `H1-03A` are accepted predecessor truth.
3. Advanced the default H1 sequence to `WP-H1-04 — Unity catalogue + identity resolution` (`LOCAL_UNITY_REQUIRED / SOURCE_DEPENDENT`), now dependency-valid but still `NOT_STARTED` until explicitly started.
4. Removed the stale statement that H1-04 is blocked on future H1-03A acceptance.
5. Preserved H1-04 ownership of effective catalogue/logical-native identity and first Quaternius Source adoption, plus all later materialization/reconciliation boundaries.

## Boundary

This DocSync changes documentation state only. It does not modify the accepted H1-03A implementation, rerun Unity, broaden the public invocation topology, implement catalogue/materialization behavior, adopt Quaternius Source bytes, authorize H1-04 automatically, alter H0/H1-03 semantics, or change CTX/DW/CITY/PA ownership.

## Next action

Next default H1 workpack: `WP-H1-04 — Unity catalogue + identity resolution` (`LOCAL_UNITY_REQUIRED / SOURCE_DEPENDENT`).

It is dependency-valid from accepted H1-03A but remains `NOT_STARTED` until a human explicitly starts its Worker.

`DOCSYNC_COMPLETE`
