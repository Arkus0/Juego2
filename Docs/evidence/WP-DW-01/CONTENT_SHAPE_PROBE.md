# WP-DW-01 — CITY invariant content-shape probe

## Approved sources and bounded real universes

DW-01 consumes three accepted repository-owned CITY authorities without changing them:

- `Docs/production/CITY_LOCATION_PROGRAMME.md` (CITY-02): programme identities, A/B importance, I-depth and default access posture;
- `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md` (CITY-05): accepted A/B functional-POI binding and normalized required access-role sets;
- `Docs/production/CITY_INTERIORS_DISCOVERY.md` (CITY-06): inherited I-depth and the Full I1–I3 allocation.

The real access invariant covers the complete current CITY-02 A/B universe: **23 functional POIs**. The second invariant covers the complete current CITY-02 non-I0 universe: **14 I1–I3 commitments** (1 I3, 6 I2, 7 I1).

These counts are observations of the current accepted source shape, not hard-coded completeness oracles. `CityDesignWorldProvider` derives the expected subject sets from CITY-02 on each build and compares independently parsed CITY-05/CITY-06 tables against them. Therefore deleting a downstream row does not shrink the obligation.

## Real content through the candidate surface

`CityDesignWorldProvider.BuildAndValidate` projects three source-owned slices through the generic DW-00 machinery:

1. CITY-02 programme facts (`city-programme-place`) containing importance, I-depth and normalized required-role flags;
2. CITY-05 binding facts (`city-access-binding`) containing required-role flags;
3. CITY-06 depth/allocation facts (`city-interior-depth` plus `city-interior-allocation --allocates-depth--> city-interior-depth`).

Every projected fact uses `AnchoredTextAuthorityReader` provenance to its actual accepted source document and exact Markdown row. The derived `WorldState` remains generic `dw.fact`; no CITY type/rule is added to H0.

### Invariant A — inherited access roles

For every source-derived CITY-02 A/B subject:

`programme_required_access_roles(place) ⊆ bound_required_access_roles(place)`

Normalization is deliberately narrow and source-defined: `{public, service, private, semi-private}` only. CITY-02 `conditional public` is not promoted. Spatial/use words such as vertical, court, rear, staff or storage never substitute for an access role.

Causal controls exercise removal of one role, removal of the entire bound-role surface and removal of an entire CITY-05 A/B row while the CITY-02 universe stays intact.

### Invariant B — complete I1–I3 allocation and depth preservation

For every source-derived CITY-02 non-I0 subject:

- the subject must exist exactly once in the CITY-06 inherited-depth table;
- the subject must exist exactly once in the CITY-06 Full I1–I3 allocation table;
- CITY-06 inherited I-depth must equal CITY-02 I-depth;
- no I0/out-of-universe subject may appear in the Full I1–I3 allocation.

The allocation facts carry an explicit `allocates-depth` relation to the independently required depth fact, so this proof exercises a relation/cardinality shape different from the access-role subset invariant.

Causal controls remove an allocation, downgrade the hero bar depth, add an I0 place to the allocation and duplicate an allocation row.

## Additional boundary probes

- Changing accepted source bytes while leaving values/anchors semantically equal invalidates cached provenance through the inherited DW-00 oracle.
- Rebuilding from fresh provider instances and reversing input/definition enumeration must preserve normalized representation and digest.
- The canonical observer checks that H0 assemblies do not acquire a reverse `Arkus.DesignWorld` or CITY-validator dependency and that `Arkus.DesignWorld` still depends only on `Arkus.Game.World`.
- The exact-SHA observer runs focused DW-01 tests plus the full Harness regression. Exact execution is a freeze gate, not repository-authored evidence.

## Findings and classification

| Finding | Classification | Consequence |
|---|---|---|
| Current accepted CITY sources expose a mechanically bounded 23-subject A/B access universe and 14-subject I1–I3 universe. | in-scope evidence | Real product data, not toy fixtures, drives both invariants. |
| CITY-02 can independently define downstream completeness for both selected invariants. | in-scope evidence | Removing a CITY-05/06 declaration cannot self-confirm completeness. |
| Exact-row provenance is sufficient for the bounded accepted Markdown tables used here. | in-scope evidence | Generic DW-00 provenance remains usable; no predecessor reopen. |
| Access-role truth and I-depth/allocation truth remain CITY-owned. | authority-boundary confirmation | DW validates accepted semantics but does not become CITY authority. |
| Full CITY projection/query/reporting is not claimed. | named residual | Owned by DW-02 and later DW work. |
| Retained geometry, Unity realization, gameplay/runtime access and discovery causality are not proven. | out of DW-01 boundary | Remain with their existing downstream owners. |
| No accepted DW-00/H0 contract contradiction was found. | predecessor reopen assessment | DW-00 and H0 remain consumed, not reopened. |

Current-WP blocker findings: **none after the bounded universe/oracle and workflow-artifact cleanliness repairs above**.
Concrete predecessor reopen conditions triggered: **none**.
