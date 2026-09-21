# H1 Remote-first execution protocol

Status: BINDING PROCESS OVERLAY for `WP-H1-02` through `WP-H1-GATE`

This protocol changes **where H1 work is executed**, not what any H1 workpack proves. Existing `LOCAL_UNITY_REQUIRED` / `HYBRID` labels continue to mean that exact effective local Unity evidence is mandatory. They no longer imply that the entire Worker must run locally.

## Goal

Keep design, repository reconstruction, implementation reasoning, review preparation and final closeout remote whenever possible. Use Codex on the Windows/Unity workstation only for the smallest execution slice that genuinely requires the local editor, local toolchain, local GPU/OS state or exact local import/rebuild evidence.

Canonical flow:

```text
REMOTE_PREP
  -> LOCAL_EXECUTION (minimal delegated executor)
  -> REMOTE_CLOSEOUT
  -> FROZEN_FOR_REVIEW
  -> independent remote Reviewer
```

The active Worker remains the single Worker owner across the first three stages. `LOCAL_EXECUTION` is a delegated execution role, **not** a second Worker and not an independent Reviewer.

## Exact-SHA vocabulary

The local handoff uses four different SHAs. They are deliberately not collapsed into one self-referential field:

- `EXECUTION_BASE_SHA`: exact product/candidate commit prepared by the remote Worker **before** the local manifest commit;
- `MANIFEST_COMMIT_SHA`: the next commit containing the round's persisted `LOCAL_EXECUTION.md`; this SHA is created by committing that manifest and therefore MUST be anchored outside the manifest itself;
- `PRODUCT_RESULT_SHA`: exact commit containing the allowlisted local execution outputs/candidate mutations, before the summary result file is committed; when no such repository mutation exists, it equals `MANIFEST_COMMIT_SHA`;
- `EVIDENCE_COMMIT_SHA`: the later commit containing `LOCAL_EXECUTION_RESULT.md`; because a file cannot contain the SHA of the commit that contains itself, this SHA is anchored outside that result file and then verified/persisted by `REMOTE_CLOSEOUT`.

No committed file is ever required to contain its own commit SHA. `MANIFEST_COMMIT_SHA` and `EVIDENCE_COMMIT_SHA` are durable external anchors, normally structured PR comments (or an equivalent immutable/check surface) created only after the corresponding commit SHA exists.

For canonical `WORKER_REVIEW_PROTOCOL.md` v1.8 purposes, the exact SHA from which delegated execution starts is `MANIFEST_COMMIT_SHA`. The persisted execution contract is the **composite** of the manifest plus its durable external handoff anchor. `EXECUTION_BASE_SHA` separately proves that the manifest-only handoff commit did not silently change the product state being exercised.

The causal chain for a normal mutating round is:

```text
EXECUTION_BASE_SHA
  -> MANIFEST_COMMIT_SHA        # manifest-only handoff commit; exact local start SHA
  -> PRODUCT_RESULT_SHA         # allowlisted local outputs, no result summary
  -> EVIDENCE_COMMIT_SHA        # result-summary-only commit
  -> optional remote closeout/repair commits
  -> frozen candidate
```

If there are no repository outputs before the result summary, `PRODUCT_RESULT_SHA == MANIFEST_COMMIT_SHA` and the evidence commit is its direct child.

## 1. REMOTE_PREP — Worker-owned

The remote Worker does all work that does not require effective local Unity execution, including:

- reconstruct current `main`, dependencies, accepted predecessor guarantees, canonical PR/branch ownership and review state;
- complete `PREDECESSOR_CONTRACT_CHECK`;
- make architecture/design decisions owned by the WP;
- implement repository code/docs/scripts/tests that can be authored remotely;
- define exact positive/negative controls and expected evidence;
- create/continue the canonical Draft + ACTIVE implementation PR;
- reduce the remaining local need to a deterministic execution contract.

Before handing off locally, the Worker MUST first commit all product/code/configuration changes for the round and record that exact commit as `EXECUTION_BASE_SHA`. It then persists:

`Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md`

as a **separate handoff-only commit** whose direct parent is `EXECUTION_BASE_SHA`. That manifest commit MUST change only the current round's `LOCAL_EXECUTION.md`; product implementation may not be smuggled into the handoff commit.

The manifest is the local executor's primary instruction surface and MUST include:

- WP ID and round number (`LOCAL_ROUND: 1`, `2`, ...);
- repository `Arkus0/Juego2`;
- canonical PR number and branch;
- exact 40-char `EXECUTION_BASE_SHA`;
- `MANIFEST_COMMIT_ANCHOR`, naming where the post-commit external `MANIFEST_COMMIT_SHA` will be read (normally a structured PR comment);
- exact Unity editor/project/toolchain requirement known at that point;
- exact commands/actions to run, in order;
- exact expected outputs/evidence;
- `ALLOWED_MUTATION_PATHS` for files the local execution may legitimately create/change, including the declared result-summary destination when it will be committed;
- `FORBIDDEN_MUTATIONS` and scope boundary;
- explicit PASS/FAIL/blocked observations to record;
- stop conditions requiring `REMOTE_DECISION_REQUIRED`;
- destination result file: `Docs/evidence/<WP-ID>/LOCAL_EXECUTION_RESULT.md` or an explicitly versioned round equivalent.

After committing the manifest, the Worker obtains the actual `MANIFEST_COMMIT_SHA`, pushes it to the canonical branch, verifies the remote branch HEAD equals that SHA, and only then writes a durable external handoff anchor containing at minimum `WP-ID`, `LOCAL_ROUND`, `EXECUTION_BASE_SHA`, `MANIFEST_COMMIT_SHA`, manifest path, PR and branch. The Worker then stops branch writes until the delegated executor returns control.

The manifest must be sufficiently closed that the local executor does not need to reconstruct H0/H1/CITY architecture or make design choices.

## 2. LOCAL_EXECUTION — Codex local executor

Recommended human prompt:

```text
Ejecuta únicamente el LOCAL_EXECUTION de <WP-ID>. Lee Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md y sigue ese contrato literalmente. No rediseñes ni repares.
```

The local executor MUST:

1. verify the checkout is `Arkus0/Juego2` using `git` plus authenticated `gh`;
2. read the durable handoff anchor, fetch the canonical branch and verify branch/checkout `HEAD == MANIFEST_COMMIT_SHA` before executing;
3. read the manifest from that exact commit and verify its declared `EXECUTION_BASE_SHA` is the direct parent of `MANIFEST_COMMIT_SHA` and that `EXECUTION_BASE_SHA..MANIFEST_COMMIT_SHA` changes only the declared manifest path;
4. read the exact WP and only the files/scripts directly needed to execute the manifest;
5. execute only the declared local actions;
6. permit deterministic Unity/tool-generated repository changes only inside `ALLOWED_MUTATION_PATHS`;
7. record exact editor/package/platform fingerprints and command results required by the manifest;
8. record the complete changed-file set and verify it is a subset of the allowlist;
9. if allowlisted repository outputs/candidate mutations exist, commit all of them **without** the summary result file and record the resulting commit as `PRODUCT_RESULT_SHA`; otherwise set `PRODUCT_RESULT_SHA = MANIFEST_COMMIT_SHA`;
10. write the declared `LOCAL_EXECUTION_RESULT` including `EXECUTION_BASE_SHA`, `MANIFEST_COMMIT_SHA`, `PRODUCT_RESULT_SHA`, environment fingerprint, commands/actions, outputs/failures, complete changed-file inventory and artifact/evidence paths;
11. commit the result summary separately so that the commit contains only that result file and record the resulting local commit SHA as `EVIDENCE_COMMIT_SHA`;
12. push the predeclared allowlisted commits to the canonical branch, verify the remote branch HEAD equals `EVIDENCE_COMMIT_SHA`, then publish the durable external result anchor (normally a structured PR comment) containing WP/round, `PRODUCT_RESULT_SHA` and `EVIDENCE_COMMIT_SHA`; STOP and return control to the remote Worker.

The local executor MUST NOT:

- reinterpret the workpack or predecessor contracts;
- choose architecture, product semantics, package strategy, thresholds or proof rules not already fixed by the manifest;
- repair unexpected failures by inventing code/configuration changes;
- expand `ALLOWED_MUTATION_PATHS`;
- modify the manifest after the anchored `MANIFEST_COMMIT_SHA`;
- combine the result-summary commit with unreviewed product/output mutations;
- perform Worker pre-review, freeze the candidate, mark Ready, independently review, merge, DocSync or start another WP.

If execution discovers that a design/code/configuration decision is required, records unexpected effective state, or would require a mutation outside the allowlist, it MUST stop with:

```text
REMOTE_DECISION_REQUIRED
```

and preserve the observation without speculative repair.

## 3. REMOTE_CLOSEOUT — same Worker owner

After local execution, the remote Worker MUST reconstruct live GitHub state again and verify the complete round chain, not merely trust field names:

- the external handoff anchor names the same `EXECUTION_BASE_SHA` as the manifest and the actual `MANIFEST_COMMIT_SHA`;
- `MANIFEST_COMMIT_SHA` is a direct child of `EXECUTION_BASE_SHA` and its diff changes only the round manifest;
- the local run started from the anchored `MANIFEST_COMMIT_SHA`;
- `PRODUCT_RESULT_SHA` is either exactly `MANIFEST_COMMIT_SHA` (no pre-summary repository outputs) or the single allowlisted local-output commit that is its direct child;
- every local mutation is inside `ALLOWED_MUTATION_PATHS` and the product/output commit excludes the result summary;
- `EVIDENCE_COMMIT_SHA` is the direct child of `PRODUCT_RESULT_SHA` and changes only the declared result-summary file;
- the remote branch actually reached `EVIDENCE_COMMIT_SHA` before the external result anchor was published;
- the external result anchor, result file and actual branch history agree on WP, round and SHA chain;
- evidence binds the exact effective editor/package/platform/content inputs required by the WP;
- PASS/FAIL meaning is supported by effective outputs rather than only declarations.

The Worker persists the verified `MANIFEST_COMMIT_SHA`, `PRODUCT_RESULT_SHA` and `EVIDENCE_COMMIT_SHA` in later closeout/pre-review evidence or the PR handoff. That later evidence may safely contain those already-existing SHAs because it is not part of either referenced commit.

The remote Worker then:

- interprets the evidence against the WP claim;
- performs any required design/code repair remotely;
- creates a new numbered local round if the repair can invalidate local Unity evidence;
- repeats `REMOTE_PREP -> LOCAL_EXECUTION -> REMOTE_CLOSEOUT` only as needed;
- verifies the final frozen candidate descends from the accepted `EVIDENCE_COMMIT_SHA` and that no later proof-relevant mutation invalidated the local result;
- runs the complete strict Worker pre-review after the final evidence-bearing mutation;
- records `WORKER_PRE_REVIEW: CLEAN` only when no known in-claim blocker remains;
- freezes the final exact candidate SHA and marks the PR Ready;
- STOPs for a fresh independent Reviewer.

No pre-local, manifest, product-result or evidence-intermediate SHA may be frozen as the final candidate merely because its local command returned exit code zero.

## Local mutation rule

Some H1 claims necessarily require Unity to materialize repository-owned files. The local executor may therefore change more than evidence files **only when those paths and the producing action were predeclared by the remote Worker**.

Examples include `ProjectVersion.txt`, `Packages/packages-lock.json`, selected `ProjectSettings/**`, generated fixture/prefab/scene serialization or deterministic inventory outputs when the exact WP owns them. This permission is narrow and path-bound; it is not permission for local discretionary implementation.

## Token/context minimization rule

The local executor is intentionally context-poor. Its required context should normally be limited to:

- the structured external handoff anchor for the current round;
- `Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md` at the anchored manifest commit;
- the exact `Docs/workpacks/**/<WP-ID>.md`;
- scripts/files named by the manifest;
- live GitHub metadata needed to verify repository/branch/PR/SHA;
- effective Unity/editor output produced by the trial.

It should not reread the whole project history, reconstruct unrelated predecessor proofs or duplicate the remote Worker's architectural reasoning.

**This minimization applies only to the delegated local executor.** It does not reduce, replace or reinterpret any required read, repository reconstruction, predecessor-contract check, proof-boundary analysis, strict pre-review duty or independent Reviewer duty imposed on the active Worker/Reviewer by `AGENTS.md`, `WORKER_REVIEW_PROTOCOL.md`, the exact WP or its binding proof rules. The manifest is an execution contract, not a compressed substitute for the evidence/context needed to design, interpret or review the workpack.

If a local action cannot be made mechanical without asking the executor to infer semantics from omitted predecessor/architecture context, the Worker must either encode the already-decided condition as an explicit executable check in the manifest or stop the handoff with `REMOTE_DECISION_REQUIRED`; the executor must not broaden its own context and make the missing decision. Any future project-wide optimization of Worker/Reviewer boot sets, project state, inherited-contract summaries or evidence representation requires its own reviewed process contract and is outside this overlay.

## Reviewer rule

The independent Reviewer remains remote by default. It reviews the complete frozen candidate plus the durable local evidence. A Reviewer only needs its own local Unity execution when the WP or observed evidence specifically requires independent re-execution; ordinary review does not automatically consume another full local Codex Worker session.

Local evidence is not trusted merely because Codex produced it. The Reviewer checks the exact `EXECUTION_BASE_SHA -> MANIFEST_COMMIT_SHA -> PRODUCT_RESULT_SHA -> EVIDENCE_COMMIT_SHA` chain (with the permitted equality case), external anchors, environment fingerprint, allowlist compliance, causal relevance, later frozen-candidate ancestry and whether the effective outputs actually prove the claimed local seam.

## H1 staging matrix

| WP | Remote Worker owns | Minimal local slice |
|---|---|---|
| `H1-02` | exact patch/package/project policy, scripts, expected graph/evidence | real editor availability, import/compile/EditMode, package resolution, clean second import; allowlisted Unity-generated baseline files |
| `H1-03` | host/workspace policy and tests | effective project/editor admission checks that depend on the installed project |
| `H1-03A` | dispatch/lifecycle design and implementation | real Editor process/main-thread/lifecycle execution probes |
| `H1-04` | catalogue/identity semantics, Quaternius adoption records, probes | exact source import and effective Unity catalogue/native-identity inventory |
| `H1-05` | managed-scene graph/materialization logic | real scene creation/update/publication and serialized/effective inspection |
| `H1-06` | source/prefab resolution and derivative rules | real prefab/source linkage and managed derivative execution |
| `H1-07` | component allowlist/schema/realization logic | effective Unity component inspection/realization probes |
| `H1-08` | validation model/diagnostic semantics | effective Unity validation and defect fixtures where Editor state matters |
| `H1-09` | drift/proposal semantics and canonical re-entry logic | real Unity drift observation/proposal execution without auto-pull |
| `H1-10` | checkpoint/rebuild contract and orchestration | clean local reconstruction/restart/import proof |
| `H1-11` | representative conformance plan and acceptance analysis | real Quaternius hierarchy/material/pivot/rig/animation conformance runs |
| `H1-GATE` | composed closure analysis, handoff, final pre-review | minimal fresh public-client/Unity parity trial required by the Gate |

The table is an orchestration decomposition only. It does not transfer semantic ownership between WPs.

## Failure and repair

A local failure has three classifications:

- `EXPECTED_NEGATIVE_CONTROL`: intended defect fixture failed for the intended causal reason;
- `CANDIDATE_DEFECT`: observation falsifies the current WP and returns to remote Worker repair;
- `ENVIRONMENT_BLOCKED`: required editor/license/tool/platform condition is unavailable and no product conclusion is drawn.

The local executor does not decide the repair for the latter two classes. The remote Worker owns that decision and creates the next local round when needed.

## Human interaction target

The desired recurring human workflow is:

```text
ChatGPT remote Worker -> prepares product state + LOCAL_EXECUTION.md
ChatGPT remote Worker -> pushes manifest commit, anchors MANIFEST_COMMIT_SHA outside the manifest
human opens Codex locally -> "Ejecuta únicamente el LOCAL_EXECUTION de H1-XX"
Codex pushes product/result commits, verifies remote HEAD, anchors EVIDENCE_COMMIT_SHA and stops
ChatGPT remote Worker -> verifies SHA chain, closeout/pre-review/freeze
fresh ChatGPT Reviewer -> PASS/FAIL
```

This is deliberately optimized so local Codex is used as the Unity/Windows execution arm rather than as the primary reasoning Worker.