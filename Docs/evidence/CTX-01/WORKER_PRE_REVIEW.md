# CTX-01 — Worker strict pre-review

`WORKER_PRE_REVIEW: CLEAN`  
`WORKER_PRE_REVIEW_FINDINGS_FIXED: 4`  
Historical baseline: `02016ba5a3d0a344525835652bbd84c9a2e9cc49`  
Candidate immediately before this report commit: `dfa4d5b56b8a633b28669ac80e32f248c09a228d`  
Canonical PR: `#106`  
fail_cycle: `0`

## Main / predecessor reconstruction

- Direct accepted predecessor remains the CTX programme plan: frozen candidate `c9ff3605048e05fded4d58a1de27450661d9fe0e`, independent PASS review `#5270686380`, plan merge `f3c8362b3d76fd4f78107d8142e07e476985f973`, DocSync merge `609ac464e0b5aed2da0a369b6f2d368f72dd35a3`.
- Historical CTX-01 baseline remains `02016ba5a3d0a344525835652bbd84c9a2e9cc49`.
- Required PROCESS_ONLY hotfix PR #107 was explicitly reconciled into the CTX-01 branch; its accepted behavior (no synthetic `EXECUTION_RECEIPT_V1` / GREEN execution claim when no canonical product/runtime command runs) is consumed, not redesigned.
- Live `main` was reconstructed again before this pre-review and had advanced to `d0566de5f0776f8cbe926aba8ec949143ae6a145` through a later unrelated PROCESS_ONLY commit whose only changed file is outside CTX-01. Per explicit human scope, that later future-planning content was not merged or imported into CTX-01. Its changed-file set does not overlap the candidate. The accepted-state index was regenerated against that live main SHA for the H0/H1/CITY/PA/CTX surfaces already owned by CTX-01.
- PR #106 remains Draft and mergeable during this pre-review.

The persisted `PREDECESSOR_CONTRACT_CHECK` remains factually valid after this reconstruction.

## Claim / trust boundary

CTX-01 owns only role-specific initial-context selection, question-specific authority ordering, stale detection, fail-closed escalation, accepted-state navigation, role-profile routing, session-handoff routing and DocSync refresh instructions.

It does not own semantic compression of accepted predecessor contracts (CTX-02), structured evidence/history migration or measurement baselines (CTX-03), product/runtime semantics, Automation V2 redesign, later-phase planning/implementation, or additional residual work.

Derived navigation is trusted only as a pointer when its explicit freshness/authority conditions hold. Live GitHub is authoritative for mutable PR/branch/review/check state; exact contracts/code/tests/accepted evidence remain authoritative for semantics and proof.

## Complete candidate diff review

The complete current PR diff contains 19 CTX-01 files only:

- `AGENTS.md`;
- seven `.agents/skills/**` role helpers;
- three `.opencode/agents/**` profiles;
- `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`;
- `Docs/engineering/context-bootstrap-profiles.json`;
- `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md`;
- `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json`;
- `scripts/context-bootstrap-check.py`;
- four `Docs/evidence/CTX-01/**` evidence files including this report after commit.

No Automation V2/workflow/product/runtime/H1 implementation/CITY implementation/PA research bytes are introduced by the PR diff. PR #107's Automation V2 bytes are base state, not CTX-01 changes.

## Acceptance / controls challenged

The final rerun re-exercised the required representative cases:

1. Worker PA-04: non-foundational minimum pack, no unrelated proof/ROADMAP default load.
2. Worker H1-02: foundational + H1 remote/local overlay load is mandatory.
3. exact-WP Reviewer: independent authoritative predecessor/frozen evidence remains mandatory; Worker/index prose is not proof.
4. blocked CITY-04: observable `ESCALATE:CROSS_TRACK_DEPENDENCY` occurs before `BLOCKED` on missing H1-08.

All eight required process controls are rechecked in `Docs/evidence/CTX-01/FINAL_RERUN.md`: foundational conditionality, cross-track escalation, stale compact-state rejection, index non-authority, Reviewer independence, narrow H1 local executor, exact main-SHA stale detection and missing-context fail-closed behavior.

## Findings fixed during Worker pre-review

### F1 — transient PR state was initially represented in the derived index

That state can change without a `main` commit, so `generated_from_main_sha` cannot prove it fresh. Fixed causally: the index now contains main-derived accepted-state hints only; Draft/Ready/HEAD/check/review/ownership state is always queried live.

### F2 — OpenCode Worker wording accidentally weakened exact-SHA evidence

An intermediate edit said exact-SHA only “when the claim requires it”. That could be read as weakening existing Worker rules. Restored the prior strict rule: `produce evidencia reproducible y exact-SHA`.

### F3 — OpenCode/AGENTS routing surface was incomplete

The `.agents/skills` routing was not sufficient because `.opencode/agents/**` and `AGENTS.md` are direct role surfaces. Added minimal routing references without changing permissions, role authority or proof duties.

### F4 — CTX-01 adoption timing in AGENTS was ambiguous

Intermediate wording said “accepted CTX-01”. Fixed to the exact adoption boundary: only **after independent PASS + merge + DocSync** do role sessions use Context Bootstrap v1 as binding routing.

After F1–F4, the complete pre-review was restarted rather than relying on the earlier partial review.

## PR #107 compatibility challenge

No candidate surface expects or fabricates a PROCESS_ONLY execution receipt/GREEN product proof. CTX-01 treats Worker handoff lint as the applicable mechanical Ready-state handoff check while its process semantics are established by the reviewed docs/evidence.

## Fail-closed / omission challenge

- stale index => hints unusable, authoritative reconstruction;
- missing/malformed compact context => authoritative reconstruction or stop, never inference;
- cross-track dependency => observable escalation;
- semantic/proof decision cannot terminate on an index/summary alone;
- local H1 executor does not broaden context to gain Worker authority;
- Reviewer minimum pack cannot substitute Worker prose for accepted predecessor evidence;
- planner/gate retains ROADMAP as initial context because ordering/gates are its claim;
- DocSync refresh cannot promote the derived index to semantic authority.

No remaining in-claim omission or dual-authority path was found.

## Scope / proof-budget verdict

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`

CTX-01 adds a small routing protocol, one machine profile file, one derived index, one checker and thin integrations. It does not build CTX-02 capsules or CTX-03 evidence infrastructure early. Further compression/measurement/history work is deliberately left to the accepted later owners.

## Worker result

`WORKER_PRE_REVIEW: CLEAN`

No known in-claim blocker remains. After this report is committed, the Worker may update only external PR handoff metadata to bind the resulting exact branch HEAD, then mark Ready. Any repository-byte mutation after this report invalidates CLEAN and requires a new complete pre-review.
