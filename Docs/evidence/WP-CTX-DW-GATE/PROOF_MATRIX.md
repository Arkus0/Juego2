# WP-CTX-DW-GATE — Proof Matrix

FOUNDATIONAL_PROOF_VERDICT: **REPAIRED / PENDING FINAL EXACT-SHA CLOSURE**  
UNRESOLVED_PROOF_OBLIGATIONS: **0 semantic blockers in Worker repair precheck**  
BOUNDED_SCENARIOS: **10**  
DISCOVERABILITY_REGRESSIONS: **0**  
DETERMINISTIC_RESULT_DIGEST: `5222f4752cd34bc4d7a2f3a6b161de708153138ec42638f168059d8c738536b9`  
PROJECTION_SOURCE_SHA256: `4460f9784854d1f8afaf95cd3364dd2afb4d6df39d2b5942fcb0def821020683`

This remains bounded proof evidence for CTX↔DW composition only. It is not product/runtime code, a general DW-adoption decision, an H1 product oracle, or H1-GATE public-client evidence.

## Repair principle

The v3 falsification suite may declare stimuli and expected route outcomes, but it cannot declare contradiction truth, authority contents, lifecycle truth, material blockers, context-unit counts, or precomputed materiality for the structural routing probes. Causal truth is computed from repository authorities, the accepted CTX resolver, the source/projection artifacts, and structural signal fields.

The three previously repaired classes remain intact: real H1-05 → H1-04 discoverability through accepted CTX dependency mechanics plus an independent parser/oracle; H1→DW lifecycle computed from separate source/projection artifacts; and two material structured DW query shapes with provenance and artifact-derived context measurement.

## Current independent-review repair

| causal class | behavioral proof | result |
|---|---|---|
| CTX↔DW contradiction | a compact CTX surface is independently built from the source universe, then the fixture mutates a real fact value. The DW projection remains lifecycle-GREEN. The harness queries both representations and compares content. Mismatch yields `CONTRADICTION_SOURCE_OPEN` + fail-closed; removing the mutation yields ordinary `USE`. A separate relation-target mutation is also detected. | GREEN |
| negative claim | an intentionally incomplete compact projection omits the queried result. For a real authority record (`catalogue:market-material/logical_id`) the compact view appears absent, authority returns the counterexample, status becomes `REFUTED_BY_AUTHORITY`, and `claim_closed=false`. A paired truly absent record becomes `CONFIRMED_BY_AUTHORITY` with `claim_closed=true`. | GREEN |
| domain leakage differential | routing advice is derived from structural semantics (`decision_critical`, `structured_lookup`) rather than text. Adopted CITY vocabulary and alpha-renamed OMEGA vocabulary traverse the same route and remain equivalent. An injected domain-label-dependent policy makes the differential diverge and is required to turn RED. | GREEN |
| false-positive | opaque text containing CITY, PA, Quaternius, finding, disposition, required-role, allocated and prefab traverses the same structural classifier and remains `NOT_MATERIAL`. Removing the text leaves the route unchanged. An injected text-presence dependency makes the differential diverge and is required to turn RED. | GREEN |

## Contradiction is detected, not declared

Scenario 02 contains no `contradiction: true` answer field. The compact CTX representation uses a different schema from the DW projection and is built independently from the same source universe. The scenario stimulus mutates `catalogue:market-prefab / logical_id` to `ctx-mutated.market.prefab`; the lifecycle-GREEN DW row remains `qsrc.market.prefab`. `route()` detects the mismatch by comparing the queried rows; that comparison drives source-open/fail-closed.

Causal controls prove mutation present → contradiction; mutation removed → no contradiction and `USE`; and a separate `derived-from` relation-target corruption → contradiction, so the detector is not hard-wired to the fact example.

## Negative-claim falsification by authority

Scenario 06 executes two worlds through the same logic. The compact view is deliberately incomplete and has no completeness oracle sufficient to close an absence, so authority is queried. A false absence for `catalogue:market-material / logical_id` is refuted by a concrete authority counterexample and cannot close. A truly absent `catalogue:missing-entry / logical_id` is confirmed. The authority result therefore changes the result rather than being discarded.

## Structural domain-leakage / false-positive seam

Cases 07/08 no longer use unused diagnostic fields and no longer predeclare `dw.advice`. `route()` computes advice structurally: both decision-critical + structured lookup → `USE`; exactly one → `OPTIONAL`; neither → `NOT_MATERIAL`. Surface labels/text travel through the same route but are not semantic inputs.

The controls do not use a token blacklist/whitelist. Instead they mutate routing behavior: a deliberately domain-label-bound policy makes CITY vs alpha-renamed OMEGA diverge, and a deliberately text-presence-bound policy makes opaque text affect materiality. Both must be detected RED.

## Preserved lifecycle and selective-utility evidence

`H1_PROJECTION_SOURCE.json` remains the independent 3-record/2-relation source universe. `H1_PROJECTION.json` remains the separate 9-fact/2-relation generic projection with exact source fingerprint and per-row provenance. Admission still requires exact independently rebuilt facts/relations/cardinality/targets, provenance, current source fingerprint and deterministic rebuild parity.

Scenario 05 still performs both material shapes: fact `catalogue:market-prefab / logical_id`, and relation `derived-from` from `asset:market-managed-variant`. `dw_loaded=true` remains impossible without an actual lifecycle-GREEN result; provenance drives source-open; context comparison remains artifact-derived.

## Adversarial controls

All previous lifecycle/discoverability corruption controls remain required, plus: equivalent relation contradiction detected; alpha-renaming structural invariance; injected domain-label dependency detected RED; opaque text structurally ignored; injected text dependency detected RED. The negative-claim pair must produce both `REFUTED_BY_AUTHORITY` and `CONFIRMED_BY_AUTHORITY`.

## Reused predecessor evidence and boundaries

No DW-04 model campaign or Unity run is repeated. No product/runtime, H0, H1 semantic, CITY, PA, CTX predecessor or DW predecessor implementation is modified. H1-03/H1-03A remain unblocked; H1-04 remains source-first; post-H1-04 projection admission still requires the full lifecycle separately; H1-GATE public-client trial remains separate.

## Reproduction

```bash
python3 scripts/ctx-dw-gate-proof.py
python3 scripts/ctx-dw-gate-proof.py --json
```

Expected deterministic headline before final exact-SHA closure:

```text
CTX_DW_GATE_GREEN cases=10 material_variants=2 negative_variants=2 abstentions=1 facts=9 relations=2 discoverability_regressions=0 digest=5222f4752cd34bc4d7a2f3a6b161de708153138ec42638f168059d8c738536b9
```
