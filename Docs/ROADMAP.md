# ROADMAP — Juego2 / Arkus Harness

Version: 1.22 — 2026-09-20

## North star

Build an engine-agnostic, commercially viable AI-native game-authoring platform whose canonical contracts, transactional semantics and verification guarantees exceed any single engine-specific AI harness; then prove it by building Juego2 on top of it.

A fresh AI agent, without C# implementation knowledge, must be able to discover available capabilities and safely create, inspect, modify, validate, diff, replay and test a representative world through stable machine-readable contracts.

**No serious game production begins before `WP-HK-GATE` passes.**

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

All H0 workpacks are foundational and must pass independent review before the next begins.

Accepted progress: `WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B` and `WP-HK-08A` are COMPLETE.

`WP-HK-05` PR `#26` passed independent review on frozen candidate `23a9fd4373a803187cd9391b1459cd48975177f6` (review `#5257350871`), exact-SHA candidate observation Actions `35464742544` GREEN, freeze validation Actions `35464834162` GREEN, and merged as `ed65661680aea2a9be79f892c96aa42bf788a842` on 2026-09-19. Two prior frozen candidates failed in the same aggregate-validation-under-ambiguous-identity class; the circuit breaker triggered a causal architecture re-audit, and the accepted candidate uses dependency-local ambiguity deferral rather than global suppression.

`WP-HK-06A` PR `#32` passed independent review on frozen candidate `4ed9a925791ae14b0b5c0d92625161504021542e` (review `#5257682288`), exact-SHA candidate observation Actions `35469103222` GREEN, freeze validation Actions `35469154331` GREEN, and merged as `8089a52e8a7bbdde46e97705df6906c0d38d593a` on 2026-09-19. One earlier frozen candidate failed because a schema-valid but semantically false normalized replay envelope could remain green; the accepted repair binds the complete machine-readable request to the parsed request fingerprint/base anchor and independently verifies deterministic entry identity before atomic state/receipt/journal publication.

`WP-HK-06B` PR `#35` passed independent review on frozen candidate `2b05e982c96e7ece08cca999075c183e75eee2fd` (review `#5258130916`), exact-SHA validation Actions `35473614054` GREEN, and merged as `28e2d0aadf63fe322eac636955e9223dc9249328` on 2026-09-19. One earlier frozen candidate failed because snapshot import publicly claimed `CanonicalMutation + CanonicalTransaction` while actually replacing the whole authored session and intentionally starting a fresh empty HK06A lineage. The accepted repair models import explicitly as `CanonicalRebase`, with separate truthful rebase authority/evidence, keyed idempotency, and no fabricated HK06A mutation history.

`WP-HK-06C` PR `#40` passed independent review on frozen candidate `55fecbd8a4a5e17ce247b164cd375d652d066fdf` (review `#5259530509`), exact-SHA validation Actions `35488920548` GREEN, and merged as `e44a5e93bf0912f5b5fb80dd749e294e21a740f2` on 2026-09-20. Replay is explicitly a distinct `CanonicalReplay` authority: accepted HK06A journal evidence is verified and replayed through the accepted HK04/HK05 mutation authority inside a staged session, audited against regenerated HK06A entry identities/results, and published only after complete success; final canonical hash and HK06B semantic diff must agree.

`WP-HK-07A` PR `#44` passed independent review on frozen candidate `f2ef88980b38482a1a635f4eeb0582ee49d735ec` (review `#5259732343`), exact-SHA validation Actions `35492562005` GREEN, and merged as `f70a81b5115cd2a6b0c8b1cb9c5dd657d5f3792b` on 2026-09-20. The accepted host freezes `arkus.neutral-projection@1` above a deterministic `arkus.reference.jsonl@1` adapter: composed canonical discovery/dispatch remains the sole semantic authority, scoped capabilities project generically without an adapter registry, cancellation/timeout are admission-only, framing and stdout/stderr boundaries are explicit, and a fresh external process flow exercises read, validation, mutation, provenance, diff, snapshot and replay.

`WP-HK-07B` PR `#47` passed independent review on frozen candidate `37ea185f28dd28e41b406f2c9407fdfe6f9b752b` (review `#5259854121`), exact-SHA freeze validation Actions `35495563546` GREEN, and merged as `58b6571b96eac4c73e5c3cf28a42a6630ea13505` on 2026-09-20. MCP over local stdio is now a genuine second projection of the accepted neutral contract: discovery/schemas/dispatch remain mechanically rooted in canonical composition; representative accepted H0 flows are semantically equivalent to JSONL; scoped capabilities project without an MCP registry; the SDK stays isolated behind the adapter; and MCP-specific naming remains framing only. One earlier frozen candidate failed because canonical-valid long capability names could exceed MCP's 128-character tool-name limit; the accepted repair assigns deterministic bounded adapter-local handles while preserving exact canonical identity for metadata and dispatch, with causal two-capability long-name coverage.

`WP-HK-08A` PR `#50` passed independent review on frozen candidate `324102d40fa0b7e36c7320216914f9d3fddadcc8` (review `#5260092080`), exact-SHA freeze validation Actions `35499569973` GREEN, and merged as `eb9d3df58df1114abafaca435da64683fdb1480e` on 2026-09-20. The accepted interaction layer raises the representative single-transaction envelope to 96 mixed operations while preserving HK04/HK05/HK06A authority, reuses compact anchored read projections, keeps `authoring.journal.read@1.0` as the complete HK06A journal contract and exposes bounded deterministic pagination only as explicit breaking `@2.0`, with integrity-bound/stale-failing cursors and JSONL↔MCP equivalence. One earlier frozen candidate failed because pagination had silently changed the accepted v1 public semantics; the repair restored v1 and versioned the new paged contract rather than redefining it in place.

Before implementation, the original HK06 and HK07 workpacks were deliberately split to reduce coupled foundational freeze/review risk while preserving their aggregate objectives. The executable dependency chain is `HK06A → HK06B → HK06C → HK07A → HK07B`. The old `WP-HK-06.md` and `WP-HK-07.md` remain as SUPERSEDED umbrella records and must not be implemented directly.

Before implementation, HK08 and HK09 were likewise split where each umbrella mixed two independently reviewable claims. The executable downstream chain is now `HK07B → HK08A → HK08B → HK09A → HK09B → HK10 → HK-GATE`. The old `WP-HK-08.md` and `WP-HK-09.md` remain as SUPERSEDED umbrella records and must not be implemented directly.

Next dependency-valid workpack: `WP-HK-08B — Structured stale-CAS recovery + agent interaction benchmark`.

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
| 15 | `WP-HK-08B` | Structured stale-CAS recovery, repair ergonomics and measured agent interaction budgets |
| 16 | `WP-HK-09A` | Capability containment: filesystem/network/process authority and below-transport policy |
| 17 | `WP-HK-09B` | Resource/input limits plus import/persistence interruption integrity |
| 18 | `WP-HK-10` | Strict property/malformed-input/fault-injection quality closure + bounded endurance |
| 19 | `WP-HK-GATE` | End-to-end AI-authoring readiness benchmark on a representative micro-world |

## H0 interaction/concurrency decision

H0 keeps the accepted whole-world revision/hash CAS and does not speculate into per-resource locking, automatic merge, distributed transactions or autonomous multi-agent scheduling.

Whole-world CAS/hash is an **internal consistency boundary**, not a requirement that every client download or reconstruct the whole world after every commit. The kernel may validate/serialize/hash the accepted authored state as a whole while clients operate on bounded revision-anchored queries, semantic diffs and HK08B recovery context. AI coherence comes from stable anchors, complete relevant slices and fail-closed stale detection; repeatedly sending the entire world to an agent is neither required nor assumed to improve coherence.

Before `WP-HK-GATE`, HK08B must make the existing optimistic-concurrency model **cheap to recover from**: an ordinary same-lineage stale plan must receive bounded machine-readable context anchored to the expected and current world so the agent can preserve intent, re-plan the affected slice and retry through the normal transaction path without ordinarily reconstructing the complete world. When lineage/history is insufficient to establish a trustworthy delta, the harness must say so explicitly rather than inventing one.

Batching is the primary H0 mitigation for whole-world commit cost. HK08A accepted a representative 96-operation mixed-resource edit as one atomic validated/provenanced transaction. The 96-operation envelope is evidence for the representative H0 shape, not a permanent shipping constant; HK09B still owns measured/enforced resource limits. Do not build logical multi-plan transactions merely because a later measured cap exists; add them only if representative product evidence proves one atomic request is insufficient.

Serializing commit execution can protect the mutation authority from simultaneous publication, but it does **not** make an already planned stale request current. A writer that planned against an older revision still requires HK08B rejection/recovery/re-plan semantics. Likewise, HK06B semantic diff is an on-demand semantic comparison primitive; H0 does not claim a revision subscription/change-feed service unless a later workpack explicitly adds one.

Per-resource concurrency, automatic merge of disjoint writers, multi-process writer coordination and multi-agent scheduling are post-GATE product work unless HK08B/GATE evidence proves they are necessary for the representative single-client authoring contract. A future concurrent-agent requirement is a valid reason to revisit CAS granularity, but not a reason to delay H0 today.

HK08A has now re-run and passed reference-transport ↔ MCP conformance for its batching/compact/pagination semantics. HK08B must do the same for recovery/repair semantics it introduces; accepted HK07B/HK08A transport parity cannot pre-prove later recovery meaning.

### H0 exit criteria

`WP-HK-GATE` is authoritative for the executable gate scenario. In addition to correctness/replay/transport neutrality, H0 must demonstrate bounded same-lineage stale-plan recovery, an atomic representative batch that fits the accepted HK09B resource budget, explicit capability containment from HK09A and bounded long-session resource behaviour. Multi-agent throughput and automatic concurrent merge are explicitly not H0 gate criteria unless earlier measured evidence reclassifies them.

---

# H0S — Post-GATE scale + concurrent authoring track (parallel, non-blocking by default)

`WP-HK-GATE` validates the correctness and usability foundation; it is not a claim that whole-world work or serialized writer semantics are the final commercial scaling architecture.

After GATE, a dedicated scale/concurrency track may run in parallel with H1 Unity integration. It is **not a prerequisite for starting H1** unless measured GATE/H1 evidence shows the representative product cannot operate acceptably without it.

This track begins from evidence, not a predetermined implementation. Trigger measurements include authored object count, whole-world validation/hash cost, p50/p95 commit latency, stale-plan rate, bounded-recovery cost, simultaneous-writer collision rate, memory growth and the amount of state a client must read to preserve intent.

Candidate techniques may include cached/incremental indexes, incremental validation/hash computation, resource-scoped preconditions, server-side coordination/leases, revision change feeds or finer-grained state partitioning. A Merkle tree, per-resource CAS or scope lock is an option only if evidence justifies it, not the definition of the solution.

Any concurrency/coordination semantic that affects whether or when a valid canonical request may execute must live in a transport-neutral reviewed service/contract boundary. It may not exist only inside MCP, JSONL or another adapter, and it may not silently weaken canonical validation, provenance, replay or deterministic state identity.

The commercial target is therefore two-layered: H0 provides a simple globally coherent correctness model; post-GATE scaling may optimize its implementation and writer coordination while preserving or explicitly versioning those semantics.

---

# H1 — Engine Bridge Foundation: Unity First (blocked by H0)

Only after `WP-HK-GATE` PASS. Detailed H1 WPs not frozen yet.

The first H1/scoped-extension producer work must preserve the HK02A opaque-kernel boundary while removing an avoidable AI footgun: when a schema-aware producer/codec understands references embedded in its payload, it must derive the corresponding typed dependency surface mechanically from the structured input it serializes. The AI should not normally be responsible for manually keeping opaque payload references and `dependencies` synchronized. Direct opaque payload + dependency authoring remains a low-level primitive; the engine-agnostic kernel still does not introspect arbitrary payload bytes.

---

# H2 — Vertical Slice Foundation (blocked by Unity parity gate)

Only after the Unity bridge is proven:

- import/select Quaternius assets under a documented licensing/content pipeline (`DEPENDENCY_IP_POLICY.md`);
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

Art *content* (import packs, scenes) remains blocked by H0 gate and Unity parity gate.

---

## Global stop rules

1. Two Reviewer FAILs that expose the same foundational class trigger architecture re-audit.
2. Proof-universe self-shrink triggers immediate proof-boundary re-audit.
3. Prefer independent oracles over endless syntax-path enumeration.
4. No downstream WP patches around a false predecessor claim.
5. No feature pressure may waive the harness gate.
6. No external framework adopted merely to save time if it reduces Arkus scope, neutrality or viability.