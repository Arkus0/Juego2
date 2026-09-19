# Automation V2 — minimal GitHub Actions orchestration

Version: 1.0 — 2026-09-19

## Purpose

Juego2 uses GitHub Actions again because the repository is public and standard hosted runners can provide practical validation compute without consuming the previous private-repository minute budget.

Automation V2 is intentionally small. It automates mechanical validation and state transitions; it does **not** become the semantic authority for Arkus and it does not spawn or impersonate independent AI roles.

There is no automation bootstrap, no role lease system, no dependency-routing daemon, no background Worker farm and no requirement to use a particular model/provider.

## Authority

The authority order remains:

1. exact WP contract and accepted architecture;
2. repository code + canonical validation scripts;
3. exact candidate SHA and durable evidence;
4. Worker pre-review;
5. independent Reviewer verdict on that exact SHA.

Workflow YAML is replaceable orchestration. It may call canonical repository entrypoints but must not redefine their proof semantics.

## Automated flow

```text
Draft Worker PR
  -> Candidate observation on every relevant PR update
  -> Worker fixes/reconciles evidence
  -> Worker pre-review CLEAN
  -> Worker freezes exact SHA and marks PR Ready
  -> Freeze exact-SHA validation
  -> REVIEW_READY marker
  -> human starts a fresh independent Reviewer
  -> Reviewer emits exact-SHA PASS or FAIL
     FAIL -> REPAIR_REQUIRED marker; human starts fresh repair Worker
     PASS -> exact-SHA merge preflight -> automatic merge
  -> DOCSYNC_REQUIRED marker
  -> human starts DocSync
  -> next WP only after DocSync
```

Only the mechanical parts above are automated. Worker, independent Reviewer and DocSync reasoning remain explicit role invocations unless a future provider integration can prove those role boundaries correctly.

## Workflows

### `.github/workflows/candidate-validation.yml`

Runs PR code with a **read-only token**.

- Draft PR: observation mode.
- Ready/non-draft PR: frozen-candidate verify mode.
- Manual `workflow_dispatch`: exact SHA + explicit observation/verify mode.
- `Mode: PROCESS_ONLY` skips product validation.

Canonical entrypoint convention for future WPs:

- `scripts/arkus-observe-exact-sha.sh <sha>`
- `scripts/arkus-verify-exact-sha.sh <sha>`

HK00 compatibility is built in while that WP is active:

- `scripts/hk00-observe-exact-sha.sh <sha>`
- `scripts/hk00-verify-exact-sha.sh <sha>`

The workflow uploads logs/receipts/observed artifacts but does not edit the candidate.

### `.github/workflows/state-transitions.yml`

Has write permissions but **never checks out or executes PR code**.

It performs only GitHub-state transitions:

- after successful frozen validation, verifies handoff metadata and emits `REVIEW_READY`;
- on canonical Reviewer `FAIL` for the exact frozen SHA, emits `REPAIR_REQUIRED`;
- on canonical Reviewer `PASS`, verifies the reviewed SHA equals PR HEAD/Frozen SHA, requires a successful `Freeze exact-SHA validation` check, and merges that exact SHA;
- after an implementation merge, emits `DOCSYNC_REQUIRED`.

This separation is deliberate for a public repository: untrusted PR code never receives the workflow token that can write/merge.

## Reviewer handoff contract

Automation recognizes a Reviewer transition only when the review body contains both:

```text
Reviewer verdict: PASS | FAIL
Reviewed candidate SHA: <40-char>
```

The GitHub author must be OWNER, MEMBER or COLLABORATOR. GitHub identity alone cannot prove session independence; the binding `WORKER_REVIEW_PROTOCOL.md` still requires a genuinely fresh independent Reviewer context.

A `PASS` cannot merge unless all of these agree:

- PR HEAD;
- `Candidate HEAD SHA`;
- `Frozen candidate SHA`;
- `Reviewed candidate SHA`;
- successful `Freeze exact-SHA validation` check.

## Cost policy

Automation V2 uses only standard GitHub-hosted runners by default. Do not introduce larger/paid runners or paid third-party CI as a normal gate without an explicit human decision.

If hosted Actions are unavailable, quota/policy changes, or a workflow platform fails, correctness does not disappear: the exact-SHA execution receipt protocol remains a valid manual fallback.

## Non-goals

Automation V2 does not:

- start ChatGPT/Claude/Codex sessions automatically;
- choose a different WP when the requested WP is blocked;
- create hidden dependency routing;
- maintain role leases;
- require Telegram or another notification transport;
- weaken Worker pre-review, trust-boundary, proof-budget or Reviewer independence rules.

Keep this layer small. If automation starts accumulating product semantics, move that logic back into canonical scripts/contracts or delete the automation.
