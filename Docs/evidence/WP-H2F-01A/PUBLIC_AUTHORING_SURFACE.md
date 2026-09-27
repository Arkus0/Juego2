# Public authoring surface (for H2F-02, H2F-03 and Astra)

This recipe authors GC2 Core state on GC2 Core **2.19.61** without touching private members. Every path below was written by the C02–C07 builders (`probe_project/Assets/H2F01A/Editor/H2F01ABuilder.cs`). The exact list per run is in `results/c0N_authoring_paths.txt`.

There are three kinds of surface:

- **(P)** public C# API;
- **(M)** GC2's own public editor entry points (menus);
- **(S)** Unity serialization paths (`SerializedObject`/`SerializedProperty`), the same surface the Inspector writes.

The (S) field names are GC2's serialized names for this exact version. H2F-02 freezes them per version, and a version change must re-verify them. The builders fail closed with `H2F01A_AUTHORING_PATH_MISSING` if a path disappears.

## Characters

| Step | Surface | Recipe |
|---|---|---|
| Create a player/NPC body | (M) | `CharacterEditor.CreatePlayer(null)` / `CharacterEditor.CreateCharacter(null)` (`GameObject/Game Creator/Characters/*`). The new object is `Selection.activeGameObject`. |
| Size to the project agent | (P) | `character.Motion.Height = 1.8f; character.Motion.Radius = 0.28f; character.Motion.LinearSpeed = 1.45f` (civilian walk; NPC 1.2) |
| Swap in the ART model | (P) | Destroy the previous `character.Animim.Animator.gameObject` (edit-mode `ChangeModel` only calls runtime `Destroy`, H2F-01 finding). Then `character.ChangeModel(J2_Citizen.prefab, new Character.ChangeOptions { controller = J2_UAL_Locomotion })`. `J2_Citizen` is the ART clothed citizen with URP ART materials. Its avatar carries the H2F-01 explicit mapping. |
| UAL locomotion controller | Unity | A 1D blend tree on the float `Speed`: `Idle_Loop` at 0 and `Walk_Loop` at 1. GC2 writes a normalized speed. Layer 0 **IK Pass on**. |
| Player move input = Juego2 action map | (S) | `m_Kernel.m_Player.m_InputMove.m_Input = new InputValueVector2InputAction()`; `…m_Input.m_Input.m_InputAsset = J2_Input`; `…m_ActionMap = "Player"`; `…m_Action = "Move"`. Juego2 enables the asset at runtime. `J2_Input/Player` holds `Move`, `Look`, `Zoom` and `Interact`. |
| NPC navigation executor | (S) | `m_Kernel.m_Driver = new UnitDriverNavmesh()`. Uses the AI Navigation NavMesh of the scene. |
| NPC dormant input (preset rule) | (S) | `m_Kernel.m_Player.m_InputMove.m_Input = new InputValueVector2None()`, so no loose GC2 device action is enabled on NPCs. Applied to every probe NPC. Without it, C02 observed an enabled `<loose>//Primary Motion` action. |
| Footsteps | (S) | `m_Footsteps.m_FootstepSounds.m_SoundsAsset = J2_Footsteps`; `m_Footsteps.m_FootstepDetector = new FootstepDetectorFulcrum()` (UAL) or `FootstepDetectorAnimationCurves` (curve-annotated clips) |
| Footstep sound mapping asset | (S) | Create a `MaterialSoundsAsset` with `m_TextureName = "_BaseMap"`. Resize `m_MaterialSounds.m_MaterialSounds` to one entry per surface, each `MaterialSoundTexture.Create()`. Fill each entry's `m_Name`, `m_Texture` (the ART material `_BaseMap`), `m_Volume` and `m_Variations[0]`. |
| Ragdoll | (S) | `m_Ragdoll.m_Ragdoll = new RagdollDefault()`; `…m_BoneRack.m_Skeleton = Core Skeleton.asset`; `…m_RecoverFaceDown/Up = Core Human@Action_StandFace{Down,Up}` |
| Identity | (P) Juego2 | Add `ArkusEntityBinding { entityKey = "j2.…", kind = … }`. This is the **only** identity carried by the object. |

Runtime operations (P):

- `Motion.MoveToLocation(new Location(point), stop, onFinish)`;
- `Interaction.Interact()`, `Interaction.Target`, `Interaction.EventFocus`;
- `Gestures.CrossFade(clip, mask, BlendMode.Blend, new ConfigGesture(delay, duration, speed, rootMotion, in, out), stopPrevious)`;
- `States.SetState(clip, mask, layer, BlendMode.Blend, new ConfigState(delay, speed, weight, in, out))` and `States.Stop(layer, delay, out)`;
- `IK.RequireRig<RigLookTo>().SetTarget(new LookToTransform(0, target, offset))` and `RemoveTarget(sameStruct)`. `ClearTargets` skips layer 0.
- `Ragdoll.StartRagdoll()` and `Ragdoll.StartRecover()`.

## Camera (S06 amendment: GC2 camera, Juego2 input)

| Step | Surface | Recipe |
|---|---|---|
| Render camera | Unity + (P) | A URP `Camera` tagged `MainCamera` (near 0.05, FOV 55, post-processing on) with GC2 `MainCamera` added |
| Shot | (M) + (S) | `ShotCameraEditor.CreateElement(null)`; then `m_ShotType = new ShotTypeThirdPerson()`; `m_ShotType.m_ThirdPerson.m_Align.m_AutoAlign = true`; `…m_Align.m_Delay = 0.5`; `…m_Align.m_SmoothTime = 1.0`; `m_ShotType.m_ThirdPerson.m_Radius.m_Property = new GetDecimalDecimal(3.0f)`. The default pivot is the GC2 Player. |
| Orbit input | (S) | `m_ShotType.m_ThirdPerson.m_InputRotate.m_Input = new InputValueVector2InputAction()` → `J2_Input` / `Player` / `Look` |
| Zoom input | (S) | `m_ShotType.m_Zoom.m_InputZoom.m_Input = new InputValueVector2InputAction()` → `J2_Input` / `Player` / `Zoom` |

The GC2 directional player unit resolves camera-relative movement from the MainCamera-tagged camera.

The displaced H2F-01 camera stays documented as the replacement alternative, and it is what C02 measured:

- a Juego2 `J2CameraRig` at `+0.6 m` over the GC2 body origin, with eased yaw;
- on top of it, a Cinemachine 3 `CinemachineCamera` + `CinemachineThirdPersonFollow` with the S06 parameters (distance 3.0, shoulder (0.35, 0.05, 0), arm 0.2, side 0.6, obstacle avoidance radius 0.2).

## Interactables and rules (visual scripting)

| Step | Surface | Recipe |
|---|---|---|
| Interactable | (P) | Add a `Trigger` and a collider. `Trigger.Reconfigure(trigger, new EventCharacterOnInteract(), new InstructionList(new InstructionArkusRequestTransition(GetGameObjectSelf.Create(), GetGameObjectTarget.Create(), "door.open"), <stock presentation instruction>))` |
| Stock presentation fed by Arkus | (S) | For example `m_Instructions.m_Instructions.Array.data[1].m_Message.m_Property = new GetStringArkusFact("j2.door.probe:state")` for `InstructionCommonDebugText`, or `…data[2].m_Light.m_Property = new GetGameObjectArkusEntity("j2.lamp.c05")` for `InstructionLightChangeIntensity`. |
| Rule on an Arkus fact | (P)+(S) | `Trigger.Reconfigure(rule, new EventOnArkusFactChanged("j2.door.c05:state"), new InstructionList(new InstructionLogicCheckConditions(), new InstructionArkusRequestTransition(GetGameObjectArkusEntity.Create("j2.lamp.c05"), GetGameObjectSelf.Create(), "lamp.on"), …))`. Then resize `…data[0].m_Conditions.m_Conditions` to 1 and set `…Array.data[0] = new ConditionArkusFact("j2.door.c05:state", "open")`. |
| Affordance | (S) | Add a `Hotspot` (default target Player, mode InRadius) and set `m_Radius.m_Property = new GetDecimalDecimal(2.5f)`. |

The adapter types show up in the GC2 pickers under **Juego2/…**:

- `ConditionArkusFact` — "Arkus Fact Equals";
- `InstructionArkusRequestTransition` — "Request Arkus Transition";
- `EventOnArkusFactChanged` — "On Arkus Fact Changed";
- `GetStringArkusFact` / `GetStringArkusLiteral` — "Arkus Fact" / "Arkus Key";
- `GetGameObjectArkusEntity` — "Arkus Entity".

A human author can therefore build the same lists in the Inspector.

## Save (evidence only; `DEFER_EVALUATION`)

- Storage backend: set `core.general` at `m_Repository.m_Save.m_Storage = new Juego2WorkspaceStorage()` (S). The asset is generated at the first windowed editor load.
- Host: `SaveLoadManager.Subscribe(new ArkusSaveHost(), priority)`, then `Save/Load/Delete(slot)` (P).
- GC2 load reloads the saved scene **by name**, so the scene must be in `EditorBuildSettings`.

## Astra guidance

- Prefer the public API and GC2 menus.
- Use the (S) paths above only for the listed fields. Never write other private GC2 fields.
- Treat the Juego2 adapter picker entries as the only way a GC2 list touches Arkus.
- Never add `Remember`, scene-load instructions or Variables-as-facts to keeper objects.
- Every keeper-facing object needs an `ArkusEntityBinding` key before its Trigger can act semantically. Unbound objects are rejected (`UNBOUND_ENTITY`, C03).
