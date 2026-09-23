# WP-H1-02 — foundational proof plan

Status: COMPLETE; LOCAL UNITY ROUND 4 PASS AND REMOTE CLOSEOUT VERIFIED

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

| Proof obligation | Claim / trust-boundary scope | Completeness argument | Positive evidence | Negative control / defect injection | Result | Residual risk |
|---|---|---|---|---|---|---|
| exact editor pin | retained Unity toolchain identity | declared pin and effective Editor API/runtime value must agree | `ProjectVersion.txt`; round-4 executable/product fingerprint; `Application.unityVersion` inventory | corrupt editor pin makes the repository checker RED; a wrong local Editor fails bootstrap | GREEN | future upgrades require a new baseline |
| exact package intent + resolved lock | direct and effective package graph | exact manifest intent is compared with Unity Package Manager output and generated lock | `manifest.json`, `packages-lock.json`, effective inventory | loosen direct version -> RED; missing lock in final mode -> RED | GREEN | upstream registry/cache integrity remains trusted infrastructure |
| Force Text / visible meta | retained project serialization policy | Unity APIs and generated settings independently agree with committed configuration | EditMode assertions, effective inventory, generated ProjectSettings and `.meta` files | effective setting mismatch fails the Unity test/inventory path | GREEN | byte-identical YAML is not claimed |
| caches ignored | repository boundary | tracked-file inventory is independent of Unity cache contents | `.gitignore`, product commit inventory, final clean-tree audit | remove Library ignore -> RED | GREEN | local caches remain disposable |
| H0 remains Unity-free | engine-neutral predecessor boundary | all `src/**` and `tests/**` project files are independently enumerated | static checker plus main safety build/test | inject a Unity reference into an enumerated H0 project -> RED | GREEN | accepted H0 semantics are consumed, not re-proved |
| assembly shape | effective Unity compilation universe | `CompilationPipeline` enumerates effective Editor/Player assemblies rather than trusting asmdef declarations alone | first and second effective inventories | omitted/drifting assembly changes normalized parity and/or required-name checks | GREEN | future assemblies require explicit revalidation |
| canonical batch entry | fixed project + Arkus entry identity | wrapper owns exact project path, action mapping, process exit and non-empty output postcondition | configure/EditMode/batch logs and outputs | remove exact-process wait -> RED; disable output postcondition -> RED | GREEN | H1-03A later owns the public host lifecycle |
| clean second import equivalence | reproducible clean-import claim | a Git archive at the manifest SHA is imported without caches and compared on six normalized effective fields | `effective-inventory.json` == `second-import-inventory.json` for all owned fields | any package/assembly/settings drift makes comparison RED | GREEN | byte identity and cross-OS equality are not claimed |
| dependency/IP | exact adopted tool/package boundary | local resolved package bytes and legal file are observed, not inferred only from manifest prose | `DEPENDENCY_ADOPTION.md`, `PACKAGE_LEGAL_OBSERVATION.md` | missing exact package or all license/notices evidence stops closed | GREEN | release-wide notice bundling remains downstream packaging work |
| exact delegated evidence chain | H1 remote/local integrity | Git ancestry, manifest-only/product/result-only diffs, remote-head anchors and allowlist are independently compared | round-4 handoff/result comments and `LOCAL_EXECUTION_RESULT.md` | self-reference, wrong parent, wrong remote head or extra mutation is rejected | GREEN | Git/GitHub integrity remains trusted base |

## Remote negative controls

`scripts/h1-02-static-check.py --mode remote-prep --self-test` performs temporary-copy defect injection for:

1. editor pin removal/corruption;
2. non-exact package version;
3. Unity dependency inserted into an independently enumerated H0 project;
4. Unity Library ignore removed;
5. exact Unity process wait removed;
6. required output postcondition disabled.

Each must turn the same checker RED for the intended causal reason, then the untouched repository remains GREEN.

## Local evidence result

No remote or plain-.NET result substitutes for the Unity API claims. Round 4
executed the exact local manifest and produced:

- exact effective editor fingerprint;
- generated `ProjectSettings` and package lock as predeclared candidate outputs;
- compile/import success;
- EditMode result;
- effective package/assembly inventory;
- clean second-import inventory equality;
- mutation allowlist audit;
- resolved license/notices observation;
- result summary bound to the H1 remote/local SHA chain.

The verified chain is:

`e9e022f871ba6b7da95f663956b07d0519b99b2d`
→ `27602941ebc98dd2befa4dea7b2354242459d049`
→ `ed7f0d00f4a6d38daee51c6661af50ff2a423cd4`
→ `91e1e42bc02e95952a00af6db549b78a9e94975e`.

The result is externally anchored in PR #152 comment `#5796728071`; every local
mutation was allowlisted and the final candidate descends from the evidence
commit.

## Residual-risk and proof-budget audit

- Editor availability/licensing remains a workstation prerequisite.
- Byte-identical import output across operating systems or future Unity patches
  is outside the claim; normalized effective state is the oracle.
- Rendered/interactive quality, bridge public capabilities, materialization,
  scenes, prefabs, asset catalogues and gameplay remain downstream.
- Release-wide SBOM/notice packaging remains a release owner; this WP records
  the exact adopted build/test package evidence needed by that later work.
- No residual above can falsify the current exact toolchain/project-baseline
  claim inside its trust boundary.
- The two observed wrapper defects produced product/acceptance progress: exact
  GUI-process waiting and a fail-closed required-output postcondition. Proof
  machinery did not expand solely for hypothetical infrastructure behavior.

UNCLASSIFIED_RESIDUALS: 0

PREDECESSOR_REOPEN_TRIGGERED: NO

## Foundational final status

FOUNDATIONAL_PROOF_VERDICT: READY

UNRESOLVED_PROOF_OBLIGATIONS: 0

KNOWN_UNDETECTED_DEFECT_CLASSES: 0

TRUST_BOUNDARY: see this document / Trust boundary

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
