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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A` and `WP-HK-07B` are COMPLETE.

Latest accepted workpack: `WP-HK-07B — MCP as second projection + cross-transport conformance`.

- implementation PR: `#47`;
- baseline SHA: `6173471245da081b21cf66fd164cbb3a30dcba1d`;
- reviewed frozen candidate: `37ea185f28dd28e41b406f2c9407fdfe6f9b752b`;
- independent Reviewer: `PASS` (PR review `#5259854121`);
- exact-SHA freeze validation: GREEN, Actions `35495563546`, artifact `10600393826`;
- merge SHA: `58b6571b96eac4c73e5c3cf28a42a6630ea13505`;
- accepted semantic addition: local-stdio MCP as a genuine second first-party projection of accepted `arkus.neutral-projection@1`; canonical composition remains the sole capability/schema/dispatch authority; scoped providers project without an MCP registry; representative accepted H0 read, validation, mutation, provenance, diff, snapshot and replay semantics remain equivalent to the deterministic JSONL reference transport; MCP framing/timeout metadata stay adapter-local; the pinned MCP SDK remains replaceable behind Arkus conformance;
- accepted naming boundary: short canonical keys retain reversible MCP encoding; canonical-valid keys whose reversible tool name exceeds MCP's 128-character limit receive deterministic bounded adapter-local handles, while exact canonical identity remains in metadata/descriptor lookup and drives neutral dispatch;
- accepted scope limits: no HTTP/cloud/authentication, vendor-specific orchestration, Unity/editor bridge, durable/crash-recovery store, multi-process writer coordination, batching/compact/pagination efficiency semantics, structured stale-conflict recovery or gameplay/runtime-state semantics.

HK07B had one repair cycle. Frozen SHA `1447e56642414ce2e75219fdfcb191ada41d6378` failed review `#5259817513` because an accepted long canonical capability could make MCP discovery throw. The repair remained inside HK07B adapter/test ownership, preserved HK07A/canonical semantics unchanged, added causal two-capability over-limit composition/discovery/invocation coverage, and then received the fresh PASS above.

## Current next product target

The original HK06/HK07 and later HK08/HK09 monoliths were split **before implementation** to keep foundational claims independently reviewable without reducing scope. The old `WP-HK-06.md`, `WP-HK-07.md`, `WP-HK-08.md` and `WP-HK-09.md` are SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A ✅ → HK07B ✅ → HK08A → HK08B → HK09A → HK09B → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-08A — Efficient interaction primitives`.

HK08A owns **atomic batching, compact/bounded reads, pagination and cheap discovery metadata** while preserving accepted canonical semantics and the now-accepted JSONL↔MCP transport neutrality. A representative coherent multi-resource authoring intent must fit in one accepted atomic batch rather than being split solely by an arbitrary implementation cap; the current 64-operation limit is a starting constraint, not a permanent semantic constant. Journal reads become bounded/paginated without changing HK06A ordering/completeness. Pagination must not duplicate/omit resources and must fail closed on stale anchors. Every primitive HK08A adds or changes must rerun cross-transport conformance through both reference transport and MCP. HK08A does **not** own structured stale-CAS recovery, diagnostic repair prioritization or final interaction budgets; those belong to HK08B.

Read `Docs/workpacks/HK/WP-HK-08A.md`, accepted HK07A/HK07B verdict/proof evidence, and the H0 interaction/concurrency section of `Docs/ROADMAP.md` before implementation.

After HK08A: HK08B owns structured same-lineage stale-CAS recovery, repair ergonomics and measured end-to-end interaction budgets; HK09A owns repository-local host capability containment; HK09B owns explicit resource/input limits plus import/persistence interruption integrity; HK10 remains the closure workpack before HK-GATE.

Whole-world CAS/hash remains the H0 global consistency anchor but does **not** require clients or AI agents to reload/reconstruct the complete world after each commit. Bounded revision-anchored inspection, semantic diff and HK08B recovery are the client-side coherence mechanisms. Serialized commit execution alone does not cure a stale plan; a request planned on an old revision still requires rejection/recovery/re-plan.

After HK-GATE, `Docs/ROADMAP.md` defines a non-blocking-by-default H0S scale/concurrency track that may run in parallel with H1. It measures real object counts, commit cost, collision/stale rate, recovery cost and memory before selecting incremental hashing/indexing, resource-scoped preconditions, coordination/leases, change feeds or finer state partitioning. No Merkle/per-resource-CAS/scope-lock design is preselected, and no transport adapter may become a separate concurrency authority.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, finalization is exact-SHA merge preflight → merge (or recognition of Automation V2 exact-SHA auto-merge) → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL returns to a fresh repair Worker; a Reviewer never repairs a failed candidate. Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A ✅ → HK-07B ✅ → HK-08A → HK-08B → HK-09A → HK-09B → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
