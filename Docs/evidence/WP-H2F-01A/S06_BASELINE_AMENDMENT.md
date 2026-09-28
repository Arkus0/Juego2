# S06 baseline amendment — player controller and camera realized by GC2 Core

Status: **PROPOSED by WP-H2F-01A**. It becomes binding for H2F-02 only if WP-H2F-01A receives independent PASS. This is the single explicit amendment allowed by the WP section "H2F-01 verdict handling". The owner chose its scope; see `OWNER_JUDGEMENT.md`.

## Displaced H2F-01 realization

From `Docs/evidence/WP-H2F-01/BASELINE_INTENT.md`:

| Role | H2F-01 selection (displaced) | Amended realization |
|---|---|---|
| Player control | CharacterController + Input System action map + project-owned locomotion/camera-root controller (the `H2F01MinimalController` pattern) | **GC2 Core `Character` as the player body.** Kernel units: `UnitPlayerDirectional` (reads `J2_Input/Player/Move`), `UnitMotionController` (height 1.8, radius 0.28, walk 1.45 m/s), `UnitDriverController` (Unity CharacterController), `UnitFacingPivot`, and `UnitAnimimKinematic` driving the Juego2 UAL locomotion controller. |
| Player camera | Cinemachine 3 `CinemachineCamera` + `ThirdPersonFollow` with obstacle avoidance | **GC2 `MainCamera` + `ShotCamera` with the Third Person shot** (radius 3, auto-align delay 0.5 s, smooth 1.0 s). Orbit reads `J2_Input/Player/Look` and zoom reads `J2_Input/Player/Zoom`. |
| Player input | Input System action map | **Unchanged.** The Juego2-owned action map is the only input source. GC2 reads it through its public `InputValueVector2InputAction`. Juego2 creates, binds, enables and disables it. |

Unchanged H2F-01 selections this amendment relies on:

- AI Navigation;
- UAL1/UAL2 with the explicit Humanoid mapping;
- Animation Rigging for contact IK;
- URP and everything else in `BASELINE_INTENT.md`.

## Measured benefit (new evidence not available to H2F-01)

1. **No regression on the accepted S06 route.** Same virtual-gamepad drive, same route and same measurements as H2F-01 S06, with the head point now taken from the body's real feet:

   | Composition | Arrived | Time | Clip frames | Occluded frames | Jerk frames (max) | Stalls | Rendered body visibility: mean share / lost samples / share at arrival |
   |---|---|---|---|---|---|---|---|
   | H2F-01 minimal controller + Cinemachine (H2F-01 S06 record) | yes | 40.4 s | 0 | 0 | 0 (10°) | 0 | not measured in H2F-01 |
   | C02: GC2 body + J2 input + Cinemachine (`results/c02_result.json`) | yes | 40.5 s | 0 | 0 | 0 (0.4°) | 0 | 1.9 % / 4 of 80 / 0.8 % |
   | **C02g: GC2 body + J2 input + GC2 camera, amended (`results/c02g_result.json`)** | yes | 40.5 s | 0 | 0 | 0 (0°) | 0 | **2.5 % / 3 of 80 / 1.6 %** |
   | C07: amended composition + all retained capabilities (`results/c07_result.json`) | yes | 40.5 s | 0 | 0 | 0 (0°) | 0 | 2.5 % / 2 of 80 / 1.6 % |

   The numbers come from the authoritative workspace-B run. H2F-01's 25 "occluded" GC2 frames came from a head point computed from GC2's capsule centre (≈ 2.5 m high). With the corrected feet-based head point both cameras show 0.

2. **One body kernel for player and NPC, with presentation and interaction helpers, and no bespoke code.** With the minimal controller, the player would need its own:
   - interaction focus/interact, which C03 proves through GC2 for the player and for NPCs;
   - surface-aware footsteps (C04/C07);
   - look-at (C04);
   - gesture/state layering (C04);
   - ragdoll/recovery (C04).

   Alternatively Juego2 would run two humanoid stacks (minimal player + GC2 NPCs) for the same ART citizen. C07 shows the amended player carrying footsteps (62 step events on the route) and interacting with Arkus-bound props, beside a GC2 NPC that navigates, interacts, gestures, looks at the player, ragdolls and recovers.
3. **Camera framing at the enterable threshold.** The owner preferred the GC2 shot's framing where the route enters the Bar F01 interior (`captures/owner/S06_amendment_camera_route.png`, final column). This is the H2F-01 residual "minimal third-person camera framing at the F01 lintel". The rendered body-visibility measurement quantifies the difference: the body is on average about 30 % larger on screen (2.5 % vs 1.9 % of pixels), and its visible share on arrival inside the bar is twice as large (1.6 % vs 0.8 %).
4. **H2F-01's reasons for not selecting GC2 are neutralized**, because Core is admitted anyway for NPC bodies, interaction and presentation (see `CORE_CAPABILITY_MATRIX.csv`):
   - the parallel runtime (managers, Variables, SaveLoad) is present regardless;
   - non-redistributable provisioning is incurred regardless;
   - the edit-mode `ChangeModel` caveat has a documented recipe;
   - the camera difference H2F-01 recorded was partly a measurement artifact.

## Authority and lifecycle burden

- **Authority:** unchanged.
  - The GC2 body and camera hold execution/presentation state only. The C02/C07 identity audit found no GC2 ID field.
  - Player identity is the Juego2 key `j2.char.player` (`ArkusEntityBinding`).
  - Every semantic change goes through an Arkus request.
  - Input remains Juego2-owned. With the NPC preset rule and the camera bindings, the enabled input actions are exactly `J2_Input/Player/{Move,Look,Zoom,Interact}`, plus Unity's own URP debug-menu actions. No GC2 device action stays enabled.
- **Lifecycle (new retained state H2F-02 must classify):**
  - S3: the player `Character` serialized kernel/footsteps/IK configuration;
  - S4: the model instance created by `ChangeModel`, which is a copy, not a prefab link;
  - S5: the hidden CharacterController added at play;
  - the GC2 `MainCamera` + `ShotCamera` serialized shot configuration (shot type, radius/align values, Juego2 input bindings);
  - S2: the Core settings assets.

  None of these is canonical. Each is a presentation projection rematerialized from a Juego2 character/camera preset.
- **Replacement cost:**
  - The Juego2 seams do not depend on GC2: input action map, Arkus bindings/adapters, UAL controller, avatar mapping, AI Navigation.
  - Replacing GC2 later would mean restoring the displaced H2F-01 realization. It is still documented (`H2F01MinimalController` + Cinemachine rig, and `probe_project/Assets/H2F01A/Runtime/J2CameraRig.cs`), plus bespoke equivalents of the helpers above.
  - The Cinemachine package is no longer needed for the player camera. H2F-02 decides whether it stays admitted for other roles (for example Timeline cinematics) or is dropped.
- **Worker note:** the Worker recommended a controller-only amendment. The camera part is the owner's feel decision. Its cost is the GC2 camera state above, and a player camera whose orbit/zoom tuning surface is GC2's rather than Cinemachine's.

## H2F-02 handoff

H2F-02 must adopt this realization **instead of** the displaced H2F-01 player controller and player camera:

1. Admit GC2 Core 2.19.61 exactly as specified in `H2F02_CORE_HANDOFF.md`.
2. Build the Juego2 player preset exactly as recorded in `PUBLIC_AUTHORING_SURFACE.md`:
   - GC2 Player with height 1.8, radius 0.28 and walk 1.45 m/s;
   - `UnitPlayerDirectional` bound to `J2_Input/Player/Move`;
   - the `J2_Citizen` model with the explicit avatar mapping;
   - the UAL locomotion controller with IK Pass on;
   - footsteps with the ART `_BaseMap` sound mapping and the Fulcrum detector (tuning residual);
   - `ArkusEntityBinding`.
3. Build the Juego2 camera preset: GC2 `MainCamera` on the MainCamera-tagged URP camera, and a `ShotCamera` Third Person shot targeting the Player (radius 3, auto-align 0.5/1.0), with orbit on `J2_Input/Player/Look` and zoom on `J2_Input/Player/Zoom`.
4. Keep the Juego2 action map as the only input owner. NPC presets set their dormant player-unit input to `InputValueVector2None`.
5. Do **not** implement the displaced `H2F01MinimalController`/Cinemachine player rig as the player baseline.
6. Classify every state family named above under the H1 lifecycle matrix.
7. Retain this amendment and its causal evidence (`results/c02_result.json`, `results/c02g_result.json`, `results/c07_result.json`, `OWNER_JUDGEMENT.md`) in the adoption record.
