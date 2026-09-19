# GitHub-hosted Actions policy

Status: DISABLED BY DEFAULT — 2026-09-19

Juego2 operates with a zero-budget hosted-CI policy.

The repository's former automatic GitHub-hosted workflows are retained only as inert historical/configuration placeholders. They must not be re-enabled as mandatory gates without an explicit process change that explains cost and why provider-neutral execution is insufficient.

Validation and freeze requirements are governed by `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md`.

GitHub remains the repository, PR, review and durable handoff surface. Worker/Reviewer/local execution environments provide compute when they satisfy the exact-SHA receipt protocol.

Telegram and agentic transition automation are convenience features, not correctness gates. Their unavailability must not block a WP that otherwise has complete provider-neutral validation evidence.
