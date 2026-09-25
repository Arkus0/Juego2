# CITY-04 → CITY-07 proposed demo-validation handoff

Status: **CONDITIONAL / NOT AN ACCEPTANCE VERDICT**. This handoff becomes CITY-04's deliverable only if process PR `#224` independently passes, merges and completes DocSync. Until then the current CITY-04 human-run and timing requirements remain binding. CITY-04 implementation PR `#221` remains Draft and is not frozen.

## Scene and evidence identity

- Current CITY-04 branch/head: `worker/city-04` at `61b98e1aa907331d7948def15e6db592495419d1`.
- Accepted CITY-03 source: `Docs/production/CITY_PRODUCT_SEED.md`, blob `3186b80d173cd20f961f33d5a28bf447b2c6971d`.
- Unity: pinned Editor `6000.3.24f1`; open `Unity/ArkusUnity/Assets/Arkus/CITY/City04Greybox.unity`.
- Technical construction: `Unity/ArkusUnity/Assets/Arkus/CITY/City04Layout.json`, generated C# and OBJ meshes, `City04GreyboxBuilder.cs`, and local `City04TraversalProbe.cs`.
- Captures: `overhead.png`, `x1_to_casco.png`, `plaza_to_casco.png`, `landing_to_port.png` in this evidence directory.
- Recorded technical observation: `LOCAL_GREYBOX_OBSERVATION.md`; direct preliminary human feedback: `OWNER_GREYBOX_FEEDBACK.md`; unexecuted timing template: `HUMAN_TRAVERSAL_RUN_SHEET.md`.

## What CITY-04 has technically observed

The generator checked the accepted seed source blob, exact outer and water polygons, three dry components, and F/S site containment. The Unity batch checks reported `CITY04_BUILD_GREEN`, `CITY04_PHYSICAL_CUTS_GREEN`, and `CITY04_CAPTURE_GREEN`: collision on the three dry components and X1, conditional X5 collision in its available state, X5 closure, and non-traversable representative Río/Arroyo cuts. Four inspectable captures were committed. The scene opened in the pinned editor, entered Play mode, and the traversal probe compiled without an observed C# error. The default probe movement changed from 1.4 to 3.5 m/s, with Shift at 5.0 m/s; this is a scene-inspection control, not a canonical player-speed decision.

These are technical observations of the represented scene. They do not establish perceived scale, route choice, sightlines, thresholds, quiet/busy contrast, timed travel or a human spatial PASS. No CITY-07 keeper or asset claim is made here.

## Human impression and unresolved product questions

The human found walking comfortable and the layout promising, but cube-only scenery too confusing to measure travel times or judge lived scale. If interpreted as the entire town, the scene felt very small and sparse. Accepted CITY-03 defines it as the first retained seed: eight F01–F08 frontage slots and five expansion seams are not the whole town. CITY-07 must still test whether a legible, asset-rich first district communicates useful scale, building density and plausible continuation. The impression is neither a pass nor a topology change request.

## CITY-07 proof package if the amendment is accepted

After CITY-04 PASS and H1-GATE PASS, CITY-07 must use admitted assets and a usable demo for the human test. Map every CITY-03 §9 spatial proxy setup `SCN-01..13` to evidence. These setups cover quiet ordinary fabric, market flow, two different ordinary trips, route A/B, F01 social thresholds, X1 crossing, F03 delivery/service, F02 civic access, a low-stakes obstacle, return to changed proxy state, authored discovery substrate, and blocked route/X5 closure. Proxy states are authored spatial setups, not NPC, governance, inventory, memory or discovery runtime claims.

Run the nine human traversal families from `HUMAN_TRAVERSAL_RUN_SHEET.md` on the asset-rich demo: free crossing; proxy A; proxy B; bridge/bar/plaza trip; port seam; residential/rural seam; blocked route and alternate with X5 closure; quiet/busy contrast; and service/private threshold. The Worker must present these through controls usable by a nondeveloper. Record observer, exact demo SHA, build/editor identity, movement-speed profile, scenario states, route choices and defects. One observation may support more than one setup when the mapping is explicit.

Measure W04, W05, W06, W12, W13, X1 and X5 when available, plus O.X1→W.PLAZA, W.SHOP→W.LANDING and W.CASCO→E.X5 under their declared conditions. Record actual seconds and pace assumptions; compare against CITY-01 planning weights. The 1.4 m/s column in `LOCAL_GREYBOX_OBSERVATION.md` is centerline arithmetic, not human timing. No absent full-city route can be labelled measured.

Judge bridge/river separation, clear widths and grades, X5 foot-only/closure readability, bank/sightline hypotheses, route choice without a map overlay, F01–F08/S01–S03 access and envelope legibility, S03 semi-private boundary, quiet/busy difference, and all five expansion seams. Issue an owner-tagged spatial **PASS / REVISE** before CITY-07 keeper PASS. A CITY-03 geometry/topology contradiction returns to CITY-03; a CITY-07 asset presentation or realization defect is repaired in CITY-07. CITY-08 performs a later human play regression after its authoring/reuse changes, repeating affected timings if movement or geometry materially changes.

Current CITY-04 Worker verdict: **NOT_READY under the current contract; no human spatial verdict issued**. This document is a proposed transfer package, not `WORKER_PRE_REVIEW: CLEAN`, a frozen candidate, or authorization to start CITY-07.
