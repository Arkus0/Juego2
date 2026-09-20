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
- exact-SHA freeze validation: GREEN, Actions `35522581043` (Candidate Validation run #470);
- implementation merge SHA: `2e7a258fdcec2e26492d308c3cb199ab62201dcd`;
- accepted envelope: `arkus.h0-resource-envelope@1`, discoverable through `system.resource-envelope.describe@1.0` — 896 KiB canonical request bytes, depth 32, 96 mutation operations, 512 KiB decoded mutation payload, 256 relations per resource, 256 KiB extension payload, page size 100, 640 KiB / 10,000-resource canonical world, 10,000 session mutation transactions, 1,024 snapshot import receipts, 5,000 ms execution budget;
- accepted enforcement layering: `H0ResourcePolicy` admits portable bytes/depth/batch/payload/relations/page/snapshot size before canonical dispatch and `WorldResourceLimits` independently rechecks the materialized canonical state before publication, so adapter-side estimates are never the semantic authority;
- accepted persistence boundary: mutation, snapshot import and replay each stage a complete aggregate — state plus receipt, lineage, rebase evidence and journal — and publish atomically, so an interrupted, expired or rejected writer cannot publish a partial world, advance revision/hash or acquire false success evidence; `durabilityLevel=process-local-checkpoint`, `publicationBoundary=validate-stage-aggregate-publish`, `powerLossDurabilityClaimed=false`;
- accepted compatibility derivation: `arkus.reference.jsonl@1` keeps its frozen 1,048,576-byte frame and `transport.frame_too_large` behaviour; the HK09B envelope fits beneath it (128 KiB framing headroom, 640 KiB world ≤ 873,816 base64 characters) so the advertised snapshot envelope stays traversable through the inherited reference transport and not only through MCP;
- accepted scope limits: the execution budget is cooperative rather than preemptive, power-loss/process-crash durability is explicitly not claimed, neutral resource equivalence is claimed only once a request is successfully framed, and multi-process/distributed writers, per-resource locks and engine/editor persistence remain outside H0.

HK09B had one Reviewer repair cycle. Frozen SHA `dcea0901a4fe78549d81271a7f192bbae77e58b7` failed review `#5261028914` because the candidate changed the frozen `arkus.reference.jsonl@1` framing contract in place — 1 MiB to 2 MiB, `transport.frame_too_large` to `resource.request_bytes_exceeded` — and rewrote the inherited HK07A regression oracle to match, so the green suite was a false green against the predecessor compatibility guarantee. The repair restored the HK07A implementation and its oracle exactly to baseline and fitted the HK09B neutral envelope beneath the inherited frame instead of widening it. Ownership transferred mid-workpack (ChatGPT Codex → ChatGPT) at transfer SHA `e370606e4278da3e08602a3167c1cb6513bea93d`.

## Current next product target

The original HK06/HK07 and later HK08/HK09 monoliths were split **before implementation** to keep foundational claims independently reviewable without reducing scope. The old `WP-HK-06.md`, `WP-HK-07.md`, `WP-HK-08.md` and `WP-HK-09.md` are SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A ✅ → HK07B ✅ → HK08A ✅ → HK08B ✅ → HK09A ✅ → HK09B ✅ → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-10 — Strict quality closure`.

HK10 is a **closure** workpack, not a new architecture workpack. It consumes every accepted H0 guarantee and challenges it: property-based coverage of canonical serialization/hash, transaction atomicity, idempotency, query determinism and replay equivalence; malformed/truncated/unknown-version/unknown-command/schema-invalid robustness; defect-injection controls proving the material validator/provenance/transaction oracles actually go red causally; fault injection across persistence interruption, thrown handler/validator failures, cancellation and restart; deterministic stale/conflicting-writer tests including the accepted HK08B recovery path; capability-closure tests consuming the HK09A authority boundary; and a compatibility corpus locking accepted Protocol v1 behaviour.

Two obligations are specific to the newly accepted predecessors. A bounded long-authoring-session endurance case must repeatedly exercise representative inspect/validate/mutate/journal/snapshot flows inside the accepted HK09B envelope and record process/session growth; if growth exceeds the declared ceilings, that is resolved explicitly rather than assumed away by future compaction. Required negative-conformance tests must include a stale-conflict defect that loses or misanchors HK08B recovery context and a bounded-session/resource-growth defect that exceeds an accepted HK09B limit without being detected.

HK10's residual-risk audit is bound to `Docs/engineering/RESIDUAL_LEDGER.md` as an independently obtained universe it did not compose itself. Every entry must be resolved as inside the boundary with a seeded causal control, outside the boundary and named for the gate, or closed by cited accepted evidence. The ledger is additive by design: entries are not removed to shorten that list, and removing one requires citing the accepted evidence that closed it.

A material product-semantic gap found in HK10 reopens or amends the causal owning workpack rather than being buried inside HK10 hardening.

Read `Docs/workpacks/HK/WP-HK-10.md`, `Docs/engineering/RESIDUAL_LEDGER.md` v1.9, the accepted HK09B verdict/resource-envelope/persistence-interruption evidence and the accepted HK08B recovery and HK09A containment evidence before implementation.

After HK10: HK-GATE exercises end-to-end AI-authoring readiness on the representative micro-world.

Whole-world CAS/hash remains the H0 global consistency anchor but does **not** require clients or AI agents to reload/reconstruct the complete world after each commit. HK08A supplies bounded/compact interaction primitives and HK08B supplies truthful bounded same-lineage stale recovery. HK09A constrains the host powers beneath those flows and HK09B now bounds what they may consume and how publication fails closed. Finer concurrent-writer semantics remain post-GATE unless later measured evidence promotes them.

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
