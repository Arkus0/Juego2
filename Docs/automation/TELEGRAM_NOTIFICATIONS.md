# Telegram Notifications — Juego2

Version: 1.0 — 2026-09-18

Telegram is an optional low-noise side channel. GitHub remains authoritative. Delivery failure never changes WP state, ownership, leases, review verdicts, merge decisions or `fail_cycle`.

## Notify only

1. `WP_COMPLETE` — implementation merged and post-merge DocSync complete/NOOP.
2. `HUMAN_ACTION_REQUIRED` — a real human decision/permission is required.
3. `BLOCKED` — durable contractual block or automatic repair fuse exhausted (`fail_cycle >= 4`).
4. `GATE_COMPLETE` — full milestone/gate accepted, merged/finalized and documentation synchronized.

Do not notify routine progress, CI success, ordinary FAILs, repair starts, lease activity, polling or recoverable provider interruptions.

## Marker

Persist authoritative state first, then add at most one deterministic marker:

```text
TELEGRAM_NOTIFY_V1
Type: WP_COMPLETE | HUMAN_ACTION_REQUIRED | BLOCKED | GATE_COMPLETE
Key: <stable unique key>
Subject: <short subject>
Detail: <one short sentence>
Item: <optional gate item; repeat 2–6 times for GATE_COMPLETE>
Backend: WORK_MOBILE_CLOUD | CODEX_DESKTOP_LOCAL | CLAUDE_CODE_CLOUD | GITHUB
```

Before adding a marker, search existing comments for the same `Key:`. Duplicate key => NOOP.

Examples:

```text
wp:WP-HK-04:complete:<merge-sha>
blocked:WP-HK-04:fail-fuse:<candidate-sha>
gate:WP-HK-GATE:complete:<merge-sha>
```

## Style

Operator-facing messages are concise Spanish. Never include secrets, private reasoning, long logs or proprietary dumps.

## Transport

`.github/workflows/telegram-notify.yml` is the repository transport and uses Actions secrets:

- `TELEGRAM_BOT_TOKEN`
- `TELEGRAM_CHAT_ID`

Never commit these values. If either credential was ever exposed outside secret storage, rotate it before use.

## Failure semantics

Transport failure is notification failure only. Never roll back or retry a WP transition because Telegram failed.
