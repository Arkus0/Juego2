# WP-DW-01 — residual-risk and proof-budget audit

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Declared boundary

DW-01 proves two bounded real CITY invariants through the generic DW-00 projection/provenance surface: complete A/B access-role preservation from CITY-02 to CITY-05, and complete I1–I3 allocation/depth plus allocation→depth relation preservation from CITY-02 to CITY-06. It does not claim complete CITY modelling, geometric truth, Unity realization, runtime availability/permissions, discovery gameplay, PA semantics or external-repository productization.

## Blocking-risk audit

No known in-boundary defect class remains intentionally undetected.

- A downstream access registry cannot shrink silently because its expected subject universe comes from CITY-02.
- A downstream role set cannot weaken silently because source-derived role flags are compared for every upstream A/B subject.
- An interior allocation/depth registry cannot shrink, over-promote or duplicate silently because both CITY-06 surfaces are compared to the independent CITY-02 non-I0 universe and duplicates fail closed during parsing.
- The production `CityInteriorRelationOracle`, invoked by `CityDesignWorldProvider.BuildAndValidate`, separately requires for every CITY-02 I1–I3 subject an `allocation.<id>` fact, a `depth.<id>` fact and exactly one literal `allocates-depth` edge to that subject's own depth fact. It consumes no `AnchoredFactDefinition`, authority reader or relation registry, so changing the construction rule cannot auto-confirm through the generic validator.
- Missing, renamed, duplicate and cross-subject `allocates-depth` relations are diagnosed with deterministic machine code, subject, semantic rule, expected/observed relation detail and CITY-02/CITY-06 provenance paths. A dangling target outside the generic fact universe is already rejected by inherited `DesignWorldProjector` before a malformed projection can be delivered.
- Equal-looking cached facts cannot survive accepted-source byte drift because inherited DW-00 provenance validation binds the whole source digest and exact row anchor.
- Input enumeration order cannot alter the canonical derived representation/digest.
- Removing/bypassing the semantic validator is not self-confirming: the causal mutation tests expect named pipeline exceptions and fail if the validation pass stops executing.
- CITY-specific implementation cannot migrate into H0 unnoticed along the tested seam because the canonical observer scans H0 sources/project references and checks DesignWorld's dependency boundary.

## Non-blocking residuals

| Residual | Why non-blocking for DW-01 | Future owner / trigger |
|---|---|---|
| Markdown table headings/column shapes may legitimately evolve in future accepted CITY edits. | Candidate deliberately fails closed rather than guessing a new semantic mapping. | Update/version the CITY adapter when an accepted source owner changes shape. |
| Only the selected access and I1–I3 invariants are projected semantically. | Full CITY conversion/query usefulness is expressly outside this WP. | WP-DW-02. |
| A/B access roles are planning-time required roles, not proof that runtime access is currently open. | Runtime schedules/permissions are not CITY-05 or DW-01 claims. | Later runtime/Living World owners. |
| I-depth/allocation correctness does not prove room geometry, route clearance or authored Unity content. | Geometry/engine-backed work is explicitly downstream. | CITY/H1/H2 downstream work as already planned. |
| A relation target outside the generic projection universe fails at the inherited DW-00 projector boundary rather than in the CITY oracle. | Such a target cannot produce an `InteriorProjection`; the anti-whack audit asserts the inherited fail-closed machine code, while the CITY oracle covers every constructible in-universe missing/renamed/cardinality/redirection defect. | Reopen only if DW-00 ever permits dangling relation targets. |
| Source-row extraction relies on normal UTF-8 Markdown and documented .NET string behavior. | Compiler/runtime/filesystem are inside the foundational trusted base; source shape is fail-closed. | Reopen only if accepted source format itself changes. |
| SHA-256/Git checkout/runner can theoretically fail outside documented behavior. | Explicit FOUNDATIONAL_PROOF_STANDARD trusted base. | Infrastructure incident, not DW-01 product proof. |

## Dependency / IP audit

DW-01 adds no external package or runtime dependency. It reuses repository-owned `Arkus.DesignWorld`, accepted CITY Markdown and existing xUnit/.NET proof infrastructure. `Arkus.DesignWorld` remains a downstream consumer of `Arkus.Game.World` only.

## Predecessor reopen audit

Real CITY evidence did not show the DW-00 generic identity/field/relation/provenance/rebuild surface to be false or unusable. The three accepted sources can be represented with generic DW facts plus typed fields/relations while CITY semantics stay in the downstream consumer. No H0 public-contract change is required.

Therefore **WP-DW-00 is not reopened**, and its stricter H0 reopen condition is likewise not triggered.

## Proof-budget rationale

The support machinery is bounded to the workpack contract:

- one CITY provider/validator over three already-accepted sources plus one production relation oracle local to that CITY surface;
- a focused DW-01 test suite exercising the positive real universes and the named causal omission/drift/relation classes;
- one observer/verifier pair registered in the existing exact-SHA dispatchers;
- compact evidence files plus the pre-implementation Worker plan.

Each negative maps directly to an acceptance criterion or named causal class in WP-DW-01. No test attempts to prove arbitrary toolchain/runner behavior, and no general CITY parser/framework is introduced. The proof is materially smaller than a full CITY conversion and stops at the two invariants DW-01 owns.
