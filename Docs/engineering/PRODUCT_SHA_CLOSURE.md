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
- The durable context-bound `REVIEW_READY` marker is the terminal mechanical Worker -> Reviewer handoff state. There is no second `REVIEW_READY_CLOSED` phase or workflow in the normal path. Reviewer PASS/FAIL independently rechecks that a matching `REVIEW_READY` marker predates the verdict and still binds the reviewed SHA/context.

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
8. Candidate Validation performs the final WP-specific exact-SHA freeze verification once and Automation persists context-bound `REVIEW_READY`.

A protocol-only correction on unchanged `PRODUCT_SHA` does not require another semantic Worker pre-review or another Main Safety run. Only the affected metadata gate is repaired/rechecked.

## 4. Reviewer classification and execution budget

Independent Reviewer verdicts are about the material claim.

- `PASS`: the material workpack claim is sufficiently demonstrated.
- `FAIL`: a material/causal blocker in code, evidence, acceptance proof, required execution, scope or an integrity defect that makes the reviewed product identity/evidence untrustworthy.
- `PROTOCOL_FIX` / `REVIEW_BLOCKED`: administrative or lifecycle metadata is malformed/stale but the exact `PRODUCT_SHA` and material evidence remain identifiable and unchanged.

A pure protocol defect must not be promoted into semantic `FAIL` merely because a checker is red. Fix it without reopening product implementation or rerunning expensive execution unless the fix changes Git bytes or makes prior evidence untrustworthy.

Reviewer independence means independent judgment and independent attempts to falsify the claim; it does **not** require mechanically repeating an identical execution that is already durably bound to the exact reviewed SHA. The Reviewer should consume trustworthy exact-SHA CI/receipt results for mechanical facts already established and run additional targeted probes only when they add information: for example to test a causal false-green hypothesis, cover a materially unproved claim, resolve contradictory evidence, or replace evidence whose identity/provenance cannot be trusted. Re-running the same build/test/proof command solely because a new Reviewer session started is redundant work, not additional independence.

### 4.1 Causal review circuit breaker — no overdefense / whack-a-mole

A Reviewer may not turn successive repair cycles into an expanding hardening campaign. A new material `FAIL` is valid only when all of the following are true:

1. **Existing claim:** the blocker is tied to an acceptance criterion, owned guarantee, required negative class, trust-boundary rule or other requirement that already belongs to the frozen workpack. The review must name that existing requirement.
2. **Concrete false-green:** there is concrete evidence or a bounded reproduction showing how the current candidate can remain GREEN while that existing claim is false. A merely conceivable adjacent failure mode is not enough.
3. **No quantifier expansion:** the blocker does not strengthen the claim from the workpack's actual boundary into a new guarantee such as broader version compatibility, stronger durability, wider platform coverage, extra content classes or a more universal completeness theorem unless the frozen contract already requires it.
4. **Residuals stay residual:** an explicitly excluded, deferred or residual risk cannot be promoted into a blocker just because a repair makes it newly imaginable. It reopens the current WP only if concrete evidence shows the accepted in-scope guarantee itself is false or the residual was misclassified against the frozen contract.
5. **Repair does not create new obligations:** closing one causal defect class does not by itself create an obligation to defend every neighboring variant. A later blocker must be either the same causal class demonstrably still open, a regression introduced by the repair, or a genuinely independent in-scope defect with concrete current evidence.

From the **second material repair cycle onward**, every Reviewer `FAIL` must include a short `CIRCUIT_BREAKER_CHECK` stating which of the three permitted categories applies: `SAME_CAUSAL_CLASS_OPEN`, `REPAIR_REGRESSION`, or `NEW_IN_SCOPE_DEFECT`. It must also state why the finding does not require stronger scope than the frozen WP. If the Reviewer cannot make that statement truthfully, the finding is residual/future work rather than a current blocker.

A Worker under this circuit breaker repairs the causal class, not an ever-growing list of examples. Once the frozen claim is sufficiently demonstrated and no qualifying in-scope falsifier survives, the correct verdict is `PASS` even though additional defensive hardening could still be imagined.

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

Where older operational text conflicts with this amendment on Action cadence, same-SHA metadata invalidation, Reviewer execution reuse, `REVIEW_READY` terminal handoff, Reviewer classification of protocol-only defects, mandatory per-PASS DocSync churn, or causal review circuit-breaking, this amendment governs. Material acceptance/proof requirements remain unchanged.
