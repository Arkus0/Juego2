# H1 Worker execution protocol

Status: BINDING PROCESS OVERLAY for `WP-H1-02` through `WP-H1-GATE`

This protocol changes **how H1 work may be owned and executed**, not what any H1 workpack proves. Existing `LOCAL_UNITY_REQUIRED` / `HYBRID` labels continue to require the exact effective Unity/toolchain evidence named by the workpack. They do **not** require a specific ChatGPT surface, Codex session, Work session or mandatory remote-preparation handoff.

## Core rule

A H1 Worker may be run end-to-end from **Chat, Codex, Work, or another explicitly authorized capable Worker environment**.

The selected Worker environment is not part of the product claim. Acceptance depends on the exact candidate, the required effective toolchain/Unity evidence, repository state, proof obligations and independent Reviewer judgment.

The following invariants remain binding regardless of interface:

- one active Worker writer for the WP candidate at a time;
- one canonical implementation PR/branch;
- live GitHub state is authoritative for mutable PR/branch/review/check state;
- required Unity/toolchain evidence must be effective, exact and bound to the candidate as required by the WP;
- exact-SHA execution/evidence rules from `EXECUTION_RECEIPT_PROTOCOL.md` remain binding;
- Worker strict pre-review, freeze and fresh independent Reviewer remain mandatory;
- changing interface never authorizes concurrent writers or reuse of Worker context as independent Reviewer context.

## Worker-selected execution modes

The Worker may choose whichever of these modes is convenient and capable for the current task.

### Mode A — single-environment Worker

One Worker session owns implementation, required execution, evidence interpretation, repair, pre-review and freeze.

Examples:

```text
Codex Worker -> implementation -> local Unity/toolchain -> evidence -> pre-review -> freeze
```

```text
Work Worker -> implementation -> required execution available to that environment -> evidence -> pre-review -> freeze
```

```text
Chat Worker -> implementation -> required execution available to that environment -> evidence -> pre-review -> freeze
```

No `REMOTE_PREP`, `LOCAL_EXECUTION.md`, delegated-executor manifest or `REMOTE_CLOSEOUT` ceremony is required merely because the WP is in H1.

A single-environment Worker still has to prove the same claim. If that environment cannot access a mandatory editor/toolchain/effective state, it must not invent or waive the evidence; it may instead switch environment or use Mode B.

### Mode B — optional delegated execution

A Worker that cannot or does not want to run the required Unity/toolchain slice itself may delegate only that execution slice to another capable environment while remaining the Worker owner.

The established staged pattern remains valid:

```text
WORKER_PREP
  -> DELEGATED_EXECUTION
  -> WORKER_CLOSEOUT
  -> FROZEN_FOR_REVIEW
  -> independent Reviewer
```

The delegated executor is **not** a second Worker and not an independent Reviewer. Its role is mechanical execution of a closed contract.

For an in-flight round already using the previous `REMOTE_PREP -> LOCAL_EXECUTION -> REMOTE_CLOSEOUT` protocol, the existing manifest/result/SHA-chain mechanism remains valid and may be completed exactly as started. No round, evidence or Unity execution must be repeated solely because this overlay now makes that handoff optional.

## Optional delegated-execution contract

When Mode B is used, the Worker may use the existing persisted handoff format:

- `Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md`
- `Docs/evidence/<WP-ID>/LOCAL_EXECUTION_RESULT.md`

and the existing SHA vocabulary:

- `EXECUTION_BASE_SHA` — committed product/candidate state before the handoff manifest;
- `MANIFEST_COMMIT_SHA` — manifest-only handoff commit, anchored after it exists;
- `PRODUCT_RESULT_SHA` — allowlisted execution outputs/candidate mutations, or the manifest SHA when there are none;
- `EVIDENCE_COMMIT_SHA` — result-summary-only commit, anchored after it exists.

For a normal mutating delegated round:

```text
EXECUTION_BASE_SHA
  -> MANIFEST_COMMIT_SHA
  -> PRODUCT_RESULT_SHA
  -> EVIDENCE_COMMIT_SHA
  -> optional Worker closeout/repair commits
  -> frozen candidate
```

The handoff manifest should close the delegated executor's decision surface: exact WP/round, repository/PR/branch, exact starting SHA, required Unity/toolchain, commands/actions, expected evidence, allowed mutation paths, forbidden mutations, stop conditions and result destination.

The delegated executor must verify the anchored start state, execute only the declared actions, keep mutations inside the allowlist, record the required effective fingerprints/results, commit allowlisted product outputs separately from the result summary when repository mutation is needed, push/anchor the resulting SHAs, and return control to the Worker.

Unexpected design/code/configuration decisions remain Worker-owned. A delegated executor must stop rather than silently redesign or broaden scope.

The Worker then verifies the actual ancestry, changed-file set, anchors, environment/effective evidence and result meaning before using that delegated evidence in pre-review/freeze.

## Interface switching

A WP may move from Chat to Codex, Work to Chat, Codex to Work, or another authorized Worker surface without a special execution handoff **provided there is still only one active Worker writer**.

The new surface reconstructs live GitHub state and continues from the exact canonical branch/HEAD. Interface change is not itself a product mutation and does not require `LOCAL_EXECUTION.md`.

If the previous surface is still writing, the switch must wait until that writer stops. The single-writer rule is about concurrent authority, not about forcing one interface for the whole WP.

## Reviewer rule

The independent Reviewer judges the frozen candidate and evidence, not which Worker interface was used.

The Reviewer MUST NOT fail a candidate solely because:

- the Worker did not begin remotely;
- the Worker was entirely Codex, entirely Work or entirely Chat;
- the Worker switched authorized interfaces while preserving single-writer state;
- the Worker used the older staged remote/local handoff and its evidence chain is otherwise valid.

The Reviewer MAY fail when the chosen environment or handoff causes a real proof defect: missing mandatory Unity/toolchain evidence, ambiguous candidate binding, concurrent/unreconciled writes, untracked mutations, incomplete effective evidence, false-green validation, or any other blocker inside the WP claim.

Reviewer independence is unchanged. A Worker session never becomes its own independent Reviewer merely because it changes interface.

## H1 execution labels

For H1 workpacks:

- `REMOTE_OK` means no effective local/engine execution is required by the claim.
- `LOCAL_UNITY_REQUIRED` means the claim requires the exact effective Unity evidence named by the WP. It does **not** mean remote preparation is mandatory.
- `HYBRID` means the claim combines repository/remote-verifiable obligations with effective Unity/toolchain obligations. It does **not** prescribe which Worker interface owns either portion.

A future WP may explicitly require a particular physical environment only when that environment is itself material to the claim. Such a requirement must be stated in the WP, not inferred from the Worker interface.

## Existing H1-02 rounds

`WP-H1-02` began under the earlier mandatory staged protocol. Those rounds remain admissible evidence when their actual SHA chain, mutation allowlist, editor/toolchain fingerprints and effective results satisfy the contract under which they were created.

This process relaxation is **not** a reason to invalidate, restart or reproduce an already valid H1-02 local execution. Conversely, it does not excuse any H1-02 evidence defect discovered by Worker or Reviewer.

## Human interaction target

The intended user experience is choice, not another mandatory route:

```text
Option 1: Chat Worker -> whatever execution it can validly perform -> Reviewer
Option 2: Codex Worker -> local Unity/toolchain -> Reviewer
Option 3: Work Worker -> whatever execution it can validly perform -> Reviewer
Option 4: any Worker -> optional delegated Unity/toolchain slice -> same Worker -> Reviewer
```

GitHub remains the persistent project state; the Worker interface is replaceable.