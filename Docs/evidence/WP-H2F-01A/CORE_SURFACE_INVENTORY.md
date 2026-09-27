# C01 — exact Core surface inventory (GC2 Core 2.19.61)

Machine inventory: `results/c01_core_surface_windowed.json` (full) and `results/c01_core_extension_types.tsv`. The TSV lists every concrete vendor type per extension family, with its public flag, `[Category]` path and assembly.

The inventory was produced by `probe_project/Assets/H2F01A/Editor/H2F01ACoreInventory.cs` inside the installed project:

- Reflection was used for **discovery only**. It enumerated the Core assemblies, public namespaces, MonoBehaviours, ScriptableObjects, auto-run hooks, and the concrete subclasses of every extension base.
- For each base it checked two things: whether the base is public, and whether any of its abstract members is `internal` (and so cannot be implemented outside GC2).

No adapter or probe reaches private GC2 members. The verifier checks this structurally.

## Assemblies and size

| Assembly | Types (public) | Notes |
|---|---|---|
| `GameCreator.Runtime.Core` | 2,473 (2,178) | One runtime assembly holds Characters, Cameras, Common (input, audio, save, pool, managers, UI binding), Variables, VisualScripting and Console. |
| `GameCreator.Editor.Core` | 622 (449) | Inspectors/pickers, creation menus (37 menu items across the assemblies), Hub/Installs/Welcome/Updates, variables editors. |
| `GameCreator.Tests.Core` | 21 (6) | `UNITY_INCLUDE_TESTS`, `autoReferenced: false`. |

Source files: 2,198 runtime and 489 editor `.cs` files. The package ships source, so there is no binary-only lock-in. Public namespaces (27) and type counts are listed in the inventory JSON (`namespaces`).

## Extension families (all 28 found; all public; none has an `internal abstract` member)

| Family | Base | Stock concrete types | Used by a probe through the public base |
|---|---|---:|---|
| visual-scripting.instruction | `Instruction` | 317 | C03/C05/C07 (`InstructionArkusRequestTransition`) |
| visual-scripting.condition | `Condition` | 93 | C05/C07 (`ConditionArkusFact`) |
| visual-scripting.event | `Event` | 67 | C05/C07 (`EventOnArkusFactChanged`) |
| visual-scripting.hotspot-spot | `Spot` | 8 | Hotspot activation in C03 |
| property.get | `TPropertyTypeGet<T>` | 402 | C03/C05 (`GetStringArkusFact`, `GetGameObjectArkusEntity`, `GetStringArkusLiteral`) |
| property.set | `TPropertyTypeSet<T>` | 103 | — |
| save.storage | `TDataStorage` | 2 | C06 (`Juego2WorkspaceStorage`) |
| save.encryption | `TDataEncryption` | 3 | — |
| save.game-save-host | `IGameSave` | 8 | C06/C07 (`ArkusSaveHost`) |
| save.remember-memory | `Memory` | 11 | — (rejected, see matrix) |
| character.unit.player-input | `TUnitPlayer` | 4 | C02/C07 (stock `UnitPlayerDirectional` reading the J2 action) |
| character.unit.motion | `TUnitMotion` | 1 | C02–C07 |
| character.unit.driver | `TUnitDriver` | 3 | C02/C07 (`UnitDriverController` player, `UnitDriverNavmesh` NPC) |
| character.unit.facing | `TUnitFacing` | 8 | C02–C07 (default pivot facing) |
| character.unit.animim | `TUnitAnimim` | 1 | C02–C07 (Kinematic animim driving the Juego2 UAL controller) |
| character.ik-rig | `IK.TRig` | 7 | C04/C07 (`RigLookTo`) |
| character.ragdoll | `TRagdollSystem` | 2 | C04/C07 (`RagdollDefault`) |
| character.footstep-detector | `FootstepDetectorBase` | 2 | C04/C07 (Fulcrum, AnimationCurves) |
| character.interaction-mode | `TInteractionMode` | 3 | C03/C07 (default near-character) |
| character.interactive | `IInteractive` | 1 | C03/C07 (`InteractionTracker` via Trigger On Interact) |
| character.animation-state-asset | `State` | 3 | — (GC2 locomotion state assets are redundant with UAL; see matrix) |
| camera.shot-type | `TShotType` | 8 | C02g (Third Person shot, comparison only) |
| camera.component | `TCamera` | 1 | C02g (`MainCamera`, comparison only) |
| input.button | `TInputButton` | 26 | J2 action map drives Interact through Juego2 glue; GC2 InputAction button types exist |
| input.value | `TInputValue<T>` | 22 | C02/C07 (`InputValueVector2InputAction` bound to `J2_Input/Player/Move`) |
| navigation.marker-type | `TMarkerType` | 2 | — |
| variables.value-type | `TVariable` | 2 | C06 observes GC2's own variable keys in every save |
| runtime.manager-singleton | `Singleton<T>` | 13 | Auto-created during C02–C07 |

## Components an author can place (43 public MonoBehaviours)

Characters and cameras: `Character`, `DriverControllerComponent`, `InteractionTracker`, `MainCamera`, `ShotCamera`.

Visual scripting: `Trigger`, `Actions`, `Conditions`, `Hotspot`, `CopyRunnerInstructionList`, `CopyRunnerConditionList`, `RunnerInstructionsList`, `RunnerConditionsList`.

Variables: `LocalNameVariables`, `LocalListVariables`, `GlobalNameVariablesManager`, `GlobalListVariablesManager`.

Save: `Remember`, `SaveLoadManager`.

Runtime managers: `ApplicationManager`, `AsyncManager`, `AudioManager`, `EventSystemManager`, `InputManager`, `PoolManager`, `RoomManager`, `ScheduleManager`, `TimeManager`, `UpdateManager`, `Marker`, `StagingGizmos`.

uGUI bindings: `ButtonInstructions`, `TextPropertyString`, `SliderPropertyFloat`, `TogglePropertyBool`, `InputField(TMP)PropertyString`, `Dropdown(TMP)PropertyInteger`, `EventCallback`.

Mobile: `TouchStick`, `TouchStickLeft`, `TouchStickRight`.

## Assets an author can create (15 ScriptableObject types)

`Skeleton`, `StateAnimation`, `StateBasicLocomotion`, `StateCompleteLocomotion`, `MaterialSoundsAsset`, `GlobalNameVariables`, `GlobalListVariables`, `Handle`, `TouchStickSkin`, `GeneralSettings`, `VariablesSettings`, `UpdatesSettings`, `WelcomeSettings`, `Installer`, `SkeletonConfigurationStage`.

## Code that runs without being asked (52 hooks)

- **Editor load** (`InitializeOnLoad[Method]`):
  - the settings registry that creates `Data/Resources/Settings/core.*.asset`;
  - the Welcome window and version notifications (network: gamecreator.io);
  - the global-variables postprocessor;
  - per-variable-type editor init;
  - input processor registration.
- **Play-mode start** (`RuntimeInitializeOnLoadMethod`):
  - `ApplicationManager`, `AudioManager`, `InputManager`, `EventSystemManager`, `SaveLoadManager`;
  - global variables managers;
  - spatial hashes (characters, interactions, markers);
  - the runtime `Console`;
  - `Props` and `Runner` hooks.

These create hidden runtime singletons in every play session, whether or not a scene uses GC2. C07 shows that they coexist cleanly with the selected stack: 0 console errors, one camera, one Cinemachine brain, and no `PlayerInput`.

## Material facts discovered by inventory and probes

1. **Everything needed is public.** Every extension base used by the Juego2 seam is public and implementable outside GC2. The adapter needs no reflection and no subclass of an internal type.
2. **Core is a single runtime assembly.** Characters cannot be admitted without also compiling Variables, SaveLoad/Remember, VisualScripting, Console and the managers. They cannot be removed without editing vendor code. This is why the authority boundary is enforced by Juego2 usage rules plus a lint (H2F-02), not by exclusion.
3. **Serialized authoring works; some runtime APIs do not.** Stock GC2 item configuration (fields of stock Instructions/Conditions, character kernel units, footsteps, ragdoll, camera shots, save storage) is private `[SerializeField]`/`[SerializeReference]`. It is reachable through Unity serialization, the same surface the Inspector writes. `PUBLIC_AUTHORING_SURFACE.md` records the exact paths used. Runtime-only public APIs (`Trigger.Reconfigure`, `Character.ChangeModel`, `Motion.MoveToLocation`, `Gestures.CrossFade`, `States.SetState`, `IK.RequireRig<T>`, `Ragdoll.StartRagdoll/StartRecover`, `SaveLoadManager.Subscribe/Save/Load/Delete`) cover the dynamic operations.
4. **Network-capable editor surfaces are present** (Hub, Installs of nested example packages, Welcome, version check). They are not admitted (`editor.hub_installs_updates`).
