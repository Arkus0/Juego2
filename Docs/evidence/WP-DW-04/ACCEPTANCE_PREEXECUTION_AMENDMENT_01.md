# DW-04 acceptance pre-execution amendment 01 — selector uniqueness

Status: mechanical pre-execution correction; no acceptance provider/model call has occurred.

The first deterministic post-freeze assembly attempt (GitHub Actions run `35848658608`) stopped before context measurement and before any model call because the CTX `A-CITY-02` selector `| `loc.puerto.landing` |` matched both the authoritative §4 programme row and the later §5 systemic-use-profile row.

This amendment changes exactly one extraction needle in `ACCEPTANCE_CONTEXT_PLAN.json` so it uniquely selects the already intended §4 programme row by including the §4 row's `Puerto Fluvial` and `X6/X7 landing / upstream port threshold` columns. It does not change the selected task, semantic question, authority file, expected facts/blockers/verdict/evidence, CTX meaning, DW query/fallback meaning, model/provider/configuration, answer schema, scorer, 36-slot order, correctness rule, byte threshold, or acceptance retry policy.

The failed assembly produced no `CONTEXT_ASSEMBLY.json`, no scorable route result, no provider request ID and no acceptance model observation. Therefore the correction is independent of acceptance outcome. The superseded freeze remains in Git history; a new freeze re-content-addresses the corrected plan before assembly is attempted again. This is a Worker/owner-directed mechanical pre-execution amendment, not an independent Reviewer approval; the later Reviewer must assess its acceptability.
