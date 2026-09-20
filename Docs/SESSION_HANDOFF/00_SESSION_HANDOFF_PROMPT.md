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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B` and `WP-HK-08A` are COMPLETE.

Latest accepted workpack: `WP-HK-08A — Efficient interaction primitives`.

- implementation PR: `#50`;
- baseline SHA: `13db4f890f4b6fe917fdf0d0c4e99cecfeed368d`;
- reviewed frozen candidate: `324102d40fa0b7e36c7320216914f9d3fddadcc8`;
- independent Reviewer: `PASS` (PR review `#5260092080`);
- exact-SHA freeze validation: GREEN, Actions `35499569973`, artifact `10601679693`;
- merge SHA: `eb9d3df58df1114abafaca435da64683fdb1480e`;
- accepted interaction shape: one representative 96-operation mixed object/extension authoring intent remains one HK04 transaction, one revision and one HK06A provenance entry; compact reads retain required world anchors while omitting optional fields; canonical discovery exposes relative cost/side-effect/batching metadata;
- accepted journal version boundary: `authoring.journal.read@1.0` remains the accepted empty-request complete `arkus.authoring.journal@1` read; bounded deterministic pagination is an explicit breaking `authoring.journal.read@2.0` contract with integrity-bound continuation and stale-anchor failure; paged responses are not silently treated as complete HK06C replay artifacts;
- accepted transport boundary: HK08A changed batch/compact/pagination/discovery shapes are exercised through real JSONL and MCP processes and remain semantically equivalent;
- accepted scope limits: no stale-CAS recovery planner, repair prioritization, automatic merge, multi-plan atomicity, final interaction/resource budgets, durable storage, Unity/editor integration or gameplay/runtime-state semantics.

HK08A had one Reviewer repair cycle. Frozen SHA `0b835891d70666dab41846017e79eb3f7c3b311a` failed review `#5260042576` because it changed `authoring.journal.read@1.0` from complete-journal semantics to default-bounded pagination without a version increment. The repair restored v1 exactly and moved pagination to explicit v2, preserving HK06A/HK06C semantics and the HK08B ownership boundary; the fresh Reviewer then passed the repaired frozen candidate above.

## Current next product target

The original HK06/HK07 and later HK08/HK09 monoliths were split **before implementation** to keep foundational claims independently reviewable without reducing scope. The old `WP-HK-06.md`, `WP-HK-07.md`, `WP-HK-08.md` and `WP-HK-09.md` are SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A ✅ → HK07B ✅ → HK08A ✅ → HK08B → HK09A → HK09B → HK10 → HK-GATE`

The next dependency-valid workpack is `WP-HK-08B — Structured stale-CAS recovery + agent interaction benchmark`.

HK08B consumes the accepted HK08A efficient primitives and owns **structured same-lineage stale-CAS recovery, repair ergonomics and measured end-to-end agent interaction budgets**. It must make ordinary stale-plan recovery cheap and machine-readable without changing the accepted whole-world CAS consistency model: return trustworthy bounded context anchored to expected/current authored state, preserve intent where possible, distinguish when lineage/history is insufficient, and route retry through the normal canonical transaction path rather than inventing an automatic merge authority. Recovery/repair semantics must remain transport-neutral and rerun JSONL↔MCP conformance. HK08B must measure representative create/inspect/modify/correct flows rather than importing arbitrary final resource caps from HK09B.

Read `Docs/workpacks/HK/WP-HK-08B.md`, accepted HK08A verdict/proof evidence, the HK06A/HK06B lineage/diff contracts, and the H0 interaction/concurrency section of `Docs/ROADMAP.md` before implementation.

After HK08B: HK09A owns repository-local host capability containment; HK09B owns explicit resource/input limits plus import/persistence interruption integrity; HK10 remains the closure workpack before HK-GATE.

Whole-world CAS/hash remains the H0 global consistency anchor but does **not** require clients or AI agents to reload/reconstruct the complete world after each commit. HK08A now supplies the bounded/compact interaction primitives; HK08B must supply the bounded same-lineage recovery semantics when an authored plan is stale. Serialized commit execution alone does not cure a stale plan.

After HK-GATE, `Docs/ROADMAP.md` defines a non-blocking-by-default H0S scale/concurrency track that may run in parallel with H1. It measures real object counts, commit cost, collision/stale rate, recovery cost and memory before selecting incremental hashing/indexing, resource-scoped preconditions, coordination/leases, change feeds or finer state partitioning. No Merkle/per-resource-CAS/scope-lock design is preselected, and no transport adapter may become a separate concurrency authority.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, finalization is exact-SHA merge preflight → merge (or recognition of Automation V2 exact-SHA auto-merge) → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL returns to a fresh repair Worker; a Reviewer never repairs a failed candidate. Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A ✅ → HK-07B ✅ → HK-08A ✅ → HK-08B → HK-09A → HK-09B → HK-10 → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
