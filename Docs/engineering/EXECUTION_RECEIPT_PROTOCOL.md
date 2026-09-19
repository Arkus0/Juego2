# Exact-SHA Execution Receipt Protocol

Version: 1.0 — 2026-09-19

## Purpose

Arkus validation must be reproducible and bound to the exact candidate being handed off, but it must not depend on a particular CI vendor or paid runner pool.

GitHub is the durable repository/PR/handoff surface. Execution may happen in a Worker environment, an independent Reviewer environment, Codex Desktop/local PC, a self-hosted runner, or another execution provider that can satisfy this protocol.

GitHub-hosted Actions are **not** a contractual dependency and are disabled by default for Juego2 because the project operates with a zero-budget hosted-CI policy.

## Binding terminology

Where an older repository document says `CI`, `green CI`, `exact-SHA CI`, or equivalent in a validation/freeze requirement, interpret that requirement as **compliant exact-SHA execution + receipt** unless that document explicitly requires a particular provider for a product reason.

A provider outage, quota exhaustion or hosted-runner unavailability is recoverable infrastructure backpressure. It is not an implementation FAIL and does not waive validation; use another compliant execution substrate.

## Required properties

A validation run is acceptable only when all of the following hold:

1. **Exact candidate** — the executor records a 40-character candidate SHA and verifies `git rev-parse HEAD` equals it before running validation.
2. **Clean input** — tracked/index/worktree state is clean before execution except for paths explicitly designated as ignored observation output.
3. **Declared environment** — OS/runtime/toolchain identities materially required by the WP are recorded or asserted by the canonical validation script.
4. **Canonical commands** — the WP/repository-owned validation entrypoint is used. Provider-specific YAML is orchestration only and may not redefine the proof.
5. **Fail closed** — skipped required commands, unavailable required toolchains or partial evidence produce NOT_READY/BLOCKED, never a synthetic GREEN.
6. **Candidate immutability** — validation does not silently mutate tracked candidate bytes. Disposable copies/worktrees are preferred for destructive negative controls.
7. **Result binding** — the receipt names the exact SHA, commands/gates executed and final result.
8. **Evidence binding** — any generated evidence used for freeze is either deterministically compared with committed evidence or reconciled into a new evidence-bearing candidate SHA and then rerun read-only on that new exact SHA.

## Receipt format

A receipt may be a repository evidence file, PR comment, attached log, provider job URL, or other durable GitHub-visible record. It must contain enough information for a fresh Reviewer to identify what actually ran.

Minimum fields:

```text
EXECUTION_RECEIPT_V1
WP: <id>
Candidate SHA: <40-char>
Executor role: WORKER | REVIEWER | LOCAL_VALIDATOR
Execution substrate: <worker shell / reviewer shell / local machine / provider>
OS: <identity when material>
Toolchain: <identity when material>
Canonical command: <entrypoint(s)>
Candidate clean before: YES | NO
Candidate clean after: YES | NO
Required gates: <gate=result; ...>
Result: GREEN | RED | BLOCKED
Evidence: <paths/links or NONE>
```

For foundational WPs, `GREEN` is valid only if every required positive proof and causal negative control mandated by the current proof matrix executed successfully inside the declared trust boundary.

## Two-phase evidence reconciliation

When committed evidence must equal an observed inventory/result, use this sequence:

```text
candidate A
→ execute observation on A
→ reconcile deterministic observed evidence into tracked files
→ commit candidate B
→ execute complete validation read-only on exact B
→ committed evidence == observed evidence
→ Worker pre-review
→ freeze B
```

The observation run on A is not the final freeze receipt. Candidate B must be executed again after evidence reconciliation.

## Worker rule

The Worker owns obtaining a compliant execution receipt before freeze. The Worker may use any available compliant substrate; no hosted-CI vendor is privileged.

If the current agent environment cannot execute the required toolchain, it must leave the candidate `NOT_READY` and hand the exact canonical command to a capable execution substrate. It must not weaken proof or fabricate results merely because its current shell is limited.

## Reviewer rule

The independent Reviewer verifies that the frozen SHA has a compliant Worker execution receipt and may rerun validation on its own capable substrate. Reviewer independence applies to judgment and falsification, not to using a different CI vendor.

## Provider neutrality

Provider configuration is replaceable infrastructure. The canonical validation logic belongs in repository scripts/tools so that moving between Worker shell, Reviewer shell, local machine or future CI requires little or no proof rewrite.

A workflow file may wrap the canonical command, but a workflow-only validation rule is architectural debt unless the WP specifically concerns that provider.

## Zero-budget policy

For Juego2:

- automatic GitHub-hosted runner usage is disabled by default;
- exhausting a hosted provider quota must never stop architectural progress when a compliant Worker/Reviewer/local executor exists;
- paid CI is optional convenience, not a prerequisite for Arkus correctness;
- GitHub remains the persistent source of repository state, PR handoff, freeze SHA, review verdict and durable evidence references.
