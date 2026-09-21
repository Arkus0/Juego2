# H1 remote-first local execution — Worker pre-review

Mode: PROCESS_ONLY
Baseline SHA: `55d919ea6cea7784431b628343b1cde46f2e3863`
Branch: `process/h1-remote-first-local-executor`
Scope: orchestration/process only; no Unity implementation, package selection, asset import, bridge semantics, gameplay or product-authority change.

## Purpose

Replace the prior assumption that `LOCAL_UNITY_REQUIRED` / `HYBRID` means the whole H1 Worker must run locally with a staged model:

`REMOTE_PREP -> LOCAL_EXECUTION -> REMOTE_CLOSEOUT -> independent Reviewer`.

The local Codex session is a delegated execution role, not a second Worker and not a Reviewer.

## Repair history

### First pre-review repair

The first frozen candidate `6c8c32e572de0fd074023ec0c3b16ce0fb053883` was voluntarily reopened before independent review after a process-risk challenge: the H1 overlay distinguished executor from Worker, but the superior `WORKER_REVIEW_PROTOCOL.md` did not yet explicitly authorize or define bounded delegated execution.

The PR was returned to Draft before mutation. The correction was causal rather than cosmetic: `WORKER_REVIEW_PROTOCOL.md` v1.8 now explicitly defines delegated execution under singular Worker ownership, transfer boundaries, concurrency limits, pre-review duties and Reviewer checks.

### Independent Reviewer FAIL repaired

Independent review #5270237551 issued FAIL on candidate `d987622d1da58ccd8776652ddf288761b8e1f35f` because the H1 overlay required a persisted `LOCAL_EXECUTION.md` to contain the same `LOCAL_INPUT_SHA` that local `HEAD` had to equal. If the manifest itself is committed in that HEAD, the requirement is self-referential: writing the commit SHA into the file changes the commit and therefore changes the SHA. The result file had the same ambiguity for its own `resulting SHA`.

The repaired protocol now uses a non-self-referential chain:

```text
EXECUTION_BASE_SHA
  -> MANIFEST_COMMIT_SHA
  -> PRODUCT_RESULT_SHA
  -> EVIDENCE_COMMIT_SHA
  -> later remote closeout/pre-review commits
  -> frozen candidate
```

Definitions are explicit:

- `EXECUTION_BASE_SHA` is the exact remote-prepared product/candidate state before the handoff manifest commit.
- `MANIFEST_COMMIT_SHA` is the manifest-only direct child of the execution base and is the exact SHA from which delegated execution starts. It is anchored outside the manifest only after the commit exists and is present on the remote canonical branch.
- `PRODUCT_RESULT_SHA` is the single allowlisted local-output commit before the result summary; when no pre-summary repository mutation exists it equals `MANIFEST_COMMIT_SHA`.
- `EVIDENCE_COMMIT_SHA` is the result-summary-only direct child of `PRODUCT_RESULT_SHA`; it is anchored outside the result file only after push succeeds and remote branch HEAD is verified to equal it.

For v1.8 canonical-protocol purposes, the exact delegated input SHA is `MANIFEST_COMMIT_SHA`. The auditable execution contract is composite: persisted manifest plus durable external handoff anchor. `EXECUTION_BASE_SHA` independently proves the handoff commit did not silently alter the product state being exercised.

Neither `LOCAL_EXECUTION.md` nor `LOCAL_EXECUTION_RESULT.md` is ever required to contain the SHA of the commit that contains that same file.

### Additional Worker pre-review finding repaired

While repairing the Reviewer blocker, strict pre-review found a second-order handoff hazard: publishing the external `EVIDENCE_COMMIT_SHA` anchor before the local commits had actually been pushed could leave a durable anchor naming a commit that never reached the canonical branch if push failed.

The final contract now requires:

1. create the product/result commits locally;
2. push them to the canonical branch;
3. verify remote branch HEAD equals `EVIDENCE_COMMIT_SHA`;
4. only then publish the external result anchor.

The same ordering is required for the Worker handoff: push and verify `MANIFEST_COMMIT_SHA` on the remote branch before publishing its external handoff anchor.

## Mechanical trace of one valid round

The repaired contract is instantiable without a fixed-point SHA:

1. Worker commits all remote implementation as commit `B`; manifest does not yet exist. `EXECUTION_BASE_SHA = B`.
2. Worker writes a manifest that contains `B`, not its own future SHA, and commits only that manifest. Git now produces commit `M`.
3. Worker pushes `M`, verifies remote branch HEAD is `M`, then publishes external handoff anchor `MANIFEST_COMMIT_SHA = M`.
4. Local executor fetches the branch, verifies `HEAD == M`, verifies `parent(M) == B`, and verifies `B..M` changes only the manifest.
5. Local executor performs only declared actions. If repository outputs exist, it commits them once as `P`; otherwise `P = M`.
6. Result file records `B`, `M`, `P`, environment/actions/results/inventory. It does not try to name its own future commit.
7. Local executor commits only that result file and Git produces `E`.
8. Local executor pushes through `E`, verifies remote branch HEAD is `E`, then publishes external result anchor `EVIDENCE_COMMIT_SHA = E`.
9. Remote Worker independently reconstructs `B -> M -> P -> E`, verifies diffs/allowlist/effective evidence, then may add later closeout/pre-review commits. If any later repair can invalidate the local evidence, a new numbered round is mandatory.
10. Final candidate may freeze only after the final Worker pre-review and must descend from the accepted evidence commit.

There is no step in which a committed file must predict or contain the hash of its own containing commit.

## Negative / fail-closed checks

The repaired surfaces now stop rather than improvise when any of these occur:

- external `MANIFEST_COMMIT_SHA` does not equal canonical remote branch/checkout HEAD at local start;
- manifest `EXECUTION_BASE_SHA` is not the direct parent of `MANIFEST_COMMIT_SHA`;
- the manifest commit changes any path other than the current round's manifest;
- result-summary destination is not allowlisted when it will be committed;
- local output mutates a path/action not predeclared by the manifest;
- product/output commit mixes in the result summary;
- result-summary commit contains anything except the declared result file;
- external result anchor is attempted before the remote branch actually reaches `EVIDENCE_COMMIT_SHA`;
- external anchors, result file and Git ancestry disagree on WP/round/SHA chain;
- a later remote repair can invalidate the effective Unity evidence without issuing a new local round;
- local execution needs architecture, package-strategy, threshold, scope or repair discretion.

Those conditions return control to the remote Worker or make the round invalid; they are not silently normalized.

## Complete pre-review checks

- `WORKER_REVIEW_PROTOCOL.md` v1.8 remains the canonical authority permitting a Worker to delegate a bounded execution slice without transferring Worker ownership.
- A conforming executor owns no workpack interpretation, architecture, product semantics, proof design, repair decision, strict pre-review or freeze authority.
- The H1 overlay narrows v1.8 and defines a concrete composite exact-SHA handoff without weakening the canonical requirement for an exact delegated input SHA.
- Old H1-specific `LOCAL_INPUT_SHA == manifest-containing HEAD` wording has been removed from the overlay, Worker skill, local-executor skill and H1-02 contract.
- The exact local start SHA is now `MANIFEST_COMMIT_SHA`, obtained only after the manifest-only commit exists and anchored externally.
- The remote product state is separately identified by `EXECUTION_BASE_SHA` and must be the direct parent of the manifest commit.
- The result summary records already-existing `EXECUTION_BASE_SHA`, `MANIFEST_COMMIT_SHA` and `PRODUCT_RESULT_SHA`; its own containing `EVIDENCE_COMMIT_SHA` is anchored externally after verified push.
- Local Codex may produce deterministic Unity-generated repository files only when both the producing action and paths are predeclared.
- Complete local changed-file inventory remains mandatory and must be checked against `ALLOWED_MUTATION_PATHS`.
- H1 unexpected effective state, extra mutations or any design/repair need stops as `REMOTE_DECISION_REQUIRED` rather than granting local discretion.
- Local execution cannot perform Worker pre-review, freeze, mark Ready, independent review, merge, DocSync or start another WP.
- Remote closeout verifies the complete SHA ancestry, both external anchors, remote branch state, environment evidence and mutation allowlist before interpreting the result.
- Any remote repair that can invalidate Unity evidence requires a new numbered local round.
- The final frozen candidate must descend from accepted local evidence and must not contain a later proof-relevant mutation that escaped rerun.
- Reviewer remains independent and remote by default; durable local evidence is reviewed rather than blindly trusted.
- Context minimization remains explicit: local Codex reads the external anchor, manifest, exact WP, named scripts/files and live GitHub identity/SHA state rather than reconstructing all project history.
- `WP-H1-02` is explicitly adapted as the first concrete consumer and now includes the SHA-chain failure class in its deterministic evidence/negative-conformance boundary.
- The implementation skill routes H1-02..H1-GATE Workers through the same exact chain and retains canonical Worker duties.
- This PROCESS_ONLY PR itself does not need Unity evidence because it does not make any effective Unity/product claim.

## Boundary checks

This candidate does not:

- choose the exact Unity 6.3 patch;
- create the Unity project;
- install or pin packages;
- adopt Quaternius content;
- alter any H0/H1 semantic product contract;
- weaken exact-SHA evidence or independent review;
- permit a delegated executor to act as an undeclared second Worker;
- authorize H2/CITY keeper/gameplay work;
- create paid CI/VM infrastructure.

## Current-main drift

Current `main` is `a57afa3d3fc60b3e1c59d04149fe982f387c2048` (`Merge PA-02 DocSync`). The drift after this process branch began changes PA-02 evidence/result, PA workpack state and the global index. It does not overlap the H1/process candidate paths or change the ownership/evidence problem addressed here. Reviewer must still reconstruct current `main` independently.

## Known tradeoff

The process permits more than one execution/commit environment on the same Draft Worker branch: remote Worker plus narrowly delegated executor. That is intentional. Singular **authority** is preserved by the canonical v1.8 contract even though execution surfaces may be plural. The price is additional SHA/allowlist/evidence bookkeeping; the benefit is that expensive local engine sessions remain context-poor and mechanical.

The explicit two-stage local commit (`PRODUCT_RESULT_SHA` then result-summary-only `EVIDENCE_COMMIT_SHA`) adds one mechanical commit per round. This is intentional because it removes the self-referential result-SHA ambiguity and gives remote closeout a causal diff boundary that can be verified from Git history.

## Worker verdict

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 3`

Fixed findings across this process candidate:

1. missing superior-protocol authorization/boundary for delegated execution;
2. independent Reviewer #5270237551 self-referential manifest/result SHA contract;
3. repair pre-review ordering hazard where an external result anchor could otherwise be published before the referenced commit was confirmed on the canonical remote branch.

No known in-scope blocker remains after the complete rerun. This evidence file intentionally does not attempt to contain the SHA of the commit that contains itself. The exact final Candidate/Frozen SHA must be recorded externally in the PR handoff after this final evidence mutation is committed.