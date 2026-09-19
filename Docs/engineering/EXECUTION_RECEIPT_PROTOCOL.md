# Exact-SHA Execution Receipt Protocol

Version: 1.1 — 2026-09-19

## Purpose

Arkus validation must be reproducible and bound to the exact candidate being handed off without depending on GitHub Actions or any automatic runner/orchestrator.

GitHub is the durable repository/PR/handoff surface. Execution happens manually in a capable Worker environment, an independent Reviewer environment, Codex Desktop/local PC, or another explicitly invoked environment that can satisfy this protocol.

Juego2 contains no GitHub Actions workflow requirement. Exact-SHA validation is mandatory; hosted CI is not.

## Binding terminology

Where an older repository document says `CI`, `green CI`, `exact-SHA CI`, or equivalent in a validation/freeze requirement, interpret it as **compliant exact-SHA execution + receipt** unless the document explicitly requires a specific product environment.

## Required properties

A validation run is acceptable only when all of the following hold:

1. **Exact candidate** — record a 40-character candidate SHA and verify `git rev-parse HEAD` equals it before validation.
2. **Clean input** — tracked/index/worktree state is clean before execution except paths explicitly designated as ignored observation output.
3. **Declared environment** — OS/runtime/toolchain identities materially required by the WP are recorded or asserted by the canonical validation script.
4. **Canonical commands** — use the WP/repository-owned validation entrypoint; the execution environment may not redefine the proof.
5. **Fail closed** — skipped required commands, unavailable required toolchains or partial evidence produce NOT_READY/BLOCKED, never synthetic GREEN.
6. **Candidate immutability** — validation does not silently mutate tracked candidate bytes. Disposable copies/worktrees are preferred for destructive negative controls.
7. **Result binding** — the receipt names the exact SHA, commands/gates executed and final result.
8. **Evidence binding** — generated evidence used for freeze is either deterministically compared with committed evidence or reconciled into a new evidence-bearing candidate SHA and then rerun read-only on that exact SHA.

## Receipt format

A receipt may be a repository evidence file, PR comment, attached log or another durable GitHub-visible record. It must contain enough information for a fresh Reviewer to identify what actually ran.

Minimum fields:

```text
EXECUTION_RECEIPT_V1
WP: <id>
Candidate SHA: <40-char>
Executor role: WORKER | REVIEWER | LOCAL_VALIDATOR
Execution environment: <worker shell / reviewer shell / local machine>
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

The Worker obtains a compliant execution receipt before freeze. If its current environment cannot execute the required toolchain, it leaves the candidate `NOT_READY` and hands the exact canonical command to a capable manually invoked environment. It must not weaken proof or fabricate results.

## Reviewer rule

The independent Reviewer verifies that the frozen SHA has a compliant Worker execution receipt and may rerun validation in its own capable environment. Reviewer independence applies to judgment and falsification, not to using a particular runner vendor.

## Manual execution policy

For Juego2:

- GitHub Actions workflows are not part of the repository operating model;
- GitHub stores source, PR state, freeze SHA, review verdict and durable evidence references;
- Worker/Reviewer/local environments provide compute when manually invoked;
- no automatic trigger, notification or background runner is a completion gate;
- exact-SHA validation and independent review remain mandatory.
