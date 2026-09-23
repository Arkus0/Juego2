# H1 — Engine Bridge Foundation: Unity First

Status: ACTIVE / `WP-H1-00` + `WP-H1-01` + `WP-H1-02` COMPLETE
Initial reconstruction baseline: `c87c4c195d63cc9255745014f2b9757d9cd34050`
Reconciled integration base: `7fe44840076eba05f1b67a7633cd33fc67b9023d` (CITY programme v2 PASS, merge and DocSync)
Binding architecture: `Docs/engineering/H1_ENGINE_BRIDGE_ARCHITECTURE.md`
Binding decisions: `Docs/architecture/ADR-H1-001-*` through `ADR-H1-004-*`
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` v1.4+
Architect pre-review: `Docs/evidence/H1-PLAN/ARCHITECTURE_PRE_REVIEW.md`
Planning acceptance: PR `#71`; reviewed frozen candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`; independent PASS review `#5263596722`; merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`
Exact-SHA validation: candidate observation Actions `35567622151` GREEN; freeze validation Actions `35567723539` GREEN
Plan DocSync: `DOCSYNC_COMPLETE`

`WP-H1-00` is **COMPLETE**. Frozen candidate `3dc513dd963b77116fd45b5af8d800ae8993dd34` passed independent review `#5264661860` in PR `#75`, exact-SHA validation run `35579126516` was GREEN, and the accepted candidate merged as `c02cf54c89c13db43edda4a602b0c1620baa3fa2`.

`WP-H1-01` is **COMPLETE**. Frozen candidate `385ce2466190003d18c849c8d944b881d184e1c7` passed independent review `#5265525323` in PR `#79`, frozen exact-SHA validation run `35587471700` was GREEN, and the accepted candidate merged as `0b8f23fbce227bbbc710c8410dff575fcb9fcf12`.

`WP-H1-02` is **COMPLETE**. Frozen candidate `d86a08e644f542e9515f5e54fd4061f61e251c70` completed valid Unity Round 4 evidence and exact-SHA validation `35888922206` GREEN in PR `#152`. Final independent review `#5293810284` raised two further adversarial checker-evasion hardenings; owner acceptance override comment `#5798755634` classified those as non-material overdefense for H1-02 acceptance and authorized the exact candidate, which merged as `faa42a3d58ab26b0dc2547f9b6b7fc49a604219d`.

Next default workpack: `WP-H1-03 — Unity host policy + project workspace authority` (`HYBRID`), dependency-valid but `NOT_STARTED` until a human starts its Worker.

H1 now has three accepted implementation WPs. No later H1 implementation WP is active or implicitly authorized by this DocSync. `WP-H1-03A` remains blocked until `WP-H1-03` is accepted.
The sequence contains 13 claim-owned implementation workpacks plus `WP-H1-GATE`.

## CTX↔DW selective-adoption note

Planned cross-track `WP-CTX-DW-GATE` does **not** block `WP-H1-03` or `WP-H1-03A`. Those authority/lifecycle workpacks continue on the existing H1 chain and are expected to use CTX plus authoritative H1/H0 sources, with DW normally `NOT_MATERIAL` unless concrete evidence says otherwise.

If the CTX↔DW gate is accepted, first material H1 observations are planned at `H1-04` (catalogue/identity + first real-source adoption) and `H1-06` (asset/prefab relationships), without changing either product claim. `H1-GATE` should then use the accepted selective route for its planned fresh-agent trial where DW is material. Deterministic H1 evidence remains the oracle.

Detailed planning input: `Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md`.

## Outcome

H1 ends only when Arkus can drive a representative Juego2 slice through public contracts into a deterministic, inspectable, repairable and rebuildable Unity projection without making Unity canonical authority.

## Asset timing amendment

The first H1 workpack that needs game-representative art is `WP-H1-04`. At that point the exact human-approved **Quaternius Source** distribution/slice is adopted under `DEPENDENCY_IP_POLICY.md` and becomes the default real-art baseline for H1-04 through H1-GATE.

This does **not** mean H1 must finish the art before the bridge works. H1 uses Quaternius Source as-is wherever practical. Harness-only synthetic fixtures remain allowed for bridge mechanics/negative controls, but H1 does not fabricate substitute production art simply to postpone using the real source. `WP-H1-11` broadens the already-adopted source into the representative real-asset conformance slice; it is no longer the first adoption point.

The downstream product strategy is source-first: after H1-GATE, H2 builds the first playable/demo with maximum practical direct Quaternius reuse, then H2/ART/CITY production creates separately identified Juego2-derived assets only where concrete needs demand Cantabrian adaptation, clothing, missing objects, variants or missing animations. Those later art derivatives preserve provenance and do not become Arkus semantic authority.

## DAG

```text
WP-HK-GATE
   +--> H1-00 neutral bridge contract --> H1-01 Unity scoped producer ----+
   +--> H1-02 Unity project/toolchain -----------------------------------+
                                                                          v
 H1-03 host/workspace policy -> H1-03A public Editor execution/lifecycle
      -> H1-04 catalogue/identity + first Quaternius Source adoption
      -> H1-05 managed scenes -> H1-06 assets/prefabs -> H1-07 components
      -> H1-08 validation -> H1-09 reconciliation/import proposals
      -> H1-10 checkpoint/rebuild -> H1-11 broad real-source conformance
      -> H1-GATE

 H1-08 -. non-blocking prerequisite .-> CITY-04 (after CITY-03)
 H1-GATE -. keeper authorization .----> CITY-07 (after CITY-04)
```

`H1-00`, `H1-01` and `H1-02` are accepted predecessor truth. `H1-03` is now the next default dependency-valid workpack. No H1-03 implementation is implicitly authorized by DocSync; it begins only when a human explicitly starts its Worker.

The CITY side edges do not add CITY work to H1. CITY-04 owns spatial greybox falsification and may consume H1-08; CITY-07 owns keeper realization after the Gate; CITY-08 later owns keeper-slice authoring efficiency/reuse. H1 consumes CITY-00 geography only as representative shape pressure and never selects the CITY-03 seed.

## Sequence summary

| Order | Workpack | Central claim | Execution |
|---:|---|---|---|
| 1 | `WP-H1-00` ✅ | engine-neutral projection state machine and reference materializer | `REMOTE_OK` |
| 2 | `WP-H1-01` ✅ | Unity scoped authoring producer and automatic dependency derivation | `REMOTE_OK` |
| 3 | `WP-H1-02` ✅ | pinned reproducible Unity project/toolchain/package baseline | `LOCAL_UNITY_REQUIRED` |
| 4 | `WP-H1-03` | explicit project-scoped Unity host authority below transports | `HYBRID` |
| 5 | `WP-H1-03A` | public host-to-Editor dispatch, main-thread and lifecycle contract | `HYBRID` |
| 6 | `WP-H1-04` | effective Unity catalogue/logical-native identity + first Quaternius Source adoption | `LOCAL_UNITY_REQUIRED` |
| 7 | `WP-H1-05` | deterministic managed scene graph and generational publication | `LOCAL_UNITY_REQUIRED` |
| 8 | `WP-H1-06` | source-asset/prefab resolution plus managed prefab derivatives | `LOCAL_UNITY_REQUIRED` |
| 9 | `WP-H1-07` | allowlisted component schema, inspection and realization | `LOCAL_UNITY_REQUIRED` |
| 10 | `WP-H1-08` | Unity-owned validation and stable diagnostics | `HYBRID` |
| 11 | `WP-H1-09` | deterministic drift plus explicit Unity-to-canonical proposals | `LOCAL_UNITY_REQUIRED` |
| 12 | `WP-H1-10` | project checkpoint and clean Unity reconstruction preserving H0 | `HYBRID` |
| 13 | `WP-H1-11` | broad real-asset/rig/material/animation conformance over accepted Quaternius Source | `LOCAL_UNITY_REQUIRED` |
| 14 | `WP-H1-GATE` | composed Unity bridge/parity readiness | `HYBRID` |

## Split review

| Boundary | Why it stays separate |
|---|---|
| H1-00 vs H1-01 | neutral projection truth can be correct while the Unity public producer/dependency semantics are false |
| H1-01 vs H1-03 | contract composition and dependency derivation do not prove editor/project host authority |
| H1-02 vs H1-03 | a reproducible project can exist while policy admission is bypassable, and vice versa |
| H1-03 vs H1-03A | admitting fixed project/editor authority does not prove public process dispatch, main-thread execution or truthful interruption lifecycle |
| H1-03A vs H1-04 | a correct public Editor execution seam can pass while effective catalogue identity/completeness or first real-source adoption is false |
| H1-04 vs H1-05 | read-only inventory/identity can pass while writes/materialization are unsafe |
| H1-05 vs H1-06 | deterministic GameObject hierarchy does not prove prefab/asset relationship fidelity |
| H1-06 vs H1-07 | prefab linkage and arbitrary component-field semantics have different universes/oracles |
| H1-07 vs H1-08 | realization success does not prove complete/actionable diagnostics |
| H1-08 vs H1-09 | detecting invalid/drifted state does not authorize a synchronization direction |
| H1-09 vs H1-10 | correct reconciliation in one session does not prove restart/checkpoint reconstruction |
| H1-10 vs H1-11 | generic rebuildability over the accepted source baseline does not prove the broader representative real hierarchy/material/rig/animation composition |
| H1-11 vs GATE | real-source conformance is an input; gate is closure/composition plus fresh public-client readiness |

No boundary is a single function or administrative checkpoint. Combining any adjacent pair above would give one Reviewer two independently rejectable claims; splitting further would create provisional contracts or duplicate the same authority proof.

## Track-wide inherited guarantees

H1 consumes H0 canonical identity/hash, complete inspection, plan/dry-run/atomic apply, structured validation, journal/provenance, snapshot/rebase, replay, neutral projection, JSONL/MCP parity, interaction/recovery semantics, host/resource boundaries and quality closure. A H1 WP reopens one only with effective contradictory evidence on its new seam.

## Track-wide proof rules

- deterministic proof is the default;
- each public/authorable semantic change gets a bounded approved Juego2 content-shape probe;
- Unity-required evidence records exact editor/package/platform fingerprints;
- missing local Unity evidence cannot be called PASS;
- no full H0 AI trial or complete H0 replay/transport suite is repeated per WP;
- every Editor-bound public operation enters canonical composition and consumes the single H1-03A execution seam; private scripts/menus or adapter-only routes are not acceptance evidence;
- from H1-04 onward, positive game-representative art probes use the accepted Quaternius Source baseline where that source supplies the needed shape; synthetic repository fixtures remain valid for harness-only mechanics and causal negative controls;
- H1 bridge work does not own finished Cantabrian remodeling, wardrobe, missing-object production or missing-animation production;
- delta checks cover only the H0 seam touched by the current claim;
- the sole planned fresh external AI-agent trial is at `WP-H1-GATE`.

## Pre-mortem result

The complete sequence was challenged for dual claims, inherited-contract re-proof, self-shrinking universes, non-causal controls, predictable downstream reopenings, closure-owned product semantics, premature AI trials, temporary contracts and Unity model leakage. The split above is the result. Workpack-specific reopen conditions and residuals make remaining uncertainty explicit; no WP requires a contract that the plan already intends to break later.

The Quaternius timing amendment removes one such temporary-contract risk: the bridge no longer proves game-shaped asset/catalogue/prefab behavior on disposable substitute art and then first encounters the intended source only at H1-11. At the same time, H1 does not absorb art-production ownership merely because it consumes the source earlier.
