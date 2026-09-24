# H1 — Engine Bridge Foundation: Unity First

Status: ACTIVE / `WP-H1-00` + `WP-H1-01` + `WP-H1-02` + `WP-H1-03` + `WP-H1-03A` COMPLETE
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

`WP-H1-03` is **COMPLETE**. Frozen candidate `6d78ebc07e48419e279d4a17b93eeb099307316b` passed independent review `#5299906832` in PR `#168`; Candidate Validation `35956034910`, H1-03 Unity Host Policy `35955397660` and Arkus Main Safety `35955397574` were GREEN on that exact SHA, which merged as `b52fe8f67bb74880af8ed2d734c1293869fd9f86`. Binding DocSync: `Docs/evidence/WP-H1-03/DOCSYNC.md`.

`WP-H1-03A` is **COMPLETE**. Frozen candidate `24d24526487af0d32e69799960f4251e44f0b5ad` passed independent review `#5303016012` in PR `#176`; Candidate Validation `35985858123`, H1-03A Unity Lifecycle `35985225199` and Arkus Main Safety `35985225228` were GREEN on that exact SHA, which merged as `90428b803948820663abfebaa3fe21eb37596247`. Binding DocSync: `Docs/evidence/WP-H1-03A/DOCSYNC.md`.

`WP-H1-UNITY-CI` is **COMPLETE / ACCEPTED** as process infrastructure. Exact candidate `b49b081a92b088d7b0fd9adce4bd5f26a3b6c1bf` passed independent review `#5299167558`, merged as `6898250be985ab5d805bbdb129e30c9c6f1f4cdf`, and its accepted run `35936805407` proved the bounded GitHub-hosted Unity substrate. Binding DocSync: `Docs/evidence/WP-H1-UNITY-CI/DOCSYNC.md`.

Next default workpack: `WP-H1-04 — Unity catalogue + identity resolution` (`LOCAL_UNITY_REQUIRED / SOURCE_DEPENDENT`), dependency-valid but `NOT_STARTED` until a human starts its Worker.

H1 now has five accepted implementation WPs plus accepted remote-Unity process infrastructure. No later H1 implementation WP is active or implicitly authorized by this DocSync. `WP-H1-04` is dependency-valid from accepted H1-03A but remains `NOT_STARTED` until explicitly started.
The sequence contains 13 claim-owned implementation workpacks plus `WP-H1-GATE`.

## Effective Unity execution policy

H1 distinguishes **effective Unity evidence** from **physical-local execution**.

Accepted `WP-H1-UNITY-CI` proves that a workpack may use GitHub-hosted Unity when all evidence needed for its claim is machine-verifiable and the runner has every required lawful input. `LOCAL_UNITY_REQUIRED` therefore continues to mean that a real pinned Unity execution is required; it does **not** by itself mean the owner's physical PC is mandatory.

A physical/local Unity session remains required whenever the active claim materially depends on any of the following:

- human visual or interactive inspection;
- scene, material, animation, GPU or appearance judgment that is not replaced by an accepted machine oracle;
- peripherals or other physical-machine state;
- source assets or licensed/private bytes that currently exist only on the owner's machine and have not been lawfully and securely made available to the hosted runner;
- any other local-state fact that the hosted substrate cannot truthfully reproduce.

Conversely, a WP must not demand the owner's PC merely because older planning text says “local” if the actual acceptance claim is fully machine-verifiable on the accepted hosted substrate.

Under the current contracts, **H1-03 and H1-03A are explicitly GitHub-hosted eligible**: both need real pinned Unity execution, but neither requires Quaternius Source assets, scene appearance judgment or interactive authoring. From **H1-04 onward**, execution must be classified by the exact claim and input availability. In particular, if the accepted Quaternius Source slice needed by H1-04+ exists only on the owner's PC, those source-dependent proofs remain local until an independently reviewed lawful/secure hosted-input path exists.

The accepted H1-02/GameCI drift exception is bounded to its reviewed substrate and does not silently authorize broader package/source mutation in later WPs.

## CTX↔DW selective-adoption note

Accepted cross-track `WP-CTX-DW-GATE` does **not** block `WP-H1-03` or `WP-H1-03A`. Those authority/lifecycle workpacks continue on the existing H1 chain and are expected to use CTX plus authoritative H1/H0 sources, with DW normally `NOT_MATERIAL` unless concrete evidence says otherwise.

The plan does not assume that DW already contains H1 catalogue/Quaternius knowledge. `H1-04` remains source-first and establishes the accepted real-source plus catalogue/identity authority. Only after H1-04 PASS may a separate non-product CTX↔DW projection owner derive an H1 DW projection, with an independently enumerated source universe, completeness oracle, exact provenance, stale detection and deterministic rebuild. No H1 product workpack waits for this projection: when unavailable or stale, CTX routes directly to authority.

`H1-05` is the first eligible real H1 consumer of that projection when it is current and materially useful; `H1-06` is the second planned observation for the different asset/prefab relation shape. DW routing advice cannot shrink CTX mandatory reads or escalation requirements.

The required `H1-GATE` fresh independent public-client AI-agent trial remains on its accepted public launch-profile/MCP discovery/schema bootstrap and is not pre-seeded with Juego2-private CTX/DW knowledge. Any CTX↔DW fresh-agent composition probe is separate and cannot substitute for or repair H1-GATE public discoverability.

Detailed planning input: `Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md`.

## Outcome

H1 ends only when Arkus can drive a representative Juego2 slice through public contracts into a deterministic, inspectable, repairable and rebuildable Unity projection without making Unity canonical authority.

## Asset timing amendment

The first H1 workpack that needs game-representative art is `WP-H1-04`. At that point the exact human-approved **Quaternius Source** distribution/slice is adopted under `DEPENDENCY_IP_POLICY.md` and becomes the default real-art baseline for H1-04 through H1-GATE.

This does **not** mean H1 must finish the art before the bridge works. H1 uses Quaternius Source as-is wherever practical. Harness-only synthetic fixtures remain allowed for bridge mechanics/negative controls, but H1 does not fabricate substitute production art simply to postpone using the real source. `WP-H1-11` broadens the already-adopted source into the representative real-asset conformance slice; it is no longer the first adoption point.

If the required approved Quaternius bytes are only present on the owner's PC, a source-dependent H1 proof is legitimately local until those exact bytes have an accepted lawful/secure path to the hosted runner. The remote-Unity policy removes unnecessary machine dependence; it does not pretend private/local asset availability away.

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

`H1-00`, `H1-01`, `H1-02`, `H1-03` and `H1-03A` are accepted predecessor truth. `H1-04` is now the next default dependency-valid workpack. No H1-04 implementation is implicitly authorized by DocSync; it begins only when a human explicitly starts its Worker.

The CITY side edges do not add CITY work to H1. CITY-04 owns spatial greybox falsification and may consume H1-08; CITY-07 owns keeper realization after the Gate; CITY-08 later owns keeper-slice authoring efficiency/reuse. H1 consumes CITY-00 geography only as representative shape pressure and never selects the CITY-03 seed.

## Sequence summary

| Order | Workpack | Central claim | Execution |
|---:|---|---|---|
| 1 | `WP-H1-00` ✅ | engine-neutral projection state machine and reference materializer | `REMOTE_OK` |
| 2 | `WP-H1-01` ✅ | Unity scoped authoring producer and automatic dependency derivation | `REMOTE_OK` |
| 3 | `WP-H1-02` ✅ | pinned reproducible Unity project/toolchain/package baseline | `EFFECTIVE_UNITY` (accepted local oracle; hosted pilot now accepted) |
| 4 | `WP-H1-03` ✅ | explicit project-scoped Unity host authority below transports | `HYBRID / GITHUB_HOSTED_UNITY_ELIGIBLE` |
| 5 | `WP-H1-03A` ✅ | public host-to-Editor dispatch, main-thread and lifecycle contract | `HYBRID / GITHUB_HOSTED_UNITY_ELIGIBLE` |
| 6 | `WP-H1-04` | effective Unity catalogue/logical-native identity + first Quaternius Source adoption | `EFFECTIVE_UNITY / SOURCE_DEPENDENT` |
| 7 | `WP-H1-05` | deterministic managed scene graph and generational publication | `EFFECTIVE_UNITY / SOURCE_OR_VISUAL_DEPENDENT_AS_CLAIM_REQUIRES` |
| 8 | `WP-H1-06` | source-asset/prefab resolution plus managed prefab derivatives | `EFFECTIVE_UNITY / SOURCE_DEPENDENT` |
| 9 | `WP-H1-07` | allowlisted component schema, inspection and realization | `EFFECTIVE_UNITY / CLASSIFY_BY_CLAIM` |
| 10 | `WP-H1-08` | Unity-owned validation and stable diagnostics | `HYBRID / CLASSIFY_BY_CLAIM` |
| 11 | `WP-H1-09` | deterministic drift plus explicit Unity-to-canonical proposals | `EFFECTIVE_UNITY / CLASSIFY_BY_CLAIM` |
| 12 | `WP-H1-10` | project checkpoint and clean Unity reconstruction preserving H0 | `HYBRID / CLASSIFY_BY_CLAIM` |
| 13 | `WP-H1-11` | broad real-asset/rig/material/animation conformance over accepted Quaternius Source | `EFFECTIVE_UNITY / SOURCE_AND_POSSIBLY_VISUAL_DEPENDENT` |
| 14 | `WP-H1-GATE` | composed Unity bridge/parity readiness | `HYBRID / CLASSIFY_BY_CLAIM` |

`EFFECTIVE_UNITY` means a real pinned Unity execution is mandatory. Hosted versus physical-local is selected from the actual evidence/input needs above, not from the historical label alone.

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
- missing **effective Unity** evidence cannot be called PASS when the WP owns an effective-Unity claim;
- physical-local execution is required only when the claim materially depends on visual/interactive/physical-local state or on required source bytes unavailable to the hosted runner;
- accepted GitHub-hosted Unity may satisfy machine-verifiable effective-Unity evidence when all required inputs are lawfully and reproducibly available there;
- no full H0 AI trial or complete H0 replay/transport suite is repeated per WP;
- every Editor-bound public operation enters canonical composition and consumes the single H1-03A execution seam; private scripts/menus or adapter-only routes are not acceptance evidence;
- from H1-04 onward, positive game-representative art probes use the accepted Quaternius Source baseline where that source supplies the needed shape; synthetic repository fixtures remain valid for harness-only mechanics and causal negative controls;
- H1 bridge work does not own finished Cantabrian remodeling, wardrobe, missing-object production or missing-animation production;
- delta checks cover only the H0 seam touched by the current claim;
- the sole planned fresh external AI-agent trial is at `WP-H1-GATE`.

## Pre-mortem result

The complete sequence was challenged for dual claims, inherited-contract re-proof, self-shrinking universes, non-causal controls, predictable downstream reopenings, closure-owned product semantics, premature AI trials, temporary contracts and Unity model leakage. The split above is the result. Workpack-specific reopen conditions and residuals make remaining uncertainty explicit; no WP requires a contract that the plan already intends to break later.

The Quaternius timing amendment removes one such temporary-contract risk: the bridge no longer proves game-shaped asset/catalogue/prefab behavior on disposable substitute art and then first encounters the intended source only at H1-11. At the same time, H1 does not absorb art-production ownership merely because it consumes the source earlier.