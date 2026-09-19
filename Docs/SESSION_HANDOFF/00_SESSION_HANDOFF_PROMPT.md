# Juego2 — Session Handoff

This file is a compact resumption aid, not an authority above current GitHub evidence.

## Project context

Juego2 / Arkus Harness is a **game-development and software-verification project, not a cybersecurity project**. Repository work is limited to the game-authoring harness, its own source code, fixtures, tests, CI and documentation. Historical terminology such as `self-attack`, `attack fixture`, `bypass` or `adversarial review` means ordinary internal negative/conformance testing only. New work should use the neutral vocabulary defined in `AGENTS.md`.

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

## Current accepted state

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04` and `WP-HK-02A` are COMPLETE.

Latest accepted workpack: `WP-HK-02A — Object-scoped extension data + typed dependencies`.

- implementation PR: `#25`;
- baseline SHA: `5a07c55aeb79406a84bff579b34c09459707b713`;
- reviewed frozen candidate: `f39a1994524c42213dafb63d440faaf9de7c040f`;
- independent Reviewer: `PASS` (PR review `#5256593405`);
- exact-SHA candidate observation: GREEN, Actions `35456397714`;
- exact-SHA freeze validation: GREEN, Actions `35456445373`;
- implementation run `35456331653`: Release 0 warnings/errors, 11/11 focused and 98/98 regression;
- merge SHA: `ac7ce1180b462f093cf0ee02bbbd6f853938e27c`;
- accepted semantic addition: global or object-scoped opaque extensions with canonical composite identity `(owner, schemaVersion, subjectId-or-global)`, declared typed dependencies and referential integrity; V2 canonical codec/hash; complete bounded HK03 inspection; HK04 composite mutations and dependency change coverage;
- accepted scope limits: opaque payload semantics are not inferred, whole-world revision/hash CAS is unchanged, V1 migration and total world budgets are deferred.

The first HK02A frozen candidate `9d0dc3739f31bcc6a7f8df12b5f6e876837efacc` independently FAILed due to a global/object `"-"` idempotency fingerprint collision and a missing dependency-page-two causal proof. Both gaps were corrected, validated and independently reviewed in the merged candidate. Preserve this history; do not reopen HK04 predecessor claims or duplicate already accepted upstream proof without concrete contrary evidence.

## Current next product target

`WP-HK-05 — Validation + repairable diagnostics` is the next dependency-valid workpack after the HK02A post-merge DocSync. It consumes HK02A's completed subject/typed-dependency state shape, and owns machine-actionable validator inventory, invariant identity, deterministic multi-violation diagnostics and consistent public pre-commit validation. Read `Docs/workpacks/HK/WP-HK-05.md` and accepted predecessor evidence before implementation. No HK05 Worker has been started by this DocSync.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.6 retains mandatory predecessor-contract reconstruction and adds a bounded representative content-shape probe for applicable foundational WPs. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 → HK-06 → HK-07 → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
