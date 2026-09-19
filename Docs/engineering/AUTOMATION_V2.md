# Automation V2 — minimal GitHub Actions orchestration

Version: 1.4 — 2026-09-19

## Purpose

Juego2 uses GitHub Actions because the repository is public and standard hosted runners can provide practical validation compute without consuming the previous private-repository minute budget.

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
          -> DOCSYNC_REQUIRED after confirmed merge
          -> successful Reviewer/finalizer performs documentation-only DocSync
          -> DOCSYNC_COMPLETE marker with dependency-valid Next WP
          -> human starts the next Worker
```

Only the mechanical validation/state-transition parts above are automated. Worker and independent Reviewer reasoning remain explicit. Routine post-PASS DocSync may continue in the successful Reviewer session or in a dedicated finalization session, but it may never alter implementation bytes or reconsider the accepted candidate.

Telegram is an optional control surface for these human-started reasoning sessions: high-value transitions may include a short copyable ChatGPT handoff prompt plus links to ChatGPT and the relevant PR. The prompt is convenience only and always instructs the new session to reconstruct GitHub state instead of trusting Telegram context.

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
- after an implementation merge, emits `DOCSYNC_REQUIRED` as durable machine state for recovery/audit.

`DOCSYNC_REQUIRED` is the first safe point for a Telegram DocSync action because the merge is already confirmed. A successful Reviewer session may continue into finalization, or the human may start a dedicated finalization/DocSync session from the Telegram handoff. Either way, `WORKER_REVIEW_PROTOCOL.md` rules remain binding.

This separation is deliberate for a public repository: untrusted PR code never receives the workflow token that can write/merge.

### `.github/workflows/telegram-notify.yml`

Optional notification/control projection. It never decides acceptance state and never executes PR code. It renders already-persisted `ARKUS_AUTOMATION_V2` markers and the objective merged-PR event into Telegram messages.

Normal Telegram notifications cover the high-value transitions:

- `REVIEW_READY` -> Worker finished; includes a short prompt for a fresh independent Reviewer;
- `PASS_PREFLIGHT_GREEN` -> Reviewer PASS is bound to the exact candidate and automatic merge is in progress; status only, so the user does not race the merge;
- `REPAIR_REQUIRED` -> Reviewer FAIL; includes a short prompt for a fresh repair Worker on the same PR/WP;
- `DOCSYNC_REQUIRED` -> merge is confirmed; includes a finalization/DocSync prompt;
- `DOCSYNC_COMPLETE` -> merge + DocSync are complete and the dependency-valid `Next WP` is included; when `Next WP != NONE`, includes a prompt for the next Worker;
- `BLOCKED` and `HUMAN_ACTION_REQUIRED` -> includes a read-only diagnostic prompt;
- `MILESTONE_COMPLETE` remains informational.

Actionable notifications use a Telegram inline keyboard:

- `📋 Copiar siguiente prompt` copies a compact handoff prompt;
- `🤖 Abrir ChatGPT` opens `https://chatgpt.com/` (the platform/client decides whether that resolves to the app or web);
- `🔗 Abrir PR` opens the authoritative GitHub surface.

Telegram Bot API `copy_text` is limited to 256 characters, so handoff prompts intentionally carry only stable identifiers and role intent. They do **not** embed review findings, repository state or long instructions. Every prompt requires the receiving ChatGPT session to reconstruct current GitHub state and read the authoritative WP/PR evidence before acting.

No OpenAI API key is used by this workflow. Telegram does not send a prompt into ChatGPT automatically; the human copies the prompt, opens ChatGPT and submits it in the normal app/chat experience.

The notifier expects repository secrets named:

- `TELEGRAM_BOT_TOKEN`
- `TELEGRAM_CHAT_ID`

If either secret is absent, the job exits successfully after logging that the notification was skipped. Telegram is convenience only; GitHub state remains authoritative.

`Mode: PROCESS_ONLY` merged PRs are excluded from the post-merge DocSync action because process-only changes do not represent a product-WP implementation merge.

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

After PASS is persisted and these conditions hold, automation may merge immediately. Finalization then treats the verdict as fixed and may perform only post-merge documentation/finalization actions.

## DocSync completion marker

After post-merge reconciliation is complete, the finalization/DocSync phase persists one comment on the merged implementation PR:

```text
ARKUS_AUTOMATION_V2
State: DOCSYNC_COMPLETE
Key: docsync-complete:<PR>:<reconciled-main-sha>
WP: <WP-ID>
Next WP: <dependency-valid next WP, or NONE>
Detail: <short reconciliation result>
```

`Next WP` is derived from current accepted GitHub state after DocSync; it is not chosen by the Telegram workflow. Telegram merely uses the already-recorded value to offer the next Worker handoff.

## Cost policy

Automation V2 uses only standard GitHub-hosted runners by default. Do not introduce larger/paid runners or paid third-party CI as a normal gate without an explicit human decision.

Do not knowingly rerun an expensive exact-SHA proof solely to transition workflow state when a durable GREEN receipt for the identical SHA can be validated without weakening the proof boundary. Reuse is an orchestration optimization, not a semantic shortcut.

If hosted Actions are unavailable, quota/policy changes, or a workflow platform fails, correctness does not disappear: the exact-SHA execution receipt protocol remains a valid manual fallback.

## Non-goals

Automation V2 does not:

- start ChatGPT/Claude/Codex Worker or Reviewer sessions automatically;
- perform semantic DocSync reasoning by itself;
- choose a different WP when the requested WP is blocked;
- create hidden dependency routing;
- maintain role leases;
- make Telegram or another notification transport authoritative;
- weaken Worker pre-review, trust-boundary, proof-budget or Reviewer independence rules.

Keep this layer small. If automation starts accumulating product semantics, move that logic back into canonical scripts/contracts or delete the automation.
