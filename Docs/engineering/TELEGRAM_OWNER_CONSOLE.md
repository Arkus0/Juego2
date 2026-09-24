# Telegram owner console (accepted opt-in)

Version: 0.3 — 2026-09-24
Status: ACCEPTED / DOCSYNC_COMPLETE

## Acceptance

- Canonical process PR: `#180`
- Accepted candidate: `8250ca10ecfe7663a0d068698e6becf624f00852`
- Final independent Reviewer PASS: `#5302446547`
- Merge: `6a8ba3108d8d5b8d9529b184420e670207031128`
- Exact-SHA validation on the accepted candidate: Arkus Candidate Validation `#1944` GREEN, Arkus Main Safety `#482` GREEN, Telegram Owner Console Validation `#4` GREEN with 19/19 offline tests.

This is an accepted **opt-in** process surface. It changes no product workpack state and does not advance the H1, PA, CITY, DW or CTX execution spines.

The accepted trust boundary protects owner authority from a Worker using the normal capabilities exposed to Worker/repair roles: the Worker does not receive `TELEGRAM_BOT_TOKEN`; `OWNER_CONTINUE` and bounded `OWNER_DECISION` require supervisor-only HMAC material validated by GitHub Actions; loopback IPC is non-authoritative; Reviewer roles receive no remote-control capability variables. Deliberately rewriting the repository's own GitHub Actions trust infrastructure is a repository-root compromise class, not a per-console authority claim.

## Purpose

Provide a bounded remote-control surface for the accepted local WP autopilot without turning Telegram into proof authority or a remote shell. The console may start a campaign, report status, pause or stop at a safe WP boundary, queue one owner note for the next fresh Worker/repair role, and deliver a bounded 2–3 option owner preference back to a Worker that explicitly requests it.

Reviewer verdicts, acceptance criteria, exact-SHA integrity, mandatory evidence, circuit breakers and hard human/physical-PC requirements are never owner-choice buttons. A Reviewer remains fresh and independent and never consumes owner notes.

## Local supervisor

The workstation runs:

```powershell
python scripts/local_wp_remote_console.py --root . --assets-root C:\Juego2-Assets
```

The supervisor is idle when no campaign is active. Codex does not need to be open in a visible interactive session: the existing autopilot launches fresh `codex exec` roles itself.

The supervisor resolves and validates `--assets-root` before accepting commands. Every fresh role receives that directory as a Codex additional workspace root plus explicit context that it is external, non-canonical source input whose upstream bytes are read-only unless the exact workpack authorizes a bounded local change. A missing, unreadable or checkout-internal assets root fails closed before a campaign starts.

The supervisor is the only Telegram `getUpdates` consumer while this mode is active. `TELEGRAM_BOT_TOKEN` and `TELEGRAM_CHAT_ID` are local supervisor configuration. The token is stripped from every autopilot/Codex child environment. Do not run the legacy GitHub-hosted long-poller concurrently with this console.

Supported private-owner commands:

- `/run H1-04` — start one WP and then follow each accepted `DOCSYNC_COMPLETE -> Next WP` handoff.
- `/status` — show current WP and pending decisions.
- `/pause` — finish the current WP but do not start the next one.
- `/resume` — continue a paused campaign.
- `/stop` — finish the current WP and end the campaign. It intentionally does not kill a Worker or Reviewer mid-turn.
- `/note <text>` — queue an owner instruction for the next fresh Worker/repair-side role only. It cannot enter a Reviewer context and cannot override repository contracts.

## Soft owner decision versus hard stop

A Worker/repair Worker may use `scripts/request_owner_decision.py` only for a real owner preference that can be represented by two or three bounded alternatives and does not weaken proof or policy. The request must bind the current PR and exact HEAD SHA (`--pr` + `--sha`). The helper blocks that same Worker turn without an application TTL while the local console and campaign remain alive.

The loopback `/v1/decision` response is only a liveness hint. It is not owner authority. When the owner presses a Telegram choice, the supervisor freezes the exact advertised request, HMAC-signs `{PR, SHA, campaign, decision_id, request_digest, choice, selected_digest, Telegram callback id}` with `TELEGRAM_BOT_TOKEN`, and dispatches a short GitHub Action. The Action verifies the supervisor-only HMAC, rechecks the exact open PR/HEAD SHA, and writes an `OWNER_DECISION` marker as `github-actions[bot]`. The Worker helper accepts the option only when that exact bot-authored attestation exists. A Worker-controlled replacement endpoint on another `127.0.0.1` port therefore cannot mint an owner choice.

The request digest also prevents a Worker from changing the question/options after Telegram displayed them and then reusing the owner's callback index for different semantics.

Examples of a soft decision: choose between two in-scope presentation/layout variants, choose which dependency-valid next implementation variant to prefer when both satisfy the WP, or select a bounded owner preference explicitly left open by the contract.

Examples that must remain a hard stop: required physical observation unavailable remotely, architecture ambiguity that needs investigation rather than preference, fourth FAIL, REVIEW_BLOCKED, unavailable mandatory evidence, security/authentication failure, or any request to waive an acceptance criterion. Those produce the normal `BLOCKED` / `HUMAN_ACTION_REQUIRED` notification and the campaign stops.

## Second-FAIL continue

In local-console mode the existing exact PR/SHA/fail-count Telegram button remains the UI. The local supervisor HMAC-signs the owner click with `TELEGRAM_BOT_TOKEN`; a short GitHub Action verifies that proof plus campaign binding before writing the durable bot-authored `OWNER_CONTINUE` marker. The GitHub-hosted ~5h40 long-poller is not started. The local transport deliberately removes only the transport TTL; the existing fourth-FAIL ceiling, exact frozen SHA checks and owner-private-chat checks remain.

## Limits

"No timeout" means no application-level owner-decision expiry while the PC and local supervisor remain alive. It does not make an offline PC execute work. Sleep suspends local execution until wake. A reboot/crash still requires explicit recovery after checking that no role remains active. Remote power-on/Wake-on-LAN is a separate feature and is not part of this amendment.

This console is transport and orchestration only. GitHub state, workpack contracts, code/tests and the independent Reviewer remain authoritative.
