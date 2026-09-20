# WP-HK-08B interaction benchmark

## Measurement boundary

The benchmark is `Hk08BInteractionBenchmarkTests.RepresentativePublicClientFlowMeasuresInteractionCostWithoutWholeWorldConflictReload`. It launches the accepted Release JSONL host as an external process and exercises public canonical capabilities only. The stopwatch begins after process launch so the measurement describes the five authoring/repair flows, not OS/process startup. Serialized response bytes are counted from the canonical neutral responses received by the client.

The five required flows are:

1. create two related objects in one canonical transaction;
2. compact bounded inspection through `world.object.get`;
3. coherent two-resource modification in one canonical transaction;
4. invalid mutation -> complete HK05 diagnostic artifact -> repaired mutation;
5. same-lineage stale plan -> structured recovery -> affected-only inspection -> explicit plan/dry-run/apply retry.

The conflict-recovery phase rejects `world.summary`, broad object query and broad extension query. A changed resource is inspected through the descriptor in `currentResources`; the representative recovery therefore performs no full-world reload.

## First green observation used to freeze budgets

Budgets were not chosen before recovery semantics. The recovery-truth contract was persisted first in `WORKER_PLAN.md`; then candidate `8b04a66079cd8a637f0abd8aa1eeb18cd90d9f99` was observed by GitHub Actions run `35501506939` on `ubuntu-24.04` with the pinned SDK.

Observed metrics:

| Metric | Observation |
|---|---:|
| flow count | 5 |
| request count | 12 |
| serialized response bytes | 12,051 |
| elapsed harness time | 185 ms |
| invalid diagnostic count | 1 |
| recovery changed resources | 1 |
| recovery inspection requests | 1 |
| recovery full-world reloads | 0 |
| coherent multi-resource modify requests | 1 |
| public transport only | true |

The same observation passed focused HK08B `8/8` and full regression `181/181` before the budget constants were frozen.

## Frozen regression budgets

The executable budgets live in `Hk08BInteractionBenchmarkTests` and are derived from that observation:

- request budget: **12 maximum**. There is no growth allowance because an extra public round trip is precisely the regression HK08B is intended to catch;
- serialized-response budget: **15,064 bytes maximum**, a 25% bounded allowance over 12,051 bytes for legitimate additive diagnostic/recovery information;
- elapsed-time guard: **1,480 ms maximum**, exactly 8x the observed 185 ms. This deliberately wide multiplier makes elapsed time a coarse pathological-regression guard rather than a flaky runner-specific latency SLO.

A change that exceeds a budget makes the HK08B test red. Raising a threshold therefore requires a reviewed code/evidence change; CI cannot silently accept it.

## Causal budget controls

`BenchmarkAndDiagnosticCompletenessOraclesTurnRedForForbiddenShortcuts` injects values one unit beyond each frozen maximum and requires the independent budget oracle to report respectively:

- `request-budget-regression`;
- `response-budget-regression`;
- `elapsed-budget-regression`.

The same test also injects private-surface measurement, omitted representative flow, full-world reload, omitted affected-resource inspection and per-resource mutation chatter. Each mutation is required to turn its corresponding oracle red.

## Interpretation

These numbers are H0 reference-client regression budgets, not product performance promises. They prove that the accepted recovery shape is usable without pathological interaction cost. HK09B may add final resource-envelope controls, but it must not reinterpret this recovery truth or treat a budget as justification for fabricating ancestry/delta information.
