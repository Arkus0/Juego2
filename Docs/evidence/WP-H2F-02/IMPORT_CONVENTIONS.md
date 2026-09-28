# Import and source-content conventions

Project-wide rules frozen before CITY-07/H2 author retained content. They are enforced by explicit apply + fail-closed verify operations (`J2ImportConventions`, `J2ShaderAudit`, `J2MaterialMigration`). The foundation does **not** install a postprocessor over ART-owned folders: ART keeps visual/content semantics, and the foundation owns only the engine-wide rules below.

## Folders: source vs derived vs generated

| Class | Location convention | Git | Rule |
|---|---|---|---|
| Restored external source bytes | `…/External/**` of the owning track (ART: `Assets/Arkus/ART/External`; H1: `Assets/Arkus/H1/SourceSlice`) | ignored | Restored from the owner vault through a hash lock, with deterministic `.meta` GUIDs. Read-only and never edited in place. |
| Juego2 derivatives | `…/Derived/**` of the owning track (e.g. `Assets/Arkus/ART/Derived/Characters`) | committed | Separately identified, lineage recorded by the owner (ART). The Humanoid mapping is part of a character derivative's contract. |
| Licensed tool bytes | `Assets/Plugins/GameCreator` | ignored | Restored by `scripts/h2f02-provision.py`. Never committed. |
| Foundation configuration and tools | `Assets/Juego2/Foundation/**`, `Assets/Juego2/Arkus`, `Assets/Juego2/Gc2Adapter` | committed | Juego2-owned. |
| Generated realizations | inside the owner's retained sidecar scene/prefab, under a `J2GeneratedRealization` root | committed with the sidecar | Deterministic from their inputs, detectably stale, never source authority. |
| Generated bakes | beside the owning sidecar (NavMeshData, lightmaps, probes) | owner's choice | Disposable and rebuildable. |
| H1 projections | `Assets/Arkus/H1/Managed*` | ignored | H1 only. No selected-tool component is authored inside (see `H1_LIFECYCLE_MATRIX.csv`). |

## Units, scale and model defaults (`J2ImportConventions.ApplyModelDefaults`)

- Metres, Y up.
- Every family: no cameras/lights imported; materials `ImportViaMaterialDescription`, which yields URP/Lit under the baseline.
- `QuaterniusCentimetreModel` (Medieval/Nature/Props Source FBX): vertices are centimetres (ART-01 measurement, H2F-01), so `globalScale 0.01` with `useFileScale off`. No animation.
- `HumanoidBaseCharacter` (Base Characters bodies and their derivatives): Humanoid, create-from-this-model, no clips, **explicit mapping** below.
- `UalClipLibrary` (UAL1/UAL2) and `UalRootMotionLibrary` (`_RM`): Humanoid, same mapping, clips imported.
- Textures follow the owner's import (ART). The foundation imposes only sRGB colour for its own generated sky texture.

## Humanoid / retarget rule

Unity's auto-mapping on the Quaternius universal skeleton picks the ground `root` bone as Hips (H2F-01 S07). The shared explicit mapping is `Hips=pelvis`, `Spine=spine_01`, `Chest=spine_02`, `UpperChest=spine_03` (UpperChest only when present).

- `J2ImportConventions.ApplyHumanoid(path, family)`: import once to get the skeleton, pin the four bones, reimport.
- `J2ImportConventions.Verify(bodies, libraries)` fails closed with `J2_IMPORT_NOT_HUMANOID`, `J2_IMPORT_HIPS_NOT_PELVIS`, `J2_IMPORT_AVATAR_INVALID` and `J2_IMPORT_LOOP_FLAG`.
- Retargeting is Unity Humanoid retargeting between the body avatar and the UAL library avatars that share this mapping.

## Clips and root motion

- `loopTime` is set **exactly** on clips whose name ends in `_Loop` (H2F-01 `BASELINE_INTENT` #5).
- **In-place libraries** (`UalClipLibrary`; the default, driven by controller or agent speed):
  - root rotation, height and XZ are baked into the pose (`lockRootRotation`, `lockRootHeightY`, `lockRootPositionXZ` on);
  - `keepOriginalOrientation` is off, which is ART-01's measured fix for a 180° body/root discrepancy on moving humanoids;
  - `keepOriginalPositionY` and `keepOriginalPositionXZ` are on.
- **Root-motion libraries** (`UalRootMotionLibrary`, the `_RM` variants): XZ and rotation stay root motion; only the height is baked.
- `Verify` reports `J2_IMPORT_ROOT_NOT_IN_PLACE`, `J2_IMPORT_ROOT_MOTION_BAKED` and `J2_IMPORT_ROOT_HEIGHT_OR_ORIENTATION`.
- Locomotion controller recipe (01A): a 1D blend on `Speed`, `Idle_Loop` at 0 and `Walk_Loop` at 1, layer 0 IK Pass on.

## Colliders, navigation, LOD

- Traversable realizations are the **only** colliders a NavMeshSurface collects (`PhysicsColliders`). Kerbs carry no collider; retaining walls and junction aprons do.
- Scenic surfaces get `NavMeshModifier` Not Walkable.
- The project agent is radius 0.28, height 1.8, climb 0.30, slope 40°.
- No foundation LOD tooling (LODGroup is native, `NO_ADOPT` in H2F-01). Owners add LODs when profiling asks for it.

## Adapter/preset locations

- Worldbuilding profiles (`J2LinearProfile`, `J2ScatterProfile`) and look presets live with their owner (CITY/ART/fixture). The foundation look preset is in `Assets/Juego2/Foundation/Rendering`.
- Character/camera presets: `Assets/Juego2/Foundation/Presets`.
- Input: `Assets/Juego2/Foundation/Input/J2_Input.inputactions`.

## Generated outputs are never source authority

A `J2GeneratedRealization` subtree, a NavMeshData, a bake, a GC2 `ChangeModel` copy or an importer-embedded material is regenerated from its declared inputs. Hand edits are detectable through output digests, and the fix is to edit the input.

## ART-01 rebase obligations (recorded, not performed here)

ART-01 must run its effective candidate on this baseline (H2F sequencing). The workspace evidence shows exactly where its paused checkpoint `174d05d2` diverges:

1. `Art01Materials.Get/Remap` hard-binds `Shader.Find("Standard")` and reverts converted materials (H2F-01 finding). It must create URP/Lit materials (`J2MaterialMigration.LitShader()`) and pass `J2ShaderAudit`. Evidence: `J2MaterialMigration` migrated all 25 ART materials in the workspace.
2. ART's `Art01ModelPostprocessor` sets `loopTime` on idle/walk/jog clips of its UAL1 copy. That gives 29 `J2_IMPORT_LOOP_FLAG` findings against the foundation's `_Loop` rule (`results/representative.json`); its root settings already match the in-place rule. ART converges to `J2ImportConventions` or drops its UAL rule.
3. `Art01WalkInspector` reads the legacy Input Manager. It is one of the two named `LegacyInputTransitionTools`, which are why `activeInputHandler = Both`. It moves to the Input System / `J2_Input` before the project can switch to Input System only.
