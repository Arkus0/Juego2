# ROADMAP — Juego2 / Arkus Harness

Version: 1.12 — 2026-09-19

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

Accepted progress: `WP-HK-00`, `WP-HK-00A`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-04` and `WP-HK-02A` are COMPLETE. `WP-HK-04` PR `#19` passed independent review on frozen candidate `849ed68e41d674ab0d50883ccd9af394e9d2e456` and merged as `b1810a5f7c5378ff06272718a11e08720d714a65` on 2026-09-19.

`WP-HK-02A` PR `#25` passed independent review on frozen candidate `f39a1994524c42213dafb63d440faaf9de7c040f` (review `#5256593405`), exact-SHA freeze validation Actions `35456445373` GREEN, and merged as `ac7ce1180b462f093cf0ee02bbbd6f853938e27c` on 2026-09-19. The earlier FAIL on `9d0dc3739f31bcc6a7f8df12b5f6e876837efacc` remains historical; its two gaps were repaired in the reviewed candidate.

Next dependency-valid workpack: `WP-HK-05 — Validation + repairable diagnostics`.

| Order | Workpack | Outcome |
|---|---|---|
| 1 | `WP-HK-00` ✅ COMPLETE | Canonical portable module boundary, pinned toolchain and headless CI |
| 2 | `WP-HK-00A` ✅ COMPLETE | Product architecture, adoption/IP boundary and anti-lock-in contract |
| 3 | `WP-HK-01` ✅ COMPLETE | Canonical contract model + machine-readable capability/schema discovery |
| 4 | `WP-HK-02` ✅ COMPLETE | Canonical world state, stable identity, deterministic serialization + hash |
| 5 | `WP-HK-03` ✅ COMPLETE | Complete read/inspection/query surface |
| 6 | `WP-HK-04` ✅ COMPLETE | Plan/dry-run/atomic apply, conflict detection and transactional mutation |
| 7 | `WP-HK-02A` ✅ COMPLETE | Object-scoped opaque extension data + typed declared dependencies |
| 8 | `WP-HK-05` | Validation/invariants and structured repairable diagnostics |
| 9 | `WP-HK-06` | Provenance, semantic diff, journal, snapshot/export and deterministic replay |
| 10 | `WP-HK-07` | Production headless host + reference JSONL + standards-compatible MCP projection |
| 11 | `WP-HK-08` | Agent ergonomics: batching, compact responses, pagination and round-trip budgets |
| 12 | `WP-HK-09` | Capability boundary: filesystem/network/process isolation and resource limits |
| 13 | `WP-HK-10` | Strict property/malformed-input/fault-injection quality closure |
| 14 | `WP-HK-GATE` | End-to-end AI-authoring readiness benchmark on a representative micro-world |

### H0 exit criteria

See prior accepted ROADMAP body; gate criteria unchanged from v1.9.

---

# H1 — Engine Bridge Foundation: Unity First (blocked by H0)

Only after `WP-HK-GATE` PASS. Detailed H1 WPs not frozen yet.

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
