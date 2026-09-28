# Reproducible, lawful provisioning

## Input classification

| Input | Class | Restoration |
|---|---|---|
| Unity 6000.3.24f1 (revision 4e7b9b5b6244) | toolchain (manual install via Unity Hub; license per seat) | install the exact editor; `ProjectVersion.txt` pins it |
| UPM packages (URP, Shader Graph, Input System, Splines, AI Navigation, Animation Rigging, uGUI, Collections, Mathematics, Test Framework, modules) | repository / package-manager reproducible | automatic on open from the committed manifest + lock |
| Juego2 foundation code, presets, shaders, input, rendering assets, ProjectSettings | repository reproducible | committed |
| Game Creator 2 Core 2.19.61 | account/licensed manual acquisition (Asset Store, per seat, not redistributable) | owner downloads it through the Unity Asset Store with their seat, then runs `python scripts/h2f02-provision.py gc2` |
| Quaternius Source (Medieval Village MegaKit URP archive, UAL1, UAL2, Base Characters, Nature, Props) | external source with pinned acquisition/provenance (owner vault; CC0) | per owning track: H1-04 `scripts/h1-04-import-source.ps1` / asset vault, ART-01 `SOURCE_LOCK.json` import, H2F-01 `SPIKE_INPUT_LOCK.json`; all hash-checked |
| Humanoid import settings of restored copies, locomotion controllers, generated realizations, bakes, GC2 settings assets, `csc.rsp` | local derivative generated from admitted inputs | `J2ImportConventions.ApplyHumanoid`; realizers; bake; GC2 first windowed load; provisioning script |

## Smallest lawful restoration path

1. Clone the repository. Install Unity 6000.3.24f1.
2. Open `Unity/ArkusUnity`, or run `J2FoundationBatch.Verify` in batch. The pinned packages resolve and URP is active. **Without Core the project compiles:** the `Juego2.Gc2Adapter*` assemblies are excluded by their `JUEGO2_GC2_CORE` define constraint, and `Verify` reports `gc2Core=ABSENT` with 0 findings. Hosted CI runs in exactly this state, and the H1 suites run there.
3. Owner seat only: download *Game Creator 2* (Catsoft Works) in the Asset Store, then run `python scripts/h2f02-provision.py gc2`. It:
   - refuses any package whose SHA-256 is not `1e4f3ba0…2f3380b` or whose size is not 28,236,878 bytes;
   - extracts `Assets/**` only into `Assets/Plugins/GameCreator` (3,630 entries); the vendor `Packages/manifest.json` is never applied;
   - proves no manifest, lock or ProjectSettings byte changed;
   - writes the git-ignored `Assets/csc.rsp` (`-define:JUEGO2_GC2_CORE`) and a receipt.

   `status` reports the state; `remove` returns to the Core-absent state.
4. `J2FoundationBatch.Verify -j2-require-gc2` → 0 findings, exactly the three Core assemblies.
5. Content owners restore their vault sources through their own locks and run `J2ImportConventions.ApplyHumanoid` on Humanoid bodies and UAL libraries.

Never commit or upload vendor bytes to make automation easier. `.gitignore` covers `Assets/Plugins/GameCreator/`, its `.meta`, `Assets/Plugins.meta`, `Assets/csc.rsp` and its `.meta`.

## Proven here

The evidence workspace (`workspace/workspace.py run --sha <candidate>`) rebuilds the project from the candidate's committed bytes only (`git archive`, no Library, no ignored state) and checks every committed byte after each phase (`results/summary.json`):

- **A:** Core absent. Clean import, `Verify` 0 findings, foundation tests green, committed bytes unchanged.
- **B:** after `provision gc2`. `Verify` 0 findings with Core required, all Juego2 tests green, committed bytes unchanged.
- **C:** representative content, lint over saved evidence scenes, Windows player build. Committed bytes unchanged. This includes URP's first-build initialization, which the baseline now commits; before that, a build rewrote 7 files.

## Network and editor notes

- The GC2 editor contacts gamecreator.io (Welcome, version check, Hub). That traffic is editor-only and admits nothing: Hub installs and nested example installers are `REJECT_CAPABILITY`.
- GC2 generates its `core.*` settings assets at the first **windowed** editor session. They are git-ignored and regenerated.
- Batch mode without `-quit` needs a headless entitlement this workstation's license lacks (H2F-01), so play-mode evidence belongs to H2F-03/GC2-00 in a windowed editor.
