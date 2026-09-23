# WP-CTX-DW-GATE — Proof Matrix

FOUNDATIONAL_PROOF_VERDICT: **READY_FOR_INDEPENDENT_REVIEW**
UNRESOLVED_PROOF_OBLIGATIONS: **0**
BOUNDED_SCENARIOS: **10**
DISCOVERABILITY_REGRESSIONS: **0**
DETERMINISTIC_RESULT_DIGEST: `af830fc4835f802bd0bed3d5ec284256cac884d36ef55f6b338e68cd33d1318e`

This is bounded proof evidence for CTX↔DW composition only. It is not product/runtime code, a general DW-adoption decision, an H1 product oracle, or H1-GATE public-client evidence.

## Contract exercised

The deterministic harness `scripts/ctx-dw-gate-proof.py` consumes the predeclared independent fixture/oracle surface `FALSIFICATION_SUITE.json`. CTX owns mandatory/claim-driven source routing; DW advice is additive and cannot shrink that set. Semantic facts remain source-authoritative.

| # | causal class | required behavior | result |
|---|---|---|---|
| 1 | hidden materiality | omitted compact clue cannot suppress claim-required authority; blocker remains discoverable | GREEN |
| 2 | CTX↔DW contradiction | do not vote between compact surfaces; source-open | GREEN |
| 3 | stale DW | reject `USE` when any lifecycle predicate is non-current; fallback to authority | GREEN |
| 4 | DW abstention | H1-03/03A-like lifecycle claim stays CTX→authority, `NOT_MATERIAL`, no DW load | GREEN |
| 5 | material DW | valid projection may `USE`, but decision-critical facts source-open through provenance | GREEN |
| 6 | negative claim | incomplete compact universe cannot prove absence; authoritative universe is opened | GREEN |
| 7 | domain-leakage differential | alpha-renaming adopted/diagnostic vocabulary leaves route invariant | GREEN |
| 8 | false-positive control | opaque text resembling CITY/PA/H1 vocabulary cannot manufacture materiality | GREEN |
| 9 | H1 projection bootstrap | pre-lifecycle `USE` rejected; after universe/completeness/provenance/stale/rebuild are GREEN it becomes eligible without changing the product oracle | GREEN |
| 10 | routing-authority conflict | CTX mandatory/claim-required authority wins over DW `NOT_MATERIAL` | GREEN |

## Selective utility without inflated claim

Scenario 5 contains two materially different subprobes:

- catalogue/identity: selective initial units `2` vs bounded baseline `6`;
- asset/prefab relation: selective initial units `2` vs bounded baseline `7`.

These are fixture-local context units used only to prove that the selective route is strictly narrower in the bounded scenarios. They are **not** token-cost measurements and carry no generalized savings claim.

Scenario 4 is the explicit abstention control: `NOT_MATERIAL`, `dw_loaded=false`.

## Causal negative controls

The harness was exercised against four one-variable corruptions of the predeclared suite:

- remove hidden claim-required authority → `CTX_DW_GATE_RED`;
- mark stale projection current without changing expected authority behavior → `CTX_DW_GATE_RED`;
- mark incomplete negative-claim universe complete → `CTX_DW_GATE_RED`;
- mark the pre-bootstrap H1 projection lifecycle fully GREEN → `CTX_DW_GATE_RED`.

These target the routing/lifecycle condition consumed by the harness rather than keyword declarations.

## Reused predecessor evidence

No DW-04 paired-agent campaign is rerun. The harness fails closed unless these accepted predecessor surfaces exist and retain their accepted markers:

- `Docs/workpacks/CTX/WP-CTX-03.md`;
- `Docs/evidence/CTX-03/DOCSYNC.md`;
- `Docs/workpacks/DW/WP-DW-GATE.md`;
- `Docs/evidence/WP-DW-GATE/DOCSYNC.md`.

Every authoritative path named by the suite must also exist in the repository.

## H1/H2 boundary

- H1-03 and H1-03A remain unblocked and normally `NOT_MATERIAL` for DW.
- H1-04 remains source-first and does not consume a fictional pre-existing H1-real projection.
- post-H1-04 projection admission requires the full lifecycle as a separate non-product boundary.
- H1-05/H1-06 are eligible consumers only when that projection is current and complete; authority fallback remains legal.
- nothing here pre-seeds the mandatory H1-GATE public-client trial.
- future H2 may consume an accepted result as knowledge/context-portability input, not product-portability proof.

## Reproduction

```bash
python3 scripts/ctx-dw-gate-proof.py
python3 scripts/ctx-dw-gate-proof.py --json
```

Expected headline:

```text
CTX_DW_GATE_GREEN cases=10 material_variants=2 abstentions=1 discoverability_regressions=0 digest=af830fc4835f802bd0bed3d5ec284256cac884d36ef55f6b338e68cd33d1318e
```
