# ROADMAP — Juego2 / Arkus Harness

Version: 1.28 — 2026-09-20

## North star

Build an engine-agnostic, commercially viable AI-native game-authoring platform whose canonical contracts, transactional semantics and verification guarantees exceed any single engine-specific AI harness; then prove it by building Juego2 on top of it.

A fresh AI agent, without C# implementation knowledge, must be able to discover available capabilities and safely create, inspect, modify, validate, diff, replay and test a representative world through stable machine-readable contracts.

**H0 / `WP-HK-GATE` has passed. The complete Engine Bridge / Unity-first H1 plan is defined, but no H1 implementation WP is active merely because the planning PR exists. Gameplay and keeper realization remain blocked until `WP-H1-GATE` passes, merges and completes DocSync; the bounded CITY-04 greybox has the narrower H1-08 prerequisite recorded below.**

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

`WP-HK-05` PR `#26` passed independent review on frozen candidate `23a9fd4373a803187cd9391b1459cd48975177f6` (review `#5257350871`), exact-SHA candidate observation Actions `35464742544` GREEN, freeze validation Actions `35464834162` GREEN, and merged as `ed65661680aea2a9be79f892c96aa42bf788a842` on 2026-09-19. Two prior frozen candidates failed in the same aggregate-validation-under-ambiguous-identity class; the circuit breaker triggered a causal architecture re-audit, and the accepted candidate uses dependency-local ambiguity deferral rather than global suppression.

`WP-HK-06A` PR `#32` passed independent review on frozen candidate `4ed9a925791ae14b0b5c0d92625161504021542e` (review `#5257682288`), exact-SHA candidate observation Actions `35469103222` GREEN, freeze validation Actions `35469154331` GREEN, and merged as `8089a52e8a7bbdde46e97705df6906c0d38d593a` on 2026-09-19. One earlier frozen candidate failed because a schema-valid but semantically false normalized replay envelope could remain green; the accepted repair binds the complete machine-readable request to the parsed request fingerprint/base anchor and independently verifies deterministic entry identity before atomic state/receipt/journal publication.

`WP-HK-06B` PR `#35` passed independent review on frozen candidate `2b05e982c96e7ece08cca999075c183e75eee2fd` (review `#5258130916`), exact-SHA validation Actions `35473614054` GREEN, and merged as `28e2d0aadf63fe322eac636955e9223dc9249328` on 2026-09-19. One earlier frozen candidate failed because snapshot import publicly claimed `CanonicalMutation + CanonicalTransaction` while actually replacing the whole authored session and intentionally starting a fresh empty HK06A lineage. The accepted repair models import explicitly as `CanonicalRebase`, with separate truthful rebase authority/evidence, keyed idempotency, and no fabricated HK06A mutation history.

`WP-HK-06C` PR `#40` passed independent review on frozen candidate `55fecbd8a4a5e17ce247b164cd375d652d066fdf` (review `#5259530509`), exact-SHA validation Actions `35488920548` GREEN, and merged as `e44a5e93bf0912f5b5fb80dd749e294e21a740f2` on 2026-09-20. Replay is explicitly a distinct `CanonicalReplay` authority: accepted HK06A journal evidence is verified and replayed through the accepted HK04/HK05 mutation authority inside a staged session, audited against regenerated HK06A entry identities/results, and published only after complete success; final canonical hash and HK06B semantic diff must agree.

`WP-HK-07A` PR `#44` passed independent review on frozen candidate `f2ef88980b38482a1a635f4eeb0582ee49d735ec` (review `#5259732343`), exact-SHA validation Actions `35492562005` GREEN, and merged as `f70a81b5115cd2a6b0c8b1cb9c5dd657d5f3792b` on 2026-09-20. The accepted host freezes `arkus.neutral-projection@1` above a deterministic `arkus.reference.jsonl@1` adapter: composed canonical discovery/dispatch remains the sole semantic authority, scoped capabilities project generically without an adapter registry, cancellation/timeout are admission-only, framing and stdout/stderr boundaries are explicit, and a fresh external process flow exercises read, validation, mutation, provenance, diff, snapshot and replay.

`WP-HK-07B` PR `#47` passed independent review on frozen candidate `37ea185f28dd28e41b406f2c9407fdfe6f9b752b` (review `#5259854121`), exact-SHA freeze validation Actions `35495563546` GREEN, and merged as `58b6571b96eac4c73e5c3cf28a42a6630ea13505` on 2026-09-20. MCP over local stdio is now a genuine second projection of the accepted neutral contract: discovery/schemas/dispatch remain mechanically rooted in canonical composition; representative accepted H0 flows are semantically equivalent to JSONL; scoped capabilities project without an MCP registry; the SDK stays isolated behind the adapter; and MCP-specific naming remains framing only. One earlier frozen candidate failed because canonical-valid long capability names could exceed MCP's 128-character tool-name limit; the accepted repair assigns deterministic bounded adapter-local handles while preserving exact canonical identity for metadata and dispatch, with causal two-capability long-name coverage.

`WP-HK-08A` PR `#50` passed independent review on frozen candidate `324102d40fa0b7e36c7320216914f9d3fddadcc8` (review `#5260092080`), exact-SHA freeze validation Actions `35499569973` GREEN, and merged as `eb9d3df58df1114abafaca435da64683fdb1480e` on 2026-09-20. The accepted interaction layer raises the representative single-transaction envelope to 96 mixed operations while preserving HK04/HK05/HK06A authority, reuses compact anchored read projections, keeps `authoring.journal.read@1.0` as the complete HK06A journal contract and exposes bounded deterministic pagination only as explicit breaking `@2.0`, with integrity-bound/stale-failing cursors and JSONL↔MCP equivalence. One earlier frozen candidate failed because pagination had silently changed the accepted v1 public semantics; the repair restored v1 and versioned the new paged contract rather than redefining it in place.

`WP-HK-08B` PR `#52` passed independent review on frozen candidate `31370f91b48408b90a587d7ad5178ba1be8d6bfe` (review `#5260337340`), exact-SHA freeze validation Actions `35503766435` GREEN, and merged as `bdf4675c17d842d73ff59637fa36314d14c2707a` on 2026-09-20. The accepted recovery contract emits a precise changed-resource delta only when the exact expected revision+hash is proven inside complete contiguous current local HK06A history; unavailable/gapped history or a non-ancestor/rebase lineage fails closed as `bounded-reinspection-required`. Ordinary same-lineage recovery inspects only affected resources, retries through normal plan/dry-run/apply, preserves validation/provenance authority and remains semantically equivalent through JSONL/MCP. The representative public-client benchmark covers five authoring/repair flows in 12 requests without recovery full-world reload, with executable byte/time regression guards. One earlier frozen candidate failed solely because the binding v1.3 content-shape probe was missing; the accepted repair added an executable approved Juego2 market/plaza/bar/workshop probe and exact-SHA gate linkage without changing production recovery semantics.

`WP-HK-09A` PR `#54` passed independent review on frozen candidate `acb1ccc341aec5131dc2ef979bd322e40e208b53` (review `#5260496498`), exact-SHA freeze validation Actions `35508529666` GREEN, and merged as `614ad941881fdefa83fd46a1a8db989cfaba2cbb` on 2026-09-20. The accepted H0 authority boundary exposes no generic shell/process power, no protocol-triggered ambient network authority and no caller-selected filesystem path authority; production `--file` is rejected before the legacy framing host can open a path. `H0HostCapabilityPolicy` rejects external/elevated/unknown authority and contradictory canonical state-change metadata, while `NeutralProjectionService` independently enforces H0 admission on every composed contract before transport-visible exposure/dispatch. One earlier frozen candidate failed because public generic composition could reach neutral projection without crossing the policy; the accepted repair closes that causal seam below JSONL/MCP/future conforming transports while keeping `ContractComposer` generic, with an executable direct-composition→projection rejection fixture and an approved Juego2 content-shape probe.

`WP-HK-09B` PR `#56` passed independent review on frozen candidate `8ed02586da9a5b6e159e1cdc76a47ae7ca89c763` (review `#5261068513`), exact-SHA freeze validation Actions `35522581043` GREEN, and merged as `2e7a258fdcec2e26492d308c3cb199ab62201dcd` on 2026-09-20. The accepted `arkus.h0-resource-envelope@1` bounds canonical arguments to 896 KiB, portable depth to 32, coherent mutation batches to 96 operations, decoded mutation payload to 512 KiB, query pages to 100 items, canonical world/snapshot state to 640 KiB, world resources to 10,000, local mutation transactions to 10,000, snapshot-import receipts to 1,024 and cooperative execution to 5 s. Materialized Authoring state is rechecked before publication; mutation/import/replay publish one staged process-local aggregate only after the authoritative budget/interruption seam, preventing rejected/expired/interrupted work from producing partial state or false evidence. `arkus.reference.jsonl@1` remains frozen at 1 MiB + `transport.frame_too_large`; one earlier candidate failed because it changed that predecessor contract in place, and the accepted repair instead fits the neutral envelope beneath it. Power-loss/WAL/fsync durability, arbitrary production scale and shipping SLOs are not claimed.

`WP-HK-10` PR `#60` passed independent review on frozen candidate `813ccf08e33fdd77a34c59da5ed766882840cbee` (review `#5261301248`), exact-SHA freeze validation Actions `35527218067` GREEN, and merged as `f893ad51d756090eeecac41b8fc7cb14f8bd359a` on 2026-09-20. The accepted closure supplies deterministic property/robustness coverage, twelve causal negative-conformance controls spanning the named H0 foundational layers, a frozen Protocol v1 compatibility corpus, bounded 512-transaction endurance with fresh-session recovery inside HK09B limits, deterministic conflicting-writer recovery and complete residual-ledger reconciliation with zero unclassified residuals and zero known undetected in-boundary defect classes. One earlier frozen candidate failed because closure had introduced structured thrown-handler public semantics as HK10-owned behavior; the accepted repair obeys the closure-only rule by reopening/amending HK01 as causal owner of `contract.handler_failure` / `contract.handler_failure_after_publication` while HK10 consumes that owner proof as fault-injection evidence.

`WP-HK-GATE` PR `#64` passed independent review on frozen candidate `0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd` (review `#5261636151`), deterministic exact-SHA observation Actions `35533486939` GREEN, frozen exact-SHA validation Actions `35534660950` GREEN, and merged as `048d2e449d5ff81e8bcc35bc15664ea4ceec18ca` on 2026-09-20. The accepted gate closes the full 14-stage public readiness scenario, including bounded recovery, coherent batching, snapshot/journal/replay/diff, cross-transport equivalence, HK09A authority, HK09B resource/persistence boundaries, HK10 endurance and full headless validation. A fresh independent AI-agent MCP trial on the same exact SHA used public discovery/schemas only, completed the representative authoring/diagnostic/repair flow with no hidden/private product call, and closed with snapshot/journal evidence. One earlier frozen candidate failed because residual reconciliation was incomplete and the GATE-owned omission control attacked only a declarative label; the accepted repair mechanically reconciles all 53 HK10 residual rows and causally removes/detects the real stage-14 execution. No new product semantics were introduced by GATE.

Before implementation, the original HK06 and HK07 workpacks were deliberately split to reduce coupled foundational freeze/review risk while preserving their aggregate objectives. The executable dependency chain is `HK06A → HK06B → HK06C → HK07A → HK07B`. The old `WP-HK-06.md` and `WP-HK-07.md` remain as SUPERSEDED umbrella records and must not be implemented directly.

Before implementation, HK08 and HK09 were likewise split where each umbrella mixed two independently reviewable claims. The executable downstream chain is now `HK07B → HK08A → HK08B → HK09A → HK09B → HK10 → HK-GATE`. The old `WP-HK-08.md` and `WP-HK-09.md` remain as SUPERSEDED umbrella records and must not be implemented directly.

No H0 workpack remains. The accepted H0 boundary is the predecessor for the proposed H1 plan below. Once that plan is accepted and merged, the first dependency-valid H1 Worker is `WP-H1-00`; this planning change does not activate or freeze it.

| Order | Workpack | Outcome |
|---|---|---|
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

Status: **PLANNED / NOT_STARTED**. Initial reconstruction baseline: `c87c4c195d63cc9255745014f2b9757d9cd34050`; reconciled integration base after CITY programme v2 PASS, merge and DocSync: `7fe44840076eba05f1b67a7633cd33fc67b9023d`.

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
      -> H1-04 catalogue/identity -> H1-05 managed scenes
      -> H1-06 assets/prefabs -> H1-07 components -> H1-08 validation
      -> H1-09 reconciliation -> H1-10 checkpoint/rebuild
      -> H1-11 representative real-asset slice -> H1-GATE

 H1-08 -. non-blocking prerequisite .-> CITY-04 (after CITY-03)
 H1-GATE -. keeper authorization .----> CITY-07 (after CITY-04)
```

`H1-00` and `H1-02` share only the accepted H0 predecessor and may run in parallel only if separately authorized. The default human sequence is numeric. Every later edge requires predecessor PASS, merge and DocSync.

| Order | Workpack | Owned outcome | Execution |
|---:|---|---|---|
| 1 | `WP-H1-00` | neutral projection contract + deterministic reference materializer | `REMOTE_OK` |
| 2 | `WP-H1-01` | Unity scoped producer + automatic dependency derivation | `REMOTE_OK` |
| 3 | `WP-H1-02` | exact reproducible Unity project/toolchain/package baseline | `LOCAL_UNITY_REQUIRED` |
| 4 | `WP-H1-03` | project-scoped Unity host authority below transports | `HYBRID` |
| 5 | `WP-H1-03A` | public host-to-Editor dispatch, main-thread and lifecycle contract | `HYBRID` |
| 6 | `WP-H1-04` | effective catalogue + logical/native identity mapping | `LOCAL_UNITY_REQUIRED` |
| 7 | `WP-H1-05` | managed scene graph + generational publication | `LOCAL_UNITY_REQUIRED` |
| 8 | `WP-H1-06` | source asset/prefab fidelity + managed derivatives | `LOCAL_UNITY_REQUIRED` |
| 9 | `WP-H1-07` | finite allowlisted component schema/adapters | `LOCAL_UNITY_REQUIRED` |
| 10 | `WP-H1-08` | Unity-owned validation + actionable diagnostics | `HYBRID` |
| 11 | `WP-H1-09` | drift/reconciliation + explicit import proposals | `LOCAL_UNITY_REQUIRED` |
| 12 | `WP-H1-10` | project checkpoint + clean Unity reconstruction | `HYBRID` |
| 13 | `WP-H1-11` | exact licensed real-asset Juego2 conformance slice | `LOCAL_UNITY_REQUIRED` |
| 14 | `WP-H1-GATE` | composed Unity parity/readiness + one fresh public AI trial | `HYBRID` |

The split is claim-driven, not quota-driven. Adjacent workpacks remain separate where authority or proof can fail independently; component types and individual assets remain together where further division would create administrative micro-WPs. H0 guarantees are inherited and only delta-checked at changed seams. The sole planned fresh external AI-agent trial is the final Gate.

H1 consumes accepted CITY-00 geography only as representative shape pressure; it does not own or redraw it. CITY remote planning remains independent. `CITY-04` may use the accepted scene/prefab/component/diagnostic surface after `H1-08` to falsify the CITY-03 greybox, without becoming an H1 acceptance stage. `CITY-07` keeper realization waits for H1-GATE. CITY-08 later owns keeper-slice authoring-efficiency/reuse proof and is not duplicated by the smaller H1 Gate readiness trial.

H1 ends only when `WP-H1-GATE` answers the published reference question affirmatively without adding semantics: Arkus can create, inspect, modify, validate, materialize and reconstruct the bounded Juego2 Unity slice from public contracts while canonical identity, transactions/provenance, dependencies, diagnostics and normalized parity remain truthful.

---

# H2 — Vertical Slice Foundation (blocked by Unity parity gate)

Only after `WP-H1-GATE` PASS, merge and DocSync:

- expand beyond the exact H1-11 representative asset slice under the documented licensing/content pipeline (`DEPENDENCY_IP_POLICY.md`);
- rebuild the visual target from the visual bible (`Docs/art/VISUAL_BIBLE.md`, seeded by `WP-ART-00`);
- setting anchor: **fictional Potes / Liébana** valley market town (`Docs/art/SETTING.md`); optional later river + small inland landing;
- camera, movement, interaction shell and one representative street/plaza;
- no system is accepted unless it is inspectable/modifiable/testable through the harness.

---

# H3+ — Game Systems

Gameplay systems follow only after H0/H1 prove the AI can safely evolve the project.

---

# ART — Visual direction track (parallel, non-gating)

Art **direction** only. Does not block H0. Not bound by `FOUNDATIONAL_PROOF_STANDARD.md`.

| Order | Workpack | Outcome |
|---|---|---|
| 1 | `WP-ART-00` ✅ SEED MERGED | Visual bible + Quaternius→Liébana rules; setting lock; refs; animation; triage dry-run |
| 2+ | not yet authored | Only if formal freeze or content pipeline needed before H2 |

Deliverables: `Docs/art/VISUAL_BIBLE.md`, `Docs/art/SETTING.md`, `Docs/art/Refs/**`, `Docs/workpacks/ART/WP-ART-00.md`, `Docs/evidence/WP-ART-00/TRIAGE_DRY_RUN.md`.

Art *content* (import packs, scenes) is no longer blocked by H0, but remains blocked by the Unity bridge/parity gate and the relevant downstream H1/H2 workpacks.

---

## Global stop rules

1. Two Reviewer FAILs that expose the same foundational class trigger architecture re-audit.
2. Proof-universe self-shrink triggers immediate proof-boundary re-audit.
3. Prefer independent oracles over endless syntax-path enumeration.
4. No downstream WP patches around a false predecessor claim.
5. No feature pressure may bypass accepted harness guarantees or downstream Engine Bridge / Unity parity gates.
6. No external framework adopted merely to save time if it reduces Arkus scope, neutrality or viability.
