# WP-H1-02 — foundational proof plan

Status: REMOTE_PREP COMPLETE; LOCAL UNITY EVIDENCE PENDING

## Central claim

A clean checkout contains a retained Unity project whose exact Unity 6.3 editor patch, direct package intent, effective resolved package graph, serialization/meta policy, assembly boundary and non-interactive batch entry are reproducible enough for later H1 bridge work without introducing Unity authority into H0.

## Trust boundary

Trusted infrastructure follows `FOUNDATIONAL_PROOF_STANDARD.md`: Git checkout semantics, the pinned Unity Editor behaving according to its documented contract, the operating system/filesystem, Unity Package Manager registry/cache integrity under ordinary documented behavior, PowerShell/Python used as execution helpers, and standard hash primitives.

Inside the claim are repository-owned pins/configuration, the effective editor version, effective resolved package/assembly inventories, project settings, Unity cache exclusion, H0→Unity dependency direction and the canonical batch invocation shape.

Outside the claim are arbitrary Unity/editor corruption, future editor upgrades, byte-identical import artifacts across machines, graphics/render parity, bridge public capabilities, scenes/prefabs/materialization, gameplay, Quaternius assets and H0 semantic re-proof.

## Independently bounded universes / oracles

- H0 project graph universe: every `*.csproj` independently enumerated under `src/**` and `tests/**`, not a Unity-owned registry.
- Direct Unity package intent: exact `Packages/manifest.json` dependency map.
- Effective package universe: Unity Package Manager `Client.List(offlineMode: true, includeIndirectDependencies: true)` plus generated `packages-lock.json`.
- Effective assembly universe: Unity `CompilationPipeline.GetAssemblies(Player|Editor)`, normalized by assembly name and project-relative sources.
- Repository-owned Unity code/meta universe: all `.cs` and `.asmdef` files under the retained project's `Assets/**`.
- Effective editor/settings oracle: Unity APIs in EditMode tests, independent of the repository-side static checker.
- Second-import parity oracle: compare normalized effective package + assembly inventories after deleting generated caches / importing a clean project copy, not Unity YAML bytes.

## Proof obligations

| Obligation | Positive evidence | Causal negative/control | Local requirement |
|---|---|---|---|
| exact editor pin | `ProjectVersion.txt` + effective `Application.unityVersion` | corrupt editor pin -> repository checker RED; wrong local Editor -> bootstrap RED | yes |
| exact package intent + resolved lock | exact manifest + generated lock + Package Manager inventory | loosen package version -> checker RED; remove lock in final state -> checker RED | yes |
| Force Text / visible meta | EditMode API assertions + effective inventory + committed meta files | effective settings assertion fails when setting is not applied | yes |
| caches ignored | `.gitignore` + clean changed-file audit | remove Library ignore -> checker RED | no/yes audit |
| H0 remains Unity-free | independent `src/**` + `tests/**` csproj scan | inject `UnityEngine` reference -> checker RED | no |
| assembly shape | effective CompilationPipeline inventory | omission/divergence detected by inventory comparison | yes |
| canonical batch entry | `scripts/h1-02-unity.ps1` fixes project root + `Arkus.H1.Editor.H1Batch.Run` | wrong editor/version or missing required output arg fails closed | yes |
| clean second import equivalence | normalized inventory A == inventory B | package/assembly drift makes comparison RED | yes |
| dependency/IP | exact adoption record + resolved package license/notices observation | unknown/contradictory terms stop local round | yes |

## Remote negative controls

`scripts/h1-02-static-check.py --mode remote-prep --self-test` performs temporary-copy defect injection for:

1. editor pin removal/corruption;
2. non-exact package version;
3. Unity dependency inserted into an independently enumerated H0 project;
4. Unity Library ignore removed.

Each must turn the same checker RED for the intended causal reason, then the untouched repository remains GREEN.

## Local evidence still required

No remote or plain-.NET result can prove the Unity API claims. The candidate is `READY_FOR_LOCAL_VALIDATION`, not review-ready, until the exact local execution manifest is run against the anchored SHA chain and produces:

- exact effective editor fingerprint;
- generated `ProjectSettings` and package lock as predeclared candidate outputs;
- compile/import success;
- EditMode result;
- effective package/assembly inventory;
- clean second-import inventory equality;
- mutation allowlist audit;
- resolved license/notices observation;
- result summary bound to the H1 remote/local SHA chain.

## Foundational status before local execution

`FOUNDATIONAL_PROOF_VERDICT: NOT_READY`

`UNRESOLVED_PROOF_OBLIGATIONS: 1` — mandatory effective local Unity round.

`KNOWN_UNDETECTED_DEFECT_CLASSES: 0` inside the remote-only subclaim; effective Unity defect classes remain deliberately unresolved until the required local oracle runs.

`TRUST_BOUNDARY: see this document / Trust boundary`

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`
