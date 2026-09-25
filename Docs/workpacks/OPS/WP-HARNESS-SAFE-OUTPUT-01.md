# WP-HARNESS-SAFE-OUTPUT-01 — Deterministic authority boundary

Status: COMPLETE / ACCEPTED
Mode: PROCESS_ONLY
Class: FOUNDATIONAL_PROCESS_AUTHORITY
Owner: Arkus harness

## Problem

The harness has multiple places where human/agent prose such as `Reviewer verdict:` or a decoded owner action can be interpreted directly as authority for a privileged lifecycle transition. Exact-SHA checks around those paths are strong, but text interpretation and authorization are still coupled. That coupling has produced stale-adoption, replay, contradictory-verdict and owner-decision repair work.

## Goal

Introduce one small deterministic safe-output contract. Agent/owner prose may explain a decision, but privileged mutation requires an `ARKUS_INTENT_V1` envelope validated against trusted runtime facts. Keep ChatGPT-subscription Codex CLI, Chat review workflows, GitHub exact-SHA evidence, CTX, DW, Unity and product semantics unchanged.

## Scope

This WP supports exactly two intent kinds:

- `REVIEW_VERDICT` — role `REVIEWER`; verdict is `PASS`, `FAIL`, `PROTOCOL_FIX` or `REVIEW_BLOCKED`; exact WP/PR/campaign/SHA/review-id binding is mandatory. Only final `PASS`/`FAIL` enter the legacy State Transitions transport.
- `OWNER_DECISION` — role `OWNER`; action is `CONTINUE`; exact WP/PR/campaign/SHA/fail-count/Telegram-update binding is mandatory.

`campaign_id` is deliberately derived as `arkus:<canonical WP>:pr:<canonical PR>`. No new campaign database is introduced. Candidate-turn freshness remains exact-SHA bound; owner-decision freshness is additionally fail-count/source bound.

## Authority rules

1. Human-readable prose is never sufficient machine authority.
2. Exactly one `ARKUS_INTENT_V1 <canonical-json>` line is required for a safe output.
3. Reviewer authority is accepted only from an owner-authored GitHub **pull request review** whose GitHub `commit_id` equals the intent candidate SHA. Issue comments are non-authoritative for Reviewer verdicts.
4. The local autopilot consumes only broker-authorized Reviewer intents. The old `Reviewer verdict`, `Reviewed candidate SHA` and `Autopilot review ID` fields may remain as human/transport mirrors but cannot authorize a transition alone.
5. The existing State Transitions workflow is guarded underneath by the canonical validation-context CLI: on review events, its legacy PASS/FAIL fields must exactly mirror a valid safe-output envelope; issue-comment verdict transitions fail closed.
6. The durable reviewer-adoption recovery path uses the same broker and the GitHub review `commit_id`; it does not parse legacy prose as authority.
7. Telegram continuation constructs and immediately revalidates an `OWNER_DECISION`; the durable `OWNER_CONTINUE` marker records the intent key and decision slot. Existing marker idempotency prevents repeated consumption for the same SHA/fail-count.
8. Worker/Repair roles receive no `REVIEW_VERDICT` capability from this broker.

## Required negative controls

The broker self-test must reject:

- stale/wrong candidate SHA;
- wrong WP/campaign;
- wrong PR;
- duplicate intent lines;
- Worker spoofing `REVIEWER` role in the schema;
- contradictory PASS/FAIL for the same review decision slot;
- stale/wrong owner fail-count;
- wrong owner source/update id;
- stale/wrong owner candidate SHA.

Adapters additionally fail closed on non-owner review actor, issue-comment Reviewer intent, GitHub review `commit_id` mismatch, contradictory duplicate review ID, stale owner offer, moved PR HEAD, changed safe-output FAIL count, expired owner decision and already-consumed owner continuation.

## Liveness / compatibility

- ChatGPT-subscription Codex CLI remains the execution engine; no API key or external agent runtime is introduced.
- Existing local autopilot helper API is re-exported through the safe-output front door so existing regression tests remain meaningful.
- Existing lifecycle implementation is retained as an internal core; only its authority-bearing seams are replaced.
- `FAIL` adoption keeps the existing idempotent fail-cycle ledger/reconciliation behavior and must not increment twice on replay.
- PASS merge still uses the existing exact-SHA merge and preflight gates.
- `PROTOCOL_FIX` / `REVIEW_BLOCKED` retain their existing controller semantics and do not enter the direct PASS/FAIL State Transitions path.

## Circuit breaker

Do not add heuristic natural-language verdict parsing, compatibility fallbacks that re-authorize legacy prose, a new database, or additional intent kinds to close this WP. If a required transition cannot be expressed through these two intents and the existing deterministic lifecycle, stop and re-audit instead of expanding the broker.

## Acceptance

GREEN requires all of the following on one candidate SHA:

- `scripts/arkus_safe_output.py --self-test` passes;
- all Python process checkers compile;
- Main Safety remains GREEN;
- local autopilot canonical entry is the safe-output front door;
- reviewer-adoption canonical entry is the safe-output front door;
- Telegram continuation canonical entry is the safe-output front door;
- State Transitions cannot authorize legacy PASS/FAIL without a matching safe-output PR-review envelope;
- no product/Unity/CTX/DW semantics change.

## Accepted identity

- Final accepted candidate: `580399bcc7c0ec999efb93b66515aaff790486f7`.
- Canonical implementation PR: `#207`.
- Predecessor review `#5314811442` correctly found the State Transitions MEMBER/COLLABORATOR authority gap on candidate `29afe30e626295283cd1c5c88a2ac8cd2933d426`.
- Final candidate `580399bcc7c0ec999efb93b66515aaff790486f7` repaired that gap by requiring both owner login and `OWNER` association and by adding real pull-request-review negative controls for MEMBER/COLLABORATOR.
- Review `#5314919546` raised a broader hostile-local-agent credential-isolation requirement; the owner rejected that requirement as overdefense outside this WP's bounded threat model and directed merge.
- Implementation merge: `8f7c1970cfd3533908a29b556cff199d8e7689f7`.
- Exact final-candidate checks were GREEN, including Arkus Main Safety run `36107865736`, Arkus Candidate Validation run `36107865734`, Arkus PROCESS_ONLY Hotfix Validation run `36107865839`, Telegram Owner Console Validation run `36107865791`, and CTX Process Envelope run `36107865821`.

`DOCSYNC_COMPLETE`

This acceptance does not establish hostile-code or credential-compromise isolation between local Codex roles. It establishes the deterministic authority boundary described above under the accepted harness threat model. Product, Unity, H1, CTX and DW semantics remain unchanged.
