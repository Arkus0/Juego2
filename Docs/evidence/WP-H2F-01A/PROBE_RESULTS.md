# C01–C07 probe results

All probes ran on 2026-09-27 on the owner's workstation:

- Windows 11, Unity 6000.3.24f1, URP 17.3.0;
- the selected H2F-01 stack;
- GC2 Core 2.19.61 imported Assets-only.

Every run used a disposable, non-keeper workspace built by `probe_project/bootstrap_01a.py`. The authoritative run is **workspace B**, rebuilt from an empty directory. Workspace A (exploration) produced identical values for every shared metric; the C06 and C07 Arkus digests are byte-identical across A and B.

Play-mode probes run in a windowed editor, because this license lacks a headless entitlement (H2F-01 finding). A scripted driver measures each run, captures it and exits the editor. The raw outputs are in `results/`:

- `c0N_result.json` and `c0N_log.txt`;
- `c0N_authoring_paths.txt`: every serialized path written;
- `c0N_shader_audit.json`: URP material audit of the probe scene. All are GREEN, with no magenta or unsupported material.

Every C02–C07 run ended with `console_errors = 0`.

Representative content only: the ART-01 structural route (digest `a7f8534f…` reproduced GREEN), the ART clothed citizen (Universal Base Characters derivative with the H2F-01 explicit avatar mapping), UAL1 clips, Quaternius Medieval/Props models (door, chair, mug, crate, barrel) and ART URP materials. No GC2 example or demo content was installed.

## C01 — exact Core surface and state inventory

**Result: DONE.** See `CORE_SURFACE_INVENTORY.md`, `CORE_STATE_INVENTORY.md`, `CORE_VERSION_AND_PROVISIONING.md`, and `results/c01_*` (batch and windowed runs).

- Version and assemblies:
  - Core **2.19.61**, exact package hash;
  - three Core assemblies only.
- Extension families:
  - 28/28 found, all public and implementable from outside GC2;
  - 1,123 family memberships (1,120 distinct stock types) listed in `results/c01_core_extension_types.tsv`.
- Code that runs automatically: 52 auto hooks.
- Settings assets GC2 creates:
  - four `core.*.asset` files, created only at the first **windowed** editor session;
  - the batch run had 0, the windowed run 4.
- Project-global state:
  - the manifest's `physics2d` line is the only GC2-route change;
  - no tags, layers or defines are added.

## C02 — character composition and S06 amendment facts (ART route, bridge → Bar F01 interior)

A GC2 **Player** and a GC2 **NPC** were created through GC2's own editor menus. Each had its model swapped to the ART citizen through the public `ChangeModel`.

The player body read the Juego2 action map `J2_Input/Player/Move`. It walked H2F-01's S06 route under the same virtual-gamepad driver, and the same measurements were taken. Head occlusion was now computed from the body's real feet (see `PREDECESSOR_CONTRACT_CHECK.md` for the H2F-01 artifact).

Two runs differ **only** in the camera. A rendered body-visibility probe was added: the share of screen pixels that change when the body renderers are hidden, sampled every 0.5 s over 80 samples. A sample counts as "lost" when the share is below 0.2 %.

| Measure | H2F-01 minimal + Cinemachine (H2F-01 record) | C02: GC2 body + J2 input + **Cinemachine** | C02g: GC2 body + J2 input + **GC2 Third Person shot** (amended) |
|---|---|---|---|
| Arrived / time | yes / 40.4 s | yes / 40.5 s | yes / 40.5 s |
| Camera clip frames | 0 | 0 | 0 |
| Head occluded frames | 0 | 0 | 0 |
| Jerk frames (>12°) / max | 0 / 10° (first-frame settle) | 0 / 0.4° | 0 / 0° |
| Stalls | 0 | 0 | 0 |
| Min camera clearance | 0.192 m | 0.192 m | never within 0.6 m of a collider |
| Body visible share: mean / at arrival (interior) | — | 1.9 % / 0.8 % | **2.5 % / 1.6 %** |
| Body-lost samples | — | 4 / 80 | **3 / 80** |

Other C02 facts, identical in both runs:

- **Composition.** The player is a GC2 Player (`UnitPlayerDirectional`, `UnitDriverController`), and so is the NPC (`UnitDriverNavmesh`).
- **Model.** The model is `J2_Citizen` with a valid Humanoid avatar. The foot bone sits **0.026 m** above the ground, inside H2F-01 S07's accepted −0.02…0.03 m range. The animator controller is the Juego2 UAL locomotion controller.
- **Navigation.** One NPC navigation command (`Motion.MoveToLocation`) arrived in **7.5 s**, 0.27 m from the goal.
- **Ownership.** One GC2 player; one enabled camera; one MainCamera tag; no `PlayerInput`. The enabled input actions are exactly `J2_Input/Player/{Move, Look, Zoom, Interact}`, plus Unity's URP debug-menu actions.
- **NPC input rule.** Without the NPC preset rule and the camera bindings, GC2 enabled loose `Primary Motion`, `Secondary Motion` and `Zoom` device actions. That was observed in workspace A before the rule was applied.
- **Identity.** No GC2 identity field in the scene; Juego2 keys unique.

## C03 — interaction / affordance execution

Three non-keeper props, all bound to Juego2 keys:

- door `j2.door.probe`;
- chair `j2.chair.probe`;
- mug `j2.mug.probe`, on a crate.

A fourth prop is **unbound**: a barrel with the same GC2 Trigger and adapter instruction but no Juego2 key.

Each prop has a GC2 **Trigger (On Interact)**. It runs `InstructionArkusRequestTransition(Self, Target, transition)` and then a stock GC2 `Debug Text` fed by the Juego2 property `GetStringArkusFact`, which stands in for a presentation effect. The door also has a **Hotspot** (radius 2.5 m).

| Step | Result |
|---|---|
| Hotspot at start → near door | inactive → **active** (1 activation) |
| Player walks up; the focus lands on the door; J2 `Interact` (gamepad south) → `player.Interaction.Interact()` | Arkus `door.open` **ACCEPTED**; presentation line logged once |
| Player presses Interact again | Arkus **REJECTED** `PRECONDITION:open`; presentation **not** run (still 1 line) |
| Player focus on the mug → Interact | `item.take` ACCEPTED, holder = `j2.char.player` |
| NPC walks to the chair; its own focus = chair; public `npc.Interaction.Interact()` (the scripted Character path) | `seat.take` ACCEPTED, occupant = `j2.npc.probe_a` |
| NPC tries the mug the player holds | **REJECTED** `PRECONDITION:j2.char.player` |
| Player interacts with the unbound barrel | **REJECTED** `UNBOUND_ENTITY` |

Proven: player focus and interact through the Juego2 input map; NPC focus and interact through the public API; Hotspot activation. Juego2 semantic IDs live outside GC2, Arkus decides, and GC2 only executes and presents. Not claimed: Inventory or persistent world-object semantics.

## C04 — character presentation helpers (Core only, no extra module)

The probe used two GC2 NPCs on two ART floor surfaces: `Cobble` (z < 0) and `WoodDark` (z > 0). One NPC uses the Juego2 UAL controller with the Fulcrum footstep detector. The other uses GC2's `CompleteLocomotion` with the AnimationCurves detector, as a comparison.

| Helper | Result |
|---|---|
| **Gesture**: UAL1 `Interact` through `Gestures.CrossFade` | playing at mid-clip, stopped afterwards; largest bone excursion **0.607 m** |
| **Looping State**: UAL1 `Sitting_Idle_Loop` on layer 1 through `States.SetState` | hips above feet 0.822 → **0.47** m in the state, 0.47 m after a full loop, 0.822 m after `Stop` |
| **Look-at IK**: `RigLookTo` on the Quaternius skeleton, target 45° off the heading | head-to-target angle **45.2° → 3.9°** |
| **Footsteps on two surfaces**: a `MaterialSoundsAsset` keyed on the ART materials' `_BaseMap`, one synthetic clip per surface | 37 step events and 37 clip plays: `j2_step_cobble` = 18 (6 UAL + 12 GC2-loco on cobble), `j2_step_wood` = 19 (7 + 12 on wood). Mapping is exact. |
| Footstep detector coverage | UAL + Fulcrum: 13 events over 10 m; GC2 curve-annotated locomotion: 24. Residual: tune the fulcrum or add phase curves to UAL. |
| **Ragdoll → recovery**: `RagdollDefault` + Core `Skeleton.asset` + Core get-up clips | ragdoll with hips at 0.153 m → recovered (`IsRagdoll = false`), hips 0.898 m, walked 1.31 m afterwards |

## C05 — Arkus ↔ GC2 public scripting seam

Setup: a door `j2.door.c05`, a lamp `j2.lamp.c05` (point light, intensity 0) and a rule object `j2.rule.c05_door_lamp`. The rule has a GC2 Trigger:

- Event: the custom **`EventOnArkusFactChanged("j2.door.c05:state")`**.
- Instruction list:
  1. stock `CheckConditions[` custom **`ConditionArkusFact(door:state == open)`** `]`;
  2. custom **`InstructionArkusRequestTransition(lamp, lamp.on)`**, with the lamp resolved by the custom Property **`GetGameObjectArkusEntity("j2.lamp.c05")`**;
  3. stock **Light Intensity** on the same Juego2 property;
  4. stock **Debug Text** of the custom Property **`GetStringArkusFact("j2.lamp.c05:light")`**.

| Check | Result |
|---|---|
| Condition before any change | false; the fact property reads `closed` |
| Entity property | resolves `J2_Lamp`; an unknown key resolves `null` |
| Rejected Arkus transition (`door.close` on a closed door) | no fact change → Trigger runs **0** times |
| Arkus `door.open` accepted | Trigger runs once → condition true → Arkus `lamp.on` **ACCEPTED** → stock light intensity **5** → debug text `on` (1 line) |
| Arkus `door.close` accepted | Trigger runs (2 total) → condition false → list stops → **no** second `lamp.on` request |

GC2 executes and presents; Arkus remains the semantic authority. All four seam parts are public GC2 bases, and the adapter uses no reflection (structural verifier check).

## C06 — save-host feasibility

GC2's `core.general` storage was set to the Juego2 backend `Juego2WorkspaceStorage` (the public `TDataStorage` extension). A Juego2 `ArkusSaveHost` (public `IGameSave`) was subscribed while Arkus was at a baseline S0. Two Arkus transitions then produced the saved state D1, digest `688d63c9…`.

| Check | Result |
|---|---|
| Stored payload | key `data-0001-juego2.arkus.probe-payload`; schema `juego2.h2f01a.probe-payload@1`, version 1 |
| GC2's own keys in the same save | `data-0000-slots`, `data-0000-volumes`, `data-0001-scenes`, `data-0001-global-name-variables`, `data-0001-global-list-variables` (Variables unused but still saved) |
| Mutate after save, then `Load(1)` | host called twice (reset + load); **digest = D1**, which is not the reset baseline S0 |
| GC2 load side effect | **the scene was reloaded** (the scene-instance marker changed). The GC2 load path owns scene loading. |
| Negative control: stored payload rewritten to version 99, then `Load(1)` | Arkus **refused** (`VERSION:99`); the digest is **not** D1. It equals S0, because GC2's greedy reset first re-applies the snapshot taken at subscribe time. |
| Residue | `PlayerPrefs` holds no key; slot deleted afterwards |

Feasibility is proven for an opaque, versioned Juego2 payload, and authority is not transferred. The GC2 load path has two behaviours: it reloads scenes, and its greedy reset writes through the host. Both conflict with H0/H1 snapshot/replay and materialization authority, so the host is `DEFER_EVALUATION` for H6. It does not replace H0/H1.

## C07 — selected-stack coexistence and authoring handoff

One disposable composition runs the H2F-01 ART route with:

- the **amended** player: GC2 body + J2 input + GC2 Third Person camera;
- a GC2 NPC (NavMesh driver, UAL, footsteps, ragdoll, look-at);
- Arkus-bound chair and mug interactables;
- the C05 door→lamp rule on the bar;
- the C06 save host with the Juego2 storage.

| Check | Result |
|---|---|
| Clean compile/play | yes; 0 console errors; shader audit GREEN |
| Ownership, before and after the run | 1 GC2 player, 1 enabled camera (GC2 `MainCamera` + 1 `ShotCamera`), 0 Cinemachine brains, 0 `PlayerInput`, 1 CharacterController (player). Enabled input = Juego2 actions only (plus Unity debug menu). |
| Identity | 7 Juego2 bindings, keys unique; **no** GC2 identity field in the scene |
| NPC | navigates to its seat; focus = chair → `seat.take` ACCEPTED; UAL gesture; looks at the player; ragdoll → recovered |
| Player route | arrived 40.5 s, 0 clip, 0 occluded, 0 jerk, 0 stalls; body-visible mean 2.5 %, lost 2/80; 62 footstep events |
| Arkus rule at the bar | `door.open` → GC2 rule → `lamp.on` ACCEPTED → lamp intensity 5 |
| Player interaction at the route end | focus = mug → J2 Interact → `item.take` ACCEPTED |
| Save through the GC2 host | the stored payload equals the Arkus snapshot (keys, values, log); slot deleted |

The authoring recipe is `PUBLIC_AUTHORING_SURFACE.md`. The exact paths per run are in `results/c0N_authoring_paths.txt`.

## S06 outcome

S06_OUTCOME: AMENDMENT_PROPOSED

`S06_BASELINE_AMENDMENT.md` proposes the single explicit amendment:

- **player controller** = GC2 Character;
- **player camera** = GC2 Main Camera + Third Person shot;
- **input** unchanged, the Juego2 action map, now also driving camera orbit/zoom.

The owner chose the camera part (`OWNER_JUDGEMENT.md`). The C02g/C07 rows above are the measured basis. `NO_AMENDMENT` does not apply.
