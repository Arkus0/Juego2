# WP-H2F-01 Worker plan and spike execution contract

Status: **DRAFT + ACTIVE**. Worker: Claude. Baseline `main` `a47f879c46abce88ee23d5959736e32ba5bd62be`. Mode: HYBRID. Real Unity evidence is produced on the owner's workstation (Windows 11, Unity 6000.3.24f1 `4e7b9b5b6244`).

## Isolation boundary

- **Spike project:** `C:\Juego2-H2F01-Spike`, a disposable Unity project **outside the repository**. It is never committed. Every spike runs there.
- **Rebuild recipe:** `spike_project/` in this evidence folder holds the package manifest, spike Editor scripts and a bootstrap script. The bootstrap creates the project from this recipe, copies the pinned inputs and runs the spikes, so anyone can regenerate the project from the repository plus the owner's vault.
- **Not touched:** `Unity/ArkusUnity`, CITY sources/keeper geometry, ART-01's branch/worktree and the owner's vault (read-only).
- **Residue:** when the WP closes, the spike project is deleted or left outside the repository. Nothing of it enters `main` except the recipe and the evidence.

## Representative inputs (pinned, read-only)

1. The ART-01 structural checkpoint at `174d05d23c3bceb9d5df00e460b33519cf68328e`: `Unity/ArkusUnity/Assets/Arkus/ART/**` (builder, derived meshes, materials, audits), `Docs/evidence/WP-ART-01/SOURCE_LOCK.json`, `Tools/art01_import_sources.py`, and the read-only `City04Layout.json` width source. Rebuilt in the spike project, it must reproduce structural digest `a7f8534f33a46a189c5afb726125b01b6ab0eeba5907cbc3ba4aaafb8e30601e`. A mismatch stops the consumption (see the predecessor check).
2. Owner vault `C:\Juego2-Assets` (read-only): the Medieval Village MegaKit Source URP project archive (H1-04 pin `b9d757dd…8b10`), UAL1/UAL2 Source, Universal Base Characters Source, Stylized Nature and Props packs. Every additional file used beyond the ART-01 lock is hash-pinned in `SPIKE_INPUT_LOCK.json`.

## Spike queue (from the H2F-00 register) and the experiment for each

| Spike | Experiment on the representative input | Discriminator / evidence |
|---|---|---|
| S01 render | Rebuild the ART-01 benchmark under URP 17.3 (6000.3 core). Compare two routes: (a) the Built-in→URP material converter on the ART-01 Standard materials, and (b) the Quaternius Source URP-native materials/shaders from the archive on the same pieces. Add an overcast light/Volume baseline (tonemapping, color, SSAO), one damp decal and probes. Views: facade, roof, stone, clothed avatar, Bar F01 threshold. | A shader/material audit with zero unsupported/error shaders and no silent fallback. Captures next to ART-01's Built-in captures. The native lighting suite is judged sufficient or not. Owner visual verdict. |
| S02 topography/linear/nav | For the W12 slope, S02 bridgehead junction, kerb/retaining edge, fence and Bar threshold, compare the ART-01 authored-mesh realization with Terrain + Unity Splines profile extrusion. Bake an AI Navigation NavMeshSurface from bridge to Bar F01 interior. | Route width equals CITY-04 classes, colliders are joined, the NavMesh path is complete, and a nav link exists at the threshold. Record authoring cost, what state is retained/generated, and whether it can be exported/replaced. EasyRoads is screened only if a critical gap remains. |
| S03 river | One riverbank segment using a project-owned URP water shader (depth fade, shoreline, ripple) on the ART bank meshes, seen from the third-person angle. | Readable flow and bank seam in the wet-dark palette, with no plugin-only state. Paid Stylized Water 3 closes as DEFER if the free route suffices. Owner visual verdict. |
| S04 nature | Deterministic seeded scatter of the Quaternius Nature pieces on the bank/slope, with no-go masks for road, threshold, wall and river. Compare against native Terrain trees/details. | Two runs give an identical placement digest, zero placements violate a mask, and the rebuild route is explicit. Owner visual verdict. |
| S05 architecture/window | Bar F01 exterior → threshold → interior (real), plus a neighbouring non-enterable window rendered three ways: flat, shallow authored recess, and a URP interior-mapping shader. | No sticker box and no fake playable entrance. Owner picks the window route or rejects fake interiors. |
| S06 control/camera | A minimal project-owned CharacterController + Input System + Cinemachine 3 route versus Unity Starter Assets ThirdPerson (acquired by the owner). Walk approach → W12 climb → door → inside. | Slope/step/narrow-lane framing without clipping or jerks, from scripted-walk captures plus owner play. Starter Assets is kept only if it measurably saves wiring. |
| S07 humanoid | Clothed ART character (Base Characters derivative) with Humanoid Avatar on UAL1: idle, walk, sit and conversation/contact. Test root-motion variants. Fill named missing civilian clips from UAL2 (owner-owned) before Mixamo. | Feet/hands/proportions/transitions read at human scale. Rig and clip lineage are recorded. Animation Rigging is tried on one contact only if needed. Owner visual verdict. |

Spike families 9 (fog/weather) and 10 (performance helpers) have no surviving external candidate in H2F-00. They are closed by the native baseline observed in S01/S04 and are not given separate spikes.

## Outputs

`SPIKE_RESULTS.md` (per spike: inputs, method, measurements, captures, verdict), `DISPOSITION_MATRIX.csv` (final dispositions over the H2F-00 register), `BASELINE_INTENT.md` (for H2F-02), `SPIKE_LEDGER.md` (project/packages/changes and residue isolation), `ACQUISITION_AND_POLICY.md` (manual inputs, blockers vs failures, dependency-policy questions), `OWNER_JUDGEMENT.md`, `captures/`, `SPIKE_INPUT_LOCK.json`, and the exact-SHA verifier `scripts/h2f01-verify-exact-sha.sh`.

## Freeze preconditions

These are required in addition to the normal protocol: the ART sequencing amendment is merged on `main`; ART-01 `PREFOUNDATION_INPUT` is published; H2F-00 DocSync (#253) is on `main`; owner judgements are recorded for S01, S03, S04, S05, S06 and S07.
