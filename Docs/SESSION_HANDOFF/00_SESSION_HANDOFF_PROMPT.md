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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C` and `WP-HK-07A` are COMPLETE.

Latest accepted workpack: `WP-HK-07A — Headless host + neutral projection contract + reference transport`.

- implementation PR: `#44`;
- baseline SHA: `9173dcef32f6e020b64c3db2816fe5f4d0994058`;
- reviewed frozen candidate: `f2ef88980b38482a1a635f4eeb0582ee49d735ec`;
- independent Reviewer: `PASS` (PR review `#5259732343`);
- exact-SHA validation: GREEN, Actions `35492562005`, artifact `10599004891`;
- merge SHA: `f70a81b5115cd2a6b0c8b1cb9c5dd657d5f3792b`;
- accepted semantic addition: non-interactive process-local canonical host; `arkus.neutral-projection@1` as the accepted transport-neutral projection contract; deterministic `arkus.reference.jsonl@1` framing as the first reference transport; generic composed-inventory discovery and dispatch; stable transport/canonical/cancelled/timed-out outcomes; strict framing and stdout/stderr separation; fresh external-process exercise of accepted read, validation, mutation, provenance, diff, snapshot and replay flows;
- accepted scope limits: no MCP implementation, HTTP/cloud service, Unity/editor integration, durable storage/crash recovery, authentication/network tenancy, multi-process writer coordination, post-dispatch execution abort/resource governor, HK08 interaction-efficiency/recovery semantics or gameplay/runtime-state semantics.

HK07A did not reopen accepted HK01-HK06C semantics. It projects the accepted composed `ComposedContract` and canonical dispatcher rather than owning a transport registry or write path. Cancellation/timeout are explicitly admission-only: once synchronous canonical dispatch begins, the canonical outcome remains authoritative.

The independent Reviewer specifically checked completeness against scoped composition rather than the observed production capability count, JSONL/framing isolation from the neutral layer, Runtime/Projection dependency direction, representative process-boundary mutation/provenance/snapshot/diff/replay behaviour, and downstream HK07B neutrality. The only review note was non-blocking evidence wording: the xUnit assembly has product references even though the external-client code path itself communicates only through compiled child processes and calls no implementation API.

## Current next product target

The original HK06 and HK07 monoliths were split **before implementation** to reduce coupled foundational freeze/review risk without reducing scope. HK08 and HK09 were later split on the same principle after their planned contracts accumulated two independently reviewable claims each. The old `WP-HK-06.md`, `WP-HK-07.md`, `WP-HK-08.md` and `WP-HK-09.md` are SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A ✅ → HK07B → HK08A → HK08B → HK09A → HK09B → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-07B — MCP as second projection + cross-transport conformance`.

HK07B must implement MCP as a standards-compatible **second projection** of the already accepted HK07A neutral projection contract. It must not change canonical or neutral semantics merely to make MCP convenient. Discovery/request/result/error meaning must be derived from or mechanically reconciled with the composed canonical contract; scoped capabilities must appear without an MCP-owned registry; MCP must expose no adapter-only mutation path; cancellation/error behaviour must map to the accepted HK07A semantics; and representative discovery, read, validation, mutation, provenance, diff, snapshot and replay flows must be semantically equivalent across JSONL and MCP. Read `Docs/workpacks/HK/WP-HK-07B.md`, `Docs/reference/HK07A_REFERENCE_TRANSPORT.md` and accepted HK07A evidence before implementation.

After 07B PASS+merge+DocSync: HK08A owns atomic batching plus compact/bounded reads and pagination; HK08B owns structured stale-CAS recovery, repair ergonomics and measured end-to-end interaction budgets; HK09A owns repository-local host capability containment; HK09B owns explicit resource limits plus import/persistence interruption integrity; HK10 remains the unsplit closure workpack before HK-GATE.

Whole-world CAS/hash remains the H0 global consistency anchor but does **not** require clients or AI agents to reload/reconstruct the complete world after each commit. Bounded revision-anchored inspection, semantic diff and HK08B recovery are the client-side coherence mechanisms. Serialized commit execution alone does not cure a stale plan; a request planned on an old revision still requires rejection/recovery/re-plan.

After HK-GATE, `Docs/ROADMAP.md` defines a non-blocking-by-default H0S scale/concurrency track that may run in parallel with H1. It measures real object counts, commit cost, collision/stale rate, recovery cost and memory before selecting incremental hashing/indexing, resource-scoped preconditions, coordination/leases, change feeds or finer state partitioning. No Merkle/per-resource-CAS/scope-lock design is preselected, and no transport adapter may become a separate concurrency authority.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the same inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, the successful Reviewer/finalization session should close the accepted cycle immediately when permissions permit: exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. No extra human session is required solely for routine DocSync. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL still returns to a fresh repair Worker; a Reviewer never repairs a failed candidate.

Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A ✅ → HK-07B → HK-08A → HK-08B → HK-09A → HK-09B → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.