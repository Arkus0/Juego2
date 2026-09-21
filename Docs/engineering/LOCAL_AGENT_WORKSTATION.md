# Local Agent Workstation — Codex + GitHub CLI + Unity

Status: PROCESS / OPERATOR GUIDE

This document prepares a Windows workstation for Juego2 workpacks that require local execution. It does not authorize a workpack, select a Unity version, create the Unity project, or replace `WORKER_REVIEW_PROTOCOL.md`.

## Goal

After one-time workstation setup, the intended human interaction is deliberately small:

```text
Worker H1-02
```

or, after an independent Reviewer FAIL in a fresh local session:

```text
Corrige el FAIL de H1-02
```

The repository owns the detailed procedure through `AGENTS.md` and `.agents/skills/**`.

## What each tool does

- `git` owns local repository bytes, branches, commits, fetch/pull/push and history.
- `gh` (GitHub CLI) owns live GitHub state: pull requests, reviews/comments, checks, draft/ready state and GitHub API queries.
- Codex local reads the repository instructions and can invoke the local tools available to the session.
- Unity/Unity Hub supplies the local engine/editor required by `LOCAL_UNITY_REQUIRED` and `HYBRID` workpacks.

A local Git checkout alone cannot tell an agent that an independent Reviewer issued FAIL, which SHA was reviewed, whether CI is green, or whether a PR is Draft/Ready. Those are GitHub facts; local agents must query live GitHub state through authenticated `gh` or an equivalent live GitHub surface.

## One-time Windows setup

### 1. Git

Install Git for Windows if needed and verify:

```powershell
git --version
```

### 2. GitHub CLI

Install the official GitHub CLI with Windows Package Manager:

```powershell
winget install --id GitHub.cli --source winget
```

Close and reopen the terminal after installation if `gh` is not immediately on `PATH`.

Verify:

```powershell
gh --version
```

Authenticate once:

```powershell
gh auth login
```

Recommended interactive choices for this repository are GitHub.com, HTTPS and browser authentication. Do not paste passwords or personal access tokens into repository files, `AGENTS.md`, prompts or committed `.env` files.

Verify authentication:

```powershell
gh auth status
```

Optionally configure Git to use the credential managed by GitHub CLI:

```powershell
gh auth setup-git
```

### 3. Clone Juego2

Use one canonical working checkout, for example:

```powershell
mkdir C:\Arkus
cd C:\Arkus
gh repo clone Arkus0/Juego2
cd Juego2
```

Equivalent `git clone` is also valid.

Verify that both Git and GitHub resolve the expected repository:

```powershell
git remote get-url origin
gh repo view --json nameWithOwner
```

The latter must resolve to:

```text
Arkus0/Juego2
```

### 4. Run the repository readiness check

From the repository root:

```powershell
.\scripts\local-agent-check.ps1
```

For a workpack whose exact Unity editor requirement is already known and installed:

```powershell
.\scripts\local-agent-check.ps1 -RequireUnity
```

The check is intentionally read-only. It reports missing prerequisites; it does not install software, mutate PRs, create branches or create a Unity project.

## Unity rule for H1-02

Do **not** manually create an ad-hoc Juego2 Unity project before H1-02. `WP-H1-02` owns the reproducible Unity project/toolchain/package baseline, including selection of the exact maintained Unity 6.3 LTS patch, package lock, Force Text/meta policy, assembly direction and canonical batch launch recipe.

It is fine to have Unity Hub installed before starting. If the exact editor patch is not yet installed, the H1-02 Worker must first resolve the contractual exact patch and then use/install that version before claiming local Unity evidence.

## Starting a normal local Worker

Open a **fresh Codex local session rooted at the Juego2 checkout** and issue the shorthand role request:

```text
Worker H1-02
```

The expected behavior is:

```text
read AGENTS.md
→ resolve Docs/workpacks/H1/WP-H1-02.md
→ reconstruct current main + dependencies + live GitHub ownership
→ use implement-workpack workflow
→ create or continue the single canonical Draft PR
→ implement only H1-02
→ run required local Unity/evidence
→ strict Worker pre-review
→ commit + push
→ freeze exact candidate SHA
→ mark Ready
→ STOP for fresh independent review
```

If the requested WP is blocked, the Worker reports the blocking prerequisite and stops rather than silently selecting another WP.

## Repair after an independent FAIL

Use a **new local Codex session**. Do not continue the Reviewer context.

Issue:

```text
Corrige el FAIL de H1-02
```

The expected behavior is:

```text
query live GitHub state with gh
→ locate the canonical H1-02 PR
→ read latest independent FAIL + exact reviewed SHA
→ verify REPAIR_REQUIRED / correct ownership
→ return PR to Draft + ACTIVE as required
→ repair the causal blocker on the same WP
→ rerun affected evidence + complete Worker pre-review
→ commit + push
→ freeze a new exact candidate SHA
→ mark Ready
→ STOP for a new independent Reviewer
```

The repair Worker must not erase the history of the failed candidate, reset `fail_cycle`, self-review, merge or start the next WP.

## Useful GitHub CLI commands

These are examples for understanding/debugging; the local agent may use structured JSON variants as appropriate.

```powershell
# Confirm repository identity
gh repo view --json nameWithOwner

# Open PRs in this repository
gh pr list

# Inspect a PR and discussion
gh pr view 123
gh pr view 123 --comments

# Inspect structured PR state
gh pr view 123 --json number,title,state,isDraft,headRefName,headRefOid,reviews,comments,statusCheckRollup

# Check Actions/status checks for the PR
gh pr checks 123

# Create a Draft PR after pushing a new Worker branch
gh pr create --draft

# Convert the frozen Draft to Ready when the protocol permits
gh pr ready 123
```

`git` remains the authority for local files/commits; `gh` is the live GitHub control surface. An agent should use both rather than treating either as a replacement for the other.

## Security boundary

- Authenticate `gh` interactively yourself.
- Never give Codex your GitHub password.
- Never commit GitHub tokens, Unity credentials or other secrets.
- Local agents may invoke `gh` under your already-authenticated OS credential context; this is equivalent to allowing that local session to perform the GitHub operations your account permits.
- Before any GitHub mutation, the agent must verify repository identity and canonical WP/PR ownership.

## Daily start checklist

From `Juego2`:

```powershell
git fetch origin
.\scripts\local-agent-check.ps1
gh pr list
```

Then start a fresh Codex local session and use the explicit role command required for the work you intend to perform.

For H1-02 specifically, do not create the Unity project by hand. Let the workpack create/pin the reproducible project substrate and preserve all required evidence in the repository/GitHub handoff.
