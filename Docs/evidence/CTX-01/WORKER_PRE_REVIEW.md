# CTX-01 — Worker strict pre-review (repair cycle 1)

`WORKER_PRE_REVIEW: CLEAN`  
`fail_cycle: 1`  
Failed candidate: `d972db98eca5527e7c30596ea069866dd878069b`  
Independent FAIL: `#5271197524`  
Revert: PR `#109`  
Repair baseline: `a85954539e0ef397009e87af322eb735d58ccc0d`  
Repair branch: `repair/ctx-01-freshness-anchor`

## Disposition / single implementation path

`SUPERSEDE_WITH_EXPLICIT_MIGRATION`.

The original canonical PR #106 was merged and then fully reverted after the post-merge DocSync transition exposed the blocker. It is closed and cannot be repaired in place. PR #109 restored the accepted tree. This repair branch is the sole active CTX-01 implementation lineage and explicitly supersedes the reverted bytes; there is no second simultaneously active implementation PR.

## Blocker replay

The old rule required `generated_from_main_sha == current main SHA` while storing that value inside the commit that defines current main. Persistence changes the SHA, so the rule is unsatisfiable.

The repair changes only that causal boundary to `projection_phase == DOCSYNC_PERSISTED AND generated_from_main_sha == first_parent(current live main)`. The index anchors the accepted source state before persistence. Candidate phase is never live-fresh. Any later main advance changes the first parent and makes the projection stale.

## Complete-diff challenge

The repaired candidate reconstructs the reverted CTX-01 role/profile/router changes and modifies only the freshness surfaces plus the exact WP/evidence needed for this fail cycle. The unrelated accepted PR #108 file is explicitly preserved from current main.

No product/runtime code, tests, Unity content, accepted predecessor contract, CTX-02 capsule mechanism, CTX-03 evidence migration, or future H2 planning semantics are authored here.

## Acceptance challenge

- source-anchor validation and live freshness are separate, so Worker pre-review cannot falsely claim post-DocSync freshness;
- a candidate projection always reports STALE for live use;
- a post-PASS DocSync projection can be fresh without self-reference;
- a subsequent main commit deterministically stales it;
- live GitHub still owns mutable PR/branch/review/check state;
- semantic/proof authority remains exact contracts/code/tests/accepted evidence;
- Reviewer independence and predecessor reconstruction remain unchanged;
- DocSync must regenerate if main moves before persistence;
- missing/malformed/contradictory compact context remains fail-closed.

## Full rerun

`Docs/evidence/CTX-01/FINAL_RERUN.md` records the complete eight-control rerun. The repaired checker self-test executed GREEN. `Docs/evidence/CTX-01/DRY_RUNS.md` records representative role routing plus the causal freshness positive/negative controls.

## Proof budget

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`

The repair does not redesign CTX-01. It replaces one impossible freshness equality with a first-parent source anchor and adds the minimum phase bit needed to distinguish a candidate from a persisted DocSync projection.

## Worker result

`WORKER_PRE_REVIEW: CLEAN`

After this report is committed, the Worker must bind the resulting exact branch HEAD externally in the PR handoff and freeze that SHA. Any repository-byte mutation after this report invalidates CLEAN and requires a new complete pre-review.
