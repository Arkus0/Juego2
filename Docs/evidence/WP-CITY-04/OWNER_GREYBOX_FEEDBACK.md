# CITY-04 owner greybox feedback — 2026-09-25

## Observation identity

- Candidate experienced: `37a47ca5ff4fee00468e83241bfacda555dc4280` on local `worker/city-04`.
- Editor: Unity `6000.3.24f1`, `City04Greybox.unity` in Play mode.
- The human did not submit a nine-run sheet, segment logs, measured times or a final spatial PASS/REVISE verdict.

## Reported result

The scene has a coherent underlying structure and is comfortable to walk, but cube-only visual language makes orientation and reliable travel-time judgment too ambiguous. If mistaken for the whole town, it also feels small and sparse. The accepted CITY-03 contract defines it as the first retained seed rather than the complete town: F01–F08 are retained frontage slots and five seams lead toward later districts.

This leaves a legitimate product question for the real demo: whether the asset-rich retained district communicates convincing scale, building density, route hierarchy and continuation. The raw greybox is not a suitable end-user playtest surface for that question.

The local traversal controller was therefore adjusted to 3.5 m/s normally and 5.0 m/s with Shift for technical inspection only; this is not a canonical gameplay-speed decision.

## Accepted routing

The requested process split was subsequently accepted by `CITY_GREYBOX_DEMO_VALIDATION_AMENDMENT.md` through process PR `#226`, PASS review `#5321232069`, merge `f86347ed2857940126a7489325ac8455f65b3f29`, with DocSync complete.

Accordingly:

- CITY-04 retains the automated geometry/collider/capture proof and technical scene-handoff verdict.
- No further cube-only human timing is required in CITY-04.
- The complete human traversal/timing/perceptual baseline is mandatory in CITY-07 on the asset-rich keeper candidate.
- Scale/density/continuation, route choice, sightlines, thresholds and quiet/busy contrast remain explicitly unvalidated until CITY-07.
- CITY-08 owns the later post-authoring human regression.

This note remains preliminary human feedback only; it neither supplies nor substitutes for CITY-07's measured spatial verdict.