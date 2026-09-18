# WP-HK-00 Worker adversarial pre-review

Status: **NOT_READY**  
Protocol: `WORKER_REVIEW_PROTOCOL.md` v1.3  
Proof standard: `FOUNDATIONAL_PROOF_STANDARD.md` v1.2  
Trust boundary: `Docs/evidence/WP-HK-00/TRUST_BOUNDARY_AUDIT.md`  
Independent Reviewer verdict: **not attempted by this Worker**

## Purpose

This is the mandatory Worker-side falsification gate. It attacks the actual HK00 acceptance claims inside the declared finite trust boundary. It is not independent review and does not attempt to prove malicious subversion of Git, the pinned .NET/MSBuild/C# toolchain, NuGet, OS or CI infrastructure.

## Acceptance-focused audit

### 1. Canonical boundary / universe — DESIGN CLEAN

- Git candidate tree is the independent project/source existence universe.
- Fixed Arkus classification is separate from generated inventories/manifest data.
- Canonical projects must exist and canonical solution membership must match the fixed set.
- Tracked production C# must map to exactly one owner.
- The previous self-shrinking scan-root/extension-boundary class has been repaired at the universe boundary rather than special-cased.

### 2. Portable/toolchain/engine boundary — DESIGN CLEAN

- Portable projects are fixed to `netstandard2.1`, C# 9, warnings-as-errors and deterministic build settings; host/tests/proof use pinned .NET 8.
- SDK `8.0.425`, runtime/ref pack `8.0.31`, roll-forward disabled.
- Product projects have no third-party NuGet package allowance and no Unity/UnityEditor/DFU dependency path.
- Current complete product source remains HK00 boundary code only; gameplay/Unity/assets remain forbidden scope.

### 3. Dependency graph / effective compilation — DESIGN CLEAN

- Evaluated `ProjectReference` graph is compared with fixed direction and checked for cycles.
- Effective compiler source inputs are observed for all canonical projects; owned sources must reach compilation.
- Portable PDB/source checks bind compiled source documents to candidate bytes.
- Required product dependency edges also appear in emitted assembly metadata as defence in depth.

### 4. Reproducible dependency/build path — DESIGN CLEAN

- Test direct dependencies are centrally pinned and the complete transitive graph is committed in `packages.lock.json` with content hashes.
- A fresh isolated lock observation is compared to the committed lock; canonical restore uses locked mode.
- Clean Linux restore/build/test is the required execution path.
- Repository-controlled alternate build channels already implemented (solution membership, response/import/compiler extension/reference/output checks) remain defence in depth; no new hostile-toolchain family will be added unless an HK00 acceptance is actually falsified.

### 5. Proof/evidence integrity — DESIGN CLEAN

- Candidate is read-only during positive proof; observations are emitted below ignored `artifacts/`.
- Self-attacks run in a disposable Git copy.
- Proof bootstrap is direct from tracked proof C# under pinned compiler as defence in depth against self-confirming proof-project configuration.
- Normalized effective compiler arguments make material input drift visible without claiming exhaustive certification of compiler internals.

### 6. Trust boundary / proof budget / handoff — DESIGN CLEAN, EXECUTION PENDING

- Git, pinned .NET/MSBuild/C# behavior, NuGet lock/restore behavior and OS/CI are explicitly trusted base.
- Existing 37 attacks are classified as `REQUIRED_FOR_HK00` or `DEFENCE_IN_DEPTH`; arbitrary attacks on trusted implementations are `OUTSIDE_TRUST_BOUNDARY` non-claims.
- Proof growth has been bounded after the prior self-shrinking-universe repairs.
- `PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
- PR #8 remains Draft/ACTIVE and is the only canonical HK00 implementation surface.

## Material causal classes repaired during Worker lineage

1. self-shrinking project/source scan boundary;
2. project/build participant pre-filtering before classification;
3. Tests/Proof outside effective source/PDB observation;
4. proof oracle able to depend on its own MSBuild compile universe;
5. response/solution/import behavior not aligned with canonical observation;
6. material compiler input channels omitted from effective observation;
7. PE/PDB output substitution gap;
8. proof/evidence mutation of the candidate;
9. transitive test dependency/analyzer authority not explicitly bounded;
10. external import/reference provenance split across inconsistent policies;
11. candidate lineage behind binding process rules.

These repairs remain regression-covered by the existing 37 causal controls. No additional family is justified by the current HK00 claim.

## Current gate

`DESIGN_AUDIT: CLEAN`  
`WORKER_PRE_REVIEW: NOT_READY`  
`WORKER_PRE_REVIEW_FINDINGS_FIXED: 11`  
`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`  
`WORKER_PRE_REVIEW_BLOCKER: FINAL_EXACT_SHA_EXECUTION_AND_EVIDENCE_RECONCILIATION_PENDING`

The candidate must remain Draft until:

1. positive proof executes GREEN on the exact evidence candidate;
2. all 37 retained attacks execute RED for intended guard → pristine reconstruction → GREEN;
3. observed inventories/results are reconciled with committed evidence;
4. the exact post-evidence SHA reruns both gates with zero drift;
5. the Worker rechecks the complete final diff/handoff without modifying candidate bytes.

Only then may this document/state be promoted to `WORKER_PRE_REVIEW: CLEAN` and the same exact HEAD frozen for a fresh independent Reviewer.
