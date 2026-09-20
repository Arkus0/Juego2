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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B`, `WP-HK-08A` and `WP-HK-08B` are COMPLETE.

Latest accepted workpack: `WP-HK-08B — Structured conflict recovery + agent interaction benchmark`.

- implementation PR: `#52`;
- baseline SHA: `dc8b6ee3d4ea62df70e28605268526c726de69d8`;
- reviewed frozen candidate: `31370f91b48408b90a587d7ad5178ba1be8d6bfe`;
- independent Reviewer: `PASS` (PR review `#5260337340`);
- exact-SHA freeze validation: GREEN, Actions `35503766435`, artifact `10603256605`;
- implementation merge SHA: `bdf4675c17d842d73ff59637fa36314d14c2707a`;
- accepted recovery truth: `arkus.world-conflict-recovery@1` emits a precise deterministic changed-resource delta only when the exact expected revision+hash is proven in the complete contiguous current local HK06A lineage; missing/gapped history or a non-ancestor/rebase lineage fails closed as `bounded-reinspection-required` with no fabricated delta;
- accepted recovery path: ordinary same-lineage stale recovery uses only affected-resource inspection descriptors and retries through normal public `plan` → `dry-run` → `apply`, preserving HK04/HK05/HK06A authority and provenance rather than introducing merge or alternate mutation authority;
- accepted benchmark: representative create/compact-inspect/coherent-modify/invalid→repair/conflict-recovery flow stays at 12 public requests, performs zero recovery full-world reloads and is guarded by executable request/response/elapsed regression limits;
- accepted transport/content-shape boundary: recovery meaning is equivalent through real JSONL and MCP processes, and the required approved Juego2 market/plaza/bar/workshop content-shape probe passes without adding product-specific schemas to H0;
- accepted scope limits: no per-resource CAS/locks, automatic merge, multi-process writer coordination, autonomous multi-agent scheduling, durable ancestry across restart, product latency SLO, final host quotas, Unity/editor integration or gameplay/runtime-state semantics.

HK08B had one Reviewer repair cycle. Frozen SHA `470665b0bf5d709142bb2de1b1650fec80bba705` failed review `#5260300588` solely because the binding foundational-proof standard required a bounded representative content-shape probe and the exact-SHA gate could still go GREEN without one. The repair added the executable `VISUAL_BIBLE`-derived probe, its representability/identity/granularity boundary analysis and verifier linkage without changing production recovery/CAS/journal/transports; the fresh Reviewer then passed the repaired frozen candidate above.

## Current next product target

The original HK06/HK07 and later HK08/HK09 monoliths were split **before implementation** to keep foundational claims independently reviewable without reducing scope. The old `WP-HK-06.md`, `WP-HK-07.md`, `WP-HK-08.md` and `WP-HK-09.md` are SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A ✅ → HK07B ✅ → HK08A ✅ → HK08B ✅ → HK09A → HK09B → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-09A — Capability containment boundary`.

HK09A consumes the accepted H0 canonical/transport/recovery surface and owns **which host powers exist at all**. It must keep generic shell/process execution absent, keep ambient network access unavailable from canonical/protocol paths, constrain filesystem authority to reviewed harness-owned locations with parent/symlink escapes failing closed, keep runtime-type selection bounded, and enforce the policy below JSONL/MCP so no transport can manufacture additional host authority. This is repository-local software capability containment, not external-system testing. HK09A does **not** own numeric quotas, time/memory/input/output caps or interrupted-persistence integrity; those remain HK09B.

Read `Docs/workpacks/HK/WP-HK-09A.md`, the accepted HK08B verdict/residual evidence, HK07A/HK07B host/transport boundaries and the H0 interaction/concurrency section of `Docs/ROADMAP.md` before implementation.

After HK09A: HK09B owns explicit resource/input limits plus import/persistence interruption integrity; HK10 remains the closure workpack before HK-GATE.

Whole-world CAS/hash remains the H0 global consistency anchor but does **not** require clients or AI agents to reload/reconstruct the complete world after each commit. HK08A supplies bounded/compact interaction primitives and HK08B now supplies truthful bounded same-lineage stale recovery. Finer concurrent-writer semantics remain post-GATE unless later measured evidence promotes them.

After HK-GATE, `Docs/ROADMAP.md` defines a non-blocking-by-default H0S scale/concurrency track that may run in parallel with H1. It measures real object counts, commit cost, collision/stale rate, recovery cost and memory before selecting incremental hashing/indexing, resource-scoped preconditions, coordination/leases, change feeds or finer state partitioning. No Merkle/per-resource-CAS/scope-lock design is preselected, and no transport adapter may become a separate concurrency authority.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, finalization is exact-SHA merge preflight → merge (or recognition of Automation V2 exact-SHA auto-merge) → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL returns to a fresh repair Worker; a Reviewer never repairs a failed candidate. Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A ✅ → HK-07B ✅ → HK-08A ✅ → HK-08B ✅ → HK-09A → HK-09B → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
