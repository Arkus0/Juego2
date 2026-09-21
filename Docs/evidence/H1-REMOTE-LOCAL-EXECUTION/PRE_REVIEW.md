# H1 remote-first local execution — Worker pre-review

Mode: PROCESS_ONLY
Baseline SHA: `55d919ea6cea7784431b628343b1cde46f2e3863`
Branch: `process/h1-remote-first-local-executor`
Scope: orchestration/process only; no Unity implementation, package selection, asset import, bridge semantics, gameplay or product-authority change.

## Purpose

Replace the prior assumption that `LOCAL_UNITY_REQUIRED` / `HYBRID` means the whole H1 Worker must run locally with a staged model:

`REMOTE_PREP -> LOCAL_EXECUTION -> REMOTE_CLOSEOUT -> independent Reviewer`.

The local Codex session is a delegated execution role, not a second Worker and not a Reviewer.

## Pre-review checks

- Existing H1 acceptance claims remain unchanged.
- Mandatory local Unity evidence is preserved for every WP that already required it.
- The remote Worker remains the single workpack owner across prep and closeout.
- Local execution is exact-input-SHA bound and must use a persisted manifest.
- The manifest must predeclare commands/actions, expected outputs and `ALLOWED_MUTATION_PATHS`.
- Local Codex may produce deterministic Unity-generated repository files only when both the action and paths are predeclared.
- Unexpected effective state, extra mutations or any design/repair need stops as `REMOTE_DECISION_REQUIRED` rather than granting local discretion.
- Local execution cannot perform Worker pre-review, freeze, mark Ready, independent review, merge, DocSync or start another WP.
- Remote closeout verifies local SHA binding, environment evidence and mutation allowlist before interpreting the result.
- Any remote repair that can invalidate Unity evidence requires a new numbered local round.
- Reviewer remains independent and remote by default; durable local evidence is reviewed rather than blindly trusted.
- Context minimization is explicit: local Codex reads the manifest, exact WP, named scripts/files and live GitHub identity/SHA state rather than reconstructing all project history.
- `WP-H1-02` is explicitly adapted as the first concrete consumer of the protocol; the binding protocol itself applies H1-02 through H1-GATE and includes a per-WP staging matrix.
- The existing implementation skill now routes all H1-02..H1-GATE Workers through the protocol.

## Boundary checks

This candidate does not:

- choose the exact Unity 6.3 patch;
- create the Unity project;
- install or pin packages;
- adopt Quaternius content;
- alter any H0/H1 semantic contract;
- weaken exact-SHA evidence or independent review;
- authorize H2/CITY keeper/gameplay work;
- create paid CI/VM infrastructure.

## Known tradeoff

The process now permits more than one commit authoring environment on the same Draft Worker branch: remote Worker plus narrowly delegated local executor. This is intentional and bounded by exact `LOCAL_INPUT_SHA`, allowed mutation paths, durable local result evidence and subsequent remote closeout. Role ownership remains singular even though execution surfaces are plural.

## Worker verdict

`WORKER_PRE_REVIEW: CLEAN`

No known in-scope blocker remains. Candidate is ready for an independent process review after the final exact branch HEAD is recorded in the PR handoff.