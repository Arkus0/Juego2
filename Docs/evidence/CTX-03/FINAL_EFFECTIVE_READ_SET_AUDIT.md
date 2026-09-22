# CTX-03 — Final effective mandatory-read-set circuit breaker

Status: **REPAIR-CYCLE 3 EVIDENCE / PRE-CLEAN INPUT**  
Scope: **only B2 effective mandatory repository read-set completeness**  
Baseline: `107694d3850a478849bffd9510dc030910fc8aa3`

## Causal defect class

The previous dynamic envelope could be GREEN while a profile/route still required repository-backed context outside the set actually discovered by production. The concrete variants were symptoms of one class: discovery was not the canonical exhaustive derivation of the effective mandatory read set.

The repair does not add a second oracle. `scripts/ctx03-dynamic-context-check.py::discover_effective_mandatory_read_set()` is the single production source of truth. Independent controls create route/profile mutations and challenge that production function.

## Production invariant

For a concrete profile/route:

1. inspect every reviewed read surface (`initial_reads`, `conditional_reads`, and the explicitly external-descriptor-only `live_state` surface);
2. fail closed on a new read-like surface or unreviewed conditional-read class;
3. discover every placeholder on those surfaces and fail closed on a new slot class;
4. classify genuinely external slots explicitly and keep only those outside repository budgets;
5. independently derive repository sources from repository authority: exact contract, mandatory contract bindings, direct dependencies, capsule navigation or accepted predecessor evidence, route evidence/track context when the conditional class requires it, anchored manifests and manifest-named files;
6. treat caller bindings as additive concrete information only — omission cannot subtract independently derivable mandatory sources;
7. validate every derived repository path and apply the reviewed per-source and de-duplicated aggregate-route ceilings.

The exact route identity still has to be supplied where no repository authority can infer which workpack is being executed. That identity selects the route; it does not enumerate the mandatory context under the route.

## Required class controls

`scripts/ctx03-effective-read-set-controls.py` challenges the real production oracle and proves:

| Class control | Required outcome |
| --- | --- |
| Mandatory placeholder in `initial_reads` | source enters effective set |
| Known placeholder in `conditional_reads` | source enters effective set |
| Direct dependency | dependency contract + repository accepted context enter effective set |
| Reviewer predecessor navigation | capsule protocol/index/capsule/authoritative evidence are derived; contract-only is insufficient |
| Caller omits predecessor binding | derived universe does not shrink |
| New role using known surfaces/classes | inherits the same resolver |
| New slot class | RED pending classification |
| New read surface | RED pending classification |
| Derived mandatory source grows above ceiling | RED from production budget oracle |
| Unrelated non-mandatory repository file grows | remains GREEN / aggregate unchanged |

These are causal controls, not assertions that a fixture field was deleted.

## Real-repository execution

Intermediate exact SHA `e31081698de1d7dabc600712517d39053e00bfaa` executed the controls in GitHub Actions after the production rewrite and CI registration:

- CTX Process Envelope run `35721489225`: **SUCCESS**;
- `CTX03_DYNAMIC_CONTEXT: PASS`;
- `CTX03_EFFECTIVE_READ_SET_CONTROLS: PASS`;
- `CTX03_FINAL_CIRCUIT_BREAKER: PASS`;
- mechanical decision: `overall=PASS`, `independent_review_authorized=true`, `semantic_review_still_required=true`;
- Context Capsule Validation run `35721489243`: **SUCCESS**;
- Arkus Candidate Validation run `35721489256`: **SUCCESS**.

This intermediate run proves the implementation/control interaction on the real repository. It is not the terminal exact-SHA evidence because this audit file and protocol clarification are later repository mutations. The complete suite must run again after the last repository/evidence byte is committed.

## Defense boundary

No new semantic/proof authority was created. Existing fixed conditional budgets, representative H1/CITY/PA calibration, CTX-02 capsule authority, exact-HEAD external CLEAN, same-SHA terminal retry and B1 DocSync/current-state derivation remain unchanged except where the effective-read discovery must consume their existing repository authority.

The repair deliberately does not attempt universal parsing of arbitrary prose or hypothetical future storage systems. The closed property is narrower: **repository-backed context made mandatory by a recognized profile/route read surface must enter the effective set automatically, or an unrecognized class/surface must fail closed.**

## Terminal requirement

Before final CLEAN: execute the full exact-SHA suite after all repository/evidence mutations, inspect the complete baseline→HEAD diff, re-run the class controls, then perform the Worker pre-review against that immutable HEAD. Final CLEAN remains external GitHub metadata tied to the exact SHA.
