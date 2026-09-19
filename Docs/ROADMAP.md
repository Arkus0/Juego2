# ROADMAP — Juego2 / Arkus Harness

Version: 1.5 — 2026-09-19

## North star

Build an engine-agnostic, commercially viable AI-native game-authoring platform whose canonical contracts, transactional semantics and verification guarantees exceed any single engine-specific AI harness; then prove it by building Juego2 on top of it.

A fresh AI agent, without C# implementation knowledge, must be able to discover available capabilities and safely create, inspect, modify, validate, diff, replay and test a representative world through stable machine-readable contracts.

**No serious game production begins before `WP-HK-GATE` passes.**

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

The canonical contract and kernel are engine-agnostic, transport-agnostic and model-vendor-agnostic. MCP is a first-class standards adapter, not the source of truth. Unity is the first engine bridge, not the platform boundary. External frameworks may be adopted selectively where they solve generic infrastructure better, but Arkus must remain a functional superset rather than inheriting another harness's ceiling.

The canonical contract system supports later reviewed scoped capability contributions. Engine bridges may contribute definitions/bindings only through Arkus-owned canonical composition; they may not publish parallel public registries, and H0 base contracts remain free of engine implementation types.

The old `Arkus0/Juego` repository is a reference archive, not a migration source of authority.

---

# H0 — Harness Kernel

All H0 workpacks are foundational and must pass independent review before the next begins.

Accepted progress: `WP-HK-00` and `WP-HK-00A` are COMPLETE. `WP-HK-00A` PR `#15` passed independent review on frozen candidate `e66ed729c75d94fb7efdfc304cf51ec52fa25e53` and merged as `0a4253443326ccab8e910c3265b015fc757cc4a2` on 2026-09-19. Its product/adoption boundary is now binding.

Next dependency-valid workpack after DocSync: `WP-HK-01`.

| Order | Workpack | Outcome |
|---|---|---|
| 1 | `WP-HK-00` ✅ COMPLETE | Canonical portable module boundary, pinned toolchain and headless CI |
| 2 | `WP-HK-00A` ✅ COMPLETE | Product architecture, adoption/IP boundary and anti-lock-in contract |
| 3 | `WP-HK-01` | Canonical contract model + machine-readable capability/schema discovery |
| 4 | `WP-HK-02` | Canonical world state, stable identity, deterministic serialization + hash |
| 5 | `WP-HK-03` | Complete read/inspection/query surface |
| 6 | `WP-HK-04` | Plan/dry-run/atomic apply, conflict detection and transactional mutation |
| 7 | `WP-HK-05` | Validation/invariants and structured repairable diagnostics |
| 8 | `WP-HK-06` | Provenance, semantic diff, journal, snapshot/export and deterministic replay |
| 9 | `WP-HK-07` | Production headless host + reference JSONL + standards-compatible MCP projection |
| 10 | `WP-HK-08` | Agent ergonomics: batching, compact responses, pagination and round-trip budgets |
| 11 | `WP-HK-09` | Safety boundary: filesystem/network/process isolation and resource limits |
| 12 | `WP-HK-10` | Adversarial/property/fuzz/fault-injection quality closure |
| 13 | `WP-HK-GATE` | End-to-end AI-authoring readiness benchmark on a representative micro-world |

### H0 exit criteria

`WP-HK-GATE` cannot PASS unless all of the following are demonstrated from a clean checkout:

- a fresh client can discover capabilities without reading C#;
- schemas describe requests, results and errors from one canonical contract source;
- transport projections do not create a second semantic truth;
- the deterministic reference transport and the MCP projection are contract-conformant and semantically equivalent for the gate surface;
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
- the product boundary remains engine/transport/vendor neutral and approved external dependencies are replaceable;
- the canonical contract system has one Arkus-owned composition path for future scoped providers and no adapter-owned public registry;
- all foundational proof obligations have zero unresolved and zero known-undetected classes.

If the gate cannot be proven without excessive complexity, STOP and simplify the harness architecture before starting an engine bridge.

---

# H1 — Engine Bridge Foundation: Unity First (blocked by H0)

Only after `WP-HK-GATE` PASS:

- define and review `Arkus.Engine`-level abstractions before engine-specific implementation;
- select and pin the then-current production Unity version from fresh evidence (prefer the appropriate supported/LTS line for the project rather than inheriting a legacy DFU-era version);
- record the selected Unity/editor/package baseline in an H1 ADR/workpack so builds are reproducible;
- implement Unity as the first downstream projection/adapter over the accepted kernel;
- prove Unity consumes the same canonical game contracts rather than a duplicate implementation;
- register every public Unity/engine-scoped capability through the Arkus-owned canonical composition path defined by HK01; the Unity bridge may contribute scoped definitions and implementation bindings but may not own a parallel discovery/schema registry;
- express Unity-scoped public request/result/error data without exposing Unity runtime/editor class types as canonical kernel dependencies;
- keep engine-scoped capabilities inside the accepted canonical policy/transaction/provenance envelope, with canonical-state mutation still entering the canonical authoring transaction pipeline;
- establish scene/prefab/runtime projection, engine observation/evidence and Unity-side validation;
- establish deterministic playtest/visual-evidence capabilities without making Unity the owner of scenario semantics;
- add Unity parity gate before content production;
- benchmark practical capability against the strongest relevant engine-authoring harness patterns identified in `EXTERNAL_HARNESS_ADOPTION_AUDIT.md` so Arkus does not become architecturally cleaner but functionally narrower;
- keep Unity-version-specific APIs, packages and editor integration behind the Unity adapter boundary wherever practical;
- treat future Unity upgrades as an adapter/parity migration: an engine update must not require redesigning the accepted H0 protocol, canonical world model or authoring kernel unless a separately reviewed contract change proves that necessary.

### Engine portability rule

H0 must contain no Unity/Godot/Unreal object-model assumption. H1 may pin Unity for reproducibility, but the engine abstraction must remain capable of supporting another engine without rewriting canonical authoring semantics. A future second-engine bridge is evidence of portability, not a prerequisite for Juego2 production.

Detailed H1 WPs remain intentionally **not frozen yet**. They will be authored from the accepted H0 contracts and then-current engine/tooling landscape rather than guessed in advance.

---

# H2 — Vertical Slice Foundation (blocked by Unity parity gate)

Only after the Unity bridge is proven:

- import/select Quaternius assets under a documented licensing/content pipeline;
- rebuild the visual target from the visual bible;
- camera, movement, interaction shell and one representative street/plaza;
- no system is accepted unless it is inspectable/modifiable/testable through the harness.

---

# H3+ — Game Systems

Gameplay systems, NPC simulation, schedules, relationships, quests, shops, minigames, combat and content follow only after H0/H1 prove that the AI can safely evolve the project.

The detailed gameplay roadmap will be replanned from the accepted harness and engine contracts rather than copied from `Juego`.

---

## Global stop rules

1. Two Reviewer FAILs that expose the same foundational class trigger architecture re-audit, not another local patch.
2. A finding that the proof universe can self-shrink — for example by omitting projects, sources, commands, validators or adapters from the inventory that claims completeness — triggers immediate proof-boundary re-audit.
3. If a proof starts growing by enumerating endless syntax-specific bypasses, prefer an independent/evaluated oracle or simplify the boundary.
4. No downstream WP patches around a false predecessor claim.
5. No feature pressure may waive the harness gate.
6. No external framework is adopted merely to save time if doing so reduces Arkus scope, engine neutrality, transport neutrality, replaceability or commercial viability.
