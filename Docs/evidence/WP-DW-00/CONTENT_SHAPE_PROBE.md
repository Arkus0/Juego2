# WP-DW-00 — representative Juego2 content-shape probe

## Approved source and bounded slice

Authority source: `Docs/production/CITY_LOCATION_PROGRAMME.md`, accepted CITY-02 programme source.

Bounded source slice: the accepted `loc.casco.bar` programme row, including its stable location id, district, planning/mobility id `W.CASCO`, importance `A`, spatial-depth `S4` and interior-priority `I3` fields.

This probe is deliberately **not** a CITY semantic oracle. It asks only whether one real accepted Juego2 shape can pass through the generic DW-00 identity/typed-field/relation/provenance/public-H0 projection boundary without forcing CITY vocabulary into H0.

## Assumptions

- `loc.casco.bar` is represented as one projected fact at the granularity already named by the accepted programme row.
- `W.CASCO` is carried as a separate projected target identity (`mobility.w.casco`) so a typed relation can exercise the generic relationship shape without claiming CITY topology truth.
- Both facts bind provenance to the exact accepted row and the source-document digest. The fact that two projected records may share one source anchor is permitted; ambiguity concerns multiple matching source locations for one anchor, not multiple facts intentionally sourced from one unique location.
- The adapter definitions are projection rules, not source authority. Changing a rule/field changes derived output but does not mutate the programme document.

## Owned boundaries exercised

`Dw00DesignWorldProjectionTests.RepresentativeJuego2CityShapeFitsGenericSurfaceWithoutChangingAuthorityBytes` exercises:

- real accepted identity and field granularity;
- a typed generic relation through `WorldReference`;
- unique source-anchor provenance and source digest;
- DW validation over the projected slice;
- public H0 reference query via `WorldInspectionService`;
- before/after byte equality for the accepted source document;
- generic H0 object type `dw.fact` and extension owner `arkus.designworld` rather than CITY-specific H0 types.

`Dw00RepresentativeProbeBoundaryTests.ApprovedCityShapeRebuildsAndDiffsThroughTheOwnedGenericBoundaries` additionally exercises the same approved row through:

- identical-authority rebuild equality;
- empty diff after identical rebuild;
- public inspection/query on rebuilt state;
- a projection-rule-only field change causing a deterministic digest and fact-level diff change while the authority bytes remain untouched.

The neutral fixture remains the independent-universe/completeness proof; this real probe supplements it and is not used to claim full CITY coverage.

## Findings and classification

| Finding | Classification | Consequence |
|---|---|---|
| Accepted location identity, scalar programme fields and one relation target fit the generic fact/reference/extension surface. | in-scope evidence | No H0 reopen required for the bounded representability claim. |
| A unique exact row plus source digest is sufficient for fail-closed provenance on this Markdown shape. | in-scope evidence | Stale/ambiguous behavior remains covered by neutral + representative negative controls. |
| The same accepted source row can legitimately anchor more than one projected fact when the projection rule declares that granularity. | in-scope design decision | Source location is authority provenance; it is not itself projected identity. |
| Whether `A`, `S4`, `I3`, access roles or the programme relation are semantically correct/complete is not decided here. | named future/residual | First semantic CITY invariant remains owned by `WP-DW-01`; broader production/query completeness remains `WP-DW-02`. |
| Full CITY programme-universe extraction/parsing is not required to represent this bounded slice. | out of DW-00 completeness boundary | No claim of full CITY parser or programme completeness. |
| No accepted H0 public-surface contradiction was found. | predecessor reopen assessment | `WP-HK-GATE` remains consumed; reopen condition did not fire. |

Current-WP blocker findings: **none after the probe additions above**.
Concrete predecessor reopen conditions triggered: **none**.
