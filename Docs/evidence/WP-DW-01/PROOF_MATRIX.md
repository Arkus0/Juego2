# WP-DW-01 — foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: DW-01 owns the bounded CITY-02→DW→CITY-05 access-role conformance seam and CITY-02→DW→CITY-06 I1–I3 allocation/depth conformance seam, including independent source-universe derivation, exact-row provenance, deterministic projection and actionable semantic diagnostics; accepted CITY design wisdom, broader CITY completeness, H0 internals outside the consumed generic seam, Unity/runtime behavior, and trusted Git/.NET/runner/hash infrastructure are outside this claim.
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

`READY` means the repository-backed proof obligations for the bounded DW-01 claim are closed. Final exact-HEAD execution, Worker pre-review and frozen-metadata verification remain mandatory handoff gates and do not mutate the semantic proof boundary.

## Independently defined universes / oracles

- **A/B access universe:** derived mechanically from every A/B row in accepted CITY-02 §4. CITY-05 §9 is parsed separately and must equal that upstream subject universe before any role comparison.
- **I1–I3 universe:** derived mechanically from every non-I0 CITY-02 §4 row. CITY-06 §5 inherited-depth subjects and §6 Full I1–I3 allocation subjects are parsed separately and must each equal the upstream universe.
- **Access semantic oracle:** source-derived CITY-02 required-role flags must be a subset of independently source-derived CITY-05 bound-role flags for every upstream A/B subject.
- **Interior semantic oracle:** source-derived CITY-02 I-depth must equal independently source-derived CITY-06 inherited I-depth for every upstream I1–I3 subject; Full allocation cardinality/identity is separately exact.
- **Allocation relation shape:** each CITY-06 allocation projects an `allocates-depth` relation to the independently required per-subject depth fact.
- **Provenance oracle:** inherited DW-00 exact source digest + unique anchor resolution is applied separately to all three accepted source documents.
- **Determinism oracle:** fresh rebuild and reversed definition/enumeration order must produce equal normalized representation and digests.
- **H0 boundary oracle:** exact-SHA observer scans accepted H0 assemblies for reverse DesignWorld/CITY-consumer dependencies and confirms DesignWorld's only project reference remains `Arkus.Game.World`.

## Acceptance / proof-obligation matrix

| Proof obligation | Claim / trust-boundary scope | Completeness argument | Positive evidence | Causal negative control / defect injection | Result | Residual risk |
|---|---|---|---|---|---|---|
| Bounded CITY provider/schema maps accepted design facts without authority inversion | DW-01 CITY consumer boundary | facts are reconstructed directly from exact accepted CITY source rows; no derived DW state is source input | accepted real-source build in `Dw01CityInvariantTests` | stale source bytes with equal-looking values are rejected by inherited provenance validation | GREEN by repository proof; exact-SHA execution required | Markdown shape changes fail closed and require adapter review. |
| Every accepted CITY-02 A/B POI is in the access proof | access completeness | expected set comes from CITY-02 A/B rows, not CITY-05 bindings or projected rows | real 23-subject positive projection | delete `loc.casco.bar` CITY-05 row while CITY-02 remains unchanged → `city.access_binding_subject_missing` | GREEN by causal test | DW-02 owns broader CITY facts outside this invariant. |
| Required access roles cannot be weakened | access semantics | role oracle compares upstream CITY-02 posture against downstream CITY-05 binding for every independently required subject | all accepted 23 bindings validate | remove `service` from bar → `city.access_role_missing`; replace whole role set with `{}` → RED | GREEN by causal tests | Conditional runtime availability is intentionally not claimed. |
| Role aliases/forms cannot silently substitute for required roles | access semantics | normalization recognizes only source-owned role tokens; CITY-05 form qualifiers are not consulted as role substitutes | accepted source role sets validate | missing required role remains RED regardless of other row text | GREEN by construction + causal role tests | New accepted role vocabulary would require explicit adapter/version change. |
| Every CITY-02 I1–I3 subject is allocated exactly once downstream | interior completeness/cardinality | expected set is every non-I0 CITY-02 subject; CITY-06 Full allocation is separate input | accepted 14-subject allocation validates | remove bar allocation → `city.interior_allocation_missing`; duplicate it → `city.interior_allocation_duplicate`; add I0 market → `city.interior_allocation_unexpected` | GREEN by causal tests | Exact room topology is outside claim. |
| Inherited I-depth is preserved | second semantic invariant | per-subject CITY-02 depth is compared to separately parsed CITY-06 §5 depth across the whole upstream universe | accepted 14 subjects agree | downgrade bar I3→I2 → `city.interior_depth_mismatch` | GREEN by causal test | Spatial quality/geometry is outside claim. |
| Second invariant exercises a different structural relation/cardinality shape | DW relation coverage | allocation and depth are separate projected facts; every allocation relation target is inside the independently required interior projection universe | 28 CITY-06 derived facts (14 depth + 14 allocation) build through DW-00 | allocation omission/extra/duplicate controls fail before a self-shrunk projection can validate | GREEN by repository proof | Broader CITY graph relations move to DW-02+. |
| Every fact used by either invariant carries accepted authority provenance | provenance boundary | three separate authority readers bind exact row anchors and whole-document digest to their actual source | positive test asserts source paths for all programme/binding/interior facts | append non-semantic bytes to CITY-05 after caching → inherited `dw.provenance_stale` | GREEN by causal test | Git/filesystem/hash primitives are trusted base. |
| Semantically equal source truth rebuilds deterministically | deterministic projection | DW-00 normalization sorts identities/fields/relations; universe implementation is independent of input order | fresh rebuild equality | reverse required-id and definition enumeration; digest/normalized representation must stay equal | GREEN by causal test | Cross-toolchain out-of-contract behavior is trusted base. |
| H0 remains generic | H0 boundary | CITY implementation lives downstream in Arkus.DesignWorld; observer independently scans H0 source/project files and dependency edge | generic `dw.fact` world objects asserted | any reverse DesignWorld/CITY-consumer reference in H0 makes observer RED | GREEN repository guard; exact-SHA execution required | Generic H0 correctness outside consumed seam remains inherited. |
| Exact-SHA observation/freeze route exists | handoff evidence | canonical dispatch resolves `WP-DW-01` to dedicated observer/verifier; observer runs locked restore, Release build, focused tests, boundary guards and regression | scripts registered in both canonical dispatchers | absent route, dirty candidate, test/build/boundary/evidence/frozen metadata mismatch returns nonzero | IMPLEMENTED; FINAL-HEAD RECONFIRM REQUIRED | Workflow-owned transient validation artifacts are excluded from candidate cleanliness only by exact pathname; tracked/other changes still fail. |

## Named causal negative-conformance classes

1. **Single access-role weakening:** downstream binding drops one inherited role → semantic validator RED.
2. **Whole role-surface omission:** downstream binding retains subject but drops every role → semantic validator RED.
3. **Self-shrinking A/B registry:** downstream row disappears → independent CITY-02 universe still requires it and REDs.
4. **Interior allocation omission:** Full I1–I3 row disappears → independent CITY-02 universe still requires it and REDs.
5. **Interior over-promotion:** an I0 place enters Full I1–I3 allocation → unexpected-subject RED.
6. **Interior duplicate/cardinality defect:** duplicate allocation row → duplicate-subject RED.
7. **Inherited-depth drift:** CITY-06 depth weakens while CITY-02 stays fixed → depth-mismatch RED.
8. **Stale authority with equal-looking semantic values:** whole source bytes change → inherited provenance RED.
9. **Enumeration-order dependence:** reverse source-derived definition/order input → canonical digest must remain identical.
10. **Validator-rule execution removed:** the mutation tests require `BuildAndValidate` itself to throw the named semantic error; deleting/bypassing either validation pass makes those tests fail rather than allowing a false GREEN.

## Exact execution

Canonical observation command:

`scripts/arkus-observe-exact-sha.sh <candidate-sha>` with `ARKUS_WP=WP-DW-01` (or equivalent PR-body resolution).

Canonical frozen verification command:

`scripts/arkus-verify-exact-sha.sh <candidate-sha>` with frozen PR metadata supplied by the workflow.

No repository evidence file is allowed to substitute for the resulting exact-SHA execution receipt.
