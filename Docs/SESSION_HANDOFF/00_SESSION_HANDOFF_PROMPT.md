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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03` and `WP-HK-04` are COMPLETE.

Latest accepted workpack: `WP-HK-04 — Planning + transactional mutation`.

- implementation PR: `#19`
- reviewed frozen candidate: `849ed68e41d674ab0d50883ccd9af394e9d2e456`
- independent Reviewer: `PASS` (PR review `#5256156672`)
- exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35450965678`)
- merge SHA: `b1810a5f7c5378ff06272718a11e08720d714a65`
- accepted mutation boundary: deterministic plan/dry-run/apply over the HK02 canonical world, atomic whole-state commit, expected revision/hash CAS, explicit idempotency replay/conflict semantics, machine-readable change sets/conditions and mechanically reconciled mutation/transaction/dispatcher surfaces
- authority boundary: `TransactionalWorldAuthoringSession` has no public commit method; canonical commit authority is internal to the Authoring→Runtime boundary and only the canonical transactional apply handler receives it
- proof convergence: the prior hidden-mutation proof loop was closed by removing public commit authority and using effective behaviour over the accepted HK01 route universe; `MutationAuthorityInspector` remains defence in depth only and must not regrow into container/wrapper enumeration

HK04 required two repair cycles before PASS. The accepted lesson is binding for downstream work: do not reopen HK01 route completeness without concrete contradictory evidence, and do not reintroduce syntax-specific whack-a-mole where an authority/effective-behaviour boundary can prove the actual claim.

## Current next product target

`WP-HK-02A — Object-scoped extension data + typed dependencies` is the active dependency-valid workpack. It started from process-adoption main SHA `5a07c55aeb79406a84bff579b34c09459707b713`, after proof standard v1.3 and Worker protocol v1.6 became non-circular binding predecessors. It depends on completed HK04 and must add an optional object subject plus declared typed dependency edges to opaque extensions, then propagate that semantic shape through canonical identity/hash, HK03 inspection and HK04 mutation/change coverage.

HK05 is paused behind HK02A. The earlier mixed Draft PR `#23` was closed as superseded before freeze; it has no candidate or Reviewer verdict. The replacement HK02A cycle is independent and begins only from the accepted process baseline.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.6 retains mandatory predecessor-contract reconstruction and adds a bounded representative content-shape probe for applicable foundational WPs. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A → HK-05 → HK-06 → HK-07 → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
