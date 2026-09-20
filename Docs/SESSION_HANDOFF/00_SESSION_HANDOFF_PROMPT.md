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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B`, `WP-HK-08A`, `WP-HK-08B` and `WP-HK-09A` are COMPLETE.

Latest accepted workpack: `WP-HK-09A — Capability containment boundary`.

- implementation PR: `#54`;
- baseline SHA: `b2a666554e88a543013ece8751ab781637f7db55`;
- reviewed frozen candidate: `acb1ccc341aec5131dc2ef979bd322e40e208b53`;
- independent Reviewer: `PASS` (PR review `#5260496498`);
- exact-SHA freeze validation: GREEN, Actions `35508529666`, artifact `10604463322`;
- implementation merge SHA: `614ad941881fdefa83fd46a1a8db989cfaba2cbb`;
- accepted authority boundary: production H0 exposes no generic shell/process authority, no protocol-triggered ambient network authority and no caller-selected filesystem path authority; production `--file` is rejected before the legacy framing host can open a path;
- accepted policy boundary: `H0HostCapabilityPolicy` rejects external side effects, elevated/unknown privilege and contradictory mutation/rebase/replay policy metadata, while `NeutralProjectionService(ComposedContract)` independently enforces H0 admission before exposing capabilities or accepting dispatch;
- accepted transport consequence: JSONL, MCP and any future adapter conforming to the accepted HK07A neutral-projection boundary cannot skip H0 admission by composing a generic canonical contract directly, and adapters cannot mint an independent host-power registry;
- accepted type/content boundary: protocol-controlled runtime-type selectors cannot acquire activation authority, and the approved Juego2 Potes market/plaza/bar/workshop slice still passes inspect → author → snapshot/import → replay under containment;
- accepted scope limits: numeric input/batch/page/depth/time/resource limits and persistence/import interruption integrity remain HK09B; authentication/multi-user cloud security, hostile OS/process isolation, anti-cheat and Unity/editor authority are not H0 HK09A claims.

HK09A had one Reviewer repair cycle. Frozen SHA `1c85a64a5d3930ad2e39451fb8db2c1fac6ae78b` failed review `#5260422246` because the host policy was enforced only through `CanonicalWorldContract.Compose(...)`; public generic `ContractComposer.Compose(...)` could still feed `NeutralProjectionService(ComposedContract)` without crossing H0 admission. The repair moved the non-skippable check to neutral-projection construction while keeping the generic composer policy-agnostic and added a causal direct-composition → projection negative that fails before handler invocation. The fresh Reviewer passed the repaired frozen candidate above.

## Current next product target

The original HK06/HK07 and later HK08/HK09 monoliths were split **before implementation** to keep foundational claims independently reviewable without reducing scope. The old `WP-HK-06.md`, `WP-HK-07.md`, `WP-HK-08.md` and `WP-HK-09.md` are SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A ✅ → HK07B ✅ → HK08A ✅ → HK08B ✅ → HK09A ✅ → HK09B → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-09B — Resource limits + persistence integrity`.

HK09B consumes the now-accepted HK09A host-authority boundary rather than re-proving shell/network/filesystem containment. It owns **how much accepted work may consume and how persistence/import fail under limits or interruption**: explicit input, batch, query-page, recursion/depth and execution/resource envelopes; fail-closed stable diagnostics for oversize/deep/over-budget requests with no partial canonical effect; snapshot/import validation before state replacement; rejected imports leaving canonical state/history unchanged; and interruption at the actual H0 persistence publication boundary being unable to publish a partial world or false success evidence.

HK09B must derive its envelope from the accepted HK08A/HK08B representative shapes, not use arbitrary legacy constants to split a coherent 96-operation authoring intent or weaken accepted atomicity/validation/provenance/recovery semantics. Resource enforcement remains below transport adapters so JSONL/MCP observe equivalent meaning. It does not add new host capabilities, distributed/cloud persistence, authentication, large-scale load architecture or multi-agent writer coordination.

Read `Docs/workpacks/HK/WP-HK-09B.md`, the accepted HK09A verdict/residual evidence, HK08A/HK08B interaction evidence and the resource/persistence entries in `Docs/engineering/RESIDUAL_LEDGER.md` before implementation.

After HK09B: HK10 performs strict property/malformed-input/fault-injection quality closure plus bounded endurance against the accepted resource envelope; HK-GATE then exercises end-to-end AI-authoring readiness on the representative micro-world.

Whole-world CAS/hash remains the H0 global consistency anchor but does **not** require clients or AI agents to reload/reconstruct the complete world after each commit. HK08A supplies bounded/compact interaction primitives and HK08B supplies truthful bounded same-lineage stale recovery. HK09A now constrains the host powers beneath those flows. Finer concurrent-writer semantics remain post-GATE unless later measured evidence promotes them.

After HK-GATE, `Docs/ROADMAP.md` defines a non-blocking-by-default H0S scale/concurrency track that may run in parallel with H1. It measures real object counts, commit cost, collision/stale rate, recovery cost and memory before selecting incremental hashing/indexing, resource-scoped preconditions, coordination/leases, change feeds or finer state partitioning. No Merkle/per-resource-CAS/scope-lock design is preselected, and no transport adapter may become a separate concurrency authority.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, finalization is exact-SHA merge preflight → merge (or recognition of Automation V2 exact-SHA auto-merge) → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL returns to a fresh repair Worker; a Reviewer never repairs a failed candidate. Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A ✅ → HK-07B ✅ → HK-08A ✅ → HK-08B ✅ → HK-09A ✅ → HK-09B → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
