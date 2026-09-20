# Juego2 — Session Handoff

This file is a compact resumption aid, not an authority above current GitHub evidence.

## Project context

Juego2 / Arkus Harness is a **game-development and software-verification project, not a cybersecurity project**. Repository work is limited to the game-authoring harness, its own source code, fixtures, tests, CI and documentation. Historical terminology such as `self-attack`, `attack fixture`, `bypass` or `adversarial review` means ordinary internal negative/conformance testing only. New work should use the neutral vocabulary defined in `AGENTS.md`.

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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B`, `WP-HK-08A`, `WP-HK-08B`, `WP-HK-09A` and `WP-HK-09B` are COMPLETE.

Latest accepted workpack: `WP-HK-09B — Resource limits + persistence integrity`.

- implementation PR: `#56`;
- baseline SHA: `ada532f99282c96db15813eb17963bc9cb6d08fb`;
- reviewed frozen candidate: `8ed02586da9a5b6e159e1cdc76a47ae7ca89c763`;
- independent Reviewer: `PASS` (PR review `#5261068513`);
- exact-SHA freeze validation: GREEN, Actions `35522581043`, artifact `10609077150`;
- implementation merge SHA: `2e7a258fdcec2e26492d308c3cb199ab62201dcd`;
- accepted resource envelope: `arkus.h0-resource-envelope@1` is discoverable through `system.resource-envelope.describe@1.0`; canonical arguments are bounded to 896 KiB, portable nesting to 32, mutation batches to 96 operations, decoded mutation payload to 512 KiB, query pages to 100 items, canonical world/snapshot state to 640 KiB, world resources to 10,000, mutation transactions to 10,000, snapshot-import receipts to 1,024 and cooperative execution to 5 s;
- accepted transport compatibility: `arkus.reference.jsonl@1` remains frozen at 1,048,576 bytes with `transport.frame_too_large`; HK09B's neutral envelope is deliberately below that boundary so successfully framed JSONL/MCP requests converge on the same resource semantics rather than redefining the transport contract;
- accepted publication integrity: materialized Authoring state is independently resource-checked before publication; mutation, snapshot import and replay stage complete aggregates and perform one authoritative publication only after budget/interruption admission, preventing rejected/expired/interrupted work from publishing partial canonical state or false success evidence;
- accepted durability boundary: `durabilityLevel=process-local-checkpoint`, `publicationBoundary=validate-stage-aggregate-publish`, `powerLossDurabilityClaimed=false`; no WAL/fsync/crash-recovery/distributed persistence guarantee is implied;
- accepted content/interaction boundary: the representative HK08A 96-operation Potes-shaped coherent edit still commits once, and real JSONL/MCP processes agree on neutral byte/page/depth resource diagnostics;
- accepted downstream ownership: HK10 owns bounded long-session endurance and broader strict closure against the accepted envelope; arbitrary production-scale streaming, large-world asymptotics, shipping SLOs, multi-process/distributed writers and power-loss durability remain outside HK09B.

HK09B had one Reviewer repair cycle. Frozen SHA `dcea0901a4fe78549d81271a7f192bbae77e58b7` failed review `#5261028914` because it silently widened the already-frozen `arkus.reference.jsonl@1` frame from 1 MiB to 2 MiB, changed the oversized-frame diagnostic and modified the inherited HK07A regression oracle to follow the new behavior. The repair restored the HK07A implementation/oracle exactly and moved the HK09B neutral ceilings below the inherited frame (896 KiB request, 640 KiB world/snapshot) instead of versionlessly redefining transport semantics. The fresh Reviewer passed the repaired frozen candidate above.

## Current next product target

The original HK06/HK07 and later HK08/HK09 monoliths were split **before implementation** to keep foundational claims independently reviewable without reducing scope. The old `WP-HK-06.md`, `WP-HK-07.md`, `WP-HK-08.md` and `WP-HK-09.md` are SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A ✅ → HK07B ✅ → HK08A ✅ → HK08B ✅ → HK09A ✅ → HK09B ✅ → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-10 — Strict quality closure`.

HK10 is a closure workpack, not an architecture workpack. It consumes the accepted H0 surface and must challenge it broadly with property-based tests, malformed/schema-invalid inputs, seeded validator/provenance/transaction defects, persistence interruption, thrown handler/validator failures, cancellation, deterministic stale/conflicting writers and restart/recovery cases. It must also execute a bounded long-authoring-session endurance flow using the accepted HK08A/HK08B interaction primitives **inside the HK09B resource envelope** and record process/session growth rather than assuming future compaction or scaling work will save the gate.

HK10 must preserve the accepted HK09A capability boundary and HK09B resource/persistence semantics rather than inventing replacement subsystems. A material semantic gap discovered during closure reopens or amends the causal owning workpack; it is not buried as a HK10 feature. Its residual-risk audit must reconcile against the independently maintained `Docs/engineering/RESIDUAL_LEDGER.md`, classifying every declared predecessor residual as in-boundary with a causal seeded control, outside the gate boundary and named, or already closed by accepted evidence.

Read `Docs/workpacks/HK/WP-HK-10.md`, `Docs/engineering/RESIDUAL_LEDGER.md`, the accepted HK09B verdict/resource/interruption evidence, and the predecessor proof/negative matrices before implementation.

After HK10, `WP-HK-GATE` exercises end-to-end AI-authoring readiness on the representative micro-world using the closed H0 guarantees. Whole-world CAS/hash remains the H0 global consistency anchor but does **not** require clients or AI agents to reload/reconstruct the complete world after each commit. HK08A supplies bounded/compact interaction primitives, HK08B supplies truthful bounded same-lineage stale recovery, HK09A constrains host powers and HK09B supplies finite resource/publication boundaries. Finer concurrent-writer semantics remain post-GATE unless measured evidence promotes them.

After HK-GATE, `Docs/ROADMAP.md` defines a non-blocking-by-default H0S scale/concurrency track that may run in parallel with H1. It measures real object counts, commit cost, collision/stale rate, recovery cost and memory before selecting incremental hashing/indexing, resource-scoped preconditions, coordination/leases, change feeds or finer state partitioning. No Merkle/per-resource-CAS/scope-lock design is preselected, and no transport adapter may become a separate concurrency authority.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, finalization is exact-SHA merge preflight → merge (or recognition of Automation V2 exact-SHA auto-merge) → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL returns to a fresh repair Worker; a Reviewer never repairs a failed candidate. Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A ✅ → HK-07B ✅ → HK-08A ✅ → HK-08B ✅ → HK-09A ✅ → HK-09B ✅ → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
