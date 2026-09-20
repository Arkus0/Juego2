# WP-HK-08B — Structured conflict recovery + agent interaction benchmark

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-08A`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Make ordinary authoring failures cheap for an AI to recover from and measure the complete client workflow under explicit interaction budgets, consuming the accepted HK08A efficiency primitives rather than redesigning them.

## Acceptance

- A stale-revision/hash conflict returns stable machine-readable recovery context anchored to both the request's expected base and the current authored revision/hash.
- When the expected base is provably an ancestor in the current local authored lineage and required accepted history is available, recovery context includes enough deterministic changed-resource/current-resource information for a client to preserve intent and re-plan only what is affected rather than reconstructing the complete world.
- If the expected base is not provably recoverable from the current local authored lineage (for example across an unrelated/new snapshot-rebase lineage or unavailable history), the harness does not fabricate a delta. It returns a stable machine-readable disposition explaining that bounded reinspection is required.
- Conflict recovery is explicit re-planning, not implicit merge: the client submits a new normal dry-run/apply request against the new base and all accepted validation, atomicity, idempotency and provenance semantics still apply.
- Structured diagnostics may expose deterministic prioritization/grouping or a likely first repair when useful, but compact/priority presentation cannot suppress independent HK05 diagnostics. The complete machine-actionable diagnostic set remains retrievable.
- A reference micro-world authoring benchmark measures request count, response bytes and elapsed harness time for representative create/inspect/modify/invalid-repair/conflict-recovery flows using the accepted HK08A batch/compact/pagination primitives.
- The same-lineage stale-plan benchmark demonstrates stale rejection → structured recovery → re-plan → successful retry without a full-world reload in the ordinary representative case.
- Baseline interaction budgets are recorded from evidence rather than guessed; regressions above explicit thresholds fail CI or require a reviewed budget update.
- The representative benchmark proves that the coherent multi-resource edit accepted by HK08A remains one atomic transaction and does not degenerate into pathological per-field round trips.
- Recovery/diagnostic ergonomics are semantically equivalent across the accepted reference transport and MCP projection. Cross-transport conformance is rerun for every HK08B-added or changed semantic.
- Recovery paths cannot bypass canonical validation/provenance, invent an alternate mutation authority or change request/result/error meaning between transports.

## Required negative-conformance tests

RED→GREEN for:

- recoverable same-lineage stale conflict returning only an opaque/generic error that forces full-world reconstruction;
- recovery context anchored to the wrong expected/current revision or omitting a material changed resource;
- non-ancestor/unavailable-history conflict falsely claiming a trustworthy delta;
- recovery retry bypassing normal validation/provenance or mutating through an alternate authority;
- diagnostic prioritization hiding an independent HK05 diagnostic;
- benchmark silently omitting a required representative flow or measuring a path different from the public accepted client surface;
- pathological round-trip/response-volume regression beyond the reviewed budget; and
- reference-transport versus MCP semantic drift for HK08B recovery/repair behaviour.

## Explicit concurrency boundary

H0 continues to use the accepted whole-world revision/hash CAS. HK08B makes ordinary same-lineage stale conflicts cheap to recover from; it does **not** add per-resource locks, automatic merge of disjoint writers, distributed transactions, multi-process writer coordination or autonomous multi-agent scheduling.

Those mechanisms remain post-`WP-HK-GATE` product work unless measured HK08B/GATE evidence proves that the representative single-client authoring contract cannot meet its accepted interaction budget without them. A theoretical future multi-agent scenario is not sufficient evidence.

## Forbidden scope

Per-resource/distributed locking, automatic merge of concurrent writers, multi-agent orchestration, multi-process writer coordination, gameplay content, model-vendor prompt engineering, natural-language planner/orchestrator, or speculative multi-plan transactional machinery.

## DoD

A measured reference client can complete representative authoring and repair flows efficiently through the accepted HK08A primitives, recover an ordinary same-lineage stale plan through bounded structured context without full-world reconstruction, fail truthfully when ancestry/history is insufficient for such a delta, and obtain semantically equivalent recovery behaviour through reference transport and MCP; recorded interaction budgets and independent PASS.
