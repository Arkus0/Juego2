# Adapter / preset boundary (AI-native authoring surface)

The smallest stable Juego2-side surfaces that CITY, H2, GC2-00, Astra and later agents use instead of authoring raw plugin-private state. Each surface is Juego2 code or a Juego2 asset. The package or vendor type behind it stays replaceable, and none of it is canonical Arkus state.

## 1. Foundation baseline (bootstrap authority)

| Surface | Use |
|---|---|
| `Juego2.Foundation.Editor.J2FoundationBatch.Apply` / `.Verify` | apply or check the URP/toolchain baseline. `Verify` returns stable codes (`J2_*`) and writes a JSON report (`-j2-output`); `-j2-require-gc2` also requires Core. |
| `J2FoundationBatch.MigrateRetainedMaterials` | URP migration + audit of retained project materials |
| `J2FoundationLook.ApplyTo(scene, J2_LookPreset)` | baseline look for a lighting sidecar scene |
| `J2ShaderAudit`, `J2MaterialMigration` | fail-closed material audit and built-in → URP upgrade for any content root |
| `J2ImportConventions.ApplyHumanoid / Verify / ApplyModelDefaults` | frozen import rules |

## 2. Worldbuilding realization (CITY/ART decide where and what; these decide how)

| Surface | Input (authorable data) | Output |
|---|---|---|
| `J2LinearRealizer.Realize(SplineContainer, J2LinearProfile, Options)` | Splines centreline + profile (width, crown, kerb/wall edges, materials, decal-receiving layers) + junction openings | traversable surface (sole collider), kerbs, retaining walls under a `J2GeneratedRealization` root (input/output digests) |
| `J2JunctionRealizer.Realize(main, mainProfile, distance, branch, branchProfile, cornerRadius)` | two centrelines, the branch leaving roughly perpendicular (≤ 12°) | main road with opening, branch from its first real cross-section, apron and kerb fillets. Oblique branches and crossroads fail with `J2_JUNCTION_NOT_TEE`. |
| `J2SurfaceContinuity.Sample(...)` | expected traversable region | holes/steps oracle (0/0 required) |
| `J2ScatterRealizer.Realize(host, area, J2ScatterProfile, seed)` | admitted prefabs + weights + scale bands, spacing, density, margin, slope; no-go = generated roads/edges + `J2ScatterExclusion` volumes | seed-deterministic placements (output digest) |
| Shaders `Juego2/StylizedWater`, `Juego2/InteriorWindow` | material parameters (ART tunes) | water with depth fade/foam; interior mapping for non-enterable windows |
| NavMesh | `NavMeshSurface` over traversable colliders, `NavMeshModifier` Not Walkable on scenic, project agent | baked NavMeshData (generated) |

**Reuse on compact urban blocks** (post-01A urban amendment): the same profiles realize port-city streets. A block is a set of centrelines with per-street `J2LinearProfile` presets: carriageway + kerbs; lane with retaining wall; stepped pedestrian lane as an owner-authored piece. Tee junctions join side streets to a main street. Plazas and crossroads beyond a tee are authored meshes or a later realizer extension, which is a capability extension, not a new tooling choice. Scatter keeps planters and street nature off traversable surfaces and out of `J2ScatterExclusion` volumes (shop fronts, thresholds, quays). Water/quay edges use the water shader with a retaining-wall profile. Nothing here needs a port keeper scene, a new district graph or an extra licensed module: `CITY-URBAN-00/01` decide the geometry, ART-URBAN-01 the kit.

## 3. Characters, camera and input (S06 amendment)

| Surface | Use |
|---|---|
| `J2_Input.inputactions` (`Player`: Move, Look, Zoom, Interact) + `J2InputOwner` | the only input source; Juego2 enables and disables it |
| `J2_Player`, `J2_Npc_Civilian` (`J2CharacterPreset`) | height 1.8, radius 0.28, speed 1.45 (player) / 1.2 (NPC, NavMesh driver), Fulcrum footsteps. Model and locomotion controller are content inputs. |
| `J2_PlayerCamera` (`J2CameraPreset`) | GC2 Third Person shot: radius 3, auto-align 0.5/1.0, near 0.05, FOV 55, Look/Zoom on `J2_Input` |
| `Juego2.Gc2Adapter.Editor.J2Gc2Presets.MaterializeCharacter(preset, entityKey, feet)` / `.MaterializePlayerCamera(preset)` | builds GC2 objects through GC2's public editor entry points + the recorded serialized paths (01A `PUBLIC_AUTHORING_SURFACE.md`). Fails closed with `J2_GC2_AUTHORING_PATH_MISSING` if a Core version moves a field. |
| `J2ImportConventions` Humanoid rule | retarget-ready bodies and UAL libraries |

## 4. GC2 ↔ Arkus seam (GC2 executes, Arkus remembers and connects)

```text
Arkus canonical authority  (production host bound by WP-GC2-00 through the accepted H0 boundary)
      ▲  IArkusFactAuthority: GetFact(key) / Request(requester, entity, transition) -> Accepted | Refused | Conflict
      │  ArkusFacts.Bind(authority); fails closed with NO_AUTHORITY / UNBOUND_REQUESTER / UNBOUND_ENTITY
Juego2.Arkus  (no GameCreator, no plugin types): ArkusEntityBinding (j2.* keys = the only identity), ArkusFacts
      ▲
Juego2.Gc2Adapter  (references GameCreator.Runtime.Core + Juego2.Arkus only; compiled only when Core is provisioned)
      ConditionArkusFact · InstructionArkusRequestTransition (stops the list on refusal) · EventOnArkusFactChanged
      GetStringArkusFact · GetStringArkusLiteral · GetGameObjectArkusEntity     ("Juego2/…" in the GC2 pickers)
      ▼
GC2 Core 2.19.61: bodies, camera, interaction, visual-scripting execution, presentation
```

- Type names are frozen seam names (GC2 serializes them by name). `[MovedFrom]` maps the 01A probe names.
- The 01A `ArkusSaveHost` / `Juego2WorkspaceStorage` are **not** ported: the save host is `DEFER_EVALUATION` for H6. Lint L7 forbids any Juego2 persistence through GC2 until then.
- **Lints** (`J2Gc2Lint`, batch `J2Gc2Batch.LintProject`; L1 in `J2AssemblyBoundary`):
  - L1: only the gated adapter assemblies reference `GameCreator.*`; no GameCreator in Arkus/CITY/story/foundation code; no GUID asmdef references.
  - L2: no `Remember`, no GC2 scene load/unload instruction.
  - L3: no GC2 Variable in the Juego2 key namespace.
  - L4: an adapter entry sits on a Juego2-bound object.
  - L5: a stock mutating instruction on a bound object only follows an Arkus request that stops on refusal.
  - L6: GC2 inputs read only `J2_Input`; NPC input is None.
  - L7: no Juego2 `IGameSave` / data storage.
- Legacy-input rule: no Juego2/Arkus code uses `UnityEngine.Input`, except the two named transition tools (`J2AssemblyBoundary.LegacyInputTransitionTools`).

## 5. What is canonical and what is not

Canonical: Arkus world state, entity keys `j2.*`, facts and transitions, the CITY semantic layout and ART content truth, all under their accepted owners.

Not canonical, and never read back as canonical: every Unity/GC2/package object above, including GC2 component state, Variables, save IDs, GUIDs, spline knots, generated meshes and bakes. Unity→canonical stays an explicit H1 proposal.

## 6. Replacement boundaries

| Dependency | Replace by | Juego2 surfaces that stay |
|---|---|---|
| URP | another SRP | presets/profiles, audit vocabulary (re-target), content |
| Splines | another centreline host | `J2LinearProfile`, realizer outputs, oracle |
| AI Navigation | another nav executor | agent values, traversable-collider rule |
| Animation Rigging | another IK solution | presentation prefabs |
| GC2 Core | the documented H2F-01 minimal controller + camera rig (01A replacement note) | Arkus seam (`Juego2.Arkus`), input asset, presets' values, UAL controller, avatar mapping |
