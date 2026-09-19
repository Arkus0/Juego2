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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A` and `WP-HK-06B` are COMPLETE.

Latest accepted workpack: `WP-HK-06B — Semantic diff + canonical snapshot portability`.

- implementation PR: `#35`;
- baseline SHA: `5281160ce4eff601ab52dfb568fd4e4976560383`;
- reviewed frozen candidate: `2b05e982c96e7ece08cca999075c183e75eee2fd`;
- independent Reviewer: `PASS` (PR review `#5258130916`);
- exact-SHA validation: GREEN, Actions `35473614054`, artifact `10594111979`;
- merge SHA: `28e2d0aadf63fe322eac636955e9223dc9249328`;
- accepted semantic addition: deterministic field/resource semantic diff over the accepted authored `WorldState`; versioned canonical snapshot artifact; fail-closed snapshot import; explicit authored/live exclusion; keyed import idempotency; and snapshot import as a distinct `CanonicalRebase` that atomically establishes the imported state as a new local lineage root with truthful rebase evidence and an empty HK06A mutation journal;
- accepted scope limits: no journal replay, deterministic gameplay simulation, Unity serialization, cloud/file/network persistence semantics or transport-specific framing.

HK06B had one historical Reviewer FAIL on frozen candidate `bebc1b6165f7228f33dc593534052cd80eb6e041`: snapshot import was publicly classified as `CanonicalMutation + CanonicalTransaction + Provenance Required` while execution used a separate whole-session replacement authority and deliberately left the HK06A journal empty. The accepted repair closes that causal classification/authority/history seam by introducing explicit `CanonicalRebase` semantics and required machine-readable rebase evidence rather than weakening HK04 mutation conformance or fabricating HK06A provenance.

The accepted HK06A guarantee remains unchanged: ordinary canonical mutations still publish truthful journal entries at the accepted commit boundary. HK06B adds a distinct whole-root rebase class; it does not reinterpret mutation provenance. Preserve this split in HK06C.

## Current next product target

The original HK06 and HK07 monoliths were split **before implementation** to reduce coupled foundational freeze/review risk without reducing scope. Their old files remain SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C → HK07A → HK07B → HK08 → HK09 → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-06C — Deterministic journal replay + end-to-end audit consistency`.

HK06C consumes both accepted artifacts rather than redefining them: HK06A owns the truthful normalized mutation-journal envelope; HK06B owns snapshot/diff semantics and the explicit canonical-rebase root. HK06C must reconstruct the final authored state/hash from an accepted base/snapshot plus journal evidence, fail closed on reordered/missing/tampered/incompatible evidence, preserve canonical validation/transaction semantics, and prove an empty HK06B semantic diff between original and replayed final states. Read `Docs/workpacks/HK/WP-HK-06C.md` plus accepted HK06A/HK06B evidence before implementation.

After 06C PASS+merge+DocSync: 07A establishes the real headless process and deterministic JSONL/reference transport; only then does 07B add MCP and prove cross-transport semantic equivalence. HK08, HK09, HK10 and HK-GATE remain unsplit.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C → HK-07A → HK-07B → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
