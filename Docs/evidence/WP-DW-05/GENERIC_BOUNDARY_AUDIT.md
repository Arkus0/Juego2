# WP-DW-05 — Generic-boundary audit

GENERIC_BOUNDARY_CONCLUSION: SURVIVES_BOUNDED_STRESS  
MATERIAL_GENERIC_CONTRADICTIONS: 0

This is a Worker falsification result awaiting independent Reviewer judgment. It is not a general DW adoption decision and it does not claim arbitrary-domain universality.

## Neutral third-shape

The probe is a synthetic observatory/calibration graph unrelated to CITY programme/content structure and unrelated to PA finding/disposition/evidence structure:

- `observatory.north` with integer elevation and boolean remote fields;
- `detector.spectro` with decimal wavelength bounds;
- `calibration.lamp-a` with string calibration mode;
- `houses` and `calibrated-by` relations.

All probe schema, fixture, query and semantic-oracle glue is test-owned. No new production fact type, generic abstraction, provider interface, H0 rule or shared capability was introduced for this shape.

## Existing generic seams exercised

The probe consumes only already-accepted public DW/H0 surfaces:

- `StaticDesignAuthorityUniverse`;
- `AnchoredFactDefinition` and `AnchoredTextAuthorityReader`;
- `DesignValue` string/integer/boolean/decimal kinds;
- `DesignRelation`;
- `DesignWorldProjector`;
- public `DesignWorldProjection.Facts`, provenance anchors and derived `WorldState`;
- `DesignWorldProjectionValidator`;
- `DesignWorldProjectionDiff`.

The candidate pins the accepted generic DW files and `Arkus.Game.World` files to their baseline Git blob identities. No semantic repair to those files is part of DW-05.

## Hostile controls and their interpretation

1. **Missing neutral authority fact/provenance** — the independent universe still requires `calibration.lamp-a`; deleting its authority anchor makes the existing projector fail with `dw.provenance_missing`. This is a generic-path RED.
2. **Stale source bytes** — changing source bytes while validating an existing projection makes the existing validator report `dw.provenance_stale`. This is a generic-path RED.
3. **Relation target outside the independent universe** — targeting `calibration.ghost` fails with `dw.relation_target_outside_universe`. This is a generic structural RED.
4. **Domain-required relation omitted but graph remains structurally valid** — removing `calibrated-by` leaves the generic structural validator GREEN, while the independent neutral consumer oracle REDs. This is intentionally retained evidence, not hidden. It is classified as `domain_need`, not as a generic Arkus deficiency: the accepted generic seam represents typed relations but does not promise to infer which domain relations are semantically mandatory. That boundary matches the accepted DW invariant model used by CITY.
5. **Injected CITY kernel vocabulary** — the leakage checker is required to RED when a synthetic `CityKernelRule` is inserted into the audited generic/H0 source set.
6. **Residual omission** — removing an independently inventoried predecessor limitation from the closure manifest is required to RED.
7. **Unsupported kernel classification** — relabeling a domain-only limitation as `generic_arkus_contradiction_deficiency` without public-seam failure evidence is required to RED.
8. **H0/generic seam mutation** — canonical DW-05 observation diffs the accepted baseline against the candidate and rejects any change to the generic DW contract/projection files or `src/Arkus.Game.World`; the test suite independently pins their baseline Git blob identities.

## CITY / PA leakage audit

The generic contract/projection files contain no CITY/PA types, rules or vocabulary required by the neutral probe. `Arkus.DesignWorld` directly references only `Arkus.Game.World`; the neutral probe does not consume `City*` or `Pa*` provider/query/oracle types.

CITY and PA implementations are currently co-located in the `Arkus.DesignWorld` assembly. That is recorded as `domain-provider-colocation`: an in-repo modularity/productization limitation, not evidence that the generic public seam requires CITY/PA knowledge. External package composition remains explicitly unproven and H2-owned.

## Provenance and rebuild

The neutral graph uses exact authority anchors and checks `Found` resolution for every projected fact. Clean rebuild from identical authority input and projection version must preserve digest and normalized representation and produce an empty projection diff. Missing and stale authority paths fail closed through the existing generic implementation.

No provenance field is dropped, weakened or replaced by probe-local generated truth. The source fixture remains authority for the probe and is not mutated by projection/query/validation.

## Real implementation defect that does not change the architectural conclusion

`src/Arkus.DesignWorld/Arkus.DesignWorld.csproj` still carries a comment saying its `CS8625`/`CS8604` warning suppression is temporary during active DW-03 iteration and should be removed before freeze. That is genuine repairable implementation debt. DW-05 does not remove it to improve the result because the neutral probe does not require the suppression and its presence does not demonstrate a public-seam contradiction. It is classified as `dw_tooling_need` with causal owner `DW_MAINTENANCE`.

## Public-seam contradiction test

The neutral shape did not require:

- widening/changing the generic public contract;
- a new shared abstraction or shared capability motivated by the probe;
- an H0 semantic change;
- CITY/PA knowledge in the probe or generic implementation;
- weakening provenance or rebuild semantics.

Therefore the Worker found no evidence satisfying the DW-00/H0 reopen conditions. This is bounded survival of the specified stress, not proof that all future domains will fit.
