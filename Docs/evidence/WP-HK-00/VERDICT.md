# WP-HK-00 Worker foundational verdict

Candidate state: **ACTIVE / NOT READY FOR REVIEW**

`FOUNDATIONAL_PROOF_VERDICT: NOT_READY`  
`UNRESOLVED_PROOF_OBLIGATIONS: 1`  
`KNOWN_UNDETECTED_DEFECT_CLASSES: 0`

## Why NOT_READY

The current architecture/proof-boundary pre-review is design-clean after causal repairs, but the Definition of Done requires executed exact-SHA CI. GitHub-hosted Actions is currently failing before runner allocation (`runner_id=0`, `steps=[]`), including the main-owned Worker Handoff job, so the Worker cannot legitimately claim the final proof or 37 negative controls executed on the current lineage.

This is external infrastructure backpressure, not a contractual implementation PASS or FAIL. The PR must remain Draft + ACTIVE.

## Required transition to READY

- GitHub-hosted runner becomes available.
- Positive read-only proof executes on exact candidate SHA and reaches GREEN.
- All 37 causal attacks execute RED for intended oracle → pristine reconstruction → GREEN.
- Observed package lock/inventories/compiler args/attack results are reconciled to committed evidence.
- A final evidence candidate reruns exact-SHA CI with zero drift.
- Worker performs final no-write adversarial pre-review and records `WORKER_PRE_REVIEW: CLEAN` in the PR handoff/evidence link.
- Only then stop writers, record exact Frozen candidate SHA, mark FROZEN_FOR_REVIEW and Ready.

Fresh independent Reviewer PASS remains mandatory after freeze.
