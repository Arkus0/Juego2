# Local WP autopilot (opt-in)

Version: 0.1 — 2026-09-23

## Adoption and authority

This is an owner-authorized, **prospective** local driver. It becomes available only after the process PR that introduces it receives a fresh independent PASS, merges, and completes DocSync. It does not take over a running/frozen PR or retroactively alter any existing review. Until then, `AGENTS.md`, `WORKER_REVIEW_PROTOCOL.md`, and Automation V2 continue to govern current candidates.

The driver is not a semantic authority. Live GitHub PR/head/review/check/marker state, exact WP contracts, repository code/tests, and the independent Reviewer remain authoritative. A local log or a Luna classification cannot create PASS. The driver never writes a PASS, merges a PR, edits implementation, or chooses a different WP against a `DOCSYNC_COMPLETE` handoff. Existing GitHub Actions own the mechanical transitions and exact-SHA merge.

## Invocation

Use a **dedicated clean local Git worktree** with `origin` set to `Arkus0/Juego2`, `gh` authenticated as repository owner `Arkus0`, and Codex CLI logged in via **ChatGPT subscription**. No OpenAI/OpenRouter/other model API key is used or needed. The driver rejects any Codex account type except `chatgpt`, ignores user model-provider configuration and removes common model API-key variables from its child environment. Telegram uses the already configured repository workflow/secrets; its bot token never enters the local driver.

```powershell
python scripts/local_wp_autopilot.py --root . --wp H1-03 --dry-run
python scripts/local_wp_autopilot.py --root . --wp H1-03 --one-wp
python scripts/local_wp_autopilot.py --root . --next
```

The first two commands are the recommended pilot **after this process PR's independent PASS, merge and DocSync**: inspect the read-only route, then complete one explicitly selected dependency-valid WP with `--one-wp`. The current H1 default is H1-03. The latest cross-track DocSync handoff says `NONE`, so `--next` currently stops rather than inferring H1-03 from unrelated track state; use explicit `--wp H1-03` for the first run. After H1-03's own merged `DOCSYNC_COMPLETE` names a dependency-valid successor, the normal third command continues automatically while quota permits. `--adopt` is an explicit recovery switch for an existing open PR **only after its current Worker/Reviewer has stopped**; it does not relax one-active-Worker ownership. Adoption and every subsequent role require a clean local checkout on the canonical same-repository PR branch at its exact live HEAD; the controller does not silently switch branches or relaunch over an untagged manual verdict. A dirty worktree, ambiguous WP/PR, missing marker, invalid SHA, missing auth/quota data, unavailable model/effort, or local CLI/GitHub failure stops the driver and requests attention. The default local state directory takes an OS-released single-process lock; using a different `--state` is not a safe way to start another owner of the same worktree.

`--next` follows the most recent **owner-authored** live merged `DOCSYNC_COMPLETE` marker only after checking its PR/WP/key, post-merge timestamp and key commit on current main. Under `PRODUCT_SHA_CLOSURE.md`, a zero-commit DocSync keys the accepted PR merge SHA; a real DocSync commit must descend from that merge and change authoritative documentation only. Neither path requires a ceremonial index refresh. If no handoff exists or it points to an already-owned PR, stop; use `--wp` only after verifying the roadmap/dependencies. The cheap precheck requires each named WP dependency to be merged and DocSync-evidenced; non-WP prerequisites require explicit verification. This is not acceptance proof: the Worker still performs mandatory predecessor and live GitHub reconstruction and stops if the WP is blocked. The dedicated worktree is reset to a clean detached `origin/main` **only before a new WP**, never while an open candidate is being worked. Runtime logs and the lock live under `%LOCALAPPDATA%/Arkus/Juego2/autopilot`, not in the repository. Treat logs as local private data.

## Role and model policy

The inexpensive deterministic driver handles polling, exact marker/SHA checks and routing; a Luna reasoning turn is used only where judgment adds value. Every `codex exec` launch creates a **fresh local ChatGPT-backed session**. No Worker history is supplied to the Reviewer. Every role must re-read authoritative live state and its project skill. The driver assigns each independent review session a unique `Autopilot review ID` line alongside the protocol's exact-SHA verdict fields. It counts that ID once even if the verdict is mirrored in a PR comment; untagged/manual verdicts are never silently counted as unattended-cycle FAILs.

| Role | Default | Boundary |
| --- | --- | --- |
| Worker / repair Worker | GPT-6 Sol high; xhigh for explicitly foundational/architectural/completeness WPs | One active owner; may delegate a bounded complex slice to a fresh Sol xhigh subagent under the existing delegated-execution rule; owns integration, tests, pre-review and freeze. |
| Independent Reviewer / appeal Reviewer | GPT-6 Sol xhigh | Fresh context, exact frozen SHA, no implementation repair. |
| Non-terminal material-FAIL audit | GPT-6 Luna xhigh | Checks circuit breakers before further work after FAILs 1–3; at the second FAIL classifies concrete valid defect, overdefense or uncertainty. Never emits/supersedes a verdict. |
| Post-merge DocSync | GPT-6 Luna high | Documentation only after exact PASS/merge; if semantics or implementation need change, stop. |

Do not raise effort merely because a WP is long. Use xhigh for difficult causal boundaries, architecture, completeness or repeated unresolved evidence. The model policy is a starting heuristic, not proof of quality: compare high/xhigh on the same historical candidates for missed material defects, false FAILs, token use and elapsed time before changing it. Never reduce independent review or required proof to save tokens.

## State flow

```text
Worker (fresh) -> context-bound REVIEW_READY at PRODUCT_SHA -> Reviewer (fresh)
  PASS -> exact-SHA merge -> DocSync -> DOCSYNC_COMPLETE -> next WP
  FAIL -> REPAIR_REQUIRED -> repair Worker (fresh) -> new SHA -> Reviewer (fresh)
  PROTOCOL_FIX -> same-SHA metadata closure -> REVIEW_READY -> fresh Reviewer
  REVIEW_BLOCKED -> stop for PC
```

At **each non-terminal material FAIL (1–3)**, Luna xhigh checks the exact criterion, effective path, accepted predecessor ownership, evidence and proof budget before another Worker or appeal may start. Its structured audit is bound to the frozen SHA and FAIL count in a durable marker. This permits an immediate stop even at the first FAIL for a self-shrinking completeness proof or other architectural circuit breaker; repeated foundational defect class and proof-machinery expansion are also checked on later FAILs. At the **second independent FAIL**, Luna additionally distinguishes a real blocker from concrete overdefense before returning the case to the owner. If uncertain or architecturally blocked, stop and require PC. If the FAIL is valid, send a Telegram button that lets the owner authorize unattended repair to continue **up to, but not through, the fourth FAIL**. If it appears to be concrete overdefense, a **new independent Sol xhigh Reviewer** re-examines the unchanged frozen SHA, explicitly addresses the prior FAIL, and alone may publish a superseding PASS. A durable appeal-start marker prevents a restart from silently shopping for another review; if the appeal is interrupted without a new verdict, PC reconciliation is required. The earlier FAIL remains durable history. If the new Reviewer sustains FAIL, offer the same owner button; Luna cannot erase it. At the fourth FAIL, stop immediately and require PC with no further Luna audit or continue button.

`PROTOCOL_FIX` is not a material FAIL and does not increment the FAIL count. The controller may launch one bounded fresh metadata-only corrector for the unchanged `PRODUCT_SHA`, then wait for a new context-bound `REVIEW_READY` without rerunning the product suite. It requires the live PR to remain open and non-Draft, then verifies the marker's key, digest and effective WP/class against the canonical validation-context resolver and current PR metadata before starting a Reviewer; a same-SHA marker with stale metadata or a Draft/closed PR is not ready. A repeated or interrupted correction, SHA movement, or `REVIEW_BLOCKED` stops for PC. The subsequent Reviewer session must reuse trustworthy exact-SHA mechanical receipts and focus its independent judgment on the material claim; it must not treat the protocol correction as a new product cycle.

The button uses Telegram `callback_data` tied to the PR, exact frozen SHA and FAIL count. `telegram-notify.yml` sends it with the existing bot secret; `telegram-continue.yml` temporarily long-polls the Bot API (no new model/API-key provider), accepts only a click from the configured **private** owner chat, revalidates live PR/frozen SHA/fail count and a durable `SECOND_FAIL_OFFERED` marker, then writes an idempotent `OWNER_CONTINUE` PR comment. The local driver waits for that comment before launching a fresh repair Worker. A group chat, configured Telegram webhook, stale button, changed SHA, missing secrets or unavailable receiver fails closed. The receiving Actions job expires after about 5 hours 40 minutes; an unanswered/expired button requires PC. The durable offer itself has a maximum age of 5 hours 50 minutes, so a crash before the receiver starts cannot preserve an old button indefinitely; restarting after expiry cannot revive that same SHA/count decision. It uses a standard hosted runner only during an active second-FAIL decision, not a permanent polling service. Do not treat a click as PASS, proof, or waiver of a circuit breaker.

## Quota and human attention

Before **each** reasoning session the driver calls App Server `account/read`, `model/list` (startup), and `account/rateLimits/read`. It checks every reported primary/secondary metered window. Missing general-window data fails closed. At **3% or less remaining** in a short window, it launches no new reasoning session, waits locally until the reported `resetsAt` plus a small margin, and re-reads limits. This local timer consumes no model turn and needs the computer/controller process running. At **3% or less in the long/general window**, it dispatches `HUMAN_ACTION_REQUIRED` through the existing Telegram workflow and exits **without automatic resume**. It never consumes reset credits automatically. Because one already-running turn can cross 3%, the threshold is enforced at role boundaries; for long operations the Worker/Reviewer must preserve durable GitHub handoff state and the driver will not start the next role below the floor.

Ambiguous/blocked states, missing required effective Unity evidence (hosted when the accepted policy allows it, physical-local when the claim needs it), unavailable required tools, unresolved architecture questions and other reasons a person is needed produce `HUMAN_ACTION_REQUIRED` or `BLOCKED` via Telegram and stop. Apart from the narrowly verified owner-continue button, Telegram is notification only; inspect GitHub and local logs before resuming. Do not store passwords, PATs, API keys, Unity credentials or bot tokens in logs/prompts/evidence.

## Process-budget sanitization

Quality gates remain: exact WP and accepted predecessor contract, causal in-boundary proof, strict Worker pre-review, exact-SHA preflight/freeze, fresh independent review, and bounded DocSync. `PRODUCT_SHA_CLOSURE.md` governs normal same-PR/same-SHA Main Safety reuse, protocol-only corrections, direct `REVIEW_READY` and zero-commit DocSync. The cost reductions are **navigation and repeated work**, not acceptance reductions:

1. Start each role from its existing context profile and validated capsule; deepen only on an explicit trigger or material contradiction. Do not reread the entire project history by habit.
2. Keep the Worker pre-review one concise causal report. Do not generate a second full independent-style report in the Worker context.
3. Reuse a valid exact-SHA GREEN receipt for the **identical** candidate; never rerun expensive validation merely to move a state marker. Main Safety is the normal hosted Worker preflight; dedicated Worker Candidate Preflight is on-demand fallback.
4. Do not re-prove an applicable accepted predecessor guarantee without concrete contradictory evidence. Record out-of-boundary theoretical concerns as residuals, not new blocking test matrices.
5. Keep deterministic parsing, polling, quota, routing and notification out of model turns. Use Luna only for real ambiguity/failure triage or bounded DocSync; use Sol where code/review quality matters.

These rules do not compress away non-compressible proof sources, required effective Unity evidence, or the Reviewer's independent causal search.

## Pilot and limitations

Run `python scripts/test_local_wp_autopilot.py`, `python scripts/test_telegram_continue_receiver.py` and `--dry-run` first. Pilot **one** next dependency-valid WP with `--one-wp` and inspect role independence, exact-SHA state, Telegram delivery, quota behavior and DocSync before using the default continuous mode. Do not treat this process PR's test run as proof that a full unattended WP lifecycle has completed. The CLI process must stay running; it is not an OS service. Recover a crashed run only after verifying no role is still active and inspecting the canonical PR. `--adopt` is not a role lease or a license for two concurrent writers.
