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

The candidate protects the accepted generic DW files plus the H0 dependency chain actually reached by the projector (`Arkus.Game.World` -> `Arkus.Game.Core`) against candidate mutation. No semantic repair to those files is part of DW-05.

## Hostile controls and their interpretation

1. **Missing neutral authority fact/provenance** — the independent universe still requires `calibration.lamp-a`; deleting its authority anchor makes the existing projector fail with `dw.provenance_missing`. This is a generic-path RED.
2. **Stale source bytes** — changing source bytes while validating an existing projection makes the existing validator report `dw.provenance_stale`. This is a generic-path RED.
3. **Relation target outside the independent universe** — targeting `calibration.ghost` fails with `dw.relation_target_outside_universe`. This is a generic structural RED.
4. **Domain-required relation omitted but graph remains structurally valid** — removing `calibrated-by` leaves the generic structural validator GREEN, while the independent neutral consumer oracle REDs. This is intentionally retained evidence, not hidden. It is classified as `domain_need`, not as a generic Arkus deficiency: the accepted generic seam represents typed relations but does not promise to infer which domain relations are semantically mandatory. That boundary matches the accepted DW invariant model used by CITY.
5. **Domain-conditioned family behavior** — the behavioral differential proof derives a repeated field family from the accepted CITY graph at runtime, passes it through an ordinary helper/property indirection and injects a `StartsWith(...)` success condition. The unrenamed CITY graph satisfies that gate while the alpha-renamed graph does not, so the same differential oracle must RED. The proof does not contain a handwritten family/prefix inventory.
6. **Opaque domain text false-positive control** — adopted CITY field text is inserted only as a string payload in an otherwise alpha-renamed PA graph. Generic DW validation, H0 validation and canonical H0 round-trip behavior remain invariant; the differential proof therefore stays GREEN rather than treating mere text coincidence as semantic dependence.
7. **Residual omission** — removing an independently inventoried predecessor limitation from the closure manifest is required to RED.
8. **Unsupported kernel classification** — relabeling a domain-only limitation as `generic_arkus_contradiction_deficiency` without public-seam failure evidence is required to RED.
9. **H0/generic seam mutation** — canonical DW-05 observation diffs the accepted baseline against the candidate and rejects any change to the generic DW contract/projection files, `src/Arkus.Game.World` or its reached `src/Arkus.Game.Core` dependency.

## CITY / PA semantic-independence audit

The earlier structurally derived vocabulary audit is retained as an auxiliary source/dependency check, but it is no longer the primary evidence for semantic independence. Exact-atom source matching is intrinsically unable to establish independence from a derived family such as a prefix, helper-returned pattern or other ordinary semantic derivation.

`Dw05SemanticAlphaRenamingTests` therefore exercises a behavioral/metamorphic proof over the accepted graphs themselves:

- CITY `ProgrammeProjection`, `BindingProjection` and `InteriorProjection` are exercised as three independent accepted universes rather than concatenated into a synthetic combined universe;
- PA is exercised as its accepted projection universe;
- every fact type, field key, relation type and fact/target identity is deterministically alpha-renamed to domain-neutral tokens;
- bounded categorical string values are alpha-renamed as values, while opaque/free-form payload text is not treated as semantic structure;
- an independent inventory check requires equal semantic-axis cardinality and zero exact atom overlap between original and renamed semantic axes;
- an independent topology signature, which does not consume the rename map, requires the same per-fact value-kind shape, in/out degree distribution and relation-type multiplicities;
- both original and renamed graphs are rebuilt through the real `DesignWorldProjector` into H0 `WorldState`, checked by `DesignWorldProjectionValidator`, checked by the production `WorldStateValidator`, serialized/deserialized by `CanonicalWorldStateCodec`, validated again and required to have the same structural observation vector and canonical round-trip stability.

This changes the question from “did the audit happen to enumerate the spelling used by a domain assumption?” to “does reached generic DW→H0 behavior remain invariant when all behaviorally meaningful domain names are coherently changed?”. A hidden success condition on an adopted CITY/PA field family changes the differential outcome even when the family string is not an exact adopted field atom.

The family mutant is itself bounded and data-derived: its family prefix is structurally selected from repeated accepted CITY field keys at runtime, then used only by a test-owned injected dependency through an indirect provider property. It is a sensitivity control for the differential oracle, not a new detector rule and not a production/H0 modification.

The generic contract/projection files still contain no CITY/PA types, rules or vocabulary required by the neutral probe. `Arkus.DesignWorld` directly references `Arkus.Game.World`; that project in turn references `Arkus.Game.Core`, whose project has no further `ProjectReference`. The executable source audit still consumes the source/project surfaces for both H0 projects in that reached closure as a complementary check.

CITY and PA implementations are currently co-located in the `Arkus.DesignWorld` assembly. That is recorded as `domain-provider-colocation`: an in-repo modularity/productization limitation, not evidence that the generic public seam requires CITY/PA knowledge. External package composition remains explicitly unproven and H2-owned.

## Boundedness / anti-overdefense

The alpha-renaming proof is intentionally not a formal non-interference proof. It does not claim to detect arbitrary encodings, hashes, reflection tricks or unreachable hidden helpers. Its claim is bounded to the behavior reached by the accepted generic projection/validation path and the H0 world-state validation/codec path exercised by DW-05. Together with the byte guard on production/H0 seams and the source/dependency audit, this is the bounded falsification required by DW-05 rather than an attempt to enumerate infinitely many equivalent encodings.

## Provenance and rebuild

The neutral graph uses exact authority anchors and checks `Found` resolution for every projected fact. Clean rebuild from identical authority input and projection version must preserve digest and normalized representation and produce an empty projection diff. Missing and stale authority paths fail closed through the existing generic implementation.

No provenance field is dropped, weakened or replaced by probe-local generated truth. The source fixture remains authority for the probe and is not mutated by projection/query/validation.

## Real implementation defect that does not change the architectural conclusion

`src/Arkus.DesignWorld/Arkus.DesignWorld.csproj` still carries a comment saying its `CS8625`/`CS8604` warning suppression is temporary during active DW-03 iteration and should be removed before freeze. That is genuine repairable implementation debt. DW-05 does not remove it to improve the result because the neutral probe does not require the suppression and its presence does not demonstrate a public-seam contradiction. It is classified as `dw_tooling_need` with causal owner `DW_MAINTENANCE`.

## Public-seam contradiction test

The neutral shape and semantic-renaming stress did not require:

- widening/changing the generic public contract;
- a new shared abstraction or shared capability motivated by the probe;
- an H0 semantic change;
- CITY/PA knowledge in the neutral probe or reached generic/H0 implementation;
- weakening provenance or rebuild semantics.

Therefore the Worker found no evidence satisfying the DW-00/H0 reopen conditions. This is bounded survival of the specified stress, not proof that all future domains will fit.
