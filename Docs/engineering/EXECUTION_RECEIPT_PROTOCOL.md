# Exact-SHA Execution Receipt Protocol

Version: 1.2 — 2026-09-19

## Purpose

Arkus validation must be reproducible and bound to the exact candidate being handed off without making any CI provider the semantic authority.

GitHub Actions may provide the default execution substrate when convenient. A capable Worker environment, independent Reviewer environment, Codex Desktop/local PC or other explicitly invoked environment may also satisfy this protocol.

The invariant is exact-SHA execution of repository-owned canonical validation commands with durable evidence. Hosted CI availability is operational infrastructure, not correctness.

## Binding terminology

Where an older repository document says `CI`, `green CI`, `exact-SHA CI`, or equivalent in a validation/freeze requirement, interpret it as **compliant exact-SHA execution + receipt** unless the document explicitly requires a specific product environment.

Automation V2 may produce that receipt automatically. Manual execution remains a valid fallback.

## Required properties

A validation run is acceptable only when all of the following hold:

1. **Exact candidate** — record a 40-character candidate SHA and verify `git rev-parse HEAD` equals it before validation.
2. **Clean input** — tracked/index/worktree state is clean before execution except paths explicitly designated as ignored observation output.
3. **Declared environment** — OS/runtime/toolchain identities materially required by the WP are recorded or asserted by the canonical validation script.
4. **Canonical commands** — use the WP/repository-owned validation entrypoint. Workflow YAML, runner images and provider configuration may orchestrate but may not redefine the proof.
5. **Fail closed** — skipped required commands, unavailable required toolchains or partial evidence produce NOT_READY/BLOCKED/RED, never synthetic GREEN.
6. **Candidate immutability** — validation does not silently mutate tracked candidate bytes. Disposable copies/worktrees are preferred for destructive negative controls.
7. **Result binding** — the receipt names the exact SHA, commands/gates executed and final result.
8. **Evidence binding** — generated evidence used for freeze is either deterministically compared with committed evidence or reconciled into a new evidence-bearing candidate SHA and then rerun read-only on that exact SHA.

## Receipt format

A receipt may be a repository evidence file, workflow artifact/log, PR comment or another durable GitHub-visible record. It must contain enough information for a fresh Reviewer to identify what actually ran.

Minimum fields:

```text
EXECUTION_RECEIPT_V1
WP: <id>
Candidate SHA: <40-char>
Executor role: WORKER | REVIEWER | LOCAL_VALIDATOR
Execution environment: <GitHub Actions / worker shell / reviewer shell / local machine>
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

## Automation V2 integration

`Docs/engineering/AUTOMATION_V2.md` defines the repository's minimal GitHub Actions orchestration.

The validation workflow supports a generic future convention:

```text
scripts/arkus-observe-exact-sha.sh <sha>
scripts/arkus-verify-exact-sha.sh <sha>
```

While HK00 is active, its existing exact-SHA wrappers are accepted compatibility entrypoints.

Draft/mutable Worker state may use observation mode. A frozen candidate requires verify mode and a `GREEN` exact-SHA receipt before review handoff.

## Worker rule

The Worker owns obtaining a compliant exact-SHA receipt before freeze. Automation V2 should normally provide the hosted execution when the PR reaches the corresponding state, but the Worker remains responsible for ensuring the exact candidate actually passed the required gates.

If hosted Actions are unavailable, use another capable environment rather than weakening proof or fabricating results.

## Reviewer rule

The independent Reviewer verifies that the frozen SHA has a compliant Worker execution receipt and may rerun validation in its own capable environment. Reviewer independence applies to judgment and falsification, not to using a different runner vendor.

A green workflow never converts Worker evidence into independent Reviewer PASS by itself.

## Hosted-runner policy

For Juego2:

- standard GitHub-hosted runners may be used for Automation V2 validation and mechanical transitions;
- repository code executed from a PR receives only read permissions;
- workflows with write/merge permissions must not checkout or execute PR code;
- larger/paid runners and paid third-party CI are not normal dependencies and require an explicit human decision;
- GitHub Actions outage or future pricing/quota change is recoverable infrastructure backpressure, not implementation FAIL;
- exact-SHA manual execution remains the fallback.
