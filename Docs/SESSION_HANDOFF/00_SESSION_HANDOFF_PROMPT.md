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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B` and `WP-HK-06C` are COMPLETE.

Latest accepted workpack: `WP-HK-06C — Deterministic journal replay + end-to-end audit consistency`.

- implementation PR: `#40`;
- baseline SHA: `c6534fa42f6bdd18a5bff4c3fe23f3868af993e8`;
- reviewed frozen candidate: `55fecbd8a4a5e17ce247b164cd375d652d066fdf`;
- independent Reviewer: `PASS` (PR review `#5259530509`);
- exact-SHA validation: GREEN, Actions `35488920548`, artifact `10598675153`;
- merge SHA: `e44a5e93bf0912f5b5fb80dd749e294e21a740f2`;
- accepted semantic addition: deterministic replay of accepted HK06A journal evidence from the exact accepted authored base; explicit compatibility policy; every replayed step executes through the accepted HK04/HK05 canonical mutation authority inside a staged session; replay publishes only after complete result/journal audit; successful replay regenerates truthful local HK06A history and proves identical final canonical hash plus empty HK06B semantic diff;
- accepted scope limits: no deterministic gameplay/runtime replay, durable journal/WAL/crash recovery, cross-lineage history merge, cross-process writer coordination, transport framing, MCP, cloud persistence or Unity replay.

HK06C introduced no predecessor contradiction. The accepted authority split is now explicit: ordinary authored changes use `CanonicalMutation`, whole-root snapshot import uses `CanonicalRebase`, and journal reconstruction uses `CanonicalReplay`. Replay consumes HK06A journal identity/provenance plus HK06B snapshot/diff semantics and does not redefine either.

The independent Reviewer specifically checked the staged aggregate publication seam, HK06B rebase-receipt lifetime, canonical mutation routing, replay compatibility, downstream HK07A projection and the WP's clean-process wording. No concrete false-green class remained inside HK06C's declared boundary.

## Current next product target

The original HK06 and HK07 monoliths were split **before implementation** to reduce coupled foundational freeze/review risk without reducing scope. Their old files remain SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A → HK07B → HK08 → HK09 → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-07A — Headless host + neutral projection contract + reference transport`.

HK07A must turn the accepted canonical runtime into a production-quality non-interactive process and freeze a transport-neutral projection contract above one deterministic reference transport. The reference transport must project the composed canonical capability inventory generically rather than own a second registry, keep protocol output isolated from diagnostics, define stable framing/failure/cancellation semantics, require no Unity/editor/network/user prompt for the local canonical path, and allow a fresh external client to discover and exercise representative read, validation, mutation, provenance, diff, snapshot and replay capabilities without source-code knowledge. Read `Docs/workpacks/HK/WP-HK-07A.md` plus accepted HK01–HK06C evidence before implementation.

After 07A PASS+merge+DocSync: 07B adds standards-compatible MCP projection and proves cross-transport semantic equivalence. HK08 then owns batching, compact responses, pagination, structured CAS recovery and measured interaction budgets; HK09 owns capability/resource boundaries; HK10 and HK-GATE remain downstream.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A → HK-07B → HK-08 → HK-09 → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
