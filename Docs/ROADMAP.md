# ROADMAP — Juego2

Version: 1.1 — 2026-09-18

## North star

Build the best practical AI-native game-development harness we can justify, then build the game on top of it.

The harness must let a fresh AI agent, without C# implementation knowledge, discover the available capabilities and safely create, inspect, modify, validate, diff, replay and test a representative world through stable machine-readable contracts.

**No serious game production begins before `WP-HK-GATE` passes.**

## Architectural direction

```text
AI / future Creator GUI / scripts
             ↓
      stable Harness Protocol
             ↓
   Authoring + Validation kernel
             ↓
       Core / World state
             ↓
   downstream adapters (Unity later)
```

The harness core is engine-agnostic and transport-agnostic. Unity is deliberately downstream. DFU is excluded from the critical path; it can only return later as an optional adapter justified by an ADR after the harness gate.

The old `Arkus0/Juego` repository is a reference archive, not a migration source of authority.

---

# H0 — Harness Kernel

All H0 workpacks are foundational and must pass independent review before the next begins.

| Order | Workpack | Outcome |
|---|---|---|
| 1 | `WP-HK-00` | Canonical portable module boundary, pinned toolchain and headless CI |
| 2 | `WP-HK-01` | Versioned protocol envelopes + machine-readable capability/schema discovery |
| 3 | `WP-HK-02` | Canonical world state, stable identity, deterministic serialization + hash |
| 4 | `WP-HK-03` | Complete read/inspection/query surface |
| 5 | `WP-HK-04` | Plan/dry-run/atomic apply, conflict detection and transactional mutation |
| 6 | `WP-HK-05` | Validation/invariants and structured repairable diagnostics |
| 7 | `WP-HK-06` | Provenance, semantic diff, journal, snapshot/export and deterministic replay |
| 8 | `WP-HK-07` | Production headless host/CLI transport with closed process boundary |
| 9 | `WP-HK-08` | Agent ergonomics: batching, compact responses, pagination and round-trip budgets |
| 10 | `WP-HK-09` | Safety boundary: filesystem/network/process isolation and resource limits |
| 11 | `WP-HK-10` | Adversarial/property/fuzz/fault-injection quality closure |
| 12 | `WP-HK-GATE` | End-to-end AI-authoring readiness benchmark on a representative micro-world |

### H0 exit criteria

`WP-HK-GATE` cannot PASS unless all of the following are demonstrated from a clean checkout:

- a fresh client can discover capabilities without reading C#;
- schemas describe requests, results and errors;
- a micro-world can be created entirely through the harness;
- the world can be inspected completely enough to make further edits safely;
- changes support dry-run and atomic application;
- invalid changes fail with stable structured diagnostics and can be repaired;
- concurrent/stale edits are detected rather than silently overwriting state;
- snapshots and journal replay reproduce the same canonical state hash;
- provenance identifies what changed, why and from which request;
- malformed/adversarial inputs fail closed;
- the harness runs headlessly in CI with no hidden editor/manual step;
- no direct repository/world-state edits are required to complete the benchmark;
- a measured interaction-budget baseline exists and batching prevents pathological one-command-per-field authoring;
- all foundational proof obligations have zero unresolved and zero known-undetected classes.

If the gate cannot be proven without excessive complexity, STOP and simplify the harness architecture before starting Unity.

---

# H1 — Unity Bridge (blocked by H0)

Only after `WP-HK-GATE` PASS:

- select and pin the then-current production Unity version from fresh evidence (prefer the appropriate supported/LTS line for the project rather than inheriting a legacy DFU-era version);
- record the selected Unity/editor/package baseline in an H1 ADR/workpack so builds are reproducible;
- define Unity as a downstream projection/adapter over the accepted kernel;
- prove Unity consumes the same canonical game contracts rather than a duplicate implementation;
- establish scene/prefab projection, runtime bootstrap and Unity-side validation;
- add Unity parity gate before content production;
- keep Unity-version-specific APIs, packages and editor integration behind the Unity adapter boundary wherever practical;
- treat future Unity upgrades as an adapter/parity migration: an engine update must not require redesigning the accepted H0 protocol, canonical world model or authoring kernel unless a separately reviewed contract change proves that necessary.

### Unity upgradeability rule

H0 must not be coupled to one Unity release. H1 may pin a concrete Unity version for reproducibility, but that pin is a deployment/integration baseline, not a permanent architectural dependency. A later Unity upgrade must be validated through the Unity bridge/parity evidence before it becomes the new accepted baseline.

Detailed H1 WPs are intentionally **not frozen yet**. They will be authored from the accepted H0 contracts and the then-current Unity/tooling landscape instead of guessing today.

---

# H2 — Vertical Slice Foundation (blocked by Unity gate)

Only after the Unity bridge is proven:

- import/select Quaternius assets under a documented licensing/content pipeline;
- rebuild the visual target from the visual bible;
- camera, movement, interaction shell and one representative street/plaza;
- no system is accepted unless it is inspectable/modifiable/testable through the harness.

---

# H3+ — Game Systems

Gameplay systems, NPC simulation, schedules, relationships, quests, shops, minigames, combat and content follow only after H0/H1 prove that the AI can safely evolve the project.

The detailed gameplay roadmap will be replanned from the accepted harness and Unity contracts rather than copied from `Juego`.

---

## Global stop rules

1. Two Reviewer FAILs that expose the same foundational class trigger architecture re-audit, not another local patch.
2. If a proof starts growing by enumerating endless syntax-specific bypasses, prefer an effective/evaluated oracle or simplify the boundary.
3. No downstream WP patches around a false predecessor claim.
4. No feature pressure may waive the harness gate.
