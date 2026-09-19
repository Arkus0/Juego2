# ROADMAP — Juego2 / Arkus Harness

Version: 1.16 — 2026-09-20

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

Accepted progress: `WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04`, `WP-HK-02A`, `WP-HK-05`, `WP-HK-06A` and `WP-HK-06B` are COMPLETE.

`WP-HK-05` PR `#26` passed independent review on frozen candidate `23a9fd4373a803187cd9391b1459cd48975177f6` (review `#5257350871`), exact-SHA candidate observation Actions `35464742544` GREEN, freeze validation Actions `35464834162` GREEN, and merged as `ed65661680aea2a9be79f892c96aa42bf788a842` on 2026-09-19. Two prior frozen candidates failed in the same aggregate-validation-under-ambiguous-identity class; the circuit breaker triggered a causal architecture re-audit, and the accepted candidate uses dependency-local ambiguity deferral rather than global suppression.

`WP-HK-06A` PR `#32` passed independent review on frozen candidate `4ed9a925791ae14b0b5c0d92625161504021542e` (review `#5257682288`), exact-SHA candidate observation Actions `35469103222` GREEN, freeze validation Actions `35469154331` GREEN, and merged as `8089a52e8a7bbdde46e97705df6906c0d38d593a` on 2026-09-19. One earlier frozen candidate failed because a schema-valid but semantically false normalized replay envelope could remain green; the accepted repair binds the complete machine-readable request to the parsed request fingerprint/base anchor and independently verifies deterministic entry identity before atomic state/receipt/journal publication.

`WP-HK-06B` PR `#35` passed independent review on frozen candidate `2b05e982c96e7ece08cca999075c183e75eee2fd` (review `#5258130916`), exact-SHA validation Actions `35473614054` GREEN, and merged as `28e2d0aadf63fe322eac636955e9223dc9249328` on 2026-09-20 Europe/Rome. One earlier frozen candidate failed because snapshot import was publicly classified as an ordinary canonical mutation/transaction while effective execution was a whole-session rebase with a deliberately empty new local mutation journal. The accepted repair introduces an explicit canonical-rebase classification and truthful machine-readable rebase evidence without weakening HK04/HK06A mutation semantics.

Before implementation, the original HK06 and HK07 workpacks were deliberately split to reduce coupled foundational freeze/review risk while preserving their aggregate objectives. The executable dependency chain is now `HK06A → HK06B → HK06C → HK07A → HK07B`. The old `WP-HK-06.md` and `WP-HK-07.md` remain as SUPERSEDED umbrella records and must not be implemented directly.

Next dependency-valid workpack: `WP-HK-06C — Deterministic journal replay + end-to-end audit consistency`.

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
| 11 | `WP-HK-06C` | Deterministic journal replay + end-to-end audit consistency |
| 12 | `WP-HK-07A` | Production headless host + deterministic JSONL/reference transport |
| 13 | `WP-HK-07B` | Standards-compatible MCP projection + cross-transport conformance |
| 14 | `WP-HK-08` | Agent ergonomics: batching, compact responses, pagination, structured CAS recovery and round-trip budgets |
| 15 | `WP-HK-09` | Capability boundary: filesystem/network/process isolation and resource limits |
| 16 | `WP-HK-10` | Strict property/malformed-input/fault-injection quality closure + bounded endurance |
| 17 | `WP-HK-GATE` | End-to-end AI-authoring readiness benchmark on a representative micro-world |

## H0 interaction/concurrency decision

H0 keeps the accepted whole-world revision/hash CAS and does not speculate into per-resource locking, automatic merge, distributed transactions or autonomous multi-agent scheduling.

Before `WP-HK-GATE`, HK08 must make the existing optimistic-concurrency model **cheap to recover from**: a stale plan must receive bounded machine-readable context anchored to the expected and current world so the agent can preserve intent, re-plan the affected slice and retry through the normal transaction path without ordinarily reconstructing the complete world.

Batching is the primary H0 mitigation for whole-world commit cost. The current 64-operation request limit is not a permanent product constant: HK08 measures a representative coherent multi-resource edit, and the accepted request shape/limit must allow that edit to remain one atomic transaction inside the HK09 resource envelope. Do not build logical multi-plan transactions merely because an arbitrary number such as 64 exists; add them only if measured representative work proves one atomic request is insufficient.

Per-resource concurrency, automatic merge of disjoint writers, multi-process writer coordination and multi-agent scheduling are post-GATE product work unless HK08/GATE evidence proves they are necessary for the representative single-client authoring contract. A future concurrent-agent requirement is a valid reason to revisit the CAS granularity, but not a reason to delay H0 today.

HK08 also re-runs reference-transport ↔ MCP conformance for every interaction primitive it changes or adds. HK07B proves the initial neutral projection; it cannot pre-prove semantics introduced later by HK08.

### H0 exit criteria

`WP-HK-GATE` is authoritative for the executable gate scenario. In addition to correctness/replay/transport neutrality, H0 must demonstrate bounded stale-plan recovery, an atomic representative batch that fits the accepted resource budget, and bounded long-session resource behaviour. Multi-agent throughput and automatic concurrent merge are explicitly not H0 gate criteria unless earlier measured evidence reclassifies them.

---

# H1 — Engine Bridge Foundation: Unity First (blocked by H0)

Only after `WP-HK-GATE` PASS. Detailed H1 WPs are not frozen yet.

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
