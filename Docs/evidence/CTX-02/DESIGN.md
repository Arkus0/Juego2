# CTX-02 Design / Coverage Note

Baseline: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Circuit-breaker trust audit: `Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md`  
Current failure lineage: `fail_cycle: 3`

## Chosen boundary

CTX-02 adds a non-authoritative accepted-contract capsule layer with contract-scoped freshness. It does not reuse CTX-01's global-main freshness because an unrelated main commit must not invalidate an immutable accepted predecessor.

After three Reviewer FAILs in the same false-green family, the boundary was re-audited before cycle-3 repair. The result is deliberately narrower than “mechanically prove capsule semantics”. CTX-02 proves source-derived facts in production, uses bounded external test oracles for selected representative semantics, and escalates everything else that needs natural-language judgment.

## Oracle split

### A — production/source-derived invariants

The production checker may prove only facts with deterministic independent oracles:

- accepted identity from the matching canonical external `Docs/workpacks/**/<capsule_id>.md`, including explicit PASS review;
- authoritative source integrity from external path constraints + recomputed Git blob SHA;
- required shape/type invariants;
- exact mandatory source identity where the contract names it (`WP-CITY-03` → `Docs/production/CITY_PRODUCT_SEED.md`);
- PA disposition key/status equality against the canonical accepted result table;
- accepted PA-chain coverage from canonical result files + COMPLETE workpacks rather than the capsule index;
- structural asymmetry (`forward != reverse`) and other deterministic shape constraints.

### B — representative semantic controls

The independent test harnesses own small **test-only** semantic oracles for the actual CTX-02 consumer examples:

- H1 navigation boundary: `WP-HK-GATE`;
- CITY navigation boundary: `WP-CITY-03`;
- cumulative PA start surface: `WP-PA-01` + `WP-PA-02` + `WP-PA-03`, because the representative PA-04 consumer inherits all three.

For those actual indexed capsules the controls validate material content, not merely IDs:

- exported `id -> statement + source_pointer`;
- exclusion/non-claim `id -> statement + source_pointer`;
- reopen-condition content;
- escalation-trigger content;
- PA-03 directional `id -> forward/reverse` values.

The causal controls preserve ID, source pointer and source fingerprints while inverting/inventing statement text, and the representative oracle REDs. Symmetric controls exist for exclusions plus structurally valid substitutions in reopen/escalation/directional fields. PA-01/02 carry the same statement-substitution controls because leaving them outside the cumulative PA consumer boundary would preserve the same false-green class.

The production checker does **not** import these semantic fixtures.

### C — human/escalation-only judgment

CTX-02 does not claim a general solution for natural-language equivalence/completeness. For future capsules or contested nuances, these remain independent-Reviewer/source-reconstruction questions:

- whether arbitrary prose is the best or complete paraphrase of source semantics;
- whether exclusions are exhaustive;
- whether reopen/escalation lists are semantically exhaustive;
- whether a source pointer supports a disputed interpretation;
- whether later evidence truly reopens the predecessor rather than belonging downstream.

Any material ambiguity resolves toward authoritative sources, not capsule prose.

## Self-confirmation closure

Fingerprints are only useful when the chosen source is independently constrained. The repaired design therefore requires:

- `identity_source` to be the matching external canonical workpack;
- `authoritative_sources`, disposition sources and mandatory reads not to point into the capsule layer or CTX-02 generated evidence/protocol as predecessor authority;
- accepted PA `disposition_source` to be the exact canonical `PA-NN.md` result discovered independently by PA-chain coverage;
- representative CITY mandatory read to be the exact product seed, not any arbitrary valid file.

This keeps the production checker source-derived rather than self-confirming.

## Representative coverage

- H1: `WP-HK-GATE` → `WP-H1-02`, boundary capsule only; no bulk H0 migration.
- CITY: `WP-CITY-03` → `WP-CITY-04`, boundary capsule only. `CITY_PRODUCT_SEED.md` is mandatory/non-compressible.
- PA: accepted result capsules for `WP-PA-01`, `WP-PA-02`, `WP-PA-03`. The representative PA consumer inherits the full three-capsule chain; semantic fixtures therefore cover all three current capsules, while PA-03 additionally exercises directional semantics. Future accepted PA results are added during DocSync but arbitrary future prose remains escalation/human territory unless deliberately added to a later bounded representative test.

## PA compression rule

Disposition state is preserved exactly as source data, including compound/multi-state values (`ADOPT ... / LATER ...`) and exclusions (`REJECT`, `REJECT baseline`, `REJECT as authority`). The capsule is not a binary summary.

The PA-03 inherited exclusion `default global/N-hop social traversal to discover targets = REJECT` is deliberately protected because it carries PA-02's bounded-discovery guarantee.

The disposition table is a production oracle only because its source identity is independently anchored to the discovered canonical accepted PA result.

## CITY non-compression rule

The capsule does not contain the playable boundary geometry, streets, parcels, scenarios, seams or measurement pack. Any CITY-04 construction question escalates to the exact `Docs/production/CITY_PRODUCT_SEED.md`. The production checker requires that exact mandatory-read identity for the representative CITY capsule.

## No authority promotion

A production `VALID_NAVIGATION_ONLY` result means only that source-derived/structural invariants passed. CTX-02's repository validation additionally requires the independent representative controls. Neither result means predecessor PASS is re-proved, arbitrary source semantics are proved equivalent, or Reviewer independent judgment is satisfied.

## Proof-budget boundary

The cycle-3 repair is acceptable only while it stays inside this split:

- small deterministic production invariants;
- bounded test-only representative semantic fixtures for the five actual current consumer capsules, not an open-ended production registry;
- no generic semantic registry, NLP/fuzzy equivalence engine or bulk historical migration;
- unresolved arbitrary/future semantics remain escalation/review responsibilities.

The final Worker pre-review must issue an explicit `PROOF_BUDGET` verdict against this boundary before a new exact-SHA freeze is allowed.
