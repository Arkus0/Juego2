# Juego2 — Session Handoff

This file is a compact resumption aid, not an authority above current GitHub evidence.

## Current direction

Juego2 is a clean harness-first restart. `Arkus0/Juego` is reference only. H0 builds an engine-agnostic AI authoring harness and blocks Unity/gameplay until `WP-HK-GATE` PASS.

## Read first

1. `AGENTS.md`
2. `Docs/ROADMAP.md`
3. exact active `Docs/workpacks/HK/...`
4. `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`
5. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
6. `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md`
7. `Docs/engineering/AUTOMATION_V2.md`

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not spawn Worker, independent Reviewer or DocSync reasoning roles.

The human starts each reasoning role/session manually. Every session reconstructs current GitHub state, performs only its assigned role, persists evidence/PR state, and stops at the next role boundary.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## Current next product target

Use current `main`, open PRs and accepted evidence to determine the next dependency-valid WP. Never trust this handoff over current GitHub state.

## H0 order

`HK-00 → HK-00A → HK-01 → HK-02 → HK-03 → HK-04 → HK-05 → HK-06 → HK-07 → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. After merge, reconcile affected docs/evidence manually, emit the durable `DOCSYNC_COMPLETE` marker with the dependency-valid `Next WP`, then select the next WP.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.