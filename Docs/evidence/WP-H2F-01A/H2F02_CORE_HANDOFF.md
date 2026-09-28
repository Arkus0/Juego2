# H2F-02 handoff — GC2 Core adoption and lifecycle

This handoff applies only if WP-H2F-01A receives independent PASS. H2F-02 owns exact admission, migration of `Unity/ArkusUnity` and H1 lifecycle classification. This file lists what it must freeze, classify and keep replaceable. H2F-02 must not re-derive what C01–C07 established.

## 1. Admission record (exact)

| Item | Value |
|---|---|
| Product and version | Game Creator 2 **Core 2.19.61** only. No separately licensed module. |
| Package | `Game Creator 2.unitypackage`, SHA-256 `1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b`, 28,236,878 bytes |
| License | Unity Asset Store EULA, per seat, not redistributable. Record the seat/redistribution terms (H2F-01 policy question 5). |
| Provisioning | Assets-only import through `Docs/evidence/WP-H2F-01/spike_project/import_unitypackage.py` (never the vendor `Packages/manifest.json`), with a hash gate as in `probe_project/bootstrap_01a.py core` |
| Excluded from install | The nested installer packages in `Packages/Core/Examples/` (Blockout, Characters, Examples), Hub content and network installs. Decide whether to strip `Packages/Core/Tests`. |
| Manifest additions | `com.unity.modules.physics2d` 1.0.0 (required) |
| Transitive dependencies to pin or assert | `com.unity.collections` (2.6.8 in the probe lock, via URP core), `com.unity.mathematics` (1.3.3). Core's runtime assembly references both. Today they are present only transitively. |
| Already in the selected manifest | Input System 1.20.0, uGUI 2.0.0 (TextMeshPro), AI Navigation 2.0.15, Terrain/Physics/Animation/Audio modules |
| Build check | `GameCreator.Runtime.Core` references `UnityEditor.CoreModule` behind `UNITY_EDITOR`. H2F-02/H2F-03 must prove a player build compiles. |

## 2. Retained Core state families to classify under the H1 lifecycle matrix

`CORE_STATE_INVENTORY.md` gives the detail and lifecycle hints.

| Family | Must decide |
|---|---|
| S1 vendor Core bytes | Externally provisioned dependency host; restoration from lawful local copies; hash gate |
| S2 `core.general/variables/updates/welcome` settings | Generated at first windowed load. Declare `core.general` as committed project configuration with expected values if Juego2 sets its storage. The others may be regenerated. |
| S3 `Character` serialized configuration (player and NPC presets) | Presentation projection materialized from a Juego2 character preset. Observe/reconcile never reads it back as canonical. |
| S4 model instance created by `ChangeModel` (a copy) | Generated from the model prefab; rematerialized |
| S5 runtime-added components (CharacterController, NavMeshAgent, InteractionTracker, kinematic Rigidbody, animator proxy, ragdoll bones) | Transient. Must not be serialized or reported as unsupported drift. |
| S6 Trigger/Hotspot/Actions/Conditions serialized lists, including Juego2 adapter entries | Authored local-execution configuration. Adapter class names are frozen (`[MovedFrom]` on rename). |
| S7 `MaterialSoundsAsset` (Juego2) | Juego2 project asset |
| S8 Core `Skeleton.asset` + recovery clips (ragdoll) | Vendor content referenced by authored configuration |
| Amended camera: GC2 `MainCamera` + `ShotCamera` serialized shot (type, radius, align, Juego2 input bindings) | Presentation projection materialized from the Juego2 camera preset |
| S10 Juego2 input action asset | Juego2-owned input configuration |
| S13 runtime singletons | Transient |

Not retained, and must stay absent from keeper content:

- S11 Variables as facts;
- S12 GC2 save data (the save host is deferred to H6);
- S14 `Remember`;
- GC2 locomotion state assets/clips (S9, redundant with UAL);
- GC2 scene load/unload instructions.

## 3. Public replacement boundary to preserve

- The Juego2 adapter surface is the only bridge between GC2 lists and Arkus. It currently holds:
  - `ConditionArkusFact`;
  - `InstructionArkusRequestTransition`;
  - `EventOnArkusFactChanged`;
  - `GetStringArkusFact`, `GetStringArkusLiteral`, `GetGameObjectArkusEntity`;
  - `ArkusSaveHost`, `Juego2WorkspaceStorage` (for H6 evaluation only).

  It is implemented on public GC2 bases only; its assembly references `GameCreator.Runtime.Core` and the Arkus-side assembly only. H2F-02 moves it to the product location and renames it from the probe namespace, keeping the type names stable from then on.
- Arkus/CITY/story assemblies never reference `GameCreator.*`.
- `ArkusEntityBinding` (or its H2F-02 successor) is the only identity carried by a GC2-driven object.
- The Juego2 input action map remains the only input source: move, look, zoom and interact.

## 4. Lints H2F-02 must add (fail closed)

1. No `GameCreator.*` reference from Arkus/CITY/story contract assemblies.
2. No `Remember` component on keeper objects. No GC2 scene load/unload instruction in keeper content.
3. No GC2 Variable used as, or named as, a Juego2 fact key.
4. Every GC2 `Trigger` whose instruction list contains a Juego2 adapter entry sits on an object with a Juego2 binding key.
5. No stock GC2 instruction directly mutates the canonical properties of an Arkus-bound keeper object. The allowed pattern is: Arkus request → stop if rejected → stock presentation.
6. No enabled GC2 device input on NPC presets. The player/camera input properties reference the Juego2 action asset only.
7. Save storage: never GC2's default PlayerPrefs backend for anything Juego2 persists.

## 5. S06 amendment (if PASS)

Adopt `S06_BASELINE_AMENDMENT.md`:

- player body = GC2 Character;
- player camera = GC2 Main Camera + Third Person shot;
- input = Juego2 action map.

This replaces the H2F-01 minimal controller and the Cinemachine player rig. It does not change any other H2F-01 selection. Decide whether the Cinemachine package stays admitted for non-player roles.

## 6. Residuals carried to H2F-02 or later

| Residual | Owner |
|---|---|
| `FootstepDetectorFulcrum` on UAL `Walk_Loop` fired about half as often as GC2's curve-annotated locomotion (13 vs 24 events over 10 m). Tune the fulcrum per foot or add phase curves to UAL clips. | H2F-02 tuning / H4 |
| Edit-mode `ChangeModel` leaves the previous animator object. The recipe removes it first. | H2F-02 authoring preset |
| `RigLookTo.ClearTargets` skips layer 0; use `RemoveTarget`. Look-at max angle is 90°. | H4 |
| GC2 load reloads scenes by name. Greedy reset re-applies the subscribe-time snapshot. | H6 (save decision) |
| The GC2 editor contacts gamecreator.io (Welcome/version/Hub). | H2F-02 provisioning note |
| Other IK rigs, Props, UI bindings, Console, combat scaffolding, jump/dash, audio manager as pipeline | `DEFER_EVALUATION` in the matrix, each with its owner |
