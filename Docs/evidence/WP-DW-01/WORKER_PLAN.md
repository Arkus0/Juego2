# WP-DW-01 — Worker plan

Baseline: `main@31f8258cf2873e9080d9dacad2cfe956f0e2fa2e`
Workpack: `WP-DW-01 — CITY invariant vertical slice`
Execution mode: REMOTE_OK

## Predecessor gate

`WP-DW-00` is accepted/merged/DocSynced on the baseline. DW-01 consumes its generic Design World projection, provenance, deterministic rebuild and H0-consumer boundary. No current evidence requires reopening DW-00 or H0.

## Exact invariant 1 — access-role preservation

For the complete accepted CITY-02 A/B functional-place universe:

`programme_required_access_roles(place) ⊆ bound_required_access_roles(place)`

Independent source surfaces:

- programme universe and programme-side required roles: `Docs/production/CITY_LOCATION_PROGRAMME.md`, especially the complete District × location programme table. Universe membership is derived from A/B classification there, not from the downstream binding table;
- bound-side required roles: `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md`, §9 `A/B place → exterior composition mapping`, independently enumerated from its binding rows and `Required access-role set` column.

Normalization is restricted to the source-owned CITY-05 rule: the CITY-02 `Default access posture` is normalized only into the four declared tokens `{public, service, private, semi-private}`. Descriptors/qualifiers do not substitute for a role; `conditional public` is not promoted to required. No synonym table or new access ontology will be invented by DW-01.

Oracle requirements:

- independently enumerate the CITY-02 A/B universe and the CITY-05 binding universe;
- require complete subject coverage and no extra binding subject masquerading as programme truth;
- compare source-derived normalized role sets;
- deterministic first offender must identify subject, missing role, violated rule and both source paths/provenance;
- omission of an entire role surface or an entire A/B subject must fail closed rather than shrink the oracle.

## Exact invariant 2 — complete I1–I3 allocation identity/cardinality

The complete set of CITY-02 places carrying an enclosed-interior commitment `I1`, `I2` or `I3` must equal the complete CITY-06 Full I1–I3 allocation set, with exactly one allocation per required place and matching declared I-depth.

Mechanically:

`CITY02_required_interior_places == CITY06_allocated_interior_places`

and for every subject:

`CITY02_I_depth(subject) == CITY06_I_depth(subject)`

The current accepted authority contains 14 required places (1 I3 + 6 I2 + 7 I1); the validator will derive the identities independently from each source rather than encode a 14-item registry as authority.

Independent source surfaces:

- required universe/depth: `Docs/production/CITY_LOCATION_PROGRAMME.md`, District × location programme table plus the accepted I1/I2/I3 declarations;
- allocated universe/depth: `Docs/production/CITY_INTERIORS_DISCOVERY.md`, §6 `Full I1–I3 allocation` (with inherited I-depth visible from the accepted allocation surface / source facts).

This invariant is structurally distinct from access-role subset checking: it is exact set/cardinality + per-subject depth equality over an independently enumerated downstream allocation.

Oracle requirements:

- an omitted required interior subject remains visible in the CITY-02 oracle and fails;
- an extra/I0 allocation fails;
- duplicate allocation fails;
- an I-depth weakening/mismatch fails;
- first offender is deterministic and source-provenanced.

## Implementation paths

Expected owned implementation/test surfaces:

- `src/Arkus.DesignWorld/CityDesignWorldProvider.cs` — bounded CITY parser/provider + invariant validator downstream of the generic DW-00 contract;
- `tests/Arkus.Harness.Tests/Dw01CityInvariantTests.cs` — real-source positive proof, independent-universe checks, causal mutations and determinism;
- `scripts/dw01-observe-exact-sha.sh` plus canonical observer/verifier dispatch only if required by the established foundational exact-SHA route;
- `Docs/evidence/WP-DW-01/*` — execution/test/proof/review evidence after implementation.

No H0 project/public generic contract will receive CITY-specific types or rules.

## RED-first proof plan

Before accepting GREEN evidence, execute causal mutations over real-shaped accepted source copies while keeping the independent oracle side intact:

1. remove one effective CITY-05 required access role from an A/B binding; expect pipeline RED naming exact subject + missing role;
2. remove the entire effective bound-role surface for a subject; expect RED;
3. omit one projected/bound A/B subject while CITY-02 universe remains unchanged; expect RED;
4. omit or weaken one CITY-06 I1–I3 allocation/depth while CITY-02 required universe remains unchanged; expect RED;
5. stale/wrong provenance with equal-looking values; expect RED through the inherited DW provenance boundary;
6. reverse source/input enumeration and rebuild from scratch; semantically equal truth must preserve verdict and digest.

Then restore accepted bytes and require GREEN for both invariants.

## Known unknowns / explicit non-inventions

- Markdown parsing will be bounded to the accepted tables/headings actually owned by CITY-02/05/06; unsupported/malformed source shape fails closed rather than being guessed.
- DW-01 will not invent runtime IDs, parcel identities, Unity objects, CITY semantics, synonyms or topology.
- `A/B`, access-role normalization and `I0..I3` meanings remain upstream CITY authority.
- Passing this slice does not claim full CITY conversion, production query completeness, geometry truth or Unity conformance.
- If real-source representability contradicts DW-00's generic contract, implementation stops and triggers the explicit DW-00 reopen condition rather than weakening source truth.
