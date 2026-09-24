# ROADMAP — Juego2 / Arkus Harness

Version: 1.35 — 2026-09-24

## North star

Build an engine-agnostic, commercially viable AI-native game-authoring platform whose canonical contracts, transactional semantics and verification guarantees exceed any single engine-specific AI harness; then prove it by building Juego2 on top of it.

A fresh AI agent, without C# implementation knowledge, must be able to discover available capabilities and safely create, inspect, modify, validate, diff, replay and test a representative world through stable machine-readable contracts.

**H0 / `WP-HK-GATE` has passed. The Engine Bridge / Unity-first H1 plan is accepted and binding; `WP-H1-00`, `WP-H1-01` and `WP-H1-02` are complete, and `WP-H1-03` is the next default dependency-valid H1 workpack. `WP-H1-UNITY-CI` is also COMPLETE / ACCEPTED as process infrastructure: machine-verifiable effective Unity evidence may use the reviewed GitHub-hosted substrate, while physical-local execution remains required when a claim depends on visual/interactive/local-machine state or on required source bytes unavailable to the runner. Gameplay and keeper realization remain blocked until `WP-H1-GATE` passes, merges and completes DocSync; the bounded CITY-04 greybox has the narrower H1-08 prerequisite recorded below. DW-GATE and CTX↔DW selective-adoption are accepted; they do not block H1-03/H1-03A and instead feed later selective H1/H2 use.**

Juego2 / Arkus Harness is a **game-development and software-verification project, not a cybersecurity project**. Robustness work in H0 is repository-local testing of the harness's own code, fixtures and contracts. New work uses the neutral negative-conformance terminology defined in `AGENTS.md`.

## Product architecture direction

```text
AI / future Creator GUI / scripts / SDK clients
                    ↓
           transport adapters
       MCP / JSONL / future HTTP
                    ↓
         canonical Arkus Contract
      base + scoped extensions
                    ↓
      Authoring + Validation kernel
                    ↓
          Core / World state
                    ↓
       Engine Bridge abstractions
                    ↓
       Unity first / others later
```

The canonical contract and kernel are engine-agnostic, transport-agnostic and model-vendor-agnostic. MCP is a first-class standards adapter, not the source of truth. Unity is the first engine bridge, not the platform boundary.

The old `Arkus0/Juego` repository is a reference archive, not a migration source of authority.

---

# H0 — Harness Kernel

All H0 workpacks are foundational and require independent review before acceptance.

Accepted progress: `WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B`, `WP-HK-08A`, `WP-HK-08B`, `WP-HK-09A`, `WP-HK-09B`, `WP-HK-10` and `WP-HK-GATE` are COMPLETE. H0 is complete.

Verbose accepted H0 PR/review/action closure chronology is preserved under `Docs/history/ROADMAP_ACCEPTED_CLOSURES.md` and exact workpack/evidence/PR sources. It is no longer part of the current ROADMAP bootstrap because it does not define live ordering or gates.

Before implementation, the original HK06 and HK07 workpacks were deliberately split to reduce coupled foundational freeze/review risk while preserving their aggregate objectives. The executable dependency chain is `HK06A → HK06B → HK06C → HK07A → HK07B`. The old `WP-HK-06.md` and `WP-HK-07.md` remain as SUPERSEDED umbrella records and must not be implemented directly.

Before implementation, HK08 and HK09 were likewise split where each umbrella mixed two independently reviewable claims. The executable downstream chain is now `HK07B → HK08A → HK08B → HK09A → HK09B → HK10 → HK-GATE`. The old `WP-HK-08.md` and `WP-HK-09.md` remain as SUPERSEDED umbrella records and must not be implemented directly.

No H0 workpack remains. The accepted H0 boundary is the predecessor for the accepted H1 plan below. `WP-H1-00`, `WP-H1-01` and `WP-H1-02` are complete; the next default dependency-valid H1 Worker is `WP-H1-03`, which remains `NOT_STARTED` until a human explicitly starts it.

| Order | Workpack | Outcome |
|---:|---|---|
| 1 | `WP-HK-00` ✅ COMPLETE | Canonical portable module boundary, pinned toolchain and headless CI |
| 2 | `WP-HK-00A` ✅ COMPLETE | Product architecture, adoption/IP boundary and anti-lock-in contract |
| 3 | `WP-HK-01` ✅ COMPLETE | Canonical contract model + machine-readable capability/schema discovery |
| 4 | `WP-HK-02` ✅ COMPLETE | Canonical world state, stable identity, deterministic serialization + hash |
| 5 | `WP-HK-03` ✅ COMPLETE | Complete read/inspection/query surface |
| 6 | `WP-HK-04` ✅ COMPLETE | Plan/dry-run/atomic apply, conflict detection and transactional mutation |
| 7 | `WP-HK-02A` ✅ COMPLETE | Object-scoped opaque extension data + typed declared dependencies |
| 8 | `WP-HK-05` ✅ COMPLETE | Validation/invariants and structured repairable diagnostics |
| 9 | `WP-HK-06A` ✅ COMPLETE | Provenance journal + authored/live-state boundary |
| 10 | `WP-HK-06B` ✅ COMPLETE | Semantic diff + canonical snapshot export/import portability |
| 11 | `WP-HK-06C` ✅ COMPLETE | Deterministic journal replay + end-to-end audit consistency |
| 12 | `WP-HK-07A` ✅ COMPLETE | Production headless host + deterministic JSONL/reference transport |
| 13 | `WP-HK-07B` ✅ COMPLETE | Standards-compatible MCP projection + cross-transport conformance |
| 14 | `WP-HK-08A` ✅ COMPLETE | Efficient interaction primitives: atomic batching, compact/bounded reads, pagination and discovery cost metadata |
| 15 | `WP-HK-08B` ✅ COMPLETE | Structured stale-CAS recovery, repair ergonomics and measured agent interaction budgets |
| 16 | `WP-HK-09A` ✅ COMPLETE | Capability containment: filesystem/network/process authority and below-transport policy |
| 17 | `WP-HK-09B` ✅ COMPLETE | Resource/input limits plus import/persistence interruption integrity |
| 18 | `WP-HK-10` ✅ COMPLETE | Strict property/malformed-input/fault-injection quality closure + bounded endurance |
| 19 | `WP-HK-GATE` ✅ COMPLETE | End-to-end AI-authoring readiness benchmark on a representative micro-world |

## H0 interaction/concurrency decision

H0 keeps the accepted whole-world revision/hash CAS and does not speculate into per-resource locking, automatic merge, distributed transactions or autonomous multi-agent scheduling.

Whole-world CAS/hash is an **internal consistency boundary**, not a requirement that every client download or reconstruct the whole world after every commit. The kernel may validate/serialize/hash the accepted authored state as a whole while clients operate on bounded revision-anchored queries, semantic diffs and HK08B recovery context. AI coherence comes from stable anchors, complete relevant slices and fail-closed stale detection; repeatedly sending the entire world to an agent is neither required nor assumed to improve coherence.

HK08B makes the existing optimistic-concurrency model **cheap to recover from** in the ordinary same-lineage case: a stale plan receives bounded machine-readable context anchored to the expected and current authored world, exact changed-resource identity derived from proven local lineage history, and current-resource inspection descriptors so the client can preserve intent and retry through the normal transaction path without reconstructing the complete world. When lineage/history is insufficient, the accepted contract says so explicitly and returns bounded reinspection rather than invented precision.

Batching is the primary H0 mitigation for whole-world commit cost. HK08A accepted a representative 96-operation mixed-resource edit as one atomic validated/provenanced transaction, and HK09B fixed that representative shape inside the accepted H0 resource envelope rather than forcing it to split. The 96-operation ceiling is an H0 reviewed bound, not a permanent shipping-scale promise; later approved product evidence may justify a reviewed/version-compatible envelope change, but clients must not silently split a coherent intent merely to satisfy a legacy cap.

Serializing commit execution can protect the mutation authority from simultaneous publication, but it does **not** make an already planned stale request current. HK08B supplies deterministic rejection/recovery/re-plan semantics for that case; a further writer may still make the retry stale again, which is handled by the same optimistic-CAS loop rather than hidden merge.

Per-resource concurrency, automatic merge of disjoint writers, multi-process writer coordination and multi-agent scheduling remain post-GATE product work. HK08B's accepted benchmark did not promote them into H0; HK10 stress-tested the accepted whole-world stale/conflict recovery semantics without adding hidden merge, and HK-GATE found no measured reason to make finer concurrency a precondition for H1. Later H1/H0S evidence may justify revisiting CAS granularity or coordination semantics.

HK08A passed reference-transport ↔ MCP conformance for batching/compact/pagination semantics, HK08B separately passed it for recovery semantics, and HK09B passes it for successfully framed neutral resource-limit/error semantics. HK-GATE composed those accepted surfaces in the final readiness proof and added an independent external MCP client trial without changing canonical authority. Physical malformed/oversized JSONL framing remains governed by the frozen HK07A transport contract and is intentionally not redefined as a neutral resource error.

HK09A constrains the production H0 host-power envelope beneath those transports and HK09B adds the finite machine-readable resource/publication envelope beneath the same neutral boundary. HK10 completed strict closure over those accepted guarantees; HK-GATE then proved the composed public path and H0→Engine Bridge boundary without inventing a replacement architecture.

### H0 exit criteria — ACCEPTED

`WP-HK-GATE` was the final H0 proof and has passed on exact frozen SHA `0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd`. H0 has demonstrated correctness/replay/transport neutrality, bounded same-lineage stale-plan recovery, an atomic representative batch inside the accepted HK09B resource budget, explicit capability containment from HK09A, bounded long-session resource behaviour accepted by HK10, full headless validation and a fresh AI-agent trial using public discovered contracts. Multi-agent throughput and automatic concurrent merge remain outside H0 because the accepted gate produced no measured evidence requiring them for the representative authoring contract.

---

# H0S — Post-GATE scale + concurrent authoring track (parallel, non-blocking by default)

`WP-HK-GATE` validates the correctness and usability foundation; it is not a claim that whole-world work or serialized writer semantics are the final commercial scaling architecture.

After GATE, a dedicated scale/concurrency track may run in parallel with H1 Unity integration. It is **not a prerequisite for starting H1** unless measured H1/H0S evidence shows the representative product cannot operate acceptably without it.

This track begins from evidence, not a predetermined implementation. Trigger measurements include authored object count, whole-world validation/hash cost, p50/p95 commit latency, stale-plan rate, bounded-recovery cost, simultaneous-writer collision rate, memory growth and the amount of state a client must read to preserve intent.

Candidate techniques may include cached/incremental indexes, incremental validation/hash computation, resource-scoped preconditions, server-side coordination/leases, revision change feeds or finer-grained state partitioning. A Merkle tree, per-resource CAS or scope lock is an option only if evidence justifies it, not the definition of the solution.

Any concurrency/coordination semantic that affects whether or when a valid canonical request may execute must live in a transport-neutral reviewed service/contract boundary. It may not exist only inside MCP, JSONL or another adapter, and it may not silently weaken canonical validation, provenance, replay or deterministic state identity.

The commercial target is therefore two-layered: H0 provides a simple globally coherent correctness model; post-GATE scaling may optimize its implementation and writer coordination while preserving or explicitly versioning those semantics.

---

# H1 — Engine Bridge Foundation: Unity First

Status: **ACTIVE / WP-H1-00 + WP-H1-01 + WP-H1-02 COMPLETE**. Initial reconstruction baseline: `c87c4c195d63cc9255745014f2b9757d9cd34050`; reconciled integration base after CITY programme v2 PASS, merge and DocSync: `7fe44840076eba05f1b67a7633cd33fc67b9023d`. H1 planning PR `#71` passed independent review `#5263596722` on frozen candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b` and merged as `09ce3fb495d331285bef0ab8aebf4c6117c84d57`; DocSync is complete. `WP-H1-UNITY-CI` is COMPLETE / ACCEPTED as bounded process infrastructure after PASS review `#5299167558` and merge `6898250be985ab5d805bbdb129e30c9c6f1f4cdf`. The next default dependency-valid H1 workpack is `WP-H1-03`.

### H1 effective-Unity execution policy

`LOCAL_UNITY_REQUIRED` is a historical/conservative label for **real pinned Unity evidence**, not an unconditional requirement to use the owner's physical PC. Accepted `WP-H1-UNITY-CI` allows GitHub-hosted Unity when the complete acceptance claim is machine-verifiable and every required lawful input is reproducibly available to the runner.

Physical/local Unity remains required when the claim materially depends on human visual or interactive inspection, scene/material/animation/GPU appearance, peripherals or local-machine state, or on required source assets/licensed bytes that currently exist only on the owner's machine. In particular, source-dependent H1-04+ work may remain local while the accepted Quaternius Source bytes are only present on the owner's PC. The hosted policy removes unnecessary machine dependence; it does not invent remote access to local/private assets.

Under the current contracts, `WP-H1-03` and `WP-H1-03A` are explicitly GitHub-hosted eligible because their oracles are structural/process/lifecycle claims and do not require Quaternius assets or visual judgment. Binding detail lives in `Docs/workpacks/H1/README.md`, the individual WP contracts and `Docs/evidence/WP-H1-UNITY-CI/DOCSYNC.md`.

The binding planning set is:

- architecture and authority: `Docs/engineering/H1_ENGINE_BRIDGE_ARCHITECTURE.md` plus `Docs/architecture/ADR-H1-*`;
- causal H0 lessons: `Docs/engineering/H0_TO_H1_RETROSPECTIVE.md`;
- DAG and complete workpack contracts: `Docs/workpacks/H1/README.md` and `Docs/workpacks/H1/WP-H1-*.md`;
- final reference scenario: `Docs/engineering/H1_UNITY_PARITY_GATE.md`;
- risk/residual handoff: `Docs/engineering/H1_RISK_AND_RESIDUAL_PLAN.md`;
- architect pre-review: `Docs/evidence/H1-PLAN/ARCHITECTURE_PRE_REVIEW.md`.

H1 keeps `WorldState`, H0 mutation/validation/provenance and public composition canonical. Unity binding intent is authored through a versioned schema-aware scoped producer; the Unity catalogue, native locators and materialized objects remain bridge/project-owned. Canonical-to-Unity is the normal projection direction. A supported Unity edit can produce only an explicit stale-aware canonical mutation proposal, which gains authority/provenance solely through the accepted H0 `plan -> dry-run -> apply` path.

The producer mechanically derives every canonical-object dependency and provider-owned catalogue dependency that it understands. The AI does not normally synchronize structured payload references with duplicate metadata manually. Raw opaque payload authoring remains a low-level primitive; the engine-neutral kernel does not inspect Unity payload meaning.

The public execution topology is also frozen before feature work: the external Arkus .NET host owns the process-local canonical session, composition and reference/MCP projection; admitted Editor-bound handlers launch short-lived pinned Unity batch workers through `WP-H1-03A`, which owns bootstrap binding, project lease, main-thread dispatch, invocation status, cancellation/interruption/restart and structured process failures. Unity never hosts a parallel public registry or canonical session.

## H1 dependency graph

```text
WP-HK-GATE
   +--> H1-00 neutral bridge contract --> H1-01 scoped producer -------+
   +--> H1-02 Unity toolchain/project ---------------------------------+
                                                                        v
 H1-03 host policy -> H1-03A public Editor execution/lifecycle
      -> H1-04 catalogue/identity + first Quaternius Source adoption
      -> H1-05 managed scenes -> H1-06 assets/prefabs -> H1-07 components
      -> H1-08 validation -> H1-09 reconciliation -> H1-10 checkpoint/rebuild
      -> H1-11 broad real-source conformance -> H1-GATE

 H1-08 -. non-blocking prerequisite .-> CITY-04 (after CITY-03)
 H1-GATE -. keeper authorization .----> CITY-07 (after CITY-04)
```

`H1-00`, `H1-01` and `H1-02` are accepted predecessor truth. The default human sequence is numeric. Every later edge requires predecessor PASS, merge and DocSync.

| Order | Workpack | Owned outcome | Execution |
|---:|---|---|---|
| 1 | `WP-H1-00` ✅ | neutral projection contract + deterministic reference materializer | `REMOTE_OK` |
| 2 | `WP-H1-01` ✅ | Unity scoped producer + automatic dependency derivation | `REMOTE_OK` |
| 3 | `WP-H1-02` ✅ | exact reproducible Unity project/toolchain/package baseline | `EFFECTIVE_UNITY` |
| 4 | `WP-H1-03` | project-scoped Unity host authority below transports | `HYBRID / GITHUB_HOSTED_UNITY_ELIGIBLE` |
| 5 | `WP-H1-03A` | public host-to-Editor dispatch, main-thread and lifecycle contract | `HYBRID / GITHUB_HOSTED_UNITY_ELIGIBLE` |
| 6 | `WP-H1-04` | effective catalogue + logical/native identity mapping + first Quaternius Source adoption | `EFFECTIVE_UNITY / SOURCE_DEPENDENT` |
| 7 | `WP-H1-05` | managed scene graph + generational publication | `EFFECTIVE_UNITY / SOURCE_OR_VISUAL_DEPENDENT_AS_CLAIM_REQUIRES` |
| 8 | `WP-H1-06` | source asset/prefab fidelity + managed derivatives | `EFFECTIVE_UNITY / SOURCE_DEPENDENT` |
| 9 | `WP-H1-07` | finite allowlisted component schema/adapters | `EFFECTIVE_UNITY / CLASSIFY_BY_CLAIM` |
| 10 | `WP-H1-08` | Unity-owned validation + actionable diagnostics | `HYBRID / CLASSIFY_BY_CLAIM` |
| 11 | `WP-H1-09` | drift/reconciliation + explicit import proposals | `EFFECTIVE_UNITY / CLASSIFY_BY_CLAIM` |
| 12 | `WP-H1-10` | project checkpoint + clean Unity reconstruction | `HYBRID / CLASSIFY_BY_CLAIM` |
| 13 | `WP-H1-11` | broad real hierarchy/material/rig/animation conformance over accepted Quaternius Source | `EFFECTIVE_UNITY / SOURCE_AND_POSSIBLY_VISUAL_DEPENDENT` |
| 14 | `WP-H1-GATE` | composed Unity parity/readiness + one fresh public AI trial | `HYBRID / CLASSIFY_BY_CLAIM` |

The split is claim-driven, not quota-driven. Adjacent workpacks remain separate where authority or proof can fail independently; component types and individual assets remain together where further division would create administrative micro-WPs. H0 guarantees are inherited and only delta-checked at changed seams. The sole planned fresh external AI-agent trial is the final Gate.

From `WP-H1-04` onward, positive game-representative art probes use the exact accepted Quaternius Source baseline wherever that source supplies the needed shape. H1 may still use repository-owned synthetic fixtures for bridge internals and causal negative controls. `WP-H1-11` broadens the source-shaped conformance challenge; it does not postpone first real-art use until the end and it does not own finished Cantabrian art production.

H1 consumes accepted CITY-00 geography only as representative shape pressure; it does not own or redraw it. CITY remote planning remains independent. `CITY-04` may use the accepted scene/prefab/component/diagnostic surface after `H1-08` to falsify the CITY-03 greybox, without becoming an H1 acceptance stage. `CITY-07` keeper realization waits for H1-GATE. CITY-08 later owns keeper-slice authoring-efficiency/reuse proof and is not duplicated by the smaller H1 Gate readiness trial.

H1 ends only when `WP-H1-GATE` answers the published reference question affirmatively without adding semantics: Arkus can create, inspect, modify, validate, materialize and reconstruct the bounded Juego2 Unity slice from public contracts while canonical identity, transactions/provenance, dependencies, diagnostics and normalized parity remain truthful.

---

# DW — Design World second-consumer validation (parallel with H1; accepted H2 boundary interlock)

Status: **PLAN ACCEPTED / IMPLEMENTATION TRACK COMPLETED THROUGH DW-GATE**. The accepted DW implementation/gate sequence culminated in `WP-DW-GATE` PASS; DW evidence is now a bounded accepted H2 planning interlock, not a blocker for H1.

DW pressure-tests accepted Arkus/H0 with two materially different non-runtime consumers while preserving source authority and generic kernel semantics. It consumes accepted CITY design facts and accepted PA research/evidence as authorities, then measures whether structured retrieval can reduce context without degrading actual agent task/review correctness. CTX remains the accepted process-context baseline rather than being invalidated by DW.

## DW dependency graph

```text
WP-HK-GATE
    |
    v
 DW-00 -> DW-01 -> DW-02 -> DW-03 -> DW-04 -> DW-05 -> DW-GATE ✅
                            ^          ^
                            |          |
                    PA-01..05 accepted CTX-03 accepted

H1 proceeds independently in parallel.
DW-GATE ---- accepted planning interlock ----> final H2 public/external-boundary acceptance
```

| Order | Workpack | Owned outcome | Execution |
|---:|---|---|---|
| 1 | `WP-DW-00` | generic authority-preserving/rebuildable Design World projection contract | `REMOTE_OK` |
| 2 | `WP-DW-01` | first CITY semantic/invariant consumer with causal omission detection | `REMOTE_OK` |
| 3 | `WP-DW-02` | useful CITY deterministic queries/content-shape projection | `REMOTE_OK` |
| 4 | `WP-DW-03` | lossless PA findings/evidence/dispositions/fixtures projection | `REMOTE_OK` |
| 5 | `WP-DW-04` | frozen paired CTX-vs-DW agent trial: deterministic scoring of actual task/review correctness plus context cost | `REMOTE_OK` |
| 6 | `WP-DW-05` | generic-boundary stress, limitation routing and H2 impact classification | `REMOTE_OK` |
| 7 | `WP-DW-GATE` ✅ | composed second-consumer readiness and explicit H2 planning consequence | `REMOTE_OK` |

Cross-track prerequisites do not transfer ownership: PA remains authority for PA research, CTX remains authority for process/context policy, CITY remains authority for CITY design, and H1 keeps Unity bridge ownership. DW does not block H1 implementation and does not itself authorize H2 gameplay/first-playable work.

Because `DW-GATE` is accepted, final H2 public/external-boundary acceptance must consume/disposition that accepted evidence before the boundary is frozen. Accepted `WP-CTX-DW-GATE` additionally establishes selective routing: CTX remains the control/routing plane, DW is used only when materially useful/current, and authoritative sources remain semantic authority.

The binding DW architecture/workpack contracts are `Docs/engineering/DW_DESIGN_WORLD_ARCHITECTURE.md` and `Docs/workpacks/DW/**`. The track does not claim arbitrary-domain universality, external-repository packaging readiness or permission to replace accepted Markdown/source authorities wholesale.

---

# H2 — Vertical Slice Foundation (blocked by Unity parity gate; final boundary interlocked with accepted DW)

Only after `WP-H1-GATE` PASS, merge and DocSync may H2 gameplay/first-playable production proceed under this roadmap. The final H2 public/external-boundary acceptance consumes the already accepted `WP-DW-GATE` evidence/interlock; this condition does not predefine H2 implementation scope or make DW a substitute for H1-GATE.

- build the first playable/demo with **maximum practical direct reuse of the Quaternius Source baseline already adopted at H1-04** rather than waiting for final custom art;
- use `Docs/art/VISUAL_BIBLE.md` as the visual/adaptation contract and progressively create separately identified Juego2-derived assets only where concrete needs require Cantabrian materials/architecture, clothing/outfits, missing props/meshes, variants or missing/retargeted animations;
- expand beyond the exact H1-11 representative bridge-conformance slice under the documented licensing/content pipeline (`DEPENDENCY_IP_POLICY.md`), preserving upstream/derivative provenance;
- setting anchor: **fictional Potes / Liébana** valley market town (`Docs/art/SETTING.md`); optional later river + small inland landing;
- camera, movement, interaction shell and one representative street/plaza;
- player-facing characters must be appropriately clothed for their role; a technically valid raw humanoid source does not make a naked/unpresentable NPC demo-ready;
- no system is accepted unless it is inspectable/modifiable/testable through the harness.

The intended visual progression is therefore **Quaternius Source playable first → demand-driven Cantabrian/Juego2 derivatives → bounded polished demo**, not “finish all art before the first playable” and not “keep source assets immutable forever.”

---

# H3+ — Game Systems

Gameplay systems follow only after H0/H1 prove the AI can safely evolve the project.

---

# ART — Visual direction track (parallel, non-gating)

Art **direction** only today. Does not block H0. Not bound by `FOUNDATIONAL_PROOF_STANDARD.md`.

| Order | Workpack | Outcome |
|---:|---|---|
| 1 | `WP-ART-00` ✅ SEED MERGED | Visual bible + Quaternius→Liébana rules; setting lock; refs; animation; triage dry-run |
| 2+ | not yet authored | Future production-art WPs only when H2/CITY needs formal ownership for derived assets/content pipeline |

Deliverables: `Docs/art/VISUAL_BIBLE.md`, `Docs/art/SETTING.md`, `Docs/art/Refs/**`, `Docs/workpacks/ART/WP-ART-00.md`, `Docs/evidence/WP-ART-00/TRIAGE_DRY_RUN.md`.

Art *content* is no longer globally blocked by the H1 parity gate: `WP-H1-04` may adopt/import the exact Quaternius Source baseline because H1 catalogue/prefab proof needs real game-shaped inputs. What remains blocked until H1-GATE is **H2 gameplay/first-playable production and keeper realization**, not the mere presence of accepted source assets in Unity. Broad Cantabrian adaptation/derived-asset production is demand-driven downstream product work, not an H1 prerequisite.

---

## Global stop rules

1. Two Reviewer FAILs that expose the same foundational class trigger architecture re-audit.
2. Proof-universe self-shrink triggers immediate proof-boundary re-audit.
3. Prefer independent oracles over endless syntax-path enumeration.
4. No downstream WP patches around a false predecessor claim.
5. No feature pressure may bypass accepted harness guarantees or downstream Engine Bridge / Unity parity gates.
6. No external framework adopted merely to save time if it reduces Arkus scope, neutrality or viability.
