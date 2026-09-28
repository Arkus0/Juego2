# GC2 Core adoption — closure of the H2F-01A handoff

Source: `Docs/evidence/WP-H2F-01A/H2F02_CORE_HANDOFF.md` and `S06_BASELINE_AMENDMENT.md` (accepted: PR #256, PASS `#5333319449`). GC2 is part of the selected foundation (the urban amendment in PR #257; the GATE freezes the GC2↔Arkus adapter/lifecycle). WP-GC2-00 builds the real semantic round trip on these surfaces.

## §1 Admission record

| Handoff item | Closure | Evidence |
|---|---|---|
| Core 2.19.61 only, no separately licensed module | provisioned exactly; any other GC2 assembly is `J2_GC2_MODULE_NOT_ADMITTED` | `results/b1_gc2_report.json` (3 Core assemblies), `J2Gc2Provisioning` |
| Package SHA-256 / size | hash + size gate in `scripts/h2f02-provision.py` | `results/b0_gc2_provisioning_receipt.json` |
| License: Asset Store EULA, per seat, not redistributable; seat/redistribution terms (H2F-01 policy question 5) | recorded | `ADOPTION_RECORDS.md` §2 |
| Assets-only provisioning with hash gate, never the vendor manifest | `provision gc2` extracts 3,630 `Assets/**` entries, excludes `Packages/manifest.json`, proves project-global files unchanged | `PROVISIONING.md`, receipt |
| Excluded from install: nested installer packages (Blockout, Characters, Examples), Hub, network installs | the nested `.unitypackage` files are extracted as inert files (vendor layout kept) and never installed; Hub/installs are `REJECT_CAPABILITY` | receipt `excluded`, `PROVISIONING.md` |
| Decide whether to strip `Packages/Core/Tests` | **kept**: vendor layout unchanged; `GameCreator.Tests.Core` is editor/test-only and never ships (player build contains only `GameCreator.Runtime.Core`) | `results/player_build.json` |
| Manifest addition `com.unity.modules.physics2d` 1.0.0 | pinned | manifest, `J2FoundationBaseline.DirectPackages` |
| Transitive `com.unity.collections` / `com.unity.mathematics` | pinned **directly** (2.6.8 / 1.3.3, depth 0 in the lock) | `J2PackageBaseline` |
| Input System 1.20.0, uGUI 2.0.0, AI Navigation 2.0.15, Terrain/Physics/Animation/Audio modules | pinned | manifest |
| Build check (`GameCreator.Runtime.Core` references `UnityEditor.CoreModule` behind `UNITY_EDITOR`) | **Windows player build succeeded**, 0 errors; ships `GameCreator.Runtime.Core`, `Juego2.Gc2Adapter`, `Juego2.Arkus`, `Juego2.Foundation`, `Arkus.H1.Projection.Runtime` | `results/player_build.json` |

## §2 Retained Core state families

Every family S1–S15, plus the amended camera, has a concrete lifecycle class and host in `H1_LIFECYCLE_MATRIX.csv` (`gc2.*` rows). Decisions taken:

- **S2:** Juego2 does **not** override `core.general` (nothing Juego2 persists through GC2 before H6), so all four settings assets are generated and git-ignored.
- **S7:** `MaterialSoundsAsset` is created by the first owner that adopts footsteps (it needs ART textures/audio).
- **S11/S12/S14:** not admitted; lints L3, L7 and L2 enforce this.

## §3 Public replacement boundary

| Item | Closure |
|---|---|
| Adapter surface at product location, renamed from the probe namespace, stable names from now on | `Assets/Juego2/Gc2Adapter/Runtime/ArkusGc2Adapters.cs`, namespace/assembly `Juego2.Gc2Adapter`; the six types keep their names; `[MovedFrom("Juego2.H2F01A.Gc2Adapter")]` keeps 01A-authored lists loadable |
| Adapter references Core runtime + Arkus side only | asmdef references exactly `GameCreator.Runtime.Core` + `Juego2.Arkus`, gated by `JUEGO2_GC2_CORE` (structural check `J2AssemblyBoundary`) |
| Arkus stand-in → product seam | the probe's `ArkusProbeAuthority` is replaced by `Juego2.Arkus.IArkusFactAuthority` + `ArkusFacts` (accepted / refused / conflict + current result, fail-closed without an authority). WP-GC2-00 binds the production host. |
| `ArkusSaveHost`, `Juego2WorkspaceStorage` | not ported (H6 `DEFER_EVALUATION`) |
| Arkus/CITY/story assemblies never reference `GameCreator.*` | lint L1 |
| `ArkusEntityBinding` or successor is the only identity | `Juego2.Arkus.ArkusEntityBinding` (key grammar `j2(.segment)+`; `[ExecuteAlways]` so edit-mode resolution works); identity audit on materialized presets finds no GC2 ID field |
| Juego2 action map is the only input source (move, look, zoom, interact) | `J2_Input` + `J2InputOwner`; lint L6 |

## §4 Lints (fail closed)

| # | Rule | Implementation | Negative control |
|---|---|---|---|
| 1 | No `GameCreator.*` reference from Arkus/CITY/story contract assemblies | `J2AssemblyBoundary` (asmdefs + code tokens; GUID refs rejected) | `J2FoundationBaselineTests.AssemblyBoundary_*` |
| 2 | No `Remember`; no GC2 scene load/unload instruction | `J2Gc2Lint` | `Lint_L2_RememberAndSceneLoadingAreRejected` |
| 3 | No GC2 Variable used/named as a Juego2 fact key | `J2Gc2Lint`: GC2 normalizes names (`j2.door.a:state` → `j2-door-a-state`), so any Variable name in the `j2` + separator namespace is rejected | `Lint_L3_VariableNamedAsJuego2FactIsRejected` |
| 4 | A Trigger with a Juego2 adapter entry sits on a Juego2-bound object | `J2Gc2Lint` | `Lint_L4_AdapterEntryOnUnboundObjectIsRejected` |
| 5 | No stock GC2 instruction directly mutates an Arkus-bound object; allowed pattern is request → stop if refused → stock presentation | `J2Gc2Lint`: Transform/GameObject/Physics/Variables/Storage instructions on a bound object must follow a stopping Arkus request in the same list | `Lint_L5_StockMutationOfBoundObjectNeedsAStoppingArkusRequestFirst` |
| 6 | No enabled GC2 device input on NPCs; player/camera inputs reference the Juego2 asset only | `J2Gc2Lint` | `Lint_L6_DeviceInputOrNonJuego2ActionsAreRejected` |
| 7 | Never GC2's PlayerPrefs backend for anything Juego2 persists | `J2Gc2Lint.CheckSaveImplementations`: no Juego2 `IGameSave`/`TDataStorage` exists before H6 | `Lint_L7_Juego2PersistenceThroughGc2IsRejected` |

Project content lint over `Assets/Juego2`, `Assets/Arkus` and the saved evidence scenes: 0 findings (`results/c2_gc2_lint.json`).

## §5 S06 amendment adopted exactly

| S06 handoff step | Closure |
|---|---|
| 1. Admit Core 2.19.61 as specified | §1 |
| 2. Player preset: GC2 Player 1.8/0.28/1.45, `UnitPlayerDirectional` on `J2_Input/Player/Move`, citizen model with explicit avatar mapping, UAL locomotion controller with IK Pass, Fulcrum footsteps, `ArkusEntityBinding` | `J2_Player` + `J2Gc2Presets.MaterializeCharacter`. The model/controller are content inputs; the workspace materializes the ART citizen `Townsfolk_Forastero` (avatar valid, Hips=pelvis) with the UAL1 Idle/Walk controller. The footstep **sounds** asset needs ART textures and is supplied by its first owner (§2 S7). |
| 3. Camera preset: GC2 `MainCamera` on the URP camera, Third Person `ShotCamera` (radius 3, 0.5/1.0), Look/Zoom on `J2_Input` | `J2_PlayerCamera` + `MaterializePlayerCamera` (`S06Presets_*` test; workspace bindings list) |
| 4. Juego2 action map sole input owner; NPC input `InputValueVector2None` | `J2_Npc_Civilian` + lint L6; workspace: NPC input None, player/camera read `J2_Input/Move`, `Look` and `Zoom` only |
| 5. Do not implement the displaced minimal controller / Cinemachine rig as baseline | Cinemachine is **not installed** and is a forbidden package; no minimal controller in the product project |
| 6. Classify every named state family | `H1_LIFECYCLE_MATRIX.csv` |
| 7. Retain the amendment and its causal evidence in the adoption record | this file + `Docs/evidence/WP-H2F-01A/{S06_BASELINE_AMENDMENT.md, OWNER_JUDGEMENT.md, results/c02_result.json, results/c02g_result.json, results/c07_result.json}` (consumed, unchanged) |

**Cinemachine decision:** not admitted for any role. The player camera was its only selected role; cinematics/Timeline are `DEFER`. A later cinematics WP adopts it explicitly if needed.

## §6 Residuals carried

| Residual | Owner now |
|---|---|
| Fulcrum footstep cadence on UAL walk (13 vs 24 events / 10 m) | H2F-03 tuning / H4 |
| Edit-mode `ChangeModel` leaves the old animator object | **closed in the preset materializer** (removes it first) |
| `RigLookTo.ClearTargets` skips layer 0; look-at max 90° | H4 |
| GC2 load reloads scenes by name; greedy reset | H6 save decision (lints L2/L7 keep it out meanwhile) |
| GC2 editor contacts gamecreator.io | recorded (`PROVISIONING.md`) |
| Other IK rigs, Props, UI bindings, Console, combat, jump/dash, audio manager | `DEFER_EVALUATION` owners per the 01A matrix |
