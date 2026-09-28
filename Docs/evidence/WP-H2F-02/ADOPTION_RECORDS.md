# Adoption records (DEPENDENCY_IP_POLICY)

Every `ADOPT_NOW` material dependency and every `AVAILABLE_ASSET` source selected by H2F-01, plus every dependency required by the accepted H2F-01A Core handoff. Versions are the exact resolved versions in `Unity/ArkusUnity/Packages/packages-lock.json` on Unity **6000.3.24f1**. License facts were read from each package's own `LICENSE.md` in the resolved package cache at that version. The hash prefix is SHA-256 of that file.

## 1. Unity packages (registry, pinned in manifest + lock)

Common record for every row in this table:

- **Source:** Unity package registry `https://packages.unity.com`, resolved by Unity Package Manager.
- **Linkage:** linked as a package. Runtime assemblies ship in players; editor assemblies do not.
- **Replacement boundary:** each package sits behind a Juego2-owned preset, profile or adapter (`ADAPTER_BOUNDARY.md`). Nothing canonical names a package type.
- **Update owner:** a change of version is an H2F amendment. `J2PackageBaseline` fails closed on any drift.
- **Notices:** packages that ship a *Third Party Notices* file keep it in the package; it must be carried into the release notice bundle.
- **Semantic authority:** none. They never define canonical state, capability semantics, transactions, validation identity, provenance or persisted format.

| Package | Exact version | Direct | License at version (LICENSE.md sha256 prefix) | Role | Guarantee it assists | Stays outside its authority |
|---|---|---|---|---|---|---|
| `com.unity.render-pipelines.universal` | 17.3.0 | yes | Unity Companion License (`4be520a425e06c8d`); ships *Third Party Notices.md* | runtime + editor | rendering baseline, SSAO, decals, Volume | ART visual semantics, CITY geometry |
| `com.unity.render-pipelines.core` | 17.3.0 | transitive | Unity Companion License (`39691adc5fdaf614`); ships *THIRD PARTY NOTICES.md* | runtime + editor | SRP core, Volume framework | same |
| `com.unity.render-pipelines.universal-config` | 17.0.3 | transitive | Unity Companion License (`fa2dcbc8ff967f49`) | editor config | URP defaults | same |
| `com.unity.shadergraph` | 17.3.0 | yes | Unity Companion License (`364b0c08b7fe15b7`) | editor (+ generated shaders) | Source URP Shader Graph materials | material semantics (ART) |
| `com.unity.inputsystem` | 1.20.0 | yes | Unity Companion License (`419718e59cd6c1f9`) | runtime + editor | Juego2 action map, the sole input owner | input semantics beyond device reading |
| `com.unity.splines` | 2.9.1 | yes | Unity Companion License (`1de0a79eecd17320`) | runtime + editor | centreline host for linear realization | road/path intent (Arkus/CITY) |
| `com.unity.ai.navigation` | 2.0.15 | yes | Unity Companion License (`2b2113e0522b3eaa`) | runtime + editor | NavMesh bake and agent execution | where/why actors move (Arkus/H3) |
| `com.unity.animation.rigging` | 1.4.1 | yes | Unity Companion License (`3d49595776c6b4cf`) | runtime + editor | contact IK correction | presentation choices (ART) |
| `com.unity.collections` | 2.6.8 | **yes (pinned for GC2 Core)** | Unity Companion License (`4947f647c031b0fe`) | runtime | referenced directly by `GameCreator.Runtime.Core` | — |
| `com.unity.mathematics` | 1.3.3 | **yes (pinned for GC2 Core)** | Unity Companion License (`5b1a26ff2ca6e6e7`) | runtime | Splines, GC2 Core math | — |
| `com.unity.burst` | 1.8.30 | transitive | Unity Companion License (`9bd567d6de754ab7`); ships *Third Party Notices.md* | runtime/editor compiler | Collections/Splines jobs | — |
| `com.unity.ugui` | 2.0.0 | yes | Unity Companion License (`a8e0cae87b2edb8c`) | runtime + editor | uGUI + TextMeshPro, bound by GC2 Core | UI architecture (deferred H2 UI) |
| `com.unity.test-framework` | 1.6.0 | yes | Unity Companion License (`d3b09e9683afbc8d`) | editor/test only | EditMode evidence suites | — |
| `com.unity.test-framework.performance` | 3.5.0 | transitive | Unity Companion License (`202adffac8b9d21d`); ships *Third Party Notices.md* | editor/test only | — | — |
| `com.unity.ext.nunit` | 2.0.5 | transitive | Unity Package Distribution License (`ea14f98ba99afcc0`); ships *Third Party Notices.md* | editor/test only | NUnit | — |
| `com.unity.nuget.mono-cecil` | 1.11.6 | transitive | Unity Companion License (`76101254034fbea9`); ships *Third Party Notices.md* | editor only | — | — |
| `com.unity.searcher` | 4.9.5 | transitive | Unity Companion Package License v1.0 (`73e67aa49be44b2b`) | editor only | Shader Graph UI | — |
| `com.unity.settings-manager` | 2.1.1 | transitive | Unity Companion License (`a3e5f04ac956baf9`) | editor only | — | — |
| `com.unity.modules.*` (ai, animation, audio, imageconversion, imgui, jsonserialize, particlesystem, physics, **physics2d**, screencapture, terrain, terrainphysics, ui, uielements; transitive hierarchycore) | 1.0.0 | yes | part of the Unity Editor under the Unity Terms of Service (built-in) | engine modules | engine features the stack uses; `physics2d` is required by GC2 Core | — |

The Unity Companion License permits use with the Unity engine; it is a Unity-dependent-project license and is not a copyleft obligation on Juego2/Arkus code. The Arkus kernel (`src/`, H0) links none of these packages. `J2AssemblyBoundary` and the accepted H1-02 MSBuild guard keep it engine-neutral.

**Not installed/admitted:** `com.unity.cinemachine`. Its only selected role, the player camera, was displaced by the accepted S06 amendment, and no other role is selected (Timeline/cinematics are `DEFER` in H2F-01). Also not installed: Terrain Tools, ProBuilder, HDRP, Timeline, Addressables, Recorder, Unity AI packages, Starter Assets. `J2PackageBaseline.ForbiddenPackages` fails closed on any of them.

## 2. Game Creator 2 Core (owner-licensed, admitted by H2F-01A)

| Field | Record |
|---|---|
| Identity | Game Creator 2 **Core**, Catsoft Works, Unity Asset Store |
| Exact version | **2.19.61** (`Packages/Core/Editor/Version.txt`) |
| Package | `Game Creator 2.unitypackage`, SHA-256 `1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b`, 28,236,878 bytes |
| License | Unity Asset Store EULA, standard per-seat license. The package carries no separate license file. **Not redistributable:** the bytes are never committed, never uploaded to CI and never packaged as source. Each developer seat needs its own purchase. Built players may ship the compiled runtime as part of the game (normal Asset Store end-product use). |
| Source/acquisition | owner purchase 2026-09-27; lawful local copy in the owner's Asset Store cache |
| Linkage | Assets-only import under `Assets/Plugins/GameCreator` (git-ignored). Runtime assembly `GameCreator.Runtime.Core` ships in players; the editor and test assemblies do not. |
| Admitted scope | Core only: exactly `GameCreator.Runtime.Core`, `GameCreator.Editor.Core` and `GameCreator.Tests.Core`. Any other GC2 assembly is `J2_GC2_MODULE_NOT_ADMITTED`. Inventory, Dialogue, Quests, Behavior, Perception, Melee, Shooter, Stats and every other module stay outside, pending their own adoption WP. |
| Guarantees it assists | local character execution (player/NPC bodies), player camera shot, interaction/visual-scripting execution, presentation helpers |
| Outside its authority | canonical identity, facts, causal history, transitions, save/replay, scene loading, input ownership (see `GC2_CORE_ADOPTION.md` and `ARKUS_GC2_AUTHORITY_BOUNDARY.md` from 01A) |
| Conformance boundary | the Juego2 adapter seam (`Juego2.Gc2Adapter` → `Juego2.Arkus.IArkusFactAuthority`), presets, and lints L1–L7 |
| Replacement | Remove Core and the gated adapter assemblies drop out. The Arkus seam, presets, input asset, UAL controller, avatar mapping and AI Navigation are unaffected. The displaced H2F-01 realization is documented in 01A as the fallback. |
| Notices | none bundled. The Asset Store EULA governs. |
| Security/update owner | version changes are an H2F amendment (re-run the 01A serialized-path recipe). The GC2 editor contacts gamecreator.io (Welcome/version/Hub); that is editor-only and not admitted as a dependency. |

## 3. Content sources (`AVAILABLE_ASSET` / adopted content, not runtime dependencies)

These are owner-vault content sources. They are provisioned locally, never committed as raw vendor bytes, and pinned by existing hash locks. H2F-02 adds no new source, version or license.

| Source | Adoption/provenance record (authoritative) | License | H2F-02 use |
|---|---|---|---|
| Quaternius Medieval Village MegaKit **Source** (Unity URP archive) | H1-04 `Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json`; archive SHA-256 `b9d757dd…7c8b10` | CC0 (owned Source) | representative route; Source URP materials |
| Quaternius Universal Animation Library 1 (UAL1) | H1-04 adoption + ART-01 `SOURCE_LOCK.json` | CC0 | Humanoid/UAL rules, locomotion controller |
| Quaternius Universal Animation Library 2 (UAL2, + `_RM`) | H2F-01 `SPIKE_INPUT_LOCK.json`, including the bundled `Animation2/License.txt` hash | CC0 (per bundled license) | named gap-filler library. Admission here is for the rules; the per-clip allowlist stays with its first consumer (H2-03 / GC2-00). |
| Quaternius Universal Base Characters (Regular Male + ART derivative `Townsfolk_Forastero`) | ART-01 `SOURCE_LOCK.json` at `174d05d2` | CC0 | Humanoid mapping, S06 preset evidence |
| Quaternius Stylized Nature, Fantasy Props | ART-01 `SOURCE_LOCK.json` | CC0 | scatter evidence |

Derivatives stay traceable through ART's `Derived/` lineage and deterministic `.meta` GUIDs (`sha256("juego2-art01-external:<rel>")[:32]`).

## 4. Native and tool capabilities (no extra dependency)

| Capability | Record |
|---|---|
| Unity Terrain (built-in module) | scenic ground only. Trees/details are not selected (`H1_LIFECYCLE_MATRIX.csv` `terrain.vegetation`). |
| Lightmaps / probes / shadows (engine) | realtime baseline. Bakes are generated state. APV is off (`URP_BASELINE.md`). |
| Humanoid / Animator / root motion (engine) | explicit mapping rule (`IMPORT_CONVENTIONS.md`) |
| Blender 5.2 (DCC, external tool) | used by ART for derivatives. It runs as a separate application; nothing in the project links it. GPL tool isolation: its outputs are Juego2/CC0-derived assets, not GPL code. |
| Juego2-owned tools | water and interior-window shaders; linear/junction/scatter realizers; presets; GC2 adapter. Juego2 source. Tests in `Assets/Juego2/**/Tests`. |
