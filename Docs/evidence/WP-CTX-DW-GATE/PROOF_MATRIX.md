# WP-CTX-DW-GATE — Proof Matrix

FOUNDATIONAL_PROOF_VERDICT: **REPAIRED / PENDING FINAL EXACT-SHA CLOSURE**
UNRESOLVED_PROOF_OBLIGATIONS: **0 semantic blockers in Worker repair precheck**
BOUNDED_SCENARIOS: **10**
DISCOVERABILITY_REGRESSIONS: **0**
DETERMINISTIC_RESULT_DIGEST: `4be0afefe56592a0fc10822a8167f4489492e848f9f576c9cf502745e032e2f6`
PROJECTION_SOURCE_SHA256: `4460f9784854d1f8afaf95cd3364dd2afb4d6df39d2b5942fcb0def821020683`

This remains bounded proof evidence for CTX↔DW composition only. It is not product/runtime code, a general DW-adoption decision, an H1 product oracle, or H1-GATE public-client evidence.

## Repair principle

The failed candidate proved an internally consistent model by reading its answers from `FALSIFICATION_SUITE.json`. The repaired v2 suite is deliberately weaker as an oracle and stronger as a falsifier: it may declare stimuli and expected route outcomes, but it is forbidden to declare authority contents, material blockers, lifecycle truth or context-unit counts. `scripts/ctx-dw-gate-proof.py` rejects the old `authority_records`, `baseline_material_blockers`, `initial_context_units` and `dw.lifecycle` answer fields.

Causal truth now comes from three independent surfaces:

1. live repository workpack authority plus the accepted CTX-03 dependency/read-set resolver;
2. `H1_PROJECTION_SOURCE.json`, an independently enumerated synthetic H1-like source universe;
3. `H1_PROJECTION.json`, a separate concrete generic facts/relations/provenance projection checked and rebuilt from that source universe.

The synthetic H1-like universe is intentionally small because H1-04 has not yet accepted a real catalogue universe. It is gate evidence only and cannot become H1 product authority.

## Contract exercised

| # | causal class | computed behavior | result |
|---|---|---|---|
| 1 | hidden materiality | compact input contains only `WP-H1-05`; the accepted CTX-03 dependency resolver derives `WP-H1-04` from the real `Depends on:` authority; an independent parser derives the baseline dependency separately; H1-04's real current status yields `dependency-not-accepted:WP-H1-04` | GREEN |
| 2 | CTX↔DW contradiction | a real projected catalogue fact is queried; its provenance source is opened instead of voting between compact surfaces | GREEN |
| 3 | stale DW | corrupting the recorded source fingerprint makes computed staleness/rebuild RED and rejects `USE`; CTX authority discovery remains intact | GREEN |
| 4 | DW abstention | H1-03/03A-like route remains `NOT_MATERIAL` and no DW query/corpus load occurs | GREEN |
| 5 | material DW | concrete fact and relation queries return DW records with provenance and source-open the represented source | GREEN |
| 6 | negative claim | absence is checked against the independent source universe, not the compact projection, and the source universe is opened | GREEN |
| 7 | domain-leakage differential | alpha-renamed diagnostic vocabulary leaves routing invariant | GREEN |
| 8 | false-positive control | opaque CITY/PA/H1-like vocabulary cannot manufacture `USE` | GREEN |
| 9 | H1 projection bootstrap | empty pre-bootstrap projection fails computed completeness/provenance/rebuild; deterministic rebuild from the source universe makes the same query eligible without mutating H1 authority | GREEN |
| 10 | routing-authority conflict | DW `NOT_MATERIAL` cannot suppress the H1-04 read derived by accepted CTX dependency mechanics | GREEN |

## Non-circular CTX discoverability

Scenario 01 no longer contains `WP-H1-04` in a fixture-owned `claim_required_sources` list. Its compact start is only:

- `Docs/workpacks/H1/WP-H1-05.md`.

The harness imports `scripts/ctx03-dynamic-context-check.py` and exercises its accepted `direct_dependency_contracts()` mechanic against the actual H1-05 bytes. That discovers `Docs/workpacks/H1/WP-H1-04.md` from the real `Depends on: WP-H1-04 PASS` line.

The comparison oracle is separate code that independently parses the authoritative H1-05 dependency declaration. It does not consume the CTX result. The blocker is then derived from the actual H1-04 `Status:` line rather than from the falsification JSON. On this candidate the authority is `PLANNED / NOT_STARTED`, therefore the bounded blocker is `dependency-not-accepted:WP-H1-04`.

Two adversarial checks bind the result causally:

- suppressing the CTX-derived dependency leaves the independently derived baseline source missing and turns the audit RED;
- mutating the H1-04 authority status to accepted in the isolated control removes the derived blocker, proving the blocker is content-driven rather than fixture-driven.

## Computed H1→DW lifecycle

`H1_PROJECTION_SOURCE.json` independently enumerates 3 H1-like records and 2 relationships. `H1_PROJECTION.json` contains the separately materialized generic projection: **9 facts + 2 relations**, each carrying source path, exact source SHA-256 and source record identity.

The harness computes all admission predicates from artifacts:

| predicate | computation |
|---|---|
| independent universe | source and projection are distinct artifacts; source schema contains independently enumerated records + relations |
| projection owner/schema | concrete adapter ID + projection schema must match the declared source projection schema |
| completeness | exact independently rebuilt fact tuples and relation tuples, including cardinality/targets, must equal the stored projection |
| provenance | every fact/relation must carry the exact source path, current source SHA-256 and non-empty source record identity |
| staleness | stored source identity/fingerprint must equal current source bytes |
| rebuild | deterministic normalized rebuild from the independent source must exactly reproduce the stored projection |

No lifecycle predicate can be supplied by the suite. Scenario 09 empties the real projected facts/relations and is rejected; its after-rebuild path calls the actual deterministic builder and re-evaluates the same predicates rather than flipping booleans.

## Selective DW utility is exercised

Scenario 05 performs two materially different generic queries against the concrete projection:

- **catalogue/identity fact** — query `catalogue:market-prefab / logical_id`; observed DW record includes `qsrc.market.prefab` plus exact provenance; compact query payload = **348 bytes** versus **1204 bytes** for the independent source artifact;
- **asset/prefab relation** — query `derived-from` from `asset:market-managed-variant`; observed relation targets `catalogue:market-prefab` and carries exact provenance; compact query payload = **380 bytes** versus **1204 bytes** for the independent source artifact.

`dw_loaded=true` is set only after a concrete query returns a record from a lifecycle-GREEN projection. The decision-critical record source is then opened from the returned provenance. The byte comparison is derived from the actual serialized query result and source artifact; it is only a bounded initial-context comparison, not a token benchmark or generalized savings claim.

An empty projection cannot produce the same PASS: it fails lifecycle admission before either material query can succeed.

## Adversarial controls

The harness executes these controls independently of fixture labels, and all must be true before the suite can return GREEN:

- baseline lifecycle computed GREEN;
- empty DW → RED;
- relation target corruption → RED;
- provenance fingerprint corruption → RED;
- stored source fingerprint staleness → RED;
- independent source/authority mutation with unchanged projection → RED;
- deterministic rebuild A == rebuild B;
- real H1-04 status content changes the authoritative blocker;
- suppressing the CTX-derived H1-04 dependency is detected against the independent baseline.

This directly answers the failure-mode question from review: a DW with no facts/relations, a projection with forged provenance, or an authority/source whose bytes changed cannot remain GREEN merely because a fixture says lifecycle is valid.

## Reused predecessor evidence

No DW-04 paired-agent campaign, model call or Unity run is repeated. The harness fails closed unless these accepted predecessor surfaces retain their accepted markers:

- `Docs/workpacks/CTX/WP-CTX-03.md`;
- `Docs/evidence/CTX-03/DOCSYNC.md`;
- `Docs/workpacks/DW/WP-DW-GATE.md`;
- `Docs/evidence/WP-DW-GATE/DOCSYNC.md`.

No product/runtime, H0, H1 semantic, CITY, PA, CTX predecessor or DW predecessor implementation is modified by this repair.

## H1/H2 boundary preserved

- H1-03 and H1-03A remain unblocked and normally `NOT_MATERIAL` for DW.
- H1-04 remains source-first and does not consume this synthetic gate projection as product knowledge.
- post-H1-04 real projection admission still requires the full lifecycle as a separate non-product boundary.
- H1-05/H1-06 remain eligible real consumers only after that real projection exists and is current/complete; authority fallback remains legal.
- nothing here pre-seeds or satisfies the mandatory H1-GATE public-client trial.
- future H2 may consume an accepted result as knowledge/context-portability input, not product-portability proof.

## Reproduction

```bash
python3 scripts/ctx-dw-gate-proof.py
python3 scripts/ctx-dw-gate-proof.py --json
```

Expected repaired headline before exact-SHA closure:

```text
CTX_DW_GATE_GREEN cases=10 material_variants=2 abstentions=1 facts=9 relations=2 discoverability_regressions=0 digest=4be0afefe56592a0fc10822a8167f4489492e848f9f576c9cf502745e032e2f6
```
