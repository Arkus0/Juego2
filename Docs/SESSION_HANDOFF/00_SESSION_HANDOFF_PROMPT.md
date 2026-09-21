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

The H1 Engine Bridge / Unity-first architecture and full sequence are ACCEPTED and binding. Planning PR `#71` passed independent review `#5263596722` on frozen candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`, with candidate observation Actions `35567622151` and freeze validation Actions `35567723539` GREEN, then merged as `09ce3fb495d331285bef0ab8aebf4c6117c84d57`. The accepted plan contains 13 claim-owned implementation workpacks plus `WP-H1-GATE`. DocSync is complete and no H1 implementation WP is active.

The first default Worker is `WP-H1-00 — Engine-neutral projection contract + reference materializer` (`REMOTE_OK`). It is dependency-valid but remains `NOT_STARTED` until a human explicitly starts it. `WP-H1-02` is independently dependency-valid from H0 and may run in parallel only with explicit authorization; the default execution order remains numeric. Every transition still requires Worker -> independent Reviewer -> PASS -> merge -> DocSync.

H1 architecture keeps canonical `WorldState` and all H0 authoring authority engine-neutral. Unity binding intent is canonical opaque extension data produced by a schema-aware provider; the Unity catalogue, locators and materialized scene/assets remain bridge-owned derived state. Unity-to-canonical flow is an explicit proposal that must re-enter H0 plan/dry-run/apply. `WP-H1-03A` freezes the missing public execution seam: the external H0/.NET host owns canonical session/composition, while short-lived pinned Unity batch workers execute serialized main-thread Editor plans behind the same reference/MCP handlers. Gameplay and CITY-07 keeper realization remain blocked until `WP-H1-GATE` passes, merges and completes DocSync. CITY-04 is a narrower non-blocking greybox sidecar after CITY-03 + H1-08; it cannot invent bridge semantics or count as H1 evidence by implication.

Accepted CITY programme v2 keeps its own spine and owners. H1 uses `CITY_SPATIAL_CONSTITUTION.md` as representative shape input only. CITY-08's later keeper-slice reuse/cost trial remains distinct from H1-GATE's bounded public-client readiness trial.

The binding final scenario is `Docs/engineering/H1_UNITY_PARITY_GATE.md`. Ordinary H1 WPs use deterministic evidence and seam-specific H0 deltas. One fresh external AI-agent trial is reserved for `WP-H1-GATE`, where public Unity-bridge readiness is the new acceptance claim.

The non-blocking H0S scale/concurrency track may run in parallel with H1 if useful. It starts from measurements rather than a predetermined Merkle/per-resource-CAS/lock design and must not make a transport adapter the semantic concurrency authority.

## Operating model

Automation V2 provides mechanical candidate validation, exact-SHA state transitions and optional low-noise notifications. It does not replace independent Reviewer judgment.

`WORKER_REVIEW_PROTOCOL.md` v1.7 requires predecessor-contract reconstruction, applicable content-shape probes, strict Worker pre-review, exact-SHA freeze/review and same-session post-PASS finalization/DocSync. Accepted predecessor guarantees compose forward and are consumed rather than redundantly re-proved unless concrete evidence reopens them.

A fresh independent Reviewer remains mandatory for future workpacks. On PASS, finalization is exact-SHA merge preflight → merge → documentation-only DocSync → `DOCSYNC_COMPLETE` with the dependency-valid next action/workpack. DocSync may not modify reviewed implementation bytes.

## Process invariants

GitHub is truth. Draft Worker writes; Ready means frozen exact-SHA and independent review. FAIL returns to the same causal owner/active WP through a fresh repair Worker. PASS binds the exact SHA. Accepted predecessor guarantees compose forward. Documentation-only finalization never redefines the reviewed implementation.

## Update rule

Replace this compact state from accepted GitHub evidence after material transitions. Do not paste private reasoning or long historical logs here.
