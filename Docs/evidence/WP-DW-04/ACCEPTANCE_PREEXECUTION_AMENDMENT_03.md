# DW-04 acceptance pre-execution amendment 03 — compact PA navigation + source-open

Status: `OWNER_AUTHORIZED / PRE_EXECUTION / 0_OF_36`.

This is a plan-level pre-execution amendment recorded after deterministic context assembly and before any acceptance provider/model call. It is **not** independent Reviewer approval.

## Trigger

Freeze `1847c45f953bfea0aabebeba0a1eb76e9d4a9a6d` materialized all six CTX/DW routes successfully in Actions run `35849847978`, job `107144665438`, but the pre-call byte guard measured median CTX `2258.5` bytes and median DW `1926.5` bytes: only `14.7%` reduction, below the unchanged `30%` acceptance threshold. The job failed before committing `CONTEXT_ASSEMBLY.json` and before any provider/model execution. Acceptance usage therefore remains `0/36`.

Per-task measurements from that failed preflight were:

- `A-CITY-01`: CTX 1928 / DW 1062 (44.9% reduction)
- `A-CITY-02`: CTX 2217 / DW 1352 (39.0%)
- `A-CITY-03`: CTX 1911 / DW 1022 (46.5%)
- `A-PA-01`: CTX 2885 / DW 2501 (13.3%)
- `A-PA-02`: CTX 5438 / DW 5020 (7.7%)
- `A-PA-03`: CTX 2300 / DW 2829 (-23.0%)

The structural materializer itself was GREEN. The cause is therefore route payload inflation in the PA treatment, not missing authority or model behaviour: `pa-fixture`/`pa-finding` query fragments injected full PA material bodies even when DW was only being used to locate a record/provenance before opening exact accepted source bytes.

## Owner authorization

PR #150 comment `5793371241` records the repository owner's authorization for one pre-call route-instrument amendment while preserving the six tasks, source truth/oracles, CTX route, model/configuration, scorer semantics, 18 pairs / 36 slots, 100% correctness and the 30% threshold. That authorization explicitly remains non-independent evidence.

## Amendment

The PA read-only DW-04 adapter continues to call the accepted DW-03 `PaDesignWorldProvider` and the same `Fixture` / `ByPa(..., "finding")` query surfaces, but serializes only a generic compact navigation record: `FactId`, `PaId`, `RecordKind`, `SourceKey`, `SourcePath` and `Anchor`. It intentionally stops injecting `MaterialText`, `DispositionText` and digest-heavy provenance into the model context. Accepted PA documents remain the sole semantic authority.

The DW context plan then uses exact source-open-on-demand for the semantic bytes actually required by the frozen oracle:

- `A-PA-01`: compact `pa04/NC-02` fixture index + the already frozen three exact PA-04 source lines.
- `A-PA-02`: compact `pa05/NC-02` fixture index + exact NC-02 heading, lineage disposition row and receiver-ownership line.
- `A-PA-03`: one compact `pa01/DL-11` finding index as navigation + exact source rows for DL-11 through DL-14, rather than four verbose finding bodies.

CITY routes are unchanged. CTX contexts are unchanged. Every required source literal remains checked on both routes. Query output remains deterministic and provenance-bound; source fallback remains bound to the accepted Git blob.

## Frozen invariants

This amendment does **not** change:

- `A-CITY-01..03` / `A-PA-01..03` task identities or semantic questions;
- accepted source files, blobs, expected facts, blockers, verdicts or evidence IDs;
- the calibration READY result or Luna/OpenAI route;
- system prompt / response schema / scorer semantics;
- pair identities/order or 36 designated executions;
- invalid-run / semantic-rerun policy;
- CTX baseline meaning;
- 100% correctness requirement;
- `30%` median injected-source-byte reduction requirement.

A new acceptance freeze must content-address the amended context plan and compact retrieval adapter before deterministic assembly is rerun. If the new assembly still misses structural completeness or the unchanged 30% threshold, no acceptance call is authorized.
