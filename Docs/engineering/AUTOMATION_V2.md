# Automation V2 — minimal GitHub Actions orchestration

Version: 1.7 — 2026-09-23

## Purpose

Juego2 uses GitHub Actions because the repository is public and standard hosted runners can provide practical validation compute without consuming the previous private-repository minute budget.

Automation V2 is intentionally small. It automates mechanical validation and state transitions; it does **not** become the semantic authority for Arkus and it does not spawn or impersonate independent AI roles. The optional, prospectively adopted **local** driver in `LOCAL_WP_AUTOPILOT.md` may start fresh role sessions after checking these markers. It is not part of GitHub Actions, a role lease, a background Worker farm or a new proof authority. Manual role starts remain supported.

## Authority

The authority order remains:

1. exact WP contract and accepted architecture;
2. repository code + canonical validation scripts;
3. exact candidate SHA and durable evidence;
4. Worker pre-review;
5. independent Reviewer verdict on that exact SHA.

Workflow YAML is replaceable orchestration. It may call canonical repository entrypoints but must not redefine their proof semantics.

For cadence, same-`PRODUCT_SHA` metadata closure, Reviewer protocol-only classification, direct `REVIEW_READY` handoff and zero-commit DocSync, `PRODUCT_SHA_CLOSURE.md` supersedes older descriptions in this document.

## Automated flow

```text
Draft Worker PR
  -> Main Safety on each material push; WP-specific observation only when requested
  -> Worker fixes/reconciles evidence
  -> Worker consumes same-PR/same-SHA Main Safety GREEN (or explicit fallback preflight)
  -> Worker pre-review CLEAN at exact PRODUCT_SHA
  -> Worker freezes that exact SHA and marks PR Ready
  -> Worker handoff lint mechanically validates the canonical Ready handoff
  -> Candidate Validation performs exact-SHA freeze verification
  -> context-bound REVIEW_READY marker (terminal handoff; no closure pass)
  -> human or opt-in local driver starts a fresh independent Reviewer
  -> Reviewer emits exact-SHA PASS / material FAIL, or protocol-only status
     FAIL -> REPAIR_REQUIRED marker; human or local driver starts fresh repair Worker
     PROTOCOL_FIX -> same-SHA metadata correction; no product rerun
     PASS -> exact-SHA merge preflight -> automatic merge
          -> DOCSYNC_REQUIRED after confirmed merge for Worker-lifecycle WPs
          -> successful Reviewer/finalizer performs bounded delta DocSync
          -> DOCSYNC_COMPLETE marker with dependency-valid Next WP
          -> human or local driver starts the next Worker
```

GitHub Actions still automates only the mechanical validation/state-transition parts above. The optional local driver automates **session launch/routing**, not Worker/Reviewer judgment. Routine post-PASS DocSync may continue in the successful Reviewer session or in a dedicated finalization session, but it may never alter implementation bytes or reconsider the accepted candidate.

The optional `telegram-continue.yml` workflow is a narrow **owner-control bridge** activated only for a second-FAIL decision from the local driver. It uses the existing Telegram bot secrets, accepts one callback from the configured private owner chat for the exact PR/SHA/fail count, and persists `OWNER_CONTINUE` as a routing marker. It does not issue a Reviewer verdict, merge, repair, or change product semantics. The fourth FAIL has no callback path. No always-on bot listener or model API key is introduced.

Telegram remains an optional handoff surface for manual role starts: high-value transitions may include a short copyable ChatGPT prompt and links to ChatGPT and the relevant PR. The prompt is convenience only and always instructs the new session to reconstruct GitHub state instead of trusting Telegram context. The opt-in local driver instead consumes the durable GitHub transition directly; only the bounded second-FAIL button changes its route.

## Workflows

### `.github/workflows/candidate-validation.yml`

Runs PR code with a **read-only token** plus read-only Actions access for receipt reuse.

- Draft PR: no automatic Candidate Validation; Main Safety covers material pushes. Observation is explicit.
- Ready/non-draft PR: frozen-candidate verify/handoff mode.
- Ready PRs also run the separate `Worker handoff lint` job.
- Manual `workflow_dispatch`: exact SHA + explicit observation/verify mode.
- `Mode: PROCESS_ONLY` skips product/runtime validation, but it does **not** skip Worker handoff lint when the PR publishes Worker lifecycle fields.
- A PROCESS_ONLY run still verifies exact checkout identity and clean repository input before classifying product/runtime execution as `NOT_APPLICABLE`.
- When PROCESS_ONLY has no canonical product/runtime command to execute, Automation V2 emits a `PROCESS_ONLY_VALIDATION_V1` N/A log and **does not emit `EXECUTION_RECEIPT_V1`, `Result: GREEN`, clean-before/after receipt claims or a synthetic gate result**.
- Pure PROCESS_ONLY maintenance/DocSync PRs that never enter the Worker lifecycle are intentionally allowed to omit the Worker handoff block.
- If a durable exact-SHA GREEN receipt already exists for the same candidate and passes strict identity/content validation, freeze reuses it instead of paying for an identical full rerun.
- If no valid reusable receipt exists where canonical execution is required, freeze executes the canonical verifier normally and fails closed on any ambiguity.

The PROCESS_ONLY N/A path is deliberately not an execution receipt. `EXECUTION_RECEIPT_V1` is reserved for runs that actually execute the canonical command/gates represented by that receipt. Exact checkout cleanliness may still be mechanically verified by the workflow, but it cannot be promoted into evidence that unexecuted proof gates were GREEN.

`Worker handoff lint` is deliberately mechanical. It checks the canonical PR handoff fields, Ready/frozen state values, exact SHA coherence, pointer existence, the presence of `PREDECESSOR_CONTRACT_CHECK` in repository-local predecessor evidence when supplied, and `WORKER_PRE_REVIEW: CLEAN` in repository-local pre-review evidence. It does **not** judge whether the predecessor reasoning, negative controls, proof arguments or semantic acceptance claims are substantively sufficient; that remains Worker pre-review + independent Reviewer work.

The linter also fails closed when a non-PROCESS_ONLY Ready PR has no canonical Worker handoff at all. For PROCESS_ONLY PRs, publishing any Worker-lifecycle surface opts the PR into the same complete handoff validation, which is how CTX workpacks remain real Worker → Reviewer cycles while pure maintenance PRs stay lightweight.

Receipt reuse is not trust-by-filename. The workflow verifies the source workflow succeeded, source `head_sha` equals the frozen candidate SHA, downloads the receipt artifact, and checks the receipt binds the exact SHA with clean-before/after YES, required gates GREEN and `Result: GREEN` before skipping execution. PROCESS_ONLY N/A runs do not participate in receipt reuse because they produce no execution receipt.

Canonical entrypoint convention for future WPs:

- `scripts/arkus-observe-exact-sha.sh <sha>`
- `scripts/arkus-verify-exact-sha.sh <sha>`

HK00 compatibility is built in while that WP is active:

- `scripts/hk00-observe-exact-sha.sh <sha>`
- `scripts/hk00-verify-exact-sha.sh <sha>`
- reusable receipt artifact: `hk00-receipt-<40-char SHA>`

The workflow uploads validation logs and observed artifacts. It uploads an execution receipt only when canonical execution actually produced one; it does not edit the candidate.

### `.github/workflows/state-transitions.yml`

Has write permissions but **never checks out or executes PR code**.

It performs only GitHub-state transitions:

- after a successful `Worker handoff lint`, verifies handoff metadata and emits `REVIEW_READY`; foundational candidates additionally require their successful frozen exact-SHA validation;
- non-foundational candidates do not acquire foundational proof/exact-SHA obligations merely because handoff lint exists;
- PROCESS_ONLY PRs with Worker lifecycle fields participate normally in `REVIEW_READY` / PASS merge / DocSync transitions, while pure PROCESS_ONLY maintenance/DocSync PRs without Worker lifecycle fields remain outside those transitions;
- on canonical Reviewer `FAIL` for the exact frozen SHA, emits `REPAIR_REQUIRED`;
- on canonical Reviewer `PASS`, verifies the reviewed SHA equals PR HEAD/Frozen SHA, requires a successful `Worker handoff lint`, additionally requires successful `Freeze exact-SHA validation` where the candidate is not non-foundational, and merges that exact SHA;
- after a Worker-lifecycle WP merge, emits `DOCSYNC_REQUIRED` as durable machine state for recovery/audit.

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

Pure PROCESS_ONLY maintenance/DocSync PRs without Worker lifecycle fields are excluded from the post-merge DocSync action. PROCESS_ONLY workpacks that do publish the canonical Worker lifecycle — including CTX — are not excluded merely because their product/runtime scope is zero.

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
- successful `Worker handoff lint` check.

For candidates whose class still requires foundational exact-SHA proof, a successful `Freeze exact-SHA validation` check is additionally required. Non-foundational WPs retain their accepted lighter proof boundary; handoff lint validates process structure and does not silently promote them into the foundational proof regime.

A successful freeze check may represent either a fresh canonical execution or strict reuse of an already-durable exact-SHA GREEN receipt for the identical frozen SHA. PROCESS_ONLY N/A validation is not such a receipt and cannot satisfy a proof obligation that actually requires canonical execution.

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

The handoff lint is intentionally cheap and bounded: it exists to prevent avoidable Reviewer cycles caused by missing or incoherent process metadata, not to grow into a second semantic review engine.

PROCESS_ONLY paths that require no product/runtime execution should also avoid installing toolchains or invoking receipt-reuse machinery merely to manufacture an N/A result. Their cheap mechanical gates remain exact-checkout verification plus handoff lint where the Worker lifecycle applies.

If hosted Actions are unavailable, quota/policy changes, or a workflow platform fails, correctness does not disappear: the exact-SHA execution receipt protocol remains a valid manual fallback where that proof regime applies, and the canonical Worker/Reviewer protocol remains authoritative.

## Non-goals

Automation V2 does not:

- start ChatGPT/Claude/Codex Worker or Reviewer sessions automatically;
- perform semantic DocSync reasoning by itself;
- choose a different WP when the requested WP is blocked;
- create hidden dependency routing;
- maintain role leases;
- make Telegram or another notification transport authoritative;
- decide that a declared control is semantically sufficient merely because a field/path exists;
- fabricate execution receipts or GREEN gate claims for commands that were not executed;
- weaken Worker pre-review, trust-boundary, proof-budget or Reviewer independence rules.

Keep this layer small. If automation starts accumulating product semantics, move that logic back into canonical scripts/contracts or delete the automation.
