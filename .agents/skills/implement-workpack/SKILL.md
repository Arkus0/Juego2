# implement-workpack

Implement exactly one explicitly authorized Juego2 workpack.

## Authority

Read `AGENTS.md`, the exact WP, relevant accepted predecessor contracts, and `Docs/engineering/PRODUCT_SHA_CLOSURE.md`. The latter is the binding operational amendment for Action cadence, same-SHA closure and DocSync cost. Material acceptance/proof requirements from the WP and engineering standards remain unchanged.

For H1 work that requires Unity/Windows/local execution, also follow `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md`. Remote-first reasoning does not remove mandatory real Unity evidence when the WP requires it.

## Flow

1. Reconstruct live `main`, the canonical PR/branch, dependencies and latest accepted predecessor identity. Do not select a different WP.
2. Keep implementation **Draft + ACTIVE** while Git bytes can change.
3. Record a bounded `PREDECESSOR_CONTRACT_CHECK`: inherited guarantees used, guarantees newly owned here, and concrete reopen conditions. Do not re-prove accepted predecessors without contradictory evidence.
4. Implement only the WP's allowed scope. Add the positive/negative/causal evidence actually required by the claim.
5. Use GitHub Actions as execution substrate, not lifecycle ceremony. During Draft, the normal hosted .NET battery is **Arkus Main Safety**. Do not wait for or trigger duplicate product workflows merely to obtain another GREEN for the same SHA.
6. If local pinned .NET is available, `scripts/worker-preflight.sh` remains valid. If it is not, a successful Main Safety pull-request run bound to this PR and exact SHA is the normal hosted preflight. The dedicated `Worker Candidate Preflight` workflow is manual fallback only.
7. For H1/local-engine work, perform only the local round(s) materially required by `H1_REMOTE_LOCAL_EXECUTION.md`; a later repair that cannot affect that evidence does not force an unrelated local rerun.
8. Finish **all repository/evidence bytes** before closeout. Then stop writers and read the exact 40-character HEAD. That is `PRODUCT_SHA` and the existing `Frozen candidate SHA` / `Candidate HEAD SHA` identity.
9. Require exact-SHA preflight evidence for `PRODUCT_SHA` (local GREEN or same-PR/same-SHA Main Safety GREEN), then perform one strict Worker pre-review against the complete candidate and exact WP claim.
10. If pre-review finds a material in-claim blocker, repair while Draft, rerun only affected validation/evidence, select the new `PRODUCT_SHA`, and repeat the pre-review. Do not rerun unrelated proof.
11. When materially clean, persist `WORKER_PRE_REVIEW: CLEAN` in a durable PR issue comment bound to `PRODUCT_SHA`, reconcile the canonical handoff once, set `FROZEN_FOR_REVIEW` / `Branch frozen: YES`, and mark Ready.
12. `Arkus Candidate Validation` is the final freeze-time WP-specific verification. Once Ready, stop writing and hand off to a fresh independent Reviewer.

## Anti-loop rule

A Git commit after freeze changes `PRODUCT_SHA` and invalidates the old material validation/pre-review.

A PR-body/comment/check/status correction on the **same** `PRODUCT_SHA` is `NON_MATERIAL_CLOSURE`. It does not invalidate Main Safety, WP-specific product execution or the semantic Worker pre-review. Finish all metadata corrections, then perform at most one deliberate Ready/recheck. Never iterate body edit -> full product run -> body edit -> full product run.

Mechanical protocol defects are `REVIEW_BLOCKED`/`PROTOCOL_FIX`, not evidence that the WP code is defective. Repair the affected metadata only unless product/evidence identity is genuinely ambiguous.

## Stop condition

After the exact candidate is frozen and final freeze validation is green, report the exact `PRODUCT_SHA` and STOP for a fresh independent Reviewer. The Worker never reviews its own candidate.
