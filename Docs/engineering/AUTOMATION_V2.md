# Automation V2 — minimal GitHub Actions orchestration

Version: 1.2 — 2026-09-19

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
  -> Worker obtains exact-SHA GREEN receipt
  -> Worker freezes that exact SHA and marks PR Ready
  -> Freeze handoff validates/reuses the same exact-SHA GREEN receipt
     (full verifier runs only when no valid reusable receipt exists)
  -> REVIEW_READY marker
  -> human starts a fresh independent Reviewer
  -> Reviewer emits exact-SHA PASS or FAIL
     FAIL -> REPAIR_REQUIRED marker; human starts fresh repair Worker
     PASS -> exact-SHA merge preflight -> automatic merge
  -> DOCSYNC_REQUIRED marker
  -> human starts DocSync
  -> DOCSYNC_COMPLETE marker with dependency-valid Next WP
  -> next WP may start
```

Only the mechanical parts above are automated. Worker, independent Reviewer and DocSync reasoning remain explicit role invocations unless a future provider integration can prove those role boundaries correctly.

## Workflows

### `.github/workflows/candidate-validation.yml`

Runs PR code with a **read-only token** plus read-only Actions access for receipt reuse.

- Draft PR: observation mode.
- Ready/non-draft PR: frozen-candidate verify/handoff mode.
- Manual `workflow_dispatch`: exact SHA + explicit observation/verify mode.
- `Mode: PROCESS_ONLY` skips product validation.
- If a durable exact-SHA GREEN receipt already exists for the same candidate and passes strict identity/content validation, freeze reuses it instead of paying for an identical full rerun.
- If no valid reusable receipt exists, freeze executes the canonical verifier normally and fails closed on any ambiguity.

Receipt reuse is not trust-by-filename. The workflow verifies the source workflow succeeded, source `head_sha` equals the frozen candidate SHA, downloads the receipt artifact, and checks the receipt binds the exact SHA with clean-before/after YES, required gates GREEN and `Result: GREEN` before skipping execution.

Canonical entrypoint convention for future WPs:

- `scripts/arkus-observe-exact-sha.sh <sha>`
- `scripts/arkus-verify-exact-sha.sh <sha>`

HK00 compatibility is built in while that WP is active:

- `scripts/hk00-observe-exact-sha.sh <sha>`
- `scripts/hk00-verify-exact-sha.sh <sha>`
- reusable receipt artifact: `hk00-receipt-<40-char SHA>`

The workflow uploads logs/receipts/observed artifacts but does not edit the candidate.

### `.github/workflows/state-transitions.yml`

Has write permissions but **never checks out or executes PR code**.

It performs only GitHub-state transitions:

- after successful frozen handoff validation (fresh execution or validated receipt reuse), verifies handoff metadata and emits `REVIEW_READY`;
- on canonical Reviewer `FAIL` for the exact frozen SHA, emits `REPAIR_REQUIRED`;
- on canonical Reviewer `PASS`, verifies the reviewed SHA equals PR HEAD/Frozen SHA, requires a successful `Freeze exact-SHA validation` check, and merges that exact SHA;
- after an implementation merge, emits `DOCSYNC_REQUIRED`.

This separation is deliberate for a public repository: untrusted PR code never receives the workflow token that can write/merge.

### `.github/workflows/telegram-notify.yml`

Optional low-noise notification projection. It never decides state and never executes PR code. It only renders already-persisted `ARKUS_AUTOMATION_V2` markers into Telegram messages.

Normal Telegram notifications are intentionally limited to high-value transitions:

- `REVIEW_READY` -> Worker finished and the exact candidate is ready for an independent Reviewer;
- `PASS_PREFLIGHT_GREEN` -> Reviewer PASS is bound to the exact candidate and merge is authorized;
- `REPAIR_REQUIRED` -> Reviewer FAIL requires a fresh repair Worker on the same WP;
- `DOCSYNC_COMPLETE` -> merge + DocSync are complete and the dependency-valid `Next WP` is included;
- `MILESTONE_COMPLETE`, `BLOCKED` and `HUMAN_ACTION_REQUIRED` are supported for future explicit markers.

`DOCSYNC_REQUIRED` is deliberately not sent to avoid an extra routine notification between merge and final DocSync completion.

The notifier expects repository secrets named:

- `TELEGRAM_BOT_TOKEN`
- `TELEGRAM_CHAT_ID`

If either secret is absent, the job exits successfully after logging that the notification was skipped. Telegram is convenience only; GitHub state remains authoritative.

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

That successful freeze check may represent either a fresh canonical execution or strict reuse of an already-durable exact-SHA GREEN receipt for the identical frozen SHA.

## DocSync completion marker

After post-merge reconciliation is complete, the DocSync/finalization role persists one comment on the merged implementation PR:

```text
ARKUS_AUTOMATION_V2
State: DOCSYNC_COMPLETE
Key: docsync-complete:<PR>:<reconciled-main-sha>
WP: <WP-ID>
Next WP: <dependency-valid next WP, or NONE>
Detail: <short reconciliation result>
```

`Next WP` is derived from current accepted GitHub state after DocSync; it is not chosen by the Telegram workflow.

## Cost policy

Automation V2 uses only standard GitHub-hosted runners by default. Do not introduce larger/paid runners or paid third-party CI as a normal gate without an explicit human decision.

Do not knowingly rerun an expensive exact-SHA proof solely to transition workflow state when a durable GREEN receipt for the identical SHA can be validated without weakening the proof boundary. Reuse is an orchestration optimization, not a semantic shortcut.

If hosted Actions are unavailable, quota/policy changes, or a workflow platform fails, correctness does not disappear: the exact-SHA execution receipt protocol remains a valid manual fallback.

## Non-goals

Automation V2 does not:

- start ChatGPT/Claude/Codex sessions automatically;
- choose a different WP when the requested WP is blocked;
- create hidden dependency routing;
- maintain role leases;
- make Telegram or another notification transport authoritative;
- weaken Worker pre-review, trust-boundary, proof-budget or Reviewer independence rules.

Keep this layer small. If automation starts accumulating product semantics, move that logic back into canonical scripts/contracts or delete the automation.