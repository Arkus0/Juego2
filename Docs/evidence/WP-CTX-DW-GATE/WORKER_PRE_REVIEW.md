# WP-CTX-DW-GATE — Worker pre-review

PREDECESSOR_CONTRACT_CHECK: GREEN
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 3
LATEST_INDEPENDENT_REVIEW: `#5295415742`
REVIEWED_FAILED_CANDIDATE: `c26124798399668996bc2a078c38ecf6cb4bfdfe`

Accepted predecessors remain unchanged and are checked by the executable gate:

- `Docs/workpacks/CTX/WP-CTX-03.md` is COMPLETE and `Docs/evidence/CTX-03/DOCSYNC.md` is complete.
- `Docs/workpacks/DW/WP-DW-GATE.md` is COMPLETE and `Docs/evidence/WP-DW-GATE/DOCSYNC.md` is complete.

## Independent FAIL repair

The repair closes the three proof-level blockers without changing the accepted CTX→selective-DW→authority architecture.

1. **Non-circular discoverability.** Scenario 01 starts only from the real H1-05 contract. The harness imports the accepted CTX-03 dependency/read-set resolver and derives H1-04 from H1-05's actual `Depends on:` declaration. A separate parser independently derives the authoritative dependency baseline, and the blocker comes from H1-04's real `Status:` content rather than fixture-owned `authority_records`.
2. **Computed H1→DW lifecycle.** A separate H1-like source universe and concrete generic projection now exist as artifacts. Independent universe, owner/schema, completeness, provenance, staleness and deterministic rebuild are computed from their records, relations, cardinality/targets and exact source SHA-256. Scenario 09 empties the actual projection and the after-rebuild path runs the deterministic builder; no lifecycle booleans are trusted.
3. **Exercised selective utility.** Scenario 05 performs one concrete fact query and one distinct relation query against the projection. `dw_loaded=true` is reachable only after an actual record is returned from lifecycle-GREEN data; the returned provenance source is then opened. Context comparison is derived from serialized query/source bytes rather than fixture counters.

## Adversarial self-falsification

The repaired harness must turn RED for all of these causal corruptions before it can return GREEN:

- empty DW facts/relations;
- relation-target corruption;
- provenance fingerprint corruption;
- stale stored source fingerprint;
- independent source/authority mutation without projection rebuild;
- suppression of the CTX-derived H1-04 dependency.

It also requires deterministic rebuild parity and proves that changing the real H1-04 status content changes the derived authoritative blocker.

The v2 falsification suite is stimulus-only for causal truth: `authority_records`, `baseline_material_blockers`, `initial_context_units` and fixture-owned `dw.lifecycle` are forbidden.

## Bounded result

Worker repair precheck over the deterministic gate logic yields 10 bounded causal classes, two material DW query shapes, one explicit abstention control, 9 projected facts, 2 projected relations, and zero bounded discoverability regressions. The exact candidate is still subject to repository-owned exact-SHA validation before review-ready handoff.

Scope remains non-product and bounded. H1-03/H1-03A stay unblocked, H1-04 stays source-first, no Unity/model campaign is run, and H1-GATE public-client evidence is not supplied or satisfied by this gate.
