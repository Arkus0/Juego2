# WP-H2F-02 Worker plan

Mode: LOCAL_UNITY_REQUIRED for licensed/vault/visual evidence (Mode A, single-environment Worker on the owner workstation, Unity 6000.3.24f1). Hosted H1 Unity workflows re-run the accepted H1 EditMode suites on the adopted project without licensed bytes.

## Write set

| Path | Why |
|---|---|
| `Unity/ArkusUnity/Packages/manifest.json`, `packages-lock.json` | exact pinned selected stack |
| `Unity/ArkusUnity/ProjectSettings/**` | URP, quality tiers, input handler, NavMesh agent, physics2d, tags/layers the stack needs |
| `Unity/ArkusUnity/Assets/Juego2/**` | new retained Juego2 production foundation (rendering baseline, shaders, input, presets, worldbuilding profiles, Arkus seam, GC2 adapter, lints, tests) |
| `Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1Baseline.cs`, `Tests/Editor/H1BaselineTests.cs` | the H1 bootstrap's render-pipeline expectation moves from built-in to the adopted URP asset (the only H1 edit) |
| `Unity/ArkusUnity/Assets/Arkus/CITY/Editor/City04GreyboxBuilder.cs`, `CITY/Materials/*.mat` | pipeline-aware greybox materials, so retained content does not render with a hidden fallback |
| `.gitignore` | GC2 vendor bytes, generated provisioning state, restored external sources |
| `scripts/h2f02-*`, `scripts/arkus-verify-exact-sha-base.sh` (routing line) | provisioning + exact-SHA verifier |
| `Docs/evidence/WP-H2F-02/**` | evidence |

Forbidden: H0/.NET kernel (`src/`, `tools/`, `tests/`); ART-01 content or its PR branch; CITY semantics or keeper geometry; GC2 vendor bytes or any separately licensed module; managed H1 roots; workpack status text (DocSync owns it).

## Decisions to make and record

1. Package set: exact direct pins (URP 17.3.0, Input System 1.20.0, Splines 2.9.1, AI Navigation 2.0.15, Animation Rigging 1.4.1, uGUI 2.0.0, Test Framework 1.6.0, explicit `com.unity.collections`/`com.unity.mathematics` for GC2, the built-in modules the stack uses, `physics2d` for GC2). Cinemachine: decided against the player role by S06; decide whether it stays admitted at all. Terrain Tools / ProBuilder / Starter Assets: not installed.
2. GC2 Core provisioning: Assets-only, hash-gated, never committed; the Juego2 GC2-dependent assemblies compile only when Core is provisioned, so a clean clone and hosted CI still compile. The mechanism is chosen by experiment on this editor.
3. `activeInputHandler`: value and rule.
4. URP baseline assets and quality tiers from the S01 values; decal rendering layers; depth texture; SSAO; Volume; sky/fog/ambient preset; GI/probe/APV policy.
5. Import/source conventions (folders, model/texture defaults, Humanoid rule, clip rules, generated outputs).
6. Adapter/preset boundary: linear profile + junction realization, scatter, water/window shaders, navigation agent, character retarget rule, GC2 player/camera presets, GC2↔Arkus adapter seam at its product location.
7. H1 lifecycle classification of every material state family (selected packages + GC2 families from `H2F02_CORE_HANDOFF.md`).

## Evidence plan

- Real project: import/compile with the stack; GC2-absent and GC2-present configurations; Juego2 + accepted H1 EditMode suites; player build compile.
- Representative content: a disposable workspace cloned from the exact candidate project, provisioned with the ART-01 `PREFOUNDATION_INPUT` route, owner-vault sources and GC2 Core; URP migration + shader audit (no hidden fallback), Humanoid/UAL import, preset materialization; captures for owner inspection.
- Negative controls for the lints and fail-closed checks.
- Exact-SHA verifier that re-reads recorded evidence and checks structural rules without licensed bytes.
