# CITY-04 local greybox observation — incomplete physical verdict

Status: `CONTRACT_AMENDMENT_REQUIRED`; intermediate Worker evidence, not `WORKER_PRE_REVIEW: CLEAN`, `REVIEW_READY` or a retained-seed PASS/REVISE verdict. See `OWNER_GREYBOX_FEEDBACK.md` for the 2026-09-25 human assessment and deferred timing decision.

## Input and execution identity

- Canonical PR/branch: `#221`, `worker/city-04`, Draft + ACTIVE.
- Unity scene/capture material commit observed: `22bf4a115070a1045c2a2b180988df7a4877d7f9`.
- Accepted CITY-03 seed source blob: `3186b80d173cd20f961f33d5a28bf447b2c6971d`; `City04Layout.json` projection blob: `7b1c83815188f323ae23a02f43aa08c085592f6e`.
- Environment: Windows 11 Pro 10.0.26200; Unity Editor `6000.3.24f1`; built-in render pipeline; `com.unity.test-framework` 1.6.0 and built-in `com.unity.modules.physics` 1.0.0. Package manifest blob `19c29d4989182a73f559eefbf131775b99097a07`, lock blob `f1cd55ead4c8bd5254372af66942e1682c2e3225`.
- Scene blob: `6846ac662c01aa4ed05c6c3b9c2ee1559f9383f3`. Existing ignored H1 generated state and owner-configured external source assets were not edited or adopted. This is a bounded CITY physical sidecar on the accepted Unity project, with no new canonical/H1 bridge semantics.

## Executed local observations

From the repository root, `PYTHONPATH=.city04-tools python scripts/city04_generate.py` with temporary local `shapely==2.1.2` / GEOS 3.13.1 regenerated the committed OBJ and C# projections. It verified source blob, the exact hard polygon, water areas, three dry components, and site containment outside effective water/no-build regions.

The pinned editor ran these repository-owned CITY-04 methods in `-batchmode -quit` against the retained project:

1. `Arkus.CITY.Editor.City04GreyboxBuilder.Build` — `CITY04_BUILD_GREEN`, scene saved from the accepted projection.
2. `Arkus.CITY.Editor.City04GreyboxBuilder.CheckPhysicalCuts` — `CITY04_PHYSICAL_CUTS_GREEN`: raycast collision on Wedge, Orilla-sur and Ensanche; X1 and available X5; no collision on representative Río/Arroyo water; no X5 collision after closure.
3. `Arkus.CITY.Editor.City04GreyboxBuilder.Capture` — `CITY04_CAPTURE_GREEN`, four camera captures committed below.

The physical cut check is a bounded machine observation of the effective Unity scene. It does not substitute for walking, sightline judgment, timing or the nine required human runs. The local batch logs are in ignored `Unity/ArkusUnity/Logs/city04-{build8,physical5,capture7}.log`; the durable facts above are transcribed from those logs. The final evidence-bearing SHA would need a fresh read-only exact-SHA check before freeze.

## Boundary geometry and provisional route quantities

The generated outer polygon is **44,817.5 m²**; Río water is **3,505.0 m²**, Arroyo water **713.6875 m²**. Dry components are Orilla-sur **3,200.0 m²**, Wedge **31,704.125 m²**, and Ensanche **5,694.6875 m²** before the accepted crossing overlays. Only X1 is permanently collidable. X5's collider is available only in its low-water probe state.

The table below is **drawn centerline geometry**, not a measured walk-time table. The `1.4 m/s` column is an arithmetic reference from the original probe configuration; after owner feedback the probe defaults to 3.5 m/s. The reference excludes human route choice, pauses, stairs, collisions, turns and perception. It is not a substitute for later human timings on the real demo.

| Edge | Drawn width (m) | Centerline (m) | End rise (m) | 1.4 m/s lower reference (min) | CITY-01 hypothesis (min) | Human measured (min) |
|---|---:|---:|---:|---:|---:|---|
| W04 | 5.0 | 47.4 | -0.24 | 0.56 | 0.50 | **MISSING** |
| W05 | 3.0 | 74.1 | -3.23 | 0.88 | 1.75 | **MISSING** |
| W06 | 2.8 | 93.9 | -3.71 | 1.12 | 2.25 | **MISSING** |
| W12 | 2.8 | 75.0 | -0.42 | 0.89 | 1.00 | **MISSING** |
| W13 | 2.6 | 82.8 | +1.91 | 0.99 | 1.00 | **MISSING** |
| X1 | 5.5 | 33.0 | -0.96 | 0.39 | 1.00 | **MISSING** |
| X5 available | 2.5 | 14.1 | +0.20 | 0.17 | 0.40 | **MISSING** |

The greater CITY-01 weights for W05, W06, X1 and X5 are hypotheses to test, not defects already established by this arithmetic. No absent full-city journey is marked measured.

## Captures

- `overhead.png` — hard boundary, water, routes, sites and soft non-playable seams.
- `x1_to_casco.png` — old bridge head toward core.
- `plaza_to_casco.png` — plaza/Casco orientation.
- `landing_to_port.png` — confluence/port-facing sightline.

These are batch-rendered Unity views. They do not establish that a person perceived route choice, city continuation, quiet/busy contrast, access-role separation, bridge constraint or acceptable scale.

## Owner-tagged deviation ledger

| ID | Causal owner | Observation | Status |
|---|---|---|---|
| D01 | CITY-01 | W05/W06/X1/X5 drawn path lengths at 1.4 m/s are shorter than planning weights; real walk-time meaning remains untested. | `HYPOTHESIS_TO_TEST`, no revision claim |
| D02 | CITY-04 | Soft seam silhouettes and sparse ordinary fabric may or may not read as a larger city in person. | `HYPOTHESIS_TO_TEST`, no physical verdict |
| D03 | CITY-04 | Historic lanes, bridge bollards, X5 low-water read, service/private threshold and quiet/busy legibility have only batch captures/collider checks. | `HYPOTHESIS_TO_TEST`, no physical verdict |

No observed defect is currently routed to CITY-00, CITY-02, CITY-03, CITY-05, CITY-06 or H1. If the human runs falsify one, add the concrete observation and causal owner; do not silently move the accepted seed or weaken predecessor obligations.

## Outstanding acceptance work

Execute and record all nine human traversals in `HUMAN_TRAVERSAL_RUN_SHEET.md`, the seven segment times and relevant concatenated trips, realized clear widths/grades/bank separation and sightlines, role thresholds, route choice, quiet/busy contrast and five seam plausibility checks. Then classify actual defects, repair CITY-04-owned issues while Draft or route predecessor contradictions, give the truthful retained-seed PASS/REVISE verdict, and only then obtain exact-SHA preflight, strict Worker pre-review and normal freeze handoff.

Current retained-seed verdict: **NOT ISSUED** under the existing contract. The owner has requested deferral of full timing to the real demo; that requires an independently accepted contract amendment before the CITY-04 candidate can be frozen. `CITY-07` remains blocked by CITY-04 and H1-GATE.
