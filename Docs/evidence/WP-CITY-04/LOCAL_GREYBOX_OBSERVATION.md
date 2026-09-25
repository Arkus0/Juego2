# CITY-04 local greybox observation — final technical handoff evidence

Status: **TECHNICAL SCENE-HANDOFF PASS EVIDENCE**. Human timing/perceptual verdicts are intentionally deferred to CITY-07 by the accepted demo-validation amendment.

## Input and execution identity

- Canonical PR/branch: `#221`, `worker/city-04`.
- Unity scene/capture material commit observed locally: `22bf4a115070a1045c2a2b180988df7a4877d7f9`.
- Accepted CITY-03 seed source blob: `3186b80d173cd20f961f33d5a28bf447b2c6971d`.
- `City04Layout.json` projection blob: `7b1c83815188f323ae23a02f43aa08c085592f6e`.
- Environment observed: Windows 11 Pro; Unity Editor `6000.3.24f1`; built-in render pipeline; `com.unity.test-framework` 1.6.0; built-in physics 1.0.0.
- Package manifest blob: `19c29d4989182a73f559eefbf131775b99097a07`; lock blob: `f1cd55ead4c8bd5254372af66942e1682c2e3225`.
- Scene blob: `6846ac662c01aa4ed05c6c3b9c2ee1559f9383f3`.

## Executed technical observations

The repository-owned generator regenerated the committed OBJ/C# projections and verified the accepted seed source blob, exact hard polygon, water areas, three dry components and site containment outside effective water/no-build regions.

The pinned editor executed the bounded CITY-04 methods:

1. `City04GreyboxBuilder.Build` → `CITY04_BUILD_GREEN`.
2. `City04GreyboxBuilder.CheckPhysicalCuts` → `CITY04_PHYSICAL_CUTS_GREEN`: collision on Wedge, Orilla-sur, Ensanche and X1; X5 collidable only while available; representative Río/Arroyo cuts non-traversable; X5 non-collidable after closure.
3. `City04GreyboxBuilder.Capture` → `CITY04_CAPTURE_GREEN`, producing four committed captures.

The scene opened in the pinned editor and entered Play mode; the local traversal probe compiled and was usable. Its 3.5 m/s default and 5.0 m/s Shift speed are inspection controls only, not canonical gameplay-speed decisions.

## Geometry quantities

- Hard outer polygon: **44,817.5 m²**.
- Río water: **3,505.0 m²**.
- Arroyo water: **713.6875 m²**.
- Dry components before accepted crossing overlays: Orilla-sur **3,200.0 m²**, Wedge **31,704.125 m²**, Ensanche **5,694.6875 m²**.
- X1 is the permanent crossing. X5 is conditional.

The route quantities recorded below are drawn centerline geometry, not human measurements:

| Edge | Drawn width (m) | Centerline (m) | CITY-01 hypothesis (min) | Human measurement owner |
|---|---:|---:|---:|---|
| W04 | 5.0 | 47.4 | 0.50 | CITY-07 |
| W05 | 3.0 | 74.1 | 1.75 | CITY-07 |
| W06 | 2.8 | 93.9 | 2.25 | CITY-07 |
| W12 | 2.8 | 75.0 | 1.00 | CITY-07 |
| W13 | 2.6 | 82.8 | 1.00 | CITY-07 |
| X1 | 5.5 | 33.0 | 1.00 | CITY-07 |
| X5 available | 2.5 | 14.1 | 0.40 | CITY-07 |

## Captures

- `overhead.png` — hard boundary, water, routes, sites and soft non-playable seams.
- `x1_to_casco.png` — old bridge head toward core.
- `plaza_to_casco.png` — plaza/Casco orientation.
- `landing_to_port.png` — confluence/port-facing sightline.

## Owner-tagged ledger

| ID | Causal owner | Observation | Final CITY-04 routing |
|---|---|---|---|
| D01 | CITY-01 / CITY-07 validation | W05/W06/X1/X5 geometric lengths do not themselves prove the planning-time hypotheses. | Human timing deferred to CITY-07; no CITY-04 defect established. |
| D02 | CITY-07 validation | Soft seams and sparse ordinary fabric may or may not communicate a larger town once assets/continuation context exist. | Explicit scale/density/continuation question for CITY-07. |
| D03 | CITY-07 validation | Historic lanes, bridge read, X5 read, thresholds and quiet/busy contrast require human perceptual proof. | Explicit human/perceptual package for CITY-07. |

No concrete technical contradiction is established against CITY-00/01/02/03/05/06 or H1.

## Final CITY-04 verdict

**PASS — technical scene handoff only.** The accepted hard boundary, water, dry components and crossings are represented and physically checked; the scene is inspectable and walkable as a local technical probe; captures are durable. Scale, density, route choice, travel times, sightlines, thresholds, quiet/busy contrast and continuation remain **UNVALIDATED BY CITY-04** and are mandatory CITY-07 demo obligations under the accepted amendment.