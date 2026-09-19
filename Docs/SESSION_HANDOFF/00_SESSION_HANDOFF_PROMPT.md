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

`WP-HK-00`, `WP-HK-00A` and `WP-HK-01` are COMPLETE.

Latest accepted workpack: `WP-HK-01 — Canonical contract model + capability discovery`.

- implementation PR: `#16`
- reviewed frozen candidate: `c16c0a7bbe4afe440252b921516b5e9b4635e082`
- independent Reviewer: `PASS` (PR review `#5255264042`)
- exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35435429932`)
- merge SHA: `24d761ba0bc33a70fc06e5ea351054b5d3c51488`
- accepted contract boundary: one transport-neutral Arkus capability inventory drives dispatch/discovery/projection; scoped providers enter only through canonical composition; portable logical references are provider-owned; same-major breaking request changes fail composition before negotiation
- conformance boundary: definitions, canonical dispatcher routes, independently enumerated concrete handlers and the emitted `system.describe` artifact are reconciled by the HK01 suite

HK01 had two rejected candidates before PASS. The final repair cycle re-audited the portability + evolution/negotiation boundary rather than stacking literal patches. Do not reopen HK01 merely to duplicate HK00 project-universe proof or for theoretical defence-in-depth outside the accepted finite claim/proof budget.

## Current next product target

`WP-HK-02 — Canonical world state + deterministic identity` is the next dependency-valid workpack. It depends on completed HK01 and must establish stable typed identity, deterministic canonical serialization/state hashing, explicit versioning, referential integrity and a minimal representative micro-world fixture without leaking Unity/gameplay design into H0.

Before any HK02 implementation, the Worker must execute the protocol v1.5 `PREDECESSOR_CONTRACT_CHECK`: read accepted HK01 contract/PASS/proof evidence, identify the HK01 guarantees HK02 consumes, separate them from HK02-owned guarantees, and record what concrete evidence would justify reopening an inherited boundary. Do not duplicate HK00/HK01 proofs merely for defence-in-depth.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.5 makes predecessor-contract reconstruction mandatory for both roles. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 → HK-03 → HK-04 → HK-05 → HK-06 → HK-07 → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
