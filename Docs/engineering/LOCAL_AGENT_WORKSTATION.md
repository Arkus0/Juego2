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

From the repository root, first inspect the effective PowerShell execution policy and the policy at every scope:

```powershell
Get-ExecutionPolicy
Get-ExecutionPolicy -List
```

On a Windows client, if every scope is `Undefined`, PowerShell's effective default can be `Restricted`, which blocks `.ps1` files before the repository checker can start. Do not persistently weaken the workstation policy just to run this repository check.

If the effective policy already permits local scripts, run the checker directly:

```powershell
.\scripts\local-agent-check.ps1
```

For the fresh/default `Restricted` case with no `MachinePolicy` or `UserPolicy` imposed by Group Policy, use a new PowerShell process with a process-only `RemoteSigned` policy:

```powershell
powershell.exe -NoProfile -ExecutionPolicy RemoteSigned -File .\scripts\local-agent-check.ps1
```

If you use PowerShell 7 instead of Windows PowerShell, the equivalent is:

```powershell
pwsh -NoProfile -ExecutionPolicy RemoteSigned -File .\scripts\local-agent-check.ps1
```

`-ExecutionPolicy` on that new process affects only that PowerShell session and disappears when the process exits; it does not rewrite `CurrentUser` or `LocalMachine` policy. `MachinePolicy` and `UserPolicy` Group Policy have higher precedence and cannot be overridden this way. If either is defined and blocks the script, stop and follow the workstation/organization policy or ask its administrator; do not work around enforced Group Policy.

For a workpack whose exact Unity editor requirement is already known and installed, use the same permitted path with `-RequireUnity`:

```powershell
.\scripts\local-agent-check.ps1 -RequireUnity
```

or, for the fresh/default `Restricted` case:

```powershell
powershell.exe -NoProfile -ExecutionPolicy RemoteSigned -File .\scripts\local-agent-check.ps1 -RequireUnity
```

PowerShell 7 equivalent:

```powershell
pwsh -NoProfile -ExecutionPolicy RemoteSigned -File .\scripts\local-agent-check.ps1 -RequireUnity
```

The check is intentionally read-only. It reports missing prerequisites; it does not install software, mutate PRs, create branches, change execution policy persistently or create a Unity project.

## Unity rule for H1-02

Do **not** manually create an ad-hoc Juego2 Unity project before H1-02. `WP-H1-02` owns the reproducible Unity project/toolchain/package baseline, including selection of the exact maintained Unity 6.3 LTS patch, package lock, Force Text/meta policy, assembly direction and canonical batch launch recipe.

It is fine to have Unity Hub installed before starting. If the exact editor patch is not yet installed, the H1-02 Worker must first resolve the contractual exact patch and then use/install that version before claiming local Unity evidence.

## Starting a normal local Worker

After the prospective local-autopilot process amendment is independently accepted and DocSync-complete, the owner may instead use the opt-in, ChatGPT-subscription-only driver in `Docs/engineering/LOCAL_WP_AUTOPILOT.md`. It launches fresh role sessions and uses GitHub markers to route them; it does not weaken this manual Worker/Reviewer path. Run its `--dry-run` first and pilot one WP with `--one-wp` before using the default continuous mode. Do not use it to take over an already active PR without confirming the current role has stopped.

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
- Do not weaken or bypass an organization-enforced `MachinePolicy` or `UserPolicy`; the process-scoped `RemoteSigned` invocation above is only for the ordinary fresh/default Windows `Restricted` case when Group Policy is not imposing the effective policy.

## Daily start checklist

From `Juego2`:

```powershell
git fetch origin
Get-ExecutionPolicy -List
.\scripts\local-agent-check.ps1
gh pr list
```

If direct `.ps1` execution is blocked only by the fresh/default Windows `Restricted` policy and no Group Policy scope is defined, substitute the documented process-scoped invocation:

```powershell
powershell.exe -NoProfile -ExecutionPolicy RemoteSigned -File .\scripts\local-agent-check.ps1
```

Then start a fresh Codex local session and use the explicit role command required for the work you intend to perform.

For H1-02 specifically, do not create the Unity project by hand. Let the workpack create/pin the reproducible project substrate and preserve all required evidence in the repository/GitHub handoff.
