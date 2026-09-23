# WP-CTX-DW-GATE — Worker pre-review

PREDECESSOR_CONTRACT_CHECK: GREEN  
WORKER_PRE_REVIEW: CLEAN  
WORKER_PRE_REVIEW_FINDINGS_FIXED: 6  
LATEST_INDEPENDENT_REVIEW: `#5296042170`  
REVIEWED_FAILED_CANDIDATE: `3d6f21cca7d3325f2991948969bb51f44a2bd249`

Accepted predecessors remain unchanged and are checked by the executable gate:

- `Docs/workpacks/CTX/WP-CTX-03.md` is COMPLETE and `Docs/evidence/CTX-03/DOCSYNC.md` is complete.
- `Docs/workpacks/DW/WP-DW-GATE.md` is COMPLETE and `Docs/evidence/WP-DW-GATE/DOCSYNC.md` is complete.

## Preserved repairs

The repair does not reopen the three classes accepted by review #5296042170: real H1-05→H1-04 discoverability; computed H1→DW lifecycle from separate artifacts; and two distinct structured DW query shapes with provenance and artifact-derived context measurement.

## New causal repairs

1. **Contradiction detection is data-driven.** The fixture no longer declares `contradiction: true`. A compact CTX representation and the DW projection are queried and their real values/targets compared. A fact-value mutation yields contradiction/fail-closed; removing it removes the contradiction route. A separate relation-target corruption is also detected.
2. **Negative-claim authority result is causal.** The compact view intentionally omits a real authority record. Authority lookup returns that record as a counterexample, produces `REFUTED_BY_AUTHORITY`, and prevents claim closure. A paired truly absent record produces `CONFIRMED_BY_AUTHORITY` and is distinguishable.
3. **Domain leakage / false-positive probes traverse routing.** Cases 07/08 no longer use decorative text fields or fixture-owned advice. Routing advice is computed from structural semantics. Adopted and alpha-renamed vocabulary stay equivalent; opaque adopted vocabulary alone remains `NOT_MATERIAL`. Deliberately injected domain-label and text-presence dependencies are both required to turn the differential RED.

## Adversarial self-falsification

The deterministic harness requires all previous corruption controls plus equivalent relation contradiction detection, structural alpha-renaming invariance, injected domain-label dependency RED, opaque text present/absent invariance, and injected text dependency RED. The false-negative absence pair must yield both `REFUTED_BY_AUTHORITY` and `CONFIRMED_BY_AUTHORITY`; the harness fails if those worlds collapse.

## Bounded result

Worker precheck reproduces 10 bounded causal classes, 2 material DW query variants, 2 negative-claim worlds, one explicit abstention control, 9 projected facts, 2 projected relations, and zero bounded discoverability regressions. Deterministic result digest on the current source/projection artifacts is `5222f4752cd34bc4d7a2f3a6b161de708153138ec42638f168059d8c738536b9`.

Scope remains non-product and bounded. H1-03/H1-03A stay unblocked, H1-04 stays source-first, no Unity/model campaign is run, and H1-GATE public-client evidence is not supplied or satisfied by this gate. Final repository-owned exact-SHA validation is still required before independent review.
