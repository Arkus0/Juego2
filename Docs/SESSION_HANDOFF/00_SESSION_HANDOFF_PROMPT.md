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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05` and `WP-HK-06A` are COMPLETE.

Latest accepted workpack: `WP-HK-06A — Provenance journal + authored/live boundary`.

- implementation PR: `#32`;
- baseline SHA: `bc6241d2db2b6e15f1a9ac78c673f7ef0735ffff`;
- reviewed frozen candidate: `4ed9a925791ae14b0b5c0d92625161504021542e`;
- independent Reviewer: `PASS` (PR review `#5257682288`);
- exact-SHA candidate observation: GREEN, Actions `35469103222`;
- exact-SHA freeze validation: GREEN, Actions `35469154331`;
- merge SHA: `8089a52e8a7bbdde46e97705df6906c0d38d593a`;
- accepted semantic addition: versioned/discoverable authored mutation journal; deterministic entry identity and ordering; complete normalized accepted mutation envelope bound to parsed request fingerprint/base anchor; truthful before/result anchors and affected resources; atomic publication of canonical state + idempotency receipts + journal under the accepted commit lock; explicit runtime-observation stamp and authored/live authority boundary;
- accepted scope limits: no semantic diff, snapshot export/import, replay execution/compatibility, Unity serialization, gameplay scheduler/AI or deterministic simulation.

HK06A had one historical Reviewer FAIL on frozen candidate `046134fd3acd9641798dbad180406688fb5d31aa`: the journal could theoretically serialize a schema-valid but semantically false replay envelope while state/fingerprint anchors stayed green. The accepted repair addressed the causal provenance boundary rather than special-casing `typeId`: the complete machine-readable request is independently re-fingerprinted across all four current operation kinds and entry identity is revalidated before publication, with a separate test-owned accepted-request oracle. Preserve that accepted truthfulness guarantee for downstream HK06B/HK06C rather than redundantly re-proving it without contradictory evidence.

## Current next product target

The original HK06 and HK07 monoliths were split **before implementation** to reduce coupled foundational freeze/review risk without reducing scope. Their old files remain SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B → HK06C → HK07A → HK07B → HK08 → HK09 → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-06B — Semantic diff + canonical snapshot portability`.

HK06B consumes the accepted HK06A journal truthfulness and authored/live authority boundary. It owns deterministic semantic diff plus the canonical snapshot format/schema and import/export semantics. Snapshot round-trip must reconstruct identical canonical authored state/hash; invalid or unsupported snapshots must not partially replace state; transient runtime observations must not leak into snapshots; import must explicitly define lineage/history behavior and must not fabricate mutation provenance. Read `Docs/workpacks/HK/WP-HK-06B.md` plus accepted HK06A evidence before implementation.

After 06B PASS+merge+DocSync: 06C consumes both accepted artifact contracts to own deterministic journal replay + end-to-end audit consistency. After 06C, 07A establishes the real headless process and deterministic JSONL/reference transport; only then does 07B add MCP and prove cross-transport semantic equivalence. HK08, HK09, HK10 and HK-GATE remain unsplit.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.6 retains mandatory predecessor-contract reconstruction and adds a bounded representative content-shape probe for applicable foundational WPs. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B → HK-06C → HK-07A → HK-07B → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
