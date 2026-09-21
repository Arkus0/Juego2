# CTX plan — Worker pre-review

`WORKER_PRE_REVIEW: CLEAN`
`WORKER_PRE_REVIEW_FINDINGS_FIXED: 2`

Baseline: `e388f6e3c9d42e89418bd8877ed36fcbcf6d2aaa`
Mode: PROCESS_ONLY planning only

## Complete candidate reviewed

Reviewed the complete baseline→candidate plan surface:

- `Docs/workpacks/CTX/README.md`
- `Docs/workpacks/CTX/WP-CTX-01.md`
- `Docs/workpacks/CTX/WP-CTX-02.md`
- `Docs/workpacks/CTX/WP-CTX-03.md`
- `Docs/evidence/CTX-PLAN/PLAN.md`

No `AGENTS.md`, role skill, Worker/Reviewer protocol, state index, ROADMAP, evidence format, H1 local-execution rule or product/runtime contract is changed by this planning candidate.

## Findings fixed before freeze

### F1 — plan/implementation boundary was initially too broad

The first Draft direction mixed the CTX programme plan with an implementation of Context Bootstrap v1. That would have made one Reviewer approve both the causal split and the first operating-rule change at once.

Repair: reset the Draft branch to current `main` and retain only the planning surfaces. `WP-CTX-01` now owns bootstrap implementation after the plan itself is independently accepted.

### F2 — `NON-PRODUCT-FOUNDATIONAL` could be misread as weaker review

The WPs correctly avoid binding the product `FOUNDATIONAL_PROOF_STANDARD.md` merely because they optimize process, but the label alone could be read as permission for a lighter lifecycle.

Repair: `Docs/workpacks/CTX/README.md` now explicitly requires the normal Worker pre-review → exact frozen candidate SHA → fresh independent Reviewer → PASS/FAIL → merge → DocSync lifecycle for every CTX WP.

## Causal-boundary challenge

- CTX-01 owns context selection/authority/escalation only; it may not introduce predecessor capsules or migrate evidence/history.
- CTX-02 owns lossy-compression risk for accepted predecessor guarantees and therefore depends on CTX-01's authority/escalation rules.
- CTX-03 owns repeated representation/DocSync/history and measured closure; it consumes CTX-01/02 rather than redefining them.
- One giant CTX WP would combine three independently rejectable claims; more than three would split one repeated-state/representation claim into administrative fragments.

## Quality-regression challenge

The plan explicitly forbids savings that:

- weaken DoD/proof;
- hide cross-track gates;
- trust stale summaries over live GitHub;
- make compact projections semantic authority;
- reduce Reviewer independence;
- delete accepted history/evidence;
- broaden H1 local executor discretion.

Each WP contains causal negative/process controls for its own principal false-green class.

## Execution / dependency challenge

- all CTX work is REMOTE_OK and PROCESS_ONLY;
- CTX is not a semantic prerequisite for H1/CITY/PA;
- human priority may run CTX before local H1 without rewriting product DAGs;
- CTX-01 becomes executable only after this plan PASS + merge + documentation-only DocSync;
- plan DocSync may register CTX and name CTX-01, but may not implement CTX-01 rules.

## Verdict

No known in-claim blocker remains. The plan is ready to freeze for a fresh independent Reviewer.