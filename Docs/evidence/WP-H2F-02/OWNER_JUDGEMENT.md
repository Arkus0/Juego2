# Owner visual inspection (WP evaluation method: targeted human visual inspection of migrated real content)

Date: 2026-09-28. Inspector: the project owner, in this Worker session, looking at `captures/owner/*.png` from the evidence workspace (ART-01 `PREFOUNDATION_INPUT` `174d05d2` route + owner-vault content on the adopted URP baseline, GC2 Core provisioned). The H2F-01 built-in reference was `Docs/evidence/WP-H2F-01/captures/owner/S01_render.png`.

OWNER_VISUAL_INSPECTION: ACCEPTED_WITH_NOTES

## Round 1 (first capture set), verbatim

> "por partes, lo casa todavía parecen assets pegados, las esquinas estan mal y las ventanas se montan encima, los marcos de las puertas no son rectos, el cartel queda muy mal sobretodo la tipografía, los modelos de los personajes hacen clipping con los objetos, los arboles directamente no se ven. Por el resto bien pero sobretodo la casa está mal, el clipping y los arb oles. Carteles iran en otro wp"

| Observation | Classification | Action |
|---|---|---|
| Character models clip with objects | **evidence defect** (edit-time bodies placed at fixed points; one NPC stood on the river kerb) | fixed: bodies placed through a capsule check; `characterOverlaps` recorded and required empty |
| Trees not visible | **evidence defect** (the worldbuilding sample scattered no trees) | fixed: ART trees with palette materials added |
| (found while fixing) black shading at the junction mouth | **product defect**: double-sided kerb fillets gave degenerate normals | fixed in `J2JunctionRealizer`; regression test on fillet normals |
| Signs / typography | out of scope | owner: *"Carteles iran en otro wp"* |
| House: modules look glued, bad corners, windows mounted on top, door frames not straight | ART-01 structural assembly. The H2F-01 **built-in** reference shows the same assembly, so it is not produced by the URP migration or the foundation. | routed to ART-01 (below) |

## Round 2 (corrected captures), verbatim

> "todo arreglado menos las casas que siguen siendo muy irregulares"

## Decision, verbatim (asked whether to freeze H2F-02 and route the houses to ART-01)

> "Congelar y derivar a ART-01"

## Routed obligations

- **ART-01:** the house assembly irregularities (glued-looking modules, corners, windows overlapping walls, door frames not straight) are an owner-observed defect of its structural benchmark. ART-01's effective candidate on this baseline must resolve them before `KEEPER_READY`.
- **Separate WP (owner):** signage and typography.
- **ART palette tuning:** fern/leaf cards with the tree-leaf texture read as floating paper, so the sample dropped the fern. The Source→palette mapping for ferns belongs to ART.
