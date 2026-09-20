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

`WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B`, `WP-HK-08A`, `WP-HK-08B`, `WP-HK-09A`, `WP-HK-09B` and `WP-HK-10` are COMPLETE.

Latest accepted workpack: `WP-HK-10 — Strict quality closure`.

- implementation PR: `#60`;
- baseline SHA: `8651a7bd55180297c3621336e9e64a2e6a211aef`;
- reviewed frozen candidate: `813ccf08e33fdd77a34c59da5ed766882840cbee`;
- independent Reviewer: `PASS` (PR review `#5261301248`);
- exact-SHA freeze validation: GREEN, Actions `35527218067`;
- implementation merge SHA: `f893ad51d756090eeecac41b8fc7cb14f8bd359a`;
- accepted strict closure: twelve deterministic causal negative-conformance controls across the named H0 foundational layers, deterministic property/robustness coverage, Protocol v1 compatibility corpus, cancellation/fault-injection coverage and full regression;
- accepted endurance: 512 canonical authoring transactions with periodic inspect/validate/journal/snapshot operations and fresh-session snapshot recovery inside the accepted HK09B resource envelope; telemetry is evidence of bounded H0 behaviour, not a shipping SLO;
- accepted residual reconciliation: `Docs/evidence/WP-HK-10/RESIDUAL_RISK.md` classifies every independently maintained ledger entry with `UNCLASSIFIED_RESIDUALS: 0`; proof evidence records `UNRESOLVED_PROOF_OBLIGATIONS: 0` and `KNOWN_UNDETECTED_DEFECT_CLASSES: 0`;
- accepted HK01 amendment: canonical handler exceptions now have defined structured pre-publication and post-publication outcomes. This semantic gap was discovered by HK10 but is causally owned by the reopened/amended `WP-HK-01`; the amendment was independently accepted on the same exact candidate;
- scope remains H0: no claim of WAL/fsync/power-loss durability, arbitrary production scale, shipping latency/throughput SLOs, automatic merge, per-resource CAS, distributed writers, authentication/remote tenancy or H1 gameplay/runtime semantics.

HK10 had one Reviewer repair cycle. Frozen SHA `dabcbeb9cce071ed6fca29a8fa01030127f2ce98` failed review `#5261225652` because the candidate had introduced the useful canonical dispatcher exception boundary as HK10-owned product semantics, violating HK10's closure-only rule. The repair explicitly reopened/amended HK01 as causal owner, added `Hk01DispatchFailureContractTests` for both pre/post-publication branches, retained HK10's handler/validator fixtures only as downstream closure evidence, refroze, revalidated and then received fresh independent PASS.

## Current next product target

The original HK06/HK07 and later HK08/HK09 monoliths were split **before implementation** to keep foundational claims independently reviewable without reducing scope. The old `WP-HK-06.md`, `WP-HK-07.md`, `WP-HK-08.md` and `WP-HK-09.md` are SUPERSEDED umbrella records and must not be implemented directly.

Execution chain:

`HK06A ✅ → HK06B ✅ → HK06C ✅ → HK07A ✅ → HK07B ✅ → HK08A ✅ → HK08B ✅ → HK09A ✅ → HK09B ✅ → HK10 ✅ → HK-GATE`

The next dependency-valid workpack is `WP-HK-GATE — AI authoring readiness gate`.

Gate is the final H0 product-readiness proof, not another architecture-growth workpack. From a clean checkout, a deterministic reference client and one fresh independent AI-agent trial must use public discovery/client-facing surfaces rather than C# implementation knowledge to create, inspect, validate, repair, mutate, recover from stale state, snapshot/restart/replay, diff and explain a representative micro-world. The flow must exercise the accepted HK08A interaction shape, HK08B conflict recovery, HK09A authority boundary, HK09B resource/publication limits and HK10 bounded-endurance closure, including semantic equivalence through reference JSONL and MCP.

Read `Docs/workpacks/HK/WP-HK-GATE.md`, the accepted HK10 verdict/proof/residual evidence, and the predecessor gate-facing evidence before implementation. Gate must name the accepted residual boundary rather than silently promoting post-GATE scale/concurrency/durability/security claims into H0 blockers.

Only after independent `WP-HK-GATE` PASS may detailed H1 Engine Bridge / Unity-first workpacks begin. A Gate PASS still does not authorize gameplay directly; Unity first receives its own downstream bridge/parity work.

After HK-GATE, `Docs/ROADMAP.md` defines a non-blocking-by-default H0S scale/concurrency track that may run in parallel with H1. It measures real object counts, commit cost, collision/stale rate, recovery cost and memory before selecting incremental hashing/indexing, resource-scoped preconditions, coordination/leases, change feeds or finer state partitioning. No Merkle/per-resource-CAS/scope-lock design is preselected, and no transport adapter may become a separate concurrency authority.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise Telegram notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 retains mandatory predecessor-contract reconstruction, bounded representative content-shape probes for applicable foundational WPs and same-session post-PASS finalization/DocSync. Worker records `Predecessor contract check` before implementation. Reviewer independently reconstructs the inherited/current ownership split and may not issue FAIL for an accepted predecessor guarantee merely because the current WP does not redundantly re-prove it; concrete evidence of inapplicability/falsehood is required.

Use the exact handoff enum values defined by the protocol (`FROZEN_FOR_REVIEW`, `IN_REVIEW`, etc.); alternate aliases can be rejected by Automation V2 even when the candidate itself is valid.

A fresh independent Reviewer remains mandatory. On exact-SHA PASS, finalization is exact-SHA merge preflight → merge (or recognition of Automation V2 exact-SHA auto-merge) → documentation-only DocSync → `DOCSYNC_COMPLETE` with dependency-valid `Next WP`. The post-PASS phase may not modify implementation bytes or reconsider the reviewed candidate; any required implementation change starts a new Worker/review cycle.

FAIL returns to a fresh repair Worker; a Reviewer never repairs a failed candidate. Telegram is convenience only. GitHub state and accepted evidence remain authoritative.

## H0 order

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A ✅ → HK-07B ✅ → HK-08A ✅ → HK-08B ✅ → HK-09A ✅ → HK-09B ✅ → HK-10 ✅ → HK-GATE`.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same WP. PASS binds the exact SHA. Accepted predecessor guarantees compose forward and are consumed rather than re-proved unless contradictory evidence reopens them. After PASS, merge and DocSync should be completed in the same successful finalization flow before the next WP is selected. The durable close marker is `DOCSYNC_COMPLETE` with the dependency-valid `Next WP`.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
