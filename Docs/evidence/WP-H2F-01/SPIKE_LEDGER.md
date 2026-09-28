# Spike project/change ledger and residue isolation

## What exists where

| Item | Location | In repository? |
|---|---|---|
| Rebuild recipe (bootstrap, package manifest, ProjectVersion, spike Editor/Runtime scripts, two project-owned shaders, Assets-only package extractor) | `spike_project/` | yes |
| Extra hash-locked vault inputs (Source URP materials folder with original `.meta`, fence/border/stairs models, UAL1_RM, UAL2, UAL2_RM, UAL2 license) | `SPIKE_INPUT_LOCK.json` (120 files, H1-04 archive pin `b9d757dd…8b10`) | lock only; bytes stay in `C:\Juego2-Assets` |
| ART-01 structural input | git `174d05d23c3bceb9d5df00e460b33519cf68328e` (`Unity/ArkusUnity/Assets/Arkus/ART/**`, `SOURCE_LOCK.json`, `KIT_COMPOSITION_MANIFEST.json`, `City04Layout.json`) | referenced by SHA |
| Workspace A (first exploration) | `C:\Juego2-H2F01-Spike` | no; disposable. The owner also opened it in Unity. |
| Workspace B (authoritative run, rebuilt from the recipe) | `C:\Juego2-H2F01-Spike-B` | no; disposable |
| Workspace GC2 (isolated owner-supplied candidate) | `C:\Juego2-H2F01-Spike-GC2` | no; disposable. Holds licensed vendor bytes that must never be committed. |
| Curated evidence | `results/`, `captures/` | yes |

Nothing under `Unity/ArkusUnity`, CITY sources, ART-01's branch/worktree or the vault was modified. The bootstrap refuses a workspace root inside the repository.

## Rebuild

```text
python spike_project/bootstrap.py init --root <empty-dir>      # pinned inputs + recipe
python spike_project/bootstrap.py unity Juego2.H2F01.Editor.H2F01RenderSpike.Reproduce r01 --root <dir>
... CaptureBuiltin, SetupUrp, ConvertRouteA, H2F01AvatarMap.Apply, CaptureRoutes, CaptureH1Slice,
    H2F01WorldSpike.Run, H2F01HumanSpike.Poses, H2F01PlayBuilder.EnableInputSystem,
    H2F01WaterSpike.Run, H2F01WindowSpike.Run, H2F01NatureSpike.Run
python spike_project/bootstrap.py play Juego2.H2F01.Editor.H2F01PlayBuilder.S06Minimal p1 --root <dir>   # windowed
python spike_project/bootstrap.py play Juego2.H2F01.Editor.H2F01PlayBuilder.S07Motion  p2 --root <dir>   # windowed
```

GC2 (owner-licensed, optional):

1. `import_unitypackage.py "<vault>/Game Creator 2.unitypackage" <dir>/Unity/Spike`, which imports `Assets/**` only and excludes `Packages/manifest.json`.
2. Add `com.unity.modules.physics2d` to that workspace's manifest.
3. Copy `spike_project/gc2_only/Assets/H2F01Gc2` into the workspace.
4. Run `play Juego2.H2F01.Gc2.H2F01Gc2Builder.S06Gc2`.

Workspace B was rebuilt from an empty directory with this recipe. It reproduced the ART digest `a7f8534f…` and the same S02 (2.795/5.490 m, 0/1674 holes, 59.0 m path) and S07 (hips 0.90 m, seat anchor 0.048 m) measurements as workspace A.

## Packages exercised (workspace B lock)

URP 17.3.0 / core 17.3.0 / shadergraph 17.3.0, Input System 1.20.0, Cinemachine 3.1.7, Splines 2.9.1, AI Navigation 2.0.15, Animation Rigging 1.4.1, Terrain Tools 5.3.3 (not selected), ProBuilder 6.1.2 (not selected), Test Framework 1.6.0, uGUI 2.0.0. Full list: `results/spike_packages_lock.json`.

## Residue after the WP

The three workspaces are outside the repository and may be deleted at any time. None is an adoption. H2F-02 starts from `Unity/ArkusUnity` plus `BASELINE_INTENT.md`, not from a spike workspace.
