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

## Current accepted state

`WP-HK-00` and `WP-HK-00A` are COMPLETE.

Latest accepted workpack: `WP-HK-00A — Product architecture + adoption boundary`.

- implementation PR: `#15`
- reviewed frozen candidate: `e66ed729c75d94fb7efdfc304cf51ec52fa25e53`
- independent Reviewer: `PASS`
- exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35430543113`)
- merge SHA: `0a4253443326ccab8e910c3265b015fc757cc4a2`
- accepted product architecture: canonical Arkus contract is engine/transport neutral, supports reviewed scoped capability composition, and forbids bridge/transport-owned parallel public registries
- dependency/adoption policy: binding; exact-version review remains required before direct external-code adoption

The prior HK00 project/source-universe defect and the HK00A engine-scoped-capability ownership ambiguity are closed inside their accepted finite claims. Do not reopen either WP merely for theoretical defence-in-depth outside its proof budget.

## Current next product target

`WP-HK-01 — Canonical contract model + capability discovery` is the next dependency-valid workpack. It must implement and prove the engine-neutral canonical contract/composition model required by the accepted HK00A boundary, including synthetic scoped-provider composition and independently/effectively enumerable completeness.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 → HK-02 → HK-03 → HK-04 → HK-05 → HK-06 → HK-07 → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
