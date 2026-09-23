# PRODUCT_SHA / Flow Simplification V2

Status: binding operational amendment after merge to `main`.

This amendment changes orchestration cost and protocol classification only. It does not weaken workpack acceptance criteria, exact-SHA product identity, independent Reviewer judgment, required Unity/local evidence, or causal proof obligations.

## 1. Material identity

`PRODUCT_SHA` is the exact frozen Git commit whose product/evidence bytes are under review. Existing `Candidate HEAD SHA` / `Frozen candidate SHA` fields represent the same identity; no new mandatory PR-body field is required.

Any Git commit after freeze changes `PRODUCT_SHA` and invalidates product validation, Worker pre-review and Reviewer verdict for the prior SHA.

A PR-body edit, issue/review comment, label, check/status update or Automation marker creates no Git commit. Such GitHub-side closure metadata is `NON_MATERIAL_CLOSURE` when it does not change effective WP, process class or proof class. It cannot invalidate already-green immutable product execution for the same `PRODUCT_SHA`.

## 2. GitHub Actions budget

GitHub Actions exists primarily to provide execution substrates unavailable to a chat Worker, especially pinned .NET/Unity/toolchain execution. It is not a protocol state machine that must re-prove the product after every metadata mutation.

Normal budget:

- While a PR is **Draft + ACTIVE**, one automatic heavy hosted battery is allowed per material push: **Arkus Main Safety**.
- `Arkus Main Safety` GREEN for the canonical PR and exact `PRODUCT_SHA` is the normal hosted Worker-preflight evidence when the Worker cannot run the pinned SDK locally.
- The dedicated **Worker Candidate Preflight** workflow is explicit/on-demand fallback only. It must not run automatically on ordinary PR activity.
- **Arkus Candidate Validation** is a freeze-time gate. Draft pushes do not run its product observation/verifier. It runs when a non-draft candidate is opened/updated or marked Ready, and manual observation remains explicit.
- `pull_request.edited` never triggers expensive product validation. Editing handoff prose or exact-SHA metadata is not a product mutation.
- If same-SHA metadata needs correction after a failed closure attempt, finish all corrections first, then perform at most one deliberate Draft -> Ready transition to request a fresh lightweight/final gate evaluation. Do not iterate edit -> Actions -> edit -> Actions.

A workflow may still fail closed when SHA/WP/classification no longer match. That is integrity, not permission to rerun unrelated product tests.

## 3. Worker closeout

For a normal implementation/repair cycle:

1. Work in Draft.
2. Let Main Safety cover hosted restore/build/test on each material push. Run additional WP-specific/local/Unity evidence only when the WP actually requires it.
3. Finish all repository/evidence bytes.
4. Select exact `PRODUCT_SHA`.
5. Require either local `WORKER_PREFLIGHT_GREEN` or a same-PR, same-SHA Main Safety GREEN. A manual dedicated preflight may substitute when explicitly requested.
6. Perform one strict Worker pre-review against that exact SHA.
7. Persist CLEAN outside repository bytes, reconcile the handoff once, mark Ready, and stop writing.
8. Candidate Validation performs the final WP-specific exact-SHA freeze verification once.

A protocol-only correction on unchanged `PRODUCT_SHA` does not require another semantic Worker pre-review or another Main Safety run. Only the affected metadata gate is repaired/rechecked.

## 4. Reviewer classification

Independent Reviewer verdicts are about the material claim.

- `PASS`: the material workpack claim is sufficiently demonstrated.
- `FAIL`: a material/causal blocker in code, evidence, acceptance proof, required execution, scope or an integrity defect that makes the reviewed product identity/evidence untrustworthy.
- `PROTOCOL_FIX` / `REVIEW_BLOCKED`: administrative or lifecycle metadata is malformed/stale but the exact `PRODUCT_SHA` and material evidence remain identifiable and unchanged.

A pure protocol defect must not be promoted into semantic `FAIL` merely because a checker is red. Fix it without reopening product implementation or rerunning expensive execution unless the fix changes Git bytes or makes prior evidence untrustworthy.

## 5. DocSync budget

DocSync is **delta reconciliation, not a second review**.

Default after PASS/merge is **zero-commit DocSync**:

- reconstruct the accepted PR/review/merge identity and dependency-valid next action;
- if no authoritative document's accepted meaning changed, do not edit repository files;
- emit `DOCSYNC_COMPLETE` using live accepted state and stop.

Create a DocSync commit only when the accepted transition actually changes authoritative durable meaning that future work needs, for example a roadmap gate, a workpack/track status consumed as authority, an architecture decision, or an accepted contract capsule whose represented contract changed.

Derived navigation (`ACCEPTED_STATE_INDEX.json`, compact handoff summaries, capsules unchanged by the accepted contract) is not required to be rewritten after every merge. Staleness in a non-authoritative navigation cache causes later escalation to live/authoritative sources; it does not block acceptance or force a ceremonial commit.

When a DocSync commit is genuinely required:

- make one bounded reconciliation pass;
- touch only docs whose effective accepted meaning changed;
- run only validators applicable to the files actually changed;
- do not rerun product tests for documentation-only changes;
- do not restart because unrelated `main` advanced unless that movement materially conflicts with the exact docs being changed.

## 6. Anti-loop invariant

The process must never create work solely to prove that the work created by the process did not change the product.

For unchanged `PRODUCT_SHA`:

`product GREEN -> metadata correction -> metadata recheck -> review`

is valid.

`product GREEN -> metadata correction -> full product rerun -> new closure metadata -> another full rerun`

is a protocol bug.

Where older operational text conflicts with this amendment on Action cadence, same-SHA metadata invalidation, Reviewer classification of protocol-only defects, or mandatory per-PASS DocSync churn, this amendment governs. Material acceptance/proof requirements remain unchanged.
