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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A` and `WP-HK-05` are COMPLETE.

Latest accepted workpack: `WP-HK-05 — Validation + repairable diagnostics`.

- implementation PR: `#26`;
- baseline SHA: `dbd8121411079f22a01d5cb85345e180ff41f7e2`;
- reviewed frozen candidate: `23a9fd4373a803187cd9391b1459cd48975177f6`;
- independent Reviewer: `PASS` (PR review `#5257350871`);
- exact-SHA candidate observation: GREEN, Actions `35464742544`;
- exact-SHA freeze validation: GREEN, Actions `35464834162`;
- merge SHA: `ed65661680aea2a9be79f892c96aa42bf788a842`;
- accepted semantic addition: discoverable/versioned current and proposed validation; mechanically reconciled finite invariant inventory; deterministic multi-violation structured diagnostics; consistent validation before canonical commit; duplicate identities get index-addressable diagnostics while only genuinely ambiguity-dependent secondary diagnostics are deferred;
- accepted scope limits: no AI natural-language repair generation, Unity validation, final gameplay invariants, journal/replay or HK06+ behavior.

HK05 had two historical Reviewer FAILs on frozen candidates `e0c865efd2127ca53d9e25064215e09a4579acd4` and `8c8d8c1a66b4995bbc9f3f933ba1791e49d69e97`, both in the same aggregate-validation-under-ambiguous-identity class. The circuit breaker was correctly triggered before the accepted repair: the final policy is dependency-local rather than a growing list of special cases. Preserve this history as evidence of the causal boundary, but do not reopen HK02/HK04 predecessor claims without concrete contradictory evidence.

## Current next product target

`WP-HK-06 — Provenance, diff, snapshot + replay` is the next dependency-valid workpack. It consumes HK05's accepted validation/pre-commit semantics and adds auditable committed mutation provenance, semantic diff, canonical snapshot export/import and deterministic authored-state journal replay. It must preserve the authored-vs-live-state boundary: ordinary runtime observations/ticks must not silently create authoring revisions, journal entries or CAS churn. Read `Docs/workpacks/HK/WP-HK-06.md` plus accepted HK05 evidence before implementation. No HK06 Worker has been started by this DocSync.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.6 retains mandatory predecessor-contract reconstruction and adds a bounded representative content-shape probe for applicable foundational WPs. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06 → HK-07 → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
