# WP-H2F-03 Worker plan

Mode: LOCAL, real Unity visual-play evidence (Mode A, single-environment Worker on the owner workstation). Unity 6000.3.24f1 (4e7b9b5b6244), batch for build/lifecycle steps, windowed for play mode (this workstation's licence has no headless play entitlement, H2F-01/H2F-02). .NET SDK 8.0.425 for the public H1 host. Owner-vault sources (`C:\Juego2-Assets` at the pinned `ae782c5f`) and the lawful GC2 Core copy stay outside the repository.

## Write set

| Path | Why |
|---|---|
| `Docs/evidence/WP-H2F-03/**` | evidence, fixture brief/spec, workspace recipe, evidence-only Unity code, lifecycle and trial drivers, results, captures |
| `scripts/h2f03-*`, `scripts/arkus-verify-exact-sha-base.sh` (routing lines only) | exact-SHA verifier |
| `Unity/ArkusUnity/Assets/Juego2/**` | **only** causal compatibility repairs that the integrated fixture exposes inside the H2F-02 adapter/preset boundary, each with a test and an `ISSUE_LEDGER.md` row. No tool choice changes. |

Forbidden: `src/`, `tools/`, `tests/` (H0/H1 .NET); `Assets/Arkus/H1/**` and H1 managed roots; `Packages/**` and `ProjectSettings/**` unless a recorded pathology makes a baseline amendment unavoidable (then STOP and escalate, because that is an H2F-01/02 decision); ART-01 content or its branch; CITY semantics; GC2 vendor bytes or any separately licensed module; workpack status text (DocSync owns it).

## Fixture: `H2F_INTEGRATION_FIXTURE / NON_KEEPER`

One short cobbled street with one house, one climbing side lane, a pond and bounded nature. It is marked NON_KEEPER in its scene name, root object and brief (`FIXTURE_BRIEF.md`). It claims nothing for ART-01, CITY-07 or a real port street.

Two layers, matching the H2F-02 boundary:

1. **Canonical, H1-managed** (authored only through the public `--h1-unity` reference host into the fixed managed scene): the house as Medieval Village Source modules (corner, facade with window + shutters, wall with door + door, side wall, roof), a market crate with the shared material override, and one UAL1 humanoid with an idle clip and a canonical link to the door. Catalogue identifiers come from public discovery.
2. **Retained realization sidecar** (`Assets/H2F03Fixture/H2F03_IntegrationFixture_NONKEEPER.unity`, the active scene; the managed scene loads additively): `J2FoundationLook` (sun, trilight, fog, sky, Volume); Terrain for scenic ground (pond basin, rising ground by the lane); the main street and the tee-junction side lane through `J2LinearRealizer`/`J2JunctionRealizer` (kerbs on the street, retaining wall on the lane's downhill side); one URP decal on the street that must not bleed onto the canonical house (decal layers); `Juego2/StylizedWater` pond with a `J2ScatterExclusion`; `Juego2/InteriorWindow` behind the canonical window; seeded nature scatter (ART Nature) with road, house, threshold and pond exclusions; `NavMeshSurface` over the traversable realization with `NavMeshModifier` Not Walkable on terrain and baked NavMeshData; GC2 player (`J2_Player`, ART citizen, UAL locomotion) and `J2_PlayerCamera`; one GC2 NPC on the NavMesh driver walking the street and lane; one humanoid with Animation Rigging hand-contact IK from a rigged presentation prefab; one GC2 interaction hook at the door threshold (Hotspot + Trigger on a Juego2-bound sidecar object → `InstructionArkusRequestTransition` → stock light instruction), answered by a fixture-local `IArkusFactAuthority`.

The fixture is built by evidence-only editor code in a disposable workspace, never committed as product. The sidecar only positions realization relative to the canonical layout; it never reads managed objects back as truth.

## Composed H1 lifecycle (public host only)

```text
W0  workspace = git archive of the candidate (repo + project, no Library/ignored state); build .NET Release
W1  H1 vault mount (Tools/AssetVault, pinned); GC2 provisioning (hash-gated); ART-01 + H2F-01 inputs (H2F-02 recipe)
W2  build the sidecar fixture (batch)
L1  host A: inspect → discover → catalogue → author (compile → plan → dry-run → apply) → projection plan → materialize → observe → drift parity
L2  witness BEFORE (batch, read-only): every H2F-02 matrix family
L3  legal change: operator edit of one managed Transform → drift → import-proposal → plan/dry-run/apply → rematerialize → drift parity
L4  checkpoint capture; close host A; remove H1 generated output (ManagedScenes/ManagedPrefabs)
L5  host B (empty process): checkpoint restore → clean-rebuild → observe → drift parity (sidecar present, not surfaced)
L6  generated/external restoration: NavMeshData deleted and rebaked; GC2 removed and re-provisioned; Library deleted and reimported
L7  witness AFTER; per-family comparison against the H2F-02 matrix
L8  windowed play: reopen sidecar + managed scene; scripted traversal attempts (capture, then two clean runs) with profiler and error capture
L9  owner third-person play inspection
```

The legal change stays inside accepted H1 authority. H1 is never given ownership of plugin-private realization.

## Fresh-author / agent trial

After L8, on the same workspace. A fresh agent session with no Worker context receives only `TRIAL_BRIEF.md` (fixture brief + operation), the H2F-02 `ADAPTER_BOUNDARY.md`, a generated public API listing of the Juego2 foundation assemblies, and three relay tools: the public H1 reference host (kept alive across calls), a Unity batch runner for editor scripts it writes under one authoring folder, and a read-only fixture inventory. Operation: add one catalogue prop to the canonical slice through Arkus (plan → dry-run → apply → materialize), then keep nature clear of it through the documented scatter surface, inspect, and show unrelated content was not regenerated. Every relayed call is ledgered. The verifier decides from the ledger and inventories, not from the agent's report.

## Performance

`PERFORMANCE_ORACLE.md` declares the comparison context and the five enumerated pathologies. It is committed **before** any capture. No numeric threshold is added.

## Evidence map (planned files)

`README.md` (PASS conditions → evidence), `FIXTURE_BRIEF.md`, `LIFECYCLE_PROOF.md` + `results/lifecycle_*.json`, `PLAY_EVIDENCE.md` + captures, `PERFORMANCE_ORACLE.md` + `results/profile_*.json`, `TRIAL_BRIEF.md` + `results/trial_*`, `OWNER_JUDGEMENT.md`, `ISSUE_LEDGER.md`, `URBAN_REUSE_PROFILE.md`, `workspace/`, `tools/`. Exact-SHA verifier `scripts/h2f03-verify-exact-sha.sh` re-reads the recorded evidence and binds it to the candidate's `Unity/ArkusUnity` tree and evidence tooling bytes, without Unity or licensed bytes.
