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

## 1. REMOTE_PREP — Worker-owned

The remote Worker does all work that does not require effective local Unity execution, including:

- reconstruct current `main`, dependencies, accepted predecessor guarantees, canonical PR/branch ownership and review state;
- complete `PREDECESSOR_CONTRACT_CHECK`;
- make architecture/design decisions owned by the WP;
- implement repository code/docs/scripts/tests that can be authored remotely;
- define exact positive/negative controls and expected evidence;
- create/continue the canonical Draft + ACTIVE implementation PR;
- reduce the remaining local need to a deterministic execution contract.

Before handing off locally, the Worker MUST persist:

`Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md`

That file is the local executor's primary instruction surface and MUST include:

- WP ID and round number (`LOCAL_ROUND: 1`, `2`, ...);
- repository `Arkus0/Juego2`;
- canonical PR number and branch;
- exact 40-char `LOCAL_INPUT_SHA` that the local machine must start from;
- exact Unity editor/project/toolchain requirement known at that point;
- exact commands/actions to run, in order;
- exact expected outputs/evidence;
- `ALLOWED_MUTATION_PATHS` for files the local execution may legitimately create/change;
- `FORBIDDEN_MUTATIONS` and scope boundary;
- explicit PASS/FAIL/blocked observations to record;
- stop conditions requiring `REMOTE_DECISION_REQUIRED`;
- destination result file: `Docs/evidence/<WP-ID>/LOCAL_EXECUTION_RESULT.md` or an explicitly versioned round equivalent.

The manifest must be sufficiently closed that the local executor does not need to reconstruct H0/H1/CITY architecture or make design choices.

## 2. LOCAL_EXECUTION — Codex local executor

Recommended human prompt:

```text
Ejecuta únicamente el LOCAL_EXECUTION de <WP-ID>. Lee Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md y sigue ese contrato literalmente. No rediseñes ni repares.
```

The local executor MUST:

1. verify the checkout is `Arkus0/Juego2` using `git` plus authenticated `gh`;
2. fetch the canonical branch and verify checkout HEAD equals the manifest `LOCAL_INPUT_SHA` before executing;
3. read the local manifest first; read the exact WP and only the files/scripts directly needed to execute it;
4. execute only the declared local actions;
5. permit deterministic Unity/tool-generated repository changes only inside `ALLOWED_MUTATION_PATHS`;
6. record exact editor/package/platform fingerprints and command results required by the manifest;
7. record the complete changed-file set and verify it is a subset of the allowlist;
8. write the declared `LOCAL_EXECUTION_RESULT` including input SHA, resulting SHA, commands/actions, outputs, failures and artifact/evidence paths;
9. commit/push only the allowlisted generated/evidence result required by the manifest when the manifest authorizes a commit;
10. STOP and return control to the remote Worker.

The local executor MUST NOT:

- reinterpret the workpack or predecessor contracts;
- choose architecture, product semantics, package strategy, thresholds or proof rules not already fixed by the manifest;
- repair unexpected failures by inventing code/configuration changes;
- expand `ALLOWED_MUTATION_PATHS`;
- perform Worker pre-review, freeze the candidate, mark Ready, independently review, merge, DocSync or start another WP.

If execution discovers that a design/code/configuration decision is required, records unexpected effective state, or would require a mutation outside the allowlist, it MUST stop with:

```text
REMOTE_DECISION_REQUIRED
```

and preserve the observation without speculative repair.

## 3. REMOTE_CLOSEOUT — same Worker owner

After local execution, the remote Worker MUST reconstruct live GitHub state again and verify:

- the local run started from the declared `LOCAL_INPUT_SHA`;
- the actual local result/commit belongs to the canonical branch/PR;
- every local mutation is inside `ALLOWED_MUTATION_PATHS`;
- evidence binds the exact effective editor/package/platform/content inputs required by the WP;
- PASS/FAIL meaning is supported by effective outputs rather than only declarations.

The remote Worker then:

- interprets the evidence against the WP claim;
- performs any required design/code repair remotely;
- creates a new numbered local round if the repair can invalidate local Unity evidence;
- repeats `REMOTE_PREP -> LOCAL_EXECUTION -> REMOTE_CLOSEOUT` only as needed;
- runs the complete strict Worker pre-review after the final evidence-bearing mutation;
- records `WORKER_PRE_REVIEW: CLEAN` only when no known in-claim blocker remains;
- freezes the final exact candidate SHA and marks the PR Ready;
- STOPs for a fresh independent Reviewer.

No pre-local or local-intermediate SHA may be frozen as the final candidate merely because its local command returned exit code zero.

## Local mutation rule

Some H1 claims necessarily require Unity to materialize repository-owned files. The local executor may therefore change more than evidence files **only when those paths and the producing action were predeclared by the remote Worker**.

Examples include `ProjectVersion.txt`, `Packages/packages-lock.json`, selected `ProjectSettings/**`, generated fixture/prefab/scene serialization or deterministic inventory outputs when the exact WP owns them. This permission is narrow and path-bound; it is not permission for local discretionary implementation.

## Token/context minimization rule

The local executor is intentionally context-poor. Its required context should normally be limited to:

- `Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md`;
- the exact `Docs/workpacks/**/<WP-ID>.md`;
- scripts/files named by the manifest;
- live GitHub metadata needed to verify repository/branch/PR/SHA;
- effective Unity/editor output produced by the trial.

It should not reread the whole project history, reconstruct unrelated predecessor proofs or duplicate the remote Worker's architectural reasoning.

## Reviewer rule

The independent Reviewer remains remote by default. It reviews the complete frozen candidate plus the durable local evidence. A Reviewer only needs its own local Unity execution when the WP or observed evidence specifically requires independent re-execution; ordinary review does not automatically consume another full local Codex Worker session.

Local evidence is not trusted merely because Codex produced it. The Reviewer checks its exact-SHA/input binding, environment fingerprint, allowlist compliance, causal relevance and whether the effective outputs actually prove the claimed local seam.

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
ChatGPT remote Worker -> prepares LOCAL_EXECUTION.md
human opens Codex locally -> "Ejecuta únicamente el LOCAL_EXECUTION de H1-XX"
Codex pushes result/evidence and stops
ChatGPT remote Worker -> closeout/freeze
fresh ChatGPT Reviewer -> PASS/FAIL
```

This is deliberately optimized so local Codex is used as the Unity/Windows execution arm rather than as the primary reasoning Worker.