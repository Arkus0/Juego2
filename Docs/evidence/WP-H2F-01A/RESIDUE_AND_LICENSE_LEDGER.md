# Residue and license ledger

## Licensed and vendor bytes

| Item | Where it lives | In repository? |
|---|---|---|
| `Game Creator 2.unitypackage` (Core 2.19.61, SHA-256 `1e4f3ba0…2f3380b`) | owner's Asset Store cache | **no**. Only its hash, size and version are recorded. |
| Imported Core (`Assets/Plugins/GameCreator/**`, 3,630 entries, source + content) | probe workspaces A and B only | **no** |
| Generated `core.*.asset` settings | probe workspaces only | **no** |
| Core content referenced by probes (`Skeleton.asset`, `Human@Action_StandFace*` recovery clips, `CompleteLocomotion.controller` for the comparison walker) | probe workspaces only | **no** |
| Quaternius/ART inputs (ART-01 lock + H2F-01 `SPIKE_INPUT_LOCK.json`) | owner vault → workspaces | **no**. Consumed through the accepted H2F-01 recipe. |

The verifier enforces all of the following:

- no `.unitypackage`, `.fbx`, `.anim`, `.asset`, `.prefab`, `.unity`, `.controller`, `.mat`, `.dll`, `.wav`, `.inputactions` or `.meta` file anywhere under this evidence directory;
- no `namespace GameCreator` declaration in any committed text file, so no vendored GC2 source;
- images only under `captures/`.

The committed probe code is Juego2-owned:

- `Arkus`: stand-in authority;
- `Gc2Adapter`: public-base adapters;
- `Runtime`: probe driver and camera rig;
- `Editor`: inventory and builders.

It *uses* GC2 types and does not contain them.

**License:** Unity Asset Store EULA, a per-seat license. The package is not redistributable, so restoration needs a lawful local copy on each seat. H2F-02 records the seat/redistribution terms at admission. H2F-01 already listed this as policy question 5. No purchase was triggered by this WP.

No separately licensed GC2 module was acquired or used. The installed assemblies are exactly `GameCreator.{Runtime,Editor,Tests}.Core`.

## Disposable workspaces

| Workspace | Purpose | State |
|---|---|---|
| `C:\Juego2-H2F01A-Core` (A) | first exploration and probe development | outside the repository; disposable |
| `C:\Juego2-H2F01A-Core-B` (B) | **authoritative run**, rebuilt from an empty directory with `probe_project/bootstrap_01a.py` (`base` → `core` → `probes` → `collect`) | outside the repository; disposable |

Nothing under `Unity/ArkusUnity`, CITY sources, ART-01's branch/worktree or the owner vault was modified. `bootstrap_01a.py` refuses a workspace root inside the repository.

H2F-01 left three disposable workspaces. `C:\Juego2-H2F01-Spike` and `C:\Juego2-H2F01-Spike-B` were deleted during this WP with explicit owner approval, because the disk was full. Their SPIKE_LEDGER marks them "may be deleted at any time", and H2F-01's evidence is already in the repository. `C:\Juego2-H2F01-Spike-GC2` was kept.

## Residue checks

- **PlayerPrefs / registry:** none. C06 configured GC2 storage to the Juego2 workspace JSON backend, and `PlayerPrefs.HasKey(<payload key>) == false` after the run. With GC2's default `StoragePlayerPrefs`, saves would land in the Windows registry under the project's company/product key. H2F-02 must select the backend explicitly if the host is ever used.
- **GC2 save data:** written by C06 and C07 to `<workspace>/out/c06/gc2_storage.json` (the Juego2 backend path). Slot 1 is deleted at the end of C06/C07 (`has_save_slot1_after_delete=false`). The shared `data-0000-slots` and `data-0000-volumes` keys remain in the workspace file only.
- **Network:** the GC2 editor makes outbound requests to gamecreator.io:
  - the Welcome window images;
  - the version check;
  - the Hub.

  The probes do not depend on them. Unity's own analytics endpoints (`config.uca.cloud.unity3d.com`, `cdp.cloud.unity3d.com`) failed DNS resolution in the logs. That is unrelated to GC2 and harmless.
- **Build settings:** C06 sets the workspace `EditorBuildSettings` to the C06 probe scene, because GC2 load reloads scenes by name. This is workspace-only.
- **Project-global settings:** only the recipe's `physics2d` manifest addition is attributable to the GC2 route. See `CORE_VERSION_AND_PROVISIONING.md` for the editor-session changes.

## Deletion

Both probe workspaces may be deleted at any time; they are rebuilt by the recipe. H2F-02 starts from `Unity/ArkusUnity`, `BASELINE_INTENT.md` (H2F-01) and `H2F02_CORE_HANDOFF.md`, not from a probe workspace.
