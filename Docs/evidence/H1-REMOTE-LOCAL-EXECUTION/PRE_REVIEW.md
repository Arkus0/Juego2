# H1 remote-first local execution — Worker pre-review

Mode: PROCESS_ONLY
Baseline SHA: `55d919ea6cea7784431b628343b1cde46f2e3863`
Branch: `process/h1-remote-first-local-executor`
Scope: orchestration/process only; no Unity implementation, package selection, asset import, bridge semantics, gameplay or product-authority change.

## Purpose

Replace the prior assumption that `LOCAL_UNITY_REQUIRED` / `HYBRID` means the whole H1 Worker must run locally with a staged model:

`REMOTE_PREP -> LOCAL_EXECUTION -> REMOTE_CLOSEOUT -> independent Reviewer`.

The local Codex session is a delegated execution role, not a second Worker and not a Reviewer.

## Reopened pre-review after first freeze

The first frozen candidate `6c8c32e572de0fd074023ec0c3b16ce0fb053883` was voluntarily reopened before independent review after a process-risk challenge: the H1 overlay distinguished executor from Worker, but the superior `WORKER_REVIEW_PROTOCOL.md` did not yet explicitly authorize or define bounded delegated execution. That left a strict Reviewer room to interpret the separate local session as an undeclared Worker transfer.

The PR was returned to Draft before mutation. The correction is causal rather than cosmetic: `WORKER_REVIEW_PROTOCOL.md` is now v1.8 and explicitly defines delegated execution under singular Worker ownership, transfer boundaries, concurrency limits, pre-review duties and Reviewer checks. The earlier clean marker/freeze is superseded by this rerun.

## Complete pre-review checks

- `WORKER_REVIEW_PROTOCOL.md` v1.8 is now the canonical authority permitting a Worker to delegate a bounded execution slice to another session/machine without transferring Worker ownership.
- A conforming executor owns no workpack interpretation, architecture, product semantics, proof design, repair decision, strict pre-review or freeze authority.
- Delegation requires an auditable exact contract before execution: input SHA, repository/PR/branch, commands/actions, environment/fingerprints, expected outputs, mutation allowlist, forbidden mutations, evidence destination and stop conditions.
- A delegated executor may mutate repository files only where both the producing action and path were predeclared; complete changed-file inventory must be checked against the allowlist.
- Unexpected effective state, a needed unplanned mutation or a design/repair choice returns control to the Worker instead of granting executor discretion.
- The canonical protocol now states that executor context may be separate/context-poor and does not enter `Worker history` while it remains bounded execution.
- If executor authority expands into discretionary implementation/design/pre-review/freeze, the delegation is breached; unauthorized work must be discarded/reverted or formalized as an explicit Worker transfer before it can be relied upon.
- Concurrent discretionary writers remain forbidden. During delegated execution the Worker cannot race overlapping mutations against the executor.
- The same Worker must reconstruct and verify returned evidence, rerun affected local execution after repairs that can invalidate it, perform the complete final strict pre-review and freeze the exact candidate.
- Reviewer duties now explicitly include checking that delegated execution did not become an undeclared second Worker/transfer and that input-SHA/environment/mutation/result evidence is causally tied to the frozen candidate.
- Existing H1 acceptance claims remain unchanged.
- Mandatory local Unity evidence is preserved for every WP that already required it.
- `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md` narrows the general v1.8 delegation rule for H1-02 through H1-GATE; it does not grant broader authority than the canonical protocol.
- The remote Worker remains the single H1 workpack owner across prep and closeout.
- H1 local execution is exact-input-SHA bound and must use a persisted `LOCAL_EXECUTION.md` manifest.
- The H1 manifest predeclares commands/actions, expected outputs and `ALLOWED_MUTATION_PATHS`.
- Local Codex may produce deterministic Unity-generated repository files only when both the action and paths are predeclared.
- H1 unexpected effective state, extra mutations or any design/repair need stops as `REMOTE_DECISION_REQUIRED` rather than granting local discretion.
- Local execution cannot perform Worker pre-review, freeze, mark Ready, independent review, merge, DocSync or start another WP.
- Remote closeout verifies local SHA binding, environment evidence and mutation allowlist before interpreting the result.
- Any remote repair that can invalidate Unity evidence requires a new numbered local round.
- Reviewer remains independent and remote by default; durable local evidence is reviewed rather than blindly trusted.
- Context minimization is explicit: local Codex reads the manifest, exact WP, named scripts/files and live GitHub identity/SHA state rather than reconstructing all project history.
- `WP-H1-02` is explicitly adapted as the first concrete consumer of the protocol; the H1 overlay applies H1-02 through H1-GATE and includes a per-WP staging matrix.
- The implementation skill routes H1-02..H1-GATE Workers through the H1 overlay while retaining the v1.8 canonical Worker duties.

## Boundary checks

This candidate does not:

- choose the exact Unity 6.3 patch;
- create the Unity project;
- install or pin packages;
- adopt Quaternius content;
- alter any H0/H1 semantic contract;
- weaken exact-SHA evidence or independent review;
- permit a delegated executor to act as an undeclared second Worker;
- authorize H2/CITY keeper/gameplay work;
- create paid CI/VM infrastructure.

## Current-main drift

After this process branch began, `main` advanced through PA-02 acceptance/DocSync. That drift changes PA-02 evidence/result, PA workpack state and the global index. It does not overlap the H1/process candidate paths or change the ownership/evidence question addressed here. The Reviewer must still reconstruct current `main` independently.

## Known tradeoff

The process permits more than one execution/commit environment on the same Draft Worker branch: remote Worker plus narrowly delegated executor. That is intentional. Singular **authority** is preserved by the canonical v1.8 contract even though execution surfaces may be plural. The price is additional exact-SHA/allowlist/evidence bookkeeping; the benefit is that expensive local engine sessions can remain context-poor and mechanical.

## Worker verdict

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 1`

The fixed finding is the missing superior-protocol authorization/boundary for delegated execution. No known in-scope blocker remains after the complete rerun. The candidate may be frozen only after this final evidence mutation is committed and the exact new branch HEAD is recorded.