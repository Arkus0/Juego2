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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02` and `WP-HK-03` are COMPLETE.

Latest accepted workpack: `WP-HK-03 — Inspection/query surface`.

- implementation PR: `#18`
- reviewed frozen candidate: `8c20a380003c082fa9bd472d3233afa9654fb231`
- independent Reviewer: `PASS` (PR review `#5255545669`)
- exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35438687868`)
- merge SHA: `d8b808450ee7863726d718a25c5756534113fcfd`
- accepted read boundary: six canonical `world.*@1.0` capabilities provide world summary, object lookup/query, flattened reference query, extension descriptors and bounded opaque-extension chunks through the single HK01 composed inventory
- inspection guarantees: deterministic ordering/cursors, revision/hash anchoring, structured stale/selector/missing-resource failures, fixed/paginated output, side-effect-free reads and public-read reconstruction of the current HK02 semantic world state

HK03 passed on its first independent review. The Reviewer explicitly classified selector-array cardinality/resource-exhaustion limits as downstream `WP-HK-09` ownership because HK09 owns input size and execution/resource limits; do not reopen HK03 for that alone without concrete evidence that the accepted inspection semantics are false. HK03's own bounded-output and finite query-language guarantees remain binding.

## Current next product target

`WP-HK-04 — Planning + transactional mutation` is the next dependency-valid workpack. It depends on completed HK03 and must introduce a safe canonical mutation model based on deterministic plan → validate/dry-run → atomic apply, with optimistic revision/hash concurrency, idempotent retries, explicit change sets and no hidden mutation bypass.

Before any HK04 implementation, the Worker must execute the protocol v1.5 `PREDECESSOR_CONTRACT_CHECK`: read accepted HK03 contract/PASS/proof evidence, identify which inspection/query guarantees HK04 consumes, separate them from HK04-owned planning/transaction guarantees, and record what concrete evidence would justify reopening an inherited boundary. Do not re-prove HK00/HK01/HK02/HK03 merely for defence-in-depth.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.5 makes predecessor-contract reconstruction mandatory for both roles. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 → HK-05 → HK-06 → HK-07 → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
