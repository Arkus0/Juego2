# Juego2 — Session Handoff

This file is a compact resumption aid, not an authority above current GitHub evidence.

## Project context

Juego2 / Arkus Harness is a game-development and software-verification project. `Arkus0/Juego` is reference only. H0 built an engine-agnostic AI-authoring harness; that phase is now COMPLETE after `WP-HK-GATE` independent PASS.

## Read first

1. `AGENTS.md`
2. `Docs/ROADMAP.md`
3. `Docs/workpacks/H1/README.md`
4. `Docs/engineering/H1_ENGINE_BRIDGE_ARCHITECTURE.md`
5. exact active workpack after a human starts one; currently none
6. `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`
7. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
8. `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md`
9. `Docs/engineering/AUTOMATION_V2.md`

The accepted CTX plan does **not** change this bootstrap list by itself. `WP-CTX-01` owns any later role-specific bootstrap reduction and must independently PASS + merge + DocSync before those rules become binding.

## Current accepted state

All H0 workpacks are COMPLETE:

`HK-00 ✅ → HK-00A ✅ → HK-01 ✅ → HK-02 ✅ → HK-03 ✅ → HK-04 ✅ → HK-02A ✅ → HK-05 ✅ → HK-06A ✅ → HK-06B ✅ → HK-06C ✅ → HK-07A ✅ → HK-07B ✅ → HK-08A ✅ → HK-08B ✅ → HK-09A ✅ → HK-09B ✅ → HK-10 ✅ → HK-GATE ✅`.

Latest accepted workpack: `WP-HK-GATE — AI authoring readiness gate`.

- implementation PR: `#64`;
- baseline SHA: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`;
- reviewed frozen candidate: `0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd`;
- independent Reviewer: `PASS` (PR review `#5261636151`);
- deterministic exact-SHA observation: GREEN, Actions `35533486939`;
- frozen exact-SHA validation: GREEN, Actions `35534660950`;
- independent AI-agent MCP trial: PASS, PR comment `#5752332211`;
- implementation merge SHA: `048d2e449d5ff81e8bcc35bc15664ea4ceec18ca`;
- repair cycles: `1`.

The earlier frozen Gate candidate `2c0df70c1ec8245999e4d144e710816d2f7eb335` failed review `#5261512538` because GATE residual reconciliation did not consume the complete accepted HK10 residual universe and the sole Gate-owned omission control attacked only a declarative stage label. The accepted repair remains proof/evidence infrastructure only: all 53 HK10 residual IDs/classifications are mechanically reconciled, and G1 removes the real unfiltered stage-14 execution while leaving its declaration intact so the normal-path wiring oracle must RED causally.

The accepted final Gate proves the complete 14-stage public readiness scenario without adding new product semantics. The deterministic path covers discovery/schemas, representative authoring, bounded inspection, plan/dry-run/atomic apply, structured invalid-state repair, same-lineage stale recovery without whole-world reconstruction, 96-operation coherent batching, snapshot/journal/replay/diff, reference↔MCP equivalence, HK09A authority, HK09B limits/persistence, HK10 endurance and full headless validation.

The fresh independent AI-agent trial used the exact-SHA self-contained MCP artifact, verified provenance/checksums, performed `tools/list` before product calls, derived argument shapes from returned schemas, completed the representative authoring flow, consumed a structured invalid-state diagnostic, proved no partial commit, repaired the request and closed with validation, snapshot and journal evidence. It reported no implementation-source read, binary semantic inspection, hidden/private product call, Worker-supplied intermediate calls or material undocumented assumptions.

H0 therefore exits with effective foundational proof READY, zero unresolved proof obligations, zero known undetected in-boundary defect classes and an explicit residual boundary. Whole-world CAS remains the accepted H0 consistency model; per-resource locking, automatic merge, distributed/multi-agent writer coordination, shipping-scale SLOs, WAL/fsync/power-loss durability, remote tenancy/auth and future lifecycle/schema growth remain outside the accepted H0 claim unless later measured evidence creates a new owner.

## Current next product target

No H0 workpack remains.

The H1 Engine Bridge / Unity-first architecture and full sequence are ACCEPTED and binding. Planning PR `#71` passed independent review `#5263596722` on frozen candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`, with candidate observation Actions `35567622151` and freeze validation Actions `35567723539` GREEN, then merged as `09ce3fb495d331285bef0ab8aebf4c6117c84d57`.

`WP-H1-00` and `WP-H1-01` are COMPLETE. The next default dependency-valid H1 workpack is `WP-H1-02 — pinned reproducible Unity project/toolchain/package baseline`; it remains `NOT_STARTED` until a human explicitly starts its Worker. Later H1 WPs still require their own accepted predecessors; in particular `WP-H1-03` remains blocked until H1-02 passes, merges and completes DocSync.

H1 local/hybrid execution is remote-first under the accepted PROCESS_ONLY protocol from PR `#99` (frozen candidate `dda339d1a3be72e4f24f2d5d212fafa1bb1f3f36`, merge `c09a00e07ca19a7a9f6d9ebb29ab9ac3c4d87f15`). The remote Worker remains the sole semantic/design/repair/pre-review/freeze owner; local Codex may execute only a predeclared bounded mechanical Unity slice with exact SHA/environment/mutation evidence and returns control to the Worker for interpretation and closeout.

H1 architecture keeps canonical `WorldState` and all H0 authoring authority engine-neutral. Unity binding intent is canonical opaque extension data produced by a schema-aware provider; the Unity catalogue, locators and materialized scene/assets remain bridge-owned derived state. Unity-to-canonical flow is an explicit proposal that must re-enter H0 plan/dry-run/apply. `WP-H1-03A` freezes the missing public execution seam: the external H0/.NET host owns canonical session/composition, while short-lived pinned Unity batch workers execute serialized main-thread Editor plans behind the same reference/MCP handlers. Gameplay and CITY-07 keeper realization remain blocked until `WP-H1-GATE` passes, merges and completes DocSync. CITY-04 is a narrower non-blocking greybox sidecar after CITY-03 + H1-08; it cannot invent bridge semantics or count as H1 evidence by implication.

Accepted CITY programme v2 keeps its own spine and owners. H1 uses `CITY_SPATIAL_CONSTITUTION.md` as representative shape input only. CITY-08's later keeper-slice reuse/cost trial remains distinct from H1-GATE's bounded public-client readiness trial.

The binding final scenario is `Docs/engineering/H1_UNITY_PARITY_GATE.md`. Ordinary H1 WPs use deterministic evidence and seam-specific H0 deltas. One fresh external AI-agent trial is reserved for `WP-H1-GATE`, where public Unity-bridge readiness is the new acceptance claim.

The non-blocking H0S scale/concurrency track may run in parallel with H1 if useful. It starts from measurements rather than a predetermined Merkle/per-resource-CAS/lock design and must not make a transport adapter the semantic concurrency authority.

## Current process-efficiency target

The CTX context-efficiency programme plan is ACCEPTED on frozen candidate `c9ff3605048e05fded4d58a1de27450661d9fe0e` (independent review `#5270686380`, PR `#102`, merge `f3c8362b3d76fd4f78107d8142e07e476985f973`). Its reviewed sequence is `CTX-01 -> CTX-02 -> CTX-03`.

The next CTX action is `WP-CTX-01 — Role-specific bootstrap + accepted-state navigation` (`REMOTE_OK`). CTX does not semantically block H1/CITY/PA, but human execution priority is to run CTX-01 before the next expensive local H1 execution so subsequent sessions can benefit from the context-routing savings once CTX-01 itself is accepted.

The plan alone grants no context-reduction permission. Until CTX-01 independently passes, merges and completes DocSync, existing Worker/Reviewer read requirements and predecessor reconstruction remain unchanged.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.8 requires predecessor-contract reconstruction, applicable content-shape probes, strict Worker pre-review, exact-SHA freeze/review and same-session post-PASS finalization/DocSync. It also permits bounded delegated execution under a single Worker when an exact environment is required, without transferring semantic/design/review authority. Accepted predecessor guarantees compose forward and are consumed rather than redundantly re-proved unless concrete evidence reopens them.

A fresh independent Reviewer remains mandatory for future workpacks. On PASS, finalization is exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with the dependency-valid next action/workpack. DocSync may not modify reviewed implementation bytes.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same causal owner/active WP through a fresh repair Worker. PASS binds the exact SHA. Accepted predecessor guarantees compose forward. Documentation-only finalization never redefines the reviewed implementation.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
